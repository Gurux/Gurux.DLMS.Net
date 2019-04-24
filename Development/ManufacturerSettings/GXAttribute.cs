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
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.ManufacturerSettings
{
#if !WINDOWS_UWP
    [Serializable]
#endif
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
        public GXDLMSAttribute(int index, DataType uiType) :
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

#if !WINDOWS_UWP
    [Serializable]
#endif
    public class GXDLMSAttributeSettings : Attribute
    {
        /// <summary>
        /// Attribute data type.
        /// </summary>
        private DataType type;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAttributeSettings(int index)
            : this()
        {
            Index = index;
            if (index == 1)
            {
                Type = DataType.OctetString;
            }
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
            target.Xml = Xml;
        }

        /// <summary>
        /// Hide TypeId.
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
        public override object TypeId
        {
            get
            {
                return base.TypeId;
            }
        }
#endif

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "Attribute #" + Index.ToString();
            }
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

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [XmlIgnore]
        public GXAttributeCollection Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute data type.
        /// </summary>
        [DefaultValue(DataType.None)]
        public DataType Type
        {
            get
            {
                if (Index == 1 && type == DataType.None)
                {
                    return DataType.OctetString;
                }
                return type;
            }
            set
            {
                type = value;
            }
        }

        /// <summary>
        /// Data type that user ẃant's to see.
        /// </summary>
        [DefaultValue(DataType.None)]
        public DataType UIType
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

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [DefaultValue(MethodAccessMode.NoAccess)]
        public MethodAccessMode MethodAccess
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Static
        {
            get;
            set;
        }

        /// <summary>
        /// Force that data is always sent as blocks.
        /// </summary>
        /// <remarks>
        /// Some meters can handle only blocks. This property is used to force send all data in blocks.
        /// </remarks>
        [DefaultValue(false)]
        public bool ForceToBlocks
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute values.
        /// </summary>
        public GXObisValueItemCollection Values
        {
            get;
            set;
        }

        /// <summary>
        /// XML Data template.
        /// </summary>
        /// <remarks>
        /// Xml template can be used to visualize complex data.
        /// </remarks>
        [DefaultValue(null)]
#if !__MOBILE__ && !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1
        [Editor(typeof(UIXmlTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
        public string Xml
        {
            get;
            set;
        }

        /// <summary>
        /// How this value is visualized on the UI.
        /// </summary>
        public ValueFieldType UIValueType
        {
            get;
            set;
        }


        /// <summary>
        /// Read order.
        /// </summary>
        [XmlIgnore()]
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public int Order
        {
            get;
            set;
        }

        /// <summary>
        /// Minimum version vhere this attribute is implemented.
        /// </summary>
        [DefaultValue(0)]
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public int MinimumVersion
        {
            get;
            set;
        }
    }
}
