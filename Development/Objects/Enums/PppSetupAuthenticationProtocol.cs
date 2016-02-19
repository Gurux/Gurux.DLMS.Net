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

namespace Gurux.DLMS.Objects.Enums
{
    /// <summary>
    /// The value of the Auth-Prot (Authentication Protocol) element indicates
    /// the authentication protocol used on the given PPP link.
    /// </summary>
    public enum PppSetupAuthenticationProtocol
    {
        /// <summary>
        /// No authentication protocol is used.
        /// </summary>
        None = 0,
        /// <summary>
        /// The PAP protocol is used.
        /// </summary>
        PAP = 0xc023,
        /// <summary>
        /// The CHAP protocol is used.
        /// </summary>
        CHAP = 0xc223,
        /// <summary>
        /// The EAP protocol is used.
        /// </summary>
        EAP = 0xc227
    }
}
