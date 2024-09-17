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
    /// The AccessMode3 enumerates the access modes for Logical Name Association version 3.
    /// </summary>
    [Flags]
    public enum AccessMode3
    {
        /// <summary>
        /// No access.
        /// </summary>
        [XmlEnum("0")]
        NoAccess = 0,
        /// <summary>
        /// The client is allowed only reading from the server.
        /// This is used in version 1. 
        /// </summary>
        [XmlEnum("1")]
        Read = 1,
        /// <summary>
        /// The client is allowed only writing to the server.
        /// </summary>
        [XmlEnum("2")]
        Write = 2,
        /// <summary>
        /// Request messages are authenticated.
        /// </summary>
        [XmlEnum("4")]
        AuthenticatedRequest = 4,
        /// <summary>
        /// Request messages are encrypted.
        /// </summary>
        [XmlEnum("8")]
        EncryptedRequest = 8,
        /// <summary>
        /// Request messages are digitally signed.
        /// </summary>
        [XmlEnum("16")]
        DigitallySignedRequest = 16,
        /// <summary>
        /// Response messages are authenticated.
        /// </summary>
        [XmlEnum("32")]
        AuthenticatedResponse = 32,
        /// <summary>
        /// Response messages are encrypted.
        /// </summary>
        [XmlEnum("64")]
        EncryptedResponse = 64,
        /// <summary>
        /// Response messages are digitally signed.
        /// </summary>
        [XmlEnum("128")]
        DigitallySignedResponse = 128
    }
}