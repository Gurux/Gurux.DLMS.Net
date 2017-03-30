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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Contains the configuration to be used for both routers and hosts to support the Neighbor Discovery protocol for IPv6.
    /// </summary>
    public class GXNeighborDiscoverySetup
    {
        public GXNeighborDiscoverySetup()
        {
            MaxRetry = 3;
            RetryWaitTime = 10000;
        }

        /// <summary>
        /// Gives the maximum number of router solicitation retries to be performed by a node if the expected router advertisement has not been received. 
        /// </summary>
        [DefaultValue(3)]
        public byte MaxRetry
        {
            get;
            set;
        }

        /// <summary>
        /// Gives the waiting time in milliseconds between two successive router solicitation retries.
        /// </summary>
        [DefaultValue(10000)]
        public UInt16 RetryWaitTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gives the router advertisement transmission period in seconds.
        /// </summary>
        public UInt32 SendPeriod
        {
            get;
            set;
        }
    }
}
