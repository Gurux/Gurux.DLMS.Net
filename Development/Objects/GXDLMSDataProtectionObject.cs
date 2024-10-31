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
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSDataProtectionObject
    {
        public GXDLMSObject Target
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute Index of the protection object.
        /// </summary>
        public sbyte AttributeIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Data index of the protection object. 
        /// </summary>
        public UInt16 DataIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Restriction type. 
        /// </summary>
        public RestrictionType RestrictionType
        {
            get;
            set;
        }

        /// <summary>
        /// Restriction start date.
        /// </summary>
        /// <seealso cref="RestrictionType"/>
        public GXDateTime RestrictionStartDate
        {
            get;
            set;
        }

        /// <summary>
        /// Restriction end date.
        /// </summary>
        /// <seealso cref="RestrictionType"/>
        public GXDateTime RestrictionEndDate
        {
            get;
            set;
        }

        /// <summary>
        /// Restriction start entry.
        /// </summary>
        /// <seealso cref="RestrictionType"/>
        public UInt32 RestrictionStartEntry
        {
            get;
            set;
        }

        /// <summary>
        /// Restriction end entry.
        /// </summary>
        /// <seealso cref="RestrictionType"/>
        public UInt32 RestrictionEndEntry
        {
            get;
            set;
        }
    }
}
