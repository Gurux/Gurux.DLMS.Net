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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSPrimeNbOfdmPlcPhysicalLayerCounters
    /// </summary>
    public class GXDLMSPrimeNbOfdmPlcPhysicalLayerCounters : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSPrimeNbOfdmPlcPhysicalLayerCounters()
        : this("0.0.28.1.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcPhysicalLayerCounters(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcPhysicalLayerCounters(string ln, ushort sn)
        : base(ObjectType.PrimeNbOfdmPlcPhysicalLayerCounters, ln, sn)
        {
        }

        /// <summary>
        ///  Number of bursts received on the physical layer for which the CRC was incorrect.
        /// </summary>
        [XmlIgnore()]
        public UInt16 CrcIncorrectCount
        {
            get;
            set;
        }

        /// <summary>
        /// Number of bursts received on the physical layer for which the CRC was correct,
        /// but the Protocol field of PHY header had invalid value.
        /// </summary>
        [XmlIgnore()]
        public UInt16 CrcFailedCount
        {
            get;
            set;
        }

        /// <summary>
        /// Number of times when PHY layer received new data to transmit.
        /// </summary>
        [XmlIgnore()]
        public UInt16 TxDropCount
        {
            get;
            set;
        }

        /// <summary>
        /// Number of times when the PHY layer received new data on the channel
        /// </summary>
        [XmlIgnore()]
        public UInt16 RxDropCount
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, CrcIncorrectCount, CrcFailedCount, TxDropCount, RxDropCount };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                CrcIncorrectCount = CrcFailedCount = TxDropCount = RxDropCount = 0;
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
            //CrcIncorrectCount
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //CrcFailedCount
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //TxDropCount
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //RxDropCount
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "CrcIncorrectCount", "CrcFailedCount", "TxDropCount", "RxDropCount" };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Reset" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
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
                    return DataType.UInt16;
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
                    return CrcIncorrectCount;
                case 3:
                    return CrcFailedCount;
                case 4:
                    return TxDropCount;
                case 5:
                    return RxDropCount;
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
                    CrcIncorrectCount = Convert.ToUInt16(e.Value);
                    break;
                case 3:
                    CrcFailedCount = Convert.ToUInt16(e.Value);
                    break;
                case 4:
                    TxDropCount = Convert.ToUInt16(e.Value);
                    break;
                case 5:
                    RxDropCount = Convert.ToUInt16(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            CrcIncorrectCount = (UInt16)reader.ReadElementContentAsInt("CrcIncorrectCount");
            CrcFailedCount = (UInt16)reader.ReadElementContentAsInt("CrcFailedCount");
            TxDropCount = (UInt16)reader.ReadElementContentAsInt("TxDropCount");
            RxDropCount = (UInt16)reader.ReadElementContentAsInt("RxDropCount");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("CrcIncorrectCount", CrcIncorrectCount, 2);
            writer.WriteElementString("CrcFailedCount", CrcFailedCount, 3);
            writer.WriteElementString("TxDropCount", TxDropCount, 4);
            writer.WriteElementString("RxDropCount", RxDropCount, 5);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
