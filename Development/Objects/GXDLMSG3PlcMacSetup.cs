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

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ShortAddress, RcCoord, PANId, KeyTable , FrameCounter ,
                ToneMask , TmrTtl , MaxFrameRetries , NeighbourTableEntryTtl , NeighbourTable , HighPriorityWindowSize,
            CscmFairnessLimit, BeaconRandomizationWindowLength, A, K, MinCwAttempts, CenelecLegacyMode,
                FccLegacyMode, MaxBe,MaxCsmaBackoffs, MinBe };
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
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "MacShortAddress", "MacRcCoord", "MacPANId", "MackeyTable ", "MacFrameCounter",
                "MacToneMask", "MacTmrTtl", "MacMaxFrameRetries", "MacneighbourTableEntryTtl", "MacNeighbourTable", "MachighPriorityWindowSize",
                "MacCscmFairnessLimit", "MacBeaconRandomizationWindowLength", "MacA", "MacK", "MacMinCwAttempts", "MacCenelecLegacyMode",
                "MacFCCLegacyMode", "MacMaxBe", "MacMaxCsmaBackoffs", "MacMinBe" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 22;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return base.GetDataType(index);
            }
            //MacRcCoord
            if (index == 3)
            {
                return DataType.UInt16;
            }
            //MacPANId
            if (index == 4)
            {
                return DataType.UInt16;
            }
            //MackeyTable
            if (index == 5)
            {
                return DataType.Array;
            }
            //MacFrameCounter
            if (index == 6)
            {
                return DataType.UInt16;
            }
            //MacToneMask
            if (index == 7)
            {
                return DataType.BitString;
            }
            //MacTmrTtl
            if (index == 8)
            {
                return DataType.UInt8;
            }
            //MacMaxFrameRetries
            if (index == 9)
            {
                return DataType.UInt8;
            }
            //MacneighbourTableEntryTtl
            if (index == 10)
            {
                return DataType.UInt8;
            }
            //MacNeighbourTable
            if (index == 11)
            {
                return DataType.Array;
            }
            //MachighPriorityWindowSize
            if (index == 12)
            {
                return DataType.UInt8;
            }
            //MacCscmFairnessLimit
            if (index == 13)
            {
                return DataType.UInt8;
            }
            //MacBeaconRandomizationWindowLength
            if (index == 14)
            {
                return DataType.UInt8;
            }
            //MacA
            if (index == 15)
            {
                return DataType.UInt8;
            }
            //MacK
            if (index == 16)
            {
                return DataType.UInt8;
            }
            //MacMinCwAttempts
            if (index == 17)
            {
                return DataType.UInt8;
            }
            //MacCenelecLegacyMode
            if (index == 18)
            {
                return DataType.UInt8;
            }
            //MacFCCLegacyMode
            if (index == 19)
            {
                return DataType.UInt8;
            }
            //MacMaxBe
            if (index == 20)
            {
                return DataType.UInt8;
            }
            //MacMaxCsmaBackoffs
            if (index == 21)
            {
                return DataType.UInt8;
            }
            //MacMinBe
            if (index == 22)
            {
                return DataType.UInt8;
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
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.Value);
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
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8((byte)DataType.Array);
                if (NeighbourTable == null)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(NeighbourTable.Length, bb);
                    foreach (GXDLMSNeighbourTable it in NeighbourTable)
                    {
                        bb.SetUInt8((byte)DataType.Structure);
                        bb.SetUInt8(11);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.ShortAddress);
                        GXCommon.SetData(settings, bb, DataType.Boolean, it.Enabled);
                        GXCommon.SetData(settings, bb, DataType.BitString, it.ToneMap);
                        GXCommon.SetData(settings, bb, DataType.Enum, it.Modulation);
                        GXCommon.SetData(settings, bb, DataType.Int8, it.TxGain);
                        GXCommon.SetData(settings, bb, DataType.Enum, it.TxRes);
                        GXCommon.SetData(settings, bb, DataType.BitString, it.TxCoeff);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.Lqi);
                        GXCommon.SetData(settings, bb, DataType.Int8, it.PhaseDifferential);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.TMRValidTime);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.NeighbourValidTime);
                    }
                }
                return bb.Array();
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
                    foreach (List<object> v in (List<object>)e.Value)
                    {
                        KeyTable.Add(new GXKeyValuePair<byte, byte[]>(Convert.ToByte(v[0]), (byte[])v[1]));
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
                List<GXDLMSNeighbourTable> list = new List<GXDLMSNeighbourTable>();
                if (e.Value != null)
                {
                    foreach (List<object> v in (List<object>)e.Value)
                    {
                        GXDLMSNeighbourTable it = new GXDLMSNeighbourTable();
                        it.ShortAddress = Convert.ToUInt16(v[0]);
                        it.Enabled = Convert.ToBoolean(v[1]);
                        it.ToneMap = Convert.ToString(v[2]);
                        it.Modulation = (Modulation)Convert.ToInt32(v[3]);
                        it.TxGain = Convert.ToSByte(v[4]);
                        it.TxRes = (GainResolution)Convert.ToInt32(v[5]);
                        it.TxCoeff = Convert.ToString(v[6]);
                        it.Lqi = Convert.ToByte(v[7]);
                        it.PhaseDifferential = Convert.ToSByte(v[8]);
                        it.TMRValidTime = Convert.ToByte(v[9]);
                        it.NeighbourValidTime = Convert.ToByte(v[10]);
                        list.Add(it);
                    }
                }
                NeighbourTable = list.ToArray();
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
        }


        void SaveKeyTable(GXXmlWriter writer)
        {
            if (KeyTable != null)
            {
                writer.WriteStartElement("KeyTable");
                foreach (GXKeyValuePair<byte, byte[]> it in KeyTable)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Key", it.Key);
                    writer.WriteElementString("Data", GXDLMSTranslator.ToHex(it.Value));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();//KeyTable
            }
        }

        void SaveNeighbourTable(GXXmlWriter writer)
        {
            if (NeighbourTable != null)
            {
                writer.WriteStartElement("NeighbourTable");
                foreach (GXDLMSNeighbourTable it in NeighbourTable)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("ShortAddress", it.ShortAddress);
                    writer.WriteElementString("Enabled", it.Enabled);
                    writer.WriteElementString("ToneMap", it.ToneMap);
                    writer.WriteElementString("Modulation", (int)it.Modulation);
                    writer.WriteElementString("TxGain", it.TxGain);
                    writer.WriteElementString("TxRes", (int)it.TxRes);
                    writer.WriteElementString("TxCoeff", it.TxCoeff);
                    writer.WriteElementString("Lqi", it.Lqi);
                    writer.WriteElementString("PhaseDifferential", it.PhaseDifferential);
                    writer.WriteElementString("TMRValidTime", it.TMRValidTime);
                    writer.WriteElementString("NeighbourValidTime", it.NeighbourValidTime);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();//NeighbourTable
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("ShortAddress", ShortAddress);
            writer.WriteElementString("RcCoord", RcCoord);
            writer.WriteElementString("PANId", PANId);
            SaveKeyTable(writer);
            writer.WriteElementString("FrameCounter", FrameCounter);
            writer.WriteElementString("ToneMask", ToneMask);
            writer.WriteElementString("TmrTtl", TmrTtl);
            writer.WriteElementString("MaxFrameRetries", MaxFrameRetries);
            writer.WriteElementString("NeighbourTableEntryTtl", NeighbourTableEntryTtl);
            SaveNeighbourTable(writer);
            writer.WriteElementString("HighPriorityWindowSize", HighPriorityWindowSize);
            writer.WriteElementString("CscmFairnessLimit", CscmFairnessLimit);
            writer.WriteElementString("BeaconRandomizationWindowLength", BeaconRandomizationWindowLength);
            writer.WriteElementString("A", A);
            writer.WriteElementString("K", K);
            writer.WriteElementString("MinCwAttempts", MinCwAttempts);
            writer.WriteElementString("CenelecLegacyMode", CenelecLegacyMode);
            writer.WriteElementString("FccLegacyMode", FccLegacyMode);
            writer.WriteElementString("MaxBe", MaxBe);
            writer.WriteElementString("MaxCsmaBackoffs", MaxCsmaBackoffs);
            writer.WriteElementString("MinBe", MinBe);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
