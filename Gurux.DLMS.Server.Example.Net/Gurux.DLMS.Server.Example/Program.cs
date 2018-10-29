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
using System.Text;
using Gurux.Net;
using System.Diagnostics;
using Gurux.Common;
using Gurux.DLMS.Secure;

namespace GuruxDLMSServerExample
{
    class Settings
    {
        public TraceLevel trace = TraceLevel.Info;
        public int port = 4060;
        public string serial;
    }

    class Program
    {
        static int Main(string[] args)
        {
            //Create Network media component and start listen events.
            //4059 is Official DLMS port.
            Settings settings = new Settings();
            int ret = GetParameters(args, settings);
            if (ret != 0)
            {
                return ret;
            }
            try
            {
                if (settings.serial != null)
                {
                    GXDLMSServerLN LNServer = new GXDLMSServerLN();
                    LNServer.Initialize(settings.serial, settings.trace);
                    Console.WriteLine("Logical Name DLMS Server in serial port {0}.", settings.serial);
                    Console.WriteLine("----------------------------------------------------------");
                    ConsoleKey k;
                    while ((k = Console.ReadKey().Key) != ConsoleKey.Escape)
                    {
                        if (k == ConsoleKey.Delete)
                        {
                            Console.Clear();
                        }
                        Console.WriteLine("Press Esc to close application or delete clear the console.");
                    }
                    //Close servers.
                    LNServer.Close();
                }
                else
                {
                    ///////////////////////////////////////////////////////////////////////
                    //Create Gurux DLMS server component for Short Name and start listen events.
                    GXDLMSServerSN SNServer = new GXDLMSServerSN();
                    SNServer.Initialize(settings.port, settings.trace);
                    Console.WriteLine("Short Name DLMS Server in port {0}.", settings.port);
                    Console.WriteLine("Example connection settings:");
                    Console.WriteLine("Gurux.DLMS.Client.Example.Net -r sn -h localhost -p {0}", settings.port);
                    Console.WriteLine("----------------------------------------------------------");
                    ///////////////////////////////////////////////////////////////////////
                    //Create Gurux DLMS server component for Short Name and start listen events.
                    GXDLMSServerLN LNServer = new GXDLMSServerLN();
                    LNServer.Initialize(settings.port + 1, settings.trace);
                    Console.WriteLine("Logical Name DLMS Server in port {0}.", settings.port + 1);
                    Console.WriteLine("Example connection settings:");
                    Console.WriteLine("Gurux.DLMS.Client.Example.Net -h localhost -p {0}", settings.port + 1);
                    Console.WriteLine("----------------------------------------------------------");
                    ///////////////////////////////////////////////////////////////////////
                    //Create Gurux DLMS server component for Short Name and start listen events.
                    GXDLMSServerSN_47 SN_47Server = new GXDLMSServerSN_47();
                    SN_47Server.Initialize(settings.port + 2, settings.trace);
                    Console.WriteLine("Short Name DLMS Server with IEC 62056-47 in port {0}.", settings.port + 2);
                    Console.WriteLine("Example connection settings:");
                    Console.WriteLine("Gurux.DLMS.Client.Example.Net -r sn -h localhost -p {0} -w", settings.port + 2);
                    Console.WriteLine("----------------------------------------------------------");
                    ///////////////////////////////////////////////////////////////////////
                    //Create Gurux DLMS server component for Short Name and start listen events.
                    GXDLMSServerLN_47 LN_47Server = new GXDLMSServerLN_47();
                    LN_47Server.Initialize(settings.port + 3, settings.trace);
                    Console.WriteLine("Logical Name DLMS Server with IEC 62056-47 in port {0}.", settings.port + 3);
                    Console.WriteLine("Example connection settings:");
                    Console.WriteLine("Gurux.DLMS.Client.Example.Net -h localhost -p {0} -w", settings.port + 3);
                    ConsoleKey k;
                    while ((k = Console.ReadKey().Key) != ConsoleKey.Escape)
                    {
                        if (k == ConsoleKey.Delete)
                        {
                            Console.Clear();
                        }
                        Console.WriteLine("Press Esc to close application or delete clear the console.");
                    }

                    //Close servers.
                    SNServer.Close();
                    LNServer.Close();
                    SN_47Server.Close();
                    LN_47Server.Close();
                    Console.WriteLine("Servers closed.");
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        static int GetParameters(string[] args, Settings settings)
        {
            List<GXCmdParameter> parameters = GXCommon.GetParameters(args, "t:p:s:");
            foreach (GXCmdParameter it in parameters)
            {
                switch (it.Tag)
                {
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
                        settings.port = int.Parse(it.Value);
                        break;
                    case 's':
                        //serial port.
                        settings.serial = it.Value;
                        break;
                    case '?':
                        switch (it.Tag)
                        {
                            case 'p':
                                throw new ArgumentException("Missing mandatory port option.");
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
            return 0;
        }

        static void ShowHelp()
        {
            Console.WriteLine("Gurux DLMS example Server implements four DLMS/COSEM devices.");
            Console.WriteLine(" -t [Error, Warning, Info, Verbose] Trace messages.");
            Console.WriteLine(" -p Start port number. Default is 4060.");
            Console.WriteLine(" -s Serial port.");
        }
    }
}
