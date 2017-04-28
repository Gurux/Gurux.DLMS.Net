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
using Gurux.DLMS.Internal;
using System.Xml;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Stores counters related to the frame exchange, transmission and repetition phases.
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

        //desynchronization_listing
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

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
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

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            attributes.Add(2);
            attributes.Add(3);
            attributes.Add(4);
            attributes.Add(5);
            attributes.Add(6);
            attributes.Add(7);
            attributes.Add(8);
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "SynchronizationRegister", "Desynchronization listing", "BroadcastFramesCounter",
                              "RepetitionsCounter", "TransmissionsCounter", "CrcOkFramesCounter", "CrcNOkFramesCounter"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 8;
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
            //SynchronizationRegister
            if (index == 2)
            {
                return DataType.Array;
            }
            //Desynchronization listing
            if (index == 3)
            {
                return DataType.Structure;
            }
            //BroadcastFramesCounter,
            if (index == 4)
            {
                return DataType.Array;
            }
            //RepetitionsCounter
            if (index == 5)
            {
                return DataType.UInt32;
            }
            //TransmissionsCounter
            if (index == 6)
            {
                return DataType.UInt32;
            }
            //CrcOkFramesCounter
            if (index == 7)
            {
                return DataType.UInt32;
            }
            //CrcNOkFramesCounter
            if (index == 8)
            {
                return DataType.UInt32;
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
                        bb.SetUInt8(2);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.Key);
                        GXCommon.SetData(settings, bb, DataType.UInt32, it.Value);
                    }
                }
                return bb.Array();
            }
            if (e.Index == 3)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8(DataType.Structure);
                bb.SetUInt8(5);
                GXCommon.SetData(settings, bb, DataType.UInt32, PhysicalLayerDesynchronization);
                GXCommon.SetData(settings, bb, DataType.UInt32, TimeOutNotAddressedDesynchronization);
                GXCommon.SetData(settings, bb, DataType.UInt32, TimeOutFrameNotOkDesynchronization);
                GXCommon.SetData(settings, bb, DataType.UInt32, WriteRequestDesynchronization);
                GXCommon.SetData(settings, bb, DataType.UInt32, WrongInitiatorDesynchronization);
                return bb.Array();
            }
            if (e.Index == 4)
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
                        bb.SetUInt8(2);
                        GXCommon.SetData(settings, bb, DataType.UInt16, it.Key);
                        GXCommon.SetData(settings, bb, DataType.UInt32, it.Value);
                    }
                }
                return bb.Array();
            }
            if (e.Index == 5)
            {
                return RepetitionsCounter;
            }
            if (e.Index == 6)
            {
                return TransmissionsCounter;
            }
            if (e.Index == 7)
            {
                return CrcOkFramesCounter;
            }
            if (e.Index == 8)
            {
                return CrcNOkFramesCounter;
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
                SynchronizationRegister.Clear();
                if (e.Value != null)
                {
                    foreach (object it in (object[])e.Value)
                    {
                        object[] tmp = (object[])it;
                        SynchronizationRegister.Add(new KeyValuePair<UInt16, UInt32>((UInt16)tmp[0], (UInt32)tmp[1]));
                    }
                }
            }
            else if (e.Index == 3)
            {
                object[] tmp = (object[])e.Value;
                PhysicalLayerDesynchronization = (UInt32)tmp[0];
                TimeOutNotAddressedDesynchronization = (UInt32)tmp[1];
                TimeOutFrameNotOkDesynchronization = (UInt32)tmp[2];
                WriteRequestDesynchronization = (UInt32)tmp[3];
                WrongInitiatorDesynchronization = (UInt32)tmp[4];
            }
            else if (e.Index == 4)
            {
                BroadcastFramesCounter.Clear();
                if (e.Value != null)
                {
                    foreach (object it in (object[])e.Value)
                    {
                        object[] tmp = (object[])it;
                        BroadcastFramesCounter.Add(new KeyValuePair<UInt16, UInt32>((UInt16)tmp[0], (UInt32)tmp[1]));
                    }
                }
            }
            else if (e.Index == 5)
            {
                RepetitionsCounter = (UInt32)e.Value;
            }
            else if (e.Index == 6)
            {
                TransmissionsCounter = (UInt32)e.Value;
            }
            else if (e.Index == 7)
            {
                CrcOkFramesCounter = (UInt32)e.Value;
            }
            else if (e.Index == 8)
            {
                CrcNOkFramesCounter = (UInt32)e.Value;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
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
                writer.WriteStartElement("SynchronizationRegisters");
                foreach (KeyValuePair<UInt16, UInt32> it in SynchronizationRegister)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Key", it.Key);
                    writer.WriteElementString("Value", it.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteElementString("PhysicalLayerDesynchronization", PhysicalLayerDesynchronization);
            writer.WriteElementString("TimeOutNotAddressedDesynchronization", TimeOutNotAddressedDesynchronization);
            writer.WriteElementString("TimeOutFrameNotOkDesynchronization", TimeOutFrameNotOkDesynchronization);
            writer.WriteElementString("WriteRequestDesynchronization", WriteRequestDesynchronization);
            writer.WriteElementString("WrongInitiatorDesynchronization", WrongInitiatorDesynchronization);
            if (BroadcastFramesCounter != null)
            {
                writer.WriteStartElement("BroadcastFramesCounters");
                foreach (KeyValuePair<UInt16, UInt32> it in BroadcastFramesCounter)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Key", it.Key);
                    writer.WriteElementString("Value", it.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteElementString("RepetitionsCounter", RepetitionsCounter);
            writer.WriteElementString("TransmissionsCounter", TransmissionsCounter);
            writer.WriteElementString("CrcOkFramesCounter", CrcOkFramesCounter);
            writer.WriteElementString("CrcNOkFramesCounter", CrcNOkFramesCounter);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
