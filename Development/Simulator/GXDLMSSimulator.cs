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
using System.Text;
using Gurux.DLMS.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
#if !WINDOWS_UWP
using System.Xml.XPath;
#endif
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Objects;
using System.ComponentModel;
using System.Diagnostics;

namespace Gurux.DLMS.Simulator
{
    ///<summary>
    ///This class is used to translate DLMS frame or PDU to xml.
    ///</summary>
    public class GXDLMSSimulator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        private GXDLMSSimulator()
        {
        }

        /// <summary>
        /// Get logical names from client request.
        /// </summary>
        /// <param name="nodes">XML nodes.</param>
        /// <returns>Collection of logical names.</returns>
        private static void GetLN(GXDLMSObjectCollection objects, List<ValueEventArgs> targets, XmlNodeList nodes)
        {
            List<ValueEventArgs> list = new List<ValueEventArgs>();
            foreach (XmlNode node in nodes)
            {
                ObjectType ot;
                string classId, instanceId;
                int attributeId;
                if (node.Name == "cosem-attribute-descriptor")
                {
                    classId = node.ChildNodes[0].ChildNodes[0].InnerText;
                    instanceId = node.ChildNodes[1].ChildNodes[0].InnerText;
                    instanceId = GXCommon.ToLogicalName(GXCommon.HexToBytes(instanceId));
                    attributeId = int.Parse(node.ChildNodes[2].ChildNodes[0].InnerText);
                    ot = (ObjectType)int.Parse(classId);
                    GXDLMSObject t = objects.FindByLN(ot, instanceId);
                    if (t == null)
                    {
                        t = GXDLMSClient.CreateDLMSObject((int)ot, 0, 0, instanceId, null, 2);
                    }
                    ValueEventArgs ve = new ValueEventArgs(t, attributeId, 0, null);
                    targets.Add(ve);
                }
                else if ("AttributeDescriptorList".Equals(node.Name))
                {
                    foreach (XmlNode it in node.ChildNodes)
                    {
                        classId = it.ChildNodes[0].ChildNodes[0].InnerText;
                        instanceId = it.ChildNodes[0].ChildNodes[1].InnerText;
                        instanceId = GXCommon.ToLogicalName(GXCommon.HexToBytes(instanceId));
                        attributeId = int.Parse(it.ChildNodes[0].ChildNodes[2].InnerText);
                        ot = (ObjectType)int.Parse(classId);
                        ValueEventArgs ve = new ValueEventArgs(objects.FindByLN(ot, instanceId), attributeId, 0, null);
                        targets.Add(ve);
                    }
                }
            }
        }

        /// <summary>
        /// Get values from server response.
        /// </summary>
        /// <param name="nodes">XML nodes.</param>
        /// <returns>Collection of requested values.</returns>
        private static bool IsLastBlock(XmlNodeList nodes)
        {
            List<string> list = new List<string>();
            foreach (XmlNode node in nodes)
            {
                if (node.ChildNodes[1].ChildNodes[0].Name == "LastBlock")
                {
                    return int.Parse(node.ChildNodes[1].ChildNodes[0].InnerXml) != 0;
                }
            }
            return true;
        }
        /// <summary>
        /// Get values from server response.
        /// </summary>
        /// <param name="nodes">XML nodes.</param>
        /// <returns>Collection of requested values.</returns>
        private static List<string> GetLNValues(XmlNodeList nodes)
        {
            List<string> list = new List<string>();
            foreach (XmlNode node in nodes)
            {
                if ("get-response-with-data-block".Equals(node.Name))
                {
                    list.Add(node.ChildNodes[1].ChildNodes[2].InnerXml);
                }
                else if ("get-response-with-list".Equals(node.Name))
                {
                    return GetLNValues(node.ChildNodes[1].ChildNodes);
                }
                else if ("data".Equals(node.Name))
                {
                    list.Add(node.InnerXml);
                }
                else if ("get-response-normal".Equals(node.Name))
                {
                    list.Add(node.ChildNodes[1].ChildNodes[0].InnerXml);
                }
                else
                {
                    throw new ArgumentException(node.Name);
                }
            }
            return list;
        }

        /// <summary>
        /// Get short names from client request.
        /// </summary>
        /// <param name="nodes">XML nodes.</param>
        /// <returns>Collection of short names.</returns>
        private static List<short> GetSN(XmlNodeList nodes)
        {
            List<short> list = new List<short>();
            foreach (XmlNode node in nodes)
            {
                //Normal read.
                if (node.ChildNodes.Count == 1)
                {
                    list.Add(short.Parse(node.ChildNodes[0].InnerText));
                }
                else if (node.ChildNodes.Count != 0)
                {
                    //Read with parameters.
                    list.Add(short.Parse(node.ChildNodes[0].ChildNodes[0].InnerText));
                }
            }
            return list;
        }
        /// <summary>
        /// Get values from server response.
        /// </summary>
        /// <param name="nodes">XML nodes.</param>
        /// <returns>Collection of requested values.</returns>
        private static List<string> GetSNValues(XmlNodeList nodes)
        {
            List<string> list = new List<string>();
            foreach (XmlNode node in nodes)
            {
                list.Add(node.ChildNodes[0].InnerXml);
            }
            return list;
        }

        /// <summary>
        /// Import server settings and COSEM objects from GXDLMSDirector trace.
        /// </summary>
        /// <param name="server">Server where settings are updated.</param>
        /// <param name="data">GXDLMSDirector trace in byte array.</param>
        public static void Import(GXDLMSServer server, byte[] data)
        {
            Import(server, data, Standard.DLMS);
        }

        /// <summary>
        /// Import server settings and COSEM objects from GXDLMSDirector trace.
        /// </summary>
        /// <param name="server">Server where settings are updated.</param>
        /// <param name="data">GXDLMSDirector trace in byte array.</param>
        public static void Import(GXDLMSServer server, byte[] data, Standard standard)
        {
            GXByteBuffer bb = new GXByteBuffer(data);
            Import(server, bb, standard);
        }


        /// <summary>
        /// Import server settings and COSEM objects from GXDLMSDirector trace.
        /// </summary>
        /// <param name="server">Server where settings are updated.</param>
        /// <param name="data">GXDLMSDirector trace in byte array.</param>
        public static void Import(GXDLMSServer server, GXByteBuffer data)
        {
            Import(server, data, Standard.DLMS);
        }

        /// <summary>
        /// Import server settings and COSEM objects from GXDLMSDirector trace.
        /// </summary>
        /// <param name="server">Server where settings are updated.</param>
        /// <param name="data">GXDLMSDirector trace in byte array.</param>
        public static void Import(GXDLMSServer server, GXByteBuffer data, Standard standard)
        {
            GXDLMSTranslator translator = new GXDLMSTranslator(TranslatorOutputType.StandardXml);
            translator.CompletePdu = true;
            translator.PduOnly = true;
            translator.OmitXmlNameSpace = translator.OmitXmlDeclaration = true;
            XmlDocument doc = new XmlDocument();
            List<ValueEventArgs> targets = new List<ValueEventArgs>();
            GXDLMSSettings settings = new GXDLMSSettings(true, InterfaceType.HDLC);
            GXByteBuffer pdu = new GXByteBuffer();
            server.InterfaceType = GXDLMSTranslator.GetDlmsFraming(data);
            bool lastBlock = true;
            GXByteBuffer val = new DLMS.GXByteBuffer();
            GXDLMSConverter converter = new GXDLMSConverter(standard);
            bool newMeter = false;
            while (translator.FindNextFrame(data, pdu, server.InterfaceType))
            {
                if (newMeter)
                {
                    break;
                }
                try
                {
                    String xml = translator.MessageToXml(data);
                    if (xml != "")
                    {
                        doc.LoadXml(xml.Replace("&", "&amp;"));
                        foreach (XmlNode node in doc.ChildNodes[doc.ChildNodes.Count - 1].ChildNodes)
                        {
                            string name = doc.ChildNodes[doc.ChildNodes.Count - 1].Name;
                            if (name == "Ua" || name == "aarq" || name == "aare")
                            {
                                break;
                            }
                            else if (name == "get-request")
                            {
                                server.UseLogicalNameReferencing = true;
                                GetLN(settings.Objects, targets, node.ChildNodes);
                            }
                            else if (name == "readRequest")
                            {
                                List<short> items = GetSN(node.ChildNodes);

                                server.UseLogicalNameReferencing = false;
                                foreach (short it in items)
                                {
                                    GXSNInfo i = GXDLMSSNCommandHandler.FindSNObject(settings.Objects, Convert.ToUInt16((it) & 0xFFFF));
                                    targets.Add(new ValueEventArgs(i.Item, i.Index, 0, null));
                                }
                            }
                            else if (name == "readResponse" ||
                                     name == "get-response")
                            {
                                if (targets != null)
                                {
                                    List<string> items;
                                    if (server.UseLogicalNameReferencing)
                                    {
                                        items = GetLNValues(doc.ChildNodes[doc.ChildNodes.Count - 1].ChildNodes);
                                    }
                                    else
                                    {
                                        items = GetSNValues(doc.ChildNodes[doc.ChildNodes.Count - 1].ChildNodes);
                                    }

                                    int pos = 0;
                                    foreach (string it in items)
                                    {
                                        if ("other-reason".Equals(it) ||
                                            "read-write-denied".Equals(it) ||
                                                "scope-of-access-violated".Equals(it) ||
                                                "object-unavailable".Equals(it) ||
                                                 "object-class-inconsistent".Equals(it))
                                        {
                                            ValueEventArgs ve = targets[pos];
                                            ve.Target.SetAccess(ve.Index, AccessMode.NoAccess);
                                            continue;
                                        }
                                        try
                                        {
                                            if (server.UseLogicalNameReferencing)
                                            {
                                                lastBlock = IsLastBlock(doc.ChildNodes[doc.ChildNodes.Count - 1].ChildNodes);
                                            }
                                            val.Set(translator.XmlToData(it));
                                            if (lastBlock)
                                            {
                                                if (settings.Objects.Count == 0)
                                                {
                                                    GXDLMSClient c = new GXDLMSClient();
                                                    c.UseLogicalNameReferencing = server.UseLogicalNameReferencing;
                                                    settings.Objects = c.ParseObjects(val, true);
                                                    //Update OBIS code description.
                                                    converter.UpdateOBISCodeInformation(settings.Objects);
                                                }
                                                else if (targets.Count != 0)
                                                {
                                                    ValueEventArgs ve = targets[pos];
                                                    GXDataInfo info = new GXDataInfo();
                                                    ve.Value = GXCommon.GetData(server.Settings, val, info);
                                                    if (ve.Value is byte[] && ve.Target != null)
                                                    {
                                                        DataType tp = ve.Target.GetUIDataType(ve.Index);
                                                        if (tp != DataType.None)
                                                        {
                                                            ve.Value = GXDLMSClient.ChangeType((byte[])ve.Value, tp, false);
                                                            ve.Target.SetDataType(ve.Index, DataType.OctetString);
                                                        }
                                                    }
                                                    if (ve.Target is IGXDLMSBase)
                                                    {
                                                        ((IGXDLMSBase)ve.Target).SetValue(settings, ve);
                                                    }
                                                }
                                                val.Clear();
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            ValueEventArgs ve = targets[pos];
                                            if (ve.Target != null)
                                            {
                                                ve.Target.SetAccess(ve.Index, AccessMode.NoAccess);
                                            }
                                        }
                                        ++pos;
                                    }
                                    if (lastBlock)
                                    {
                                        targets.Clear();
                                        break;
                                    }
                                }
                            }
                            else if (name == "rlre")
                            {
                                newMeter = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    continue;
                }
            }
            server.Items.Clear();
            server.Items.AddRange(settings.Objects);
        }
    }
}