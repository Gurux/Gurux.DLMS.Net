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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.SFSKReportingSystemList
    /// </summary>
    public class SFSKReportingSystemList : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SFSKReportingSystemList()
        : this("0.0.26.6.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public SFSKReportingSystemList(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public SFSKReportingSystemList(string ln, ushort sn)
        : base(ObjectType.SFSKReportingSystemList, ln, sn)
        {
            ReportingSystemList = new List<byte[]>();
        }

        /// <summary>
        /// Contains the system titles of the server systems which have made a
        /// DiscoverReport request and which have not already been registered.
        /// </summary>
        [XmlIgnore()]
        public List<byte[]> ReportingSystemList
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ReportingSystemList };
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
            //ReportingSystemList
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "ReportingSystemList" };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[0];
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 2;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.Array;
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
                    {
                        int cnt = ReportingSystemList.Count;
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        //Add count
                        GXCommon.SetObjectCount(cnt, data);
                        foreach (byte[] it in ReportingSystemList)
                        {
                            GXCommon.SetData(settings, data, DataType.OctetString, it);
                        }
                        return data.Array();
                    }
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
                    {
                        ReportingSystemList.Clear();
                        if (e.Value is IEnumerable<object>)
                        {
                            foreach (byte[] tmp in (IEnumerable<object>)e.Value)
                            {
                                ReportingSystemList.Add(tmp);
                            }
                        }
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            ReportingSystemList.Clear();
            if (reader.IsStartElement("ReportingSystems", true))
            {
                while (reader.IsStartElement("Item", false))
                {
                    ReportingSystemList.Add(GXCommon.HexToBytes(reader.ReadElementContentAsString("Item")));
                }
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (ReportingSystemList != null)
            {
                writer.WriteStartElement("ReportingSystems", 2);
                foreach (byte[] it in ReportingSystemList)
                {
                    writer.WriteElementString("Item", GXCommon.ToHex(it, false), 0);
                }
                writer.WriteEndElement();
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
