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

using Gurux.DLMS.ASN;
using Gurux.DLMS.Ecdsa;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;
using System;
using System.Collections.Generic;

namespace Gurux.DLMS.Secure
{
    /// <summary>
    /// Gurux DLMS/COSEM Transport security (Ciphering) settings.
    /// </summary>
    public class GXCiphering : GXICipher
    {
        /// <summary>
        /// Authentication key.
        /// </summary>
        private byte[] authenticationKey;
        /// <summary>
        /// System title.
        /// </summary>
        private byte[] systemTitle;
        /// <summary>
        /// Server System title.
        /// </summary>
        private byte[] _serverSystemTitle;
        /// <summary>
        /// Block ciphering key.
        /// </summary>
        private byte[] blockCipherKey;

        /// <summary>
        /// Broadcast block ciphering key.
        /// </summary>
        private byte[] broadcastBlockCipherKey;        

        /// <summary>
        /// Dedicated key.
        /// </summary>
        private byte[] dedicatedKey;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXCiphering()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Default values are from the Green Book.
        /// </remarks>
        /// <param name="title">System title.</param>
        public GXCiphering(byte[] title)
        {
            Security = Security.None;
            SystemTitle = title;
            BlockCipherKey = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            AuthenticationKey = new byte[] { 0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF };
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Default values are from the Green Book.
        /// </remarks>
        /// <param name="invocationCounter">Default invocation counter value. Set to Zero.</param>
        /// <param name="title">System title.</param>
        /// <param name="blockCipherKey"></param>
        /// <param name="authenticationKey"></param>
        public GXCiphering(UInt32 invocationCounter, byte[] title, byte[] blockCipherKey, byte[] authenticationKey)
        {
            Security = Security.None;
            InvocationCounter = invocationCounter;
            SystemTitle = title;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
        }

        public void CopyTo(GXCiphering target)
        {
            target.Security = Security;
            target.SecuritySuite = SecuritySuite;
            target.KeyAgreementScheme = KeyAgreementScheme;
            target.InvocationCounter = InvocationCounter;
            target.SystemTitle = SystemTitle;
            target.BlockCipherKey = BlockCipherKey;
            target.AuthenticationKey = AuthenticationKey;
        }

        /// <summary>
        /// Is test mode used.
        /// </summary>
        public bool TestMode
        {
            get;
            set;
        }

        /// <summary>
        /// Used security level.
        /// </summary>
        public Security Security
        {
            get;
            set;
        }

        /// <summary>
        /// Security level can't be changed during the connection.
        /// </summary>
        public bool SecurityChangeCheck
        {
            get;
            set;
        }

        /// <summary>
        /// Used security policy.
        /// </summary>
        public SecurityPolicy SecurityPolicy
        {
            get;
            set;
        }

        /// <summary>
        /// Used security suite.
        /// </summary>
        public SecuritySuite SecuritySuite
        {
            get;
            set;
        }

        /// <summary>
        /// Used key agreement scheme.
        /// </summary>
        public KeyAgreementScheme KeyAgreementScheme
        {
            get;
            set;
        }

        /// <summary>
        /// Invocation counter for sending.
        /// </summary>
        public UInt32 InvocationCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Available certificates.
        /// </summary>
        public List<GXx509Certificate> Certificates
        {
            get;
            set;
        }

        /// <summary>
        /// Public/private key signing key pair.
        /// </summary>
        /// <remarks>
        /// Private key is for the initializer and Public key is for the target.
        /// </remarks>
        public KeyValuePair<GXPublicKey, GXPrivateKey> SigningKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// Public/private key TLS key pair.
        /// </summary>
        /// <remarks>
        /// Private key is for the initializer and Public key is for the target.
        /// </remarks>
        public KeyValuePair<GXPublicKey, GXPrivateKey> TlsKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// Ephemeral key pair.
        /// </summary>
        public KeyValuePair<GXPublicKey, GXPrivateKey> EphemeralKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// Public/private key key agreement key pair.
        /// </summary>
        public KeyValuePair<GXPublicKey, GXPrivateKey> KeyAgreementKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// The SystemTitle is a 8 bytes (64 bit) value that identifies a partner of the communication.
        /// First 3 bytes contains the three letters manufacturer ID.
        /// The remainder of the system title holds for example a serial number.
        /// </summary>
        /// <seealso href="http://www.dlms.com/organization/flagmanufacturesids">List of manufacturer ID.</seealso>
        public byte[] SystemTitle
        {
            get
            {
                return systemTitle;
            }
            set
            {
                if (!TestMode && value != null && value.Length != 8 && value.Length != 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid System Title.");
                }
                systemTitle = value;
            }
        }

        /// <summary>
        /// Recipient system Title.
        /// </summary>
        public byte[] RecipientSystemTitle
        {
            get
            {
                return _serverSystemTitle;
            }
            set
            {
                if (!TestMode && value != null && value.Length != 8 && value.Length != 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid System Title.");
                }
                _serverSystemTitle = value;
            }
        }

        /// <summary>
        /// Each block is ciphered with this key.
        /// </summary>
        public byte[] BlockCipherKey
        {
            get
            {
                return blockCipherKey;
            }
            set
            {
                if (!TestMode && value != null && value.Length != 16 && value.Length != 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid block cipher key.");
                }
                blockCipherKey = value;
            }
        }

        /// <summary>
        /// Each broadcast block is ciphered with this key.
        /// </summary>
        public byte[] BroadcastBlockCipherKey
        {
            get
            {
                return broadcastBlockCipherKey;
            }
            set
            {
                if (!TestMode && value != null && value.Length != 16 && value.Length != 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid broadcast block cipher Key.");
                }
                broadcastBlockCipherKey = value;
            }
        }

        /// <summary>
        /// Authentication Key is 16 bytes value.
        /// </summary>
        public byte[] AuthenticationKey
        {
            get
            {
                return authenticationKey;
            }
            set
            {
                if (!TestMode && value != null && value.Length != 16 && value.Length != 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid Authentication Key.");
                }
                authenticationKey = value;
            }
        }
        /// <summary>
        /// Dedicated Key is 16 bytes value.
        /// </summary>
        public byte[] DedicatedKey
        {
            get
            {
                return dedicatedKey;
            }
            set
            {
                if (!TestMode && value != null && value.Length != 16 && value.Length != 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid DedicatedKey Key.");
                }
                dedicatedKey = value;
            }
        }

        /// <summary>
        /// Ephemeral private key of the client.
        /// </summary>
        public GXPkcs8 ClientEphemeralPrivateKey
        {
            get;
            set;
        }

        /// <summary>
        /// Ephemeral private key of the server.
        /// </summary>
        public GXPkcs8 ServerEphemeralPrivateKey
        {
            get;
            set;
        }

        internal static byte[] Encrypt(AesGcmParameter p, byte[] data)
        {
            if (p.Security != Security.None)
            {
                return GXDLMSChippering.EncryptAesGcm(p, data);
            }
            return data;
        }

        internal static byte[] Decrypt(AesGcmParameter p, GXByteBuffer data)
        {
            byte[] tmp = GXDLMSChippering.DecryptAesGcm(p, data);
            data.Clear();
            data.Set(tmp);
            return tmp;
        }

        public void Reset()
        {
            Security = Security.None;
            InvocationCounter = 0;
        }

        bool GXICipher.IsCiphered()
        {
            return Security != Security.None;
        }

        /// <summary>
        /// Generate GMAC password from given challenge.
        /// </summary>
        /// <param name="challenge"></param>
        /// <returns></returns>
        public byte[] GenerateGmacPassword(byte[] challenge)
        {
            AesGcmParameter p = new AesGcmParameter(0x10, Security.Authentication,
                SecuritySuite,
                InvocationCounter,
                systemTitle,
                BlockCipherKey,
                AuthenticationKey);
            GXByteBuffer bb = new GXByteBuffer();
            GXDLMSChippering.EncryptAesGcm(p, challenge);
            bb.SetUInt8(0x10);
            bb.SetUInt32(InvocationCounter);
            bb.Set(p.CountTag);
            return bb.Array();
        }
    }
}
