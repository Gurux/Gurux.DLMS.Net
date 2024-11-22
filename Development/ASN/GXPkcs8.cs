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
using Gurux.DLMS.Ecdsa;
using Gurux.DLMS.Ecdsa.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gurux.DLMS.ASN
{
    /// <summary>
    /// Pkcs8 certification request. Private key is saved using this format.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc5208
    /// </remarks>
    public class GXPkcs8
    {
        /// <summary>
        /// Loaded PKCS #8 certificate as a raw data.
        /// </summary>
        private byte[] _rawData;

        /// <summary>
        /// Returns default file path.
        /// </summary>
        /// <param name="scheme">Used scheme.</param>
        /// <param name="certificateType">Certificate type.</param>
        /// <param name="systemTitle"> System title.</param>
        /// <returns>File path.</returns>
        public static string GetFilePath(Ecc scheme,
            CertificateType certificateType,
            byte[] systemTitle)
        {
            string path;
            switch (certificateType)
            {
                case CertificateType.DigitalSignature:
                    path = "D";
                    break;
                case CertificateType.KeyAgreement:
                    path = "A";
                    break;
                case CertificateType.TLS:
                    path = "T";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown certificate type.");
            }
            path += GXDLMSTranslator.ToHex(systemTitle, false) + ".pem";
            if (scheme == Ecc.P256)
            {
                path = Path.Combine("Keys", path);
            }
            else
            {
                path = Path.Combine("Keys384", path);
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
        /// Private key version.
        /// </summary>
        public CertificateVersion Version
        {
            get;
            set;
        }

        /// <summary>
        /// Algorithm.
        /// </summary>
        private X9ObjectIdentifier Algorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Private key.
        /// </summary>
        public GXPrivateKey PrivateKey
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
        /// Returns default file path.
        /// </summary>
        /// <param name="scheme">Used scheme.</param>
        /// <param name="certificateType">Certificate type.</param>
        /// <returns></returns>
        public string GetFilePath(Ecc scheme, CertificateType certificateType)
        {
            string path;
            switch (certificateType)
            {
                case CertificateType.DigitalSignature:
                    path = "D";
                    break;
                case CertificateType.KeyAgreement:
                    path = "A";
                    break;
                case CertificateType.TLS:
                    path = "T";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown certificate type.");
            }
            path += PrivateKey.ToString().Substring(1) + ".pem";
            if (scheme == Ecc.P256)
            {
                path = Path.Combine("Certificates", path);
            }
            else
            {
                path = Path.Combine("Certificates384", path);
            }
            return path;
        }

        public byte[] Encoded
        {
            get
            {
                if (_rawData != null)
                {
                    return _rawData;
                }
                GXAsn1Sequence d = new GXAsn1Sequence();
                d.Add((sbyte)Version);
                GXAsn1Sequence d1 = new GXAsn1Sequence();
                d1.Add(new GXAsn1ObjectIdentifier(X9ObjectIdentifierConverter.GetString(Algorithm)));
                GXAsn1ObjectIdentifier alg;
                if (PublicKey.Scheme == Ecdsa.Enums.Ecc.P256)
                {
                    alg = new GXAsn1ObjectIdentifier("1.2.840.10045.3.1.7");
                }
                else
                {
                    alg = new GXAsn1ObjectIdentifier("1.3.132.0.34");
                }
                d1.Add(alg);
                d.Add(d1);
                GXAsn1Sequence d2 = new GXAsn1Sequence();
                d2.Add((sbyte)1);
                d2.Add(PrivateKey.RawValue);
                GXAsn1Context d3 = new GXAsn1Context();
                d3.Index = 1;
                d3.Add(new GXBitString(PublicKey.RawValue, 0));
                d2.Add(d3);
                d.Add(GXAsn1Converter.ToByteArray(d2));
                _rawData = GXAsn1Converter.ToByteArray(d);
                return _rawData;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXPkcs8()
        {
            Version = CertificateVersion.Version1;
            Algorithm = X9ObjectIdentifier.IdECPublicKey;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">Private key.</param>
        public GXPkcs8(GXPrivateKey key) : this()
        {
            PrivateKey = key;
            PublicKey = key.GetPublicKey();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pair">Private public key pair.</param>
        public GXPkcs8(KeyValuePair<GXPublicKey, GXPrivateKey> pair) : this()
        {
            PrivateKey = pair.Value;
            PublicKey = pair.Key;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">PEM string.</param>
        [Obsolete("Use FromPem instead.")]
        public GXPkcs8(string data)
        {
            const string START = "PRIVATE KEY-----\n";
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
        /// Create PKCS #8 from PEM string.
        /// </summary>
        /// <param name="data">PEM string.</param>
        public static GXPkcs8 FromPem(string data)
        {
            const string START = "PRIVATE KEY-----\n";
            const string END = "-----END";
            data = data.Replace("\r\n", "\n");
            int start = data.IndexOf(START);
            if (start == -1)
            {
                throw new ArgumentException("Invalid PEM file.");
            }
            string desc = null;
            if (start != START.Length)
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
            GXPkcs8 cert = FromDer(data.Substring(0, end));
            cert.Description = desc;
            return cert;
        }

        /// <summary>
        /// Create PKCS 8 from hex string.
        /// </summary>
        /// <param name="data">Hex string.</param>
        /// <returns>PKCS 8</returns>
        public static GXPkcs8 FromHexString(string data)
        {
            GXPkcs8 cert = new GXPkcs8();
            cert.Init(GXCommon.HexToBytes(data));
            return cert;
        }

        /// <summary>
        /// Create PKCS #8 from DER Base64 encoded string.
        /// </summary>
        /// <param name="der">Base64 DER string.</param>
        /// <returns></returns>
        public static GXPkcs8 FromDer(string der)
        {
            der = der.Replace("\r\n", "");
            der = der.Replace("\n", "");
            GXPkcs8 cert = new GXPkcs8();
            cert.Init(GXCommon.FromBase64(der));
            return cert;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Encoded bytes. </param>
        public GXPkcs8(byte[] data)
        {
            Init(data);
        }

        private void Init(byte[] data)
        {
            _rawData = data;
            GXAsn1Sequence seq = (GXAsn1Sequence)GXAsn1Converter.FromByteArray(data);
            if (seq.Count < 3)
            {
                throw new System.ArgumentException("Wrong number of elements in sequence.");
            }
            if (!(seq[0] is sbyte))
            {
                PkcsType type = GXAsn1Converter.GetCertificateType(data, seq);
                switch (type)
                {
                    case PkcsType.Pkcs10:
                        throw new GXDLMSCertificateException("Invalid Certificate. This is PKCS 10 certification requests, not PKCS 8.");
                    case PkcsType.x509Certificate:
                        throw new GXDLMSCertificateException("Invalid Certificate. This is PKCS x509 certificate, not PKCS 8.");
                }
                throw new GXDLMSCertificateException("Invalid Certificate Version.");
            }
            Version = (CertificateVersion)seq[0];
            if (seq[1] is byte[])
            {
                throw new GXDLMSCertificateException("Invalid Certificate. This looks more like private key, not PKCS 8.");
            }
            GXAsn1Sequence tmp = (GXAsn1Sequence)seq[1];
            Algorithm = X9ObjectIdentifierConverter.FromString(tmp[0].ToString());
            PrivateKey = GXPrivateKey.FromRawBytes((byte[])((GXAsn1Sequence)seq[2])[1]);
            if (PrivateKey == null)
            {
                throw new Exception("Invalid private key.");
            }
            //If public key is not included.
            List<object> tmp2 = (List<object>) ((GXAsn1Sequence)seq[2]);
            if (tmp2.Count > 2)
            {
                PublicKey = GXPublicKey.FromRawBytes(((GXBitString)((List<object>)(tmp2)[2])[0]).Value);
                GXEcdsa.Validate(PublicKey);
            }
            else
            {
                PublicKey = PrivateKey.GetPublicKey();
            }
        }

        public override sealed string ToString()
        {
            StringBuilder bb = new StringBuilder();
            bb.AppendLine("PKCS #8:");
            bb.Append("Version: ");
            bb.AppendLine(Version.ToString());
            bb.Append("Algorithm: ");
            bb.AppendLine(Algorithm.ToString());
            bb.Append("PrivateKey: ");
            bb.AppendLine(PrivateKey.ToHex());
            bb.Append("PublicKey: ");
            bb.AppendLine(PublicKey.ToString());
            return bb.ToString();
        }

        /// <summary>Load private key from the PEM file.
        /// </summary>
        /// <param name="path">File path. </param>
        /// <returns> Created GXPkcs8 object. </returns>
        public static GXPkcs8 Load(string path)
        {
            return FromPem(File.ReadAllText(path));
        }

        /// <summary>
        /// Save private key to PEM file.
        /// </summary>
        /// <param name="path">
        /// File path.
        /// </param>
        public virtual void Save(string path)
        {
            File.WriteAllText(path, ToPem());
        }

        /// <summary>
        /// Private key in PEM format.
        /// </summary>
        /// <returns>Private key as in PEM string.</returns>
        public string ToPem()
        {
            StringBuilder sb = new StringBuilder();
            if (PrivateKey == null)
            {
                throw new System.ArgumentException("Public or private key is not set.");
            }
            if (!string.IsNullOrEmpty(Description))
            {
                sb.Append("#Description");
                sb.AppendLine(Description);
            }
            sb.Append("-----BEGIN PRIVATE KEY-----" + Environment.NewLine);
            sb.Append(ToDer());
            sb.Append(Environment.NewLine + "-----END PRIVATE KEY-----");
            return sb.ToString();
        }

        /// <summary>
        /// Private key in DER format.
        /// </summary>
        /// <returns>Private key as in DER string.</returns>
        public string ToDer()
        {
            return GXCommon.ToBase64(Encoded);
        }

        /// <summary>
        /// Import certificate from string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GXPkcs8 Import(string value)
        {
            GXPrivateKey pk;
            GXPkcs8 ret;
            try
            {
                ret = GXPkcs8.FromPem(value);
            }
            catch (Exception)
            {
                try
                {
                    ret = GXPkcs8.FromDer(value);
                }
                catch (Exception)
                {
                    try
                    {
                        //If PEM.
                        pk = GXPrivateKey.FromPem(value);
                        ret = new GXPkcs8(pk);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            //If DER.
                            pk = GXPrivateKey.FromDer(value);
                            ret = new GXPkcs8(pk);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                //If Raw.
                                pk = GXPrivateKey.FromRawBytes(GXDLMSTranslator.HexToBytes(value));
                                ret = new GXPkcs8(pk);
                            }
                            catch (Exception)
                            {
                                throw new Exception("Invalid private key format.");
                            }
                        }
                    }
                }
            }
            return ret;
        }
        public override bool Equals(object obj)
        {
            if (obj is GXPkcs8 pk)
            {
                if (pk.PrivateKey.Equals(PrivateKey))
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (PrivateKey == null)
            {
                return 0;
            }
            return PrivateKey.GetHashCode();
        }
    }
}