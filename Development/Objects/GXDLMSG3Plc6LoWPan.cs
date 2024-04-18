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
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// G3-PLC 6LoWPAN adaptation layer setup.
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSG3Plc6LoWPan
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
            Version = 3;
            BlacklistTable = new List<GXKeyValuePair<UInt16, UInt16>>();
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
        public byte[] PrefixTable
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
        /// Routing table.
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

        /// <summary>
        /// If true, the default route will be created.
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x24
        /// </remarks>
        [XmlIgnore()]
        public bool DefaultCoordRouteEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// List of the addresses of the devices for which this LOADng 
        /// router is providing connectivity.
        /// </summary>
        ///  <remarks>
        /// PIB attribute 0x23.
        /// </remarks>
        [XmlIgnore()]
        public UInt16[] DestinationAddress
        {
            get;
            set;
        }

        /// <summary>
        /// PIB attribute 0x04.
        /// </summary>
        [XmlIgnore()]
        public byte LowLQI
        {
            get;
            set;
        }

        /// <summary>
        /// PIB attribute 0x05.
        /// </summary>
        [XmlIgnore()]
        public byte HighLQI
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, MaxHops, WeakLqiValue , SecurityLevel, PrefixTable , RoutingConfiguration , BroadcastLogTableEntryTtl ,
            RoutingTable,ContextInformationTable, BlacklistTable, BroadcastLogTable, GroupTable,MaxJoinWaitTime,  PathDiscoveryTime,
            ActiveKeyIndex, MetricType,CoordShortAddress, DisableDefaultRouting, DeviceType,
            DefaultCoordRouteEnabled, DestinationAddress, LowLQI, HighLQI };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
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
            //MaxHops
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //WeakLqiValue
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //SecurityLevel
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //PrefixTable
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //RoutingConfiguration
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //BroadcastLogTableEntryTtl
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //RoutingTable
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //ContextInformationTable
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //BlacklistTable
            if (all || CanRead(10))
            {
                attributes.Add(10);
            }
            //BroadcastLogTable
            if (all || CanRead(11))
            {
                attributes.Add(11);
            }
            //GroupTable
            if (all || CanRead(12))
            {
                attributes.Add(12);
            }
            //MaxJoinWaitTime
            if (all || CanRead(13))
            {
                attributes.Add(13);
            }
            //PathDiscoveryTime
            if (all || CanRead(14))
            {
                attributes.Add(14);
            }
            //ActiveKeyIndex
            if (all || CanRead(15))
            {
                attributes.Add(15);
            }
            //MetricType
            if (all || CanRead(16))
            {
                attributes.Add(16);
            }
            if (Version > 0)
            {
                //CoordShortAddress
                if (all || CanRead(17))
                {
                    attributes.Add(17);
                }
                //DisableDefaultRouting
                if (all || CanRead(18))
                {
                    attributes.Add(18);
                }
                //DeviceType
                if (all || CanRead(19))
                {
                    attributes.Add(19);
                }
                if (Version > 1)
                {
                    //DefaultCoordRouteEnabled
                    if (all || CanRead(20))
                    {
                        attributes.Add(20);
                    }
                    //DestinationAddress
                    if (all || CanRead(21))
                    {
                        attributes.Add(21);
                    }
                    if (Version > 2)
                    {
                        //LowLQI
                        if (all || CanRead(22))
                        {
                            attributes.Add(22);
                        }
                        //HighLQI
                        if (all || CanRead(23))
                        {
                            attributes.Add(23);
                        }
                    }
                }
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "MaxHops", "WeakLqiValue",
                "SecurityLevel", "PrefixTable", "RoutingConfiguration", "BroadcastLogTableEntryTtl",
                "RoutingTable", "ContextInformationTable", "BlacklistTable", "BroadcastLogTable",
                "GroupTable", "MaxJoinWaitTime", " PathDiscoveryTime", "ActiveKeyIndex",
                "MetricType", "CoordShortAddress", "DisableDefaultRouting", "DeviceType",
                "Default coord route enabled", "Destination address",
                "Low LQI", "High LQI"};
        }
        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[0];
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 3;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            if (Version == 0)
            {
                return 16;
            }
            if (Version == 1)
            {
                return 19;
            }
            if (Version == 2)
            {
                return 21;
            }
            //Version 3.
            return 23;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        /// <inheritdoc />
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
            if (Version > 0)
            {
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
                if (Version > 1)
                {
                    //DefaultCoordRouteEnabled
                    if (index == 20)
                    {
                        return DataType.Boolean;
                    }
                    //DestinationAddress
                    if (index == 21)
                    {
                        return DataType.Array;
                    }
                    //LowLQI
                    if (index == 22)
                    {
                        return DataType.UInt8;
                    }
                    //HighLQI
                    if (index == 23)
                    {
                        return DataType.UInt8;
                    }

                }
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
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
                        GXCommon.SetData(settings, bb, DataType.UInt8, it);
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
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.RreqReqWait);
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
            if (e.Index == 20)
            {
                return DefaultCoordRouteEnabled;
            }
            if (e.Index == 21)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8((byte)DataType.Array);
                if (DestinationAddress == null)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(DestinationAddress.Length, bb);
                    foreach (UInt16 it in DestinationAddress)
                    {
                        GXCommon.SetData(settings, bb, DataType.UInt16, it);
                    }
                }
                return bb.Array();
            }
            if (e.Index == 22)
            {
                return LowLQI;
            }
            if (e.Index == 23)
            {
                return HighLQI;
            }
            e.Error = ErrorCode.ReadWriteDenied;
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
                    MaxHops = Convert.ToByte(e.Value);
                    break;
                case 3:
                    WeakLqiValue = Convert.ToByte(e.Value);
                    break;
                case 4:
                    SecurityLevel = Convert.ToByte(e.Value);
                    break;
                case 5:
                    {
                        List<byte> list = new List<byte>();
                        if (e.Value != null)
                        {
                            foreach (var it in (IEnumerable<object>)e.Value)
                            {
                                list.Add(Convert.ToByte(it));
                            }
                        }
                        PrefixTable = list.ToArray();
                        break;
                    }

                case 6:
                    {
                        RoutingConfiguration.Clear();
                        if (e.Value != null)
                        {
                            foreach (object tmp in (IEnumerable<object>)e.Value)
                            {
                                List<object> arr;
                                if (tmp is List<object>)
                                {
                                    arr = (List<object>)tmp;
                                }
                                else
                                {
                                    arr = new List<object>((object[])tmp);
                                }
                                GXDLMSRoutingConfiguration it = new GXDLMSRoutingConfiguration();
                                it.NetTraversalTime = Convert.ToByte(arr[0]);
                                it.RoutingTableEntryTtl = Convert.ToUInt16(arr[1]);
                                it.Kr = Convert.ToByte(arr[2]);
                                it.Km = Convert.ToByte(arr[3]);
                                it.Kc = Convert.ToByte(arr[4]);
                                it.Kq = Convert.ToByte(arr[5]);
                                it.Kh = Convert.ToByte(arr[6]);
                                it.Krt = Convert.ToByte(arr[7]);
                                it.RreqRetries = Convert.ToByte(arr[8]);
                                it.RreqReqWait = Convert.ToByte(arr[9]);
                                it.BlacklistTableEntryTtl = Convert.ToUInt16(arr[10]);
                                it.UnicastRreqGenEnable = Convert.ToBoolean(arr[11]);
                                it.RlcEnabled = Convert.ToBoolean(arr[12]);
                                it.AddRevLinkCost = Convert.ToByte(arr[13]);
                                RoutingConfiguration.Add(it);
                            }
                        }

                        break;
                    }

                case 7:
                    BroadcastLogTableEntryTtl = Convert.ToUInt16(e.Value);
                    break;
                case 8:
                    {
                        RoutingTable.Clear();
                        if (e.Value != null)
                        {
                            foreach (object tmp in (IEnumerable<object>)e.Value)
                            {
                                List<object> arr;
                                if (tmp is List<object>)
                                {
                                    arr = (List<object>)tmp;
                                }
                                else
                                {
                                    arr = new List<object>((object[])tmp);
                                }
                                GXDLMSRoutingTable it = new GXDLMSRoutingTable();
                                it.DestinationAddress = Convert.ToUInt16(arr[0]);
                                it.NextHopAddress = Convert.ToUInt16(arr[1]);
                                it.RouteCost = Convert.ToUInt16(arr[2]);
                                it.HopCount = Convert.ToByte(arr[3]);
                                it.WeakLinkCount = Convert.ToByte(arr[4]);
                                it.ValidTime = Convert.ToUInt16(arr[5]);
                                RoutingTable.Add(it);
                            }
                        }

                        break;
                    }

                case 9:
                    {
                        ContextInformationTable.Clear();
                        if (e.Value != null)
                        {
                            foreach (object tmp in (IEnumerable<object>)e.Value)
                            {
                                List<object> arr;
                                if (tmp is List<object>)
                                {
                                    arr = (List<object>)tmp;
                                }
                                else
                                {
                                    arr = new List<object>((object[])tmp);
                                }
                                GXDLMSContextInformationTable it = new GXDLMSContextInformationTable();
                                it.CID = Convert.ToString(arr[0]);
                                it.Context = (byte[])arr[2];
                                it.Compression = Convert.ToBoolean(arr[3]);
                                it.ValidLifetime = Convert.ToUInt16(arr[4]);
                                ContextInformationTable.Add(it);
                            }
                        }

                        break;
                    }

                case 10:
                    {
                        List<GXKeyValuePair<ushort, ushort>> list = new List<GXKeyValuePair<ushort, ushort>>();
                        if (e.Value != null)
                        {
                            foreach (object tmp in (IEnumerable<object>)e.Value)
                            {
                                List<object> arr;
                                if (tmp is List<object>)
                                {
                                    arr = (List<object>)tmp;
                                }
                                else
                                {
                                    arr = new List<object>((object[])tmp);
                                }
                                list.Add(new GXKeyValuePair<ushort, ushort>(Convert.ToUInt16(arr[0]), Convert.ToUInt16(arr[1])));
                            }
                        }
                        BlacklistTable = list;
                        break;
                    }

                case 11:
                    {
                        BroadcastLogTable.Clear();
                        if (e.Value != null)
                        {
                            foreach (object tmp in (IEnumerable<object>)e.Value)
                            {
                                List<object> arr;
                                if (tmp is List<object>)
                                {
                                    arr = (List<object>)tmp;
                                }
                                else
                                {
                                    arr = new List<object>((object[])tmp);
                                }
                                GXDLMSBroadcastLogTable it = new GXDLMSBroadcastLogTable();
                                it.SourceAddress = Convert.ToUInt16(arr[0]);
                                it.SequenceNumber = Convert.ToByte(arr[1]);
                                it.ValidTime = Convert.ToUInt16(arr[2]);
                                BroadcastLogTable.Add(it);
                            }
                        }

                        break;
                    }

                case 12:
                    {
                        List<ushort> list = new List<ushort>();
                        if (e.Value != null)
                        {
                            foreach (object it in (IEnumerable<object>)e.Value)
                            {
                                list.Add(Convert.ToUInt16(it));
                            }
                        }
                        GroupTable = list.ToArray();
                        break;
                    }

                case 13:
                    MaxJoinWaitTime = Convert.ToUInt16(e.Value);
                    break;
                case 14:
                    PathDiscoveryTime = Convert.ToByte(e.Value);
                    break;
                case 15:
                    ActiveKeyIndex = Convert.ToByte(e.Value);
                    break;
                case 16:
                    MetricType = Convert.ToByte(e.Value);
                    break;
                case 17:
                    CoordShortAddress = Convert.ToUInt16(e.Value);
                    break;
                case 18:
                    DisableDefaultRouting = (bool)e.Value;
                    break;
                case 19:
                    DeviceType = (DeviceType)Convert.ToInt32(e.Value);
                    break;
                case 20:
                    DefaultCoordRouteEnabled = Convert.ToBoolean(e.Value);
                    break;
                case 21:
                    {
                        List<ushort> list = new List<ushort>();
                        if (e.Value != null)
                        {
                            foreach (object it in (IEnumerable<object>)e.Value)
                            {
                                list.Add(Convert.ToUInt16(it));
                            }
                        }
                        DestinationAddress = list.ToArray();
                        break;
                    }

                case 22:
                    LowLQI = Convert.ToByte(e.Value);
                    break;
                case 23:
                    HighLQI = Convert.ToByte(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        private void LoadPrefixTable(GXXmlReader reader)
        {
            List<byte> list = new List<byte>();
            if (reader.IsStartElement("PrefixTable", true))
            {
                while (reader.IsStartElement("Value", false))
                {
                    list.Add((byte)reader.ReadElementContentAsInt("Value", 0));
                }
                reader.ReadEndElement("PrefixTable");
            }
            PrefixTable = list.ToArray();
        }

        private void LoadRoutingConfiguration(GXXmlReader reader)
        {
            RoutingConfiguration.Clear();
            if (reader.IsStartElement("RoutingConfiguration", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSRoutingConfiguration it = new GXDLMSRoutingConfiguration();
                    RoutingConfiguration.Add(it);
                    it.NetTraversalTime = (byte)reader.ReadElementContentAsInt("NetTraversalTime");
                    it.RoutingTableEntryTtl = (UInt16)reader.ReadElementContentAsInt("RoutingTableEntryTtl");
                    it.Kr = (byte)reader.ReadElementContentAsInt("Kr");
                    it.Km = (byte)reader.ReadElementContentAsInt("Km");
                    it.Kc = (byte)reader.ReadElementContentAsInt("Kc");
                    it.Kq = (byte)reader.ReadElementContentAsInt("Kq");
                    it.Kh = (byte)reader.ReadElementContentAsInt("Kh");
                    it.Krt = (byte)reader.ReadElementContentAsInt("Krt");
                    it.RreqRetries = (byte)reader.ReadElementContentAsInt("RreqRetries");
                    it.RreqReqWait = (byte)reader.ReadElementContentAsInt("RreqReqWait");
                    it.BlacklistTableEntryTtl = (UInt16)reader.ReadElementContentAsInt("BlacklistTableEntryTtl");
                    it.UnicastRreqGenEnable = reader.ReadElementContentAsInt("UnicastRreqGenEnable") != 0;
                    it.RlcEnabled = reader.ReadElementContentAsInt("RlcEnabled") != 0;
                    it.AddRevLinkCost = (byte)reader.ReadElementContentAsInt("AddRevLinkCost");
                }
                reader.ReadEndElement("RoutingConfiguration");
            }
        }

        private void LoadRoutingTable(GXXmlReader reader)
        {
            RoutingTable.Clear();
            if (reader.IsStartElement("RoutingTable", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSRoutingTable it = new GXDLMSRoutingTable();
                    RoutingTable.Add(it);
                    it.DestinationAddress = (UInt16)reader.ReadElementContentAsInt("DestinationAddress");
                    it.NextHopAddress = (UInt16)reader.ReadElementContentAsInt("NextHopAddress");
                    it.RouteCost = (UInt16)reader.ReadElementContentAsInt("RouteCost");
                    it.HopCount = (byte)reader.ReadElementContentAsInt("HopCount");
                    it.WeakLinkCount = (byte)reader.ReadElementContentAsInt("WeakLinkCount");
                    it.ValidTime = (UInt16)reader.ReadElementContentAsInt("ValidTime");
                }
                reader.ReadEndElement("RoutingTable");
            }
        }

        private void LoadContextInformationTable(GXXmlReader reader)
        {
            ContextInformationTable.Clear();
            if (reader.IsStartElement("ContextInformationTable", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSContextInformationTable it = new GXDLMSContextInformationTable();
                    ContextInformationTable.Add(it);
                    it.CID = reader.ReadElementContentAsString("CID");
                    it.Context = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("Context"));
                    it.Compression = reader.ReadElementContentAsInt("Compression") != 0;
                    it.ValidLifetime = (UInt16)reader.ReadElementContentAsInt("ValidLifetime");
                }
                reader.ReadEndElement("ContextInformationTable");
            }
        }

        private void LoadBlacklistTable(GXXmlReader reader)
        {
            BlacklistTable.Clear();
            if (reader.IsStartElement("BlacklistTable", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    UInt16 k = (UInt16)reader.ReadElementContentAsInt("Key");
                    UInt16 v = (UInt16)reader.ReadElementContentAsInt("Value");
                    BlacklistTable.Add(new GXKeyValuePair<UInt16, UInt16>(k, v));
                }
                reader.ReadEndElement("BlacklistTable");
            }
        }

        private void LoadBroadcastLogTable(GXXmlReader reader)
        {
            BroadcastLogTable.Clear();
            if (reader.IsStartElement("BroadcastLogTable", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSBroadcastLogTable it = new GXDLMSBroadcastLogTable();
                    BroadcastLogTable.Add(it);
                    it.SourceAddress = (UInt16)reader.ReadElementContentAsInt("SourceAddress");
                    it.SequenceNumber = (byte)reader.ReadElementContentAsInt("SequenceNumber");
                    it.ValidTime = (UInt16)reader.ReadElementContentAsInt("ValidTime");
                }
                reader.ReadEndElement("BroadcastLogTable");
            }
        }

        private void LoadGroupTable(GXXmlReader reader)
        {
            List<UInt16> list = new List<UInt16>();
            if (reader.IsStartElement("GroupTable", true))
            {
                while (reader.IsStartElement("Value", false))
                {
                    list.Add((UInt16)reader.ReadElementContentAsInt("Value"));
                }
                reader.ReadEndElement("GroupTable");
            }
            GroupTable = list.ToArray();
        }

        private void LoadDestinationAddress(GXXmlReader reader)
        {
            List<UInt16> list = new List<UInt16>();
            if (reader.IsStartElement("DestinationAddress", true))
            {
                while (reader.IsStartElement("Value", false))
                {
                    list.Add((UInt16)reader.ReadElementContentAsInt("Value"));
                }
                reader.ReadEndElement("DestinationAddress");
            }
            DestinationAddress = list.ToArray();
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            MaxHops = (byte)reader.ReadElementContentAsInt("MaxHops");
            WeakLqiValue = (byte)reader.ReadElementContentAsInt("WeakLqiValue");
            SecurityLevel = (byte)reader.ReadElementContentAsInt("SecurityLevel");
            LoadPrefixTable(reader);
            LoadRoutingConfiguration(reader);
            BroadcastLogTableEntryTtl = (UInt16)reader.ReadElementContentAsInt("BroadcastLogTableEntryTtl");
            LoadRoutingTable(reader);
            LoadContextInformationTable(reader);
            LoadBlacklistTable(reader);
            LoadBroadcastLogTable(reader);
            LoadGroupTable(reader);
            MaxJoinWaitTime = (UInt16)reader.ReadElementContentAsInt("MaxJoinWaitTime");
            PathDiscoveryTime = (byte)reader.ReadElementContentAsInt("PathDiscoveryTime");
            ActiveKeyIndex = (byte)reader.ReadElementContentAsInt("ActiveKeyIndex");
            MetricType = (byte)reader.ReadElementContentAsInt("MetricType");
            CoordShortAddress = (UInt16)reader.ReadElementContentAsInt("CoordShortAddress");
            DisableDefaultRouting = reader.ReadElementContentAsInt("DisableDefaultRouting") != 0;
            DeviceType = (DeviceType)reader.ReadElementContentAsInt("DeviceType");
            DefaultCoordRouteEnabled = reader.ReadElementContentAsInt("DefaultCoordRouteEnabled") != 0;
            LoadDestinationAddress(reader);
            LowLQI = (byte)reader.ReadElementContentAsInt("LowLQI");
            HighLQI = (byte)reader.ReadElementContentAsInt("HighLQI");
        }

        private void SavePrefixTable(GXXmlWriter writer, int index)
        {
            if (PrefixTable != null)
            {
                writer.WriteStartElement("PrefixTable", index);
                foreach (object it in PrefixTable)
                {
                    writer.WriteElementObject("Value", it, index);
                }
                writer.WriteEndElement();
            }
        }

        private void SaveRoutingConfiguration(GXXmlWriter writer, int index)
        {
            if (RoutingConfiguration != null)
            {
                writer.WriteStartElement("RoutingConfiguration", index);
                foreach (GXDLMSRoutingConfiguration it in RoutingConfiguration)
                {
                    writer.WriteStartElement("Item", index);
                    writer.WriteElementString("NetTraversalTime", it.NetTraversalTime, index);
                    writer.WriteElementString("RoutingTableEntryTtl", it.RoutingTableEntryTtl, index);
                    writer.WriteElementString("Kr", it.Kr, index);
                    writer.WriteElementString("Km", it.Km, index);
                    writer.WriteElementString("Kc", it.Kc, index);
                    writer.WriteElementString("Kq", it.Kq, index);
                    writer.WriteElementString("Kh", it.Kh, index);
                    writer.WriteElementString("Krt", it.Krt, index);
                    writer.WriteElementString("RreqRetries", it.RreqRetries, index);
                    writer.WriteElementString("RreqReqWait", it.RreqReqWait, index);
                    writer.WriteElementString("BlacklistTableEntryTtl", it.BlacklistTableEntryTtl, index);
                    writer.WriteElementString("UnicastRreqGenEnable", it.UnicastRreqGenEnable, index);
                    writer.WriteElementString("RlcEnabled", it.RlcEnabled, index);
                    writer.WriteElementString("AddRevLinkCost", it.AddRevLinkCost, index);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void SaveRoutingTable(GXXmlWriter writer, int index)
        {
            if (RoutingTable != null)
            {
                writer.WriteStartElement("RoutingTable", index);
                foreach (GXDLMSRoutingTable it in RoutingTable)
                {
                    writer.WriteStartElement("Item", index);
                    writer.WriteElementString("DestinationAddress", it.DestinationAddress, index);
                    writer.WriteElementString("NextHopAddress", it.NextHopAddress, index);
                    writer.WriteElementString("RouteCost", it.RouteCost, index);
                    writer.WriteElementString("HopCount", it.HopCount, index);
                    writer.WriteElementString("WeakLinkCount", it.WeakLinkCount, index);
                    writer.WriteElementString("ValidTime", it.ValidTime, index);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void SaveContextInformationTable(GXXmlWriter writer, int index)
        {
            if (ContextInformationTable != null)
            {
                writer.WriteStartElement("ContextInformationTable", index);
                foreach (GXDLMSContextInformationTable it in ContextInformationTable)
                {
                    writer.WriteStartElement("Item", index);
                    writer.WriteElementString("CID", it.CID, index);
                    writer.WriteElementString("Context", GXDLMSTranslator.ToHex(it.Context), index);
                    writer.WriteElementString("Compression", it.Compression, index);
                    writer.WriteElementString("ValidLifetime", it.ValidLifetime, index);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void SaveBlacklistTable(GXXmlWriter writer, int index)
        {
            if (BlacklistTable != null)
            {
                writer.WriteStartElement("BlacklistTable", index);
                foreach (GXKeyValuePair<UInt16, UInt16> it in BlacklistTable)
                {
                    writer.WriteStartElement("Item", index);
                    writer.WriteElementObject("Key", it.Key, index);
                    writer.WriteElementObject("Value", it.Value, index);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void SaveBroadcastLogTable(GXXmlWriter writer, int index)
        {
            if (BroadcastLogTable != null)
            {
                writer.WriteStartElement("BroadcastLogTable", index);
                foreach (GXDLMSBroadcastLogTable it in BroadcastLogTable)
                {
                    writer.WriteStartElement("Item", index);
                    writer.WriteElementObject("SourceAddress", it.SourceAddress, index);
                    writer.WriteElementObject("SequenceNumber", it.SequenceNumber, index);
                    writer.WriteElementObject("ValidTime", it.ValidTime, index);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void SaveGroupTable(GXXmlWriter writer, int index)
        {
            if (GroupTable != null)
            {
                writer.WriteStartElement("GroupTable", index);
                foreach (UInt16 it in GroupTable)
                {
                    writer.WriteElementObject("Value", it, index);
                }
                writer.WriteEndElement();
            }
        }

        private void SaveDestinationAddress(GXXmlWriter writer, int index)
        {
            if (DestinationAddress != null)
            {
                writer.WriteStartElement("DestinationAddress", index);
                foreach (UInt16 it in DestinationAddress)
                {
                    writer.WriteElementObject("Value", it, index);
                }
                writer.WriteEndElement();
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("MaxHops", MaxHops, 2);
            writer.WriteElementString("WeakLqiValue", WeakLqiValue, 3);
            writer.WriteElementString("SecurityLevel", SecurityLevel, 4);
            SavePrefixTable(writer, 5);
            SaveRoutingConfiguration(writer, 6);
            writer.WriteElementString("BroadcastLogTableEntryTtl", BroadcastLogTableEntryTtl, 7);
            SaveRoutingTable(writer, 8);
            SaveContextInformationTable(writer, 9);
            SaveBlacklistTable(writer, 10);
            SaveBroadcastLogTable(writer, 11);
            SaveGroupTable(writer, 12);
            writer.WriteElementString("MaxJoinWaitTime", MaxJoinWaitTime, 13);
            writer.WriteElementString("PathDiscoveryTime", PathDiscoveryTime, 14);
            writer.WriteElementString("ActiveKeyIndex", ActiveKeyIndex, 15);
            writer.WriteElementString("MetricType", MetricType, 16);
            writer.WriteElementString("CoordShortAddress", CoordShortAddress, 17);
            writer.WriteElementString("DisableDefaultRouting", DisableDefaultRouting, 18);
            writer.WriteElementString("DeviceType", (int)DeviceType, 19);
            writer.WriteElementString("DefaultCoordRouteEnabled", DefaultCoordRouteEnabled, 20);
            SaveDestinationAddress(writer, 21);
            writer.WriteElementString("LowLQI", LowLQI, 22);
            writer.WriteElementString("HighLQI", HighLQI, 23);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
