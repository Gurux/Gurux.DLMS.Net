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
using System.Data;
using System.Threading;
using System.Security.Cryptography;
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

        GXDLMSLongTransaction transaction;

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
        /// Check whether the authentication and password are correct.
        /// </summary>
        /// <param name="authentication">Authentication level.</param>
        /// <param name="password">Password</param>
        /// <returns>Source diagnostic.</returns>
        protected abstract SourceDiagnostic ValidateAuthentication(
                Authentication authentication, byte[] password);

        /// <summary>
        /// Get selected value.
        /// </summary>
        /// <param name="e">Handle get request.</param>
        public abstract void Update(UpdateType type, ValueEventArgs e);      

        /// <summary>
        /// Read selected item.
        /// </summary>
        /// <param name="args">Handled read requests.</param>
        protected abstract void Read(ValueEventArgs[] args);

        /// <summary>
        /// Write selected item.
        /// </summary>
        /// <param name="args">Handled write requests.</param>
        protected abstract void Write(ValueEventArgs[] args);

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
        /// Action is occurred.
        /// </summary>
        /// <param name="args">Handled action requests.</param>
        protected abstract void Action(ValueEventArgs[] args);

        /// <summary>
        /// Constructor.
        /// </summary>        
        public GXDLMSServer()
            : this(false, InterfaceType.HDLC)
        {
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
            private set
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
            private set
            {
                Settings.InterfaceType = value;
            }
        }

        /// <summary>
        /// Gets Logical Name Settings, read from the device. 
        /// </summary>
        public GXDLMSLNSettings LNSettings
        {
            get
            {
                return Settings.LnSettings;
            }
        }

        /// <summary>
        /// Gets Short Name Settings, read from the device.
        /// </summary>
        public GXDLMSSNSettings SNSettings
        {
            get
            {
                return Settings.SnSettings;
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
            bool association = false;
            Initialized = true;
            for (int pos = 0; pos != Items.Count; ++pos)
            {
                GXDLMSObject it = Items[pos];
                if (this.UseLogicalNameReferencing &&
                    (string.IsNullOrEmpty(it.LogicalName) || it.LogicalName.Split('.').Length != 6))
                {
                    throw new Exception("Invalid Logical Name.");
                }
                if (it is GXDLMSProfileGeneric)
                {
                    GXDLMSProfileGeneric pg = it as GXDLMSProfileGeneric;
                    /*
                    if (pg.ProfileEntries < 1)
                    {
                        throw new Exception("Invalid Profile Entries. Profile entries tells amount of rows in the table.");
                    }
                     * */
                    foreach (var obj in pg.CaptureObjects)
                    {
                        if (obj.Value.AttributeIndex < 1)
                        {
                            throw new Exception("Invalid attribute index. SelectedAttributeIndex is not set for " + obj.Key.Name);
                        }
                    }
                    if (pg.CapturePeriod != 0)
                    {
                        GXProfileGenericUpdater p = new GXProfileGenericUpdater(this, pg);
                        Thread thread = new Thread(new ThreadStart(p.UpdateProfileGenericData));
                        thread.IsBackground = true;
                        thread.Start();
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
                        it.ShortName = (ushort) sn;                        
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
        /// <param name="all"></param>
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
                    Settings.Cipher.Security = Security.None;
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
                    return ReportError(info.Command, ErrorCode.HardwareFault);
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
        private byte[] GenerateConfirmedServiceError(ConfirmedServiceError service, ServiceError type, byte code)
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
                case Command.SetRequest:
                    HandleSetRequest(data);
                    break;
                case Command.WriteRequest:
                    HandleWriteRequest(data);
                    break;
                case Command.GetRequest:
                    if (data.Size != 0)
                    {
                        HandleGetRequest(data);
                    }
                    break;
                case Command.ReadRequest:
                    HandleReadRequest(data);
                    break;
                case Command.MethodRequest:
                    HandleMethodRequest(data, connectionInfo);
                    break;
                case Command.Snrm:
                    HandleSnrmRequest();
                    frame = (byte)Command.Ua;
                    break;
                case Command.Aarq:
                    HandleAarqRequest(data, connectionInfo);
                    break;
                case Command.DisconnectRequest:
                case Command.Disc:
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
                GXDLMS.GetLNPdu(new GXDLMSLNParameters(Settings, cmd, 1, null, null, (byte)error), replyData);
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
        /// Handle action request.
        ///</summary>
        ///<param name="Reply">
        /// Received data from the client.
        ///</param>
        ///<returns>
        ///Reply. 
        ///</returns>
        private void HandleMethodRequest(GXByteBuffer data, GXDLMSConnectionEventArgs connectionInfo)
        {
            ErrorCode error = ErrorCode.Ok;
            GXByteBuffer bb = new GXByteBuffer();
            // Get type.
            data.GetUInt8();
            // Get invoke ID and priority.
            data.GetUInt8();
            // CI
            ObjectType ci = (ObjectType)data.GetUInt16();
            byte[] ln = new byte[6];
            data.Get(ln);
            // Attribute Id
            int id = data.GetUInt8();
            // Get parameters.
            object parameters = null;
            if (data.GetUInt8() != 0)
            {
                GXDataInfo info = new GXDataInfo();
                parameters = GXCommon.GetData(data, info);
            }
            GXDLMSObject obj = Settings.Objects.FindByLN(ci, GXDLMSObject.ToLogicalName(ln));
            if (!Settings.Connected && (ci != ObjectType.AssociationLogicalName || id != 1))
            {
                replyData.Add(GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                    ServiceError.Service, (byte)Service.ServiceUnsupported));
                return;
            }

            if (obj == null)
            {
                obj = FindObject(ci, 0, GXDLMSObject.ToLogicalName(ln));
            }
            if (obj == null)
            {
                // Device reports a undefined object.
                error = ErrorCode.UndefinedObject;
            }
            else
            {
                if (obj.GetMethodAccess(id) == MethodAccessMode.NoAccess)
                {
                    error = ErrorCode.ReadWriteDenied;
                }
                else
                {
                    ValueEventArgs e = new ValueEventArgs(Settings, obj, id, 0, parameters);
                    Action(new ValueEventArgs[] { e });
                    byte[] actionReply;
                    if (e.Handled)
                    {
                        actionReply = (byte[])e.Value;
                    }
                    else
                    {
                        actionReply = (obj as IGXDLMSBase).Invoke(Settings, e);
                    }
                    //Set default action reply if not given.
                    if (actionReply != null && e.Error == 0)
                    {
                        //Add return parameters
                        bb.SetUInt8(1);
                        //Add parameters error code.
                        bb.SetUInt8(0);
                        GXCommon.SetData(bb, GXCommon.GetValueType(actionReply), actionReply);
                    }
                    else
                    {
                        error = e.Error;
                        //Add return parameters
                        bb.SetUInt8(0);
                    }
                }
            }

            GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, Command.MethodResponse, 1, null, bb, (byte)error);
            GXDLMS.GetLNPdu(p, replyData);
            //If High level authentication fails.
            if (!Settings.Connected && obj is GXDLMSAssociationLogicalName && id == 1)
            {
                InvalidConnection(connectionInfo);
            }
        }

        ///<summary>
        /// Server reports error.
        ///</summary>
        ///<param name="error">
        /// Error code. </param>
        ///<param name="bb">
        /// Byte buffer where error info is saved.
        ///</param>
        private static void addError(ErrorCode error, GXByteBuffer bb)
        {
            if (error == ErrorCode.Ok)
            {
                bb.SetUInt8(0);
            }
            else
            {
                bb.SetUInt8(1);
                bb.SetUInt8((byte)error);
            }
        }

        private void HandleSetRequest(GXByteBuffer data, byte type, GXDLMSLNParameters p)
        {
            GXDataInfo reply = new GXDataInfo();
            // CI
            ObjectType ci = (ObjectType)data.GetUInt16();
            byte[] ln = new byte[6];
            data.Get(ln);
            // Attribute index.
            int index = data.GetUInt8();
            // Get Access Selection.
            data.GetUInt8();
            if (type == 2)
            {
                p.multipleBlocks = data.GetUInt8() == 0;
                UInt32 blockNumber = data.GetUInt32();
                if (blockNumber != Settings.BlockIndex)
                {
                    Debug.WriteLine("HandleSetRequest failed. Invalid block number. " + Settings.BlockIndex + "/" + blockNumber);
                    p.status = (byte)ErrorCode.DataBlockNumberInvalid;
                    return;
                }
                Settings.IncreaseBlockIndex();
                int size = GXCommon.GetObjectCount(data);
                int realSize = data.Size - data.Position;
                if (size != realSize)
                {
                    Debug.WriteLine("HandleSetRequest failed. Invalid block size.");
                    p.status = (byte)ErrorCode.DataBlockUnavailable;
                    return;
                }
            }
            object value = null;
            if (!p.multipleBlocks)
            {
                Settings.ResetBlockIndex();
                value = GXCommon.GetData(data, reply);
            }

            GXDLMSObject obj = Settings.Objects.FindByLN(ci, GXDLMSObject.ToLogicalName(ln));
            if (obj == null)
            {
                obj = FindObject(ci, 0, GXDLMSObject.ToLogicalName(ln));
            }
            // If target is unknown.
            if (obj == null)
            {
                // Device reports a undefined object.
                p.status = (byte)ErrorCode.UndefinedObject;
            }
            else
            {
                AccessMode am = obj.GetAccess(index);
                // If write is denied.
                if (am != AccessMode.Write && am != AccessMode.ReadWrite)
                {
                    //Read Write denied.
                    p.status = (byte)ErrorCode.ReadWriteDenied;
                }
                else
                {
                    try
                    {
                        if (value is byte[])
                        {
                            DataType dt = (obj as IGXDLMSBase).GetDataType(index);
                            if (dt != DataType.None && dt != DataType.OctetString)
                            {
                                value = GXDLMSClient.ChangeType((byte[])value, dt);
                            }
                        }
                        ValueEventArgs e = new ValueEventArgs(Settings, obj, index, 0, null);
                        e.Value = value;
                        ValueEventArgs[] list = new ValueEventArgs[]{e};
                        if (p.multipleBlocks)
                        {
                            transaction = new GXDLMSLongTransaction(list, Command.GetRequest, data);
                        }
                        Write(list);
                        if (!e.Handled && !p.multipleBlocks)
                        {
                            (obj as IGXDLMSBase).SetValue(Settings, e);
                        }
                    }
                    catch (Exception)
                    {
                        p.status = (byte)ErrorCode.HardwareFault;
                    }
                }
            }
        }

        private void HanleSetRequestWithDataBlock(GXByteBuffer data, GXDLMSLNParameters p)
        {
            GXDataInfo reply = new GXDataInfo();
            p.multipleBlocks = data.GetUInt8() == 0;
            UInt32 blockNumber = data.GetUInt32();
            if (blockNumber != Settings.BlockIndex)
            {
                Debug.WriteLine("HanleSetRequestWithDataBlock failed. Invalid block number. " + Settings.BlockIndex + "/" + blockNumber);
                p.status = (byte)ErrorCode.DataBlockNumberInvalid;
            }
            else
            {
                int size = GXCommon.GetObjectCount(data);
                int realSize = data.Size - data.Position;
                if (size != realSize)
                {
                    Debug.WriteLine("HanleSetRequestWithDataBlock failed. Invalid block size.");
                    p.status = (byte)ErrorCode.DataBlockUnavailable;
                }
                transaction.data.Set(data);
                //If all data is received.
                if (!p.multipleBlocks)
                {
                    try
                    {
                        object value = GXCommon.GetData(transaction.data, reply);
                        if (value is byte[])
                        {
                            DataType dt = (transaction.targets[0].Target as IGXDLMSBase).GetDataType(transaction.targets[0].Index);
                            if (dt != DataType.None && dt != DataType.OctetString)
                            {
                                value = GXDLMSClient.ChangeType((byte[])value, dt);
                            }
                        }
                        transaction.targets[0].Value = value;
                        Write(transaction.targets);
                        if (!transaction.targets[0].Handled && !p.multipleBlocks)
                        {
                            (transaction.targets[0].Target as IGXDLMSBase).SetValue(Settings, transaction.targets[0]);
                        }
                    }
                    catch (Exception)
                    {
                        p.status = (byte)ErrorCode.HardwareFault;
                    }
                    finally
                    {
                        transaction = null;
                    }
                    Settings.ResetBlockIndex();
                }
            }
            p.multipleBlocks = true;
        }

        ///<summary>
        ///Handle set request.
        ///</summary>
        ///<returns>
        ///Reply to the client.
        ///</returns>
        private void HandleSetRequest(GXByteBuffer data)
        {
            //Return error if connection is not established.
            if (!Settings.Connected)
            {
                replyData.Set(GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                    ServiceError.Service, (byte)Service.ServiceUnsupported));
                return;
            }
            // Get type.
            byte type = data.GetUInt8();
            // Get invoke ID and priority.
            data.GetUInt8();
            // SetRequest normal or Set Request With First Data Block
            GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, Command.SetResponse, type, null, null, 0);
            if (type == 1 || type == 2)
            {
                HandleSetRequest(data, type, p);
            }            
            //Set Request With Data Block
            else if (type == 3)
            {
                HanleSetRequestWithDataBlock(data, p);   
            }
            else
            {
                Debug.WriteLine("HandleSetRequest failed. Unknown command.");
                Settings.ResetBlockIndex();
                p.status = (byte) ErrorCode.HardwareFault;
            }
            GXDLMS.GetLNPdu(p, replyData);
        }

        /// <summary>
        /// Handle get request normal command.
        /// </summary>
        /// <param name="data">Received data.</param>
        private void GetRequestNormal(GXByteBuffer data)
        {
            ValueEventArgs e = null;
            GXByteBuffer bb = new GXByteBuffer();
            // Get type.
            ErrorCode status = ErrorCode.Ok;

            Settings.Count = Settings.Index = 0;
            Settings.ResetBlockIndex();
            // CI
            ObjectType ci = (ObjectType)data.GetUInt16();
            byte[] ln = new byte[6];
            data.Get(ln);
            // Attribute Id
            byte attributeIndex = data.GetUInt8();
            GXDLMSObject obj = Settings.Objects.FindByLN(ci, GXDLMSObject.ToLogicalName(ln));
            if (obj == null)
            {
                obj = FindObject(ci, 0, GXDLMSObject.ToLogicalName(ln));
            }
            if (obj == null)
            {
                // "Access Error : Device reports a undefined object."
                status = ErrorCode.UndefinedObject;
            }
            else
            {
                if (obj.GetAccess(attributeIndex) == AccessMode.NoAccess)
                {
                    //Read Write denied.
                    status = ErrorCode.ReadWriteDenied;
                }
                else
                {
                    // AccessSelection
                    byte selection = data.GetUInt8();
                    byte selector = 0;
                    object parameters = null;
                    if (selection != 0)
                    {
                        selector = data.GetUInt8();
                        GXDataInfo info = new GXDataInfo();
                        parameters = GXCommon.GetData(data, info);
                    }

                    e = new ValueEventArgs(Settings, obj, attributeIndex, selector, parameters);
                    Read(new ValueEventArgs[] { e });
                    object value;
                    if (e.Handled)
                    {
                        value = e.Value;
                    }
                    else
                    {
                        value = (obj as IGXDLMSBase).GetValue(Settings, e);
                    }
                    GXDLMS.AppendData(obj, attributeIndex, bb, value);
                    status = e.Error;
                }
            }
            GXDLMS.GetLNPdu(new GXDLMSLNParameters(Settings, Command.GetResponse, 1, null, bb, (byte)status), replyData);
            if (Settings.Count != Settings.Index || bb.Size != bb.Position)
            {
                transaction = new GXDLMSLongTransaction(new ValueEventArgs[] { e }, Command.GetRequest, bb);
            }
        }

        /// <summary>
        /// Handle get request next data block command.
        /// </summary>
        /// <param name="data">Received data.</param>
        private void GetRequestNextDataBlock(GXByteBuffer data)
        {
            GXByteBuffer bb = new GXByteBuffer();
            int index;
            // Get block index.
            index = (int)data.GetUInt32();
            if (index != Settings.BlockIndex)
            {
                Debug.WriteLine("handleGetRequest failed. Invalid block number. " + Settings.BlockIndex + "/" + index);
                GXDLMS.GetLNPdu(new GXDLMSLNParameters(Settings, Command.GetResponse, 2, null, bb, (byte)ErrorCode.DataBlockNumberInvalid), replyData);
            }
            else
            {
                Settings.IncreaseBlockIndex();
                GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, Command.GetResponse, 2, null, bb, (byte)ErrorCode.Ok);
                //If transaction is not in progress.
                if (transaction == null)
                {
                    p.status = (byte)ErrorCode.NoLongGetOrReadInProgress;
                }
                else
                {
                    bb.Set(transaction.data);
                    bool moreData = Settings.Index != Settings.Count;
                    if (moreData)
                    {
                        //If there is multiple blocks on the buffer.
                        //This might happen when Max PDU size is very small.
                        if (bb.Size < Settings.MaxPDUSize)
                        {
                            foreach (ValueEventArgs arg in transaction.targets)
                            {
                                object value;
                                if (arg.Handled)
                                {
                                    value = arg.Value;
                                }
                                else
                                {
                                    value = (arg.Target as IGXDLMSBase).GetValue(Settings, arg);
                                }
                                //Add data.
                                GXDLMS.AppendData(arg.Target, arg.Index, bb, value);
                            }
                        }
                    }
                    p.multipleBlocks = true;
                    GXDLMS.GetLNPdu(p, replyData);
                    if (moreData || bb.Size - bb.Position != 0)
                    {
                        transaction.data = bb;
                    }
                    else
                    {
                        transaction = null;
                    }
                }
            }
        }

        /// <summary>
        /// Handle get request with list command.
        /// </summary>
        /// <param name="data">Received data.</param>
        private void GetRequestWithList(GXByteBuffer data) 
        {
            ValueEventArgs e;
            GXByteBuffer bb = new GXByteBuffer();
            int pos;
            int cnt = GXCommon.GetObjectCount(data);
            GXCommon.SetObjectCount(cnt, bb);
            List<ValueEventArgs> list = new List<ValueEventArgs>();
            for (pos = 0; pos != cnt; ++pos)
            {
                ObjectType ci = (ObjectType)data.GetUInt16();
                byte[] ln = new byte[6];
                data.Get(ln);
                short attributeIndex = data.GetUInt8();

                GXDLMSObject obj = Settings.Objects.FindByLN(ci, GXDLMSObject.ToLogicalName(ln));
                if (obj == null)
                {
                    obj = FindObject(ci, 0, GXDLMSObject.ToLogicalName(ln));
                }
                if (obj == null)
                {
                    // "Access Error : Device reports a undefined object."
                    e = new ValueEventArgs(Settings, obj, attributeIndex, 0, 0);
                    e.Error = ErrorCode.UndefinedObject;
                    list.Add(e);
                }
                else
                {
                    if (obj.GetAccess(attributeIndex) == AccessMode.NoAccess)
                    {
                        //Read Write denied.
                        ValueEventArgs arg = new ValueEventArgs(Settings, obj, attributeIndex, 0, null);
                        arg.Error = ErrorCode.ReadWriteDenied;
                        list.Add(arg);
                    }
                    else
                    {
                        // AccessSelection
                        int selection = data.GetUInt8();
                        int selector = 0;
                        object parameters = null;
                        if (selection != 0)
                        {
                            selector = data.GetUInt8();
                            GXDataInfo info = new GXDataInfo();
                            parameters = GXCommon.GetData(data, info);
                        }
                        ValueEventArgs arg = new ValueEventArgs(Settings, obj, attributeIndex, selector, parameters);
                        list.Add(arg);
                    }
                }
            }
            Read(list.ToArray());
            object value;
            pos = 0;
            foreach (ValueEventArgs it in list)
            {
                try
                {
                    if (it.Handled)
                    {
                        value = it.Value;
                    }
                    else
                    {
                        value = (it.Target as IGXDLMSBase).GetValue(Settings, it);
                    }
                    bb.SetUInt8(it.Error);
                    GXDLMS.AppendData(it.Target, it.Index, bb, value);
                }
                catch (Exception)
                {
                    bb.SetUInt8((byte)ErrorCode.HardwareFault);
                }
                if (Settings.Index != Settings.Count)
                {
                    transaction = new GXDLMSLongTransaction(list.ToArray(), Command.GetRequest, null);
                }
                ++pos;
            }
            GXDLMSLNParameters p = new GXDLMSLNParameters(Settings, Command.GetResponse, 3, null, bb, 0xFF);
            GXDLMS.GetLNPdu(p, replyData);
        }

        private void HandleGetRequest(GXByteBuffer data)
        {
            //Return error if connection is not established.
            if (!Settings.Connected)
            {
                replyData.Add(GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                    ServiceError.Service, (byte)Service.ServiceUnsupported));
                return;
            }

            byte type = data.GetUInt8();
            // Get invoke ID and priority.
            data.GetUInt8();
            // GetRequest normal
            if (type == (byte)GetCommandType.Normal)
            {
                GetRequestNormal(data);
            }
            else if (type == (byte)GetCommandType.NextDataBlock)
            {
                // Get request for next data block
                GetRequestNextDataBlock(data);
            }
            else if (type == (byte)GetCommandType.WithList)
            {
                // Get request with a list.
                GetRequestWithList(data);
            }
            else
            {
                Debug.WriteLine("HandleGetRequest failed. Invalid command type.");
                Settings.ResetBlockIndex();
                GXByteBuffer bb = new GXByteBuffer();
                // Access Error : Device reports a hardware fault.
                bb.SetUInt8((byte) ErrorCode.HardwareFault);
                GXDLMS.GetLNPdu(new GXDLMSLNParameters(Settings, Command.GetResponse, type, null, bb, (byte)ErrorCode.Ok), replyData);
            }
        }

        ///<summary>
        /// Find Short Name object.
        ///</summary>
        ///<param name="sn">
        ///Short name to find.
        ///</param>
        private GXSNInfo FindSNObject(int sn)
        {
            GXSNInfo i = new GXSNInfo();
            int offset, count;
            foreach (GXDLMSObject it in Items)
            {
                if (sn >= it.ShortName)
                {
                    //If attribute is accessed.
                    if (sn < it.ShortName + (it as IGXDLMSBase).GetAttributeCount() * 8)
                    {
                        i.IsAction = false;
                        i.Item = it;
                        i.Index = ((sn - i.Item.ShortName) / 8) + 1;
                        Debug.WriteLine(string.Format("Reading {0:D}, attribute index {1:D}", i.Item.Name, i.Index));
                        break;
                    }
                    else
                    {
                        //If method is accessed.
                        GXDLMS.GetActionInfo(it.ObjectType, out offset, out count);
                        if (sn < it.ShortName + offset + (8 * count))
                        {
                            i.Item = it;
                            i.IsAction = true;
                            i.Index = (sn - it.ShortName - offset) / 8 + 1;
                            break;
                        }
                    }
                }
            }
            if (i.Item == null)
            {
                i.Item = FindObject(ObjectType.None, sn, null);
            }
            return i;
        }

        private void HandleRead(byte type, GXByteBuffer data, List<ValueEventArgs> list, List<ValueEventArgs> reads, List<ValueEventArgs> actions)
        {
            // GetRequest normal
            int sn = data.GetUInt16();
            GXSNInfo info = FindSNObject(sn);
            ValueEventArgs e = new ValueEventArgs(Settings, info.Item, info.Index, 0, null);
            e.action = info.IsAction;
            if (type == (byte)VariableAccessSpecification.ParameterisedAccess)
            {
                e.Selector = data.GetUInt8();
                GXDataInfo di = new GXDataInfo();
                e.Parameters = GXCommon.GetData(data, di);
            }
            //Return error if connection is not established.
            if (!Settings.Connected && (!e.action || e.Target.ShortName != 0xFA00 || e.Index != 8))
            {
                replyData.Add(GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                    ServiceError.Service, (byte)Service.ServiceUnsupported));
                return;
            }

            list.Add(e);
            if (e.action && info.Item.GetMethodAccess(info.Index) == MethodAccessMode.NoAccess)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            else if (info.Item.GetAccess(info.Index) == AccessMode.NoAccess)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            else
            {
                if (e.action)
                {
                    actions.Add(e);
                }
                else
                {
                    reads.Add(e);
                }
            }
        }

        /// <summary>
        /// Handle read Block in blocks.
        /// </summary>
        /// <param name="data">Received data.</param>
        void HandleReadBlockNumberAccess(GXByteBuffer data)
        {
            UInt16 blockNumber = data.GetUInt16();
            GXByteBuffer bb = new GXByteBuffer();
            if (blockNumber != Settings.BlockIndex)
            {
                Debug.WriteLine("handleReadRequest failed. Invalid block number. " + Settings.BlockIndex + "/" + blockNumber);
                bb.SetUInt8(ErrorCode.DataBlockNumberInvalid);
                GXDLMS.GetSNPdu(new GXDLMSSNParameters(Settings, Command.ReadResponse, 1, (byte)SingleReadResponse.DataAccessError, bb, null), replyData);
                Settings.ResetBlockIndex();
                return;
            }
            if (Settings.Index != Settings.Count && transaction.data.Size < Settings.MaxPDUSize)
            {
                List<ValueEventArgs> reads = new List<ValueEventArgs>();
                List<ValueEventArgs> actions = new List<ValueEventArgs>();
                foreach (ValueEventArgs it in transaction.targets)
                {
                    if (it.action)
                    {
                        actions.Add(it);
                    }
                    else
                    {
                        reads.Add(it);
                    }
                }
                if (reads.Count != 0)
                {
                    Read(reads.ToArray());
                }

                if (actions.Count != 0)
                {
                    Action(actions.ToArray());
                }
                GetReadData(transaction.targets, transaction.data);
            }
            Settings.IncreaseBlockIndex();
            GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.ReadResponse, 1, (byte)SingleReadResponse.DataBlockResult, bb, transaction.data);
            p.multipleBlocks = true;
            GXDLMS.GetSNPdu(p, replyData);
            //If all data is sent.
            if (transaction.data.Size == transaction.data.Position)
            {
                transaction = null;
                Settings.ResetBlockIndex();
            }
            else
            {
                transaction.data.Trim();
            }
        }

        private void HandleReadDataBlockAccess(Command command, GXByteBuffer data, int cnt)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bool isLast = data.GetUInt8() != 0;
            UInt16 blockNumber = data.GetUInt16();
            if (blockNumber != Settings.BlockIndex)
            {
                Debug.WriteLine("handleReadRequest failed. Invalid block number. " + Settings.BlockIndex + "/" + blockNumber);
                bb.SetUInt8(ErrorCode.DataBlockNumberInvalid);
                GXDLMS.GetSNPdu(new GXDLMSSNParameters(Settings, command, 1, (byte)SingleReadResponse.DataAccessError, bb, null), replyData);
                Settings.ResetBlockIndex();
                return;
            }
            int count = 1;
            byte type = (byte) DataType.OctetString;
            if (command == Command.WriteResponse)
            {
                count = GXCommon.GetObjectCount(data);
                type = data.GetUInt8();
            }
            int size = GXCommon.GetObjectCount(data);
            int realSize = data.Size - data.Position;
            if (count != 1 || type != (byte)DataType.OctetString || size != realSize)
            {
                Debug.WriteLine("handleGetRequest failed. Invalid block size.");
                bb.SetUInt8(ErrorCode.DataBlockUnavailable);
                GXDLMS.GetSNPdu(new GXDLMSSNParameters(Settings, command, cnt, (byte)SingleReadResponse.DataAccessError, bb, null), replyData);
                Settings.ResetBlockIndex();
                return;
            }
            if (transaction == null)
            {
                transaction = new GXDLMSLongTransaction(null, command, data);
            }
            else
            {
                transaction.data.Set(data);
            }
            if (!isLast)
            {
                bb.SetUInt16(blockNumber);
                Settings.IncreaseBlockIndex();
                if (command == Command.ReadResponse)
                {
                    type = (byte)SingleReadResponse.BlockNumber;
                }
                else
                {
                    type = (byte)SingleWriteResponse.BlockNumber;
                }
                GXDLMS.GetSNPdu(new GXDLMSSNParameters(Settings, command, cnt, type, null, bb), replyData);
                return;
            }
            else
            {
                if (transaction != null)
                {
                    data.Size = 0;
                    data.Set(transaction.data);
                    transaction = null;
                }
                if (command == Command.ReadResponse)
                {
                    HandleReadRequest(data);
                }
                else
                {
                    HandleWriteRequest(data);
                }
                Settings.ResetBlockIndex();
            }
        }
        private void ReturnSNError(Command cmd, ErrorCode error)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(error);
            GXDLMS.GetSNPdu(new GXDLMSSNParameters(Settings, cmd, 1, (byte)SingleReadResponse.DataAccessError, bb, null), replyData);
            Settings.ResetBlockIndex();
        }

        /// <summary>
        /// Get data for Read command.
        /// </summary>
        /// <param name="list">received objects.</param>
        /// <param name="data">Data as byte array.</param>
        /// <returns>Response type.</returns>
        private SingleReadResponse GetReadData(ValueEventArgs[] list, GXByteBuffer data)
        {
            object value;
            bool first = true;
            SingleReadResponse type = SingleReadResponse.Data;
            foreach (ValueEventArgs e in list)
            {
                if (e.Handled)
                {
                    value = e.Value;
                }
                else
                {
                    //If action.
                    if (e.action)
                    {
                        value = ((IGXDLMSBase)e.Target).Invoke(Settings, e);
                    }
                    else
                    {
                        value = (e.Target as IGXDLMSBase).GetValue(Settings, e);
                    }
                }
                if (e.Error == 0)
                {
                    if (!first && list.Length != 1)
                    {
                        data.SetUInt8(SingleReadResponse.Data);
                    }
                    //If action.
                    if (e.action)
                    {
                        GXCommon.SetData(data, GXCommon.GetValueType(value), value);
                    }
                    else
                    {
                        GXDLMS.AppendData(e.Target, e.Index, data, value);
                    }
                }
                else
                {
                    if (!first && list.Length != 1)
                    {
                        data.SetUInt8(SingleReadResponse.DataAccessError);
                    }
                    data.SetUInt8(e.Error);
                    type = SingleReadResponse.DataAccessError;
                }
                first = false;
            }
            return type;
        }

        /// <summary>
        /// Handle read request.
        /// </summary>
        /// <param name="data">Received data.</param>
        private void HandleReadRequest(GXByteBuffer data)
        {
            GXByteBuffer attributeDescriptor = new GXByteBuffer();
            GXByteBuffer bb = new GXByteBuffer();
            int cnt = 0xFF;
            byte type = 0;
            List<ValueEventArgs> list = new List<ValueEventArgs>();
            //If get next frame.
            if (data.Size == 0)
            {
                if (transaction != null)
                {
                    return;
                }
                bb.Set(replyData);
                replyData.Clear();
                foreach (ValueEventArgs it in transaction.targets)
                {
                    list.Add(it);
                }
            }
            else
            {
                cnt = GXCommon.GetObjectCount(data);
                List<ValueEventArgs> reads = new List<ValueEventArgs>();
                List<ValueEventArgs> actions = new List<ValueEventArgs>();
                for (int pos = 0; pos != cnt; ++pos)
                {
                    type = data.GetUInt8();
                    if (type == (byte)VariableAccessSpecification.VariableName ||
                        type == (byte)VariableAccessSpecification.ParameterisedAccess)
                    {
                        HandleRead(type, data, list, reads, actions);                       
                    }                   
                    else if (type == (byte)VariableAccessSpecification.BlockNumberAccess)
                    {
                        HandleReadBlockNumberAccess(data);                          
                        return;
                    }
                    else if (type == (byte)VariableAccessSpecification.ReadDataBlockAccess)
                    {
                        HandleReadDataBlockAccess(Command.ReadResponse, data, cnt);
                        return;
                    }
                    else
                    {
                        ReturnSNError(Command.ReadResponse, ErrorCode.ReadWriteDenied);
                        return;
                    }
                }
                if (reads.Count != 0)
                {
                    Read(reads.ToArray());
                }

                if (actions.Count != 0)
                {
                    Action(actions.ToArray());
                }
            }
            byte requestType = (byte) GetReadData(list.ToArray(), bb);
            GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.ReadResponse, list.Count, requestType, attributeDescriptor, bb);
            GXDLMS.GetSNPdu(p, replyData);
            if (transaction == null && (bb.Size != bb.Position || Settings.Count != Settings.Index))
            {
                List<ValueEventArgs> reads = new List<ValueEventArgs>();
                foreach (var it in list)
                {
                    reads.Add(it);
                }
                transaction = new GXDLMSLongTransaction(reads.ToArray(), Command.ReadRequest, bb);
            }
            else if (transaction != null)
            {
                replyData.Set(bb);
                return;
            }
        }       

        ///<summary>
        /// Handle write request.
        ///</summary>
        ///<param name="Reply">
        /// Received data from the client.
        /// </param>
        ///<returns> 
        /// Reply.
        ///</returns>
        private void HandleWriteRequest(GXByteBuffer data)
        {
            //Return error if connection is not established.
            if (!Settings.Connected)
            {
                replyData.Add(GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                    ServiceError.Service, (byte)Service.ServiceUnsupported));
                return;
            }
            short type;
            object value;
            // Get object count.
            List<GXSNInfo> targets = new List<GXSNInfo>();
            int cnt = GXCommon.GetObjectCount(data);
            GXByteBuffer results = new GXByteBuffer((ushort)cnt);
            for (int pos = 0; pos != cnt; ++pos)
            {
                type = data.GetUInt8();
                if (type == (byte)VariableAccessSpecification.VariableName)
                {
                    int sn = data.GetUInt16();
                    GXSNInfo info = FindSNObject(sn);
                    targets.Add(info);
                    // If target is unknown.
                    if (info == null)
                    {
                        // Device reports a undefined object.
                        results.SetUInt8(ErrorCode.UndefinedObject);
                    }
                    else
                    {
                        results.SetUInt8(ErrorCode.Ok);
                    }
                }
                else if (type == (byte)VariableAccessSpecification.WriteDataBlockAccess)
                {
                    HandleReadDataBlockAccess(Command.WriteResponse, data, cnt);
                    return;
                }
                else
                {
                    // Device reports a HW error.
                    results.SetUInt8(ErrorCode.HardwareFault);
                }
            }
            // Get data count.
            cnt = GXCommon.GetObjectCount(data);
            GXDataInfo di = new GXDataInfo();
            for (int pos = 0; pos != cnt; ++pos)
            {
                if (results.GetUInt8(pos) == 0)
                {
                    // If object has found.
                    GXSNInfo target = targets[pos];
                    value = GXCommon.GetData(data, di);
                    if (value is byte[])
                    {
                        DataType dt = target.Item.GetDataType(target.Index);
                        if (dt != DataType.None && dt != DataType.OctetString)
                        {
                            value = GXDLMSClient.ChangeType((byte[])value, dt);
                        }
                    }
                    di.Clear();
                    AccessMode am = target.Item.GetAccess(target.Index);
                    // If write is denied.
                    if (am != AccessMode.Write && am != AccessMode.ReadWrite)
                    {
                        results.SetUInt8((byte)pos, (byte)ErrorCode.ReadWriteDenied);
                    }
                    else
                    {
                        ValueEventArgs e = new ValueEventArgs(Settings, target.Item, target.Index, 0, null);
                        e.Value = value;
                        Write(new ValueEventArgs[] { e });
                        if (!e.Handled)
                        {
                            (target.Item as IGXDLMSBase).SetValue(Settings, e);
                        }
                    }
                }
            }
            GXByteBuffer bb = new GXByteBuffer((UInt16)(2 * cnt));
            byte ret;
            for (int pos = 0; pos != cnt; ++pos)
            {
                ret = results.GetUInt8(pos);
                // If meter returns error.
                if (ret != 0)
                {
                    bb.SetUInt8(1);
                }
                bb.SetUInt8(ret);
            }
            GXDLMSSNParameters p = new GXDLMSSNParameters(Settings, Command.WriteResponse, cnt, 0xFF, null, bb);
            GXDLMS.GetSNPdu(p, replyData);
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
            SourceDiagnostic diagnostic = GXAPDU.ParsePDU(Settings, Settings.Cipher, data);
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
                    Settings.StoCChallenge = GXSecure.GenerateChallenge(Settings.Authentication);
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
            GXAPDU.GenerateAARE(Settings, replyData, result, diagnostic, Settings.Cipher);
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
                    ServiceError.Service, (byte)Service.ServiceUnsupported));
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