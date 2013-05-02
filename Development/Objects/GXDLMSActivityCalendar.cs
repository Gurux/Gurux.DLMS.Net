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

namespace Gurux.DLMS.Objects
{    
    public class GXDLMSActivityCalendar : GXDLMSObject
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSActivityCalendar()
            : base(ObjectType.ActivityCalendar)
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
        public object CalendarNameActive
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(3, DataType.Array)]
        public object SeasonProfileActive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        [GXDLMSAttribute(4, DataType.Array)]
        public object WeekProfileTableActive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        [GXDLMSAttribute(5, DataType.Array)]
        public object DayProfileTableActive
        {
            get;
            set;
        }
        [XmlIgnore()]
        [GXDLMSAttribute(6, DataType.String)]
        public String CalendarNamePassive
        {
            get;
            set;
        }
        [XmlIgnore()]
        [GXDLMSAttribute(7, DataType.Array)]
        public object SeasonProfilePassive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        [GXDLMSAttribute(8, DataType.Array)]
        public object WeekProfileTablePassive
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore()]
        [GXDLMSAttribute(9, DataType.Array)]
        public object DayProfileTablePassive
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(10, DataType.DateTime)]
        public string Time
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, CalendarNameActive, SeasonProfileActive, WeekProfileTableActive, DayProfileTableActive, CalendarNamePassive, SeasonProfilePassive, WeekProfileTablePassive, DayProfileTablePassive, Time };
        }
    }
}