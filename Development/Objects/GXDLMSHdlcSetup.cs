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
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSHdlcSetup
    /// </summary>
    public class GXDLMSHdlcSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSHdlcSetup()
        : this("0.0.22.0.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSHdlcSetup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSHdlcSetup(string ln, ushort sn)
        : base(ObjectType.IecHdlcSetup, ln, sn)
        {
            CommunicationSpeed = BaudRate.Baudrate9600;
            WindowSizeReceive = WindowSizeTransmit = 1;
            MaximumInfoLengthTransmit = MaximumInfoLengthReceive = 128;
            InactivityTimeout = 120;
            InterCharachterTimeout = 30;
            Version = 1;
        }

        [XmlIgnore()]
        [DefaultValue(BaudRate.Baudrate9600)]
        public BaudRate CommunicationSpeed
        {
            get;
            set;
        }

        [XmlIgnore()]
        [DefaultValue(1)]
        public byte WindowSizeTransmit
        {
            get;
            set;
        }

        [XmlIgnore()]
        [DefaultValue(1)]
        public byte WindowSizeReceive
        {
            get;
            set;
        }

        [XmlIgnore()]
        [DefaultValue(128)]
        public UInt16 MaximumInfoLengthTransmit
        {
            get;
            set;
        }

        [XmlIgnore()]
        [DefaultValue(128)]
        public UInt16 MaximumInfoLengthReceive
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
                              InterCharachterTimeout, InactivityTimeout, DeviceAddress
                            };
        }

        #region IGXDLMSBase Members

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //CommunicationSpeed
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            //WindowSizeTransmit
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //WindowSizeReceive
            if (all || !base.IsRead(4))
            {
                attributes.Add(4);
            }
            //MaximumInfoLengthTransmit
            if (all || !base.IsRead(5))
            {
                attributes.Add(5);
            }
            //MaximumInfoLengthReceive
            if (all || !base.IsRead(6))
            {
                attributes.Add(6);
            }
            //InterCharachterTimeout
            if (all || !base.IsRead(7))
            {
                attributes.Add(7);
            }
            //InactivityTimeout
            if (all || !base.IsRead(8))
            {
                attributes.Add(8);
            }
            //DeviceAddress
            if (all || !base.IsRead(9))
            {
                attributes.Add(9);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Communication Speed",
                             "Window Size Transmit",
                             "Window Size Receive",
                             "Maximum Info Length Transmit",
                             "Maximum Info Length Receive",
                             "InterCharachter Timeout",
                             "Inactivity Timeout",
                             "Device Address"
                            };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[0];
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 1;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 9;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
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

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return this.CommunicationSpeed;
            }
            if (e.Index == 3)
            {
                return this.WindowSizeTransmit;
            }
            if (e.Index == 4)
            {
                return this.WindowSizeReceive;
            }
            if (e.Index == 5)
            {
                return this.MaximumInfoLengthTransmit;
            }
            if (e.Index == 6)
            {
                return this.MaximumInfoLengthReceive;
            }
            if (e.Index == 7)
            {
                return InterCharachterTimeout;
            }
            if (e.Index == 8)
            {
                return InactivityTimeout;
            }
            if (e.Index == 9)
            {
                return DeviceAddress;
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
                CommunicationSpeed = (BaudRate)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 3)
            {
                WindowSizeTransmit = Convert.ToByte(e.Value);
            }
            else if (e.Index == 4)
            {
                WindowSizeReceive = Convert.ToByte(e.Value);
            }
            else if (e.Index == 5)
            {
                MaximumInfoLengthTransmit = Convert.ToUInt16(e.Value);
            }
            else if (e.Index == 6)
            {
                MaximumInfoLengthReceive = Convert.ToUInt16(e.Value);
            }
            else if (e.Index == 7)
            {
                InterCharachterTimeout = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 8)
            {
                InactivityTimeout = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 9)
            {
                DeviceAddress = Convert.ToInt32(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            CommunicationSpeed = (BaudRate)reader.ReadElementContentAsInt("Speed");
            WindowSizeTransmit = (byte)reader.ReadElementContentAsInt("WindowSizeTx");
            WindowSizeReceive = (byte)reader.ReadElementContentAsInt("WindowSizeRx");
            MaximumInfoLengthTransmit = (UInt16)reader.ReadElementContentAsInt("MaximumInfoLengthTx");
            MaximumInfoLengthReceive = (UInt16)reader.ReadElementContentAsInt("MaximumInfoLengthRx");
            InterCharachterTimeout = reader.ReadElementContentAsInt("InterCharachterTimeout");
            InactivityTimeout = reader.ReadElementContentAsInt("InactivityTimeout");
            DeviceAddress = reader.ReadElementContentAsInt("DeviceAddress");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Speed", (int)CommunicationSpeed, (int)BaudRate.Baudrate9600);
            writer.WriteElementString("WindowSizeTx", WindowSizeTransmit, 1);
            writer.WriteElementString("WindowSizeRx", WindowSizeReceive, 1);
            writer.WriteElementString("MaximumInfoLengthTx", MaximumInfoLengthTransmit, 0x80);
            writer.WriteElementString("MaximumInfoLengthRx", MaximumInfoLengthReceive, 0x80);
            writer.WriteElementString("InterCharachterTimeout", InterCharachterTimeout, 30);
            writer.WriteElementString("InactivityTimeout", InactivityTimeout, 120);
            writer.WriteElementString("DeviceAddress", DeviceAddress, 0);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
