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
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// MIB variable initiator electrical phase.
    /// </summary>
    public enum InitiatorElectricalPhase
    {
        /// <summary>
        /// Not defined.
        /// </summary>
        NotDefined = 0,
        /// <summary>
        /// Phase 1.
        /// </summary>
        Phase1,
        /// <summary>
        /// Phase 2.
        /// </summary>
        Phase2,
        /// <summary>
        /// Phase 3.
        /// </summary>
        Phase3
    }

    public enum DeltaElectricalPhase
    {
        /// <summary>
        /// Not defined.
        /// </summary>
        NotDefined = 0,
        /// <summary>
        /// The server system is connected to the same phase as the client system.
        /// </summary>
        Same,
        Degrees60,
        Degrees120,
        Degrees180,
        DegreesMinus120,
        DegreesMinus60
    }

    /// <summary>
    /// Repeater enumerator values.
    /// </summary>
    public enum Repeater
    {
        /// <summary>
        /// Newer repeater.
        /// </summary>
        Never = 0,
        /// <summary>
        /// Always repeater.
        /// </summary>
        Always,
        /// <summary>
        /// Dynamic repeater.
        /// </summary>
        Dynamic
    }

    /// <summary>
    /// The class stores the data necessary to set up and manage the physical and the MAC layer of the PLC S-FSK lower layer profile.
    /// </summary>
    public class GXDLMSSFSKPhyMacSetUp : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSFSKPhyMacSetUp()
        : this("0.0.26.0.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSSFSKPhyMacSetUp(string ln)
        : base(ObjectType.SFSKPhyMacSetUp, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSSFSKPhyMacSetUp(string ln, ushort sn)
        : base(ObjectType.SFSKPhyMacSetUp, ln, sn)
        {
        }

        /// <summary>
        /// Initiator electrical phase.
        /// </summary>
        [XmlIgnore()]
        public InitiatorElectricalPhase InitiatorElectricalPhase
        {
            get;
            set;
        }

        /// <summary>
        /// Delta electrical phase.
        /// </summary>
        [XmlIgnore()]
        public DeltaElectricalPhase DeltaElectricalPhase
        {
            get;
            set;
        }

        /// <summary>
        /// Corresponds to the maximum allowed gain bound to be used by the server system in the receiving mode. The default unit is dB.
        /// </summary>
        [XmlIgnore()]
        public byte MaxReceivingGain
        {
            get;
            set;
        }

        /// <summary>
        /// Corresponds to the maximum attenuation bound to be used by the server system in the transmitting mode.The default unit is dB.
        /// </summary>
        [XmlIgnore()]
        public byte MaxTransmittingGain
        {
            get;
            set;
        }

        /// <summary>
        /// Intelligent search initiator process. If the value of the initiator signal is above the value of this attribute, a fast synchronization process is possible.
        /// </summary>
        [XmlIgnore()]
        public byte SearchInitiatorThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// Mark frequency required for S-FSK modulation.
        /// </summary>
        [XmlIgnore()]
        public UInt32 MarkFrequency
        {
            get;
            set;
        }

        /// <summary>
        /// Space frequency required for S-FSK modulation.
        /// </summary>
        [XmlIgnore()]
        public UInt32 SpaceFrequency
        {
            get;
            set;
        }

        /// <summary>
        /// Space frequency required for S-FSK modulation.
        /// </summary>
        [XmlIgnore()]
        public UInt16 MacAddress
        {
            get;
            set;
        }

        [XmlIgnore()]
        public UInt16[] MacGroupAddresses
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies are all frames repeated.
        /// </summary>
        [XmlIgnore()]
        public Repeater Repeater
        {
            get;
            set;
        }

        /// <summary>
        /// Repeater status.
        /// </summary>
        [XmlIgnore()]
        public bool RepeaterStatus
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore()]
        public byte MinDeltaCredit
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore()]
        public UInt16 InitiatorMacAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Synchronization locked/unlocked state.
        /// </summary>
        [XmlIgnore()]
        public bool SynchronizationLocked
        {
            get;
            set;
        }

        /// <summary>
        /// Transmission speed supported by the physical device.
        /// </summary>
        [XmlIgnore()]
        public BaudRate TransmissionSpeed
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, InitiatorElectricalPhase, DeltaElectricalPhase, MaxReceivingGain,
                              MaxTransmittingGain, SearchInitiatorThreshold, new Object[] { MarkFrequency, SpaceFrequency },
                              MacAddress, MacGroupAddresses, Repeater, RepeaterStatus, MinDeltaCredit, InitiatorMacAddress,
                              SynchronizationLocked, TransmissionSpeed
                            };
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
            //InitiatorElectricalPhase
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //DeltaElectricalPhase
            attributes.Add(3);

            //MaxReceivingGain,
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            //MaxTransmittingGain
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            //SearchInitiatorThreshold
            if (CanRead(6))
            {
                attributes.Add(6);
            }
            //MarkFrequency, SpaceFrequency
            if (CanRead(7))
            {
                attributes.Add(7);
            }
            //MacAddress
            attributes.Add(8);

            //MacGroupAddresses
            if (CanRead(9))
            {
                attributes.Add(9);
            }
            //Repeater
            if (CanRead(10))
            {
                attributes.Add(10);
            }
            //RepeaterStatus
            attributes.Add(11);

            //MinDeltaCredit
            attributes.Add(12);

            //InitiatorMacAddress,
            attributes.Add(13);

            //SynchronizationLocked
            attributes.Add(14);

            //TransmissionSpeed
            if (CanRead(15))
            {
                attributes.Add(15);
            }
            return attributes.ToArray();
        }

        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "InitiatorElectricalPhase", "DeltaElectricalPhase", "MaxReceivingGain",
                              "MaxTransmittingGain", "SearchInitiatorThreshold", "Frequency",
                              "MacAddress", "MacGroupAddresses", "Repeater", "RepeaterStatus", "MinDeltaCredit", "InitiatorMacAddress",
                              "SynchronizationLocked", "TransmissionSpeed"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 15;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            //LogicalName
            if (index == 1)
            {
                return DataType.OctetString;
            }
            //InitiatorElectricalPhase
            if (index == 2)
            {
                return DataType.Enum;
            }
            //DeltaElectricalPhase
            if (index == 3)
            {
                return DataType.Enum;
            }
            //MaxReceivingGain
            if (index == 4)
            {
                return DataType.UInt8;
            }
            //MaxTransmittingGain
            if (index == 5)
            {
                return DataType.UInt8;
            }
            //SearchInitiatorThreshold
            if (index == 6)
            {
                return DataType.UInt8;
            }
            //Frequency
            if (index == 7)
            {
                return DataType.Structure;
            }
            //MacAddress
            if (index == 8)
            {
                return DataType.UInt16;
            }
            //MacGroupAddresses
            if (index == 9)
            {
                return DataType.Array;
            }
            //Repeater
            if (index == 10)
            {
                return DataType.Enum;
            }
            //RepeaterStatus
            if (index == 11)
            {
                return DataType.Boolean;
            }
            //MinDeltaCredit
            if (index == 12)
            {
                return DataType.UInt8;
            }
            //InitiatorMacAddress
            if (index == 13)
            {
                return DataType.UInt16;
            }
            //SynchronizationLocked
            if (index == 14)
            {
                return DataType.Boolean;
            }
            //TransmissionSpeed
            if (index == 15)
            {
                return DataType.Enum;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            else if (e.Index == 2)
            {
                return InitiatorElectricalPhase;
            }
            else if (e.Index == 3)
            {
                return DeltaElectricalPhase;
            }
            else if (e.Index == 4)
            {
                return MaxReceivingGain;
            }
            else if (e.Index == 5)
            {
                return MaxTransmittingGain;
            }
            else if (e.Index == 6)
            {
                return SearchInitiatorThreshold;
            }
            else if (e.Index == 7)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8(DataType.Structure);
                bb.SetUInt8(2);
                GXCommon.SetData(settings, bb, DataType.UInt32, MarkFrequency);
                GXCommon.SetData(settings, bb, DataType.UInt32, SpaceFrequency);
                return bb.Array();
            }
            else if (e.Index == 8)
            {
                return MacAddress;
            }
            else if (e.Index == 9)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8(DataType.Array);
                if (MacGroupAddresses == null)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(MacGroupAddresses.Length, bb);
                    foreach (UInt16 it in MacGroupAddresses)
                    {
                        GXCommon.SetData(settings, bb, DataType.UInt16, it);
                    }
                }
                return bb.Array();
            }
            else if (e.Index == 10)
            {
                return Repeater;
            }
            else if (e.Index == 11)
            {
                return RepeaterStatus;
            }
            else if (e.Index == 12)
            {
                return MinDeltaCredit;
            }
            else if (e.Index == 13)
            {
                return InitiatorMacAddress;
            }
            else if (e.Index == 14)
            {
                return SynchronizationLocked;
            }
            else if (e.Index == 15)
            {
                return TransmissionSpeed;
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
                InitiatorElectricalPhase = (InitiatorElectricalPhase)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 3)
            {
                DeltaElectricalPhase = (DeltaElectricalPhase)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 4)
            {
                MaxReceivingGain = (byte)e.Value;
            }
            else if (e.Index == 5)
            {
                MaxTransmittingGain = (byte)e.Value;
            }
            else if (e.Index == 6)
            {
                SearchInitiatorThreshold = (byte)e.Value;
            }
            else if (e.Index == 7)
            {
                object[] tmp = (object[])e.Value;
                MarkFrequency = (UInt32)tmp[0];
                SpaceFrequency = (UInt32)tmp[1];
            }
            else if (e.Index == 8)
            {
                MacAddress = (UInt16)e.Value;
            }
            else if (e.Index == 9)
            {
                List<ushort> list = new List<ushort>();
                if (e.Value != null)
                {
                    foreach (object it in (object[])e.Value)
                    {
                        list.Add((ushort)it);
                    }
                }
                MacGroupAddresses = list.ToArray();
            }
            else if (e.Index == 10)
            {
                Repeater = (Repeater)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 11)
            {
                RepeaterStatus = (bool)e.Value;
            }
            else if (e.Index == 12)
            {
                MinDeltaCredit = (byte)e.Value;
            }
            else if (e.Index == 13)
            {
                InitiatorMacAddress = (UInt16)e.Value;
            }
            else if (e.Index == 14)
            {
                SynchronizationLocked = (bool)e.Value;
            }
            else if (e.Index == 15)
            {
                TransmissionSpeed = (BaudRate)Convert.ToInt32(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            InitiatorElectricalPhase = (InitiatorElectricalPhase)reader.ReadElementContentAsInt("InitiatorElectricalPhase");
            DeltaElectricalPhase = (DeltaElectricalPhase)reader.ReadElementContentAsInt("DeltaElectricalPhase");
            MaxReceivingGain = (byte)reader.ReadElementContentAsInt("MaxReceivingGain");
            MaxTransmittingGain = (byte)reader.ReadElementContentAsInt("MaxTransmittingGain");
            SearchInitiatorThreshold = (byte)reader.ReadElementContentAsInt("SearchInitiatorThreshold");
            MarkFrequency = (UInt32)reader.ReadElementContentAsInt("MarkFrequency");
            SpaceFrequency = (UInt32)reader.ReadElementContentAsInt("SpaceFrequency");
            MacAddress = (UInt16)reader.ReadElementContentAsInt("MacAddress");
            List<UInt16> list = new List<ushort>();
            if (reader.IsStartElement("MacGroupAddresses", true))
            {
                while (reader.IsStartElement("Value", false))
                {
                    list.Add((UInt16)reader.ReadElementContentAsInt("Value"));
                }
                reader.ReadEndElement("MacGroupAddresses");
            }
            MacGroupAddresses = list.ToArray();
            Repeater = (Repeater)reader.ReadElementContentAsInt("Repeater");
            RepeaterStatus = reader.ReadElementContentAsInt("RepeaterStatus") != 0;
            MinDeltaCredit = (byte)reader.ReadElementContentAsInt("MinDeltaCredit");
            InitiatorMacAddress = (UInt16)reader.ReadElementContentAsInt("InitiatorMacAddress");
            SynchronizationLocked = reader.ReadElementContentAsInt("SynchronizationLocked") != 0;
            TransmissionSpeed = (BaudRate)reader.ReadElementContentAsInt("TransmissionSpeed");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("InitiatorElectricalPhase", (int)InitiatorElectricalPhase);
            writer.WriteElementString("DeltaElectricalPhase", (int)DeltaElectricalPhase);
            writer.WriteElementString("MaxReceivingGain", MaxReceivingGain);
            writer.WriteElementString("MaxTransmittingGain", MaxTransmittingGain);
            writer.WriteElementString("SearchInitiatorThreshold", SearchInitiatorThreshold);
            writer.WriteElementString("MarkFrequency", MarkFrequency);
            writer.WriteElementString("SpaceFrequency", SpaceFrequency);
            writer.WriteElementString("MacAddress", MacAddress);
            if (MacGroupAddresses != null)
            {
                writer.WriteStartElement("MacGroupAddresses");
                foreach (UInt16 it in MacGroupAddresses)
                {
                    writer.WriteElementString("Value", it.ToString());
                }
                writer.WriteEndElement();
            }
            writer.WriteElementString("Repeater", (int)Repeater);
            writer.WriteElementString("RepeaterStatus", RepeaterStatus);
            writer.WriteElementString("MinDeltaCredit", MinDeltaCredit);
            writer.WriteElementString("InitiatorMacAddress", InitiatorMacAddress);
            writer.WriteElementString("SynchronizationLocked", SynchronizationLocked);
            writer.WriteElementString("TransmissionSpeed", (int)TransmissionSpeed);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
