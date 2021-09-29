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
using System.Linq;
using System.Security.Cryptography;
using Gurux.DLMS.ASN;
using Gurux.DLMS.Ecdsa;
using Gurux.DLMS.Ecdsa.Enums;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Secure
{
    internal class GXDLMSChippering
    {
        /// <summary>
        /// Get Nonse from frame counter and system title.
        /// </summary>
        /// <param name="invocationCounter">Invocation counter.</param>
        /// <param name="systemTitle">System title.</param>
        /// <returns></returns>
        static byte[] GetNonse(UInt32 invocationCounter, byte[] systemTitle)
        {
            byte[] nonce = new byte[12];
            systemTitle.CopyTo(nonce, 0);
            byte[] tmp = BitConverter.GetBytes(invocationCounter).Reverse().ToArray();
            tmp.CopyTo(nonce, 8);
            return nonce;
        }

        static internal byte[] EncryptAesGcm(AesGcmParameter param, byte[] plainText)
        {
            System.Diagnostics.Debug.WriteLine("Encrypt settings: " + param.ToString());
            param.CountTag = null;
            GXByteBuffer data = new GXByteBuffer();
            if (param.Type == CountType.Packet)
            {
                data.SetUInt8((byte)((byte)param.Security | (byte)param.SecuritySuite));
            }
            byte[] tmp = BitConverter.GetBytes((UInt32)param.InvocationCounter).Reverse().ToArray();
            byte[] aad = GetAuthenticatedData(param, plainText);
            GXDLMSChipperingStream gcm = new GXDLMSChipperingStream(param.Security, true, param.BlockCipherKey,
                    aad, GetNonse((UInt32)param.InvocationCounter, param.SystemTitle), null);
            // Encrypt the secret message
            if (param.Security != Security.Authentication)
            {
                gcm.Write(plainText);
            }
            byte[] ciphertext = gcm.FlushFinalBlock();
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
                    param.CountTag = gcm.GetTag();
                    data.Set(param.CountTag);
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
                    param.CountTag = gcm.GetTag();
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

        private static byte[] GetAuthenticatedData(AesGcmParameter p, byte[] plainText)
        {
            if (p.Security == Security.AuthenticationEncryption)
            {
                GXByteBuffer tmp2 = new GXByteBuffer();
                tmp2.SetUInt8((byte)((byte)p.Security | (byte)p.SecuritySuite));
                tmp2.Set(p.AuthenticationKey);
                return tmp2.Array();
            }
            if (p.Security == Security.Authentication)
            {
                GXByteBuffer tmp2 = new GXByteBuffer();
                tmp2.SetUInt8((byte)((byte)p.Security | (byte)p.SecuritySuite));
                tmp2.Set(p.AuthenticationKey);
                tmp2.Set(plainText);
                return tmp2.Array();
            }
            if (p.Security == Security.Encryption)
            {
                return p.AuthenticationKey;
            }
            return null;
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
                        if (kp.Value == null)
                        {
                            key = (GXPrivateKey)p.Settings.GetKey(CertificateType.KeyAgreement, p.RecipientSystemTitle, true);
                        }
                        if (kp.Key == null || kp.Value == null)
                        {
                            kp = p.Settings.Cipher.KeyAgreementKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                        }
                    }
                }
                else
                {
                    //Update security because server needs it and client when push message is received.
                    p.Settings.Cipher.Signing = Signing.GeneralSigning;
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
            //Content lenght is not add for the signed data.
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
            }
            if ((sc & 0x40) != 0)
            {
                System.Diagnostics.Debug.WriteLine("Key_Set is used.");
                if (p.Xml == null)
                {
                    throw new GXDLMSExceptionResponse(ExceptionStateError.ServiceNotAllowed,
                                ExceptionServiceError.DecipheringError, 0);
                }
            }
            if ((sc & 0x20) != 0)
            {
                System.Diagnostics.Debug.WriteLine("Encryption is applied.");
                if (p.Xml == null && p.Settings.IsServer && (p.Settings.Cipher.SecurityPolicy & SecurityPolicy.EncryptedRequest) == 0)
                {
                    throw new GXDLMSExceptionResponse(ExceptionStateError.ServiceNotAllowed,
                            ExceptionServiceError.DecipheringError, 0);
                }
            }
            if ((sc & 0x10) != 0)
            {
                System.Diagnostics.Debug.WriteLine("Authentication is applied.");
                if (p.Xml == null && p.Settings.IsServer && (p.Settings.Cipher.SecurityPolicy & SecurityPolicy.AuthenticatedRequest) == 0)
                {
                    throw new GXDLMSExceptionResponse(ExceptionStateError.ServiceNotAllowed,
                            ExceptionServiceError.DecipheringError, 0);
                }
            }
            if (value != 0 && p.Xml != null && kp.Key == null)
            {
                return p.CipheredContent;
            }
            if (cmd == Command.GeneralSigning && p.Security != Security.None)
            {
                if (!(p.Xml != null && (kp.Value == null || kp.Key == null)))
                {
                    System.Diagnostics.Debug.WriteLine("Private signing key: " + kp.Value.ToHex());
                    System.Diagnostics.Debug.WriteLine("Public signing key: " + kp.Key.ToHex());
                }
            }
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
            if (p.Xml == null && value != 0 && kp.Key == null)
            {
                throw new ArgumentOutOfRangeException("Invalid Key-id value.");
            }
            UInt32 invocationCounter = 0;
            if (p.Security != Security.None)
            {
                invocationCounter = data.GetUInt32();
            }
            if (p.Settings.InvocationCounter != null && p.Settings.InvocationCounter.Value is UInt32)
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
                if (!GXDLMSChipperingStream.TagsEquals(tag, p.CountTag))
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
            byte[] ciphertext = null;
            if (p.Security == Security.Encryption)
            {
                length = len - 5;
                ciphertext = new byte[length];
                data.Get(ciphertext);
            }
            else if (p.Security == Security.AuthenticationEncryption)
            {
                length = len - 12 - 5;
                ciphertext = new byte[length];
                data.Get(ciphertext);
                data.Get(tag);
            }
            byte[] decrypted;
            //Data might be without ciphering in GeneralSigning.
            if (p.Security != Security.None)
            {
                byte[] aad = GetAuthenticatedData(p, ciphertext),
                        iv = GetNonse(invocationCounter, p.SystemTitle);
                GXDLMSChipperingStream gcm = new GXDLMSChipperingStream(p.Security, true,
                        p.BlockCipherKey, aad, iv, tag);
                gcm.Write(ciphertext);
                decrypted = gcm.FlushFinalBlock();
                System.Diagnostics.Debug.WriteLine("Decrypted: " + GXCommon.ToHex(decrypted, true));
                if (p.Security != Security.Encryption)
                {
                    if (!GXCommon.Compare(gcm.GetTag(), tag))
                    {
                        if (p.Xml == null)
                        {
                            throw new GXDLMSCipherException("Decrypt failed. Invalid authentication tag.");
                        }
                        p.Xml.AppendComment("Decrypt failed. Invalid authentication tag.");
                    }
                }
            }
            else
            {
                length = len;
                decrypted = new byte[length];
                data.Get(decrypted);
            }
            if (cmd == Command.GeneralSigning)
            {
                //Content lenght is not add for the signed data.
                GXByteBuffer signedData = new GXByteBuffer();
                signedData.Set(data.Data, 1, contentStart - 1);
                signedData.Set(p.CipheredContent);
                len = GXCommon.GetObjectCount(data);
                p.Signature = new byte[len];
                data.Get(p.Signature);
                //signedData.Set(p.Signature);
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
                    if (!c.Verify(p.Signature, signedData.Array()))
                    {
                        throw new Exception("Invalid signature.");
                    }
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
                }
            }
            return decrypted;
        }
    }
}
