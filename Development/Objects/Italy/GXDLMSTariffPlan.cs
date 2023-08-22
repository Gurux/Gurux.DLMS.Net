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
using System.Text;
using Gurux.DLMS.Objects.Italy.Enums;
using System.Globalization;

namespace Gurux.DLMS.Objects.Italy
{
    /// <summary>
    /// Tariff Plan (Piano Tariffario) is used in Italian standard UNI/TS 11291-11.
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTariffPlan
    /// </summary>
    public class GXDLMSTariffPlan : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSTariffPlan()
        : this("0.0.94.39.21.101")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSTariffPlan(string ln)
        : base(ObjectType.TariffPlan, ln, 0)
        {
            Plan = new GXTariffPlan();
        }

        /// <summary>
        /// Calendar Name.
        /// </summary>
        [XmlIgnore()]
        public string CalendarName
        {
            get;
            set;
        }

        /// <summary>
        /// Is tariff plan enabled.
        /// </summary>
        [XmlIgnore()]
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Tariff plan.
        /// </summary>
        [XmlIgnore()]
        public GXTariffPlan Plan
        {
            get;
            set;
        }

        /// <summary>
        /// Activation date and time.
        /// </summary>
        [XmlIgnore()]
        public GXDateTime ActivationTime
        {
            get;
            set;
        }


        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, CalendarName, Enabled, Plan, ActivationTime };
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
            //CalendarName
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //Enabled
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //Plan
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //ActivationTime
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "CalendarName", "Enabled", "Plan", "ActivationTime" };
        }

        /// <inheritdoc />
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
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
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
                return DataType.OctetString;
            }
            if (index == 3)
            {
                return DataType.Boolean;
            }
            if (index == 4)
            {
                return DataType.Structure;
            }
            if (index == 5)
            {
                return DataType.Structure;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        private static void GetIntervals(GXDLMSInterval[] intervals, GXByteBuffer data)
        {
            data.SetUInt8((byte)DataType.Array);
            data.SetUInt8(5);
            foreach (GXDLMSInterval interval in intervals)
            {
                byte b = (byte)(interval.UseInterval ? 1 : 0);
                b |= (byte)((byte)interval.IntervalTariff << 1);
                b |= (byte)(interval.StartHour << 3);
                data.SetUInt8(b);
            }
        }

        private static void GetSeason(GXBandDescriptor season, GXByteBuffer data)
        {
            data.SetUInt8((byte)DataType.Array);
            data.SetUInt8(5);
            GXCommon.SetData(null, data, DataType.UInt8, season.DayOfMonth);
            GXCommon.SetData(null, data, DataType.UInt8, season.Month);
            GetIntervals(season.WorkingDayIntervals, data);
            GetIntervals(season.SaturdayIntervals, data);
            GetIntervals(season.HolidayIntervals, data);
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    if (CalendarName == null)
                    {
                        return null;
                    }
                    return ASCIIEncoding.ASCII.GetBytes(CalendarName);
                case 3:
                    return Enabled;
                case 4:
                    {
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(4);
                        data.SetUInt8((byte)DataType.UInt8);
                        data.SetUInt8(Plan.DefaultTariffBand);

                        data.SetUInt8((byte)DataType.Array);
                        data.SetUInt8(2);
                        GetSeason(Plan.WinterSeason, data);
                        GetSeason(Plan.SummerSeason, data);

                        GXCommon.SetData(null, data, DataType.BitString, Plan.WeeklyActivation);
                        data.SetUInt8((byte)DataType.Array);
                        if (Plan.SpecialDays == null)
                        {
                            data.SetUInt8(0);
                        }
                        else
                        {
                            data.SetUInt8((byte)Plan.SpecialDays.Length);
                            foreach (UInt16 it in Plan.SpecialDays)
                            {
                                data.SetUInt8((byte)DataType.UInt16);
                                data.SetUInt16(it);
                            }
                        }
                        return data.Array();
                    }

                case 5:
                    {
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Structure);
                        //Count
                        data.SetUInt8(2);
                        if (ActivationTime == null)
                        {
                            //Time
                            GXCommon.SetData(settings, data, DataType.OctetString, new GXTime());
                            //Date
                            GXCommon.SetData(settings, data, DataType.OctetString, new GXDate(DateTime.MinValue));
                        }
                        else
                        {
                            //Time
                            GXCommon.SetData(settings, data, DataType.OctetString, new GXTime(ActivationTime));
                            //Date
                            GXCommon.SetData(settings, data, DataType.OctetString, new GXDate(ActivationTime));
                        }
                        return data.Array();
                    }
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        private static void UpdateIntervals(GXDLMSInterval[] intervals, List<object> value)
        {
            int pos = 0;
            foreach (byte it in value)
            {
                GXDLMSInterval interval = intervals[pos];
                ++pos;
                interval.StartHour = (byte)(it >> 3);
                interval.IntervalTariff = (DefaultTariffBand)((it >> 1) & 0x3);
                interval.UseInterval = (it & 1) != 0;
            }
        }

        private static void UpdateSeason(GXBandDescriptor season, List<object> value)
        {
            if (value != null)
            {
                season.DayOfMonth = Convert.ToByte(value[0]);
                season.Month = Convert.ToByte(value[1]);
                UpdateIntervals(season.WorkingDayIntervals, (List<object>)value[2]);
                UpdateIntervals(season.SaturdayIntervals, (List<object>)value[3]);
                UpdateIntervals(season.HolidayIntervals, (List<object>)value[4]);
            }
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    if (e.Value is byte[])
                    {
                        if (GXByteBuffer.IsAsciiString((byte[])e.Value))
                        {
                            CalendarName = ASCIIEncoding.ASCII.GetString((byte[])e.Value);
                        }
                        else
                        {
                            CalendarName = GXCommon.ToHex((byte[])e.Value, true);
                        }
                    }
                    else
                    {
                        CalendarName = Convert.ToString(e.Value);
                    }
                    break;
                case 3:
                    Enabled = Convert.ToBoolean(e.Value);
                    break;
                case 4:
                    {
                        if (e.Value is List<object>)
                        {
                            List<object> it = e.Value as List<object>;
                            Plan.DefaultTariffBand = Convert.ToByte(it[0]);
                            UpdateSeason(Plan.WinterSeason, (it[1] as List<object>)[0] as List<object>);
                            UpdateSeason(Plan.SummerSeason, (it[1] as List<object>)[1] as List<object>);
                            Plan.WeeklyActivation = Convert.ToString(it[2]);
                            List<UInt16> days = new List<ushort>();
                            foreach (UInt16 v in (List<object>)it[3])
                            {
                                days.Add(v);
                            }
                            Plan.SpecialDays = days.ToArray();
                        }

                        break;
                    }

                case 5:
                    {
                        if (e.Value is List<object>)
                        {
                            List<object> it = e.Value as List<object>;
                            GXDateTime time = (GXDateTime)it[0];
                            time.Skip &= ~(DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.DayOfWeek);
                            GXDateTime date = (GXDateTime)it[1];
                            date.Skip &= ~(DateTimeSkips.Hour | DateTimeSkips.Minute | DateTimeSkips.Second | DateTimeSkips.Ms);
                            ActivationTime = new DLMS.GXDateTime(date);
                            ActivationTime.Value = ActivationTime.Value.AddHours(time.Value.Hour);
                            ActivationTime.Value = ActivationTime.Value.AddMinutes(time.Value.Minute);
                            ActivationTime.Value = ActivationTime.Value.AddSeconds(time.Value.Second);
                            ActivationTime.Skip = date.Skip | time.Skip;
                        }
                        else if (e.Value is string)
                        {
                            ActivationTime = new GXDateTime((string)e.Value);
                        }
                        else
                        {
                            ActivationTime = new GXDateTime(DateTime.MinValue);
                        }

                        break;
                    }

                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            CalendarName = reader.ReadElementContentAsString("Name");
            Enabled = reader.ReadElementContentAsInt("Enabled") != 0;
            string tmp = reader.ReadElementContentAsString("ActivationTime");
            {
                ActivationTime = new GXDateTime(tmp, CultureInfo.InvariantCulture);
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Name", CalendarName, 2);
            writer.WriteElementString("Enabled", Enabled, 3);
            writer.WriteElementString("ActivationTime", ActivationTime, 4);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
