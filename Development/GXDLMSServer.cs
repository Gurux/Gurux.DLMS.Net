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
using System.ComponentModel;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Internal;
using Gurux.DLMS.ManufacturerSettings;
using System.Reflection;
using System.Threading;
#if !WINDOWS_UWP
using System.Security.Cryptography;
#endif
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;
using System.Diagnostics;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMSServer implements methods to implement DLMS/COSEM meter/proxy.
    /// </summary>
    public abstract class GXDLMSServer
    {
        /// <summary>
        /// DLMS Settings. 
        /// </summary>
        internal GXDLMSSettings Settings;

        private readonly GXReplyData info = new GXReplyData();
        /// <summary>
        /// Received data.
        /// </summary>
        private GXByteBuffer receivedData = new GXByteBuffer();
        /// <summary>
        /// Reply data.
        /// </summary>
        private GXByteBuffer replyData = new GXByteBuffer();

        internal GXDLMSLongTransaction transaction;

        bool Initialized = false;

        /// <summary>
        /// HDLC settings.
        /// </summary>
        GXDLMSHdlcSetup hdlcSetup = null;

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
        /// <param name="sender">Sender.</param>
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
        /// Constructor.
        /// </summary>
        public GXDLMSServer(bool logicalNameReferencing, InterfaceType type)
        {
            Settings = new GXDLMSSettings(true);
            Settings.UseLogicalNameReferencing = logicalNameReferencing;
            Reset();
            this.InterfaceType = type;
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical name settings.</param>
        /// <param name="type">Interface type.</param>
        public GXDLMSServer(GXDLMSAssociationLogicalName ln, InterfaceType type)
        {
            Settings = new GXDLMSSettings(true);
            Settings.UseLogicalNameReferencing = true;
            Reset();
            Settings.Objects.Add(ln);
            this.InterfaceType = type;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="type">Interface type.</param>
        public GXDLMSServer(GXDLMSAssociationShortName sn, InterfaceType type)
        {
            Settings = new GXDLMSSettings(true);
            Settings.UseLogicalNameReferencing = false;
            Reset();
            Settings.Objects.Add(sn);
            this.InterfaceType = type;
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
        /// Information from the connection size that server can handle.
        /// </summary>
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
                else if (it is GXDLMSAssociationShortName && !this.UseLogicalNameReferencing)
                {
                    if ((it as GXDLMSAssociationShortName).ObjectList.Count == 0)
                    {
                        (it as GXDLMSAssociationShortName).ObjectList.AddRange(this.Items);
                    }
                    association = true;
                }
                else if (it is GXDLMSAssociationLogicalName && this.UseLogicalNameReferencing)
                {
                    if ((it as GXDLMSAssociationLogicalName).ObjectList.Count == 0)
                    {
                        (it as GXDLMSAssociationLogicalName).ObjectList.AddRange(this.Items);
                    }
                    association = true;
                }
                else if (it is GXDLMSHdlcSetup)
                {
                    hdlcSetup = it as GXDLMSHdlcSetup;
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
                    GXDLMSAssociationLogicalName it = new GXDLMSAssociationLogicalName();
                    it.ObjectList = this.Items;
                    Items.Add(it);
                }
                else
                {
                    GXDLMSAssociationShortName it = new GXDLMSAssociationShortName();
                    it.ObjectList = this.Items;
                    Items.Add(it);
                }
            }
            //Arrange items by Short Name.
            int sn = 0xA0;
            int offset, count;
            if (!this.UseLogicalNameReferencing)
            {
                foreach (GXDLMSObject it in Items)
                {
                    //Generate Short Name if not given.
                    if (it.ShortName == 0)
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
            transaction = null;
            Settings.BlockIndex = 1;
            Settings.Count = Settings.Index = 0;
            Settings.Connected = false;
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
            Settings.IsAuthenticationRequired = false;
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
        }

        ///<summary>
        /// Handles client request.
        /// </summary>
        ///<param name="buff">
        /// Received data from the client. </param>
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
            if (buff == null || buff.Length == 0)
            {
                return null;
            }
            if (!Initialized)
            {
                throw new Exception("Server not Initialized.");
            }
            try
            {
                receivedData.Set(buff);
                bool first = Settings.ServerAddress == 0 && Settings.ClientAddress == 0;
                GXDLMS.GetData(Settings, receivedData, info);
                //If all data is not received yet.
                if (!info.IsComplete)
                {
                    return null;
                }
                receivedData.Clear();
                if (first)
                {
                    // Check is data send to this server.
                    if (!IsTarget(Settings.ServerAddress,
                                  Settings.ClientAddress))
                    {
                        info.Clear();
                        return null;
                    }
                }

                //If client want next frame.
                if ((info.MoreData & RequestTypes.Frame) == RequestTypes.Frame)
                {
                    return GXDLMS.GetHdlcFrame(Settings, Settings.ReceiverReady(), replyData);
                }
                //Update command if transaction and next frame is asked.
                if (info.Command == Command.None)
                {
                    if (transaction != null)
                    {
                        info.Command = transaction.command;
                    }
                }
                //Check inactivity time out.
                if (hdlcSetup != null)
                {
                    if (info.Command == Command.Snrm)
                    {
                        dataReceived = DateTime.Now;
                    }
                    else
                    {
                        int elapsed = (int)(DateTime.Now - dataReceived).TotalSeconds;
                        //If inactivity time out is elapsed.
                        if (elapsed >= hdlcSetup.InactivityTimeout)
                        {
                            Reset();
                            dataReceived = DateTime.MinValue;
                            return null;
                        }
                        dataReceived = DateTime.Now;
                    }
                }

                byte[] reply = HandleCommand(info.Command, info.Data, connectionInfo);
                info.Clear();
                return reply;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                if (info.Command != Command.None)
                {
                    byte[] reply = ReportError(info.Command, ErrorCode.HardwareFault);
                    info.Clear();
                    return reply;
                }
                else
                {
                    Reset();
                    if (Settings.Connected)
                    {
                        Settings.Connected = false;
                        Disconnected(connectionInfo);
                    }
                    return null;
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
        private byte[] HandleCommand(Command cmd, GXByteBuffer data, GXDLMSConnectionEventArgs connectionInfo)
        {
            byte frame = 0;
            switch (cmd)
            {
                case Command.AccessRequest:
                    GXDLMSLNCommandHandler.HandleAccessRequest(Settings, this, data, replyData, null);
                    break;
                case Command.SetRequest:
                    GXDLMSLNCommandHandler.HandleSetRequest(Settings, this, data, replyData, null);
                    break;
                case Command.WriteRequest:
                    GXDLMSSNCommandHandler.HandleWriteRequest(Settings, this, data, replyData, null);
                    break;
                case Command.GetRequest:
                    if (data.Size != 0)
                    {
                        GXDLMSLNCommandHandler.HandleGetRequest(Settings, this, data, replyData, null);
                    }
                    break;
                case Command.ReadRequest:
                    GXDLMSSNCommandHandler.HandleReadRequest(Settings, this, data, replyData, null);
                    break;
                case Command.MethodRequest:
                    GXDLMSLNCommandHandler.HandleMethodRequest(Settings, this, data, connectionInfo, replyData, null);
                    break;
                case Command.Snrm:
                    HandleSnrmRequest();
                    frame = (byte)Command.Ua;
                    break;
                case Command.Aarq:
                    HandleAarqRequest(data, connectionInfo);
                    break;
                case Command.ReleaseRequest:
                case Command.DisconnectRequest:
                    GenerateDisconnectRequest();
                    Settings.Connected = false;
                    Disconnected(connectionInfo);
                    frame = (byte)Command.Ua;
                    break;
                case Command.None:
                    //Get next frame.
                    break;
                default:
                    Debug.WriteLine("Invalid command: " + (int)cmd);
                    break;
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
            return reply;
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
                GXDLMS.GetLNPdu(new GXDLMSLNParameters(Settings, 0, cmd, 1, null, null, (byte)error), replyData);
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
            AssociationResult result = AssociationResult.Accepted;
            Settings.CtoSChallenge = null;
            if (!Settings.UseCustomChallenge)
            {
                Settings.StoCChallenge = null;
            }
            // Reset settings for wrapper.
            if (Settings.InterfaceType == InterfaceType.WRAPPER)
            {
                Reset(true);
            }
            SourceDiagnostic diagnostic = GXAPDU.ParsePDU(Settings, Settings.Cipher, data, null);
            if (diagnostic != SourceDiagnostic.None)
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
                    if (!Settings.UseCustomChallenge)
                    {
                        Settings.StoCChallenge = GXSecure.GenerateChallenge(Settings.Authentication);
                    }
                    result = AssociationResult.Accepted;
                    diagnostic = SourceDiagnostic.AuthenticationRequired;
                }
                else
                {
                    Connected(connectionInfo);
                    Settings.Connected = true;
                }
            }
            Settings.IsAuthenticationRequired = diagnostic == SourceDiagnostic.AuthenticationRequired;
            if (Settings.InterfaceType == Enums.InterfaceType.HDLC)
            {
                replyData.Set(GXCommon.LLCReplyBytes);
            }
            // Generate AARE packet.
            GXAPDU.GenerateAARE(Settings, replyData, result, diagnostic, Settings.Cipher, null);
        }

        ///<summary>
        ///Parse SNRM Request. If server do not accept client empty byte array is returned.
        /// </summary>
        ///<returns>
        ///Returns returned UA packet.
        ///</returns>
        private void HandleSnrmRequest()
        {
            Reset(true);
            replyData.SetUInt8(0x81); // FromatID
            replyData.SetUInt8(0x80); // GroupID
            replyData.SetUInt8(0); // Length
            replyData.SetUInt8(HDLCInfo.MaxInfoTX);
            replyData.SetUInt8(GXCommon.GetSize(Limits.MaxInfoTX));
            replyData.Add(Limits.MaxInfoTX);
            replyData.SetUInt8(HDLCInfo.MaxInfoRX);
            replyData.SetUInt8(GXCommon.GetSize(Limits.MaxInfoRX));
            replyData.Add(Limits.MaxInfoRX);
            replyData.SetUInt8(HDLCInfo.WindowSizeTX);
            replyData.SetUInt8(GXCommon.GetSize(Limits.WindowSizeTX));
            replyData.Add(Limits.WindowSizeTX);
            replyData.SetUInt8(HDLCInfo.WindowSizeRX);
            replyData.SetUInt8(GXCommon.GetSize(Limits.WindowSizeRX));
            replyData.Add(Limits.WindowSizeRX);
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
            if (!Settings.Connected && !Settings.IsAuthenticationRequired)
            {
                replyData.Add(GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                              ServiceError.Service, (byte)Service.Unsupported));
                return;
            }

            if (this.InterfaceType == InterfaceType.WRAPPER)
            {
                replyData.SetUInt8(0x63);
                replyData.SetUInt8(0x0);
            }
            else
            {
                replyData.SetUInt8(0x81); // FromatID
                replyData.SetUInt8(0x80); // GroupID
                replyData.SetUInt8(0); // Length

                replyData.SetUInt8(HDLCInfo.MaxInfoTX);
                replyData.SetUInt8(GXCommon.GetSize(Limits.MaxInfoTX));
                replyData.Add(Limits.MaxInfoTX);

                replyData.SetUInt8(HDLCInfo.MaxInfoRX);
                replyData.SetUInt8(GXCommon.GetSize(Limits.MaxInfoRX));
                replyData.Add(Limits.MaxInfoRX);

                replyData.SetUInt8(HDLCInfo.WindowSizeTX);
                replyData.SetUInt8(GXCommon.GetSize(Limits.WindowSizeTX));
                replyData.Add(Limits.WindowSizeTX);

                replyData.SetUInt8(HDLCInfo.WindowSizeRX);
                replyData.SetUInt8(GXCommon.GetSize(Limits.WindowSizeRX));
                replyData.Add(Limits.WindowSizeRX);

                int len = replyData.Position - 3;
                replyData.SetUInt8(2, (byte)len); // Length.
            }
        }
    }
}