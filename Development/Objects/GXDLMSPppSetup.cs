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

namespace Gurux.DLMS.Objects
{
    public class GXDLMSPppSetup : GXDLMSObject
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSPppSetup()
            : base(ObjectType.PppSetup)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSPppSetup(string ln)
            : base(ObjectType.PppSetup, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSPppSetup(string ln, ushort sn)
            : base(ObjectType.PppSetup, ln, 0)
        {
        }

        [XmlIgnore()]
        [GXDLMSAttribute(2, DataType.OctetString)]
        public string PHYReference
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(3)]
        public object LCPOptions
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(4)]
        public object IPCPOptions
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(5)]
        public object PPPAuthentication
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, PHYReference, LCPOptions, IPCPOptions, PPPAuthentication };
        }
    }
}
