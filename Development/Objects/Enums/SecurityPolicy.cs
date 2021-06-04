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
    /// Security policy Enforces authentication and/or encryption algorithm provided with security suite version 0.
    /// </summary>
    [Flags]
    public enum SecurityPolicy : byte
    {
        /// <summary>
        /// Security is not used.
        /// </summary>
        None = 0,
        /// <summary>
        /// All messages are authenticated using Security Suite 0.
        /// </summary>
        Authenticated = 0x1,
        /// <summary>
        /// All messages are encrypted using Security Suite 0.
        /// </summary>
        Encrypted = 0x2,
        /// <summary>
        /// All messages are authenticated and encrypted using Security Suite 0.
        /// </summary>
        AuthenticatedEncrypted = 0x3
    }
}
