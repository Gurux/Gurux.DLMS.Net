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

using System;

namespace Gurux.DLMS
{
    /// <summary>
    /// LN Parameters
    /// </summary>
    internal class GXDLMSLNParameters
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
        ///Received Ciphered command.
        /// </summary>
        public Command cipheredCommand;
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
        public GXDateTime time;
        /// <summary>
        /// Reply status.
        /// </summary>
        public byte status;
        /// <summary>
        /// Are there more data to send or more data to receive.
        /// </summary>
        public bool multipleBlocks;
        /// <summary>
        /// Is this last block in send.
        /// </summary>
        public bool lastBlock;
        /// <summary>
        /// Block index.
        /// </summary>
        public uint blockIndex;

        /// <summary>
        /// Block number ack.
        /// </summary>
        public UInt16 blockNumberAck;
        ///<summary>
        /// Received invoke ID.
        ///</summary>
        public UInt32 InvokeId;

        ///<summary>
        /// GBT window size.
        ///</summary>
        public byte WindowSize;

        ///<summary>
        /// Is GBT streaming used.
        ///</summary>
        public bool Streaming;

        public GXDLMSClient Owner
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner">Owner component.</param>
        /// <param name="forSettings">DLMS settings.</param>
        /// <param name="forCommand">Command.</param>
        /// <param name="forCommandType">Command type.</param>
        /// <param name="forAttributeDescriptor">Attribute descriptor,</param>
        /// <param name="forData">Data,</param>
        public GXDLMSLNParameters(GXDLMSClient owner, GXDLMSSettings forSettings, UInt32 invokeId, Command forCommand, byte forCommandType,
           GXByteBuffer forAttributeDescriptor, GXByteBuffer forData, byte forStatus, Command forCipheredCommand)
        {
            Owner = owner;
            settings = forSettings;
            InvokeId = invokeId;
            blockIndex = settings.BlockIndex;
            blockNumberAck = settings.BlockNumberAck;
            command = forCommand;
            cipheredCommand = forCipheredCommand;
            requestType = forCommandType;
            attributeDescriptor = forAttributeDescriptor;
            data = forData;
            time = null;
            status = forStatus;
            multipleBlocks = settings.ForceToBlocks || forSettings.Count != forSettings.Index;
            lastBlock = forSettings.Count == forSettings.Index;
            WindowSize = 1;
        }
    }
}
