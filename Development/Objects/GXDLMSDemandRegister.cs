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
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using System.Globalization;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSDemandRegister
    /// </summary>
    public class GXDLMSDemandRegister : GXDLMSObject, IGXDLMSBase
    {
        protected int scaler;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSDemandRegister()
        : base(ObjectType.DemandRegister)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSDemandRegister(string ln)
        : base(ObjectType.DemandRegister, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSDemandRegister(string ln, ushort sn)
        : base(ObjectType.DemandRegister, ln, sn)
        {
        }

        /// <summary>
        /// Current average value of COSEM Data object.
        /// </summary>
        [XmlIgnore()]
        public object CurrentAverageValue
        {
            get;
            set;
        }

        /// <summary>
        /// Last average value of COSEM Data object.
        /// </summary>
        [XmlIgnore()]
        public object LastAverageValue
        {
            get;
            set;
        }

        /// <summary>
        /// Scaler of COSEM Register object.
        /// </summary>
        [DefaultValue(1.0)]
        public double Scaler
        {
            get
            {
                return Math.Pow(10, scaler);
            }
            set
            {
                scaler = (int)Math.Log10(value);
            }
        }

        /// <summary>
        /// Unit of COSEM Register object.
        /// </summary>
        [DefaultValue(Unit.None)]
        public Unit Unit
        {
            get;
            set;
        }

        /// <summary>
        /// Provides Demand register specific status information.
        /// </summary>
        [XmlIgnore()]
        public Object Status
        {
            get;
            set;
        }

        /// <summary>
        /// Capture time of COSEM Register object.
        /// </summary>
        [XmlIgnore()]
        public GXDateTime CaptureTime
        {
            get;
            set;
        }

        /// <summary>
        /// Current start time of COSEM Register object.
        /// </summary>
        [XmlIgnore()]
        public GXDateTime StartTimeCurrent
        {
            get;
            set;
        }

        /// <summary>
        /// Period is the interval between two successive updates of the Last Average Value.
        /// (NumberOfPeriods * Period is the denominator for the calculation of the demand).
        /// </summary>
        [XmlIgnore()]
        public UInt32 Period
        {
            get;
            set;
        }

        /// <summary>
        /// The number of periods used to calculate the LastAverageValue.
        /// NumberOfPeriods >= 1 NumberOfPeriods > 1 indicates that the LastAverageValue represents �sliding demand�.
        /// NumberOfPeriods = 1 indicates that the LastAverageValue represents "block demand".
        /// The behaviour of the meter after writing a new value to this attribute shall be
        /// specified by the manufacturer.
        /// </summary>
        [XmlIgnore()]
        public UInt16 NumberOfPeriods
        {
            get;
            set;
        }

        public override bool IsRead(int index)
        {
            if (index == 4)
            {
                return this.Unit != Unit.None;
            }
            return base.IsRead(index);
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, CurrentAverageValue, LastAverageValue, new GXStructure{ Scaler, Unit },
                              Status, CaptureTime, StartTimeCurrent, Period, NumberOfPeriods
                            };
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

            //Scaler and Unit
            if (all || !IsRead(4))
            {
                attributes.Add(4);
            }
            //CurrentAverageValue
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }

            //LastAverageValue
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //Status
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //CaptureTime
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //StartTimeCurrent
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //Period
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //NumberOfPeriods
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Current Average Value",
                             "Last Average Value",
                             "Scaler and Unit",
                             "Status",
                             "Capture Time",
                             "Start Time Current",
                             "Period",
                             "Number Of Periods"
                            };
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Reset", "Next period" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 9;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
        }

        public override DataType GetUIDataType(int index)
        {
            if (index == 6 || index == 7)
            {
                return DataType.DateTime;
            }
            return base.GetUIDataType(index);
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            DataType dt;
            switch (index)
            {
                case 1:
                    dt = DataType.OctetString;
                    break;
                case 2:
                    dt = base.GetDataType(index);
                    if (dt == DataType.None && CurrentAverageValue != null)
                    {
                        dt = GXCommon.GetDLMSDataType(CurrentAverageValue.GetType());
                    }
                    break;
                case 3:
                    dt = base.GetDataType(index);
                    if (dt == DataType.None && LastAverageValue != null)
                    {
                        dt = GXCommon.GetDLMSDataType(LastAverageValue.GetType());
                    }
                    break;
                case 4:
                    dt = DataType.Array;
                    break;
                case 5:
                    dt = base.GetDataType(index);
                    if (dt == DataType.None && Status != null)
                    {
                        dt = GXCommon.GetDLMSDataType(Status.GetType());
                    }
                    break;
                case 6:
                    dt = DataType.OctetString;
                    break;
                case 7:
                    dt = DataType.OctetString;
                    break;
                case 8:
                    dt = DataType.UInt32;
                    break;
                case 9:
                    dt = DataType.UInt16;
                    break;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
            return dt;
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
                if (!settings.IsServer && Scaler != 1 && CurrentAverageValue != null)
                {
                    Type type = null;
                    if (CurrentAverageValue != null)
                    {
                        type = CurrentAverageValue.GetType();
                    }
                    object tmp;
                    tmp = Convert.ToDouble(CurrentAverageValue) / Scaler;
                    if (type != null)
                    {
                        tmp = Convert.ChangeType(tmp, type);
                    }
                    return tmp;
                }
                return CurrentAverageValue;
            }
            if (e.Index == 3)
            {
                //If client set new value.
                if (!settings.IsServer && Scaler != 1 && LastAverageValue != null)
                {
                    DataType dt = base.GetDataType(3);
                    if (dt == DataType.None && LastAverageValue != null)
                    {
                        dt = GXCommon.GetDLMSDataType(LastAverageValue.GetType());
                    }
                    object tmp;
                    tmp = Convert.ToDouble(LastAverageValue) / Scaler;
                    if (dt != DataType.None)
                    {
                        tmp = Convert.ChangeType(tmp, GXCommon.GetDataType(dt));
                    }
                    return tmp;
                }
                return LastAverageValue;
            }
            if (e.Index == 4)
            {
                e.ByteArray = true;
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(2);
                GXCommon.SetData(settings, data, DataType.Int8, scaler);
                GXCommon.SetData(settings, data, DataType.Enum, Unit);
                return data.Array();
            }
            if (e.Index == 5)
            {
                return Status;
            }
            if (e.Index == 6)
            {
                return CaptureTime;
            }
            if (e.Index == 7)
            {
                return StartTimeCurrent;
            }
            if (e.Index == 8)
            {
                return Period;
            }
            if (e.Index == 9)
            {
                return NumberOfPeriods;
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
                    try
                    {
                        if (settings.IsServer)
                        {
                            CurrentAverageValue = e.Value;
                        }
                        else
                        {
                            CurrentAverageValue = Convert.ToDouble(e.Value) * Scaler;
                        }
                    }
                    catch (Exception)
                    {
                        //Sometimes scaler is set for wrong Object type.
                        CurrentAverageValue = e.Value;
                    }
                }
                else
                {
                    CurrentAverageValue = e.Value;
                }
            }
            else if (e.Index == 3)
            {
                if (Scaler != 1 && e.Value != null && !e.User)
                {
                    try
                    {
                        SetDataType(e.Index, GXCommon.GetDLMSDataType(e.Value.GetType()));
                        if (settings.IsServer)
                        {
                            LastAverageValue = e.Value;
                        }
                        else
                        {
                            LastAverageValue = Convert.ToDouble(e.Value) * Scaler;
                        }
                    }
                    catch (Exception)
                    {
                        //Sometimes scaler is set for wrong Object type.
                        LastAverageValue = e.Value;
                    }
                }
                else
                {
                    LastAverageValue = e.Value;
                }
            }
            else if (e.Index == 4)
            {
                if (e.Value == null)
                {
                    Scaler = 1;
                    Unit = Unit.None;
                }
                else
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
                    if (arr.Count != 2)
                    {
                        throw new Exception("setValue failed. Invalid scaler unit value.");
                    }
                    scaler = Convert.ToInt32(arr[0]);
                    Unit = (Unit)Convert.ToInt32(arr[1]);
                }
            }
            else if (e.Index == 5)
            {
                Status = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 6)
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
                    CaptureTime = (GXDateTime)e.Value;
                }
            }
            else if (e.Index == 7)
            {
                if (e.Value == null)
                {
                    StartTimeCurrent = new GXDateTime(DateTime.MinValue);
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
                    StartTimeCurrent = (GXDateTime)e.Value;
                }
            }
            else if (e.Index == 8)
            {
                Period = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 9)
            {
                NumberOfPeriods = Convert.ToUInt16(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        /// <summary>
        /// Reset value.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Reset(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        /// <summary>
        /// Closes the current period and starts a new one.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] NextPeriod(GXDLMSClient client)
        {
            return client.Method(this, 2, (sbyte)0);
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            // Resets the value to the default value.
            // The default value is an instance specific constant.
            if (e.Index == 1)
            {
                CurrentAverageValue = LastAverageValue = null;
                CaptureTime = StartTimeCurrent = new GXDateTime(DateTime.Now);
            }
            else if (e.Index == 2)
            {
                LastAverageValue = CurrentAverageValue;
                CurrentAverageValue = null;
                CaptureTime = StartTimeCurrent = new GXDateTime(DateTime.Now);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            return null;
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            CurrentAverageValue = reader.ReadElementContentAsObject("CurrentAverageValue", null, this, 2);
            LastAverageValue = reader.ReadElementContentAsObject("LastAverageValue", null, this, 3);
            Scaler = reader.ReadElementContentAsDouble("Scaler", 1);
            Unit = (Unit)reader.ReadElementContentAsInt("Unit");
            Status = reader.ReadElementContentAsObject("Status", null, this, 5);
            string str = reader.ReadElementContentAsString("CaptureTime");
            if (str == null)
            {
                CaptureTime = null;
            }
            else
            {
                CaptureTime = new GXDateTime(str, CultureInfo.InvariantCulture);
            }
            str = reader.ReadElementContentAsString("StartTimeCurrent");
            if (str == null)
            {
                StartTimeCurrent = null;
            }
            else
            {
                StartTimeCurrent = new GXDateTime(str, CultureInfo.InvariantCulture);
            }
            Period = (UInt32)reader.ReadElementContentAsInt("Period");
            NumberOfPeriods = (UInt16)reader.ReadElementContentAsInt("NumberOfPeriods");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementObject("CurrentAverageValue", CurrentAverageValue, GetDataType(2), GetUIDataType(2), 2);
            writer.WriteElementObject("LastAverageValue", LastAverageValue, 3);
            writer.WriteElementString("Scaler", Scaler, 1, 4);
            writer.WriteElementString("Unit", (int)Unit, 5);
            writer.WriteElementObject("Status", Status, 6);
            writer.WriteElementString("CaptureTime", CaptureTime, 7);
            writer.WriteElementString("StartTimeCurrent", StartTimeCurrent, 8);
            writer.WriteElementString("Period", Period, 9);
            writer.WriteElementString("NumberOfPeriods", NumberOfPeriods, 10);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
