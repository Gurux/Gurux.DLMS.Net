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
                data.SetUInt8(0x8A);
                data.SetUInt8(2);
                data.SetUInt16(0x0780);
                data.SetUInt8(0x8B);
                data.SetUInt8(7);
                byte[] p = { (byte)0x60, (byte)0x85, (byte)0x74, (byte)0x05, (byte)0x08, (byte)0x02, (byte)settings.Authentication };
                data.Set(p);
                //Add Calling authentication information.
                int len = 0;
                if (settings.Password != null)
                {
                    len = settings.Password.Length;
                }
                data.SetUInt8(0xAC);
                data.SetUInt8((byte)(2 + len));
                //Add authentication information.
                data.SetUInt8((byte)0x80);
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
        internal void CodeData(GXDLMSSettings settings, bool ciphering, GXByteBuffer data)
        {
            //AARQ APDU Tag
            data.SetUInt8(GXCommon.AARQTag);
            //Length
            UInt16 offset = data.Size;
            data.SetUInt8(0);
            ///////////////////////////////////////////
            // Add Application context name.
            GXApplicationContextName.CodeData(settings, data, ciphering);
            GetAuthenticationString(settings, data);
            GXUserInformation.CodeData(settings, data);
            data.SetUInt8(offset, (byte)(data.Size - offset - 1));
        }

        ///<summary>
        ///EncodeData
        ///</summary>
        internal bool EncodeData(GXDLMSSettings settings, GXByteBuffer buff)
        {
            // Get AARE tag and length
            int tag = buff.GetUInt8();
            if (tag != 0x61 && tag != 0x60 && tag != 0x81 && tag != 0x80)
            {
                throw new Exception("Invalid tag.");
            }
            int len = buff.GetUInt8();
            int size = buff.Size - buff.Position;
            if (len > size)
            {
                throw new Exception("Not enough data.");
            }
            while (buff.Position < buff.Size)
            {
                tag = buff.GetUInt8(buff.Position);
                if (tag == 0xA1)
                {
                    if (!GXApplicationContextName.EncodeData(settings, buff))
                    {
                        return false;
                    }
                }
                else if (tag == 0xBE)
                {
                    if (this.ResultComponent != AssociationResult.Accepted && ResultDiagnosticValue != SourceDiagnostic.None)
                    {
                        return true;
                    }
                    GXUserInformation.EncodeData(settings, buff);
                }
                else if (tag == 0xA2) //Result
                {
                    tag = buff.GetUInt8();
                    len = buff.GetUInt8();
                    //Choice for result (INTEGER, universal)
                    tag = buff.GetUInt8();
                    len = buff.GetUInt8();
                    this.ResultComponent = (AssociationResult)buff.GetUInt8();
                }
                else if (tag == 0xA3) //SourceDiagnostic
                {
                    tag = buff.GetUInt8();
                    len = buff.GetUInt8();
                    // ACSE service user tag.
                    tag = buff.GetUInt8();
                    len = buff.GetUInt8();
                    // Result source diagnostic component.
                    tag = buff.GetUInt8();
                    len = buff.GetUInt8();
                    ResultDiagnosticValue = (SourceDiagnostic)buff.GetUInt8();
                }
                else if (tag == 0x8A || tag == 0x88) //Authentication.
                {
                    tag = buff.GetUInt8();
                    //Get sender ACSE-requirements field component.
                    if (buff.GetUInt8() != 2)
                    {
                        throw new Exception("Invalid tag.");
                    }
                    int val = buff.GetUInt16();
                    if (val != 0x0780 && val != 0x0680)
                    {
                        throw new Exception("Invalid tag.");
                    }
                }               
                else if (tag == 0xAA) //Server Challenge.                
                {
                    tag = buff.GetUInt8();
                    len = buff.GetUInt8();
                    ++buff.Position;
                    len = buff.GetUInt8();
                    //Get challenge and save it to the PW.
                    settings.StoCChallenge = new byte[len];
                    buff.Get(settings.StoCChallenge);
                }
                else if (tag == 0x8B || tag == 0x89) //Authentication.
                {
                    tag = buff.GetUInt8();
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
                    if (tmp != 0)
                    {
                        byte tag2 = buff.GetUInt8();
                        if (tag2 != 0xAC && tag2 != 0xAA)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        len = buff.GetUInt8();
                        // Get authentication information.
                        if ((buff.GetUInt8() & 0xFF) != 0x80)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        len = buff.GetUInt8() & 0xFF;
                        byte[] tmp2 = new byte[len];
                        buff.Get(tmp2);
                        if (tmp < 2)
                        {
                            settings.Password = tmp2;
                        }
                        else
                        {
                            if (settings.IsServer)
                            {
                                settings.CtoSChallenge = tmp2;
                            }
                            else
                            {
                                settings.StoCChallenge = tmp2;
                            }
                        }
                    }
                }
                //Unknown tags.
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unknown tag.");
                    tag = buff.GetUInt8();
                    len = buff.GetUInt8();
                    buff.Position += (UInt16)len;
                }
            }
            return true;
        }       

        ///<summary>
        ///Server generates AARE message.
        ///</summary>
        internal void GenerateAARE(GXDLMSSettings settings, GXByteBuffer data, 
            AssociationResult result, SourceDiagnostic diagnostic, bool ciphering)
        {
            int offset = data.Position;
            // Set AARE tag and length
            data.SetUInt8(0x61);
            // Length is updated later.
            data.SetUInt8(0);
            GXApplicationContextName.CodeData(settings, data, ciphering);
            //Result
            data.SetUInt8(0xA2);
            data.SetUInt8(3); //len
            data.SetUInt8(2); //Tag
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
                data.SetUInt8(0x80);
                data.SetUInt8((byte)settings.StoCChallenge.Length);
                data.Set(settings.StoCChallenge);
            }            
            //Add User Information
            data.SetUInt8(0xBE); //Tag
            data.SetUInt8(0x11); //Length for AARQ user field
            data.SetUInt8(0x04); //Coding the choice for user-information (Octet STRING, universal)
            data.SetUInt8(0xF); //Length
            data.SetUInt8(GXCommon.InitialResponce); // Tag for xDLMS-Initiate response
            data.SetUInt8(0x01);
            data.SetUInt8(0x00); // Usage field for the response allowed component (not used)
            data.SetUInt8(6); // DLMSVersioNumber
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
            data.SetUInt8((UInt16)(offset + 1), (byte) (data.Size - offset - 2));
        }
    }
}
