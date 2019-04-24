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
    /// The routing configuration element specifies all parameters linked to the routing mechanism described in ITU-T G.9903:2014.
    /// </summary>
    public class GXDLMSRoutingConfiguration
    {
        /// <summary>
        /// Maximum time that a packet is expected to take to reach any node from any node in seconds. 
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x11.
        /// </remarks>
        public byte NetTraversalTime
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum time-to-live of a routing table entry (in minutes). 
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x12.
        /// </remarks>
        public UInt16 RoutingTableEntryTtl
        {
            get;
            set;
        }

        /// <summary>
        /// A weight factor for the Robust Mode to calculate link cost. 
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x13.
        /// </remarks>
        public byte Kr
        {
            get;
            set;
        }

        /// <summary>
        /// A weight factor for modulation to calculate link cost.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x14.
        /// </remarks>
        public byte Km
        {
            get;
            set;
        }

        /// <summary>
        /// A weight factor for number of active tones to calculate link cost. 
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x15.
        /// </remarks>
        public byte Kc
        {
            get;
            set;
        }

        /// <summary>
        /// A weight factor for LQI to calculate route cost.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x16.
        /// </remarks>
        public byte Kq
        {
            get;
            set;
        }

        /// <summary>
        ///  A weight factor for hop to calculate link cost. 
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x17.
        /// </remarks>
        public byte Kh
        {
            get;
            set;
        }

        /// <summary>
        /// A weight factor for the number of active routes in the routing table to calculate link cost. 
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x1B.
        /// </remarks>
        public byte Krt
        {
            get;
            set;
        }

        /// <summary>
        /// The number of RREQ retransmission in case of RREP reception time out. 
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x18.
        /// </remarks>
        public byte RreqRetries
        {
            get;
            set;
        }

        /// <summary>
        /// The number of seconds to wait between two consecutive RREQ – RERR generations.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x19.
        /// </remarks>
        public byte RreqRerrWait
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum time-to-live of a blacklisted neighbour entry (in minutes). 
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x1F.
        /// </remarks>
        public UInt16 BlacklistTableEntryTtl
        {
            get;
            set;
        }

        /// <summary>
        ///  If TRUE, the RREQ shall be generated with its 'unicast RREQ'.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x0D.
        /// </remarks>
        public bool UnicastRreqGenEnable
        {
            get;
            set;
        }

        /// <summary>
        /// Enable the sending of RLCREQ frame by the device.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x09.
        /// </remarks>
        public bool RlcEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// It represents an additional cost to take into account a possible asymmetry in the link.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x0A.
        /// </remarks>
        public byte AddRevLinkCost
        {
            get;
            set;
        }
    }
}
