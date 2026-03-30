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

namespace Gurux.DLMS.Objects.Enums
{
    /// <summary>
    /// Enumerates token status codes.
    /// </summary>
    ///  <remarks>
    ///  Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTokenGateway
    /// </remarks>
    public enum TokenStatusCode : int
    {
        /// <summary>
        /// Token format result OK.
        /// </summary>
        FormatOk,
        /// <summary>
        /// Authentication result OK.
        /// </summary>
        AuthenticationOk,
        /// <summary>
        /// Validation result OK.
        /// </summary>
        ValidationOk,
        /// <summary>
        /// Token execution result OK.
        /// </summary>
        TokenExecutionOk,
        /// <summary>
        /// Token format failure.
        /// </summary>
        TokenFormatFailure,
        /// <summary>
        /// Authentication failure.
        /// </summary>
        AuthenticationFailure,
        /// <summary>
        /// Validation result failure.
        /// </summary>
        ValidationResultFailure,
        /// <summary>
        /// Token execution result failure.
        /// </summary>
        TokenExecutionResultFailure,
        /// <summary>
        /// Token received and not yet processed.
        /// </summary>
        TokenReceived
    }
}
