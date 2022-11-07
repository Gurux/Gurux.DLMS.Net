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
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.ManufacturerSettings
{
#if !WINDOWS_UWP && !__MOBILE__
    [Serializable]
    [TypeConverter(typeof(GXObisCodeConverter)), RefreshProperties(RefreshProperties.All)]
#endif
    public class GXObisCode
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXObisCode()
        {
            Attributes = new GXAttributeCollection();
            Attributes.Parent = this;
        }

        public override string ToString()
        {
            return ObjectType.ToString();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXObisCode(string ln, ObjectType objectType, int index)
            : this(ln, objectType, null)
        {
            AttributeIndex = index;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXObisCode(string ln, ObjectType objectType, string description) : this()
        {
            LogicalName = ln;
            ObjectType = objectType;
            Description = description;
        }

        /// <summary>
        /// Attribute index.
        /// </summary>
        [XmlIgnore()]
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public int AttributeIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Logical name of the OBIS item.
        /// </summary>
        public string LogicalName
        {
            get;
            set;
        }

        /// <summary>
        /// Description of the OBIS item.
        /// </summary>
        [DefaultValue("")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// All meters are not supporting Association view.
        /// If OBIS code is wanted to added by default set append to true.
        /// </summary>
        [DefaultValue(false)]
        public bool Append
        {
            get;
            set;
        }

        /// <summary>
        /// UI data type. This is obsolete. Use ObjectType instead.
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [DefaultValue(null)]
        public string UIDataType
        {
            get;
            set;
        }
     
        /// <summary>
        /// Interface type. This is obsolete. Use ObjectType instead.
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [DefaultValue(null)]
        public string Interface
        {
            get
            {
                //TODO: return null after java supports this.
                return this.ObjectType.ToString();
            }
            set
            {
                ObjectType = (ObjectType)Enum.Parse(typeof(ObjectType), value);
            }
        }

        /// <summary>
        /// Object type.
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [DefaultValue(ObjectType.None)]
        public ObjectType ObjectType
        {
            get;
            set;
        }

        /// <summary>
        /// Object version.
        /// </summary>
        [DefaultValue(0)]
#if !__MOBILE__ && !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_1 && !NETCOREAPP3_0 && !NETCOREAPP3_1 && !NET6_0
        [Editor(typeof(GXVersionUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
        public int Version
        {
            get;
            set;
        }

        /// <summary>
        /// object attribute collection.
        /// </summary>
        public GXAttributeCollection Attributes
        {
            get;
            set;
        }
    }
}
