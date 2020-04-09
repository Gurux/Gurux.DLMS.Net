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
using System.Text;
using System.Diagnostics;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{
    /// <summary>
    /// BitString class is used with Bit strings.
    /// </summary>
    public class GXBitString : System.IConvertible
    {
        /// <summary>
        /// Bit string value.
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXBitString()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Bitstring value.</param>
        public GXBitString(String value)
        {
            Value = value;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">byte array.</param>
        public GXBitString(byte[] value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte it in value)
            {
                GXCommon.ToBitString(sb, it, 8);
            }
            Value = sb.ToString();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">byte array.</param>
        public GXBitString(byte[] value, int index, int count)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte it in value)
            {
                if (index != 0)
                {
                    --index;
                }
                else
                {
                    if (count < 1)
                    {
                        break;
                    }
                    GXCommon.ToBitString(sb, it, count);
                    count -= 8;
                }
            }
            Value = sb.ToString();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">byte.</param>
        public GXBitString(byte value, int count)
        {
            StringBuilder sb = new StringBuilder();
            GXCommon.ToBitString(sb, value, 8);
            if (count != 8)
            {
                sb.Remove(count, 8 - count);
            }
            Value = sb.ToString();
        }

        public override string ToString()
        {
            return Value;
        }

        private static void FillWithZeroes(GXByteBuffer bb, int size)
        {
            if (bb.Size < size)
            {
                bb.Set(0, new byte[size - bb.Size]);
            }
        }
        private static GXByteBuffer GetByteBuffer(string value, int size)
        {
            GXByteBuffer bb = new GXByteBuffer();
            GXCommon.SetBitString(bb, value, false);
            FillWithZeroes(bb, size);
            return bb;
        }


        TypeCode IConvertible.GetTypeCode()
        {
            throw new NotImplementedException();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            GXByteBuffer bb = GetByteBuffer(Value, 1);
            return bb.GetUInt8();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            GXByteBuffer bb = GetByteBuffer(Value, 2);
            return bb.GetInt16();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            GXByteBuffer bb = GetByteBuffer(Value, 2);
            return bb.GetUInt16();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            GXByteBuffer bb = GetByteBuffer(Value, 4);
            return bb.GetInt32();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            GXByteBuffer bb = GetByteBuffer(Value, 4);
            return bb.GetUInt32();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            GXByteBuffer bb = GetByteBuffer(Value, 8);
            return bb.GetInt64();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            GXByteBuffer bb = GetByteBuffer(Value, 8);
            return bb.GetUInt64();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return Value;
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
