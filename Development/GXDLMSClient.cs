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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Ecdsa;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMS implements methods to communicate with DLMS/COSEM metering devices.
    /// </summary>
    public class GXDLMSClient
    {
        /// <summary>
        /// Manufacturer ID.
        /// </summary>
        /// <remarks>
        /// Manufacturer ID (FLAG ID) is used for manufacturer depending functionality.
        /// </remarks>
        private string manufacturerId;

        protected GXDLMSTranslator translator;
        /// <summary>
        /// Initialize challenge that is restored after the connection is closed.
        /// </summary>
        private byte[] InitializeChallenge;
        /// <summary>
        /// Initialize PDU size that is restored after the connection is closed.
        /// </summary>
        private UInt16 InitializePduSize;

        /// <summary>
        /// Initialize Max HDLC transmission size that is restored after the connection is closed.
        /// </summary>
        private UInt16 InitializeMaxInfoTX;

        /// <summary>
        /// Initialize Max HDLC receive size that is restored after the connection is closed.
        /// </summary>
        private UInt16 InitializeMaxInfoRX;

        /// <summary>
        /// Initialize max HDLC window size in transmission that is restored after the connection is closed.
        /// </summary>
        private byte InitializeWindowSizeTX;

        /// <summary>
        /// Initialize max HDLC window size in receive that is restored after the connection is closed.
        /// </summary>
        private byte InitializeWindowSizeRX;

        /// <summary>
        /// XML client don't throw exceptions. It serializes them as a default. Set value to true, if exceptions are thrown.
        /// </summary>
        protected bool throwExceptions;

        /// <summary>
        /// DLMS settings.
        /// </summary>
        public GXDLMSSettings Settings
        {
            get;
            private set;
        }

        /// <summary>
        /// Manufacturer ID.
        /// </summary>
        /// <remarks>
        /// Manufacturer ID (FLAG ID) is used for manucaturer depending functionality.
        /// </remarks>
        public string ManufacturerId
        {
            get
            {
                return manufacturerId;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length != 3)
                {
                    throw new ArgumentOutOfRangeException("Manufacturer ID is 3 chars long string");
                }
                manufacturerId = value;
            }
        }


        private static Dictionary<ObjectType, Type> AvailableObjectTypes = new Dictionary<ObjectType, Type>();

        /// <summary>
        /// Static Constructor. This is called only once. Get available COSEM objects.
        /// </summary>
        static GXDLMSClient()
        {
            GXDLMS.GetAvailableObjects(AvailableObjectTypes);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSClient() : this(false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="useLogicalNameReferencing">Is Logical or short name referencing used.</param>
        /// <param name="clientAddress">Client address. Default is 16 (0x10)</param>
        /// <param name="serverAddress">Server ID. Default is 1.</param>
        /// <param name="authentication">Authentication type. Default is None</param>
        /// <param name="password">Password if authentication is used.</param>
        /// <param name="interfaceType">Interface type. Default is general.</param>
        public GXDLMSClient(bool useLogicalNameReferencing) : this(useLogicalNameReferencing, 16, 1, Authentication.None, null, InterfaceType.HDLC)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="useLogicalNameReferencing">Is Logical or short name referencing used.</param>
        /// <param name="clientAddress">Client address. Default is 16 (0x10)</param>
        /// <param name="serverAddress">Server ID. Default is 1.</param>
        /// <param name="authentication">Authentication type. Default is None</param>
        /// <param name="password">Password if authentication is used.</param>
        /// <param name="interfaceType">Interface type. Default is general.</param>
        public GXDLMSClient(bool useLogicalNameReferencing,
                            int clientAddress, int serverAddress, Authentication authentication,
                            string password, InterfaceType interfaceType)
        {
            Settings = new GXDLMSSettings(false, interfaceType);
            Settings.Objects.Parent = this;
            Settings.UseLogicalNameReferencing = useLogicalNameReferencing;
            Settings.Authentication = authentication;
            Settings.ServerAddress = serverAddress;
            Settings.ClientAddress = clientAddress;
            if (password != null)
            {
                Settings.Password = ASCIIEncoding.ASCII.GetBytes(password);
            }
            Settings.Plc.Reset();
            Settings.CryptoNotifier = new GXCryptoNotifier();
        }

        /// <summary>
        /// Notify generated PDU.
        /// </summary>
        public event PduEventHandler OnPdu
        {
            add
            {
                Settings.CryptoNotifier.pdu += value;
            }
            remove
            {
                Settings.CryptoNotifier.pdu -= value;
            }
        }

        /// <summary>
        /// This event is called when meter implemenets manufacturer spesific object.
        /// </summary>
        /// <remarks>
        /// The manufacturer spesific object must implement GXDLMSObject and IGXDLMSBase.
        /// </remarks>
        public event ObjectCreateEventHandler OnCustomObject
        {
            add
            {
                Settings.customObject += value;
            }
            remove
            {
                Settings.customObject -= value;
            }
        }

        /// <summary>
        /// This event is called when meter implemenets custom non DLMS PDU.
        /// </summary>
        public event CustomPduEventHandler OnCustomPdu
        {
            add
            {
                Settings.customPdu += value;
            }
            remove
            {
                Settings.customPdu -= value;
            }
        }

        /// <summary>
        /// Copy client settings.
        /// </summary>
        /// <param name="target"></param>
        public void CopyTo(GXDLMSClient target)
        {
            Settings.CopyTo(target.Settings);
        }


        /// <summary>
        /// List of available custom obis codes.
        /// </summary>
        /// <remarks>
        /// This list is used when Association view is read from the meter and description of the object is needed.
        /// If collection is not set description of object is empty.
        /// </remarks>
        public GXObisCodeCollection CustomObisCodes
        {
            get;
            set;
        }

        /// <summary>
        /// Client address.
        /// </summary>
        public int ClientAddress
        {
            get
            {
                return Settings.ClientAddress;
            }
            set
            {
                Settings.ClientAddress = value;
            }
        }

        /// <summary>
        /// Standard says that Time zone is from normal time to UTC in minutes.
        /// If meter is configured to use UTC time (UTC to normal time) set this to true.
        /// Example. Italy, Saudi Arabia and India standards are using UTC time zone, not DLMS standard time zone.
        /// </summary>
        public bool UseUtc2NormalTime
        {
            get
            {
                return Settings.UseUtc2NormalTime;
            }
            set
            {
                Settings.UseUtc2NormalTime = value;
            }
        }

        /// <summary>
        /// Expected Invocation (Frame) counter value.
        /// </summary>
        /// <remarks>
        /// If this value is set ciphered PDUs that are using smaller invocation counter values are rejected.
        /// Invocation counter value is not validate if value is zero.
        /// </remarks>
        public UInt64 ExpectedInvocationCounter
        {
            get
            {
                return Settings.ExpectedInvocationCounter;
            }
            set
            {
                Settings.ExpectedInvocationCounter = value;
            }
        }

        /// <summary>
        /// Some meters expect that Invocation Counter is increased for GMAC Authentication when connection is established.
        /// </summary>
        public bool IncreaseInvocationCounterForGMacAuthentication
        {
            get
            {
                return Settings.IncreaseInvocationCounterForGMacAuthentication;
            }
            set
            {
                Settings.IncreaseInvocationCounterForGMacAuthentication = value;
            }
        }

        /// <summary>
        /// Skipped date time fields. This value can be used if meter can't handle deviation or status.
        /// </summary>
        public DateTimeSkips DateTimeSkips
        {
            get
            {
                return Settings.DateTimeSkips;
            }
            set
            {
                Settings.DateTimeSkips = value;
            }
        }


        /// <summary>
        /// Standard says that Time zone is from normal time to UTC in minutes.
        /// If meter is configured to use UTC time (UTC to normal time) set this to true.
        /// Example. Italy, Saudi Arabia and India standards are using UTC time zone, not DLMS standard time zone.
        /// </summary>
        [Obsolete("Use UseUtc2NormalTime instead.")]
        public bool UtcTimeZone
        {
            get
            {
                return Settings.UseUtc2NormalTime;
            }
            set
            {
                Settings.UseUtc2NormalTime = value;
            }
        }

        /// <summary>
        /// Used standard.
        /// </summary>
        public Standard Standard
        {
            get
            {
                return Settings.Standard;
            }
            set
            {
                Settings.Standard = value;
            }
        }

        /// <summary>
        /// Force that data is always sent as blocks.
        /// </summary>
        /// <remarks>
        /// Some meters can handle only blocks. This property is used to force send all data in blocks.
        /// </remarks>
        public bool ForceToBlocks
        {
            get
            {
                return Settings.ForceToBlocks;
            }
            set
            {
                Settings.ForceToBlocks = value;
            }
        }

        /// <summary>
        /// Quality of service.
        /// </summary>
        [DefaultValue(0)]
        public byte QualityOfService
        {
            get
            {
                return Settings.QualityOfService;
            }
            set
            {
                Settings.QualityOfService = value;
            }
        }

        /// <summary>
        /// User id is the identifier of the user.
        /// </summary>
        /// <remarks>
        /// This value is used if user list on Association LN is used.
        /// </remarks>
        [DefaultValue(-1)]
        public int UserId
        {
            get
            {
                return Settings.UserId;
            }
            set
            {
                if (value < -1 || value > 255)
                {
                    throw new ArgumentOutOfRangeException("Invalid user Id.");
                }
                Settings.UserId = value;
            }
        }

        /// <summary>
        /// Server address.
        /// </summary>
        public int ServerAddress
        {
            get
            {
                return Settings.ServerAddress;
            }
            set
            {
                Settings.ServerAddress = value;
            }
        }

        /// <summary>
        /// Size of server address.
        /// </summary>
        public byte ServerAddressSize
        {
            get
            {
                return Settings.ServerAddressSize;
            }
            set
            {
                Settings.ServerAddressSize = value;
            }
        }

        /// <summary>
        ///  Source system title.
        /// </summary>
        /// <remarks>
        /// Meter returns system title when ciphered connection is made or GMAC authentication is used.
        /// </remarks>
        public byte[] SourceSystemTitle
        {
            get
            {
                return Settings.SourceSystemTitle;
            }
        }

        /// <summary>
        /// DLMS version number.
        /// </summary>
        /// <remarks>
        /// Gurux DLMS component supports DLMS version number 6.
        /// </remarks>
        /// <seealso cref="SNRMRequest"/>
        [DefaultValue(6)]
        public byte DLMSVersion
        {
            get
            {
                return Settings.DLMSVersion;
            }
        }

        /// <summary>
        /// Retrieves the maximum size of PDU receiver.
        /// </summary>
        /// <remarks>
        /// PDU size tells maximum size of PDU packet.
        /// Value can be from 0 to 0xFFFF. By default the value is 0xFFFF.
        /// </remarks>
        /// <seealso cref="ClientAddress"/>
        /// <seealso cref="ServerAddress"/>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="UseLogicalNameReferencing"/>
        [DefaultValue(0xFFFF)]
        public ushort MaxReceivePDUSize
        {
            get
            {
                return Settings.MaxPduSize;
            }
            set
            {
                Settings.MaxPduSize = value;
            }
        }

        /// <summary>
        /// Maximum GBT window size.
        /// </summary>
        [DefaultValue(1)]
        public byte GbtWindowSize
        {
            get
            {
                return Settings.GbtWindowSize;
            }
            set
            {
                Settings.GbtWindowSize = value;
            }
        }

        /// <summary>
        /// Determines, whether Logical, or Short name, referencing is used.
        /// </summary>
        /// <remarks>
        /// Referencing depends on the device to communicate with.
        /// Normally, a device supports only either Logical or Short name referencing.
        /// The referencing is defined by the device manufacurer.
        /// If the referencing is wrong, the SNMR message will fail.
        /// </remarks>
        [DefaultValue(false)]
        public bool UseLogicalNameReferencing
        {
            get
            {
                return Settings.UseLogicalNameReferencing;
            }
            set
            {
                Settings.UseLogicalNameReferencing = value;
            }
        }

        /// <summary>
        /// Client to Server custom challenge.
        /// </summary>
        /// <remarks>
        /// This is for debugging purposes. Reset custom challenge settings CtoSChallenge to null.
        /// </remarks>
        public byte[] CtoSChallenge
        {
            get
            {
                return Settings.CtoSChallenge;
            }
            set
            {
                Settings.UseCustomChallenge = value != null;
                Settings.CtoSChallenge = value;
            }
        }

        /// <summary>
        /// Retrieves the password that is used in communication.
        /// </summary>
        /// <remarks>
        /// If authentication is set to none, password is not used.
        /// For HighSHA1, HighMD5 and HighGMAC password is worked as a shared secret.
        /// </remarks>
        /// <seealso cref="Authentication"/>
        public byte[] Password
        {
            get
            {
                return Settings.Password;
            }
            set
            {
                Settings.Password = value;
            }
        }

        /// <summary>
        /// When connection is made client tells what kind of services it want's to use.
        /// </summary>
        public Conformance ProposedConformance
        {
            get
            {
                return Settings.ProposedConformance;
            }
            set
            {
                Settings.ProposedConformance = value;
            }
        }

        /// <summary>
        /// Functionality what server can offer.
        /// </summary>
        public Conformance NegotiatedConformance
        {
            get
            {
                return Settings.NegotiatedConformance;
            }
            set
            {
                Settings.NegotiatedConformance = value;
            }
        }

        /// <summary>
        /// Protocol version.
        /// </summary>
        public string ProtocolVersion
        {
            get
            {
                return Settings.protocolVersion;
            }
            set
            {
                Settings.protocolVersion = value;
            }
        }

        /// <summary>
        /// Retrieves the authentication used in communicating with the device.
        /// </summary>
        /// <remarks>
        /// By default authentication is not used. If authentication is used,
        /// set the password with the Password property.
        /// Note!
        /// For HLS authentication password (shared secret) is needed from the manufacturer.
        /// </remarks>
        /// <seealso cref="Password"/>
        /// <seealso cref="ClientAddress"/>
        /// <seealso cref="ServerAddress"/>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="UseLogicalNameReferencing"/>
        /// <seealso cref="MaxReceivePDUSize"/>
        [DefaultValue(Authentication.None)]
        public Authentication Authentication
        {
            get
            {
                return Settings.Authentication;
            }
            set
            {
                Settings.Authentication = value;
            }
        }

        /// <summary>
        /// Challenge Size.
        /// </summary>
        /// <remarks>
        /// Random challenge is used if value is zero.
        /// </remarks>
        public byte ChallengeSize
        {
            get
            {
                return Settings.ChallengeSize;
            }
            set
            {
                Settings.ChallengeSize = value;
            }
        }

        /// <summary>
        /// Set starting block index in HDLC framing.
        /// Default is One based, but some meters use Zero based value.
        /// Usually this is not used.
        /// </summary>
        public UInt32 StartingBlockIndex
        {
            get
            {
                return Settings.StartingBlockIndex;
            }
            set
            {
                Settings.StartingBlockIndex = value;
                Settings.ResetBlockIndex();
            }
        }

        /// <summary>
        /// Used priority in HDLC framing.
        /// </summary>
        public Priority Priority
        {
            get
            {
                return Settings.Priority;
            }
            set
            {
                Settings.Priority = value;
            }
        }

        /// <summary>
        /// Used service class in HDLC framing.
        /// </summary>
        public ServiceClass ServiceClass
        {
            get
            {
                return Settings.ServiceClass;
            }
            set
            {
                Settings.ServiceClass = value;
            }
        }

        /// <summary>
        /// Invoke ID.
        /// </summary>
        public byte InvokeID
        {
            get
            {
                return Settings.InvokeID;
            }
            set
            {
                Settings.InvokeID = value;
            }
        }

        /// <summary>
        /// Auto increase Invoke ID.
        /// </summary>
        [DefaultValue(false)]
        public bool AutoIncreaseInvokeID
        {
            get
            {
                return Settings.AutoIncreaseInvokeID;
            }
            set
            {
                Settings.AutoIncreaseInvokeID = value;
            }
        }

        /// <summary>
        /// Determines the type of the connection
        /// </summary>
        /// <remarks>
        /// All DLMS meters do not support the IEC 62056-47 standard.
        /// If the device does not support the standard, and the connection is made
        /// using TCP/IP, set the type to InterfaceType.General.
        /// </remarks>
        public InterfaceType InterfaceType
        {
            get
            {
                return Settings.InterfaceType;
            }
            set
            {
                Settings.InterfaceType = value;
            }
        }

        /// <summary>
        /// Is pre-established connection used.
        /// </summary>
        /// <remarks>
        /// AARQ or release messages are not used with pre-established connections.
        /// </remarks>
        public bool PreEstablishedConnection
        {
            get
            {
                return Settings.PreEstablishedSystemTitle != null;
            }
        }


        /// <summary>
        /// HDLC connection settings.
        /// </summary>
        [Obsolete("Use HdlcSettings instead.")]
        public GXDLMSLimits Limits
        {
            get
            {
                return (GXDLMSLimits)Settings.Hdlc;
            }
        }

        /// <summary>
        /// HDLC connection settings.
        /// </summary>
        public GXHdlcSettings HdlcSettings
        {
            get
            {
                return Settings.Hdlc;
            }
        }

        /// <summary>
        /// PLC settings.
        /// </summary>
        public GXPlcSettings Plc
        {
            get
            {
                return Settings.Plc;
            }
        }

        /// <summary>
        /// M-Bus settings.
        /// </summary>
        public GXMBusSettings MBus
        {
            get
            {
                return Settings.MBus;
            }
        }

        /// <summary>
        /// PDU settings.
        /// </summary>
        public GXPduSettings Pdu
        {
            get
            {
                return Settings.Pdu;
            }
        }

        /// <summary>
        /// CoAP settings.
        /// </summary>
        public GXCoAPSettings Coap
        {
            get
            {
                return Settings.Coap;
            }
        }


        /// <summary>
        /// Gateway settings.
        /// </summary>
        public GXDLMSGateway Gateway
        {
            get
            {
                return Settings.Gateway;
            }
            set
            {
                Settings.Gateway = value;
            }
        }

        /// <summary>
        /// Is data send as a broadcast or unicast.
        /// </summary>
        public bool Broacast
        {
            get
            {
                return Settings.Broacast;
            }
            set
            {
                Settings.Broacast = value;
            }
        }

        /// <summary>
        /// Connection state to the meter.
        /// </summary>
        public ConnectionState ConnectionState
        {
            get
            {
                return Settings.Connected;
            }
        }

        /// <summary>
        /// Generates SNRM request.
        /// </summary>
        /// <remarks>
        /// his method is used to generate send SNRMRequest.
        /// Before the SNRM request can be generated, at least the following
        /// properties must be set:
        /// <ul>
        /// <li>ClientAddress</li>
        /// <li>ServerAddress</li>
        /// </ul>
        /// <b>Note! </b>According to IEC 62056-47: when communicating using
        /// TCP/IP, the SNRM request is not send.
        /// </remarks>
        /// <returns>SNRM request as byte array.</returns>
        /// <seealso cref="ClientAddress"/>
        /// <seealso cref="ServerAddress"/>
        /// <seealso cref="ParseUAResponse"/>
        public byte[] SNRMRequest()
        {
            return SNRMRequest(false);
        }

        /// <summary>
        /// Generates SNRM request.
        /// </summary>
        /// <remarks>
        /// his method is used to generate send SNRMRequest.
        /// Before the SNRM request can be generated, at least the following
        /// properties must be set:
        /// <ul>
        /// <li>ClientAddress</li>
        /// <li>ServerAddress</li>
        /// </ul>
        /// <b>Note! </b>According to IEC 62056-47: when communicating using
        /// TCP/IP, the SNRM request is not send.
        /// </remarks>
        /// <param name="forceParameters">Are HDLC parameters forced. Some meters require this.</param>
        /// <returns>SNRM request as byte array.</returns>
        /// <seealso cref="ClientAddress"/>
        /// <seealso cref="ServerAddress"/>
        /// <seealso cref="ParseUAResponse"/>
        public byte[] SNRMRequest(bool forceParameters)
        {
            //Save default values.
            Settings.Closing = false;
            InitializeMaxInfoTX = HdlcSettings.MaxInfoTX;
            InitializeMaxInfoRX = HdlcSettings.MaxInfoRX;
            InitializeWindowSizeTX = HdlcSettings.WindowSizeTX;
            InitializeWindowSizeRX = HdlcSettings.WindowSizeRX;
            Settings.Connected = ConnectionState.None;
            IsAuthenticationRequired = false;
            Settings.ResetFrameSequence();
            // SNRM request is not used for all communication channels.
            if (InterfaceType == InterfaceType.PlcHdlc)
            {
                return GXDLMS.GetMacHdlcFrame(Settings, (byte)Command.Snrm, 0, null);
            }
            if (InterfaceType != InterfaceType.HDLC && InterfaceType != InterfaceType.HdlcWithModeE)
            {
                return null;
            }
            GXByteBuffer data = new GXByteBuffer(25);
            data.SetUInt8(0x81); // FromatID
            data.SetUInt8(0x80); // GroupID
            data.SetUInt8(0); // Length.
            int maxInfoTX = HdlcSettings.MaxInfoTX, maxInfoRX = HdlcSettings.MaxInfoRX;
            if (HdlcSettings.UseFrameSize)
            {
                byte[] primaryAddress, secondaryAddress;
                primaryAddress = GXDLMS.GetHdlcAddressBytes(Settings.ServerAddress, Settings.ServerAddressSize);
                secondaryAddress = GXDLMS.GetHdlcAddressBytes(Settings.ClientAddress, 0);
                maxInfoTX -= (10 + secondaryAddress.Length);
                maxInfoRX -= (10 + primaryAddress.Length);
            }

            // If custom HDLC parameters are used.
            if (InterfaceType != InterfaceType.PlcHdlc &&
                (forceParameters ||
                GXDLMSLimitsDefault.DefaultMaxInfoTX != maxInfoTX ||
                GXDLMSLimitsDefault.DefaultMaxInfoRX != maxInfoRX ||
                GXDLMSLimitsDefault.DefaultWindowSizeTX != HdlcSettings.WindowSizeTX ||
                GXDLMSLimitsDefault.DefaultWindowSizeRX != HdlcSettings.WindowSizeRX))
            {
                data.SetUInt8((byte)HDLCInfo.MaxInfoTX);
                GXDLMS.AppendHdlcParameter(data, (UInt16)maxInfoTX);
                data.SetUInt8((byte)HDLCInfo.MaxInfoRX);
                GXDLMS.AppendHdlcParameter(data, (UInt16)maxInfoRX);
                data.SetUInt8((byte)HDLCInfo.WindowSizeTX);
                data.SetUInt8(4);
                data.SetUInt32(HdlcSettings.WindowSizeTX);
                data.SetUInt8((byte)HDLCInfo.WindowSizeRX);
                data.SetUInt8(4);
                data.SetUInt32(HdlcSettings.WindowSizeRX);
            }
            // If default HDLC parameters are not used.
            if (data.Size != 3)
            {
                data.SetUInt8(2, (byte)(data.Size - 3)); // Length.
            }
            else
            {
                data = null;
            }
            return GXDLMS.GetHdlcFrame(Settings, (byte)Command.Snrm, data);
        }

        /// <summary>
        /// Parses UAResponse from byte array.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="data"></param>
        /// <seealso cref="ParseUAResponse"/>
        public void ParseUAResponse(GXByteBuffer data)
        {
            if (Settings.InterfaceType == InterfaceType.HDLC ||
                Settings.InterfaceType == InterfaceType.HdlcWithModeE ||
                Settings.InterfaceType == InterfaceType.PlcHdlc)
            {
                GXDLMS.ParseSnrmUaResponse(data, Settings);
                Settings.Connected = ConnectionState.Hdlc;
            }
        }

        /// <summary>
        /// Generate AARQ request.
        /// </summary>
        /// <returns>AARQ request as byte array.</returns>
        /// <seealso cref="ParseAAREResponse"/>
        public byte[][] AARQRequest()
        {
            if (PreEstablishedConnection)
            {
                // AARQ is not generate for pre-established connections.
                return null;
            }
            if (ProposedConformance == 0)
            {
                throw new Exception("Invalid conformance.");
            }
            //Save default values.
            Settings.Closing = false;
            InitializePduSize = MaxReceivePDUSize;
            InitializeChallenge = Settings.CtoSChallenge;
            Settings.NegotiatedConformance = (Conformance)0;
            Settings.ResetBlockIndex();
            Settings.ServerPublicKeyCertificate = null;
            Settings.Connected &= ~ConnectionState.Dlms;
            GXByteBuffer buff = new GXByteBuffer(20);
            GXDLMS.CheckInit(Settings);
            Settings.StoCChallenge = null;
            if (AutoIncreaseInvokeID)
            {
                Settings.InvokeID = 0;
            }
            else
            {
                Settings.InvokeID = 1;
            }
            // Reset Ephemeral keys.
            Settings.EphemeralBlockCipherKey = null;
            Settings.EphemeralBroadcastBlockCipherKey = null;
            Settings.EphemeralAuthenticationKey = null;
            Settings.EphemeralKek = null;
            //If High authentication is used.
            if (Authentication > Authentication.Low)
            {
                if (!Settings.UseCustomChallenge)
                {
                    Settings.CtoSChallenge = GXSecure.GenerateChallenge(Settings.Authentication, Settings.ChallengeSize);
                }
            }
            else
            {
                Settings.CtoSChallenge = null;
            }
            GXAPDU.GenerateAarq(Settings, Settings.Cipher, null, buff);
            byte[][] reply;
            if (UseLogicalNameReferencing)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.Aarq, 0, buff, null, 0xff, Command.None);
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                reply = GXDLMS.GetSnMessages(new GXDLMSSNParameters(Settings, Command.Aarq, 0, 0, null, buff));
            }
            return reply;
        }

        /// <summary>
        /// Parses the AARE response.
        /// </summary>
        /// <param name="reply"></param>
        /// <remarks>
        /// Parse method will update the following data:
        /// <ul>
        /// <li>DLMSVersion</li>
        /// <li>MaxReceivePDUSize</li>
        /// <li>UseLogicalNameReferencing</li>
        /// <li>LNSettings or SNSettings</li>
        /// </ul>
        /// LNSettings or SNSettings will be updated, depending on the referencing,
        /// Logical name or Short name.
        /// </remarks>
        /// <returns>The AARE response</returns>
        /// <seealso cref="AARQRequest"/>
        /// <seealso cref="UseLogicalNameReferencing"/>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="MaxReceivePDUSize"/>
        public void ParseAAREResponse(GXByteBuffer reply)
        {
            try
            {
                IsAuthenticationRequired = (SourceDiagnostic)GXAPDU.ParsePDU(Settings, Settings.Cipher, reply, null) == SourceDiagnostic.AuthenticationRequired;
                if (IsAuthenticationRequired)
                {
                    System.Diagnostics.Debug.WriteLine("Authentication is required.");
                }
                else
                {
                    Settings.Connected |= ConnectionState.Dlms;
                }
                System.Diagnostics.Debug.WriteLine("- Server max PDU size is " + MaxReceivePDUSize);
                if (DLMSVersion != 6)
                {
                    throw new GXDLMSException("Invalid DLMS version number.");
                }
            }
            catch (OutOfMemoryException)
            {
                throw new Exception("Frame is not fully received.");
            }
        }

        /// <summary>
        /// Is authentication Required.
        /// </summary>
        /// <seealso cref="GetApplicationAssociationRequest"/>
        /// <seealso cref="ParseApplicationAssociationResponse"/>
        public bool IsAuthenticationRequired
        {
            get;
            private set;
        }

        /// <summary>
        /// Get challenge request if HLS authentication is used.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="IsAuthenticationRequired"/>
        /// <seealso cref="ParseApplicationAssociationResponse"/>
        public byte[][] GetApplicationAssociationRequest()
        {
            return GetApplicationAssociationRequest(null);
        }

        /// <summary>
        /// Get challenge request if HLS authentication is used.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="IsAuthenticationRequired"/>
        /// <seealso cref="ParseApplicationAssociationResponse"/>
        public byte[][] GetApplicationAssociationRequest(string ln)
        {
            if (Settings.Authentication != Authentication.HighECDSA &&
                Settings.Authentication != Authentication.HighGMAC &&
                    (Settings.Password == null || Settings.Password.Length == 0))
            {
                throw new ArgumentException("Password is invalid.");
            }
            Settings.ResetBlockIndex();
            byte[] challenge;
            //Count challenge for Landis+Gyr. L+G is using custom way to count the challenge.
            if (manufacturerId == "LGZ" && Settings.Authentication == Enums.Authentication.High)
            {
                challenge = EncryptLandisGyrHighLevelAuthentication(Settings.Password, Settings.StoCChallenge);
                if (UseLogicalNameReferencing)
                {
                    if (string.IsNullOrEmpty(ln))
                    {
                        ln = "0.0.40.0.0.255";
                    }
                    return Method(ln, ObjectType.AssociationLogicalName,
                                  1, challenge, DataType.OctetString);
                }
                return Method(0xFA00, ObjectType.AssociationShortName, 8, challenge, DataType.OctetString);
            }

            byte[] pw;
            if (Settings.Authentication == Enums.Authentication.HighGMAC)
            {
                pw = Settings.Cipher.SystemTitle;
            }
            else if (Settings.Authentication == Enums.Authentication.HighSHA256)
            {
                GXByteBuffer tmp = new GXByteBuffer();
                tmp.Set(Settings.Password);
                tmp.Set(Settings.Cipher.SystemTitle);
                tmp.Set(Settings.SourceSystemTitle);
                tmp.Set(Settings.StoCChallenge);
                tmp.Set(Settings.CtoSChallenge);
                pw = tmp.Array();
            }
            else if (Settings.Authentication == Enums.Authentication.HighECDSA)
            {
                if (Settings.Cipher.SigningKeyPair.Key == null)
                {
                    Settings.Cipher.SigningKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(
                        (GXPublicKey)Settings.GetKey(DLMS.Objects.Enums.CertificateType.DigitalSignature,
                    Settings.SourceSystemTitle, false),
                        Settings.Cipher.SigningKeyPair.Value);
                }
                if (Settings.Cipher.SigningKeyPair.Value == null)
                {
                    Settings.Cipher.SigningKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(Settings.Cipher.SigningKeyPair.Key,
                        (GXPrivateKey)Settings.GetKey(DLMS.Objects.Enums.CertificateType.DigitalSignature,
                    Settings.Cipher.SystemTitle, true));
                }
                GXByteBuffer tmp = new GXByteBuffer();
                tmp.Set(Settings.Cipher.SystemTitle);
                tmp.Set(Settings.SourceSystemTitle);
                tmp.Set(Settings.StoCChallenge);
                tmp.Set(Settings.CtoSChallenge);
                pw = tmp.Array();
            }
            else
            {
                pw = Settings.Password;
            }
            challenge = GXSecure.Secure(Settings, Settings.Cipher, Settings.Cipher.InvocationCounter,
                                               Settings.StoCChallenge, pw);
            if (Settings.Cipher != null && Settings.IncreaseInvocationCounterForGMacAuthentication)
            {
                ++Settings.Cipher.InvocationCounter;
            }
            if (UseLogicalNameReferencing)
            {
                if (string.IsNullOrEmpty(ln))
                {
                    ln = "0.0.40.0.0.255";
                }
                return Method(ln, ObjectType.AssociationLogicalName,
                              1, challenge, DataType.OctetString);
            }
            return Method(0xFA00, ObjectType.AssociationShortName, 8, challenge,
                          DataType.OctetString);
        }

        /// <summary>
        /// Parse server's challenge if HLS authentication is used.
        /// </summary>
        /// <param name="reply"></param>
        /// <seealso cref="IsAuthenticationRequired"/>
        /// <seealso cref="GetApplicationAssociationRequest"/>
        public void ParseApplicationAssociationResponse(GXByteBuffer reply)
        {
            //Landis+Gyr is not returning StoC.
            if (manufacturerId == "LGZ" && Settings.Authentication == Enums.Authentication.High)
            {
                Settings.Connected |= ConnectionState.Dlms;
            }
            else
            {
                GXDataInfo info = new GXDataInfo();
                bool equals = false;
                byte[] value = (byte[])GXCommon.GetData(Settings, reply, info);
                if (value != null)
                {
                    byte[] secret;
                    UInt32 ic = 0;
                    if (Settings.Authentication == Enums.Authentication.HighECDSA)
                    {
#if !WINDOWS_UWP
                        if (Settings.Cipher.Equals(new KeyValuePair<byte[], byte[]>()))
                        {
                            throw new ArgumentNullException("SigningKeyPair is empty.");
                        }
                        GXByteBuffer tmp2 = new GXByteBuffer();
                        tmp2.Set(Settings.SourceSystemTitle);
                        tmp2.Set(Settings.Cipher.SystemTitle);
                        tmp2.Set(Settings.CtoSChallenge);
                        tmp2.Set(Settings.StoCChallenge);
                        GXEcdsa sig = new GXEcdsa(Settings.Cipher.SigningKeyPair.Key);
                        equals = sig.Verify(value, tmp2.Array());
#endif //!WINDOWS_UWP                    
                    }
                    else
                    {
                        if (Settings.Authentication == Authentication.HighGMAC)
                        {
                            secret = Settings.SourceSystemTitle;
                            GXByteBuffer bb = new GXByteBuffer(value);
                            bb.GetUInt8();
                            ic = bb.GetUInt32();
                        }
                        else if (Settings.Authentication == Enums.Authentication.HighSHA256)
                        {
                            GXByteBuffer tmp2 = new GXByteBuffer();
                            tmp2.Set(Settings.Password);
                            tmp2.Set(Settings.SourceSystemTitle);
                            tmp2.Set(Settings.Cipher.SystemTitle);
                            tmp2.Set(Settings.CtoSChallenge);
                            tmp2.Set(Settings.StoCChallenge);
                            secret = tmp2.Array();
                        }
                        else
                        {
                            secret = Settings.Password;
                        }
                        byte[] tmp = GXSecure.Secure(Settings, Settings.Cipher, ic,
                                                     Settings.CtoSChallenge, secret);
                        GXByteBuffer challenge = new GXByteBuffer(tmp);
                        equals = challenge.Compare(value);
                    }
                    Settings.Connected |= ConnectionState.Dlms;
                }
                if (!equals)
                {
                    Settings.Connected &= ~ConnectionState.Dlms;
                    throw new GXDLMSException("Invalid password. Server to Client challenge do not match.");
                }
            }
        }

        /// <summary>
        /// The version can be used for backward compatibility.
        /// </summary>
        public int Version
        {
            get
            {
                return Settings.Version;
            }
            set
            {
                Settings.Version = value;
            }
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
        /// Generates a release request.
        /// </summary>
        /// <returns>Release request, as byte array.</returns>
        public byte[][] ReleaseRequest()
        {
            return ReleaseRequest(false);
        }

        /// <summary>
        /// Generates a release request.
        /// </summary>
        /// <returns>Release request, as byte array.</returns>
        public byte[][] ReleaseRequest(bool force)
        {
            if (PreEstablishedConnection)
            {
                // Disconnect message is not used for pre-established connections.
                return null;
            }
            // If connection is not established, there is no need to send
            // DisconnectRequest.
            if (!force && (Settings.Connected & ConnectionState.Dlms) == 0)
            {
                return null;
            }
            GXByteBuffer buff = new GXByteBuffer();
            if (!UseProtectedRelease)
            {
                buff.SetUInt8(3);
                buff.SetUInt8(0x80);
                buff.SetUInt8(1);
                buff.SetUInt8(0);
            }
            else
            {
                //Length.
                buff.SetUInt8(0);
                buff.SetUInt8(0x80);
                buff.SetUInt8(01);
                buff.SetUInt8(00);
                //Restore default PDU size for the release.
                MaxReceivePDUSize = InitializePduSize;
                GXAPDU.GenerateUserInformation(Settings, Settings.Cipher, null, buff);
                //Increase IC.
                if (Settings.IsCiphered(false))
                {
                    ++Settings.Cipher.InvocationCounter;
                }
                buff.SetUInt8(0, (byte)(buff.Size - 1));
            }
            byte[][] reply;
            if (UseLogicalNameReferencing)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.ReleaseRequest, 0, buff, null, 0xff, Command.None);
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                reply = GXDLMS.GetSnMessages(new GXDLMSSNParameters(Settings, Command.ReleaseRequest, 0xFF, 0xFF, null, buff));
            }
            Settings.Connected &= ~ConnectionState.Dlms;
            Settings.Closing = true;
            //Restore default values.
            MaxReceivePDUSize = InitializePduSize;
            Settings.CtoSChallenge = InitializeChallenge;
            return reply;
        }
        /// <summary>
        /// Generates a disconnect request.
        /// </summary>
        /// <returns>Disconnected request, as byte array.</returns>
        public byte[] DisconnectRequest()
        {
            return DisconnectRequest(false);
        }

        /// <summary>
        /// Generates a disconnect request.
        /// </summary>
        /// <returns>Disconnected request, as byte array.</returns>
        public byte[] DisconnectRequest(bool force)
        {
            if (!force && Settings.Connected == ConnectionState.None)
            {
                return null;
            }
            byte[] ret = null;
            if (GXDLMS.UseHdlc(Settings.InterfaceType))
            {
                if (Settings.InterfaceType == InterfaceType.PlcHdlc)
                {
                    ret = GXDLMS.GetMacHdlcFrame(Settings, (byte)Command.DisconnectRequest, 0, null);
                }
                else
                {
                    ret = GXDLMS.GetHdlcFrame(Settings, (byte)Command.DisconnectRequest, null);
                }
            }
            else if (force || Settings.Connected == ConnectionState.Dlms)
            {
                ret = ReleaseRequest(force)[0];
            }
            if (GXDLMS.UseHdlc(Settings.InterfaceType))
            {
                //Restore default HDLC values.
                HdlcSettings.MaxInfoTX = InitializeMaxInfoTX;
                HdlcSettings.MaxInfoRX = InitializeMaxInfoRX;
                HdlcSettings.WindowSizeTX = InitializeWindowSizeTX;
                HdlcSettings.WindowSizeRX = InitializeWindowSizeRX;
            }
            //Restore default values.
            MaxReceivePDUSize = InitializePduSize;
            Settings.Connected = ConnectionState.None;
            Settings.ResetFrameSequence();
            Settings.Closing = true;
            return ret;
        }

        /// <summary>
        /// Returns object types.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This can be used with serialization.
        /// </remarks>
        public static Type[] GetObjectTypes()
        {
            return GXDLMS.GetObjectTypes(AvailableObjectTypes);
        }

        /// <summary>
        /// Returns object types and version numbers.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This can be used with serialization.
        /// </remarks>
        public static KeyValuePair<ObjectType, int>[] GetObjectTypes2()
        {
            return GXDLMS.GetObjectTypes2(AvailableObjectTypes);
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="ClassID">Class ID.</param>
        /// <param name="Version">Object version.</param>
        /// <param name="BaseName">Short name.</param>
        /// <param name="LN">Logical name.</param>
        /// <param name="AccessRights">Access rights.</param>
        /// <returns></returns>
        internal static GXDLMSObject CreateDLMSObject(GXDLMSSettings settings, int ClassID, object Version, int BaseName, object LN, object AccessRights, int lnVersion)
        {
            ObjectType type = (ObjectType)ClassID;
            GXDLMSObject obj = GXDLMS.CreateObject(settings, (int)type, Convert.ToByte(Version), AvailableObjectTypes);
            UpdateObjectData(obj, type, Version, BaseName, LN, AccessRights, lnVersion);
            return obj;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        GXDLMSObjectCollection ParseSNObjects(GXByteBuffer buff, bool onlyKnownObjects, bool ignoreInactiveObjects)
        {
            //Get array tag.
            byte size = buff.GetUInt8();
            //Check that data is in the array
            if (size != 0x01)
            {
                throw new GXDLMSException("Invalid response.");
            }
            GXDLMSObjectCollection items = new GXDLMSObjectCollection(this);
            long cnt = GXCommon.GetObjectCount(buff);
            GXDataInfo info = new GXDataInfo();
            for (long objPos = 0; objPos != cnt; ++objPos)
            {
                // Some meters give wrong item count.
                if (buff.Position == buff.Size)
                {
                    break;
                }
                object tmp = GXCommon.GetData(Settings, buff, info);
                List<object> objects;
                if (tmp is List<object>)
                {
                    objects = (List<object>)tmp;
                }
                else
                {
                    objects = new List<object>((object[])tmp);
                }
                info.Clear();
                if (objects.Count != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                int ot = Convert.ToUInt16(objects[1]);
                int baseName = Convert.ToInt32(objects[0]) & 0xFFFF;
                int version = Convert.ToByte(objects[2]);
                if (!onlyKnownObjects || AvailableObjectTypes.ContainsKey((ObjectType)ot))
                {
                    GXDLMSObject comp = CreateDLMSObject(Settings, ot, objects[2], baseName, objects[3], null, 2);
                    if (comp != null)
                    {
                        if (!ignoreInactiveObjects || comp.LogicalName != "0.0.127.0.0.0")
                        {
                            items.Add(comp);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Inactive object : {0} {1}", ot, baseName));
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}", ot, baseName));
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}", ot, baseName));
                }
            }
            return items;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="version"></param>
        /// <param name="baseName"></param>
        /// <param name="logicalName"></param>
        /// <param name="accessRights"></param>
        /// <param name="lnVersion"></param>
        internal static void UpdateObjectData(
            GXDLMSObject obj,
            ObjectType objectType,
            object version,
            object baseName,
            object logicalName,
            object accessRights,
            int lnVersion)
        {
            int tmp;
            obj.ObjectType = objectType;
            if (obj != null && obj.GetType() != typeof(GXDLMSObject))
            {
                //Some meters return only supported access rights.
                //All access rights are set to NoAccess.
                if (lnVersion < 3)
                {
                    for (int pos = 0; pos != ((IGXDLMSBase)obj).GetAttributeCount(); ++pos)
                    {
                        obj.SetAccess(pos + 1, AccessMode.NoAccess);
                    }
                    for (int pos = 0; pos != ((IGXDLMSBase)obj).GetMethodCount(); ++pos)
                    {
                        obj.SetMethodAccess(pos + 1, MethodAccessMode.NoAccess);
                    }
                }
                else
                {
                    for (int pos = 0; pos != ((IGXDLMSBase)obj).GetAttributeCount(); ++pos)
                    {
                        obj.SetAccess3(pos + 1, AccessMode3.NoAccess);
                    }
                    for (int pos = 0; pos != ((IGXDLMSBase)obj).GetMethodCount(); ++pos)
                    {
                        obj.SetMethodAccess3(pos + 1, MethodAccessMode3.NoAccess);
                    }
                }
            }
            // Check access rights...
            if (accessRights is List<object> && ((List<object>)accessRights).Count == 2)
            {
                //access_rights: access_right
                List<object> access = (List<object>)accessRights;
                foreach (List<object> attributeAccess in (List<object>)access[0])
                {
                    int id = Convert.ToInt32(attributeAccess[0]);
                    tmp = Convert.ToInt32(attributeAccess[1]);
                    //With some meters id is negative.
                    if (id > 0)
                    {
                        if (lnVersion < 3)
                        {
                            obj.SetAccess(id, (AccessMode)tmp);
                        }
                        else
                        {
                            obj.SetAccess3(id, (AccessMode3)tmp);
                        }
                    }
                    if (attributeAccess.Count > 2 && attributeAccess[2] != null)
                    {
                        byte value = 0;
                        foreach (object it in (GXArray)attributeAccess[2])
                        {
                            value |= (byte)(1 << Convert.ToSByte(it));
                        }
                        obj.SetAccessSelector(id, value);
                    }
                }
                if (((List<object>)access[1]).Count != 0)
                {
                    if (((List<object>)access[1])[0] is List<object>)
                    {
                        foreach (List<object> methodAccess in (List<object>)access[1])
                        {
                            int id = Convert.ToInt32(methodAccess[0]);
                            //If version is 0.
                            if (methodAccess[1] is Boolean)
                            {
                                tmp = ((Boolean)methodAccess[1]) ? 1 : 0;
                            }
                            else//If version is 1.
                            {
                                tmp = Convert.ToInt32(methodAccess[1]);
                            }
                            if (id > 0)
                            {
                                if (lnVersion < 3)
                                {
                                    obj.SetMethodAccess(id, (MethodAccessMode)tmp);
                                }
                                else
                                {
                                    obj.SetMethodAccess3(id, (MethodAccessMode3)tmp);
                                }
                            }
                        }
                    }
                    else //All versions from Actaris SL 7000 do not return collection as standard says.
                    {
                        List<object> arr = (List<object>)access[1];
                        int id = Convert.ToInt32(arr[0]) + 1;
                        //If version is 0.
                        if (arr[1] is Boolean)
                        {
                            tmp = ((Boolean)arr[1]) ? 1 : 0;
                        }
                        else //If version is 1.
                        {
                            tmp = Convert.ToInt32(arr[1]);
                        }
                        if (lnVersion < 3)
                        {
                            obj.SetMethodAccess(id, (MethodAccessMode)tmp);
                        }
                        else
                        {
                            obj.SetMethodAccess3(id, (MethodAccessMode3)tmp);
                        }
                    }
                }
            }
            if (baseName != null)
            {
                obj.ShortName = Convert.ToUInt16(baseName);
            }
            if (version != null)
            {
                obj.Version = Convert.ToInt32(version);
            }
            if (logicalName is byte[])
            {
                obj.LogicalName = GXCommon.ToLogicalName((byte[])logicalName);
            }
            else
            {
                obj.LogicalName = logicalName.ToString();
            }
        }

        /// <summary>
        /// Available objects.
        /// </summary>
        public GXDLMSObjectCollection Objects
        {
            get
            {
                return Settings.Objects;
            }
        }

        /// <summary>
        /// Parses the COSEM objects of the received data.
        /// </summary>
        /// <param name="data">Received data, from the device, as byte array. </param>
        /// <param name="onlyKnownObjects">Parse only DLMS standard objects. Manufacture specific objects are ignored.</param>
        /// <returns>Collection of COSEM objects.</returns>
        public GXDLMSObjectCollection ParseObjects(
        GXByteBuffer data,
        bool onlyKnownObjects)
        {
            return ParseObjects(data, onlyKnownObjects, true);
        }

        /// <summary>
        /// Parses the COSEM objects of the received data.
        /// </summary>
        /// <param name="data">Received data, from the device, as byte array. </param>
        /// <param name="onlyKnownObjects">Parse only DLMS standard objects. Manufacture specific objects are ignored.</param>
        /// <param name="ignoreInactiveObjects">Inactive objects are ignored.</param>
        /// <returns>Collection of COSEM objects.</returns>
        public GXDLMSObjectCollection ParseObjects(
        GXByteBuffer data,
        bool onlyKnownObjects,
        bool ignoreInactiveObjects)
        {
            if (data == null || data.Size == 0)
            {
                throw new GXDLMSException("ParseObjects failed. Invalid parameter.");
            }
            GXDLMSObjectCollection objects = null;
            if (UseLogicalNameReferencing)
            {
                objects = ParseLNObjects(data, onlyKnownObjects, ignoreInactiveObjects);
            }
            else
            {
                objects = ParseSNObjects(data, onlyKnownObjects, ignoreInactiveObjects);
            }
            if (CustomObisCodes != null)
            {
                foreach (GXObisCode it in CustomObisCodes)
                {
                    if (it.Append)
                    {
                        GXDLMSObject obj = GXDLMSClient.CreateObject(it.ObjectType);
                        obj.Version = it.Version;
                        obj.LogicalName = it.LogicalName;
                        obj.Description = it.Description;
                        objects.Add(obj);
                    }
                }
            }
            GXDLMSConverter c = new GXDLMSConverter(Standard);
            c.UpdateOBISCodeInformation(objects);
            Settings.Objects = objects;
            Settings.Objects.Parent = this;
            return objects;
        }

        /// <summary>
        /// Returns collection of push objects.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <returns>Array of objects and called indexes.</returns>
        public List<KeyValuePair<GXDLMSObject, int>> ParsePushObjects(List<object> data)
        {
            List<KeyValuePair<GXDLMSObject, int>> objects = new List<KeyValuePair<GXDLMSObject, int>>();
            if (data != null)
            {
                GXDLMSConverter c = new GXDLMSConverter(Standard);
                foreach (object tmp in data)
                {
                    List<object> it;
                    if (tmp is List<object>)
                    {
                        it = (List<object>)tmp;
                    }
                    else
                    {
                        it = new List<object>((object[])tmp);
                    }
                    int classID = ((UInt16)(it[0])) & 0xFFFF;
                    if (classID > 0)
                    {
                        GXDLMSObject comp;
                        comp = this.Objects.FindByLN((ObjectType)classID, GXCommon.ToLogicalName(it[1] as byte[]));
                        if (comp == null)
                        {
                            comp = GXDLMSClient.CreateDLMSObject(Settings, classID, 0, 0, it[1], null, 2);
                            c.UpdateOBISCodeInformation(comp);
                        }
                        if ((comp is IGXDLMSBase))
                        {
                            objects.Add(new KeyValuePair<GXDLMSObject, int>(comp, (sbyte)it[2]));
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}",
                                classID, GXCommon.ToLogicalName((byte[])it[1])));
                        }
                    }
                }
            }
            return objects;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        GXDLMSObjectCollection ParseLNObjects(
        GXByteBuffer buff,
        bool onlyKnownObjects,
        bool ignoreInactiveObjects)
        {
            byte size = buff.GetUInt8();
            //Check that data is in the array.
            if (size != 0x01)
            {
                throw new GXDLMSException("Invalid response.");
            }
            //get object count
            int cnt = GXCommon.GetObjectCount(buff);
            int objectCnt = 0;
            GXDLMSObjectCollection items = new GXDLMSObjectCollection(this);
            GXDataInfo info = new GXDataInfo();
            int lnVersion = 2;
            //Find LN Version because some meters don't add LN Association the first object.
            int pos = buff.Position;
            while (buff.Position != buff.Size && cnt != objectCnt)
            {
                info.Clear();
                object tmp = GXCommon.GetData(Settings, buff, info);
                List<object> objects;
                if (tmp is List<object>)
                {
                    objects = (List<object>)tmp;
                }
                else
                {
                    objects = new List<object>((object[])tmp);
                }
                if (objects.Count != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                ++objectCnt;
                int ot = Convert.ToInt16(objects[0]);
                //Get LN association version.
                if (ot == (int)ObjectType.AssociationLogicalName
                     && GXCommon.ToLogicalName((byte[])objects[2]) == "0.0.40.0.0.255")
                {
                    lnVersion = Convert.ToInt32(objects[1]);
                    break;
                }
            }
            objectCnt = 0;
            buff.Position = pos;
            //Some meters give wrong item count.
            while (buff.Position != buff.Size && cnt != objectCnt)
            {
                info.Clear();
                object tmp = GXCommon.GetData(Settings, buff, info);
                List<object> objects;
                if (tmp is List<object>)
                {
                    objects = (List<object>)tmp;
                }
                else
                {
                    objects = new List<object>((object[])tmp);
                }
                if (objects.Count != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                ++objectCnt;
                UInt16 ot = Convert.ToUInt16(objects[0]);
                int version = Convert.ToByte(objects[1]);
                if (!onlyKnownObjects || AvailableObjectTypes.ContainsKey((ObjectType)ot))
                {
                    GXDLMSObject comp = CreateDLMSObject(Settings, ot, objects[1], 0, objects[2], objects[3], lnVersion);
                    if (comp != null)
                    {
                        if ((!ignoreInactiveObjects || comp.LogicalName != "0.0.127.0.0.0"))
                        {
                            items.Add(comp);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Inactive object : {0} {1}", ot, comp.LogicalName));
                        }
                    }
                }
                else
                {
                    GXDLMSObject comp = CreateDLMSObject(Settings, ot, objects[1], 0, objects[2], objects[3], lnVersion);
                    if (comp != null && comp.GetType() != typeof(GXDLMSObject))
                    {
                        items.Add(comp);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}", ot, GXCommon.ToLogicalName((byte[])objects[2])));
                    }
                }
            }
            if (cnt != objectCnt)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Association expect size is {0} and actual size is {1}", cnt, objectCnt));
            }
            return items;
        }


        /// <summary>
        /// Get Value from byte array received from the meter.
        /// </summary>
        public object UpdateValue(
            GXDLMSObject target,
            int attributeIndex,
            object value)
        {
            return UpdateValue(target, attributeIndex, value, null);
        }

        /// <summary>
        /// Get Value from byte array received from the meter.
        /// </summary>
        public object UpdateValue(
            GXDLMSObject target,
            int attributeIndex,
            object value,
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> columns)
        {
            //Update data type if value is readable.
            if (value != null && target.GetDataType(attributeIndex) == DataType.None)
            {
                try
                {
                    target.SetDataType(attributeIndex, GXCommon.GetDLMSDataType(value.GetType()));
                }
                catch (Exception)
                {
                    //It's ok if this fails.
                }
            }
            if (value is byte[])
            {
                DataType type = target.GetUIDataType(attributeIndex);
                if (type != DataType.None)
                {
                    if (type == DataType.DateTime && ((byte[])value).Length == 5)
                    {
                        type = DataType.Date;
                        target.SetUIDataType(attributeIndex, type);
                    }
                    value = ChangeType((byte[])value, type, UseUtc2NormalTime);
                }
            }
            ValueEventArgs e = new ValueEventArgs(Settings, target, attributeIndex, 0, columns);
            e.Value = value;
            (target as IGXDLMSBase).SetValue(Settings, e);
            target.ClearDirty(e.Index);
            return target.GetValues()[attributeIndex - 1];
        }

        /// <summary>
        /// Update list values.
        /// </summary>
        /// <param name="list">COSEM objects to update.</param>
        /// <param name="values">Received values.</param>
        public void UpdateValues(List<KeyValuePair<GXDLMSObject, int>> list, List<object> values)
        {
            int pos = 0;
            foreach (KeyValuePair<GXDLMSObject, int> it in list)
            {
                ValueEventArgs e = new ValueEventArgs(Settings, it.Key, it.Value, 0, null);
                e.Value = values[pos];
                DataType type;
                if (e.Value is byte[] && (type = it.Key.GetUIDataType(it.Value)) != DataType.None)
                {
                    e.Value = GXDLMSClient.ChangeType((byte[])e.Value, type, UseUtc2NormalTime);
                }
                (it.Key as IGXDLMSBase).SetValue(Settings, e);
                it.Key.ClearDirty(e.Index);
                ++pos;
            }
        }

        /// <summary>
        /// Parse access response.
        /// </summary>
        /// <param name="list">Collection of access items.</param>
        /// <param name="data">Received data from the meter.</param>
        /// <seealso cref="AccessRequest"/>
        public void ParseAccessResponse(
            List<GXDLMSAccessItem> list,
            GXByteBuffer data)
        {
            //Get count
            GXDataInfo info = new GXDataInfo();
            int cnt = GXCommon.GetObjectCount(data);
            if (list.Count != cnt)
            {
                throw new ArgumentOutOfRangeException("List size and values size do not match.");
            }
            foreach (GXDLMSAccessItem it in list)
            {
                info.Clear();
                it.Value = GXCommon.GetData(Settings, data, info);
            }
            //Get status codes.
            cnt = GXCommon.GetObjectCount(data);
            if (list.Count != cnt)
            {
                throw new ArgumentOutOfRangeException("List size and values size do not match.");
            }
            foreach (GXDLMSAccessItem it in list)
            {
                //Get access type.
                data.GetUInt8();
                //Get status.
                it.Error = (ErrorCode)data.GetUInt8();
                if (it.Command == AccessServiceCommandType.Get && it.Error == ErrorCode.Ok)
                {
                    UpdateValue(it.Target, it.Index, it.Value);
                }
            }
        }

        public static GXDLMSAttributeSettings GetAttributeInfo(
            GXDLMSObject item,
            int index)
        {
            GXDLMSAttributeSettings att = item.Attributes.Find(index);
            return att;
        }


        /// <summary>
        /// Changes byte array received from the meter to given type.
        /// </summary>
        /// <param name="value">Byte array received from the meter.</param>
        /// <param name="type">Wanted type.</param>
        /// <returns>Value changed by type.</returns>
        public static object ChangeType(byte[] value, DataType type)
        {
            return ChangeType(new GXByteBuffer(value), type, false);
        }

        /// <summary>
        /// Changes byte array received from the meter to given type.
        /// </summary>
        /// <param name="value">Byte array received from the meter.</param>
        /// <param name="type">Wanted type.</param>
        /// <param name="useUtc">Standard says that Time zone is from normal time to UTC in minutes.
        /// If meter is configured to use UTC time (UTC to normal time) set this to true.</param>
        /// <returns>Value changed by type.</returns>
        public static object ChangeType(byte[] value, DataType type, bool useUtc)
        {
            return ChangeType(new GXByteBuffer(value), type, useUtc);
        }

        /// <summary>
        /// Changes byte array received from the meter to given type.
        /// </summary>
        /// <param name="value">Byte array received from the meter.</param>
        /// <param name="type">Wanted type.</param>
        /// <returns>Value changed by type.</returns>
        public object ChangeType(GXByteBuffer value, DataType type)
        {
            return ChangeType(value, type, false);
        }

        /// <summary>
        /// Changes byte array received from the meter to given type.
        /// </summary>
        /// <param name="value">Byte array received from the meter.</param>
        /// <param name="type">Wanted type.</param>
        /// <param name="useUtc">Standard says that Time zone is from normal time to UTC in minutes.
        /// If meter is configured to use UTC time (UTC to normal time) set this to true.</param>
        /// <returns>Value changed by type.</returns>
        public static object ChangeType(GXByteBuffer value, DataType type, bool useUtc)
        {
            if ((value == null || value.Size == 0) && (type == DataType.String || type == DataType.OctetString))
            {
                return string.Empty;
            }
            if (value == null)
            {
                return null;
            }
            if (type == DataType.None)
            {
                return BitConverter.ToString(value.Data, value.Position, value.Size - value.Position).Replace('-', ' ');
            }
            if (type == DataType.OctetString)
            {
                return value;
            }
            if (type == DataType.String && !value.IsAsciiString())
            {
                return value;
            }
            if (value.Size == 0
                    && (type == DataType.String || type == DataType.OctetString))
            {
                return "";
            }
            if (value.Size == 0 && type == DataType.DateTime)
            {
                return new GXDateTime(DateTime.MinValue);
            }
            if (value.Size == 0 && type == DataType.Date)
            {
                return new GXDate(DateTime.MinValue);
            }
            if (value.Size == 0 && type == DataType.Time)
            {
                return new GXTime(DateTime.MinValue);
            }

            GXDataInfo info = new GXDataInfo();
            info.Type = type;
            GXDLMSSettings settings = new GXDLMSSettings(false, InterfaceType.HDLC);
            settings.UseUtc2NormalTime = useUtc;
            Object ret = GXCommon.GetData(settings, value, info);
            if (!info.Complete)
            {
                throw new OutOfMemoryException();
            }
            return ret;
        }

        /// <summary>
        /// Reads the selected object from the device.
        /// </summary>
        /// <remarks>
        /// This method is used to get all registers in the device.
        /// </remarks>
        /// <returns>Read request, as byte array.</returns>
        public byte[][] GetObjectsRequest()
        {
            return GetObjectsRequest(null);
        }

        /// <summary>
        /// Reads the selected object from the device.
        /// </summary>
        /// <remarks>
        /// This method is used to get all registers in the device.
        /// </remarks>
        /// <returns>Read request, as byte array.</returns>
        public byte[][] GetObjectsRequest(string ln)
        {
            object name;
            Settings.ResetBlockIndex();
            if (UseLogicalNameReferencing)
            {
                if (string.IsNullOrEmpty(ln))
                {
                    name = "0.0.40.0.0.255";
                }
                else
                {
                    name = ln;
                }
            }
            else
            {
                name = (ushort)0xFA00;
            }
            return Read(name, ObjectType.AssociationLogicalName, 2, null, 0);
        }

        /// <summary>
        /// Generate Method (Action) request.
        /// </summary>
        /// <param name="item">object to write.</param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <returns></returns>
        public byte[][] Method(GXDLMSObject item, int index, Object data)
        {
            return Method(item.Name, item.ObjectType, index, data, DataType.None);
        }

        /// <summary>
        /// Generate Method (Action) request.
        /// </summary>
        /// <param name="item">object to write.</param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <param name="data">Additional data.</param>
        /// <param name="type">Additional data type.</param>
        /// <returns></returns>
        public byte[][] Method(GXDLMSObject item, int index, Object data, DataType type)
        {
            int mode = (int)item.GetMethodAccess3(index);
            return Method(item.Name, item.ObjectType, index, data, type, mode);
        }

        /// <summary>
        /// Generate Method (Action) request.
        /// </summary>
        /// <param name="name">Method object short name or Logical Name.</param>
        /// <param name="objectType">Object type.</param>
        /// <param name="index">Method index.</param>
        /// <param name="value">Additional data.</param>
        /// <param name="type">Additional data type.</param>
        /// <returns></returns>
        public byte[][] Method(object name, ObjectType objectType, int index, Object value, DataType type)
        {
            return Method(name, objectType, index, value, type, 0);
        }

        /// <summary>
        /// Generate Method (Action) request.
        /// </summary>
        /// <param name="name">Method object short name or Logical Name.</param>
        /// <param name="objectType">Object type.</param>
        /// <param name="index">Method index.</param>
        /// <param name="value">Additional data.</param>
        /// <param name="type">Additional data type.</param>
        /// <returns></returns>
        private byte[][] Method(object name, ObjectType objectType, int index, Object value, DataType type, int mode)
        {
            if (name == null || (index < 1 && Standard != Standard.Italy))
            {
                throw new ArgumentOutOfRangeException("Invalid parameter");
            }
            Settings.ResetBlockIndex();
            if (type == DataType.None && value != null)
            {
                type = GXDLMSConverter.GetDLMSDataType(value);
            }
            GXByteBuffer attributeDescriptor = new GXByteBuffer();
            GXByteBuffer data = new GXByteBuffer();
            GXCommon.SetData(Settings, data, type, value);
            if (UseLogicalNameReferencing)
            {
                // CI
                attributeDescriptor.SetUInt16((UInt16)objectType);
                // Add LN
                attributeDescriptor.Set(GXCommon.LogicalNameToBytes((string)name));
                // Attribute ID.
                attributeDescriptor.SetUInt8((byte)index);
                // Method Invocation Parameters is not used.
                if (type == DataType.None)
                {
                    attributeDescriptor.SetUInt8(0);
                }
                else
                {
                    attributeDescriptor.SetUInt8(1);
                }
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.MethodRequest,
                    (byte)ActionRequestType.Normal, attributeDescriptor, data, 0xff, Command.None);
                p.AccessMode = mode;
                //GBT Window size or streaming is not used with method because there is no information available from the
                //GBT block number and client doesn't know when ACK is expected.
                return GXDLMS.GetLnMessages(p);
            }
            else
            {
                int ind, count;
                GXDLMS.GetActionInfo(objectType, out ind, out count);
                if (index > count)
                {
                    throw new ArgumentException("methodIndex");
                }
                UInt16 sn = Convert.ToUInt16(name);
                index = (ind + (index - 1) * 0x8);
                sn += (UInt16)index;
                // Add name.
                attributeDescriptor.SetUInt16(sn);
                // Add selector.
                if (type != DataType.None)
                {
                    attributeDescriptor.SetUInt8(1);
                }
                return GXDLMS.GetSnMessages(new GXDLMSSNParameters(Settings, Command.WriteRequest, 1, (byte)VariableAccessSpecification.VariableName, attributeDescriptor, data));
            }
        }

        /// <summary>
        /// Generates a write message.
        /// </summary>
        /// <param name="item">object to write.</param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <returns></returns>
        public byte[][] Write(
            GXDLMSObject item,
            int index)
        {
            if (item == null || index < 1)
            {
                throw new GXDLMSException("Invalid parameter");
            }
            Object value = (item as IGXDLMSBase).GetValue(Settings, new ValueEventArgs(Settings, item, index, 0, null));
            DataType type = item.GetDataType(index);
            if (type == DataType.None)
            {
                type = GXDLMSConverter.GetDLMSDataType(value);
            }
            //If values is show as string, but send as byte array.
            if (value is string && type == DataType.OctetString)
            {
                DataType tp = item.GetUIDataType(index);
                if (tp == DataType.String)
                {
                    value = ASCIIEncoding.ASCII.GetBytes((string)value);
                }
            }
            int mode = (int)item.GetAccess3(index);
            return Write2(item.Name, value, type, item.ObjectType, index, mode);
        }

        /// <summary>
        /// Generates a write message.
        /// </summary>
        /// <param name="name">Short or Logical Name.</param>
        /// <param name="value">Data to Write.</param>
        /// <param name="type">Data type of write object.</param>
        /// <param name="objectType"></param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <returns>Generated write frames.</returns>
        [Obsolete("Use Write")]
        public byte[][] Write(object name, object value, DataType type, ObjectType objectType, int index)
        {
            return Write2(name, value, type, objectType, index, 0);
        }

        private byte[][] Write2(object name, object value, DataType type, ObjectType objectType, int index, int mode)
        {
            Settings.ResetBlockIndex();
            if (type == DataType.None && value != null)
            {
                type = GXDLMSConverter.GetDLMSDataType(value);
                if (type == DataType.None)
                {
                    throw new ArgumentException("Invalid parameter. Unknown value type.");
                }
            }
            GXByteBuffer attributeDescriptor = new GXByteBuffer();
            GXByteBuffer data = new GXByteBuffer();
            byte[][] reply;
            GXCommon.SetData(Settings, data, type, value);
            if (UseLogicalNameReferencing)
            {
                // Add CI.
                attributeDescriptor.SetUInt16((UInt16)objectType);
                // Add LN.
                attributeDescriptor.Set(GXCommon.LogicalNameToBytes((string)name));
                // Attribute ID.
                attributeDescriptor.SetUInt8((byte)index);
                // Access selection is not used.
                attributeDescriptor.SetUInt8(0);
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.SetRequest, (byte)SetRequestType.Normal, attributeDescriptor, data, 0xff, Command.None);
                p.AccessMode = mode;
                p.blockIndex = Settings.BlockIndex;
                p.blockNumberAck = Settings.BlockNumberAck;
                p.Streaming = false;
                //GBT Window size or streaming is not used with write because there is no information available from the
                //GBT block number and client doesn't know when ACK is expected.
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                // Add name.
                UInt16 sn = Convert.ToUInt16(name);
                sn += (UInt16)((index - 1) * 8);
                attributeDescriptor.SetUInt16(sn);
                //Data cnt.
                attributeDescriptor.SetUInt8(1);
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.WriteRequest, 1,
                        (byte)VariableAccessSpecification.VariableName, attributeDescriptor, data);
                reply = GXDLMS.GetSnMessages(p);
            }
            return reply;
        }

        /// <summary>
        /// Generates a read message.
        /// </summary>
        /// <param name="name">Short or Logical Name.</param>
        /// <param name="objectType">Read Interface.</param>
        /// <param name="attributeOrdinal">Read attribute index.</param>
        /// <returns>Read request as byte array.</returns>
        [Obsolete("Use Read")]
        public byte[][] Read(object name, ObjectType objectType, int attributeOrdinal)
        {
            return Read(name, objectType, attributeOrdinal, null, 0);
        }

        private byte[][] Read(object name, ObjectType objectType, int attributeOrdinal, GXByteBuffer data, int mode)
        {
            if ((attributeOrdinal < 0))
            {
                throw new ArgumentException("Invalid parameter");
            }
            Settings.ResetBlockIndex();
            GXByteBuffer attributeDescriptor = new GXByteBuffer();
            byte[][] reply;
            if (UseLogicalNameReferencing)
            {
                // CI
                attributeDescriptor.SetUInt16((UInt16)objectType);
                // Add LN
                attributeDescriptor.Set(GXCommon.LogicalNameToBytes((string)name));
                // Attribute ID.
                attributeDescriptor.SetUInt8((byte)attributeOrdinal);
                if (data == null || data.Size == 0)
                {
                    // Access selection is not used.
                    attributeDescriptor.SetUInt8(0);
                }
                else
                {
                    // Access selection is used.
                    attributeDescriptor.SetUInt8(1);
                }
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.GetRequest, (byte)GetCommandType.Normal, attributeDescriptor, data, 0xff, Command.None);
                p.AccessMode = mode;
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                byte requestType;
                UInt16 sn = Convert.ToUInt16(name);
                sn += (UInt16)((attributeOrdinal - 1) * 8);
                attributeDescriptor.SetUInt16(sn);
                // parameterized-access
                if (data != null && data.Size != 0)
                {
                    requestType = (byte)VariableAccessSpecification.ParameterisedAccess;
                }
                else //variable-name
                {
                    requestType = (byte)VariableAccessSpecification.VariableName;
                }
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.ReadRequest, 1, requestType, attributeDescriptor, data);
                reply = GXDLMS.GetSnMessages(p);
            }
            return reply;
        }

        /// <summary>
        /// Generates a read message.
        /// </summary>
        /// <param name="item">DLMS object to read.</param>
        /// <param name="attributeOrdinal">Read attribute index.</param>
        /// <returns>Read request as byte array.</returns>
        public byte[][] Read(GXDLMSObject item, int attributeOrdinal)
        {
            int mode = (int)item.GetAccess3(attributeOrdinal);
            return Read(item.Name, item.ObjectType, attributeOrdinal, null, mode);
        }

        /// <summary>
        /// Read list of COSEM objects.
        /// </summary>
        /// <param name="list">List of COSEM object and attribute index to read.</param>
        /// <returns>Read List request as byte array.</returns>
        public byte[][] ReadList(List<KeyValuePair<GXDLMSObject, int>> list)
        {
            if ((NegotiatedConformance & Conformance.MultipleReferences) == 0)
            {
                throw new ArgumentOutOfRangeException("Meter doesn't support multiple objects reading with one request.");
            }
            if (list == null || list.Count == 0)
            {
                throw new ArgumentOutOfRangeException("Invalid parameter.");
            }
            Settings.ResetBlockIndex();
            List<byte[]> messages = new List<byte[]>();
            GXByteBuffer data = new GXByteBuffer();
            if (this.UseLogicalNameReferencing)
            {
                //Find highest access mode.
                int mode = 0;
                foreach (var it in list)
                {
                    int m = (int)it.Key.GetAccess3(it.Value);
                    if (m > mode)
                    {
                        mode = m;
                    }
                }
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.GetRequest, (byte)GetCommandType.WithList, data, null, 0xff, Command.None);
                p.AccessMode = mode;
                //Request service primitive shall always fit in a single APDU.
                int pos = 0, count = (Settings.MaxPduSize - 12) / 10;
                if (list.Count < count)
                {
                    count = list.Count;
                }
                //All meters can handle 10 items.
                if (count > 10)
                {
                    count = 10;
                }
                // Add length.
                GXCommon.SetObjectCount(count, data);
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    // CI.
                    data.SetUInt16((UInt16)it.Key.ObjectType);
                    String[] items = it.Key.LogicalName.Split('.');
                    if (items.Length != 6)
                    {
                        throw new ArgumentException("Invalid Logical Name.");
                    }
                    foreach (String it2 in items)
                    {
                        data.SetUInt8(byte.Parse(it2));
                    }
                    // Attribute ID.
                    data.SetUInt8((byte)it.Value);
                    // Attribute selector is not used.
                    data.SetUInt8(0);
                    ++pos;
                    if (pos % count == 0 && list.Count != pos)
                    {
                        messages.AddRange(GXDLMS.GetLnMessages(p));
                        data.Clear();
                        if (list.Count - pos < count)
                        {
                            GXCommon.SetObjectCount(list.Count - pos, data);
                        }
                        else
                        {
                            GXCommon.SetObjectCount(count, data);
                        }
                    }
                }
                messages.AddRange(GXDLMS.GetLnMessages(p));
            }
            else
            {
                if (list.Count == 1)
                {
                    return Read(list[0].Key, list[0].Value);
                }
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.ReadRequest, list.Count, 0xFF, data, null);
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    // Add variable type.
                    data.SetUInt8(VariableAccessSpecification.VariableName);
                    int sn = it.Key.ShortName;
                    sn += (it.Value - 1) * 8;
                    data.SetUInt16((UInt16)sn);
                    if (data.Size >= Settings.MaxPduSize)
                    {
                        messages.AddRange(GXDLMS.GetSnMessages(p));
                        data.Clear();
                    }
                }
                messages.AddRange(GXDLMS.GetSnMessages(p));
            }
            return messages.ToArray();
        }

        /// <summary>
        /// Write list of COSEM objects.
        /// </summary>
        /// <param name="list">List of COSEM object and attribute index to read.</param>
        /// <returns>Write List request as byte array.</returns>
        public byte[][] WriteList(List<KeyValuePair<GXDLMSObject, int>> list)
        {
            if ((NegotiatedConformance & Conformance.MultipleReferences) == 0)
            {
                throw new ArgumentOutOfRangeException("Meter doesn't support multiple objects writing with one request.");
            }
            if (list == null || list.Count == 0)
            {
                throw new ArgumentOutOfRangeException("Invalid parameter.");
            }
            //Find highest access mode.
            int mode = 0;
            foreach (var it in list)
            {
                int m = (int)it.Key.GetAccess3(it.Value);
                if (m > mode)
                {
                    mode = m;
                }
            }

            Settings.ResetBlockIndex();
            List<byte[]> messages = new List<byte[]>();
            GXByteBuffer data = new GXByteBuffer();
            if (UseLogicalNameReferencing)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.SetRequest, (byte)SetCommandType.WithList, null, data, 0xff, Command.None);
                p.AccessMode = mode;
                // Add length.
                GXCommon.SetObjectCount(list.Count, data);
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    // CI.
                    data.SetUInt16((UInt16)it.Key.ObjectType);
                    data.Set(GXCommon.LogicalNameToBytes(it.Key.LogicalName));
                    // Attribute ID.
                    data.SetUInt8((byte)it.Value);
                    // Attribute selector is not used.
                    data.SetUInt8(0);
                }
                // Add length.
                GXCommon.SetObjectCount(list.Count, data);
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    Object value = (it.Key as IGXDLMSBase).GetValue(Settings, new ValueEventArgs(Settings, it.Key, it.Value, 0, null));
                    DataType type = it.Key.GetDataType(it.Value);
                    if (type == DataType.None)
                    {
                        type = GXDLMSConverter.GetDLMSDataType(value);
                    }
                    //If values is show as string, but send as byte array.
                    if (value is string && type == DataType.OctetString)
                    {
                        DataType tp = it.Key.GetUIDataType(it.Value);
                        if (tp == DataType.String)
                        {
                            value = ASCIIEncoding.ASCII.GetBytes((string)value);
                        }
                    }
                    GXCommon.SetData(Settings, data, type, value);
                }
                messages.AddRange(GXDLMS.GetLnMessages(p));
            }
            else
            {
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.WriteRequest, list.Count, 0xFF, null, data);
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    // Add variable type.
                    data.SetUInt8(VariableAccessSpecification.VariableName);
                    int sn = it.Key.ShortName;
                    sn += (it.Value - 1) * 8;
                    data.SetUInt16((UInt16)sn);
                }
                // Add length.
                GXCommon.SetObjectCount(list.Count, data);
                p.count = list.Count;
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    Object value = (it.Key as IGXDLMSBase).GetValue(Settings, new ValueEventArgs(Settings, it.Key, it.Value, 0, null));
                    DataType type = it.Key.GetDataType(it.Value);
                    if (type == DataType.None)
                    {
                        type = GXDLMSConverter.GetDLMSDataType(value);
                    }
                    //If values is show as string, but send as byte array.
                    if (value is string && type == DataType.OctetString)
                    {
                        DataType tp = it.Key.GetUIDataType(it.Value);
                        if (tp == DataType.String)
                        {
                            value = ASCIIEncoding.ASCII.GetBytes((string)value);
                        }
                    }
                    GXCommon.SetData(Settings, data, type, value);
                }
                messages.AddRange(GXDLMS.GetSnMessages(p));
            }
            return messages.ToArray();
        }

        /// <summary>
        /// Generates the keep alive message.
        /// </summary>
        /// <remarks>
        /// Keepalive message is needed to keep the connection up. Connection is closed if keepalive is not sent in meter's inactivity timeout period.
        /// </remarks>
        /// <returns>Returns Keep alive message, as byte array.</returns>
        public byte[] GetKeepAlive()
        {
            Settings.ResetBlockIndex();
            if (UseLogicalNameReferencing)
            {
                GXDLMSAssociationLogicalName ln = new GXDLMSAssociationLogicalName();
                return Read(ln, 1)[0];
            }
            GXDLMSAssociationShortName sn = new GXDLMSAssociationShortName();
            return Read(sn, 1)[0];
        }

        /// <summary>
        /// Read rows by entry.
        /// </summary>
        /// <remarks>
        /// Check Conformance because all meters do not support this.
        /// </remarks>
        /// <param name="pg">Profile generic object to read.</param>
        /// <param name="index">One based start index.</param>
        /// <param name="count">Rows count to read.</param>
        /// <returns>Read message as byte array.</returns>
        public byte[][] ReadRowsByEntry(GXDLMSProfileGeneric pg, UInt32 index, UInt32 count)
        {
            return ReadRowsByEntry(pg, index, count, null);
        }

        /// <summary>
        /// Read rows by entry.
        /// </summary>
        /// <remarks>
        /// Check Conformance because all meters do not support this.
        /// </remarks>
        /// <param name="pg">Profile generic object to read.</param>
        /// <param name="index">One based start index.</param>
        /// <param name="count">Rows count to read.</param>
        /// <param name="columns">Columns to read.</param>
        /// <returns>Read message as byte array.</returns>
        public byte[][] ReadRowsByEntry(GXDLMSProfileGeneric pg, UInt32 index, UInt32 count,
                                        List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> columns)
        {
            int columnIndex = 1;
            int columnEnd = 0;
            int pos = 0;
            // If columns are given find indexes.
            if (columns != null && columns.Count != 0)
            {
                if (pg.CaptureObjects == null || pg.CaptureObjects.Count == 0)
                {
                    throw new Exception("Read capture objects first.");
                }
                columnIndex = pg.CaptureObjects.Count;
                columnEnd = 1;
                foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> c in columns)
                {
                    pos = 0;
                    bool found = false;
                    foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in pg.CaptureObjects)
                    {
                        ++pos;
                        if (it.Key.ObjectType == c.Key.ObjectType
                                && it.Key.LogicalName.CompareTo(c.Key.LogicalName) == 0
                                && it.Value.AttributeIndex == c.Value.AttributeIndex
                                && it.Value.DataIndex == c.Value.DataIndex)
                        {
                            found = true;
                            if (pos < columnIndex)
                            {
                                columnIndex = pos;
                            }
                            columnEnd = pos;
                            break;
                        }
                    }
                    if (!found)
                    {
                        throw new Exception("Invalid column: " + c.Key.LogicalName);
                    }
                }
            }
            return ReadRowsByEntry(pg, index, count, columnIndex, columnEnd);
        }

        /// <summary>
        /// Read rows by entry.
        /// </summary>
        /// <remarks>
        /// Check Conformance because all meters do not support this.
        /// </remarks>
        /// <param name="pg">Profile generic object to read.</param>
        /// <param name="index">One based start index.</param>
        /// <param name="count">Rows count to read.</param>
        /// <param name="columnStart">One based column start index.</param>
        /// <param name="columnEnd">Column end index.</param>
        /// <returns>Read message as byte array.</returns>
        public byte[][] ReadRowsByEntry(GXDLMSProfileGeneric pg, UInt32 index, UInt32 count, int columnStart, int columnEnd)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (columnStart < 1)
            {
                throw new ArgumentOutOfRangeException("columnStart");
            }
            if (columnEnd < 0)
            {
                throw new ArgumentOutOfRangeException("columnEnd");
            }
            if (pg.CaptureObjects.Count == 0)
            {
                throw new Exception("Capture objects not read.");
            }
            pg.Buffer.Clear();
            Settings.ResetBlockIndex();
            GXByteBuffer buff = new GXByteBuffer(19);
            // Add AccessSelector value
            buff.SetUInt8(0x02);
            // Add enum tag.
            buff.SetUInt8((byte)DataType.Structure);
            // Add item count
            buff.SetUInt8(0x04);
            // Add start index
            GXCommon.SetData(Settings, buff, DataType.UInt32, index);
            // Add Count
            if (count == 0)
            {
                GXCommon.SetData(Settings, buff, DataType.UInt32, count);
            }
            else
            {
                GXCommon.SetData(Settings, buff, DataType.UInt32, index + count - 1);
            }
            // Select columns to read.
            GXCommon.SetData(Settings, buff, DataType.UInt16, columnStart);
            GXCommon.SetData(Settings, buff, DataType.UInt16, columnEnd);
            int mode = (int)pg.GetAccess3(2);
            return Read(pg.Name, ObjectType.ProfileGeneric, 2, buff, mode);
        }

        /// <summary>
        /// Read rows by range.
        /// </summary>
        /// <remarks>
        /// Use this method to read Profile Generic table between dates.
        /// Check Conformance because all meters do not support this.
        /// Some meters return error if there are no data betweens start and end time.
        /// </remarks>
        /// <param name="pg">Profile generic object to read.</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        /// <returns>Read message as byte array.</returns>
        public byte[][] ReadRowsByRange(GXDLMSProfileGeneric pg, DateTime start, DateTime end)
        {
            return ReadRowsByRange(pg, start, end, null);
        }

        /// <summary>
        /// Read rows by range.
        /// </summary>
        /// <remarks>
        /// Use this method to read Profile Generic table between dates.
        /// Check Conformance because all meters do not support this.
        /// Some meters return error if there are no data betweens start and end time.
        /// </remarks>
        /// <param name="pg">Profile generic object to read.</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        /// <returns>Read message as byte array.</returns>
        public byte[][] ReadRowsByRange(GXDLMSProfileGeneric pg, GXDateTime start, GXDateTime end)
        {
            return ReadRowsByRange(pg, start, end, null);
        }

        /// <summary>
        /// Read rows by range.
        /// </summary>
        /// <remarks>
        /// Use this method to read Profile Generic table between dates.
        /// Check Conformance because all meters do not support this.
        /// Some meters return error if there are no data betweens start and end time.
        /// </remarks>
        /// <param name="pg">Profile generic object to read.</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        /// <param name="columns">Columns to read.</param>
        /// <returns>Read message as byte array.</returns>
        public byte[][] ReadRowsByRange(GXDLMSProfileGeneric pg, DateTime start, DateTime end,
                                        List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> columns)
        {
            return ReadRowsByRange(pg, new GXDateTime(start), new GXDateTime(end), columns);
        }

        /// <summary>
        /// Read rows by range.
        /// </summary>
        /// <remarks>
        /// Use this method to read Profile Generic table between dates.
        /// Check Conformance because all meters do not support this.
        /// Some meters return error if there are no data betweens start and end time.
        /// </remarks>
        /// <param name="pg">Profile generic object to read.</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        /// <param name="columns">Columns to read.</param>
        /// <returns>Read message as byte array.</returns>
        public byte[][] ReadRowsByRange(GXDLMSProfileGeneric pg, GXDateTime start, GXDateTime end,
                                        List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> columns)
        {

            if (pg.CaptureObjects.Count == 0)
            {
                throw new Exception("Capture objects not read.");
            }
            pg.Buffer.Clear();
            Settings.ResetBlockIndex();
            GXDLMSObject sort = pg.SortObject;
            if (sort == null)
            {
                sort = pg.CaptureObjects[0].Key;
            }
            string ln = "0.0.1.0.0.255";
            ObjectType type = ObjectType.Clock;
            ClockType clockType = ClockType.Clock;
            //If Unix time is used.
            if (sort is GXDLMSData && sort.LogicalName == "0.0.1.1.0.255")
            {
                clockType = ClockType.Unix;
                ln = "0.0.1.1.0.255";
                type = ObjectType.Data;
            }
            //If high resolution time is used.
            else if (sort is GXDLMSData && sort.LogicalName == "0.0.1.2.0.255")
            {
                clockType = ClockType.HighResolution;
                ln = "0.0.1.2.0.255";
                type = ObjectType.Data;
            }
            GXByteBuffer buff = new GXByteBuffer(51);
            // Add AccessSelector value.
            buff.SetUInt8(0x01);
            // Add enum tag.
            buff.SetUInt8((byte)DataType.Structure);
            // Add item count
            buff.SetUInt8(0x04);
            // Add enum tag.
            buff.SetUInt8((byte)DataType.Structure);
            // Add item count
            buff.SetUInt8(0x04);
            // CI
            GXCommon.SetData(Settings, buff, DataType.UInt16,
                             type);
            // LN
            GXCommon.SetData(Settings, buff, DataType.OctetString, GXCommon.LogicalNameToBytes(ln));
            // Add attribute index.
            GXCommon.SetData(Settings, buff, DataType.Int8, 2);
            // Add data index.
            GXCommon.SetData(Settings, buff, DataType.UInt16, 0);
            if (clockType == ClockType.Clock)
            {
                // Add start time.
                GXCommon.SetData(Settings, buff, DataType.OctetString, start);
                // Add end time.
                GXCommon.SetData(Settings, buff, DataType.OctetString, end);
            }
            else if (clockType == ClockType.Unix)
            {
                // Add start time.
                GXCommon.SetData(Settings, buff, DataType.UInt32, GXDateTime.ToUnixTime(start));
                // Add end time.
                GXCommon.SetData(Settings, buff, DataType.UInt32, GXDateTime.ToUnixTime(end));
            }
            else if (clockType == ClockType.HighResolution)
            {
                // Add start time.
                GXCommon.SetData(Settings, buff, DataType.UInt64, GXDateTime.ToHighResolutionTime(start));
                // Add end time.
                GXCommon.SetData(Settings, buff, DataType.UInt64, GXDateTime.ToHighResolutionTime(end));
            }

            // Add array of read columns.
            buff.SetUInt8(DataType.Array);
            if (columns == null)
            {
                // Add item count
                buff.SetUInt8(0x00);
            }
            else
            {
                GXCommon.SetObjectCount(columns.Count, buff);
                foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in columns)
                {
                    buff.SetUInt8(DataType.Structure);
                    // Add items count.
                    buff.SetUInt8(4);
                    // CI
                    GXCommon.SetData(Settings, buff, DataType.UInt16, it.Key.ObjectType);
                    // LN
                    GXCommon.SetData(Settings, buff, DataType.OctetString, GXCommon.LogicalNameToBytes(it.Key.LogicalName));
                    // Add attribute index.
                    GXCommon.SetData(Settings, buff, DataType.Int8, it.Value.AttributeIndex);
                    // Add data index.
                    GXCommon.SetData(Settings, buff, DataType.UInt16, it.Value.DataIndex);
                }
            }
            int mode = (int)pg.GetAccess3(2);
            return Read(pg.Name, ObjectType.ProfileGeneric, 2, buff, mode);
        }

        /// <summary>
        /// Create given type of COSEM object.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <returns>COSEM object.</returns>
        public static GXDLMSObject CreateObject(ObjectType type)
        {
            return GXDLMS.CreateObject(null, (int)type, 0xFF, AvailableObjectTypes);
        }

        /// <summary>
        /// Create given type of COSEM object.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <param name="version">Object version number.</param>
        /// <returns>COSEM object.</returns>
        public static GXDLMSObject CreateObject(ObjectType type, byte version)
        {
            return GXDLMS.CreateObject(null, (int)type, version, AvailableObjectTypes);
        }

        /// <summary>
        /// Generates an acknowledgment message, with which the server is informed to
        /// send next packets.
        /// </summary>
        /// <param name="type">Frame type</param>
        /// <returns>Acknowledgment message as byte array.</returns>
        [Obsolete()]
        public byte[] ReceiverReady(RequestTypes type)
        {
            return GXDLMS.ReceiverReady(Settings, type);
        }

        /// <summary>
        /// Generates an acknowledgment message, with which the server is informed to
        /// send next packets.
        /// </summary>
        /// <param name="reply">Reply data.</param>
        /// <returns>Acknowledgment message as byte array.</returns>
        public byte[] ReceiverReady(GXReplyData reply)
        {
            return GXDLMS.ReceiverReady(Settings, reply);
        }

        ///<summary>Removes the frame from the packet, and returns DLMS PDU.</summary>
        ///<param name="reply">The received data from the device.</param>
        ///<param name="data">Information from the received data.</param>
        ///<returns>Is frame complete.</returns>
        [Obsolete("Use GetData with notify parameter instead.")]
        public bool GetData(byte[] reply, GXReplyData data)
        {
            return GetData(new GXByteBuffer(reply), data);
        }

        ///<summary>Removes the frame from the packet, and returns DLMS PDU.</summary>
        ///<param name="reply">The received data from the device.</param>
        ///<param name="data">Information from the received data.</param>
        ///<returns>Is frame complete.</returns>
        public bool GetData(byte[] reply, GXReplyData data, GXReplyData notify)
        {
            return GetData(new GXByteBuffer(reply), data, notify);
        }

        ///<summary>
        ///Removes the HDLC frame from the packet, and returns COSEM data only.
        ///</summary>
        ///<param name="reply">
        ///The received data from the device.
        ///</param>
        ///<param name="data">
        ///The exported reply information.
        ///</param>
        ///<returns>
        /// Is frame complete.
        ///</returns>
        public virtual bool GetData(GXByteBuffer reply, GXReplyData data)
        {
            return GetData(reply, data, null);
        }

        ///<summary>
        ///Removes the HDLC frame from the packet, and returns COSEM data only.
        ///</summary>
        ///<param name="reply">The received data from the device.</param>
        ///<param name="data">The exported reply information.</param>
        ///<param name="notify">Optional notify data.</param>
        ///<returns>Is frame complete.</returns>
        public virtual bool GetData(GXByteBuffer reply, GXReplyData data, GXReplyData notify)
        {
            data.Xml = null;
            bool ret;
            try
            {
                ret = GXDLMS.GetData(Settings, reply, data, notify);
            }
            catch
            {
                if (translator == null || throwExceptions)
                {
                    throw;
                }
                ret = true;
            }
            if (ret && translator != null && data.MoreData == RequestTypes.None)
            {
                if (data.Xml == null)
                {
                    data.Xml = new GXDLMSTranslatorStructure(translator.OutputType, translator.OmitXmlNameSpace, translator.Hex, translator.ShowStringAsHex, translator.Comments, translator.tags);
                }
                int pos = data.Data.Position;
                try
                {
                    GXByteBuffer data2 = data.Data;
                    if (data.Command == Command.GetResponse)
                    {
                        GXByteBuffer tmp = new GXByteBuffer((UInt16)(4 + data.Data.Size));
                        tmp.SetUInt8(data.Command);
                        tmp.SetUInt8(GetCommandType.Normal);
                        tmp.SetUInt8((byte)data.InvokeId);
                        tmp.SetUInt8(0);
                        tmp.Set(data.Data);
                        data.Data = tmp;
                    }
                    else if (data.Command == Command.MethodResponse)
                    {
                        GXByteBuffer tmp = new GXByteBuffer((UInt16)(6 + data.Data.Size));
                        tmp.SetUInt8(data.Command);
                        tmp.SetUInt8(GetCommandType.Normal);
                        tmp.SetUInt8((byte)data.InvokeId);
                        tmp.SetUInt8(0);
                        tmp.SetUInt8(1);
                        tmp.SetUInt8(0);
                        tmp.Set(data.Data);
                        data.Data = tmp;
                    }
                    else if (data.Command == Command.ReadResponse)
                    {
                        GXByteBuffer tmp = new GXByteBuffer((UInt16)(3 + data.Data.Size));
                        tmp.SetUInt8(data.Command);
                        tmp.SetUInt8(VariableAccessSpecification.VariableName);
                        tmp.SetUInt8((byte)data.InvokeId);
                        tmp.SetUInt8(0);
                        tmp.Set(data.Data);
                        data.Data = tmp;
                    }
                    data.Data.Position = 0;
                    if (data.Command == Command.Snrm || data.Command == Command.Ua)
                    {
                        data.Xml.AppendStartTag(data.Command);
                        if (data.Data.Size != 0)
                        {
                            translator.PduToXml(data.Xml, data.Data, translator.OmitXmlDeclaration, translator.OmitXmlNameSpace, true, null);
                        }
                        data.Xml.AppendEndTag(data.Command);
                    }
                    else
                    {
                        if (data.Data.Size != 0)
                        {
                            translator.PduToXml(data.Xml, data.Data, translator.OmitXmlDeclaration, translator.OmitXmlNameSpace, true, null);
                        }
                        data.Data = data2;
                    }
                }
                finally
                {
                    data.Data.Position = pos;
                }
            }
            return ret;
        }

        /// <summary>
        /// Get value from DLMS byte stream.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <returns>Parsed value.</returns>
        public static object GetValue(GXByteBuffer data)
        {
            return GetValue(data, DataType.None, false);
        }


        /// <summary>
        /// Get value from DLMS byte stream.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <param name="useUtc">Standard says that Time zone is from normal time to UTC in minutes.
        /// If meter is configured to use UTC time (UTC to normal time) set this to true.</param>
        /// <returns>Parsed value.</returns>
        public static object GetValue(GXByteBuffer data, bool useUtc)
        {
            return GetValue(data, DataType.None, useUtc);
        }

        /// <summary>
        /// Get value from DLMS byte stream.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <param name="type">Conversion type is used if returned data is byte array.</param>
        /// <returns>Parsed value.</returns>
        public static object GetValue(GXByteBuffer data, DataType type)
        {
            return GetValue(data, type, false);
        }

        /// <summary>
        /// Get value from DLMS byte stream.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <param name="type">Conversion type is used if returned data is byte array.</param>
        /// <param name="useUtc">Standard says that Time zone is from normal time to UTC in minutes.
        /// If meter is configured to use UTC time (UTC to normal time) set this to true.</param>
        /// <returns>Parsed value.</returns>
        public static object GetValue(GXByteBuffer data, DataType type, bool useUtc)
        {
            GXDataInfo info = new GXDataInfo();
            object value = GXCommon.GetData(null, data, info);
            if (value is byte[] && type != DataType.None)
            {
                value = GXDLMSClient.ChangeType((byte[])value, type, useUtc);
            }
            return value;
        }

        /// <summary>
        /// Convert physical address and logical address to server address.
        /// </summary>
        /// <param name="logicalAddress">Server logical address.</param>
        /// <param name="physicalAddress">Server physical address.</param>
        /// <returns>Server address.</returns>
        public static int GetServerAddress(int logicalAddress, int physicalAddress)
        {
            return GetServerAddress(logicalAddress, physicalAddress, 0);
        }

        /// <summary>
        /// Convert physical address and logical address to server address.
        /// </summary>
        /// <param name="logicalAddress">Server logical address.</param>
        /// <param name="physicalAddress">Server physical address.</param>
        /// <param name="addressSize">Address size in bytes.</param>
        /// <returns>Server address.</returns>
        public static int GetServerAddress(int logicalAddress, int physicalAddress, int addressSize)
        {
            int value;
            if (addressSize < 4 && physicalAddress < 0x80 && logicalAddress < 0x80)
            {
                value = logicalAddress << 7 | physicalAddress;
            }
            else if (physicalAddress < 0x4000 && logicalAddress < 0x4000)
            {
                value = logicalAddress << 14 | physicalAddress;
            }
            else
            {
                throw new ArgumentException("Invalid logical or physical address.");
            }
            return value;
        }

        /// <summary>
        /// Converts meter serial number to server address.
        /// Default formula is used.
        /// </summary>
        /// <remarks>
        /// All meters do not use standard formula or support serial number addressing at all.
        /// </remarks>
        /// <param name="serialNumber">Meter serial number.</param>
        /// <returns>Server address.</returns>
        [Obsolete("Use GetServerAddressFromSerialNumber instead")]
        public static int GetServerAddress(int serialNumber)
        {
            return GetServerAddress(serialNumber, null);
        }

        /// <summary>
        /// Converts meter serial number to server address.
        /// </summary>
        /// <param name="serialNumber">Meter serial number.</param>
        /// <param name="formula">Formula used to convert serial number to server address.</param>
        /// <returns>Server address.</returns>
        /// <remarks>
        /// All meters do not use standard formula or support serial number addressing at all.
        /// </remarks>
        [Obsolete("Use GetServerAddressFromSerialNumber instead")]
        public static int GetServerAddress(int serialNumber, string formula)
        {
            //If formula is not given use default formula.
            //This formula is defined in DLMS specification.
            if (String.IsNullOrEmpty(formula))
            {
                formula = "SN % 10000 + 1000";
            }
            return 0x4000 | (int)SerialnumberCounter.Count(serialNumber, formula);
        }

        /// <summary>
        /// Converts meter serial number to server address.
        /// Default formula is used.
        /// </summary>
        /// <remarks>
        /// All meters do not use standard formula or support serial number addressing at all.
        /// </remarks>
        /// <param name="serialNumber">Meter serial number.</param>
        /// <param name="logicalAddress">Used logical address.</param>
        /// <returns>Server address.</returns>
        public static int GetServerAddressFromSerialNumber(long serialNumber, int logicalAddress)
        {
            return GetServerAddressFromSerialNumber(serialNumber, logicalAddress, null);
        }

        /// <summary>
        /// Converts meter serial number to server address.
        /// </summary>
        /// <param name="serialNumber">Meter serial number.</param>
        /// <param name="logicalAddress">Server logical address.</param>
        /// <param name="formula">Formula used to convert serial number to server address.</param>
        /// <returns>Server address.</returns>
        /// <remarks>
        /// All meters do not use standard formula or support serial number addressing at all.
        /// </remarks>
        public static int GetServerAddressFromSerialNumber(long serialNumber, int logicalAddress, string formula)
        {
            //If formula is not given use default formula.
            //This formula is defined in DLMS specification.
            if (String.IsNullOrEmpty(formula))
            {
                formula = "SN % 10000 + 1000";
            }
            if (logicalAddress == 0)
            {
                return (int)SerialnumberCounter.Count(serialNumber, formula);
            }
            return logicalAddress << 14 | (int)SerialnumberCounter.Count(serialNumber, formula);
        }

        /// <summary>
        /// Encrypt Landis+Gyr High level password.
        /// </summary>
        /// <param name="password">User password.</param>
        /// <param name="seed">Seed received from the meter.</param>
        /// <returns></returns>
        public static byte[] EncryptLandisGyrHighLevelAuthentication(byte[] password, byte[] seed)
        {
            byte[] crypted = new byte[seed.Length];
            seed.CopyTo(crypted, 0);
            for (int pos = 0; pos != password.Length; ++pos)
            {
                if (password[pos] != 0x30)
                {
                    crypted[pos] += (byte)(password[pos] - 0x30);
                    //Convert to upper case letter.
                    if (crypted[pos] > '9' && crypted[pos] < 'A')
                    {
                        crypted[pos] += 7;
                    }
                    if (crypted[pos] > 'F')
                    {
                        crypted[pos] = (byte)('0' + crypted[pos] - 'G');
                    }
                }
            }
            return crypted;
        }

        /// <summary>
        /// Generates a access service message.
        /// </summary>
        /// <param name="time">Send time. Set to DateTime.MinValue is not used.</param>
        /// <param name="list"></param>
        /// <returns>Access request as byte array.</returns>
        /// <seealso cref="ParseAccessResponse"/>
        public byte[][] AccessRequest(DateTime time, List<GXDLMSAccessItem> list)
        {
            int mode = 0;
            GXByteBuffer bb = new GXByteBuffer();
            GXCommon.SetObjectCount(list.Count, bb);
            foreach (GXDLMSAccessItem it in list)
            {
                bb.SetUInt8(it.Command);
                //Object type.
                bb.SetUInt16((ushort)it.Target.ObjectType);
                //LN
                String[] items = it.Target.LogicalName.Split('.');
                if (items.Length != 6)
                {
                    throw new ArgumentException("Invalid Logical Name.");
                }
                foreach (String it2 in items)
                {
                    bb.SetUInt8(byte.Parse(it2));
                }
                // Attribute ID.
                bb.SetUInt8(it.Index);
                int m = (int)it.Target.GetAccess3(it.Index);
                if (m > mode)
                {
                    mode = m;
                }
            }
            //Data
            GXCommon.SetObjectCount(list.Count, bb);
            DateTime dt = DateTime.Now;
            foreach (GXDLMSAccessItem it in list)
            {
                if (it.Command == AccessServiceCommandType.Get)
                {
                    it.Target.SetLastReadTime(it.Index, dt);
                    bb.SetUInt8(0);
                }
                else if (it.Command == AccessServiceCommandType.Set)
                {
                    object value = (it.Target as IGXDLMSBase).GetValue(Settings, new ValueEventArgs(it.Target, it.Index, 0, null));
                    DataType type = it.Target.GetDataType(it.Index);
                    if (type == DataType.None)
                    {
                        type = GXDLMSConverter.GetDLMSDataType(value);
                    }
                    GXCommon.SetData(Settings, bb, type, value);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Invalid command.");
                }
            }
            GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.AccessRequest, 0xFF, null, bb, 0xff, Command.None);
            p.AccessMode = mode;
            p.time = new GXDateTime(time);
            return GXDLMS.GetLnMessages(p);
        }

        /// <summary>
        /// Get initial Conformance
        /// </summary>
        /// <param name="useLogicalNameReferencing">Is logical name referencing used.</param>
        /// <returns>Initial Conformance.</returns>
        public static Conformance GetInitialConformance(bool useLogicalNameReferencing)
        {
            if (useLogicalNameReferencing)
            {
                return Conformance.GeneralBlockTransfer |
                    Conformance.BlockTransferWithAction |
                           Conformance.BlockTransferWithSetOrWrite |
                           Conformance.BlockTransferWithGetOrRead |
                           Conformance.Set | Conformance.SelectiveAccess |
                           Conformance.Action | Conformance.MultipleReferences |
                           Conformance.Get | Conformance.Access |
                           Conformance.GeneralProtection |
                           Conformance.DeltaValueEncoding;
            }
            return Conformance.InformationReport |
                        Conformance.Read | Conformance.UnconfirmedWrite |
                        Conformance.Write | Conformance.ParameterizedAccess |
                        Conformance.MultipleReferences;
        }

        /// <summary>
        /// Parse received Information reports and Event notifications.
        /// </summary>
        /// <param name="reply">Reply.</param>
        /// <returns>Data notification data.</returns>
        public object ParseReport(GXReplyData reply, List<KeyValuePair<GXDLMSObject, int>> list)
        {
            if (reply.Command == Command.EventNotification)
            {
                GXDLMSLNCommandHandler.HandleEventNotification(Settings, reply, list);
                return null;
            }
            else if (reply.Command == Command.InformationReport)
            {
                GXDLMSSNCommandHandler.HandleInformationReport(Settings, reply, list);
                return null;
            }
            else if (reply.Command == Command.DataNotification)
            {
                return reply.Value;
            }
            else
            {
                throw new ArgumentException("Invalid command. " + reply.Command);
            }
        }

        /// <summary>
        /// Generates a invalid HDLC frame.
        /// </summary>
        /// <param name="command">HDLC command.</param>
        /// <param name="data">data</param>
        /// <returns>HDLC frame request, as byte array.</returns>
        /// <remarks>
        /// This method can be used for sending custom HDLC frames example in testing.
        /// </remarks>
        public byte[] CustomHdlcFrameRequest(byte command, GXByteBuffer data)
        {
            if (!GXDLMS.UseHdlc(Settings.InterfaceType))
            {
                throw new Exception("This method can be used only to generate HDLC custom frames");
            }
            return GXDLMS.GetHdlcFrame(Settings, command, data, true);
        }

        /// <summary>
        /// Generates a custom HDLC frame.
        /// </summary>
        /// <param name="command">HDLC command.</param>
        /// <param name="data">data</param>
        /// <returns>HDLC frame request, as byte array.</returns>
        /// <remarks>
        /// This method can be used for sending custom HDLC frames example in testing.
        /// </remarks>
        public byte[][] CustomFrameRequest(Command command, GXByteBuffer data)
        {
            if (GXDLMS.UseHdlc(Settings.InterfaceType) ||
                Settings.InterfaceType == InterfaceType.WRAPPER)
            {
                byte[][] reply;
                if (command == Command.None)
                {
                    List<byte[]> messages = new List<byte[]>();
                    if (Settings.InterfaceType == Enums.InterfaceType.WRAPPER)
                    {
                        messages.Add(GXDLMS.GetWrapperFrame(Settings, command, data));
                    }
                    else if (GXDLMS.UseHdlc(Settings.InterfaceType))
                    {
                        messages.Add(GXDLMS.GetHdlcFrame(Settings, (byte)command, data));
                    }
                    reply = messages.ToArray();
                }
                else if (UseLogicalNameReferencing)
                {
                    GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, command, 0, data, null, 0xff, Command.None);
                    reply = GXDLMS.GetLnMessages(p);
                }
                else
                {
                    reply = GXDLMS.GetSnMessages(new GXDLMSSNParameters(Settings, command, 0, 0, null, data));
                }
                return reply;
            }
            throw new Exception("This method can be used only to generate HDLC or WRAPPER custom frames");
        }

        /// <summary>
        /// Get size of the frame.
        /// </summary>
        /// <remarks>
        /// When WRAPPER is used this method can be used to check how many bytes we need to read.
        /// </remarks>
        /// <param name="data">Received data.</param>
        /// <returns>Size of received bytes on the frame.</returns>
        public int GetFrameSize(GXByteBuffer data)
        {
            int ret;
            switch (InterfaceType)
            {
                case InterfaceType.HDLC:
                case InterfaceType.HdlcWithModeE:
                    {
                        ret = 0;
                        short ch;
                        int pos, index = data.Position;
                        try
                        {
                            // If whole frame is not received yet.
                            if (data.Available > 8)
                            {
                                // Find start of HDLC frame.
                                for (pos = data.Position; pos < data.Size; ++pos)
                                {
                                    ch = data.GetUInt8();
                                    if (ch == GXCommon.HDLCFrameStartEnd)
                                    {
                                        break;
                                    }
                                }
                                byte frame = data.GetUInt8();
                                // Check frame length.
                                if ((frame & 0x7) != 0)
                                {
                                    ret = ((frame & 0x7) << 8);
                                }
                                ret += 1 + data.GetUInt8();
                            }
                        }
                        finally
                        {
                            data.Position = index;
                        }
                    }
                    break;
                case InterfaceType.WRAPPER:
                    if (data.Available < 8 || data.GetUInt16(data.Position) != 1)
                    {
                        ret = 8 - data.Available;
                    }
                    else
                    {
                        ret = 8 + data.GetUInt16(data.Position + 6) - data.Available;
                    }
                    break;
                case InterfaceType.Plc:
                    if (data.Available < 2 || data.GetUInt8(data.Position) != 2)
                    {
                        ret = 2 - data.Available;
                    }
                    else
                    {
                        ret = 2 + data.GetUInt8(data.Position + 1) - data.Available;
                    }
                    break;
                case InterfaceType.PlcHdlc:
                    ret = GXDLMS.GetPlcSfskFrameSize(data);
                    if (ret == 0)
                    {
                        ret = 2;
                    }
                    break;
                default:
                    ret = 1;
                    break;
            }
            if (ret < 1)
            {
                ret = 1;
            }
            return ret;
        }

        /// <summary>
        /// Get HDLC sender and receiver address information.
        /// </summary>
        /// <param name="reply">Received data.</param>
        /// <param name="target">target (primary) address</param>
        /// <param name="source">Source (secondary) address.</param>
        /// <param name="type">DLMS frame type.</param>
        public static void GetHdlcAddressInfo(GXByteBuffer reply, out int target, out int source, out byte type)
        {
            GXDLMS.GetHdlcAddressInfo(reply, out target, out source, out type);
        }


        /// <summary>
        /// Overwrite attribute access rights if association view tells wrong access rights and they are overwritten.
        /// </summary>
        public bool OverwriteAttributeAccessRights
        {
            get
            {
                return Settings.OverwriteAttributeAccessRights;
            }
            set
            {
                Settings.OverwriteAttributeAccessRights = value;
            }
        }


        /// <summary>
        /// Can client read the object attribute index. 
        /// </summary>
        /// <remarks>
        /// This method is added because Association Logical Name version #3 where access rights are defined with bitmask.
        /// </remarks>
        /// <param name="target">Object to read.</param>
        /// <param name="index">Attribute index.</param>
        /// <returns>True, if read is allowed.</returns>
        public bool CanRead(GXDLMSObject target, int index)
        {
            //Handle access rights for Association LN Version < 3.
            AccessMode access = target.GetAccess(index);
            if ((access & AccessMode.Read) == 0 &&
                access != AccessMode.AuthenticatedRead &&
                access != AccessMode.AuthenticatedReadWrite)
            {
                //If bit mask is used.
                AccessMode3 m = target.GetAccess3(index);
                if ((m & AccessMode3.Read) == 0)
                {
                    return false;
                }
                Security security = Security.None;
                Signing signing = Signing.None;
                if (Settings.Cipher != null)
                {
                    security = Settings.Cipher.Security;
                    signing = Settings.Cipher.Signing;
                }
                //If authenticatation is expected, but secured connection is not used.
                if ((m & (AccessMode3.AuthenticatedRequest | AccessMode3.AuthenticatedResponse)) != 0 && (security & (Security.Authentication)) == 0)
                {
                    return false;
                }
                //If encryption is expected, but secured connection is not used.
                if ((m & (AccessMode3.EncryptedRequest | AccessMode3.EncryptedResponse)) != 0 &&
                    (security & (Security.Encryption)) == 0)
                {
                    return false;
                }
                //If signing is expected, but it's not used.
                if ((m & (AccessMode3.DigitallySignedRequest | AccessMode3.DigitallySignedResponse)) != 0 &&
                    (signing & (Signing.GeneralSigning)) == 0)
                {
                    //If signing keys are not set.
                    if (Settings.Cipher.SigningKeyPair.Key == null ||
                        Settings.Cipher.SigningKeyPair.Value == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Can client write the object attribute index. 
        /// </summary>
        /// <remarks>
        /// This method is added because Association Logical Name version #3 where access rights are defined with bitmask.
        /// </remarks>
        /// <param name="target">Object to write.</param>
        /// <param name="index">Attribute index.</param>
        /// <returns>True, if write is allowed.</returns>
        public bool CanWrite(GXDLMSObject target, int index)
        {
            //Handle access rights for Association LN Version < 3.
            AccessMode access = target.GetAccess(index);
            if ((access & AccessMode.Write) == 0 &&
                access != AccessMode.AuthenticatedWrite &&
                access != AccessMode.AuthenticatedReadWrite)
            {
                //If bit mask is used.
                AccessMode3 m = target.GetAccess3(index);
                if ((m & AccessMode3.Write) == 0)
                {
                    return false;
                }
                Security security = Security.None;
                Signing signing = Signing.None;
                if (Settings.Cipher != null)
                {
                    security = Settings.Cipher.Security;
                    signing = Settings.Cipher.Signing;
                }
                //If authentication is expected, but secured connection is not used.
                if ((m & (AccessMode3.AuthenticatedRequest | AccessMode3.AuthenticatedResponse)) != 0 && (security & (Security.Authentication)) == 0)
                {
                    return false;
                }
                //If encryption is expected, but secured connection is not used.
                if ((m & (AccessMode3.EncryptedRequest | AccessMode3.EncryptedResponse)) != 0 &&
                    (security & (Security.Encryption)) == 0)
                {
                    return false;
                }
                //If signing is expected, but it's not used.
                if ((m & (AccessMode3.DigitallySignedRequest | AccessMode3.DigitallySignedResponse)) != 0 &&
                    (signing & (Signing.GeneralSigning)) == 0)
                {
                    //If signing keys are not set.
                    if (Settings.Cipher.SigningKeyPair.Key == null ||
                        Settings.Cipher.SigningKeyPair.Value == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Can client invoke server methods. 
        /// </summary>
        /// <remarks>
        /// This method is added because Association Logical Name version #3 where access rights are defined with bitmask.
        /// </remarks>
        /// <param name="target">Object to invoke.</param>
        /// <param name="index">Method attribute index.</param>
        /// <returns>True, if client can access meter methods.</returns>
        public bool CanInvoke(GXDLMSObject target, int index)
        {
            //Handle access rights for Association LN Version < 3.
            if (target.GetMethodAccess(index) == MethodAccessMode.NoAccess)
            {
                //If bit mask is used.
                MethodAccessMode3 m = target.GetMethodAccess3(index);
                if ((m & MethodAccessMode3.Access) == 0)
                {
                    return false;
                }
                Security security = Security.None;
                Signing signing = Signing.None;
                if (Settings.Cipher != null)
                {
                    security = Settings.Cipher.Security;
                    signing = Settings.Cipher.Signing;
                }
                //If authentication is expected, but secured connection is not used.
                if ((m & (MethodAccessMode3.AuthenticatedRequest | MethodAccessMode3.AuthenticatedResponse)) != 0 && (security & (Security.Authentication)) == 0)
                {
                    return false;
                }
                //If encryption is expected, but secured connection is not used.
                if ((m & (MethodAccessMode3.EncryptedRequest | MethodAccessMode3.EncryptedResponse)) != 0 &&
                    (security & (Security.Encryption)) == 0)
                {
                    return false;
                }
                //If signing is expected, but it's not used.
                if ((m & (MethodAccessMode3.DigitallySignedRequest | MethodAccessMode3.DigitallySignedResponse)) != 0 &&
                    (signing & (Signing.GeneralSigning)) == 0)
                {
                    //If signing keys are not set.
                    if (Settings.Cipher.SigningKeyPair.Key == null ||
                        Settings.Cipher.SigningKeyPair.Value == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}