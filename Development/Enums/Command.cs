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
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS
{
    /// <summary>
    /// DLMS command enumeration.
    /// </summary>
    enum Command
    {
        /// <summary>
        /// No command to execute.
        /// </summary>
        None = 0,

        /// <summary>
        /// Read request.
        /// </summary>
        ReadRequest = 0x5,

        /// <summary>
        /// Read response.
        /// </summary>
        ReadResponse = 0xC,

        /// <summary>
        /// Write request.
        /// </summary>
        WriteRequest = 0x6,

        /// <summary>
        /// Write response.
        /// </summary>
        WriteResponse = 0xD,

        /// <summary>
        /// Get request.
        /// </summary>
        GetRequest = 0xC0,

        /// <summary>
        /// Get response.
        /// </summary>
        GetResponse = 0xC4,

        /// <summary>
        /// Set request.
        /// </summary>
        SetRequest = 0xC1,

        /// <summary>
        /// Set response.
        /// </summary>
        SetResponse = 0xC5,

        /// <summary>
        /// Action request.
        /// </summary>
        MethodRequest = 0xC3,

        /// <summary>
        /// Action response.
        /// </summary>
        MethodResponse = 0xC7,

        /// <summary>
        /// Command rejected.
        /// </summary>
        Rejected = 0x97,

        /// <summary>
        /// SNRM request.
        /// </summary>
        Snrm = 0x93,

        /// <summary>
        /// UA request.
        /// </summary>
        Ua = 0x73,

        /// <summary>
        /// AARQ request.
        /// </summary>
        Aarq = 0x60,

        /// <summary>
        /// AARE request.
        /// </summary>
        Aare = 0x61,

        /// <summary>
        /// Disconnect request for HDLC framing.
        /// </summary>
        Disc = 0x53,

        /// <summary>
        /// Disconnect request for WRAPPER.
        /// </summary>
        DisconnectRequest = 0x62,

        /// <summary>
        /// Disconnect response for WRAPPER.
        /// </summary>
        DisconnectResponse = 0x63,

        /// <summary>
        /// Exception Response.
        /// </summary>
        ExceptionResponse = 0xD8,
        
        /// <summary>
        /// General Block Transfer.
        /// </summary>
        GeneralBlockTransfer = 0xE0,

        /// <summary>
        /// Data Notification request.
        /// </summary>
        DataNotification = 0x0F,

        /// <summary>
        /// Glo get request.
        /// </summary>
        GloGetRequest = 0xC8,

        /// <summary>
        /// Glo get response.
        /// </summary>
        GloGetResponse = 0xCC,

        /// <summary>
        /// Glo set request.
        /// </summary>
        GloSetRequest = 0xC9,

        /// <summary>
        /// Glo set response.
        /// </summary>
        GloSetResponse = 0xCD,

        /// <summary>
        /// Glo method request.
        /// </summary>
        GloMethodRequest = 0xCB,

        /// <summary>
        /// Glo method response.
        /// </summary>
        GloMethodResponse = 0xCF
    }
}
