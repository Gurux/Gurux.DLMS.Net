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

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// CoAP request response counter.
    /// </summary>
    public class GXCoapRequestResponseCounter
    {
        /// <summary>
        /// CoAP requests received.
        /// </summary>
        public UInt32 RxRequests
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP requests sent.
        /// </summary>
        public UInt32 TxRequests
        {
            get;
            set;
        }
        /// <summary>
        /// CoAP responses received.
        /// </summary>
        public UInt32 RxResponse
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP responses sent.
        /// </summary>
        public UInt32 TxResponse
        {
            get;
            set;
        }

        /// <summary>
        ///  CoAP client errors sent.
        /// </summary>
        public UInt32 TxClientError
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP client errors received.
        /// </summary>
        public UInt32 RxClientError
        {
            get;
            set;
        }

        /// <summary>
        ///  CoAP server errors sent.
        /// </summary>
        public UInt32 TxServerError
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP server errors received.
        /// </summary>
        public UInt32 RxServerError
        {
            get;
            set;
        }
    }
}
