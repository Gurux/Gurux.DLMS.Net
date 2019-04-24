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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Internal
{
    /// <summary>
    /// APDU types.
    /// </summary>
    enum PduType
    {
        /// <summary>
        /// IMPLICIT BIT STRING {version1 (0)} DEFAULT {version1}
        /// </summary>
        ProtocolVersion = 0,
        /// <summary>
        /// Application-context-name
        /// </summary>
        ApplicationContextName,
        /// <summary>
        /// AP-title OPTIONAL
        /// </summary>
        CalledApTitle,
        /// <summary>
        /// AE-qualifier OPTIONAL.
        /// </summary>
        CalledAeQualifier,
        /// <summary>
        /// AP-invocation-identifier OPTIONAL.
        /// </summary>
        CalledApInvocationId,
        /// <summary>
        /// AE-invocation-identifier OPTIONAL
        /// </summary>
        CalledAeInvocationId,
        /// <summary>
        /// AP-title OPTIONAL
        /// </summary>
        CallingApTitle,
        /// <summary>
        /// AE-qualifier OPTIONAL
        /// </summary>
        CallingAeQualifier,
        /// <summary>
        /// AP-invocation-identifier OPTIONAL
        /// </summary>
        CallingApInvocationId,
        /// <summary>
        /// AE-invocation-identifier OPTIONAL
        /// </summary>
        CallingAeInvocationId,
        /// <summary>
        /// The following field shall not be present if only the kernel is used.
        /// </summary>
        SenderAcseRequirements,
        /// <summary>
        /// The following field shall only be present if the authentication functional unit is selected.     
        /// </summary>
        MechanismName = 11,
        /// <summary>
        /// The following field shall only be present if the authentication functional unit is selected.
        /// </summary>
        CallingAuthenticationValue = 12,
        /// <summary>
        /// Implementation-data.
        /// </summary>
        ImplementationInformation = 29,
        /// <summary>
        /// Association-information OPTIONAL 
        /// </summary>
        UserInformation = 30
    }
}
