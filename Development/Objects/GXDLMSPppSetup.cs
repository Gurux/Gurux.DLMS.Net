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

namespace Gurux.DLMS.Objects
{
    public class GXDLMSPppSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSPppSetup()
            : base(ObjectType.PppSetup)
        {
            LCPOptions = new GXDLMSPppSetupLcpOption[0];
            IPCPOptions = new GXDLMSPppSetupIPCPOption[0];
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSPppSetup(string ln)
            : base(ObjectType.PppSetup, ln, 0)
        {
            LCPOptions = new GXDLMSPppSetupLcpOption[0];
            IPCPOptions = new GXDLMSPppSetupIPCPOption[0];
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSPppSetup(string ln, ushort sn)
            : base(ObjectType.PppSetup, ln, 0)
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

        public enum GXDLMSPppSetupPppAuthenticationType
        {
            /// <summary>
            /// No authentication.
            /// </summary>
            None = 0,
            /// <summary>
            /// PAP Login
            /// </summary>
            PAP = 1,
            /// <summary>
            /// CHAP-algorithm
            /// </summary>
            CHAP = 2
        }

        [XmlIgnore()]
        public object PPPAuthentication
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, PHYReference, LCPOptions, IPCPOptions, PPPAuthentication };
        }

        #region IGXDLMSBase Members


        void IGXDLMSBase.Invoke(int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
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

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }         

        object IGXDLMSBase.GetValue(int index, out DataType type, byte[] parameters, bool raw)
        {
            if (index == 1)
            {
                type = DataType.OctetString;
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                type = DataType.OctetString;
                return PHYReference;
            }
            if (index == 3)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
                if (LCPOptions == null)
                {
                    data.Add(0);
                }
                else
                {
                    data.Add((byte)IPCPOptions.Length);
                    foreach (GXDLMSPppSetupLcpOption it in LCPOptions)
                    {
                        data.Add((byte)DataType.Structure);
                        data.Add((byte)3);
                        GXCommon.SetData(data, DataType.UInt8, it.Type);
                        GXCommon.SetData(data, DataType.UInt8, it.Length);
                        GXCommon.SetData(data, GXCommon.GetValueType(it.Data), it.Data);
                    }
                }
                return data.ToArray();
            }
            if (index == 4)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
                if (PPPAuthentication == null)
                {
                    data.Add(0);
                }
                else
                {
                    data.Add((byte)IPCPOptions.Length);
                    foreach (GXDLMSPppSetupIPCPOption it in IPCPOptions)
                    {
                        data.Add((byte)DataType.Structure);
                        data.Add((byte)3);
                        GXCommon.SetData(data, DataType.UInt8, it.Type);
                        GXCommon.SetData(data, DataType.UInt8, it.Length);
                        GXCommon.SetData(data, GXCommon.GetValueType(it.Data), it.Data);
                    }
                }
                return data.ToArray();
            }
            if (index == 5)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
                if (IPCPOptions == null)
                {
                    data.Add(0);
                }
                else
                {
                    data.Add((byte)IPCPOptions.Length);
                    foreach (GXDLMSPppSetupIPCPOption it in IPCPOptions)
                    {
                        data.Add((byte)DataType.Structure);
                        data.Add((byte)3);
                        GXCommon.SetData(data, DataType.UInt8, it.Type);
                        GXCommon.SetData(data, DataType.UInt8, it.Length);
                        GXCommon.SetData(data, GXCommon.GetValueType(it.Data), it.Data);
                    }
                }
                return data.ToArray();
            }
            //TODO:
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value, bool raw)
        {
            if (index == 1)
            {
                if (value is string)
                {
                    LogicalName = value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
                }
            }
            else if (index == 2)
            {
                if (value is string)
                {
                    PHYReference = value.ToString();
                }
                else
                {
                    PHYReference = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
                }
            }
            else if (index == 3)
            {
                List<GXDLMSPppSetupLcpOption> items = new List<GXDLMSPppSetupLcpOption>();                
                if (value != null)
                {
                    foreach (Object[] item in (Object[])value)
                    {
                        GXDLMSPppSetupLcpOption it = new GXDLMSPppSetupLcpOption();
                        it.Type = (GXDLMSPppSetupLcpOptionType)Convert.ToByte(item[0]);
                        it.Length = Convert.ToByte(item[1]);
                        it.Data = item[2];
                        items.Add(it);
                    }
                }
                LCPOptions = items.ToArray();
            }
            else if (index == 4)
            {
                List<GXDLMSPppSetupIPCPOption> items = new List<GXDLMSPppSetupIPCPOption>();
                if (value != null)
                {
                    foreach (Object[] item in (Object[])value)
                    {
                        GXDLMSPppSetupIPCPOption it = new GXDLMSPppSetupIPCPOption();
                        it.Type = (GXDLMSPppSetupIPCPOptionType)Convert.ToByte(item[0]);
                        it.Length = Convert.ToByte(item[1]);
                        it.Data = item[2];
                        items.Add(it);
                    }
                }
                IPCPOptions = items.ToArray();
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
        #endregion
    }
}
