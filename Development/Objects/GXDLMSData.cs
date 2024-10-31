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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSData
    /// </summary>
    public class GXDLMSData : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Value of COSEM Data object.
        /// </summary>
        [XmlIgnore()]
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSData()
        : this(null, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSData(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSData(string ln, ushort sn)
        : base(ObjectType.Data, ln, sn)
        {
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Value };
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
            return new string[] { GXCommon.GetLogicalNameString(), "Value" };
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
            return 2;
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
                    //Logical name.
                    return DataType.OctetString;
                case 2:
                    {
                        //Value.
                        DataType dt = base.GetDataType(index);
                        if (dt == DataType.None && Value != null)
                        {
                            dt = GXCommon.GetDLMSDataType(Value.GetType());
                            //If user has set initial value.
                            if (dt == DataType.String)
                            {
                                dt = DataType.None;
                            }
                        }
                        return dt;
                    }
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return Value;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    DataType dt = GetDataType(2);
                    if (!e.User && e.Value != null && (dt == DataType.None || dt == DataType.DateTime || dt == DataType.String))
                    {
                        DataType dt2 = GXCommon.GetDLMSDataType(e.Value.GetType());
                        if (dt != dt2)
                        {
                            SetDataType(2, dt2);
                        }
                    }
                    dt = GetUIDataType(2);
                    if (dt == DataType.DateTime && (e.Value is UInt32 || e.Value is UInt64 || e.Value is Int32 || e.Value is Int64))
                    {
                        Value = GXDateTime.FromUnixTime(Convert.ToUInt32(e.Value)).Value.UtcDateTime;
                    }
                    else
                    {
                        Value = e.Value;
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Value = reader.ReadElementContentAsObject("Value", null, this, 2);
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementObject("Value", Value, GetDataType(2), GetUIDataType(2), 2);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
