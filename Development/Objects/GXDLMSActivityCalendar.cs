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
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSActivityCalendar(string ln)
            : base(ObjectType.ActivityCalendar, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSActivityCalendar(string ln, ushort sn)
            : base(ObjectType.ActivityCalendar, ln, 0)
        {
        }

        [XmlIgnore()]
        [GXDLMSAttribute(2, DataType.String)]
        public string CalendarNameActive
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(3, DataType.Array)]
        public GXDLMSSeasonProfile[] SeasonProfileActive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        [GXDLMSAttribute(4, DataType.Array)]
        public GXDLMSWeekProfile[] WeekProfileTableActive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        [GXDLMSAttribute(5, DataType.Array)]
        public GXDLMSDayProfile[] DayProfileTableActive
        {
            get;
            set;
        }
        [XmlIgnore()]
        [GXDLMSAttribute(6, DataType.String)]
        public string CalendarNamePassive
        {
            get;
            set;
        }
        [XmlIgnore()]
        [GXDLMSAttribute(7, DataType.Array)]
        public GXDLMSSeasonProfile[] SeasonProfilePassive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        [GXDLMSAttribute(8, DataType.Array)]
        public GXDLMSWeekProfile[] WeekProfileTablePassive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        [GXDLMSAttribute(9, DataType.Array)]
        public GXDLMSDayProfile[] DayProfileTablePassive
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(10, DataType.DateTime)]
        public GXDateTime Time
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, CalendarNameActive, SeasonProfileActive, WeekProfileTableActive, DayProfileTableActive, CalendarNamePassive, SeasonProfilePassive, WeekProfileTablePassive, DayProfileTablePassive, Time };
        }

        #region IGXDLMSBase Members

        int IGXDLMSBase.GetAttributeCount()
        {
            return 10;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
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
                type = DataType.OctetString;
                return GXDLMSClient.ChangeType(ASCIIEncoding.ASCII.GetBytes(CalendarNameActive), DataType.OctetString);
            }
            if (index == 3)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
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
                        data.Add((byte)DataType.Structure);
                        data.Add(3);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Name));
                        GXCommon.SetData(data, DataType.OctetString, it.Start);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.WeekName));
                    }
                }
                return data.ToArray();
            }
            if (index == 4)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
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
                        data.Add((byte)DataType.Array);
                        data.Add(8);
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
                return data.ToArray();
            }
            if (index == 5)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
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
                        data.Add((byte)DataType.Structure);
                        data.Add(2);
                        GXCommon.SetData(data, DataType.UInt8, it.DayId);
                        data.Add((byte)DataType.Array);
                        //Add count            
                        GXCommon.SetObjectCount(it.DaySchedules.Length, data);                        
                        foreach (GXDLMSDayProfileAction action in it.DaySchedules)
                        {
                            data.Add((byte)DataType.Structure);
                            data.Add(3);
                            GXCommon.SetData(data, DataType.Time, action.StartTime);
                            GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(action.ScriptLogicalName));
                            GXCommon.SetData(data, DataType.UInt16, action.ScriptSelector);
                        }
                    }
                }
                return data.ToArray();
            }
            if (index == 6)
            {
                type = DataType.OctetString;
                return GXDLMSClient.ChangeType(ASCIIEncoding.ASCII.GetBytes(CalendarNamePassive), DataType.OctetString);
            }
            //
            if (index == 7)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
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
                        data.Add((byte)DataType.Structure);
                        data.Add(3);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Name));
                        GXCommon.SetData(data, DataType.OctetString, it.Start);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.WeekName));
                    }
                }
                return data.ToArray();
            }
            if (index == 8)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
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
                        data.Add((byte)DataType.Array);
                        data.Add(8);
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
                return data.ToArray();
            }
            if (index == 9)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
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
                        data.Add((byte)DataType.Structure);
                        data.Add(2);
                        GXCommon.SetData(data, DataType.UInt8, it.DayId);
                        data.Add((byte)DataType.Array);
                        //Add count            
                        GXCommon.SetObjectCount(it.DaySchedules.Length, data);
                        foreach (GXDLMSDayProfileAction action in it.DaySchedules)
                        {
                            data.Add((byte)DataType.Structure);
                            data.Add(3);
                            GXCommon.SetData(data, DataType.Time, action.StartTime);
                            GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(action.ScriptLogicalName));
                            GXCommon.SetData(data, DataType.UInt16, action.ScriptSelector);
                        }
                    }
                }
                return data.ToArray();
            }
            if (index == 10)
            {
                type = DataType.DateTime;
                return Time;
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
                CalendarNameActive = GXDLMSClient.ChangeType((byte[])value, DataType.String).ToString();
            }
            else if (index == 3)
            {
                SeasonProfileActive = null;
                if (value != null)
                {
                    List<GXDLMSSeasonProfile> items = new List<GXDLMSSeasonProfile>();
                    foreach (object[] item in (object[])value)
                    {
                        GXDLMSSeasonProfile it = new GXDLMSSeasonProfile();
                        it.Name = GXDLMSClient.ChangeType((byte[]) item[0], DataType.String).ToString();
                        it.Start = (GXDateTime) GXDLMSClient.ChangeType((byte[])item[1], DataType.DateTime);
                        it.WeekName = GXDLMSClient.ChangeType((byte[]) item[2], DataType.String).ToString();
                        items.Add(it);
                    }
                    SeasonProfileActive = items.ToArray();
                }
            }
            else if (index == 4)
            {
                WeekProfileTableActive = null;
                if (value != null)
                {
                    List<GXDLMSWeekProfile> items = new List<GXDLMSWeekProfile>();
                    foreach (object[] item in (object[])value)
                    {
                        GXDLMSWeekProfile it = new GXDLMSWeekProfile();
                        it.Name = GXDLMSClient.ChangeType((byte[])item[0], DataType.String).ToString();
                        it.Monday = Convert.ToInt32(item[1]);
                        it.Tuesday = Convert.ToInt32(item[2]);
                        it.Wednesday= Convert.ToInt32(item[3]);
                        it.Thursday = Convert.ToInt32(item[4]);
                        it.Friday = Convert.ToInt32(item[5]);
                        it.Saturday = Convert.ToInt32(item[6]);
                        it.Sunday = Convert.ToInt32(item[7]);
                        items.Add(it);
                    }
                    WeekProfileTableActive = items.ToArray();
                }
            }
            else if (index == 5)
            {
                DayProfileTableActive = null;
                if (value != null)
                {
                    List<GXDLMSDayProfile> items = new List<GXDLMSDayProfile>();
                    foreach (object[] item in (object[])value)
                    {
                        GXDLMSDayProfile it = new GXDLMSDayProfile();
                        it.DayId = Convert.ToInt32(item[0]);
                        List<GXDLMSDayProfileAction> actions = new List<GXDLMSDayProfileAction>();
                        foreach (object[] it2 in (object[])item[1])
                        {
                            GXDLMSDayProfileAction ac = new GXDLMSDayProfileAction();
                            ac.StartTime = (GXDateTime)GXDLMSClient.ChangeType((byte[])it2[0], DataType.Time);
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
            else if (index == 6)
            {
                CalendarNamePassive = GXDLMSClient.ChangeType((byte[])value, DataType.String).ToString();
            }
            else if (index == 7)
            {
                SeasonProfilePassive = null;
                if (value != null)
                {
                    List<GXDLMSSeasonProfile> items = new List<GXDLMSSeasonProfile>();
                    foreach (object[] item in (object[])value)
                    {
                        GXDLMSSeasonProfile it = new GXDLMSSeasonProfile();
                        it.Name = GXDLMSClient.ChangeType((byte[])item[0], DataType.String).ToString();
                        it.Start = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[1], DataType.DateTime);
                        it.WeekName = GXDLMSClient.ChangeType((byte[])item[2], DataType.String).ToString();
                        items.Add(it);
                    }
                    SeasonProfilePassive = items.ToArray();
                }
            }
            else if (index == 8)
            {
                WeekProfileTablePassive = null;
                if (value != null)
                {
                    List<GXDLMSWeekProfile> items = new List<GXDLMSWeekProfile>();
                    foreach (object[] item in (object[])value)
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
            else if (index == 9)
            {
                DayProfileTablePassive = null;
                if (value != null)
                {
                    List<GXDLMSDayProfile> items = new List<GXDLMSDayProfile>();
                    foreach (object[] item in (object[])value)
                    {
                        GXDLMSDayProfile it = new GXDLMSDayProfile();
                        it.DayId = Convert.ToInt32(item[0]);
                        List<GXDLMSDayProfileAction> actions = new List<GXDLMSDayProfileAction>();
                        foreach (object[] it2 in (object[])item[1])
                        {
                            GXDLMSDayProfileAction ac = new GXDLMSDayProfileAction();
                            ac.StartTime = (GXDateTime)GXDLMSClient.ChangeType((byte[])it2[0], DataType.Time);
                            ac.ScriptLogicalName = GXDLMSClient.ChangeType((byte[])it2[1], DataType.String).ToString();
                            ac.ScriptSelector = Convert.ToUInt16(it2[2]);
                            actions.Add(ac);
                        }
                        it.DaySchedules = actions.ToArray();
                        items.Add(it);
                    }
                    DayProfileTablePassive = items.ToArray();
                }
            }
            else if (index == 10)
            {
                Time = (GXDateTime)GXDLMSClient.ChangeType((byte[])value, DataType.DateTime);
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