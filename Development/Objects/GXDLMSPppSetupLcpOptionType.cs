using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public enum GXDLMSPppSetupLcpOptionType
    {
        MaxRecUnit = 1,
        AsyncControlCharMap = 2,
        AuthProtocol = 3,
        MagicNumber = 5,
        ProtocolFieldCompression = 7,
        AddressAndCtrCompression = 8,
        FCSAlternatives = 9,
        Callback = 13
    }
}
