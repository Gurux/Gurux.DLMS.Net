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
using System.Runtime.InteropServices;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using Microsoft.Win32;
using System.Xml;
using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Text;
using System.Linq;

using System.Reflection;
using System.Collections.Generic;
#if !__MOBILE__
using System.Windows.Forms;
#endif
namespace Gurux.Common
{
    /// <summary>
    /// Common Gurux helpers.
    /// </summary>
    public class GXCommon
    {
        /// <summary>
        /// Is string guid.
        /// </summary>
        /// <param name="possibleGuid"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsGuid(string possibleGuid, out Guid guid)
        {
            try
            {
                if (possibleGuid.Length == 36 && possibleGuid[8] == '-')
                {
                    guid = new Guid(possibleGuid);
                    return true;
                }
                else
                {
                    guid = Guid.Empty;
                    return false;
                }
            }
            catch (Exception)
            {
                guid = Guid.Empty;
                return false;
            }
        }
#if !__MOBILE__
        /// <summary>
        /// Check that correct framework is installed.
        /// </summary>
        static public void CheckFramework()
        {
            //Is .Net 4.0 client installed.
            const string net40 = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client";
            using (RegistryKey subKey = Registry.LocalMachine.OpenSubKey(net40))
            {
                if (subKey != null && Convert.ToUInt32(subKey.GetValue("Install")) == 1)
                {
                    //Everything is OK.
                    return;
                }
            }

            //Is .Net 3.5 installed.
            const string net35 = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5";
            using (RegistryKey subKey = Registry.LocalMachine.OpenSubKey(net35))
            {
                if (subKey != null && Convert.ToUInt32(subKey.GetValue("Install")) == 1)
                {
                    string version = Convert.ToString(subKey.GetValue("Version"));
                    string servicePack = Convert.ToString(subKey.GetValue("SP"));
                    if (string.IsNullOrEmpty(servicePack))
                    {
                        throw new Exception(Gurux.Common.Properties.Resources.NetFramework35SP1MustBeInstalledBeforeTheApplicationCanBeUsed);
                    }
                    return;
                }
            }
            throw new Exception(Gurux.Common.Properties.Resources.NetFramework35SP1Or40MustBeInstalledBeforeTheApplicationCanBeUsed);
        }

        /// <summary>
        /// Title of message box
        /// </summary>
        public static string Title = "";

        /// <summary>
        /// Parent window of message box
        /// </summary>
        public static Control Owner = null;
#endif
        /// <summary>
        /// Convert byte array to hex string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <param name="addSpace">Is space added between bytes.</param>
        /// <returns>Byte array as hex string.</returns>
        public static string ToHex(byte[] bytes, bool addSpace)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return string.Empty;
            }
            int len = bytes.Length * (addSpace ? 3 : 2);
            char[] str = new char[len];
            int tmp;
            len = 0;
            for (int pos = 0; pos != bytes.Length; ++pos)
            {
                tmp = (bytes[pos] >> 4);
                str[len] = (char)(tmp > 9 ? tmp + 0x37 : tmp + 0x30);
                ++len;
                tmp = (bytes[pos] & 0x0F);
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
        /// Convert string to byte array.
        /// </summary>
        /// <param name="str">Hex string</param>
        /// <param name="includeSpace">Is there space between hex values.</param>
        /// <returns>Byte array.</returns>
        [ObsoleteAttribute("Use HexToBytes without includeSpace.")]
        public static byte[] HexToBytes(string str, bool includeSpace)
        {
            //Remove spaces.
            if (str != null)
            {
                str = str.Trim();
            }
            if (string.IsNullOrEmpty(str))
            {
                return new byte[0];
            }
            if (includeSpace && !string.IsNullOrEmpty(str) && str[str.Length - 1] != ' ')
            {
                str += " ";
            }
            if (str.Length == 0)
            {
                return new byte[0];
            }
            int cnt = includeSpace ? 3 : 2;
            if (str.Length % cnt != 0)
            {
                throw new ArgumentException(Gurux.Common.Properties.Resources.NotHexString);
            }
            byte[] buffer = new byte[str.Length / cnt];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                c = str[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);
                c = str[++sx];
                buffer[bx] |= (byte)(c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0'));
                if (includeSpace)
                {
                    ++sx;
                }
            }
            return buffer;
        }

        /// <summary>
        /// Convert char hex value to byte value.
        /// </summary>
        /// <param name="c">Character to convert hex.</param>
        /// <returns>Byte value of hex char value.</returns>
        private static byte GetValue(byte c)
        {
            byte value = 0xFF;
            // If number
            if (c > 0x2F && c < 0x3A)
            {
                value = (byte)(c - '0');
            }
            else if (c > 0x40 && c < 'G')
            {
                // If upper case.
                value = (byte)(c - 'A' + 10);
            }
            else if (c > 0x60 && c < 'g')
            {
                // If lower case.
                value = (byte)(c - 'a' + 10);
            }
            return value;
        }

        private static bool IsHex(byte c)
        {
            return GetValue(c) != 0xFF;
        }

        /// <summary>
        /// Convert string to byte array.
        /// </summary>
        /// <param name="value">Hex string</param>
        /// <returns>Byte array.</returns>
        public static byte[] HexToBytes(string value)
        {
            if (value == null || value.Length == 0)
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
            // If there are no spaces in the hex string.
            if (buffer.Length == index)
            {
                return buffer;
            }
            byte[] tmp = new byte[index];
            Buffer.BlockCopy(buffer, 0, tmp, 0, index);
            return tmp;
        }

        /// <summary>
        /// Writes a timestamped line using System.Diagnostics.Trace.WriteLine
        /// </summary>
        /// <param name="line">Trace string</param>
        public static void TraceWriteLine(string line)
        {
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("HH:mm:ss.ffff") + " " + line);
        }

        /// <summary>
        /// Writes a timestamped string using System.Diagnostics.Trace.Write
        /// </summary>
        /// <param name="line">Trace string</param>
        public static void TraceWrite(string line)
        {
            System.Diagnostics.Trace.Write(DateTime.Now.ToString("HH:mm:ss.ffff") + " " + line);
        }

        /// <summary>
        /// Convert object to byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Object as byte array.</returns>
        public static byte[] GetAsByteArray(object value)
        {
            if (value == null)
            {
                return new byte[0];
            }
            if (value is byte[])
            {
                return (byte[])value;
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

        /// <summary>
        /// Convert byte array to object.
        /// </summary>
        /// <param name="byteArray">Byte array where Object uis created.</param>
        /// <param name="type">Object type.</param>
        /// <param name="index">Byte array index.</param>
        /// <param name="count">Byte count.</param>
        /// <param name="reverse">Is value reversed.</param>
        /// <param name="readBytes">Count of read bytes.</param>
        /// <returns>Return object of given type.</returns>
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
            //If count is higher than one and type is not array.
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
        /// <param name="type">Object type.</param>
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
            //If not enough data available.
            if ((count - index) < pattern.Length)
            {
                return -1;
            }
            byte firstByte = pattern[0];
            int pos = -1;
            if ((pos = Array.IndexOf(input, firstByte, index, count - index)) >= 0)
            {
                if (count - pos < pattern.Length)
                {
                    pos = -1;
                }
                else
                {
                    for (int i = 0; i < pattern.Length; i++)
                    {
                        if (pos + i >= input.Length || pattern[i] != input[pos + i])
                        {
                            return IndexOf(input, pattern, pos + 1, count);
                        }
                    }
                }
            }
            return pos;
        }

        /// <summary>
        /// Compares two byte or byte array values.
        /// </summary>
        public static bool EqualBytes(byte[] a, byte[] b)
        {
            if (a == null)
            {
                return b == null;
            }
            if (b == null)
            {
                return a == null;
            }
            if (a is Array && b is Array)
            {
                int pos = 0;
                if (((Array)a).Length != ((Array)b).Length)
                {
                    return false;
                }
                foreach (byte mIt in (byte[])a)
                {
                    if ((((byte)((byte[])b).GetValue(pos++)) & mIt) != mIt)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return BitConverter.Equals(a, b);
            }
            return true;
        }

        /// <summary>
        /// Retrieves the path to application data.
        /// </summary>
        public static string ApplicationDataPath
        {
            get
            {
                string path = string.Empty;
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    path = "/usr";
                }
                else
                {
                    //Vista: C:\ProgramData
                    //XP: c:\Program Files\Common Files
                    //XP = 5.1 & Vista = 6.0
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    }
                    else
                    {
                        path = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                    }
                }
                return path;
            }
        }

        /// <summary>
        /// If we are running program from debugger, all protocol Add-Ins are loaded from child "Protocols"- directory.
        /// </summary>
        public static string ProtocolAddInsPath
        {
            get
            {
                string strPath = "";
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    strPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    strPath = System.IO.Path.Combine(strPath, ".Gurux");
                }
                else
                {
                    if (Environment.OSVersion.Version.Major < 6)
                    {
                        strPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                    }
                    else
                    {
                        strPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    }
                    strPath = System.IO.Path.Combine(strPath, "Gurux");
                }
                strPath = Path.Combine(strPath, "AddIns");
                return strPath;
            }
        }

        /// <summary>
        /// Retrieves application data path from environment variables.
        /// </summary>
        public static string UserDataPath
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return path;
            }
        }

#if !__MOBILE__
        /// <summary>
        /// Shows an error message.
        /// </summary>
        public static void ShowError(IWin32Window parent, string title, Exception ex)
        {
            try
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                if (ex.StackTrace != null)
                {
                    Gurux.Common.GXCommon.TraceWrite(ex.StackTrace.ToString());
                }
                string path = ApplicationDataPath;
                if (System.Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    path = Path.Combine(path, ".Gurux");
                }
                else
                {
                    path = Path.Combine(path, "Gurux");
                }
                path = Path.Combine(path, "LastError.txt");
                try
                {
                    using (System.IO.TextWriter tw = System.IO.File.CreateText(path))
                    {
                        tw.Write(ex.ToString());
                        if (ex.StackTrace != null)
                        {
                            tw.Write("----------------------------------------------------------\r\n");
                            tw.Write(ex.StackTrace.ToString());
                        }
                        tw.Close();
                    }
                    GXFileSystemSecurity.UpdateFileSecurity(path);
                }
                catch (Exception)
                {
                    //Skip error.
                }
                if (parent != null && !((Control)parent).IsDisposed && !((Control)parent).InvokeRequired)
                {
                    MessageBox.Show(parent, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                //Do nothing. Fatal exception blew up message box.
            }
        }

        /// <summary>
        /// Shows an error message.
        /// </summary>
        public static void ShowError(Exception ex)
        {
            ShowError(Owner, Title, ex);
        }

        /// <summary>
        /// Shows an error question dialog.
        /// </summary>
        public static DialogResult ShowQuestion(string str)
        {
            return ShowQuestion(Owner, Title, str);
        }

        /// <summary>
        /// Shows an error exclamation dialog.
        /// </summary>
        public static DialogResult ShowExclamation(string str)
        {
            return ShowExclamation(Owner, Title, str);
        }

        /// <summary>
        /// Shows an error message.
        /// </summary>
        public static void ShowError(IWin32Window parent, Exception ex)
        {
            ShowError(parent, Title, ex);
        }

        /// <summary>
        /// Shows an error question dialog.
        /// </summary>
        public static DialogResult ShowQuestion(IWin32Window parent, string str)
        {
            return ShowQuestion(parent, Title, str);
        }

        /// <summary>
        /// Shows an error exclamation dialog.
        /// </summary>
        public static DialogResult ShowExclamation(IWin32Window parent, string str)
        {
            return ShowExclamation(parent, Title, str);
        }

        /// <summary>
        /// Shows an error question dialog.
        /// </summary>
        public static DialogResult ShowQuestion(IWin32Window parent, string title, string str)
        {
            try
            {
                if (Environment.UserInteractive)
                {
                    Control tmp = parent as Control;
                    if (tmp != null)
                    {
                        if (tmp.InvokeRequired)
                        {
                            if (tmp.IsHandleCreated)
                            {
                                return (DialogResult)tmp.Invoke(new ShowDialogEventHandler(ShowQuestion), new object[] { parent, title, str });
                            }
                            return MessageBox.Show(str, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                        }
                        return MessageBox.Show(parent, str, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    }
                    else
                    {
                        return MessageBox.Show(str, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    }
                }
                else
                {
                    return DialogResult.Yes;
                }
            }
            catch
            {
                //Do nothing. Fatal exception blew up message box.
                return DialogResult.Abort;
            }
        }

        delegate DialogResult ShowDialogEventHandler(IWin32Window parent, string title, string str);

        /// <summary>
        /// Shows an error exclamation dialog.
        /// </summary>
        public static DialogResult ShowExclamation(IWin32Window parent, string title, string str)
        {
            try
            {
                if (Environment.UserInteractive)
                {
                    Control tmp = parent as Control;
                    if (tmp != null)
                    {
                        if (tmp.InvokeRequired)
                        {
                            if (tmp.IsHandleCreated)
                            {
                                return (DialogResult)tmp.Invoke(new ShowDialogEventHandler(ShowExclamation), new object[] { parent, title, str });
                            }
                            return MessageBox.Show(str, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                        }
                        return MessageBox.Show(parent, str, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        return MessageBox.Show(str, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    }
                }
                return DialogResult.OK;
            }
            catch
            {
                //Do nothing. Fatal exception blew up message box.
                return DialogResult.Abort;
            }
        }
#endif

        /// <summary>
        /// Check an assembly to see if it has the given public key token
        /// </summary>
        /// <remarks>
        /// Does not check to make sure the assembly's signature is valid.
        /// Loads the assembly in the LoadFrom context.
        /// </remarks>
        /// <param name='target'>Path to the assembly to check</param>
        /// <param name='expectedToken'>Token to search for</param>
        /// <exception cref='System.ArgumentNullException'>If assembly or expectedToken are null</exception>
        /// <returns>true if the assembly was signed with a key that has this token, false otherwise</returns>
        private static bool CheckToken(Assembly target, byte[] expectedToken)
        {
            try
            {
                if (expectedToken == null || expectedToken.Length == 0)
                {
                    return false;
                }
                // Get the public key token of the given assembly
                byte[] asmToken = target.GetName().GetPublicKeyToken();

                // Compare it to the given token
                if (asmToken.Length != expectedToken.Length)
                    return false;

                for (int i = 0; i < asmToken.Length; i++)
                    if (asmToken[i] != expectedToken[i])
                        return false;

                return true;
            }
            catch (System.IO.FileNotFoundException)
            {
                // couldn't find the assembly
                return false;
            }
            catch (BadImageFormatException)
            {
                // the given file couldn't get through the loader
                return false;
            }
        }

        /// <summary>
        /// Is assembly default .Net assembly.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static bool IsDefaultAssembly(Assembly asm)
        {
            // Check to see if it is a Microsoft assembly
            byte[] msClrToken = new byte[] { 0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89 };
            byte[] msFxToken = new byte[] { 0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a };
            bool isMsAsm = CheckToken(asm, msClrToken) || CheckToken(asm, msFxToken);
            if (isMsAsm)
            {
                return true;
            }

            //If Gurux common assembly
            if (CheckToken(asm, typeof(GXCommon).Assembly.GetName().GetPublicKeyToken()))
            {
                return true;
            }
            if (asm.GetTypes().Length == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes case sensitivity of given string.
        /// </summary>
        /// <param name="original">Original string.</param>
        /// <param name="pattern">String to replace.</param>
        /// <param name="replacement">Replacing string.</param>
        /// <returns>The replaced string.</returns>
        public static string ReplaceEx(string original, string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) *
                      (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern,
                                                    position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            return new string(chars, 0, count);
        }

        /// <summary>
        /// Returns command line parameters.
        /// </summary>
        /// <param name="args">Command line parameters.</param>
        /// <param name="optstring">Expected option tags.</param>
        /// <returns>List of command line parameters</returns>
        public static List<GXCmdParameter> GetParameters(string[] args, string optstring)
        {
            List<GXCmdParameter> list = new List<GXCmdParameter>();
            for (int index = 0; index < args.Length; ++index)
            {
                if (args[index][0] != '-' && args[index][0] != '/')
                {
                    throw new ArgumentException("Invalid parameter: " + args[index]);
                }
                int pos = optstring.IndexOf(args[index][1]);
                if (pos == -1)
                {
                    throw new ArgumentException("Invalid parameter: " + args[index]);
                }
                GXCmdParameter c = new GXCmdParameter();
                c.Tag = args[index][1];
                list.Add(c);
                if (pos < optstring.Length - 1 && optstring[1 + pos] == ':')
                {
                    ++index;
                    if (args.Length <= index)
                    {
                        c.Missing = true;
                    }
                    c.Value = args[index];
                }
            }
            return list;
        }
    }
}
