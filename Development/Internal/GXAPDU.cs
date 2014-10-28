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
        public byte[] Password;
        public Authentication Authentication;
        GXApplicationContextName ApplicationContextName = new GXApplicationContextName();
        AssociationResult ResultValue;
        GXDLMSTagCollection Tags;
        internal GXUserInformation UserInformation = new GXUserInformation();

        ///<summary>
        ///Constructor.
        ///</summary>
        public GXAPDU(GXDLMSTagCollection tags)
        {
            this.Authentication = Authentication.None;
            Tags = tags;
        }

        ///<summary>
        ///UseLN
        ///</summary>
        public bool UseLN
        {
            get
            {
                return ApplicationContextName.UseLN;
            }
            set
            {
                ApplicationContextName.UseLN = value;
            }
        }

        ///<summary>
        ///AssociationResult
        ///</summary>
        internal AssociationResult ResultComponent
        {
            get
            {
                return ResultValue;
            }
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
        ///Determines the authentication level, and password, if used.
        ///</summary>
        internal void SetAuthentication(Authentication val, byte[] pw)
        {
            Authentication = val;
            Password = pw;
        }

        ///<summary>
        ///Retrieves the string that indicates the level of authentication, if any. 
        ///</summary>
        internal void GetAuthenticationString(List<byte> data, byte[] challenge)
        {
            //If low authentication is used.
            if (this.Authentication != Authentication.None)
            {
                //Add sender ACSE-requirenents field component.
                data.Add(0x8A);
                data.Add(2);
                GXCommon.SetUInt16(0x0780, data);
                data.Add(0x8B);
                data.Add(7);
                byte[] p = { (byte)0x60, (byte)0x85, (byte)0x74, (byte)0x05, (byte)0x08, (byte)0x02, (byte)this.Authentication };
                data.AddRange(p);
                //Add Calling authentication information.
                int len = 0;
                if (this.Authentication == Authentication.Low || this.Authentication == Authentication.High)
                {
                    if (Password != null)
                    {
                        len = Password.Length;
                    }
                }
                else
                {                    
                    len = challenge.Length;
                }
                data.Add(0xAC);
                data.Add((byte)(2 + len));
                //Add authentication information.
                data.Add((byte)0x80);
                data.Add((byte)len);
                if (challenge != null)
                {
                    data.AddRange(challenge);
                }
                else if (Password != null)
                {
                    data.AddRange(Password);
                }
            }
        }

        ///<summary>
        ///CodeData
        ///</summary>
        internal void CodeData(List<byte> data, InterfaceType interfaceType, byte[] challenge)
        {
            //AARQ APDU Tag
            data.Add(GXCommon.AARQTag);
            //Length
            int LenPos = data.Count;
            data.Add(0);
            ///////////////////////////////////////////
            // Add Application context name.
            ApplicationContextName.CodeData(data);
            GetAuthenticationString(data, challenge);            
            UserInformation.CodeData(data);
            //Add extra tags...
            if (Tags != null)
            {
                for (int a = 0; a < Tags.Count; ++a)
                {
                    GXDLMSTag tag = Tags[a];
                    if (tag != null)
                    {
                        //Add data ID.
                        data.Add((byte)tag.ID);
                        //Add data len.
                        data.Add((byte)tag.Data.Length);
                        //Add data.
                        data.AddRange(tag.Data);
                    }
                }
            }
            data[LenPos] = (byte)(data.Count() - LenPos - 1);
        }

        ///<summary>
        ///EncodeData
        ///</summary>
        internal void EncodeData(byte[] buff, ref int index)
        {
            // Get AARE tag and length
            int tag = buff[index++];
            if (tag != 0x61 && tag != 0x60 && tag != 0x81 && tag != 0x80)
            {
                throw new Exception("Invalid tag.");
            }
            int len = buff[index++];
            int size = buff.Length - index;
            if (len > size)
            {
                throw new Exception("Not enough data.");
            }
            while (index < buff.Length)
            {
                tag = buff[index];
                if (tag == 0xA1)
                {
                    ApplicationContextName.EncodeData(buff, ref index);
                }
                else if (tag == 0xBE)
                {
                    if (ResultValue != AssociationResult.Accepted && ResultDiagnosticValue != SourceDiagnostic.None)
                    {
                        return;
                    }
                    UserInformation.EncodeData(buff, ref index);
                }
                else if (tag == 0xA2) //Result
                {
                    tag = buff[index++];
                    len = buff[index++];
                    //Choice for result (INTEGER, universal)
                    tag = buff[index++];
                    len = buff[index++];
                    ResultValue = (AssociationResult)buff[index++];
                }
                else if (tag == 0xA3) //SourceDiagnostic
                {
                    tag = buff[index++];
                    len = buff[index++];
                    // ACSE service user tag.
                    tag = buff[index++];
                    len = buff[index++];
                    // Result source diagnostic component.
                    tag = buff[index++];
                    len = buff[index++];
                    ResultDiagnosticValue = (SourceDiagnostic)buff[index++];
                }
                else if (tag == 0x8A || tag == 0x88) //Authentication.
                {
                    tag = buff[index++];
                    //Get sender ACSE-requirenents field component.
                    if (buff[index++] != 2)
                    {
                        throw new Exception("Invalid tag.");
                    }
                    int val = GXCommon.GetUInt16(buff, ref index);
                    if (val != 0x0780 && val != 0x0680)
                    {
                        throw new Exception("Invalid tag.");
                    }
                }               
                else if (tag == 0xAA) //Server Challenge.                
                {
                    tag = buff[index++];
                    len = buff[index++];
                    ++index;
                    len = buff[index++];
                    //Get challenge
                    index += len;
                }
                else if (tag == 0x8B || tag == 0x89) //Authentication.
                {
                    tag = buff[index++];
                    len = buff[index++];
                    bool IsAuthenticationTag = len > 7;
                    if (buff[index++] != 0x60)
                    {
                        throw new Exception("Invalid tag.");
                    }
                    if (buff[index++] != 0x85)
                    {
                        throw new Exception("Invalid tag.");
                    }
                    if (buff[index++] != 0x74)
                    {
                        throw new Exception("Invalid tag.");
                    }
                    if (buff[index++] != 0x05)
                    {
                        throw new Exception("Invalid tag.");
                    }
                    if (buff[index++] != 0x08)
                    {
                        throw new Exception("Invalid tag.");
                    }
                    if (buff[index++] != 0x02)
                    {
                        throw new Exception("Invalid tag.");
                    }
                    int tmp = buff[index++];
                    if (tmp < 0 || tmp > 5)
                    {
                        throw new Exception("Invalid tag.");
                    }
                    if (IsAuthenticationTag)
                    {
                        Authentication = (Authentication)tmp;
                        byte tag2 = buff[index++];
                        if (tag2 != 0xAC && tag2 != 0xAA)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        len = buff[index++];
                        //Get authentication information.
                        if (buff[index++] != 0x80)
                        {
                            throw new Exception("Invalid tag.");
                        }
                        len = buff[index++];
                        Password = new byte[len];
                        Array.Copy(buff, index, Password, 0, len);
                        index += len;
                    }
                    else
                    {
                        Authentication = Authentication.None;
                    }
                }
                //Unknown tags.
                else
                {
                    tag = buff[index++];
                    len = buff[index++];
                    if (Tags != null)
                    {
                        GXDLMSTag tmp = new GXDLMSTag();
                        tmp.ID = tag;
                        tmp.Data = new byte[len];
                        tmp.Data = GXCommon.Swap(buff, index, len);
                        Tags.Add(tmp);
                    }
                    index += len;
                }
            }
        }       

        ///<summary>
        ///Server generates AARE message.
        ///</summary>
        internal void GenerateAARE(List<byte> data, Authentication authentication, byte[] challenge, ushort maxReceivePDUSize, byte[] conformanceBlock, AssociationResult result, SourceDiagnostic diagnostic)
        {
            // Set AARE tag and length
            data.Add(0x61);
            ApplicationContextName.CodeData(data);
            //Result
            data.Add(0xA2);
            data.Add(3); //len
            data.Add(2); //Tag
            //Choice for result (INTEGER, universal)
            data.Add(1); //Len
            data.Add((byte) result); //ResultValue            
            //SourceDiagnostic
            data.Add(0xA3);
            data.Add(5); //len
            data.Add(0xA1); //Tag
            data.Add(3); //len
            data.Add(2); //Tag
            //Choice for result (INTEGER, universal)
            data.Add(1); //Len
            data.Add((byte)diagnostic); //diagnostic            
            if (diagnostic == SourceDiagnostic.AuthenticationRequired)
            {                
                //Add server ACSE-requirenents field component.
                data.Add(0x88);
                data.Add(0x02);  //Len.
                GXCommon.SetUInt16(0x0780, data);
                //Add tag.
                data.Add(0x89);
                data.Add(0x07);//Len
                data.Add(0x60);
                data.Add(0x85);
                data.Add(0x74);
                data.Add(0x05);
                data.Add(0x08);
                data.Add(0x02);
                data.Add((byte) authentication);
                //Add tag.
                data.Add(0xAA);
                data.Add((byte) (2 + challenge.Length));//Len
                data.Add(0x80);
                data.Add((byte) challenge.Length);
                data.AddRange(challenge);
            }            
            //Add User Information
            data.Add(0xBE); //Tag
            data.Add(0x10); //Length for AARQ user field
            data.Add(0x04); //Coding the choice for user-information (Octet STRING, universal)
            data.Add(0xE); //Length
            data.Add(GXCommon.InitialResponce); // Tag for xDLMS-Initiate response
            data.Add(0x00); // Usage field for the response allowed component (not used)
            data.Add(6); // DLMSVersioNumber
            data.Add(0x5F);
            data.Add(0x1F);
            data.Add(0x04);// length of the conformance block
            data.Add(0x00);// encoding the number of unused bits in the bit string            
            data.AddRange(conformanceBlock);
            GXCommon.SetUInt16(maxReceivePDUSize, data);
            //VAA Name VAA name (0x0007 for LN referencing and 0xFA00 for SN)
            if (UseLN)
            {
                GXCommon.SetUInt16(0x0007, data);
            }
            else
            {
                GXCommon.SetUInt16(0xFA00, data);
            }            
            data.Insert(1, (byte) (data.Count - 1));
        }
    }
}
