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
using System.Text;
using Gurux.DLMS.Enums;
using System;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Internal;
using System.Security.Cryptography;

namespace Gurux.DLMS.Secure
{
    public class GXDLMSSecureClient : GXDLMSClient
    {
        ///<summary>
        /// Constructor.
        ///</summary>
        public GXDLMSSecureClient() : this(false)
        {
            Ciphering = new GXCiphering(ASCIIEncoding.ASCII.GetBytes("ABCDEFGH"));
            Settings.Cipher = Ciphering;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="useLogicalNameReferencing">Is Logical or short name referencing used.</param>
        public GXDLMSSecureClient(bool useLogicalNameReferencing) : this(useLogicalNameReferencing, 16, 1, Authentication.None, null, InterfaceType.HDLC)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="useLogicalNameReferencing">Is Logical or short name referencing used.</param>
        /// <param name="clientAddress">Client address. Default is 16 (0x10)</param>
        /// <param name="serverAddress">Server ID. Default is 1.</param>
        /// <param name="authentication">Authentication type. Default is None</param>
        /// <param name="password">Password if authentication is used.</param>
        /// <param name="interfaceType">Interface type. Default is general.</param>
        public GXDLMSSecureClient(bool useLogicalNameReferencing,
                                  int clientAddress, int serverAddress, Authentication authentication,
                                  string password, InterfaceType interfaceType) : base(useLogicalNameReferencing,
                                          clientAddress, serverAddress, authentication,
                                          password, interfaceType)
        {
            Ciphering = new GXCiphering(ASCIIEncoding.ASCII.GetBytes("ABCDEFGH"));
            Settings.Cipher = Ciphering;
        }

        /// <summary>
        /// Get ciphering keys as needed.
        /// </summary>
        /// <remarks>
        /// Keys are not saved and they are asked when needed to improve the security.
        /// </remarks>
        public event KeyEventHandler OnKeys
        {
            add
            {
                Settings.CryptoNotifier.keys += value;
            }
            remove
            {
                Settings.CryptoNotifier.keys -= value;
            }
        }

        /// <summary>
        /// Encrypt or decrypt data when needed.
        /// </summary>
        /// <remarks>
        /// Hardware Security Module can be used to improve the security.
        /// </remarks>
        public event CryptoEventHandler OnCrypto
        {
            add
            {
                Settings.CryptoNotifier.crypto += value;
            }
            remove
            {
                Settings.CryptoNotifier.crypto -= value;
            }
        }

        /// <summary>
        /// Ciphering settings.
        /// </summary>
        public GXCiphering Ciphering
        {
            get;
            private set;
        }

        /// <summary>
        /// Used security suite.
        /// </summary>
        internal SecuritySuite SecuritySuite
        {
            get
            {
                return Settings.Cipher.SecuritySuite;
            }
            set
            {
                Settings.Cipher.SecuritySuite = value;
            }
        }

        /// <summary>
        /// Server system title.
        /// </summary>
        /// <remarks>
        /// Server system title is optional and it's used when Pre-established Application Associations is used.
        /// </remarks>
        public byte[] ServerSystemTitle
        {
            get
            {
                return Settings.PreEstablishedSystemTitle;
            }
            set
            {
                if (value != null && value.Length == 0)
                {
                    value = null;
                }
                if (value != null && value.Length != 8)
                {
                    throw new ArgumentOutOfRangeException("Invalid System Title.");
                }
                Settings.PreEstablishedSystemTitle = value;
            }
        }

        private static readonly byte[] IV = new byte[] { 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6 };

        /// <summary>
        /// Encrypt data using AES RFC3394 key-wrapping.
        /// </summary>
        /// <param name="kek">Key Encrypting Key, also known as Master key.</param>
        /// <param name="data">Data to encrypt.</param>
        /// <returns>Encrypt data.</returns>
        public static byte[] Encrypt(byte[] kek, byte[] data)
        {
            if (kek == null || (kek.Length != 16 && kek.Length != 32))
            {
                throw new ArgumentNullException("Invalid Key Encrypting Key");
            }
            if (data == null || (data.Length != 16 && data.Length != 32))
            {
                throw new ArgumentNullException("Invalid data.");
            }

            int n = data.Length / 8;
            byte[] block = new byte[data.Length + IV.Length];
            Array.Copy(IV, 0, block, 0, IV.Length);
            Array.Copy(data, 0, block, IV.Length, data.Length);
            byte[] buf = new byte[8 + IV.Length];
            using (Aes aes1 = Aes.Create())
            {
                aes1.Mode = CipherMode.ECB;
                aes1.Padding = PaddingMode.None;
                aes1.Key = kek;
                ICryptoTransform encryptor = aes1.CreateEncryptor();
                for (int j = 0; j != 6; j++)
                {
                    for (int i = 1; i <= n; i++)
                    {
                        Array.Copy(block, 0, buf, 0, IV.Length);
                        Array.Copy(block, 8 * i, buf, IV.Length, 8);
                        encryptor.TransformBlock(buf, 0, buf.Length, buf, 0);
                        int t = n * j + i;
                        for (int k = 1; t != 0; k++)
                        {
                            byte v = (byte)t;
                            buf[IV.Length - k] ^= v;
                            t = t >> 8;
                        }
                        Array.Copy(buf, 0, block, 0, 8);
                        Array.Copy(buf, 8, block, 8 * i, 8);
                    }
                }
                return block;
            }
        }

        /// <summary>
        /// Decrypt data using AES RFC3394 key-wrapping.
        /// </summary>
        /// <param name="kek">Key Encrypting Key, also known as Master key.</param>
        /// <param name="data">Data to decrypt.</param>
        /// <returns>Decrypted data.</returns>
        public static byte[] Decrypt(byte[] kek, byte[] input)
        {
            if (kek == null || (kek.Length != 16 && kek.Length != 32))
            {
                throw new ArgumentNullException("Invalid Key Encrypting Key");
            }
            byte[] block;
            if (input.Length > IV.Length)
            {
                block = new byte[input.Length - IV.Length];
            }
            else
            {
                block = new byte[IV.Length];
            }
            byte[] a = new byte[IV.Length];
            byte[] buf = new byte[8 + IV.Length];
            Array.Copy(input, 0, a, 0, IV.Length);
            Array.Copy(input, 0 + IV.Length, block, 0, input.Length - IV.Length);
            int n = input.Length / 8;
            n = n - 1;
            if (n == 0)
            {
                n = 1;
            }
            using (Aes aes1 = Aes.Create())
            {
                aes1.Mode = CipherMode.ECB;
                aes1.Padding = PaddingMode.None;
                aes1.Key = kek;
                ICryptoTransform decryptor = aes1.CreateDecryptor();
                for (int j = 5; j >= 0; j--)
                {
                    for (int i = n; i >= 1; i--)
                    {
                        Array.Copy(a, 0, buf, 0, IV.Length);
                        Array.Copy(block, 8 * (i - 1), buf, IV.Length, 8);
                        int t = n * j + i;
                        for (int k = 1; t != 0; k++)
                        {
                            byte v = (byte)t;
                            buf[IV.Length - k] ^= v;
                            t = t >> 8;
                        }
                        decryptor.TransformBlock(buf, 0, buf.Length, buf, 0);
                        Array.Copy(buf, 0, a, 0, 8);
                        Array.Copy(buf, 8, block, 8 * (i - 1), 8);
                    }
                }
            }
            if (!GXCommon.Compare(a, IV))
            {
                throw new ArithmeticException("AES key wrapping failed.");
            }
            return block;
        }

        /// <summary>
        /// ECDSA key agreement key is send in part of AARE.
        /// </summary>
        public bool KeyAgreementInAARE
        {
            get
            {
                return Settings.KeyAgreementInAARE;
            }
            set
            {
                Settings.KeyAgreementInAARE = value;
            }
        }
    }
}
