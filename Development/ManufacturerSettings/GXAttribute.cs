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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Gurux.DLMS.ManufacturerSettings
{
    [Serializable]
    public class GXDLMSAttribute : GXDLMSAttributeSettings
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttribute(int index)
            : this(index, DataType.None, 0)
        {
            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttribute() :
            this(0, DataType.None, DataType.None, 0)
        {

        }        

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttribute(int index, DataType uiType)  :
            this(index, DataType.None, uiType, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttribute(int index, DataType type, DataType uiType) :
            this(index, type, uiType, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttribute(int index, DataType type, DataType uiType, int order) :
            base()
        {
            Index = index;
            Type = type;
            UIType = uiType;
            Order = order;
        }        
    }

    [Serializable]
    public class GXDLMSAttributeSettings : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttributeSettings(int index)
            : this()
        {
            Index = index;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttributeSettings()
        {
            Access = AccessMode.ReadWrite;
            Values = new GXObisValueItemCollection();
            UIType = DataType.None;
        }

        public void CopyTo(GXDLMSAttributeSettings target)
        {
            target.Name = this.Name;
            target.Index = Index;
            target.Type = Type;
            target.UIType = UIType;
            target.Access = Access;
            target.Static = Static;
            target.Values = Values;
            target.Order = Order;
            target.MinimumVersion = MinimumVersion;
        }
        
        /// <summary>
        /// Hide TypeId.
        /// </summary>
        [Browsable(false)]
        public override object TypeId
        {
            get
            {
                return base.TypeId;
            }
        }

        public override string ToString()
        {            
            return Name + Index.ToString();
        }

        /// <summary>
        /// Attribute name.
        /// </summary>
        [DefaultValue(null)]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute Index.
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        [Browsable(false)]
        [XmlIgnore]
        public GXAttributeCollection Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute data type.
        /// </summary>
        [DefaultValue(Gurux.DLMS.DataType.None)]
        public Gurux.DLMS.DataType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Data type that user ẃant's to see.
        /// </summary>
        [DefaultValue(Gurux.DLMS.DataType.None)]
        public Gurux.DLMS.DataType UIType
        {
            get;
            set;
        }

        [DefaultValue(AccessMode.ReadWrite)]
        public AccessMode Access
        {
            get;
            set;
        }

        [Browsable(false)]
        [DefaultValue(MethodAccessMode.NoAccess)]
        public MethodAccessMode MethodAccess
        {
            get;
            set;
        }

        [DefaultValue(false)]
        [Browsable(false)]
        public bool Static
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute values.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public GXObisValueItemCollection Values
        {
            get;
            set;
        }

        /// <summary>
        /// Read order.
        /// </summary>
        [XmlIgnore()]
        public int Order
        {
            get;
            set;
        }

        /// <summary>
        /// Minimum version vhere this attribute is implemented.
        /// </summary>
        [DefaultValue(0)]
        [Browsable(false)]
        public int MinimumVersion
        {
            get;
            set;
        }
    }
}
