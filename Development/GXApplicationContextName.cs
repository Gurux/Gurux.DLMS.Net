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
        /// Code application context name.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data">Byte buffer where data is saved.</param>
        /// <param name="ciphered">Is chiohering used.</param>
        internal static void CodeData(GXDLMSSettings settings, GXByteBuffer data, bool ciphered)
        {
            //Application context name tag
            data.SetUInt8(0xA1);
            //Len
            data.SetUInt8(0x09);
            data.SetUInt8(0x06);
            data.SetUInt8(0x07);
            if (settings.UseLogicalNameReferencing)
            {
                if (ciphered)
                {
                    data.Set(GXCommon.LogicalNameObjectIdWithCiphering);

                }
                else
                {
                    data.Set(GXCommon.LogicalNameObjectID);
                }
            }
            else
            {
                if (ciphered)
                {
                    data.Set(GXCommon.ShortNameObjectIdWithCiphering);
                }
                else
                {
                    data.Set(GXCommon.ShortNameObjectID);
                }
            }
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="buff"></param>
        internal static bool EncodeData(GXDLMSSettings settings, GXByteBuffer buff)
        {
            int tag = buff.GetUInt8();
            if (tag != 0xA1)
            {
                throw new Exception("Invalid tag.");
            }
            //Get length.
            int len = buff.GetUInt8();
            if (buff.Size - buff.Position < len)
            {
                throw new Exception("Encoding failed. Not enough data.");
            }
            if (buff.GetUInt8() != 0x6)
            {
                throw new Exception("Encoding failed. Not an Object ID.");
            }
            //Object ID length.
            len = buff.GetUInt8();
            if (settings.UseLogicalNameReferencing)
            {
                if (buff.Compare(GXCommon.LogicalNameObjectID))
                {
                    return true;
                }
                // If ciphering is used.
                return buff.Compare(GXCommon.LogicalNameObjectIdWithCiphering);
            }
            if (buff.Compare(GXCommon.ShortNameObjectID))
            {
                return true;
            }
            // If ciphering is used.
            return buff.Compare(GXCommon.ShortNameObjectIdWithCiphering);
        }
    }
}
