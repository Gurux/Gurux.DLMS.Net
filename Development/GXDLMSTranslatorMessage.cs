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
using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{
    /// <summary>
    /// XML translator message detailed data.
    /// </summary>
    public class GXDLMSTranslatorMessage
    {
        /// <summary>
        /// Message to convert to XML.
        /// </summary>
        public GXByteBuffer Message
        {
            get;
            set;
        }

        /// <summary>
        /// Converted XML.
        /// </summary>
        public string Xml
        {
            get;
            set;
        }

        /// <summary>
        /// Executed Command.
        /// </summary>
        public Command Command
        {
            get;
            set;
        }

        /// <summary>
        /// System title from AARQ or AARE messages.
        /// </summary>
        public byte[] SystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Dedicated key from AARQ messages.
        /// </summary>
        public byte[] DedicatedKey
        {
            get;
            set;
        }

        //Interface type.
        public InterfaceType InterfaceType
        {
            get;
            set;
        }

    }
}