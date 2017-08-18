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

namespace Gurux.DLMS.Client.Example
{
    class Program
    {
        static void ShowHelp()
        {
            Console.WriteLine("GuruxDlmsSample reads data from the DLMS/COSEM device.");
            Console.WriteLine("GuruxDlmsSample /h=[Meter IP Address] /p=[Meter Port No] [/s=] /c=16 /s=1 /r=SN");
            Console.WriteLine(" /h=\t host name or IP address.");
            Console.WriteLine(" /p=\t port number or name (Example: 1000).");
            Console.WriteLine(" /sp=\t serial port.");
            Console.WriteLine(" /IEC use IEC as start protocol.");
            Console.WriteLine(" /a=\t Authentication (None, Low, High).");
            Console.WriteLine(" /pw=\t Password for authentication.");
            Console.WriteLine(" /c=\t Client address. (Default: 16)");
            Console.WriteLine(" /s=\t Server address. (Default: 1)");
            Console.WriteLine(" /sn=\t Server address as serial number.");
            Console.WriteLine(" /r=[SN, LN]\t Short name or Logican Name (default) referencing is used.");
            Console.WriteLine(" /WRAPPER profile is used. HDLC is default.");
            Console.WriteLine(" /\t Trace messages.");
            Console.WriteLine(" /g=\"0.0.1.0.0.255:1; 0.0.1.0.0.255:2\" Get selected object(s) with given attribute index.");
            Console.WriteLine("Example:");
            Console.WriteLine("Read LG device using TCP/IP connection.");
            Console.WriteLine("GuruxDlmsSample /r=SN /c=16 /s=1 /h=[Meter IP Address] /p=[Meter Port No]");
            Console.WriteLine("Read LG device using serial port connection.");
            Console.WriteLine("GuruxDlmsSample /r=SN /c=16 /s=1 /sp=COM1 /IEC");
            Console.WriteLine("Read Indian device using serial port connection.");
            Console.WriteLine("GuruxDlmsSample /sp=COM1 /c=16 /s=1 /a=Low /pw=[password]");
        }

        static void Trace(TextWriter writer, string text)
        {
            writer.Write(text);
            Console.Write(text);
        }

        static void TraceLine(TextWriter writer, string text)
        {
            writer.WriteLine(text);
            Console.WriteLine(text);
        }

        static int Main(string[] args)
        {
            IGXMedia media = null;
            GXCommunicatation comm = null;
            //Objects to read.
            List<KeyValuePair<string, int>> readObjects = new List<KeyValuePair<string, int>>();
            try
            {
                bool iec = false, trace = false;
                string str;
                TextWriter logFile = new StreamWriter(File.Open("LogFile.txt", FileMode.Create));
                ////////////////////////////////////////
                //Handle command line parameters.
                GXDLMSSecureClient dlms = new GXDLMSSecureClient();
                foreach (string it in args)
                {
                    String item = it.Trim().ToLower();
                    if (item.StartsWith("/sn="))//Serial number.
                    {
                        dlms.ServerAddress = GXDLMSClient.GetServerAddress(int.Parse(item.Replace("/sn=", "")));
                    }
                    else if (string.Compare(item, "/wrapper", true) == 0)//Wrapper is used.
                    {
                        dlms.InterfaceType = InterfaceType.WRAPPER;
                    }
                    else if (item.StartsWith("/r="))//referencing
                    {
                        str = item.Replace("/r=", "");
                        if (string.Compare(str, "sn", true) == 0)
                        {
                            dlms.UseLogicalNameReferencing = false;
                        }
                        else if (string.Compare(str, "ln", true) == 0)
                        {
                            dlms.UseLogicalNameReferencing = true;
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("Invalid reference. Set LN or SN.");
                        }
                    }
                    else if (item.StartsWith("/c="))//Client address
                    {
                        dlms.ClientAddress = int.Parse(item.Replace("/c=", ""));
                    }
                    else if (item.StartsWith("/s="))//Server address
                    {
                        dlms.ServerAddress = int.Parse(item.Replace("/c=", ""));
                    }
                    else if (item.StartsWith("/h=")) //Host
                    {
                        if (media == null)
                        {
                            media = new Gurux.Net.GXNet();
                        }
                        Gurux.Net.GXNet net = media as GXNet;
                        net.HostName = item.Replace("/h=", "");
                    }
                    else if (item.StartsWith("/p="))// TCP/IP Port
                    {
                        if (media == null)
                        {
                            media = new Gurux.Net.GXNet();
                        }
                        Gurux.Net.GXNet net = media as GXNet;
                        net.Port = int.Parse(item.Replace("/p=", ""));
                    }
                    else if (item.StartsWith("/sp="))//Serial Port
                    {
                        media = new GXSerial();
                        GXSerial serial = media as GXSerial;
                        serial.PortName = item.Replace("/sp=", "");
                    }
                    else if (item.StartsWith("/t"))//Are messages traced.
                    {
                        trace = true;
                    }
                    else if (item.StartsWith("/iec"))//IEC is start protocol.
                    {
                        iec = true;
                    }
                    else if (item.StartsWith("/a="))//Authentication
                    {
                        dlms.Authentication = (Authentication)Enum.Parse(typeof(Authentication), it.Trim().Replace("/a=", ""));
                    }
                    else if (item.StartsWith("/pw="))//Password
                    {
                        dlms.Password = ASCIIEncoding.ASCII.GetBytes(it.Trim().Replace("/pw=", ""));
                    }
                    else if (item.StartsWith("/g="))//Get objects
                    {
                        foreach (string o in item.Replace("/g=", "").Split(new char[] { ';', ',' }))
                        {
                            string[] tmp = o.Split(new char[] { ':' });
                            if (tmp.Length != 2)
                            {
                                throw new ArgumentOutOfRangeException("Invalid Logical name or attribute index.");
                            }
                            readObjects.Add(new KeyValuePair<string, int>(tmp[0].Trim(), int.Parse(tmp[1].Trim())));
                        }
                    }
                    else
                    {
                        ShowHelp();
                        return 1;
                    }
                }
                if (media == null)
                {
                    ShowHelp();
                    return 1;
                }
                ////////////////////////////////////////
                //Initialize connection settings.
                if (media is GXSerial)
                {
                    GXSerial serial = media as GXSerial;
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
                else if (media is GXNet)
                {
                }
                else
                {
                    throw new Exception("Unknown media type.");
                }
                ////////////////////////////////////////
                //Update manufacturer depended settings.
                comm = new GXCommunicatation(dlms, media, iec);
                comm.Trace = trace;
                comm.InitializeConnection();
                GXDLMSObjectCollection objects = null;
                string path = media.ToString() + ".xml";

                List<Type> extraTypes = new List<Type>(Gurux.DLMS.GXDLMSClient.GetObjectTypes());
                extraTypes.Add(typeof(GXDLMSAttributeSettings));
                extraTypes.Add(typeof(GXDLMSAttribute));
                XmlSerializer x = new XmlSerializer(typeof(GXDLMSObjectCollection), extraTypes.ToArray());
                //You can save association view, but make sure that it is not change.
                //Save Association view to the cache so it is not needed to retrieve every time.
                /*
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
                 */
                {
                    Console.WriteLine("Get available objects from the device.");
                    objects = comm.GetAssociationView();
                    if (readObjects.Count != 0)
                    {
                        Console.WriteLine("");
                        foreach (KeyValuePair<string, int> it in readObjects)
                        {
                            object val = comm.Read(objects.FindByLN(ObjectType.None, it.Key), it.Value);
                            ShowValue(logFile, it.Value, val);
                        }
                        return 0;
                    }
                    GXDLMSObjectCollection objs = objects.GetObjects(new ObjectType[] { ObjectType.Register, ObjectType.ExtendedRegister, ObjectType.DemandRegister });
                    Console.WriteLine("Read scalers and units from the device.");
                    if ((dlms.NegotiatedConformance & Conformance.MultipleReferences) != 0)
                    {
                        List<KeyValuePair<GXDLMSObject, int>> list = new List<KeyValuePair<GXDLMSObject, int>>();
                        foreach (GXDLMSObject it in objs)
                        {
                            if (it is GXDLMSRegister)
                            {
                                list.Add(new KeyValuePair<GXDLMSObject, int>(it, 3));
                            }
                            if (it is GXDLMSDemandRegister)
                            {
                                list.Add(new KeyValuePair<GXDLMSObject, int>(it, 4));
                            }
                        }
                        comm.ReadList(list);
                    }
                    else
                    {
                        //Read values one by one.
                        foreach (GXDLMSObject it in objs)
                        {
                            try
                            {
                                if (it is GXDLMSRegister)
                                {
                                    Console.WriteLine(it.Name);
                                    comm.Read(it, 3);
                                }
                                if (it is GXDLMSDemandRegister)
                                {
                                    Console.WriteLine(it.Name);
                                    comm.Read(it, 4);
                                }
                            }
                            catch
                            {
                                //Actaric SL7000 can return error here. Continue reading.
                            }
                        }
                    }
                    //Read Profile Generic columns first.
                    foreach (GXDLMSObject it in objects.GetObjects(ObjectType.ProfileGeneric))
                    {
                        try
                        {
                            Console.WriteLine(it.Name);
                            comm.Read(it, 3);
                            GXDLMSObject[] cols = (it as GXDLMSProfileGeneric).GetCaptureObject();
                            TraceLine(logFile, "Profile Generic " + it.Name + " Columns:");
                            StringBuilder sb = new StringBuilder();
                            bool First = true;
                            foreach (GXDLMSObject col in cols)
                            {
                                if (!First)
                                {
                                    sb.Append(" | ");
                                }
                                First = false;
                                sb.Append(col.Name);
                                sb.Append(" ");
                                sb.Append(col.Description);
                            }
                            TraceLine(logFile, sb.ToString());
                        }
                        catch (Exception ex)
                        {
                            TraceLine(logFile, "Err! Failed to read columns:" + ex.Message);
                            //Continue reading.
                        }
                    }
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
                foreach (GXDLMSObject it in objects)
                {
                    // Profile generics are read later because they are special cases.
                    // (There might be so lots of data and we so not want waste time to read all the data.)
                    if (it is GXDLMSProfileGeneric)
                    {
                        continue;
                    }
                    if (!(it is IGXDLMSBase))
                    {
                        //If interface is not implemented.
                        //Example manufacturer spesific interface.
                        Console.WriteLine("Unknown Interface: " + it.ObjectType.ToString());
                        continue;
                    }
                    TraceLine(logFile, "-------- Reading " + it.GetType().Name + " " + it.Name + " " + it.Description);
                    foreach (int pos in (it as IGXDLMSBase).GetAttributeIndexToRead())
                    {
                        try
                        {
                            object val = comm.Read(it, pos);
                            ShowValue(logFile, pos, val);
                        }
                        catch (Exception ex)
                        {
                            TraceLine(logFile, "Error! Index: " + pos + " " + ex.Message);
                        }
                    }
                }
                //Find profile generics and read them.
                foreach (GXDLMSObject it in objects.GetObjects(ObjectType.ProfileGeneric))
                {
                    TraceLine(logFile, "-------- Reading " + it.GetType().Name + " " + it.Name + " " + it.Description);
                    long entriesInUse = Convert.ToInt64(comm.Read(it, 7));
                    long entries = Convert.ToInt64(comm.Read(it, 8));
                    TraceLine(logFile, "Entries: " + entriesInUse + "/" + entries);
                    //If there are no columns or rows.
                    if (entriesInUse == 0 || (it as GXDLMSProfileGeneric).CaptureObjects.Count == 0)
                    {
                        continue;
                    }
                    //All meters are not supporting parameterized read.
                    if ((dlms.NegotiatedConformance & (Conformance.ParameterizedAccess | Conformance.SelectiveAccess)) != 0)
                    {
                        try
                        {
                            //Read first row from Profile Generic.
                            object[] rows = comm.ReadRowsByEntry(it as GXDLMSProfileGeneric, 1, 1);
                            StringBuilder sb = new StringBuilder();
                            foreach (object[] row in rows)
                            {
                                foreach (object cell in row)
                                {
                                    if (cell is byte[])
                                    {
                                        sb.Append(GXCommon.ToHex((byte[])cell, true));
                                    }
                                    else
                                    {
                                        sb.Append(Convert.ToString(cell));
                                    }
                                    sb.Append(" | ");
                                }
                                sb.Append("\r\n");
                            }
                            Trace(logFile, sb.ToString());
                        }
                        catch (Exception ex)
                        {
                            TraceLine(logFile, "Error! Failed to read first row: " + ex.Message);
                            //Continue reading.
                        }
                    }
                    //All meters are not supporting parameterized read.
                    if ((dlms.NegotiatedConformance & (Conformance.ParameterizedAccess | Conformance.SelectiveAccess)) != 0)
                    {
                        try
                        {
                            //Read last day from Profile Generic.
                            object[] rows = comm.ReadRowsByRange(it as GXDLMSProfileGeneric, DateTime.Now.Date, DateTime.MaxValue);
                            StringBuilder sb = new StringBuilder();
                            foreach (object[] row in rows)
                            {
                                foreach (object cell in row)
                                {
                                    if (cell is byte[])
                                    {
                                        sb.Append(GXCommon.ToHex((byte[])cell, true));
                                    }
                                    else
                                    {
                                        sb.Append(Convert.ToString(cell));
                                    }
                                    sb.Append(" | ");
                                }
                                sb.Append("\r\n");
                            }
                            Trace(logFile, sb.ToString());
                        }
                        catch (Exception ex)
                        {
                            TraceLine(logFile, "Error! Failed to read last day: " + ex.Message);
                            //Continue reading.
                        }
                    }
                }
                logFile.Flush();
                logFile.Close();
            }
            catch (Exception ex)
            {
                if (comm != null)
                {
                    comm.Close();
                }
                Console.WriteLine(ex.Message);
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.ReadKey();
                }
                return 1;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Close();
                }
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.WriteLine("Ended. Press any key to continue.");
                    Console.ReadKey();
                }
            }
            return 0;
        }

        static void ShowValue(TextWriter logFile, int pos, object val)
        {
            string str;
            //If data is array.
            if (val is byte[])
            {
                val = GXCommon.ToHex((byte[])val, true);
            }
            else if (val is Array)
            {
                str = "";
                for (int pos2 = 0; pos2 != (val as Array).Length; ++pos2)
                {
                    if (str != "")
                    {
                        str += ", ";
                    }
                    if ((val as Array).GetValue(pos2) is byte[])
                    {
                        str += GXCommon.ToHex((byte[])(val as Array).GetValue(pos2), true);
                    }
                    else
                    {
                        str += (val as Array).GetValue(pos2).ToString();
                    }
                }
                val = str;
            }
            else if (val is System.Collections.IList)
            {
                str = "[";
                bool empty = true;
                foreach (object it2 in val as System.Collections.IList)
                {
                    if (!empty)
                    {
                        str += ", ";
                    }
                    empty = false;
                    if (it2 is byte[])
                    {
                        str += GXCommon.ToHex((byte[])it2, true);
                    }
                    else
                    {
                        str += it2.ToString();
                    }
                }
                str += "]";
                val = str;
            }
            TraceLine(logFile, "Index: " + pos + " Value: " + val);
        }
    }
}
