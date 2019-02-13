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
using System.ComponentModel;
using System.Text;

namespace Gurux.DLMS.ManufacturerSettings
{
#if !WINDOWS_UWP
    [Serializable]
#endif
    public class GXObisValueItem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXObisValueItem()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXObisValueItem(int deviceValue, string uiValue)
        {
            Value = deviceValue;
            UIValue = uiValue;
        }

        /// <summary>
        /// Value that is read from or written to the Device.
        /// </summary>
        public int Value
        {
            get;
            set;
        }

        /// <summary>
        /// Value that is shown to the user.
        /// </summary>
        public string UIValue
        {
            get;
            set;
        }

        /// <summary>
        /// Mask size in bits. Zero if not used.
        /// </summary>
        [DefaultValue(0)]
        public int MaskSize
        {
            get;
            set;
        }

        /// <summary>
        /// Abount of bits to shift the mask.
        /// </summary>
        [DefaultValue(0)]
        public int Shift
        {
            get;
            set;
        }

        public override string ToString()
        {
            return UIValue + " (" + Value + ")";
        }
    }
}
