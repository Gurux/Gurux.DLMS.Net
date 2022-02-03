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

namespace Gurux.DLMS.Ecdsa
{
    public class GXBigInteger
    {
        /// <summary>
        /// List of values. Least Significated is in the first item.
        /// </summary>
        private UInt32[] Data = new UInt32[70];

        /// <summary>
        /// Items count in the data buffer.
        /// </summary>
        internal UInt16 Count
        {
            get;
            set;
        }

        /// <summary>
        /// Is value IsNegative.
        /// </summary>
        /// <returns>True, if value is IsNegative.</returns>
        public bool IsNegative
        {
            get;
            private set;
        }

        /// <summary>
        /// Is value zero.
        /// </summary>
        /// <returns>True, if value is zero.</returns>
        public bool IsZero
        {
            get
            {
                return Count == 0 || (Count == 1 && Data[0] == 0);
            }
        }

        /// <summary>
        /// Is value even.
        /// </summary>
        /// <returns>True, if value is even.</returns>
        public bool IsEven
        {
            get
            {
                return Count != 0 && Data[0] % 2 == 0;
            }
        }

        /// <summary>
        /// Is value One.
        /// </summary>
        /// <returns>True, if value is one.</returns>
        public bool IsOne
        {
            get
            {
                return Count == 1 && Data[0] == 1;
            }
        }

        /// <summary>
        /// Constuctor.
        /// </summary>
        public GXBigInteger()
        {
        }

        private void Add(UInt32 value)
        {
            Data[Count] = value;
            ++Count;
        }

        private void AddRange(UInt32[] value)
        {
            Array.Copy(value, 0, Data, Count, value.Length);
            Count += (UInt16)value.Length;
        }


        /// <summary>
        /// Get values from byte buffer.
        /// </summary>
        /// <param name="value"></param>
        private void FromByteBuffer(GXByteBuffer value)
        {
            for (int pos = value.Size - 4; pos > -1; pos = pos - 4)
            {
                Add(value.GetUInt32(pos));
            }
            switch (value.Size % 4)
            {
                case 1:
                    Add(value.GetUInt8());
                    break;
                case 2:
                    Add(value.GetUInt16());
                    break;
                case 3:
                    // Data.Add(value.GetUInt24());
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Constuctor.
        /// </summary>
        /// <param name="value">Big integer value in MSB.</param>
        public GXBigInteger(string value)
        {
            IsNegative = value.StartsWith("-");
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetHexString(value);
            FromByteBuffer(bb);
        }

        public GXBigInteger(UInt64 value) : this()
        {
            Add((UInt32)value);
            value >>= 32;
            Add((UInt32)value);
        }

        public GXBigInteger(UInt32 value) : this()
        {
            Add(value);
        }

        public GXBigInteger(int value) : this()
        {
            Add((UInt32)value);
        }

        /// <summary>
        /// Constructor value.
        /// </summary>
        /// <param name="values">Data in MSB format.</param>
        public GXBigInteger(UInt32[] values)
        {
            AddRange(values);
            Array.Reverse(Data, 0, values.Length);
        }

        /// <summary>
        /// Constructor value.
        /// </summary>
        /// <param name="value">Byte array Data in MSB format.</param>
        public GXBigInteger(byte[] value)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.Set(value);
            FromByteBuffer(bb);
        }

        /// <summary>
        /// Constructor value.
        /// </summary>
        /// <param name="value">Byte array Data in MSB format.</param>
        public GXBigInteger(GXByteBuffer value)
        {
            FromByteBuffer(value);
        }

        public GXBigInteger(GXBigInteger value)
        {
            AddRange(value.Data);
            Count = value.Count;
            IsNegative = value.IsNegative;
        }

        /// <summary>
        /// Convert value to byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return ToArray(true);
        }

        /// <summary>
        /// Convert value to byte array.
        /// </summary>
        /// <returns></returns>
        internal byte[] ToArray(bool removeLeadingZeroes)
        {
            int pos;
            UInt32 value;
            GXByteBuffer bb = new GXByteBuffer();
            int zeroIndex = -1;
            for (pos = 0; pos != Count; ++pos)
            {
                value = Data[pos];
                if (value == 0)
                {
                    zeroIndex = pos;
                }
                else
                {
                    zeroIndex = -1;
                }
                bb.SetUInt32(value);
                Array.Reverse(bb.Data, 4 * pos, 4);
            }
            //Remove leading zeroes.
            if (removeLeadingZeroes && zeroIndex != -1)
            {
                bb.Size = zeroIndex * 4;
            }
            Array.Reverse(bb.Data, 0, bb.Size);
            return bb.Array();
        }

        /// <summary>
        /// Convert value to byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray(int start, int count)
        {
            int pos;
            GXByteBuffer bb = new GXByteBuffer();
            for (pos = start; pos != count; ++pos)
            {
                bb.SetUInt32(Data[pos]);
                Array.Reverse(bb.Data, 4 * pos, 4);
            }
            Array.Reverse(bb.Data, 0, bb.Size);
            return bb.Array();
        }

        public void Or(GXBigInteger value)
        {
            int pos;
            if (Count < value.Count)
            {
                Count = value.Count;
            }
            for (pos = 0; pos < value.Count; ++pos)
            {
                Data[pos] |= value.Data[pos];
            }
        }

        public void Add(GXBigInteger value)
        {
            if (value.IsNegative)
            {
                value.IsNegative = false;
                try
                {
                    Sub(value);
                }
                finally
                {
                    value.IsNegative = true;
                }
            }
            else
            {
                while (Count < value.Count)
                {
                    Add(0);
                }
                UInt64 overflow = 0;
                for (int pos = 0; pos != Count; ++pos)
                {
                    UInt64 tmp = Data[pos];
                    if (pos < value.Count)
                    {
                        tmp += value.Data[pos];
                    }
                    tmp += overflow;
                    Data[pos] = (UInt32)(tmp);
                    overflow = tmp >> 32;
                }
                if (overflow != 0)
                {
                    Add((UInt32)overflow);
                }
            }
        }


        public void Sub(GXBigInteger value)
        {
            int c = Compare(value);
            if (c == 0)
            {
                Clear();
            }
            else if (value.IsNegative || c == -1)
            {
                if (!value.IsNegative && !IsNegative)
                {
                    //If biger value is decreased from smaller value.
                    GXBigInteger tmp = new GXBigInteger(value);
                    tmp.Sub(this);
                    Clear();
                    AddRange(tmp.Data);
                    Count = tmp.Count;
                    IsNegative = true;
                }
                else
                {
                    //If IsNegative value is decreased from the value.
                    bool ret = value.IsNegative;
                    value.IsNegative = false;
                    try
                    {
                        Add(value);
                    }
                    finally
                    {
                        value.IsNegative = ret;
                        IsNegative = !ret;
                    }
                }
            }
            else
            {
                if (!value.IsZero)
                {
                    if (IsZero)
                    {
                        IsNegative = true;
                        Clear();
                        AddRange(value.Data);
                        Count = value.Count;
                    }
                    else
                    {
                        while (Count < value.Count)
                        {
                            Add(0);
                        }
                        byte borrow = 0;
                        UInt64 tmp;
                        int pos;
                        for (pos = 0; pos != value.Count; ++pos)
                        {
                            tmp = Data[pos];
                            tmp += 0x100000000;
                            tmp -= value.Data[pos];
                            tmp -= borrow;
                            Data[pos] = (UInt32)tmp;
                            borrow = (byte)((tmp < 0x100000000) ? 1 : 0);
                        }
                        if (borrow != 0)
                        {
                            for (; pos != Count; ++pos)
                            {
                                tmp = Data[pos];
                                tmp += 0x100000000;
                                tmp -= borrow;
                                Data[pos] = (UInt32)tmp;
                                borrow = (byte)((tmp < 0x100000000) ? 1 : 0);
                                if (borrow == 0)
                                {
                                    break;
                                }
                            }
                        }
                        //Remove empty last item(s).
                        while (Count != 1 && Data[Count - 1] == 0)
                        {
                            --Count;
                        }
                    }
                }
            }
        }

        private static void AddValue(UInt32[] list, UInt32 value, int index)
        {
            UInt64 tmp;
            if (index < list.Length)
            {
                tmp = list[index];
                tmp += value;
                list[index] = (UInt32)tmp;
                UInt32 reminer = (UInt32)(tmp >> 32);
                if (reminer > 0)
                {
                    AddValue(list, reminer, index + 1);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Big integer value is too high.");
            }
        }

        public void Multiply(GXBigInteger value)
        {
            if (value.IsZero || IsZero)
            {
                Count = 0;
            }
            else if (!value.IsOne)
            {
                UInt32[] ret = new UInt32[1 + value.Count + Count];
                UInt32 overflow = 0;
                int index = 0;
                for (int i = 0; i != value.Count; ++i)
                {
                    overflow = 0;
                    for (int j = 0; j != Count; ++j)
                    {
                        UInt64 result = value.Data[i];
                        result *= Data[j];
                        result += overflow;
                        overflow = (UInt32)(result >> 32);
                        index = i + j;
                        AddValue(ret, (UInt32)result, index);
                    }
                    if (overflow > 0)
                    {
                        AddValue(ret, overflow, 1 + index);
                        overflow = 0;
                    }
                }
                index = ret.Length - 1;
                while (index != 0 && ret[index] == 0)
                {
                    --index;
                }
                ++index;
                Array.Copy(ret, Data, index);
                Count = (UInt16)index;
            }
            if (value.IsNegative != IsNegative)
            {
                if (!IsNegative)
                {
                    IsNegative = true;
                }
            }
            else if (IsNegative)
            {
                //If both values are IsNegative.
                IsNegative = false;
            }
        }

        public int Compare(GXBigInteger value)
        {
            int ret = 0;
            if (IsNegative != value.IsNegative)
            {
                //If other value is IsNegative.
                if (IsNegative)
                {
                    ret = -1;
                }
                else
                {
                    ret = 1;
                }
            }
            else if (IsZero && value.IsZero)
            {
                ret = 0;
            }
            else
            {
                int cntA = Count - 1;
                //Skip zero values.
                while (cntA != -1 && Data[cntA] == 0)
                {
                    --cntA;
                }
                int cntB = value.Count - 1;
                //Skip zero values.
                while (cntB != -1 && value.Data[cntB] == 0)
                {
                    --cntB;
                }
                if (cntA > cntB)
                {
                    ret = 1;
                }
                else if (cntA < cntB)
                {
                    ret = -1;
                }
                else
                {
                    do
                    {
                        if (Data[cntA] > value.Data[cntA])
                        {
                            ret = 1;
                            break;
                        }
                        else if (Data[cntA] < value.Data[cntA])
                        {
                            ret = -1;
                            break;
                        }
                        cntA -= 1;
                    }
                    while (cntA != -1);
                }
            }
            return ret;
        }

        /// <summary>
        /// Compare value to integer value.
        /// </summary>
        /// <param name="value">Returns 1 is compared value is bigger, -1 if smaller and 0 if values are equals.</param>
        /// <returns></returns>
        public int Compare(int value)
        {
            if (Count == 0)
            {
                return -1;
            }
            if (Data[0] == value)
            {
                return 0;
            }
            return Data[0] < value ? -1 : 1;
        }

        public void Lshift(int amount)
        {
            if (amount != 0)
            {
                UInt32 overflow = 0;
                for (int pos = 0; pos != Count; ++pos)
                {
                    UInt64 tmp = Data[pos];
                    tmp <<= amount;
                    tmp |= overflow;
                    Data[pos] = (UInt32)tmp;
                    overflow = (UInt32)(tmp >> 32);
                }
                if (overflow != 0)
                {
                    Add(overflow);
                }
            }
        }

        public void Rshift(int amount)
        {
            UInt64 overflow = 0;
            UInt32 mask = 0xFFFFFFFF;
            mask = mask >> (32 - amount);
            int cnt = Count - 1;
            for (int pos = cnt; pos != -1; --pos)
            {
                UInt64 tmp = Data[pos];
                Data[pos] = (UInt32)((tmp >> amount) | overflow);
                overflow = (tmp & mask) << (32 - amount);
            }
            //Remove last item if it's empty.
            while (Count != 1 && Data[cnt] == 0)
            {
                --Count;
                --cnt;
            }
        }

        /// <summary>
        /// Reset value to Zero.
        /// </summary>
        public void Clear()
        {
            Count = 0;
            IsNegative = false;
        }

        public void Pow(int exponent)
        {
            if (exponent != 1)
            {
                int pos = 1;
                GXBigInteger tmp = new GXBigInteger(this);
                while (pos <= exponent / 2)
                {
                    Multiply(this);
                    pos <<= 1;
                }
                while (pos < exponent)
                {
                    Multiply(tmp);
                    ++pos;
                }
            }
        }

        public void Div(GXBigInteger value)
        {
            GXBigInteger current = new GXBigInteger(1);
            GXBigInteger denom = new GXBigInteger(value);
            GXBigInteger tmp = new GXBigInteger(this);
            bool neq = IsNegative;
            IsNegative = false;
            try
            {
                // while denom < this.
                while (denom.Compare(this) == -1)
                {
                    current.Lshift(1);
                    denom.Lshift(1);
                }
                //If overflow.
                if (denom.Compare(this) == 1)
                {
                    if (current.IsOne)
                    {
                        Clear();
                        return;
                    }
                    Clear();
                    current.Rshift(1);
                    denom.Rshift(1);
                    while (!current.IsZero)
                    {
                        int r = tmp.Compare(denom);
                        if (r == 1)
                        {
                            tmp.Sub(denom);
                            Add(current);
                        }
                        else if (r == 0)
                        {
                            Add(current);
                            break;
                        }
                        current.Rshift(1);
                        denom.Rshift(1);
                    }
                    current.Data = Data;
                }
            }
            finally
            {
                IsNegative = neq;
            }
            Data = current.Data;
        }

        /// <summary>
        /// Modulus.
        /// </summary>
        /// <param name="mod">Modulus.</param>
        public void Mod(GXBigInteger mod)
        {
            GXBigInteger current = new GXBigInteger(1);
            GXBigInteger denom = new GXBigInteger(mod);
            bool neq = IsNegative;
            IsNegative = false;
            // while denom < this.
            while (denom.Compare(this) == -1)
            {
                current.Lshift(1);
                denom.Lshift(1);
            }
            //If overflow.
            if (denom.Compare(this) == 1)
            {
                if (current.IsOne)
                {
                    if (neq)
                    {
                        Sub(mod);
                        IsNegative = false;
                    }
                    return;
                }
                current.Rshift(1);
                denom.Rshift(1);
                while (!current.IsZero)
                {
                    int r = Compare(denom);
                    if (r == 1)
                    {
                        Sub(denom);
                    }
                    else if (r == 0)
                    {
                        break;
                    }
                    current.Rshift(1);
                    denom.Rshift(1);
                }
            }
            else
            {
                Clear();
            }
            if (neq)
            {
                Sub(mod);
                IsNegative = false;
            }
        }

        public void Inv(GXBigInteger value)
        {
            if (!IsZero)
            {
                GXBigInteger lm = new GXBigInteger(1);
                GXBigInteger hm = new GXBigInteger(0);
                GXBigInteger low = new GXBigInteger(this);
                low.Mod(value);
                GXBigInteger high = new GXBigInteger(value);
                while (!(low.IsZero || low.IsOne))
                {
                    GXBigInteger r = new GXBigInteger(high);
                    r.Div(low);
                    GXBigInteger tmp = new GXBigInteger(lm);
                    tmp.Multiply(r);
                    GXBigInteger nm = new GXBigInteger(hm);
                    nm.Sub(tmp);
                    tmp = new GXBigInteger(low);
                    tmp.Multiply(r);
                    high.Sub(tmp);
                    // lm, low, hm, high = nm, new, lm, low
                    tmp = low;
                    low = new GXBigInteger(high);
                    high = tmp;
                    hm = new GXBigInteger(lm);
                    lm = new GXBigInteger(nm);
                }
                Data = lm.Data;
                IsNegative = lm.IsNegative;
                Mod(value);
            }
        }

        public override string ToString()
        {
            string str = null;
            if (IsZero)
            {
                str = "0x00";
            }
            else
            {
                int pos, cnt = 0;
                GXByteBuffer bb = new GXByteBuffer();
                for (pos = Count - 1; pos != -1; --pos)
                {
                    bb.SetUInt32(Data[pos]);
                }
                for (pos = 0; pos != bb.Size; ++pos)
                {
                    if (bb.Data[pos] != 0)
                    {
                        cnt = bb.Size - pos;
                        break;
                    }
                }
                if (IsNegative)
                {
                    str = "-";
                }
                str += "0x";
                str += bb.ToHex(false, pos, cnt);
            }
            return str;
        }
    }
}
