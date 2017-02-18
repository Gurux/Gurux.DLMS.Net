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

namespace Gurux.DLMS.Internal
{
    ///<summary>
    ///The services to access the attributes and methods of COSEM objects are
    ///determined on DLMS/COSEM Application layer. The services are carried by
    ///Application Protocol Data Units (APDUs).
    ///<p />In DLMS/COSEM the meter is primarily a server, and the controlling system
    ///is a client. Also unsolicited (received without a request) messages are available.
    ///</summary>
    class GXAPDU
    {
        ///<summary>
        ///Retrieves the string that indicates the level of authentication, if any.
        ///</summary>
        private static void GetAuthenticationString(GXDLMSSettings settings, GXByteBuffer data)
        {
            //If authentication is used.
            if (settings.Authentication != Authentication.None)
            {
                //Add sender ACSE-requirements field component.
                data.SetUInt8((byte)BerType.Context | (byte)PduType.SenderAcseRequirements);
                data.SetUInt8(2);
                data.SetUInt8(BerType.BitString | BerType.OctetString);
                data.SetUInt8(0x80);

                data.SetUInt8((byte)BerType.Context | (byte)PduType.MechanismName);
                //Len
                data.SetUInt8(7);
                // OBJECT IDENTIFIER
                byte[] p = { (byte)0x60, (byte)0x85, (byte)0x74, (byte)0x05, (byte)0x08, (byte)0x02, (byte)settings.Authentication };
                data.Set(p);
                //Add Calling authentication information.
                int len = 0;
                byte[] callingAuthenticationValue = null;
                if (settings.Authentication == Authentication.Low)
                {
                    if (settings.Password != null)
                    {
                        callingAuthenticationValue = settings.Password;
                        len = callingAuthenticationValue.Length;
                    }
                }
                else
                {
                    callingAuthenticationValue = settings.CtoSChallenge;
                    len = callingAuthenticationValue.Length;
                }
                //0xAC
                data.SetUInt8((byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.CallingAuthenticationValue);
                //Len
                data.SetUInt8((byte)(2 + len));
                //Add authentication information.
                data.SetUInt8((byte)BerType.Context);
                //Len.
                data.SetUInt8((byte)len);
                if (len != 0)
                {
                    data.Set(callingAuthenticationValue);
                }
            }
        }

        /// <summary>
        /// Code application context name.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data">Byte buffer where data is saved.</param>
        /// <param name="cipher">Is ciphering settings.</param>
        private static void GenerateApplicationContextName(GXDLMSSettings settings, GXByteBuffer data, GXICipher cipher)
        {
            //Application context name tag
            data.SetUInt8(((byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.ApplicationContextName));
            //Len
            data.SetUInt8(0x09);
            data.SetUInt8(BerType.ObjectIdentifier);
            //Len
            data.SetUInt8(0x07);
            bool ciphered = cipher != null && cipher.IsCiphered();
            if (settings.UseLogicalNameReferencing)
            {
                if (ciphered)
                {
                    data.Set(GXCommon.LogicalNameObjectIdWithCiphering);
                }
                else
                {
                    data.Set(GXCommon.LogicalNameObjectID);
                }
            }
            else
            {
                if (ciphered)
                {
                    data.Set(GXCommon.ShortNameObjectIdWithCiphering);
                }
                else
                {
                    data.Set(GXCommon.ShortNameObjectID);
                }
            }
            //Add system title if cipher or GMAC authentication is used..
            if (!settings.IsServer && (ciphered || settings.Authentication == Authentication.HighGMAC))
            {
                if (cipher.SystemTitle == null || cipher.SystemTitle.Length == 0)
                {
                    throw new ArgumentNullException("SystemTitle");
                }
                //Add calling-AP-title
                data.SetUInt8(((byte)BerType.Context | (byte)BerType.Constructed | 6));
                //LEN
                data.SetUInt8((byte)(2 + cipher.SystemTitle.Length));
                data.SetUInt8((byte)BerType.OctetString);
                //LEN
                data.SetUInt8((byte)cipher.SystemTitle.Length);
                data.Set(cipher.SystemTitle);
            }
        }

        /// <summary>
        /// Generate User information initiate request.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="cipher"></param>
        /// <param name="data"></param>
        private static void GetInitiateRequest(GXDLMSSettings settings, GXICipher cipher, GXByteBuffer data)
        {
            // Tag for xDLMS-Initiate request
            data.SetUInt8((byte)Command.InitiateRequest);
            // Usage field for dedicated-key component. Not used
            data.SetUInt8(0x00);
            //encoding of the response-allowed component (BOOLEAN DEFAULT TRUE)
            // usage flag (FALSE, default value TRUE conveyed)
            data.SetUInt8(0);

            // Usage field of the proposed-quality-of-service component. Not used
            data.SetUInt8(0x00);
            data.SetUInt8(settings.DLMSVersion);
            // Tag for conformance block
            data.SetUInt8(0x5F);
            data.SetUInt8(0x1F);
            // length of the conformance block
            data.SetUInt8(0x04);
            // encoding the number of unused bits in the bit string
            data.SetUInt8(0x00);
            if (settings.UseLogicalNameReferencing)
            {
                data.Set(settings.LnSettings.ConformanceBlock);
            }
            else
            {
                data.Set(settings.SnSettings.ConformanceBlock);
            }
            data.SetUInt16(settings.MaxPduSize);
        }

        /// <summary>
        /// Generate user information.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="cipher"></param>
        /// <param name="data">Generated user information.</param>
        static internal void GenerateUserInformation(GXDLMSSettings settings, GXICipher cipher, GXByteBuffer encryptedData, GXByteBuffer data)
        {
            data.SetUInt8((byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.UserInformation);
            if (cipher == null || !cipher.IsCiphered())
            {
                //Length for AARQ user field
                data.SetUInt8(0x10);
                //Coding the choice for user-information (Octet STRING, universal)
                data.SetUInt8(BerType.OctetString);
                //Length
                data.SetUInt8(0x0E);
                GetInitiateRequest(settings, cipher, data);
            }
            else
            {
                if (encryptedData != null && encryptedData.Size != 0)
                {
                    //Length for AARQ user field
                    data.SetUInt8((byte)(4 + encryptedData.Size));
                    //Tag
                    data.SetUInt8(BerType.OctetString);
                    data.SetUInt8((byte)(2 + encryptedData.Size));
                    //Coding the choice for user-information (Octet STRING, universal)
                    data.SetUInt8((byte)Command.GloInitiateRequest);
                    data.SetUInt8((byte)encryptedData.Size);
                    data.Set(encryptedData);
                }
                else
                {
                    GXByteBuffer tmp = new GXByteBuffer();
                    GetInitiateRequest(settings, cipher, tmp);
                    byte[] crypted = cipher.Encrypt((byte)Command.GloInitiateRequest, cipher.SystemTitle, tmp.Array());
                    //Length for AARQ user field
                    data.SetUInt8((byte)(2 + crypted.Length));
                    //Coding the choice for user-information (Octet STRING, universal)
                    data.SetUInt8(BerType.OctetString);
                    data.SetUInt8((byte)crypted.Length);
                    data.Set(crypted);
                }
            }
        }

        ///<summary>
        /// Generates Aarq.
        ///</summary>
        static internal void GenerateAarq(GXDLMSSettings settings, GXICipher cipher, GXByteBuffer encryptedData, GXByteBuffer data)
        {
            //AARQ APDU Tag
            data.SetUInt8(BerType.Application | BerType.Constructed);
            //Length is updated later.
            int offset = data.Size;
            data.SetUInt8(0);
            ///////////////////////////////////////////
            // Add Application context name.
            GenerateApplicationContextName(settings, data, cipher);
            GetAuthenticationString(settings, data);
            GenerateUserInformation(settings, cipher, encryptedData, data);
            data.SetUInt8(offset, (byte)(data.Size - offset - 1));
        }

        private static void GetConformance(UInt32 value, GXDLMSTranslatorStructure xml)
        {
            foreach (var it in Enum.GetValues(typeof(Conformance)))
            {
                if (((UInt32)it & value) != 0)
                {
                    xml.AppendLine(TranslatorGeneralTags.ConformanceBit, "Name", it.ToString());
                }
            }
        }

        /// <summary>
        /// Parse User Information from PDU.
        /// </summary>
        public static void ParseUserInformation(GXDLMSSettings settings, GXICipher cipher, GXByteBuffer data, GXDLMSTranslatorStructure xml)
        {
            byte[] tmp;
            byte len = data.GetUInt8();
            GXByteBuffer tmp2 = new GXByteBuffer();
            tmp2.SetUInt8(0);
            if (data.Size - data.Position < len)
            {
                if (xml == null)
                {
                    throw new Exception("Not enough data.");
                }
                xml.AppendComment("Error: Invalid data size.");
            }
            if (xml != null && xml.OutputType == TranslatorOutputType.StandardXml)
            {
                len = (byte)(data.Size - data.Position);
                xml.AppendLine(Command.InitiateRequest, null, GXCommon
                               .ToHex(data.Data, false, data.Position, len));
                data.Position = data.Position + len;
                return;
            }
            //Excoding the choice for user information
            int tag = data.GetUInt8();
            if (tag != 0x4)
            {
                throw new Exception("Invalid tag.");
            }
            len = data.GetUInt8();
            if (data.Size - data.Position < len)
            {
                if (xml == null)
                {
                    throw new Exception("Not enough data.");
                }
                xml.AppendComment("Error: Invalid data size.");
            }
            //Tag for xDLMS-Initate.response
            tag = data.GetUInt8();
            if (tag == (byte)Command.GloInitiateResponse)
            {
                if (xml != null)
                {
                    int cnt = GXCommon.GetObjectCount(data);
                    tmp = new byte[cnt];
                    data.Get(tmp);
                    //<glo_InitiateResponse>
                    xml.AppendLine(Command.GloInitiateResponse, "Value", GXCommon.ToHex(tmp, false));
                    return;
                }
                --data.Position;
                cipher.Security = cipher.Decrypt(settings.SourceSystemTitle, data);
                tag = data.GetUInt8();
            }
            else if (tag == (byte)Command.GloInitiateRequest)
            {
                if (xml != null)
                {
                    int cnt = GXCommon.GetObjectCount(data);
                    tmp = new byte[cnt];
                    data.Get(tmp);
                    //<glo_InitiateRequest>
                    xml.AppendLine(Command.GloInitiateRequest, "Value", GXCommon.ToHex(tmp, false));
                    return;
                }
                --data.Position;
                cipher.Security = cipher.Decrypt(settings.SourceSystemTitle, data);
                tag = data.GetUInt8();
            }
            bool response = tag == (byte)Command.InitiateResponse;
            if (response)
            {
                if (xml != null)
                {
                    //<InitiateResponse>
                    xml.AppendStartTag(Command.InitiateResponse);
                }
                //Optional usage field of the negotiated quality of service component
                tag = data.GetUInt8();
                len = 0;
                if (tag != 0)//Skip if used.
                {
                    len = data.GetUInt8();
                    data.Position += len;
                    if (len == 0 && xml != null)
                    {
                        //NegotiatedQualityOfService
                        xml.AppendLine(TranslatorGeneralTags.NegotiatedQualityOfService, "Value", "00");
                    }
                }
            }
            else if (tag == (byte)Command.InitiateRequest)
            {
                if (xml != null)
                {
                    //<InitiateRequest>
                    xml.AppendStartTag(Command.InitiateRequest);
                }
                //Optional usage field of the negotiated quality of service component
                tag = data.GetUInt8();
                //CtoS.
                if (tag != 0)
                {
                    len = data.GetUInt8();
                    settings.CtoSChallenge = new byte[len];
                    data.Get(settings.CtoSChallenge);
                }
                //Optional usage field of the negotiated quality of service component
                tag = data.GetUInt8();
                if (tag != 0)//Skip if used.
                {
                    len = data.GetUInt8();
                    data.Position += len;
                }
                //Optional usage field of the proposed quality of service component
                tag = data.GetUInt8();
                if (tag != 0)//Skip if used.
                {
                    len = data.GetUInt8();
                    data.Position += len;
                }
            }
            else
            {
                throw new Exception("Invalid tag.");
            }
            //Get DLMS version number.
            if (!response)
            {
                if (data.GetUInt8() != 6)
                {
                    throw new Exception("Invalid DLMS version number.");
                }
                //ProposedDlmsVersionNumber
                if (xml != null)
                {
                    xml.AppendLine(TranslatorGeneralTags.ProposedDlmsVersionNumber, "Value", xml.IntegerToHex(settings.DLMSVersion, 2));
                }
            }
            else
            {
                if (data.GetUInt8() != 6)
                {
                    throw new Exception("Invalid DLMS version number.");
                }
                if (xml != null)
                {
                    xml.AppendLine(TranslatorGeneralTags.NegotiatedDlmsVersionNumber, "Value", xml.IntegerToHex(settings.DLMSVersion, 2));
                }
            }

            //Tag for conformance block
            tag = data.GetUInt8();
            if (tag != 0x5F)
            {
                throw new Exception("Invalid tag.");
            }
            //Old Way...
            if (data.GetUInt8(data.Position) == 0x1F)
            {
                data.GetUInt8();
            }
            len = data.GetUInt8();
            //The number of unused bits in the bit string.
            tag = data.GetUInt8();
            tmp = new byte[3];
            GXByteBuffer bb = new GXByteBuffer(4);
            data.Get(tmp);
            bb.SetUInt8(0);
            bb.Set(tmp);
            UInt32 v = bb.GetUInt32();
            if (settings.IsServer)
            {
                if (settings.UseLogicalNameReferencing)
                {
                    settings.NegotiatedConformance = (Conformance)v & settings.LnSettings.Conformance;
                }
                else
                {
                    settings.NegotiatedConformance = (Conformance)v & settings.SnSettings.Conformance;
                }
                if (xml != null)
                {
                    xml.AppendStartTag(TranslatorGeneralTags.ProposedConformance);
                    GetConformance(v, xml);
                }
            }
            else
            {
                if (xml != null)
                {
                    xml.AppendStartTag(TranslatorGeneralTags.NegotiatedConformance);
                    GetConformance(v, xml);
                }
                settings.NegotiatedConformance = (Conformance)v;
                if (settings.UseLogicalNameReferencing)
                {
                    settings.LnSettings.Conformance = settings.NegotiatedConformance;
                }
                else
                {
                    settings.SnSettings.Conformance = settings.NegotiatedConformance;
                }
            }

            if (!response)
            {
                //Proposed max PDU size.
                settings.MaxPduSize = data.GetUInt16();
                if (xml != null)
                {
                    //ProposedConformance closing
                    xml.AppendEndTag(TranslatorGeneralTags.ProposedConformance);
                    //ProposedMaxPduSize
                    xml.AppendLine(TranslatorGeneralTags.ProposedMaxPduSize, "Value", xml.IntegerToHex(settings.MaxPduSize, 4));
                }
                //If client asks too high PDU.
                if (settings.MaxPduSize > settings.MaxServerPDUSize)
                {
                    settings.MaxPduSize = settings.MaxServerPDUSize;
                }
            }
            else
            {
                //Max PDU size.
                settings.MaxPduSize = data.GetUInt16();
                if (xml != null)
                {
                    //NegotiatedConformance closing
                    xml.AppendEndTag(TranslatorGeneralTags.NegotiatedConformance);
                    //NegotiatedMaxPduSize
                    xml.AppendLine(TranslatorGeneralTags.NegotiatedMaxPduSize, "Value", xml.IntegerToHex(settings.MaxPduSize, 4));
                }
            }
            if (response)
            {
                //VAA Name
                tag = data.GetUInt16();
                if (xml != null)
                {
                    xml.AppendLine(TranslatorGeneralTags.VaaName, "Value", xml.IntegerToHex(tag, 4));
                }
                if (tag == 0x0007)
                {
                    // If LN
                    if (!settings.UseLogicalNameReferencing)
                    {
                        throw new ArgumentException("Invalid VAA.");
                    }
                }
                else if (tag == 0xFA00)
                {
                    // If SN
                    if (settings.UseLogicalNameReferencing)
                    {
                        throw new ArgumentException("Invalid VAA.");
                    }
                }
                else
                {
                    // Unknown VAA.
                    throw new ArgumentException("Invalid VAA.");
                }
                if (xml != null)
                {
                    //<InitiateResponse>
                    xml.AppendEndTag(Command.InitiateResponse);
                }
            }
            else if (xml != null)
            {
                //</InitiateRequest>
                xml.AppendEndTag(Command.InitiateRequest);
            }
        }

        /// <summary>
        /// Parse application context name.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="buff">Received data.</param>
        private static bool ParseApplicationContextName(GXDLMSSettings settings, GXByteBuffer buff, GXDLMSTranslatorStructure xml)
        {
            //Get length.
            int len = buff.GetUInt8();
            if (buff.Size - buff.Position < len)
            {
                throw new Exception("Encoding failed. Not enough data.");
            }
            if (buff.GetUInt8() != 0x6)
            {
                throw new Exception("Encoding failed. Not an Object ID.");
            }
            if (settings.IsServer && settings.Cipher != null)
            {
                settings.Cipher.Security = Gurux.DLMS.Enums.Security.None;
            }
            //Object ID length.
            len = buff.GetUInt8();
            if (xml != null)
            {
                if (buff.Compare(GXCommon.LogicalNameObjectID))
                {
                    if (xml.OutputType == TranslatorOutputType.SimpleXml)
                    {
                        xml.AppendLine(TranslatorGeneralTags.ApplicationContextName,
                                       "Value", "LN");
                    }
                    else
                    {
                        xml.AppendLine(
                            TranslatorGeneralTags.ApplicationContextName,
                            null, "1");
                    }
                    settings.UseLogicalNameReferencing = true;
                }
                else if (buff.Compare(GXCommon.LogicalNameObjectIdWithCiphering))
                {
                    if (xml.OutputType == TranslatorOutputType.SimpleXml)
                    {
                        xml.AppendLine(TranslatorGeneralTags.ApplicationContextName,
                                       "Value", "LN_WITH_CIPHERING");
                    }
                    else
                    {
                        xml.AppendLine(
                            TranslatorGeneralTags.ApplicationContextName,
                            null, "3");
                    }
                    settings.UseLogicalNameReferencing = true;
                }
                else if (buff.Compare(GXCommon.ShortNameObjectID))
                {
                    if (xml.OutputType == TranslatorOutputType.SimpleXml)
                    {
                        xml.AppendLine(TranslatorGeneralTags.ApplicationContextName,
                                       "Value", "SN");
                    }
                    else
                    {
                        xml.AppendLine(
                            TranslatorGeneralTags.ApplicationContextName,
                            null, "2");
                    }
                    settings.UseLogicalNameReferencing = false;
                }
                else if (buff.Compare(GXCommon.ShortNameObjectIdWithCiphering))
                {
                    if (xml.OutputType == TranslatorOutputType.SimpleXml)
                    {
                        xml.AppendLine(TranslatorGeneralTags.ApplicationContextName,
                                       "Value", "SN_WITH_CIPHERING");
                    }
                    else
                    {
                        xml.AppendLine(
                            TranslatorGeneralTags.ApplicationContextName,
                            null, "4");
                    }
                    settings.UseLogicalNameReferencing = false;
                }
                else
                {
                    return false;
                }
                return true;
            }
            if (settings.UseLogicalNameReferencing)
            {
                if (buff.Compare(GXCommon.LogicalNameObjectID))
                {
                    return true;
                }
                // If ciphering is used.
                return buff.Compare(GXCommon.LogicalNameObjectIdWithCiphering);
            }
            if (buff.Compare(GXCommon.ShortNameObjectID))
            {
                return true;
            }
            // If ciphering is used.
            return buff.Compare(GXCommon.ShortNameObjectIdWithCiphering);
        }

        private static void UpdateAuthentication(GXDLMSSettings settings,
                GXByteBuffer buff)
        {
            byte len = buff.GetUInt8();
            if (buff.GetUInt8() != 0x60)
            {
                throw new Exception("Invalid tag.");
            }
            if (buff.GetUInt8() != 0x85)
            {
                throw new Exception("Invalid tag.");
            }
            if (buff.GetUInt8() != 0x74)
            {
                throw new Exception("Invalid tag.");
            }
            if (buff.GetUInt8() != 0x05)
            {
                throw new Exception("Invalid tag.");
            }
            if (buff.GetUInt8() != 0x08)
            {
                throw new Exception("Invalid tag.");
            }
            if (buff.GetUInt8() != 0x02)
            {
                throw new Exception("Invalid tag.");
            }
            int tmp = buff.GetUInt8();
            if (tmp < 0 || tmp > 7)
            {
                throw new Exception("Invalid tag.");
            }
            settings.Authentication = (Authentication)tmp;
        }

        private static void AppendServerSystemTitleToXml(
            GXDLMSSettings settings, GXDLMSTranslatorStructure xml,
            int tag)
        {
            if (xml != null)
            {
                // RespondingAuthentication
                if (xml.OutputType == TranslatorOutputType.SimpleXml)
                {
                    xml.AppendLine(tag, "Value", GXCommon.ToHex(settings.StoCChallenge, false));
                }
                else
                {
                    xml.Append(tag, true);
                    xml.Append((int)TranslatorGeneralTags.CharString, true);
                    xml.Append(GXCommon.ToHex(settings.StoCChallenge, false));
                    xml.Append((int)TranslatorGeneralTags.CharString, false);
                    xml.Append(tag, false);
                    xml.Append("\r\n");
                }
            }
        }

        ///<summary>
        ///Parse APDU.
        ///</summary>
        static internal SourceDiagnostic ParsePDU(GXDLMSSettings settings, GXICipher cipher,
                GXByteBuffer buff, GXDLMSTranslatorStructure xml)
        {
            // Get AARE tag and length
            int tag = buff.GetUInt8();
            if (settings.IsServer)
            {
                if (tag != ((byte)BerType.Application | (byte)BerType.Constructed | (byte)PduType.ProtocolVersion))
                {
                    throw new Exception("Invalid tag.");
                }
            }
            else
            {
                if (tag != ((byte)BerType.Application | (byte)BerType.Constructed | (byte)PduType.ApplicationContextName))
                {
                    throw new Exception("Invalid tag.");
                }
            }
            int len = buff.GetUInt8();
            int size = buff.Size - buff.Position;
            if (len > size)
            {
                if (xml == null)
                {
                    throw new Exception("Not enough data.");
                }
                xml.AppendComment("Error: Invalid data size.");
            }
            //Opening tags
            if (xml != null)
            {
                if (settings.IsServer)
                {
                    xml.AppendStartTag(Command.Aarq);
                }
                else
                {
                    xml.AppendStartTag(Command.Aare);
                }
            }
            AssociationResult resultComponent = AssociationResult.Accepted;
            SourceDiagnostic resultDiagnosticValue = SourceDiagnostic.None;
            while (buff.Position < buff.Size)
            {
                tag = buff.GetUInt8();
                switch (tag)
                {
                    case (byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.ApplicationContextName://0xA1
                        if (!ParseApplicationContextName(settings, buff, xml))
                        {
                            throw new GXDLMSException(AssociationResult.PermanentRejected, SourceDiagnostic.ApplicationContextNameNotSupported);
                        }
                        break;
                    case (byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.CalledApTitle://0xA2
                                                                                                         //Get len.
                        if (buff.GetUInt8() != 3)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        //Choice for result (INTEGER, universal)
                        if (buff.GetUInt8() != (byte)BerType.Integer)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        //Get len.
                        if (buff.GetUInt8() != 1)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        resultComponent = (AssociationResult)buff.GetUInt8();
                        if (xml != null)
                        {
                            xml.AppendLine(TranslatorGeneralTags.AssociationResult, "Value", xml.IntegerToHex((int)resultComponent, 2));
                            xml.AppendStartTag(TranslatorGeneralTags.ResultSourceDiagnostic);
                        }
                        break;
                    case (byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.CalledAeQualifier:////SourceDiagnostic 0xA3
                        len = buff.GetUInt8();
                        // ACSE service user tag.
                        tag = buff.GetUInt8();
                        len = buff.GetUInt8();
                        // Result source diagnostic component.
                        if (buff.GetUInt8() != (byte)BerType.Integer)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        if (buff.GetUInt8() != 1)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        resultDiagnosticValue = (SourceDiagnostic)buff.GetUInt8();
                        if (xml != null)
                        {
                            xml.AppendLine(TranslatorGeneralTags.ACSEServiceUser, "Value", xml.IntegerToHex((int)resultDiagnosticValue, 2));
                            xml.AppendEndTag(TranslatorGeneralTags.ResultSourceDiagnostic);
                        }
                        break;
                    case (byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.CalledApInvocationId:
                        ////Result 0xA4
                        //Get len.
                        if (buff.GetUInt8() != 0xA)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        //Choice for result (Universal, Octetstring type)
                        if (buff.GetUInt8() != (byte)BerType.OctetString)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        //responding-AP-title-field
                        //Get len.
                        len = buff.GetUInt8();
                        settings.SourceSystemTitle = new byte[len];
                        buff.Get(settings.SourceSystemTitle);
                        if (xml != null)
                        {
                            //RespondingAPTitle
                            xml.AppendLine(TranslatorGeneralTags.RespondingAPTitle, "Value", GXCommon.ToHex(settings.SourceSystemTitle, false));
                        }
                        break;
                    //Client Challenge.
                    case (byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.CallingApTitle://0xA6
                        len = buff.GetUInt8();
                        tag = buff.GetUInt8();
                        len = buff.GetUInt8();
                        settings.SourceSystemTitle = new byte[len];
                        buff.Get(settings.SourceSystemTitle);
                        if (xml != null)
                        {
                            //CallingAPTitle
                            xml.AppendLine(TranslatorGeneralTags.CallingAPTitle, "Value", GXCommon.ToHex(settings.SourceSystemTitle, false));
                        }
                        break;
                    //Server Challenge.
                    case (byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.SenderAcseRequirements://0xAA
                        len = buff.GetUInt8();
                        tag = buff.GetUInt8();
                        len = buff.GetUInt8();
                        settings.StoCChallenge = new byte[len];
                        buff.Get(settings.StoCChallenge);
                        AppendServerSystemTitleToXml(settings, xml, tag);
                        break;
                    case (byte)BerType.Context | (byte)PduType.SenderAcseRequirements:
                    //0x8A
                    case (byte)BerType.Context | (byte)PduType.CallingApInvocationId:
                        //0x88

                        //Get sender ACSE-requirements field component.
                        if (buff.GetUInt8() != 2)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        if (buff.GetUInt8() != (byte)BerType.ObjectDescriptor)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        if (buff.GetUInt8() != 0x80)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        //SenderACSERequirements
                        if (xml != null)
                        {
                            xml.AppendLine(tag, "Value", "1");
                        }
                        break;
                    case (byte)BerType.Context | (byte)PduType.MechanismName://0x8B
                    case (byte)BerType.Context | (byte)PduType.CallingAeInvocationId://0x89
                        UpdateAuthentication(settings, buff);
                        if (xml != null)
                        {
                            if (xml.OutputType == TranslatorOutputType.SimpleXml)
                            {
                                xml.AppendLine(tag, "Value", settings.Authentication.ToString());
                            }
                            else
                            {
                                xml.AppendLine(tag, "Value", ((int)settings.Authentication).ToString());
                            }
                        }
                        break;
                    case (byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.CallingAuthenticationValue://0xAC
                        updatePassword(settings, buff, xml);
                        break;
                    case (byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.UserInformation:
                        //0xBE
                        if (xml == null && resultComponent != AssociationResult.Accepted && resultDiagnosticValue != SourceDiagnostic.None)
                        {
                            throw new GXDLMSException(resultComponent, resultDiagnosticValue);
                        }
                        ParseUserInformation(settings, cipher, buff, xml);
                        break;
                    default:
                        //Unknown tags.
                        System.Diagnostics.Debug.WriteLine("Unknown tag: " + tag + ".");
                        if (buff.Position < buff.Size)
                        {
                            len = buff.GetUInt8();
                            buff.Position += (UInt16)len;
                        }
                        break;
                }
            }
            //Closing tags
            if (xml != null)
            {
                if (settings.IsServer)
                {
                    xml.AppendEndTag(Command.Aarq);
                }
                else
                {
                    xml.AppendEndTag(Command.Aare);
                }
            }
            return resultDiagnosticValue;
        }

        private static void updatePassword(GXDLMSSettings settings, GXByteBuffer buff, GXDLMSTranslatorStructure xml)
        {
            int len = buff.GetUInt8();
            // Get authentication information.
            if (buff.GetUInt8() != 0x80)
            {
                throw new Exception("Invalid tag.");
            }
            len = buff.GetUInt8();
            if (settings.Authentication == Authentication.Low)
            {
                settings.Password = new byte[len];
                buff.Get(settings.Password);
            }
            else
            {
                settings.CtoSChallenge = new byte[len];
                buff.Get(settings.CtoSChallenge);
            }

            if (xml != null)
            {
                if (xml.OutputType == TranslatorOutputType.SimpleXml)
                {
                    if (settings.Authentication == Authentication.Low)
                    {
                        xml.AppendLine(TranslatorGeneralTags.CallingAuthentication,
                                       "Value",
                                       GXCommon.ToHex(settings.Password, false));
                    }
                    else
                    {
                        xml.AppendLine(TranslatorGeneralTags.CallingAuthentication,
                                       "Value",
                                       GXCommon.ToHex(settings.CtoSChallenge, false));
                    }
                }
                else
                {
                    xml.AppendStartTag(
                        TranslatorGeneralTags.CallingAuthentication);
                    xml.AppendStartTag(TranslatorGeneralTags.CharString);
                    if (settings.Authentication == Authentication.Low)
                    {
                        xml.Append(GXCommon.ToHex(settings.Password, false));
                    }
                    else
                    {
                        xml.Append(
                            GXCommon.ToHex(settings.CtoSChallenge, false));
                    }
                    xml.AppendEndTag(TranslatorGeneralTags.CharString);
                    xml.AppendEndTag(TranslatorGeneralTags.CallingAuthentication);
                }
            }
        }

        private static byte[] GetUserInformation(GXDLMSSettings settings, GXICipher cipher)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8(Command.InitiateResponse); // Tag for xDLMS-Initiate response
                                                     // NegotiatedQualityOfService (not used)
            data.SetUInt8(0x1);
            data.SetUInt8(0x00);
            // DLMS Version Number
            data.SetUInt8(06);
            data.SetUInt8(0x5F);
            data.SetUInt8(0x1F);
            data.SetUInt8(0x04);// length of the conformance block
            data.SetUInt8(0x00);// encoding the number of unused bits in the bit string
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt32((UInt32)settings.NegotiatedConformance);
            data.Set(bb.Data, 1, 3);
            data.SetUInt16(settings.MaxPduSize);
            //VAA Name VAA name (0x0007 for LN referencing and 0xFA00 for SN)
            if (settings.UseLogicalNameReferencing)
            {
                data.SetUInt16(0x0007);
            }
            else
            {
                data.SetUInt16(0xFA00);
            }
            if (cipher != null && cipher.IsCiphered())
            {
                return cipher.Encrypt((byte)Command.GloInitiateResponse, cipher.SystemTitle, data.Array());
            }
            return data.Array();
        }

        ///<summary>
        ///Server generates AARE message.
        ///</summary>
        internal static void GenerateAARE(GXDLMSSettings settings, GXByteBuffer data,
                                          AssociationResult result, SourceDiagnostic diagnostic, GXICipher cipher,
                                          GXByteBuffer encryptedData)
        {
            int offset = data.Size;
            // Set AARE tag and length
            data.SetUInt8(((byte)BerType.Application | (byte)BerType.Constructed | (byte)PduType.ApplicationContextName)); //0x61
                                                                                                                           // Length is updated later.
            data.SetUInt8(0);
            GenerateApplicationContextName(settings, data, cipher);
            //Result
            data.SetUInt8((byte)BerType.Context | (byte)BerType.Constructed | (byte)BerType.Integer);//0xA2
            data.SetUInt8(3); //len
            data.SetUInt8(BerType.Integer); //Tag
                                            //Choice for result (INTEGER, universal)
            data.SetUInt8(1); //Len
            data.SetUInt8((byte)result); //ResultValue
                                         //SourceDiagnostic
            data.SetUInt8(0xA3);
            data.SetUInt8(5); //len
            data.SetUInt8(0xA1); //Tag
            data.SetUInt8(3); //len
            data.SetUInt8(2); //Tag
                              //Choice for result (INTEGER, universal)
            data.SetUInt8(1); //Len
            data.SetUInt8((byte)diagnostic); //diagnostic

            //SystemTitle
            if (cipher != null && (cipher.IsCiphered() || settings.Authentication == Authentication.HighGMAC))
            {
                data.SetUInt8((byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.CalledApInvocationId);
                data.SetUInt8((byte)(2 + cipher.SystemTitle.Length));
                data.SetUInt8((byte)BerType.OctetString);
                data.SetUInt8((byte)cipher.SystemTitle.Length);
                data.Set(cipher.SystemTitle);
            }

            if (result != AssociationResult.PermanentRejected && diagnostic == SourceDiagnostic.AuthenticationRequired)
            {
                //Add server ACSE-requirenents field component.
                data.SetUInt8(0x88);
                data.SetUInt8(0x02);  //Len.
                data.SetUInt16(0x0780);
                //Add tag.
                data.SetUInt8(0x89);
                data.SetUInt8(0x07);//Len
                data.SetUInt8(0x60);
                data.SetUInt8(0x85);
                data.SetUInt8(0x74);
                data.SetUInt8(0x05);
                data.SetUInt8(0x08);
                data.SetUInt8(0x02);
                data.SetUInt8((byte)settings.Authentication);
                //Add tag.
                data.SetUInt8(0xAA);
                data.SetUInt8((byte)(2 + settings.StoCChallenge.Length));//Len
                data.SetUInt8((byte)BerType.Context);
                data.SetUInt8((byte)settings.StoCChallenge.Length);
                data.Set(settings.StoCChallenge);
            }
            byte[] tmp;
            //Add User Information
            //Tag 0xBE
            data.SetUInt8((byte)BerType.Context | (byte)BerType.Constructed | (byte)PduType.UserInformation);
            if (encryptedData != null && encryptedData.Size != 0)
            {
                GXByteBuffer tmp2 = new GXByteBuffer((UInt16)(2 + encryptedData.Size));
                tmp2.SetUInt8((byte)Command.GloInitiateResponse);
                GXCommon.SetObjectCount(encryptedData.Size, tmp2);
                tmp2.Set(encryptedData);
                tmp = tmp2.Array();
            }
            else
            {
                tmp = GetUserInformation(settings, cipher);
            }
            data.SetUInt8((byte)(2 + tmp.Length));
            //Coding the choice for user-information (Octet STRING, universal)
            data.SetUInt8(BerType.OctetString);
            //Length
            data.SetUInt8((byte)tmp.Length);
            data.Set(tmp);
            data.SetUInt8((UInt16)(offset + 1), (byte)(data.Size - offset - 2));
        }
    }
}
