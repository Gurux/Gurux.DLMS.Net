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
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Secure
{
    internal class AesGcmParameter
    {
        public byte Tag
        {
            get;
            set;
        }
        public Gurux.DLMS.Enums.Security Security
        {
            get;
            set;
        }

        public UInt64 InvocationCounter
        {
            get;
            set;
        }

        public byte[] SystemTitle
        {
            get;
            set;
        }
        public byte[] BlockCipherKey
        {
            get;
            set;
        }
        public byte[] AuthenticationKey
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

        /**
    * Recipient system title.
    */
        public byte[] RecipientSystemTitle
        {
            get;
            set;
        }
        /**
         * Date time.
         */
        public byte[] DateTime
        {
            get;
            set;
        }
        /**
         * Other information.
         */
        public byte[] OtherInformation
        {
            get;
            set;
        }

        /**
         * Key parameters.
         */
        public int KeyParameters
        {
            get;
            set;
        }

        /**
         * Key ciphered data.
         */
        public byte[] KeyCipheredData
        {
            get;
            set;
        }

        /**
         * Ciphered content.
         */
        public byte[] CipheredContent
        {
            get;
            set;
        }

        /**
         * Shared secret is generated when connection is made.
         */
        public byte[] SharedSecret
        {
            get;
            set;
        }

        /**
         * Used security suite.
         */
        public SecuritySuite SecuritySuite
        {
            get;
            set;
        }

        //<summary>
        /// xml settings. This is used only on xml parser.
        ///</summary>
        internal GXDLMSTranslatorStructure Xml
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tag">Command.</param>
        /// <param name="security"></param>
        /// <param name="invocationCounter">Invocation counter.</param>
        /// <param name="systemTitle"></param>
        /// <param name="blockCipherKey"></param>
        /// <param name="authenticationKey"></param>
        public AesGcmParameter(
            byte tag,
            Gurux.DLMS.Enums.Security security,
            UInt32 invocationCounter,
            byte[] systemTitle,
            byte[] blockCipherKey,
            byte[] authenticationKey)
        {
            Tag = tag;
            Security = security;
            InvocationCounter = invocationCounter;
            SystemTitle = systemTitle;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
            Type = CountType.Packet;
            SecuritySuite = SecuritySuite.AesGcm128;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="systemTitle"></param>
        /// <param name="blockCipherKey"></param>
        /// <param name="authenticationKey"></param>
        public AesGcmParameter(
            byte[] systemTitle,
            byte[] blockCipherKey,
            byte[] authenticationKey)
        {
            SystemTitle = systemTitle;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
            Type = CountType.Packet;
            SecuritySuite = SecuritySuite.AesGcm128;
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
