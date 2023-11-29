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
using Gurux.DLMS.Objects;
using Gurux.Net;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;
using System.IO;
using System.Diagnostics;
using Gurux.Common;
using System.Net.Sockets;
using Gurux.Serial;
using System.Threading;
using System.IO.Ports;
using Gurux.DLMS.Objects.Enums;
using System.Text;

namespace Gurux.DLMS.Simulator.Net
{
    /// <summary>
    /// Simulated meter.
    /// </summary>
    class GXDLMSMeter : GXDLMSSecureServer
    {
        //Image to update.
        string ImageUpdate = null;
        //What is expected image size.
        UInt32 ImageSize = 0;

        /// <summary>
        /// Application is closing
        /// </summary>
        AutoResetEvent closing = new AutoResetEvent(false);
        /// <summary>
        /// Server that is used to parse Gateway messages.
        /// </summary>
        public static GXDLMSMeter GatewayServer = null;

        static Dictionary<object, GXByteBuffer> buffers = new Dictionary<object, GXByteBuffer>();

        /// <summary>
        /// List of simulated meters.
        /// </summary>
        public static Dictionary<int, GXDLMSMeter> meters = new Dictionary<int, GXDLMSMeter>();

        /// <summary>
        /// List of gateway clients.
        /// </summary>
        public static Dictionary<int, GXDLMSClient> clients = new Dictionary<int, GXDLMSClient>();

        /// <summary>
        /// List of connections. This is used to close connection if meter is leave without diconnect.
        /// </summary>
        public static Dictionary<object, GXDLMSMeter> connections = new Dictionary<object, GXDLMSMeter>();

        static InterfaceType interfaceType;

        //Are all meters using the same port.
        bool Exclusive;
        string objectsFile;
        static TraceLevel Trace = TraceLevel.Error;
        /// <summary>
        /// Lock settings file when used.
        /// </summary>
        private static object settingsLock = new object();

        Gurux.Common.IGXMedia Media = null;
        /// <summary>
        /// Serial number of the meter.
        /// </summary>
        UInt32 serialNumber;

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="logicalNameReferencing">Is logical name referencing used.</param>
        ///<param name="type">Interface type.</param>
        ///<param name="useUtc2NormalTime">Is UTC time used.</param>
        ///<param name="flagId">Flag ID.</param>
        public GXDLMSMeter(bool logicalNameReferencing, InterfaceType type, bool useUtc2NormalTime,
            string flagId) : base(logicalNameReferencing, type)
        {
            interfaceType = type;
            UseUtc2NormalTime = useUtc2NormalTime;
            FlaID = flagId;
        }
        public void Initialize(IGXMedia media, TraceLevel trace, string path, UInt32 sn, bool exclusive)
        {
            serialNumber = sn;
            objectsFile = path;
            Media = media;
            Trace = trace;
            Exclusive = exclusive;
            // Each association has own conformance.
            Conformance = Conformance.None;
            Init(exclusive);
        }

        /// <summary>
        /// Update simulated values for the meter instance.
        /// </summary>
        /// <param name="items">Simulated COSEM objects.</param>
        void UpdateValues(GXDLMSObjectCollection items)
        {
            //Update COSEM Logical Device Name
            GXDLMSData d = items.FindByLN(ObjectType.Data, "0.0.42.0.0.255") as GXDLMSData;
            if (d != null && d.Value is string v)
            {
                d.Value = string.Format("{0}{1}", v.Substring(0, 3), serialNumber.ToString("D13"));
            }

            //Update Meter serial number.
            d = items.FindByLN(ObjectType.Data, "0.0.96.1.0.255") as GXDLMSData;
            if (d != null && d.Value is string v2)
            {
                string tmp = "";
                foreach (char it in v2)
                {
                    //Append chars.
                    if (it < 0x30 || it > 0x39)
                    {
                        tmp += it;
                    }
                    else
                    {
                        break;
                    }
                }
                d.Value = tmp + serialNumber.ToString("D" + Convert.ToString(v2.Length - tmp.Length));
            }
        }

        /// <summary>
        /// Load saved COSEM objects from XML.
        /// </summary>
        /// <param name="path">File path.</param>
        bool LoadObjects(string path, GXDLMSObjectCollection items)
        {
            lock (settingsLock)
            {
                if (File.Exists(path))
                {
                    GXDLMSObjectCollection objects = GXDLMSObjectCollection.Load(path);
                    items.Clear();
                    items.AddRange(objects);
                    //Add objects from profile generic that are not in association view.
                    foreach (GXDLMSProfileGeneric pg in objects.GetObjects(ObjectType.ProfileGeneric))
                    {
                        //Remove invalid rows.
                        for (int pos = 0; pos != pg.Buffer.Count; ++pos)
                        {
                            if (pg.Buffer[pos].Length != pg.CaptureObjects.Count)
                            {
                                pg.Buffer.RemoveAt(pos);
                                --pos;
                            }
                        }

                        pg.EntriesInUse = (UInt32)pg.Buffer.Count;
                        foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in pg.CaptureObjects)
                        {
                            if (objects.FindByLN(it.Key.ObjectType, it.Key.LogicalName) == null)
                            {
                                objects.Add(it.Key);
                            }
                        }
                    }
                    UpdateValues(items);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Client has send data for for the gateway.
        /// </summary>
        /// <remarks>
        /// GW finds the correct client and sends data for it.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnGatewayReceived(object sender, Gurux.Common.ReceiveEventArgs e)
        {
            try
            {
                lock (buffers)
                {
                    GXByteBuffer bb;
                    if (!buffers.ContainsKey(e.SenderInfo))
                    {
                        bb = new GXByteBuffer();
                        buffers[e.SenderInfo] = bb;
                    }
                    else
                    {
                        bb = buffers[e.SenderInfo];
                    }
                    bb.Set((byte[])e.Data);
                    GXServerReply sr = new GXServerReply(bb.Data);
                    GatewayServer.Reset();
                    try
                    {
                        GatewayServer.HandleRequest(sr);
                        if (sr.Reply != null)
                        {
                            if (Trace > TraceLevel.Info)
                            {
                                Console.WriteLine("TX:\t" + Gurux.Common.GXCommon.ToHex(sr.Reply, true));
                            }
                            ((IGXMedia)sender).Send(sr.Reply, e.SenderInfo);
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        //Return error.
                        sr.Reply = GatewayServer.ReportError(sr.Command, ErrorCode.HardwareFault);
                    }
                    if (sr.Gateway != null && sr.Data != null)
                    {
                        GXByteBuffer pdu = new GXByteBuffer(sr.Data);
                        InterfaceType type = (InterfaceType)sr.Gateway.NetworkId;
                        GXByteBuffer address = new GXByteBuffer();
                        address.Set(sr.Gateway.PhysicalDeviceAddress);
                        int addr = address.GetUInt8();
                        //Find correct meter using GW information.
                        if (meters.ContainsKey(addr))
                        {
                            //Find client for the server or create a new one.
                            GXDLMSClient cl;
                            if (!clients.ContainsKey(addr))
                            {
                                //Set client address if data is send without framing.
                                if (GatewayServer.Settings.ClientAddress == 0)
                                {
                                    GatewayServer.Settings.ClientAddress = 0x10;
                                }
                                cl = new GXDLMSClient(true, GatewayServer.Settings.ClientAddress, addr, GatewayServer.Authentication, null, type);
                                clients.Add(addr, cl);
                            }
                            else
                            {
                                cl = clients[addr];
                            }
                            GXReplyData data = new GXReplyData();
                            GXReplyData notify = new GXReplyData();
                            GXDLMSMeter m = meters[addr];
                            //Send SNRM if needed.
                            if (sr.Command == Command.Aarq && (type == InterfaceType.HDLC || type == InterfaceType.HdlcWithModeE))
                            {
                                GXServerReply sr2 = new GXServerReply(cl.SNRMRequest());
                                m.HandleRequest(sr2);
                                if (cl.GetData(sr2.Reply, data, notify))
                                {
                                    data.Clear();
                                    notify.Clear();
                                }
                                else
                                {
                                    //If the meter doesn't reply.
                                    bb.Clear();
                                    return;
                                }
                            }
                            byte[][] frames = cl.CustomFrameRequest(Command.None, pdu);
                            foreach (byte[] it in frames)
                            {
                                sr.Data = it;
                                m.HandleRequest(sr);
                                if (Trace > TraceLevel.Info)
                                {
                                    Console.WriteLine("RX:\t" + Gurux.Common.GXCommon.ToHex(sr.Reply, true));
                                }
                                data.RawPdu = true;
                                if (cl.GetData(sr.Reply, data, notify))
                                {
                                    while (data.IsMoreData)
                                    {
                                        sr.Data = cl.ReceiverReady(data);
                                        m.HandleRequest(sr);
                                        if (Trace > TraceLevel.Info)
                                        {
                                            Console.WriteLine("RX:\t" + Gurux.Common.GXCommon.ToHex(sr.Reply, true));
                                        }
                                        cl.GetData(sr.Reply, data, notify);
                                    }
                                    byte[] reply = sr.Reply;
                                    try
                                    {
                                        GXByteBuffer tmp = new GXByteBuffer();
                                        tmp.Set(data.Data);
                                        GatewayServer.Gateway = sr.Gateway;
                                        reply = GatewayServer.CustomFrameRequest(Command.None, tmp);
                                    }
                                    finally
                                    {
                                        GatewayServer.Gateway = null;
                                    }
                                    if (Trace > TraceLevel.Info)
                                    {
                                        Console.WriteLine("TX:\t" + Gurux.Common.GXCommon.ToHex(reply, true));
                                    }
                                    ((IGXMedia)sender).Send(reply, e.SenderInfo);
                                }
                            }
                        }
                        bb.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                if (!(ex is SocketException))
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void HandleReply(GXByteBuffer bb, Gurux.Common.ReceiveEventArgs e)
        {
            try
            {
                lock (bb)
                {
                    int target, source;
                    //All simulated meters are using the same interface type.
                    GXDLMSTranslator.GetAddressInfo(interfaceType, bb, out target, out source);
                    if (target != 0 && meters.ContainsKey(target))
                    {
                        GXDLMSMeter m = meters[target];
                        GXServerReply sr = new GXServerReply(bb.Data);
                        sr.ConnectionInfo = new GXDLMSConnectionEventArgs() { ConnectionInfo = e.SenderInfo };
                        do
                        {
                            m.HandleRequest(sr);
                            //Reply is null if we do not want to send any data to the client.
                            //This is done if client try to make connection with wrong device ID.
                            if (sr.Reply != null)
                            {
                                if (Trace > TraceLevel.Info)
                                {
                                    Console.WriteLine("TX:\t" + Gurux.Common.GXCommon.ToHex(sr.Reply, true));
                                }
                                bb.Clear();
                                m.Media.Send(sr.Reply, e.SenderInfo);
                                sr.Data = null;
                            }
                        }
                        while (sr.IsStreaming);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!(ex is SocketException))
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Client has send data for the meters that are using the same port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnExclusiveReceived(object sender, Gurux.Common.ReceiveEventArgs e)
        {
            try
            {
                if (Trace > TraceLevel.Info)
                {
                    Console.WriteLine("RX:\t" + Gurux.Common.GXCommon.ToHex((byte[])e.Data, true));
                }
                GXByteBuffer bb;
                lock (buffers)
                {
                    if (!buffers.ContainsKey(e.SenderInfo))
                    {
                        bb = new GXByteBuffer();
                        buffers[e.SenderInfo] = bb;
                    }
                    else
                    {
                        bb = buffers[e.SenderInfo];
                    }
                    lock (bb)
                    {
                        bb.Set((byte[])e.Data);
                    }
                }
                //Each reply is handled in own thread.
                new Thread(() =>
                {
                    HandleReply(bb, e);
                }).Start();
            }
            catch (Exception ex)
            {
                if (!(ex is SocketException))
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        bool Init(bool exclusive)
        {
            //Load added objects.
            if (!LoadObjects(objectsFile, Items))
            {
                throw new Exception(string.Format("Invalid device template file {0}", objectsFile));
            }
            GXDLMSObjectCollection objs;
            //Find default local port setup when optical head is used.
            if (InterfaceType == InterfaceType.HdlcWithModeE)
            {
                objs = Items.GetObjects(ObjectType.IecLocalPortSetup);
                if (objs.Count != 0)
                {
                    LocalPortSetup = (GXDLMSIECLocalPortSetup)objs[0];
                }
                else
                {
                    throw new Exception("HdlcWithModeE can't be used because LocalPortSetup not found.");
                }
            }
            //Find default HDLC Setup settings.
            objs = Items.GetObjects(ObjectType.IecHdlcSetup);
            if (objs.Count != 0)
            {
                Hdlc = (GXDLMSHdlcSetup)objs[0];
            }
            //Find default Tcp/IP setup Setup settings.
            objs = Items.GetObjects(ObjectType.TcpUdpSetup);
            if (objs.Count != 0)
            {
                Wrapper = (GXDLMSTcpUdpSetup)objs[0];
            }

            //Create thread for every profile generic so values are captured if capture period is given.
            new Thread(() =>
            {
                int wt = 0;
                do
                {
                    wt = Run(closing);
                    //Wait until next event needs to execute.
                    Console.WriteLine("Waiting " + TimeSpan.FromSeconds(wt).ToString() + " before next execution.");
                    wt *= 1000;
                    wt -= DateTime.Now.Millisecond;
                }
                while (!closing.WaitOne(wt));
            }).Start();

            //Own listener isn't created if there are multiple meters in the same port.
            if (!exclusive)
            {
                Media.OnReceived += new Gurux.Common.ReceivedEventHandler(OnReceived);
                Media.OnClientConnected += new Gurux.Common.ClientConnectedEventHandler(OnClientConnected);
                Media.OnClientDisconnected += new Gurux.Common.ClientDisconnectedEventHandler(OnClientDisconnected);
                Media.OnError += new Gurux.Common.ErrorEventHandler(OnError);
            }
            if (!Media.IsOpen)
            {
                Media.Open();
            }
            ///////////////////////////////////////////////////////////////////////
            //Server must initialize after all objects are added.
            Initialize();
            return true;
        }

        public override void Close()
        {
            closing.Set();
            base.Close();
            if (Media != null)
            {
                Media.Close();
            }
        }

        public static void OnError(object sender, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

        /// <summary>
        /// Each server has own history file.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private string GetProfileGenericName(GXDLMSObject target)
        {
            string name = (Media as GXNet).Port + "_" + target.LogicalName + ".csv";
            return name;
        }

        /// <summary>
        /// Generic read handle for all servers.
        /// Update dynamic values here.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="e"></param>
        protected override void PreRead(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                if (Trace > TraceLevel.Warning)
                {
                    System.Diagnostics.Debug.WriteLine("PreRead {0}:{1}", it.Target.LogicalName, it.Index);
                }
                //Update date-time of the clock object when client asks it.
                if ((it.Target is GXDLMSClock c) && it.Index == 2)
                {
                    c.Time = c.Now(UseUtc2NormalTime);
                    //Set milliseconds to zero.
                    c.Time.Value = c.Time.Value.AddMilliseconds(-c.Time.Value.Millisecond);
                }
            }
        }

        protected override void PostRead(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                if (Trace > TraceLevel.Warning)
                {
                    System.Diagnostics.Debug.WriteLine("PostRead {0}:{1}", it.Target.LogicalName, it.Index);
                }
            }
        }

        protected override void PreWrite(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                if (Trace > TraceLevel.Warning)
                {
                    System.Diagnostics.Debug.WriteLine("PreWrite {0}:{1}", it.Target.LogicalName, it.Index);
                }
            }
        }

        protected override void PostWrite(ValueEventArgs[] args)
        {
            GXXmlWriterSettings settings = new GXXmlWriterSettings();
            foreach (ValueEventArgs it in args)
            {
                if (it.Error != ErrorCode.Ok)
                {
                    // Load default values if user has try to save invalid data.
                    Items.Clear();
                    Items.AddRange(GXDLMSObjectCollection.Load(objectsFile));
                    return;
                }
            }
            Items.Save(objectsFile, settings);
        }

        protected override void InvalidConnection(GXDLMSConnectionEventArgs e)
        {
        }

        void SendPush(GXDLMSPushSetup target)
        {
            int pos = target.Destination.IndexOf(':');
            if (pos == -1)
            {
                throw new ArgumentException("Invalid destination.");
            }
            byte[][] data = GeneratePushSetupMessages(DateTime.MinValue, target);
            string host = target.Destination.Substring(0, pos);
            int port = int.Parse(target.Destination.Substring(pos + 1));
            GXNet net = new GXNet(NetworkType.Tcp, host, port);
            try
            {
                net.Open();
                foreach (byte[] it in data)
                {
                    net.Send(it, null);
                }
            }
            finally
            {
                net.Close();
            }
        }

        protected override void PreAction(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                if (Trace > TraceLevel.Warning)
                {
                    System.Diagnostics.Debug.WriteLine("PreAction {0}:{1}", it.Target.LogicalName, it.Index);
                }
                if ((it.Target is GXDLMSProfileGeneric pg) && it.Index == 2)
                {
                    //Update clock for profile-generic object when capture is invoked.
                    foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> co in pg.CaptureObjects)
                    {
                        if ((co.Key is GXDLMSClock clock) && co.Value.AttributeIndex == 2)
                        {
                            clock.Time = clock.Now();
                        }
                    }
                }
                if ((it.Target is GXDLMSPushSetup push) && it.Index == 1)
                {
                    //Send push msg.
                    SendPush(push);
                    it.Handled = true; 
                    continue;
                }
                if ((it.Target is GXDLMSAutoConnect ac) && it.Index == 1)
                {
                    //Connect for the give IP address.
                    it.Handled = true;
                    continue;
                }
                

                if (it.Target is GXDLMSImageTransfer)
                {
                    GXDLMSImageTransfer i = it.Target as GXDLMSImageTransfer;
                    //Image name and size to transfer
                    if (it.Index == 1)
                    {
                        i.ImageTransferStatus = ImageTransferStatus.NotInitiated;
                        i.ImageActivateInfo = null;
                        ImageUpdate = ASCIIEncoding.ASCII.GetString((byte[])(it.Parameters as List<object>)[0]);
                        ImageSize = Convert.ToUInt32((it.Parameters as List<object>)[1]);
                        string file = Path.Combine(Path.GetDirectoryName(typeof(GXDLMSMeter).Assembly.Location), ImageUpdate + ".exe");
                        System.Diagnostics.Debug.WriteLine("Updating image" + ImageUpdate + " Size:" + ImageSize);
                        using (var writer = File.Create(file))
                        {
                        }
                    }
                    //Transfers one block of the Image to the server
                    else if (it.Index == 2)
                    {
                        i.ImageTransferStatus = ImageTransferStatus.TransferInitiated;
                        string file = Path.Combine(Path.GetDirectoryName(typeof(GXDLMSMeter).Assembly.Location), ImageUpdate + ".exe");
                        List<object> p = (List<object>)it.Parameters;
                        try
                        {
                            using (FileStream fs = new FileStream(file, FileMode.Append))
                            {
                                using (BinaryWriter writer = new BinaryWriter(fs))
                                {
                                    writer.Write((byte[])p[1]);
                                }
                                fs.Close();
                            }
                        }
                        catch (System.IO.IOException)
                        {
                            Thread.Sleep(1000);
                            using (FileStream fs = new FileStream(file, FileMode.Append))
                            {
                                using (BinaryWriter writer = new BinaryWriter(fs))
                                {
                                    writer.Write((byte[])p[1]);
                                }
                                fs.Close();
                            }
                        }
                    }
                    //Verifies the integrity of the Image before activation.
                    else if (it.Index == 3)
                    {
                        string file = Path.Combine(Path.GetDirectoryName(typeof(GXDLMSMeter).Assembly.Location), ImageUpdate + ".exe");
                        bool init = i.ImageTransferStatus == ImageTransferStatus.TransferInitiated;
                        if (init)
                        {
                            i.ImageTransferStatus = ImageTransferStatus.VerificationInitiated;
                            //Check that size match.
                            uint size = (uint)new FileInfo(file).Length;
                            if (size != ImageSize)
                            {
                                i.ImageTransferStatus = ImageTransferStatus.VerificationFailed;
                                it.Error = ErrorCode.OtherReason;
                            }
                            else
                            {
                                Thread t = new Thread(() =>
                                {
                                    //Wait 5 seconds before image is verified.
                                    Thread.Sleep(5000);
                                    i.ImageTransferStatus = ImageTransferStatus.VerificationSuccessful;
                                    Console.WriteLine("Image is verificated");
                                });
                                t.Start();
                            }
                        }
                        if (i.ImageTransferStatus != ImageTransferStatus.VerificationFailed &&
                            i.ImageTransferStatus != ImageTransferStatus.VerificationSuccessful)
                        {
                            Console.WriteLine("Image verification is on progress.");
                            it.Error = ErrorCode.TemporaryFailure;
                        }
                    }
                    //Activates the Image.
                    else if (it.Index == 4)
                    {
                        bool init = i.ImageTransferStatus == ImageTransferStatus.VerificationSuccessful;
                        if (init)
                        {
                            i.ImageTransferStatus = ImageTransferStatus.ActivationInitiated;
                            Thread t = new Thread(() =>
                            {
                                //Wait 5 seconds before image is activated.
                                Thread.Sleep(5000);
                                i.ImageTransferStatus = ImageTransferStatus.ActivationSuccessful;
                                Console.WriteLine("Image is activated.");
                            });
                            t.Start();
                        }
                        //Wait 5 seconds before image is verified.
                        if (i.ImageTransferStatus != ImageTransferStatus.ActivationFailed &&
                            i.ImageTransferStatus != ImageTransferStatus.ActivationSuccessful)
                        {
                            Console.WriteLine("Image activation is on progress.");
                            it.Error = ErrorCode.TemporaryFailure;
                        }
                    }
                }
            }
        }

        protected override void PostAction(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                //Image update returns TemporaryFailure if image verify or acticvation is not finished.
                if (it.Error != ErrorCode.Ok && !(it.Target is GXDLMSImageTransfer))
                {
                    // Load default values if user has try to save invalid data.
                    Items.Clear();
                    Items.AddRange(GXDLMSObjectCollection.Load(objectsFile));
                    return;
                }
                // Save value if it's updated with action.
                if (IsChangedWithAction(it.Target.ObjectType, it.Index))
                {
                    GXXmlWriterSettings settings = new GXXmlWriterSettings();
                    Items.Save(objectsFile, settings);
                }
                if (it.Target is GXDLMSSecuritySetup && it.Index == 2)
                {
                    Debug.WriteLine("----------------------------------------------------------");
                    Debug.WriteLine("Updated keys:");
                    Debug.WriteLine("Server System title: " + GXDLMSTranslator.ToHex(Ciphering.SystemTitle));
                    Debug.WriteLine("Authentication key: " + GXDLMSTranslator.ToHex(Ciphering.AuthenticationKey));
                    Debug.WriteLine("Block cipher key: " + GXDLMSTranslator.ToHex(Ciphering.BlockCipherKey));
                    Debug.WriteLine("Client System title: " + GXDLMSTranslator.ToHex(ClientSystemTitle));
                    Debug.WriteLine("Master key (KEK) title: " + GXDLMSTranslator.ToHex(Kek));
                }
            }
        }

        /// <summary>
        /// Our example server accept all connections.
        /// </summary>
        protected override bool IsTarget(int serverAddress, int clientAddress)
        {
            //Only one connection per meter at the time is allowed.
            if (AssignedAssociation != null)
            {
                if (AssignedAssociation.ServerSAP == serverAddress &&
                    AssignedAssociation.ClientSAP != clientAddress)
                {
                    return false;
                }
                AssignedAssociation = null;
            }
            bool ret = false;
            //Check HDLC station address if it's used.
            if (InterfaceType == InterfaceType.HDLC &&
                    Hdlc != null && Hdlc.DeviceAddress != 0)
            {
                ret = Hdlc.DeviceAddress == serverAddress;
            }
            // Check server address using serial number.
            if (!(serverAddress == 0x3FFF || serverAddress == 0x7F ||
                (serverAddress & 0x3FFF) == serialNumber % 10000 + 1000))
            {
                // Find address from the SAP table.
                GXDLMSObjectCollection saps = Items.GetObjects(ObjectType.SapAssignment);
                if (saps.Count != 0)
                {
                    foreach (GXDLMSSapAssignment sap in saps)
                    {
                        if (sap.SapAssignmentList.Count == 0)
                        {
                            ret = true;
                            break;
                        }
                        foreach (KeyValuePair<UInt16, string> e in sap.SapAssignmentList)
                        {
                            // Check server address with two bytes.
                            if ((serverAddress & 0xFFFF0000) == 0 && (serverAddress & 0x7FFF) == e.Key)
                            {
                                ret = true;
                                break;
                            }
                            // Check server address with one byte.
                            if ((serverAddress & 0xFFFFFF00) == 0 && (serverAddress & 0x7F) == e.Key)
                            {
                                ret = true;
                                break;
                            }
                        }
                        if (ret)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //Accept all server addresses if there is no SAP table available.
                    ret = true;
                }
            }
            if (ret)
            {
                AssignedAssociation = null;
                foreach (GXDLMSAssociationLogicalName it in Items.GetObjects(ObjectType.AssociationLogicalName))
                {
                    if (it.ClientSAP == clientAddress)
                    {
                        AssignedAssociation = it;
                        break;
                    }
                }
            }
            return ret;
        }

        protected override AccessMode GetAttributeAccess(ValueEventArgs arg)
        {
            return AssignedAssociation.GetAccess(arg.Target, arg.Index);
        }
        protected override AccessMode3 GetAttributeAccess3(ValueEventArgs arg)
        {
            return AssignedAssociation.GetAccess3(arg.Target, arg.Index);
        }

        /// <summary>
        /// Get method access mode.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Method access mode</returns>
        protected override MethodAccessMode GetMethodAccess(ValueEventArgs arg)
        {
            return AssignedAssociation.GetMethodAccess(arg.Target, arg.Index);
        }

        /// <summary>
        /// Get method access mode.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Method access mode</returns>
        protected override MethodAccessMode3 GetMethodAccess3(ValueEventArgs arg)
        {
            return AssignedAssociation.GetMethodAccess3(arg.Target, arg.Index);
        }
        /// <summary>
        /// Check authentication.
        /// </summary>
        protected override SourceDiagnostic ValidateAuthentication(Authentication authentication, byte[] password)
        {
            if (UseLogicalNameReferencing)
            {
                if (AssignedAssociation != null)
                {
                    if (AssignedAssociation.AuthenticationMechanismName.MechanismId != authentication)
                    {
                        if (authentication == Authentication.None)
                        {
                            return SourceDiagnostic.AuthenticationRequired;
                        }
                        return SourceDiagnostic.AuthenticationFailure;
                    }
                    if (authentication != Authentication.Low)
                    {
                        // Other authentication levels are check later.
                        return SourceDiagnostic.None;
                    }
                    if (GXCommon.EqualBytes(AssignedAssociation.Secret, password))
                    {
                        return SourceDiagnostic.None;
                    }
                    Debug.WriteLine("Invalid password. Expected: " + GXCommon.ToHex(AssignedAssociation.Secret) +
                        " Actual: " + GXCommon.ToHex(password));
                }
            }
            return SourceDiagnostic.AuthenticationFailure;
        }

        /// <summary>
        /// All objects are static in our example.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="sn"></param>
        /// <param name="ln"></param>
        /// <returns></returns>
        protected override GXDLMSObject FindObject(ObjectType objectType, int sn, string ln)
        {
            if (objectType == ObjectType.AssociationLogicalName)
            {
                foreach (GXDLMSObject it in Items)
                {
                    if (it.ObjectType == ObjectType.AssociationLogicalName)
                    {
                        GXDLMSAssociationLogicalName a = (GXDLMSAssociationLogicalName)it;
                        if (a.ClientSAP == Settings.ClientAddress
                                && a.AuthenticationMechanismName.MechanismId == Settings.Authentication
                                && (ln == a.LogicalName || ln == "0.0.40.0.0.255"))
                        {
                            return it;
                        }
                    }
                }
            }
            // Find object from the active association view.
            else if (AssignedAssociation != null)
            {
                return AssignedAssociation.ObjectList.FindByLN(objectType, ln);
            }
            return null;
        }


        /// <summary>
        /// Client has close connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnClientDisconnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            //Show trace only for one meter.
            if (Trace > TraceLevel.Warning)
            {
                Console.WriteLine("TCP/IP connection closed.");
            }
            //Clear the buffer.
            if (buffers.ContainsKey(e.Info))
            {
                buffers[e.Info].Clear();
                buffers.Remove(e.Info);
            }
            if (connections.ContainsKey(e.Info))
            {
                connections[e.Info].Reset();
                connections.Remove(e.Info);
            }
        }

        /// <summary>
        /// Client has made connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnClientConnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            //Show trace only for one meter.
            if (Trace > TraceLevel.Warning)
            {
                Console.WriteLine("TCP/IP connection established.");
            }
            //Clear the buffer.
            if (buffers.ContainsKey(e.Info))
            {
                buffers[e.Info].Clear();
            }
            if (connections.ContainsKey(e.Info))
            {
                connections[e.Info].Reset();
                connections.Remove(e.Info);
            }
        }

        /// <summary>
        /// Client has send data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnReceived(object sender, Gurux.Common.ReceiveEventArgs e)
        {
            try
            {
                lock (this)
                {
                    //Show trace only for connected meters.
                    if (Trace > TraceLevel.Info && this.ConnectionState != ConnectionState.None)
                    {
                        Console.WriteLine("RX:\t" + Gurux.Common.GXCommon.ToHex((byte[])e.Data, true));
                    }
                    GXServerReply sr = new GXServerReply((byte[])e.Data);
                    sr.ConnectionInfo = new GXDLMSConnectionEventArgs() { ConnectionInfo = e.SenderInfo };
                    do
                    {
                        HandleRequest(sr);
                        //Reply is null if we do not want to send any data to the client.
                        //This is done if client try to make connection with wrong device ID.
                        if (sr.Reply != null)
                        {
                            Media.Send(sr.Reply, e.SenderInfo);
                            if (Trace > TraceLevel.Info)
                            {
                                Console.WriteLine("TX:\t" + Gurux.Common.GXCommon.ToHex(sr.Reply, true));
                            }
                            if ((Media is GXSerial serial) && sr.NewBaudRate != 0)
                            {
                                if (ConnectionState == ConnectionState.Iec)
                                {
                                    serial.BaudRate = sr.NewBaudRate;
                                    serial.DataBits = 8;
                                    serial.Parity = Parity.None;
                                    serial.StopBits = StopBits.One;
                                    Console.WriteLine("Connected with optical probe. The new baudrate is: " + serial.BaudRate);
                                }
                                else
                                {
                                    Thread.Sleep(200);
                                    //Reset optical probe default settings.
                                    serial.BaudRate = sr.NewBaudRate;
                                    serial.DataBits = 7;
                                    serial.Parity = Parity.Even;
                                    serial.StopBits = StopBits.One;
                                    Console.WriteLine("Disconnected with optical probe. The new baudrate is: " + serial.BaudRate);
                                }
                            }
                            sr.Data = null;
                        }
                    }
                    while (sr.IsStreaming);
                }
            }
            catch (Exception ex)
            {
                if (!(ex is SocketException))
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public override void PreGet(ValueEventArgs[] args)
        {

        }

        public override void PostGet(ValueEventArgs[] args)
        {
        }

        /// <summary>
        /// Execute selected actions
        /// </summary>
        /// <param name="actions">List of actions to execute.</param>
        protected override void Execute(List<KeyValuePair<GXDLMSObject, int>> actions)
        {
            foreach (var it in actions)
            {
                Console.WriteLine(DateTime.Now + " Executing: " + it.Key.ObjectType + " " + it.Key);
            }
        }

        protected override void Connected(GXDLMSConnectionEventArgs e)
        {
            if (e != null && e.ConnectionInfo != null)
            {
                if (connections.ContainsKey(e.ConnectionInfo))
                {
                    connections[e.ConnectionInfo].Reset();
                }
                connections[e.ConnectionInfo] = this;
            }
            if (Trace > TraceLevel.Warning)
            {
                Console.WriteLine("Client Connected.");
            }
        }

        protected override void Disconnected(GXDLMSConnectionEventArgs e)
        {
            if (Trace > TraceLevel.Warning && this.ConnectionState != ConnectionState.None)
            {
                Console.WriteLine("Client Disconnected");
            }
            if (e != null && e.ConnectionInfo != null)
            {
                if (connections.ContainsKey(e.ConnectionInfo))
                {
                    connections.Remove(e.ConnectionInfo);
                }
            }
        }
    }
}
