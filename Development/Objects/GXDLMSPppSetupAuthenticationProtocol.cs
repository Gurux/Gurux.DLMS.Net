using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// The value of the Auth-Prot (Authentication Protocol) element indicates
    /// the authentication protocol used on the given PPP link.
    /// </summary>
    public enum GXDLMSPppSetupAuthenticationProtocol
    {
        /// <summary>
        /// No authentication protocol is used.
        /// </summary>
        None = 0,
        /// <summary>
        /// The PAP protocol is used.
        /// </summary>
        PAP = 0xc023,
        /// <summary>
        /// The CHAP protocol is used.
        /// </summary>
        CHAP = 0xc223,
        /// <summary>
        /// The EAP protocol is used.
        /// </summary>
        EAP = 0xc227
    }
}
