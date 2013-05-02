using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.Common;
using Gurux.Serial;
using Gurux.DLMS;
using Gurux.DLMS.ManufacturerSettings;
using System.IO;
using System.Xml.Serialization;
using Gurux.DLMS.Objects;

namespace GuruxDLMSDemoAppCSharp
{
    class Program
    {
        static void ShowHelp()
        {
            Console.WriteLine("GuruxDlmsSample reads data from the DLMS/COSEM device.");
            Console.WriteLine("Note! Before first use update initial settings with /u.");
            Console.WriteLine("");
            Console.WriteLine("GuruxDlmsSample /m=lgz /h=www.gurux.org /p=1000 [/s=] [/u]");
            Console.WriteLine(" /u\t Update meter settings from Gurux web portal.");
            Console.WriteLine(" /m=\t manufacturer identifier.");
            Console.WriteLine(" /sp=\t serial port.");
            Console.WriteLine(" /h=\t host name.");
            Console.WriteLine(" /p=\t port number or name (Example: 1000).");
            Console.WriteLine(" /s=\t start protocol (IEC or DLMS).");
            Console.WriteLine(" /a=\t Authentication (None, Low, High).");
            Console.WriteLine(" /pw=\t Password for authentication.");
            Console.WriteLine(" /t\t Trace messages.");
            Console.WriteLine("Example:");
            Console.WriteLine("Read LG device using TCP/IP connection.");
            Console.WriteLine("GuruxDlmsSample /m=lgz /h=www.gurux.org /p=1000");
            Console.WriteLine("Read LG device using serial port connection.");
            Console.WriteLine("GuruxDlmsSample /m=lgz /sp=COM1 /s=DLMS");
        }

        static void Main(string[] args)
        {
            IGXMedia media = null;
            GXCommunicatation comm = null;
            try
            {
                ////////////////////////////////////////
                //Handle command line parameters.
                String id = "", host = "", port = "", pw = "";
                bool trace = false, iec = true, isSerial = false;
                Authentication auth = Authentication.None;
                foreach (string it in args)
                {
                    String item = it.Trim().ToLower();
                    if (string.Compare(item, "/u", true) == 0)//Update
                    {
                        //Get latest manufacturer settings from Gurux web server.
                        GXManufacturerCollection.UpdateManufactureSettings();
                    }
                    else if (item.StartsWith("/m="))//Manufacturer
                    {
                        id = item.Replace("/m=", "");
                    }
                    else if (item.StartsWith("/h=")) //Host
                    {
                        host = item.Replace("/h=", "");
                    }
                    else if (item.StartsWith("/p="))// TCP/IP Port
                    {
                        media = new Gurux.Net.GXNet();
                        port = item.Replace("/p=", "");
                    }
                    else if (item.StartsWith("/sp="))//Serial Port
                    {
                        port = item.Replace("/sp=", "");
                        isSerial = true;
                        media = new GXSerial();
                    }
                    else if (item.StartsWith("/t"))//Are messages traced.
                    {
                        trace = true;
                    }
                    else if (item.StartsWith("/s="))//Start
                    {
                        String tmp = item.Replace("/s=", "");
                        iec = string.Compare(tmp, "dlms", true) != 0;
                    }
                    else if (item.StartsWith("/a="))//Authentication
                    {
                        auth = (Authentication)Enum.Parse(typeof(Authentication), it.Trim().Replace("/a=", ""));
                    }
                    else if (item.StartsWith("/pw="))//Password
                    {
                        pw = it.Trim().Replace("/pw=", "");
                    }
                    else
                    {
                        ShowHelp();
                        return;
                    }
                }
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(port) || (!isSerial && string.IsNullOrEmpty(host)))
                {
                    ShowHelp();
                    return;
                }
                ////////////////////////////////////////
                //Initialize connection settings.
                if (isSerial)
                {
                    GXSerial serial = media as GXSerial;
                    string[] t = GXSerial.GetPortNames();
                    serial.PortName = port;
                    if (iec)
                    {
                        serial.BaudRate = 300;
                        serial.DataBits = 7;
                        serial.Parity = System.IO.Ports.Parity.Even;
                        serial.StopBits = System.IO.Ports.StopBits.One;
                    }
                    else
                    {
                        serial.BaudRate = 9600;
                        serial.DataBits = 8;
                        serial.Parity = System.IO.Ports.Parity.None;
                        serial.StopBits = System.IO.Ports.StopBits.One;
                    }
                }
                else
                {
                    Gurux.Net.GXNet net = media as Gurux.Net.GXNet;
                    net.Port = Convert.ToInt32(port);
                    net.HostName = host;
                    net.Protocol = Gurux.Net.NetworkType.Tcp;
                }
                ////////////////////////////////////////
                //Update manufacturer debended settings.
                GXManufacturerCollection Manufacturers = new GXManufacturerCollection();
                GXManufacturerCollection.ReadManufacturerSettings(Manufacturers);
                GXManufacturer man = Manufacturers.FindByIdentification(id);
                if (man == null)
                {
                    throw new Exception("Unknown manufacturer: " + id);
                }
                GXDLMSClient dlms = new GXDLMSClient();
                //Update Obis code list so we can get right descriptions to the objects.
                dlms.ObisCodes = man.ObisCodes;
                comm = new GXCommunicatation(dlms, media, iec, auth, pw);
                comm.Trace = trace;
                comm.InitializeConnection(man);

                //Save Association view to the cache so it is not needed to retreave every time.
                string path = man.Identification + ".xml";
                GXDLMSObjectCollection objects = null;
                List<Type> extraTypes = new List<Type>(Gurux.DLMS.GXDLMSClient.GetObjectTypes());
                extraTypes.Add(typeof(GXDLMSAttributeSettings));
                extraTypes.Add(typeof(GXDLMSAttribute));
                XmlSerializer x = new XmlSerializer(typeof(GXDLMSObjectCollection), extraTypes.ToArray());
                if (File.Exists(path))
                {
                    try
                    {
                        using (Stream stream = File.Open(path, FileMode.Open))
                        {
                            Console.WriteLine("Get available objects from the cache.");                            
                            objects = x.Deserialize(stream) as GXDLMSObjectCollection;
                            stream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        throw ex;
                    }
                }
                else
                {
                    Console.WriteLine("Get available objects from the device.");
                    objects = comm.GetAssociationView();
                    try
                    {
                        using (Stream stream = File.Open(path, FileMode.Create))
                        {                            
                            TextWriter writer = new StreamWriter(stream);                            
                            x.Serialize(writer, objects);
                            writer.Close();
                            stream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        throw ex;
                    }
                    Console.WriteLine("--- Available objects ---");
                    foreach (GXDLMSObject it in objects)
                    {
                        Console.WriteLine(it.Name + " " + it.Description);
                    }
                }              
                //Read available clock and data objects.
                foreach (GXDLMSObject it in objects.GetObjects(new ObjectType[] { ObjectType.Clock, ObjectType.Data}))
                {
                    object value = comm.Read(it, 2);
                    Console.WriteLine(it.Name + " " + it.Description + " " + value);
                }
                //Read Profile Generic columns.
                GXDLMSObjectCollection cols = null;
                foreach (GXDLMSObject it in objects.GetObjects(ObjectType.ProfileGeneric))
                {
                    Console.WriteLine("Profile Generic " + it.Name + " Columns:");
                    cols = comm.GetColumns(it);
                    foreach (GXDLMSObject col in cols)
                    {
                        Console.Write(col.Name + " | ");
                    }
                    Console.WriteLine("");
                    //Read first row from Profile Generic.
                    Console.WriteLine("Profile Generic " + it.Name + " Columns:");
                    object[] rows = comm.ReadRowsByEntry(it, 0, 1, cols);
                    foreach (object[] row in rows)
                    {
                        foreach (object cell in row)
                        {
                            Console.Write(cell + " | ");
                        }
                        Console.WriteLine("");
                    }
                    //Read last day from Profile Generic.
                    Console.WriteLine("Profile Generic " + it.Name + " Columns:");
                    rows = comm.ReadRowsByRange(it, DateTime.Now.Date, DateTime.MaxValue, cols);
                    foreach (object[] row in rows)
                    {
                        foreach (object cell in row)
                        {
                            Console.Write(cell + " | ");
                        }
                        Console.WriteLine("");
                    }
                }
            }
            catch (Exception ex)
            {
                if (comm != null)
                {
                    comm.Close();
                }
                Console.WriteLine(ex.Message);
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    Console.ReadKey();
                }
            }
            finally
            {
                if (comm != null)
                {
                    comm.Close();
                }
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.ReadKey();
                }
            }
        }
    }
}
