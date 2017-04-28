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
using Gurux.DLMS.ManufacturerSettings;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using System.Xml;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSAssociationLogicalName : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAssociationLogicalName()
        : this("0.0.40.0.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSAssociationLogicalName(string ln)
        : base(ObjectType.AssociationLogicalName, ln, 0)
        {
            Version = 3;
            ObjectList = new GXDLMSObjectCollection();
            ApplicationContextName = new GXApplicationContextName();
            XDLMSContextInfo = new GXxDLMSContextType();
            AuthenticationMechanismName = new GXAuthenticationMechanismName();
        }

        [XmlIgnore()]
        public GXDLMSObjectCollection ObjectList
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the identifiers of the COSEM client APs within the physical devices hosting these APs,
        /// which belong to the AA modelled by the Association LN object.
        /// </summary>
        [XmlIgnore()]
        public byte ClientSAP
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the identifiers of the COSEM server (logical device) APs within the physical
        /// devices hosting these APs, which belong to the AA modelled by the Association LN object.
        /// </summary>
        [XmlIgnore()]
        public UInt16 ServerSAP
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXApplicationContextName ApplicationContextName
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXxDLMSContextType XDLMSContextInfo
        {
            get;
            set;
        }


        [XmlIgnore()]
        public GXAuthenticationMechanismName AuthenticationMechanismName
        {
            get;
            set;
        }

        /// <summary>
        /// Low Level Security secret.
        /// </summary>
        [XmlIgnore()]
        public byte[] Secret
        {
            get;
            set;
        }

        /// <summary>
        /// High Level Security secret.
        /// </summary>
        [XmlIgnore()]
        public byte[] HlsSecret
        {
            get;
            set;
        }

        [XmlIgnore()]
        public AssociationStatus AssociationStatus
        {
            get;
            set;
        }

        [XmlIgnore()]
        public string SecuritySetupReference
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ObjectList, ClientSAP + "/" + ServerSAP, ApplicationContextName,
                              XDLMSContextInfo, AuthenticationMechanismName, Secret, AssociationStatus, SecuritySetupReference
                            };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            //Check reply_to_HLS_authentication
            if (e.Index == 1)
            {
                UInt32 ic = 0;
                byte[] secret;
                if (settings.Authentication == Authentication.HighGMAC)
                {
                    secret = settings.SourceSystemTitle;
                    GXByteBuffer bb = new GXByteBuffer(e.Parameters as byte[]);
                    bb.GetUInt8();
                    ic = bb.GetUInt32();
                }
                else
                {
                    secret = HlsSecret;
                }
                byte[] serverChallenge = GXSecure.Secure(settings, settings.Cipher, ic, settings.StoCChallenge, secret);
                byte[] clientChallenge = (byte[])e.Parameters;
                if (serverChallenge != null && clientChallenge != null && GXCommon.Compare(serverChallenge, clientChallenge))
                {
                    if (settings.Authentication == Authentication.HighGMAC)
                    {
                        secret = settings.Cipher.SystemTitle;
                        ic = settings.Cipher.InvocationCounter;
                    }
                    else
                    {
                        secret = HlsSecret;
                    }
                    settings.Connected = true;
                    return GXSecure.Secure(settings, settings.Cipher, ic, settings.CtoSChallenge, secret);
                }
                else //If the password does not match.
                {
                    settings.Connected = false;
                    return null;
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
                return null;
            }
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }

            //ObjectList is static and read only once.
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }

            //associated_partners_id is static and read only once.
            if (!base.IsRead(3))
            {
                attributes.Add(3);
            }
            //Application Context Name is static and read only once.
            if (!base.IsRead(4))
            {
                attributes.Add(4);
            }

            //xDLMS Context Info
            if (!base.IsRead(5))
            {
                attributes.Add(5);
            }

            // Authentication Mechanism Name
            if (!base.IsRead(6))
            {
                attributes.Add(6);
            }

            // LLS Secret
            if (!base.IsRead(7))
            {
                attributes.Add(7);
            }
            // Association Status
            if (!base.IsRead(8))
            {
                attributes.Add(8);
            }
            //Security Setup Reference is from version 1.
            if (Version > 0 && !base.IsRead(9))
            {
                attributes.Add(9);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            if (Version == 0)
            {
                return new string[] {Internal.GXCommon.GetLogicalNameString(),
                                 "Object List",
                                 "Aassociated_partners_id",
                                 "Application Context Name",
                                 "xDLMS Context Info",
                                 "Authentication Mechanism Name",
                                 "Secret",
                                 "Association Status"
                                };
            }
            return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Object List",
                             "Aassociated_partners_id",
                             "Application Context Name",
                             "xDLMS Context Info",
                             "Authentication Mechanism Name",
                             "Secret",
                             "Association Status",
                             "Security Setup Reference"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            //Security Setup Reference is from version 1.
            if (Version > 0)
                return 9;
            return 8;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 4;
        }

        /// <summary>
        /// Returns Association View.
        /// </summary>
        private GXByteBuffer GetObjects(GXDLMSSettings settings, ValueEventArgs e)
        {
            GXByteBuffer data = new GXByteBuffer();
            //Add count only for first time.
            if (settings.Index == 0)
            {
                settings.Count = (UInt16)ObjectList.Count;
                data.SetUInt8((byte)DataType.Array);
                GXCommon.SetObjectCount(ObjectList.Count, data);
            }
            ushort pos = 0;
            foreach (GXDLMSObject it in ObjectList)
            {
                ++pos;
                if (!(pos <= settings.Index))
                {
                    data.SetUInt8((byte)DataType.Structure);
                    data.SetUInt8((byte)4); //Count
                    GXCommon.SetData(settings, data, DataType.UInt16, it.ObjectType); //ClassID
                    GXCommon.SetData(settings, data, DataType.UInt8, it.Version); //Version
                    //LN
                    GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(it.LogicalName));
                    GetAccessRights(settings, it, e.Server, data); //Access rights.
                    ++settings.Index;
                    //If PDU is full.
                    if (!e.SkipMaxPduSize && data.Size >= settings.MaxPduSize)
                    {
                        break;
                    }
                }
            }
            return data;
        }

        private void GetAccessRights(GXDLMSSettings settings, GXDLMSObject item, GXDLMSServer server, GXByteBuffer data)
        {
            data.SetUInt8((byte)DataType.Structure);
            data.SetUInt8((byte)2);
            data.SetUInt8((byte)DataType.Array);
            int cnt = (item as IGXDLMSBase).GetAttributeCount();
            data.SetUInt8((byte)cnt);
            ValueEventArgs e;
            if (server != null)
            {
                e = new DLMS.ValueEventArgs(server, item, 0, 0, null);
            }
            else
            {
                e = new DLMS.ValueEventArgs(settings, item, 0, 0, null);
            }
            for (int pos = 0; pos != cnt; ++pos)
            {
                e.Index = pos + 1;
                AccessMode m;
                if (server != null)
                {
                    m = server.NotifyGetAttributeAccess(e);
                }
                else
                {
                    m = AccessMode.ReadWrite;
                }
                //attribute_access_item
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8((byte)3);
                GXCommon.SetData(settings, data, DataType.Int8, e.Index);
                GXCommon.SetData(settings, data, DataType.Enum, m);
                GXCommon.SetData(settings, data, DataType.None, null);
            }
            data.SetUInt8((byte)DataType.Array);
            cnt = (item as IGXDLMSBase).GetMethodCount();
            data.SetUInt8((byte)cnt);
            for (int pos = 0; pos != cnt; ++pos)
            {
                e.Index = pos + 1;
                MethodAccessMode m;
                if (server != null)
                {
                    m = server.NotifyGetMethodAccess(e);
                }
                else
                {
                    m = MethodAccessMode.Access;
                }
                //attribute_access_item
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8((byte)2);
                GXCommon.SetData(settings, data, DataType.Int8, pos + 1);
                GXCommon.SetData(settings, data, DataType.Enum, m);
            }
        }

        void UpdateAccessRights(GXDLMSObject obj, Object[] buff)
        {
            if (buff.Length != 0)
            {
                foreach (Object[] attributeAccess in (Object[])buff[0])
                {
                    int id = Convert.ToInt32(attributeAccess[0]);
                    int mode = Convert.ToInt32(attributeAccess[1]);
                    obj.SetAccess(id, (AccessMode)mode);
                }
                foreach (Object[] methodAccess in (Object[])buff[1])
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
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.Array;
            }
            if (index == 3)
            {
                return DataType.Structure;
            }
            if (index == 4)
            {
                return DataType.Structure;
            }
            if (index == 5)
            {
                return DataType.Structure;
            }
            if (index == 6)
            {
                return DataType.Structure;
            }
            if (index == 7)
            {
                return DataType.OctetString;
            }
            if (index == 8)
            {
                return DataType.Enum;
            }
            if (index == 9)
            {
                return DataType.OctetString;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return GetObjects(settings, e).Array();
            }
            if (e.Index == 3)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                //Add count
                data.SetUInt8(2);
                data.SetUInt8((byte)DataType.UInt8);
                data.SetUInt8(ClientSAP);
                data.SetUInt8((byte)DataType.UInt16);
                data.SetUInt16(ServerSAP);
                return data.Array();
            }
            if (e.Index == 4)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                //Add count
                data.SetUInt8(0x7);
                GXCommon.SetData(settings, data, DataType.UInt8, ApplicationContextName.JointIsoCtt);
                GXCommon.SetData(settings, data, DataType.UInt8, ApplicationContextName.Country);
                GXCommon.SetData(settings, data, DataType.UInt16, ApplicationContextName.CountryName);
                GXCommon.SetData(settings, data, DataType.UInt8, ApplicationContextName.IdentifiedOrganization);
                GXCommon.SetData(settings, data, DataType.UInt8, ApplicationContextName.DlmsUA);
                GXCommon.SetData(settings, data, DataType.UInt8, ApplicationContextName.ApplicationContext);
                GXCommon.SetData(settings, data, DataType.UInt8, ApplicationContextName.ContextId);
                return data.Array();
            }
            if (e.Index == 5)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(6);
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt32((UInt32)XDLMSContextInfo.Conformance);
                GXCommon.SetData(settings, data, DataType.BitString, bb.SubArray(1, 3));
                GXCommon.SetData(settings, data, DataType.UInt16, XDLMSContextInfo.MaxReceivePduSize);
                GXCommon.SetData(settings, data, DataType.UInt16, XDLMSContextInfo.MaxSendPduSize);
                GXCommon.SetData(settings, data, DataType.UInt8, XDLMSContextInfo.DlmsVersionNumber);
                GXCommon.SetData(settings, data, DataType.Int8, XDLMSContextInfo.QualityOfService);
                GXCommon.SetData(settings, data, DataType.OctetString, XDLMSContextInfo.CypheringInfo);
                return data.Array();
            }
            if (e.Index == 6)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                //Add count
                data.SetUInt8(0x7);
                GXCommon.SetData(settings, data, DataType.UInt8, AuthenticationMechanismName.JointIsoCtt);
                GXCommon.SetData(settings, data, DataType.UInt8, AuthenticationMechanismName.Country);
                GXCommon.SetData(settings, data, DataType.UInt16, AuthenticationMechanismName.CountryName);
                GXCommon.SetData(settings, data, DataType.UInt8, AuthenticationMechanismName.IdentifiedOrganization);
                GXCommon.SetData(settings, data, DataType.UInt8, AuthenticationMechanismName.DlmsUA);
                GXCommon.SetData(settings, data, DataType.UInt8, AuthenticationMechanismName.AuthenticationMechanismName);
                GXCommon.SetData(settings, data, DataType.UInt8, AuthenticationMechanismName.MechanismId);
                return data.Array();
            }
            if (e.Index == 7)
            {
                return Secret;
            }
            if (e.Index == 8)
            {
                return AssociationStatus;
            }
            if (e.Index == 9)
            {
                return GXCommon.LogicalNameToBytes(SecuritySetupReference);
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                LogicalName = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 2)
            {
                ObjectList.Clear();
                if (e.Value != null)
                {
                    foreach (Object[] item in (Object[])e.Value)
                    {
                        ObjectType type = (ObjectType)Convert.ToInt32(item[0]);
                        int version = Convert.ToInt32(item[1]);
                        String ln = GXCommon.ToLogicalName((byte[])item[2]);
                        GXDLMSObject obj = null;
                        if (settings.Objects != null)
                        {
                            obj = settings.Objects.FindByLN(type, ln);
                        }
                        if (obj == null)
                        {
                            obj = Gurux.DLMS.GXDLMSClient.CreateObject(type);
                            obj.LogicalName = ln;
                            obj.Version = version;
                        }
                        //Unknown objects are not shown.
                        if (obj is IGXDLMSBase && item[3] != null)
                        {
                            UpdateAccessRights(obj, (Object[])item[3]);
                            ObjectList.Add(obj);
                        }
                    }
                }
            }
            else if (e.Index == 3)
            {
                if (e.Value != null)
                {
                    ClientSAP = Convert.ToByte(((Object[])e.Value)[0]);
                    ServerSAP = Convert.ToUInt16(((Object[])e.Value)[1]);
                }
            }
            else if (e.Index == 4)
            {
                //Value of the object identifier encoded in BER
                if (e.Value is byte[])
                {
                    GXByteBuffer arr = new GXByteBuffer(e.Value as byte[]);
                    if (arr.GetUInt8(0) == 0x60)
                    {

                        ApplicationContextName.JointIsoCtt = 0;
                        ++arr.Position;
                        ApplicationContextName.Country = 0;
                        ++arr.Position;
                        ApplicationContextName.CountryName = 0;
                        ++arr.Position;
                        ApplicationContextName.IdentifiedOrganization = arr.GetUInt8();
                        ApplicationContextName.DlmsUA = arr.GetUInt8();
                        ApplicationContextName.ApplicationContext = arr.GetUInt8();
                        ApplicationContextName.ContextId = arr.GetUInt8();
                    }
                    else
                    {
                        //Get Tag and Len.
                        if (arr.GetUInt8() != (int)BerType.Integer && arr.GetUInt8() != 7)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.JointIsoCtt = arr.GetUInt8();
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.Country = arr.GetUInt8();
                        //Get tag
                        if (arr.GetUInt8() != 0x12)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.CountryName = arr.GetUInt16();
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.IdentifiedOrganization = arr.GetUInt8();
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.DlmsUA = arr.GetUInt8();
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.ApplicationContext = arr.GetUInt8();
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.ContextId = arr.GetUInt8();
                    }
                }
                else if (e.Value != null)
                {
                    Object[] arr = (Object[])e.Value;
                    ApplicationContextName.JointIsoCtt = Convert.ToByte(arr[0]);
                    ApplicationContextName.Country = Convert.ToByte(arr[1]);
                    ApplicationContextName.CountryName = Convert.ToUInt16(arr[2]);
                    ApplicationContextName.IdentifiedOrganization = Convert.ToByte(arr[3]);
                    ApplicationContextName.DlmsUA = Convert.ToByte(arr[4]);
                    ApplicationContextName.ApplicationContext = Convert.ToByte(arr[5]);
                    ApplicationContextName.ContextId = Convert.ToByte(arr[6]);
                }
            }
            else if (e.Index == 5)
            {
                if (e.Value != null)
                {
                    Object[] arr = (Object[])e.Value;
                    GXByteBuffer bb = new GXByteBuffer();
                    GXCommon.SetBitString(bb, arr[0]);
                    bb.SetUInt8(0, 0);
                    XDLMSContextInfo.Conformance = (Conformance)bb.GetUInt32();
                    XDLMSContextInfo.MaxReceivePduSize = Convert.ToUInt16(arr[1]);
                    XDLMSContextInfo.MaxSendPduSize = Convert.ToUInt16(arr[2]);
                    XDLMSContextInfo.DlmsVersionNumber = Convert.ToByte(arr[3]);
                    XDLMSContextInfo.QualityOfService = Convert.ToSByte(arr[4]);
                    XDLMSContextInfo.CypheringInfo = (byte[])arr[5];
                }
            }
            else if (e.Index == 6)
            {
                //Value of the object identifier encoded in BER
                if (e.Value is byte[])
                {
                    GXByteBuffer arr = new GXByteBuffer(e.Value as byte[]);
                    if (arr.GetUInt8(0) == 0x60)
                    {
                        AuthenticationMechanismName.JointIsoCtt = 0;
                        ++arr.Position;
                        AuthenticationMechanismName.Country = 0;
                        ++arr.Position;
                        AuthenticationMechanismName.CountryName = 0;
                        ++arr.Position;
                        AuthenticationMechanismName.IdentifiedOrganization = arr.GetUInt8();
                        AuthenticationMechanismName.DlmsUA = arr.GetUInt8();
                        AuthenticationMechanismName.AuthenticationMechanismName = arr.GetUInt8();
                        AuthenticationMechanismName.MechanismId = (Authentication)arr.GetUInt8();
                    }
                    else
                    {
                        //Get Tag and Len.
                        if (arr.GetUInt8() != (int)BerType.Integer && arr.GetUInt8() != 7)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismName.JointIsoCtt = arr.GetUInt8();
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismName.Country = arr.GetUInt8();
                        //Get tag
                        if (arr.GetUInt8() != 0x12)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismName.CountryName = arr.GetUInt16();
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismName.IdentifiedOrganization = arr.GetUInt8();
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismName.DlmsUA = arr.GetUInt8();
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismName.AuthenticationMechanismName = arr.GetUInt8();
                        //Get tag
                        if (arr.GetUInt8() != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismName.MechanismId = (Authentication)arr.GetUInt8();
                    }
                }
                else if (e.Value != null)
                {
                    Object[] arr = (Object[])e.Value;
                    AuthenticationMechanismName.JointIsoCtt = Convert.ToByte(arr[0]);
                    AuthenticationMechanismName.Country = Convert.ToByte(arr[1]);
                    AuthenticationMechanismName.CountryName = Convert.ToUInt16(arr[2]);
                    AuthenticationMechanismName.IdentifiedOrganization = Convert.ToByte(arr[3]);
                    AuthenticationMechanismName.DlmsUA = Convert.ToByte(arr[4]);
                    AuthenticationMechanismName.AuthenticationMechanismName = Convert.ToByte(arr[5]);
                    AuthenticationMechanismName.MechanismId = (Authentication)Convert.ToByte(arr[6]);
                }
            }
            else if (e.Index == 7)
            {
                Secret = (byte[])e.Value;
            }
            else if (e.Index == 8)
            {
                if (e.Value == null)
                {
                    AssociationStatus = AssociationStatus.NonAssociated;
                }
                else
                {
                    AssociationStatus = (AssociationStatus)Convert.ToInt32(e.Value);
                }
            }
            else if (e.Index == 9)
            {
                SecuritySetupReference = GXCommon.ToLogicalName(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            ClientSAP = (byte)reader.ReadElementContentAsInt("ClientSAP");
            ServerSAP = (byte)reader.ReadElementContentAsInt("ServerSAP");
            if (reader.IsStartElement("ApplicationContextName", true))
            {
                ApplicationContextName.JointIsoCtt = (byte)reader.ReadElementContentAsInt("JointIsoCtt");
                ApplicationContextName.Country = (byte)reader.ReadElementContentAsInt("Country");
                ApplicationContextName.CountryName = (UInt16)reader.ReadElementContentAsInt("CountryName");
                ApplicationContextName.IdentifiedOrganization = (byte)reader.ReadElementContentAsInt("IdentifiedOrganization");
                ApplicationContextName.DlmsUA = (byte)reader.ReadElementContentAsInt("DlmsUA");
                ApplicationContextName.ApplicationContext = (byte)reader.ReadElementContentAsInt("ApplicationContext");
                ApplicationContextName.ContextId = (byte)reader.ReadElementContentAsInt("ContextId");
                reader.ReadEndElement("ApplicationContextName");
            }

            if (reader.IsStartElement("XDLMSContextInfo", true))
            {
                XDLMSContextInfo.Conformance = (Conformance)reader.ReadElementContentAsInt("Conformance");
                XDLMSContextInfo.MaxReceivePduSize = (UInt16)reader.ReadElementContentAsInt("MaxReceivePduSize");
                XDLMSContextInfo.MaxSendPduSize = (UInt16)reader.ReadElementContentAsInt("MaxSendPduSize");
                XDLMSContextInfo.DlmsVersionNumber = (byte)reader.ReadElementContentAsInt("DlmsVersionNumber");
                XDLMSContextInfo.QualityOfService = (sbyte)reader.ReadElementContentAsInt("QualityOfService");
                XDLMSContextInfo.CypheringInfo = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("CypheringInfo"));
                reader.ReadEndElement("XDLMSContextInfo");
            }
            if (reader.IsStartElement("XDLMSContextInfo", true))
            {
                AuthenticationMechanismName.JointIsoCtt = (byte)reader.ReadElementContentAsInt("JointIsoCtt");
                AuthenticationMechanismName.Country = (byte)reader.ReadElementContentAsInt("Country");
                AuthenticationMechanismName.CountryName = (UInt16)reader.ReadElementContentAsInt("CountryName");
                AuthenticationMechanismName.IdentifiedOrganization = (byte)reader.ReadElementContentAsInt("IdentifiedOrganization");
                AuthenticationMechanismName.DlmsUA = (byte)reader.ReadElementContentAsInt("DlmsUA");
                AuthenticationMechanismName.AuthenticationMechanismName = (byte)reader.ReadElementContentAsInt("AuthenticationMechanismName");
                AuthenticationMechanismName.MechanismId = (Authentication)reader.ReadElementContentAsInt("MechanismId");
                reader.ReadEndElement("XDLMSContextInfo");
            }
            string str = reader.ReadElementContentAsString("Secret");
            if (str == null)
            {
                Secret = null;
            }
            else
            {
                Secret = GXDLMSTranslator.HexToBytes(str);
            }
            AssociationStatus = (AssociationStatus)reader.ReadElementContentAsInt("AssociationStatus");
            SecuritySetupReference = reader.ReadElementContentAsString("SecuritySetupReference");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("ClientSAP", ClientSAP);
            writer.WriteElementString("ServerSAP", ServerSAP);
            if (ApplicationContextName != null)
            {
                writer.WriteStartElement("ApplicationContextName");
                writer.WriteElementString("JointIsoCtt", ApplicationContextName.JointIsoCtt);
                writer.WriteElementString("Country", ApplicationContextName.Country);
                writer.WriteElementString("CountryName", ApplicationContextName.CountryName);
                writer.WriteElementString("IdentifiedOrganization", ApplicationContextName.IdentifiedOrganization);
                writer.WriteElementString("DlmsUA", ApplicationContextName.DlmsUA);
                writer.WriteElementString("ApplicationContext", ApplicationContextName.ApplicationContext);
                writer.WriteElementString("ContextId", ApplicationContextName.ContextId);
                writer.WriteEndElement();
            }
            if (XDLMSContextInfo != null)
            {
                writer.WriteStartElement("XDLMSContextInfo");
                writer.WriteElementString("Conformance", ((int)XDLMSContextInfo.Conformance));
                writer.WriteElementString("MaxReceivePduSize", XDLMSContextInfo.MaxReceivePduSize);
                writer.WriteElementString("MaxSendPduSize", XDLMSContextInfo.MaxSendPduSize);
                writer.WriteElementString("DlmsVersionNumber", XDLMSContextInfo.DlmsVersionNumber);
                writer.WriteElementString("QualityOfService", XDLMSContextInfo.QualityOfService);
                writer.WriteElementString("CypheringInfo", GXDLMSTranslator.ToHex(XDLMSContextInfo.CypheringInfo));
                writer.WriteEndElement();
            }
            if (AuthenticationMechanismName != null)
            {
                writer.WriteStartElement("XDLMSContextInfo");
                writer.WriteElementString("JointIsoCtt", AuthenticationMechanismName.JointIsoCtt);
                writer.WriteElementString("Country", AuthenticationMechanismName.Country);
                writer.WriteElementString("CountryName", AuthenticationMechanismName.CountryName);
                writer.WriteElementString("IdentifiedOrganization", AuthenticationMechanismName.IdentifiedOrganization);
                writer.WriteElementString("DlmsUA", AuthenticationMechanismName.DlmsUA);
                writer.WriteElementString("AuthenticationMechanismName", AuthenticationMechanismName.AuthenticationMechanismName);
                writer.WriteElementString("MechanismId", (int)AuthenticationMechanismName.MechanismId);
                writer.WriteEndElement();
            }
            writer.WriteElementString("Secret", GXDLMSTranslator.ToHex(Secret));
            writer.WriteElementString("AssociationStatus", (int)AssociationStatus);
            writer.WriteElementString("SecuritySetupReference", SecuritySetupReference);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
