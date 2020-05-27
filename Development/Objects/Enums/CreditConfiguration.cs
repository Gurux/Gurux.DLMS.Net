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
    /// Enumerated Credit configuration values.
    /// </summary>
    [Flags]
    public enum CreditConfiguration : byte
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// Requires visual indication,
        /// </summary>
        Visual = 0x1,
        /// <summary>
        /// Requires confirmation before it can be selected/invoked
        /// </summary>
        Confirmation = 0x2,
        /// <summary>
        /// Requires the credit amount to be paid back.
        /// </summary>
        PaidBack = 0x4,
        /// <summary>
        /// Resettable.
        /// </summary>
        Resettable = 0x8,
        /// <summary>
        /// Able to receive credit amounts from tokens.
        /// </summary>
        Tokens = 0x10
    }
}
