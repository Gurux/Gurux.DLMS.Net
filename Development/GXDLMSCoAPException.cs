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
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{
    /// <summary>
    /// CoAP exception.
    /// </summary>
    public class GXDLMSCoAPException : Exception
    {
        /// <summary>
        /// Constructor for the client error.
        /// </summary>
        /// <param name="error">Client error code.</param>
        public GXDLMSCoAPException(CoAPClientError error)
            : base(error.ToString())
        {
            ClientError = error;
        }

        /// <summary>
        /// Constructor for the server error.
        /// </summary>
        /// <param name="error">Server error code.</param>
        public GXDLMSCoAPException(CoAPServerError error)
            : base(error.ToString())
        {
            ServerError = error;
        }

        /// <summary>
        /// CoAP client error.
        /// </summary>
        public CoAPClientError ClientError
        {
            get;
            internal set;
        }

        /// <summary>
        /// CoAP server error.
        /// </summary>
        public CoAPServerError ServerError
        {
            get;
            internal set;
        }
    }
}
