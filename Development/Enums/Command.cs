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
    public enum Command
    {
        /// <summary>
        /// No command to execute.
        /// </summary>
        None = 0,

        /// <summary>
        /// Initiate request.
        /// </summary>
        InitiateRequest = 0x1,

        /// <summary>
        /// Initiate response.
        /// </summary>
        InitiateResponse = 0x8,

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
        /// HDLC Disconnect Mode. 
        /// </summary>
        /// <remarks>
        /// Responder for Disconnect Mode request.
        /// </remarks>
        DisconnectMode = 0x1f,

        /// <summary>
        /// HDLC Unacceptable Frame.
        /// </summary>
        UnacceptableFrame = 0x97,

        /// <summary>
        /// HDLC SNRM request.
        /// </summary>
        Snrm = 0x93,

        /// <summary>
        /// HDLC UA request.
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
        /// Disconnect request for HDLC framing. (DISC)
        /// </summary>
        DisconnectRequest = 0x53,

        /// <summary>
        /// Release request.
        /// </summary>
        ReleaseRequest = 0x62,

        /// <summary>
        /// Release response.
        /// </summary>
        ReleaseResponse = 0x63,

        /// <summary>
        /// Confirmed Service Error.
        /// </summary>
        ConfirmedServiceError = 0x0E,

        /// <summary>
        /// Exception Response.
        /// </summary>
        ExceptionResponse = 0xD8,

        /// <summary>
        /// General Block Transfer.
        /// </summary>
        GeneralBlockTransfer = 0xE0,

        /// <summary>
        /// Access Request.
        /// </summary>
        AccessRequest = 0xD9,
        /// <summary>
        /// Access Response.
        /// </summary>
        AccessResponse = 0xDA,

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
        /// Glo event notification request.
        /// </summary>
        GloEventNotificationRequest = 0xCA,

        /// <summary>
        /// Glo method request.
        /// </summary>
        GloMethodRequest = 0xCB,

        /// <summary>
        /// Glo method response.
        /// </summary>
        GloMethodResponse = 0xCF,

        /// <summary>
        /// Glo Initiate request.
        /// </summary>
        GloInitiateRequest = 0x21,

        /// <summary>
        /// Glo read request.
        /// </summary>
        GloReadRequest = 37,

        /// <summary>
        /// Glo write request.
        /// </summary>
        GloWriteRequest = 38,

        /// <summary>
        /// Glo Initiate response.
        /// </summary>
        GloInitiateResponse = 0x28,

        /// <summary>
        /// Glo read response.
        /// </summary>
        GloReadResponse = 44,

        /// <summary>
        /// Glo write response.
        /// </summary>
        GloWriteResponse = 45,

        /// <summary>
        /// General GLO ciphering.
        /// </summary>
        GeneralGloCiphering = 0xDB,

        /// <summary>
        /// General DED ciphering.
        /// </summary>
        GeneralDedCiphering = 0xDC,

        /// <summary>
        /// General ciphering.
        /// </summary>
        GeneralCiphering = 0xDD,

        /// <summary>
        /// Information Report request.
        /// </summary>
        InformationReport = 0x18,

        /// <summary>
        /// Event Notification request.
        /// </summary>
        EventNotification = 0xC2,



        /// <summary>
        /// Ded get request.
        /// </summary>
        DedGetRequest = 0xD0,

        /// <summary>
        /// Ded get response.
        /// </summary>
        DedGetResponse = 0xD4,

        /// <summary>
        /// Ded set request.
        /// </summary>
        DedSetRequest = 0xD1,

        /// <summary>
        /// Ded set response.
        /// </summary>
        DedSetResponse = 0xD5,

        /// <summary>
        /// Ded event notification request.
        /// </summary>
        DedEventNotificationRequest = 0xD2,

        /// <summary>
        /// Ded method request.
        /// </summary>
        DedMethodRequest = 0xD3,

        /// <summary>
        /// Ded method response.
        /// </summary>
        DedMethodResponse = 0xD7,

        /// <summary>
        /// Request message from client to gateway.
        /// </summary>
        GatewayRequest = 0xE6,

        /// <summary>
        /// Response message from gateway to client.
        /// </summary>
        GatewayResponse = 0xE7
    }
}
