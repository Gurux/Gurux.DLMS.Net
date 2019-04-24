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


namespace Gurux.DLMS.Internal
{
    /// <summary>
    /// Encryption modes.
    /// </summary>
    public enum MBusEncryptionMode
    {
        /// <summary>
        /// Encryption is not used.
        /// </summary>
        None,
        /// <summary>
        ///  AES with Counter Mode (CTR) noPadding and IV.
        /// </summary>
        Aes128,
        /// <summary>
        ///  DES with Cipher Block Chaining Mode (CBC).
        /// </summary>
        DesCbc,
        /// <summary>
        ///  DES with Cipher Block Chaining Mode (CBC) and Initial Vector.
        /// </summary>
        DesCbcIv,
        /// <summary>
        /// AES with Cipher Block Chaining Mode (CBC) and Initial Vector.
        /// </summary>
        AesCbcIv = 5,
        /// <summary>
        /// AES 128 with Cipher Block Chaining Mode (CBC) and dynamic key and Initial Vector with 0.
        /// </summary>
        AesCbcIv0 = 7,
        /// <summary>
        /// TLS
        /// </summary>
        Tls = 13
    }
}
