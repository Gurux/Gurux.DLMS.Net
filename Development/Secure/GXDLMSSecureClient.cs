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
        ///  Server system title.
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
                Settings.PreEstablishedSystemTitle = value;
            }
        }

        /// <summary>
        /// Encrypt data using Key Encrypting Key.
        /// </summary>
        /// <param name="kek">Key Encrypting Key, also known as Master key.</param>
        /// <param name="data">Data to encrypt.</param>
        /// <returns>Encrypt data.</returns>
        public static byte[] Encrypt(byte[] kek, byte[] data)
        {
            if (kek == null)
            {
                throw new ArgumentNullException("Key Encrypting Key");
            }
            if (kek.Length < 16)
            {
                throw new ArgumentOutOfRangeException("Key Encrypting Key");
            }
            if (kek.Length % 8 != 0)
            {
                throw new ArgumentException("Key Encrypting Key");
            }
            GXDLMSChipperingStream gcm = new GXDLMSChipperingStream(true, kek);
            return gcm.EncryptAes(data);
        }

        /// <summary>
        /// Decrypt data using Key Encrypting Key.
        /// </summary>
        /// <param name="kek">Key Encrypting Key, also known as Master key.</param>
        /// <param name="data">Data to decrypt.</param>
        /// <returns>Decrypted data.</returns>
        public static byte[] Decrypt(byte[] kek, byte[] data)
        {
            if (kek == null)
            {
                throw new ArgumentNullException("Key Encrypting Key");
            }
            if (kek.Length < 16)
            {
                throw new ArgumentOutOfRangeException("Key Encrypting Key");
            }
            if (kek.Length % 8 != 0)
            {
                throw new ArgumentException("Key Encrypting Key");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length < 16)
            {
                throw new ArgumentOutOfRangeException("data");
            }
            if (data.Length % 8 != 0)
            {
                throw new ArgumentException("data");
            }
            GXDLMSChipperingStream gcm = new GXDLMSChipperingStream(false, kek);
            return gcm.DecryptAes(data);
        }
    }
}
