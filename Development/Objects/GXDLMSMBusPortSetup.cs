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
using System.Globalization;
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSMBusPortSetup
    /// </summary>
    public class GXDLMSMBusPortSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSMBusPortSetup()
        : this("0.0.24.8.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSMBusPortSetup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSMBusPortSetup(string ln, ushort sn)
        : base(ObjectType.MBusPortSetup, ln, sn)
        {
            ListeningWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
        }

        /// <summary>
        /// References an M-Bus communication port setup object describing 
        /// the physical capabilities for wired or wireless communication.
        /// </summary>
        [XmlIgnore()]
        public string ProfileSelection
        {
            get;
            set;
        }

        /// <summary>
        /// Communication status of the M-Bus node.
        /// </summary>
        [XmlIgnore()]
        public MBusPortCommunicationState PortCommunicationStatus
        {
            get;
            set;
        }

        /// <summary>
        /// M-Bus data header type.
        /// </summary>
        [XmlIgnore()]
        public MBusDataHeaderType DataHeaderType
        {
            get;
            set;
        }

        /// <summary>
        /// The primary address of the M-Bus slave device. 
        /// </summary>
        [XmlIgnore()]
        public byte PrimaryAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Identification Number element of the data header.
        /// </summary>
        [XmlIgnore()]
        public UInt32 IdentificationNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Manufacturer Identification element.
        /// </summary>
        [XmlIgnore()]
        public UInt16 ManufacturerId
        {
            get;
            set;
        }

        /// <summary>
        /// M-Bus version.
        /// </summary>
        [XmlIgnore()]
        public byte MBusVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Device type.
        /// </summary>
        [XmlIgnore()]
        public MBusDeviceType DeviceType
        {
            get;
            set;
        }

        /// <summary>
        /// Max PDU size.
        /// </summary>
        [XmlIgnore()]
        public UInt16 MaxPduSize
        {
            get;
            set;
        }

        /// <summary>
        /// Listening windows.
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<GXDateTime, GXDateTime>> ListeningWindow
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ProfileSelection, PortCommunicationStatus, DataHeaderType, PrimaryAddress, IdentificationNumber,
            ManufacturerId,Version, DeviceType, MaxPduSize, ListeningWindow };
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
            //ProfileSelection
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //PortCommunicationStatus
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //DataHeaderType
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //PrimaryAddress
            if (all || !base.IsRead(5))
            {
                attributes.Add(5);
            }
            //IdentificationNumber            
            if (all || !base.IsRead(6))
            {
                attributes.Add(6);
            }
            //ManufacturerId
            if (all || !base.IsRead(7))
            {
                attributes.Add(7);
            }
            //MBusVersion
            if (all || !base.IsRead(8))
            {
                attributes.Add(8);
            }
            //DeviceType
            if (all || !base.IsRead(9))
            {
                attributes.Add(9);
            }
            //MaxPduSize, 
            if (all || !base.IsRead(10))
            {
                attributes.Add(10);
            }
            //ListeningWindow
            if (all || !base.IsRead(11))
            {
                attributes.Add(11);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(),  "Profile selection", "Port communication status",
                "Data header type", "Primary address", "Identification number",
            "Manufacturer Id", "MBus version", "Device type", "Max PDU size", "Listening window"};
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
            return 11;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.OctetString;
                case 3:
                    return DataType.Enum;
                case 4:
                    return DataType.Enum;
                case 5:
                    return DataType.UInt8;
                case 6:
                    return DataType.UInt32;
                case 7:
                    return DataType.UInt16;
                case 8:
                    return DataType.UInt8;
                case 9:
                    return DataType.UInt8;
                case 10:
                    return DataType.UInt16;
                case 11:
                    return DataType.Array;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return GXCommon.LogicalNameToBytes(ProfileSelection);
                case 3:
                    return PortCommunicationStatus;
                case 4:
                    return DataHeaderType;
                case 5:
                    return PrimaryAddress;
                case 6:
                    return IdentificationNumber;
                case 7:
                    return ManufacturerId;
                case 8:
                    return MBusVersion;
                case 9:
                    return DeviceType;
                case 10:
                    return MaxPduSize;
                case 11:
                    int cnt = ListeningWindow.Count;
                    GXByteBuffer data = new GXByteBuffer();
                    data.SetUInt8((byte)DataType.Array);
                    //Add count
                    GXCommon.SetObjectCount(cnt, data);
                    if (cnt != 0)
                    {
                        foreach (var it in ListeningWindow)
                        {
                            data.SetUInt8((byte)DataType.Structure);
                            data.SetUInt8(2); //Count
                            //start_time
                            GXCommon.SetData(settings, data, DataType.OctetString, it.Key);
                            //end_time
                            GXCommon.SetData(settings, data, DataType.OctetString, it.Value); 
                        }
                    }
                    return data.Array();
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
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
                    ProfileSelection = GXCommon.ToLogicalName(e.Value);
                    break;
                case 3:
                    PortCommunicationStatus = (MBusPortCommunicationState)Convert.ToInt32(e.Value);
                    break;
                case 4:
                    DataHeaderType = (MBusDataHeaderType)Convert.ToInt32(e.Value);
                    break;
                case 5:
                    PrimaryAddress = Convert.ToByte(e.Value);
                    break;
                case 6:
                    IdentificationNumber = Convert.ToUInt32(e.Value);
                    break;
                case 7:
                    ManufacturerId = Convert.ToUInt16(e.Value);
                    break;
                case 8:
                    MBusVersion = Convert.ToByte(e.Value);
                    break;
                case 9:
                    DeviceType = (MBusDeviceType)Convert.ToByte(e.Value);
                    break;
                case 10:
                    MaxPduSize = Convert.ToUInt16(e.Value);
                    break;
                case 11:
                    ListeningWindow.Clear();
                    if (e.Value != null)
                    {
                        List<object> item;
                        foreach (object tmp in (IEnumerable<object>)e.Value)
                        {
                            if (tmp is List<object>)
                            {
                                item = (List<object>)tmp;
                            }
                            else
                            {
                                item = new List<object>((object[])tmp);
                            }
                            GXDateTime start = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[0], DataType.DateTime, settings.UseUtc2NormalTime);
                            GXDateTime end = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[1], DataType.DateTime, settings.UseUtc2NormalTime);
                            ListeningWindow.Add(new KeyValuePair<GXDateTime, GXDateTime>(start, end));
                        }
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            ProfileSelection = reader.ReadElementContentAsString("ProfileSelection");
            PortCommunicationStatus = (MBusPortCommunicationState)reader.ReadElementContentAsInt("Status");
            DataHeaderType = (MBusDataHeaderType)reader.ReadElementContentAsInt("DataHeaderType");
            PrimaryAddress = (byte)reader.ReadElementContentAsInt("PrimaryAddress");
            IdentificationNumber = (UInt32)reader.ReadElementContentAsInt("IdentificationNumber");
            ManufacturerId = (UInt16)reader.ReadElementContentAsInt("ManufacturerId");
            MBusVersion = (byte)reader.ReadElementContentAsInt("Version");
            DeviceType = (MBusDeviceType)reader.ReadElementContentAsInt("DeviceType");
            MaxPduSize = (UInt16)reader.ReadElementContentAsInt("MaxPduSize");
            ListeningWindow.Clear();
            if (reader.IsStartElement("ListeningWindow", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDateTime start = new GXDateTime(reader.ReadElementContentAsString("Start"), CultureInfo.InvariantCulture);
                    GXDateTime end = new GXDateTime(reader.ReadElementContentAsString("End"), CultureInfo.InvariantCulture);
                    ListeningWindow.Add(new KeyValuePair<DLMS.GXDateTime, DLMS.GXDateTime>(start, end));
                }
                reader.ReadEndElement("ListeningWindow");
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("ProfileSelection", ProfileSelection, 2);
            writer.WriteElementString("Status", (int)PortCommunicationStatus, 3);
            writer.WriteElementString("DataHeaderType", (int)DataHeaderType, 4);
            writer.WriteElementString("PrimaryAddress", PrimaryAddress, 5);
            writer.WriteElementString("IdentificationNumber", IdentificationNumber, 6);

            writer.WriteElementString("ManufacturerId", ManufacturerId, 7);
            writer.WriteElementString("Version", MBusVersion, 8);
            writer.WriteElementString("DeviceType", (int)DeviceType, 9);
            writer.WriteElementString("MaxPduSize", MaxPduSize, 10);
            writer.WriteStartElement("ListeningWindow", 11);
            if (ListeningWindow != null)
            {
                foreach (KeyValuePair<GXDateTime, GXDateTime> it in ListeningWindow)
                {
                    writer.WriteStartElement("Item", 11);
                    //Some meters are returning time here, not date-time.
                    writer.WriteElementString("Start", new GXDateTime(it.Key), 11);
                    writer.WriteElementString("End", new GXDateTime(it.Value), 11);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();

        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
