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
using System.Security.Cryptography;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMS implements methods to communicate with DLMS/COSEM metering devices.
    /// </summary>
    public class GXDLMSClient
    {
        /// <summary>
        /// DLMS settings.
        /// </summary>
        internal GXDLMSSettings Settings;

        private static Dictionary<ObjectType, Type> AvailableObjectTypes = new Dictionary<ObjectType, Type>();
        /// <summary>
        /// Standard OBIS code
        /// </summary>
        private static GXStandardObisCodeCollection codes = new GXStandardObisCodeCollection();

        /// <summary>
        /// Static Constructor. This is called only once. Get available COSEM objects.
        /// </summary>
        static GXDLMSClient()
        {
            GXDLMS.GetAvailableObjects(AvailableObjectTypes);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSClient()
        {
            Settings = new GXDLMSSettings(false);
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="useLogicalNameReferencing">Is Logical or short name referencing used.</param>
        /// <param name="clientAddress">Client address. Default is 0x10</param>
        /// <param name="ServerAddress">Server ID. Default is 1.</param>
        /// <param name="authentication">Authentication type. Default is None</param>
        /// <param name="password">Password if authentication is used.</param>
        /// <param name="interfaceType">Interface type. Default is general.</param>
        public GXDLMSClient(bool useLogicalNameReferencing,
            int clientAddress, int serverAddress, Authentication authentication,
            string password, InterfaceType interfaceType)
        {
            Settings = new GXDLMSSettings(false);
            Settings.UseLogicalNameReferencing = useLogicalNameReferencing;
            Settings.InterfaceType = interfaceType;
            Settings.Authentication = authentication;
            Settings.ServerAddress = serverAddress;
            Settings.ClientAddress = clientAddress;
            if (password != null)
            {
                Settings.Password = ASCIIEncoding.ASCII.GetBytes(password);
            }
        }

        /// <summary>
        /// List of available custom obis codes.
        /// </summary>
        /// <remarks>
        /// This list is used when Association view is read from the meter and description of the object is needed.
        /// If collection is not set description of object is empty.
        /// </remarks>
        public Gurux.DLMS.ManufacturerSettings.GXObisCodeCollection CustomObisCodes
        {
            get;
            set;
        }

        /// <summary>
        /// Client address.
        /// </summary>
        public int ClientAddress
        {
            get
            {
                return Settings.ClientAddress;
            }
            set
            {
                Settings.ClientAddress = value;
            }
        }

        /// <summary>
        /// Server address.
        /// </summary>
        public int ServerAddress
        {
            get
            {
                return Settings.ServerAddress;
            }
            set
            {
                Settings.ServerAddress = value;
            }
        }

        /// <summary>
        /// Size of server address.
        /// </summary>
        public byte ServerAddressSize
        {
            get
            {
                return Settings.ServerAddressSize;
            }
            set
            {
                Settings.ServerAddressSize = value;
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
                return Settings.DLMSVersion;
            }
        }

        /// <summary>
        /// Retrieves the maximum size of PDU receiver.
        /// </summary>
        /// <remarks>
        /// PDU size tells maximum size of PDU packet.
        /// Value can be from 0 to 0xFFFF. By default the value is 0xFFFF.
        /// </remarks>
        /// <seealso cref="ClientAddress"/>
        /// <seealso cref="ServerAddress"/>
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
        /// The referencing is defined by the device manufacurer.
        /// If the referencing is wrong, the SNMR message will fail.
        /// </remarks>
        [DefaultValue(false)]
        public bool UseLogicalNameReferencing
        {
            get
            {
                return Settings.UseLogicalNameReferencing;
            }
            set
            {
                Settings.UseLogicalNameReferencing = value;
            }
        }

        /// <summary>
        /// Client to Server custom challenge. 
        /// </summary>
        /// <remarks>
        /// This is for debugging purposes. Reset custom challenge settings CtoSChallenge to null.
        /// </remarks>
        public byte[] CtoSChallenge
        {
            get
            {
                return Settings.CtoSChallenge;
            }
            set
            {
                Settings.UseCustomChallenge = value != null;
                Settings.CtoSChallenge = value;
            }
        }

        /// <summary>
        /// Retrieves the password that is used in communication.
        /// </summary>
        /// <remarks>
        /// If authentication is set to none, password is not used.
        /// For HighSHA1, HighMD5 and HighGMAC password is worked as a shared secret.
        /// </remarks>
        /// <seealso cref="Authentication"/>
        public byte[] Password
        {
            get
            {
                return Settings.Password;
            }
            set
            {
                Settings.Password = value;
            }
        }

        /// <summary>
        /// Gets Logical Name settings, read from the device. 
        /// </summary>
        public GXDLMSLNSettings LNSettings
        {
            get
            {
                return Settings.LnSettings;
            }
        }

        /// <summary>
        /// Gets Short Name settings, read from the device.
        /// </summary>
        public GXDLMSSNSettings SNSettings
        {
            get
            {
                return Settings.SnSettings;
            }
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
        /// <seealso cref="ClientAddress"/>
        /// <seealso cref="ServerAddress"/>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="UseLogicalNameReferencing"/>
        /// <seealso cref="MaxReceivePDUSize"/>    
        [DefaultValue(Authentication.None)]
        public Authentication Authentication
        {
            get
            {
                return Settings.Authentication;
            }
            set
            {
                Settings.Authentication = value;
            }
        }

        /// <summary>
        /// Set starting block index in HDLC framing. 
        /// Default is One based, but some meters use Zero based value.
        /// Usually this is not used.
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
                Settings.ResetBlockIndex();
            }
        }

        /// <summary>
        /// Used priority in HDLC framing.
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
        /// Used service class in HDLC framing.
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
            set
            {
                Settings.InterfaceType = value;
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
        /// Generates SNRM request.
        /// </summary>
        /// <remarks>
        /// his method is used to generate send SNRMRequest. 
        /// Before the SNRM request can be generated, at least the following 
        /// properties must be set:
        /// <ul>
        /// <li>ClientAddress</li>
        /// <li>ServerAddress</li>    
        /// </ul>
        /// <b>Note! </b>According to IEC 62056-47: when communicating using 
        /// TCP/IP, the SNRM request is not send.
        /// </remarks>
        /// <returns>SNRM request as byte array.</returns>
        /// <seealso cref="ClientAddress"/>
        /// <seealso cref="ServerAddress"/>
        /// <seealso cref="ParseUAResponse"/>    
        public byte[] SNRMRequest()
        {
            Settings.Connected = false;
            IsAuthenticationRequired = false;
            Settings.MaxReceivePDUSize = 0xFFFF;
            // SNRM request is not used in network connections.
            if (InterfaceType == InterfaceType.WRAPPER)
            {
                return null;
            }
            GXByteBuffer data = new GXByteBuffer(25);
            data.SetUInt8(0x81); // FromatID
            data.SetUInt8(0x80); // GroupID
            data.SetUInt8(0); // Length.

            // If custom HDLC parameters are used.
            if (!GXDLMSLimitsDefault.DefaultMaxInfoTX.Equals(Limits.MaxInfoTX))
            {
                data.SetUInt8((byte)HDLCInfo.MaxInfoTX);
                data.SetUInt8(GXCommon.GetSize(Limits.MaxInfoTX));
                data.Add(Limits.MaxInfoTX);
            }
            if (!GXDLMSLimitsDefault.DefaultMaxInfoRX.Equals(Limits.MaxInfoRX))
            {
                data.SetUInt8((byte)HDLCInfo.MaxInfoRX);
                data.SetUInt8(GXCommon.GetSize(Limits.MaxInfoRX));
                data.Add(Limits.MaxInfoRX);
            }
            if (!GXDLMSLimitsDefault.DefaultWindowSizeTX.Equals(Limits.WindowSizeTX))
            {
                data.SetUInt8((byte)HDLCInfo.WindowSizeTX);
                data.SetUInt8(GXCommon.GetSize(Limits.WindowSizeTX));
                data.Add(Limits.WindowSizeTX);
            }
            if (!GXDLMSLimitsDefault.DefaultWindowSizeRX.Equals(Limits.WindowSizeRX))
            {
                data.SetUInt8((byte)HDLCInfo.WindowSizeRX);
                data.SetUInt8(GXCommon.GetSize(Limits.WindowSizeRX));
                data.Add(Limits.WindowSizeRX);
            }
            // If default HDLC parameters are not used.
            if (data.Size != 3)
            {
                data.SetUInt8(2, (byte)(data.Size - 3)); // Length.
            }
            else
            {
                data = null;
            }
            Settings.ResetFrameSequence();
            return GXDLMS.GetHdlcFrame(Settings, (byte)Command.Snrm, null);
        }

        /// <summary>
        /// Parses UAResponse from byte array.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="data"></param>        
        /// <seealso cref="ParseUAResponse"/>        
        public void ParseUAResponse(GXByteBuffer data)
        {
            //If default settings are used.
            if (data.Size == 0)
            {
                return;
            }
            data.GetUInt8(); // Skip FromatID
            data.GetUInt8(); // Skip Group ID.
            data.GetUInt8(); // Skip Group len
            Object val;
            while (data.Position < data.Size)
            {
                HDLCInfo id = (HDLCInfo)data.GetUInt8();
                short len = data.GetUInt8();
                switch (len)
                {
                    case 1:
                        val = data.GetUInt8();
                        break;
                    case 2:
                        val = data.GetUInt16();
                        break;
                    case 4:
                        val = data.GetUInt32();
                        break;
                    default:
                        throw new GXDLMSException("Invalid Exception.");
                }
                // RX / TX are delivered from the partner's point of view =>
                // reversed to ours
                switch (id)
                {
                    case HDLCInfo.MaxInfoTX:
                        Limits.MaxInfoRX = val;
                        break;
                    case HDLCInfo.MaxInfoRX:
                        Limits.MaxInfoTX = val;
                        break;
                    case HDLCInfo.WindowSizeTX:
                        Limits.WindowSizeRX = val;
                        break;
                    case HDLCInfo.WindowSizeRX:
                        Limits.WindowSizeTX = val;
                        break;
                    default:
                        throw new GXDLMSException("Invalid UA response.");
                }
            }
        }

        /// <summary>
        /// Generate AARQ request. 
        /// </summary>
        /// <returns>AARQ request as byte array.</returns>
        /// <seealso cref="ParseAAREResponse"/>
        public byte[][] AARQRequest()
        {
            Settings.ResetBlockIndex();
            Settings.Connected = false;
            GXByteBuffer buff = new GXByteBuffer(20);
            GXDLMS.CheckInit(Settings);
            Settings.StoCChallenge = null;
            //If High authentication is used.
            if (Authentication > Authentication.Low)
            {
                if (!Settings.UseCustomChallenge)
                { 
                    Settings.CtoSChallenge = GXSecure.GenerateChallenge(Settings.Authentication);
                }
            }
            else
            {
                Settings.CtoSChallenge = null;
            }
            GXAPDU.GenerateAarq(Settings, Settings.Cipher, buff);
            return GXDLMS.GetMessages(Settings, Command.Aarq, 0, buff, DateTime.MinValue);
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
        public void ParseAAREResponse(GXByteBuffer reply)
        {
            Settings.Connected = true;
            IsAuthenticationRequired = GXAPDU.ParsePDU(Settings, Settings.Cipher, reply) == SourceDiagnostic.AuthenticationRequired;
            if (IsAuthenticationRequired)
            {
                System.Diagnostics.Debug.WriteLine("Authentication is required.");
            }
            System.Diagnostics.Debug.WriteLine("- Server max PDU size is " + MaxReceivePDUSize);
            if (DLMSVersion != 6)
            {
                throw new GXDLMSException("Invalid DLMS version number.");
            }
        }

        /// <summary>
        /// Is authentication Required.
        /// </summary>
        /// <seealso cref="GetApplicationAssociationRequest"/>
        /// <seealso cref="ParseApplicationAssociationResponse"/>
        public bool IsAuthenticationRequired
        {
            get;
            private set;
        }

        /// <summary>
        /// Get challenge request if HLS authentication is used.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="IsAuthenticationRequired"/>
        /// <seealso cref="ParseApplicationAssociationResponse"/>
        public byte[][] GetApplicationAssociationRequest()
        {
            if (Settings.Password == null || Settings.Password.Length == 0)
            {
                throw new ArgumentException("Password is invalid.");
            }
            Settings.ResetBlockIndex();
            byte[] pw;
            if (Settings.Authentication == Enums.Authentication.HighGMAC)
            {
                pw = Settings.Cipher.SystemTitle;
            }
            else
            {
                pw = Settings.Password;
            }
            UInt32 ic = 0;
            if (Settings.Cipher != null)
            {
                ic = Settings.Cipher.FrameCounter;
            }
            byte[] challenge = GXSecure.Secure(Settings, Settings.Cipher, ic, Settings.StoCChallenge, pw);
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8((byte)DataType.OctetString);
            GXCommon.SetObjectCount(challenge.Length, bb);
            bb.Set(challenge);
            if (UseLogicalNameReferencing)
            {
                return Method("0.0.40.0.0.255", ObjectType.AssociationLogicalName,
                        1, bb.Array(), DataType.OctetString);
            }
            return Method(0xFA00, ObjectType.AssociationShortName, 8, bb.Array(),
                    DataType.OctetString);
        }

        /// <summary>
        /// Parse server's challenge if HLS authentication is used.
        /// </summary>
        /// <param name="reply"></param>
        /// <seealso cref="IsAuthenticationRequired"/>
        /// <seealso cref="GetApplicationAssociationRequest"/>        
        public void ParseApplicationAssociationResponse(GXByteBuffer reply)
        {
            GXDataInfo info = new GXDataInfo();
            bool equals = false;
            byte[] value = (byte[])GXCommon.GetData(reply, info);
            if (value != null)
            {
                byte[] secret;
                UInt32 ic = 0;
                if (Settings.Authentication == Authentication.HighGMAC)
                {
                    secret = Settings.SourceSystemTitle;
                    GXByteBuffer bb = new GXByteBuffer(value);
                    bb.GetUInt8();
                    ic = bb.GetUInt32();
                }
                else
                {
                    secret = Settings.Password;
                }
                byte[] tmp = GXSecure.Secure(Settings, Settings.Cipher, ic, Settings.CtoSChallenge, secret);
                GXByteBuffer challenge = new GXByteBuffer(tmp);
                equals = challenge.Compare(value);
            }
            if (!equals)
            {
                throw new GXDLMSException("Invalid password. Server to Client challenge do not match.");
            }
        }
     
        /// <summary>
        /// Generates a disconnect request.
        /// </summary>
        /// <returns>Disconnected request, as byte array.</returns>
        public byte[] DisconnectRequest()
        {
            // If connection is not established, there is no need to send
            // DisconnectRequest.
            if (!Settings.Connected)
            {
                return null;
            }
            if (Settings.InterfaceType == InterfaceType.HDLC)
            {
                return GXDLMS.GetHdlcFrame(Settings, (byte)Command.Disc, null);
            }
            GXByteBuffer bb = new GXByteBuffer(2);
            bb.SetUInt8((byte)Command.DisconnectRequest);
            bb.SetUInt8(0x0);
            return GXDLMS.GetWrapperFrame(Settings, bb);
        }

        /// <summary>
        /// Returns object types.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This can be used with serialization.
        /// </remarks>
        public static Type[] GetObjectTypes()
        {
            return GXDLMS.GetObjectTypes(AvailableObjectTypes);
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
        internal static GXDLMSObject CreateDLMSObject(int ClassID, object Version, int BaseName, object LN, object AccessRights)
        {
            ObjectType type = (ObjectType)ClassID;
            GXDLMSObject obj = GXDLMS.CreateObject(type, AvailableObjectTypes);
            UpdateObjectData(obj, type, Version, BaseName, LN, AccessRights);
            return obj;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        GXDLMSObjectCollection ParseSNObjects(GXByteBuffer buff, bool onlyKnownObjects)
        {
            //Get array tag.
            byte size = buff.GetUInt8();
            //Check that data is in the array
            if (size != 0x01)
            {
                throw new GXDLMSException("Invalid response.");
            }
            GXDLMSObjectCollection items = new GXDLMSObjectCollection(this);
            long cnt = GXCommon.GetObjectCount(buff);
            GXDataInfo info = new GXDataInfo();
            for (long objPos = 0; objPos != cnt; ++objPos)
            {
                // Some meters give wrong item count.
                if (buff.Position == buff.Size)
                {
                    break;
                }
                object[] objects = (object[])GXCommon.GetData(buff, info);
                info.Clear();
                if (objects.Length != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                int ot = Convert.ToUInt16(objects[1]);
                int baseName = Convert.ToInt32(objects[0]) & 0xFFFF;
                if (!onlyKnownObjects || AvailableObjectTypes.ContainsKey((ObjectType)ot))
                {
                    if (baseName > 0)
                    {
                        GXDLMSObject comp = CreateDLMSObject(ot, objects[2], baseName, objects[3], null);
                        if (comp != null)
                        {
                            items.Add(comp);
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}", ot, baseName));
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
        internal static void UpdateObjectData(GXDLMSObject obj, ObjectType objectType, object version, object baseName, object logicalName, object accessRights)
        {
            obj.ObjectType = objectType;
            // Check access rights...            
            if (accessRights is object[] && ((object[])accessRights).Length == 2)
            {
                //access_rights: access_right
                object[] access = (object[])accessRights;
                foreach (object[] attributeAccess in (object[])access[0])
                {
                    int id = Convert.ToInt32(attributeAccess[0]);
                    AccessMode mode = (AccessMode)Convert.ToInt32(attributeAccess[1]);
                    //With some meters id is negative. 
                    if (id > 0)
                    {
                        obj.SetAccess(id, mode);
                    }
                }
                if (((object[])access[1]).Length != 0)
                {
                    if (((object[])access[1])[0] is object[])
                    {
                        foreach (object[] methodAccess in (object[])access[1])
                        {
                            int id = Convert.ToInt32(methodAccess[0]);
                            int tmp;
                            //If version is 0.
                            if (methodAccess[1] is Boolean)
                            {
                                tmp = ((Boolean)methodAccess[1]) ? 1 : 0;
                            }
                            else//If version is 1.
                            {
                                tmp = Convert.ToInt32(methodAccess[1]);
                            }
                            obj.SetMethodAccess(id, (MethodAccessMode)tmp);
                        }
                    }
                    else //All versions from Actaris SL 7000 do not return collection as standard says.
                    {
                        object[] arr = (object[])access[1];
                        int id = Convert.ToInt32(arr[0]) + 1;
                        int tmp;
                        //If version is 0.
                        if (arr[1] is Boolean)
                        {
                            tmp = ((Boolean)arr[1]) ? 1 : 0;
                        }
                        else//If version is 1.
                        {
                            tmp = Convert.ToInt32(arr[1]);
                        }
                        obj.SetMethodAccess(id, (MethodAccessMode)tmp);
                    }
                }
            }
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
                obj.LogicalName = GXDLMSObject.ToLogicalName((byte[])logicalName);
            }
            else
            {
                obj.LogicalName = logicalName.ToString();
            }
        }

        /// <summary>
        /// Available objects.
        /// </summary>
        public GXDLMSObjectCollection Objects
        {
            get
            {
                return Settings.Objects;
            }
        }

        /// <summary>
        /// Parses the COSEM objects of the received data.
        /// </summary>
        /// <param name="data">Received data, from the device, as byte array. </param>
        /// <returns>Collection of COSEM objects.</returns>
        public GXDLMSObjectCollection ParseObjects(GXByteBuffer data, bool onlyKnownObjects)
        {
            if (data == null || data.Size == 0)
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
            GXDLMSConverter c = new GXDLMSConverter();
            c.UpdateOBISCodeInformation(objects);
            Settings.Objects = objects;
            return objects;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        GXDLMSObjectCollection ParseLNObjects(GXByteBuffer buff, bool onlyKnownObjects)
        {
            byte size = buff.GetUInt8();
            //Check that data is in the array.
            if (size != 0x01)
            {
                throw new GXDLMSException("Invalid response.");
            }
            //get object count
            int cnt = GXCommon.GetObjectCount(buff);
            int objectCnt = 0;
            GXDLMSObjectCollection items = new GXDLMSObjectCollection(this);
            GXDataInfo info = new GXDataInfo();
            //Some meters give wrong item count.
            while (buff.Position != buff.Size && cnt != objectCnt)
            {                
                info.Clear();
                object[] objects = (object[])GXCommon.GetData(buff, info);
                if (objects.Length != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                ++objectCnt;
                int ot = Convert.ToInt16(objects[0]);
                if (!onlyKnownObjects || AvailableObjectTypes.ContainsKey((ObjectType)ot))
                {
                    GXDLMSObject comp = CreateDLMSObject(ot, objects[1], 0, objects[2], objects[3]);
                    items.Add(comp);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}", ot, GXDLMSObject.ToLogicalName((byte[])objects[2])));
                }
            }
            return items;
        }       

        /// <summary>
        /// Get Value from byte array received from the meter.
        /// </summary>
        public object UpdateValue(GXDLMSObject target, int attributeIndex, object value)
        {
            if (value is byte[])
            {
                DataType type = target.GetUIDataType(attributeIndex);
                if (type != DataType.None)
                {
                    if (type == DataType.DateTime && ((byte[])value).Length == 5)
                    {
                        type = DataType.Date;
                        target.SetUIDataType(attributeIndex, type);
                    }
                    value = ChangeType((byte[])value, type);
                }
            }
            ValueEventArgs e = new ValueEventArgs(Settings, target, attributeIndex, 0, null);
            e.Value = value;
            (target as IGXDLMSBase).SetValue(Settings, e);
            return target.GetValues()[attributeIndex - 1];
        }

        /// <summary>
        /// Update list values.
        /// </summary>
        public void UpdateValues(List<KeyValuePair<GXDLMSObject, int>> list, GXByteBuffer data)
        {
            Object value;
            GXDataInfo info = new GXDataInfo();
            foreach (KeyValuePair<GXDLMSObject, int> it in list)
            {
                int ret = data.GetUInt8();
                if (ret == 0)
                {
                    value = GXCommon.GetData(data, info);
                    ValueEventArgs e = new ValueEventArgs(Settings, it.Key, it.Value, 0, null);
                    e.Value = value;
                    (it.Key as IGXDLMSBase).SetValue(Settings, e);
                    info.Clear();
                }
                else
                {
                    throw new GXDLMSException(ret);
                }
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
            GXByteBuffer tmp = null;
            if (value != null)
            {
                tmp = new GXByteBuffer(value);
            }
            return ChangeType(tmp, type);
        }

        /// <summary>
        /// Changes byte array received from the meter to given type.
        /// </summary>
        /// <param name="value">Byte array received from the meter.</param>
        /// <param name="type">Wanted type.</param>
        /// <returns>Value changed by type.</returns>
        public static object ChangeType(GXByteBuffer value, DataType type)
        {
            if ((value == null || value.Size == 0) && (type == DataType.String || type == DataType.OctetString))
            {
                return string.Empty;
            }
            if (value == null)
            {
                return null;
            }
            if (type == DataType.None)
            {
                return BitConverter.ToString(value.Data, value.Position, value.Size - value.Position).Replace('-', ' ');
            }
            if (value.Size == 0
                    && (type == DataType.String || type == DataType.OctetString))
            {
                return "";
            }
            GXDataInfo info = new GXDataInfo();
            info.Type = type;
            Object ret = GXCommon.GetData(value, info);
            if (!info.Compleate)
            {
                throw new OutOfMemoryException();
            }
            if (type == DataType.OctetString && ret is byte[])
            {
                String str;
                byte[] arr = (byte[])ret;
                if (arr.Length == 0)
                {
                    str = "";
                }
                else
                {
                    StringBuilder bcd = new StringBuilder(arr.Length * 4);
                    foreach (int it in arr)
                    {
                        if (bcd.Length != 0)
                        {
                            bcd.Append(".");
                        }
                        bcd.Append(it.ToString());
                    }
                    str = bcd.ToString();
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
            Settings.ResetBlockIndex();
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
        /// Generate Method (Action) request.
        /// </summary>
        /// <param name="item">object to write.</param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <returns></returns>
        public byte[][] Method(GXDLMSObject item, int index, Object data)
        {
            return Method(item.Name, item.ObjectType, index, data, DataType.None);
        }

        /// <summary>
        /// Generate Method (Action) request.
        /// </summary>
        /// <param name="item">object to write.</param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <param name="data">Additional data.</param>
        /// <param name="type">Additional data type.</param>
        /// <returns></returns>
        public byte[][] Method(GXDLMSObject item, int index, Object data, DataType type)
        {
            return Method(item.Name, item.ObjectType, index, data, type);
        }

        /// <summary>
        /// Generate Method (Action) request.
        /// </summary>
        /// <param name="name">Method object short name or Logical Name.</param>
        /// <param name="objectType">Object type.</param>
        /// <param name="index">Method index.</param>
        /// <returns></returns>
        public byte[][] Method(object name, ObjectType objectType, int index, Object value, DataType type)
        {
            if (name == null || index < 1)
            {
                throw new ArgumentOutOfRangeException("Invalid parameter");
            }
            Settings.ResetBlockIndex();
            if (type == DataType.None && value != null)
            {
                type = GXCommon.GetValueType(value);
                if (type == DataType.None)
                {
                    throw new GXDLMSException(
                            "Invalid parameter. In java value type must give.");
                }
            }
            GXByteBuffer bb = new GXByteBuffer();
            Command cmd;
            if (UseLogicalNameReferencing)
            {
                cmd = Command.MethodRequest;
                // CI
                bb.SetUInt16((UInt16)objectType);
                // Add LN
                String[] items = ((String)name).Split('.');
                if (items.Length != 6)
                {
                    throw new ArgumentException("Invalid Logical Name.");
                }
                foreach (String it2 in items)
                {
                    bb.SetUInt8(byte.Parse(it2));
                }
                // Attribute ID.
                bb.SetUInt8((byte)index);
                // Method Invocation Parameters is not used.
                if (type == DataType.None)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    bb.SetUInt8(1);
                }
            }
            else
            {
                cmd = Command.ReadRequest;
                int data, count;
                GXDLMS.GetActionInfo(objectType, out data, out count);
                if (index > count)
                {
                    throw new ArgumentException("methodIndex");
                }
                else
                {
                    UInt16 sn = Convert.ToUInt16(name);
                    index = (data + (index - 1) * 0x8);
                    sn += (UInt16)index;
                    // Add SN count.
                    bb.SetUInt8(1);
                    // Add VariableName.
                    if (type == DataType.None)
                    {
                        bb.SetUInt8(2);
                    }
                    else //ParameterisedAccess
                    {
                        bb.SetUInt8(4);
                    }
                    // Add name.
                    bb.SetUInt16(sn);
                    // Add selector.
                    if (type != DataType.None)
                    {
                        bb.SetUInt8(1);
                    }
                }
            }
            if ((value is byte[]))
            {
                bb.Set((byte[])value);
            }
            else if (type != DataType.None)
            {
                GXCommon.SetData(bb, type, value);
            }
            return GXDLMS.GetMessages(Settings, cmd, 1, bb, DateTime.MinValue);
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
            Object value = (item as IGXDLMSBase).GetValue(Settings, new ValueEventArgs(Settings, item, index, 0, null));
            DataType type = item.GetDataType(index);
            if (type == DataType.None)
            {
                type = Gurux.DLMS.Internal.GXCommon.GetValueType(value);
            }
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
            Settings.ResetBlockIndex();
            if (type == DataType.None && value != null)
            {
                type = GXCommon.GetValueType(value);
                if (type == DataType.None)
                {
                    throw new ArgumentException("Invalid parameter. Unknown value type.");
                }
            }
            GXByteBuffer bb = new GXByteBuffer();
            Command cmd;
            if (UseLogicalNameReferencing)
            {
                cmd = Command.SetRequest;
                // Add CI.
                bb.SetUInt16((UInt16)objectType);
                // Add LN.
                String[] items = ((String)name).Split('.');
                if (items.Length != 6)
                {
                    throw new ArgumentException("Invalid Logical Name.");
                }
                foreach (String it2 in items)
                {
                    bb.SetUInt8(byte.Parse(it2));
                }
                // Attribute ID.
                bb.SetUInt8((byte)index);
                // Access selection is not used.
                bb.SetUInt8(0);
            }
            else
            {
                cmd = Command.WriteRequest;
                // Add SN count.
                bb.SetUInt8(1);
                // Add name length.
                bb.SetUInt8(2);
                // Add name.
                UInt16 sn = Convert.ToUInt16(name);
                sn += (UInt16)((index - 1) * 8);
                bb.SetUInt16(sn);
                // Add data count.
                bb.SetUInt8(1);
            }
            GXCommon.SetData(bb, type, value);
            return GXDLMS.GetMessages(Settings, cmd, 1, bb, DateTime.MinValue);
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
            return Read(name, objectType, attributeOrdinal, null);
        }

        private byte[][] Read(object name, ObjectType objectType, int attributeOrdinal, GXByteBuffer data)
        {
            if ((attributeOrdinal < 0))
            {
                throw new ArgumentException("Invalid parameter");
            }
            Settings.ResetBlockIndex();
            Command cmd;
            GXByteBuffer bb = new GXByteBuffer();
            if (UseLogicalNameReferencing)
            {
                cmd = Command.GetRequest;
                // CI
                bb.SetUInt16((UInt16)objectType);
                // Add LN
                String[] items = ((String)name).Split('.');
                if (items.Length != 6)
                {
                    throw new ArgumentException("Invalid Logical Name.");
                }
                foreach (String it2 in items)
                {
                    bb.SetUInt8(byte.Parse(it2));
                }
                // Attribute ID.
                bb.SetUInt8((byte)attributeOrdinal);
                if (data == null || data.Size == 0)
                {
                    // Access selection is not used.
                    bb.SetUInt8(0);
                }
                else
                {
                    // Access selection is used.
                    bb.SetUInt8(1);
                    // Add data.
                    bb.Set(data.Data, 0, data.Size);
                }
            }
            else
            {
                cmd = Command.ReadRequest;
                // Add length.
                bb.SetUInt8(1);
                // parameterized-access
                if (data != null && data.Size != 0)
                {
                    bb.SetUInt8(4);
                }
                else //variable-name
                {
                    bb.SetUInt8(2);
                }
                UInt16 sn = Convert.ToUInt16(name);
                sn += (UInt16)((attributeOrdinal - 1) * 8);
                bb.SetUInt16(sn);
                // Add data.
                if (data != null && data.Size != 0)
                {
                    bb.Set(data.Data, 0, data.Size);
                }
            }
            return GXDLMS.GetMessages(Settings, cmd, 1, bb, DateTime.MinValue);
        }

        /// <summary>
        /// Generates a read message.
        /// </summary>
        /// <param name="item">DLMS object to read.</param>
        /// <param name="attributeOrdinal">Read attribute index.</param>
        /// <returns>Read request as byte array.</returns>
        public byte[][] Read(GXDLMSObject item, int attributeOrdinal)
        {
            return Read(item.Name, item.ObjectType, attributeOrdinal, null);
        }

        /// <summary>
        /// Read list of COSEM objects.
        /// </summary>
        /// <param name="item">DLMS object to read.</param>
        /// <param name="attributeOrdinal">Read attribute index.</param>
        /// <returns>Read request as byte array.</returns>
        public byte[][] ReadList(List<KeyValuePair<GXDLMSObject, int>> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentOutOfRangeException("Invalid parameter.");
            }
            Settings.ResetBlockIndex();
            List<byte[]> messages = new List<byte[]>();
            Command cmd;
            GXByteBuffer bb = new GXByteBuffer();
            if (this.UseLogicalNameReferencing)
            {
                cmd = Command.GetRequest;
                //Request service primitive shall always fit in a single APDU.
                int pos = 0, count = (Settings.MaxReceivePDUSize - 12) / 10;
                if (list.Count < count)
                {
                    count = list.Count;
                }
                //All meters can handle 10 items.
                if (count > 10)
                {
                    count = 10;
                }
                // Add length.
                GXCommon.SetObjectCount(count, bb);
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {                    
                    // CI.
                    bb.SetUInt16((UInt16)it.Key.ObjectType);
                    String[] items = it.Key.LogicalName.Split('.');
                    if (items.Length != 6)
                    {
                        throw new ArgumentException("Invalid Logical Name.");
                    }
                    foreach (String it2 in items)
                    {
                        bb.SetUInt8(byte.Parse(it2));
                    }
                    // Attribute ID.
                    bb.SetUInt8((byte)it.Value);
                    // Attribute selector is not used.
                    bb.SetUInt8(0);
                    ++pos;
                    if (pos % count == 0 && list.Count != pos)
                    {
                        messages.AddRange(GXDLMS.GetMessages(Settings, cmd, 3, bb, DateTime.MinValue));
                        bb.Clear();
                        if (list.Count - pos < count)
                        {
                            GXCommon.SetObjectCount(list.Count - pos, bb);
                        }
                        else
                        {
                            GXCommon.SetObjectCount(count, bb);
                        }
                    }
                }
            }
            else
            {
                cmd = Command.ReadRequest;
                // Add length.
                GXCommon.SetObjectCount(list.Count, bb);
                foreach (KeyValuePair<GXDLMSObject, int> it in list)
                {
                    // Add variable type.
                    bb.SetUInt8(2);
                    int sn = it.Key.ShortName;
                    sn += (it.Value - 1) * 8;
                    bb.SetUInt16((UInt16)sn);
                }
            }
            messages.AddRange(GXDLMS.GetMessages(Settings, cmd, 3, bb, DateTime.MinValue));
            return messages.ToArray();
        }

        /// <summary>
        /// Generates the keep alive message. 
        /// </summary>
        /// <remarks>
        /// Keepalive message is needed only HDLC framing.
        /// For keepalive we are reading logical name for fist object.
        /// </remarks>
        /// <returns>Returns Keep alive message, as byte array.</returns>
        public byte[] GetKeepAlive()
        {
            // There is no need for keep alive in IEC 62056-47.
            if (this.InterfaceType == InterfaceType.WRAPPER)
            {
                return new byte[0];
            }
            if (this.Objects.Count != 0)
            {
                return Read(this.Objects[0], 1)[0];
            }
            return new byte[0];
        }

        /// <summary>
        /// Read rows by entry.
        /// </summary>
        /// <param name="pg">Profile generic object to read.</param>
        /// <param name="index">Zero bases start index.</param>
        /// <param name="count">Rows count to read.</param>
        /// <returns>Read message as byte array.</returns>
        public byte[][] ReadRowsByEntry(GXDLMSProfileGeneric pg, int index, int count)
        {
            Settings.ResetBlockIndex();
            GXByteBuffer buff = new GXByteBuffer(19);
            // Add AccessSelector value
            buff.SetUInt8(0x02);
            // Add enum tag.
            buff.SetUInt8((byte)DataType.Structure);
            // Add item count
            buff.SetUInt8(0x04);
            // Add start index
            GXCommon.SetData(buff, DataType.UInt32, index);
            // Add Count
            GXCommon.SetData(buff, DataType.UInt32, count);
            // Read all columns.
            if (Settings.UseLogicalNameReferencing)
            {
                GXCommon.SetData(buff, DataType.UInt16, 1);
            }
            else
            {
                GXCommon.SetData(buff, DataType.UInt16, 0);
            }
            GXCommon.SetData(buff, DataType.UInt16, 0);
            return Read(pg.Name, ObjectType.ProfileGeneric, 2, buff);
        }

        /// <summary>
        /// Read rows by range.
        /// </summary>
        /// <remarks>
        /// Use this method to read Profile Generic table between dates.
        /// </remarks>
        /// <param name="pg">Profile generic object to read.</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        /// <returns></returns>
        public byte[][] ReadRowsByRange(GXDLMSProfileGeneric pg, DateTime start, DateTime end)
        {
            Settings.ResetBlockIndex();
            GXDLMSObject sort = pg.SortObject;
            if (sort == null && pg.CaptureObjects.Count != 0)
            {
                sort = pg.CaptureObjects[0].Key;                
            }
            //If sort object is not found or it is not clock object read all.
            if (sort == null || sort.ObjectType != ObjectType.Clock)
            {
                return Read(pg, 2);
            }
            GXByteBuffer buff = new GXByteBuffer(51);
            // Add AccessSelector value.
            buff.SetUInt8(0x01);
            // Add enum tag.
            buff.SetUInt8((byte)DataType.Structure);
            // Add item count
            buff.SetUInt8(0x04);
            // Add enum tag.
            buff.SetUInt8((byte)DataType.Structure);
            // Add item count
            buff.SetUInt8(0x04);
            // CI
            GXCommon.SetData(buff, DataType.UInt16,
                    sort.ObjectType);
            // LN
            GXCommon.SetData(buff, DataType.OctetString, sort.LogicalName);
            // Add attribute index.
            GXCommon.SetData(buff, DataType.Int8, 2);
            // Add version.
            GXCommon.SetData(buff, DataType.UInt16, sort.Version);
            // Add start time.
            GXCommon.SetData(buff, DataType.OctetString, start);
            // Add end time.
            GXCommon.SetData(buff, DataType.OctetString, end);
            // Add array of read columns. Read All...
            buff.SetUInt8(0x01);
            // Add item count
            buff.SetUInt8(0x00);
            return Read(pg.Name, ObjectType.ProfileGeneric, 2, buff);
        }

        /// <summary>
        /// Create given type of COSEM object.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static GXDLMSObject CreateObject(ObjectType type)
        {
            return GXDLMS.CreateObject(type, AvailableObjectTypes);
        }

        /// <summary>
        /// Generates an acknowledgment message, with which the server is informed to 
        /// send next packets.
        /// </summary>
        /// <param name="type">Frame type</param>
        /// <returns>Acknowledgment message as byte array.</returns>
        public byte[] ReceiverReady(RequestTypes type)
        {
            return GXDLMS.ReceiverReady(Settings, type);
        }

        ///<summary>
        /// Removes the HDLC frame from the packet, and returns COSEM data only.
        ///</summary>
        ///<param name="reply">
        /// The received data from the device. 
        ///</param>
        ///<param name="data">
        /// Information from the received data. 
        ///</param>
        ///<returns>
        /// Is frame complete.
        ///</returns>
        public bool GetData(byte[] reply, GXReplyData data)
        {
            return GXDLMS.GetData(Settings, new GXByteBuffer(reply), data);
        }

        /// <summary>
        /// Get value from DLMS byte stream.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <returns>Parsed value.</returns>
        public static object GetValue(GXByteBuffer data)
        {
            return GetValue(data, DataType.None);
        }

        /// <summary>
        /// Get value from DLMS byte stream.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <param name="type">Conversion type is used if returned data is byte array.</param>
        /// <returns>Parsed value.</returns>
        public static object GetValue(GXByteBuffer data, DataType type)
        {
            GXDataInfo info = new GXDataInfo();
            object value = GXCommon.GetData(data, info);
            if (value is byte[] && type != DataType.None)
            {
                value = GXDLMSClient.ChangeType((byte[]) value, type);
            }
            return value;
        }

        ///<summary>
        ///Removes the HDLC frame from the packet, and returns COSEM data only.
        ///</summary>
        ///<param name="reply">
        ///The received data from the device. 
        ///</param>
        ///<param name="data">
        ///The exported reply information. 
        ///</param>
        ///<returns>
        /// Is frame complete.
        ///</returns>
        public virtual bool GetData(GXByteBuffer reply, GXReplyData data)
        {
            return GXDLMS.GetData(Settings, reply, data);
        }

        /// <summary>
        /// Convert physical address and logical address to server address.
        /// </summary>
        /// <param name="logicalAddress">Server logical address.</param>
        /// <param name="physicalAddress">Server physical address.</param>
        /// <returns>Server address.</returns>
        public static int GetServerAddress(int logicalAddress, int physicalAddress)
        {
            return GetServerAddress(logicalAddress, physicalAddress, 0);
        }

        /// <summary>
        /// Convert physical address and logical address to server address.
        /// </summary>
        /// <param name="logicalAddress">Server logical address.</param>
        /// <param name="physicalAddress">Server physical address.</param>
        /// <param name="addressSize">Address size in bytes.</param>
        /// <returns>Server address.</returns>
        public static int GetServerAddress(int logicalAddress, int physicalAddress, int addressSize)
        {
            int value;
            if (addressSize < 4 && physicalAddress < 0x80 && logicalAddress < 0x80)
            {
                value = logicalAddress << 7 | physicalAddress;
            }
            else if (physicalAddress < 0x4000 && logicalAddress < 0x4000)
            {
                value = logicalAddress << 14 | physicalAddress;
            }
            else
            {
                throw new ArgumentException("Invalid logical or physical address.");
            }
            return value;
        }


        private static void GetItem(GXSerialNumberItem value)
        {

        }

        class GXSerialNumberItem
        {
            public int Index
            {
                get;
                set;
            }
            public string Formula
            {
                get;
                set;
            }
            public string Tag
            {
                get;
                set;
            }
            public string Value
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Converts meter serial number to server address.
        /// Default formula is used.
        /// </summary>
        /// <remarks>
        /// All meters do not use standard formula or support serial number addressing at all.
        /// </remarks>
        /// <param name="serialNumber">Meter serial number.</param>
        /// <returns>Server address.</returns>
        public static int GetServerAddress(int serialNumber)
        {
            return GetServerAddress(serialNumber, null);
        }
        /// <summary>
        /// Converts meter serial number to server address.
        /// </summary>
        /// <param name="serialNumber">Meter serial number.</param>
        /// <param name="formula">Formula used to convert serial number to server address.</param>
        /// <returns>Server address.</returns>
        /// <remarks>
        /// All meters do not use standard formula or support serial number addressing at all.
        /// </remarks>
        public static int GetServerAddress(int serialNumber, string formula)
        {
            //If formula is not given use default formula.
            //This formula is defined in DLMS specification.
            if (String.IsNullOrEmpty(formula))
            {
                formula = "SN % 10000 + 1000";
            }
            return 0x4000 | SerialnumberCounter.Count(serialNumber, formula);
        }       
    }
}