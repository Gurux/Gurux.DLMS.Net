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

namespace Gurux.DLMS.Objects.Enums
{
    /// <summary>
    /// Defines the weekdays.
    /// </summary>
    [Flags]
    public enum Weekdays
    {
        /// <summary>
        /// No day of week is selected.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates Monday.
        /// </summary>
        Monday = 0x1,
        /// <summary>
        /// Indicates Tuesday.
        /// </summary>
        Tuesday = 0x2,
        /// <summary>
        /// Indicates Wednesday.
        /// </summary>
        Wednesday = 0x4,
        /// <summary>
        /// Indicates Thursday.
        /// </summary>
        Thursday = 0x8,
        /// <summary>
        /// Indicates Friday.
        /// </summary>
        Friday = 0x10,
        /// <summary>
        /// Indicates Saturday.
        /// </summary>
        Saturday = 0x20,
        /// <summary>
        /// Indicates Sunday.
        /// </summary>
        Sunday = 0x40
    }
}
