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
    /// InterfaceType enumerates the usable types of connection in GuruxDLMS.
    /// </summary>
    public enum InterfaceType
    {
        /// <summary>
        /// General interface type is used for meters that supports
        /// IEC 62056-46 Data link layer using HDLC protocol.
        /// </summary>
        [XmlEnum("0")]
        HDLC,
        /// <summary>
        /// Network interface type is used for meters that supports
        /// IEC 62056-47 COSEM transport layers for IPv4 networks.
        /// </summary>
        [XmlEnum("1")]
        WRAPPER,
        /// <summary>
        /// Plain PDU is returned.
        /// </summary>
        [XmlEnum("2")]
        PDU,
        /// <summary>
        /// EN 13757-4/-5 Wireless M-Bus profile is used.
        /// </summary>
        [XmlEnum("3")]
        WirelessMBus,
        /// <summary>
        /// IEC 62056-21 E-Mode is used to initialize communication before moving to HDLC protocol.
        /// </summary>
        [XmlEnum("4")]
        HdlcWithModeE,
        /// <summary>
        /// PLC Logical link control (LLC) profile is used with IEC 61334-4-32 connectionless LLC sublayer.
        /// </summary>
        /// <remarks>
        /// Blue Book: 10.4.4.3.3 The connectionless LLC sublayer.
        /// </remarks>
        [XmlEnum("5")]
        Plc,
        /// <summary>
        /// PLC Logical link control (LLC) profile is used with HDLC.
        /// </summary>
        /// <remarks>
        /// Blue Book: 10.4.4.3.4 The HDLC based LLC sublayer.
        /// </remarks>
        [XmlEnum("6")]
        PlcHdlc,
        /// <summary>
        /// LowPower Wide Area Networks (LPWAN) profile is used.
        /// </summary>
        [XmlEnum("7")]
        LPWAN,
        /// <summary>
        /// Wi-SUN FAN mesh network is used.
        /// </summary>
        [XmlEnum("8")]
        WiSUN,
        /// <summary>
        /// OFDM PLC PRIME is defined in IEC 62056-8-4.
        /// </summary>
        [XmlEnum("9")]
        PlcPrime,
        /// <summary>
        /// EN 13757-2 wired (twisted pair based) M-Bus scheme is used.
        /// </summary>
        [XmlEnum("10")]
        WiredMBus,
        /// <summary>
        /// SMS short wrapper scheme is used.
        /// </summary>
        [XmlEnum("11")]
        SMS
    }
}
