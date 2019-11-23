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
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSRegisterActivation
    /// </summary>
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

        /// <summary>
        /// Add new register.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="script">Register to add.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] AddRegister(GXDLMSClient client, GXDLMSObject target)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            GXCommon.SetData(null, bb, DataType.UInt16, target.ObjectType);
            GXCommon.SetData(null, bb, DataType.OctetString, GXCommon.LogicalNameToBytes(target.LogicalName));
            return client.Method(this, 1, bb.Array(), DataType.Array);
        }

        /// <summary>
        /// Add new register activation mask.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="name">Register activation mask name.</param>
        /// <param name="indexes">Register activation indexes.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] AddMask(GXDLMSClient client, byte[] name, byte[] indexes)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            GXCommon.SetData(null, bb, DataType.OctetString, name);
            bb.SetUInt8(DataType.Array);
            bb.SetUInt8((byte)indexes.Length);
            foreach (byte it in indexes)
            {
                GXCommon.SetData(null, bb, DataType.UInt8, it);
            }
            return client.Method(this, 2, bb.Array(), DataType.Array);
        }

        /// <summary>
        /// Remove register activation mask.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="name">Register activation mask name.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] RemoveMask(GXDLMSClient client, byte[] name)
        {
            return client.Method(this, 3, name);
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //RegisterAssignment
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            //MaskList
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //ActiveMask
            if (all || !base.IsRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Register Assignment", "Mask List", "Active Mask" };
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
                return GXCommon.LogicalNameToBytes(LogicalName);
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
                        GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(it.LogicalName));
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
                LogicalName = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 2)
            {
                List<GXDLMSObjectDefinition> items = new List<GXDLMSObjectDefinition>();
                if (e.Value != null)
                {
                    foreach (object tmp in (IEnumerable<object>)e.Value)
                    {
                        List<object> it;
                        if (tmp is List<object>)
                        {
                            it = (List<object>)tmp;
                        }
                        else
                        {
                            it = new List<object>((object[])tmp);
                        }
                        GXDLMSObjectDefinition item = new GXDLMSObjectDefinition();
                        item.ObjectType = (ObjectType)Convert.ToInt32(it[0]);
                        item.LogicalName = GXCommon.ToLogicalName((byte[])it[1]);
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
                    foreach (object tmp in (IEnumerable<object>)e.Value)
                    {
                        List<object> it;
                        if (tmp is List<object>)
                        {
                            it = (List<object>)tmp;
                        }
                        else
                        {
                            it = new List<object>((object[])tmp);
                        }
                        List<byte> index_list = new List<byte>();
                        foreach (byte b in (IEnumerable<object>)it[1])
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

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            List<GXDLMSObjectDefinition> list = new List<GXDLMSObjectDefinition>();
            if (reader.IsStartElement("RegisterAssignment", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSObjectDefinition it = new GXDLMSObjectDefinition();
                    it.ObjectType = (ObjectType)reader.ReadElementContentAsInt("ObjectType");
                    it.LogicalName = reader.ReadElementContentAsString("LN");
                    list.Add(it);
                }
                reader.ReadEndElement("RegisterAssignment");
            }
            RegisterAssignment = list.ToArray();
            MaskList.Clear();
            if (reader.IsStartElement("MaskList", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    byte[] mask = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("Mask"));
                    byte[] i = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("Index"));
                    MaskList.Add(new KeyValuePair<byte[], byte[]>(mask, i));
                }
                reader.ReadEndElement("MaskList");
            }
            ActiveMask = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("ActiveMask"));
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (RegisterAssignment != null)
            {
                writer.WriteStartElement("RegisterAssignment");
                foreach (GXDLMSObjectDefinition it in RegisterAssignment)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("ObjectType", (int)it.ObjectType);
                    writer.WriteElementString("LN", it.LogicalName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (MaskList != null)
            {
                writer.WriteStartElement("MaskList");
                foreach (KeyValuePair<byte[], byte[]> it in MaskList)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Mask", GXDLMSTranslator.ToHex(it.Key));
                    writer.WriteElementString("Index", GXDLMSTranslator.ToHex(it.Value).Replace(" ", ";"));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            if (ActiveMask != null)
            {
                writer.WriteElementString("ActiveMask", GXDLMSTranslator.ToHex(ActiveMask));
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
