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
        ///AssociationResult
        ///</summary>
        internal AssociationResult ResultComponent
        {
            get;
            set;
        }
        ///<summary>
        ///SourceDiagnostic
        ///</summary>
        internal SourceDiagnostic ResultDiagnosticValue
        {
            get;
            set;
        }       

        ///<summary>
        ///Retrieves the string that indicates the level of authentication, if any. 
        ///</summary>
        internal static void GetAuthenticationString(GXDLMSSettings settings, GXByteBuffer data)
        {
            //If authentication is used.
            if (settings.Authentication != Authentication.None)
            {
                //Add sender ACSE-requirements field component.
                data.SetUInt8((byte)GXBer.ContextClass | (byte)GXAarqApdu.SenderAcseRequirements);
                data.SetUInt8(2);
                data.SetUInt8(GXBer.BitStringTag | GXBer.OctetStringTag);
                data.SetUInt8(0x80);

                data.SetUInt8((byte)GXBer.ContextClass | (byte)GXAarqApdu.MechanismName);
                //Len
                data.SetUInt8(7);
                // OBJECT IDENTIFIER
                byte[] p = { (byte)0x60, (byte)0x85, (byte)0x74, (byte)0x05, (byte)0x08, (byte)0x02, (byte)settings.Authentication };
                data.Set(p);
                //Add Calling authentication information.
                int len = 0;
                if (settings.Password != null)
                {
                    len = settings.Password.Length;
                }
                data.SetUInt8((byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.CallingAuthenticationValue); //0xAC
                //Len
                data.SetUInt8((byte)(2 + len));
                //Add authentication information.
                data.SetUInt8((byte)GXBer.ContextClass);
                //Len.
                data.SetUInt8((byte)len);
                if (len != 0)
                {
                    data.Set(settings.Password);
                }
            }
        }

        ///<summary>
        ///CodeData
        ///</summary>
        internal void CodeData(GXDLMSSettings settings, GXICipher cipher, GXByteBuffer data)
        {
            //AARQ APDU Tag
            data.SetUInt8(GXBer.ApplicationClass | GXBer.Constructed);
            //Length is updated later.
            UInt16 offset = data.Size;
            data.SetUInt8(0);
            ///////////////////////////////////////////
            // Add Application context name.
            GXApplicationContextName.CodeData(settings, data, cipher);
            GetAuthenticationString(settings, data);
            GXUserInformation.CodeData(settings, cipher, data);
            data.SetUInt8(offset, (byte)(data.Size - offset - 1));
        }

        ///<summary>
        ///EncodeData
        ///</summary>
        internal bool EncodeData(GXDLMSSettings settings, GXICipher cipher, GXByteBuffer buff)
        {
            // Get AARE tag and length
            int tag = buff.GetUInt8();
            if (settings.IsServer)
            {
                if (tag != ((byte)GXBer.ApplicationClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.ProtocolVersion))
                {
                    throw new Exception("Invalid tag.");
                }
            }
            else
            {
                if (tag != ((byte)GXBer.ApplicationClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.ApplicationContextName))
                {
                    throw new Exception("Invalid tag.");
                }
            }
            int len = buff.GetUInt8();
            int size = buff.Size - buff.Position;
            if (len > size)
            {
                throw new Exception("Not enough data.");
            }
            while (buff.Position < buff.Size)
            {
                tag = buff.GetUInt8();
                switch (tag)
                {
                    case (byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.ApplicationContextName://0xA1
                        if (!GXApplicationContextName.EncodeData(settings, buff))
                        {
                            return false;
                        }
                        break;
                    case (byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.CalledApTitle:////Result 0xA2
                        //Get len.
                        if (buff.GetUInt8() != 3)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        //Choice for result (INTEGER, universal)
                        if (buff.GetUInt8() != (byte)GXBer.IntegerTag)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        //Get len.
                        if (buff.GetUInt8() != 1)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        this.ResultComponent = (AssociationResult)buff.GetUInt8();
                        break;
                    case (byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.CalledAeQualifier:////SourceDiagnostic 0xA3
                        len = buff.GetUInt8();
                        // ACSE service user tag.
                        tag = buff.GetUInt8();
                        len = buff.GetUInt8();
                        // Result source diagnostic component.
                        if (buff.GetUInt8() != (byte)GXBer.IntegerTag)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        if (buff.GetUInt8() != 1)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        ResultDiagnosticValue = (SourceDiagnostic)buff.GetUInt8();
                        break;
                    case (byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.CalledApInvocationId:////Result 0xA4
                        //Get len.
                        if (buff.GetUInt8() != 0xA)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        //Choice for result (Universal, Octetstring type)
                        if (buff.GetUInt8() != (byte)GXBer.OctetStringTag)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        //responding-AP-title-field
                        //Get len.
                        len = buff.GetUInt8();
                        settings.StoCChallenge = new byte[len];
                        buff.Get(settings.StoCChallenge);
                        break;
                    //Client Challenge.
                    case (byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.CallingApTitle://0xA6
                        len = buff.GetUInt8();
                        tag = buff.GetUInt8();
                        len = buff.GetUInt8();
                        settings.CtoSChallenge = new byte[len];
                        buff.Get(settings.CtoSChallenge);
                        break;
                    //Server Challenge.
                    case (byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.SenderAcseRequirements://0xAA
                        len = buff.GetUInt8();
                        tag = buff.GetUInt8();
                        len = buff.GetUInt8();
                        settings.StoCChallenge = new byte[len];
                        buff.Get(settings.StoCChallenge);
                        break;
                    case (byte)GXBer.ContextClass | (byte)GXAarqApdu.SenderAcseRequirements://0x8A
                    case (byte)GXBer.ContextClass | (byte)GXAarqApdu.CallingApInvocationId://0x88
                        //Get sender ACSE-requirements field component.
                        if (buff.GetUInt8() != 2)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        if (buff.GetUInt8() != (byte)GXBer.ObjectDescriptor)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        if (buff.GetUInt8() != 0x80)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        break;
                    case (byte)GXBer.ContextClass | (byte)GXAarqApdu.MechanismName://0x8B
                    case (byte)GXBer.ContextClass | (byte)GXAarqApdu.CallingAeInvocationId://0x89
                        len = buff.GetUInt8();
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
                        if (tmp < 0 || tmp > 5)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        settings.Authentication = (Authentication)tmp;
                        break;
                    case (byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.CallingAuthenticationValue://0xAC
                        len = buff.GetUInt8();
                        // Get authentication information.
                        if (buff.GetUInt8() != 0x80)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        len = buff.GetUInt8();
                        settings.Password = new byte[len];
                        buff.Get(settings.Password);
                        break;
                    case (byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.UserInformation://0xBE
                        if (this.ResultComponent != AssociationResult.Accepted && ResultDiagnosticValue != SourceDiagnostic.None)
                        {
                            return true;
                        }
                        GXUserInformation.EncodeData(settings, cipher, buff);
                        break;
                    default:
                        //Unknown tags.
                        System.Diagnostics.Debug.WriteLine("Unknown tag: " + tag + ".");
                        len = buff.GetUInt8();
                        buff.Position += (UInt16)len;
                        break;
                }
            }
            return true;
        }

        private static byte[] GetUserInformation(GXDLMSSettings settings, GXICipher cipher)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8(GXCommon.InitialResponce); // Tag for xDLMS-Initiate response
            data.SetUInt8(0x01);
            data.SetUInt8(0x00); // Usage field for the response allowed component (not used)
            // DLMS Version Number
            data.SetUInt8(06); 
            data.SetUInt8(0x5F);
            data.SetUInt8(0x1F);
            data.SetUInt8(0x04);// length of the conformance block
            data.SetUInt8(0x00);// encoding the number of unused bits in the bit string            
            if (settings.UseLogicalNameReferencing)
            {
                data.Set(settings.LnSettings.ConformanceBlock);
            }
            else
            {
                data.Set(settings.SnSettings.ConformanceBlock);

            }
            data.SetUInt16(settings.MaxReceivePDUSize);
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
                AesGcmParameter p = new AesGcmParameter(0x28, cipher.Security, cipher.FrameCounter,
                    cipher.SystemTitle, cipher.BlockCipherKey, cipher.AuthenticationKey, data);
                return GXDLMSChippering.EncryptAesGcm(p);
            }
            return data.Array();
        }

        ///<summary>
        ///Server generates AARE message.
        ///</summary>
        internal void GenerateAARE(GXDLMSSettings settings, GXByteBuffer data,
            AssociationResult result, SourceDiagnostic diagnostic, GXICipher cipher)
        {
            int offset = data.Position;
            // Set AARE tag and length
            data.SetUInt8(((byte)GXBer.ApplicationClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.ApplicationContextName)); //0x61
            // Length is updated later.
            data.SetUInt8(0);
            GXApplicationContextName.CodeData(settings, data, cipher);
            //Result
            data.SetUInt8((byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXBer.IntegerTag);//0xA2
            data.SetUInt8(3); //len
            data.SetUInt8(GXBer.IntegerTag); //Tag
            //Choice for result (INTEGER, universal)
            data.SetUInt8(1); //Len
            data.SetUInt8((byte) result); //ResultValue            
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
            if (cipher != null && cipher.IsCiphered())
            {
                data.SetUInt8((byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.CalledApInvocationId);
                data.SetUInt8((byte)(2 + cipher.SystemTitle.Length));
                data.SetUInt8((byte)GXBer.OctetStringTag);
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
                data.SetUInt8((byte) settings.Authentication);
                //Add tag.
                data.SetUInt8(0xAA);
                data.SetUInt8((byte)(2 + settings.StoCChallenge.Length));//Len
                data.SetUInt8((byte)GXBer.ContextClass);
                data.SetUInt8((byte)settings.StoCChallenge.Length);
                data.Set(settings.StoCChallenge);
            }            
            //Add User Information
            //Tag 0xBE
            data.SetUInt8((byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.UserInformation);
            byte[] tmp = GetUserInformation(settings, cipher);
            data.SetUInt8((byte)(2 + tmp.Length));
            //Coding the choice for user-information (Octet STRING, universal)
            data.SetUInt8(GXBer.OctetStringTag); 
            //Length
            data.SetUInt8((byte)tmp.Length); 
            data.Set(tmp);
            data.SetUInt8((UInt16)(offset + 1), (byte)(data.Size - offset - 2));
        }
    }
}
