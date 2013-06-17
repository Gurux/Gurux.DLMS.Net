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
        ///<remarks>
        /// DLMS/COSEM type is: array.
        ///</remarks>
        Array = 1,
        ///<summary>
        ///Data type is Binary coded decimal.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: bcd.
        ///</remarks>
        BinaryCodedDesimal = 13,
        ///<summary>
        ///Data type is Bit string.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: bit-string.
        ///</remarks>
        BitString = 4,
        ///<summary>
        ///Data type is Boolean.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: boolean.
        ///</remarks>
        Boolean = 3,
        ///<summary>
        ///Data type is Compact array.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: compact array.
        ///</remarks>
        CompactArray = 0x13,
        ///<summary>
        ///Data type is Date.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: date.
        ///</remarks>
        Date = 0x1a,

        ///<summary>
        ///Data type is DateTime.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: date_time.
        ///</remarks>
        DateTime = 0x19,

        ///<summary>
        ///Data type is Enum.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: enum.
        ///</remarks>
        Enum = 0x16,

        ///<summary>
        ///Data type is Float32.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: float 32.
        ///</remarks>
        Float32 = 0x17,
        ///<summary>
        ///Data type is Float64.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: float 64.
        ///</remarks>
        Float64 = 0x18,
        ///<summary>
        ///Data type is Int16.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: long.
        ///</remarks>
        Int16 = 0x10,
        ///<summary>
        ///Data type is Int32.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: double-long.
        ///</remarks>
        Int32 = 5,
        ///<summary>
        ///Data type is Int64.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: Integer64.
        ///</remarks>
        Int64 = 20,
        ///<summary>
        ///Data type is Int8.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: integer.
        ///</remarks>
        Int8 = 15,
        ///<summary>
        ///By default, no data type is set.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: null-data.
        ///</remarks>
        None = 0,
        ///<summary>
        ///Data type is Octet string.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: octet-string.
        ///</remarks>
        OctetString = 9,
        ///<summary>
        ///Data type is String.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: visible-string.
        ///</remarks>
        String = 10,
        ///<summary>
        ///Data type is Structure.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: structure.
        ///</remarks>
        Structure = 2,
        ///<summary>
        ///Data type is Time.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: time.
        ///</remarks>
        Time = 0x1b,
        ///<summary>
        ///Data type is UInt16.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: long-unsigned.
        ///</remarks>
        UInt16 = 0x12,
        ///<summary>
        ///Data type is UInt32.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: double-longunsigned.
        ///</remarks>
        UInt32 = 6,        
        ///<summary>
        ///Data type is UInt64.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: long64-unsigned.
        ///</remarks>
        UInt64 = 0x15,
        ///<summary>
        ///Data type is UInt8.
        ///</summary>
        ///<remarks>
        /// DLMS/COSEM type is: unsigned.
        ///</remarks>
        UInt8 = 0x11
    }
}