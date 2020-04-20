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
    /// MAC PHY communication parameters.
    /// </summary>
    public class GXMacPhyCommunication
    {
        /// <summary>
        /// EUI is the EUI-48 of the other device.
        /// </summary>
        public byte[] Eui
        {
            get;
            set;
        }

        /// <summary>
        /// The tx power of GPDU packets sent to the device
        /// </summary>
        public sbyte TxPower
        {
            get;
            set;
        }

        /// <summary>
        /// The Tx coding of GPDU packets sent to the device;
        /// </summary>
        public sbyte TxCoding
        {
            get;
            set;
        }

        /// <summary>
        /// The Rx coding of GPDU packets received from the device
        /// </summary>
        public sbyte RxCoding
        {
            get;
            set;
        }

        /// <summary>
        /// The Rx power level of GPDU packets received from the device.
        /// </summary>
        public sbyte RxLvl
        {
            get;
            set;
        }

        /// <summary>
        /// SNR of GPDU packets received from the device.
        /// </summary>
        public sbyte Snr
        {
            get;
            set;
        }

        /// <summary>
        /// The number of times the Tx power was modified.
        /// </summary>
        public sbyte TxPowerModified
        {
            get;
            set;
        }

        /// <summary>
        /// The number of times the Tx coding was modified.
        /// </summary>
        public sbyte TxCodingModified
        {
            get;
            set;
        }

        /// <summary>
        /// The number of times the Rx coding was modified.
        /// </summary>
        public sbyte RxCodingModified
        {
            get;
            set;
        }

    }
}
