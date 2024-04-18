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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Last received frame counter and client identification.
    /// </summary>
    public class GXBroadcastFrameCounter
    {
        /// <summary>
        /// Client ID.
        /// </summary>
        public byte ClientId
        {
            get;
            set;
        }

        /// <summary>
        /// Counter.
        /// </summary>
        public UInt32 Counter
        {
            get;
            set;
        }

        /// <summary>
        /// Time stamp.
        /// </summary>
        public GXDateTime TimeStamp
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Last changed value.
    /// </summary>
    public class GXCaptureTime
    {
        /// <summary>
        /// Attribute ID.
        /// </summary>
        public byte AttributeId
        {
            get;
            set;
        }

        /// <summary>
        /// Time stamp.
        /// </summary>
        public GXDateTime TimeStamp
        {
            get;
            set;
        }
    }


    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSMBusDiagnostic
    /// </summary>
    public class GXDLMSMBusDiagnostic : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSMBusDiagnostic()
        : this("0.0.24.9.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSMBusDiagnostic(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSMBusDiagnostic(string ln, ushort sn)
        : base(ObjectType.MBusDiagnostic, ln, sn)
        {
            BroadcastFrames = new List<GXBroadcastFrameCounter>();
            CaptureTime = new GXCaptureTime();
        }

        /// <summary>
        /// Received signal strength in dBm.
        /// </summary>
        [XmlIgnore()]
        public byte ReceivedSignalStrength
        {
            get;
            set;
        }

        /// <summary>
        /// Currently used channel ID.
        /// </summary>
        [XmlIgnore()]
        public byte ChannelId
        {
            get;
            set;
        }

        /// <summary>
        /// Link status.
        /// </summary>
        [XmlIgnore()]
        public MBusLinkStatus LinkStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Broadcast frame counters.
        /// </summary>
        [XmlIgnore()]
        public List<GXBroadcastFrameCounter> BroadcastFrames
        {
            get;
            set;
        }

        /// <summary>
        /// Transmitted frames.
        /// </summary>
        [XmlIgnore()]
        public UInt32 Transmissions
        {
            get;
            set;
        }

        /// <summary>
        /// Received frames with a correct checksum.
        /// </summary>
        [XmlIgnore()]
        public UInt32 ReceivedFrames
        {
            get;
            set;
        }

        /// <summary>
        /// Received frames with a incorrect checksum.
        /// </summary>
        [XmlIgnore()]
        public UInt32 FailedReceivedFrames
        {
            get;
            set;
        }

        /// <summary>
        /// Last time when ReceivedSignalStrength, LinkStatus, Transmissions, ReceivedFrames or FailedReceivedFrames was changed.
        /// </summary>
        /// <seealso cref="ReceivedSignalStrength"/>
        /// <seealso cref="LinkStatus"/>
        /// <seealso cref="Transmissions"/>
        /// <seealso cref="ReceivedFrames"/>
        /// <seealso cref="FailedReceivedFrames"/>
        [XmlIgnore()]
        public GXCaptureTime CaptureTime
        {
            get;
            set;
        }

        /// <summary>
        /// Reset value.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Reset(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ReceivedSignalStrength ,ChannelId
                ,LinkStatus ,BroadcastFrames,Transmissions ,ReceivedFrames
                ,FailedReceivedFrames,CaptureTime };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                ReceivedSignalStrength = 0;
                Transmissions = 0;
                ReceivedFrames = 0;
                FailedReceivedFrames = 0;
                CaptureTime = null;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            return null;
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //ReceivedSignalStrength
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //ChannelId
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //LinkStatus 
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //BroadcastFrames
            if (all || !base.IsRead(5))
            {
                attributes.Add(5);
            }
            //Transmissions
            if (all || !base.IsRead(6))
            {
                attributes.Add(6);
            }
            //ReceivedFrames
            if (all || !base.IsRead(7))
            {
                attributes.Add(7);
            }
            //FailedReceivedFrames
            if (all || !base.IsRead(8))
            {
                attributes.Add(8);
            }
            //CaptureTime
            if (all || !base.IsRead(9))
            {
                attributes.Add(9);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { GXCommon.GetLogicalNameString(), "Received signal strength ","Channel Id"
                ,"Link status" ,"Broadcast frames","Transmissions" ,"Received frames"
                ,"Failed received frames","Capture time"};
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Reset" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 9;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.UInt8;
                case 3:
                    return DataType.UInt8;
                case 4:
                    return DataType.Enum;
                case 5:
                    return DataType.Array;
                case 6:
                    return DataType.UInt32;
                case 7:
                    return DataType.UInt32;
                case 8:
                    return DataType.UInt32;
                case 9:
                    return DataType.Structure;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return ReceivedSignalStrength;
                case 3:
                    return ChannelId;
                case 4:
                    return LinkStatus;
                case 5:
                    {
                        int cnt = BroadcastFrames.Count;
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        //Add count
                        GXCommon.SetObjectCount(cnt, data);
                        if (cnt != 0)
                        {
                            foreach (var it in BroadcastFrames)
                            {
                                data.SetUInt8((byte)DataType.Structure);
                                data.SetUInt8((byte)3); //Count
                                GXCommon.SetData(settings, data, DataType.UInt8, it.ClientId);
                                GXCommon.SetData(settings, data, DataType.UInt32, it.Counter);
                                GXCommon.SetData(settings, data, DataType.DateTime, it.TimeStamp);
                            }
                        }
                        return data.Array();
                    }
                case 6:
                    return Transmissions;
                case 7:
                    return ReceivedFrames;
                case 8:
                    return FailedReceivedFrames;
                case 9:
                    {
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Structure);
                        //Add count
                        GXCommon.SetObjectCount(2, data);
                        GXCommon.SetData(settings, data, DataType.UInt8, CaptureTime.AttributeId);
                        GXCommon.SetData(settings, data, DataType.DateTime, CaptureTime.TimeStamp);
                        return data.Array();
                    }
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    ReceivedSignalStrength = Convert.ToByte(e.Value);
                    break;
                case 3:
                    ChannelId = Convert.ToByte(e.Value);
                    break;
                case 4:
                    LinkStatus = (MBusLinkStatus)Convert.ToByte(e.Value);
                    break;
                case 5:
                    BroadcastFrames.Clear();
                    if (e.Value != null)
                    {
                        List<object> item;
                        foreach (object tmp in (IEnumerable<object>)e.Value)
                        {
                            if (tmp is List<object>)
                            {
                                item = (List<object>)tmp;
                            }
                            else
                            {
                                item = new List<object>((object[])tmp);
                            }
                            //Time stamp should be date-time.
                            GXDateTime timeStamp;
                            if (item[2] is GXDateTime dt)
                            {
                                timeStamp = dt;
                            }
                            else if (item[2] is byte[] ba)
                            {
                                timeStamp = (GXDateTime)GXDLMSClient.ChangeType(ba, DataType.DateTime, settings.UseUtc2NormalTime);
                            }
                            else
                            {
                                timeStamp = null;
                            }
                            BroadcastFrames.Add(new GXBroadcastFrameCounter()
                            {
                                ClientId = (byte)item[0],
                                Counter = (UInt32)item[1],
                                TimeStamp = timeStamp
                            });
                        }
                    }
                    break;
                case 6:
                    Transmissions = Convert.ToUInt32(e.Value);
                    break;
                case 7:
                    ReceivedFrames = Convert.ToUInt32(e.Value);
                    break;
                case 8:
                    FailedReceivedFrames = Convert.ToUInt32(e.Value);
                    break;
                case 9:
                    if (e.Value != null)
                    {
                        List<object> item;
                        if (e.Value is List<object>)
                        {
                            item = (List<object>)e.Value;
                        }
                        else
                        {
                            item = new List<object>((object[])e.Value);
                        }
                        CaptureTime.AttributeId = (byte)item[0];
                        //TimeStamp should be date time.
                        if (item[1] is GXDateTime dt)
                        {
                            CaptureTime.TimeStamp = dt;
                        }
                        else if (item[1] is byte[] ba)
                        {
                            CaptureTime.TimeStamp = (GXDateTime)GXDLMSClient.ChangeType(ba, DataType.DateTime, settings.UseUtc2NormalTime);
                        }
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            string str;
            ReceivedSignalStrength = (byte)reader.ReadElementContentAsInt("ReceivedSignalStrength");
            ChannelId = (byte)reader.ReadElementContentAsInt("ChannelId");
            LinkStatus = (MBusLinkStatus)reader.ReadElementContentAsInt("LinkStatus");
            BroadcastFrames.Clear();
            if (reader.IsStartElement("BroadcastFrames", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    var item = new GXBroadcastFrameCounter()
                    {
                        ClientId = (byte)reader.ReadElementContentAsInt("ClientId"),
                        Counter = (byte)reader.ReadElementContentAsInt("Counter")
                    };
                    str = reader.ReadElementContentAsString("TimeStamp");
                    if (str == null)
                    {
                        item.TimeStamp = null;
                    }
                    else
                    {
                        item.TimeStamp = new GXDateTime(str, CultureInfo.InvariantCulture);
                    }
                    BroadcastFrames.Add(item);
                }
                reader.ReadEndElement("BroadcastFrames");
            }
            Transmissions = (UInt32)reader.ReadElementContentAsInt("Transmissions");
            ReceivedFrames = (UInt32)reader.ReadElementContentAsInt("ReceivedFrames");
            FailedReceivedFrames = (UInt32)reader.ReadElementContentAsInt("FailedReceivedFrames");
            if (reader.IsStartElement("CaptureTime", true))
            {
                CaptureTime.AttributeId = (byte)reader.ReadElementContentAsInt("AttributeId");
                str = reader.ReadElementContentAsString("TimeStamp");
                if (str == null)
                {
                    CaptureTime.TimeStamp = null;
                }
                else
                {
                    CaptureTime.TimeStamp = new GXDateTime(str, CultureInfo.InvariantCulture);
                }
                reader.ReadEndElement("CaptureTime");
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("ReceivedSignalStrength", ReceivedSignalStrength, 2);
            writer.WriteElementString("ChannelId", ChannelId, 3);
            writer.WriteElementString("LinkStatus", (byte)LinkStatus, 4);
            writer.WriteStartElement("BroadcastFrames", 5);
            if (BroadcastFrames != null)
            {
                foreach (var it in BroadcastFrames)
                {
                    writer.WriteStartElement("Item", 5);
                    //Some meters are returning time here, not date-time.
                    writer.WriteElementString("ClientId", it.ClientId, 5);
                    writer.WriteElementString("Counter", it.Counter, 5);
                    writer.WriteElementString("TimeStamp", it.TimeStamp, 5);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteElementString("Transmissions", Transmissions, 6);
            writer.WriteElementString("ReceivedFrames", ReceivedFrames, 7);
            writer.WriteElementString("FailedReceivedFrames", FailedReceivedFrames, 8);
            writer.WriteStartElement("CaptureTime", 9);
            writer.WriteElementString("AttributeId", CaptureTime.AttributeId, 9);
            writer.WriteElementString("TimeStamp", CaptureTime.TimeStamp, 9);
            writer.WriteEndElement();
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
