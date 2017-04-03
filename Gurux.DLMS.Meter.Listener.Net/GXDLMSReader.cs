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
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;
using Gurux.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Gurux.DLMS.Meter.Listener.Net
{
    class GXDLMSReader
    {
        string path;
        int WaitTime = 5000;
        GXNet Media;
        bool Trace = true;
        GXDLMSClient Client;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="media"></param>
        public GXDLMSReader(GXNet media)
        {
            Media = media;
            Client = new GXDLMSClient(true, 0x10, 1, Enums.Authentication.None, null, Enums.InterfaceType.HDLC);
            path = "readout.xml";
        }

        /// <summary>
        /// Read all data from the meter.
        /// </summary>
        public void ReadAll()
        {
            try
            {
                InitializeConnection();
                GetAssociationView();
                GetScalersAndUnits();
                GetProfileGenericColumns();
                GetReadOut();
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Initialize connection to the meter.
        /// </summary>
        public void InitializeConnection()
        {
            GXReplyData reply = new GXReplyData();
            byte[] data;
            data = Client.SNRMRequest();
            if (data != null)
            {
                if (Trace)
                {
                    Console.WriteLine("Send SNRM request." + GXCommon.ToHex(data, true));
                }
                ReadDataBlock(data, reply);
                if (Trace)
                {
                    Console.WriteLine("Parsing UA reply." + reply.ToString());
                }
                //Has server accepted client.
                Client.ParseUAResponse(reply.Data);
                Console.WriteLine("Parsing UA reply succeeded.");
            }
            //Generate AARQ request.
            //Split requests to multiple packets if needed.
            //If password is used all data might not fit to one packet.
            foreach (byte[] it in Client.AARQRequest())
            {
                if (Trace)
                {
                    Console.WriteLine("Send AARQ request", GXCommon.ToHex(it, true));
                }
                reply.Clear();
                ReadDataBlock(it, reply);
            }
            if (Trace)
            {
                Console.WriteLine("Parsing AARE reply" + reply.ToString());
            }
            //Parse reply.
            Client.ParseAAREResponse(reply.Data);
            reply.Clear();
            //Get challenge Is HLS authentication is used.
            if (Client.IsAuthenticationRequired)
            {
                foreach (byte[] it in Client.GetApplicationAssociationRequest())
                {
                    reply.Clear();
                    ReadDataBlock(it, reply);
                }
                Client.ParseApplicationAssociationResponse(reply.Data);
            }
            Console.WriteLine("Parsing AARE reply succeeded.");
        }

        /// <summary>
        /// Read association view.
        /// </summary>
        public void GetAssociationView()
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.GetObjectsRequest(), reply);
            Client.ParseObjects(reply.Data, true);
        }

        /// <summary>
        /// Read scalers and units.
        /// </summary>
        public void GetScalersAndUnits()
        {
            GXDLMSObjectCollection objs = Client.Objects.GetObjects(new ObjectType[] { ObjectType.Register, ObjectType.ExtendedRegister, ObjectType.DemandRegister });
            Console.WriteLine("Read scalers and units from the device.");
            if ((Client.NegotiatedConformance & Conformance.MultipleReferences) != 0)
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
                ReadList(list);
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
                            Read(it, 3);
                        }
                        if (it is GXDLMSDemandRegister)
                        {
                            Console.WriteLine(it.Name);
                            Read(it, 4);
                        }
                    }
                    catch
                    {
                        //Actaric SL7000 can return error here. Continue reading.
                    }
                }
            }
        }

        /// <summary>
        /// Read profile generic columns.
        /// </summary>
        public void GetProfileGenericColumns()
        {
            //Read Profile Generic columns first.
            foreach (GXDLMSObject it in Client.Objects.GetObjects(ObjectType.ProfileGeneric))
            {
                try
                {
                    Console.WriteLine(it.Name);
                    Read(it, 3);
                    GXDLMSObject[] cols = (it as GXDLMSProfileGeneric).GetCaptureObject();
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
                }
                catch (Exception ex)
                {
                    //Continue reading.
                }
            }
            try
            {
                List<Type> extraTypes = new List<Type>(Gurux.DLMS.GXDLMSClient.GetObjectTypes());
                extraTypes.Add(typeof(GXDLMSAttributeSettings));
                extraTypes.Add(typeof(GXDLMSAttribute));
                XmlSerializer x = new XmlSerializer(typeof(GXDLMSObjectCollection), extraTypes.ToArray());
                using (Stream stream = File.Open(path, FileMode.Create))
                {
                    TextWriter writer = new StreamWriter(stream);
                    x.Serialize(writer, Client.Objects);
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
        }

        /// <summary>
        /// Read all objects from the meter.
        /// </summary>
        /// <remarks>
        /// It's not normal to read all data from the meter. This is just an example.
        /// </remarks>
        public void GetReadOut()
        {
            foreach (GXDLMSObject it in Client.Objects)
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
                Console.WriteLine("-------- Reading " + it.GetType().Name + " " + it.Name + " " + it.Description);
                foreach (int pos in (it as IGXDLMSBase).GetAttributeIndexToRead())
                {
                    try
                    {
                        object val = Read(it, pos);
                        //If data is array.
                        if (val is byte[])
                        {
                            val = GXCommon.ToHex((byte[])val, true);
                        }
                        else if (val is Array)
                        {
                            string str = "";
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
                            string str = "[";
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
                        Console.WriteLine("Index: " + pos + " Value: " + val);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error! Index: " + pos + " " + ex.Message);
                    }
                }
            }
            //Find profile generics and read them.
            foreach (GXDLMSObject it in Client.Objects.GetObjects(ObjectType.ProfileGeneric))
            {
                Console.WriteLine("-------- Reading " + it.GetType().Name + " " + it.Name + " " + it.Description);
                long entriesInUse = Convert.ToInt64(Read(it, 7));
                long entries = Convert.ToInt64(Read(it, 8));
                Console.WriteLine("Entries: " + entriesInUse + "/" + entries);
                //If there are no columns or rows.
                if (entriesInUse == 0 || (it as GXDLMSProfileGeneric).CaptureObjects.Count == 0)
                {
                    continue;
                }
                //All meters are not supporting parameterized read.
                if ((Client.NegotiatedConformance & (Conformance.ParameterizedAccess | Conformance.SelectiveAccess)) != 0)
                {
                    try
                    {
                        //Read first row from Profile Generic.
                        object[] rows = ReadRowsByEntry(it as GXDLMSProfileGeneric, 1, 1);
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
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error! Failed to read first row: " + ex.Message);
                        //Continue reading.
                    }
                }
                //All meters are not supporting parameterized read.
                if ((Client.NegotiatedConformance & (Conformance.ParameterizedAccess | Conformance.SelectiveAccess)) != 0)
                {
                    try
                    {
                        //Read last day from Profile Generic.
                        object[] rows = ReadRowsByRange(it as GXDLMSProfileGeneric, DateTime.Now.Date, DateTime.MaxValue);
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
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error! Failed to read last day: " + ex.Message);
                        //Continue reading.
                    }
                }
            }
        }

        /// <summary>
        /// Read DLMS Data from the device.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <returns>Received data.</returns>
        public void ReadDLMSPacket(byte[] data, GXReplyData reply)
        {
            if (data == null)
            {
                return;
            }
            reply.Error = 0;
            object eop = (byte)0x7E;
            //In network connection terminator is not used.
            if (Client.InterfaceType == InterfaceType.WRAPPER && Media is GXNet)
            {
                eop = null;
            }
            int pos = 0;
            bool succeeded = false;
            ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
            {
                Eop = eop,
                Count = 5,
                WaitTime = WaitTime,
            };
            lock (Media.Synchronous)
            {
                while (!succeeded && pos != 3)
                {
                    WriteTrace("<- " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(data, true));
                    Media.Send(data, null);
                    succeeded = Media.Receive(p);
                    if (!succeeded)
                    {
                        //If Eop is not set read one byte at time.
                        if (p.Eop == null)
                        {
                            p.Count = 1;
                        }
                        //Try to read again...
                        if (++pos != 3)
                        {
                            System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                            continue;
                        }
                        throw new Exception("Failed to receive reply from the device in given time.");
                    }
                }
                try
                {
                    //Loop until whole COSEM packet is received.
                    while (!Client.GetData(p.Reply, reply))
                    {
                        //If Eop is not set read one byte at time.
                        if (p.Eop == null)
                        {
                            p.Count = 1;
                        }
                        while (!Media.Receive(p))
                        {
                            //If echo.
                            if (p.Reply.Length == data.Length)
                            {
                                Media.Send(data, null);
                            }
                            //Try to read again...
                            if (++pos != 3)
                            {
                                System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                                continue;
                            }
                            throw new Exception("Failed to receive reply from the device in given time.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteTrace("-> " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(p.Reply, true));
                    throw ex;
                }
            }
            WriteTrace("-> " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(p.Reply, true));
            if (reply.Error != 0)
            {
                if (reply.Error == (short)ErrorCode.Rejected)
                {
                    Thread.Sleep(1000);
                    ReadDLMSPacket(data, reply);
                }
                else
                {
                    throw new GXDLMSException(reply.Error);
                }
            }
        }

        /// <summary>
        /// Send data block(s) to the meter.
        /// </summary>
        /// <param name="data">Send data block(s).</param>
        /// <param name="reply">Received reply from the meter.</param>
        /// <returns>Return false if frame is rejected.</returns>
        public bool ReadDataBlock(byte[][] data, GXReplyData reply)
        {
            foreach (byte[] it in data)
            {
                reply.Clear();
                ReadDataBlock(it, reply);
            }
            return reply.Error == 0;
        }

        /// <summary>
        /// Read data block from the device.
        /// </summary>
        /// <param name="data">data to send</param>
        /// <param name="text">Progress text.</param>
        /// <param name="multiplier"></param>
        /// <returns>Received data.</returns>
        public void ReadDataBlock(byte[] data, GXReplyData reply)
        {
            ReadDLMSPacket(data, reply);
            while (reply.IsMoreData)
            {
                data = Client.ReceiverReady(reply.MoreData);
                ReadDLMSPacket(data, reply);
                if (!Trace)
                {
                    //If data block is read.
                    if ((reply.MoreData & RequestTypes.Frame) == 0)
                    {
                        Console.Write("+");
                    }
                    else
                    {
                        Console.Write("-");
                    }
                }
            }
        }

        /// <summary>
        /// Read attribute value.
        /// </summary>
        /// <param name="it">COSEM object to read.</param>
        /// <param name="attributeIndex">Attribute index.</param>
        /// <returns>Read value.</returns>
        public object Read(GXDLMSObject it, int attributeIndex)
        {
            GXReplyData reply = new GXReplyData();
            if (!ReadDataBlock(Client.Read(it, attributeIndex), reply))
            {
                if (reply.Error != (short)ErrorCode.Rejected)
                {
                    throw new GXDLMSException(reply.Error);
                }
                reply.Clear();
                Thread.Sleep(1000);
                if (!ReadDataBlock(Client.Read(it, attributeIndex), reply))
                {
                    throw new GXDLMSException(reply.Error);
                }
            }
            //Update data type.
            if (it.GetDataType(attributeIndex) == DataType.None)
            {
                it.SetDataType(attributeIndex, reply.DataType);
            }
            return Client.UpdateValue(it, attributeIndex, reply.Value);
        }


        /// <summary>
        /// Read list of attributes.
        /// </summary>
        public void ReadList(List<KeyValuePair<GXDLMSObject, int>> list)
        {
            byte[][] data = Client.ReadList(list);
            GXReplyData reply = new GXReplyData();
            List<object> values = new List<object>();
            foreach (byte[] it in data)
            {
                ReadDataBlock(it, reply);
                if (reply.Value is object[])
                {
                    values.AddRange((object[])reply.Value);
                }
                else if (reply.Value != null)
                {
                    //Value is null if data is send multiple frames.
                    values.Add(reply.Value);
                }
                reply.Clear();
            }
            if (values.Count != list.Count)
            {
                throw new Exception("Invalid reply. Read items count do not match.");
            }
            Client.UpdateValues(list, values);
        }

        /// <summary>
        /// Write attribute value.
        /// </summary>
        public void Write(GXDLMSObject it, int attributeIndex)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.Write(it, attributeIndex), reply);
        }

        /// <summary>
        /// Method attribute value.
        /// </summary>
        public void Method(GXDLMSObject it, int attributeIndex, object value, DataType type)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.Method(it, attributeIndex, value, type), reply);
        }

        /// <summary>
        /// Read Profile Generic Columns by entry.
        /// </summary>
        public object[] ReadRowsByEntry(GXDLMSProfileGeneric it, int index, int count)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.ReadRowsByEntry(it, index, count), reply);
            return (object[])Client.UpdateValue(it, 2, reply.Value);
        }

        /// <summary>
        /// Read Profile Generic Columns by range.
        /// </summary>
        public object[] ReadRowsByRange(GXDLMSProfileGeneric it, DateTime start, DateTime end)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.ReadRowsByRange(it, start, end), reply);
            return (object[])Client.UpdateValue(it, 2, reply.Value);
        }

        /// <summary>
        /// Close connection to the meter.
        /// </summary>
        public void Close()
        {
            if (Media != null && Client != null)
            {
                try
                {
                    Console.WriteLine("Disconnecting from the meter.");
                    GXReplyData reply = new GXReplyData();
                    ReadDLMSPacket(Client.DisconnectRequest(), reply);
                    Media.Close();
                }
                catch
                {

                }
                Media = null;
                Client = null;
            }
        }

        /// <summary>
        /// Write trace.
        /// </summary>
        /// <param name="line"></param>
        void WriteTrace(string line)
        {
            if (Trace)
            {
                Console.WriteLine(line);
            }
            using (TextWriter writer = new StreamWriter(File.Open("trace.txt", FileMode.Append)))
            {
                writer.WriteLine(line);
            }
        }
    }
}
