
namespace Gurux.DLMS.Secure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Security.Cryptography;
    using Gurux.DLMS.Internal;
    using Gurux.DLMS.Enums;

    public class GXSecure
    {

        ///<summary> 
        /// Constructor. 
        ///</summary>
        private GXSecure()
        {

        }

        ///<summary>
        /// Chipher text.
        ///</summary>
        ///<param name="auth">
        ///Authentication level. 
        ///</param>
        ///<param name="data">
        ///Text to chipher. 
        ///</param>
        ///<param name="secret">
        ///Secret. 
        ///</param>
        ///<returns>
        ///Chiphered text.
        ///</returns>
        public static byte[] Secure(Authentication auth, byte[] data, byte[] secret)
        {
            byte[] tmp;
            if (auth == Authentication.High)
            {
                byte[] p = new byte[secret.Length + 15];
                byte[] s = new byte[16];
                byte[] x = new byte[16];
                int i;
                data.CopyTo(p, 0);
                secret.CopyTo(s, 0);
                for (i = 0; i < p.Length; i += 16)
                {
                    Buffer.BlockCopy(p, i, x, 0, 16);
                    GXAes128.Encrypt(x, s);
                    x.CopyTo(p, i);
                }
                return p;
            }
            // Get server Challenge.
            GXByteBuffer challenge = new GXByteBuffer();
            // Get shared secret
            challenge.Set(secret);
            challenge.Set(data);
            tmp = challenge.Array();
            if (auth == Authentication.HighMD5)
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    tmp = md5Hash.ComputeHash(tmp);
                    return tmp;
                }
            }
            if (auth == Authentication.HighSHA1)
            {
                using (SHA1 sha = new SHA1CryptoServiceProvider())
                {
                    tmp = sha.ComputeHash(tmp);
                    return tmp;
                }
            }
            return data;
        }

        ///<summary>
        ///Generates challenge.
        ///</summary>
        ///<param name="authentication">
        ///Used authentication. 
        ///</param>
        ///<returns> 
        ///Generated challenge. 
        ///</returns>
        public static byte[] GenerateChallenge(Authentication authentication)
        {
            Random r = new Random();
            // Random challenge is 8 to 64 bytes.
            int len = r.Next(57) + 8;
            byte[] result = new byte[len];
            for (int pos = 0; pos != len; ++pos)
            {
                // Allow printable characters only.
                do
                {
                    result[pos] = (byte)r.Next(0x7A);
                } while (result[pos] < 0x21);
            }
            return result;
        }
    }
}
