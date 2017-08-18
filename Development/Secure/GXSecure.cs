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


namespace Gurux.DLMS.Secure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
#if !WINDOWS_UWP
    using System.Security.Cryptography;
#endif
    using Gurux.DLMS.Internal;
    using Gurux.DLMS.Enums;

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
        public static byte[] Secure(GXDLMSSettings settings, GXICipher cipher, UInt32 ic, byte[] data, byte[] secret)
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
            if (settings.Authentication == Authentication.HighGMAC)
            {
                challenge.Set(data);
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
#if !WINDOWS_UWP
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
                AesGcmParameter p = new AesGcmParameter(0, Security.Authentication, ic,
                    secret, cipher.BlockCipherKey, cipher.AuthenticationKey);
                p.Type = CountType.Tag;
                challenge.Clear();
                challenge.SetUInt8((byte)Security.Authentication);
                challenge.SetUInt32((UInt32)p.InvocationCounter);
                challenge.Set(GXDLMSChippering.EncryptAesGcm(p, tmp));
                tmp = challenge.Array();
                return tmp;
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
    }
}
