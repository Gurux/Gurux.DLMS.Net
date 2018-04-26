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
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAssociationShortName
    /// </summary>
    public class GXDLMSAssociationShortName : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAssociationShortName()
        : this("0.0.40.0.0.255", 0xFA00)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSAssociationShortName(string ln, ushort sn)
        : base(ObjectType.AssociationShortName, ln, sn)
        {
            Version = 2;
            ObjectList = new GXDLMSObjectCollection();
        }

        /// <summary>
        /// Secret used in Authentication
        /// </summary>
        [XmlIgnore()]
        public byte[] Secret
        {
            get;
            set;
        }

        /// <summary>
        /// List of available objects in short name referencing.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSObjectCollection ObjectList
        {
            get;
            set;
        }

        /// <summary>
        /// Security setup reference.
        /// </summary>
        [XmlIgnore()]
        public string SecuritySetupReference
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ObjectList, null, SecuritySetupReference };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            //Check reply_to_HLS_authentication
            if (e.Index == 8)
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
                    secret = Secret;
                }
                byte[] serverChallenge = GXSecure.Secure(settings, settings.Cipher, ic, settings.StoCChallenge, secret);
                byte[] clientChallenge = (byte[])e.Parameters;
                if (GXCommon.Compare(serverChallenge, clientChallenge))
                {
                    if (settings.Authentication == Authentication.HighGMAC)
                    {
                        secret = settings.Cipher.SystemTitle;
                        ic = settings.Cipher.InvocationCounter;
                    }
                    else
                    {
                        secret = Secret;
                    }
                    settings.Connected = ConnectionState.Dlms;
                    return GXSecure.Secure(settings, settings.Cipher, ic, settings.CtoSChallenge, secret);
                }
                else
                {
                    // If the password does not match.
                    settings.Connected &= ~ConnectionState.Dlms;
                    return null;
                }

            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
                return null;
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
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            if (Version > 1)
            {
                //AccessRightsList is static and read only once.
                if (all || !base.IsRead(3))
                {
                    attributes.Add(3);
                }
                //SecuritySetupReference is static and read only once.
                if (all || !base.IsRead(4))
                {
                    attributes.Add(4);
                }
                if (Version > 2)
                {
                    //user_list
                    // current_user 
                }
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            if (Version < 2)
            {
                return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Object List"};
            }
            return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Object List",
                             "Access Rights List",
                             "Security Setup Reference"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            if (Version < 2)
            {
                return 2;
            }
            return 4;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 8;
        }

        private void GetAccessRights(GXDLMSSettings settings, GXDLMSObject item, GXDLMSServer server, GXByteBuffer data)
        {
            data.SetUInt8((byte)DataType.Structure);
            data.SetUInt8((byte)3);
            GXCommon.SetData(settings, data, DataType.UInt16, item.ShortName);

            int cnt = (item as IGXDLMSBase).GetAttributeCount();
            data.SetUInt8((byte)DataType.Array);
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
                data.SetUInt8((byte)2);
                GXCommon.SetData(settings, data, DataType.Int8, e.Index);
                GXCommon.SetData(settings, data, DataType.Enum, m);
            }
            cnt = (item as IGXDLMSBase).GetMethodCount();
            data.SetUInt8((byte)DataType.Array);
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
                GXCommon.SetData(settings, data, DataType.Int8, e.Index);
                GXCommon.SetData(settings, data, DataType.Enum, m);
            }
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            else if (index == 2)
            {
                return DataType.Array;
            }
            else if (index == 3)
            {
                return DataType.Array;
            }
            else if (index == 4)
            {
                return DataType.OctetString;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        /// <summary>
        /// Returns Association View.
        /// </summary>
        private GXByteBuffer GetObjects(GXDLMSSettings settings, ValueEventArgs e)
        {
            int cnt = ObjectList.Count;
            GXByteBuffer data = new GXByteBuffer();
            //Add count only for first time.
            if (settings.Index == 0)
            {
                settings.Count = (UInt16)cnt;
                data.SetUInt8((byte)DataType.Array);
                GXCommon.SetObjectCount(cnt, data);
            }
            ushort pos = 0;
            foreach (GXDLMSObject it in ObjectList)
            {
                ++pos;
                if (!(pos <= settings.Index))
                {
                    data.SetUInt8((byte)DataType.Structure);
                    //Count
                    data.SetUInt8((byte)4);
                    //base address.
                    GXCommon.SetData(settings, data, DataType.Int16, it.ShortName);
                    //ClassID
                    GXCommon.SetData(settings, data, DataType.UInt16, it.ObjectType);
                    //Version
                    GXCommon.SetData(settings, data, DataType.UInt8, it.Version);
                    GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(it.LogicalName));
                    ++settings.Index;
                    if (settings.IsServer)
                    {
                        //If PDU is full.
                        if (!e.SkipMaxPduSize && data.Size >= settings.MaxPduSize)
                        {
                            break;
                        }
                    }
                }
            }
            return data;
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (ObjectList == null)
            {
                ObjectList = new GXDLMSObjectCollection();
            }
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            else if (e.Index == 2)
            {
                return GetObjects(settings, e).Array();
            }
            else if (e.Index == 3)
            {
                bool lnExists = ObjectList.FindBySN(this.ShortName) != null;
                //Add count
                int cnt = ObjectList.Count;
                if (!lnExists)
                {
                    ++cnt;
                }
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                GXCommon.SetObjectCount(cnt, data);
                foreach (GXDLMSObject it in ObjectList)
                {
                    GetAccessRights(settings, it, e.Server, data);
                }
                if (!lnExists)
                {
                    GetAccessRights(settings, this, e.Server, data);
                }
                return data.Array();
            }
            else if (e.Index == 4)
            {
                return GXCommon.LogicalNameToBytes(SecuritySetupReference);
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void UpdateAccessRights(Object[] buff)
        {
            foreach (Object[] access in buff)
            {
                ushort sn = Convert.ToUInt16(access[0]);
                GXDLMSObject obj = ObjectList.FindBySN(sn);
                if (obj != null)
                {
                    foreach (Object[] attributeAccess in (Object[])access[1])
                    {
                        int id = Convert.ToInt32(attributeAccess[0]);
                        int mode = Convert.ToInt32(attributeAccess[1]);
                        obj.SetAccess(id, (AccessMode)mode);
                    }
                    foreach (Object[] methodAccess in (Object[])access[2])
                    {
                        int id = Convert.ToInt32(methodAccess[0]);
                        int mode = Convert.ToInt32(methodAccess[1]);
                        obj.SetMethodAccess(id, (MethodAccessMode)mode);
                    }
                }
            }
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
                        ushort sn = (ushort)(Convert.ToInt32(item[0]) & 0xFFFF);
                        ObjectType type = (ObjectType)Convert.ToInt32(item[1]);
                        int version = Convert.ToInt32(item[2]);
                        String ln = GXCommon.ToLogicalName((byte[])item[3]);
                        GXDLMSObject obj = null;
                        if (settings.Objects != null)
                        {
                            obj = settings.Objects.FindBySN(sn);
                        }
                        if (obj == null)
                        {
                            obj = Gurux.DLMS.GXDLMSClient.CreateObject(type);
                            if (obj != null)
                            {
                                obj.LogicalName = ln;
                                obj.ShortName = sn;
                                obj.Version = version;
                            }
                        }
                        //Unknown objects are not shown.
                        if (obj is IGXDLMSBase)
                        {
                            ObjectList.Add(obj);
                        }
                    }
                }
            }
            else if (e.Index == 3)
            {
                if (e.Value == null)
                {
                    foreach (GXDLMSObject it in ObjectList)
                    {
                        for (int pos = 1; pos != (it as IGXDLMSBase).GetAttributeCount(); ++pos)
                        {
                            it.SetAccess(pos, AccessMode.NoAccess);
                        }
                    }
                }
                else
                {
                    UpdateAccessRights((Object[])e.Value);
                }
            }
            else if (e.Index == 4)
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
            string str = reader.ReadElementContentAsString("Secret");
            if (str == null)
            {
                Secret = null;
            }
            else
            {
                Secret = GXDLMSTranslator.HexToBytes(str);
            }
            SecuritySetupReference = reader.ReadElementContentAsString("SecuritySetupReference");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Secret", GXDLMSTranslator.ToHex(Secret));
            writer.WriteElementString("SecuritySetupReference", SecuritySetupReference);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
