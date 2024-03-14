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
using System.Globalization;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSClock
    /// </summary>
    public class GXDLMSClock : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSClock()
        : this("0.0.1.0.0.255", 0)
        {
        }

        public override DataType GetUIDataType(int index)
        {
            if (index == 2 || index == 5 || index == 6)
            {
                return DataType.DateTime;
            }
            return base.GetUIDataType(index);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSClock(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSClock(string ln, ushort sn)
        : base(ObjectType.Clock, ln, sn)
        {
            Time = new GXDateTime(DateTime.MinValue);
        }       

        /// <summary>
        /// Time of COSEM Clock object.
        /// </summary>
        [XmlIgnore()]
        public GXDateTime Time
        {
            get;
            set;
        }

        /// <summary>
        /// TimeZone of COSEM Clock object.
        /// </summary>
        [XmlIgnore()]
        public int TimeZone
        {
            get;
            set;
        }

        /// <summary>
        /// Status of COSEM Clock object.
        /// </summary>
        [XmlIgnore()]
        public ClockStatus Status
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDateTime Begin
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDateTime End
        {
            get;
            set;
        }

        [XmlIgnore()]
        public int Deviation
        {
            get;
            set;
        }

        /// <summary>
        /// Is summer time enabled.
        /// </summary>
        [XmlIgnore()]
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Clock base of COSEM Clock object.
        /// </summary>
        [XmlIgnore()]
        public ClockBase ClockBase
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Time, TimeZone, Status, Begin, End,
                              Deviation, Enabled, ClockBase
                            };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            DateTimeOffset tm = this.Time.Value;
            // Resets the value to the default value.
            // The default value is an instance specific constant.
            if (e.Index == 1)
            {
                int minutes = tm.Minute;
                if (minutes < 8)
                {
                    minutes = 0;
                }
                else if (minutes < 23)
                {
                    minutes = 15;
                }
                else if (minutes < 38)
                {
                    minutes = 30;
                }
                else if (minutes < 53)
                {
                    minutes = 45;
                }
                else
                {
                    minutes = 0;
                    tm = tm.AddHours(1);
                }
                tm = tm.AddMinutes(-tm.Minute + minutes);
                tm = tm.AddSeconds(-tm.Second);
                tm = tm.AddMilliseconds(-tm.Millisecond);
                this.Time.Value = tm;
            }
            // Sets the meter's time to the nearest minute.
            else if (e.Index == 3)
            {
                tm = this.Time.Value;
                int s = tm.Second;
                if (s > 30)
                {
                    tm = tm.AddMinutes(1);
                }
                tm = tm.AddSeconds(-tm.Second);
                tm = tm.AddMilliseconds(-tm.Millisecond);
                this.Time.Value = tm;
            }
            // Presets the time to a new value (preset_time) and defines
            // avalidity_interval within which the new time can be activated.
            else if (e.Index == 5)
            {
                GXDateTime presetTime = (GXDateTime)GXDLMSClient.ChangeType((byte[])((List<object>)e.Parameters)[0], DataType.DateTime, settings.UseUtc2NormalTime);
                GXDateTime validityIntervalStart = (GXDateTime)GXDLMSClient.ChangeType((byte[])((List<object>)e.Parameters)[1], DataType.DateTime, settings.UseUtc2NormalTime);
                GXDateTime validityIntervalEnd = (GXDateTime)GXDLMSClient.ChangeType((byte[])((List<object>)e.Parameters)[2], DataType.DateTime, settings.UseUtc2NormalTime);
                this.Time.Value = presetTime.Value;
            }
            // Shifts the time.
            else if (e.Index == 6)
            {
                int shift = Convert.ToInt32(e.Parameters);
                tm = tm.AddSeconds(shift);
                this.Time.Value = tm;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            return null;
        }

        /// <summary>
        /// Sets the meter's time to the nearest (+/-) quarter of an hour value (*:00, *:15, *:30, *:45).
        /// </summary>
        /// <returns></returns>
        public byte[][] AdjustToQuarter(GXDLMSClient client)
        {
            return client.Method(this, 1, 0, DataType.Int8);
        }


        /// <summary>
        /// Sets the meter's time to the nearest (+/-) starting point of a measuring period.
        /// </summary>
        /// <returns></returns>
        public byte[][] AdjustToMeasuringPeriod(GXDLMSClient client)
        {
            return client.Method(this, 2, 0, DataType.Int8);
        }

        /// <summary>
        /// Sets the meter's time to the nearest minute.
        /// If second_counter lower 30 s, so second_counter is set to 0.
        /// If second_counter higher 30 s, so second_counter is set to 0, and
        /// minute_counter and all depending clock values are incremented if necessary.
        /// </summary>
        /// <returns></returns>
        public byte[][] AdjustToMinute(GXDLMSClient client)
        {
            return client.Method(this, 3, 0, DataType.Int8);
        }

        /// <summary>
        /// This Method is used in conjunction with the preset_adjusting_time
        /// Method. If the meter's time lies between validity_interval_start and
        /// validity_interval_end, then time is set to preset_time.
        /// </summary>
        /// <returns></returns>
        public byte[][] AdjustToPresetTime(GXDLMSClient client)
        {
            return client.Method(this, 4, 0, DataType.Int8);
        }

        /// <summary>
        /// Presets the time to a new value (preset_time) and defines a validity_interval within which the new time can be activated.
        /// </summary>
        /// <param name="presetTime"></param>
        /// <param name="validityIntervalStart"></param>
        /// <param name="validityIntervalEnd"></param>
        /// <returns></returns>
        public byte[][] PresetAdjustingTime(GXDLMSClient client, DateTime presetTime, DateTime validityIntervalStart, DateTime validityIntervalEnd)
        {
            GXByteBuffer buff = new GXByteBuffer();
            buff.Add((byte)DataType.Structure);
            buff.Add((byte)3);
            GXCommon.SetData(client.Settings, buff, DataType.OctetString, presetTime);
            GXCommon.SetData(client.Settings, buff, DataType.OctetString, validityIntervalStart);
            GXCommon.SetData(client.Settings, buff, DataType.OctetString, validityIntervalEnd);
            return client.Method(this, 5, buff.Array(), DataType.Array);
        }

        /// <summary>
        /// Shifts the time by n (-900 &lt;= n &lt;= 900) s.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public byte[][] ShiftTime(GXDLMSClient client, int time)
        {
            if (time < -900 || time > 900)
            {
                throw new ArgumentOutOfRangeException("Invalid shift time.");
            }
            return client.Method(this, 6, time, DataType.Int16);
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //Time
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //TimeZone
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //Status
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //Begin
            if (all || !base.IsRead(5))
            {
                attributes.Add(5);
            }
            //End
            if (all || !base.IsRead(6))
            {
                attributes.Add(6);
            }
            //Deviation
            if (all || !base.IsRead(7))
            {
                attributes.Add(7);
            }
            //Enabled
            if (all || !base.IsRead(8))
            {
                attributes.Add(8);
            }
            //ClockBase
            if (all || !base.IsRead(9))
            {
                attributes.Add(9);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(),
                              "Time",
                              "Time Zone",
                              "Status",
                              "Begin",
                              "End",
                              "Deviation",
                              "Enabled",
                              "Clock Base"
                            };
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] {"Adjust to quarter", "Adjust to measuring period",
                "Adjust to minute", "Adjust to preset time", "Preset adjusting time", "Shift time" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 9;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 6;
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
                return DataType.Int16;
            }
            if (index == 4)
            {
                return DataType.UInt8;
            }
            if (index == 5)
            {
                return DataType.OctetString;
            }
            if (index == 6)
            {
                return DataType.OctetString;
            }
            if (index == 7)
            {
                return DataType.Int8;
            }
            if (index == 8)
            {
                return DataType.Boolean;
            }
            if (index == 9)
            {
                return DataType.Enum;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        /// <summary>
        /// Return current time.
        /// </summary>
        /// <returns>Current time</returns>
        public GXDateTime Now()
        {
            return GetTime(DateTime.Now, false);
        }

        /// <summary>
        /// Return current time.
        /// </summary>
        ///<param name="useUtc2NormalTime">Is UTC time used.</param>
        /// <returns>Current time</returns>
        public GXDateTime Now(bool useUtc2NormalTime)
        {
            return GetTime(DateTime.Now, useUtc2NormalTime);
        }

        /// <summary>
        /// Returns time using clock settings.
        /// </summary>
        /// <param name="time"></param>
        /// <returns>Current time</returns>
        public GXDateTime GetTime(DateTime time)
        {
            return GetTime(time, false);
        }

        /// <summary>
        /// Returns time using clock settings.
        /// </summary>
        /// <param name="time"></param>
        ///<param name="useUtc2NormalTime">Is UTC time used.</param>
        /// <returns>Current time</returns>
        public GXDateTime GetTime(DateTime time, bool useUtc2NormalTime)
        {
            GXDateTime tm = new GXDateTime(time);
            //-32768 == 0x8000
            if (TimeZone == -1 || TimeZone == -32768)
            {
                tm.Skip |= DateTimeSkips.Deviation;
            }
            else
            {
                //If clock's time zone is different what user want's to use.
                int offset = TimeZone;
                if (useUtc2NormalTime)
                {
                    offset -= (int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes;
                }
                else
                {
                    offset += (int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes;
                }
                if (offset != 0 ||
                    (!Enabled && time.IsDaylightSavingTime()))
                {
                    TimeSpan zone;
                    if (Enabled)
                    {
                        zone = new TimeSpan(0, -TimeZone + Deviation, 0);
                    }
                    else
                    {
                        time = time.AddMinutes(-60);
                        if (useUtc2NormalTime)
                        {
                            zone = new TimeSpan(0, TimeZone, 0);
                        }
                        else
                        {
                            zone = new TimeSpan(0, -TimeZone, 0);
                        }
                    }
                    time = time.AddMinutes(-offset);
                    time = new DateTime(time.Ticks, DateTimeKind.Unspecified);
                    tm.Value = new DateTimeOffset(time, zone);
                }
            }
            //If clock's daylight saving is active but user do not want to use it.
            if (!Enabled && time.IsDaylightSavingTime())
            {
                tm.Status &= ~ClockStatus.DaylightSavingActive;
                //                tm.Value = tm.Value.AddMinutes(-Deviation);
            }
            return tm;
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return Time;
            }
            if (e.Index == 3)
            {
                return TimeZone;
            }
            if (e.Index == 4)
            {
                return Status;
            }
            if (e.Index == 5)
            {
                return Begin;
            }
            if (e.Index == 6)
            {
                return End;
            }
            if (e.Index == 7)
            {
                return Deviation;
            }
            if (e.Index == 8)
            {
                return Enabled;
            }
            if (e.Index == 9)
            {
                return ClockBase;
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
                if (e.Value == null)
                {
                    Time = new GXDateTime(DateTime.MinValue);
                }
                else
                {
                    if (e.Value is byte[])
                    {
                        e.Value = GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime, settings != null && settings.UseUtc2NormalTime);
                    }
                    else if (e.Value is string)
                    {
                        e.Value = new GXDateTime((string)e.Value);
                    }
                    if (e.Value is GXDateTime)
                    {
                        Time = (GXDateTime)e.Value;
                    }
                    else if (e.Value is String)
                    {
                        DateTime tm;
                        if (!DateTime.TryParse((String)e.Value, out tm))
                        {
                            Time = DateTime.ParseExact((String)e.Value, CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern, CultureInfo.CurrentUICulture);
                        }
                        else
                        {
                            Time = tm;
                        }
                    }
                    else
                    {
                        Time = Convert.ToDateTime(e.Value);
                    }
                }
            }
            else if (e.Index == 3)
            {
                TimeZone = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 4)
            {
                Status = (ClockStatus)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 5)
            {
                if (e.Value == null)
                {
                    Begin = new GXDateTime(DateTime.MinValue);
                }
                else
                {
                    if (e.Value is byte[])
                    {
                        e.Value = GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime, settings.UseUtc2NormalTime);
                    }
                    else if (e.Value is string)
                    {
                        e.Value = new GXDateTime((string)e.Value);
                    }
                    Begin = (GXDateTime)e.Value;
                }
            }
            else if (e.Index == 6)
            {
                if (e.Value == null)
                {
                    End = new GXDateTime(DateTime.MinValue);
                }
                else
                {
                    if (e.Value is byte[])
                    {
                        e.Value = GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime, settings.UseUtc2NormalTime);
                    }
                    else if (e.Value is string)
                    {
                        e.Value = new GXDateTime((string)e.Value);
                    }
                    End = (GXDateTime)e.Value;
                }
            }
            else if (e.Index == 7)
            {
                Deviation = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 8)
            {
                Enabled = Convert.ToBoolean(e.Value);
                if (settings != null && settings.IsServer)
                {
                    if (Enabled)
                    {
                        Status |= ClockStatus.DaylightSavingActive;
                    }
                    else
                    {
                        Status &= ~ClockStatus.DaylightSavingActive;
                    }
                }
            }
            else if (e.Index == 9)
            {
                ClockBase = (ClockBase)Convert.ToInt32(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Time = reader.ReadElementContentAsDateTime("Time");
            TimeZone = reader.ReadElementContentAsInt("TimeZone");
            Status = (ClockStatus)reader.ReadElementContentAsInt("Status");
            Begin = reader.ReadElementContentAsDateTime("Begin");
            End = reader.ReadElementContentAsDateTime("End");
            Deviation = reader.ReadElementContentAsInt("Deviation");
            Enabled = reader.ReadElementContentAsInt("Enabled") != 0;
            ClockBase = (ClockBase)reader.ReadElementContentAsInt("ClockBase");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Time", Time, 2);
            writer.WriteElementString("TimeZone", TimeZone, 3);
            writer.WriteElementString("Status", (int)Status, 4);
            writer.WriteElementString("Begin", Begin, 5);
            writer.WriteElementString("End", End, 6);
            writer.WriteElementString("Deviation", Deviation, 7);
            writer.WriteElementString("Enabled", Enabled, 8);
            writer.WriteElementString("ClockBase", (int)ClockBase, 9);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
