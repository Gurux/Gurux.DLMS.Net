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
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSParameterMonitor
    /// </summary>
    public class GXDLMSParameterMonitor : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSParameterMonitor()
        : this("0.0.16.2.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSParameterMonitor(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSParameterMonitor(string ln, ushort sn)
        : base(ObjectType.ParameterMonitor, ln, sn)
        {
            Parameters = new List<GXDLMSTarget>();
            ChangedParameter = new GXDLMSTarget();
        }

        /// <summary>
        /// Changed parameter.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSTarget ChangedParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Capture time.
        /// </summary>
        [XmlIgnore()]
        public DateTime CaptureTime
        {
            get;
            set;
        }

        /// <summary>
        /// Changed Parameter
        /// </summary>
        [XmlIgnore()]
        public List<GXDLMSTarget> Parameters
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ChangedParameter, CaptureTime, Parameters };
        }

        /// <summary>
        /// Inserts a new entry in the table.
        /// </summary>
        public byte[][] Insert(GXDLMSClient client, GXDLMSTarget entry)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(3);
            GXCommon.SetData(null, bb, DataType.UInt16, entry.Target.ObjectType);
            GXCommon.SetData(null, bb, DataType.OctetString, GXCommon.LogicalNameToBytes(entry.Target.LogicalName));
            GXCommon.SetData(null, bb, DataType.Int8, entry.AttributeIndex);
            return client.Method(this, 1, bb.Array(), DataType.Array);
        }

        /// <summary>
        /// Deletes an entry from the table.
        /// </summary>
        public byte[][] Delete(GXDLMSClient client, GXDLMSTarget entry)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(3);
            GXCommon.SetData(null, bb, DataType.UInt16, entry.Target.ObjectType);
            GXCommon.SetData(null, bb, DataType.OctetString, GXCommon.LogicalNameToBytes(entry.Target.LogicalName));
            GXCommon.SetData(null, bb, DataType.Int8, entry.AttributeIndex);
            return client.Method(this, 2, bb.Array(), DataType.Array);
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index != 1 && e.Index != 2)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            else
            {
                if (e.Index == 1)
                {
                    List<object> tmp = (List<object>)e.Parameters;
                    ObjectType type = (ObjectType)Convert.ToUInt16(tmp[0]);
                    string ln = GXCommon.ToLogicalName((byte[])tmp[1]);
                    byte index = Convert.ToByte(tmp[2]);
                    foreach (GXDLMSTarget item in Parameters)
                    {
                        if (item.Target.ObjectType == type && item.Target.LogicalName == ln && item.AttributeIndex == index)
                        {
                            Parameters.Remove(item);
                            break;
                        }
                    }
                    GXDLMSTarget it = new GXDLMSTarget();
                    it.Target = settings.Objects.FindByLN(type, (byte[])tmp[1]);
                    if (it.Target == null)
                    {
                        it.Target = GXDLMSClient.CreateObject(type);
                        it.Target.LogicalName = ln;
                    }
                    it.AttributeIndex = index;
                    Parameters.Add(it);
                }
                else if (e.Index == 2)
                {
                    List<object> tmp = (List<object>)e.Parameters;
                    ObjectType ot = (ObjectType)Convert.ToUInt16(tmp[0]);
                    string ln = GXCommon.ToLogicalName((byte[])tmp[1]);
                    byte index = Convert.ToByte(tmp[2]);
                    foreach (GXDLMSTarget item in Parameters)
                    {
                        if (item.Target.ObjectType == ot && item.Target.LogicalName == ln && item.AttributeIndex == index)
                        {
                            Parameters.Remove(item);
                            break;
                        }
                    }
                }
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
            //ChangedParameter
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //CaptureTime
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //Parameters
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "ChangedParameter", "CaptureTime", "Parameters" };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Add parameter", "Delete parameter" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 4;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.Structure;
                case 3:
                    return DataType.OctetString;
                case 4:
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
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(4);
                        if (ChangedParameter == null || ChangedParameter.Target == null)
                        {
                            GXCommon.SetData(settings, data, DataType.UInt16, 0);
                            GXCommon.SetData(settings, data, DataType.OctetString, new byte[] { 0, 0, 0, 0, 0, 0 });
                            GXCommon.SetData(settings, data, DataType.Int8, 1);
                            GXCommon.SetData(settings, data, DataType.None, null);
                        }
                        else
                        {
                            GXCommon.SetData(settings, data, DataType.UInt16, ChangedParameter.Target.ObjectType);
                            GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(ChangedParameter.Target.LogicalName));
                            GXCommon.SetData(settings, data, DataType.Int8, ChangedParameter.AttributeIndex);
                            GXCommon.SetData(settings, data, GXDLMSConverter.GetDLMSDataType(ChangedParameter.Value), ChangedParameter.Value);
                        }
                        return data.Array();
                    }
                case 3:
                    return CaptureTime;
                case 4:
                    {
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        if (Parameters == null)
                        {
                            data.SetUInt8(0);
                        }
                        else
                        {
                            data.SetUInt8((byte)Parameters.Count);
                            foreach (GXDLMSTarget it in Parameters)
                            {
                                data.SetUInt8((byte)DataType.Structure);
                                data.SetUInt8((byte)3);
                                GXCommon.SetData(settings, data, DataType.UInt16, it.Target.ObjectType);
                                GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(it.Target.LogicalName));
                                GXCommon.SetData(settings, data, DataType.Int8, it.AttributeIndex);
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
                    {
                        ChangedParameter = new GXDLMSTarget();
                        List<object> tmp = null;
                        if (e.Value is List<object>)
                        {
                            tmp = (List<object>)e.Value;
                        }
                        else if (e.Value is object[])
                        {
                            tmp = new List<object>((object[])e.Value);
                        }
                        if (tmp != null)
                        {
                            if (tmp.Count != 4)
                            {
                                throw new GXDLMSException("Invalid structure format.");
                            }
                            ObjectType type = (ObjectType)Convert.ToInt16(tmp[0]);
                            ChangedParameter.Target = settings.Objects.FindByLN(type, (byte[])tmp[1]);
                            if (ChangedParameter.Target == null)
                            {
                                ChangedParameter.Target = GXDLMSClient.CreateObject(type);
                                ChangedParameter.Target.LogicalName = GXCommon.ToLogicalName((byte[])tmp[1]);
                            }
                            ChangedParameter.AttributeIndex = Convert.ToByte(tmp[2]);
                            ChangedParameter.Value = tmp[3];
                        }

                        break;
                    }

                case 3:
                    {
                        if (e.Value == null)
                        {
                            CaptureTime = new GXDateTime(DateTime.MinValue);
                        }
                        else
                        {
                            if (e.Value is byte[])
                            {
                                e.Value = GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime, settings.UseUtc2NormalTime);
                            }
                            else if (e.Value is string)
                            {
                                e.Value = new GXDateTime((string)e.Value);
                            }
                            if (e.Value is GXDateTime)
                            {
                                CaptureTime = (GXDateTime)e.Value;
                            }
                            else if (e.Value is string)
                            {
                                DateTime tm;
                                if (!DateTime.TryParse((String)e.Value, out tm))
                                {
                                    CaptureTime = DateTime.ParseExact((String)e.Value, CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern, CultureInfo.CurrentUICulture);
                                }
                                else
                                {
                                    CaptureTime = tm;
                                }
                            }
                            else
                            {
                                CaptureTime = Convert.ToDateTime(e.Value);
                            }
                        }

                        break;
                    }

                case 4:
                    {
                        Parameters.Clear();
                        if (e.Value != null)
                        {
                            foreach (object it in e.Value as IEnumerable<object>)
                            {
                                List<object> tmp;
                                if (it is List<object>)
                                {
                                    tmp = (List<object>)it;
                                }
                                else
                                {
                                    tmp = new List<object>((object[])it);
                                }
                                if (tmp.Count != 3)
                                {
                                    throw new GXDLMSException("Invalid structure format.");
                                }
                                GXDLMSTarget obj = new GXDLMSTarget();
                                ObjectType type = (ObjectType)Convert.ToInt16(tmp[0]);
                                obj.Target = settings.Objects.FindByLN(type, (byte[])tmp[1]);
                                if (obj.Target == null)
                                {
                                    obj.Target = GXDLMSClient.CreateObject(type);
                                    obj.Target.LogicalName = GXCommon.ToLogicalName((byte[])tmp[1]);
                                }
                                obj.AttributeIndex = Convert.ToByte(tmp[2]);
                                Parameters.Add(obj);
                            }
                        }

                        break;
                    }

                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            ChangedParameter = new GXDLMSTarget();
            if (reader.IsStartElement("ChangedParameter", true))
            {
                ObjectType ot = (ObjectType)reader.ReadElementContentAsInt("Type");
                string ln = reader.ReadElementContentAsString("LN");
                ChangedParameter.Target = reader.Objects.FindByLN(ot, ln);
                if (ChangedParameter.Target == null)
                {
                    ChangedParameter.Target = GXDLMSClient.CreateObject(ot);
                    ChangedParameter.Target.LogicalName = ln;
                }
                ChangedParameter.AttributeIndex = (byte)reader.ReadElementContentAsInt("Index");
                ChangedParameter.Value = reader.ReadElementContentAsObject("Value", null, null, 0);
                reader.ReadEndElement("ChangedParameter");
            }
            if (string.Compare("Time", reader.Name, true) == 0)
            {
                CaptureTime = new GXDateTime(reader.ReadElementContentAsString("Time"), CultureInfo.InvariantCulture);
            }
            else
            {
                CaptureTime = DateTime.MinValue;
            }
            Parameters.Clear();
            if (reader.IsStartElement("Parameters", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSTarget obj = new GXDLMSTarget();
                    ObjectType ot = (ObjectType)reader.ReadElementContentAsInt("Type");
                    string ln = reader.ReadElementContentAsString("LN");
                    obj.Target = reader.Objects.FindByLN(ot, ln);
                    if (obj.Target == null)
                    {
                        obj.Target = GXDLMSClient.CreateObject(ot);
                        obj.Target.LogicalName = ln;
                    }
                    obj.AttributeIndex = (byte)reader.ReadElementContentAsInt("Index");
                    Parameters.Add(obj);
                }
                reader.ReadEndElement("Parameters");
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteStartElement("ChangedParameter", 2);
            if (ChangedParameter != null && ChangedParameter.Target != null)
            {
                writer.WriteElementString("Type", (int)ChangedParameter.Target.ObjectType, 2);
                writer.WriteElementString("LN", ChangedParameter.Target.LogicalName, 2);
                writer.WriteElementString("Index", ChangedParameter.AttributeIndex, 2);
                writer.WriteElementObject("Value", ChangedParameter.Value, 2);
            }
            writer.WriteEndElement();
            writer.WriteElementString("Time", CaptureTime, 3);
            writer.WriteStartElement("Parameters", 4);
            if (Parameters != null && Parameters.Count != 0)
            {
                foreach (GXDLMSTarget it in Parameters)
                {
                    writer.WriteStartElement("Item", 4);
                    writer.WriteElementString("Type", (int)it.Target.ObjectType, 4);
                    writer.WriteElementString("LN", it.Target.LogicalName, 4);
                    writer.WriteElementString("Index", it.AttributeIndex, 4);
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
