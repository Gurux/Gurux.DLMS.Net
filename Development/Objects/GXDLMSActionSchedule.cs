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
    public class GXDLMSActionSchedule : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSActionSchedule()
        : base(ObjectType.ActionSchedule)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSActionSchedule(string ln)
        : base(ObjectType.ActionSchedule, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSActionSchedule(string ln, ushort sn)
        : base(ObjectType.ActionSchedule, ln, sn)
        {
        }

        [XmlIgnore()]
        public string ExecutedScriptLogicalName
        {
            get;
            set;
        }

        [XmlIgnore()]
        public UInt16 ExecutedScriptSelector
        {
            get;
            set;
        }

        [XmlIgnore()]
        public SingleActionScheduleType Type
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDateTime[] ExecutionTime
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ExecutedScriptLogicalName + " " + ExecutedScriptSelector,
                              Type, ExecutionTime
                            };
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
            //ExecutedScriptLogicalName is static and read only once.
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }
            //Type is static and read only once.
            if (!base.IsRead(3))
            {
                attributes.Add(3);
            }
            //ExecutionTime is static and read only once.
            if (!base.IsRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }


        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Executed Script Logical Name",
                              "Type", "Execution Time"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 4;
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
                return DataType.Array;
            }
            if (index == 3)
            {
                return DataType.Enum;
            }
            if (index == 4)
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
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(2);
                //LN
                GXCommon.SetData(data, DataType.OctetString, ExecutedScriptLogicalName);
                GXCommon.SetData(data, DataType.UInt16, ExecutedScriptSelector);
                return data.Array();
            }
            if (e.Index == 3)
            {
                return this.Type;
            }
            if (e.Index == 4)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (ExecutionTime == null)
                {
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    GXCommon.SetObjectCount(ExecutionTime.Length, data);
                    foreach (GXDateTime it in ExecutionTime)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8((byte)2); //Count
                                                //Time
                        GXCommon.SetData(data, DataType.OctetString, new GXTime(it));
                        //Date
                        GXCommon.SetData(data, DataType.OctetString, new GXDate(it));
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
                ExecutedScriptLogicalName = GXDLMSClient.ChangeType((byte[])((object[])e.Value)[0], DataType.OctetString).ToString();
                ExecutedScriptSelector = Convert.ToUInt16(((object[])e.Value)[1]);
            }
            else if (e.Index == 3)
            {
                Type = (SingleActionScheduleType)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 4)
            {
                ExecutionTime = null;
                if (e.Value != null)
                {
                    List<GXDateTime> items = new List<GXDateTime>();
                    foreach (object[] it in (object[])e.Value)
                    {
                        GXDateTime tm = (GXDateTime)GXDLMSClient.ChangeType((byte[])it[0], DataType.Time);
                        GXDateTime date = (GXDateTime)GXDLMSClient.ChangeType((byte[])it[1], DataType.Date);
                        tm.Value.AddYears(date.Value.Year - 1);
                        tm.Value.AddMonths(date.Value.Month - 1);
                        tm.Value.AddDays(date.Value.Day - 1);
                        tm.Skip |= date.Skip;
                        items.Add(tm);
                    }
                    ExecutionTime = items.ToArray();
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        #endregion

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }
    }
}
