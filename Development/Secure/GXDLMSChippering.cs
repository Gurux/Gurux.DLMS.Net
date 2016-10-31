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
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;


namespace Gurux.DLMS.Secure
{
    internal class GXDLMSChippering
    {
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

        static internal byte[] EncryptAesGcm(AesGcmParameter param, byte[] plainText)
        {
            System.Diagnostics.Debug.WriteLine("Encrypt settings: " + param.ToString());
            param.CountTag = null;
            GXByteBuffer data = new GXByteBuffer();
            if (param.Type == CountType.Packet)
            {
                data.SetUInt8((byte)param.Security);
            }
            byte[] tmp = BitConverter.GetBytes(param.FrameCounter).Reverse().ToArray();
            byte[] aad = GetAuthenticatedData(param.Security, param.AuthenticationKey, plainText);
            GXDLMSChipperingStream gcm = new GXDLMSChipperingStream(param.Security, true, param.BlockCipherKey,
                    aad, GetNonse(param.FrameCounter, param.SystemTitle), null);
            // Encrypt the secret message
            if (param.Security != Gurux.DLMS.Enums.Security.Authentication)
            {
                gcm.Write(plainText);
            }
            byte[] ciphertext = gcm.FlushFinalBlock();
            if (param.Security == Gurux.DLMS.Enums.Security.Authentication)
            {
                if (param.Type == CountType.Packet)
                {
                    data.Set(tmp);
                }
                if ((param.Type & CountType.Data) != 0)
                {
                    data.Set(plainText);
                }
                if ((param.Type & CountType.Tag) != 0)
                {
                    param.CountTag = gcm.GetTag();
                    data.Set(param.CountTag);
                }
            }
            else if (param.Security == Gurux.DLMS.Enums.Security.Encryption)
            {
                if (param.Type == CountType.Packet)
                {
                    data.Set(tmp);
                }
                data.Set(ciphertext);
            }
            else if (param.Security == Gurux.DLMS.Enums.Security.AuthenticationEncryption)
            {
                if (param.Type == CountType.Packet)
                {
                    data.Set(tmp);
                }
                if ((param.Type & CountType.Data) != 0)
                {
                    data.Set(ciphertext);
                }
                if ((param.Type & CountType.Tag) != 0)
                {
                    param.CountTag = gcm.GetTag();
                    data.Set(param.CountTag);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("security");
            }
            if (param.Type == CountType.Packet)
            {
                GXByteBuffer tmp2 = new GXByteBuffer((ushort)(10 + data.Size));
                tmp2.SetUInt8(param.Tag);
                GXCommon.SetObjectCount(data.Size, tmp2);
                tmp2.Set(data.Array());
                return tmp2.Array();
            }
            byte[] crypted = data.Array();
            System.Diagnostics.Debug.WriteLine("Crypted: " + GXCommon.ToHex(crypted, true));
            return crypted;
        }

        private static byte[] GetAuthenticatedData(Gurux.DLMS.Enums.Security security, byte[] AuthenticationKey, byte[] plainText)
        {
            if (security == Gurux.DLMS.Enums.Security.Authentication)
            {
                GXByteBuffer tmp2 = new GXByteBuffer();
                tmp2.SetUInt8((byte)security);
                tmp2.Set(AuthenticationKey);
                tmp2.Set(plainText);
                return tmp2.Array();
            }
            else if (security == Gurux.DLMS.Enums.Security.Encryption)
            {
                return AuthenticationKey;
            }
            else if (security == Gurux.DLMS.Enums.Security.AuthenticationEncryption)
            {
                GXByteBuffer tmp2 = new GXByteBuffer();
                tmp2.SetUInt8((byte)security);
                tmp2.Set(AuthenticationKey);
                return tmp2.Array();
            }
            return null;
        }

        /// <summary>
        /// Decrypt data.
        /// </summary>
        /// <param name="p">Decryption parameters</param>
        /// <returns>Decrypted data.</returns>
        public static byte[] DecryptAesGcm(AesGcmParameter p, GXByteBuffer data)
        {
            if (data == null || data.Size < 2)
            {
                throw new ArgumentOutOfRangeException("cryptedData");
            }
            int len;
            Command cmd = (Command)data.GetUInt8();
            switch (cmd)
            {
                case Command.GloGeneralCiphering:
                    len = GXCommon.GetObjectCount(data);
                    p.SystemTitle = new byte[len];
                    data.Get(p.SystemTitle);
                    break;
                case Command.GloInitiateRequest:
                case Command.GloInitiateResponse:
                case Command.GloReadRequest:
                case Command.GloReadResponse:
                case Command.GloWriteRequest:
                case Command.GloWriteResponse:
                case Command.GloGetRequest:
                case Command.GloGetResponse:
                case Command.GloSetRequest:
                case Command.GloSetResponse:
                case Command.GloMethodRequest:
                case Command.GloMethodResponse:
                case Command.GloEventNotificationRequest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("cryptedData");
            }
            len = Gurux.DLMS.Internal.GXCommon.GetObjectCount(data);
            p.Security = (Gurux.DLMS.Enums.Security)data.GetUInt8();
            p.FrameCounter = data.GetUInt32();
            System.Diagnostics.Debug.WriteLine("Decrypt settings: " + p.ToString());
            System.Diagnostics.Debug.WriteLine("Encrypted: " + GXCommon.ToHex(data.Array(), true));

            byte[] tag = new byte[12];
            byte[] encryptedData;
            int length;
            if (p.Security == Gurux.DLMS.Enums.Security.Authentication)
            {
                length = data.Size - data.Position - 12;
                encryptedData = new byte[length];
                data.Get(encryptedData);
                data.Get(tag);
                // Check tag.
                EncryptAesGcm(p, encryptedData);
                if (!GXDLMSChipperingStream.TagsEquals(tag, p.CountTag))
                {
                    throw new GXDLMSException("Decrypt failed. Invalid tag.");
                }
                return encryptedData;
            }
            byte[] ciphertext = null;
            if (p.Security == Gurux.DLMS.Enums.Security.Encryption)
            {
                length = data.Size - data.Position;
                ciphertext = new byte[length];
                data.Get(ciphertext);
            }
            else if (p.Security == Gurux.DLMS.Enums.Security.AuthenticationEncryption)
            {
                length = data.Size - data.Position - 12;
                ciphertext = new byte[length];
                data.Get(ciphertext);
                data.Get(tag);
            }
            byte[] aad = GetAuthenticatedData(p.Security, p.AuthenticationKey, data.Array());
            byte[] iv = GetNonse(p.FrameCounter, p.SystemTitle);
            GXDLMSChipperingStream gcm = new GXDLMSChipperingStream(p.Security, true, p.BlockCipherKey, aad, iv, tag);
            gcm.Write(ciphertext);
            return gcm.FlushFinalBlock();
        }
    }
}
