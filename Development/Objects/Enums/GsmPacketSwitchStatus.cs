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

namespace Gurux.DLMS.Objects.Enums
{
    /// <summary>
    /// Packet switched status of the modem.
    /// </summary>
    public enum GsmPacketSwitchStatus
    {
        ///<summary>
        ///Inactive
        ///</summary>
        Inactive,
        ///<summary>
        ///GPRS
        ///</summary>
        GPRS,
        ///<summary>
        ///EDGE
        ///</summary>
        EDGE,
        ///<summary>
        ///UMTS
        ///</summary>
        UMTS,
        ///<summary>
        ///HSDPA
        ///</summary>
        HSDPA,
        /// <summary>
        /// LTE
        /// </summary>
        LTE,
        /// <summary>
        /// CDMA
        /// </summary>
        CDMA,
        /// <summary>
        /// LTE Cat M1.
        /// </summary>
        LteCatM1,
        /// <summary>
        /// LTE Cat NB1.
        /// </summary>
        LteCatNb1,
        /// <summary>
        /// LTE Cat NB2.
        /// </summary>
        LteCatNb2,
    }
}
