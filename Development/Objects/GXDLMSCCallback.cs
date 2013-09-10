using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSCCallback
    {
        public bool Active
        {
            get;
            set;
        }

        public byte Length
        {
            get;
            set;
        }

        public byte Operation
        {
            get;
            set;
        }

        public String Message
        {
            get;
            set;
        }
    }
}
