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
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{

    /// <summary>
    /// Diagnostic interface class allows to have knowledge about the device status inside the PLC network.
    /// </summary>
    public class GXDLMSIEC14908Diagnostic : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSIEC14908Diagnostic()
        : this("0.0.34.3.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSIEC14908Diagnostic(string ln)
        : base(ObjectType.IEC14908Diagnostic, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIEC14908Diagnostic(string ln, ushort sn)
        : base(ObjectType.IEC14908Diagnostic, ln, sn)
        {
        }

        /// <summary>
        /// PLC Signal quality status.
        /// </summary>
        [XmlIgnore()]
        public byte PlcSignalQualityStatus
        {
            get;
            set;
        }
        /// <summary>
        /// Transceiver state.
        /// </summary>
        [XmlIgnore()]
        public byte TransceiverState
        {
            get;
            set;
        }

        /// <summary>
        /// Received message status.
        /// </summary>
        [XmlIgnore()]
        public byte ReceivedMessageStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Number of times the application layer transmitted data to the 14908-1 layer but the receive buffer was not available.
        /// </summary>
        [XmlIgnore()]
        public UInt16 NoReceiveBuffer
        {
            get;
            set;
        }

        /// <summary>
        /// Transmit no data.
        /// </summary>
        [XmlIgnore()]
        public UInt16 TransmitNoData
        {
            get;
            set;
        }

        /// <summary>
        /// Unexpected PLC command count.
        /// </summary>
        [XmlIgnore()]
        public UInt16 UnexpectedPlcCommandCount
        {
            get;
            set;
        }

        /// <summary>
        /// Backlog overflows.
        /// </summary>
        [XmlIgnore()]
        public UInt16 BacklogOverflows
        {
            get;
            set;
        }

        /// <summary>
        /// Late ACK.
        /// </summary>
        [XmlIgnore()]
        public UInt16 LateAck
        {
            get;
            set;
        }

        /// <summary>
        /// Frequency is invalid.
        /// </summary>
        [XmlIgnore()]
        public UInt16 FrequencyInvalid
        {
            get;
            set;
        }

        /// <summary>
        /// PLC test rate.
        /// </summary>
        [XmlIgnore()]
        public UInt16 PlcTestRate
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, PlcSignalQualityStatus, TransceiverState, ReceivedMessageStatus,
                              NoReceiveBuffer, TransmitNoData, UnexpectedPlcCommandCount, BacklogOverflows,
                              LateAck, FrequencyInvalid, PlcTestRate
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
            //PlcSignalQualityStatus
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //TransceiverState
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            attributes.Add(4);
            attributes.Add(5);
            attributes.Add(6);
            attributes.Add(7);
            attributes.Add(8);
            attributes.Add(9);
            attributes.Add(10);
            attributes.Add(11);
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "PlcSignalQualityStatus", "TransceiverState", "ReceivedMessageStatus",
                              "NoReceiveBuffer", "TransmitNoData", "UnexpectedPlcCommandCount", "BacklogOverflows","LateAck", "FrequencyInvalid", "PlcTestRate"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 11;
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
                return DataType.UInt8;
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
            if (index == 10)
            {
                return DataType.UInt16;
            }
            if (index == 11)
            {
                return DataType.UInt8;
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
                return PlcSignalQualityStatus;
            }
            if (e.Index == 3)
            {
                return TransceiverState;
            }
            if (e.Index == 4)
            {
                return ReceivedMessageStatus;
            }
            if (e.Index == 5)
            {
                return NoReceiveBuffer;
            }
            if (e.Index == 6)
            {
                return TransmitNoData;
            }
            if (e.Index == 7)
            {
                return UnexpectedPlcCommandCount;
            }
            if (e.Index == 8)
            {
                return BacklogOverflows;
            }
            if (e.Index == 9)
            {
                return LateAck;
            }
            if (e.Index == 10)
            {
                return FrequencyInvalid;
            }
            if (e.Index == 11)
            {
                return PlcTestRate;
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
                PlcSignalQualityStatus = (byte)e.Value;
            }
            else if (e.Index == 3)
            {
                TransceiverState = (byte)e.Value;
            }
            else if (e.Index == 4)
            {
                ReceivedMessageStatus = (byte)e.Value;
            }
            else if (e.Index == 5)
            {
                NoReceiveBuffer = (UInt16)e.Value;
            }
            else if (e.Index == 6)
            {
                TransmitNoData = (UInt16)e.Value;
            }
            else if (e.Index == 7)
            {
                UnexpectedPlcCommandCount = (UInt16)e.Value;
            }
            else if (e.Index == 8)
            {
                BacklogOverflows = (UInt16)e.Value;
            }
            else if (e.Index == 9)
            {
                LateAck = (UInt16)e.Value;
            }
            else if (e.Index == 10)
            {
                FrequencyInvalid = (UInt16)e.Value;
            }
            else if (e.Index == 11)
            {
                PlcTestRate = (byte)e.Value;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            PlcSignalQualityStatus = (byte)reader.ReadElementContentAsInt("PlcSignalQualityStatus");
            TransceiverState = (byte)reader.ReadElementContentAsInt("TransceiverState");
            ReceivedMessageStatus = (byte)reader.ReadElementContentAsInt("ReceivedMessageStatus");
            NoReceiveBuffer = (UInt16)reader.ReadElementContentAsInt("NoReceiveBuffer");
            TransmitNoData = (UInt16)reader.ReadElementContentAsInt("TransmitNoData");
            UnexpectedPlcCommandCount = (UInt16)reader.ReadElementContentAsInt("UnexpectedPlcCommandCount");
            BacklogOverflows = (UInt16)reader.ReadElementContentAsInt("BacklogOverflows");
            LateAck = (UInt16)reader.ReadElementContentAsInt("LateAck");
            FrequencyInvalid = (UInt16)reader.ReadElementContentAsInt("FrequencyInvalid");
            PlcTestRate = (UInt16)reader.ReadElementContentAsInt("PlcTestRate");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("PlcSignalQualityStatus", PlcSignalQualityStatus);
            writer.WriteElementString("TransceiverState", TransceiverState);
            writer.WriteElementString("ReceivedMessageStatus", ReceivedMessageStatus);
            writer.WriteElementString("NoReceiveBuffer", NoReceiveBuffer);
            writer.WriteElementString("TransmitNoData", TransmitNoData);
            writer.WriteElementString("UnexpectedPlcCommandCount", UnexpectedPlcCommandCount);
            writer.WriteElementString("BacklogOverflows", BacklogOverflows);
            writer.WriteElementString("LateAck", LateAck);
            writer.WriteElementString("FrequencyInvalid", FrequencyInvalid);
            writer.WriteElementString("PlcTestRate", PlcTestRate);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
