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

namespace Gurux.DLMS.Server.Example2.Net
{
    /// <summary>
    /// Simulated meter.
    /// </summary>
    class GXDLMSMeter : GXDLMSSecureServer
    {
        //Are all meters using the same port.
        bool Exclusive;
        string objectsFile;
        TraceLevel Trace = TraceLevel.Error;
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
        }
        public void Initialize(IGXMedia media, TraceLevel trace, string path, UInt32 sn, bool exclusive)
        {
            serialNumber = sn;
            objectsFile = path;
            Media = media;
            Trace = trace;
            Exclusive = exclusive;
            Init();
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

        bool Init()
        {
            //Load added objects.
            if (!LoadSettings())
            {
                throw new Exception(string.Format("Invalid device template file {0}", objectsFile));
            }
            Media.OnReceived += new Gurux.Common.ReceivedEventHandler(OnReceived);
            Media.OnClientConnected += new Gurux.Common.ClientConnectedEventHandler(OnClientConnected);
            Media.OnClientDisconnected += new Gurux.Common.ClientDisconnectedEventHandler(OnClientDisconnected);
            Media.OnError += new Gurux.Common.ErrorEventHandler(OnError);
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

        void OnError(object sender, Exception ex)
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

        /// <summary>
        /// Load meter settings.
        /// </summary>
        private bool LoadSettings()
        {
            return LoadObjects(objectsFile, Items);
        }

        /// <summary>
        /// Save meter settings.
        /// </summary>
        private void SaveSettings()
        {
            SaveObjects(objectsFile);
        }

        private void UpdateAssociationView(GXDLMSObjectCollection collection)
        {
            //Add new items.
            bool newItems = false;
            foreach (GXDLMSObject it in collection)
            {
                if (!Items.Contains(it))
                {
                    Items.Add(it);
                    newItems = true;
                }
            }
            //Remove old items.
            List<GXDLMSObject> removed = new List<GXDLMSObject>();
            foreach (GXDLMSObject it in Items)
            {
                if (!collection.Contains(it))
                {
                    removed.Add(it);
                }
            }
            foreach (GXDLMSObject it in removed)
            {
                Items.Remove(it);
            }
            //Update short names if new items are added.
            if (newItems && !UseLogicalNameReferencing)
            {
                UpdateShortNames();
            }

            //Save added objects.
            SaveSettings();
        }

        protected override void PostWrite(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs e in args)
            {
                //If association is updated.
                if (e.Target is GXDLMSAssociationLogicalName && e.Index == 2)
                {
                    //Update new items to object list.
                    GXDLMSAssociationLogicalName a = (GXDLMSAssociationLogicalName)e.Target;
                    UpdateAssociationView(a.ObjectList);
                }
                //If association is updated.
                else if (e.Target is GXDLMSAssociationShortName && e.Index == 2)
                {
                    //Update new items to object list.
                    GXDLMSAssociationShortName a = (GXDLMSAssociationShortName)e.Target;
                    UpdateAssociationView(a.ObjectList);
                }
                else
                {
                    SaveSettings();
                }
                System.Diagnostics.Debug.WriteLine("Writing " + e.Target.LogicalName);
            }
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
                if (Trace > TraceLevel.Warning)
                {
                    System.Diagnostics.Debug.WriteLine("PostAction {0}:{1}", it.Target.LogicalName, it.Index);
                }
            }
        }

        /// <summary>
        /// Our example server accept all connections.
        /// </summary>
        protected override bool IsTarget(int serverAddress, int clientAddress)
        {
            bool ret = false;
            foreach (GXDLMSAssociationLogicalName it in Items.GetObjects(ObjectType.AssociationLogicalName))
            {
                if (it.ClientSAP == clientAddress)
                {
                    this.MaxReceivePDUSize = it.XDLMSContextInfo.MaxSendPduSize;
                    this.Conformance = it.XDLMSContextInfo.Conformance;
                    ret = true;
                    break;
                }
            }
            if (ret)
            {
                // Check server address using serial number.
                if (!(serverAddress == 0x3FFF || serverAddress == 0x7F ||
                    (serverAddress & 0x3FFF) == serialNumber % 10000 + 1000))
                {
                    // Find address from the SAP table.
                    GXDLMSObjectCollection saps = Items.GetObjects(ObjectType.SapAssignment);
                    if (saps.Count != 0)
                    {
                        ret = false;
                        foreach (GXDLMSObject it in saps)
                        {
                            GXDLMSSapAssignment sap = (GXDLMSSapAssignment)it;
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
                        }
                    }
                    else
                    {
                        ret = serverAddress == 1;
                    }
                }
            }
            return ret;
        }

        protected override AccessMode GetAttributeAccess(ValueEventArgs arg)
        {
            if (this.UseLogicalNameReferencing)
            {
                foreach (GXDLMSAssociationLogicalName it in Items.GetObjects(ObjectType.AssociationLogicalName))
                {
                    if (it.AuthenticationMechanismName.MechanismId == Authentication)
                    {
                        return it.ObjectList.FindByLN(arg.Target.ObjectType, arg.Target.LogicalName).GetAccess(arg.Index);
                    }
                }
            }
            return AccessMode.NoAccess;
        }

        /// <summary>
        /// Get method access mode.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Method access mode</returns>
        protected override MethodAccessMode GetMethodAccess(ValueEventArgs arg)
        {
            if (this.UseLogicalNameReferencing)
            {
                foreach (GXDLMSAssociationLogicalName it in Items.GetObjects(ObjectType.AssociationLogicalName))
                {
                    if (it.AuthenticationMechanismName.MechanismId == Authentication)
                    {
                        return it.ObjectList.FindByLN(arg.Target.ObjectType, arg.Target.LogicalName).GetMethodAccess(arg.Index);
                    }
                }
            }
            return MethodAccessMode.NoAccess;
        }

        /// <summary>
        /// Check authentication.
        /// </summary>
        protected override SourceDiagnostic ValidateAuthentication(Authentication authentication, byte[] password)
        {
            if (this.UseLogicalNameReferencing)
            {
                GXDLMSAssociationLogicalName target = null;
                foreach (GXDLMSAssociationLogicalName it in Items.GetObjects(ObjectType.AssociationLogicalName))
                {
                    if (it.AuthenticationMechanismName.MechanismId == authentication)
                    {
                        target = it;
                        break;
                    }
                }
                if (target != null)
                {
                    if (authentication != Authentication.Low)
                    {
                        //Other authentication levels are check later.
                        return SourceDiagnostic.None;
                    }
                    if (GXCommon.EqualBytes(target.Secret, password))
                    {
                        return SourceDiagnostic.None;
                    }
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
                foreach (GXDLMSAssociationLogicalName it in Items.GetObjects(ObjectType.AssociationLogicalName))
                {
                    if (it.AuthenticationMechanismName.MechanismId == Authentication)
                    {
                        return it;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Client has close connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClientDisconnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            //Show trace only for one meter.
            if (Trace > TraceLevel.Warning && (!Exclusive || serialNumber == 1))
            {
                Console.WriteLine("Client Disconnected.");
            }
        }

        /// <summary>
        /// Client has made connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClientConnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            //Show trace only for one meter.
            if (Trace > TraceLevel.Warning && (!Exclusive || serialNumber == 1))
            {
                Console.WriteLine("Client Connected.");
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

        /// <summary>
        /// Generate random value for profile generic.
        /// </summary>
        /// <param name="args"></param>
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
                Console.WriteLine("Connected.");
            }
        }

        protected override void Disconnected(GXDLMSConnectionEventArgs e)
        {
            //Show trace only for one meter.
            if (Trace > TraceLevel.Warning && (!Exclusive || serialNumber == 1))
            {
                Console.WriteLine("Disconnected");
            }
        }
    }
}
