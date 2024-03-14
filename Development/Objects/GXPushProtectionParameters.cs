﻿//
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

using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Push protection parameters.
    /// </summary>
    public class GXPushProtectionParameters
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXPushProtectionParameters()
        {
            KeyInfo = new GXDLMSDataProtectionKey();
        }

        /// <summary>
        /// Protection type.
        /// </summary>
        public ProtectionType ProtectionType
        {
            get;
            set;
        }

        /// <summary>
        /// Transaction Id.
        /// </summary>
        public byte[] TransactionId
        {
            get;
            set;
        }

        /// <summary>
        /// Originator system title.
        /// </summary>
        public byte[] OriginatorSystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Recipient system title.
        /// </summary>
        public byte[] RecipientSystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Other information.
        /// </summary>
        public byte[] OtherInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Key info.
        /// </summary>
        public GXDLMSDataProtectionKey KeyInfo
        {
            get;
            set;
        }
    }
}
