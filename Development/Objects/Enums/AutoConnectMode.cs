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

namespace Gurux.DLMS.Objects.Enums
{
    /// <summary>
    /// Defines the mode controlling the auto dial functionality concerning the timing.
    /// </summary>
    public enum AutoConnectMode
    {
        /// <summary>
        /// No auto dialling,
        /// </summary>
        NoAutoDialling,
        /// <summary>
        /// Auto dialling allowed anytime,
        /// </summary>
        AutoDiallingAllowedAnytime,
        /// <summary>
        /// Auto dialling allowed within the validity time of the calling window.
        /// </summary>
        AutoDiallingAllowedCallingWindow,
        /// <summary>
        /// “regular” auto dialling allowed within the validity time
        /// of the calling window; “alarm” initiated auto dialling allowed anytime,
        /// </summary>        
        RegularAutoDiallingAllowedCallingWindow,
        /// <summary>
        /// SMS sending via Public Land Mobile Network (PLMN),
        /// </summary>
        SmsSendingPlmn,
        /// <summary>
        /// SMS sending via PSTN.
        /// </summary>
        SmsSendingPstn,
        /// <summary>
        /// Email sending.
        /// </summary>
        EmailSending
        //(200..255) manufacturer specific modes
    }
}