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
using Gurux.DLMS.Ecdsa.Enums;
using Gurux.DLMS.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
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
        /// Return default file path.
        /// </summary>
        /// <returns></returns>
        public static string GetFilePath(GXx509Certificate cert)
        {
            string path;
            switch (cert.KeyUsage)
            {
                case KeyUsage.DigitalSignature:
                    path = "D";
                    break;
                case KeyUsage.KeyAgreement:
                    path = "A";
                    break;
                case KeyUsage.DigitalSignature | KeyUsage.KeyAgreement:
                    path = "T";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown certificate type.");
            }
            path += GXAsn1Converter.HexSystemTitleFromSubject(cert.Subject).Trim() + ".pem";
            if (cert.PublicKey.Scheme == Ecc.P256)
            {
                path = Path.Combine("Certificates", path);
            }
            else
            {
                path = Path.Combine("Certificates384", path);
            }
            return path;
        }

        /// <summary>
        /// Description is extra metadata that is saved to PEM file. 
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Loaded x509Certificate as raw data.
        /// </summary>
        private byte[] rawData;

        /// <summary>
        /// This extension identifies the public key being certified.
        /// </summary>
        public byte[] SubjectKeyIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// May be used either as a certificate or CRL extension. It identifies the
        /// public key to be used to verify the signature on this certificate or CRL.
        /// It enables distinct keys used by the same CA to be distinguished. </summary>
        public byte[] AuthorityKeyIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Authority certification serial number.
        /// </summary>
        public BigInteger AuthorityCertificationSerialNumber
        {
            get;
            set;
        }


        /// <summary>
        /// Indicates if the Subject may act as a CA.
        /// </summary>
        public bool BasicConstraints
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates that a certificate can be used as an TLS server or client certificate.
        /// </summary>
        public ExtendedKeyUsage ExtendedKeyUsage
        {
            get;
            set;
        }

        /// <summary>
        /// Signature algorithm.
        /// </summary>
        public HashAlgorithm SignatureAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Signature Parameters.
        /// </summary>
        public object SignatureParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Public key.
        /// </summary>
        public GXPublicKey PublicKey
        {
            get;
            set;
        }

        /// <summary>
        /// Public Key algorithm.
        /// </summary>
        public HashAlgorithm PublicKeyAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Parameters.
        /// </summary>
        public object PublicKeyParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Signature.
        /// </summary>
        public byte[] Signature
        {
            get;
            set;
        }

        /// <summary>
        /// Subject. Example: "CN=Test, O=Gurux, L=Tampere, C=FI".
        /// </summary>
        public string Subject
        {
            get;
            set;
        }


        /// <summary>
        /// Issuer. Example: "CN=Test O=Gurux, L=Tampere, C=FI".
        /// </summary>
        public string Issuer
        {
            get;
            set;
        }

        /// <summary>
        /// Authority Cert Issuer. Example: "CN=Test O=Gurux, L=Tampere, C=FI".
        /// </summary>
        public string AuthorityCertIssuer
        {
            get;
            set;
        }

        /// <summary>
        /// Serial number.
        /// </summary>
        public BigInteger SerialNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Version.
        /// </summary>
        /// <remarks>
        /// Version is read-only because DLMS supports only v3.
        /// </remarks>
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
            set;
        }

        /// <summary>
        /// Validity to.
        /// </summary>
        public DateTime ValidTo
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the purpose for which the certified public key is used.
        /// </summary>
        public KeyUsage KeyUsage
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXx509Certificate()
        {
            Version = CertificateVersion.Version3;
            KeyUsage = KeyUsage.None;
            SignatureAlgorithm = HashAlgorithm.Sha256WithEcdsa;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">PEM string.</param>
        [Obsolete("Use FromPem instead.")]
        public GXx509Certificate(string data)
        {
            data = data.Replace("\r\n", "\n");
            const string START = "CERTIFICATE-----\n";
            const string END = "-----END";
            data = data.Replace("\r\n", "\n");
            int start = data.IndexOf(START);
            if (start == -1)
            {
                throw new ArgumentException("Invalid PEM file.");
            }
            data = data.Substring(start + START.Length);
            int end = data.IndexOf(END);
            if (end == -1)
            {
                throw new ArgumentException("Invalid PEM file.");
            }
            Init(GXCommon.FromBase64(data.Substring(0, end)));
        }

        /// <summary>
        /// Create x509Certificate from hex string.
        /// </summary>
        /// <param name="data">Hex string.</param>
        /// <returns>x509 certificate</returns>
        public static GXx509Certificate FromHexString(string data)
        {
            GXx509Certificate cert = new GXx509Certificate();
            cert.Init(GXCommon.HexToBytes(data));
            return cert;
        }

        public static GXx509Certificate FromCertificate(X509Certificate2 cert)
        {
            GXx509Certificate tmp = new GXx509Certificate();
            tmp.Init(cert.RawData);
            return tmp;
        }

        /// <summary>
        /// Create x509Certificate from PEM string.
        /// </summary>
        /// <param name="data">PEM string.</param>
        /// <returns>x509 certificate</returns>
        public static GXx509Certificate FromPem(string data)
        {
            data = data.Replace("\r\n", "\n");
            const string START = "CERTIFICATE-----\n";
            const string END = "-----END";
            data = data.Replace("\r\n", "\n");
            int start = data.IndexOf(START);
            if (start == -1)
            {
                throw new ArgumentException("Invalid PEM file.");
            }
            string desc = null;
            if (start != 11)
            {
                desc = data.Substring(0, start);
                const string DESCRIPTION = "#Description";
                //Check if there is a description metadata.
                int descStart = desc.LastIndexOf(DESCRIPTION);
                if (descStart != -1)
                {
                    int descEnd = desc.IndexOf("\n", descStart, start);
                    desc = desc.Substring(descStart + DESCRIPTION.Length, descEnd - DESCRIPTION.Length);
                    desc = desc.Trim();
                }
                else
                {
                    desc = null;
                }
            }

            data = data.Substring(start + START.Length);
            int end = data.IndexOf(END);
            if (end == -1)
            {
                throw new ArgumentException("Invalid PEM file.");
            }
            GXx509Certificate cert = FromDer(data.Substring(0, end));
            cert.Description = desc;
            return cert;
        }

        /// <summary>
        /// Create x509Certificate from DER Base64 encoded string.
        /// </summary>
        /// <param name="der">Base64 DER string.</param>
        /// <returns>x509 certificate</returns>
        public static GXx509Certificate FromDer(string der)
        {
            der = der.Replace("\r\n", "");
            der = der.Replace("\n", "");
            GXx509Certificate cert = new GXx509Certificate();
            cert.Init(GXCommon.FromBase64(der));
            return cert;
        }

        //  https://tools.ietf.org/html/rfc5280#section-4.1
        private void Init(byte[] data)
        {
            rawData = data;
            GXAsn1Sequence seq = (GXAsn1Sequence)GXAsn1Converter.FromByteArray(data);
            if (seq.Count != 3)
            {
                throw new GXDLMSCertificateException("Invalid Certificate Version. Wrong number of elements in sequence.");
            }
            if (!(seq[0] is GXAsn1Sequence))
            {
                PkcsType type = GXAsn1Converter.GetCertificateType(data, seq);
                switch (type)
                {
                    case PkcsType.Pkcs8:
                        throw new GXDLMSCertificateException("Invalid Certificate. This is PKCS 8 private key, not x509 certificate.");
                    case PkcsType.Pkcs10:
                        throw new GXDLMSCertificateException("Invalid Certificate. This is PKCS 10 certification requests, not x509 certificate.");
                }
                throw new GXDLMSCertificateException("Invalid Certificate Version.");
            }
            GXAsn1Sequence reqInfo = (GXAsn1Sequence)seq[0];
            if ((reqInfo[0] is GXAsn1Integer))
            {
                throw new GXDLMSCertificateException("Invalid Certificate. DLMS certificate version number must be 3.");
            }
            Version = (CertificateVersion)((GXAsn1Context)reqInfo[0])[0];
            if (reqInfo[1] is sbyte)
            {
                SerialNumber = Convert.ToUInt64(reqInfo[1]);
            }
            else
            {
                SerialNumber = ((GXAsn1Integer)reqInfo[1]).ToBigInteger();
            }
            string tmp = ((GXAsn1Sequence)reqInfo[2])[0].ToString();
            // Signature Algorithm
            SignatureAlgorithm = HashAlgorithmConverter.FromString(tmp);
            if (SignatureAlgorithm != HashAlgorithm.Sha256WithEcdsa &&
                SignatureAlgorithm != HashAlgorithm.Sha384WithEcdsa)
            {
                throw new Exception("DLMS certificate must be signed with ECDSA with SHA256 or SHA384.");
            }
            // Optional.
            if (((GXAsn1Sequence)reqInfo[2]).Count > 1)
            {
                SignatureParameters = ((GXAsn1Sequence)reqInfo[2])[1];
            }
            // Issuer
            Issuer = GXAsn1Converter.GetSubject((GXAsn1Sequence)reqInfo[3]);
            bool basicConstraintsExists = false;
            // Validity
            ValidFrom = (DateTime)((GXAsn1Sequence)reqInfo[4])[0];
            ValidTo = (DateTime)((GXAsn1Sequence)reqInfo[4])[1];
            Subject = GXAsn1Converter.GetSubject((GXAsn1Sequence)reqInfo[5]);
            string CN = X509NameConverter.GetString(X509Name.CN);
            // Subject public key Info
            GXAsn1Sequence subjectPKInfo = (GXAsn1Sequence)reqInfo[6];
            PublicKey = GXPublicKey.FromRawBytes(((GXAsn1BitString)subjectPKInfo[1]).Value);
            GXEcdsa.Validate(PublicKey);
            // Get Standard Extensions.
            if (reqInfo.Count > 7)
            {
                foreach (GXAsn1Sequence s in (GXAsn1Sequence)((GXAsn1Context)reqInfo[7])[0])
                {
                    GXAsn1ObjectIdentifier id = (GXAsn1ObjectIdentifier)s[0];
                    object value = s[1];
                    X509CertificateType t = X509CertificateTypeConverter.FromString(id.ToString());
                    switch (t)
                    {
                        case X509CertificateType.SubjectKeyIdentifier:
                            SubjectKeyIdentifier = (byte[])value;
                            break;
                        case X509CertificateType.AuthorityKeyIdentifier:
                            foreach (GXAsn1Context it in (GXAsn1Sequence)value)
                            {
                                switch (it.Index)
                                {
                                    case 0:
                                        //Authority Key Identifier.
                                        AuthorityKeyIdentifier = (byte[])it[0];
                                        break;
                                    case 1:
                                        {
                                            StringBuilder sb = new StringBuilder();
                                            //authorityCertIssuer
                                            foreach (KeyValuePair<object, object> it2 in ((GXAsn1Sequence)((GXAsn1Context)it[0])[0]))
                                            {
                                                if (sb.Length != 0)
                                                {
                                                    sb.Append(", ");
                                                }
                                                sb.Append(X509NameConverter.FromString(Convert.ToString(it2.Key)));
                                                sb.Append("=");
                                                sb.Append(Convert.ToString(it2.Value));
                                            }
                                            AuthorityCertIssuer = sb.ToString();
                                        }
                                        break;
                                    case 2:
                                        //Authority cert serial number .
                                        AuthorityCertificationSerialNumber = new BigInteger((byte[])it[0]);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException("Invalid context." + it.Index);
                                }
                            }
                            break;
                        case X509CertificateType.KeyUsage:
                            if (value is GXAsn1BitString)
                            {
                                // critical is optional. BOOLEAN DEFAULT FALSE,
                                KeyUsage = (KeyUsage)((GXAsn1BitString)value).ToInteger();
                            }
                            else if (value is bool?)
                            {
                                value = s[2];
                                KeyUsage = (KeyUsage)((GXAsn1BitString)value).ToInteger();
                            }
                            else
                            {
                                throw new ArgumentException("Invalid key usage.");
                            }
                            break;
                        case X509CertificateType.ExtendedKeyUsage:
                            if (value is GXAsn1Sequence ext)
                            {
                                foreach (GXAsn1ObjectIdentifier it in ext)
                                {
                                    if (it.ObjectIdentifier == "1.3.6.1.5.5.7.3.1")
                                    {
                                        ExtendedKeyUsage |= ExtendedKeyUsage.ServerAuth;
                                    }
                                    else if (it.ObjectIdentifier == "1.3.6.1.5.5.7.3.2")
                                    {
                                        ExtendedKeyUsage |= ExtendedKeyUsage.ClientAuth;
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Invalid extended key usage.");
                                    }
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Invalid extended key usage.");
                            }
                            break;
                        case X509CertificateType.BasicConstraints:
                            basicConstraintsExists = true;
                            if (value is GXAsn1Sequence)
                            {
                                if (((GXAsn1Sequence)value).Count != 0)
                                {
                                    BasicConstraints = (bool)((GXAsn1Sequence)value)[0];
                                }
                            }
                            else if (value is bool?)
                            {
                                BasicConstraints = (bool)value;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid key usage.");
                            }
                            break;
                        default:
                            System.Diagnostics.Debug.WriteLine("Unknown extensions: " + id.ToString());
                            break;
                    }
                }
            }
            if (!basicConstraintsExists)
            {
                // Verify that subject Common Name includes system title.
                bool commonNameFound = false;
                foreach (KeyValuePair<object, object> it in (GXAsn1Sequence)reqInfo[5])
                {
                    if (CN == it.Key.ToString())
                    {
                        if (Convert.ToString(it.Value).Length != 16)
                        {
                            throw new GXDLMSCertificateException("System title is not included in Common Name.");
                        }
                        commonNameFound = true;
                        break;
                    }
                }
                if (!commonNameFound)
                {
                    throw new GXDLMSCertificateException("Common name doesn't exist.");
                }
            }

            if (KeyUsage == KeyUsage.None)
            {
                throw new Exception("Key usage not present. It's mandotory.");
            }
            if ((KeyUsage & (KeyUsage.KeyCertSign | KeyUsage.CrlSign)) != 0 && !basicConstraintsExists)
            {
                throw new Exception("Basic Constraints value not present. It's mandotory.");
            }
            if (KeyUsage == (KeyUsage.DigitalSignature | KeyUsage.KeyAgreement) && ExtendedKeyUsage == ExtendedKeyUsage.None)
            {
                throw new Exception("Extended key usage not present. It's mandotory for TLS.");
            }
            if (ExtendedKeyUsage != ExtendedKeyUsage.None && KeyUsage != (KeyUsage.DigitalSignature | KeyUsage.KeyAgreement))
            {
                throw new Exception("Extended key usage present. It's used only for TLS.");
            }
            PublicKeyAlgorithm = HashAlgorithmConverter.FromString(((GXAsn1Sequence)seq[1])[0].ToString());
            if (PublicKeyAlgorithm != HashAlgorithm.Sha256WithEcdsa &&
               PublicKeyAlgorithm != HashAlgorithm.Sha384WithEcdsa)
            {
                throw new Exception("DLMS certificate must be signed with ECDSA with SHA256 or SHA384.");
            }
            // Optional.
            if (((GXAsn1Sequence)seq[1]).Count > 1)
            {
                PublicKeyParameters = ((GXAsn1Sequence)seq[1])[1];
            }
            /////////////////////////////
            //Get signature.
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

        private object[] GetDataList()
        {
            if (string.IsNullOrEmpty(Issuer))
            {
                throw new ArgumentNullException("Issuer is empty.");
            }
            if (string.IsNullOrEmpty(Subject))
            {
                throw new ArgumentNullException("Subject is empty.");
            }
            GXAsn1ObjectIdentifier a = new GXAsn1ObjectIdentifier(HashAlgorithmConverter.GetString(SignatureAlgorithm));
            GXAsn1Sequence seq;
            GXAsn1Context p = new GXAsn1Context();
            p.Add((sbyte)Version);
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
            if (AuthorityKeyIdentifier != null || AuthorityCertIssuer != null || AuthorityCertificationSerialNumber != null)
            {
                s1 = new GXAsn1Sequence();
                s1.Add(new GXAsn1ObjectIdentifier(X509CertificateTypeConverter.GetString(Enums.X509CertificateType.AuthorityKeyIdentifier)));
                s.Add(s1);
                GXAsn1Context s2 = new GXAsn1Context() { Index = 3 };
                GXAsn1Sequence c1 = new GXAsn1Sequence();
                if (AuthorityKeyIdentifier != null)
                {
                    GXAsn1Context c4 = new GXAsn1Context() { Constructed = false, Index = 0 };
                    c4.Add(AuthorityKeyIdentifier);
                    c1.Add(c4);
                    s1.Add(GXAsn1Converter.ToByteArray(c1));
                }
                if (AuthorityCertIssuer != null)
                {
                    GXAsn1Context c2 = new GXAsn1Context();
                    c2.Index = 1;
                    c1.Add(c2);
                    GXAsn1Context c3 = new GXAsn1Context() { Index = 4 };
                    c2.Add(c3);
                    c3.Add(GXAsn1Converter.EncodeSubject(AuthorityCertIssuer));
                    s2.Add(c1);
                }
                if (AuthorityCertificationSerialNumber != null)
                {
                    GXAsn1Context c4 = new GXAsn1Context() { Constructed = false, Index = 2 };
                    byte[] tmp5 = AuthorityCertificationSerialNumber.ToByteArray();
                    Array.Reverse(tmp5);
                    c4.Add(tmp5);
                    c1.Add(c4);
                    s1.Add(GXAsn1Converter.ToByteArray(c1));
                }
            }
            // BasicConstraints
            s1 = new GXAsn1Sequence();
            s1.Add(new GXAsn1ObjectIdentifier(X509CertificateTypeConverter.GetString(Enums.X509CertificateType.BasicConstraints)));
            seq = new GXAsn1Sequence();
            if (BasicConstraints)
            {
                //BasicConstraints is critical if it exists.
                s1.Add(BasicConstraints);
            }
            else if (KeyUsage == KeyUsage.None)
            {
                throw new Exception("Key usage not present.");
            }
            s1.Add(GXAsn1Converter.ToByteArray(seq));
            s.Add(s1);
            //KeyUsage
            s1 = new GXAsn1Sequence();
            s1.Add(new GXAsn1ObjectIdentifier(X509CertificateTypeConverter.GetString(X509CertificateType.KeyUsage)));
            byte value = 0;
            int min = 255;
            byte keyUsage = GXCommon.SwapBits((byte)KeyUsage);
            foreach (KeyUsage it in Enum.GetValues(typeof(KeyUsage)))
            {
                if ((((byte)it) & keyUsage) != 0)
                {
                    byte val = (byte)it;
                    value |= val;
                    if (val < min)
                    {
                        min = val;
                    }
                }
            }
            int ignore = 0;
            while ((min >>= 1) != 0)
            {
                ++ignore;
            }
            byte[] tmp = GXAsn1Converter.ToByteArray(new GXAsn1BitString(new byte[] { (byte)(ignore % 8), value }));
            s1.Add(tmp);
            s.Add(s1);
            //extKeyUsage
            if (ExtendedKeyUsage != ExtendedKeyUsage.None)
            {
                s1 = new GXAsn1Sequence();
                s1.Add(new GXAsn1ObjectIdentifier(X509CertificateTypeConverter.GetString(X509CertificateType.ExtendedKeyUsage)));
                GXAsn1Sequence s2 = new GXAsn1Sequence();
                if ((ExtendedKeyUsage & ExtendedKeyUsage.ServerAuth) != 0)
                {
                    s2.Add(new GXAsn1ObjectIdentifier("1.3.6.1.5.5.7.3.1"));
                }
                if ((ExtendedKeyUsage & ExtendedKeyUsage.ClientAuth) != 0)
                {
                    s2.Add(new GXAsn1ObjectIdentifier("1.3.6.1.5.5.7.3.2"));
                }
                tmp = GXAsn1Converter.ToByteArray(s2);
                s1.Add(tmp);
                s.Add(s1);
            }
            GXAsn1Sequence valid = new GXAsn1Sequence();
            valid.Add(ValidFrom);
            valid.Add(ValidTo);
            GXAsn1ObjectIdentifier alg;
            if (PublicKey.Scheme == Ecdsa.Enums.Ecc.P256)
            {
                alg = new GXAsn1ObjectIdentifier("1.2.840.10045.3.1.7");
            }
            else
            {
                alg = new GXAsn1ObjectIdentifier("1.3.132.0.34");
            }
            object[] list;
            object[] tmp3 = new object[]{new GXAsn1ObjectIdentifier("1.2.840.10045.2.1"),
            alg };
            GXAsn1Context tmp4 = new GXAsn1Context();
            tmp4.Index = 3;
            tmp4.Add(s);
            object[] tmp2 = new object[] { tmp3, new GXAsn1BitString(PublicKey.RawValue, 0) };
            object[] p2;
            if (SignatureParameters == null)
            {
                p2 = new object[] { a };
            }
            else
            {
                p2 = new object[] { a, SignatureParameters };
            }
            list = new object[] { p, new GXAsn1Integer(SerialNumber), p2, GXAsn1Converter.EncodeSubject(Issuer), valid, GXAsn1Converter.EncodeSubject(Subject), tmp2, tmp4 };
            return list;
        }

        public byte[] Encoded
        {
            get
            {
                if (rawData != null)
                {
                    return rawData;
                }
                object tmp = new object[] { new GXAsn1ObjectIdentifier(HashAlgorithmConverter.GetString(SignatureAlgorithm)) };
                object[] list = new object[] { GetDataList(), tmp, new GXAsn1BitString(Signature, 0) };
                rawData = GXAsn1Converter.ToByteArray(list);
                return rawData;
            }
        }

        /// <summary>
        /// Get data as byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] GetData()
        {
            return GXAsn1Converter.ToByteArray(GetDataList());
        }

        public override sealed string ToString()
        {
            StringBuilder bb = new StringBuilder();
            if (ExtendedKeyUsage == ExtendedKeyUsage.ServerAuth)
            {
                bb.AppendLine("Server certificate");
            }
            else if (ExtendedKeyUsage == ExtendedKeyUsage.ClientAuth)
            {
                bb.AppendLine("Client certificate");
            }
            else if (ExtendedKeyUsage == (ExtendedKeyUsage.ServerAuth | ExtendedKeyUsage.ClientAuth))
            {
                bb.AppendLine("TLS certificate");
            }
            bb.AppendLine("");
            bb.Append("Version: ");
            bb.AppendLine(Version.ToString());
            bb.Append("SerialNumber: ");
            bb.AppendLine(SerialNumber.ToString());
            bb.Append("Signature: ");
            bb.Append(SignatureAlgorithm.ToString());
            bb.Append(", OID = ");
            bb.Append(HashAlgorithmConverter.GetString(SignatureAlgorithm));
            bb.Append("\n");
            bb.Append("Issuer: ");
            bb.Append(Issuer);
            bb.Append("\n");
            bb.Append("Validity: [From: ");
            bb.Append(ValidFrom.ToUniversalTime().ToString());
            bb.Append(" GMT To: ");
            bb.Append(ValidTo.ToUniversalTime().ToString());
            bb.Append(" GMT]\n");
            bb.Append("Subject: ");
            bb.Append(Subject);
            bb.Append("\n");
            bb.Append("Public Key Algorithm: ");
            bb.Append(PublicKeyAlgorithm.ToString());
            bb.Append("\n");
            bb.Append("Key: ");
            bb.Append(PublicKey.ToHex());
            bb.Append("\n");
            if (PublicKey.Scheme == Ecdsa.Enums.Ecc.P256)
            {
                bb.Append("ASN1 OID: prime256v1\n");
                bb.Append("NIST CURVE: P-256");
            }
            else if (PublicKey.Scheme == Ecdsa.Enums.Ecc.P384)
            {
                bb.Append("ASN1 OID: prime384v1\n");
                bb.Append("\n");
                bb.Append("NIST CURVE: P-384");
            }
            bb.Append("\n");
            bb.Append("Basic constraints: ");
            bb.Append(BasicConstraints);
            bb.Append("\n");
            bb.Append("SubjectKeyIdentifier: ");
            bb.Append(GXCommon.ToHex(SubjectKeyIdentifier, true));
            bb.Append("\n");
            bb.Append("KeyUsage: ");
            bb.Append(KeyUsage);
            bb.Append("\n");
            bb.Append("Signature Algorithm: ");
            bb.Append(SignatureAlgorithm.ToString());
            bb.Append("\n");
            bb.Append("Signature: ");
            bb.Append(GXCommon.ToHex(Signature, false));
            bb.Append("\n");
            return bb.ToString();
        }

        /// <summary>
        /// Load x509 certificate from the PEM file.
        /// </summary>
        /// <param name="path">File path. </param>
        /// <returns>Created GXx509Certificate object. </returns>
        public static GXx509Certificate Load(string path)
        {
            return FromPem(File.ReadAllText(path));
        }

        /// <summary>
        /// x509 certificate to PEM file.
        /// </summary>
        /// <param name="path">File path.</param>
        public void Save(string path)
        {
            File.WriteAllText(path, ToPem());
        }

        /// <summary>
        /// x509 certificate in PEM format.
        /// </summary>
        /// <returns>Public key as in PEM string.</returns>
        public string ToPem()
        {
            StringBuilder sb = new StringBuilder();
            if (PublicKey == null)
            {
                throw new System.ArgumentException("Public or private key is not set.");
            }
            if (!string.IsNullOrEmpty(Description))
            {
                sb.Append("#Description");
                sb.AppendLine(Description);
            }
            sb.AppendLine("-----BEGIN CERTIFICATE-----");
            sb.AppendLine(ToDer());
            sb.AppendLine("-----END CERTIFICATE-----");
            return sb.ToString();
        }

        /// <summary>
        /// x509 certificate in DER format.
        /// </summary>
        /// <returns>Public key as in PEM string.</returns>
        public string ToDer()
        {
            return GXCommon.ToBase64(Encoded);
        }

        public override bool Equals(object obj)
        {
            if (obj is GXx509Certificate o)
            {
                if (SerialNumber == o.SerialNumber)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return SerialNumber.GetHashCode();
        }

        /// <summary>
        /// Test is x509 file certified by the certifier.
        /// </summary>
        /// <param name="certifier">Public key of the certifier.</param>
        /// <returns>True, if certifier has certified the certificate.</returns>
        public bool IsCertified(GXPublicKey certifier)
        {
            if (certifier == null)
            {
                throw new ArgumentNullException(nameof(certifier));
            }
            //Get raw data
            GXByteBuffer tmp2 = new GXByteBuffer();
            tmp2.Set(Encoded);
            GXAsn1Converter.GetNext(tmp2);
            tmp2.Size = tmp2.Position;
            tmp2.Position = 1;
            GXCommon.GetObjectCount(tmp2);
            GXEcdsa e = new GXEcdsa(certifier);
            GXAsn1Sequence tmp3 = (GXAsn1Sequence)GXAsn1Converter.FromByteArray(Signature);
            GXByteBuffer bb = new GXByteBuffer();
            int size = SignatureAlgorithm == HashAlgorithm.Sha256WithEcdsa ? 32 : 48;
            //Some implementations might add extra byte. It must removed.
            bb.Set(((GXAsn1Integer)tmp3[0]).Value, ((GXAsn1Integer)tmp3[0]).Value.Length == size ? 0 : 1, size);
            bb.Set(((GXAsn1Integer)tmp3[1]).Value, ((GXAsn1Integer)tmp3[1]).Value.Length == size ? 0 : 1, size);
            return e.Verify(bb.Array(), tmp2.SubArray(tmp2.Position, tmp2.Available));
        }

        /// <summary>Search x509 Certificate from the PEM file in given folder.
        /// </summary>
        /// <param name="folder">Folder to search. </param>
        /// <returns> Created GXPkcs8 object.</returns>
        public static GXx509Certificate Search(string path, byte[] systemtitle)
        {
            string subject = GXAsn1Converter.SystemTitleToSubject(systemtitle);
            foreach (string it in Directory.GetFiles(path))
            {
                string ext = Path.GetExtension(it);
                if (string.Compare(ext, ".pem", true) == 0 || string.Compare(ext, ".cer", true) == 0)
                {
                    try
                    {
                        GXx509Certificate cert = FromPem(File.ReadAllText(it));
                        if (cert.Subject.Contains(subject))
                        {
                            return cert;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
            }
            return null;
        }

    }
}