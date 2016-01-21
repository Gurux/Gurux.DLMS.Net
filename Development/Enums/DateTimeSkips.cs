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

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// DataType enumerates skipped fields from date time.
    /// </summary>
    public enum DateTimeSkips
    {
        /// <summary>
        /// Nothing is skipped from date time.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Year part of date time is skipped.
        /// </summary>
        Year = 0x1,
        /// <summary>
        /// Month part of date time is skipped.
        /// </summary>
        Month = 0x2,
        /// <summary>
        /// Day part is skipped.
        /// </summary>
        Day = 0x4,
        /// <summary>
        /// Day of week part of date time is skipped.
        /// </summary>
        DayOfWeek = 0x8,
        /// <summary>
        /// Hours part of date time is skipped.
        /// </summary>
        Hour = 0x10,
        /// <summary>
        /// Minute part of date time is skipped.
        /// </summary>
        Minute = 0x20,
        /// <summary>
        /// Seconds part of date time is skipped.
        /// </summary>
        Second = 0x40,
        /// <summary>
        /// Hundreds of seconds part of date time is skipped.
        /// </summary>
        Ms = 0x80,
        /// <summary>
        /// Devitation is skipped on write.
        /// </summary>
        Devitation = 0x100        
    }
}
