using Gurux.Common;
using Gurux.DLMS.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.XmlClient
{
    public class GXSettings
    {
        public IGXMedia media = null;
        public TraceLevel trace = TraceLevel.Info;
        public bool iec = false;
        public string path = null;
        public GXDLMSXmlClient client = new GXDLMSXmlClient(TranslatorOutputType.SimpleXml);
    }
}
