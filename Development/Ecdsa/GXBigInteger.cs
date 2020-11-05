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

namespace Gurux.DLMS.Ecdsa
{
    public class GXBigInteger
    {
        /// <summary>
        /// List of values. Least Significated is in the first item.
        /// </summary>
        private List<UInt32> Data = new List<UInt32>();
        /// <summary>
        /// Is value changed.
        /// </summary>
        private bool changed;
        /// <summary>
        /// Is value negative.
        /// </summary>
        private bool negative;
        /// <summary>
        /// Is value zero.
        /// </summary>
        private bool zero;
        /// <summary>
        /// Is value even.
        /// </summary>
        private bool even;
        /// <summary>
        /// Is value one.
        /// </summary>
        private bool one;

        /// <summary>
        /// Is value negative.
        /// </summary>
        /// <returns>True, if value is negative.</returns>
        public bool IsNegative
        {
            get
            {
                if (changed)
                {
                    UpdateBooleanProperties();
                }
                return negative;
            }
        }

        /// <summary>
        /// Is value zero.
        /// </summary>
        /// <returns>True, if value is zero.</returns>
        public bool IsZero
        {
            get
            {
                if (changed)
                {
                    UpdateBooleanProperties();
                }
                return zero;
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
                if (changed)
                {
                    UpdateBooleanProperties();
                }
                return even;
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
                if (changed)
                {
                    UpdateBooleanProperties();
                }
                return one;
            }
        }

        private void UpdateBooleanProperties()
        {
            int pos;
            //Remove extra zeroes.
            for (pos = Data.Count - 1; pos > 0; --pos)
            {
                if (Data[pos] != 0)
                {
                    break;
                }
            }
            if (pos != Data.Count - 1)
            {
                Data.RemoveRange(1 + pos, Data.Count - pos - 1);
            }

            one = even = false;
            if (Data.Count == 0)
            {
                zero = true;
            }
            else
            {
                zero = Data[0] == 0;
                if (zero)
                {
                    //Check is value zero.
                    for (pos = 1; pos < Data.Count; ++pos)
                    {
                        if (Data[pos] != 0)
                        {
                            zero = false;
                            break;
                        }
                    }
                }
                if (!zero)
                {
                    //Check is value even.
                    even = Data[0] % 2 == 0;
                    one = !even && Data[0] == 1;
                    if (one)
                    {
                        //Check is value One.
                        for (pos = 1; pos < Data.Count; ++pos)
                        {
                            if (Data[pos] != 0)
                            {
                                one = false;
                                break;
                            }
                        }
                    }
                }
            }
            changed = false;
        }


        /// <summary>
        /// Constuctor.
        /// </summary>
        public GXBigInteger()
        {
            zero = true;
        }

        /// <summary>
        /// Get values from byte buffer.
        /// </summary>
        /// <param name="value"></param>
        private void FromByteBuffer(GXByteBuffer value)
        {
            for (int pos = value.Size - 4; pos > -1; pos = pos - 4)
            {
                Data.Add(value.GetUInt32(pos));
            }
            switch (value.Size % 4)
            {
                case 1:
                    Data.Add(value.GetUInt8());
                    break;
                case 2:
                    Data.Add(value.GetUInt16());
                    break;
                case 3:
                    // Data.Add(value.GetUInt24());
                    break;
                default:
                    break;
            }
            changed = true;
        }

        /// <summary>
        /// Constuctor.
        /// </summary>
        /// <param name="value">Big integer value in MSB.</param>
        public GXBigInteger(string value)
        {
            negative = value.StartsWith("-");
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetHexString(value);
            FromByteBuffer(bb);
        }

        public GXBigInteger(UInt64 value) : this()
        {
            Data.Add((UInt32)value);
            value >>= 32;
            Data.Add((UInt32)value);
            changed = true;
        }

        public GXBigInteger(UInt32 value) : this()
        {
            Data.Add(value);
            changed = true;
        }

        public GXBigInteger(int value) : this()
        {
            Data.Add((UInt32)value);
            changed = true;
        }

        /// <summary>
        /// Constructor value.
        /// </summary>
        /// <param name="values">Data in MSB format.</param>
        public GXBigInteger(UInt32[] values) : this()
        {
            Data.AddRange(values);
            Data.Reverse();
            changed = true;
        }

        /// <summary>
        /// Constructor value.
        /// </summary>
        /// <param name="value">Byte array Data in MSB format.</param>
        public GXBigInteger(byte[] value) : this()
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.Set(value);
            FromByteBuffer(bb);
        }

        /// <summary>
        /// Constructor value.
        /// </summary>
        /// <param name="value">Byte array Data in MSB format.</param>
        public GXBigInteger(GXByteBuffer value) : this()
        {
            FromByteBuffer(value);
        }

        public GXBigInteger(GXBigInteger value) : this()
        {
            Data = new List<UInt32>(value.Data);
            negative = value.negative;
            changed = true;
        }

        /// <summary>
        /// Convert value to byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            int pos;
            GXByteBuffer bb = new GXByteBuffer();
            for (pos = 0; pos != Data.Count; ++pos)
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
            while (Data.Count < value.Data.Count)
            {
                Data.Add(0);
            }
            for (pos = 0; pos < value.Data.Count; ++pos)
            {
                Data[pos] |= value.Data[pos];
            }
            changed = true;
        }

        public void Add(GXBigInteger value)
        {
            if (value.negative)
            {
                value.negative = false;
                try
                {
                    Sub(value);
                }
                finally
                {
                    value.negative = true;
                }
            }
            else
            {
                while (Data.Count < value.Data.Count)
                {
                    Data.Add(0);
                }
                UInt64 overflow = 0;
                for (int pos = 0; pos != Data.Count; ++pos)
                {
                    UInt64 tmp = Data[pos];
                    if (pos < value.Data.Count)
                    {
                        tmp += value.Data[pos];
                    }
                    tmp += overflow;
                    Data[pos] = (UInt32)(tmp);
                    overflow = tmp >> 32;
                }
                if (overflow != 0)
                {
                    Data.Add((UInt32)overflow);
                }
                changed = true;
            }
        }


        public void Sub(GXBigInteger value)
        {
            int c = Compare(value);
            if (c == 0)
            {
                Clear();
            }
            else if (value.negative || c == -1)
            {
                if (!value.negative && !negative)
                {
                    //If biger value is decreased from smaller value.
                    GXBigInteger tmp = new GXBigInteger(value);
                    tmp.Sub(this);
                    Clear();
                    Data.AddRange(tmp.Data);
                    negative = true;
                    changed = true;
                }
                else
                {
                    //If negative value is decreased from the value.
                    bool ret = value.negative;
                    value.negative = false;
                    try
                    {
                        Add(value);
                        /*
                        GXBigInteger tmp = new GXBigInteger(value);
                      //  this.negative = false;
                        tmp.Sub(this);
                        //                    tmp.negative = false;
                        Clear();
                        Data.AddRange(tmp.Data);
                        changed = true;
                        //                    Add(value);
                        //                  Sub(tmp);
                        */
                    }
                    finally
                    {
                        value.negative = ret;
                        negative = !ret;
                    }
                }
            }
            else
            {
                if (!value.IsZero)
                {
                    if (IsZero)
                    {
                        negative = true;
                        Clear();
                        Data.AddRange(value.Data);
                    }
                    else
                    {
                        while (Data.Count < value.Data.Count)
                        {
                            Data.Add(0);
                        }
                        byte borrow = 0;
                        UInt64 tmp;
                        int pos;
                        for (pos = 0; pos != value.Data.Count; ++pos)
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
                            for (; pos != Data.Count; ++pos)
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
                    }
                    changed = true;
                }
            }
        }

        private static void AddValue(List<UInt32> list, UInt32 value, int index)
        {
            UInt64 tmp;
            if (index < list.Count)
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
                list.Add(value);
            }
        }

        public void Multiply(GXBigInteger value)
        {
            if (!value.IsOne)
            {
                List<UInt32> ret = new List<UInt32>();
                UInt32 overflow;
                int index = 0;
                for (int i = 0; i != value.Data.Count; ++i)
                {
                    overflow = 0;
                    for (int j = 0; j != Data.Count; ++j)
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
                    }
                }
                Data = ret;
                changed = true;
            }
            if (value.IsNegative != IsNegative)
            {
                if (!negative)
                {
                    negative = true;
                }
            }
            else if (IsNegative)
            {
                //If both values are negative.
                negative = false;
            }
        }

        public int Compare(GXBigInteger value)
        {
            int ret = 0;
            if (negative != value.negative)
            {
                //If other value is negative.
                if (negative)
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
                int cntA = Data.Count - 1;
                //Skip zero values.
                while (cntA != -1 && Data[cntA] == 0)
                {
                    --cntA;
                }
                int cntB = value.Data.Count - 1;
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
            return Compare(new GXBigInteger(value));
        }

        public void Lshift(int amount)
        {
            if (amount != 0)
            {
                UInt32 overflow = 0;
                for (int pos = 0; pos != Data.Count; ++pos)
                {
                    UInt64 tmp = Data[pos];
                    tmp <<= amount;
                    tmp |= overflow;
                    Data[pos] = (UInt32)tmp;
                    overflow = (UInt32)(tmp >> 32);
                }
                if (overflow != 0)
                {
                    Data.Add(overflow);
                }
                changed = true;
            }
        }

        public void Rshift(int amount)
        {
            UInt64 overflow = 0;
            UInt32 mask = 0xFFFFFFFF;
            mask = mask >> (32 - amount);
            int cnt = Data.Count - 1;
            for (int pos = cnt; pos != -1; --pos)
            {
                UInt64 tmp = Data[pos];
                Data[pos] = (UInt32)((tmp >> amount) | overflow);
                overflow = (tmp & mask) << (32 - amount);
            }
            //Remove last item if it's empty.
            if (Data.Count != 1 && Data[cnt] == 0)
            {
                Data.RemoveAt(cnt);
            }
            changed = true;
        }

        /// <summary>
        /// Reset value to Zero.
        /// </summary>
        public void Clear()
        {
            Data.Clear();
            changed = true;
            negative = false;
        }

        public void Div(GXBigInteger value)
        {
            GXBigInteger current = new GXBigInteger(1);
            GXBigInteger denom = new GXBigInteger(value);
            GXBigInteger tmp = new GXBigInteger(this);
            bool neq = negative;
            negative = false;
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
                negative = neq;
            }
            Data = current.Data;
            changed = true;
        }

        /// <summary>
        /// Modulus.
        /// </summary>
        /// <param name="mod">Modulus.</param>
        public void Mod(GXBigInteger mod)
        {
            /*
            //value = value - (mod * (value / mod) )
            GXBigInteger tmp = new GXBigInteger(this);
            tmp.Div(mod);
            tmp.Multiply(mod);
            Sub(tmp);
            changed = true;
*/
            GXBigInteger current = new GXBigInteger(1);
            GXBigInteger denom = new GXBigInteger(mod);
            bool neq = negative;
            negative = false;
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
                        negative = false;
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
                negative = false;
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
                negative = lm.negative;
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
                for (pos = Data.Count - 1; pos != -1; --pos)
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
                if (negative)
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
