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
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using System.Threading;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSProfileGeneric
    /// </summary>
    public class GXDLMSProfileGeneric : GXDLMSObject, IGXDLMSBase
    {
        private GXDLMSServer server = null;
        private GXProfileGenericUpdater updater = null;
#if WINDOWS_UWP
        private System.Threading.Tasks.Task thread = null;
#else
        private Thread thread = null;
#endif
        private int capturePeriod;

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
            SortMethod = SortMethod.FiFo;
            Version = 1;
            From = DateTime.Now.Date;
            To = DateTime.Now.AddDays(1);
            AccessSelector = AccessRange.Last;
            Buffer = new List<object[]>();
            CaptureObjects = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
        }

        /// <summary>
        /// Client uses this to save how values are access.
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
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
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public object From
        {
            get;
            set;
        }

        /// <summary>
        /// Client uses this to save to which date values are retrieved.
        /// </summary>
        [XmlIgnore()]
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public object To
        {
            get;
            set;
        }

        /// <summary>
        /// Data of profile generic.
        /// </summary>
        [XmlIgnore()]
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
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
            get
            {
                return capturePeriod;
            }
            set
            {
                capturePeriod = value;
                if (server != null)
                {
                    Start(server);
                }
            }
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
        public UInt32 ProfileEntries
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
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Reset(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        /// <summary>
        /// Copies the values of the objects to capture into the buffer by reading each capture object.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Capture(GXDLMSClient client)
        {
            return client.Method(this, 2, (sbyte)0);
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
            lock (this)
            {
                object[] values = new object[CaptureObjects.Count];
                int pos = 0;
                ValueEventArgs[] args = new ValueEventArgs[] { new ValueEventArgs(server, this, 2, 0, null) };
                server.PreGet(args);
                if (!args[0].Handled)
                {
                    foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in CaptureObjects)
                    {
                        values[pos] = it.Key.GetValues()[it.Value.AttributeIndex - 1];
                        ++pos;
                    }
                    lock (Buffer)
                    {
                        //Remove first items if buffer is full.
                        if (ProfileEntries != 0 && ProfileEntries == Buffer.Count)
                        {
                            --EntriesInUse;
                            Buffer.RemoveAt(0);
                        }
                        Buffer.Add(values);
                        ++EntriesInUse;
                    }
                }
                server.PostGet(args);
                server.NotifyAction(args);
                server.NotifyPostAction(args);
            }
        }

        internal override void Start(GXDLMSServer svr)
        {
            server = svr;
            if (CapturePeriod != 0)
            {
                updater = new GXProfileGenericUpdater(server, this);
#if !WINDOWS_UWP
                thread = new Thread(new ThreadStart(updater.UpdateProfileGenericData));
                thread.IsBackground = true;
                thread.Start();
#else
                thread = System.Threading.Tasks.Task.Factory.StartNew(updater.UpdateProfileGenericData);
#endif
            }
        }

        internal override void Stop(GXDLMSServer server)
        {
            if (updater != null)
            {
                updater.Closing.Set();
                if (thread != null)
                {
#if !WINDOWS_UWP
                    thread.Join(10000);
#else
                    thread.Wait(10000);
#endif
                    thread = null;
                }
                updater = null;
            }
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
            if (all || (CaptureObjects.Count == 0 && !base.IsRead(3)))
            {
                attributes.Add(3);
            }
            //CapturePeriod
            if (all || !base.IsRead(4))
            {
                attributes.Add(4);
            }
            //Buffer
            attributes.Add(2);

            //SortMethod
            if (all || !base.IsRead(5))
            {
                attributes.Add(5);
            }
            //SortObject
            if (all || !base.IsRead(6))
            {
                attributes.Add(6);
            }
            //EntriesInUse
            attributes.Add(7);
            //ProfileEntries
            if (all || !base.IsRead(8))
            {
                attributes.Add(8);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Internal.GXCommon.GetLogicalNameString(), "Buffer", "CaptureObjects",
                             "Capture Period", "Sort Method", "Sort Object", "Entries In Use", "Profile Entries"};
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
        /// Returns buffer data.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="e"></param>
        /// <param name="table"></param>
        /// <param name="columns">Columns to get. NULL if not used.</param>
        /// <returns></returns>
        byte[] GetData(GXDLMSSettings settings, ValueEventArgs e, List<object[]> table,
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> columns)
        {
            int pos = 0;
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> cols = columns;
            if (columns == null)
            {
                cols = CaptureObjects;
            }
            DataType[] types = new DataType[cols.Count];
            foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in cols)
            {
                types[pos] = it.Key.GetDataType(it.Value.AttributeIndex);
                ++pos;
            }
            UInt16 columnStart = 1, columnEnd = 0;
            if (e.Selector == 2)
            {
                columnStart = (UInt16)((List<object>)e.Parameters)[2];
                columnEnd = (UInt16)((List<object>)e.Parameters)[3];
            }

            if (columnStart > 1 || columnEnd != 0)
            {
                pos = 1;
                cols = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
                foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in CaptureObjects)
                {
                    if (!(pos < columnStart || pos > columnEnd))
                    {
                        cols.Add(it);
                    }
                    ++pos;
                }
                pos = 0;
            }
            GXByteBuffer data = new GXByteBuffer();
            if (settings.Index == 0)
            {
                data.SetUInt8((byte)DataType.Array);
                if (e.RowEndIndex != 0)
                {
                    GXCommon.SetObjectCount((int)(e.RowEndIndex - e.RowBeginIndex), data);
                }
                else
                {
                    GXCommon.SetObjectCount(table.Count, data);
                }
            }

            foreach (object[] items in table)
            {
                data.SetUInt8((byte)DataType.Structure);
                GXCommon.SetObjectCount(cols.Count, data);
                pos = 0;
                DataType tp;
                foreach (object value in items)
                {
                    if (cols == null || cols.Contains(CaptureObjects[pos]))
                    {
                        tp = types[pos];
                        if (tp == DataType.None)
                        {
                            tp = GXDLMSConverter.GetDLMSDataType(value);
                            types[pos] = tp;
                        }
                        if (value == null)
                        {
                            tp = DataType.None;
                        }
                        GXCommon.SetData(settings, data, tp, value);
                    }
                    ++pos;
                }
                ++settings.Index;
            }
            if (e.RowEndIndex != 0)
            {
                e.RowBeginIndex += (UInt32)table.Count;
            }
            else
            {
                settings.Index = 0;
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
                return GetColumns((List<object>)((List<object>)parameters)[3]);
            }
            else if (selector == 2)
            {
                List<object> arr = (List<object>)parameters;
                int colStart = 1;
                int colCount = 0;
                if (arr.Count > 2)
                {
                    colStart = Convert.ToInt32(arr[2]);
                }
                if (arr.Count > 3)
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
        private List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> GetColumns(List<object> cols)
        {
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> columns = null;
            if (cols != null && cols.Count != 0)
            {
                columns = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
                foreach (List<object> it in cols)
                {
                    ObjectType ot = (ObjectType)Convert.ToInt32(it[0]);
                    String ln = GXCommon.ToLogicalName((byte[])it[1]);
                    short attributeIndex = Convert.ToInt16(it[2]);
                    short dataIndex = Convert.ToInt16(it[3]);
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

        byte[] GetProfileGenericData(GXDLMSSettings settings, ValueEventArgs e)
        {
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> columns = null;
            //If all data is read.
            if (e.Selector == 0 || e.Parameters == null || e.RowEndIndex != 0)
            {
                return GetData(settings, e, Buffer, columns);
            }
            List<object> arr = (List<object>)e.Parameters;
            List<object[]> table = new List<object[]>();
            lock (Buffer)
            {
                if (e.Selector == 1) //Read by range
                {
                    GXDataInfo info = new GXDataInfo();
                    info.Type = DataType.DateTime;
                    DateTime start = ((GXDateTime)GXCommon.GetData(settings, new GXByteBuffer((byte[])arr[1]), info)).Value.LocalDateTime;
                    info.Clear();
                    info.Type = DataType.DateTime;
                    DateTime end = ((GXDateTime)GXCommon.GetData(settings, new GXByteBuffer((byte[])arr[2]), info)).Value.LocalDateTime;
                    if (arr.Count > 3)
                    {
                        columns = GetColumns((List<object>)arr[3]);
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
                else if (e.Selector == 2)//Read by entry.
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
                    if (arr.Count > 2)
                    {
                        colStart = Convert.ToUInt16(arr[2]);
                    }
                    if (arr.Count > 3)
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
            return GetData(settings, e, table, columns);
        }

        /// <summary>
        /// Returns captured objects.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
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
                GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(it.Key.LogicalName));//LN
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
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return GetProfileGenericData(settings, e);
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
                    GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(SortObject.LogicalName)); //LN
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

        private void SetBuffer(GXDLMSSettings settings, ValueEventArgs e)
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
            if (e.Value != null && (e.Value as List<object>).Count != 0)
            {
                int index2 = 0;
                DateTime lastDate = DateTime.MinValue;
                foreach (List<object> t in (e.Value as List<object>))
                {
                    List<object> row = new List<object>();
                    foreach(object it in t)
                    {
                        if (it is GXStructure)
                        {
                            row.Add((List<object>) it);
                        }
                        else if (it is GXArray)
                        {
                            row.AddRange((List<object>)it);
                        }
                        else
                        {
                            row.Add(it);
                        }
                    }
                    if (cols.Count != 0)
                    {
                        if (row.Count != cols.Count)
                        {
                            throw new Exception("The number of columns does not match.");
                        }
                        for (int pos = 0; pos != row.Count; ++pos)
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
                            if (index2 > 0)
                            {
                                type = cols[pos].Key.GetUIDataType(index2);
                            }
                            else
                            {
                                type = DataType.None;
                            }
                            if (row[pos] is GXEnum)
                            {
                                row[pos] = Convert.ToByte(row[pos]);
                            }
                            else if (row[pos] is byte[])
                            {
                                if (type != DataType.None && row[pos] is byte[])
                                {
                                    row[pos] = GXDLMSClient.ChangeType(row[pos] as byte[], type, settings.UseUtc2NormalTime);
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
                            else if (type == DataType.DateTime && row[pos] is UInt32)
                            {
                                row[pos] = GXDateTime.FromUnixTime(((UInt32)row[pos]));
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
                            else if (cols[pos].Key is GXDLMSDemandRegister && (index2 == 2 || index2 == 3))
                            {
                                double scaler = (cols[pos].Key as GXDLMSDemandRegister).Scaler;
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
                            else if (cols[pos].Key is GXDLMSRegister && index2 == 3)
                            {
                                try
                                {
                                    GXDLMSRegister r = new GXDLMSRegister();
                                    ValueEventArgs v = new ValueEventArgs(r, 3, 0, null);
                                    v.Value = row[pos];
                                    (r as IGXDLMSBase).SetValue(null, v);
                                    row[pos] = new object[] { r.Scaler, r.Unit };
                                }
                                catch
                                {
                                    //Skip error
                                }
                            }
                        }
                    }
                    Buffer.Add(row.ToArray());
                }
                EntriesInUse = Buffer.Count;
            }
        }

        /// <summary>
        /// Get capture objects.
        /// </summary>
        /// <param name="array">Received data.</param>
        public static List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> GetCaptureObjects(object[] array)
        {
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> list = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
            List<object> tmp = new List<object>();
            tmp.AddRange(array);
            SetCaptureObjects(null, null, list, tmp);
            return list;
        }

        /// <summary>
        /// Get capture objects.
        /// </summary>
        /// <param name="array">Received data.</param>
        public static List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> GetCaptureObjects(List<object> array)
        {
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> list = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
            SetCaptureObjects(null, null, list, array);
            return list;
        }

        private static void SetCaptureObjects(GXDLMSProfileGeneric parent, GXDLMSSettings settings, List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> list, List<object> array)
        {
            GXDLMSConverter c = null;
            try
            {
                foreach (object it in array)
                {
                    List<object> tmp = it as List<object>;
                    if (tmp.Count != 4)
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
                    int dataIndex = Convert.ToInt16(tmp[3]);
                    GXDLMSObject obj = null;
                    if (settings != null && settings.Objects != null)
                    {
                        obj = settings.Objects.FindByLN(type, ln);
                    }
                    //Create a new instance to avoid circular references.
                    if (obj == null || obj == parent)
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
            catch (Exception)
            {
                list.Clear();
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
                SetBuffer(settings, e);
            }
            else if (e.Index == 3)
            {
                Reset();
                //Clear file
                if (e.Server != null)
                {
                    ValueEventArgs[] list = new ValueEventArgs[] { new ValueEventArgs(this, 1, 0, null) };
                    e.Server.NotifyAction(list);
                    e.Server.NotifyPostAction(list);
                }
                CaptureObjects.Clear();
                if (e.Value != null)
                {
                    SetCaptureObjects(this, settings, CaptureObjects, e.Value as List<object>);
                }
            }
            else if (e.Index == 4)
            {
                //Any write access to one of the attributes will automatically call a reset
                //and this call will propagate to all other profiles capturing this profile.
                if (settings.IsServer)
                {
                    Reset();
                }
                CapturePeriod = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 5)
            {
                //Any write access to one of the attributes will automatically call a reset
                //and this call will propagate to all other profiles capturing this profile.
                if (settings.IsServer)
                {
                    Reset();
                }
                SortMethod = (SortMethod)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 6)
            {
                //Any write access to one of the attributes will automatically call a reset
                //and this call will propagate to all other profiles capturing this profile.
                if (settings.IsServer)
                {
                    Reset();
                }

                if (e.Value is List<object>)
                {
                    List<object> tmp = e.Value as List<object>;
                    if (tmp.Count != 4)
                    {
                        throw new GXDLMSException("Invalid structure format.");
                    }
                    ObjectType type = (ObjectType)Convert.ToInt16(tmp[0]);
                    string ln = GXCommon.ToLogicalName((byte[])tmp[1]);
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
                //Any write access to one of the attributes will automatically call a reset
                //and this call will propagate to all other profiles capturing this profile.
                if (settings.IsServer)
                {
                    Reset();
                }
                ProfileEntries = Convert.ToUInt32(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Buffer.Clear();
            if (reader.IsStartElement("Buffer", true))
            {
                while (reader.IsStartElement("Row", true))
                {
                    List<object> row = new List<object>();
                    while (reader.IsStartElement("Cell", false))
                    {
                        row.Add(reader.ReadElementContentAsObject("Cell", null));
                    }
                    Buffer.Add(row.ToArray());
                }
                reader.ReadEndElement("Buffer");
            }
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
            CapturePeriod = reader.ReadElementContentAsInt("CapturePeriod");
            SortMethod = (SortMethod)reader.ReadElementContentAsInt("SortMethod");
            if (reader.IsStartElement("SortObject", true))
            {
                CapturePeriod = reader.ReadElementContentAsInt("CapturePeriod");
                ObjectType ot = (ObjectType)reader.ReadElementContentAsInt("ObjectType");
                string ln = reader.ReadElementContentAsString("LN");
                SortObject = reader.Objects.FindByLN(ot, ln);
                reader.ReadEndElement("SortObject");
            }
            EntriesInUse = reader.ReadElementContentAsInt("EntriesInUse");
            ProfileEntries = (UInt32)reader.ReadElementContentAsLong("ProfileEntries");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (Buffer != null)
            {
                writer.WriteStartElement("Buffer");
                GXDateTime lastdt = null;
                int add = CapturePeriod;
                //Some meters are returning 0 if capture period is one hour.
                if (add == 0)
                {
                    add = 60;
                }
                foreach (object[] row in Buffer)
                {
                    writer.WriteStartElement("Row");
                    int pos = 0;
                    foreach (object it in row)
                    {
                        //If capture objects is not read.
                        if (CaptureObjects.Count > pos)
                        {
                            GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> c = CaptureObjects[pos];
                            ++pos;
                            if (CaptureObjects != null && c.Key is GXDLMSClock && c.Value.AttributeIndex == 2)
                            {
                                if (it != null)
                                {
                                    lastdt = (c.Key as GXDLMSClock).Time;
                                }
                                else if (lastdt != null)
                                {
                                    lastdt = new GXDateTime(lastdt.Value.AddMinutes(add));
                                    writer.WriteElementObject("Cell", lastdt, false);
                                    continue;
                                }
                                else
                                {
                                    writer.WriteElementObject("Cell", DateTime.MinValue, false);
                                }
                            }
                        }
                        writer.WriteElementObject("Cell", it, false);
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            if (CaptureObjects != null)
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
            writer.WriteElementString("CapturePeriod", CapturePeriod);
            writer.WriteElementString("SortMethod", (int)SortMethod);
            if (SortObject != null)
            {
                writer.WriteStartElement("SortObject");
                writer.WriteElementString("ObjectType", (int)SortObject.ObjectType);
                writer.WriteElementString("LN", SortObject.LogicalName);
                writer.WriteEndElement();//SortObject
            }
            writer.WriteElementString("EntriesInUse", EntriesInUse);
            writer.WriteElementString("ProfileEntries", ProfileEntries);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
