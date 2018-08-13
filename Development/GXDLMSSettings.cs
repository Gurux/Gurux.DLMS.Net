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
using Gurux.DLMS.Objects;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS
{
    /// <summary>
    /// This class includes DLMS communication settings.
    /// </summary>
    public class GXDLMSSettings
    {
        private bool useLogicalNameReferencing;

        ///<summary>
        /// Server frame sequence starting number.
        ///</summary>
        private const byte ServerStartFrameSequence = 0x0F;

        ///<summary>
        /// Client frame sequence starting number.
        ///</summary>
        private const byte ClientStartFrameSequence = 0xEE;

        ///<summary>
        /// Default Max received PDU size.
        ///</summary>
        private const UInt16 DefaultMaxReceivePduSize = 0xFFFF;

        /// <summary>
        /// Invoke ID.
        /// </summary>
        private byte invokeID = 0x1;

        /// <summary>
        /// Invoke ID.
        /// </summary>
        internal UInt32 longInvokeID = 0x1;

        ///<summary>
        ///Client to server challenge.
        ///</summary>
        private byte[] ctoSChallenge;

        ///<summary>
        ///Server to Client challenge.
        ///</summary>
        private byte[] stoCChallenge;

        /// <summary>
        /// HDLC sender frame sequence number.
        /// </summary>
        private byte SenderFrame;

        /// <summary>
        /// HDLC receiver frame sequence number.
        /// </summary>
        private byte ReceiverFrame;

        /// <summary>
        /// Source system title.
        /// </summary>
        internal byte[] SourceSystemTitle;

        /// <summary>
        /// Pre-established system title.
        /// </summary>
        internal byte[] PreEstablishedSystemTitle;


        /// <summary>
        /// Key Encrypting Key, also known as Master key.
        /// </summary>
        internal byte[] Kek;

        /// <summary>
        /// Long data count.
        /// </summary>
        internal UInt32 Count;

        /// <summary>
        /// Long data index.
        /// </summary>
        internal UInt32 Index;

        /// <summary>
        /// Maximum PDU size.
        /// </summary>
        internal UInt16 maxReceivePDUSize;

        /// <summary>
        /// Protocol version.
        /// </summary>
        internal string protocolVersion = null;

        internal bool CanAccess()
        {
            return SourceSystemTitle != null;
        }

        /// <summary>
        /// When connection is made client tells what kind of services it want's to use.
        /// </summary>
        internal Conformance ProposedConformance = (Conformance)0;

        /// <summary>
        /// Server tells what functionality is available and client will know it.
        /// </summary>
        internal Conformance NegotiatedConformance = (Conformance)0;

        /// <summary>
        /// Is authentication Required.
        /// </summary>
        internal bool IsAuthenticationRequired
        {
            get;
            set;
        }

        /// <summary>
        /// Cipher interface.
        /// </summary>
        /// <remarks>
        /// GXDLMSAssociationShortName and GXDLMSAssociationLogicalName use this is GMAC authentication is used.
        /// </remarks>
        internal GXICipher Cipher
        {
            get;
            set;
        }

        /// <summary>
        /// Used security suite.
        /// </summary>
        internal SecuritySuite SecuritySuite
        {
            get;
            set;
        }

        /// <summary>
        /// User id is the identifier of the user. 
        /// </summary>
        /// <remarks>
        /// This value is used if user list on Association LN is used.
        /// </remarks>
        public int UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Quality of service.
        /// </summary>
        public byte QualityOfService
        {
            get;
            set;
        }
        
        /// <summary>
        /// Standard says that Time zone is from normal time to UTC in minutes.
        /// If meter is configured to use UTC time (UTC to normal time) set this to true.
        /// </summary>
        internal bool UseUtc2NormalTime
        {
            get;
            set;
        }

        /// <summary>
        /// Force that data is always sent as blocks.
        /// </summary>
        internal bool ForceToBlocks
        {
            get;
            set;
        }

        /// <summary>
        /// Used standard.
        /// </summary>
        public Standard Standard
        {
            get;
            set;
        }

        ///<summary>
        ///Constructor.
        ///</summary>
        internal GXDLMSSettings(bool server)
        {
            UseCustomChallenge = false;
            StartingBlockIndex = BlockIndex = 1;
            DLMSVersion = 6;
            InvokeID = 0x1;
            Priority = Priority.High;
            ServiceClass = ServiceClass.Confirmed;
            MaxServerPDUSize = MaxPduSize = DefaultMaxReceivePduSize;
            IsServer = server;
            Objects = new GXDLMSObjectCollection();
            Limits = new GXDLMSLimits();
            Gateway = null;
            ProposedConformance = GXDLMSClient.GetInitialConformance(false);
            ResetFrameSequence();
            WindowSize = 1;
            UserId = -1;
            Standard = Standard.DLMS;
        }

        /// <summary>
        /// Copy settings.
        /// </summary>
        /// <param name="target"></param>
        internal void CopyTo(GXDLMSSettings target)
        {
            target.UseCustomChallenge = UseCustomChallenge;
            target.StartingBlockIndex = StartingBlockIndex;
            target.DLMSVersion = DLMSVersion;
            target.BlockIndex = BlockIndex;
            target.IsServer = IsServer;
            target.useLogicalNameReferencing = useLogicalNameReferencing;
            target.ClientAddress = ClientAddress;
            target.ServerAddress = ServerAddress;
            target.ServerAddressSize = ServerAddressSize;
            target.Authentication = Authentication;
            target.Password = Password;
            target.ProposedConformance = ProposedConformance;
            target.InvokeID = InvokeID;
            target.longInvokeID = longInvokeID;
            target.Priority = Priority;
            target.ServiceClass = ServiceClass;
            target.ctoSChallenge = ctoSChallenge;
            target.stoCChallenge = stoCChallenge;
            target.SenderFrame = SenderFrame;
            target.ReceiverFrame = ReceiverFrame;
            target.SourceSystemTitle = SourceSystemTitle;
            target.Kek = Kek;
            target.Count = Count;
            target.Index = Index;
            target.maxReceivePDUSize = maxReceivePDUSize;
            target.MaxServerPDUSize = MaxServerPDUSize;
            target.ProposedConformance = ProposedConformance;
            target.NegotiatedConformance = NegotiatedConformance;
            target.SecuritySuite = SecuritySuite;
            if (Cipher != null && target.Cipher != null)
            {
                ((GXCiphering)Cipher).CopyTo((GXCiphering)target.Cipher);
            }
            target.UserId = UserId;
            target.UseUtc2NormalTime = UseUtc2NormalTime;
            target.WindowSize = WindowSize;
            target.Objects.Clear();
            target.Objects.AddRange(Objects);
            target.Limits.MaxInfoRX = Limits.MaxInfoRX;
            target.Limits.MaxInfoTX = Limits.MaxInfoTX;
            target.Limits.WindowSizeRX = Limits.WindowSizeRX;
            target.Limits.WindowSizeTX = Limits.WindowSizeTX;
            if (Gateway != null)
            {
                target.Gateway = new GXDLMSGateway();
                target.Gateway.NetworkId = Gateway.NetworkId;
                target.Gateway.PhysicalDeviceAddress = Gateway.PhysicalDeviceAddress;
            }
        }

        /// <summary>
        /// Client to Server challenge.
        /// </summary>
        public byte[] CtoSChallenge
        {
            get
            {
                return ctoSChallenge;
            }
            set
            {
                ctoSChallenge = value;
            }
        }

        /// <summary>
        /// Server to Client challenge.
        /// </summary>
        public byte[] StoCChallenge
        {
            get
            {
                return stoCChallenge;
            }
            set
            {
                stoCChallenge = value;
            }
        }

        /// <summary>
        /// Used authentication.
        /// </summary>
        public Authentication Authentication
        {
            get;
            set;
        }

        /// <summary>
        /// Client password.
        /// </summary>
        public byte[] Password
        {
            get;
            set;
        }

        internal ConnectionState Connected = ConnectionState.None;

        ///<summary>
        /// Increase receiver sequence.
        ///</summary>
        ///<param name="value">
        /// Frame value.
        ///</param>
        ///<returns>
        /// Increased receiver frame sequence.
        ///</returns>
        static byte IncreaseReceiverSequence(byte value)
        {
            return (byte)(value + 0x20 | 0x10 | value & 0xE);
        }

        ///<summary>
        ///Increase sender sequence.
        ///</summary>
        ///<param name="value">
        /// Increase frame value.
        ///</param>
        ///<returns>
        /// Increased sender frame sequence.
        ///</returns>
        static byte IncreaseSendSequence(byte value)
        {
            return (byte)(value & 0xF0 | (value + 0x2) & 0xE);
        }

        ///<summary>
        ///Reset frame sequence.
        ///</summary>
        public void ResetFrameSequence()
        {
            if (IsServer)
            {
                SenderFrame = 0x1E;
                ReceiverFrame = 0xEE;
            }
            else
            {
                SenderFrame = 0x10;
                ReceiverFrame = 0xE;
            }
        }

        public bool CheckFrame(byte frame)
        {
            //If notify
            if (frame == 0x13)
            {
                return true;
            }
            //If U frame.
            if ((frame & (byte)HdlcFrameType.Uframe) == (byte)HdlcFrameType.Uframe)
            {
                if (frame == 0x73 || frame == 0x93)
                {
                    bool isEcho = !IsServer && frame == 0x93 && SenderFrame == 0x10 && ReceiverFrame == 0xE;
                    ResetFrameSequence();
                    return !isEcho;
                }
                return true;
            }
            //Return true if pre-established connections are used.
            if (Connected == ConnectionState.None)
            {
                return true;
            }
            //If S -frame.
            if ((frame & (byte)HdlcFrameType.Sframe) == (byte)HdlcFrameType.Sframe)
            {
                ReceiverFrame = IncreaseReceiverSequence(ReceiverFrame);
                return true;
            }
            //Handle I-frame.
            byte expected;
            if ((SenderFrame & 0x1) == 0)
            {
                expected = IncreaseReceiverSequence(IncreaseSendSequence(ReceiverFrame));
                if (frame == expected)
                {
                    ReceiverFrame = frame;
                    return true;
                }
            }
            //If answer for RR.
            else
            {
                expected = IncreaseSendSequence(ReceiverFrame);
                if (frame == expected)
                {
                    ReceiverFrame = frame;
                    return true;
                }
            }
            //If try to find data from bytestream and not real communicating.
            if (ReceiverFrame == 0xEE)
            {
                return true;
            }
            System.Diagnostics.Debug.WriteLine("Invalid HDLC Frame: " + frame.ToString("X") + ". Expected: " + expected.ToString("X"));
            return false;
        }

        ///<summary>
        /// Generates I-frame.
        ///</summary>
        internal byte NextSend(bool first)
        {
            if (first)
            {
                SenderFrame = IncreaseReceiverSequence(IncreaseSendSequence(SenderFrame));
            }
            else
            {
                SenderFrame = IncreaseSendSequence(SenderFrame);
            }
            return SenderFrame;
        }

        ///<summary>
        ///Generates Receiver Ready S-frame.
        ///</summary>
        internal byte ReceiverReady()
        {
            SenderFrame = (byte)(IncreaseReceiverSequence(SenderFrame) | 1);
            return (byte)(SenderFrame & 0xF1);
        }

        ///<summary>
        ///Generates Keep Alive S-frame.
        ///</summary>
        internal byte KeepAlive()
        {
            SenderFrame = (byte)(SenderFrame | 1);
            return (byte)(SenderFrame & 0xF1);
        }

        ///<summary>
        ///Current block index.
        ///</summary>
        public UInt32 BlockIndex
        {
            get;
            internal set;
        }

        /// <summary>
        ///  Gets starting block index. Default is One based, but some meters use Zero based value.
        ///  Usually this is not used.
        /// </summary>
        public UInt32 StartingBlockIndex
        {
            get;
            internal set;
        }


        ///<summary>
        /// Block number acknowledged in GBT. 
        ///</summary>
        public UInt16 BlockNumberAck
        {
            get;
            set;
        }

        ///<summary>
        /// Resets block index to default value.
        ///</summary>
        internal void ResetBlockIndex()
        {
            BlockIndex = StartingBlockIndex;
            BlockNumberAck = 0;
        }

        ///<summary>
        /// Increases block index.
        ///</summary>
        internal void IncreaseBlockIndex()
        {
            BlockIndex += 1;
        }

        /// <summary>
        /// Is this server or client settings.
        /// </summary>
        internal bool IsServer
        {
            get;
            set;
        }

        /// <summary>
        /// Information from the frame size that server can handle.
        /// </summary>
        public GXDLMSLimits Limits
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gateway settings.
        /// </summary>
        public GXDLMSGateway Gateway
        {
            get;
            internal set;
        }        

        /// <summary>
        /// Used interface.
        /// </summary>
        public InterfaceType InterfaceType
        {
            get;
            set;
        }

        /// <summary>
        /// Client address.
        /// </summary>
        public int ClientAddress
        {
            get;
            set;
        }

        /// <summary>
        /// GBT window size.
        /// </summary>
        public byte WindowSize
        {
            get;
            set;
        }

        /// <summary>
        /// Server address.
        /// </summary>
        public int ServerAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Size of Server address.
        /// </summary>
        public byte ServerAddressSize
        {
            get;
            set;
        }

        /// <summary>
        /// DLMS version number.
        /// </summary>
        public byte DLMSVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum PDU size.
        /// </summary>
        public UInt16 MaxPduSize
        {
            get
            {
                return maxReceivePDUSize;
            }
            set
            {
                if (!IsServer && value < 64)
                {
                    throw new ArgumentOutOfRangeException("MaxReceivePDUSize");
                }
                maxReceivePDUSize = value;
            }
        }

        /// <summary>
        /// Server maximum PDU size.
        /// </summary>
        public UInt16 MaxServerPDUSize
        {
            get;
            set;
        }

        /// <summary>
        /// Is Logical Name Referencing used.
        /// </summary>
        public bool UseLogicalNameReferencing
        {
            get
            {
                return useLogicalNameReferencing;
            }
            set
            {
                if (useLogicalNameReferencing != value)
                {
                    useLogicalNameReferencing = value;
                    ProposedConformance = GXDLMSClient.GetInitialConformance(value);
                }
            }
        }

        /// <summary>
        /// Used priority.
        /// </summary>
        public Priority Priority
        {
            get;
            set;
        }

        /// <summary>
        /// Used service class.
        /// </summary>
        public ServiceClass ServiceClass
        {
            get;
            set;
        }

        /// <summary>
        /// Invoke ID.
        /// </summary>
        public byte InvokeID
        {
            get
            {
                return invokeID;
            }
            set
            {
                if (value > 0xF)
                {
                    throw new System.ArgumentException("Invalid InvokeID");
                }
                invokeID = value;
            }
        }

        /// <summary>
        /// Collection of the objects.
        /// </summary>
        public GXDLMSObjectCollection Objects
        {
            get;
            internal set;
        }

        ///<summary>
        /// Is custom challenges used. If custom challenge is used new challenge is
        /// not generated if it is set. This is for debugging purposes.
        ///</summary>
        internal bool UseCustomChallenge
        {
            get;
            set;
        }
    }
}