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
using System.ComponentModel;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMSLimits contains commands for retrieving and setting the limits of 
    /// field length and window size, when communicating with the server. 
    /// </summary>
    public class GXDLMSLimits
    {
        internal GXDLMSLimits()
        {
            MaxInfoTX = (byte)128;
            MaxInfoRX = (byte)62;
            WindowSizeRX = WindowSizeTX = (byte) 1;            
        }

        /// <summary>
        /// The maximum information field length in transmit.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 128.
        /// </remarks>
        public object MaxInfoTX
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum information field length in receive.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 62.
        /// </remarks>
        public object MaxInfoRX
        {
            get;
            set;
        }

        /// <summary>
        /// The window size in transmit.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 1.
        /// </remarks>
        public object WindowSizeTX
        {
            get;
            set;
        }

        /// <summary>
        /// The window size in receive.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 1.
        /// </remarks>
        public object WindowSizeRX
        {
            get;
            set;
        }
    }
}