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
namespace Gurux.DLMS.Objects.Enums
{
    /// <summary>
    /// Application context name.
    /// </summary>
    public enum ApplicationContextName : byte
    {
        /// <summary>
        /// Invalid application context name.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Logical name.
        /// </summary>
        LogicalName = 1,
        /// <summary>
        /// Short name.
        /// </summary>
        ShortName = 2,
        /// <summary>
        /// Logical name with ciphering.
        /// </summary>
        LogicalNameWithCiphering = 3,
        /// <summary>
        /// Short name with ciphering.
        /// </summary>
        ShortNameWithCiphering = 4
    }
}
