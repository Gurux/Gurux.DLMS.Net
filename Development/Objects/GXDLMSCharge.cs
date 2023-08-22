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
using System.Text;
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Charge per unit scaling.
    /// </summary>
    /// <remarks>
    ///  Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
    /// </remarks>
    public class GXChargePerUnitScaling
    {
        /// <summary>
        /// Commodity scale.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        public sbyte CommodityScale
        {
            get;
            set;
        }

        /// <summary>
        /// Price scale.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        public sbyte PriceScale
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Commodity.
    /// </summary>
    /// <remarks>
    ///  Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
    /// </remarks>
    public class GXCommodity
    {
        /// <summary>
        /// Executed object
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        public GXDLMSObject Target
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute index.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        public int Index
        {
            get;
            set;
        }
    };

    /// <summary>
    /// Charge table.
    /// </summary>
    /// <remarks>
    ///  Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
    /// </remarks>
    public class GXChargeTable
    {
        /// <summary>
        /// Index.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        public string Index
        {
            get;
            set;
        }

        /// <summary>
        /// Charge per unit.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        public short ChargePerUnit
        {
            get;
            set;
        }
    }


    /// <summary>
    ///  Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
    /// </summary>
    public class GXUnitCharge
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXUnitCharge()
        {
            ChargePerUnitScaling = new GXChargePerUnitScaling();
            Commodity = new GXCommodity();
        }

        /// <summary>
        /// Charge per unit scaling.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        public GXChargePerUnitScaling ChargePerUnitScaling
        {
            get;
            set;
        }

        /// <summary>
        /// Commodity.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        public GXCommodity Commodity
        {
            get;
            set;
        }

        /// <summary>
        /// Charge tables.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        public GXChargeTable[] ChargeTables
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Charge type.
    /// </summary>
    /// <remarks>
    ///  Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
    /// </remarks>
    public enum ChargeType : byte
    {
        /// <summary>
        /// Consumption based collection.
        /// </summary>
        ConsumptionBasedCollection,
        /// <summary>
        /// Time based collection.
        /// </summary>
        TimeBasedCollection,
        /// <summary>
        /// Payment based collection.
        /// </summary>
        PaymentEventBasedCollection
    }

    /// <summary>
    ///  Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
    /// </summary>
    public class GXDLMSCharge : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSCharge()
        : this("0.0.19.20.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSCharge(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSCharge(string ln, ushort sn)
        : base(ObjectType.Charge, ln, sn)
        {
            UnitChargeActive = new GXUnitCharge();
            UnitChargePassive = new GXUnitCharge();
        }

        /// <summary>
        /// Total amount paid
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public Int32 TotalAmountPaid
        {
            get;
            set;
        }

        /// <summary>
        /// Charge type.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public ChargeType ChargeType
        {
            get;
            set;
        }
        /// <summary>
        /// Priority.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public byte Priority
        {
            get;
            set;
        }
        /// <summary>
        /// Unit charge active.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public GXUnitCharge UnitChargeActive

        {
            get;
            set;
        }
        /// <summary>
        /// Unit charge passive.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public GXUnitCharge UnitChargePassive
        {
            get;
            set;
        }
        /// <summary>
        /// Unit charge activation time.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public GXDateTime UnitChargeActivationTime
        {
            get;
            set;
        }
        /// <summary>
        /// Period.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public UInt32 Period
        {
            get;
            set;
        }
        /// <summary>
        /// Charge configuration,
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public ChargeConfiguration ChargeConfiguration
        {
            get;
            set;
        }
        /// <summary>
        /// Last collection time.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public GXDateTime LastCollectionTime
        {
            get;
            set;
        }
        /// <summary>
        /// Last collection amount.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public Int32 LastCollectionAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Total amount remaining.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public Int32 TotalAmountRemaining
        {
            get;
            set;
        }

        /// <summary>
        /// Proportion.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public UInt16 Proportion
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName,  TotalAmountPaid, ChargeType, Priority,
    UnitChargeActive, UnitChargePassive, UnitChargeActivationTime, Period,
    ChargeConfiguration, LastCollectionTime, LastCollectionAmount, TotalAmountRemaining, Proportion };
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
            //TotalAmountPaid
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //ChargeType
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //Priority
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //UnitChargeActive
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //UnitChargePassive
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //UnitChargeActivationTime
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //Period
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //ChargeConfiguration
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //LastCollectionTime
            if (all || CanRead(10))
            {
                attributes.Add(10);
            }
            //LastCollectionAmount
            if (all || CanRead(11))
            {
                attributes.Add(11);
            }
            //TotalAmountRemaining
            if (all || CanRead(12))
            {
                attributes.Add(12);
            }
            //Proportion
            if (all || CanRead(13))
            {
                attributes.Add(13);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "TotalAmountPaid", "ChargeType", "Priority",
    "UnitChargeActive", "UnitChargePassive", "UnitChargeActivationTime", "Period",
    "ChargeConfiguration", "LastCollectionTime", "LastCollectionAmount", "TotalAmountRemaining", "Proportion"};
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] {"Update unit charge", "Activate passive unit charge",
                "Collect", "Update total amount remaining", "Set total amount remaining" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }
        int IGXDLMSBase.GetAttributeCount()
        {
            return 13;
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
                    return DataType.Int32;
                case 3:
                    return DataType.Enum;
                case 4:
                    return DataType.UInt8;
                case 5:
                    return DataType.Structure;
                case 6:
                    return DataType.Structure;
                case 7:
                    return DataType.OctetString;
                case 8:
                    return DataType.UInt32;
                case 9:
                    return DataType.BitString;
                case 10:
                    return DataType.DateTime;
                case 11:
                    return DataType.Int32;
                case 12:
                    return DataType.Int32;
                case 13:
                    return DataType.UInt16;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        private static byte[] GetUnitCharge(GXDLMSSettings settings, GXUnitCharge charge)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(3);
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            GXCommon.SetData(settings, bb, DataType.Int8, charge.ChargePerUnitScaling.CommodityScale);
            GXCommon.SetData(settings, bb, DataType.Int8, charge.ChargePerUnitScaling.PriceScale);
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(3);
            if (charge.Commodity.Target == null)
            {
                GXCommon.SetData(settings, bb, DataType.UInt16, 0);
                bb.SetUInt8(DataType.OctetString);
                bb.SetUInt8(6);
                bb.SetUInt8(0);
                bb.SetUInt8(0);
                bb.SetUInt8(0);
                bb.SetUInt8(0);
                bb.SetUInt8(0);
                bb.SetUInt8(0);
                GXCommon.SetData(settings, bb, DataType.Int8, 0);
            }
            else
            {
                GXCommon.SetData(settings, bb, DataType.UInt16, charge.Commodity.Target.ObjectType);
                GXCommon.SetData(settings, bb, DataType.OctetString, GXCommon.LogicalNameToBytes(charge.Commodity.Target.LogicalName));
                GXCommon.SetData(settings, bb, DataType.Int8, charge.Commodity.Index);
            }
            bb.SetUInt8(DataType.Array);
            if (charge.ChargeTables == null)
            {
                bb.SetUInt8(0);
            }
            else
            {
                GXCommon.SetObjectCount(charge.ChargeTables.Length, bb);
                foreach (GXChargeTable it in charge.ChargeTables)
                {
                    bb.SetUInt8(DataType.Structure);
                    bb.SetUInt8(2);
                    GXCommon.SetData(settings, bb, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Index));
                    GXCommon.SetData(settings, bb, DataType.Int16, it.ChargePerUnit);
                }
            }
            return bb.Array();
        }

        public override DataType GetUIDataType(int index)
        {
            if (index == 7 || index == 10)
            {
                return DataType.DateTime;
            }
            return base.GetUIDataType(index);
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return TotalAmountPaid;
                case 3:
                    return ChargeType;
                case 4:
                    return Priority;
                case 5:
                    return GetUnitCharge(settings, UnitChargeActive);
                case 6:
                    return GetUnitCharge(settings, UnitChargePassive);
                case 7:
                    return UnitChargeActivationTime;
                case 8:
                    return Period;
                case 9:
                    return GXBitString.ToBitString((UInt32)ChargeConfiguration, 2);
                case 10:
                    return LastCollectionTime;
                case 11:
                    return LastCollectionAmount;
                case 12:
                    return TotalAmountRemaining;
                case 13:
                    return Proportion;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return null;
        }

        private void SetUnitCharge(GXUnitCharge charge, object value)
        {
            if (value != null)
            {
                List<object> tmp, tmp2, it;
                if (value is List<object>)
                {
                    tmp = (List<object>)value;
                }
                else
                {
                    tmp = new List<object>((object[])value);
                }
                if (tmp[0] is List<object>)
                {
                    tmp2 = (List<object>)tmp[0];
                }
                else
                {
                    tmp2 = new List<object>((object[])tmp[0]);
                }
                charge.ChargePerUnitScaling.CommodityScale = (sbyte)tmp2[0];
                charge.ChargePerUnitScaling.PriceScale = (sbyte)tmp2[1];
                if (tmp[1] is List<object>)
                {
                    tmp2 = (List<object>)tmp[1];
                }
                else
                {
                    tmp2 = new List<object>((object[])tmp[1]);
                }
                ObjectType ot = (ObjectType)Convert.ToInt32(tmp2[0]);
                string ln = GXCommon.ToLogicalName(tmp2[1]);
                if (ot != ObjectType.None)
                {
                    if (Parent != null)
                    {
                        charge.Commodity.Target = Parent.FindByLN(ot, ln);
                    }
                    else
                    {
                        charge.Commodity.Target = GXDLMSClient.CreateObject(ot);
                        charge.Commodity.Target.LogicalName = ln;
                    }
                }
                else
                {
                    charge.Commodity.Target = null;
                }
                charge.Commodity.Index = (sbyte)tmp2[2];
                List<GXChargeTable> list = new List<GXChargeTable>();
                foreach (object tmp3 in (IEnumerable<object>)tmp[2])
                {
                    if (tmp3 is List<object>)
                    {
                        it = (List<object>)tmp3;
                    }
                    else
                    {
                        it = new List<object>((object[])tmp3);
                    }
                    GXChargeTable item = new GXChargeTable();
                    item.Index = ASCIIEncoding.ASCII.GetString((byte[])it[0]);
                    item.ChargePerUnit = (Int16)it[1];
                    list.Add(item);
                }
                charge.ChargeTables = list.ToArray();
            }
            else
            {
                charge.ChargePerUnitScaling.CommodityScale = 0;
                charge.ChargePerUnitScaling.PriceScale = 0;
                charge.Commodity.Target = null;
                charge.Commodity.Index = 0;
                charge.ChargeTables = new GXChargeTable[0];
            }
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    TotalAmountPaid = (Int32)e.Value;
                    break;
                case 3:
                    ChargeType = (ChargeType)Convert.ToByte(e.Value);
                    break;
                case 4:
                    Priority = (byte)e.Value;
                    break;
                case 5:
                    SetUnitCharge(UnitChargeActive, e.Value);
                    break;
                case 6:
                    SetUnitCharge(UnitChargePassive, e.Value);
                    break;
                case 7:
                    if (e.Value is GXDateTime)
                    {
                        UnitChargeActivationTime = (GXDateTime)e.Value;
                    }
                    else if (e.Value is byte[])
                    {
                        UnitChargeActivationTime = (GXDateTime)GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime, settings != null && settings.UseUtc2NormalTime);
                    }
                    else if (e.Value is string)
                    {
                        UnitChargeActivationTime = new GXDateTime((string)e.Value);
                    }
                    else
                    {
                        UnitChargeActivationTime = null;
                    }
                    break;
                case 8:
                    Period = (UInt32)e.Value;
                    break;
                case 9:
                    ChargeConfiguration = (ChargeConfiguration)Convert.ToInt32(e.Value);
                    break;
                case 10:
                    if (e.Value is GXDateTime)
                    {
                        LastCollectionTime = (GXDateTime)e.Value;
                    }
                    else if (e.Value is DateTime)
                    {
                        LastCollectionTime = (DateTime)e.Value;
                    }
                    else if (e.Value is byte[])
                    {
                        LastCollectionTime = (GXDateTime)GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime, settings != null && settings.UseUtc2NormalTime);
                    }
                    else if (e.Value is string)
                    {
                        LastCollectionTime = new GXDateTime(e.Value as string);
                    }
                    else
                    {
                        LastCollectionTime = null;
                    }
                    break;
                case 11:
                    LastCollectionAmount = (Int32)e.Value;
                    break;
                case 12:
                    TotalAmountRemaining = (Int32)e.Value;
                    break;
                case 13:
                    Proportion = (UInt16)e.Value;
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        private static void LoadUnitChargeActive(GXXmlReader reader, string name, GXUnitCharge charge)
        {
            if (reader.IsStartElement(name, true))
            {
                charge.ChargePerUnitScaling.CommodityScale = (sbyte)reader.ReadElementContentAsInt("Scale");
                charge.ChargePerUnitScaling.PriceScale = (sbyte)reader.ReadElementContentAsInt("PriceScale");
                ObjectType ot = (ObjectType)reader.ReadElementContentAsInt("Type");
                string ln = reader.ReadElementContentAsString("Ln");
                charge.Commodity.Target = reader.Objects.FindByLN(ot, ln);
                charge.Commodity.Index = reader.ReadElementContentAsInt("Index");
                List<GXChargeTable> list = new List<GXChargeTable>();
                if (reader.IsStartElement("ChargeTables", true))
                {
                    while (reader.IsStartElement("Item", true))
                    {
                        GXChargeTable it = new GXChargeTable();
                        it.Index = reader.ReadElementContentAsString("Index");
                        it.ChargePerUnit = (short)reader.ReadElementContentAsInt("ChargePerUnit");
                        list.Add(it);
                    }
                    reader.ReadEndElement("ChargeTables");
                }
                reader.ReadEndElement(name);
                charge.ChargeTables = list.ToArray();
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            TotalAmountPaid = reader.ReadElementContentAsInt("TotalAmountPaid");
            ChargeType = (ChargeType)reader.ReadElementContentAsInt("ChargeType");
            Priority = (byte)reader.ReadElementContentAsInt("Priority");
            LoadUnitChargeActive(reader, "UnitChargeActive", UnitChargeActive);
            LoadUnitChargeActive(reader, "UnitChargePassive", UnitChargePassive);
            string tmp = reader.ReadElementContentAsString("UnitChargeActivationTime");
            if (tmp != null)
            {
                UnitChargeActivationTime = new GXDateTime(tmp, System.Globalization.CultureInfo.InvariantCulture);
            }
            Period = (UInt16)reader.ReadElementContentAsInt("Period");
            ChargeConfiguration = (ChargeConfiguration)reader.ReadElementContentAsInt("ChargeConfiguration");
            tmp = reader.ReadElementContentAsString("LastCollectionTime");
            if (tmp != null)
            {
                LastCollectionTime = new GXDateTime(tmp, System.Globalization.CultureInfo.InvariantCulture);
            }
            LastCollectionAmount = reader.ReadElementContentAsInt("LastCollectionAmount");
            TotalAmountRemaining = reader.ReadElementContentAsInt("TotalAmountRemaining");
            Proportion = (UInt16)reader.ReadElementContentAsInt("Proportion");
        }

        private static void SaveUnitCharge(GXXmlWriter writer, string name, GXUnitCharge charge, int index)
        {
            writer.WriteStartElement(name, index);
            writer.WriteElementString("Scale", charge.ChargePerUnitScaling.CommodityScale, index);
            writer.WriteElementString("PriceScale", charge.ChargePerUnitScaling.PriceScale, index);
            if (charge.Commodity.Target != null)
            {
                writer.WriteElementString("Type", (int)charge.Commodity.Target.ObjectType, index);
                writer.WriteElementString("Ln", charge.Commodity.Target.LogicalName, index);
            }
            else
            {
                writer.WriteElementString("Type", 0, index);
                writer.WriteElementString("Ln", "0.0.0.0.0.0", index);
            }
            writer.WriteElementString("Index", charge.Commodity.Index, index);
            writer.WriteStartElement("ChargeTables", 0);
            if (charge.ChargeTables != null)
            {
                foreach (GXChargeTable it in charge.ChargeTables)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("Index", it.Index, index);
                    writer.WriteElementString("ChargePerUnit", it.ChargePerUnit, index);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("TotalAmountPaid", TotalAmountPaid, 2);
            writer.WriteElementString("ChargeType", (int)ChargeType, 3);
            writer.WriteElementString("Priority", Priority, 4);
            SaveUnitCharge(writer, "UnitChargeActive", UnitChargeActive, 5);
            SaveUnitCharge(writer, "UnitChargePassive", UnitChargePassive, 6);
            writer.WriteElementString("UnitChargeActivationTime", UnitChargeActivationTime, 7);
            writer.WriteElementString("Period", Period, 8);
            writer.WriteElementString("ChargeConfiguration", (int)ChargeConfiguration, 9);
            writer.WriteElementString("LastCollectionTime", LastCollectionTime, 10);
            writer.WriteElementString("LastCollectionAmount", LastCollectionAmount, 11);
            writer.WriteElementString("TotalAmountRemaining", TotalAmountRemaining, 12);
            writer.WriteElementString("Proportion", Proportion, 13);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
