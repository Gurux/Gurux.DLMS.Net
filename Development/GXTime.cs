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
using System.Globalization;

namespace Gurux.DLMS
{
    /// <summary>
    /// This class is used because in COSEM object model some fields from date time can be ignored.
    /// Default behavior of DateTime do not allow this.
    /// </summary>
    public class GXTime : GXDateTime
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXTime()
            : base()
        {
            Skip = DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.DayOfWeek;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXTime(DateTime value)
            : base(value)
        {
            Skip = DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.DayOfWeek;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXTime(GXDateTime value)
            : base(value)
        {
            Skip = value.Skip | DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.DayOfWeek;
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public GXTime(int hour, int minute, int second, int millisecond)
            : base(-1, -1, -1, hour, minute, second, millisecond)
        {
        }

        public GXTime(string time)
            : this(time, CultureInfo.CurrentCulture)
        {
        }

        public GXTime(string time, CultureInfo culture)
            : base()
        {
            Skip = DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.DayOfWeek;
            if (time != null)
            {
                int year = 2000, month = 1, day = 1, hour = 0, min = 0, sec = 0;
#if !WINDOWS_UWP
                string dateSeparator = culture.DateTimeFormat.DateSeparator, timeSeparator = culture.DateTimeFormat.TimeSeparator;
                List<string> shortDatePattern = new List<string>(culture.DateTimeFormat.ShortDatePattern.Split(new string[] { CultureInfo.InvariantCulture.DateTimeFormat.DateSeparator, dateSeparator, " " }, StringSplitOptions.RemoveEmptyEntries));
#else
                //In UWP Standard Date and Time Format Strings are used.
                string dateSeparator = Internal.GXCommon.GetDateSeparator(), timeSeparator = Internal.GXCommon.GetTimeSeparator();
                List<string> shortDatePattern = new List<string>("yyyy-MM-dd".Split(new string[] { dateSeparator, " " }, StringSplitOptions.RemoveEmptyEntries));
#endif
                List<string> shortTimePattern = new List<string>(culture.DateTimeFormat.LongTimePattern.Split(new string[] { CultureInfo.InvariantCulture.DateTimeFormat.TimeSeparator, timeSeparator, " ", "." }, StringSplitOptions.RemoveEmptyEntries));
                string[] values = time.Trim().Split(new string[] { dateSeparator, timeSeparator, " " }, StringSplitOptions.None);
                int cnt = shortTimePattern.Count;
                if (cnt > values.Length)
                {
                    cnt = values.Length;
                }
                for (int pos = 0; pos != cnt; ++pos)
                {
                    bool skip = false;
                    if (values[pos] == "*")
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
                            hour = int.Parse(values[pos]);
                        }
                        if (!string.IsNullOrEmpty(culture.DateTimeFormat.PMDesignator))
                        {
                            if (time.IndexOf(culture.DateTimeFormat.PMDesignator) != -1)
                            {
                                hour += 12;
                            }
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
                            min = int.Parse(values[pos]);
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
                            sec = int.Parse(values[pos]);
                        }
                    }
                    else
                    {
                        //This is OK. There might be some extra in some cultures.
                    }
                }
                DateTime dt = culture.Calendar.ToDateTime(year, month, day, hour, min, sec, 0);
                Value = new DateTimeOffset(dt, TimeZoneInfo.Local.GetUtcOffset(dt));
            }
        }
    }
}
