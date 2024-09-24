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
using System.ComponentModel;
using Gurux.DLMS.Objects;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Enums;
using System.Xml.Serialization;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMSMeterBase implements properties to communicate with DLMS/COSEM metering devices.
    /// </summary>
    public class GXDLMSMeterBase
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
        /// Authentication Level.
        /// </summary>
        [DefaultValue(Authentication.None)]
        [Description("Authentication Level.")]
        public Authentication Authentication
        {
            get;
            set;
        }

        /// <summary>
        /// Authentication Level.
        /// </summary>
        [Description("Name of authentication level.")]
        public string AuthenticationName
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
        /// Used Security Suite.
        /// </summary>
        [DefaultValue(Objects.Enums.SecuritySuite.Suite0)]
        public SecuritySuite SecuritySuite
        {
            get;
            set;
        }


        /// <summary>
        /// Used Key agreement scheme.
        /// </summary>
        /// Obsolete. This is removed at some point.
        public KeyAgreementScheme KeyAgreementScheme
        {
            get;
            set;
        }

        /// <summary>
        /// System Title.
        /// </summary>
        [DefaultValue(null)]
        [Description("Client system title.")]
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
        /// <seealso cref="IgnoreSNRMWithPreEstablished"/>
        [DefaultValue(false)]
        public bool PreEstablished
        {
            get;
            set;
        }

        /// <summary>
        /// SNRM command is not send With pre-established connection.
        /// </summary>
        /// <remarks>
        /// DLMS standard defines that SNRM message is sent with pre-established connections, 
        /// but there are some meters that don't follow the standard.
        /// </remarks>
        /// <seealso cref="PreEstablished"/>
        [DefaultValue(false)]
        public bool IgnoreSNRMWithPreEstablished
        {
            get;
            set;
        }

        /// <summary>
        /// Block cipher key.
        /// </summary>
        [DefaultValue(null)]
        [Description("Block cipher key.")]
        public virtual string BlockCipherKey
        {
            get;
            set;
        }

        /// <summary>
        /// Authentication key.
        /// </summary>
        [DefaultValue(null)]
        public virtual string AuthenticationKey
        {
            get;
            set;
        }

        /// <summary>
        /// Broadcast key.
        /// </summary>
        [DefaultValue(null)]
        public virtual string BroadcastKey
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
        /// Signing type.
        /// </summary>
        public Signing Signing
        {
            get;
            set;
        }

        /// <summary>
        /// Signing key of the client.
        /// </summary>
        [DefaultValue(null)]
        public string ClientSigningKey
        {
            get;
            set;
        }

        /// <summary>
        /// Agreement key of the client.
        /// </summary>
        [DefaultValue(null)]
        public string ClientAgreementKey
        {
            get;
            set;
        }

        /// <summary>
        /// Signing key of the server.
        /// </summary>
        [DefaultValue(null)]
        public string ServerSigningKey
        {
            get;
            set;
        }

        /// <summary>
        /// Agreement key of the server.
        /// </summary>
        [DefaultValue(null)]
        public string ServerAgreementKey
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
        [DefaultValue(1)]
        virtual public int PhysicalAddress
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
        /// Example. Italy, Saudi Arabia and India standards are using UTC time zone, not DLMS standard time zone.
        /// </summary>
        [DefaultValue(false)]
        [Description("Use UTC time zone.")]
        public bool UtcTimeZone
        {
            get;
            set;
        }

        /// <summary>
        /// Skipped date time fields. This value can be used if meter can't handle deviation or status.
        /// </summary>
        [DefaultValue(DateTimeSkips.None)]
        [Description("Skipped date time fields. This value can be used if meter can't handle deviation or status.")]
        public DateTimeSkips DateTimeSkips
        {
            get;
            set;
        }

        /// <summary>
        /// USed logical client ID.
        /// </summary>
        /// <remarks>
        /// Client ID is always 1 byte long in HDLC and 2 bytes long when WRAPPER is used.
        /// </remarks>
        [DefaultValue(0x10)]
        [Description("Client address.")]
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
        [Description("Interface type.")]
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
        /// PLC MAC Source Address
        /// </summary>
        /// <remarks>
        /// DefaultValue is 0xC00.
        /// </remarks>
        [DefaultValue(0xC00)]
        public UInt16 MACSourceAddress
        {
            get;
            set;
        }

        /// <summary>
        /// PLC MAC Target Address
        /// </summary>
        /// <remarks>
        public UInt16 MacDestinationAddress
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
        /// Is broadcast or unicast connection used.
        /// </summary>
        [DefaultValue(false)]
        public bool Broadcast
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
        ///Frame delay in ms.
        /// </summary>
        public int FrameDelay
        {
            get;
            set;
        }

        /// <summary>
        ///Object delay in ms.
        /// </summary>
        public int ObjectDelay
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
        /// Challenge size.
        /// </summary>
        /// <returns></returns>
        [DefaultValue(16)]
        public byte ChallengeSize
        {
            get;
            set;
        }

        /// <summary>
        /// Public key certificate is send in part of initialize messages (AARQ and AARE).
        /// </summary>
        /// <returns></returns>
        [DefaultValue(false)]
        public bool PublicKeyInInitialize
        {
            get;
            set;
        }
        /// <summary>
        /// Are Initiate Request and Response (AARQ and AARE) signed.
        /// </summary>
        /// <returns></returns>
        [DefaultValue(false)]
        public bool SignInitiateRequestResponse
        {
            get;
            set;
        }


        /// <summary>
        /// Overwrite attribute access rights if association view tells wrong access rights and they are overwritten.
        /// </summary>
        [DefaultValue(false)]
        public bool OverwriteAttributeAccessRights
        {
            get;
            set;
        }

        /// <summary>
        /// Some meters expect that Invocation Counter is increased for GMAC Authentication when connection is established.
        /// </summary>
        [DefaultValue(false)]
        public bool IncreaseInvocationCounterForGMacAuthentication
        {
            get;
            set;
        }

        /// <summary>
        /// Used signing and ciphering order.
        /// </summary>
        public SignCipherOrder SignCipherOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the meter.
        /// </summary>
        [Description("Device Name.")]
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
        /// Proposed Conformance.
        /// </summary>
        [Description("Proposed Conformance.")]
        public int Conformance
        {
            get;
            set;
        }

        /// <summary>
        /// FLAG ID.
        /// </summary>
        [Description("FLAG ID.")]
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
        /// If protected release is used release is including a ciphered xDLMS Initiate request.
        /// </summary>
        public bool UseProtectedRelease
        {
            get;
            set;
        }

        /// <summary>
        /// Security level can't be changed during the connection.
        /// </summary>
        public bool SecurityChangeCheck
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSMeterBase()
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
            MACSourceAddress = 0xC00;
            ChallengeSize = 16;
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

        /// <summary>
        /// IEC serial number can be used with HDLC framing.
        /// </summary>
        public string IecSerialNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Copy meter settings.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void Copy(GXDLMSMeterBase target, GXDLMSMeterBase source)
        {
            target.WaitTime = source.WaitTime;
            target.ResendCount = source.ResendCount;
            target.MaximumBaudRate = source.MaximumBaudRate;
            target.Authentication = source.Authentication;
            target.Standard = source.Standard;
            target.Password = source.Password;
            target.HexPassword = source.HexPassword;
            target.Security = source.Security;
            target.SystemTitle = source.SystemTitle;
            target.ServerSystemTitle = source.ServerSystemTitle;
            target.DedicatedKey = source.DedicatedKey;
            target.PreEstablished = source.PreEstablished;
            target.BlockCipherKey = source.BlockCipherKey;
            target.AuthenticationKey = source.AuthenticationKey;
            target.InvocationCounter = source.InvocationCounter;
            target.FrameCounter = source.FrameCounter;
            target.Challenge = source.Challenge;
            target.PhysicalAddress = source.PhysicalAddress;
            target.LogicalAddress = source.LogicalAddress;
            target.UtcTimeZone = source.UtcTimeZone;
            target.ClientAddress = source.ClientAddress;
            target.StartProtocol = source.StartProtocol;
            target.UseRemoteSerial = source.UseRemoteSerial;
            target.InterfaceType = source.InterfaceType;
            target.UseFrameSize = source.UseFrameSize;
            target.MaxInfoTX = source.MaxInfoTX;
            target.MaxInfoRX = source.MaxInfoRX;
            target.WindowSizeTX = source.WindowSizeTX;
            target.WindowSizeRX = source.WindowSizeRX;
            target.PduSize = source.PduSize;
            target.UserId = source.UserId;
            target.NetworkId = source.NetworkId;
            target.PhysicalDeviceAddress = source.PhysicalDeviceAddress;
            target.InactivityTimeout = source.InactivityTimeout;
            target.ServiceClass = source.ServiceClass;
            target.Priority = source.Priority;
            target.ServerAddressSize = source.ServerAddressSize;
            target.Name = source.Name;
            target.Verbose = source.Verbose;
            target.Conformance = source.Conformance;
            target.Manufacturer = source.Manufacturer;
            target.HDLCAddressing = source.HDLCAddressing;
            target.MediaType = source.MediaType;
            target.MediaSettings = source.MediaSettings;
            target.UseLogicalNameReferencing = source.UseLogicalNameReferencing;
            target.UseProtectedRelease = source.UseProtectedRelease;
        }
    }

    /// <summary>
    /// GXDLMSmeter implements properties to communicate with DLMS/COSEM metering devices.
    /// </summary>
    public class GXDLMSMeter : GXDLMSMeterBase
    {
        private GXDLMSObjectCollection objects;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSMeter() : base()
        {
            Objects = new GXDLMSObjectCollection();
            PduWaitTime = 100;
            GbtWindowSize = 1;
        }


        /// <summary>
        /// Gets or sets the object that contains data about the control.
        /// </summary>
#if !WINDOWS_UWP
        [ReadOnly(true)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        [System.Xml.Serialization.XmlIgnore()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Copy meter settings.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public void Copy(GXDLMSMeter target, GXDLMSMeter source)
        {
            GXDLMSMeterBase.Copy(target, source);
            target.Objects = source.Objects;
            target.Tag = source.Tag;
            target.PduWaitTime = source.PduWaitTime;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [XmlArray("Objects2")]
        public virtual GXDLMSObjectCollection Objects
        {
            get
            {
                return objects;
            }
            set
            {
                objects = value;
            }
        }

        /// <summary>
        /// Define how long reply is waited in seconds.
        /// </summary>
        [DefaultValue(100)]
        public int PduWaitTime
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum GBT window size.
        /// </summary>
        /// <remarks>
        /// DefaultValue is 1.
        /// </remarks>
        [DefaultValue(1)]
        public byte GbtWindowSize
        {
            get;
            set;
        }
    }
}