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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSSchedule
    /// </summary>
    public class GXDLMSSchedule : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSchedule()
        : this("0.0.12.0.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSSchedule(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSSchedule(string ln, ushort sn)
        : base(ObjectType.Schedule, ln, sn)
        {
            Entries = new List<GXScheduleEntry>();
        }

        /// <summary>
        /// Specifies the scripts to be executed at given times.
        /// </summary>
        [XmlIgnore()]
        public List<GXScheduleEntry> Entries
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Entries };
        }

        /// <summary>
        /// Add entry to entries list.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="entry">Schedule entry.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Insert(GXDLMSClient client, GXScheduleEntry entry)
        {
            GXByteBuffer data = new GXByteBuffer();
            AddEntry(client.Settings, entry, data);
            return client.Method(this, 2, data.Array(), DataType.Structure);
        }

        /// <summary>
        /// Remove entry from entries list.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="entry">Schedule entry.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Delete(GXDLMSClient client, GXScheduleEntry entry)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Structure);
            //Add structure size.
            data.SetUInt8(2);
            //firstIndex
            GXCommon.SetData(null, data, DataType.UInt16, entry.Index);
            //lastIndex
            GXCommon.SetData(null, data, DataType.UInt16, entry.Index);
            return client.Method(this, 3, data.Array(), DataType.Structure);
        }

        /// <summary>
        /// Enable entry from entries list.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="entry">Schedule entries.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Enable(GXDLMSClient client, GXScheduleEntry entry)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Structure);
            //Add structure size.
            data.SetUInt8(4);
            //firstIndex
            GXCommon.SetData(null, data, DataType.UInt16, 0);
            GXCommon.SetData(null, data, DataType.UInt16, 0);
            GXCommon.SetData(null, data, DataType.UInt16, entry.Index);
            //lastIndex
            GXCommon.SetData(null, data, DataType.UInt16, entry.Index);
            return client.Method(this, 1, data.Array(), DataType.Structure);
        }

        /// <summary>
        /// Disable entry from entries list.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="entry">Schedule entries.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Disable(GXDLMSClient client, GXScheduleEntry entry)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Structure);
            //Add structure size.
            data.SetUInt8(4);
            //firstIndex
            GXCommon.SetData(null, data, DataType.UInt16, entry.Index);
            //lastIndex
            GXCommon.SetData(null, data, DataType.UInt16, entry.Index);
            GXCommon.SetData(null, data, DataType.UInt16, 0);
            GXCommon.SetData(null, data, DataType.UInt16, 0);
            return client.Method(this, 1, data.Array(), DataType.Structure);
        }

        #region IGXDLMSBase Members

        private void RemoveEntry(UInt16 index)
        {
            foreach (GXScheduleEntry it in Entries)
            {
                if (it.Index == index)
                {
                    Entries.Remove(it);
                    break;
                }
            }
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                //Enable/disable entry
                case 1:
                    {
                        List<object> tmp = (List<object>)e.Parameters;
                        //Disable
                        for (int index = (UInt16)tmp[0]; index <= (UInt16)tmp[1]; ++index)
                        {
                            if (index != 0)
                            {
                                foreach (GXScheduleEntry it in Entries)
                                {
                                    if (it.Index == index)
                                    {
                                        it.Enable = false;
                                        break;
                                    }
                                }
                            }
                        }
                        //Enable
                        for (int index = (UInt16)tmp[2]; index <= (UInt16)tmp[3]; ++index)
                        {
                            if (index != 0)
                            {
                                foreach (GXScheduleEntry it in Entries)
                                {
                                    if (it.Index == index)
                                    {
                                        it.Enable = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
                //Insert entry
                case 2:
                    GXScheduleEntry entry = CreateEntry(settings, (List<object>)e.Parameters);
                    RemoveEntry(entry.Index);
                    Entries.Add(entry);
                    break;
                //Delete entry
                case 3:
                    {
                        List<object> tmp = (List<object>)e.Parameters;
                        for (UInt16 index = (UInt16)tmp[0]; index <= (UInt16)tmp[1]; ++index)
                        {
                            RemoveEntry(index);
                        }
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
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
            //Entries
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Entries" };
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Enable/disable", "Insert", "Delete" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 2;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 3;
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.Array;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        private void AddEntry(GXDLMSSettings settings, GXScheduleEntry it, GXByteBuffer data)
        {
            data.SetUInt8((byte)DataType.Structure);
            data.SetUInt8(10);
            //Add index.
            data.SetUInt8((byte)DataType.UInt16);
            data.SetUInt16(it.Index);
            //Add enable.
            data.SetUInt8((byte)DataType.Boolean);
            data.SetUInt8((byte)(it.Enable ? 1 : 0));
            //Add logical Name.
            data.SetUInt8((byte)DataType.OctetString);
            data.SetUInt8(6);
            if (it.Script == null)
            {
                data.Set(new byte[] { 0, 0, 0, 0, 0, 0 });
            }
            else
            {
                data.Set(GXCommon.LogicalNameToBytes(it.Script.LogicalName));
            }
            //Add script selector.
            data.SetUInt8((byte)DataType.UInt16);
            data.SetUInt16(it.ScriptSelector);
            //Add switch time.
            GXCommon.SetData(settings, data, DataType.OctetString, it.SwitchTime);
            //Add validity window.
            data.SetUInt8((byte)DataType.UInt16);
            data.SetUInt16(it.ValidityWindow);
            //Add exec week days.
            GXCommon.SetData(settings, data, DataType.BitString, GXBitString.ToBitString((byte)it.ExecWeekdays, 7));
            //Add exec spec days.
            GXCommon.SetData(settings, data, DataType.BitString, it.ExecSpecDays);
            //Add begin date.
            GXCommon.SetData(settings, data, DataType.OctetString, it.BeginDate);
            //Add end date.
            GXCommon.SetData(settings, data, DataType.OctetString, it.EndDate);
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                GXCommon.SetObjectCount(Entries.Count, data);
                foreach (GXScheduleEntry it in Entries)
                {
                    AddEntry(settings, it, data);
                }
                return data.Array();
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        /// <summary>
        /// Create a new entry.
        /// </summary>
        private GXScheduleEntry CreateEntry(GXDLMSSettings settings, List<object> it)
        {
            GXScheduleEntry item = new GXScheduleEntry();
            item.Index = Convert.ToUInt16(it[0]);
            item.Enable = (bool)it[1];
            string ln = GXCommon.ToLogicalName(it[2]);
            if (settings != null && ln != "0.0.0.0.0.0")
            {
                item.Script = (GXDLMSScriptTable)settings.Objects.FindByLN(ObjectType.ScriptTable, ln);
            }
            if (item.Script == null)
            {
                item.Script = new GXDLMSScriptTable(ln);
            }
            item.ScriptSelector = Convert.ToUInt16(it[3]);
            item.SwitchTime = (GXTime)GXDLMSClient.ChangeType((byte[])it[4], DataType.Time, settings.UseUtc2NormalTime);
            item.ValidityWindow = Convert.ToUInt16(it[5]);
            item.ExecWeekdays = (Weekdays)Convert.ToByte(it[6]);
            item.ExecSpecDays = Convert.ToString(it[7]);
            item.BeginDate = (GXDate)GXDLMSClient.ChangeType((byte[])it[8], DataType.Date, settings.UseUtc2NormalTime);
            item.EndDate = (GXDate)GXDLMSClient.ChangeType((byte[])it[9], DataType.Date, settings.UseUtc2NormalTime);
            return item;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                LogicalName = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 2)
            {
                Entries.Clear();
                IEnumerable<object> arr = (IEnumerable<object>)e.Value;
                if (arr != null)
                {
                    foreach (List<object> it in arr)
                    {
                        Entries.Add(CreateEntry(settings, it));
                    }
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Entries.Clear();
            if (reader.IsStartElement("Entries", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXScheduleEntry it = new GXScheduleEntry();
                    it.Index = (byte)reader.ReadElementContentAsInt("Index");
                    it.Enable = reader.ReadElementContentAsInt("Enable") != 0;
                    string ln = reader.ReadElementContentAsString("LogicalName");
                    if (!string.IsNullOrEmpty(ln))
                    {
                        it.Script = new GXDLMSScriptTable(ln);
                    }
                    it.ScriptSelector = (byte)reader.ReadElementContentAsInt("ScriptSelector");
                    it.SwitchTime = reader.ReadElementContentAsTime("SwitchTime");
                    it.ValidityWindow = (byte)reader.ReadElementContentAsInt("ValidityWindow");
                    it.ExecWeekdays = (Weekdays)reader.ReadElementContentAsInt("ExecWeekdays");
                    it.ExecSpecDays = reader.ReadElementContentAsString("ExecSpecDays");
                    it.BeginDate = reader.ReadElementContentAsDate("BeginDate");
                    it.EndDate = reader.ReadElementContentAsDate("EndDate");
                    Entries.Add(it);
                }
                reader.ReadEndElement("Entries");
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (Entries != null)
            {
                writer.WriteStartElement("Entries", 2);
                foreach (GXScheduleEntry it in Entries)
                {
                    writer.WriteStartElement("Item", 2);
                    writer.WriteElementString("Index", it.Index, 2);
                    writer.WriteElementString("Enable", it.Enable, 2);
                    if (it.Script != null)
                    {
                        writer.WriteElementString("LogicalName", it.Script.LogicalName, 2);
                    }
                    writer.WriteElementString("ScriptSelector", it.ScriptSelector, 2);
                    writer.WriteElementString("SwitchTime", it.SwitchTime, 2);
                    writer.WriteElementString("ValidityWindow", it.ValidityWindow, 2);
                    writer.WriteElementString("ExecWeekdays", (int)it.ExecWeekdays, 2);
                    writer.WriteElementString("ExecSpecDays", it.ExecSpecDays, 2);
                    writer.WriteElementString("BeginDate", it.BeginDate, 2);
                    writer.WriteElementString("EndDate", it.EndDate, 2);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();//Entries
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
            //Upload entries Value after load.
            if (Entries != null)
            {
                foreach (GXScheduleEntry it in Entries)
                {
                    GXDLMSScriptTable target = (GXDLMSScriptTable)reader.Objects.FindByLN(ObjectType.ScriptTable, it.Script.LogicalName);
                    if (target != null && target != it.Script)
                    {
                        it.Script = target;
                    }
                }
            }
        }

        #endregion
    }
}
