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
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMSLimits contains commands for retrieving and setting the limits of
    /// field length and window size, when communicating with the server.
    /// </summary>
    public class GXDLMSLimits
    {
        private GXDLMSSettings settings;

        internal GXDLMSLimits(GXDLMSSettings s)
        {
            settings = s;
            MaxInfoRX = GXDLMSLimitsDefault.DefaultMaxInfoRX;
            MaxInfoTX = GXDLMSLimitsDefault.DefaultMaxInfoTX;
            WindowSizeRX = GXDLMSLimitsDefault.DefaultWindowSizeRX;
            WindowSizeTX = GXDLMSLimitsDefault.DefaultWindowSizeTX;
        }

        /// <summary>
        /// Is Max Info TX and RX count for frame size or PDU size.
        /// </summary>
        public bool UseFrameSize
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum information field length in transmit.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 128. Minimum value is 32 and max value is 128.
        /// </remarks>
        public UInt16 MaxInfoTX
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum information field length in receive.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 128. Minimum value is 32 and max value is 128.
        /// </remarks>
        public UInt16 MaxInfoRX
        {
            get;
            set;
        }

        /// <summary>
        /// The window size in transmit.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 1.
        /// </remarks>
        public byte WindowSizeTX
        {
            get;
            set;
        }

        /// <summary>
        /// The window size in receive.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 1.
        /// </remarks>
        public byte WindowSizeRX
        {
            get;
            set;
        }

        /// <summary>
        ///  HDLC sender frame sequence number.
        /// </summary>
        public byte SenderFrame
        {
            get
            {
                return settings.SenderFrame;
            }
            set
            {
                settings.SenderFrame = value;
            }
        }

        /// <summary>
        /// HDLC receiver frame sequence number.
        /// </summary>
        public byte ReceiverFrame
        {
            get
            {
                return settings.ReceiverFrame;
            }
            set
            {
                settings.ReceiverFrame = value;
            }
        }
    }
}