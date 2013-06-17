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

    public class GXDLMSProfileGeneric : GXDLMSObject, IGXDLMSBase
    {
        internal GXDLMSServerBase Server;
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
            Buffer = new DataTable();
            CaptureObjects = new GXDLMSObjectCollection();
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
        [GXDLMSAttribute(2, Access = AccessMode.Read, Type=DataType.Array)]
        public DataTable Buffer
        {
            get;
            set;
        }

        /// <summary>
        /// Captured Objects.
        /// </summary>
        [GXDLMSAttribute(3, Static = true, Access = AccessMode.Read, Order = 6)]
        [XmlArray("Columns")]
        public GXDLMSObjectCollection CaptureObjects
        {
            get;
            set;
        }

        /// <summary>
        /// How often values are captured.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(4, Static = true, Access = AccessMode.Read, Order = 5)]
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
        [GXDLMSAttribute(5, Static = true, Access = AccessMode.Read, Order = 4)]
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
        [GXDLMSAttribute(6, Static = true, Access = AccessMode.Read, Order = 3)]
        public GXDLMSObject SortObject
        {
            get;
            set;
        }

        /// <summary>
        /// Entries (rows) in Use.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(7, Access = AccessMode.Read, Order = 1)]
        public int EntriesInUse
        {
            get
            {
                return Buffer.Rows.Count;
            }
        }    

        /// <summary>
        /// Maximum Entries (rows) count.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(8, Static = true, Access = AccessMode.Read, Order = 2)]
        public int ProfileEntries
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, Buffer, CaptureObjects, 
                CapturePeriod, SortMethod, 
                SortObject, EntriesInUse, ProfileEntries };
        }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void Reset()
        {
            if (Server == null)
            {
                throw new Exception("This functionality is available only in server side.");
            }
            lock (this)
            {
                Buffer.Clear();
            }
        }

        /// <summary>
        /// Copies the values of the objects to capture 
        /// into the buffer by reading capture objects.
        /// </summary>
        public void Capture()
        {
            if (Server == null)
            {
                throw new Exception("This functionality is available only in server side.");
            }
            object[] values = new object[CaptureObjects.Count];
            int pos = -1;
            foreach (GXDLMSObject obj in CaptureObjects)
            {
                ValueEventArgs e = new ValueEventArgs(obj, obj.SelectedAttributeIndex);
                Server.Read(e);
                if (e.Handled)
                {
                    values[++pos] = e.Value;
                }
                else
                {
                    values[++pos] = obj.GetValues()[obj.SelectedAttributeIndex - 1];
                }
            }
            lock (Buffer)
            {
                //Remove first items if buffer is full.
                if (ProfileEntries == Buffer.Rows.Count)
                {
                    Buffer.Rows.RemoveAt(0);
                }
                Buffer.Rows.Add(values);
            }
        }

        #region IGXDLMSBase Members

        void IGXDLMSBase.Invoke(int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
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
        byte[] GetData(DataTable table)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)DataType.Array);
            GXCommon.SetObjectCount(table.Rows.Count, data);
            foreach (DataRow row in table.Rows)
            {
                object[] items = row.ItemArray;
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

        /// <summary>
        /// Returns Association View.
        /// </summary>
        /// <param name="sortObject">Object to used in sort.</param>
        /// <returns></returns>
        byte[] GetSortObject(GXDLMSObject sortObject)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)DataType.Structure);
            data.Add(4);//Count    
            if (sortObject == null)
            {
                GXCommon.SetData(data, DataType.UInt16, 0);//ClassID
                GXCommon.SetData(data, DataType.OctetString, new byte[6]);//LN
                GXCommon.SetData(data, DataType.Int8, 0); //Selected Attribute Index
                GXCommon.SetData(data, DataType.UInt16, 0); //Selected Data Index                
            }
            else
            {
                GXCommon.SetData(data, DataType.UInt16, sortObject.ObjectType);//ClassID
                GXCommon.SetData(data, DataType.OctetString, sortObject.LogicalName);//LN
                GXCommon.SetData(data, DataType.Int8, sortObject.SelectedAttributeIndex); //Selected Attribute Index
                GXCommon.SetData(data, DataType.UInt16, sortObject.SelectedDataIndex); //Selected Data Index                
            }
            return data.ToArray();
        }

        /// <summary>
        /// Returns Association View.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        byte[] GetColumns(GXDLMSObjectCollection items)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)DataType.Array);
            //Add count
            GXCommon.SetObjectCount(items.Count, data);
            foreach (GXDLMSObject it in items)
            {
                data.Add((byte)DataType.Structure);
                data.Add(4);//Count    
                GXCommon.SetData(data, DataType.UInt16, it.ObjectType);//ClassID
                GXCommon.SetData(data, DataType.OctetString, it.LogicalName);//LN
                GXCommon.SetData(data, DataType.Int8, it.SelectedAttributeIndex); //Selected Attribute Index
                GXCommon.SetData(data, DataType.UInt16, it.SelectedDataIndex); //Selected Data Index                
            }
            return data.ToArray();
        }

        byte[] GetProfileGenericData(byte[] data)
        {
            int selector;
            object from, to;
            //If all data is readed.
            if (data == null || data.Length == 0)
            {
                return GetData(Buffer);
            }
            GetAccessSelector(data, out selector, out from, out to);
            DataTable table;
            lock (Buffer)
            {
                table = Buffer.Clone();
                if (selector == 1) //Read by range
                {
                    GXDateTime start = (GXDateTime)from;
                    GXDateTime end = (GXDateTime)to;
                    foreach (DataRow row in Buffer.Rows)
                    {
                        DateTime tm = Convert.ToDateTime(row[0]);
                        if (tm >= start.Value && tm <= end.Value)
                        {
                            table.Rows.Add(row.ItemArray);
                        }
                    }
                }
                else if (selector == 2)//Read by entry.
                {
                    int start = Convert.ToInt32(from);
                    int count = Convert.ToInt32(to);
                    for (int pos = 0; pos < count; ++pos)
                    {
                        if (pos + start == Buffer.Rows.Count)
                        {
                            break;
                        }
                        table.Rows.Add(Buffer.Rows[start + pos].ItemArray);
                    }
                }
            }
            return GetData(table);
        }

        void GetAccessSelector(byte[] data, out int selector, out object start, out object to)
        {
            selector = data[0];
            int pos;
            DataType type = DataType.None;
            //Start index
            int index = 0, count = 0, cachePos = 0;
            if (selector == 1)//Read by range
            {
                if (data[1] != (int)DataType.Structure ||
                    data[2] != 4 ||
                    data[3] != (int)DataType.Structure ||
                    data[4] != 4)
                {
                    throw new GXDLMSException("Invalid parameter");
                }
                pos = 5;
                object classId = GXCommon.GetData(data, ref pos, ActionType.None, out count, out index, ref type, ref cachePos);
                type = DataType.None;
                object ln = GXCommon.GetData(data, ref pos, ActionType.None, out count, out index, ref type, ref cachePos);
                type = DataType.None;
                object attributeIndex = GXCommon.GetData(data, ref pos, ActionType.None, out count, out index, ref type, ref cachePos);
                type = DataType.None;
                object version = GXCommon.GetData(data, ref pos, ActionType.None, out count, out index, ref type, ref cachePos);
                type = DataType.None;
                byte[] tmp = GXCommon.GetData(data, ref pos, ActionType.None, out count, out index, ref type, ref cachePos) as byte[];
                start = GXDLMSClient.ChangeType(tmp, DataType.DateTime);
                type = DataType.None;
                tmp = GXCommon.GetData(data, ref pos, ActionType.None, out count, out index, ref type, ref cachePos) as byte[];
                to = GXDLMSClient.ChangeType(tmp, DataType.DateTime);
            }
            else if (selector == 2)//Read by entry.
            {
                if (data[1] != (int)DataType.Structure ||
                    data[2] != 4)
                {
                    throw new GXDLMSException("Invalid parameter");
                }
                pos = 3;
                start = GXCommon.GetData(data, ref pos, ActionType.None, out count, out index, ref type, ref cachePos);
                type = DataType.None;
                to = GXCommon.GetData(data, ref pos, ActionType.None, out count, out index, ref type, ref cachePos);
                if (Convert.ToUInt32(start) > Convert.ToUInt32(to))
                {
                    throw new GXDLMSException("Invalid parameter");
                }
            }
            else
            {
                throw new GXDLMSException("Invalid parameter");
            }
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
            foreach (GXDLMSObject it in CaptureObjects)
            {
                data.Add((byte)DataType.Structure);
                data.Add(4);//Count    
                GXCommon.SetData(data, DataType.UInt16, it.ObjectType);//ClassID
                GXCommon.SetData(data, DataType.OctetString, it.LogicalName);//LN
                GXCommon.SetData(data, DataType.Int8, it.SelectedAttributeIndex); //Selected Attribute Index
                GXCommon.SetData(data, DataType.UInt16, it.SelectedDataIndex); //Selected Data Index                
            }
            return data.ToArray();
        }

        object IGXDLMSBase.GetValue(int index, out DataType type, byte[] parameters)
        {
            if (index == 1)
            {
                type = DataType.OctetString;
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                type = DataType.Array;
                return GetProfileGenericData(parameters);   
            }
            if (index == 3)
            {
                type = DataType.Array;
                return GetColumns();
            }
            if (index == 4)
            {
                type = DataType.Int8;
                return CapturePeriod;
            }
            if (index == 5)
            {
                type = DataType.Int8;
                return SortMethod;
            }
            if (index == 6)
            {                
                type = DataType.Array;                
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
                    GXCommon.SetData(data, DataType.Int8, SortObject.SelectedAttributeIndex); //Selected Attribute Index
                    GXCommon.SetData(data, DataType.UInt16, SortObject.SelectedDataIndex); //Selected Data Index
                }            
                return data.ToArray();
            }
            if (index == 7)
            {
                type = DataType.Int8;
                return EntriesInUse;
            }
            if (index == 8)
            {
                type = DataType.Int8;
                return ProfileEntries;
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value)
        {
            if (index == 1)
            {
                LogicalName = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
            }
            else if (index == 2)
            {
                //Client can't set data.
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
            else if (index == 3)
            {
                CaptureObjects.Clear();
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
                    GXDLMSObject obj = Server.Items.FindByLN(type, ln);
                    CaptureObjects.Add(obj);
                }
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
                object[] tmp = value as object[];
                if (tmp.Length != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                ObjectType type = (ObjectType)Convert.ToInt16(tmp[0]);
                string ln = GXDLMSObject.toLogicalName((byte[])tmp[1]);
                int attributeIndex = Convert.ToInt16(tmp[2]);
                int dataIndex = Convert.ToInt16(tmp[3]);
                SortObject = CaptureObjects.FindByLN(type, ln);
            }
            else if (index == 7)
            {
                //Client can't set row count.
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
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
