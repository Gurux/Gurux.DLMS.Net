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
#if !WINDOWS_UWP
using System.Security.Cryptography;
#endif
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using System.Collections.Generic;
using Gurux.DLMS.Ecdsa;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Ecdsa.Enums;
using Gurux.DLMS.ASN;
using System.Linq;

namespace Gurux.DLMS.Secure
{
    internal class GXSecure
    {

        ///<summary>
        /// Constructor.
        ///</summary>
        private GXSecure()
        {

        }

        ///<summary>
        /// Chipher text.
        ///</summary>
        ///<param name="settings">
        ///DLMS settings.
        ///</param>
        ///<param name="cipher">Cipher.</param>
        ///<param name="ic">Invocation counter.</param>
        ///<param name="data">Text to chipher.</param>
        ///<param name="secret">Secret.</param>
        ///<returns>Chiphered text.</returns>
        internal static byte[] Secure(GXDLMSSettings settings, GXICipher cipher, UInt32 ic, byte[] data, byte[] secret)
        {
            byte[] tmp;
            if (settings.Authentication == Authentication.High)
            {
                int len = secret.Length;
                if (len % 16 != 0)
                {
                    len += 16 - (secret.Length % 16);
                }
                if (data.Length < len)
                {
                    len = data.Length;
                }
                byte[] p = new byte[len];
                byte[] s = new byte[16];
                byte[] x = new byte[16];
                int i;
                Array.Copy(data, p, len);
                secret.CopyTo(s, 0);
                for (i = 0; i < p.Length; i += 16)
                {
                    Buffer.BlockCopy(p, i, x, 0, Math.Min(p.Length, 16));
                    GXAes128.Encrypt(x, s);
                    Buffer.BlockCopy(x, 0, p, i, Math.Min(p.Length, 16));
                }
                Buffer.BlockCopy(p, 0, x, 0, Math.Min(p.Length, 16));
                return x;
            }
            // Get server Challenge.
            GXByteBuffer challenge = new GXByteBuffer();
            // Get shared secret
            if (settings.Authentication == Authentication.HighGMAC)
            {
                challenge.Set(data);
            }
            else if (settings.Authentication == Authentication.HighSHA256 ||
                settings.Authentication == Authentication.HighECDSA)
            {
                challenge.Set(secret);
            }
            else
            {
                challenge.Set(data);
                challenge.Set(secret);
            }
            tmp = challenge.Array();
            if (settings.Authentication == Authentication.HighMD5)
            {
#if !WINDOWS_UWP
                using (MD5 md5Hash = MD5.Create())
                {
                    tmp = md5Hash.ComputeHash(tmp);
                    return tmp;
                }
#endif
            }
            else if (settings.Authentication == Authentication.HighSHA1)
            {
#if !WINDOWS_UWP
                using (SHA1 sha = new SHA1CryptoServiceProvider())
                {
                    tmp = sha.ComputeHash(tmp);
                    return tmp;
                }
#endif
            }
            else if (settings.Authentication == Authentication.HighSHA256)
            {
                //Windows UWP, IOS ad Android don't support this.
#if !WINDOWS_UWP && !__IOS__ && !__ANDROID__
                using (SHA256 sha = new SHA256CryptoServiceProvider())
                {
                    tmp = sha.ComputeHash(tmp);
                    return tmp;
                }
#endif
            }
            else if (settings.Authentication == Authentication.HighGMAC)
            {
                //SC is always Security.Authentication.
                AesGcmParameter p = new AesGcmParameter(0,
                    settings,
                    Security.Authentication,
                    cipher.SecuritySuite,
                    ic,
                    secret,
                    cipher.BlockCipherKey,
                    cipher.AuthenticationKey);
                p.Type = CountType.Tag;
                challenge.Clear();
                //Security suite is 0.
                challenge.SetUInt8((byte)((int)Security.Authentication | (int)settings.Cipher.SecuritySuite));
                challenge.SetUInt32((UInt32)p.InvocationCounter);
                challenge.Set(GXSecure.EncryptAesGcm(p, tmp));
                tmp = challenge.Array();
                return tmp;
            }
#if !WINDOWS_UWP
            else if (settings.Authentication == Authentication.HighECDSA)
            {
                GXPrivateKey key = settings.Cipher.SigningKeyPair.Value;
                GXPublicKey pub = settings.Cipher.SigningKeyPair.Key;
                if (key == null)
                {
                    key = (GXPrivateKey)settings.GetKey(CertificateType.DigitalSignature, settings.Cipher.SystemTitle, true);
                    settings.Cipher.KeyAgreementKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                }
                if (pub == null)
                {
                    pub = (GXPublicKey)settings.GetKey(CertificateType.DigitalSignature, settings.SourceSystemTitle, false);
                    settings.Cipher.KeyAgreementKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                }
                if (key == null)
                {
                    throw new ArgumentNullException("Signing key is not set.");
                }
                System.Diagnostics.Debug.WriteLine("Private signed key: " + key.ToHex());
                System.Diagnostics.Debug.WriteLine("Public signed key: " + key.GetPublicKey().ToHex());
                GXEcdsa sig = new GXEcdsa(key);
                data = sig.Sign(secret);
            }
#endif //!WINDOWS_UWP
            return data;
        }

        ///<summary>
        ///Generates challenge.
        ///</summary>
        ///<param name="authentication">Used authentication.</param>
        ///<param name="size">Challenge size. Random if it's zero.</param>
        ///<returns>
        ///Generated challenge.
        ///</returns>
        public static byte[] GenerateChallenge(Authentication authentication, byte size)
        {
            Random r = new Random();
            int len = size;
            if (size == 0 ||
                (size == 16 && authentication == Authentication.HighECDSA))
            {
                if (authentication == Authentication.HighECDSA)
                {
                    len = r.Next(32) + 32;
                }
                else
                {
                    len = r.Next(57) + 8;
                }
            }
            byte[] result = new byte[len];
            for (int pos = 0; pos != len; ++pos)
            {
                result[pos] = (byte)r.Next(0x7A);
            }
            return result;
        }

#if !WINDOWS_UWP
        /// <summary>
        /// Generate KDF.
        /// </summary>
        /// <remarks>
        /// GB: Table 18 – Cryptographic algorithm ID-s 
        /// </remarks>
        /// <param name="securitySuite">Used security suite.</param>
        /// <param name="z">Shared Secret.</param>
        /// <param name="algorithmID">Algorithm ID.</param>
        /// <param name="partyUInfo">Sender system title.</param>
        /// <param name="partyVInfo">Receiver system title.</param>
        /// <param name="suppPubInfo">Not used in DLMS.</param>
        /// <param name="suppPrivInfo">Not used in DLMS.</param>
        /// <returns></returns>
        public static byte[] GenerateKDF(
                SecuritySuite securitySuite,
                byte[] z,
                AlgorithmId algorithmID,
                byte[] partyUInfo,
                byte[] partyVInfo,
                byte[] suppPubInfo,
                byte[] suppPrivInfo)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.Set(new byte[] { 0x60, 0x85, 0x74, 0x05, 0x08, 0x03, (byte)algorithmID });
            bb.Set(partyUInfo);
            bb.Set(partyVInfo);
            if (suppPubInfo != null)
            {
                bb.Set(suppPubInfo);
            }
            if (suppPrivInfo != null)
            {
                bb.Set(suppPrivInfo);
            }
            return GenerateKDF(securitySuite, z, bb.Array());
        }

        /// <summary>
        /// Generate KDF.
        /// </summary>
        /// <param name="securitySuite">Security suite.</param>
        /// <param name="z">z Shared Secret.</param>
        /// <param name="otherInfo">Other info.</param>
        /// <returns></returns>
        public static byte[] GenerateKDF(SecuritySuite securitySuite, byte[] z, byte[] otherInfo)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt32(1);
            bb.Set(z);
            bb.Set(otherInfo);
            if (securitySuite == SecuritySuite.Suite1)
            {
                using (SHA256 sha = new SHA256CryptoServiceProvider())
                {
                    return sha.ComputeHash(bb.Array());
                }
            }
            else if (securitySuite == SecuritySuite.Suite2)
            {
                using (SHA384 sha = new SHA384CryptoServiceProvider())
                {
                    return sha.ComputeHash(bb.Array());
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid security suite.");
            }
        }
#endif //!WINDOWS_UWP

        /// <summary>
        /// Get Ephemeral Public Key Signature.
        /// </summary>
        /// <param name="keyId">Key ID.</param>
        /// <param name="ephemeralKey">Ephemeral key.</param>
        /// <returns>Ephemeral Public Key Signature.</returns>
        public static byte[] GetEphemeralPublicKeyData(int keyId,
                GXPublicKey ephemeralKey)
        {
            GXAsn1BitString tmp = (GXAsn1BitString)((GXAsn1Sequence)GXAsn1Converter.FromByteArray(ephemeralKey.ToEncoded()))[1];
            // Ephemeral public key client
            GXByteBuffer epk = new GXByteBuffer(tmp.Value);
            // First byte is 4 and that is not used. We can override it.
            epk.Data[0] = (byte)keyId;
            return epk.Array();
        }

#if !WINDOWS_UWP
        /// <summary>
        /// Get Ephemeral Public Key Signature.
        /// </summary>
        /// <param name="keyId">Key ID.</param>
        /// <param name="ephemeralKey">Ephemeral key.</param>
        /// <param name="signKey">Private Key.</param>
        /// <returns>Ephemeral Public Key Signature.</returns>
        public static byte[] GetEphemeralPublicKeySignature(
            int keyId,
                GXPublicKey ephemeralKey,
                GXPrivateKey signKey)
        {
            byte[] epk = GetEphemeralPublicKeyData(keyId, ephemeralKey);
            // Add ephemeral public key signature.
            GXEcdsa c = new GXEcdsa(signKey);
            byte[] sign = c.Sign(epk);
            return sign;
        }

        /// <summary>
        /// Validate ephemeral public key signature.
        /// </summary>
        /// <param name="data">Data to validate.</param>
        /// <param name="sign">Sign</param>
        /// <param name="publicSigningKey">Public Signing key from other party.</param>
        /// <returns>Is verify succeeded.</returns>
        public static bool ValidateEphemeralPublicKeySignature(
            byte[] data,
            byte[] sign,
            GXPublicKey publicSigningKey)
        {
            GXAsn1Integer a = new GXAsn1Integer(sign, 0, 32);
            GXAsn1Integer b = new GXAsn1Integer(sign, 32, 32);
            GXAsn1Sequence s = new GXAsn1Sequence();
            s.Add(a);
            s.Add(b);
            byte[] tmp = GXAsn1Converter.ToByteArray(s);
            GXEcdsa c = new GXEcdsa(publicSigningKey);
            bool ret = c.Verify(sign, data);
            if (!ret)
            {
                System.Diagnostics.Debug.WriteLine("Data:" + GXCommon.ToHex(data, true));
                System.Diagnostics.Debug.WriteLine("Sign:" + GXCommon.ToHex(sign, true));
            }
            return ret;
        }
#endif //!WINDOWS_UWP

        static public byte[] EncryptAesGcm(AesGcmParameter param, byte[] plainText)
        {
            System.Diagnostics.Debug.WriteLine("Encrypt settings: " + param.ToString());
            byte tag;
            param.CountTag = null;
            GXByteBuffer data = new GXByteBuffer();
            tag = (byte)((byte)param.Security | (byte)param.SecuritySuite);
            if (param.Broacast)
            {
                tag |= 0x40;
            }
            if (param.Compression)
            {
                tag |= 0x80;
            }
            if (param.Type == CountType.Packet)
            {
                data.SetUInt8(tag);
            }
            byte[] ciphertext = null;
            byte[] tmp = BitConverter.GetBytes((UInt32)param.InvocationCounter).Reverse().ToArray();
            //If external Hardware Security Module is used.
            if (param.Settings != null)
            {
                CryptoKeyType keyType;
                switch (param.Security)
                {
                    case Security.Authentication:
                        keyType = CryptoKeyType.Authentication;
                        break;
                    case Security.Encryption:
                        keyType = CryptoKeyType.BlockCipher;
                        break;
                    case Security.AuthenticationEncryption:
                        keyType = CryptoKeyType.Authentication | CryptoKeyType.BlockCipher;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Security");
                }
                ciphertext = param.Settings.Crypt(CertificateType.DigitalSignature, plainText, true, keyType);
            }
            if (ciphertext != null)
            {
                data.Set(tmp);
                if (param.Security == Security.Authentication)
                {
                    data.Set(plainText);
                }
                data.Set(ciphertext);
                return data.Array();
            }

            GXGMac gmac = new GXGMac(param.AuthenticationKey, param.BlockCipherKey, param.SystemTitle, (UInt32)param.InvocationCounter);
            ciphertext = gmac.Encrypt(plainText, tag);
            if (param.Security == Security.Authentication)
            {
                if (param.Type == CountType.Packet)
                {
                    data.Set(tmp);
                }
                if ((param.Type & CountType.Data) != 0)
                {
                    data.Set(plainText);
                }
                if ((param.Type & CountType.Tag) != 0)
                {
                    data.Set(ciphertext);
                }
            }
            else if (param.Security == Security.Encryption)
            {
                if (param.Type == CountType.Packet)
                {
                    data.Set(tmp);
                }
                data.Set(ciphertext);
            }
            else if (param.Security == Security.AuthenticationEncryption)
            {
                if (param.Type == CountType.Packet)
                {
                    data.Set(tmp);
                }
                if ((param.Type & CountType.Data) != 0)
                {
                    data.Set(ciphertext);
                }
                if ((param.Type & CountType.Tag) != 0)
                {
                    data.Set(param.CountTag);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("security");
            }
            if (param.Type == CountType.Packet)
            {
                GXByteBuffer tmp2 = new GXByteBuffer((ushort)(10 + data.Size));
                tmp2.SetUInt8(param.Tag);
                if (param.Tag == (int)Command.GeneralGloCiphering ||
                    param.Tag == (int)Command.GeneralDedCiphering ||
                    param.Tag == (int)Command.DataNotification)
                {
                    if (!param.IgnoreSystemTitle)
                    {
                        GXCommon.SetObjectCount(param.SystemTitle.Length, tmp2);
                        tmp2.Set(param.SystemTitle);
                    }
                    else
                    {
                        tmp2.SetUInt8(0);
                    }
                }
                GXCommon.SetObjectCount(data.Size, tmp2);
                tmp2.Set(data.Array());
                return tmp2.Array();
            }
            byte[] crypted = data.Array();
            System.Diagnostics.Debug.WriteLine("Crypted: " + GXCommon.ToHex(crypted, true));
            return crypted;
        }

        /// <summary>
        /// Are tags equals.
        /// </summary>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        /// <returns></returns>
        private static bool TagsEquals(byte[] tag1, byte[] tag2)
        {
            for (int pos = 0; pos != 12; ++pos)
            {
                if (tag1[pos] != tag2[pos])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Decrypt data.
        /// </summary>
        /// <param name="p">Decryption parameters</param>
        /// <returns>Decrypted data.</returns>
        public static byte[] DecryptAesGcm(AesGcmParameter p, GXByteBuffer data)
        {
            if (data == null || data.Size < 2)
            {
                throw new ArgumentOutOfRangeException("cryptedData");
            }
            byte[] tmp;
            int len;
            Command cmd = (Command)data.GetUInt8();
            switch (cmd)
            {
                case Command.GeneralGloCiphering:
                case Command.GeneralDedCiphering:
                    len = GXCommon.GetObjectCount(data);
                    if (len != 0)
                    {
                        p.SystemTitle = new byte[len];
                        data.Get(p.SystemTitle);
                        if (p.Xml != null && p.Xml.Comments)
                        {
                            p.Xml.AppendComment(GXCommon.SystemTitleToString(Standard.DLMS, p.SystemTitle, true));
                        }
                    }
                    if (p.SystemTitle == null || p.SystemTitle.Length != 8)
                    {
                        if (p.Xml == null)
                        {
                            throw new ArgumentNullException("Invalid sender system title.");
                        }
                        else
                        {
                            p.Xml.AppendComment("Invalid sender system title.");
                        }
                    }
                    break;
                case Command.GeneralCiphering:
                case Command.GloInitiateRequest:
                case Command.GloInitiateResponse:
                case Command.GloReadRequest:
                case Command.GloReadResponse:
                case Command.GloWriteRequest:
                case Command.GloWriteResponse:
                case Command.GloGetRequest:
                case Command.GloGetResponse:
                case Command.GloSetRequest:
                case Command.GloSetResponse:
                case Command.GloMethodRequest:
                case Command.GloMethodResponse:
                case Command.GloEventNotification:
                case Command.DedInitiateRequest:
                case Command.DedInitiateResponse:
                case Command.DedGetRequest:
                case Command.DedGetResponse:
                case Command.DedSetRequest:
                case Command.DedSetResponse:
                case Command.DedMethodRequest:
                case Command.DedMethodResponse:
                case Command.DedEventNotification:
                case Command.DedReadRequest:
                case Command.DedReadResponse:
                case Command.DedWriteRequest:
                case Command.DedWriteResponse:
                case Command.GloConfirmedServiceError:
                case Command.DedConfirmedServiceError:
                case Command.GeneralSigning:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("cryptedData");
            }
            int value = 0;
            KeyValuePair<GXPublicKey, GXPrivateKey> kp = new KeyValuePair<GXPublicKey, GXPrivateKey>(null, null);
            GXPrivateKey key = null;
            GXPublicKey pub = null;
            GXByteBuffer transactionId = null;
            if (cmd == Command.GeneralCiphering || cmd == Command.GeneralSigning)
            {
                transactionId = new GXByteBuffer();
                len = GXCommon.GetObjectCount(data);
                if (len != 0)
                {
                    GXCommon.SetObjectCount(len, transactionId);
                    transactionId.Set(data, len);
                    p.TransactionId = transactionId.GetUInt64(1);
                }
                else
                {
                    p.TransactionId = 0;
                }
                len = GXCommon.GetObjectCount(data);
                if (len != 0)
                {
                    tmp = new byte[len];
                    data.Get(tmp);
                    p.SystemTitle = tmp;
                }
                if (p.SystemTitle == null || p.SystemTitle.Length != 8)
                {
                    if (p.Xml == null)
                    {
                        throw new ArgumentNullException("Invalid sender system title.");
                    }
                    else
                    {
                        p.Xml.AppendComment("Invalid sender system title.");
                    }
                }
                len = GXCommon.GetObjectCount(data);
                tmp = new byte[len];
                data.Get(tmp);
                p.RecipientSystemTitle = tmp;
                // Get date time.
                len = GXCommon.GetObjectCount(data);
                if (len != 0)
                {
                    tmp = new byte[len];
                    data.Get(tmp);
                    p.DateTime = tmp;
                }
                // other-information
                len = data.GetUInt8();
                if (len != 0)
                {
                    tmp = new byte[len];
                    data.Get(tmp);
                    p.OtherInformation = tmp;
                }
                if (cmd == Command.GeneralCiphering)
                {
                    // KeyInfo OPTIONAL
                    data.GetUInt8();
                    // AgreedKey CHOICE tag.
                    data.GetUInt8();
                    // key-parameters
                    data.GetUInt8();
                    value = data.GetUInt8();
                    p.KeyParameters = value;
                    if (value == (int)KeyAgreementScheme.OnePassDiffieHellman)
                    {
                        //Update security because server needs it and client when push message is received.
                        p.Settings.Cipher.Signing = Signing.OnePassDiffieHellman;
                        // key-ciphered-data
                        len = GXCommon.GetObjectCount(data);
                        GXByteBuffer bb = new GXByteBuffer();
                        bb.Set(data, len);
                        if (p.Xml != null)
                        {
                            p.KeyCipheredData = bb.Array();
                        }
                        kp = p.Settings.Cipher.KeyAgreementKeyPair;
                        if (kp.Key == null)
                        {
                            pub = (GXPublicKey)p.Settings.GetKey(CertificateType.KeyAgreement, p.SystemTitle, false);
                            if (pub != null)
                            {
                                p.Settings.Cipher.KeyAgreementKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, p.Settings.Cipher.KeyAgreementKeyPair.Value);
                            }
                        }
                        if (kp.Value == null)
                        {
                            key = (GXPrivateKey)p.Settings.GetKey(CertificateType.KeyAgreement, p.SystemTitle, true);
                            if (key != null)
                            {
                                p.Settings.Cipher.KeyAgreementKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(p.Settings.Cipher.KeyAgreementKeyPair.Key, key);
                            }
                        }
                        if (kp.Key != null)
                        {
                            //Get Ephemeral public key.
                            int keySize = len / 2;
                            kp = new KeyValuePair<GXPublicKey, GXPrivateKey>(GXPublicKey.FromRawBytes(bb.SubArray(0, keySize)), kp.Value);
                        }
                    }
                    else if (value == (int)KeyAgreementScheme.StaticUnifiedModel)
                    {
                        //Update security because server needs it and client when push message is received.
                        p.Settings.Cipher.Signing = Signing.StaticUnifiedModel;
                        len = GXCommon.GetObjectCount(data);
                        if (len != 0)
                        {
                            throw new ArgumentException("Invalid key parameters");
                        }
                        kp = p.Settings.Cipher.KeyAgreementKeyPair;
                        if (kp.Key == null)
                        {
                            pub = (GXPublicKey)p.Settings.GetKey(CertificateType.KeyAgreement, p.SystemTitle, false);
                        }
                        else
                        {
                            pub = p.Settings.Cipher.KeyAgreementKeyPair.Key;
                        }
                        if (kp.Value == null)
                        {
                            key = (GXPrivateKey)p.Settings.GetKey(CertificateType.KeyAgreement, p.RecipientSystemTitle, true);
                        }
                        else
                        {
                            key = p.Settings.Cipher.KeyAgreementKeyPair.Value;
                        }
                        if (kp.Key == null || kp.Value == null)
                        {
                            kp = p.Settings.Cipher.KeyAgreementKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                        }
                    }
                }
                else
                {
                    kp = p.Settings.Cipher.SigningKeyPair;
                    if (kp.Key == null || kp.Value == null)
                    {
                        if (kp.Key == null)
                        {
                            key = p.Settings.Cipher.SigningKeyPair.Value;
                            pub = (GXPublicKey)p.Settings.GetKey(CertificateType.DigitalSignature, p.SystemTitle, false);
                            kp = p.Settings.Cipher.SigningKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                        }
                        if (kp.Value == null)
                        {
                            pub = p.Settings.Cipher.SigningKeyPair.Key;
                            key = (GXPrivateKey)p.Settings.GetKey(CertificateType.DigitalSignature, p.RecipientSystemTitle, true);
                            kp = p.Settings.Cipher.SigningKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                        }
                    }
                }
            }
            int contentStart = data.Position;
            //Content length is not add for the signed data.
            len = GXCommon.GetObjectCount(data);
            if (len > data.Available)
            {
                throw new Exception("Not enought data.");
            }
            p.CipheredContent = data.SubArray(data.Position, len);
            if (cmd == Command.GeneralSigning && p.Xml != null)
            {
                p.Xml.AppendLine(TranslatorTags.TransactionId, null, p.Xml.IntegerToHex(p.TransactionId, 16, true));
                p.Xml.AppendLine(TranslatorTags.OriginatorSystemTitle, null, GXCommon.ToHex(p.SystemTitle, false));
                p.Xml.AppendLine(TranslatorTags.RecipientSystemTitle, null, GXCommon.ToHex(p.RecipientSystemTitle, false));
                p.Xml.AppendLine(TranslatorTags.DateTime, null, GXCommon.ToHex(p.DateTime, false));
                p.Xml.AppendLine(TranslatorTags.OtherInformation, null, GXCommon.ToHex(p.OtherInformation, false));
            }
            if (cmd == Command.GeneralSigning)
            {
                if (!GXDLMS.IsCiphered(data.GetUInt8()))
                {
                    --data.Position;
                    if (p.Settings.IsServer)
                    {
                        if ((p.Settings.Cipher.SecurityPolicy & (SecurityPolicy.EncryptedRequest | SecurityPolicy.AuthenticatedRequest)) != 0)
                        {
                            throw new GXDLMSExceptionResponse(ExceptionStateError.ServiceNotAllowed,
                                    ExceptionServiceError.DecipheringError, 0);
                        }
                    }
                    return data.Remaining();
                }
                len = GXCommon.GetObjectCount(data);
                if (len > data.Available)
                {
                    throw new Exception("Not enought data.");
                }
            }

            byte sc = data.GetUInt8();
            p.SecuritySuite = (SecuritySuite)(sc & 0x3);
            p.Security = (Security)(sc & 0x30);
            if ((sc & 0x80) != 0)
            {
                System.Diagnostics.Debug.WriteLine("Compression is used.");
                p.Compression = true;
            }
            if ((sc & 0x40) != 0)
            {
                System.Diagnostics.Debug.WriteLine("Broacast is used.");
                if (p.Xml == null && p.Settings.Cipher.BroadcastBlockCipherKey == null)
                {
                    throw new GXDLMSExceptionResponse(ExceptionStateError.ServiceNotAllowed,
                                ExceptionServiceError.DecipheringError, 0);
                }
                p.Broacast = true;
                p.BlockCipherKey = p.Settings.Cipher.BroadcastBlockCipherKey;
            }
            if ((sc & 0x20) != 0)
            {
                System.Diagnostics.Debug.WriteLine("Encryption is applied.");
            }
            if ((sc & 0x10) != 0)
            {
                System.Diagnostics.Debug.WriteLine("Authentication is applied.");
            }
            if (value != 0 && p.Xml != null && kp.Key == null)
            {
                return p.CipheredContent;
            }
            if (cmd == Command.GeneralSigning && p.Security != Security.None)
            {
                if (!(p.Xml != null && (kp.Value == null || kp.Key == null)))
                {
                    if (kp.Value != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Private signing key: " + kp.Value.ToHex());
                    }
                    if (kp.Key != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Public signing key: " + kp.Key.ToHex());
                    }
                }
            }
#if !WINDOWS_UWP
            else if (value == (int)KeyAgreementScheme.OnePassDiffieHellman)
            {
                GXEcdsa c = new GXEcdsa(kp.Value);
                //Get Ephemeral signing key and verify it.
                byte[] z = c.GenerateSecret(kp.Key);
                System.Diagnostics.Debug.WriteLine("Private agreement key: " + kp.Value.ToHex());
                System.Diagnostics.Debug.WriteLine("Public ephemeral key: " + kp.Key.ToHex());
                System.Diagnostics.Debug.WriteLine("Shared secret:" + GXCommon.ToHex(z, true));
                GXByteBuffer kdf = new GXByteBuffer();
                kdf.Set(GXSecure.GenerateKDF(p.SecuritySuite, z,
                    p.SecuritySuite == SecuritySuite.Suite1 ? AlgorithmId.AesGcm128 : AlgorithmId.AesGcm256,
                    p.SystemTitle,
                    p.RecipientSystemTitle,
                    null, null));
                System.Diagnostics.Debug.WriteLine("KDF:" + kdf.ToString());
                p.BlockCipherKey = kdf.SubArray(0, 16);

            }
            else if (value == (int)KeyAgreementScheme.StaticUnifiedModel)
            {
                GXEcdsa c = new GXEcdsa(kp.Value);
                byte[] z = c.GenerateSecret(kp.Key);
                System.Diagnostics.Debug.WriteLine("Private agreement key: " + kp.Value.ToHex());
                System.Diagnostics.Debug.WriteLine("Public agreement key: " + kp.Key.ToHex());
                System.Diagnostics.Debug.WriteLine("Shared secret:" + GXCommon.ToHex(z, true));
                GXByteBuffer kdf = new GXByteBuffer();
                kdf.Set(GXSecure.GenerateKDF(p.SecuritySuite, z,
                    p.SecuritySuite == SecuritySuite.Suite1 ? AlgorithmId.AesGcm128 : AlgorithmId.AesGcm256,
                    p.SystemTitle,
                    transactionId.Array(),
                    p.RecipientSystemTitle,
                    null));
                System.Diagnostics.Debug.WriteLine("KDF: " + kdf.ToString());
                System.Diagnostics.Debug.WriteLine("Authentication key: " + GXCommon.ToHex(p.AuthenticationKey, true));
                p.BlockCipherKey = kdf.SubArray(0, 16);
            }
#endif //!WINDOWS_UWP
            if (p.Xml == null && value != 0 && kp.Key == null)
            {
                throw new ArgumentOutOfRangeException("Invalid Key-id value.");
            }
            UInt32 invocationCounter = 0;
            if (p.Security != Security.None)
            {
                invocationCounter = data.GetUInt32();
            }
            if (p.Settings != null && p.Settings.InvocationCounter != null)
            {
                if (invocationCounter < Convert.ToUInt32(p.Settings.InvocationCounter.Value))
                {
                    throw new GXDLMSExceptionResponse(ExceptionStateError.ServiceNotAllowed,
                            ExceptionServiceError.InvocationCounterError, p.Settings.InvocationCounter.Value);
                }
                // Update IC value.
                p.Settings.InvocationCounter.Value = invocationCounter;
            }
            p.InvocationCounter = invocationCounter;

            if (p.AuthenticationKey == null || p.BlockCipherKey == null)
            {
                if (p.Settings.CryptoNotifier == null ||
                    p.Settings.CryptoNotifier.keys == null)
                {
                    throw new Exception("Failed to get the block cipher key.");
                }
                GXCryptoKeyParameter args = new GXCryptoKeyParameter()
                {
                    InvocationCounter = invocationCounter,
                    SystemTitle = p.SystemTitle
                };
                if (p.BlockCipherKey == null)
                {
                    args.KeyType |= CryptoKeyType.BlockCipher;
                }
                if (p.AuthenticationKey == null)
                {
                    args.KeyType |= CryptoKeyType.Authentication;
                }
                p.Settings.CryptoNotifier.keys(p.Settings.CryptoNotifier, args);
                if (p.BlockCipherKey == null)
                {
                    if (args.BlockCipherKey == null || (
                        args.BlockCipherKey.Length != 16 && args.BlockCipherKey.Length != 32))
                    {
                        throw new Exception("Invalid Block cipher key.");
                    }
                    p.BlockCipherKey = args.BlockCipherKey;
                }
                if (p.AuthenticationKey == null)
                {
                    if (args.AuthenticationKey == null || (
                        args.AuthenticationKey.Length != 16 && args.AuthenticationKey.Length != 32))
                    {
                        throw new Exception("Invalid authentication key.");
                    }
                    p.AuthenticationKey = args.AuthenticationKey;
                }
            }
            System.Diagnostics.Debug.WriteLine("Decrypt settings: " + p.ToString());
            System.Diagnostics.Debug.WriteLine("Encrypted: " + GXCommon.ToHex(data.Data,
                    false, data.Position, data.Size - data.Position));
            byte[] tag = new byte[12];
            byte[] encryptedData;
            int length;
            if (p.Security == Security.Authentication)
            {
                length = len - 12 - 5;
                encryptedData = new byte[length];
                data.Get(encryptedData);
                data.Get(tag);
                // Check tag.
                EncryptAesGcm(p, encryptedData);
                if (!TagsEquals(tag, p.CountTag))
                {
                    if (p.Xml == null)
                    {
                        throw new GXDLMSCipherException("Decrypt failed. Invalid authentication tag.");
                    }
                    else
                    {
                        p.Xml.AppendComment("Decrypt failed. Invalid authentication tag.");
                    }
                }
                return encryptedData;
            }          
            byte[] decrypted;
            //Data might be without ciphering in GeneralSigning.
            if (p.Security != Security.None)
            {
                GXGMac gmac = new GXGMac(p.AuthenticationKey,
                    p.Broacast ? p.BlockCipherKey : p.BlockCipherKey, 
                    p.SystemTitle,
                    invocationCounter);
                byte [] tmp2 = new byte[len - 5];
                data.Get(tmp2);
                byte tag2 = (byte)((byte)p.Security | (byte)p.SecuritySuite);
                if (p.Broacast)
                {
                    tag2 |= 0x40;
                }
                decrypted = gmac.Decrypt(tmp2, tag2, p.Xml);
                System.Diagnostics.Debug.WriteLine("Decrypted: " + GXCommon.ToHex(decrypted, true));
            }
            else
            {
                length = len;
                decrypted = new byte[length];
                data.Get(decrypted);
            }
            if (cmd == Command.GeneralSigning)
            {
                //Content length is not add for the signed data.
                GXByteBuffer signedData = new GXByteBuffer();
                signedData.Set(data.Data, 1, contentStart - 1);
                signedData.Set(p.CipheredContent);
                len = GXCommon.GetObjectCount(data);
                if (len != 64 && len != 96)
                {
                    throw new ArgumentException("Invalid signing.");
                }
                p.Signature = new byte[len];
                data.Get(p.Signature);
                System.Diagnostics.Debug.WriteLine("Verifying signature for sender:" + GXCommon.ToHex(p.SystemTitle, true));
                if (p.Xml == null)
                {
                    if (kp.Key == null)
                    {
                        throw new Exception("Public key is not set.");
                    }
                    if (kp.Key.SystemTitle != null && !GXCommon.Compare(kp.Key.SystemTitle, p.SystemTitle))
                    {
                        throw new Exception(string.Format("Invalid certificate. Expected certificate is for {0} and actual is for {1}",
                            GXCommon.ToHex(p.SystemTitle, false),
                            GXCommon.ToHex(kp.Key.SystemTitle, false)));
                    }
                    GXEcdsa c = new GXEcdsa(kp.Key);
                    System.Diagnostics.Debug.WriteLine("Verifying signature: " + signedData);
#if !WINDOWS_UWP

                    if (!c.Verify(p.Signature, signedData.Array()))
                    {
                        throw new Exception("Invalid signature.");
                    }
#endif //!WINDOWS_UWP
                }
                else
                {
                    if (kp.Key == null && kp.Value == null)
                    {
                        p.Xml.AppendComment("Failed to verify signed data. Public key is not set.");
                    }
                    else if ((kp.Key != null && kp.Key.SystemTitle != null && !GXCommon.Compare(kp.Key.SystemTitle, p.SystemTitle)) &&
                        kp.Value != null && kp.Value.SystemTitle != null && !GXCommon.Compare(kp.Value.SystemTitle, p.SystemTitle))
                    {
                        string st;
                        if (kp.Key != null)
                        {
                            st = GXCommon.ToHex(kp.Key.SystemTitle, false);
                        }
                        else
                        {
                            st = GXCommon.ToHex(kp.Value.SystemTitle, false);
                        }
                        p.Xml.AppendComment(string.Format("Failed to verify signed data. Invalid certificate. Expected certificate is for {0} and actual is for {1}",
                            GXCommon.ToHex(p.SystemTitle, false),
                            st));
                    }
#if !WINDOWS_UWP
                    else
                    {
                        GXEcdsa c;
                        if (kp.Key != null && GXCommon.Compare(kp.Key.SystemTitle, p.SystemTitle))
                        {
                            c = new GXEcdsa(kp.Key);
                        }
                        else
                        {
                            c = new GXEcdsa(kp.Value);
                        }
                        System.Diagnostics.Debug.WriteLine("Verifying signature:" + signedData.ToString());
                        if (!c.Verify(p.Signature, signedData.Array()))
                        {
                            p.Xml.AppendComment("Failed to verify signed data. Invalid signature.");
                        }
                    }
#endif //!WINDOWS_UWP
                }
            }
            return decrypted;
        }
    }
}
