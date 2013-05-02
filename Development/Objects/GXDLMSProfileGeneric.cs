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
        public object SortObject
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
                return Buffer;
            }
            if (index == 3)
            {
                type = DataType.Array;
                return CaptureObjects;
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
                //Mikko
                type = DataType.Array;
                return SortObject;
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
                LogicalName = value.ToString();
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
                SortObject = Server.Items.FindByLN(type, ln);
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
