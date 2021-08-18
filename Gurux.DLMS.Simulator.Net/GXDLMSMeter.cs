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

namespace Gurux.DLMS.Simulator.Net
{
    /// <summary>
    /// Simulated meter.
    /// </summary>
    class GXDLMSMeter : GXDLMSSecureServer
    {
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
        public GXDLMSMeter(bool logicalNameReferencing, InterfaceType type) : base(logicalNameReferencing, type)
        {
            interfaceType = type;
        }
        public void Initialize(IGXMedia media, TraceLevel trace, string path, UInt32 sn, bool exclusive)
        {
            serialNumber = sn;
            objectsFile = path;
            Media = media;
            Trace = trace;
            Exclusive = exclusive;
            Init(exclusive);
        }

        /// <summary>
        /// Save COSEM objects to XML.
        /// </summary>
        /// <param name="path">File path.</param>
        void SaveObjects(string path)
        {
            lock (settingsLock)
            {
                GXXmlWriterSettings settings = new Objects.GXXmlWriterSettings();
                Items.Save(path, settings);
            }
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



        /// <summary>
        /// Client has send data for the meters that are using the same port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnExclusiveReceived(object sender, Gurux.Common.ReceiveEventArgs e)
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
                    int target, source;
                    //All simulated meters are using the same interface type.
                    GXDLMSTranslator.GetAddressInfo(interfaceType, bb, out target, out source);
                    if (target != 0 && meters.ContainsKey(target))
                    {
                        GXDLMSMeter m = meters[target];
                        if (Trace > TraceLevel.Info)
                        {
                            Console.WriteLine("RX:\t" + Gurux.Common.GXCommon.ToHex((byte[])e.Data, true));
                        }
                        GXServerReply sr = new GXServerReply(bb.Data);
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

        bool Init(bool exclusive)
        {
            //Load added objects.
            if (!LoadObjects(objectsFile, Items))
            {
                throw new Exception(string.Format("Invalid device template file {0}", objectsFile));
            }
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
                    c.Time = c.Now();
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

        protected override void PreAction(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                if (Trace > TraceLevel.Warning)
                {
                    System.Diagnostics.Debug.WriteLine("PreAction {0}:{1}", it.Target.LogicalName, it.Index);
                }
            }
        }

        protected override void PostAction(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                if (it.Error != ErrorCode.Ok)
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
            bool ret = false;
            AssignedAssociation = null;
            foreach (GXDLMSAssociationLogicalName it in Items.GetObjects(ObjectType.AssociationLogicalName))
            {
                if (it.ClientSAP == clientAddress)
                {
                    AssignedAssociation = it;
                    break;
                }
            }
            if (AssignedAssociation != null)
            {
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
                                return true;
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
            }
            return ret;
        }

        protected override AccessMode GetAttributeAccess(ValueEventArgs arg)
        {
            return AssignedAssociation.GetAccess(arg.Target, arg.Index);
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
//                    if (Trace > TraceLevel.Info && this.ConnectionState != ConnectionState.None)
                    {
                        Console.WriteLine("RX:\t" + Gurux.Common.GXCommon.ToHex((byte[])e.Data, true));
                    }
                    GXServerReply sr = new GXServerReply((byte[])e.Data);
                    do
                    {
                        HandleRequest(sr);
                        //Reply is null if we do not want to send any data to the client.
                        //This is done if client try to make connection with wrong device ID.
                        if (sr.Reply != null)
                        {
                            if (Trace > TraceLevel.Info)
                            {
                                Console.WriteLine("TX:\t" + Gurux.Common.GXCommon.ToHex(sr.Reply, true));
                            }
                            Media.Send(sr.Reply, e.SenderInfo);
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
        }
    }
}
