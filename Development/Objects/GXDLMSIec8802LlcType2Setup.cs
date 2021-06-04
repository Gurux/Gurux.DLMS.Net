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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXIec8802LlcType2Setup
    /// </summary>
    public class GXDLMSIec8802LlcType2Setup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSIec8802LlcType2Setup()
        : this("0.0.27.1.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSIec8802LlcType2Setup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIec8802LlcType2Setup(string ln, ushort sn)
        : base(ObjectType.Iec8802LlcType2Setup, ln, sn)
        {
            TransmitWindowSizeK = 1;
            TransmitWindowSizeRW = 1;
            MaximumOctetsPdu = 128;
        }

        /// <summary>
        /// Transmit Window Size K
        /// </summary>
        [XmlIgnore()]
        public byte TransmitWindowSizeK
        {
            get;
            set;
        }

        /// <summary>
        /// Transmit Window Size RW
        /// </summary>
        [XmlIgnore()]
        public byte TransmitWindowSizeRW
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum octets in I Pdu N1.
        /// </summary>
        [XmlIgnore()]
        public UInt16 MaximumOctetsPdu
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum number of transmissions, N2.
        /// </summary>
        [XmlIgnore()]
        public byte MaximumNumberTransmissions
        {
            get;
            set;
        }

        /// <summary>
        /// Acknowledgement timer in seconds.
        /// </summary>
        [XmlIgnore()]
        public UInt16 AcknowledgementTimer
        {
            get;
            set;
        }


        /// <summary>
        /// P-bit timer inseconds.
        /// </summary>
        [XmlIgnore()]
        public UInt16 BitTimer
        {
            get;
            set;
        }

        /// <summary>
        /// Reject timer.
        /// </summary>
        [XmlIgnore()]
        public UInt16 RejectTimer
        {
            get;
            set;
        }

        /// <summary>
        /// Busy state timer.
        /// </summary>
        [XmlIgnore()]
        public UInt16 BusyStateTimer
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, TransmitWindowSizeK, TransmitWindowSizeRW, MaximumOctetsPdu, MaximumNumberTransmissions,
            AcknowledgementTimer, BitTimer, RejectTimer, BusyStateTimer};
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
            //TransmitWindowSizeK
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //TransmitWindowSizeRW
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //MaximumOctetsPdu
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //MaximumNumberTransmissions
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //AcknowledgementTimer
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //BitTimer
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //RejectTimer
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //BusyStateTimer
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "TransmitWindowSizeK", "TransmitWindowSizeRW",
                "MaximumOctetsPdu", "MaximumNumberTransmissions",
            "AcknowledgementTimer", "BitTimer", "RejectTimer", "BusyStateTimer"};
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
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
            return 9;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                case 3:
                case 5:
                    return DataType.UInt8;
                case 4:
                case 6:
                case 7:
                case 8:
                case 9:
                    return DataType.UInt16;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            object ret;
            switch (e.Index)
            {
                case 1:
                    ret = GXCommon.LogicalNameToBytes(LogicalName);
                    break;
                case 2:
                    ret = TransmitWindowSizeK;
                    break;
                case 3:
                    ret = TransmitWindowSizeRW;
                    break;
                case 4:
                    ret = MaximumOctetsPdu;
                    break;
                case 5:
                    ret = MaximumNumberTransmissions;
                    break;
                case 6:
                    ret = AcknowledgementTimer;
                    break;
                case 7:
                    ret = BitTimer;
                    break;
                case 8:
                    ret = RejectTimer;
                    break;
                case 9:
                    ret = BusyStateTimer;
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    ret = null;
                    break;
            }
            return ret;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    TransmitWindowSizeK = Convert.ToByte(e.Value);
                    break;
                case 3:
                    TransmitWindowSizeRW = Convert.ToByte(e.Value);
                    break;
                case 4:
                    MaximumOctetsPdu = Convert.ToUInt16(e.Value);
                    break;
                case 5:
                    MaximumNumberTransmissions = Convert.ToByte(e.Value);
                    break;
                case 6:
                    AcknowledgementTimer = Convert.ToUInt16(e.Value);
                    break;
                case 7:
                    BitTimer = Convert.ToUInt16(e.Value);
                    break;
                case 8:
                    RejectTimer = Convert.ToUInt16(e.Value);
                    break;
                case 9:
                    BusyStateTimer = Convert.ToUInt16(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            TransmitWindowSizeK = (byte)reader.ReadElementContentAsInt("TransmitWindowSizeK");
            TransmitWindowSizeRW = (byte)reader.ReadElementContentAsInt("TransmitWindowSizeRW");
            MaximumOctetsPdu = (UInt16)reader.ReadElementContentAsInt("MaximumOctetsPdu");
            MaximumNumberTransmissions = (byte)reader.ReadElementContentAsInt("MaximumNumberTransmissions");
            AcknowledgementTimer = (UInt16)reader.ReadElementContentAsInt("AcknowledgementTimer");
            BitTimer = (UInt16)reader.ReadElementContentAsInt("BitTimer");
            RejectTimer = (UInt16)reader.ReadElementContentAsInt("RejectTimer");
            BusyStateTimer = (UInt16)reader.ReadElementContentAsInt("BusyStateTimer");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("TransmitWindowSizeK", TransmitWindowSizeK, 2);
            writer.WriteElementString("TransmitWindowSizeRW", TransmitWindowSizeRW, 3);
            writer.WriteElementString("MaximumOctetsPdu", MaximumOctetsPdu, 4);
            writer.WriteElementString("MaximumNumberTransmissions", MaximumNumberTransmissions, 5);
            writer.WriteElementString("AcknowledgementTimer", AcknowledgementTimer, 6);
            writer.WriteElementString("BitTimer", BitTimer, 7);
            writer.WriteElementString("RejectTimer", RejectTimer, 8);
            writer.WriteElementString("BusyStateTimer", BusyStateTimer, 8);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
