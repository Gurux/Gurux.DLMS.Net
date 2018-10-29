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
using System.ComponentModel;
using Gurux.DLMS.Objects;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Enums;
using System.Xml.Serialization;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMS implements methods to communicate with DLMS/COSEM metering devices.
    /// </summary>
    public class GXDLMSMeter
    {
        /// <summary>
        /// Define how long reply is waited in seconds.
        /// </summary>
        [DefaultValue(5)]
        public int WaitTime
        {
            get;
            set;
        }

        /// <summary>
        /// Define re-send count.
        /// </summary>
        [DefaultValue(3)]
        public int ResendCount
        {
            get;
            set;
        }

        
        /// <summary>
        /// Maximum used baud rate.
        /// </summary>
        [DefaultValue(0)]
        public int MaximumBaudRate
        {
            get;
            set;
        }

        /// <summary>
        /// Used authentication.
        /// </summary>
        [DefaultValue(Authentication.None)]
        public Authentication Authentication
        {
            get;
            set;
        }

        /// <summary>
        /// Used standard.
        /// </summary>
        [DefaultValue(Standard.DLMS)]
        public Standard Standard
        {
            get;
            set;
        }

        /// <summary>
        /// Password is used only if authentication is used.
        /// </summary>
        [DefaultValue(null)]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Password is used only if authentication is used.
        /// </summary>
        [DefaultValue(null)]
        public byte[] HexPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Used communication security.
        /// </summary>
        [DefaultValue(Security.None)]
        public Security Security
        {
            get;
            set;
        }

        /// <summary>
        /// System Title.
        /// </summary>
        [DefaultValue(null)]
        public string SystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Server System Title.
        /// </summary>
        [DefaultValue(null)]
        public string ServerSystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Dedicated Key.
        /// </summary>
        [DefaultValue(null)]
        public string DedicatedKey
        {
            get;
            set;
        }

        /// <summary>
        /// Use pre-established application associations.
        /// </summary>
        [DefaultValue(false)]
        public bool PreEstablished
        {
            get;
            set;
        }

        /// <summary>
        /// Block cipher key.
        /// </summary>
        [DefaultValue(null)]
        public string BlockCipherKey
        {
            get;
            set;
        }

        /// <summary>
        /// Authentication key.
        /// </summary>
        [DefaultValue(null)]
        public string AuthenticationKey
        {
            get;
            set;
        }

        /// <summary>
        /// Invocation counter.
        /// </summary>
        [DefaultValue(0)]
        public UInt32 InvocationCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Frame counter is used to update InvocationCounter automatically.
        /// </summary>
        [DefaultValue(null)]
        public string FrameCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Static challenge.
        /// </summary>
        [DefaultValue(null)]
        public string Challenge
        {
            get;
            set;
        }

        /// <summary>
        /// Used Physical address.
        /// </summary>
        /// <remarks>
        /// Server HDLC Address (Logical + Physical address)  might be 1,2 or 4 bytes long.
        /// </remarks>
        [DefaultValue(null)]
        public object PhysicalAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Used logical address.
        /// </summary>
        [DefaultValue(0)]
        public int LogicalAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Standard says that Time zone is from normal time to UTC in minutes.
        /// If meter is configured to use UTC time (UTC to normal time) set this to true.
        /// </summary>
        [DefaultValue(false)]
        public bool UtcTimeZone
        {
            get;
            set;
        }

        /// <summary>
        /// USed logical client ID.
        /// </summary>
        /// <remarks>
        /// Client ID is always 1 byte long.
        /// </remarks>
        [DefaultValue(0x10)]
        public int ClientAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Is IEC 62056-21 skipped when using serial port connection.
        /// </summary>
        [DefaultValue(StartProtocolType.IEC)]
        public StartProtocolType StartProtocol
        {
            get;
            set;
        }

        /// <summary>
        /// Is serial port access through TCP/IP or UDP converter.
        /// </summary>
        [DefaultValue(false)]
        public bool UseRemoteSerial
        {
            get;
            set;
        }

        /// <summary>
        /// Used interface type.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(InterfaceType.HDLC)]
        public InterfaceType InterfaceType
        {
            get;
            set;
        }

        /// <summary>
        /// Is MaxInfoTX and RX count for frame size or PDU size.
        /// </summary>
        [DefaultValue(false)]
        public bool UseFrameSize
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum information field length in transmit.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 128. Minimum value is 32 and max value is 2030.
        /// </remarks>
        [DefaultValue(128)]
        public UInt16 MaxInfoTX
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum information field length in receive.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 128. Minimum value is 32 and max value is 2030.
        /// </remarks>
        [DefaultValue(128)]
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
        [DefaultValue(1)]
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
        [DefaultValue(1)]
        public byte WindowSizeRX
        {
            get;
            set;
        }


        /// <summary>
        /// Proposed maximum size of PDU.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 0xFFFF.
        /// </remarks>
        [DefaultValue(0xFFFF)]
        public UInt16 PduSize
        {
            get;
            set;
        }

        /// <summary>
        /// User Id.
        /// </summary>
        /// <remarks>
        /// In default user id is not used.
        /// </remarks>
        [DefaultValue(-1)]
        public short UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Network ID.
        /// </summary>
        [DefaultValue(0)]
        public byte NetworkId
        {
            get;
            set;
        }

        /// <summary>
        /// Physical device address.
        /// </summary>
        [DefaultValue(null)]
        public string PhysicalDeviceAddress
        {
            get;
            set;
        }

        /// <summary>
        ///Inactivity timeout.
        /// </summary>
        public int InactivityTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Used Service class.
        /// </summary>
        [DefaultValue(ServiceClass.Confirmed)]
        public ServiceClass ServiceClass
        {
            get;
            set;
        }

        /// <summary>
        /// Used priority.
        /// </summary>
        [DefaultValue(Priority.High)]
        public Priority Priority
        {
            get;
            set;
        }


        /// <summary>
        /// Server address size.
        /// </summary>
        /// <remarks>
        /// This is not udes in default. Some meters require that server address size is constant.
        /// </remarks>
        [DefaultValue(0)]
        public byte ServerAddressSize
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the meter.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Is media verbose mode used.
        /// </summary>
        [DefaultValue(false)]
        public bool Verbose
        {
            get;
            set;
        }

        /// <summary>
        /// Used Conformance.
        /// </summary>
        public int Conformance
        {
            get;
            set;
        }
      
        /// <summary>
        /// Name of the manufacturer.
        /// </summary>
        public string Manufacturer
        {
            get;
            set;
        }

        /// <summary>
        /// What HDLC Addressing is used.
        /// </summary>
        public HDLCAddressType HDLCAddressing
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSMeter()
        {
            StartProtocol = StartProtocolType.IEC;
            Standard = Standard.DLMS;
            ClientAddress = 0x10; // Public client (lowest security level).
            PhysicalAddress = 1;
            Password = null;
            Authentication = Authentication.None;
            WaitTime = 5;
            ResendCount = 3;
            InactivityTimeout = 120;
            WindowSizeRX = WindowSizeTX = 1;
            MaxInfoRX = MaxInfoTX = 128;
            PduSize = 0xFFFF;
            ServiceClass = ServiceClass.Confirmed;
            Priority = Priority.High;
            UserId = -1;
            Objects = new GXDLMSObjectCollection(this);
        }

        /// <summary>
        /// Media type.
        /// </summary>
        public virtual string MediaType
        {
            get;
            set;
        }

        /// <summary>
        /// Media settings as a string.
        /// </summary>
        public string MediaSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Is Logical name referencing used.
        /// </summary>
        [XmlElement("UseLN")]
        public bool UseLogicalNameReferencing
        {
            get;
            set;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [XmlArray("Objects2")]
        public GXDLMSObjectCollection Objects
        {
            get;
            set;
        }
    }
}