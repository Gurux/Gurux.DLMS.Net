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
using Gurux.DLMS;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;

namespace Gurux.DLMS.Objects
{    
    public class GXDLMSExtendedRegister : GXDLMSRegister
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSExtendedRegister()
            : base(ObjectType.ExtendedRegister, null, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSExtendedRegister(string ln)
            : base(ObjectType.ExtendedRegister, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSExtendedRegister(string ln, ushort sn)
            : base(ObjectType.ExtendedRegister, ln, 0)
        {
        }

        /// <inheritdoc cref="GXDLMSObject.UpdateDefaultValueItems"/>
        public override void UpdateDefaultValueItems()
        {
            GXDLMSAttributeSettings att = this.Attributes.Find(4);
            if (att == null)
            {
                att = new GXDLMSAttribute(4, DataType.Int32, DataType.Int32);
                att.Access = AccessMode.Read;
                Attributes.Add(att);
            }
            att.Values.Add(new GXObisValueItem(0, "Null"));
            att.Values.Add(new GXObisValueItem(4, "Bit String"));
            att.Values.Add(new GXObisValueItem(6, "Double Long Unsigned"));
            att.Values.Add(new GXObisValueItem(9, "Octet String"));
            att.Values.Add(new GXObisValueItem(10, "Visible String"));
            att.Values.Add(new GXObisValueItem(12, "UTF8 String"));
            att.Values.Add(new GXObisValueItem(17, "Unsigned"));
            att.Values.Add(new GXObisValueItem(18, "Long Unsigned"));
            att = this.Attributes.Find(9);
            if (att == null)
            {
                att = new GXDLMSAttribute(9);
                att.Access = AccessMode.Read;
                Attributes.Add(att);
            }            
            att.Values.Add(new GXObisValueItem(0, "Not defined"));
            att.Values.Add(new GXObisValueItem(1, "Internal Crystal"));
            att.Values.Add(new GXObisValueItem(2, "Mains frequency 50 Hz"));
            att.Values.Add(new GXObisValueItem(3, "Mains Frequency 60 Hz"));
            att.Values.Add(new GXObisValueItem(4, "GPS (Global Positioning System)"));
            att.Values.Add(new GXObisValueItem(5, "Radio Controlled"));
        }

        /// <summary>
        /// Scaler of COSEM Register object.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(4, Access = AccessMode.Read)]
        public int Status
        {
            get;
            set;
        }

        /// <summary>
        /// Scaler of COSEM Register object.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(5, DataType.DateTime, Access = AccessMode.Read)]
        public DateTime CaptureTime
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, Value, ScalerUnit, Status, CaptureTime };
        }
    }
}
