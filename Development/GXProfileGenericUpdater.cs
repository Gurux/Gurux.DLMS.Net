using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS.Objects;
using System.Threading;

namespace Gurux.DLMS
{
    class GXProfileGenericUpdater
    {
        GXDLMSProfileGeneric Target;
        GXDLMSServerBase Server;
        public GXProfileGenericUpdater(GXDLMSServerBase server, GXDLMSProfileGeneric pg)
        {
            Server = server;
            Target = pg;
        }

        public void UpdateProfileGenericData()
        {
            while (true)
            {
                Thread.Sleep(Target.CapturePeriod * 1000);
                Target.Capture();
            }
        }
    }
}
