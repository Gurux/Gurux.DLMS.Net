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
using Gurux.DLMS.ASN;
using Gurux.DLMS.ASN.Enums;
using Gurux.DLMS.Ecdsa.Enums;
using Gurux.DLMS.Internal;
using System;

namespace Gurux.DLMS.Ecdsa
{
    /// <summary>
    /// Private key.
    /// </summary>
    public class GXPrivateKey
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
        /// Create the private key from raw bytes.
        /// </summary>
        /// <param name="key">Raw data</param>
        /// <returns>Private key.</returns>
        public static GXPrivateKey FromRawBytes(byte[] key)
        {
            GXPrivateKey value = new GXPrivateKey();
            //If private key is given
            if (key.Length == 32)
            {
                value.Scheme = Ecc.P256;
                value.RawValue = key;
            }
            else if (key.Length == 48)
            {
                value.Scheme = Ecc.P384;
                value.RawValue = key;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid private key.");
            }
            return value;
        }

        /// <summary>
        /// Create the private key from DER.
        /// </summary>
        /// <param name="key">DER Base64 coded string.</param>
        /// <returns></returns>
        public static GXPrivateKey FromDer(string der)
        {
            byte[] key = GXCommon.FromBase64(der);
            object[] tmp = (object[])GXAsn1Converter.FromByteArray(key);
            GXPrivateKey value = new GXPrivateKey();
            //If private key is given
            if (key.Length == 32)
            {
                value.Scheme = Ecc.P256;
                value.RawValue = key;
            }
            else if (key.Length == 48)
            {
                value.Scheme = Ecc.P384;
                value.RawValue = key;
            }
            else if (key.Length == 65)
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
                throw new ArgumentOutOfRangeException("Invalid key.");
            }
            return value;
        }

        public string ToDer()
        {
            GXAsn1Sequence d = new GXAsn1Sequence();
            d.Add((sbyte)CertificateVersion.Version2);
            d.Add(RawValue);
            GXAsn1Sequence d1 = new GXAsn1Sequence();
            if (Scheme == Ecc.P256)
            {
                d1.Add(new GXAsn1ObjectIdentifier("1.2.840.10045.3.1.7"));
            }
            else if (Scheme == Ecc.P384)
            {
                d1.Add(new GXAsn1ObjectIdentifier("1.3.132.0.34"));
            }
            else
            {
                throw new Exception("Invalid ECC scheme.");
            }
            d.Add(d1);
            d.Add(GetPublicKey().RawValue);
            return GXCommon.ToBase64(GXAsn1Converter.ToByteArray(d));
        }
        public string ToPem()
        {
            return "-----BEGIN EC PRIVATE KEY-----\n" + ToDer() + "-----END EC PRIVATE KEY-----";
        }

        /// <summary>
        /// Get public key from private key.
        /// </summary>
        /// <param name="scheme">Used scheme.</param>
        /// <param name="privateKey">Private key bytes.</param>
        /// <returns>Public key.</returns>
        public GXPublicKey GetPublicKey()
        {
            GXBigInteger secret = new GXBigInteger(RawValue);
            GXCurve curve = new GXCurve(Scheme);
            GXEccPoint p = new GXEccPoint(curve.G.x, curve.G.y, new GXBigInteger(1));
            p = GXEcdsa.JacobianMultiply(p, secret, curve.N, curve.A, curve.P);
            GXEcdsa.FromJacobian(p, curve.P);
            GXByteBuffer key = new GXByteBuffer(65);
            //Public key is un-compressed format.
            key.SetUInt8(4);
            byte[] tmp = p.x.ToArray();
            key.Set(tmp, tmp.Length % 32, 32);
            tmp = p.y.ToArray();
            key.Set(tmp, tmp.Length % 32, 32);
            return GXPublicKey.FromRawBytes(key.Array());
        }
    }
}
