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

using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using System;
using System.Collections.Generic;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMSPrimeDataConcentrator contains information that is needed 
    /// if PRIME data concentrator is used between the client and the meter.
    /// </summary>
    public class GXDLMSPrimeDataConcentrator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSPrimeDataConcentrator()
        {
        }

        /// <summary>
        /// Notification message type.
        /// </summary>
        public PrimeDcMsgType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Device identifier that will be used to address the device this
        /// notification refers to, both in custom messages and in the
        /// standard DLMS/TCP communications.
        /// </summary>
        public UInt16 DeviceID
        {
            get;
            set;
        }

        /// <summary>
        /// Flags with the capabilities of the device.
        /// </summary>
        public UInt16 Capabilities
        {
            get;
            set;
        }

        /// <summary>
        /// DLMS Identifier of the reported device.
        /// </summary>
        public byte[] DlmsId
        {
            get;
            set;
        }
        /// <summary>
        /// EUI48 of the reported device.
        /// </summary>
        public byte[] Eui48
        {
            get;
            set;
        }

        /// <summary>
        /// This method generates new device notification message.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <returns>Generated message.</returns>
        public byte[] GenerateNewDeviceNotification(GXDLMSClient client)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(PrimeDcMsgType.NewDeviceNotification);
            bb.SetUInt16(DeviceID);
            bb.SetUInt16(Capabilities);
            bb.SetUInt8((byte)DlmsId.Length);
            bb.Set(DlmsId);
            bb.Set(Eui48);
            return GXDLMS.GetWrapperFrame(client.Settings, Command.DataNotification, bb);
        }

        /// <summary>
        /// This method generates remove device notification message.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <returns>Generated message.</returns>
        public byte[] GenerateRemoveDeviceNotification(GXDLMSClient client)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(PrimeDcMsgType.RemoveDeviceNotification);
            bb.SetUInt16(DeviceID);
            return GXDLMS.GetWrapperFrame(client.Settings, Command.DataNotification, bb);
        }

        /// <summary>
        /// This method generates start reporting meters message.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <returns>Generated message.</returns>
        public byte[] GenerateStartReportingMeters(GXDLMSClient client)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(PrimeDcMsgType.StartReportingMeters);
            return GXDLMS.GetWrapperFrame(client.Settings, Command.SetRequest, bb);
        }

        /// <summary>
        /// This method generates delete meters notification message.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <returns>Generated message.</returns>
        public byte[] GenerateDeleteMetersNotification(GXDLMSClient client)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(PrimeDcMsgType.DeleteMeters);
            bb.SetUInt16(DeviceID);
            return GXDLMS.GetWrapperFrame(client.Settings, Command.SetRequest, bb);
        }

        /// <summary>
        /// This method generates enable auto close notification message.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <returns>Generated message.</returns>
        public byte[] GenerateEnableAutoCloseNotification(GXDLMSClient client)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(PrimeDcMsgType.EnableAutoClose);
            bb.SetUInt16(DeviceID);
            return GXDLMS.GetWrapperFrame(client.Settings, Command.SetRequest, bb);
        }

        /// <summary>
        /// This method generates disable auto close notification message.
        /// </summary>
        /// <param name="client">DLMS client settings.</param>
        /// <returns>Generated message.</returns>
        public byte[] GenerateDisableAutoCloseNotification(GXDLMSClient client)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(PrimeDcMsgType.DisableAutoClose);
            bb.SetUInt16(DeviceID);
            return GXDLMS.GetWrapperFrame(client.Settings, Command.SetRequest, bb);
        }
    }
}