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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    public class AdjacentCell
    {
        /// <summary>
        /// Cell ID in hex format.
        /// </summary>
        public UInt32 CellId
        {
            get;
            set;
        }

        /// <summary>
        ///  Signal quality.
        /// </summary>
        public byte SignalQuality
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSGSMDiagnostic
    /// </summary>
    public class GXDLMSGSMDiagnostic : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSGSMDiagnostic()
        : this("0.0.25.6.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSGSMDiagnostic(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSGSMDiagnostic(string ln, ushort sn)
        : base(ObjectType.GSMDiagnostic, ln, sn)
        {
            CellInfo = new GXDLMSGSMCellInfo();
            AdjacentCells = new List<AdjacentCell>();
            Version = 1;
        }

        /// <summary>
        /// Name of network operator.
        /// </summary>
        [XmlIgnore()]
        public string Operator
        {
            get;
            set;
        }

        /// <summary>
        /// Registration status of the modem.
        /// </summary>
        [XmlIgnore()]
        public GsmStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// Registration status of the modem.
        /// </summary>
        [XmlIgnore()]
        public GsmCircuitSwitchStatus CircuitSwitchStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Registration status of the modem.
        /// </summary>
        [XmlIgnore()]
        public GsmPacketSwitchStatus PacketSwitchStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Registration status of the modem.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSGSMCellInfo CellInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Registration status of the modem.
        /// </summary>
        [XmlIgnore()]
        public List<AdjacentCell> AdjacentCells
        {
            get;
            set;
        }

        /// <summary>
        /// Date and time when the data have been last captured.
        /// </summary>
        [XmlIgnore()]
        public GXDateTime CaptureTime
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Operator, Status, CircuitSwitchStatus, PacketSwitchStatus, CellInfo, AdjacentCells, CaptureTime };
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
            //Operator
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //Status
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //CircuitSwitchStatus
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //PacketSwitchStatus
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //CellInfo
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //AdjacentCells
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //CaptureTime
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Operator", "Status", "CircuitSwitchStatus", "PacketSwitchStatus", "CellInfo", "AdjacentCells", "CaptureTime" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 8;
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
                    return DataType.String;
                case 3:
                    return DataType.Enum;
                case 4:
                    return DataType.Enum;
                case 5:
                    return DataType.Enum;
                case 6:
                    return DataType.Structure;
                case 7:
                    return DataType.Array;
                case 8:
                    return DataType.DateTime;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            GXByteBuffer bb;
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    if (string.IsNullOrEmpty(Operator))
                    {
                        return null;
                    }
                    return ASCIIEncoding.ASCII.GetBytes(Operator);
                case 3:
                    return Status;
                case 4:
                    return CircuitSwitchStatus;
                case 5:
                    return PacketSwitchStatus;
                case 6:
                    bb = new GXByteBuffer();
                    bb.SetUInt8(DataType.Structure);
                    if (Version == 0)
                    {
                        bb.SetUInt8(4);
                        GXCommon.SetData(settings, bb, DataType.UInt16, CellInfo.CellId);
                    }
                    else
                    {
                        bb.SetUInt8(7);
                        GXCommon.SetData(settings, bb, DataType.UInt32, CellInfo.CellId);
                    }
                    GXCommon.SetData(settings, bb, DataType.UInt16, CellInfo.LocationId);
                    GXCommon.SetData(settings, bb, DataType.UInt8, CellInfo.SignalQuality);
                    GXCommon.SetData(settings, bb, DataType.UInt8, CellInfo.Ber);
                    if (Version > 0)
                    {
                        GXCommon.SetData(settings, bb, DataType.UInt16, CellInfo.MobileCountryCode);
                        GXCommon.SetData(settings, bb, DataType.UInt16, CellInfo.MobileNetworkCode);
                        GXCommon.SetData(settings, bb, DataType.UInt32, CellInfo.ChannelNumber);
                    }
                    return bb.Array();
                case 7:
                    bb = new GXByteBuffer();
                    bb.SetUInt8(DataType.Array);
                    if (AdjacentCells == null)
                    {
                        bb.SetUInt8(0);
                    }
                    else
                    {
                        bb.SetUInt8((byte)AdjacentCells.Count);
                    }
                    foreach (AdjacentCell it in AdjacentCells)
                    {
                        bb.SetUInt8(DataType.Structure);
                        bb.SetUInt8(2);
                        GXCommon.SetData(settings, bb, Version == 0 ? DataType.UInt16 : DataType.UInt32, it.CellId);
                        GXCommon.SetData(settings, bb, DataType.UInt8, it.SignalQuality);
                    }
                    return bb.Array();
                case 8:
                    return CaptureTime;
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
                    if (e.Value is byte[])
                    {
                        Operator = ASCIIEncoding.ASCII.GetString((byte[])e.Value);
                    }
                    else
                    {
                        Operator = (string)e.Value;
                    }
                    break;
                case 3:
                    Status = (GsmStatus)Convert.ToByte(e.Value);
                    break;
                case 4:
                    CircuitSwitchStatus = (GsmCircuitSwitchStatus)Convert.ToByte(e.Value);
                    break;
                case 5:
                    PacketSwitchStatus = (GsmPacketSwitchStatus)Convert.ToByte(e.Value);
                    break;
                case 6:
                    if (e.Value != null)
                    {
                        List<object> tmp = (List<object>)e.Value;
                        CellInfo.CellId = Convert.ToUInt32(tmp[0]);
                        CellInfo.LocationId = (UInt16)tmp[1];
                        CellInfo.SignalQuality = (byte)tmp[2];
                        CellInfo.Ber = (byte)tmp[3];
                        if (Version > 0)
                        {
                            CellInfo.MobileCountryCode = (UInt16)tmp[4];
                            CellInfo.MobileNetworkCode = (UInt16)tmp[5];
                            CellInfo.ChannelNumber = (UInt32)tmp[6];
                        }
                    }
                    break;
                case 7:
                    AdjacentCells.Clear();
                    if (e.Value != null)
                    {
                        foreach (List<object> it in (List<object>)e.Value)
                        {
                            AdjacentCell ac = new Objects.AdjacentCell();
                            ac.CellId = Convert.ToUInt32(it[0]);
                            ac.SignalQuality = (byte)it[1];
                            AdjacentCells.Add(ac);
                        }
                    }
                    break;
                case 8:
                    if (e.Value is byte[])
                    {
                        e.Value = GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime, settings.UseUtc2NormalTime);
                    }
                    else if (e.Value is string)
                    {
                        e.Value = new GXDateTime((string)e.Value);
                    }
                    CaptureTime = (GXDateTime)e.Value;
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Operator = reader.ReadElementContentAsString("Operator");
            Status = (GsmStatus)reader.ReadElementContentAsInt("Status");
            CircuitSwitchStatus = (GsmCircuitSwitchStatus)reader.ReadElementContentAsInt("CircuitSwitchStatus");
            PacketSwitchStatus = (GsmPacketSwitchStatus)reader.ReadElementContentAsInt("PacketSwitchStatus");
            if (reader.IsStartElement("CellInfo", true))
            {
                CellInfo.CellId = (UInt16)reader.ReadElementContentAsInt("CellId");
                CellInfo.LocationId = (UInt16)reader.ReadElementContentAsInt("LocationId");
                CellInfo.SignalQuality = (byte)reader.ReadElementContentAsInt("SignalQuality");
                CellInfo.Ber = (byte)reader.ReadElementContentAsInt("Ber");
                reader.ReadEndElement("CellInfo");
            }
            AdjacentCells.Clear();
            if (reader.IsStartElement("AdjacentCells", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    AdjacentCell it = new Objects.AdjacentCell();
                    it.CellId = (UInt32) reader.ReadElementContentAsInt("CellId");
                    it.SignalQuality = (byte)reader.ReadElementContentAsInt("SignalQuality");
                    AdjacentCells.Add(it);
                }
                reader.ReadEndElement("AdjacentCells");
            }
            CaptureTime = new GXDateTime(reader.ReadElementContentAsString("CaptureTime"), System.Globalization.CultureInfo.InvariantCulture);
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Operator", Operator);
            writer.WriteElementString("Status", (int)Status);
            writer.WriteElementString("CircuitSwitchStatus", (int)CircuitSwitchStatus);
            writer.WriteElementString("PacketSwitchStatus", (int)PacketSwitchStatus);
            if (CellInfo != null)
            {
                writer.WriteStartElement("CellInfo");
                writer.WriteElementString("CellId", CellInfo.CellId);
                writer.WriteElementString("LocationId", CellInfo.LocationId);
                writer.WriteElementString("SignalQuality", CellInfo.SignalQuality);
                writer.WriteElementString("Ber", CellInfo.Ber);
                writer.WriteEndElement();
            }

            if (AdjacentCells != null)
            {
                writer.WriteStartElement("AdjacentCells");
                foreach (AdjacentCell it in AdjacentCells)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("CellId", it.CellId);
                    writer.WriteElementString("SignalQuality", it.SignalQuality);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteElementObject("CaptureTime", CaptureTime);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
