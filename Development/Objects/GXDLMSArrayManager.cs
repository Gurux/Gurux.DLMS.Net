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
using System.Globalization;
using System.Xml.Linq;
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
        public List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> Elements { get; set; } = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
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
        : this(null, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSArrayManager(string ln)
        : this(ln, 0)
        {
            Objects = new List<GXDLMSArrayManagerItem>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSArrayManager(string ln, ushort sn)
        : base(ObjectType.ArrayManager, ln, sn)
        {
            Objects = new List<GXDLMSArrayManagerItem>();
        }

        [XmlArray("Objects")]
        public List<GXDLMSArrayManagerItem> Objects
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Objects };
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
            //Value
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
                        int cnt = Objects == null ? 0 : Objects.Count;
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        //Add count
                        GXCommon.SetObjectCount(cnt, data);
                        if (cnt != 0)
                        {
                            foreach (var it in Objects)
                            {
                                data.SetUInt8((byte)DataType.Structure);
                                data.SetUInt8(2); //Count
                                GXCommon.SetData(settings, data, DataType.UInt8, it.Id);
                                data.SetUInt8((byte)DataType.Array);
                                //Add count
                                GXCommon.SetObjectCount(it.Elements.Count, data);
                                foreach (var element in it.Elements)
                                {
                                    data.SetUInt8((byte)DataType.Structure);
                                    data.SetUInt8(3); //Count
                                    GXCommon.SetData(settings, data, DataType.UInt16, element.Key.ObjectType);
                                    GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(element.Key.LogicalName));
                                    GXCommon.SetData(settings, data, DataType.Int8, element.Value.AttributeIndex);
                                }
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
                    Objects.Clear();
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
                            foreach (object[] a in (IEnumerable<object>)item[1])
                            {
                                ObjectType ot = (ObjectType)a[0];
                                var obj = GXDLMSClient.CreateObject(ot);
                                obj.LogicalName = GXCommon.ToLogicalName(a[1]);
                                sbyte attributeIndex = (sbyte)a[2];
                                tmp2.Elements.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, new GXDLMSCaptureObject(attributeIndex, 0)));
                            }
                            Objects.Add(tmp2);
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
            Objects.Clear();
            if (reader.IsStartElement("Objects", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    var item = new GXDLMSArrayManagerItem()
                    {
                        Id = (byte)reader.ReadElementContentAsInt("Id")
                    };
                    if (reader.IsStartElement("Targets", true))
                    {
                        while (reader.IsStartElement("Target", true))
                        {
                            ObjectType ot = (ObjectType)reader.ReadElementContentAsInt("Type");
                            int index = reader.ReadElementContentAsInt("Index");
                            var obj = GXDLMSClient.CreateObject(ot);
                            obj.LogicalName = reader.ReadElementContentAsString("LN");
                            item.Elements.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, new GXDLMSCaptureObject(index, 0)));
                        }
                    }
                    Objects.Add(item);
                }
                reader.ReadEndElement("Objects");
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteStartElement("Objects", 1);
            if (Objects != null)
            {
                foreach (var it in Objects)
                {
                    writer.WriteStartElement("Item", 1);
                    //Some meters are returning time here, not date-time.
                    writer.WriteElementString("Id", it.Id, 1);
                    writer.WriteStartElement("Targets", 1);
                    foreach (var element in it.Elements)
                    {
                        writer.WriteStartElement("Target", 1);
                        writer.WriteElementString("Type", (UInt16)element.Key.ObjectType, 1);
                        writer.WriteElementString("LN", element.Key.LogicalName, 1);
                        writer.WriteElementString("Index", element.Value.AttributeIndex, 1);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
