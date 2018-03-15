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
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSSapAssignment
    /// </summary>
    public class GXDLMSSapAssignment : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSapAssignment()
        : base(ObjectType.SapAssignment, "0.0.41.0.0.255", 0)
        {
            SapAssignmentList = new List<KeyValuePair<UInt16, string>>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSSapAssignment(string ln)
        : base(ObjectType.SapAssignment, ln, 0)
        {
            SapAssignmentList = new List<KeyValuePair<UInt16, string>>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSSapAssignment(string ln, ushort sn)
        : base(ObjectType.SapAssignment, ln, sn)
        {
            SapAssignmentList = new List<KeyValuePair<UInt16, string>>();
        }

        [XmlIgnore()]
        public List<KeyValuePair<UInt16, string>> SapAssignmentList
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, SapAssignmentList };
        }

        #region IGXDLMSBase Members

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //SapAssignmentList
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Sap Assignment List" };
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
                int cnt = 0;
                if (SapAssignmentList != null)
                {
                    cnt = SapAssignmentList.Count;
                }
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                //Add count
                GXCommon.SetObjectCount(cnt, data);
                if (cnt != 0)
                {
                    foreach (var it in SapAssignmentList)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8((byte)2); //Count
                        GXCommon.SetData(settings, data, DataType.UInt16, it.Key);
                        GXCommon.SetData(settings, data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Value));
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
                SapAssignmentList.Clear();
                if (e.Value != null)
                {
                    foreach (Object[] item in (Object[])e.Value)
                    {
                        string str;
                        if (item[1] is byte[])
                        {
                            str = GXDLMSClient.ChangeType((byte[])item[1], DataType.String, settings.UseUtc2NormalTime).ToString();
                        }
                        else
                        {
                            str = Convert.ToString(item[1]);
                        }
                        SapAssignmentList.Add(new KeyValuePair<UInt16, string>(Convert.ToUInt16(item[0]), str));
                    }
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            SapAssignmentList.Clear();
            if (reader.IsStartElement("SapAssignmentList", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    UInt16 sap = (UInt16)reader.ReadElementContentAsInt("SAP");
                    string ldn = reader.ReadElementContentAsString("LDN");
                    SapAssignmentList.Add(new KeyValuePair<UInt16, string>(sap, ldn));
                }
                reader.ReadEndElement("SapAssignmentList");
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (SapAssignmentList != null)
            {
                writer.WriteStartElement("SapAssignmentList");
                foreach (KeyValuePair<UInt16, string> it in SapAssignmentList)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("SAP", it.Key);
                    writer.WriteElementString("LDN", it.Value);
                    writer.WriteEndElement();
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
