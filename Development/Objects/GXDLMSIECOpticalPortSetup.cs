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

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Defines the minimum time between the reception of a request 
    /// (end of request telegram) and the transmission of the response (begin of response telegram).
    /// </summary>
    public enum LocalPortResponseTime
    {
        /// <summary>
        /// Minimium time is 20 ms.
        /// </summary>
        ms20 = 0,
        /// <summary>
        /// Minimium time is 200 ms.
        /// </summary>
        ms200 = 1
    }
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
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSIECOpticalPortSetup(string ln)
            : base(ObjectType.IecLocalPortSetup, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIECOpticalPortSetup(string ln, ushort sn)
            : base(ObjectType.IecLocalPortSetup, ln, 0)
        {
        }

        /// <summary>
        /// Start communication mode.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(2)]
        public OpticalProtocolMode DefaultMode
        {
            get;
            set;
        }

        /// <summary>
        /// Default Baudrate.
        /// </summary>
        [XmlIgnore()]        
        [GXDLMSAttribute(3)]
        public BaudRate DefaultBaudrate
        {
            get;
            set;
        }

        /// <summary>
        /// Proposed Baudrate.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(4)]
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
        [GXDLMSAttribute(5)]
        public LocalPortResponseTime ResponseTime
        {
            get;
            set;
        }

        /// <summary>
        /// Device address according to IEC 62056-21.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(6, DataType.String)]
        public string DeviceAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Password 1 according to IEC 62056-21.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(7, DataType.String)]
        public string Password1
        {
            get;
            set;
        }

        /// <summary>
        /// Password 2 according to IEC 62056-21.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(8, DataType.String)]
        public string Password2
        {
            get;
            set;
        }

        /// <summary>
        /// Password W5 reserved for national applications.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(9, DataType.String)]
        public string Password5
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, DefaultMode, DefaultBaudrate, ProposedBaudrate, ResponseTime, DeviceAddress, Password1, Password2, Password5 };
        }

        #region IGXDLMSBase Members

        int IGXDLMSBase.GetAttributeCount()
        {
            return 9;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        object IGXDLMSBase.GetValue(int index, out DataType type, byte[] parameters)
        {
            if (index == 1)
            {
                type = DataType.OctetString;
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                type = DataType.Enum;
                return this.DefaultMode;
            }
            if (index == 3)
            {
                type = DataType.Enum;
                return this.DefaultBaudrate;
            }
            if (index == 4)
            {
                type = DataType.Enum;
                return this.ProposedBaudrate;
            }
            if (index == 5)
            {
                type = DataType.Enum;
                return this.ResponseTime;
            }
            if (index == 6)
            {
                type = DataType.OctetString;
                return ASCIIEncoding.ASCII.GetBytes(DeviceAddress);
            }
            if (index == 7)
            {
                type = DataType.OctetString;
                return ASCIIEncoding.ASCII.GetBytes(Password1);
            }
            if (index == 8)
            {
                type = DataType.OctetString;
                return ASCIIEncoding.ASCII.GetBytes(Password2);
            }
            if (index == 9)
            {
                type = DataType.OctetString;
                return ASCIIEncoding.ASCII.GetBytes(Password5);
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value)
        {
            if (index == 1)
            {
                LogicalName = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
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
                DeviceAddress = GXDLMSClient.ChangeType((byte[])value, DataType.String).ToString();
            }
            else if (index == 7)
            {
                Password1 = GXDLMSClient.ChangeType((byte[])value, DataType.String).ToString();
            }
            else if (index == 8)
            {
                Password2 = GXDLMSClient.ChangeType((byte[])value, DataType.String).ToString();
            }
            else if (index == 9)
            {
                Password5 = GXDLMSClient.ChangeType((byte[])value, DataType.String).ToString();
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }

        void IGXDLMSBase.Invoke(int index, object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
        }

        #endregion
    }
}
