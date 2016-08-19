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
using Gurux.DLMS;
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSPppSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSPppSetup()
            : this(null)
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
            string str = "";
            if (UserName != null)
            {
                str = ASCIIEncoding.ASCII.GetString(UserName);
            }
            if (Password != null)
            {
                str += " " + ASCIIEncoding.ASCII.GetString(Password);
            }
            return new object[] { LogicalName, PHYReference, LCPOptions, IPCPOptions, str };
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
            //PHYReference
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }
            //LCPOptions
            if (!base.IsRead(3))
            {
                attributes.Add(3);
            }
            //IPCPOptions
            if (!base.IsRead(4))
            {
                attributes.Add(4);
            }
            //PPPAuthentication
            if (!base.IsRead(5))
            {
                attributes.Add(5);
            }            
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "PHY Reference", 
                                "LCP Options", "IPCP Options", "PPP Authentication"};            
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

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
                return DataType.Structure;
            } 
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return this.LogicalName;
            }
            if (e.Index == 2)
            {
                return PHYReference;
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
                    data.SetUInt8((byte)IPCPOptions.Length);
                    foreach (GXDLMSPppSetupLcpOption it in LCPOptions)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8((byte)3);
                        GXCommon.SetData(data, DataType.UInt8, it.Type);
                        GXCommon.SetData(data, DataType.UInt8, it.Length);
                        GXCommon.SetData(data, GXCommon.GetValueType(it.Data), it.Data);
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
                        GXCommon.SetData(data, DataType.UInt8, it.Type);
                        GXCommon.SetData(data, DataType.UInt8, it.Length);
                        GXCommon.SetData(data, GXCommon.GetValueType(it.Data), it.Data);
                    }
                }
                return data.Array();
            }
            else if (e.Index == 5)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(2);
                GXCommon.SetData(data, DataType.OctetString, UserName);
                GXCommon.SetData(data, DataType.OctetString, Password);
                return data.Array();
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e) 
        {
            if (e.Index == 1)
            {
                if (e.Value is string)
                {
                    LogicalName = e.Value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString).ToString();
                }
            }
            else if (e.Index == 2)
            {
                if (e.Value is string)
                {
                    PHYReference = e.Value.ToString();
                }
                else
                {
                    PHYReference = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString).ToString();
                }
            }
            else if (e.Index == 3)
            {
                List<GXDLMSPppSetupLcpOption> items = new List<GXDLMSPppSetupLcpOption>();
                if (e.Value is Object[])
                {
                    foreach (Object[] item in (Object[])e.Value)
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
                if (e.Value is Object[])
                {
                    foreach (Object[] item in (Object[])e.Value)
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
                UserName = (byte[])((Object[])e.Value)[0];
                Password = (byte[])((Object[])e.Value)[1];
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }
        #endregion
    }
}
