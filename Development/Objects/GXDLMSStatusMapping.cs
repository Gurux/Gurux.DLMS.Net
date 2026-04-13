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
    /// Represents a COSEM StatusMapping object that holds a value and provides access to its logical name and StatusMapping attributes.
    /// </summary>
    /// <remarks>
    /// Online help:
    /// http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSStatusMapping
    /// </remarks>
    public class GXDLMSStatusMapping : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Current value of the status word.
        /// </summary>
        [XmlIgnore()]
        public object StatusWord
        {
            get;
            set;
        }

        /// <summary>
        /// Current value of the mapping table.
        /// </summary>
        [XmlIgnore()]
        public GXMappingTable MappingTable
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSStatusMapping()
        : this("0.0.96.5.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSStatusMapping(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSStatusMapping(string ln, ushort sn)
        : base(ObjectType.StatusMapping, ln, sn)
        {
            MappingTable = new GXMappingTable();
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, StatusWord, MappingTable };
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
            //StatusWord
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //MappingTable
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { GXCommon.GetLogicalNameString(), "Status word", "Mapping table" };
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
            return 3;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    //Logical name.
                    return DataType.OctetString;
                case 2:
                    //StatusWord.
                    DataType dt = base.GetDataType(index);
                    if (dt == DataType.None && StatusWord != null)
                    {
                        dt = GXCommon.GetDLMSDataType(StatusWord.GetType());
                        //If user has set initial value.
                        if (dt == DataType.String)
                        {
                            dt = DataType.None;
                        }
                    }
                    return dt;
                case 3:
                    //MappingTable.
                    return DataType.Structure;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        public override DataType GetUIDataType(int index)
        {
            return GetDataType(index);
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return StatusWord;
                case 3:
                    GXByteBuffer data = new GXByteBuffer();
                    data.SetUInt8((byte)DataType.Structure);
                    //Add count
                    data.SetUInt8((byte)2);
                    GXCommon.SetData(settings, data, DataType.UInt8, MappingTable.RefefenceTableId);
                    GXCommon.SetData(settings, data, GXDLMSConverter.GetDLMSDataType(MappingTable.RefefenceTableMapping), MappingTable.RefefenceTableMapping);
                    return data.Array();
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
                    StatusWord = e.Value;
                    break;
                case 3:
                    GXStructure s = e.Value as GXStructure;
                    if (s != null && s.Count == 2)
                    {
                        MappingTable.RefefenceTableId = (byte)s[0];
                        MappingTable.RefefenceTableMapping = s[1];
                    }
                    else
                    {
                        e.Error = ErrorCode.ReadWriteDenied;
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            StatusWord = reader.ReadElementContentAsObject("StatusWord", null, this, 2);
            MappingTable.RefefenceTableId = (byte)reader.ReadElementContentAsInt("TableId");
            MappingTable.RefefenceTableMapping = reader.ReadElementContentAsObject("RefefenceTableMapping", null, this, 3);
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementObject("StatusWord", StatusWord, GetDataType(2), GetUIDataType(2));
            writer.WriteElementString("TableId", MappingTable.RefefenceTableId);
            DataType dt = DataType.None;
            if (MappingTable.RefefenceTableMapping != null)
            {
                dt = GXCommon.GetDLMSDataType(MappingTable.RefefenceTableMapping.GetType());
            }
            writer.WriteElementObject("RefefenceTableMapping", MappingTable.RefefenceTableMapping, dt, DataType.None);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
