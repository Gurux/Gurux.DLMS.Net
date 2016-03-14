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

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// SourceDiagnostic enumerates the error codes for reasons that can cause the server to reject the client.
    /// </summary>
    public enum SourceDiagnostic
    {
        /// <summary>
        /// OK.
        /// </summary>
        None = 0,
        /// <summary>
        /// No reason is given.
        /// </summary>
        NoReasonGiven = 1,
        /// <summary>
        /// The application context name is not supported. 
        /// </summary>
        ApplicationContextNameNotSupported = 2,
        /// <summary>
        /// The authentication mechanism name is not recognized.
        /// </summary>
        AuthenticationMechanismNameNotRecognised = 11,
        /// <summary>
        /// Authentication mechanism name is required.
        /// </summary>
        AuthenticationMechanismNameReguired = 12,
        /// <summary>
        /// Authentication failure.
        /// </summary>
        AuthenticationFailure = 13,
        /// <summary>
        /// Authentication is required.
        /// </summary>
        AuthenticationRequired = 14
    }
}
