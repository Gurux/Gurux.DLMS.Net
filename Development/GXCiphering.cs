using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS
{
    /// <summary>
    /// Gurux DLMS/COSEM Transport security (Ciphering) settings.
    /// </summary>
    public class GXCiphering
    {
        byte[] m_AuthenticationKey;
        byte[] m_SystemTitle;
        byte[] m_BlockCipherKey;
        internal UInt32 FrameCounter = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="frameCounter">Default frame counter value. Set to Zero.</param>
        /// <param name="systemTitle"></param>
        /// <param name="blockCipherKey"></param>
        /// <param name="authenticationKey"></param>
        public GXCiphering(byte[] systemTitle)
        {
            FrameCounter = 1;
            SystemTitle = systemTitle;
            BlockCipherKey = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            AuthenticationKey = new byte[] { 0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF};
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="frameCounter">Default frame counter value. Set to Zero.</param>
        /// <param name="systemTitle"></param>
        /// <param name="blockCipherKey"></param>
        /// <param name="authenticationKey"></param>
        public GXCiphering(UInt32 frameCounter, byte[] systemTitle, byte[] blockCipherKey, byte[] authenticationKey)
        {
            FrameCounter = frameCounter;
            SystemTitle = systemTitle;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
        }

        /// <summary>
        /// Is chippering enabled.
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// The SystemTitle is a 8 bytes (64 bit) value that completely identifies a partner of the communication. 
        /// In COSEM, the first 3 octets of the System Title hold the three letters manufacturer ID.
        /// The remainder of the system title holds for example a serial number. 
        /// </summary>
        public byte[] SystemTitle
        {
            get
            {
                return m_SystemTitle;
            }
            set
            {
                if (value != null && value.Length != 8)
                {
                    throw new ArgumentOutOfRangeException("Invalid System Title.");
                }
                m_SystemTitle = value;
            }
        }

        public byte[] BlockCipherKey
        {
            get
            {
                return m_BlockCipherKey;
            }
            set
            {
                if (value != null && value.Length != 16)
                {
                    throw new ArgumentOutOfRangeException("Invalid Block Cipher Key.");
                }
                m_BlockCipherKey = value;
            }
        }

        /// <summary>
        /// Authentication Key is 16 bytes value.
        /// </summary>
        public byte[] AuthenticationKey
        {
            get
            {
                return m_AuthenticationKey;
            }
            set
            {
                if (value != null && value.Length != 16)
                {
                    throw new ArgumentOutOfRangeException("Invalid Authentication Key.");
                }
                m_AuthenticationKey = value;
            }
        }
    }
}
