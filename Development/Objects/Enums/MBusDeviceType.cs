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

using System.Xml.Serialization;

namespace Gurux.DLMS.Objects.Enums
{
    /// <summary>
    /// M-Bus device type enumerations.
    /// </summary>
    public enum MBusDeviceType : byte
    {
        /// <summary>
        /// Other.
        /// </summary>
        [XmlEnum("0")]
        Other,
        /// <summary>
        /// Oil meter.
        /// </summary>
        [XmlEnum("1")]
        Oil,
        /// <summary>
        /// Electricity meter.
        /// </summary>
        [XmlEnum("2")]
        Electricity,
        /// <summary>
        /// Gas meter.
        /// </summary>
        [XmlEnum("3")]
        Gas,
        /// <summary>
        /// Heat meter.
        /// </summary>
        [XmlEnum("4")]
        Heat,
        /// <summary>
        /// Steam meter.
        /// </summary>
        [XmlEnum("5")]
        Steam,
        /// <summary>
        /// Hot water meter.
        /// </summary>
        [XmlEnum("6")]
        HotWater,
        /// <summary>
        /// Water meter.
        /// </summary>
        [XmlEnum("7")]
        Water,
        /// <summary>
        /// Heat cost allocator meter.
        /// </summary>
        [XmlEnum("8")]
        HeatCostAllocator,
        /// <summary>
        /// Reserved.
        /// </summary>
        [XmlEnum("9")]
        Reserved,
        /// <summary>
        /// Gas mode 2 meter.
        /// </summary>
        [XmlEnum("10")]
        GasMode2,
        /// <summary>
        /// Heat mode 2 meter.
        /// </summary>
        [XmlEnum("11")]
        HeatMode2,
        /// <summary>
        /// Hot water mode 2 meter.
        /// </summary>
        [XmlEnum("12")]
        HotWaterMode2,
        /// <summary>
        /// Water mode 2 meter.
        /// </summary>
        [XmlEnum("13")]
        WaterMode2,
        /// <summary>
        /// Heat cost allocator mode 2 meter.
        /// </summary>
        [XmlEnum("14")]
        HeatCostAllocatorMode2,
        /// <summary>
        /// Reserver.
        /// </summary>
        [XmlEnum("15")]
        Reserved2
    }
}
