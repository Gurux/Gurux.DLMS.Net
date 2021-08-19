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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Secure;
using Gurux.DLMS.ASN;
using Gurux.DLMS.ASN.Enums;
using Gurux.DLMS.Ecdsa;
using Gurux.DLMS.Ecdsa.Enums;
using System.Numerics;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSSecuritySetup
    /// </summary>
    public class GXDLMSSecuritySetup : GXDLMSObject, IGXDLMSBase
    {
        SecurityPolicy _securityPolicy = SecurityPolicy.None;
        private GXDLMSSettings serverSettings;

        /// <summary>
        /// Signing key of the server.
        /// </summary>
        internal KeyValuePair<GXPublicKey, GXPrivateKey>? SigningKey
        {
            get;
            set;
        }

        /// <summary>
        /// Signing key of the server.
        /// </summary>
        internal KeyValuePair<GXPublicKey, GXPrivateKey>? KeyAgreementKey
        {
            get;
            set;
        }

        /// <summary>
        /// TLS pair of the server.
        /// </summary>
        internal KeyValuePair<GXPublicKey, GXPrivateKey>? TlsKey
        {
            get;
            set;
        }

        internal GXx509CertificateCollection ServerCertificates
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSecuritySetup()
            : this("0.0.43.0.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSSecuritySetup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSSecuritySetup(string ln, ushort sn)
        : base(ObjectType.SecuritySetup, ln, sn)
        {
            Version = 1;
            Certificates = new GXDLMSCertificateCollection();
            ServerCertificates = new GXx509CertificateCollection();
        }

        /// <summary>
        /// Block cipher key.
        /// </summary>
        internal byte[] Guek
        {
            get;
            set;
        }

        /// <summary>
        /// Broadcast block cipher key.
        /// </summary>
        internal byte[] Gbek
        {
            get;
            set;
        }

        /// <summary>
        /// Authentication key.
        /// </summary>
        internal byte[] Gak
        {
            get;
            set;
        }

        /// <summary>
        /// Master key.
        /// </summary>
        internal byte[] Kek
        {
            get;
            set;
        }

        /// <summary>
        /// Security policy for Version 0 and 1.
        /// </summary>
        [XmlIgnore()]
        public SecurityPolicy SecurityPolicy
        {
            get
            {
                return _securityPolicy;
            }
            set
            {
                if (Version == 0)
                {
                    switch (value)
                    {
                        case SecurityPolicy.None:
                        case SecurityPolicy.Authenticated:
                        case SecurityPolicy.Encrypted:
                        case SecurityPolicy.AuthenticatedEncrypted:
                            break;
                        default:
                            throw new Exception(string.Format("Invalid security policy value {0} for Version 0.", value));
                    }
                }
                else if (Version == 1)
                {
                    if (((Convert.ToByte(value)) & 0x3) != 0)
                    {
                        throw new Exception(string.Format("Invalid security policy value {0} for version 1.", value));
                    }
                }
                _securityPolicy = value;
            }
        }

        /// <summary>
        /// Security suite.
        /// </summary>
        [XmlIgnore()]
        public SecuritySuite SecuritySuite
        {
            get;
            set;
        }

        /// <summary>
        /// Client system title.
        /// </summary>
        [XmlIgnore()]
        public byte[] ClientSystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Server system title.
        /// </summary>
        [XmlIgnore()]
        public byte[] ServerSystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Available certificates.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSCertificateCollection Certificates
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, SecurityPolicy, SecuritySuite,
                              ClientSystemTitle, ServerSystemTitle, Certificates};
        }

        /// <summary>
        /// Get security enum as integer value.
        /// </summary>
        /// <param name="security">Security level.</param>
        /// <returns>Integer value of security level.</returns>
        private static int GetSecurityValue(Security security)
        {
            int value;
            switch (security)
            {
                case Security.None:
                    value = 0;
                    break;
                case Security.Authentication:
                    value = 1;
                    break;
                case Security.Encryption:
                    value = 2;
                    break;
                case Security.AuthenticationEncryption:
                    value = 3;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid Security enum.");
            }
            return value;
        }

        /// <summary>
        /// Activates and strengthens the security policy.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="security">New security level.</param>
        /// <returns>Generated action.</returns>
        public byte[][] Activate(GXDLMSClient client, Security security)
        {
            return client.Method(this, 1, GetSecurityValue(security), DataType.Enum);
        }

        /// <summary>
        /// Activates and strengthens the security policy.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="security">New security level.</param>
        /// <returns>Generated action.</returns>
        public byte[][] Activate(GXDLMSClient client, SecurityPolicy policy)
        {
            return client.Method(this, 1, (byte)policy, DataType.Enum);
        }

        /// <summary>
        /// Updates one or more global keys.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="kek">Master key, also known as Key Encrypting Key.</param>
        /// <param name="list">List of Global key types and keys.</param>
        /// <returns>Generated action.</returns>
        public byte[][] GlobalKeyTransfer(GXDLMSClient client, byte[] kek, List<KeyValuePair<GlobalKeyType, byte[]>> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentException("Invalid list. It is empty.");
            }
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Array);
            bb.SetUInt8((byte)list.Count);
            byte[] tmp;
            foreach (KeyValuePair<GlobalKeyType, byte[]> it in list)
            {
                bb.SetUInt8(DataType.Structure);
                bb.SetUInt8(2);
                GXCommon.SetData(client.Settings, bb, DataType.Enum, it.Key);
                tmp = GXDLMSSecureClient.Encrypt(kek, it.Value);
                GXCommon.SetData(client.Settings, bb, DataType.OctetString, tmp);
            }
            return client.Method(this, 2, bb.Array(), DataType.Array);
        }

        /// <summary>
        /// Agree on one or more symmetric keys using the key agreement algorithm.
        /// </summary>
        /// <param name="client"> DLMS client that is used to generate action.</param>
        /// <param name="list"> List of keys.</param>
        /// <returns>Generated action.</returns>
        public byte[][] KeyAgreement(GXDLMSClient client, List<KeyValuePair<GlobalKeyType, byte[]>> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentException("Invalid list. It is empty.");
            }
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Array);
            bb.SetUInt8((byte)list.Count);
            foreach (KeyValuePair<GlobalKeyType, byte[]> it in list)
            {
                bb.SetUInt8(DataType.Structure);
                bb.SetUInt8(2);
                GXCommon.SetData(client.Settings, bb, DataType.Enum, it.Key);
                GXCommon.SetData(client.Settings, bb, DataType.OctetString, it.Value);
            }
            return client.Method(this, 3, bb.Array(), DataType.Array);
        }

        /// <summary>
        /// Agree on global unicast encryption key.
        /// </summary>
        /// <param name="client"> DLMS client that is used to generate action.</param>
        /// <param name="key"> List of keys.</param>
        /// <returns>Generated action.</returns>
        public byte[][] KeyAgreement(GXDLMSSecureClient client)
        {
            if (Version == 0)
            {
                throw new ArgumentException("Public and private key isn't implemented for version 0.");
            }
            if (client.Ciphering.EphemeralKeyPair.Value == null)
            {
                throw new ArgumentException("Invalid Ephemeral key.");
            }
            if (client.Ciphering.SigningKeyPair.Value == null)
            {
                throw new ArgumentException("Invalid Signiture key.");
            }
            GXByteBuffer bb = new GXByteBuffer();
            //Add Ephemeral public key.
            bb.Set(client.Ciphering.EphemeralKeyPair.Value.RawValue, 1, client.Ciphering.EphemeralKeyPair.Value.RawValue.Length - 1);
            //Add signature.
            byte[] sign = GXSecure.GetEphemeralPublicKeySignature(0, client.Ciphering.EphemeralKeyPair.Key,
                client.Ciphering.SigningKeyPair.Value);
            bb.Set(sign);
            List<KeyValuePair<GlobalKeyType, byte[]>> list = new List<KeyValuePair<GlobalKeyType, byte[]>>();
            list.Add(new KeyValuePair<GlobalKeyType, byte[]>(GlobalKeyType.UnicastEncryption, bb.Array()));
            return KeyAgreement(client, list);
        }

        /// <summary>
        ///  Generates an asymmetric key pair as required by the security suite.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="type">New certificate type.</param>
        /// <returns>Generated action.</returns>
        public byte[][] GenerateKeyPair(GXDLMSClient client, CertificateType type)
        {
            return client.Method(this, 4, type, DataType.Enum);
        }

        /// <summary>
        ///  Ask Server sends the Certificate Signing Request (CSR) data.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="type">identifies the key pair for which the certificate will be requested.</param>
        /// <returns>Generated action.</returns>
        public byte[][] GenerateCertificate(GXDLMSClient client, CertificateType type)
        {
            return client.Method(this, 5, type, DataType.Enum);
        }

        /// <summary>
        ///  Imports an X.509 v3 certificate of a public key.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="key">Public key.</param>
        /// <returns>Generated action.</returns>
        [Obsolete("Use ImportCertificate instead.")]
        public byte[][] Import(GXDLMSClient client, GXx509Certificate certificate)
        {
            return ImportCertificate(client, certificate.Encoded);
        }

        /// <summary>
        ///  Imports an X.509 v3 certificate of a public key.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="key">Public key.</param>
        /// <returns>Generated action.</returns>
        public byte[][] ImportCertificate(GXDLMSClient client, GXx509Certificate certificate)
        {
            return ImportCertificate(client, certificate.Encoded);
        }

        /// <summary>
        ///  Imports an X.509 v3 certificate of a public key.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="key">Public key.</param>
        /// <returns>Generated action.</returns>
        public byte[][] ImportCertificate(GXDLMSClient client, byte[] key)
        {
            return client.Method(this, 6, key, DataType.OctetString);
        }

        /// <summary>
        /// Exports an X.509 v3 certificate from the server using entity information.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="entity">Certificate entity.</param>
        /// <param name="type">Certificate type.</param>
        /// <param name="systemTitle">System title.</param>
        /// <returns>Generated action.</returns>
        public byte[][] ExportCertificateByEntity(GXDLMSClient client, CertificateEntity entity, CertificateType type, byte[] systemTitle)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            //Add enum
            bb.SetUInt8(DataType.Enum);
            bb.SetUInt8(0);
            //Add certificate_identification_by_entity
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(3);
            //Add certificate_entity
            bb.SetUInt8(DataType.Enum);
            bb.SetUInt8(entity);
            //Add certificate_type
            bb.SetUInt8(DataType.Enum);
            bb.SetUInt8(type);
            //system_title
            GXCommon.SetData(client.Settings, bb, DataType.OctetString, systemTitle);
            return client.Method(this, 7, bb.Array(), DataType.Structure);
        }

        /// <summary>
        /// Exports an X.509 v3 certificate from the server using serial information.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="serialNumber">Serial number.</param>
        /// <param name="issuer">Issuer</param>
        /// <returns>Generated action.</returns>
        public byte[][] ExportCertificateBySerial(GXDLMSClient client, BigInteger serialNumber, byte[] issuer)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            //Add enum
            bb.SetUInt8(DataType.Enum);
            bb.SetUInt8(1);
            //Add certificate_identification_by_entity
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            //serialNumber
            byte[] sn = serialNumber.ToByteArray();
            Array.Reverse(sn);
            GXCommon.SetData(client.Settings, bb, DataType.OctetString, sn);
            //issuer
            GXCommon.SetData(client.Settings, bb, DataType.OctetString, issuer);
            return client.Method(this, 7, bb.Array(), DataType.Structure);
        }

        /// <summary>
        /// Removes X.509 v3 certificate from the server using entity.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="entity">Certificate entity type.</param>
        /// <param name="type">Certificate type.</param>
        /// <param name="systemTitle">System title.</param>
        /// <returns>Generated action.</returns>
        public byte[][] RemoveCertificateByEntity(GXDLMSClient client, CertificateEntity entity, CertificateType type, byte[] systemTitle)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            //Add enum
            bb.SetUInt8(DataType.Enum);
            bb.SetUInt8(0);
            //Add certificate_identification_by_entity
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(3);
            //Add certificate_entity
            bb.SetUInt8(DataType.Enum);
            bb.SetUInt8(entity);
            //Add certificate_type
            bb.SetUInt8(DataType.Enum);
            bb.SetUInt8(type);
            //system_title
            GXCommon.SetData(client.Settings, bb, DataType.OctetString, systemTitle);
            return client.Method(this, 8, bb.Array(), DataType.Structure);
        }

        /// <summary>
        /// Removes X.509 v3 certificate from the server using serial number.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="serialNumber">Serial number.</param>
        /// <param name="issuer">Issuer.</param>
        /// <returns>Generated action.</returns>
        public byte[][] RemoveCertificateBySerial(GXDLMSClient client, BigInteger serialNumber, byte[] issuer)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            //Add enum
            bb.SetUInt8(DataType.Enum);
            bb.SetUInt8(1);
            //Add certificate_identification_by_entity
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            //serialNumber
            byte[] sn = serialNumber.ToByteArray();
            Array.Reverse(sn);
            GXCommon.SetData(client.Settings, bb, DataType.OctetString, sn);
            //issuer
            GXCommon.SetData(client.Settings, bb, DataType.OctetString, issuer);
            return client.Method(this, 8, bb.Array(), DataType.Structure);
        }

        /// <summary>
        /// Update ephemeral keys.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>List of Parsed key id and GUAK. This is for debugging purpose.</returns>
        public List<KeyValuePair<GlobalKeyType, byte[]>> UpdateEphemeralKeys(GXDLMSSecureClient client, byte[] value)
        {
            return UpdateEphemeralKeys(client, new GXByteBuffer(value));
        }

        /// <summary>
        /// Update ephemeral keys.
        /// </summary>
        /// <param name="client">DLMS Client.</param>
        /// <param name="value">Received reply from the server.</param>
        /// <returns>List of Parsed key id and GUAK. This is for debugging purpose.</returns>
        public List<KeyValuePair<GlobalKeyType, byte[]>> UpdateEphemeralKeys(GXDLMSSecureClient client, GXByteBuffer value)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (value.GetUInt8() != (byte)DataType.Array)
            {
                throw new ArgumentOutOfRangeException("Invalid tag.");
            }
            GXEcdsa c = new GXEcdsa(client.Ciphering.EphemeralKeyPair.Key);
            int count = GXCommon.GetObjectCount(value);
            List<KeyValuePair<GlobalKeyType, byte[]>> list = new List<KeyValuePair<GlobalKeyType, byte[]>>();
            for (int pos = 0; pos != count; ++pos)
            {
                if (value.GetUInt8() != (byte)DataType.Structure)
                {
                    throw new ArgumentOutOfRangeException("Invalid tag.");
                }
                if (value.GetUInt8() != 2)
                {
                    throw new ArgumentOutOfRangeException("Invalid length.");
                }
                if (value.GetUInt8() != (byte)DataType.Enum)
                {
                    throw new ArgumentOutOfRangeException("Invalid key id data type.");
                }
                int keyId = value.GetUInt8();
                if (keyId > 4)
                {
                    throw new ArgumentOutOfRangeException("Invalid key type.");
                }
                if (value.GetUInt8() != (byte)DataType.OctetString)
                {
                    throw new ArgumentOutOfRangeException("Invalid tag.");
                }
                if (GXCommon.GetObjectCount(value) != 128)
                {
                    throw new ArgumentOutOfRangeException("Invalid length.");
                }
                //Get ephemeral public key server.
                GXByteBuffer key = new GXByteBuffer();
                key.SetUInt8(4);
                key.Set(value, 64);
                GXPublicKey targetEphemeralKey = GXPublicKey.FromRawBytes(key.Array());
                //Get ephemeral public key signature server.
                byte[] signature = new byte[64];
                value.Get(signature);
                key.SetUInt8(0, (byte)keyId);
                //Verify signature.
                if (!GXSecure.ValidateEphemeralPublicKeySignature(key.Array(), signature, client.Ciphering.SigningKeyPair.Key))
                {
                    throw new GXDLMSCipherException("Invalid signature.");
                }
                byte[] z = c.GenerateSecret(targetEphemeralKey);
                System.Diagnostics.Debug.WriteLine("Shared secret:" + GXCommon.ToHex(z, true));
                GXByteBuffer kdf = new GXByteBuffer();
                kdf.Set(GXSecure.GenerateKDF(client.SecuritySuite, z,
                    AlgorithmId.AesGcm128,
                    client.Ciphering.SystemTitle,
                    client.Settings.SourceSystemTitle, null, null));
                System.Diagnostics.Debug.WriteLine("KDF:" + kdf.ToString());
                list.Add(new KeyValuePair<GlobalKeyType, byte[]>((GlobalKeyType)keyId, kdf.SubArray(0, 16)));
            }
            //Update ephemeral keys.
            foreach (KeyValuePair<GlobalKeyType, byte[]> it in list)
            {
                switch (it.Key)
                {
                    case GlobalKeyType.UnicastEncryption:
                        client.Settings.EphemeralBlockCipherKey = it.Value;
                        break;
                    case GlobalKeyType.BroadcastEncryption:
                        client.Settings.EphemeralBroadcastBlockCipherKey = it.Value;
                        break;
                    case GlobalKeyType.Authentication:
                        client.Settings.EphemeralAuthenticationKey = it.Value;
                        break;
                    case GlobalKeyType.Kek:
                        client.Settings.EphemeralKek = it.Value;
                        break;
                }
            }
            return list;
        }

        #region IGXDLMSBase Members

        private static KeyUsage CertificateTypeToKeyUsage(CertificateType type)
        {
            KeyUsage k;
            switch (type)
            {
                case CertificateType.DigitalSignature:
                    k = KeyUsage.DigitalSignature;
                    break;
                case CertificateType.KeyAgreement:
                    k = KeyUsage.KeyAgreement;
                    break;
                case CertificateType.TLS:
                    k = KeyUsage.KeyCertSign;
                    break;
                case CertificateType.Other:
                    k = KeyUsage.CrlSign;
                    break;
                default:
                    // At least one bit must be used.
                    k = KeyUsage.None;
                    break;
            }
            return k;
        }

        /// <summary>
        /// Find certificate using entity information.
        /// </summary>
        /// <param name="certificates">Certificate collection.</param>
        /// <param name="entity">Certificate entity type.</param>
        /// <param name="type">Certificate type.</param>
        /// <param name="systemtitle">System title.</param>
        /// <returns></returns>
        private static GXx509Certificate FindCertificateByEntity(GXx509CertificateCollection certificates, CertificateEntity entity, CertificateType type, byte[] systemtitle)
        {
            String subject = GXAsn1Converter.SystemTitleToSubject(systemtitle);
            KeyUsage k = CertificateTypeToKeyUsage(type);
            foreach (GXx509Certificate it in certificates)
            {
                if ((it.KeyUsage & k) != 0 && it.Subject == subject)
                {
                    return it;
                }
            }
            return null;
        }

        /// <summary>
        /// Find certificate using serial information.
        /// </summary>
        /// <param name="certificates">Certificate collection.</param>
        /// <param name="serialNumber">Serial number.</param>
        /// <param name="issuer">Issuer.</param>
        /// <returns></returns>
        private static GXx509Certificate FindCertificateBySerial(GXx509CertificateCollection certificates, byte[] serialNumber, string issuer)
        {
            foreach (GXx509Certificate it in certificates)
            {
                if (GXCommon.ToHex(it.SerialNumber.ToByteArray(), false) == GXCommon.ToHex(serialNumber, false) && it.Issuer == issuer)
                {
                    return it;
                }
            }
            return null;
        }

        private static Ecc GetEcc(SecuritySuite suite)
        {
            if (suite == Enums.SecuritySuite.Suite1)
            {
                return Ecc.P256;
            }
            return Ecc.P384;
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            try
            {
                if (e.Index == 1)
                {
                    SecurityPolicy = (SecurityPolicy)e.Parameters;
                }
                else if (e.Index == 2)
                {
                    KeyTransfer(settings, e);
                }
                else if (e.Index == 3)
                {
                    return InvokeKeyAgreement(settings, e);
                }
                else if (e.Index == 4)
                {
                    GenerateKeyPair(settings, e);
                }
                else if (e.Index == 5)
                {
                    return GenerateCertificateRequest(settings, e);
                }
                else if (e.Index == 6)
                {
                    ImportCertificate(settings, e);
                }
                else if (e.Index == 7)
                {
                    return ExportCertificate(settings, e);
                }
                else if (e.Index == 8)
                {
                    // remove_certificate
                    RemoveCertificate(settings, e);
                }
                else
                {
                    e.Error = ErrorCode.ReadWriteDenied;
                }
            }
            catch (Exception)
            {
                e.Error = ErrorCode.InconsistentClass;
            }
            //Return standard reply.
            return null;
        }

        private void RemoveCertificate(GXDLMSSettings settings, ValueEventArgs e)
        {
            List<Object> tmp = (List<Object>)((List<object>)e.Parameters)[0];
            short type = (short)tmp[0];
            tmp = (List<Object>)tmp[1];
            GXx509Certificate cert = null;
            if (type == 0)
            {
                cert = FindCertificateByEntity(ServerCertificates, (CertificateEntity)tmp[1], (CertificateType)tmp[2], (byte[])tmp[3]);
            }
            else if (type == 1)
            {
                Array.Reverse((byte[])tmp[0]);
                cert = ServerCertificates.FindBySerial(new BigInteger((byte[])tmp[0]), ASCIIEncoding.ASCII.GetString((byte[])tmp[1]));
            }
            if (cert == null)
            {
                e.Error = ErrorCode.InconsistentClass;
            }
            else
            {
                ServerCertificates.Remove(cert);
            }
        }

        private void ImportCertificate(GXDLMSSettings settings, ValueEventArgs e)
        {
            GXx509Certificate cert = new GXx509Certificate((byte[])e.Parameters);
            byte[] st = ServerSystemTitle;
            if (st == null)
            {
                st = settings.Cipher.SystemTitle;
            }
            String serverSubject = GXAsn1Converter.SystemTitleToSubject(st);
            // If server certification is added.
            bool isServerCert = cert.Subject.Contains(serverSubject);
            if (cert.KeyUsage != KeyUsage.KeyAgreement && cert.KeyUsage != KeyUsage.DigitalSignature
                    && cert.KeyUsage != (KeyUsage.KeyAgreement | KeyUsage.DigitalSignature))
            {
                // At least one bit must be used.
                e.Error = ErrorCode.InconsistentClass;
            }
            else
            {
                // Remove old certificate if it exists.
                List<GXx509Certificate> list = ServerCertificates.GetCertificates(cert.KeyUsage);
                foreach (GXx509Certificate it in list)
                {
                    bool isServer = it.Subject.Contains(serverSubject);
                    if (isServer == isServerCert)
                    {
                        ServerCertificates.Remove(it);
                    }
                }
            }
            ServerCertificates.Add(cert);
            System.Diagnostics.Debug.WriteLine("New certificate imported: " + cert.SerialNumber);
        }

        private byte[] ExportCertificate(GXDLMSSettings settings, ValueEventArgs e)
        {
            List<Object> tmp = (List<Object>)e.Parameters;
            byte type = Convert.ToByte(tmp[0]);
            GXx509Certificate cert = null;
            tmp = (List<Object>)tmp[1];
            lock (ServerCertificates)
            {
                if (type == 0)
                {
                    cert = FindCertificateByEntity(ServerCertificates, (CertificateEntity)Convert.ToByte(tmp[0]), (CertificateType)Convert.ToByte(tmp[1]), (byte[])tmp[2]);
                }
                else if (type == 1)
                {
                    Array.Reverse((byte[])tmp[0]);
                    cert = ServerCertificates.FindBySerial(new BigInteger((byte[])tmp[0]), ASCIIEncoding.ASCII.GetString((byte[])tmp[1]));
                }
                if (cert == null)
                {
                    e.Error = ErrorCode.InconsistentClass;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Export certificate: " + cert.SerialNumber);
                    return cert.Encoded;
                }
            }
            return null;
        }

        private void GenerateKeyPair(GXDLMSSettings settings, ValueEventArgs e)
        {
            CertificateType key = (CertificateType)Convert.ToInt32(e.Parameters);
            KeyValuePair<GXPublicKey, GXPrivateKey> value = GXEcdsa.GenerateKeyPair(GetEcc(SecuritySuite));
            switch (key)
            {
                case CertificateType.DigitalSignature:
                    SigningKey = value;
                    // If all associations are using same certifications.
                    if (settings.AssociationsShareCertificates)
                    {
                        settings.Cipher.SigningKeyPair = value;
                    }
                    break;
                case CertificateType.KeyAgreement:
                    KeyAgreementKey = value;
                    // If all associations are using same certifications.
                    if (settings.AssociationsShareCertificates)
                    {
                        settings.Cipher.KeyAgreementKeyPair = value;
                    }
                    break;
                default:
                    TlsKey = value;
                    // If all associations are using same certifications.
                    if (settings.AssociationsShareCertificates)
                    {
                        settings.Cipher.TlsKeyPair = value;
                    }
                    break;
            }
        }

        private byte[] GenerateCertificateRequest(GXDLMSSettings settings, ValueEventArgs e)
        {
            CertificateType key = (CertificateType)Convert.ToInt32(e.Parameters);
            KeyValuePair<GXPublicKey, GXPrivateKey> kp = default(KeyValuePair<GXPublicKey, GXPrivateKey>);
            byte[] st = ServerSystemTitle;
            if (st == null)
            {
                st = settings.Cipher.SystemTitle;
            }

            switch (key)
            {
                case CertificateType.DigitalSignature:
                    kp = SigningKey.Value;
                    break;
                case CertificateType.KeyAgreement:
                    kp = KeyAgreementKey.Value;
                    break;
                case CertificateType.TLS:
                    kp = TlsKey.Value;
                    break;
                default:
                    break;
            }
            if (kp.Key != null)
            {
                GXPkcs10 pkc10 = GXPkcs10.CreateCertificateSigningRequest(kp, GXAsn1Converter.SystemTitleToSubject(st));
                return pkc10.Encoded;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            return null;
        }

        private void KeyTransfer(GXDLMSSettings settings, ValueEventArgs e)
        {
            // Keys are take in action after reply is generated.
            foreach (List<object> item in e.Parameters as List<object>)
            {
                GlobalKeyType type = (GlobalKeyType)Convert.ToInt32(item[0]);
                byte[] data = (byte[])item[1];
                //if settings.Cipher is null non secure server is used.
                //Keys are take in action after reply is generated.
                switch (type)
                {
                    case GlobalKeyType.UnicastEncryption:
                        Gbek = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                        break;
                    case GlobalKeyType.BroadcastEncryption:
                        Guek = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                        break;
                    case GlobalKeyType.Authentication:
                        Gak = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                        break;
                    case GlobalKeyType.Kek:
                        Kek = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                        break;
                    default:
                        //Invalid type
                        e.Error = ErrorCode.InconsistentClass;
                        break;
                }
            }
        }

        private byte[] InvokeKeyAgreement(GXDLMSSettings settings, ValueEventArgs e)
        {
            List<Object> tmp = (List<Object>)(e.Parameters as List<Object>)[0];
            byte keyId = (byte)tmp[0];
            if (keyId != 0)
            {
                e.Error = ErrorCode.InconsistentClass;
            }
            else
            {
                byte[] data = (byte[])tmp[0];
                // ephemeral public key
                GXByteBuffer data2 = new GXByteBuffer(65);
                data2.SetUInt8(keyId);
                data2.Set(data, 0, 64);
                GXByteBuffer sign = new GXByteBuffer();
                sign.Set(data, 64, 64);
                GXPublicKey pk = null;
                string subject = GXAsn1Converter.SystemTitleToSubject(settings.SourceSystemTitle);
                foreach (GXx509Certificate it in settings.Cipher.Certificates)
                {
                    if ((it.KeyUsage & KeyUsage.DigitalSignature) != 0 && it.Subject == subject)
                    {
                        pk = it.PublicKey;
                        break;
                    }
                }
                if (pk == null) //TODO:|| !GXSecure.ValidateEphemeralPublicKeySignature(data2.Array(), sign.Array(), pk))
                {
                    e.Error = ErrorCode.InconsistentClass;
                    settings.TargetEphemeralKey = null;
                }
                else
                {
                    settings.TargetEphemeralKey = GXPublicKey.FromRawBytes(data2.SubArray(1, 64));
                    // Generate ephemeral keys.
                    KeyValuePair<GXPublicKey, GXPrivateKey> eKpS = settings.Cipher.EphemeralKeyPair;
                    if (eKpS.Key == null)
                    {
                        eKpS = GXEcdsa.GenerateKeyPair(GetEcc(SecuritySuite));
                        settings.Cipher.EphemeralKeyPair = eKpS;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Start to use new keys after reply is generated.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="e"></param>
        internal void ApplyKeys(GXDLMSSettings settings, ValueEventArgs e)
        {
            try
            {
                foreach (List<object> item in e.Parameters as List<object>)
                {
                    GlobalKeyType type = (GlobalKeyType)Convert.ToInt32(item[0]);
                    byte[] data = (byte[])item[1];
                    switch (type)
                    {
                        case GlobalKeyType.UnicastEncryption:
                            settings.Cipher.BlockCipherKey = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                            break;
                        case GlobalKeyType.BroadcastEncryption:
                            //Invalid type
                            e.Error = ErrorCode.ReadWriteDenied;
                            break;
                        case GlobalKeyType.Authentication:
                            //if settings.Cipher is null non secure server is used.
                            settings.Cipher.AuthenticationKey = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                            break;
                        case GlobalKeyType.Kek:
                            settings.Kek = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                            break;
                        default:
                            //Invalid type
                            e.Error = ErrorCode.ReadWriteDenied;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //SecurityPolicy
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //SecuritySuite
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //ClientSystemTitle
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //ServerSystemTitle
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            if (Version > 0)
            {
                //Certificates
                if (all || CanRead(6))
                {
                    attributes.Add(6);
                }
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            List<string> list = new List<string>();
            list.Add(Internal.GXCommon.GetLogicalNameString());
            list.Add("Security Policy");
            list.Add("Security Suite");
            list.Add("Client System Title");
            list.Add("Server System Title");
            if (Version > 0)
            {
                list.Add("Certificates");
            }
            return list.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            List<string> list = new List<string>();
            list.Add("Security activate");
            list.Add("Key transfer");
            if (Version > 0)
            {
                list.Add("Key agreement");
                list.Add("Generate key pair");
                list.Add("Generate certificate request");
                list.Add("Import certificate");
                list.Add("Export certificate");
                list.Add("Remove certificate");
            }
            return list.ToArray();
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 1;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            if (Version == 0)
            {
                return 5;
            }
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            if (Version == 0)
            {
                return 2;
            }
            return 8;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.Enum;
            }
            if (index == 3)
            {
                return DataType.Enum;
            }
            if (index == 4)
            {
                return DataType.OctetString;
            }
            if (index == 5)
            {
                return DataType.OctetString;
            }
            if (index == 6)
            {
                return DataType.Array;
            }
            else
            {
                throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        /// <summary>
        /// Get sertificates as byte buffer.
        /// </summary>
        /// <returns></returns>
        private byte[] GetSertificates(GXDLMSSettings settings)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8((byte)DataType.Array);
            GXCommon.SetObjectCount(ServerCertificates.Count, bb);
            foreach (GXx509Certificate it in ServerCertificates)
            {
                bb.SetUInt8(DataType.Structure);
                GXCommon.SetObjectCount(6, bb);
                bb.SetUInt8(DataType.Enum);
                if (it.BasicConstraints)
                {
                    bb.SetUInt8(CertificateEntity.CertificationAuthority);
                }
                else if (it.Subject.Contains(GXAsn1Converter.SystemTitleToSubject(ServerSystemTitle)))
                {
                    bb.SetUInt8(CertificateEntity.Server);
                }
                else
                {
                    bb.SetUInt8(CertificateEntity.Client);
                }
                bb.SetUInt8(DataType.Enum);
                if (it.KeyUsage == (KeyUsage.DigitalSignature | KeyUsage.KeyAgreement))
                {
                    bb.SetUInt8(CertificateType.TLS);
                }
                else if (it.KeyUsage == KeyUsage.DigitalSignature)
                {
                    bb.SetUInt8(CertificateType.DigitalSignature);
                }
                else if (it.KeyUsage == KeyUsage.KeyAgreement)
                {
                    bb.SetUInt8(CertificateType.KeyAgreement);
                }
                else
                {
                    bb.SetUInt8(CertificateType.Other);
                }
                bb.SetUInt8(DataType.OctetString);
                byte[] tmp = it.SerialNumber.ToByteArray();
                Array.Reverse(tmp);
                bb.SetUInt8((byte)tmp.Length);
                bb.Set(tmp);
                GXCommon.AddString(it.Issuer, bb);
                GXCommon.AddString(it.Subject, bb);
                GXCommon.AddString("", bb);
            }
            return bb.Array();
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return SecurityPolicy;
            }
            if (e.Index == 3)
            {
                return SecuritySuite;
            }
            if (e.Index == 4)
            {
                return ClientSystemTitle;
            }
            if (e.Index == 5)
            {
                return ServerSystemTitle;
            }
            if (e.Index == 6)
            {
                return GetSertificates(settings);
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        private void UpdateSertificates(IEnumerable<object> list)
        {
            Certificates.Clear();
            if (list != null)
            {
                foreach (object tmp in list)
                {
                    List<object> it;
                    if (tmp is List<object>)
                    {
                        it = (List<object>)tmp;
                    }
                    else
                    {
                        it = new List<object>((object[])tmp);
                    }
                    GXDLMSCertificateInfo info = new GXDLMSCertificateInfo();
                    info.Entity = (CertificateEntity)Convert.ToInt32(it[0]);
                    info.Type = (CertificateType)Convert.ToInt32(it[1]);
                    byte[] tmp2 = (byte[])it[2];
                    Array.Reverse(tmp2);
                    info.SerialNumber = new BigInteger(tmp2);
                    info.Issuer = ASCIIEncoding.ASCII.GetString((byte[])it[3]);
                    info.Subject = ASCIIEncoding.ASCII.GetString((byte[])it[4]);
                    info.SubjectAltName = ASCIIEncoding.ASCII.GetString((byte[])it[5]);
                    Certificates.Add(info);
                }
            }
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    SecurityPolicy = (SecurityPolicy)Convert.ToByte(e.Value);
                    break;
                case 3:
                    SecuritySuite = (SecuritySuite)Convert.ToInt32(e.Value);
                    break;
                case 4:
                    ClientSystemTitle = (byte[])e.Value;
                    break;
                case 5:
                    ServerSystemTitle = (byte[])e.Value;
                    break;
                case 6:
                    UpdateSertificates((IEnumerable<object>)e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            SecurityPolicy = (SecurityPolicy)reader.ReadElementContentAsInt("SecurityPolicy");
            //This is old functionality.It can be removed in some point.
            reader.ReadElementContentAsInt("SecurityPolicy0");
            SecuritySuite = (SecuritySuite)reader.ReadElementContentAsInt("SecuritySuite");
            string str = reader.ReadElementContentAsString("ClientSystemTitle");
            if (str == null)
            {
                ClientSystemTitle = null;
            }
            else
            {
                ClientSystemTitle = GXDLMSTranslator.HexToBytes(str);
            }
            str = reader.ReadElementContentAsString("ServerSystemTitle");
            if (str == null)
            {
                ServerSystemTitle = null;
            }
            else
            {
                ServerSystemTitle = GXDLMSTranslator.HexToBytes(str);
            }
            Certificates.Clear();
            if (reader.IsStartElement("Certificates", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSCertificateInfo it = new GXDLMSCertificateInfo();
                    Certificates.Add(it);
                    it.Entity = (CertificateEntity)reader.ReadElementContentAsInt("Entity");
                    it.Type = (CertificateType)reader.ReadElementContentAsInt("Type");
                    it.SerialNumber = BigInteger.Parse(reader.ReadElementContentAsString("SerialNumber"));
                    it.Issuer = reader.ReadElementContentAsString("Issuer");
                    it.Subject = reader.ReadElementContentAsString("Subject");
                    it.SubjectAltName = reader.ReadElementContentAsString("SubjectAltName");
                }
                reader.ReadEndElement("Certificates");
            }
            str = reader.ReadElementContentAsString("SigningKey");
            if (str == null)
            {
                SigningKey = null;
            }
            else
            {
                GXPkcs8 pk = GXPkcs8.FromDer(str);
                SigningKey = new KeyValuePair<GXPublicKey, GXPrivateKey>(pk.PublicKey, pk.PrivateKey);
                System.Diagnostics.Debug.WriteLine("Signing Private Key: " + pk.PrivateKey.ToHex());
                System.Diagnostics.Debug.WriteLine("Signing Public Key: " + pk.PublicKey.ToHex());
            }
            str = reader.ReadElementContentAsString("KeyAgreement");
            if (str == null)
            {
                KeyAgreementKey = null;
            }
            else
            {
                GXPkcs8 pk = GXPkcs8.FromDer(str);
                KeyAgreementKey = new KeyValuePair<GXPublicKey, GXPrivateKey>(pk.PublicKey, pk.PrivateKey);
            }
            str = reader.ReadElementContentAsString("TLS");
            if (str == null)
            {
                TlsKey = null;
            }
            else
            {
                GXPkcs8 pk = GXPkcs8.FromDer(str);
                TlsKey = new KeyValuePair<GXPublicKey, GXPrivateKey>(pk.PublicKey, pk.PrivateKey);
            }
            ServerCertificates.Clear();
            if (reader.IsStartElement("ServerCertificates", true))
            {
                while (reader.IsStartElement("Cert", false))
                {
                    GXx509Certificate cert = GXx509Certificate.FromDer(reader.ReadElementContentAsString("Cert"));
                    ServerCertificates.Add(cert);
                }
                reader.ReadEndElement("ServerCertificates");
            }
            str = reader.ReadElementContentAsString("Guek");
            if (str != null)
            {
                Guek = GXCommon.HexToBytes(str);
            }
            str = reader.ReadElementContentAsString("Gbek");
            if (str != null)
            {
                Gbek = GXCommon.HexToBytes(str);
            }
            str = reader.ReadElementContentAsString("Gak");
            if (str != null)
            {
                Gak = GXCommon.HexToBytes(str);
            }
            str = reader.ReadElementContentAsString("Kek");
            if (str != null)
            {
                Kek = GXCommon.HexToBytes(str);
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("SecurityPolicy", (byte)SecurityPolicy, 2);
            writer.WriteElementString("SecuritySuite", (int)SecuritySuite, 3);
            writer.WriteElementString("ClientSystemTitle", GXDLMSTranslator.ToHex(ClientSystemTitle), 4);
            writer.WriteElementString("ServerSystemTitle", GXDLMSTranslator.ToHex(ServerSystemTitle), 5);
            if (Certificates != null)
            {
                writer.WriteStartElement("Certificates", 6);
                foreach (GXDLMSCertificateInfo it in Certificates)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("Entity", (int)it.Entity, 0);
                    writer.WriteElementString("Type", (int)it.Type, 0);
                    writer.WriteElementString("SerialNumber", it.SerialNumber.ToString(), 0);
                    writer.WriteElementString("Issuer", it.Issuer, 0);
                    writer.WriteElementString("Subject", it.Subject, 0);
                    writer.WriteElementString("SubjectAltName", it.SubjectAltName, 0);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            // If all associations are using the same certificates.
            if (serverSettings != null && serverSettings.AssociationsShareCertificates)
            {
                SigningKey = serverSettings.Cipher.SigningKeyPair;
                KeyAgreementKey = serverSettings.Cipher.KeyAgreementKeyPair;
            }
            if (SigningKey != null)
            {
                GXPkcs8 kp = new GXPkcs8(SigningKey.Value);
                writer.WriteElementString("SigningKey", kp.ToDer(), 0);
            }
            if (KeyAgreementKey != null)
            {
                GXPkcs8 kp = new GXPkcs8(KeyAgreementKey.Value);
                writer.WriteElementString("KeyAgreement", kp.ToDer(), 0);
            }
            if (TlsKey != null)
            {
                GXPkcs8 kp = new GXPkcs8(TlsKey.Value);
                writer.WriteElementString("TLS", kp.ToDer(), 0);
            }
            if (ServerCertificates.Count != 0)
            {
                writer.WriteStartElement("ServerCertificates", 0);
                foreach (GXx509Certificate it in ServerCertificates)
                {
                    writer.WriteElementString("Cert", it.ToDer(), 0);
                }
                writer.WriteEndElement();
            }
            if (Guek != null)
            {
                writer.WriteElementString("Guek", GXCommon.ToHex(Guek, false), 0);
            }
            if (Gbek != null)
            {
                writer.WriteElementString("Gbek", GXCommon.ToHex(Gbek, false), 0);
            }
            if (Gak != null)
            {
                writer.WriteElementString("Gak", GXCommon.ToHex(Gak, false), 0);
            }
            if (Kek != null)
            {
                writer.WriteElementString("Kek", GXCommon.ToHex(Kek, false), 0);
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}