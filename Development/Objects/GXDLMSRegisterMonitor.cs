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
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSRegisterMonitor
    /// </summary>
    public class GXDLMSRegisterMonitor : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSRegisterMonitor()
        : base(ObjectType.RegisterMonitor)
        {
            this.Thresholds = new object[0];
            MonitoredValue = new GXDLMSMonitoredValue();
            Actions = new GXDLMSActionSet[0];
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSRegisterMonitor(string ln)
        : base(ObjectType.RegisterMonitor, ln, 0)
        {
            this.Thresholds = new object[0];
            MonitoredValue = new GXDLMSMonitoredValue();
            Actions = new GXDLMSActionSet[0];
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSRegisterMonitor(string ln, ushort sn)
        : base(ObjectType.RegisterMonitor, ln, sn)
        {
            this.Thresholds = new object[0];
            MonitoredValue = new GXDLMSMonitoredValue();
            Actions = new GXDLMSActionSet[0];
        }

        [XmlIgnore()]
        public object[] Thresholds
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDLMSMonitoredValue MonitoredValue
        {
            get;
            set;
        }


        [XmlIgnore()]
        public GXDLMSActionSet[] Actions
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Thresholds, MonitoredValue, Actions };
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
            //Thresholds
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            //MonitoredValue
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //Actions
            if (all || !base.IsRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Thresholds", "Monitored Value", "Actions" };
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[0];
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
            return 0;
        }

        /// <inheritdoc />
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
                return this.Thresholds;
            }
            if (e.Index == 3)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((int)DataType.Structure);
                data.SetUInt8(3);
                GXCommon.SetData(settings, data, DataType.UInt16, MonitoredValue.ObjectType); //ClassID
                GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(MonitoredValue.LogicalName)); //Logical name.
                GXCommon.SetData(settings, data, DataType.Int8, MonitoredValue.AttributeIndex); //Attribute Index
                return data.Array();
            }
            if (e.Index == 4)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((int)DataType.Array);
                if (Actions == null)
                {
                    data.SetUInt8(0);
                }
                else
                {
                    data.SetUInt8((byte)Actions.Length);
                    foreach (GXDLMSActionSet it in Actions)
                    {
                        data.SetUInt8((int)DataType.Structure);
                        data.SetUInt8(2);
                        data.SetUInt8((int)DataType.Structure);
                        data.SetUInt8(2);
                        GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(it.ActionUp.LogicalName)); //Logical name.
                        GXCommon.SetData(settings, data, DataType.UInt16, it.ActionUp.ScriptSelector); //ScriptSelector
                        data.SetUInt8((int)DataType.Structure);
                        data.SetUInt8(2);
                        GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(it.ActionDown.LogicalName)); //Logical name.
                        GXCommon.SetData(settings, data, DataType.UInt16, it.ActionDown.ScriptSelector); //ScriptSelector
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
                if (e.Value != null)
                {
                    List<object> arr;
                    if (e.Value is List<object>)
                    {
                        arr = (List<object>)e.Value;
                    }
                    else
                    {
                        arr = new List<object>((object[])e.Value);
                    }
                    Thresholds = arr.ToArray();
                }
                else
                {
                    Thresholds = new object[0];
                }
            }
            else if (e.Index == 3)
            {
                if (MonitoredValue == null)
                {
                    MonitoredValue = new GXDLMSMonitoredValue();
                }
                if (e.Value != null)
                {
                    List<object> arr;
                    if (e.Value is List<object>)
                    {
                        arr = (List<object>)e.Value;
                    }
                    else
                    {
                        arr = new List<object>((object[])e.Value);
                    }
                    MonitoredValue.ObjectType = (ObjectType)Convert.ToInt32(arr[0]);
                    MonitoredValue.LogicalName = GXCommon.ToLogicalName(arr[1]);
                    MonitoredValue.AttributeIndex = Convert.ToInt32(arr[2]);
                }
                else
                {
                    MonitoredValue.ObjectType = ObjectType.None;
                    MonitoredValue.LogicalName = "";
                    MonitoredValue.AttributeIndex = 0;
                }
            }
            else if (e.Index == 4)
            {
                Actions = null;
                if (e.Value != null)
                {
                    List<GXDLMSActionSet> items = new List<GXDLMSActionSet>();
                    List<object> arr, action_set, it;
                    if (e.Value is List<object>)
                    {
                        arr = (List<object>)e.Value;
                    }
                    else
                    {
                        arr = new List<object>((object[])e.Value);
                    }
                    foreach (object tmp in arr)
                    {
                        if (tmp is List<object>)
                        {
                            action_set = (List<object>)tmp;
                        }
                        else
                        {
                            action_set = new List<object>((object[])tmp);
                        }
                        if (action_set[0] is List<object>)
                        {
                            it = (List<object>)action_set[0];
                        }
                        else
                        {
                            it = new List<object>((object[])action_set[0]);
                        }
                        GXDLMSActionSet set = new GXDLMSActionSet();
                        set.ActionUp.LogicalName = GXCommon.ToLogicalName(it[0]);
                        set.ActionUp.ScriptSelector = Convert.ToUInt16(it[1]);
                        if (action_set[1] is List<object>)
                        {
                            it = (List<object>)action_set[1];
                        }
                        else
                        {
                            it = new List<object>((object[])action_set[1]);
                        }
                        set.ActionDown.LogicalName = GXCommon.ToLogicalName(it[0]);
                        set.ActionDown.ScriptSelector = Convert.ToUInt16(it[1]);
                        items.Add(set);
                    }
                    Actions = items.ToArray();
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            List<object> thresholds = new List<object>();
            if (reader.IsStartElement("Thresholds", true))
            {
                while (reader.IsStartElement("Value", false))
                {
                    object it = reader.ReadElementContentAsObject("Value", null, null, 0);
                    thresholds.Add(it);
                }
                reader.ReadEndElement("Thresholds");
            }
            Thresholds = thresholds.ToArray();
            if (reader.IsStartElement("MonitoredValue", true))
            {
                MonitoredValue.ObjectType = (ObjectType)reader.ReadElementContentAsInt("ObjectType");
                MonitoredValue.LogicalName = reader.ReadElementContentAsString("LN");
                MonitoredValue.AttributeIndex = reader.ReadElementContentAsInt("Index");
                reader.ReadEndElement("MonitoredValue");
            }

            List<GXDLMSActionSet> list = new List<GXDLMSActionSet>();
            if (reader.IsStartElement("Actions", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSActionSet it = new GXDLMSActionSet();
                    list.Add(it);
                    if (reader.IsStartElement("Up", true))
                    {
                        it.ActionUp.LogicalName = reader.ReadElementContentAsString("LN", null);
                        if (it.ActionUp.LogicalName == "")
                        {
                            it.ActionUp.LogicalName = "0.0.0.0.0.0";
                        }
                        it.ActionUp.ScriptSelector = (UInt16)reader.ReadElementContentAsInt("Selector");
                        reader.ReadEndElement("Up");
                    }
                    if (reader.IsStartElement("Down", true))
                    {
                        it.ActionDown.LogicalName = reader.ReadElementContentAsString("LN", null);
                        if (it.ActionDown.LogicalName == "")
                        {
                            it.ActionDown.LogicalName = "0.0.0.0.0.0";
                        }
                        it.ActionDown.ScriptSelector = (UInt16)reader.ReadElementContentAsInt("Selector");
                        reader.ReadEndElement("Down");
                    }
                }
                reader.ReadEndElement("Actions");
            }
            Actions = list.ToArray();
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (Thresholds != null)
            {
                writer.WriteStartElement("Thresholds", 2);
                foreach (Object it in Thresholds)
                {
                    writer.WriteElementObject("Value", it, 0);
                }
                writer.WriteEndElement();
            }
            if (MonitoredValue != null)
            {
                writer.WriteStartElement("MonitoredValue", 3);
                writer.WriteElementString("ObjectType", (int)MonitoredValue.ObjectType, 3);
                writer.WriteElementString("LN", MonitoredValue.LogicalName, 3);
                writer.WriteElementString("Index", MonitoredValue.AttributeIndex, 3);
                writer.WriteEndElement();
            }

            if (Actions != null)
            {
                writer.WriteStartElement("Actions", 4);
                foreach (GXDLMSActionSet it in Actions)
                {
                    writer.WriteStartElement("Item", 4);
                    writer.WriteStartElement("Up", 4);
                    writer.WriteElementString("LN", it.ActionUp.LogicalName, 4);
                    writer.WriteElementString("Selector", it.ActionUp.ScriptSelector, 4);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Down", 4);
                    writer.WriteElementString("LN", it.ActionDown.LogicalName, 4);
                    writer.WriteElementString("Selector", it.ActionDown.ScriptSelector, 4);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
