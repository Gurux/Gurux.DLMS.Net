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
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSSFSKPhyMacSetUp
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
        /// Mac Address.
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
        /// Min delta credit.
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

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //InitiatorElectricalPhase
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //DeltaElectricalPhase
            attributes.Add(3);

            //MaxReceivingGain,
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //MaxTransmittingGain
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //SearchInitiatorThreshold
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //MarkFrequency, SpaceFrequency
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //MacAddress
            attributes.Add(8);

            //MacGroupAddresses
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //Repeater
            if (all || CanRead(10))
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
            if (all || CanRead(15))
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

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[0];
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
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
            DataType ret;
            //LogicalName
            switch (index)
            {
                case 1:
                    ret = DataType.OctetString;
                    break;
                case 2:
                    ret = DataType.Enum;
                    break;
                case 3:
                    ret = DataType.Enum;
                    break;
                case 4:
                    ret = DataType.UInt8;
                    break;
                case 5:
                    ret = DataType.UInt8;
                    break;
                case 6:
                    ret = DataType.UInt8;
                    break;
                case 7:
                    ret = DataType.Structure;
                    break;
                case 8:
                    ret = DataType.UInt16;
                    break;
                case 9:
                    ret = DataType.Array;
                    break;
                case 10:
                    ret = DataType.Enum;
                    break;
                case 11:
                    ret = DataType.Boolean;
                    break;
                case 12:
                    ret = DataType.UInt8;
                    break;
                case 13:
                    ret = DataType.UInt16;
                    break;
                case 14:
                    ret = DataType.Boolean;
                    break;
                case 15:
                    ret = DataType.Enum;
                    break;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
            return ret;
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            object ret;
            switch (e.Index)
            {
                case 1:
                    ret = GXCommon.LogicalNameToBytes(LogicalName);
                    break;
                case 2:
                    ret = InitiatorElectricalPhase;
                    break;
                case 3:
                    ret = DeltaElectricalPhase;
                    break;
                case 4:
                    ret = MaxReceivingGain;
                    break;
                case 5:
                    ret = MaxTransmittingGain;
                    break;
                case 6:
                    ret = SearchInitiatorThreshold;
                    break;
                case 7:
                    {
                        GXByteBuffer bb = new GXByteBuffer();
                        bb.SetUInt8(DataType.Structure);
                        bb.SetUInt8(2);
                        GXCommon.SetData(settings, bb, DataType.UInt32, MarkFrequency);
                        GXCommon.SetData(settings, bb, DataType.UInt32, SpaceFrequency);
                        ret = bb.Array();
                        break;
                    }

                case 8:
                    ret = MacAddress;
                    break;
                case 9:
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
                            foreach (ushort it in MacGroupAddresses)
                            {
                                GXCommon.SetData(settings, bb, DataType.UInt16, it);
                            }
                        }
                        ret = bb.Array();
                        break;
                    }

                case 10:
                    ret = Repeater;
                    break;
                case 11:
                    ret = RepeaterStatus;
                    break;
                case 12:
                    ret = MinDeltaCredit;
                    break;
                case 13:
                    ret = InitiatorMacAddress;
                    break;
                case 14:
                    ret = SynchronizationLocked;
                    break;
                case 15:
                    ret = TransmissionSpeed;
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    ret = null;
                    break;
            }
            return ret;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    InitiatorElectricalPhase = (InitiatorElectricalPhase)Convert.ToInt32(e.Value);
                    break;
                case 3:
                    DeltaElectricalPhase = (DeltaElectricalPhase)Convert.ToInt32(e.Value);
                    break;
                case 4:
                    MaxReceivingGain = (byte)e.Value;
                    break;
                case 5:
                    MaxTransmittingGain = (byte)e.Value;
                    break;
                case 6:
                    SearchInitiatorThreshold = (byte)e.Value;
                    break;
                case 7:
                    {
                        if (e.Value != null)
                        {
                            List<object> tmp;
                            if (e.Value is List<object>)
                            {
                                tmp = (List<object>)e.Value;
                            }
                            else
                            {
                                tmp = new List<object>((object[])e.Value);
                            }
                            MarkFrequency = (uint)tmp[0];
                            SpaceFrequency = (uint)tmp[1];
                        }
                        else
                        {
                            MarkFrequency = 0;
                            SpaceFrequency = 0;
                        }

                        break;
                    }

                case 8:
                    MacAddress = (ushort)e.Value;
                    break;
                case 9:
                    {
                        List<ushort> list = new List<ushort>();
                        if (e.Value != null)
                        {
                            foreach (object it in (IEnumerable<object>)e.Value)
                            {
                                list.Add((ushort)it);
                            }
                        }
                        MacGroupAddresses = list.ToArray();
                        break;
                    }

                case 10:
                    Repeater = (Repeater)Convert.ToInt32(e.Value);
                    break;
                case 11:
                    RepeaterStatus = (bool)e.Value;
                    break;
                case 12:
                    MinDeltaCredit = (byte)e.Value;
                    break;
                case 13:
                    InitiatorMacAddress = (ushort)e.Value;
                    break;
                case 14:
                    SynchronizationLocked = (bool)e.Value;
                    break;
                case 15:
                    TransmissionSpeed = (BaudRate)Convert.ToInt32(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
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
            writer.WriteElementString("InitiatorElectricalPhase", (int)InitiatorElectricalPhase, 2);
            writer.WriteElementString("DeltaElectricalPhase", (int)DeltaElectricalPhase, 3);
            writer.WriteElementString("MaxReceivingGain", MaxReceivingGain, 4);
            writer.WriteElementString("MaxTransmittingGain", MaxTransmittingGain, 5);
            writer.WriteElementString("SearchInitiatorThreshold", SearchInitiatorThreshold, 6);
            writer.WriteElementString("MarkFrequency", MarkFrequency, 7);
            writer.WriteElementString("SpaceFrequency", SpaceFrequency, 8);
            writer.WriteElementString("MacAddress", MacAddress, 9);
            writer.WriteStartElement("MacGroupAddresses", 10);
            if (MacGroupAddresses != null)
            {
                foreach (UInt16 it in MacGroupAddresses)
                {
                    writer.WriteElementString("Value", it.ToString(), 10);
                }
            }
            writer.WriteEndElement();
            writer.WriteElementString("Repeater", (int)Repeater, 11);
            writer.WriteElementString("RepeaterStatus", RepeaterStatus, 12);
            writer.WriteElementString("MinDeltaCredit", MinDeltaCredit, 13);
            writer.WriteElementString("InitiatorMacAddress", InitiatorMacAddress, 14);
            writer.WriteElementString("SynchronizationLocked", SynchronizationLocked, 15);
            writer.WriteElementString("TransmissionSpeed", (int)TransmissionSpeed, 16);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
