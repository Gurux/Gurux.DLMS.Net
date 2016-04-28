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


using System.Text;
using Gurux.DLMS.Enums;
using System;
namespace Gurux.DLMS.Secure
{
    public class GXDLMSSecureNotify : GXDLMSNotify
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="useLogicalNameReferencing">Is Logical or short name referencing used.</param>
        /// <param name="clientAddress">Client address. Default is 16 (0x10)</param>
        /// <param name="serverAddress">Server ID. Default is 1.</param>
        /// <param name="interfaceType">Interface type. Default is general.</param>
        public GXDLMSSecureNotify(bool useLogicalNameReferencing,
            int clientAddress, int serverAddress, InterfaceType interfaceType)
            : base(useLogicalNameReferencing,
                clientAddress, serverAddress, interfaceType)
        {
            Ciphering = new GXCiphering(ASCIIEncoding.ASCII.GetBytes("ABCDEFGH"));
            Settings.Cipher = Ciphering;
        }

        /// <summary>
        /// Ciphering settings.
        /// </summary>
        public GXCiphering Ciphering
        {
            get;
            private set;
        }
    }
}
