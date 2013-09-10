using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public enum GXDLMSPppSetupIPCompressionProtocol
    {
        /// <summary>
        /// No IP Compression is used (default).
        /// </summary>
        None = 0,
        /// <summary>
        /// Van Jacobson (RFC 1332).
        /// </summary>
        VanJacobson = 0x002d,
        /// <summary>
        /// IP Header Compression (RFC 2507, 3544).
        /// </summary>
        IPHeaderCompression = 0x0061,
        /// <summary>
        /// Robust Header Compression (RFC 3241).
        /// </summary>
        RobustHeaderCompression = 0x0003
    }
}
