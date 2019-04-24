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
using System.Xml.Serialization;

namespace Gurux.DLMS.Enums
{   
    /// <summary>
    /// Enumerates skipped fields from date time.
    /// </summary>
    [Flags]
    public enum DateTimeSkips
    {
        /// <summary>
        /// Nothing is skipped from date time.
        /// </summary>
        [XmlEnum("0")]
        None = 0x0,
        /// <summary>
        /// Year part of date time is skipped.
        /// </summary>
        [XmlEnum("1")]
        Year = 0x1,
        /// <summary>
        /// Month part of date time is skipped.
        /// </summary>
        [XmlEnum("2")]
        Month = 0x2,
        /// <summary>
        /// Day part is skipped.
        /// </summary>
        [XmlEnum("4")]
        Day = 0x4,
        /// <summary>
        /// Day of week part of date time is skipped.
        /// </summary>
        [XmlEnum("8")]
        DayOfWeek = 0x8,
        /// <summary>
        /// Hours part of date time is skipped.
        /// </summary>
        [XmlEnum("16")]
        Hour = 0x10,
        /// <summary>
        /// Minute part of date time is skipped.
        /// </summary>
        [XmlEnum("32")]
        Minute = 0x20,
        /// <summary>
        /// Seconds part of date time is skipped.
        /// </summary>
        [XmlEnum("64")]
        Second = 0x40,
        /// <summary>
        /// Hundreds of seconds part of date time is skipped.
        /// </summary>
        [XmlEnum("128")]
        Ms = 0x80,
        /// <summary>
        /// Deviation is skipped on write.
        /// </summary>
        [XmlEnum("256")]
        Deviation = 0x100,
        /// <summary>
        /// Status is skipped on write.
        /// </summary>
        [XmlEnum("512")]
        Status = 0x200
    }
}
