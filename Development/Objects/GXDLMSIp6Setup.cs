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
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using System.Net;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSIp6Setup
    /// </summary>
    public class GXDLMSIp6Setup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSIp6Setup()
        : this("0.0.25.7.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSIp6Setup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIp6Setup(string ln, ushort sn)
        : base(ObjectType.Ip6Setup, ln, sn)
        {
        }

        [XmlIgnore()]
        public string DataLinkLayerReference
        {
            get;
            set;
        }

        [XmlIgnore()]
        public AddressConfigMode AddressConfigMode
        {
            get;
            set;
        }


        [XmlIgnore()]
        public IPAddress[] UnicastIPAddress
        {
            get;
            set;
        }

        [XmlIgnore()]
        public IPAddress[] MulticastIPAddress
        {
            get;
            set;
        }

        [XmlIgnore()]
        public IPAddress[] GatewayIPAddress
        {
            get;
            set;
        }

        [XmlIgnore()]
        public IPAddress PrimaryDNSAddress
        {
            get;
            set;
        }

        [XmlIgnore()]
        public IPAddress SecondaryDNSAddress
        {
            get;
            set;
        }

        [XmlIgnore()]
        public byte TrafficClass
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXNeighborDiscoverySetup[] NeighborDiscoverySetup
        {
            get;
            set;
        }


        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, DataLinkLayerReference,
        AddressConfigMode, UnicastIPAddress, MulticastIPAddress,
        GatewayIPAddress, PrimaryDNSAddress, SecondaryDNSAddress,
        TrafficClass, NeighborDiscoverySetup};
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
            //DataLinkLayerReference
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            //AddressConfigMode
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //UnicastIPAddress
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //MulticastIPAddress
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //GatewayIPAddress
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //PrimaryDNSAddress
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //SecondaryDNSAddress
            if (all || !base.IsRead(8))
            {
                attributes.Add(8);
            }
            //TrafficClass
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //NeighborDiscoverySetup
            if (all || CanRead(10))
            {
                attributes.Add(10);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Data LinkLayer Reference",
        "Address Config Mode", "Unicast IP Address", "Multicast IP Address",
        "Gateway IP Address", "Primary DNS Address", "Secondary DNS Address",
        "Traffic Class", "Neighbor Discovery Setup"};
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 10;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
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
                return DataType.OctetString;
            }
            if (index == 3)
            {
                return DataType.Enum;
            }
            if (index == 4)
            {
                return DataType.Array;
            }
            if (index == 5)
            {
                return DataType.Array;
            }
            if (index == 6)
            {
                return DataType.Array;
            }
            if (index == 7)
            {
                return DataType.OctetString;
            }
            if (index == 8)
            {
                return DataType.OctetString;
            }
            if (index == 9)
            {
                return DataType.UInt8;
            }
            if (index == 10)
            {
                return DataType.Array;
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
                return this.DataLinkLayerReference;
            }
            if (e.Index == 3)
            {
                return AddressConfigMode;
            }
            if (e.Index == 4)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (UnicastIPAddress == null)
                {
                    //Object count is zero.
                    data.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(UnicastIPAddress.Length, data);
                    foreach (IPAddress it in UnicastIPAddress)
                    {
                        GXCommon.SetData(settings, data, DataType.OctetString, it.GetAddressBytes());
                    }
                }
                return data.Array();
            }
            if (e.Index == 5)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (MulticastIPAddress == null)
                {
                    //Object count is zero.
                    data.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(MulticastIPAddress.Length, data);
                    foreach (IPAddress it in MulticastIPAddress)
                    {
                        GXCommon.SetData(settings, data, DataType.OctetString, it.GetAddressBytes());
                    }
                }
                return data.Array();
            }
            if (e.Index == 6)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (GatewayIPAddress == null)
                {
                    //Object count is zero.
                    data.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(GatewayIPAddress.Length, data);
                    foreach (IPAddress it in GatewayIPAddress)
                    {
                        GXCommon.SetData(settings, data, DataType.OctetString, it.GetAddressBytes());
                    }
                }
                return data.Array();
            }
            if (e.Index == 7)
            {
                if (PrimaryDNSAddress == null)
                {
                    return null;
                }
                return PrimaryDNSAddress.GetAddressBytes();
            }
            if (e.Index == 8)
            {
                if (SecondaryDNSAddress == null)
                {
                    return null;
                }
                return SecondaryDNSAddress.GetAddressBytes();
            }
            if (e.Index == 9)
            {
                return TrafficClass;
            }
            if (e.Index == 10)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (NeighborDiscoverySetup == null)
                {
                    //Object count is zero.
                    data.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(NeighborDiscoverySetup.Length, data);
                    foreach (GXNeighborDiscoverySetup it in NeighborDiscoverySetup)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(3);
                        GXCommon.SetData(settings, data, DataType.UInt8, it.MaxRetry);
                        GXCommon.SetData(settings, data, DataType.UInt16, it.RetryWaitTime);
                        GXCommon.SetData(settings, data, DataType.UInt32, it.SendPeriod);
                    }
                }
                return data.Array();
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
                if (e.Value is string)
                {
                    this.DataLinkLayerReference = e.Value.ToString();
                }
                else
                {
                    this.DataLinkLayerReference = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString, settings.UseUtc2NormalTime).ToString();
                }
            }
            else if (e.Index == 3)
            {
                AddressConfigMode = (AddressConfigMode)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 4)
            {
                List<IPAddress> data = new List<IPAddress>();
                if (e.Value != null)
                {
                    foreach (object it in (Object[])e.Value)
                    {
                        data.Add(new IPAddress((byte[])it));
                    }
                }
                UnicastIPAddress = data.ToArray();
            }
            else if (e.Index == 5)
            {
                List<IPAddress> data = new List<IPAddress>();
                if (e.Value != null)
                {
                    foreach (object it in (Object[])e.Value)
                    {
                        data.Add(new IPAddress((byte[])it));
                    }
                }
                MulticastIPAddress = data.ToArray();
            }
            else if (e.Index == 6)
            {
                List<IPAddress> data = new List<IPAddress>();
                if (e.Value != null)
                {
                    foreach (object it in (Object[])e.Value)
                    {
                        data.Add(new IPAddress((byte[])it));
                    }
                }
                GatewayIPAddress = data.ToArray();
            }
            else if (e.Index == 7)
            {
                if (e.Value == null || ((byte[])e.Value).Length == 0)
                {
                    PrimaryDNSAddress = null;
                }
                else
                {
                    PrimaryDNSAddress = new IPAddress((byte[])e.Value);
                }
            }
            else if (e.Index == 8)
            {
                if (e.Value == null || ((byte[])e.Value).Length == 0)
                {
                    SecondaryDNSAddress = null;
                }
                else
                {
                    SecondaryDNSAddress = new IPAddress((byte[])e.Value);
                }
            }
            else if (e.Index == 9)
            {
                TrafficClass = Convert.ToByte(e.Value);
            }
            else if (e.Index == 10)
            {
                List<GXNeighborDiscoverySetup> data = new List<GXNeighborDiscoverySetup>();
                if (e.Value != null)
                {
                    foreach (object it in (Object[])e.Value)
                    {
                        object[] tmp = (object[])it;
                        GXNeighborDiscoverySetup v = new GXNeighborDiscoverySetup();
                        v.MaxRetry = Convert.ToByte(tmp[0]);
                        v.RetryWaitTime = Convert.ToUInt16(tmp[1]);
                        v.SendPeriod = Convert.ToUInt32(tmp[2]);
                        data.Add(v);
                    }
                }
                NeighborDiscoverySetup = data.ToArray();
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        private IPAddress[] LoadIPAddress(GXXmlReader reader, string name)
        {
            List<IPAddress> list = new List<IPAddress>();
            if (reader.IsStartElement(name, true))
            {
                while (reader.IsStartElement("Value", false))
                {
                    list.Add(IPAddress.Parse(reader.ReadElementContentAsString("Value")));
                }
                reader.ReadEndElement(name);
            }
            return list.ToArray();
        }

        private GXNeighborDiscoverySetup[] LoadNeighborDiscoverySetup(GXXmlReader reader, string name)
        {
            List<GXNeighborDiscoverySetup> list = new List<GXNeighborDiscoverySetup>();
            if (reader.IsStartElement(name, true))
            {
                while (reader.IsStartElement("Item", false))
                {
                    GXNeighborDiscoverySetup it = new GXNeighborDiscoverySetup();
                    list.Add(it);
                    it.MaxRetry = (byte)reader.ReadElementContentAsInt("MaxRetry");
                    it.RetryWaitTime = (UInt16)reader.ReadElementContentAsInt("RetryWaitTime");
                    it.SendPeriod = (UInt32)reader.ReadElementContentAsInt("SendPeriod");
                }
                reader.ReadEndElement(name);
            }
            return list.ToArray();
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            DataLinkLayerReference = reader.ReadElementContentAsString("DataLinkLayerReference");
            AddressConfigMode = (AddressConfigMode)reader.ReadElementContentAsInt("AddressConfigMode");
            UnicastIPAddress = LoadIPAddress(reader, "UnicastIPAddress");
            MulticastIPAddress = LoadIPAddress(reader, "MulticastIPAddress");
            GatewayIPAddress = LoadIPAddress(reader, "GatewayIPAddress");
            string str = reader.ReadElementContentAsString("PrimaryDNSAddress");
            if (!string.IsNullOrEmpty(str))
            {
                PrimaryDNSAddress = IPAddress.Parse(str);
            }
            else
            {
                PrimaryDNSAddress = null;
            }
            str = reader.ReadElementContentAsString("SecondaryDNSAddress");
            if (!string.IsNullOrEmpty(str))
            {
                SecondaryDNSAddress = IPAddress.Parse(str);
            }
            else
            {
                SecondaryDNSAddress = null;
            }
            TrafficClass = (byte)reader.ReadElementContentAsInt("TrafficClass");
            NeighborDiscoverySetup = LoadNeighborDiscoverySetup(reader, "NeighborDiscoverySetup");
        }

        private void SaveIPAddress(GXXmlWriter writer, IPAddress[] list, string name)
        {
            if (list != null)
            {
                writer.WriteStartElement(name);
                foreach (IPAddress it in list)
                {
                    writer.WriteElementString("Value", it.ToString());
                }
                writer.WriteEndElement();
            }
        }

        private void SaveNeighborDiscoverySetup(GXXmlWriter writer, GXNeighborDiscoverySetup[] list, string name)
        {
            if (list != null)
            {
                writer.WriteStartElement(name);
                foreach (GXNeighborDiscoverySetup it in list)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("MaxRetry", it.MaxRetry);
                    writer.WriteElementString("RetryWaitTime", it.RetryWaitTime);
                    writer.WriteElementString("SendPeriod", it.SendPeriod);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("DataLinkLayerReference", DataLinkLayerReference);
            writer.WriteElementString("AddressConfigMode", (int)AddressConfigMode);
            SaveIPAddress(writer, UnicastIPAddress, "UnicastIPAddress");
            SaveIPAddress(writer, MulticastIPAddress, "MulticastIPAddress");
            SaveIPAddress(writer, GatewayIPAddress, "GatewayIPAddress");
            if (PrimaryDNSAddress != null)
            {
                writer.WriteElementString("PrimaryDNSAddress", PrimaryDNSAddress.ToString());
            }
            if (SecondaryDNSAddress != null)
            {
                writer.WriteElementString("SecondaryDNSAddress", SecondaryDNSAddress.ToString());
            }
            writer.WriteElementString("TrafficClass", TrafficClass);
            SaveNeighborDiscoverySetup(writer, NeighborDiscoverySetup, "NeighborDiscoverySetup");
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
