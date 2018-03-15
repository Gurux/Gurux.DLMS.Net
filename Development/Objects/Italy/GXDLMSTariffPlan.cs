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
using Gurux.DLMS.Internal;
using System.Text;
using Gurux.DLMS.Objects.Italy.Enums;

namespace Gurux.DLMS.Objects.Italy
{
    /// <summary>
    /// Tariff Plan (Piano Tariffario) is used in Italian standard UNI/TS 11291-11.
    /// Online help:
    /// http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTariffPlan
    /// </summary>
    public class GXDLMSTariffPlan : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSTariffPlan()
        : this(null)
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


        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
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

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "CalendarName", "Enabled", "Plan", "ActivationTime" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
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

        private static void GetInterval(GXDLMSInterval interval, GXByteBuffer data)
        {
            data.SetUInt8((byte)DataType.Array);
            data.SetUInt8(5);
            byte b = interval.StartHour;
            b |= (byte)((byte)interval.IntervalTariff << 5);
            b |= (byte)((interval.UseInterval ? 1 : 0)<< 7);
            data.SetUInt8(b);
            data.SetUInt16((UInt16) interval.WeeklyActivation);
            UInt16 v = interval.SpecialDayMonth;
            v |= (UInt16)(interval.SpecialDay << 8);
            v |= (UInt16)((interval.SpecialDayEnabled ? 1 : 0) << 15);
            data.SetUInt16(v);
        }

        private static void GetSeason(GXBandDescriptor season, GXByteBuffer data)
        {
            GXCommon.SetData(null, data, DataType.UInt8, season.DayOfMonth);
            GXCommon.SetData(null, data, DataType.UInt8, season.Month);
            GetInterval(season.WorkingDayIntervals, data);
            GetInterval(season.SaturdayIntervals, data);
            GetInterval(season.HolidayIntervals, data);
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return CalendarName;
            }
            if (e.Index == 3)
            {
                return Enabled;
            }
            if (e.Index == 4)
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
                data.SetUInt8((byte)Plan.SpecialDays.Length);
                foreach (UInt16 it in Plan.SpecialDays)
                {
                    data.SetUInt8((byte)DataType.UInt16);
                    data.SetUInt16(it);
                }
            }
            if (e.Index == 5)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                if (ActivationTime == null)
                {
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    data.SetUInt8((byte)DataType.Structure);
                    //Count
                    data.SetUInt8(2);
                    //Time
                    GXCommon.SetData(settings, data, DataType.OctetString, new GXTime(ActivationTime));
                    //Date
                    GXCommon.SetData(settings, data, DataType.OctetString, new GXDate(ActivationTime));
                }
                return data.Array();
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        private static void UpdateInterval(GXDLMSInterval interval, byte[] value)
        {
            GXByteBuffer bb = new GXByteBuffer(value);
            byte b = bb.GetUInt8();
            interval.StartHour = (byte) (b & 0x1F);
            interval.IntervalTariff = (DefaultTariffBand)((b >> 5) & 0x3);
            interval.UseInterval = (b >> 7) != 0;
            interval.WeeklyActivation = (WeeklyActivation) bb.GetUInt16();
            UInt16 v = bb.GetUInt16();
            interval.SpecialDayMonth = (byte)(v & 0xF);
            interval.SpecialDay = (byte)((v >> 8) & 0xF);
            interval.SpecialDayEnabled = (v >> 15) != 0;
        }

        private static void UpdateSeason(GXBandDescriptor season, object[] value)
        {
            season.DayOfMonth = Convert.ToByte(value[0]);
            season.Month = Convert.ToByte(value[1]);
            UpdateInterval(season.WorkingDayIntervals, (byte[])value[2]);
            UpdateInterval(season.SaturdayIntervals, (byte[])value[3]);
            UpdateInterval(season.HolidayIntervals, (byte[])value[4]);
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                LogicalName = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 2)
            {
                CalendarName = ASCIIEncoding.ASCII.GetString((byte[])e.Value);
            }
            else if (e.Index == 3)
            {
                Enabled = Convert.ToBoolean(e.Value);
            }
            else if (e.Index == 4)
            {
                if (e.Value is object[])
                {
                    object[] it = e.Value as object[];
                    Plan.DefaultTariffBand = Convert.ToByte(it[0]);
                    UpdateSeason(Plan.WinterSeason, (it[1] as object[])[0] as object[]);
                    UpdateSeason(Plan.SummerSeason, (it[1] as object[])[1] as object[]);
                    Plan.WeeklyActivation = (string)it[2];
                    Plan.SpecialDays = (UInt16[])it[3];
                }
            }
            else if (e.Index == 5)
            {
                if (e.Value is object[])
                {
                    object[] it = e.Value as object[];
                    GXDateTime time = (GXDateTime)GXDLMSClient.ChangeType((byte[])it[0], DataType.Time, settings.UseUtc2NormalTime);
                    time.Skip &= ~(DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.DayOfWeek);
                    GXDateTime date = (GXDateTime)GXDLMSClient.ChangeType((byte[])it[1], DataType.Date, settings.UseUtc2NormalTime);
                    date.Skip &= ~(DateTimeSkips.Hour | DateTimeSkips.Minute | DateTimeSkips.Second | DateTimeSkips.Ms);
                    ActivationTime = new DLMS.GXDateTime(date);
                    ActivationTime.Value = ActivationTime.Value.AddHours(time.Value.Hour);
                    ActivationTime.Value = ActivationTime.Value.AddMinutes(time.Value.Minute);
                    ActivationTime.Value = ActivationTime.Value.AddSeconds(time.Value.Second);
                    ActivationTime.Skip = date.Skip | time.Skip;
                }
                else
                {
                    ActivationTime = new GXDateTime(DateTime.MinValue);
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            CalendarName = reader.ReadElementContentAsString("Name");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Name", CalendarName);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
