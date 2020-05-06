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
                Console.WriteLine(reply.ToString());
            }
            else
            {
                Console.WriteLine(Convert.ToString(reply));
            }
        }

        static int Main(string[] args)
        {
            Settings settings = new Settings();
            List<string> files = new List<string>();
            Reader.GXDLMSReader reader = null;
            try
            {
                ////////////////////////////////////////
                //Handle command line parameters.
                int ret = Settings.GetParameters(args, settings);
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
                            if (it.IsRequest())
                            {
                                reader.ReadDataBlock(settings.client.PduToMessages(it), reply);
                                HandleReply(it, reply);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("------------------------------------------------------------");
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        //Send disconnect if not in xml.
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
    }
}
