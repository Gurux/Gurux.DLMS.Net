using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Secure
{
    class AesGcmParameter
    {
        public Command Command
        {
            get;
            set;
        }
        public Security Security
        {
            get;
            set;
        }

        public UInt32 FrameCounter
        {
            get;
            set;
        }

        public byte[] SystemTitle
        {
            get;
            set;
        }
        public byte[] BlockCipherKey
        {
            get;
            set;
        }
        public byte[] AuthenticationKey
        {
            get;
            set;
        }
        public byte[] PlainText
        {
            get;
            set;
        }
        public CountType Type
        {
            get;
            set;
        }
        public byte[] CountTag
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="security"></param>
        /// <param name="frameCounter"></param>
        /// <param name="systemTitle"></param>
        /// <param name="blockCipherKey"></param>
        /// <param name="authenticationKey"></param>
        /// <param name="plainText"></param>
        public AesGcmParameter(Command command, Security security,
            UInt32 frameCounter, byte[] systemTitle,
            byte[] blockCipherKey, byte[] authenticationKey,
            byte[] plainText)
        {
            Command = command;
            Security = security;
            FrameCounter = frameCounter;
            SystemTitle = systemTitle;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
            PlainText = plainText;
            Type = CountType.Packet;
        }
    }
}
