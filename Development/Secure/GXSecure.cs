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
        ///<param name="cipher">
        ///Cipher.
        ///</param>
        ///<param name="ic">
        ///Invocation counter.
        ///</param>
        ///<param name="data">
        ///Text to chipher.
        ///</param>
        ///<param name="secret">
        ///Secret.
        ///</param>
        ///<returns>
        ///Chiphered text.
        ///</returns>
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
                byte[] p = new byte[len];
                byte[] s = new byte[16];
                byte[] x = new byte[16];
                int i;
                data.CopyTo(p, 0);
                secret.CopyTo(s, 0);
                for (i = 0; i < p.Length; i += 16)
                {
                    Buffer.BlockCopy(p, i, x, 0, 16);
                    GXAes128.Encrypt(x, s);
                    Buffer.BlockCopy(x, 0, p, i, 16);
                }
                Buffer.BlockCopy(p, 0, x, 0, 16);
                return x;
            }
            // Get server Challenge.
            GXByteBuffer challenge = new GXByteBuffer();
            // Get shared secret
            if (settings.Authentication == Authentication.HighGMAC ||
                settings.Authentication == Authentication.HighECDSA)
            {
                challenge.Set(data);
            }
            else if (settings.Authentication == Authentication.HighSHA256)
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
                AesGcmParameter p = new AesGcmParameter(0, Security.Authentication,
                    cipher.SecuritySuite,
                    ic,
                    secret,
                    cipher.BlockCipherKey,
                    cipher.AuthenticationKey);
                p.Type = CountType.Tag;
                challenge.Clear();
                //Security suite is 0.
                challenge.SetUInt8((byte)Security.Authentication);
                challenge.SetUInt32((UInt32)p.InvocationCounter);
                challenge.Set(GXDLMSChippering.EncryptAesGcm(p, tmp));
                tmp = challenge.Array();
                return tmp;
            }
            else if (settings.Authentication == Authentication.HighECDSA)
            {
                if (cipher.SigningKeyPair.Key == null)
                {
                    throw new ArgumentNullException("SigningKeyPair is not set.");
                }
                GXEcdsa sig = new GXEcdsa(cipher.SigningKeyPair.Key);
                GXByteBuffer bb = new GXByteBuffer();
                bb.Set(settings.Cipher.SystemTitle);
                bb.Set(settings.SourceSystemTitle);
                if (settings.IsServer)
                {
                    bb.Set(settings.CtoSChallenge);
                    bb.Set(settings.StoCChallenge);
                }
                else
                {
                    bb.Set(settings.StoCChallenge);
                    bb.Set(settings.CtoSChallenge);
                }
                data = sig.Sign(bb.Array());
            }
            return data;
        }

        ///<summary>
        ///Generates challenge.
        ///</summary>
        ///<param name="authentication">
        ///Used authentication.
        ///</param>
        ///<returns>
        ///Generated challenge.
        ///</returns>
        public static byte[] GenerateChallenge(Authentication authentication)
        {
            Random r = new Random();
            // Random challenge is 8 to 64 bytes.
            // Texas Instruments accepts only 16 byte long challenge.
            // For this reason challenge size is 16 bytes at the moment.
            int len = 16;
            //            int len = r.Next(57) + 8;
            byte[] result = new byte[len];
            for (int pos = 0; pos != len; ++pos)
            {
                result[pos] = (byte)r.Next(0x7A);
            }
            return result;
        }

        /// <summary>
        /// Generate KDF.
        /// </summary>
        /// <param name="securitySuite">Used security suite.</param>
        /// <param name="z">Shared Secret.</param>
        /// <param name="algorithmID">Algorithm ID.</param>
        /// <param name="partyUInfo">Sender system title.</param>
        /// <param name="partyVInfo">Receiver system title.</param>
        /// <param name="suppPubInfo">Not used in DLMS.</param>
        /// <param name="suppPrivInfo">Not used in DLMS.</param>
        /// <returns></returns>
        public static byte[] GenerateKDF(SecuritySuite securitySuite, byte[] z,
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
            if (securitySuite == SecuritySuite.Ecdsa256)
            {
                using (SHA256 sha = new SHA256CryptoServiceProvider())
                {
                    return sha.ComputeHash(bb.Array());
                }
            }
            else if (securitySuite == SecuritySuite.Ecdsa384)
            {
                using (SHA384 sha = new SHA384CryptoServiceProvider())
                {
                    return sha.ComputeHash(bb.Array());
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid sevurity suite.");
            }
        }

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

        /// <summary>
        ///  Get Ephemeral Public Key Signature.
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
    }
}
