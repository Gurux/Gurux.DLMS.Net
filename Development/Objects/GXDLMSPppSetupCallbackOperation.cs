using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public enum GXDLMSPppSetupCallbackOperation
    {
        //Location is determined by user authentication.
        User = 0,
        //Dialling string,
        Dialling = 1,
        // Location identifier.
        Location = 2,
        //E.164 number.
        E_164 = 3,
        //X500 distinguished name.
        X500 = 4,
        //Location is determined during CBCP negotiation.
        CBCP = 6
    }
}
