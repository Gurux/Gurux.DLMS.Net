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

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Entries };
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
            //Entries
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Entries" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 2;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 3;
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
                return DataType.Array;
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
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                data.SetUInt8((byte)Entries.Count);
                foreach (GXScheduleEntry it in Entries)
                {
                    data.SetUInt8((byte)DataType.Structure);
                    data.SetUInt8(10);
                    //Add index.
                    data.SetUInt8((byte)DataType.UInt16);
                    data.SetUInt16(it.Index);
                    //Add enable.
                    data.SetUInt8((byte)DataType.Boolean);
                    data.SetUInt8((byte) (it.Enable ? 1 : 0));
                    //Add logical Name.
                    data.SetUInt8((byte)DataType.OctetString);
                    data.SetUInt8((byte) it.LogicalName.Length);
                    data.Set(GXCommon.LogicalNameToBytes(it.LogicalName));
                    //Add script selector.
                    data.SetUInt8((byte)DataType.UInt16);
                    data.SetUInt16(it.ScriptSelector);
                    //Add switch time.
                    GXCommon.SetData(settings, data, DataType.OctetString, it.SwitchTime);
                    //Add validity window.
                    data.SetUInt8((byte)DataType.UInt16);
                    data.SetUInt16(it.ValidityWindow);
                    //Add exec week days.
                    GXCommon.SetData(settings, data, DataType.BitString, it.ExecWeekdays);
                    //Add exec spec days.
                    GXCommon.SetData(settings, data, DataType.BitString, it.ExecSpecDays);
                    //Add begin date.
                    GXCommon.SetData(settings, data, DataType.OctetString, it.BeginDate);
                    //Add end date.
                    GXCommon.SetData(settings, data, DataType.OctetString, it.EndDate);
                }
                return data.Array();
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
                Entries.Clear();
                List<object> arr = (List<object>)e.Value;
                if (arr != null)
                {
                    foreach (List<object> it in arr)
                    {
                        GXScheduleEntry item = new GXScheduleEntry();
                        item.Index = Convert.ToUInt16(it[0]);
                        item.Enable = (bool)it[1];
                        item.LogicalName = GXCommon.ToLogicalName(it[2]);
                        item.ScriptSelector = Convert.ToUInt16(it[3]);
                        item.SwitchTime = (GXDateTime)GXDLMSClient.ChangeType((byte[])it[4], DataType.DateTime, settings.UseUtc2NormalTime);
                        item.ValidityWindow = Convert.ToUInt16(it[5]);
                        item.ExecWeekdays = Convert.ToString(it[6]);
                        item.ExecSpecDays = Convert.ToString(it[7]);
                        item.BeginDate = (GXDateTime)GXDLMSClient.ChangeType((byte[])it[8], DataType.DateTime, settings.UseUtc2NormalTime);
                        item.EndDate = (GXDateTime)GXDLMSClient.ChangeType((byte[])it[9], DataType.DateTime, settings.UseUtc2NormalTime);
                        Entries.Add(item);
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
                    it.LogicalName = reader.ReadElementContentAsString("LogicalName");
                    it.ScriptSelector = (byte)reader.ReadElementContentAsInt("ScriptSelector");
                    it.SwitchTime = (GXDateTime)reader.ReadElementContentAsObject("SwitchTime", new GXDateTime(), null, 0);
                    it.ValidityWindow = (byte)reader.ReadElementContentAsInt("ValidityWindow");
                    it.ExecWeekdays = reader.ReadElementContentAsString("ExecWeekdays");
                    it.ExecSpecDays = reader.ReadElementContentAsString("ExecSpecDays");
                    it.BeginDate = (GXDateTime)reader.ReadElementContentAsObject("BeginDate", new GXDateTime(), null, 0);
                    it.EndDate = (GXDateTime)reader.ReadElementContentAsObject("EndDate", new GXDateTime(), null, 0);
                    Entries.Add(it);
                }
                reader.ReadEndElement("Entries");
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (Entries != null)
            {
                writer.WriteStartElement("Entries");
                foreach (GXScheduleEntry it in Entries)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Index", it.Index);
                    writer.WriteElementString("Enable", it.Enable);
                    writer.WriteElementString("LogicalName", it.LogicalName);
                    writer.WriteElementString("ScriptSelector", it.ScriptSelector);
                    writer.WriteElementString("SwitchTime", it.SwitchTime);
                    writer.WriteElementString("ValidityWindow", it.ValidityWindow);
                    writer.WriteElementString("ExecWeekdays", it.ExecWeekdays);
                    writer.WriteElementString("ExecSpecDays", it.ExecSpecDays);
                    writer.WriteElementString("BeginDate", it.BeginDate);
                    writer.WriteElementString("EndDate", it.EndDate);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();//Entries
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
