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
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

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
                              Password1, Password2, Password5
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
            //DefaultMode
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            //DefaultBaudrate
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //ProposedBaudrate
            if (all || !base.IsRead(4))
            {
                attributes.Add(4);
            }
            //ResponseTime
            if (all || !base.IsRead(5))
            {
                attributes.Add(5);
            }
            //DeviceAddress
            if (all || !base.IsRead(6))
            {
                attributes.Add(6);
            }
            //Password1
            if (all || !base.IsRead(7))
            {
                attributes.Add(7);
            }
            //Password2
            if (all || !base.IsRead(8))
            {
                attributes.Add(8);
            }
            //Password5
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
                             "Default Mode",
                             "Default Baud rate",
                             "Proposed Baud rate",
                             "Response Time",
                             "Device Address",
                             "Password 1",
                             "Password 2",
                             "Password 5"
                            };
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
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.Enum;
                case 3:
                    return DataType.Enum;
                case 4:
                    return DataType.Enum;
                case 5:
                    return DataType.Enum;
                case 6:
                    return DataType.OctetString;
                case 7:
                    return DataType.OctetString;
                case 8:
                    return DataType.OctetString;
                case 9:
                    return DataType.OctetString;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return this.DefaultMode;
                case 3:
                    return this.DefaultBaudrate;
                case 4:
                    return this.ProposedBaudrate;
                case 5:
                    return this.ResponseTime;
                case 6:
                    if (DeviceAddress == null)
                    {
                        return null;
                    }
                    return ASCIIEncoding.ASCII.GetBytes(DeviceAddress);
                case 7:
                    if (Password1 == null)
                    {
                        return null;
                    }
                    return ASCIIEncoding.ASCII.GetBytes(Password1);
                case 8:
                    if (Password2 == null)
                    {
                        return null;
                    }
                    return ASCIIEncoding.ASCII.GetBytes(Password2);
                case 9:
                    if (Password5 == null)
                    {
                        return null;
                    }
                    return ASCIIEncoding.ASCII.GetBytes(Password5);
            }

            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    DefaultMode = (OpticalProtocolMode)Convert.ToInt32(e.Value);
                    break;
                case 3:
                    DefaultBaudrate = (BaudRate)Convert.ToInt32(e.Value);
                    break;
                case 4:
                    ProposedBaudrate = (BaudRate)Convert.ToInt32(e.Value);
                    break;
                case 5:
                    ResponseTime = (LocalPortResponseTime)Convert.ToInt32(e.Value);
                    break;
                case 6:
                    if (e.Value is byte[])
                    {
                        DeviceAddress = GXDLMSClient.ChangeType((byte[])e.Value, DataType.String, settings.UseUtc2NormalTime).ToString();
                    }
                    else
                    {
                        DeviceAddress = Convert.ToString(e.Value);
                    }
                    break;
                case 7:
                    if (e.Value is byte[])
                    {
                        Password1 = GXDLMSClient.ChangeType((byte[])e.Value, DataType.String, settings.UseUtc2NormalTime).ToString();
                    }
                    else
                    {
                        Password1 = Convert.ToString(e.Value);
                    }
                    break;
                case 8:
                    if (e.Value is byte[])
                    {
                        Password2 = GXDLMSClient.ChangeType((byte[])e.Value, DataType.String, settings.UseUtc2NormalTime).ToString();
                    }
                    else
                    {
                        Password2 = Convert.ToString(e.Value);
                    }
                    break;
                case 9:
                    if (e.Value is byte[])
                    {
                        Password5 = GXDLMSClient.ChangeType((byte[])e.Value, DataType.String, settings.UseUtc2NormalTime).ToString();
                    }
                    else
                    {
                        Password5 = Convert.ToString(e.Value);
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            DefaultMode = (OpticalProtocolMode)reader.ReadElementContentAsInt("DefaultMode");
            DefaultBaudrate = (BaudRate)reader.ReadElementContentAsInt("DefaultBaudrate");
            ProposedBaudrate = (BaudRate)reader.ReadElementContentAsInt("ProposedBaudrate");
            ResponseTime = (LocalPortResponseTime)reader.ReadElementContentAsInt("ResponseTime");
            DeviceAddress = reader.ReadElementContentAsString("DeviceAddress");
            Password1 = reader.ReadElementContentAsString("Password1");
            Password2 = reader.ReadElementContentAsString("Password2");
            Password5 = reader.ReadElementContentAsString("Password5");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("DefaultMode", (int)DefaultMode);
            writer.WriteElementString("DefaultBaudrate", (int)DefaultBaudrate);
            writer.WriteElementString("ProposedBaudrate", (int)ProposedBaudrate);
            writer.WriteElementString("ResponseTime", (int)ResponseTime);
            writer.WriteElementString("DeviceAddress", DeviceAddress);
            writer.WriteElementString("Password1", Password1);
            writer.WriteElementString("Password2", Password2);
            writer.WriteElementString("Password5", Password5);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
