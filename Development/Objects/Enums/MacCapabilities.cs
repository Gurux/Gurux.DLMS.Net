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

namespace Gurux.DLMS.Objects.Enums
{
    /// <summary>
    /// Present functional state of the node.
    /// </summary>
    [Flags]
    public enum MacCapabilities : UInt16
    {
        /// <summary>
        /// Switch capable.
        /// </summary>
        SwitchCapable = 1,
        /// <summary>
        /// Packet aggregation.
        /// </summary>
        PacketAggregation = 2,
        /// <summary>
        /// Contention free period.
        /// </summary>
        ContentionFreePeriod = 4,
        /// <summary>
        /// Direct connection.
        /// </summary>
        DirectConnection = 8,
        /// <summary>
        /// Multicast.
        /// </summary>
        Multicast = 0x10,
        /// <summary>
        ///  PHY Robustness Management.
        /// </summary>
        PhyRobustnessManagement = 0x20,
        /// <summary>
        /// ARQ.
        /// </summary>
        Arq = 0x40,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        ReservedForFutureUse = 0x80,
        /// <summary>
        ///  Direct Connection Switching.
        /// </summary>
        DirectConnectionSwitching = 0x100,
        /// <summary>
        /// Multicast Switching Capability.
        /// </summary>
        MulticastSwitchingCapability = 0x200,
        /// <summary>
        /// PHY Robustness Management Switching Capability.
        /// </summary>
        PhyRobustnessManagementSwitchingCapability = 0x400,
        /// <summary>
        /// ARQ Buffering Switching Capability.
        /// </summary>
        ArqBufferingSwitchingCapability = 0x800
    }
}
