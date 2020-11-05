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

using Gurux.DLMS.ASN.Enums;
using System;
using System.Text;

namespace Gurux.DLMS.ASN
{
    public class GXAsn1ObjectIdentifier
    {
        public string ObjectIdentifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXAsn1ObjectIdentifier()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oid">Object identifier in dotted format. </param>
        public GXAsn1ObjectIdentifier(string oid)
        {
            ObjectIdentifier = oid;
        }

        //  Constructor.
        public GXAsn1ObjectIdentifier(GXByteBuffer bb, int count)
        {
            ObjectIdentifier = OidStringFromBytes(bb, count);
        }

        /// <summary>
        /// Get OID string from bytes.
        /// </summary>
        /// <param name="bb">converted bytes. </param>
        /// <param name="len">byte count. </param>
        /// <returns> OID string. </returns>
        private static string OidStringFromBytes(GXByteBuffer bb, int len)
        {
            long value = 0;
            StringBuilder sb = new StringBuilder();
            if (len != 0)
            {
                // Get first byte.
                int tmp = bb.GetUInt8();
                sb.Append(tmp / 40);
                sb.Append('.');
                sb.Append(tmp % 40);
                for (int pos = 1; pos != len; ++pos)
                {
                    tmp = bb.GetUInt8();
                    if ((tmp & 0x80) != 0)
                    {
                        value += (tmp & 0x7F);
                        value <<= 7;
                    }
                    else
                    {
                        value += tmp;
                        sb.Append('.');
                        sb.Append(value);
                        value = 0;
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Convert OID string to bytes.
        /// </summary>
        internal static byte[] OidStringtoBytes(string oid)
        {
            Int64 value;
            string[] arr = oid.Trim().Split(new char[] { '.' });
            // Make first byte.
            GXByteBuffer tmp = new GXByteBuffer();
            value = Convert.ToInt32(arr[0]) * 40;
            value += Convert.ToInt32(arr[1]);
            tmp.SetUInt8((byte) value);
            for (int pos = 2; pos != arr.Length; ++pos)
            {
                value = Convert.ToInt32(arr[pos]);
                if (value < 0x80)
                {
                    tmp.SetUInt8((byte)value);
                }
                else if (value < 0x4000)
                {
                    tmp.SetUInt8((byte)(0x80 | (value >> 7)));
                    tmp.SetUInt8((byte)(value & 0x7F));
                }
                else if (value < 0x200000)
                {
                    tmp.SetUInt8((byte)(0x80 | (value >> 14)));
                    tmp.SetUInt8((byte)(0x80 | (value >> 7)));
                    tmp.SetUInt8((byte)(value & 0x7F));
                }
                else if (value < 0x10000000)
                {
                    tmp.SetUInt8((byte)(0x80 | (value >> 21)));
                    tmp.SetUInt8((byte)(0x80 | (value >> 14)));
                    tmp.SetUInt8((byte)(0x80 | (value >> 7)));
                    tmp.SetUInt8((byte)(value & 0x7F));
                }
                else if (value < 0x800000000L)
                {
                    tmp.SetUInt8((byte)(0x80 | (value >> 49)));
                    tmp.SetUInt8((byte)(0x80 | (value >> 42)));
                    tmp.SetUInt8((byte)(0x80 | (value >> 35)));
                    tmp.SetUInt8((byte)(0x80 | (value >> 28)));
                    tmp.SetUInt8((byte)(0x80 | (value >> 21)));
                    tmp.SetUInt8((byte)(0x80 | (value >> 14)));
                    tmp.SetUInt8((byte)(0x80 | (value >> 7)));
                    tmp.SetUInt8((byte)(value & 0x7F));
                }
                else
                {
                    throw new System.ArgumentException("Invalid OID.");
                }
            }
            return tmp.Array();
        }

        public override sealed string ToString()
        {
            return ObjectIdentifier;
        }

        /// <returns>
        /// Object identifier as byte array.
        /// </returns>
        public byte[] Encoded
        {
            get
            {
                return OidStringtoBytes(ObjectIdentifier);
            }
        }

        public string Description
        {
            get
            {
                object tmp = X509NameConverter.FromString(ObjectIdentifier);
                if ((X509Name) tmp != X509Name.None)
                {
                    return X509NameConverter.GetString((X509Name) tmp);
                }
                tmp = HashAlgorithmConverter.FromString(ObjectIdentifier);
                if ((HashAlgorithm) tmp != HashAlgorithm.None)
                {
                    return HashAlgorithmConverter.GetString((HashAlgorithm)tmp);
                }
                tmp = X9ObjectIdentifierConverter.FromString(ObjectIdentifier);
                if ((X9ObjectIdentifier)tmp != X9ObjectIdentifier.None)
                {
                    return X9ObjectIdentifierConverter.GetString((X9ObjectIdentifier)tmp);
                }
                tmp = PkcsObjectIdentifierConverter.FromString(ObjectIdentifier);
                if ((PkcsObjectIdentifier) tmp != PkcsObjectIdentifier.None)
                {
                    return PkcsObjectIdentifierConverter.GetString((PkcsObjectIdentifier) tmp);
                }
                tmp = X509CertificateTypeConverter.FromString(ObjectIdentifier);
                if ((X509CertificateType) tmp != X509CertificateType.None)
                {
                    return X509CertificateTypeConverter.GetString((X509CertificateType)tmp);
                }
                return null;
            }
        }
    }

}