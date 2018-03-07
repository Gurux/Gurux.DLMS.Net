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
using System.Text;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSImageActivateInfo
    {
        /// <summary>
        /// Image size is the size of the Image(s) to be activated. Expressed in octets;
        /// </summary>
        public ulong Size
        {
            get;
            set;
        }

        /// <summary>
        /// Image identification is the identification of the Image(s) 
        /// to be activated, and may contain information like
        /// manufacturer, device type, version information, etc.
        /// </summary>
        public byte[] Identification
        {
            get;
            set;
        }

        /// <summary>
        /// Image signature is the signature of the Image(s) to be activated.
        /// </summary>
        public byte[] Signature
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (GXByteBuffer.IsAsciiString(Identification))
            {
                sb.Append(ASCIIEncoding.ASCII.GetString(Identification));
            }
            else
            {
                sb.Append(Internal.GXCommon.ToHex(Identification, false));
            }
            sb.Append(" ");
            if (GXByteBuffer.IsAsciiString(Signature))
            {
                sb.Append(ASCIIEncoding.ASCII.GetString(Signature));
            }
            else
            {
                sb.Append(Internal.GXCommon.ToHex(Signature, false));
            }
            sb.Append(" ");
            sb.Append(Convert.ToString(Size));
            return sb.ToString();
        }
    }
}
