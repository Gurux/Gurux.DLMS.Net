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
using System.Text;

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

        private static bool IsNumeric(char value)
        {
            return value >= '0' && value <= '9';
        }

        private static string GetDateTimeFormat(CultureInfo culture)
        {
            string str = culture.DateTimeFormat.ShortDatePattern + " " + culture.DateTimeFormat.LongTimePattern;
            foreach (string it in culture.DateTimeFormat.GetAllDateTimePatterns())
            {
                if (!it.Contains("dddd") && it.Contains(culture.DateTimeFormat.ShortDatePattern) && it.Contains(culture.DateTimeFormat.LongTimePattern))
                {
                    return it;
                }
            }
            return str;
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
                StringBuilder format = new StringBuilder();
                format.Append(GetDateTimeFormat(culture));
                Remove(format, culture);
                String v = value;
                if (value.IndexOf('*') != -1)
                {
                    int lastFormatIndex = -1;
                    int offset = 0;
                    for (int pos = 0; pos < value.Length; ++pos)
                    {
                        char c = value[pos];
                        if (!IsNumeric(c))
                        {
                            if (c == '*')
                            {
                                int cnt = 1;
                                c = format[lastFormatIndex + 1];
                                string val = "1";
                                while (lastFormatIndex + cnt + 1 < format.Length && format[lastFormatIndex + cnt + 1] == c)
                                {
                                    val += "0";
                                    ++cnt;
                                }
                                v = v.Substring(0, pos + offset) + val
                                        + value.Substring(pos + 1);
                                offset += cnt - 1;
                                String tmp = format.ToString().Substring(lastFormatIndex + 1, cnt).Trim();
                                if (tmp.StartsWith("y"))
                                {
                                    Skip |= DateTimeSkips.Year;
                                }
                                else if (tmp == "M" || tmp == "MM")
                                {
                                    Skip |= DateTimeSkips.Month;
                                }
                                else if (tmp == "M" || tmp == "MM" || tmp == "MMM")
                                {
                                    Skip |= DateTimeSkips.Month;
                                }
                                else if (tmp.Equals("dd") || tmp.Equals("d"))
                                {
                                    Skip |= DateTimeSkips.Day;
                                }
                                else if (tmp.Equals("h") || tmp.Equals("hh")
                                      || tmp.Equals("HH") || tmp.Equals("H"))
                                {
                                    Skip |= DateTimeSkips.Hour;
                                    if (format.ToString().IndexOf("tt") != -1)
                                    {
                                        value += " " + culture.DateTimeFormat.AMDesignator;
                                        v += " " + culture.DateTimeFormat.AMDesignator;
                                    }
                                }
                                else if (tmp.Equals("mm") || tmp.Equals("m"))
                                {
                                    Skip |= DateTimeSkips.Minute;
                                }
                                else if (tmp.Equals("tt"))
                                {
                                    Skip |= DateTimeSkips.Hour;
                                    format.Replace("tt", "");
                                }
                                else if (tmp.Equals("ss"))
                                {
                                    Skip |= DateTimeSkips.Second;
                                }
                                else if (tmp.Length != 0 && !tmp.Equals("G"))
                                {
                                    throw new Exception("Invalid date time format.");
                                }
                            }
                            else
                            {
                                lastFormatIndex = format.ToString().IndexOf(c, lastFormatIndex + 1);
                                //Dot is used time separator in some countries.
                                if (lastFormatIndex == -1 && c == ':')
                                {
                                    lastFormatIndex = format.ToString().IndexOf('.', lastFormatIndex + 1);
                                }
                            }
                        }
                    }
                }
                try
                {
                    Value = DateTime.ParseExact(v, format.ToString(), culture);
                    Skip |= DateTimeSkips.Ms;
                    if ((Skip & (DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.Hour | DateTimeSkips.Minute)) == 0)
                    {
                        if (Value.DateTime.IsDaylightSavingTime())
                        {
                            Status |= ClockStatus.DaylightSavingActive;
                        }
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        if ((Skip & DateTimeSkips.Second) == 0)
                        {
                            format.Replace("mm", "mm" + culture.DateTimeFormat.TimeSeparator + "ss");
                        }
                        Value = DateTime.ParseExact(v, format.ToString().Trim(), culture);
                        Skip |= DateTimeSkips.Ms;
                    }
                    catch (Exception e1)
                    {
                        try
                        {
                            if ((Skip & DateTimeSkips.Ms) == 0)
                            {
                                format.Replace("ss", "ss.fff");
                            }
                            Value = DateTime.ParseExact(v, format.ToString().Trim(), culture);
                        }
                        catch (Exception e2)
                        {
                            throw e2;
                        }
                    }
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
            return ToFormatString(CultureInfo.CurrentCulture);
        }

        public string ToFormatString(CultureInfo culture)
        {
            StringBuilder format = new StringBuilder();
            if (Skip != DateTimeSkips.None)
            {
                format.Append(GetDateTimeFormat(culture));
                Remove(format, culture);
                if ((Skip & DateTimeSkips.Year) != 0)
                {
                    Replace(format, "yyyy");
                    Replace(format, "yy");
                }
                if ((Skip & DateTimeSkips.Month) != 0)
                {
                    Replace(format, "MMM");
                    Replace(format, "MM");
                    Replace(format, "M");
                }
                if ((Skip & DateTimeSkips.Day) != 0)
                {
                    Replace(format, "dd");
                    Replace(format, "d");
                }
                if ((Skip & DateTimeSkips.Hour) != 0)
                {
                    Replace(format, "HH");
                    Replace(format, "H");
                    Replace(format, "hh");
                    Replace(format, "h");
                    Remove(format, "tt", null);
                }
                if ((Skip & DateTimeSkips.Ms) != 0)
                {
                    Replace(format, ".fff");
                }
                else if (format.ToString().IndexOf(".fff") == -1)
                {
                    format.Replace("ss", "ss.fff");
                }
                if ((Skip & DateTimeSkips.Second) != 0)
                {
                    Replace(format, "ss");
                }
                else if (format.ToString().IndexOf("ss") == -1)
                {
                    format.Replace("mm", "mm" + culture.DateTimeFormat.TimeSeparator + "ss");
                }
                if ((Skip & DateTimeSkips.Minute) != 0)
                {
                    Replace(format, "mm");
                    Replace(format, "m");
                }
                return Value.LocalDateTime.ToString(format.ToString().Trim());
            }
            return Value.LocalDateTime.ToString(culture);
        }

        private void Remove(StringBuilder value, String tag, string sep)
        {
            if (sep != null)
            {
                if (value.ToString().IndexOf(tag + sep) != -1)
                {
                    value.Replace(tag + sep, "");
                    return;
                }
                else if (value.ToString().IndexOf(sep + tag) != -1)
                {
                    value.Replace(sep + tag, "");
                    return;
                }
            }
            value.Replace(tag, "");
        }

        private void Replace(StringBuilder value, string tag)
        {
            value.Replace(tag, "*");
        }

        private void Remove(StringBuilder format, CultureInfo culture)
        {
            if (this is GXDate)
            {
                Remove(format, "HH", culture.DateTimeFormat.TimeSeparator);
                Remove(format, "hh", culture.DateTimeFormat.TimeSeparator);
                Remove(format, "H", culture.DateTimeFormat.TimeSeparator);
                Remove(format, "h", culture.DateTimeFormat.TimeSeparator);
                Remove(format, "mm", culture.DateTimeFormat.TimeSeparator);
                Remove(format, "m", culture.DateTimeFormat.TimeSeparator);
                Remove(format, "ss", culture.DateTimeFormat.TimeSeparator);
                Remove(format, "tt", culture.DateTimeFormat.TimeSeparator);
                Skip |= DateTimeSkips.Hour | DateTimeSkips.Minute | DateTimeSkips.Second | DateTimeSkips.Ms;
            }
            else if (this is GXTime)
            {
                Remove(format, "yyyy", culture.DateTimeFormat.DateSeparator);
                Remove(format, "yy", culture.DateTimeFormat.DateSeparator);
                Remove(format, "MM", culture.DateTimeFormat.DateSeparator);
                Remove(format, "M", culture.DateTimeFormat.DateSeparator);
                Remove(format, "dd", culture.DateTimeFormat.DateSeparator);
                Remove(format, "d", culture.DateTimeFormat.DateSeparator);
                Skip |= DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.DayOfWeek;
            }
            // Trim
            String tmp = format.ToString();
            format.Length = 0;
            format.Append(tmp.Trim());
        }

        /// <summary>
        /// Date time to string.
        /// </summary>
        /// <returns>Date time as a string.</returns>
        public override string ToString()
        {
            StringBuilder format = new StringBuilder();
            if (Skip != DateTimeSkips.None)
            {
                System.Globalization.CultureInfo culture = CultureInfo.CurrentCulture;
                format.Append(GetDateTimeFormat(culture));
                Remove(format, culture);
                if ((Skip & DateTimeSkips.Year) != 0)
                {
                    Remove(format, "yyyy", culture.DateTimeFormat.DateSeparator);
                    Remove(format, "yy", culture.DateTimeFormat.DateSeparator);
                }
                if ((Skip & DateTimeSkips.Month) != 0)
                {
                    Remove(format, "MM", culture.DateTimeFormat.DateSeparator);
                    Remove(format, "M", culture.DateTimeFormat.DateSeparator);
                }
                if ((Skip & DateTimeSkips.Day) != 0)
                {
                    Remove(format, "dd", culture.DateTimeFormat.DateSeparator);
                    Remove(format, "d", culture.DateTimeFormat.DateSeparator);
                }
                if ((Skip & DateTimeSkips.Hour) != 0)
                {
                    Remove(format, "HH", culture.DateTimeFormat.TimeSeparator);
                    Remove(format, "H", culture.DateTimeFormat.TimeSeparator);
                    Remove(format, "hh", culture.DateTimeFormat.TimeSeparator);
                    Remove(format, "h", culture.DateTimeFormat.TimeSeparator);
                    Remove(format, "tt", culture.DateTimeFormat.TimeSeparator);
                }
                if ((Skip & DateTimeSkips.Ms) != 0)
                {
                    Remove(format, ".fff", culture.DateTimeFormat.TimeSeparator);
                }
                else if (format.ToString().IndexOf(".fff") == -1)
                {
                    format.Replace("ss", "ss.fff");
                }
                if ((Skip & DateTimeSkips.Second) != 0)
                {
                    Remove(format, "ss", culture.DateTimeFormat.TimeSeparator);
                }
                else if (format.ToString().IndexOf("ss") == -1)
                {
                    format.Replace("mm", "mm" + culture.DateTimeFormat.TimeSeparator + "ss");
                }
                if ((Skip & DateTimeSkips.Minute) != 0)
                {
                    Remove(format, "mm", culture.DateTimeFormat.TimeSeparator);
                    Remove(format, "m", culture.DateTimeFormat.TimeSeparator);
                }
                return Value.LocalDateTime.ToString(format.ToString());
            }
            return Value.LocalDateTime.ToString();
        }

        private static int GetSeconds(DateTime start, GXDateTime value)
        {
            int ret = 0;
            if ((value.Skip & DateTimeSkips.Second) != 0)
            {
                return value.Value.Second;
            }
            else if ((value.Skip & DateTimeSkips.Minute) != 0)
            {
                ret = value.Value.Second;
                ret -= start.Second;
            }
            else if ((value.Skip & DateTimeSkips.Hour) != 0)
            {
                ret = (60 * value.Value.Minute) + value.Value.Second;
                ret -= (60 * start.Minute) + start.Second;
            }
            else if ((value.Skip & DateTimeSkips.Day) != 0)
            {
                ret = (60 * 60 * value.Value.Hour) + (60 * value.Value.Minute) + value.Value.Second;
                ret -= (60 * 60 * start.Hour) + (60 * start.Minute) + start.Second;
            }
            else if ((value.Skip & DateTimeSkips.Month) != 0)
            {
                ret = 1 * ((60 * 60 * value.Value.Hour) + (60 * value.Value.Minute) + value.Value.Second);
                ret -= 1 * ((60 * 60 * start.Hour) + (60 * start.Minute) + start.Second);
            }
            else if ((value.Skip & DateTimeSkips.Year) != 0)
            {
                DateTime tmp = value.Value.DateTime;
                tmp = tmp.AddYears(start.Year - tmp.Year);
                ret = (int)(tmp - start).TotalSeconds;
            }
            return ret;
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
                int seconds = GetSeconds(start, value);
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
                    start = start.AddSeconds(1);
                    list.Add(start);
                }
                else if ((value.Skip & DateTimeSkips.Minute) != 0)
                {
                    if (pos != 0 && seconds == 0)
                    {
                        start = start.AddMinutes(1);
                    }
                    else
                    {
                        if (seconds < 0)
                        {
                            start = start.AddMinutes(1);
                        }
                        start = start.AddSeconds(seconds);
                    }
                    list.Add(start);
                }
                else if ((value.Skip & DateTimeSkips.Hour) != 0)
                {
                    if (pos != 0 && seconds == 0)
                    {
                        start = start.AddHours(1);
                    }
                    else
                    {
                        if (seconds < 0)
                        {
                            start = start.AddHours(1);
                        }
                        start = start.AddSeconds(seconds);
                    }
                    list.Add(start);
                }
                else if ((value.Skip & DateTimeSkips.Day) != 0)
                {
                    if (pos != 0 && seconds == 0)
                    {
                        start = start.AddDays(1);
                    }
                    else
                    {
                        if (seconds < 0)
                        {
                            start = start.AddDays(1);
                        }
                        start = start.AddSeconds(seconds);
                    }
                    list.Add(start);
                }
                else if ((value.Skip & DateTimeSkips.Month) != 0)
                {
                    if (pos != 0 && seconds == 0)
                    {
                        start = start.AddMonths(1);
                    }
                    else
                    {
                        if (seconds < 0)
                        {
                            start = start.AddMonths(1);
                        }
                        start = start.AddSeconds(seconds);
                    }
                    list.Add(start);
                }
                else if ((value.Skip & DateTimeSkips.Year) != 0)
                {
                    if (pos != 0 && seconds == 0)
                    {
                        start = start.AddYears(1);
                    }
                    else
                    {
                        if (seconds < 0)
                        {
                            start = start.AddYears(1);
                        }
                        start = start.AddSeconds(seconds);
                    }
                    list.Add(start);
                }
                else
                {
                    start = start.Add(value - start);
                    list.Add(start);
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
            if (date == DateTime.MinValue)
            {
                return Convert.ToInt64(0);
            }
            if (date == DateTime.MaxValue)
            {
                return 0xFFFFFFFF;
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
            return (uint)GXDateTime.ToUnixTime(this.Value.DateTime);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return (ulong)GXDateTime.ToUnixTime(this.Value.DateTime);
        }

        #endregion      

        /// <summary>
        /// Compare to date time.
        /// </summary>
        /// <param name="value">Date and time.</param>
        /// <returns>Zero if values are equal, -1 if value is bigger and - if value is smaller.</returns>
        public int Compare(DateTime value)
        {
            int ret = 0;
            if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Year) == 0 &&
                Value.Month != value.Year)
            {
                ret = Value.Month < value.Year ? -1 : 1;
            }
            else if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Month) == 0 &&
                Value.Month != value.Month)
            {
                ret = Value.Month < value.Month ? -1 : 1;
            }
            else if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Day) == 0 &&
                Value.Day != value.Day)
            {
                ret = Value.Day < value.Day ? -1 : 1;
            }
            else if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Hour) == 0 &&
                Value.Hour != value.Hour)
            {
                ret = Value.Hour < value.Hour ? -1 : 1;
            }
            else if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Minute) == 0 &&
                Value.Minute != value.Minute)
            {
                ret = Value.Minute < value.Minute ? -1 : 1;
            }
            else if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Second) == 0 &&
                Value.Second != value.Second)
            {
                ret = Value.Second < value.Second ? -1 : 1;
            }
            return ret;
        }
    }
}
