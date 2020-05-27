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
        public GXBitString(string value)
        {
            Value = value;
        }

        private static void ToBitString(StringBuilder sb, byte value, int count)
        {
            if (count > 8)
            {
                count = 8;
            }
            for(int pos = 0; pos != count; ++pos)
            {
                if ((value & (1 << pos)) != 0)
                {
                    sb.Append("1");
                }
                else
                {
                    sb.Append("0");
                }
            }
        }

        /// <summary>
        /// Convert integer value to BitString.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="count">Amount of bits.</param>
        /// <returns>Bitstring.</returns>
        public static string ToBitString(UInt32 value, int count)
        {
            StringBuilder sb = new StringBuilder();
            ToBitString(sb, (byte)(value & 0xFF), count);
            if (count > 8)
            {
                ToBitString(sb, (byte)((value >> 8) & 0xFF), count - 8);
                if (count > 16)
                {
                    ToBitString(sb, (byte)((value >> 16) & 0xFF), count - 16);
                    if (count > 24)
                    {
                        ToBitString(sb, (byte)((value >> 24) & 0xFF), count - 24);
                    }
                }
            }
            if (sb.Length > count)
            {
                sb.Remove(count, sb.Length - count);
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return Value;
        }


        private UInt32 ToNumeric()
        {
            UInt32 ret = 0;
            int pos;
            if (Value != null)
            {
                for (pos = 0; pos != Value.Length; ++pos)
                {
                    if (Value[pos] == '1')
                    {
                        ret |= (UInt32)(1 << pos);
                    }
                }
            }
            return ret;
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
            return (byte)ToNumeric();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return (Int16)ToNumeric();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return (UInt16)ToNumeric();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return (Int32)ToNumeric();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return (UInt32)ToNumeric();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return (Int64)ToNumeric();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return (UInt64)ToNumeric();
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
