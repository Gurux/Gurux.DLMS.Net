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

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// Prime Data Concentrator message type.
    /// </summary>
    public enum PrimeDcMsgType : byte
    {
        /// <summary>
        /// New device notification.
        /// </summary>
        [XmlEnum("1")]
        NewDeviceNotification = 1,
        /// <summary>
        /// Remove device notification.
        /// </summary>
        [XmlEnum("2")]
        RemoveDeviceNotification = 2,
        /// <summary>
        /// Start reporting meters.
        /// </summary>
        [XmlEnum("3")]
        StartReportingMeters = 3,
        /// <summary>
        /// Delete meters.
        /// </summary>
        [XmlEnum("4")]
        DeleteMeters = 4,
        /// <summary>
        /// Enable auto close.
        /// </summary>
        [XmlEnum("5")]
        EnableAutoClose = 5,
        /// <summary>
        /// Disable auto close.
        /// </summary>
        [XmlEnum("6")]
        DisableAutoClose = 6
    }
}