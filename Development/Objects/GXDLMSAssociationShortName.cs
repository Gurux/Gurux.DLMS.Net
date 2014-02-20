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
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
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
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSAssociationShortName(string ln, ushort sn)
            : base(ObjectType.AssociationShortName, ln, 0)
        {
            ObjectList = new GXDLMSObjectCollection();
        }

        [XmlIgnore()]
        public GXDLMSObjectCollection ObjectList
        {
            get;
            internal set;
        }

        [XmlIgnore()]
        public object AccessRightsList
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
            return new object[] { LogicalName, ObjectList, AccessRightsList, SecuritySetupReference };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(object sender, int index, Object parameters)
        {
            //Check reply_to_HLS_authentication
            if (index == 8)
            {
                GXDLMSServerBase s = sender as GXDLMSServerBase;
                if (s == null)
                {
                    throw new ArgumentException("sender");
                }
                GXDLMS b = s.m_Base;
                //Get server Challenge.
                List<byte> challenge = null;
                List<byte> CtoS = null;
                //Find shared secret
                foreach (GXAuthentication it in s.Authentications)
                {
                    if (it.Type == b.Authentication)
                    {
                        CtoS = new List<byte>(it.SharedSecret);
                        challenge = new List<byte>(it.SharedSecret);
                        challenge.AddRange(b.StoCChallenge);
                        break;
                    }
                }
                byte[] serverChallenge = GXDLMS.Chipher(b.Authentication, challenge.ToArray());
                byte[] clientChallenge = (byte[]) parameters;
                int pos = 0;
                if (GXCommon.Compare(clientChallenge, ref pos, serverChallenge))
                {
                    CtoS.AddRange(b.CtoSChallenge);
                    return s.Acknowledge(Command.WriteResponse, 0, GXDLMS.Chipher(b.Authentication, CtoS.ToArray()), DataType.OctetString);
                }
                else
                {
                    //Return error.
                    return s.ServerReportError(1, 5, 3);                    
                }
            }
            else
            {
                throw new ArgumentException("Invoke failed. Invalid attribute index.");
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
            //AccessRightsList is static and read only once.
            if (!base.IsRead(3))
            {
                attributes.Add(3);
            }
            //SecuritySetupReference is static and read only once.
            if (!base.IsRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 4;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 8;
        }

        private void GetAccessRights(GXDLMSObject item, List<byte> data)
        {
            data.Add((byte)DataType.Structure);
            data.Add((byte)3);
            GXCommon.SetData(data, DataType.UInt16, item.ShortName);
            data.Add((byte)DataType.Array);
            data.Add((byte)item.Attributes.Count);
            foreach (GXDLMSAttributeSettings att in item.Attributes)
            {
                data.Add((byte)DataType.Structure); //attribute_access_item
                data.Add((byte)3);
                GXCommon.SetData(data, DataType.Int8, att.Index);
                GXCommon.SetData(data, DataType.Enum, att.Access);
                GXCommon.SetData(data, DataType.None, null);
            }
            data.Add((byte)DataType.Array);
            data.Add((byte)item.MethodAttributes.Count);
            foreach (GXDLMSAttributeSettings it in item.MethodAttributes)
            {
                data.Add((byte)DataType.Structure); //attribute_access_item
                data.Add((byte)2);
                GXCommon.SetData(data, DataType.Int8, it.Index);
                GXCommon.SetData(data, DataType.Enum, it.MethodAccess);
            }
        }

        override public DataType GetDataType(int index)
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

        object IGXDLMSBase.GetValue(int index, int selector, object parameters)
        {
            if (ObjectList == null)
            {
                ObjectList = new GXDLMSObjectCollection();
            }
            if (index == 1)
            {
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            else if (index == 2)
            {
                int cnt = ObjectList.Count;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
                //Add count            
                GXCommon.SetObjectCount(cnt, data);
                if (cnt != 0)
                {
                    foreach (GXDLMSObject it in ObjectList)
                    {
                        data.Add((byte)DataType.Structure);
                        data.Add((byte)4); //Count
                        GXCommon.SetData(data, DataType.UInt16, it.ShortName); //base address.
                        GXCommon.SetData(data, DataType.UInt16, it.ObjectType); //ClassID
                        GXCommon.SetData(data, DataType.UInt8, 0); //Version
                        GXCommon.SetData(data, DataType.OctetString, it.LogicalName); //LN
                    }
                    if (ObjectList.FindBySN(this.ShortName) == null)
                    {
                        data.Add((byte)DataType.Structure);
                        data.Add((byte)4); //Count
                        GXCommon.SetData(data, DataType.UInt16, this.ShortName); //base address.
                        GXCommon.SetData(data, DataType.UInt16, this.ObjectType); //ClassID
                        GXCommon.SetData(data, DataType.UInt8, 0); //Version
                        GXCommon.SetData(data, DataType.OctetString, this.LogicalName); //LN
                    }
                }
                return data.ToArray();
            }
            else if (index == 3)
            {
                bool lnExists = ObjectList.FindBySN(this.ShortName) != null;
                //Add count        
                int cnt = ObjectList.Count;
                if (!lnExists)
                {
                    ++cnt;
                }
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
                GXCommon.SetObjectCount(cnt, data);
                foreach (GXDLMSObject it in ObjectList)
                {
                    GetAccessRights(it, data);
                }
                if (!lnExists)
                {
                    GetAccessRights(this, data);
                }
                return data.ToArray();
            }
            else if (index == 4)
            {
                List<byte> data = new List<byte>();
                GXCommon.SetData(data, DataType.OctetString, SecuritySetupReference);
                return data.ToArray();
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
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

        void IGXDLMSBase.SetValue(int index, object value, bool raw)
        {
            if (index == 1)
            {
                if (value is string)
                {
                    LogicalName = value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
                }                
            }
            else if (index == 2)
            {
                ObjectList.Clear();
                if (value != null)
                {
                    foreach (Object[] item in (Object[])value)
                    {
                        ushort sn = (ushort)(Convert.ToInt32(item[0]) & 0xFFFF);
                        ObjectType type = (ObjectType)Convert.ToInt32(item[1]);
                        int version = Convert.ToInt32(item[2]);
                        String ln = GXDLMSObject.toLogicalName((byte[])item[3]);
                        GXDLMSObject obj = Gurux.DLMS.GXDLMSClient.CreateObject(type);
                        if (obj != null)
                        {
                            obj.LogicalName = ln;
                            obj.ShortName = sn;
                            obj.Version = version;
                            ObjectList.Add(obj);
                        }
                    }
                }
            }
            else if (index == 3)
            {
                if (value == null)
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
                    UpdateAccessRights((Object[])value);
                }
            }
            else if (index == 4)
            {
                if (value is string)
                {
                    SecuritySetupReference = value.ToString();
                }
                else
                {
                    SecuritySetupReference = GXDLMSClient.ChangeType(value as byte[], DataType.OctetString).ToString();
                }
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
        #endregion
    }
}
