//
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

using Gurux.DLMS.ASN.Enums;
using Gurux.DLMS.Ecdsa;
using Gurux.DLMS.Internal;
using System;
using System.IO;
using System.Text;

namespace Gurux.DLMS.ASN
{
    /// <summary>
    /// x509 Certificate.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc5280
    /// </remarks>
    public class GXx509Certificate
    {
        /// <summary>
        /// This extension identifies the public key being certified.
        /// </summary>
        public byte[] SubjectKeyIdentifier
        {
            get;
            private set;
        }

        /// <summary>May be used either as a certificate or CRL extension. It identifies the
        ///  public key to be used to verify the signature on this certificate or CRL.
        ///  It enables distinct keys used by the same CA to be distinguished. </summary>
        public byte[] AuthorityKeyIdentifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates if the Subject may act as a CA.
        /// </summary>
        public bool BasicConstraints
        {
            get;
            private set;
        }

        /// <summary>
        /// Signature algorithm.
        /// </summary>
        public HashAlgorithm SignatureAlgorithm
        {
            get;
            private set;
        }

        /// <summary>
        /// Signature Parameters.
        /// </summary>
        public object SignatureParameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Public key.
        /// </summary>
        public GXPublicKey PublicKey
        {
            get;
            private set;
        }

        /// <summary>
        /// Public Key algorithm.
        /// </summary>
        public HashAlgorithm PublicKeySignature
        {
            get;
            private set;
        }

        /// <summary>
        /// Parameters.
        /// </summary>
        public object PublicKeyParameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Signature.
        /// </summary>
        public byte[] Signature
        {
            get;
            private set;
        }

        /// <summary>
        /// Subject. Example: "CN=Test O=Gurux, L=Tampere, C=FI".
        /// </summary>
        public string Subject
        {
            get;
            private set;
        }


        /// <summary>
        /// Issuer. Example: "CN=Test O=Gurux, L=Tampere, C=FI".
        /// </summary>
        public string Issuer
        {
            get;
            private set;
        }

        /// <summary>Serial number. </summary>
        public GXAsn1Integer SerialNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Version.
        /// </summary>
        public CertificateVersion Version
        {
            get;
            private set;
        }

        /// <summary>
        /// Validity from.
        /// </summary>
        public DateTime ValidFrom
        {
            get;
            private set;
        }

        /// <summary>
        /// Validity to.
        /// </summary>
        public DateTime ValidTo
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates the purpose for which the certified public key is used.
        /// </summary>
        public KeyUsage KeyUsage
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXx509Certificate()
        {
            Version = CertificateVersion.Version3;
            KeyUsage = KeyUsage.None;
        }

        ///
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Base64 string. </param>
        public GXx509Certificate(string data)
        {
            string tmp = data.Replace("-----BEGIN EC CERTIFICATE-----", "");
            tmp = tmp.Replace("-----END EC CERTIFICATE-----", "");
            tmp = data.Replace("-----BEGIN CERTIFICATE-----", "");
            tmp = tmp.Replace("-----END CERTIFICATE-----", "");
            Init(GXCommon.FromBase64(tmp.Trim()));
        }

        //  https://tools.ietf.org/html/rfc5280#section-4.1
        private void Init(byte[] data)
        {
            GXAsn1Sequence seq = (GXAsn1Sequence)GXAsn1Converter.FromByteArray(data);
            if (seq.Count != 3)
            {
                throw new ArgumentException("Wrong number of elements in sequence.");
            }
            GXAsn1Sequence reqInfo = (GXAsn1Sequence)seq[0];
            Version = (CertificateVersion)((GXAsn1Context)reqInfo[0])[0];
            if (reqInfo[1] is sbyte)
            {
                SerialNumber = new GXAsn1Integer(Convert.ToUInt64(reqInfo[1]));
            }
            else
            {
                SerialNumber = (GXAsn1Integer)reqInfo[1];
            }
            string tmp = ((GXAsn1Sequence)reqInfo[2])[0].ToString();
            // Signature Algorithm
            SignatureAlgorithm = HashAlgorithmConverter.FromString(tmp);
            if (SignatureAlgorithm != HashAlgorithm.Sha256withecdsa)
            {
                throw new Exception("DLMS certificate must be signed with ecdsa-with-SHA256.");
            }
            // Optional.
            if (((GXAsn1Sequence)reqInfo[2]).Count > 1)
            {
                SignatureParameters = ((GXAsn1Sequence)reqInfo[2])[1];
            }
            // Issuer
            Issuer = GXAsn1Converter.GetSubject((GXAsn1Sequence)reqInfo[3]);
            // Validity
            ValidFrom = (DateTime)((GXAsn1Sequence)reqInfo[4])[0];
            ValidTo = (DateTime)((GXAsn1Sequence)reqInfo[4])[1];
            Subject = GXAsn1Converter.GetSubject((GXAsn1Sequence)reqInfo[5]);
            // Subject public key Info
            GXAsn1Sequence subjectPKInfo = (GXAsn1Sequence)reqInfo[6];
            PublicKey = GXPublicKey.FromRawBytes(((GXAsn1BitString)subjectPKInfo[1]).Value);
            // Get Standard Extensions.
            if (reqInfo.Count > 7)
            {
                foreach (GXAsn1Sequence s in (GXAsn1Sequence)((GXAsn1Context)reqInfo[7])[0])
                {
                    GXAsn1ObjectIdentifier id = (GXAsn1ObjectIdentifier)s[0];
                    object value = s[1];
                    Enums.X509CertificateType t = Enums.X509CertificateTypeConverter.FromString(id.ToString());
                    switch (t)
                    {
                        case Enums.X509CertificateType.SubjectKeyIdentifier:
                            SubjectKeyIdentifier = (byte[])value;
                            break;
                        case Enums.X509CertificateType.AuthorityKeyIdentifier:
                            AuthorityKeyIdentifier = (byte[])((GXAsn1Sequence)value)[0];
                            break;
                        case Enums.X509CertificateType.KeyUsage:
                            if (value is GXAsn1BitString)
                            {
                                // critical is optional. BOOLEAN DEFAULT FALSE,
                                KeyUsage = (KeyUsage)((GXAsn1BitString)value).Value[0];
                            }
                            else if (value is bool?)
                            {
                                value = s[2];
                                KeyUsage = (KeyUsage)((GXAsn1BitString)value).Value[0];
                            }
                            else
                            {
                                throw new ArgumentException("Invalid key usage.");
                            }
                            break;
                        default:
                            System.Diagnostics.Debug.WriteLine("Unknown extensions: " + t.ToString());
                            break;
                    }
                }
            }
            if (KeyUsage == KeyUsage.None)
            {
                throw new Exception("Key usage not present.");
            }
            PublicKeySignature = HashAlgorithmConverter.FromString(((GXAsn1Sequence)seq[1])[0].ToString());
            // Optional.
            if (((GXAsn1Sequence)seq[1]).Count > 1)
            {
                SignatureParameters = ((GXAsn1Sequence)seq[1])[1];
            }
            // signature
            Signature = ((GXAsn1BitString)seq[2]).Value;
        }

        ///
        /// <summary>Constructor.
        /// </summary>
        /// <param name="data">
        /// Encoded bytes.
        /// </param>
        public GXx509Certificate(byte[] data)
        {
            Init(data);
        }

        private object[] GetData()
        {
            GXAsn1ObjectIdentifier a = new GXAsn1ObjectIdentifier(HashAlgorithmConverter.GetString(SignatureAlgorithm));
            GXAsn1Context p = new GXAsn1Context();
            p.Add((sbyte)Version);
            object subjectPKInfo = GXAsn1Converter.FromByteArray(PublicKey.RawValue);
            GXAsn1Sequence s = new GXAsn1Sequence();
            GXAsn1Sequence s1;
            if (SubjectKeyIdentifier != null)
            {
                s1 = new GXAsn1Sequence();
                s1.Add(new GXAsn1ObjectIdentifier(X509CertificateTypeConverter.GetString(Enums.X509CertificateType.SubjectKeyIdentifier)));
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8(BerType.OctetString);
                GXCommon.SetObjectCount(SubjectKeyIdentifier.Length, bb);
                bb.Set(SubjectKeyIdentifier);
                s1.Add(bb.Array());
                s.Add(s1);
            }
            if (AuthorityKeyIdentifier != null)
            {
                s1 = new GXAsn1Sequence();
                s1.Add(new GXAsn1ObjectIdentifier(X509CertificateTypeConverter.GetString(Enums.X509CertificateType.AuthorityKeyIdentifier)));
                GXAsn1Sequence seq = new GXAsn1Sequence();
                seq.Add(AuthorityKeyIdentifier);
                s1.Add(GXAsn1Converter.ToByteArray(seq));
                s.Add(s1);
            }
            if (BasicConstraints)
            {
                s1 = new GXAsn1Sequence();
                s1.Add(new GXAsn1ObjectIdentifier(X509CertificateTypeConverter.GetString(Enums.X509CertificateType.BasicConstraints)));
                GXAsn1Sequence seq = new GXAsn1Sequence();
                seq.Add(BasicConstraints);
                s1.Add(GXAsn1Converter.ToByteArray(seq));
                s.Add(s1);
            }
            if (KeyUsage == KeyUsage.None)
            {
                throw new Exception("Key usage not present.");
            }
            s1 = new GXAsn1Sequence();
            s1.Add(new GXAsn1ObjectIdentifier(X509CertificateTypeConverter.GetString(Enums.X509CertificateType.KeyUsage)));
            byte value = 0;
            int min = 255;
            foreach (KeyUsage it in Enum.GetValues(typeof(KeyUsage)))
            {
                if ((it & KeyUsage) != 0)
                {
                    byte val = (byte)it;
                    value |= val;
                    if (val < min)
                    {
                        min = val;
                    }
                }
            }
            int offset = 7;
            while ((min >>= 2) != 0)
            {
                ++offset;
            }
            byte[] tmp = GXAsn1Converter.ToByteArray(new GXAsn1BitString(new byte[] { 0, value }));
            s1.Add(tmp);
            s.Add(s1);
            GXAsn1Sequence valid = new GXAsn1Sequence();
            valid.Add(ValidFrom);
            valid.Add(ValidTo);
            object[] list;
            if (s.Count == 0)
            {
                list = new object[] { p, SerialNumber, new object[] { a, SignatureParameters }, GXAsn1Converter.EncodeSubject(Issuer), valid, GXAsn1Converter.EncodeSubject(Subject), subjectPKInfo };
            }
            else
            {
                GXAsn1Context tmp2 = new GXAsn1Context();
                tmp2.Index = 3;
                tmp2.Add(s);
                list = new object[] { p, SerialNumber, new object[] { a, SignatureParameters }, GXAsn1Converter.EncodeSubject(Issuer), valid, GXAsn1Converter.EncodeSubject(Subject), subjectPKInfo, tmp2 };
            }
            return list;
        }

        public byte[] Encoded
        {
            get
            {
                object tmp = new object[] { new GXAsn1ObjectIdentifier(HashAlgorithmConverter.GetString(SignatureAlgorithm)), SignatureParameters };
                object[] list = new object[] { GetData(), tmp, new GXAsn1BitString(Signature, 0) };
                return GXAsn1Converter.ToByteArray(list);
            }
        }

        public override sealed string ToString()
        {
            StringBuilder bb = new StringBuilder();
            bb.Append("Version: ");
            bb.Append(Version.ToString());
            bb.Append("\n");
            bb.Append("Subject: ");
            bb.Append(Subject);
            bb.Append("\n");

            bb.Append("Signature: ");
            bb.Append(SignatureAlgorithm.ToString());
            bb.Append(", OID = ");
            bb.Append(HashAlgorithmConverter.GetString(SignatureAlgorithm));
            bb.Append("\n");
            bb.Append("Key: ");
            if (PublicKey != null)
            {
                bb.Append(PublicKey.ToString());
            }
            bb.Append("\n");
            bb.Append("Validity: [From: ");
            bb.Append(ValidFrom.ToString());
            bb.Append(", \n");
            bb.Append("To: ");
            bb.Append(ValidTo.ToString());
            bb.Append("]\n");
            bb.Append("Issuer: ");
            bb.Append(Issuer);
            bb.Append("\n");
            bb.Append("SerialNumber: ");
            bb.Append(SerialNumber);
            bb.Append("\n");
            bb.Append("Algorithm: ");
            bb.Append(PublicKeySignature.ToString());
            bb.Append("\n");
            bb.Append("Signature: ");
            bb.Append(GXCommon.ToHex(Signature, false));
            bb.Append("\n");
            return bb.ToString();
        }

        /// <summary>
        /// Load private key from the PEM file.
        /// </summary>
        /// <param name="path">File path. </param>
        /// <returns> Created GXPkcs8 object. </returns>
        public static GXx509Certificate Load(string path)
        {
            return new GXx509Certificate(File.ReadAllText(path));
        }

        /// <summary>
        /// Save private key to PEM file.
        /// </summary>
        /// <param name="path">File path.</param>
        public virtual void Save(string path)
        {
            StringBuilder sb = new StringBuilder();
            if (PublicKey != null)
            {
                sb.Append("-----BEGIN CERTIFICATE-----\n");
                sb.Append(ToPem());
                sb.Append(Environment.NewLine + "-----END CERTIFICATE-----\n");
                File.WriteAllText(path, sb.ToString());
            }
            else
            {
                throw new System.ArgumentException("Public or private key is not set.");
            }
        }

        /// <summary>
        /// Public key in PEM format.
        /// </summary>
        /// <returns>Public key as in PEM string.</returns>
        public string ToPem()
        {
            return GXCommon.ToBase64(Encoded);
        }
    }

}