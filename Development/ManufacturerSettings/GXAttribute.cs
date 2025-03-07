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
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.ManufacturerSettings
{
#if !WINDOWS_UWP
    [Serializable]
#endif
    public class GXDLMSAttribute : GXDLMSAttributeSettings
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttribute(int index)
            : this(index, DataType.None, 0)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttribute() :
                this(0, DataType.None, DataType.None, 0)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttribute(int index, DataType uiType) :
                this(index, DataType.None, uiType, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttribute(int index, DataType type, DataType uiType) :
                this(index, type, uiType, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttribute(int index, DataType type, DataType uiType, int order) :
                base()
        {
            Index = index;
            Type = type;
            UIType = uiType;
            Order = order;
        }
    }
}
