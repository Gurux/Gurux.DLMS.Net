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
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSNeighbourTable
    {
        /// <summary>
        /// MAC Short Address of the node.
        /// </summary>
        public UInt16 ShortAddress
        {
            get;
            set;
        }
        /// <summary>
        /// Is Payload Modulation scheme used.
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }
        /// <summary>
        /// Frequency sub-band can be used for communication with the device.
        /// </summary>
        public string ToneMap
        {
            get;
            set;
        }
        /// <summary>
        /// Modulation type.
        /// </summary>
        public Modulation Modulation
        {
            get;
            set;
        }
        /// <summary>
        ///  Tx Gain.
        /// </summary>
        public sbyte TxGain
        {
            get;
            set;
        }
        /// <summary>
        /// Tx Gain resolution.
        /// </summary>
        public GainResolution TxRes
        {
            get;
            set;
        }
        /// <summary>
        /// Transmitter gain for each group of tones represented by one valid bit of the tone map.
        /// </summary>
        public string TxCoeff
        {
            get;
            set;
        }

        /// <summary>
        /// Link Quality Indicator.
        /// </summary>
        public byte Lqi
        {
            get;
            set;
        }

        /// <summary>
        /// Phase difference in multiples of 60 degrees between the mains phase of the local node and the neighbour node. 
        /// </summary>
        public sbyte PhaseDifferential
        {
            get;
            set;
        }

        /// <summary>
        /// Remaining time in minutes until which the tone map response parameters in the neighbour table are considered valid.
        /// </summary>
        public byte TMRValidTime
        {
            get;
            set;
        }
        /// <summary>
        /// Remaining time in minutes until which this entry in the neighbour table is considered valid.
        /// </summary>
        public byte NeighbourValidTime
        {
            get;
            set;
        }
    }
}
