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
    /// CoAP client error codes.
    /// </summary>
    public enum CoAPClientError : byte
    {
        /// <summary>
        /// Bad Request.
        /// </summary>
        BadRequest = 0,
        /// <summary>
        /// Unauthorized.
        /// </summary>
        Unauthorized = 1,
        /// <summary>
        /// Bad Option.
        /// </summary>
        BadOption = 2,
        /// <summary>
        /// Forbidden.
        /// </summary>
        Forbidden = 3,
        /// <summary>
        /// Not Found.
        /// </summary>
        NotFound = 4,
        /// <summary>
        /// Method Not Allowed.
        /// </summary>
        MethodNotAllowed = 5,
        /// <summary>
        /// Not Acceptable.
        /// </summary>
        NotAcceptable = 6,
        /// <summary>
        /// Request Entity Incomplete.
        /// </summary>
        RequestEntityIncomplete = 8,
        /// <summary>
        /// Conflict.
        /// </summary>
        Conflict = 9,
        /// <summary>
        /// Precondition Failed.
        /// </summary>
        PreconditionFailed = 12,
        /// <summary>
        /// Request Entity Too Large.
        /// </summary>
        RequestEntityTooLarge = 13,
        /// <summary>
        /// Unsupported Content-Format.
        /// </summary>
        UnsupportedContentFormat = 15
    }
}
