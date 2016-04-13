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
        protected GXDLMSSettings Settings;

        private GXServerReply ServerReply = new GXServerReply();

        /// <summary>
        /// Frames to send.
        /// </summary>
        private byte[][] Frames = null;

        /// <summary>
        /// Received and parsed data from the client.
        /// </summary>
        private GXReplyData Reply = new GXReplyData();

        /// <summary>
        /// Connected server address.
        /// </summary>
        private int ConnectedServerAddress = 0;
        /// <summary>
        /// Connected client address.
        /// </summary>
        private int ConnectedClientAddress = 0;

        /**
         * Frame index.
         */
        private int FrameIndex = 0;
        bool Initialized = false;
        private SortedDictionary<ushort, GXDLMSObject> SortedItems = new SortedDictionary<ushort, GXDLMSObject>();

        /// <summary>
        /// Find object.
        /// </summary>
        /// <param name="objectType">Object type. In Short Name referencing this is not used.</param>
        /// <param name="sn">Short Name. In Logical name referencing this is not used.</param>
        /// <param name="ln">Logical Name. In Short Name referencing this is not used.</param>
        /// <returns>Found object or null if object is not found.</returns>
        public abstract GXDLMSObject FindObject(ObjectType objectType, int sn, String ln);

        /// <summary>
        /// Check is data sent to this server.
        /// </summary>
        /// <param name="serverAddress">Server address.</param>
        /// <param name="clientAddress">Client address.</param>
        /// <returns>True, if data is sent to this server.</returns>
        public abstract bool IsTarget(int serverAddress, int clientAddress);


        /// <summary>
        /// Check whether the authentication and password are correct.
        /// </summary>
        /// <param name="authentication">Authentication level.</param>
        /// <param name="password">Password</param>
        /// <returns>Source diagnostic.</returns>
        public abstract SourceDiagnostic ValidateAuthentication(
                Authentication authentication, byte[] password);


        /// <summary>
        /// Read selected item.
        /// </summary>
        /// <param name="args">Handled read requests.</param>
        abstract public void Read(ValueEventArgs[] args);

        /// <summary>
        /// Write selected item.
        /// </summary>
        /// <param name="args">Handled write requests.</param>
        abstract public void Write(ValueEventArgs[] args);

        /// <summary>
        /// Client attempts to connect with the wrong server or client address.
        /// </summary>
        abstract public void InvalidConnection(ConnectionEventArgs e);

        /// <summary>
        /// Action is occurred.
        /// </summary>
        /// <param name="args">Handled action requests.</param>
        abstract public void Action(ValueEventArgs[] args);

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
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="UseLogicalNameReferencing"/>
        [DefaultValue(0xFFFF)]
        public ushort MaxReceivePDUSize
        {
            get
            {
                return Settings.MaxReceivePDUSize;
            }
            set
            {
                Settings.MaxReceivePDUSize = value;
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
        /// <seealso cref="DLMSVersion"/>
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
        public UInt32 StartingPacketIndex
        {
            get
            {
                return Settings.BlockIndex;
            }
            set
            {
                Settings.BlockIndex = value;
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
            if (SortedItems.Count != Items.Count)
            {
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
                        if (pg.ProfileEntries < 1)
                        {
                            throw new Exception("Invalid Profile Entries. Profile entries tells amount of rows in the table.");
                        }
                        foreach (var obj in pg.CaptureObjects)
                        {
                            if (obj.Value.AttributeIndex < 1)
                            {
                                throw new Exception("Invalid attribute index. SelectedAttributeIndex is not set for " + obj.Key.Name);
                            }
                        }
                        if (pg.ProfileEntries < 1)
                        {
                            throw new Exception("Invalid Profile Entries. Profile entries tells amount of rows in the table.");
                        }
                        if (pg.CapturePeriod != 0)
                        {
                            GXProfileGenericUpdater p = new GXProfileGenericUpdater(this, pg);
                            Thread thread = new Thread(new ThreadStart(p.UpdateProfileGenericData));
                            thread.IsBackground = true;
                            thread.Start();
                        }
                    }
                    else if ((it is GXDLMSAssociationShortName && !this.UseLogicalNameReferencing) ||
                        (it is GXDLMSAssociationLogicalName && this.UseLogicalNameReferencing))
                    {
                        association = true;
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
                ushort sn = 0xA0;
                if (!this.UseLogicalNameReferencing)
                {
                    SortedItems.Clear();
                    foreach (GXDLMSObject it in Items)
                    {
                        if (it is GXDLMSAssociationShortName)
                        {
                            it.ShortName = 0xFA00;
                        }
                        //Generate Short Name if not given.
                        if (it.ShortName == 0)
                        {
                            do
                            {
                                it.ShortName = sn;
                                sn += 0xA0;
                            }
                            while (SortedItems.ContainsKey(it.ShortName));
                        }
                        SortedItems.Add(it.ShortName, it);
                    }
                }
            }
        }

        /// <summary>
        /// Reset after connection is closed.
        /// </summary>
        public void Reset()
        {
            Settings.Connected = false;
            ConnectedClientAddress = 0;
            Reply.Clear();
            Settings.ServerAddress = 0;
            Settings.ClientAddress = 0;
            Settings.Authentication = Authentication.None;
            if (Settings.Cipher != null)
            {
                Settings.Cipher.Reset();
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
                byte[] data = GetPacket(buff);
                // If all data is not received yet or message is not accepted.
                if (!Reply.IsComplete)
                {
                    return null;
                }
                if (data != null)
                {
                    return data;
                }
                HandleCommand();
                if (!Reply.IsMoreData)
                {
                    Reply.Clear();
                }
                data = Frames[FrameIndex++];
                return data;
            }
            catch (Exception e)
            {
                // Disconnect.
                Debug.WriteLine(e.ToString());
                byte[] data = GXDLMS.SplitToHdlcFrames(Settings, (byte)FrameType.Rejected, null)[0];
                Reset();
                return data;
            }
        }

        ///<summary>
        /// Get packet from received data.
        ///</summary>
        ///<param name="buff">
        /// Received data. </param>
        ///<returns>
        ///Reply if any.
        ///</returns>
        private byte[] GetPacket(byte[] buff)
        {
            GXByteBuffer receivedFrame = new GXByteBuffer(buff);
            // Save position where data was. This is used if data is not sent to
            // this server.
            ushort offset = Reply.Data.Size;

            GXDLMS.GetData(Settings, receivedFrame, Reply);
            // If all data is not received yet.
            if (!Reply.IsComplete)
            {
                return null;
            }           
            byte[] data;
            // If client sends keepalive or get next frame request.
            if ((Reply.MoreData & RequestTypes.Frame) != 0)
            {
                if (Frames != null && Frames.Length > FrameIndex)
                {
                    data = Frames[FrameIndex++];
                    return data;
                }
                FrameIndex = 0;
                data = GXDLMS.SplitToHdlcFrames(Settings, Settings.KeepAlive(), null)[0];
                return data;
            }

            // Check is data sent to this server.
            if (ConnectedServerAddress == 0 || ConnectedClientAddress == 0)
            {
                // Check is data send to this server.
                if (!IsTarget(Settings.ServerAddress, Settings.ClientAddress))
                {
                    Reply.Data.Size = offset;
                    Reply.IsComplete = false;
                }
                ConnectedServerAddress = Settings.ServerAddress;
                ConnectedClientAddress = Settings.ClientAddress;
            }
            else if (Settings.ServerAddress != ConnectedServerAddress
                  || Settings.ClientAddress != ConnectedClientAddress)
            {
                Reply.Data.Size = offset;
                Reply.IsComplete = false;
            }


            // Clear received data.
            receivedFrame.Clear();
            ServerReply.Data = Reply.Data;
            FrameIndex = 0;
            return null;
        }

        ///<summary>
        /// Handle received command. 
        ///</summary>
        private void HandleCommand()
        {
            switch (Reply.Command)
            {
                case Command.SetRequest:
                    Frames = HandleSetRequest();
                    break;
                case Command.WriteRequest:
                    Frames = HandleWriteRequest();
                    break;
                case Command.GetRequest:
                    Frames = HandleGetRequest();
                    break;
                case Command.ReadRequest:
                    Frames = HandleReadRequest();
                    break;
                case Command.MethodRequest:
                    Frames = HandleMethodRequest();
                    break;
                case Command.Snrm:
                    Frames = HandleSnrmRequest();
                    break;
                case Command.Aarq:
                    Frames = HandleAarqRequest();
                    Settings.Connected = true;
                    break;
                case Command.DisconnectRequest:
                    Frames = GenerateDisconnectRequest();
                    Settings.Connected = false;
                    break;
                default:
                    Debug.WriteLine("Invalid command: " + Reply.Command.ToString());
                    Frames = GXDLMS.SplitToHdlcFrames(Settings, (byte)FrameType.Rejected, null);
                    break;
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
        private byte[][] HandleMethodRequest()
        {
            GXByteBuffer data = Reply.Data;
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
            if (obj == null)
            {
                obj = FindObject(ci, 0, GXDLMSObject.ToLogicalName(ln));
            }
            if (obj == null)
            {
                // Device reports a undefined object.
                addError(ErrorCode.UndefinedObject, bb);
                Debug.WriteLine("Undefined object.");
            }
            else
            {
                ValueEventArgs e = new ValueEventArgs(obj, id, 0, parameters);
                Action(new ValueEventArgs[] { e });
                if (e.Handled)
                {
                    bb.Add((obj as IGXDLMSBase).Invoke(Settings, id, parameters));
                }
                else
                {
                    bb.Add((obj as IGXDLMSBase).Invoke(Settings, id, parameters));
                }
            }
            return GXDLMS.SplitPdu(Settings, Command.MethodResponse, 1, bb, ErrorCode.Ok, DateTime.MinValue)[0];
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

        ///<summary>
        ///Handle set request.
        ///</summary>
        ///<returns>
        ///Reply to the client.
        ///</returns>
        private byte[][] HandleSetRequest()
        {
            ErrorCode error = ErrorCode.Ok;
            GXByteBuffer data = Reply.Data;
            GXDataInfo info = new GXDataInfo();
            GXByteBuffer bb = new GXByteBuffer();
            // Get type.
            short type = data.GetUInt8();
            // Get invoke ID and priority.
            data.GetUInt8();
            // SetRequest normal
            if (type == 1)
            {
                Settings.ResetBlockIndex();
                ServerReply.Index = 0;
                // CI
                ObjectType ci = (ObjectType)data.GetUInt16();
                byte[] ln = new byte[6];
                data.Get(ln);
                // Attribute index.
                int index = data.GetUInt8();
                // Get Access Selection.
                data.GetUInt8();
                object value = GXCommon.GetData(data, info);
                GXDLMSObject obj = Settings.Objects.FindByLN(ci, GXDLMSObject.ToLogicalName(ln));
                if (obj == null)
                {
                    obj = FindObject(ci, 0, GXDLMSObject.ToLogicalName(ln));
                }
                // If target is unknown.
                if (obj == null)
                {
                    Debug.WriteLine("Undefined object.");
                    // Device reports a undefined object.
                    error = ErrorCode.UndefinedObject;
                }
                else
                {
                    AccessMode am = obj.GetAccess(index);
                    // If write is denied.
                    if (am != AccessMode.Write && am != AccessMode.ReadWrite)
                    {
                        Debug.WriteLine("Read Write denied.");
                        error = ErrorCode.ReadWriteDenied;
                    }
                    else
                    {
                        try
                        {
                            if (value is byte[])
                            {
                                DataType dt = (obj as IGXDLMSBase).GetDataType(index);
                                if (dt != DataType.None)
                                {
                                    value = GXDLMSClient.ChangeType((byte[])value, dt);
                                }
                            }
                            ValueEventArgs e = new ValueEventArgs(obj, index, 0, null);
                            Write(new ValueEventArgs[] { e });
                            if (!e.Handled)
                            {
                                (obj as IGXDLMSBase).SetValue(Settings, index, value);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                            error = ErrorCode.HardwareFault;
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("handleSetRequest failed. Unknown command.");
                Settings.ResetBlockIndex();
                error = ErrorCode.HardwareFault;
            }
            return GXDLMS.SplitPdu(Settings, Command.SetResponse, 1, bb, error, DateTime.MinValue)[0];
        }

        private byte[][] HandleGetRequest()
        {
            ErrorCode error = ErrorCode.Ok;
            GXByteBuffer data = Reply.Data;
            GXByteBuffer bb = new GXByteBuffer();
            short type;
            int index = 0;
            object parameters = null;
            // Get type.
            type = data.GetUInt8();
            // Get invoke ID and priority.
            data.GetUInt8();
            // GetRequest normal
            if (type == 1)
            {
                Settings.ResetBlockIndex();
                ServerReply.Index = 0;
                parameters = null;
                // CI
                ObjectType ci = (ObjectType)data.GetUInt16();
                byte[] ln = new byte[6];
                data.Get(ln);
                // Attribute Id
                int attributeIndex = data.GetUInt8();
                GXDLMSObject obj = Settings.Objects.FindByLN(ci, GXDLMSObject.ToLogicalName(ln));
                if (obj == null)
                {
                    obj = FindObject(ci, 0, GXDLMSObject.ToLogicalName(ln));
                }
                if (obj == null)
                {
                    // "Access Error : Device reports a undefined object."
                    error = ErrorCode.UndefinedObject;
                }
                else
                {
                    // AccessSelection
                    int selection = data.GetUInt8();
                    int selector = 0;
                    if (selection != 0)
                    {
                        selector = data.GetUInt8();
                        GXDataInfo info = new GXDataInfo();
                        parameters = GXCommon.GetData(data, info);
                    }

                    ValueEventArgs e = new ValueEventArgs(obj, attributeIndex, 0, parameters);
                    Read(new ValueEventArgs[] { e });
                    object value;
                    if (e.Handled)
                    {
                        value = e.Value;
                    }
                    else
                    {
                        value = (obj as IGXDLMSBase).GetValue(Settings, attributeIndex, selector, parameters);
                    }
                    GXDLMS.AppendData(obj, attributeIndex, bb, value);
                }
                ServerReply.ReplyMessages = GXDLMS.SplitPdu(Settings, Command.GetResponse, 1, bb, error, DateTime.MinValue);

            }
            else if (type == 2)
            {
                // Get request for next data block
                // Get block index.
                index = (int)data.GetUInt32();
                if (index != Settings.BlockIndex + 1)
                {
                    Debug.WriteLine("handleGetRequest failed. Invalid block number. " + Settings.BlockIndex + "/" + index);
                    ServerReply.ReplyMessages = GXDLMS.SplitPdu(Settings, Command.GetResponse, 1, bb,
                        ErrorCode.DataBlockNumberInvalid, DateTime.MinValue);
                    index = 0;
                    ServerReply.Index = index;
                }
                else
                {
                    Settings.IncreaseBlockIndex();
                    index = ServerReply.Index + 1;
                    ServerReply.Index = index;
                }
            }
            else if (type == 3)
            {
                // Get request with a list.
                int cnt = GXCommon.GetObjectCount(data);
                GXCommon.SetObjectCount(cnt, bb);
                List<ValueEventArgs> list = new List<ValueEventArgs>();

                for (int pos = 0; pos != cnt; ++pos)
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
                        error = ErrorCode.UndefinedObject;
                    }
                    else
                    {
                        // AccessSelection
                        int selection = data.GetUInt8();
                        int selector = 0;
                        if (selection != 0)
                        {
                            selector = data.GetUInt8();
                            GXDataInfo info = new GXDataInfo();
                            parameters = GXCommon.GetData(data, info);
                        }
                        ValueEventArgs e = new ValueEventArgs(obj, attributeIndex, 0, parameters);
                        list.Add(e);
                    }
                }
                Read(list.ToArray());
                object value;
                foreach (ValueEventArgs it in list)
                {
                    if (it.Handled)
                    {
                        value = it.Value;
                    }
                    else
                    {
                        value = (it.Target as IGXDLMSBase).GetValue(Settings, it.Index, it.Selector, it.Parameters);
                    }
                    bb.SetUInt8(ErrorCode.Ok);
                    GXDLMS.AppendData(it.Target, it.Index, bb, value);
                }
                ServerReply.ReplyMessages = GXDLMS.SplitPdu(Settings, Command.GetResponse, 3, bb, error, DateTime.MinValue);
            }
            else
            {
                Debug.WriteLine("handleGetRequest failed. Invalid command type.");
                Settings.ResetBlockIndex();
                // Access Error : Device reports a hardware fault.
                ServerReply.ReplyMessages = GXDLMS.SplitPdu(Settings, Command.GetResponse, 1, bb, ErrorCode.HardwareFault, DateTime.MinValue);
            }
            ServerReply.Index = index;
            return ServerReply.ReplyMessages[index];
        }

        ///<summary>
        /// Find Short Name object.
        ///</summary>
        ///<param name="sn">
        ///Short name to find.
        ///</param>
        private GXSNInfo FindSNObject(int sn)
        {
            GXSNInfo info = new GXSNInfo();
            foreach (KeyValuePair<ushort, GXDLMSObject> it in SortedItems)
            {
                int aCnt = ((IGXDLMSBase)it.Value).GetAttributeCount();
                if (sn >= it.Key && sn <= (it.Key + (8 * aCnt)))
                {
                    info.IsAction = false;
                    info.Item = it.Value;
                    info.Index = ((sn - info.Item.ShortName) / 8) + 1;
                    Debug.WriteLine(string.Format("Reading {0:D}, attribute index {1:D}", info.Item.Name, info.Index));
                    break;
                }
                else if (sn >= it.Key + aCnt && ((IGXDLMSBase)it.Value).GetMethodCount() != 0)
                {
                    // Check if action.

                    // Convert DLMS data to object type.
                    int value2, count;
                    GXDLMS.GetActionInfo(it.Value.ObjectType, out value2, out count);
                    if (sn <= it.Key + value2 + (8 * count))
                    {
                        info.Item = it.Value;
                        info.IsAction = true;
                        info.Index = (sn - it.Value.ShortName - value2) / 8 + 1;
                        break;
                    }
                }
            }
            if (info.Item == null)
            {
                info.Item = FindObject(ObjectType.None, sn, null);
            }
            if (info.Item == null)
            {
                throw new System.ArgumentException("Invalid SN Command.");
            }
            return info;
        }

        ///    
        ///<summary>Handle read request.
        /// </summary>
        ///<returns> Reply to the client. </returns>
        ///     
        private byte[][] HandleReadRequest()
        {
            GXByteBuffer data = Reply.Data;
            short type;
            object value = null;
            GXByteBuffer bb = new GXByteBuffer();
            int cnt = GXCommon.GetObjectCount(data);
            GXCommon.SetObjectCount(cnt, bb);
            GXSNInfo info;
            for (int pos = 0; pos != cnt; ++pos)
            {
                type = data.GetUInt8();
                // GetRequest normal
                if (type == 2)
                {
                    int sn = data.GetUInt16();
                    info = FindSNObject(sn);
                    if (!info.IsAction)
                    {
                        ValueEventArgs e = new ValueEventArgs(info.Item, info.Index, 0, null);
                        Read(new ValueEventArgs[]{e});
                        if (e.Handled)
                        {
                            value = e.Value;
                        }
                        else
                        {
                            value = (info.Item as IGXDLMSBase).GetValue(Settings, info.Index, 0, null);
                        }
                        // Set status.
                        bb.SetUInt8(0);
                        GXDLMS.AppendData(info.Item, info.Index, bb, value);
                    }
                    else
                    {
                        ValueEventArgs e = new ValueEventArgs(info.Item, info.Index, 0, null);
                        Action(new ValueEventArgs[] { e });
                        if (e.Handled)
                        {
                            value = e.Value;
                        }
                        else
                        {
                            value = ((IGXDLMSBase)info.Item).Invoke(Settings, info.Index, null);
                        }
                        // Set status.
                        bb.SetUInt8(0);
                        // Add value
                        bb.SetUInt8(GXCommon.GetValueType(value));
                        bb.Add(value);
                    }
                }
                else if (type == 2)
                {
                    // Get request for next data block
                    throw new System.ArgumentException("TODO: Invalid Command.");
                }
                else if (type == 4)
                {
                    // Parameterised access.
                    int sn = data.GetUInt16();
                    int selector = data.GetUInt8();
                    GXDataInfo di = new GXDataInfo();
                    object parameters = GXCommon.GetData(data, di);
                    info = FindSNObject(sn);
                    if (!info.IsAction)
                    {
                        ValueEventArgs e = new ValueEventArgs(info.Item, info.Index, 0, parameters);
                        Read(new ValueEventArgs[] { e });
                        if (e.Handled)
                        {
                            value = e.Value;
                        }
                        else
                        {
                            value = (info.Item as IGXDLMSBase).GetValue(Settings, info.Index, selector, parameters);
                        }
                        // Set status.
                        bb.SetUInt8(0);
                        GXDLMS.AppendData(info.Item, info.Index, bb, value);
                    }
                    else
                    {
                        ValueEventArgs e = new ValueEventArgs(info.Item, info.Index, 0, parameters);
                        e.Value = parameters;
                        Action(new ValueEventArgs[] { e });
                        if (e.Handled)
                        {
                            value = e.Value;
                        }
                        else
                        {
                            value = ((IGXDLMSBase)info.Item).Invoke(Settings, info.Index, parameters);
                        }
                        // Add value
                        bb.Add(value);
                    }
                }
                else
                {
                    throw new System.ArgumentException("Invalid Command.");
                }
            }
            return GXDLMS.SplitPdu(Settings, Command.ReadResponse, 1, bb,
                                ErrorCode.Ok, DateTime.MinValue)[0];
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
        private byte[][] HandleWriteRequest()
        {
            GXByteBuffer data = Reply.Data;
            short type;
            object value;
            // Get object count.
            IList<GXSNInfo> targets = new List<GXSNInfo>();
            int cnt = GXCommon.GetObjectCount(data);
            GXByteBuffer results = new GXByteBuffer((ushort)cnt);
            for (int pos = 0; pos != cnt; ++pos)
            {
                type = data.GetUInt8();
                if (type == 2)
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
                        ValueEventArgs e = new ValueEventArgs(target.Item, target.Index, 0, null);
                        e.Value = value;
                        Write(new ValueEventArgs[] { e });
                        if (!e.Handled)
                        {
                            (target.Item as IGXDLMSBase).SetValue(Settings, target.Index, value);
                        }
                    }
                }
            }
            GXByteBuffer bb = new GXByteBuffer((UInt16)(2 * cnt + 2));
            GXCommon.SetObjectCount(cnt, bb);
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
            return GXDLMS.SplitPdu(Settings, Command.WriteResponse, 1, bb, ErrorCode.Ok, DateTime.MinValue)[0];
        }

        ///<summary>
        ///Parse AARQ request that client send and returns AARE request.
        /// </summary>
        ///<returns> 
        ///Reply to the client. 
        ///</returns>
        private byte[][] HandleAarqRequest()
        {
            AssociationResult result = AssociationResult.Accepted;
            Settings.CtoSChallenge = null;
            if (!Settings.UseCustomChallenge)
            {
                Settings.StoCChallenge = null;
            }
            SourceDiagnostic diagnostic = GXAPDU.ParsePDU(Settings, Settings.Cipher, Reply.Data);
            if (diagnostic != SourceDiagnostic.None)
            {
                result = AssociationResult.PermanentRejected;
                diagnostic = SourceDiagnostic.ApplicationContextNameNotSupported;
            }
            else
            {
                // Check that user can access server.
                diagnostic = ValidateAuthentication(Settings.Authentication, Settings.Password);
                if (diagnostic != SourceDiagnostic.None)
                {
                    result = AssociationResult.PermanentRejected;
                }
                else if (Settings.Authentication > Authentication.Low)
                {
                    // If High authentication is used.
                    Settings.StoCChallenge = GXSecure.GenerateChallenge(Settings.Authentication);
                    result = AssociationResult.Accepted;
                    diagnostic = SourceDiagnostic.AuthenticationRequired;
                }
            }
            // Generate AARE packet.
            GXByteBuffer buff = new GXByteBuffer(150);
            GXAPDU.GenerateAARE(Settings, buff, result, diagnostic, Settings.Cipher);
            return GXDLMS.SplitPdu(Settings, Command.Aare, 0, buff,
                            ErrorCode.Ok, DateTime.MinValue)[0];
        }

        ///<summary>
        ///Parse SNRM Request. If server do not accept client empty byte array is returned.
        /// </summary>
        ///<returns>
        ///Returns returned UA packet. 
        ///</returns>
        private byte[][] HandleSnrmRequest()
        {
            GXByteBuffer bb = new GXByteBuffer(25);
            bb.SetUInt8(0x81); // FromatID
            bb.SetUInt8(0x80); // GroupID
            bb.SetUInt8(0); // Length
            bb.SetUInt8(HDLCInfo.MaxInfoTX);
            bb.SetUInt8(GXCommon.GetSize(Limits.MaxInfoTX));
            bb.Add(Limits.MaxInfoTX);
            bb.SetUInt8(HDLCInfo.MaxInfoRX);
            bb.SetUInt8(GXCommon.GetSize(Limits.MaxInfoRX));
            bb.Add(Limits.MaxInfoRX);
            bb.SetUInt8(HDLCInfo.WindowSizeTX);
            bb.SetUInt8(GXCommon.GetSize(Limits.WindowSizeTX));
            bb.Add(Limits.WindowSizeTX);
            bb.SetUInt8(HDLCInfo.WindowSizeRX);
            bb.SetUInt8(GXCommon.GetSize(Limits.WindowSizeRX));
            bb.Add(Limits.WindowSizeRX);
            bb.SetUInt8(2, (byte)(bb.Size - 3));
            return GXDLMS.SplitToHdlcFrames(Settings, (byte)Command.Ua, bb);
        }

        ///<summary>
        ///Generates disconnect request.
        /// </summary>
        ///<returns>
        ///Disconnect request. 
        ///</returns>
        private byte[][] GenerateDisconnectRequest()
        {
            GXByteBuffer buff;
            if (this.InterfaceType == InterfaceType.WRAPPER)
            {
                buff = new GXByteBuffer(2);
                buff.SetUInt8(0x63);
                buff.SetUInt8(0x0);
                return GXDLMS.SplitToWrapperFrames(Settings, buff);
            }
            else
            {
                buff = new GXByteBuffer(22);
                buff.SetUInt8(0x81); // FromatID
                buff.SetUInt8(0x80); // GroupID
                buff.SetUInt8(0); // Length

                buff.SetUInt8(HDLCInfo.MaxInfoTX);
                buff.SetUInt8(GXCommon.GetSize(Limits.MaxInfoTX));
                buff.Add(Limits.MaxInfoTX);

                buff.SetUInt8(HDLCInfo.MaxInfoRX);
                buff.SetUInt8(GXCommon.GetSize(Limits.MaxInfoRX));
                buff.Add(Limits.MaxInfoRX);

                buff.SetUInt8(HDLCInfo.WindowSizeTX);
                buff.SetUInt8(GXCommon.GetSize(Limits.WindowSizeTX));
                buff.Add(Limits.WindowSizeTX);

                buff.SetUInt8(HDLCInfo.WindowSizeRX);
                buff.SetUInt8(GXCommon.GetSize(Limits.WindowSizeRX));
                buff.Add(Limits.WindowSizeRX);

                int len = buff.Position - 3;
                buff.SetUInt8(2, (byte)len); // Length.
            }
            return GXDLMS.SplitToHdlcFrames(Settings, (byte)Command.Ua, buff);
        }
    }
}