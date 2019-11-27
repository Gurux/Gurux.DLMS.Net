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
    /// Date time extra info.
    /// </summary>
    [Flags]
    public enum DateTimeExtraInfo
    {
        /// <summary>
        /// No extra info.
        /// </summary>
        [XmlEnum("0")]
        None = 0x0,
        /// <summary>
        /// Daylight savings begin.
        /// </summary>
        [XmlEnum("1")]
        DstBegin = 0x1,
        /// <summary>
        ///Daylight savings end.
        /// </summary>
        [XmlEnum("2")]
        DstEnd = 0x2,
        /// <summary>
        /// Last day of month.
        /// </summary>
        [XmlEnum("4")]
        LastDay = 0x4,
        /// <summary>
        /// 2nd last day of month
        /// </summary>
        [XmlEnum("8")]
        LastDay2 = 0x8,
    }
}
