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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gurux.Common
{
    /// <summary>
    /// Trace Type enumerates where trace is sent.
    /// </summary>
    [Flags]
    public enum TraceTypes
    {
        /// <summary>
        /// Data is sent.
        /// </summary>
        [EnumMember(Value = "1")]
        Sent = 0x1,
        /// <summary>
        /// Data is received.
        /// </summary>
        [EnumMember(Value = "2")]
        Received = 0x2,
        /// <summary>
        /// Error has occurred.
        /// </summary>
        [EnumMember(Value = "4")]
        Error = 0x4,
        /// <summary>
        /// Warning.
        /// </summary>
        [EnumMember(Value = "8")]
        Warning = 0x8,
        /// <summary>
        /// Info. Example Media states are notified as info.
        /// </summary>
        [EnumMember(Value = "16")]
        Info = 0x10,
    }
}
