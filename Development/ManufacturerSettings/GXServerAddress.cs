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
using System.Xml.Serialization;
using System.ComponentModel;

namespace Gurux.DLMS.ManufacturerSettings
{
    public enum HDLCAddressType : int
    {
        [XmlEnum("0")]
        Default,
        [XmlEnum("1")]
        SerialNumber,
        [XmlEnum("2")]
        Custom
    }

    [Serializable]
    public class GXServerAddress
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXServerAddress()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXServerAddress(HDLCAddressType address, object value, bool enabled)
        {
            HDLCAddress = address;
            PhysicalAddress = value;            
        }

        /// <summary>
        /// Is server address enabled
        /// </summary>
        [DefaultValue(false)]
        public bool Selected
        {
            get;
            set;
        }        

        public HDLCAddressType HDLCAddress
        {
            get;
            set;
        }      

        public string Formula
        {
            get;
            set;
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public object PhysicalAddress
        {
            get;
            set;
        }
        
        [DefaultValue(0)]
        public int LogicalAddress
        {
            get;
            set;
        }       
    }
}
