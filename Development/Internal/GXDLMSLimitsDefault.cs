using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Internal
{
    class GXDLMSLimitsDefault
    {
        internal const byte DefaultMaxInfoRX = 128;
        internal const byte DefaultMaxInfoTX = 128;
        internal const UInt32 DefaultWindowSizeRX = 1;
        internal const UInt32 DefaultWindowSizeTX = 1;

        internal static void SetValue(GXByteBuffer buff, object data)
        {
            GXByteBuffer tmp = new GXByteBuffer();
            tmp.Add(data);
            buff.Add((byte)tmp.Size);
            buff.Set(tmp.Array());
        }
    }
}
