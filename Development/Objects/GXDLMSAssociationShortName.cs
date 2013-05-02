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
        [GXDLMSAttribute(2, Static = true)]
        public GXDLMSObjectCollection ObjectList
        {
            get;
            internal set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(3, Static = true)]
        public object AccessRightsList
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(4, Static = true)]
        public object SecuritySetupReference
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, ObjectList, AccessRightsList, SecuritySetupReference };
        }

        #region IGXDLMSBase Members

        void IGXDLMSBase.Invoke(int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
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

        object IGXDLMSBase.GetValue(int index, out DataType type, byte[] parameters)
        {
            if (ObjectList == null)
            {
                ObjectList = new GXDLMSObjectCollection();
            }
            if (index == 1)
            {
                type = DataType.OctetString;
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            else if (index == 2)
            {
                type = DataType.Array;
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
                type = DataType.Array;
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

        override public DataType GetDataType(int index)
        {
            if (index == 2 || index == 3)
            {
                return DataType.Array;
            }
            return base.GetDataType(index);
        }

        void IGXDLMSBase.SetValue(int index, object value)
        {
            if (index == 1)
            {
                LogicalName = Convert.ToString(value);
            }
            else if (index == 2)
            {
                ObjectList.Clear();
                if (value != null)
                {
                    foreach (Object[] item in (Object[])value)
                    {
                        ushort sn = Convert.ToUInt16(item[0]);
                        ObjectType type = (ObjectType)Convert.ToInt32(item[1]);
                        int version = Convert.ToInt32(item[2]);
                        String ln = GXDLMSObject.toLogicalName((byte[])item[3]);
                        GXDLMSObject obj = Gurux.DLMS.GXDLMSClient.CreateObject(type);
                        obj.LogicalName = ln;
                        obj.ShortName = sn;
                        obj.Version = version;
                        ObjectList.Add(obj);
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
                if (value != null)
                {
                    throw new ArgumentException("SetValue failed. Invalid attribute index.");
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
