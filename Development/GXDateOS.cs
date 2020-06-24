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
    /// GXDateOS is used to write date as a octet string.
    /// </summary>
    public class GXDateOS : GXDate
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateOS()
            : base()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateOS(DateTime value)
            : base(value)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateOS(GXDateTime value)
            : base(value)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="date">Date string.</param>
        public GXDateOS(string date)
            : base(date, CultureInfo.CurrentCulture)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="date">Date string.</param>
        public GXDateOS(string date, System.Globalization.CultureInfo culture)
            : base(date, culture)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDateOS(int year, int month, int day)
            : base(year, month, day)
        {
        }
    }
}
