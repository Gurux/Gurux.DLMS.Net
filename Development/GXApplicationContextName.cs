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
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{

    /// <summary>
    /// Reserved for internal use.
    /// </summary>
    class GXApplicationContextName
    {

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="data"></param>
        internal void CodeData(List<byte> data)
        {
            //Application context name tag
            data.Add(0xA1);
            //Len
            data.Add(0x09);
            data.Add(0x06);
            data.Add(0x07);
            if (UseLN)
            {
                data.AddRange(GXCommon.LogicalNameObjectID);
            }
            else
            {
                data.AddRange(GXCommon.ShortNameObjectID);
            }
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="index"></param>
        internal void EncodeData(byte[] buff, ref int index)
        {
            int tag = buff[index++];
            if (tag != 0xA1)
            {
                throw new Exception("Invalid tag.");
            }
            //Get length.
            int len = buff[index++];
            if (buff.Length - index < len)
            {
                throw new Exception("Encoding failed. Not enough data.");
            }
            if (buff[index++] != 0x6)
            {
                throw new Exception("Encoding failed. Not an Object ID.");
            }
            //Object ID length.
            len = buff[index++];
            //Compare Object ID to check is Logical name used.
            UseLN = GXCommon.Compare(buff, ref index, GXCommon.LogicalNameObjectID);
            if (!UseLN)
            {
                index += GXCommon.ShortNameObjectID.Length;
            }
        }
        internal bool UseLN = false;
    }
}
