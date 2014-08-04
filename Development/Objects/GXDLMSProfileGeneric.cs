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
using System.Data;
using Gurux.DLMS;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    [Serializable]
    public struct GXKeyValuePair<K, V>
    {
        private K m_Key;
        private V m_Value;

        public K Key
        {
            get
            {
                return m_Key;
            }
            set
            {
                m_Key = value;
            }
        }
        public V Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public GXKeyValuePair(K key, V value)
        {
            m_Key = key;
            m_Value = value;
        }
    }

    public enum AccessRange
    {
        /// <summary>
        /// Read emtries.
        /// </summary>
        Entry,
        /// <summary>
        /// Read last N days.
        /// </summary>
        Last,
        /// <summary>
        /// Read between days
        /// </summary>
        Range
    };

    /// <summary>
    /// Sort methods.
    /// </summary>
    public enum SortMethod
    {
        /// <summary>
        /// First in first out
        /// </summary>        
        /// <remarks>
        /// When circle buffer is full first item is removed.
        /// </remarks>
        FiFo = 0,
        /// <summary>
        /// Last in first out.
        /// </summary>
        /// <remarks>
        /// When circle buffer is full last item is removed.
        /// </remarks>
        LiFo,
        /// <summary>
        /// Largest is first.
        /// </summary>
        Largest,
        /// <summary>
        /// Smallest is first.
        /// </summary>
        Smallest,
        /// <summary>
        /// Nearst to zero is first.
        /// </summary>
        NearestToZero,
        /// <summary>
        /// Farest from zero is first.
        /// </summary>
        FarestFromZero
    }

    public class GXDLMSCaptureObject
    {
        /// <summary>
        /// Attribute Index of DLMS object in the profile generic table.
        /// </summary>
        public int AttributeIndex
        {
            get;
            set;
        }
        
        /// <summary>
        /// Data index of DLMS object in the profile generic table. 
        /// </summary>
        public int DataIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GXDLMSCaptureObject()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="attributeIndex">Attribute index.</param>
        /// <param name="dataIndex">Data Index.</param>
        public GXDLMSCaptureObject(int attributeIndex, int dataIndex)
        {
            AttributeIndex = attributeIndex;
            DataIndex = dataIndex;
        }
    }

    public class GXDLMSProfileGeneric : GXDLMSObject, IGXDLMSBase
    {
        object Owner
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.Parent;
                }
                return null;
            }
        }
        
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
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSProfileGeneric(string ln)
            : this(ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSProfileGeneric(string ln, ushort sn)
            : base(ObjectType.ProfileGeneric, ln, 0)
        {
            From = DateTime.Now.Date;
            To = DateTime.Now.AddDays(1);
            AccessSelector = AccessRange.Last;
            Buffer = new List<object[]>();
            CaptureObjects = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
        }

        /// <inheritdoc cref="GXDLMSObject.UpdateDefaultValueItems"/>
        public override void UpdateDefaultValueItems()
        {
            GXDLMSAttributeSettings att = this.Attributes.Find(5);
            if (att == null)
            {
                att = new GXDLMSAttribute(5);
                Attributes.Add(att);
            }
            att.Values.Add(new GXObisValueItem(SortMethod.FiFo, "FIFO"));
            att.Values.Add(new GXObisValueItem(SortMethod.LiFo, "LIFO"));
            att.Values.Add(new GXObisValueItem(SortMethod.Largest, "Largest"));
            att.Values.Add(new GXObisValueItem(SortMethod.Smallest, "Smallest"));
            att.Values.Add(new GXObisValueItem(SortMethod.NearestToZero, "Nearest To Zero"));
            att.Values.Add(new GXObisValueItem(SortMethod.FarestFromZero, "Farest from Zero"));
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
                SortObject, EntriesInUse, ProfileEntries };
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
            if (!(Owner is GXDLMSServerBase))
            {
                throw new Exception("This functionality is available only in server side.");
            }
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
        public void Capture()
        {
            if (!(Owner is GXDLMSServerBase))
            {
                throw new Exception("This functionality is available only in server side.");
            }
            object[] values = new object[CaptureObjects.Count];
            int pos = -1;
            foreach (var obj in CaptureObjects)
            {
                ValueEventArgs e = new ValueEventArgs(obj.Key, obj.Value.AttributeIndex, 0);
                (Owner as GXDLMSServerBase).Read(e);
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

        byte[][] IGXDLMSBase.Invoke(object sender, int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
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
                "Capture Period", "Buffer", "Sort Method", "Sort Object", "Entries In Use", "Profile Entries"};            
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 8;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

        /// <summary>
        /// Returns Association View.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        byte[] GetData(List<object[]> table)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)DataType.Array);
            GXCommon.SetObjectCount(table.Count, data);
            foreach (object[] items in table)
            {
                data.Add((byte)DataType.Structure);
                GXCommon.SetObjectCount(items.Length, data);
                foreach (object value in items)
                {
                    Gurux.DLMS.DataType tp = Gurux.DLMS.Internal.GXCommon.GetValueType(value);
                    GXCommon.SetData(data, tp, value);
                }
            }
            return data.ToArray();
        }             

        byte[] GetProfileGenericData(int selector, object parameters)
        {                        
            //If all data is readed.
            if (selector == 0 || parameters == null)
            {
                return GetData(Buffer);
            }
            object[] arr = (object[])parameters;
            List<object[]> table = new List<object[]>();
            lock (Buffer)
            {
                if (selector == 1) //Read by range
                {
                    DataType dt = DataType.DateTime;
                    int a, b, c = 0, pos = 0;
                    GXDateTime start = (GXDateTime)GXCommon.GetData((byte[])arr[1], ref pos, ActionType.None, out a, out b, ref dt, ref c);
                    c = pos = 0;
                    GXDateTime end = (GXDateTime)GXCommon.GetData((byte[])arr[2], ref pos, ActionType.None, out a, out b, ref dt, ref c);
                    foreach (object[] row in Buffer)
                    {
                        DateTime tm = Convert.ToDateTime(row[0]);
                        if (tm >= start.Value && tm <= end.Value)
                        {
                            table.Add(row);
                        }
                    }
                }
                else if (selector == 2)//Read by entry.
                {
                    int start = Convert.ToInt32(arr[0]);
                    int count = Convert.ToInt32(arr[1]);
                    for (int pos = 0; pos < count; ++pos)
                    {
                        if (pos + start == Buffer.Count)
                        {
                            break;
                        }
                        table.Add(Buffer[start + pos]);
                    }
                }
                else
                {
                    throw new Exception("Invalid selector.");
                }
            }
            return GetData(table);
        }

        /// <summary>
        /// Returns captured objects.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        byte[] GetColumns()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)DataType.Array);
            //Add count
            GXCommon.SetObjectCount(CaptureObjects.Count, data);
            foreach (var it in CaptureObjects)
            {
                data.Add((byte)DataType.Structure);
                data.Add(4);//Count    
                GXCommon.SetData(data, DataType.UInt16, it.Key.ObjectType);//ClassID
                GXCommon.SetData(data, DataType.OctetString, it.Key.LogicalName);//LN
                GXCommon.SetData(data, DataType.Int8, it.Value.AttributeIndex); //Selected Attribute Index
                GXCommon.SetData(data, DataType.UInt16, it.Value.DataIndex); //Selected Data Index                
            }
            return data.ToArray();
        }

        override public DataType GetDataType(int index)
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
                return DataType.Int8;
            }
            if (index == 5)
            {
                return DataType.Int8;                
            }
            if (index == 6)
            {
                return DataType.Array;                
            }
            if (index == 7)
            {
                return DataType.Int8;                
            }
            if (index == 8)
            {
                return DataType.Int8;                
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(int index, int selector, object parameters)
        {
            if (index == 1)
            {
                return this.LogicalName;
            }
            if (index == 2)
            {
                return GetProfileGenericData(selector, parameters);   
            }
            if (index == 3)
            {
                return GetColumns();
            }
            if (index == 4)
            {
                return CapturePeriod;
            }
            if (index == 5)
            {
                return SortMethod;
            }
            if (index == 6)
            {                
                List<byte> data = new List<byte>();            
                data.Add((byte)DataType.Structure);
                data.Add((byte) 4); //Count  
                if (SortObject == null)
                {
                    GXCommon.SetData(data, DataType.UInt16, 0); //ClassID
                    GXCommon.SetData(data, DataType.OctetString, new byte[6]); //LN
                    GXCommon.SetData(data, DataType.Int8, 0); //Selected Attribute Index
                    GXCommon.SetData(data, DataType.UInt16, 0); //Selected Data Index
                }
                else
                {
                    GXCommon.SetData(data, DataType.UInt16, SortObject.ObjectType); //ClassID
                    GXCommon.SetData(data, DataType.OctetString, SortObject.LogicalName); //LN
                    GXCommon.SetData(data, DataType.Int8, SortAttributeIndex); //Selected Attribute Index
                    GXCommon.SetData(data, DataType.UInt16, SortDataIndex); //Selected Data Index
                }            
                return data.ToArray();
            }
            if (index == 7)
            {
                return EntriesInUse;
            }
            if (index == 8)
            {
                return ProfileEntries;
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value)
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
                if (CaptureObjects == null || CaptureObjects.Count == 0)
                {
                    throw new Exception("Read capture objects first.");
                }
                if (value != null && (value as object[]).Length != 0)
                {
                    DateTime lastDate = DateTime.MinValue;
                    foreach (object[] row in (value as object[]))
                    {
                        if ((row as object[]).Length != CaptureObjects.Count)
                        {
                            throw new Exception("Number of columns do not match.");
                        }
                        for (int pos = 0; pos != row.Length; ++pos)
                        {
                            DataType type = CaptureObjects[pos].Key.GetUIDataType(CaptureObjects[pos].Value.AttributeIndex);
                            if (row[pos] is byte[])
                            {
                                if (type != DataType.None && row[pos] is byte[])
                                {
                                    row[pos] = GXDLMSClient.ChangeType(row[pos] as byte[], type);
                                    if (row[pos] is GXDateTime)
                                    {
                                        GXDateTime dt = (GXDateTime)row[pos];
                                        lastDate = dt.Value;
                                    }
                                }
                            }
                            else if (type == DataType.DateTime && row[pos] == null && CapturePeriod != 0)
                            {
                                if (lastDate == DateTime.MinValue && Buffer.Count != 0)
                                {
                                    lastDate = ((GXDateTime)Buffer[Buffer.Count - 1].GetValue(pos)).Value;
                                }
                                if (lastDate != DateTime.MinValue)
                                {
                                    lastDate = lastDate.AddSeconds(CapturePeriod);
                                    row[pos] = new GXDateTime(lastDate);
                                }
                            }
                            if (CaptureObjects[pos].Key is GXDLMSRegister && CaptureObjects[pos].Value.AttributeIndex == 2)
                            {
                                double scaler = (CaptureObjects[pos].Key as GXDLMSRegister).Scaler;
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
            else if (index == 3)
            {
                Buffer.Clear();
                EntriesInUse = 0;
                CaptureObjects.Clear();
                GXDLMSObjectCollection objects = new GXDLMSObjectCollection();
                foreach (object it in value as object[])
                {
                    object[] tmp = it as object[];
                    if (tmp.Length != 4)
                    {
                        throw new GXDLMSException("Invalid structure format.");
                    }
                    ObjectType type = (ObjectType)Convert.ToInt16(tmp[0]);
                    string ln = GXDLMSObject.toLogicalName((byte[]) tmp[1]);
                    int attributeIndex = Convert.ToInt16(tmp[2]);
                    int dataIndex = Convert.ToInt16(tmp[3]);
                    GXDLMSObject obj = null;
                    if (Parent != null)
                    {
                        obj = Parent.FindByLN(type, ln);
                    }
                    if (obj == null)
                    {
                        obj = GXDLMSClient.CreateDLMSObject((int)type, null, 0, ln, 0);
                    }
                    CaptureObjects.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, new GXDLMSCaptureObject(attributeIndex, dataIndex)));
                    objects.Add(obj);
                }
                GXDLMSClient.UpdateOBISCodes(objects);
            }
            else if (index == 4)
            {
                CapturePeriod = Convert.ToInt32(value);
            }
            else if (index == 5)
            {
                SortMethod = (SortMethod)Convert.ToInt32(value);
            }
            else if (index == 6)
            {
                if (value != null)
                {
                    object[] tmp = value as object[];
                    if (tmp.Length != 4)
                    {
                        throw new GXDLMSException("Invalid structure format.");
                    }
                    ObjectType type = (ObjectType)Convert.ToInt16(tmp[0]);
                    string ln = GXDLMSObject.toLogicalName((byte[])tmp[1]);
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
                }
                else
                {
                    SortObject = null;
                }
            }
            else if (index == 7)
            {
                EntriesInUse = Convert.ToInt32(value);
            }
            else if (index == 8)
            {
                ProfileEntries = Convert.ToInt32(value);
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
        #endregion
    }
}
