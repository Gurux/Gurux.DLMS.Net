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
using System.Text;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using System.Globalization;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSActivityCalendar
    /// </summary>
    public class GXDLMSActivityCalendar : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSActivityCalendar()
        : this("0.0.13.0.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSActivityCalendar(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSActivityCalendar(string ln, ushort sn)
        : base(ObjectType.ActivityCalendar, ln, sn)
        {
        }

        [XmlIgnore()]
        public string CalendarNameActive
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDLMSSeasonProfile[] SeasonProfileActive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        public GXDLMSWeekProfile[] WeekProfileTableActive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        public GXDLMSDayProfile[] DayProfileTableActive
        {
            get;
            set;
        }
        [XmlIgnore()]
        public string CalendarNamePassive
        {
            get;
            set;
        }
        [XmlIgnore()]
        public GXDLMSSeasonProfile[] SeasonProfilePassive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        public GXDLMSWeekProfile[] WeekProfileTablePassive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        public GXDLMSDayProfile[] DayProfileTablePassive
        {
            get;
            set;
        }

        /// <summary>
        /// Activate Passive Calendar Time.
        /// </summary>
        [XmlIgnore()]
        public GXDateTime Time
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, CalendarNameActive, SeasonProfileActive,
                              WeekProfileTableActive, DayProfileTableActive, CalendarNamePassive,
                              SeasonProfilePassive, WeekProfileTablePassive, DayProfileTablePassive, Time
                            };
        }

        /// <summary>
        /// This method copies all passive to the active.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] ActivatePassiveCalendar(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        #region IGXDLMSBase Members

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //CalendarNameActive
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //SeasonProfileActive
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }

            //WeekProfileTableActive
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //DayProfileTableActive
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //CalendarNamePassive
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //SeasonProfilePassive
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //WeekProfileTablePassive
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //DayProfileTablePassive
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //Time.
            if (all || CanRead(10))
            {
                attributes.Add(10);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Internal.GXCommon.GetLogicalNameString(), "Active Calendar Name ", "Active Season Profile", "Active Week Profile Table",
                             "Active Day Profile Table", "Passive Calendar Name", "Passive Season Profile", "Passive Week Profile Table", "Passive Day Profile Table", "Time"
                            };

        }
        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Activate passive calendar" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 10;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

        private bool IsSec()
        {
            if (Parent != null && Parent.Parent is GXDLMSClient)
            {
                return (Parent.Parent as GXDLMSClient).Standard == Standard.SaudiArabia;
            }
            return false;
        }

        public override DataType GetUIDataType(int index)
        {
            if (index == 10)
            {
                return DataType.DateTime;
            }
            return base.GetUIDataType(index);
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
                return DataType.OctetString;
            }
            if (index == 3)
            {
                return DataType.Array;
            }
            if (index == 4)
            {
                return DataType.Array;
            }
            if (index == 5)
            {
                return DataType.Array;
            }
            if (index == 6)
            {
                return DataType.OctetString;
            }
            //
            if (index == 7)
            {
                return DataType.Array;
            }
            if (index == 8)
            {
                return DataType.Array;
            }
            if (index == 9)
            {
                return DataType.Array;
            }
            if (index == 10)
            {
                if (IsSec())
                {
                    return DataType.DateTime;
                }
                return DataType.OctetString;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        /// <summary>
        /// Get season profile bytes.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="target">Season profile array.</param>
        /// <param name="useOctetString">Is date time send as octet string.</param>
        /// <returns></returns>
        static Object GetSeasonProfile(GXDLMSSettings settings, GXDLMSSeasonProfile[] target, bool useOctetString)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Array);
            if (target == null)
            {
                //Add count
                GXCommon.SetObjectCount(0, data);
            }
            else
            {
                int cnt = target.Length;
                //Add count
                GXCommon.SetObjectCount(cnt, data);
                foreach (GXDLMSSeasonProfile it in target)
                {
                    data.SetUInt8((byte)DataType.Structure);
                    data.SetUInt8(3);
                    GXCommon.SetData(settings, data, DataType.OctetString, it.Name);
                    if (useOctetString)
                    {
                        GXCommon.SetData(settings, data, DataType.OctetString, it.Start);
                    }
                    else
                    {
                        GXCommon.SetData(settings, data, DataType.DateTime, it.Start);
                    }
                    GXCommon.SetData(settings, data, DataType.OctetString, it.WeekName);
                }
            }
            return data.Array();
        }

        static byte[] GetWeekProfileTable(GXDLMSSettings settings, GXDLMSWeekProfile[] target)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Array);
            if (target == null)
            {
                //Add count
                GXCommon.SetObjectCount(0, data);
            }
            else
            {
                int cnt = target.Length;
                //Add count
                GXCommon.SetObjectCount(cnt, data);
                foreach (GXDLMSWeekProfile it in target)
                {
                    data.SetUInt8((byte)DataType.Structure);
                    data.SetUInt8(8);
                    GXCommon.SetData(settings, data, DataType.OctetString, it.Name);
                    GXCommon.SetData(settings, data, DataType.UInt8, it.Monday);
                    GXCommon.SetData(settings, data, DataType.UInt8, it.Tuesday);
                    GXCommon.SetData(settings, data, DataType.UInt8, it.Wednesday);
                    GXCommon.SetData(settings, data, DataType.UInt8, it.Thursday);
                    GXCommon.SetData(settings, data, DataType.UInt8, it.Friday);
                    GXCommon.SetData(settings, data, DataType.UInt8, it.Saturday);
                    GXCommon.SetData(settings, data, DataType.UInt8, it.Sunday);
                }
            }
            return data.Array();
        }

        static Object GetDayProfileTable(GXDLMSSettings settings, GXDLMSDayProfile[] target)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Array);
            if (target == null)
            {
                //Add count
                GXCommon.SetObjectCount(0, data);
            }
            else
            {
                int cnt = target.Length;
                //Add count
                GXCommon.SetObjectCount(cnt, data);
                foreach (GXDLMSDayProfile it in target)
                {
                    data.SetUInt8((byte)DataType.Structure);
                    data.SetUInt8(2);
                    GXCommon.SetData(settings, data, DataType.UInt8, it.DayId);
                    data.SetUInt8((byte)DataType.Array);
                    //Add count
                    if (it.DaySchedules == null)
                    {
                        data.SetUInt8(0);
                    }
                    else
                    {
                        GXCommon.SetObjectCount(it.DaySchedules.Length, data);
                        foreach (GXDLMSDayProfileAction action in it.DaySchedules)
                        {
                            data.SetUInt8((byte)DataType.Structure);
                            data.SetUInt8(3);
                            GXCommon.SetData(settings, data, DataType.OctetString, action.StartTime);
                            GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(action.ScriptLogicalName));
                            GXCommon.SetData(settings, data, DataType.UInt16, action.ScriptSelector);
                        }
                    }
                }
            }
            return data.Array();

        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                if (CalendarNameActive == null)
                {
                    return null;
                }
                if (IsSec())
                {
                    return GXCommon.HexToBytes(CalendarNameActive);
                }
                return ASCIIEncoding.ASCII.GetBytes(CalendarNameActive);
            }
            if (e.Index == 3)
            {
                e.ByteArray = true;
                bool useOctetString = settings.Standard != Standard.SaudiArabia;
                return GetSeasonProfile(settings, SeasonProfileActive, useOctetString);
            }
            if (e.Index == 4)
            {
                e.ByteArray = true;
                return GetWeekProfileTable(settings, WeekProfileTableActive);
            }
            if (e.Index == 5)
            {
                e.ByteArray = true;
                return GetDayProfileTable(settings, DayProfileTableActive);
            }
            if (e.Index == 6)
            {
                if (CalendarNamePassive == null)
                {
                    return null;
                }
                if (IsSec())
                {
                    return GXCommon.HexToBytes(CalendarNamePassive);
                }
                return ASCIIEncoding.ASCII.GetBytes(CalendarNamePassive);
            }
            if (e.Index == 7)
            {
                e.ByteArray = true;
                bool useOctetString = settings.Standard != Standard.SaudiArabia;
                return GetSeasonProfile(settings, SeasonProfilePassive, useOctetString);
            }
            if (e.Index == 8)
            {
                e.ByteArray = true;
                return GetWeekProfileTable(settings, WeekProfileTablePassive);
            }
            if (e.Index == 9)
            {
                e.ByteArray = true;
                return GetDayProfileTable(settings, DayProfileTablePassive);
            }
            if (e.Index == 10)
            {
                return Time;
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        static GXDLMSSeasonProfile[] SetSeasonProfile(GXDLMSSettings settings, Object value)
        {
            if (value != null)
            {
                List<GXDLMSSeasonProfile> items = new List<GXDLMSSeasonProfile>();
                List<object> item;
                foreach (object tmp in (IEnumerable<object>)value)
                {
                    if (tmp is List<object>)
                    {
                        item = (List<object>)tmp;
                    }
                    else
                    {
                        item = new List<object>((object[])tmp);
                    }
                    GXDLMSSeasonProfile it = new GXDLMSSeasonProfile();
                    it.Name = (byte[])item[0];
                    if (item[1] is byte[])
                    {
                        it.Start = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[1], DataType.DateTime, settings == null ? false : settings.UseUtc2NormalTime);
                    }
                    else if (item[1] is GXDateTime)
                    {
                        it.Start = (GXDateTime)item[1];
                    }
                    else
                    {
                        throw new Exception("Invalid date time.");
                    }
                    it.WeekName = (byte[])item[2];
                    items.Add(it);
                }
                return items.ToArray();
            }
            return null;
        }

        static GXDLMSWeekProfile[] SetWeekProfileTable(GXDLMSSettings settings, Object value)
        {
            if (value != null)
            {
                List<GXDLMSWeekProfile> items = new List<GXDLMSWeekProfile>();
                List<object> arr, item;
                if (value is List<object>)
                {
                    arr = (List<object>)value;
                }
                else
                {
                    arr = new List<object>((object[])value);
                }
                foreach (object tmp in arr)
                {
                    if (tmp is List<object>)
                    {
                        item = (List<object>)tmp;
                    }
                    else
                    {
                        item = new List<object>((object[])tmp);
                    }
                    GXDLMSWeekProfile it = new GXDLMSWeekProfile();
                    it.Name = (byte[])item[0];
                    it.Monday = Convert.ToInt32(item[1]);
                    it.Tuesday = Convert.ToInt32(item[2]);
                    it.Wednesday = Convert.ToInt32(item[3]);
                    it.Thursday = Convert.ToInt32(item[4]);
                    it.Friday = Convert.ToInt32(item[5]);
                    it.Saturday = Convert.ToInt32(item[6]);
                    it.Sunday = Convert.ToInt32(item[7]);
                    items.Add(it);
                }
                return items.ToArray();
            }
            return null;
        }

        static GXDLMSDayProfile[] SetDayProfileTable(GXDLMSSettings settings, Object value)
        {
            if (value != null)
            {
                List<object> arr, item, it2;
                if (value is List<object>)
                {
                    arr = (List<object>)value;
                }
                else
                {
                    arr = new List<object>((object[])value);
                }
                List<GXDLMSDayProfile> items = new List<GXDLMSDayProfile>();
                foreach (object tmp in arr)
                {
                    if (tmp is List<object>)
                    {
                        item = (List<object>)tmp;
                    }
                    else
                    {
                        item = new List<object>((object[])tmp);
                    }
                    GXDLMSDayProfile it = new GXDLMSDayProfile();
                    it.DayId = Convert.ToInt32(item[0]);
                    List<GXDLMSDayProfileAction> actions = new List<GXDLMSDayProfileAction>();
                    if (item[1] is List<object>)
                    {
                        item = (List<object>)item[1];
                    }
                    else
                    {
                        item = new List<object>((object[])item[1]);
                    }
                    foreach (object tmp2 in item)
                    {
                        if (tmp2 is List<object>)
                        {
                            it2 = (List<object>)tmp2;
                        }
                        else
                        {
                            it2 = new List<object>((object[])tmp2);
                        }
                        GXDLMSDayProfileAction ac = new GXDLMSDayProfileAction();
                        if (it2[0] is GXTime)
                        {
                            ac.StartTime = (GXTime)it2[0];
                        }
                        else if (it2[0] is GXDateTime)
                        {
                            ac.StartTime = new GXTime((GXDateTime)it2[0]);
                        }
                        else
                        {
                            ac.StartTime = (GXTime)GXDLMSClient.ChangeType((byte[])it2[0], DataType.Time, settings.UseUtc2NormalTime);
                        }
                        ac.ScriptLogicalName = GXCommon.ToLogicalName(it2[1]);
                        ac.ScriptSelector = Convert.ToUInt16(it2[2]);
                        actions.Add(ac);
                    }
                    it.DaySchedules = actions.ToArray();
                    items.Add(it);
                }
                return items.ToArray();
            }
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
                if (e.Value is byte[] v)
                {
                    if (IsSec() || !GXByteBuffer.IsAsciiString(v))
                    {
                        CalendarNameActive = GXCommon.ToHex(v, false);
                    }
                    else
                    {
                        CalendarNameActive = ASCIIEncoding.ASCII.GetString(v);
                    }
                }
                else
                {
                    CalendarNameActive = Convert.ToString(e.Value);
                }
            }
            else if (e.Index == 3)
            {
                SeasonProfileActive = SetSeasonProfile(settings, e.Value);
            }
            else if (e.Index == 4)
            {
                WeekProfileTableActive = SetWeekProfileTable(settings, e.Value);
            }
            else if (e.Index == 5)
            {
                DayProfileTableActive = SetDayProfileTable(settings, e.Value);
            }
            else if (e.Index == 6)
            {
                if (e.Value is byte[] v)
                {
                    if (IsSec() || !GXByteBuffer.IsAsciiString(v))
                    {
                        CalendarNamePassive = GXCommon.ToHex(v, false);
                    }
                    else
                    {
                        CalendarNamePassive = ASCIIEncoding.ASCII.GetString(v);
                    }
                }
                else
                {
                    CalendarNamePassive = Convert.ToString(e.Value);
                }
            }
            else if (e.Index == 7)
            {
                SeasonProfilePassive = SetSeasonProfile(settings, e.Value);
            }
            else if (e.Index == 8)
            {
                WeekProfileTablePassive = SetWeekProfileTable(settings, e.Value);
            }
            else if (e.Index == 9)
            {
                DayProfileTablePassive = SetDayProfileTable(settings, e.Value);
            }
            else if (e.Index == 10)
            {
                if (e.Value is byte[])
                {
                    Time = (GXDateTime)GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime, settings.UseUtc2NormalTime);
                }
                else if (e.Value is GXDateTime)
                {
                    Time = (GXDateTime)e.Value;
                }
                else
                {
                    Time = new GXDateTime(Convert.ToDateTime(e.Value));
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        private static GXDLMSSeasonProfile[] LoadSeasonProfile(GXXmlReader reader, string name)
        {
            List<GXDLMSSeasonProfile> list = new List<GXDLMSSeasonProfile>();
            if (reader.IsStartElement(name, true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSSeasonProfile it = new GXDLMSSeasonProfile();
                    it.Name = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("Name"));
                    it.Start = new GXDateTime(reader.ReadElementContentAsString("Start"), CultureInfo.InvariantCulture);
                    it.WeekName = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("WeekName"));
                    list.Add(it);
                }
                reader.ReadEndElement(name);
            }
            return list.ToArray();
        }

        private static GXDLMSWeekProfile[] LoadWeekProfileTable(GXXmlReader reader, string name)
        {
            List<GXDLMSWeekProfile> list = new List<GXDLMSWeekProfile>();
            if (reader.IsStartElement(name, true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSWeekProfile it = new GXDLMSWeekProfile();
                    it.Name = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("Name"));
                    it.Monday = reader.ReadElementContentAsInt("Monday");
                    it.Tuesday = reader.ReadElementContentAsInt("Tuesday");
                    it.Wednesday = reader.ReadElementContentAsInt("Wednesday");
                    it.Thursday = reader.ReadElementContentAsInt("Thursday");
                    it.Friday = reader.ReadElementContentAsInt("Friday");
                    it.Saturday = reader.ReadElementContentAsInt("Saturday");
                    it.Sunday = reader.ReadElementContentAsInt("Sunday");
                    list.Add(it);
                }
                reader.ReadEndElement(name);
            }
            return list.ToArray();
        }

        private static GXDLMSDayProfile[] LoadDayProfileTable(GXXmlReader reader, string name)
        {
            List<GXDLMSDayProfile> list = new List<GXDLMSDayProfile>();
            if (reader.IsStartElement(name, true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSDayProfile it = new GXDLMSDayProfile();
                    it.DayId = reader.ReadElementContentAsInt("DayId");
                    list.Add(it);
                    List<GXDLMSDayProfileAction> actions = new List<GXDLMSDayProfileAction>();
                    if (reader.IsStartElement("Actions", true))
                    {
                        while (reader.IsStartElement("Action", true))
                        {
                            GXDLMSDayProfileAction d = new GXDLMSDayProfileAction();
                            actions.Add(d);
                            d.StartTime = new GXTime(reader.ReadElementContentAsString("Start"), CultureInfo.InvariantCulture);
                            d.ScriptLogicalName = reader.ReadElementContentAsString("LN");
                            d.ScriptSelector = (UInt16)reader.ReadElementContentAsInt("Selector");
                        }
                        reader.ReadEndElement("Actions");
                    }
                    it.DaySchedules = actions.ToArray();
                }
                reader.ReadEndElement(name);
            }
            return list.ToArray();
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            CalendarNameActive = reader.ReadElementContentAsString("CalendarNameActive");
            SeasonProfileActive = LoadSeasonProfile(reader, "SeasonProfileActive");
            WeekProfileTableActive = LoadWeekProfileTable(reader, "WeekProfileTableActive");
            DayProfileTableActive = LoadDayProfileTable(reader, "DayProfileTableActive");
            CalendarNamePassive = reader.ReadElementContentAsString("CalendarNamePassive");
            SeasonProfilePassive = LoadSeasonProfile(reader, "SeasonProfilePassive");
            WeekProfileTablePassive = LoadWeekProfileTable(reader, "WeekProfileTablePassive");
            DayProfileTablePassive = LoadDayProfileTable(reader, "DayProfileTablePassive");
            string str = reader.ReadElementContentAsString("Time");
            if (!string.IsNullOrEmpty(str))
            {
                Time = new GXDateTime(str, CultureInfo.InvariantCulture);
            }
        }

        private void SaveSeasonProfile(GXXmlWriter writer, GXDLMSSeasonProfile[] list, string name, int index)
        {
            if (list != null)
            {
                writer.WriteStartElement(name, index);
                foreach (GXDLMSSeasonProfile it in list)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("Name", GXDLMSTranslator.ToHex(it.Name), 0);
                    //Some meters are returning time here, not date-time.
                    writer.WriteElementString("Start", new GXDateTime(it.Start), 0);
                    writer.WriteElementString("WeekName", GXDLMSTranslator.ToHex(it.WeekName), 0);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void SaveWeekProfileTable(GXXmlWriter writer, GXDLMSWeekProfile[] list, string name, int index)
        {
            if (list != null)
            {
                writer.WriteStartElement(name, index);
                foreach (GXDLMSWeekProfile it in list)
                {
                    writer.WriteStartElement("Item", index);
                    writer.WriteElementString("Name", GXDLMSTranslator.ToHex(it.Name), index);
                    writer.WriteElementString("Monday", it.Monday, index);
                    writer.WriteElementString("Tuesday", it.Tuesday, index);
                    writer.WriteElementString("Wednesday", it.Wednesday, index);
                    writer.WriteElementString("Thursday", it.Thursday, index);
                    writer.WriteElementString("Friday", it.Friday, index);
                    writer.WriteElementString("Saturday", it.Saturday, index);
                    writer.WriteElementString("Sunday", it.Sunday, index);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void SaveDayProfileTable(GXXmlWriter writer, GXDLMSDayProfile[] list, string name, int index)
        {
            if (list != null)
            {
                writer.WriteStartElement(name, index);
                foreach (GXDLMSDayProfile it in list)
                {
                    writer.WriteStartElement("Item", index);
                    writer.WriteElementString("DayId", it.DayId, index);
                    writer.WriteStartElement("Actions", index);
                    foreach (GXDLMSDayProfileAction d in it.DaySchedules)
                    {
                        writer.WriteStartElement("Action", index);
                        writer.WriteElementString("Start", d.StartTime, index);
                        writer.WriteElementString("LN", d.ScriptLogicalName, index);
                        writer.WriteElementString("Selector", d.ScriptSelector, index);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("CalendarNameActive", CalendarNameActive, 2);
            SaveSeasonProfile(writer, SeasonProfileActive, "SeasonProfileActive", 3);
            SaveWeekProfileTable(writer, WeekProfileTableActive, "WeekProfileTableActive", 4);
            SaveDayProfileTable(writer, DayProfileTableActive, "DayProfileTableActive", 5);
            writer.WriteElementString("CalendarNamePassive", CalendarNamePassive, 6);
            SaveSeasonProfile(writer, SeasonProfilePassive, "SeasonProfilePassive", 7);
            SaveWeekProfileTable(writer, WeekProfileTablePassive, "WeekProfileTablePassive", 8);
            SaveDayProfileTable(writer, DayProfileTablePassive, "DayProfileTablePassive", 9);
            writer.WriteElementString("Time", Time, 10);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}