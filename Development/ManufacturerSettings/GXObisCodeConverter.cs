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
using System.ComponentModel;

namespace Gurux.DLMS.ManufacturerSettings
{
    public class GXObisCodeConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);
            PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
            Gurux.DLMS.ObjectType type = ((GXObisCode)value).ObjectType;
            bool bBrowsable = type == Gurux.DLMS.ObjectType.Data || type == Gurux.DLMS.ObjectType.Register || type == Gurux.DLMS.ObjectType.ExtendedRegister || type == Gurux.DLMS.ObjectType.DemandRegister;
            foreach (PropertyDescriptor it in pdc)
            {
                if ((it.Name == "UIType" || it.Name == "Type") && !bBrowsable)
                {
                    continue;
                }
                properties.Add(it);
            }
            return properties;
        }
    }
}
