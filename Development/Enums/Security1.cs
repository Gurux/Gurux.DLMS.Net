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

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// Enumerates security policy for version 1.
    /// </summary>
    [Flags]
    public enum Security1
    {
        /// <summary>
        /// Transport security is not used.
        /// </summary>
        None = 0,
        /// <summary>
        /// Request messages are authenticated.
        /// </summary>
        AuthenticatedRequest = 0x4,
        /// <summary>
        /// Request messages are encrypted.
        /// </summary>
        EncryptedRequest = 0x8,
        /// <summary>
        /// Request messages are digitally signed.
        /// </summary>
        DigitallySignedRequest = 0x10,
        /// <summary>
        /// Response messages are authenticated.
        /// </summary>
        AuthenticatedResponse = 0x20,
        /// <summary>
        /// Response messages are encrypted.
        /// </summary>
        EncryptedResponse = 0x40,
        /// <summary>
        /// Response messages are digitally signed.
        /// </summary>
        DigitallySignedResponse = 0x80
    }
}
