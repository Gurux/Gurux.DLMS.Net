//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

namespace Gurux.DLMS
{
    using System;
    using System.Text;
    using Gurux.DLMS.Internal;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.XPath;
    using Gurux.DLMS.Enums;
    using Gurux.DLMS.Secure;
    using Gurux.DLMS.Objects;
    using System.ComponentModel;
    using System.Diagnostics;

    ///<summary>
    ///This class is used to translate DLMS frame or PDU to xml.
    ///</summary>
    public class GXDLMSTranslator
    {
        private SortedList<int, string> tags = new SortedList<int, string>();
        private SortedList<string, int> tagsByName = new SortedList<string, int>();

        /// <summary>
        /// Sending data in multiple frames.
        /// </summary>
        private bool multipleFrames = false;
        /// <summary>
        /// If only PDUs are shown and PDU is received on parts.
        /// </summary>
        private GXByteBuffer pduFrames = new GXByteBuffer();

        /// <summary>
        /// Is only PDU shown when data is parsed with MessageToXml
        /// </summary>
        /// <seealso cref="MessageToXml"/>
        /// <seealso cref="CompletePdu"/>
        [DefaultValue(false)]
        public bool PduOnly
        {
            get;
            set;
        }

        /// <summary>
        ///  Convert string to byte array.
        /// </summary>
        /// <param name="value">Hex string.</param>
        /// <returns>Parsed byte array.</returns>
        [DebuggerStepThrough]
        public static byte[] HexToBytes(string value)
        {
            return GXCommon.HexToBytes(value);
        }

        /// <summary>
        /// Is only complete PDU parsed and shown.
        /// </summary>
        /// <seealso cref="MessageToXml"/>
        /// <seealso cref="PduOnly"/>
        [DefaultValue(false)]
        public bool CompletePdu
        {
            get;
            set;
        }


        /// <summary>
        /// Are numeric values shown as hex.
        /// </summary>
        [DefaultValue(true)]
        public bool Hex
        {
            get;
            set;
        }



        /// <summary>
        /// Is string serialized as hex.
        /// </summary>
        /// <seealso cref="MessageToXml"/>
        /// <seealso cref="PduOnly"/>
        [DefaultValue(false)]
        public bool ShowStringAsHex
        {
            get;
            set;
        }

        public TranslatorOutputType OutputType
        {
            get;
            private set;
        }

        /// <summary>
        /// Is XML declaration skipped.
        /// </summary>
        public bool OmitXmlDeclaration
        {
            get;
            set;
        }

        /// <summary>
        /// Is XML name space skipped.
        /// </summary>
        public bool OmitXmlNameSpace
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">Translator output type.</param>
        public GXDLMSTranslator(TranslatorOutputType type)
        {
            OutputType = type;
            GetTags(type, tags, tagsByName);
            if (type == TranslatorOutputType.SimpleXml)
            {
                Hex = true;
            }
        }

        /// <summary>
        /// Find next frame from the string.
        /// </summary>
        /// <remarks>
        /// Position of data is set to the begin of new frame. If Pdu is null it is not updated.
        /// </remarks>
        /// <param name="data">Data where frame is search.</param>
        /// <param name="pdu">Pdu of received frame is set here.</param>
        /// <returns>Is new frame found.</returns>
        public bool FindNextFrame(GXByteBuffer data, GXByteBuffer pdu)
        {
            GXDLMSSettings settings = new GXDLMSSettings(true);
            GXReplyData reply = new GXReplyData();
            reply.Xml = new GXDLMSTranslatorStructure(OutputType, Hex, ShowStringAsHex, null);
            int pos;
            while (data.Position != data.Size)
            {
                if (data.GetUInt8(data.Position) == 0x7e)
                {
                    pos = data.Position;
                    settings.InterfaceType = Enums.InterfaceType.HDLC;
                    GXDLMS.GetData(settings, data, reply);
                    data.Position = pos;
                    break;
                }
                else if (data.Position + 2 < data.Size && data.GetUInt16(data.Position) == 0x1)
                {
                    pos = data.Position;
                    settings.InterfaceType = Enums.InterfaceType.WRAPPER;
                    GXDLMS.GetData(settings, data, reply);
                    data.Position = pos;
                    break;
                }
                ++data.Position;
            }
            if (pdu != null)
            {
                pdu.Clear();
                pdu.Set(reply.Data.Data, 0, reply.Data.Size);
            }
            return data.Position != data.Size;
        }

        internal static void AddTag(SortedList<int, string> list, Enum value, string text)
        {
            list.Add(Convert.ToInt32(value), text);
        }

        /// <summary>
        /// Get all tags.
        /// </summary>
        /// <param name="type">Output type.</param>
        /// <param name="list">List of tags by ID.</param>
        /// <param name="tagsByName">List of tags by name.</param>
        private static void GetTags(TranslatorOutputType type,
                                    SortedList<int, string> list, SortedList<string, int> tagsByName)
        {
            if (type == TranslatorOutputType.SimpleXml)
            {
                TranslatorSimpleTags.GetGeneralTags(type, list);
                TranslatorSimpleTags.GetSnTags(type, list);
                TranslatorSimpleTags.GetLnTags(type, list);
                TranslatorSimpleTags.GetGloTags(type, list);
                TranslatorSimpleTags.GetTranslatorTags(type, list);
                TranslatorSimpleTags.GetDataTypeTags(list);
            }
            else
            {
                TranslatorStandardTags.GetGeneralTags(type, list);
                TranslatorStandardTags.GetSnTags(type, list);
                TranslatorStandardTags.GetLnTags(type, list);
                TranslatorStandardTags.GetGloTags(type, list);
                TranslatorStandardTags.GetTranslatorTags(type, list);
                TranslatorStandardTags.GetDataTypeTags(list);
            }
            // Simple is not case sensitive.
            bool lowercase = type == TranslatorOutputType.SimpleXml;
            foreach (var it in list)
            {
                string str = it.Value;
                if (lowercase)
                {
                    str = str.ToLower();
                }
                if (!tagsByName.ContainsKey(str))
                {
                    tagsByName.Add(str, it.Key);
                }
            }
        }

        public byte[] GetPdu(byte[] value)
        {
            return GetPdu(new GXByteBuffer(value));
        }

        public byte[] GetPdu(GXByteBuffer value)
        {
            GXReplyData data = new GXReplyData();
            data.Xml = new GXDLMSTranslatorStructure(OutputType, Hex, ShowStringAsHex, tags);
            GXDLMSSettings settings = new GXDLMSSettings(true);
            if (value.GetUInt8(0) == 0x7e)
            {
                settings.InterfaceType = Enums.InterfaceType.HDLC;
            }
            //If wrapper.
            else if (value.GetUInt16(0) == 1)
            {
                settings.InterfaceType = Enums.InterfaceType.WRAPPER;
            }
            else
            {
                throw new ArgumentNullException("Invalid DLMS framing.");
            }
            GXDLMS.GetData(settings, value, data);
            //Only fully PDUs are returned.
            if (data.IsMoreData)
            {
                return null;
            }
            return data.Data.Array();
        }

        /// <summary>
        /// Convert message to xml.
        /// </summary>
        /// <param name="value">Bytes to convert.</param>
        /// <returns>Converted xml.</returns>
        /// <seealso cref="PduOnly"/>
        /// <seealso cref="CompletePdu"/>
        public string MessageToXml(byte[] value)
        {
            return MessageToXml(new GXByteBuffer(value));
        }

        /// <summary>
        /// Convert message to xml.
        /// </summary>
        /// <param name="value">Bytes to convert.</param>
        /// <returns>Converted xml.</returns>
        /// <seealso cref="PduOnly"/>
        /// <seealso cref="CompletePdu"/>
        public string MessageToXml(GXByteBuffer value)
        {
            if (value == null || value.Size == 0)
            {
                throw new ArgumentNullException("value");
            }
            try
            {
                GXReplyData data = new GXReplyData();
                GXDLMSTranslatorStructure xml = new GXDLMSTranslatorStructure(OutputType, Hex, ShowStringAsHex, tags);
                data.Xml = xml;
                //If HDLC framing.
                int offset = value.Position;
                if (value.GetUInt8(value.Position) == 0x7e)
                {
                    GXDLMSSettings settings = new GXDLMSSettings(true);
                    settings.InterfaceType = Enums.InterfaceType.HDLC;
                    if (GXDLMS.GetData(settings, value, data))
                    {
                        if (!PduOnly)
                        {
                            xml.AppendLine("<HDLC len=\"" + (data.PacketLength - offset).ToString("X") + "\" >");
                            xml.AppendLine("<TargetAddress Value=\"" + settings.ServerAddress.ToString("X") + "\" />");
                            xml.AppendLine("<SourceAddress Value=\"" + settings.ClientAddress.ToString("X") + "\" />");
                        }
                        if (data.Data.Size == 0)
                        {
                            if ((data.FrameId & 1) != 0 && data.Command == Command.None)
                            {
                                if (!CompletePdu)
                                {
                                    xml.AppendLine("<Command Value=\"NextFrame\" />");
                                }
                                multipleFrames = true;
                            }
                            else
                            {
                                xml.AppendStartTag(data.Command);
                                xml.AppendEndTag(data.Command);
                            }
                        }
                        else
                        {
                            if (multipleFrames || (data.MoreData & Enums.RequestTypes.Frame) != 0)
                            {
                                if (CompletePdu)
                                {
                                    pduFrames.Set(data.Data.Data);
                                }
                                else
                                {
                                    xml.AppendLine("<NextFrame Value=\"" + GXCommon.ToHex(data.Data.Data, false, data.Data.Position, data.Data.Size - data.Data.Position) + "\" />");
                                }
                                multipleFrames = false;
                            }
                            else
                            {
                                if (!PduOnly)
                                {
                                    xml.AppendLine("<PDU>");
                                }
                                if (pduFrames.Size != 0)
                                {
                                    pduFrames.Set(data.Data.Data);
                                    xml.AppendLine(PduToXml(pduFrames));
                                    pduFrames.Clear();
                                }
                                else
                                {
                                    xml.AppendLine(PduToXml(data.Data));
                                }
                                //Remove \r\n.
                                xml.sb.Length -= 2;
                                if (!PduOnly)
                                {
                                    xml.AppendLine("</PDU>");
                                }
                            }
                        }
                        if (!PduOnly)
                        {
                            xml.AppendLine("</HDLC>");
                        }
                    }
                    return xml.sb.ToString();
                }
                //If wrapper.
                if (value.GetUInt16(value.Position) == 1)
                {
                    GXDLMSSettings settings = new GXDLMSSettings(true);
                    settings.InterfaceType = Enums.InterfaceType.WRAPPER;
                    GXDLMS.GetData(settings, value, data);
                    if (!PduOnly)
                    {
                        xml.AppendLine("<WRAPPER len=\"" + (data.PacketLength - offset).ToString("X") + "\" >");
                        xml.AppendLine("<TargetAddress Value=\"" + settings.ClientAddress.ToString("X") + "\" />");
                        xml.AppendLine("<SourceAddress Value=\"" + settings.ServerAddress.ToString("X") + "\" />");
                    }
                    if (data.Data.Size == 0)
                    {
                        xml.AppendLine("<Command Value=\"" + data.Command.ToString().ToUpper() + "\" />");
                    }
                    else
                    {
                        if (!PduOnly)
                        {
                            xml.AppendLine("<PDU>");
                        }
                        xml.AppendLine(PduToXml(data.Data));
                        //Remove \r\n.
                        xml.sb.Length -= 2;
                        if (!PduOnly)
                        {
                            xml.AppendLine("</PDU>");
                        }
                    }
                    if (!PduOnly)
                    {
                        xml.AppendLine("</WRAPPER>");
                    }
                    return xml.sb.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
            }
            throw new ArgumentNullException("Invalid DLMS framing.");
        }

        /// <summary>
        /// Convert hex string to xml.
        /// </summary>
        /// <param name="hex">Converted hex string.</param>
        /// <returns>Converted xml.</returns>
        public string PduToXml(string hex)
        {
            return PduToXml(GXCommon.HexToBytes(hex));
        }

        /// <summary>
        /// Convert bytes to xml.
        /// </summary>
        /// <param name="value">Bytes to convert.</param>
        /// <returns>Converted xml.</returns>
        public string PduToXml(byte[] value)
        {
            return PduToXml(new GXByteBuffer(value));
        }

        private void GetUa(GXByteBuffer data, GXDLMSTranslatorStructure xml)
        {
            xml.AppendStartTag(Command.Ua);
            data.GetUInt8(); // Skip FromatID
            data.GetUInt8(); // Skip Group ID.
            data.GetUInt8(); // Skip Group len
            Object val;
            while (data.Position < data.Size)
            {
                HDLCInfo id = (HDLCInfo)data.GetUInt8();
                short len = data.GetUInt8();
                switch (len)
                {
                    case 1:
                        val = data.GetUInt8();
                        break;
                    case 2:
                        val = data.GetUInt16();
                        break;
                    case 4:
                        val = data.GetUInt32();
                        break;
                    default:
                        throw new GXDLMSException("Invalid Exception.");
                }
                // RX / TX are delivered from the partner's point of view =>
                // reversed to ours
                switch (id)
                {
                    case HDLCInfo.MaxInfoTX:
                        xml.AppendLine("<MaxInfoRX Value=\"" + val.ToString() + "\" />");
                        break;
                    case HDLCInfo.MaxInfoRX:
                        xml.AppendLine("<MaxInfoTX Value=\"" + val.ToString() + "\" />");
                        break;
                    case HDLCInfo.WindowSizeTX:
                        xml.AppendLine("<WindowSizeRX Value=\"" + val.ToString() + "\" />");
                        break;
                    case HDLCInfo.WindowSizeRX:
                        xml.AppendLine("<WindowSizeTX Value=\"" + val.ToString() + "\" />");
                        break;
                    default:
                        throw new GXDLMSException("Invalid UA response.");
                }
            }
            xml.AppendEndTag(Command.Ua);
        }

        /// <summary>
        /// Convert bytes to xml.
        /// </summary>
        /// <param name="value">Bytes to convert.</param>
        /// <returns>Converted xml.</returns>
        public string PduToXml(GXByteBuffer value)
        {
            return PduToXml(value, OmitXmlDeclaration, OmitXmlNameSpace);
        }

        private string PduToXml(GXByteBuffer value, bool omitDeclaration, bool omitNameSpace)
        {
            if (value == null || value.Size == 0)
            {
                throw new ArgumentNullException("value");
            }
            GXDLMSTranslatorStructure xml = new GXDLMSTranslatorStructure(OutputType, Hex, ShowStringAsHex, tags);
            GXDLMSSettings settings = new GXDLMSSettings(true);
            GXReplyData data = new GXReplyData();
            byte cmd = value.GetUInt8();
            switch (cmd)
            {
                case (byte)Command.Aarq:
                    value.Position = 0;
                    settings = new GXDLMSSettings(true);
                    GXAPDU.ParsePDU(settings, settings.Cipher, value, xml);
                    break;
                case 0x81://Ua
                    value.Position = 0;
                    GetUa(value, xml);
                    break;
                case (byte)Command.Aare:
                    value.Position = 0;
                    settings = new GXDLMSSettings(false);
                    GXAPDU.ParsePDU(settings, settings.Cipher, value, xml);
                    break;
                case (byte)Command.GetRequest:
                    GXDLMSLNCommandHandler.HandleGetRequest(settings, null, value, null, xml);
                    break;
                case (byte)Command.SetRequest:
                    GXDLMSLNCommandHandler.HandleSetRequest(settings, null, value, null, xml);
                    break;
                case (byte)Command.ReadRequest:
                    GXDLMSSNCommandHandler.HandleReadRequest(settings, null, value, null, xml);
                    break;
                case (byte)Command.MethodRequest:
                    GXDLMSLNCommandHandler.HandleMethodRequest(settings, null, value, null, null, xml);
                    break;
                case (byte)Command.WriteRequest:
                    GXDLMSSNCommandHandler.HandleWriteRequest(settings, null, value, null, xml);
                    break;
                case (byte)Command.AccessRequest:
                    GXDLMSLNCommandHandler.HandleAccessRequest(settings, null, value, null, xml);
                    break;
                case (byte)Command.DataNotification:
                    data.Xml = xml;
                    data.Data = value;
                    value.Position = 0;
                    GXDLMS.GetPdu(settings, data);
                    break;
                case (byte)Command.ReadResponse:
                case (byte)Command.WriteResponse:
                case (byte)Command.GetResponse:
                case (byte)Command.SetResponse:
                case (byte)Command.MethodResponse:
                case (byte)Command.AccessResponse:
                    data.Xml = xml;
                    data.Data = value;
                    value.Position = 0;
                    GXDLMS.GetPdu(settings, data);
                    break;
                case (byte)Command.DisconnectRequest:
                case (byte)Command.DisconnectResponse:
                    xml.AppendStartTag((Command)cmd);
                    //Len.
                    if (value.GetUInt8() != 0)
                    {
                        //BerType
                        value.GetUInt8();
                        //Len.
                        value.GetUInt8();
                        xml.AppendLine(TranslatorTags.Reason, "Value", ((ReleaseRequestReason)value.GetUInt8()).ToString());
                    }
                    xml.AppendEndTag((Command)cmd);
                    break;
                case (byte)Command.GloReadRequest:
                case (byte)Command.GloWriteRequest:
                case (byte)Command.GloGetRequest:
                case (byte)Command.GloSetRequest:
                case (byte)Command.GloReadResponse:
                case (byte)Command.GloWriteResponse:
                case (byte)Command.GloGetResponse:
                case (byte)Command.GloSetResponse:
                case (byte)Command.GloMethodRequest:
                case (byte)Command.GloMethodResponse:
                    int cnt = GXCommon.GetObjectCount(value);
                    xml.AppendLine(cmd, "Value", GXCommon.ToHex(value.Data, false, value.Position, value.Size - value.Position));
                    break;
                default:
                    xml.AppendLine("<Data=\"" + GXCommon.ToHex(value.Data, false, value.Position, value.Size - value.Position) + "\" />");
                    break;
            }
            if (OutputType == TranslatorOutputType.StandardXml)
            {
                StringBuilder sb = new StringBuilder();
                if (!omitDeclaration)
                {
                    sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                }
                if (!omitNameSpace)
                {
                    if (cmd != (byte)Command.Aare && cmd != (byte)Command.Aarq)
                    {
                        sb.AppendLine(
                            "<x:xDLMS-APDU xmlns:x=\"http://www.dlms.com/COSEMpdu\">");
                    }
                    else
                    {
                        sb.AppendLine(
                            "<x:aCSE-APDU xmlns:x=\"http://www.dlms.com/COSEMpdu\">");
                    }
                }
                sb.Append(xml.ToString());
                if (!omitNameSpace)
                {
                    if (cmd != (byte)Command.Aare && cmd != (byte)Command.Aarq)
                    {
                        sb.AppendLine("</x:xDLMS-APDU>");
                    }
                    else
                    {
                        sb.AppendLine("</x:aCSE-APDU>");
                    }
                }
                return sb.ToString();
            }
            return xml.ToString();
        }

        private static void ReadAllNodes(XmlDocument doc, GXDLMSXmlSettings s)
        {
            if (doc != null)
            {
                foreach (XmlNode node in doc.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        ReadNode(node, s);
                    }
                }
            }
        }

        /// <summary>
        /// Get command from XML.
        /// </summary>
        /// <param name="node">XML node.</param>
        /// <param name="s">XML settings.</param>
        /// <param name="tag">tag.</param>
        private static void GetCommand(XmlNode node, GXDLMSXmlSettings s, int tag)
        {
            s.command = (Command)tag;
            switch (tag)
            {
                case (byte)Command.Snrm:
                case (byte)Command.Aarq:
                case (byte)Command.GetRequest:
                case (byte)Command.SetRequest:
                case (byte)Command.ReadRequest:
                case (byte)Command.WriteRequest:
                case (byte)Command.MethodRequest:
                case (byte)Command.DisconnectRequest:
                case (int)Command.AccessRequest:
                    s.settings.IsServer = false;
                    break;
                case (byte)Command.Ua:
                case (byte)Command.Aare:
                case (byte)Command.GetResponse:
                case (byte)Command.SetResponse:
                case (byte)Command.ReadResponse:
                case (byte)Command.WriteResponse:
                case (byte)Command.MethodResponse:
                case (byte)Command.DisconnectResponse:
                case (int)Command.DataNotification:
                case (int)Command.AccessResponse:
                    break;
                default:
                    throw new ArgumentException("Invalid Command: " + node.Name);
            }
        }

        private static Conformance ValueOfConformance(string value)
        {
            Conformance ret;
            if (string.Compare("access", value, true) == 0)
            {
                ret = Conformance.Access;
            }
            else if (string.Compare("action", value, true) == 0)
            {
                ret = Conformance.Action;
            }
            else if (string.Compare("attribute0-supported-with-get", value, true) == 0)
            {
                ret = Conformance.Attribute0SupportedWithGet;
            }
            else if (string.Compare("attribute0-supported-with-set", value, true) == 0)
            {
                ret = Conformance.Attribute0SupportedWithSet;
            }
            else if (string.Compare("block-transfer-with-action", value, true) == 0)
            {
                ret = Conformance.BlockTransferWithAction;
            }
            else if (string.Compare("block-transfer-with-get-or-read", value, true) == 0)
            {
                ret = Conformance.BlockTransferWithGetOrRead;
            }
            else if (string.Compare("block-transfer-with-set-or-write", value, true) == 0)
            {
                ret = Conformance.BlockTransferWithSetOrWrite;
            }
            else if (string.Compare("data-notification", value, true) == 0)
            {
                ret = Conformance.DataNotification;
            }
            else if (string.Compare("event-notification", value, true) == 0)
            {
                ret = Conformance.EventNotification;
            }
            else if (string.Compare("general-block-transfer", value, true) == 0)
            {
                ret = Conformance.GeneralBlockTransfer;
            }
            else if (string.Compare("general-protection", value, true) == 0)
            {
                ret = Conformance.GeneralProtection;
            }
            else if (string.Compare("get", value, true) == 0)
            {
                ret = Conformance.Get;
            }
            else if (string.Compare("information-report", value, true) == 0)
            {
                ret = Conformance.InformationReport;
            }
            else if (string.Compare("multiple-references", value, true) == 0)
            {
                ret = Conformance.MultipleReferences;
            }
            else if (string.Compare("parameterized-access", value, true) == 0)
            {
                ret = Conformance.ParameterizedAccess;
            }
            else if (string.Compare("priority-mgmt-supported", value, true) == 0)
            {
                ret = Conformance.PriorityMgmtSupported;
            }
            else if (string.Compare("read", value, true) == 0)
            {
                ret = Conformance.Read;
            }
            else if (string.Compare("reserved-seven", value, true) == 0)
            {
                ret = Conformance.ReservedSeven;
            }
            else if (string.Compare("reserved-six", value, true) == 0)
            {
                ret = Conformance.ReservedSix;
            }
            else if (string.Compare("reserved-zero", value, true) == 0)
            {
                ret = Conformance.ReservedZero;
            }
            else if (string.Compare("selective-access", value, true) == 0)
            {
                ret = Conformance.SelectiveAccess;
            }
            else if (string.Compare("set", value, true) == 0)
            {
                ret = Conformance.Set;
            }
            else if (string.Compare("unconfirmed-write", value, true) == 0)
            {
                ret = Conformance.UnconfirmedWrite;
            }
            else if (string.Compare("write", value, true) == 0)
            {
                ret = Conformance.Write;
            }
            else
            {
                throw new ArgumentOutOfRangeException(value);
            }
            return ret;
        }

        /// <summary>
        /// Handle AARE and AARQ XML tags.
        /// </summary>
        /// <param name="node">XML node.</param>
        /// <param name="s">XML Settings.</param>
        /// <param name="tag">XML tag.</param>
        private static void HandleAarqAare(XmlNode node, GXDLMSXmlSettings s, int tag)
        {
            byte[] tmp;
            byte[] conformanceBlock;
            int value;
            switch (tag)
            {
                case (int)TranslatorGeneralTags.ApplicationContextName:
                    if (s.OutputType == TranslatorOutputType.StandardXml)
                    {
                        value = int.Parse(node.InnerText);
                        switch (value)
                        {
                            case 1:
                                s.settings.UseLogicalNameReferencing = true;
                                break;
                            case 2:
                                s.settings.UseLogicalNameReferencing = false;
                                break;
                            case 3:
                                s.settings.UseLogicalNameReferencing = true;
                                break;
                            case 4:
                                s.settings.UseLogicalNameReferencing = false;
                                break;
                            default:
                                throw new ArgumentException("Invalid dedicated key.");
                        }
                    }
                    else
                    {
                        string str = node.Attributes[0].InnerText;
                        if (string.Compare(str, "SN") == 0 ||
                                string.Compare(str, "SN_WITH_CIPHERING") == 0)
                        {
                            s.settings.UseLogicalNameReferencing = false;
                        }
                        else if (string.Compare(str, "LN") == 0 ||
                                 string.Compare(str, "LN_WITH_CIPHERING") == 0)
                        {
                            s.settings.UseLogicalNameReferencing = true;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid Reference type name.");
                        }
                    }
                    break;
                case (byte)Command.GloInitiateRequest:
                case (byte)Command.GloGetRequest:
                case (byte)Command.GloSetRequest:
                case (byte)Command.GloMethodRequest:
                case (byte)Command.GloReadRequest:
                case (byte)Command.GloWriteRequest:
                    s.settings.IsServer = false;
                    tmp = GXCommon.HexToBytes(GetValue(node, s));
                    s.settings.Cipher.Security = (Security)tmp[0];
                    s.data.Set(tmp);
                    break;
                case (byte)Command.GloInitiateResponse:
                case (byte)Command.GloGetResponse:
                case (byte)Command.GloSetResponse:
                case (byte)Command.GloMethodResponse:
                case (byte)Command.GloReadResponse:
                case (byte)Command.GloWriteResponse:
                    tmp = GXCommon.HexToBytes(GetValue(node, s));
                    s.settings.Cipher.Security = (Security)tmp[0];
                    s.data.Set(tmp);
                    break;
                case (byte)Command.InitiateRequest:
                case (byte)Command.InitiateResponse:
                    if (s.OutputType == TranslatorOutputType.StandardXml)
                    {
                        GXByteBuffer bb = new GXByteBuffer();
                        tmp = GXCommon.HexToBytes(GetValue(node, s));
                        GXCommon.SetObjectCount(tmp.Length, bb);
                        bb.Set(tmp);
                        GXAPDU.ParseUserInformation(s.settings,
                                                    s.settings.Cipher, bb, null);
                        if (s.command == Command.Aarq)
                        {
                            if (s.settings.UseLogicalNameReferencing)
                            {
                                s.settings.LnSettings.ConformanceBlock =
                                    s.settings.ConformanceBlock;
                            }
                            else
                            {
                                s.settings.SnSettings.ConformanceBlock =
                                    s.settings.ConformanceBlock;
                            }
                        }
                    }
                    break;
                case 0xBE00:
                    //NegotiatedQualityOfService
                    break;
                case 0xBE06:
                case 0xBE01:
                    //NegotiatedDlmsVersionNumber or ProposedDlmsVersionNumber is skipped.
                    break;
                case 0xBE04:
                    //VaaName is not needed.
                    break;
                case 0x8A:
                    //SenderACSERequirements is not needed.
                    break;
                case 0x8B:
                case 0x89:
                    //MechanismName.
                    s.settings.Authentication = (Authentication)Enum.Parse(typeof(Authentication), GetValue(node, s));
                    if (s.OutputType == TranslatorOutputType.SimpleXml)
                    {
                        s.settings.Authentication = (Authentication)Enum.Parse(typeof(Authentication), GetValue(node, s));
                    }
                    else
                    {
                        s.settings.Authentication = (Authentication)int.Parse(GetValue(node, s));
                    }
                    break;
                case 0xAC:
                    //CallingAuthentication.
                    if (s.settings.Authentication == Authentication.Low)
                    {
                        s.settings.Password = GXCommon.HexToBytes(GetValue(node, s));
                    }
                    else
                    {
                        s.settings.CtoSChallenge = GXCommon.HexToBytes(GetValue(node, s));
                    }
                    break;
                case 0xA6:
                    //CallingAPTitle.
                    s.settings.CtoSChallenge = GXCommon.HexToBytes(GetValue(node, s));
                    break;
                case 0xA4:
                    //RespondingAPTitle.
                    s.settings.StoCChallenge = GXCommon.HexToBytes(GetValue(node, s));
                    break;
                case 0xBE03:
                case 0xBE05:
                    //ProposedConformance or NegotiatedConformance
                    if (s.settings.UseLogicalNameReferencing)
                    {
                        s.settings.LnSettings.Clear();
                    }
                    else
                    {
                        s.settings.SnSettings.Clear();
                    }
                    if (s.OutputType == TranslatorOutputType.StandardXml)
                    {
                        String nodes = node.InnerText;

                        if (s.settings.UseLogicalNameReferencing)
                        {
                            conformanceBlock = s.settings.LnSettings.ConformanceBlock;
                        }
                        else
                        {
                            conformanceBlock = s.settings.SnSettings.ConformanceBlock;
                        }
                        foreach (String it in nodes.Split(' '))
                        {
                            if (it.Trim() != string.Empty)
                            {
                                value = (int)ValueOfConformance(it.Trim());
                                if (value < 0x100)
                                {
                                    conformanceBlock[2] |= (byte)value;
                                }
                                else if (value < 0x10000)
                                {
                                    conformanceBlock[1] |= (byte)(value >> 8);
                                }
                                else
                                {
                                    conformanceBlock[0] |= (byte)(value >> 16);
                                }
                            }
                        }
                    }
                    break;
                case 0xBE08:
                    //ConformanceBit.
                    value = (int)Enum.Parse(typeof(Conformance), node.Attributes["Name"].InnerText);
                    if (s.settings.UseLogicalNameReferencing)
                    {
                        conformanceBlock = s.settings.LnSettings.ConformanceBlock;
                    }
                    else
                    {
                        conformanceBlock = s.settings.SnSettings.ConformanceBlock;
                    }
                    if (value < 0x100)
                    {
                        conformanceBlock[2] |= (byte)value;
                    }
                    else if (value < 0x10000)
                    {
                        conformanceBlock[1] |= (byte)(value >> 8);
                    }
                    else
                    {
                        conformanceBlock[0] |= (byte)(value >> 16);
                    }
                    break;
                case 0xA2:
                    //AssociationResult
                    s.result = (AssociationResult)Enum.Parse(typeof(AssociationResult), GetValue(node, s));
                    break;
                case 0xBE02:
                case 0xBE07:
                    //NegotiatedMaxPduSize or ProposedMaxPduSize.
                    s.settings.MaxPduSize = (UInt16)s.ParseInt(GetValue(node, s));
                    break;
                case 0xA3:
                    //ResultSourceDiagnostic
                    s.diagnostic = SourceDiagnostic.None;
                    break;
                case 0xA301:
                    //ACSEServiceUser
                    s.diagnostic = (SourceDiagnostic)s.ParseInt(GetValue(node, s));
                    break;
                case 0xBE09:
                    // ProposedQualityOfService
                    break;
                case (int)TranslatorGeneralTags.CharString:
                    // Get PW
                    if (s.settings.Authentication == Authentication.Low)
                    {
                        s.settings
                        .Password = GXCommon.HexToBytes(GetValue(node, s));
                    }
                    else
                    {
                        if (s.command == Command.Aarq)
                        {
                            s.settings.CtoSChallenge =
                                GXCommon.HexToBytes(GetValue(node, s));
                        }
                        else
                        {
                            s.settings.StoCChallenge =
                                GXCommon.HexToBytes(GetValue(node, s));
                        }
                    }
                    break;
                case (int)TranslatorGeneralTags.ResponderACSERequirement:
                    break;
                case (int)TranslatorGeneralTags.RespondingAuthentication:
                    s.settings
                    .StoCChallenge = GXCommon.HexToBytes(GetValue(node, s));
                    break;
                case (int)TranslatorTags.Result:
                    s.result = (AssociationResult)
                               int.Parse(GetValue(node, s));
                    break;
                default:
                    throw new ArgumentException("Invalid AARQ node: " + node.Name);

            }
        }

        private static GXByteBuffer UpdateDataType(XmlNode node, GXDLMSXmlSettings s, int tag)
        {
            GXByteBuffer preData = null;
            switch ((DataType)(tag - GXDLMS.DATA_TYPE_OFFSET))
            {
                case DataType.Array:
                    s.data.SetUInt8(DataType.Array);
                    preData = new GXByteBuffer(s.data);
                    s.data.Size = 0;
                    break;
                case DataType.Bcd:
                    GXCommon.SetData(s.data, DataType.Bcd, s.ParseShort(GetValue(node, s)));
                    break;
                case DataType.BitString:
                    GXCommon.SetData(s.data, DataType.BitString, GetValue(node, s));
                    break;
                case DataType.Boolean:
                    GXCommon.SetData(s.data, DataType.Boolean, s.ParseShort(GetValue(node, s)));
                    break;
                case DataType.Date:
                    GXCommon.SetData(s.data, DataType.Date, GXDLMSClient.ChangeType(GXCommon.HexToBytes(GetValue(node, s)), DataType.DateTime));
                    break;
                case DataType.DateTime:
                    GXCommon.SetData(s.data, DataType.DateTime, GXDLMSClient.ChangeType(GXCommon.HexToBytes(GetValue(node, s)), DataType.DateTime));
                    break;
                case DataType.Enum:
                    GXCommon.SetData(s.data, DataType.Enum, s.ParseShort(GetValue(node, s)));
                    break;
                case DataType.Float32:
                    GetFloat32(node, s);
                    break;
                case DataType.Float64:
                    GetFloat64(node, s);
                    break;
                case DataType.Int16:
                    GXCommon.SetData(s.data, DataType.Int16, s.ParseShort(GetValue(node, s)));
                    break;
                case DataType.Int32:
                    GXCommon.SetData(s.data, DataType.Int32, s.ParseInt(GetValue(node, s)));
                    break;
                case DataType.Int64:
                    GXCommon.SetData(s.data, DataType.Int64, s.ParseLong(GetValue(node, s)));
                    break;
                case DataType.Int8:
                    GXCommon.SetData(s.data, DataType.Int8, s.ParseShort(GetValue(node, s)));
                    break;
                case DataType.None:
                    GXCommon.SetData(s.data, DataType.None, null);
                    break;
                case DataType.OctetString:
                    GetOctetString(node, s);
                    break;
                case DataType.String:
                    if (s.showStringAsHex)
                    {
                        GXCommon.SetData(s.data, DataType.String, GXCommon.HexToBytes(GetValue(node, s)));
                    }
                    else
                    {
                        GXCommon.SetData(s.data, DataType.String, GetValue(node, s));
                    }
                    break;
                case DataType.StringUTF8:
                    if (s.showStringAsHex)
                    {
                        GXCommon.SetData(s.data, DataType.StringUTF8, GXCommon.HexToBytes(GetValue(node, s)));
                    }
                    else
                    {
                        GXCommon.SetData(s.data, DataType.StringUTF8, GetValue(node, s));
                    }
                    break;
                case DataType.Structure:
                    s.data.SetUInt8(DataType.Structure);
                    preData = new GXByteBuffer(s.data);
                    s.data.Size = 0;
                    //s.parameters.Add(DataType.Structure);
                    break;
                case DataType.Time:
                    GXCommon.SetData(s.data, DataType.Time, GXDLMSClient.ChangeType(GXCommon.HexToBytes(GetValue(node, s)), DataType.DateTime));
                    break;
                case DataType.UInt16:
                    GXCommon.SetData(s.data, DataType.UInt16, s.ParseInt(GetValue(node, s)));
                    break;
                case DataType.UInt32:
                    GXCommon.SetData(s.data, DataType.UInt32, s.ParseLong(GetValue(node, s)));
                    break;
                case DataType.UInt64:
                    GXCommon.SetData(s.data, DataType.UInt64, s.ParseULong(GetValue(node, s)));
                    break;
                case DataType.UInt8:
                    GXCommon.SetData(s.data, DataType.UInt8, s.ParseShort(GetValue(node, s)));
                    break;
                default:
                    throw new ArgumentException("Invalid node: " + node.Name);
            }
            return preData;
        }

        private static bool GetFrame(XmlNode node, GXDLMSXmlSettings s, int tag)
        {
            bool found = true;
            switch (tag)
            {
                case (int)TranslatorTags.Wrapper:
                    s.settings.InterfaceType = InterfaceType.WRAPPER;
                    break;
                case (int)TranslatorTags.Hdlc:
                    s.settings.InterfaceType = InterfaceType.HDLC;
                    break;
                case (int)TranslatorTags.TargetAddress:
                    s.settings.ServerAddress = int.Parse(GetValue(node, s), System.Globalization.NumberStyles.AllowHexSpecifier);
                    break;
                case (int)TranslatorTags.SourceAddress:
                    s.settings.ClientAddress = int.Parse(GetValue(node, s), System.Globalization.NumberStyles.AllowHexSpecifier);
                    break;
                default:
                    //It's OK if frame is not found.
                    found = false;
                    break;
            }
            return found;
        }

        private static string GetValue(XmlNode node, GXDLMSXmlSettings s)
        {
            if (s.OutputType == TranslatorOutputType.StandardXml)
            {
                if (node.FirstChild == null)
                {
                    return null;
                }
                return node.FirstChild.Value;
            }
            else
            {
                if (node.Attributes.Count == 0)
                {
                    return "";
                }
                return node.Attributes[0].InnerText;
            }
        }

        static ErrorCode ValueOfErrorCode(TranslatorOutputType type,
                                          String value)
        {
            if (type == TranslatorOutputType.StandardXml)
            {
                return TranslatorStandardTags.ValueOfErrorCode(value);
            }
            else
            {
                return TranslatorSimpleTags.ValueOfErrorCode(value);
            }
        }

        internal static String ErrorCodeToString(TranslatorOutputType type,
                ErrorCode value)
        {
            if (type == TranslatorOutputType.StandardXml)
            {
                return TranslatorStandardTags.ErrorCodeToString(value);
            }
            else
            {
                return TranslatorSimpleTags.ErrorCodeToString(value);
            }
        }

        static GXByteBuffer UpdateDateTime(XmlNode node, GXDLMSXmlSettings s, GXByteBuffer preData)
        {
            byte[] tmp;
            if (s.requestType != 0xFF)
            {
                preData = UpdateDataType(node, s,
                                         (int)DataType.DateTime + GXDLMS.DATA_TYPE_OFFSET);
            }
            else
            {
                tmp = GXCommon.HexToBytes(GetValue(node, s));
                if (tmp.Length != 0)
                {
                    DataType dt = DataType.DateTime;
                    if (tmp.Length == 5)
                    {
                        dt = DataType.Date;
                    }
                    else if (tmp.Length == 4)
                    {
                        dt = DataType.Time;
                    }
                    s.time = (GXDateTime)GXDLMSClient.ChangeType(tmp, dt);
                }
            }
            return preData;
        }

        private static void ReadNode(XmlNode node, GXDLMSXmlSettings s)
        {
            int tag;
            if (s.OutputType == TranslatorOutputType.SimpleXml)
            {
                tag = s.tags[node.Name.ToLower()];
            }
            else
            {
                tag = s.tags[node.Name];
            }
            string str;
            ErrorCode err;
            UInt32 value;
            byte[] tmp;
            GXByteBuffer preData = null;
            if (s.command == Command.None)
            {
                if (!((s.settings.ClientAddress == 0 ||
                        s.settings.ServerAddress == 0) &&
                        GetFrame(node, s, tag) ||
                        tag == (int)TranslatorTags.PduDlms ||
                        tag == (int)TranslatorTags.PduCse))
                {
                    GetCommand(node, s, tag);
                }
            }
            else if (s.command == Command.Aarq ||
                     s.command == Command.Aare)
            {
                HandleAarqAare(node, s, tag);
            }
            else if (tag >= GXDLMS.DATA_TYPE_OFFSET)
            {
                if (tag == (int)DataType.DateTime + GXDLMS.DATA_TYPE_OFFSET)
                {
                    preData = UpdateDateTime(node, s, preData);
                }
                else
                {
                    preData = UpdateDataType(node, s, tag);
                }
            }
            else
            {
                switch (tag)
                {
                    case (int)(Command.GetRequest) << 8 | (byte)GetCommandType.Normal:
                    case (int)(Command.GetRequest) << 8 | (byte)GetCommandType.NextDataBlock:
                    case (int)(Command.GetRequest) << 8 | (byte)GetCommandType.WithList:
                    case (int)(Command.SetRequest) << 8 | (byte)SetRequestType.Normal:
                    case (int)(Command.SetRequest) << 8 | (byte)SetRequestType.FirstDataBlock:
                    case (int)(Command.SetRequest) << 8 | (byte)SetRequestType.WithDataBlock:
                    case (int)(Command.SetRequest) << 8 | (byte)SetRequestType.WithList:
                        s.requestType = (byte)(tag & 0xF);
                        break;
                    case (int)(Command.GetResponse) << 8 | (byte)GetCommandType.Normal:
                    case (int)(Command.GetResponse) << 8 | (byte)GetCommandType.NextDataBlock:
                    case (int)(Command.GetResponse) << 8 | (byte)GetCommandType.WithList:
                    case (int)(Command.SetResponse) << 8 | (byte)SetResponseType.Normal:
                    case (int)(Command.SetResponse) << 8 | (byte)SetResponseType.DataBlock:
                    case (int)(Command.SetResponse) << 8 | (byte)SetResponseType.LastDataBlock:
                    case (int)(Command.SetResponse) << 8 | (byte)SetResponseType.WithList:
                    case (int)(Command.SetResponse) << 8 | (byte)SetResponseType.LastDataBlockWithList:
                        s.requestType = (byte)(tag & 0xF);
                        break;

                    case (int)(Command.ReadResponse) << 8 | (byte)SingleReadResponse.DataBlockResult:
                        ++s.count;
                        s.requestType = (byte)(tag & 0xF);
                        break;
                    case (int)(Command.ReadRequest) << 8 | (byte)VariableAccessSpecification.ParameterisedAccess:
                        s.requestType = (byte)VariableAccessSpecification.ParameterisedAccess;
                        break;
                    case (int)(Command.ReadRequest) << 8 | (byte)VariableAccessSpecification.BlockNumberAccess:
                        s.requestType = (byte)VariableAccessSpecification.BlockNumberAccess;
                        ++s.count;
                        break;
                    case (byte)(int)(Command.MethodRequest) << 8 | (byte)ActionRequestType.Normal:
                        s.requestType = (byte)(tag & 0xFF);
                        break;
                    case (byte)(int)(Command.MethodRequest) << 8 | (byte)ActionRequestType.NextBlock:
                        s.requestType = (byte)(tag & 0xFF);
                        break;
                    case (byte)(int)(Command.MethodRequest) << 8 | (byte)ActionRequestType.WithList:
                        s.requestType = (byte)(tag & 0xFF);
                        break;
                    case (byte)(int)(Command.MethodResponse) << 8 | (byte)ActionRequestType.Normal:
                        //MethodResponseNormal
                        s.requestType = (byte)(tag & 0xFF);
                        break;
                    case (int)(Command.ReadResponse) << 8 | (byte)SingleReadResponse.Data:
                    case (int)TranslatorTags.Data:
                        if (s.command == Command.ReadRequest
                                || s.command == Command.ReadResponse
                                || s.command == Command.GetRequest)
                        {
                            ++s.count;
                            s.requestType = 0;
                        }
                        else if (s.command == Command.GetResponse ||
                                 s.command == Command.MethodResponse)
                        {
                            s.data.SetUInt8(0); // Add status.
                        }
                        break;

                    case (int)TranslatorTags.Success:
                        ++s.count;
                        s.attributeDescriptor.Add((byte)ErrorCode.Ok);
                        break;
                    case (int)TranslatorTags.DataAccessError:
                        ++s.count;
                        s.attributeDescriptor.SetUInt8(1);
                        s.attributeDescriptor.SetUInt8(ValueOfErrorCode(s.OutputType, GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.ListOfVariableAccessSpecification:
                    case (int)TranslatorTags.VariableAccessSpecification:
                        break;
                    case (int)TranslatorTags.ListOfData:
                        if (s.command == Command.AccessResponse
                                && s.data.Size == 0)
                        {
                            // If access-request-specification is not given.
                            s.data.SetUInt8(0);
                        }
                        if (s.OutputType == TranslatorOutputType.SimpleXml
                                || s.command != Command.WriteRequest)
                        {
                            GXCommon.SetObjectCount(node.ChildNodes.Count, s.data);
                        }
                        break;
                    case (int)Command.AccessResponse << 8 | (byte)AccessServiceCommandType.Get:
                    case (int)Command.AccessResponse << 8 | (byte)AccessServiceCommandType.Set:
                    case (int)Command.AccessResponse << 8 | (byte)AccessServiceCommandType.Action:
                        s.data.SetUInt8((byte)(0xFF & tag));
                        break;
                    case (int)TranslatorTags.DateTime:
                        preData = UpdateDateTime(node, s, preData);
                        break;
                    case (int)TranslatorTags.InvokeId:
                        value = (uint)s.ParseShort(GetValue(node, s));
                        if ((value & 0x80) != 0)
                        {
                            s.settings.Priority = Priority.High;
                        }
                        else
                        {
                            s.settings.Priority = Priority.Normal;
                        }
                        if ((value & 0x40) != 0)
                        {
                            s.settings.ServiceClass = ServiceClass.Confirmed;
                        }
                        else
                        {
                            s.settings.ServiceClass = ServiceClass.UnConfirmed;
                        }
                        s.settings.InvokeID = (byte)(value & 0xF);
                        break;
                    case (int)TranslatorTags.LongInvokeId:
                        value = (uint)s.ParseLong(GetValue(node, s));
                        if ((value & 0x80000000) != 0)
                        {
                            s.settings.Priority = Priority.High;
                        }
                        else
                        {
                            s.settings.Priority = Priority.Normal;
                        }
                        if ((value & 0x40000000) != 0)
                        {
                            s.settings.ServiceClass = ServiceClass.Confirmed;
                        }
                        else
                        {
                            s.settings.ServiceClass = ServiceClass.UnConfirmed;
                        }
                        s.settings.longInvokeID = (UInt16)(value & 0xFFFFFFF);
                        break;
                    case 0x88:
                        //ResponderACSERequirement
                        break;
                    case 0x80:
                        //RespondingAuthentication
                        s.settings.StoCChallenge = GXCommon.HexToBytes(GetValue(node, s));
                        break;
                    case (int)TranslatorTags.AttributeDescriptor:
                        break;
                    case (int)TranslatorTags.ClassId:
                        s.attributeDescriptor.SetUInt16((UInt16)s.ParseInt(GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.InstanceId:
                        s.attributeDescriptor.Add(GXCommon.HexToBytes(GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.AttributeId:
                        s.attributeDescriptor.SetUInt8((byte)s.ParseShort(GetValue(node, s)));
                        //Add AccessSelection.
                        if (s.command != Command.AccessRequest)
                        {
                            s.attributeDescriptor.SetUInt8(0);
                        }
                        break;
                    case (int)TranslatorTags.MethodInvocationParameters:
                        s.attributeDescriptor.SetUInt8(s.attributeDescriptor.Size - 1, 1);
                        break;
                    case (int)TranslatorTags.Selector:
                        s.attributeDescriptor.Set(GXCommon.HexToBytes(GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.Parameter:
                        break;
                    case (int)TranslatorTags.LastBlock:
                        s.data.SetUInt8((byte)s.ParseShort(GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.BlockNumber:
                        //BlockNumber
                        if (s.command == Command.GetRequest || s.command == Command.GetResponse)
                        {
                            s.data.SetUInt32((UInt32)s.ParseLong(GetValue(node, s)));
                        }
                        else
                        {
                            s.data.SetUInt16((UInt16)s.ParseInt(GetValue(node, s)));
                        }
                        break;
                    case (int)TranslatorTags.RawData:
                        //RawData
                        if (s.command == Command.GetResponse)
                        {
                            s.data.SetUInt8(0);
                        }
                        tmp = GXCommon.HexToBytes(GetValue(node, s));
                        GXCommon.SetObjectCount(tmp.Length, s.data);
                        s.data.Set(tmp);
                        break;
                    case (int)TranslatorTags.MethodDescriptor:
                        break;
                    case (int)TranslatorTags.MethodId:
                        s.attributeDescriptor.SetUInt8((byte)s.ParseShort(GetValue(node, s)));
                        //Add MethodInvocationParameters
                        s.attributeDescriptor.SetUInt8(0);
                        break;
                    case (int)TranslatorTags.Result:
                    case (int)TranslatorGeneralTags.AssociationResult:
                        //Result.
                        if (s.command == Command.GetRequest || s.requestType == 3)
                        {
                            GXCommon.SetObjectCount(node.ChildNodes.Count, s.attributeDescriptor);
                        }
                        else if (s.command == Command.MethodResponse || s.command == Command.SetResponse)
                        {
                            str = GetValue(node, s);
                            if (str != "")
                            {
                                s.attributeDescriptor.SetUInt8((byte)ValueOfErrorCode(s.OutputType, str));
                            }

                        }
                        else if (s.command == Command.AccessResponse)
                        {
                            str = GetValue(node, s);
                            if (str != "")
                            {
                                s.data.SetUInt8(ValueOfErrorCode(s.OutputType, str));
                            }
                        }
                        break;
                    case (int)TranslatorTags.Reason:
                        s.reason = (ReleaseRequestReason)Enum.Parse(typeof(ReleaseRequestReason), GetValue(node, s));
                        break;
                    case (int)TranslatorTags.ReturnParameters:
                        s.attributeDescriptor.SetUInt8(1);
                        break;
                    case (int)TranslatorTags.AccessSelection:
                        s.attributeDescriptor.SetUInt8(s.attributeDescriptor.Size - 1, 1);
                        break;
                    case (int)TranslatorTags.Value:
                        break;
                    case (int)TranslatorTags.AccessSelector:
                        s.data.SetUInt8((byte)s.ParseShort(GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.AccessParameters:
                        break;
                    case (int)TranslatorTags.AttributeDescriptorList:
                        GXCommon.SetObjectCount(node.ChildNodes.Count, s.attributeDescriptor);
                        break;
                    case (int)TranslatorTags.AttributeDescriptorWithSelection:
                    case (int)Command.AccessRequest << 8 | (byte)AccessServiceCommandType.Get:
                    case (int)Command.AccessRequest << 8 | (byte)AccessServiceCommandType.Set:
                    case (int)Command.AccessRequest << 8 | (byte)AccessServiceCommandType.Action:
                        s.attributeDescriptor.SetUInt8((byte)(tag & 0xFF));
                        break;
                    case (int)Command.ReadRequest << 8 | (byte)VariableAccessSpecification.VariableName:
                    case (int)Command.WriteRequest << 8 | (byte)VariableAccessSpecification.VariableName:
                    case (int)Command.WriteRequest << 8 | (byte)SingleReadResponse.Data:
                        if (s.command != Command.AccessRequest
                                && s.command != Command.AccessResponse)
                        {
                            if (!(s.OutputType == TranslatorOutputType.StandardXml
                                    && tag == ((int)Command.WriteRequest << 8
                                               | (int)SingleReadResponse.Data)))
                            {
                                if (s.requestType == 0xFF)
                                {
                                    s.attributeDescriptor.SetUInt8(
                                        VariableAccessSpecification.VariableName);
                                }
                                else
                                {
                                    s.attributeDescriptor.SetUInt8(s.requestType);
                                    s.requestType = 0xFF;
                                }
                                ++s.count;
                            }
                            else
                            {
                                s.attributeDescriptor.SetUInt8((byte)s.count);
                            }
                            if (s.OutputType == TranslatorOutputType.SimpleXml)
                            {
                                s.attributeDescriptor.SetUInt16((UInt16)s.ParseShort(GetValue(node, s)));
                            }
                            else
                            {
                                str = GetValue(node, s);
                                if (!String.IsNullOrEmpty(str))
                                {
                                    s.attributeDescriptor.SetInt16(Int16.Parse(str));
                                }
                            }
                        }
                        break;
                    case (int)TranslatorTags.Choice:
                        break;
                    case (int)Command.ReadResponse << 8
                        | (int)SingleReadResponse.DataAccessError:
                        err =
                            ValueOfErrorCode(s.OutputType, GetValue(node, s));
                        ++s.count;
                        s.data.SetUInt8(1);
                        s.data.SetUInt8(err);
                        break;
                    case (int)TranslatorTags.NotificationBody:
                        break;
                    case (int)TranslatorTags.DataValue:
                        break;
                    case (int)TranslatorTags.AccessRequestBody:
                        break;
                    case (int)TranslatorTags.ListOfAccessRequestSpecification:
                        s.attributeDescriptor.SetUInt8((byte)node.ChildNodes.Count);
                        break;
                    case (int)TranslatorTags.AccessRequestSpecification:
                        break;
                    case (int)TranslatorTags.AccessRequestListOfData:
                        s.attributeDescriptor.SetUInt8((byte)node.ChildNodes.Count);
                        break;
                    case (int)TranslatorTags.AccessResponseBody:
                        break;
                    case (int)TranslatorTags.ListOfAccessResponseSpecification:
                        s.data.SetUInt8((byte)node.ChildNodes.Count);
                        break;
                    case (int)TranslatorTags.AccessResponseSpecification:
                        break;
                    case (int)TranslatorTags.AccessResponseListOfData:
                        // Add access-response-list-of-data. Optional
                        s.data.SetUInt8(0);
                        s.data.SetUInt8((byte)node.ChildNodes.Count);
                        break;
                    case (int)TranslatorTags.SingleResponse:
                        break;
                    default:
                        throw new ArgumentException("Invalid node: " + node.Name);
                }
            }
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    ReadNode(childNode, s);
                }
            }
            if (preData != null)
            {
                GXCommon.SetObjectCount(node.ChildNodes.Count, preData);
                preData.Set(s.data);
                s.data.Size = 0;
                s.data.Set(preData);
            }
        }

        private static void GetOctetString(XmlNode node, GXDLMSXmlSettings s)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetHexString(GetValue(node, s));
            GXCommon.SetData(s.data, DataType.OctetString, bb.Array());
        }

        private static void GetFloat32(XmlNode node, GXDLMSXmlSettings s)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetHexString(GetValue(node, s));
            GXCommon.SetData(s.data, DataType.Float32, bb.GetFloat());
        }

        private static void GetFloat64(XmlNode node, GXDLMSXmlSettings s)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetHexString(GetValue(node, s));
            GXCommon.SetData(s.data, DataType.Float64, bb.GetDouble());
        }

        /// <summary>
        /// Convert xml to hex string.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string XmlToHexPdu(string xml)
        {
            return GXCommon.ToHex(XmlToPdu(xml), false);
        }

        /// <summary>
        /// Convert xml to byte array.
        /// </summary>
        /// <param name="xml">Converted xml.</param>
        /// <returns>Converted bytes.</returns>
        public byte[] XmlToPdu(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            GXDLMSXmlSettings s = new GXDLMSXmlSettings(OutputType, Hex, ShowStringAsHex, tagsByName);
            ReadAllNodes(doc, s);
            GXByteBuffer bb = new GXByteBuffer();
            GXDLMSLNParameters ln;
            GXDLMSSNParameters sn;
            switch (s.command)
            {
                case Command.InitiateRequest:
                case Command.InitiateResponse:
                    break;
                case Command.ReadRequest:
                case Command.WriteRequest:
                case Command.ReadResponse:
                case Command.WriteResponse:
                    sn = new GXDLMSSNParameters(s.settings, s.command, s.count,
                                                s.requestType, s.attributeDescriptor, s.data);
                    GXDLMS.GetSNPdu(sn, bb);
                    break;
                case Command.GetRequest:
                case Command.GetResponse:
                case Command.SetRequest:
                case Command.SetResponse:
                case Command.MethodRequest:
                case Command.MethodResponse:
                    ln = new GXDLMSLNParameters(s.settings, s.command, s.requestType,
                                                s.attributeDescriptor, s.data, 0xff);
                    GXDLMS.GetLNPdu(ln, bb);
                    break;
                case Command.GloGetRequest:
                case Command.GloGetResponse:
                case Command.GloSetRequest:
                case Command.GloSetResponse:
                case Command.GloMethodRequest:
                case Command.GloMethodResponse:
                case Command.GloReadRequest:
                case Command.GloWriteRequest:
                case Command.GloReadResponse:
                case Command.GloWriteResponse:
                    bb.SetUInt8((byte)s.command);
                    GXCommon.SetObjectCount(s.data.Size, bb);
                    bb.Set(s.data);
                    break;
                case Command.Rejected:
                    break;
                case Command.Snrm:
                    s.settings.IsServer = false;
                    bb.Set(GXDLMS.GetHdlcFrame(s.settings, (byte)Command.Snrm, null));
                    break;
                case Command.Ua:
                    break;
                case Command.Aarq:
                case Command.GloInitiateRequest:
                    GXAPDU.GenerateAarq(s.settings, s.settings.Cipher, s.data, bb);
                    break;
                case Command.Aare:
                case Command.GloInitiateResponse:
                    GXAPDU.GenerateAARE(s.settings, bb, s.result, s.diagnostic, s.settings.Cipher, s.data);
                    break;
                case Command.Disc:
                    break;
                case Command.DisconnectRequest:
                    bb.SetUInt8((byte)s.command);
                    bb.SetUInt8(0);
                    break;
                case Command.DisconnectResponse:
                    bb.SetUInt8((byte)s.command);
                    //Len
                    bb.SetUInt8(3);
                    //BerType
                    bb.SetUInt8(BerType.Context);
                    //Len.
                    bb.SetUInt8(1);
                    bb.SetUInt8(s.reason);
                    break;
                case Command.ConfirmedServiceError:
                    break;
                case Command.ExceptionResponse:
                    break;
                case Command.GeneralBlockTransfer:
                    break;
                case Command.AccessRequest:
                    ln = new GXDLMSLNParameters(s.settings, s.command, s.requestType,
                                                s.attributeDescriptor, s.data, 0xff);
                    GXDLMS.GetLNPdu(ln, bb);
                    break;
                case Command.AccessResponse:
                    ln = new GXDLMSLNParameters(s.settings, s.command, s.requestType,
                                                s.attributeDescriptor, s.data, 0xff);
                    GXDLMS.GetLNPdu(ln, bb);
                    break;
                case Command.DataNotification:
                    ln = new GXDLMSLNParameters(s.settings, s.command, s.requestType,
                                                s.attributeDescriptor, s.data, 0xff);
                    ln.time = s.time;
                    GXDLMS.GetLNPdu(ln, bb);
                    break;
                case Command.GloGeneralCiphering:
                    break;
                case Command.GloEventNotificationRequest:
                    break;
                default:
                case Command.None:
                    throw new ArgumentException("Invalid command.");
            }
            return bb.Array();
        }
    }
}