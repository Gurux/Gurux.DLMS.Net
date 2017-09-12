//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

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
using Gurux.Net;
using Gurux.DLMS.Enums;
using System.Threading;
using Gurux.DLMS.Secure;
using System.Diagnostics;

namespace Gurux.DLMS.Client.Example
{
    public class Settings
    {
        public IGXMedia media = null;
        public TraceLevel trace = TraceLevel.Info;
        public bool iec = false;
        public GXDLMSSecureClient client = new GXDLMSSecureClient(true);
        //Objects to read.
        public List<KeyValuePair<string, int>> readObjects = new List<KeyValuePair<string, int>>();
    }

    class Program
    {
        static int Main(string[] args)
        {
            Settings settings = new Settings();
            Reader.GXDLMSReader reader = null;
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
                ////////////////////////////////////////
                reader = new Reader.GXDLMSReader(settings.client, settings.media, settings.trace);
                settings.media.Open();
                if (settings.readObjects.Count != 0)
                {
                    reader.InitializeConnection();
                    reader.GetAssociationView(false);
                    foreach (KeyValuePair<string, int> it in settings.readObjects)
                    {
                        object val = reader.Read(settings.client.Objects.FindByLN(ObjectType.None, it.Key), it.Value);
                        reader.ShowValue(val, it.Value);
                    }
                }
                else
                {
                    reader.ReadAll(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.ToString());
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
                    Console.WriteLine("Ended. Press any key to continue.");
                    Console.ReadKey();
                }
            }
            return 0;
        }

        static int GetParameters(string[] args, Settings settings)
        {
            List<GXCmdParameter> parameters = GXCommon.GetParameters(args, "h:p:c:s:r:it:a:p:wP:g:");
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
                    case 'g':
                        //Get (read) selected objects.
                        foreach (string o in it.Value.Split(new char[] { ';', ',' }))
                        {
                            string[] tmp = o.Split(new char[] { ':' });
                            if (tmp.Length != 2)
                            {
                                throw new ArgumentOutOfRangeException("Invalid Logical name or attribute index.");
                            }
                            settings.readObjects.Add(new KeyValuePair<string, int>(tmp[0].Trim(), int.Parse(tmp[1].Trim())));
                        }
                        break;
                    case 'S'://Serial Port
                        settings.media = new GXSerial();
                        GXSerial serial = settings.media as GXSerial;
                        serial.PortName = it.Value;
                        break;
                    case 'a':
                        try
                        {
                            if (string.Compare("None", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.None;
                            }
                            else if (string.Compare("Low", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.Low;
                            }
                            else if (string.Compare("High", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.High;
                            }
                            else if (string.Compare("HighMd5", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.HighMD5;
                            }
                            else if (string.Compare("HighSha1", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.HighSHA1;
                            }
                            else if (string.Compare("HighSha256", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.HighSHA256;
                            }
                            else if (string.Compare("HighGMac", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.HighGMAC;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid Authentication option: '" + it.Value + "'. (None, Low, High, HighMd5, HighSha1, HighGMac, HighSha256)");
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid Authentication option: '" + it.Value + "'. (None, Low, High, HighMd5, HighSha1, HighGMac, HighSha256)");
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
                                throw new ArgumentException("Missing mandatory Serial port option.");
                            case 't':
                                throw new ArgumentException("Missing mandatory trace option.");
                            case 'g':
                                throw new ArgumentException("Missing mandatory OBIS code option.");
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
            Console.WriteLine("GuruxDlmsSample reads data from the DLMS/COSEM device.");
            Console.WriteLine("GuruxDlmsSample -h [Meter IP Address] -p [Meter Port No] -c 16 -s 1 -r SN");
            Console.WriteLine(" -h \t host name or IP address.");
            Console.WriteLine(" -p \t port number or name (Example: 1000).");
            Console.WriteLine(" -S \t serial port.");
            Console.WriteLine(" -i IEC is a start protocol.");
            Console.WriteLine(" -a \t Authentication (None, Low, High).");
            Console.WriteLine(" -P \t Password for authentication.");
            Console.WriteLine(" -c \t Client address. (Default: 16)");
            Console.WriteLine(" -s \t Server address. (Default: 1)");
            Console.WriteLine(" -n \t Server address as serial number.");
            Console.WriteLine(" -r [sn, sn]\t Short name or Logican Name (default) referencing is used.");
            Console.WriteLine(" -w WRAPPER profile is used. HDLC is default.");
            Console.WriteLine(" -t [Error, Warning, Info, Verbose] Trace messages.");
            Console.WriteLine(" -g \"0.0.1.0.0.255:1; 0.0.1.0.0.255:2\" Get selected object(s) with given attribute index.");
            Console.WriteLine("Example:");
            Console.WriteLine("Read LG device using TCP/IP connection.");
            Console.WriteLine("GuruxDlmsSample -r SN -c 16 -s 1 -h [Meter IP Address] -p [Meter Port No]");
            Console.WriteLine("Read LG device using serial port connection.");
            Console.WriteLine("GuruxDlmsSample -r SN -c 16 -s 1 -sp COM1 -i");
            Console.WriteLine("Read Indian device using serial port connection.");
            Console.WriteLine("GuruxDlmsSample -S COM1 -c 16 -s 1 -a Low -P [password]");
        }
    }
}
