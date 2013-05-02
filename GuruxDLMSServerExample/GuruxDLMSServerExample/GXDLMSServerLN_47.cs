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
    /// IEC 62056-47 COSEM transport layers for IPv4 networks.
    /// </summary>
    /// <remarks>
    /// Example Iskraemeco uses this.
    /// Note! For serial port communication is used GXDLMSServerLN.
    /// </remarks>    
    class GXDLMSServerLN_47 : GXDLMSBase
    {
        public GXDLMSServerLN_47()
            : base(true)
        {
            this.InterfaceType = InterfaceType.Net;

        }
    }
}
