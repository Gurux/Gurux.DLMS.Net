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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSPrimeNbOfdmPlcMacNetworkAdministrationData
    /// </summary>
    public class GXDLMSPrimeNbOfdmPlcMacNetworkAdministrationData : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSPrimeNbOfdmPlcMacNetworkAdministrationData()
        : this("0.0.28.5.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcMacNetworkAdministrationData(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcMacNetworkAdministrationData(string ln, ushort sn)
        : base(ObjectType.PrimeNbOfdmPlcMacNetworkAdministrationData, ln, sn)
        {
        }

        /// <summary>
        /// List of entries in multicast switching table.
        /// </summary>
        [XmlIgnore()]
        public GXMacMulticastEntry[] MulticastEntries
        {
            get;
            set;
        }

        /// <summary>
        /// Switch table.
        /// </summary>
        [XmlIgnore()]
        public Int16[] SwitchTable
        {
            get;
            set;
        }

        /// <summary>
        /// List of entries in multicast switching table.
        /// </summary>
        [XmlIgnore()]
        public GXMacDirectTable[] DirectTable
        {
            get;
            set;
        }

        /// <summary>
        /// List of available switches.
        /// </summary>
        [XmlIgnore()]
        public GXMacAvailableSwitch[] AvailableSwitches
        {
            get;
            set;
        }

        /// <summary>
        /// List of PHY communication parameters.
        /// </summary>
        [XmlIgnore()]
        public GXMacPhyCommunication[] Communications
        {
            get;
            set;
        }

        /// <summary>
        /// Reset all counters.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Reset(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, MulticastEntries, SwitchTable, DirectTable, AvailableSwitches, Communications };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                MulticastEntries = null;
                SwitchTable = null;
                DirectTable = null;
                AvailableSwitches = null;
                Communications = null;
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
            //MulticastEntries
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //SwitchTable
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //DirectTable
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //AvailableSwitches
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //Communications
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "MulticastEntries", "SwitchTable", "DirectTable", "AvailableSwitches", "Communications" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    return DataType.Array;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        byte[] GetMulticastEntries(GXDLMSSettings settings)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8((byte)DataType.Array);
            if (MulticastEntries == null)
            {
                GXCommon.SetObjectCount(0, bb);
            }
            else
            {
                GXCommon.SetObjectCount(MulticastEntries.Length, bb);
                foreach (GXMacMulticastEntry it in MulticastEntries)
                {
                    bb.SetUInt8((byte)DataType.Structure);
                    bb.SetUInt8(2);
                    GXCommon.SetData(settings, bb, DataType.Int8, it.Id);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.Members);
                }
            }
            return bb.Array();
        }

        byte[] GetSwitchTable(GXDLMSSettings settings)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8((byte)DataType.Array);
            if (SwitchTable == null)
            {
                GXCommon.SetObjectCount(0, bb);
            }
            else
            {
                GXCommon.SetObjectCount(SwitchTable.Length, bb);
                foreach (Int16 it in SwitchTable)
                {
                    GXCommon.SetData(settings, bb, DataType.Int16, it);
                }
            }
            return bb.Array();
        }

        byte[] GetDirectTable(GXDLMSSettings settings)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8((byte)DataType.Array);
            if (DirectTable == null)
            {
                GXCommon.SetObjectCount(0, bb);
            }
            else
            {
                GXCommon.SetObjectCount(DirectTable.Length, bb);
                foreach (GXMacDirectTable it in DirectTable)
                {
                    bb.SetUInt8((byte)DataType.Structure);
                    bb.SetUInt8(7);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.SourceSId);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.SourceLnId);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.SourceLcId);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.DestinationSId);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.DestinationLnId);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.DestinationLcId);
                    GXCommon.SetData(settings, bb, DataType.OctetString, it.Did);
                }
            }
            return bb.Array();
        }

        byte[] GetAvailableSwitches(GXDLMSSettings settings)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8((byte)DataType.Array);
            if (AvailableSwitches == null)
            {
                GXCommon.SetObjectCount(0, bb);
            }
            else
            {
                GXCommon.SetObjectCount(AvailableSwitches.Length, bb);
                foreach (GXMacAvailableSwitch it in AvailableSwitches)
                {
                    bb.SetUInt8((byte)DataType.Structure);
                    bb.SetUInt8(5);
                    GXCommon.SetData(settings, bb, DataType.OctetString, it.Sna);
                    GXCommon.SetData(settings, bb, DataType.Int32, it.LsId);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.Level);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.RxLevel);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.RxSnr);
                }
            }

            return bb.Array();
        }

        byte[] GetCommunications(GXDLMSSettings settings)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8((byte)DataType.Array);
            if (Communications == null)
            {
                GXCommon.SetObjectCount(0, bb);
            }
            else
            {
                GXCommon.SetObjectCount(Communications.Length, bb);
                foreach (GXMacPhyCommunication it in Communications)
                {
                    bb.SetUInt8((byte)DataType.Structure);
                    bb.SetUInt8(9);
                    GXCommon.SetData(settings, bb, DataType.OctetString, it.Eui);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.TxPower);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.TxCoding);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.RxCoding);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.RxLvl);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.Snr);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.TxPowerModified);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.TxCodingModified);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.RxCodingModified);
                }
            }
            return bb.Array();
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return GetMulticastEntries(settings);
                case 3:
                    return GetSwitchTable(settings);
                case 4:
                    return GetDirectTable(settings);
                case 5:
                    return GetAvailableSwitches(settings);
                case 6:
                    return GetCommunications(settings);
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return null;
        }

        GXMacMulticastEntry[] SetMulticastEntry(IEnumerable<object> value)
        {
            List<GXMacMulticastEntry> data = new List<GXMacMulticastEntry>();
            if (value != null)
            {
                foreach (object tmp in value)
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
                    GXMacMulticastEntry v = new GXMacMulticastEntry();
                    v.Id = Convert.ToSByte(it[0]);
                    v.Members = Convert.ToInt16(it[1]);
                    data.Add(v);
                }
            }
            return data.ToArray();
        }

        Int16[] SetSwitchTable(IEnumerable<object> value)
        {
            List<Int16> data = new List<Int16>();
            if (value != null)
            {
                foreach (Int16 it in value)
                {
                    data.Add(it);
                }
            }
            return data.ToArray();
        }

        GXMacDirectTable[] SetDirectTable(IEnumerable<object> value)
        {
            List<GXMacDirectTable> data = new List<GXMacDirectTable>();
            if (value != null)
            {
                foreach (object tmp in value)
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
                    GXMacDirectTable v = new GXMacDirectTable();
                    v.SourceSId = Convert.ToInt16(it[0]);
                    v.SourceLnId = Convert.ToInt16(it[1]);
                    v.SourceLcId = Convert.ToInt16(it[2]);
                    v.DestinationSId = Convert.ToInt16(it[3]);
                    v.DestinationLnId = Convert.ToInt16(it[4]);
                    v.DestinationLcId = Convert.ToInt16(it[5]);
                    v.Did = (byte[])it[6];
                    data.Add(v);
                }
            }
            return data.ToArray();
        }

        GXMacAvailableSwitch[] SetAvailableSwitches(IEnumerable<object> value)
        {
            List<GXMacAvailableSwitch> data = new List<GXMacAvailableSwitch>();
            if (value != null)
            {
                foreach (object tmp in value)
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
                    GXMacAvailableSwitch v = new GXMacAvailableSwitch();
                    v.Sna = (byte[])it[0];
                    v.LsId = Convert.ToInt32(it[1]);
                    v.Level = Convert.ToInt16(it[2]);
                    v.RxLevel = Convert.ToInt16(it[3]);
                    v.RxSnr = Convert.ToInt16(it[4]);
                    data.Add(v);
                }
            }
            return data.ToArray();
        }

        GXMacPhyCommunication[] SetCommunications(IEnumerable<object> value)
        {
            List<GXMacPhyCommunication> data = new List<GXMacPhyCommunication>();
            if (value != null)
            {
                foreach (object tmp in value)
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
                    GXMacPhyCommunication v = new GXMacPhyCommunication();
                    v.Eui = (byte[])it[0];
                    v.TxPower = Convert.ToSByte(it[1]);
                    v.TxCoding = Convert.ToSByte(it[2]);
                    v.RxCoding = Convert.ToSByte(it[3]);
                    v.RxLvl = Convert.ToSByte(it[4]);
                    v.Snr = Convert.ToSByte(it[5]);
                    v.TxPowerModified = Convert.ToSByte(it[6]);
                    v.TxCodingModified = Convert.ToSByte(it[7]);
                    v.RxCodingModified = Convert.ToSByte(it[8]);
                    data.Add(v);
                }
            }
            return data.ToArray();
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    MulticastEntries = SetMulticastEntry((IEnumerable<object>)e.Value);
                    break;
                case 3:
                    SwitchTable = SetSwitchTable((IEnumerable<object>)e.Value);
                    break;
                case 4:
                    DirectTable = SetDirectTable((IEnumerable<object>)e.Value);
                    break;
                case 5:
                    AvailableSwitches = SetAvailableSwitches((IEnumerable<object>)e.Value);
                    break;
                case 6:
                    Communications = SetCommunications((IEnumerable<object>)e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        private GXMacMulticastEntry[] LoadMulticastEntries(GXXmlReader reader)
        {
            List<GXMacMulticastEntry> list = new List<GXMacMulticastEntry>();
            if (reader.IsStartElement("MulticastEntries", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXMacMulticastEntry it = new GXMacMulticastEntry();
                    list.Add(it);
                    it.Id = (sbyte)reader.ReadElementContentAsInt("Id");
                    it.Members = (Int16)reader.ReadElementContentAsInt("Members");
                }
                reader.ReadEndElement("MulticastEntries");
            }
            return list.ToArray();
        }

        private Int16[] LoadSwitchTable(GXXmlReader reader)
        {
            List<Int16> list = new List<Int16>();
            if (reader.IsStartElement("SwitchTable", true))
            {
                while (reader.IsStartElement("Item", false))
                {
                    list.Add((Int16)reader.ReadElementContentAsInt("Item"));
                }
                reader.ReadEndElement("SwitchTable");
            }
            return list.ToArray();
        }

        private GXMacDirectTable[] LoadDirectTable(GXXmlReader reader)
        {
            List<GXMacDirectTable> list = new List<GXMacDirectTable>();
            if (reader.IsStartElement("DirectTable", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXMacDirectTable it = new GXMacDirectTable();
                    list.Add(it);
                    it.SourceSId = (Int16)reader.ReadElementContentAsInt("SourceSId");
                    it.SourceLnId = (Int16)reader.ReadElementContentAsInt("SourceLnId");
                    it.SourceLcId = (Int16)reader.ReadElementContentAsInt("SourceLcId");
                    it.DestinationSId = (Int16)reader.ReadElementContentAsInt("DestinationSId");
                    it.DestinationLnId = (Int16)reader.ReadElementContentAsInt("DestinationLnId");
                    it.DestinationLcId = (Int16)reader.ReadElementContentAsInt("DestinationLcId");
                    it.Did = GXCommon.HexToBytes(reader.ReadElementContentAsString("Did"));
                }
                reader.ReadEndElement("DirectTable");
            }
            return list.ToArray();
        }

        private GXMacAvailableSwitch[] LoadAvailableSwitches(GXXmlReader reader)
        {
            List<GXMacAvailableSwitch> list = new List<GXMacAvailableSwitch>();
            if (reader.IsStartElement("AvailableSwitches", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXMacAvailableSwitch it = new GXMacAvailableSwitch();
                    list.Add(it);
                    it.Sna = GXCommon.HexToBytes(reader.ReadElementContentAsString("Sna"));
                    it.LsId = reader.ReadElementContentAsInt("LsId");
                    it.Level = (Int16)reader.ReadElementContentAsInt("Level");
                    it.RxLevel = (Int16)reader.ReadElementContentAsInt("RxLevel");
                    it.RxSnr = (Int16)reader.ReadElementContentAsInt("RxSnr");
                }
                reader.ReadEndElement("AvailableSwitches");
            }
            return list.ToArray();
        }

        private GXMacPhyCommunication[] LoadCommunications(GXXmlReader reader)
        {
            List<GXMacPhyCommunication> list = new List<GXMacPhyCommunication>();
            if (reader.IsStartElement("Communications", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXMacPhyCommunication it = new GXMacPhyCommunication();
                    list.Add(it);
                    it.Eui = GXCommon.HexToBytes(reader.ReadElementContentAsString("Eui"));
                    it.TxPower = (Int16)reader.ReadElementContentAsInt("TxPower");
                    it.TxCoding = (Int16)reader.ReadElementContentAsInt("TxCoding");
                    it.RxCoding = (Int16)reader.ReadElementContentAsInt("RxCoding");
                    it.RxLvl = (Int16)reader.ReadElementContentAsInt("RxLvl");
                    it.Snr = (Int16)reader.ReadElementContentAsInt("Snr");
                    it.TxPowerModified = (Int16)reader.ReadElementContentAsInt("TxPowerModified");
                    it.TxCodingModified = (Int16)reader.ReadElementContentAsInt("TxCodingModified");
                    it.RxCodingModified = (Int16)reader.ReadElementContentAsInt("RxCodingModified");
                }
                reader.ReadEndElement("Communications");
            }
            return list.ToArray();
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            MulticastEntries = LoadMulticastEntries(reader);
            SwitchTable = LoadSwitchTable(reader);
            DirectTable = LoadDirectTable(reader);
            AvailableSwitches = LoadAvailableSwitches(reader);
            Communications = LoadCommunications(reader);
        }

        private void SaveMulticastEntries(GXXmlWriter writer)
        {
            writer.WriteStartElement("MulticastEntries");
            if (MulticastEntries != null)
            {
                foreach (GXMacMulticastEntry it in MulticastEntries)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Id", it.Id);
                    writer.WriteElementString("Members", it.Members);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }


        private void SaveSwitchTable(GXXmlWriter writer)
        {
            writer.WriteStartElement("SwitchTable");
            if (SwitchTable != null)
            {
                foreach (Int16 it in SwitchTable)
                {
                    writer.WriteElementString("Item", it);
                }
            }
            writer.WriteEndElement();
        }

        private void SaveDirectTable(GXXmlWriter writer)
        {
            writer.WriteStartElement("DirectTable");
            if (DirectTable != null)
            {
                foreach (GXMacDirectTable it in DirectTable)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("SourceSId", it.SourceSId);
                    writer.WriteElementString("SourceLnId", it.SourceLnId);
                    writer.WriteElementString("SourceLcId", it.SourceLcId);
                    writer.WriteElementString("DestinationSId", it.DestinationSId);
                    writer.WriteElementString("DestinationLnId", it.DestinationLnId);
                    writer.WriteElementString("DestinationLcId", it.DestinationLcId);
                    writer.WriteElementString("Did", GXCommon.ToHex(it.Did, false));
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        private void SaveAvailableSwitches(GXXmlWriter writer)
        {
            writer.WriteStartElement("AvailableSwitches");
            if (AvailableSwitches != null)
            {
                foreach (GXMacAvailableSwitch it in AvailableSwitches)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Sna", GXCommon.ToHex(it.Sna, false));
                    writer.WriteElementString("LsId", it.LsId);
                    writer.WriteElementString("Level", it.Level);
                    writer.WriteElementString("RxLevel", it.RxLevel);
                    writer.WriteElementString("RxSnr", it.RxSnr);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        private void SaveCommunications(GXXmlWriter writer)
        {
            writer.WriteStartElement("Communications");
            if (Communications != null)
            {
                foreach (GXMacPhyCommunication it in Communications)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Eui", GXCommon.ToHex(it.Eui, false));
                    writer.WriteElementString("TxPower", it.TxPower);
                    writer.WriteElementString("TxCoding", it.TxCoding);
                    writer.WriteElementString("RxCoding", it.RxCoding);
                    writer.WriteElementString("RxLvl", it.RxLvl);
                    writer.WriteElementString("Snr", it.Snr);
                    writer.WriteElementString("TxPowerModified", it.TxPowerModified);
                    writer.WriteElementString("TxCodingModified", it.TxCodingModified);
                    writer.WriteElementString("RxCodingModified", it.RxCodingModified);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            SaveMulticastEntries(writer);
            SaveSwitchTable(writer);
            SaveDirectTable(writer);
            SaveAvailableSwitches(writer);
            SaveCommunications(writer);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
