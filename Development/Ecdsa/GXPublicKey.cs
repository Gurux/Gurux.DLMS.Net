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
using System.Text;

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
        /// SystemTitle is an extra information that can be used in debugging.
        /// </summary>
        /// <remarks>
        /// SystemTitle is not serialized.
        /// </remarks>
        public byte[] SystemTitle
        {
            get;
            set;
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
            else if (key.Length == 64)
            {
                //Compression tag is not send in DLMS messages.
                value.Scheme = Ecc.P256;
                value.RawValue = new byte[65];
                value.RawValue[0] = 4;
                Array.Copy(key, 0, value.RawValue, 1, 64);
            }
            else if (key.Length == 96)
            {
                //Compression tag is not send in DLMS messages.
                value.Scheme = Ecc.P384;
                value.RawValue = new byte[96];
                value.RawValue[0] = 4;
                Array.Copy(key, 0, value.RawValue, 1, 95);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid public key.");
            }
            return value;
        }

        /// <summary>
        /// Create the public key from DER.
        /// </summary>
        /// <param name="der">DER Base64 coded string.</param>
        /// <returns>Public key.</returns>
        public static GXPublicKey FromDer(string der)
        {
            der = der.Replace("\r\n", "");
            der = der.Replace("\n", "");
            GXPublicKey value = new GXPublicKey();
            byte[] key = GXCommon.FromBase64(der);
            GXAsn1Sequence seq = (GXAsn1Sequence)GXAsn1Converter.FromByteArray(key);
            List<object> tmp = (List<object>)seq[0];
            X9ObjectIdentifier id = X9ObjectIdentifierConverter.FromString(tmp[1].ToString());
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
                        throw new ArgumentOutOfRangeException("Invalid public key " + tmp[0].ToString() + ".");
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Invalid public key " + id + " " + tmp[0].ToString() + ".");
                    }
            }
            if (seq[1] is byte[])
            {
                value.RawValue = (byte[])seq[1];
            }
            else
            {
                //Open SSL PEM.
                value.RawValue = ((GXBitString)seq[1]).Value;
            }
            return value;
        }

        /// <summary>
        /// Create the public key from PEM.
        /// </summary>
        /// <param name="pem">PEM Base64 coded string.</param>
        /// <returns>Public key.</returns>
        public static GXPublicKey FromPem(string pem)
        {
            pem = pem.Replace("\r\n", "\n");
            const string START = "-----BEGIN PUBLIC KEY-----\n";
            const string END = "\n-----END PUBLIC KEY-----";
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
        /// Create the public key from PEM file.
        /// </summary>
        /// <param name="path">Path to the PEM file.</param>
        /// <returns>Public key.</returns>
        public static GXPublicKey Load(string path)
        {
            return FromPem(File.ReadAllText(path));
        }

        /// <summary>
        /// Save public key to PEM file.
        /// </summary>
        /// <param name="path">File path. </param>
        public virtual void Save(string path)
        {
            File.WriteAllText(path, ToPem());
        }

        /// <summary>
        /// Returns the public key as a hex string.
        /// </summary>
        public string ToHex()
        {
            return GXDLMSTranslator.ToHex(RawValue);
        }

        /// <summary>
        /// Get public key as DER format.
        /// </summary>
        /// <returns></returns>
        public string ToDer()
        {
            return GXCommon.ToBase64(ToEncoded());
        }

        /// <summary>
        /// Get public key as encoded format.
        /// </summary>
        /// <returns></returns>
        public byte[] ToEncoded()
        {
            //Subject Public Key Info.
            GXAsn1Sequence d = new GXAsn1Sequence();
            GXAsn1Sequence d1 = new GXAsn1Sequence();
            d1.Add(new GXAsn1ObjectIdentifier("1.2.840.10045.2.1"));
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
            d.Add(new GXBitString(RawValue, 0));
            return GXAsn1Converter.ToByteArray(d);
        }

        /// <summary>
        /// Get public key as PEM format.
        /// </summary>
        public string ToPem()
        {
            return "-----BEGIN PUBLIC KEY-----\n" + ToDer() + "\n-----END PUBLIC KEY-----\n";
        }

        /// <summary>
        /// X Coordinate.
        /// </summary>
        public byte[] X
        {
            get 
            {
                GXByteBuffer pk = new GXByteBuffer(RawValue);
                int size = pk.Size / 2;
                return pk.SubArray(1, size);
            }
        }

        /// <summary>
        /// Y Coordinate.
        /// </summary>
        public byte[] Y
        {
            get
            {
                GXByteBuffer pk = new GXByteBuffer(RawValue);
                int size = pk.Size / 2;
                return pk.SubArray(1 + size, size);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Scheme == Ecc.P256)
            {
                sb.AppendLine("NIST P-256");
            }
            else if (Scheme == Ecc.P384)
            {
                sb.AppendLine("NIST P-384");
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid scheme.");
            }
            GXByteBuffer pk = new GXByteBuffer(RawValue);
            int size = pk.Size / 2;
            sb.Append(" public x coord: ");
            sb.AppendLine(new GXBigInteger(pk.SubArray(1, size)).ToString());
            sb.Append(" public y coord: ");
            sb.AppendLine(new GXBigInteger(pk.SubArray(1 + size, size)).ToString());
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is GXPublicKey o)
            {
                return GXCommon.Compare(RawValue, o.RawValue);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return RawValue.GetHashCode();
        }
    }
}
