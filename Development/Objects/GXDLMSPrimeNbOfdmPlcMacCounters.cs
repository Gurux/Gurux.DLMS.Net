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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSPrimeNbOfdmPlcMacCounters
    /// </summary>
    public class GXDLMSPrimeNbOfdmPlcMacCounters : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSPrimeNbOfdmPlcMacCounters()
        : this("0.0.28.4.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcMacCounters(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcMacCounters(string ln, ushort sn)
        : base(ObjectType.PrimeNbOfdmPlcMacCounters, ln, sn)
        {
        }

        /// <summary>
        /// Count of successfully transmitted MSDUs.
        /// </summary>
        [XmlIgnore()]
        public UInt32 TxDataPktCount
        {
            get;
            set;
        }

        /// <summary>
        /// Count of successfully received MSDUs whose destination address was this node.
        /// </summary>
        [XmlIgnore()]
        public UInt32 RxDataPktCount
        {
            get;
            set;
        }

        /// <summary>
        /// Count of successfully transmitted MAC control packets.
        /// </summary>
        [XmlIgnore()]
        public UInt32 TxCtrlPktCount
        {
            get;
            set;
        }

        /// <summary>
        /// Count of successfully received MAC control packets whose destination was this node.
        /// </summary>
        [XmlIgnore()]
        public UInt32 RxCtrlPktCount
        {
            get;
            set;
        }

        /// <summary>
        /// Count of failed CSMA transmit attempts.
        /// </summary>
        [XmlIgnore()]
        public UInt32 CsmaFailCount
        {
            get;
            set;
        }

        /// <summary>
        /// Count of number of times this node has to back off SCP transmission due to channel busy state.
        /// </summary>
        [XmlIgnore()]
        public UInt32 CsmaChBusyCount
        {
            get;
            set;
        }

        /// <summary>
        /// Reset all counters.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Reset(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, TxDataPktCount, RxDataPktCount, TxCtrlPktCount, RxCtrlPktCount, CsmaFailCount, CsmaChBusyCount };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                TxDataPktCount = RxDataPktCount = TxCtrlPktCount = RxCtrlPktCount = CsmaFailCount = CsmaChBusyCount = 0;
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
            //TxDataPktCount
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //RxDataPktCount
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //TxCtrlPktCount
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //RxCtrlPktCount
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //CsmaFailCount
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //CsmaChBusyCount
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "TxDataPktCount", "RxDataPktCount", "TxCtrlPktCount",
                "RxCtrlPktCount", "CsmaFailCount", "CsmaChBusyCount" };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Reset" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 7;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    return DataType.UInt32;
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
                    return TxDataPktCount;
                case 3:
                    return RxDataPktCount;
                case 4:
                    return TxCtrlPktCount;
                case 5:
                    return RxCtrlPktCount;
                case 6:
                    return CsmaFailCount;
                case 7:
                    return CsmaChBusyCount;
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
                    TxDataPktCount = Convert.ToUInt32(e.Value);
                    break;
                case 3:
                    RxDataPktCount = Convert.ToUInt32(e.Value);
                    break;
                case 4:
                    TxCtrlPktCount = Convert.ToUInt32(e.Value);
                    break;
                case 5:
                    RxCtrlPktCount = Convert.ToUInt32(e.Value);
                    break;
                case 6:
                    CsmaFailCount = Convert.ToUInt32(e.Value);
                    break;
                case 7:
                    CsmaChBusyCount = Convert.ToUInt32(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            TxDataPktCount = (UInt32)reader.ReadElementContentAsLong("TxDataPktCount");
            RxDataPktCount = (UInt32)reader.ReadElementContentAsLong("RxDataPktCount");
            TxCtrlPktCount = (UInt32)reader.ReadElementContentAsLong("TxCtrlPktCount");
            RxCtrlPktCount = (UInt32)reader.ReadElementContentAsLong("RxCtrlPktCount");
            CsmaFailCount = (UInt32)reader.ReadElementContentAsLong("CsmaFailCount");
            CsmaChBusyCount = (UInt32)reader.ReadElementContentAsLong("CsmaChBusyCount");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("TxDataPktCount", TxDataPktCount, 2);
            writer.WriteElementString("RxDataPktCount", RxDataPktCount, 3);
            writer.WriteElementString("TxCtrlPktCount", TxCtrlPktCount, 4);
            writer.WriteElementString("RxCtrlPktCount", RxCtrlPktCount, 5);
            writer.WriteElementString("CsmaFailCount", CsmaFailCount, 6);
            writer.WriteElementString("CsmaChBusyCount", CsmaChBusyCount, 7);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
