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
using System.ComponentModel;
using Gurux.DLMS;



using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSHdlcSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSHdlcSetup()
            : base(ObjectType.IecHdlcSetup)
        {
            CommunicationSpeed = BaudRate.Baudrate9600;
            WindowSizeReceive = WindowSizeTransmit = 1;
            MaximumInfoLengthTransmit = MaximumInfoLengthReceive = 128;
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSHdlcSetup(string ln)
            : base(ObjectType.IecHdlcSetup, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSHdlcSetup(string ln, ushort sn)
            : base(ObjectType.IecHdlcSetup, ln, 0)
        {
        }       

        [XmlIgnore()]
        public BaudRate CommunicationSpeed
        {
            get;
            set;
        }

        [XmlIgnore()]
        [DefaultValue(1)]
        public int WindowSizeTransmit
        {
            get;
            set;
        }

        [XmlIgnore()]
        [DefaultValue(1)]
        public int WindowSizeReceive
        {
            get;
            set;
        }

        [XmlIgnore()]
        [DefaultValue(128)]
        public int MaximumInfoLengthTransmit
        {
            get;
            set;
        }

        [XmlIgnore()]
        [DefaultValue(128)]
        public int MaximumInfoLengthReceive
        {
            get;
            set;
        }

        [XmlIgnore()]
        [DefaultValue(30)]
        public int InterCharachterTimeout
        {
            get;
            set;
        }

        [XmlIgnore()]
        [DefaultValue(120)]
        public int InactivityTimeout
        {
            get;
            set;
        }

        [XmlIgnore()]
        [DefaultValue(0)]
        public int DeviceAddress
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, CommunicationSpeed, 
                WindowSizeTransmit, WindowSizeReceive, 
                MaximumInfoLengthTransmit, MaximumInfoLengthReceive, 
                InterCharachterTimeout, InactivityTimeout, DeviceAddress };
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
            //CommunicationSpeed
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }
            //WindowSizeTransmit
            if (!base.IsRead(3))
            {
                attributes.Add(3);
            }
            //WindowSizeReceive
            if (!base.IsRead(4))
            {
                attributes.Add(4);
            }
            //MaximumInfoLengthTransmit
            if (!base.IsRead(5))
            {
                attributes.Add(5);
            }
            //MaximumInfoLengthReceive
            if (!base.IsRead(6))
            {
                attributes.Add(6);
            }
            //InterCharachterTimeout
            if (!base.IsRead(7))
            {
                attributes.Add(7);
            }
            //InactivityTimeout
            if (!base.IsRead(8))
            {
                attributes.Add(8);
            }
            //DeviceAddress
            if (!base.IsRead(9))
            {
                attributes.Add(9);
            }
            return attributes.ToArray();
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
                return DataType.UInt8;
            }
            if (index == 4)
            {
                return DataType.UInt8;
            }
            if (index == 5)
            {
                return DataType.UInt16;
            }
            if (index == 6)
            {
                return DataType.UInt16;
            }
            if (index == 7)
            {
                return DataType.UInt16;
            }
            if (index == 8)
            {
                return DataType.UInt16;
            }
            if (index == 9)
            {
                return DataType.UInt16;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(int index, int selector, object parameters)
        {
            if (index == 1)
            {
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                return this.CommunicationSpeed;
            }
            if (index == 3)
            {
                return this.WindowSizeTransmit;
            }
            if (index == 4)
            {
                return this.WindowSizeReceive;
            }
            if (index == 5)
            {
                return this.MaximumInfoLengthTransmit;
            }
            if (index == 6)
            {
                return this.MaximumInfoLengthReceive;
            }
            if (index == 7)
            {
                return InterCharachterTimeout;
            }
            if (index == 8)
            {
                return InactivityTimeout;
            }
            if (index == 9)
            {
                return DeviceAddress;
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
                CommunicationSpeed = (BaudRate) Convert.ToInt32(value);
            }
            else if (index == 3)
            {
                WindowSizeTransmit = Convert.ToInt32(value);
            }
            else if (index == 4)
            {
                WindowSizeReceive = Convert.ToInt32(value);
            }
            else if (index == 5)
            {
                MaximumInfoLengthTransmit = Convert.ToInt32(value);
            }
            else if (index == 6)
            {
                MaximumInfoLengthReceive = Convert.ToInt32(value);
            }
            else if (index == 7)
            {
                InterCharachterTimeout = Convert.ToInt32(value);
            }
            else if (index == 8)
            {
                InactivityTimeout = Convert.ToInt32(value);
            }
            else if (index == 9)
            {
                DeviceAddress = Convert.ToInt32(value);
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }

        byte[][] IGXDLMSBase.Invoke(object sender, int index, Object parameters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
