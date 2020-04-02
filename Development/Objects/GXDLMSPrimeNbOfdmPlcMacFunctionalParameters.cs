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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSPrimeNbOfdmPlcMacFunctionalParameters
    /// </summary>
    public class GXDLMSPrimeNbOfdmPlcMacFunctionalParameters : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSPrimeNbOfdmPlcMacFunctionalParameters()
        : this("0.0.28.3.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcMacFunctionalParameters(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcMacFunctionalParameters(string ln, ushort sn)
        : base(ObjectType.PrimeNbOfdmPlcMacFunctionalParameters, ln, sn)
        {
        }

        /// <summary>
        /// LNID allocated to this node at time of its registration.
        /// </summary>
        [XmlIgnore()]
        public short LnId
        {
            get;
            set;
        }

        /// <summary>
        /// LSID allocated to this node at the time of its promotion.
        /// </summary>
        [XmlIgnore()]
        public byte LsId
        {
            get;
            set;
        }

        /// <summary>
        /// SID of the switch node through which this node is connected to the subnetwork.
        /// </summary>
        [XmlIgnore()]
        public byte SId
        {
            get;
            set;
        }

        /// <summary>
        /// Subnetwork address to which this node is registered.
        /// </summary>
        [XmlIgnore()]
        public byte[] Sna
        {
            get;
            set;
        }

        /// <summary>
        /// Present functional state of the node.
        /// </summary>
        [XmlIgnore()]
        public MacState State
        {
            get;
            set;
        }

        /// <summary>
        /// The SCP length, in symbols, in present frame.
        /// </summary>
        [XmlIgnore()]
        public Int16 ScpLength
        {
            get;
            set;
        }

        /// <summary>
        /// Level of this node in subnetwork hierarchy.
        /// </summary>
        [XmlIgnore()]
        public byte NodeHierarchyLevel

        {
            get;
            set;
        }

        /// <summary>
        /// Number of beacon slots provisioned in present frame structure.
        /// </summary>
        [XmlIgnore()]
        public byte BeaconSlotCount

        {
            get;
            set;
        }

        /// <summary>
        /// Beacon slot in which this device’s switch node transmits its beacon.
        /// </summary>
        [XmlIgnore()]
        public byte BeaconRxSlot

        {
            get;
            set;
        }

        /// <summary>
        /// Beacon slot in which this device transmits its beacon.
        /// </summary>
        [XmlIgnore()]
        public byte BeaconTxSlot

        {
            get;
            set;
        }

        /// <summary>
        /// Number of frames between receptions of two successive beacons.
        /// </summary>
        [XmlIgnore()]
        public byte BeaconRxFrequency

        {
            get;
            set;
        }

        /// <summary>
        /// Number of frames between transmissions of two successive beacons.
        /// </summary>
        [XmlIgnore()]
        public byte BeaconTxFrequency

        {
            get;
            set;
        }

        /// <summary>
        /// This attribute defines the capabilities of the node.
        /// </summary>
        [XmlIgnore()]
        public MacCapabilities Capabilities

        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, LnId, LsId, SId, Sna , State, ScpLength, NodeHierarchyLevel, BeaconSlotCount,
                BeaconRxSlot, BeaconTxSlot, BeaconRxFrequency , BeaconTxFrequency, Capabilities };
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
            //LnId
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //LsId
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //SId
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //SNa
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //State
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //ScpLength
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //NodeHierarchyLevel
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //BeaconSlotCount
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //BeaconRxSlot
            if (all || CanRead(10))
            {
                attributes.Add(10);
            }
            //BeaconTxSlot
            if (all || CanRead(11))
            {
                attributes.Add(11);
            }
            //BeaconRxFrequency
            if (all || CanRead(12))
            {
                attributes.Add(12);
            }
            //BeaconTxFrequency
            if (all || CanRead(13))
            {
                attributes.Add(13);
            }
            //Capabilities
            if (all || CanRead(14))
            {
                attributes.Add(14);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "LnId", "LsId", "SId", "SNa" , "State", "ScpLength",
                "NodeHierarchyLevel", "BeaconSlotCount", "BeaconRxSlot", "BeaconTxSlot", "BeaconRxFrequency" , "BeaconTxFrequency", "Capabilities" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 14;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.Int16;
                case 3:
                    return DataType.UInt8;
                case 4:
                    return DataType.UInt8;
                case 5:
                    return DataType.OctetString;
                case 6:
                    return DataType.Enum;
                case 7:
                    return DataType.Int16;
                case 8:
                    return DataType.UInt8;
                case 9:
                    return DataType.UInt8;
                case 10:
                    return DataType.UInt8;
                case 11:
                    return DataType.UInt8;
                case 12:
                    return DataType.UInt8;
                case 13:
                    return DataType.UInt8;
                case 14:
                    return DataType.UInt16;
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
                    return LnId;
                case 3:
                    return LsId;
                case 4:
                    return SId;
                case 5:
                    return Sna;
                case 6:
                    return State;
                case 7:
                    return ScpLength;
                case 8:
                    return NodeHierarchyLevel;
                case 9:
                    return BeaconSlotCount;
                case 10:
                    return BeaconRxSlot;
                case 11:
                    return BeaconTxSlot;
                case 12:
                    return BeaconRxFrequency;
                case 13:
                    return BeaconTxFrequency;
                case 14:
                    return Capabilities;
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
                    LnId = Convert.ToInt16(e.Value);
                    break;
                case 3:
                    LsId = Convert.ToByte(e.Value);
                    break;
                case 4:
                    SId = Convert.ToByte(e.Value);
                    break;
                case 5:
                    Sna = (byte[])e.Value;
                    break;
                case 6:
                    State = (MacState)Convert.ToByte(e.Value);
                    break;
                case 7:
                    ScpLength = Convert.ToInt16(e.Value);
                    break;
                case 8:
                    NodeHierarchyLevel = Convert.ToByte(e.Value);
                    break;
                case 9:
                    BeaconSlotCount = Convert.ToByte(e.Value);
                    break;
                case 10:
                    BeaconRxSlot = Convert.ToByte(e.Value);
                    break;
                case 11:
                    BeaconTxSlot = Convert.ToByte(e.Value);
                    break;
                case 12:
                    BeaconRxFrequency = Convert.ToByte(e.Value);
                    break;
                case 13:
                    BeaconTxFrequency = Convert.ToByte(e.Value);
                    break;
                case 14:
                    Capabilities = (MacCapabilities)Convert.ToUInt16(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            LnId = (short)reader.ReadElementContentAsInt("LnId");
            LsId = (byte)reader.ReadElementContentAsInt("LsId");
            SId = (byte)reader.ReadElementContentAsInt("SId");
            Sna = GXCommon.HexToBytes(reader.ReadElementContentAsString("SNa"));
            State = (MacState)reader.ReadElementContentAsInt("State");
            ScpLength = (byte)reader.ReadElementContentAsInt("ScpLength");
            NodeHierarchyLevel = (byte)reader.ReadElementContentAsInt("NodeHierarchyLevel");
            BeaconSlotCount = (byte)reader.ReadElementContentAsInt("BeaconSlotCount");
            BeaconRxSlot = (byte)reader.ReadElementContentAsInt("BeaconRxSlot");
            BeaconTxSlot = (byte)reader.ReadElementContentAsInt("BeaconTxSlot");
            BeaconRxFrequency = (byte)reader.ReadElementContentAsInt("BeaconRxFrequency");
            BeaconTxFrequency = (byte)reader.ReadElementContentAsInt("BeaconTxFrequency");
            Capabilities = (MacCapabilities)reader.ReadElementContentAsInt("Capabilities");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("LnId", LnId);
            writer.WriteElementString("LsId", LsId);
            writer.WriteElementString("SId", SId);
            writer.WriteElementString("SNa", GXCommon.ToHex(Sna, false));
            writer.WriteElementString("State", (int)State);
            writer.WriteElementString("ScpLength", ScpLength);
            writer.WriteElementString("NodeHierarchyLevel", NodeHierarchyLevel);
            writer.WriteElementString("BeaconSlotCount", BeaconSlotCount);
            writer.WriteElementString("BeaconRxSlot", BeaconRxSlot);
            writer.WriteElementString("BeaconTxSlot", BeaconTxSlot);
            writer.WriteElementString("BeaconRxFrequency", BeaconRxFrequency);
            writer.WriteElementString("BeaconTxFrequency", BeaconTxFrequency);
            writer.WriteElementString("Capabilities", (int)Capabilities);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
