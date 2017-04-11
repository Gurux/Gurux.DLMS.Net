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
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSContextInformationTable
    {
        /// <summary>
        /// Corresponds to the 4-bit context information used for source and destination addresses (SCI, DCI).
        /// </summary>
        public string CID
        {
            get;
            set;
        }

        /// <summary>
        /// Context.
        /// </summary>
        public byte[] Context
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates if the context is valid for use in compression.
        /// </summary>
        public bool Compression
        {
            get;
            set;
        }

        /// <summary>
        /// Remaining time in minutes during which the context information table is considered valid. It is updated upon reception of the advertised context. 
        /// </summary>
        public UInt16 ValidLifetime
        {
            get;
            set;
        }
    }
}
