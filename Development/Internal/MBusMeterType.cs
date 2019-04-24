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


namespace Gurux.DLMS.Internal
{
    /// <summary>
    /// M-Bus meter type.
    /// </summary>
    public enum MBusMeterType
    {
        /// <summary>
        /// Oil meter.
        /// </summary>
        Oil = 1,
        /// <summary>
        /// Energy meter.
        /// </summary>
        Energy = 2,
        /// <summary>
        /// Gas meter.
        /// </summary>
        Gas = 3,
        /// <summary>
        /// Water meter.
        /// </summary>
        Water = 7,
        /// <summary>
        /// Unknown meter type.
        /// </summary>
        Unknown = 0x0F
    }
}
