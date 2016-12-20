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

namespace Gurux.DLMS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Gurux.DLMS.Internal;
    using Gurux.DLMS.Enums;

    /// <summary>
    /// This class is used to build custom DLMS content.
    /// </summary>
    public class GXDLMSBuilder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        private GXDLMSBuilder()
        {
        }

        /// <summary>
        /// Set item count.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="buff"></param>
        internal static void SetObjectCount(int count, GXByteBuffer data)
        {
            GXCommon.SetObjectCount(count, data);
        }

        /// <summary>
        /// Get object count. If first byte is 0x80 or higger it will tell bytes count.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <returns>Object count.</returns>
        public static int GetObjectCount(GXByteBuffer data)
        {
            return GXCommon.GetObjectCount(data);
        }

        ///<summary>
        ///Convert object to DLMS bytes.
        ///</summary>
        ///<param name="buff">
        ///Byte buffer where data is write.
        ///</param>
        ///<param name="dataType">
        ///Data type.
        ///</param>
        ///<param name="value">
        /// Added Value.
        ///</param>
        public static void SetData(GXByteBuffer buff, DataType type, object value)
        {
            GXCommon.SetData(null, buff, type, value);
        }
    }
}
