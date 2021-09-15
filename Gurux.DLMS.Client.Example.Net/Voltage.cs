using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Client.Example.Net
{
    public class Voltage
    {

        public static IEGReader Intializer()
        {
            IEGReader eGReader = new IEGReader();

            eGReader.Initialize_Connection();
            
            return eGReader;
        }

        public static double Reader(IEGReader eGReader)
        {
            object val = eGReader.Read_Object("1.0.32.7.0.255", 2);
            return (double)val;

        }

        public static void Closer(IEGReader eGReader)
        {
            eGReader.Close_Connection();
        }

        
    }
}
