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

using Gurux.Common;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.Net;
using Gurux.Serial;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Gurux.DLMS.Simulator.Net
{
    class Program
    {
        /// <summary>
        /// Read simulated values from the meter.
        /// </summary>
        static void ReadSimulatedValues(Settings settings)
        {
            Reader.GXDLMSReader reader = null;
            try
            {
                ////////////////////////////////////////
                //Initialise connection settings.
                if (settings.media is GXSerial)
                {
                }
                else if (settings.media is GXNet)
                {
                }
                else
                {
                    throw new Exception("Unknown media type.");
                }
                ////////////////////////////////////////
                reader = new Reader.GXDLMSReader(settings.client, settings.media, settings.trace, settings.invocationCounter);
                if (settings.gatewaySettings != null)
                {
                    settings.client.Gateway.NetworkId = Convert.ToByte(settings.gatewaySettings);
                }
                settings.media.Open();
                //Some meters need a break here.
                Thread.Sleep(1000);
                reader.ReadAll(settings.outputFile);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }    

        /// <summary>
        /// Password are given as command line parameters 
        /// because they can't read from the meter.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="server"></param>
        static private void UpdateSettings(Settings settings, GXDLMSMeter server)
        {
            bool changed = false;
            if (settings.client.Password != null)
            {
                foreach (GXDLMSAssociationLogicalName it in server.Items.GetObjects(ObjectType.AssociationLogicalName))
                {
                    if (GXCommon.ToHex(it.Secret) != GXCommon.ToHex(settings.client.Password))
                    {
                        it.Secret = settings.client.Password;
                        changed = true;
                    }
                }
            }
            //Update IP address to the meter.
            if (settings.media is GXNet net && !string.IsNullOrEmpty(net.HostName))
            {
                IPAddress ipAddress = IPAddress.Parse(net.HostName);
                IPAddress subnetMask = null;
                IPAddress gateway = null;
                string mac = null;
                try
                {
                    foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (ni.OperationalStatus == OperationalStatus.Up &&
                            ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        {
                            foreach (UnicastIPAddressInformation ipInfo in ni.GetIPProperties().UnicastAddresses)
                            {
                                if (ipInfo.Address.ToString() == ipAddress.ToString())
                                {
                                    mac = string.Join(":", ni.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
                                    subnetMask = ipInfo.IPv4Mask;
                                    foreach (var it in ni.GetIPProperties().GatewayAddresses)
                                    {
                                        if (it.Address.AddressFamily == AddressFamily.InterNetwork)
                                        {
                                            gateway = it.Address;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        if (subnetMask != null)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Find first IP address.
                    foreach (var it in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                    {
                        if (it.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddress = it;
                            break;
                        }
                    }
                }

                IPAddress dnsAddress = null;
                foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    var dnsAddresses = adapterProperties.DnsAddresses;

                    if (dnsAddresses.Count > 0)
                    {
                        foreach (var dns in dnsAddresses)
                        {
                            dnsAddress = dns;
                            break;
                        }
                    }
                }

                if (mac != null)
                {
                    foreach (GXDLMSMacAddressSetup it in server.Items.GetObjects(ObjectType.MacAddressSetup))
                    {
                        if (it.MacAddress != mac)
                        {
                            it.MacAddress = mac;
                            changed = true;
                        }
                    }
                }
                if (ipAddress != null || dnsAddress != null)
                {
                    foreach (GXDLMSIp4Setup it in server.Items.GetObjects(ObjectType.Ip4Setup))
                    {
                        if (ipAddress != null)
                        {
                            if (it.IPAddress.ToString() != ipAddress.ToString())
                            {
                                it.IPAddress = ipAddress;
                                it.SubnetMask = subnetMask;
                                it.GatewayIPAddress = gateway;
                                changed = true;
                            }
                        }
                        if (dnsAddress != null)
                        {
                            if (it.PrimaryDNSAddress.ToString() != dnsAddress.ToString())
                            {
                                it.PrimaryDNSAddress = dnsAddress;
                                changed = true;
                            }
                        }
                    }
                    foreach (GXDLMSIp6Setup it in server.Items.GetObjects(ObjectType.Ip6Setup))
                    {
                        if (dnsAddress != null)
                        {
                            if (it.PrimaryDNSAddress.ToString() != dnsAddress.ToString())
                            {
                                it.PrimaryDNSAddress = dnsAddress;
                                changed = true;
                            }
                        }
                    }
                }
            }
            if (changed)
            {
                GXXmlWriterSettings s = new GXXmlWriterSettings();
                server.Items.Save(settings.inputFile, s);
            }
        }

        /// <summary>
        /// Start simulator.
        /// </summary>
        static void StartSimulator(Settings settings)
        {
            if (settings.media is GXSerial)
            {
                GXDLMSMeter server = new GXDLMSMeter(settings.client.UseLogicalNameReferencing, settings.client.InterfaceType,
                                                        settings.client.UseUtc2NormalTime, settings.client.ManufacturerId);
                if (settings.client.UseLogicalNameReferencing)
                {
                    Console.WriteLine("Logical Name DLMS Server in serial port {0} using {1}.", settings.media, settings.client.InterfaceType);
                }
                else
                {
                    Console.WriteLine("Short Name DLMS Server in serial port {0} using {1}.", settings.media, settings.client.InterfaceType);
                }
                server.Initialize(settings.media, settings.trace, settings.inputFile, 1, false, null);
                UpdateSettings(settings, server);
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
                server.Close();
            }
            else
            {
                //Create Network media component and start listen events.
                //4059 is Official DLMS port.
                ///////////////////////////////////////////////////////////////////////
                //Create Gurux DLMS server component for Short Name and start listen events.
                List<GXDLMSMeter> servers = new List<GXDLMSMeter>();
                string str = "DLMS " + settings.client.InterfaceType;
                if (settings.gatewaySettings != null)
                {
                    str += " Gateway for " + settings.gatewaySettings + " meters.";
                }
                if (settings.client.UseLogicalNameReferencing)
                {
                    str += " Logical Name ";
                }
                else
                {
                    str += " Short Name ";
                }
                GXNet net = (GXNet)settings.media;
                net.Server = true;
                if (settings.exclusive)
                {
                    Console.WriteLine(str + "simulator start in port {0} implementing {1} meters.", net.Port, settings.serverCount);
                    if (settings.gatewaySettings != null)
                    {
                        net.OnReceived += new Gurux.Common.ReceivedEventHandler(GXDLMSMeter.OnGatewayReceived);
                        GXDLMSMeter.GatewayServer = new GXDLMSMeter(settings.client.UseLogicalNameReferencing, settings.client.InterfaceType,
                                                                    settings.client.UseUtc2NormalTime,
                                                                    settings.client.ManufacturerId);
                        GXDLMSMeter.GatewayServer.Initialize();
                        UpdateSettings(settings, GXDLMSMeter.GatewayServer);
                        settings.client.InterfaceType = (InterfaceType)settings.gatewaySettings;
                    }
                    else
                    {
                        net.OnReceived += new Gurux.Common.ReceivedEventHandler(GXDLMSMeter.OnExclusiveReceived);
                    }
                    net.OnClientConnected += GXDLMSMeter.OnClientConnected;
                    net.OnClientDisconnected += GXDLMSMeter.OnClientDisconnected;
                    net.OnError += new Gurux.Common.ErrorEventHandler(GXDLMSMeter.OnError);
                }
                else
                {
                    Console.WriteLine(str + "simulator start in {0} ports {1}-{2}.", net.Protocol, net.Port, net.Port + settings.serverCount - 1);
                }
                int index = 0;
                if (settings.useSerialNumberAsMeterAddress)
                {
                    index = settings.client.Settings.ServerAddress - 1;
                }
                GXDLMSObjectCollection sharedObjects = null;
                for (int pos = 0; pos != settings.serverCount; ++pos)
                {
                    ++index;
                    GXDLMSMeter server = new GXDLMSMeter(settings.client.UseLogicalNameReferencing, settings.client.InterfaceType,
                                                        settings.client.UseUtc2NormalTime, settings.client.ManufacturerId);
                    servers.Add(server);
                    if (settings.SharedObjects && settings.inputFile != null)
                    {
                        sharedObjects = new GXDLMSObjectCollection();
                        server.LoadObjects(settings.inputFile, sharedObjects);
                        settings.inputFile = null;
                    }
                    if (settings.exclusive)
                    {
                        server.Initialize(net, settings.trace,
                            settings.inputFile, (UInt32)index,
                            settings.exclusive, sharedObjects);
                        GXDLMSMeter.meters.Add(index, server);
                    }
                    else
                    {
                        try
                        {
                            server.Initialize(new GXNet(net.Protocol, net.Port + pos), settings.trace,
                                settings.inputFile, (UInt32)index + 1,
                                settings.exclusive, sharedObjects);
                        }
                        catch (System.Net.Sockets.SocketException ex)
                        {
                            Console.WriteLine(string.Format("Port {0} already in use.", net.Port + pos));
                        }
                    }
                    if (pos == 0)
                    {
                        UpdateSettings(settings, server);
                    }
                    if (pos == 0 && settings.client.UseLogicalNameReferencing)
                    {
                        str = "Server address: " + settings.client.ServerAddress.ToString();
                        Console.WriteLine(str);
                        Console.WriteLine("Associations:");
                        foreach (GXDLMSAssociationLogicalName it in server.Items.GetObjects(ObjectType.AssociationLogicalName))
                        {
                            str = "++++++++++++++++++++++++++++" + Environment.NewLine;
                            //Overwrite the password.
                            if (settings.client.Password != null && settings.client.Password.Length != 0)
                            {
                                it.Secret = settings.client.Password;
                            }
                            str += "Client address: " + it.ClientSAP.ToString();
                            if (it.AuthenticationMechanismName.MechanismId == Authentication.None)
                            {
                                str += " Without authentication.";
                            }
                            else
                            {
                                str += string.Format(" {0} authentication",
                                    it.AuthenticationMechanismName.MechanismId);
                                if (it.Secret != null)
                                {
                                    str += string.Format(", password {0}", ASCIIEncoding.ASCII.GetString(it.Secret));
                                }
                            }
                            str += Environment.NewLine + " Conformance:" + Environment.NewLine;
                            str += it.XDLMSContextInfo.Conformance + Environment.NewLine;
                            str += " MaxReceivePduSize: " + it.XDLMSContextInfo.MaxReceivePduSize;
                            str += " MaxSendPduSize: " + it.XDLMSContextInfo.MaxSendPduSize + Environment.NewLine;
                            GXDLMSSecuritySetup ss = server.Items.FindByLN(ObjectType.SecuritySetup, it.SecuritySetupReference) as GXDLMSSecuritySetup;
                            if (ss != null)
                            {
                                str += Environment.NewLine;
                                str += " Security suite: " + ss.SecuritySuite;
                                str += Environment.NewLine;
                                str += " Security policy: " + ss.SecurityPolicy;
                                str += Environment.NewLine;
                                str += " Authentication key: " + GXDLMSTranslator.ToHex(ss.Gak);
                                str += Environment.NewLine;
                                str += " Block cipher key: " + GXDLMSTranslator.ToHex(ss.Guek);
                                str += Environment.NewLine;
                                if (ss.Gbek != null)
                                {
                                    str += " Broadcast block cipher key: " + GXDLMSTranslator.ToHex(ss.Gbek);
                                }
                                str += Environment.NewLine;
                            }
                            if (settings.SharedObjects)
                            {
                                str = "++++++++++++++++++++++++++++" + Environment.NewLine;
                                str += "All meters are sharing the COSEM objects.";
                                str += Environment.NewLine;
                            }
                            Console.WriteLine(str);
                        }
                    }

                }
                ConsoleKey k;
                while ((k = Console.ReadKey().Key) != ConsoleKey.Escape)
                {
                    if (k == ConsoleKey.Delete)
                    {
                        Console.Clear();
                    }
                    Console.WriteLine("Press Esc to close application or delete clear the console.");
                }
                Console.WriteLine("Closing servers.");
                //Close servers.
                foreach (GXDLMSMeter server in servers)
                {
                    server.Close();
                }
                Console.WriteLine("Servers closed.");
            }
        }

        static int Main(string[] args)
        {
            try
            {
                Settings settings = new Settings();
                int ret = Settings.GetParameters(args, settings);
                if (ret != 0)
                {
                    return ret;
                }
                if (!string.IsNullOrEmpty(settings.outputFile))
                {
                    ReadSimulatedValues(settings);
                    Console.WriteLine("----------------------------------------------------------");
                    Console.WriteLine("Simulator template is created: " + settings.outputFile);
                }
                else if (!string.IsNullOrEmpty(settings.inputFile))
                {
                    StartSimulator(settings);
                }
                else
                {
                    Console.WriteLine("Device values file is not given.");
                }
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Available ports:");
                Console.WriteLine(string.Join(" ", Gurux.Serial.GXSerial.GetPortNames()));
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
            return 0;
        }
    }
}
