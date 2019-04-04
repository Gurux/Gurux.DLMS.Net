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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    public enum CaptureMethod : byte
    {
        /// <summary>
        /// Data is captured with Capture-method.
        /// </summary>
        Invoke,
        /// <summary>
        /// Data is captured upon reading.
        /// </summary>
        Implicit
    }

    /// <summary>
    /// Online help:
    /// http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCompactData
    /// </summary>
    public class GXDLMSCompactData : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSCompactData()
        : this("0.0.66.0.1.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSCompactData(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSCompactData(string ln, ushort sn)
        : base(ObjectType.CompactData, ln, sn)
        {
            CaptureObjects = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
        }

        /// <summary>
        /// Compact buffer
        /// </summary>
        [XmlIgnore()]
        public byte[] Buffer
        {
            get;
            set;
        }

        /// <summary>
        /// Capture objects.
        /// </summary>
        [XmlIgnore()]
        [XmlArray("CaptureObjects")]
        public List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> CaptureObjects
        {
            get;
            internal set;
        }

        /// <summary>
        /// Template ID.
        /// </summary>
        [XmlIgnore()]
        public byte TemplateId
        {
            get;
            set;
        }

        /// <summary>
        /// Template description.
        /// </summary>
        [XmlIgnore()]
        public byte[] TemplateDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Capture method.
        /// </summary>
        [XmlIgnore()]
        public CaptureMethod CaptureMethod
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Buffer, CaptureObjects, TemplateId, TemplateDescription, CaptureMethod };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                //Reset.
                Reset();
            }
            else if (e.Index == 2)
            {
                //Capture.
                Capture(e.Server);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
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
            //CaptureObjects
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //TemplateDescription
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //Buffer
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //TemplateId
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //CaptureMethod
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Buffer", "CaptureObjects", "TemplateId", "TemplateDescription", "CaptureMethod" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
        }

        public static object[] GetDataTypes(byte[] value)
        {
            if (value == null || value.Length == 0)
            {
                return new object[0];
            }
            List<object> list = new List<object>();
            GXDataInfo info = new GXDataInfo();
            return (object[])GXCommon.GetCompactArray(null, new GXByteBuffer(value), info, true);
        }

        public static object[][] GetData(byte[] columns, byte[] value)
        {
            List<object[]> row = new List<object[]>();
            if (columns == null || columns.Length == 0 ||
                value == null || value.Length == 0)
            {
                return row.ToArray();
            }
            List<DataType> list = new List<DataType>();
            GXDataInfo info = new GXDataInfo();
            GXByteBuffer bb = new GXByteBuffer();
            bb.Set(columns);
            GXCommon.SetObjectCount(value.Length, bb);
            bb.Set(value);
            object[] tmp = (object[])GXCommon.GetCompactArray(null, bb, info, false);
            row.Add((object[])tmp[0]);
            return row.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.OctetString;
                case 3:
                    return DataType.Array;
                case 4:
                    return DataType.UInt8;
                case 5:
                    return DataType.OctetString;
                case 6:
                    return DataType.Enum;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        /// <summary>
        /// Returns captured objects.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <returns></returns>
        byte[] GetCaptureObjects(GXDLMSSettings settings)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Array);
            //Add count
            GXCommon.SetObjectCount(CaptureObjects.Count, data);
            foreach (var it in CaptureObjects)
            {
                data.SetUInt8((byte)DataType.Structure);
                //Count
                data.SetUInt8(4);
                //ClassID
                GXCommon.SetData(settings, data, DataType.UInt16, it.Key.ObjectType);
                //LN
                GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(it.Key.LogicalName));
                //Selected Attribute Index
                GXCommon.SetData(settings, data, DataType.Int8, it.Value.AttributeIndex);
                //Selected Data Index
                GXCommon.SetData(settings, data, DataType.UInt16, it.Value.DataIndex);
            }
            return data.Array();
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return Buffer;
                case 3:
                    return GetCaptureObjects(settings);
                case 4:
                    return TemplateId;
                case 5:
                    return TemplateDescription;
                case 6:
                    return CaptureMethod;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return null;
        }

        private static void SetCaptureObjects(GXDLMSSettings settings, List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> list, object[] array)
        {
            GXDLMSConverter c = null;
            list.Clear();
            try
            {
                if (array != null)
                {
                    foreach (object it in array)
                    {
                        object[] tmp = it as object[];
                        if (tmp.Length != 4)
                        {
                            throw new GXDLMSException("Invalid structure format.");
                        }
                        int v = Convert.ToInt16(tmp[0]);
                        if (Enum.GetName(typeof(ObjectType), v) == null)
                        {
                            list.Clear();
                            return;
                        }
                        ObjectType type = (ObjectType)v;
                        string ln = GXCommon.ToLogicalName((byte[])tmp[1]);
                        int attributeIndex = Convert.ToInt16(tmp[2]);
                        //If profile generic selective access is used.
                        if (attributeIndex < 0)
                        {
                            attributeIndex = 2;
                        }
                        int dataIndex = Convert.ToInt16(tmp[3]);
                        GXDLMSObject obj = null;
                        if (settings != null && settings.Objects != null)
                        {
                            obj = settings.Objects.FindByLN(type, ln);
                        }
                        if (obj == null)
                        {
                            obj = GXDLMSClient.CreateDLMSObject((int)type, null, 0, ln, 0);
                            if (c == null)
                            {
                                c = new GXDLMSConverter();
                            }
                            c.UpdateOBISCodeInformation(obj);
                        }
                        list.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, new GXDLMSCaptureObject(attributeIndex, dataIndex)));
                    }
                }
            }
            catch (Exception)
            {
                list.Clear();
            }
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    if (e.Value is byte[])
                    {
                        Buffer = (byte[])e.Value;
                    }
                    else if (e.Value is string)
                    {
                        Buffer = GXCommon.HexToBytes((string)e.Value);
                    }
                    break;
                case 3:
                    SetCaptureObjects(settings, CaptureObjects, (object[])e.Value);
                    if (settings.IsServer)
                    {
                        UpdateTemplateDescription();
                    }
                    break;
                case 4:
                    TemplateId = (byte)e.Value;
                    break;
                case 5:
                    TemplateDescription = (byte[])e.Value;
                    break;
                case 6:
                    CaptureMethod = (CaptureMethod)Convert.ToByte(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Buffer = GXCommon.HexToBytes(reader.ReadElementContentAsString("Buffer"));
            CaptureObjects.Clear();
            if (reader.IsStartElement("CaptureObjects", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    ObjectType ot = (ObjectType)reader.ReadElementContentAsInt("ObjectType");
                    string ln = reader.ReadElementContentAsString("LN");
                    int ai = reader.ReadElementContentAsInt("Attribute");
                    int di = reader.ReadElementContentAsInt("Data");
                    GXDLMSCaptureObject co = new GXDLMSCaptureObject(ai, di);
                    GXDLMSObject obj = reader.Objects.FindByLN(ot, ln);
                    if (obj == null)
                    {
                        obj = GXDLMSClient.CreateObject(ot);
                        obj.LogicalName = ln;
                    }
                    CaptureObjects.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, co));
                }
                reader.ReadEndElement("CaptureObjects");
            }
            TemplateId = (byte)reader.ReadElementContentAsInt("TemplateId");
            TemplateDescription = GXCommon.HexToBytes(reader.ReadElementContentAsString("TemplateDescription"));
            CaptureMethod = (CaptureMethod)reader.ReadElementContentAsInt("CaptureMethod");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Buffer", GXCommon.ToHex(Buffer, false));
            if (CaptureObjects.Count != 0)
            {
                writer.WriteStartElement("CaptureObjects");
                foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in CaptureObjects)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("ObjectType", (int)it.Key.ObjectType);
                    writer.WriteElementString("LN", it.Key.LogicalName);
                    writer.WriteElementString("Attribute", it.Value.AttributeIndex);
                    writer.WriteElementString("Data", it.Value.DataIndex);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteElementString("TemplateId", TemplateId);
            writer.WriteElementString("TemplateDescription", GXCommon.ToHex(TemplateDescription, false));
            writer.WriteElementString("CaptureMethod", (int)CaptureMethod);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void Reset()
        {
            lock (this)
            {
                Buffer = null;
            }
        }

        /// <summary>
        /// Copies the values of the objects to capture
        /// into the buffer by reading capture objects.
        /// </summary>
        public void Capture(GXDLMSServer server)
        {
            lock (this)
            {
                GXByteBuffer bb = new GXByteBuffer();
                ValueEventArgs[] args = new ValueEventArgs[] { new ValueEventArgs(server, this, 2, 0, null) };
                Buffer = null;
                server.PreGet(args);
                if (!args[0].Handled)
                {
                    foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in CaptureObjects)
                    {
                        ValueEventArgs e = new ValueEventArgs(server, it.Key, it.Value.AttributeIndex, 0, null);
                        object value = (it.Key as IGXDLMSBase).GetValue(server.Settings, e);
                        if (value is byte[])
                        {
                            bb.Set((byte[])value);
                        }
                        else
                        {
                            GXByteBuffer tmp = new GXByteBuffer();
                            GXCommon.SetData(server.Settings, tmp, (it.Key as IGXDLMSBase).GetDataType(it.Value.AttributeIndex), value);
                            //If data is empty.
                            if (tmp.Size == 1)
                            {
                                bb.SetUInt8(0);
                            }
                            else
                            {
                                tmp.Position = 1;
                                bb.Set(tmp);
                            }
                        }
                    }
                    Buffer = bb.Array();
                }
                server.PostGet(args);
                server.NotifyAction(args);
                server.NotifyPostAction(args);
            }
        }

        /// <summary>
        /// Update template description.
        /// </summary>
        public void UpdateTemplateDescription()
        {
            lock (this)
            {
                GXByteBuffer bb = new GXByteBuffer();
                Buffer = null;
                bb.SetUInt8((byte) DataType.Structure);
                GXCommon.SetObjectCount(CaptureObjects.Count, bb);
                foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in CaptureObjects)
                {
                    DataType type = (it.Key as IGXDLMSBase).GetDataType(it.Value.AttributeIndex);
                    if (type == DataType.Array || type == DataType.Structure)
                    {
                        object val = it.Key.GetValues()[it.Value.DataIndex - 1];
                        bb.SetUInt8(GXCommon.GetDLMSDataType(val.GetType()));
                    }
                    else
                    {
                        bb.SetUInt8((it.Key as IGXDLMSBase).GetDataType(it.Value.AttributeIndex));
                    }
                }
                TemplateDescription = bb.Array();
            }
        }
    }
}
