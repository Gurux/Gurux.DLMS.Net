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
// More information of Gurux products: https://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Internal;
#if !WINDOWS_UWP
using System.Security.Cryptography;
#endif
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;
using System.Diagnostics;
using Gurux.DLMS.Objects.Enums;
using System.Threading;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMSServer implements methods to implement DLMS/COSEM meter/proxy.
    /// </summary>
    public abstract class GXDLMSServer
    {
        /// <summary>
        /// DLMS settings.
        /// </summary>
        public GXDLMSSettings Settings
        {
            get;
            private set;
        }

        EventWaitHandle waiting = null;

        /// <summary>
        /// List of times when last objects are executed last time.
        /// </summary>
        public Dictionary<GXDLMSObject, DateTime> ExecutionTimes;


        private readonly GXReplyData info = new GXReplyData();
        /// <summary>
        /// Received data.
        /// </summary>
        private GXByteBuffer receivedData = new GXByteBuffer();
        /// <summary>
        /// Reply data.
        /// </summary>
        private GXByteBuffer replyData = new GXByteBuffer();


        /// <summary>
        /// Client system title.
        /// </summary>
        /// <remarks>
        /// Client system title is optional and it's used when Pre-established Application Associations is used.
        /// </remarks>
        public byte[] ClientSystemTitle
        {
            get
            {
                return Settings.PreEstablishedSystemTitle;
            }
            set
            {
                Settings.PreEstablishedSystemTitle = value;
            }
        }

        internal GXDLMSLongTransaction transaction;

        bool Initialized = false;

        /// <summary>
        /// When data was received last time.
        /// </summary>
        DateTime dataReceived = DateTime.MinValue;

        /// <summary>
        /// Find object.
        /// </summary>
        /// <param name="objectType">Object type. In Short Name referencing this is not used.</param>
        /// <param name="sn">Short Name. In Logical name referencing this is not used.</param>
        /// <param name="ln">Logical Name. In Short Name referencing this is not used.</param>
        /// <returns>Found object or null if object is not found.</returns>
        protected abstract GXDLMSObject FindObject(ObjectType objectType, int sn, String ln);

        /// <summary>
        /// Check is data sent to this server.
        /// </summary>
        /// <param name="serverAddress">Server address.</param>
        /// <param name="clientAddress">Client address.</param>
        /// <returns>True, if data is sent to this server.</returns>
        protected abstract bool IsTarget(int serverAddress, int clientAddress);

        /// <summary>
        /// Get attribute access mode.
        /// </summary>
        /// <param name="arg">Value event argument.</param>
        /// <returns>Access mode.</returns>
        protected abstract AccessMode GetAttributeAccess(ValueEventArgs arg);

        /// <summary>
        /// Get method access mode.
        /// </summary>
        /// <param name="arg">Value event argument.</param>
        /// <returns>Method access mode.</returns>
        protected abstract MethodAccessMode GetMethodAccess(ValueEventArgs arg);

        /// <summary>
        /// Check whether the authentication and password are correct.
        /// </summary>
        /// <param name="authentication">Authentication level.</param>
        /// <param name="password">Password</param>
        /// <returns>Source diagnostic.</returns>
        protected abstract SourceDiagnostic ValidateAuthentication(
            Authentication authentication, byte[] password);

        /// <summary>
        /// Accepted connection is made for the server.
        /// </summary>
        /// <remarks>
        /// All initialization is done here.
        /// Example access level of the COSEM objects is good to update here.
        /// </remarks>
        /// <param name="connectionInfo">Connection information.</param>
        protected abstract void Connected(GXDLMSConnectionEventArgs connectionInfo);

        /// <summary>
        /// Client has try to made invalid connection. Password is incorrect.
        /// </summary>
        /// <param name="connectionInfo">Connection information.</param>
        protected abstract void InvalidConnection(GXDLMSConnectionEventArgs connectionInfo);

        /// <summary>
        /// Server has close the connection. All clean up is made here.
        /// </summary>
        /// <param name="connectionInfo">Connection information.</param>
        protected abstract void Disconnected(GXDLMSConnectionEventArgs connectionInfo);

        /// <summary>
        /// Pre get selected value.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        public abstract void PreGet(ValueEventArgs[] args);

        /// <summary>
        /// Post get selected value.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        public abstract void PostGet(ValueEventArgs[] args);

        /// <summary>
        /// Read selected item.
        /// </summary>
        /// <param name="args">Handled read requests.</param>
        protected abstract void PreRead(ValueEventArgs[] args);

        /// <summary>
        /// Write selected item.
        /// </summary>
        /// <param name="args">Handled write requests.</param>
        protected abstract void PreWrite(ValueEventArgs[] args);
        /// <summary>
        /// Action is occurred.
        /// </summary>
        /// <param name="args">Handled action requests.</param>
        protected abstract void PreAction(ValueEventArgs[] args);

        /// <summary>
        /// Read selected item.
        /// </summary>
        /// <param name="args">Handled read requests.</param>
        protected abstract void PostRead(ValueEventArgs[] args);

        /// <summary>
        /// Write selected item.
        /// </summary>
        /// <param name="args">Handled write requests.</param>
        protected abstract void PostWrite(ValueEventArgs[] args);
        /// <summary>
        /// Action is occurred.
        /// </summary>
        /// <param name="args">Handled action requests.</param>
        protected abstract void PostAction(ValueEventArgs[] args);


        /// <summary>
        /// Execute selected actions
        /// </summary>
        /// <param name="actions">List of actions to execute.</param>
        protected abstract void Execute(List<KeyValuePair<GXDLMSObject, int>> actions);

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSServer()
        : this(false, InterfaceType.HDLC)
        {
        }

        /// <summary>
        /// Find object.
        /// </summary>
        /// <param name="objectType">Object type. In Short Name referencing this is not used.</param>
        /// <param name="sn">Short Name. In Logical name referencing this is not used.</param>
        /// <param name="ln">Logical Name. In Short Name referencing this is not used.</param>
        /// <returns>Found object or null if object is not found.</returns>
        internal GXDLMSObject NotifyFindObject(ObjectType objectType, int sn, String ln)
        {
            return FindObject(objectType, sn, ln);
        }

        /// <summary>
        /// Client has try to made invalid connection. Password is incorrect.
        /// </summary>
        /// <param name="connectionInfo">Connection information.</param>
        internal void NotifyInvalidConnection(GXDLMSConnectionEventArgs connectionInfo)
        {
            InvalidConnection(connectionInfo);
        }

        /// <summary>
        /// Get attribute access mode.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Access mode.</returns>
        internal AccessMode NotifyGetAttributeAccess(ValueEventArgs arg)
        {
            if (arg.Index == 1)
            {
                return AccessMode.Read;
            }
            return GetAttributeAccess(arg);
        }

        /// <summary>
        /// Get method access mode.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Method access mode</returns>
        internal MethodAccessMode NotifyGetMethodAccess(ValueEventArgs arg)
        {
            return GetMethodAccess(arg);
        }

        /// <summary>
        /// Read selected item.
        /// </summary>
        /// <param name="args">Handled read requests.</param>
        internal void NotifyRead(ValueEventArgs[] args)
        {
            PreRead(args);
        }

        /// <summary>
        /// Read selected item.
        /// </summary>
        /// <param name="args">Handled read requests.</param>
        internal void NotifyPostRead(ValueEventArgs[] args)
        {
            PostRead(args);
        }

        /// <summary>
        /// Action is occurred.
        /// </summary>
        /// <param name="args">Handled action requests.</param>
        internal void NotifyAction(ValueEventArgs[] args)
        {
            PreAction(args);
        }

        /// <summary>
        /// Action is occurred.
        /// </summary>
        /// <param name="args">Handled action requests.</param>
        internal void NotifyPostAction(ValueEventArgs[] args)
        {
            PostAction(args);
        }

        /// <summary>
        /// Write selected item.
        /// </summary>
        /// <param name="args">Handled write requests.</param>
        internal void NotifyWrite(ValueEventArgs[] args)
        {
            PreWrite(args);
        }

        /// <summary>
        /// Write selected item.
        /// </summary>
        /// <param name="args">Handled write requests.</param>
        internal void NotifyPostWrite(ValueEventArgs[] args)
        {
            PostWrite(args);
        }

        /// <summary>
        /// Notify client from connection.
        /// </summary>
        /// <param name="connectionInfo"></param>
        internal void NotifyConnected(GXDLMSConnectionEventArgs connectionInfo)
        {
            Connected(connectionInfo);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSServer(bool logicalNameReferencing, InterfaceType type)
        {
            Settings = new GXDLMSSettings(true);
            Settings.Objects.Parent = this;
            Settings.ServerAddress = 1;
            Settings.ClientAddress = 16;
            Settings.UseLogicalNameReferencing = logicalNameReferencing;
            Reset();
            InterfaceType = type;
            ExecutionTimes = new Dictionary<GXDLMSObject, DateTime>();
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical name settings.</param>
        /// <param name="type">Interface type.</param>
        public GXDLMSServer(GXDLMSAssociationLogicalName ln, InterfaceType type) : this(true, type)
        {
            Settings.Objects.Add(ln);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="type">Interface type.</param>
        public GXDLMSServer(GXDLMSAssociationShortName sn, InterfaceType type) : this(false, type)
        {
            Settings.Objects.Add(sn);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical name settings.</param>
        /// <param name="hdlc">HDLC settings.</param>
        public GXDLMSServer(GXDLMSAssociationLogicalName ln, GXDLMSHdlcSetup hdlc) : this(true, InterfaceType.HDLC)
        {
            Settings.Objects.Add(ln);
            Settings.Objects.Add(hdlc);
            Hdlc = hdlc;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="hdlc">HDLC settings.</param>
        public GXDLMSServer(GXDLMSAssociationShortName sn, GXDLMSHdlcSetup hdlc) : this(false, InterfaceType.HDLC)
        {
            Settings.Objects.Add(sn);
            Settings.Objects.Add(hdlc);
            Hdlc = hdlc;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical name settings.</param>
        /// <param name="wrapper">WRAPPER settings.</param>
        public GXDLMSServer(GXDLMSAssociationLogicalName ln, GXDLMSTcpUdpSetup wrapper) : this(true, InterfaceType.WRAPPER)
        {
            Settings.Objects.Add(ln);
            Settings.Objects.Add(wrapper);
            Wrapper = wrapper;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="wrapper">WRAPPER settings.</param>
        public GXDLMSServer(GXDLMSAssociationShortName sn, GXDLMSTcpUdpSetup wrapper) : this(false, InterfaceType.WRAPPER)
        {
            Settings.Objects.Add(sn);
            Settings.Objects.Add(wrapper);
            Wrapper = wrapper;
        }

        /// <summary>
        /// List of objects that meter supports.
        /// </summary>
        public GXDLMSObjectCollection Items
        {
            get
            {
                return Settings.Objects;
            }
        }

        /// <summary>
        /// HDLC settings.
        /// </summary>
        public GXDLMSHdlcSetup Hdlc
        {
            get;
            internal set;
        }
        /// <summary>
        /// Wrapper settings.
        /// </summary>
        public GXDLMSTcpUdpSetup Wrapper
        {
            get;
            internal set;
        }

        /// <summary>
        /// GBT window size.
        /// </summary>
        public byte WindowSize
        {
            get
            {
                return Settings.WindowSize;
            }
            set
            {
                Settings.WindowSize = value;
            }
        }

        /// <summary>
        /// Information from the connection size that server can handle.
        /// </summary>
        [Obsolete("Use Hdlc to set limits.")]
        public GXDLMSLimits Limits
        {
            get
            {
                return Settings.Limits;
            }
        }

        /// <summary>
        /// Retrieves the maximum size of PDU receiver.
        /// </summary>
        /// <remarks>
        /// PDU size tells maximum size of PDU packet.
        /// Value can be from 0 to 0xFFFF. By default the value is 0xFFFF.
        /// </remarks>
        /// <seealso cref="UseLogicalNameReferencing"/>
        [DefaultValue(0xFFFF)]
        public ushort MaxReceivePDUSize
        {
            get
            {
                return Settings.MaxServerPDUSize;
            }
            set
            {
                Settings.MaxServerPDUSize = value;
            }
        }

        /// <summary>
        /// Determines, whether Logical, or Short name, referencing is used.
        /// </summary>
        /// <remarks>
        /// Referencing depends on the device to communicate with.
        /// Normally, a device supports only either Logical or Short name referencing.
        /// The referencing is defined by the device manufacturer.
        /// If the referencing is wrong, the SNMR message will fail.
        /// </remarks>
        /// <seealso cref="MaxReceivePDUSize"/>
        [DefaultValue(false)]
        public bool UseLogicalNameReferencing
        {
            get
            {
                return Settings.UseLogicalNameReferencing;
            }
            internal set
            {
                Settings.UseLogicalNameReferencing = value;
            }
        }

        /// <summary>
        /// Used priority.
        /// </summary>
        public Priority Priority
        {
            get
            {
                return Settings.Priority;
            }
            set
            {
                Settings.Priority = value;
            }
        }

        /// <summary>
        /// Used service class.
        /// </summary>
        public ServiceClass ServiceClass
        {
            get
            {
                return Settings.ServiceClass;
            }
            set
            {
                Settings.ServiceClass = value;
            }
        }

        /// <summary>
        /// Set starting packet index. Default is One based, but some meters use Zero based value. Usually this is not used.
        /// </summary>
        public UInt32 StartingBlockIndex
        {
            get
            {
                return Settings.StartingBlockIndex;
            }
            set
            {
                Settings.StartingBlockIndex = value;
            }
        }

        /// <summary>
        /// Invoke ID.
        /// </summary>
        public byte InvokeID
        {
            get
            {
                return Settings.InvokeID;
            }
            set
            {
                Settings.InvokeID = value;
            }
        }

        /// <summary>
        /// Determines the type of the connection
        /// </summary>
        /// <remarks>
        /// All DLMS meters do not support the IEC 62056-47 standard.
        /// If the device does not support the standard, and the connection is made
        /// using TCP/IP, set the type to InterfaceType.General.
        /// </remarks>
        public InterfaceType InterfaceType
        {
            get
            {
                return Settings.InterfaceType;
            }
            internal set
            {
                Settings.InterfaceType = value;
            }
        }

        /// <summary>
        /// Functionality what client is ask from the meter meter updates this value and tells what it can offer.
        /// </summary>
        /// <remarks>
        /// When connection is made client tells what kind of services it want's to use.
        /// Meter returns functionality what it can offer.
        /// </remarks>
        public Conformance Conformance
        {
            get
            {
                return Settings.ProposedConformance;
            }
            set
            {
                Settings.ProposedConformance = value;
            }
        }

        /// <summary>
        ///  Close server.
        /// </summary>
        public virtual void Close()
        {
            foreach (GXDLMSObject it in Items)
            {
                it.Stop(this);
            }
        }

        /// <summary>
        ///  Initialize server.
        /// </summary>
        /// <remarks>
        /// This must call after server objects are set.
        /// </remarks>
        public void Initialize()
        {
            Initialize(false);
        }

        /// <summary>
        ///  Initialize server.
        /// </summary>
        /// <remarks>
        /// This must call after server objects are set.
        /// </remarks>
        /// <param name="manually">If true, server handle objects and all data are updated manually.</param>
        public void Initialize(bool manually)
        {
            Initialized = true;
            if (manually)
            {
                return;
            }
            bool association = false;
            for (int pos = 0; pos != Items.Count; ++pos)
            {
                GXDLMSObject it = Items[pos];
                if (this.UseLogicalNameReferencing &&
                        (string.IsNullOrEmpty(it.LogicalName) || it.LogicalName.Split('.').Length != 6))
                {
                    throw new Exception("Invalid Logical Name.");
                }
                it.Start(this);
                if (it is GXDLMSProfileGeneric)
                {
                    GXDLMSProfileGeneric pg = it as GXDLMSProfileGeneric;
                    foreach (var obj in pg.CaptureObjects)
                    {
                        if (obj.Value.AttributeIndex < 1)
                        {
                            throw new Exception("Invalid attribute index. SelectedAttributeIndex is not set for " + obj.Key.Name);
                        }
                    }
                }
                else if (it is GXDLMSAssociationShortName && !UseLogicalNameReferencing)
                {
                    if ((it as GXDLMSAssociationShortName).ObjectList.Count == 0)
                    {
                        (it as GXDLMSAssociationShortName).ObjectList.AddRange(this.Items);
                    }
                    association = true;
                }
                else if (it is GXDLMSAssociationLogicalName && UseLogicalNameReferencing)
                {
                    GXDLMSAssociationLogicalName ln = it as GXDLMSAssociationLogicalName;
                    if (ln.ObjectList.Count == 0)
                    {
                        ln.ObjectList.AddRange(this.Items);
                    }
                    association = true;
                    ln.XDLMSContextInfo.MaxReceivePduSize = ln.XDLMSContextInfo.MaxSendPduSize = Settings.MaxServerPDUSize;
                    ln.XDLMSContextInfo.Conformance = Settings.ProposedConformance;
                }
                else if (!(it is IGXDLMSBase))//Remove unsupported items.
                {
                    Debug.WriteLine(it.ObjectType.ToString() + " not supported.");
                    Items.RemoveAt(pos);
                    --pos;
                }
            }
            if (!association)
            {
                if (UseLogicalNameReferencing)
                {
                    GXDLMSAssociationLogicalName ln = new GXDLMSAssociationLogicalName();
                    ln.XDLMSContextInfo.MaxReceivePduSize = ln.XDLMSContextInfo.MaxSendPduSize = Settings.MaxServerPDUSize;
                    ln.XDLMSContextInfo.Conformance = Settings.ProposedConformance;
                    ln.ObjectList = Items;
                    Items.Add(ln);
                }
                else
                {
                    GXDLMSAssociationShortName it = new GXDLMSAssociationShortName();
                    it.ObjectList = Items;
                    Items.Add(it);
                }
            }
            //Arrange items by Short Name.
            UpdateShortNames(false);
        }

        /// <summary>
        /// Update short names.
        /// </summary>
        protected void UpdateShortNames()
        {
            UpdateShortNames(true);
        }

        /// <summary>
        /// Update short names.
        /// </summary>
        private void UpdateShortNames(bool force)
        {
            //Arrange items by Short Name.
            int sn = 0xA0;
            int offset, count;
            if (!this.UseLogicalNameReferencing)
            {
                foreach (GXDLMSObject it in Items)
                {
                    if (!(it is GXDLMSAssociationShortName ||
                        it is GXDLMSAssociationLogicalName))
                    {
                        //Generate Short Name if not given.
                        if (force || it.ShortName == 0)
                        {
                            it.ShortName = (ushort)sn;
                            //Add method index addresses.
                            GXDLMS.GetActionInfo(it.ObjectType, out offset, out count);
                            if (count != 0)
                            {
                                sn += offset + (8 * count);
                            }
                            else //If there are no methods.
                            {
                                //Add attribute index addresses.
                                sn += 8 * (it as IGXDLMSBase).GetAttributeCount();
                            }
                        }
                        else
                        {
                            sn = it.ShortName;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reset after connection is closed.
        /// </summary>
        public void Reset()
        {
            Reset(false);
        }

        /// <summary>
        /// Reset settings when connection is made or close.
        /// </summary>
        /// <param name="connected"></param>
        public void Reset(bool connected)
        {
            Settings.protocolVersion = null;
            transaction = null;
            Settings.BlockIndex = 1;
            Settings.Count = Settings.Index = 0;
            Settings.Connected = ConnectionState.None;
            replyData.Clear();
            receivedData.Clear();
            Settings.Password = null;
            if (!connected)
            {
                info.Clear();
                Settings.ServerAddress = 0;
                Settings.ClientAddress = 0;
            }
            Settings.Authentication = Authentication.None;
            if (Settings.Cipher != null)
            {
                if (!connected)
                {
                    Settings.Cipher.Reset();
                }
                else
                {
                    Settings.Cipher.Security = Gurux.DLMS.Enums.Security.None;
                }
            }
            dataReceived = DateTime.MinValue;
        }

        ///<summary>
        /// Handles client request.
        /// </summary>
        ///<param name="buff">Received data from the client. </param>
        ///<returns>
        ///Response to the request. Response is null if request packet is not complete.
        ///</returns>
        public virtual byte[] HandleRequest(byte[] buff)
        {
            return HandleRequest(buff, new GXDLMSConnectionEventArgs());
        }

        ///<summary>
        /// Handles client request.
        /// </summary>
        ///<param name="buff">
        /// Received data from the client. </param>
        ///<returns>
        ///Response to the request. Response is null if request packet is not complete.
        ///</returns>
        public virtual byte[] HandleRequest(byte[] buff, GXDLMSConnectionEventArgs connectionInfo)
        {
            GXServerReply sr = new GXServerReply(buff);
            sr.ConnectionInfo = connectionInfo;
            HandleRequest(sr);
            return sr.Reply;
        }

        ///<summary>
        /// Handles client request.
        /// </summary>
        ///<param name="sr">Server reply parameters. </param>
        public virtual void HandleRequest(GXServerReply sr)
        {
            sr.Reply = null;
            if (!sr.IsStreaming && (sr.Data == null || sr.Data.Length == 0))
            {
                return;
            }
            if (!Initialized)
            {
                throw new Exception("Server not Initialized.");
            }
            try
            {
                if (!sr.IsStreaming)
                {
                    receivedData.Set(sr.Data);
                    bool first = Settings.ServerAddress == 0 && Settings.ClientAddress == 0;
                    try
                    {
                        GXDLMS.GetData(Settings, receivedData, info, null, null);
                    }
                    catch (Exception)
                    {
                        dataReceived = DateTime.Now;
                        receivedData.Size = 0;
                        sr.Reply = GXDLMS.GetHdlcFrame(Settings, (byte)Command.UnacceptableFrame, replyData);
                        return;
                    }
                    //If all data is not received yet.
                    if (!info.IsComplete)
                    {
                        return;
                    }
                    receivedData.Clear();
                    if (info.Command == Command.DisconnectRequest && Settings.Connected == ConnectionState.None)
                    {
                        sr.Reply = GXDLMS.GetHdlcFrame(Settings, (byte)Command.DisconnectMode, replyData);
                        info.Clear();
                        return;
                    }

                    if (first || info.Command == Command.Snrm ||
                        (Settings.InterfaceType == InterfaceType.WRAPPER && info.Command == Command.Aarq))
                    {
                        // Check is data send to this server.
                        if (!IsTarget(Settings.ServerAddress,
                                      Settings.ClientAddress))
                        {
                            info.Clear();
                            return;
                        }
                    }

                    //If client want next frame.
                    if ((info.MoreData & RequestTypes.Frame) == RequestTypes.Frame)
                    {
                        dataReceived = DateTime.Now;
                        sr.Reply = GXDLMS.GetHdlcFrame(Settings, Settings.ReceiverReady(), replyData);
                        return;
                    }
                    //Update command if transaction and next frame is asked.
                    if (info.Command == Command.None)
                    {
                        if (transaction != null)
                        {
                            info.Command = transaction.command;
                        }
                        else if (replyData.Size == 0)
                        {
                            sr.Reply = GXDLMS.GetHdlcFrame(Settings, Settings.ReceiverReady(), replyData);
                            return;
                        }
                    }
                    //Check inactivity time out.
                    if (Hdlc != null && Hdlc.InactivityTimeout != 0)
                    {
                        if (info.Command != Command.Snrm)
                        {
                            int elapsed = (int)(DateTime.Now - dataReceived).TotalSeconds;
                            //If inactivity time out is elapsed.
                            if (elapsed >= Hdlc.InactivityTimeout)
                            {
                                Reset();
                                return;
                            }
                        }
                    }
                    else if (Wrapper != null && Wrapper.InactivityTimeout != 0)
                    {
                        if (info.Command != Command.Aarq)
                        {
                            int elapsed = (int)(DateTime.Now - dataReceived).TotalSeconds;
                            //If inactivity time out is elapsed.
                            if (elapsed >= Wrapper.InactivityTimeout)
                            {
                                Reset();
                                return;
                            }
                        }
                    }
                }
                else
                {
                    info.Command = Command.GeneralBlockTransfer;
                }
                try
                {
                    sr.Reply = HandleCommand(info.Command, info.Data, sr, info.CipheredCommand);
                }
                catch (Exception)
                {
                    receivedData.Size = 0;
                    sr.Reply = GXDLMS.GetHdlcFrame(Settings, (byte)Command.UnacceptableFrame, replyData);
                }
                info.Clear();
                dataReceived = DateTime.Now;
            }
            catch (GXDLMSConfirmedServiceError e)
            {
                Debug.WriteLine(e.ToString());
                replyData.Set(GenerateConfirmedServiceError(e.ConfirmedServiceError, e.ServiceError, e.ServiceErrorValue));
                info.Clear();
                if (this.InterfaceType == Enums.InterfaceType.WRAPPER)
                {
                    sr.Reply = GXDLMS.GetWrapperFrame(Settings, replyData);
                }
                else
                {
                    sr.Reply = GXDLMS.GetHdlcFrame(Settings, 0, replyData);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                if (info.Command != Command.None)
                {
                    sr.Reply = ReportError(info.Command, ErrorCode.HardwareFault);
                    info.Clear();
                }
                else
                {
                    Reset();
                    if (Settings.Connected == ConnectionState.Dlms)
                    {
                        Settings.Connected &= ~ConnectionState.Dlms;
                        Disconnected(sr.ConnectionInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Generate confirmed service error.
        /// </summary>
        /// <param name="service">Confirmed service error.</param>
        /// <param name="type">Service error.</param>
        /// <param name="code">code</param>
        /// <returns></returns>
        internal static byte[] GenerateConfirmedServiceError(ConfirmedServiceError service, ServiceError type, byte code)
        {
            return new byte[] { (byte)Command.ConfirmedServiceError, (byte)service, (byte)type, code };
        }

        ///<summary>
        /// Handle received command.
        ///</summary>
        private byte[] HandleCommand(Command cmd, GXByteBuffer data, GXServerReply sr, Command cipheredCommand)
        {
            byte frame = 0;
            if (Settings.InterfaceType == InterfaceType.HDLC && replyData.Size != 0)
            {
                //Get next frame.
                frame = Settings.NextSend(false);
            }
            switch (cmd)
            {
                case Command.AccessRequest:
                    GXDLMSLNCommandHandler.HandleAccessRequest(Settings, this, data, replyData, null, cipheredCommand);
                    break;
                case Command.SetRequest:
                    GXDLMSLNCommandHandler.HandleSetRequest(Settings, this, data, replyData, null, cipheredCommand);
                    break;
                case Command.WriteRequest:
                    GXDLMSSNCommandHandler.HandleWriteRequest(Settings, this, data, replyData, null, cipheredCommand);
                    break;
                case Command.GetRequest:
                    if (data.Size != 0)
                    {
                        GXDLMSLNCommandHandler.HandleGetRequest(Settings, this, data, replyData, null, cipheredCommand);
                    }
                    break;
                case Command.ReadRequest:
                    GXDLMSSNCommandHandler.HandleReadRequest(Settings, this, data, replyData, null, cipheredCommand);
                    break;
                case Command.MethodRequest:
                    GXDLMSLNCommandHandler.HandleMethodRequest(Settings, this, data, sr.ConnectionInfo, replyData, null, cipheredCommand);
                    break;
                case Command.Snrm:
                    HandleSnrmRequest(data);
                    frame = (byte)Command.Ua;
                    Settings.Connected = ConnectionState.Hdlc;
                    break;
                case Command.Aarq:
                    HandleAarqRequest(data, sr.ConnectionInfo);
                    break;
                case Command.ReleaseRequest:
                    HandleReleaseRequest(data, sr.ConnectionInfo);
                    Settings.Connected &= ~ConnectionState.Dlms;
                    Disconnected(sr.ConnectionInfo);
                    break;
                case Command.DisconnectRequest:
                    GenerateDisconnectRequest();
                    if (Settings.Connected > ConnectionState.None)
                    {
                        if (Settings.Connected == ConnectionState.Dlms)
                        {
                            Disconnected(sr.ConnectionInfo);
                        }
                        Settings.Connected = ConnectionState.None;
                    }
                    frame = (byte)Command.Ua;
                    break;
                case Command.GeneralBlockTransfer:
                    if (!HandleGeneralBlockTransfer(data, sr, info.CipheredCommand))
                    {
                        return null;
                    }
                    break;
                case Command.None:
                    //Get next frame.
                    break;
                default:
                    throw new Exception("Invalid command: " + (int)cmd);
            }
            byte[] reply;
            if (this.InterfaceType == Enums.InterfaceType.WRAPPER)
            {
                reply = GXDLMS.GetWrapperFrame(Settings, replyData);
            }
            else
            {
                reply = GXDLMS.GetHdlcFrame(Settings, frame, replyData);
            }
            if (cmd == Command.DisconnectRequest ||
                (InterfaceType == InterfaceType.WRAPPER && cmd == Command.ReleaseRequest))
            {
                Reset();
            }
            return reply;
        }

        private bool HandleGeneralBlockTransfer(GXByteBuffer data, GXServerReply sr, Command cipheredCommand)
        {
            if (transaction != null)
            {
                if (transaction.command == Command.GetRequest)
                {
                    // Get request for next data block
                    if (sr.Count == 0)
                    {
                        ++Settings.BlockNumberAck;
                        sr.Count = Settings.WindowSize;
                    }
                    GXDLMSLNCommandHandler.GetRequestNextDataBlock(Settings, 0, this, data, replyData, null, true, cipheredCommand);
                    if (sr.Count != 0)
                    {
                        --sr.Count;
                    }
                    if (this.transaction == null)
                    {
                        sr.Count = 0;
                    }
                }
                else
                {
                    //BlockControl
                    byte bc = data.GetUInt8();
                    //Block number.
                    UInt16 blockNumber = data.GetUInt16();
                    //Block number acknowledged.
                    UInt16 blockNumberAck = data.GetUInt16();
                    int len = GXCommon.GetObjectCount(data);
                    if (len > data.Size - data.Position)
                    {
                        replyData.Set(GXDLMSServer.GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                        ServiceError.Service, (byte)Service.Unsupported));
                    }
                    else
                    {
                        transaction.data.Set(data);
                        //Send ACK.
                        bool igonoreAck = (bc & 0x40) != 0 && (blockNumberAck * WindowSize) + 1 > blockNumber;
                        UInt16 windowSize = Settings.WindowSize;
                        UInt16 bn = (UInt16)Settings.BlockIndex;
                        if ((bc & 0x80) != 0)
                        {
                            HandleCommand(transaction.command, transaction.data, sr, cipheredCommand);
                            transaction = null;
                            igonoreAck = false;
                            windowSize = 1;
                        }
                        if (igonoreAck)
                        {
                            return false;
                        }
                        replyData.SetUInt8(Command.GeneralBlockTransfer);
                        replyData.SetUInt8((byte)(0x80 | windowSize));
                        ++Settings.BlockIndex;
                        replyData.SetUInt16(bn);
                        replyData.SetUInt16(blockNumber);
                        replyData.SetUInt8(0);
                    }
                }
            }
            else
            {
                //BlockControl
                byte bc = data.GetUInt8();
                //Block number.
                UInt16 blockNumber = data.GetUInt16();
                //Block number acknowledged.
                UInt16 blockNumberAck = data.GetUInt16();
                int len = GXCommon.GetObjectCount(data);
                if (len > data.Size - data.Position)
                {
                    replyData.Set(GXDLMSServer.GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                       ServiceError.Service, (byte)Service.Unsupported));
                }
                else
                {
                    transaction = new GXDLMSLongTransaction(null, (Command)data.GetUInt8(), data);
                    replyData.SetUInt8(Command.GeneralBlockTransfer);
                    replyData.SetUInt8((byte)(0x80 | Settings.WindowSize));
                    replyData.SetUInt16(blockNumber);
                    ++blockNumberAck;
                    replyData.SetUInt16(blockNumberAck);
                    replyData.SetUInt8(0);
                }
            }
            return true;
        }

        private byte[] ReportError(Command cmd, ErrorCode error)
        {
            switch (cmd)
            {
                case Command.ReadRequest:
                    cmd = Command.ReadResponse;
                    break;
                case Command.WriteRequest:
                    cmd = Command.WriteResponse;
                    break;
                case Command.GetRequest:
                    cmd = Command.GetResponse;
                    break;
                case Command.SetRequest:
                    cmd = Command.SetResponse;
                    break;
                case Command.MethodRequest:
                    cmd = Command.MethodResponse;
                    break;
                default:
                    //Return HW error and close connection..
                    break;
            }
            if (Settings.UseLogicalNameReferencing)
            {
                GXDLMS.GetLNPdu(new GXDLMSLNParameters(null, Settings, 0, cmd, 1, null, null, (byte)error, info.CipheredCommand), replyData);
            }
            else
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8(error);
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, cmd, 1, (byte)error, null, bb);
                GXDLMS.GetSNPdu(p, replyData);
            }
            if (this.InterfaceType == Enums.InterfaceType.WRAPPER)
            {
                return GXDLMS.GetWrapperFrame(Settings, replyData);
            }
            else
            {
                return GXDLMS.GetHdlcFrame(Settings, 0, replyData);
            }
        }

        ///<summary>
        ///Parse AARQ request that client send and returns AARE request.
        /// </summary>
        ///<returns>
        ///Reply to the client.
        ///</returns>
        private void HandleAarqRequest(GXByteBuffer data, GXDLMSConnectionEventArgs connectionInfo)
        {
            GXByteBuffer error = null;
            AssociationResult result = AssociationResult.Accepted;
            Settings.CtoSChallenge = null;
            if (Settings.Cipher != null)
            {
                Settings.Cipher.DedicatedKey = null;
            }
            if (!Settings.UseCustomChallenge)
            {
                Settings.StoCChallenge = null;
            }
            // Reset settings for wrapper.
            if (Settings.InterfaceType == InterfaceType.WRAPPER)
            {
                Reset(true);
            }
            SourceDiagnostic diagnostic = SourceDiagnostic.NoReasonGiven;
            try
            {
                diagnostic = (SourceDiagnostic)GXAPDU.ParsePDU(Settings, Settings.Cipher, data, null);
                if (Settings.NegotiatedConformance == Conformance.None)
                {
                    result = AssociationResult.PermanentRejected;
                    diagnostic = SourceDiagnostic.NoReasonGiven;
                    error = new GXByteBuffer();
                    error.SetUInt8(0xE);
                    error.SetUInt8(ConfirmedServiceError.InitiateError);
                    error.SetUInt8(ServiceError.Initiate);
                    error.SetUInt8(Initiate.IncompatibleConformance);
                }
                //If PDU is too low.
                else if (Settings.MaxPduSize < 64)
                {
                    result = AssociationResult.PermanentRejected;
                    diagnostic = SourceDiagnostic.NoReasonGiven;
                    error = new GXByteBuffer();
                    error.SetUInt8(0xE);
                    error.SetUInt8(ConfirmedServiceError.InitiateError);
                    error.SetUInt8(ServiceError.Initiate);
                    error.SetUInt8(Initiate.PduSizeTooShort);
                }
                else if (Settings.DLMSVersion != 6)
                {
                    Settings.DLMSVersion = 6;
                    result = AssociationResult.PermanentRejected;
                    diagnostic = SourceDiagnostic.NoReasonGiven;
                    error = new GXByteBuffer();
                    error.SetUInt8(0xE);
                    error.SetUInt8(ConfirmedServiceError.InitiateError);
                    error.SetUInt8(ServiceError.Initiate);
                    error.SetUInt8(Initiate.DlmsVersionTooLow);
                }
                else if (diagnostic != SourceDiagnostic.None)
                {
                    result = AssociationResult.PermanentRejected;
                    diagnostic = SourceDiagnostic.ApplicationContextNameNotSupported;
                    InvalidConnection(connectionInfo);
                }
                else
                {
                    diagnostic = ValidateAuthentication(Settings.Authentication, Settings.Password);
                    if (diagnostic != SourceDiagnostic.None)
                    {
                        result = AssociationResult.PermanentRejected;
                        InvalidConnection(connectionInfo);
                    }
                    else if (Settings.Authentication > Authentication.Low)
                    {
                        // If High authentication is used.
                        result = AssociationResult.Accepted;
                        diagnostic = SourceDiagnostic.AuthenticationRequired;
                        if (UseLogicalNameReferencing)
                        {
                            GXDLMSAssociationLogicalName ln = (GXDLMSAssociationLogicalName)Items.FindByLN(ObjectType.AssociationLogicalName, "0.0.40.0.0.255");
                            if (ln != null)
                            {
                                if (Settings.Cipher == null || Settings.Cipher.Security == Enums.Security.None)
                                {
                                    ln.ApplicationContextName.ContextId = ApplicationContextName.LogicalName;
                                }
                                else
                                {
                                    ln.ApplicationContextName.ContextId = ApplicationContextName.LogicalNameWithCiphering;
                                }
                                ln.AuthenticationMechanismName.MechanismId = Settings.Authentication;
                                ln.AssociationStatus = AssociationStatus.AssociationPending;
                            }
                        }
                    }
                    else
                    {
                        if (UseLogicalNameReferencing)
                        {
                            GXDLMSAssociationLogicalName ln = (GXDLMSAssociationLogicalName)Items.FindByLN(ObjectType.AssociationLogicalName, "0.0.40.0.0.255");
                            if (ln != null)
                            {
                                if (Settings.Cipher == null || Settings.Cipher.Security == Enums.Security.None)
                                {
                                    ln.ApplicationContextName.ContextId = ApplicationContextName.LogicalName;
                                }
                                else
                                {
                                    ln.ApplicationContextName.ContextId = ApplicationContextName.LogicalNameWithCiphering;
                                }
                                ln.AuthenticationMechanismName.MechanismId = Settings.Authentication;
                                ln.AssociationStatus = AssociationStatus.Associated;
                            }
                        }
                        Connected(connectionInfo);
                        Settings.Connected |= ConnectionState.Dlms;
                    }
                }
            }
            catch (GXDLMSException e)
            {
                result = e.Result;
                diagnostic = (SourceDiagnostic)e.Diagnostic;
            }
            if (Settings.Authentication > Authentication.Low && !Settings.UseCustomChallenge)
            {
                // If High authentication is used.
                Settings.StoCChallenge = GXSecure.GenerateChallenge(Settings.Authentication);
            }
            if (Settings.InterfaceType == Enums.InterfaceType.HDLC)
            {
                replyData.Set(GXCommon.LLCReplyBytes);
            }
            // Generate AARE packet.
            GXAPDU.GenerateAARE(Settings, replyData, result, diagnostic, Settings.Cipher, error, null);
        }

        /// <summary>
        /// Handles release reuest.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <param name="connectionInfo">Connection info.</param>
        private void HandleReleaseRequest(GXByteBuffer data, GXDLMSConnectionEventArgs connectionInfo)
        {
            //Return error if connection is not established.
            if ((Settings.Connected & ConnectionState.Hdlc) == 0)
            {
                replyData.Add(GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                              ServiceError.Service, (byte)Service.Unsupported));
                return;
            }
            if (Settings.InterfaceType == InterfaceType.HDLC)
            {
                replyData.Set(0, GXCommon.LLCReplyBytes);
            }
            byte[] tmp = GXAPDU.GetUserInformation(Settings, Settings.Cipher);
            replyData.SetUInt8(0x63);
            //Len.
            replyData.SetUInt8((byte)(tmp.Length + 3));
            replyData.SetUInt8(0x80);
            replyData.SetUInt8(0x01);
            replyData.SetUInt8(0x00);
            replyData.SetUInt8(0xBE);
            replyData.SetUInt8((byte)(tmp.Length + 1));
            replyData.SetUInt8(4);
            replyData.SetUInt8((byte)(tmp.Length));
            replyData.Set(tmp);
        }

        ///<summary>
        ///Parse SNRM Request. If server do not accept client empty byte array is returned.
        /// </summary>
        ///<returns>
        ///Returns returned UA packet.
        ///</returns>
        private void HandleSnrmRequest(GXByteBuffer data)
        {
            GXDLMS.ParseSnrmUaResponse(data, Settings);
            Reset(true);
            if (Hdlc != null)
            {
                //If client wants send larger HDLC frames what meter accepts.
                if (Settings.Limits.MaxInfoTX > Hdlc.MaximumInfoLengthReceive)
                {
                    Settings.Limits.MaxInfoTX = (UInt16)Hdlc.MaximumInfoLengthReceive;
                }
                //If client wants receive larger HDLC frames what meter accepts.
                if (Settings.Limits.MaxInfoRX > Hdlc.MaximumInfoLengthTransmit)
                {
                    Settings.Limits.MaxInfoRX = (UInt16)Hdlc.MaximumInfoLengthTransmit;
                }
                //If client asks higher window size what meter accepts.
                if (Settings.Limits.WindowSizeTX > Hdlc.WindowSizeReceive)
                {
                    Settings.Limits.WindowSizeTX = (byte)Hdlc.WindowSizeReceive;
                }
                //If client asks higher window size what meter accepts.
                if (Settings.Limits.WindowSizeRX > Hdlc.WindowSizeTransmit)
                {
                    Settings.Limits.WindowSizeRX = (byte)Hdlc.WindowSizeTransmit;
                }
            }
            replyData.SetUInt8(0x81); // FromatID
            replyData.SetUInt8(0x80); // GroupID
            replyData.SetUInt8(0); // Length
            replyData.SetUInt8(HDLCInfo.MaxInfoTX);
            GXDLMS.AppendHdlcParameter(replyData, Settings.Limits.MaxInfoTX);
            replyData.SetUInt8(HDLCInfo.MaxInfoRX);
            GXDLMS.AppendHdlcParameter(replyData, Settings.Limits.MaxInfoRX);
            replyData.SetUInt8(HDLCInfo.WindowSizeTX);
            replyData.SetUInt8(4);
            replyData.SetUInt32(Settings.Limits.WindowSizeTX);
            replyData.SetUInt8(HDLCInfo.WindowSizeRX);
            replyData.SetUInt8(4);
            replyData.SetUInt32(Settings.Limits.WindowSizeRX);
            replyData.SetUInt8(2, (byte)(replyData.Size - 3));
        }

        ///<summary>
        ///Generates disconnect request.
        /// </summary>
        ///<returns>
        ///Disconnect request.
        ///</returns>
        private void GenerateDisconnectRequest()
        {
            //Return error if connection is not established.
            if ((Settings.Connected & ConnectionState.Dlms) == 0)
            {
                replyData.Add(GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                              ServiceError.Service, (byte)Service.Unsupported));
                return;
            }

            replyData.SetUInt8(0x81); // FromatID
            replyData.SetUInt8(0x80); // GroupID
            replyData.SetUInt8(0); // Length

            replyData.SetUInt8(HDLCInfo.MaxInfoTX);
            GXDLMS.AppendHdlcParameter(replyData, Settings.Limits.MaxInfoTX);

            replyData.SetUInt8(HDLCInfo.MaxInfoRX);
            GXDLMS.AppendHdlcParameter(replyData, Settings.Limits.MaxInfoRX);

            replyData.SetUInt8(HDLCInfo.WindowSizeTX);
            replyData.SetUInt8(4);
            replyData.SetUInt32(Settings.Limits.WindowSizeTX);

            replyData.SetUInt8(HDLCInfo.WindowSizeRX);
            replyData.SetUInt8(4);
            replyData.SetUInt32(Settings.Limits.WindowSizeRX);

            int len = replyData.Position - 3;
            replyData.SetUInt8(2, (byte)len); // Length.
        }

        /// <summary>
        /// Add value of COSEM object to byte buffer.
        /// </summary>
        /// <param name="obj">COSEM object.</param>
        /// <param name="index">Attribute index.</param>
        /// <param name="buff">Byte buffer.</param>
        /// <remarks>
        /// AddData method can be used with GetDataNotificationMessage -method.
        /// DLMS spesification do not specify the structure of Data-Notification body.
        /// So each manufacture can sent different data.
        /// </remarks>
        internal static void AddData(GXDLMSSettings settings, GXDLMSObject obj, int index, GXByteBuffer buff)
        {
            DataType dt;
            object value = (obj as IGXDLMSBase).GetValue(settings, new ValueEventArgs(settings, obj, index, 0, null));
            dt = obj.GetDataType(index);
            if (dt == DataType.None && value != null)
            {
                dt = GXDLMSConverter.GetDLMSDataType(value);
            }
            GXCommon.SetData(settings, buff, dt, value);
        }

        /// <summary>
        /// Add value of COSEM object to byte buffer.
        /// </summary>
        /// <param name="obj">COSEM object.</param>
        /// <param name="index">Attribute index.</param>
        /// <param name="buff">Byte buffer.</param>
        /// <remarks>
        /// AddData method can be used with GetDataNotificationMessage -method.
        /// DLMS spesification do not specify the structure of Data-Notification body.
        /// So each manufacture can sent different data.
        /// </remarks>
        /// <seealso cref="GenerateDataNotificationMessages"/>
        public void AddData(GXDLMSObject obj, int index, GXByteBuffer buff)
        {
            AddData(Settings, obj, index, buff);
        }

        /// <summary>
        /// Generates data notification message(s).
        /// </summary>
        /// <param name="time">Date time. Set To Min or Max if not added</param>
        /// <param name="data">Notification body.</param>
        /// <returns>Generated data notification message(s).</returns>
        public byte[][] GenerateDataNotificationMessages(DateTime time, byte[] data)
        {
            byte[][] reply;
            if (UseLogicalNameReferencing)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(null, Settings, 0, Command.DataNotification, 0, null, new GXByteBuffer(data), 0xff, Command.None);
                p.time = time;
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.DataNotification, 1, 0, new GXByteBuffer(data), null);
                reply = GXDLMS.GetSnMessages(p);
            }
            if ((Settings.ProposedConformance & Conformance.GeneralBlockTransfer) == 0 && reply.Length != 1)
            {
                throw new ArgumentException("Data is not fit to one PDU. Use general block transfer.");
            }
            return reply;
        }

        /// <summary>
        /// Generates data notification message(s).
        /// </summary>
        /// <param name="time">Date time. Set To Min or Max if not added</param>
        /// <param name="data">Notification body.</param>
        /// <returns>Generated data notification message(s).</returns>
        public byte[][] GenerateDataNotificationMessages(DateTime time, GXByteBuffer data)
        {
            byte[][] reply;
            if (UseLogicalNameReferencing)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(null, Settings, 0, Command.DataNotification, 0, null, data, 0xff, Command.None);
                p.time = time;
                p.time.Skip |= DateTimeSkips.Ms;
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.DataNotification, 1, 0, data, null);
                reply = GXDLMS.GetSnMessages(p);
            }
            return reply;
        }

        /// <summary>
        /// Generates push setup message.
        /// </summary>
        /// <param name="date"> Date time. Set To Min or Max if not added.</param>
        /// <param name="push">Target Push object.</param>
        /// <returns>Generated data notification message(s).</returns>
        public byte[][] GeneratePushSetupMessages(DateTime date, GXDLMSPushSetup push)
        {
            if (push == null)
            {
                throw new ArgumentNullException("push");
            }
            GXByteBuffer buff = new GXByteBuffer();
            buff.SetUInt8((byte)DataType.Structure);
            GXCommon.SetObjectCount(push.PushObjectList.Count, buff);
            foreach (KeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in push.PushObjectList)
            {
                AddData(it.Key, it.Value.AttributeIndex, buff);
            }
            return GenerateDataNotificationMessages(date, buff);
        }

        internal void TimeUpdated()
        {
            if (waiting != null)
            {
                waiting.Set();
            }
        }


        /// <summary>
        /// Run the background processes.
        /// </summary>
        /// <returns>Wait time before next exent is triggered.</returns>
        public int Run(EventWaitHandle wait)
        {
            List<KeyValuePair<GXDLMSObject, int>> list = new List<KeyValuePair<GXDLMSObject, int>>();
            waiting = wait;
            DateTime now = DateTime.Now;
            DateTime next = now.AddDays(1);
            foreach (GXDLMSAutoConnect it in Items.GetObjects(ObjectType.AutoConnect))
            {
                foreach (var time in it.CallingWindow)
                {
                    if (time.Key.Compare(now) != 1 && time.Value.Compare(now) != -1)
                    {
                        if (ExecutionTimes.ContainsKey(it))
                        {
                            DateTime tmp = ExecutionTimes[it];
                            if (tmp < now)
                            {
                                list.Add(new KeyValuePair<GXDLMSObject, int>(it, 1));
                                ExecutionTimes[it] = now;
                            }
                        }
                        else
                        {
                            list.Add(new KeyValuePair<GXDLMSObject, int>(it, 1));
                            ExecutionTimes.Add(it, now);
                        }
                        break;
                    }
                    else
                    {
                        DateTime tmp = GXDateTime.GetNextScheduledDates(now, time.Key, 1)[0];
                        if (tmp < next)
                        {
                            next = tmp;
                        }
                    }
                }
            }
            /*
            foreach (GXDLMSAutoConnect it in Items.GetObjects(ObjectType.AutoConnect))
            {
                foreach (var time in it.CallingWindow)
                {
                    if (time.Key.Compare(now) != 1 && time.Value.Compare(now) != -1)
                    {
                        DateTime tmp = GXDateTime.GetNextScheduledDates(now, time.Key, 1)[0];
                        if (tmp < next)
                        {
                            next = tmp;
                        }
                    }
                }
            }
            */
            if (list.Count != 0)
            {
                Execute(list);
            }
            return (int)(next - now).TotalSeconds;
        }

        /// <summary>
        /// Server to Client challenge.
        /// </summary>
        public byte[] StoCChallenge
        {
            get
            {
                return Settings.StoCChallenge;
            }
            set
            {
                Settings.UseCustomChallenge = value != null;
                Settings.StoCChallenge = value;
            }
        }
    }
}