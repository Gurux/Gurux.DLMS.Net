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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSIEC14908PhysicalStatus
    /// </summary>
    public class GXDLMSIEC14908PhysicalStatus : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSIEC14908PhysicalStatus()
        : this("0.0.34.2.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSIEC14908PhysicalStatus(string ln)
        : base(ObjectType.IEC14908PhysicalStatus, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIEC14908PhysicalStatus(string ln, ushort sn)
        : base(ObjectType.IEC14908PhysicalStatus, ln, sn)
        {
        }

        /// <summary>
        /// Number of CRC errors detected during packet reception.
        /// </summary>
        [XmlIgnore()]
        public UInt16 TransmissionErrors
        {
            get;
            set;
        }

        /// <summary>
        /// Transmit Tx Failures.
        /// </summary>
        [XmlIgnore()]
        public UInt16 TransmitFailure
        {
            get;
            set;
        }

        /// <summary>
        /// Transmit Tx retries.
        /// </summary>
        [XmlIgnore()]
        public UInt16 TransmitRetries
        {
            get;
            set;
        }

        /// <summary>
        /// Receive Full.
        /// </summary>
        [XmlIgnore()]
        public UInt16 ReceiveFull
        {
            get;
            set;
        }

        /// <summary>
        /// Lost messages.
        /// </summary>
        [XmlIgnore()]
        public UInt16 LostMessages
        {
            get;
            set;
        }

        /// <summary>
        /// Number of times that an incoming packet was discarded because there was no network buffer available.
        /// </summary>
        [XmlIgnore()]
        public UInt16 MissedMessages
        {
            get;
            set;
        }

        /// <summary>
        /// Number of messages received by the MAC layer with valid CRC.
        /// </summary>
        [XmlIgnore()]
        public UInt16 Layer2Received
        {
            get;
            set;
        }

        /// <summary>
        /// Number of messages received by the MAC layer with valid CRC addressed specifically to this node.
        /// </summary>
        [XmlIgnore()]
        public UInt16 Layer3Received
        {
            get;
            set;
        }

        /// <summary>
        /// Number of message received delivered to the Adaptation layer.
        /// </summary>
        [XmlIgnore()]
        public UInt16 MessagesReceived
        {
            get;
            set;
        }
        /// <summary>
        /// Number of message received delivered to the Adaptation layer.
        /// </summary>
        [XmlIgnore()]
        public UInt16 MessagesValidated
        {
            get;
            set;
        }


        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, TransmissionErrors, TransmitFailure, TransmitRetries, ReceiveFull,
                              LostMessages, MissedMessages, Layer2Received, Layer3Received, MessagesReceived, MessagesValidated
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
            //TransmissionErrors
            attributes.Add(2);
            //TransmitFailure
            attributes.Add(3);
            //TransmitRetries
            attributes.Add(4);
            //ReceiveFull
            attributes.Add(5);
            //LostMessages
            attributes.Add(6);
            //MissedMessages
            attributes.Add(7);
            //Layer2Received
            attributes.Add(8);
            //Layer3Received
            attributes.Add(9);
            //MessagesReceived
            attributes.Add(10);
            //MessagesValidated
            attributes.Add(11);
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "TransmissionErrors", "TransmitFailure", "TransmitRetries", "ReceiveFull",
                              "LostMessages", "MissedMessages", "Layer2Received", "Layer3Received", "MessagesReceived", "MessagesValidated"
                            };
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
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.UInt16;
            }
            if (index == 3)
            {
                return DataType.UInt16;
            }
            if (index == 4)
            {
                return DataType.UInt16;
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
                return DataType.UInt16;
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
                return TransmissionErrors;
            }
            if (e.Index == 3)
            {
                return TransmitFailure;
            }
            if (e.Index == 4)
            {
                return TransmitRetries;
            }
            if (e.Index == 5)
            {
                return ReceiveFull;
            }
            if (e.Index == 6)
            {
                return LostMessages; ;
            }
            if (e.Index == 7)
            {
                return MissedMessages;
            }
            if (e.Index == 8)
            {
                return Layer2Received;
            }
            if (e.Index == 9)
            {
                return Layer3Received;
            }
            if (e.Index == 10)
            {
                return MessagesReceived;
            }
            if (e.Index == 11)
            {
                return MessagesValidated;
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
                TransmissionErrors = (UInt16)e.Value;
            }
            else if (e.Index == 3)
            {
                TransmitFailure = (UInt16)e.Value;
            }
            else if (e.Index == 4)
            {
                TransmitRetries = (UInt16)e.Value;
            }
            else if (e.Index == 5)
            {
                ReceiveFull = (UInt16)e.Value;
            }
            else if (e.Index == 6)
            {
                LostMessages = (UInt16)e.Value;
            }
            else if (e.Index == 7)
            {
                MissedMessages = (UInt16)e.Value;
            }
            else if (e.Index == 8)
            {
                Layer2Received = (UInt16)e.Value;
            }
            else if (e.Index == 9)
            {
                Layer3Received = (UInt16)e.Value;
            }
            else if (e.Index == 10)
            {
                MessagesReceived = (UInt16)e.Value;
            }
            else if (e.Index == 11)
            {
                MessagesValidated = (UInt16)e.Value;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            TransmissionErrors = (UInt16)reader.ReadElementContentAsInt("TransmissionErrors");
            TransmitFailure = (UInt16)reader.ReadElementContentAsInt("TransmitFailure");
            TransmitRetries = (UInt16)reader.ReadElementContentAsInt("TransmitRetries");
            ReceiveFull = (UInt16)reader.ReadElementContentAsInt("ReceiveFull");
            LostMessages = (UInt16)reader.ReadElementContentAsInt("LostMessages");
            MissedMessages = (UInt16)reader.ReadElementContentAsInt("MissedMessages");
            Layer2Received = (UInt16)reader.ReadElementContentAsInt("Layer2Received");
            Layer3Received = (UInt16)reader.ReadElementContentAsInt("Layer3Received");
            MessagesReceived = (UInt16)reader.ReadElementContentAsInt("MessagesReceived");
            MessagesValidated = (UInt16)reader.ReadElementContentAsInt("MessagesValidated");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("TransmissionErrors", TransmissionErrors, 2);
            writer.WriteElementString("TransmitFailure", TransmitFailure, 3);
            writer.WriteElementString("TransmitRetries", TransmitRetries, 4);
            writer.WriteElementString("ReceiveFull", ReceiveFull, 5);
            writer.WriteElementString("LostMessages", LostMessages, 6);
            writer.WriteElementString("MissedMessages", MissedMessages, 7);
            writer.WriteElementString("Layer2Received", Layer2Received, 8);
            writer.WriteElementString("Layer3Received", Layer3Received, 9);
            writer.WriteElementString("MessagesReceived", MessagesReceived, 10);
            writer.WriteElementString("MessagesValidated", MessagesValidated, 11);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
