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

namespace Gurux.DLMS.Objects
{
    public interface IGXDLMSBase
    {
        /// <summary>
        /// Returns collection of attributes to read.
        /// </summary>
        /// <remarks>
        /// If attribute is static and already read or device is returned HW error it is not returned.
        /// </remarks>
        /// <returns>Collection of attributes to read.</returns>                
        int[] GetAttributeIndexToRead();

        /// <summary>
        /// Returns amount of attributes.
        /// </summary>
        /// <returns>Count of attributes.</returns>
        int GetAttributeCount();

        /// <summary>
        /// Returns amount of methods.
        /// </summary>
        /// <returns></returns>
        int GetMethodCount();

        DataType GetDataType(int index);

        /// <summary>
        /// Returns value of given attribute.
        /// </summary>
        /// <remarks>
        /// When raw parameter us not used example register multiplies value by scalar.
        /// </remarks>
        /// <param name="index">Attribute index</param>
        /// <returns>Value of the attribute index.</returns>
        Object GetValue(int index, int selector, object parameters);

        /// <summary>
        /// Set value of given attribute.
        /// </summary>
        /// <remarks>
        /// When raw parameter us not used example register multiplies value by scalar.
        /// </remarks>
        /// <param name="index">Attribute index</param>
        /// <param name="value">Value of the attribute index.</param>
        void SetValue(int index, Object value);

        /// <summary>
        /// Invokes method.
        /// </summary>
        /// <param name="index">Method index.</param>
        byte[][] Invoke(object sender, int index, Object parameters);
    }
}
