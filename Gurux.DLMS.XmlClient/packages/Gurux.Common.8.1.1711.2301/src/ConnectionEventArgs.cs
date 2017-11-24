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
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gurux.Common
{    
    /// <summary>
	/// Argument class for IGXMedia connection and disconnection events.
	/// </summary>
    public class TraceEventArgs
    {
        /// <summary>
        /// Timestamp.
        /// </summary>
        public DateTime Timestamp
        {
            get;
            internal set;
        }

        /// <summary>
        /// Is data send or received and type of trace.
        /// </summary>
        public TraceTypes Type
        {
            get;
            internal set;
        }

        /// <summary>
        /// Received/send data.
        /// </summary>
        public object Data
        {
            get;
            internal set;
        }

        /// <summary>
        /// Data receiver.
        /// </summary>
        public string Receiver
        {
            get;
            set;
        }

        /// <summary>
        /// Convert data to string.
        /// </summary>
        /// <param name="ascii">Is content get as ascii or hex string.</param>
        /// <returns>Content of data as string.</returns>
        public string DataToString(bool ascii)
        {
            if (Data == null)
            {
                return "";
            }
            if (Data is byte[])
            {
                if (ascii)
                {
                    return ASCIIEncoding.ASCII.GetString(Data as byte[]);
                }
                return GXCommon.ToHex((Data as byte[]), true);
            }
            return Convert.ToString(Data);
        }

        /// <summary>
        /// Show trace event content as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Timestamp.ToString("hh:mm:ss.fff") + "\t" + Type.ToString() + "\t" + DataToString(false);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TraceEventArgs(TraceTypes type, object data, string receiver)
        {
            Timestamp = DateTime.Now;
            Type = type;
            Data = data;
            Receiver = receiver;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TraceEventArgs(TraceTypes type, byte[] data, int index, int length, string receiver)
        {
            Timestamp = DateTime.Now;
            Type = type;
            byte[] tmp = new byte[length];
            Array.Copy(data, index, tmp, 0, length);
            Data = tmp;
            Receiver = receiver;
        }
    }

	/// <summary>
	/// Argument class for IGXMedia connection and disconnection events.
	/// </summary>
    public class ConnectionEventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionEventArgs()
        {
            Accept = true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionEventArgs(string info)            
        {
            Accept = true;
            Info = info;
        }

        /// <summary>
        /// Media depend information.
        /// </summary>
        public string Info
        {
            get;
            set;
        }

        /// <summary>
        /// False, if the client is not accepted to connect.
        /// </summary>
        public bool Accept
        {
            get;
            set;
        }
    }
}
