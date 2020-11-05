using Gurux.DLMS.ASN.Enums;
using Gurux.DLMS.Ecdsa;
using Gurux.DLMS.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

//
// --------------------------------------------------------------------------
//  Gurux Ltd
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

        public object Attributes
        {
            get;
            private set;
        }

        /// <summary>
        /// Algorithm.
        /// </summary>
        public object Algorithm
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
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Base64 string. </param>
        public GXPkcs10(string data)
        {
            string tmp = data.Replace("-----BEGIN CERTIFICATE REQUEST-----", "");
            tmp = tmp.Replace("-----END CERTIFICATE REQUEST-----", "");
            tmp = tmp.Replace("-----BEGIN NEW CERTIFICATE REQUEST-----", "");
            tmp = tmp.Replace("-----END NEW CERTIFICATE REQUEST-----", "");
            Init(Convert.FromBase64String(tmp.Trim()));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Encoded bytes. </param>
        public GXPkcs10(byte[] data)
        {
            Init(data);
        }
        private void Init(byte[] data)
        {
            GXAsn1Sequence seq = (GXAsn1Sequence)GXAsn1Converter.FromByteArray(data);
            if (seq.Count < 3)
            {
                throw new System.ArgumentException("Wrong number of elements in sequence.");
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
                Attributes = reqInfo[3];
            }
            GXAsn1Sequence tmp = (GXAsn1Sequence)subjectPKInfo[0];
            Algorithm = PkcsObjectIdentifierConverter.FromString(tmp[0].ToString());
            if ((PkcsObjectIdentifier)Algorithm == PkcsObjectIdentifier.None)
            {
                Algorithm = X9ObjectIdentifierConverter.FromString(tmp[0].ToString());
            }
            byte[] encodedKey = GXAsn1Converter.ToByteArray(subjectPKInfo);
            PublicKey = GXPublicKey.FromRawBytes(encodedKey);
            /////////////////////////////
            // signatureAlgorithm
            GXAsn1Sequence sign = (GXAsn1Sequence)seq[1];
            SignatureAlgorithm = HashAlgorithmConverter.FromString(sign[0].ToString());
            if (sign.Count != 1)
            {
                SignatureParameters = sign[1];
            }
            /////////////////////////////
            // signature
            Signature = ((GXAsn1BitString)seq[2]).Value;
            GXEcdsa e = new GXEcdsa(PublicKey);
            if (!e.Verify(GXAsn1Converter.ToByteArray(reqInfo), data))
            {
                throw new ArgumentException("Invalid Signature.");
            }
        }

        public override sealed string ToString()
        {
            StringBuilder bb = new StringBuilder();
            bb.Append("PKCS #10 certificate request:");
            bb.Append("\n");
            bb.Append("Version: ");
            bb.Append(Version.ToString());
            bb.Append("\n");

            bb.Append("Subject: ");
            bb.Append(Subject);
            bb.Append("\n");

            bb.Append("Algorithm: ");
            if (Algorithm != null)
            {
                bb.Append(Algorithm.ToString());
            }
            bb.Append("\n");
            bb.Append("Public Key: ");
            if (PublicKey != null)
            {
                bb.Append(PublicKey.ToString());
            }
            bb.Append("\n");
            bb.Append("Signature algorithm: ");
            bb.Append(SignatureAlgorithm.ToString());
            bb.Append("\n");
            bb.Append("Signature parameters: ");
            if (SignatureParameters != null)
            {
                bb.Append(SignatureParameters.ToString());
            }
            bb.Append("\n");
            bb.Append("Signature: ");
            bb.Append(GXCommon.ToHex(Signature, false));
            bb.Append("\n");
            return bb.ToString();
        }

        private object[] Getdata()
        {
            object subjectPKInfo = GXAsn1Converter.FromByteArray(PublicKey.RawValue);
            object[] list;
            if (Attributes != null)
            {
                list = new object[] { (byte)Version, GXAsn1Converter.EncodeSubject(Subject), subjectPKInfo, Attributes };
            }
            else
            {
                list = new object[] { (byte)Version, GXAsn1Converter.EncodeSubject(Subject), subjectPKInfo, new GXAsn1Context() };
            }
            return list;
        }

        public byte[] Encoded
        {
            get
            {
                if (Signature == null)
                {
                    throw new System.ArgumentException("Sign first.");
                }
                // Certification request info.
                // subject Public key info.
                GXAsn1ObjectIdentifier sa = new GXAsn1ObjectIdentifier(HashAlgorithmConverter.GetString(SignatureAlgorithm));
                object[] list = new object[] { Getdata(), new object[] { sa }, new GXAsn1BitString(Signature, 0) };
                return GXAsn1Converter.ToByteArray(list);
            }
        }

        /// <summary>
        /// Sign
        /// </summary>
        /// <param name="key">Private key. </param>
        /// <param name="HashAlgorithm">Used algorithm for signing. </param>
        void Sign(GXPrivateKey key, HashAlgorithm HashAlgorithm)
        {
            byte[] data = GXAsn1Converter.ToByteArray(Getdata());
            GXEcdsa e = new GXEcdsa(key);
            SignatureAlgorithm = HashAlgorithm;
            Signature = e.Sign(data);
        }

        /// <summary>
        /// Create Certificate Signing Request.
        /// </summary>
        /// <param name="kp">KeyPair </param>
        /// <param name="subject">Subject.</param>
        /// <returns> Created GXPkcs10.</returns>
        public static GXPkcs10 CreateCertificateSigningRequest(KeyValuePair<GXPrivateKey, GXPublicKey> kp, string subject)
        {
            GXPkcs10 pkc10 = new GXPkcs10();
            pkc10.Algorithm = X9ObjectIdentifier.IdECPublicKey;
            pkc10.PublicKey = kp.Value;
            pkc10.Subject = subject;
            pkc10.Sign(kp.Key, HashAlgorithm.Sha256withecdsa);
            return pkc10;
        }

        ///
        /// <summary>
        /// Load public key from the PEM file.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns> Created GXPkcs10 object. </returns>
        ///
        public static GXPkcs10 Load(string path)
        {
            return new GXPkcs10(File.ReadAllText(path));
        }

        /// <summary>
        /// Save public key to PEM file.
        /// </summary>
        /// <param name="path">File path. </param>
        public virtual void Save(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-----BEGIN CERTIFICATE REQUEST-----" + Environment.NewLine);
            sb.Append(GXCommon.ToBase64(Encoded));
            sb.Append(Environment.NewLine + "-----END CERTIFICATE REQUEST-----");
            File.WriteAllText(path, sb.ToString());
        }
    }
}