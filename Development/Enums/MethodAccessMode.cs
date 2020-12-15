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
    /// The MethodAccessMode enumerates the method access modes.
    /// </summary>
    [Flags]
    public enum MethodAccessMode
    {
        /// <summary>
        /// Client can't use method.
        /// </summary>
        [XmlEnum("0")]
        NoAccess = 0x0,
        /// <summary>
        /// Method is allowed to use.
        /// </summary>
        [XmlEnum("1")]
        Access = 0x1,
        /// <summary>
        /// Authenticated access is allowed.
        /// </summary>
        [XmlEnum("2")]
        AuthenticatedAccess = 0x2,
        /// <summary>
        /// Authenticated request is allowed.
        /// </summary>
        [XmlEnum("4")]
        AuthenticatedRequest = 0x4,
        /// <summary>
        /// Encrypted request is allowed.
        /// </summary>
        [XmlEnum("8")]
        EncryptedRequest = 0x8,
        /// <summary>
        /// Digitally signed request is allowed.
        /// </summary>
        [XmlEnum("16")]
        DigitallySignedRequest = 0x10,
        /// <summary>
        /// Authenticated response is allowed.
        /// </summary>
        [XmlEnum("32")]
        AuthenticatedResponse = 0x20,
        /// <summary>
        /// Encrypted response is allowed.
        /// </summary>
        [XmlEnum("64")]
        EncryptedResponse = 0x40,
        /// <summary>
        /// Digitally signed response is allowed.
        /// </summary>
        [XmlEnum("128")]
        DigitallySignedResponse = 0x80
    }
}
