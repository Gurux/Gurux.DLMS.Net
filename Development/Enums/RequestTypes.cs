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

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// RequestTypes enumerates the replies of the server to a client's request,
    /// indicating the request type.
    /// </summary>
    [Flags]
    public enum RequestTypes : int
    {
        /// <summary>
        /// No more data is available for the request.
        /// </summary>
        None = 0,
        /// <summary>
        /// More data blocks are available for the request.
        /// </summary>
        DataBlock = 1,
        /// <summary>
        /// More data frames are available for the request.
        /// </summary>
        Frame = 2,
        /// <summary>
        /// More data is available for the General Block Transfer.
        /// </summary>
        GBT = 4
    }
}