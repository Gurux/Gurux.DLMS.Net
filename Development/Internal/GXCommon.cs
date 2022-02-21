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
using System.Text;
using Gurux.DLMS.Enums;
using System.Diagnostics;
using System.Net;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Ecdsa;

namespace Gurux.DLMS.Internal
{
    /// <summary>
    /// Reserved for internal use.
    /// </summary>
    internal class GXCommon
    {
        internal const byte HDLCFrameStartEnd = 0x7E;

        /// <summary>
        /// Sent LLC bytes.
        /// </summary>
        internal static readonly byte[] LLCSendBytes = { 0xE6, 0xE6, 0x00 };
        /// <summary>
        /// Received LLC bytes.
        /// </summary>
        internal static readonly byte[] LLCReplyBytes = { 0xE6, 0xE7, 0x00 };

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
        /// Is string hex string.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns>Returns true, if string is hex string.</returns>
        public static bool IsHexString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            foreach (char ch in value)
            {
                if (ch != ' ' && !((ch > 0x40 && ch < 'G')
                        || (ch > 0x60 && ch < 'g') || (ch > '/' && ch < ':')))
                {
                    return false;
                }
            }
            return true;
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
                    buffer[index] = (byte)lastValue;
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
        /// Get HDLC address from byte array.
        /// </summary>
        /// <param name="buff">Byte array.</param>
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
        /// Insert item count to the begin of the buffer.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="buff"></param>
        internal static void InsertObjectCount(int count, GXByteBuffer buff, int index)
        {
            if (count < 0x80)
            {
                buff.Move(index, index + 1, buff.Size);
                buff.Size -= index;
                buff.SetUInt8(index, (byte)count);
            }
            else if (count < 0x100)
            {
                buff.Move(index, index + 2, buff.Size);
                buff.Size -= index;
                buff.SetUInt8(index, 0x81);
                buff.SetUInt8(index + 1, (byte)count);
            }
            else if (count < 0x10000)
            {
                buff.Move(index, index + 4, buff.Size);
                buff.Size -= index;
                buff.SetUInt8(index, 0x82);
                buff.SetUInt16(index + 1, (UInt16)count);
            }
            else
            {
                buff.Move(index, index + 5, buff.Size);
                buff.Size -= index;
                buff.SetUInt8(index, 0x84);
                buff.SetUInt32(index + 1, (UInt32)count);
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
                else if (cnt == 0x83)
                {
                    cnt = data.GetUInt24(data.Position);
                    data.Position += 3;
                    return cnt;
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
        internal static void ToBitString(StringBuilder sb, byte value, int count)
        {
            if (count > 0)
            {
                if (count > 8)
                {
                    count = 8;
                }
                for (int pos = 7; pos != 8 - count - 1; --pos)
                {
                    if ((value & (1 << pos)) != 0)
                    {
                        sb.Append('1');
                    }
                    else
                    {
                        sb.Append('0');
                    }
                }
            }
        }


        ///<summary>
        ///Get data from DLMS frame.
        ///</summary>
        ///<param name="settings">DLMS settings.</param>
        ///<param name="data">Received data.</param>
        ///<param name="info"> Data info.</param>
        ///<returns>Parsed object.</returns>
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
                    value = GetArray(settings, data, info, startIndex);
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
                    value = GetOctetString(settings, data, info, knownType);
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
                    value = GetCompactArray(settings, data, info, false);
                    break;
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
                case DataType.DeltaInt8:
                    value = new GXDeltaInt8(GetInt8(data, info));
                    break;
                case DataType.DeltaInt16:
                    value = new GXDeltaInt16(GetInt16(data, info));
                    break;
                case DataType.DeltaInt32:
                    value = new GXDeltaInt32(GetInt32(data, info));
                    break;
                case DataType.DeltaUInt8:
                    value = new GXDeltaUInt8(GetUInt8(data, info));
                    break;
                case DataType.DeltaUInt16:
                    value = new GXDeltaUInt16(GetUInt16(data, info));
                    break;
                case DataType.DeltaUInt32:
                    value = new GXDeltaUInt32(GetUInt32(data, info));
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
        private static object GetArray(GXDLMSSettings settings, GXByteBuffer buff, GXDataInfo info, int index)
        {
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
            List<object> arr;
            if (info.Type == DataType.Array)
            {
                arr = new GXArray();
            }
            else
            {
                arr = new GXStructure();
            }
            // Position where last row was found. Cache uses this info.
            int pos = info.Index;
            for (; pos != info.Count; ++pos)
            {
                GXDataInfo info2 = new GXDataInfo();
                info2.xml = info.xml;
                object tmp = GetData(settings, buff, info2);
                if (!info2.Complete)
                {
                    if (info.xml != null)
                    {
                        info.xml.AppendComment(string.Format("Error: Not enough data. {0} rows are missing.", info.Count - pos));
                    }
                    buff.Position = startIndex;
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
            if (settings.Version == 8)
            {
                return arr.ToArray();
            }
            return arr;
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
            GXTime value = null;
            if (buff.Size - buff.Position < 4)
            {
                // If there is not enough data available.
                info.Complete = false;
                return null;
            }
            string str = null;
            if (info.xml != null)
            {
                str = GXCommon.ToHex(buff.Data, false, buff.Position, 4);
            }
            try
            {
                // Get time.
                int hour = buff.GetUInt8();
                int minute = buff.GetUInt8();
                int second = buff.GetUInt8();
                int ms = buff.GetUInt8();
                if (ms != 0xFF)
                {
                    ms *= 10;
                }
                else
                {
                    ms = -1;
                }
                value = new GXTime(hour, minute, second, ms);
            }
            catch (Exception ex)
            {
                if (info.xml == null)
                {
                    throw ex;
                }
            }
            if (info.xml != null)
            {
                if (value != null)
                {
                    info.xml.AppendComment(value.ToFormatString());
                }
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", str);
            }
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
            GXDate value = null;
            if (buff.Size - buff.Position < 5)
            {
                // If there is not enough data available.
                info.Complete = false;
                return null;
            }
            string str = null;
            if (info.xml != null)
            {
                str = GXCommon.ToHex(buff.Data, false, buff.Position, 5);
            }
            try
            {
                // Get year.
                int year = buff.GetUInt16();
                // Get month
                int month = buff.GetUInt8();
                DateTimeExtraInfo extra = DateTimeExtraInfo.None;
                DateTimeSkips skip = DateTimeSkips.None;
                if (month == 0 || month == 0xFF)
                {
                    month = 1;
                    skip |= DateTimeSkips.Month;
                }
                else if (month == 0xFE)
                {
                    //Daylight savings begin.
                    month = 1;
                    extra |= DateTimeExtraInfo.DstBegin;
                }
                else if (month == 0xFD)
                {
                    // Daylight savings end.
                    month = 1;
                    extra |= DateTimeExtraInfo.DstEnd;
                }
                // Get day
                int day = buff.GetUInt8();
                if (day == 0xFD)
                {
                    // 2nd last day of month.
                    day = 1;
                    extra |= DateTimeExtraInfo.LastDay2;
                }
                else if (day == 0xFE)
                {
                    //Last day of month
                    day = 1;
                    extra |= DateTimeExtraInfo.LastDay;
                }
                else if (day < 1 || day == 0xFF)
                {
                    day = 1;
                    skip |= DateTimeSkips.Day;
                }
                value = new GXDate(year, month, day);
                value.Extra = extra;
                value.Skip |= skip;
                // Skip week day
                if (buff.GetUInt8() == 0xFF)
                {
                    value.Skip |= DateTimeSkips.DayOfWeek;
                }
            }
            catch (Exception ex)
            {
                if (info.xml == null)
                {
                    throw ex;
                }
            }
            if (info.xml != null)
            {
                if (value != null)
                {
                    info.xml.AppendComment(value.ToFormatString());
                }
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", str);
            }
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
            string str = null;
            if (info.xml != null)
            {
                str = GXCommon.ToHex(buff.Data, false, buff.Position, 12);
            }

            GXDateTime dt = new GXDateTime();
            try
            {
                //Get year.
                int year = buff.GetUInt16();
                if (year == 0xFFFF || year == 0)
                {
                    year = DateTime.Now.Year;
                    dt.Skip |= DateTimeSkips.Year;
                }
                //Get month
                int month = buff.GetUInt8();
                if (month == 0 || month == 0xFF)
                {
                    month = 1;
                    dt.Skip |= DateTimeSkips.Month;
                }
                else if (month == 0xFE)
                {
                    //Daylight savings begin.
                    month = 1;
                    dt.Extra |= DateTimeExtraInfo.DstBegin;
                }
                else if (month == 0xFD)
                {
                    // Daylight savings end.
                    month = 1;
                    dt.Extra |= DateTimeExtraInfo.DstEnd;
                }
                int day = buff.GetUInt8();
                if (day == 0xFD)
                {
                    // 2nd last day of month.
                    day = 1;
                    dt.Extra |= DateTimeExtraInfo.LastDay2;
                }
                else if (day == 0xFE)
                {
                    //Last day of month
                    day = 1;
                    dt.Extra |= DateTimeExtraInfo.LastDay;
                }
                else if (day < 1 || day == 0xFF)
                {
                    day = 1;
                    dt.Skip |= DateTimeSkips.Day;
                }

                //Skip week day.
                byte wd = buff.GetUInt8();
                if (wd == 0xFF)
                {
                    dt.Skip |= DateTimeSkips.DayOfWeek;
                }
                else
                {
                    dt.DayOfWeek = wd;
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
                if (settings != null && settings.UseUtc2NormalTime && deviation != -32768)
                {
                    deviation = -deviation;
                }
                //0x8000 == -32768
                //deviation = -1 if skipped.
                if (deviation != -1 && deviation != -32768 && year != 1 && (dt.Skip & DateTimeSkips.Year) == 0)
                {
                    dt.Value = new DateTimeOffset(new DateTime(year, month, day, hours, minutes, seconds, milliseconds),
                                                  new TimeSpan(0, -deviation, 0));
                }
                else //Use current time if deviation is not defined.
                {
                    dt.Skip |= DateTimeSkips.Deviation;
                    DateTime tmp = new DateTime(year, month, day, hours, minutes, seconds, milliseconds, DateTimeKind.Local);
                    dt.Value = new DateTimeOffset(tmp);
                }
            }
            catch (Exception)
            {
                if (info.xml == null)
                {
                    throw;
                }
                dt = null;
            }
            if (info.xml != null)
            {
                if (dt != null)
                {
                    info.xml.AppendComment(dt.ToFormatMeterString());
                }
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", str);
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
                if (info.xml.Comments)
                {
                    info.xml.AppendComment(value.ToString());
                }
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
                if (info.xml.Comments)
                {
                    info.xml.AppendComment(value.ToString());
                }
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
            return new GXEnum(value);
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
        private static UInt16 GetUInt16(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 2)
            {
                info.Complete = false;
                return 0;
            }
            UInt16 value = buff.GetUInt16();
            if (info.xml != null)
            {
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", info.xml.IntegerToHex(value, 4));
            }
            return value;
        }

        private static void GetCompactArrayItem(GXDLMSSettings settings, GXByteBuffer buff, List<object> dt, List<Object> list, int len, bool array)
        {
            List<object> tmp;
            if (array)
            {
                tmp = new GXArray();
            }
            else
            {
                tmp = new GXStructure();
            }
            foreach (object it in dt)
            {
                if (it is DataType)
                {
                    GetCompactArrayItem(settings, buff, (DataType)it, tmp, 1);
                }
                else
                {
                    GetCompactArrayItem(settings, buff, (List<object>)it, tmp, 1, it is GXArray);
                }
            }
            list.Add(tmp);
        }

        private static void GetCompactArrayItem(GXDLMSSettings settings, GXByteBuffer buff, DataType dt, List<Object> list, int len)
        {
            GXDataInfo tmp = new GXDataInfo();
            tmp.Type = dt;
            int start = buff.Position;
            if (dt == DataType.String)
            {
                while (buff.Position - start < len)
                {
                    tmp.Clear();
                    tmp.Type = dt;
                    list.Add(GetString(buff, tmp, false));
                    if (!tmp.Complete)
                    {
                        break;
                    }
                }
            }
            else if (dt == DataType.OctetString)
            {
                while (buff.Position - start < len)
                {
                    tmp.Clear();
                    tmp.Type = dt;
                    list.Add(GetOctetString(settings, buff, tmp, false));
                    if (!tmp.Complete)
                    {
                        break;
                    }
                }
            }
            else
            {
                while (buff.Position - start < len)
                {
                    tmp.Clear();
                    tmp.Type = dt;
                    list.Add(GetData(null, buff, tmp));
                    if (!tmp.Complete)
                    {
                        break;
                    }
                }
            }
        }

        private static void GetDataTypes(GXByteBuffer buff, List<object> cols, int len)
        {
            DataType dt;
            for (int pos = 0; pos != len; ++pos)
            {
                dt = (DataType)buff.GetUInt8();
                if (dt == DataType.Array)
                {
                    int cnt = buff.GetUInt16();
                    List<object> tmp = new List<object>();
                    GXArray tmp2 = new GXArray();
                    GetDataTypes(buff, tmp, 1);
                    for (int i = 0; i != cnt; ++i)
                    {
                        tmp2.Add(tmp[0]);
                    }
                    cols.Add(tmp2);
                }
                else if (dt == DataType.Structure)
                {
                    GXStructure tmp = new GXStructure();
                    GetDataTypes(buff, tmp, buff.GetUInt8());
                    cols.Add(tmp);
                }
                else
                {
                    cols.Add(dt);
                }
            }
        }


        private static void AppendDataTypeAsXml(List<object> cols, GXDataInfo info)
        {
            foreach (object it in cols)
            {
                if (it is DataType)
                {
                    info.xml.AppendEmptyTag(info.xml.GetDataType((DataType)it));
                }
                else if (it is GXStructure)
                {
                    info.xml.AppendStartTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Structure, null, null);
                    AppendDataTypeAsXml((List<object>)it, info);
                    info.xml.AppendEndTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Structure);
                }
                else
                {
                    info.xml.AppendStartTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Array, null, null);
                    AppendDataTypeAsXml(((List<Object>)it), info);
                    info.xml.AppendEndTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Array);
                }
            }
        }

        private static void ToString(object it, StringBuilder sb)
        {
            if (it is byte[])
            {
                sb.Append(GXCommon.ToHex((byte[])it, true));
            }
            else if (it is IEnumerable<object>)
            {
                bool empty = true;
                //                sb.Append("[");
                foreach (object it2 in (IEnumerable<object>)it)
                {
                    empty = false;
                    ToString(it2, sb);
                }
                if (!empty)
                {
                    --sb.Length;
                }
                //                sb.Append("]");
            }
            else
            {
                sb.Append(Convert.ToString(it));
            }
            sb.Append(";");
        }

        /// <summary>
        /// Get compact array value from DLMS data.
        /// </summary>
        /// <param name="settings">Received DLMS data.</param>
        /// <param name="buff">Data info.</param>
        /// <param name="info"></param>
        /// <returns>Parsed value</returns>
        internal static object GetCompactArray(GXDLMSSettings settings, GXByteBuffer buff, GXDataInfo info, bool onlyDataTypes)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 2)
            {
                info.Complete = false;
                return null;
            }
            DataType dt = (DataType)buff.GetUInt8();
            if (dt == DataType.Array)
            {
                throw new ArgumentException("Invalid compact array data.");
            }
            int len = GXCommon.GetObjectCount(buff);
            List<Object> list;
            if (dt == DataType.Structure)
            {
                list = new GXStructure();
            }
            else
            {
                list = new GXArray();
            }
            if (dt == DataType.Structure)
            {
                // Get data types.
                GXStructure cols = new GXStructure();
                GetDataTypes(buff, cols, len);
                if (onlyDataTypes)
                {
                    return cols;
                }
                if (buff.Position == buff.Size)
                {
                    len = 0;
                }
                else
                {
                    len = GXCommon.GetObjectCount(buff);
                }
                if (info.xml != null)
                {
                    info.xml.AppendStartTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.CompactArray, null, null);
                    info.xml.AppendStartTag(TranslatorTags.ContentsDescription);
                    AppendDataTypeAsXml(cols, info);
                    info.xml.AppendEndTag(TranslatorTags.ContentsDescription);
                    if (info.xml.OutputType == TranslatorOutputType.StandardXml)
                    {
                        info.xml.AppendStartTag((int)TranslatorTags.ArrayContents, null, null, true);
                        info.xml.Append(buff.RemainingHexString(true));
                        info.xml.AppendEndTag(TranslatorTags.ArrayContents, true);
                        info.xml.AppendEndTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.CompactArray);
                    }
                    else
                    {
                        info.xml.AppendStartTag(TranslatorTags.ArrayContents);
                    }
                }
                int start = buff.Position;
                while (buff.Position - start < len)
                {
                    GXStructure row = new GXStructure();
                    for (int pos = 0; pos != cols.Count; ++pos)
                    {
                        if (cols[pos] is GXStructure)
                        {
                            GetCompactArrayItem(null, buff, (List<object>)cols[pos], row, 1, false);
                        }
                        else if (cols[pos] is GXArray)
                        {
                            //For some reason there is count here in Italy standard. Remove it.
                            if (info.AppendAA)
                            {
                                GXCommon.GetObjectCount(buff);
                            }
                            GetCompactArrayItem(null, buff, ((List<object>)cols[pos]), row, 1, true);
                        }
                        else
                        {
                            GetCompactArrayItem(null, buff, (DataType)cols[pos], row, 1);
                        }
                        if (buff.Position == buff.Size)
                        {
                            break;
                        }
                    }
                    //If all columns are read.
                    if (row.Count >= cols.Count)
                    {
                        list.Add(row);
                    }
                    else
                    {
                        break;
                    }
                }
                if (info.xml != null && info.xml.OutputType == TranslatorOutputType.SimpleXml)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (List<object> row in list)
                    {
                        foreach (object it in row)
                        {
                            ToString(it, sb);
                        }
                        if (sb.Length != 0)
                        {
                            --sb.Length;
                        }
                        info.xml.AppendLine(sb.ToString());
                        sb.Length = 0;
                    }
                }
                if (info.xml != null && info.xml.OutputType == TranslatorOutputType.SimpleXml)
                {
                    info.xml.AppendEndTag(TranslatorTags.ArrayContents);
                    info.xml.AppendEndTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.CompactArray);
                }
                return list;
            }
            else
            {
                if (info.xml != null)
                {
                    info.xml.AppendStartTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.CompactArray, null, null);
                    info.xml.AppendStartTag(TranslatorTags.ContentsDescription);
                    info.xml.AppendEmptyTag(GXDLMS.DATA_TYPE_OFFSET + (int)dt);
                    info.xml.AppendEndTag(TranslatorTags.ContentsDescription);
                    info.xml.AppendStartTag((int)TranslatorTags.ArrayContents, null, null, true);
                    if (info.xml.OutputType == TranslatorOutputType.StandardXml)
                    {
                        info.xml.Append(buff.RemainingHexString(true));
                        info.xml.AppendEndTag(TranslatorTags.ArrayContents, true);
                        info.xml.AppendEndTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.CompactArray);
                    }
                }
                GetCompactArrayItem(null, buff, dt, list, len);
                if (info.xml != null && info.xml.OutputType == TranslatorOutputType.SimpleXml)
                {
                    foreach (Object it in list)
                    {
                        if (it is byte[])
                        {
                            info.xml.Append(GXCommon.ToHex((byte[])it, true));
                        }
                        else
                        {
                            info.xml.Append(Convert.ToString(it));
                        }
                        info.xml.Append(";");
                    }
                    if (list.Count != 0)
                    {
                        info.xml.SetXmlLength(info.xml.GetXmlLength() - 1);
                    }
                    info.xml.AppendEndTag(TranslatorTags.ArrayContents, true);
                    info.xml.AppendEndTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.CompactArray);
                }
            }
            return list;
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
        private static byte GetUInt8(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 1)
            {
                info.Complete = false;
                return 0;
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
        private static Int16 GetInt16(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 2)
            {
                info.Complete = false;
                return 0;
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
        private static sbyte GetInt8(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 1)
            {
                info.Complete = false;
                return 0;
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
                buff.Position += len;
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

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Logical name as a string.</returns>
        internal static string ToLogicalName(object value)
        {
            if (value is byte[])
            {
                byte[] buff = (byte[])value;
                if (buff.Length == 0)
                {
                    buff = new byte[6];
                }
                if (buff.Length == 6)
                {
                    return (buff[0] & 0xFF) + "." + (buff[1] & 0xFF) + "." + (buff[2] & 0xFF) + "." +
                           (buff[3] & 0xFF) + "." + (buff[4] & 0xFF) + "." + (buff[5] & 0xFF);
                }
#if !WINDOWS_UWP && !__MOBILE__
                throw new ArgumentException(Properties.Resources.InvalidLogicalName);
#else
#if WINDOWS_UWP
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                throw new ArgumentException(loader.GetString("InvalidLogicalName"));
#endif //WINDOWS_UWP
#if __MOBILE__
                throw new ArgumentException(Resources.InvalidLogicalName);
#endif //__MOBILE__
#endif //!WINDOWS_UWP && !__MOBILE__
            }
            return Convert.ToString(value);
        }

        internal static byte[] LogicalNameToBytes(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new byte[6];
            }
            string[] items = value.Split('.');
            // If data is string.
            if (items.Length != 6)
            {
#if !WINDOWS_UWP && !__MOBILE__
                throw new ArgumentException(Properties.Resources.InvalidLogicalName);
#else
#if WINDOWS_UWP
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                throw new ArgumentException(loader.GetString("InvalidLogicalName"));
#endif //WINDOWS_UWP
#if __MOBILE__
                throw new ArgumentException(Resources.InvalidLogicalName);
#endif //__MOBILE__
#endif //!WINDOWS_UWP && !__MOBILE__
            }
            byte[] buff = new byte[6];
            byte pos = 0;
            try
            {
                foreach (string it in items)
                {
                    buff[pos] = Convert.ToByte(it);
                    ++pos;
                }
            }
            catch (Exception)
            {
#if !WINDOWS_UWP && !__MOBILE__
                throw new ArgumentException(Properties.Resources.InvalidLogicalName);
#else
#if WINDOWS_UWP
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                throw new ArgumentException(loader.GetString("InvalidLogicalName"));
#endif //WINDOWS_UWP
#if __MOBILE__
                throw new ArgumentException(Resources.InvalidLogicalName);
#endif //__MOBILE__
#endif //!WINDOWS_UWP && !__MOBILE__
            }
            return buff;
        }

        ///<summary>
        ///Get octet string value from DLMS data.
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
        private static object GetOctetString(GXDLMSSettings settings, GXByteBuffer buff, GXDataInfo info, bool knownType)
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
                if (buff.Available < len)
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
                if (info.xml.Comments && tmp.Length != 0)
                {
                    // This might be logical name.
                    if (tmp.Length == 6 && tmp[5] == 0xFF)
                    {
                        info.xml.AppendComment(GXCommon.ToLogicalName(tmp));
                    }
                    else
                    {
                        bool isString = true;
                        //Try to change octet string to DateTime, Date or time.
                        if (tmp.Length == 12 || tmp.Length == 5 || tmp.Length == 4)
                        {
                            try
                            {
                                DataType type;
                                if (tmp.Length == 12)
                                {
                                    type = DataType.DateTime;
                                }
                                else if (tmp.Length == 5)
                                {
                                    type = DataType.Date;
                                }
                                else //if (tmp.Length == 4)
                                {
                                    type = DataType.Time;
                                }
                                GXDateTime dt = (GXDateTime)GXDLMSClient.ChangeType(tmp, type, settings.UseUtc2NormalTime);
                                if (dt.Value != DateTime.MaxValue)
                                {
                                    info.xml.AppendComment(dt.ToFormatMeterString());
                                    isString = false;
                                }
                            }
                            catch (Exception)
                            {
                                isString = true;
                            }
                        }
                        if (isString)
                        {
                            foreach (char it in tmp)
                            {
                                if (it < 32 || it > 126)
                                {
                                    isString = false;
                                    break;
                                }
                            }
                        }
                        if (isString)
                        {
                            info.xml.AppendComment(ASCIIEncoding.ASCII.GetString(tmp));
                        }
                    }
                }
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
        private static UInt32 GetUInt32(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 4)
            {
                info.Complete = false;
                return 0;
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
        private static Int32 GetInt32(GXByteBuffer buff, GXDataInfo info)
        {
            // If there is not enough data available.
            if (buff.Size - buff.Position < 4)
            {
                info.Complete = false;
                return 0;
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
        private static GXBitString GetBitString(GXByteBuffer buff, GXDataInfo info)
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
            return new GXBitString(sb.ToString());
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
                info.xml.AppendLine(info.xml.GetDataType(info.Type), "Value", value != 0 ? "true" : "false");
            }
            return value != 0;
        }

        /// <summary>
        /// Get data type in bytes.
        /// </summary>
        /// <param name="type">Data type.</param>
        /// <returns>Size of data type in bytes.</returns>
        public static int GetDataTypeSize(DataType type)
        {
            int size = -1;
            switch (type)
            {
                case DataType.Bcd:
                    size = 1;
                    break;
                case DataType.Boolean:
                    size = 1;
                    break;
                case DataType.Date:
                    size = 5;
                    break;
                case DataType.DateTime:
                    size = 12;
                    break;
                case DataType.Enum:
                    size = 1;
                    break;
                case DataType.Float32:
                    size = 4;
                    break;
                case DataType.Float64:
                    size = 8;
                    break;
                case DataType.Int16:
                    size = 2;
                    break;
                case DataType.Int32:
                    size = 4;
                    break;
                case DataType.Int64:
                    size = 8;
                    break;
                case DataType.Int8:
                    size = 1;
                    break;
                case DataType.None:
                    size = 0;
                    break;
                case DataType.Time:
                    size = 4;
                    break;
                case DataType.UInt16:
                    size = 2;
                    break;
                case DataType.UInt32:
                    size = 4;
                    break;
                case DataType.UInt64:
                    size = 8;
                    break;
                case DataType.UInt8:
                    size = 1;
                    break;
                default:
                    break;
            }
            return size;
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
                        int v = Convert.ToInt32(value);
                        if (v == 0x8000)
                        {
                            buff.SetUInt16(0x8000);
                        }
                        else
                        {
                            buff.SetInt16((short)v);
                        }
                    }
                    break;
                case DataType.UInt16:
                    buff.SetUInt16(Convert.ToUInt16(value));
                    break;
                case DataType.Int32:
                    if (value is DateTime)
                    {
                        buff.SetUInt32((UInt32)GXDateTime.ToUnixTime((DateTime)value));
                    }
                    else if (value is GXDateTime)
                    {
                        buff.SetUInt32((UInt32)GXDateTime.ToUnixTime(((GXDateTime)value).Value.DateTime));
                    }
                    else
                    {
                        buff.SetInt32(Convert.ToInt32(value));
                    }
                    break;
                case DataType.UInt32:
                    if (value is DateTime)
                    {
                        buff.SetUInt32((UInt32)GXDateTime.ToUnixTime((DateTime)value));
                    }
                    else if (value is GXDateTime)
                    {
                        buff.SetUInt32((UInt32)GXDateTime.ToUnixTime(((GXDateTime)value).Value.DateTime));
                    }
                    else
                    {
                        buff.SetUInt32(Convert.ToUInt32(value));
                    }
                    break;
                case DataType.Int64:
                    buff.SetUInt64(Convert.ToUInt64(value));
                    break;
                case DataType.UInt64:
                    buff.SetUInt64(Convert.ToUInt64(value));
                    break;
                case DataType.Float32:
                    buff.SetFloat(Convert.ToSingle(value));
                    break;
                case DataType.Float64:
                    buff.SetDouble(Convert.ToDouble(value));
                    break;
                case DataType.BitString:
                    SetBitString(buff, value, true);
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
                        SetDate(settings, buff, value);
                    }
                    else if (value is GXTime)
                    {
                        //Add size
                        buff.SetUInt8(4);
                        SetTime(settings, buff, value);
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
                    SetDate(settings, buff, value);
                    break;
                case DataType.Time:
                    SetTime(settings, buff, value);
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
        private static void SetTime(GXDLMSSettings settings, GXByteBuffer buff, object value)
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
            //Add additional date time skips.
            if (settings != null && settings.DateTimeSkips != DateTimeSkips.None)
            {
                dt.Skip |= settings.DateTimeSkips;
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
        private static void SetDate(GXDLMSSettings settings, GXByteBuffer buff, object value)
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
            //Add additional date time skips.
            if (settings != null && settings.DateTimeSkips != DateTimeSkips.None)
            {
                dt.Skip |= settings.DateTimeSkips;
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
            if ((dt.Skip & DateTimeSkips.Month) != 0)
            {
                buff.SetUInt8(0xFF);
            }
            else
            {
                if ((dt.Extra & DateTimeExtraInfo.DstBegin) != 0)
                {
                    buff.SetUInt8(0xFE);
                }
                else if ((dt.Extra & DateTimeExtraInfo.DstEnd) != 0)
                {
                    buff.SetUInt8(0xFD);
                }
                else
                {
                    buff.SetUInt8((byte)dt.Value.Month);
                }
            }
            // Add day
            if ((dt.Skip & DateTimeSkips.Day) != 0)
            {
                buff.SetUInt8(0xFF);
            }
            else
            {
                if ((dt.Extra & DateTimeExtraInfo.LastDay) != 0)
                {
                    buff.SetUInt8(0xFE);
                }
                else if ((dt.Extra & DateTimeExtraInfo.LastDay2) != 0)
                {
                    buff.SetUInt8(0xFD);
                }
                else
                {
                    buff.SetUInt8((byte)dt.Value.Day);
                }
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
            //Add additional date time skips.
            if (settings != null && settings.DateTimeSkips != DateTimeSkips.None)
            {
                dt.Skip |= settings.DateTimeSkips;
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
                if ((dt.Extra & DateTimeExtraInfo.DstBegin) != 0)
                {
                    buff.SetUInt8(0xFE);
                }
                else if ((dt.Extra & DateTimeExtraInfo.DstEnd) != 0)
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
                if ((dt.Extra & DateTimeExtraInfo.LastDay) != 0)
                {
                    buff.SetUInt8(0xFE);
                }
                else if ((dt.Extra & DateTimeExtraInfo.LastDay2) != 0)
                {
                    buff.SetUInt8(0xFD);
                }
                else
                {
                    buff.SetUInt8((byte)tm.Day);
                }
            }
            else
            {
                buff.SetUInt8(0xFF);
            }
            // Add week day
            if ((dt.Skip & DateTimeSkips.DayOfWeek) != 0)
            {
                //Skip.
                buff.SetUInt8(0xFF);
            }
            else
            {
                if (dt.DayOfWeek == 0)
                {
                    byte d = (byte)dt.Value.DayOfWeek;
                    //If Sunday.
                    if (d == 0)
                    {
                        d = 7;
                    }
                    buff.SetUInt8(d);
                }
                else
                {
                    buff.SetUInt8((byte)(dt.DayOfWeek));
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
            if ((dt.Skip & DateTimeSkips.Deviation) == 0)
            {
                Int16 deviation = (Int16)dt.Value.Offset.TotalMinutes;
                if (settings != null && settings.UseUtc2NormalTime)
                {
                    buff.SetInt16(deviation);
                }
                else
                {
                    buff.SetInt16((Int16)(-deviation));
                }
            }
            else //deviation not used  .
            {
                buff.SetUInt16(0x8000);
            }
            //Add clock_status
            if ((dt.Skip & DateTimeSkips.Status) == 0)
            {
                buff.SetUInt8((byte)dt.Status);
            }
            else //Status is not used.
            {
                buff.SetUInt8((byte)0xFF);
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
                List<object> tmp;
                if (value is List<object>)
                {
                    tmp = (List<object>)value;
                }
                else
                {
                    tmp = new List<object>();
                    tmp.AddRange((object[])value);
                }
                SetObjectCount(tmp.Count, buff);
                foreach (object it in tmp)
                {
                    DataType dt = GXDLMSConverter.GetDLMSDataType(it);
                    SetData(settings, buff, dt, it);
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
            if (value is string)
            {
                byte[] tmp = GXCommon.HexToBytes((string)value);
                SetObjectCount(tmp.Length, buff);
                buff.Set(tmp);
            }
            else if (value is byte[] || value is sbyte[])
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
            if (value is byte[])
            {
                byte[] tmp = (byte[])value;
                SetObjectCount(tmp.Length, buff);
                buff.Set(tmp);
            }
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
        internal static void SetBitString(GXByteBuffer buff, object value, bool addCount)
        {
            if (value is GXBitString)
            {
                value = (value as GXBitString).Value;
            }
            if (value is string)
            {
                byte val = 0;
                String str = (String)value;
                if (addCount)
                {
                    SetObjectCount(str.Length, buff);
                }
                int index = 7;
                for (int pos = 0; pos != str.Length; ++pos)
                {
                    char it = str[pos];
                    if (it == '1')
                    {
                        val |= (byte)(1 << index);
                    }
                    else if (it != '0')
                    {
                        throw new ArgumentException("Not a bit string.");
                    }
                    --index;
                    if (index == -1)
                    {
                        index = 7;
                        buff.SetUInt8(val);
                        val = 0;
                    }
                }
                if (index != 7)
                {
                    buff.SetUInt8(val);
                }
            }
            else if (value is byte[])
            {
                byte[] arr = (byte[])value;
                SetObjectCount(8 * arr.Length, buff);
                buff.Set(arr);
            }
            else if (value == null)
            {
                buff.SetUInt8(0);
            }
            else if (value is byte)
            {
                SetObjectCount(8, buff);
                buff.SetUInt8((byte)value);
            }
            else
            {
                throw new Exception("BitString must give as string.");
            }
        }

        /// <summary>
        /// Get Logical name test from properties.
        /// </summary>
        /// <returns></returns>
        public static string GetLogicalNameString()
        {
#if !WINDOWS_UWP && !__MOBILE__
            return Gurux.DLMS.Properties.Resources.LogicalNameTxt;
#else
#if WINDOWS_UWP
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            return loader.GetString("LogicalNameTxt");
#endif //WINDOWS_UWP
#if __MOBILE__
            return Resources.LogicalNameTxt;
#endif //__MOBILE__
#endif //!WINDOWS_UWP && !__MOBILE__
        }

#if WINDOWS_UWP
        public static string GetDateSeparator()
        {
            return "-";
        }
        public static string GetTimeSeparator()
        {
            return ":";
        }
#endif

        /// <summary>
        /// Convert DLMS data type to .Net data type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static public Type GetDataType(DataType type)
        {
            switch (type)
            {
                case DataType.None:
                    return null;
                case DataType.Array:
                    return typeof(GXArray);
                case DataType.CompactArray:
                case DataType.Structure:
                    return typeof(GXStructure);
                case DataType.Bcd:
                    return typeof(string);
                case DataType.BitString:
                    return typeof(GXBitString);
                case DataType.Boolean:
                    return typeof(bool);
                case DataType.Date:
                    return typeof(DateTime);
                case DataType.DateTime:
                    return typeof(DateTime);
                case DataType.Float32:
                    return typeof(float);
                case DataType.Float64:
                    return typeof(double);
                case DataType.Int16:
                    return typeof(Int16);
                case DataType.Int32:
                    return typeof(Int32);
                case DataType.Int64:
                    return typeof(Int64);
                case DataType.Int8:
                    return typeof(sbyte);
                case DataType.OctetString:
                    return typeof(byte[]);
                case DataType.String:
                    return typeof(string);
                case DataType.Time:
                    return typeof(DateTime);
                case DataType.UInt16:
                    return typeof(UInt16);
                case DataType.UInt32:
                    return typeof(UInt32);
                case DataType.UInt64:
                    return typeof(UInt64);
                case DataType.UInt8:
                    return typeof(byte);
                case DataType.Enum:
                    return typeof(GXEnum);
            }
            throw new Exception("Invalid DLMS data type.");
        }

        static public DataType GetDLMSDataType(Type type)
        {
            //If expected type is not given return property type.
            if (type == null)
            {
                return DataType.None;
            }
            else if (type == typeof(Int32))
            {
                return DataType.Int32;
            }
            else if (type == typeof(UInt32))
            {
                return DataType.UInt32;
            }
            else if (type == typeof(String))
            {
                return DataType.String;
            }
            else if (type == typeof(byte))
            {
                return DataType.UInt8;
            }
            else if (type == typeof(sbyte))
            {
                return DataType.Int8;
            }
            else if (type == typeof(Int16))
            {
                return DataType.Int16;
            }
            else if (type == typeof(UInt16))
            {
                return DataType.UInt16;
            }
            else if (type == typeof(Int64))
            {
                return DataType.Int64;
            }
            else if (type == typeof(UInt64))
            {
                return DataType.UInt64;
            }
            if (type == typeof(float))
            {
                return DataType.Float32;
            }
            else if (type == typeof(double))
            {
                return DataType.Float64;
            }
            else if (type == typeof(DateTime) || type == typeof(GXDateTime))
            {
                return DataType.DateTime;
            }
            else if (type == typeof(GXDate))
            {
                return DataType.Date;
            }
            else if (type == typeof(GXTime))
            {
                return DataType.Time;
            }
            else if (type == typeof(bool))
            {
                return DataType.Boolean;
            }
            else if (type == typeof(byte[]))
            {
                return DataType.OctetString;
            }
            else if (type == typeof(GXStructure))
            {
                return DataType.Structure;
            }
            else if (type == typeof(GXArray) || type == typeof(object[]))
            {
                return DataType.Array;
            }
            else if (type == typeof(GXEnum) || type.IsEnum)
            {
                return DataType.Enum;
            }
            else if (type == typeof(GXBitString))
            {
                return DataType.BitString;
            }
            else if (type == typeof(GXByteBuffer))
            {
                return DataType.OctetString;
            }
            else
            {
                throw new Exception("Failed to convert data type to DLMS data type. Unknown data type.");
            }
        }

        public static DateTimeOffset GetGeneralizedTime(string dateString)
        {
            int year, month, day, hour, minute, second = 0;
            year = int.Parse(dateString.Substring(0, 4));
            month = int.Parse(dateString.Substring(4, 2));
            day = int.Parse(dateString.Substring(6, 2));
            hour = int.Parse(dateString.Substring(8, 2));
            minute = int.Parse(dateString.Substring(10, 2));
            // If UTC time.
            if (dateString.EndsWith("Z"))
            {
                if (dateString.Length > 13)
                {
                    second = int.Parse(dateString.Substring(12, 2));
                }
                return new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero).ToLocalTime();
            }
            if (dateString.Length > 17)
            {
                second = int.Parse(dateString.Substring(12, 2));
            }
            int deviation = 60 * int.Parse(dateString.Substring(dateString.Length - 4, 2));
            deviation += int.Parse(dateString.Substring(dateString.Length - 2, 2));
            DateTime dt = new DateTime(year, month, day, hour, minute, second);
            if (dt.IsDaylightSavingTime())
            {
                deviation += 60;
            }
            return new DateTimeOffset(dt, TimeSpan.FromMinutes(deviation));
        }

        public static String GeneralizedTime(DateTimeOffset date)
        {
            date = date.ToUniversalTime();
            StringBuilder sb = new StringBuilder();
            sb.Append(date.Year.ToString("D4"));
            sb.Append(date.Month.ToString("D2"));
            sb.Append(date.Day.ToString("D2"));
            sb.Append(date.Hour.ToString("D2"));
            sb.Append(date.Minute.ToString("D2"));
            sb.Append(date.Second.ToString("D2"));
            //UTC time.
            sb.Append("Z");
            return sb.ToString();
        }

        public static void DatatoXml(object value, GXDLMSTranslatorStructure xml)
        {
            if (value == null)
            {
                xml.AppendEmptyTag(xml.GetDataType(DataType.None));
            }
            else if (value is GXStructure)
            {
                xml.AppendStartTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Structure, null, null);
                foreach (object it in (List<object>)value)
                {
                    DatatoXml(it, xml);
                }
                xml.AppendEndTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Structure);
            }
            else if (value is GXArray)
            {
                xml.AppendStartTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Array, null, null);
                foreach (object it in (List<object>)value)
                {
                    DatatoXml(it, xml);
                }
                xml.AppendEndTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Array);
            }
            else if (value is IPAddress)
            {
                xml.AppendLine(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.OctetString, null, ((IPAddress)value).GetAddressBytes());
            }
            else if (value is GXDLMSObjectCollection)
            {
                xml.AppendStartTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Array, null, null);
                foreach (GXDLMSObject it in value as GXDLMSObjectCollection)
                {
                    DatatoXml(it, xml);
                }
                xml.AppendEndTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Array);
            }
            else if (value is GXDLMSObject)
            {
                xml.AppendStartTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Structure, null, null);
                GXDLMSObject obj = value as GXDLMSObject;
                xml.AppendLine(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt16, null, obj.ObjectType);
                xml.AppendLine(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt8, null, obj.Version);
                xml.AppendLine(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.OctetString, null, obj.LogicalName);
                xml.AppendEndTag(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Structure);
            }
            else
            {
                DataType dt = GetDLMSDataType(value.GetType());
                xml.AppendLine(GXDLMS.DATA_TYPE_OFFSET + (int)dt, null, value);
            }
        }

        /// <summary>
        /// Encrypt Flag name to two bytes.
        /// </summary>
        /// <param name="flagName">3 letter Flag name.</param>
        /// <returns>Encrypted Flag name.</returns>
        public static UInt16 EncryptManufacturer(string flagName)
        {
            if (flagName.Length != 3)
            {
                throw new ArgumentOutOfRangeException("Invalid Flag name.");
            }
            UInt16 value = (char)((flagName[0] - 0x40) & 0x1f);
            value <<= 5;
            value += (char)((flagName[1] - 0x40) & 0x1f);
            value <<= 5;
            value += (char)((flagName[2] - 0x40) & 0x1f);
            return value;
        }

        /// <summary>
        /// Descrypt two bytes to Flag name.
        /// </summary>
        /// <param name="value">Encrypted Flag name.</param>
        /// <returns>Flag name.</returns>
        public static string DecryptManufacturer(UInt16 value)
        {
            UInt16 tmp = (UInt16)(value >> 8 | value << 8);
            char c = (char)((tmp & 0x1f) + 0x40);
            tmp = (UInt16)(tmp >> 5);
            char c1 = (char)((tmp & 0x1f) + 0x40);
            tmp = (UInt16)(tmp >> 5);
            char c2 = (char)((tmp & 0x1f) + 0x40);
            return new string(new char[] { c2, c1, c });
        }

        static string IdisSystemTitleToString(byte[] st, bool addComments)
        {
            StringBuilder sb = new StringBuilder();
            if (addComments)
            {
                sb.AppendLine("IDIS system title:");
                sb.Append("Manufacturer Code: ");
                sb.AppendLine(new string(new char[] { (char)st[0], (char)st[1], (char)st[2] }));
                sb.Append("Device type: ");
                switch(st[3])
                {
                    case 99:
                        sb.Append("DC");
                        break;
                    case 100:
                        sb.Append("IDIS package1 PLC single phase meter");
                        break;
                    case 101:
                        sb.Append("IDIS package1 PLC polyphase meter");
                        break;
                    case 102:
                        sb.Append("IDIS package2 IP single phase meter");
                        break;
                    case 103:
                        sb.Append("IDIS package2 IP polyphase meter");
                        break;
                }
                sb.AppendLine("");
                sb.Append("Function type: ");
                int ft = st[4] >> 4;
                bool add = false;
                if ((ft & 0x1) != 0)
                {
                    sb.Append("Disconnector");
                    add = true;
                }
                if ((ft & 0x2) != 0)
                {
                    if (add)
                    {
                        sb.Append(", ");
                    }
                    add = true;
                    sb.Append("Load Management");
                }
                if ((ft & 0x4) != 0)
                {
                    if (add)
                    {
                        sb.Append(", ");
                    }
                    sb.Append("Multi Utility");
                }
                //Serial number;
                int sn = (st[4] & 0xF) << 24;
                sn |= (st[5] << 16);
                sn |= (st[6] << 8);
                sn |= (st[7]);
                sb.AppendLine("");
                sb.Append("Serial number: ");
                sb.AppendLine(sn.ToString());
            }
            else
            {
                sb.Append(new string(new char[] { (char)st[0], (char)st[1], (char)st[2] }));
                sb.Append(" ");
                //Serial number;
                int sn = (st[4] & 0xF) << 24;
                sn |= (st[5] << 16);
                sn |= (st[6] << 8);
                sn |= (st[7]);
                sb.Append(sn.ToString());
            }
            return sb.ToString();
        }

        static string DlmsSystemTitleToString(byte[] st, bool addComments)
        {
            StringBuilder sb = new StringBuilder();
            if (addComments)
            {
                sb.AppendLine("DLMS system title:");
                sb.Append("Manufacturer Code: ");
                sb.AppendLine(new string(new char[] { (char)st[0], (char)st[1], (char)st[2] }));
                sb.Append("Serial number: ");
                //Serial number;
                int sn = (st[5] << 16);
                sn |= (st[6] << 8);
                sn |= (st[7]);
                sb.AppendLine(sn.ToString());
            }
            else
            {
                sb.Append(new string(new char[] { (char)st[0], (char)st[1], (char)st[2] }));
                sb.Append(" ");
                //Serial number;
                int sn = (st[5] << 16);
                sn |= (st[6] << 8);
                sn |= (st[7]);
                sb.AppendLine(sn.ToString());
            }
            return sb.ToString();
        }

        static string UNISystemTitleToString(byte[] st, bool addComments)
        {
            StringBuilder sb = new StringBuilder();
            if (addComments)
            {
                sb.AppendLine("UNI/TS system title:");
                sb.Append("Manufacturer: ");
                UInt16 m = (UInt16)(st[0] << 8 | st[1]);
                sb.AppendLine(DecryptManufacturer(m));
                sb.Append("Serial number: ");
                sb.AppendLine(ToHex(new byte[] { st[7], st[6], st[5], st[4], st[3], st[2] }, false));
            }
            else
            {
                UInt16 m = (UInt16)(st[0] << 8 | st[1]);
                sb.Append(DecryptManufacturer(m));
                sb.Append(ToHex(new byte[] { st[7], st[6], st[5], st[4], st[3], st[2] }, false));
            }
            return sb.ToString();
        }

        private static bool IsT1(byte value)
        {
            return value > 98 && value < 104;
        }

        private static bool IsT2(byte value)
        {
            return (value & 0xf0) != 0;
        }

        /// <summary>
        /// Convert system title to string.
        /// </summary>
        /// <param name="standard">Used standard.</param>
        /// <param name="st">System title.</param>
        /// <param name="addComments">Are comments added.</param>
        /// <returns>System title in string format.</returns>
        public static string SystemTitleToString(Standard standard, byte[] st, bool addComments)
        {
            if (st == null || st.Length != 8)
            {
                return "";
            }
            if (standard == Standard.Italy || !Char.IsLetter((char)st[0]) || !Char.IsLetter((char)st[1]) ||
                !Char.IsLetter((char)st[2]))
            {
                return UNISystemTitleToString(st, addComments);
            }
            if (standard == Standard.Idis || (IsT1(st[3]) && IsT2(st[4])))
            {
                return IdisSystemTitleToString(st, addComments);
            }
            return DlmsSystemTitleToString(st, addComments);
        }

        // Reserved for internal use.
        public static byte SwapBits(byte value)
        {
            byte ret = 0;
            for (int pos = 0; pos != 8; ++pos)
            {
                ret = (byte)((ret << 1) | (value & 0x01));
                value = (byte)(value >> 1);
            }
            return ret;
        }

        public static byte[] FromBase64(string value)
        {
            return Convert.FromBase64String(value);
        }

        public static string ToBase64(byte[] value)
        {
            return System.Convert.ToBase64String(value);
        }       
    }

    static class GXHelpers2
    {
        public static T[] Fill<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = value;
            }
            return arr;
        }
    }
}