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
    public class GXDLMSSpecialDaysTable : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSSpecialDaysTable()
            : base(ObjectType.SpecialDaysTable, "0.0.11.0.0.255", 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSSpecialDaysTable(string ln)
            : base(ObjectType.SpecialDaysTable, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSSpecialDaysTable(string ln, ushort sn)
            : base(ObjectType.SpecialDaysTable, ln, 0)
        {
        }

        /// <inheritdoc cref="GXDLMSObject.LogicalName"/>
        [DefaultValue("0.0.11.0.0.255")]
        override public string LogicalName
        {
            get;
            set;
        }

        /// <summary>
        /// Value of COSEM Data object.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(2)]
        public GXDLMSSpecialDay[] Entries
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, Entries };
        }

        #region IGXDLMSBase Members

        int IGXDLMSBase.GetAttributeCount()
        {
            return 2;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
        }

        object IGXDLMSBase.GetValue(int index, out DataType type, byte[] parameters)
        {
            if (index == 1)
            {
                type = DataType.OctetString;
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                type = DataType.Array;
                int cnt = Entries.Length;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
                //Add count            
                GXCommon.SetObjectCount(cnt, data);
                if (cnt != 0)
                {
                    foreach (GXDLMSSpecialDay it in Entries)
                    {
                        data.Add((byte)DataType.Structure);
                        data.Add((byte)3); //Count
                        GXCommon.SetData(data, DataType.UInt16, it.Index);
                        GXCommon.SetData(data, DataType.DateTime, it.Date);
                        GXCommon.SetData(data, DataType.UInt8, it.DayId);
                    }
                }
                return data.ToArray();
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value)
        {
            if (index == 1)
            {
                LogicalName = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
            }
            else if (index == 2)
            {
                Entries = null;
                if (value != null)
                {
                    List<GXDLMSSpecialDay> items = new List<GXDLMSSpecialDay>();
                    foreach (Object[] item in (Object[])value)
                    {
                        GXDLMSSpecialDay it = new GXDLMSSpecialDay();
                        it.Index = Convert.ToUInt16(item[0]);
                        it.Date = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[1], DataType.DateTime);
                        it.DayId = Convert.ToByte(item[2]);
                        items.Add(it);
                    }
                    Entries = items.ToArray();
                }
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }

        void IGXDLMSBase.Invoke(int index, object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
        }

        #endregion
    }
}
