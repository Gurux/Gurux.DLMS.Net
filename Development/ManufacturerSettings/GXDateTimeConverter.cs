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

using Gurux.DLMS.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GXDLMS.ManufacturerSettings
{
    public class GXDateTimeConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                if (value is DateTime && destinationType == typeof(string))
                {
                    DateTime dt = (DateTime)value;
                    if (dt == DateTime.MinValue)
                    {
                        return "";
                    }
                    DateTimeSkips skip = (DateTimeSkips)dt.Millisecond;
                    if (skip != DateTimeSkips.None)
                    {
                        List<string> shortDatePattern = new List<string>(culture.DateTimeFormat.ShortDatePattern.Split(new string[] { culture.DateTimeFormat.DateSeparator }, StringSplitOptions.RemoveEmptyEntries));
                        if ((skip & DateTimeSkips.Year) != 0)
                        {
                            shortDatePattern.Remove("yyyy");
                        }
                        if ((skip & DateTimeSkips.Month) != 0)
                        {
                            shortDatePattern.Remove("M");
                        }
                        if ((skip & DateTimeSkips.Day) != 0)
                        {
                            shortDatePattern.Remove("d");
                        }
                        return dt.ToString(string.Join(culture.DateTimeFormat.DateSeparator, shortDatePattern.ToArray()));
                    }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
            catch
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
