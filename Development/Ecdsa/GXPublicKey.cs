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
using Gurux.DLMS.Ecdsa.Enums;
using System;

namespace Gurux.DLMS.Ecdsa
{
    /// <summary>
    /// Private key.
    /// </summary>
    public class GXPublicKey
    {
        /// <summary>
        /// Used scheme.
        /// </summary>
        public Ecc Scheme
        {
            get;
            private set;
        }

        /// <summary>
        /// Private key raw value.
        /// </summary>
        public byte[] RawValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Create the public key from raw bytes.
        /// </summary>
        /// <param name="key">Raw data</param>
        /// <returns>Public key.</returns>
        public static GXPublicKey FromRawBytes(byte[] key)
        {
            GXPublicKey value = new GXPublicKey();
            if (key.Length == 65)
            {
                value.Scheme = Ecc.P256;
                value.RawValue = key;
            }
            else if (key.Length == 97)
            {
                value.Scheme = Ecc.P384;
                value.RawValue = key;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid public key.");
            }
            return value;
        }

        /// <summary>
        /// Returns the public key as a hex string.
        /// </summary>
        /// <returns></returns>
        public string ToHex()
        {
            return GXDLMSTranslator.ToHex(RawValue);
        }
    }
}
