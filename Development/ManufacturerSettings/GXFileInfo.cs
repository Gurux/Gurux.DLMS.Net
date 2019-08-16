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

using System.Security.Principal;
#if !WINDOWS_UWP
using System.Security.AccessControl;
#endif
using System.IO;

namespace Gurux.DLMS.ManufacturerSettings
{
    class GXFileInfo
    {
        /// <summary>
        /// Opens up directory access for Everyone at FullAccess.
        /// </summary>
        /// <param name="path">Directory to updated.</param>
        public static void UpdateDirectorySecurity(string path)
        {
#if !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0
            DirectoryInfo dInfo = new DirectoryInfo(path);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                    FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
#endif //WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0
        }

        /// <summary>
        /// Opens up file access for Everyone at FullAccess.
        /// </summary>
        /// <param name="filePath">File path.</param>
        public static void UpdateFileSecurity(string filePath)
        {
#if !__MOBILE__ && !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0
            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            FileInfo fInfo = new FileInfo(filePath);
            FileSecurity fSecurity = File.GetAccessControl(filePath);
            fSecurity.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl,
                                    InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            fInfo.SetAccessControl(fSecurity);
#endif //!__MOBILE__ && !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0
        }
    }
}
