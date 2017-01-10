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
using Gurux.DLMS;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSProfileGeneric : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSProfileGeneric()
        : this(null, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSProfileGeneric(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSProfileGeneric(string ln, ushort sn)
        : base(ObjectType.ProfileGeneric, ln, sn)
        {
            From = DateTime.Now.Date;
            To = DateTime.Now.AddDays(1);
            AccessSelector = AccessRange.Last;
            Buffer = new List<object[]>();
            CaptureObjects = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
        }

        /// <summary>
        /// Client uses this to save how values are access.
        /// </summary>
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore()]
        public AccessRange AccessSelector
        {
            get;
            set;
        }

        /// <summary>
        /// Client uses this to save from which date values are retrieved.
        /// </summary>
        [XmlIgnore()]
        [Browsable(false)]
        public object From
        {
            get;
            set;
        }

        /// <summary>
        /// Client uses this to save to which date values are retrieved.
        /// </summary>
        [XmlIgnore()]
        [Browsable(false)]
        public object To
        {
            get;
            set;
        }

        /// <summary>
        /// Data of profile generic.
        /// </summary>
        [XmlIgnore()]
        [Browsable(false)]
        public List<object[]> Buffer
        {
            get;
            set;
        }

        [XmlArray("CaptureObjects")]
        public List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> CaptureObjects
        {
            get;
            set;
        }

        /// <summary>
        /// How often values are captured.
        /// </summary>
        [XmlIgnore()]
        public int CapturePeriod
        {
            get;
            set;
        }

        /// <summary>
        /// How columns are sorted.
        /// </summary>
        [XmlIgnore()]
        [DefaultValue(SortMethod.FiFo)]
        public SortMethod SortMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Column that is used for sorting.
        /// </summary>
        [XmlIgnore()]
        [DefaultValue(null)]
        public GXDLMSObject SortObject
        {
            get;
            set;
        }

        /// <summary>
        /// Sort object's attribute index.
        /// </summary>
        [XmlIgnore()]
        [DefaultValue(0)]
        public int SortAttributeIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Sort object's data index.
        /// </summary>
        [XmlIgnore()]
        [DefaultValue(0)]
        public int SortDataIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Entries (rows) in Use.
        /// </summary>
        [XmlIgnore()]
        public int EntriesInUse
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum Entries (rows) count.
        /// </summary>
        [XmlIgnore()]
        public int ProfileEntries
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Buffer.ToArray(), CaptureObjects,
                              CapturePeriod, SortMethod,
                              SortObject, EntriesInUse, ProfileEntries
                            };
        }

        /// <summary>
        /// Get captured objects.
        /// </summary>
        /// <returns></returns>
        public GXDLMSObject[] GetCaptureObject()
        {
            List<GXDLMSObject> list = new List<GXDLMSObject>();
            foreach (var it in CaptureObjects)
            {
                list.Add(it.Key);
            }
            return list.ToArray();
        }


        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void Reset()
        {
            lock (this)
            {
                Buffer.Clear();
                EntriesInUse = 0;
            }
        }

        /// <summary>
        /// Copies the values of the objects to capture
        /// into the buffer by reading capture objects.
        /// </summary>
        public void Capture(GXDLMSServer server)
        {
            object[] values = new object[CaptureObjects.Count];
            int pos = -1;
            foreach (var obj in CaptureObjects)
            {
                ValueEventArgs e = new ValueEventArgs(server.Settings, obj.Key, obj.Value.AttributeIndex, 0, null);
                server.Update(UpdateType.ProfileGeneric, e);
                if (e.Handled)
                {
                    values[++pos] = e.Value;
                }
                else
                {
                    values[++pos] = obj.Key.GetValues()[obj.Value.AttributeIndex - 1];
                }
            }
            lock (Buffer)
            {
                //Remove first items if buffer is full.
                if (ProfileEntries == Buffer.Count)
                {
                    Buffer.RemoveAt(0);
                }
                Buffer.Add(values);
                EntriesInUse = Buffer.Count;
            }
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
            //CaptureObjects
            if (CaptureObjects.Count == 0 && !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //CapturePeriod
            if (!base.IsRead(4))
            {
                attributes.Add(4);
            }
            //Buffer
            attributes.Add(2);

            //SortMethod
            if (!base.IsRead(5))
            {
                attributes.Add(5);
            }
            //SortObject
            if (!base.IsRead(6))
            {
                attributes.Add(6);
            }
            //EntriesInUse
            attributes.Add(7);
            //ProfileEntries
            if (!base.IsRead(8))
            {
                attributes.Add(8);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Gurux.DLMS.Properties.Resources.LogicalNameTxt, "CaptureObjects",
                             "Capture Period", "Buffer", "Sort Method", "Sort Object", "Entries In Use", "Profile Entries"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 8;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
        }

        /// <summary>
        /// Returns Association View.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns">Columns to get. NULL if not used.</param>
        /// <returns></returns>
        byte[] GetData(GXDLMSSettings settings, List<object[]> table, List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> columns)
        {
            int pos;
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Array);
            GXCommon.SetObjectCount(table.Count, data);
            foreach (object[] items in table)
            {
                data.SetUInt8((byte)DataType.Structure);
                if (columns == null || columns.Count == 0)
                {
                    GXCommon.SetObjectCount(items.Length, data);
                }
                else
                {
                    GXCommon.SetObjectCount(columns.Count, data);
                }
                pos = 0;
                foreach (object value in items)
                {
                    if (columns == null || columns.Contains(CaptureObjects[pos]))
                    {
                        DataType tp = Gurux.DLMS.Internal.GXCommon.GetValueType(value);
                        GXCommon.SetData(settings, data, tp, value);
                    }
                    ++pos;
                }
            }
            return data.Array();
        }

        /// <summary>
        /// Get selected columns from parameters.
        /// </summary>
        /// <param name="selector">Is read by entry or range.</param>
        /// <param name="parameters">Received parameters where columns information is found.</param>
        /// <returns>Selected columns.</returns>
        public List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> GetSelectedColumns(int selector, Object parameters)
        {
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> columns = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
            if (selector == 0)
            {
                // Return all rows.
                columns.AddRange(CaptureObjects);
                return columns;
            }
            else if (selector == 1)
            {
                return GetColumns((Object[])((Object[])parameters)[3]);
            }
            else if (selector == 2)
            {
                Object[] arr = (Object[])parameters;
                int colStart = 1;
                int colCount = 0;
                if (arr.Length > 2)
                {
                    colStart = Convert.ToInt32(arr[2]);
                }
                if (arr.Length > 3)
                {
                    colCount = Convert.ToInt32(arr[3]);
                }
                else if (colStart != 1)
                {
                    colCount = CaptureObjects.Count;
                }
                if (colStart != 1 || colCount != 0)
                {
                    for (int pos = 0; pos != colCount; ++pos)
                    {
                        columns.Add(CaptureObjects[colStart + pos - 1]);
                    }
                }
                else
                {
                    // Return all rows.
                    columns.AddRange(CaptureObjects);
                }
                return columns;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid selector.");
            }
        }

        /// <summary>
        /// Get selected (filtered) columns.
        /// </summary>
        /// <param name="cols">Selected columns.</param>
        /// <returns>Selected columns.</returns>
        private List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> GetColumns(object[] cols)
        {
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> columns = null;
            if (cols != null && cols.Length != 0)
            {
                columns = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
                foreach (object it in cols)
                {
                    Object[] tmp = (Object[])it;
                    ObjectType ot = (ObjectType)Convert.ToInt32(tmp[0]);
                    String ln = GXDLMSObject.ToLogicalName((byte[])tmp[1]);
                    short attributeIndex = Convert.ToInt16(tmp[2]);
                    short dataIndex = Convert.ToInt16(tmp[3]);
                    // Find columns and update only them.
                    foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> c in CaptureObjects)
                    {
                        if (c.Key.ObjectType == ot
                                && c.Value.AttributeIndex == attributeIndex
                                && c.Value.DataIndex == dataIndex
                                && c.Key.LogicalName.CompareTo(ln) == 0)
                        {
                            columns.Add(c);
                            break;
                        }
                    }
                }
            }
            return columns;
        }

        byte[] GetProfileGenericData(GXDLMSSettings settings, int selector, object parameters)
        {
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> columns = null;
            //If all data is read.
            if (selector == 0 || parameters == null)
            {
                return GetData(settings, Buffer, columns);
            }
            object[] arr = (object[])parameters;
            List<object[]> table = new List<object[]>();
            lock (Buffer)
            {
                if (selector == 1) //Read by range
                {
                    GXDataInfo info = new GXDataInfo();
                    info.Type = DataType.DateTime;
                    DateTime start = ((GXDateTime)GXCommon.GetData(settings, new GXByteBuffer((byte[])arr[1]), info)).Value.LocalDateTime;
                    info.Clear();
                    info.Type = DataType.DateTime;
                    DateTime end = ((GXDateTime)GXCommon.GetData(settings, new GXByteBuffer((byte[])arr[2]), info)).Value.LocalDateTime;
                    if (arr.Length > 3)
                    {
                        columns = GetColumns((Object[])((Object[])arr)[3]);
                    }
                    foreach (object[] row in Buffer)
                    {
                        DateTime tm = Convert.ToDateTime(row[0]);
                        if (tm >= start && tm <= end)
                        {
                            table.Add(row);
                        }
                    }
                }
                else if (selector == 2)//Read by entry.
                {
                    int start = Convert.ToInt32(arr[0]);
                    if (start == 0)
                    {
                        start = 1;
                    }
                    int count = Convert.ToInt32(arr[1]);
                    if (count == 0)
                    {
                        count = Buffer.Count;
                    }
                    if (start + count > Buffer.Count + 1)
                    {
                        count = Buffer.Count;
                    }

                    int colStart = 1;
                    int colCount = 0;
                    if (arr.Length > 2)
                    {
                        colStart = Convert.ToUInt16(arr[2]);
                    }
                    if (arr.Length > 3)
                    {
                        colCount = Convert.ToUInt16(arr[3]);
                    }
                    else if (colStart != 1)
                    {
                        colCount = CaptureObjects.Count;
                    }
                    if (colStart != 1 || colCount != 0)
                    {
                        columns = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
                        for (int pos = 0; pos != colCount; ++pos)
                        {
                            columns.Add(CaptureObjects[colStart + pos - 1]);
                        }
                    }

                    //Get rows.
                    // Starting index is 1.
                    for (int pos = 0; pos < count; ++pos)
                    {
                        if (pos + start - 1 == Buffer.Count)
                        {
                            break;
                        }
                        table.Add(Buffer[start + pos - 1]);
                    }
                }
                else
                {
                    throw new Exception("Invalid selector.");
                }
            }
            return GetData(settings, table, columns);
        }

        /// <summary>
        /// Returns captured objects.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        byte[] GetColumns(GXDLMSSettings settings)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Array);
            //Add count
            GXCommon.SetObjectCount(CaptureObjects.Count, data);
            foreach (var it in CaptureObjects)
            {
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(4);//Count
                GXCommon.SetData(settings, data, DataType.UInt16, it.Key.ObjectType);//ClassID
                GXCommon.SetData(settings, data, DataType.OctetString, it.Key.LogicalName);//LN
                GXCommon.SetData(settings, data, DataType.Int8, it.Value.AttributeIndex); //Selected Attribute Index
                GXCommon.SetData(settings, data, DataType.UInt16, it.Value.DataIndex); //Selected Data Index
            }
            return data.Array();
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
                return DataType.UInt32;
            }
            if (index == 5)
            {
                return DataType.Enum;
            }
            if (index == 6)
            {
                return DataType.Array;
            }
            if (index == 7)
            {
                return DataType.UInt32;
            }
            if (index == 8)
            {
                return DataType.UInt32;
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
                return GetProfileGenericData(settings, e.Selector, e.Parameters);
            }
            if (e.Index == 3)
            {
                return GetColumns(settings);
            }
            if (e.Index == 4)
            {
                return CapturePeriod;
            }
            if (e.Index == 5)
            {
                return SortMethod;
            }
            if (e.Index == 6)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8((byte)4); //Count
                if (SortObject == null)
                {
                    GXCommon.SetData(settings, data, DataType.UInt16, 0); //ClassID
                    GXCommon.SetData(settings, data, DataType.OctetString, new byte[6]); //LN
                    GXCommon.SetData(settings, data, DataType.Int8, 0); //Selected Attribute Index
                    GXCommon.SetData(settings, data, DataType.UInt16, 0); //Selected Data Index
                }
                else
                {
                    GXCommon.SetData(settings, data, DataType.UInt16, SortObject.ObjectType); //ClassID
                    GXCommon.SetData(settings, data, DataType.OctetString, SortObject.LogicalName); //LN
                    GXCommon.SetData(settings, data, DataType.Int8, SortAttributeIndex); //Selected Attribute Index
                    GXCommon.SetData(settings, data, DataType.UInt16, SortDataIndex); //Selected Data Index
                }
                return data.Array();
            }
            if (e.Index == 7)
            {
                return EntriesInUse;
            }
            if (e.Index == 8)
            {
                return ProfileEntries;
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        private void SetBuffer(ValueEventArgs e)
        {
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> cols = null;
            if (e.Parameters is List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>)
            {
                cols = (List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>)e.Parameters;
            }
            if (cols == null)
            {
                cols = CaptureObjects;
            }
            //Mikko Buffer.Clear();
            if (e.Value != null && (e.Value as object[]).Length != 0)
            {
                int index2 = 0;
                DateTime lastDate = DateTime.MinValue;
                foreach (object[] row in (e.Value as object[]))
                {
                    if ((row as object[]).Length != cols.Count)
                    {
                        throw new Exception("Number of columns do not match.");
                    }
                    for (int pos = 0; pos != row.Length; ++pos)
                    {
                        if (cols == null)
                        {
                            index2 = 0;
                        }
                        else
                        {
                            index2 = cols[pos].Value.AttributeIndex;
                        }
                        DataType type;
                        //Actaris SL 7000 and ACE 6000 returns 0.
                        if (index2 != 0)
                        {
                            type = cols[pos].Key.GetUIDataType(index2);
                        }
                        else
                        {
                            type = DataType.None;
                        }
                        if (row[pos] is byte[])
                        {
                            if (type != DataType.None && row[pos] is byte[])
                            {
                                row[pos] = GXDLMSClient.ChangeType(row[pos] as byte[], type);
                                if (row[pos] is GXDateTime)
                                {
                                    GXDateTime dt = (GXDateTime)row[pos];
                                    lastDate = dt.Value.LocalDateTime;
                                }
                            }
                        }
                        else if (type == DataType.DateTime && row[pos] == null && CapturePeriod != 0)
                        {
                            if (lastDate == DateTime.MinValue && Buffer.Count != 0)
                            {
                                lastDate = ((GXDateTime)Buffer[Buffer.Count - 1].GetValue(pos)).Value.LocalDateTime;
                            }
                            if (lastDate != DateTime.MinValue)
                            {
                                lastDate = lastDate.AddSeconds(CapturePeriod);
                                row[pos] = new GXDateTime(lastDate);
                            }
                        }
                        if (cols[pos].Key is GXDLMSRegister && index2 == 2)
                        {
                            double scaler = (cols[pos].Key as GXDLMSRegister).Scaler;
                            if (scaler != 1)
                            {
                                try
                                {
                                    row[pos] = Convert.ToDouble(row[pos]) * scaler;
                                }
                                catch
                                {
                                    //Skip error
                                }
                            }
                        }
                    }
                    Buffer.Add(row);
                }
                EntriesInUse = Buffer.Count;
            }
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
                SetBuffer(e);
            }
            else if (e.Index == 3)
            {
                Buffer.Clear();
                EntriesInUse = 0;
                CaptureObjects.Clear();
                GXDLMSObjectCollection objects = new GXDLMSObjectCollection();
                if (e.Value != null)
                {
                    foreach (object it in e.Value as object[])
                    {
                        object[] tmp = it as object[];
                        if (tmp.Length != 4)
                        {
                            throw new GXDLMSException("Invalid structure format.");
                        }
                        ObjectType type = (ObjectType)Convert.ToInt16(tmp[0]);
                        string ln = GXDLMSObject.ToLogicalName((byte[])tmp[1]);
                        int attributeIndex = Convert.ToInt16(tmp[2]);
                        int dataIndex = Convert.ToInt16(tmp[3]);
                        GXDLMSObject obj = null;
                        if (settings != null && settings.Objects != null)
                        {
                            obj = settings.Objects.FindByLN(type, ln);
                        }
                        if (obj == null)
                        {
                            obj = GXDLMSClient.CreateDLMSObject((int)type, null, 0, ln, 0);
                        }
                        CaptureObjects.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, new GXDLMSCaptureObject(attributeIndex, dataIndex)));
                        objects.Add(obj);
                    }
                }
            }
            else if (e.Index == 4)
            {
                CapturePeriod = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 5)
            {
                SortMethod = (SortMethod)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 6)
            {
                if (e.Value != null)
                {
                    object[] tmp = e.Value as object[];
                    if (tmp.Length != 4)
                    {
                        throw new GXDLMSException("Invalid structure format.");
                    }
                    ObjectType type = (ObjectType)Convert.ToInt16(tmp[0]);
                    string ln = GXDLMSObject.ToLogicalName((byte[])tmp[1]);
                    SortAttributeIndex = Convert.ToInt16(tmp[2]);
                    SortDataIndex = Convert.ToInt16(tmp[3]);
                    SortObject = null;
                    foreach (var it in CaptureObjects)
                    {
                        if (it.Key.ObjectType == type && it.Key.LogicalName == ln)
                        {
                            SortObject = it.Key;
                            break;
                        }
                    }
                    if (SortObject == null)
                    {
                        SortObject = GXDLMSClient.CreateObject(type);
                        SortObject.LogicalName = ln;
                    }
                }
                else
                {
                    SortObject = null;
                }
            }
            else if (e.Index == 7)
            {
                EntriesInUse = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 8)
            {
                ProfileEntries = Convert.ToInt32(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }
        #endregion
    }
}
