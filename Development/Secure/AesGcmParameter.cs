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
using System.Text;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Secure
{
    internal class AesGcmParameter
    {
        /// <summary>
        /// Enumerated security level.
        /// </summary>
        private Security _security;

        public byte Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Used transaction ID.
        /// </summary>
        public UInt64 TransactionId
        {
            get;
            set;
        }


        public GXDLMSSettings Settings
        {
            get;
            set;
        }

        /// <summary>
        /// Enumerated security level.
        /// </summary>
        public Security Security
        {
            get
            {
                return _security;
            }
            set
            {
                if (value > Security.AuthenticationEncryption)
                {
                    value = Security.AuthenticationEncryption;
                }
                _security = value;
            }
        }

        public UInt64 InvocationCounter
        {
            get;
            set;
        }

        byte[] systemTitle, blockCipherKey, authenticationKey, recipientSystemTitle;

        public byte[] SystemTitle
        {
            get
            {
                return systemTitle;
            }
            set
            {
                systemTitle = value;
            }
        }

        public byte[] BlockCipherKey
        {
            get
            {
                return blockCipherKey;
            }
            set
            {              
                blockCipherKey = value;
            }
        }

        public byte[] AuthenticationKey
        {
            get
            {
                return authenticationKey;
            }
            set
            {                
                authenticationKey = value;
            }
        }

        /// <summary>
        /// Is data send as a broadcast or unicast.
        /// </summary>
        public bool Broacast
        {
            get;
            set;
        }


        /// <summary>
        /// V.44 Compression is used.
        /// </summary>
        public bool Compression
        {
            get;
            set;
        }

        public CountType Type
        {
            get;
            set;
        }
        public byte[] CountTag
        {
            get;
            set;
        }

        /// <summary>
        /// Recipient system title.
        /// </summary>
        public byte[] RecipientSystemTitle
        {
            get
            {
                return recipientSystemTitle;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length == 0)
                    {
                        value = null;
                    }
                    else if (value.Length != 8)
                    {
                        throw new ArgumentOutOfRangeException("Invalid recipient system title. Recipient system title size is 8 bytes.");
                    }
                }
                recipientSystemTitle = value;
            }
        }

        /// <summary>
        /// Date time.
        /// </summary>
        public byte[] DateTime
        {
            get;
            set;
        }
        /// <summary>
        /// Other information.
        /// </summary>
        public byte[] OtherInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Key parameters.
        /// </summary>
        public int KeyParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Key ciphered data.
        /// </summary>
        public byte[] KeyCipheredData
        {
            get;
            set;
        }


        /// <summary>
        /// Ciphered content.
        /// </summary>
        public byte[] CipheredContent
        {
            get;
            set;
        }

        /// <summary>
        /// Signature.
        /// </summary>
        public byte[] Signature
        {
            get;
            set;
        }


        /// <summary>
        ///       Used security suite.
        /// </summary>
        public SecuritySuite SecuritySuite
        {
            get;
            set;
        }

        ///<summary>
        /// xml settings. This is used only on xml parser.
        ///</summary>
        internal GXDLMSTranslatorStructure Xml
        {
            get;
            set;
        }

        /// <summary>
        /// System title is not send on pre-established connections.
        /// </summary>
        public bool IgnoreSystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tag">Tag.</param>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="security">Security level.</param>
        /// <param name="securitySuite">Security suite.</param>
        /// <param name="invocationCounter">Invocation counter.</param>
        /// <param name="kdf">KDF.</param>
        /// <param name="authenticationKey">Authentication key.</param>
        /// <param name="originatorSystemTitle">Originator system title.</param>
        /// <param name="recipientSystemTitle">Recipient system title.</param>
        /// <param name="dateTime"> Date and time.</param>
        /// <param name="otherInformation">Other information.</param>
        public AesGcmParameter(byte tag,
                GXDLMSSettings settings,
                Security security,
                SecuritySuite securitySuite,
                UInt64 invocationCounter,
                byte[] kdf,
                byte[] authenticationKey,
                byte[] originatorSystemTitle,
                byte[] recipientSystemTitle,
                byte[] dateTime,
                byte[] otherInformation)
        {
            Tag = tag;
            Settings = settings;
            Security = security;
            InvocationCounter = invocationCounter;
            SecuritySuite = securitySuite;
            BlockCipherKey = kdf;
            AuthenticationKey = authenticationKey;
            SystemTitle = originatorSystemTitle;
            RecipientSystemTitle = recipientSystemTitle;
            Type = CountType.Packet;
            DateTime = dateTime;
            OtherInformation = otherInformation;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tag">Command.</param>
        /// <param name="security">Security level.</param>
        /// <param name="invocationCounter">Invocation counter.</param>
        /// <param name="systemTitle"></param>
        /// <param name="blockCipherKey"></param>
        /// <param name="authenticationKey"></param>
        public AesGcmParameter(
            byte tag,
            GXDLMSSettings settings,
            Security security,
            SecuritySuite securitySuite,
            UInt32 invocationCounter,
            byte[] systemTitle,
            byte[] blockCipherKey,
            byte[] authenticationKey)
        {
            Tag = tag;
            Settings = settings;
            Security = security;
            InvocationCounter = invocationCounter;
            SystemTitle = systemTitle;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
            Type = CountType.Packet;
            SecuritySuite = securitySuite;
            if (settings != null)
            {
                Broacast = settings.Broacast;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="systemTitle"></param>
        /// <param name="blockCipherKey"></param>
        /// <param name="authenticationKey"></param>
        public AesGcmParameter(
            GXDLMSSettings settings,
            byte[] systemTitle,
            byte[] blockCipherKey,
            byte[] authenticationKey)
        {
            Security = settings.Cipher.Security;
            Settings = settings;
            SystemTitle = systemTitle;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
            Type = CountType.Packet;
            SecuritySuite = settings.Cipher.SecuritySuite;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Security: ");
            sb.Append(Security);
            sb.Append(" Invocation Counter: ");
            sb.Append(InvocationCounter);
            sb.Append(" SystemTitle: ");
            sb.Append(GXCommon.ToHex(SystemTitle, true));
            sb.Append(" AuthenticationKey: ");
            sb.Append(GXCommon.ToHex(AuthenticationKey, true));
            sb.Append(" BlockCipherKey: ");
            sb.Append(GXCommon.ToHex(BlockCipherKey, true));
            return sb.ToString();
        }
    }
}
