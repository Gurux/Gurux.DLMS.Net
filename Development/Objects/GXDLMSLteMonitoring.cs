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
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSLteMonitoring
    /// </summary>
    public class GXDLMSLteMonitoring : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSLteMonitoring()
        : this("0.0.25.11.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSLteMonitoring(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSLteMonitoring(string ln, ushort sn)
        : base(ObjectType.LteMonitoring, ln, sn)
        {
            Version = 1;
            NetworkParameters = new GXLteNetworkParameters();
            QualityOfService = new GXLteQualityOfService();
        }

        /// <summary>
        /// Network parameters for the LTE network.
        /// </summary>
        [XmlIgnore()]
        public GXLteNetworkParameters NetworkParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Quality of service of the LTE network.
        /// </summary>
        [XmlIgnore()]
        public GXLteQualityOfService QualityOfService
        {
            get;
            set;
        }


        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName,
            NetworkParameters,
            QualityOfService};
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
            //NetworkParameters
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            if (Version > 0)
            {
                //QualityOfService
                if (all || CanRead(3))
                {
                    attributes.Add(3);
                }
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {
                Internal.GXCommon.GetLogicalNameString(),
                "Network parameters",
                "Quality of service"};
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
            if (Version == 0)
            {
                return 2;
            }
            return 3;
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
                case 3:
                    return DataType.Structure;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            object ret = null;
            GXByteBuffer buff = new GXByteBuffer();
            switch (e.Index)
            {
                case 1:
                    ret = GXCommon.LogicalNameToBytes(LogicalName);
                    break;
                case 2:
                    buff.SetUInt8(DataType.Structure);
                    GXCommon.SetObjectCount(9, buff);
                    GXCommon.SetData(settings, buff, DataType.UInt16, NetworkParameters.T3402);
                    GXCommon.SetData(settings, buff, DataType.UInt16, NetworkParameters.T3412);
                    GXCommon.SetData(settings, buff, DataType.UInt32, NetworkParameters.T3412ext2);
                    GXCommon.SetData(settings, buff, DataType.UInt16, NetworkParameters.T3324);
                    GXCommon.SetData(settings, buff, DataType.UInt32, NetworkParameters.TeDRX);
                    GXCommon.SetData(settings, buff, DataType.UInt16, NetworkParameters.TPTW);
                    GXCommon.SetData(settings, buff, DataType.Int8, NetworkParameters.QRxlevMin);
                    GXCommon.SetData(settings, buff, DataType.Int8, NetworkParameters.QRxlevMinCE);
                    GXCommon.SetData(settings, buff, DataType.Int8, NetworkParameters.QRxLevMinCE1);
                    ret = buff.Array();
                    break;
                case 3:
                    if (Version == 0)
                    {
                        e.Error = ErrorCode.ReadWriteDenied;
                    }
                    else
                    {
                        buff.SetUInt8(DataType.Structure);
                        GXCommon.SetObjectCount(4, buff);
                        GXCommon.SetData(settings, buff, DataType.Int8, QualityOfService.SignalQuality);
                        GXCommon.SetData(settings, buff, DataType.Int8, QualityOfService.SignalLevel);
                        GXCommon.SetData(settings, buff, DataType.Int8, QualityOfService.SignalToNoiseRatio);
                        GXCommon.SetData(settings, buff, DataType.Enum, QualityOfService.CoverageEnhancement);
                        ret = buff.Array();
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return ret;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    {
                        GXStructure s = e.Value as GXStructure;
                        NetworkParameters.T3402 = (UInt16)s[0];
                        NetworkParameters.T3412 = (UInt16)s[1];
                        NetworkParameters.T3412ext2 = (UInt32)s[2];
                        NetworkParameters.T3324 = (UInt16)s[3];
                        NetworkParameters.TeDRX = (UInt32)s[4];
                        NetworkParameters.TPTW = (UInt16)s[5];
                        NetworkParameters.QRxlevMin = (sbyte)s[6];
                        NetworkParameters.QRxlevMinCE = (sbyte)s[7];
                        NetworkParameters.QRxLevMinCE1 = (sbyte)s[8];
                    }
                    break;
                case 3:
                    if (Version == 0)
                    {
                        e.Error = ErrorCode.ReadWriteDenied;
                    }
                    else
                    {
                        GXStructure s = e.Value as GXStructure;
                        QualityOfService.SignalQuality = (sbyte)s[0];
                        QualityOfService.SignalLevel = (sbyte)s[1];
                        QualityOfService.SignalToNoiseRatio = (sbyte)s[2];
                        QualityOfService.CoverageEnhancement = (LteCoverageEnhancement)Convert.ToByte(s[3]);
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            NetworkParameters.T3402 = (UInt16)reader.ReadElementContentAsInt("T3402");
            NetworkParameters.T3412 = (UInt16)reader.ReadElementContentAsInt("T3412");
            NetworkParameters.T3412ext2 = (UInt32)reader.ReadElementContentAsInt("T3412ext2");
            NetworkParameters.T3324 = (UInt16)reader.ReadElementContentAsInt("T3324");
            NetworkParameters.TeDRX = (UInt32)reader.ReadElementContentAsInt("TeDRX");
            NetworkParameters.TPTW = (UInt16)reader.ReadElementContentAsInt("TPTW");
            NetworkParameters.QRxlevMin = (sbyte)reader.ReadElementContentAsInt("QRxlevMin");
            NetworkParameters.QRxlevMinCE = (sbyte)reader.ReadElementContentAsInt("QRxlevMinCE");
            NetworkParameters.QRxLevMinCE1 = (sbyte)reader.ReadElementContentAsInt("QRxLevMinCE1");
            QualityOfService.SignalQuality = (sbyte)reader.ReadElementContentAsInt("SignalQuality");
            QualityOfService.SignalLevel = (sbyte)reader.ReadElementContentAsInt("SignalLevel");
            QualityOfService.SignalToNoiseRatio = (sbyte)reader.ReadElementContentAsInt("SignalToNoiseRatio");
            QualityOfService.CoverageEnhancement = (LteCoverageEnhancement)reader.ReadElementContentAsInt("CoverageEnhancement");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("T3402", NetworkParameters.T3402, 2);
            writer.WriteElementString("T3412", NetworkParameters.T3412, 2);
            writer.WriteElementString("T3412ext2", NetworkParameters.T3412ext2, 2);
            writer.WriteElementString("T3324", NetworkParameters.T3324, 2);
            writer.WriteElementString("TeDRX", NetworkParameters.TeDRX, 2);
            writer.WriteElementString("TPTW", NetworkParameters.TPTW, 2);
            writer.WriteElementString("QRxlevMin", NetworkParameters.QRxlevMin, 2);
            writer.WriteElementString("QRxlevMinCE", NetworkParameters.QRxlevMinCE, 2);
            writer.WriteElementString("QRxLevMinCE1", NetworkParameters.QRxLevMinCE1, 2);
            writer.WriteElementString("SignalQuality", QualityOfService.SignalQuality, 2);
            writer.WriteElementString("SignalLevel", QualityOfService.SignalLevel, 2);
            writer.WriteElementString("SignalToNoiseRatio", QualityOfService.SignalToNoiseRatio, 2);
            writer.WriteElementString("CoverageEnhancement", (int)QualityOfService.CoverageEnhancement, 2);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
