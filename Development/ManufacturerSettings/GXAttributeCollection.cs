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
    internal class GXCollectionPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// Internal list of properties.
        /// </summary>
        protected System.Collections.IList _list = null;
        /// <summary>
        /// Index of the current instance.
        /// </summary>
        protected int Index = -1;

        /// <summary>
        /// Initializes a new instance of the GXCollectionPropertyDescriptor class.
        /// </summary>
        /// <param name="idx">Index of the new instance.</param>
        /// <param name="list">Collection list.</param>
        public GXCollectionPropertyDescriptor(int idx, System.Collections.IList list) :
            base("#" + idx.ToString(), null)
        {
            _list = list;
            this.Index = idx;
        }

        /// <summary>
        /// Can always reset the value.
        /// </summary>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// Returns the type of the component.
        /// </summary>
        public override Type ComponentType
        {
            get
            {
                return _list.GetType();
            }
        }

        /// <summary>
        /// Returns value of property of the component in at position Index.
        /// </summary>
        public override object GetValue(object component)
        {
            if (_list.Count <= Index)
            {
                return string.Empty;
            }
            return _list[Index];
        }

        /// <summary>
        /// Never true.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Type of property of the component at position Index.
        /// </summary>
        public override Type PropertyType
        {
            get
            {
                return _list[Index].GetType();
            }
        }

        /// <summary>
        /// Always true.
        /// </summary>
        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        /// <summary>
        /// Not used, just for compiler
        /// </summary>
        public override void ResetValue(object component)
        {
            //Not used, just for compiler
        }

        /// <summary>
        /// Not used, just for compiler
        /// </summary>
        public override void SetValue(object component, object value)
        {
            //Not used, just for compiler
        }
    }

    /// <summary>
    /// A helper class for showing a media type in the property grid.
    /// </summary>
    /// <remarks>
    /// The GXAttributeCollectionPropertyDescriptor is used to set default settings for the media type.
    /// <br />
    /// Example: The port number in TCP/IP protocol.
    /// </remarks>
    internal class GXAttributeCollectionPropertyDescriptor : GXCollectionPropertyDescriptor
    {
        public GXAttributeCollectionPropertyDescriptor(GXAttributeCollection coll, int idx)
            : base(idx, coll)
        {
        }

        GXAttributeCollection List
        {
            get
            {
                return (GXAttributeCollection)_list;
            }
        }

        public override string DisplayName
        {
            get
            {
                if (Index >= List.Count)
                {
                    return string.Empty;
                }
                return List[Index].ToString();
            }
        }

        public override string Description
        {
            get
            {
                if (Index >= List.Count)
                {
                    return string.Empty;
                }
                return List[Index].Name + "Media properties";
            }
        }

        /// <summary>
        /// Returns value of property of the component in at position Index.
        /// </summary>
        public override object GetValue(object component)
        {
            if (_list.Count <= Index)
            {
                return string.Empty;
            }
            GXDLMSAttributeSettings a = List[Index];
            return a.Access.ToString() + "[" + a.Type + " / " + a.UIType + "]";
        }

        public override string Name
        {
            get
            {
                if (Index >= List.Count)
                {
                    return string.Empty;
                }
                return List[Index].Name.ToString();
            }
        }

        public override void ResetValue(object component)
        {
            
        }
    }

    /// <summary>
    /// This is a special type converter which will be associated with the GXAttributeCollection class.
    /// It converts an GXAttribute object to a string representation for use in a property grid.
    /// </summary>
    internal class AllowedGXAttributesConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Allows displaying the + symbol in the property grid.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
        /// <returns>True to find properties of this object.</returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        
        /// <summary>
        /// Converts given value object to the specified destination type.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">Culture info.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="destType">Destination type.</param>
        /// <returns>Converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is GXAttributeCollection)
            {
                string data = "";
                if (((GXAttributeCollection)value).Count == 0)
                {
                    return data;
                }
                foreach (GXDLMSAttributeSettings type in ((GXAttributeCollection)value))
                {
                    data += type.Name + ", ";
                }
                if (data.Length > 1)
                {
                    data = data.Remove(data.Length - 2, 2);
                }
                return data;
            }
            return base.ConvertTo(context, culture, value, destType);
        }
        
        /// <summary>
        /// Loops through all device types and adds them to the property list.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
        /// <param name="value">An Object that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type Attribute that is used as a filter.</param>
        /// <returns>Collection of properties exposed to this data type.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            GXAttributeCollection list = (GXAttributeCollection)value;
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);
            for (int pos = 0; pos < list.Count; ++pos)
            {
                PropertyDescriptor pd = new GXAttributeCollectionPropertyDescriptor(list, pos);
                pds.Add(pd);
            }
            return pds;
        }
    }

    [Serializable]
    [TypeConverter(typeof(AllowedGXAttributesConverter))]
    public class GXAttributeCollection : List<GXDLMSAttributeSettings>, IList<GXDLMSAttributeSettings>
    {
        /// <summary>
        /// Parent object.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public object Parent
        {
            get;
            set;
        }

        public GXDLMSAttributeSettings Find(int index)
        {
            if (index < 1)
            {
                throw new ArgumentOutOfRangeException("Invalid attribute Index.");
            }
            foreach (GXDLMSAttributeSettings it in this)
            {
                if (it.Index == index)
                {
                    return it;
                }
            }
            return null;
        }

        #region ICollection<GXDLMSObject> Members

        /// <summary>
        /// Add new GXDLMSAttributeSettings item to the collection.
        /// </summary>
        /// <param name="item"></param>
        public new void Add(GXDLMSAttributeSettings item)
        {
            item.Parent = this;
            base.Add(item);
        }
        #endregion
    }
}
