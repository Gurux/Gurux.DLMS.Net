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
using System.Text;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// M-Bus data definition element.
    /// </summary>
    public class GXMBusClientData
    {
        /// <summary>
        /// Data information block.
        /// </summary>
        public byte[] DataInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Value information block.
        /// </summary>
        public byte[] ValueInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Data.
        /// </summary>
        public object Data
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Use this class to setup M-Bus slave devices and to exchange data with them.
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSMBusClient
    /// </summary>
    public class GXDLMSMBusClient : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSMBusClient()
        : this(null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSMBusClient(string ln)
        : this(ln, 0)
        {
            CaptureDefinition = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSMBusClient(string ln, ushort sn)
        : base(ObjectType.MBusClient, ln, sn)
        {
            CaptureDefinition = new List<KeyValuePair<string, string>>();
            Version = 1;
        }

        /// <summary>
        /// Provides reference to an M-Bus master port setup object, used to configure
        /// an M-Bus port, each interface allowing to exchange data with one or more
        /// M-Bus slave devices
        /// </summary>
        [XmlIgnore()]
        public string MBusPortReference
        {
            get;
            set;
        }

        [XmlIgnore()]
        public List<KeyValuePair<string, string>> CaptureDefinition
        {
            get;
            set;
        }

        [XmlIgnore()]
        public UInt32 CapturePeriod
        {
            get;
            set;
        }

        [XmlIgnore()]
        public int PrimaryAddress
        {
            get;
            set;
        }

        [XmlIgnore()]
        public UInt32 IdentificationNumber
        {
            get;
            set;
        }

        [XmlIgnore()]
        public UInt16 ManufacturerID
        {
            get;
            set;
        }

        /// <summary>
        /// Carries the Version element of the data header as specified in
        /// EN 13757-3 sub-clause 5.6.
        /// </summary>
        [XmlIgnore()]
        public int DataHeaderVersion
        {
            get;
            set;
        }

        [XmlIgnore()]
        public int DeviceType
        {
            get;
            set;
        }

        [XmlIgnore()]
        public int AccessNumber
        {
            get;
            set;
        }

        [XmlIgnore()]
        public int Status
        {
            get;
            set;
        }

        [XmlIgnore()]
        public int Alarm
        {
            get;
            set;
        }

        [XmlIgnore()]
        public UInt16 Configuration
        {
            get;
            set;
        }

        [XmlIgnore()]
        public MBusEncryptionKeyStatus EncryptionKeyStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Installs a slave device.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <param name="primaryAddress">Primary address.</param>
        /// <returns>Generated DLMS data.</returns>
        public byte[][] SlaveInstall(GXDLMSClient client, byte primaryAddress)
        {
            return client.Method(this, 1, primaryAddress, DataType.Int8);
        }

        /// <summary>
        /// De-installs a slave device.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <returns>Generated DLMS data.</returns>
        public byte[][] SlaveDeInstall(GXDLMSClient client)
        {
            return client.Method(this, 2, 0, DataType.Int8);
        }

        /// <summary>
        /// Captures values.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <returns>Generated DLMS data.</returns>
        public byte[][] Capture(GXDLMSClient client)
        {
            return client.Method(this, 3, 0, DataType.Int8);
        }

        /// <summary>
        /// Resets alarm state of the M-Bus slave device.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <returns>Generated DLMS data.</returns>
        public byte[][] ResetAlarm(GXDLMSClient client)
        {
            return client.Method(this, 4, 0, DataType.Int8);
        }

        /// <summary>
        /// Synchronize the clock.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <returns>Generated DLMS data.</returns>
        public byte[][] SynchronizeClock(GXDLMSClient client)
        {
            return client.Method(this, 5, 0, DataType.Int8);
        }

        /// <summary>
        /// Sends data to the M-Bus slave device.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <param name="data">data to send</param>
        /// <returns>Generated DLMS data.</returns>
        public byte[][] SendData(GXDLMSClient client, GXMBusClientData[] data)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Array);
            GXCommon.SetObjectCount(data.Length, bb);
            foreach (GXMBusClientData it in data)
            {
                bb.SetUInt8(DataType.Structure);
                bb.SetUInt8(3);
                bb.SetUInt8(DataType.OctetString);
                GXCommon.SetObjectCount(it.DataInformation.Length, bb);
                bb.Set(it.DataInformation);
                bb.SetUInt8(DataType.OctetString);
                GXCommon.SetObjectCount(it.ValueInformation.Length, bb);
                bb.Set(it.ValueInformation);
                GXCommon.SetData(client.Settings, bb, GXDLMSConverter.GetDLMSDataType(it.Data), it.Data);
            }
            return client.Method(this, 6, bb.Array(), DataType.Array);
        }

        /// <summary>
        /// Sets the encryption key in the M-Bus client and enables encrypted communication
        /// with the M-Bus slave device.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <param name="encryptionKey">encryption key</param>
        /// <returns>Generated DLMS data.</returns>
        public byte[][] SetEncryptionKey(GXDLMSClient client, byte[] encryptionKey)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.OctetString);
            if (encryptionKey == null)
            {
                bb.SetUInt8(0);
            }
            else
            {
                GXCommon.SetObjectCount(encryptionKey.Length, bb);
                bb.Set(encryptionKey);
            }
            return client.Method(this, 7, bb.Array(), DataType.Array);
        }

        /// <summary>
        /// Transfers an encryption key to the M-Bus slave device.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <param name="encryptionKey">encryption key</param>
        /// <returns>Generated DLMS data.</returns>
        public byte[][] TransferKey(GXDLMSClient client, byte[] encryptionKey)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.OctetString);
            if (encryptionKey == null)
            {
                bb.SetUInt8(0);
            }
            else
            {
                GXCommon.SetObjectCount(encryptionKey.Length, bb);
                bb.Set(encryptionKey);
            }
            return client.Method(this, 8, bb.Array(), DataType.Array);
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            if (Version == 0)
            {
                return new object[] { LogicalName, MBusPortReference, CaptureDefinition, CapturePeriod,
                              PrimaryAddress, IdentificationNumber, ManufacturerID, DataHeaderVersion, DeviceType, AccessNumber,
                              Status, Alarm
                            };
            }
            return new object[] { LogicalName, MBusPortReference, CaptureDefinition, CapturePeriod,
                              PrimaryAddress, IdentificationNumber, ManufacturerID, DataHeaderVersion, DeviceType, AccessNumber,
                              Status, Alarm, Configuration, EncryptionKeyStatus
                            };
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
            //MBusPortReference
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //CaptureDefinition
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //CapturePeriod
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //PrimaryAddress
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //IdentificationNumber
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //ManufacturerID
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //Version
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //DeviceType
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //AccessNumber
            if (all || CanRead(10))
            {
                attributes.Add(10);
            }
            //Status
            if (all || CanRead(11))
            {
                attributes.Add(11);
            }
            //Alarm
            if (all || CanRead(12))
            {
                attributes.Add(12);
            }
            if (Version > 0)
            {
                //Configuration
                if (all || CanRead(13))
                {
                    attributes.Add(13);
                }
                //EncryptionKeyStatus
                if (all || CanRead(14))
                {
                    attributes.Add(14);
                }
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            if (Version == 0)
            {
                return new string[] {Internal.GXCommon.GetLogicalNameString(), "MBus Port Reference",
                                 "Capture Definition", "Capture Period", "Primary Address", "Identification Number",
                                 "Manufacturer ID", "Version", "Device Type", "Access Number", "Status", "Alarm"
                                };
            }
            return new string[] {Internal.GXCommon.GetLogicalNameString(), "MBus Port Reference",
                             "Capture Definition", "Capture Period", "Primary Address", "Identification Number",
                             "Manufacturer ID", "Version", "Device Type", "Access Number", "Status", "Alarm",
                             "Configuration", "Encryption Key Status"
                            };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Slave install", "Slave deinstall", "Capture",
                "Reset alarm", "Synchronize clock", "Data send", "Set encryption key", "Transfer key"};
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 1;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            if (Version == 0)
            {
                return 12;
            }
            return 14;

        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 8;
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
                return DataType.OctetString;
            }
            if (index == 3)
            {
                return DataType.Array;
            }
            if (index == 4)
            {
                return DataType.UInt32;
            }
            if (index == 5)
            {
                return DataType.UInt8;
            }
            if (index == 6)
            {
                return DataType.UInt32;
            }
            if (index == 7)
            {
                return DataType.UInt16;
            }
            if (index == 8)
            {
                return DataType.UInt8;
            }
            if (index == 9)
            {
                return DataType.UInt8;
            }
            if (index == 10)
            {
                return DataType.UInt8;
            }
            if (index == 11)
            {
                return DataType.UInt8;
            }
            if (index == 12)
            {
                return DataType.UInt8;
            }
            if (Version > 0)
            {
                if (index == 13)
                {
                    return DataType.UInt16;
                }
                if (index == 14)
                {
                    return DataType.Enum;
                }
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
                return GXCommon.LogicalNameToBytes(MBusPortReference);
            }
            if (e.Index == 3)
            {
                GXByteBuffer buff = new GXByteBuffer();
                buff.SetUInt8((byte)DataType.Array);
                GXCommon.SetObjectCount(CaptureDefinition.Count, buff);
                foreach (KeyValuePair<string, string> it in CaptureDefinition)
                {
                    buff.SetUInt8((byte)DataType.Structure);
                    buff.SetUInt8(2);
                    GXCommon.SetData(settings, buff, DataType.UInt8, it.Key);
                    GXCommon.SetData(settings, buff, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Value));
                }
                return buff.Array();
            }
            if (e.Index == 4)
            {
                return CapturePeriod;
            }
            if (e.Index == 5)
            {
                return PrimaryAddress;
            }
            if (e.Index == 6)
            {
                return IdentificationNumber;
            }
            if (e.Index == 7)
            {
                return ManufacturerID;
            }
            if (e.Index == 8)
            {
                return DataHeaderVersion;
            }
            if (e.Index == 9)
            {
                return DeviceType;
            }
            if (e.Index == 10)
            {
                return AccessNumber;
            }
            if (e.Index == 11)
            {
                return Status;
            }
            if (e.Index == 12)
            {
                return Alarm;
            }
            if (Version > 0)
            {
                if (e.Index == 13)
                {
                    return Configuration;
                }
                if (e.Index == 14)
                {
                    return EncryptionKeyStatus;
                }
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
                MBusPortReference = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 3)
            {
                CaptureDefinition.Clear();
                if (e.Value != null)
                {
                    foreach (object tmp in (IEnumerable<object>)e.Value)
                    {
                        List<object> it;
                        if (tmp is List<object>)
                        {
                            it = (List<object>)tmp;
                        }
                        else
                        {
                            it = new List<object>((object[])tmp);
                        }
                        CaptureDefinition.Add(new KeyValuePair<string, string>(GXDLMSClient.ChangeType((byte[])it[0], DataType.OctetString, settings.UseUtc2NormalTime).ToString(),
                                              GXDLMSClient.ChangeType((byte[])it[1], DataType.OctetString, settings.UseUtc2NormalTime).ToString()));
                    }
                }
            }
            else if (e.Index == 4)
            {
                CapturePeriod = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 5)
            {
                PrimaryAddress = Convert.ToByte(e.Value);
            }
            else if (e.Index == 6)
            {
                IdentificationNumber = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 7)
            {
                ManufacturerID = Convert.ToUInt16(e.Value);
            }
            else if (e.Index == 8)
            {
                DataHeaderVersion = Convert.ToByte(e.Value);
            }
            else if (e.Index == 9)
            {
                DeviceType = Convert.ToByte(e.Value);
            }
            else if (e.Index == 10)
            {
                AccessNumber = Convert.ToByte(e.Value);
            }
            else if (e.Index == 11)
            {
                Status = Convert.ToByte(e.Value);
            }
            else if (e.Index == 12)
            {
                Alarm = Convert.ToByte(e.Value);
            }
            else if (Version > 0)
            {
                if (e.Index == 13)
                {
                    Configuration = Convert.ToUInt16(e.Value);
                }
                else if (e.Index == 14)
                {
                    EncryptionKeyStatus = (MBusEncryptionKeyStatus)Convert.ToInt32(e.Value);
                }
                else
                {
                    e.Error = ErrorCode.ReadWriteDenied;
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            MBusPortReference = reader.ReadElementContentAsString("MBusPortReference");
            CaptureDefinition.Clear();
            if (reader.IsStartElement("CaptureDefinition", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    string d = reader.ReadElementContentAsString("Data");
                    string v = reader.ReadElementContentAsString("Value");
                    CaptureDefinition.Add(new KeyValuePair<string, string>(d, v));
                }
                reader.ReadEndElement("CaptureDefinition");
            }
            CapturePeriod = (UInt16)reader.ReadElementContentAsInt("CapturePeriod");
            PrimaryAddress = reader.ReadElementContentAsInt("PrimaryAddress");
            IdentificationNumber = (UInt16)reader.ReadElementContentAsInt("IdentificationNumber");
            ManufacturerID = (UInt16)reader.ReadElementContentAsInt("ManufacturerID");
            DataHeaderVersion = reader.ReadElementContentAsInt("DataHeaderVersion");
            DeviceType = reader.ReadElementContentAsInt("DeviceType");
            AccessNumber = reader.ReadElementContentAsInt("AccessNumber");

            Status = reader.ReadElementContentAsInt("Status");
            Alarm = reader.ReadElementContentAsInt("Alarm");
            if (Version > 0)
            {
                Configuration = (UInt16)reader.ReadElementContentAsInt("Configuration");
                EncryptionKeyStatus = (MBusEncryptionKeyStatus)reader.ReadElementContentAsInt("EncryptionKeyStatus");
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("MBusPortReference", MBusPortReference, 2);
            writer.WriteStartElement("CaptureDefinition", 3);
            if (CaptureDefinition != null)
            {
                foreach (KeyValuePair<string, string> it in CaptureDefinition)
                {
                    writer.WriteStartElement("Item", 3);
                    writer.WriteElementString("Data", it.Key, 3);
                    writer.WriteElementString("Value", it.Value, 3);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteElementString("CapturePeriod", CapturePeriod, 4);
            writer.WriteElementString("PrimaryAddress", PrimaryAddress, 5);
            writer.WriteElementString("IdentificationNumber", IdentificationNumber, 6);
            writer.WriteElementString("ManufacturerID", ManufacturerID, 7);
            writer.WriteElementString("DataHeaderVersion", DataHeaderVersion, 8);
            writer.WriteElementString("DeviceType", DeviceType, 9);
            writer.WriteElementString("AccessNumber", AccessNumber, 10);
            writer.WriteElementString("Status", Status, 11);
            writer.WriteElementString("Alarm", Alarm, 12);
            if (Version > 0)
            {
                writer.WriteElementString("Configuration", Configuration, 13);
                writer.WriteElementString("EncryptionKeyStatus", (int)EncryptionKeyStatus, 14);
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
