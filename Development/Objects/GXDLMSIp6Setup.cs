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
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using System.Net;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Address type to add.
    /// </summary>
    public enum IPv6AddressType : byte
    {
        /// <summary>
        /// Unicast address.
        /// </summary>
        Unicast,
        /// <summary>
        /// Multicast address.
        /// </summary>
        Multicast,
        /// <summary>
        /// Gateway address.
        /// </summary>
        Gateway
    }

    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSIp6Setup
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

        /// <summary>
        /// Adds IP v6 address to the meter.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="type">Address type.</param>
        /// <param name="address">IP v6 Address to add.</param>
        /// <returns></returns>
        public byte[][] AddAddress(GXDLMSClient client, IPv6AddressType type, IPAddress address)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            GXCommon.SetData(null, bb, DataType.Enum, type);
            GXCommon.SetData(null, bb, DataType.OctetString, address.GetAddressBytes());
            return client.Method(this, 1, bb.Array(), DataType.Structure);
        }

        /// <summary>
        /// Removes IP v6 address from the meter.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="type">Address type.</param>
        /// <param name="address">IP v6 Address to remove.</param>
        /// <returns></returns>
        public byte[][] RemoveAddress(GXDLMSClient client, IPv6AddressType type, IPAddress address)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            GXCommon.SetData(null, bb, DataType.Enum, type);
            GXCommon.SetData(null, bb, DataType.OctetString, address.GetAddressBytes());
            return client.Method(this, 2, bb.Array(), DataType.Structure);
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

        /// <summary>
        /// Remove address from the list.
        /// </summary>
        private void Remove(List<IPAddress> list, IPAddress address)
        {
            foreach (IPAddress it in list)
            {
                if (it.Equals(address))
                {
                    list.Remove(it);
                    return;
                }
            }
            throw new Exception("IP address not found.");
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            GXStructure val = (GXStructure)e.Parameters;
            IPv6AddressType type = (IPv6AddressType)Convert.ToByte(val[0]);
            IPAddress address = new IPAddress((byte[])val[1]);
            List<IPAddress> list;
            if (e.Index == 1)
            {
                switch (type)
                {
                    case IPv6AddressType.Unicast:
                        list = new List<IPAddress>();
                        if (UnicastIPAddress != null)
                        {
                            list.AddRange(UnicastIPAddress);
                        }
                        list.Add(address);
                        UnicastIPAddress = list.ToArray();
                        break;
                    case IPv6AddressType.Multicast:
                        list = new List<IPAddress>();
                        if (MulticastIPAddress != null)
                        {
                            list.AddRange(MulticastIPAddress);
                        }
                        list.Add(address);
                        MulticastIPAddress = list.ToArray();
                        break;
                    case IPv6AddressType.Gateway:
                        list = new List<IPAddress>();
                        if (GatewayIPAddress != null)
                        {
                            list.AddRange(GatewayIPAddress);
                        }
                        list.Add(address);
                        GatewayIPAddress = list.ToArray();
                        break;
                    default:
                        e.Error = ErrorCode.ReadWriteDenied;
                        break;
                }
            }
            else if (e.Index == 2)
            {
                switch (type)
                {
                    case IPv6AddressType.Unicast:
                        list = new List<IPAddress>(UnicastIPAddress);
                        Remove(list, address);
                        UnicastIPAddress = list.ToArray();
                        break;
                    case IPv6AddressType.Multicast:
                        list = new List<IPAddress>(MulticastIPAddress);
                        Remove(list, address);
                        MulticastIPAddress = list.ToArray();
                        break;
                    case IPv6AddressType.Gateway:
                        list = new List<IPAddress>(GatewayIPAddress);
                        Remove(list, address);
                        GatewayIPAddress = list.ToArray();
                        break;
                    default:
                        e.Error = ErrorCode.ReadWriteDenied;
                        break;
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
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

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Add IP v6 address", "Remove IP v6 address" };
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
                return GXCommon.LogicalNameToBytes(DataLinkLayerReference);
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
                    DataLinkLayerReference = e.Value.ToString();
                }
                else
                {
                    DataLinkLayerReference = GXCommon.ToLogicalName(e.Value);
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
                    foreach (object it in (IEnumerable<object>)e.Value)
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
                    foreach (object it in (IEnumerable<object>)e.Value)
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
                    foreach (object it in (IEnumerable<object>)e.Value)
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
                        GXNeighborDiscoverySetup v = new GXNeighborDiscoverySetup();
                        v.MaxRetry = Convert.ToByte(it[0]);
                        v.RetryWaitTime = Convert.ToUInt16(it[1]);
                        v.SendPeriod = Convert.ToUInt32(it[2]);
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
                while (reader.IsStartElement("Item", true))
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

        private void SaveIPAddress(GXXmlWriter writer, IPAddress[] list, string name, int index)
        {
            if (list != null)
            {
                writer.WriteStartElement(name, index);
                foreach (IPAddress it in list)
                {
                    writer.WriteElementString("Value", it.ToString(), index);
                }
                writer.WriteEndElement();
            }
        }

        private void SaveNeighborDiscoverySetup(GXXmlWriter writer, GXNeighborDiscoverySetup[] list, string name, int index)
        {
            if (list != null)
            {
                writer.WriteStartElement(name, index);
                foreach (GXNeighborDiscoverySetup it in list)
                {
                    writer.WriteStartElement("Item", index);
                    writer.WriteElementString("MaxRetry", it.MaxRetry, index);
                    writer.WriteElementString("RetryWaitTime", it.RetryWaitTime, index);
                    writer.WriteElementString("SendPeriod", it.SendPeriod, index);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("DataLinkLayerReference", DataLinkLayerReference, 2);
            writer.WriteElementString("AddressConfigMode", (int)AddressConfigMode, 3);
            SaveIPAddress(writer, UnicastIPAddress, "UnicastIPAddress", 4);
            SaveIPAddress(writer, MulticastIPAddress, "MulticastIPAddress", 5);
            SaveIPAddress(writer, GatewayIPAddress, "GatewayIPAddress", 6);
            if (PrimaryDNSAddress != null)
            {
                writer.WriteElementString("PrimaryDNSAddress", PrimaryDNSAddress.ToString(), 7);
            }
            if (SecondaryDNSAddress != null)
            {
                writer.WriteElementString("SecondaryDNSAddress", SecondaryDNSAddress.ToString(), 8);
            }
            writer.WriteElementString("TrafficClass", TrafficClass, 9);
            SaveNeighborDiscoverySetup(writer, NeighborDiscoverySetup, "NeighborDiscoverySetup", 10);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
