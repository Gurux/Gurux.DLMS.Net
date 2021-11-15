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
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{
    /// <summary>
    /// HDLC settings contains commands for retrieving and setting the limits of
    /// field length and window size, when communicating with the server.
    /// </summary>
    public class GXMBusSettings
    {
        /// <summary>
        /// Device identification number.
        /// </summary>
        public UInt32 Id
        {
            get;
            set;
        }

        /// <summary>
        /// Manufacturer Id.
        /// </summary>
        public string ManufacturerId
        {
            get;
            set;
        }

        /// <summary>
        /// Version.
        /// </summary>
        public byte Version
        {
            get;
            set;
        }

        /// <summary>
        /// Device type.
        /// </summary>
        public MBusMeterType MeterType
        {
            get;
            set;
        }
    }
}