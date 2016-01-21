
using System.Text;
namespace Gurux.DLMS.Secure
{
    public class GXDLMSSecureClient : GXDLMSClient
    {
        ///<summary>
        /// Constructor.
        ///</summary>
        public GXDLMSSecureClient() : base()
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
    }
}
