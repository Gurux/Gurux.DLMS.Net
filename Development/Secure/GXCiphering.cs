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
using Gurux.DLMS.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

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
        /// Block ciphering key.
        /// </summary>
        private byte[] blockCipherKey;

        /// <summary>
        /// Dedicated key.
        /// </summary>
        private byte[] dedicatedKey;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Default values are from the Green Book.
        /// </remarks>
        /// <param name="title">System title.</param>
        public GXCiphering(byte[] title)
        {
            Security = (byte)Enums.Security.None;
            SystemTitle = title;
            BlockCipherKey = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            AuthenticationKey = new byte[] { 0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF };
            PublicKeys = new List<KeyValuePair<CertificateType, GXx509Certificate>>();
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
            Security = (byte)Enums.Security.None;
            InvocationCounter = invocationCounter;
            SystemTitle = title;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
            PublicKeys = new List<KeyValuePair<CertificateType, GXx509Certificate>>();
        }

        public void CopyTo(GXCiphering target)
        {
            target.Security = Security;
            target.SecuritySuite = SecuritySuite;
            target.InvocationCounter = InvocationCounter;
            target.SystemTitle = SystemTitle;
            target.BlockCipherKey = BlockCipherKey;
            target.AuthenticationKey = AuthenticationKey;
            if (PublicKeys != null)
            {
                target.PublicKeys.AddRange(PublicKeys);
            }
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
        /// Used security policy.
        /// </summary>
        public byte Security
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
        /// Invocation counter for sending.
        /// </summary>
        public UInt32 InvocationCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Target (Server or client) Public key.
        /// </summary>
        public List<KeyValuePair<CertificateType, GXx509Certificate>> PublicKeys
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

        public byte[] SharedSecret
        {
            get;
            set;
        }

        /// <summary>
        /// Public/private key signing key pair.
        /// </summary>
        public KeyValuePair<GXPrivateKey, GXPublicKey> SigningKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// Ephemeral key pair.
        /// </summary>
        public KeyValuePair<GXPrivateKey, GXPublicKey> EphemeralKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// Public/private key key agreement key pair.
        /// </summary>
        public KeyValuePair<GXPrivateKey, GXPublicKey> KeyAgreementKeyPair
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
                    throw new ArgumentOutOfRangeException("Invalid Block Cipher Key.");
                }
                blockCipherKey = value;
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
        /// Authentication Key is 16 bytes value.
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


        internal static byte[] Encrypt(AesGcmParameter p, byte[] data)
        {
            if (p.Security != (byte)Enums.Security.None)
            {
                byte[] tmp = GXDLMSChippering.EncryptAesGcm(p, data);
                return tmp;
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
            Security = 0;
            InvocationCounter = 0;
        }

        bool GXICipher.IsCiphered()
        {
            return Security != 0;
        }

        /// <summary>
        /// Generate GMAC password from given challenge.
        /// </summary>
        /// <param name="challenge"></param>
        /// <returns></returns>
        public byte[] GenerateGmacPassword(byte[] challenge)
        {
            AesGcmParameter p = new AesGcmParameter(0x10, (byte)Enums.Security.Authentication, InvocationCounter,
                                                       systemTitle, BlockCipherKey, AuthenticationKey);
            GXByteBuffer bb = new GXByteBuffer();
            GXDLMSChippering.EncryptAesGcm(p, challenge);
            bb.SetUInt8(0x10);
            bb.SetUInt32(InvocationCounter);
            bb.Set(p.CountTag);
            return bb.Array();
        }
    }
}
