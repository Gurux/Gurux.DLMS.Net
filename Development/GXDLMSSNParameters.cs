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

namespace Gurux.DLMS
{
    /// <summary>
    /// SN Parameters
    /// </summary> 
    internal class GXDLMSSNParameters
    {
        /// <summary>
        /// DLMS settings.
        /// </summary>
        public GXDLMSSettings settings;
        /// <summary>
        /// DLMS Command.
        /// </summary>
        public Command command;
        /// <summary>
        /// Request type.
        /// </summary>
        public byte requestType;
        /// <summary>
        /// Attribute descriptor.
        /// </summary>
        public GXByteBuffer attributeDescriptor;
        /// <summary>
        /// Data.
        /// </summary>
        public GXByteBuffer data;
        /// <summary>
        /// Send date and time. This is used in Data notification messages.
        /// </summary>
        public DateTime time;
        /// <summary>
        /// Item Count.
        /// </summary>
        public int count;
        
        /// <summary>
        /// Are there more data to send or more data to receive.
        /// </summary>
        public bool multipleBlocks;
        
        /// <summary>
        /// Block index.
        /// </summary>
        public UInt16 blockIndex;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="forSettings">DLMS settings.</param>
        /// <param name="forCommand">Command.</param>
        /// <param name="forCommandType">Command type.</param>
        /// <param name="forAttributeDescriptor"></param>
        /// <param name="forData">Attribute descriptor</param>
        /// <returns>Generated messages.</returns>
        internal GXDLMSSNParameters(GXDLMSSettings forSettings, Command forCommand,  int forCount, 
            byte forCommandType,
           GXByteBuffer forAttributeDescriptor, GXByteBuffer forData)
        {
            settings = forSettings;
            blockIndex = (UInt16)settings.BlockIndex;
            command = forCommand;
            count = forCount;
            requestType = forCommandType;
            attributeDescriptor = forAttributeDescriptor;
            data = forData;
            time = DateTime.MinValue;
            multipleBlocks = false;
        }
    }
}
