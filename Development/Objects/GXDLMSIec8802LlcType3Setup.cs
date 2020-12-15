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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXIec8802LlcType3Setup
    /// </summary>
    public class GXDLMSIec8802LlcType3Setup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSIec8802LlcType3Setup()
        : this("0.0.27.2.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSIec8802LlcType3Setup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIec8802LlcType3Setup(string ln, ushort sn)
        : base(ObjectType.Iec8802LlcType3Setup, ln, sn)
        {
        }

        /// <summary>
        ///Maximum number of octets in an ACn command PDU, N3.
        /// </summary>
        [XmlIgnore()]
        public UInt16 MaximumOctetsACnPdu
        {
            get;
            set;
        }

        /// <summary>
        ///Maximum number of times in transmissions N4.
        /// </summary>
        [XmlIgnore()]
        public byte MaximumTransmissions
        {
            get;
            set;
        }

        /// <summary>
        /// Acknowledgement time, T1
        /// </summary>
        [XmlIgnore()]
        public UInt16 AcknowledgementTime
        {
            get;
            set;
        }

        /// <summary>
        /// Receive lifetime variable, T2.
        /// </summary>
        [XmlIgnore()]
        public UInt16 ReceiveLifetime
        {
            get;
            set;
        }

        /// <summary>
        /// Transmit lifetime variable, T3
        /// </summary>
        [XmlIgnore()]
        public UInt16 TransmitLifetime
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, MaximumOctetsACnPdu, MaximumTransmissions, AcknowledgementTime,
           ReceiveLifetime, TransmitLifetime };
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
            //MaximumOctetsACnPdu
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //MaximumTransmissions
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //AcknowledgementTime
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //ReceiveLifetime
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //TransmitLifetime
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "MaximumOctetsACnPdu", "MaximumTransmissions", "AcknowledgementTime",
           "ReceiveLifetime", "TransmitLifetime"};
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[0];
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 6;
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
                case 3:
                    return DataType.UInt8;
                case 2:
                case 4:
                case 5:
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
                    ret = MaximumOctetsACnPdu;
                    break;
                case 3:
                    ret = MaximumTransmissions;
                    break;
                case 4:
                    ret = AcknowledgementTime;
                    break;
                case 5:
                    ret = ReceiveLifetime;
                    break;
                case 6:
                    ret = TransmitLifetime;
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
                    MaximumOctetsACnPdu = Convert.ToUInt16(e.Value);
                    break;
                case 3:
                    MaximumTransmissions = Convert.ToByte(e.Value);
                    break;
                case 4:
                    AcknowledgementTime = Convert.ToUInt16(e.Value);
                    break;
                case 5:
                    ReceiveLifetime = Convert.ToUInt16(e.Value);
                    break;
                case 6:
                    TransmitLifetime = Convert.ToUInt16(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            MaximumOctetsACnPdu = (UInt16)reader.ReadElementContentAsInt("MaximumOctetsACnPdu");
            MaximumTransmissions = (byte)reader.ReadElementContentAsInt("MaximumTransmissions");
            AcknowledgementTime = (UInt16)reader.ReadElementContentAsInt("AcknowledgementTime");
            ReceiveLifetime = (UInt16)reader.ReadElementContentAsInt("ReceiveLifetime");
            TransmitLifetime = (UInt16)reader.ReadElementContentAsInt("TransmitLifetime");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("MaximumOctetsACnPdu", MaximumOctetsACnPdu, 2);
            writer.WriteElementString("MaximumTransmissions", MaximumTransmissions, 3);
            writer.WriteElementString("AcknowledgementTime", AcknowledgementTime, 4);
            writer.WriteElementString("ReceiveLifetime", ReceiveLifetime, 5);
            writer.WriteElementString("TransmitLifetime", TransmitLifetime, 6);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
