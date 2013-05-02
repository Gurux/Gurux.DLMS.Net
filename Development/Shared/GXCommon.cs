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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Gurux.Shared
{
	class GXCommon
	{
		/// <summary>
		/// Convert object to byte array.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] GetAsByteArray(object value)
		{
			if (value == null)
			{
				return new byte[0];
			}
			if (value is string)
			{
				return Encoding.UTF8.GetBytes((string)value);
			}
			int rawsize = 0;
			byte[] rawdata = null;
			GCHandle handle;
			if (value is Array)
			{
				Array arr = value as Array;
				if (arr.Length != 0)
				{
					int valueSize = Marshal.SizeOf(arr.GetType().GetElementType());
					rawsize = valueSize * arr.Length;
					rawdata = new byte[rawsize];
					handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
					long pos = handle.AddrOfPinnedObject().ToInt64();
					foreach (object it in arr)
					{
						Marshal.StructureToPtr(it, new IntPtr(pos), false);
						pos += valueSize;
					}
					handle.Free();
					return rawdata;
				}
				return new byte[0];
			}

			rawsize = Marshal.SizeOf(value);
			rawdata = new byte[rawsize];
			handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
			Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
			handle.Free();
			return rawdata;
		}		

        public static object ByteArrayToObject(byte[] byteArray, Type type, int index, int count, bool reverse, out int readBytes)
		{            
            if (byteArray == null)
            {
                throw new ArgumentException("byteArray");
            }
            if (count <= 0)
            {
                count = byteArray.Length - index;
            }
            //If count is higger than one and type is not array.
            if (count != 1 && !type.IsArray && type != typeof(string))
            {
                throw new ArgumentException("count");
            }
            if (index < 0 || index > byteArray.Length)
            {
                throw new ArgumentException("index");
            }
            if (type == typeof(byte[]) && index == 0 && count == byteArray.Length)
            {
                readBytes = byteArray.Length;
                return byteArray;
            }
            readBytes = 0;
            Type valueType = null;
            int valueSize = 0;
            if (index != 0 || reverse)
            {                
                if (type == typeof(string))
                {
                    readBytes = count;
                }
                else if (type.IsArray)
                {
                    valueType = type.GetElementType();
                    valueSize = Marshal.SizeOf(valueType);
                    readBytes = (valueSize * count);
                }
                else if (type == typeof(bool) || type == typeof(Boolean))
                {
                    readBytes = 1;
                }
                else
                {
                    readBytes = Marshal.SizeOf(type);
                }
                byte[] tmp = byteArray;
                byteArray = new byte[readBytes];
                Array.Copy(tmp, index, byteArray, 0, readBytes);
            }                        			
			object value = null;
            if (type == typeof(string))
            {
                return Encoding.UTF8.GetString(byteArray);
            }
            else if (reverse)
            {
                byteArray = byteArray.Reverse().ToArray();
            }
			GCHandle handle;
			if (type.IsArray)
			{				
                if (count == -1)
                {
                    count = byteArray.Length / Marshal.SizeOf(valueType);
                }                
                Array arr = (Array)Activator.CreateInstance(type, count);
				handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
				long start = handle.AddrOfPinnedObject().ToInt64();
                for (int pos = 0; pos != count; ++pos)
				{
					arr.SetValue(Marshal.PtrToStructure(new IntPtr(start), valueType), pos);
					start += valueSize;
					readBytes += valueSize;
				}
				handle.Free();
				return arr;
			}
			handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);            
			value = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
			readBytes = Marshal.SizeOf(type);
			handle.Free();
			return value;
		}

        /// <summary>
        /// Convert received byte stream to wanted object.
        /// </summary>
        /// <param name="byteArray">Bytes to parse.</param>
        /// <param name="type">object type.</param>
        /// <param name="readBytes">Read byte count.</param>
        /// <returns></returns>
        public static object ByteArrayToObject(byte[] byteArray, Type type, out int readBytes)
        {
            return ByteArrayToObject(byteArray, type, 0, byteArray.Length, false, out readBytes);
        }

        /// <summary>
        /// Searches for the specified pattern and returns the index of the first occurrence
        /// within the range of elements in the byte buffer that starts at the specified
        /// index and contains the specified number of elements.
        /// </summary>
        /// <param name="input">Input byte buffer</param>
        /// <param name="pattern"></param>
        /// <param name="index">Index where search is started.</param>
        /// <param name="count">Maximum search buffer size.</param>
        /// <returns></returns>
		public static int IndexOf(byte[] input, byte[] pattern, int index, int count)
		{
			//If not enought data available.
            if (count < pattern.Length)
			{
				return -1;
			}
			byte firstByte = pattern[0];
			int pos = -1;
            if ((pos = Array.IndexOf(input, firstByte, index, count - index)) >= 0)
			{
				for (int i = 0; i < pattern.Length; i++)
				{
					if (pos + i >= input.Length || pattern[i] != input[pos + i])
					{
						return -1;
					}
				}
			}
			return pos;
		}
	}
}
