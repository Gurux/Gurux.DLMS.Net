using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS;
using Gurux.DLMS.Objects;

namespace GuruxDLMSServerExample
{
    /// <summary>
    /// DLMS Server that uses Logical Name referencing with 
    /// IEC 62056-46 Data link layer using HDLC protocol.
    /// </summary>
    /// <remarks>
    /// Example Iskraemeco and Actaris uses this.
    /// </remarks>
    class GXDLMSServerLN : GXDLMSBase
    {
        public GXDLMSServerLN()
            : base(true)
        {            
        }
    }
}
