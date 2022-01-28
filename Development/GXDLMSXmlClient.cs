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
// More information of Gurux products: https://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;
using System.Xml;
using System.IO;
using System.ComponentModel;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS
{
    public class GXDLMSXmlPdu
    {
        public XmlNode XmlNode
        {
            get;
            private set;
        }

        /// <summary>
        /// Command.
        /// </summary>
        public Command Command
        {
            get;
            set;
        }

        /// <summary>
        /// Generated Pdu.
        /// </summary>
        public byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// Description of the test.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Shown error if this test fails.
        /// </summary>
        public string Error
        {
            get;
            set;
        }

        /// <summary>
        /// Error url if test fails.
        /// </summary>
        public string ErrorUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Sleep time in milliseconds.
        /// </summary>
        public int Sleep
        {
            get;
            set;
        }


        /// <summary>
        /// Return PDU as XML string.
        /// </summary>
        public string PduAsXml
        {
            get
            {
                if (XmlNode == null)
                {
                    return "";
                }
                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    ConformanceLevel = ConformanceLevel.Auto,
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };
                using (XmlWriter writer = XmlWriter.Create(sb, settings))
                {
                    XmlNode.WriteTo(writer);
                }
                return sb.ToString();
            }
        }

        private static void Compare(XmlNode expectedNode, XmlNode actualNode, List<string> list)
        {
            int cnt = expectedNode.ChildNodes.Count;
            if (string.Compare(expectedNode.Name, actualNode.Name) != 0)
            {
                XmlAttribute a = expectedNode.Attributes["Value"];
                if (string.Compare(expectedNode.Name, "None") == 0 && a != null && string.Compare(a.Value, "*") == 0)
                {
                    return;
                }
                list.Add(expectedNode.Name + "-" + actualNode.Name);
                return;
            }
            else if (cnt != actualNode.ChildNodes.Count)
            {
                //If we are reading array items count might vary.
                if (expectedNode.Name == "Array" || expectedNode.Name == "Structure")
                {
                    if (cnt < actualNode.ChildNodes.Count)
                    {
                        //Check only first If meter is returning more nodes what we have in template.
                    }
                    else
                    {
                        cnt = actualNode.ChildNodes.Count;
                    }
                }
                else
                {
                    list.Add("Different amount: " + expectedNode.Name + "-" + actualNode.Name);
                    return;
                }
            }
            for (int pos = 0; pos != cnt; ++pos)
            {
                if (actualNode.ChildNodes[pos] == null)
                {
                    list.Add("Different values. Expected: '" + expectedNode.ChildNodes[pos].OuterXml + "'. Actual: 'null'.");
                }
                else if (actualNode.ChildNodes[pos].ChildNodes.Count != 0)
                {
                    Compare(expectedNode.ChildNodes[pos], actualNode.ChildNodes[pos], list);
                }
                else if (string.Compare(expectedNode.ChildNodes[pos].OuterXml, actualNode.ChildNodes[pos].OuterXml) != 0)
                {
                    XmlAttribute a = expectedNode.ChildNodes[pos].Attributes["Value"];
                    if (a == null ||
                        //If value type is not defined.
                        (string.Compare(expectedNode.ChildNodes[pos].Name, "None") != 0 &&
                        string.Compare(expectedNode.ChildNodes[pos].Name, actualNode.ChildNodes[pos].Name) != 0)
                        || string.Compare(a.Value, "*") != 0)
                    {
                        if (expectedNode.FirstChild.Name != "Structure" && expectedNode.FirstChild.Name != "Array" && expectedNode.ParentNode.Name != "Array")
                        {
                            list.Add("Different values. Expected: '" + expectedNode.ChildNodes[pos].OuterXml + "'. Actual: '" + actualNode.ChildNodes[pos].OuterXml + "'.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compare load XML.
        /// </summary>
        /// <param name="xml">XML string to compare.</param>
        /// <returns>True, if content is same.</returns>
        public List<string> Compare(string xml)
        {
            List<string> list = new List<string>();
            XmlDocument doc2 = new XmlDocument();
            doc2.LoadXml(xml);
            Compare(XmlNode, doc2.ChildNodes[0], list);
            return list;
        }

        /// <summary>
        /// Is command request.
        /// </summary>
        /// <remarks>
        /// This information is used to tell is PDU send.
        /// </remarks>
        /// <returns>True, if command is request.</returns>
        public bool IsRequest()
        {
            bool ret;
            switch (Command)
            {
                case Command.Snrm:
                case Command.Aarq:
                case Command.ReadRequest:
                case Command.GloReadRequest:
                case Command.WriteRequest:
                case Command.GloWriteRequest:
                case Command.GetRequest:
                case Command.GloGetRequest:
                case Command.SetRequest:
                case Command.GloSetRequest:
                case Command.MethodRequest:
                case Command.GloMethodRequest:
                case Command.DisconnectRequest:
                case Command.ReleaseRequest:
                    ret = true;
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSXmlPdu()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="command">Generated command.</param>
        /// <param name="xml">Generated PDU as XML.</param>
        /// <param name="pdu">Generated PDU.</param>
        public GXDLMSXmlPdu(Command command, XmlNode xml, byte[] pdu)
        {
            Command = command;
            XmlNode = xml;
            Data = pdu;
        }

        public override string ToString()
        {
            return PduAsXml;
        }
    }

    public class GXXmlLoadSettings
    {
        /// <summary>
        /// Start date of profile Generic.
        /// </summary>
        public DateTime Start
        {
            get;
            set;
        }

        /// <summary>
        /// End date of profile Generic.
        /// </summary>
        public DateTime End
        {
            get;
            set;
        }
    }

    /// <summary>
    /// GXDLMS Xml client implements methods to communicate with DLMS/COSEM metering devices using XML.
    /// </summary>
    public class GXDLMSXmlClient : GXDLMSSecureClient
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">XML type.</param>
        public GXDLMSXmlClient(TranslatorOutputType type)
        {
            translator = new GXDLMSTranslator(type);
            translator.Hex = false;
            UseLogicalNameReferencing = true;
        }

        /// <summary>
        /// Is string serialized as hex.
        /// </summary>
        /// <seealso cref="MessageToXml"/>
        /// <seealso cref="PduOnly"/>
        [DefaultValue(false)]
        public bool ShowStringAsHex
        {
            get
            {
                return translator.ShowStringAsHex;
            }
            set
            {
                translator.ShowStringAsHex = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">XML type.</param>
        public GXDLMSXmlClient(TranslatorOutputType type, bool hex)
        {
            translator = new GXDLMSTranslator(type);
            translator.Hex = hex;
            UseLogicalNameReferencing = true;
        }

        /// <summary>
        /// XML client don't throw exceptions. It serializes them as a default.
        /// Set value to true, if exceptions are thrown.
        /// </summary>
        public bool ThrowExceptions
        {
            get
            {
                return throwExceptions;
            }
            set
            {
                throwExceptions = value;
            }
        }

        /// <summary>
        /// Load XML commands from the file.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> Load(Stream stream)
        {
            return Load(stream, null);
        }

        /// <summary>
        /// Load XML commands from the file.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> Load(Stream stream, GXXmlLoadSettings settings)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            return Load(doc, settings);
        }

        /// <summary>
        /// Load XML commands from the file.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> Load(StreamReader stream)
        {
            return Load(stream, null);
        }

        /// <summary>
        /// Load XML commands from the file.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> Load(StreamReader stream, GXXmlLoadSettings settings)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            return Load(doc, settings);
        }

        /// <summary>
        /// Load XML commands from the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> Load(string fileName)
        {
            return Load(fileName, null);
        }

        /// <summary>
        /// Load XML commands from the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="settings">Additional settings.</param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> Load(string fileName, GXXmlLoadSettings settings)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            return Load(doc, settings);
        }

        /// <summary>
        /// Load XML commands from xml string.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> LoadXml(string xml)
        {
            return LoadXml(xml, null);
        }

        /// <summary>
        /// Load XML commands from xml string.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> LoadXml(string xml, GXXmlLoadSettings settings)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return Load(doc, settings);
        }

        private List<GXDLMSXmlPdu> Load(XmlDocument doc, GXXmlLoadSettings settings)
        {
            //Remove comments.
            List<XmlNode> comments = new List<XmlNode>();
            foreach (XmlNode node in doc.SelectNodes("//comment()"))
            {
                comments.Add(node);
            }
            foreach (XmlNode node in comments)
            {
                node.ParentNode.RemoveChild(node);
            }
            List<GXDLMSXmlPdu> actions = new List<GXDLMSXmlPdu>();
            string description = null, error = null, errorUrl = null, sleep = null;
            foreach (XmlNode m1 in doc.ChildNodes)
            {
                if (m1.NodeType == XmlNodeType.Element)
                {
                    if (m1.Name == "AssociationRequest")
                    {
                        GXDLMSXmlSettings s = new GXDLMSXmlSettings(translator.OutputType, translator.Hex, translator.ShowStringAsHex, translator.tagsByName);
                        s.settings.ClientAddress = Settings.ClientAddress;
                        s.settings.ServerAddress = Settings.ServerAddress;
                        ((GXCiphering)s.settings.Cipher).TestMode = this.Ciphering.TestMode;
                        byte[] reply = translator.XmlToPdu(m1.OuterXml, s);
                        GXDLMSXmlPdu p = new GXDLMSXmlPdu(s.command, m1, reply);
                        actions.Add(p);
                        return actions;
                    }
                    foreach (XmlNode node in m1.ChildNodes)
                    {
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            if (node.Name == "Description")
                            {
                                description = node.Value;
                                continue;
                            }
                            if (node.Name == "Error")
                            {
                                error = node.Value;
                                continue;
                            }
                            if (node.Name == "ErrorUrl")
                            {
                                errorUrl = node.Value;
                                continue;
                            }
                            if (node.Name == "Sleep")
                            {
                                sleep = node.Value;
                                continue;
                            }
                            if (settings != null && node.Name == "GetRequest")
                            {
                                if (settings.Start != DateTime.MinValue && settings.End != DateTime.MinValue)
                                {
                                    foreach (XmlNode n1 in node.ChildNodes)
                                    {
                                        if (n1.Name == "GetRequestNormal")
                                        {
                                            foreach (XmlNode n2 in n1.ChildNodes)
                                            {
                                                if (n2.Name == "AccessSelection")
                                                {
                                                    foreach (XmlNode n3 in n2.ChildNodes)
                                                    {
                                                        if (n3.Name == "AccessSelector")
                                                        {
                                                            if (n3.Attributes["Value"].Value != "1")
                                                            {
                                                                break;
                                                            }
                                                        }
                                                        else if (n3.Name == "AccessParameters")
                                                        {
                                                            foreach (XmlNode n4 in n3.ChildNodes)
                                                            {
                                                                if (n4.Name == "Structure")
                                                                {
                                                                    bool start = true;
                                                                    foreach (XmlNode n5 in n4.ChildNodes)
                                                                    {
                                                                        if (n5.Name == "OctetString")
                                                                        {
                                                                            if (start)
                                                                            {
                                                                                GXByteBuffer bb = new GXByteBuffer();
                                                                                GXCommon.SetData(this.Settings, bb, DataType.OctetString, settings.Start);
                                                                                n5.Attributes["Value"].Value = bb.ToHex(false, 2);
                                                                                start = false;
                                                                            }
                                                                            else
                                                                            {
                                                                                GXByteBuffer bb = new GXByteBuffer();
                                                                                GXCommon.SetData(this.Settings, bb, DataType.OctetString, settings.End);
                                                                                n5.Attributes["Value"].Value = bb.ToHex(false, 2);
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            }
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }

                            GXDLMSXmlSettings s = new GXDLMSXmlSettings(translator.OutputType, translator.Hex, translator.ShowStringAsHex, translator.tagsByName); ;
                            s.settings.ClientAddress = Settings.ClientAddress;
                            s.settings.ServerAddress = Settings.ServerAddress;
                            byte[] reply = translator.XmlToPdu(node.OuterXml, s);
                            if (s.command == Command.Snrm && !s.settings.IsServer)
                            {
                                Settings.Hdlc.MaxInfoTX = s.settings.Hdlc.MaxInfoTX;
                                Settings.Hdlc.MaxInfoRX = s.settings.Hdlc.MaxInfoRX;
                                Settings.Hdlc.WindowSizeRX = s.settings.Hdlc.WindowSizeRX;
                                Settings.Hdlc.WindowSizeTX = s.settings.Hdlc.WindowSizeTX;
                            }
                            else if (s.command == Command.Ua && s.settings.IsServer)
                            {
                                Settings.Hdlc.MaxInfoTX = s.settings.Hdlc.MaxInfoTX;
                                Settings.Hdlc.MaxInfoRX = s.settings.Hdlc.MaxInfoRX;
                                Settings.Hdlc.WindowSizeRX = s.settings.Hdlc.WindowSizeRX;
                                Settings.Hdlc.WindowSizeTX = s.settings.Hdlc.WindowSizeTX;
                            }
                            if (s.template)
                            {
                                reply = null;
                            }
                            GXDLMSXmlPdu p = new GXDLMSXmlPdu(s.command, node, reply);
                            if (description != "")
                            {
                                p.Description = description;
                            }
                            if (error != "")
                            {
                                p.Error = error;
                            }
                            if (errorUrl != "")
                            {
                                p.ErrorUrl = errorUrl;
                            }
                            if (!string.IsNullOrEmpty(sleep))
                            {
                                p.Sleep = int.Parse(sleep);
                            }
                            actions.Add(p);
                        }
                    }
                }
            }
            return actions;
        }

        /// <summary>
        /// Load XML commands from the file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[][] PduToMessages(GXDLMSXmlPdu pdu)
        {
            List<byte[]> messages = new List<byte[]>();
            if (pdu.Command == Command.Snrm)
            {
                messages.Add(pdu.Data);
            }
            else if (pdu.Command == Command.Ua)
            {
                messages.Add(pdu.Data);
            }
            else if (pdu.Command == Command.DisconnectRequest)
            {
                messages.Add(GXDLMS.GetHdlcFrame(Settings, (byte)Command.DisconnectRequest, new GXByteBuffer(pdu.Data)));
            }
            else
            {
                GXByteBuffer reply;
                if (Settings.InterfaceType == InterfaceType.WRAPPER)
                {
                    if (Ciphering.Security != Security.None)
                    {
                        GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, pdu.Command, 0x0, null, null, 0xff, Command.None);
                        reply = new GXByteBuffer(GXDLMS.Cipher0(p, pdu.Data));
                    }
                    else
                    {
                        reply = new GXByteBuffer(pdu.Data);
                    }
                }
                else
                {
                    if (Ciphering.Security != Security.None)
                    {
                        GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, pdu.Command, 0x0, null, null, 0xff, Command.None);
                        byte[] tmp = GXDLMS.Cipher0(p, pdu.Data);
                        reply = new GXByteBuffer((UInt16)(3 + tmp.Length));
                        reply.Set(GXCommon.LLCSendBytes);
                        reply.Set(tmp);
                    }
                    else
                    {
                        reply = new GXByteBuffer((UInt16)(3 + pdu.Data.Length));
                        reply.Set(GXCommon.LLCSendBytes);
                        reply.Set(pdu.Data);
                    }
                }
                byte frame = 0;
                while (reply.Position != reply.Size)
                {
                    if (Settings.InterfaceType == Enums.InterfaceType.WRAPPER)
                    {
                        messages.Add(GXDLMS.GetWrapperFrame(Settings, pdu.Command, reply));
                    }
                    else if (GXDLMS.UseHdlc(Settings.InterfaceType))
                    {
                        if (pdu.Command == Command.Aarq)
                        {
                            frame = 0x10;
                        }
                        else if (pdu.Command == Command.Aare)
                        {
                            frame = 0x30;
                        }
                        else if (pdu.Command == Command.EventNotification)
                        {
                            frame = 0x13;
                        }
                        messages.Add(GXDLMS.GetHdlcFrame(Settings, frame, reply));
                        if (reply.Position != reply.Size)
                        {
                            frame = Settings.NextSend(false);
                        }
                    }
                    else if (Settings.InterfaceType == Enums.InterfaceType.PDU)
                    {
                        messages.Add(reply.Array());
                        break;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("InterfaceType");
                    }
                }
            }
            return messages.ToArray();
        }
    }
}