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
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSPppSetup
    /// </summary>
    public class GXDLMSPppSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSPppSetup()
        : this("0.0.25.3.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSPppSetup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSPppSetup(string ln, ushort sn)
        : base(ObjectType.PppSetup, ln, sn)
        {
            LCPOptions = new GXDLMSPppSetupLcpOption[0];
            IPCPOptions = new GXDLMSPppSetupIPCPOption[0];
        }

        [XmlIgnore()]
        public string PHYReference
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDLMSPppSetupLcpOption[] LCPOptions
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDLMSPppSetupIPCPOption[] IPCPOptions
        {
            get;
            set;
        }

        /// <summary>
        /// PPP authentication procedure type.
        /// </summary>
        [XmlIgnore()]
        public PppAuthenticationType Authentication
        {
            get;
            set;
        }

        /// <summary>
        /// PPP authentication procedure user name.
        /// </summary>
        [XmlIgnore()]
        public byte[] UserName
        {
            get;
            set;
        }

        /// <summary>
        /// PPP authentication procedure password.
        /// </summary>
        [XmlIgnore()]
        public byte[] Password
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            StringBuilder sb = new StringBuilder();
            if (UserName != null)
            {
                sb.Append(ASCIIEncoding.ASCII.GetString(UserName));
            }
            if (Password != null)
            {
                if (sb.Length != 0)
                {
                    sb.Append(" ");
                }
                sb.Append(ASCIIEncoding.ASCII.GetString(Password));
            }
            return new object[] { LogicalName, PHYReference, LCPOptions, IPCPOptions, sb.ToString() };
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
            //PHYReference
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            //LCPOptions
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //IPCPOptions
            if (all || !base.IsRead(4))
            {
                attributes.Add(4);
            }
            //PPPAuthentication
            if (all || !base.IsRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "PHY Reference",
                              "LCP Options", "IPCP Options", "PPP Authentication"
                            };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[0];
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
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
                return DataType.Array;
            }
            if (index == 4)
            {
                return DataType.Array;
            }
            if (index == 5)
            {
                if (UserName == null || UserName.Length == 0)
                {
                    return DataType.None;
                }
                return DataType.Structure;
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
                return GXCommon.LogicalNameToBytes(PHYReference);
            }
            if (e.Index == 3)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (LCPOptions == null)
                {
                    data.SetUInt8(0);
                }
                else
                {
                    data.SetUInt8((byte)LCPOptions.Length);
                    foreach (GXDLMSPppSetupLcpOption it in LCPOptions)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8((byte)3);
                        GXCommon.SetData(settings, data, DataType.UInt8, it.Type);
                        GXCommon.SetData(settings, data, DataType.UInt8, it.Length);
                        GXCommon.SetData(settings, data, GXDLMSConverter.GetDLMSDataType(it.Data), it.Data);
                    }
                }
                return data.Array();
            }
            if (e.Index == 4)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (IPCPOptions == null)
                {
                    data.SetUInt8(0);
                }
                else
                {
                    data.SetUInt8((byte)IPCPOptions.Length);
                    foreach (GXDLMSPppSetupIPCPOption it in IPCPOptions)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8((byte)3);
                        GXCommon.SetData(settings, data, DataType.UInt8, it.Type);
                        GXCommon.SetData(settings, data, DataType.UInt8, it.Length);
                        GXCommon.SetData(settings, data, GXDLMSConverter.GetDLMSDataType(it.Data), it.Data);
                    }
                }
                return data.Array();
            }
            else if (e.Index == 5)
            {
                if (UserName == null || UserName.Length == 0)
                {
                    return null;
                }
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(2);
                GXCommon.SetData(settings, data, DataType.OctetString, UserName);
                GXCommon.SetData(settings, data, DataType.OctetString, Password);
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
                PHYReference = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 3)
            {
                List<GXDLMSPppSetupLcpOption> items = new List<GXDLMSPppSetupLcpOption>();
                if (e.Value is List<object>)
                {
                    foreach (List<object> item in (List<object>)e.Value)
                    {
                        GXDLMSPppSetupLcpOption it = new GXDLMSPppSetupLcpOption();
                        it.Type = (PppSetupLcpOptionType)Convert.ToByte(item[0]);
                        it.Length = Convert.ToByte(item[1]);
                        it.Data = item[2];
                        items.Add(it);
                    }
                }
                LCPOptions = items.ToArray();
            }
            else if (e.Index == 4)
            {
                List<GXDLMSPppSetupIPCPOption> items = new List<GXDLMSPppSetupIPCPOption>();
                if (e.Value is List<object>)
                {
                    foreach (List<object> item in (List<object>)e.Value)
                    {
                        GXDLMSPppSetupIPCPOption it = new GXDLMSPppSetupIPCPOption();
                        it.Type = (PppSetupIPCPOptionType)Convert.ToByte(item[0]);
                        it.Length = Convert.ToByte(item[1]);
                        it.Data = item[2];
                        items.Add(it);
                    }
                }
                IPCPOptions = items.ToArray();
            }
            else if (e.Index == 5)
            {
                if (e.Value != null)
                {
                    UserName = (byte[])((List<object>)e.Value)[0];
                    Password = (byte[])((List<object>)e.Value)[1];
                }
                else
                {
                    UserName = Password = null;
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            PHYReference = reader.ReadElementContentAsString("PHYReference");
            List<GXDLMSPppSetupLcpOption> options = new List<GXDLMSPppSetupLcpOption>();
            if (reader.IsStartElement("LCPOptions", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSPppSetupLcpOption it = new GXDLMSPppSetupLcpOption();
                    it.Type = (PppSetupLcpOptionType)reader.ReadElementContentAsInt("Type");
                    it.Length = (byte)reader.ReadElementContentAsInt("Length");
                    it.Data = reader.ReadElementContentAsObject("Data", null, null, 0);
                }
                reader.ReadEndElement("LCPOptions");
            }
            LCPOptions = options.ToArray();

            List<GXDLMSPppSetupIPCPOption> list = new List<GXDLMSPppSetupIPCPOption>();
            if (reader.IsStartElement("IPCPOptions", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSPppSetupIPCPOption it = new GXDLMSPppSetupIPCPOption();
                    it.Type = (PppSetupIPCPOptionType)reader.ReadElementContentAsInt("Type");
                    it.Length = (byte)reader.ReadElementContentAsInt("Length");
                    it.Data = reader.ReadElementContentAsObject("Data", null, null, 0);
                }
                reader.ReadEndElement("IPCPOptions");
            }
            IPCPOptions = list.ToArray();

            UserName = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("UserName"));
            Password = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("Password"));
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("PHYReference", PHYReference, 2);
            writer.WriteStartElement("LCPOptions", 3);
            if (LCPOptions != null)
            {
                foreach (GXDLMSPppSetupLcpOption it in LCPOptions)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("Type", (int)it.Type, 0);
                    writer.WriteElementString("Length", it.Length, 0);
                    writer.WriteElementObject("Data", it.Data, 0);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteStartElement("IPCPOptions", 4);
            if (IPCPOptions != null)
            {
                foreach (GXDLMSPppSetupIPCPOption it in IPCPOptions)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("Type", (int)it.Type, 0);
                    writer.WriteElementString("Length", it.Length, 0);
                    writer.WriteElementObject("Data", it.Data, 0);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteElementString("UserName", GXDLMSTranslator.ToHex(UserName), 5);
            writer.WriteElementString("Password", GXDLMSTranslator.ToHex(Password), 5);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
