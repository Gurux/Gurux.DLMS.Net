﻿//
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
using Gurux.DLMS.Secure;
using Gurux.Net;
using Gurux.Serial;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace Gurux.DLMS.XmlClient
{
    public class Settings
    {
        public string path = null;
        public IGXMedia media = null;
        public TraceLevel trace = TraceLevel.Info;
        public GXDLMSXmlClient client = new GXDLMSXmlClient(TranslatorOutputType.SimpleXml);
        // Invocation counter (frame counter).
        public string invocationCounter = null;
        //Objects to read.
        public List<KeyValuePair<string, int>> readObjects = new List<KeyValuePair<string, int>>();
        //Cache file.
        public string outputFile = null;

        public static int GetParameters(string[] args, Settings settings)
        {
            //Has user give the custom serial port settings or are the default values used in mode E.
            bool modeEDefaultValues = true;
            string[] tmp;
            List<GXCmdParameter> parameters = GXCommon.GetParameters(args, "h:p:c:s:r:i:It:a:P:g:S:C:n:v:o:T:A:B:D:d:l:x:F:");
            GXSerial serial;
            GXNet net = null;
            foreach (GXCmdParameter it in parameters)
            {
                switch (it.Tag)
                {
                    case 'x':
                        settings.path = it.Value;
                        break;
                    case 'i':
                        try
                        {
                            settings.client.InterfaceType = (InterfaceType)Enum.Parse(typeof(InterfaceType), it.Value);
                            settings.client.Plc.Reset();
                            if (modeEDefaultValues && settings.client.InterfaceType == InterfaceType.HdlcWithModeE &&
                                settings.media is GXSerial)
                            {
                                serial = settings.media as GXSerial;
                                serial.BaudRate = 300;
                                serial.DataBits = 7;
                                serial.Parity = Parity.Even;
                                serial.StopBits = StopBits.One;
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid interface type option. (HDLC, WRAPPER, HdlcWithModeE, Plc, PlcHdlc)");
                        }
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
                            throw new ArgumentException("Invalid trace level option. (Error, Warning, Info, Verbose, Off)");
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
                    case 'I':
                        //AutoIncreaseInvokeID.
                        settings.client.AutoIncreaseInvokeID = true;
                        break;
                    case 'v':
                        settings.invocationCounter = it.Value.Trim();
                        Objects.GXDLMSObject.ValidateLogicalName(settings.invocationCounter);
                        break;
                    case 'g':
                        //Get (read) selected objects.
                        foreach (string o in it.Value.Split(new char[] { ';', ',' }))
                        {
                            tmp = o.Split(new char[] { ':' });
                            if (tmp.Length != 2)
                            {
                                throw new ArgumentOutOfRangeException("Invalid Logical name or attribute index.");
                            }
                            settings.readObjects.Add(new KeyValuePair<string, int>(tmp[0].Trim(), int.Parse(tmp[1].Trim())));
                        }
                        break;
                    case 'S'://Serial Port
                        settings.media = new GXSerial();
                        serial = settings.media as GXSerial;
                        tmp = it.Value.Split(':');
                        serial.PortName = tmp[0];
                        if (tmp.Length > 1)
                        {
                            modeEDefaultValues = false;
                            serial.BaudRate = int.Parse(tmp[1]);
                            serial.DataBits = int.Parse(tmp[2].Substring(0, 1));
                            serial.Parity = (Parity)Enum.Parse(typeof(Parity), tmp[2].Substring(1, tmp[2].Length - 2));
                            serial.StopBits = (StopBits)int.Parse(tmp[2].Substring(tmp[2].Length - 1, 1));
                        }
                        else
                        {
                            if (settings.client.InterfaceType == InterfaceType.HdlcWithModeE)
                            {
                                serial.BaudRate = 300;
                                serial.DataBits = 7;
                                serial.Parity = Parity.Even;
                                serial.StopBits = StopBits.One;
                            }
                            else
                            {
                                serial.BaudRate = 9600;
                                serial.DataBits = 8;
                                serial.Parity = Parity.None;
                                serial.StopBits = StopBits.One;
                            }
                        }
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
                    case 'C':
                        try
                        {
                            settings.client.Ciphering.Security = (Security) Enum.Parse(typeof(Security), it.Value);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid Ciphering option '" + it.Value + "'. (None, Authentication, Encryption, AuthenticationEncryption)");
                        }
                        break;
                    case 'T':
                        settings.client.Ciphering.SystemTitle = GXCommon.HexToBytes(it.Value);
                        break;
                    case 'A':
                        settings.client.Ciphering.AuthenticationKey = GXCommon.HexToBytes(it.Value);
                        break;
                    case 'B':
                        settings.client.Ciphering.BlockCipherKey = GXCommon.HexToBytes(it.Value);
                        break;
                    case 'D':
                        settings.client.Ciphering.DedicatedKey = GXCommon.HexToBytes(it.Value);
                        break;
                    case 'F':
                        settings.client.Ciphering.InvocationCounter = UInt32.Parse(it.Value.Trim());
                        break;
                    case 'o':
                        settings.outputFile = it.Value;
                        break;
                    case 'd':
                        try
                        {
                            settings.client.Standard = (Standard)Enum.Parse(typeof(Standard), it.Value);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid DLMS standard option '" + it.Value + "'. (DLMS, India, Italy, SaudiArabia, IDIS)");
                        }
                        break;
                    case 'c':
                        settings.client.ClientAddress = int.Parse(it.Value);
                        break;
                    case 's':
                        if (settings.client.ServerAddress != 1)
                        {
                            settings.client.ServerAddress = GXDLMSClient.GetServerAddress(settings.client.ServerAddress, int.Parse(it.Value));
                        }
                        else
                        {
                            settings.client.ServerAddress = int.Parse(it.Value);
                        }
                        break;
                    case 'l':
                        settings.client.ServerAddress = GXDLMSClient.GetServerAddress(int.Parse(it.Value), settings.client.ServerAddress);
                        break;
                    case 'n':
                        settings.client.ServerAddress = GXDLMSClient.GetServerAddressFromSerialNumber(int.Parse(it.Value), 1);
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
                            case 'C':
                                throw new ArgumentException("Missing mandatory Ciphering option.");
                            case 'v':
                                throw new ArgumentException("Missing mandatory invocation counter logical name option.");
                            case 'T':
                                throw new ArgumentException("Missing mandatory system title option.");
                            case 'A':
                                throw new ArgumentException("Missing mandatory authentication key option.");
                            case 'B':
                                throw new ArgumentException("Missing mandatory block cipher key option.");
                            case 'D':
                                throw new ArgumentException("Missing mandatory dedicated key option.");
                            case 'd':
                                throw new ArgumentException("Missing mandatory DLMS standard option.");
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
            Console.WriteLine("Gurux.DLMS.XmlClient reads data from the DLMS/COSEM device.");
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
            Console.WriteLine(" -l \t Logical Server address.");
            Console.WriteLine(" -r [sn, ln]\t Short name or Logical Name (default) referencing is used.");
            Console.WriteLine(" -w WRAPPER profile is used. HDLC is default.");
            Console.WriteLine(" -t [Error, Warning, Info, Verbose] Trace messages.");
            Console.WriteLine(" -g \"0.0.1.0.0.255:1; 0.0.1.0.0.255:2\" Get selected object(s) with given attribute index.");
            Console.WriteLine(" -C \t Security Level. (None, Authentication, Encrypted, AuthenticationEncryption)");
            Console.WriteLine(" -v \t Invocation counter data object Logical Name. Ex. 0.0.43.1.1.255");
            Console.WriteLine(" -I \t Auto increase invoke ID");
            Console.WriteLine(" -o \t Cache association view to make reading faster. Ex. -o C:\\device.xml");
            Console.WriteLine(" -T \t System title that is used with chiphering. Ex -D 4775727578313233");
            Console.WriteLine(" -A \t Authentication key that is used with chiphering. Ex -D D0D1D2D3D4D5D6D7D8D9DADBDCDDDEDF");
            Console.WriteLine(" -B \t Block cipher key that is used with chiphering. Ex -D 000102030405060708090A0B0C0D0E0F");
            Console.WriteLine(" -D \t Dedicated key that is used with chiphering. Ex -D 00112233445566778899AABBCCDDEEFF");
            Console.WriteLine(" -F \t Initial Frame Counter (Invocation counter) value.");
            Console.WriteLine(" -d \t Used DLMS standard. Ex -d India (DLMS, India, Italy, SaudiArabia, IDIS)");
            Console.WriteLine(" -x input XML file.");
            Console.WriteLine("Example:");
            Console.WriteLine("Read LG device using TCP/IP connection.");
            Console.WriteLine("Gurux.DLMS.XmlClient -r SN -c 16 -s 1 -h [Meter IP Address] -p [Meter Port No]");
            Console.WriteLine("Read LG device using serial port connection.");
            Console.WriteLine("Gurux.DLMS.XmlClient -r SN -c 16 -s 1 -sp COM1 -i");
            Console.WriteLine("Read Indian device using serial port connection.");
            Console.WriteLine("Gurux.DLMS.XmlClient -S COM1 -c 16 -s 1 -a Low -P [password]");
        }
    }
}
