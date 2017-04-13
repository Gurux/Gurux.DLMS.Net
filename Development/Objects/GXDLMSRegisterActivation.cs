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
using System.Text;
using System.ComponentModel;
using Gurux.DLMS;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSRegisterActivation : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSRegisterActivation()
        : this(null, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSRegisterActivation(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSRegisterActivation(string ln, ushort sn)
        : base(ObjectType.RegisterActivation, ln, sn)
        {
            MaskList = new List<KeyValuePair<byte[], byte[]>>();
        }

        /// <summary>
        ///Assignment list.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSObjectDefinition[] RegisterAssignment
        {
            get;
            set;
        }

        /// <summary>
        /// Mask list.
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<byte[], byte[]>> MaskList
        {
            get;
            set;
        }

        /// <summary>
        /// Active mask.
        /// </summary>
        [XmlIgnore()]
        public byte[] ActiveMask
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, RegisterAssignment, MaskList, ActiveMask };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //RegisterAssignment
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }
            //MaskList
            if (!base.IsRead(3))
            {
                attributes.Add(3);
            }
            //ActiveMask
            if (!base.IsRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Register Assignment", "Mask List", "Active Mask" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 4;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 3;
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
                return DataType.Array;
            }
            if (index == 4)
            {
                return DataType.OctetString;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return this.LogicalName;
            }
            if (e.Index == 2)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (RegisterAssignment == null)
                {
                    data.SetUInt8(0);
                }
                else
                {
                    data.SetUInt8((byte)RegisterAssignment.Length);
                    foreach (GXDLMSObjectDefinition it in RegisterAssignment)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(2);
                        GXCommon.SetData(settings, data, DataType.UInt16, it.ObjectType);
                        GXCommon.SetData(settings, data, DataType.OctetString, it.LogicalName);
                    }
                }
                return data.Array();
            }
            if (e.Index == 3)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (MaskList == null)
                {
                    data.SetUInt8(0);
                }
                else
                {
                    data.SetUInt8((byte)MaskList.Count);
                    foreach (KeyValuePair<byte[], byte[]> it in MaskList)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(2);
                        GXCommon.SetData(settings, data, DataType.OctetString, it.Key);
                        data.SetUInt8((byte)DataType.Array);
                        data.SetUInt8((byte)it.Value.Length);
                        foreach (byte b in it.Value)
                        {
                            GXCommon.SetData(settings, data, DataType.UInt8, b);
                        }
                    }
                }
                return data.Array();
            }
            if (e.Index == 4)
            {
                return ActiveMask;
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                if (e.Value is string)
                {
                    LogicalName = e.Value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString).ToString();
                }
            }
            else if (e.Index == 2)
            {
                List<GXDLMSObjectDefinition> items = new List<GXDLMSObjectDefinition>();
                if (e.Value != null)
                {
                    foreach (Object[] it in (Object[])e.Value)
                    {
                        GXDLMSObjectDefinition item = new GXDLMSObjectDefinition();
                        item.ObjectType = (ObjectType)Convert.ToInt32(it[0]);
                        item.LogicalName = GXDLMSObject.ToLogicalName((byte[])it[1]);
                        items.Add(item);
                    }
                }
                RegisterAssignment = items.ToArray();
            }
            else if (e.Index == 3)
            {
                MaskList.Clear();
                if (e.Value != null)
                {
                    foreach (Object[] it in (Object[])e.Value)
                    {
                        List<byte> index_list = new List<byte>();
                        foreach (byte b in (Object[])it[1])
                        {
                            index_list.Add(b);
                        }
                        MaskList.Add(new KeyValuePair<byte[], byte[]>((byte[])it[0], index_list.ToArray()));
                    }
                }
            }
            else if (e.Index == 4)
            {
                if (e.Value == null)
                {
                    ActiveMask = null;
                }
                else
                {
                    ActiveMask = (byte[])e.Value;
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }
        #endregion
    }
}
