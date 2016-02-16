using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;

namespace Gurux.DLMS.Secure
{
    /// <summary>
    /// Gurux DLMS/COSEM Transport security (Ciphering) settings.
    /// </summary>
    public class GXCiphering : GXICipher
    {
        byte[] authenticationKey;
        byte[] systemTitle;
        byte[] blockCipherKey;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Default values are from the Green Book.
        /// </remarks>
        /// <param name="frameCounter">Default frame counter value. Set to Zero.</param>
        /// <param name="systemTitle"></param>
        /// <param name="blockCipherKey"></param>
        /// <param name="authenticationKey"></param>
        public GXCiphering(byte[] systemTitle)
        {
            Security = Security.None;            
            SystemTitle = systemTitle;            
            BlockCipherKey = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            AuthenticationKey = new byte[] { 0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF};
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Default values are from the Green Book.
        /// </remarks>
        /// <param name="frameCounter">Default frame counter value. Set to Zero.</param>
        /// <param name="systemTitle"></param>
        /// <param name="blockCipherKey"></param>
        /// <param name="authenticationKey"></param>
        public GXCiphering(UInt32 frameCounter, byte[] systemTitle, byte[] blockCipherKey, byte[] authenticationKey)
        {
            Security = Security.None;            
            FrameCounter = frameCounter;
            SystemTitle = systemTitle;
            BlockCipherKey = blockCipherKey;
            AuthenticationKey = authenticationKey;
        }
        
        /// <summary>
        /// Used security.
        /// </summary>
        public Security Security
        {
            get;
            set;
        }

        /// <summary>
        /// Frame counter.
        /// </summary>
        public UInt32 FrameCounter
        {
            get;
            set;
        }

        /// <summary>
        /// The SystemTitle is a 8 bytes (64 bit) value that identifies a partner of the communication. 
        /// First 3 bytes contains the three letters manufacturer ID.
        /// The remainder of the system title holds for example a serial number. 
        /// </summary>
        /// <seealso href="http://www.dlms.com/organization/flagmanufacturesids">List of manufacturer ID.</seealso>
        public byte[] SystemTitle
        {
            get
            {
                return systemTitle;
            }
            set
            {
                if (value != null && value.Length != 8)
                {
                    throw new ArgumentOutOfRangeException("Invalid System Title.");
                }
                systemTitle = value;
            }
        }

        /// <summary>
        /// Each block is ciphered with this key.
        /// </summary>
        public byte[] BlockCipherKey
        {
            get
            {
                return blockCipherKey;
            }
            set
            {
                if (value != null && value.Length != 16)
                {
                    throw new ArgumentOutOfRangeException("Invalid Block Cipher Key.");
                }
                blockCipherKey = value;
            }
        }

        /// <summary>
        /// Authentication Key is 16 bytes value.
        /// </summary>
        public byte[] AuthenticationKey
        {
            get
            {
                return authenticationKey;
            }
            set
            {
                if (value != null && value.Length != 16)
                {
                    throw new ArgumentOutOfRangeException("Invalid Authentication Key.");
                }
                authenticationKey = value;
            }
        }

        byte[] GXICipher.Encrypt(Command command, byte[] data)
        {
            if (Security != Security.None && command != Command.Aarq && command != Command.Aare)
            {
                byte[] tmp = GXDLMSChippering.EncryptAesGcm(command, Security,
                        FrameCounter, SystemTitle, BlockCipherKey,
                       AuthenticationKey, data);
                ++FrameCounter;                
                return tmp;
            }
            return data;
        }

        void GXICipher.Decrypt(GXByteBuffer data)
        {
            AesGcmParameter p = new AesGcmParameter(SystemTitle, BlockCipherKey, AuthenticationKey, data);
            byte[] tmp = GXDLMSChippering.DecryptAesGcm(p);
            data.Clear();
            data.Set(tmp);
        }

        public void Reset()
        {
            Security = Security.None;
            FrameCounter = 0;
        }

        bool GXICipher.IsCiphered()
        {
            return Security != Security.None;
        }
    }
}
