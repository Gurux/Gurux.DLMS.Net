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
using Gurux.DLMS.Objects.Enums;
using System;
using System.Collections.Generic;

namespace Gurux.DLMS.Secure
{
    internal interface GXICipher
    {
        /// <summary>
        /// Reset encrypt settings.
        /// </summary>
        void Reset();

        /// <summary>
        /// Is ciphering used.
        /// </summary>
        /// <returns></returns>
        bool IsCiphered();

        /// <summary>
        /// Used security level.
        /// </summary>
        Security Security
        {
            get;
            set;
        }

        /// <summary>
        /// Used security suite.
        /// </summary>
        SecuritySuite SecuritySuite
        {
            get;
            set;
        }

        /// <summary>
        /// Used key agreement scheme.
        /// </summary>
        KeyAgreementScheme KeyAgreementScheme
        {
            get;
            set;
        }

        /// <summary>
        /// System title.
        /// </summary>
        byte[] SystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Recipient system title.
        /// </summary>
        byte[] RecipientSystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Block cipher key.
        /// </summary>
        byte[] BlockCipherKey
        {
            get;
            set;
        }

        /// <summary>
        /// Authentication key.
        /// </summary>
        byte[] AuthenticationKey
        {
            get;
            set;
        }

        /// <summary>
        /// Dedicated key.
        /// </summary>
        byte[] DedicatedKey
        {
            get;
            set;
        }

        /// <summary>
        /// Frame counter. Invocation counter.
        /// </summary>
        UInt32 InvocationCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Ephemeral key pair.
        /// </summary>
        KeyValuePair<GXPrivateKey, GXPublicKey> EphemeralKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// Client's key agreement key pair.
        /// </summary>
        KeyValuePair<GXPrivateKey, GXPublicKey> KeyAgreementKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// Available certificates.
        /// </summary>
        List<GXx509Certificate> Certificates
        {
            get;
        }

        KeyValuePair<GXPrivateKey, GXPublicKey> SigningKeyPair
        {
            get;
            set;
        }
    }
}
