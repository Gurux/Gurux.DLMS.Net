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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSPrimeNbOfdmPlcMacSetup
    /// </summary>
    public class GXDLMSPrimeNbOfdmPlcMacSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSPrimeNbOfdmPlcMacSetup()
        : this("0.0.28.2.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcMacSetup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcMacSetup(string ln, ushort sn)
        : base(ObjectType.PrimeNbOfdmPlcMacSetup, ln, sn)
        {
        }

        /// <summary>
        /// PIB attribute 0x0010
        /// </summary>
        [XmlIgnore()]
        public byte MacMinSwitchSearchTime
        {
            get;
            set;
        }

        /// <summary>
        /// PIB attribute 0x0011.
        /// </summary>
        [XmlIgnore()]
        public byte MacMaxPromotionPdu
        {
            get;
            set;
        }

        /// <summary>
        /// PIB attribute 0x0012.
        /// </summary>
        [XmlIgnore()]
        public byte MacPromotionPduTxPeriod
        {
            get;
            set;
        }

        /// <summary>
        /// PIB attribute 0x0013.
        /// </summary>
        [XmlIgnore()]
        public byte MacBeaconsPerFrame
        {
            get;
            set;
        }

        /// <summary>
        /// PIB attribute 0x0014.
        /// </summary>
        [XmlIgnore()]
        public byte MacScpMaxTxAttempts
        {
            get;
            set;
        }

        /// <summary>
        /// PIB attribute 0x0015.
        /// </summary>
        [XmlIgnore()]
        public byte MacCtlReTxTimer
        {
            get;
            set;
        }

        /// <summary>
        /// PIB attribute 0x0018.
        /// </summary>
        [XmlIgnore()]
        public byte MacMaxCtlReTx
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, MacMinSwitchSearchTime, MacMaxPromotionPdu, MacPromotionPduTxPeriod, MacBeaconsPerFrame,
            MacScpMaxTxAttempts, MacCtlReTxTimer, MacMaxCtlReTx};
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
            //MacMinSwitchSearchTime
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //MacMaxPromotionPdu
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //MacPromotionPduTxPeriod
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //MacBeaconsPerFrame
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //MacScpMaxTxAttempts
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //MacCtlReTxTimer
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //MacMaxCtlReTx
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "MacMinSwitchSearchTime", "MacMaxPromotionPdu",
                "MacPromotionPduTxPeriod", "MacBeaconsPerFrame", "MacScpMaxTxAttempts", "MacCtlReTxTimer", "MacMaxCtlReTx" };
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
            return 8;
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
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    return DataType.UInt8;
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
                    return MacMinSwitchSearchTime;
                case 3:
                    return MacMaxPromotionPdu;
                case 4:
                    return MacPromotionPduTxPeriod;
                case 5:
                    return MacBeaconsPerFrame;
                case 6:
                    return MacScpMaxTxAttempts;
                case 7:
                    return MacCtlReTxTimer;
                case 8:
                    return MacMaxCtlReTx;
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
                    MacMinSwitchSearchTime = Convert.ToByte(e.Value);
                    break;
                case 3:
                    MacMaxPromotionPdu = Convert.ToByte(e.Value);
                    break;
                case 4:
                    MacPromotionPduTxPeriod = Convert.ToByte(e.Value);
                    break;
                case 5:
                    MacBeaconsPerFrame = Convert.ToByte(e.Value);
                    break;
                case 6:
                    MacScpMaxTxAttempts = Convert.ToByte(e.Value);
                    break;
                case 7:
                    MacCtlReTxTimer = Convert.ToByte(e.Value);
                    break;
                case 8:
                    MacMaxCtlReTx = Convert.ToByte(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            MacMinSwitchSearchTime = (byte)reader.ReadElementContentAsInt("MacMinSwitchSearchTime");
            MacMaxPromotionPdu = (byte)reader.ReadElementContentAsInt("MacMaxPromotionPdu");
            MacPromotionPduTxPeriod = (byte)reader.ReadElementContentAsInt("MacPromotionPduTxPeriod");
            MacBeaconsPerFrame = (byte)reader.ReadElementContentAsInt("MacBeaconsPerFrame");
            MacScpMaxTxAttempts = (byte)reader.ReadElementContentAsInt("MacScpMaxTxAttempts");
            MacCtlReTxTimer = (byte)reader.ReadElementContentAsInt("MacCtlReTxTimer");
            MacMaxCtlReTx = (byte)reader.ReadElementContentAsInt("MacMaxCtlReTx");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("MacMinSwitchSearchTime", MacMinSwitchSearchTime, 2);
            writer.WriteElementString("MacMaxPromotionPdu", MacMaxPromotionPdu, 3);
            writer.WriteElementString("MacPromotionPduTxPeriod", MacPromotionPduTxPeriod, 4);
            writer.WriteElementString("MacBeaconsPerFrame", MacBeaconsPerFrame, 5);
            writer.WriteElementString("MacScpMaxTxAttempts", MacScpMaxTxAttempts, 6);
            writer.WriteElementString("MacCtlReTxTimer", MacCtlReTxTimer, 7);
            writer.WriteElementString("MacMaxCtlReTx", MacMaxCtlReTx, 8);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
