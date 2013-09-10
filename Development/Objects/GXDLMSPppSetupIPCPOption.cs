using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSPppSetupIPCPOption
    {
        public GXDLMSPppSetupIPCPOptionType Type
        {
            get;
            set;
        }
        public byte Length
        {
            get;
            set;
        }

        public object Data
        {
            get;
            set;
        }
    }
}
