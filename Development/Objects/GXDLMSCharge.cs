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
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Charge per unit scaling.
    /// </summary>
    /// <remarks>
    ///  Online help:<br/>
    ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
    /// </remarks>
    public class GXChargePerUnitScaling
    {
        /// <summary>
        /// Commodity scale.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
    ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
    /// </remarks>
    public class GXCommodity
    {
        /// <summary>
        /// Executed object
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
    ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
    /// </remarks>
    public class GXChargeTable
    {
        /// <summary>
        /// Index.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        public short ChargePerUnit
        {
            get;
            set;
        }
    }


    /// <summary>
    ///  Online help:<br/>
    ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
    ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
    ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public string ChargeConfiguration
        {
            get;
            set;
        }
        /// <summary>
        /// Last collection time.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
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
        ///  http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCharge
        /// </remarks>
        [XmlIgnore()]
        public UInt16 Proportion
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
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

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //TotalAmountPaid
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //ChargeType
            if (CanRead(3))
            {
                attributes.Add(3);
            }
            //Priority
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            //UnitChargeActive
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            //UnitChargePassive
            if (CanRead(6))
            {
                attributes.Add(6);
            }
            //UnitChargeActivationTime
            if (CanRead(7))
            {
                attributes.Add(7);
            }
            //Period
            if (CanRead(8))
            {
                attributes.Add(8);
            }
            //ChargeConfiguration
            if (CanRead(9))
            {
                attributes.Add(9);
            }
            //LastCollectionTime
            if (CanRead(10))
            {
                attributes.Add(10);
            }
            //LastCollectionAmount
            if (CanRead(11))
            {
                attributes.Add(11);
            }
            //TotalAmountRemaining
            if (CanRead(12))
            {
                attributes.Add(12);
            }
            //Proportion
            if (CanRead(13))
            {
                attributes.Add(13);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "TotalAmountPaid", "ChargeType", "Priority",
    "UnitChargeActive", "UnitChargePassive", "UnitChargeActivationTime", "Period",
    "ChargeConfiguration", "LastCollectionTime", "LastCollectionAmount", "TotalAmountRemaining", "Proportion"};
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 13;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 5;
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
                    return DataType.OctetString;
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
                    GXCommon.SetData(settings, bb, DataType.OctetString, it.Index);
                    GXCommon.SetData(settings, bb, DataType.Int16, it.ChargePerUnit);
                }
            }
            return bb.Array();
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
                    return ChargeConfiguration;
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
            object[] tmp = (object[])value;
            object[] tmp2 = (object[])tmp[0];
            charge.ChargePerUnitScaling.CommodityScale = (sbyte)tmp2[0];
            charge.ChargePerUnitScaling.PriceScale = (sbyte)tmp2[1];
            tmp2 = (object[])tmp[1];
            ObjectType ot = (ObjectType)Convert.ToInt32(tmp2[0]);
            string ln = GXCommon.ToLogicalName(tmp2[1]);
            charge.Commodity.Target = Parent.FindByLN(ot, ln);
            charge.Commodity.Index = (sbyte)tmp2[2];
            List<GXChargeTable> list = new List<GXChargeTable>();
            tmp2 = (object[])tmp[2];
            foreach (object[] it in tmp2)
            {
                GXChargeTable item = new GXChargeTable();
                item.Index = ASCIIEncoding.ASCII.GetString((byte[])it[0]);
                item.ChargePerUnit = (Int16)it[1];
                list.Add(item);
            }
            charge.ChargeTables = list.ToArray();
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
                    ChargeType = (ChargeType)e.Value;
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
                    UnitChargeActivationTime = (GXDateTime)GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime);
                    break;
                case 8:
                    Period = (UInt32)e.Value;
                    break;
                case 9:
                    ChargeConfiguration = (string)e.Value;
                    break;
                case 10:
                    LastCollectionTime = (GXDateTime)GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime);
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
            ChargeConfiguration = reader.ReadElementContentAsString("ChargeConfiguration");
            tmp = reader.ReadElementContentAsString("LastCollectionTime");
            if (tmp != null)
            {
                LastCollectionTime = new GXDateTime(tmp, System.Globalization.CultureInfo.InvariantCulture);
            }
            LastCollectionAmount = reader.ReadElementContentAsInt("LastCollectionAmount");
            TotalAmountRemaining = reader.ReadElementContentAsInt("TotalAmountRemaining");
            Proportion = (UInt16)reader.ReadElementContentAsInt("Proportion");
        }

        private static void SaveUnitChargeActive(GXXmlWriter writer, string name, GXUnitCharge charge)
        {

        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("TotalAmountPaid", TotalAmountPaid);
            writer.WriteElementString("ChargeType", (int)ChargeType);
            writer.WriteElementString("Priority", Priority);
            SaveUnitChargeActive(writer, "UnitChargeActive", UnitChargeActive);
            SaveUnitChargeActive(writer, "UnitChargePassive", UnitChargePassive);
            writer.WriteElementString("UnitChargeActivationTime", UnitChargeActivationTime);
            writer.WriteElementString("Period", Period);
            writer.WriteElementString("ChargeConfiguration", ChargeConfiguration);
            writer.WriteElementString("LastCollectionTime", LastCollectionTime);
            writer.WriteElementString("LastCollectionAmount", LastCollectionAmount);
            writer.WriteElementString("TotalAmountRemaining", TotalAmountRemaining);
            writer.WriteElementString("Proportion", Proportion);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
