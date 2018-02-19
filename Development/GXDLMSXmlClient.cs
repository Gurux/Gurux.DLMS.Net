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

using System;
using System.Collections.Generic;
using System.Text;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;
using System.Xml;
using System.IO;

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
        /// Load XML commands from the file.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> Load(Stream stream)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            return Load(doc);
        }


        /// <summary>
        /// Load XML commands from the file.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> Load(StreamReader stream)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            return Load(doc);
        }

        /// <summary>
        /// Load XML commands from the file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> Load(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            return Load(doc);
        }

        /// <summary>
        /// Load XML commands from xml string.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public List<GXDLMSXmlPdu> LoadXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return Load(doc);
        }

        private List<GXDLMSXmlPdu> Load(XmlDocument doc)
        {
            List<GXDLMSXmlPdu> actions = new List<GXDLMSXmlPdu>();
            foreach (XmlNode m1 in doc.ChildNodes)
            {
                if (m1.NodeType == XmlNodeType.Element)
                {
                    foreach (XmlNode node in m1.ChildNodes)
                    {
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            GXDLMSXmlSettings s = new GXDLMSXmlSettings(translator.OutputType, translator.Hex, translator.ShowStringAsHex, translator.tagsByName); ;
                            s.settings.ClientAddress = Settings.ClientAddress;
                            s.settings.ServerAddress = Settings.ServerAddress;
                            byte[] reply = translator.XmlToPdu(node.OuterXml, s);
                            if (s.command == Command.Snrm && !s.settings.IsServer)
                            {
                                Settings.Limits.MaxInfoTX = s.settings.Limits.MaxInfoTX;
                                Settings.Limits.MaxInfoRX = s.settings.Limits.MaxInfoRX;
                                Settings.Limits.WindowSizeRX = s.settings.Limits.WindowSizeRX;
                                Settings.Limits.WindowSizeTX = s.settings.Limits.WindowSizeTX;
                            }
                            else if (s.command == Command.Ua && s.settings.IsServer)
                            {
                                Settings.Limits.MaxInfoTX = s.settings.Limits.MaxInfoTX;
                                Settings.Limits.MaxInfoRX = s.settings.Limits.MaxInfoRX;
                                Settings.Limits.WindowSizeRX = s.settings.Limits.WindowSizeRX;
                                Settings.Limits.WindowSizeTX = s.settings.Limits.WindowSizeTX;
                            }
                            if (s.template)
                            {
                                reply = null;
                            }
                            actions.Add(new GXDLMSXmlPdu(s.command, node, reply));
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
                if (Settings.InterfaceType == Enums.InterfaceType.WRAPPER)
                {
                    reply = new GXByteBuffer(pdu.Data);
                }
                else
                {
                    reply = new GXByteBuffer((UInt16)(3 + pdu.Data.Length));
                    reply.Set(GXCommon.LLCSendBytes);
                    reply.Set(pdu.Data);
                }
                while (reply.Position != reply.Size)
                {
                    if (Settings.InterfaceType == Enums.InterfaceType.WRAPPER)
                    {
                        messages.Add(GXDLMS.GetWrapperFrame(Settings, reply));
                    }
                    else if (Settings.InterfaceType == Enums.InterfaceType.HDLC)
                    {
                        byte frame = 0;
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
                            if (Settings.IsServer || pdu.Command == Command.SetRequest ||
                                pdu.Command == Command.MethodRequest)
                            {
                                frame = 0;
                            }
                            else
                            {
                                frame = Settings.NextSend(false);
                            }
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