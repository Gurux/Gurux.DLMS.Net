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

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// Authentication enumerates the authentication levels.
    /// </summary>
    public enum Authentication
    {
        /// <summary>
        /// No authentication is used.
        /// India DLMS standard IS 15959 uses name "Public client".
        /// </summary>
        None,
        /// <summary>
        /// Low authentication is used.
        /// India DLMS standard IS 15959 uses name "Meter reading".
        /// </summary>
        Low,
        /// <summary>
        /// High authentication is used.  
        /// Because DLMS/COSEM specification does not 
        /// specify details of the HLS mechanism Indian standard is implemented.
        /// </summary>
        High,
        /// <summary>
        /// High authentication is used. Password is hashed with MD5.
        /// </summary>
        HighMD5,
        /// <summary>
        /// High authentication is used. Password is hashed with SHA1.
        /// </summary>
        HighSHA1,
        /// <summary>
        /// High authentication is used. Password is hashed with GMAC.
        /// </summary>
        HighGMAC,
        /// <summary>
        /// High authentication is used. Password is hashed with SHA-256.
        /// </summary>
        HighSHA256,
        /// <summary>
        /// High authentication is used. Password is hashed with ECDSA.
        /// </summary>
        HighECDSA
    }
}
