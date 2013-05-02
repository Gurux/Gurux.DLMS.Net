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

namespace Gurux.DLMS.ManufacturerSettings
{   
    [Serializable]
    [TypeConverter(typeof(GXObisCodeConverter)), RefreshProperties(RefreshProperties.All)]
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
        public GXObisCode(string ln, Gurux.DLMS.ObjectType objectType, int index)
            : this(ln, objectType, null)
        {
            AttributeIndex = index;            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXObisCode(string ln, Gurux.DLMS.ObjectType objectType, string description) : this()
        {            
            LogicalName = ln;
            ObjectType = objectType;            
            Description = description;
        }

        /// <summary>
        /// Attribute index.
        /// </summary>
        [XmlIgnore()]
        [Browsable(false)]
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
        /// Interface type. Opsolite. Use ObjectType instead.
        /// </summary>
        [Browsable(false)]
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
        /// object type.
        /// </summary>
        [Browsable(false)]
        public ObjectType ObjectType
        {
            get;
            set;
        }

        /// <summary>
        /// Interface type.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(0)]
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

        /// <summary>
        /// Opsolite. Do not use.
        /// </summary>
        /// <remarks>        
        /// </remarks>
        [Browsable(false), DefaultValue(Gurux.DLMS.DataType.None)]                
        public Gurux.DLMS.DataType Type
        {
            get
            {
                return DataType.None;
            }
            set
            {
                GXDLMSAttributeSettings att = Attributes.Find(2);
                if (att == null)
                {
                    att = new GXDLMSAttribute();
                    att.Index = 2;
                    Attributes.Add(att);
                }
                att.Type = value;
            }
        }

        /// <summary>
        /// Opsolite. Do not use.
        /// </summary>
        /// <remarks>        
        /// </remarks>
        [Browsable(false), DefaultValue(Gurux.DLMS.DataType.None)]
        public Gurux.DLMS.DataType UIType
        {
            get
            {
                return DataType.None;
            }
            set
            {
                GXDLMSAttributeSettings att = Attributes.Find(2);
                if (att == null)
                {
                    att = new GXDLMSAttribute();
                    att.Index = 2;
                    Attributes.Add(att);
                }
                att.UIType = value;
            }
        }

        /// <summary>
        /// Convert DLMS data type to .Net data type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static public Type GetDataType(Gurux.DLMS.DataType type)
        {
            switch (type)
            {
                case Gurux.DLMS.DataType.Array:
                    return typeof(byte[]);
                case Gurux.DLMS.DataType.BinaryCodedDesimal:
                    return typeof(string);
                case Gurux.DLMS.DataType.BitString:
                    return typeof(string);
                case Gurux.DLMS.DataType.Boolean:
                    return typeof(bool);
                case Gurux.DLMS.DataType.Date:
                    return typeof(DateTime);
                case Gurux.DLMS.DataType.DateTime:
                    return typeof(DateTime);
                case Gurux.DLMS.DataType.Float32:
                    return typeof(float);
                case Gurux.DLMS.DataType.Float64:
                    return typeof(double);
                case Gurux.DLMS.DataType.Int16:
                    return typeof(Int16);
                case Gurux.DLMS.DataType.Int32:
                    return typeof(Int32);
                case Gurux.DLMS.DataType.Int64:
                    return typeof(Int64);
                case Gurux.DLMS.DataType.Int8:
                    return typeof(sbyte);
                case Gurux.DLMS.DataType.None:
                    return null;
                case Gurux.DLMS.DataType.OctetString:
                    return typeof(string);
                case Gurux.DLMS.DataType.String:
                    return typeof(string);
                case Gurux.DLMS.DataType.Time:
                    return typeof(DateTime);
                case Gurux.DLMS.DataType.UInt16:
                    return typeof(UInt16);
                case Gurux.DLMS.DataType.UInt32:
                    return typeof(UInt32);
                case Gurux.DLMS.DataType.UInt64:
                    return typeof(UInt64);
                case Gurux.DLMS.DataType.UInt8:
                    return typeof(byte);
                default:
                case Gurux.DLMS.DataType.CompactArray:
                case Gurux.DLMS.DataType.Enum:
                case Gurux.DLMS.DataType.Structure:
                    break;
            }
            throw new Exception("Invalid DLMS data type.");
        }
    }
}
