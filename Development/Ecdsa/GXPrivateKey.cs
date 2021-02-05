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
using System.Collections.Generic;
using System.IO;

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

        private GXPublicKey publicKey;

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
            der = der.Replace("\r\n", "");
            der = der.Replace("\n", "");
            byte[] key = GXCommon.FromBase64(der);
            GXAsn1Sequence seq = (GXAsn1Sequence)GXAsn1Converter.FromByteArray(key);
            if ((sbyte)seq[0] > 3)
            {
                throw new ArgumentOutOfRangeException("Invalid private key version.");
            }
            List<object> tmp = (List<object>)seq[2];
            GXPrivateKey value = new GXPrivateKey();
            X9ObjectIdentifier id = X9ObjectIdentifierConverter.FromString(tmp[0].ToString());
            switch (id)
            {
                case X9ObjectIdentifier.Prime256v1:
                    value.Scheme = Ecc.P256;
                    break;
                case X9ObjectIdentifier.Secp384r1:
                    value.Scheme = Ecc.P384;
                    break;
                default:
                    if (id == X9ObjectIdentifier.None)
                    {
                        throw new ArgumentOutOfRangeException("Invalid private key " + tmp[0].ToString() + ".");
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Invalid private key " + id + " " + tmp[0].ToString() + ".");
                    }
            }
            value.RawValue = (byte[])seq[1];
            if (seq[3] is byte[])
            {
                value.publicKey = GXPublicKey.FromRawBytes((byte[])seq[3]);
            }
            else
            {
                //Open SSL PEM.
                value.publicKey = GXPublicKey.FromRawBytes(((GXAsn1BitString)((List<object>)seq[3])[0]).Value);
            }
            return value;
        }

        /// <summary>
        /// Create the private key from PEM.
        /// </summary>
        /// <param name="pem">PEM in Base64 coded string.</param>
        /// <returns>Private key.</returns>
        public static GXPrivateKey FromPem(string pem)
        {
            pem = pem.Replace("\r\n", "\n");
            const string START = "PRIVATE KEY-----\n";
            const string END = "-----END";
            int index = pem.IndexOf(START);
            if (index == -1)
            {
                throw new ArgumentException("Invalid PEM file.");
            }
            pem = pem.Substring(index + START.Length);
            index = pem.IndexOf(END);
            if (index == -1)
            {
                throw new ArgumentException("Invalid PEM file.");
            }
            return FromDer(pem.Substring(0, index));
        }

        /// <summary>
        /// Create the private key from PEM file.
        /// </summary>
        /// <param name="path">Path to the PEM file.</param>
        /// <returns>Private key.</returns>
        public static GXPrivateKey Load(string path)
        {
            return FromPem(File.ReadAllText(path));
        }

        /// <summary>
        /// Save private key to PEM file.
        /// </summary>
        /// <param name="path">File path. </param>
        public virtual void Save(string path)
        {
            File.WriteAllText(path, ToPem());
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
            d.Add(new GXAsn1BitString(GetPublicKey().RawValue));
            return GXCommon.ToBase64(GXAsn1Converter.ToByteArray(d));
        }

        public string ToPem()
        {
            return "-----BEGIN EC PRIVATE KEY-----\n" + ToDer() + "\n-----END EC PRIVATE KEY-----";
        }

        /// <summary>
        /// Get public key from private key.
        /// </summary>
        /// <param name="scheme">Used scheme.</param>
        /// <param name="privateKey">Private key bytes.</param>
        /// <returns>Public key.</returns>
        public GXPublicKey GetPublicKey()
        {
            if (publicKey == null)
            {
                GXBigInteger pk = new GXBigInteger(RawValue);
                GXCurve curve = new GXCurve(Scheme);
                GXEccPoint p = new GXEccPoint(curve.G.x, curve.G.y, new GXBigInteger(1));
                p = GXEcdsa.JacobianMultiply(p, pk, curve.N, curve.A, curve.P);
                GXEcdsa.FromJacobian(p, curve.P);
                GXByteBuffer key = new GXByteBuffer(65);
                //Public key is un-compressed format.
                key.SetUInt8(4);
                byte[] tmp = p.x.ToArray();
                int size = Scheme == Ecc.P256 ? 32 : 48;
                key.Set(tmp, tmp.Length % size, size);
                tmp = p.y.ToArray();
                key.Set(tmp, tmp.Length % size, size);
                publicKey = GXPublicKey.FromRawBytes(key.Array());
            }
            return publicKey;
        }

        /// <summary>
        /// Returns the private key as a hex string.
        /// </summary>
        /// <returns>Private key as hex string.</returns>
        public string ToHex()
        {
            return ToHex(true);
        }

        /// <summary>
        /// Returns the private key as a hex string.
        /// </summary>
        /// <param name="addSpace">Is space added between the bytes.</param>
        /// <returns>Private key as hex string.</returns>
        public string ToHex(bool addSpace)
        {
            return GXDLMSTranslator.ToHex(RawValue, addSpace);
        }

        public override bool Equals(object obj)
        {
            if (obj is GXPrivateKey pk)
            {
                return GXCommon.Compare(RawValue, pk.RawValue);
            }
            return false;
        }
        public override int GetHashCode()
        {
            if (RawValue == null)
            {
                return 0;
            }
            return RawValue.GetHashCode();
        }
    }
}
