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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Credit Type.
    /// </summary>
    public enum CreditType : byte
    {
        /// <summary>
        /// Token credit.
        /// </summary>
        Token,
        /// <summary>
        /// Reserved credit.
        /// </summary>
        Reserved,
        /// <summary>
        /// Emergency credit.
        /// </summary>
        Emergency,
        /// <summary>
        /// TimeBased credit.
        /// </summary>
        TimeBased,
        /// <summary>
        /// Consumption based credit.
        /// </summary>
        ConsumptionBased
    }

    /// <summary>
    ///  Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCredit
    /// </summary>
    public class GXDLMSCredit : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSCredit()
        : this("0.0.19.10.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSCredit(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSCredit(string ln, ushort sn)
        : base(ObjectType.Credit, ln, sn)
        {
            CreditConfiguration = CreditConfiguration.None;
        }

        /// <summary>
        /// Current credit amount.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCredit
        /// </remarks>
        [XmlIgnore()]
        public int CurrentCreditAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Type.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCredit
        /// </remarks>
        [XmlIgnore()]
        public CreditType Type
        {
            get;
            set;
        }
        /// <summary>
        /// Priority.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCredit
        /// </remarks>
        [XmlIgnore()]
        public byte Priority
        {
            get;
            set;
        }
        /// <summary>
        /// Warning threshold.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCredit
        /// </remarks>
        [XmlIgnore()]
        public int WarningThreshold
        {
            get;
            set;
        }
        /// <summary>
        /// Limit.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCredit
        /// </remarks>
        [XmlIgnore()]
        public int Limit
        {
            get;
            set;
        }
        /// <summary>
        /// Credit configuration.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCredit
        /// </remarks>
        [XmlIgnore()]
        public CreditConfiguration CreditConfiguration
        {
            get;
            set;
        }
        /// <summary>
        /// Status.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCredit
        /// </remarks>
        [XmlIgnore()]
        public CreditStatus Status
        {
            get;
            set;
        }
        /// <summary>
        /// Preset credit amount.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCredit
        /// </remarks>
        [XmlIgnore()]
        public int PresetCreditAmount
        {
            get;
            set;
        }
        /// <summary>
        /// Credit available threshold.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCredit
        /// </remarks>
        [XmlIgnore()]
        public int CreditAvailableThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// Period.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCredit
        /// </remarks>
        [XmlIgnore()]
        public GXDateTime Period
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, CurrentCreditAmount, Type, Priority, WarningThreshold, Limit, CreditConfiguration, Status, PresetCreditAmount, CreditAvailableThreshold, Period };
        }

        /// <summary>
        /// Adjusts the value of the current credit amount attribute.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="value">Current credit amount</param>
        /// <returns>Action bytes.</returns>
        /// <summary>
        public byte[][] UpdateAmount(GXDLMSClient client, Int32 value)
        {
            return client.Method(this, 1, value, DataType.Int32);
        }

        /// <summary>
        /// Sets the value of the current credit amount attribute.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="value">Current credit amount</param>
        /// <returns>Action bytes.</returns>
        /// <summary>
        public byte[][] SetAmountToValue(GXDLMSClient client, Int32 value)
        {
            return client.Method(this, 2, value, DataType.Int32);
        }

        /// <summary>
        /// Sets the value of the current credit amount attribute.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="value">Current credit amount</param>
        /// <returns>Action bytes.</returns>
        /// <summary>
        public byte[][] InvokeCredit(GXDLMSClient client, CreditStatus value)
        {
            return client.Method(this, 3, (byte) value, DataType.UInt8);
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    CurrentCreditAmount += Convert.ToInt32(e.Value);
                    break;
                case 2:
                    CurrentCreditAmount = Convert.ToInt32(e.Value);
                    break;
                case 3:
                    if ((CreditConfiguration & CreditConfiguration.Confirmation) != 0 &&  Status == CreditStatus.Selectable)
                    {
                        Status = CreditStatus.Invoked;
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
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
            //CurrentCreditAmount
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //Type
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //Priority
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //WarningThreshold
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //Limit
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //creditConfiguration
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //Status
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //PresetCreditAmount
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //CreditAvailableThreshold
            if (all || CanRead(10))
            {
                attributes.Add(10);
            }
            //Period
            if (all || CanRead(11))
            {
                attributes.Add(11);
            }
            return attributes.ToArray();
        }

        public override DataType GetUIDataType(int index)
        {
            //Period
            if (index == 11)
            {
                return DataType.DateTime;
            }
            return base.GetUIDataType(index);
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "CurrentCreditAmount", "Type",
                "Priority", "WarningThreshold", "Limit", "CreditConfiguration", "Status",
                "PresetCreditAmount", "CreditAvailableThreshold", "Period"
            };
        }
        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Update amount", "Set amount to value", "Invoke credit" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 11;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 3;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.Int32;
                case 3:
                    return DataType.Enum;
                case 4:
                    return DataType.UInt8;
                case 5:
                    return DataType.Int32;
                case 6:
                    return DataType.Int32;
                case 7:
                    return DataType.BitString;
                case 8:
                    return DataType.Enum;
                case 9:
                    return DataType.Int32;
                case 10:
                    return DataType.Int32;
                case 11:
                    return DataType.OctetString;
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
                    return CurrentCreditAmount;
                case 3:
                    return Type;
                case 4:
                    return Priority;
                case 5:
                    return WarningThreshold;
                case 6:
                    return Limit;
                case 7:
                    return GXBitString.ToBitString((byte) CreditConfiguration, 5);
                case 8:
                    return Status;
                case 9:
                    return PresetCreditAmount;
                case 10:
                    return CreditAvailableThreshold;
                case 11:
                    return Period;
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
                    CurrentCreditAmount = (int)e.Value;
                    break;
                case 3:
                    Type = (CreditType)Convert.ToByte(e.Value);
                    break;
                case 4:
                    Priority = (byte)e.Value;
                    break;
                case 5:
                    WarningThreshold = (int)e.Value;
                    break;
                case 6:
                    Limit = (int)e.Value;
                    break;
                case 7:
                    CreditConfiguration = (CreditConfiguration)Convert.ToByte(e.Value);
                    break;
                case 8:
                    Status = (CreditStatus)Convert.ToByte(e.Value);
                    break;
                case 9:
                    PresetCreditAmount = (int)e.Value;
                    break;
                case 10:
                    CreditAvailableThreshold = (int)e.Value;
                    break;
                case 11:
                    if (e.Value == null)
                    {
                        Period = new GXDateTime(DateTime.MinValue);
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
                            Period = (GXDateTime)e.Value;
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
            CurrentCreditAmount = reader.ReadElementContentAsInt("CurrentCreditAmount");
            Type = (CreditType)reader.ReadElementContentAsInt("Type");
            Priority = (byte)reader.ReadElementContentAsInt("Priority");
            WarningThreshold = reader.ReadElementContentAsInt("WarningThreshold");
            Limit = reader.ReadElementContentAsInt("Limit");
            CreditConfiguration = (CreditConfiguration)reader.ReadElementContentAsInt("CreditConfiguration");
            Status = (CreditStatus)reader.ReadElementContentAsInt("Status");
            PresetCreditAmount = reader.ReadElementContentAsInt("PresetCreditAmount");
            CreditAvailableThreshold = reader.ReadElementContentAsInt("CreditAvailableThreshold");
            string str = reader.ReadElementContentAsString("Period");
            if (str != null)
            {
                Period = new GXDateTime(str, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("CurrentCreditAmount", CurrentCreditAmount, 2);
            writer.WriteElementString("Type", (byte)Type, 3);
            writer.WriteElementString("Priority", Priority, 4);
            writer.WriteElementString("WarningThreshold", WarningThreshold, 5);
            writer.WriteElementString("Limit", Limit, 6);
            writer.WriteElementString("CreditConfiguration", (byte)CreditConfiguration, 7);
            writer.WriteElementString("Status", (byte)Status, 8);
            writer.WriteElementString("PresetCreditAmount", PresetCreditAmount, 9);
            writer.WriteElementString("CreditAvailableThreshold", CreditAvailableThreshold, 10);
            writer.WriteElementString("Period", Period, 11);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
