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
using Gurux.DLMS.Enums;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{
    /// <summary>
    /// This class is used because in COSEM object model some fields from date time can be ignored.
    /// Default behavior of DateTime do not allow this.
    /// </summary>
    [TypeConverter(typeof(GXDateTimeConverter))]
    public class GXDateTime : IConvertible
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateTime() : this(DateTime.MinValue, null)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateTime(GXDateTime value) : this(value, null)
        {
            if (value != null)
            {
                Skip = value.Skip;
                Extra = value.Extra;
            }
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
            DayOfWeek = 0xFF;
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
            : this(value, CultureInfo.CurrentCulture)
        {
        }

        private static bool IsNumeric(char value)
        {
            return value >= '0' && value <= '9';
        }

        private static string GetDateTimeFormat(CultureInfo culture)
        {
            string str = culture.DateTimeFormat.ShortDatePattern + " " + culture.DateTimeFormat.LongTimePattern;
#if !WINDOWS_UWP
            foreach (string it in culture.DateTimeFormat.GetAllDateTimePatterns())
            {
                if (!it.Contains("dddd") && it.Contains(culture.DateTimeFormat.ShortDatePattern) && it.Contains(culture.DateTimeFormat.LongTimePattern))
                {
                    return it;
                }
            }
#endif //!WINDOWS_UWP
            return str;
        }

        /// <summary>
        /// Check is time zone included and return index of time zone.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int TimeZonePosition(string value)
        {
            if (value.Length > 5)
            {
                int pos = value.Length - 6;
                char sep = value[pos];
                if (sep == '-' || sep == '+')
                {
                    return pos;
                }
            }
            return -1;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Date time value as a string.</param>
        /// <param name="culture">Used culture.</param>
        public GXDateTime(string value, CultureInfo culture)
            : base()
        {
            bool addTimeZone = true;
            DayOfWeek = 0xFF;
            if (!string.IsNullOrEmpty(value))
            {
                StringBuilder format = new StringBuilder();
                format.Append(GetDateTimeFormat(culture));
                Remove(format, culture);
                if (value.IndexOf("BEGIN") != -1)
                {
                    Extra |= DateTimeExtraInfo.DstBegin;
                    value = value.Replace("BEGIN", "01");
                }
                if (value.IndexOf("END") != -1)
                {
                    Extra |= DateTimeExtraInfo.DstEnd;
                    value = value.Replace("END", "01");
                }
                if (value.IndexOf("LASTDAY2") != -1)
                {
                    Extra |= DateTimeExtraInfo.LastDay2;
                    value = value.Replace("LASTDAY2", "01");
                }
                if (value.IndexOf("LASTDAY") != -1)
                {
                    Extra |= DateTimeExtraInfo.LastDay;
                    value = value.Replace("LASTDAY", "01");
                }
                String v = value;

                if (value.IndexOf('*') != -1)
                {
                    //Day of week is not supported when date time is give as a string.
                    Skip |= DateTimeSkips.DayOfWeek;
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
                                    addTimeZone = false;
                                    Skip |= DateTimeSkips.Year;
                                }
                                else if (tmp == "M" || tmp == "MM" || tmp == "MMM")
                                {
                                    addTimeZone = false;
                                    Skip |= DateTimeSkips.Month;
                                }
                                else if (tmp.Equals("dd") || tmp.Equals("d"))
                                {
                                    addTimeZone = false;
                                    Skip |= DateTimeSkips.Day;
                                }
                                else if (tmp.Equals("h") || tmp.Equals("hh")
                                      || tmp.Equals("HH") || tmp.Equals("H"))
                                {
                                    addTimeZone = false;
                                    Skip |= DateTimeSkips.Hour;
                                    if (format.ToString().IndexOf("tt") != -1)
                                    {
                                        value += " " + culture.DateTimeFormat.AMDesignator;
                                        v += " " + culture.DateTimeFormat.AMDesignator;
                                    }
                                }
                                else if (tmp.Equals("mm") || tmp.Equals("m"))
                                {
                                    addTimeZone = false;
                                    Skip |= DateTimeSkips.Minute;
                                }
                                else if (tmp.Equals("tt"))
                                {
                                    addTimeZone = false;
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
                    // If time zone is used.
                    int pos;
                    if (addTimeZone && (pos = TimeZonePosition(value)) != -1)
                    {
                        format.Append("zzz");
                        culture = null;
                    }
                    else if (addTimeZone && value.IndexOf('Z') != -1)
                    {
                        format.Append("zzz");
                        v = v.Replace("Z", "+00:00");
                        culture = null;
                    }
                    else if (culture == CultureInfo.InvariantCulture)
                    {
                        Skip |= DateTimeSkips.Deviation;
                    }
                    if (culture == null)
                    {
                        Value = DateTimeOffset.ParseExact(v, format.ToString(), culture);
                    }
                    else
                    {
                        v = v.Replace("Z", "+00:00");
                        Value = DateTime.ParseExact(v, format.ToString(), culture);
                    }
                    Skip |= DateTimeSkips.DayOfWeek;
                    Skip |= DateTimeSkips.Ms;
                    if ((Skip & (DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.Hour | DateTimeSkips.Minute)) == 0)
                    {
                        if (Value.DateTime.IsDaylightSavingTime())
                        {
                            Status |= ClockStatus.DaylightSavingActive;
                        }
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        //Append seconds if not in the format.
                        if ((Skip & DateTimeSkips.Second) == 0 && format.ToString().IndexOf("ss") == -1)
                        {
                            format.Replace("mm", "mm.ss");
                        }
                        Value = DateTime.ParseExact(v, format.ToString().Trim(), culture);
                        Skip |= DateTimeSkips.Ms;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            //Append ms if not in the format.
                            if ((Skip & DateTimeSkips.Ms) == 0 && format.ToString().IndexOf("fff") == -1)
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
            DayOfWeek = 0xFF;
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
            DayOfWeek = 0xFF;
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

            if (day == 0xFE)
            {
                Extra |= DateTimeExtraInfo.LastDay;
                day = 1;
            }
            else if (day == 0xFD)
            {
                Extra |= DateTimeExtraInfo.LastDay2;
                day = 1;
            }
            else if (day < 1 || day > 31)
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
        [DefaultValue(-1)]
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
            return ToFormatString(culture, true);
        }

        public string ToFormatMeterString()
        {
            return ToFormatMeterString(CultureInfo.CurrentCulture);
        }
        public string ToFormatMeterString(CultureInfo culture)
        {
            return ToFormatString(culture, false);
        }

        public string ToFormatString(CultureInfo culture, bool useLocalTime)
        {
            if (Value.DateTime == DateTime.MinValue ||
                Value.DateTime == DateTime.MaxValue)
            {
                return "";
            }
            StringBuilder format = new StringBuilder();
            format.Append(GetDateTimeFormat(culture));
            Remove(format, culture);
            if (!useLocalTime && (Skip & DateTimeSkips.Deviation) == 0)
            {
                format.Append("zzz");
            }
            if ((Extra & DateTimeExtraInfo.DstBegin) != 0)
            {
                format.Replace("MMM", "BEGIN");
                format.Replace("MM", "BEGIN");
                format.Replace("M", "BEGIN");
            }
            else if ((Extra & DateTimeExtraInfo.DstEnd) != 0)
            {
                format.Replace("MMM", "END");
                format.Replace("MM", "END");
                format.Replace("M", "END");
            }
            else if ((Extra & DateTimeExtraInfo.LastDay) != 0)
            {
                format.Replace("dd", "LASTDAY");
                format.Replace("d", "LASTDAY");
            }
            else if ((Extra & DateTimeExtraInfo.LastDay2) != 0)
            {
                format.Replace("dd", "LASTDAY2");
                format.Replace("d", "LASTDAY2");
            }
            if ((Skip & DateTimeSkips.Year) != 0)
            {
                Replace(format, "yyyy");
                Replace(format, "yy");
                Remove(format, "zzz", null);
            }
            if ((Skip & DateTimeSkips.Month) != 0)
            {
                Replace(format, "MMM");
                Replace(format, "MM");
                Replace(format, "M");
                Remove(format, "zzz", null);
            }
            if ((Skip & DateTimeSkips.Day) != 0)
            {
                Replace(format, "dd");
                Replace(format, "d");
                Remove(format, "zzz", null);
            }
            if ((Skip & DateTimeSkips.Hour) != 0)
            {
                Replace(format, "HH");
                Replace(format, "H");
                Replace(format, "hh");
                Replace(format, "h");
                Remove(format, "tt", null);
                Remove(format, "zzz", null);
            }
            if ((Skip & DateTimeSkips.Ms) != 0 || Value.LocalDateTime.Millisecond == 0)
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
#if !WINDOWS_UWP
                format.Replace("mm", "mm.ss");
#else
                    format.Replace("mm", "mm.ss");
#endif //!WINDOWS_UWP
            }
            if ((Skip & DateTimeSkips.Minute) != 0)
            {
                Replace(format, "mm");
                Replace(format, "m");
                Remove(format, "zzz", null);
            }
            if (useLocalTime)
            {
                return Value.LocalDateTime.ToString(format.ToString().Trim(), culture);
            }
            string ret = Value.ToString(format.ToString().Trim(), culture);
            if (Value.DateTime == Value.UtcDateTime)
            {
                ret = ret.Substring(0, ret.Length - 6) + "Z";
            }
            return ret;
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
#if !WINDOWS_UWP
            string timeSeparator = culture.DateTimeFormat.TimeSeparator;
            string dateSeparator = culture.DateTimeFormat.DateSeparator;
#else
            string timeSeparator = ":";
            string dateSeparator = "/";
#endif //!WINDOWS_UWP
            if (this is GXDate)
            {
                Remove(format, "HH", timeSeparator);
                Remove(format, "hh", timeSeparator);
                Remove(format, "H", timeSeparator);
                Remove(format, "h", timeSeparator);
                Remove(format, "mm", timeSeparator);
                Remove(format, "m", timeSeparator);
                Remove(format, "ss", timeSeparator);
                Remove(format, "tt", timeSeparator);
                Skip |= DateTimeSkips.Hour | DateTimeSkips.Minute | DateTimeSkips.Second | DateTimeSkips.Ms;
            }
            else if (this is GXTime)
            {
                Remove(format, "yyyy", dateSeparator);
                Remove(format, "yy", dateSeparator);
                Remove(format, "MM", dateSeparator);
                Remove(format, "M", dateSeparator);
                Remove(format, "dd", dateSeparator);
                Remove(format, "d", dateSeparator);
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
            return ToString(CultureInfo.CurrentCulture, true);
        }

        /// <summary>
        /// Date time to string.
        /// </summary>
        /// <returns>Date time as a string.</returns>
        public string ToString(CultureInfo culture)
        {
            return ToString(culture, true);
        }

        /// <summary>
        /// Date time to meter string.
        /// </summary>
        /// <returns>Date time as a string.</returns>
        public string ToMeterString()
        {
            return ToString(CultureInfo.CurrentCulture, false);
        }

        /// <summary>
        /// Date time to meter string.
        /// </summary>
        /// <returns>Date time as a string.</returns>
        public string ToMeterString(CultureInfo culture)
        {
            return ToString(culture, false);
        }

        private string ToString(CultureInfo culture, bool useLocalTime)
        {
            StringBuilder format = new StringBuilder();
            if (Skip != DateTimeSkips.None)
            {
#if !WINDOWS_UWP && !__MOBILE__
                string timeSeparator = culture.DateTimeFormat.TimeSeparator;
                string dateSeparator = culture.DateTimeFormat.DateSeparator;
#else
            string timeSeparator = ":";
            string dateSeparator = "/";
#endif //!WINDOWS_UWP && !__MOBILE__
                format.Append(GetDateTimeFormat(culture));
                Remove(format, culture);
                if (!useLocalTime)
                {
                    format.Append("zzz");
                }
                if ((Skip & DateTimeSkips.Year) != 0)
                {
                    Remove(format, "yyyy", dateSeparator);
                    Remove(format, "yy", dateSeparator);
                    Remove(format, "zzz", null);
                }
                if ((Skip & DateTimeSkips.Month) != 0)
                {
                    Remove(format, "MM", dateSeparator);
                    Remove(format, "M", dateSeparator);
                    Remove(format, "zzz", null);
                }
                if ((Skip & DateTimeSkips.Day) != 0)
                {
                    Remove(format, "dd", dateSeparator);
                    Remove(format, "d", dateSeparator);
                    Remove(format, "zzz", null);
                }
                if ((Skip & DateTimeSkips.Hour) != 0)
                {
                    Remove(format, "HH", timeSeparator);
                    Remove(format, "H", timeSeparator);
                    Remove(format, "hh", timeSeparator);
                    Remove(format, "h", timeSeparator);
                    Remove(format, "tt", timeSeparator);
                    Remove(format, "zzz", null);
                }
                if ((Skip & DateTimeSkips.Ms) != 0)
                {
                    Remove(format, ".fff", timeSeparator);
                }
                else if (format.ToString().IndexOf(".fff") == -1)
                {
                    format.Replace("ss", "ss.fff");
                }
                if ((Skip & DateTimeSkips.Second) != 0)
                {
                    Remove(format, "ss", timeSeparator);
                }
                else if (format.ToString().IndexOf("ss") == -1)
                {
                    format.Replace("mm", "mm" + timeSeparator + "ss");
                }
                if ((Skip & DateTimeSkips.Minute) != 0)
                {
                    Remove(format, "mm", timeSeparator);
                    Remove(format, "m", timeSeparator);
                    Remove(format, "zzz", null);
                }
                string tmp = format.ToString().Trim();
                if (tmp == "")
                {
                    return "";
                }
                //FormatException is thrown if length of format is 1.
                if (tmp.IndexOf(dateSeparator) == -1 && tmp.IndexOf(timeSeparator) == -1)
                {
                    if ((Skip & DateTimeSkips.Year) == 0)
                    {
                        return Value.Year.ToString();
                    }
                    if ((Skip & DateTimeSkips.Month) == 0)
                    {
                        return Value.Month.ToString();
                    }
                    if ((Skip & DateTimeSkips.Day) == 0)
                    {
                        return Value.Day.ToString();
                    }
                    if ((Skip & DateTimeSkips.Hour) == 0)
                    {
                        return Value.Hour.ToString();
                    }
                    if ((Skip & DateTimeSkips.Minute) == 0)
                    {
                        return Value.Minute.ToString();
                    }
                    if ((Skip & DateTimeSkips.Second) == 0)
                    {
                        return Value.Second.ToString();
                    }
                    if ((Skip & DateTimeSkips.Ms) == 0)
                    {
                        return Value.Millisecond.ToString();
                    }
                }
                if (useLocalTime)
                {
                    return Value.LocalDateTime.ToString(tmp, culture);
                }
                string ret = Value.ToString(tmp, culture);
                if (Value.DateTime == Value.UtcDateTime)
                {
                    ret = ret.Substring(0, ret.Length - 6) + "Z";
                }
                return ret;
            }
            if (useLocalTime)
            {
                return Value.LocalDateTime.ToString(culture);
            }
            return Value.ToString(culture);
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

        /// <summary>
        /// Get date time as hex string.
        /// </summary>
        /// <param name="addSpace">Add space between bytes.</param>
        /// <param name="useMeterTimeZone">Date-Time values are shown using meter's time zone and it's not localized to use PC time.</param>
        /// <returns></returns>
        public string ToHex(bool addSpace, bool useMeterTimeZone)
        {
            GXByteBuffer buff = new GXByteBuffer();
            GXDLMSSettings settings = new GXDLMSSettings() { UseUtc2NormalTime = useMeterTimeZone };
            GXCommon.SetData(settings, buff, DataType.OctetString, this);
            //Dont add data type or length.
            return buff.ToHex(addSpace, 2);
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
            return (int)GXDateTime.ToUnixTime(this.Value.DateTime);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return (long)GXDateTime.ToUnixTime(this.Value.DateTime);
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
            DateTime localValue = Value.LocalDateTime;
            if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Year) == 0 &&
                localValue.Year != value.Year)
            {
                ret = localValue.Year < value.Year ? -1 : 1;
            }
            else if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Month) == 0 &&
                localValue.Month != value.Month)
            {
                ret = localValue.Month < value.Month ? -1 : 1;
            }
            else if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Day) == 0 &&
                localValue.Day != value.Day)
            {
                ret = localValue.Day < value.Day ? -1 : 1;
            }
            else if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Hour) == 0 &&
                localValue.Hour != value.Hour)
            {
                ret = localValue.Hour < value.Hour ? -1 : 1;
            }
            else if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Minute) == 0 &&
                localValue.Minute != value.Minute)
            {
                ret = localValue.Minute < value.Minute ? -1 : 1;
            }
            else if ((Skip & Gurux.DLMS.Enums.DateTimeSkips.Second) == 0 &&
                localValue.Second != value.Second)
            {
                ret = localValue.Second < value.Second ? -1 : 1;
            }
            return ret;
        }
    }
}
