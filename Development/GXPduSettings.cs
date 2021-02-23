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
    /// PDU settings contains settings for PDU without framing.
    /// </summary>
    public class GXPduSettings
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXPduSettings()
        {
            WaitTime = 100;
        }

        /// <summary>
        /// Time to wait in ms for PDU to fully received.
        /// </summary>
        /// <remarks>
        /// Because there is no end of the packet or data length app must wait given time to expect PDU to fully received.
        /// </remarks>
        public UInt16 WaitTime
        {
            get;
            set;
        }
    }
}