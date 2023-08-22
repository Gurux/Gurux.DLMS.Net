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

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSSFSKMacCounters
    /// </summary>
    public class GXDLMSSFSKMacCounters : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSFSKMacCounters()
        : this("0.0.26.3.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSSFSKMacCounters(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSSFSKMacCounters(string ln, ushort sn)
        : base(ObjectType.SFSKMacCounters, ln, sn)
        {
            SynchronizationRegister = new List<KeyValuePair<ushort, uint>>();
            BroadcastFramesCounter = new List<KeyValuePair<ushort, uint>>();
        }


        /// <summary>
        /// List of synchronization registers.
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<UInt16, UInt32>> SynchronizationRegister
        {
            get;
            set;
        }

        public UInt32 PhysicalLayerDesynchronization
        {
            get;
            set;
        }
        public UInt32 TimeOutNotAddressedDesynchronization
        {
            get;
            set;
        }
        public UInt32 TimeOutFrameNotOkDesynchronization
        {
            get;
            set;
        }
        public UInt32 WriteRequestDesynchronization
        {
            get;
            set;
        }
        public UInt32 WrongInitiatorDesynchronization
        {
            get;
            set;
        }

        /// <summary>
        /// List of broadcast frames counter.
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<UInt16, UInt32>> BroadcastFramesCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Repetitions counter.
        /// </summary>
        [XmlIgnore()]
        public UInt32 RepetitionsCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Transmissions counter.
        /// </summary>
        [XmlIgnore()]
        public UInt32 TransmissionsCounter
        {
            get;
            set;
        }

        /// <summary>
        /// CRC OK frames counter.
        /// </summary>
        [XmlIgnore()]
        public UInt32 CrcOkFramesCounter
        {
            get;
            set;
        }

        /// <summary>
        /// CRC NOK frames counter.
        /// </summary>
        [XmlIgnore()]
        public UInt32 CrcNOkFramesCounter
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, SynchronizationRegister, new object[] {PhysicalLayerDesynchronization, TimeOutNotAddressedDesynchronization, TimeOutFrameNotOkDesynchronization, WriteRequestDesynchronization, WrongInitiatorDesynchronization },
                              BroadcastFramesCounter, RepetitionsCounter, TransmissionsCounter, CrcOkFramesCounter, CrcNOkFramesCounter
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
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            if (CanRead(3))
            {
                attributes.Add(3);
            }
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            if (CanRead(6))
            {
                attributes.Add(6);
            }
            if (CanRead(7))
            {
                attributes.Add(7);
            }
            if (CanRead(8))
            {
                attributes.Add(8);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "SynchronizationRegister", "Desynchronization listing", "BroadcastFramesCounter",
                              "RepetitionsCounter", "TransmissionsCounter", "CrcOkFramesCounter", "CrcNOkFramesCounter"
                            };
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
            return 8;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            DataType ret;
            switch (index)
            {
                case 1:
                    ret = DataType.OctetString;
                    break;
                case 2:
                    ret = DataType.Array;
                    break;
                case 3:
                    ret = DataType.Structure;
                    break;
                case 4:
                    ret = DataType.Array;
                    break;
                case 5:
                    ret = DataType.UInt32;
                    break;
                case 6:
                    ret = DataType.UInt32;
                    break;
                case 7:
                    ret = DataType.UInt32;
                    break;
                case 8:
                    ret = DataType.UInt32;
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
                    {
                        GXByteBuffer bb = new GXByteBuffer();
                        bb.SetUInt8(DataType.Array);
                        if (SynchronizationRegister == null)
                        {
                            bb.SetUInt8(0);
                        }
                        else
                        {
                            GXCommon.SetObjectCount(SynchronizationRegister.Count, bb);
                            foreach (var it in SynchronizationRegister)
                            {
                                bb.SetUInt8(DataType.Structure);
                                bb.SetUInt8(2);
                                GXCommon.SetData(settings, bb, DataType.UInt16, it.Key);
                                GXCommon.SetData(settings, bb, DataType.UInt32, it.Value);
                            }
                        }
                        ret = bb.Array();
                    }
                    break;
                case 3:
                    {
                        GXByteBuffer bb = new GXByteBuffer();
                        bb.SetUInt8(DataType.Structure);
                        bb.SetUInt8(5);
                        GXCommon.SetData(settings, bb, DataType.UInt32, PhysicalLayerDesynchronization);
                        GXCommon.SetData(settings, bb, DataType.UInt32, TimeOutNotAddressedDesynchronization);
                        GXCommon.SetData(settings, bb, DataType.UInt32, TimeOutFrameNotOkDesynchronization);
                        GXCommon.SetData(settings, bb, DataType.UInt32, WriteRequestDesynchronization);
                        GXCommon.SetData(settings, bb, DataType.UInt32, WrongInitiatorDesynchronization);
                        ret = bb.Array();
                    }
                    break;
                case 4:
                    {
                        GXByteBuffer bb = new GXByteBuffer();
                        bb.SetUInt8(DataType.Array);
                        if (BroadcastFramesCounter == null)
                        {
                            bb.SetUInt8(0);
                        }
                        else
                        {
                            GXCommon.SetObjectCount(BroadcastFramesCounter.Count, bb);
                            foreach (var it in BroadcastFramesCounter)
                            {
                                bb.SetUInt8(DataType.Structure);
                                bb.SetUInt8(2);
                                GXCommon.SetData(settings, bb, DataType.UInt16, it.Key);
                                GXCommon.SetData(settings, bb, DataType.UInt32, it.Value);
                            }
                        }
                        ret = bb.Array();
                    }
                    break;
                case 5:
                    ret = RepetitionsCounter;
                    break;
                case 6:
                    ret = TransmissionsCounter;
                    break;
                case 7:
                    ret = CrcOkFramesCounter;
                    break;
                case 8:
                    ret = CrcNOkFramesCounter;
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
                    {
                        SynchronizationRegister.Clear();
                        if (e.Value != null)
                        {
                            foreach (object tmp in (IEnumerable<object>)e.Value)
                            {
                                List<object> it;
                                if (tmp is List<object>)
                                {
                                    it = (List<object>)tmp;
                                }
                                else
                                {
                                    it = new List<object>((object[])tmp);
                                }
                                SynchronizationRegister.Add(new KeyValuePair<ushort, uint>((ushort)it[0], (uint)it[1]));
                            }
                        }
                    }
                    break;
                case 3:
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
                            PhysicalLayerDesynchronization = (uint)tmp[0];
                            TimeOutNotAddressedDesynchronization = (uint)tmp[1];
                            TimeOutFrameNotOkDesynchronization = (uint)tmp[2];
                            WriteRequestDesynchronization = (uint)tmp[3];
                            WrongInitiatorDesynchronization = (uint)tmp[4];
                        }
                        else
                        {
                            PhysicalLayerDesynchronization = 0;
                            TimeOutNotAddressedDesynchronization = 0;
                            TimeOutFrameNotOkDesynchronization = 0;
                            WriteRequestDesynchronization = 0;
                            WrongInitiatorDesynchronization = 0;
                        }
                    }
                    break;
                case 4:
                    {
                        BroadcastFramesCounter.Clear();
                        if (e.Value != null)
                        {
                            foreach (List<object> it in (IEnumerable<object>)e.Value)
                            {
                                BroadcastFramesCounter.Add(new KeyValuePair<ushort, uint>((ushort)it[0], (uint)it[1]));
                            }
                        }
                    }
                    break;
                case 5:
                    RepetitionsCounter = (uint)e.Value;
                    break;
                case 6:
                    TransmissionsCounter = (uint)e.Value;
                    break;
                case 7:
                    CrcOkFramesCounter = (uint)e.Value;
                    break;
                case 8:
                    CrcNOkFramesCounter = (uint)e.Value;
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            SynchronizationRegister.Clear();
            if (reader.IsStartElement("SynchronizationRegisters", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    UInt16 k = (UInt16)reader.ReadElementContentAsInt("Key");
                    UInt32 v = (UInt32)reader.ReadElementContentAsInt("Value");
                    SynchronizationRegister.Add(new KeyValuePair<UInt16, UInt32>(k, v));
                }
                reader.ReadEndElement("SynchronizationRegisters");
            }
            PhysicalLayerDesynchronization = (UInt16)reader.ReadElementContentAsInt("PhysicalLayerDesynchronization");
            TimeOutNotAddressedDesynchronization = (UInt16)reader.ReadElementContentAsInt("TimeOutNotAddressedDesynchronization");
            TimeOutFrameNotOkDesynchronization = (UInt16)reader.ReadElementContentAsInt("TimeOutFrameNotOkDesynchronization");
            WriteRequestDesynchronization = (UInt16)reader.ReadElementContentAsInt("WriteRequestDesynchronization");
            WrongInitiatorDesynchronization = (UInt16)reader.ReadElementContentAsInt("WrongInitiatorDesynchronization");
            BroadcastFramesCounter.Clear();
            if (reader.IsStartElement("BroadcastFramesCounters", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    UInt16 k = (UInt16)reader.ReadElementContentAsInt("Key");
                    UInt32 v = (UInt32)reader.ReadElementContentAsInt("Value");
                    BroadcastFramesCounter.Add(new KeyValuePair<UInt16, UInt32>(k, v));
                }
                reader.ReadEndElement("BroadcastFramesCounters");
            }
            RepetitionsCounter = (UInt16)reader.ReadElementContentAsInt("RepetitionsCounter");
            TransmissionsCounter = (UInt16)reader.ReadElementContentAsInt("TransmissionsCounter");
            CrcOkFramesCounter = (UInt16)reader.ReadElementContentAsInt("CrcOkFramesCounter");
            CrcNOkFramesCounter = (UInt16)reader.ReadElementContentAsInt("CrcNOkFramesCounter");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (SynchronizationRegister != null)
            {
                writer.WriteStartElement("SynchronizationRegisters", 2);
                foreach (KeyValuePair<UInt16, UInt32> it in SynchronizationRegister)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("Key", it.Key, 0);
                    writer.WriteElementString("Value", it.Value, 0);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteElementString("PhysicalLayerDesynchronization", PhysicalLayerDesynchronization, 3);
            writer.WriteElementString("TimeOutNotAddressedDesynchronization", TimeOutNotAddressedDesynchronization, 3);
            writer.WriteElementString("TimeOutFrameNotOkDesynchronization", TimeOutFrameNotOkDesynchronization, 3);
            writer.WriteElementString("WriteRequestDesynchronization", WriteRequestDesynchronization, 3);
            writer.WriteElementString("WrongInitiatorDesynchronization", WrongInitiatorDesynchronization, 3);
            if (BroadcastFramesCounter != null)
            {
                writer.WriteStartElement("BroadcastFramesCounters", 4);
                foreach (KeyValuePair<UInt16, UInt32> it in BroadcastFramesCounter)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("Key", it.Key, 0);
                    writer.WriteElementString("Value", it.Value, 0);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteElementString("RepetitionsCounter", RepetitionsCounter, 5);
            writer.WriteElementString("TransmissionsCounter", TransmissionsCounter, 6);
            writer.WriteElementString("CrcOkFramesCounter", CrcOkFramesCounter, 7);
            writer.WriteElementString("CrcNOkFramesCounter", CrcNOkFramesCounter, 8);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
