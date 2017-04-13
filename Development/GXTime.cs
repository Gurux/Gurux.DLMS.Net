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
            : base()
        {
            Skip = DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.DayOfWeek;
            if (time != null)
            {
                int hour = 0, min = 0, sec = 0;
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentUICulture;
                List<string> shortTimePattern = new List<string>(culture.DateTimeFormat.LongTimePattern.Split(new string[] { culture.DateTimeFormat.TimeSeparator }, StringSplitOptions.RemoveEmptyEntries));
                string[] values = time.Split(new string[] { culture.DateTimeFormat.TimeSeparator }, StringSplitOptions.None);
                if (shortTimePattern.Count != values.Length)
                {
                    throw new ArgumentOutOfRangeException("Invalid Time");
                }
                for (int pos = 0; pos != shortTimePattern.Count; ++pos)
                {
                    bool skip = false;
                    if (values[pos] == "*")
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
                            hour = int.Parse(values[pos]);
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
                            min = int.Parse(values[pos]);
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
                            sec = int.Parse(values[pos]);
                        }
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Invalid Time pattern.");
                    }
                }
                DateTime dt = new DateTime(2000, 1, 1, hour, min, sec);
                this.Value = new DateTimeOffset(dt, TimeZoneInfo.Local.GetUtcOffset(dt));
            }
        }
    }
}
