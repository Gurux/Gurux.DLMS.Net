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

namespace Gurux.DLMS
{
    ///<summary>
    ///DataType enumerates usable types of data in GuruxDLMS.
    ///</summary>
    public enum DataType : byte
    {
        ///<summary>
        ///Data type is Array.
        ///</summary>
        Array = 1,

        ///<summary>
        ///Data type is Binary coded decimal.
        ///</summary>
        BinaryCodedDesimal = 13,

        ///<summary>
        ///Data type is Bit string.
        ///</summary>
        BitString = 4,

        ///<summary>
        ///Data type is Boolean.
        ///</summary>
        Boolean = 3,

        ///<summary>
        ///Data type is Compact array.
        ///</summary>
        CompactArray = 0x13,

        ///<summary>
        ///Data type is Date.
        ///</summary>
        Date = 0x1a,

        ///<summary>
        ///Data type is DateTime.
        ///</summary>
        DateTime = 0x19,

        ///<summary>
        ///Data type is Enum.
        ///</summary>
        Enum = 0x16,

        ///<summary>
        ///Data type is Float32.
        ///</summary>
        Float32 = 0x17,

        ///<summary>
        ///Data type is Float64.
        ///</summary>
        Float64 = 0x18,

        ///<summary>
        ///Data type is Int16.
        ///</summary>
        Int16 = 0x10,

        ///<summary>
        ///Data type is Int32.
        ///</summary>
        Int32 = 5,

        ///<summary>
        ///Data type is Int64.
        ///</summary>
        Int64 = 20,

        ///<summary>
        ///Data type is Int8.
        ///</summary>
        Int8 = 15,

        ///<summary>
        ///By default, no data type is set.
        ///</summary>
        None = 0,

        ///<summary>
        ///Data type is Octet string.
        ///</summary>
        OctetString = 9,

        ///<summary>
        ///Data type is String.
        ///</summary>
        String = 10,

        ///<summary>
        ///Data type is Structure.
        ///</summary>
        Structure = 2,

        ///<summary>
        ///Data type is Time.
        ///</summary>
        Time = 0x1b,

        ///<summary>
        ///Data type is UInt16.
        ///</summary>
        UInt16 = 0x12,

        ///<summary>
        ///Data type is UInt32.
        ///</summary>
        UInt32 = 6,

        ///<summary>
        ///Data type is UInt64.
        ///</summary>
        UInt64 = 0x15,

        ///<summary>
        ///Data type is UInt8.
        ///</summary>
        UInt8 = 0x11
    }
}