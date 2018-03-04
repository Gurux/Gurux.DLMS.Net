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
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using System.Globalization;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSSpecialDaysTable : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSpecialDaysTable()
        : base(ObjectType.SpecialDaysTable, "0.0.11.0.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSSpecialDaysTable(string ln)
        : base(ObjectType.SpecialDaysTable, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSSpecialDaysTable(string ln, ushort sn)
        : base(ObjectType.SpecialDaysTable, ln, sn)
        {
        }

        /// <inheritdoc cref="GXDLMSObject.LogicalName"/>
        [DefaultValue("0.0.11.0.0.255")]
        override public string LogicalName
        {
            get;
            set;
        }

        /// <summary>
        /// Value of COSEM Data object.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSSpecialDay[] Entries
        {
            get;
            set;
        }

        /// <summary>
        /// Inserts a new entry in the table.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>
        ///  If a special day with the same index or with the same date as an already defined day is inserted, 
        ///  the old entry will be overwritten.
        /// </returns>
        public byte[][] Insert(GXDLMSClient client, GXDLMSSpecialDay entry)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(3);
            GXCommon.SetData(null, bb, DataType.UInt16, entry.Index);
            GXCommon.SetData(null, bb, DataType.OctetString, entry.Date);
            GXCommon.SetData(null, bb, DataType.UInt8, entry.DayId);
            return client.Method(this, 1, bb.Array(), DataType.Array);
        }

        /// <summary>
        /// Deletes an entry from the table.
        /// </summary>
        /// <returns></returns>
        public byte[][] Delete(GXDLMSClient client, GXDLMSSpecialDay entry)
        {
            return client.Method(this, 2, (UInt16)entry.Index);
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Entries };
        }

        #region IGXDLMSBase Members

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //Entries
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Entries" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 2;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
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
                if (Entries == null)
                {
                    data.SetUInt8(0);
                }
                else
                {
                    int cnt = Entries.Length;
                    //Add count
                    GXCommon.SetObjectCount(cnt, data);
                    if (cnt != 0)
                    {
                        foreach (GXDLMSSpecialDay it in Entries)
                        {
                            data.SetUInt8((byte)DataType.Structure);
                            data.SetUInt8((byte)3); //Count
                            GXCommon.SetData(settings, data, DataType.UInt16, it.Index);
                            GXCommon.SetData(settings, data, DataType.OctetString, it.Date);
                            GXCommon.SetData(settings, data, DataType.UInt8, it.DayId);
                        }
                    }
                }
                return data.Array();
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
                Entries = null;
                if (e.Value != null)
                {
                    List<GXDLMSSpecialDay> items = new List<GXDLMSSpecialDay>();
                    foreach (Object[] item in (Object[])e.Value)
                    {
                        GXDLMSSpecialDay it = new GXDLMSSpecialDay();
                        it.Index = Convert.ToUInt16(item[0]);
                        it.Date = (GXDate)GXDLMSClient.ChangeType((byte[])item[1], DataType.Date, settings.UseUtc2NormalTime);
                        it.DayId = Convert.ToByte(item[2]);
                        items.Add(it);
                    }
                    Entries = items.ToArray();
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index != 1 && e.Index != 2)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            else
            {

                List<GXDLMSSpecialDay> items = new List<GXDLMSSpecialDay>();
                if (Entries != null)
                {
                    items.AddRange(Entries);
                }
                if (e.Index == 1)
                {
                    Object[] item = (Object[])e.Parameters;
                    GXDLMSSpecialDay it = new GXDLMSSpecialDay();
                    it.Index = Convert.ToUInt16(item[0]);
                    it.Date = (GXDate)GXDLMSClient.ChangeType((byte[])item[1], DataType.Date, settings.UseUtc2NormalTime);
                    it.DayId = Convert.ToByte(item[2]);
                    foreach (GXDLMSSpecialDay item2 in items)
                    {
                        if (item2.Index == it.Index)
                        {
                            items.Remove(item2);
                            break;
                        }
                    }
                    items.Add(it);
                }
                else if (e.Index == 2)
                {
                    UInt16 index = Convert.ToUInt16(e.Parameters);
                    foreach (GXDLMSSpecialDay item in items)
                    {
                        if (item.Index == index)
                        {
                            items.Remove(item);
                            break;
                        }
                    }
                }
                Entries = items.ToArray();
            }
            return null;
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            List<GXDLMSSpecialDay> list = new List<GXDLMSSpecialDay>();
            if (reader.IsStartElement("Entries", true))
            {
                while (reader.IsStartElement("Entry", true))
                {
                    GXDLMSSpecialDay it = new GXDLMSSpecialDay();
                    it.Index = (UInt16)reader.ReadElementContentAsInt("Index");
                    it.Date = new GXDate(reader.ReadElementContentAsString("Date"), CultureInfo.InvariantCulture);
                    it.DayId = (byte)reader.ReadElementContentAsInt("DayId");
                    list.Add(it);
                }
                reader.ReadEndElement("Entries");
            }
            Entries = list.ToArray();
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (Entries != null)
            {
                writer.WriteStartElement("Entries");
                foreach (GXDLMSSpecialDay it in Entries)
                {
                    writer.WriteStartElement("Entry");
                    writer.WriteElementString("Index", it.Index);
                    writer.WriteElementString("Date", it.Date.ToFormatString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteElementString("DayId", it.DayId);
                    writer.WriteEndElement();//Entry
                }
                writer.WriteEndElement();//Entries
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
