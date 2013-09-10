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
using System.ComponentModel;
using Gurux.DLMS;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSExtendedRegister : GXDLMSRegister, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSExtendedRegister()
            : base(ObjectType.ExtendedRegister, null, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSExtendedRegister(string ln)
            : base(ObjectType.ExtendedRegister, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSExtendedRegister(string ln, ushort sn)
            : base(ObjectType.ExtendedRegister, ln, 0)
        {
        }

        /// <inheritdoc cref="GXDLMSObject.UpdateDefaultValueItems"/>
        public override void UpdateDefaultValueItems()
        {
            GXDLMSAttributeSettings att = this.Attributes.Find(4);
            if (att == null)
            {
                att = new GXDLMSAttribute(4, DataType.Int32, DataType.Int32);
                att.Access = AccessMode.Read;
                Attributes.Add(att);
            }
            att.Values.Add(new GXObisValueItem(0, "Null"));
            att.Values.Add(new GXObisValueItem(4, "Bit String"));
            att.Values.Add(new GXObisValueItem(6, "Double Long Unsigned"));
            att.Values.Add(new GXObisValueItem(9, "Octet String"));
            att.Values.Add(new GXObisValueItem(10, "Visible String"));
            att.Values.Add(new GXObisValueItem(12, "UTF8 String"));
            att.Values.Add(new GXObisValueItem(17, "Unsigned"));
            att.Values.Add(new GXObisValueItem(18, "Long Unsigned"));
            att = this.Attributes.Find(9);
            if (att == null)
            {
                att = new GXDLMSAttribute(9);
                att.Access = AccessMode.Read;
                Attributes.Add(att);
            }            
            att.Values.Add(new GXObisValueItem(0, "Not defined"));
            att.Values.Add(new GXObisValueItem(1, "Internal Crystal"));
            att.Values.Add(new GXObisValueItem(2, "Mains frequency 50 Hz"));
            att.Values.Add(new GXObisValueItem(3, "Mains Frequency 60 Hz"));
            att.Values.Add(new GXObisValueItem(4, "GPS (Global Positioning System)"));
            att.Values.Add(new GXObisValueItem(5, "Radio Controlled"));
        }

        /// <summary>
        /// Status
        /// </summary>
        [XmlIgnore()]
        public object Status
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

        public override DataType GetUIDataType(int index)
        {
            if (index == 5)
            {
                return DataType.DateTime;
            }
            return base.GetUIDataType(index);
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Value, "Scaler: " + Scaler + " Unit: " + Unit, Status, CaptureTime };
        }

        public override bool IsRead(int index)
        {
            if (index == 3)
            {
                return Unit != 0;
            }
            return base.IsRead(index);
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //ScalerUnit
            if (!IsRead(3))
            {
                attributes.Add(3);
            }
            //Value
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //Status
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            //CaptureTime
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
        }

        object IGXDLMSBase.GetValue(int index, out DataType type, byte[] parameters, bool raw)
        {
            if (index == 1)
            {
                type = DataType.OctetString;
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                type = DataType.None;
                return Value;
            }
            if (index == 3)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Structure);
                data.Add(2);
                GXCommon.SetData(data, DataType.UInt8, m_Scaler);
                GXCommon.SetData(data, DataType.UInt8, Unit);
                return data.ToArray();
            }
            if (index == 4)
            {
                type = DataType.None;
                return Status;
            }
            if (index == 5)
            {
                type = DataType.DateTime;
                return CaptureTime;
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value, bool raw)
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
                if (!raw)
                {
                    if (Scaler != 1)
                    {
                        try
                        {
                            Value = Convert.ToDouble(value) * Scaler;
                        }
                        catch (Exception)
                        {
                            //Sometimes scaler is set for wrong Object type.
                            Value = value;
                        }
                    }
                    else
                    {
                        Value = value;
                    }
                }
                else
                {
                    Value = value;
                }                
            }
            else if (index == 3)
            {
                if (value == null)
                {
                    Scaler = 1;
                    Unit = Unit.None;
                }
                else
                {
                    object[] arr = (object[])value;
                    if (arr.Length != 2)
                    {
                        throw new Exception("setValue failed. Invalid scaler unit value.");
                    }
                    m_Scaler = Convert.ToInt32(arr[0]);
                    Unit = (Unit)Convert.ToInt32(arr[1]);
                }
            }
            else if (index == 4)
            {                
                Status = value;
            }
            else if (index == 5)
            {
                if (value is byte[])
                {
                    value = GXDLMSClient.ChangeType((byte[])value, DataType.DateTime);
                }
                CaptureTime = ((GXDateTime)value).Value;
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
    }
}
