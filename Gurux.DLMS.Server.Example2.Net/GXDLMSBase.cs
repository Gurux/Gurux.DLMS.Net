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
using Gurux.DLMS.Objects;
using Gurux.Net;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;
using System.IO;
using System.Globalization;

namespace Gurux.DLMS.Server.Example2.Net
{
    /// <summary>
    /// All example servers are using same objects.
    /// </summary>
    class GXDLMSBase : GXDLMSSecureServer
    {
        GXBatteryUseTimeCounter batteryUseTimeCounter;
        bool trace = true;
        /// <summary>
        /// Lock settings file when used.
        /// </summary>
        private object settingsLock = new object();

        //Temperature register.
        private GXDLMSRegister temperature;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical name settings.</param>
        /// <param name="hdlc">HDLC settings.</param>
        public GXDLMSBase(GXDLMSAssociationLogicalName ln, GXDLMSHdlcSetup hdlc)
        : base(ln, hdlc)
        {
            MaxReceivePDUSize = 1024;
            //Default secret.
            ln.Secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="hdlc">HDLC settings.</param>
        public GXDLMSBase(GXDLMSAssociationShortName sn, GXDLMSHdlcSetup hdlc)
        : base(sn, hdlc, "GRX", 12345678)
        {
            MaxReceivePDUSize = 1024;
            //Default secret.
            sn.Secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical name settings.</param>
        /// <param name="wrapper">Wrapper settings.</param>
        public GXDLMSBase(GXDLMSAssociationLogicalName ln, GXDLMSTcpUdpSetup wrapper)
        : base(ln, wrapper, "GRX", 12345678)
        {
            MaxReceivePDUSize = 1024;
            //Default secret.
            ln.Secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="wrapper">Wrapper settings.</param>
        public GXDLMSBase(GXDLMSAssociationShortName sn, GXDLMSTcpUdpSetup wrapper)
        : base(sn, wrapper, "GRX", 12345678)
        {
            MaxReceivePDUSize = 1024;
            //Default secret.
            sn.Secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
        }

        Gurux.Common.IGXMedia Media = null;

        public void Initialize(string port)
        {
            Media = new Gurux.Serial.GXSerial(port);
            Init();
        }

        /// <summary>
        /// Generic initialize for all servers.
        /// </summary>
        /// <param name="server"></param>
        public void Initialize(int port)
        {
            Media = new GXNet(NetworkType.Tcp, port);
            Init();
        }

        /// <summary>
        /// Get temperature. This value is retrieve only when needed.
        /// </summary>
        private void UpdateTemperature()
        {
            //Random value is used here because Windows is not supporting this very well.
            temperature.Value = new Random().Next(-40, 25);
        }

        /// <summary>
        /// Save COSEM objects to XML.
        /// </summary>
        /// <param name="path">File path.</param>
        void SaveObjects(string path)
        {
            lock (settingsLock)
            {
                GXXmlWriterSettings settings = new Objects.GXXmlWriterSettings(); ;
                Items.Save(path, settings);
            }
        }

        /// <summary>
        /// Load saved COSEM objects from XML.
        /// </summary>
        /// <param name="path">File path.</param>
        void LoadObjects(string path)
        {
            lock (settingsLock)
            {
                if (File.Exists(path))
                {
                    GXDLMSObjectCollection objects = GXDLMSObjectCollection.Load(path);
                    Items.Clear();
                    Items.AddRange(objects);
                }
            }
        }

        /// <summary>
        /// In this example we have only two register objects. Battery use time counter that can be reset and CPU temperature.
        /// Battery use time counter is increased from the own thread.
        /// </summary>
        void Init()
        {
            Media.OnReceived += new Gurux.Common.ReceivedEventHandler(OnReceived);
            Media.OnClientConnected += new Gurux.Common.ClientConnectedEventHandler(OnClientConnected);
            Media.OnClientDisconnected += new Gurux.Common.ClientDisconnectedEventHandler(OnClientDisconnected);
            Media.OnError += new Gurux.Common.ErrorEventHandler(OnError);
            Media.Open();
            //Load added objects.
            LoadSettings();

            temperature = this.Items.FindByLN(ObjectType.Register, "0.0.96.9.0.255") as GXDLMSRegister;
            if (temperature == null)
            {
                // CPU temperature.
                temperature = new GXDLMSRegister("0.0.96.9.0.255");
                temperature.Scaler = 1;
                temperature.Unit = Unit.Temperature;
                temperature.Value = 100;
                temperature.SetDataType(2, DataType.Int8);
                this.Items.Add(temperature);
            }
            // Battery use time counter
            GXDLMSRegister r = this.Items.FindByLN(ObjectType.Register, "0.0.96.6.0.255") as GXDLMSRegister;
            if (r == null)
            {
                r = new GXDLMSRegister("0.0.96.6.0.255");
                r.SetDataType(2, DataType.UInt16);
                this.Items.Add(r);
            }
            batteryUseTimeCounter = new GXBatteryUseTimeCounter(r);
            batteryUseTimeCounter.Start();
            ///////////////////////////////////////////////////////////////////////
            //Server must initialize after all objects are added.
            Initialize();
        }

        public override void Close()
        {
            base.Close();
            if (Media != null)
            {
                Media.Close();
            }
            if (batteryUseTimeCounter != null)
            {
                batteryUseTimeCounter.Stop();
                batteryUseTimeCounter = null;
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
        /// Return data using start and end indexes.
        /// </summary>
        /// <param name="p">ProfileGeneric</param>
        /// <param name="index">Row index.</param>
        /// <param name="count">Row count.</param>
        void GetProfileGenericDataByEntry(GXDLMSProfileGeneric p, UInt32 index, UInt32 count)
        {
            //Clear old data. It's already serialized.
            p.Buffer.Clear();
            string name = GetProfileGenericName(p);
            if (count != 0)
            {
                lock (p)
                {
                    if (!File.Exists(name))
                    {
                        return;
                    }

                    using (var fs = File.OpenRead(name))
                    {
                        using (var reader = new StreamReader(fs))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                if (line.Length != 0)
                                {
                                    //Skip row
                                    if (index > 0)
                                    {
                                        --index;
                                    }
                                    else
                                    {
                                        string[] values = line.Split(';');
                                        object[] list = new object[values.Length];
                                        for (int pos = 0; pos != values.Length; ++pos)
                                        {
                                            DataType t = p.CaptureObjects[pos].Key.GetUIDataType(p.CaptureObjects[pos].Value.AttributeIndex);
                                            if (t == DataType.DateTime)
                                            {
                                                list[pos] = new GXDateTime(values[pos]);
                                            }
                                            else if (t == DataType.Date)
                                            {
                                                list[pos] = new GXDate(values[pos]);
                                            }
                                            else if (t == DataType.Time)
                                            {
                                                list[pos] = new GXTime(values[pos]);
                                            }
                                            else
                                            {
                                                list[pos] = values[pos];
                                            }
                                        }
                                        p.Buffer.Add(list);
                                    }
                                    if (p.Buffer.Count == count)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find start index and row count using start and end date time.
        /// </summary>
        /// <param name="e">Value arguments.</param>
        void GetProfileGenericDataByRange(ValueEventArgs e)
        {
            GXDateTime start = (GXDateTime)GXDLMSClient.ChangeType((byte[])((object[])e.Parameters)[1], DataType.DateTime);
            GXDateTime end = (GXDateTime)GXDLMSClient.ChangeType((byte[])((object[])e.Parameters)[2], DataType.DateTime);
            string name = GetProfileGenericName(e.Target);
            lock (e.Target)
            {
                if (!File.Exists(name))
                {
                    using (var fs = File.CreateText(name))
                    {
                        return;
                    }
                }
                using (var fs = File.OpenRead(name))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (line.Length != 0)
                            {
                                string[] values = line.Split(';');
                                DateTime tm = new GXDateTime(values[0]);
                                if (tm > end)
                                {
                                    //If all data is read.
                                    break;
                                }
                                if (tm < start)
                                {
                                    //If we have not find first item.
                                    ++e.RowBeginIndex;
                                }
                                ++e.RowEndIndex;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get row count.
        /// </summary>
        /// <returns></returns>
        UInt16 GetProfileGenericDataCount(GXDLMSProfileGeneric p)
        {
            string name = GetProfileGenericName(p);
            UInt16 rows = 0;
            lock (p)
            {
                if (File.Exists(name))
                {
                    using (var fs = File.OpenRead(name))
                    {
                        using (var reader = new StreamReader(fs))
                        {
                            while (!reader.EndOfStream)
                            {
                                if (reader.ReadLine().Length != 0)
                                {
                                    ++rows;
                                }
                            }
                        }
                    }
                }
            }
            return rows;
        }

        /// <summary>
        /// Generic read handle for all servers.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="e"></param>
        protected override void PreRead(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs e in args)
            {
                //If user wants to know CPU temperature.
                if (e.Target == temperature && e.Index == 2)
                {
                    UpdateTemperature();
                }
                //Read only size of PDU at the memory at once.
                else if (e.Target is GXDLMSProfileGeneric)
                {
                    //If buffer is read and we want to save memory.
                    if (e.Index == 6)
                    {
                        //If client wants to know EntriesInUse.
                        GXDLMSProfileGeneric p = (GXDLMSProfileGeneric)e.Target;
                        p.EntriesInUse = GetProfileGenericDataCount(p);
                    }
                    if (e.Index == 2)
                    {
                        //Client reads buffer.
                        GXDLMSProfileGeneric p = (GXDLMSProfileGeneric)e.Target;
                        //If reading first time.
                        if (e.RowEndIndex == 0)
                        {
                            if (e.Selector == 0)
                            {
                                e.RowEndIndex = GetProfileGenericDataCount(p);
                            }
                            else if (e.Selector == 1)
                            {
                                //Read by entry.
                                GetProfileGenericDataByRange(e);
                            }
                            else if (e.Selector == 2)
                            {
                                //Read by range.
                                e.RowBeginIndex = (UInt32)((object[])e.Parameters)[0];
                                e.RowEndIndex = e.RowBeginIndex + (UInt32)((object[])e.Parameters)[1];
                                //If client wants to read more data what we have.
                                UInt16 cnt = GetProfileGenericDataCount(p);
                                if (e.RowEndIndex - e.RowBeginIndex > cnt - e.RowBeginIndex)
                                {
                                    e.RowEndIndex = cnt - e.RowBeginIndex;
                                    if (e.RowEndIndex < 0)
                                    {
                                        e.RowEndIndex = 0;
                                    }
                                }
                            }
                        }
                        UInt32 count = e.RowEndIndex - e.RowBeginIndex;
                        //Read only rows that can fit to one PDU.
                        if (e.RowToPdu != 0 && e.RowEndIndex - e.RowBeginIndex > e.RowToPdu)
                        {
                            count = e.RowToPdu;
                        }
                        GetProfileGenericDataByEntry(p, e.RowBeginIndex, count);
                    }
                }
            }
        }

        protected override void PostRead(ValueEventArgs[] args)
        {

        }

        protected override void PreWrite(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                if (it.Target is GXDLMSProfileGeneric)
                {
                    //We do not want that buffer can be write here.
                    if (it.Index == 2)
                    {
                        it.Error = ErrorCode.ReadWriteDenied;
                    }
                    else if (it.Index == 3)
                    {
                        //If user is updating capture objects.
                        //We are expecting that first column is always clock object's date time.
                        GXDLMSProfileGeneric pg = it.Target as GXDLMSProfileGeneric;
                        List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> list = GXDLMSProfileGeneric.GetCaptureObjects(it.Value as object[]);
                        if (list.Count != 0 && !(list[0].Key is GXDLMSClock && list[0].Value.AttributeIndex == 2))
                        {
                            it.Error = ErrorCode.ReadWriteDenied;
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine("Writing " + it.Target.LogicalName);
            }
        }

        /// <summary>
        /// Load meter settings.
        /// </summary>
        private void LoadSettings()
        {
            if (UseLogicalNameReferencing)
            {
                LoadObjects("AssociationLogicalName.xml");
            }
            else
            {
                LoadObjects("AssociationShortName.xml");
            }
        }

        /// <summary>
        /// Save meter settings.
        /// </summary>
        private void SaveSettings()
        {
            if (UseLogicalNameReferencing)
            {
                SaveObjects("AssociationLogicalName.xml");
            }
            else
            {
                SaveObjects("AssociationShortName.xml");
            }
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

        void SendPush(GXDLMSPushSetup target)
        {
            int pos = target.Destination.IndexOf(':');
            if (pos == -1)
            {
                throw new ArgumentException("Invalid destination.");
            }
            GXDLMSNotify notify = new GXDLMSNotify(true, 1, 1, InterfaceType.WRAPPER);
            byte[][] data = notify.GeneratePushSetupMessages(DateTime.MinValue, target);
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
                System.Diagnostics.Debug.WriteLine("Action " + it.Target.LogicalName);
                if (it.Target is GXDLMSPushSetup && it.Index == 1)
                {
                    SendPush(it.Target as GXDLMSPushSetup);
                    it.Handled = true;
                }
            }
        }

        /// <summary>
        /// Remove rows from the file.
        /// </summary>
        /// <param name="name">File name.</param>
        /// <param name="count">Amount of removed rows.</param>
        private void RemoveRows(string name, int count)
        {
            using (FileStream fs = File.Open(name, FileMode.Open, FileAccess.ReadWrite))
            {
                //Find end position of the first row.
                int ch = 0;
                while (fs.Position != fs.Length && count > 0)
                {
                    ch = fs.ReadByte();
                    if (ch == '\n' || ch == '\r')
                    {
                        while (ch == '\n' || ch == '\r')
                        {
                            ch = fs.ReadByte();
                        }
                        --fs.Position;
                        --count;
                    }
                }
                int offset = (int)fs.Position;
                byte[] row = new byte[1024];
                int cnt;
                while ((cnt = fs.Read(row, 0, row.Length)) != 0)
                {
                    fs.Seek(-offset - cnt, SeekOrigin.Current);
                    fs.Write(row, 0, cnt);
                    fs.Seek(offset, SeekOrigin.Current);
                }
                fs.SetLength(fs.Position - offset);
            }
        }

        private void HandleProfileGenericActions(ValueEventArgs it)
        {
            GXDLMSProfileGeneric pg = (GXDLMSProfileGeneric)it.Target;
            string name = GetProfileGenericName(pg);
            if (it.Index == 1)
            {
                //Profile generic clear is called. Clear data.
                lock (pg)
                {
                    using (var fs = File.CreateText(name))
                    {
                    }
                }
            }
            else if (it.Index == 2)
            {
            }
        }

        protected override void PostAction(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                if (it.Target is GXDLMSProfileGeneric)
                {
                    HandleProfileGenericActions(it);
                }
            }
        }

        /// <summary>
        /// Our example server accept all connections.
        /// </summary>
        protected override bool IsTarget(int serverAddress, int clientAddress)
        {
            return true;
        }

        protected override AccessMode GetAttributeAccess(ValueEventArgs arg)
        {
            //Only read is allowed for register.
            if (arg.Target is GXDLMSRegister)
            {
                return AccessMode.Read;
            }
            //Only read is allowed
            if (arg.Settings.Authentication == Authentication.None)
            {
                return AccessMode.Read;
            }
            //Only clock write is allowed.
            if (arg.Settings.Authentication == Authentication.Low)
            {
                if (arg.Target is GXDLMSClock)
                {
                    return AccessMode.ReadWrite;
                }
                return AccessMode.Read;
            }
            //All write are allowed.
            return AccessMode.ReadWrite;
        }

        /// <summary>
        /// Get method access mode.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Method access mode</returns>
        protected override MethodAccessMode GetMethodAccess(ValueEventArgs arg)
        {
            //Temperature can't reset.
            if (arg.Target == temperature)
            {
                return MethodAccessMode.NoAccess;
            }

            //Methods are not allowed.
            if (arg.Settings.Authentication == Authentication.None)
            {
                return MethodAccessMode.NoAccess;
            }
            //Only clock methods are allowed.
            if (arg.Settings.Authentication == Authentication.Low)
            {
                if (arg.Target is GXDLMSClock)
                {
                    return MethodAccessMode.Access;
                }
                return MethodAccessMode.NoAccess;
            }
            return MethodAccessMode.Access;
        }


        /// <summary>
        /// Our example server accept all authentications.
        /// </summary>
        protected override SourceDiagnostic ValidateAuthentication(Authentication authentication, byte[] password)
        {
            //If low authentication fails.
            if (authentication == Authentication.Low)
            {
                byte[] expected;
                if (UseLogicalNameReferencing)
                {
                    GXDLMSAssociationLogicalName ln = (GXDLMSAssociationLogicalName)Items.FindByLN(ObjectType.AssociationLogicalName, "0.0.40.0.0.255");
                    expected = ln.Secret;
                }
                else
                {
                    GXDLMSAssociationShortName sn = (GXDLMSAssociationShortName)Items.FindByLN(ObjectType.AssociationShortName, "0.0.40.0.0.255");
                    expected = sn.Secret;
                }
                if (Gurux.Common.GXCommon.EqualBytes(expected, password))
                {
                    return SourceDiagnostic.None;
                }
                return SourceDiagnostic.AuthenticationFailure;
            }
            //Other authentication levels are check later.
            return SourceDiagnostic.None;
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
            return null;
        }


        /// <summary>
        /// Client has close connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClientDisconnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            //Reset server settings when connection closed.
            this.Reset();
            Console.WriteLine("Client Disconnected.");
        }

        /// <summary>
        /// Client has made connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClientConnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            Console.WriteLine("Client Connected.");
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
                    if (trace)
                    {
                        Console.WriteLine("<- " + Gurux.Common.GXCommon.ToHex((byte[])e.Data, true));
                    }
                    byte[] reply = HandleRequest((byte[])e.Data);
                    //Reply is null if we do not want to send any data to the client.
                    //This is done if client try to make connection with wrong device ID.
                    if (reply != null)
                    {
                        if (trace)
                        {
                            Console.WriteLine("-> " + Gurux.Common.GXCommon.ToHex(reply, true));
                        }
                        Media.Send(reply, e.SenderInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Generate random value for profile generic.
        /// </summary>
        /// <param name="args"></param>
        public override void PreGet(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs e in args)
            {
                if (e.Target is GXDLMSProfileGeneric)
                {
                    //We want to save values to the file right a way.
                    GXDLMSProfileGeneric pg = (GXDLMSProfileGeneric)e.Target;
                    //Get entries in use if not know yet.
                    if (pg.EntriesInUse == 0)
                    {
                        pg.EntriesInUse = GetProfileGenericDataCount(pg);
                    }
                    string name = GetProfileGenericName(pg);
                    object[] values = new object[pg.CaptureObjects.Count];
                    int pos = 0;
                    lock (pg)
                    {
                        foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in pg.CaptureObjects)
                        {
                            if (it.Key is GXDLMSClock && it.Value.AttributeIndex == 2)
                            {
                                GXDLMSClock c = (it.Key as GXDLMSClock);
                                c.Time = c.Now();
                            }
                            else if (it.Key == temperature && it.Value.AttributeIndex == 2)
                            {
                                //Get CPU temperature.
                                UpdateTemperature();
                            }
                            values[pos] = it.Key.GetValues()[it.Value.AttributeIndex - 1];
                            ++pos;
                        }
                        ++pg.EntriesInUse;
                        //Remove first row if maximum row count is received.
                        if (pg.ProfileEntries != 0 && pg.EntriesInUse >= pg.ProfileEntries)
                        {
                            RemoveRows(name, pg.EntriesInUse - pg.ProfileEntries);
                            pg.EntriesInUse = pg.ProfileEntries;
                        }
                        using (var writer = File.AppendText(name))
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int c = 0; c != values.Length; ++c)
                            {
                                if (c != 0)
                                {
                                    sb.Append(';');
                                }
                                object col = values[c];
                                if (col is DateTime)
                                {
                                    sb.Append(((DateTime)col).ToString(CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    sb.Append(Convert.ToString(col));
                                }
                            }
                            sb.AppendLine("");
                            writer.Write(sb.ToString());
                        }
                    }
                    e.Handled = true;
                }
            }
        }

        public override void PostGet(ValueEventArgs[] args)
        {
        }

        protected override void Connected(GXDLMSConnectionEventArgs e)
        {
            Console.WriteLine("Connected.");
        }

        protected override void Disconnected(GXDLMSConnectionEventArgs e)
        {
            Console.WriteLine("Disconnected");
        }
    }
}
