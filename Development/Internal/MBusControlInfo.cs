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


namespace Gurux.DLMS.Internal
{
    /// <summary>
    /// M-Bus control info.
    /// </summary>
    public enum MBusControlInfo
    {
        /// <summary>
        /// Long M-Bus data header present, direction master to slave
        /// </summary>
        LongHeaderMaster = 0x60,
        /// <summary>
        /// Short M-Bus data header present, direction master to slave
        /// </summary>
        ShortHeaderMaster = 0x61,
        /// <summary>
        /// Long M-Bus data header present, direction slave to master
        /// </summary>
        LongHeaderSlave = 0x7C,
        /// <summary>
        /// Short M-Bus data header present, direction slave to master
        /// </summary>
        ShortHeaderSlave = 0x7D,
        /// <summary>
        /// M-Bus short Header.
        /// </summary>
        ShortHeader = 0x7A,
        /// <summary>
        /// M-Bus long Header.
        /// </summary>
        LongHeader = 0x72
    }
}
