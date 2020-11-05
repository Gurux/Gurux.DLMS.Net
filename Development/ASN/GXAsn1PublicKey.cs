using Gurux.DLMS.Ecdsa;
using Gurux.DLMS.Internal;
using System;
using System.Security.Cryptography.X509Certificates;

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

namespace Gurux.DLMS.ASN
{

    /// <summary>
    /// ASN1 Public key.
    /// </summary>
    public class GXAsn1PublicKey
    {
        /// <summary>
        /// Public key.
        /// </summary>
        public byte[] Value
        {
            get;
            private set;
        }

        private void Init(byte[] key)
        {
            if (key == null || key.Length != 270)
            {
                throw new System.ArgumentException("data");
            }
            Value = new byte[key.Length];
            Array.Copy(key, 0, Value, 0, key.Length);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXAsn1PublicKey()
        {

        }

        /// <summary>
        /// Constructor for RSA Public Key (PKCS#1). This is read from PEM file.
        /// </summary>
        ///  <param name="data">(PKCS#1) Public key. </param>
        ///
        public GXAsn1PublicKey(GXAsn1BitString data)
        {
            if (data == null)
            {
                throw new System.ArgumentException("key");
            }
            GXAsn1Sequence seq = (GXAsn1Sequence)GXAsn1Converter.FromByteArray(data.Value);
            Init(GXAsn1Converter.ToByteArray(new object[] { seq[0], seq[1] }));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        ///  <param name="key">Public key. </param>
        public GXAsn1PublicKey(byte[] key)
        {
            Init(key);
        }

        public override sealed string ToString()
        {
            if (Value == null)
            {
                return "";
            }
            return GXCommon.ToHex(Value, false);
        }
    }

}