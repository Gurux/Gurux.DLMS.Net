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
using System.Collections.Generic;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{
    /// <summary>
    /// CoAP settings contains CoAP settings.
    /// </summary>
    public class GXCoAPSettings
    {
        /// <summary>
        /// CoAP version.
        /// </summary>
        public byte Version
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP type.
        /// </summary>
        public CoAPType Type
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP class code.
        /// </summary>
        public CoAPClass Class
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP Method.
        /// </summary>
        public CoAPMethod Method
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP Success.
        /// </summary>
        public CoAPSuccess Success
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP client error.
        /// </summary>
        public CoAPClientError ClientError
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP server error.
        /// </summary>
        public CoAPServerError ServerError
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP signaling.
        /// </summary>
        public CoAPSignaling Signaling
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP message Id.
        /// </summary>
        public UInt16 MessageId
        {
            get;
            set;
        }


        /// <summary>
        /// CoAP token.
        /// </summary>
        public UInt64 Token
        {
            get;
            set;
        }

        /// <summary>
        /// Uri host.
        /// </summary>
        public string Host
        {
            get;
            set;
        }

        /// <summary>
        /// Uri Path.
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Uri port.
        /// </summary>
        public UInt16 Port
        {
            get;
            set;
        }

        /// <summary>
        /// If none match.
        /// </summary>
        public CoAPContentType IfNoneMatch
        {
            get;
            set;
        } = CoAPContentType.ApplicationOscore;

        /// <summary>
        /// Content format.
        /// </summary>
        public CoAPContentType ContentFormat
        {
            get;
            set;
        } = CoAPContentType.ApplicationOscore;

        /// <summary>
        /// Max age.
        /// </summary>
        public UInt16 MaxAge
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP block number.
        /// </summary>
        public byte BlockNumber
        {
            get;
            internal set;
        }

        /// <summary>
        /// Unknown options.
        /// </summary>
        public SortedList<UInt16, object> Options
        {
            get;
            set;
        } = new SortedList<UInt16, object>();

        /// <summary>
        /// Reset all values.
        /// </summary>
        public void Reset()
        {
            Version = 0;
            Type = CoAPType.Confirmable;
            Class = CoAPClass.Method;
            Method = CoAPMethod.None;
            Success = CoAPSuccess.None;
            ClientError = CoAPClientError.BadRequest;
            ServerError = CoAPServerError.Internal;
            Signaling = CoAPSignaling.Unassigned;
            MessageId = 0;
            Token = 0;
            Host = null;
            Path = null;
            Port = 0;
            IfNoneMatch = CoAPContentType.None;
            ContentFormat = CoAPContentType.None;
            MaxAge = 0;
            BlockNumber = 0;
            Options.Clear();
        }
    }
}