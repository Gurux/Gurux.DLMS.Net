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
    /// <summary>
    /// Network parameters for the LTE network
    /// </summary>
    public class GXLteNetworkParameters
    {
        /// <summary>
        /// T3402 timer in seconds.
        /// </summary>
        public UInt16 T3402
        {
            get;
            set;
        }

        /// <summary>
        /// T3412 timer in seconds.
        /// </summary>
        public UInt16 T3412
        {
            get;
            set;
        }

        /// <summary>
        /// T3412ext2 timer in seconds.
        /// </summary>
        public UInt32 T3412ext2
        {
            get;
            set;
        }

        /// <summary>
        /// Power saving mode active timer timer in 0,01 seconds.
        /// </summary>
        public UInt16 T3324
        {
            get;
            set;
        }
        /// <summary>
        /// Extended idle mode DRX cycle timer in 0,01 seconds.
        /// </summary>
        public UInt32 TeDRX
        {
            get;
            set;
        }

        /// <summary>
        /// DRX paging time window timer in seconds.
        /// </summary>
        public UInt16 TPTW
        {
            get;
            set;
        }

        /// <summary>
        /// The minimum required Rx level in the cell in dBm.
        /// </summary>
        public sbyte QRxlevMin
        {
            get;
            set;
        }

        /// <summary>
        /// The minimum required Rx level in enhanced coverage CE Mode A.
        /// </summary>
        public sbyte QRxlevMinCE
        {
            get;
            set;
        }

        /// <summary>
        /// The minimum required Rx level in enhanced coverage CE Mode B.
        /// </summary>
        public sbyte QRxLevMinCE1
        {
            get;
            set;
        }
    }

}
