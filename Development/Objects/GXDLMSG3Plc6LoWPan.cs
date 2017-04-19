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
using Gurux.DLMS;
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// G3-PLC 6LoWPAN adaptation layer setup.
    /// </summary>
    public class GXDLMSG3Plc6LoWPan : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSG3Plc6LoWPan()
        : this("0.0.29.2.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSG3Plc6LoWPan(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSG3Plc6LoWPan(string ln, ushort sn)
        : base(ObjectType.G3Plc6LoWPan, ln, sn)
        {
            ContextInformationTable = new List<Objects.GXDLMSContextInformationTable>();
            RoutingConfiguration = new List<GXDLMSRoutingConfiguration>();
            RoutingTable = new List<GXDLMSRoutingTable>();
            BroadcastLogTable = new List<Objects.GXDLMSBroadcastLogTable>();
            MaxHops = 8;
            WeakLqiValue = 52;
            SecurityLevel = 5;
            BroadcastLogTableEntryTtl = 2;
            MaxJoinWaitTime = 20;
            PathDiscoveryTime = 40;
            ActiveKeyIndex = 0;
            MetricType = 0xF;
            CoordShortAddress = 0;
            DisableDefaultRouting = false;
            DeviceType = DeviceType.NotDefined;
        }

        /// <summary>
        /// Defines the maximum number of hops to be used by the routing algorithm. 
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x0F.
        /// </remarks>
        [XmlIgnore()]
        public byte MaxHops
        {
            get;
            set;
        }

        /// <summary>
        /// The weak link value defines the LQI value below which a link to a neighbour is considered as a weak link. 
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x1A.
        /// </remarks>
        [XmlIgnore()]
        public byte WeakLqiValue
        {
            get;
            set;
        }

        /// <summary>
        /// The minimum security level to be used for incoming and outgoing adaptation frames.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x00.
        /// </remarks>
        [XmlIgnore()]
        public byte SecurityLevel
        {
            get;
            set;
        }

        /// <summary>
        ///  Contains the list of prefixes defined on this PAN
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x01.
        /// </remarks>
        [XmlIgnore()]
        public object[] PrefixTable
        {
            get;
            set;
        }

        /// <summary>
        /// The routing configuration element specifies all parameters linked to the routing mechanism described in ITU-T G.9903:2014.
        /// </summary>
        [XmlIgnore()]
        public List<GXDLMSRoutingConfiguration> RoutingConfiguration
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum time to live of an adpBroadcastLogTable entry (in minutes).
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x02.
        /// </remarks>
        [XmlIgnore()]
        public UInt16 BroadcastLogTableEntryTtl
        {
            get;
            set;
        }
        /// <summary>
        /// Routing table
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x03.
        /// </remarks>
        [XmlIgnore()]
        public List<GXDLMSRoutingTable> RoutingTable
        {
            get;
            set;
        }

        /// <summary>
        ///  Contains the context information associated to each CID extension field.  
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x07.
        /// </remarks>
        [XmlIgnore()]
        public List<GXDLMSContextInformationTable> ContextInformationTable
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the list of the blacklisted neighbours.Key is 16-bit address of the blacklisted neighbour. 
        /// Value is Remaining time in minutes until which this entry in the blacklisted neighbour table is considered valid.
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x1E.
        /// </remarks>
        [XmlIgnore()]
        public List<GXKeyValuePair<UInt16, UInt16>> BlacklistTable
        {
            get;
            set;
        }
        /// <summary>
        ///  Broadcast log table
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x0B.
        /// </remarks>
        [XmlIgnore()]
        public List<GXDLMSBroadcastLogTable> BroadcastLogTable
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the group addresses to which the device belongs. array 
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x0E.
        /// </remarks>
        [XmlIgnore()]
        public UInt16[] GroupTable
        {
            get;
            set;
        }
        /// <summary>
        ///  Network join timeout in seconds for LBD
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x20.
        /// </remarks>
        [XmlIgnore()]
        public UInt16 MaxJoinWaitTime
        {
            get;
            set;
        }
        /// <summary>
        /// Timeout for path discovery in seconds. 
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x21.
        /// </remarks>
        [XmlIgnore()]
        public byte PathDiscoveryTime
        {
            get;
            set;
        }
        /// <summary>
        /// Index of the active GMK to be used for data transmission.
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x22.
        /// </remarks>
        [XmlIgnore()]
        public byte ActiveKeyIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Metric Type to be used for routing purposes.
        /// </summary>  
        /// <remarks>
        /// PIB attribute 0x3.
        /// </remarks>
        [XmlIgnore()]
        public byte MetricType
        {
            get;
            set;
        }
        /// <summary>
        /// Defines the short address of the coordinator.
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x8.
        /// </remarks>
        [XmlIgnore()]
        public UInt16 CoordShortAddress
        {
            get;
            set;
        }
        /// <summary>
        /// Is default routing (LOADng) disabled. 
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0xF0.
        /// </remarks>
        [XmlIgnore()]
        public bool DisableDefaultRouting
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the type of the device connected to the modem
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x10.
        /// </remarks>
        [XmlIgnore()]
        public DeviceType DeviceType
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, MaxHops, WeakLqiValue , SecurityLevel, PrefixTable , RoutingConfiguration , BroadcastLogTableEntryTtl ,
            RoutingTable,ContextInformationTable, BlacklistTable, BroadcastLogTable, GroupTable,MaxJoinWaitTime,  PathDiscoveryTime,
            ActiveKeyIndex, MetricType,CoordShortAddress, DisableDefaultRouting, DeviceType };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //MaxHops
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //WeakLqiValue
            if (CanRead(3))
            {
                attributes.Add(3);
            }
            //SecurityLevel
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            //PrefixTable
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            //RoutingConfiguration
            if (CanRead(6))
            {
                attributes.Add(6);
            }
            //BroadcastLogTableEntryTtl
            if (CanRead(7))
            {
                attributes.Add(7);
            }
            //RoutingTable
            if (CanRead(8))
            {
                attributes.Add(8);
            }
            //ContextInformationTable
            if (CanRead(9))
            {
                attributes.Add(9);
            }
            //BlacklistTable
            if (CanRead(10))
            {
                attributes.Add(10);
            }
            //BroadcastLogTable
            if (CanRead(11))
            {
                attributes.Add(11);
            }
            //GroupTable
            if (CanRead(12))
            {
                attributes.Add(12);
            }
            //MaxJoinWaitTime
            if (CanRead(13))
            {
                attributes.Add(13);
            }
            //PathDiscoveryTime
            if (CanRead(14))
            {
                attributes.Add(14);
            }
            //ActiveKeyIndex
            if (CanRead(15))
            {
                attributes.Add(15);
            }
            //MetricType
            if (CanRead(16))
            {
                attributes.Add(16);
            }
            //CoordShortAddress
            if (CanRead(17))
            {
                attributes.Add(17);
            }
            //DisableDefaultRouting
            if (CanRead(18))
            {
                attributes.Add(18);
            }
            //DeviceType
            if (CanRead(19))
            {
                attributes.Add(19);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "MaxHops", "WeakLqiValue",
                "SecurityLevel", "PrefixTable", "RoutingConfiguration", "BroadcastLogTableEntryTtl",
                "RoutingTable", "ContextInformationTable", "BlacklistTable", "BroadcastLogTable",
                "GroupTable", "MaxJoinWaitTime", " PathDiscoveryTime", "ActiveKeyIndex",
                "MetricType", "CoordShortAddress", "DisableDefaultRouting", "DeviceType" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 19;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            //LN.
            if (index == 1)
            {
                return DataType.OctetString;
            }
            //MaxHops
            if (index == 2)
            {
                return DataType.UInt8;
            }
            //WeakLqiValue
            if (index == 3)
            {
                return DataType.UInt8;
            }
            //SecurityLevel
            if (index == 4)
            {
                return DataType.UInt8;
            }
            //PrefixTable
            if (index == 5)
            {
                return DataType.Array;
            }
            //RoutingConfiguration
            if (index == 6)
            {
                return DataType.Array;
            }
            //BroadcastLogTableEntryTtl
            if (index == 7)
            {
                return DataType.UInt16;
            }
            //RoutingTable
            if (index == 8)
            {
                return DataType.Array;
            }
            //ContextInformationTable
            if (index == 9)
            {
                return DataType.Array;
            }
            //BlacklistTable
            if (index == 10)
            {
                return DataType.Array;
            }
            //BroadcastLogTable
            if (index == 11)
            {
                return DataType.Array;
            }
            //GroupTable
            if (index == 12)
            {
                return DataType.Array;
            }
            //MaxJoinWaitTime
            if (index == 13)
            {
                return DataType.UInt16;
            }
            //PathDiscoveryTime
            if (index == 14)
            {
                return DataType.UInt8;
            }
            //ActiveKeyIndex
            if (index == 15)
            {
                return DataType.UInt8;
            }
            //MetricType
            if (index == 16)
            {
                return DataType.UInt8;
            }
            //CoordShortAddress
            if (index == 17)
            {
                return DataType.UInt16;
            }
            //DisableDefaultRouting
            if (index == 18)
            {
                return DataType.Boolean;
            }
            //DeviceType
            if (index == 19)
            {
                return DataType.Enum;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return this.LogicalName;
            }
            if (e.Index == 2)
            {
                return MaxHops;
            }
            if (e.Index == 3)
            {
                return WeakLqiValue;
            }
            if (e.Index == 4)
            {
                return SecurityLevel;
            }
            if (e.Index == 5)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8((byte)DataType.Array);
                if (PrefixTable == null)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(PrefixTable.Length, bb);
                    foreach (var it in PrefixTable)
                    {
                        GXCommon.SetData(settings, bb, GXCommon.GetValueType(it), it);
                    }
                }
                return bb.Array();
            }
            if (e.Index == 6)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8((byte)DataType.Array);
                if (RoutingConfiguration == null)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(RoutingConfiguration.Count, bb);
                    foreach (GXDLMSRoutingConfiguration it in RoutingConfiguration)
                    {
                        bb.SetUInt8((byte)DataType.Structure);
                        bb.SetUInt8(14);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.NetTraversalTime);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.RoutingTableEntryTtl);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.Kr);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.Km);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.Kc);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.Kq);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.Kh);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.Krt);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.RreqRetries);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.RreqRerrWait);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.BlacklistTableEntryTtl);
                        GXCommon.SetData(settings, bb, DataType.Boolean, it.UnicastRreqGenEnable);
                        GXCommon.SetData(settings, bb, DataType.Boolean, it.RlcEnabled);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.AddRevLinkCost);
                    }
                }
                return bb.Array();
            }
            if (e.Index == 7)
            {
                return BroadcastLogTableEntryTtl;
            }
            if (e.Index == 8)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8((byte)DataType.Array);
                if (RoutingTable == null)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(RoutingTable.Count, bb);
                    foreach (GXDLMSRoutingTable it in RoutingTable)
                    {
                        bb.SetUInt8((byte)DataType.Structure);
                        bb.SetUInt8(6);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.DestinationAddress);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.NextHopAddress);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.RouteCost);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.HopCount);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.WeakLinkCount);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.ValidTime);
                    }
                }
                return bb.Array();
            }
            if (e.Index == 9)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8((byte)DataType.Array);
                if (ContextInformationTable == null)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(ContextInformationTable.Count, bb);
                    foreach (GXDLMSContextInformationTable it in ContextInformationTable)
                    {
                        bb.SetUInt8((byte)DataType.Structure);
                        bb.SetUInt8(5);
                        GXCommon.SetData(settings, bb, DataType.BitString, it.CID);
                        if (it.Context == null)
                        {
                            GXCommon.SetData(settings, bb, DataType.UInt8, 0);
                        }
                        else
                        {
                            GXCommon.SetData(settings, bb, DataType.UInt8, it.Context.Length);
                        }
                        GXCommon.SetData(settings, bb, DataType.OctetString, it.Context);
                        GXCommon.SetData(settings, bb, DataType.Boolean, it.Compression);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.ValidLifetime);
                    }
                }
                return bb.Array();
            }
            if (e.Index == 10)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8((byte)DataType.Array);
                if (BlacklistTable == null)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(BlacklistTable.Count, bb);
                    foreach (var it in BlacklistTable)
                    {
                        bb.SetUInt8((byte)DataType.Structure);
                        bb.SetUInt8(2);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.Key);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.Value);
                    }
                }
                return bb.Array();
            }
            if (e.Index == 11)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8((byte)DataType.Array);
                if (BroadcastLogTable == null)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(BroadcastLogTable.Count, bb);
                    foreach (GXDLMSBroadcastLogTable it in BroadcastLogTable)
                    {
                        bb.SetUInt8((byte)DataType.Structure);
                        bb.SetUInt8(3);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.SourceAddress);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.SequenceNumber);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.ValidTime);
                    }
                }
                return bb.Array();
            }
            if (e.Index == 12)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8((byte)DataType.Array);
                if (GroupTable == null)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(GroupTable.Length, bb);
                    foreach (UInt16 it in GroupTable)
                    {
                        GXCommon.SetData(settings, bb, DataType.UInt16, it);
                    }
                }
                return bb.Array();
            }
            if (e.Index == 13)
            {
                return MaxJoinWaitTime;
            }
            if (e.Index == 14)
            {
                return PathDiscoveryTime;
            }
            if (e.Index == 15)
            {
                return ActiveKeyIndex;
            }
            if (e.Index == 16)
            {
                return MetricType;
            }
            if (e.Index == 17)
            {
                return CoordShortAddress;
            }
            if (e.Index == 18)
            {
                return DisableDefaultRouting;
            }
            if (e.Index == 19)
            {
                return DeviceType;
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                if (e.Value is string)
                {
                    LogicalName = e.Value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString, settings.UseUtc2NormalTime).ToString();
                }
            }
            else if (e.Index == 2)
            {
                MaxHops = Convert.ToByte(e.Value);
            }
            else if (e.Index == 3)
            {
                WeakLqiValue = Convert.ToByte(e.Value);
            }
            else if (e.Index == 4)
            {
                SecurityLevel = Convert.ToByte(e.Value);
            }
            else if (e.Index == 5)
            {
                List<object> list = new List<object>();
                if (e.Value != null)
                {
                    list.AddRange((object[])e.Value);
                }
                PrefixTable = list.ToArray();
            }
            else if (e.Index == 6)
            {
                RoutingConfiguration.Clear();
                if (e.Value != null)
                {
                    foreach (object v in (object[])e.Value)
                    {
                        object[] tmp = (object[])v;
                        GXDLMSRoutingConfiguration it = new GXDLMSRoutingConfiguration();
                        it.NetTraversalTime = Convert.ToByte(tmp[0]);
                        it.RoutingTableEntryTtl = Convert.ToUInt16(tmp[1]);
                        it.Kr = Convert.ToByte(tmp[2]);
                        it.Km = Convert.ToByte(tmp[3]);
                        it.Kc = Convert.ToByte(tmp[4]);
                        it.Kq = Convert.ToByte(tmp[5]);
                        it.Kh = Convert.ToByte(tmp[6]);
                        it.Krt = Convert.ToByte(tmp[7]);
                        it.RreqRetries = Convert.ToByte(tmp[8]);
                        it.RreqRerrWait = Convert.ToByte(tmp[9]);
                        it.BlacklistTableEntryTtl = Convert.ToUInt16(tmp[10]);
                        it.UnicastRreqGenEnable = Convert.ToBoolean(tmp[11]);
                        it.RlcEnabled = Convert.ToBoolean(tmp[12]);
                        it.AddRevLinkCost = Convert.ToByte(tmp[13]);
                        RoutingConfiguration.Add(it);
                    }
                }
            }
            else if (e.Index == 7)
            {
                BroadcastLogTableEntryTtl = Convert.ToUInt16(e.Value);
            }
            else if (e.Index == 8)
            {
                RoutingTable.Clear();
                if (e.Value != null)
                {
                    foreach (object v in (object[])e.Value)
                    {
                        object[] tmp = (object[])v;
                        GXDLMSRoutingTable it = new GXDLMSRoutingTable();
                        it.DestinationAddress = Convert.ToUInt16(tmp[10]);
                        it.NextHopAddress = Convert.ToUInt16(tmp[10]);
                        it.RouteCost = Convert.ToUInt16(tmp[10]);
                        it.HopCount = Convert.ToByte(tmp[10]);
                        it.WeakLinkCount = Convert.ToByte(tmp[10]);
                        it.ValidTime = Convert.ToUInt16(tmp[10]);
                        RoutingTable.Add(it);
                    }
                }
            }
            else if (e.Index == 9)
            {
                ContextInformationTable.Clear();
                if (e.Value != null)
                {
                    foreach (object v in (object[])e.Value)
                    {
                        object[] tmp = (object[])v;
                        GXDLMSContextInformationTable it = new GXDLMSContextInformationTable();
                        it.CID = (string)tmp[0];
                        it.Context = (byte[])tmp[2];
                        it.Compression = Convert.ToBoolean(tmp[3]);
                        it.ValidLifetime = Convert.ToUInt16(tmp[4]);
                        ContextInformationTable.Add(it);
                    }
                }
            }
            else if (e.Index == 10)
            {
                List<GXKeyValuePair<UInt16, UInt16>> list = new List<GXKeyValuePair<UInt16, UInt16>>();
                if (e.Value != null)
                {
                    foreach (object v in (object[])e.Value)
                    {
                        object[] tmp = (object[])v;
                        list.Add(new GXKeyValuePair<UInt16, UInt16>(Convert.ToUInt16(tmp[0]), Convert.ToUInt16(tmp[1])));
                    }
                }
                BlacklistTable = list;
            }
            else if (e.Index == 11)
            {
                BroadcastLogTable.Clear();
                if (e.Value != null)
                {
                    foreach (object v in (object[])e.Value)
                    {
                        object[] tmp = (object[])v;
                        GXDLMSBroadcastLogTable it = new GXDLMSBroadcastLogTable();
                        it.SourceAddress = Convert.ToUInt16(tmp[0]);
                        it.SequenceNumber = Convert.ToByte(tmp[1]);
                        it.ValidTime = Convert.ToUInt16(tmp[2]);
                        BroadcastLogTable.Add(it);
                    }
                }
            }
            else if (e.Index == 12)
            {
                List<UInt16> list = new List<UInt16>();
                if (e.Value != null)
                {
                    foreach (object it in (object[])e.Value)
                    {
                        list.Add(Convert.ToUInt16(it));
                    }
                }
                GroupTable = list.ToArray();
            }
            else if (e.Index == 13)
            {
                MaxJoinWaitTime = Convert.ToUInt16(e.Value);
            }
            else if (e.Index == 14)
            {
                PathDiscoveryTime = Convert.ToByte(e.Value);
            }
            else if (e.Index == 15)
            {
                ActiveKeyIndex = Convert.ToByte(e.Value);
            }
            else if (e.Index == 16)
            {
                MetricType = Convert.ToByte(e.Value);
            }
            else if (e.Index == 17)
            {
                CoordShortAddress = Convert.ToUInt16(e.Value);
            }
            else if (e.Index == 18)
            {
                DisableDefaultRouting = (bool)e.Value;
            }
            else if (e.Index == 19)
            {
                DeviceType = (DeviceType)Convert.ToInt32(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }
        #endregion
    }
}
