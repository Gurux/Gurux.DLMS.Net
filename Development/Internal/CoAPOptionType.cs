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
    /// CoAP option types.
    /// </summary>
    public enum CoAPOptionType : byte
    {
        /// <summary>
        /// If-Match.
        /// </summary>
        IfMatch = 1,
        /// <summary>
        /// Uri-Host.
        /// </summary>
        UriHost = 3,
        /// <summary>
        /// ETag.
        /// </summary>
        ETag = 4,
        /// <summary>
        /// If-None-Match.
        /// </summary>
        IfNoneMatch = 5,
        /// <summary>
        /// Uri-Port.
        /// </summary>
        UriPort = 7,
        /// <summary>
        /// Location-Path.
        /// </summary>
        LocationPath = 8,
        /// <summary>
        /// Uri-Path.
        /// </summary>
        UriPath = 11,
        /// <summary>
        /// Content-Format.
        /// </summary>
        ContentFormat = 12,
        /// <summary>
        ///  Max-Age.
        /// </summary>
        MaxAge = 14,
        /// <summary>
        /// Uri-Query.
        /// </summary>
        UriQuery = 15,
        /// <summary>
        /// Accept.
        /// </summary>
        Accept = 17,
        /// <summary>
        /// Location-Query.
        /// </summary>
        LocationQuery = 20,
        /// <summary>
        /// Block2.
        /// </summary>
        Block2 = 23,
        /// <summary>
        /// Block1.
        /// </summary>
        Block1 = 27,
        /// <summary>
        /// Proxy-Uri.
        /// </summary>
        ProxyUri = 35,
        /// <summary>
        /// Proxy-Scheme.
        /// </summary>
        ProxyScheme = 39,
        /// <summary>
        /// Size1.
        /// </summary>
        Size1 = 60
    }
}
