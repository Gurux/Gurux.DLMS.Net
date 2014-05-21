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

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMSServer implements methods to implement DLMS/COSEM meter/proxy.
    /// </summary>
    public abstract class GXDLMSServerBase
    {
        internal GXDLMS m_Base;
        List<byte> ReceivedFrame = new List<byte>();
        byte LastCommand;
        List<byte> ReceivedData = new List<byte>();
        List<byte[]> SendData = new List<byte[]>();
        int FrameIndex = 0;
        bool Initialized = false;
        //TODO: StartProtocolType Protocol;

        private SortedDictionary<ushort, GXDLMSObject> SortedItems = new SortedDictionary<ushort, GXDLMSObject>();


        /// <summary>
        /// Read selected item.
        /// </summary>
        /// <param name="e"></param>
        abstract public void Read(ValueEventArgs e);

        /// <summary>
        /// Write selected item.
        /// </summary>
        /// <param name="e"></param>
        abstract public void Write(ValueEventArgs e);

        /// <summary>
        /// Client attempts to connect with the wrong server or client address.
        /// </summary>
        abstract public void InvalidConnection(ConnectionEventArgs e);

        /// <summary>
        /// Action is occurred.
        /// </summary>
        /// <param name="e"></param>
        abstract public void Action(ValueEventArgs e);

        /// <summary>
        /// Constructor.
        /// </summary>        
        public GXDLMSServerBase() : this(false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>        
        public GXDLMSServerBase(bool logicalNameReferencing)
        {
            m_Base = new GXDLMS(true);
            m_Base.UseLogicalNameReferencing = logicalNameReferencing;
            //TODO: StartProtocol = StartProtocolType.DLMS;
            Reset();
            Items = new GXDLMSObjectCollection(this);
            m_Base.LNSettings = new GXDLMSLNSettings(new byte[] { 0x00, 0x7E, 0x1F });
            m_Base.SNSettings = new GXDLMSSNSettings(new byte[] { 0x1C, 0x03, 0x20 });
            ServerIDs = new List<object>();
            this.InterfaceType = InterfaceType.General;
        }

        /// <summary>
        /// Count server ID from serial number.
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns>Server ID.</returns>
        static public uint CountServerIDFromSerialNumber(uint serialNumber)
        {
            return (uint) GXManufacturer.CountServerAddress(HDLCAddressType.SerialNumber, "SN%10000+1000", serialNumber, 0);            
        }

        public GXCiphering Ciphering
        {
            get
            {
                return m_Base.Ciphering;
            }
        }

        /* TODO: Add IEC handshake later.
        /// <summary>
        /// Start protocol.
        /// </summary>
        public StartProtocolType StartProtocol
        {
            get;
            set;
        }

        /// <summary>
        /// IEC manufacturer ID.
        /// </summary>
        public string ManufacturerID
        {
            get;
            set;
        }

        /// <summary>
        /// IEC Baudrate ID.
        /// </summary>
        public int BaudRateID
        {
            get;
            set;
        }

        /// <summary>
        /// IEC device ID.
        /// </summary>
        public string DeviceID
        {
            get;
            set;
        }
         * */

        /// <summary>
        /// Count server ID from physical and logical addresses.
        /// </summary>
        /// <returns>Server ID.</returns>
        public object CountServerID(object physicalAddress, int LogicalAddress)
        {
            if (this.InterfaceType == InterfaceType.Net)
            {
                return Convert.ToUInt16(physicalAddress);
            }
            return GXManufacturer.CountServerAddress(HDLCAddressType.Default, null, physicalAddress, LogicalAddress);
        }

        /// <summary>
        /// List of objects that meter supports.
        /// </summary>
        public GXDLMSObjectCollection Items
        {
            get;
            internal set;
        }

        /// <summary>
        /// Information from the connection size that server can handle.
        /// </summary>
        public GXDLMSLimits Limits
        {
            get
            {
                return m_Base.Limits;
            }
        }

        static GXDLMSObject CreateObject(Gurux.DLMS.ObjectType type)
        {
            return GXDLMS.CreateObject(type);
        }

        /// <summary>
        /// Collection of server IDs.
        /// </summary>
        /// <remarks>
        /// Server ID is the indentification of the device that is used as a server.
        /// Server ID is aka HDLC Address.        
        /// </remarks>
        public List<object> ServerIDs
        {
            get;
            private set;
        }

        /// <summary>
        /// DLMS version number. 
        /// </summary>
        /// <remarks>
        /// Gurux DLMS component supports DLMS version number 6.
        /// </remarks>
        [DefaultValue(6)]
        public byte DLMSVersion
        {
            get
            {
                return m_Base.DLMSVersion;
            }
            set
            {
                m_Base.DLMSVersion = value;
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
                return m_Base.MaxReceivePDUSize;
            }
            set
            {
                m_Base.MaxReceivePDUSize = value;
            }
        }

        /// <summary>
        /// Determines, whether Logical, or Short name, referencing is used.     
        /// </summary>
        /// <remarks>
        /// Referencing depends on the device to communicate with.
        /// Normally, a device supports only either Logical or Short name referencing.
        /// The referencing is defined by the device manufacurer.
        /// If the referencing is wrong, the SNMR message will fail.
        /// </remarks>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="MaxReceivePDUSize"/>
        [DefaultValue(false)]
        public bool UseLogicalNameReferencing
        {
            get
            {
                return m_Base.UseLogicalNameReferencing;
            }
            set
            {
                m_Base.UseLogicalNameReferencing = value;
            }
        }

        /// <summary>
        /// Used Authentication password pairs.
        /// </summary>
        public List<GXAuthentication> Authentications
        {
            get;
            private set;
        }

        public Priority Priority
        {
            get
            {
                return m_Base.Priority;
            }
            set
            {
                m_Base.Priority = value;
            }
        }

        /// <summary>
        /// Used service class.
        /// </summary>
        public ServiceClass ServiceClass
        {
            get
            {
                return m_Base.ServiceClass;
            }
            set
            {
                m_Base.ServiceClass = value;
            }
        }

        /// <summary>
        /// Invoke ID.
        /// </summary>
        public byte InvokeID
        {
            get
            {
                return m_Base.InvokeID;
            }
            set
            {
                m_Base.InvokeID = value;
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
                return m_Base.InterfaceType;
            }
            set
            {
                bool changed = m_Base.InterfaceType != value;
                if (changed || Authentications == null)
                {
                    m_Base.InterfaceType = value;
                    if (value == InterfaceType.General)
                    {
                        if (Authentications == null)
                        {
                            Authentications = new List<GXAuthentication>();                            
                        }
                        else
                        {
                            Authentications.Clear();
                            ServerIDs.Clear();
                        }                                               
                        Authentications.Add(new GXAuthentication(Authentication.None, "", (byte)0x10));
                        Authentications.Add(new GXAuthentication(Authentication.Low, "GuruxLow", (byte)0x20));
                        Authentications.Add(new GXAuthentication(Authentication.High, "GuruxHigh", (byte)0x40));
                        Authentications.Add(new GXAuthentication(Authentication.HighMD5, "GuruxHighMD5", (byte)0x40));
                        Authentications.Add(new GXAuthentication(Authentication.HighSHA1, "GuruxHighSHA1", (byte)0x40));
                        Authentications.Add(new GXAuthentication(Authentication.HighGMAC, "GuruxHighGMAC", (byte)0x40));
                        ServerIDs.Add(CountServerID((byte)1, 0));
                    }
                    else
                    {
                        if (Authentications == null)
                        {
                            Authentications = new List<GXAuthentication>();
                        }
                        else
                        {
                            Authentications.Clear();
                            ServerIDs.Clear();
                        }
                        Authentications.Add(new GXAuthentication(Authentication.None, "", (ushort)0x10));
                        Authentications.Add(new GXAuthentication(Authentication.Low, "GuruxLow", (ushort)0x20));
                        Authentications.Add(new GXAuthentication(Authentication.High, "GuruxHigh", (ushort)0x40));
                        Authentications.Add(new GXAuthentication(Authentication.HighMD5, "GuruxHighMD5", (ushort)0x40));
                        Authentications.Add(new GXAuthentication(Authentication.HighSHA1, "GuruxHighSHA1", (ushort)0x40));
                        Authentications.Add(new GXAuthentication(Authentication.HighGMAC, "GuruxHighGMAC", (ushort)0x40));
                        ServerIDs.Add((ushort)1);
                    }
                }
            }
        }

        /// <summary>
        /// Gets Logical Name settings, read from the device. 
        /// </summary>
        public GXDLMSLNSettings LNSettings
        {
            get
            {
                return m_Base.LNSettings;
            }
        }

        /// <summary>
        /// Gets Short Name settings, read from the device.
        /// </summary>
        public GXDLMSSNSettings SNSettings
        {
            get
            {
                return m_Base.SNSettings;
            }
        }

        /// <summary>
        /// Quality Of Service is an analysis of nonfunctional aspects of the software properties.
        /// </summary>
        /// <returns></returns>
        public int ValueOfQualityOfService
        {
            get
            {
                return m_Base.ValueOfQualityOfService;
            }
            set
            {
                m_Base.ValueOfQualityOfService = value;
            }
        }

        /// <summary>
        /// Retrieves the amount of unused bits.
        /// </summary>
        /// <returns></returns>
        public int NumberOfUnusedBits
        {
            get
            {
                return m_Base.NumberOfUnusedBits;
            }
            set
            {
                m_Base.NumberOfUnusedBits = value;
            }
        }

        /// <summary>
        /// Returns object types.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This can be used with serizlization.
        /// </remarks>
        public static Type[] GetObjectTypes()
        {
            return GXDLMS.GetObjectTypes();
        }

        /// <summary>
        /// Get command, OBIS Code and attribute index.
        /// </summary>
        /// <param name="data"></param>        
        /// <param name="name"></param>
        /// <param name="attributeIndex"></param>
        private void GetCommand(byte cmd, byte[] data, out ObjectType type, List<object> names, out int attributeIndex, out int selector, out object parameters)
        {
            selector = 0;
            type = ObjectType.None;
            parameters = null;
            int index = 0;
            names.Clear();
            if (this.UseLogicalNameReferencing)
            {
                type = (ObjectType)GXCommon.GetUInt16(data, ref index);
                string str = null;
                for (int pos = 0; pos != 6; ++pos)
                {
                    if (str != null)
                    {
                        str += ".";
                    }
                    str += data[index++].ToString();
                }
                names.Add(str);
                attributeIndex = data[index++];                
                //if Value
                if (data.Length - index != 0)
                {
                    //If access selector is used.
                    if (data[index++] != 0)
                    {
                        if (cmd != (byte)Command.MethodRequest)
                        {
                            selector = data[index++];
                        }
                    }
                    DataType dt = DataType.None;
                    int a, b, c = 0;
                    parameters = GXCommon.GetData(data, ref index, ActionType.None, out a, out b, ref dt, ref c);
                }               
            }
            else
            {
                attributeIndex = 0;
                int cnt = GXCommon.GetObjectCount(data, ref index);
                if (cmd == (int)Command.ReadRequest)
                {
                    for (int pos = 0; pos != cnt; ++pos)
                    {
                        int tp = data[index++];
                        if (tp == 2)
                        {
                            names.Add(GXCommon.GetUInt16(data, ref index));
                        }
                        else if (tp == 4)
                        {
                            names.Add(GXCommon.GetUInt16(data, ref index));
                            selector = data[index++];
                            DataType dt = DataType.None;
                            int a, b, c = 0;
                            parameters = GXCommon.GetData(data, ref index, ActionType.None, out a, out b, ref dt, ref c);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }
                else if (cmd == (int)Command.WriteRequest)
                {
                    List<byte> accessTypes = new List<byte>();
                    for (int pos = 0; pos != cnt; ++pos)
                    {
                        accessTypes.Add(data[index++]);
                        names.Add(GXCommon.GetUInt16(data, ref index));
                    }
                    //Get data count
                    cnt = GXCommon.GetObjectCount(data, ref index);
                    for (int pos = 0; pos != cnt; ++pos)
                    {
                        if (accessTypes[pos] == 4)
                        {
                            selector = data[index++];
                        }
                        DataType dt = DataType.None;
                        int a, b, c = 0;
                        parameters = GXCommon.GetData(data, ref index, ActionType.None, out a, out b, ref dt, ref c);
                    }
                }
            }
        }                          

        /// <summary>
        /// Generates an acknowledgment message, with which the server is informed to 
        /// send next packets.
        /// </summary>
        /// <param name="type">Frame type</param>
        /// <returns>Acknowledgment message as byte array.</returns>
        byte[] ReceiverReady(RequestTypes type)
        {
            return m_Base.ReceiverReady(type);
        }

        /// <summary>
        /// This method is used to solve current index of received DLMS packet, 
        /// by retrieving the current progress status.
        /// </summary>
        /// <param name="data">DLMS data to parse.</param>
        /// <returns>The current index of the packet.</returns>
        public int GetCurrentProgressStatus(byte[] data)
        {
            return m_Base.GetCurrentProgressStatus(data);
        }

        /// <summary>
        /// This method is used to solve the total amount of received items,
        /// by retrieving the maximum progress status.
        /// </summary>
        /// <param name="data">DLMS data to parse.</param>
        /// <returns>Total amount of received items.</returns>
        public int GetMaxProgressStatus(byte[] data)
        {
            return m_Base.GetMaxProgressStatus(data);
        }

        /// <summary>
        ///  Initialize server.
        /// </summary>
        /// <remarks>
        /// This must call after server objects are set.
        /// </remarks>
        public void Initialize()
        {
            if (Authentications.Count == 0)
            {
                throw new Exception("Authentications is not set.");
            }
            bool association = false;
            Initialized = true;            
            if (SortedItems.Count != Items.Count)
            {
                for (int pos = 0; pos != Items.Count; ++pos)
                {                    
                    GXDLMSObject it = Items[pos];
                    if (string.IsNullOrEmpty(it.LogicalName) || it.LogicalName.Split('.').Length != 6)
                    {
                        throw new Exception("Invalid Logical Name.");
                    }                    
                    if (it is GXDLMSProfileGeneric)
                    {
                        GXDLMSProfileGeneric pg = it as GXDLMSProfileGeneric;
                        pg.Parent.Parent = this;
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
                    else if ((it is GXDLMSAssociationShortName && !this.UseLogicalNameReferencing)||
                        (it is GXDLMSAssociationLogicalName && this.UseLogicalNameReferencing))
                    {
                        association = true;
                    }
                    else if (!(it is IGXDLMSBase))//Remove unsupported items.
                    {
                        System.Diagnostics.Debug.WriteLine(it.ObjectType.ToString() + " not supported.");
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
        /// Parse AARQ request that cliend send and returns AARE request.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] HandleAARQRequest(byte[] data)
        {
            int index = 0, error;
            byte frame;            
            List<byte> arr = new List<byte>(data);
            bool packetFull, wrongCrc;
            byte command;
            m_Base.GetDataFromFrame(arr, index, out frame, true, out error, false, out packetFull, out wrongCrc, out command, false);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            GXAPDU aarq = new GXAPDU(null);
            aarq.UseLN = this.UseLogicalNameReferencing;
            int pos = 0;
            aarq.EncodeData(arr.ToArray(), ref pos);
            AssociationResult result = AssociationResult.Accepted;
            SourceDiagnostic diagnostic = SourceDiagnostic.None;
            m_Base.Authentication = aarq.Authentication;
            m_Base.CtoSChallenge = null;
            m_Base.StoCChallenge = null;
            if (aarq.Authentication >= Authentication.High)
            {
                m_Base.CtoSChallenge = aarq.Password;
            }
            if (this.UseLogicalNameReferencing != aarq.UseLN)
            {
                result = AssociationResult.PermanentRejected;
                diagnostic = SourceDiagnostic.ApplicationContextNameNotSupported;
            }
            else
            {
                GXAuthentication auth = null;
                foreach (GXAuthentication it in Authentications)
                {
                    if (it.Type == aarq.Authentication)
                    {
                        auth = it;
                        break;
                    }
                }                
                if (auth == null)
                {
                    result = AssociationResult.PermanentRejected;
                    //If authentication is required.
                    if (aarq.Authentication == Authentication.None)
                    {
                        diagnostic = SourceDiagnostic.AuthenticationRequired;
                    }
                    else
                    {
                        diagnostic = SourceDiagnostic.AuthenticationMechanismNameNotRecognised;
                    }
                }
                //If authentication is used check pw.
                else if (aarq.Authentication != Authentication.None)
                {                    
                    if (aarq.Authentication == Authentication.Low)
                    {
                        //If Low authentication is used and pw don't match.
                        if (string.Compare(auth.Password, ASCIIEncoding.ASCII.GetString(aarq.Password)) != 0)
                        {
                            result = AssociationResult.PermanentRejected;
                            diagnostic = SourceDiagnostic.AuthenticationFailure;
                        }
                    }
                    else //If High authentication is used.
                    {
                        m_Base.StoCChallenge = GXDLMS.GenerateChallenge();
                        System.Diagnostics.Debug.WriteLine("StoC: " + BitConverter.ToString(m_Base.StoCChallenge));
                        result = AssociationResult.Accepted;
                        diagnostic = SourceDiagnostic.AuthenticationRequired;
                    }
                }
            }
            //Generate AARE packet.
            List<byte> buff = new List<byte>();
            byte[] conformanceBlock;
            if (UseLogicalNameReferencing)
            {
                conformanceBlock = LNSettings.m_ConformanceBlock;
            }
            else
            {
                conformanceBlock = SNSettings.m_ConformanceBlock;
            }
            aarq.GenerateAARE(buff, aarq.Authentication, m_Base.StoCChallenge, MaxReceivePDUSize, conformanceBlock, result, diagnostic);
            if (this.InterfaceType == InterfaceType.General)
            {
                buff.InsertRange(0, Gurux.DLMS.Internal.GXCommon.LLCReplyBytes);
            }
            m_Base.ExpectedFrame = 0;
            m_Base.FrameSequence = -1;
            m_Base.ReceiveSequenceNo = 1;
            m_Base.SendSequenceNo = 0;            
            return m_Base.AddFrame(m_Base.GenerateIFrame(), false, buff, 0, buff.Count);
        }

        void SetValue(List<byte> buff, object data)
        {
            byte[] tmp = Gurux.Shared.GXCommon.GetAsByteArray(data);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(tmp);
            }
            buff.Add((byte)tmp.Length);
            buff.AddRange(tmp);
        } 

        /// <summary>
        /// Parse SNRM Request.
        /// </summary>
        /// <remarks>
        /// If server do not accept client empty byte array is returned.
        /// </remarks>
        /// <returns>Returns returned UA packet.</returns>
        byte[] HandleSnrmRequest()
        {
            List<byte> buff = new List<byte>();
            buff.Add((byte)HDLCInfo.MaxInfoTX);
            SetValue(buff, Limits.MaxInfoTX);
            buff.Add((byte)HDLCInfo.MaxInfoRX);
            SetValue(buff, Limits.MaxInfoRX);
            buff.Add((byte)HDLCInfo.WindowSizeTX);
            SetValue(buff, Limits.WindowSizeTX);
            buff.Add((byte)HDLCInfo.WindowSizeRX);
            SetValue(buff, Limits.WindowSizeRX);
            byte len = (byte)buff.Count;
            buff.Insert(0, 0x81); //FromatID
            buff.Insert(1, 0x80); //GroupID
            buff.Insert(2, len); //len           
            return m_Base.AddFrame(GXDLMS.GenerateUFrame(GXDLMS.UFrameMode.UA), false, buff, 0, buff.Count);
        }

        /// <summary>
        /// Generate disconnect request.
        /// </summary>
        /// <returns></returns>
        byte[] GenerateDisconnectRequest()
        {
            List<byte> buff = new List<byte>();
            if (this.InterfaceType == InterfaceType.Net)
            {
                buff.Add(0x63);
                buff.Add(0x0);
                return m_Base.AddFrame(0, false, buff, 0, buff.Count);
            }            
            buff.Add((byte)HDLCInfo.MaxInfoTX);
            SetValue(buff, Limits.MaxInfoTX);
            buff.Add((byte)HDLCInfo.MaxInfoRX);
            SetValue(buff, Limits.MaxInfoRX);
            buff.Add((byte)HDLCInfo.WindowSizeTX);
            SetValue(buff, Limits.WindowSizeTX);
            buff.Add((byte)HDLCInfo.WindowSizeRX);
            SetValue(buff, Limits.WindowSizeRX);
            byte len = (byte)buff.Count;
            buff.Insert(0, 0x81); //FromatID
            buff.Insert(1, 0x80); //GroupID
            buff.Insert(2, len); //len
            return m_Base.AddFrame((byte)FrameType.UA, false, buff, 0, buff.Count);
        }         

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal bool GetAddress(byte[] buff, ref object clientId, ref object serverId)
        {
            if (this.InterfaceType == DLMS.InterfaceType.General)
            {
                if (buff.Length < 5)
                {
                    return false;
                }
            }
            else if (this.InterfaceType == DLMS.InterfaceType.Net)
            {
                if (buff.Length < 6)
                {
                    return false;
                }
            }
            else
            {
                throw new Exception("Unknown interface type.");
            }
            byte[] serverAddress = Gurux.Shared.GXCommon.GetAsByteArray(serverId);            
            int index = 0;
            int PacketStartID = 0, len = buff.Length;
            int FrameLen = 0;
            //If DLMS frame is generated.
            if (InterfaceType != InterfaceType.Net)
            {
                //Find start of HDLC frame.
                for (; index < len; ++index)
                {
                    if (buff[index] == GXCommon.HDLCFrameStartEnd)
                    {
                        PacketStartID = index;
                        ++index;
                        break;
                    }
                }
                if (index == len) //Not a HDLC frame.
                {
                    throw new GXDLMSException("Invalid data format.");
                }
                byte frame = buff[index++];
                RequestTypes MoreData = RequestTypes.None;

                //Is there more data available.
                if ((frame & 0x8) != 0)
                {
                    MoreData = RequestTypes.Frame;
                }
                //Check frame length.
                if ((frame & 0x7) != 0)
                {
                    FrameLen = ((frame & 0x7) << 8);
                }                
                //If not enought data.
                FrameLen += buff[index++];
                if (len < FrameLen + index - 1)
                {
                    return false;
                }
                if (MoreData == RequestTypes.None && 
                    buff[FrameLen + PacketStartID + 1] != GXCommon.HDLCFrameStartEnd)
                {
                    throw new GXDLMSException("Invalid data format.");
                }
                serverId = m_Base.GetAddress(buff, ref index);
                clientId = m_Base.GetAddress(buff, ref index); //Client address is always one byte.
            }
            else
            {
                //Get version
                int ver = (buff[index++] & 0xFF) << 8;
                ver |= buff[index++] & 0xFF;
                if (ver != 1)
                {
                    throw new GXDLMSException("Unknown version.");
                }
                clientId = GXCommon.GetUInt16(buff, ref index);
                serverId = GXCommon.GetUInt16(buff, ref index);
            }
            return true;
        }

        /// <summary>
        /// Reset after connection is closed.
        /// </summary>
        public void Reset()
        {
            //TODO: Protocol = StartProtocol;
            ReceivedFrame.Clear();
            ReceivedData.Clear();
            LastCommand = 0;
            m_Base.ServerID = null;
            m_Base.ClientID = null;
            m_Base.Authentication = Authentication.None;
            m_Base.Ciphering.Security = Security.None;
            m_Base.Ciphering.FrameCounter = 0;
        }

        private byte[] GetValue(object name, GXDLMSObject item, int index, int selector, object parameters)
        {
            IGXDLMSBase tmp = item as IGXDLMSBase;
            object value = null;             
            if (tmp != null)
            {
                value = tmp.GetValue(index, selector, parameters);
            }
            else
            {
                object[] values = item.GetValues();
                if (index <= values.Length)
                {
                    value = values[index - 1];
                }
            }
            Gurux.DLMS.DataType tp = item.GetDataType(index);
            if (tp == DataType.None)
            {
                tp = Gurux.DLMS.Internal.GXCommon.GetValueType(value);
            }
            //If data is shown as string, but it's OCTECT String.
            if (tp == DataType.OctetString && value is string && item.GetUIDataType(index) == DataType.String)
            {
                value = ASCIIEncoding.ASCII.GetBytes((string)value);
            }
            if (tp != DataType.None || (value == null && tp == DataType.None))
            {
                SendData.AddRange(ReadReply(name, item.ObjectType, index, value, tp));
                return SendData[FrameIndex];
            }
            //Return HW error.
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Mandles client request.
        /// </summary>
        /// <param name="buff">Received data from the client.</param>
        /// <returns>Response to the request.</returns>        
        /// <remarks>
        /// Response is null if request packet is not compleate.
        /// </remarks>
        public byte[] HandleRequest(byte[] buff)
        {
            byte command = 0;
            lock (this)
            {
                if (buff == null)
                {
                    return null;
                }
                if (!Initialized)
                {
                    throw new Exception("Server not Initialized.");
                }
                try
                {
                    byte[] data = null;
                    if (ReceivedFrame.Count != 0)
                    {
                        ReceivedFrame.AddRange(buff);
                        data = ReceivedFrame.ToArray();
                    }
                    else
                    {
                        data = buff;
                    }                    
                    if (m_Base.ServerID == null)
                    {
                        object sid = null, cid = null;
                        GetAddress(data, ref cid, ref sid);
                        //If there is not enought data yet.
                        if (cid == null || sid == null)
                        {
                            ReceivedFrame.AddRange(buff);
                            return null;
                        }
                        foreach (object it in this.ServerIDs)
                        {
                            if (sid.Equals(it))
                            {
                                m_Base.ServerID = sid;
                                m_Base.ClientID = cid;
                                break;
                            }
                        }
                        //We do not communicate if server ID not found.
                        if (m_Base.ServerID == null)
                        {
                            System.Diagnostics.Debug.WriteLine("Invalid server ID: " + sid.ToString());
                            InvalidConnection(new ConnectionEventArgs(sid));
                            return null;
                        }
                    }
                    if (!m_Base.IsDLMSPacketComplete(data))
                    {
                        if (ReceivedFrame.Count == 0)
                        {
                            ReceivedFrame.AddRange(buff);
                        }
                        return null; //Wait more data.
                    }
                    List<object> names = new List<object>();
                    GXDLMSObject item = null;
                    byte[] allData = null;
                    byte frame;
                    RequestTypes ret = m_Base.GetDataFromPacket(data, ref allData, out frame, out command);
                    ReceivedFrame.Clear();
                    //Ask next part.
                    if ((ret & (RequestTypes.Frame | RequestTypes.DataBlock)) != 0)
                    {
                        if (ret == RequestTypes.DataBlock)
                        {
                            //Add new data.
                            if ((frame & 0x1) == 0)
                            {
                                if (command != 0)
                                {
                                    LastCommand = command;
                                }
                                ReceivedData.AddRange(allData);
                                SendData.Clear();
                                FrameIndex = 0;                                
                                return m_Base.ReceiverReady(RequestTypes.DataBlock);
                            }
                            ++FrameIndex;
                            int index = 0;
                            int BlockIndex = (int)GXCommon.GetUInt32(allData, ref index);
                            return SendData[FrameIndex];
                        }
                        else
                        {
                            ++FrameIndex;
                            //Add new data.
                            if ((frame & 0x1) == 0)
                            {
                                if (command != 0)
                                {
                                    LastCommand = command;
                                }
                                ReceivedData.AddRange(allData);
                                SendData.Clear();
                                FrameIndex = 0;
                                --m_Base.ExpectedFrame;
                                byte[] tmp = m_Base.ReceiverReady(RequestTypes.Frame);                                
                                return tmp;
                            }
                            //Keep alive...
                            else if (FrameIndex >= SendData.Count && (frame & 0x1) == 1)                            
                            {
                                SendData.Clear();
                                FrameIndex = 0;
                                return m_Base.AddFrame(m_Base.GenerateAliveFrame(), false, null, 0, 0);
                            }
                            return SendData[FrameIndex];
                        }
                    }
                    if (ReceivedData.Count != 0)
                    {
                        ReceivedData.AddRange(allData);
                        allData = ReceivedData.ToArray();
                        ReceivedData.Clear();
                        command = LastCommand;
                    }
                    FrameIndex = 0;
                    SendData.Clear();
                    if (command == (byte) Command.GloGetRequest ||
                        command == (byte) Command.GloSetRequest ||
                        command == (byte) Command.GloMethodRequest)
                    {
                        Command cmd;
                        int error;
                        allData = m_Base.Decrypt(allData.ToArray(), out cmd, out error);
                        command = (byte) cmd;
                    }
                    if (command == (int)Command.Snrm)
                    {
                        SendData.Add(HandleSnrmRequest());
                        return SendData[FrameIndex];
                    }
                    else if (command == (int)Command.Aarq)
                    {
                        SendData.Add(HandleAARQRequest(data));
                        return SendData[FrameIndex];
                    }                 
                    else if (command == (int)Command.DisconnectRequest)
                    {
                        System.Diagnostics.Debug.WriteLine("Disconnecting");
                        SendData.Add(GenerateDisconnectRequest());
                        return SendData[FrameIndex];
                    }
                    else if (command == (int)Command.WriteRequest)
                    {
                        int attributeIndex = 0;
                        object value = null;
                        ObjectType type;
                        int selector;
                        GetCommand(command, allData, out type, names, out attributeIndex, out selector, out value);
                        ushort sn = (ushort)names[0];
                        foreach (var it in SortedItems)
                        {
                            int aCnt = (it.Value as IGXDLMSBase).GetAttributeCount();
                            if (sn >= it.Key && sn <= (it.Key + (8 * aCnt)))
                            {
                                item = it.Value;
                                attributeIndex = ((sn - item.ShortName) / 8) + 1;
                                //If write is denied.
                                AccessMode acc = item.GetAccess(attributeIndex);
                                if (acc == AccessMode.NoAccess || acc == AccessMode.Read ||
                                    acc == AccessMode.AuthenticatedRead)
                                {
                                    SendData.AddRange(ServerReportError((Command)command, 3));
                                    return SendData[FrameIndex];
                                }
                                if (value is byte[])
                                {
                                    DataType tp = item.GetUIDataType(attributeIndex);
                                    if (tp != DataType.None)
                                    {
                                        value = GXDLMSClient.ChangeType((byte[])value, tp);
                                    }
                                }
                                ValueEventArgs e = new ValueEventArgs(item, attributeIndex, selector);
                                e.Value = value;
                                Write(e);
                                if (!e.Handled)
                                {
                                    (item as IGXDLMSBase).SetValue(attributeIndex, value);
                                }
                                //Return OK.
                                SendData.AddRange(Acknowledge(UseLogicalNameReferencing ? Command.SetResponse : Command.WriteResponse, 0));
                                return SendData[FrameIndex];
                            }
                        }
                    }
                    else if (command == (int)Command.SetRequest)
                    {
                        int attributeIndex = 0;
                        object value = null;
                        ObjectType type;
                        int selector;
                        GetCommand(command, allData, out type, names, out attributeIndex, out selector, out value);
                        item = Items.FindByLN(type, names[0].ToString());
                        if (item != null)
                        {
                            //If write is denied.
                            AccessMode acc = item.GetAccess(attributeIndex);
                            if (acc == AccessMode.NoAccess || acc == AccessMode.Read ||
                                acc == AccessMode.AuthenticatedRead)
                            {
                                SendData.AddRange(ServerReportError((Command)command, 3));
                                return SendData[FrameIndex];
                            }
                            if (value is byte[])
                            {
                                DataType tp = item.GetUIDataType(attributeIndex);
                                if (tp != DataType.None)
                                {
                                    value = GXDLMSClient.ChangeType((byte[])value, tp);
                                }
                            }
                            ValueEventArgs e = new ValueEventArgs(item, attributeIndex, selector);
                            e.Value = value;
                            Write(e);
                            if (!e.Handled)
                            {
                                (item as IGXDLMSBase).SetValue(attributeIndex, value);
                            }
                            //Return OK.
                            SendData.AddRange(Acknowledge(UseLogicalNameReferencing ? Command.SetResponse : Command.WriteResponse, 0));
                            return SendData[FrameIndex];
                        }
                    }
                    else if (command == (int)Command.ReadRequest && !UseLogicalNameReferencing)
                    {
                        ObjectType type;
                        int attributeIndex;
                        object value;
                        int selector;
                        GetCommand(command, allData, out type, names, out attributeIndex, out selector, out value);
                        ushort sn = Convert.ToUInt16(names[0]);
                        foreach (var it in SortedItems)
                        {
                            int aCnt = (it.Value as IGXDLMSBase).GetAttributeCount();
                            if (sn >= it.Key && sn <= (it.Key + (8 * aCnt)))
                            {
                                item = it.Value;
                                attributeIndex = ((sn - item.ShortName) / 8) + 1;
                                System.Diagnostics.Debug.WriteLine(string.Format("Reading {0}, attribute index {1}", item.Name, attributeIndex));
                                ValueEventArgs e = new ValueEventArgs(item, attributeIndex, selector);
                                e.Value = value;
                                Read(e);
                                if (e.Handled)
                                {
                                    Gurux.DLMS.DataType tp = Gurux.DLMS.Internal.GXCommon.GetValueType(e.Value);
                                    SendData.AddRange(ReadReply(names[0], type, attributeIndex, e.Value, tp));
                                    return SendData[FrameIndex];
                                }
                                if (item != null)
                                {
                                    return GetValue(names[0], item, attributeIndex, selector, value);
                                }
                            }
                            //If action.
                            else if (sn >= it.Key + aCnt && (it.Value as IGXDLMSBase).GetMethodCount() != 0)
                            {
                                //Convert DLMS data to object type.
                                int value2 = 0, count = 0;
                                GXDLMS.GetActionInfo(it.Value.ObjectType, out value2, out count);
                                if (sn <= it.Key + value2 + (8 * count))//If action
                                {
                                    item = it.Value;
                                    attributeIndex = ((sn - item.ShortName - value2) / 8) + 1;
                                    ValueEventArgs e = new ValueEventArgs(item, attributeIndex, selector);
                                    System.Diagnostics.Debug.WriteLine(string.Format("Action on {0}, attribute index {1}", item.ShortName, attributeIndex));
                                    e.Value = value;
                                    Action(e);
                                    if (!e.Handled)
                                    {
                                        byte[][] reply = (item as IGXDLMSBase).Invoke(this, attributeIndex, e.Value);
                                        if (reply != null)
                                        {
                                            SendData.AddRange(reply);
                                            return SendData[FrameIndex];
                                        }
                                    }
                                    SendData.AddRange(Acknowledge(Command.MethodResponse, 0));
                                    return SendData[FrameIndex];
                                }
                            }
                        }                           
                        throw new ArgumentOutOfRangeException();
                    }
                    else if (command == (int)Command.GetRequest && UseLogicalNameReferencing)
                    {
                        ObjectType type;
                        int index;
                        object parameter;
                        int selector;
                        GetCommand(command, allData, out type, names, out index, out selector, out parameter);
                        System.Diagnostics.Debug.WriteLine(string.Format("Reading {0}, attribute index {1}", names[0], index));
                        item = Items.FindByLN(type, names[0].ToString());
                        if (item != null)
                        {
                            ValueEventArgs e = new ValueEventArgs(item, index, selector);
                            Read(e);
                            if (e.Handled)
                            {
                                Gurux.DLMS.DataType tp = Gurux.DLMS.Internal.GXCommon.GetValueType(e.Value);
                                SendData.AddRange(ReadReply(names[0], type, index, e.Value, tp));
                                return SendData[FrameIndex];
                            }
                            return GetValue(names[0], item, index, selector, parameter);
                        }
                    }
                    else if (command == (int)Command.MethodRequest)
                    {
                        ObjectType type;
                        int index;
                        object parameter;
                        int selector;
                        GetCommand(command, allData, out type, names, out index, out selector, out parameter);
                        item = Items.FindByLN(type, names[0].ToString());
                        if (item != null)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Action on {0}, attribute index {1}", names[0], index));
                            ValueEventArgs e = new ValueEventArgs(item, index, selector);
                            e.Value = parameter;
                            Action(e);
                            if (!e.Handled && item is IGXDLMSBase)
                            {
                                byte[][] reply = (item as IGXDLMSBase).Invoke(this, index, e.Value);
                                if (reply != null)
                                {
                                    SendData.AddRange(reply);
                                    return SendData[FrameIndex];
                                }
                            }
                            SendData.AddRange(Acknowledge(Command.MethodResponse, 0));
                            return SendData[FrameIndex];
                        }
                    }
                    //Return HW error.
                    SendData.AddRange(ServerReportError((Command) command, 1));
                    return SendData[FrameIndex];
                }
                catch(Exception ex)
                {
                    //Return HW error.
                    ReceivedFrame.Clear();
                    SendData.AddRange(ServerReportError((Command) command, 1));
                    return SendData[FrameIndex];
                }
            }
        }
        
        /// <summary>
        /// Generates a read message.
        /// </summary>
        /// <param name="name">Short or Logical Name.</param>
        /// <param name="objectType">Read Interface.</param>
        /// <param name="attributeOrdinal">Read attribute index.</param>
        /// <returns>Read request as byte array.</returns>
        public byte[][] ReadReply(object name, ObjectType objectType, int attributeOrdinal, object value, DataType type)
        {
            if ((objectType != ObjectType.None && attributeOrdinal < 0))
            {
                throw new GXDLMSException("Invalid parameter");
            }
            List<byte> data = new List<byte>();
            GXCommon.SetData(data, type, value);
            return m_Base.GenerateMessage(name, data.ToArray(), objectType, attributeOrdinal, this.UseLogicalNameReferencing ? Command.GetResponse : Command.ReadResponse);
        }

        internal byte[][] Acknowledge(Command cmd, byte status)
        {
            return Acknowledge(cmd, status, null, DataType.None);
        }

        /// <summary>
        /// Generates a acknowledge message.
        /// </summary>
        internal byte[][] Acknowledge(Command cmd, byte status, Object data, DataType type)
        {
            List<byte> buff = new List<byte>(10);
            //Get request normal
            if (!UseLogicalNameReferencing)
            {
                buff.Add(0x01);
                buff.Add(status);
            }
            if (type != DataType.None)
            {
                buff.Add(0x01);
                buff.Add(0x00);
                GXCommon.SetData(buff, type, data);
            }
            int index = 0;
            return m_Base.SplitToFrames(buff, 1, ref index, buff.Count, cmd, 0);
        }

        /// <summary>
        /// Generates a acknowledge message.
        /// </summary>
        internal byte[][] ServerReportError(Command cmd, byte serviceErrorCode)
        {
            List<byte> buff = new List<byte>(10);
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
                    throw new Exception("Invalid Command.");
            }
            if (!UseLogicalNameReferencing)
            {
                buff.Add(0x01);
                buff.Add(0x01);
                buff.Add(serviceErrorCode);
            }            
            int index = 0;
            return m_Base.SplitToFrames(buff, 1, ref index, buff.Count, cmd, serviceErrorCode);
        }
    }
}
