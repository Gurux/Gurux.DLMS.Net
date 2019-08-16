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
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Diagnostics;

namespace GXDLMS.ManufacturerSettings
{
    class GXFileInfo
    {
        public const uint TOKEN_QUERY = 0x0008;

        /// <summary>
        /// Enumerates what type of information of the token it contains.
        /// </summary>
        public enum TOKEN_INFORMATION_CLASS
        {
            /// <summary>
            /// User account of the token.
            /// </summary>
            TokenUser = 1,
            /// <summary>
            /// Group accounts associated with the token.
            /// </summary>
            TokenGroups = 2,
            /// <summary>
            /// Privileges of the token.
            /// </summary>
            TokenPrivileges = 3,
            /// <summary>
            /// Default owner security identifier (SID) for new objects.
            /// </summary>
            TokenOwner = 4,
            /// <summary>
            /// Default primary group for new objects.
            /// </summary>
            TokenPrimaryGroup = 5,
            /// <summary>
            /// Default DACL for new objects.
            /// </summary>
            TokenDefaultDacl = 6,
            /// <summary>
            /// Source of the token.
            /// </summary>
            TokenSource = 7,
            /// <summary>
            /// Token type (primary or impersonation).
            /// </summary>
            TokenType = 8,
            /// <summary>
            /// Impersonation level of the token.
            /// </summary>
            TokenImpersonationLevel = 9,
            /// <summary>
            /// Statistics of the token.
            /// </summary>
            TokenStatistics = 10,
            /// <summary>
            /// List of restricting security identifiers (SIDs).
            /// </summary>
            TokenRestrictedSids = 11,
            /// <summary>
            /// Terminal Services session identifier (DWORD value).
            /// </summary>
            TokenSessionId = 12,
            /// <summary>
            /// User SID, the group accounts, the restricted security identifiers (SIDs), and the authentication ID.
            /// </summary>
            TokenGroupsAndPrivileges = 13,
            /// <summary>
            /// Reserved.
            /// </summary>
            TokenSessionReference = 14,
            /// <summary>
            /// Nonzero (DWORD value), if the token includes the SANDBOX_INERT flag.
            /// </summary>
            TokenSandBoxInert = 15,
            /// <summary>
            /// Reserved.
            /// </summary>
            TokenAuditPolicy = 16,
            /// <summary>
            /// Origin of the token.
            /// </summary>
            TokenOrigin = 17,
            /// <summary>
            /// Elevation level of the token.
            /// </summary>
            TokenElevationType = 18,
            /// <summary>
            /// A handle to another token, linked to this token.
            /// </summary>
            TokenLinkedToken = 19,
            /// <summary>
            /// Whether the token is elevated, or not.
            /// </summary>
            TokenElevation = 20,
            /// <summary>
            /// Nonzero (DWORD value), if virtualization is allowed for the token.
            /// </summary>
            TokenHasRestrictions = 21,
            /// <summary>
            /// Security information is contained in the token.
            /// </summary>
            TokenAccessInformation = 22,
            /// <summary>
            /// Nonzero (DWORD value), if virtualization is allowed for the token.
            /// </summary>
            TokenVirtualizationAllowed = 23,
            /// <summary>
            /// Nonzero (DWORD value), if virtualization is enabled for the token.
            /// </summary>
            TokenVirtualizationEnabled = 24,
            /// <summary>
            /// Integrity level of the token.
            /// </summary>
            TokenIntegrityLevel = 25,
            /// <summary>
            /// Nonzero (DWORD value), if the token has the UIAccess flag set.
            /// </summary>
            TokenUIAccess = 26,
            /// <summary>
            /// Mandatory integrity level of the token.
            /// </summary>
            TokenMandatoryPolicy = 27,
            /// <summary>
            /// Logon security identifier (SID) of the token.
            /// </summary>
            TokenLogonSid = 28,
            /// <summary>
            /// The maximum value for this enumeration.
            /// </summary>
            MaxTokenInfoClass = 29  // MaxTokenInfoClass should always be the last enum
        }

        public struct TOKEN_ELEVATION
        {
            public UInt32 TokenIsElevated;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle,
                                            UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetTokenInformation(
            IntPtr TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass,
            IntPtr TokenInformation,
            uint TokenInformationLength,
            out uint ReturnLength);

        /// <summary>
        /// Loads a library by given name.
        /// </summary>
        /// <param name="lpFileName">The name of the library to load.</param>
        /// <returns>A handle to the library, if successfully loaded.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = false)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// Retrieves the address of an exported function or variable from the specified dynamic link library (DLL).
        /// </summary>
        /// <param name="hmodule">A handle to the DLL module that contains the function or variable.</param>
        /// <param name="procName">The function or variable name, or the function's ordinal value.</param>
        /// <returns>The address of the exported function or variable.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern IntPtr GetProcAddress(IntPtr hmodule, string procName);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hObject">A valid handle to an open object.</param>
        /// <returns>The return value is nonzero, if the function succeeds.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// Opens up file access for Everyone at FullAccess.
        /// </summary>
        public static void UpdateFileSecurity(string filePath)
        {
#if !__MOBILE__ && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0
            if (!IsReallyVista() || !IsElevated())
        {
            return;
        }
        SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
        NTAccount act = (NTAccount)sid.Translate(typeof(NTAccount));
        FileSecurity sec = File.GetAccessControl(filePath);
        FileSystemAccessRule fsar = new FileSystemAccessRule(act, FileSystemRights.FullControl, AccessControlType.Allow);
        sec.AddAccessRule(fsar);
        File.SetAccessControl(filePath, sec);
#endif
        }

        private static bool IsReallyVista()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return false;
            }
            IntPtr hmodule = LoadLibrary("kernel32");

            if (hmodule.ToInt32() != 0)
            {
                //just any old API function that happens only to exist on Vista and higher
                IntPtr hProc = GetProcAddress(hmodule, "CreateThreadpoolWait");
                if (hProc.ToInt32() != 0)
                {
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        ///The possible values are:
        ///TRUE - the current process is elevated.
        ///	This value indicates that either UAC is enabled, and the process was elevated by
        ///	the administrator, or that UAC is disabled and the process was started by a user
        ///	who is a member of the Administrators group.
        ///FALSE - the current process is not elevated (limited).
        ///	This value indicates that either UAC is enabled, and the process was started normally,
        ///	without the elevation, or that UAC is disabled and the process was started by a standard user.
        /// </summary>
        /// <returns>Bool indicating whether the current process is elevated</returns>
        private static bool IsElevated() //= NULL )
        {
            if (!IsReallyVista())
            {
                throw new Exception("Function requires Vista or higher");
            }

            bool retVal = false;
            IntPtr hToken = IntPtr.Zero;
            IntPtr hProcess = GetCurrentProcess();

            if (hProcess == IntPtr.Zero)
            {
                throw new Exception("Error getting current process handle");
            }

            retVal = OpenProcessToken(hProcess, TOKEN_QUERY, out hToken);


            if (!retVal)
            {
                throw new Exception("Error opening process token");
            }
            try
            {

                TOKEN_ELEVATION te;
                te.TokenIsElevated = 0;

                UInt32 dwReturnLength = 0;
                Int32 teSize = System.Runtime.InteropServices.Marshal.SizeOf(te);
                IntPtr tePtr = Marshal.AllocHGlobal(teSize);
                try
                {
                    System.Runtime.InteropServices.Marshal.StructureToPtr(te, tePtr, true);

                    retVal = GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenElevation, tePtr, (UInt32)teSize, out dwReturnLength);

                    if ((!retVal) | (teSize != dwReturnLength))
                    {
                        throw new Exception("Error getting token information");
                    }

                    te = (TOKEN_ELEVATION)Marshal.PtrToStructure(tePtr, typeof(TOKEN_ELEVATION));

                }
                finally
                {
                    Marshal.FreeHGlobal(tePtr);
                }

                return (te.TokenIsElevated != 0);

            }
            finally
            {
                CloseHandle(hToken);
            }
        }
    }

    class GXLogWriter
    {
        static public string LogPath
        {
            get
            {
                string path = string.Empty;
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    path += "~";
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
                path = System.IO.Path.Combine(path, "Gurux");
                path = System.IO.Path.Combine(path, "GXDLMSDirector");
                path = System.IO.Path.Combine(path, "GXDLMSDirector.log");
                return path;
            }
        }

        /// <summary>
        /// Append data to log file.
        /// </summary>
        static public void WriteLog(string data)
        {
            if (data == null)
            {
                return;
            }
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToLongTimeString() + " " + data.Replace("\r", "<CR>").Replace("\n", "<LF>"));
        }

        static public void WriteLog(string text, byte[] value)
        {
            string str = DateTime.Now.ToLongTimeString() + " " + text;
            if (value != null)
            {
                str += "\r\n" + BitConverter.ToString(value).Replace('-', ' ');
            }
            System.Diagnostics.Trace.WriteLine(str);
        }

        /// <summary>
        /// Clear log file.
        /// </summary>
        static public void ClearLog()
        {
            foreach (TraceListener it in System.Diagnostics.Trace.Listeners)
            {
                if (it is TextWriterTraceListener)
                {
                    //Flush and close the output.
                    Trace.Flush();
                    it.Flush();
                    if (((TextWriterTraceListener)it).Writer != null)
                    {
                        ((TextWriterTraceListener)it).Writer.Close();
                    }
                    ((TextWriterTraceListener)it).Writer = new StreamWriter(GXLogWriter.LogPath);
                    GXFileInfo.UpdateFileSecurity(GXLogWriter.LogPath);
                    break;
                }
            }
            Debug.WriteLine("Log created " + DateTime.Now.ToLongTimeString());
        }
    }
}
