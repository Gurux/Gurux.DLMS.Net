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
using Gurux.DLMS.Enums;
using System.Xml;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    public class AdjacentCell
    {
        /// <summary>
        /// Two byte cell ID.
        /// </summary>
        public string CellId
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

    public class GXDLMSGSMDiagnostic : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSGSMDiagnostic()
        : this(null, 0)
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

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //Operator
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //Status
            if (CanRead(3))
            {
                attributes.Add(3);
            }
            //CircuitSwitchStatus
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            //PacketSwitchStatus
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            //CellInfo
            if (CanRead(6))
            {
                attributes.Add(6);
            }
            //AdjacentCells
            if (CanRead(7))
            {
                attributes.Add(7);
            }
            //CaptureTime
            if (CanRead(8))
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
            if (index == 1)
            {
                return DataType.OctetString;
            }
            // Operator
            if (index == 2)
            {
                return DataType.String;
            }
            //Status
            if (index == 3)
            {
                return DataType.Enum;
            }
            //CircuitSwitchStatus
            if (index == 4)
            {
                return DataType.Enum;
            }
            //PacketSwitchStatus
            if (index == 5)
            {
                return DataType.Enum;
            }
            //CellInfo
            if (index == 6)
            {
                return DataType.Structure;
            }
            //AdjacentCells
            if (index == 7)
            {
                return DataType.Array;
            }
            //CaptureTime
            if (index == 8)
            {
                return DataType.DateTime;
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
                return Operator;
            }
            if (e.Index == 3)
            {
                return Status;
            }
            if (e.Index == 4)
            {
                return CircuitSwitchStatus;
            }
            if (e.Index == 5)
            {
                return PacketSwitchStatus;
            }
            if (e.Index == 6)
            {
                return CellInfo;
            }
            if (e.Index == 7)
            {
                return AdjacentCells;
            }
            if (e.Index == 8)
            {
                return CaptureTime;
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
                Operator = (string)e.Value;
            }
            else if (e.Index == 3)
            {
                Status = (GsmStatus)e.Value;
            }
            else if (e.Index == 4)
            {
                CircuitSwitchStatus = (GsmCircuitSwitchStatus)e.Value;
            }
            else if (e.Index == 5)
            {
                PacketSwitchStatus = (GsmPacketSwitchStatus)e.Value;
            }
            else if (e.Index == 6)
            {
                if (e.Value != null)
                {
                    object[] tmp = (object[])e.Value;
                    CellInfo.CellId = (string)tmp[0];
                    CellInfo.LocationId = (UInt16)tmp[1];
                    CellInfo.SignalQuality = (byte)tmp[2];
                    CellInfo.Ber = (byte)tmp[3];
                }
            }
            else if (e.Index == 7)
            {
                AdjacentCells.Clear();
                if (e.Value != null)
                {
                    foreach (object it in (object[])e.Value)
                    {
                        object[] tmp = (object[])it;
                        AdjacentCell ac = new Objects.AdjacentCell();
                        ac.CellId = (string)tmp[0];
                        ac.SignalQuality = (byte)tmp[1];
                        AdjacentCells.Add(ac);
                    }
                }
            }
            else if (e.Index == 8)
            {
                CaptureTime = (GXDateTime)e.Value;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
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
                CellInfo.CellId = reader.ReadElementContentAsString("CellId");
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
                    it.CellId = reader.ReadElementContentAsString("CellId");
                    it.SignalQuality = (byte)reader.ReadElementContentAsInt("SignalQuality");
                    AdjacentCells.Add(it);
                }
                reader.ReadEndElement("AdjacentCells");
            }
            CaptureTime = new GXDateTime(reader.ReadElementContentAsString("CaptureTime"));
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementObject("Operator", Operator);
            writer.WriteElementString("Status", (int) Status);
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
