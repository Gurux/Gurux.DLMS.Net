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
using System.Reflection;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSIECOpticalPortSetup : GXDLMSObject
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSIECOpticalPortSetup()
            : base(ObjectType.IecLocalPortSetup)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSIECOpticalPortSetup(string ln)
            : base(ObjectType.IecLocalPortSetup, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIECOpticalPortSetup(string ln, ushort sn)
            : base(ObjectType.IecLocalPortSetup, ln, 0)
        {
        }

        [XmlIgnore()]
        [GXDLMSAttribute(2)]
        public int DefaultMode
        {
            get;
            set;
        }

        [XmlIgnore()]        
        [GXDLMSAttribute(3)]
        public int DefaultBaudrate
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(4)]
        public int MaximumBaudrate
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(5)]
        public int ResponseTime
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(6, DataType.String)]
        public string DeviceAddress
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(7, DataType.String)]
        public string Password1
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(8, DataType.String)]
        public string Password2
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(9, DataType.String)]
        public string Password5
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.UpdateDefaultValueItems"/>
        public override void UpdateDefaultValueItems()
        {
            GXDLMSAttributeSettings att = this.Attributes.Find(2);
            if (att == null)
            {
                att = new GXDLMSAttribute(2);
                Attributes.Add(att);
            }
            att.Values.Add(new GXObisValueItem(0, "IEC"));
            att.Values.Add(new GXObisValueItem(1, "DLMS"));

            att = this.Attributes.Find(3);
            if (att == null)
            {
                att = new GXDLMSAttribute(3);
                Attributes.Add(att);
            } 
            att.Values.Add(new GXObisValueItem(0, "300"));
            att.Values.Add(new GXObisValueItem(1, "600"));
            att.Values.Add(new GXObisValueItem(2, "1200"));
            att.Values.Add(new GXObisValueItem(3, "2400"));
            att.Values.Add(new GXObisValueItem(4, "4800"));
            att.Values.Add(new GXObisValueItem(5, "9600"));
            att.Values.Add(new GXObisValueItem(6, "19200"));
            att.Values.Add(new GXObisValueItem(7, "38400"));
            att.Values.Add(new GXObisValueItem(8, "57600"));
            att.Values.Add(new GXObisValueItem(9, "115200"));
            att = this.Attributes.Find(4);
            if (att == null)
            {
                att = new GXDLMSAttribute(4);
                Attributes.Add(att);
            }             
            att.Values.Add(new GXObisValueItem(0, "300"));
            att.Values.Add(new GXObisValueItem(1, "600"));
            att.Values.Add(new GXObisValueItem(2, "1200"));
            att.Values.Add(new GXObisValueItem(3, "2400"));
            att.Values.Add(new GXObisValueItem(4, "4800"));
            att.Values.Add(new GXObisValueItem(5, "9600"));
            att.Values.Add(new GXObisValueItem(6, "19200"));
            att.Values.Add(new GXObisValueItem(7, "38400"));
            att.Values.Add(new GXObisValueItem(8, "57600"));
            att.Values.Add(new GXObisValueItem(9, "115200"));
            att = this.Attributes.Find(5);
            if (att == null)
            {
                att = new GXDLMSAttribute(5);
                Attributes.Add(att);
            }
            att.Values.Add(new GXObisValueItem(0, "20"));
            att.Values.Add(new GXObisValueItem(1, "200"));
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, DefaultMode, DefaultBaudrate, MaximumBaudrate, ResponseTime, DeviceAddress, Password1, Password2, Password5 };
        }
    }
}
