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
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Ecdsa;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{

    /// <summary>
    /// Crypto key parameter is used to get public or private key.
    /// </summary>
    public class GXCryptoKeyParameter
    {
        /// <summary>
        /// Crypto key type.
        /// </summary>
        public CryptoKeyType KeyType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Is data encrypted or decrypted.
        /// </summary>
        public bool Encrypt
        {
            get;
            internal set;
        }

        /// <summary>
        /// Encrypted data.
        /// </summary>
        public byte[] Encrypted
        {
            get;
            internal set;
        }

        /// <summary>
        /// Decrypted data.
        /// </summary>
        public byte[] PlainText
        {
            get;
            internal set;
        }

        /// <summary>
        /// Used security suite.
        /// </summary>
        public SecuritySuite SecuritySuite
        {
            get;
            internal set;
        }

        /// <summary>
        /// Used security policy.
        /// </summary>
        public SecurityPolicy SecurityPolicy
        {
            get;
            internal set;
        }

        /// <summary>
        /// Used certificate type.
        /// </summary>
        public CertificateType CertificateType
        {
            get;
            internal set;
        }

        /// <summary>
        /// System title
        /// </summary>
        public byte[] SystemTitle
        {
            get;
            internal set;
        }

        /// <summary>
        /// Recipient system title.
        /// </summary>
        public byte[] RecipientSystemTitle
        {
            get;
            internal set;
        }

        /// <summary>
        /// Block cipher key.
        /// </summary>
        public byte[] BlockCipherKey
        {
            get;
            set;
        }

        /// <summary>
        /// Authentication key.
        /// </summary>
        public byte[] AuthenticationKey
        {
            get;
            set;
        }

        /// <summary>
        /// Frame counter. Invocation counter.
        /// </summary>
        public UInt32 InvocationCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Transaction Id.
        /// </summary>
        public byte[] TransactionId
        {
            get;
            set;
        }

        /// <summary>
        /// Private key to used to encrypt the data.
        /// </summary>
        public GXPrivateKey PrivateKey
        {
            get;
            set;
        }
        /// <summary>
        /// Public key to used to decrypt the data.
        /// </summary>
        public GXPublicKey PublicKey
        {
            get;
            set;
        }
    }
}
