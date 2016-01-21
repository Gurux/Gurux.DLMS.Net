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
using System.Reflection;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSIECOpticalPortSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSIECOpticalPortSetup()
            : base(ObjectType.IecLocalPortSetup, "0.0.20.0.0.255", 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSIECOpticalPortSetup(string ln)
            : base(ObjectType.IecLocalPortSetup, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIECOpticalPortSetup(string ln, ushort sn)
            : base(ObjectType.IecLocalPortSetup, ln, sn)
        {
        }

        /// <summary>
        /// Start communication mode.
        /// </summary>
        [XmlIgnore()]
        public OpticalProtocolMode DefaultMode
        {
            get;
            set;
        }

        /// <summary>
        /// Default Baudrate.
        /// </summary>
        [XmlIgnore()]        
        public BaudRate DefaultBaudrate
        {
            get;
            set;
        }

        /// <summary>
        /// Proposed Baudrate.
        /// </summary>
        [XmlIgnore()]
        public BaudRate ProposedBaudrate
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the minimum time between the reception of a request 
        /// (end of request telegram) and the transmission of the response (begin of response telegram).
        /// </summary>
        [XmlIgnore()]
        public LocalPortResponseTime ResponseTime
        {
            get;
            set;
        }

        /// <summary>
        /// Device address according to IEC 62056-21.
        /// </summary>
        [XmlIgnore()]
        public string DeviceAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Password 1 according to IEC 62056-21.
        /// </summary>
        [XmlIgnore()]
        public string Password1
        {
            get;
            set;
        }

        /// <summary>
        /// Password 2 according to IEC 62056-21.
        /// </summary>
        [XmlIgnore()]
        public string Password2
        {
            get;
            set;
        }

        /// <summary>
        /// Password W5 reserved for national applications.
        /// </summary>
        [XmlIgnore()]
        public string Password5
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, DefaultMode, DefaultBaudrate, 
                ProposedBaudrate, ResponseTime, DeviceAddress, 
                Password1, Password2, Password5 };
        }

        #region IGXDLMSBase Members

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //DefaultMode
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }
            //DefaultBaudrate
            if (!base.IsRead(3))
            {
                attributes.Add(3);
            }
            //ProposedBaudrate
            if (!base.IsRead(4))
            {
                attributes.Add(4);
            }
            //ResponseTime
            if (!base.IsRead(5))
            {
                attributes.Add(5);
            }
            //DeviceAddress
            if (!base.IsRead(6))
            {
                attributes.Add(6);
            }
            //Password1
            if (!base.IsRead(7))
            {
                attributes.Add(7);
            }
            //Password2
            if (!base.IsRead(8))
            {
                attributes.Add(8);
            }
            //Password5
            if (!base.IsRead(9))
            {
                attributes.Add(9);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Gurux.DLMS.Properties.Resources.LogicalNameTxt, 
                "Default Mode", 
                "Default Baud rate",
                "Proposed Baud rate", 
                "Response Time", 
                "Device Address", 
                "Password 1", 
                "Password 2", 
                "Password 5"};
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 9;
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
                return DataType.Enum;
            }
            if (index == 6)
            {
                return DataType.OctetString;
            }
            if (index == 7)
            {
                return DataType.OctetString;
            }
            if (index == 8)
            {
                return DataType.OctetString;
            }
            if (index == 9)
            {
                return DataType.OctetString;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, int index, int selector, object parameters)
        {
            if (index == 1)
            {
                return this.LogicalName;
            }
            if (index == 2)
            {
                return this.DefaultMode;
            }
            if (index == 3)
            {
                return this.DefaultBaudrate;
            }
            if (index == 4)
            {
                return this.ProposedBaudrate;
            }
            if (index == 5)
            {
                return this.ResponseTime;
            }
            if (index == 6)
            {
                return ASCIIEncoding.ASCII.GetBytes(DeviceAddress);
            }
            if (index == 7)
            {
                return ASCIIEncoding.ASCII.GetBytes(Password1);
            }
            if (index == 8)
            {
                return ASCIIEncoding.ASCII.GetBytes(Password2);
            }
            if (index == 9)
            {
                return ASCIIEncoding.ASCII.GetBytes(Password5);
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, int index, object value) 
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
                DefaultMode = (OpticalProtocolMode)Convert.ToInt32(value);
            }
            else if (index == 3)
            {
                DefaultBaudrate = (BaudRate)Convert.ToInt32(value);
            }
            else if (index == 4)
            {
                ProposedBaudrate = (BaudRate)Convert.ToInt32(value);
            }
            else if (index == 5)
            {
                ResponseTime = (LocalPortResponseTime)Convert.ToInt32(value);
            }
            else if (index == 6)
            {
                if (value is byte[])
                {
                    DeviceAddress = GXDLMSClient.ChangeType((byte[])value, DataType.String).ToString();
                }
                else
                {
                    DeviceAddress = Convert.ToString(value);
                }
            }
            else if (index == 7)
            {
                if (value is byte[])
                {
                    Password1 = GXDLMSClient.ChangeType((byte[])value, DataType.String).ToString();
                }
                else
                {
                    Password1 = Convert.ToString(value);
                }
            }
            else if (index == 8)
            {
                if (value is byte[])
                {
                    Password2 = GXDLMSClient.ChangeType((byte[])value, DataType.String).ToString();
                }
                else
                {
                    Password2 = Convert.ToString(value);
                }
            }
            else if (index == 9)
            {
                if (value is byte[])
                {
                    Password5 = GXDLMSClient.ChangeType((byte[])value, DataType.String).ToString();
                }
                else
                {
                    Password5 = Convert.ToString(value);
                }
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
        }

        #endregion
    }
}
