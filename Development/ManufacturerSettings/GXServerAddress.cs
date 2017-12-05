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
using System.Xml.Serialization;
using System.ComponentModel;

namespace Gurux.DLMS.ManufacturerSettings
{
    /// <summary>
    /// Available HDLC Address types.
    /// </summary>
    /// <remarks>
    /// Read more from HDLC Address counting from here:
    /// http://www.gurux.fi/index.php?q=node/336 
    /// </remarks>
    public enum HDLCAddressType : int
    {
        /// <summary>
        /// Default HDLC Address count is used.
        /// </summary>
        /// <remarks>
        /// Newer meters usually supports this.
        /// </remarks>
        [XmlEnum("0")]
        Default,
        /// <summary>
        /// HDLC Address is counted from serial number.
        /// </summary>
        [XmlEnum("1")]
        SerialNumber,
        /// <summary>
        /// Custom HDLC address counting is used.
        /// <remarks>
        /// Some older meters needs this.
        /// </remarks>
        /// </summary>
        [XmlEnum("2")]
        Custom
    }

    /// <summary>
    /// Server address settings.
    /// </summary>
#if !WINDOWS_UWP
    [Serializable]
#endif
    public class GXServerAddress
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXServerAddress()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXServerAddress(HDLCAddressType address, int value, bool enabled)
        {
            HDLCAddress = address;
            PhysicalAddress = value;
        }

        /// <summary>
        /// Is server address enabled
        /// </summary>
        [DefaultValue(false)]
        public bool Selected
        {
            get;
            set;
        }

        /// <summary>
        /// HDLC address type.
        /// </summary>
        public HDLCAddressType HDLCAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Used formula.
        /// </summary>
        public string Formula
        {
            get;
            set;
        }

        /// <summary>
        /// Physical address of the meter.
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [DefaultValue(null)]
        public int PhysicalAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Logical address of the meter.
        /// </summary>
        [DefaultValue(0)]
        public int LogicalAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Custom address size of the meter.
        /// </summary>
        /// <remarks>
        /// Some meters require fixed address size.
        /// </remarks>
        [DefaultValue(0)]
        public int Size
        {
            get;
            set;
        }
    }
}
