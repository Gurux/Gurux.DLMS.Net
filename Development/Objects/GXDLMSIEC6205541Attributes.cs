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

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSIEC6205541Attributes
    /// </summary>
    public class GXDLMSIEC6205541Attributes : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSIEC6205541Attributes()
        : this("0.0.19.60.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSIEC6205541Attributes(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIEC6205541Attributes(string ln, ushort sn)
        : base(ObjectType.IEC6205541Attributes, ln, sn)
        {
            MeterPan = new GXDLMSMeterPrimaryAccountNumber();
        }

        /// <summary>
        /// Meter primary account number.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSMeterPrimaryAccountNumber MeterPan
        {
            get;
            set;
        }

        /// <summary>
        /// Meter commodity.
        /// </summary>
        /// <remarks>
        /// This can be ELECTRICITY, WATER, GAS, or TIME. 
        /// </remarks>
        public string Commodity
        {
            get;
            set;
        }

        /// <summary>
        /// Token carrier types
        /// </summary>
        public byte[] TokenCarrierTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Encryption algorithm.
        /// </summary>
        public byte EncryptionAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Supply group code.
        /// </summary>
        public UInt32 SupplyGroupCode
        {
            get;
            set;
        }

        /// <summary>
        /// Tariff index.
        /// </summary>
        public byte TariffIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Key revision number.
        /// </summary>
        public byte KeyRevisionNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Key type.
        /// </summary>
        public byte KeyType
        {
            get;
            set;
        }

        /// <summary>
        /// Key expiry number.
        /// </summary>
        public byte KeyExpiryNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Number of key change tokens supported by the serve.
        /// </summary>
        public byte KctSupported
        {
            get;
            set;
        }

        /// <summary>
        /// Conformance certificate number.
        /// </summary>
        public string StsCertificate
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, MeterPan, Commodity,
                TokenCarrierTypes,EncryptionAlgorithm, SupplyGroupCode,
            TariffIndex, KeyRevisionNumber, KeyType, KeyExpiryNumber,
            KctSupported, StsCertificate};
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
            //MeterPan
            if (all || !IsRead(2))
            {
                attributes.Add(2);
            }
            //Commodity
            if (all || !IsRead(3))
            {
                attributes.Add(3);
            }
            //Token carrier types.
            if (all || !IsRead(4))
            {
                attributes.Add(4);
            }
            //Encryption algorithm.
            if (all || !IsRead(5))
            {
                attributes.Add(5);
            }
            // Supply group code.
            if (all || !CanRead(6))
            {
                attributes.Add(6);
            }
            // Tariff index.
            if (all || !CanRead(7))
            {
                attributes.Add(7);
            }
            // Key revision number.
            if (all || !CanRead(8))
            {
                attributes.Add(8);
            }
            //Key type.
            if (all || !CanRead(9))
            {
                attributes.Add(9);
            }
            //Key expiry number.
            if (all || !CanRead(10))
            {
                attributes.Add(10);
            }
            // Kct supported.
            if (all || !base.IsRead(11))
            {
                attributes.Add(11);
            }
            //Sts sertificate.
            if (all || !base.IsRead(12))
            {
                attributes.Add(12);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(),
            "MeterPan", "Commodity", "TokenCarrierTypes",
                "EncryptionAlgorithm", "SupplyGroupCode",
                "TariffIndex", "KeyRevisionNumber", "KeyType", "KeyExpiryNumber",
                "KctSupported", "StsCertificate"};
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
            return 12;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.Structure;
                case 3:
                case 12:
                    return DataType.String;
                case 4:
                    return DataType.Array;
                case 5:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    return DataType.UInt8;
                case 6:
                    return DataType.UInt32;
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
                        data.SetUInt8(DataType.Structure);
                        data.SetUInt8(3);
                        GXCommon.SetData(settings, data, DataType.UInt32, MeterPan.IssuerId);
                        GXCommon.SetData(settings, data, DataType.UInt64, MeterPan.DecoderReferenceNumber);
                        GXCommon.SetData(settings, data, DataType.UInt8, MeterPan.PanCheckDigit);
                        return data.Array();
                    }
                case 3:
                    return Commodity;
                case 4:
                    {
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        if (TokenCarrierTypes == null)
                        {
                            //Object count is zero.
                            data.SetUInt8(0);
                        }
                        else
                        {
                            GXCommon.SetObjectCount(TokenCarrierTypes.Length, data);
                            foreach (byte it in TokenCarrierTypes)
                            {
                                GXCommon.SetData(settings, data, DataType.UInt8, it);
                            }
                        }
                        return data.Array();
                    }
                case 5:
                    return EncryptionAlgorithm;
                case 6:
                    return SupplyGroupCode;
                case 7:
                    return TariffIndex;
                case 8:
                    return KeyRevisionNumber;
                case 9:
                    return KeyType;
                case 10:
                    return KeyExpiryNumber;
                case 11:
                    return KctSupported;
                case 12:
                    return StsCertificate;
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
                        if (e.Value is GXStructure s)
                        {
                            MeterPan.IssuerId = (UInt32)s[0];
                            MeterPan.DecoderReferenceNumber = (UInt64)s[1];
                            MeterPan.PanCheckDigit = (byte)s[2];
                        }
                    }
                    break;
                case 3:
                    Commodity = (string)e.Value;
                    break;
                case 4:
                    {
                        List<byte> data = new List<byte>();
                        if (e.Value is List<object>)
                        {
                            foreach (object it in (List<object>)e.Value)
                            {
                                data.Add((byte)it);
                            }
                        }
                        TokenCarrierTypes = data.ToArray();
                    }
                    break;
                case 5:
                    EncryptionAlgorithm = (byte)e.Value;
                    break;
                case 6:
                    SupplyGroupCode = (UInt32)e.Value;
                    break;
                case 7:
                    TariffIndex = (byte)e.Value;
                    break;
                case 8:
                    KeyRevisionNumber = (byte)e.Value;
                    break;
                case 9:
                    KeyType = (byte)e.Value;
                    break;
                case 10:
                    KeyExpiryNumber = (byte)e.Value;
                    break;
                case 11:
                    KctSupported = (byte)e.Value;
                    break;
                case 12:
                    StsCertificate = (string)e.Value;
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            MeterPan.IssuerId = (UInt32)reader.ReadElementContentAsULong("IssuerId");
            MeterPan.DecoderReferenceNumber = reader.ReadElementContentAsULong("DecoderReferenceNumber");
            MeterPan.PanCheckDigit = (byte)reader.ReadElementContentAsInt("PanCheckDigit");
            Commodity = reader.ReadElementContentAsString("Commodity");
            List<byte> list = new List<byte>();
            if (reader.IsStartElement("TokenCarrierTypes", true))
            {
                while (reader.IsStartElement("Value", false))
                {
                    list.Add((byte)reader.ReadElementContentAsInt("Value"));
                }
                reader.ReadEndElement("TokenCarrierTypes");
            }
            TokenCarrierTypes = list.ToArray();
            EncryptionAlgorithm = (byte)reader.ReadElementContentAsInt("EncryptionAlgorithm");
            SupplyGroupCode = (UInt32)reader.ReadElementContentAsULong("SupplyGroupCode");
            TariffIndex = (byte)reader.ReadElementContentAsULong("TariffIndex");
            KeyRevisionNumber = (byte)reader.ReadElementContentAsULong("KeyRevisionNumber");
            KeyType = (byte)reader.ReadElementContentAsULong("KeyType");
            KeyExpiryNumber = (byte)reader.ReadElementContentAsULong("KeyExpiryNumber");
            KctSupported = (byte)reader.ReadElementContentAsULong("KctSupported");
            StsCertificate = reader.ReadElementContentAsString("StsCertificate");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("IssuerId", MeterPan.IssuerId, 2);
            writer.WriteElementString("DecoderReferenceNumber", MeterPan.DecoderReferenceNumber, 2);
            writer.WriteElementString("PanCheckDigit", MeterPan.PanCheckDigit, 2);
            writer.WriteElementString("Commodity", Commodity, 3);
            writer.WriteStartElement("TokenCarrierTypes", 4);
            if (TokenCarrierTypes != null)
            {
                foreach (byte it in TokenCarrierTypes)
                {
                    writer.WriteElementString("Value", Convert.ToString(it), 4);
                }
            }
            writer.WriteEndElement();
            writer.WriteElementString("EncryptionAlgorithm", EncryptionAlgorithm, 5);
            writer.WriteElementString("SupplyGroupCode", SupplyGroupCode, 6);
            writer.WriteElementString("TariffIndex", TariffIndex, 7);
            writer.WriteElementString("KeyRevisionNumber", KeyRevisionNumber, 8);
            writer.WriteElementString("KeyType", KeyType, 9);
            writer.WriteElementString("KeyExpiryNumber", KeyExpiryNumber, 10);
            writer.WriteElementString("KctSupported", KctSupported, 11);
            writer.WriteElementString("StsCertificate", StsCertificate, 12);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
