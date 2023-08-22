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
using System.Xml.Serialization;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Model and configure communication channels.
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSMBusSlavePortSetup
    /// </summary>
    public class GXDLMSMBusSlavePortSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSMBusSlavePortSetup()
        : base(ObjectType.MBusSlavePortSetup)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSMBusSlavePortSetup(string ln)
        : base(ObjectType.MBusSlavePortSetup, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSMBusSlavePortSetup(string ln, ushort sn)
        : base(ObjectType.MBusSlavePortSetup, ln, sn)
        {
        }

        /// <summary>
        /// Defines the baud rate for the opening sequence.
        /// </summary>
        [XmlIgnore()]
        public BaudRate DefaultBaud
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the baud rate for the opening sequence.
        /// </summary>
        [XmlIgnore()]
        public BaudRate AvailableBaud
        {
            get;
            set;
        }

        /// <summary>
        /// Defines whether or not the device has been assigned an address
        /// since last power up of the device.
        /// </summary>
        [XmlIgnore()]
        public AddressState AddressState
        {
            get;
            set;
        }


        /// <summary>
        /// The currently assigned device address.
        /// </summary>
        [XmlIgnore()]
        public int BusAddress
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new Object[] { LogicalName, DefaultBaud, AvailableBaud, AddressState, BusAddress };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //DefaultBaud
            if (all || !IsRead(2))
            {
                attributes.Add(2);
            }
            //AvailableBaud
            if (all || !IsRead(3))
            {
                attributes.Add(3);
            }
            //AddressState
            if (all || !IsRead(4))
            {
                attributes.Add(4);
            }
            //BusAddress
            if (all || !IsRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Default Baud Rate",
                              "Available Baud rate", "Address State", "Bus Address"
                            };
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[0];
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.Enum;
            }
            if (index == 3)
            {
                return DataType.Enum;
            }
            if (index == 4)
            {
                return DataType.Enum;
            }
            if (index == 5)
            {
                return DataType.UInt8;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return DefaultBaud;
            }
            if (e.Index == 3)
            {
                return AvailableBaud;
            }
            if (e.Index == 4)
            {
                return AddressState;
            }
            if (e.Index == 5)
            {
                return BusAddress;
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                LogicalName = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 2)
            {
                if (e.Value == null)
                {
                    DefaultBaud = BaudRate.Baudrate300;
                }
                else
                {
                    DefaultBaud = (BaudRate)Convert.ToInt32(e.Value);
                }
            }
            else if (e.Index == 3)
            {
                if (e.Value == null)
                {
                    AvailableBaud = BaudRate.Baudrate300;
                }
                else
                {
                    AvailableBaud = (BaudRate)Convert.ToInt32(e.Value);
                }
            }
            else if (e.Index == 4)
            {
                if (e.Value == null)
                {
                    AddressState = AddressState.None;
                }
                else
                {
                    AddressState = (AddressState)Convert.ToInt32(e.Value);
                }
            }
            else if (e.Index == 5)
            {
                if (e.Value == null)
                {
                    BusAddress = 0;
                }
                else
                {
                    BusAddress = Convert.ToInt32(e.Value);
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            DefaultBaud = (BaudRate)reader.ReadElementContentAsInt("DefaultBaud");
            AvailableBaud = (BaudRate)reader.ReadElementContentAsInt("AvailableBaud");
            AddressState = (AddressState)reader.ReadElementContentAsInt("AddressState");
            BusAddress = reader.ReadElementContentAsInt("BusAddress");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("DefaultBaud", (int)DefaultBaud, 2);
            writer.WriteElementString("AvailableBaud", (int)AvailableBaud, 3);
            writer.WriteElementString("AddressState", (int)AddressState, 4);
            writer.WriteElementString("BusAddress", BusAddress, 5);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}