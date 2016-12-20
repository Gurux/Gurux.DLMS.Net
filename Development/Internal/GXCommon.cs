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
using Gurux.DLMS.Enums;
using System.Diagnostics;

namespace Gurux.DLMS.Internal
{
    /// <summary>
    /// Reserved for internal use.
    /// </summary>
    internal class GXCommon
    {
        internal const byte HDLCFrameStartEnd = 0x7E;
        internal static readonly byte[] LogicalNameObjectID = { 0x60, 0x85, 0x74, 0x05, 0x08, 0x01, 0x01 };
        internal static readonly byte[] ShortNameObjectID = { 0x60, 0x85, 0x74, 0x05, 0x08, 0x01, 0x02 };
        internal static readonly byte[] LogicalNameObjectIdWithCiphering = { 0x60, (byte)0x85, 0x74, 0x05, 0x08, 0x01, 0x03 };
        internal static readonly byte[] ShortNameObjectIdWithCiphering = { 0x60, (byte)0x85, 0x74, 0x05, 0x08, 0x01, 0x04 };

        /// <summary>
        /// Sent LLC bytes.
        /// </summary>
        internal static readonly byte[] LLCSendBytes = { 0xE6, 0xE6, 0x00 };
        /// <summary>
        /// Received LLC bytes.
        /// </summary>
        internal static readonly byte[] LLCReplyBytes = { 0xE6, 0xE7, 0x00 };

        public static byte GetSize(Object value)
        {
            if (value is byte)
            {
                return 1;
            }
            if (value is UInt16)
            {
                return 2;
            }
            if (value is UInt32)
            {
                return 4;
            }
            if (value is UInt64)
            {
                return 8;
            }
            throw new ArgumentException("Invalid object type.");
        }

        /// <summary>
        /// Convert char hex value to byte value.
        /// </summary>
        /// <param name="c">Character to convert hex.</param>
        /// <returns> Byte value of hex char value.</returns>
        [DebuggerStepThrough]
        private static byte GetValue(byte c)
        {
            byte value = 0xFF;
            //If number
            if (c > 0x2F && c < 0x3A)
            {
                value = (byte)(c - '0');
            }
            //If uppercase.
            else if (c > 0x40 && c < 'G')
            {
                value = (byte)(c - 'A' + 10);
            }
            //If lowercase.
            else if (c > 0x60 && c < 'g')
            {
                value = (byte)(c - 'a' + 10);
            }
            return value;
        }

        [DebuggerStepThrough]
        private static bool IsHex(byte c)
        {
            return GetValue(c) != 0xFF;
        }

        /// <summary>
        ///  Convert string to byte array.
        /// </summary>
        /// <param name="value">Hex string.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static byte[] HexToBytes(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return new byte[0];
            }
            int len = value.Length / 2;
            if (value.Length % 2 != 0)
            {
                ++len;
            }
            byte[] buffer = new byte[len];
            int lastValue = -1;
            int index = 0;
            foreach (byte ch in value)
            {
                if (IsHex(ch))
                {
                    if (lastValue == -1)
                    {
                        lastValue = GetValue(ch);
                    }
                    else if (lastValue != -1)
                    {
                        buffer[index] = (byte)(lastValue << 4 | GetValue(ch));
                        lastValue = -1;
                        ++index;
                    }
                }
                else if (lastValue != -1 && ch == ' ')
                {
                    buffer[index] = GetValue(ch);
                    lastValue = -1;
                    ++index;
                }
                else
                {
                    lastValue = -1;
                }
            }
            if (lastValue != -1)
            {
                buffer[index] = (byte)lastValue;
                ++index;
            }

            //If there are no spaces in the hex string.
            if (buffer.Length == index)
            {
                return buffer;
            }
            byte[] tmp = new byte[index];
            Buffer.BlockCopy(buffer, 0, tmp, 0, index);
            return tmp;
        }

        [DebuggerStepThrough]
        public static string ToHex(byte[] bytes, bool addSpace)
        {
            return ToHex(bytes, addSpace, 0, bytes == null ? 0 : bytes.Length);
        }

        /// <summary>
        /// Convert byte array to hex string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <param name="addSpace">Is space added between bytes.</param>
        /// <returns>Byte array as hex string.</returns>
        [DebuggerStepThrough]
        public static string ToHex(byte[] bytes, bool addSpace, int index, int count)
        {
            if (bytes == null || bytes.Length == 0 || count == 0)
            {
                return string.Empty;
            }
            char[] str = new char[count * 3];
            int tmp;
            int len = 0;
            for (int pos = 0; pos != count; ++pos)
            {
                tmp = (bytes[index + pos] >> 4);
                str[len] = (char)(tmp > 9 ? tmp + 0x37 : tmp + 0x30);
                ++len;
                tmp = (bytes[index + pos] & 0x0F);
                str[len] = (char)(tmp > 9 ? tmp + 0x37 : tmp + 0x30);
                ++len;
                if (addSpace)
                {
                    str[len] = ' ';
                    ++len;
                }
            }
            if (addSpace)
            {
                --len;
            }
            return new string(str, 0, len);
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="BitMask"></param>
        /// <param name="val"></param>
        internal static void SetBits(ref byte value, byte BitMask, bool val)
        {
            value &= (byte)~BitMask;
            //Set bit.
            if (val)
            {
                value |= BitMask;
            }
            else //Clear bit.
            {
                value &= (byte)~BitMask;
            }
        }

        /// <summary>
        /// Is bit set.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="BitMask"></param>
        /// <returns></returns>
        internal static bool GetBits(byte value, int BitMask)
        {
            return (value & BitMask) != 0;
        }

        /// <summary>
        /// Get HDLC address from byte array.
        /// </summary>
        /// <param name="GXByteBuffer">Byte array.</param>
        /// <returns>HDLC address.</returns>
        public static int GetHDLCAddress(GXByteBuffer buff)
        {
            int size = 0;
            for (int pos = buff.Position; pos != buff.Size; ++pos)
            {
                ++size;
                if ((buff.GetUInt8(pos) & 0x1) == 1)
                {
                    break;
                }
            }
            if (size == 1)
            {
                return (byte)((buff.GetUInt8() & 0xFE) >> 1);
            }
            else if (size == 2)
            {
                size = buff.GetUInt16();
                size = ((size & 0xFE) >> 1) | ((size & 0xFE00) >> 2);
                return size;
            }
            else if (size == 4)
            {
                UInt32 tmp = buff.GetUInt32();
                tmp = ((tmp & 0xFE) >> 1) | ((tmp & 0xFE00) >> 2)
                      | ((tmp & 0xFE0000) >> 3) | ((tmp & 0xFE000000) >> 4);
                return (int)tmp;
            }
            throw new ArgumentException("Wrong size.");
        }

        internal byte[] GetLogicalName(string logicalName)
        {
            byte[] ln = new byte[6];
            string[] items = logicalName.Split('.');
            if (items.Length != 6)
            {
                throw new Exception("Invalid Logical name.");
            }
            int pos = -1;
            foreach (string it in items)
            {
                ln[++pos] = byte.Parse(it);
            }
            return ln;
        }

        /// <summary>
        /// Return how many bytes object count takes.
        /// </summary>
        /// <param name="count">Value.</param>
        /// <returns>Value size in bytes.</returns>
        internal static int GetObjectCountSizeInBytes(int count)
        {
            if (count < 0x80)
            {
                return 1;
            }
            else if (count < 0x100)
            {
                return 2;
            }
            else if (count < 0x10000)
            {
                return 3;
            }
            else
            {
                return 5;
            }
        }

        /// <summary>
        /// Add string to byte buffer.
        /// </summary>
        /// <param name="value">String to add.</param>
        /// <param name="bb">Byte buffer where string is added.</param>
        public static void AddString(string value, GXByteBuffer bb)
        {
            bb.SetUInt8((byte)DataType.OctetString);
            if (value == null)
            {
                GXCommon.SetObjectCount(0, bb);
            }
            else
            {
                GXCommon.SetObjectCount(value.Length, bb);
                bb.Set(ASCIIEncoding.ASCII.GetBytes(value));
            }
        }

        /// <summary>
        /// Set item count.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="buff"></param>
        internal static void SetObjectCount(int count, GXByteBuffer buff)
        {
            if (count < 0x80)
            {
                buff.SetUInt8((byte)count);
            }
            else if (count < 0x100)
            {
                buff.SetUInt8(0x81);
                buff.SetUInt8((byte)count);
            }
            else if (count < 0x10000)
            {
                buff.SetUInt8(0x82);
                buff.SetUInt16((UInt16)count);
            }
            else
            {
                buff.SetUInt8(0x84);
                buff.SetUInt32((UInt32)count);
            }
        }

        /// <summary>
        /// Get object count. If first byte is 0x80 or higger it will tell bytes count.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <returns>Object count.</returns>
        public static int GetObjectCount(GXByteBuffer data)
        {
            int cnt = data.GetUInt8();
            if (cnt > 0x80)
            {
                if (cnt == 0x81)
                {
                    return data.GetUInt8();
                }
                else if (cnt == 0x82)
                {
                    return data.GetUInt16();
                }
                else if (cnt == 0x84)
                {
                    return (int)data.GetUInt32();
                }
                else
                {
                    throw new System.ArgumentException("Invalid count.");
                }
            }
            return cnt;
        }

        /// <summary>
        /// Compares, whether two given arrays are similar.
        /// </summary>
        /// <param name="arr1">First array to compare.</param>
        /// <param name="arr2">Second array to compare.</param>
        /// <returns>True, if arrays are similar. False, if the arrays differ.</returns>
        public static bool Compare(byte[] arr1, byte[] arr2)
        {
            //If other array is null and other is not.
            if (arr1 == null && arr2 != null || arr1 != null && arr2 == null)
            {
                return false;
            }
            if (arr1.Length != arr2.Length)
            {
                return false;
            }
            int pos;
            for (pos = 0; pos != arr2.Length; ++pos)
            {
                if (arr1[pos] != arr2[pos])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Append bitstring to string.
        /// </summary>
        /// <param name="sb">String where bit string is append.</param>
        /// <param name="value">Byte value</param>
        /// <param name="count">Bit count to add.</param>
        static void ToBitString(StringBuilder sb, byte value, int count)
        {
            if (count > 8)
            {
                count = 8;
            }
            char[] data = new char[count];
            for (int pos = 0; pos != count; ++pos)
            {
                if ((value & (1 << pos)) != 0)
                {
                    data[count - pos - 1] = '1';
                }
                else
                {
                    data[count - pos - 1] = '0';
                }
            }
            sb.Append(data);
        }


        ///<summary>
        ///Get data from DLMS frame.
        ///</summary>
        ///<param name="settings">DLMS settings.</param>
        ///<param name="data">Received data.</param>
        ///<param name="info"> Data info.</param>
        ///<returns>Parsed object.</returns>
        ///
        public static object GetData(GXDLMSSettings settings, GXByteBuffer data, GXDataInfo info)
        {
            object value = null;
            int startIndex = data.Position;
            if (data.Position == data.Size)
            {
                info.Complete = false;
                return null;
            }
            info.Complete = true;
            bool knownType = info.Type != DataType.None;
            // Get data type if it is unknown.
            if (!knownType)
            {
                info.Type = (DataType)data.GetUInt8();
            }
            if (info.Type == DataType.None)
            {
                if (info.xml != null)
                {
                    info.xml.AppendLine("<" + info.xml.GetDataType(info.Type) + " />");
                }
                return value;
            }
            if (data.Position == data.Size)
            {
                info.Complete = false;
                return null;
            }
            switch (info.Type)
            {
                case DataType.Array:
                case DataType.Structure:
                    value = GetArray(data, info, startIndex);
                    break;
                case DataType.Boolean:
                    value = GetBoolean(data, info);
                    break;
                case DataType.BitString:
                    value = GetBitString(data, info);
                    break;
                case DataType.Int32:
                    value = GetInt32(data, info);
                    break;
                case DataType.UInt32:
                    value = GetUInt32(data, info);
                    break;
                case DataType.String:
                    value = GetString(data, info, knownType);
                    break;
                case DataType.StringUTF8:
                    value = GetUtfString(data, info, knownType);
                    break;
                case DataType.OctetString:
                    value = GetOctetString(data, info, knownType);
                    break;
                case DataType.Bcd:
                    value = GetBcd(data, info, knownType);
                    break;
                case DataType.Int8:
                    value = GetInt8(data, info);
                    break;
                case DataType.Int16:
                    value = GetInt16(data, info);
                    break;
                case DataType.UInt8:
                    value = GetUInt8(data, info);
                    break;
                case DataType.UInt16:
                    value = GetUInt16(data, info);
                    break;
                case DataType.CompactArray:
                    throw new Exception("Invalid data type.");
                case DataType.Int64:
                    value = GetInt64(data, info);
                    break;
                case DataType.UInt64:
                    value = GetUInt64(data, info);
                    break;
                case DataType.Enum:
                    value = GetEnum(data, info);
                    break;
                case DataType.Float32:
                    value = Getfloat(data, info);
                    break;
                case DataType.Float64:
                    value = GetDouble(data, info);
                    break;
                case DataType.DateTime:
                    value = GetDateTime(settings, data, info);
                    break;
                case DataType.Date:
                    value = GetDate(data, info);
                    break;
                case DataType.Time:
                    value = GetTime(data, info);
                    break;
                default:
                    throw new Exception("Invalid data type.");
            }
            return value;
        }

        ///<summary>
        ///Get array from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<param name="index">
        ///starting index.
        ///</param>
        ///<returns>Object array.
        ///</returns>
        private static object GetArray(GXByteBuffer buff, GXDataInfo info, int index)
        {
            object value;
            if (info.Count == 0)
            {
                info.Count = GXCommon.GetObjectCount(buff);
            }
            if (info.xml != null)
            {
                info.xml.AppendStartTag(GXDLMS.DATA_TYPE_OFFSET | (int)info.Type, "Qty", info.xml.IntegerToHex(info.Count, 2));
            }

            int size = buff.Size - buff.Position;
            if (info.Count != 0 && size < 1)
            {
                info.Complete = false;
                return null;
            }
            int startIndex = index;
            List<object> arr = new List<object>(info.Count - info.Index);
            // Position where last row was found. Cache uses this info.
            int pos = info.Index;
            for (; pos != info.Count; ++pos)
            {
                GXDataInfo info2 = new GXDataInfo();
                info2.xml = info.xml;
                object tmp = GetData(null, buff, info2);
                if (!info2.Complete)
                {
                    buff.Position = (UInt16)startIndex;
                    info.Complete = false;
                    break;
                }
                else
                {
                    if (info2.Count == info2.Index)
                    {
                        startIndex = buff.Position;
                        arr.Add(tmp);
                    }
                }
            }
            if (info.xml != null)
            {
                info.xml.AppendEndTag(GXDLMS.DATA_TYPE_OFFSET + (int)info.Type);
            }
            info.Index = pos;
            value = arr.ToArray();
            return value;
        }

        ///<summary>
        ///Get time from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed time.
        ///</returns>
        private static object GetTime(GXByteBuffer buff, GXDataInfo info)
        {
            object value;
            if (buff.Size - buff.Position < 4)
            {
                // If there is not enough data available.
                info.Complete = false;
                return null;
            }
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", GXCommon.ToHex(buff.Data, false, buff.Position, 4));
            }

            // Get time.
            int hour = buff.GetUInt8();
            int minute = buff.GetUInt8();
            int second = buff.GetUInt8();
            int ms = buff.GetUInt8();
            if (ms != 0xFF)
            {
                ms *= 10;
            }
            GXTime dt = new GXTime(hour, minute, second, ms);
            value = dt;
            return value;
        }

        ///<summary>
        ///Get date from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed date.
        ///</returns>
        private static object GetDate(GXByteBuffer buff, GXDataInfo info)
        {
            object value;
            if (buff.Size - buff.Position < 5)
            {
                // If there is not enough data available.
                info.Complete = false;
                return null;
            }
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", GXCommon.ToHex(buff.Data, false, buff.Position, 5));
            }
            // Get year.
            int year = buff.GetUInt16();
            // Get month
            int month = buff.GetUInt8();
            // Get day
            int day = buff.GetUInt8();
            GXDate dt = new GXDate(year, month, day);
            // Skip week day
            if (buff.GetUInt8() == 0xFF)
            {
                dt.Skip |= DateTimeSkips.DayOfWeek;
            }
            value = dt;
            return value;
        }

        ///<summary>
        ///Get date and time from DLMS data.
        ///</summary>
        ///<param name="settings">DLMS settings.</param>
        ///<param name="buff">Received DLMS data.</param>
        ///<param name="info">Data info.</param>
        ///<returns>
        ///Parsed date and time.
        ///</returns>
        private static object GetDateTime(GXDLMSSettings settings, GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 12)
            {
                //If time.
                if (buff.Size - buff.Position < 5)
                {
                    return GetTime(buff, info);
                }
                //If date.
                else if (buff.Size - buff.Position < 6)
                {
                    return GetDate(buff, info);
                }

                info.Complete = false;
                return null;
            }
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", GXCommon.ToHex(buff.Data, false, buff.Position, 12));
            }

            GXDateTime dt = new GXDateTime();
            //Get year.
            int year = buff.GetUInt16();
            if (year == 0xFFFF || year == 0)
            {
                year = DateTime.Now.Year;
                dt.Skip |= DateTimeSkips.Year;
            }
            //Get month
            int month = buff.GetUInt8();
            if (month == 0 || month == 0xFF || month == 0xFE || month == 0xFD)
            {
                month = 1;
                dt.Skip |= DateTimeSkips.Month;
            }
            int day = buff.GetUInt8();
            if (day < 1 || day == 0xFF)
            {
                day = 1;
                dt.Skip |= DateTimeSkips.Day;
            }
            else if (day == 0xFD || day == 0xFE)
            {
                day = DateTime.DaysInMonth(year, month) + (sbyte)day + 2;
            }
            //Skip week day
            if (buff.GetUInt8() == 0xFF)
            {
                dt.Skip |= DateTimeSkips.DayOfWeek;
            }
            //Get time.
            int hours = buff.GetUInt8();
            if (hours == 0xFF)
            {
                hours = 0;
                dt.Skip |= DateTimeSkips.Hour;
            }
            int minutes = buff.GetUInt8();
            if (minutes == 0xFF)
            {
                minutes = 0;
                dt.Skip |= DateTimeSkips.Minute;
            }
            int seconds = buff.GetUInt8();
            if (seconds == 0xFF)
            {
                seconds = 0;
                dt.Skip |= DateTimeSkips.Second;
            }
            int milliseconds = buff.GetUInt8();
            if (milliseconds != 0xFF)
            {
                milliseconds *= 10;
            }
            else
            {
                milliseconds = 0;
                dt.Skip |= DateTimeSkips.Ms;
            }
            int deviation = buff.GetInt16();
            dt.Status = (ClockStatus)buff.GetUInt8();
            if (settings != null && settings.UtcTimeZone)
            {
                deviation = -deviation;
            }
            dt.Deviation = deviation;
            //0x8000 == -32768
            //deviation = -1 if skipped.
            if (deviation != -1 && deviation != -32768 && year != 1 && (dt.Skip & DateTimeSkips.Year) == 0)
            {
                dt.Value = new DateTimeOffset(new DateTime(year, month, day, hours, minutes, seconds, milliseconds),
                                              new TimeSpan(0, -deviation, 0));
            }
            else //Use current time if deviation is not defined.
            {
                dt.Skip |= DateTimeSkips.Devitation;
                DateTime tmp = new DateTime(year, month, day, hours, minutes, seconds, milliseconds, DateTimeKind.Local);
                dt.Value = new DateTimeOffset(tmp, TimeZoneInfo.Local.GetUtcOffset(tmp));
            }
            return dt;
        }

        ///<summary>
        ///Get double value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed double value.
        ///</returns>
        private static object GetDouble(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 8)
            {
                info.Complete = false;
                return null;
            }
            double value = buff.GetDouble();
            if (info.xml != null)
            {
                GXByteBuffer tmp = new GXByteBuffer();
                SetData(null, tmp, DataType.Float64, value);
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", GXCommon.ToHex(tmp.Data, false, 1, tmp.Size - 1));
            }
            return value;
        }

        ///<summary>
        ///Get float value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed float value.
        ///</returns>
        private static object Getfloat(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 4)
            {
                info.Complete = false;
                return null;
            }
            float value = buff.GetFloat();
            if (info.xml != null)
            {
                GXByteBuffer tmp = new GXByteBuffer();
                SetData(null, tmp, DataType.Float32, value);
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", GXCommon.ToHex(tmp.Data, false, 1, tmp.Size - 1));
            }
            return value;
        }

        ///<summary>
        ///Get enumeration value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed enumeration value.
        ///</returns>
        private static object GetEnum(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 1)
            {
                info.Complete = false;
                return null;
            }
            byte value = buff.GetUInt8();
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", info.xml.IntegerToHex(value, 2));
            }
            return value;
        }

        ///<summary>
        ///Get UInt64 value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed UInt64 value.
        ///</returns>
        private static object GetUInt64(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 8)
            {
                info.Complete = false;
                return null;
            }
            UInt64 value = buff.GetUInt64();
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", info.xml.IntegerToHex(value));
            }
            return value;
        }

        ///<summary>
        ///Get Int64 value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed Int64 value.
        ///</returns>
        private static object GetInt64(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 8)
            {
                info.Complete = false;
                return null;
            }
            Int64 value = buff.GetInt64();
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", info.xml.IntegerToHex(value, 16));
            }
            return value;
        }

        ///<summary>
        ///Get UInt16 value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed UInt16 value.
        ///</returns>
        private static object GetUInt16(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 2)
            {
                info.Complete = false;
                return null;
            }
            UInt16 value = buff.GetUInt16();
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", info.xml.IntegerToHex(value, 4));
            }
            return value;
        }

        ///<summary>
        ///Get UInt8 value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed UInt8 value.
        ///</returns>
        private static object GetUInt8(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 1)
            {
                info.Complete = false;
                return null;
            }
            byte value = buff.GetUInt8();
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", info.xml.IntegerToHex(value, 2));
            }
            return value;
        }

        ///<summary>
        ///Get Int16 value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed Int16 value.
        ///</returns>
        private static object GetInt16(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 2)
            {
                info.Complete = false;
                return null;
            }
            Int16 value = buff.GetInt16();
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", info.xml.IntegerToHex(value, 4));
            }
            return value;
        }

        ///<summary>
        ///Get Int8 value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed Int8 value.
        ///</returns>
        private static object GetInt8(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 1)
            {
                info.Complete = false;
                return null;
            }
            sbyte value = buff.GetInt8();
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", value);
            }
            return value;
        }

        ///<summary>
        ///Get BCD value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed BCD value.
        ///</returns>
        private static object GetBcd(GXByteBuffer buff, GXDataInfo info, bool knownType)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 1)
            {
                info.Complete = false;
                return null;
            }
            byte value = buff.GetUInt8();
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", value);
            }
            return value;
        }

        ///<summary>
        ///Get UTF string value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed UTF string value.
        ///</returns>
        private static object GetUtfString(GXByteBuffer buff, GXDataInfo info, bool knownType)
        {
            object value;
            int len;
            if (knownType)
            {
                len = buff.Size;
            }
            else
            {
                len = GXCommon.GetObjectCount(buff);
                // If there is not enough data available.
                if (buff.Size - buff.Position < len)
                {
                    info.Complete = false;
                    return null;
                }
            }
            if (len > 0)
            {
                value = buff.GetStringUtf8(buff.Position, len);
            }
            else
            {
                value = "";
            }
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", value);
            }
            return value;
        }

        ///<summary>
        ///Get octect string value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed octet string value.
        ///</returns>
        private static object GetOctetString(GXByteBuffer buff, GXDataInfo info, bool knownType)
        {
            object value;
            int len;
            if (knownType)
            {
                len = buff.Size;
            }
            else
            {
                len = GXCommon.GetObjectCount(buff);
                // If there is not enough data available.
                if (buff.Size - buff.Position < len)
                {
                    info.Complete = false;
                    return null;
                }
            }
            byte[] tmp = new byte[len];
            buff.Get(tmp);
            value = tmp;
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", GXCommon.ToHex(tmp, false));
            }
            return value;
        }

        ///<summary>
        ///Get string value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed string value.
        ///</returns>
        private static object GetString(GXByteBuffer buff, GXDataInfo info, bool knownType)
        {
            string value;
            int len;
            if (knownType)
            {
                len = buff.Size;
            }
            else
            {
                len = GXCommon.GetObjectCount(buff);
                // If there is not enough data available.
                if (buff.Size - buff.Position < len)
                {
                    info.Complete = false;
                    return null;
                }
            }
            if (len > 0)
            {
                value = buff.GetString(len);
            }
            else
            {
                value = "";
            }
            if (info.xml != null)
            {
                if (info.xml.ShowStringAsHex)
                {
                    info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", GXCommon.ToHex(buff.Data, false, buff.Position - len, len));
                }
                else
                {
                    info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", value.Replace('\"', '\''));
                }
            }
            return value;
        }

        ///<summary>
        ///Get UInt32 value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed UInt32 value.
        /// </returns>
        private static object GetUInt32(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 4)
            {
                info.Complete = false;
                return null;
            }
            UInt32 value = buff.GetUInt32();
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", value);
            }
            return value;
        }

        ///<summary>
        ///Get Int32 value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed Int32 value.
        ///</returns>
        private static object GetInt32(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 4)
            {
                info.Complete = false;
                return null;
            }
            Int32 value = buff.GetInt32();
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", value);
            }
            return value;
        }

        ///<summary>
        ///Get bit string value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed bit string value.
        ///</returns>
        private static string GetBitString(GXByteBuffer buff, GXDataInfo info)
        {
            int cnt = GetObjectCount(buff);
            double t = cnt;
            t /= 8;
            if (cnt % 8 != 0)
            {
                ++t;
            }
            int byteCnt = (int)Math.Floor(t);
            // If there is not enough data available.
            if (buff.Size - buff.Position < byteCnt)
            {
                info.Complete = false;
                return null;
            }
            StringBuilder sb = new StringBuilder();
            while (cnt > 0)
            {
                ToBitString(sb, buff.GetUInt8(), cnt);
                cnt -= 8;
            }
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", sb.ToString());
            }
            return sb.ToString();
        }

        ///<summary>
        ///Get boolean value from DLMS data.
        ///</summary>
        ///<param name="buff">
        ///Received DLMS data.
        ///</param>
        ///<param name="info">
        ///Data info.
        ///</param>
        ///<returns>
        ///Parsed boolean value.
        ///</returns>
        private static object GetBoolean(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 1)
            {
                info.Complete = false;
                return null;
            }
            byte value = buff.GetUInt8();
            if (info.xml != null)
            {
                info.xml.AppendLine(GXDLMS.DATA_TYPE_OFFSET + (int)info.Type, "Value", value);
            }
            return value != 0;
        }

        public static DataType GetValueType(object value)
        {
            if (value == null)
            {
                return DataType.None;
            }
            if (value is byte[])
            {
                return DataType.OctetString;
            }
            if (value.GetType().IsEnum)
            {
                return DataType.Enum;
            }
            if (value is byte)
            {
                return DataType.UInt8;
            }
            if (value is sbyte)
            {
                return DataType.Int8;
            }
            if (value is UInt16)
            {
                return DataType.UInt16;
            }
            if (value is Int16)
            {
                return DataType.Int16;
            }
            if (value is UInt32)
            {
                return DataType.UInt32;
            }
            if (value is Int32)
            {
                return DataType.Int32;
            }
            if (value is UInt64)
            {
                return DataType.UInt64;
            }
            if (value is Int64)
            {
                return DataType.Int64;
            }
            if (value is GXDate)
            {
                return DataType.Date;
            }
            if (value is GXTime)
            {
                return DataType.Time;
            }
            if (value is DateTime || value is GXDateTime)
            {
                return DataType.DateTime;
            }
            if (value.GetType().IsArray)
            {
                return DataType.Array;
            }
            if (value is string)
            {
                return DataType.String;
            }
            if (value is bool)
            {
                return DataType.Boolean;
            }
            if (value is float)
            {
                return DataType.Float32;
            }
            if (value is double)
            {
                return DataType.Float64;
            }
            throw new Exception("Invalid value.");
        }

        ///<summary>
        ///Convert object to DLMS bytes.
        ///</summary>
        ///<param name="settings">DLMS settings.</param>
        ///<param name="buff">Byte buffer where data is write.</param>
        ///<param name="dataType">Data type.</param>
        ///<param name="value">Added Value.</param>
        public static void SetData(GXDLMSSettings settings, GXByteBuffer buff, DataType type, object value)
        {
            if ((type == DataType.Array || type == DataType.Structure) && value is byte[])
            {
                // If byte array is added do not add type.
                buff.Set((byte[])value);
                return;
            }
            buff.SetUInt8((byte)type);
            switch (type)
            {
                case DataType.None:
                    break;
                case DataType.Boolean:
                    if (Convert.ToBoolean(value))
                    {
                        buff.SetUInt8(1);
                    }
                    else
                    {
                        buff.SetUInt8(0);
                    }
                    break;
                case DataType.Int8:
                    buff.SetUInt8((byte)Convert.ToSByte(value));
                    break;
                case DataType.UInt8:
                case DataType.Enum:
                    buff.SetUInt8(Convert.ToByte(value));
                    break;
                case DataType.Int16:
                    if (value is UInt16)
                    {
                        buff.SetUInt16((UInt16)value);
                    }
                    else
                    {
                        buff.SetUInt16((UInt16)(Convert.ToInt16(value) & 0xFFFF));
                    }
                    break;
                case DataType.UInt16:
                    buff.SetUInt16(Convert.ToUInt16(value));
                    break;
                case DataType.Int32:
                    buff.SetUInt32((UInt32)Convert.ToInt32(value));
                    break;
                case DataType.UInt32:

                    buff.SetUInt32(Convert.ToUInt32(value));
                    break;
                case DataType.Int64:
                    buff.SetUInt64((UInt64)Convert.ToInt64(value));
                    break;
                case DataType.UInt64:
                    buff.SetUInt64(Convert.ToUInt64(value));
                    break;
                case DataType.Float32:
                    buff.SetFloat((float)value);
                    break;
                case DataType.Float64:
                    buff.SetDouble((double)value);
                    break;
                case DataType.BitString:
                    SetBitString(buff, value);
                    break;
                case DataType.String:
                    SetString(buff, value);
                    break;
                case DataType.StringUTF8:
                    SetUtcString(buff, value);
                    break;
                case DataType.OctetString:
                    if (value is GXDate)
                    {
                        //Add size
                        buff.SetUInt8(5);
                        SetDate(buff, value);
                    }
                    else if (value is GXTime)
                    {
                        //Add size
                        buff.SetUInt8(4);
                        SetTime(buff, value);
                    }
                    else if (value is GXDateTime || value is DateTime)
                    {
                        //Add size
                        buff.SetUInt8(12);
                        SetDateTime(settings, buff, value);
                    }
                    else
                    {
                        SetOctetString(buff, value);
                    }
                    break;
                case DataType.Array:
                case DataType.Structure:
                    SetArray(settings, buff, value);
                    break;
                case DataType.Bcd:
                    SetBcd(buff, value);
                    break;
                case DataType.CompactArray:
                    throw new Exception("Invalid data type.");
                case DataType.DateTime:
                    SetDateTime(settings, buff, value);
                    break;
                case DataType.Date:
                    SetDate(buff, value);
                    break;
                case DataType.Time:
                    SetTime(buff, value);
                    break;
                default:
                    throw new Exception("Invalid data type.");
            }
        }

        ///<summary>
        ///Convert time to DLMS bytes.
        ///</summary>
        ///<param name="buff">
        ///Byte buffer where data is write.
        ///</param>
        ///<param name="value">
        ///Added value.
        ///</param>
        private static void SetTime(GXByteBuffer buff, object value)
        {
            GXDateTime dt;
            if (value is GXDateTime)
            {
                dt = (GXDateTime)value;
            }
            else if (value is DateTime)
            {
                dt = new GXDateTime((DateTime)value);
            }
            else if (value is DateTimeOffset)
            {
                dt = new GXDateTime((DateTimeOffset)value);
            }
            else if (value is string)
            {
                dt = DateTime.Parse((string)value);
            }
            else
            {
                throw new Exception("Invalid date format.");
            }
            //Add time.
            if ((dt.Skip & DateTimeSkips.Hour) != 0)
            {
                buff.SetUInt8(0xFF);
            }
            else
            {
                buff.SetUInt8((byte)dt.Value.Hour);
            }

            if ((dt.Skip & DateTimeSkips.Minute) != 0)
            {
                buff.SetUInt8(0xFF);
            }
            else
            {
                buff.SetUInt8((byte)dt.Value.Minute);
            }

            if ((dt.Skip & DateTimeSkips.Second) != 0)
            {
                buff.SetUInt8(0xFF);
            }
            else
            {
                buff.SetUInt8((byte)dt.Value.Second);
            }

            //Hundredths of second is not used.
            if ((dt.Skip & DateTimeSkips.Ms) != 0)
            {
                buff.SetUInt8(0xFF);
            }
            else
            {
                buff.SetUInt8((byte)(dt.Value.Millisecond / 10));
            }
        }

        ///<summary>
        ///Convert date to DLMS bytes.
        ///</summary>
        ///<param name="buff">
        ///Byte buffer where data is write.
        ///</param>
        ///<param name="value">
        ///Added value.
        ///</param>
        private static void SetDate(GXByteBuffer buff, object value)
        {
            GXDateTime dt;
            if (value is GXDateTime)
            {
                dt = (GXDateTime)value;
            }
            else if (value is DateTime)
            {
                dt = new GXDateTime((DateTime)value);
            }
            else if (value is DateTimeOffset)
            {
                dt = new GXDateTime((DateTimeOffset)value);
            }
            else if (value is string)
            {
                dt = DateTime.Parse((string)value);
            }
            else
            {
                throw new Exception("Invalid date format.");
            }
            // Add year.
            if ((dt.Skip & DateTimeSkips.Year) != 0)
            {
                buff.SetUInt16(0xFFFF);
            }
            else
            {
                buff.SetUInt16((UInt16)dt.Value.Year);
            }
            // Add month
            if (dt.DaylightSavingsBegin)
            {
                buff.SetUInt8(0xFE);
            }
            else if (dt.DaylightSavingsEnd)
            {
                buff.SetUInt8(0xFD);
            }
            else if ((dt.Skip & DateTimeSkips.Month) != 0)
            {
                buff.SetUInt8(0xFF);
            }
            else
            {
                buff.SetUInt8((byte)dt.Value.Month);
            }
            // Add day
            if ((dt.Skip & DateTimeSkips.Day) != 0)
            {
                buff.SetUInt8(0xFF);
            }
            else
            {
                buff.SetUInt8((byte)dt.Value.Day);
            }
            // Add week day
            if ((dt.Skip & DateTimeSkips.DayOfWeek) != 0)
            {
                buff.SetUInt8(0xFF);
            }
            else
            {
                if (dt.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    buff.SetUInt8(7);
                }
                else
                {
                    buff.SetUInt8((byte)(dt.Value.DayOfWeek));
                }
            }
        }

        ///<summary>
        ///Convert date time to DLMS bytes.
        ///</summary>
        ///<param name="buff">
        ///Byte buffer where data is write.
        ///</param>
        ///<param name="value">
        ///Added value.
        ///</param>
        private static void SetDateTime(GXDLMSSettings settings, GXByteBuffer buff, object value)
        {
            GXDateTime dt;
            if (value is GXDateTime)
            {
                dt = (GXDateTime)value;
            }
            else if (value is DateTime)
            {
                dt = new GXDateTime((DateTime)value);
                dt.Skip = dt.Skip | DateTimeSkips.Ms;
            }
            else if (value is string)
            {
                dt = new GXDateTime(DateTime.Parse((string)value));
                dt.Skip = dt.Skip | DateTimeSkips.Ms;
            }
            else
            {
                throw new Exception("Invalid date format.");
            }
            if (dt.Value.UtcDateTime == DateTime.MinValue)
            {
                dt.Value = DateTime.SpecifyKind(new DateTime(2000, 1, 1).Date, DateTimeKind.Utc);
            }
            else if (dt.Value.UtcDateTime == DateTime.MaxValue)
            {
                dt.Value = DateTime.SpecifyKind(DateTime.Now.AddYears(1).Date, DateTimeKind.Utc);
            }
            DateTimeOffset tm = dt.Value;
            if ((dt.Skip & DateTimeSkips.Year) == 0)
            {
                buff.SetUInt16((ushort)tm.Year);
            }
            else
            {
                buff.SetUInt16(0xFFFF);
            }
            if ((dt.Skip & DateTimeSkips.Month) == 0)
            {
                if (dt.DaylightSavingsBegin)
                {
                    buff.SetUInt8(0xFE);
                }
                else if (dt.DaylightSavingsEnd)
                {
                    buff.SetUInt8(0xFD);
                }
                else
                {
                    buff.SetUInt8((byte)tm.Month);
                }
            }
            else
            {
                buff.SetUInt8(0xFF);
            }
            if ((dt.Skip & DateTimeSkips.Day) == 0)
            {
                buff.SetUInt8((byte)tm.Day);
            }
            else
            {
                buff.SetUInt8(0xFF);
            }
            // Add week day
            if ((dt.Skip & DateTimeSkips.DayOfWeek) != 0)
            {
                buff.SetUInt8(0xFF);
            }
            else
            {
                if (dt.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    buff.SetUInt8(7);
                }
                else
                {
                    buff.SetUInt8((byte)(dt.Value.DayOfWeek));
                }
            }
            //Add time.
            if ((dt.Skip & DateTimeSkips.Hour) == 0)
            {
                buff.SetUInt8((byte)tm.Hour);
            }
            else
            {
                buff.SetUInt8(0xFF);
            }

            if ((dt.Skip & DateTimeSkips.Minute) == 0)
            {
                buff.SetUInt8((byte)tm.Minute);
            }
            else
            {
                buff.SetUInt8(0xFF);
            }
            if ((dt.Skip & DateTimeSkips.Second) == 0)
            {
                buff.SetUInt8((byte)tm.Second);
            }
            else
            {
                buff.SetUInt8(0xFF);
            }

            if ((dt.Skip & DateTimeSkips.Ms) == 0)
            {
                buff.SetUInt8((byte)(tm.Millisecond / 10));
            }
            else
            {
                buff.SetUInt8((byte)0xFF); //Hundredths of second is not used.
            }
            //Add deviation.
            if ((dt.Skip & DateTimeSkips.Devitation) == 0)
            {
                if (settings != null && settings.UtcTimeZone)
                {
                    buff.SetInt16((Int16)(dt.Value.Offset.TotalMinutes));
                }
                else
                {
                    buff.SetInt16((Int16)(-dt.Value.Offset.TotalMinutes));
                }
            }
            else //deviation not used.
            {
                buff.SetUInt16(0x8000);
            }
            //Add clock_status
            if (dt.Value.LocalDateTime.IsDaylightSavingTime())
            {
                buff.SetUInt8((byte)(dt.Status | ClockStatus.DaylightSavingActive));
            }
            else
            {
                buff.SetUInt8((byte)dt.Status);
            }
        }

        ///<summary>
        ///Convert BCD to DLMS bytes.
        ///</summary>
        ///<param name="buff">
        ///Byte buffer where data is write.
        ///</param>
        ///<param name="value">
        ///Added value.
        ///</param>
        private static void SetBcd(GXByteBuffer buff, object value)
        {
            //Standard supports only size of byte.
            buff.SetUInt8(Convert.ToByte(value));
        }

        ///<summary>
        ///Convert Array to DLMS bytes.
        ///</summary>
        ///<param name="buff">
        ///Byte buffer where data is write.
        ///</param>
        ///<param name="value">
        ///Added value.
        ///</param>
        private static void SetArray(GXDLMSSettings settings, GXByteBuffer buff, object value)
        {
            if (value != null)
            {
                object[] arr = (object[])value;
                SetObjectCount(arr.Length, buff);
                foreach (object it in arr)
                {
                    SetData(settings, buff, GetValueType(it), it);
                }
            }
            else
            {
                SetObjectCount(0, buff);
            }
        }

        ///<summary>
        ///Convert Octet string to DLMS bytes.
        ///</summary>
        ///<param name="buff">
        ///Byte buffer where data is write.
        ///</param>
        ///<param name="value">
        ///Added value.
        ///</param>
        private static void SetOctetString(GXByteBuffer buff, object value)
        {
            // Example Logical name is octet string, so do not change to
            // string...
            if (value is string)
            {
                string[] items = ((string)value).Split('.');
                // If data is string.
                if (items.Length == 1)
                {
                    byte[] tmp = ASCIIEncoding.ASCII.GetBytes((string)value);
                    SetObjectCount(tmp.Length, buff);
                    buff.Set(tmp);
                }
                else
                {
                    SetObjectCount(items.Length, buff);
                    foreach (string it in items)
                    {
                        buff.SetUInt8(Convert.ToByte(it));
                    }
                }
            }
            else if (value is sbyte[])
            {
                SetObjectCount(((byte[])value).Length, buff);
                buff.Set((byte[])value);
            }
            else if (value == null)
            {
                SetObjectCount(0, buff);
            }
            else
            {
                throw new Exception("Invalid data type.");
            }
        }

        ///<summary>
        ///Convert UTC string to DLMS bytes.
        ///</summary>
        ///<param name="buff">
        ///Byte buffer where data is write.
        ///</param>
        ///<param name="value">
        ///Added value.
        ///</param>
        private static void SetUtcString(GXByteBuffer buff, object value)
        {
            if (value != null)
            {
                byte[] tmp = ASCIIEncoding.UTF8.GetBytes(Convert.ToString(value));
                SetObjectCount(tmp.Length, buff);
                buff.Set(tmp);
            }
            else
            {
                buff.SetUInt8(0);
            }
        }

        ///<summary>
        ///Convert ASCII string to DLMS bytes.
        ///</summary>
        ///<param name="buff">
        ///Byte buffer where data is write.
        ///</param>
        ///<param name="value">
        ///Added value.
        ///</param>
        private static void SetString(GXByteBuffer buff, object value)
        {
            if (value != null)
            {
                string str = Convert.ToString(value);
                SetObjectCount(str.Length, buff);
                buff.Set(ASCIIEncoding.ASCII.GetBytes(str));
            }
            else
            {
                buff.SetUInt8(0);
            }
        }

        ///<summary>
        ///Convert Bit string to DLMS bytes.
        ///</summary>
        ///<param name="buff">
        ///Byte buffer where data is write.
        ///</param>
        ///<param name="value">
        ///Added value.
        ///</param>
        private static void SetBitString(GXByteBuffer buff, object value)
        {
            if (value is string)
            {
                GXByteBuffer tmp = new GXByteBuffer();
                byte val = 0;
                int index = 0;
                string str = ((string)value);
                SetObjectCount(str.Length, buff);
                foreach (char it in str.Reverse())
                {
                    if (it == '1')
                    {
                        val |= (byte)(1 << index);
                        ++index;
                    }
                    else if (it == '0')
                    {
                        ++index;
                    }
                    else
                    {
                        throw new Exception("Not a bit string.");
                    }
                    if (index == 8)
                    {
                        index = 0;
                        tmp.SetUInt8(val);
                        val = 0;
                    }
                }
                if (index != 0)
                {
                    tmp.SetUInt8(val);
                }
                for (int pos = tmp.Size - 1; pos != -1; --pos)
                {
                    buff.SetUInt8(tmp.GetUInt8(pos));
                }
            }
            else if (value is sbyte[])
            {
                byte[] arr = (byte[])value;
                SetObjectCount(arr.Length, buff);
                buff.Set(arr);
            }
            else if (value == null)
            {
                buff.SetUInt8(0);
            }
            else
            {
                throw new Exception("BitString must give as string.");
            }
        }
    }
}