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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCoAPDiagnostic
    /// </summary>
    public class GXDLMSCoAPDiagnostic : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSCoAPDiagnostic()
        : this("0.0.25.17.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSCoAPDiagnostic(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSCoAPDiagnostic(string ln, ushort sn)
        : base(ObjectType.CoAPDiagnostic, ln, sn)
        {
            MessagesCounter = new GXCoapMessagesCounter();
            RequestResponseCounter = new GXCoapRequestResponseCounter();
            BtCounter = new GXCoapBtCounter();
            CaptureTime = new GXCoapCaptureTime();
        }

        /// <summary>
        /// CoAP messages counter.
        /// </summary>
        [XmlIgnore()]
        public GXCoapMessagesCounter MessagesCounter
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP request response counter.
        /// </summary>
        [XmlIgnore()]
        public GXCoapRequestResponseCounter RequestResponseCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Bt counter.
        /// </summary>
        [XmlIgnore()]
        public GXCoapBtCounter BtCounter
        {
            get;
            set;
        }

        /// <summary>
        /// CoAP Capture time.
        /// </summary>
        [XmlIgnore()]
        public GXCoapCaptureTime CaptureTime
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName,
                MessagesCounter,
                RequestResponseCounter,
                BtCounter,
                CaptureTime};
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
            //MessagesCounter
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //RequestResponseCounter
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //BtCounter
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //CaptureTime
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(),
                "Messages counter",
                "Request response counter",
                "BT counter",
                "Capture time"};
        }

        /// <summary>
        /// Reset diagnostic values.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Reset(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        /// <inheritdoc />
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
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
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
                    return DataType.Structure;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            GXByteBuffer buff = new GXByteBuffer();
            object ret = null;
            switch (e.Index)
            {
                case 1:
                    ret = GXCommon.LogicalNameToBytes(LogicalName);
                    break;
                case 2:
                    buff.SetUInt8(DataType.Structure);
                    GXCommon.SetObjectCount(10, buff);
                    GXCommon.SetData(settings, buff, DataType.UInt32, MessagesCounter.Tx);
                    GXCommon.SetData(settings, buff, DataType.UInt32, MessagesCounter.Rx);
                    GXCommon.SetData(settings, buff, DataType.UInt32, MessagesCounter.TxResend);
                    GXCommon.SetData(settings, buff, DataType.UInt32, MessagesCounter.TxReset);
                    GXCommon.SetData(settings, buff, DataType.UInt32, MessagesCounter.RxReset);
                    GXCommon.SetData(settings, buff, DataType.UInt32, MessagesCounter.TxAck);
                    GXCommon.SetData(settings, buff, DataType.UInt32, MessagesCounter.RxAck);
                    GXCommon.SetData(settings, buff, DataType.UInt32, MessagesCounter.RxDrop);
                    GXCommon.SetData(settings, buff, DataType.UInt32, MessagesCounter.TxNonPiggybacked);
                    GXCommon.SetData(settings, buff, DataType.UInt32, MessagesCounter.MaxRtxExceeded);
                    ret = buff.Array();
                    break;
                case 3:
                    buff.SetUInt8(DataType.Structure);
                    GXCommon.SetObjectCount(8, buff);
                    GXCommon.SetData(settings, buff, DataType.UInt32, RequestResponseCounter.RxRequests);
                    GXCommon.SetData(settings, buff, DataType.UInt32, RequestResponseCounter.TxRequests);
                    GXCommon.SetData(settings, buff, DataType.UInt32, RequestResponseCounter.RxResponse);
                    GXCommon.SetData(settings, buff, DataType.UInt32, RequestResponseCounter.TxResponse);
                    GXCommon.SetData(settings, buff, DataType.UInt32, RequestResponseCounter.TxClientError);
                    GXCommon.SetData(settings, buff, DataType.UInt32, RequestResponseCounter.RxClientError);
                    GXCommon.SetData(settings, buff, DataType.UInt32, RequestResponseCounter.TxServerError);
                    GXCommon.SetData(settings, buff, DataType.UInt32, RequestResponseCounter.RxServerError);
                    ret = buff.Array();
                    break;
                case 4:
                    buff.SetUInt8(DataType.Structure);
                    GXCommon.SetObjectCount(3, buff);
                    GXCommon.SetData(settings, buff, DataType.UInt32, BtCounter.BlockWiseTransferStarted);
                    GXCommon.SetData(settings, buff, DataType.UInt32, BtCounter.BlockWiseTransferCompleted);
                    GXCommon.SetData(settings, buff, DataType.UInt32, BtCounter.BlockWiseTransferTimeout);
                    ret = buff.Array();
                    break;
                case 5:
                    buff.SetUInt8(DataType.Structure);
                    GXCommon.SetObjectCount(2, buff);
                    GXCommon.SetData(settings, buff, DataType.UInt8, CaptureTime.AttributeId);
                    GXCommon.SetData(settings, buff, DataType.DateTime, CaptureTime.TimeStamp);
                    ret = buff.Array();
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
                        MessagesCounter.Tx = (UInt32)s[0];
                        MessagesCounter.Rx = (UInt32)s[1];
                        MessagesCounter.TxResend = (UInt32)s[2];
                        MessagesCounter.TxReset = (UInt32)s[3];
                        MessagesCounter.RxReset = (UInt32)s[4];
                        MessagesCounter.TxAck = (UInt32)s[5];
                        MessagesCounter.RxAck = (UInt32)s[6];
                        MessagesCounter.RxDrop = (UInt32)s[7];
                        MessagesCounter.TxNonPiggybacked = (UInt32)s[8];
                        MessagesCounter.MaxRtxExceeded = (UInt32)s[9];
                    }
                    break;
                case 3:
                    {
                        GXStructure s = e.Value as GXStructure;
                        RequestResponseCounter.RxRequests = (UInt32)s[0];
                        RequestResponseCounter.TxRequests = (UInt32)s[1];
                        RequestResponseCounter.RxResponse = (UInt32)s[2];
                        RequestResponseCounter.TxResponse = (UInt32)s[3];
                        RequestResponseCounter.TxClientError = (UInt32)s[4];
                        RequestResponseCounter.RxClientError = (UInt32)s[5];
                        RequestResponseCounter.TxServerError = (UInt32)s[6];
                        RequestResponseCounter.RxServerError = (UInt32)s[7];
                    }
                    break;
                case 4:
                    {
                        GXStructure s = e.Value as GXStructure;
                        BtCounter.BlockWiseTransferStarted = (UInt32)s[0];
                        BtCounter.BlockWiseTransferCompleted = (UInt32)s[1];
                        BtCounter.BlockWiseTransferTimeout = (UInt32)s[2];
                    }
                    break;
                case 5:
                    {
                        GXStructure s = e.Value as GXStructure;
                        CaptureTime.AttributeId = (byte)s[0];
                        CaptureTime.TimeStamp = (GXDateTime)s[1];
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            MessagesCounter.Tx = (UInt32)reader.ReadElementContentAsLong("Tx");
            MessagesCounter.Rx = (UInt32)reader.ReadElementContentAsLong("Rx");
            MessagesCounter.TxResend = (UInt32)reader.ReadElementContentAsLong("TxResend");
            MessagesCounter.TxReset = (UInt32)reader.ReadElementContentAsLong("TxReset");
            MessagesCounter.RxReset = (UInt32)reader.ReadElementContentAsLong("RxReset");
            MessagesCounter.TxAck = (UInt32)reader.ReadElementContentAsLong("TxAck");
            MessagesCounter.RxAck = (UInt32)reader.ReadElementContentAsLong("RxAck");
            MessagesCounter.RxDrop = (UInt32)reader.ReadElementContentAsLong("RxDrop");
            MessagesCounter.TxNonPiggybacked = (UInt32)reader.ReadElementContentAsLong("TxNonPiggybacked");
            MessagesCounter.MaxRtxExceeded = (UInt32)reader.ReadElementContentAsLong("MaxRtxExceeded");

            RequestResponseCounter.RxRequests = (UInt32)reader.ReadElementContentAsLong("RxRequests");
            RequestResponseCounter.TxRequests = (UInt32)reader.ReadElementContentAsLong("TxRequests");
            RequestResponseCounter.RxResponse = (UInt32)reader.ReadElementContentAsLong("RxResponse");
            RequestResponseCounter.TxResponse = (UInt32)reader.ReadElementContentAsLong("TxResponse");
            RequestResponseCounter.TxClientError = (UInt32)reader.ReadElementContentAsLong("TxClientError");
            RequestResponseCounter.RxClientError = (UInt32)reader.ReadElementContentAsLong("RxClientError");
            RequestResponseCounter.TxServerError = (UInt32)reader.ReadElementContentAsLong("TxServerError");
            RequestResponseCounter.RxServerError = (UInt32)reader.ReadElementContentAsLong("RxServerError");
            BtCounter.BlockWiseTransferStarted = (UInt32)reader.ReadElementContentAsLong("TransferStarted");
            BtCounter.BlockWiseTransferCompleted = (UInt32)reader.ReadElementContentAsLong("TransferCompleted");
            BtCounter.BlockWiseTransferTimeout = (UInt32)reader.ReadElementContentAsLong("TransferTimeout");
            CaptureTime.AttributeId = (byte)reader.ReadElementContentAsInt("AttributeId");
            CaptureTime.TimeStamp = reader.ReadElementContentAsDateTime("TimeStamp");

        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Tx", MessagesCounter.Tx, 2);
            writer.WriteElementString("Rx", MessagesCounter.Rx, 2);
            writer.WriteElementString("TxResend", MessagesCounter.TxResend, 2);
            writer.WriteElementString("TxReset", MessagesCounter.TxReset, 2);
            writer.WriteElementString("RxReset", MessagesCounter.RxReset, 2);
            writer.WriteElementString("TxAck", MessagesCounter.TxAck, 2);
            writer.WriteElementString("RxAck", MessagesCounter.RxAck, 2);
            writer.WriteElementString("RxDrop", MessagesCounter.RxDrop, 2);
            writer.WriteElementString("TxNonPiggybacked", MessagesCounter.TxNonPiggybacked, 2);
            writer.WriteElementString("MaxRtxExceeded", MessagesCounter.MaxRtxExceeded, 2);
            writer.WriteElementString("RxRequests", RequestResponseCounter.RxRequests, 3);
            writer.WriteElementString("TxRequests", RequestResponseCounter.TxRequests, 3);
            writer.WriteElementString("RxResponse", RequestResponseCounter.RxResponse, 3);
            writer.WriteElementString("TxResponse", RequestResponseCounter.TxResponse, 3);
            writer.WriteElementString("TxClientError", RequestResponseCounter.TxClientError, 3);
            writer.WriteElementString("RxClientError", RequestResponseCounter.RxClientError, 3);
            writer.WriteElementString("TxServerError", RequestResponseCounter.TxServerError, 3);
            writer.WriteElementString("RxServerError", RequestResponseCounter.RxServerError, 3);
            writer.WriteElementString("TransferStarted", BtCounter.BlockWiseTransferStarted, 4);
            writer.WriteElementString("TransferCompleted", BtCounter.BlockWiseTransferCompleted, 4);
            writer.WriteElementString("TransferTimeout", BtCounter.BlockWiseTransferTimeout, 4);
            writer.WriteElementString("AttributeId", CaptureTime.AttributeId, 5);
            writer.WriteElementString("TimeStamp", CaptureTime.TimeStamp, 5);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
