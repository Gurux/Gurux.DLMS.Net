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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{

    public class GXDLMSArrayManagerItem
    {
        /// <summary>
        /// Array object ID.
        /// </summary>
        public byte Id
        {
            get;
            set;
        }

        /// <summary>
        /// Array objects.
        /// </summary>
        public GXDLMSTargetObject Element { get; set; }
    }

    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSArrayManager
    /// </summary>
    public class GXDLMSArrayManager : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSArrayManager()
        : this("0.0.18.0.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSArrayManager(string ln)
        : this(ln, 0)
        {
            Elements = new List<GXDLMSArrayManagerItem>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSArrayManager(string ln, ushort sn)
        : base(ObjectType.ArrayManager, ln, sn)
        {
            Elements = new List<GXDLMSArrayManagerItem>();
        }

        [XmlArray("Objects")]
        public List<GXDLMSArrayManagerItem> Elements
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Elements };
        }

        #region IGXDLMSBase Members

        /// <summary>
        /// Returns the number of entries in the array identified.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="id">Array identified.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] NumberOfEntries(GXDLMSClient client, byte id)
        {
            return client.Method(this, 1, id);
        }

        /// <summary>
        /// Parse the number of entries from the meter reply.
        /// </summary>
        /// <param name="reply">Meter reply.</param>
        /// <returns>Number of entries.</returns>
        public int ParseNumberOfEntries(byte[] reply)
        {
            return ParseNumberOfEntries(new GXByteBuffer(reply));
        }

        /// <summary>
        /// Parse the number of entries from the meter reply.
        /// </summary>
        /// <param name="reply">Meter reply.</param>
        /// <returns>Number of entries.</returns>
        public int ParseNumberOfEntries(GXByteBuffer reply)
        {
            GXDataInfo info = new GXDataInfo();
            return Convert.ToInt32(GXCommon.GetData(null, reply, info));
        }

        /// <summary>
        /// Returns entries from the given range.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="id">Array identified.</param>
        /// <param name="from">From index.</param>
        /// <param name="to">To index.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] RetrieveEntries(GXDLMSClient client, byte id, UInt16 from, UInt16 to)
        {
            GXByteBuffer reply = new GXByteBuffer();
            reply.SetUInt8(DataType.Structure);
            reply.SetUInt8(2);
            GXCommon.SetData(null, reply, DataType.UInt8, id);
            reply.SetUInt8(DataType.Structure);
            reply.SetUInt8(2);
            GXCommon.SetData(null, reply, DataType.UInt16, from);
            GXCommon.SetData(null, reply, DataType.UInt16, to);
            return client.Method(this, 2, reply.Array(), DataType.Structure);
        }

        /// <summary>
        /// Parse the number of entries from the meter reply.
        /// </summary>
        /// <param name="reply">Meter reply.</param>
        /// <returns>Number of entries.</returns>
        public List<object> ParseEntries(GXByteBuffer reply)
        {
            GXDataInfo info = new GXDataInfo();
            return (List<object>)GXCommon.GetData(null, reply, info);
        }

        /// <summary>
        /// Insert a new entry.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="id">Array identified.</param>
        /// <param name="index">One based entry index number.</param>
        /// <param name="entry">Inserted entry.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] InsertEntry(GXDLMSClient client, byte id, UInt16 index, object entry)
        {
            GXByteBuffer reply = new GXByteBuffer();
            reply.SetUInt8(DataType.Structure);
            reply.SetUInt8(2);
            GXCommon.SetData(null, reply, DataType.UInt8, id);
            reply.SetUInt8(DataType.Structure);
            reply.SetUInt8(2);
            GXCommon.SetData(null, reply, DataType.UInt16, index);
            GXCommon.SetData(null, reply, GXCommon.GetDLMSDataType(entry.GetType()), entry);
            return client.Method(this, 3, reply.Array(), DataType.Structure);
        }

        /// <summary>
        /// Update entry.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="id">Array identified.</param>
        /// <param name="index">One based entry index number.</param>
        /// <param name="entry">Inserted entry.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] UpdateEntry(GXDLMSClient client, byte id, UInt16 index, object entry)
        {
            GXByteBuffer reply = new GXByteBuffer();
            reply.SetUInt8(DataType.Structure);
            reply.SetUInt8(2);
            GXCommon.SetData(null, reply, DataType.UInt8, id);
            reply.SetUInt8(DataType.Structure);
            reply.SetUInt8(2);
            GXCommon.SetData(null, reply, DataType.UInt16, index);
            GXCommon.SetData(null, reply, GXCommon.GetDLMSDataType(entry.GetType()), entry);
            return client.Method(this, 4, reply.Array(), DataType.Structure);
        }

        /// <summary>
        /// Remove entries from the given range.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="id">Array identified.</param>
        /// <param name="from">From index.</param>
        /// <param name="to">To index.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] RemoveEntries(GXDLMSClient client, byte id, UInt16 from, UInt16 to)
        {
            GXByteBuffer reply = new GXByteBuffer();
            reply.SetUInt8(DataType.Structure);
            reply.SetUInt8(2);
            GXCommon.SetData(null, reply, DataType.UInt8, id);
            reply.SetUInt8(DataType.Structure);
            reply.SetUInt8(2);
            GXCommon.SetData(null, reply, DataType.UInt16, from);
            GXCommon.SetData(null, reply, DataType.UInt16, to);
            return client.Method(this, 5, reply.Array(), DataType.Structure);
        }

        private GXByteBuffer GetData(GXDLMSSettings settings, GXDLMSObject target, int attributeIndex,
            int selector = 0, object parameters = null)
        {
            //Check that data type is array.
            if (target.GetDataType(attributeIndex) == DataType.Array)
            {
                ValueEventArgs arg = new ValueEventArgs(target, attributeIndex, selector, parameters);
                object tmp = ((IGXDLMSBase)target).GetValue(settings, arg);
                if (tmp is byte[] bytes)
                {
                    GXByteBuffer bb = new GXByteBuffer(bytes);
                    //Get data type.
                    if (bb.GetUInt8() == (byte)DataType.Array)
                    {
                        return bb;
                    }
                }
            }
            throw new ArgumentException();
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.ByteArray = true;
            byte[] ret = null;
            GXByteBuffer reply = new GXByteBuffer();
            switch (e.Index)
            {
                case 1:
                    NumberOfEntries(settings, e, (byte)e.Parameters, reply);
                    break;
                case 2:
                    {
                        GXStructure args = (GXStructure)e.Parameters;
                        RetrieveEntries(settings, e, args, reply);
                    }
                    break;
                case 3:
                    {
                        GXStructure args = (GXStructure)e.Parameters;
                        InsertEntry(settings, e, args, reply);
                    }
                    break;
                case 4:
                    {
                        GXStructure args = (GXStructure)e.Parameters;
                        UpdateEntry(settings, e, args, reply);
                    }
                    break;
                case 5:
                    {
                        GXStructure args = (GXStructure)e.Parameters;
                        RemoveEntries(settings, e, args, reply);
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            if (reply.Size != 0)
            {
                ret = reply.Array();
            }
            return ret;
        }

        private void NumberOfEntries(GXDLMSSettings settings, ValueEventArgs e, byte id, GXByteBuffer reply)
        {
            bool found = false;
            foreach (var it in Elements)
            {
                if (it.Id == id)
                {
                    if (it.Element.Target is GXDLMSProfileGeneric pg)
                    {
                        if (it.Element.AttributeIndex == 2)
                        {
                            //Entries in use is returned when buffer size is asked.
                            GXCommon.SetData(settings, reply, DataType.UInt32, pg.EntriesInUse);
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        GXByteBuffer value = GetData(settings, it.Element.Target, it.Element.AttributeIndex);
                        int count = GXCommon.GetObjectCount(value);
                        DataType dt;
                        if (count <= byte.MaxValue)
                        {
                            dt = DataType.UInt8;
                        }
                        else if (count <= UInt16.MaxValue)
                        {
                            dt = DataType.UInt16;
                        }
                        else
                        {
                            dt = DataType.UInt32;
                        }
                        GXCommon.SetData(settings, reply, dt, count);
                        found = true;
                    }
                    break;
                }
            }
            if (!found)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        private void RetrieveEntries(GXDLMSSettings settings, ValueEventArgs e, GXStructure args, GXByteBuffer reply)
        {
            bool found = false;
            byte id = (byte)args[0];
            UInt16 from = (UInt16)((GXStructure)args[1])[0];
            UInt16 to = (UInt16)((GXStructure)args[1])[1];
            foreach (var it in Elements)
            {
                if (it.Id == id && from <= to && from > 0)
                {
                    object parameters = null;
                    if (it.Element.Target is GXDLMSProfileGeneric)
                    {
                        parameters = new object[] { null, null, from, to };
                    }
                    ValueEventArgs arg = new ValueEventArgs(it.Element.Target, it.Element.AttributeIndex, 2, parameters);
                    GXByteBuffer bb = new GXByteBuffer((byte[])((IGXDLMSBase)it.Element.Target).GetValue(settings, arg));
                    GXDataInfo info = new GXDataInfo();
                    GXArray arr = (GXArray)GXCommon.GetData(settings, bb, info);
                    //Change from to zero-based.
                    --from;
                    if (to < arr.Count)
                    {
                        arr.RemoveRange(to, arr.Count - to);
                    }
                    if (from != 0)
                    {
                        arr.RemoveRange(0, from);
                    }
                    GXCommon.SetData(settings, reply, DataType.Array, arr);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        private void InsertEntry(GXDLMSSettings settings, ValueEventArgs e, GXStructure args, GXByteBuffer reply)
        {
            bool found = false;
            byte id = (byte)args[0];
            foreach (var it in Elements)
            {
                if (it.Id == id)
                {
                    UInt16 index = (UInt16)((GXStructure)args[1])[0];
                    object data = ((GXStructure)args[1])[1];
                    ValueEventArgs arg = new ValueEventArgs(it.Element.Target, it.Element.AttributeIndex, 0, null);
                    GXByteBuffer bb = new GXByteBuffer((byte[])((IGXDLMSBase)it.Element.Target).GetValue(settings, arg));
                    GXDataInfo info = new GXDataInfo();
                    GXArray arr = (GXArray)GXCommon.GetData(settings, bb, info);
                    if (index == 0)
                    {
                        arr.Insert(0, data);
                    }
                    else if (index > arr.Count)
                    {
                        arr.Add(data);
                    }
                    else
                    {
                        //Add item after existing entry.
                        arr.Insert(index, data);
                    }
                    arg.Value = arr;
                    ((IGXDLMSBase)it.Element.Target).SetValue(settings, arg);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }
        private void UpdateEntry(GXDLMSSettings settings, ValueEventArgs e, GXStructure args, GXByteBuffer reply)
        {
            bool found = false;
            byte id = (byte)args[0];
            foreach (var it in Elements)
            {
                if (it.Id == id)
                {
                    UInt16 index = (UInt16)((GXStructure)args[1])[0];
                    object data = ((GXStructure)args[1])[1];
                    if (index > 0)
                    {
                        //Change index to zero-based.
                        --index;
                        ValueEventArgs arg = new ValueEventArgs(it.Element.Target, it.Element.AttributeIndex, 0, null);
                        GXByteBuffer bb = new GXByteBuffer((byte[])((IGXDLMSBase)it.Element.Target).GetValue(settings, arg));
                        GXDataInfo info = new GXDataInfo();
                        GXArray arr = (GXArray)GXCommon.GetData(settings, bb, info);
                        if (index < arr.Count)
                        {
                            arr[index] = data;
                            arg.Value = arr;
                            ((IGXDLMSBase)it.Element.Target).SetValue(settings, arg);
                            found = true;
                        }
                    }
                    break;
                }
            }
            if (!found)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        private void RemoveEntries(GXDLMSSettings settings, ValueEventArgs e, GXStructure args, GXByteBuffer reply)
        {
            bool found = false;
            byte id = (byte)args[0];
            UInt16 from = (UInt16)((GXStructure)args[1])[0];
            UInt16 to = (UInt16)((GXStructure)args[1])[1];
            if (from != 0 && from <= to)
            {
                foreach (var it in Elements)
                {
                    if (it.Id == id)
                    {
                        UInt16 index = (UInt16)((GXStructure)args[1])[0];
                        object data = ((GXStructure)args[1])[1];
                        ValueEventArgs arg = new ValueEventArgs(it.Element.Target, it.Element.AttributeIndex, 0, null);
                        GXByteBuffer bb = new GXByteBuffer((byte[])((IGXDLMSBase)it.Element.Target).GetValue(settings, arg));
                        GXDataInfo info = new GXDataInfo();
                        GXArray arr = (GXArray)GXCommon.GetData(settings, bb, info);
                        //Change from to zero-based.
                        --from;
                        arr.RemoveRange(from, to - from);
                        arg.Value = arr;
                        ((IGXDLMSBase)it.Element.Target).SetValue(settings, arg);
                        found = true;
                        break;
                    }
                }
            }
            if (!found)
            {
                e.Error = ErrorCode.ReadWriteDenied;
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
            //Array object list
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Objects" };
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Amount", "Retrieve", "Insert", "Update", "Remove" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 2;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 5;
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.Array;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    {
                        int cnt = Elements == null ? 0 : Elements.Count;
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        //Add count
                        GXCommon.SetObjectCount(cnt, data);
                        if (cnt != 0)
                        {
                            foreach (var it in Elements)
                            {
                                data.SetUInt8((byte)DataType.Structure);
                                data.SetUInt8(2); //Count
                                GXCommon.SetData(settings, data, DataType.UInt8, it.Id);
                                data.SetUInt8((byte)DataType.Structure);
                                data.SetUInt8(3); //Count
                                GXCommon.SetData(settings, data, DataType.UInt16, it.Element.Target.ObjectType);
                                GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(it.Element.Target.LogicalName));
                                GXCommon.SetData(settings, data, DataType.Int8, it.Element.AttributeIndex);
                            }
                        }
                        return data.Array();
                    }
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    Elements.Clear();
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
                            var tmp2 = new GXDLMSArrayManagerItem() { Id = (byte)item[0] };
                            GXStructure a = (GXStructure)item[1];
                            ObjectType ot = (ObjectType)Convert.ToInt32(a[0]);
                            var obj = GXDLMSClient.CreateObject(ot);
                            obj.LogicalName = GXCommon.ToLogicalName(a[1]);
                            sbyte attributeIndex = (sbyte)a[2];
                            tmp2.Element = new GXDLMSTargetObject(obj, attributeIndex);
                            Elements.Add(tmp2);
                        }
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Elements.Clear();
            if (reader.IsStartElement("Elements", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    var item = new GXDLMSArrayManagerItem()
                    {
                        Id = (byte)reader.ReadElementContentAsInt("Id")
                    };
                    if (reader.IsStartElement("Target", true))
                    {
                        ObjectType ot = (ObjectType)reader.ReadElementContentAsInt("Type");
                        var obj = GXDLMSClient.CreateObject(ot);
                        obj.LogicalName = reader.ReadElementContentAsString("LN");
                        int index = reader.ReadElementContentAsInt("Index");
                        item.Element = new GXDLMSTargetObject(obj, index);
                        reader.ReadEndElement("Target");
                    }
                    reader.ReadEndElement("Item");
                    Elements.Add(item);
                }
                reader.ReadEndElement("Elements");
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteStartElement("Elements", 1);
            if (Elements != null)
            {
                foreach (var it in Elements)
                {
                    writer.WriteStartElement("Item", 1);
                    //Some meters are returning time here, not date-time.
                    writer.WriteElementString("Id", it.Id, 1);
                    writer.WriteStartElement("Target", 1);
                    writer.WriteElementString("Type", (UInt16)it.Element.Target.ObjectType, 1);
                    writer.WriteElementString("LN", it.Element.Target.LogicalName, 1);
                    writer.WriteElementString("Index", it.Element.AttributeIndex, 1);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
            //Update element objects after read.
            foreach (var it in Elements)
            {
                if (it.Element != null && it.Element.Target != null)
                {
                    var obj = reader.Objects.FindByLN(it.Element.Target.ObjectType, it.Element.Target.LogicalName);
                    if (obj != null)
                    {
                        it.Element.Target = obj;
                    }
                }
            }
        }
        #endregion
    }
}
