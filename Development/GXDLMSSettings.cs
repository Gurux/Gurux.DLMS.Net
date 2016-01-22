
namespace Gurux.DLMS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Gurux.DLMS.Objects;
    using Gurux.DLMS.Enums;

    /// <summary> 
    /// This class includes DLMS communication settings.
    /// </summary>
    public class GXDLMSSettings
    {
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

        ///<summary>
        /// Is custom challenges used. If custom challenge is used new challenge is
        /// not generated if it is set. This is for debugging purposes. 
        ///</summary>
        private bool customChallenges = false;

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
        /// Is this server or client.
        /// </summary>
        private bool Server;

        ///<summary>
        ///Constructor. 
        ///</summary>
        internal GXDLMSSettings(bool isServer)
        {
            BlockIndex = 1;
            DLMSVersion = 6;
            InvokeID = 0x1;
            Priority = Priority.High;
            ServiceClass = ServiceClass.UnConfirmed;
            MaxReceivePDUSize = DefaultMaxReceivePduSize;
            Server = isServer;
            Objects = new GXDLMSObjectCollection();
            Limits = new GXDLMSLimits();
            ResetFrameSequence();
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
                if (!customChallenges || ctoSChallenge == null)
                {
                    ctoSChallenge = value;
                }
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
                if (!customChallenges || stoCChallenge == null)
                {
                    stoCChallenge = value;
                }
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

        /// <summary>
        /// DLMS version number.
        /// </summary>
        public byte DlmsVersionNumber
        {
            get;
            set;
        }

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
                ReceiverFrame = 0xFE;
            }
            else
            {
                SenderFrame = 0x10;
                ReceiverFrame = 0xE;
            }
        }

        public bool CheckFrame(byte frame)
        {
            //If U frame.
            if ((frame & 0x3) == 3)
            {
                ResetFrameSequence();
                return true;
            }
            //If S -frame
            if ((frame & 0x3) == 1)
            {
                if ((frame & 0xE0) == ((ReceiverFrame) & 0xE0))
                {
                    ReceiverFrame = frame;
                    return true;
                }
                System.Diagnostics.Debug.WriteLine("Frame ID do not match.");
                return true;
            }

            //If I frame sent.
            if ((SenderFrame & 0x1) == 0)
            {
                if ((frame & 0xE0) == ((ReceiverFrame + 0x20) & 0xE0) &&
                    (frame & 0xE) == ((ReceiverFrame + 2) & 0xE)) 
                {
                    ReceiverFrame = (byte)(frame);
                    return true;
                }
            }
            //If S-frame sent.
            else if (frame == ReceiverFrame ||
                ((frame & 0xE0) == (ReceiverFrame & 0xE0) &&
                (frame & 0xE) == ((ReceiverFrame + 2) & 0xE))) 
            {
                ReceiverFrame = frame;
                return true;
            }

            System.Diagnostics.Debug.WriteLine("Frame ID do not match.");
            return true;
        }

        /// <summary>
        /// Is frame generated. This is used to tell is any packet send to the server.
        /// </summary>
        /// <returns>
        /// True, if any packet is sent to the server.
        /// </returns>
        public bool IsGenerated()
        {
            if (IsServer)
            {
                return SenderFrame != 0x1E || ReceiverFrame != 0xFE;
            }
            return SenderFrame != 0x10 ||  ReceiverFrame != 0xE;
        }

        ///<summary>
        /// Generates I-frame. 
        ///</summary>
        public byte NextSend()
        {
            SenderFrame = IncreaseReceiverSequence(IncreaseSendSequence(SenderFrame));
            return SenderFrame;
        }
      
        ///<summary>
        ///Generates Receiver Ready S-frame. 
        ///</summary>
        public byte ReceiverReady()
        {
            SenderFrame = (byte)(IncreaseReceiverSequence(SenderFrame) | 1);
            return (byte)(SenderFrame & 0xF1);           
        }

        ///<summary>
        ///Generates Keep Alive S-frame. 
        ///</summary>
        public byte KeepAlive()
        {
            SenderFrame = (byte)(SenderFrame | 1);
            return (byte)(SenderFrame & 0xF1);
        }

        ///<summary>
        /// Gets Logical Name settings. 
        ///</summary>
        public GXDLMSLNSettings LnSettings
        {
            get;
            set;
        }

        ///<summary>
        /// Short name settings.
        ///</summary>
        public GXDLMSSNSettings SnSettings
        {
            get;
            set;
        }

        ///<summary>
        ///Current block index. 
        public UInt32 BlockIndex
        {
            get;
            internal set;
        }

        ///<summary>
        /// Resets block index to default value.
        ///</summary>
        public void ResetBlockIndex()
        {
            BlockIndex = 1;
        }

        ///<summary>
        /// Increases block index.
        ///</summary>
        public void IncreaseBlockIndex()
        {
            BlockIndex += 1;
        }

        /// <summary>
        /// Is this server or client settings.
        /// </summary>
        public bool IsServer
        {
            get
            {
                return Server;
            }
        }

        /// <summary>
        /// Information from the frame size that server can handle.
        /// </summary>
        public GXDLMSLimits Limits
        {
            get;
            set;
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
        /// Server address.
        /// </summary>
        public int ServerAddress
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
        public UInt16 MaxReceivePDUSize
        {
            get;
            set;
        }

        /// <summary>
        /// Is Logical Name Referencing used.
        /// </summary>
        public bool UseLogicalNameReferencing
        {
            get;
            set;
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

        /// <summary>
        /// Is custom challenge used.
        /// </summary>
        public bool UseCustomChallenge
        {
            get;
            set;
        }
    }
}