//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Filename:    $HeadURL$
//
// Version:     $Revision$,
//      $Date$
//      $Author$
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
    /// This class is used to count repetition delay for the next push message.
    /// </summary>
    public class GXRepetitionDelay
    {
        /// <summary>
        /// The minimum delay until a next push attempt is started in seconds.
        /// </summary>
        public UInt16 Min
        {
            get;
            set;
        }

        /// <summary>
        /// Calculating the next delay.
        /// </summary>
        public UInt16 Exponent
        {
            get;
            set;
        }
        /// <summary>
        /// The maximum delay until a next push attempt is started in seconds.
        /// </summary>
        public UInt16 Max
        {
            get;
            set;
        }
    }
}
