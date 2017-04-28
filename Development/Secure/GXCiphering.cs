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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;

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
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Default values are from the Green Book.
        /// </remarks>
        /// <param name="title">System title.</param>
        public GXCiphering(byte[] title)
        {
            Security = Gurux.DLMS.Enums.Security.None;
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
            Security = Gurux.DLMS.Enums.Security.None;
            InvocationCounter = invocationCounter;
            SystemTitle = title;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
        }

        /// <summary>
        /// Used security.
        /// </summary>
        public Gurux.DLMS.Enums.Security Security
        {
            get;
            set;
        }

        /// <summary>
        /// Invocation counter.
        /// </summary>
        public UInt32 InvocationCounter
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
                if (value != null && value.Length != 8 && value.Length != 0)
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
                if (value != null && value.Length != 16 && value.Length != 0)
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
                if (value != null && value.Length != 16 && value.Length != 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid Authentication Key.");
                }
                authenticationKey = value;
            }
        }

        byte[] GXICipher.Encrypt(byte tag, byte[] title, byte[] data)
        {
            if (Security != Gurux.DLMS.Enums.Security.None)
            {
                AesGcmParameter p = new AesGcmParameter(tag, Security, InvocationCounter,
                                                        title, BlockCipherKey, AuthenticationKey);
                byte[] tmp = GXDLMSChippering.EncryptAesGcm(p, data);
                ++InvocationCounter;
                return tmp;
            }
            return data;
        }

        Gurux.DLMS.Enums.Security GXICipher.Decrypt(byte[] title, GXByteBuffer data)
        {
            AesGcmParameter p = new AesGcmParameter(title, BlockCipherKey, AuthenticationKey);
            byte[] tmp = GXDLMSChippering.DecryptAesGcm(p, data);
            data.Clear();
            data.Set(tmp);
            return p.Security;
        }

        public void Reset()
        {
            Security = Gurux.DLMS.Enums.Security.None;
            InvocationCounter = 0;
        }

        bool GXICipher.IsCiphered()
        {
            return Security != Gurux.DLMS.Enums.Security.None;
        }

        /// <summary>
        /// Generate GMAC password from given challenge.
        /// </summary>
        /// <param name="challenge"></param>
        /// <returns></returns>
        public byte[] GenerateGmacPassword(byte[] challenge)
        {
            AesGcmParameter p = new AesGcmParameter(0x10, Gurux.DLMS.Enums.Security.Authentication, InvocationCounter,
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
