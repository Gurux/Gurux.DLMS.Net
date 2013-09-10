using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSQosElement
    {
        public byte Precedence
        {
            get;
            set;
        }

        public byte Delay
        {
            get;
            set;
        }

        public byte Reliability
        {
            get;
            set;
        }

        public byte PeakThroughput
        {
            get;
            set;
        }

        public byte MeanThroughput
        {
            get;
            set;
        }
    }
}
