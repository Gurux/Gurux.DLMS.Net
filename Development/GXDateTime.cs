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
        public GXDateTime(DateTime value)
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
                if (value.Kind == DateTimeKind.Utc)
                {
                    Value = new DateTimeOffset(value, TimeZoneInfo.Utc.GetUtcOffset(value));
                }
                else
                {
                    Value = new DateTimeOffset(value, TimeZoneInfo.Local.GetUtcOffset(value));
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Date time value as a string.</param>
        public GXDateTime(string value)
            : base()
        {
            if (value != null)
            {
                int year = 2000, month = 1, day = 1, hour = 0, min = 0, sec = 0;
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentUICulture;
                List<string> shortDatePattern = new List<string>(culture.DateTimeFormat.ShortDatePattern.Split(new string[] { culture.DateTimeFormat.DateSeparator }, StringSplitOptions.RemoveEmptyEntries));
                List<string> shortTimePattern = new List<string>(culture.DateTimeFormat.LongTimePattern.Split(new string[] { culture.DateTimeFormat.TimeSeparator }, StringSplitOptions.RemoveEmptyEntries));
                string[] values = value.Trim().Split(new string[] { culture.DateTimeFormat.DateSeparator, culture.DateTimeFormat.TimeSeparator, " " }, StringSplitOptions.None);
                if (shortDatePattern.Count != values.Length && shortDatePattern.Count + shortTimePattern.Count != values.Length)
                {
                    throw new ArgumentOutOfRangeException("Invalid DateTime");
                }
                for (int pos = 0; pos != shortDatePattern.Count; ++pos)
                {
                    bool skip = false;
                    if (values[pos] == "*")
                    {
                        skip = true;
                    }
                    if (shortDatePattern[pos] == "yyyy")
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
                    else if (string.Compare(shortDatePattern[pos], "M", true) == 0)
                    {
                        if (skip)
                        {
                            Skip |= DateTimeSkips.Month;
                        }
                        else
                        {
                            month = int.Parse(values[pos]);
                        }
                    }
                    else if (string.Compare(shortDatePattern[pos], "d", true) == 0)
                    {
                        if (skip)
                        {
                            Skip |= DateTimeSkips.Day;
                        }
                        else
                        {
                            day = int.Parse(values[pos]);
                        }
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Invalid Date time pattern.");
                    }
                }
                if (values.Length > 3)
                {
                    for (int pos = 0; pos != shortTimePattern.Count; ++pos)
                    {
                        bool skip = false;
                        if (values[3 + pos] == "*")
                        {
                            skip = true;
                        }
                        if (string.Compare(shortTimePattern[pos], "h", true) == 0)
                        {
                            if (skip)
                            {
                                Skip |= DateTimeSkips.Hour;
                            }
                            else
                            {
                                hour = int.Parse(values[3 + pos]);
                            }
                        }
                        else if (string.Compare(shortTimePattern[pos], "mm", true) == 0)
                        {
                            if (skip)
                            {
                                Skip |= DateTimeSkips.Minute;
                            }
                            else
                            {
                                min = int.Parse(values[3 + pos]);
                            }
                        }
                        else if (string.Compare(shortTimePattern[pos], "ss", true) == 0)
                        {
                            if (skip)
                            {
                                Skip |= DateTimeSkips.Second;
                            }
                            else
                            {
                                sec = int.Parse(values[3 + pos]);
                            }
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("Invalid Date time pattern.");
                        }
                    }
                }
                DateTime dt = new DateTime(year, month, day, hour, min, sec);
                this.Value = new DateTimeOffset(dt, TimeZoneInfo.Local.GetUtcOffset(dt));
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateTime(DateTimeOffset value)
        {
            Value = value;
        }

        /// <summary>
        /// Convert DateTime to GXDateTime.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator GXDateTime(DateTime value)
        {
            return new GXDateTime(value);
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
                Skip |= DateTimeSkips.Year;
                year = 2;
            }
            DaylightSavingsBegin = month == 0xFE;
            DaylightSavingsEnd = month == 0xFD;
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
                Value = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Local);
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
        public DateTimeSkips Skip
        {
            get;
            set;
        }

        /// <summary>
        /// Daylight savings begin.
        /// </summary>
        public bool DaylightSavingsBegin
        {
            get;
            set;
        }

        /// <summary>
        /// Daylight savings end.
        /// </summary>
        public bool DaylightSavingsEnd
        {
            get;
            set;
        }

        /// <summary>
        /// Status of the clock.
        /// </summary>
        public ClockStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// Is data time serialized as octet string. Default value is false.
        /// </summary>
        ///<remarks>
        ///Meters are serializing date, time and date time usually as octet string.
        ///Some meters want to serialize date, time and date time using own data structure.
        ///</remarks>
        public bool SerializeUsingOwnType
        {
            get;
            set;
        }

        public string ToFormatString()
        {
            int pos;
            if (Skip != DateTimeSkips.None)
            {
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentUICulture;
                List<string> shortDatePattern = new List<string>(culture.DateTimeFormat.ShortDatePattern.Split(new string[] { culture.DateTimeFormat.DateSeparator }, StringSplitOptions.RemoveEmptyEntries));
                List<string> shortTimePattern = new List<string>(culture.DateTimeFormat.LongTimePattern.Split(new string[] { culture.DateTimeFormat.TimeSeparator }, StringSplitOptions.RemoveEmptyEntries));
                if (this is GXTime)
                {
                    shortDatePattern.Clear();
                }
                else
                {
                    if ((Skip & DateTimeSkips.Year) != 0)
                    {
                        pos = shortDatePattern.IndexOf("yyyy");
                        shortDatePattern[pos] = "*";
                    }
                    if ((Skip & DateTimeSkips.Month) != 0)
                    {
                        pos = shortDatePattern.IndexOf("M");
                        shortDatePattern[pos] = "*";
                    }
                    if ((Skip & DateTimeSkips.Day) != 0)
                    {
                        pos = shortDatePattern.IndexOf("d");
                        shortDatePattern[pos] = "*";
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
                    format = string.Join(culture.DateTimeFormat.DateSeparator, shortDatePattern.ToArray());
                }
                if (shortTimePattern.Count != 0)
                {
                    if (format != null)
                    {
                        format += " ";
                    }
                    format += string.Join(culture.DateTimeFormat.TimeSeparator, shortTimePattern.ToArray());
                }
                if (format == "H")
                {
                    return Value.Hour.ToString();
                }
                if (format == null)
                {
                    return "";
                }
                return Value.LocalDateTime.ToString(format);
            }
            return Value.LocalDateTime.ToString();
        }

        public override string ToString()
        {
            if (Skip != DateTimeSkips.None)
            {
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentUICulture;
                List<string> shortDatePattern = new List<string>(culture.DateTimeFormat.ShortDatePattern.Split(new string[] { culture.DateTimeFormat.DateSeparator }, StringSplitOptions.RemoveEmptyEntries));
                List<string> shortTimePattern = new List<string>(culture.DateTimeFormat.LongTimePattern.Split(new string[] { culture.DateTimeFormat.TimeSeparator }, StringSplitOptions.RemoveEmptyEntries));
                if ((Skip & DateTimeSkips.Year) != 0)
                {
                    shortDatePattern.Remove("yyyy");
                }
                if ((Skip & DateTimeSkips.Month) != 0)
                {
                    shortDatePattern.Remove("M");
                }
                if ((Skip & DateTimeSkips.Day) != 0)
                {
                    shortDatePattern.Remove("d");
                }
                if ((Skip & DateTimeSkips.Hour) != 0)
                {
                    shortTimePattern.Remove("H");
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
                    format = string.Join(culture.DateTimeFormat.DateSeparator, shortDatePattern.ToArray());
                }
                if (shortTimePattern.Count != 0)
                {
                    if (format != null)
                    {
                        format += " ";
                    }
                    format += string.Join(culture.DateTimeFormat.TimeSeparator, shortTimePattern.ToArray());
                }
                if (format == "H")
                {
                    return Value.Hour.ToString();
                }
                if (format == null)
                {
                    return "";
                }
                return Value.LocalDateTime.ToString(format);
            }
            return Value.LocalDateTime.ToString();
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
