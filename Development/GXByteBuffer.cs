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
    using System.Text;
    using System.Diagnostics;

    /// <summary>
    /// Byte array class is used to save received bytes.
    /// </summary>
    public class GXByteBuffer
    {
        private int position;
        private int size;

        /// <summary>
        /// Array capacity increase size.
        /// </summary>
        const int ArrayCapacity = 10;

        ///<summary>
        ///Constructor.
        ///</summary>
        [DebuggerStepThrough]
        public GXByteBuffer()
        {

        }


        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="capacity">
        /// Buffer capacity.
        ///</param>
        [DebuggerStepThrough]
        public GXByteBuffer(UInt16 capacity)
        {
            Capacity = capacity;
        }


        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="value">
        /// Byte array to attach.
        ///</param>
        [DebuggerStepThrough]
        public GXByteBuffer(byte[] value)
        {
            if (value != null)
            {
                Capacity = value.Length;
                Set(value);
            }
        }

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="value">
        /// Byte array to attach.
        ///</param>
        [DebuggerStepThrough]
        public GXByteBuffer(GXByteBuffer value)
        {
            Capacity = (value.Size - value.Position);
            Set(value);
        }

        ///<summary>
        /// Clear buffer but do not release memory.
        ///</summary>
        public void Clear()
        {
            Position = 0;
            Size = 0;
        }


        ///<summary>
        /// Buffer capacity.
        ///</summary>
        public int Capacity
        {
            get
            {
                if (Data == null)
                {
                    return 0;
                }
                return Data.Length;
            }
            set
            {
                if (value == 0)
                {
                    Data = null;
                    Size = 0;
                }
                else
                {
                    if (Data == null)
                    {
                        Data = new byte[value];
                    }
                    else
                    {
                        byte[] tmp = Data;
                        Data = new byte[value];
                        if (Size < value)
                        {
                            Buffer.BlockCopy(tmp, 0, Data, 0, Size);
                        }
                        else
                        {
                            Buffer.BlockCopy(tmp, 0, Data, 0, (int)Capacity);
                            Size = Capacity;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Byte buffer read position.
        /// </summary>
        public int Position
        {
            get
            {
                return position;
            }
            set
            {
                if (value > Size || value < 0)
                {
                    throw new ArgumentOutOfRangeException("Position");
                }
                position = value;
            }
        }

        /// <summary>
        /// Byte buffer data size.
        /// </summary>
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                if (value > Capacity || value < 0)
                {
                    throw new ArgumentOutOfRangeException("Size");
                }
                size = value;
                if (position > size)
                {
                    position = size;
                }
            }
        }

        /// <summary>
        /// Amount of not reed bytes in the buffer.
        /// </summary>
        public int Available
        {
            get
            {
                return size - position;
            }
        }

        /// <summary>
        /// Returs data as byte array.
        /// </summary>
        /// <returns>Byte buffer as a byte array.</returns>
        public byte[] Array()
        {
            if (Capacity == Size)
            {
                return Data;
            }
            return SubArray(0, Size);
        }

        /// <summary>
        /// Returns data as byte array.
        /// </summary>
        /// <returns>Byte buffer as a byte array.</returns>
        public byte[] SubArray(int index, int count)
        {
            byte[] tmp = new byte[count];
            Buffer.BlockCopy(Data, index, tmp, 0, count);
            return tmp;
        }

        /// <summary>
        /// Move content from source to destination.
        /// </summary>
        /// <param name="srcPos">Source position.</param>
        /// <param name="destPos">Destination position.</param>
        /// <param name="count">Item count.</param>
        public void Move(int srcPos, int destPos, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (count != 0)
            {
                if (destPos + count > Size)
                {
                    Capacity = destPos + count;
                }
                Buffer.BlockCopy(Data, srcPos, Data, destPos, count);
                Size = (destPos + count);
                if (Position > Size)
                {
                    Position = Size;
                }
            }
        }

        /// <summary>
        /// Remove handled bytes.
        /// </summary>
        /// <remarks>
        /// This can be used in debugging to remove handled bytes.
        /// </remarks>
        public void Trim()
        {
            if (Size == Position)
            {
                Size = 0;
            }
            else
            {
                Move(Position, 0, Size - Position);
            }
            Position = 0;
        }

        /// <summary>
        /// Push the given byte into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="value">The value to be added.</param>
        public void SetUInt8(byte value)
        {
            SetUInt8(Size, value);
            ++Size;
        }

        /// <summary>
        /// Push the given enumeration value as byte into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="value">The value to be added.</param>
        internal void SetUInt8(Enum value)
        {
            SetUInt8(Convert.ToByte(value));
        }

        /// <summary>
        /// Get UInt8 value from byte array from the current position and then increments the position.
        /// </summary>
        public byte GetUInt8()
        {
            byte value = GetUInt8(Position);
            ++Position;
            return value;
        }

        /// <summary>
        /// Push the given UInt8 into this buffer at the given position.
        /// </summary>
        /// <param name="index">Zero based byte index where value is set.</param>
        /// <param name="value"> The byte to be added.</param>
        public void SetUInt8(int index, byte value)
        {
            if (index >= Capacity)
            {
                Capacity = (index + ArrayCapacity);
            }
            Data[index] = value;
        }

        /// <summary>
        /// Push the given UInt16 into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="value">The value to be added.</param>
        public void SetUInt16(UInt16 value)
        {
            SetUInt16(Size, value);
            Size += 2;
        }

        /// <summary>
        /// Push the given Int16 into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="value">The value to be added.</param>
        public void SetInt16(Int16 value)
        {
            SetInt16(Size, value);
            Size += 2;
        }

        /// <summary>
        /// Get UInt16 value from byte array from the current position and then increments the position.
        /// </summary>
        public UInt16 GetUInt16()
        {
            UInt16 value = GetUInt16(Position);
            Position += 2;
            return value;
        }

        /// <summary>
        /// Push the given UInt16 into this buffer at the given position.
        /// </summary>
        /// <param name="index">Zero based byte index where value is set.</param>
        /// <param name="value">The value to be added.</param>
        public void SetUInt16(int index, UInt16 value)
        {
            if (index + 2 >= Capacity)
            {
                Capacity = (index + ArrayCapacity);
            }
            Data[index] = (byte)((value >> 8) & 0xFF);
            Data[index + 1] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Push the given Int16 into this buffer at the given position.
        /// </summary>
        /// <param name="index">Zero based byte index where value is set.</param>
        /// <param name="value">The value to be added.</param>
        public void SetInt16(int index, Int16 value)
        {
            if (index + 2 >= Capacity)
            {
                Capacity = (index + ArrayCapacity);
            }
            Data[index] = (byte)((value >> 8) & 0xFF);
            Data[index + 1] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Push the given UInt32 into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="value"> The value to be added.</param>
        public void SetUInt32(UInt32 value)
        {
            SetUInt32(Size, value);
            Size += 4;
        }

        /// <summary>
        /// Push the given UInt32 into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="value"> The value to be added.</param>
        public void SetInt32(Int32 value)
        {
            SetInt32(Size, value);
            Size += 4;
        }

        /// <summary>
        /// Get UInt32 value from byte array from the current position and then increments the position.
        /// </summary>
        public UInt32 GetUInt32()
        {
            UInt32 value = GetUInt32(Position);
            Position += 4;
            return value;
        }

        /// <summary>
        /// Push the given UInt32 into this buffer at the given position.
        /// </summary>
        /// <param name="index">Zero based byte index where value is set.</param>
        /// <param name="value"> The value to be added.</param>
        public void SetUInt32(int index, UInt32 value)
        {
            if (index + 4 >= Capacity)
            {
                Capacity = (index + ArrayCapacity);
            }
            Data[index] = (byte)((value >> 24) & 0xFF);
            Data[index + 1] = (byte)((value >> 16) & 0xFF);
            Data[index + 2] = (byte)((value >> 8) & 0xFF);
            Data[index + 3] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Push the given UInt32 into this buffer at the given position.
        /// </summary>
        /// <param name="index">Zero based byte index where value is set.</param>
        /// <param name="value"> The value to be added.</param>
        public void SetInt32(int index, Int32 value)
        {
            if (index + 4 >= Capacity)
            {
                Capacity = (index + ArrayCapacity);
            }
            Data[index] = (byte)((value >> 24) & 0xFF);
            Data[index + 1] = (byte)((value >> 16) & 0xFF);
            Data[index + 2] = (byte)((value >> 8) & 0xFF);
            Data[index + 3] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Push the given UInt64 into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="value"> The value to be added.</param>
        public void SetUInt64(UInt64 value)
        {
            SetUInt64(Size, value);
            Size += 8;
        }

        /// <summary>
        /// Push the given UInt64 into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="value"> The value to be added.</param>
        public void SetInt64(Int64 value)
        {
            SetInt64(Size, value);
            Size += 8;
        }


        /// <summary>
        /// Get UInt64 value from byte array from the current position and then increments the position.
        /// </summary>
        public UInt64 GetUInt64()
        {
            if (Position + 8 > Size)
            {
                throw new System.OutOfMemoryException();
            }
            UInt64 value = (UInt64)((Data[Position] & 0xFF) << 58 |
                                    (Data[Position + 1] & 0xFF) << 48 |
                                    (Data[Position + 2] & 0xFF) << 40 |
                                    (Data[Position + 3] & 0xFF) << 32 |
                                    (Data[Position + 4] & 0xFF) << 24 |
                                    (Data[Position + 5] & 0xFF) << 16 |
                                    (Data[Position + 6] & 0xFF) << 8 |
                                    (Data[Position + 7] & 0xFF));
            Position += 8;
            return value;
        }

        /// <summary>
        /// Push the given UInt64 into this buffer at the given position.
        /// </summary>
        /// <param name="index">Zero based byte index where value is set.</param>
        /// <param name="item"> The value to be added.</param>
        public void SetUInt64(int index, UInt64 item)
        {
            if (index + 8 >= Capacity)
            {
                Capacity = (index + ArrayCapacity);
            }
            Data[Size] = (byte)((item >> 56) & 0xFF);
            Data[Size + 1] = (byte)((item >> 48) & 0xFF);
            Data[Size + 2] = (byte)((item >> 40) & 0xFF);
            Data[Size + 3] = (byte)((item >> 32) & 0xFF);
            Data[Size + 4] = (byte)((item >> 24) & 0xFF);
            Data[Size + 5] = (byte)((item >> 16) & 0xFF);
            Data[Size + 6] = (byte)((item >> 8) & 0xFF);
            Data[Size + 7] = (byte)(item & 0xFF);
        }

        /// <summary>
        /// Push the given UInt64 into this buffer at the given position.
        /// </summary>
        /// <param name="index">Zero based byte index where value is set.</param>
        /// <param name="item"> The value to be added.</param>
        public void SetInt64(int index, Int64 item)
        {
            if (index + 8 >= Capacity)
            {
                Capacity = (index + ArrayCapacity);
            }
            Data[Size + 7] = (byte)((item >> 56) & 0xFF);
            Data[Size + 6] = (byte)((item >> 48) & 0xFF);
            Data[Size + 5] = (byte)((item >> 40) & 0xFF);
            Data[Size + 4] = (byte)((item >> 32) & 0xFF);
            Data[Size + 3] = (byte)((item >> 24) & 0xFF);
            Data[Size + 2] = (byte)((item >> 16) & 0xFF);
            Data[Size + 1] = (byte)((item >> 8) & 0xFF);
            Data[Size] = (byte)(item & 0xFF);
        }

        /// <summary>
        /// Get Int8 value from byte array from the current position and then increments the position.
        /// </summary>
        public sbyte GetInt8()
        {
            sbyte value = (sbyte)GetUInt8(Position);
            ++Position;
            return value;
        }

        /// <summary>
        /// Get UInt8 value from byte array.
        /// </summary>
        /// <param name="index">Byte index.</param>
        public byte GetUInt8(int index)
        {
            if (index >= Size)
            {
                throw new System.OutOfMemoryException();
            }
            return Data[index];
        }

        /// <summary>
        /// Get UInt16 value from byte array.
        /// </summary>
        /// <param name="index">Byte index.</param>
        public UInt16 GetUInt16(int index)
        {
            if (index + 2 > Size)
            {
                throw new System.OutOfMemoryException();
            }
            return (UInt16)(((Data[index] & 0xFF) << 8) | (Data[index + 1] & 0xFF));
        }

        /// <summary>
        /// Get UInt32 value from byte array from the current position and then increments the position.
        /// </summary>
        public Int32 GetInt32()
        {
            Int32 value = (Int32)GetUInt32(Position);
            Position += 4;
            return value;
        }

        /// <summary>
        /// Get Int16 value from byte array from the current position and then increments the position.
        /// </summary>
        public Int16 GetInt16()
        {
            Int16 value = (Int16)GetInt16(Position);
            Position += 2;
            return value;
        }

        /// <summary>
        /// Get Int16 value from byte array.
        /// </summary>
        /// <param name="index">Byte index.</param>
        public Int16 GetInt16(int index)
        {
            if (index + 2 > Size)
            {
                throw new System.OutOfMemoryException();
            }
            return (Int16)(((Data[index] & 0xFF) << 8) | (Data[index + 1] & 0xFF));
        }

        /// <summary>
        /// Get Int32 value from byte array.
        /// </summary>
        /// <param name="index">Byte index.</param>
        public UInt32 GetUInt32(int index)
        {
            if (index + 4 > Size)
            {
                throw new System.OutOfMemoryException();
            }
            return (UInt32)((Data[index] & 0xFF) << 24 | (Data[index + 1] & 0xFF) << 16 |
                            (Data[index + 2] & 0xFF) << 8 | (Data[index + 3] & 0xFF));
        }

        /// <summary>
        /// Get float value from byte array from the current position and then increments the position.
        /// </summary>
        public float GetFloat()
        {
            byte[] tmp = new byte[4];
            Get(tmp);
            System.Array.Reverse(tmp, 0, 4);
            float value = BitConverter.ToSingle(tmp, 0);
            return value;
        }

        /// <summary>
        /// Set float value to byte array.
        /// </summary>
        public void SetFloat(float value)
        {
            byte[] tmp = BitConverter.GetBytes(value);
            System.Array.Reverse(tmp, 0, 4);
            Set(tmp);

        }
        /// <summary>
        /// Set float value to byte array.
        /// </summary>
        public void SetDouble(double value)
        {
            byte[] tmp = BitConverter.GetBytes(value);
            System.Array.Reverse(tmp, 0, 8);
            Set(tmp);
        }
        /// <summary>
        /// Get double value from byte array from the current position and then increments the position.
        /// </summary>
        public double GetDouble()
        {
            byte[] tmp = new byte[8];
            Get(tmp);
            System.Array.Reverse(tmp, 0, 8);
            double value = BitConverter.ToDouble(tmp, 0);
            return value;
        }

        /// <summary>
        /// Get Int64 value from byte array from the current position and then increments the position.
        /// </summary>
        public Int64 GetInt64()
        {
            if (Position + 8 > Size)
            {
                throw new System.OutOfMemoryException();
            }
            Int64 value = (Int64)((Data[Position] & 0xFF) << 58 |
                                  (Data[Position + 1] & 0xFF) << 48 |
                                  (Data[Position + 2] & 0xFF) << 40 |
                                  (Data[Position + 3] & 0xFF) << 32 |
                                  (Data[Position + 4] & 0xFF) << 24 |
                                  (Data[Position + 5] & 0xFF) << 16 |
                                  (Data[Position + 6] & 0xFF) << 8 |
                                  (Data[Position + 7] & 0xFF));
            Position += 8;
            return value;
        }


        /// <summary>
        /// The data as byte array.
        /// </summary>
        public byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// Check is byte buffer ASCII string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAsciiString(byte[] value)
        {
            if (value != null)
            {
                foreach (char it in value)
                {
                    if (it == 0)
                    {
                        return true;
                    }
                    if ((it < 32 || it > 127) && it != '\r' && it != '\n' && it != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Get String value from byte array.
        /// </summary>
        /// <param name="count">Byte count.</param>
        public string GetString(int count)
        {
            if (count == 0)
            {
                return string.Empty;
            }
            byte[] tmp = new byte[count];
            Get(tmp);
            if (IsAsciiString(tmp))
            {
                string str = ASCIIEncoding.ASCII.GetString(tmp);
                int pos = str.IndexOf('\0');
                if (pos != -1)
                {
                    str = str.Substring(0, pos);
                }
                return str;
            }
            return Gurux.DLMS.Internal.GXCommon.ToHex(tmp, true);
        }

        /// <summary>
        /// Get String value from byte array.
        /// </summary>
        /// <param name="index">Byte index.</param>
        /// <param name="count">Byte count.</param>
        public string GetString(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (index + count > Size)
            {
                throw new System.OutOfMemoryException();
            }
            return ASCIIEncoding.ASCII.GetString(Data, index, count);
        }

        /// <summary>
        /// Get String value from byte array.
        /// </summary>
        /// <param name="index">Byte index.</param>
        /// <param name="count">Byte count.</param>
        public string GetStringUtf8(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            return Encoding.UTF8.GetString(Data, index, count);
        }

        /// <summary>
        /// Push the given byte array into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="value"> The value to be added.</param>
        public void Set(byte[] value)
        {
            if (value != null)
            {
                Set(value, 0, value.Length);
            }
        }

        /// <summary>
        /// Push the given byte array into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="index">Byte index.</param>
        /// <param name="value">The value to be added.</param>
        public void Set(int index, byte[] value)
        {
            if (value != null)
            {
                if (Capacity == 0)
                {
                    Capacity = value.Length;
                }
                else
                {
                    Move(index, value.Length, Size - index);
                }
                Buffer.BlockCopy(value, 0, Data, index, value.Length);
                if (index + value.Length > Size)
                {
                    Size = index + value.Length;
                }
            }
        }

        /// <summary>
        /// Set new value to byte array.
        /// </summary>
        /// <param name="value">Byte array to add.</param>
        /// <param name="index">Byte index.</param>
        /// <param name="count">Byte count.</param>
        public void Set(byte[] value, int index, int count)
        {
            if (value != null && count != 0)
            {
                if (count == -1)
                {
                    count = value.Length - index;
                }
                if (Size + count > Capacity)
                {
                    Capacity = (Size + count + ArrayCapacity);
                }
                Buffer.BlockCopy(value, index, Data, Size, count);
                Size += count;
            }
        }

        public void Set(GXByteBuffer value)
        {
            if (value != null)
            {
                Set(value, value.Size - value.Position);
            }
        }

        /// <summary>
        /// Set new value to byte array.
        /// </summary>
        /// <param name="value">Byte array to add.</param>
        /// <param name="count">Byte count.</param>
        public void Set(GXByteBuffer value, int count)
        {
            if (Size + count > Capacity)
            {
                Capacity = (Size + count + ArrayCapacity);
            }
            if (count != 0)
            {
                Buffer.BlockCopy(value.Data, value.Position, Data, Size, count);
                Size += count;
                value.Position += (ushort)count;
            }
        }

        /// <summary>
        /// Add new object to the byte buffer.
        /// </summary>
        /// <param name="value">Value to add.</param>
        public void Add(object value)
        {
            if (value != null)
            {
                if (value is byte[])
                {
                    Set((byte[])value);
                }
                else if (value is byte)
                {
                    SetUInt8((byte)value);
                }
                else if (value is UInt16)
                {
                    SetUInt16((UInt16)value);
                }
                else if (value is UInt32)
                {
                    SetUInt32((UInt32)value);
                }
                else if (value is UInt64)
                {
                    SetUInt64((UInt64)value);
                }
                else if (value is Int16)
                {
                    SetInt16((Int16)value);
                }
                else if (value is Int32)
                {
                    SetInt32((Int32)value);
                }
                else if (value is Int64)
                {
                    SetInt64((Int64)value);
                }
                else if (value is string)
                {
                    Set(ASCIIEncoding.ASCII.GetBytes((string)value));
                }
                else
                {
                    throw new ArgumentException("Invalid object type.");
                }
            }
        }

        /// <summary>
        /// Get value from the byte array.
        /// </summary>
        /// <param name="target">Target array.</param>
        public void Get(byte[] target)
        {
            if (Size - Position < target.Length)
            {
                throw new OutOfMemoryException();
            }
            Buffer.BlockCopy(Data, Position, target, 0, target.Length);
            Position += target.Length;
        }

        /// <summary>
        /// Compares, whether two given arrays are similar starting from current position.
        /// </summary>
        /// <param name="arr">Array to compare.</param>
        /// <returns>True, if arrays are similar. False, if the arrays differ.</returns>
        public bool Compare(byte[] arr)
        {
            if (arr == null || Size - Position < arr.Length)
            {
                return false;
            }
            byte[] bytes = new byte[arr.Length];
            Get(bytes);
            bool ret = Gurux.DLMS.Internal.GXCommon.Compare(bytes, arr);
            if (!ret)
            {
                this.Position -= arr.Length;
            }
            return ret;
        }

        /// <summary>
        /// Push the given hex string as byte array into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="value"> The hex string to be added.</param>
        public void SetHexString(string value)
        {
            Set(Gurux.DLMS.Internal.GXCommon.HexToBytes(value));
        }

        /// <summary>
        /// Push the given hex string as byte array into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="index">Byte index.</param>
        /// <param name="value"> The hex string to be added.</param>
        public void SetHexString(int index, string value)
        {
            Set(index, Gurux.DLMS.Internal.GXCommon.HexToBytes(value));
        }

        /// <summary>
        /// Push the given hex string as byte array into this buffer at the current position, and then increments the position.
        /// </summary>
        /// <param name="value">Byte array to add.</param>
        /// <param name="index">Byte index.</param>
        /// <param name="count">Byte count.</param>
        public void SetHexString(string value, int index, int count)
        {
            Set(Gurux.DLMS.Internal.GXCommon.HexToBytes(value), index, count);
        }

        public override string ToString()
        {
            return Gurux.DLMS.Internal.GXCommon.ToHex(Data, true, 0, Size);
        }

        /// <summary>
        /// Get remaining data as a string.
        /// </summary>
        /// <returns>Remaining data as string</returns>
        public byte[] Remaining()
        {
            return SubArray(position, size - position);
        }


        /// <summary>
        /// Get remaining data as hex string.
        /// </summary>
        /// <returns>Remaining data as hex string</returns>
        public string RemainingHexString()
        {
            return Gurux.DLMS.Internal.GXCommon.ToHex(Data, true, position, size - position);
        }

    }
}
