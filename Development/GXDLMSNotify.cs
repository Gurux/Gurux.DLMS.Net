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

using Gurux.DLMS.Enums;
using System.Collections.Generic;
using Gurux.DLMS.Objects;
using System;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Secure;
using System.ComponentModel;

namespace Gurux.DLMS
{
    /// <summary>
    /// This class is used to send data notify and push messages to the clients.
    /// </summary>
    public class GXDLMSNotify
    {

        /// <summary>
        /// DLMS settings.
        /// </summary>
        protected GXDLMSSettings Settings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="useLogicalNameReferencing">Is Logical or short name referencing used.</param>
        /// <param name="clientAddress">Client address. Default is 0x10</param>
        /// <param name="serverAddress">Server ID. Default is 1.</param>
        /// <param name="interfaceType">Interface type. Default is general.</param>
        public GXDLMSNotify(bool useLogicalNameReferencing,
            int clientAddress, int serverAddress, InterfaceType interfaceType)
        {
            Settings = new GXDLMSSettings(true, interfaceType);
            Settings.UseLogicalNameReferencing = useLogicalNameReferencing;
            Settings.ServerAddress = serverAddress;
            Settings.ClientAddress = clientAddress;
        }

        /// <summary>
        ///  What kind of services are used.
        /// </summary>
        public Conformance Conformance
        {
            get
            {
                return Settings.NegotiatedConformance;
            }
            set
            {
                Settings.NegotiatedConformance = value;
            }
        }

        /// <summary>
        /// Used priority in General Block Transfer.
        /// </summary>
        public Priority Priority
        {
            get
            {
                return Settings.Priority;
            }
            set
            {
                Settings.Priority = value;
            }
        }
        /// <summary>
        /// HDLC connection settings.
        /// </summary>
        [Obsolete("Use HdlcSettings instead.")]
        public GXDLMSLimits Limits
        {
            get
            {
                return (GXDLMSLimits)Settings.Hdlc;
            }
        }

        /// <summary>
        /// HDLC connection settings.
        /// </summary>
        public GXHdlcSettings HdlcSettings
        {
            get
            {
                return Settings.Hdlc;
            }
        }

        /// <summary>
        /// Retrieves the maximum size of PDU receiver.
        /// </summary>
        /// <remarks>
        /// PDU size tells maximum size of PDU packet.
        /// Value can be from 0 to 0xFFFF. By default the value is 0xFFFF.
        /// </remarks>
        /// <seealso cref="UseLogicalNameReferencing"/>
        [DefaultValue(0xFFFF)]
        public ushort MaxReceivePDUSize
        {
            get
            {
                return Settings.MaxPduSize;
            }
            set
            {
                Settings.MaxPduSize = value;
            }
        }

        /// <summary>
        /// Determines, whether Logical, or Short name, referencing is used.
        /// </summary>
        /// <remarks>
        /// Referencing depends on the device to communicate with.
        /// Normally, a device supports only either Logical or Short name referencing.
        /// The referencing is defined by the device manufacturer.
        /// If the referencing is wrong, the SNMR message will fail.
        /// </remarks>
        /// <seealso cref="MaxReceivePDUSize"/>
        [DefaultValue(false)]
        public bool UseLogicalNameReferencing
        {
            get
            {
                return Settings.UseLogicalNameReferencing;
            }
            private set
            {
                Settings.UseLogicalNameReferencing = value;
            }
        }

        /// <summary>
        /// Used service class.
        /// </summary>
        public ServiceClass ServiceClass
        {
            get
            {
                return Settings.ServiceClass;
            }
            set
            {
                Settings.ServiceClass = value;
            }
        }

        /// <summary>
        /// Available objects.
        /// </summary>
        public GXDLMSObjectCollection Objects
        {
            get
            {
                return Settings.Objects;
            }
        }

        ///<summary>
        /// Removes the HDLC frame from the packet, and returns COSEM data only.
        ///</summary>
        ///<param name="reply">
        /// The received data from the device.
        ///</param>
        ///<param name="data">
        /// Information from the received data.
        ///</param>
        ///<returns>
        /// Is frame complete.
        ///</returns>
        public bool GetData(GXByteBuffer reply, GXReplyData data)
        {
            return GXDLMS.GetData(Settings, reply, data, null);
        }

        ///<summary>
        /// Removes the HDLC frame from the packet, and returns COSEM data only.
        ///</summary>
        ///<param name="reply">
        /// The received data from the device.
        ///</param>
        ///<param name="data">
        /// Information from the received data.
        ///</param>
        ///<returns>
        /// Is frame complete.
        ///</returns>
        public bool GetData(GXByteBuffer reply, GXReplyData data, GXReplyData notify)
        {
            return GXDLMS.GetData(Settings, reply, data, notify);
        }

        /// <summary>
        /// Add value of COSEM object to byte buffer.
        /// </summary>
        /// <param name="obj">COSEM object.</param>
        /// <param name="index">Attribute index.</param>
        /// <param name="buff">Byte buffer.</param>
        /// <remarks>
        /// AddData method can be used with GetDataNotificationMessage -method.
        /// DLMS spesification do not specify the structure of Data-Notification body.
        /// So each manufacture can sent different data.
        /// </remarks>
        internal static void AddData(GXDLMSSettings settings, GXDLMSObject obj, int index, GXByteBuffer buff)
        {
            DataType dt;
            object value = (obj as IGXDLMSBase).GetValue(settings, new ValueEventArgs(settings, obj, index, 0, null));
            dt = obj.GetDataType(index);
            if (dt == DataType.None && value != null)
            {
                dt = GXDLMSConverter.GetDLMSDataType(value);
            }
            GXCommon.SetData(settings, buff, dt, value);
        }

        /// <summary>
        /// Add value of COSEM object to byte buffer.
        /// </summary>
        /// <param name="obj">COSEM object.</param>
        /// <param name="index">Attribute index.</param>
        /// <param name="buff">Byte buffer.</param>
        /// <remarks>
        /// AddData method can be used with GetDataNotificationMessage -method.
        /// DLMS spesification do not specify the structure of Data-Notification body.
        /// So each manufacture can sent different data.
        /// </remarks>
        /// <seealso cref="GenerateDataNotificationMessages"/>
        public void AddData(GXDLMSObject obj, int index, GXByteBuffer buff)
        {
            AddData(Settings, obj, index, buff);
        }

        /// <summary>
        /// Generates data notification message(s).
        /// </summary>
        /// <param name="time">Date time. Set To Min or Max if not added</param>
        /// <param name="data">Notification body.</param>
        /// <returns>Generated data notification message(s).</returns>
        public byte[][] GenerateDataNotificationMessages(DateTime time, byte[] data)
        {
            byte[][] reply;
            if (UseLogicalNameReferencing)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.DataNotification, 0, null, new GXByteBuffer(data), 0xff, Command.None);
                p.time = time;
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.DataNotification, 1, 0, new GXByteBuffer(data), null);
                reply = GXDLMS.GetSnMessages(p);
            }
            if ((Settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) == 0 && reply.Length != 1)
            {
                throw new ArgumentException("Data is not fit to one PDU. Use general block transfer.");
            }
            return reply;
        }

        /// <summary>
        /// Generates data notification message(s).
        /// </summary>
        /// <param name="time">Date time. Set To Min or Max if not added</param>
        /// <param name="data">Notification body.</param>
        /// <returns>Generated data notification message(s).</returns>
        public byte[][] GenerateDataNotificationMessages(DateTime time, GXByteBuffer data)
        {
            byte[][] reply;
            if (UseLogicalNameReferencing)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.DataNotification, 0, null, data, 0xff, Command.None);
                p.time = time;
                p.time.Skip |= DateTimeSkips.Ms;
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.DataNotification, 1, 0, data, null);
                reply = GXDLMS.GetSnMessages(p);
            }
            return reply;
        }

        /// <summary>
        /// Generates data notification message.
        /// </summary>
        /// <param name="time">Date time. Set To Min or Max if not added</param>
        /// <param name="objects">List of objects and attribute indexes to notify.</param>
        /// <returns>Generated data notification message(s).</returns>
        public byte[][] GenerateDataNotificationMessages(DateTime time, List<KeyValuePair<GXDLMSObject, int>> objects)
        {
            if (objects == null)
            {
                throw new ArgumentNullException("objects");
            }
            GXByteBuffer buff = new GXByteBuffer();
            buff.SetUInt8(DataType.Structure);
            GXCommon.SetObjectCount(objects.Count, buff);
            foreach (KeyValuePair<GXDLMSObject, int> it in objects)
            {
                AddData(it.Key, it.Value, buff);
            }
            byte[][] reply;
            if (UseLogicalNameReferencing)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.DataNotification, 0, null, buff, 0xff, Command.None);
                p.time = time;
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.DataNotification, 1, 0, buff, null);
                reply = GXDLMS.GetSnMessages(p);
            }
            return reply;
        }

        /// <summary>
        /// Generates push setup message.
        /// </summary>
        /// <param name="date"> Date time. Set To Min or Max if not added.</param>
        /// <param name="push">Target Push object.</param>
        /// <returns>Generated data notification message(s).</returns>
        public byte[][] GeneratePushSetupMessages(DateTime date, GXDLMSPushSetup push)
        {
            if (push == null)
            {
                throw new ArgumentNullException("push");
            }
            GXByteBuffer buff = new GXByteBuffer();
            buff.SetUInt8((byte)DataType.Structure);
            GXCommon.SetObjectCount(push.PushObjectList.Count, buff);
            foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in push.PushObjectList)
            {
                if (it.Value.AttributeIndex == 0)
                {
                    buff.SetUInt8(DataType.Structure);
                    int count = (it.Key as IGXDLMSBase).GetAttributeCount();
                    GXCommon.SetObjectCount(count, buff);
                    for (int index = 1; index <= count; ++index)
                    {
                        AddData(it.Key, index, buff);
                    }
                }
                else
                {
                    AddData(it.Key, it.Value.AttributeIndex, buff);
                }
            }
            return GenerateDataNotificationMessages(date, buff);
        }

        /// <summary>
        /// Returns collection of push objects.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <returns>Array of objects and called indexes.</returns>
        public List<KeyValuePair<GXDLMSObject, int>> ParsePush(List<object> data)
        {
            int index;
            GXDLMSObject obj;
            object value;
            DataType dt;
            List<KeyValuePair<GXDLMSObject, int>> items = new List<KeyValuePair<GXDLMSObject, int>>();
            if (data != null)
            {
                GXDLMSConverter c = new GXDLMSConverter();
                GXDLMSObjectCollection objects = new GXDLMSObjectCollection();
                foreach (List<object> it in (List<object>)data[0])
                {
                    int classID = ((UInt16)(it[0])) & 0xFFFF;
                    if (classID > 0)
                    {
                        GXDLMSObject comp;
                        comp = this.Objects.FindByLN((ObjectType)classID, GXCommon.ToLogicalName(it[1] as byte[]));
                        if (comp == null)
                        {
                            comp = GXDLMSClient.CreateDLMSObject(Settings, classID, 0, 0, it[1], null, 2);
                            c.UpdateOBISCodeInformation(comp);
                            objects.Add(comp);
                        }
                        if ((comp is IGXDLMSBase))
                        {
                            items.Add(new KeyValuePair<GXDLMSObject, int>(comp, (sbyte)it[2]));
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}",
                                classID, GXCommon.ToLogicalName((byte[])it[1])));
                        }
                    }
                }
                for (int pos = 0; pos < data.Count; ++pos)
                {
                    obj = items[pos].Key as GXDLMSObject;
                    value = data[pos];
                    index = items[pos].Value;
                    if (value is byte[] && (dt = obj.GetUIDataType(index)) != DataType.None)
                    {
                        value = GXDLMSClient.ChangeType(value as byte[], dt, Settings.UseUtc2NormalTime);
                    }
                    ValueEventArgs e = new ValueEventArgs(Settings, obj, index, 0, null);
                    e.Value = value;
                    (obj as IGXDLMSBase).SetValue(Settings, e);
                }
            }
            return items;
        }

        /// <summary>
        /// Sends Event Notification or Information Report Request.
        /// </summary>
        /// <param name="time">Send time.</param>
        /// <param name="list">List of COSEM object and attribute index to report.</param>
        /// <returns>Report request as byte array.</returns>
        public byte[][] GenerateReport(DateTime time, List<KeyValuePair<GXDLMSObject, int>> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentNullException("list");
            }
            if (UseLogicalNameReferencing && list.Count != 1)
            {
                throw new ArgumentException("Only one object can send with Event Notification request.");
            }

            GXByteBuffer buff = new GXByteBuffer();
            byte[][] reply;
            if (UseLogicalNameReferencing)
            {
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    buff.SetUInt16((ushort)it.Key.ObjectType);
                    buff.Set(GXCommon.LogicalNameToBytes(it.Key.LogicalName));
                    buff.SetUInt8((byte)it.Value);
                    AddData(it.Key, it.Value, buff);
                }
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.EventNotification, 0, null, buff, 0xff, Command.None);
                p.time = time;
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.InformationReport, list.Count, 0xFF, null, buff);
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    // Add variable type.
                    buff.SetUInt8(VariableAccessSpecification.VariableName);
                    int sn = it.Key.ShortName;
                    sn += (it.Value - 1) * 8;
                    buff.SetUInt16((UInt16)sn);
                }
                GXCommon.SetObjectCount(list.Count, buff);
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    AddData(it.Key, it.Value, buff);
                }
                reply = GXDLMS.GetSnMessages(p);
            }
            return reply;
        }


        /// <summary>
        /// Sends Event Notification Request.
        /// </summary>
        /// <param name="time">Send time.</param>
        /// <param name="item">COSEM object and attribute index to report.</param>
        /// <returns>Report request as byte array.</returns>
        public byte[][] GenerateEventNotification(DateTime time, KeyValuePair<GXDLMSObject, int> item)
        {
            GXByteBuffer buff = new GXByteBuffer();
            byte[][] reply;
            if (UseLogicalNameReferencing)
            {
                buff.SetUInt16((ushort)item.Key.ObjectType);
                buff.Set(GXCommon.LogicalNameToBytes(item.Key.LogicalName));
                buff.SetUInt8((byte)item.Value);
                AddData(item.Key, item.Value, buff);
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.EventNotification, 0, null, buff, 0xff, Command.None);
                p.time = time;
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                throw new Exception("Use GenerateInformationReport when Short Name referencing is used.");
            }
            return reply;
        }

        /// <summary>
        /// Sends Information Report Request.
        /// </summary>
        /// <param name="time">Send time.</param>
        /// <param name="list">List of COSEM object and attribute index to report.</param>
        /// <returns>Report request as byte array.</returns>
        public byte[][] GenerateInformationReport(DateTime time, List<KeyValuePair<GXDLMSObject, int>> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentNullException("list");
            }
            GXByteBuffer buff = new GXByteBuffer();
            byte[][] reply;
            if (UseLogicalNameReferencing)
            {
                throw new Exception("Use GenerateEventNotification when Logical Name referencing is used.");
            }
            else
            {
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.InformationReport, list.Count, 0xFF, null, buff);
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    // Add variable type.
                    buff.SetUInt8(VariableAccessSpecification.VariableName);
                    int sn = it.Key.ShortName;
                    sn += (it.Value - 1) * 8;
                    buff.SetUInt16((UInt16)sn);
                }
                GXCommon.SetObjectCount(list.Count, buff);
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    AddData(it.Key, it.Value, buff);
                }
                reply = GXDLMS.GetSnMessages(p);
            }
            return reply;
        }
    }
}
