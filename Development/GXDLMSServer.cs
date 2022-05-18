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
using System.Text;

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
        private string flaID;

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

        /// <summary>
        /// Server is using push client address when sending push messages. Client address is used if PushAddress is zero.
        /// </summary>
        public int PushClientAddress
        {
            get
            {
                return Settings.PushClientAddress;
            }
            set
            {
                Settings.PushClientAddress = value;
            }
        }

        /// <summary>
        /// Flag ID is used when server is operating using optical probe.
        /// </summary>
        public string FlaID
        {
            get
            {
                return flaID;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length != 3)
                {
                    throw new ArgumentOutOfRangeException("Invalid FLAG ID.");
                }
                flaID = value;
            }
        }

        /// <summary>
        /// Local port setup is used when communicating with optical probe.
        /// </summary>
        public GXDLMSIECLocalPortSetup LocalPortSetup
        {
            get;
            set;
        }

        /// <summary>
        /// Client connection state.
        /// </summary>
        public ConnectionState ConnectionState
        {
            get
            {
                return Settings.Connected;
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
        /// Get attribute access mode.
        /// </summary>
        /// <param name="arg">Value event argument.</param>
        /// <returns>Access mode.</returns>
        protected abstract AccessMode3 GetAttributeAccess3(ValueEventArgs arg);

        /// <summary>
        /// Get method access mode.
        /// </summary>
        /// <param name="arg">Value event argument.</param>
        /// <returns>Method access mode.</returns>
        protected abstract MethodAccessMode GetMethodAccess(ValueEventArgs arg);

        /// <summary>
        /// Get method access mode.
        /// </summary>
        /// <param name="arg">Value event argument.</param>
        /// <returns>Method access mode.</returns>
        protected abstract MethodAccessMode3 GetMethodAccess3(ValueEventArgs arg);

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
        [Obsolete("Use PreRead instead.")]
        public abstract void PreGet(ValueEventArgs[] args);

        /// <summary>
        /// Post get selected value.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        [Obsolete("Use PostRead instead.")]
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
        internal int NotifyGetAttributeAccess(ValueEventArgs arg)
        {
            if (arg.Index == 1)
            {
                return (int)AccessMode.Read;
            }
            if (AssignedAssociation != null)
            {
                if (AssignedAssociation.Version < 3)
                {
                    return (int)GetAttributeAccess(arg);
                }
                return (int)GetAttributeAccess3(arg);
            }
            return (int)GetAttributeAccess(arg);
        }

        /// <summary>
        /// Get method access mode.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Method access mode</returns>
        internal int NotifyGetMethodAccess(ValueEventArgs arg)
        {
            if (AssignedAssociation != null)
            {
                if (AssignedAssociation.Version < 3)
                {
                    return (int)GetMethodAccess(arg);
                }
                return (int)GetMethodAccess3(arg);
            }
            return (int)GetMethodAccess(arg);
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
        internal void NotifyPreAction(ValueEventArgs[] args)
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
            Settings = new GXDLMSSettings(true, type);
            Settings.Plc.Reset();
            Settings.Objects.Parent = this;
            Settings.ServerAddress = 1;
            Settings.ClientAddress = 16;
            Settings.UseLogicalNameReferencing = logicalNameReferencing;
            Reset();
            ExecutionTimes = new Dictionary<GXDLMSObject, DateTime>();
            if (type == InterfaceType.Plc)
            {
                Settings.MaxServerPDUSize = 134;
            }
            Settings.CryptoNotifier = new GXCryptoNotifier();
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
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="llc">IEC 8802-2 LLC settings.</param>
        public GXDLMSServer(GXDLMSAssociationShortName sn, GXDLMSIec8802LlcType2Setup llc) : this(false, InterfaceType.Plc)
        {
            Settings.Objects.Add(sn);
            Settings.Objects.Add(llc);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="llc">IEC 8802-2 LLC settings.</param>
        public GXDLMSServer(GXDLMSAssociationLogicalName ln, GXDLMSIec8802LlcType2Setup llc) : this(true, InterfaceType.Plc)
        {
            Settings.Objects.Add(ln);
            Settings.Objects.Add(llc);
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="mac">IEC 61334-4-512 settings.</param>
        public GXDLMSServer(GXDLMSAssociationShortName sn, GXDLMSSFSKPhyMacSetUp mac) : this(false, InterfaceType.PlcHdlc)
        {
            Settings.Objects.Add(sn);
            Settings.Objects.Add(mac);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="mac">IEC 61334-4-512 settings.</param>
        public GXDLMSServer(GXDLMSAssociationLogicalName ln, GXDLMSSFSKPhyMacSetUp mac) : this(true, InterfaceType.PlcHdlc)
        {
            Settings.Objects.Add(ln);
            Settings.Objects.Add(mac);
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
        /// Used authentication.
        /// </summary>
        public Authentication Authentication
        {
            get
            {
                return Settings.Authentication;
            }
        }


        /// <summary>
        /// Challenge Size.
        /// </summary>
        /// <remarks>
        /// Random challenge is used if value is zero.
        /// </remarks>
        public byte ChallengeSize
        {
            get
            {
                return Settings.ChallengeSize;
            }
            set
            {
                Settings.ChallengeSize = value;
            }
        }

        /// <summary>
        /// HDLC settings.
        /// </summary>
        public GXDLMSHdlcSetup Hdlc
        {
            get;
            set;
        }

        /// <summary>
        /// Wrapper settings.
        /// </summary>
        public GXDLMSTcpUdpSetup Wrapper
        {
            get;
            set;
        }

        /// <summary>
        /// Gateway settings.
        /// </summary>
        public GXDLMSGateway Gateway
        {
            get
            {
                return Settings.Gateway;
            }
            set
            {
                Settings.Gateway = value;
            }
        }

        /// <summary>
        /// GBT window size.
        /// </summary>
        public byte GBTWindowSize
        {
            get
            {
                return Settings.GbtWindowSize;
            }
            set
            {
                Settings.GbtWindowSize = value;
            }
        }

        /// <summary>
        /// HDLC connection settings.
        /// </summary>
        [Obsolete("Use HdlcSettings instead.")]
        public GXDLMSLimits Limits
        {
            get
            {
                return (GXDLMSLimits)Settings.Hdlc;
            }
        }

        /// <summary>
        /// HDLC connection settings.
        /// </summary>
        public GXHdlcSettings HdlcSettings
        {
            get
            {
                return Settings.Hdlc;
            }
        }

        /// <summary>
        /// Standard says that Time zone is from normal time to UTC in minutes.
        /// If meter is configured to use UTC time (UTC to normal time) set this to true.
        /// Example. Italy, Saudi Arabia and India standards are using UTC time zone, not DLMS standard time zone.
        /// </summary>
        public bool UseUtc2NormalTime
        {
            get
            {
                return Settings.UseUtc2NormalTime;
            }
            set
            {
                Settings.UseUtc2NormalTime = value;
            }
        }

        /// <summary>
        /// Expected Invocation (Frame) counter value.
        /// </summary>
        /// <remarks>
        /// If this value is set ciphered PDUs that are using smaller invocation counter values are rejected.
        /// Invocation counter value is not validate if value is zero.
        /// </remarks>
        public UInt64 ExpectedInvocationCounter
        {
            get
            {
                return Settings.ExpectedInvocationCounter;
            }
            set
            {
                Settings.ExpectedInvocationCounter = value;
            }
        }

        /// <summary>
        /// Some meters expect that Invocation Counter is increased for Authentication when connection is established.
        /// </summary>
        public bool IncreaseInvocationCounterForAuthentication
        {
            get
            {
                return Settings.IncreaseInvocationCounterForGMacAuthentication;
            }
            set
            {
                Settings.IncreaseInvocationCounterForGMacAuthentication = value;
            }
        }

        /// <summary>
        /// Skipped date time fields. This value can be used if meter can't handle deviation or status.
        /// </summary>
        public DateTimeSkips DateTimeSkips
        {
            get
            {
                return Settings.DateTimeSkips;
            }
            set
            {
                Settings.DateTimeSkips = value;
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
        /// Assigned association for the server.
        /// </summary>
        protected GXDLMSAssociationLogicalName AssignedAssociation
        {
            get
            {
                return Settings.AssignedAssociation;
            }
            set
            {
                Settings.AssignedAssociation = value;
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
            GXDLMSObject associationObject = null;
            Initialized = true;
            if (manually)
            {
                return;
            }
            if (InterfaceType == InterfaceType.HdlcWithModeE)
            {
                if (string.IsNullOrEmpty(flaID))
                {
                    throw new Exception("Invalid flag ID.");
                }
                if (LocalPortSetup == null)
                {
                    throw new Exception("Default LocalPortSetup is not set.");
                }

            }
            for (int pos = 0; pos != Items.Count; ++pos)
            {
                GXDLMSObject it = Items[pos];
                if (this.UseLogicalNameReferencing &&
                        (string.IsNullOrEmpty(it.LogicalName) || it.LogicalName.Split('.').Length != 6))
                {
                    throw new Exception("Invalid Logical Name.");
                }
                it.Start(this);
                if (it is GXDLMSAssociationShortName && !UseLogicalNameReferencing)
                {
                    if ((it as GXDLMSAssociationShortName).ObjectList.Count == 0)
                    {
                        (it as GXDLMSAssociationShortName).ObjectList.AddRange(this.Items);
                    }
                    associationObject = it;
                }
                else if (it is GXDLMSAssociationLogicalName && UseLogicalNameReferencing)
                {
                    GXDLMSAssociationLogicalName ln = it as GXDLMSAssociationLogicalName;
                    if (ln.ObjectList.Count == 0)
                    {
                        ln.ObjectList.AddRange(this.Items);
                    }
                    associationObject = it;
                    if (Settings.ProposedConformance != 0)
                    {
                        ln.XDLMSContextInfo.Conformance = Settings.ProposedConformance;
                    }
                }
                else if (!(it is IGXDLMSBase))//Remove unsupported items.
                {
                    Debug.WriteLine(it.ObjectType.ToString() + " not supported.");
                    Items.RemoveAt(pos);
                    --pos;
                }
            }
            if (associationObject == null)
            {
                if (UseLogicalNameReferencing)
                {
                    GXDLMSAssociationLogicalName ln = new GXDLMSAssociationLogicalName();
                    ln.XDLMSContextInfo.MaxReceivePduSize = ln.XDLMSContextInfo.MaxSendPduSize = Settings.MaxServerPDUSize;
                    ln.XDLMSContextInfo.Conformance = Settings.ProposedConformance;
                    ln.ObjectList.AddRange(Items);
                    Items.Add(ln);
                }
                else
                {
                    GXDLMSAssociationShortName it = new GXDLMSAssociationShortName();
                    it.ObjectList.AddRange(Items);
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
            // Reset Ephemeral keys.
            Settings.EphemeralBlockCipherKey = null;
            Settings.EphemeralBroadcastBlockCipherKey = null;
            Settings.EphemeralAuthenticationKey = null;
            Settings.EphemeralKek = null;
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
                AssignedAssociation = null;
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
                    Settings.Cipher.Security = (byte)Security.None;
                }
            }
            dataReceived = DateTime.MinValue;
            Settings.GbtCount = 0;
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

        private byte[] AddPduToFrame(Command cmd, byte frame, GXByteBuffer data, bool final)
        {
            byte[] reply;
            switch (InterfaceType)
            {
                case InterfaceType.WRAPPER:
                    reply = GXDLMS.GetWrapperFrame(Settings, cmd, data);
                    break;
                case InterfaceType.HDLC:
                case InterfaceType.HdlcWithModeE:
                    reply = GXDLMS.GetHdlcFrame(Settings, frame, data, final);
                    break;
                case InterfaceType.Plc:
                case InterfaceType.PlcHdlc:
                    reply = GXDLMS.GetMacFrame(Settings, frame, 0, data);
                    break;
                case InterfaceType.PDU:
                    reply = data.Array();
                    break;
                default:
                    throw new Exception("Unknown interface type " + InterfaceType);
            }
            return reply;
        }

        /// <summary>
        /// Find IEC frame. Sometimes there are extra bytes or multiple packets on the data so they are removed.
        /// </summary>
        /// <returns></returns>
        private bool GetIecPacket()
        {
            if (receivedData.Size < 5)
            {
                return false;
            }
            int eop = -1;
            int bop = -1;
            //Find EOP.
            for (int pos = receivedData.Size - 2; pos != 2; --pos)
            {
                if (receivedData.GetUInt8(pos) == 0x0D &&
                    receivedData.GetUInt8(pos + 1) == 0x0A)
                {
                    eop = pos;
                    break;
                }
            }
            if (eop == -1)
            {
                return false;
            }
            //Find BOP
            byte ch;
            for (int pos = eop - 1; pos != -1; --pos)
            {
                ch = receivedData.GetUInt8(pos);
                if (ch == 6 ||
                    (pos + 2 < receivedData.Size && ch == '/' &&
                        receivedData.GetUInt8(pos + 1) == '?' &&
                        receivedData.GetUInt8(pos + 2) == '!'))
                {
                    bop = pos;
                    break;
                }
            }
            if (bop == -1)
            {
                return false;
            }
            receivedData.Position = bop;
            return true;
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
                    //If using optical probe.
                    if (Settings.InterfaceType == InterfaceType.HdlcWithModeE)
                    {
                        if (Settings.Connected == ConnectionState.None)
                        {
                            //If IEC packet not found.
                            if (!GetIecPacket())
                            {
                                return;
                            }
                            if (receivedData.GetUInt8(receivedData.Position) == 6)
                            {
                                //User changes the baud rate.
                                //Only Mode E is allowed.
                                if (receivedData.GetUInt8(receivedData.Position + 1) != 0x32 || receivedData.GetUInt8(receivedData.Position + 3) != 0x32)
                                {
                                    //Return error.
                                }
                                BaudRate baudrate = (BaudRate)receivedData.GetUInt8(receivedData.Position + 2) - '0';
                                if (baudrate > LocalPortSetup.ProposedBaudrate)
                                {
                                    baudrate = LocalPortSetup.ProposedBaudrate;
                                }
                                receivedData.Clear();
                                //Return used baud rate.
                                Settings.Connected = ConnectionState.Iec;
                                //"2" //(HDLC protocol procedure) (Binary mode)
                                //Set mode E.
                                sr.Reply = new byte[] { 0x06,
                                    //"2" HDLC protocol procedure (Mode E)
                                    (byte)'2', 
                                    //Send Baud rate character
                                    (byte)('0' + baudrate), 
                                    //Mode control character
                                    (byte)'2', 13, 10 };
                                //Change the baud rate.
                                sr.NewBaudRate = 300 << (int)baudrate;
                                Settings.Connected = ConnectionState.Iec;
                            }
                            else if (receivedData.GetUInt8(receivedData.Position) == '/')
                            {
                                string meterAddress = ASCIIEncoding.ASCII.GetString(receivedData.SubArray(receivedData.Position + 3, receivedData.Available - 5));
                                //If meter address is wrong.
                                if (meterAddress != "" && meterAddress != LocalPortSetup.DeviceAddress)
                                {
                                    receivedData.Clear();
                                    return;
                                }
                                receivedData.Clear();
                                receivedData.SetUInt8((byte)'/');
                                //Add flag ID.
                                receivedData.Set(ASCIIEncoding.ASCII.GetBytes(flaID));
                                //Add proposed baud rate.
                                receivedData.SetUInt8('0' + LocalPortSetup.ProposedBaudrate);
                                //Add device address.
                                receivedData.Add(LocalPortSetup.DeviceAddress);
                                receivedData.Add("\r\n");
                                sr.Reply = receivedData.Array();
                                receivedData.Clear();
                            }
                            return;
                        }
                    }
                    try
                    {
                        GXDLMS.GetData(Settings, receivedData, info, null);
                    }
                    catch (GXDLMSExceptionResponse ex)
                    {
                        dataReceived = DateTime.Now;
                        receivedData.Size = 0;
                        sr.Reply = ReportExceptionResponse(ex);
                        info.Clear();
                        return;
                    }
                    catch (Exception)
                    {
                        dataReceived = DateTime.Now;
                        receivedData.Size = 0;
                        if (info.Command == Command.GeneralSigning)
                        {
                            sr.Reply = ReportExceptionResponse(new GXDLMSExceptionResponse(ExceptionStateError.ServiceNotAllowed, ExceptionServiceError.DecipheringError, 0));
                        }
                        else
                        {
                            sr.Reply = ReportError(info.Command, ErrorCode.HardwareFault);
                        }
                        info.Clear();
                        return;
                    }
                    //If all data is not received yet.
                    if (!info.IsComplete)
                    {
                        return;
                    }
                    receivedData.Clear();
                    sr.Command = info.Command;
                    if (info.Command == Command.DisconnectRequest && Settings.Connected == ConnectionState.None)
                    {
                        // Check is data send to this server.
                        if (IsTarget(Settings.ServerAddress,
                                      Settings.ClientAddress))
                        {
                            sr.Reply = GXDLMS.GetHdlcFrame(Settings, (byte)Command.DisconnectMode, replyData);
                        }
                        info.Clear();
                        return;
                    }
                    if ((first || info.Command == Command.Snrm ||
                        (Settings.InterfaceType == InterfaceType.WRAPPER && info.Command == Command.Aarq)) &&
                        Settings.InterfaceType != InterfaceType.PDU)
                    {
                        // Check is data send to this server.
                        if (!IsTarget(Settings.ServerAddress,
                                      Settings.ClientAddress))
                        {
                            info.Clear();
                            Settings.ClientAddress = Settings.ServerAddress = 0;
                            return;
                        }
                    }
                    //If the next frame is request from the client.
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
                                UpdateDefaultBaudRate(sr);
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
                    if (sr.Command != Command.None)
                    {
                        info.Command = sr.Command;
                    }
                    else
                    {
                        //If GBT is streaming.
                        info.Command = Command.GeneralBlockTransfer;
                    }
                }
                try
                {
                    sr.Reply = HandleCommand(info.Command, info.Data, sr, info.CipheredCommand, true);
                }
                catch (Exception)
                {
                    receivedData.Size = 0;
                    if (InterfaceType == InterfaceType.HDLC || InterfaceType == InterfaceType.HdlcWithModeE)
                    {
                        sr.Reply = GXDLMS.GetHdlcFrame(Settings, (byte)Command.UnacceptableFrame, replyData);
                    }
                }
                info.Clear();
                dataReceived = DateTime.Now;
                // Save command if there is more data available.
                // This is needed when Windows size is bigger than one.
                if ((sr.MoreData & RequestTypes.Frame) != 0)
                {
                    info.Command = sr.Command;
                    info.MoreData |= RequestTypes.Frame;
                }
            }
            catch (GXDLMSConfirmedServiceError e)
            {
                Debug.WriteLine(e.ToString());
                replyData.Clear();
                replyData.Set(GenerateConfirmedServiceError(e.ConfirmedServiceError, e.ServiceError, e.ServiceErrorValue));
                info.Clear();
                sr.Reply = AddPduToFrame(info.Command, 0, replyData, true);
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
                        UpdateDefaultBaudRate(sr);
                    }
                }
            }
        }

        private void UpdateDefaultBaudRate(GXServerReply sr)
        {
            if (Settings.InterfaceType == InterfaceType.HdlcWithModeE)
            {
                sr.NewBaudRate = 300 << (int)LocalPortSetup.DefaultBaudrate;
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


        /// <summary>
        /// Handle received command.
        /// </summary>
        /// <param name="cmd">Command</param>
        /// <param name="data">Received data.</param>
        /// <param name="sr"></param>
        /// <param name="cipheredCommand">Ciphered command.</param>
        /// <param name="addFrame">Frame is not add when GBT is used.</param>
        /// <returns></returns>
        private byte[] HandleCommand(
            Command cmd,
            GXByteBuffer data,
            GXServerReply sr,
            Command cipheredCommand,
            bool addFrame)
        {
            byte frame = 0;
            if (GXDLMS.UseHdlc(Settings.InterfaceType) && replyData.Size != 0)
            {
                //Get next frame.
                frame = Settings.NextSend(false);
            }
            switch (cmd)
            {
                case Command.AccessRequest:
                    if (data.Size != 0)
                    {
                        GXDLMSLNCommandHandler.HandleAccessRequest(Settings, this, data, replyData, null, cipheredCommand);
                    }
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
                    if (data.Size != 0)
                    {
                        GXDLMSLNCommandHandler.HandleMethodRequest(Settings, this, data, sr.ConnectionInfo, replyData, null, cipheredCommand);
                    }
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
                        if ((Settings.Connected & ConnectionState.Dlms) != 0)
                        {
                            Disconnected(sr.ConnectionInfo);
                        }
                        Settings.Connected = ConnectionState.None;
                        UpdateDefaultBaudRate(sr);
                    }
                    frame = (byte)Command.Ua;
                    break;
                case Command.GeneralBlockTransfer:
                    if (!HandleGeneralBlockTransfer(data, sr, info.CipheredCommand))
                    {
                        return null;
                    }
                    break;
                case Command.DiscoverRequest:
                    Settings.Plc.ParseDiscoverRequest(data);
                    bool newMeter = Settings.Plc.MacSourceAddress == 0xFFE && Settings.Plc.MacDestinationAddress == 0xFFF;
                    return Settings.Plc.DiscoverReport(Settings.Plc.SystemTitle, newMeter);
                case Command.RegisterRequest:
                    Settings.Plc.ParseRegisterRequest(data);
                    return Settings.Plc.DiscoverReport(Settings.Plc.SystemTitle, false);
                case Command.GeneralSigning:
                    break;
                case Command.PingRequest:
                    break;
                case Command.None:
                    //Get next frame.
                    break;
                default:
                    throw new Exception("Invalid command: " + (int)cmd);
            }
            byte[] reply;
            if (addFrame)
            {
                bool final = true;
                if ((Settings.InterfaceType == InterfaceType.HDLC ||
                    Settings.InterfaceType == InterfaceType.HdlcWithModeE) &&
                    replyData.Available > Settings.Hdlc.MaxInfoTX)
                {
                    sr.MoreData |= RequestTypes.Frame;
                    if (sr.HdlcWindowCount == 0 && Settings.Hdlc.WindowSizeTX != 1)
                    {
                        sr.HdlcWindowCount = Settings.Hdlc.WindowSizeTX;
                        if (frame == 0)
                        {
                            frame = Settings.NextSend(true);
                        }
                    }
                    if (sr.HdlcWindowCount != 0)
                    {
                        --sr.HdlcWindowCount;
                    }
                    //Reset final bit.
                    if (sr.HdlcWindowCount != 0)
                    {
                        final = false;
                    }
                }
                else
                {
                    sr.MoreData &= ~RequestTypes.Frame;
                    sr.HdlcWindowCount = 0;
                }
                sr.GbtCount = Settings.GbtCount;
                reply = AddPduToFrame(cmd, frame, replyData, final);
            }
            else
            {
                reply = null;
            }
            if (cmd == Command.DisconnectRequest ||
                (cmd == Command.ReleaseRequest && !GXDLMS.UseHdlc(InterfaceType)))
            {
                Reset();
            }
            return reply;
        }

        public byte[] CustomFrameRequest(Command command, GXByteBuffer data)
        {
            GXByteBuffer tmp = new GXByteBuffer();
            //If gateway is used.
            if (Settings.Gateway != null)
            {
                tmp.SetUInt8(Command.GatewayResponse);
                tmp.SetUInt8(Settings.Gateway.NetworkId);
                tmp.SetUInt8((byte)Settings.Gateway.PhysicalDeviceAddress.Length);
                tmp.Set(Settings.Gateway.PhysicalDeviceAddress);
            }
            tmp.Set(data);
            return AddPduToFrame(command, 0, tmp, true);
        }

        private bool HandleGeneralBlockTransfer(GXByteBuffer data, GXServerReply sr, Command cipheredCommand)
        {
            //If GBT data is in multiple frames.
            if (replyData.Available != 0)
            {
                return true;
            }
            //Next PDU is not generated streaming is on progress.
            if ((sr.MoreData & RequestTypes.Frame) == 0)
            {
                byte bc = 0;
                UInt16 blockNumberAck = 0, blockNumber = 0;
                if (!sr.IsStreaming && data.Available != 0)
                {
                    //BlockControl
                    bc = data.GetUInt8();
                    //Block number.
                    blockNumber = data.GetUInt16();
                    //Block number acknowledged.
                    blockNumberAck = data.GetUInt16();
                    int len = GXCommon.GetObjectCount(data);
                    if (len > data.Size - data.Position)
                    {
                        replyData.Set(GXDLMSServer.GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                        ServiceError.Service, (byte)Service.Unsupported));
                    }
                }
                if (transaction != null)
                {
                    if (transaction.command == Command.GetRequest || transaction.command == Command.MethodResponse)
                    {
                        // Get request for next data block
                        if (sr.GbtCount == 0)
                        {
                            ++Settings.BlockNumberAck;
                            sr.GbtCount = (byte)(bc & 0x3F);
                        }
                        if (transaction.command == Command.GetRequest)
                        {
                            GXDLMSLNCommandHandler.GetRequestNextDataBlock(Settings, 0, this, data, replyData, null, true, cipheredCommand);
                        }
                        else
                        {
                            GXDLMSLNCommandHandler.MethodRequestNextDataBlock(Settings, 0, this, data, replyData, null, true, cipheredCommand);
                        }
                        if (sr.GbtCount != 0)
                        {
                            --sr.GbtCount;
                        }
                        if (transaction == null)
                        {
                            sr.GbtCount = 0;
                        }
                        //Save server GBT window size to settings because sr is lost.
                        if (Settings.IsServer)
                        {
                            Settings.GbtCount = sr.GbtCount;
                        }
                    }
                    else
                    {
                        transaction.data.Set(data);
                        //Send ACK.
                        bool igonoreAck = (bc & 0x40) != 0 && (blockNumberAck * GBTWindowSize) + 1 > blockNumber;
                        UInt16 windowSize = Settings.GbtWindowSize;
                        UInt16 bn = (UInt16)Settings.BlockIndex;
                        if ((bc & 0x80) != 0)
                        {
                            Command cmd = transaction.command;
                            GXByteBuffer tmp = transaction.data;
                            transaction = null;
                            replyData.Clear();
                            replyData.Add(HandleCommand(cmd, tmp, sr, cipheredCommand, false));
                            return true;
                        }
                        if (igonoreAck)
                        {
                            return false;
                        }
                        replyData.Clear();
                        if (GXDLMS.UseHdlc(Settings.InterfaceType))
                        {
                            replyData.Set(GXCommon.LLCReplyBytes);
                        }
                        replyData.SetUInt8(Command.GeneralBlockTransfer);
                        replyData.SetUInt8((byte)(0x80 | windowSize));
                        ++Settings.BlockIndex;
                        replyData.SetUInt16(bn);
                        replyData.SetUInt16(blockNumber);
                        replyData.SetUInt8(0);
                    }
                }
                else if (data.Available != 0)
                {
                    transaction = new GXDLMSLongTransaction(null, (Command)data.GetUInt8(), data);
                    //Send ACK.
                    bool igonoreAck = (bc & 0x40) != 0 && (blockNumberAck * GBTWindowSize) + 1 > blockNumber;
                    UInt16 windowSize = Settings.GbtWindowSize;
                    UInt16 bn = (UInt16)Settings.BlockIndex;
                    if ((bc & 0x80) != 0)
                    {
                        replyData.Clear();
                        Command cmd = transaction.command;
                        GXByteBuffer tmp = transaction.data;
                        transaction = null;
                        replyData.Add(HandleCommand(cmd, tmp, sr, cipheredCommand, false));
                        return true;
                    }
                    if (igonoreAck)
                    {
                        return false;
                    }
                    replyData.Clear();
                    if (GXDLMS.UseHdlc(Settings.InterfaceType))
                    {
                        replyData.Set(GXCommon.LLCReplyBytes);
                    }
                    replyData.SetUInt8(Command.GeneralBlockTransfer);
                    replyData.SetUInt8((byte)(0x80 | windowSize));
                    ++Settings.BlockIndex;
                    replyData.SetUInt16(bn);
                    replyData.SetUInt16(blockNumber);
                    replyData.SetUInt8(0);
                }
            }
            return true;
        }

        protected byte[] ReportExceptionResponse(GXDLMSExceptionResponse ex)
        {
            GXByteBuffer bb = new GXByteBuffer();
            if (GXDLMS.UseHdlc(Settings.InterfaceType))
            {
                bb.Set(GXCommon.LLCReplyBytes);
            }
            bb.SetUInt8(ExceptionStateError.ServiceNotAllowed);
            bb.SetUInt8(ex.ExceptionServiceError);
            if (ex.ExceptionServiceError == ExceptionServiceError.InvocationCounterError)
            {
                replyData.SetUInt32((UInt32)Settings.ExpectedInvocationCounter);
            }
            if (Settings.UseLogicalNameReferencing)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.ExceptionResponse, 1, bb, null, 0xFF, info.CipheredCommand);
                GXDLMS.GetLNPdu(p, replyData);
            }
            else
            {
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.ExceptionResponse, 1, 0, null, bb);
                GXDLMS.GetSNPdu(p, replyData);
            }
            return AddPduToFrame(Command.ExceptionResponse, 0, replyData, true);
        }

        protected byte[] ReportError(Command cmd, ErrorCode error)
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
                case Command.GeneralSigning:
                case Command.GeneralCiphering:
                case Command.GeneralDedCiphering:
                case Command.GeneralGloCiphering:
                    cmd = Command.ExceptionResponse;
                    break;
                default:
                    //Return HW error and close connection..
                    break;
            }
            if (Settings.UseLogicalNameReferencing)
            {
                GXDLMS.GetLNPdu(new GXDLMSLNParameters(Settings, 0, cmd, 1, null, null, (byte)error, info.CipheredCommand), replyData);
            }
            else
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8(error);
                GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, cmd, 1, (byte)error, null, bb);
                GXDLMS.GetSNPdu(p, replyData);
            }
            return AddPduToFrame(cmd, 0, replyData, true);
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
            // Reset settings for wrapper and raw PDU.
            if (Settings.InterfaceType == InterfaceType.WRAPPER ||
                Settings.InterfaceType == InterfaceType.PDU)
            {
                Reset(true);
            }
            object ret;
            byte name;
            try
            {
                ret = GXAPDU.ParsePDU(Settings, Settings.Cipher, data, null);
                if (ret is ExceptionServiceError e)
                {
                    if (GXDLMS.UseHdlc(Settings.InterfaceType))
                    {
                        replyData.Set(GXCommon.LLCReplyBytes);
                    }
                    replyData.SetUInt8(Command.ExceptionResponse);
                    replyData.SetUInt8(ExceptionStateError.ServiceNotAllowed);
                    replyData.SetUInt8(e);
                    if (e == ExceptionServiceError.InvocationCounterError)
                    {
                        replyData.SetUInt32((UInt32)Settings.ExpectedInvocationCounter);
                    }
                    return;
                }
                if (!(ret is AcseServiceProvider))
                {
                    if (ret is ApplicationContextName)
                    {
                        name = (byte)ret;
                        result = AssociationResult.PermanentRejected;
                        ret = SourceDiagnostic.ApplicationContextNameNotSupported;
                    }
                    else if (Settings.NegotiatedConformance == Conformance.None)
                    {
                        result = AssociationResult.PermanentRejected;
                        ret = SourceDiagnostic.NoReasonGiven;
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
                        ret = SourceDiagnostic.NoReasonGiven;
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
                        ret = SourceDiagnostic.NoReasonGiven;
                        error = new GXByteBuffer();
                        error.SetUInt8(0xE);
                        error.SetUInt8(ConfirmedServiceError.InitiateError);
                        error.SetUInt8(ServiceError.Initiate);
                        error.SetUInt8(Initiate.DlmsVersionTooLow);
                    }
                    else if ((SourceDiagnostic)ret != SourceDiagnostic.None)
                    {
                        result = AssociationResult.PermanentRejected;
                        ret = SourceDiagnostic.ApplicationContextNameNotSupported;
                        InvalidConnection(connectionInfo);
                    }
                    else
                    {
                        if (AssignedAssociation != null &&
                            AssignedAssociation.AuthenticationMechanismName.MechanismId != Settings.Authentication)
                        {
                                ret = SourceDiagnostic.ApplicationContextNameNotSupported;
                        }
                        else
                        {
                            ret = ValidateAuthentication(Settings.Authentication, Settings.Password);
                        }
                        if ((SourceDiagnostic)ret != SourceDiagnostic.None)
                        {
                            result = AssociationResult.PermanentRejected;
                            InvalidConnection(connectionInfo);
                        }
                        else if (Settings.Authentication > Authentication.Low)
                        {
                            // If High authentication is used.
                            result = AssociationResult.Accepted;
                            ret = SourceDiagnostic.AuthenticationRequired;
                            if (UseLogicalNameReferencing)
                            {
                                GXDLMSAssociationLogicalName ln = AssignedAssociation;
                                if (ln == null)
                                {
                                    ln = (GXDLMSAssociationLogicalName)Items.FindByLN(ObjectType.AssociationLogicalName, "0.0.40.0.0.255");
                                    if (ln == null)
                                    {
                                        ln = (GXDLMSAssociationLogicalName)NotifyFindObject(ObjectType.AssociationLogicalName, 0, "0.0.40.0.0.255");
                                    }
                                }
                                if (ln != null)
                                {
                                    ln.AssociationStatus = AssociationStatus.AssociationPending;
                                }
                            }
                        }
                        else
                        {
                            if (UseLogicalNameReferencing)
                            {
                                GXDLMSAssociationLogicalName ln = AssignedAssociation;
                                if (ln == null)
                                {
                                    ln = (GXDLMSAssociationLogicalName)Items.FindByLN(ObjectType.AssociationLogicalName, "0.0.40.0.0.255");
                                    if (ln == null)
                                    {
                                        ln = (GXDLMSAssociationLogicalName)NotifyFindObject(ObjectType.AssociationLogicalName, 0, "0.0.40.0.0.255");
                                    }
                                }
                                if (ln != null)
                                {
                                    ln.AssociationStatus = AssociationStatus.Associated;
                                }
                            }
                            Connected(connectionInfo);
                            Settings.Connected |= ConnectionState.Dlms;
                        }
                    }
                }
                else if (result == AssociationResult.Accepted && Convert.ToByte(ret) != 0)
                {
                    result = AssociationResult.PermanentRejected;
                }
            }
            catch (GXDLMSException e)
            {
                result = e.Result;
                ret = (SourceDiagnostic)e.Diagnostic;
            }
            if (Settings.Authentication > Authentication.Low && !Settings.UseCustomChallenge)
            {
                // If High authentication is used.
                Settings.StoCChallenge = GXSecure.GenerateChallenge(Settings.Authentication, Settings.ChallengeSize);
            }
            if (GXDLMS.UseHdlc(Settings.InterfaceType))
            {
                replyData.Set(GXCommon.LLCReplyBytes);
            }
            // Generate AARE packet.
            GXAPDU.GenerateAARE(Settings, replyData, result, ret, Settings.Cipher, error, null);
        }

        /// <summary>
        /// Handles release request.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <param name="connectionInfo">Connection info.</param>
        private void HandleReleaseRequest(GXByteBuffer data, GXDLMSConnectionEventArgs connectionInfo)
        {
            //Return error if connection is not established.
            if ((Settings.Connected & ConnectionState.Dlms) == 0)
            {
                replyData.Add(GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                              ServiceError.Service, (byte)Service.Unsupported));
                return;
            }
            if (GXDLMS.UseHdlc(Settings.InterfaceType))
            {
                replyData.Set(0, GXCommon.LLCReplyBytes);
            }
            byte[] tmp = GXAPDU.GetUserInformation(Settings, Settings.Cipher);
            if (Settings.Gateway != null && Settings.Gateway.PhysicalDeviceAddress != null)
            {
                replyData.SetUInt8(Command.GatewayResponse);
                replyData.SetUInt8(Settings.Gateway.NetworkId);
                replyData.SetUInt8((byte)Settings.Gateway.PhysicalDeviceAddress.Length);
                replyData.Set(Settings.Gateway.PhysicalDeviceAddress);
            }

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
            //Initialize default settings.
            if (Hdlc != null)
            {
                Settings.Hdlc.Update(Hdlc);
            }
            else
            {
                Settings.Hdlc.MaxInfoRX = GXDLMSLimitsDefault.DefaultMaxInfoRX;
                Settings.Hdlc.MaxInfoTX = GXDLMSLimitsDefault.DefaultMaxInfoTX;
                Settings.Hdlc.WindowSizeRX = GXDLMSLimitsDefault.DefaultWindowSizeRX;
                Settings.Hdlc.WindowSizeTX = GXDLMSLimitsDefault.DefaultWindowSizeTX;
            }
            GXDLMS.ParseSnrmUaResponse(data, Settings);
            Reset(true);
            if (Hdlc != null)
            {
                //If client wants send larger HDLC frames what meter accepts.
                if (Settings.Hdlc.MaxInfoTX > Hdlc.MaximumInfoLengthReceive)
                {
                    Settings.Hdlc.MaxInfoTX = (UInt16)Hdlc.MaximumInfoLengthReceive;
                }
                //If client wants receive larger HDLC frames what meter accepts.
                if (Settings.Hdlc.MaxInfoRX > Hdlc.MaximumInfoLengthTransmit)
                {
                    Settings.Hdlc.MaxInfoRX = (UInt16)Hdlc.MaximumInfoLengthTransmit;
                }
                //If client asks higher window size what meter accepts.
                if (Settings.Hdlc.WindowSizeTX > Hdlc.WindowSizeTransmit)
                {
                    Settings.Hdlc.WindowSizeTX = (byte)Hdlc.WindowSizeReceive;
                }
                //If client asks higher window size what meter accepts.
                if (Settings.Hdlc.WindowSizeRX > Hdlc.WindowSizeReceive)
                {
                    Settings.Hdlc.WindowSizeRX = (byte)Hdlc.WindowSizeTransmit;
                }
            }
            replyData.SetUInt8(0x81); // FromatID
            replyData.SetUInt8(0x80); // GroupID
            replyData.SetUInt8(0); // Length
            replyData.SetUInt8(HDLCInfo.MaxInfoTX);
            GXDLMS.AppendHdlcParameter(replyData, Settings.Hdlc.MaxInfoTX);
            replyData.SetUInt8(HDLCInfo.MaxInfoRX);
            GXDLMS.AppendHdlcParameter(replyData, Settings.Hdlc.MaxInfoRX);
            replyData.SetUInt8(HDLCInfo.WindowSizeTX);
            replyData.SetUInt8(4);
            replyData.SetUInt32(Settings.Hdlc.WindowSizeTX);
            replyData.SetUInt8(HDLCInfo.WindowSizeRX);
            replyData.SetUInt8(4);
            replyData.SetUInt32(Settings.Hdlc.WindowSizeRX);
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
            if ((Settings.Connected & ConnectionState.Hdlc) == 0)
            {
                replyData.Add(GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                              ServiceError.Service, (byte)Service.Unsupported));
                return;
            }

            replyData.SetUInt8(0x81); // FromatID
            replyData.SetUInt8(0x80); // GroupID
            replyData.SetUInt8(0); // Length

            replyData.SetUInt8(HDLCInfo.MaxInfoTX);
            GXDLMS.AppendHdlcParameter(replyData, Settings.Hdlc.MaxInfoTX);

            replyData.SetUInt8(HDLCInfo.MaxInfoRX);
            GXDLMS.AppendHdlcParameter(replyData, Settings.Hdlc.MaxInfoRX);

            replyData.SetUInt8(HDLCInfo.WindowSizeTX);
            replyData.SetUInt8(4);
            replyData.SetUInt32(Settings.Hdlc.WindowSizeTX);

            replyData.SetUInt8(HDLCInfo.WindowSizeRX);
            replyData.SetUInt8(4);
            replyData.SetUInt32(Settings.Hdlc.WindowSizeRX);

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
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.DataNotification, 0, null, new GXByteBuffer(data), 0xff, Command.None);
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
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, 0, Command.DataNotification, 0, null, data, 0xff, Command.None);
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
            foreach (GXDLMSProfileGeneric it in Items.GetObjects(ObjectType.ProfileGeneric))
            {
                if (it.CapturePeriod != 0)
                {
                    UInt32 seconds = (UInt32)(now - now.Date).TotalSeconds;
                    seconds = it.CapturePeriod - (seconds % it.CapturePeriod);
                    if (seconds == 0)
                    {
                        seconds = it.CapturePeriod;
                    }
                    if (next > now.AddSeconds(seconds))
                    {
                        next = now.AddSeconds(seconds);
                    }
                    if (!ExecutionTimes.ContainsKey(it) ||
                        ExecutionTimes[it].AddSeconds(it.CapturePeriod) <= now)
                    {
                        it.Capture(this);
                        ExecutionTimes[it] = now.AddSeconds(seconds).AddSeconds(-it.CapturePeriod);
                    }
                }
            }

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

        /// <summary>
        /// Is value of the object changed with action instead of write.
        /// </summary>
        /// <param name="objectType">Object type.</param>
        /// <param name="methodIndex">Method index.</param>
        /// <returns>Returns true if object is modified with action.</returns>
        public bool IsChangedWithAction(ObjectType objectType, int methodIndex)
        {
            if ((objectType == ObjectType.AssociationLogicalName && methodIndex != 1) ||
                (objectType == ObjectType.SecuritySetup && (methodIndex == 1 || methodIndex == 4 || methodIndex == 6 || methodIndex == 7 || methodIndex == 8)))
            {
                return true;
            }
            //SAP assignment is added or removed.
            return objectType == ObjectType.SapAssignment ||
            //Connection state is changed.
            objectType == ObjectType.DisconnectControl ||
            objectType == ObjectType.SpecialDaysTable ||
            objectType == ObjectType.RegisterActivation;
        }

        internal void InvokePreRead(ValueEventArgs[] args)
        {
            PreRead(args);
        }
        internal void InvokePostRead(ValueEventArgs[] args)
        {
            PostRead(args);
        }
    }
}