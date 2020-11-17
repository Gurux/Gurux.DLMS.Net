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

using Gurux.DLMS.Internal;
using Gurux.DLMS.Plc;
using Gurux.DLMS.Plc.Enums;
using System;
using System.Collections.Generic;

namespace Gurux.DLMS
{
    /// <summary>
    /// PLC communication settings.
    /// </summary>
    public class GXPlcSettings
    {
        byte[] _systemTitle;
        private GXDLMSSettings settings;

        /// <summary>
        /// Initial credit (IC) tells how many times the frame must be repeated. Maximum value is 7.
        /// </summary>
        public byte InitialCredit
        {
            get;
            set;
        }

        /// <summary>
        /// The current credit (CC) initial value equal to IC and automatically decremented by the MAC layer after each repetition.
        /// Maximum value is 7.
        /// </summary>
        public byte CurrentCredit
        {
            get;
            set;
        }

        /// <summary>
        /// Delta credit (DC) is used by the system management application entity
        /// (SMAE) of the Client for credit management, while it has no meaning for a Server or a REPEATER.
        /// It represents the difference(IC-CC) of the last communication originated by the system identified by the DA address to the system identified by the SA address.
        ///  Maximum value is 3.
        /// </summary>
        public byte DeltaCredit
        {
            get;
            set;
        }

        /// <summary>
        /// IEC 61334-4-32 LLC uses 6 bytes long system title. IEC 61334-5-1 uses 8 bytes long system title so we can use the default one.
        /// </summary>
        public byte[] SystemTitle
        {
            get
            {
                if (settings != null && settings.InterfaceType != Enums.InterfaceType.Plc && settings.Cipher != null)
                {
                    return settings.Cipher.SystemTitle;
                }
                return _systemTitle;
            }
            set
            {
                if (settings != null && settings.InterfaceType != Enums.InterfaceType.Plc && settings.Cipher != null)
                {
                    settings.Cipher.SystemTitle = value;
                }
                _systemTitle = value;
            }
        }

        /// <summary>
        /// Source MAC address.
        /// </summary>
        public UInt16 MacSourceAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Destination MAC address.
        /// </summary>
        public UInt16 MacDestinationAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Response probability.
        /// </summary>
        public byte ResponseProbability
        {
            get;
            set;
        }

        /// <summary>
        /// Allowed time slots.
        /// </summary>
        public UInt16 AllowedTimeSlots
        {
            get;
            set;
        }

        /// <summary>
        /// Server saves client system title.
        /// </summary>
        public byte[] ClientSystemTitle
        {
            get;
            set;
        }

        internal GXPlcSettings(GXDLMSSettings s)
        {
            settings = s;
            InitialCredit = 7;
            CurrentCredit = 7;
            DeltaCredit = 0;
            //New device addresses are used.
            if (s.InterfaceType == Enums.InterfaceType.Plc)
            {
                if (s.IsServer)
                {
                    MacSourceAddress = (UInt16)PlcSourceAddress.New;
                    MacDestinationAddress = (UInt16)PlcSourceAddress.Initiator;
                }
                else
                {
                    MacSourceAddress = (UInt16)PlcSourceAddress.Initiator;
                    MacDestinationAddress = (UInt16)PlcDestinationAddress.AllPhysical;
                }
            }
            else
            {
                if (s.IsServer)
                {
                    MacSourceAddress = (UInt16)PlcSourceAddress.New;
                    MacDestinationAddress = (UInt16)PlcHdlcSourceAddress.Initiator;
                }
                else
                {
                    MacSourceAddress = (UInt16)PlcHdlcSourceAddress.Initiator;
                    MacDestinationAddress = (UInt16)PlcDestinationAddress.AllPhysical;
                }
            }
            ResponseProbability = 100;
            if (s.InterfaceType == Enums.InterfaceType.Plc)
            {
                AllowedTimeSlots = 10;
            }
            else
            {
                AllowedTimeSlots = 0x14;
            }
        }

        /// <summary>
        /// Discover available PLC meters.
        /// </summary>
        /// <returns>Generated bytes.</returns>
        public byte[] DiscoverRequest()
        {
            GXByteBuffer bb = new GXByteBuffer();
            if (settings.InterfaceType != Enums.InterfaceType.Plc &&
                settings.InterfaceType != Enums.InterfaceType.PlcHdlc)
            {
                throw new ArgumentOutOfRangeException("Invalid interface type.");
            }
            if (settings.InterfaceType == Enums.InterfaceType.PlcHdlc)
            {
                bb.Set(GXCommon.LLCSendBytes);
            }
            bb.SetUInt8((byte)Command.DiscoverRequest);
            bb.SetUInt8(ResponseProbability);
            bb.SetUInt16(AllowedTimeSlots);
            //DiscoverReport initial credit
            bb.SetUInt8(0);
            // IC Equal credit
            bb.SetUInt8(0);
            int val = 0;
            int clientAddress = settings.ClientAddress;
            int serverAddress = settings.ServerAddress;
            UInt16 da = settings.Plc.MacDestinationAddress;
            UInt16 sa = settings.Plc.MacSourceAddress;
            try
            {
                //10.4.6.4 Source and destination APs and addresses of CI-PDUs
                //Client address is No-station in discoverReport.
                if (settings.InterfaceType == Enums.InterfaceType.PlcHdlc)
                {
                    settings.Plc.InitialCredit = 0;
                    settings.Plc.CurrentCredit = 0;
                    settings.Plc.MacSourceAddress = 0xC01;
                    settings.Plc.MacDestinationAddress = 0xFFF;
                    settings.ClientAddress = 0x66;
                    // All-station
                    settings.ServerAddress = 0x33FF;
                }
                else
                {
                    val = settings.Plc.InitialCredit << 5;
                    val |= settings.Plc.CurrentCredit << 2;
                    val |= settings.Plc.DeltaCredit & 0x3;
                    settings.Plc.MacSourceAddress = 0xC00;
                    settings.ClientAddress = 1;
                    settings.ServerAddress = 0;
                }
                return GXDLMS.GetMacFrame(settings, 0x13, (byte)val, bb);
            }
            finally
            {
                settings.ClientAddress = clientAddress;
                settings.ServerAddress = serverAddress;
                settings.Plc.MacDestinationAddress = da;
                settings.Plc.MacSourceAddress = sa;
            }
        }

        /// <summary>
        /// Generates discover report.
        /// </summary>
        /// <param name="systemTitle">System title</param>
        /// <param name="newMeter">Is this a new meter.</param>
        /// <returns>Generated bytes.</returns>
        public byte[] DiscoverReport(byte[] systemTitle, bool newMeter)
        {
            GXByteBuffer bb = new GXByteBuffer();
            if (settings.InterfaceType != Enums.InterfaceType.Plc &&
                settings.InterfaceType != Enums.InterfaceType.PlcHdlc)
            {
                throw new ArgumentOutOfRangeException("Invalid interface type.");
            }
            byte alarmDescription;
            if (settings.InterfaceType == Enums.InterfaceType.Plc)
            {
                alarmDescription = (byte)(newMeter ? 1 : 0x82);
            }
            else
            {
                alarmDescription = 0;
            }
            if (settings.InterfaceType == Enums.InterfaceType.PlcHdlc)
            {
                bb.Set(GXCommon.LLCReplyBytes);
            }
            bb.SetUInt8((byte)Command.DiscoverReport);
            bb.SetUInt8(1);
            bb.Set(systemTitle);
            if (alarmDescription != 0)
            {
                bb.SetUInt8(1);
            }
            bb.SetUInt8(alarmDescription);
            int clientAddress = settings.ClientAddress;
            int serverAddress = settings.ServerAddress;
            UInt16 macSourceAddress = settings.Plc.MacSourceAddress;
            UInt16 macTargetAddress = settings.Plc.MacDestinationAddress;
            try
            {
                //10.4.6.4 Source and destination APs and addresses of CI-PDUs
                //Client address is No-station in discoverReport.
                if (settings.InterfaceType == Enums.InterfaceType.PlcHdlc)
                {
                    settings.Plc.MacDestinationAddress = (UInt16) PlcHdlcSourceAddress.Initiator;
                }
                else
                {
                    settings.ClientAddress = 0;
                    settings.ServerAddress = 0xFD;
                }
                return GXDLMS.GetMacFrame(settings, 0x13, 0, bb);
            }
            finally
            {
                settings.ClientAddress = clientAddress;
                settings.ServerAddress = serverAddress;
                settings.Plc.MacSourceAddress = macSourceAddress;
                settings.Plc.MacDestinationAddress = macTargetAddress;
            }
        }

        /// <summary>
        /// Parse discover reply.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Array of system titles and alarm descriptor error code</returns>
        public List<GXDLMSPlcMeterInfo> ParseDiscover(GXByteBuffer value, UInt16 sa, UInt16 da)
        {
            List<GXDLMSPlcMeterInfo> list = new List<GXDLMSPlcMeterInfo>();
            byte count = value.GetUInt8();
            for (int pos = 0; pos != count; ++pos)
            {
                GXDLMSPlcMeterInfo info = new GXDLMSPlcMeterInfo();
                info.SourceAddress = sa;
                info.DestinationAddress = da;
                //Get System title.
                if (settings.InterfaceType == Enums.InterfaceType.PlcHdlc)
                {
                    info.SystemTitle = new byte[8];
                }
                else
                {
                    info.SystemTitle = new byte[6];
                }
                value.Get(info.SystemTitle);
                // Alarm descriptor of the reporting system.
                // Alarm-Descriptor presence flag
                if (value.GetUInt8() != 0)
                {
                    //Alarm-Descriptor
                    info.AlarmDescriptor = value.GetUInt8();
                }
                list.Add(info);
            }
            return list;
        }

        /// <summary>
        /// Register PLC meters.
        /// </summary>
        /// <param name="initiatorSystemTitle">Active initiator systemtitle</param>
        /// <param name="systemTitle"></param>
        /// <returns>Generated bytes.</returns>
        public byte[] RegisterRequest(byte[] initiatorSystemTitle, byte[] systemTitle)
        {
            GXByteBuffer bb = new GXByteBuffer();
            //Control byte.
            bb.SetUInt8((byte)Command.RegisterRequest);
            bb.Set(initiatorSystemTitle);
            //LEN
            bb.SetUInt8(0x1);
            bb.Set(systemTitle);
            //MAC address.
            bb.SetUInt16(MacSourceAddress);
            int val = settings.Plc.InitialCredit << 5;
            val |= settings.Plc.CurrentCredit << 2;
            val |= settings.Plc.DeltaCredit & 0x3;

            int clientAddress = settings.ClientAddress;
            int serverAddress = settings.ServerAddress;
            UInt16 macSourceAddress = settings.Plc.MacSourceAddress;
            UInt16 macTargetAddress = settings.Plc.MacDestinationAddress;
            try
            {
                //10.4.6.4 Source and destination APs and addresses of CI-PDUs
                //Client address is No-station in discoverReport.
                if (settings.InterfaceType == Enums.InterfaceType.PlcHdlc)
                {
                    settings.Plc.InitialCredit = 0;
                    settings.Plc.CurrentCredit = 0;
                    settings.Plc.MacSourceAddress = 0xC01;
                    settings.Plc.MacDestinationAddress = 0xFFF;
                    settings.ClientAddress = 0x66;
                    // All-station
                    settings.ServerAddress = 0x33FF;

                }
                else
                {
                    settings.ClientAddress = 1;
                    settings.ServerAddress = 0;
                    settings.Plc.MacSourceAddress = 0xC00;
                    settings.Plc.MacDestinationAddress = 0xFFF;
                }
                return GXDLMS.GetMacFrame(settings, 0x13, (byte)val, bb);
            }
            finally
            {
                settings.ClientAddress = clientAddress;
                settings.ServerAddress = serverAddress;
                settings.Plc.MacSourceAddress = macSourceAddress;
                settings.Plc.MacDestinationAddress = macTargetAddress;
            }
        }

        /// <summary>
        /// Parse register request.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>System title mac address.</returns>
        public void ParseRegisterRequest(GXByteBuffer value)
        {
            //Get System title.
            byte[] st;
            if (settings.InterfaceType == Enums.InterfaceType.PlcHdlc)
            {
                st = new byte[8];
            }
            else
            {
                st = new byte[6];
            }
            value.Get(st);
            byte count = value.GetUInt8();
            for (int pos = 0; pos != count; ++pos)
            {
                //Get System title.
                if (settings.InterfaceType == Enums.InterfaceType.PlcHdlc)
                {
                    st = new byte[8];
                }
                else
                {
                    st = new byte[6];
                }
                value.Get(st);
                SystemTitle = st;
                //MAC address.
                MacSourceAddress = value.GetUInt16();
            }
        }

        /// <summary>
        /// Parse discover request.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GXDLMSPlcRegister ParseDiscoverRequest(GXByteBuffer value)
        {
            GXDLMSPlcRegister ret = new GXDLMSPlcRegister();
            ret.ResponseProbability = value.GetUInt8();
            ret.AllowedTimeSlots = value.GetUInt16();
            ret.DiscoverReportInitialCredit = value.GetUInt8();
            ret.ICEqualCredit = value.GetUInt8();
            return ret;
        }

        /// <summary>
        /// Ping PLC meter.
        /// </summary>
        /// <returns>Generated bytes.</returns>
        public byte[] PingRequest(byte[] systemTitle)
        {
            GXByteBuffer bb = new GXByteBuffer();
            //Control byte.
            bb.SetUInt8((byte)Command.PingRequest);
            bb.Set(systemTitle);
            return GXDLMS.GetMacFrame(settings, 0x13, 0, bb);
        }

        /// <summary>
        /// Parse ping response.
        /// </summary>
        /// <param name="value">Received data.</param>
        public byte[] ParsePing(GXByteBuffer value)
        {
            return value.SubArray(1, 6);
        }

        /// <summary>
        /// Repear call request.
        /// </summary>
        /// <returns>Generated bytes.</returns>
        public byte[] RepeaterCallRequest()
        {
            GXByteBuffer bb = new GXByteBuffer();
            //Control byte.
            bb.SetUInt8((byte)Command.RepeatCallRequest);
            //MaxAdrMac.
            bb.SetUInt16(0x63);
            //Nb_Tslot_For_New
            bb.SetUInt8(0);
            //Reception-Threshold default value
            bb.SetUInt8(0);
            return GXDLMS.GetMacFrame(settings, 0x13, 0xFC, bb);
        }
    }
}