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
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{
    /// <summary>
    /// Reserved for internal use. 
    /// </summary>
    sealed class GXUserInformation
    {
        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="data"></param>
        static internal void CodeData(GXDLMSSettings settings, GXByteBuffer data)
        {
            data.SetUInt8(0xBE); //Tag
            data.SetUInt8(0x10); //Length for AARQ user field
            data.SetUInt8(0x04); //Coding the choice for user-information (Octet STRING, universal)
            data.SetUInt8(0x0E); //Length
            data.SetUInt8(GXCommon.InitialRequest); // Tag for xDLMS-Initiate request
            data.SetUInt8(0x00); // Usage field for dedicated-key component. Not used
            data.SetUInt8(0x00); // Usage field for the response allowed component. Not used
            data.SetUInt8(0x00); // Usage field of the proposed-quality-of-service component. Not used
            data.SetUInt8(settings.DLMSVersion); // Tag for conformance block
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
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal static void EncodeData(GXDLMSSettings settings, GXByteBuffer data)
        {
            int tag = data.GetUInt8();
            if (tag != 0xBE)
            {
                throw new Exception("Invalid tag.");
            }
            byte len = data.GetUInt8();
            if (data.Size - data.Position < len)
            {
                throw new Exception("Not enough data.");
            }
            //Excoding the choice for user information
            tag = data.GetUInt8();
            if (tag != 0x4)
            {
                throw new Exception("Invalid tag.");
            }
            len = data.GetUInt8();
            //Tag for xDLMS-Initate.response
            tag = data.GetUInt8();
            bool response = tag == GXCommon.InitialResponce;
            if (response)
            {
                //Optional usage field of the negotiated quality of service component
                tag = data.GetUInt8();
                if (tag != 0)//Skip if used.
                {
                    len = data.GetUInt8();
                    data.Position += len;
                }
            }
            else if (tag == GXCommon.InitialRequest)
            {
                //Optional usage field of the negotiated quality of service component
                tag = data.GetUInt8();
                if (tag != 0)//Skip if used.
                {
                    len = data.GetUInt8();
                    data.Position += len;
                }
                //Optional usage field of the negotiated quality of service component
                tag = data.GetUInt8();
                if (tag != 0)//Skip if used.
                {
                    len = data.GetUInt8();
                    data.Position += len;
                }
                //Optional usage field of the negotiated quality of service component
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
            if (settings.IsServer)
            {
                settings.DlmsVersionNumber = data.GetUInt8();
            }
            else
            {
                if (data.GetUInt8() != 6)
                {
                    throw new Exception("Invalid DLMS version number.");
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
            if (settings.UseLogicalNameReferencing)
            {
                if (settings.LnSettings == null)
                {
                    settings.LnSettings = new GXDLMSLNSettings();
                }
                if (settings.IsServer)
                {
                    //Skip settings what client asks.
                    //All server settings are always returned.
                    byte[] tmp = new byte[3];
                    data.Get(tmp);
                }
                else
                {
                    data.Get(settings.LnSettings.ConformanceBlock);
                }
            }
            else
            {
                if (settings.SnSettings == null)
                {
                    settings.SnSettings = new GXDLMSSNSettings();
                }
                if (settings.IsServer)
                {
                    //Skip settings what client asks.
                    //All server settings are always returned.
                    byte[] tmp = new byte[3];
                    data.Get(tmp);
                }
                else
                {
                    data.Get(settings.SnSettings.ConformanceBlock);
                }
            }
            if (settings.IsServer)
            {
                data.GetUInt16();
            }
            else
            {
                settings.MaxReceivePDUSize = data.GetUInt16();
            }
            if (response)
            {
                //VAA Name
                tag = data.GetUInt16();
            }
        }
    }
}
