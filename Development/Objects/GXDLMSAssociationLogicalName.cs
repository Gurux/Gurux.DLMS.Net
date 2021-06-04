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
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using System.Text;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAssociationLogicalName
    /// </summary>
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
            UserList = new List<KeyValuePair<byte, string>>();
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
            return new object[] { LogicalName, ObjectList, new object[]{ClientSAP, ServerSAP }, ApplicationContextName,
                              XDLMSContextInfo, AuthenticationMechanismName, Secret, AssociationStatus, SecuritySetupReference,
                            UserList, CurrentUser};
        }

        [XmlIgnore()]
        public List<KeyValuePair<byte, string>> UserList
        {
            get;
            set;
        }

        [XmlIgnore()]
        public KeyValuePair<byte, string> CurrentUser
        {
            get;
            set;
        }

        /// <summary>
        /// Updates secret.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] UpdateSecret(GXDLMSClient client)
        {
            if (AuthenticationMechanismName.MechanismId == Authentication.None)
            {
                throw new ArgumentException("Invalid authentication level in MechanismId.");
            }
            if (AuthenticationMechanismName.MechanismId == Authentication.HighGMAC)
            {
                throw new ArgumentException("HighGMAC secret is updated using Security setup.");
            }
            if (AuthenticationMechanismName.MechanismId == Authentication.Low)
            {
                return client.Write(this, 7);
            }
            //Action is used to update High authentication password.
            return client.Method(this, 2, Secret, DataType.OctetString);
        }

        /// <summary>
        /// Add object to object list.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="obj">COSEM object.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] AddObject(GXDLMSClient client, GXDLMSObject obj)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Structure);
            //Add structure size.
            data.SetUInt8(4);
            //ClassID
            GXCommon.SetData(null, data, DataType.UInt16, obj.ObjectType);
            //Version
            GXCommon.SetData(null, data, DataType.UInt8, obj.Version);
            //LN
            GXCommon.SetData(null, data, DataType.OctetString, GXCommon.LogicalNameToBytes(obj.LogicalName));
            //Access rights.
            GetAccessRights(null, obj, null, data);
            return client.Method(this, 3, data.Array(), DataType.Structure);
        }

        /// <summary>
        /// Remove object from object list.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="obj">COSEM object.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] RemoveObject(GXDLMSClient client, GXDLMSObject obj)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Structure);
            //Add structure size.
            data.SetUInt8(4);
            //ClassID
            GXCommon.SetData(null, data, DataType.UInt16, obj.ObjectType);
            //Version
            GXCommon.SetData(null, data, DataType.UInt8, obj.Version);
            //LN
            GXCommon.SetData(null, data, DataType.OctetString, GXCommon.LogicalNameToBytes(obj.LogicalName));
            //Access rights.
            GetAccessRights(null, obj, null, data);
            return client.Method(this, 4, data.Array(), DataType.Structure);
        }


        /// <summary>
        /// Add user to user list.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="id">User ID.</param>
        /// <param name="name">User name.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] AddUser(GXDLMSClient client, byte id, string name)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Structure);
            //Add structure size.
            data.SetUInt8(2);
            GXCommon.SetData(null, data, DataType.UInt8, id);
            GXCommon.SetData(null, data, DataType.String, name);
            return client.Method(this, 5, data.Array(), DataType.Structure);
        }

        /// <summary>
        /// Remove user from user list.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="id">User ID.</param>
        /// <param name="name">User name.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] RemoveUser(GXDLMSClient client, byte id, string name)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Structure);
            //Add structure size.
            data.SetUInt8(2);
            GXCommon.SetData(null, data, DataType.UInt8, id);
            GXCommon.SetData(null, data, DataType.String, name);
            return client.Method(this, 6, data.Array(), DataType.Structure);
        }


        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            //Check reply_to_HLS_authentication
            switch (e.Index)
            {
                case 1:
                    return ReplyToHlsAuthentication(settings, e);
                case 2:
                    ChangeHlsSecret(e);
                    break;
                case 3:
                    AddObject(settings, e);
                    break;
                case 4:
                    RemoveObject(settings, e);
                    break;
                case 5:
                    AddUser(e);
                    break;
                case 6:
                    RemoveUser(e);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return null;
        }

        private byte[] ReplyToHlsAuthentication(GXDLMSSettings settings, ValueEventArgs e)
        {
            uint ic = 0;
            byte[] secret;
            if (settings.Authentication == Authentication.HighGMAC)
            {
                secret = settings.SourceSystemTitle;
                GXByteBuffer bb = new GXByteBuffer(e.Parameters as byte[]);
                bb.GetUInt8();
                ic = bb.GetUInt32();
            }
            else if (settings.Authentication == Authentication.HighSHA256)
            {
                GXByteBuffer tmp = new GXByteBuffer();
                tmp.Set(Secret);
                tmp.Set(settings.SourceSystemTitle);
                tmp.Set(settings.Cipher.SystemTitle);
                tmp.Set(settings.StoCChallenge);
                tmp.Set(settings.CtoSChallenge);
                secret = tmp.Array();
            }
            else
            {
                secret = Secret;
            }
            byte[] serverChallenge = GXSecure.Secure(settings, settings.Cipher, ic, settings.StoCChallenge, secret);
            byte[] clientChallenge = (byte[])e.Parameters;
            if (serverChallenge != null && clientChallenge != null && GXCommon.Compare(serverChallenge, clientChallenge))
            {
                if (settings.Authentication == Authentication.HighGMAC)
                {
                    secret = settings.Cipher.SystemTitle;
                    ic = settings.Cipher.InvocationCounter;
                    ++settings.Cipher.InvocationCounter;
                }
                else
                {
                    secret = Secret;
                }
                AssociationStatus = AssociationStatus.Associated;
                if (settings.Authentication == Authentication.HighSHA256)
                {
                    GXByteBuffer tmp = new GXByteBuffer();
                    tmp.Set(Secret);
                    tmp.Set(settings.Cipher.SystemTitle);
                    tmp.Set(settings.SourceSystemTitle);
                    tmp.Set(settings.CtoSChallenge);
                    tmp.Set(settings.StoCChallenge);
                    secret = tmp.Array();
                }
                return GXSecure.Secure(settings, settings.Cipher, ic, settings.CtoSChallenge, secret);
            }
            else //If the password does not match.
            {
                AssociationStatus = AssociationStatus.NonAssociated;
                e.Error = ErrorCode.ReadWriteDenied;
                return null;
            }
        }

        private void RemoveUser(ValueEventArgs e)
        {
            List<object> tmp = e.Parameters as List<object>;
            if (tmp == null || tmp.Count != 2)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            else
            {
                UserList.Remove(new KeyValuePair<byte, string>(Convert.ToByte(tmp[0]), Convert.ToString(tmp[1])));
            }
        }

        private void AddUser(ValueEventArgs e)
        {
            List<object> tmp = e.Parameters as List<object>;
            if (tmp == null || tmp.Count != 2)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            else
            {
                UserList.Add(new KeyValuePair<byte, string>(Convert.ToByte(tmp[0]), Convert.ToString(tmp[1])));
            }
        }

        private void RemoveObject(GXDLMSSettings settings, ValueEventArgs e)
        {
            //Remove COSEM object.
            GXDLMSObject obj = GetObject(settings, e.Parameters as List<object>, false);
            //Unknown objects are not removed.
            if (obj is IGXDLMSBase)
            {
                GXDLMSObject t = ObjectList.FindByLN(obj.ObjectType, obj.LogicalName);
                if (t != null)
                {
                    ObjectList.Remove(t);
                }
            }
        }

        private void AddObject(GXDLMSSettings settings, ValueEventArgs e)
        {
            //Add COSEM object.
            GXDLMSObject obj = GetObject(settings, e.Parameters as List<object>, true);
            //Unknown objects are not add.
            if (obj is IGXDLMSBase)
            {
                if (ObjectList.FindByLN(obj.ObjectType, obj.LogicalName) == null)
                {
                    ObjectList.Add(obj);
                }
            }
        }

        private void ChangeHlsSecret(ValueEventArgs e)
        {
            byte[] tmp = e.Parameters as byte[];
            if (tmp == null || tmp.Length == 0)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            else
            {
                Secret = tmp;
            }
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }

            //ObjectList is static and read only once.
            if (all || ObjectList.Count == 0)
            {
                attributes.Add(2);
            }

            //associated_partners_id is static and read only once.
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //Application Context Name is static and read only once.
            if (all || !base.IsRead(4))
            {
                attributes.Add(4);
            }

            //xDLMS Context Info
            if (all || !base.IsRead(5))
            {
                attributes.Add(5);
            }

            // Authentication Mechanism Name
            if (all || !base.IsRead(6))
            {
                attributes.Add(6);
            }

            // LLS Secret
            if (all || !base.IsRead(7))
            {
                attributes.Add(7);
            }
            // Association Status
            if (all || !base.IsRead(8))
            {
                attributes.Add(8);
            }
            //Security Setup Reference is from version 1.
            if (Version > 0 && (all || !base.IsRead(9)))
            {
                attributes.Add(9);
            }
            //User list and current user are in version 2.
            if (Version > 1)
            {
                if (all || !base.IsRead(10))
                {
                    attributes.Add(10);
                }
                if (all || !base.IsRead(11))
                {
                    attributes.Add(11);
                }
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
                                 "Associated partners Id",
                                 "Application Context Name",
                                 "xDLMS Context Info",
                                 "Authentication Mechanism Name",
                                 "Secret",
                                 "Association Status"
                                };
            }
            if (Version == 1)
            {
                return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Object List",
                             "Associated partners Id",
                             "Application Context Name",
                             "xDLMS Context Info",
                             "Authentication Mechanism Name",
                             "Secret",
                             "Association Status",
                             "Security Setup Reference"
                            };
            }
            return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Object List",
                             "Associated partners Id",
                             "Application Context Name",
                             "xDLMS Context Info",
                             "Authentication Mechanism Name",
                             "Secret",
                             "Association Status",
                             "Security Setup Reference",
                             "UserList", "CurrentUser"
                            };

        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            if (Version > 1)
                return new string[] { "Reply to HLS authentication", "Change HLS secret",
                "Add object", "Remove object", "Add user", "Remove user"};
            return new string[] { "Reply to HLS authentication", "Change HLS secret",
                "Add object", "Remove object"};
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 3;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            if (Version > 1)
                return 11;
            //Security Setup Reference is from version 1.
            if (Version > 0)
                return 9;
            return 8;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            if (Version > 1)
                return 6;
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
                    if (settings.IsServer)
                    {
                        //If PDU is full.
                        if ((settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) == 0 && !e.SkipMaxPduSize && data.Size >= settings.MaxPduSize)
                        {
                            break;
                        }
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
                    m = item.GetAccess(e.Index);
                }
                //attribute_access_item
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8((byte)3);
                GXCommon.SetData(settings, data, DataType.Int8, e.Index);
                GXCommon.SetData(settings, data, DataType.Enum, m);
                byte accessSelector = item.GetAccessSelector(e.Index);
                if (accessSelector == 0)
                {
                    //Profile generic buffer can be read using entry and range.
                    if (item is GXDLMSProfileGeneric && e.Index == 2)
                    {
                        accessSelector = 3;
                    }
                }
                if (accessSelector != 0)
                {
                    List<object> list = new List<object>();
                    for (sbyte index = 0; index != 8; ++index)
                    {
                        if ((accessSelector & (1 << index)) != 0)
                        {
                            list.Add(index);
                        }
                    }
                    GXCommon.SetData(settings, data, DataType.Array, list);
                }
                else
                {
                    GXCommon.SetData(settings, data, DataType.None, null);
                }
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
                    m = item.GetMethodAccess(e.Index);
                }
                //attribute_access_item
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8((byte)2);
                GXCommon.SetData(settings, data, DataType.Int8, pos + 1);
                GXCommon.SetData(settings, data, DataType.Enum, m);
            }
        }

        void UpdateAccessRights(GXDLMSObject obj, List<object> buff)
        {
            if (buff.Count != 0)
            {
                List<object> arr, it;
                if (buff[0] is List<object>)
                {
                    arr = (List<object>)buff[0];
                }
                else
                {
                    arr = new List<object>((object[])buff[0]);
                }
                foreach (object tmp in arr)
                {
                    if (tmp is List<object>)
                    {
                        it = (List<object>)tmp;
                    }
                    else
                    {
                        it = new List<object>((object[])tmp);
                    }
                    int id = Convert.ToInt32(it[0]);
                    int mode = Convert.ToInt32(it[1]);
                    if (Version < 3)
                    {
                        obj.SetAccess(id, (AccessMode)mode);
                    }
                    else
                    {
                        obj.SetAccess3(id, (AccessMode3)mode);
                    }
                }
                if (buff[1] is List<object>)
                {
                    arr = (List<object>)buff[1];
                }
                else
                {
                    arr = new List<object>((object[])buff[1]);
                }
                foreach (object tmp in arr)
                {
                    if (tmp is List<object>)
                    {
                        it = (List<object>)tmp;
                    }
                    else
                    {
                        it = new List<object>((object[])tmp);
                    }
                    int id = Convert.ToInt32(it[0]);
                    int tmp2;
                    //If version is 0.
                    if (it[1] is Boolean)
                    {
                        tmp2 = ((Boolean)it[1]) ? 1 : 0;
                    }
                    else//If version is 1.
                    {
                        tmp2 = Convert.ToInt32(it[1]);
                    }
                    if (Version < 3)
                    {
                        obj.SetMethodAccess(id, (MethodAccessMode)tmp2);
                    }
                    else
                    {
                        obj.SetMethodAccess3(id, (MethodAccessMode3)tmp2);
                    }
                }
            }
        }

        /// <summary>
        /// Returns User list.
        /// </summary>
        private GXByteBuffer GetUserList(GXDLMSSettings settings, ValueEventArgs e)
        {
            GXByteBuffer data = new GXByteBuffer();
            //Add count only for first time.
            if (settings.Index == 0)
            {
                settings.Count = (UInt16)UserList.Count;
                data.SetUInt8((byte)DataType.Array);
                GXCommon.SetObjectCount(UserList.Count, data);
            }
            ushort pos = 0;
            foreach (KeyValuePair<byte, string> it in UserList)
            {
                ++pos;
                if (!(pos <= settings.Index))
                {
                    ++settings.Index;
                    data.SetUInt8((byte)DataType.Structure);
                    data.SetUInt8(2); //Count
                    GXCommon.SetData(settings, data, DataType.UInt8, it.Key); //Id
                    GXCommon.SetData(settings, data, DataType.String, it.Value); //Name
                }
            }
            return data;
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
            if (Version > 0)
            {
                if (index == 9)
                {
                    return DataType.OctetString;
                }
            }
            if (Version > 1)
            {
                if (index == 10)
                {
                    return DataType.Array;
                }
                if (index == 11)
                {
                    return DataType.Structure;
                }
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
                e.ByteArray = true;
                return GetObjects(settings, e).Array();
            }
            if (e.Index == 3)
            {
                e.ByteArray = true;
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                //Add count
                data.SetUInt8(2);
                data.SetUInt8((byte)DataType.Int8);
                data.SetUInt8(ClientSAP);
                data.SetUInt8((byte)DataType.UInt16);
                data.SetUInt16(ServerSAP);
                return data.Array();
            }
            if (e.Index == 4)
            {
                e.ByteArray = true;
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
                e.ByteArray = true;
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(6);
                GXCommon.SetData(settings, data, DataType.BitString, GXBitString.ToBitString((UInt32)XDLMSContextInfo.Conformance, 24));
                GXCommon.SetData(settings, data, DataType.UInt16, XDLMSContextInfo.MaxReceivePduSize);
                GXCommon.SetData(settings, data, DataType.UInt16, XDLMSContextInfo.MaxSendPduSize);
                GXCommon.SetData(settings, data, DataType.UInt8, XDLMSContextInfo.DlmsVersionNumber);
                GXCommon.SetData(settings, data, DataType.Int8, XDLMSContextInfo.QualityOfService);
                GXCommon.SetData(settings, data, DataType.OctetString, XDLMSContextInfo.CypheringInfo);
                return data.Array();
            }
            if (e.Index == 6)
            {
                e.ByteArray = true;
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
            if (e.Index == 10)
            {
                return GetUserList(settings, e).Array();
            }
            if (e.Index == 11)
            {
                e.ByteArray = true;
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                //Add structure size.
                data.SetUInt8(2);
                GXCommon.SetData(settings, data, DataType.UInt8, CurrentUser.Key);
                GXCommon.SetData(settings, data, DataType.String, CurrentUser.Value);
                return data.Array();
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        /// <summary>
        /// Get object.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="item">Received data.</param>
        /// <param name="add">Is data added to settings object list.</param>
        private GXDLMSObject GetObject(GXDLMSSettings settings, List<object> item, bool add)
        {
            ObjectType type = (ObjectType)Convert.ToInt32(item[0]);
            int version = Convert.ToInt32(item[1]);
            string ln = GXCommon.ToLogicalName((byte[])item[2]);
            GXDLMSObject obj = null;
            if (settings != null && settings.Objects != null)
            {
                obj = settings.Objects.FindByLN(type, ln);
            }
            if (obj == null)
            {
                obj = GXDLMSClient.CreateObject(type);
                obj.LogicalName = ln;
                obj.Version = version;
                if (add && settings.IsServer)
                {
                    settings.Objects.Add(obj);
                }
            }
            if (add && obj is IGXDLMSBase && item[3] != null)
            {
                List<object> arr;
                if (item[3] is List<object>)
                {
                    arr = (List<object>)item[3];
                }
                else
                {
                    arr = new List<object>((object[])item[3]);
                }
                UpdateAccessRights(obj, arr);
            }
            return obj;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                LogicalName = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 2)
            {
                UpdateObjectList(settings, e);
            }
            else if (e.Index == 3)
            {
                if (e.Value != null)
                {
                    List<object> arr;
                    if (e.Value is List<object>)
                    {
                        arr = (List<object>)e.Value;
                    }
                    else
                    {
                        arr = new List<object>((object[])e.Value);
                    }
                    ClientSAP = Convert.ToByte(arr[0]);
                    ServerSAP = Convert.ToUInt16(arr[1]);
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
                        //BB 11.4.
                        byte val = arr.GetUInt8();
                        ApplicationContextName.JointIsoCtt = (byte)((val - 16) / 40);
                        ApplicationContextName.Country = 16;
                        int tmp = arr.GetUInt16();
                        tmp = ((tmp & 0x7F00) >> 1) | (tmp & 0x7F);
                        ApplicationContextName.CountryName = (UInt16)tmp;
                        ApplicationContextName.CountryName = (UInt16)tmp;
                        ApplicationContextName.IdentifiedOrganization = arr.GetUInt8();
                        ApplicationContextName.DlmsUA = arr.GetUInt8();
                        ApplicationContextName.ApplicationContext = arr.GetUInt8();
                        ApplicationContextName.ContextId = (ApplicationContextName)arr.GetUInt8();
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
                        ApplicationContextName.ContextId = (ApplicationContextName)arr.GetUInt8();
                    }
                }
                else if (e.Value != null)
                {
                    List<object> arr;
                    if (e.Value is List<object>)
                    {
                        arr = (List<object>)e.Value;
                    }
                    else
                    {
                        arr = new List<object>((object[])e.Value);
                    }
                    ApplicationContextName.JointIsoCtt = Convert.ToByte(arr[0]);
                    ApplicationContextName.Country = Convert.ToByte(arr[1]);
                    ApplicationContextName.CountryName = Convert.ToUInt16(arr[2]);
                    ApplicationContextName.IdentifiedOrganization = Convert.ToByte(arr[3]);
                    ApplicationContextName.DlmsUA = Convert.ToByte(arr[4]);
                    ApplicationContextName.ApplicationContext = Convert.ToByte(arr[5]);
                    ApplicationContextName.ContextId = (ApplicationContextName)Convert.ToByte(arr[6]);
                }
            }
            else if (e.Index == 5)
            {
                if (e.Value != null)
                {
                    List<object> arr;
                    if (e.Value is List<object>)
                    {
                        arr = (List<object>)e.Value;
                    }
                    else
                    {
                        arr = new List<object>((object[])e.Value);
                    }
                    XDLMSContextInfo.Conformance = (Conformance)Convert.ToUInt32(arr[0]);
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
                        //BB 11.4.
                        byte val = arr.GetUInt8();
                        AuthenticationMechanismName.JointIsoCtt = (byte)((val - 16) / 40);
                        AuthenticationMechanismName.Country = 16;
                        int tmp = arr.GetUInt16();
                        tmp = ((tmp & 0x7F00) >> 1) | (tmp & 0x7F);
                        AuthenticationMechanismName.CountryName = (UInt16)tmp;
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
                    List<object> arr;
                    if (e.Value is List<object>)
                    {
                        arr = (List<object>)e.Value;
                    }
                    else
                    {
                        arr = new List<object>((object[])e.Value);
                    }
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
            else if (e.Index == 10)
            {
                UserList.Clear();
                if (e.Value != null)
                {
                    List<object> item;
                    foreach (object tmp in (IEnumerable<object>)e.Value)
                    {
                        if (tmp is List<object>)
                        {
                            item = (List<object>)tmp;
                        }
                        else
                        {
                            item = new List<object>((object[])tmp);
                        }
                        UserList.Add(new KeyValuePair<byte, string>(Convert.ToByte(item[0]), Convert.ToString(item[1])));
                    }
                }
            }
            else if (e.Index == 11)
            {
                if (e.Value != null)
                {
                    List<object> arr;
                    if (e.Value is List<object>)
                    {
                        arr = (List<object>)e.Value;
                    }
                    else
                    {
                        arr = new List<object>((object[])e.Value);
                    }
                    if (arr.Count == 1)
                    {
                        CurrentUser = new KeyValuePair<byte, string>(Convert.ToByte(arr[0]), null);
                    }
                    else
                    {
                        CurrentUser = new KeyValuePair<byte, string>(Convert.ToByte(arr[0]), Convert.ToString(arr[1]));
                    }
                }
                else
                {
                    CurrentUser = new KeyValuePair<byte, string>(0, null);
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        private void UpdateObjectList(GXDLMSSettings settings, ValueEventArgs e)
        {
            ObjectList.Clear();
            if (e.Value != null)
            {
                List<object> arr = null;
                if (e.Value is List<object>)
                {
                    arr = (List<object>)e.Value;
                }
                else if (e.Value != null)
                {
                    arr = new List<object>((object[])e.Value);
                }
                foreach (object tmp in arr)
                {
                    List<object> item = null;
                    if (tmp is List<object>)
                    {
                        item = (List<object>)tmp;
                    }
                    else if (tmp != null)
                    {
                        item = new List<object>((object[])tmp);
                    }
                    GXDLMSObject obj = GetObject(settings, item, true);
                    //Unknown objects are not shown.
                    if (obj is IGXDLMSBase)
                    {
                        ObjectList.Add(obj);
                    }
                }
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            string str;
            //Load objects.
            ObjectList.Clear();
            if (reader.IsStartElement("ObjectList", true))
            {
                string target;
                GXDLMSObject obj;
                while (!reader.EOF)
                {
                    obj = null;
                    if (reader.IsStartElement())
                    {
                        target = reader.Name;
                        if (target.StartsWith("GXDLMS"))
                        {
                            str = target.Substring(6);
                            reader.Read();
                            ObjectType type = (ObjectType)Enum.Parse(typeof(ObjectType), str);
                            string ln = reader.ReadElementContentAsString("LN");
                            obj = reader.Objects.FindByLN(type, ln);
                            if (obj == null)
                            {
                                obj = GXDLMSClient.CreateObject(type);
                                obj.LogicalName = ln;
                                reader.Objects.Add(obj);
                            }
                            ObjectList.Add(obj);
                        }
                    }
                    else
                    {
                        if (reader.Name == "ObjectList")
                        {
                            break;
                        }
                        reader.Read();
                    }
                }
                reader.ReadEndElement("ObjectList");
            }
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
                ApplicationContextName.ContextId = (ApplicationContextName)reader.ReadElementContentAsInt("ContextId");
                reader.ReadEndElement("ApplicationContextName");
            }

            if (reader.IsStartElement("XDLMSContextInfo", true))
            {
                XDLMSContextInfo.Conformance = (Conformance)reader.ReadElementContentAsInt("Conformance");
                XDLMSContextInfo.MaxReceivePduSize = (UInt16)reader.ReadElementContentAsInt("MaxReceivePduSize");
                XDLMSContextInfo.MaxSendPduSize = (UInt16)reader.ReadElementContentAsInt("MaxSendPduSize");
                XDLMSContextInfo.DlmsVersionNumber = (byte)reader.ReadElementContentAsInt("DlmsVersionNumber");
                XDLMSContextInfo.QualityOfService = (sbyte)reader.ReadElementContentAsInt("QualityOfService");
                str = reader.ReadElementContentAsString("CypheringInfo");
                if (str != null)
                {
                    XDLMSContextInfo.CypheringInfo = GXDLMSTranslator.HexToBytes(str);
                }
                reader.ReadEndElement("XDLMSContextInfo");
            }
            if (reader.IsStartElement("AuthenticationMechanismName", true) ||
                reader.IsStartElement("XDLMSContextInfo", true))
            {
                AuthenticationMechanismName.JointIsoCtt = (byte)reader.ReadElementContentAsInt("JointIsoCtt");
                AuthenticationMechanismName.Country = (byte)reader.ReadElementContentAsInt("Country");
                AuthenticationMechanismName.CountryName = (UInt16)reader.ReadElementContentAsInt("CountryName");
                AuthenticationMechanismName.IdentifiedOrganization = (byte)reader.ReadElementContentAsInt("IdentifiedOrganization");
                AuthenticationMechanismName.DlmsUA = (byte)reader.ReadElementContentAsInt("DlmsUA");
                AuthenticationMechanismName.AuthenticationMechanismName = (byte)reader.ReadElementContentAsInt("AuthenticationMechanismName");
                AuthenticationMechanismName.MechanismId = (Authentication)reader.ReadElementContentAsInt("MechanismId");
                reader.ReadEndElement("AuthenticationMechanismName");
                reader.ReadEndElement("XDLMSContextInfo");
            }
            str = reader.ReadElementContentAsString("Secret");
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
            //Load users.
            UserList.Clear();
            if (reader.IsStartElement("Users", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    byte id = (byte)reader.ReadElementContentAsInt("Id");
                    string name = reader.ReadElementContentAsString("Name");
                    UserList.Add(new KeyValuePair<byte, string>(id, name));
                }
                reader.ReadEndElement("Users");
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            //Save objects.
            if (ObjectList != null)
            {
                writer.WriteStartElement("ObjectList", 2);
                foreach (GXDLMSObject it in ObjectList)
                {
                    if (it.ObjectType != ObjectType.AssociationLogicalName)
                    {
                        writer.WriteStartElement("GXDLMS" + it.ObjectType.ToString(), 0);
                        // Add LN
                        writer.WriteElementString("LN", it.LogicalName, 0);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }
            writer.WriteElementString("ClientSAP", ClientSAP, 3);
            writer.WriteElementString("ServerSAP", ServerSAP, 3);
            writer.WriteStartElement("ApplicationContextName", 4);
            if (ApplicationContextName != null)
            {
                writer.WriteElementString("JointIsoCtt", ApplicationContextName.JointIsoCtt, 4);
                writer.WriteElementString("Country", ApplicationContextName.Country, 4);
                writer.WriteElementString("CountryName", ApplicationContextName.CountryName, 4);
                writer.WriteElementString("IdentifiedOrganization", ApplicationContextName.IdentifiedOrganization, 4);
                writer.WriteElementString("DlmsUA", ApplicationContextName.DlmsUA, 4);
                writer.WriteElementString("ApplicationContext", ApplicationContextName.ApplicationContext, 4);
                writer.WriteElementString("ContextId", (int)ApplicationContextName.ContextId, 4);
            }
            writer.WriteEndElement();
            writer.WriteStartElement("XDLMSContextInfo", 5);
            if (XDLMSContextInfo != null)
            {
                writer.WriteElementString("Conformance", (int)XDLMSContextInfo.Conformance, 5);
                writer.WriteElementString("MaxReceivePduSize", XDLMSContextInfo.MaxReceivePduSize, 5);
                writer.WriteElementString("MaxSendPduSize", XDLMSContextInfo.MaxSendPduSize, 5);
                writer.WriteElementString("DlmsVersionNumber", XDLMSContextInfo.DlmsVersionNumber, 5);
                writer.WriteElementString("QualityOfService", XDLMSContextInfo.QualityOfService, 5);
                writer.WriteElementString("CypheringInfo", GXDLMSTranslator.ToHex(XDLMSContextInfo.CypheringInfo), 5);
            }
            writer.WriteEndElement();
            writer.WriteStartElement("AuthenticationMechanismName", 6);
            if (AuthenticationMechanismName != null)
            {
                writer.WriteElementString("JointIsoCtt", AuthenticationMechanismName.JointIsoCtt, 6);
                writer.WriteElementString("Country", AuthenticationMechanismName.Country, 6);
                writer.WriteElementString("CountryName", AuthenticationMechanismName.CountryName, 6);
                writer.WriteElementString("IdentifiedOrganization", AuthenticationMechanismName.IdentifiedOrganization, 6);
                writer.WriteElementString("DlmsUA", AuthenticationMechanismName.DlmsUA, 6);
                writer.WriteElementString("AuthenticationMechanismName", AuthenticationMechanismName.AuthenticationMechanismName, 6);
                writer.WriteElementString("MechanismId", (int)AuthenticationMechanismName.MechanismId, 6);
            }
            writer.WriteEndElement();
            writer.WriteElementString("Secret", GXDLMSTranslator.ToHex(Secret), 7);
            writer.WriteElementString("AssociationStatus", (int)AssociationStatus, 8);
            if (string.IsNullOrEmpty(SecuritySetupReference))
            {
                writer.WriteElementString("SecuritySetupReference", "0.0.0.0.0.0", 9);
            }
            else
            {
                writer.WriteElementString("SecuritySetupReference", SecuritySetupReference, 9);
            }
            //Save users.
            if (UserList != null)
            {
                writer.WriteStartElement("Users", 10);
                foreach (KeyValuePair<byte, string> it in UserList)
                {
                    writer.WriteStartElement("User", 10);
                    writer.WriteElementString("Id", it.Key, 10);
                    writer.WriteElementString("Name", it.Value, 10);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
            //Copy objects from the object collection to the association object list.
            for (int pos = 0; pos != ObjectList.Count; ++pos)
            {
                GXDLMSObject obj = ObjectList[pos];
                GXDLMSObject tmp = reader.Objects.FindByLN(obj.ObjectType, obj.LogicalName);
                if (tmp != null)
                {
                    ObjectList.Remove(obj);
                    ObjectList.Add(tmp);
                }
            }
        }
        #endregion
    }
}
