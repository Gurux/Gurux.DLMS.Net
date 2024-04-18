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
    /// CoAP messages counter.
    /// </summary>
    public class GXCoapMessagesCounter
    {
        /// <summary>
        /// Transmit messages.
        /// </summary>
        public UInt32 Tx
        {
            get;
            set;
        }
        /// <summary>
        /// Received messages.
        /// </summary>
        public UInt32 Rx
        {
            get;
            set;
        }
        /// <summary>
        /// CoAP messages that have been re-sent.
        /// </summary>
        public UInt32 TxResend
        {
            get;
            set;
        }

        /// <summary>
        /// Received CoAP reset messages.
        /// </summary>
        public UInt32 RxReset
        {
            get;
            set;
        }
        /// <summary>
        /// Transmit CoAP reset messages.
        /// </summary>
        public UInt32 TxReset
        {
            get;
            set;
        }

        /// <summary>
        /// Received CoAP acknowledgement messages.
        /// </summary>
        public UInt32 RxAck
        {
            get;
            set;
        }

        /// <summary>
        /// Transmit CoAP acknowledgement messages.
        /// </summary>
        public UInt32 TxAck
        {
            get;
            set;
        }

        /// <summary>
        /// The number of CoAP messages received but silently dropped 
        /// due to message format error or other reason.
        /// </summary>
        public UInt32 RxDrop
        {
            get;
            set;
        }

        /// <summary>
        /// The number of CoAP responses that were returned non-piggybacked.
        /// </summary>
        public UInt32 TxNonPiggybacked
        {
            get;
            set;
        }

        /// <summary>
        /// The number of times transmission of a CoAP message 
        /// was abandoned due to exceed of the max retransmission counter of the CoAP 
        /// messaging layer.
        /// </summary>
        public UInt32 MaxRtxExceeded
        {
            get;
            set;
        }
    }
}
