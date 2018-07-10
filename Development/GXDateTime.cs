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
using Gurux.DLMS.Enums;
using System.ComponentModel;
using System.Globalization;

namespace Gurux.DLMS
{
    /// <summary>
    /// This class is used because in COSEM object model some fields from date time can be ignored.
    /// Default behavior of DateTime do not allow this.
    /// </summary>
    public class GXDateTime : IConvertible
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateTime()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateTime(DateTime value) : this(value, null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateTime(DateTime value, TimeZoneInfo timeZone)
        {
            if (value == DateTime.MinValue)
            {
                Value = DateTimeOffset.MinValue;
            }
            else if (value == DateTime.MaxValue)
            {
                Value = DateTimeOffset.MaxValue;
            }
            else
            {
                if (timeZone != null)
                {
                    Value = new DateTimeOffset(value, timeZone.GetUtcOffset(value));
                }
                else if (value.Kind == DateTimeKind.Utc)
                {
                    Value = new DateTimeOffset(value, TimeZoneInfo.Utc.GetUtcOffset(value));
                }
                else
                {
                    Value = new DateTimeOffset(value, TimeZoneInfo.Local.GetUtcOffset(value));
                }
                if (value.IsDaylightSavingTime())
                {
                    Status |= ClockStatus.DaylightSavingActive;
                }
                Skip |= DateTimeSkips.Ms;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Date time value as a string.</param>
        public GXDateTime(string value)
            : this(value, System.Globalization.CultureInfo.CurrentCulture)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Date time value as a string.</param>
        /// <param name="culture">Used culture.</param>
        public GXDateTime(string value, CultureInfo culture)
            : base()
        {
            if (!string.IsNullOrEmpty(value))
            {
                DateTime dt;
                if (value.IndexOf('*') == -1)
                {
                    if (DateTime.TryParse(value, culture, System.Globalization.DateTimeStyles.None, out dt))
                    {
                        Value = new DateTimeOffset(dt, TimeZoneInfo.Local.GetUtcOffset(dt));
                        if (dt.IsDaylightSavingTime())
                        {
                            Status |= ClockStatus.DaylightSavingActive;
                        }
                        return;
                    }
                }
                int year = 2000, month = 1, day = 1, hour = 0, min = 0, sec = 0;
#if !WINDOWS_UWP                
                string dateSeparator = culture.DateTimeFormat.DateSeparator, timeSeparator = culture.DateTimeFormat.TimeSeparator;
                List<string> shortDatePattern = new List<string>(culture.DateTimeFormat.ShortDatePattern.Split(new string[] { CultureInfo.InvariantCulture.DateTimeFormat.DateSeparator, dateSeparator, " " }, StringSplitOptions.RemoveEmptyEntries));
                List<string> shortTimePattern = new List<string>(culture.DateTimeFormat.LongTimePattern.Split(new string[] { CultureInfo.InvariantCulture.DateTimeFormat.TimeSeparator, timeSeparator, " ", "." }, StringSplitOptions.RemoveEmptyEntries));
#else
                //In UWP Standard Date and Time Format Strings are used.
                string dateSeparator = Internal.GXCommon.GetDateSeparator(), timeSeparator = Internal.GXCommon.GetTimeSeparator();
                List<string> shortDatePattern = new List<string>("yyyy-MM-dd".Split(new string[] { dateSeparator, " " }, StringSplitOptions.RemoveEmptyEntries));
                List<string> shortTimePattern = new List<string>(culture.DateTimeFormat.LongTimePattern.Split(new string[] { timeSeparator, " ", "." }, StringSplitOptions.RemoveEmptyEntries));
#endif
                //If week day is used.
                if (char.IsLetter(value.Trim()[0]))
                {
                    shortDatePattern.Insert(0, "dddd");
                }
                else
                {
                    Skip |= DateTimeSkips.DayOfWeek;
                }
                string[] values = value.Trim().Split(new string[] { dateSeparator, timeSeparator, " " }, StringSplitOptions.None);
                int cnt = shortDatePattern.Count + shortTimePattern.Count;
                if (!string.IsNullOrEmpty(culture.DateTimeFormat.PMDesignator))
                {
                    if (value.IndexOf(culture.DateTimeFormat.PMDesignator) != -1)
                    {
                        ++cnt;
                    }
                    else if (value.IndexOf(culture.DateTimeFormat.AMDesignator) != -1)
                    {
                        ++cnt;
                    }
                }
                int offset = 0;
                for (int pos = 0; pos != shortDatePattern.Count; ++pos)
                {
                    bool skip = false;
                    if (values[pos] == "*")
                    {
                        skip = true;
                    }
                    if (shortDatePattern[pos].ToLower().StartsWith("yy"))
                    {
                        if (skip)
                        {
                            Skip |= DateTimeSkips.Year;
                        }
                        else
                        {
                            year = int.Parse(values[pos]);
                        }
                    }
                    else if (shortDatePattern[pos].ToLower().StartsWith("m"))
                    {
                        if (skip)
                        {
                            Skip |= DateTimeSkips.Month;
                        }
                        else
                        {
                            if (string.Compare(values[pos], "B", true) == 0)
                            {
                                month = 1;
                                Extra |= DateTimeExtraInfo.DstBegin;
                            }
                            else if (string.Compare(values[pos], "E", true) == 0)
                            {
                                month = 1;
                                Extra |= DateTimeExtraInfo.DstEnd;
                            }
                            else
                            {
                                month = int.Parse(values[pos]);
                            }
                        }
                    }
                    else if (shortDatePattern[pos].ToLower().StartsWith("dddd"))
                    {
                        if (skip)
                        {
                            Skip |= DateTimeSkips.DayOfWeek;
                        }
                        else
                        {
                            DayOfWeek = (int)Enum.Parse(typeof(DayOfWeek), values[pos]);
                            //Sunday is special case.
                            if (DayOfWeek == 0)
                            {
                                DayOfWeek = 7;
                            }
                        }
                    }
                    else if (shortDatePattern[pos].ToLower().StartsWith("d"))
                    {
                        if (skip)
                        {
                            Skip |= DateTimeSkips.Day;
                        }
                        else
                        {
                            day = int.Parse(values[pos]);
                            if (day == -1)
                            {
                                day = 1;
                                Extra |= DateTimeExtraInfo.LastDay;
                            }
                            else if (day == -1)
                            {
                                day = 1;
                                Extra |= DateTimeExtraInfo.LastDay2;
                            }
                        }
                    }
                    ++offset;
                }
                if (values.Length > 3)
                {
                    for (int pos = 0; pos != shortTimePattern.Count; ++pos)
                    {
                        //If ms is used.
                        if (offset + pos >= values.Length)
                        {
                            continue;
                        }
                        bool skip = false;
                        if (values[offset + pos] == "*")
                        {
                            skip = true;
                        }
                        if (shortTimePattern[pos].ToLower().StartsWith("h"))
                        {
                            if (skip)
                            {
                                Skip |= DateTimeSkips.Hour;
                            }
                            else
                            {
                                hour = int.Parse(values[offset + pos]);
                            }
                        }
                        else if (shortTimePattern[pos].ToLower().StartsWith("m"))
                        {
                            if (skip)
                            {
                                Skip |= DateTimeSkips.Minute;
                            }
                            else
                            {
                                min = int.Parse(values[offset + pos]);
                            }
                        }
                        else if (shortTimePattern[pos].ToLower().StartsWith("ss"))
                        {
                            if (skip)
                            {
                                Skip |= DateTimeSkips.Second;
                            }
                            else
                            {
                                sec = int.Parse(values[offset + pos]);
                            }
                        }
                        else if (shortTimePattern[pos].ToLower().StartsWith("tt"))
                        {
                            if ((Skip & DateTimeSkips.Hour) == 0)
                            {
                                if (!string.IsNullOrEmpty(culture.DateTimeFormat.PMDesignator))
                                {
                                    if (values[offset + pos] == culture.DateTimeFormat.PMDesignator && hour != 12)
                                    {
                                        hour += 12;
                                    }
                                }
                                if (!string.IsNullOrEmpty(culture.DateTimeFormat.AMDesignator))
                                {
                                    if (values[offset + pos] == culture.DateTimeFormat.AMDesignator && hour == 12)
                                    {
                                        hour = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //This is OK. There might be some extra in some cultures.
                            // ++offset;
                        }
                    }
                }
                dt = culture.Calendar.ToDateTime(year, month, day, hour, min, sec, 0);
                Value = new DateTimeOffset(dt);
                if (dt.IsDaylightSavingTime())
                {
                    Status |= ClockStatus.DaylightSavingActive;
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateTime(DateTimeOffset value)
        {
            Value = value;
            if (value.DateTime.IsDaylightSavingTime())
            {
                Status |= ClockStatus.DaylightSavingActive;
            }
        }

        /// <summary>
        /// Convert DateTime to GXDateTime.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator GXDateTime(DateTime value)
        {
            GXDateTime dt = new GXDateTime(value);
            dt.Skip |= DateTimeSkips.Ms;
            if (value.IsDaylightSavingTime())
            {
                dt.Status |= ClockStatus.DaylightSavingActive;
            }
            return dt;
        }

        /// <summary>
        /// Convert GXDateTime to DateTime.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator DateTime(GXDateTime value)
        {
            if (value != null)
            {
                return value.Value.LocalDateTime;
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            if (year < 1 || year == 0xFFFF)
            {
                Skip |= DateTimeSkips.Year | DateTimeSkips.DayOfWeek;
                //Set year to 4 because there might be leap year.
                //29/02/*" (29th February) 
                year = 4;
            }
            if (month == 0xFE)
            {
                Extra |= DateTimeExtraInfo.DstBegin;
            }
            else if (month == 0xFD)
            {
                Extra |= DateTimeExtraInfo.DstEnd;
            }
            if (month < 1 || month > 12)
            {
                Skip |= DateTimeSkips.Month;
                month = 1;
            }
            if (day < 1 || day > 31)
            {
                Skip |= DateTimeSkips.Day;
                day = 1;
            }
            if (hour < 0 || hour > 24)
            {
                Skip |= DateTimeSkips.Hour;
                hour = 0;
            }
            if (minute < 0 || minute > 60)
            {
                Skip |= DateTimeSkips.Minute;
                minute = 0;
            }
            if (second < 0 || second > 60)
            {
                Skip |= DateTimeSkips.Second;
                second = 0;
            }
            if (millisecond < 0 || millisecond > 1000)
            {
                Skip |= DateTimeSkips.Ms;
                millisecond = 0;
            }
            try
            {
                if (year == 1 && month == 1 && day == 1 && hour == 0 && minute == 0 && second == 0)
                {
                    Value = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
                }
                else
                {
                    Value = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Local);
                    if (Value.DateTime.IsDaylightSavingTime())
                    {
                        Status |= ClockStatus.DaylightSavingActive;
                    }
                }
            }
            catch
            {
                Value = DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Used date time value.
        /// </summary>
        public DateTimeOffset Value
        {
            get;
            set;
        }

        /// <summary>
        /// Skip selected date time fields.
        /// </summary>
        [DefaultValue(DateTimeSkips.None)]
        public DateTimeSkips Skip
        {
            get;
            set;
        }

        /// <summary>
        /// Date time extra information.
        /// </summary>
        [DefaultValue(DateTimeExtraInfo.None)]
        public DateTimeExtraInfo Extra
        {
            get;
            set;
        }

        /// <summary>
        /// Day of week.
        /// </summary>
        [DefaultValue(0)]
        public int DayOfWeek
        {
            get;
            set;
        }


        /// <summary>
        /// Status of the clock.
        /// </summary>
        [DefaultValue(ClockStatus.Ok)]
        public ClockStatus Status
        {
            get;
            set;
        }

        public string ToFormatString()
        {
            return ToFormatString(System.Globalization.CultureInfo.CurrentCulture);
        }

        public string ToFormatString(CultureInfo culture)
        {
            int pos;
            if (Skip != DateTimeSkips.None)
            {
#if !WINDOWS_UWP
                string dateSeparator = culture.DateTimeFormat.DateSeparator, timeSeparator = culture.DateTimeFormat.TimeSeparator;
                List<string> shortDatePattern = new List<string>(culture.DateTimeFormat.ShortDatePattern.Split(new string[] { dateSeparator, " " }, StringSplitOptions.RemoveEmptyEntries));
#else
                //In UWP Standard Date and Time Format Strings are used.
                string dateSeparator = Internal.GXCommon.GetDateSeparator(), timeSeparator = Internal.GXCommon.GetTimeSeparator();
                List<string> shortDatePattern = new List<string>("yyyy-MM-dd".Split(new string[] { dateSeparator }, StringSplitOptions.RemoveEmptyEntries));
#endif
                List<string> shortTimePattern = new List<string>(culture.DateTimeFormat.LongTimePattern.Split(new string[] { timeSeparator, " ", "." }, StringSplitOptions.RemoveEmptyEntries));
                if (this is GXTime)
                {
                    shortDatePattern.Clear();
                }
                else
                {
                    if ((Skip & DateTimeSkips.Year) != 0)
                    {
                        pos = shortDatePattern.IndexOf("yyyy");
                        if (pos == -1)
                        {
                            pos = shortDatePattern.IndexOf("yy");
                        }
                        shortDatePattern[pos] = "*";
                    }
                    if ((Skip & DateTimeSkips.Month) != 0)
                    {
                        pos = shortDatePattern.IndexOf("M");
                        if (pos == -1)
                        {
                            pos = shortDatePattern.IndexOf("MM");
                        }
                        shortDatePattern[pos] = "*";
                    }
                    if ((Skip & DateTimeSkips.Day) != 0)
                    {
                        pos = shortDatePattern.IndexOf("d");
                        if (pos == -1)
                        {
                            pos = shortDatePattern.IndexOf("dd");
                        }
                        shortDatePattern[pos] = "*";
                    }
                    else
                    {
                        pos = shortDatePattern.IndexOf("d");
                        if (pos == -1)
                        {
                            pos = shortDatePattern.IndexOf("dd");
                        }
                        if ((Extra & DateTimeExtraInfo.LastDay) != 0)
                        {
                            shortDatePattern[pos] = "-1";
                        }
                        else if ((Extra & DateTimeExtraInfo.LastDay2) != 0)
                        {
                            shortDatePattern[pos] = "-2";
                        }
                    }
                }
                if (this is GXDate)
                {
                    shortTimePattern.Clear();
                }
                else
                {
                    if ((Skip & DateTimeSkips.Hour) != 0)
                    {
                        pos = shortTimePattern.IndexOf("h");
                        if (pos == -1)
                        {
                            pos = shortTimePattern.IndexOf("H");
                            if (pos == -1)
                            {
                                pos = shortTimePattern.IndexOf("HH");
                            }
                        }
                        shortTimePattern[pos] = "*";
                    }
                    if ((Skip & DateTimeSkips.Minute) != 0)
                    {
                        pos = shortTimePattern.IndexOf("mm");
                        shortTimePattern[pos] = "*";
                    }
                    if ((Skip & DateTimeSkips.Second) != 0 ||
                        (shortTimePattern.Count == 1 && Value.Second == 0))
                    {
                        pos = shortTimePattern.IndexOf("ss");
                        shortTimePattern[pos] = "*";
                    }
                }
                string format = null;
                if (shortDatePattern.Count != 0)
                {
                    format = string.Join(dateSeparator, shortDatePattern.ToArray());
                }
                if (shortTimePattern.Count != 0)
                {
                    if (format != null)
                    {
                        format += " ";
                    }
                    format += string.Join(timeSeparator, shortTimePattern.ToArray());
                    format = format.Replace(timeSeparator + "tt", " tt");
                }
                if (format == "H")
                {
                    return Value.Hour.ToString();
                }
                if (format == null)
                {
                    return "";
                }
                string str = Value.LocalDateTime.ToString(format, culture);
                if (DayOfWeek != 0 && (Skip & DateTimeSkips.DayOfWeek) == 0 &&
                    (Skip & (DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day)) != 0)
                {
                    System.DayOfWeek t = (System.DayOfWeek)DayOfWeek;
                    if (DayOfWeek == 7)
                    {
                        t = System.DayOfWeek.Sunday;
                    }
                    str = t.ToString() + " " + str;
                }
                return str;
            }
            return Value.LocalDateTime.ToString(culture);
        }

        /// <summary>
        /// Date time to string.
        /// </summary>
        /// <returns>Date time as a string.</returns>
        public override string ToString()
        {
            if (Skip != DateTimeSkips.None)
            {
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;
#if !WINDOWS_UWP
                string dateSeparator = culture.DateTimeFormat.DateSeparator, timeSeparator = culture.DateTimeFormat.TimeSeparator;
                List<string> shortDatePattern = new List<string>(culture.DateTimeFormat.ShortDatePattern.Split(new string[] { dateSeparator }, StringSplitOptions.RemoveEmptyEntries));
#else
                //In UWP Standard Date and Time Format Strings are used.
                string dateSeparator = Internal.GXCommon.GetDateSeparator(), timeSeparator = Internal.GXCommon.GetTimeSeparator();
                List<string> shortDatePattern = new List<string>("yyyy-MM-dd".Split(new string[] { dateSeparator }, StringSplitOptions.RemoveEmptyEntries));
#endif
                List<string> shortTimePattern = new List<string>(culture.DateTimeFormat.LongTimePattern.Split(new string[] { timeSeparator, " ", "." }, StringSplitOptions.RemoveEmptyEntries));
                if ((Skip & DateTimeSkips.Year) != 0)
                {
                    shortDatePattern.Remove("yyyy");
                    shortDatePattern.Remove("yy");
                }
                if ((Skip & DateTimeSkips.Month) != 0)
                {
                    shortDatePattern.Remove("M");
                    shortDatePattern.Remove("MM");
                }
                if ((Skip & DateTimeSkips.Day) != 0 || (Extra & DateTimeExtraInfo.LastDay) != 0 || (Extra & DateTimeExtraInfo.LastDay2) != 0)
                {
                    shortDatePattern.Remove("d");
                    shortDatePattern.Remove("dd");
                }
                if ((Skip & DateTimeSkips.Hour) != 0)
                {
                    shortTimePattern.Remove("h");
                    shortTimePattern.Remove("H");
                    shortTimePattern.Remove("HH");
                    shortTimePattern.Remove("tt");
                }
                if ((Skip & DateTimeSkips.Minute) != 0)
                {
                    shortTimePattern.Remove("mm");
                }
                if ((Skip & DateTimeSkips.Second) != 0 ||
                    (shortTimePattern.Count == 1 && Value.Second == 0))
                {
                    shortTimePattern.Remove("ss");
                }
                string format = null;
                if (shortDatePattern.Count != 0)
                {
                    format = string.Join(dateSeparator, shortDatePattern.ToArray());
                }
                if (shortTimePattern.Count != 0)
                {
                    if (format != null)
                    {
                        format += " ";
                    }
                    format += string.Join(timeSeparator, shortTimePattern.ToArray());
                }
                if (format == "H")
                {
                    return Value.Hour.ToString();
                }
                if (format == null)
                {
                    return "";
                }
                string str = Value.LocalDateTime.ToString(format, culture);
                if (DayOfWeek != 0 && (Skip & DateTimeSkips.DayOfWeek) == 0)
                {
                    System.DayOfWeek t = (System.DayOfWeek)DayOfWeek;
                    if (DayOfWeek == 7)
                    {
                        t = System.DayOfWeek.Sunday;
                    }
                    str = t.ToString() + " " + str;

                    if ((Extra & DateTimeExtraInfo.LastDay) != 0)
                    {
                        str = "Last " + str;
                    }
                }
                else if ((Extra & DateTimeExtraInfo.LastDay) != 0)
                {
                    str = "Last Day of month " + str;
                }
                return str;
            }
            if (DayOfWeek != 0)
            {
#if !WINDOWS_UWP
                return Value.LocalDateTime.ToLongDateString();
#else
                return Value.LocalDateTime.ToString();
#endif //!WINDOWS_UWP
            }
            return Value.LocalDateTime.ToString();
        }

        /// <summary>
        /// Get next schedule dates.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static DateTime[] GetNextScheduledDates(DateTime start, GXDateTime value, int count)
        {
            DateTime tmp;
            List<DateTime> list = new List<DateTime>();
            for (int pos = 0; pos != count; ++pos)
            {
                if ((value.Extra & DateTimeExtraInfo.LastDay) != 0)
                {
                    int y = 0;
                    int m = start.Month + pos;
                    if (m > 12)
                    {
                        y = m / 12;
                        m %= 12;
                    }
                    tmp = new DateTime(start.Year + y, m, DateTime.DaysInMonth(start.Year, m), value.Value.Hour, value.Value.Minute, value.Value.Second);
                    if ((value.Skip & DateTimeSkips.DayOfWeek) == 0)
                    {
                        int d = value.DayOfWeek;
                        if (value.DayOfWeek == 7)
                        {
                            d = 0;
                        }
                        tmp = tmp.AddDays(-((int)tmp.DayOfWeek - d));
                    }
                    list.Add(tmp);
                }
                else if ((value.Skip & DateTimeSkips.Second) != 0)
                {
                    list.Add(start.AddSeconds(pos));
                }
                else if ((value.Skip & DateTimeSkips.Minute) != 0)
                {
                    list.Add(start.AddMinutes(pos));
                }
                else if ((value.Skip & DateTimeSkips.Hour) != 0)
                {
                    list.Add(start.AddHours(pos));
                }
                else if ((value.Skip & DateTimeSkips.Day) != 0)
                {
                    list.Add(start.AddDays(pos));
                }
                else if ((value.Skip & DateTimeSkips.Month) != 0)
                {
                    list.Add(start.AddMonths(pos));
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Get difference between given time and run time in ms.
        /// </summary>
        /// <param name="start">Start date time.</param>
        /// <param name="to">Compared time.</param>
        /// <returns>Difference in milliseconds.</returns>
        public static long GetDifference(DateTime start, GXDateTime to)
        {
            long diff = 0;
            //Compare ms.
            if ((to.Skip & DateTimeSkips.Ms) == 0)
            {
                if (start.Millisecond < to.Value.Millisecond)
                {
                    diff = to.Value.Millisecond;
                }
                else
                {
                    diff = -to.Value.Millisecond;
                }
            }
            //Compare seconds.
            if ((to.Skip & DateTimeSkips.Second) == 0)
            {
                if (start.Second < to.Value.Second)
                {
                    diff += (to.Value.Second - start.Second) * 1000L;
                }
                else
                {
                    diff -= (start.Second - to.Value.Second) * 1000L;
                }
            }
            else if (diff < 0)
            {
                diff = 60000 + diff;
            }
            //Compare minutes.
            if ((to.Skip & DateTimeSkips.Minute) == 0)
            {
                if (start.Minute < to.Value.Minute)
                {
                    diff += (to.Value.Minute - start.Minute) * 60000L;
                }
                else
                {
                    diff -= (start.Minute - to.Value.Minute) * 60000L;
                }
            }
            else if (diff < 0)
            {
                diff = 60 * 60000 + diff;
            }
            //Compare hours.
            if ((to.Skip & DateTimeSkips.Hour) == 0)
            {
                if (start.Hour < to.Value.Hour)
                {
                    diff += (to.Value.Hour - start.Hour) * 60 * 60000L;
                }
                else
                {
                    diff -= (start.Hour - to.Value.Hour) * 60 * 60000L;
                }
            }
            else if (diff < 0)
            {
                diff = 60 * 60000 + diff;
            }
            //Compare days.
            if ((to.Skip & DateTimeSkips.Day) == 0)
            {
                if (start.Day < to.Value.Day)
                {
                    diff += (to.Value.Day - start.Day) * 24 * 60 * 60000;
                }
                else if (start.Day != to.Value.Day)
                {
                    if ((to.Skip & DateTimeSkips.Month) == 0)
                    {
                        diff += (to.Value.Day - start.Day) * 24 * 60 * 60000L;
                    }
                    else
                    {
                        diff = ((DateTime.DaysInMonth(start.Year, start.Month) - start.Day + to.Value.Day) * 24 * 60 * 60000L) + diff;
                    }
                }
            }
            else if (diff < 0)
            {
                diff = 24 * 60 * 60000 + diff;
            }
            //Compare months.
            if ((to.Skip & DateTimeSkips.Month) == 0)
            {
                if (start.Month < to.Value.Month)
                {
                    for (int m = start.Month; m != to.Value.Month; ++m)
                    {
                        diff += DateTime.DaysInMonth(start.Year, m) * 24 * 60 * 60000L;
                    }
                }
                else
                {
                    for (int m = to.Value.Month; m != start.Month; ++m)
                    {
                        diff += -DateTime.DaysInMonth(start.Year, m) * 24 * 60 * 60000L;
                    }
                }
            }
            else if (diff < 0)
            {
                diff = DateTime.DaysInMonth(start.Year, start.Month) * 24 * 60 * 60000L + diff;
            }
            return diff;
        }

        /// <summary>
        /// Get date time from Epoch time.
        /// </summary>
        /// <param name="unixTime">Unix time.</param>
        /// <returns>Date and time.</returns>
        public static GXDateTime FromUnixTime(long unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc);
            return new GXDateTime(epoch.AddSeconds(unixTime).ToLocalTime());
        }

        /// <summary>
        /// Convert date time to Epoch time.
        /// </summary>
        /// <param name="date">Date and time.</param>
        /// <returns>Unix time.</returns>
        public static long ToUnixTime(DateTime date)
        {
            if (date == DateTime.MinValue ||
                date == DateTime.MaxValue)
            {
                return Convert.ToInt64(0);
            }
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        }

        /// <summary>
        /// Convert date time to Epoch time.
        /// </summary>
        /// <param name="date">Date and time.</param>
        /// <returns>Unix time.</returns>
        public static long ToUnixTime(GXDateTime date)
        {
            return ToUnixTime(date.Value.DateTime);
        }

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.String;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Value.LocalDateTime;
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

#endregion
    }
}
