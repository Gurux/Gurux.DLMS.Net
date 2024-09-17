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

using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using System;
using System.Security.Cryptography;

namespace Gurux.DLMS.Secure
{
    internal class GXGMac
    {
        private readonly ICryptoTransform _encrypt;
        private readonly ICryptoTransform _authentication = null;
        private readonly Aes _aes;
        private byte[] _H;
        private readonly byte[] _authenticationKey;

        public GXGMac(
            byte[] authenticationKey,
            byte[] cipherKey,
            byte[] nonce,
            UInt32 counter)
        {
            _authenticationKey = authenticationKey;
            _aes = Aes.Create();
            _aes.Mode = CipherMode.ECB;
            _aes.Padding = PaddingMode.None;
            _encrypt = new GXGCMCryptoTransform(_aes, cipherKey, nonce, counter, 2);
            if (_authenticationKey != null)
            {
                _authentication = new GXGCMCryptoTransform(_aes, cipherKey, nonce, counter, 1);
                using (var tmp = new GXGCMCryptoTransform(_aes, cipherKey, new byte[12], 0, 0))
                {
                    _H = new byte[16];
                    tmp.TransformBlock(new byte[16], 0, 16, _H, 0);
                }
            }
        }

        /// <summary>
        /// Make Xor for 128 bits.
        /// </summary>
        /// <param name="block">block.</param>
        /// <param name="value"></param>
        private static void Xor(byte[] block, byte[] value)
        {
            for (int pos = 0; pos != 16; ++pos)
            {
                block[pos] ^= value[pos];
            }
        }

        /// <summary>
        /// Convert uint32 to Big Endian byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buff"></param>
        /// <param name="offset"></param>
        static void UInt32ToBe(uint value, byte[] buff, int offset)
        {
            buff[offset] = (byte)(value >> 24);
            buff[++offset] = (byte)(value >> 16);
            buff[++offset] = (byte)(value >> 8);
            buff[++offset] = (byte)(value);
        }

        /// <summary>
        /// Shift block to right.
        /// </summary>
        /// <param name="block"></param>
        private static void ShiftRight(byte[] block)
        {
            uint val = BeToUInt32(block, 12);
            val >>= 1;
            if ((block[11] & 1) != 0)
            {
                val |= 0x80000000;
            }
            UInt32ToBe(val, block, 12);
            val = BeToUInt32(block, 8);
            val >>= 1;
            if ((block[7] & 1) != 0)
            {
                val |= 0x80000000;
            }
            UInt32ToBe(val, block, 8);
            val = BeToUInt32(block, 4);
            val >>= 1;
            if ((block[3] & 1) != 0)
            {
                val |= 0x80000000;
            }
            UInt32ToBe(val, block, 4);
            val = BeToUInt32(block, 0);
            val >>= 1;
            UInt32ToBe(val, block, 0);
        }


        /// <summary>
        /// Convert Big Endian byte array to Little Endian UInt 32.
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static uint BeToUInt32(byte[] buff, int offset)
        {
            return (uint)((buff[offset] << 24) | buff[++offset] << 16 | (buff[++offset] << 8) | buff[++offset]);
        }

        private void MultiplyH(byte[] y, byte[] h)
        {
            byte[] tmp = new byte[16];
            byte[] z = new byte[16];
            Array.Copy(h, tmp, h.Length);
            //Loop every byte.
            for (int i = 0; i != 16; ++i)
            {
                //Loop every bit.
                for (int j = 0; j != 8; j++)
                {
                    if ((y[i] & (1 << (7 - j))) != 0)
                    {
                        Xor(z, tmp);
                    }
                    //If last bit.
                    if ((tmp[15] & 0x01) != 0)
                    {
                        ShiftRight(tmp);
                        tmp[0] ^= 0xe1;
                    }
                    else
                    {
                        ShiftRight(tmp);
                    }
                }
            }
            Array.Copy(z, y, y.Length);
        }

        /// <summary>
        /// Get GHASH.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>GHASH value.</returns>
        private byte[] GetGHash(byte[] value)
        {
            byte[] Y = new byte[16];
            for (int pos = 0; pos < value.Length; pos += 16)
            {
                byte[] X = new byte[16];
                int cnt = System.Math.Min(value.Length - pos, 16);
                Array.Copy(value, pos, X, 0, cnt);
                Xor(Y, X);
                MultiplyH(Y, _H);
            }
            return Y;
        }

        /// <summary>
        /// Count tag for the data.
        /// </summary>
        /// <param name="tag">Used tag.</param>
        /// <param name="plainText">Plain text.</param>
        /// <param name="cipheredData">Ciphered data.</param>
        /// <returns>Tag value.</returns>
        byte[] GetTag(byte tag, byte[] plainText, byte[] cipheredData)
        {
            byte[] zero = new byte[16];
            GXByteBuffer bb = new GXByteBuffer();
            //Length of the crypted data.
            double lenC = cipheredData != null ? 8 * cipheredData.Length : 0;
            //Length of the authenticated data.
            double lenA = (1 + _authenticationKey.Length) * 8;
            if (cipheredData == null)
            {
                //If data is not ciphered.
                lenA += 8 * plainText.Length;
            }
            int v = (int)(128 * Math.Ceiling(lenA / 128) - lenA);
            int u = (int)(128 * Math.Ceiling(lenC / 128) - lenC);
            bb.SetUInt8(tag);
            bb.Set(_authenticationKey, 0, _authenticationKey.Length);
            if (cipheredData == null)
            {
                bb.Set(plainText, 0, plainText.Length);
            }
            //Fill with zeroes.
            bb.Set(zero, 0, v / 8);
            //Write Ciphered data.
            if (cipheredData != null)
            {
                bb.Set(cipheredData, 0, cipheredData.Length);
                //Fill with zeroes.
                bb.Set(zero, 0, u / 8);
            }
            //Write length of the autheticated data in bits.
            bb.SetUInt64((UInt32)lenA);
            //Write length of the crypted data.
            bb.SetUInt64((UInt32)lenC);
            byte[] tmp = GetGHash(bb.Array());
            var auth = new byte[tmp.Length];
            _authentication.TransformBlock(tmp, 0, tmp.Length, auth, 0);
            tmp = new byte[12];
            Array.Copy(auth, tmp, 12);
            return tmp;
        }

        /// <summary>
        /// Encrypt the plain text.
        /// </summary>
        /// <param name="data">Encrypted data.</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] data, byte tag)
        {
            byte[] tmp;
            switch ((byte)(tag & 0xF0))
            {
                case (byte)Security.Encryption:
                    {
                        tmp = new byte[data.Length];
                        _encrypt.TransformBlock(data, 0, data.Length, tmp, 0);
                    }
                    break;
                case (byte)Security.Authentication:
                    tmp = GetTag(tag, data, null);
                    break;
                case (byte)Security.AuthenticationEncryption:
                case 0x70://Broadcast.
                    byte[] encrypted = new byte[data.Length];
                    _encrypt.TransformBlock(data, 0, data.Length, encrypted, 0);
                    byte[] tmp2 = GetTag(tag, data, encrypted);
                    tmp = new byte[12 + data.Length];
                    //Add encrypted data.
                    Array.Copy(encrypted, tmp, data.Length);
                    //Add tag.
                    Array.Copy(tmp2, 0, tmp, data.Length, 12);
                    break;
                default:
                    throw new ArgumentException();
            }
            return tmp;
        }

        /// <summary>
        /// Decrypt the data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tag"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        /// <exception cref="GXDLMSCipherException"></exception>
        internal byte[] Decrypt(byte[] data, byte tag, GXDLMSTranslatorStructure xml)
        {
            int len = data.Length;
            if ((tag & 0xF0) != (byte)Security.Encryption)
            {
                //Remove tag from the decrypted data.
                len -= 12;
            }
            byte[] value = new byte[len];
            _encrypt.TransformBlock(data, 0, len, value, 0);
            if ((tag & 0x30) != (byte)Security.Encryption)
            {
                byte[] readTag = new byte[12];
                Array.Copy(data, len, readTag, 0, 12);
                byte[] ciphered = new byte[len];
                Array.Copy(data, ciphered, len);
                byte[] countTag = GetTag(tag, null, ciphered);
                if (!GXCommon.Compare(readTag, countTag))
                {
                    if (xml == null)
                    {
                        throw new GXDLMSCipherException(Properties.Resources.DecryptFailedInvalidAuthenticationTag);
                    }
                    xml.AppendComment(Properties.Resources.DecryptFailedInvalidAuthenticationTag);
                }
            }
            return value;
        }
    }
}
