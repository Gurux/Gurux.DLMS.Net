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
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSMacPosTable
    {
        /// <summary>
        /// The 16-bit address the device is using to communicate through the PAN.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x53.
        /// </remarks>
        public UInt16 ShortAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Link Quality Indicator.
        /// </summary>
        public byte LQI
        {
            get;
            set;
        }

        /// <summary>
        /// Valid time,
        /// </summary>
        public UInt16 ValidTime
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSG3PlcMacSetup
    /// </summary>
    public class GXDLMSG3PlcMacSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSG3PlcMacSetup()
        : this("0.0.29.1.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSG3PlcMacSetup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSG3PlcMacSetup(string ln, ushort sn)
        : base(ObjectType.G3PlcMacSetup, ln, sn)
        {
            Version = 3;
            KeyTable = new List<DLMS.GXKeyValuePair<byte, byte[]>>();
            ShortAddress = 0xFFFF;
            RcCoord = 0xFFFF;
            PANId = 0xFFFF;
            FrameCounter = 0;
            //ToneMask
            TmrTtl = 2;
            MaxFrameRetries = 5;
            NeighbourTableEntryTtl = 255;
            HighPriorityWindowSize = 7;
            CscmFairnessLimit = 25;
            BeaconRandomizationWindowLength = 12;
            A = 8;
            K = 5;
            MinCwAttempts = 10;
            CenelecLegacyMode = 1;
            FccLegacyMode = 1;
            MaxBe = 8;
            MaxCsmaBackoffs = 50;
            MinBe = 3;
        }

        /// <summary>
        /// The 16-bit address the device is using to communicate through the PAN.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x53.
        /// </remarks>
        [XmlIgnore()]
        public UInt16 ShortAddress
        {
            get;
            set;
        }
        /// <summary>
        /// Route cost to coordinator.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x10F.
        /// </remarks>
        [XmlIgnore()]
        public UInt16 RcCoord
        {
            get;
            set;
        }

        /// <summary>
        /// The 16-bit identifier of the PAN through which the device is operating.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x50.
        /// </remarks>
        [XmlIgnore()]
        public UInt16 PANId
        {
            get;
            set;
        }

        /// <summary>
        /// This attribute holds GMK keys required for MAC layer ciphering.
        /// </summary>
        /// /// <remarks>
        /// PIB attribute 0x71.
        /// </remarks>
        [XmlIgnore()]
        public List<GXKeyValuePair<byte, byte[]>> KeyTable
        {
            get;
            set;
        }

        /// <summary>
        /// The outgoing frame counter for this device, used when ciphering frames at MAC layer.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x77.
        /// </remarks>
        [XmlIgnore()]
        public UInt32 FrameCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the tone mask to use during symbol formation.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x110.
        /// </remarks>
        [XmlIgnore()]
        public string ToneMask
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum time to live of tone map parameters entry in the neighbour table in minutes.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x10D.
        /// </remarks>
        [XmlIgnore()]
        public byte TmrTtl
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum number of retransmissions.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x59.
        /// </remarks>
        [XmlIgnore()]
        public byte MaxFrameRetries
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum time to live for an entry in the neighbour table in minutes
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x10E.
        /// </remarks>
        [XmlIgnore()]
        public byte NeighbourTableEntryTtl
        {
            get;
            set;
        }

        /// <summary>
        /// The neighbour table contains information about all the devices within the POS of the device
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x010A.
        /// </remarks>
        [XmlIgnore()]
        public GXDLMSNeighbourTable[] NeighbourTable
        {
            get;
            set;
        }

        /// <summary>
        /// The high priority contention window size in number of slots.
        /// </summary>
        /// <remarks>
        ///PIB attribute 0x0100.
        /// </remarks>
        [XmlIgnore()]
        public byte HighPriorityWindowSize
        {
            get;
            set;
        }

        /// <summary>
        /// Channel access fairness limit.
        /// </summary>
        /// <remarks>
        ///PIB attribute 0x10C.
        /// </remarks>
        [XmlIgnore()]
        public byte CscmFairnessLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Duration time in seconds for the beacon randomization.
        /// </summary>
        /// <remarks>
        ///PIB attribute 0x111.
        /// </remarks>
        [XmlIgnore()]
        public byte BeaconRandomizationWindowLength
        {
            get;
            set;
        }

        /// <summary>
        /// This parameter controls the adaptive CW linear decrease.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x0112.
        /// </remarks>
        [XmlIgnore()]
        public byte A
        {
            get;
            set;
        }

        /// <summary>
        /// Rate adaptation factor for channel access fairness limit.
        /// </summary>
        /// <remarks>
        ///PIB attribute 0x113.
        /// </remarks>
        [XmlIgnore()]
        public byte K
        {
            get;
            set;
        }

        /// <summary>
        /// Number of consecutive attempts while using minimum CW.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x114.
        /// </remarks>
        [XmlIgnore()]
        public byte MinCwAttempts
        {
            get;
            set;
        }

        /// <summary>
        /// This read only attribute indicates the capability of the node.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x115.
        /// </remarks>
        [XmlIgnore()]
        public byte CenelecLegacyMode
        {
            get;
            set;
        }

        /// <summary>
        /// This read only attribute indicates the capability of the node.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x116.
        /// </remarks>
        [XmlIgnore()]
        public byte FccLegacyMode
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum value of backoff exponent.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x47.
        /// </remarks>
        [XmlIgnore()]
        public byte MaxBe
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum number of backoff attempts.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x4E.
        /// </remarks>
        [XmlIgnore()]
        public byte MaxCsmaBackoffs
        {
            get;
            set;
        }

        /// <summary>
        /// Minimum value of backoff exponent.
        /// </summary>
        /// <remarks>
        /// PIB attribute 0x4F.
        /// </remarks>
        [XmlIgnore()]
        public byte MinBe
        {
            get;
            set;
        }

        /// <summary>
        ///  If true, MAC uses maximum contention window.
        /// </summary>
        [XmlIgnore()]
        public bool MacBroadcastMaxCwEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Attenuation of the output level in dB.
        /// </summary>
        [XmlIgnore()]
        public byte MacTransmitAtten
        {
            get;
            set;
        }

        /// <summary>
        /// The neighbour table contains some information 
        /// about all the devices within the POS of the device.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSMacPosTable[] MacPosTable
        {
            get;
            set;
        }

        /// <summary>
        /// Duplicate frame detection time in seconds.
        /// </summary>
        [XmlIgnore()]
        public byte MacDuplicateDetectionTtl
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ShortAddress, RcCoord, PANId, KeyTable , FrameCounter ,
                ToneMask , TmrTtl , MaxFrameRetries , NeighbourTableEntryTtl , NeighbourTable , HighPriorityWindowSize,
            CscmFairnessLimit, BeaconRandomizationWindowLength, A, K, MinCwAttempts, CenelecLegacyMode,
                FccLegacyMode, MaxBe,MaxCsmaBackoffs, MinBe,
            MacBroadcastMaxCwEnabled, MacTransmitAtten, MacPosTable, MacDuplicateDetectionTtl};
        }

        private static byte[] GetNeighbourTables(GXDLMSNeighbourTable[] tables)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8((byte)DataType.Array);
            if (tables == null)
            {
                bb.SetUInt8(0);
            }
            else
            {
                GXCommon.SetObjectCount(tables.Length, bb);
                foreach (GXDLMSNeighbourTable it in tables)
                {
                    bb.SetUInt8((byte)DataType.Structure);
                    bb.SetUInt8(11);
                    GXCommon.SetData(null, bb, DataType.UInt16, it.ShortAddress);
                    GXCommon.SetData(null, bb, DataType.Boolean, it.Enabled);
                    GXCommon.SetData(null, bb, DataType.BitString, it.ToneMap);
                    GXCommon.SetData(null, bb, DataType.Enum, it.Modulation);
                    GXCommon.SetData(null, bb, DataType.Int8, it.TxGain);
                    GXCommon.SetData(null, bb, DataType.Enum, it.TxRes);
                    GXCommon.SetData(null, bb, DataType.BitString, it.TxCoeff);
                    GXCommon.SetData(null, bb, DataType.UInt8, it.Lqi);
                    GXCommon.SetData(null, bb, DataType.Int8, it.PhaseDifferential);
                    GXCommon.SetData(null, bb, DataType.UInt8, it.TMRValidTime);
                    GXCommon.SetData(null, bb, DataType.UInt8, it.NeighbourValidTime);
                }
            }
            return bb.Array();
        }

        private static byte[] GetPosTables(GXDLMSMacPosTable[] tables)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8((byte)DataType.Array);
            if (tables == null)
            {
                bb.SetUInt8(0);
            }
            else
            {
                GXCommon.SetObjectCount(tables.Length, bb);
                foreach (var it in tables)
                {
                    bb.SetUInt8((byte)DataType.Structure);
                    bb.SetUInt8(3);
                    GXCommon.SetData(null, bb, DataType.UInt16, it.ShortAddress);
                    GXCommon.SetData(null, bb, DataType.UInt8, it.LQI);
                    GXCommon.SetData(null, bb, DataType.UInt8, it.ValidTime);
                }
            }
            return bb.Array();
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                List<GXDLMSNeighbourTable> list = new List<GXDLMSNeighbourTable>();
                UInt16 index = (UInt16)e.Value;
                foreach (var it in NeighbourTable)
                {
                    if (it.ShortAddress == index)
                    {
                        list.Add(it);
                    }
                }
                return GetNeighbourTables(list.ToArray());
            }
            else if (e.Index == 2)
            {
                List<GXDLMSMacPosTable> list = new List<GXDLMSMacPosTable>();
                UInt16 index = (UInt16)e.Value;
                foreach (var it in MacPosTable)
                {
                    if (it.ShortAddress == index)
                    {
                        list.Add(it);
                    }
                }
                return GetPosTables(list.ToArray());

            }
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
            //MacShortAddress
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //MacRcCoord
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //MacPANId
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //MackeyTable
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //MacFrameCounter
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //MacToneMask
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //MacTmrTtl
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //MacMaxFrameRetries
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //MacneighbourTableEntryTtl
            if (all || CanRead(10))
            {
                attributes.Add(10);
            }
            //MacNeighbourTable
            if (all || CanRead(11))
            {
                attributes.Add(11);
            }
            //MachighPriorityWindowSize
            if (all || CanRead(12))
            {
                attributes.Add(12);
            }
            //MacCscmFairnessLimit
            if (all || CanRead(13))
            {
                attributes.Add(13);
            }
            //MacBeaconRandomizationWindowLength
            if (all || CanRead(14))
            {
                attributes.Add(14);
            }
            //MacA
            if (all || CanRead(15))
            {
                attributes.Add(15);
            }
            //MacK
            if (all || CanRead(16))
            {
                attributes.Add(16);
            }
            //MacMinCwAttempts
            if (all || CanRead(17))
            {
                attributes.Add(17);
            }
            //MacCenelecLegacyMode
            if (all || CanRead(18))
            {
                attributes.Add(18);
            }
            //MacFCCLegacyMode
            if (all || CanRead(19))
            {
                attributes.Add(19);
            }
            //MacMaxBe
            if (all || CanRead(20))
            {
                attributes.Add(20);
            }
            //MacMaxCsmaBackoffs,
            if (all || CanRead(21))
            {
                attributes.Add(21);
            }
            //MacMinBe
            if (all || CanRead(22))
            {
                attributes.Add(22);
            }
            //MacBroadcastMaxCwEnabled
            if (all || CanRead(23))
            {
                attributes.Add(23);
            }
            //MacTransmitAtten
            if (all || CanRead(24))
            {
                attributes.Add(24);
            }
            //MacPosTable
            if (all || CanRead(25))
            {
                attributes.Add(25);
            }
            //MacDuplicateDetectionTtl
            if (Version > 2)
            {
                if (all || CanRead(26))
                {
                    attributes.Add(26);
                }
            }
            return attributes.ToArray();
        }

        /// <summary>
        /// Retrieves the MAC neighbour table.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="address">MAC short address</param>
        /// <returns>Generated bytes.</returns>
        /// <seealso cref="ParseNeighbourTableEntry"/>
        public byte[][] GetNeighbourTableEntry(GXDLMSClient client, UInt16 address)
        {
            return client.Method(this, 1, address);
        }

        private static GXDLMSNeighbourTable[] ParseNeighbourTableEntry(object value)
        {
            List<GXDLMSNeighbourTable> list = new List<GXDLMSNeighbourTable>();
            if (value != null)
            {
                foreach (object tmp in (IEnumerable<object>)value)
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
                    GXDLMSNeighbourTable it = new GXDLMSNeighbourTable();
                    it.ShortAddress = Convert.ToUInt16(arr[0]);
                    it.Enabled = Convert.ToBoolean(arr[1]);
                    it.ToneMap = Convert.ToString(arr[2]);
                    it.Modulation = (Modulation)Convert.ToInt32(arr[3]);
                    it.TxGain = Convert.ToSByte(arr[4]);
                    it.TxRes = (GainResolution)Convert.ToInt32(arr[5]);
                    it.TxCoeff = Convert.ToString(arr[6]);
                    it.Lqi = Convert.ToByte(arr[7]);
                    it.PhaseDifferential = Convert.ToSByte(arr[8]);
                    it.TMRValidTime = Convert.ToByte(arr[9]);
                    it.NeighbourValidTime = Convert.ToByte(arr[10]);
                    list.Add(it);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Parse neighbour table entry.
        /// </summary>
        /// <param name="reply">Received reply</param>
        /// <returns></returns>
        /// <seealso cref="GetNeighbourTableEntry"/>
        public GXDLMSNeighbourTable[] ParseNeighbourTableEntry(GXByteBuffer reply)
        {
            GXDataInfo info = new GXDataInfo();
            object value = GXCommon.GetData(null, reply, info);
            return ParseNeighbourTableEntry(value);
        }

        /// <summary>
        /// Retrieves the mac POS table.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="address">MAC short address</param>
        /// <returns>Generated bytes.</returns>
        /// <seealso cref="ParsePosTableEntry"/>
        public byte[][] GetPosTableEntry(GXDLMSClient client, UInt16 address)
        {
            return client.Method(this, 2, address);
        }

        private static GXDLMSMacPosTable[] ParsePosTableEntry(object value)
        {
            List<GXDLMSMacPosTable> list = new List<GXDLMSMacPosTable>();
            if (value != null)
            {
                foreach (object tmp in (IEnumerable<object>)value)
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
                    GXDLMSMacPosTable it = new GXDLMSMacPosTable();
                    it.ShortAddress = Convert.ToUInt16(arr[0]);
                    it.LQI = Convert.ToByte(arr[1]);
                    it.ValidTime = Convert.ToByte(arr[2]);
                    list.Add(it);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Parse MAC POS tables.
        /// </summary>
        /// <param name="reply">Received reply</param>
        /// <returns></returns>
        /// <seealso cref="GetPosTableEntry"/>
        public GXDLMSMacPosTable[] ParsePosTableEntry(GXByteBuffer reply)
        {
            GXDataInfo info = new GXDataInfo();
            object value = GXCommon.GetData(null, reply, info);
            return ParsePosTableEntry(value);
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "MacShortAddress", "MacRcCoord", "MacPANId", "MackeyTable ", "MacFrameCounter",
                "MacToneMask", "MacTmrTtl", "MacMaxFrameRetries", "MacneighbourTableEntryTtl", "MacNeighbourTable", "MachighPriorityWindowSize",
                "MacCscmFairnessLimit", "MacBeaconRandomizationWindowLength", "MacA", "MacK", "MacMinCwAttempts", "MacCenelecLegacyMode",
                "MacFCCLegacyMode", "MacMaxBe", "MacMaxCsmaBackoffs", "MacMinBe",
            "MacBroadcastMaxCwEnabled", "MacTransmitAtten", "MacPosTable", "MacDuplicateDetectionTtl"};
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "MAC get neighbour table entry", "MAC get POS tableentry" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 3;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            if (Version == 3)
            {
                return 26;
            }
            if (Version == 2)
            {
                return 25;
            }
            return 22;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            if (Version == 3)
            {
                return 2;
            }
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
                    return DataType.UInt16;
                //MacRcCoord
                case 3:
                    return DataType.UInt16;
                //MacPANId
                case 4:
                    return DataType.UInt16;
                //MackeyTable
                case 5:
                    return DataType.Array;
                //MacFrameCounter
                case 6:
                    return DataType.UInt16;
                //MacToneMask
                case 7:
                    return DataType.BitString;
                //MacTmrTtl
                case 8:
                    return DataType.UInt8;
                //MacMaxFrameRetries
                case 9:
                    return DataType.UInt8;
                //MacneighbourTableEntryTtl
                case 10:
                    return DataType.UInt8;
                //MacNeighbourTable
                case 11:
                    return DataType.Array;
                //MachighPriorityWindowSize
                case 12:
                    return DataType.UInt8;
                //MacCscmFairnessLimit
                case 13:
                    return DataType.UInt8;
                //MacBeaconRandomizationWindowLength
                case 14:
                    return DataType.UInt8;
                //MacA
                case 15:
                    return DataType.UInt8;
                //MacK
                case 16:
                    return DataType.UInt8;
                //MacMinCwAttempts
                case 17:
                    return DataType.UInt8;
                //MacCenelecLegacyMode
                case 18:
                    return DataType.UInt8;
                //MacFCCLegacyMode
                case 19:
                    return DataType.UInt8;
                //MacMaxBe
                case 20:
                    return DataType.UInt8;
                //MacMaxCsmaBackoffs
                case 21:
                    return DataType.UInt8;
                //MacMinBe
                case 22:
                    return DataType.UInt8;
                //MacBroadcastMaxCwEnabled
                case 23:
                    return DataType.Boolean;
                //MacTransmitAtten
                case 24:
                    return DataType.UInt8;
                //MacPosTable
                case 25:
                    return DataType.Array;
                case 26:
                    //MacDuplicateDetectionTtl
                    return DataType.UInt8;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return ShortAddress;
            }
            if (e.Index == 3)
            {
                return RcCoord;
            }
            if (e.Index == 4)
            {
                return PANId;
            }
            if (e.Index == 5)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8((byte)DataType.Array);
                if (KeyTable == null)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(KeyTable.Count, bb);
                    foreach (GXKeyValuePair<byte, byte[]> it in KeyTable)
                    {
                        bb.SetUInt8((byte)DataType.Structure);
                        bb.SetUInt8(2);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.Key);
                        GXCommon.SetData(settings, bb, DataType.OctetString, it.Value);
                    }
                }
                return bb.Array();
            }
            if (e.Index == 6)
            {
                return FrameCounter;
            }
            if (e.Index == 7)
            {
                return ToneMask;
            }
            if (e.Index == 8)
            {
                return TmrTtl;
            }
            if (e.Index == 9)
            {
                return MaxFrameRetries;
            }
            if (e.Index == 10)
            {
                return NeighbourTableEntryTtl;
            }
            if (e.Index == 11)
            {
                return GetNeighbourTables(NeighbourTable);
            }
            if (e.Index == 12)
            {
                return HighPriorityWindowSize;
            }
            if (e.Index == 13)
            {
                return CscmFairnessLimit;
            }
            if (e.Index == 14)
            {
                return BeaconRandomizationWindowLength;
            }
            if (e.Index == 15)
            {
                return A;
            }
            if (e.Index == 16)
            {
                return K;
            }
            if (e.Index == 17)
            {
                return MinCwAttempts;
            }
            if (e.Index == 18)
            {
                return CenelecLegacyMode;
            }
            if (e.Index == 19)
            {
                return FccLegacyMode;
            }
            if (e.Index == 20)
            {
                return MaxBe;
            }
            if (e.Index == 21)
            {
                return MaxCsmaBackoffs;
            }
            if (e.Index == 22)
            {
                return MinBe;
            }
            if (e.Index == 23)
            {
                return MacBroadcastMaxCwEnabled;
            }
            if (e.Index == 24)
            {
                return MacTransmitAtten;
            }
            if (e.Index == 25)
            {
                return GetPosTables(MacPosTable);
            }
            if (e.Index == 26)
            {
                return MacDuplicateDetectionTtl;
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                LogicalName = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 2)
            {
                ShortAddress = Convert.ToUInt16(e.Value);
            }
            else if (e.Index == 3)
            {
                RcCoord = Convert.ToUInt16(e.Value);
            }
            else if (e.Index == 4)
            {
                PANId = Convert.ToUInt16(e.Value);
            }
            else if (e.Index == 5)
            {
                KeyTable.Clear();
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
                        KeyTable.Add(new GXKeyValuePair<byte, byte[]>(Convert.ToByte(arr[0]), (byte[])arr[1]));
                    }
                }
            }
            else if (e.Index == 6)
            {
                FrameCounter = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 7)
            {
                ToneMask = Convert.ToString(e.Value);
            }
            else if (e.Index == 8)
            {
                TmrTtl = Convert.ToByte(e.Value);
            }
            else if (e.Index == 9)
            {
                MaxFrameRetries = Convert.ToByte(e.Value);
            }
            else if (e.Index == 10)
            {
                NeighbourTableEntryTtl = Convert.ToByte(e.Value);
            }
            else if (e.Index == 11)
            {
                NeighbourTable = ParseNeighbourTableEntry(e.Value);
            }
            else if (e.Index == 12)
            {
                HighPriorityWindowSize = Convert.ToByte(e.Value);
            }
            else if (e.Index == 13)
            {
                CscmFairnessLimit = Convert.ToByte(e.Value);
            }
            else if (e.Index == 14)
            {
                BeaconRandomizationWindowLength = Convert.ToByte(e.Value);
            }
            else if (e.Index == 15)
            {
                A = Convert.ToByte(e.Value);
            }
            else if (e.Index == 16)
            {
                K = Convert.ToByte(e.Value);
            }
            else if (e.Index == 17)
            {
                MinCwAttempts = Convert.ToByte(e.Value);
            }
            else if (e.Index == 18)
            {
                CenelecLegacyMode = Convert.ToByte(e.Value);
            }
            else if (e.Index == 19)
            {
                FccLegacyMode = Convert.ToByte(e.Value);
            }
            else if (e.Index == 20)
            {
                MaxBe = Convert.ToByte(e.Value);
            }
            else if (e.Index == 21)
            {
                MaxCsmaBackoffs = Convert.ToByte(e.Value);
            }
            else if (e.Index == 22)
            {
                MinBe = Convert.ToByte(e.Value);
            }
            else if (e.Index == 23)
            {
                MacBroadcastMaxCwEnabled = Convert.ToBoolean(e.Value);
            }
            else if (e.Index == 24)
            {
                MacTransmitAtten = Convert.ToByte(e.Value);
            }
            else if (e.Index == 25)
            {
                MacPosTable = ParsePosTableEntry(e.Value);
            }
            else if (e.Index == 26)
            {
                MacDuplicateDetectionTtl = Convert.ToByte(e.Value);
            }

            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        private void LoadKeyTable(GXXmlReader reader)
        {
            KeyTable.Clear();
            if (reader.IsStartElement("KeyTable", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    byte k = (byte)reader.ReadElementContentAsInt("Key");
                    byte[] d = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("Data"));
                    KeyTable.Add(new GXKeyValuePair<byte, byte[]>(k, d));
                }
                reader.ReadEndElement("KeyTable");
            }
        }

        private void LoadNeighbourTable(GXXmlReader reader)
        {
            List<GXDLMSNeighbourTable> list = new List<Objects.GXDLMSNeighbourTable>();
            if (reader.IsStartElement("NeighbourTable", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSNeighbourTable it = new GXDLMSNeighbourTable();
                    it.ShortAddress = (UInt16)reader.ReadElementContentAsInt("ShortAddress");
                    it.Enabled = reader.ReadElementContentAsInt("Enabled") != 0;
                    it.ToneMap = reader.ReadElementContentAsString("ToneMap");
                    it.Modulation = (Modulation)reader.ReadElementContentAsInt("Modulation");
                    it.TxGain = (sbyte)reader.ReadElementContentAsInt("TxGain");
                    it.TxRes = (GainResolution)reader.ReadElementContentAsInt("TxRes");
                    it.TxCoeff = reader.ReadElementContentAsString("TxCoeff");
                    it.Lqi = (byte)reader.ReadElementContentAsInt("Lqi");
                    it.PhaseDifferential = (sbyte)reader.ReadElementContentAsInt("PhaseDifferential");
                    it.TMRValidTime = (byte)reader.ReadElementContentAsInt("TMRValidTime");
                    it.NeighbourValidTime = (byte)reader.ReadElementContentAsInt("NeighbourValidTime");
                    list.Add(it);
                }
                reader.ReadEndElement("NeighbourTable");
            }
            NeighbourTable = list.ToArray();
        }


        private void LoadMacPosTable(GXXmlReader reader)
        {
            List<GXDLMSMacPosTable> list = new List<GXDLMSMacPosTable>();
            if (reader.IsStartElement("MacPosTable", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSMacPosTable it = new GXDLMSMacPosTable();
                    it.ShortAddress = (UInt16)reader.ReadElementContentAsInt("ShortAddress");
                    it.LQI = (byte)reader.ReadElementContentAsInt("LQI");
                    it.ValidTime = (byte)reader.ReadElementContentAsInt("ValidTime");
                    list.Add(it);
                }
                reader.ReadEndElement("MacPosTable");
            }
            MacPosTable = list.ToArray();
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            ShortAddress = (UInt16)reader.ReadElementContentAsInt("ShortAddress");
            RcCoord = (UInt16)reader.ReadElementContentAsInt("RcCoord");
            PANId = (UInt16)reader.ReadElementContentAsInt("PANId");
            LoadKeyTable(reader);
            FrameCounter = (UInt16)reader.ReadElementContentAsInt("FrameCounter");
            ToneMask = reader.ReadElementContentAsString("ToneMask");
            TmrTtl = (byte)reader.ReadElementContentAsInt("TmrTtl");
            MaxFrameRetries = (byte)reader.ReadElementContentAsInt("MaxFrameRetries");
            NeighbourTableEntryTtl = (byte)reader.ReadElementContentAsInt("NeighbourTableEntryTtl");
            LoadNeighbourTable(reader);
            HighPriorityWindowSize = (byte)reader.ReadElementContentAsInt("HighPriorityWindowSize");
            CscmFairnessLimit = (byte)reader.ReadElementContentAsInt("CscmFairnessLimit");
            BeaconRandomizationWindowLength = (byte)reader.ReadElementContentAsInt("BeaconRandomizationWindowLength");
            A = (byte)reader.ReadElementContentAsInt("A");
            K = (byte)reader.ReadElementContentAsInt("K");
            MinCwAttempts = (byte)reader.ReadElementContentAsInt("MinCwAttempts");
            CenelecLegacyMode = (byte)reader.ReadElementContentAsInt("CenelecLegacyMode");
            FccLegacyMode = (byte)reader.ReadElementContentAsInt("FccLegacyMode");
            MaxBe = (byte)reader.ReadElementContentAsInt("MaxBe");
            MaxCsmaBackoffs = (byte)reader.ReadElementContentAsInt("MaxCsmaBackoffs");
            MinBe = (byte)reader.ReadElementContentAsInt("MinBe");
            MacBroadcastMaxCwEnabled = reader.ReadElementContentAsInt("MacBroadcastMaxCwEnabled") != 0;
            MacTransmitAtten = (byte)reader.ReadElementContentAsInt("MacTransmitAtten");
            LoadMacPosTable(reader);
            MacDuplicateDetectionTtl = (byte)reader.ReadElementContentAsInt("MacDuplicateDetectionTtl");
        }

        void SaveKeyTable(GXXmlWriter writer, int index)
        {
            writer.WriteStartElement("KeyTable", index);
            if (KeyTable != null)
            {
                foreach (GXKeyValuePair<byte, byte[]> it in KeyTable)
                {
                    writer.WriteStartElement("Item", index);
                    writer.WriteElementString("Key", it.Key, index);
                    writer.WriteElementString("Data", GXDLMSTranslator.ToHex(it.Value), index);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();//KeyTable
        }

        void SaveNeighbourTable(GXXmlWriter writer, int index)
        {
            writer.WriteStartElement("NeighbourTable", index);
            if (NeighbourTable != null)
            {
                foreach (GXDLMSNeighbourTable it in NeighbourTable)
                {
                    writer.WriteStartElement("Item", index);
                    writer.WriteElementString("ShortAddress", it.ShortAddress, index);
                    writer.WriteElementString("Enabled", it.Enabled, index);
                    writer.WriteElementString("ToneMap", it.ToneMap, index);
                    writer.WriteElementString("Modulation", (int)it.Modulation, index);
                    writer.WriteElementString("TxGain", it.TxGain, index);
                    writer.WriteElementString("TxRes", (int)it.TxRes, index);
                    writer.WriteElementString("TxCoeff", it.TxCoeff, index);
                    writer.WriteElementString("Lqi", it.Lqi, index);
                    writer.WriteElementString("PhaseDifferential", it.PhaseDifferential, index);
                    writer.WriteElementString("TMRValidTime", it.TMRValidTime, index);
                    writer.WriteElementString("NeighbourValidTime", it.NeighbourValidTime, index);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();//NeighbourTable
        }

        void SaveMacPosTable(GXXmlWriter writer)
        {
            writer.WriteStartElement("MacPosTable", 25);
            if (MacPosTable != null)
            {
                foreach (var it in MacPosTable)
                {
                    writer.WriteStartElement("Item", 25);
                    writer.WriteElementString("ShortAddress", it.ShortAddress, 25);
                    writer.WriteElementString("LQI", it.LQI, 25);
                    writer.WriteElementString("ValidTime", it.ValidTime, 25);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();//MacPosTable
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("ShortAddress", ShortAddress, 2);
            writer.WriteElementString("RcCoord", RcCoord, 3);
            writer.WriteElementString("PANId", PANId, 4);
            SaveKeyTable(writer, 5);
            writer.WriteElementString("FrameCounter", FrameCounter, 6);
            writer.WriteElementString("ToneMask", ToneMask, 7);
            writer.WriteElementString("TmrTtl", TmrTtl, 8);
            writer.WriteElementString("MaxFrameRetries", MaxFrameRetries, 9);
            writer.WriteElementString("NeighbourTableEntryTtl", NeighbourTableEntryTtl, 10);
            SaveNeighbourTable(writer, 11);
            writer.WriteElementString("HighPriorityWindowSize", HighPriorityWindowSize, 12);
            writer.WriteElementString("CscmFairnessLimit", CscmFairnessLimit, 13);
            writer.WriteElementString("BeaconRandomizationWindowLength", BeaconRandomizationWindowLength, 14);
            writer.WriteElementString("A", A, 15);
            writer.WriteElementString("K", K, 16);
            writer.WriteElementString("MinCwAttempts", MinCwAttempts, 17);
            writer.WriteElementString("CenelecLegacyMode", CenelecLegacyMode, 18);
            writer.WriteElementString("FccLegacyMode", FccLegacyMode, 19);
            writer.WriteElementString("MaxBe", MaxBe, 20);
            writer.WriteElementString("MaxCsmaBackoffs", MaxCsmaBackoffs, 21);
            writer.WriteElementString("MinBe", MinBe, 22);
            writer.WriteElementString("MacBroadcastMaxCwEnabled", MacBroadcastMaxCwEnabled, 23);
            writer.WriteElementString("MacTransmitAtten", MacTransmitAtten, 24);
            SaveMacPosTable(writer);
            writer.WriteElementString("MacDuplicateDetectionTtl", MacDuplicateDetectionTtl, 26);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
