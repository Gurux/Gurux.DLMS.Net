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
    /// GXTimeOSOS is used to write time as a octet string.
    /// </summary>
    public class GXTimeOS : GXTime
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXTimeOS()
            : base()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXTimeOS(DateTime value)
            : base(value)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXTimeOS(GXDateTime value)
            : base(value)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXTimeOS(int hour, int minute, int second, int millisecond)
            : base(hour, minute, second, millisecond)
        {
        }

        public GXTimeOS(string time)
            : base(time, CultureInfo.CurrentCulture)
        {
        }

        public GXTimeOS(string time, CultureInfo culture)
            : base(time, culture)
        {
        }
    }
}
