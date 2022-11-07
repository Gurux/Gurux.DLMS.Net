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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCompactData
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
            Version = 1;
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

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] {"Reset", "Capture" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
        }

        public static List<object> GetDataTypes(byte[] value)
        {
            if (value == null || value.Length == 0)
            {
                return new List<object>();
            }
            GXDataInfo info = new GXDataInfo();
            object ret = GXCommon.GetCompactArray(null, new GXByteBuffer(value), info, true);
            return ((List<object>)ret);
        }

        public static List<object> GetData(byte[] columns, byte[] value)
        {
            return GetData(columns, value, false);
        }

        public static List<object> GetData(byte[] columns, byte[] value, bool AppendAA)
        {
            if (columns == null || columns.Length == 0 ||
                value == null || value.Length == 0)
            {
                return new GXArray();
            }
            List<DataType> list = new List<DataType>();
            GXDataInfo info = new GXDataInfo();
            info.AppendAA = AppendAA;
            GXByteBuffer bb = new GXByteBuffer();
            bb.Set(columns);
            GXCommon.SetObjectCount(value.Length, bb);
            bb.Set(value);
            List<object> tmp = (List<object>)GXCommon.GetCompactArray(null, bb, info, false);
            return tmp;
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

        private void SetCaptureObjects(GXDLMSSettings settings, List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> list, IEnumerable<object> array)
        {
            GXDLMSConverter c = null;
            list.Clear();
            try
            {
                if (array != null)
                {
                    foreach (object tmp in array)
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
                        if (Version == 0 && it.Count != 4)
                        {
                            throw new GXDLMSException("Invalid structure format.");
                        }
                        else if (Version == 1 && it.Count != 5)
                        {
                            throw new GXDLMSException("Invalid structure format.");
                        }
                        int v = Convert.ToInt16(it[0]);
                        if (Enum.GetName(typeof(ObjectType), v) == null)
                        {
                            list.Clear();
                            return;
                        }
                        ObjectType type = (ObjectType)v;
                        string ln = GXCommon.ToLogicalName((byte[])it[1]);
                        int attributeIndex = Convert.ToInt16(it[2]);
                        //If profile generic selective access is used.
                        if (attributeIndex < 0)
                        {
                            attributeIndex = 2;
                        }
                        int dataIndex = Convert.ToUInt16(it[3]);
                        GXDLMSObject obj = null;
                        if (settings != null && settings.Objects != null)
                        {
                            obj = settings.Objects.FindByLN(type, ln);
                        }
                        if (obj == null)
                        {
                            obj = GXDLMSClient.CreateDLMSObject((int)type, null, 0, ln, 0, 2);
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
            catch (Exception ex)
            {
                list.Clear();
                throw ex;
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
                    SetCaptureObjects(settings, CaptureObjects, (IEnumerable<object>)e.Value);
                    if (settings != null && settings.IsServer)
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
            writer.WriteElementString("Buffer", GXCommon.ToHex(Buffer, false), 2);
            writer.WriteStartElement("CaptureObjects", 3);
            if (CaptureObjects.Count != 0)
            {
                foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in CaptureObjects)
                {
                    writer.WriteStartElement("Item", 3);
                    writer.WriteElementString("ObjectType", (int)it.Key.ObjectType, 3);
                    writer.WriteElementString("LN", it.Key.LogicalName, 3);
                    writer.WriteElementString("Attribute", it.Value.AttributeIndex, 3);
                    writer.WriteElementString("Data", it.Value.DataIndex, 3);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteElementString("TemplateId", TemplateId, 4);
            writer.WriteElementString("TemplateDescription", GXCommon.ToHex(TemplateDescription, false), 5);
            writer.WriteElementString("CaptureMethod", (int)CaptureMethod, 6);
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

        private static void CaptureArray(GXDLMSServer server, GXByteBuffer tmp, GXByteBuffer bb, int index)
        {
            //Skip type.
            tmp.GetUInt8();
            int cnt = GXCommon.GetObjectCount(tmp);
            for (int pos = 0; pos != cnt; ++pos)
            {
                if (index == -1 || index == pos)
                {
                    DataType dt = (DataType)tmp.GetUInt8(tmp.Position);
                    if (dt == DataType.Structure || dt == DataType.Array)
                    {
                        CaptureArray(server, tmp, bb, -1);
                    }
                    else
                    {
                        CaptureValue(server, tmp, bb);
                    }
                    if (index == pos)
                    {
                        break;
                    }
                }
            }
        }

        private static void CaptureValue(GXDLMSServer server, GXByteBuffer tmp, GXByteBuffer bb)
        {
            GXByteBuffer tmp2 = new GXByteBuffer();
            GXDataInfo info = new GXDataInfo();
            object value = GXCommon.GetData(server.Settings, tmp, info);
            GXCommon.SetData(server.Settings, tmp2, info.Type, value);
            //If data is empty.
            if (tmp2.Size == 1)
            {
                bb.SetUInt8(0);
            }
            else
            {
                tmp2.Position = 1;
                bb.Set(tmp2);
            }
        }

        /// <summary>
        /// Copies the values of the objects to capture
        /// into the buffer by reading capture objects.
        /// </summary>
        public void Capture()
        {
            Capture(null);
        }

        /// <summary>
        /// Copies the values of the objects to capture
        /// into the buffer by reading capture objects.
        /// </summary>
        internal void Capture(GXDLMSServer server)
        {
            lock (this)
            {
                GXByteBuffer bb = new GXByteBuffer();
                ValueEventArgs[] args = new ValueEventArgs[] { new ValueEventArgs(server, this, 2, 0, null) };
                Buffer = null;
                if (server != null)
                {
                    server.NotifyPreAction(args);
                    server.PreGet(args);
                }
                if (!args[0].Handled)
                {
                    foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in CaptureObjects)
                    {
                        ValueEventArgs e = new ValueEventArgs(server, it.Key, it.Value.AttributeIndex, 0, null);
                        object value = (it.Key as IGXDLMSBase).GetValue(server == null ? null : server.Settings, e);
                        DataType dt = (it.Key as IGXDLMSBase).GetDataType(it.Value.AttributeIndex);
                        if ((value is byte[] || value is GXByteBuffer[]) && (dt == DataType.Structure || dt == DataType.Array))
                        {
                            GXByteBuffer tmp;
                            if (value is byte[])
                            {
                                tmp = new GXByteBuffer((byte[])value);
                            }
                            else
                            {
                                tmp = (GXByteBuffer)value;
                            }
                            CaptureArray(server, tmp, bb, it.Value.DataIndex - 1);
                        }
                        else
                        {
                            GXByteBuffer tmp = new GXByteBuffer();
                            GXCommon.SetData(server == null ? null : server.Settings, tmp, dt, value);
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
                if (server != null)
                {
                    server.PostGet(args);
                    server.NotifyPostAction(args);
                }
            }
        }

        private static void UpdateTemplateDescription(GXByteBuffer columns, GXByteBuffer data, int index)
        {
            DataType ch = (DataType)data.GetUInt8();
            int count = GXCommon.GetObjectCount(data);
            if (index == -1)
            {
                columns.SetUInt8(ch);
                if (ch == DataType.Array)
                {
                    columns.SetUInt16((UInt16)count);
                }
                else
                {
                    columns.SetUInt8((byte)count);
                }
            }
            GXDataInfo info = new GXDataInfo();
            for (int pos = 0; pos < count; ++pos)
            {
                //If all data is captured.
                if (index == -1 || pos == index)
                {
                    DataType dt = (DataType)data.GetUInt8(data.Position);
                    if (dt == DataType.Array || dt == DataType.Structure)
                    {
                        UpdateTemplateDescription(columns, data, -1);
                        if (ch == DataType.Array)
                        {
                            break;
                        }
                    }
                    else
                    {
                        info.Clear();
                        columns.SetUInt8((byte)dt);
                        //Get data.
                        GXCommon.GetData(null, data, info);
                    }
                    if (index == pos)
                    {
                        break;
                    }
                }
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
                bb.SetUInt8((byte)DataType.Structure);
                GXCommon.SetObjectCount(CaptureObjects.Count, bb);
                foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in CaptureObjects)
                {
                    DataType dt = (it.Key as IGXDLMSBase).GetDataType(it.Value.AttributeIndex);
                    if (dt == DataType.Array || dt == DataType.Structure)
                    {
                        ValueEventArgs e = new ValueEventArgs(null, it.Value.AttributeIndex, 0, null);
                        GXByteBuffer data = new GXByteBuffer();
                        object v = ((IGXDLMSBase)it.Key).GetValue(null, e);
                        if (v is byte[])
                        {
                            data.Set((byte[])v);
                        }
                        else
                        {
                            data = (GXByteBuffer)v;
                        }
                        UpdateTemplateDescription(bb, data, ((GXDLMSCaptureObject)it.Value).DataIndex - 1);
                    }
                    else
                    {
                        bb.SetUInt8(dt);
                    }
                }
                TemplateDescription = bb.Array();
            }
        }


        /// <summary>
        /// Convert compact data buffer to array of values.
        /// </summary>
        /// <param name="templateDescription">Template description byte array.</param>
        /// <param name="buffer">Buffer byte array.</param>
        /// <returns>Values from byte buffer.</returns>
        public List<object> GetValues(byte[] templateDescription, byte[] buffer)
        {
            //If templateDescription or buffer is not given.
            if (templateDescription == null || buffer == null || templateDescription.Length == 0 || buffer.Length == 0)
            {
                throw new ArgumentException();
            }
            GXDataInfo info = new GXDataInfo();
            object tmp;
            GXByteBuffer data = new GXByteBuffer();
            data.Set(templateDescription);
            GXCommon.SetObjectCount(buffer.Length, data);
            data.Set(buffer);
            info.Type = DataType.CompactArray;
            tmp = GXCommon.GetData(null, data, info);
            return (List<object>)tmp;
        }
    }
}
