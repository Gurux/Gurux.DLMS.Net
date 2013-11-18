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
            //Set default system title to transport security.
            Ciphering.SystemTitle = ASCIIEncoding.ASCII.GetBytes("GRX12345");
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
        private void GetCommand(byte[] data, out ObjectType type, out object name, out int attributeIndex, out byte[] parameters)
        {
            type = ObjectType.None;
            parameters = null;
            int index = 0;
            if (this.UseLogicalNameReferencing)
            {
                type = (ObjectType)GXCommon.GetUInt16(data, ref index);
                string str = null;
                List<byte> tmp = new List<byte>(GXCommon.RawData(data, ref index, 6));
                foreach (byte it in tmp)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        str += ".";
                    }
                    str += it.ToString();
                }
                name = str;
                attributeIndex = data[index++];
                //Skip data index
                ++index;
                int cnt = data.Length - index;
                if (cnt != 0)
                {
                    parameters = new byte[cnt];
                    Array.Copy(data, index, parameters, 0, cnt);
                }
            }
            else
            {
                attributeIndex = 0;
                int cnt = data[index++];
                ++index;//Len.
                name = GXCommon.GetUInt16(data, ref index);
                cnt = data.Length - index;
                if (cnt != 0)
                {
                    parameters = new byte[cnt];
                    Array.Copy(data, index, parameters, 0, cnt);
                }
            }
        }

        List<byte> GetAccessRights(GXDLMSObject item)
        {
            GXAttributeCollection attribs = new GXAttributeCollection();
            attribs.AddRange(item.Attributes);
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(item);
            foreach (PropertyDescriptor it in pdc)
            {
                GXDLMSAttribute att = it.Attributes[typeof(GXDLMSAttribute)] as GXDLMSAttribute;
                if (att != null && attribs.Find(att.Index) == null)
                {
                    //TODO: attribs.Add(att);
                }
            }
            List<byte> data = new List<byte>();
            data.Add((byte)DataType.Structure);
            data.Add(2);
            data.Add((byte)DataType.Array);
            data.Add((byte)attribs.Count);
            foreach (GXDLMSAttributeSettings att in attribs)
            {
                data.Add((byte)DataType.Structure); //attribute_access_item
                data.Add(3);
                GXCommon.SetData(data, DataType.Int8, att.Index);
                GXCommon.SetData(data, DataType.Enum, att.Access);
                GXCommon.SetData(data, DataType.None, 0);
            }
            data.Add((byte)DataType.Array);
            data.Add((byte)item.MethodAttributes.Count);            
            foreach (GXDLMSAttributeSettings it in item.MethodAttributes)
            {
                data.Add((byte)DataType.Structure); //attribute_access_item
                data.Add(2);
                GXCommon.SetData(data, DataType.Int8, it.Index);
                GXCommon.SetData(data, DataType.Enum, it.MethodAccess);
            }
            return data;
        }

        /// <summary>
        /// Returns access rights collection.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        List<byte> GetAccessRights(GXDLMSObjectCollection items)
        {
            List<byte> data = new List<byte>();            
            data.Add((byte)DataType.None);
            /* TODO: Implement Access rights...
            data.Add((byte)DataType.Array);
            GXCommon.SetObjectCount(items.Length, data);
            foreach (GXDLMSObject it in items)
            {
                data.AddRange(GetAccessRights(it));
            }
             * */
            return data;
        }               
       
        /// <summary>
        /// Returns Association View.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        byte[][] GetObjects(GXDLMSObjectCollection items)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)DataType.Array);
            //Add count
            GXCommon.SetObjectCount(items.Count, data);
            foreach (GXDLMSObject it in items)
            {
                data.Add((byte)DataType.Structure);
                data.Add(4);//Count    
                if (UseLogicalNameReferencing)
                {
                    GXCommon.SetData(data, DataType.UInt16, it.ObjectType);//ClassID
                    GXCommon.SetData(data, DataType.UInt8, it.Version);//Version
                    GXCommon.SetData(data, DataType.OctetString, it.LogicalName);//LN                    
                    data.AddRange(GetAccessRights(it)); //Access rights.
                }
                else
                {
                    GXCommon.SetData(data, DataType.UInt16, it.ShortName);//base address.
                    GXCommon.SetData(data, DataType.UInt16, it.ObjectType);//ClassID
                    GXCommon.SetData(data, DataType.UInt8, 0);//Version
                    GXCommon.SetData(data, DataType.OctetString, it.LogicalName);//LN
                }
            }
            return m_Base.SplitToBlocks(data, Command.GetResponse);
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
                        foreach (GXDLMSObject obj in pg.CaptureObjects)
                        {
                            if (obj.SelectedAttributeIndex < 1)
                            {
                                throw new Exception("Invalid attribute index. SelectedAttributeIndex is not set for " + obj.Name);
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
                buff.InsertRange(0, new byte[] { 0xE6, 0xE7, 0x00 });
            }
            m_Base.ExpectedFrame = 0;
            m_Base.FrameSequence = -1;            
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
            return m_Base.AddFrame((byte)FrameType.UA, false, buff, 0, buff.Count);
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
            ReceivedData.Clear();
            m_Base.ServerID = null;
            m_Base.ClientID = null;
        }

        private byte[] GetValue(object name, GXDLMSObject item, int index, byte[] parameters)
        {
            IGXDLMSBase tmp = item as IGXDLMSBase;
            object value = null;
            Gurux.DLMS.DataType tp = DataType.None;
            if (tmp != null)
            {
                value = tmp.GetValue(index, out tp, parameters, true);
            }
            else
            {
                object[] values = item.GetValues();
                if (index <= values.Length)
                {
                    value = values[index - 1];
                }
            }
            if (tp == DataType.None)
            {
                tp = item.GetDataType(index);
            }
            if (tp == DataType.None)
            {
                tp = Gurux.DLMS.Internal.GXCommon.GetValueType(value);
            }
            if (tp != DataType.None || (value == null && tp == DataType.None))
            {
                SendData.AddRange(ReadReply(name, item.ObjectType, index, value, tp));
                return SendData[FrameIndex];
            }
            //Return HW error.
            SendData.Add(ServerReportError(1, 5, 3));
            return SendData[FrameIndex];
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
                    if (ReceivedData.Count != 0)
                    {
                        ReceivedData.AddRange(buff);
                        data = ReceivedData.ToArray();
                    }
                    else
                    {
                        data = buff;
                    }
                    /* TODO:
                    if (Protocol == StartProtocolType.IEC)
                    {
                        if (buff.Length > 6 && Gurux.Shared.GXCommon.IndexOf(data, new byte[] { 0x0d, 0x0a}, 0, data.Length) != -1)
                        {
                        //Change To Programming Mode
                        int pos = Gurux.Shared.GXCommon.IndexOf(data, new byte[] { 0x06, 0x02}, 0, data.Length);
                        if (pos != -1)
                        {
                            pos += 2;
                            int BaudRate = 0;
                            char baudrate = (char) data[pos++];
                            switch (baudrate)
                            {
                                case '0':
                                    BaudRate = 300;
                                    break;
                                case '1':
                                    BaudRate = 600;
                                    break;
                                case '2':
                                    BaudRate = 1200;
                                    break;
                                case '3':
                                    BaudRate = 2400;
                                    break;
                                case '4':
                                    BaudRate = 4800;
                                    break;                            
                                case '6':
                                    BaudRate = 19200;
                                    break;
                                case '5':
                                default:
                                    BaudRate = 9600;
                                    break;
                            }
                            if (data[pos++] != '2') // ModeControlCharacter
                            {
                                return null;
                            }
                            return null;
                        }
                        string str = ASCIIEncoding.ASCII.GetString(data);
                        if (str == "/?!\r\n")
                        {
                            str = "/" + ManufacturerID + BaudRateID.ToString() + "\\" + DeviceID + "\r\n";
                            return ASCIIEncoding.ASCII.GetBytes(str);
                        }
                        else
                        {
                            return null;
                        }
                        }
                        else
                        {
                            return null;
                        }
                    }
                     * */
                    if (m_Base.ServerID == null)
                    {
                        object sid = null, cid = null;
                        GetAddress(data, ref cid, ref sid);
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
                            InvalidConnection(new ConnectionEventArgs(sid));
                            return null;
                        }
                    }
                    if (!m_Base.IsDLMSPacketComplete(data))
                    {
                        if (ReceivedData.Count == 0)
                        {
                            ReceivedData.AddRange(buff);
                        }
                        return null; //Wait more data.
                    }
                    object name;
                    GXDLMSObject item = null;
                    byte[] allData = null;
                    byte frame;
                    byte command;
                    RequestTypes ret = m_Base.GetDataFromPacket(data, ref allData, out frame, out command);
                    System.Diagnostics.Debug.WriteLine(Convert.ToString(frame, 16));
                    ReceivedData.Clear();
                    //Ask next part.
                    if ((ret & (RequestTypes.Frame | RequestTypes.DataBlock)) != 0)
                    {
                        if (ret == RequestTypes.DataBlock)
                        {
                            ++FrameIndex;
                            int index = 0;
                            int BlockIndex = (int)GXCommon.GetUInt32(allData, ref index);
                            return SendData[FrameIndex];
                        }
                        else
                        {
                            ++FrameIndex;
                            //Keep alive...
                            if (FrameIndex >= SendData.Count)
                            {
                                SendData.Clear();
                                FrameIndex = 0;
                                SendData.Add(m_Base.AddFrame(m_Base.GenerateAliveFrame(), false, null, 0, 0));
                                return SendData[FrameIndex];
                            }
                            return SendData[FrameIndex];
                        }
                    }
                    //If I-frame
                    if ((frame & 0x1) == 0)
                    {
                        //TODO: Search item.
                    }
                    FrameIndex = 0;
                    SendData.Clear();
                    if (command == (byte) Command.GloGetRequest ||
                        command == (byte) Command.GloSetRequest ||
                        command == (byte) Command.GloMethodRequest)
                    {
                        Command cmd;
                        allData = m_Base.Decrypt(allData.ToArray(), out cmd);
                        command = (byte) cmd;
                    }
                    if (command == (int)Command.Aarq)
                    {
                        SendData.Add(HandleAARQRequest(data));
                        return SendData[FrameIndex];
                    }
                    else if (command == (int)Command.Snrm)
                    {
                        SendData.Add(HandleSnrmRequest());
                        return SendData[FrameIndex];
                    }
                    else if (command == (int)Command.DisconnectRequest)
                    {
                        SendData.Add(GenerateDisconnectRequest());
                        return SendData[FrameIndex];
                    }
                    else if (command == (int)Command.WriteRequest ||
                        (command == (int)Command.SetRequest))
                    {
                        int index = 0;
                        object value = null;
                        if (!UseLogicalNameReferencing && command == (int)Command.WriteRequest)
                        {
                            int cnt = allData[index++];//Get item count.
                            int len = allData[index++];//Get item len.
                            ushort sn = GXCommon.GetUInt16(allData, ref index);
                            //Convert DLMS data to object type.
                            int count, index2, pos = 0;
                            foreach (var it in SortedItems)
                            {
                                if (it.Key > sn)
                                {
                                    break;
                                }
                                item = it.Value;
                            }
                            int attributeIndex = ((sn - item.ShortName) / 8) + 1;
                            System.Diagnostics.Debug.WriteLine(string.Format("Writing {0}, attribute index {1}", item.Name, attributeIndex));
                            DataType type = item.GetDataType(attributeIndex);
                            DataType type2 = DataType.None;
                            value = GXCommon.GetData(allData, ref index, ActionType.None, out count, out index2, ref type2, ref pos);
                            if (value is byte[] && type != DataType.None)
                            {
                                value = GXDLMSClient.ChangeType((byte[])value, type);
                            }
                            index = attributeIndex;
                        }
                        else if (command == (int)Command.SetRequest)
                        {
                            ObjectType type;
                            byte[] parameter;
                            GetCommand(allData, out type, out name, out index, out parameter);
                            DataType type2 = DataType.None;
                            int pos = 0, index2, count, index3 = 0;
                            value = GXCommon.GetData(parameter, ref index3, ActionType.None, out count, out index2, ref type2, ref pos);
                            item = Items.FindByLN(type, name.ToString());
                        }
                        if (item != null)
                        {
                            if (value is byte[])
                            {
                                DataType tp = item.GetUIDataType(index);
                                if (tp != DataType.None)
                                {
                                    value = GXDLMSClient.ChangeType((byte[])value, tp);
                                }
                            }
                            if (item != null)
                            {
                                ValueEventArgs e = new ValueEventArgs(item, index);
                                e.Value = value;
                                Write(e);
                                if (e.Handled)
                                {
                                    SendData.Add(Acknowledge(UseLogicalNameReferencing ? Command.SetResponse : Command.WriteResponse, 0));
                                    return SendData[FrameIndex];
                                }
                            }
                            (item as IGXDLMSBase).SetValue(index, value, true);
                            //Return OK.
                            SendData.Add(Acknowledge(UseLogicalNameReferencing ? Command.SetResponse : Command.WriteResponse, 0));
                            return SendData[FrameIndex];
                        }
                    }
                    else if (command == (int)Command.ReadRequest && !UseLogicalNameReferencing)
                    {
                        ObjectType type;
                        int index;
                        byte[] parameter;
                        GetCommand(allData, out type, out name, out index, out parameter);
                        ushort sn = Convert.ToUInt16(name);
                        foreach (var it in SortedItems)
                        {
                            if (it.Key > sn)
                            {
                                break;
                            }
                            item = it.Value;
                        }
                        index = ((sn - item.ShortName) / 8) + 1;
                        System.Diagnostics.Debug.WriteLine(string.Format("Reading {0}, attribute index {1}", item.Name, index));
                        ValueEventArgs e = new ValueEventArgs(item, index);
                        Read(e);
                        if (e.Handled)
                        {
                            Gurux.DLMS.DataType tp = Gurux.DLMS.Internal.GXCommon.GetValueType(e.Value);
                            SendData.AddRange(ReadReply(name, type, index, e.Value, tp));
                            return SendData[FrameIndex];
                        }                        
                        if (item != null)
                        {
                            return GetValue(name, item, index, parameter);
                        }
                    }
                    else if (command == (int)Command.GetRequest && UseLogicalNameReferencing)
                    {
                        ObjectType type;
                        int index;
                        byte[] parameter;
                        GetCommand(allData, out type, out name, out index, out parameter);
                        System.Diagnostics.Debug.WriteLine(string.Format("Reading {0}, attribute index {1}", name, index));
                        item = Items.FindByLN(type, name.ToString());
                        if (item != null)
                        {
                            ValueEventArgs e = new ValueEventArgs(item, index);
                            Read(e);
                            if (e.Handled)
                            {
                                Gurux.DLMS.DataType tp = Gurux.DLMS.Internal.GXCommon.GetValueType(e.Value);
                                SendData.AddRange(ReadReply(name, type, index, e.Value, tp));
                                return SendData[FrameIndex];
                            }                           
                            return GetValue(name, item, index, parameter);
                        }
                    }
                    else if (command == (int)Command.MethodRequest)
                    {
                        ObjectType type;
                        int index;
                        byte[] parameter;
                        object p = null;
                        GetCommand(allData, out type, out name, out index, out parameter);
                        if (parameter != null)
                        {
                            DataType dtype = DataType.None;
                            int read, total, index2 = 0, cache = 0;
                            p = GXCommon.GetData(parameter, ref index2, ActionType.None,
                                out total, out read, ref dtype, ref cache);
                        }
                        if (UseLogicalNameReferencing)
                        {
                            item = Items.FindByLN(type, name.ToString());
                        }
                        else
                        {
                            ushort sn = Convert.ToUInt16(name);
                            foreach (var it in SortedItems)
                            {
                                if (it.Key > sn)
                                {
                                    break;
                                }
                                item = it.Value;
                            }
                            int value, count;
                            GXDLMS.GetActionInfo(item.ObjectType, out value, out count);
                            index = ((sn - item.ShortName - value) / 8) + 1;
                        }
                        if (item != null)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Action on {0}, attribute index {1}", name, index));
                            ValueEventArgs e = new ValueEventArgs(item, index);
                            e.Value = p;
                            Action(e);
                            if (!e.Handled && item is IGXDLMSBase)
                            {
                                byte[] reply = (item as IGXDLMSBase).Invoke(this, index, e.Value);
                                if (reply != null)
                                {
                                    SendData.Add(reply);
                                    return SendData[FrameIndex];
                                }
                            }
                            SendData.Add(Acknowledge(Command.MethodResponse, 0));
                            return SendData[FrameIndex];
                        }
                    }
                    //Return HW error.
                    SendData.Add(ServerReportError(1, 5, 3));
                    return SendData[FrameIndex];
                }
                catch
                {
                    //Return HW error.
                    ReceivedData.Clear();
                    SendData.Add(ServerReportError(1, 5, 3));
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
            return m_Base.GenerateMessage(name, 0, data.ToArray(), objectType, attributeOrdinal, this.UseLogicalNameReferencing ? Command.GetResponse : Command.ReadResponse);
        }

        internal byte[] Acknowledge(Command cmd, byte status)
        {
            return Acknowledge(cmd, status, null, DataType.None);
        }

        /// <summary>
        /// Generates a acknowledge message.
        /// </summary>
        internal byte[] Acknowledge(Command cmd, byte status, Object data, DataType type)
        {
            List<byte> buff = new List<byte>(10);                   
            //Get request normal
            buff.Add((byte) cmd);
            buff.Add(0x01);
            //Invoke ID and priority.
            buff.Add(m_Base.GetInvokeIDPriority());
            buff.Add(status);
            if (type != DataType.None)
            {
                buff.Add(0x01);
                buff.Add(0x00);
                GXCommon.SetData(buff, type, data);
            }
            return m_Base.AddFrame(m_Base.GenerateIFrame(), false, buff, 0, buff.Count);
        }

        /// <summary>
        /// Generates a acknowledge message.
        /// </summary>
        internal byte[] ServerReportError(byte serviceErrorCode, byte type, byte code)
        {
            List<byte> buff = new List<byte>(10);           
            byte cmd;
            if (m_Base.Server)
            {
                if (this.UseLogicalNameReferencing)
                {
                    cmd = (byte)Command.GetResponse;
                }
                else
                {
                    cmd = (byte)Command.ReadResponse;
                }
            }
            else
            {
                if (this.UseLogicalNameReferencing)
                {
                    cmd = (byte)Command.GetRequest;
                }
                else
                {
                    cmd = (byte)Command.ReadRequest;
                }
            }

            //Get request normal
            buff.Add(cmd);
            if (this.UseLogicalNameReferencing)
            {
                buff.Add(0x01);                
            }
            buff.Add(serviceErrorCode);
            //Invoke ID and priority.
            buff.Add(type);
            buff.Add(code);
            return m_Base.AddFrame(m_Base.GenerateIFrame(), false, buff, 0, buff.Count);
        }
    }
}
