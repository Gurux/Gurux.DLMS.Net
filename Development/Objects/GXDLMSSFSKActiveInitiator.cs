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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSSFSKActiveInitiator
    /// </summary>
    public class GXDLMSSFSKActiveInitiator : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSFSKActiveInitiator()
        : this("0.0.26.1.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSSFSKActiveInitiator(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSSFSKActiveInitiator(string ln, ushort sn)
        : base(ObjectType.SFSKActiveInitiator, ln, sn)
        {
        }

        /// <summary>
        /// System title of active initiator.
        /// </summary>
        [XmlIgnore()]
        public byte[] SystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// MAC address of active initiator.
        /// </summary>
        [XmlIgnore()]
        public UInt16 MacAddress
        {
            get;
            set;
        }
        /// <summary>
        /// L SAP selector of active initiator.
        /// </summary>
        [XmlIgnore()]
        public byte LSapSelector
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, new object[] { SystemTitle, MacAddress, LSapSelector } };
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
            //Active Initiator
            attributes.Add(2);
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Active Initiator" };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Reset NEW not synchronized" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 2;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
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
                return DataType.Structure;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            object ret;
            if (e.Index == 1)
            {
                ret = GXCommon.LogicalNameToBytes(LogicalName);
            }
            else if (e.Index == 2)
            {
                GXByteBuffer bb = new GXByteBuffer();
                bb.SetUInt8(DataType.Structure);
                bb.SetUInt8(3);
                GXCommon.SetData(settings, bb, DataType.OctetString, SystemTitle);
                GXCommon.SetData(settings, bb, DataType.UInt16, MacAddress);
                GXCommon.SetData(settings, bb, DataType.UInt8, LSapSelector);
                ret = bb.Array();
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
                ret = null;
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
                        if (e.Value != null)
                        {
                            List<object> tmp;
                            if (e.Value is List<object>)
                            {
                                tmp = (List<object>)e.Value;
                            }
                            else
                            {
                                tmp = new List<object>((object[])e.Value);
                            }
                            SystemTitle = (byte[])tmp[0];
                            MacAddress = (ushort)tmp[1];
                            LSapSelector = (byte)tmp[2];
                        }
                        else
                        {
                            SystemTitle = null;
                            MacAddress = 0;
                            LSapSelector = 0;
                        }
                        break;
                    }

                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            SystemTitle = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("SystemTitle"));
            MacAddress = (UInt16)reader.ReadElementContentAsInt("MacAddress");
            LSapSelector = (byte)reader.ReadElementContentAsInt("LSapSelector");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("SystemTitle", GXDLMSTranslator.ToHex(SystemTitle), 2);
            writer.WriteElementString("MacAddress", MacAddress, 3);
            writer.WriteElementString("LSapSelector", LSapSelector, 4);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
