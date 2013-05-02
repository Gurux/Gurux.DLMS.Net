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
    class GXUserInformation
    {
        internal byte DLMSVersioNumber;
        internal ushort MaxReceivePDUSize;

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal byte[] ConformanceBlock = new byte[3];

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXUserInformation()
        {
            MaxReceivePDUSize = 0xFFFF;
            DLMSVersioNumber = 6;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="data"></param>
        internal void CodeData(List<byte> data)
        {
            data.Add(0xBE); //Tag
            data.Add(0x10); //Length for AARQ user field
            data.Add(0x04); //Coding the choice for user-information (Octet STRING, universal)
            data.Add(0x0E); //Length
            data.Add(GXCommon.InitialRequest); // Tag for xDLMS-Initiate request
            data.Add(0x00); // Usage field for dedicated-key component � not used
            data.Add(0x00); // Usage field for the response allowed component � not used
            data.Add(0x00); // Usage field of the proposed-quality-of-service component � not used
            data.Add(DLMSVersioNumber); // Tag for conformance block
            data.Add(0x5F);
            data.Add(0x1F);
            data.Add(0x04);// length of the conformance block
            data.Add(0x00);// encoding the number of unused bits in the bit string
            data.AddRange(ConformanceBlock);
            GXCommon.SetUInt16(MaxReceivePDUSize, data);
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal void EncodeData(byte[] data, ref int index)
        {
            int tag = data[index++];
            if (tag != 0xBE)
            {
                throw new Exception("Invalid tag.");
            }
            int len = data[index++];
            if (data.Length - index < len)
            {
                throw new Exception("Not enought data.");
            }
            //Excoding the choice for user information
            tag = data[index++];
            if (tag != 0x4)
            {
                throw new Exception("Invalid tag.");
            }
            len = data[index++];
            //Tag for xDLMS-Initate.response
            tag = data[index++];
            bool response = tag == GXCommon.InitialResponce;
            if (response)
            {
                //Optional usage field of the negotiated quality of service component
                tag = data[index++];
                if (tag != 0)//Skip if used.
                {
                    len = data[index++];
                    index += len;
                }
            }
            else if (tag == GXCommon.InitialRequest)
            {
                //Optional usage field of the negotiated quality of service component
                tag = data[index++];
                if (tag != 0)//Skip if used.
                {
                    len = data[index++];
                    index += len;
                }
                //Optional usage field of the negotiated quality of service component
                tag = data[index++];
                if (tag != 0)//Skip if used.
                {
                    len = data[index++];
                    index += len;
                }
                //Optional usage field of the negotiated quality of service component
                tag = data[index++];
                if (tag != 0)//Skip if used.
                {
                    len = data[index++];
                    index += len;
                }
            }
            else
            {
                throw new Exception("Invalid tag.");
            }
            //Get DLMS version number.
            DLMSVersioNumber = data[index++];
            //Tag for conformance block
            tag = data[index++];
            if (tag != 0x5F)
            {
                throw new Exception("Invalid tag.");
            }
            //Old Way...
            if (data[index] == 0x1F)
            {
                ++index;
            }
            len = data[index++];
            //The number of unused bits in the bit string.
            tag = data[index++];
            Array.Copy(data, index, ConformanceBlock, 0, 3);
            index += 3;
            MaxReceivePDUSize = GXCommon.GetUInt16(data, ref index);
            if (response)
            {
                //VAA Name
                tag = data[index++];
                tag = data[index++];
            }
        }
    }
}
