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
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using System.Globalization;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSActionSchedule
    /// </summary>
    public class GXDLMSActionSchedule : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSActionSchedule()
        : this("0.0.15.0.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSActionSchedule(string ln)
        : this(ln, 0)
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

        /// <summary>
        /// Script to execute.
        /// </summary>
        public GXDLMSScriptTable Target
        {
            get;
            set;
        }


        [XmlIgnore()]
        [ObsoleteAttribute("Use Target.")]
        public string ExecutedScriptLogicalName
        {
            get;
            set;
        }

        /// <summary>
        /// Zero based script index to execute.
        /// </summary>
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
            if (Target != null)
            {
                return new object[] { LogicalName, Target.LogicalName + " " + ExecutedScriptSelector,
                                  Type, ExecutionTime
                                };
            }
#pragma warning disable CS0618
            return new object[] { LogicalName, ExecutedScriptLogicalName + " " + ExecutedScriptSelector,
                              Type, ExecutionTime
                            };
#pragma warning restore CS0618
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
            //ExecutedScriptLogicalName is static and read only once.
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            //Type is static and read only once.
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //ExecutionTime is static and read only once.
            if (all || !base.IsRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }


        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Executed Script Logical Name",
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
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(2);
                //LN
                if (Target != null)
                {
                    GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(Target.LogicalName));
                }
                else
                {
#pragma warning disable CS0618
                    GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(ExecutedScriptLogicalName));
#pragma warning restore CS0618
                }
                GXCommon.SetData(settings, data, DataType.UInt16, ExecutedScriptSelector);
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
                        //Count
                        data.SetUInt8((byte)2);
                        if (settings != null && settings.Standard == Standard.SaudiArabia)
                        {
                            //Time
                            GXCommon.SetData(settings, data, DataType.Time, new GXTime(it));
                            //Date
                            GXCommon.SetData(settings, data, DataType.Date, new GXDate(it));
                        }
                        else
                        {
                            //Time
                            GXCommon.SetData(settings, data, DataType.OctetString, new GXTime(it));
                            //Date
                            GXCommon.SetData(settings, data, DataType.OctetString, new GXDate(it));
                        }
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
                if (e.Value != null)
                {
                    String ln = GXCommon.ToLogicalName(((object[])e.Value)[0]);
                    Target = (GXDLMSScriptTable)settings.Objects.FindByLN(ObjectType.ScriptTable, ln);
                    if (Target == null)
                    {
                        Target = new GXDLMSScriptTable(ln);
                    }
                    ExecutedScriptSelector = Convert.ToUInt16(((object[])e.Value)[1]);
                }
                else
                {
                    Target = null;
                    ExecutedScriptSelector = 0;
                }
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
                        GXDateTime time;
                        if (it[0] is byte[])
                        {
                            time = (GXDateTime)GXDLMSClient.ChangeType((byte[])it[0], DataType.Time, settings.UseUtc2NormalTime);
                        }
                        else if (it[0] is GXDateTime)
                        {
                            time = (GXDateTime)it[0];
                        }
                        else
                        {
                            throw new Exception("Invalid time.");
                        }
                        time.Skip &= ~(DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.DayOfWeek);
                        GXDateTime date;
                        if (it[1] is byte[])
                        {
                            date = (GXDateTime)GXDLMSClient.ChangeType((byte[])it[1], DataType.Date, settings.UseUtc2NormalTime);
                        }
                        else if (it[1] is GXDateTime)
                        {
                            date = (GXDateTime)it[1];
                        }
                        else
                        {
                            throw new Exception("Invalid date.");
                        }
                        date.Skip &= ~(DateTimeSkips.Hour | DateTimeSkips.Minute | DateTimeSkips.Second | DateTimeSkips.Ms);
                        GXDateTime tmp = new DLMS.GXDateTime(date);
                        tmp.Value = tmp.Value.AddHours(time.Value.Hour);
                        tmp.Value = tmp.Value.AddMinutes(time.Value.Minute);
                        tmp.Value = tmp.Value.AddSeconds(time.Value.Second);
                        tmp.Skip = date.Skip | time.Skip;
                        items.Add(tmp);
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

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            ObjectType ot = (ObjectType)reader.ReadElementContentAsInt("ObjectType");
            string ln = reader.ReadElementContentAsString("LN");
            if (ot != ObjectType.None && !string.IsNullOrEmpty(ln))
            {
                Target = (GXDLMSScriptTable)reader.Objects.FindByLN(ot, ln);
                //if object is not load yet.
                if (Target == null)
                {
                    Target = new GXDLMSScriptTable(ln);
                }
            }
            ExecutedScriptSelector = (UInt16)reader.ReadElementContentAsInt("ExecutedScriptSelector");
            Type = (SingleActionScheduleType)reader.ReadElementContentAsInt("Type");
            List<GXDateTime> list = new List<GXDateTime>();
            if (reader.IsStartElement("ExecutionTime", true))
            {
                while (reader.IsStartElement("Time", false))
                {
                    GXDateTime it = new GXDateTime(reader.ReadElementContentAsString("Time"), CultureInfo.InvariantCulture);
                    list.Add(it);
                }
                reader.ReadEndElement("ExecutionTime");
            }
            ExecutionTime = list.ToArray();
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (Target != null)
            {
                writer.WriteElementString("ObjectType", (int)Target.ObjectType);
                writer.WriteElementString("LN", Target.LogicalName);
            }
            writer.WriteElementString("ExecutedScriptSelector", ExecutedScriptSelector);
            writer.WriteElementString("Type", (int)Type);
            if (ExecutionTime != null)
            {
                writer.WriteStartElement("ExecutionTime");
                foreach (GXDateTime it in ExecutionTime)
                {
                    writer.WriteElementString("Time", it.ToFormatString(CultureInfo.InvariantCulture));
                }
                writer.WriteEndElement();
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
            //Upload target after load.
            if (Target != null)
            {
                GXDLMSScriptTable target = (GXDLMSScriptTable)reader.Objects.FindByLN(ObjectType.ScriptTable, Target.LogicalName);
                if (target != Target)
                {
                    Target = target;
                }
            }
        }
    }
}
