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
using Gurux.DLMS.Secure;
using Gurux.Serial;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace Gurux.DLMS.Reader
{
    public class GXDLMSReader
    {
        /// <summary>
        /// Wait time in ms.
        /// </summary>
        private int _waitTime = 5000;
        /// <summary>
        /// Retry count.
        /// </summary>
        private int _retryCount = 3;
        private IGXMedia _media;
        private TraceLevel _trace;
        private GXDLMSSecureClient _client;
        // Invocation counter (frame counter).
        private string _invocationCounter = null;
        private string _mediaSettings = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">DLMS Client.</param>
        /// <param name="media">Media.</param>
        /// <param name="trace">Trace level.</param>
        /// <param name="invocationCounter">Logical name of invocation counter.</param>
        /// <param name="iec">Is optical head used.</param>
        public GXDLMSReader(GXDLMSSecureClient client, IGXMedia media, TraceLevel trace, string invocationCounter)
        {
            _trace = trace;
            _media = media;
            _client = client;
            _invocationCounter = invocationCounter;
        }

        /// <summary>
        /// Read all data from the meter.
        /// </summary>
        public void ReadAll(string outputFile)
        {
            try
            {
                InitializeConnection();
                if (GetAssociationView(outputFile))
                {
                    GetScalersAndUnits();
                    GetProfileGenericColumns();
                }
                GetReadOut();
                GetProfileGenerics();
                if (outputFile != null)
                {
                    try
                    {
                        _client.Objects.Save(outputFile, new GXXmlWriterSettings() { UseMeterTime = true, IgnoreDefaultValues = false });
                    }
                    catch (Exception)
                    {
                        //It's OK if this fails.
                    }
                }
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Send SNRM Request to the meter.
        /// </summary>
        public void SNRMRequest()
        {
            GXReplyData reply = new GXReplyData();
            byte[] data;
            data = _client.SNRMRequest();
            if (data != null)
            {
                if (_trace > TraceLevel.Info)
                {
                    Console.WriteLine("Send SNRM request." + GXCommon.ToHex(data, true));
                }
                ReadDataBlock(data, reply);
                if (_trace == TraceLevel.Verbose)
                {
                    Console.WriteLine("Parsing UA reply." + reply.ToString());
                }
                //Has server accepted client.
                _client.ParseUAResponse(reply.Data);
                if (_trace > TraceLevel.Info)
                {
                    Console.WriteLine("Parsing UA reply succeeded.");
                }
            }
        }

        /// <summary>
        /// Send AARQ Request to the meter.
        /// </summary>
        public void AarqRequest()
        {
            GXReplyData reply = new GXReplyData();
            //Generate AARQ request.
            //Split requests to multiple packets if needed.
            //If password is used all data might not fit to one packet.
            foreach (byte[] it in _client.AARQRequest())
            {
                if (_trace > TraceLevel.Info)
                {
                    Console.WriteLine("Send AARQ request", GXCommon.ToHex(it, true));
                }
                reply.Clear();
                ReadDataBlock(it, reply);
            }
            if (_trace > TraceLevel.Info)
            {
                Console.WriteLine("Parsing AARE reply" + reply.ToString());
            }
            //Parse reply.
            _client.ParseAAREResponse(reply.Data);
            reply.Clear();
            //Get challenge Is HLS authentication is used.
            if (_client.IsAuthenticationRequired)
            {
                foreach (byte[] it in _client.GetApplicationAssociationRequest())
                {
                    reply.Clear();
                    ReadDataBlock(it, reply);
                }
                _client.ParseApplicationAssociationResponse(reply.Data);
            }
            if (_trace > TraceLevel.Info)
            {
                Console.WriteLine("Parsing AARE reply succeeded.");
            }
        }

        /// <summary>
        /// Read Invocation counter (frame counter) from the meter and update it.
        /// </summary>
        private void UpdateFrameCounter()
        {
            //Read frame counter if GeneralProtection is used.
            if (!string.IsNullOrEmpty(_invocationCounter) && _client.Ciphering != null && _client.Ciphering.Security != (byte)Security.None)
            {
                InitializeOpticalHead();
                byte[] data;
                GXReplyData reply = new GXReplyData();
                _client.ProposedConformance |= Conformance.GeneralProtection;
                int add = _client.ClientAddress;
                Authentication auth = _client.Authentication;
                Security security = _client.Ciphering.Security;
                byte[] challenge = _client.CtoSChallenge;
                Signing signing = _client.Ciphering.Signing;
                try
                {
                    _client.ClientAddress = 16;
                    _client.Authentication = Authentication.None;
                    _client.Ciphering.Security = (byte)Security.None;
                    _client.Ciphering.Signing = Signing.None;
                    data = _client.SNRMRequest();
                    if (data != null)
                    {
                        if (_trace > TraceLevel.Info)
                        {
                            Console.WriteLine("Send SNRM request." + GXCommon.ToHex(data, true));
                        }
                        ReadDataBlock(data, reply);
                        if (_trace == TraceLevel.Verbose)
                        {
                            Console.WriteLine("Parsing UA reply." + reply.ToString());
                        }
                        //Has server accepted client.
                        _client.ParseUAResponse(reply.Data);
                        if (_trace > TraceLevel.Info)
                        {
                            Console.WriteLine("Parsing UA reply succeeded.");
                        }
                    }
                    //Generate AARQ request.
                    //Split requests to multiple packets if needed.
                    //If password is used all data might not fit to one packet.
                    foreach (byte[] it in _client.AARQRequest())
                    {
                        if (_trace > TraceLevel.Info)
                        {
                            Console.WriteLine("Send AARQ request", GXCommon.ToHex(it, true));
                        }
                        reply.Clear();
                        ReadDataBlock(it, reply);
                    }
                    if (_trace > TraceLevel.Info)
                    {
                        Console.WriteLine("Parsing AARE reply" + reply.ToString());
                    }
                    try
                    {
                        //Parse reply.
                        _client.ParseAAREResponse(reply.Data);
                        reply.Clear();
                        GXDLMSData d = new GXDLMSData(_invocationCounter);
                        Read(d, 2);
                        _client.Ciphering.InvocationCounter = 1 + Convert.ToUInt32(d.Value);
                        Console.WriteLine("Invocation counter: " + Convert.ToString(_client.Ciphering.InvocationCounter));
                        reply.Clear();
                        if (_client.InterfaceType == InterfaceType.HdlcWithModeE)
                        {
                            Disconnect();
                            //Initialize IEC again for optical port connection.
                            if (!string.IsNullOrEmpty(_mediaSettings))
                            {
                                _media.Settings = _mediaSettings;
                            }
                            InitializeOpticalHead();
                        }
                        else
                        {
                            Disconnect();
                        }
                    }
                    catch (Exception Ex)
                    {
                        Disconnect();
                        throw Ex;
                    }
                }
                finally
                {
                    _client.ClientAddress = add;
                    _client.Authentication = auth;
                    _client.Ciphering.Security = security;
                    _client.CtoSChallenge = challenge;
                    _client.Ciphering.Signing = signing;
                }
            }
        }

        /// <summary>
        /// Send IEC disconnect message.
        /// </summary>
        void DiscIEC()
        {
            ReceiveParameters<string> p = new ReceiveParameters<string>()
            {
                AllData = false,
                Eop = (byte)0x0A,
                WaitTime = _waitTime * 1000
            };
            string data = (char)0x01 + "B0" + (char)0x03 + "\r\n";
            _media.Send(data, null);
            p.Eop = "\n";
            p.AllData = true;
            p.Count = 1;

            _media.Receive(p);
        }
        /// <summary>
        /// Initialize optical head.
        /// </summary>
        void InitializeOpticalHead()
        {
            if (_client.InterfaceType != InterfaceType.HdlcWithModeE)
            {
                return;
            }
            _mediaSettings = _media.Settings;
            GXSerial serial = _media as GXSerial;
            byte Terminator = (byte)0x0A;
            if (!_media.IsOpen)
            {
                _media.Open();
            }
            //Some meters need a little break.
            Thread.Sleep(1000);
            //Query device information.
            string data = "/?!\r\n";
            if (_trace > TraceLevel.Info)
            {
                Console.WriteLine("IEC Sending:" + data);
            }
            ReceiveParameters<string> p = new ReceiveParameters<string>()
            {
                AllData = false,
                Eop = Terminator,
                WaitTime = _waitTime * 1000
            };
            lock (_media.Synchronous)
            {
                _media.Send(data, null);
                if (!_media.Receive(p))
                {
                    //Try to move away from mode E.
                    try
                    {
                        Disconnect();
                    }
                    catch (Exception)
                    {
                    }
                    DiscIEC();
                    string str = "Failed to receive reply from the device in given time.";
                    if (_trace > TraceLevel.Info)
                    {
                        Console.WriteLine(str);
                    }
                    _media.Send(data, null);
                    if (!_media.Receive(p))
                    {
                        throw new Exception(str);
                    }
                }
                //If echo is used.
                if (p.Reply == data)
                {
                    p.Reply = null;
                    if (!_media.Receive(p))
                    {
                        //Try to move away from mode E.
                        GXReplyData reply = new GXReplyData();
                        Disconnect();
                        if (serial != null)
                        {
                            DiscIEC();
                            serial.DtrEnable = serial.RtsEnable = false;
                            serial.BaudRate = 9600;
                            serial.DtrEnable = serial.RtsEnable = true;
                            DiscIEC();
                        }
                        data = "Failed to receive reply from the device in given time.";
                        if (_trace > TraceLevel.Info)
                        {
                            Console.WriteLine(data);
                        }
                        throw new Exception(data);
                    }
                }
            }
            if (_trace > TraceLevel.Info)
            {
                Console.WriteLine("HDLC received: " + p.Reply);
            }
            if (p.Reply[0] != '/')
            {
                p.WaitTime = 100;
                _media.Receive(p);
                DiscIEC();
                throw new Exception("Invalid responce.");
            }
            string manufactureID = p.Reply.Substring(1, 3);
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
            if (_trace > TraceLevel.Info)
            {
                Console.WriteLine("BaudRate is : " + BaudRate.ToString());
            }
            //Send ACK
            //Send Protocol control character
            // "2" HDLC protocol procedure (Mode E)
            byte controlCharacter = (byte)'2';
            //Send Baud rate character
            //Mode control character
            byte ModeControlCharacter = (byte)'2';
            //"2" //(HDLC protocol procedure) (Binary mode)
            //Set mode E.
            byte[] arr = new byte[] { 0x06, controlCharacter, (byte)baudrate, ModeControlCharacter, 13, 10 };
            if (_trace > TraceLevel.Info)
            {
                Console.WriteLine("Moving to mode E.", arr);
            }
            lock (_media.Synchronous)
            {
                p.Reply = null;
                _media.Send(arr, null);
                //Some meters need this sleep. Do not remove.
                Thread.Sleep(200);
                p.WaitTime = 2000;
                //Note! All meters do not echo this.
                _media.Receive(p);
                if (p.Reply != null)
                {
                    if (_trace > TraceLevel.Info)
                    {
                        Console.WriteLine("Received: " + p.Reply);
                    }
                }
                serial.BaudRate = BaudRate;
                serial.DataBits = 8;
                serial.Parity = Parity.None;
                serial.StopBits = StopBits.One;
                //Some meters need this sleep. Do not remove.
                Thread.Sleep(800);
            }
        }


        /// <summary>
        /// Initialize connection to the meter.
        /// </summary>
        public void InitializeConnection()
        {
            Console.WriteLine("Standard: " + _client.Standard);
            if (_client.Ciphering.Security != (byte)Security.None)
            {
                Console.WriteLine("Security: " + _client.Ciphering.Security);
                Console.WriteLine("System title: " + GXCommon.ToHex(_client.Ciphering.SystemTitle, true));
                Console.WriteLine("Authentication key: " + GXCommon.ToHex(_client.Ciphering.AuthenticationKey, true));
                Console.WriteLine("Block cipher key " + GXCommon.ToHex(_client.Ciphering.BlockCipherKey, true));
                if (_client.Ciphering.DedicatedKey != null)
                {
                    Console.WriteLine("Dedicated key: " + GXCommon.ToHex(_client.Ciphering.DedicatedKey, true));
                }
            }
            UpdateFrameCounter();
            InitializeOpticalHead();
            GXReplyData reply = new GXReplyData();
            SNRMRequest();
            //Generate AARQ request.
            //Split requests to multiple packets if needed.
            //If password is used all data might not fit to one packet.
            foreach (byte[] it in _client.AARQRequest())
            {
                if (_trace > TraceLevel.Info)
                {
                    Console.WriteLine("Send AARQ request", GXCommon.ToHex(it, true));
                }
                reply.Clear();
                ReadDataBlock(it, reply);
            }
            if (_trace > TraceLevel.Info)
            {
                Console.WriteLine("Parsing AARE reply" + reply.ToString());
            }
            //Parse reply.
            _client.ParseAAREResponse(reply.Data);
            reply.Clear();
            //Get challenge Is HLS authentication is used.
            if (_client.IsAuthenticationRequired)
            {
                foreach (byte[] it in _client.GetApplicationAssociationRequest())
                {
                    reply.Clear();
                    ReadDataBlock(it, reply);
                }
                _client.ParseApplicationAssociationResponse(reply.Data);
            }
            if (_trace > TraceLevel.Info)
            {
                Console.WriteLine("Parsing AARE reply succeeded.");
            }
        }

        /// <summary>
        /// Read association view.
        /// </summary>
        public bool GetAssociationView(string outputFile)
        {
            if (outputFile != null)
            {
                //Save Association view to the cache so it is not needed to retrieve every time.
                if (File.Exists(outputFile))
                {
                    try
                    {
                        _client.Objects.Clear();
                        _client.Objects.AddRange(GXDLMSObjectCollection.Load(outputFile));
                        return false;
                    }
                    catch (Exception)
                    {
                        if (File.Exists(outputFile))
                        {
                            File.Delete(outputFile);
                        }
                    }
                }
            }
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(_client.GetObjectsRequest(), reply);
            _client.ParseObjects(reply.Data, true);
            //Access rights must read differently when short Name referencing is used.
            if (!_client.UseLogicalNameReferencing)
            {
                GXDLMSAssociationShortName sn = (GXDLMSAssociationShortName)_client.Objects.FindBySN(0xFA00);
                if (sn != null && sn.Version > 0)
                {
                    Read(sn, 3);
                }
            }
            if (outputFile != null)
            {
                try
                {
                    _client.Objects.Save(outputFile, new GXXmlWriterSettings() { Values = false });
                }
                catch (Exception)
                {
                    //It's OK if this fails.
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Read scalers and units.
        /// </summary>
        public void GetScalersAndUnits()
        {
            GXDLMSObjectCollection objs = _client.Objects.GetObjects(new ObjectType[] { ObjectType.Register, ObjectType.ExtendedRegister, ObjectType.DemandRegister });
            //If trace is info.
            if (_trace > TraceLevel.Warning)
            {
                Console.WriteLine("Read scalers and units from the device.");
            }
            if ((_client.NegotiatedConformance & Gurux.DLMS.Enums.Conformance.MultipleReferences) != 0)
            {
                List<KeyValuePair<GXDLMSObject, int>> list = new List<KeyValuePair<GXDLMSObject, int>>();
                foreach (GXDLMSObject it in objs)
                {
                    if (it is GXDLMSRegister || it is GXDLMSExtendedRegister)
                    {
                        list.Add(new KeyValuePair<GXDLMSObject, int>(it, 3));
                    }
                    if (it is GXDLMSDemandRegister)
                    {
                        list.Add(new KeyValuePair<GXDLMSObject, int>(it, 4));
                    }
                }
                if (list.Count != 0)
                {
                    try
                    {
                        ReadList(list);
                    }
                    catch (Exception)
                    {
                        _client.NegotiatedConformance &= ~Gurux.DLMS.Enums.Conformance.MultipleReferences;
                    }
                }
            }
            if ((_client.NegotiatedConformance & Gurux.DLMS.Enums.Conformance.MultipleReferences) == 0)
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
            foreach (GXDLMSObject it in _client.Objects.GetObjects(ObjectType.ProfileGeneric))
            {
                try
                {
                    //If info.
                    if (_trace > TraceLevel.Warning)
                    {
                        Console.WriteLine(it.LogicalName);
                    }
                    Read(it, 3);
                    //If info.
                    if (_trace > TraceLevel.Warning)
                    {
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
                        Console.WriteLine(sb.ToString());
                    }
                }
                catch (Exception ex)
                {
                    //Continue reading.
                }
            }
        }

        public void ShowValue(object val, int pos)
        {
            //If trace is info.
            if (_trace > TraceLevel.Warning)
            {
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
        }

        public void GetProfileGenerics()
        {
            //Find profile generics register objects and read them.
            foreach (GXDLMSObject it in _client.Objects.GetObjects(ObjectType.ProfileGeneric))
            {
                //If trace is info.
                if (_trace > TraceLevel.Warning)
                {
                    Console.WriteLine("-------- Reading " + it.GetType().Name + " " + it.Name + " " + it.Description);
                }
                Read(it, 4);
                Read(it, 5);
                Read(it, 6);
                long entriesInUse = Convert.ToInt64(Read(it, 7));
                long entries = Convert.ToInt64(Read(it, 8));
                //If trace is info.
                if (_trace > TraceLevel.Warning)
                {
                    Console.WriteLine("Entries: " + entriesInUse + "/" + entries);
                }
                //If there are no columns or rows.
                if (entriesInUse == 0 || (it as GXDLMSProfileGeneric).CaptureObjects.Count == 0)
                {
                    continue;
                }
                //All meters are not supporting parameterized read.
                if ((_client.NegotiatedConformance & (Gurux.DLMS.Enums.Conformance.ParameterizedAccess | Gurux.DLMS.Enums.Conformance.SelectiveAccess)) != 0)
                {
                    try
                    {
                        //Read first row from Profile Generic.
                        object[] rows = ReadRowsByEntry(it as GXDLMSProfileGeneric, 1, 1);
                        //If trace is info.
                        if (_trace > TraceLevel.Warning)
                        {
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
                            Console.WriteLine(sb.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error! Failed to read first row: " + ex.Message);
                        //Continue reading.
                    }
                }
                //All meters are not supporting parameterized read.
                if ((_client.NegotiatedConformance & (Gurux.DLMS.Enums.Conformance.ParameterizedAccess | Gurux.DLMS.Enums.Conformance.SelectiveAccess)) != 0)
                {
                    try
                    {
                        //Read last day from Profile Generic.
                        object[] rows = ReadRowsByRange(it as GXDLMSProfileGeneric, DateTime.Now.Date, DateTime.MaxValue);
                        //If trace is info.
                        if (_trace > TraceLevel.Warning)
                        {
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
                            Console.WriteLine(sb.ToString());
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
        /// Read all objects from the meter.
        /// </summary>
        /// <remarks>
        /// It's not normal to read all data from the meter. This is just an example.
        /// </remarks>
        public void GetReadOut()
        {
            foreach (GXDLMSObject it in _client.Objects)
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
                    if (_trace > TraceLevel.Error)
                    {
                        Console.WriteLine("Unknown Interface: " + it.ObjectType.ToString());
                    }
                    continue;
                }
                if (_trace > TraceLevel.Warning)
                {
                    Console.WriteLine("-------- Reading " + it.GetType().Name + " " + it.Name + " " + it.Description);
                }
                foreach (int pos in (it as IGXDLMSBase).GetAttributeIndexToRead(true))
                {
                    try
                    {
                        if ((it.GetAccess(pos) & AccessMode.Read) != 0)
                        {
                            object val = Read(it, pos);
                            ShowValue(val, pos);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error! " + it.GetType().Name + " " + it.Name + "Index: " + pos + " " + ex.Message);
                        Console.WriteLine(ex.ToString());
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
            if (data == null && !reply.IsStreaming())
            {
                return;
            }
            GXReplyData notify = new GXReplyData();
            reply.Error = 0;
            object eop = (byte)0x7E;
            //In network connection terminator is not used.
            if (_client.InterfaceType == InterfaceType.WRAPPER)
            {
                eop = null;
            }
            int pos = 0;
            bool succeeded = false;
            ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
            {
                Eop = eop,
                WaitTime = _waitTime,
            };
            if (eop == null)
            {
                p.Count = 8;
            }
            else
            {
                p.Count = 5;
            }
            GXByteBuffer rd = new GXByteBuffer();
            lock (_media.Synchronous)
            {
                while (!succeeded && pos != 3)
                {
                    if (!reply.IsStreaming())
                    {
                        WriteTrace("TX:\t" + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(data, true));
                        _media.Send(data, null);
                    }
                    succeeded = _media.Receive(p);
                    if (!succeeded)
                    {
                        if (++pos >= _retryCount)
                        {
                            throw new Exception("Failed to receive reply from the device in given time.");
                        }
                        //If Eop is not set read one byte at time.
                        if (p.Eop == null)
                        {
                            p.Count = 1;
                        }
                        //Try to read again...
                        System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                    }
                }
                rd = new GXByteBuffer(p.Reply);
                try
                {
                    pos = 0;
                    //Loop until whole COSEM packet is received.
                    while (!_client.GetData(rd, reply, notify))
                    {
                        p.Reply = null;
                        if (notify.IsComplete && notify.Data.Data != null)
                        {
                            //Handle notify.
                            if (!notify.IsMoreData)
                            {
                                //Show received push message as XML.
                                string xml;
                                GXDLMSTranslator t = new GXDLMSTranslator(TranslatorOutputType.SimpleXml);
                                t.DataToXml(notify.Data, out xml);
                                Console.WriteLine(xml);
                                notify.Clear();
                                continue;
                            }
                        }
                        if (p.Eop == null)
                        {
                            p.Count = _client.GetFrameSize(rd);
                        }
                        while (!_media.Receive(p))
                        {
                            if (++pos >= _retryCount)
                            {
                                throw new Exception("Failed to receive reply from the device in given time.");
                            }
                            //If echo.
                            if (rd == null || rd.Size == data.Length)
                            {
                                _media.Send(data, null);
                            }
                            //Try to read again...
                            System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                        }
                        rd.Set(p.Reply);
                    }
                }
                catch (Exception ex)
                {
                    WriteTrace("RX:\t" + DateTime.Now.ToLongTimeString() + "\t" + rd);
                    throw ex;
                }
            }
            WriteTrace("RX:\t" + DateTime.Now.ToLongTimeString() + "\t" + rd);
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
            if (data == null)
            {
                return true;
            }
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
            lock (_media.Synchronous)
            {
                while (reply.IsMoreData)
                {
                    if (reply.IsStreaming())
                    {
                        data = null;
                    }
                    else
                    {
                        data = _client.ReceiverReady(reply);
                    }
                    ReadDLMSPacket(data, reply);
                    if (_trace > TraceLevel.Info)
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

        /// <summary>
        /// Read attribute value.
        /// </summary>
        /// <param name="it">COSEM object to read.</param>
        /// <param name="attributeIndex">Attribute index.</param>
        /// <returns>Read value.</returns>
        public object Read(GXDLMSObject it, int attributeIndex)
        {
            if ((it.GetAccess(attributeIndex) & AccessMode.Read) != 0)
            {
                GXReplyData reply = new GXReplyData();
                if (!ReadDataBlock(_client.Read(it, attributeIndex), reply))
                {
                    if (reply.Error != (short)ErrorCode.Rejected)
                    {
                        throw new GXDLMSException(reply.Error);
                    }
                    reply.Clear();
                    Thread.Sleep(1000);
                    if (!ReadDataBlock(_client.Read(it, attributeIndex), reply))
                    {
                        throw new GXDLMSException(reply.Error);
                    }
                }
                //Update data type.
                if (it.GetDataType(attributeIndex) == DataType.None)
                {
                    it.SetDataType(attributeIndex, reply.DataType);
                }
                return _client.UpdateValue(it, attributeIndex, reply.Value);
            }

            return null;
        }


        /// <summary>
        /// Read list of attributes.
        /// </summary>
        public void ReadList(List<KeyValuePair<GXDLMSObject, int>> list)
        {
            byte[][] data = _client.ReadList(list);
            GXReplyData reply = new GXReplyData();
            List<object> values = new List<object>();
            foreach (byte[] it in data)
            {
                ReadDataBlock(it, reply);
                //Value is null if data is send in multiple frames.
                if (reply.Value is IEnumerable<object>)
                {
                    values.AddRange((IEnumerable<object>)reply.Value);
                }
                else if (reply.Value != null)
                {
                    values.Add(reply.Value);
                }
                reply.Clear();
            }
            if (values.Count != list.Count)
            {
                throw new Exception("Invalid reply. Read items count do not match.");
            }
            _client.UpdateValues(list, values);
        }

        /// <summary>
        /// Write list of attributes.
        /// </summary>
        public void WriteList(List<KeyValuePair<GXDLMSObject, int>> list)
        {
            byte[][] data = _client.WriteList(list);
            GXReplyData reply = new GXReplyData();
            foreach (byte[] it in data)
            {
                ReadDataBlock(it, reply);
                reply.Clear();
            }
        }

        /// <summary>
        /// Write attribute value.
        /// </summary>
        public void Write(GXDLMSObject it, int attributeIndex)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(_client.Write(it, attributeIndex), reply);
        }

        /// <summary>
        /// Method attribute value.
        /// </summary>
        public void Method(GXDLMSObject it, int attributeIndex, object value, DataType type)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(_client.Method(it, attributeIndex, value, type), reply);
        }

        /// <summary>
        /// Read Profile Generic Columns by entry.
        /// </summary>
        public object[] ReadRowsByEntry(GXDLMSProfileGeneric it, UInt32 index, UInt32 count)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(_client.ReadRowsByEntry(it, index, count), reply);
            return (object[])_client.UpdateValue(it, 2, reply.Value);
        }

        /// <summary>
        /// Read Profile Generic Columns by range.
        /// </summary>
        public object[] ReadRowsByRange(GXDLMSProfileGeneric it, DateTime start, DateTime end)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(_client.ReadRowsByRange(it, start, end), reply);
            return (object[])_client.UpdateValue(it, 2, reply.Value);
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        public void Disconnect()
        {
            if (_media != null && _client != null)
            {
                try
                {
                    if (_trace > TraceLevel.Info)
                    {
                        Console.WriteLine("Disconnecting from the meter.");
                    }
                    GXReplyData reply = new GXReplyData();
                    ReadDLMSPacket(_client.DisconnectRequest(), reply);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Release.
        /// </summary>
        public void Release()
        {
            if (_media != null && _client != null)
            {
                try
                {
                    if (_trace > TraceLevel.Info)
                    {
                        Console.WriteLine("Release from the meter.");
                    }
                    GXReplyData reply = new GXReplyData();
                    ReadDataBlock(_client.ReleaseRequest(), reply);
                }
                catch (Exception ex)
                {
                    //All meters don't support Release.
                    Console.WriteLine("Release failed. " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Close connection to the meter.
        /// </summary>
        public void Close()
        {
            if (_media != null && _client != null)
            {
                try
                {
                    if (_trace > TraceLevel.Info)
                    {
                        Console.WriteLine("Disconnecting from the meter.");
                    }
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        //Release is call only for secured connections.
                        //All meters are not supporting Release and it's causing problems.
                        if (_client.InterfaceType == InterfaceType.WRAPPER ||
                            (_client.InterfaceType == InterfaceType.HDLC && _client.Ciphering.Security != (byte)Security.None))
                        {
                            ReadDataBlock(_client.ReleaseRequest(), reply);
                        }
                    }
                    catch (Exception ex)
                    {
                        //All meters don't support Release.
                        Console.WriteLine("Release failed. " + ex.Message);
                    }
                    reply.Clear();
                    ReadDLMSPacket(_client.DisconnectRequest(), reply);
                    _media.Close();
                }
                catch
                {

                }
                _media = null;
                _client = null;
            }
        }

        /// <summary>
        /// Write trace.
        /// </summary>
        /// <param name="line"></param>
        void WriteTrace(string line)
        {
            if (_trace > TraceLevel.Info)
            {
                Console.WriteLine(line);
            }
            using (FileStream fs = File.Open("trace.txt", FileMode.Append))
            {
                using (TextWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}
