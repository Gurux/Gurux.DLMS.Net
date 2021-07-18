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
        [XmlEnum("0")]
        None = 0,
        /// <summary>
        /// All messages are authenticated using Security Suite 0.
        /// </summary>
        [XmlEnum("1")]
        Authenticated = 0x1,
        /// <summary>
        /// All messages are encrypted using Security Suite 0.
        /// </summary>
        [XmlEnum("2")]
        Encrypted = 0x2,
        /// <summary>
        /// All messages are authenticated and encrypted using Security Suite 0.
        /// </summary>
        [XmlEnum("3")]
        AuthenticatedEncrypted = 0x3,
        /// <summary>
        /// Request is authenticated.
        /// </summary>
        /// <remarks>
        /// This is used in Security Suite 1.
        /// </remarks>
        [XmlEnum("4")]
        AuthenticatedRequest = 0x4,
        /// <summary>
        /// Request is encrypted.
        /// </summary>
        /// <remarks>
        /// This is used in Security Suite 1.
        /// </remarks>
        [XmlEnum("8")]
        EncryptedRequest = 0x8,
        /// <summary>
        /// Request is digitally signed.
        /// </summary>
        /// <remarks>
        /// This is used in Security Suite 1.
        /// </remarks>
        [XmlEnum("16")]
        DigitallySignedRequest = 0x10,

        /// <summary>
        /// Response is authenticated.
        /// </summary>
        /// <remarks>
        /// This is used in Security Suite 1.
        /// </remarks>
        [XmlEnum("32")]
        AuthenticatedResponse = 0x20,

        /// <summary>
        /// Response is encrypted.
        /// </summary>
        /// <remarks>
        /// This is used in Security Suite 1.
        /// </remarks>
        [XmlEnum("64")]
        EncryptedResponse = 0x40,

        /// <summary>
        /// Response is digitally signed.
        /// </summary>
        /// <remarks>
        /// This is used in Security Suite 1.
        /// </remarks>
        [XmlEnum("128")]
        DigitallySignedResponse = 0x80
    }
}
