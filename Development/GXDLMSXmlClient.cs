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
using System.ComponentModel;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;
using System.Xml;
using System.IO;

namespace Gurux.DLMS
{
    public class GXDLMSXmlMessage
    {
        private XmlNode XmlNode;

        /// <summary>
        /// Command.
        /// </summary>
        public Command Command
        {
            get;
            set;
        }

        /// <summary>
        /// Generated messages.
        /// </summary>
        public byte[][] Messages
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

        private static void Compare(XmlNode node1, XmlNode node2, List<string> list)
        {
            int cnt = node1.ChildNodes.Count;
            if (string.Compare(node1.Name, node2.Name) != 0)
            {
                list.Add(node1.Name + "-" + node2.Name);
            }
            else if (cnt != node2.ChildNodes.Count)
            {
                list.Add("Different amount: " + node1.Name + "-" + node2.Name);
            }
            else
            {
                for (int pos = 0; pos != cnt; ++pos)
                {
                    if (node2.ChildNodes[pos] == null)
                    {
                        list.Add("Different values. Expected: '" + node1.ChildNodes[pos].OuterXml + "'. Actual: 'null'.");
                    }
                    else if (string.Compare(node1.ChildNodes[pos].OuterXml, node2.ChildNodes[pos].OuterXml) != 0)
                    {
                        list.Add("Different values. Expected: '" + node1.ChildNodes[pos].OuterXml + "'. Actual: '" + node2.ChildNodes[pos].OuterXml + "'.");
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
        public GXDLMSXmlMessage()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="xml"></param>
        /// <param name="messages"></param>
        public GXDLMSXmlMessage(Command command, XmlNode xml, byte[][] messages)
        {
            Command = command;
            XmlNode = xml;
            Messages = messages;
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
        }

        /// <summary>
        /// Load XML commands from the file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<GXDLMSXmlMessage> Load(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(fileName));
            List<GXDLMSXmlMessage> actions = new List<GXDLMSXmlMessage>();
            foreach (XmlNode m1 in doc.ChildNodes)
            {
                if (m1.NodeType == XmlNodeType.Element)
                {
                    foreach (XmlNode node in m1.ChildNodes)
                    {
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            List<byte[]> messages = new List<byte[]>();
                            GXDLMSXmlSettings s = new GXDLMSXmlSettings(translator.OutputType, translator.Hex, translator.ShowStringAsHex, translator.tagsByName); ;
                            s.settings.ClientAddress = Settings.ClientAddress;
                            s.settings.ServerAddress = Settings.ServerAddress;
                            GXByteBuffer reply = new GXByteBuffer(translator.XmlToPdu(node.OuterXml, s));
                            if (s.command == Command.Snrm)
                            {
                                messages.Add(reply.Array());
                            }
                            else if (s.command == Command.Ua)
                            {
                                Settings.Limits.MaxInfoTX = s.settings.Limits.MaxInfoTX;
                                Settings.Limits.MaxInfoRX = s.settings.Limits.MaxInfoRX;
                                Settings.Limits.WindowSizeRX = s.settings.Limits.WindowSizeRX;
                                Settings.Limits.WindowSizeTX = s.settings.Limits.WindowSizeTX;
                                messages.Add(reply.Array());
                            }
                            else if (s.command == Command.DisconnectRequest)
                            {
                                messages.Add(GXDLMS.GetHdlcFrame(Settings, (byte)Command.DisconnectRequest, reply));
                            }
                            else
                            {
                                while (reply.Position != reply.Size)
                                {
                                    if (Settings.InterfaceType == Enums.InterfaceType.WRAPPER)
                                    {
                                        messages.Add(GXDLMS.GetWrapperFrame(Settings, reply));
                                    }
                                    else if (Settings.InterfaceType == Enums.InterfaceType.HDLC)
                                    {
                                        byte frame = 0;
                                        if (s.command == Command.Aarq)
                                        {
                                            frame = 0x10;
                                        }
                                        else if (s.command == Command.Aare)
                                        {
                                            frame = 0x30;
                                        }
                                        else if (s.command == Command.EventNotification)
                                        {
                                            frame = 0x13;
                                        }
                                        messages.Add(GXDLMS.GetHdlcFrame(Settings, frame, reply));
                                        if (reply.Position != reply.Size)
                                        {
                                            if (Settings.IsServer || s.command == Command.SetRequest)
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
                            actions.Add(new GXDLMSXmlMessage(s.command, node, messages.ToArray()));
                        }
                    }
                }
            }
            return actions;
        }

        public byte[][] GenerateMessages(string xml)
        {
            return null;
        }

        public string ParseData(GXByteBuffer bb)
        {
            return null;
        }

    }
}