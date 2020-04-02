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
    /// MAC direct table element.
    /// </summary>
    public class GXMacDirectTable
    {
        /// <summary>
        /// SID of switch through which the source service node is connected.
        /// </summary>
        public Int16 SourceSId
        {
            get;
            set;
        }

        /// <summary>
        ///NID allocated to the source service node.
        /// </summary>
        public Int16 SourceLnId
        {
            get;
            set;
        }

        /// <summary>
        /// LCID allocated to this connection at the source.
        /// </summary>
        public Int16 SourceLcId
        {
            get;
            set;
        }

        /// <summary>
        /// SID of the switch through which the destination service node is connected.
        /// </summary>
        public Int16 DestinationSId
        {
            get;
            set;
        }

        /// <summary>
        /// NID allocated to the destination service node.
        /// </summary>
        public Int16 DestinationLnId
        {
            get;
            set;
        }

        /// <summary>
        /// LCID allocated to this connection at the destination.
        /// </summary>
        public Int16 DestinationLcId
        {
            get;
            set;
        }

        /// <summary>
        /// Entry DID is the EUI-48 of the direct switch
        /// </summary>
        public byte[] Did
        {
            get;
            set;
        }
    }
}
