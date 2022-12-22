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
using Gurux.DLMS.Internal;
using System.Collections.Generic;
using System.Xml;
#if !WINDOWS_UWP
using System.Xml.XPath;
#endif
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;
using System.ComponentModel;
using System.Diagnostics;
using Gurux.DLMS.Plc.Enums;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.ASN;
using Gurux.DLMS.Ecdsa;

namespace Gurux.DLMS
{

    ///<summary>
    ///This class is used to translate DLMS frame or PDU to xml.
    ///</summary>
    public class GXDLMSTranslator
    {
        byte[] systemTitle, blockCipherKey, authenticationKey, dedicatedKey, serverSystemTitle;
        GXCryptoNotifier cryptoNotifier = new GXCryptoNotifier();
        UInt16 SAck = 0, RAck = 0;
        bool sending = false;
        byte SSendSequence = 0;
        byte SReceiveSequence = 0;
        byte RSendSequence = 0;
        byte RReceiveSequence = 0;

        internal SortedList<int, string> tags = new SortedList<int, string>();
        internal SortedList<string, int> tagsByName = new SortedList<string, int>();

        /// <summary>
        /// Sending data in multiple frames.
        /// </summary>
        private bool multipleFrames = false;
        /// <summary>
        /// If only PDUs are shown and PDU is received on parts.
        /// </summary>
        private GXByteBuffer pduFrames = new GXByteBuffer();

        /// <summary>
        /// Get ciphering keys as needed.
        /// </summary>
        /// <remarks>
        /// Keys are not saved and they are asked when needed to improve the security.
        /// </remarks>
        public event KeyEventHandler OnKeys
        {
            add
            {
                cryptoNotifier.keys += value;
            }
            remove
            {
                cryptoNotifier.keys -= value;
            }
        }

        /// <summary>
        /// Encrypt or decrypt data when needed.
        /// </summary>
        /// <remarks>
        /// Hardware Security Module can be used to improve the security.
        /// </remarks>
        public event CryptoEventHandler OnCrypto
        {
            add
            {
                cryptoNotifier.crypto += value;
            }
            remove
            {
                cryptoNotifier.crypto -= value;
            }
        }

        /// <summary>
        /// Are comments added.
        /// </summary>
        public bool Comments
        {
            get;
            set;
        }

        /// <summary>
        /// Is only PDU shown when data is parsed with MessageToXml
        /// </summary>
        /// <seealso cref="MessageToXml"/>
        /// <seealso cref="CompletePdu"/>
        [DefaultValue(false)]
        public bool PduOnly
        {
            get;
            set;
        }

        /// <summary>
        ///  Convert string to byte array.
        /// </summary>
        /// <param name="value">Hex string.</param>
        /// <returns>Parsed byte array.</returns>
        [DebuggerStepThrough]
        public static byte[] HexToBytes(string value)
        {
            if (value == null)
            {
                return new byte[0];
            }
            value = value.Replace("\r\n", "").Replace("\n", "");
            return GXCommon.HexToBytes(value);
        }

        /// <summary>
        /// Convert byte array to hex string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <returns>Byte array as hex string.</returns>
        [DebuggerStepThrough]
        public static string ToHex(byte[] bytes)
        {
            return GXCommon.ToHex(bytes, true);
        }

        /// <summary>
        /// Convert byte array to hex string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <param name="addSpace">Is space added between bytes.</param>
        /// <returns>Byte array as hex string.</returns>
        [DebuggerStepThrough]
        public static string ToHex(byte[] bytes, bool addSpace)
        {
            return GXCommon.ToHex(bytes, addSpace);
        }

        /// <summary>
        /// Convert byte array to hex string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <param name="addSpace">Is space added between bytes.</param>
        /// <param name="index">Start index.</param>
        /// <param name="count">Byte count.</param>
        /// <returns>Byte array as hex string.</returns>
        [DebuggerStepThrough]
        public static string ToHex(byte[] bytes, bool addSpace, int index, int count)
        {
            return GXCommon.ToHex(bytes, addSpace, index, count);
        }

        /// <summary>
        /// Is only complete PDU parsed and shown.
        /// </summary>
        /// <seealso cref="MessageToXml"/>
        /// <seealso cref="PduOnly"/>
        [DefaultValue(false)]
        public bool CompletePdu
        {
            get;
            set;
        }


        /// <summary>
        /// Are numeric values shown as hex.
        /// </summary>
        [DefaultValue(true)]
        public bool Hex
        {
            get;
            set;
        }



        /// <summary>
        /// Is string serialized as hex.
        /// </summary>
        /// <seealso cref="MessageToXml"/>
        /// <seealso cref="PduOnly"/>
        [DefaultValue(false)]
        public bool ShowStringAsHex
        {
            get;
            set;
        }

        public TranslatorOutputType OutputType
        {
            get;
            private set;
        }

        /// <summary>
        /// Is XML declaration skipped.
        /// </summary>
        public bool OmitXmlDeclaration
        {
            get;
            set;
        }

        /// <summary>
        /// Is XML name space skipped.
        /// </summary>
        public bool OmitXmlNameSpace
        {
            get;
            set;
        }

        /// <summary>
        /// Used security.
        /// </summary>
        public Security Security
        {
            get;
            set;
        }

        /// <summary>
        /// Used security suite.
        /// </summary>
        public SecuritySuite SecuritySuite
        {
            get;
            set;
        }

        /// <summary>
        /// System title.
        /// </summary>
        public byte[] SystemTitle
        {
            get
            {
                return systemTitle;
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
                        throw new ArgumentOutOfRangeException("Invalid system title. System title size is 8 bytes.");
                    }
                }
                systemTitle = value;
            }
        }

        /// <summary>
        /// Server system title.
        /// </summary>
        public byte[] ServerSystemTitle
        {
            get
            {
                return serverSystemTitle;
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
                        throw new ArgumentOutOfRangeException("Invalid server system title. Server system title size is 8 bytes.");
                    }
                }
                serverSystemTitle = value;
            }
        }

        /// <summary>
        /// Dedicated key.
        /// </summary>
        public byte[] DedicatedKey
        {
            get
            {
                return dedicatedKey;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length == 0)
                    {
                        value = null;
                    }
                    else if (value.Length != 16)
                    {
                        throw new ArgumentOutOfRangeException("Invalid dedicated key. Dedicated key size is 16 bytes.");
                    }
                }
                dedicatedKey = value;
            }
        }


        /// <summary>
        /// Block cipher key.
        /// </summary>
        public byte[] BlockCipherKey
        {
            get
            {
                return blockCipherKey;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length == 0)
                    {
                        value = null;
                    }
                    else if (value.Length != 16)
                    {
                        throw new ArgumentOutOfRangeException("Invalid block cipher key. Block cipher key size is 16 bytes.");
                    }
                }
                blockCipherKey = value;
            }
        }

        /// <summary>
        /// Authentication key.
        /// </summary>
        public byte[] AuthenticationKey
        {
            get
            {
                return authenticationKey;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length == 0)
                    {
                        value = null;
                    }
                    else if (value.Length != 16)
                    {
                        throw new ArgumentOutOfRangeException("Invalid authentication key. Authentication key size is 16 bytes.");
                    }
                }
                authenticationKey = value;
            }
        }

        /// <summary>
        /// Invocation Counter.
        /// </summary>
        public UInt32 InvocationCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Is General Protection used.
        /// </summary>
        public bool UseGeneralProtection
        {
            get;
            set;
        }

        /// <summary>
        /// Used standard.
        /// </summary>
        public Standard Standard
        {
            get;
            set;
        }

        /// <summary>
        /// List of private keys and certifications.
        /// </summary>
        public List<KeyValuePair<GXPkcs8, GXx509Certificate>> Keys
        {
            get;
            set;
        }

        /// <summary>
        /// Public/private key signing key pair.
        /// </summary>
        /// <remarks>
        /// Private key is for the initializer and Public key is for the target.
        /// </remarks>
        public KeyValuePair<GXPublicKey, GXPrivateKey> SigningKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// Public/private key TLS key pair.
        /// </summary>
        /// <remarks>
        /// Private key is for the initializer and Public key is for the target.
        /// </remarks>
        public KeyValuePair<GXPublicKey, GXPrivateKey> TlsKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// Ephemeral key pair.
        /// </summary>
        public KeyValuePair<GXPublicKey, GXPrivateKey> EphemeralKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// Public/private key key agreement key pair.
        /// </summary>
        public KeyValuePair<GXPublicKey, GXPrivateKey> KeyAgreementKeyPair
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSTranslator() :
            this(TranslatorOutputType.SimpleXml)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">Translator output type.</param>
        public GXDLMSTranslator(TranslatorOutputType type)
        {
            Keys = new List<KeyValuePair<GXPkcs8, GXx509Certificate>>();
            OutputType = type;
            GetTags(type, tags, tagsByName);
            if (type == TranslatorOutputType.SimpleXml)
            {
                Hex = true;
            }
        }

        /// <summary>
        /// Identify used DLMS framing type.
        /// </summary>
        /// <param name="value">Input data.</param>
        /// <returns>Interface type.</returns>
        public static InterfaceType GetDlmsFraming(GXByteBuffer value)
        {
            for (int pos = value.Position; pos < value.Size; ++pos)
            {
                if (value.GetUInt8(pos) == 0x7e)
                {
                    return InterfaceType.HDLC;
                }
                if (value.Size - pos > 1 && value.GetUInt16(pos) == 1)
                {
                    return InterfaceType.WRAPPER;
                }
                if (value.GetUInt8(pos) == 0x2)
                {
                    return InterfaceType.Plc;
                }
                if (value.GetUInt8(pos) == 0x68)
                {
                    return InterfaceType.WiredMBus;
                }
                if (GXDLMS.IsWirelessMBusData(value))
                {
                    return InterfaceType.WirelessMBus;
                }
                if (GXDLMS.GetPlcSfskFrameSize(value) != 0)
                {
                    return InterfaceType.PlcHdlc;
                }
            }
            throw new ArgumentException("Invalid DLMS framing.");
        }

        /// <summary>
        /// Get HDLC sender and receiver address information.
        /// </summary>
        /// <param name="reply">Received data.</param>
        /// <param name="target">target (primary) address</param>
        /// <param name="source">Source (secondary) address.</param>
        public static void GetHdlcAddressInfo(GXByteBuffer reply, out int target, out int source)
        {
            byte type;
            GXDLMS.GetHdlcAddressInfo(reply, out target, out source, out type);
        }

        /// <summary>
        /// Get HDLC sender and receiver address information.
        /// </summary>
        /// <param name="reply">Received data.</param>
        /// <param name="target">target (primary) address</param>
        /// <param name="source">Source (secondary) address.</param>
        /// <param name="type">DLMS frame type.</param>
        public static void GetHdlcAddressInfo(GXByteBuffer reply, out int target, out int source, out byte type)
        {
            GXDLMS.GetHdlcAddressInfo(reply, out target, out source, out type);
        }

        /// <summary>
        /// Get HDLC sender and receiver address information.
        /// </summary>
        /// <param name="reply">Received data.</param>
        /// <param name="target">target (primary) address</param>
        /// <param name="source">Source (secondary) address.</param>
        public static void GetWrapperAddressInfo(GXByteBuffer reply, out int target, out int source)
        {
            int pos = reply.Position;
            try
            {
                if (reply.Available > 2 && reply.GetUInt16() == 1)
                {
                    GXDLMSSettings settings = new GXDLMSSettings(true, InterfaceType.WRAPPER);
                    GXDLMS.CheckWrapperAddress(settings, reply, new GXReplyData(), null);
                    target = settings.ServerAddress;
                    source = settings.ClientAddress;
                }
                else
                {
                    target = source = 0;
                }
            }
            finally
            {
                reply.Position = pos;
            }
        }

        public static void GetAddressInfo(InterfaceType type, GXByteBuffer reply, out int target, out int source)
        {
            target = source = 0;
            switch (type)
            {
                case InterfaceType.HDLC:
                    GetHdlcAddressInfo(reply, out target, out source);
                    break;
                case InterfaceType.WRAPPER:
                    GetWrapperAddressInfo(reply, out target, out source);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid interface type.");
            }
        }

        /// <summary>
        /// Find next frame from the string.
        /// </summary>
        /// <remarks>
        /// Position of data is set to the begin of new frame. If Pdu is null it is not updated.
        /// </remarks>
        /// <param name="data">Data where frame is search.</param>
        /// <param name="pdu">Pdu of received frame is set here.</param>
        /// <returns>Is new frame found.</returns>
        public bool FindNextFrame(GXByteBuffer data, GXByteBuffer pdu, InterfaceType type)
        {
            GXDLMSTranslatorMessage msg = new GXDLMSTranslatorMessage() { Message = data };
            msg.InterfaceType = type;
            return FindNextFrame(msg, pdu);
        }

        /// <summary>
        /// Find next frame from the string.
        /// </summary>
        /// <remarks>
        /// Position of data is set to the begin of new frame. If Pdu is null it is not updated.
        /// </remarks>
        /// <param name="msg">GXDLMS Translator message.</param>
        /// <param name="pdu">Pdu of received frame is set here.</param>
        /// <returns>Is new frame found.</returns>
        public bool FindNextFrame(GXDLMSTranslatorMessage msg, GXByteBuffer pdu)
        {
            return FindNextFrame(msg, pdu, 0, 0);
        }

        /// <summary>
        /// Find next frame from the string.
        /// </summary>
        /// <remarks>
        /// Position of data is set to the begin of new frame. If Pdu is null it is not updated.
        /// </remarks>
        /// <param name="msg">GXDLMS Translator message.</param>
        /// <param name="pdu">Pdu of received frame is set here.</param>
        /// <param name="clientAddress">If client address is set only messages that use this client address are accepted.</param>
        /// <param name="serverAddress">If server address is set only messages that use this server addresses are find.</param>
        /// <returns>Is new frame found.</returns>
        public bool FindNextFrame(GXDLMSTranslatorMessage msg, GXByteBuffer pdu, int clientAddress, int serverAddress)
        {
            msg.SourceAddress = msg.TargetAddress = 0;
            int original = msg.Message.Position;
            GXByteBuffer data = msg.Message;
            GXDLMSSettings settings = new GXDLMSSettings(true, msg.InterfaceType);
            GXReplyData reply = new GXReplyData();
            reply.MoreData = msg.MoreData;
            reply.Xml = new GXDLMSTranslatorStructure(OutputType, OmitXmlNameSpace, Hex, ShowStringAsHex, Comments, tags);
            int pos;
            reply.Data.Clear();
            bool found = false;
            try
            {
                while (data.Position != data.Size)
                {
                    if ((msg.InterfaceType == InterfaceType.HDLC ||
                        msg.InterfaceType == InterfaceType.HdlcWithModeE) &&
                        data.GetUInt8(data.Position) == 0x7e)
                    {
                        pos = data.Position;
                        found = GXDLMS.GetData(settings, data, reply, null);
                        data.Position = pos;
                        if (found)
                        {
                            break;
                        }
                    }
                    else if ((msg.InterfaceType == InterfaceType.WRAPPER ||
                        msg.InterfaceType == InterfaceType.PrimeDcWrapper) &&
                        data.Available > 1 && data.GetUInt16(data.Position) == 0x1)
                    {
                        pos = data.Position;
                        found = GXDLMS.GetData(settings, data, reply, null);
                        if (found)
                        {
                            //If client and server address are used as a filter.
                            if ((clientAddress == 0 || clientAddress == reply.SourceAddress || clientAddress == reply.TargetAddress) &&
                                (serverAddress == 0 || serverAddress == reply.TargetAddress || serverAddress == reply.SourceAddress))
                            {
                                data.Position = pos;
                                break;
                            }
                        }
                        else
                        {
                            data.Position = pos;
                        }
                    }
                    else if (msg.InterfaceType == InterfaceType.Plc &&
                        data.GetUInt8(data.Position) == 0x2)
                    {
                        pos = data.Position;
                        found = GXDLMS.GetData(settings, data, reply, null);
                        data.Position = pos;
                        if (found)
                        {
                            break;
                        }
                    }
                    else if (msg.InterfaceType == InterfaceType.PlcHdlc &&
                        GXDLMS.GetPlcSfskFrameSize(data) != 0)
                    {
                        pos = data.Position;
                        found = GXDLMS.GetData(settings, data, reply, null);
                        data.Position = pos;
                        if (found)
                        {
                            break;
                        }
                    }
                    else if (msg.InterfaceType == InterfaceType.WirelessMBus &&
                        GXDLMS.IsWirelessMBusData(data))
                    {
                        pos = data.Position;
                        found = GXDLMS.GetData(settings, data, reply, null);
                        data.Position = pos;
                        if (found)
                        {
                            break;
                        }
                    }
                    else if (msg.InterfaceType == InterfaceType.WiredMBus &&
                        GXDLMS.IsWiredMBusData(data))
                    {
                        pos = data.Position;
                        found = GXDLMS.GetData(settings, data, reply, null);
                        data.Position = pos;
                        if (found)
                        {
                            break;
                        }
                    }
                    else if (msg.InterfaceType == InterfaceType.SMS && data.Available > 2)
                    {
                        pos = data.Position;
                        found = GXDLMS.GetData(settings, data, reply, null);
                        if (found)
                        {
                            //If client and server address are used as a filter.
                            if ((clientAddress == 0 || clientAddress == reply.SourceAddress || clientAddress == reply.TargetAddress) &&
                                (serverAddress == 0 || serverAddress == reply.TargetAddress || serverAddress == reply.SourceAddress))
                            {
                                data.Position = pos;
                                break;
                            }
                        }
                        else
                        {
                            data.Position = pos;
                        }
                    }
                    ++data.Position;
                }
            }
            catch (Exception)
            {
                data.Position = original;
                throw new Exception(string.Format("Invalid {0} frame.", msg.InterfaceType));
            }
            msg.MoreData = reply.MoreData;
            msg.SourceAddress = reply.SourceAddress;
            msg.TargetAddress = reply.TargetAddress;
            if (pdu != null)
            {
                pdu.Clear();
                pdu.Set(reply.Data.Data, 0, reply.Data.Size);
            }
            bool r = data.Position != data.Size;
            if (!found)
            {
                data.Position = original;
            }
            return r;
        }

        /// <summary>
        /// Find next frame from the string.
        /// </summary>
        /// <remarks>
        /// Position of data is set to the begin of new frame. If Pdu is null it is not updated.
        /// </remarks>
        /// <param name="data">Data where frame is search.</param>
        /// <param name="pdu">Pdu of received frame is set here.</param>
        /// <returns>Is new frame found.</returns>
        public bool FindNextFrame(GXByteBuffer data, GXByteBuffer pdu)
        {
            GXDLMSSettings settings = new GXDLMSSettings(true, InterfaceType.HDLC);
            GXReplyData reply = new GXReplyData();
            reply.Xml = new GXDLMSTranslatorStructure(OutputType, OmitXmlNameSpace, Hex, ShowStringAsHex, Comments, null);
            int pos;
            bool found;
            while (data.Position != data.Size)
            {
                if (data.GetUInt8(data.Position) == 0x7e)
                {
                    pos = data.Position;
                    settings.InterfaceType = InterfaceType.HDLC;
                    found = GXDLMS.GetData(settings, data, reply, null);
                    data.Position = pos;
                    if (found)
                    {
                        break;
                    }
                }
                else if (data.Position + 2 < data.Size && data.GetUInt16(data.Position) == 0x1)
                {
                    pos = data.Position;
                    settings.InterfaceType = InterfaceType.WRAPPER;
                    found = GXDLMS.GetData(settings, data, reply, null);
                    data.Position = pos;
                    if (found)
                    {
                        break;
                    }
                }
                //STX for IEC 61334-4-32 LLC sublayer.
                else if (data.GetUInt8(data.Position) == 0x2)
                {
                    pos = data.Position;
                    settings.InterfaceType = InterfaceType.Plc;
                    found = GXDLMS.GetData(settings, data, reply, null);
                    data.Position = pos;
                    if (found)
                    {
                        break;
                    }
                }
                //If Mac HDLC Frame.
                else if (data.Position + 2 < data.Size &&
                    (data.GetUInt16(data.Position) == (int)PlcMacSubframes.One || data.GetUInt16(data.Position) == (int)PlcMacSubframes.Two || data.GetUInt16(data.Position) == (int)PlcMacSubframes.Three))
                {
                    pos = data.Position;
                    settings.InterfaceType = InterfaceType.PlcHdlc;
                    found = GXDLMS.GetData(settings, data, reply, null);
                    data.Position = pos;
                    if (found)
                    {
                        break;
                    }
                }
                ++data.Position;
            }
            if (pdu != null)
            {
                pdu.Clear();
                pdu.Set(reply.Data.Data, 0, reply.Data.Size);
            }
            return data.Position != data.Size;
        }

        internal static void AddTag(SortedList<int, string> list, Enum value, string text)
        {
            list.Add(Convert.ToInt32(value), text);
        }

        /// <summary>
        /// Get all tags.
        /// </summary>
        /// <param name="type">Output type.</param>
        /// <param name="list">List of tags by ID.</param>
        /// <param name="tagsByName">List of tags by name.</param>
        private static void GetTags(TranslatorOutputType type,
                                    SortedList<int, string> list, SortedList<string, int> tagsByName)
        {
            if (type == TranslatorOutputType.SimpleXml)
            {
                TranslatorSimpleTags.GetGeneralTags(type, list);
                TranslatorSimpleTags.GetSnTags(type, list);
                TranslatorSimpleTags.GetLnTags(type, list);
                TranslatorSimpleTags.GetGloTags(type, list);
                TranslatorSimpleTags.GetDedTags(type, list);
                TranslatorSimpleTags.GetTranslatorTags(type, list);
                TranslatorSimpleTags.GetDataTypeTags(list);
                TranslatorSimpleTags.GetPlcTags(list);
            }
            else
            {
                TranslatorStandardTags.GetGeneralTags(type, list);
                TranslatorStandardTags.GetSnTags(type, list);
                TranslatorStandardTags.GetLnTags(type, list);
                TranslatorStandardTags.GetGloTags(type, list);
                TranslatorStandardTags.GetDedTags(type, list);
                TranslatorStandardTags.GetTranslatorTags(type, list);
                TranslatorStandardTags.GetDataTypeTags(list);
                TranslatorStandardTags.GetPlcTags(list);
            }
            // Simple is not case sensitive.
            bool lowercase = type == TranslatorOutputType.SimpleXml;
            foreach (var it in list)
            {
                string str = it.Value;
                if (lowercase)
                {
                    str = str.ToLower();
                }
                if (!tagsByName.ContainsKey(str))
                {
                    tagsByName.Add(str, it.Key);
                }
            }
        }


        /// <summary>
        /// Get PDU from data.
        /// </summary>
        /// <param name="value">Data.</param>
        /// <returns>PDU from the data.</returns>
        public byte[] GetPdu(byte[] value)
        {
            return GetPdu(new GXByteBuffer(value));
        }

        /// <summary>
        /// Get PDU from data.
        /// </summary>
        /// <param name="value">Data.</param>
        /// <returns>PDU from the data.</returns>
        public byte[] GetPdu(GXByteBuffer value)
        {
            GXReplyData data = new GXReplyData();
            data.Xml = new GXDLMSTranslatorStructure(OutputType, OmitXmlNameSpace, Hex, ShowStringAsHex, Comments, tags);
            GXDLMSSettings settings = new GXDLMSSettings(true, InterfaceType.HDLC);
            GetCiphering(settings, true);
            if (value.GetUInt8(0) == 0x7e)
            {
                settings.InterfaceType = Enums.InterfaceType.HDLC;
            }
            //If wrapper.
            else if (value.GetUInt16(0) == 1)
            {
                settings.InterfaceType = Enums.InterfaceType.WRAPPER;
            }
            else
            {
                throw new ArgumentNullException("Invalid DLMS framing.");
            }
            GXDLMS.GetData(settings, value, data, null);
            //Only fully PDUs are returned.
            if (data.IsMoreData)
            {
                return null;
            }
            return data.Data.Array();
        }


        /// <summary>
        ///  Clear MessageToXml internal settings.
        /// </summary>
        public void Clear()
        {
            multipleFrames = false;
            pduFrames.Clear();
            sending = false;
            SSendSequence = SReceiveSequence = RSendSequence = RReceiveSequence = 0;
            SAck = RAck = 0;
        }

        /// <summary>
        /// Convert message to xml.
        /// </summary>
        /// <param name="value">Bytes to convert.</param>
        /// <returns>Converted xml.</returns>
        /// <seealso cref="PduOnly"/>
        /// <seealso cref="CompletePdu"/>
        public string MessageToXml(byte[] value)
        {
            return MessageToXml(new GXByteBuffer(value));
        }

        private void GetCiphering(GXDLMSSettings settings, bool force)
        {
            settings.CryptoNotifier = cryptoNotifier;
            if (force || Security != Security.None)
            {
                GXCiphering c = new Secure.GXCiphering(this.SystemTitle);
                c.SecuritySuite = SecuritySuite;
                c.Security = Security;
                c.SystemTitle = SystemTitle;
                c.BlockCipherKey = BlockCipherKey;
                c.AuthenticationKey = AuthenticationKey;
                c.InvocationCounter = InvocationCounter;
                if (DedicatedKey != null && DedicatedKey.Length != 0)
                {
                    c.DedicatedKey = DedicatedKey;
                }
                if (UseGeneralProtection)
                {
                    settings.ProposedConformance |= Conformance.GeneralProtection;
                    settings.NegotiatedConformance |= Conformance.GeneralProtection;
                }
                else
                {
                    settings.ProposedConformance &= ~Conformance.GeneralProtection;
                    settings.NegotiatedConformance &= ~Conformance.GeneralProtection;
                }
                settings.Cipher = c;
                settings.Keys = Keys;
                if (ServerSystemTitle != null && ServerSystemTitle.Length != 0)
                {
                    settings.SourceSystemTitle = ServerSystemTitle;
                }
                settings.Cipher.SigningKeyPair = SigningKeyPair;
                settings.Cipher.KeyAgreementKeyPair = KeyAgreementKeyPair;
                settings.Cipher.TlsKeyPair = TlsKeyPair;
            }
            else
            {
                settings.Cipher = null;
            }
        }
        private void CheckFrame(byte frame, GXDLMSTranslatorStructure xml)
        {
            ///Skip notification frames.
            if (frame == 0x13)
            {
                xml.AppendComment("Notification frame.");
                return;
            }
            sending = !sending;
            if (frame == (byte)Command.Snrm)
            {
                sending = true;
                xml.AppendComment("SNRM frame.");
                SSendSequence = SReceiveSequence = RSendSequence = RReceiveSequence = 0;
                SAck = RAck = 0;
            }
            else if (frame == (byte)Command.Ua)
            {
                sending = false;
                SSendSequence = SReceiveSequence = RSendSequence = RReceiveSequence = 0;
                SAck = RAck = 0;
                xml.AppendComment("UA frame.");
            }
            else if (frame == (byte)Command.DisconnectRequest)
            {
                sending = false;
                xml.AppendComment("Disconnect Request frame.");
            }
            else if (frame == (byte)Command.DisconnectMode)
            {
                sending = false;
                xml.AppendComment("Disconnect mode.");
            }
            //If S -frame.
            else if ((frame & (byte)HdlcFrameType.Sframe) == (byte)HdlcFrameType.Sframe)
            {
                xml.AppendComment("S frame.");
                byte receiveSequence = (byte)((frame >> 5) & 7);
                byte sendSequence = (byte)((frame >> 1) & 7);
                if (!sending && SSendSequence + 1 == receiveSequence)
                {
                    ++RAck;
                }
                else if (sending && ((RSendSequence + 1) % 8) == receiveSequence)
                {
                    ++SAck;
                }
                else if (SSendSequence != 0 && RSendSequence != 0)
                {
                    byte expected = 0;
                    xml.AppendComment("Invalid I Frame: " + frame.ToString("X") + ". Expected: " + expected.ToString("X"));
                }
            }
            //Handle U-frame.
            else if ((frame & 1) == (byte)HdlcFrameType.Uframe)
            {
                xml.AppendComment("U frame.");
            }
            else //I-frame.
            {
                if (frame == 0x10 && SReceiveSequence == 0 && SSendSequence == 0)
                {
                    xml.AppendComment("AARQ frame.");
                }
                else if (frame == 0x30 && RReceiveSequence == 0 && RSendSequence == 0)
                {
                    xml.AppendComment("AARE frame.");
                    RReceiveSequence = 1;
                }
                else
                {
                    byte sendSequence = (byte)((frame >> 1) & 7);
                    byte receiveSequence = (byte)((frame >> 5) & 7);
                    if (sending)
                    {
                        //If send.
                        if ((SSendSequence + 1) % 8 == sendSequence)
                        {
                            if ((SReceiveSequence + 1) % 8 == receiveSequence)
                            {
                                SSendSequence = sendSequence;
                                SReceiveSequence = receiveSequence;
                            }
                            else if (RAck != 0 && SReceiveSequence == receiveSequence)
                            {
                                SSendSequence = sendSequence;
                                SReceiveSequence = receiveSequence;
                            }
                            else if ((SReceiveSequence + 1 + SAck) % 8 == receiveSequence)
                            {
                                SSendSequence = sendSequence;
                                SReceiveSequence = receiveSequence;
                                SAck = 0;
                            }
                        }
                        else if (SSendSequence == 0 && SReceiveSequence == 0 && RSendSequence == 0 && RReceiveSequence == 0)
                        {
                            //If all data is not handled.
                            SSendSequence = sendSequence;
                            SReceiveSequence = receiveSequence;
                        }
                        else
                        {
                            byte expected = (byte)(((SReceiveSequence + 1) << 1 | 1) << 4 | (SSendSequence + 1) << 1);
                            xml.AppendComment("Invalid I Frame: " + frame.ToString("X") + ". Expected: " + expected.ToString("X"));
                        }
                    }
                    else
                    {
                        if ((RReceiveSequence + RAck + 1) % 8 == receiveSequence ||
                            SAck != 0 && RReceiveSequence == receiveSequence)
                        {
                            if ((RSendSequence + 1) % 8 == sendSequence)
                            {
                                RSendSequence = sendSequence;
                                RReceiveSequence = receiveSequence;
                            }
                            else
                            {
                                SAck = 0;
                            }
                            if (RAck != 0)
                            {
                                RAck = 0;
                                RReceiveSequence = (byte)((RReceiveSequence - 1) % 8);
                            }
                        }
                        else if (RSendSequence == 0 && RReceiveSequence == 0)
                        {
                            //If all data is not handled.
                            RSendSequence = sendSequence;
                            RReceiveSequence = receiveSequence;
                        }
                        else
                        {
                            byte expected = (byte)(((SReceiveSequence + 1) << 1 | 1) << 4 | (SSendSequence + 1) << 1);
                            xml.AppendComment("Invalid I Frame: " + frame.ToString("X") + ". Expected: " + expected.ToString("X"));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Convert message to xml.
        /// </summary>
        /// <param name="value">Message to convert.</param>
        /// <returns>Converted xml.</returns>
        /// <seealso cref="PduOnly"/>
        /// <seealso cref="CompletePdu"/>
        /// /// <summary>
        /// Convert message to xml.
        /// </summary>
        /// <param name="value">Bytes to convert.</param>
        /// <returns>Converted xml.</returns>
        /// <seealso cref="PduOnly"/>
        /// <seealso cref="CompletePdu"/>
        public string MessageToXml(GXByteBuffer value)
        {
            GXDLMSTranslatorMessage msg = new GXDLMSTranslatorMessage() { Message = value };
            MessageToXml(msg);
            return msg.Xml;
        }

        /// <summary>
        /// Get logical and physical address from the server address.
        /// </summary>
        /// <param name="value">Server address.</param>
        /// <param name="logical">Logical server address.</param>
        /// <param name="physical">Physical server address.</param>
        public static void GetLogicalAndPhysicalAddress(int value, out int logical, out int physical)
        {
            if (value > 0x3FFF)
            {
                logical = value >> 14;
                physical = value & 0x3FFF;
            }
            else
            {
                logical = value >> 7;
                physical = value & 0x7F;
            }
        }

        private static void UpdateAddress(GXDLMSSettings settings, GXDLMSTranslatorMessage msg)
        {
            bool reply = false;
            switch (msg.Command)
            {
                case Command.ReadRequest:
                case Command.WriteRequest:
                case Command.GetRequest:
                case Command.SetRequest:
                case Command.MethodRequest:
                case Command.Snrm:
                case Command.Aarq:
                case Command.DisconnectRequest:
                case Command.ReleaseRequest:
                case Command.AccessRequest:
                case Command.GloGetRequest:
                case Command.GloSetRequest:
                case Command.GloMethodRequest:
                case Command.GloInitiateRequest:
                case Command.GloReadRequest:
                case Command.GloWriteRequest:
                case Command.DedInitiateRequest:
                case Command.DedReadRequest:
                case Command.DedWriteRequest:
                case Command.DedGetRequest:
                case Command.DedSetRequest:
                case Command.DedMethodRequest:
                case Command.GatewayRequest:
                case Command.DiscoverRequest:
                case Command.RegisterRequest:
                case Command.PingRequest:
                    reply = false;
                    break;
                case Command.ReadResponse:
                case Command.WriteResponse:
                case Command.GetResponse:
                case Command.SetResponse:
                case Command.MethodResponse:
                case Command.DisconnectMode:
                case Command.UnacceptableFrame:
                case Command.Ua:
                case Command.Aare:
                case Command.ReleaseResponse:
                case Command.ConfirmedServiceError:
                case Command.ExceptionResponse:
                //                case Command.GeneralBlockTransfer:
                case Command.AccessResponse:
                case Command.DataNotification:
                case Command.GloGetResponse:
                case Command.GloSetResponse:
                case Command.GloEventNotification:
                case Command.GloMethodResponse:
                case Command.GloInitiateResponse:
                case Command.GloReadResponse:
                case Command.GloWriteResponse:
                case Command.GloConfirmedServiceError:
                case Command.GloInformationReport:
                //                case Command.GeneralGloCiphering:
                //              case Command.GeneralDedCiphering:
                //                case Command.GeneralCiphering:
                //              case Command.GeneralSigning:
                case Command.InformationReport:
                case Command.EventNotification:
                case Command.DedInitiateResponse:
                case Command.DedReadResponse:
                case Command.DedWriteResponse:
                case Command.DedConfirmedServiceError:
                case Command.DedUnconfirmedWriteRequest:
                case Command.DedInformationReport:
                case Command.DedGetResponse:
                case Command.DedSetResponse:
                case Command.DedEventNotification:
                case Command.DedMethodResponse:
                case Command.GatewayResponse:
                case Command.DiscoverReport:
                case Command.PingResponse:
                    reply = true;
                    break;
            }
            if (reply)
            {
                msg.TargetAddress = settings.ClientAddress;
                msg.SourceAddress = settings.ServerAddress;
            }
            else
            {
                msg.SourceAddress = settings.ClientAddress;
                msg.TargetAddress = settings.ServerAddress;
            }
        }

        /// <summary>
        /// Convert message to xml.
        /// </summary>
        /// <param name="msg">Translator message data.</param>
        /// <returns>Converted xml.</returns>
        /// <seealso cref="PduOnly"/>
        /// <seealso cref="CompletePdu"/>
        /// /// <summary>
        /// Convert message to xml.
        /// </summary>
        /// <seealso cref="PduOnly"/>
        /// <seealso cref="CompletePdu"/>
        public void MessageToXml(GXDLMSTranslatorMessage msg)
        {
            if (msg.Message == null || msg.Message.Size == 0)
            {
                throw new ArgumentNullException("Message is empty.");
            }
            msg.Exception = null;
            GXReplyData data = new GXReplyData();
            GXDLMSTranslatorStructure xml = new GXDLMSTranslatorStructure(OutputType, OmitXmlNameSpace, Hex, ShowStringAsHex, Comments, tags);
            GXDLMSSettings settings = new GXDLMSSettings(true, msg.InterfaceType);
            GetCiphering(settings, true);
            data.Xml = xml;
            try
            {
                //If HDLC framing.
                int offset = msg.Message.Position;
                if ((msg.InterfaceType == InterfaceType.HDLC || msg.InterfaceType == InterfaceType.HdlcWithModeE) && msg.Message.GetUInt8(msg.Message.Position) == 0x7e)
                {
                    msg.InterfaceType = settings.InterfaceType = InterfaceType.HDLC;
                    if (GXDLMS.GetData(settings, msg.Message, data, null))
                    {
                        msg.MoreData = data.MoreData;
                        msg.SourceAddress = data.SourceAddress;
                        msg.TargetAddress = data.TargetAddress;
                        //settings.SourceSystemTitle
                        if (!PduOnly)
                        {
                            int logical, physical;
                            xml.AppendLine("<HDLC len=\"" + (data.PacketLength - offset).ToString("X") + "\" >");
                            GetLogicalAndPhysicalAddress(settings.ServerAddress, out logical, out physical);
                            if (logical != 0)
                            {
                                xml.AppendComment("Logical address:" + logical.ToString() + ", Physical address:" + physical.ToString());
                            }
                            xml.AppendLine("<TargetAddress Value=\"" + settings.ServerAddress.ToString("X") + "\" />");
                            GetLogicalAndPhysicalAddress(settings.ClientAddress, out logical, out physical);
                            if (logical != 0)
                            {
                                xml.AppendComment("Logical address:" + logical.ToString() + ", Physical address:" + physical.ToString());
                            }
                            xml.AppendLine("<SourceAddress Value=\"" + settings.ClientAddress.ToString("X") + "\" />");
                            //Check frame.
                            CheckFrame(data.FrameId, xml);
                            xml.AppendLine("<FrameType Value=\"" + data.FrameId.ToString("X") + "\" />");

                        }
                        if (data.Data.Size == 0)
                        {
                            if ((data.FrameId & 1) != 0 && data.Command == Command.None)
                            {
                                if (!CompletePdu)
                                {
                                    xml.AppendLine("<Command Value=\"NextFrame\" />");
                                }
                                multipleFrames = true;
                            }
                            else
                            {
                                msg.Command = data.Command;
                                xml.AppendStartTag(data.Command);
                                xml.AppendEndTag(data.Command);
                            }
                        }
                        else
                        {
                            if (multipleFrames || (data.MoreData & Enums.RequestTypes.Frame) != 0)
                            {
                                if (CompletePdu)
                                {
                                    pduFrames.Set(data.Data.Data);
                                    if (data.MoreData == RequestTypes.None)
                                    {
                                        xml.AppendLine(PduToXml(pduFrames, true, true, msg));
                                        pduFrames.Clear();
                                    }
                                }
                                else
                                {
                                    xml.AppendLine("<NextFrame Value=\"" + GXCommon.ToHex(data.Data.Data, false, data.Data.Position, data.Data.Size - data.Data.Position) + "\" />");
                                }
                                if (data.MoreData != RequestTypes.DataBlock)
                                {
                                    multipleFrames = false;
                                }
                            }
                            else
                            {
                                if (!PduOnly)
                                {
                                    xml.AppendLine("<PDU>");
                                }
                                if (pduFrames.Size != 0)
                                {
                                    pduFrames.Set(data.Data.Data);
                                    xml.AppendLine(PduToXml(pduFrames, true, true, msg));
                                    pduFrames.Clear();
                                }
                                else
                                {
                                    if (data.Command == Command.Snrm || data.Command == Command.Ua)
                                    {
                                        xml.AppendStartTag(data.Command);
                                        PduToXml(xml, data.Data, true, true, true, msg);
                                        xml.AppendEndTag(data.Command);
                                        xml.sb.Length += 2;
                                    }
                                    else
                                    {
                                        if (CompletePdu)
                                        {
                                            pduFrames.Set(data.Data.Data);
                                            if (data.MoreData == RequestTypes.None)
                                            {
                                                xml.AppendLine(PduToXml(pduFrames, true, true, msg));
                                                pduFrames.Clear();
                                            }
                                        }
                                        else
                                        {
                                            xml.AppendLine(PduToXml(data.Data, OmitXmlDeclaration, OmitXmlNameSpace, msg));
                                        }
                                    }
                                }
                                // Remove \r\n.
                                xml.sb.Length -= Environment.NewLine.Length;
                                if (!PduOnly)
                                {
                                    xml.AppendLine("</PDU>");
                                }
                            }
                        }
                        if (!PduOnly)
                        {
                            xml.AppendLine("</HDLC>");
                        }
                    }
                    UpdateAddress(settings, msg);
                    msg.Xml = xml.sb.ToString();
                    return;
                }
                //If wrapper.
                else if ((msg.InterfaceType == InterfaceType.HDLC ||
                    msg.InterfaceType == InterfaceType.WRAPPER ||
                    msg.InterfaceType == InterfaceType.PrimeDcWrapper) &&
                    msg.Message.Available > 1 && msg.Message.GetUInt16(msg.Message.Position) == 1)
                {
                    if (msg.InterfaceType == InterfaceType.PrimeDcWrapper)
                    {
                        settings.InterfaceType = msg.InterfaceType;
                    }
                    else
                    {
                        msg.InterfaceType = settings.InterfaceType = InterfaceType.WRAPPER;
                    }
                    GXDLMS.GetData(settings, msg.Message, data, null);
                    msg.MoreData = data.MoreData;
                    msg.SourceAddress = data.SourceAddress;
                    msg.TargetAddress = data.TargetAddress;
                    string pdu = PduToXml(data.Data, OmitXmlDeclaration, OmitXmlNameSpace, msg);
                    if (!PduOnly)
                    {
                        xml.AppendLine("<WRAPPER len=\"" + xml.IntegerToHex(data.Data.Size, 0, Hex) + "\" >");
                        xml.AppendLine("<SourceAddress Value=\"" + settings.ClientAddress.ToString("X") + "\" />");
                        xml.AppendLine("<TargetAddress Value=\"" + settings.ServerAddress.ToString("X") + "\" />");
                    }
                    if (!PduOnly)
                    {
                        xml.AppendLine("<PDU>");
                    }
                    xml.AppendLine(pdu);
                    //Remove \r\n.
                    xml.sb.Length -= Environment.NewLine.Length;
                    if (!PduOnly)
                    {
                        xml.AppendLine("</PDU>");
                    }
                    if (!PduOnly)
                    {
                        xml.AppendLine("</WRAPPER>");
                    }
                    UpdateAddress(settings, msg);
                    msg.Xml = xml.sb.ToString();
                    return;
                }
                //If PLC.
                else if ((msg.InterfaceType == InterfaceType.HDLC || msg.InterfaceType == InterfaceType.Plc) && msg.Message.GetUInt8(msg.Message.Position) == 2)
                {
                    msg.InterfaceType = settings.InterfaceType = InterfaceType.Plc;
                    GXDLMS.GetData(settings, msg.Message, data, null);
                    msg.MoreData = data.MoreData;
                    msg.SourceAddress = data.SourceAddress;
                    msg.TargetAddress = data.TargetAddress;
                    if (!PduOnly)
                    {
                        xml.AppendLine("<Plc len=\"" + (data.PacketLength - offset).ToString("X") + "\" >");
                        if (Comments)
                        {
                            if (data.TargetAddress == (UInt16)PlcSourceAddress.Initiator)
                            {
                                xml.AppendComment("Initiator");
                            }
                            else if (data.TargetAddress == (UInt16)PlcSourceAddress.New)
                            {
                                xml.AppendComment("New");
                            }
                        }
                        xml.AppendLine("<SourceAddress Value=\"" + data.TargetAddress.ToString("X") + "\" />");
                        if (Comments && data.SourceAddress == (UInt16)PlcDestinationAddress.AllPhysical)
                        {
                            xml.AppendComment("AllPhysical");
                        }
                        xml.AppendLine("<DestinationAddress Value=\"" + data.SourceAddress.ToString("X") + "\" />");
                    }
                    if (data.Data.Size == 0)
                    {
                        xml.AppendLine("<Command Value=\"" + data.Command.ToString().ToUpper() + "\" />");
                    }
                    else
                    {
                        if (!PduOnly)
                        {
                            xml.AppendLine("<PDU>");
                        }
                        xml.AppendLine(PduToXml(data.Data, OmitXmlDeclaration, OmitXmlNameSpace, msg));
                        //Remove \r\n.
                        xml.sb.Length -= Environment.NewLine.Length;
                        if (!PduOnly)
                        {
                            xml.AppendLine("</PDU>");
                        }
                    }
                    if (!PduOnly)
                    {
                        xml.AppendLine("</Plc>");
                    }
                    UpdateAddress(settings, msg);
                    msg.Xml = xml.sb.ToString();
                    return;
                }
                //If PLC.
                else if ((msg.InterfaceType == InterfaceType.HDLC || msg.InterfaceType == InterfaceType.PlcHdlc) && GXDLMS.GetPlcSfskFrameSize(msg.Message) != 0)
                {
                    msg.InterfaceType = settings.InterfaceType = InterfaceType.PlcHdlc;
                    GXDLMS.GetData(settings, msg.Message, data, null);
                    msg.MoreData = data.MoreData;
                    msg.SourceAddress = data.SourceAddress;
                    msg.TargetAddress = data.TargetAddress;
                    if (!PduOnly)
                    {
                        xml.AppendLine("<PlcSFsk len=\"" + (data.PacketLength - offset).ToString("X") + "\" >");
                        if (Comments)
                        {
                            if (data.TargetAddress == (UInt16)PlcSourceAddress.Initiator)
                            {
                                xml.AppendComment("Initiator");
                            }
                            else if (data.TargetAddress == (UInt16)PlcSourceAddress.New)
                            {
                                xml.AppendComment("New");
                            }
                        }
                        xml.AppendLine("<SourceAddress Value=\"" + data.TargetAddress.ToString("X") + "\" />");
                        if (Comments && data.SourceAddress == (UInt16)PlcDestinationAddress.AllPhysical)
                        {
                            xml.AppendComment("AllPhysical");
                        }
                        xml.AppendLine("<DestinationAddress Value=\"" + data.SourceAddress.ToString("X") + "\" />");
                    }
                    if (data.Data.Size == 0)
                    {
                        xml.AppendLine("<Command Value=\"" + data.Command.ToString().ToUpper() + "\" />");
                    }
                    else
                    {
                        if (!PduOnly)
                        {
                            xml.AppendLine("<PDU>");
                        }
                        xml.AppendLine(PduToXml(data.Data, OmitXmlDeclaration, OmitXmlNameSpace, msg));
                        //Remove \r\n.
                        xml.sb.Length -= Environment.NewLine.Length;
                        if (!PduOnly)
                        {
                            xml.AppendLine("</PDU>");
                        }
                    }
                    if (!PduOnly)
                    {
                        xml.AppendLine("</PlcSFsk>");
                    }
                    UpdateAddress(settings, msg);
                    msg.Xml = xml.sb.ToString();
                    return;
                }
                //If Wired M-Bus.
                else if ((msg.InterfaceType == InterfaceType.HDLC || msg.InterfaceType == InterfaceType.WiredMBus) && GXDLMS.IsWiredMBusData(msg.Message))
                {
                    msg.InterfaceType = settings.InterfaceType = InterfaceType.WiredMBus;
                    int len = xml.GetXmlLength();
                    GXDLMS.GetData(settings, msg.Message, data, null);
                    msg.MoreData = data.MoreData;
                    msg.SourceAddress = data.SourceAddress;
                    msg.TargetAddress = data.TargetAddress;
                    string tmp = xml.ToString().Substring(len);
                    xml.SetXmlLength(len);
                    if (!PduOnly)
                    {
                        xml.AppendLine("<WiredMBus len=\"" + (data.PacketLength - offset).ToString("X") + "\" >");
                        xml.AppendLine("<TargetAddress Value=\"" + settings.ServerAddress.ToString("X") + "\" />");
                        xml.AppendLine("<SourceAddress Value=\"" + settings.ClientAddress.ToString("X") + "\" />");
                        xml.Append(tmp);
                    }
                    if (data.Data.Size == 0)
                    {
                        xml.AppendLine("<Command Value=\"" + data.Command.ToString().ToUpper() + "\" />");
                    }
                    else
                    {
                        if (multipleFrames || (data.MoreData & Enums.RequestTypes.Frame) != 0)
                        {
                            if (CompletePdu)
                            {
                                pduFrames.Set(data.Data.Data);
                                if (data.MoreData == RequestTypes.None)
                                {
                                    xml.AppendLine(PduToXml(pduFrames, true, true, msg));
                                    pduFrames.Clear();
                                }
                            }
                            else
                            {
                                xml.AppendLine("<NextFrame Value=\"" + GXCommon.ToHex(data.Data.Data, false, data.Data.Position, data.Data.Size - data.Data.Position) + "\" />");
                            }
                            if ((data.MoreData & RequestTypes.Frame) != 0)
                            {
                                multipleFrames = true;
                            }
                            if (data.MoreData == RequestTypes.DataBlock)
                            {
                                multipleFrames = false;
                            }
                        }
                        else
                        {
                            if (!PduOnly)
                            {
                                xml.AppendLine("<PDU>");
                            }
                            if (pduFrames.Size != 0)
                            {
                                pduFrames.Set(data.Data);
                                data.Data.Clear();
                                data.Data.Set(pduFrames);
                            }
                            xml.AppendLine(PduToXml(data.Data, OmitXmlDeclaration, OmitXmlNameSpace, msg));
                            //Remove \r\n.
                            xml.sb.Length -= Environment.NewLine.Length;
                            if (!PduOnly)
                            {
                                xml.AppendLine("</PDU>");
                            }
                        }
                    }
                    if (!PduOnly)
                    {
                        xml.AppendLine("</WiredMBus>");
                    }
                    UpdateAddress(settings, msg);
                    msg.Xml = xml.sb.ToString();
                    return;
                }
                //If Wireless M-Bus.
                else if ((msg.InterfaceType == InterfaceType.HDLC || msg.InterfaceType == InterfaceType.WirelessMBus) && GXDLMS.IsWirelessMBusData(msg.Message))
                {
                    msg.InterfaceType = settings.InterfaceType = InterfaceType.WirelessMBus;
                    int len = xml.GetXmlLength();
                    GXDLMS.GetData(settings, msg.Message, data, null);
                    msg.MoreData = data.MoreData;
                    msg.SourceAddress = data.SourceAddress;
                    msg.TargetAddress = data.TargetAddress;
                    string tmp = xml.ToString().Substring(len);
                    xml.SetXmlLength(len);
                    if (!PduOnly)
                    {
                        xml.AppendLine("<WirelessMBus len=\"" + (data.PacketLength - offset).ToString("X") + "\" >");
                        xml.AppendLine("<TargetAddress Value=\"" + settings.ServerAddress.ToString("X") + "\" />");
                        xml.AppendLine("<SourceAddress Value=\"" + settings.ClientAddress.ToString("X") + "\" />");
                        xml.Append(tmp);
                    }
                    if (data.Data.Size == 0)
                    {
                        xml.AppendLine("<Command Value=\"" + data.Command.ToString().ToUpper() + "\" />");
                    }
                    else
                    {
                        if (!PduOnly)
                        {
                            xml.AppendLine("<PDU>");
                        }
                        xml.AppendLine(PduToXml(data.Data, OmitXmlDeclaration, OmitXmlNameSpace, msg));
                        //Remove \r\n.
                        xml.sb.Length -= Environment.NewLine.Length;
                        if (!PduOnly)
                        {
                            xml.AppendLine("</PDU>");
                        }
                    }
                    if (!PduOnly)
                    {
                        xml.AppendLine("</WirelessMBus>");
                    }
                    UpdateAddress(settings, msg);
                    msg.Xml = xml.sb.ToString();
                    return;
                }
                //If SMS.
                else if (msg.InterfaceType == InterfaceType.SMS && msg.Message.Available > 2)
                {
                    msg.InterfaceType = settings.InterfaceType = InterfaceType.SMS;
                    GXDLMS.GetData(settings, msg.Message, data, null);
                    msg.MoreData = data.MoreData;
                    msg.SourceAddress = data.SourceAddress;
                    msg.TargetAddress = data.TargetAddress;
                    string pdu = PduToXml(data.Data, OmitXmlDeclaration, OmitXmlNameSpace, msg);
                    if (!PduOnly)
                    {
                        xml.AppendLine("<SMS len=\"" + xml.IntegerToHex(data.Data.Size, 0, Hex) + "\" >");
                        xml.AppendLine("<Destination AP Value=\"" + settings.ClientAddress.ToString("X") + "\" />");
                        xml.AppendLine("<Source AP Value=\"" + settings.ServerAddress.ToString("X") + "\" />");
                    }
                    if (!PduOnly)
                    {
                        xml.AppendLine("<PDU>");
                    }
                    xml.AppendLine(pdu);
                    //Remove \r\n.
                    xml.sb.Length -= Environment.NewLine.Length;
                    if (!PduOnly)
                    {
                        xml.AppendLine("</PDU>");
                    }
                    if (!PduOnly)
                    {
                        xml.AppendLine("</SMS>");
                    }
                    UpdateAddress(settings, msg);
                    msg.Xml = xml.sb.ToString();
                    return;
                }
            }
            catch (Exception ex)
            {
                msg.Exception = ex;
                xml.sb.AppendLine(ex.Message);
                msg.Xml = xml.sb.ToString();
                return;
            }
            throw new ArgumentNullException("Invalid DLMS framing.");
        }

        /// <summary>
        /// Convert hex string to xml.
        /// </summary>
        /// <param name="hex">Converted hex string.</param>
        /// <returns>Converted xml.</returns>
        public string PduToXml(string hex)
        {
            return PduToXml(HexToBytes(hex));
        }

        /// <summary>
        /// Convert bytes to xml.
        /// </summary>
        /// <param name="value">Bytes to convert.</param>
        /// <returns>Converted xml.</returns>
        public string PduToXml(byte[] value)
        {
            return PduToXml(new GXByteBuffer(value));
        }

        private void GetUa(GXByteBuffer data, GXDLMSTranslatorStructure xml)
        {
            data.GetUInt8(); // Skip FromatID
            data.GetUInt8(); // Skip Group ID.
            data.GetUInt8(); // Skip Group len
            Object val;
            while (data.Position < data.Size)
            {
                HDLCInfo id = (HDLCInfo)data.GetUInt8();
                short len = data.GetUInt8();
                switch (len)
                {
                    case 1:
                        val = data.GetUInt8();
                        break;
                    case 2:
                        val = data.GetUInt16();
                        break;
                    case 4:
                        val = data.GetUInt32();
                        break;
                    default:
                        throw new GXDLMSException("Invalid Exception.");
                }
                switch (id)
                {
                    case HDLCInfo.MaxInfoTX:
                        xml.AppendLine("<MaxInfoTX Value=\"" + val.ToString() + "\" />");
                        break;
                    case HDLCInfo.MaxInfoRX:
                        xml.AppendLine("<MaxInfoRX Value=\"" + val.ToString() + "\" />");
                        break;
                    case HDLCInfo.WindowSizeTX:
                        xml.AppendLine("<WindowSizeTX Value=\"" + val.ToString() + "\" />");
                        break;
                    case HDLCInfo.WindowSizeRX:
                        xml.AppendLine("<WindowSizeRX Value=\"" + val.ToString() + "\" />");
                        break;
                    default:
                        throw new GXDLMSException("Invalid UA response.");
                }
            }
        }

        /// <summary>
        /// Convert bytes to xml.
        /// </summary>
        /// <param name="value">Bytes to convert.</param>
        /// <param name="interfaceType">Interface type.</param>
        /// <returns>Converted xml.</returns>
        internal string PduToXml(GXByteBuffer value, InterfaceType interfaceType)
        {
            GXDLMSTranslatorMessage msg = new GXDLMSTranslatorMessage();
            msg.InterfaceType = interfaceType;
            return PduToXml(value, OmitXmlDeclaration, OmitXmlNameSpace, msg);
        }


        /// <summary>
        /// Convert bytes to xml.
        /// </summary>
        /// <param name="value">Bytes to convert.</param>
        /// <returns>Converted xml.</returns>
        public string PduToXml(GXByteBuffer value)
        {
            return PduToXml(value, OmitXmlDeclaration, OmitXmlNameSpace, null);
        }

        private string PduToXml(GXByteBuffer value, bool omitDeclaration, bool omitNameSpace, GXDLMSTranslatorMessage arg)
        {
            GXDLMSTranslatorStructure xml = new GXDLMSTranslatorStructure(OutputType, OmitXmlNameSpace, Hex, ShowStringAsHex, Comments, tags);
            return PduToXml(xml, value, omitDeclaration, omitNameSpace, true, arg);
        }

        private void HandleDiscoverRequest(GXReplyData data)
        {
            byte responseProbability = data.Data.GetUInt8();
            byte allowedTimeSlots = data.Data.GetUInt8();
            byte discoverReportInitialCredit = data.Data.GetUInt8();
            byte icEqualCredit = data.Data.GetUInt8();
            data.Xml.AppendLine("<ResponseProbability Value=\"" + responseProbability.ToString() + "\" />");
            data.Xml.AppendLine("<AllowedTimeSlots Value=\"" + allowedTimeSlots.ToString() + "\" />");
            data.Xml.AppendLine("<DiscoverReportInitialCredit Value=\"" + discoverReportInitialCredit.ToString() + "\" />");
            data.Xml.AppendLine("<ICEqualCredit Value=\"" + icEqualCredit.ToString() + "\" />");
        }

        private void HandleRegisterRequest(InterfaceType interfaceType, GXReplyData data)
        {
            //Get System title.
            byte[] st;
            if (interfaceType == Enums.InterfaceType.PlcHdlc)
            {
                st = new byte[8];
            }
            else
            {
                st = new byte[6];
            }
            data.Data.Get(st);
            data.Xml.AppendLine("<ActiveInitiator Value=\"" + GXCommon.ToHex(st, true) + "\" />");
            byte count = data.Data.GetUInt8();
            for (int pos = 0; pos != count; ++pos)
            {
                //Get System title.
                data.Data.Get(st);
                data.Xml.AppendLine("<SystemTitle Value=\"" + GXCommon.ToHex(st, true) + "\" />");
                //MAC address.
                UInt16 address = data.Data.GetUInt16();
                data.Xml.AppendLine("<MacAddress Value=\"" + address.ToString() + "\" />");
            }
        }

        private void HandleDiscoverResponse(GXDLMSSettings settings, GXReplyData data)
        {
            byte count = data.Data.GetUInt8();
            for (int pos = 0; pos != count; ++pos)
            {
                //Get System title.
                byte[] st;
                if (settings.InterfaceType == InterfaceType.Plc)
                {
                    st = new byte[6];
                }
                else
                {
                    st = new byte[8];
                }
                data.Data.Get(st);
                data.Xml.AppendLine("<SystemTitle Value=\"" + GXCommon.ToHex(st, true) + "\" />");
            }
            // Alarm-Descriptor presence flag
            if (data.Data.GetUInt8() != 0)
            {
                //Alarm-Descriptor
                byte alarmDescriptor = data.Data.GetUInt8();
                data.Xml.AppendLine("<AlarmDescriptor Value=\"" + alarmDescriptor.ToString() + "\" />");
            }
        }

        internal string PduToXml(GXDLMSTranslatorStructure xml,
            GXByteBuffer value,
            bool omitDeclaration,
            bool omitNameSpace,
            bool allowUnknownCommand,
            GXDLMSTranslatorMessage msg)
        {
            if (msg != null && msg.InterfaceType == InterfaceType.PrimeDcWrapper)
            {
                if (GXPrimeDcHandlers.HandleNotification(value, null, xml))
                {
                    return xml.ToString();
                }
            }
            GXDLMSSettings settings = new GXDLMSSettings(true, InterfaceType.HDLC);
            try
            {
                GXReplyData data = new GXReplyData();
                byte cmd = value.GetUInt8();
                if (msg != null)
                {
                    msg.Command = (Command)cmd;
                }
                GetCiphering(settings, GXDLMS.IsCiphered(cmd));
                if (settings != null && settings.Cipher != null)
                {
                    ((GXCiphering)settings.Cipher).TestMode = true;
                }
                settings.Standard = Standard;
                string str;
                int len;
                byte[] tmp;
                switch (cmd)
                {
                    case (byte)Command.Aarq:
                        value.Position = 0;
                        settings.SourceSystemTitle = null;
                        GXAPDU.ParsePDU(settings, settings.Cipher, value, xml);
                        //Update new dedicated key.
                        if (settings.Cipher != null && settings.Cipher.DedicatedKey != null)
                        {
                            DedicatedKey = settings.Cipher.DedicatedKey;
                        }
                        if (msg != null)
                        {
                            msg.SystemTitle = settings.SourceSystemTitle;
                            msg.DedicatedKey = settings.Cipher.DedicatedKey;
                        }
                        break;
                    case (byte)Command.InitiateRequest:
                        value.Position = 0;
                        settings = new GXDLMSSettings(true, InterfaceType.HDLC);
                        GXAPDU.ParseInitiate(true, settings, settings.Cipher, value,
                                xml);
                        break;
                    case (byte)Command.InitiateResponse:
                        value.Position = 0;
                        settings = new GXDLMSSettings(false, InterfaceType.HDLC);
                        GetCiphering(settings, true);
                        GXAPDU.ParseInitiate(true, settings, settings.Cipher, value,
                                xml);
                        break;
                    case 0x81://Ua
                        if (msg != null)
                        {
                            msg.Command = Command.Ua;
                        }
                        value.Position = 0;
                        GetUa(value, xml);
                        break;
                    case (byte)Command.Aare:
                        value.Position = 0;
                        settings = new GXDLMSSettings(false, InterfaceType.HDLC);
                        GetCiphering(settings, true);
                        settings.SourceSystemTitle = null;
                        GXAPDU.ParsePDU(settings, settings.Cipher, value, xml);
                        if (msg != null)
                        {
                            msg.SystemTitle = settings.SourceSystemTitle;
                        }
                        break;
                    case (byte)Command.GetRequest:
                        GXDLMSLNCommandHandler.HandleGetRequest(settings, null, value, null, xml, Command.None);
                        break;
                    case (byte)Command.SetRequest:
                        GXDLMSLNCommandHandler.HandleSetRequest(settings, null, value, null, xml, Command.None);
                        break;
                    case (byte)Command.ReadRequest:
                        GXDLMSSNCommandHandler.HandleReadRequest(settings, null, value, null, xml, Command.None);
                        break;
                    case (byte)Command.MethodRequest:
                        GXDLMSLNCommandHandler.HandleMethodRequest(settings, null, value, null, null, xml, Command.None);
                        break;
                    case (byte)Command.WriteRequest:
                        GXDLMSSNCommandHandler.HandleWriteRequest(settings, null, value, null, xml, Command.None);
                        break;
                    case (byte)Command.AccessRequest:
                        GXDLMSLNCommandHandler.HandleAccessRequest(settings, null, value, null, xml, Command.None);
                        break;
                    case (byte)Command.DataNotification:
                        data.Xml = xml;
                        data.Data = value;
                        value.Position = 0;
                        GXDLMS.GetPdu(settings, data);
                        break;
                    case (byte)Command.InformationReport:
                        data.Xml = xml;
                        data.Data = value;
                        GXDLMSSNCommandHandler.HandleInformationReport(settings, data, null);
                        break;
                    case (byte)Command.EventNotification:
                        data.Xml = xml;
                        data.Data = value;
                        GXDLMSLNCommandHandler.HandleEventNotification(settings, data, null);
                        break;
                    case (byte)Command.ReadResponse:
                    case (byte)Command.WriteResponse:
                    case (byte)Command.GetResponse:
                    case (byte)Command.SetResponse:
                    case (byte)Command.MethodResponse:
                    case (byte)Command.AccessResponse:
                    case (byte)Command.GeneralBlockTransfer:
                        data.Xml = xml;
                        data.Data = value;
                        value.Position = 0;
                        GXDLMS.GetPdu(settings, data);
                        break;
                    case (byte)Command.GeneralCiphering:
                        data.Xml = xml;
                        data.Data = value;
                        value.Position = 0;
                        GXDLMS.GetPdu(settings, data);
                        break;
                    case (byte)Command.ReleaseRequest:
                        xml.AppendStartTag((Command)cmd);
                        value.GetUInt8();
                        // Len.
                        if (value.Available != 0)
                        {
                            // BerType
                            value.GetUInt8();
                            // Len.
                            value.GetUInt8();
                            if (xml.OutputType == TranslatorOutputType.SimpleXml)
                            {
                                str = TranslatorSimpleTags.ReleaseRequestReasonToString((ReleaseRequestReason)value.GetUInt8());
                            }
                            else
                            {
                                str = TranslatorStandardTags.ReleaseRequestReasonToString((ReleaseRequestReason)value.GetUInt8());
                            }
                            xml.AppendLine(TranslatorTags.Reason, null, str);
                            if (value.Available != 0)
                            {
                                int len2 = xml.GetXmlLength();
                                try
                                {
                                    xml.AppendStartTag(TranslatorGeneralTags.UserInformation);
                                    GXAPDU.ParsePDU2(settings, settings.Cipher, value,
                                            xml);
                                    xml.AppendEndTag(TranslatorGeneralTags.UserInformation);
                                }
                                catch (Exception)
                                {
                                    //This usually fails if keys are wrong.
                                    xml.SetXmlLength(len2);
                                }
                            }
                        }
                        xml.AppendEndTag(cmd);
                        break;
                    case (byte)Command.ReleaseResponse:
                        xml.AppendStartTag((Command)cmd);
                        if (value.GetUInt8() != 0)
                        {
                            //BerType
                            value.GetUInt8();
                            //Len.
                            value.GetUInt8();
                            if (xml.OutputType == TranslatorOutputType.SimpleXml)
                            {
                                str = TranslatorSimpleTags.ReleaseResponseReasonToString((ReleaseResponseReason)value.GetUInt8());
                            }
                            else
                            {
                                str = TranslatorStandardTags.ReleaseResponseReasonToString((ReleaseResponseReason)value.GetUInt8());
                            }

                            xml.AppendLine(TranslatorTags.Reason, "Value", str);
                            if (value.Available != 0)
                            {
                                GXAPDU.ParsePDU2(settings, settings.Cipher, value, xml);
                            }
                        }
                        xml.AppendEndTag((Command)cmd);
                        break;
                    case (byte)Command.GloReadRequest:
                    case (byte)Command.GloWriteRequest:
                    case (byte)Command.GloGetRequest:
                    case (byte)Command.GloSetRequest:
                    case (byte)Command.GloReadResponse:
                    case (byte)Command.GloWriteResponse:
                    case (byte)Command.GloGetResponse:
                    case (byte)Command.GloSetResponse:
                    case (byte)Command.GloMethodRequest:
                    case (byte)Command.GloMethodResponse:
                    case (byte)Command.DedGetRequest:
                    case (byte)Command.DedSetRequest:
                    case (byte)Command.DedReadResponse:
                    case (byte)Command.DedWriteResponse:
                    case (byte)Command.DedGetResponse:
                    case (byte)Command.DedSetResponse:
                    case (byte)Command.DedMethodRequest:
                    case (byte)Command.DedMethodResponse:
                    case (byte)Command.GloConfirmedServiceError:
                    case (byte)Command.DedConfirmedServiceError:
                        if (settings.Cipher != null && Comments)
                        {
                            int originalPosition = value.Position;
                            int len2 = xml.GetXmlLength();
                            try
                            {
                                --value.Position;
                                Command c = (Command)cmd;
                                byte[] st;
                                if (c == Command.GloReadRequest || c == Command.GloWriteRequest || c == Command.GloGetRequest || c == Command.GloSetRequest || c == Command.GloMethodRequest ||
                                    c == Command.DedGetRequest || c == Command.DedSetRequest || c == Command.DedMethodRequest)
                                {
                                    st = settings.Cipher.SystemTitle;
                                }
                                else
                                {
                                    st = settings.SourceSystemTitle;
                                }
                                if (st != null)
                                {
                                    AesGcmParameter p;
                                    if ((cmd == (byte)Command.DedGetRequest || cmd == (byte)Command.DedSetRequest ||
                                        cmd == (byte)Command.DedReadResponse || cmd == (byte)Command.DedWriteResponse ||
                                        cmd == (byte)Command.DedGetResponse || cmd == (byte)Command.DedSetResponse ||
                                        cmd == (byte)Command.DedMethodRequest || cmd == (byte)Command.DedMethodResponse) &&
                                        settings.Cipher.DedicatedKey != null && settings.Cipher.DedicatedKey.Length != 0)
                                    {
                                        p = new AesGcmParameter(settings, st, settings.Cipher.DedicatedKey, settings.Cipher.AuthenticationKey);
                                    }
                                    else
                                    {
                                        p = new AesGcmParameter(settings, st, settings.Cipher.BlockCipherKey, settings.Cipher.AuthenticationKey);
                                    }
                                    p.Xml = xml;
                                    if (p.BlockCipherKey != null)
                                    {
                                        GXByteBuffer data2 = new GXByteBuffer(GXDLMSChippering.DecryptAesGcm(p, value));
                                        xml.AppendComment("Invocation Counter: " + p.InvocationCounter.ToString());
                                        xml.StartComment("Decrypt data: " + data2.ToString());
                                        PduToXml(xml, data2, omitDeclaration, omitNameSpace, false, msg);
                                        xml.EndComment();
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                // It's OK if this fails. Ciphering settings are not correct.
                                if (msg != null)
                                {
                                    msg.Command = (Command)cmd;
                                }
                                xml.SetXmlLength(len2);
                                value.Position = originalPosition;
                                break;
                            }
                            value.Position = originalPosition;
                        }

                        int cnt = GXCommon.GetObjectCount(value);
                        if (cnt != value.Size - value.Position)
                        {
                            xml.AppendComment("Invalid length: " + cnt + ". It should be: " + (value.Size - value.Position));
                        }
                        xml.AppendLine(cmd, "Value", GXCommon.ToHex(value.Data, false, value.Position, value.Size - value.Position));
                        break;
                    case (byte)Command.GeneralGloCiphering:
                    case (byte)Command.GeneralDedCiphering:
                        {
                            AesGcmParameter p = null;
                            if (settings.Cipher != null && Comments)
                            {
                                int len2 = xml.GetXmlLength();
                                int originalPosition = value.Position;
                                --value.Position;
                                //Check is this client msg.
                                try
                                {
                                    byte[] st;
                                    st = settings.Cipher.SystemTitle;
                                    if (cmd == (byte)Command.GeneralDedCiphering && settings.Cipher.DedicatedKey != null)
                                    {
                                        p = new AesGcmParameter(settings, st, settings.Cipher.DedicatedKey, settings.Cipher.AuthenticationKey);
                                    }
                                    else
                                    {
                                        p = new AesGcmParameter(settings, st, settings.Cipher.BlockCipherKey, settings.Cipher.AuthenticationKey);
                                    }
                                    p.Xml = xml;
                                    GXByteBuffer data2 = new GXByteBuffer(GXDLMSChippering.DecryptAesGcm(p, value));
                                    len2 = xml.GetXmlLength();
                                    xml.AppendComment("Invocation Counter: " + p.InvocationCounter.ToString());
                                    xml.StartComment("Decrypt data: " + data2.ToString());
                                    PduToXml(xml, data2, omitDeclaration, omitNameSpace, false, msg);
                                    xml.EndComment();
                                }
                                catch (Exception)
                                {
                                    xml.SetXmlLength(len2);
                                    value.Position = originalPosition;
                                    --value.Position;
                                    //Check is this server msg.
                                    try
                                    {
                                        byte[] st;
                                        st = settings.SourceSystemTitle;
                                        if (st != null)
                                        {
                                            p = new AesGcmParameter(settings, st, settings.Cipher.BlockCipherKey, settings.Cipher.AuthenticationKey);
                                            p.Xml = xml;
                                            GXByteBuffer data2 = new GXByteBuffer(GXDLMSChippering.DecryptAesGcm(p, value));
                                            xml.AppendComment("Invocation Counter: " + p.InvocationCounter.ToString());
                                            xml.StartComment("Decrypt data: " + data2.ToString());
                                            PduToXml(xml, data2, omitDeclaration, omitNameSpace, false, msg);
                                            xml.EndComment();
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        // It's OK if this fails. Ciphering settings are not correct.
                                        if (msg != null)
                                        {
                                            msg.Command = (Command)cmd;
                                        }
                                        xml.SetXmlLength(len2);
                                    }
                                }
                                value.Position = originalPosition;
                            }
                            len = GXCommon.GetObjectCount(value);
                            tmp = new byte[len];
                            value.Get(tmp);
                            xml.AppendStartTag((Command)cmd);
                            xml.AppendLine(TranslatorTags.SystemTitle, null,
                                    GXCommon.ToHex(tmp, false, 0, len));
                            len = GXCommon.GetObjectCount(value);
                            tmp = new byte[len];
                            value.Get(tmp);
                            xml.AppendLine(TranslatorTags.CipheredService, null,
                                    GXCommon.ToHex(tmp, false, 0, len));
                            xml.AppendEndTag(cmd);
                        }
                        break;
                    case (byte)Command.GeneralSigning:
                        {
                            xml.AppendStartTag((Command)cmd);
                            AesGcmParameter p = null;
                            int originalPosition = value.Position;
                            if (settings.Cipher != null && Comments)
                            {
                                int len2 = xml.GetXmlLength();
                                --value.Position;
                                try
                                {
                                    byte[] st;
                                    st = settings.Cipher.SystemTitle;
                                    p = new AesGcmParameter(settings, st, settings.Cipher.BlockCipherKey, settings.Cipher.AuthenticationKey);
                                    p.Xml = xml;
                                    GXByteBuffer data2 = new GXByteBuffer(GXDLMSChippering.DecryptAesGcm(p, value));
                                    xml.AppendComment("Security : " + p.Security);
                                    xml.AppendComment("Security Suite: " + p.SecuritySuite);
                                    xml.AppendComment("Invocation Counter: " + p.InvocationCounter.ToString());
                                    xml.StartComment("Decrypt data: " + data2.ToString());
                                    PduToXml(xml, data2, omitDeclaration, omitNameSpace, false, msg);
                                    xml.EndComment();
                                    p.Xml.AppendLine(TranslatorTags.Content, null, GXCommon.ToHex(p.CipheredContent, false));
                                    p.Xml.AppendLine(TranslatorTags.Signature, null, GXCommon.ToHex(p.Signature, false));
                                }
                                catch (Exception)
                                {
                                    // It's OK if this fails. Ciphering settings are not correct.
                                    if (msg != null)
                                    {
                                        msg.Command = (Command)cmd;
                                    }
                                    xml.SetXmlLength(len2);
                                    value.Position = originalPosition;
                                }
                            }
                            if (value.Position == originalPosition)
                            {
                                GXByteBuffer transactionId = new GXByteBuffer();
                                len = GXCommon.GetObjectCount(value);
                                GXCommon.SetObjectCount(len, transactionId);
                                transactionId.Set(value, len);
                                xml.AppendLine(TranslatorTags.TransactionId, null, xml.IntegerToHex(transactionId.GetUInt64(1), 16, true));
                                len = GXCommon.GetObjectCount(value);
                                tmp = new byte[len];
                                value.Get(tmp);
                                xml.AppendLine(TranslatorTags.OriginatorSystemTitle, null, GXCommon.ToHex(tmp, false));
                                len = GXCommon.GetObjectCount(value);
                                tmp = new byte[len];
                                value.Get(tmp);
                                xml.AppendLine(TranslatorTags.RecipientSystemTitle, null, GXCommon.ToHex(tmp, false));
                                // Get date time.
                                len = GXCommon.GetObjectCount(value);
                                tmp = new byte[len];
                                value.Get(tmp);
                                xml.AppendLine(TranslatorTags.DateTime, null, GXCommon.ToHex(tmp, false));
                                // other-information
                                len = value.GetUInt8();
                                tmp = new byte[len];
                                value.Get(tmp);
                                xml.AppendLine(TranslatorTags.OtherInformation, null, GXCommon.ToHex(tmp, false));
                                len = GXCommon.GetObjectCount(value);
                                tmp = new byte[len];
                                value.Get(tmp);
                                xml.AppendLine(TranslatorTags.Content, null, GXCommon.ToHex(tmp, false));
                                len = GXCommon.GetObjectCount(value);
                                tmp = new byte[len];
                                value.Get(tmp);
                                xml.AppendLine(TranslatorTags.Signature, null, GXCommon.ToHex(tmp, false));
                            }
                            value.Position = originalPosition;
                            xml.AppendEndTag(cmd);
                        }
                        break;
                    case (byte)Command.ConfirmedServiceError:
                        data.Xml = xml;
                        data.Data = value;
                        GXDLMS.HandleConfirmedServiceError(data);
                        break;
                    case (byte)Command.GatewayRequest:
                    case (byte)Command.GatewayResponse:
                        data.Xml = xml;
                        data.Data = value;
                        //Get Network ID.
                        byte id = value.GetUInt8();
                        //Get Physical device address.
                        len = GXCommon.GetObjectCount(value);
                        tmp = new byte[len];
                        value.Get(tmp);
                        xml.AppendStartTag((Command)cmd);
                        xml.AppendLine(TranslatorTags.NetworkId, null, id.ToString());
                        xml.AppendLine(TranslatorTags.PhysicalDeviceAddress, null, GXCommon.ToHex(tmp, false, 0, len));
                        PduToXml(xml, new GXByteBuffer(value.Remaining()), omitDeclaration, omitNameSpace, allowUnknownCommand, msg);
                        xml.AppendEndTag(cmd);
                        break;
                    case (byte)Command.ExceptionResponse:
                        data.Xml = xml;
                        data.Data = value;
                        GXDLMS.HandleExceptionResponse(data);
                        break;
                    case (byte)Command.DiscoverRequest:
                        data.Xml = xml;
                        data.Data = value;
                        xml.AppendStartTag((Command)cmd);
                        HandleDiscoverRequest(data);
                        xml.AppendEndTag((Command)cmd);
                        break;
                    case (byte)Command.DiscoverReport:
                        data.Xml = xml;
                        data.Data = value;
                        xml.AppendStartTag((Command)cmd);
                        HandleDiscoverResponse(settings, data);
                        xml.AppendEndTag((Command)cmd);
                        break;
                    case (byte)Command.RegisterRequest:
                        data.Xml = xml;
                        data.Data = value;
                        xml.AppendStartTag((Command)cmd);
                        HandleRegisterRequest(msg.InterfaceType, data);
                        xml.AppendEndTag((Command)cmd);
                        break;
                    case (byte)Command.PingResponse:
                        data.Xml = xml;
                        data.Data = value;
                        xml.AppendStartTag((Command)cmd);
                        GXDLMS.HandleExceptionResponse(data);
                        xml.AppendEndTag((Command)cmd);
                        break;
                    case (byte)Command.RepeatCallRequest:
                        data.Xml = xml;
                        data.Data = value;
                        xml.AppendStartTag((Command)cmd);
                        GXDLMS.HandleExceptionResponse(data);
                        xml.AppendEndTag((Command)cmd);
                        break;
                    default:
                        if (!allowUnknownCommand)
                        {
                            throw new Exception("Invalid command.");
                        }
                        --value.Position;
                        xml.AppendLine("<Data=\"" + GXCommon.ToHex(value.Data, false, value.Position, value.Size - value.Position) + "\" />");
                        break;
                }
                if (OutputType == TranslatorOutputType.StandardXml)
                {
                    StringBuilder sb = new StringBuilder();
                    if (!omitDeclaration)
                    {
                        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    }
                    if (!omitNameSpace)
                    {
                        if (cmd != (byte)Command.Aare && cmd != (byte)Command.Aarq)
                        {
                            sb.AppendLine(
                                "<x:xDLMS-APDU xmlns:x=\"http://www.dlms.com/COSEMpdu\">");
                        }
                        else
                        {
                            sb.AppendLine(
                                "<x:aCSE-APDU xmlns:x=\"http://www.dlms.com/COSEMpdu\">");
                        }
                    }
                    sb.Append(xml.ToString());
                    if (!omitNameSpace)
                    {
                        if (cmd != (byte)Command.Aare && cmd != (byte)Command.Aarq)
                        {
                            sb.AppendLine("</x:xDLMS-APDU>");
                        }
                        else
                        {
                            sb.AppendLine("</x:aCSE-APDU>");
                        }
                    }
                    return sb.ToString();
                }
                return xml.ToString();
            }
            finally
            {
                if (settings != null && settings.Cipher != null)
                {
                    ((GXCiphering)settings.Cipher).TestMode = false;
                }
            }
        }

        private static void ReadAllNodes(XmlDocument doc, GXDLMSXmlSettings s)
        {
            if (doc != null)
            {
                foreach (XmlNode node in doc.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        ReadNode(node, s);
                    }
                }
            }
        }

        /// <summary>
        /// Get command from XML.
        /// </summary>
        /// <param name="node">XML node.</param>
        /// <param name="s">XML settings.</param>
        /// <param name="tag">tag.</param>
        private static void GetCommand(XmlNode node, GXDLMSXmlSettings s, int tag)
        {
            s.command = (Command)tag;
            byte[] tmp;
            switch (tag)
            {
                case (byte)Command.Snrm:
                case (byte)Command.Aarq:
                case (byte)Command.GetRequest:
                case (byte)Command.SetRequest:
                case (byte)Command.ReadRequest:
                case (byte)Command.WriteRequest:
                case (byte)Command.MethodRequest:
                case (byte)Command.ReleaseRequest:
                case (int)Command.AccessRequest:
                case (int)Command.InitiateRequest:
                case (int)Command.ConfirmedServiceError:
                case (int)Command.ExceptionResponse:
                    s.settings.IsServer = false;
                    break;
                case (byte)Command.GloInitiateRequest:
                case (byte)Command.GloGetRequest:
                case (byte)Command.GloSetRequest:
                case (byte)Command.GloMethodRequest:
                case (byte)Command.GloReadRequest:
                case (byte)Command.GloWriteRequest:
                case (byte)Command.DedGetRequest:
                case (byte)Command.DedSetRequest:
                case (byte)Command.DedMethodRequest:
                case (byte)Command.DedInitiateRequest:
                    if (s.settings.Cipher == null)
                    {
                        throw new Exception("Security level is None.");
                    }
                    s.settings.IsServer = false;
                    tmp = GXCommon.HexToBytes(GetValue(node, s));
                    s.settings.Cipher.Security = (Security)tmp[0];
                    s.data.Set(tmp);
                    break;
                case (byte)Command.Ua:
                case (byte)Command.Aare:
                case (byte)Command.GetResponse:
                case (byte)Command.SetResponse:
                case (byte)Command.ReadResponse:
                case (byte)Command.WriteResponse:
                case (byte)Command.MethodResponse:
                case (byte)Command.ReleaseResponse:
                case (int)Command.DataNotification:
                case (int)Command.AccessResponse:
                case (int)Command.InitiateResponse:
                case (int)Command.InformationReport:
                case (int)Command.EventNotification:
                case (int)Command.DisconnectRequest:
                case (int)Command.GeneralBlockTransfer:
                    break;
                case (byte)Command.GloInitiateResponse:
                case (byte)Command.GloGetResponse:
                case (byte)Command.GloSetResponse:
                case (byte)Command.GloMethodResponse:
                case (byte)Command.GloReadResponse:
                case (byte)Command.GloWriteResponse:
                case (byte)Command.GloEventNotification:
                case (byte)Command.DedInitiateResponse:
                case (byte)Command.DedGetResponse:
                case (byte)Command.DedSetResponse:
                case (byte)Command.DedMethodResponse:
                case (byte)Command.DedEventNotification:
                    tmp = GXCommon.HexToBytes(GetValue(node, s));
                    if (s.settings.Cipher == null)
                    {
                        throw new Exception("Security level is None.");
                    }
                    s.settings.Cipher.Security = (Security)tmp[0];
                    s.data.Set(tmp);
                    break;
                case (byte)Command.GeneralGloCiphering:
                    break;
                case (int)TranslatorTags.FrameType:
                    s.command = 0;
                    break;
                case (byte)Command.GatewayRequest:
                    s.gwCommand = Command.GatewayRequest;
                    s.settings.IsServer = false;
                    break;
                case (byte)Command.GatewayResponse:
                    s.gwCommand = Command.GatewayResponse;
                    break;
                default:
                    throw new ArgumentException("Invalid Command: " + node.Name);
            }
        }

        /// <summary>
        /// Handle AARE and AARQ XML tags.
        /// </summary>
        /// <param name="node">XML node.</param>
        /// <param name="s">XML Settings.</param>
        /// <param name="tag">XML tag.</param>
        private static bool HandleAarqAare(XmlNode node, GXDLMSXmlSettings s, int tag)
        {
            string str;
            byte[] tmp;
            Conformance c;
            int value;
            switch (tag)
            {
                case (int)TranslatorGeneralTags.ApplicationContextName:
                    if (s.OutputType == TranslatorOutputType.StandardXml)
                    {
                        value = int.Parse(node.InnerText);
                        switch (value)
                        {
                            case 1:
                                s.settings.UseLogicalNameReferencing = true;
                                break;
                            case 2:
                                s.settings.UseLogicalNameReferencing = false;
                                break;
                            case 3:
                                s.settings.UseLogicalNameReferencing = true;
                                break;
                            case 4:
                                s.settings.UseLogicalNameReferencing = false;
                                break;
                            default:
                                throw new ArgumentException("Invalid dedicated key.");
                        }
                    }
                    else
                    {
                        str = node.Attributes[0].InnerText;
                        if (string.Compare(str, "SN") == 0 ||
                                string.Compare(str, "SN_WITH_CIPHERING") == 0)
                        {
                            s.settings.UseLogicalNameReferencing = false;
                        }
                        else if (string.Compare(str, "LN") == 0 ||
                                 string.Compare(str, "LN_WITH_CIPHERING") == 0)
                        {
                            s.settings.UseLogicalNameReferencing = true;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid Reference type name.");
                        }
                    }
                    break;
                case (byte)Command.GloInitiateRequest:
                    s.settings.IsServer = false;
                    tmp = GXCommon.HexToBytes(GetValue(node, s));
                    s.settings.Cipher.Security = (Security)tmp[0];
                    s.data.Set(tmp);
                    break;
                case (byte)Command.GloInitiateResponse:
                    tmp = GXCommon.HexToBytes(GetValue(node, s));
                    s.settings.Cipher.Security = (Security)tmp[0];
                    s.data.Set(tmp);
                    break;
                case (byte)Command.InitiateRequest:
                case (byte)Command.InitiateResponse:
                    break;
                case (byte)TranslatorGeneralTags.UserInformation:
                    if (s.OutputType == TranslatorOutputType.StandardXml)
                    {
                        GXByteBuffer bb = new GXByteBuffer();
                        tmp = GXCommon.HexToBytes(GetValue(node, s));
                        GXCommon.SetObjectCount(tmp.Length, bb);
                        bb.Set(tmp);
                        if (s.settings.IsServer)
                        {
                            s.settings.ProposedConformance = (Conformance)0xFFFFFF;
                        }
                        GXAPDU.ParseUserInformation(s.settings,
                                                    s.settings.Cipher, bb, null);

                        // Update proposed conformance or XML to PDU will fail.
                        if (!s.settings.IsServer)
                        {
                            s.settings.ProposedConformance = s.settings.NegotiatedConformance;
                        }
                    }
                    break;
                case 0xBE00:
                    //NegotiatedQualityOfService
                    s.settings.QualityOfService = (byte)s.ParseInt(GetValue(node, s));
                    break;
                case 0xBE06:
                case 0xBE01:
                    //NegotiatedDlmsVersionNumber or ProposedDlmsVersionNumber is skipped.
                    s.settings.DLMSVersion = byte.Parse(GetValue(node, s));
                    break;
                case 0xBE04:
                    //VaaName is not needed.
                    break;
                case 0x8A:
                    //SenderACSERequirements is not needed.
                    break;
                case 0x8B:
                case 0x89:
                    //MechanismName.
                    s.settings.Authentication = (Authentication)Enum.Parse(typeof(Authentication), GetValue(node, s));
                    if (s.OutputType == TranslatorOutputType.SimpleXml)
                    {
                        s.settings.Authentication = (Authentication)Enum.Parse(typeof(Authentication), GetValue(node, s));
                    }
                    else
                    {
                        s.settings.Authentication = (Authentication)int.Parse(GetValue(node, s));
                    }
                    break;
                case 0xAC:
                    //CallingAuthentication.
                    if (s.settings.Authentication == Authentication.Low)
                    {
                        s.settings.Password = GXCommon.HexToBytes(GetValue(node, s));
                    }
                    else
                    {
                        s.settings.CtoSChallenge = GXCommon.HexToBytes(GetValue(node, s));
                    }
                    break;
                case (int)TranslatorGeneralTags.DedicatedKey:
                    tmp = GXCommon.HexToBytes(GetValue(node, s));
                    s.settings.Cipher.DedicatedKey = tmp;
                    break;
                case (int)TranslatorGeneralTags.CallingAPTitle:
                    s.settings
                    .CtoSChallenge = GXCommon.HexToBytes(GetValue(node, s));
                    break;
                case (int)TranslatorGeneralTags.CallingAeInvocationId:
                    s.settings.UserId = s.ParseInt(GetValue(node, s));
                    break;
                case (int)TranslatorGeneralTags.CalledAeInvocationId:
                    s.settings.UserId = s.ParseInt(GetValue(node, s));
                    s.attributeDescriptor.SetUInt8(0xA5);
                    s.attributeDescriptor.SetUInt8(03);
                    s.attributeDescriptor.SetUInt8(BerType.Integer);
                    s.attributeDescriptor.SetUInt8(1);
                    s.attributeDescriptor.SetUInt8((byte)s.ParseInt(GetValue(node, s)));
                    break;
                case (int)TranslatorGeneralTags.RespondingAeInvocationId:
                    s.settings.UserId = s.ParseInt(GetValue(node, s));
                    break;
                case 0xA4:
                    //RespondingAPTitle.
                    s.settings.StoCChallenge = GXCommon.HexToBytes(GetValue(node, s));
                    break;
                case 0xBE03:
                case 0xBE05:
                    //ProposedConformance or NegotiatedConformance
                    if (s.settings.IsServer)
                    {
                        s.settings.NegotiatedConformance = Conformance.None;
                    }
                    else
                    {
                        s.settings.ProposedConformance = Conformance.None;
                    }
                    if (s.OutputType == TranslatorOutputType.StandardXml)
                    {
                        String nodes = node.InnerText;
                        foreach (String it in nodes.Split(' '))
                        {
                            if (it.Trim() != string.Empty)
                            {
                                c = TranslatorStandardTags.ValueOfConformance(it.Trim());
                                if (s.settings.IsServer)
                                {
                                    s.settings.NegotiatedConformance |= c;
                                }
                                else
                                {
                                    s.settings.ProposedConformance |= c;
                                }
                            }
                        }
                    }
                    break;
                case 0xBE08:
                    //ConformanceBit.
                    c = (Conformance)Enum.Parse(typeof(Conformance), node.Attributes["Name"].InnerText);
                    if (s.settings.IsServer)
                    {
                        s.settings.NegotiatedConformance |= c;
                    }
                    else
                    {
                        s.settings.ProposedConformance |= c;
                    }
                    break;
                case 0xA2:
                    //AssociationResult
                    s.result = (AssociationResult)Enum.Parse(typeof(AssociationResult), GetValue(node, s));
                    break;
                case 0xBE02:
                case 0xBE07:
                    //NegotiatedMaxPduSize or ProposedMaxPduSize.
                    s.settings.maxReceivePDUSize = (UInt16)s.ParseLong(GetValue(node, s));
                    break;
                case 0xA3:
                    //ResultSourceDiagnostic
                    s.diagnostic = SourceDiagnostic.None;
                    break;
                case 0xA301:
                    //ACSEServiceUser
                    s.diagnostic = (SourceDiagnostic)s.ParseInt(GetValue(node, s));
                    break;
                case 0xBE09:
                    // ProposedQualityOfService
                    s.settings.QualityOfService = (byte)s.ParseInt(GetValue(node, s));
                    break;
                case (int)TranslatorGeneralTags.CharString:
                    // Get PW
                    if (s.settings.Authentication == Authentication.Low)
                    {
                        s.settings.Password = GXCommon.HexToBytes(GetValue(node, s));
                    }
                    else
                    {
                        if (s.command == Command.Aarq)
                        {
                            s.settings.CtoSChallenge =
                                GXCommon.HexToBytes(GetValue(node, s));
                        }
                        else
                        {
                            s.settings.StoCChallenge =
                                GXCommon.HexToBytes(GetValue(node, s));
                        }
                    }
                    break;
                case (int)TranslatorGeneralTags.ResponderACSERequirement:
                    break;
                case (int)TranslatorGeneralTags.RespondingAuthentication:
                    s.settings
                    .StoCChallenge = GXCommon.HexToBytes(GetValue(node, s));
                    break;
                case (int)TranslatorTags.Result:
                    s.result = (AssociationResult)int.Parse(GetValue(node, s));
                    break;
                case (int)Command.ConfirmedServiceError:
                    if (s.command == Command.None)
                    {
                        s.settings.IsServer = false;
                        s.command = (Command)tag;
                    }
                    break;
                case (int)TranslatorTags.Reason:
                    if (s.command == Command.ReleaseRequest)
                    {
                        if (s.OutputType == TranslatorOutputType.SimpleXml)
                        {
                            s.reason = (byte)TranslatorSimpleTags.ValueOfReleaseRequestReason(GetValue(node, s));
                        }
                        else
                        {
                            s.reason = (byte)TranslatorStandardTags.ValueOfReleaseRequestReason(GetValue(node, s));
                        }
                    }
                    else
                    {
                        if (s.OutputType == TranslatorOutputType.SimpleXml)
                        {
                            s.reason = (byte)TranslatorSimpleTags.ValueOfReleaseResponseReason(GetValue(node, s));
                        }
                        else
                        {
                            s.reason = (byte)TranslatorStandardTags.ValueOfReleaseResponseReason(GetValue(node, s));
                        }
                    }
                    break;

                case (int)TranslatorTags.Service:
                    s.attributeDescriptor.SetUInt8(0xE);
                    s.attributeDescriptor.SetUInt8((byte)s.ParseInt(GetValue(node, s)));
                    break;
                case (int)TranslatorTags.ServiceError:
                    if (s.command == Command.Aare)
                    {
                        s.attributeDescriptor.SetUInt8(6);
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            if (childNode.NodeType == XmlNodeType.Element)
                            {
                                s.attributeDescriptor.SetUInt8((byte)TranslatorSimpleTags.GetInitiate(GetValue(childNode, s)));
                            }
                        }
                        return false;
                    }
                    break;
                case (UInt16)TranslatorTags.ProtocolVersion:
                    str = GetValue(node, s);
                    GXByteBuffer pv = new GXByteBuffer();
                    pv.SetUInt8((byte)(8 - str.Length));
                    GXCommon.SetBitString(pv, str, false);
                    s.settings.protocolVersion = str;
                    break;
                case (int)TranslatorTags.CalledAPTitle:
                case (int)TranslatorTags.CalledAEQualifier:
                    tmp = GXCommon.HexToBytes(GetValue(node, s));
                    s.attributeDescriptor.SetUInt8((byte)(tag == (int)TranslatorTags.CalledAPTitle ? 0xA2 : 0xA3));
                    s.attributeDescriptor.SetUInt8(03);
                    s.attributeDescriptor.SetUInt8(BerType.OctetString);
                    s.attributeDescriptor.SetUInt8((byte)tmp.Length);
                    s.attributeDescriptor.Set(tmp);
                    break;
                case (int)TranslatorTags.CalledAPInvocationId:
                    s.attributeDescriptor.SetUInt8(0xA4);
                    s.attributeDescriptor.SetUInt8(03);
                    s.attributeDescriptor.SetUInt8(BerType.Integer);
                    s.attributeDescriptor.SetUInt8(1);
                    s.attributeDescriptor.SetUInt8((byte)s.ParseInt(GetValue(node, s)));
                    break;
                case (int)TranslatorTags.CalledAEInvocationId:
                    s.attributeDescriptor.SetUInt8(0xA5);
                    s.attributeDescriptor.SetUInt8(03);
                    s.attributeDescriptor.SetUInt8(BerType.Integer);
                    s.attributeDescriptor.SetUInt8(1);
                    s.attributeDescriptor.SetUInt8((byte)s.ParseInt(GetValue(node, s)));
                    break;
                case (int)TranslatorTags.CallingApInvocationId:
                    s.attributeDescriptor.SetUInt8(0xA8);
                    s.attributeDescriptor.SetUInt8(03);
                    s.attributeDescriptor.SetUInt8(BerType.Integer);
                    s.attributeDescriptor.SetUInt8(1);
                    s.attributeDescriptor.SetUInt8((byte)s.ParseInt(GetValue(node, s)));
                    break;
                default:
                    throw new ArgumentException("Invalid AARQ node: " + node.Name);

            }
            return true;
        }

        private static GXByteBuffer UpdateDataType(XmlNode node, GXDLMSXmlSettings s, int tag)
        {
            GXByteBuffer preData = null;
            string v = GetValue(node, s);
            if (s.template || v == "*")
            {
                s.template = true;
                return preData;
            }
            switch ((DataType)(tag - GXDLMS.DATA_TYPE_OFFSET))
            {
                case DataType.Array:
                    s.data.SetUInt8(DataType.Array);
                    preData = new GXByteBuffer(s.data);
                    s.data.Size = 0;
                    break;
                case DataType.Bcd:
                    GXCommon.SetData(s.settings, s.data, DataType.Bcd, s.ParseShort(GetValue(node, s)));
                    break;
                case DataType.BitString:
                    GXCommon.SetData(s.settings, s.data, DataType.BitString, GetValue(node, s));
                    break;
                case DataType.Boolean:
                    GXCommon.SetData(s.settings, s.data, DataType.Boolean, Boolean.Parse(GetValue(node, s)));
                    break;
                case DataType.Date:
                    GXCommon.SetData(s.settings, s.data, DataType.Date, GXDLMSClient.ChangeType(GXCommon.HexToBytes(GetValue(node, s)), DataType.DateTime, s.settings.UseUtc2NormalTime));
                    break;
                case DataType.DateTime:
                    DataType dt = DataType.DateTime;
                    byte[] tmp = GXCommon.HexToBytes(GetValue(node, s));
                    if (tmp.Length == 5)
                    {
                        dt = DataType.Date;
                    }
                    else if (tmp.Length == 4)
                    {
                        dt = DataType.Time;
                    }
                    GXCommon.SetData(s.settings, s.data, dt, GXDLMSClient.ChangeType(GXCommon.HexToBytes(GetValue(node, s)), dt, s.settings.UseUtc2NormalTime));
                    break;
                case DataType.Enum:
                    GXCommon.SetData(s.settings, s.data, DataType.Enum, s.ParseShort(GetValue(node, s)));
                    break;
                case DataType.Float32:
                    GetFloat32(node, s);
                    break;
                case DataType.Float64:
                    GetFloat64(node, s);
                    break;
                case DataType.Int16:
                    GXCommon.SetData(s.settings, s.data, DataType.Int16, s.ParseShort(GetValue(node, s)));
                    break;
                case DataType.Int32:
                    GXCommon.SetData(s.settings, s.data, DataType.Int32, s.ParseInt(GetValue(node, s)));
                    break;
                case DataType.Int64:
                    GXCommon.SetData(s.settings, s.data, DataType.Int64, s.ParseLong(GetValue(node, s)));
                    break;
                case DataType.Int8:
                    GXCommon.SetData(s.settings, s.data, DataType.Int8, s.ParseShort(GetValue(node, s)));
                    break;
                case DataType.None:
                    GXCommon.SetData(s.settings, s.data, DataType.None, null);
                    break;
                case DataType.OctetString:
                    GetOctetString(node, s);
                    break;
                case DataType.String:
                    if (s.showStringAsHex)
                    {
                        GXCommon.SetData(s.settings, s.data, DataType.String, GXCommon.HexToBytes(GetValue(node, s)));
                    }
                    else
                    {
                        GXCommon.SetData(s.settings, s.data, DataType.String, GetValue(node, s));
                    }
                    break;
                case DataType.StringUTF8:
                    if (s.showStringAsHex)
                    {
                        GXCommon.SetData(s.settings, s.data, DataType.StringUTF8, GXCommon.HexToBytes(GetValue(node, s)));
                    }
                    else
                    {
                        GXCommon.SetData(s.settings, s.data, DataType.StringUTF8, GetValue(node, s));
                    }
                    break;
                case DataType.Structure:
                    s.data.SetUInt8(DataType.Structure);
                    preData = new GXByteBuffer(s.data);
                    s.data.Size = 0;
                    break;
                case DataType.Time:
                    GXCommon.SetData(s.settings, s.data, DataType.Time, GXDLMSClient.ChangeType(GXCommon.HexToBytes(GetValue(node, s)), DataType.DateTime, s.settings.UseUtc2NormalTime));
                    break;
                case DataType.UInt16:
                    GXCommon.SetData(s.settings, s.data, DataType.UInt16, s.ParseInt(GetValue(node, s)));
                    break;
                case DataType.UInt32:
                    GXCommon.SetData(s.settings, s.data, DataType.UInt32, s.ParseLong(GetValue(node, s)));
                    break;
                case DataType.UInt64:
                    GXCommon.SetData(s.settings, s.data, DataType.UInt64, s.ParseULong(GetValue(node, s)));
                    break;
                case DataType.UInt8:
                    GXCommon.SetData(s.settings, s.data, DataType.UInt8, s.ParseShort(GetValue(node, s)));
                    break;
                case DataType.CompactArray:
                    s.data.SetUInt8(DataType.CompactArray);
                    break;
                default:
                    throw new ArgumentException("Invalid node: " + node.Name);
            }
            return preData;
        }

        private static bool GetFrame(XmlNode node, GXDLMSXmlSettings s, int tag)
        {
            bool found = true;
            switch (tag)
            {
                case (int)TranslatorTags.Wrapper:
                    s.settings.InterfaceType = InterfaceType.WRAPPER;
                    break;
                case (int)TranslatorTags.Hdlc:
                    s.settings.InterfaceType = InterfaceType.HDLC;
                    break;
                case (int)TranslatorTags.TargetAddress:
                    s.settings.ServerAddress = int.Parse(GetValue(node, s), System.Globalization.NumberStyles.AllowHexSpecifier);
                    break;
                case (int)TranslatorTags.SourceAddress:
                    s.settings.ClientAddress = int.Parse(GetValue(node, s), System.Globalization.NumberStyles.AllowHexSpecifier);
                    break;
                default:
                    //It's OK if frame is not found.
                    found = false;
                    break;
            }
            return found;
        }

        private static string GetValue(XmlNode node, GXDLMSXmlSettings s)
        {
            if (s.OutputType == TranslatorOutputType.StandardXml)
            {
                if (node.FirstChild == null)
                {
                    return null;
                }
                return node.FirstChild.Value;
            }
            else
            {
                if (node.Attributes.Count == 0)
                {
                    return "";
                }
                return node.Attributes[0].InnerText;
            }
        }

        static ErrorCode ValueOfErrorCode(TranslatorOutputType type,
                                          String value)
        {
            if (type == TranslatorOutputType.StandardXml)
            {
                return TranslatorStandardTags.ValueOfErrorCode(value);
            }
            else
            {
                return TranslatorSimpleTags.ValueOfErrorCode(value);
            }
        }

        internal static String ErrorCodeToString(TranslatorOutputType type,
                ErrorCode value)
        {
            if (type == TranslatorOutputType.StandardXml)
            {
                return TranslatorStandardTags.ErrorCodeToString(value);
            }
            else
            {
                return TranslatorSimpleTags.ErrorCodeToString(value);
            }
        }

        static GXByteBuffer UpdateDateTime(XmlNode node, GXDLMSXmlSettings s, GXByteBuffer preData)
        {
            byte[] tmp;
            if (s.requestType != 0xFF)
            {
                preData = UpdateDataType(node, s,
                                         (int)DataType.DateTime + GXDLMS.DATA_TYPE_OFFSET);
            }
            else
            {
                tmp = GXCommon.HexToBytes(GetValue(node, s));
                if (tmp.Length != 0)
                {
                    DataType dt = DataType.DateTime;
                    if (tmp.Length == 5)
                    {
                        dt = DataType.Date;
                    }
                    else if (tmp.Length == 4)
                    {
                        dt = DataType.Time;
                    }
                    s.time = (GXDateTime)GXDLMSClient.ChangeType(tmp, dt, s.settings.UseUtc2NormalTime);
                }
            }
            return preData;
        }

        private static void ReadNode(
            XmlNode node, GXDLMSXmlSettings s)
        {
            int tag = 0;
            String str;
            if (s.OutputType == TranslatorOutputType.SimpleXml)
            {
                str = node.Name.ToLower();
            }
            else
            {
                if (node.Name.StartsWith("x:"))
                {
                    str = node.Name.Substring(2);
                }
                else
                {
                    str = node.Name;
                }
            }
            if (s.command != Command.ConfirmedServiceError
                    || s.tags.ContainsKey(str))
            {
                tag = s.tags[str];
            }
            ErrorCode err;
            UInt32 value;
            byte[] tmp;
            GXByteBuffer preData = null;
            if (s.command == Command.None)
            {
                if (!((s.settings.ClientAddress == 0 ||
                        s.settings.ServerAddress == 0) &&
                        GetFrame(node, s, tag) ||
                        tag == (int)TranslatorTags.PduDlms ||
                        tag == (int)TranslatorTags.PduCse))
                {
                    GetCommand(node, s, tag);
                }
            }
            else if (s.command == Command.Aarq ||
                     s.command == Command.Aare ||
                     s.command == Command.InitiateRequest ||
                     s.command == Command.InitiateResponse ||
                     s.command == Command.ReleaseRequest ||
                     s.command == Command.ReleaseResponse)
            {
                if (!HandleAarqAare(node, s, tag))
                {
                    return;
                }
            }
            else if (tag >= GXDLMS.DATA_TYPE_OFFSET)
            {
                if (tag == (int)DataType.DateTime + GXDLMS.DATA_TYPE_OFFSET ||
                   (s.command == Command.EventNotification && s.attributeDescriptor.Size == 0))
                {
                    preData = UpdateDateTime(node, s, preData);
                    if (preData == null && s.command == Command.GeneralCiphering)
                    {
                        s.data.SetUInt8(0);
                    }
                }
                else
                {
                    if (s.command == Command.MethodResponse)
                    {
                        //Add Data-Access-Result.
                        s.attributeDescriptor.SetUInt8(0);
                    }
                    preData = UpdateDataType(node, s, tag);
                }
            }
            else if (s.command == Command.ConfirmedServiceError)
            {
                if (s.OutputType == TranslatorOutputType.StandardXml)
                {
                    if (tag == (int)TranslatorTags.InitiateError)
                    {
                        s.attributeDescriptor.SetUInt8(1);
                    }
                    else
                    {
                        ServiceError se = TranslatorStandardTags.GetServiceError(str.Substring(2));
                        s.attributeDescriptor.SetUInt8(se);
                        s.attributeDescriptor.SetUInt8(TranslatorStandardTags.GetError(se, GetValue(node, s)));
                    }
                }
                else
                {
                    if (tag == (int)TranslatorTags.ServiceError)
                    {
                    }
                    else
                    {
                        if (s.attributeDescriptor.Size == 0)
                        {
                            s.attributeDescriptor.SetUInt8((byte)s.ParseShort(GetValue(node, s)));
                        }
                        else
                        {
                            ServiceError se = TranslatorSimpleTags.GetServiceError(str);
                            s.attributeDescriptor.SetUInt8(se);
                            s.attributeDescriptor.SetUInt8(TranslatorSimpleTags.GetError(se, GetValue(node, s)));
                        }
                    }
                }
            }
            else
            {
                switch (tag)
                {
                    case (int)(Command.GetRequest) << 8 | (byte)GetCommandType.Normal:
                    case (int)(Command.GetRequest) << 8 | (byte)GetCommandType.NextDataBlock:
                    case (int)(Command.GetRequest) << 8 | (byte)GetCommandType.WithList:
                    case (int)(Command.SetRequest) << 8 | (byte)SetRequestType.Normal:
                    case (int)(Command.SetRequest) << 8 | (byte)SetRequestType.FirstDataBlock:
                    case (int)(Command.SetRequest) << 8 | (byte)SetRequestType.WithDataBlock:
                    case (int)(Command.SetRequest) << 8 | (byte)SetRequestType.WithList:
                        s.requestType = (byte)(tag & 0xF);
                        break;
                    case (int)(Command.GetResponse) << 8 | (byte)GetCommandType.Normal:
                    case (int)(Command.GetResponse) << 8 | (byte)GetCommandType.NextDataBlock:
                    case (int)(Command.GetResponse) << 8 | (byte)GetCommandType.WithList:
                    case (int)(Command.SetResponse) << 8 | (byte)SetResponseType.Normal:
                    case (int)(Command.SetResponse) << 8 | (byte)SetResponseType.DataBlock:
                    case (int)(Command.SetResponse) << 8 | (byte)SetResponseType.LastDataBlock:
                    case (int)(Command.SetResponse) << 8 | (byte)SetResponseType.WithList:
                    case (int)(Command.SetResponse) << 8 | (byte)SetResponseType.LastDataBlockWithList:
                        s.requestType = (byte)(tag & 0xF);
                        break;

                    case (int)(Command.ReadResponse) << 8 | (byte)SingleReadResponse.DataBlockResult:
                        ++s.count;
                        s.requestType = (byte)(tag & 0xF);
                        break;
                    case (int)(Command.ReadRequest) << 8 | (byte)VariableAccessSpecification.ParameterisedAccess:
                        s.requestType = (byte)VariableAccessSpecification.ParameterisedAccess;
                        break;
                    case (int)(Command.ReadRequest) << 8 | (byte)VariableAccessSpecification.BlockNumberAccess:
                        s.requestType = (byte)VariableAccessSpecification.BlockNumberAccess;
                        ++s.count;
                        break;
                    case (byte)(int)(Command.MethodRequest) << 8 | (byte)ActionRequestType.Normal:
                        s.requestType = (byte)(tag & 0xFF);
                        break;
                    case (byte)(int)(Command.MethodRequest) << 8 | (byte)ActionRequestType.NextBlock:
                        s.requestType = (byte)(tag & 0xFF);
                        break;
                    case (byte)(int)(Command.MethodRequest) << 8 | (byte)ActionRequestType.WithList:
                        s.requestType = (byte)(tag & 0xFF);
                        break;
                    case (byte)(int)(Command.MethodResponse) << 8 | (byte)ActionResponseType.Normal:
                    case (byte)(int)(Command.MethodResponse) << 8 | (byte)ActionResponseType.WithBlock:
                    case (byte)(int)(Command.MethodResponse) << 8 | (byte)ActionResponseType.WithList:
                    case (byte)(int)(Command.MethodResponse) << 8 | (byte)ActionResponseType.NextBlock:
                        //MethodResponseNormal
                        s.requestType = (byte)(tag & 0xFF);
                        break;
                    case (int)(Command.ReadResponse) << 8 | (byte)SingleReadResponse.Data:
                    case (int)TranslatorTags.Data:
                        if (s.command == Command.ReadRequest
                                || s.command == Command.ReadResponse
                                || s.command == Command.GetRequest)
                        {
                            ++s.count;
                            s.requestType = 0;
                        }
                        else if (s.command == Command.GetResponse ||
                                 s.command == Command.MethodResponse)
                        {
                            s.data.SetUInt8(0); // Add status.
                        }
                        break;

                    case (int)TranslatorTags.Success:
                        ++s.count;
                        s.attributeDescriptor.Add((byte)ErrorCode.Ok);
                        break;
                    case (int)TranslatorTags.DataAccessError:
                        ++s.count;
                        s.attributeDescriptor.SetUInt8(1);
                        s.attributeDescriptor.SetUInt8(ValueOfErrorCode(s.OutputType, GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.ListOfVariableAccessSpecification:
                        if (s.command == Command.WriteRequest)
                        {
                            GXCommon.SetObjectCount(node.ChildNodes.Count, s.data);
                        }
                        break;
                    case (int)TranslatorTags.VariableAccessSpecification:
                        break;
                    case (int)TranslatorTags.ListOfData:
                        if (s.command == Command.AccessResponse
                                && s.data.Size == 0)
                        {
                            // If access-request-specification is not given.
                            s.data.SetUInt8(0);
                        }
                        if (s.OutputType == TranslatorOutputType.SimpleXml
                                || s.command != Command.WriteRequest)
                        {
                            GXCommon.SetObjectCount(node.ChildNodes.Count, s.data);
                        }
                        break;
                    case (int)Command.AccessResponse << 8 | (byte)AccessServiceCommandType.Get:
                    case (int)Command.AccessResponse << 8 | (byte)AccessServiceCommandType.Set:
                    case (int)Command.AccessResponse << 8 | (byte)AccessServiceCommandType.Action:
                        s.data.SetUInt8((byte)(0xFF & tag));
                        break;
                    case (int)TranslatorTags.DateTime:
                        preData = UpdateDateTime(node, s, preData);
                        break;
                    case (int)TranslatorTags.CurrentTime:
                        if (s.OutputType == TranslatorOutputType.SimpleXml)
                        {
                            UpdateDateTime(node, s, preData);
                        }
                        else
                        {
                            str = GetValue(node, s);
                            s.time = new GXDateTime(GXCommon.GetGeneralizedTime(str));
                        }
                        break;
                    case (int)TranslatorTags.Time:
                        preData = UpdateDateTime(node, s, preData);
                        break;
                    case (int)TranslatorTags.InvokeId:
                        string id = GetValue(node, s);
                        if (id == "*")
                        {
                            s.settings.Priority = Priority.High;
                            s.settings.ServiceClass = ServiceClass.Confirmed;
                            s.settings.InvokeID = 1;
                        }
                        else
                        {
                            value = (byte)s.ParseShort(id);
                            s.settings.UpdateInvokeId((byte)value);
                        }
                        break;
                    case (int)TranslatorTags.LongInvokeId:
                        value = (uint)s.ParseLong(GetValue(node, s));
                        if ((value & 0x80000000) != 0)
                        {
                            s.settings.Priority = Priority.High;
                        }
                        else
                        {
                            s.settings.Priority = Priority.Normal;
                        }
                        if ((value & 0x40000000) != 0)
                        {
                            s.settings.ServiceClass = ServiceClass.Confirmed;
                        }
                        else
                        {
                            s.settings.ServiceClass = ServiceClass.UnConfirmed;
                        }
                        s.settings.longInvokeID = (UInt16)(value & 0xFFFFFFF);
                        break;
                    case 0x88:
                        //ResponderACSERequirement
                        break;
                    case 0x80:
                        //RespondingAuthentication
                        s.settings.StoCChallenge = GXCommon.HexToBytes(GetValue(node, s));
                        break;
                    case (int)TranslatorTags.AttributeDescriptor:
                        break;
                    case (int)TranslatorTags.ClassId:
                        s.attributeDescriptor.SetUInt16((UInt16)s.ParseInt(GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.InstanceId:
                        s.attributeDescriptor.Add(GXCommon.HexToBytes(GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.AttributeId:
                        s.attributeDescriptor.SetUInt8((byte)s.ParseShort(GetValue(node, s)));
                        //Add AccessSelection.
                        if (s.command != Command.AccessRequest &&
                            s.command != Command.EventNotification)
                        {
                            s.attributeDescriptor.SetUInt8(0);
                        }
                        break;
                    case (int)TranslatorTags.MethodInvocationParameters:
                        s.attributeDescriptor.SetUInt8(s.attributeDescriptor.Size - 1, 1);
                        break;
                    case (int)TranslatorTags.Selector:
                        s.attributeDescriptor.Set(GXCommon.HexToBytes(GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.Parameter:
                        break;
                    case (int)TranslatorTags.LastBlock:
                        s.data.SetUInt8((byte)s.ParseShort(GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.BlockNumber:
                        //BlockNumber
                        if (s.command == Command.GetRequest || s.command == Command.GetResponse ||
                            s.command == Command.SetRequest || s.command == Command.SetResponse ||
                            s.command == Command.MethodRequest || s.command == Command.MethodResponse)
                        {
                            s.data.SetUInt32((UInt32)s.ParseLong(GetValue(node, s)));
                        }
                        else
                        {
                            s.data.SetUInt16((UInt16)s.ParseInt(GetValue(node, s)));
                        }
                        break;
                    case (int)TranslatorTags.RawData:
                        //RawData
                        if (s.command == Command.GetResponse || s.command == Command.MethodResponse)
                        {
                            s.data.SetUInt8(0);
                        }
                        tmp = GXCommon.HexToBytes(GetValue(node, s));
                        GXCommon.SetObjectCount(tmp.Length, s.data);
                        s.data.Set(tmp);
                        break;
                    case (int)TranslatorTags.MethodDescriptor:
                        break;
                    case (int)TranslatorTags.MethodId:
                        s.attributeDescriptor.SetUInt8((byte)s.ParseShort(GetValue(node, s)));
                        //Add MethodInvocationParameters
                        s.attributeDescriptor.SetUInt8(0);
                        break;
                    case (int)TranslatorTags.Result:
                    case (int)TranslatorTags.Pblock:
                    case (int)TranslatorGeneralTags.AssociationResult:
                        //Result.
                        if (s.command == Command.GetRequest || s.requestType == 3)
                        {
                            GXCommon.SetObjectCount(node.ChildNodes.Count, s.attributeDescriptor);
                        }
                        else if (s.command == Command.SetResponse ||
                            s.command == Command.MethodResponse)
                        {
                            if (s.requestType == (byte)SetResponseType.WithList)
                            {
                                GXCommon.SetObjectCount(node.ChildNodes.Count, s.attributeDescriptor);
                            }
                            else
                            {
                                str = GetValue(node, s);
                                if (!string.IsNullOrEmpty(str))
                                {
                                    s.attributeDescriptor.SetUInt8((byte)ValueOfErrorCode(s.OutputType, str));
                                }
                            }
                        }
                        else if (s.command == Command.AccessResponse)
                        {
                            str = GetValue(node, s);
                            if (str != "")
                            {
                                s.data.SetUInt8(ValueOfErrorCode(s.OutputType, str));
                            }
                        }
                        break;
                    case (int)TranslatorTags.ReturnParameters:
                        s.attributeDescriptor.SetUInt8(1);
                        break;
                    case (int)TranslatorTags.AccessSelection:
                        s.attributeDescriptor.SetUInt8(s.attributeDescriptor.Size - 1, 1);
                        break;
                    case (int)TranslatorTags.Value:
                        break;
                    case (int)TranslatorTags.AccessSelector:
                        s.data.SetUInt8((byte)s.ParseShort(GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.AccessParameters:
                        break;
                    case (int)TranslatorTags.AttributeDescriptorList:
                        GXCommon.SetObjectCount(node.ChildNodes.Count, s.attributeDescriptor);
                        break;
                    case (int)TranslatorTags.AttributeDescriptorWithSelection:
                    case (int)Command.AccessRequest << 8 | (byte)AccessServiceCommandType.Get:
                    case (int)Command.AccessRequest << 8 | (byte)AccessServiceCommandType.Set:
                    case (int)Command.AccessRequest << 8 | (byte)AccessServiceCommandType.Action:
                        if (s.command != Command.SetRequest)
                        {
                            s.attributeDescriptor.SetUInt8((byte)(tag & 0xFF));
                        }
                        break;
                    case (int)Command.ReadRequest << 8 | (byte)VariableAccessSpecification.VariableName:
                    case (int)Command.WriteRequest << 8 | (byte)VariableAccessSpecification.VariableName:
                    case (int)Command.WriteRequest << 8 | (byte)SingleReadResponse.Data:
                        if (s.command != Command.AccessRequest
                                && s.command != Command.AccessResponse)
                        {
                            if (!(s.OutputType == TranslatorOutputType.StandardXml
                                    && tag == ((int)Command.WriteRequest << 8
                                               | (int)SingleReadResponse.Data)))
                            {
                                if (s.requestType == 0xFF)
                                {
                                    s.attributeDescriptor.SetUInt8(
                                        VariableAccessSpecification.VariableName);
                                }
                                else
                                {
                                    s.attributeDescriptor.SetUInt8(s.requestType);
                                    s.requestType = 0xFF;
                                }
                                ++s.count;
                            }
                            else if (s.command != Command.InformationReport)
                            {
                                s.attributeDescriptor.SetUInt8((byte)s.count);
                            }
                            if (s.OutputType == TranslatorOutputType.SimpleXml)
                            {
                                s.attributeDescriptor.SetUInt16((UInt16)s.ParseShort(GetValue(node, s)));
                            }
                            else
                            {
                                str = GetValue(node, s);
                                if (!String.IsNullOrEmpty(str))
                                {
                                    s.attributeDescriptor.SetInt16(Int16.Parse(str));
                                }
                            }
                        }
                        break;
                    case (int)TranslatorTags.Choice:
                        break;
                    case (int)Command.ReadResponse << 8
                        | (int)SingleReadResponse.DataAccessError:
                        err =
                            ValueOfErrorCode(s.OutputType, GetValue(node, s));
                        ++s.count;
                        s.data.SetUInt8(1);
                        s.data.SetUInt8(err);
                        break;
                    case (int)TranslatorTags.NotificationBody:
                        break;
                    case (int)TranslatorTags.DataValue:
                        break;
                    case (int)TranslatorTags.AccessRequestBody:
                        break;
                    case (int)TranslatorTags.PduDlms:
                        break;
                    case (int)TranslatorTags.ListOfAccessRequestSpecification:
                        s.attributeDescriptor.SetUInt8((byte)node.ChildNodes.Count);
                        break;
                    case (int)TranslatorTags.AccessRequestSpecification:
                        break;
                    case (int)TranslatorTags.AccessRequestListOfData:
                        s.attributeDescriptor.SetUInt8((byte)node.ChildNodes.Count);
                        break;
                    case (int)TranslatorTags.AccessResponseBody:
                        break;
                    case (int)TranslatorTags.ListOfAccessResponseSpecification:
                        s.data.SetUInt8((byte)node.ChildNodes.Count);
                        break;
                    case (int)TranslatorTags.AccessResponseSpecification:
                        break;
                    case (int)TranslatorTags.AccessResponseListOfData:
                        // Add access-response-list-of-data. Optional
                        s.data.SetUInt8(0);
                        s.data.SetUInt8((byte)node.ChildNodes.Count);
                        break;
                    case (int)TranslatorTags.SingleResponse:
                        break;
                    case (int)TranslatorTags.SystemTitle:
                        tmp = GXCommon.HexToBytes(GetValue(node, s));
                        s.settings.SourceSystemTitle = tmp;
                        break;
                    case (int)TranslatorTags.CipheredService:
                    case (int)Command.GloInitiateRequest:
                        tmp = GXCommon.HexToBytes(GetValue(node, s));
                        if (s.command == Command.GeneralCiphering)
                        {
                            GXCommon.SetObjectCount(tmp.Length, s.data);
                        }
                        else if (s.command == Command.ReleaseRequest)
                        {
                            s.data.SetUInt8(0xBE);
                            GXCommon.SetObjectCount(4 + tmp.Length, s.data);
                            s.data.SetUInt8(4);
                            GXCommon.SetObjectCount(2 + tmp.Length, s.data);
                            s.data.SetUInt8(0x21);
                            GXCommon.SetObjectCount(tmp.Length, s.data);
                        }
                        s.data.Set(tmp);
                        break;
                    case (int)TranslatorTags.DataBlock:
                        break;
                    case (int)TranslatorGeneralTags.UserInformation:
                        tmp = GXCommon.HexToBytes(GetValue(node, s));
                        s.data.SetUInt8(0xBE);
                        s.data.SetUInt8((byte)(2 + tmp.Length));
                        s.data.SetUInt8(0x4);
                        s.data.SetUInt8((byte)tmp.Length);
                        s.data.Set(tmp);
                        break;
                    case (int)TranslatorTags.TransactionId:
                        tmp = GXCommon.HexToBytes(GetValue(node, s));
                        GXCommon.SetObjectCount(tmp.Length, s.data);
                        s.data.Set(tmp);
                        break;
                    case (int)TranslatorTags.OriginatorSystemTitle:
                    case (int)TranslatorTags.RecipientSystemTitle:
                    case (int)TranslatorTags.OtherInformation:
                    case (int)TranslatorTags.KeyCipheredData:
                        tmp = GXCommon.HexToBytes(GetValue(node, s));
                        GXCommon.SetObjectCount(tmp.Length, s.data);
                        s.data.Set(tmp);
                        break;
                    case (int)TranslatorTags.CipheredContent:
                        tmp = GXCommon.HexToBytes(GetValue(node, s));
                        GXCommon.SetObjectCount(tmp.Length, s.data);
                        s.data.Set(tmp);
                        break;
                    case (int)TranslatorTags.KeyInfo:
                        s.data.SetUInt8(1);
                        break;
                    case (int)TranslatorTags.AgreedKey:
                        s.data.SetUInt8(2);
                        break;
                    case (int)TranslatorTags.KeyParameters:
                        s.data.SetUInt8(1);
                        s.data.SetUInt8(Byte.Parse(GetValue(node, s)));
                        break;
                    case (int)TranslatorTags.AttributeValue:
                        break;
                    case (int)TranslatorTags.MaxInfoTX:
                        value = Byte.Parse(GetValue(node, s));
                        if ((s.command == Command.Snrm && !s.settings.IsServer) ||
                            (s.command == Command.Ua && s.settings.IsServer))
                        {
                            s.settings.Hdlc.MaxInfoRX = (UInt16)value;
                        }
                        s.data.SetUInt8((byte)HDLCInfo.MaxInfoRX);
                        s.data.SetUInt8(1);
                        s.data.SetUInt8((byte)value);
                        break;
                    case (int)TranslatorTags.MaxInfoRX:
                        value = Byte.Parse(GetValue(node, s));
                        if ((s.command == Command.Snrm && !s.settings.IsServer) ||
                            (s.command == Command.Ua && s.settings.IsServer))
                        {
                            s.settings.Hdlc.MaxInfoTX = (UInt16)value;
                        }
                        s.data.SetUInt8((byte)HDLCInfo.MaxInfoTX);
                        s.data.SetUInt8(1);
                        s.data.SetUInt8((byte)value);
                        break;
                    case (int)TranslatorTags.WindowSizeTX:
                        value = Byte.Parse(GetValue(node, s));
                        if ((s.command == Command.Snrm && !s.settings.IsServer) ||
                           (s.command == Command.Ua && s.settings.IsServer))
                        {
                            s.settings.Hdlc.WindowSizeRX = (byte)value;
                        }
                        s.data.SetUInt8((byte)HDLCInfo.WindowSizeRX);
                        s.data.SetUInt8(4);
                        s.data.SetUInt32(value);
                        break;
                    case (int)TranslatorTags.WindowSizeRX:
                        value = Byte.Parse(GetValue(node, s));
                        if ((s.command == Command.Snrm && !s.settings.IsServer) ||
                           (s.command == Command.Ua && s.settings.IsServer))
                        {
                            s.settings.Hdlc.WindowSizeTX = (byte)value;
                        }
                        s.data.SetUInt8((byte)HDLCInfo.WindowSizeTX);
                        s.data.SetUInt8(4);
                        s.data.SetUInt32(value);
                        break;
                    case (byte)Command.InitiateRequest:
                        break;
                    case (UInt16)TranslatorTags.ValueList:
                        GXCommon.SetObjectCount(node.ChildNodes.Count, s.data);
                        break;
                    case (UInt16)TranslatorTags.DataAccessResult:
                        s.data.SetUInt8((byte)ValueOfErrorCode(s.OutputType, GetValue(node, s)));
                        break;
                    case (UInt16)TranslatorTags.WriteDataBlockAccess:
                        break;
                    case (UInt16)TranslatorTags.FrameType:
                        break;
                    case (UInt16)TranslatorTags.BlockControl:
                        s.attributeDescriptor.SetUInt8((Byte)s.ParseShort(GetValue(node, s)));
                        break;
                    case (UInt16)TranslatorTags.BlockNumberAck:
                        s.attributeDescriptor.SetUInt16((UInt16)s.ParseShort(GetValue(node, s)));
                        break;
                    case (UInt16)TranslatorTags.BlockData:
                        s.data.Set(GXCommon.HexToBytes(GetValue(node, s)));
                        break;
                    case (UInt16)TranslatorTags.ContentsDescription:
                        GetNodeTypes(s, node, true);
                        return;
                    case (UInt16)TranslatorTags.ArrayContents:
                        if (s.OutputType == TranslatorOutputType.SimpleXml)
                        {
                            GetNodeValues(s, node);
                        }
                        else
                        {
                            tmp = GXCommon.HexToBytes(GetValue(node, s));
                            GXCommon.SetObjectCount(tmp.Length, s.data);
                            s.data.Set(tmp);
                        }
                        break;
                    case (UInt16)TranslatorTags.NetworkId:
                        s.networkId = (byte)s.ParseShort(GetValue(node, s));
                        break;
                    case (UInt16)TranslatorTags.PhysicalDeviceAddress:
                        s.physicalDeviceAddress = GXCommon.HexToBytes(GetValue(node, s));
                        s.command = Command.None;
                        break;
                    case (UInt16)TranslatorTags.StateError:
                        if (s.OutputType == TranslatorOutputType.SimpleXml)
                        {
                            s.attributeDescriptor.SetUInt8((byte)TranslatorSimpleTags.ValueofStateError(GetValue(node, s)));
                        }
                        else
                        {
                            s.attributeDescriptor.SetUInt8((byte)TranslatorStandardTags.ValueofStateError(GetValue(node, s)));
                        }
                        break;
                    case (UInt16)TranslatorTags.ServiceError:
                        if (s.OutputType == TranslatorOutputType.SimpleXml)
                        {
                            s.attributeDescriptor.SetUInt8((byte)TranslatorSimpleTags.ValueOfExceptionServiceError(GetValue(node, s)));
                        }
                        else
                        {
                            s.attributeDescriptor.SetUInt8((byte)TranslatorStandardTags.ValueOfExceptionServiceError(GetValue(node, s)));
                        }
                        break;
                    default:
                        throw new ArgumentException("Invalid node: " + node.Name);
                }
            }
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    ReadNode(childNode, s);
                }
            }
            if (preData != null)
            {
                GXCommon.SetObjectCount(node.ChildNodes.Count, preData);
                preData.Set(s.data);
                s.data.Size = 0;
                s.data.Set(preData);
            }
        }

        private static void AppendValue(GXDLMSXmlSettings s, object dt, GXByteBuffer tmp2, string[] values, ref int pos)
        {
            GXByteBuffer tmp = new GXByteBuffer();
            if (dt is List<object>)
            {
                foreach (object dt2 in (List<object>)dt)
                {
                    //For some reason there is count here in Italy standard. Add it.
                    if (s.settings.Standard == Standard.Italy && dt is List<object>)
                    {
                        tmp2.SetUInt8((byte)((List<object>)dt).Count);
                    }
                    AppendValue(s, dt2, tmp2, values, ref pos);
                }
            }
            else
            {
                string it = values[pos];
                if ((DataType)dt == DataType.OctetString)
                {
                    GXCommon.SetData(s.settings, tmp, (DataType)dt, GXCommon.HexToBytes(it));
                }
                else
                {
                    GXCommon.SetData(s.settings, tmp, (DataType)dt, Convert.ChangeType(it, GXCommon.GetDataType((DataType)dt)));
                }
                if (tmp.Size == 1)
                {
                    // If value is null.
                    s.data.SetUInt8(0);
                }
                else
                {
                    tmp2.Set(tmp.SubArray(1, tmp.Size - 1));
                }
                ++pos;
            }
        }

        private static void GetNodeValues(GXDLMSXmlSettings s, XmlNode node)
        {
            s.data.Position = 2;
            GXDataInfo info = new GXDataInfo();
            List<object> types = (List<object>)GXCommon.GetCompactArray(null, s.data, info, true);
            object dt;
            s.data.Position = 0;
            GXByteBuffer tmp2 = new GXByteBuffer();
            foreach (XmlNode str in node.ChildNodes)
            {
                foreach (string r in str.Value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (r.Trim() == "")
                    {
                        continue;
                    }
                    int col = 0;
                    int pos = 0;
                    string[] values = r.Trim().Split(';');
                    while (pos < values.Length)
                    {
                        dt = types[col % types.Count];
                        AppendValue(s, dt, tmp2, values, ref pos);
                        ++col;
                    }
                }
            }
            GXCommon.SetObjectCount(tmp2.Size, s.data);
            s.data.Set(tmp2);
        }

        private static void GetNodeTypes(GXDLMSXmlSettings s, XmlNode node, bool addCount)
        {
            int len = node.ChildNodes.Count;
            if (len > 1 && addCount)
            {
                s.data.SetUInt8(DataType.Structure);
                GXCommon.SetObjectCount(len, s.data);
            }
            foreach (XmlNode node2 in node.ChildNodes)
            {
                String str;
                if (s.OutputType == TranslatorOutputType.SimpleXml)
                {
                    str = node2.Name.ToLower();
                }
                else
                {
                    if (node2.Name.StartsWith("x:"))
                    {
                        str = node2.Name.Substring(2);
                    }
                    else
                    {
                        str = node2.Name;
                    }
                }
                int tag = s.tags[str];
                byte type = (byte)(tag - GXDLMS.DATA_TYPE_OFFSET);
                if (type == (byte)DataType.Structure || type == (byte)DataType.Array)
                {
                    s.data.SetUInt8(type);
                    len = node2.ChildNodes.Count;
                    if (type == (byte)DataType.Array)
                    {
                        s.data.SetUInt16((UInt16)len);
                    }
                    else
                    {
                        GXCommon.SetObjectCount(len, s.data);
                    }
                    GetNodeTypes(s, node2, false);
                }
                else
                {
                    s.data.SetUInt8(type);
                }
            }
        }

        private static void GetOctetString(XmlNode node, GXDLMSXmlSettings s)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetHexString(GetValue(node, s));
            GXCommon.SetData(s.settings, s.data, DataType.OctetString, bb.Array());
        }

        private static void GetFloat32(XmlNode node, GXDLMSXmlSettings s)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetHexString(GetValue(node, s));
            GXCommon.SetData(s.settings, s.data, DataType.Float32, bb.GetFloat());
        }

        private static void GetFloat64(XmlNode node, GXDLMSXmlSettings s)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetHexString(GetValue(node, s));
            GXCommon.SetData(s.settings, s.data, DataType.Float64, bb.GetDouble());
        }

        /// <summary>
        /// Convert xml to hex string.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string XmlToHexPdu(string xml)
        {
            return GXCommon.ToHex(XmlToPdu(xml), false);
        }

        /// <summary>
        /// Convert xml to byte array.
        /// </summary>
        /// <param name="xml">Converted xml.</param>
        /// <returns>Converted bytes.</returns>
        public byte[] XmlToPdu(string xml)
        {
            return XmlToPdu(xml, null);
        }

        /// <summary>
        /// Convert xml to byte array.
        /// </summary>
        /// <param name="xml">Converted xml.</param>
        /// <returns>Converted bytes.</returns>
        internal byte[] XmlToPdu(string xml, GXDLMSXmlSettings s)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            if (s == null)
            {
                s = new GXDLMSXmlSettings(OutputType, Hex, ShowStringAsHex, tagsByName);
                ((GXCiphering)s.settings.Cipher).TestMode = true;
                s.settings.Standard = Standard;
                GetCiphering(s.settings, false);
            }
            ReadAllNodes(doc, s);
            GXByteBuffer bb = new GXByteBuffer();
            GXDLMSLNParameters ln;
            GXDLMSSNParameters sn;
            switch (s.command)
            {
                case Command.InitiateRequest:
                case Command.InitiateResponse:
                    break;
                case Command.ReadRequest:
                case Command.WriteRequest:
                case Command.ReadResponse:
                case Command.WriteResponse:
                    sn = new GXDLMSSNParameters(s.settings, s.command, s.count,
                                                s.requestType, s.attributeDescriptor, s.data);
                    GXDLMS.GetSNPdu(sn, bb);
                    break;
                case Command.GetRequest:
                case Command.GetResponse:
                case Command.SetRequest:
                case Command.SetResponse:
                case Command.MethodRequest:
                case Command.MethodResponse:
                    ln = new GXDLMSLNParameters(s.settings, 0, s.command, s.requestType,
                                                s.attributeDescriptor, s.data, 0xff, Command.None);
                    GXDLMS.GetLNPdu(ln, bb);
                    break;
                case Command.GloGetRequest:
                case Command.GloGetResponse:
                case Command.GloSetRequest:
                case Command.GloSetResponse:
                case Command.GloMethodRequest:
                case Command.GloMethodResponse:
                case Command.GloReadRequest:
                case Command.GloWriteRequest:
                case Command.GloReadResponse:
                case Command.GloWriteResponse:
                case Command.DedGetRequest:
                case Command.DedGetResponse:
                case Command.DedSetRequest:
                case Command.DedSetResponse:
                case Command.DedMethodRequest:
                case Command.DedMethodResponse:
                    bb.SetUInt8((byte)s.command);
                    GXCommon.SetObjectCount(s.data.Size, bb);
                    bb.Set(s.data);
                    break;
                case Command.UnacceptableFrame:
                    break;
                case Command.Snrm:
                    s.settings.IsServer = false;
                    if (s.data.Size != 0)
                    {
                        bb.SetUInt8(0x81);
                        bb.SetUInt8(0x80);
                        bb.SetUInt8((byte)s.data.Size);
                        bb.Set(s.data);
                        s.data.Clear();
                        s.data.Set(bb);
                        bb.Clear();
                    }
                    bb.Set(GXDLMS.GetHdlcFrame(s.settings, (byte)Command.Snrm, s.data));
                    break;
                case Command.Ua:
                    if (s.data.Size != 0)
                    {
                        bb.SetUInt8(0x81);
                        bb.SetUInt8(0x80);
                        bb.SetUInt8((byte)s.data.Size);
                        bb.Set(s.data);
                        s.data.Clear();
                        s.data.Set(bb);
                        bb.Clear();
                    }
                    bb.Set(GXDLMS.GetHdlcFrame(s.settings, (byte)Command.Ua, s.data));
                    break;
                case Command.Aarq:
                case Command.GloInitiateRequest:
                    GXAPDU.GenerateAarq(s.settings, s.settings.Cipher, s.data, bb);
                    break;
                case Command.Aare:
                case Command.GloInitiateResponse:
                    GXAPDU.GenerateAARE(s.settings, bb, s.result, s.diagnostic, s.settings.Cipher, s.attributeDescriptor, s.data);
                    break;
                case Command.DisconnectRequest:
                    break;
                case Command.ReleaseRequest:
                    bb.SetUInt8((byte)s.command);
                    if (s.data.Size == 0)
                    {
                        GXAPDU.GenerateUserInformation(s.settings, s.settings.Cipher, null, s.data);
                    }
                    // Len
                    bb.SetUInt8((byte)(3 + s.data.Size));
                    // BerType
                    bb.SetUInt8(BerType.Context);
                    // Len.
                    bb.SetUInt8(1);
                    bb.SetUInt8(s.reason);
                    if (s.data.Size == 0)
                    {
                        bb.SetUInt8(0);
                    }
                    else
                    {
                        bb.Set(s.data);
                    }
                    break;
                case Command.ReleaseResponse:
                    bb.SetUInt8((byte)s.command);
                    //Len
                    bb.SetUInt8(3);
                    //BerType
                    bb.SetUInt8(BerType.Context);
                    //Len.
                    bb.SetUInt8(1);
                    bb.SetUInt8(s.reason);
                    break;
                case Command.ConfirmedServiceError:
                case Command.ExceptionResponse:
                    bb.SetUInt8(s.command);
                    bb.Set(s.attributeDescriptor);
                    break;
                case Command.GeneralBlockTransfer:
                    ln = new GXDLMSLNParameters(s.settings, 0, s.command, s.requestType,
                                                 s.attributeDescriptor, s.data, 0xff, Command.None);
                    GXDLMS.GetLNPdu(ln, bb);
                    break;
                case Command.AccessRequest:
                    ln = new GXDLMSLNParameters(s.settings, 0, s.command, s.requestType,
                                                s.attributeDescriptor, s.data, 0xff, Command.None);
                    GXDLMS.GetLNPdu(ln, bb);
                    break;
                case Command.AccessResponse:
                    ln = new GXDLMSLNParameters(s.settings, 0, s.command, s.requestType,
                                                s.attributeDescriptor, s.data, 0xff, Command.None);
                    GXDLMS.GetLNPdu(ln, bb);
                    break;
                case Command.DataNotification:
                    ln = new GXDLMSLNParameters(s.settings, 0, s.command, s.requestType,
                                                s.attributeDescriptor, s.data, 0xff, Command.None);
                    ln.time = s.time;
                    GXDLMS.GetLNPdu(ln, bb);
                    break;
                case Command.InformationReport:
                    sn = new GXDLMSSNParameters(s.settings, s.command, s.count,
                                               s.requestType, s.attributeDescriptor, s.data);
                    sn.time = s.time;
                    GXDLMS.GetSNPdu(sn, bb);
                    break;
                case Command.EventNotification:
                    ln = new GXDLMSLNParameters(s.settings, 0, s.command, s.requestType,
                                                s.attributeDescriptor, s.data, 0xff, Command.None);
                    ln.time = s.time;
                    GXDLMS.GetLNPdu(ln, bb);
                    break;
                case Command.GeneralGloCiphering:
                    bb.SetUInt8(s.command);
                    GXCommon.SetObjectCount(
                            s.settings.SourceSystemTitle.Length, bb);
                    bb.Set(s.settings.SourceSystemTitle);
                    GXCommon.SetObjectCount(s.data.Size, bb);
                    bb.Set(s.data);
                    break;
                case Command.GloEventNotification:
                    break;
                default:
                case Command.None:
                    throw new ArgumentException("Invalid command.");
            }
            if (s.physicalDeviceAddress != null)
            {
                GXByteBuffer bb2 = new GXByteBuffer();
                bb2.SetUInt8(s.gwCommand);
                bb2.SetUInt8(s.networkId);
                if (s.physicalDeviceAddress != null)
                {
                    GXCommon.SetObjectCount(s.physicalDeviceAddress.Length, bb2);
                    bb2.Set(s.physicalDeviceAddress);
                }
                else
                {
                    bb2.SetUInt8(0);
                }
                bb2.Set(bb);
                return bb2.Array();
            }
            return bb.Array();
        }

        private void GetAllDataNodes(XmlNodeList nodes, GXDLMSXmlSettings s)
        {
            GXByteBuffer preData;
            int tag;
            foreach (XmlNode it in nodes)
            {
                if (it.NodeType == XmlNodeType.Element)
                {
                    if (s.OutputType == TranslatorOutputType.SimpleXml)
                    {
                        tag = s.tags[it.Name.ToLower()];
                    }
                    else
                    {
                        tag = s.tags[it.Name];
                    }
                    if (tag == (int)TranslatorTags.RawData)
                    {
                        s.data.SetHexString(it.InnerText);
                    }
                    else
                    {
                        preData = UpdateDataType(it, s, tag);
                        if (preData != null)
                        {
                            GXCommon.SetObjectCount(it.ChildNodes.Count, preData);
                            preData.Set(s.data);
                            s.data.Size = 0;
                            s.data.Set(preData);
                            GetAllDataNodes(it.ChildNodes, s);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert XML data to PDU bytes.
        /// </summary>
        /// <param name="xml">XML data.</param>
        /// <returns>Data in bytes.</returns>
        public byte[] XmlToData(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            GXDLMSXmlSettings s = new GXDLMSXmlSettings(OutputType, Hex, ShowStringAsHex, tagsByName);
            GetAllDataNodes(doc.ChildNodes, s);
            return s.data.Array();
        }

        /// <summary>
        /// Convert data bytes to XML.
        /// </summary>
        /// <param name="data">Data to parse as a hex string.</param>
        /// <param name="xml">Generated xml.</param>
        /// <returns></returns>
        public void DataToXml(string data, out string xml)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetHexString(data);
            DataToXml(bb, out xml);
        }

        /// <summary>
        /// Convert data bytes to XML.
        /// </summary>
        /// <param name="data">Data to parse.</param>
        /// <param name="xml">Generated xml.</param>
        /// <returns></returns>
        public void DataToXml(byte[] data, out string xml)
        {
            DataToXml(new GXByteBuffer(data), out xml);
        }

        /// <summary>
        /// Convert data bytes to XML.
        /// </summary>
        /// <param name="data">Data to parse.</param>
        /// <param name="xml">Generated xml.</param>
        public void DataToXml(GXByteBuffer data, out string xml)
        {
            GXDataInfo di = new GXDataInfo();
            GXDLMSSettings settings = new GXDLMSSettings(false, InterfaceType.HDLC);
            di.xml = new GXDLMSTranslatorStructure(OutputType, OmitXmlNameSpace, Hex, ShowStringAsHex, Comments, tags);
            try
            {
                GXCommon.GetData(settings, data, di);
            }
            finally
            {
                xml = di.xml.ToString();
                if (data.Position != data.Size)
                {
                    xml += "<!--";
                    xml += Environment.NewLine + "Extra data:" + Environment.NewLine + data.RemainingHexString(true);
                    xml += Environment.NewLine + "-->";
                }
            }
        }

        /// <summary>
        /// Convert data to xml.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ValueToXml(object value)
        {
            SortedList<int, string> tags = new SortedList<int, string>();
            TranslatorSimpleTags.GetDataTypeTags(tags);
            GXDLMSTranslatorStructure xml = new GXDLMSTranslatorStructure(TranslatorOutputType.SimpleXml, true, false, false, false, tags);
            GXCommon.DatatoXml(value, xml);
            return xml.ToString();
        }

        private static List<object> GetNodes(XmlNodeList nodes)
        {
            List<object> values = new List<object>();
            String str;
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (node.Name.StartsWith("x:"))
                    {
                        str = node.Name.Substring(2);
                    }
                    else
                    {
                        str = node.Name;
                    }
                    DataType dt = (DataType)Enum.Parse(typeof(DataType), str);
                    if (dt == DataType.Array)
                    {
                        GXArray arr = new GXArray();
                        arr.AddRange(GetNodes(node.ChildNodes));
                        values.Add(arr);
                    }
                    else if (dt == DataType.Structure)
                    {
                        GXStructure strucure = new GXStructure();
                        strucure.AddRange(GetNodes(node.ChildNodes));
                        values.Add(strucure);
                    }
                    else if (dt == DataType.OctetString)
                    {
                        values.Add(GXCommon.HexToBytes(node.Attributes["Value"].Value));
                    }
                    else if (dt == DataType.Enum)
                    {
                        values.Add(new GXEnum(byte.Parse(node.Attributes["Value"].Value)));
                    }
                    else if (dt == DataType.BitString)
                    {
                        values.Add(new GXBitString(node.Attributes["Value"].Value));
                    }
                    else if (dt == DataType.None)
                    {
                        values.Add(null);
                    }
                    else
                    {
                        values.Add(Convert.ChangeType(node.Attributes["Value"].Value, GXCommon.GetDataType(dt)));
                    }
                }
            }
            return values;
        }

        /// <summary>
        /// Convert data to xml.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object XmlToValue(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            List<object> values = GetNodes(doc.ChildNodes);
            if (values.Count == 0)
            {
                return null;
            }
            return values[0];
        }
    }
}