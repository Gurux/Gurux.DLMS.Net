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
    /// Security policy Enforces authentication and/or encryption algorithm provided with security suite version 0 and 1.
    /// </summary>
    /// <remarks>
    /// Authentication, Encryption and AuthenticationEncryption flags are used inly with Security suite version 0.
    /// </remarks>
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
        AuthenticatedEncrypted = 0x3,
        /// <summary>
        /// Request is authenticated.
        /// </summary>
        AuthenticatedRequest = 0x4,
        /// <summary>
        /// Request is encrypted.
        /// </summary>
        EncryptedRequest = 0x8,
        /// <summary>
        /// Request is digitally signed.
        /// </summary>
        DigitallySignedRequest = 0x10,

        /// <summary>
        /// Response is authenticated.
        /// </summary>
        AuthenticatedResponse = 0x20,

        /// <summary>
        /// Response is encrypted.
        /// </summary>
        EncryptedResponse = 0x40,

        /// <summary>
        /// Response is digitally signed.
        /// </summary>
        DigitallySignedResponse = 0x80
    }
}
