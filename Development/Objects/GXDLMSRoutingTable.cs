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
    /// G3-PLC 6LoWPAN routing table.
    /// </summary>
    public class GXDLMSRoutingTable
    {
        /// <summary>
        /// Address of the destination.
        /// </summary>
        public UInt16 DestinationAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Address of the next hop on the route towards the destination.
        /// </summary>
        public UInt16 NextHopAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Cumulative link cost along the route towards the destination.
        /// </summary>
        public UInt16 RouteCost
        {
            get;
            set;
        }

        /// <summary>
        /// Number of hops of the selected route to the destination.
        /// </summary>
        public byte HopCount
        {
            get;
            set;
        }

        /// <summary>
        /// Number of weak links to destination.
        /// </summary>
        public byte WeakLinkCount
        {
            get;
            set;
        }

        /// <summary>
        /// Remaining time in minutes until when this entry in the routing table is considered valid.
        /// </summary>
        public UInt16 ValidTime
        {
            get;
            set;
        }
    }
}
