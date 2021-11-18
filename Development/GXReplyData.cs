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

namespace Gurux.DLMS
{
    public class GXReplyData
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="more">Is more data available.</param>
        /// <param name="cmd"> Received command.</param>
        /// <param name="buff">Received data.</param>
        /// <param name="complete">Is frame complete.</param>
        /// <param name="error">Received error ID.</param>
        internal GXReplyData(RequestTypes more, Command cmd, GXByteBuffer buff, bool complete, byte error)
        {
            Data = new GXByteBuffer();
            Clear();
            MoreData = more;
            Command = cmd;
            Data = buff;
            IsComplete = complete;
            Error = error;
        }

        ///<summary>
        /// XML settings. This is used only on xml parser.
        ///</summary>
        internal GXDLMSTranslatorStructure Xml
        {
            get;
            set;
        }

        ///<summary>
        /// Constructor.
        ///</summary>
        public GXReplyData()
        {
            Data = new GXByteBuffer();
            Clear();
        }

        public DataType DataType
        {
            get;
            set;
        }

        ///<summary>
        /// Read value.
        ///</summary>
        public object Value
        {
            get;
            set;
        }

        ///<summary>
        /// Last read position. This is used in peek to solve how far data is read.
        ///</summary>
        public int ReadPosition
        {
            get;
            set;
        }

        ///<summary>
        /// Packet length.
        ///</summary>
        public int PacketLength
        {
            get;
            set;
        }

        ///<summary>
        /// PDU is not parsed and it's returned as it is.
        ///</summary>
        public bool RawPdu
        {
            get;
            set;
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
        /// Received command type.
        /// </summary>
        internal byte CommandType
        {
            get;
            set;
        }

        ///<summary>
        /// Received Ciphered command.
        ///</summary>
        public Command CipheredCommand
        {
            get;
            internal set;
        }


        ///<summary>
        /// Received data.
        ///</summary>
        public GXByteBuffer Data
        {
            get;
            set;
        }

        ///<summary>
        /// Is frame complete.
        ///</summary>
        public bool IsComplete
        {
            get;
            internal set;
        }

        ///<summary>
        /// HDLC frame ID.
        ///</summary>
        public byte FrameId
        {
            get;
            internal set;
        }

        ///<summary>
        /// Received invoke ID.
        ///</summary>
        public UInt32 InvokeId
        {
            get;
            internal set;
        }

        ///<summary>
        /// Received error.
        ///</summary>
        public short Error
        {
            get;
            set;
        }

        ///<summary>
        /// True, if there are empty frames or blocks.
        ///</summary>
        public RequestTypes EmptyResponses
        {
            get;
            set;
        }


        ///<summary>
        /// Expected count of elements in the array.
        ///</summary>
        /// <seealso cref="Count"/>
        ///<seealso cref="Peek"/>
        public int TotalCount
        {
            get;
            set;
        }

        /// <summary>
        /// Cipher index is position where data is decrypted or GBT is read.
        /// </summary>
        public int CipherIndex
        {
            get;
            set;
        }

        ///<summary>
        /// Data notification date time.
        ///</summary>
        public DateTime Time
        {
            get;
            set;
        }

        /// <summary>
        /// GBT block number.
        /// </summary>
        public UInt16 BlockNumber
        {
            get;
            set;
        }
        /// <summary>
        /// GBT block number ACK.
        /// </summary>
        public UInt16 BlockNumberAck
        {
            get;
            set;
        }

        /// <summary>
        /// Is GBT streaming in use.
        /// </summary>
        internal bool Streaming
        {
            get;
            set;
        }

        /// <summary>
        /// GBT Window size.
        /// </summary>
        /// <remarks>
        /// This is for internal use.
        /// </remarks>
        internal byte GbtWindowSize
        {
            get;
            set;
        }

        /// <summary>
        /// Is HDLC streaming in progress.
        /// </summary>
        /// <remarks>
        /// This is for internal use.
        /// </remarks>
        internal bool HdlcStreaming
        {
            get;
            set;
        }

        /// <summary>
        /// Client address of the notification message.
        /// </summary>
        [Obsolete("Use TargetAddress instead.")]
        public int ClientAddress
        {
            get
            {
                return TargetAddress;
            }
            set
            {
                TargetAddress = value;
            }
        }

        /// <summary>
        /// Client address of the notification message.
        /// </summary>
        /// <remarks>
        /// Notification message sets this. This is also used with XML parser.
        /// </remarks>
        public int TargetAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Server address of the notification message.
        /// </summary>
        /// <remarks>
        /// Notification message sets this.
        /// </remarks>
        [Obsolete("Use SourceAddress instead.")]
        public int ServerAddress
        {
            get
            {
                return SourceAddress;
            }
            set
            {
                SourceAddress = value;
            }
        }

        /// <summary>
        /// Server address of the notification message.
        /// </summary>
        /// <remarks>
        /// Notification message sets this. This is also used with XML parser.
        /// </remarks>
        public int SourceAddress
        {
            get;
            set;
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
        /// Is GBT or HDLC streaming used.
        /// </summary>
        /// <returns></returns>
        public bool IsStreaming()
        {
            return ((MoreData & RequestTypes.Frame) == 0 &&
                Streaming && (BlockNumberAck * GbtWindowSize) + 1 > BlockNumber) ||
                ((MoreData & RequestTypes.Frame) == RequestTypes.Frame && HdlcStreaming);
        }

        ///<summary>
        /// Reset data values to default.
        ///</summary>
        public void Clear()
        {
            MoreData = RequestTypes.None;
            CipheredCommand = Command = Command.None;
            CommandType = 0;
            Data.Capacity = 0;
            IsComplete = false;
            Error = 0;
            TotalCount = 0;
            Value = null;
            ReadPosition = 0;
            PacketLength = 0;
            DataType = DataType.None;
            CipherIndex = 0;
            Time = DateTime.MinValue;
            GbtWindowSize = 0;
            if (Xml != null)
            {
                Xml.SetXmlLength(0);
            }
            InvokeId = 0;
        }

        /// <summary>
        /// Is more data available.
        /// </summary>
        public bool IsMoreData
        {
            get
            {
                return MoreData != RequestTypes.None && Error == 0;
            }
        }

        /// <summary>
        /// Is notify message.
        /// </summary>
        public bool IsNotify
        {
            get
            {
                return FrameId == 0x13 || Command == Command.EventNotification || Command == Command.DataNotification || Command == Command.InformationReport;
            }
        }

        /// <summary>
        /// Is more data available. Return None if more data is not available or Frame or Block type.
        /// </summary>
        public RequestTypes MoreData
        {
            get;
            internal set;
        }

        public string ErrorMessage
        {
            get
            {
                return GXDLMS.GetDescription((ErrorCode)Error);
            }
        }

        /// <summary>
        /// Get count of read elements. If this method is used peek must be set true.
        /// </summary>
        /// <seealso cref="Peek"/>
        /// <seealso cref="TotalCount"/>
        public int Count
        {
            get
            {
                if (Value is List<object>)
                {
                    return ((List<object>)Value).Count;
                }
                return 0;
            }
        }

        /// <summary>
        /// Is value try to peek.
        /// </summary>
        /// <seealso cref="Count"/>
        /// <seealso cref="TotalCount"/>
        public bool Peek
        {
            get;
            set;
        }

        /// <summary>
        /// Get content of reply data as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Xml != null)
            {
                return Xml.ToString();
            }
            if (Data == null)
            {
                return "";
            }
            return Gurux.DLMS.Internal.GXCommon.ToHex(Data.Data, true, 0, Data.Size);
        }
    }
}
