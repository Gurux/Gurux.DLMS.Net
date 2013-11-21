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
using System.IO;


namespace Gurux.DLMS
{        
    class GXDLMSChippering
    {        
        enum CountType
        {
            Tag = 0x1,
            Data = 0x2,
            Packet = 0x3
        }

        /// <summary>
        /// Get Nonse from frame counter and system title.
        /// </summary>
        /// <param name="FrameCounter">Frame counter.</param>
        /// <param name="systemTitle">System title.</param>
        /// <returns></returns>
        static byte[] GetNonse(UInt32 FrameCounter, byte[] systemTitle)
        {
            byte[] nonce = new byte[12];
            systemTitle.CopyTo(nonce, 0);
            byte[] tmp = BitConverter.GetBytes(FrameCounter).Reverse().ToArray();
            tmp.CopyTo(nonce, 8);
            return nonce;
        }

        static public byte[] EncryptAesGcm(Command command, Security security, UInt32 FrameCounter, byte[] systemTitle, byte[] BlockCipherKey, byte[] AuthenticationKey, byte[] plainText)
        {
            byte[] tag = null;
            return EncryptAesGcm(command, security, FrameCounter, systemTitle, BlockCipherKey, AuthenticationKey, plainText, CountType.Packet, out tag);
        }

        static private byte[] EncryptAesGcm(Command command, Security security, UInt32 FrameCounter, byte[] systemTitle, byte[] BlockCipherKey, byte[] AuthenticationKey, byte[] plainText, CountType type, out byte[] countTag)
        {
            countTag = null;
            List<byte> data = new List<byte>();
            if (type == CountType.Packet)
            {
                data.Add((byte)security);
            }
            byte[] tmp = BitConverter.GetBytes(FrameCounter).Reverse().ToArray();
            byte[] aad = GetAuthenticatedData(security, AuthenticationKey, plainText);
            GXDLMSChipperingStream gcm = new GXDLMSChipperingStream(security, true, BlockCipherKey, aad, GetNonse(FrameCounter, systemTitle), null);
            // Encrypt the secret message
            if (security != Security.Authentication)
            {                
                gcm.Write(plainText);
            }
            byte[] ciphertext = gcm.FlushFinalBlock();
            if (security == Security.Authentication)
            {
                if (type == CountType.Packet)
                {
                    data.AddRange(tmp);
                }
                if ((type & CountType.Data) != 0)
                {
                    data.AddRange(plainText);
                }
                if ((type & CountType.Tag) != 0)
                {
                    countTag = gcm.GetTag();
                    data.AddRange(countTag);                    
                }
            }
            else if (security == Security.Encryption)
            {
                data.AddRange(tmp);
                data.AddRange(ciphertext);
            }
            else if (security == Security.AuthenticationEncryption)
            {
                if (type == CountType.Packet)
                {
                    data.AddRange(tmp);
                }
                if ((type & CountType.Data) != 0)
                {
                    data.AddRange(ciphertext);
                }
                if ((type & CountType.Tag) != 0)
                {
                    countTag = gcm.GetTag();
                    data.AddRange(countTag);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("security");
            }
            if (type == CountType.Packet)
            {
                data.Insert(0, (byte)data.Count);
                data.Insert(0, (byte)command);
            }
            return data.ToArray();        
        }

        private static byte[] GetAuthenticatedData(Security security, byte[] AuthenticationKey, byte[] plainText)
        {
            if (security == Security.Authentication)
            {
                List<byte> tmp2 = new List<byte>();
                tmp2.Add((byte)security);
                tmp2.AddRange(AuthenticationKey);
                tmp2.AddRange(plainText);
                return tmp2.ToArray();
            }
            else if (security == Security.Encryption)
            {
                return AuthenticationKey;
            }
            else if (security == Security.AuthenticationEncryption)
            {
                List<byte> tmp2 = new List<byte>();
                tmp2.Add((byte)security);
                tmp2.AddRange(AuthenticationKey);
                return tmp2.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Decrypt data.
        /// </summary>
        /// <param name="cryptedText">Crypted data.</param>
        /// <param name="systemTitle"></param>
        /// <param name="BlockCipherKey"></param>
        /// <param name="AuthenticationKey"></param>
        /// <returns></returns>
        public static byte[] DecryptAesGcm(byte[] cryptedText, byte[] systemTitle, byte[] blockCipherKey, byte[] authenticationKey)
        {
            if (cryptedText == null || cryptedText.Length < 2)
            {
                throw new ArgumentOutOfRangeException("cryptedData");
            }
            int pos = -1;
            Command cmd = (Command) cryptedText[++pos];
            if (!(cmd == Command.GloGetRequest ||
                cmd == Command.GloGetResponse ||
                cmd == Command.GloSetRequest ||
                cmd == Command.GloSetResponse ||
                cmd == Command.GloMethodRequest ||
                cmd == Command.GloMethodResponse))
            {
                throw new ArgumentOutOfRangeException("cryptedData");
            }
            int len = cryptedText[++pos];
            Security security = (Security)cryptedText[++pos];
            byte[] FrameCounterData = new byte[4];            
            FrameCounterData[3] = cryptedText[++pos];
            FrameCounterData[2] = cryptedText[++pos];
            FrameCounterData[1] = cryptedText[++pos];
            FrameCounterData[0] = cryptedText[++pos];
            UInt32 frameCounter = BitConverter.ToUInt32(FrameCounterData, 0);
            byte[] tag = new byte[12];
            byte[] encryptedData;
            ++pos;
            int length;
            if (security == Security.Authentication)
            {
                length = cryptedText.Length - pos - 12;
                encryptedData = new byte[length];
                Array.Copy(cryptedText, pos, encryptedData, 0, length);
                pos += length;
                Array.Copy(cryptedText, pos, tag, 0, 12);
                //Check tag.
                byte[] countTag;
                EncryptAesGcm(Command.None, security, frameCounter, systemTitle,
                    blockCipherKey, authenticationKey, encryptedData, CountType.Packet, out countTag);
                if (!GXDLMSChipperingStream.TagsEquals(tag, countTag))
                {
                    throw new GXDLMSException("Decrypt failed. Invalid tag.");
                }
                return encryptedData;
            }
            byte[] ciphertext = null;
            if (security == Security.Encryption)
            {
                length = cryptedText.Length - pos;
                ciphertext = new byte[length];
                Array.Copy(cryptedText, pos, ciphertext, 0, ciphertext.Length);
                pos += ciphertext.Length;
            }
            else if (security == Security.AuthenticationEncryption)
            {
                length = cryptedText.Length - pos - 12;
                ciphertext = new byte[length];
                Array.Copy(cryptedText, pos, ciphertext, 0, ciphertext.Length);
                pos += ciphertext.Length;
                Array.Copy(cryptedText, pos, tag, 0, 12);
                pos += tag.Length;                
            }            
            byte[] aad = GetAuthenticatedData(security, authenticationKey, cryptedText);
            GXDLMSChipperingStream gcm = new GXDLMSChipperingStream(security, false, blockCipherKey, aad, GetNonse(frameCounter, systemTitle), tag);
            gcm.Write(ciphertext);
            return gcm.FlushFinalBlock();            
        }
    }
}
