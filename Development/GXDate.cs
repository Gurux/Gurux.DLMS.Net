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
using System.Globalization;

namespace Gurux.DLMS
{
    /// <summary>
    /// This class is used because in COSEM object model some fields from date time can be ignored.
    /// Default behavior of DateTime do not allow this.
    /// </summary>
    public class GXDate : GXDateTime
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDate()
            : base()
        {
            Skip = DateTimeSkips.Hour | DateTimeSkips.Minute | DateTimeSkips.Second | DateTimeSkips.Ms;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDate(DateTime value)
            : base(value)
        {
            Skip = DateTimeSkips.Hour | DateTimeSkips.Minute | DateTimeSkips.Second | DateTimeSkips.Ms;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDate(GXDateTime value)
            : base(value)
        {
            Skip = value.Skip | DateTimeSkips.Hour | DateTimeSkips.Minute | DateTimeSkips.Second | DateTimeSkips.Ms;
            Extra = value.Extra;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="date">Date string.</param>
        public GXDate(string date)
            : this(date, CultureInfo.CurrentCulture)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="date">Date string.</param>
        public GXDate(string date, System.Globalization.CultureInfo culture)
            : base(date, culture)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDate(int year, int month, int day)
            : base(year, month, day, -1, -1, -1, -1)
        {
        }
    }
}
