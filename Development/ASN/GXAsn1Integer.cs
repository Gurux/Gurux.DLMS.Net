using System;

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

namespace Gurux.DLMS.ASN
{
    /// <summary>
    /// ASN1 bit string.
    /// </summary>
    public class GXAsn1Integer
    {
        /// <summary>
        /// Bit string.
        /// </summary>
        public byte[] Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXAsn1Integer()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Integer. </param>
        public GXAsn1Integer(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentException("data");
            }
            Value = data;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        ///  <param name="data">Data.</param>
        ///  <param name="index">Index.</param>
        ///  <param name="count">Count.</param>
        public GXAsn1Integer(byte[] data, int index, int count)
        {
            if (data == null || data.Length < index + count)
            {
                throw new System.ArgumentException("data");
            }
            Value = new byte[count];
            Array.Copy(data, index, Value, 0, count);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Integer. </param>
        public GXAsn1Integer(string data)
        {
            if (data == null)
            {
                throw new System.ArgumentException("data");
            }
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt64(UInt64.Parse(data));
            Value = bb.Array();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Integer. </param>
        public GXAsn1Integer(UInt64 data)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt64(data);
            Value = bb.Array();
        }

        /// <returns>
        /// Get integer value as int.
        /// </returns>
        public sbyte ToByte()
        {
            GXByteBuffer bb = new GXByteBuffer(Value);
            return bb.GetInt8();
        }

        /// <returns>
        /// Get integer value as short.
        /// </returns>
        public short ToShort()
        {
            GXByteBuffer bb = new GXByteBuffer(Value);
            return bb.GetInt16();
        }

        /// <returns>
        /// Get integer value as int.
        /// </returns>
        public int ToInt()
        {
            GXByteBuffer bb = new GXByteBuffer(Value);
            return bb.GetInt32();
        }

        /// <returns>
        /// Get integer value as long.
        /// </returns>
        public long ToLong()
        {
            GXByteBuffer bb = new GXByteBuffer(Value);
            return bb.GetInt64();
        }

        public override sealed string ToString()
        {
            string str;
            switch (Value.Length)
            {
                case 1:
                    str = Convert.ToString(ToByte());
                    break;
                case 2:
                    str = Convert.ToString(ToShort());
                    break;
                case 4:
                    str = Convert.ToString(ToInt());
                    break;
                case 8:
                    str = Convert.ToString(ToLong());
                    break;
                default:
                    GXByteBuffer bb = new GXByteBuffer();
                    bb.Set(Value);
                    str = bb.GetUInt64().ToString();
                    break;
            }
            return str;
        }
    }
}