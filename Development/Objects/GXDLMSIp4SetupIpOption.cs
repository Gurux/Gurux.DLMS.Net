using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSIp4SetupIpOption
    {
        public GXDLMSIp4SetupIpOptionType Type
        {
            get;
            set;
        }

        public byte Length
        {
            get;
            set;
        }

        public byte[] Data
        {
            get;
            set;
        }
    }
}
