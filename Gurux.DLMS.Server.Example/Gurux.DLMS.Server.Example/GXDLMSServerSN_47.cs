using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS;
using Gurux.DLMS.Objects;

namespace GuruxDLMSServerExample
{
    /// <summary>
    /// DLMS Server that uses Short Same referencing with
    /// IEC 62056-47 COSEM transport layers for IPv4 networks.
    /// </summary>
    /// <remarks>
    /// Example Iskraemeco uses this.
    /// Note! For serial port communication is used GXDLMSServerSN.
    /// </remarks>
    class GXDLMSServerSN_47 : GXDLMSBase
    {
        public GXDLMSServerSN_47()
            : base(false)
        {
            this.InterfaceType = InterfaceType.Net;
        }
    }
}
