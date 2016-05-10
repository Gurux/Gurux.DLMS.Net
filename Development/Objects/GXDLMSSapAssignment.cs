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

namespace Gurux.DLMS.Objects
{
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

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //SapAssignmentList
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Sap Assignment List" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 2;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

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
                return this.LogicalName;
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
                        GXCommon.SetData(data, DataType.UInt16, it.Key);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Value));
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
                SapAssignmentList.Clear();
                if (e.Value != null)
                {
                    foreach (Object[] item in (Object[])e.Value)
                    {
                        string str;
                        if (item[1] is byte[])
                        {
                            str = GXDLMSClient.ChangeType((byte[])item[1], DataType.String).ToString();
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

        #endregion
    }
}
