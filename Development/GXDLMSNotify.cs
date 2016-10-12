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
        /// <param name="ServerAddress">Server ID. Default is 1.</param>
        /// <param name="interfaceType">Interface type. Default is general.</param>
        public GXDLMSNotify(bool useLogicalNameReferencing,
            int clientAddress, int serverAddress, InterfaceType interfaceType)
        {
            Settings = new GXDLMSSettings(true);
            Settings.UseLogicalNameReferencing = useLogicalNameReferencing;
            Settings.InterfaceType = interfaceType;
            Settings.ServerAddress = serverAddress;
            Settings.ClientAddress = clientAddress;
        }

        /// <summary>
        /// Is General block transfer supported.
        /// </summary>
        public bool GeneralBlockTransfer
        {
            get
            {
                if (Settings.UseLogicalNameReferencing)
                {
                    return Settings.LnSettings.GeneralBlockTransfer;
                }
                return Settings.SnSettings.GeneralBlockTransfer;
            }
            set
            {
                if (Settings.UseLogicalNameReferencing)
                {
                    Settings.LnSettings.GeneralBlockTransfer = value;
                }
                else
                {
                    Settings.SnSettings.GeneralBlockTransfer = value;
                }
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
        /// Information from the connection size that server can handle.
        /// </summary>
        public GXDLMSLimits Limits
        {
            get
            {
                return Settings.Limits;
            }
        }

        /// <summary>
        /// Retrieves the maximum size of PDU receiver.
        /// </summary>
        /// <remarks>
        /// PDU size tells maximum size of PDU packet.
        /// Value can be from 0 to 0xFFFF. By default the value is 0xFFFF.
        /// </remarks>
        /// <seealso cref="DLMSVersion"/>
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
        /// <seealso cref="DLMSVersion"/>
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
            return GXDLMS.GetData(Settings, reply, data);
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
        /// <seealso cref="GetDataNotificationMessages"/>
        internal static void AddData(GXDLMSSettings settings, GXDLMSObject obj, int index, GXByteBuffer buff)
        {
            DataType dt;
            object value = (obj as IGXDLMSBase).GetValue(settings, new ValueEventArgs(settings, obj, index, 0, null));
            dt = obj.GetDataType(index);
            if (dt == DataType.None && value != null)
            {
                dt = GXCommon.GetValueType(value);
            }
            GXCommon.SetData(buff, dt, value);
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
        /// <seealso cref="GetDataNotificationMessages"/>
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
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, Command.DataNotification, 0, null, new GXByteBuffer(data), 0xff);
                p.time = time;
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.DataNotification, 1, 0, new GXByteBuffer(data), null);
                reply = GXDLMS.GetSnMessages(p);
            }
            if (!GeneralBlockTransfer && reply.Length != 1)
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
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, Command.DataNotification, 0, null, data, 0xff);
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
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, Command.DataNotification, 0, null, buff, 0xff);
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
            foreach (KeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in push.PushObjectList)
            {
                AddData(it.Key, it.Value.AttributeIndex, buff);
            }
            return GenerateDataNotificationMessages(date, buff);
        }

        /// <summary>
        /// Returns collection of push objects.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <returns>Array of objects and called indexes.</returns>
        public List<KeyValuePair<GXDLMSObject, int>> 
            ParsePush(Object[] data)
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
                foreach (Object it in (Object[])data[0])
                {
                    Object[] tmp = (Object[])it;
                    int classID = ((UInt16)(tmp[0])) & 0xFFFF;
                    if (classID > 0)
                    {
                        GXDLMSObject comp;
                        comp = this.Objects.FindByLN((ObjectType)classID, GXDLMSObject.ToLogicalName(tmp[1] as byte[]));
                        if (comp == null)
                        {
                            comp = GXDLMSClient.CreateDLMSObject(classID, 0, 0, tmp[1], null);
                            c.UpdateOBISCodeInformation(comp);
                            objects.Add(comp);
                        }
                        if ((comp is IGXDLMSBase))
                        {
                            items.Add(new KeyValuePair<GXDLMSObject, int>(comp, (sbyte)tmp[2]));
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}",
                                classID, GXDLMSObject.ToLogicalName((byte[])tmp[1])));
                        }
                    }
                }
                for (int pos = 0; pos < data.Length; ++pos)
                {
                    obj = items[pos].Key as GXDLMSObject;
                    value = data[pos];
                    index = items[pos].Value;
                    if (value is byte[] && (dt = obj.GetUIDataType(index)) != DataType.None)
                    {
                        value = GXDLMSClient.ChangeType(value as byte[], dt);
                    }
                    ValueEventArgs e = new ValueEventArgs(Settings, obj, index, 0, null);
                    e.Value = value;
                    (obj as IGXDLMSBase).SetValue(Settings, e);
                }
            }
            return items;
        }       
    }
}
