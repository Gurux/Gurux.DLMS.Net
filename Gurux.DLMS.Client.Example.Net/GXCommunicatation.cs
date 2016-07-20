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
using Gurux.DLMS;
using Gurux.Net;
using Gurux.Serial;
using System.IO.Ports;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;
using System.IO;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using System.Threading;
using Gurux.DLMS.Secure;

namespace Gurux.DLMS.Client.Example
{
    class GXCommunicatation
    {
        public bool Trace = false;
        public GXDLMSSecureClient Client;
        int WaitTime = 5000;
        IGXMedia Media;
        bool InitializeIEC;
        GXManufacturer Manufacturer;
        HDLCAddressType HDLCAddressing;        

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXCommunicatation(GXDLMSSecureClient dlms, IGXMedia media, bool initializeIEC, Authentication authentication, string password)
        {
            Client = dlms;
            Media = media;
            InitializeIEC = initializeIEC;
            Client.Authentication = authentication;
            Client.Password = ASCIIEncoding.ASCII.GetBytes(password);            
            //Delete trace file if exists.
            if (File.Exists("trace.txt"))
            {
                File.Delete("trace.txt");
            }
        }

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
        /// Destructor.
        /// </summary>
        /// <remarks>
        /// Close connection to the media and send disconnect message.
        /// </remarks>
        ~GXCommunicatation()
        {
            Close();
        }

        public void UpdateManufactureSettings(string id)
        {
            if (Manufacturer != null && string.Compare(Manufacturer.Identification, id, true) != 0)
            {
                throw new Exception(string.Format("Manufacturer type does not match. Manufacturer is {0} and it should be {1}.", id, Manufacturer.Identification));
            }
            if (this.Media is GXNet && Manufacturer.UseIEC47)
            {
                Client.InterfaceType = InterfaceType.WRAPPER;
            }
            else
            {
                Client.InterfaceType = InterfaceType.HDLC;
            }
            Client.UseLogicalNameReferencing = Manufacturer.UseLogicalNameReferencing;
            //If network media is used check is manufacturer supporting IEC 62056-47
            GXServerAddress server = Manufacturer.GetServer(HDLCAddressing);
           //Mikko Client.ClientAddress = Manufacturer.GetAuthentication(Client.Authentication).ClientAddress;
            if (Client.InterfaceType == InterfaceType.WRAPPER)
            {
                if (HDLCAddressing == HDLCAddressType.SerialNumber)
                {
                    Client.ServerAddress = GXDLMSClient.GetServerAddress(server.PhysicalAddress, server.Formula);
                }
                else
                {
                    Client.ServerAddress = server.PhysicalAddress;
                }
                Client.ServerAddress = Client.ClientAddress = 1; 
            }
            else
            {
                if (HDLCAddressing == HDLCAddressType.SerialNumber)
                {
                    Client.ServerAddress = GXDLMSClient.GetServerAddress(server.PhysicalAddress, server.Formula);
                }
                else
                {
                    Client.ServerAddress = GXDLMSClient.GetServerAddress(server.LogicalAddress, server.PhysicalAddress);
                }
            }
        }

        void InitSerial()
        {
            GXSerial serial = Media as GXSerial;
            byte Terminator = (byte)0x0A;
            if (serial != null && InitializeIEC)
            {
                serial.BaudRate = 300;
                serial.DataBits = 7;
                serial.Parity = Parity.Even;
                serial.StopBits = StopBits.One;
            }
            Media.Open();
            //Query device information.
            if (Media != null && InitializeIEC)
            {
                string data = "/?!\r\n";
                if (Trace)
                {
                    Console.WriteLine("IEC sending:" + data);
                }
                ReceiveParameters<string> p = new ReceiveParameters<string>()
                {
                    Eop = Terminator,
                    WaitTime = WaitTime
                };
                lock (Media.Synchronous)
                {
                    WriteTrace("<- " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(data), true));
                    Media.Send(data, null);
                    if (!Media.Receive(p))
                    {
                        //Try to move away from mode E.
                        try
                        {
                            GXReplyData reply = new GXReplyData();
                            ReadDLMSPacket(Client.DisconnectRequest(), reply);
                        }
                        catch (Exception)
                        {
                        }
                        data = (char)0x01 + "B0" + (char)0x03;
                        Media.Send(data, null);
                        p.Count = 1;
                        if (!Media.Receive(p))
                        {
                        }
                        data = "Failed to receive reply from the device in given time.";
                        Console.WriteLine(data);
                        throw new Exception(data);
                    }
                    WriteTrace("-> " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(p.Reply), true));
                    //If echo is used.
                    if (p.Reply == data)
                    {
                        p.Reply = null;
                        if (!Media.Receive(p))
                        {
                            //Try to move away from mode E.
                            GXReplyData reply = new GXReplyData();
                            ReadDLMSPacket(Client.DisconnectRequest(), reply);
                            if (serial != null)
                            {
                                data = (char)0x01 + "B0" + (char)0x03;
                                Media.Send(data, null);
                                p.Count = 1;
                                Media.Receive(p);
                                serial.BaudRate = 9600;
                                data = (char)0x01 + "B0" + (char)0x03 + "\r\n";
                                Media.Send(data, null);
                                p.Count = 1;
                                Media.Receive(p);
                            }

                            data = "Failed to receive reply from the device in given time.";
                            Console.WriteLine(data);
                            throw new Exception(data);
                        }
                        WriteTrace("-> " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(p.Reply), true));
                    }
                }
                Console.WriteLine("IEC received: " + p.Reply);
                if (p.Reply[0] != '/')
                {
                    p.WaitTime = 100;
                    Media.Receive(p);
                    throw new Exception("Invalid responce.");
                }
                string manufactureID = p.Reply.Substring(1, 3);
                UpdateManufactureSettings(manufactureID);
                char baudrate = p.Reply[4];
                int BaudRate = 0;
                switch (baudrate)
                {
                    case '0':
                        BaudRate = 300;
                        break;
                    case '1':
                        BaudRate = 600;
                        break;
                    case '2':
                        BaudRate = 1200;
                        break;
                    case '3':
                        BaudRate = 2400;
                        break;
                    case '4':
                        BaudRate = 4800;
                        break;
                    case '5':
                        BaudRate = 9600;
                        break;
                    case '6':
                        BaudRate = 19200;
                        break;
                    default:
                        throw new Exception("Unknown baud rate.");
                }
                Console.WriteLine("BaudRate is : " + BaudRate.ToString());
                //Send ACK
                //Send Protocol control character
                byte controlCharacter = (byte)'2';// "2" HDLC protocol procedure (Mode E)
                //Send Baudrate character
                //Mode control character 
                byte ModeControlCharacter = (byte)'2';//"2" //(HDLC protocol procedure) (Binary mode)
                //Set mode E.
                byte[] arr = new byte[] { 0x06, controlCharacter, (byte)baudrate, ModeControlCharacter, 13, 10 };
                Console.WriteLine("Moving to mode E.", arr);
                lock (Media.Synchronous)
                {
                    WriteTrace("<- " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(arr, true));
                    Media.Send(arr, null);
                    p.Reply = null;
                    if (!Media.Receive(p))
                    {
                        //Try to move away from mode E.
                        GXReplyData reply = new GXReplyData();
                        ReadDLMSPacket(Client.DisconnectRequest(), reply);
                        data = "Failed to receive reply from the device in given time.";
                        Console.WriteLine(data);
                        throw new Exception(data);
                    }
                    WriteTrace("-> " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(p.Reply), true));
                    Console.WriteLine("Received: " + p.Reply);
                    if (serial != null)
                    {
                        System.Threading.Thread.Sleep(400);
                        serial.Close();
                        serial.BaudRate = BaudRate;
                        serial.DataBits = 8;
                        serial.Parity = Parity.None;
                        serial.StopBits = StopBits.One;
                        System.Threading.Thread.Sleep(400);
                        serial.Open();
                        serial.ResetSynchronousBuffer();                        
                    }
                }
            }
        }

        void InitNet()
        {
            Media.Open();
        }

        public void InitializeConnection(GXManufacturer man)
        {
            Manufacturer = man;
            UpdateManufactureSettings(man.Identification);
            if (Media is GXSerial)
            {
                Console.WriteLine("Initializing serial connection.");
                InitSerial();                
            }
            else if (Media is GXNet)
            {
                Console.WriteLine("Initializing Network connection.");
                InitNet();
                //Some Electricity meters need some time before first message can be send.
                System.Threading.Thread.Sleep(500);
            }
            else
            {                
                throw new Exception("Unknown media type.");
            }
            GXReplyData reply = new GXReplyData();
            byte[] data;
            data = Client.SNRMRequest();
            if (data != null)
            {
                if (Trace)
                {
                    Console.WriteLine("Send SNRM request." + GXCommon.ToHex(data, true));
                }
                ReadDLMSPacket(data, reply);
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
                ReadDLMSPacket(it, reply);
            }
            if (Trace)
            {
                Console.WriteLine("Parsing AARE reply" + reply.ToString());
            }
            //Parse reply.
            Client.ParseAAREResponse(reply.Data);
            reply.Clear();
            //Get challenge Is HSL authentication is used.
            if (Client.IsAuthenticationRequired)
            {
                foreach (byte[] it in Client.GetApplicationAssociationRequest())
                {
                    reply.Clear();
                    ReadDLMSPacket(it, reply);
                }
                Client.ParseApplicationAssociationResponse(reply.Data);
            }
            Console.WriteLine("Parsing AARE reply succeeded.");
        }

        /// <summary>
        /// Read attribute value.
        /// </summary>
        public object Read(GXDLMSObject it, int attributeIndex)
        {
            GXReplyData reply = new GXReplyData();
            if (!ReadDataBlock(Client.Read(it, attributeIndex), reply))
            {
                reply.Clear();
                Thread.Sleep(1000);
                if (!ReadDataBlock(Client.Read(it, attributeIndex), reply))
                {
                    if (reply.Error != 0)
                    {
                        throw new GXDLMSException(reply.Error);
                    }
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
            GXByteBuffer bb = new GXByteBuffer();
            int cnt = 0;
            byte[][] data = Client.ReadList(list);
            GXReplyData reply = new GXReplyData();
            foreach (byte[] it in data)
            {
                reply.Clear();
                ReadDataBlock(it, reply);
                if (reply.IsComplete)
                {
                    cnt += GXDLMSBuilder.GetObjectCount(reply.Data);
                    bb.Set(reply.Data);
                }
            }
            if (cnt != list.Count)
            {
                throw new Exception("Invalid reply. Read items count do not match.");
            }
            Client.UpdateValues(list, bb);
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
            return (object[]) Client.UpdateValue(it, 2, reply.Value);
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
        /// Read Association View from the meter.
        /// </summary>
        public GXDLMSObjectCollection GetAssociationView()
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.GetObjectsRequest(), reply);
            return Client.ParseObjects(reply.Data, true);
        }

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
                        while(!Media.Receive(p))
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

        public void UpdateImage(GXDLMSImageTransfer target, byte[] data, string Identification)
        {
            GXReplyData reply = new GXReplyData();
            //Check that image transfer ia enabled.
            ReadDataBlock(Client.Read(target, 5), reply);
            Client.UpdateValue(target, 5, reply);
            if (!target.ImageTransferEnabled)
            {
                throw new Exception("Image transfer is not enabled");
            }

            //Step 1: The client gets the ImageBlockSize.
            reply.Clear();
            ReadDataBlock(Client.Read(target, 2), reply);
            Client.UpdateValue(target, 2, reply);

            // Step 2: The client initiates the Image transfer process.
            reply.Clear();
            ReadDataBlock(target.ImageTransferInitiate(Client, Identification, data.Length), reply);           
            // Step 3: The client transfers ImageBlocks.
            reply.Clear();
            int ImageBlockCount;
            ReadDataBlock(target.ImageBlockTransfer(Client, data, out ImageBlockCount), reply);
            //Step 4: The client checks the completeness of the Image in 
            //each server individually and transfers any ImageBlocks not (yet) transferred;
            reply.Clear();
            Client.UpdateValue(target, 2, reply);

            // Step 5: The Image is verified;
            reply.Clear();
            ReadDataBlock(target.ImageVerify(Client), reply);
            // Step 6: Before activation, the Image is checked;

            //Get list to imaages to activate.
            reply.Clear(); 
            ReadDataBlock(Client.Read(target, 7), reply);
            Client.UpdateValue(target, 7, reply);
            bool bFound = false;
            foreach (GXDLMSImageActivateInfo it in target.ImageActivateInfo)
            {
                if (it.Identification == Identification)
                {
                    bFound = true;
                    break;
                }
            }

            //Read image transfer status.
            reply.Clear(); 
            ReadDataBlock(Client.Read(target, 6), reply);
            Client.UpdateValue(target, 6, reply.Value);
            if (target.ImageTransferStatus != ImageTransferStatus.VerificationSuccessful)
            {
                throw new Exception("Image transfer status is " + target.ImageTransferStatus.ToString());
            }

            if (!bFound)
            {
                throw new Exception("Image not found.");
            }

            //Step 7: Activate image.
            reply.Clear();
            ReadDataBlock(target.ImageActivate(Client), reply);
        }

        public bool ReadDataBlock(byte[][] data, GXReplyData reply)
        {
            foreach (byte[] it in data)
            {
                reply.Clear();
                ReadDataBlock(it, reply);
            }
            return true;
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
    }
}
