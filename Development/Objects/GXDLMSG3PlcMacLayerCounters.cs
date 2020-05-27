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
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSG3PlcMacLayerCounters
    /// </summary>
    public class GXDLMSG3PlcMacLayerCounters : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSG3PlcMacLayerCounters()
        : this("0.0.29.0.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSG3PlcMacLayerCounters(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSG3PlcMacLayerCounters(string ln, ushort sn)
        : base(ObjectType.G3PlcMacLayerCounters, ln, sn)
        {
        }

        /// <summary>
        /// Statistic counter of successfully transmitted data packets.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x0101.
        /// </remarks>
        [XmlIgnore()]
        public UInt32 TxDataPacketCount
        {
            get;
            set;
        }

        /// <summary>
        ///Statistic counter of successfully received data packets.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x0102.
        /// </remarks>
        [XmlIgnore()]
        public UInt32 RxDataPacketCount
        {
            get;
            set;
        }

        /// <summary>
        /// Statistic counter of successfully transmitted command packets.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x0103.
        /// </remarks>
        [XmlIgnore()]
        public UInt32 TxCmdPacketCount
        {
            get;
            set;
        }

        /// <summary>
        /// Statistic counter of successfully received command packets.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x0104.
        /// </remarks>
        [XmlIgnore()]
        public UInt32 RxCmdPacketCount

        {
            get;
            set;
        }

        /// <summary>
        /// Counts the number of times when CSMA backoffs reach macMaxCSMABackoffs.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x0105.
        /// </remarks>
        [XmlIgnore()]
        public UInt32 CSMAFailCount
        {
            get;
            set;
        }

        /// <summary>
        /// Counts the number of times when an ACK is not received while transmitting a unicast data frame.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x0106.
        /// </remarks>
        [XmlIgnore()]
        public UInt32 CSMANoAckCount
        {
            get;
            set;
        }

        /// <summary>
        /// Statistic counter of the number of frames received with bad CRC.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x0109.
        /// </remarks>
        [XmlIgnore()]
        public UInt32 BadCrcCount
        {
            get;
            set;
        }

        /// <summary>
        /// Statistic counter of the number of broadcast frames sent.
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x0108.
        /// </remarks>
        [XmlIgnore()]
        public UInt32 TxDataBroadcastCount
        {
            get;
            set;
        }

        /// <summary>
        /// Statistic counter of successfully received broadcast packets
        /// </summary>
        /// <remarks>
        /// PIB attribute: 0x0107.
        /// </remarks>
        [XmlIgnore()]
        public UInt32 RxDataBroadcastCount
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, TxDataPacketCount, RxDataPacketCount, TxCmdPacketCount ,
           RxCmdPacketCount,  CSMAFailCount, CSMANoAckCount, BadCrcCount,TxDataBroadcastCount,  RxDataBroadcastCount};
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                TxDataPacketCount = RxDataPacketCount = TxCmdPacketCount = RxCmdPacketCount = CSMAFailCount = CSMANoAckCount = BadCrcCount = TxDataBroadcastCount = RxDataBroadcastCount = 0;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
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
            //TxDataPacketCount
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //RxDataPacketCount
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //TxCmdPacketCount
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //RxCmdPacketCount
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //CSMAFailCount
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //CSMANoAckCount
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //BadCrcCount
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //TxDataBroadcastCount
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //RxDataBroadcastCount
            if (all || CanRead(10))
            {
                attributes.Add(10);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "TxDataPacketCount", "RxDataPacketCount", "TxCmdPacketCount",
           "RxCmdPacketCount", " CSMAFailCount", "CSMANoAckCount", "BadCrcCount", "TxDataBroadcastCount", " RxDataBroadcastCount" };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Reset" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 10;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
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
                return DataType.UInt32;
            }
            if (index == 3)
            {
                return DataType.UInt32;
            }
            if (index == 4)
            {
                return DataType.UInt32;
            }
            if (index == 5)
            {
                return DataType.UInt32;
            }
            if (index == 6)
            {
                return DataType.UInt32;
            }
            if (index == 7)
            {
                return DataType.UInt32;
            }
            if (index == 8)
            {
                return DataType.UInt32;
            }
            if (index == 9)
            {
                return DataType.UInt32;
            }
            if (index == 10)
            {
                return DataType.UInt32;
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
                return TxDataPacketCount;
            }
            if (e.Index == 3)
            {
                return RxDataPacketCount;
            }
            if (e.Index == 4)
            {
                return TxCmdPacketCount;
            }
            if (e.Index == 5)
            {
                return RxCmdPacketCount;
            }
            if (e.Index == 6)
            {
                return CSMAFailCount;
            }
            if (e.Index == 7)
            {
                return CSMANoAckCount;
            }
            if (e.Index == 8)
            {
                return BadCrcCount;
            }
            if (e.Index == 9)
            {
                return TxDataBroadcastCount;
            }
            if (e.Index == 10)
            {
                return RxDataBroadcastCount;
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
                TxDataPacketCount = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 3)
            {
                RxDataPacketCount = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 4)
            {
                TxCmdPacketCount = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 5)
            {
                RxCmdPacketCount = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 6)
            {
                CSMAFailCount = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 7)
            {
                CSMANoAckCount = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 8)
            {
                BadCrcCount = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 9)
            {
                TxDataBroadcastCount = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 10)
            {
                RxDataBroadcastCount = Convert.ToUInt32(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            TxDataPacketCount = (UInt16)reader.ReadElementContentAsInt("TxDataPacketCount");
            RxDataPacketCount = (UInt16)reader.ReadElementContentAsInt("RxDataPacketCount");
            TxCmdPacketCount = (UInt16)reader.ReadElementContentAsInt("TxCmdPacketCount");
            RxCmdPacketCount = (UInt16)reader.ReadElementContentAsInt("RxCmdPacketCount");
            CSMAFailCount = (UInt16)reader.ReadElementContentAsInt("CSMAFailCount");
            CSMANoAckCount = (UInt16)reader.ReadElementContentAsInt("CSMANoAckCount");
            BadCrcCount = (UInt16)reader.ReadElementContentAsInt("BadCrcCount");
            TxDataBroadcastCount = (UInt16)reader.ReadElementContentAsInt("TxDataBroadcastCount");
            RxDataBroadcastCount = (UInt16)reader.ReadElementContentAsInt("RxDataBroadcastCount");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("TxDataPacketCount", TxDataPacketCount, 2);
            writer.WriteElementString("RxDataPacketCount", RxDataPacketCount, 3);
            writer.WriteElementString("TxCmdPacketCount", TxCmdPacketCount, 4);
            writer.WriteElementString("RxCmdPacketCount", RxCmdPacketCount, 5);
            writer.WriteElementString("CSMAFailCount", CSMAFailCount, 6);
            writer.WriteElementString("CSMANoAckCount", CSMANoAckCount, 7);
            writer.WriteElementString("BadCrcCount", BadCrcCount, 8);
            writer.WriteElementString("TxDataBroadcastCount", TxDataBroadcastCount, 9);
            writer.WriteElementString("RxDataBroadcastCount", RxDataBroadcastCount, 10);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
