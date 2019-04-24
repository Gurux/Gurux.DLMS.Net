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

namespace Gurux.DLMS.Objects
{
    public class GXDLMSGSMCellInfo
    {
        /// <summary>
        /// Four byte cell ID.
        /// </summary>
        public UInt32 CellId
        {
            get;
            set;
        }

        /// <summary>
        ///  Two byte location area code (LAC).
        /// </summary>
        public UInt16 LocationId
        {
            get;
            set;
        }

        /// <summary>
        ///  Signal quality.
        /// </summary>
        public byte SignalQuality
        {
            get;
            set;
        }

        /// <summary>
        /// Bit Error Rate.
        /// </summary>
        public byte Ber
        {
            get;
            set;
        }

        /// <summary>
        /// Mobile Country Code.
        /// </summary>
        public UInt16 MobileCountryCode
        {
            get;
            set;
        }

        /// <summary>
        ///  Mobile Network Code.
        /// </summary>
        public UInt16 MobileNetworkCode
        {
            get;
            set;
        }

        /// <summary>
        ///  Absolute radio frequency channel number.
        /// </summary>
        public UInt32 ChannelNumber
        {
            get;
            set;
        }

    }
}
