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
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSActivityCalendar : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSActivityCalendar()
        : base(ObjectType.ActivityCalendar, "0.0.13.0.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSActivityCalendar(string ln)
        : base(ObjectType.ActivityCalendar, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSActivityCalendar(string ln, ushort sn)
        : base(ObjectType.ActivityCalendar, ln, sn)
        {
        }

        [XmlIgnore()]
        public string CalendarNameActive
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDLMSSeasonProfile[] SeasonProfileActive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        public GXDLMSWeekProfile[] WeekProfileTableActive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        public GXDLMSDayProfile[] DayProfileTableActive
        {
            get;
            set;
        }
        [XmlIgnore()]
        public string CalendarNamePassive
        {
            get;
            set;
        }
        [XmlIgnore()]
        public GXDLMSSeasonProfile[] SeasonProfilePassive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        public GXDLMSWeekProfile[] WeekProfileTablePassive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        public GXDLMSDayProfile[] DayProfileTablePassive
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDateTime Time
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, CalendarNameActive, SeasonProfileActive,
                              WeekProfileTableActive, DayProfileTableActive, CalendarNamePassive,
                              SeasonProfilePassive, WeekProfileTablePassive, DayProfileTablePassive, Time
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
            //CalendarNameActive
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //SeasonProfileActive
            if (CanRead(3))
            {
                attributes.Add(3);
            }

            //WeekProfileTableActive
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            //DayProfileTableActive
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            //CalendarNamePassive
            if (CanRead(6))
            {
                attributes.Add(6);
            }
            //SeasonProfilePassive
            if (CanRead(7))
            {
                attributes.Add(7);
            }
            //WeekProfileTablePassive
            if (CanRead(8))
            {
                attributes.Add(8);
            }
            //DayProfileTablePassive
            if (CanRead(9))
            {
                attributes.Add(9);
            }
            //Time.
            if (CanRead(10))
            {
                attributes.Add(10);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Active Calendar Name ", "Active Season Profile", "Active Week Profile Table",
                             "Active Day Profile Table", "Passive Calendar Name", "Passive Season Profile", "Passive Week Profile Table", "Passive Day Profile Table", "Time"
                            };

        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 10;
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
                return DataType.OctetString;
            }
            if (index == 3)
            {
                return DataType.Array;
            }
            if (index == 4)
            {
                return DataType.Array;
            }
            if (index == 5)
            {
                return DataType.Array;
            }
            if (index == 6)
            {
                return DataType.OctetString;
            }
            //
            if (index == 7)
            {
                return DataType.Array;
            }
            if (index == 8)
            {
                return DataType.Array;
            }
            if (index == 9)
            {
                return DataType.Array;
            }
            if (index == 10)
            {
                return DataType.OctetString;
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
                if (CalendarNameActive == null)
                {
                    return null;
                }
                return GXDLMSClient.ChangeType(ASCIIEncoding.ASCII.GetBytes(CalendarNameActive), DataType.OctetString);
            }
            if (e.Index == 3)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (SeasonProfileActive == null)
                {
                    //Add count
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    int cnt = SeasonProfileActive.Length;
                    //Add count
                    GXCommon.SetObjectCount(cnt, data);
                    foreach (GXDLMSSeasonProfile it in SeasonProfileActive)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(3);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Name));
                        GXCommon.SetData(data, DataType.OctetString, it.Start);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.WeekName));
                    }
                }
                return data.Array();
            }
            if (e.Index == 4)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (WeekProfileTableActive == null)
                {
                    //Add count
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    int cnt = WeekProfileTableActive.Length;
                    //Add count
                    GXCommon.SetObjectCount(cnt, data);
                    foreach (GXDLMSWeekProfile it in WeekProfileTableActive)
                    {
                        data.SetUInt8((byte)DataType.Array);
                        data.SetUInt8(8);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Name));
                        GXCommon.SetData(data, DataType.UInt8, it.Monday);
                        GXCommon.SetData(data, DataType.UInt8, it.Tuesday);
                        GXCommon.SetData(data, DataType.UInt8, it.Wednesday);
                        GXCommon.SetData(data, DataType.UInt8, it.Thursday);
                        GXCommon.SetData(data, DataType.UInt8, it.Friday);
                        GXCommon.SetData(data, DataType.UInt8, it.Saturday);
                        GXCommon.SetData(data, DataType.UInt8, it.Sunday);
                    }
                }
                return data.Array();
            }
            if (e.Index == 5)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (DayProfileTableActive == null)
                {
                    //Add count
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    int cnt = DayProfileTableActive.Length;
                    //Add count
                    GXCommon.SetObjectCount(cnt, data);
                    foreach (GXDLMSDayProfile it in DayProfileTableActive)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(2);
                        GXCommon.SetData(data, DataType.UInt8, it.DayId);
                        data.SetUInt8((byte)DataType.Array);
                        //Add count
                        GXCommon.SetObjectCount(it.DaySchedules.Length, data);
                        foreach (GXDLMSDayProfileAction action in it.DaySchedules)
                        {
                            data.SetUInt8((byte)DataType.Structure);
                            data.SetUInt8(3);
                            GXCommon.SetData(data, DataType.Time, action.StartTime);
                            GXCommon.SetData(data, DataType.OctetString, action.ScriptLogicalName);
                            GXCommon.SetData(data, DataType.UInt16, action.ScriptSelector);
                        }
                    }
                }
                return data.Array();
            }
            if (e.Index == 6)
            {
                if (CalendarNamePassive == null)
                {
                    return null;
                }
                return GXDLMSClient.ChangeType(ASCIIEncoding.ASCII.GetBytes(CalendarNamePassive), DataType.OctetString);
            }
            //
            if (e.Index == 7)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (SeasonProfilePassive == null)
                {
                    //Add count
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    int cnt = SeasonProfilePassive.Length;
                    //Add count
                    GXCommon.SetObjectCount(cnt, data);
                    foreach (GXDLMSSeasonProfile it in SeasonProfilePassive)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(3);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Name));
                        GXCommon.SetData(data, DataType.OctetString, it.Start);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.WeekName));
                    }
                }
                return data.Array();
            }
            if (e.Index == 8)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (WeekProfileTablePassive == null)
                {
                    //Add count
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    int cnt = WeekProfileTablePassive.Length;
                    //Add count
                    GXCommon.SetObjectCount(cnt, data);
                    foreach (GXDLMSWeekProfile it in WeekProfileTablePassive)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(8);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Name));
                        GXCommon.SetData(data, DataType.UInt8, it.Monday);
                        GXCommon.SetData(data, DataType.UInt8, it.Tuesday);
                        GXCommon.SetData(data, DataType.UInt8, it.Wednesday);
                        GXCommon.SetData(data, DataType.UInt8, it.Thursday);
                        GXCommon.SetData(data, DataType.UInt8, it.Friday);
                        GXCommon.SetData(data, DataType.UInt8, it.Saturday);
                        GXCommon.SetData(data, DataType.UInt8, it.Sunday);
                    }
                }
                return data.Array();
            }
            if (e.Index == 9)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (DayProfileTablePassive == null)
                {
                    //Add count
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    int cnt = DayProfileTablePassive.Length;
                    //Add count
                    GXCommon.SetObjectCount(cnt, data);
                    foreach (GXDLMSDayProfile it in DayProfileTablePassive)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(2);
                        GXCommon.SetData(data, DataType.UInt8, it.DayId);
                        data.SetUInt8((byte)DataType.Array);
                        //Add count
                        GXCommon.SetObjectCount(it.DaySchedules.Length, data);
                        foreach (GXDLMSDayProfileAction action in it.DaySchedules)
                        {
                            data.SetUInt8((byte)DataType.Structure);
                            data.SetUInt8(3);
                            GXCommon.SetData(data, DataType.Time, action.StartTime);
                            GXCommon.SetData(data, DataType.OctetString, action.ScriptLogicalName);
                            GXCommon.SetData(data, DataType.UInt16, action.ScriptSelector);
                        }
                    }
                }
                return data.Array();
            }
            if (e.Index == 10)
            {
                return Time;
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
                if (e.Value is byte[])
                {
                    CalendarNameActive = GXDLMSClient.ChangeType((byte[])e.Value, DataType.String).ToString();
                }
                else
                {
                    CalendarNameActive = Convert.ToString(e.Value);
                }
            }
            else if (e.Index == 3)
            {
                SeasonProfileActive = null;
                if (e.Value != null)
                {
                    List<GXDLMSSeasonProfile> items = new List<GXDLMSSeasonProfile>();
                    foreach (object[] item in (object[])e.Value)
                    {
                        GXDLMSSeasonProfile it = new GXDLMSSeasonProfile();
                        it.Name = GXDLMSClient.ChangeType((byte[])item[0], DataType.String).ToString();
                        it.Start = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[1], DataType.DateTime);
                        it.WeekName = GXDLMSClient.ChangeType((byte[])item[2], DataType.String).ToString();
                        items.Add(it);
                    }
                    SeasonProfileActive = items.ToArray();
                }
            }
            else if (e.Index == 4)
            {
                WeekProfileTableActive = null;
                if (e.Value != null)
                {
                    List<GXDLMSWeekProfile> items = new List<GXDLMSWeekProfile>();
                    foreach (object[] item in (object[])e.Value)
                    {
                        GXDLMSWeekProfile it = new GXDLMSWeekProfile();
                        it.Name = GXDLMSClient.ChangeType((byte[])item[0], DataType.String).ToString();
                        it.Monday = Convert.ToInt32(item[1]);
                        it.Tuesday = Convert.ToInt32(item[2]);
                        it.Wednesday = Convert.ToInt32(item[3]);
                        it.Thursday = Convert.ToInt32(item[4]);
                        it.Friday = Convert.ToInt32(item[5]);
                        it.Saturday = Convert.ToInt32(item[6]);
                        it.Sunday = Convert.ToInt32(item[7]);
                        items.Add(it);
                    }
                    WeekProfileTableActive = items.ToArray();
                }
            }
            else if (e.Index == 5)
            {
                DayProfileTableActive = null;
                if (e.Value != null)
                {
                    List<GXDLMSDayProfile> items = new List<GXDLMSDayProfile>();
                    foreach (object[] item in (object[])e.Value)
                    {
                        GXDLMSDayProfile it = new GXDLMSDayProfile();
                        it.DayId = Convert.ToInt32(item[0]);
                        List<GXDLMSDayProfileAction> actions = new List<GXDLMSDayProfileAction>();
                        foreach (object[] it2 in (object[])item[1])
                        {
                            GXDLMSDayProfileAction ac = new GXDLMSDayProfileAction();
                            if (it2[0] is GXDateTime)
                            {
                                ac.StartTime = (GXDateTime)it2[0];
                            }
                            else
                            {
                                ac.StartTime = (GXDateTime)GXDLMSClient.ChangeType((byte[])it2[0], DataType.Time);
                            }
                            ac.ScriptLogicalName = GXDLMSClient.ChangeType((byte[])it2[1], DataType.String).ToString();
                            ac.ScriptSelector = Convert.ToUInt16(it2[2]);
                            actions.Add(ac);
                        }
                        it.DaySchedules = actions.ToArray();
                        items.Add(it);
                    }
                    DayProfileTableActive = items.ToArray();
                }
            }
            else if (e.Index == 6)
            {
                if (e.Value is byte[])
                {
                    CalendarNamePassive = GXDLMSClient.ChangeType((byte[])e.Value, DataType.String).ToString();
                }
                else
                {
                    CalendarNamePassive = Convert.ToString(e.Value);
                }
            }
            else if (e.Index == 7)
            {
                SeasonProfilePassive = null;
                if (e.Value != null)
                {
                    List<GXDLMSSeasonProfile> items = new List<GXDLMSSeasonProfile>();
                    foreach (object[] item in (object[])e.Value)
                    {
                        GXDLMSSeasonProfile it = new GXDLMSSeasonProfile();
                        it.Name = GXDLMSClient.ChangeType((byte[])item[0], DataType.String).ToString();
                        it.Start = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[1], DataType.DateTime);
                        byte[] weekName = (byte[])item[2];
                        //If week name is ignored.
                        if (weekName != null && weekName.Length == 1 && weekName[0] == 0xFF)
                        {
                            it.WeekName = "";
                        }
                        else
                        {
                            it.WeekName = GXDLMSClient.ChangeType((byte[])item[2], DataType.String).ToString();
                        }
                        items.Add(it);
                    }
                    SeasonProfilePassive = items.ToArray();
                }
            }
            else if (e.Index == 8)
            {
                WeekProfileTablePassive = null;
                if (e.Value != null)
                {
                    List<GXDLMSWeekProfile> items = new List<GXDLMSWeekProfile>();
                    foreach (object[] item in (object[])e.Value)
                    {
                        GXDLMSWeekProfile it = new GXDLMSWeekProfile();
                        it.Name = GXDLMSClient.ChangeType((byte[])item[0], DataType.String).ToString();
                        it.Monday = Convert.ToInt32(item[1]);
                        it.Tuesday = Convert.ToInt32(item[2]);
                        it.Wednesday = Convert.ToInt32(item[3]);
                        it.Thursday = Convert.ToInt32(item[4]);
                        it.Friday = Convert.ToInt32(item[5]);
                        it.Saturday = Convert.ToInt32(item[6]);
                        it.Sunday = Convert.ToInt32(item[7]);
                        items.Add(it);
                    }
                    WeekProfileTablePassive = items.ToArray();
                }
            }
            else if (e.Index == 9)
            {
                DayProfileTablePassive = null;
                if (e.Value != null)
                {
                    List<GXDLMSDayProfile> items = new List<GXDLMSDayProfile>();
                    foreach (object[] item in (object[])e.Value)
                    {
                        GXDLMSDayProfile it = new GXDLMSDayProfile();
                        it.DayId = Convert.ToInt32(item[0]);
                        List<GXDLMSDayProfileAction> actions = new List<GXDLMSDayProfileAction>();
                        foreach (object[] it2 in (object[])item[1])
                        {
                            GXDLMSDayProfileAction ac = new GXDLMSDayProfileAction();
                            if (it2[0] is GXDateTime)
                            {
                                ac.StartTime = (GXDateTime)it2[0];
                            }
                            else
                            {
                                ac.StartTime = (GXDateTime)GXDLMSClient.ChangeType((byte[])it2[0], DataType.Time);
                            }
                            ac.ScriptLogicalName = GXDLMSClient.ChangeType((byte[])it2[1], DataType.OctetString).ToString();
                            ac.ScriptSelector = Convert.ToUInt16(it2[2]);
                            actions.Add(ac);
                        }
                        it.DaySchedules = actions.ToArray();
                        items.Add(it);
                    }
                    DayProfileTablePassive = items.ToArray();
                }
            }
            else if (e.Index == 10)
            {
                if (e.Value is byte[])
                {
                    Time = (GXDateTime)GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime);
                }
                else
                {
                    Time = new GXDateTime(Convert.ToDateTime(e.Value));
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