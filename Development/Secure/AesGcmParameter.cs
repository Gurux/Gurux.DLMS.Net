using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Secure
{
    internal class AesGcmParameter
    {
        public byte Tag
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
        /// <summary>
        /// Crypted text in decrypt or plain text in encryption.
        /// </summary>
        public GXByteBuffer Data
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
        public AesGcmParameter(byte tag, Security security,
            UInt32 frameCounter, byte[] systemTitle,
            byte[] blockCipherKey, byte[] authenticationKey,
            GXByteBuffer plainText)
        {
            Tag = tag;
            Security = security;
            FrameCounter = frameCounter;
            SystemTitle = systemTitle;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
            Data = plainText;
            Type = CountType.Packet;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="systemTitle"></param>
        /// <param name="blockCipherKey"></param>
        /// <param name="authenticationKey"></param>
        /// <param name="crypted">Crypted data.</param>
        public AesGcmParameter(byte[] systemTitle,
            byte[] blockCipherKey, byte[] authenticationKey,
            GXByteBuffer crypted)
        {
            SystemTitle = systemTitle;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
            Data = crypted;
            Type = CountType.Packet;
        }
    }
}
