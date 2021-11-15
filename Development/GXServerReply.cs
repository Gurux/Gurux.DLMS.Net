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
using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{
    public class GXServerReply
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Received data.</param>
        public GXServerReply(byte[] data)
        {
            Data = data;
        }

        ///<summary>
        ///Server received data.
        ///</summary>
        public byte[] Data
        {
            get;
            set;
        }

        ///<summary>
        ///Server reply message.
        ///</summary>
        public byte[] Reply
        {
            get;
            set;
        }

        ///<summary>
        /// Connection info.
        ///</summary>
        public GXDLMSConnectionEventArgs ConnectionInfo
        {
            get;
            set;
        }

        ///<summary>
        ///Is GBT streaming in progress.
        ///</summary>
        public bool IsStreaming
        {
            get
            {
                return (GbtCount != 0 && MoreData == RequestTypes.None) ||
                    (HdlcWindowCount != 0 && (MoreData & RequestTypes.Frame) != 0);
            }
        }

        ///<summary>
        ///Is GBT streaming in progress.
        ///</summary>
        public RequestTypes MoreData
        {
            get;
            internal set;
        }

        ///<summary>
        ///GBT Message count to send.
        ///</summary>
        public byte GbtCount
        {
            get;
            internal set;
        }

        ///<summary>
        ///HDLC window count to send.
        ///</summary>
        public byte HdlcWindowCount
        {
            get;
            internal set;
        }

        ///<summary>
        /// Received command.
        ///</summary>
        public Command Command
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gateway information.
        /// </summary>
        public GXDLMSGateway Gateway
        {
            get;
            set;
        }

        /// <summary>
        /// Baudrate is changed when optical probe is used.
        /// </summary>
        public int NewBaudRate
        {
            get;
            set;
        }
    }
}
