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
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
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
        /// <param name="ln">Logican Name of the object.</param>
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
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSRegisterMonitor(string ln, ushort sn)
            : base(ObjectType.RegisterMonitor, ln, 0)
        {
            this.Thresholds = new object[0];
            MonitoredValue = new GXDLMSMonitoredValue();
            Actions = new GXDLMSActionSet[0];
        }
        
        [XmlIgnore()]
        [GXDLMSAttribute(2, Static = true)]
        public object[] Thresholds
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(3, Static = true)]
        public GXDLMSMonitoredValue MonitoredValue
        {
            get;
            internal set;
        }


        [XmlIgnore()]
        [GXDLMSAttribute(4, Static = true)]
        public GXDLMSActionSet[] Actions
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, Thresholds, MonitoredValue, Actions };
        }

        #region IGXDLMSBase Members

        int IGXDLMSBase.GetAttributeCount()
        {
            return 4;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
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
                type = DataType.None;
                return this.Thresholds;
            }
            if (index == 3)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((int) DataType.Structure);
                data.Add(3);
                GXCommon.SetData(data, DataType.UInt16, MonitoredValue.ObjectType); //ClassID
                GXCommon.SetData(data, DataType.OctetString, MonitoredValue.LogicalName); //Logical name.
                GXCommon.SetData(data, DataType.Int8, MonitoredValue.AttributeIndex); //Attribute Index
                return data.ToArray();
            }
            if (index == 4)
            {
                type = DataType.Structure;
                return Actions;
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
                Thresholds = (object[]) value;
            }
            else if (index == 3)
            {
                if (MonitoredValue == null)
                {
                    MonitoredValue = new GXDLMSMonitoredValue();
                }
                MonitoredValue.ObjectType = (ObjectType)Convert.ToInt32(((object[])value)[0]);
                MonitoredValue.LogicalName = GXDLMSClient.ChangeType((byte[])((object[])value)[1], DataType.OctetString).ToString();
                MonitoredValue.AttributeIndex = Convert.ToInt32(((object[])value)[2]);
            }
            else if (index == 4)
            {
                Actions = null;
                if (value != null)
                {
                    List<GXDLMSActionSet> items = new List<GXDLMSActionSet>();
                    foreach (Object[] action_set in (Object[])value)
                    {
                        GXDLMSActionSet set = new GXDLMSActionSet();                        
                        set.ActionUp.LogicalName = GXDLMSClient.ChangeType((byte[])((Object[])action_set[0])[0], DataType.OctetString).ToString();
                        set.ActionUp.ScriptSelector = Convert.ToUInt16(((Object[])action_set[0])[1]);
                        set.ActionDown.LogicalName = GXDLMSClient.ChangeType((byte[])((Object[])action_set[1])[0], DataType.OctetString).ToString();
                        set.ActionDown.ScriptSelector = Convert.ToUInt16(((Object[])action_set[1])[1]);
                        items.Add(set);
                    }
                    Actions = items.ToArray();
                }
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }

        void IGXDLMSBase.Invoke(int index, object parameters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
