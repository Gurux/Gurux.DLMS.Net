
namespace Gurux.DLMS.Client.Example.Net
{
    public class Credit
    {

        public static object Reader(IEGReader eGReader)
        {
            object val = eGReader.Read_Object("0.0.19.10.0.255", 2);
            return val;

        }


    }
}
