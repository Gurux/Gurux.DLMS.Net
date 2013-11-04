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
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects;
using Gurux.DLMS.ManufacturerSettings;
using System.Reflection;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMS implements methods to communicate with DLMS/COSEM metering devices.
    /// </summary>
    public class GXDLMSClient
    {       
        GXDLMS m_Base;        
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSClient()
        {
            m_Base = new GXDLMS(false);
            this.Authentication = Authentication.None;
        }
        
        /// <summary>
        /// List of available obis codes.
        /// </summary>
        /// <remarks>
        /// This list is used when Association view is read from the meter and description of the object is needed.
        /// If collection is not set description of object is empty.
        /// </remarks>
        public Gurux.DLMS.ManufacturerSettings.GXObisCodeCollection ObisCodes
        {
            get;
            set;
        }

        /// <summary>
        /// Checks, whether the received packet is a reply to the sent packet.
        /// </summary>
        /// <param name="sendData">The sent data as a byte array. </param>
        /// <param name="receivedData">The received data as a byte array.</param>
        /// <returns>True, if the received packet is a reply to the sent packet. False, if not.</returns>
        public bool IsReplyPacket(byte[] sendData, byte[] receivedData)
        {
            return m_Base.IsReplyPacket(sendData, receivedData);
        }

        /// <summary>
        /// Checks, whether the received packet is a reply to the previous sent packet.
        /// </summary>
        /// <remarks>
        /// In HDLC framing data is sometimes coming late.
        /// </remarks>
        /// <param name="sendData">The sent data as a byte array. </param>
        /// <param name="receivedData">The received data as a byte array.</param>
        /// <returns>True, if the received packet is a reply to the previous sent packet. False, if not.</returns>
        public bool IsPreviousPacket(byte[] sendData, byte[] receivedData)
        {
            return m_Base.IsPreviousPacket(sendData, receivedData);
        }        

        /// <summary>
        /// Returns frame number.
        /// </summary>
        /// <param name="data">Byte array where frame number is try to found.</param>
        /// <returns>Frame number between Zero to seven (0-7).</returns>
        public int GetFrameNumber(byte[] data)
        {
            return m_Base.GetFrameNumber(data);
        }

        /// <summary>
        /// Client ID is the identification of the device that is used as a client.
        /// Client ID is aka HDLC Address.
        /// </summary>
        public object ClientID
        {
            get
            {
                return m_Base.ClientID;
            }
            set
            {
                m_Base.ClientID = value;
            }
        }

        /// <summary>
        /// Server ID is the indentification of the device that is used as a server.
        /// Server ID is aka HDLC Address.
        /// </summary>
        public object ServerID
        {
            get
            {
                return m_Base.ServerID;
            }
            set
            {
                m_Base.ServerID = value;
            }
        }

        /// <summary>
        /// Set server ID.
        /// </summary>
        /// <remarks>
        /// This method is reserved for languages like Python where is no byte size.
        /// </remarks>
        /// <param name="value">Server ID.</param>
        /// <param name="size">Size of server ID as bytes.</param>
        public void SetServerID(object value, int size)
        {
            if (size != 1 && size != 2 && size != 4)
            {
                throw new ArgumentOutOfRangeException("size");
            }
            if (size == 1)
            {
                m_Base.ServerID = Convert.ToByte(value);
            }
            else if (size == 2)
            {
                m_Base.ServerID = Convert.ToUInt16(value);
            }
            else if (size == 4)
            {
                m_Base.ServerID = Convert.ToUInt32(value);
            }
        }

        /// <summary>
        /// Set client ID.
        /// </summary>
        /// <remarks>
        /// This method is reserved for languages like Python where is no byte size.
        /// </remarks>
        /// <param name="value">Client ID.</param>
        /// <param name="size">Size of server ID as bytes.</param>
        public void SetClientID(object value, int size)
        {
            if (size != 1 && size != 2 && size != 4)
            {
                throw new ArgumentOutOfRangeException("size");
            }
            if (size == 1)
            {
                m_Base.ClientID = Convert.ToByte(value);
            }
            else if (size == 2)
            {
                m_Base.ClientID = Convert.ToUInt16(value);
            }
            else if (size == 4)
            {
                m_Base.ClientID = Convert.ToUInt32(value);
            }
        }


        /// <summary>
        /// Are BOP, EOP and checksum added to the packet.
        /// </summary>
        public bool GenerateFrame
        {
            get
            {
                return m_Base.GenerateFrame;
            }
            set
            {
                m_Base.GenerateFrame = value;
            }
        }

        /// <summary>
        /// Is cache used. Default value is True;
        /// </summary>
        public bool UseCache
        {
            get
            {
                return m_Base.UseCache;
            }
            set
            {
                m_Base.UseCache = value;
            }
        }

        /// <summary>
        /// DLMS version number. 
        /// </summary>
        /// <remarks>
        /// Gurux DLMS component supports DLMS version number 6.
        /// </remarks>
        /// <seealso cref="SNRMRequest"/>
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
        /// <seealso cref="ClientID"/>
        /// <seealso cref="ServerID"/>
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
        /// Retrieves the password that is used in communication.
        /// </summary>
        /// <remarks>
        /// If authentication is set to none, password is not used.
        /// </remarks>
        /// <seealso cref="Authentication"/>
        public byte[] Password
        {
            get;
            set;
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
        }
       
        /// <summary>
        /// Retrieves the data type. 
        /// </summary>
        /// <param name="data">Data to parse.</param>
        /// <returns>The current data type.</returns>
        public DataType GetDLMSDataType(byte[] data)
        {
            //Return cache size.
            if (UseCache && data.Length == m_Base.CacheSize)
            {
                return m_Base.CacheType;
            }
            if (!UseCache)
            {
                m_Base.ClearProgress();
            }
            else if (m_Base.CacheIndex != 0)
            {
                return m_Base.CacheType;
            }
            DataType type;
            object value = null;
            m_Base.ParseReplyData(UseCache ? ActionType.Index : ActionType.Count, data, out value, out type);
            return type;
        }


        /// <summary>
        /// Retrieves the authentication used in communicating with the device.
        /// </summary>
        /// <remarks>
        /// By default authentication is not used. If authentication is used,
        /// set the password with the Password property.
        /// Note!
        /// For HLS authentication password (shared secret) is needed from the manufacturer.
        /// </remarks>        
        /// <seealso cref="Password"/>
        /// <seealso cref="ClientID"/>
        /// <seealso cref="ServerID"/>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="UseLogicalNameReferencing"/>
        /// <seealso cref="MaxReceivePDUSize"/>    
        [DefaultValue(Authentication.None)]
        public Authentication Authentication
        {
            get
            {
                return m_Base.Authentication;
            }
            set
            {
                m_Base.Authentication = value;
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
                m_Base.InterfaceType = value;
            }
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


        /// <summary>
        /// Generates SNRM request.
        /// </summary>
        /// <remarks>
        /// his method is used to generate send SNRMRequest. 
        /// Before the SNRM request can be generated, at least the following 
        /// properties must be set:
        /// <ul>
        /// <li>ClientID</li>
        /// <li>ServerID</li>    
        /// </ul>
        /// <b>Note! </b>According to IEC 62056-47: when communicating using 
        /// TCP/IP, the SNRM request is not send.
        /// </remarks>
        /// <returns>SNRM request as byte array.</returns>
        /// <seealso cref="ClientID"/>
        /// <seealso cref="ServerID"/>
        /// <seealso cref="ParseUAResponse"/>    
        public byte[] SNRMRequest()
        {
            IsAuthenticationRequired = false;
            m_Base.MaxReceivePDUSize = 0xFFFF;
            m_Base.ClearProgress();
            //SNRM reguest is not used in network connections.
            if (this.InterfaceType == InterfaceType.Net)
            {
                return null;
            }
            return m_Base.AddFrame((byte)FrameType.SNRM, false, null, 0, 0);
        }
        
        /// <summary>
        /// Parses UAResponse from byte array.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="data"></param>        
        /// <seealso cref="ParseUAResponse"/>        
        public void ParseUAResponse(byte[] data)
        {
            int index = 0, error;
            byte frame;            
            List<byte> arr = new List<byte>(data);
            bool packetFull, wrongCrc;
            byte command;
            m_Base.GetDataFromFrame(arr, index, out frame, true, out error, true, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            if (this.InterfaceType != InterfaceType.Net && frame != (byte)FrameType.UA)
            {
                throw new GXDLMSException("Not a UA response :" + frame);
            }
            byte FromatID = arr[index++];
            byte GroupID = arr[index++];
            byte GroupLen = arr[index++];
            object val;
            while (index < arr.Count)
            {
                HDLCInfo id = (HDLCInfo)arr[index++];
                byte len = arr[index++];
                switch (len)
                {
                    case 1:
                        val = (byte)arr[index];
                        break;
                    case 2:
                        val = BitConverter.ToUInt16(GXCommon.Swap(arr, index, len), 0);
                        break;
                    case 4:
                        val = BitConverter.ToUInt32(GXCommon.Swap(arr, index, len), 0);
                        break;
                    default:
                        throw new GXDLMSException("Invalid Exception.");
                }
                index += len;
                switch (id)
                {
                    case HDLCInfo.MaxInfoTX:
                        Limits.MaxInfoTX = val;
                        break;
                    case HDLCInfo.MaxInfoRX:
                        Limits.MaxInfoRX = val;
                        break;
                    case HDLCInfo.WindowSizeTX:
                        Limits.WindowSizeTX = val;
                        break;
                    case HDLCInfo.WindowSizeRX:
                        Limits.WindowSizeRX = val;
                        break;
                    default:
                        throw new GXDLMSException("Invalid UA response.");
                }
            }
        }

        /// <summary>
        /// Generate AARQ request. 
        /// </summary>
        /// <param name="Tags">Reserved for future use.</param>
        /// <returns>AARQ request as byte array.</returns>
        /// <seealso cref="ParseAAREResponse"/>
        /// <seealso cref="IsDLMSPacketComplete"/>
        public byte[][] AARQRequest(GXDLMSTagCollection Tags)
        {
            List<byte> buff = new List<byte>();
            m_Base.CheckInit();
            GXAPDU aarq = new GXAPDU(Tags);
            aarq.UseLN = this.UseLogicalNameReferencing;
            if (this.UseLogicalNameReferencing)
            {
                m_Base.SNSettings = null;
                m_Base.LNSettings = new GXDLMSLNSettings(new byte[] { 0x00, 0x7E, 0x1F });
                aarq.UserInformation.ConformanceBlock = LNSettings.m_ConformanceBlock;
            }
            else
            {
                m_Base.LNSettings = null;
                m_Base.SNSettings = new GXDLMSSNSettings(new byte[] { 0x1C, 0x03, 0x20 });
                aarq.UserInformation.ConformanceBlock = SNSettings.m_ConformanceBlock;
            }
            aarq.SetAuthentication(this.Authentication, Password);            
            aarq.UserInformation.DLMSVersioNumber = DLMSVersion;
            aarq.UserInformation.MaxReceivePDUSize = MaxReceivePDUSize;
            m_Base.StoCChallenge = null;
            if (Authentication > Authentication.High)
            {
                m_Base.CtoSChallenge = GXDLMS.GenerateChallenge();
            }
            else
            {
                m_Base.CtoSChallenge = null;
            }
            aarq.CodeData(buff, this.InterfaceType, m_Base.CtoSChallenge);
            m_Base.FrameSequence = -1;
            m_Base.ExpectedFrame = -1;
            return m_Base.SplitToBlocks(buff, Command.None);
        }

        /// <summary>
        /// Parses the AARE response.
        /// </summary>
        /// <param name="reply"></param>
        /// <remarks>
        /// Parse method will update the following data:
        /// <ul>
        /// <li>DLMSVersion</li>
        /// <li>MaxReceivePDUSize</li>
        /// <li>UseLogicalNameReferencing</li>
        /// <li>LNSettings or SNSettings</li>
        /// </ul>
        /// LNSettings or SNSettings will be updated, depending on the referencing, 
        /// Logical name or Short name.
        /// </remarks>
        /// <returns>The AARE response</returns>
        /// <seealso cref="AARQRequest"/>
        /// <seealso cref="UseLogicalNameReferencing"/>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="MaxReceivePDUSize"/>
        /// <seealso cref="LNSettings"/>
        /// <seealso cref="SNSettings"/>
        public GXDLMSTagCollection ParseAAREResponse(byte[] reply)
        {
            byte frame;
            int error, index = 0;            
            List<byte> arr = new List<byte>(reply);
            bool packetFull, wrongCrc;
            byte command;
            m_Base.GetDataFromFrame(arr, index, out frame, true, out error, false, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            //Parse AARE data.
            GXDLMSTagCollection Tags = new GXDLMSTagCollection();
            GXAPDU pdu = new GXAPDU(Tags);
            pdu.EncodeData(arr.ToArray(), ref index);
            UseLogicalNameReferencing = pdu.UseLN;
            if (UseLogicalNameReferencing)
            {
                System.Diagnostics.Debug.WriteLine("--- Logical Name settings are---\r\n");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("--- Short Name settings are---\r\n");
            }            
            m_Base.StoCChallenge = pdu.Password;
            System.Diagnostics.Debug.WriteLine("StoC: " + BitConverter.ToString(m_Base.StoCChallenge));
            AssociationResult ret = pdu.ResultComponent;
            if (ret == AssociationResult.Accepted)
            {
                System.Diagnostics.Debug.WriteLine("- Client has accepted connection.");
                if (UseLogicalNameReferencing)
                {
                    m_Base.LNSettings = new GXDLMSLNSettings(pdu.UserInformation.ConformanceBlock);
                }
                else
                {
                    m_Base.SNSettings = new GXDLMSSNSettings(pdu.UserInformation.ConformanceBlock);
                }
                MaxReceivePDUSize = pdu.UserInformation.MaxReceivePDUSize;
                DLMSVersion = pdu.UserInformation.DLMSVersioNumber;
            }
            else
            {
                throw new GXDLMSException(ret, pdu.ResultDiagnosticValue);
            }
            IsAuthenticationRequired = pdu.ResultDiagnosticValue == SourceDiagnostic.AuthenticationRequired;
            System.Diagnostics.Debug.WriteLine("- Server max PDU size is " + MaxReceivePDUSize);
            System.Diagnostics.Debug.WriteLine("- Value of quality of service is " + ValueOfQualityOfService);
            System.Diagnostics.Debug.WriteLine("- Server DLMS version number is " + DLMSVersion);
            if (DLMSVersion != 6)
            {
                throw new GXDLMSException("Invalid DLMS version number.");
            }
            System.Diagnostics.Debug.WriteLine("- Number of unused bits is " + NumberOfUnusedBits);
            return Tags;
        }

        /// <summary>
        /// Is authentication Required.
        /// </summary>
        public bool IsAuthenticationRequired
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Get challenge request if HLS authentication is used.
        /// </summary>
        /// <returns></returns>
        public byte[] GetApplicationAssociationRequest()
        {
            if (Password == null || Password.Length == 0)
            {
                throw new ArgumentException("Password is invalid.");
            }
            List<byte> CtoS = new List<byte>(Password);
            CtoS.AddRange(m_Base.StoCChallenge);
            byte[] challenge = GXDLMS.Chipher(this.Authentication, CtoS.ToArray());
            if (UseLogicalNameReferencing)
            {
                return Method("0.0.40.0.0.255", ObjectType.AssociationLogicalName, 1, challenge, DataType.OctetString);
            }
            return Method((ushort)0xFA00, ObjectType.AssociationShortName, 8, challenge, DataType.OctetString);            
        }

        /// <summary>
        /// Parse server's challenge if HLS authentication is used.
        /// </summary>
        /// <param name="reply"></param>
        public void ParseApplicationAssociationResponse(byte[] reply)
        {
            byte frame;
            int error, index = 0;
            List<byte> arr = new List<byte>(reply);
            bool packetFull, wrongCrc;
            byte command;
            m_Base.GetDataFromFrame(arr, index, out frame, true, out error, false, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            //Skip invoke ID and priority.
            index += 2;
            //Skip Error
            ++index;
            //Skip item count
            ++index;
            //Skip item status
            ++index;
            int total = 0, read = 0, CacheIndex = 0;
            DataType type = DataType.None;
            byte[] serverChallenge = (byte[])GXCommon.GetData(arr.ToArray(), ref index, ActionType.None, out total, out read, ref type, ref CacheIndex);
            List<byte> challenge = new List<byte>(Password);
            challenge.AddRange(m_Base.CtoSChallenge);
            byte[] clientChallenge = GXDLMS.Chipher(this.Authentication, challenge.ToArray());
            int pos = 0;
            if (!GXCommon.Compare(serverChallenge, ref pos, clientChallenge))
            {
                throw new Exception("Server returns invalid challenge.");
            }
        }

        /// <summary>
        /// Generates a disconnect mode request.
        /// </summary>
        /// <returns>Disconnect mode request, as byte array.</returns>
        public byte[] DisconnectedModeRequest()
        {
            m_Base.ClearProgress();
            //If connection is not established, there is no need to send DisconnectRequest.
            if (SNSettings == null && LNSettings == null)
            {
                return null;
            }
            //In current behavior, disconnect is not generated for network connection.
            if (this.InterfaceType != InterfaceType.Net)
            {
                return m_Base.AddFrame((byte)FrameType.DisconnectMode, false, null, 0, 0);
            }
            return null;
        }

        /// <summary>
        /// Generates a disconnect request.
        /// </summary>
        /// <returns>Disconnected request, as byte array.</returns>
        public byte[] DisconnectRequest()
        {
            m_Base.ClearProgress();
            //If connection is not established there is no need to send DisconnectRequest.
            if (SNSettings == null && LNSettings == null)
            {
                return null;
            }
            if (this.InterfaceType != InterfaceType.Net)
            {
                return m_Base.AddFrame((byte)FrameType.Disconnect, false, null, 0, 0);
            }
            List<byte> data = new List<byte>(new byte[] { 0x62, 0x0 });
            return m_Base.AddFrame((byte)FrameType.Disconnect, false, data, 0, data.Count);
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
        /// Reserved for internal use.
        /// </summary>
        /// <param name="ClassID"></param>
        /// <param name="Version"></param>
        /// <param name="BaseName"></param>
        /// <param name="LN"></param>
        /// <param name="AccessRights"></param>
        /// <param name="AttributeIndex"></param>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
        internal static GXDLMSObject CreateDLMSObject(int ClassID, object Version, int BaseName, object LN, object AccessRights, int AttributeIndex, int dataIndex)
        {
            GXDLMSObject obj = null;
            ObjectType type = (ObjectType)ClassID;
            if (GXDLMS.AvailableObjectTypes.ContainsKey(type))
            {
                Type tmp = GXDLMS.AvailableObjectTypes[type];
                obj = Activator.CreateInstance(tmp) as GXDLMSObject;
            }
            else
            {
                obj = new GXDLMSObject();
            }
            UpdateObjectData(obj, type, Version, BaseName, LN, AccessRights, AttributeIndex, dataIndex);
            return obj;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        GXDLMSObjectCollection ParseSNObjects(byte[] buff, bool onlyKnownObjects)
        {
            int index = 0;
            //Get array tag.
            byte size = buff[index++];
            //Check that data is in the array
            if (size != 0x01)
            {
                throw new GXDLMSException("Invalid response.");
            }
            GXDLMSObjectCollection items = new GXDLMSObjectCollection(this);
            long cnt = GXCommon.GetObjectCount(buff, ref index);
            int total, count;
            int[] values = null;
            if (onlyKnownObjects)
            {
                Array arr = Enum.GetValues(typeof(ObjectType));
                values = new int[arr.Length];
                arr.CopyTo(values, 0);
            }
            for (long objPos = 0; objPos != cnt; ++objPos)
            {
                DataType type = DataType.None;
                int cachePosition = 0;
                object[] objects = (object[])GXCommon.GetData(buff, ref index, ActionType.None, out total, out count, ref type, ref cachePosition);
                if (index == -1)
                {
                    throw new OutOfMemoryException();
                }
                if (objects.Length != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                int type2 = Convert.ToInt16(objects[1]);
                if (!onlyKnownObjects || values.Contains(type2))
                {
                    int baseName = Convert.ToInt32(objects[0]) & 0xFFFF;
                    if (baseName > 0)
                    {
                        GXDLMSObject comp = CreateDLMSObject(type2, objects[2], baseName, objects[3], null, 0, 0);
                        if (comp != null)
                        {
                            items.Add(comp);
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}", type2, objects[0]));
                }
            }
            return items;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="version"></param>
        /// <param name="baseName"></param>
        /// <param name="logicalName"></param>
        /// <param name="accessRights"></param>
        /// <param name="attributeIndex"></param>
        /// <param name="dataIndex"></param>
        internal static void UpdateObjectData(GXDLMSObject obj, ObjectType objectType, object version, object baseName, object logicalName, object accessRights, int attributeIndex, int dataIndex)
        {
            obj.ObjectType = objectType;
            // Check access rights...            
            if (accessRights != null && accessRights.GetType().IsArray)
            {
                //access_rights: access_right
                object[] access = (object[])accessRights;
                foreach (object[] attributeAccess in (object[])access[0])
                {
                    int id = Convert.ToInt32(attributeAccess[0]);
                    AccessMode mode = (AccessMode)Convert.ToInt32(attributeAccess[1]);
                    //TODO: Check why...
                    //With some meters id is negative. 
                    if (id > 0)
                    {
                        obj.SetAccess(id, mode);
                    }
                }
                if (obj.ShortName == 0) //If Logical Name is used.
                {
                }
                else //If Short Name is used.
                {
                    foreach (object[] methodAccess in (object[])access[1])
                    {
                        int id = Convert.ToInt32(methodAccess[0]);
                        MethodAccessMode mode = (MethodAccessMode)Convert.ToInt32(methodAccess[1]);
                        obj.SetMethodAccess(id, mode);
                    }
                }
            }
            ((IGXDLMSColumnObject)obj).SelectedAttributeIndex = attributeIndex;
            ((IGXDLMSColumnObject)obj).SelectedDataIndex = dataIndex;
            if (baseName != null)
            {
                obj.ShortName = Convert.ToUInt16(baseName);
            }
            if (version != null)
            {
                obj.Version = Convert.ToInt32(version);
            }
            if (logicalName is byte[])
            {
                obj.LogicalName = GXDLMSObject.toLogicalName((byte[])logicalName);
            }
            else
            {
                obj.LogicalName = logicalName.ToString();
            }
        }

        static void UpdateOBISCodes(GXDLMSObjectCollection objects)
        {
            GXStandardObisCodeCollection codes = new GXStandardObisCodeCollection();
            string[] rows = Gurux.DLMS.Properties.Resources.OBISCodes.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string it in rows)
            {
                string[] items = it.Split(new char[] { ';' });
                string[] obis = items[0].Split(new char[] { '.' });
                GXStandardObisCode code = new GXStandardObisCode(obis, items[3] + "; " + items[4] + "; " +
                    items[5] + "; " + items[6] + "; " + items[7], items[1], items[2]);
                codes.Add(code);
            }
            foreach (GXDLMSObject it in objects)
            {
                if (!string.IsNullOrEmpty(it.Description) && it.ObjectType != ObjectType.None)
                {
                    continue;
                }
                GXStandardObisCode code = codes.Find(it.LogicalName, it.ObjectType);                
                if (code != null)
                {
                    it.Description = code.Description;
                    //If string is used
                    if (code.DataType.Contains("10"))
                    {
                        code.DataType = "10";
                    }
                    //If date time is used.
                    else if (code.DataType.Contains("25") || code.DataType.Contains("26"))
                    {
                        code.DataType = "25";
                    }                    
                    //Time stamps of the billing periods objects (first scheme if there are two)
                    else if (code.DataType.Contains("9"))
                    {
                        if ((GXStandardObisCodeCollection.EqualsMask("0.0-64.96.7.10-14.255", it.LogicalName) ||
                            //Time stamps of the billing periods objects (second scheme)
                        GXStandardObisCodeCollection.EqualsMask("0.0-64.0.1.5.0-99,255", it.LogicalName) ||
                            //Time of power failure
                        GXStandardObisCodeCollection.EqualsMask("0.0-64.0.1.2.0-99,255", it.LogicalName) ||
                            //Time stamps of the billing periods objects (first scheme if there are two)                        
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.1.2.0-99,255", it.LogicalName) ||
                            //Time stamps of the billing periods objects (second scheme)
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.1.5.0-99,255", it.LogicalName) ||
                            //Time expired since last end of billing period
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.0.255", it.LogicalName) ||
                            //Time of last reset
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.6.255", it.LogicalName) ||
                            //Date of last reset
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.7.255", it.LogicalName) ||
                            //Time expired since last end of billing period (Second billing period scheme)
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.13.255", it.LogicalName) ||
                            //Time of last reset (Second billing period scheme)
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.14.255", it.LogicalName) ||
                            //Date of last reset (Second billing period scheme)
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.15.255", it.LogicalName)))
                        {
                            code.DataType = "25";
                        }
                        //Local time
                        else if (GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.1.255", it.LogicalName))
                        {
                            code.DataType = "27";
                        }
                        //Local date
                        else if (GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.2.255", it.LogicalName))
                        {
                            code.DataType = "26";
                        }
                    }
                    if (code.DataType != "*" && code.DataType != string.Empty && !code.DataType.Contains(","))
                    {
                        DataType type = (DataType)int.Parse(code.DataType);
                        switch (it.ObjectType)
                        {                            
                            case ObjectType.Data:
                            case ObjectType.Register:
                            case ObjectType.RegisterActivation:
                            case ObjectType.ExtendedRegister:
                                it.SetUIDataType(2, type);
                                break;
                            default:
                                break;
                        }                        
                    }                    
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unknown OBIS Code: " + it.LogicalName + " Type: " + it.ObjectType);
                }
            }
        }

        public GXDLMSObjectCollection Objects
        {
            get;
            set;
        }

        /// <summary>
        /// Parses the COSEM objects of the received data.
        /// </summary>
        /// <param name="data">Received data, from the device, as byte array. </param>
        /// <returns>Collection of COSEM objects.</returns>
        public GXDLMSObjectCollection ParseObjects(byte[] data, bool onlyKnownObjects)
        {
            if (data == null || data.Length == 0)
            {
                throw new GXDLMSException("ParseObjects failed. Invalid parameter.");
            }
            GXDLMSObjectCollection objects = null;
            if (UseLogicalNameReferencing)
            {
                objects = ParseLNObjects(data, onlyKnownObjects);
            }
            else
            {
                objects = ParseSNObjects(data, onlyKnownObjects);
            }
            UpdateOBISCodes(objects);
            Objects = objects;
            return objects;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        GXDLMSObjectCollection ParseLNObjects(byte[] buff, bool onlyKnownObjects)
        {
            int index = 0;
            byte size = buff[index++];
            //Check that data is in the array.
            if (size != 0x01)
            {
                throw new GXDLMSException("Invalid response.");
            }
            //get object count
            int cnt = GXCommon.GetObjectCount(buff, ref index);
            int objectCnt = 0;
            GXDLMSObjectCollection items = new GXDLMSObjectCollection(this);
            int total, count;
            int[] values = null;
            if (onlyKnownObjects)
            {
                Array arr = Enum.GetValues(typeof(ObjectType));
                values = new int[arr.Length];
                arr.CopyTo(values, 0);
            }
            //Some meters give wrong item count.
            while (index != buff.Length && cnt != objectCnt)
            {
                DataType type = DataType.None;
                int cachePosition = 0;
                object[] objects = (object[])GXCommon.GetData(buff, ref index, ActionType.None, out total, out count, ref type, ref cachePosition);
                if (index == -1)
                {
                    throw new OutOfMemoryException();
                }
                if (objects.Length != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                ++objectCnt;
                int type2 = Convert.ToInt16(objects[0]);
                if (!onlyKnownObjects || values.Contains(type2))
                {
                    GXDLMSObject comp = CreateDLMSObject(type2, objects[1], 0, objects[2], objects[3], 0, 0);
                    items.Add(comp);                    
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}", type2, objects[2]));
                }
            }
            return items;
        }

        /// <summary>
        /// Parse data columns.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public GXDLMSObjectCollection ParseColumns(byte[] data)
        {
            if (data == null)
            {
                throw new GXDLMSException("Invalid parameter.");
            }
            int index = 0;
            byte size = data[index++];
            //Check that data is in the array.
            if (size != 0x01)
            {
                throw new GXDLMSException("Invalid response.");
            }
            //get object count
            int cnt = GXCommon.GetObjectCount(data, ref index);
            int objectCnt = 0;
            GXDLMSObjectCollection items = new GXDLMSObjectCollection();
            int total, count;
            while (index != data.Length && cnt != objectCnt)
            {
                DataType type = DataType.None;
                int cachePosition = 0;
                object[] objects = (object[])GXCommon.GetData(data, ref index, ActionType.None, out total, out count, ref type, ref cachePosition);
                if (index == -1)
                {
                    throw new OutOfMemoryException();
                }
                if (objects.Length != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                ++objectCnt;
                GXDLMSObject comp = CreateDLMSObject(Convert.ToInt16(objects[0]), null, 0, objects[1], 0, Convert.ToInt16(objects[2]), Convert.ToInt16(objects[3]));
                if (comp != null)
                {
                    items.Add(comp);
                }
                //Update data type and scaler unit if register.
                GXDLMSObject tmp = Objects.FindByLN(comp.ObjectType, comp.LogicalName);
                if (tmp != null)
                {
                    if (comp is GXDLMSRegister)
                    {
                        int index2 = tmp.SelectedAttributeIndex;
                        //Some meters return zero.
                        if (index2 == 0)
                        {
                            index2 = 2;
                        }
                        comp.SetUIDataType(index2, tmp.GetUIDataType(index2));
                        (comp as GXDLMSRegister).Scaler = (tmp as GXDLMSRegister).Scaler;
                        (comp as GXDLMSRegister).Unit = (tmp as GXDLMSRegister).Unit;
                    }
                }
            }            
            UpdateOBISCodes(items);
            return items;
        }

        /// <summary>
        /// Get Value from byte array received from the meter.
        /// </summary>
        public object UpdateValue(byte[] data, GXDLMSObject target, int attributeIndex)
        {
            target.SetValue(attributeIndex, GetValue(data, target, attributeIndex));
            return target.GetValues()[attributeIndex - 1];
        }

        /// <summary>
        /// Get Value from byte array received from the meter.
        /// </summary>
        /// <param name="data">Reply byte array received from the meter.</param>
        /// <param name="target">Read COSEM Object.</param>
        /// <param name="attributeIndex"></param>
        /// <returns></returns>
        /// <seealso cref="TryGetValue"/>
        public object GetValue(byte[] data, GXDLMSObject target, int attributeIndex)
        {        
            DataType type = target.GetUIDataType(attributeIndex);
            Object value = GetValue(data);
            if (value is byte[] && type != DataType.None)
            {
                return GXDLMSClient.ChangeType((byte[]) value, type);
            }
            return value;
        }
        
        /// <summary>
        /// Get Value from byte array received from the meter.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="ln"></param>
        /// <param name="attributeIndex"></param>
        /// <returns></returns>
        public object GetValue(byte[] data, ObjectType type, string ln, int attributeIndex)
        {
            object value = GetValue(data);
            //If logical name.
            if (attributeIndex == 1)
            {
                return ChangeType((byte[])value, DataType.OctetString);
            }
            if (value is byte[] && ObisCodes != null)
            {
                Gurux.DLMS.ManufacturerSettings.GXObisCode code = ObisCodes.FindByLN(type, ln, null);
                if (code != null)
                {
                    GXDLMSAttributeSettings att = code.Attributes.Find(attributeIndex);
                    if (att != null && value != null && ((byte[])value).Length != 0)
                    {
                        return ChangeType((byte[])value, att.UIType);
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Get Value from byte array received from the meter.
        /// </summary>
        /// <param name="data">Byte array received from the meter.</param>
        /// <param name="rawData"></param>
        /// <returns>Received data.</returns>
        public object GetValue(byte[] data)
        {
            if (!UseCache || data.Length < m_Base.CacheIndex)
            {
                m_Base.ClearProgress();
            }
            //Return cached items.
            if (UseCache && m_Base.CacheSize == data.Length)
            {
                return m_Base.CacheData;
            }
            DataType type;
            object value = null;
            m_Base.ParseReplyData(UseCache ? ActionType.Index : ActionType.None, data, out value, out type);
            return m_Base.CacheData;
        }

        /// <summary>
        /// TryGetValue try parse multirow value from byte array to variant.
        /// </summary>
        /// <remarks>
        /// This method can be used when Profile Generic is read and if 
        /// data is need to update at collection time.
        /// Cached data is cleared after read.
        /// 
        /// Use TryGetValue if you are reading large amount of data like Load Profile.
        /// Using TryGetValue you can read data from the meter even if transmission line is bad.
        /// If communication breaks using TryGetValue you can continue reading from last success row.
        /// 
        /// Another good aspect of TryGetValue is that data is progressing while reading. You can 
        /// save data to the DB while reading and you do not have to wait until add data is read.
        /// This will smooth out the CPU load significantly after translation.
        /// </remarks>
        /// <param name="data">Byte array received from the meter.</param>        
        /// <returns>Received data.</returns>
        /// <seealso cref="UseCache">UseCache</seealso>
        /// <seealso cref="GetValue"/>
        public object TryGetValue(byte[] data)
        {
            if (!UseCache || data.Length < m_Base.CacheIndex)
            {
                m_Base.ClearProgress();
            }
            DataType type = DataType.None;
            int read, total, index = 0;
            try
            {
                //Return cached items.
                if (UseCache)
                {
                    if (m_Base.CacheSize == data.Length)
                    {
                        //Clear cached data after read.
                        object tmp = m_Base.CacheData;
                        m_Base.CacheData = null;
                        return tmp;
                    }
                    if (m_Base.CacheData != null)
                    {
                        throw new Exception("Cache data is not empty.");
                    }
                }
                object value = GXCommon.GetData(data, ref index, ActionType.None, out total, out read, ref type, ref m_Base.CacheIndex);
                if (UseCache)
                {
                    m_Base.CacheData = null;
                    m_Base.ItemCount += read;
                    m_Base.CacheSize = data.Length;
                    m_Base.MaxItemCount += total;
                }
                return value;
            }
            catch
            {
                return null;
            }
        }

        public static GXDLMSAttributeSettings GetAttributeInfo(GXDLMSObject item, int index)
        {
            GXDLMSAttributeSettings att = item.Attributes.Find(index);
            return att;
        }

        /// <summary>
        /// Changes byte array received from the meter to given type.
        /// </summary>
        /// <param name="value">Byte array received from the meter.</param>
        /// <param name="type">Wanted type.</param>
        /// <returns>Value changed by type.</returns>
        public static object ChangeType(byte[] value, DataType type)
        {
            if ((value == null || value.Length == 0) && (type == DataType.String || type == DataType.OctetString))
            {
                return string.Empty;
            }
            if (value == null)
            {
                return null;
            }
            if (type == DataType.None)
            {
                return BitConverter.ToString(value).Replace('-', ' ');
            }
            int index = 0;
            int read, total, cachePosition = 0;
            object ret = GXCommon.GetData(value, ref index, ActionType.None, out total, out read, ref type, ref cachePosition);
            if (index == -1)
            {
                throw new OutOfMemoryException();
            }
            if (type == DataType.OctetString && ret is byte[])
            {
                string str = null;
                byte[] arr = (byte[])ret;
                if (arr.Length == 0)
                {
                    str = string.Empty;
                }
                else
                {
                    foreach (byte it in arr)
                    {
                        if (str != null)
                        {
                            str += ".";
                        }
                        str += it.ToString();
                    }
                }
                return str;
            }
            return ret;
        }

         /// <summary>
        /// Reads the selected object from the device.
        /// </summary>
        /// <remarks>
        /// This method is used to get all registers in the device.
        /// </remarks>
        /// <returns>Read request, as byte array.</returns>
        public byte[] GetObjectsRequest()
        {
            object name;
            if (UseLogicalNameReferencing)
            {
                name = "0.0.40.0.0.255";
            }
            else
            {
                name = (ushort)0xFA00;
            }
            return Read(name, ObjectType.AssociationLogicalName, 2)[0];
        }

        /// <summary>
        /// Generates a write message.
        /// </summary>
        /// <param name="item">object to write.</param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <returns></returns>
        public byte[] Method(GXDLMSObject item, int index, Object data)
        {
            return Method(item.Name, item.ObjectType, index, data, DataType.None);
        }

        /// <summary>
        /// Generates a write message.
        /// </summary>
        /// <param name="item">object to write.</param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <param name="data">Additional data.</param>
        /// <param name="type">Additional data type.</param>
        /// <returns></returns>
        public byte[] Method(GXDLMSObject item, int index, Object data, DataType type)
        {
            return Method(item.Name, item.ObjectType, index, data, type);
        }

        /// <summary>
        /// Generate Method (Action) request.
        /// </summary>
        /// <param name="name">Method object short name or Logical Name.</param>
        /// <param name="objectType">Object type.</param>
        /// <param name="index">Methdod index.</param>
        /// <returns></returns>
        public byte[] Method(object name, ObjectType objectType, int index, Object data, DataType type)
        {
            if (name == null || index < 1)
            {
                throw new ArgumentOutOfRangeException("Invalid parameter");
            }
            m_Base.ClearProgress();
            if (type == DataType.None)
            {
                type = GXCommon.GetValueType(data);
            }
            List<byte> buff = new List<byte>();
            GXCommon.SetData(buff, type, data);
            if (!this.UseLogicalNameReferencing)
            {
                int value, count;
                GXDLMS.GetActionInfo(objectType, out value, out count);
                if (index > count)
                {
                    throw new ArgumentOutOfRangeException("methodIndex");
                }
                index = (value + (index - 1) * 0x8);
            }
            return m_Base.GenerateMessage(name, 0, buff.ToArray(), objectType, index, Command.MethodRequest)[0];
        }


        /// <summary>
        /// Generates a write message.
        /// </summary>
        /// <param name="item">object to write.</param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <returns></returns>
        public byte[][] Write(GXDLMSObject item, int index)
        {
            if (item == null || index < 1)
            {
                throw new GXDLMSException("Invalid parameter");
            }
            DataType type;
            Object value = (item as IGXDLMSBase).GetValue(index, out type, null, true);
            return Write(item.Name, value, type, item.ObjectType, index);
        }

        /// <summary>
        /// Generates a write message.
        /// </summary>
        /// <param name="name">Short or Logical Name.</param>
        /// <param name="value">Data to Write.</param>
        /// <param name="type">Data type of write object.</param>
        /// <param name="objectType"></param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <returns></returns>
        public byte[][] Write(object name, object value, DataType type, ObjectType objectType, int index)
        {
            if (index < 1)
            {
                throw new GXDLMSException("Invalid parameter");
            }
            if (type == DataType.None)
            {
                type = GXCommon.GetValueType(value);
            }
            m_Base.ClearProgress();
            List<byte> data = new List<byte>();
            GXCommon.SetData(data, type, value);            
            return m_Base.GenerateMessage(name, 2, data.ToArray(), objectType, index, UseLogicalNameReferencing ? Command.SetRequest : Command.WriteRequest);
        }

        /// <summary>
        /// Generates a read message.
        /// </summary>
        /// <param name="name">Short or Logical Name.</param>
        /// <param name="objectType">Read Interface.</param>
        /// <param name="attributeOrdinal">Read attribute index.</param>
        /// <returns>Read request as byte array.</returns>
        public byte[][] Read(object name, ObjectType objectType, int attributeOrdinal)
        {
            if ((attributeOrdinal < 0))
            {
                throw new GXDLMSException("Invalid parameter");
            }
            //Clear cache
            m_Base.ClearProgress();
            return m_Base.GenerateMessage(name, 2, new byte[0], objectType, attributeOrdinal, this.UseLogicalNameReferencing ? Command.GetRequest : Command.ReadRequest);
        }

        /// <summary>
        /// Generates a read message.
        /// </summary>
        /// <param name="item">DLMS object to read.</param>
        /// <param name="attributeOrdinal">Read attribute index.</param>
        /// <returns>Read request as byte array.</returns>
        public byte[][] Read(GXDLMSObject item, int attributeOrdinal)
        {
            if ((attributeOrdinal < 1))
            {
                throw new GXDLMSException("Invalid parameter");
            }
            //Clear cache
            m_Base.ClearProgress();
            return m_Base.GenerateMessage(item.Name, 2, new byte[0], item.ObjectType, attributeOrdinal, this.UseLogicalNameReferencing ? Command.GetRequest : Command.ReadRequest);
        }

        /// <summary>
        /// Generates the keep alive message. 
        /// </summary>
        /// <remarks>
        /// Keep alive message is sent to keep the connection to the device alive.
        /// </remarks>
        /// <returns>Returns Keep alive message, as byte array.</returns>
        public byte[] GetKeepAlive()
        {
            m_Base.ClearProgress();
            //There is no keepalive in IEC 62056-47.
            if (this.InterfaceType == InterfaceType.Net)
            {
                return null;
            }
            return m_Base.AddFrame(m_Base.GenerateAliveFrame(), false, null, 0, 0);
        }

        /// <summary>
        /// Read rows by entry.
        /// </summary>
        /// <param name="name">object name.</param>
        /// <param name="index">Zero bases start index.</param>
        /// <param name="count">Rows count to read.</param>
        /// <returns>Read message as byte array.</returns>
        public byte[] ReadRowsByEntry(object name, int index, int count)
        {
            m_Base.ClearProgress();
            List<byte> buff = new List<byte>();
            buff.Add(0x02);  //Add AccessSelector
            buff.Add((byte)DataType.Structure); //Add enum tag.
            buff.Add(0x04); //Add item count
            GXCommon.SetData(buff, DataType.UInt32, index); //Add start index
            GXCommon.SetData(buff, DataType.UInt32, count);//Add Count
            GXCommon.SetData(buff, DataType.UInt16, 0);//Read all columns.
            GXCommon.SetData(buff, DataType.UInt16, 0);
            return m_Base.GenerateMessage(name, 4, buff.ToArray(), ObjectType.ProfileGeneric, 2, this.UseLogicalNameReferencing ? Command.GetRequest : Command.ReadRequest)[0];
        }

        /// <summary>
        /// Read rows by range.
        /// </summary>
        /// <remarks>
        /// Use this method to read Profile Generic table between dates.
        /// </remarks>
        /// <param name="name">object name.</param>
        /// <param name="ln">The logical name of the first column.</param>
        /// <param name="objectType">The ObjectType of the first column.</param>
        /// <param name="version">The version of the first column.</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        /// <returns></returns>
        public byte[] ReadRowsByRange(object name, string ln, ObjectType objectType, int version, DateTime start, DateTime end)
        {
            GXDateTime s = new GXDateTime(start);
            s.Skip = DateTimeSkips.Ms;
            GXDateTime e = new GXDateTime(end);
            e.Skip = DateTimeSkips.Ms;
            m_Base.ClearProgress();
            List<byte> buff = new List<byte>();
            buff.Add(0x01);  //Add AccessSelector
            buff.Add((byte)DataType.Structure); //Add enum tag.
            buff.Add(0x04); //Add item count
            buff.Add(0x02); //Add enum tag.
            buff.Add(0x04); //Add item count           
            GXCommon.SetData(buff, DataType.UInt16, (ushort)8);// Add class_id	            
            GXCommon.SetData(buff, DataType.OctetString, ln);// Add parameter Logical name
            GXCommon.SetData(buff, DataType.Int8, 2);// Add attribute index.
            GXCommon.SetData(buff, DataType.UInt16, version);// Add version
            GXCommon.SetData(buff, DataType.DateTime, s);// Add start time.
            GXCommon.SetData(buff, DataType.DateTime, e);// Add end time.
            //Add array of read columns. Read All...
            buff.Add(0x01); //Add item count   
            buff.Add(0x00); //Add item count   
            return m_Base.GenerateMessage(name, 4, buff.ToArray(), ObjectType.ProfileGeneric, 2, this.UseLogicalNameReferencing ? Command.GetRequest : Command.ReadRequest)[0];
        }

        /// <summary>
        /// Create given type of COSEM object.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static GXDLMSObject CreateObject(Gurux.DLMS.ObjectType type)
        {
            return GXDLMS.CreateObject(type);
        }        

        /// <summary>
        /// Determines, whether the DLMS packet is completed.
        /// </summary>
        /// <param name="data">The data to be parsed, to complete the DLMS packet.</param>
        /// <returns>True, when the DLMS packet is completed.</returns>
        public bool IsDLMSPacketComplete(byte[] data)
        {
            return m_Base.IsDLMSPacketComplete(data);
        }

        /// <summary>
        /// Check if server (meter) is returned error.
        /// </summary>
        /// <param name="sendData">Send data.</param>
        /// <param name="receivedData">Received data.</param>
        /// <returns>List of occurred errors as id and string collection.</returns>
        public object[,] CheckReplyErrors(byte[] sendData, byte[] receivedData)
        {
            return m_Base.CheckReplyErrors(sendData, receivedData);
        }

        /// <summary>
        /// Generates an acknowledgment message, with which the server is informed to 
        /// send next packets.
        /// </summary>
        /// <param name="type">Frame type</param>
        /// <returns>Acknowledgment message as byte array.</returns>
        public byte[] ReceiverReady(RequestTypes type)
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
        /// Removes the HDLC header from the packet, and returns COSEM data only.
        /// </summary>
        /// <param name="packet">The received packet, from the device, as byte array.</param>
        /// <param name="data">The exported data.</param>
        /// <returns>COSEM data.</returns>
        public RequestTypes GetDataFromPacket(byte[] packet, ref byte[] data)
        {
            byte frame;
            byte command;
            return m_Base.GetDataFromPacket(packet, ref data, out frame, out command);
        }

        /// <summary>
        /// Returns unit text.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetUnit(Unit value)
        {
            switch (value)
            {
                case Unit.Year:
                    return Gurux.DLMS.Properties.Resources.UnitYearTxt;
                case Unit.Month:
                    return Gurux.DLMS.Properties.Resources.UnitMonthTxt;
                case Unit.Week:
                    return Gurux.DLMS.Properties.Resources.UnitWeekTxt;
                case Unit.Day:
                    return Gurux.DLMS.Properties.Resources.UnitDayTxt;
                case Unit.Hour:
                    return Gurux.DLMS.Properties.Resources.UnitHourTxt;
                case Unit.Minute:
                    return Gurux.DLMS.Properties.Resources.UnitMinuteTxt;
                case Unit.Second:
                    return Gurux.DLMS.Properties.Resources.UnitSecondTxt;
                case Unit.PhaseAngleGegree:
                    return Gurux.DLMS.Properties.Resources.UnitPhasAngleGegreeTxt;
                case Unit.Temperature:
                    return Gurux.DLMS.Properties.Resources.UnitTemperatureTxt;
                case Unit.LocalCurrency:
                    return Gurux.DLMS.Properties.Resources.UnitLocalCurrencyTxt;
                case Unit.Length:
                    return Gurux.DLMS.Properties.Resources.UnitLengthTxt;
                case Unit.Speed:
                    return Gurux.DLMS.Properties.Resources.UnitSpeedTxt;
                case Unit.VolumeCubicMeter:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeCubicMeterTxt;
                case Unit.CorrectedVolume:
                    return Gurux.DLMS.Properties.Resources.UnitCorrectedVolumeTxt;
                case Unit.VolumeFluxHour:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeFluxHourTxt;
                case Unit.CorrectedVolumeFluxHour:
                    return Gurux.DLMS.Properties.Resources.UnitCorrectedVolumeFluxHourTxt;
                case Unit.VolumeFluxDay:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeFluxDayTxt;
                case Unit.CorrecteVolumeFluxDay:
                    return Gurux.DLMS.Properties.Resources.UnitCorrecteVolumeFluxDayTxt;
                case Unit.VolumeLiter:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeLiterTxt;
                case Unit.MassKg:
                    return Gurux.DLMS.Properties.Resources.UnitMassKgTxt;
                case Unit.Force:
                    return Gurux.DLMS.Properties.Resources.UnitForceTxt;
                case Unit.Energy:
                    return Gurux.DLMS.Properties.Resources.UnitEnergyTxt;
                case Unit.PressurePascal:
                    return Gurux.DLMS.Properties.Resources.UnitPressurePascalTxt;
                case Unit.PressureBar:
                    return Gurux.DLMS.Properties.Resources.UnitPressureBarTxt;
                case Unit.EnergyJoule:
                    return Gurux.DLMS.Properties.Resources.UnitEnergyJouleTxt;
                case Unit.ThermalPower:
                    return Gurux.DLMS.Properties.Resources.UnitThermalPowerTxt;
                case Unit.ActivePower:
                    return Gurux.DLMS.Properties.Resources.UnitActivePowerTxt;
                case Unit.ApparentPower:
                    return Gurux.DLMS.Properties.Resources.UnitApparentPowerTxt;
                case Unit.ReactivePower:
                    return Gurux.DLMS.Properties.Resources.UnitReactivePowerTxt;
                case Unit.ActiveEnergy:
                    return Gurux.DLMS.Properties.Resources.UnitActiveEnergyTxt;
                case Unit.ApparentEnergy:
                    return Gurux.DLMS.Properties.Resources.UnitApparentEnergyTxt;
                case Unit.ReactiveEnergy:
                    return Gurux.DLMS.Properties.Resources.UnitReactiveEnergyTxt;
                case Unit.Current:
                    return Gurux.DLMS.Properties.Resources.UnitCurrentTxt;
                case Unit.ElectricalCharge:
                    return Gurux.DLMS.Properties.Resources.UnitElectricalChargeTxt;
                case Unit.Voltage:
                    return Gurux.DLMS.Properties.Resources.UnitVoltageTxt;
                case Unit.ElectricalFieldStrength:
                    return Gurux.DLMS.Properties.Resources.UnitElectricalFieldStrengthTxt;
                case Unit.Capacity:
                    return Gurux.DLMS.Properties.Resources.UnitCapacityTxt;
                case Unit.Resistance:
                    return Gurux.DLMS.Properties.Resources.UnitResistanceTxt;
                case Unit.Resistivity:
                    return Gurux.DLMS.Properties.Resources.UnitResistivityTxt;
                case Unit.MagneticFlux:
                    return Gurux.DLMS.Properties.Resources.UnitMagneticFluxTxt;
                case Unit.Induction:
                    return Gurux.DLMS.Properties.Resources.UnitInductionTxt;
                case Unit.Magnetic:
                    return Gurux.DLMS.Properties.Resources.UnitMagneticTxt;
                case Unit.Inductivity:
                    return Gurux.DLMS.Properties.Resources.UnitInductivityTxt;
                case Unit.Frequency:
                    return Gurux.DLMS.Properties.Resources.UnitFrequencyTxt;
                case Unit.Active:
                    return Gurux.DLMS.Properties.Resources.UnitActiveTxt;
                case Unit.Reactive:
                    return Gurux.DLMS.Properties.Resources.UnitReactiveTxt;
                case Unit.Apparent:
                    return Gurux.DLMS.Properties.Resources.UnitApparentTxt;
                case Unit.V260:
                    return Gurux.DLMS.Properties.Resources.UnitV260Txt;
                case Unit.A260:
                    return Gurux.DLMS.Properties.Resources.UnitA260Txt;
                case Unit.MassKgPerSecond:
                    return Gurux.DLMS.Properties.Resources.UnitMassKgPerSecondTxt;
                case Unit.Conductance:
                    return Gurux.DLMS.Properties.Resources.UnitConductanceTxt;
                case Unit.OtherUnit:
                    return Gurux.DLMS.Properties.Resources.UnitOtherTxt;
                case Unit.NoUnit:
                    return Gurux.DLMS.Properties.Resources.UnitNoneTxt;
            }
            return "";
        }       
    }
}
