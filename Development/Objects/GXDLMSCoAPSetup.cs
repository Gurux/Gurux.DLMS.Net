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
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCoAPSetup
    /// </summary>
    public class GXDLMSCoAPSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSCoAPSetup()
        : this("0.0.25.16.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSCoAPSetup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSCoAPSetup(string ln, ushort sn)
        : base(ObjectType.CoAPSetup, ln, sn)
        {
        }

        /// <summary>
        /// TCP-UDP setup object.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSTcpUdpSetup UdpReference
        {
            get;
            set;
        }

        /// <summary>
        /// The minimum initial ACK timeout in milliseconds.
        /// </summary>
        [XmlIgnore()]
        public UInt16 AckTimeout
        {
            get;
            set;
        }


        /// <summary>
        /// The random factor to apply for randomness of the initial ACK timeout.
        /// </summary>
        [XmlIgnore()]
        public UInt16 AckRandomFactor
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum number of retransmissions for a confirmable message.
        /// </summary>
        [XmlIgnore()]
        public UInt16 MaxRetransmit
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of simultaneous outstanding CoAP request messages.
        /// </summary>
        [XmlIgnore()]
        public UInt16 NStart
        {
            get;
            set;
        }

        /// <summary>
        /// Delay acknowledge timeout in milliseconds.
        /// </summary>
        [XmlIgnore()]
        public UInt16 DelayAckTimeout

        {
            get;
            set;
        }

        /// <summary>
        /// Exponential back off.
        /// </summary>
        [XmlIgnore()]
        public UInt16 ExponentialBackOff

        {
            get;
            set;
        }

        /// <summary>
        /// Probing rate.
        /// </summary>
        [XmlIgnore()]
        public UInt16 ProbingRate

        {
            get;
            set;
        }

        /// <summary>
        /// CoAP Uri path.
        /// </summary>
        [XmlIgnore()]
        public string CoAPUriPath

        {
            get;
            set;
        }

        /// <summary>
        /// CoAP transport mode.
        /// </summary>
        [XmlIgnore()]
        public TransportMode TransportMode
        {
            get;
            set;
        }

        /// <summary>
        /// The version of the DLMS/COSEM CoAP wrapper.
        /// </summary>
        [XmlIgnore()]
        public object WrapperVersion

        {
            get;
            set;
        }

        /// <summary>
        /// The length of the Token.
        /// </summary>
        [XmlIgnore()]
        public byte TokenLength
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName,
                UdpReference,
                AckTimeout,
                AckRandomFactor,
                MaxRetransmit,
                NStart,
                DelayAckTimeout,
                ExponentialBackOff,
                ProbingRate,
                CoAPUriPath,
                TransportMode,
                WrapperVersion,
                TokenLength };
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
            //Value
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(),
                "Value",
                "UdpReference",
                "AckTimeout",
                "AckRandomFactor",
                "MaxRetransmit",
                "NStart",
                "DelayAckTimeout",
                "ExponentialBackOff",
                "ProbingRate",
                "CoAPUriPath",
                "TransportMode",
                "WrapperVersion",
                "TokenLength"};
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
            return 13;
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
                case 2:
                case 10:
                    return DataType.OctetString;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return DataType.UInt16;
                case 11:
                    return DataType.Enum;
                case 12:
                    DataType dt = base.GetDataType(index);
                    if (dt == DataType.None && WrapperVersion != null)
                    {
                        dt = GXCommon.GetDLMSDataType(WrapperVersion.GetType());
                    }
                    return dt;
                case 13:
                    return DataType.UInt8;
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
                    if (UdpReference != null)
                    {
                        ret = GXCommon.LogicalNameToBytes(UdpReference.LogicalName);
                    }
                    else
                    {
                        ret = null;
                    }
                    break;
                case 3:
                    ret = AckTimeout;
                    break;
                case 4:
                    ret = AckRandomFactor;
                    break;
                case 5:
                    ret = MaxRetransmit;
                    break;
                case 6:
                    ret = NStart;
                    break;
                case 7:
                    ret = DelayAckTimeout;
                    break;
                case 8:
                    ret = ExponentialBackOff;
                    break;
                case 9:
                    ret = ProbingRate;
                    break;
                case 10:
                    ret = ASCIIEncoding.ASCII.GetBytes(CoAPUriPath);
                    break;
                case 11:
                    ret = TransportMode;
                    break;
                case 12:
                    ret = WrapperVersion;
                    break;
                case 13:
                    ret = TokenLength;
                    break;
                default:
                    ret = null;
                    e.Error = ErrorCode.ReadWriteDenied;
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
                    {
                        UdpReference = null;
                        string ln = GXCommon.ToLogicalName(e.Value);
                        UdpReference = (GXDLMSTcpUdpSetup)settings.Objects.FindByLN(ObjectType.TcpUdpSetup, ln);
                        if (UdpReference == null)
                        {
                            UdpReference = new GXDLMSTcpUdpSetup(ln);
                        }
                    }
                    break;
                case 3:
                    AckTimeout = Convert.ToUInt16(e.Value);
                    break;
                case 4:
                    AckRandomFactor = Convert.ToUInt16(e.Value);
                    break;
                case 5:
                    MaxRetransmit = Convert.ToUInt16(e.Value);
                    break;
                case 6:
                    NStart = Convert.ToUInt16(e.Value);
                    break;
                case 7:
                    DelayAckTimeout = Convert.ToUInt16(e.Value);
                    break;
                case 8:
                    ExponentialBackOff = Convert.ToUInt16(e.Value);
                    break;
                case 9:
                    ProbingRate = Convert.ToUInt16(e.Value);
                    break;
                case 10:
                    CoAPUriPath = ASCIIEncoding.ASCII.GetString((byte[])e.Value);
                    break;
                case 11:
                    TransportMode = (TransportMode)e.Value;
                    break;
                case 12:
                    WrapperVersion = e.Value;
                    break;
                case 13:
                    TokenLength = Convert.ToByte(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            UdpReference = null;
            string ln = reader.ReadElementContentAsString("UdpReference");
            if (!string.IsNullOrEmpty(ln))
            {
                UdpReference = (GXDLMSTcpUdpSetup)reader.Objects.FindByLN(ObjectType.TcpUdpSetup, ln);
                if (UdpReference == null)
                {
                    UdpReference = new GXDLMSTcpUdpSetup(ln);
                }
            }
            AckTimeout = (UInt16)reader.ReadElementContentAsInt("AckTimeout");
            AckRandomFactor = (UInt16)reader.ReadElementContentAsInt("AckRandomFactor");
            MaxRetransmit = (UInt16)reader.ReadElementContentAsInt("MaxRetransmit");
            NStart = (UInt16)reader.ReadElementContentAsInt("NStart");
            DelayAckTimeout = (UInt16)reader.ReadElementContentAsInt("DelayAckTimeout");
            ExponentialBackOff = (UInt16)reader.ReadElementContentAsInt("ExponentialBackOff");
            ProbingRate = (UInt16)reader.ReadElementContentAsInt("ProbingRate");
            CoAPUriPath = reader.ReadElementContentAsString("CoAPUriPath");
            TransportMode = (TransportMode)reader.ReadElementContentAsInt("TransportMode");
            WrapperVersion = reader.ReadElementContentAsObject("WrapperVersion", null, this, 12);
            TokenLength = (byte)reader.ReadElementContentAsInt("TokenLength");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (UdpReference != null)
            {
                writer.WriteElementString("UdpReference", UdpReference.LogicalName, 2);
            }
            writer.WriteElementString("AckTimeout", AckTimeout, 3);
            writer.WriteElementString("AckRandomFactor", AckRandomFactor, 4);
            writer.WriteElementString("MaxRetransmit", MaxRetransmit, 5);
            writer.WriteElementString("NStart", NStart, 6);
            writer.WriteElementString("DelayAckTimeout", DelayAckTimeout, 7);
            writer.WriteElementString("ExponentialBackOff", ExponentialBackOff, 8);
            writer.WriteElementString("ProbingRate", ProbingRate, 9);
            writer.WriteElementString("CoAPUriPath", CoAPUriPath, 10);
            writer.WriteElementString("TransportMode", (int)TransportMode, 11);
            writer.WriteElementObject("WrapperVersion", WrapperVersion, 12);
            writer.WriteElementString("TokenLength", TokenLength, 13);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
            if (UdpReference != null)
            {
                GXDLMSTcpUdpSetup target = (GXDLMSTcpUdpSetup)reader.Objects.FindByLN(ObjectType.TcpUdpSetup,
                    UdpReference.LogicalName);
                if (target != null && target != UdpReference)
                {
                    UdpReference = target;
                }
            }
        }
        #endregion
    }
}
