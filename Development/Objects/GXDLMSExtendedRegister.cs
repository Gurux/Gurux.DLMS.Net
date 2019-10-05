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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSExtendedRegister
    /// </summary>
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
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSExtendedRegister(string ln)
        : base(ObjectType.ExtendedRegister, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSExtendedRegister(string ln, ushort sn)
        : base(ObjectType.ExtendedRegister, ln, sn)
        {
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

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //ScalerUnit
            if (all || !IsRead(3))
            {
                attributes.Add(3);
            }
            //Value
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //Status
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //CaptureTime
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Value",
                             "Scaler and Unit",
                             "Status",
                             "CaptureTime"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
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
                return base.GetDataType(index);
            }
            if (index == 3)
            {
                return DataType.Array;
            }
            if (index == 4)
            {
                return base.GetDataType(index);
            }
            if (index == 5)
            {
                return DataType.OctetString;
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
                //If client set new value.
                if (!settings.IsServer && Scaler != 1 && Value != null)
                {
                    DataType dt = base.GetDataType(2);
                    if (dt == DataType.None && Value != null)
                    {
                        dt = GXCommon.GetDLMSDataType(Value.GetType());
                    }
                    object tmp;
                    tmp = Convert.ToDouble(Value) / Scaler;
                    if (dt != DataType.None)
                    {
                        tmp = Convert.ChangeType(tmp, GXCommon.GetDataType(dt));
                    }
                    return tmp;
                }
                return Value;
            }
            if (e.Index == 3)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(2);
                GXCommon.SetData(settings, data, DataType.Int8, scaler);
                GXCommon.SetData(settings, data, DataType.Enum, Unit);
                return data.Array();
            }
            if (e.Index == 4)
            {
                return Status;
            }
            if (e.Index == 5)
            {
                return CaptureTime;
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
                if (Scaler != 1 && e.Value != null && !e.User)
                {
                    SetDataType(2, GXCommon.GetDLMSDataType(e.Value.GetType()));
                    try
                    {
                        if (settings.IsServer)
                        {
                            Value = e.Value;
                        }
                        else
                        {
                            Value = Convert.ToDouble(e.Value) * Scaler;
                        }
                    }
                    catch (Exception)
                    {
                        //Sometimes scaler is set for wrong Object type.
                        Value = e.Value;
                    }
                }
                else
                {
                    Value = e.Value;
                }
            }
            else if (e.Index == 3)
            {
                if (e.Value == null)
                {
                    Scaler = 1;
                    Unit = Unit.None;
                }
                else
                {
                    List<object> arr = (List<object>)e.Value;
                    if (arr.Count != 2)
                    {
                        throw new Exception("setValue failed. Invalid scaler unit value.");
                    }
                    scaler = Convert.ToInt32(arr[0]);
                    Unit = (Unit)Convert.ToInt32(arr[1]);
                }
            }
            else if (e.Index == 4)
            {
                Status = e.Value;
            }
            else if (e.Index == 5)
            {
                if (e.Value is byte[])
                {
                    e.Value = GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime, settings == null ? false : settings.UseUtc2NormalTime);
                }
                else if (e.Value is string)
                {
                    e.Value = new GXDateTime((string)e.Value);
                }
                //Actaris meters might return null.
                if (e.Value == null)
                {
                    CaptureTime = new GXDateTime();
                }
                else
                {
                    CaptureTime = ((GXDateTime)e.Value).Value.LocalDateTime;
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Unit = (Unit)reader.ReadElementContentAsInt("Unit", 0);
            Scaler = reader.ReadElementContentAsDouble("Scaler", 1);
            Value = reader.ReadElementContentAsObject("Value", null);
            Status = reader.ReadElementContentAsObject("Status", null);
            CaptureTime = (GXDateTime)reader.ReadElementContentAsObject("CaptureTime", null);
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Unit", (int)Unit);
            writer.WriteElementString("Scaler", Scaler, 1);
            writer.WriteElementObject("Value", Value);
            writer.WriteElementObject("Status", Status);
            writer.WriteElementObject("CaptureTime", CaptureTime);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
    }
}
