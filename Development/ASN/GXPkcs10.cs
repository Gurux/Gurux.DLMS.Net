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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.ASN
{
    /// <summary>
    /// Pkcs10 Certificate Signing Request.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc2986
    /// </remarks>
    public class GXPkcs10
    {
        /// <summary>
        /// Loaded PKCS #10 certificate as a raw data.
        /// </summary>
        private byte[] rawData;

        /// <summary>
        /// Certificate version.
        /// </summary>
        public CertificateVersion Version
        {
            get;
            private set;
        }

        /// <summary>
        /// Subject.
        /// </summary>
        public string Subject
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of attributes providing additional information about the subject of the certificate.
        /// </summary>
        public List<KeyValuePair<PkcsObjectIdentifier, object[]>> Attributes
        {
            get;
            private set;
        }

        /// <summary>
        /// Algorithm.
        /// </summary>
        public X9ObjectIdentifier Algorithm
        {
            get;
            private set;
        }

        /// <summary>
        /// Subject public key.
        /// </summary>
        public GXPublicKey PublicKey
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
        /// Signature parameters.
        /// </summary>
        public object SignatureParameters
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
        /// Constructor.
        /// </summary>
        public GXPkcs10()
        {
            Algorithm = X9ObjectIdentifier.IdECPublicKey;
            Version = CertificateVersion.Version1;
            Attributes = new List<KeyValuePair<PkcsObjectIdentifier, object[]>>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">PEM string.</param>
        [Obsolete("Use FromPem instead.")]
        public GXPkcs10(string data)
        {
            data = data.Replace("\r\n", "\n");
            const string START = "CERTIFICATE REQUEST-----\n";
            const string END = "-----END CERTIFICATE REQUEST-----";
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
        /// Constructor.
        /// </summary>
        /// <param name="data">Encoded bytes. </param>
        public GXPkcs10(byte[] data)
        {
            Init(data);
        }

        /// <summary>
        /// Create PKCS 10 from hex string.
        /// </summary>
        /// <param name="data">Hex string.</param>
        /// <returns>PKCS 10</returns>
        public static GXPkcs10 FromHexString(string data)
        {
            GXPkcs10 cert = new GXPkcs10();
            cert.Init(GXCommon.HexToBytes(data));
            return cert;
        }

        /// <summary>
        /// Create x509Certificate from PEM string.
        /// </summary>
        /// <param name="data">PEM string.</param>
        public static GXPkcs10 FromPem(string data)
        {
            data = data.Replace("\r\n", "\n");
            const string START = "CERTIFICATE REQUEST-----\n";
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
            return FromDer(data.Substring(0, end).Trim());
        }

        /// <summary>
        /// Create x509Certificate from DER Base64 encoded string.
        /// </summary>
        /// <param name="der">Base64 DER string.</param>
        /// <returns></returns>
        public static GXPkcs10 FromDer(string der)
        {
            der = der.Replace("\r\n", "");
            der = der.Replace("\n", "");
            GXPkcs10 cert = new GXPkcs10();
            cert.Init(GXCommon.FromBase64(der));
            return cert;
        }

        private void Init(byte[] data)
        {
            rawData = data;
            Attributes = new List<KeyValuePair<PkcsObjectIdentifier, object[]>>();
            GXAsn1Sequence seq = (GXAsn1Sequence)GXAsn1Converter.FromByteArray(data);
            if (seq.Count < 3)
            {
                throw new System.ArgumentException("Wrong number of elements in sequence.");
            }
            if (!(seq[0] is GXAsn1Sequence))
            {
                PkcsType type = GXAsn1Converter.GetCertificateType(data, seq);
                switch (type)
                {
                    case PkcsType.Pkcs8:
                        throw new GXDLMSCertificateException("Invalid Certificate. This is PKCS 8, not PKCS 10.");
                    case PkcsType.x509Certificate:
                        throw new GXDLMSCertificateException("Invalid Certificate. This is PKCS x509 certificate, not PKCS 10.");
                }
                throw new GXDLMSCertificateException("Invalid Certificate Version.");
            }
            /////////////////////////////
            // CertificationRequestInfo ::= SEQUENCE {
            // version INTEGER { v1(0) } (v1,...),
            // subject Name,
            // subjectPKInfo SubjectPublicKeyInfo{{ PKInfoAlgorithms }},
            // attributes [0] Attributes{{ CRIAttributes }}
            // }

            GXAsn1Sequence reqInfo = (GXAsn1Sequence)seq[0];
            Version = (CertificateVersion)(sbyte)reqInfo[0];
            Subject = GXAsn1Converter.GetSubject((GXAsn1Sequence)reqInfo[1]);
            // subject Public key info.
            GXAsn1Sequence subjectPKInfo = (GXAsn1Sequence)reqInfo[2];
            if (reqInfo.Count > 3)
            {
                //PkcsObjectIdentifier
                foreach (GXAsn1Sequence it in (GXAsn1Context)reqInfo[3])
                {
                    List<object> values = new List<object>();
                    foreach (object v in (List<object>)((KeyValuePair<object, object>)it[1]).Key)
                    {
                        values.Add(v);
                    }
                    Attributes.Add(new KeyValuePair<PkcsObjectIdentifier, object[]>(PkcsObjectIdentifierConverter.FromString(it[0].ToString()), values.ToArray()));
                }
            }
            GXAsn1Sequence tmp = (GXAsn1Sequence)subjectPKInfo[0];
            Algorithm = X9ObjectIdentifierConverter.FromString(tmp[0].ToString());
            if (Algorithm != X9ObjectIdentifier.IdECPublicKey)
            {
                object algorithm = Algorithm;
                if (Algorithm == X9ObjectIdentifier.None)
                {
                    algorithm = PkcsObjectIdentifierConverter.FromString(tmp[0].ToString());
                    if ((PkcsObjectIdentifier)algorithm == PkcsObjectIdentifier.None)
                    {
                        algorithm = tmp[0].ToString();
                    }
                }
                throw new Exception("Invalid PKCS #10 certificate algorithm. " + algorithm);
            }
            PublicKey = GXPublicKey.FromRawBytes(((GXAsn1BitString)subjectPKInfo[1]).Value);
            GXEcdsa.Validate(PublicKey);
            /////////////////////////////
            // signatureAlgorithm
            GXAsn1Sequence sign = (GXAsn1Sequence)seq[1];
            SignatureAlgorithm = HashAlgorithmConverter.FromString(sign[0].ToString());
            if (SignatureAlgorithm != HashAlgorithm.Sha256WithEcdsa && SignatureAlgorithm != HashAlgorithm.Sha384WithEcdsa)
            {
                throw new GXDLMSCertificateException("Invalid signature algorithm. " + sign[0].ToString());
            }
            if (sign.Count != 1)
            {
                SignatureParameters = sign[1];
            }
            /////////////////////////////
            // signature
            //Get raw data
            GXByteBuffer tmp2 = new GXByteBuffer();
            tmp2.Set(data);
            GXAsn1Converter.GetNext(tmp2);
            tmp2.Size = tmp2.Position;
            tmp2.Position = 1;
            GXCommon.GetObjectCount(tmp2);
            //Get signature.
            Signature = ((GXAsn1BitString)seq[2]).Value;
            GXEcdsa e = new GXEcdsa(PublicKey);
            GXAsn1Sequence tmp3 = (GXAsn1Sequence)GXAsn1Converter.FromByteArray(Signature);
            GXByteBuffer bb = new GXByteBuffer();
            int size = SignatureAlgorithm == HashAlgorithm.Sha256WithEcdsa ? 32 : 48;
#if !WINDOWS_UWP
            //Some implementations might add extra byte. It must removed.
            bb.Set(((GXAsn1Integer)tmp3[0]).Value, ((GXAsn1Integer)tmp3[0]).Value.Length == size ? 0 : 1, size);
            bb.Set(((GXAsn1Integer)tmp3[1]).Value, ((GXAsn1Integer)tmp3[1]).Value.Length == size ? 0 : 1, size);
            if (!e.Verify(bb.Array(), tmp2.SubArray(tmp2.Position, tmp2.Available)))
            {
                throw new ArgumentException("Invalid Signature.");
            }
#endif //!WINDOWS_UWP
        }

        public override sealed string ToString()
        {
            StringBuilder bb = new StringBuilder();
            bb.Append("PKCS #10 certificate request:");
            bb.Append(Environment.NewLine);
            bb.Append("Version: ");
            bb.Append(Version.ToString());
            bb.Append(Environment.NewLine);

            bb.Append("Subject: ");
            bb.Append(Subject);
            bb.Append(Environment.NewLine);

            bb.Append("Algorithm: ");
            bb.Append(Algorithm.ToString());
            bb.Append(Environment.NewLine);
            bb.Append("Public Key: ");
            if (PublicKey != null)
            {
                bb.Append(PublicKey.ToString());
            }
            bb.Append(Environment.NewLine);
            bb.Append("Signature algorithm: ");
            bb.Append(SignatureAlgorithm.ToString());
            bb.Append(Environment.NewLine);
            if (SignatureParameters != null)
            {
                bb.Append("Signature parameters: ");
                bb.Append(SignatureParameters.ToString());
                bb.Append(Environment.NewLine);
            }
            bb.Append("Signature: ");
            bb.Append(GXCommon.ToHex(Signature, false));
            bb.Append(Environment.NewLine);
            return bb.ToString();
        }

        private object[] GetData()
        {
            GXAsn1ObjectIdentifier alg;
            if (PublicKey.Scheme == Ecdsa.Enums.Ecc.P256)
            {
                alg = new GXAsn1ObjectIdentifier("1.2.840.10045.3.1.7");
            }
            else
            {
                alg = new GXAsn1ObjectIdentifier("1.3.132.0.34");
            }
            object subjectPKInfo = new GXAsn1BitString(PublicKey.RawValue, 0);
            object[] tmp = new object[]{new GXAsn1ObjectIdentifier("1.2.840.10045.2.1"),
                alg };
            GXAsn1Context attributes = new GXAsn1Context();
            foreach (KeyValuePair<PkcsObjectIdentifier, object[]> it in Attributes)
            {
                GXAsn1Sequence s = new GXAsn1Sequence();
                s.Add(new GXAsn1ObjectIdentifier(PkcsObjectIdentifierConverter.GetString(it.Key)));
                //Convert object array to list.
                List<object> values = new List<object>();
                foreach (object v in it.Value)
                {
                    values.Add(v);
                }
                s.Add(new KeyValuePair<object, object>(values, null));
                attributes.Add(s);
            }
            return new object[] { (sbyte)Version, GXAsn1Converter.EncodeSubject(Subject), new object[] { tmp, subjectPKInfo }, attributes };
        }

        public byte[] Encoded
        {
            get
            {
                if (rawData != null)
                {
                    return rawData;
                }
                if (Signature == null)
                {
                    throw new System.ArgumentException("Sign first.");
                }
                // Certification request info.
                // subject Public key info.
                GXAsn1ObjectIdentifier sa = new GXAsn1ObjectIdentifier(HashAlgorithmConverter.GetString(SignatureAlgorithm));
                object[] list = new object[] { GetData(), new object[] { sa }, new GXAsn1BitString(Signature, 0) };
                return GXAsn1Converter.ToByteArray(list);
            }
        }

#if !WINDOWS_UWP
        /// <summary>
        /// Sign
        /// </summary>
        /// <param name="key">Private key. </param>
        /// <param name="hashAlgorithm">Used algorithm for signing. </param>
        void Sign(GXPrivateKey key, HashAlgorithm hashAlgorithm)
        {
            byte[] data = GXAsn1Converter.ToByteArray(GetData());
            GXEcdsa e = new GXEcdsa(key);
            SignatureAlgorithm = hashAlgorithm;
            GXByteBuffer bb = new GXByteBuffer();
            bb.Set(e.Sign(data));
            int size = SignatureAlgorithm == HashAlgorithm.Sha256WithEcdsa ? 32 : 48;
            object[] tmp = new object[] { new GXAsn1Integer(bb.SubArray(0, size)), new GXAsn1Integer(bb.SubArray(size, size)) };
            Signature = GXAsn1Converter.ToByteArray(tmp);
        }

        /// <summary>
        /// Create Certificate Signing Request.
        /// </summary>
        /// <param name="kp">KeyPair </param>
        /// <param name="subject">Subject.</param>
        /// <returns> Created GXPkcs10.</returns>
        public static GXPkcs10 CreateCertificateSigningRequest(KeyValuePair<GXPublicKey, GXPrivateKey> kp, string subject)
        {
            if (subject == null || !subject.Contains("CN="))
            {
                throw new ArgumentException(nameof(subject));
            }
            GXPkcs10 pkc10 = new GXPkcs10();
            pkc10.Algorithm = X9ObjectIdentifier.IdECPublicKey;
            pkc10.PublicKey = kp.Key;
            pkc10.Subject = subject;
            pkc10.Sign(kp.Value, kp.Key.Scheme == Ecdsa.Enums.Ecc.P256 ? HashAlgorithm.Sha256WithEcdsa : HashAlgorithm.Sha384WithEcdsa);
            return pkc10;
        }

        /// <summary>
        /// Ask Gurux certificate server to generate the new certificate.
        /// </summary>
        /// <param name="address">Certificate server address.</param>
        /// <param name="certifications">List of certification requests.</param>
        /// <returns>Generated certificate(s).</returns>
        public static GXx509Certificate[] GetCertificate(string address, List<GXCertificateRequest> certifications)
        {
            StringBuilder usage = new StringBuilder();
            foreach (GXCertificateRequest it in certifications)
            {
                if (usage.Length != 0)
                {
                    usage.Append(", ");
                }
                usage.Append("{\"KeyUsage\":");
                switch (it.CertificateType)
                {
                    case CertificateType.DigitalSignature:
                        usage.Append(Convert.ToString((int)KeyUsage.DigitalSignature));
                        break;
                    case CertificateType.KeyAgreement:
                        usage.Append(Convert.ToString((int)KeyUsage.KeyAgreement));
                        break;
                    case CertificateType.TLS:
                        usage.Append(Convert.ToString((int)KeyUsage.DigitalSignature | (int)KeyUsage.KeyAgreement));
                        break;
                    default:
                        throw new Exception("Invalid type.");
                }
                if (it.ExtendedKeyUsage != ExtendedKeyUsage.None)
                {
                    usage.Append(", \"ExtendedKeyUsage\":");
                    usage.Append(Convert.ToString((int)it.ExtendedKeyUsage));
                }
                usage.Append(", \"CSR\":\"");
                usage.Append(it.Certificate.ToDer());
                usage.Append("\"}");
            }
            HttpWebRequest request = HttpWebRequest.Create(address) as HttpWebRequest;
            string der = "{\"Certificates\":[" + usage + "]}";
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(der);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                using (HttpWebResponse webresponse = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(webresponse.GetResponseStream()))
                    {
                        string str = reader.ReadToEnd();
                        int pos = str.IndexOf("[");
                        if (pos == -1)
                        {
                            throw new Exception("Certificates are missing.");
                        }
                        str = str.Substring(pos + 2);
                        pos = str.IndexOf("]");
                        if (pos == -1)
                        {
                            throw new Exception("Certificates are missing.");
                        }
                        str = str.Substring(0, pos - 1);
                        List<GXx509Certificate> list = new List<GXx509Certificate>();
                        string[] tmp = str.Split(new string[] { "\"", "," }, StringSplitOptions.RemoveEmptyEntries);
                        pos = 0;
                        foreach (string it in tmp)
                        {
                            GXx509Certificate x509 = GXx509Certificate.FromDer(it);
                            if (!GXCommon.Compare(certifications[pos].Certificate.PublicKey.RawValue, x509.PublicKey.RawValue))
                            {
                                throw new Exception("Create certificate signingRequest generated wrong public key.");
                            }
                            ++pos;
                            list.Add(x509);
                        }
                        return list.ToArray();
                    }
                }
            }
            catch (WebException ex)
            {
                throw new Exception(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }
        }
#endif //!WINDOWS_UWP

        /// <summary>
        /// Load Pkcs10 Certificate Signing Request from the PEM (.csr) file.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>Created GXPkcs10 object. </returns>
        ///
        public static GXPkcs10 Load(string path)
        {
            return FromPem(File.ReadAllText(path));
        }

        /// <summary>
        /// Save Pkcs #10 Certificate Signing Request to PEM file.
        /// </summary>
        /// <param name="path">File path. </param>
        public virtual void Save(string path)
        {
            File.WriteAllText(path, ToPem());
        }

        /// <summary>
        /// Pkcs #10 Certificate Signing Request in DER format.
        /// </summary>
        /// <returns>Public key as in PEM string.</returns>
        public string ToPem()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-----BEGIN CERTIFICATE REQUEST-----");
            sb.AppendLine(ToDer());
            sb.AppendLine("-----END CERTIFICATE REQUEST-----");
            return sb.ToString();
        }

        /// <summary>
        /// Pkcs #10 Certificate Signing Request in DER format.
        /// </summary>
        /// <returns>Public key as in PEM string.</returns>
        public string ToDer()
        {
            if (rawData != null)
            {
                return GXCommon.ToBase64(rawData);
            }
            return GXCommon.ToBase64(Encoded);
        }
    }
}