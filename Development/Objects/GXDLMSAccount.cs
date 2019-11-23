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

using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Enumerates payment Modes.
    /// </summary>
    public enum PaymentMode : byte
    {
        /// <summary>
        /// Credit mode.
        /// </summary>
        Credit = 1,
        /// <summary>
        /// Prepayment mode.
        /// </summary>
        Prepayment = 2
    }

    public enum AccountStatus : byte
    {
        /// <summary>
        /// New (inactive) account.
        /// </summary>
        NewInactiveAccount = 1,
        /// <summary>
        /// Account active.
        /// </summary>
        AccountActive = 2,
        /// <summary>
        /// Account closed.
        /// </summary>
        AccountClosed = 3
    }

    /// <summary>
    /// Credit status.
    /// </summary>
    [Flags]
    public enum AccountCreditStatus
    {
        /// <summary>
        /// In credit.
        /// </summary>
        InCredit = 0x80,
        /// <summary>
        /// Low credit.
        /// </summary>
        LowCredit = 0x40,
        /// <summary>
        /// Next credit enabled.
        /// </summary>
        NextCreditEnabled = 0x20,
        /// <summary>
        /// Next credit selectable.
        /// </summary>
        NextCreditSelectable = 0x10,
        /// <summary>
        /// Credit reference list.
        /// </summary>
        CreditReferenceList = 0x8,
        /// <summary>
        /// Selectable credit in use.
        /// </summary>
        SelectableCreditInUse = 0x4,
        /// <summary>
        /// Out of credit.
        /// </summary>
        OutOfCredit = 0x2,
        /// <summary>
        /// Reserved.
        /// </summary>
        Reserved = 0x1
    }

    /// <summary>
    /// Defines behaviour under specific conditions
    /// </summary>
    [Flags]
    public enum CreditCollectionConfiguration
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// Collect when supply disconnected.
        /// </summary>
        Disconnected = 0x80,
        /// <summary>
        /// Collect in load limiting periods.
        /// </summary>
        LoadLimiting = 0x40,
        /// <summary>
        /// Collect in friendly credit periods.
        /// </summary>
        FriendlyCredit = 0x20
    }

    public class GXCreditChargeConfiguration
    {
        public string CreditReference
        {
            get;
            set;
        }

        public string ChargeReference
        {
            get;
            set;
        }

        public CreditCollectionConfiguration CollectionConfiguration
        {
            get;
            set;
        }
    }

    public class GXTokenGatewayConfiguration
    {
        public string CreditReference
        {
            get;
            set;
        }

        public byte TokenProportion
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Used currency.
    /// </summary>
    public enum Currency : byte
    {
        /// <summary>
        /// Time.
        /// </summary>
        Time,
        /// <summary>
        /// Consumption.
        /// </summary>
        Consumption,
        /// <summary>
        /// Monetary.
        /// </summary>
        Monetary
    }

    /// <summary>
    /// Used currency.
    /// </summary>
    public class GXCurrency
    {
        /// <summary>
        /// Currency name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Currency scale.
        /// </summary>
        public sbyte Scale
        {
            get;
            set;
        }

        /// <summary>
        /// Currency unit.
        /// </summary>
        public Currency Unit
        {
            get;
            set;
        }
    }

    /// <summary>
    ///  Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
    /// </summary>
    public class GXDLMSAccount : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAccount()
        : this("0.0.19.0.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSAccount(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSAccount(string ln, ushort sn)
        : base(ObjectType.Account, ln, sn)
        {
            PaymentMode = PaymentMode.Credit;
            AccountStatus = AccountStatus.NewInactiveAccount;
            CreditReferences = new List<string>();
            ChargeReferences = new List<string>();
            CreditChargeConfigurations = new List<GXCreditChargeConfiguration>();
            TokenGatewayConfigurations = new List<GXTokenGatewayConfiguration>();
            Currency = new GXCurrency();
        }

        /// <summary>
        /// Payment mode.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public PaymentMode PaymentMode
        {
            get;
            set;
        }

        /// <summary>
        /// Account status.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public AccountStatus AccountStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Index into the credit reference list indicating which Credit object is In use
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public byte CurrentCreditInUse

        {
            get;
            set;
        }

        /// <summary>
        /// Current credit status.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public AccountCreditStatus CurrentCreditStatus
        {
            get;
            set;
        }

        /// <summary>
        /// The available_credit attribute is the sum of the positive current credit amount values in the instances of the Credit class.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public int AvailableCredit
        {
            get;
            set;
        }

        /// <summary>
        /// Amount to clear.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public int AmountToClear
        {
            get;
            set;
        }

        /// <summary>
        /// Conjunction with the amount to clear, and is included in the description of that attribute.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public int ClearanceThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// Simple sum of total_amount_remaining of all the Charge objects which are listed in the Account object.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public int AggregatedDebt
        {
            get;
            set;
        }

        /// <summary>
        /// Credit references.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public List<string> CreditReferences
        {
            get;
            set;
        }

        /// <summary>
        /// Charge references.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public List<string> ChargeReferences
        {
            get;
            set;
        }

        /// <summary>
        /// Credit charge configurations
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public List<GXCreditChargeConfiguration> CreditChargeConfigurations
        {
            get;
            set;
        }

        /// <summary>
        /// Token gateway configurations.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public List<GXTokenGatewayConfiguration> TokenGatewayConfigurations
        {
            get;
            set;
        }

        /// <summary>
        /// Account activation time.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public GXDateTime AccountActivationTime
        {
            get;
            set;
        }

        /// <summary>
        /// Account closure time.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public GXDateTime AccountClosureTime
        {
            get;
            set;
        }

        /// <summary>
        /// Currency settings.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public GXCurrency Currency
        {
            get;
            set;
        }

        /// <summary>
        /// Low credit threshold.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public int LowCreditThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// Next credit available threshold.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public int NextCreditAvailableThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// Max provision.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public UInt16 MaxProvision
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAccount
        /// </remarks>
        [XmlIgnore()]
        public int MaxProvisionPeriod
        {
            get;
            set;
        }


        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, new object[]{PaymentMode, AccountStatus }, CurrentCreditInUse,
       CurrentCreditStatus, AvailableCredit, AmountToClear, ClearanceThreshold,
        AggregatedDebt, CreditReferences, ChargeReferences, CreditChargeConfigurations,
TokenGatewayConfigurations, AccountActivationTime, AccountClosureTime, Currency,
LowCreditThreshold, NextCreditAvailableThreshold, MaxProvision, MaxProvisionPeriod };
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
            //PaymentMode, AccountStatus
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //CurrentCreditInUse
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //CurrentCreditStatus
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //AvailableCredit
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //AmountToClear
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //ClearanceThreshold
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //AggregatedDebt
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //CreditReferences
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //ChargeReferences
            if (all || CanRead(10))
            {
                attributes.Add(10);
            }
            //CreditChargeConfigurations
            if (all || CanRead(11))
            {
                attributes.Add(11);
            }
            //TokenGatewayConfigurations
            if (all || CanRead(12))
            {
                attributes.Add(12);
            }
            //AccountActivationTime
            if (all || CanRead(13))
            {
                attributes.Add(13);
            }
            //AccountClosureTime
            if (all || CanRead(14))
            {
                attributes.Add(14);
            }
            //Currency
            if (all || CanRead(15))
            {
                attributes.Add(15);
            }
            //LowCreditThreshold
            if (all || CanRead(16))
            {
                attributes.Add(16);
            }
            //NextCreditAvailableThreshold
            if (all || CanRead(17))
            {
                attributes.Add(17);
            }
            //MaxProvision
            if (all || CanRead(18))
            {
                attributes.Add(18);
            }
            //MaxProvisionPeriod
            if (all || CanRead(19))
            {
                attributes.Add(19);
            }
            return attributes.ToArray();
        }

        public override DataType GetUIDataType(int index)
        {
            //AccountActivationTime or AccountClosureTime
            if (index == 13 || index == 14)
            {
                return DataType.DateTime;
            }
            return base.GetUIDataType(index);
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(),"PaymentMode", "AccountStatus", "CurrentCreditInUse",
        "CurrentCreditStatus", "AvailableCredit", "AmountToClear", "ClearanceThreshold",
        "AggregatedDebt", "CreditReferences", "ChargeReferences", "CreditChargeConfigurations",
                "TokenGatewayConfigurations", "AccountActivationTime", "AccountClosureTime", "Currency",
        "LowCreditThreshold", "NextCreditAvailableThreshold", "MaxProvision", "MaxProvisionPeriod"};
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 19;
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
                    return DataType.Structure;
                case 3:
                    return DataType.UInt8;
                case 4:
                    return DataType.BitString;
                case 5:
                    return DataType.Int32;
                case 6:
                    return DataType.Int32;
                case 7:
                    return DataType.Int32;
                case 8:
                    return DataType.Int32;
                case 9:
                    return DataType.Array;
                case 10:
                    return DataType.Array;
                case 11:
                    return DataType.Array;
                case 12:
                    return DataType.Array;
                case 13:
                    return DataType.OctetString;
                case 14:
                    return DataType.OctetString;
                case 15:
                    return DataType.Structure;
                case 16:
                    return DataType.Int32;
                case 17:
                    return DataType.Int32;
                case 18:
                    return DataType.UInt16;
                case 19:
                    return DataType.Int32;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            GXByteBuffer bb;
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    bb = new GXByteBuffer();
                    bb.SetUInt8(DataType.Structure);
                    bb.SetUInt8(2);
                    bb.SetUInt8(DataType.Enum);
                    bb.SetUInt8(AccountStatus);
                    bb.SetUInt8(DataType.Enum);
                    bb.SetUInt8(PaymentMode);
                    return bb.Array();
                case 3:
                    return CurrentCreditInUse;
                case 4:
                    return (byte) CurrentCreditStatus;
                case 5:
                    return AvailableCredit;
                case 6:
                    return AmountToClear;
                case 7:
                    return ClearanceThreshold;
                case 8:
                    return AggregatedDebt;
                case 9:
                    bb = new GXByteBuffer();
                    bb.SetUInt8(DataType.Array);
                    if (CreditReferences == null)
                    {
                        bb.SetUInt8(0);
                    }
                    else
                    {
                        GXCommon.SetObjectCount(CreditReferences.Count, bb);
                        foreach (string it in CreditReferences)
                        {
                            bb.SetUInt8(DataType.OctetString);
                            bb.SetUInt8(6);
                            bb.Set(GXCommon.LogicalNameToBytes(it));
                        }
                    }
                    return bb.Array();
                case 10:
                    bb = new GXByteBuffer();
                    bb.SetUInt8(DataType.Array);
                    if (ChargeReferences == null)
                    {
                        bb.SetUInt8(0);
                    }
                    else
                    {
                        GXCommon.SetObjectCount(ChargeReferences.Count, bb);
                        foreach (string it in ChargeReferences)
                        {
                            bb.SetUInt8(DataType.OctetString);
                            bb.SetUInt8(6);
                            bb.Set(GXCommon.LogicalNameToBytes(it));
                        }
                    }
                    return bb.Array();
                case 11:
                    bb = new GXByteBuffer();
                    bb.SetUInt8(DataType.Array);
                    if (CreditChargeConfigurations == null)
                    {
                        bb.SetUInt8(0);
                    }
                    else
                    {
                        GXCommon.SetObjectCount(CreditChargeConfigurations.Count, bb);
                        foreach (GXCreditChargeConfiguration it in CreditChargeConfigurations)
                        {
                            bb.SetUInt8(DataType.Structure);
                            bb.SetUInt8(3);
                            bb.SetUInt8(DataType.OctetString);
                            bb.SetUInt8(6);
                            bb.Set(GXCommon.LogicalNameToBytes(it.CreditReference));
                            bb.SetUInt8(DataType.OctetString);
                            bb.SetUInt8(6);
                            bb.Set(GXCommon.LogicalNameToBytes(it.ChargeReference));
                            bb.SetUInt8(DataType.BitString);
                            bb.SetUInt8(3);
                            bb.SetUInt8(it.CollectionConfiguration);
                        }
                    }
                    return bb.Array();
                case 12:
                    bb = new GXByteBuffer();
                    bb.SetUInt8(DataType.Array);
                    if (TokenGatewayConfigurations == null)
                    {
                        bb.SetUInt8(0);
                    }
                    else
                    {
                        GXCommon.SetObjectCount(TokenGatewayConfigurations.Count, bb);
                        foreach (GXTokenGatewayConfiguration it in TokenGatewayConfigurations)
                        {
                            bb.SetUInt8(DataType.Structure);
                            bb.SetUInt8(2);
                            bb.SetUInt8(DataType.OctetString);
                            bb.SetUInt8(6);
                            bb.Set(GXCommon.LogicalNameToBytes(it.CreditReference));
                            bb.SetUInt8(DataType.UInt8);
                            bb.SetUInt8(it.TokenProportion);
                        }
                    }
                    return bb.Array();
                case 13:
                    return AccountActivationTime;
                case 14:
                    return AccountClosureTime;
                case 15:
                    bb = new GXByteBuffer();
                    bb.SetUInt8(DataType.Structure);
                    bb.SetUInt8(3);
                    GXCommon.SetData(settings, bb, DataType.StringUTF8, Currency.Name);
                    GXCommon.SetData(settings, bb, DataType.Int8, Currency.Scale);
                    GXCommon.SetData(settings, bb, DataType.Enum, Currency.Unit);
                    return bb.Array();
                case 16:
                    return LowCreditThreshold;
                case 17:
                    return NextCreditAvailableThreshold;
                case 18:
                    return MaxProvision;
                case 19:
                    return MaxProvisionPeriod;
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
                        AccountStatus = (AccountStatus)Convert.ToByte(arr[0]);
                        PaymentMode = (PaymentMode)Convert.ToByte(arr[1]);
                    }
                    else
                    {
                        AccountStatus = AccountStatus.NewInactiveAccount;
                        PaymentMode = PaymentMode.Credit;
                    }
                    break;
                case 3:
                    CurrentCreditInUse = (byte)e.Value;
                    break;
                case 4:
                    CurrentCreditStatus = (AccountCreditStatus)Convert.ToByte(e.Value);
                    break;
                case 5:
                    AvailableCredit = (int)e.Value;
                    break;
                case 6:
                    AmountToClear = (int)e.Value;
                    break;
                case 7:
                    ClearanceThreshold = (int)e.Value;
                    break;
                case 8:
                    AggregatedDebt = (int)e.Value;
                    break;
                case 9:
                    CreditReferences.Clear();
                    if (e.Value != null)
                    {
                        foreach (object it in (IEnumerable<object>)e.Value)
                        {
                            CreditReferences.Add(GXCommon.ToLogicalName(it));
                        }
                    }
                    break;
                case 10:
                    ChargeReferences.Clear();
                    if (e.Value != null)
                    {
                        foreach (object it in (IEnumerable<object>)e.Value)
                        {
                            ChargeReferences.Add(GXCommon.ToLogicalName(it));
                        }
                    }
                    break;
                case 11:
                    CreditChargeConfigurations.Clear();
                    if (e.Value != null)
                    {
                        foreach (object tmp in (IEnumerable<object>)e.Value)
                        {
                            List<object> it;
                            if (tmp is List<object>)
                            {
                                it = (List<object>)tmp;
                            }
                            else
                            {
                                it = new List<object>((object[])tmp);
                            }
                            GXCreditChargeConfiguration item = new GXCreditChargeConfiguration();
                            item.CreditReference = GXCommon.ToLogicalName(it[0]);
                            item.ChargeReference = GXCommon.ToLogicalName(it[1]);
                            item.CollectionConfiguration = (CreditCollectionConfiguration)Convert.ToByte(it[2]);
                            CreditChargeConfigurations.Add(item);
                        }
                    }
                    break;
                case 12:
                    TokenGatewayConfigurations.Clear();
                    if (e.Value != null)
                    {
                        foreach (object tmp in (IEnumerable<object>)e.Value)
                        {
                            List<object> it;
                            if (tmp is List<object>)
                            {
                                it = (List<object>)tmp;
                            }
                            else
                            {
                                it = new List<object>((object[])tmp);
                            }
                            GXTokenGatewayConfiguration item = new GXTokenGatewayConfiguration();
                            item.CreditReference = GXCommon.ToLogicalName(it[0]);
                            item.TokenProportion = (byte)it[1];
                            TokenGatewayConfigurations.Add(item);
                        }
                    }
                    break;
                case 13:
                    if (e.Value == null)
                    {
                        AccountActivationTime = new GXDateTime(DateTime.MinValue);
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
                            AccountActivationTime = (GXDateTime)e.Value;
                        }
                    }
                    break;
                case 14:
                    if (e.Value == null)
                    {
                        AccountClosureTime = new GXDateTime(DateTime.MinValue);
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
                            AccountClosureTime = (GXDateTime)e.Value;
                        }
                    }
                    break;
                case 15:
                    if (e.Value != null)
                    {
                        List<object> tmp;
                        if (e.Value is List<object>)
                        {
                            tmp = (List<object>)e.Value;
                        }
                        else
                        {
                            tmp = new List<object>((object[])e.Value);
                        }
                        Currency.Name = (string)tmp[0];
                        Currency.Scale = (sbyte)tmp[1];
                        Currency.Unit = (Currency)Convert.ToByte(tmp[2]);
                    }
                    else
                    {
                        Currency.Name = null;
                        Currency.Scale = 0;
                        Currency.Unit = 0;
                    }
                    break;
                case 16:
                    LowCreditThreshold = (int)e.Value;
                    break;
                case 17:
                    NextCreditAvailableThreshold = (int)e.Value;
                    break;
                case 18:
                    MaxProvision = (UInt16)e.Value;
                    break;
                case 19:
                    MaxProvisionPeriod = (int)e.Value;
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        private static void LoadReferences(GXXmlReader reader, string name, List<string> list)
        {
            list.Clear();
            if (reader.IsStartElement(name, true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    list.Add(reader.ReadElementContentAsString("Name"));
                }
                reader.ReadEndElement(name);
            }
        }

        private static void LoadCreditChargeConfigurations(GXXmlReader reader, List<GXCreditChargeConfiguration> list)
        {
            list.Clear();
            if (reader.IsStartElement("CreditChargeConfigurations", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXCreditChargeConfiguration it = new GXCreditChargeConfiguration();
                    it.CreditReference = reader.ReadElementContentAsString("Credit");
                    it.ChargeReference = reader.ReadElementContentAsString("Charge");
                    it.CollectionConfiguration = (CreditCollectionConfiguration) reader.ReadElementContentAsInt("Configuration");
                    list.Add(it);
                }
                reader.ReadEndElement("CreditChargeConfigurations");
            }
        }
        private static void LoadTokenGatewayConfigurations(GXXmlReader reader, List<GXTokenGatewayConfiguration> list)
        {
            list.Clear();
            if (reader.IsStartElement("TokenGatewayConfigurations", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXTokenGatewayConfiguration it = new GXTokenGatewayConfiguration();
                    it.CreditReference = reader.ReadElementContentAsString("Credit");
                    it.TokenProportion = (byte)reader.ReadElementContentAsInt("Token");
                    list.Add(it);
                }
                reader.ReadEndElement("TokenGatewayConfigurations");
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            PaymentMode = (PaymentMode)reader.ReadElementContentAsInt("PaymentMode");
            AccountStatus = (AccountStatus)reader.ReadElementContentAsInt("AccountStatus");
            CurrentCreditInUse = (byte)reader.ReadElementContentAsInt("CurrentCreditInUse");
            CurrentCreditStatus = (AccountCreditStatus) reader.ReadElementContentAsInt("CurrentCreditStatus");
            AvailableCredit = reader.ReadElementContentAsInt("AvailableCredit");
            AmountToClear = reader.ReadElementContentAsInt("AmountToClear");
            ClearanceThreshold = reader.ReadElementContentAsInt("ClearanceThreshold");
            AggregatedDebt = reader.ReadElementContentAsInt("AggregatedDebt");
            LoadReferences(reader, "CreditReferences", CreditReferences);
            LoadReferences(reader, "ChargeReferences", ChargeReferences);
            LoadCreditChargeConfigurations(reader, CreditChargeConfigurations);
            LoadTokenGatewayConfigurations(reader, TokenGatewayConfigurations);
            string tmp = reader.ReadElementContentAsString("AccountActivationTime");
            if (!string.IsNullOrEmpty(tmp))
            {
                AccountActivationTime = new GXDateTime(tmp, System.Globalization.CultureInfo.InvariantCulture);
            }
            tmp = reader.ReadElementContentAsString("AccountClosureTime");
            if (!string.IsNullOrEmpty(tmp))
            {
                AccountClosureTime = new GXDateTime(tmp, System.Globalization.CultureInfo.InvariantCulture);
            }

            Currency.Name = reader.ReadElementContentAsString("CurrencyName");
            Currency.Scale = (sbyte)reader.ReadElementContentAsInt("CurrencyScale");
            Currency.Unit = (Currency)reader.ReadElementContentAsInt("CurrencyUnit");
            LowCreditThreshold = reader.ReadElementContentAsInt("LowCreditThreshold");
            NextCreditAvailableThreshold = reader.ReadElementContentAsInt("NextCreditAvailableThreshold");
            MaxProvision = (UInt16)reader.ReadElementContentAsInt("MaxProvision");
            MaxProvisionPeriod = reader.ReadElementContentAsInt("MaxProvisionPeriod");
        }

        private void SaveReferences(GXXmlWriter writer, List<string> list, string name)
        {
            if (list != null)
            {
                writer.WriteStartElement(name);
                foreach (string it in list)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Name", it);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void SaveCreditChargeConfigurations(GXXmlWriter writer, List<GXCreditChargeConfiguration> list)
        {
            if (list != null)
            {
                writer.WriteStartElement("CreditChargeConfigurations");
                foreach (GXCreditChargeConfiguration it in list)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Credit", it.CreditReference);
                    writer.WriteElementString("Charge", it.ChargeReference);
                    writer.WriteElementString("Configuration", (int)it.CollectionConfiguration);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void SaveTokenGatewayConfigurations(GXXmlWriter writer, List<GXTokenGatewayConfiguration> list)
        {
            if (list != null)
            {
                writer.WriteStartElement("TokenGatewayConfigurations");
                foreach (GXTokenGatewayConfiguration it in list)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Credit", it.CreditReference);
                    writer.WriteElementString("Token", it.TokenProportion);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }
        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("PaymentMode", (int)PaymentMode);
            writer.WriteElementString("AccountStatus", (int)AccountStatus);
            writer.WriteElementString("CurrentCreditInUse", CurrentCreditInUse);
            writer.WriteElementString("CurrentCreditStatus", (byte) CurrentCreditStatus);
            writer.WriteElementString("AvailableCredit", AvailableCredit);
            writer.WriteElementString("AmountToClear", AmountToClear);
            writer.WriteElementString("ClearanceThreshold", ClearanceThreshold);
            writer.WriteElementString("AggregatedDebt", AggregatedDebt);
            SaveReferences(writer, CreditReferences, "CreditReferences");
            SaveReferences(writer, ChargeReferences, "ChargeReferences");
            SaveCreditChargeConfigurations(writer, CreditChargeConfigurations);
            SaveTokenGatewayConfigurations(writer, TokenGatewayConfigurations);
            if (AccountActivationTime != null)
            {
                writer.WriteElementString("AccountActivationTime", AccountActivationTime.ToFormatString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteElementString("AccountActivationTime", "");
            }
            if (AccountClosureTime != null)
            {
                writer.WriteElementString("AccountClosureTime", AccountClosureTime.ToFormatString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteElementString("AccountClosureTime", "");
            }
            writer.WriteElementString("CurrencyName", Currency.Name);
            writer.WriteElementString("CurrencyScale", Currency.Scale);
            writer.WriteElementString("CurrencyUnit", (int)Currency.Unit);

            writer.WriteElementString("LowCreditThreshold", LowCreditThreshold);
            writer.WriteElementString("NextCreditAvailableThreshold", NextCreditAvailableThreshold);

            writer.WriteElementString("MaxProvision", MaxProvision);
            writer.WriteElementString("MaxProvisionPeriod", MaxProvisionPeriod);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
