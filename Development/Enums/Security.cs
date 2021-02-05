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

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// Enumerates security policy for version 0.
    /// </summary>
    public enum Security
    {
        /// <summary>
        /// Transport security is not used.
        /// </summary>
        None = 0,
        /// <summary>
        /// Authentication security is used.
        /// </summary>
        Authentication = 0x10,
        /// <summary>
        /// Encryption security is used.
        /// </summary>
        Encryption = 0x20,
        /// <summary>
        /// Authentication and Encryption security are used.
        /// </summary>
        AuthenticationEncryption = 0x30,
        /// <summary>
        /// The Ephemeral Unified Model scheme is used.
        /// </summary>
        EphemeralUnifiedModel = 0x40,
        /// <summary>
        /// The One-Pass Diffie-Hellman scheme is used;
        /// </summary>
        OnePassDiffieHellman = 0x50,
        /// <summary>
        /// The Static Unified Model scheme is used.
        /// </summary>
        StaticUnifiedModel = 0x60
    }
}
