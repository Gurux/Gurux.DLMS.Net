
namespace Gurux.DLMS.Secure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Security.Cryptography;
    using Gurux.DLMS.Internal;
    using Gurux.DLMS.Enums;

    public abstract class GXDLMSSecureServerBase : GXDLMSServerBase
    {       
        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="logicalNameReferencing">
        /// Is logical name referencing used. 
        ///</param>
        ///<param name="type">
        /// Interface type. 
        ///</param>
        public GXDLMSSecureServerBase(bool logicalNameReferencing, InterfaceType type) :
            base(logicalNameReferencing, type)
        {
            Ciphering = new GXCiphering(ASCIIEncoding.ASCII.GetBytes("ABCDEFGH"));
            Cipher = Ciphering;
        }

        /// <summary>
        /// Ciphering settings.
        /// </summary>
        public GXCiphering Ciphering
        {
            get;
            private set;
        }

        /// <summary>
        /// Server to Client challenge.         
        /// </summary>
        public byte[] StoCChallenge
        {
            get
            {
                return Settings.StoCChallenge;
            }
            set
            {
                Settings.StoCChallenge = value;
            }
        }
        
    }
}
