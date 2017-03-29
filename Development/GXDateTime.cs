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
using System.Linq;
using System.Text;
using Gurux.DLMS.Enums;
using System.ComponentModel;

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
