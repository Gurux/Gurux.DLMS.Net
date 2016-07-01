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

namespace Gurux.DLMS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Gurux.DLMS.Enums;

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
        public UInt16 ReadPosition
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
        /// Received command. 
        ///</summary>
        internal Command Command
        {
            get;
            set;
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
        /// Received error. 
        ///</summary>
        public short Error
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
        /// Cipher index is position where data is decrypted.
        /// </summary>
        public UInt16 CipherIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Is received message General Block Transfer message.
        /// </summary>
        public bool Gbt
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

        ///<summary>
        /// Reset data values to default. 
        ///</summary>         
        public void Clear()
        {
            MoreData = RequestTypes.None;
            Command = Command.None;
            Data.Capacity = 0;
            IsComplete = false;
            Error = 0;
            TotalCount = 0;
            Value = null;
            ReadPosition = 0;
            Gbt = false;
            PacketLength = 0;
            DataType = DataType.None;
            CipherIndex = 0;
            Time = DateTime.MinValue;
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
                if (Value is object[])
                {
                    return ((object[])Value).Length;
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
            if (Data == null)
            {
                return "";
            }
            return Gurux.DLMS.Internal.GXCommon.ToHex(Data.Data, true, 0, Data.Size);
        }
    }
}
