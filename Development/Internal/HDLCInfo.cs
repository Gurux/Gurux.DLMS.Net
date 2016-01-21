using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Internal
{
    /// <summary>
    /// Reserved for internal use.
    /// </summary>
    internal enum HDLCInfo : byte
    {
        MaxInfoTX = 0x5,
        MaxInfoRX = 0x6,
        WindowSizeTX = 0x7,
        WindowSizeRX = 0x8
    }
}
