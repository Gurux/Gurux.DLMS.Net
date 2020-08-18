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
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSIp4Setup
    /// </summary>
    public class GXDLMSIp4Setup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSIp4Setup()
        : this("0.0.25.1.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSIp4Setup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIp4Setup(string ln, ushort sn)
        : base(ObjectType.Ip4Setup, ln, sn)
        {
        }

        [XmlIgnore()]
        public string DataLinkLayerReference
        {
            get;
            set;
        }

        [XmlIgnore()]
        public IPAddress IPAddress
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
        public GXDLMSIp4SetupIpOption[] IPOptions
        {
            get;
            set;
        }

        [XmlIgnore()]
        public IPAddress SubnetMask
        {
            get;
            set;
        }

        [XmlIgnore()]
        public IPAddress GatewayIPAddress
        {
            get;
            set;
        }

        [XmlIgnore()]
        public bool UseDHCP
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

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, DataLinkLayerReference, IPAddress,
                              MulticastIPAddress, IPOptions, SubnetMask, GatewayIPAddress,
                              UseDHCP, PrimaryDNSAddress, SecondaryDNSAddress
                            };
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
            //IPAddress
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //MulticastIPAddress
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //IPOptions
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //SubnetMask
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //GatewayIPAddress
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //UseDHCP
            if (all || !base.IsRead(8))
            {
                attributes.Add(8);
            }
            //PrimaryDNSAddress
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //SecondaryDNSAddress
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
                             "Data LinkLayer Reference", "IP Address", "Multicast IP Address",
                             "IP Options", "Subnet Mask", "Gateway IP Address", "Use DHCP",
                             "Primary DNS Address", "Secondary DNS Address"
                            };

        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Add mc IP address", "Delete mc IP address", "Get nbof mc IP addresses" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 10;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 3;
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
                return DataType.UInt32;
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
                return DataType.UInt32;
            }
            if (index == 7)
            {
                return DataType.UInt32;
            }
            if (index == 8)
            {
                return DataType.Boolean;
            }
            if (index == 9)
            {
                return DataType.UInt32;
            }
            if (index == 10)
            {
                return DataType.UInt32;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        private static UInt32 FromAddressString(IPAddress value)
        {
            if (value == null)
            {
                return 0;
            }
            GXByteBuffer bb = new GXByteBuffer(value.GetAddressBytes());
            return bb.GetUInt32();
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
                return FromAddressString(IPAddress);
            }
            if (e.Index == 4)
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
                        GXCommon.SetData(settings, data, DataType.UInt32, FromAddressString(it));
                    }
                }
                return data.Array();
            }
            if (e.Index == 5)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (IPOptions == null)
                {
                    //Object count is zero.
                    data.SetUInt8(0);
                }
                else
                {
                    GXCommon.SetObjectCount(IPOptions.Length, data);
                    foreach (GXDLMSIp4SetupIpOption it in IPOptions)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(3);
                        GXCommon.SetData(settings, data, DataType.UInt8, it.Type);
                        GXCommon.SetData(settings, data, DataType.UInt8, it.Length);
                        GXCommon.SetData(settings, data, DataType.OctetString, it.Data);
                    }
                }
                return data.Array();
            }
            if (e.Index == 6)
            {
                //If subnet mask is not given.
                return FromAddressString(SubnetMask);
            }
            if (e.Index == 7)
            {
                return FromAddressString(GatewayIPAddress);
            }
            if (e.Index == 8)
            {
                return this.UseDHCP;
            }
            if (e.Index == 9)
            {
                return FromAddressString(PrimaryDNSAddress);
            }
            if (e.Index == 10)
            {
                return FromAddressString(SecondaryDNSAddress);
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        private static IPAddress ToAddressString(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is string)
            {
                return IPAddress.Parse((string)value);
            }
            GXByteBuffer bb = new GXByteBuffer();
            bb.Add(value);
            return new System.Net.IPAddress(bb.Array());
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                LogicalName = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 2)
            {
                DataLinkLayerReference = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 3)
            {
                IPAddress = ToAddressString(e.Value);
            }
            else if (e.Index == 4)
            {
                List<IPAddress> data = new List<IPAddress>();
                if (e.Value is List<object>)
                {
                    foreach (object it in (List<object>)e.Value)
                    {
                        data.Add(ToAddressString(it));
                    }
                }
                else if (e.Value is UInt16[])
                {
                    //Some meters are returning wrong data here.
                    foreach (UInt16 it in (UInt16[])e.Value)
                    {
                        data.Add(ToAddressString(it));
                    }
                }
                MulticastIPAddress = data.ToArray();
            }
            else if (e.Index == 5)
            {
                List<GXDLMSIp4SetupIpOption> data = new List<GXDLMSIp4SetupIpOption>();
                if (e.Value is List<object>)
                {
                    foreach (List<object> it in (List<object>)e.Value)
                    {
                        GXDLMSIp4SetupIpOption item = new GXDLMSIp4SetupIpOption();
                        item.Type = (Ip4SetupIpOptionType)Convert.ToInt32(it[0]);
                        item.Length = Convert.ToByte(it[1]);
                        item.Data = (byte[])it[2];
                        data.Add(item);
                    }
                }
                IPOptions = data.ToArray();
            }
            else if (e.Index == 6)
            {
                SubnetMask = ToAddressString(e.Value);
            }
            else if (e.Index == 7)
            {
                GatewayIPAddress = ToAddressString(e.Value);
            }
            else if (e.Index == 8)
            {
                UseDHCP = Convert.ToBoolean(e.Value);
            }
            else if (e.Index == 9)
            {
                PrimaryDNSAddress = ToAddressString(e.Value);
            }
            else if (e.Index == 10)
            {
                SecondaryDNSAddress = ToAddressString(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }


        private static IPAddress GetIpaddress(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return IPAddress.Parse("0.0.0.0");
            }
            return IPAddress.Parse(value);
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            DataLinkLayerReference = reader.ReadElementContentAsString("DataLinkLayerReference");
            IPAddress = GetIpaddress(reader.ReadElementContentAsString("IPAddress"));
            List<IPAddress> list = new List<IPAddress>();
            if (reader.IsStartElement("MulticastIPAddress", true))
            {
                while (reader.IsStartElement("Value", false))
                {
                    list.Add(GetIpaddress(reader.ReadElementContentAsString("Value")));
                }
                reader.ReadEndElement("MulticastIPAddress");
            }
            MulticastIPAddress = list.ToArray();
            List<GXDLMSIp4SetupIpOption> ipOptions = new List<GXDLMSIp4SetupIpOption>();
            if (reader.IsStartElement("IPOptions", true))
            {
                while (reader.IsStartElement("IPOption", true))
                {
                    GXDLMSIp4SetupIpOption it = new GXDLMSIp4SetupIpOption();
                    it.Type = (Ip4SetupIpOptionType)reader.ReadElementContentAsInt("Type");
                    it.Length = (byte)reader.ReadElementContentAsInt("Length");
                    it.Data = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("Data"));
                    ipOptions.Add(it);
                }
                //OLD. This can remove in the future.
                while (reader.IsStartElement("IPOptions", true))
                {
                    GXDLMSIp4SetupIpOption it = new GXDLMSIp4SetupIpOption();
                    it.Type = (Ip4SetupIpOptionType)reader.ReadElementContentAsInt("Type");
                    it.Length = (byte)reader.ReadElementContentAsInt("Length");
                    it.Data = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("Data"));
                    ipOptions.Add(it);
                }
                reader.ReadEndElement("IPOptions");
            }
            IPOptions = ipOptions.ToArray();
            SubnetMask = GetIpaddress(reader.ReadElementContentAsString("SubnetMask"));
            GatewayIPAddress = GetIpaddress(reader.ReadElementContentAsString("GatewayIPAddress"));
            UseDHCP = reader.ReadElementContentAsInt("UseDHCP") != 0;
            PrimaryDNSAddress = GetIpaddress(reader.ReadElementContentAsString("PrimaryDNSAddress"));
            SecondaryDNSAddress = GetIpaddress(reader.ReadElementContentAsString("SecondaryDNSAddress"));
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("DataLinkLayerReference", DataLinkLayerReference, 2);
            writer.WriteElementString("IPAddress", Convert.ToString(IPAddress), 3);
            writer.WriteStartElement("MulticastIPAddress", 4);
            if (MulticastIPAddress != null)
            {
                foreach (IPAddress it in MulticastIPAddress)
                {
                    writer.WriteElementString("Value", Convert.ToString(it), 4);
                }
            }
            writer.WriteEndElement();
            writer.WriteStartElement("IPOptions", 5);
            if (IPOptions != null)
            {
                foreach (GXDLMSIp4SetupIpOption it in IPOptions)
                {
                    writer.WriteStartElement("IPOption", 5);
                    writer.WriteElementString("Type", (int)it.Type, 5);
                    writer.WriteElementString("Length", it.Length, 5);
                    writer.WriteElementString("Data", GXDLMSTranslator.ToHex(it.Data), 5);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteElementString("SubnetMask", Convert.ToString(SubnetMask), 6);
            writer.WriteElementString("GatewayIPAddress", Convert.ToString(GatewayIPAddress), 7);
            writer.WriteElementString("UseDHCP", UseDHCP, 8);
            writer.WriteElementString("PrimaryDNSAddress", Convert.ToString(PrimaryDNSAddress), 9);
            writer.WriteElementString("SecondaryDNSAddress", Convert.ToString(SecondaryDNSAddress), 10);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
