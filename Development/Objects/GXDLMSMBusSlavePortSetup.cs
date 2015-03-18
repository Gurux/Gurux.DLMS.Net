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
using Gurux.DLMS;
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;

namespace Gurux.DLMS.Objects
{
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
            : base(ObjectType.MBusSlavePortSetup, ln, 0)
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
        /// Defines the baud rate for the opening sequence.
        /// </summary>
        [XmlIgnore()]
        public int BusAddress
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new Object[] { LogicalName, DefaultBaud, AvailableBaud, AddressState, BusAddress };
        }

        #region IGXDLMSBase Members

        /// <summary>
        /// Data interface do not have any methods.
        /// </summary>
        /// <param name="index"></param>
        byte[][] IGXDLMSBase.Invoke(object sender, int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //DefaultBaud
            if (!IsRead(2))
            {
                attributes.Add(2);
            }
            //AvailableBaud
            if (!IsRead(3))
            {
                attributes.Add(3);
            }
            //AddressState
            if (!IsRead(4))
            {
                attributes.Add(4);
            }
            //BusAddress
            if (!IsRead(5))
            {
                attributes.Add(5);
            }             
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Default Baud Rate", 
                "Available Baud rate", "Address State", "Bus Address" };            
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        override public DataType GetDataType(int index)
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
                return DataType.UInt16;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(int index, int selector, object parameters)
        {
            if (index == 1)
            {
                return this.LogicalName;
            }
            if (index == 2)
            {
                return DefaultBaud;
            }
            if (index == 3)
            {
                return AvailableBaud;
            }
            if (index == 4)
            {
                return AddressState;
            }
            if (index == 5)
            {
                return BusAddress;
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value)
        {
            if (index == 1)
            {
                if (value is string)
                {
                    LogicalName = value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
                }
            }
            else if (index == 2)
            {
                if (value == null)
                {
                    DefaultBaud = BaudRate.Baudrate300;
                }
                else
                {
                    DefaultBaud = (BaudRate)Convert.ToInt32(value);
                }
            }
            else if (index == 3)
            {
                if (value == null)
                {
                    AvailableBaud = BaudRate.Baudrate300;
                }
                else
                {
                    AvailableBaud = (BaudRate)Convert.ToInt32(value);
                }
            }
            else if (index == 4)
            {
                if (value == null)
                {
                    AddressState = AddressState.None;
                }
                else
                {
                    AddressState = (AddressState)Convert.ToInt32(value);
                }
            }
            else if (index == 5)
            {
                if (value == null)
                {
                    BusAddress = 0;
                }
                else
                {
                    BusAddress = Convert.ToInt32(value);
                }
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
        #endregion
    }
}