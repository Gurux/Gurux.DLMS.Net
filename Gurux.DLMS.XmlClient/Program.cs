using Gurux.Common;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Reader;
using Gurux.Net;
using Gurux.Serial;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace Gurux.DLMS.XmlClient
{
    public class Program
    {
        /// <summary>
        /// Handle meter reply.
        /// </summary>
        /// <param name="item">Command to sent.</param>
        /// <param name="reply">Received reply.</param>
        static void HandleReply(GXDLMSXmlPdu item, GXReplyData reply)
        {
            if (reply.Value is byte[])
            {
                Console.WriteLine(GXCommon.ToHex((byte[])reply.Value, true));
            }
            else
            {
                Console.WriteLine(Convert.ToString(reply.Value));
            }
        }

        static int Main(string[] args)
        {
            List<string> files = new List<string>();
            GXDLMSReader reader = null;
            GXSettings settings = new GXSettings();
            try
            {
                ////////////////////////////////////////
                //Handle command line parameters.
                int ret = GetParameters(args, settings);
                if (ret != 0)
                {
                    return ret;
                }
                ////////////////////////////////////////
                //Initialize connection settings.
                if (settings.media is GXSerial)
                {
                    GXSerial serial = settings.media as GXSerial;
                    if (settings.iec)
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
                else if (settings.media is GXNet)
                {
                }
                else
                {
                    throw new Exception("Unknown media type.");
                }
                if (settings.path == null)
                {
                    if (settings.client.UseLogicalNameReferencing)
                    {
                        settings.path = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "Messages\\LN");
                    }
                    else
                    {
                        settings.path = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "Messages\\SN");
                    }
                }
                FileAttributes attr = File.GetAttributes(settings.path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    files.AddRange(Directory.GetFiles(settings.path, "*.xml"));
                }
                else
                {
                    files.Add(settings.path);
                }
                //Execute messages.
                foreach (string file in files)
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    if (settings.trace > TraceLevel.Warning)
                    {
                        Console.WriteLine("------------------------------------------------------------");
                        Console.WriteLine(name);
                    }
                    List<GXDLMSXmlPdu> actions = settings.client.Load(file);
                    if (actions.Count == 0)
                    {
                        continue;
                    }
                    try
                    {
                        settings.media.Open();
                        reader = new Reader.GXDLMSReader(settings.client, settings.media, settings.trace);
                        GXReplyData reply = new GXReplyData();
                        //Send SNRM if not in xml.
                        if (settings.client.InterfaceType == InterfaceType.HDLC)
                        {
                            if (!ContainsCommand(actions, Command.Snrm))
                            {
                                reader.SNRMRequest();
                            }
                        }

                        //Send AARQ if not in xml.
                        if (!ContainsCommand(actions, Command.Aarq))
                        {
                            if (!ContainsCommand(actions, Command.Snrm))
                            {
                                reader.AarqRequest();
                            }
                        }

                        foreach (GXDLMSXmlPdu it in actions)
                        {
                            if (it.Command == Command.Snrm && settings.client.InterfaceType == InterfaceType.WRAPPER)
                            {
                                continue;
                            }
                            if (it.Command == Command.DisconnectRequest && settings.client.InterfaceType == InterfaceType.WRAPPER)
                            {
                                break;
                            }
                            //Send
                            reply.Clear();
                            if (settings.trace > TraceLevel.Warning)
                            {
                                Console.WriteLine("------------------------------------------------------------");
                                Console.WriteLine(it.ToString());
                            }
                            reader.ReadDataBlock(settings.client.PduToMessages(it), reply);
                            HandleReply(it, reply);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("------------------------------------------------------------");
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        //Send AARQ if not in xml.
                        if (!ContainsCommand(actions, Command.DisconnectRequest))
                        {
                            reader.Disconnect();
                        }
                        else
                        {
                            settings.media.Close();
                        }
                    }

                }
                return 0;
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (settings.trace > TraceLevel.Off)
                {
                    Console.WriteLine("------------------------------------------------------------");
                    Console.WriteLine(ex.Message);
                }
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.ReadKey();
                }
                return 1;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Is command in XML file.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        static bool ContainsCommand(List<GXDLMSXmlPdu> actions, Command command)
        {
            foreach (GXDLMSXmlPdu it in actions)
            {
                if (it.Command == command)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get command line parameters.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <param name="settings">Settings class where parameters are updated.</param>
        /// <returns>Return 1 if parameters are invalid.</returns>
        static int GetParameters(string[] args, GXSettings settings)
        {
            List<GXCmdParameter> parameters = GXCommon.GetParameters(args, "h:p:c:s:r:it:a:p:wP:x:S:");
            GXNet net = null;
            foreach (GXCmdParameter it in parameters)
            {
                switch (it.Tag)
                {
                    case 'w':
                        settings.client.InterfaceType = InterfaceType.WRAPPER;
                        break;
                    case 'r':
                        if (string.Compare(it.Value, "sn", true) == 0)
                        {
                            settings.client.UseLogicalNameReferencing = false;
                        }
                        else if (string.Compare(it.Value, "ln", true) == 0)
                        {
                            settings.client.UseLogicalNameReferencing = true;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid reference option.");
                        }
                        break;
                    case 'h':
                        //Host address.
                        if (settings.media == null)
                        {
                            settings.media = new GXNet();
                        }
                        net = settings.media as GXNet;
                        net.HostName = it.Value;
                        break;
                    case 't':
                        //Trace.
                        try
                        {
                            settings.trace = (TraceLevel)Enum.Parse(typeof(TraceLevel), it.Value);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid Authentication option. (Error, Warning, Info, Verbose, Off)");
                        }
                        break;
                    case 'p':
                        //Port.
                        if (settings.media == null)
                        {
                            settings.media = new GXNet();
                        }
                        net = settings.media as GXNet;
                        net.Port = int.Parse(it.Value);
                        break;
                    case 'P'://Password
                        settings.client.Password = ASCIIEncoding.ASCII.GetBytes(it.Value);
                        break;
                    case 'i':
                        //IEC.
                        settings.iec = true;
                        break;
                    case 'S'://Serial Port
                        settings.media = new GXSerial();
                        GXSerial serial = settings.media as GXSerial;
                        string[] tmp = it.Value.Split(':');
                        serial.PortName = tmp[0];
                        serial.BaudRate = int.Parse(tmp[1]);
                        serial.DataBits = int.Parse(tmp[2].Substring(0, 1));
                        serial.Parity = (Parity)Enum.Parse(typeof(Parity), tmp[2].Substring(1, tmp[2].Length - 2));
                        serial.StopBits = (StopBits)int.Parse(tmp[2].Substring(tmp[2].Length - 1, 1));
                        break;
                    case 'a':
                        try
                        {
                            settings.client.Authentication = (Authentication)Enum.Parse(typeof(Authentication), it.Value);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid Authentication option. (None, Low, High, HighMd5, HighSha1, HighGmac, HighSha256)");
                        }
                        break;
                    case 'o':
                        break;
                    case 'c':
                        settings.client.ClientAddress = int.Parse(it.Value);
                        break;
                    case 's':
                        settings.client.ServerAddress = int.Parse(it.Value);
                        break;
                    case 'x':
                        settings.path = it.Value;
                        break;
                    case '?':
                        switch (it.Tag)
                        {
                            case 'c':
                                throw new ArgumentException("Missing mandatory client option.");
                            case 's':
                                throw new ArgumentException("Missing mandatory server option.");
                            case 'h':
                                throw new ArgumentException("Missing mandatory host name option.");
                            case 'p':
                                throw new ArgumentException("Missing mandatory port option.");
                            case 'r':
                                throw new ArgumentException("Missing mandatory reference option.");
                            case 'a':
                                throw new ArgumentException("Missing mandatory authentication option.");
                            case 'S':
                                throw new ArgumentException("Missing mandatory Serial port option.\n");
                            case 't':
                                throw new ArgumentException("Missing mandatory trace option.\n");
                            default:
                                ShowHelp();
                                return 1;
                        }
                    default:
                        ShowHelp();
                        return 1;
                }
            }
            if (settings.media == null)
            {
                ShowHelp();
                return 1;
            }
            return 0;
        }

        static void ShowHelp()
        {
            Console.WriteLine("Run Gurux Xml Client for DLMS/COSEM device.");
            Console.WriteLine("Gurux.DLMS.XmlClient -h [Meter IP Address] -p [Meter Port No] -c 16 -s 1 -r SN");
            Console.WriteLine(" -h \t host name or IP address.");
            Console.WriteLine(" -p \t port number or name (Example: 1000).");
            Console.WriteLine(" -S [COM1:9600:8None1]\t serial port.");
            Console.WriteLine(" -i IEC is a start protocol.");
            Console.WriteLine(" -a \t Authentication (None, Low, High).");
            Console.WriteLine(" -P \t Password for authentication.");
            Console.WriteLine(" -c \t Client address. (Default: 16)");
            Console.WriteLine(" -s \t Server address. (Default: 1)");
            Console.WriteLine(" -n \t Server address as serial number.");
            Console.WriteLine(" -r [sn, sn]\t Short name or Logican Name (default) referencing is used.");
            Console.WriteLine(" -w WRAPPER profile is used. HDLC is default.");
            Console.WriteLine(" -t [Error, Warning, Info, Verbose] Trace messages.");
            Console.WriteLine(" -x input XML file.");
            Console.WriteLine("Example:");
            Console.WriteLine("Gurux DLMS Xml Client TCP/IP connection.");
            Console.WriteLine("Gurux.DLMS.XmlClient -r SN -c 16 -s 1 -h [Meter IP Address] -p [Meter Port No]");
            Console.WriteLine("Gurux DLMS Xml Client using serial port connection.");
            Console.WriteLine("Gurux.DLMS.XmlClient -r SN -c 16 -s 1 -S COM1:9600:8None1 -i");
            Console.WriteLine("Gurux.DLMS.XmlClient -S COM1:9600:8None1 -c 16 -s 1 -a Low -P [password]");
        }

    }
}
