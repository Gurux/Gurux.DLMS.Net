using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS;
using Gurux.DLMS.Objects;

namespace GuruxDLMSServerExample
{
    /// <summary>
    /// DLMS Server that uses Short Name  referencing with 
    /// IEC 62056-46 Data link layer using HDLC protocol.
    /// </summary>
    /// <remarks>
    /// Example Landis&Gyr uses this.
    /// </remarks>
    class GXDLMSServerSN : GXDLMSBase
    {
        public GXDLMSServerSN() : base(false)
        {            
        }
    }
}
