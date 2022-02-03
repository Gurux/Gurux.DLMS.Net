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
using System.Linq;
using System.Text;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Objects.Italy;
using Gurux.DLMS.Plc.Enums;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Ecdsa.Enums;
using Gurux.DLMS.Ecdsa;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMS implements methods to communicate with DLMS/COSEM metering devices.
    /// </summary>
    sealed class GXDLMS
    {
        internal static bool UseHdlc(InterfaceType type)
        {
            return type == InterfaceType.HDLC ||
                type == InterfaceType.HdlcWithModeE ||
                type == InterfaceType.PlcHdlc;
        }

        const byte CipheringHeaderSize = 7 + 12 + 3;
        internal const int DATA_TYPE_OFFSET = 0xFF0000;

        /// <summary>
        /// Generates Invoke ID and priority.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="increase">Is invoke ID increased.</param>
        /// <returns>Invoke ID and priority.</returns>
        static byte GetInvokeIDPriority(GXDLMSSettings settings, bool increase)
        {
            byte value = 0;
            if (settings.Priority == Priority.High)
            {
                value = 0x80;
            }
            if (settings.ServiceClass == ServiceClass.Confirmed)
            {
                value |= 0x40;
            }
            if (increase)
            {
                settings.InvokeID = (byte)((settings.InvokeID + 1) & 0xF);
            }
            value |= (byte)(settings.InvokeID & 0xF);
            return value;
        }

        /// <summary>
        /// Generates Invoke ID and priority.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <returns>Invoke ID and priority.</returns>
        static UInt32 GetLongInvokeIDPriority(GXDLMSSettings settings)
        {
            UInt32 value = 0;
            if (settings.Priority == Priority.High)
            {
                value = 0x80000000;
            }
            if (settings.ServiceClass == ServiceClass.Confirmed)
            {
                value |= 0x40000000;
            }
            value |= (UInt32)(settings.longInvokeID & 0xFFFFFF);
            ++settings.longInvokeID;
            return value;
        }

        /// <summary>
        /// Get all COSEM objects.
        /// </summary>
        /// <param name="availableObjectTypes">List of available COSEM objects.</param>
        private static void GetCosemObjects(Dictionary<ObjectType, Type> availableObjectTypes)
        {
            if (availableObjectTypes.Count == 0)
            {
                availableObjectTypes.Add(ObjectType.G3PlcMacLayerCounters, typeof(GXDLMSG3PlcMacLayerCounters));
                availableObjectTypes.Add(ObjectType.G3Plc6LoWPan, typeof(GXDLMSG3Plc6LoWPan));
                availableObjectTypes.Add(ObjectType.G3PlcMacSetup, typeof(GXDLMSG3PlcMacSetup));
                availableObjectTypes.Add(ObjectType.IEC14908Diagnostic, typeof(GXDLMSIEC14908Diagnostic));
                availableObjectTypes.Add(ObjectType.IEC14908PhysicalStatus, typeof(GXDLMSIEC14908PhysicalStatus));
                availableObjectTypes.Add(ObjectType.IEC14908PhysicalSetup, typeof(GXDLMSIEC14908PhysicalSetup));
                availableObjectTypes.Add(ObjectType.IEC14908Identification, typeof(GXDLMSIEC14908Identification));
                availableObjectTypes.Add(ObjectType.Ip6Setup, typeof(GXDLMSIp6Setup));
                availableObjectTypes.Add(ObjectType.SFSKMacCounters, typeof(GXDLMSSFSKMacCounters));
                availableObjectTypes.Add(ObjectType.SFSKMacSynchronizationTimeouts, typeof(GXDLMSSFSKMacSynchronizationTimeouts));
                availableObjectTypes.Add(ObjectType.SFSKActiveInitiator, typeof(GXDLMSSFSKActiveInitiator));
                availableObjectTypes.Add(ObjectType.SFSKPhyMacSetUp, typeof(GXDLMSSFSKPhyMacSetUp));
                availableObjectTypes.Add(ObjectType.IecTwistedPairSetup, typeof(GXDLMSIecTwistedPairSetup));
                availableObjectTypes.Add(ObjectType.DisconnectControl, typeof(GXDLMSDisconnectControl));
                availableObjectTypes.Add(ObjectType.ImageTransfer, typeof(GXDLMSImageTransfer));
                availableObjectTypes.Add(ObjectType.Limiter, typeof(GXDLMSLimiter));
                availableObjectTypes.Add(ObjectType.MBusClient, typeof(GXDLMSMBusClient));
                availableObjectTypes.Add(ObjectType.MBusMasterPortSetup, typeof(GXDLMSMBusMasterPortSetup));
                availableObjectTypes.Add(ObjectType.MBusSlavePortSetup, typeof(GXDLMSMBusSlavePortSetup));
                availableObjectTypes.Add(ObjectType.MacAddressSetup, typeof(GXDLMSMacAddressSetup));
                availableObjectTypes.Add(ObjectType.AssociationLogicalName, typeof(GXDLMSAssociationLogicalName));
                availableObjectTypes.Add(ObjectType.AssociationShortName, typeof(GXDLMSAssociationShortName));
                availableObjectTypes.Add(ObjectType.AutoAnswer, typeof(GXDLMSAutoAnswer));
                availableObjectTypes.Add(ObjectType.DemandRegister, typeof(GXDLMSDemandRegister));
                availableObjectTypes.Add(ObjectType.ActionSchedule, typeof(GXDLMSActionSchedule));
                availableObjectTypes.Add(ObjectType.ActivityCalendar, typeof(GXDLMSActivityCalendar));
                availableObjectTypes.Add(ObjectType.AutoConnect, typeof(GXDLMSAutoConnect));
                availableObjectTypes.Add(ObjectType.Clock, typeof(GXDLMSClock));
                availableObjectTypes.Add(ObjectType.Data, typeof(GXDLMSData));
                availableObjectTypes.Add(ObjectType.ExtendedRegister, typeof(GXDLMSExtendedRegister));
                availableObjectTypes.Add(ObjectType.GprsSetup, typeof(GXDLMSGprsSetup));
                availableObjectTypes.Add(ObjectType.IecHdlcSetup, typeof(GXDLMSHdlcSetup));
                availableObjectTypes.Add(ObjectType.IecLocalPortSetup, typeof(GXDLMSIECLocalPortSetup));
                availableObjectTypes.Add(ObjectType.Ip4Setup, typeof(GXDLMSIp4Setup));
                availableObjectTypes.Add(ObjectType.ModemConfiguration, typeof(GXDLMSModemConfiguration));
                availableObjectTypes.Add(ObjectType.PppSetup, typeof(GXDLMSPppSetup));
                availableObjectTypes.Add(ObjectType.ProfileGeneric, typeof(GXDLMSProfileGeneric));
                availableObjectTypes.Add(ObjectType.PushSetup, typeof(GXDLMSPushSetup));
                availableObjectTypes.Add(ObjectType.Register, typeof(GXDLMSRegister));
                availableObjectTypes.Add(ObjectType.RegisterActivation, typeof(GXDLMSRegisterActivation));
                availableObjectTypes.Add(ObjectType.RegisterMonitor, typeof(GXDLMSRegisterMonitor));
                availableObjectTypes.Add(ObjectType.SapAssignment, typeof(GXDLMSSapAssignment));
                availableObjectTypes.Add(ObjectType.Schedule, typeof(GXDLMSSchedule));
                availableObjectTypes.Add(ObjectType.ScriptTable, typeof(GXDLMSScriptTable));
                availableObjectTypes.Add(ObjectType.SecuritySetup, typeof(GXDLMSSecuritySetup));
                availableObjectTypes.Add(ObjectType.SpecialDaysTable, typeof(GXDLMSSpecialDaysTable));
                availableObjectTypes.Add(ObjectType.TcpUdpSetup, typeof(GXDLMSTcpUdpSetup));
                availableObjectTypes.Add(ObjectType.GSMDiagnostic, typeof(GXDLMSGSMDiagnostic));
                availableObjectTypes.Add(ObjectType.Account, typeof(GXDLMSAccount));
                availableObjectTypes.Add(ObjectType.Credit, typeof(GXDLMSCredit));
                availableObjectTypes.Add(ObjectType.Charge, typeof(GXDLMSCharge));
                availableObjectTypes.Add(ObjectType.TokenGateway, typeof(GXDLMSTokenGateway));
                availableObjectTypes.Add(ObjectType.ParameterMonitor, typeof(GXDLMSParameterMonitor));
                availableObjectTypes.Add(ObjectType.CompactData, typeof(GXDLMSCompactData));
                availableObjectTypes.Add(ObjectType.WirelessModeQchannel, typeof(GXDLMSWirelessModeQchannel));
                availableObjectTypes.Add(ObjectType.UtilityTables, typeof(GXDLMSUtilityTables));
                availableObjectTypes.Add(ObjectType.LlcSscsSetup, typeof(GXDLMSLlcSscsSetup));
                availableObjectTypes.Add(ObjectType.PrimeNbOfdmPlcPhysicalLayerCounters, typeof(GXDLMSPrimeNbOfdmPlcPhysicalLayerCounters));
                availableObjectTypes.Add(ObjectType.PrimeNbOfdmPlcMacSetup, typeof(GXDLMSPrimeNbOfdmPlcMacSetup));
                availableObjectTypes.Add(ObjectType.PrimeNbOfdmPlcMacFunctionalParameters, typeof(GXDLMSPrimeNbOfdmPlcMacFunctionalParameters));
                availableObjectTypes.Add(ObjectType.PrimeNbOfdmPlcMacCounters, typeof(GXDLMSPrimeNbOfdmPlcMacCounters));
                availableObjectTypes.Add(ObjectType.PrimeNbOfdmPlcMacNetworkAdministrationData, typeof(GXDLMSPrimeNbOfdmPlcMacNetworkAdministrationData));
                availableObjectTypes.Add(ObjectType.PrimeNbOfdmPlcApplicationsIdentification, typeof(GXDLMSPrimeNbOfdmPlcApplicationsIdentification));
                availableObjectTypes.Add(ObjectType.Iec8802LlcType1Setup, typeof(GXDLMSIec8802LlcType1Setup));
                availableObjectTypes.Add(ObjectType.Iec8802LlcType2Setup, typeof(GXDLMSIec8802LlcType2Setup));
                availableObjectTypes.Add(ObjectType.Iec8802LlcType3Setup, typeof(GXDLMSIec8802LlcType3Setup));
                availableObjectTypes.Add(ObjectType.SFSKReportingSystemList, typeof(GXDLMSSFSKReportingSystemList));
                availableObjectTypes.Add(ObjectType.Arbitrator, typeof(GXDLMSArbitrator));
                availableObjectTypes.Add(ObjectType.NtpSetup, typeof(GXDLMSNtpSetup));
                availableObjectTypes.Add(ObjectType.CommunicationPortProtection, typeof(GXDLMSCommunicationPortProtection));
                //Italian standard uses this.
                availableObjectTypes.Add(ObjectType.TariffPlan, typeof(GXDLMSTariffPlan));
            }
        }

        /// <summary>
        /// Returns object types.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This can be used with serialization.
        /// </remarks>
        public static Type[] GetObjectTypes(Dictionary<ObjectType, Type> availableObjectTypes)
        {
            lock (availableObjectTypes)
            {
                GetCosemObjects(availableObjectTypes);
                List<Type> types = new List<Type>();
                types.AddRange(availableObjectTypes.Values);
                //This is removed later.
                types.Add(typeof(GXDLMSIECOpticalPortSetup));
                return types.ToArray();
            }
        }

        /// <summary>
        /// Returns object types.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This can be used with serialization.
        /// </remarks>
        public static KeyValuePair<ObjectType, int>[] GetObjectTypes2(Dictionary<ObjectType, Type> availableObjectTypes)
        {
            lock (availableObjectTypes)
            {
                GetCosemObjects(availableObjectTypes);
                KeyValuePair<ObjectType, int>[] types = new KeyValuePair<ObjectType, int>[availableObjectTypes.Count];
                int pos = 0;
                foreach (UInt32 it in availableObjectTypes.Keys)
                {
                    types[pos] = new KeyValuePair<ObjectType, int>((ObjectType)(it & 0xFFFF), (int)(it >> 16));
                    ++pos;
                }
                return types;
            }
        }


        /// <summary>
        /// Get available COSEM objects.
        /// </summary>
        /// <param name="availableObjectTypes"></param>
        internal static void GetAvailableObjects(Dictionary<ObjectType, Type> availableObjectTypes)
        {
            lock (availableObjectTypes)
            {
                GetObjectTypes(availableObjectTypes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="version">COSEM version number. It's ignored if it's 255.</param>
        /// <param name="availableObjectTypes"></param>
        /// <returns></returns>
        internal static GXDLMSObject CreateObject(ObjectType type, byte version, Dictionary<ObjectType, Type> availableObjectTypes)
        {
            GXDLMSObject obj;
            lock (availableObjectTypes)
            {
                //Update objects.
                if (availableObjectTypes.Count == 0)
                {
                    GetObjectTypes(availableObjectTypes);
                }
                if (availableObjectTypes.ContainsKey(type))
                {
                    obj = Activator.CreateInstance(availableObjectTypes[type]) as GXDLMSObject;
                    if (version != 0xFF)
                    {
                        obj.Version = version;
                    }
                }
                else
                {
                    obj = new GXDLMSObject();
                    obj.ObjectType = type;
                    if (version != 0xFF)
                    {
                        obj.Version = version;
                    }
                }
            }
            return obj;
        }

        ///<summary>
        ///Generates an acknowledgment message, with which the server is informed to send next packets.
        ///</summary>
        ///<param name="type">
        /// Frame type.
        /// </param>
        ///<returns>
        ///Acknowledgment message as byte array.
        ///</returns>
        [Obsolete()]
        internal static byte[] ReceiverReady(GXDLMSSettings settings, RequestTypes type)
        {
            GXReplyData reply = new GXReplyData() { MoreData = type };
            reply.GbtWindowSize = settings.GbtWindowSize;
            reply.BlockNumberAck = settings.BlockNumberAck;
            reply.BlockNumber = (UInt16)settings.BlockIndex;
            return ReceiverReady(settings, reply);
        }

        ///<summary>
        ///Generates an acknowledgment message, with which the server is informed to send next packets.
        ///</summary>
        ///<param name="reply">Reply data.</param>
        ///<returns>
        ///Acknowledgment message as byte array.
        ///</returns>
        internal static byte[] ReceiverReady(GXDLMSSettings settings, GXReplyData reply)
        {
            if (reply.MoreData == RequestTypes.None)
            {
                //Generate RR.
                byte id = settings.KeepAlive();
                if (settings.InterfaceType == InterfaceType.PlcHdlc)
                {
                    return GXDLMS.GetMacHdlcFrame(settings, id, 0, null);
                }
                return GetHdlcFrame(settings, id, null);
            }
            // Get next frame.
            if ((reply.MoreData & RequestTypes.Frame) != 0)
            {
                byte id = settings.ReceiverReady();
                if (settings.InterfaceType == InterfaceType.PlcHdlc)
                {
                    return GXDLMS.GetMacHdlcFrame(settings, id, 0, null);
                }
                return GetHdlcFrame(settings, id, null);
            }
            Command cmd = settings.Command;
            // Get next block.
            byte[][] data;
            if (reply.MoreData == RequestTypes.GBT)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(settings, 0, Command.GeneralBlockTransfer, 0, null, null, 0xff, Command.None);
                p.GbtWindowSize = reply.GbtWindowSize;
                p.blockNumberAck = reply.BlockNumber;
                p.blockIndex = settings.BlockIndex;
                data = GetLnMessages(p);
            }
            else
            {
                // Get next block.
                GXByteBuffer bb = new GXByteBuffer(4);
                if (settings.UseLogicalNameReferencing)
                {
                    bb.SetUInt32(settings.BlockIndex);
                }
                else
                {
                    bb.SetUInt16((UInt16)settings.BlockIndex);
                }
                settings.IncreaseBlockIndex();
                if (settings.UseLogicalNameReferencing)
                {
                    GXDLMSLNParameters p = new GXDLMSLNParameters(settings, 0, cmd, (byte)GetCommandType.NextDataBlock, bb, null, 0xff, Command.None);
                    data = GXDLMS.GetLnMessages(p);
                }
                else
                {
                    GXDLMSSNParameters p = new GXDLMSSNParameters(settings, cmd, 1, (byte)VariableAccessSpecification.BlockNumberAccess, bb, null);
                    data = GXDLMS.GetSnMessages(p);
                }
            }
            return data[0];
        }

        /// <summary>
        /// Get error description.
        /// </summary>
        /// <param name="error">Error number.</param>
        /// <returns>Error as plain text.</returns>
        internal static string GetDescription(ErrorCode error)
        {
#if !WINDOWS_UWP && !__MOBILE__
            string str = null;
            switch (error)
            {
                case ErrorCode.Ok:
                    str = "";
                    break;
                case ErrorCode.Rejected:
                    str = Properties.Resources.Rejected;
                    break;
                case ErrorCode.UnacceptableFrame:
                    str = Properties.Resources.UnacceptableFrame;
                    break;
                case ErrorCode.DisconnectMode:
                    str = Properties.Resources.DisconnectMode;
                    break;
                case ErrorCode.HardwareFault: //Access Error : Device reports a hardware fault
                    str = Gurux.DLMS.Properties.Resources.HardwareFaultTxt;
                    break;
                case ErrorCode.TemporaryFailure: //Access Error : Device reports a temporary failure
                    str = Gurux.DLMS.Properties.Resources.TemporaryFailureTxt;
                    break;
                case ErrorCode.ReadWriteDenied: // Access Error : Device reports Read-Write denied
                    str = Gurux.DLMS.Properties.Resources.ReadWriteDeniedTxt;
                    break;
                case ErrorCode.UndefinedObject: // Access Error : Device reports a undefined object
                    str = Gurux.DLMS.Properties.Resources.UndefinedObjectTxt;
                    break;
                case ErrorCode.InconsistentClass: // Access Error : Device reports a inconsistent Class or object
                    str = Gurux.DLMS.Properties.Resources.InconsistentClassTxt;
                    break;
                case ErrorCode.UnavailableObject: // Access Error : Device reports a unavailable object
                    str = Gurux.DLMS.Properties.Resources.UnavailableObjectTxt;
                    break;
                case ErrorCode.UnmatchedType: // Access Error : Device reports a unmatched type
                    str = Gurux.DLMS.Properties.Resources.UnmatchedTypeTxt;
                    break;
                case ErrorCode.AccessViolated: // Access Error : Device reports scope of access violated
                    str = Gurux.DLMS.Properties.Resources.AccessViolatedTxt;
                    break;
                case ErrorCode.DataBlockUnavailable: // Access Error : Data Block Unavailable.
                    str = Gurux.DLMS.Properties.Resources.DataBlockUnavailableTxt;
                    break;
                case ErrorCode.LongGetOrReadAborted: // Access Error : Long Get Or Read Aborted.
                    str = Gurux.DLMS.Properties.Resources.LongGetOrReadAbortedTxt;
                    break;
                case ErrorCode.NoLongGetOrReadInProgress: // Access Error : No Long Get Or Read In Progress.
                    str = Gurux.DLMS.Properties.Resources.NoLongGetOrReadInProgressTxt;
                    break;
                case ErrorCode.LongSetOrWriteAborted: // Access Error : Long Set Or Write Aborted.
                    str = Gurux.DLMS.Properties.Resources.LongSetOrWriteAbortedTxt;
                    break;
                case ErrorCode.NoLongSetOrWriteInProgress: // Access Error : No Long Set Or Write In Progress.
                    str = Gurux.DLMS.Properties.Resources.NoLongSetOrWriteInProgressTxt;
                    break;
                case ErrorCode.DataBlockNumberInvalid: // Access Error : Data Block Number Invalid.
                    str = Gurux.DLMS.Properties.Resources.DataBlockNumberInvalidTxt;
                    break;
                case ErrorCode.OtherReason: // Access Error : Other Reason.
                    str = Gurux.DLMS.Properties.Resources.OtherReasonTxt;
                    break;
                default:
                    str = Gurux.DLMS.Properties.Resources.UnknownErrorTxt;
                    break;
            }
            return str;
#else
#if WINDOWS_UWP
            return error.ToString();
#endif //WINDOWS_UWP
#if __MOBILE__
            string str = null;
            switch (error)
            {
                case ErrorCode.Ok:
                    str = "";
                    break;
                case ErrorCode.Rejected:
                    str = Resources.Rejected;
                    break;
                case ErrorCode.UnacceptableFrame:
                    str = Resources.UnacceptableFrame;
                    break;
                case ErrorCode.DisconnectMode:
                    str = Resources.DisconnectMode;
                    break;
                case ErrorCode.HardwareFault: //Access Error : Device reports a hardware fault
                    str = Resources.HardwareFaultTxt;
                    break;
                case ErrorCode.TemporaryFailure: //Access Error : Device reports a temporary failure
                    str = Resources.TemporaryFailureTxt;
                    break;
                case ErrorCode.ReadWriteDenied: // Access Error : Device reports Read-Write denied
                    str = Resources.ReadWriteDeniedTxt;
                    break;
                case ErrorCode.UndefinedObject: // Access Error : Device reports a undefined object
                    str = Resources.UndefinedObjectTxt;
                    break;
                case ErrorCode.InconsistentClass: // Access Error : Device reports a inconsistent Class or object
                    str = Resources.InconsistentClassTxt;
                    break;
                case ErrorCode.UnavailableObject: // Access Error : Device reports a unavailable object
                    str = Resources.UnavailableObjectTxt;
                    break;
                case ErrorCode.UnmatchedType: // Access Error : Device reports a unmatched type
                    str = Resources.UnmatchedTypeTxt;
                    break;
                case ErrorCode.AccessViolated: // Access Error : Device reports scope of access violated
                    str = Resources.AccessViolatedTxt;
                    break;
                case ErrorCode.DataBlockUnavailable: // Access Error : Data Block Unavailable.
                    str = Resources.DataBlockUnavailableTxt;
                    break;
                case ErrorCode.LongGetOrReadAborted: // Access Error : Long Get Or Read Aborted.
                    str = Resources.LongGetOrReadAbortedTxt;
                    break;
                case ErrorCode.NoLongGetOrReadInProgress: // Access Error : No Long Get Or Read In Progress.
                    str = Resources.NoLongGetOrReadInProgressTxt;
                    break;
                case ErrorCode.LongSetOrWriteAborted: // Access Error : Long Set Or Write Aborted.
                    str = Resources.LongSetOrWriteAbortedTxt;
                    break;
                case ErrorCode.NoLongSetOrWriteInProgress: // Access Error : No Long Set Or Write In Progress.
                    str = Resources.NoLongSetOrWriteInProgressTxt;
                    break;
                case ErrorCode.DataBlockNumberInvalid: // Access Error : Data Block Number Invalid.
                    str = Resources.DataBlockNumberInvalidTxt;
                    break;
                case ErrorCode.OtherReason: // Access Error : Other Reason.
                    str = Resources.OtherReasonTxt;
                    break;
                default:
                    str = Resources.UnknownErrorTxt;
                    break;
            }
            return str;
#endif //__MOBILE__
#endif
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        internal static void CheckInit(GXDLMSSettings settings)
        {
            if (settings.InterfaceType != InterfaceType.PDU)
            {
                if (settings.ClientAddress == 0)
                {
                    throw new ArgumentException("Invalid Client Address");
                }
                if (settings.ServerAddress == 0)
                {
                    throw new ArgumentException("Invalid Server Address");
                }
            }
        }

        internal static void AppendData(GXDLMSSettings settings, GXDLMSObject obj, int index, GXByteBuffer bb, Object value)
        {
            DataType tp = obj.GetDataType(index);
            if (tp == DataType.Array)
            {
                if (value is byte[])
                {
                    bb.Set((byte[])value);
                    return;
                }
                else if (value is GXByteBuffer)
                {
                    bb.Set((GXByteBuffer)value);
                    return;
                }
            }
            else
            {
                if (tp == DataType.None)
                {
                    tp = GXDLMSConverter.GetDLMSDataType(value);
                }
                else if (tp == DataType.OctetString && value is string)
                {
                    DataType ui = obj.GetUIDataType(index);
                    if (ui == DataType.String)
                    {
                        value = ASCIIEncoding.ASCII.GetBytes((string)value);
                    }
                    else if (ui == DataType.OctetString)
                    {
                        value = GXDLMSTranslator.HexToBytes((string)value);
                    }
                }
            }
            GXCommon.SetData(settings, bb, tp, value);
        }

        /// <summary>
        /// Get used glo message.
        /// </summary>
        /// <param name="cmd">Executed command.</param>
        /// <returns>Integer value of glo message.</returns>
        private static byte GetGloMessage(Command cmd)
        {
            switch (cmd)
            {
                case Command.ReadRequest:
                    cmd = Command.GloReadRequest;
                    break;
                case Command.GetRequest:
                    cmd = Command.GloGetRequest;
                    break;
                case Command.WriteRequest:
                    cmd = Command.GloWriteRequest;
                    break;
                case Command.SetRequest:
                    cmd = Command.GloSetRequest;
                    break;
                case Command.MethodRequest:
                    cmd = Command.GloMethodRequest;
                    break;
                case Command.ReadResponse:
                    cmd = Command.GloReadResponse;
                    break;
                case Command.GetResponse:
                    cmd = Command.GloGetResponse;
                    break;
                case Command.WriteResponse:
                    cmd = Command.GloWriteResponse;
                    break;
                case Command.SetResponse:
                    cmd = Command.GloSetResponse;
                    break;
                case Command.MethodResponse:
                    cmd = Command.GloMethodResponse;
                    break;
                case Command.DataNotification:
                    cmd = Command.GeneralGloCiphering;
                    break;
                case Command.ReleaseRequest:
                    cmd = Command.ReleaseRequest;
                    break;
                case Command.ReleaseResponse:
                    cmd = Command.ReleaseResponse;
                    break;
                case Command.AccessRequest:
                case Command.AccessResponse:
                    cmd = Command.GeneralCiphering;
                    break;
                default:
                    throw new GXDLMSException("Invalid GLO command.");
            }
            return (byte)cmd;
        }

        /// <summary>
        /// Get used ded message.
        /// </summary>
        /// <param name="cmd">Executed command.</param>
        /// <returns>Integer value of ded message.</returns>
        private static byte GetDedMessage(Command cmd)
        {
            switch (cmd)
            {
                case Command.GetRequest:
                    cmd = Command.DedGetRequest;
                    break;
                case Command.SetRequest:
                    cmd = Command.DedSetRequest;
                    break;
                case Command.MethodRequest:
                    cmd = Command.DedMethodRequest;
                    break;
                case Command.GetResponse:
                    cmd = Command.DedGetResponse;
                    break;
                case Command.SetResponse:
                    cmd = Command.DedSetResponse;
                    break;
                case Command.MethodResponse:
                    cmd = Command.DedMethodResponse;
                    break;
                case Command.DataNotification:
                    cmd = Command.GeneralDedCiphering;
                    break;
                case Command.ReleaseRequest:
                    cmd = Command.ReleaseRequest;
                    break;
                case Command.ReleaseResponse:
                    cmd = Command.ReleaseResponse;
                    break;
                case Command.AccessRequest:
                case Command.AccessResponse:
                    cmd = Command.GeneralDedCiphering;
                    break;
                default:
                    throw new GXDLMSException("Invalid DED command.");
            }
            return (byte)cmd;
        }

        /// <summary>
        /// Add LLC bytes to generated message.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data">Data where bytes are added.</param>
        internal static void AddLLCBytes(GXDLMSSettings settings, GXByteBuffer data)
        {
            if (settings.IsServer)
            {
                data.Set(0, GXCommon.LLCReplyBytes);
            }
            else
            {
                data.Set(0, GXCommon.LLCSendBytes);
            }
        }

        /// <summary>
        /// Check is all data fit to one data block.
        /// </summary>
        /// <param name="p">LN parameters.</param>
        /// <param name="reply">Generated reply.</param>
        private static void MultipleBlocks(GXDLMSLNParameters p, GXByteBuffer reply, bool ciphering)
        {
            //Check is all data fit to one message if data is given.
            int len = 0;
            if (p.data != null)
            {
                len = p.data.Available;
            }
            if (p.attributeDescriptor != null)
            {
                len += p.attributeDescriptor.Size;
            }
            if (ciphering)
            {
                len += CipheringHeaderSize;
            }
            //If system title is sent.
            if ((p.settings.NegotiatedConformance & Conformance.GeneralProtection) != 0)
            {
                len += 9;
            }
            len += GetSigningSize(p);
            if (!p.multipleBlocks)
            {
                //Add command type and invoke and priority.
                p.multipleBlocks = 2 + reply.Size + len > p.settings.MaxPduSize;
            }
            if (p.multipleBlocks)
            {
                //Add command type and invoke and priority.
                p.lastBlock = !(8 + reply.Available + len > p.settings.MaxPduSize);
            }
            if (p.lastBlock)
            {
                //Add command type and invoke and priority.
                p.lastBlock = !(8 + reply.Available + len > p.settings.MaxPduSize);
            }
        }

        static bool IsGloMessage(Command cmd)
        {
            return cmd == Command.GloGetRequest || cmd == Command.GloSetRequest || cmd == Command.GloMethodRequest;
        }

        private static byte[] GetBlockCipherKey(GXDLMSSettings settings)
        {
            if (settings.EphemeralBlockCipherKey != null)
            {
                return settings.EphemeralBlockCipherKey;
            }
            return settings.Cipher.BlockCipherKey;

        }

        private static byte[] GetAuthenticationKey(GXDLMSSettings settings)
        {
            if (settings.EphemeralAuthenticationKey != null)
            {
                return settings.EphemeralAuthenticationKey;
            }
            return settings.Cipher.AuthenticationKey;
        }

        internal static AesGcmParameter GetCipheringParameters(GXDLMSLNParameters p)
        {
            byte cmd;
            byte[] key;
            GXICipher cipher = p.settings.Cipher;
            //If client.
            if (p.cipheredCommand == Command.None)
            {
                //General protection can be used with pre-established connections.
                if (((p.settings.Connected & ConnectionState.Dlms) == 0 ||
                    (p.settings.NegotiatedConformance & Conformance.GeneralProtection) == 0) &&
                    (p.settings.PreEstablishedSystemTitle == null || p.settings.PreEstablishedSystemTitle.Length == 0 || (p.settings.ProposedConformance & Conformance.GeneralProtection) == 0))
                {
                    if (cipher.DedicatedKey != null && (p.settings.Connected & ConnectionState.Dlms) != 0)
                    {
                        cmd = GetDedMessage(p.command);
                        key = cipher.DedicatedKey;
                    }
                    else
                    {
                        cmd = GetGloMessage(p.command);
                        key = GetBlockCipherKey(p.settings);
                    }
                }
                else
                {
                    if (p.settings.Cipher.DedicatedKey != null)
                    {
                        cmd = (byte)Command.GeneralDedCiphering;
                        key = cipher.DedicatedKey;
                    }
                    else
                    {
                        cmd = (byte)Command.GeneralGloCiphering;
                        key = GetBlockCipherKey(p.settings);
                    }
                }
            }
            else //If server.
            {
                if (p.cipheredCommand == Command.GeneralDedCiphering)
                {
                    cmd = (byte)Command.GeneralDedCiphering;
                    key = cipher.DedicatedKey;
                }
                else if (p.cipheredCommand == Command.GeneralGloCiphering)
                {
                    cmd = (byte)Command.GeneralGloCiphering;
                    key = GetBlockCipherKey(p.settings);
                }
                else if (p.settings.Cipher.DedicatedKey == null || IsGloMessage(p.cipheredCommand))
                {
                    cmd = GetGloMessage(p.command);
                    key = GetBlockCipherKey(p.settings);
                }
                else
                {
                    cmd = GetDedMessage(p.command);
                    key = cipher.DedicatedKey;
                }
            }
            AesGcmParameter s = new AesGcmParameter(cmd,
                cipher.Security,
                cipher.SecuritySuite,
                cipher.InvocationCounter,
                cipher.SystemTitle,
                key,
                GetAuthenticationKey(p.settings));
            s.IgnoreSystemTitle = p.settings.Standard == Standard.Italy;
            s.RecipientSystemTitle = p.settings.SourceSystemTitle;
            return s;
        }

        internal static byte[] Cipher0(GXDLMSLNParameters p, byte[] data)
        {
            //If external Hardware Security Module is used.
            byte[] ret = p.settings.Crypt(CertificateType.DigitalSignature, data, true);
            if (ret == null)
            {
                ret = GXCiphering.Encrypt(GetCipheringParameters(p), data);
            }
            ++p.settings.Cipher.InvocationCounter;
            return ret;
        }

        /// <summary>
        /// Should the message sign.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        static private bool ShoudSign(GXDLMSLNParameters p)
        {
            bool signing = p.settings.Cipher != null && 
                p.settings.Cipher.Signing != Signing.None;
            if (!signing)
            {
                //Association LN V3 and signing is not needed.
                if (p.settings.IsServer)
                {
                    signing = (p.AccessMode & (int)(AccessMode3.DigitallySignedResponse)) != 0;
                }
                else
                {
                    signing = (p.AccessMode & (int)(AccessMode3.DigitallySignedRequest)) != 0;
                }
            }
            return signing;
        }

        /// <summary>
        /// Cipher using security suite 1 or 2.
        /// </summary>
        /// <param name="p">LN settings.</param>
        /// <param name="data">Data to encrypt</param>
        /// <returns></returns>
        private static byte[] Cipher1(GXDLMSLNParameters p, byte[] data, bool sign)
        {
            if (!sign && p.settings.Cipher.Signing == Signing.GeneralSigning)
            {
                sign = true;
            }
            byte keyid;
            switch (p.settings.Cipher.Signing)
            {
                case Signing.OnePassDiffieHellman:
                    keyid = 1;
                    break;
                case Signing.StaticUnifiedModel:
                    keyid = 2;
                    break;
                default:
                    keyid = 0;
                    break;
            }
            GXICipher c = p.settings.Cipher;
            byte sc = 0;
            if (p.settings.SourceSystemTitle == null && p.settings.PreEstablishedSystemTitle == null)
            {
                throw new ArgumentOutOfRangeException("Invalid Recipient System Title.");
            }
            if (c.SystemTitle == null)
            {
                throw new ArgumentOutOfRangeException("Invalid System Title.");
            }
            switch (c.Security)
            {
                case Security.Authentication:
                    sc = 0x10;
                    break;
                case Security.AuthenticationEncryption:
                    sc = 0x30;
                    break;
                case Security.Encryption:
                    sc = 0x20;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid security.");
                    break;
            }
            AlgorithmId algorithmID;
            switch (c.SecuritySuite)
            {
                case SecuritySuite.Suite1:
                    algorithmID = AlgorithmId.AesGcm128;
                    sc |= 1;
                    break;
                case SecuritySuite.Suite2:
                    algorithmID = AlgorithmId.AesGcm256;
                    sc |= 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid security suite.");
            }
            GXByteBuffer tmp2 = new GXByteBuffer();
            byte[] z = null;
            GXPrivateKey key;
            GXPublicKey pub;
            if (!sign)
            {
                //If external Hardware Security Module is used.
                byte[] ret = p.settings.Crypt(CertificateType.KeyAgreement, data, true);
                if (ret != null)
                {
                    return ret;
                }
                key = c.KeyAgreementKeyPair.Value;
                pub = c.KeyAgreementKeyPair.Key;
                if (key == null)
                {
                    key = (GXPrivateKey)p.settings.GetKey(CertificateType.KeyAgreement, p.settings.Cipher.SystemTitle, true);
                    c.KeyAgreementKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                }
                if (pub == null)
                {
                    pub = (GXPublicKey)p.settings.GetKey(CertificateType.KeyAgreement, p.settings.SourceSystemTitle, false);
                    c.KeyAgreementKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                }
                if (keyid == 1)
                {
                    //Generate ephemeral key pair for each transaction.
                    c.EphemeralKeyPair = GXEcdsa.GenerateKeyPair(c.SecuritySuite == SecuritySuite.Suite1 ? Ecc.P256 : Ecc.P384);
                    GXEcdsa ka = new GXEcdsa(c.EphemeralKeyPair.Value);
                    System.Diagnostics.Debug.WriteLine("Private ephemeral: " + c.EphemeralKeyPair.Value.ToHex());
                    System.Diagnostics.Debug.WriteLine("Public ephemeral: " + c.EphemeralKeyPair.Key.ToHex());
                    System.Diagnostics.Debug.WriteLine("Public agreement key: " + c.KeyAgreementKeyPair.Key.ToHex());
                    z = ka.GenerateSecret(c.KeyAgreementKeyPair.Key);
                }
                else if (keyid == 2)
                {
                    System.Diagnostics.Debug.WriteLine("Private agreement key: " + key.ToHex());
                    System.Diagnostics.Debug.WriteLine("Public agreement key: " + pub.ToHex());
                    System.Diagnostics.Debug.WriteLine("Authentication key: " + GXDLMSTranslator.ToHex(c.AuthenticationKey));
                    GXEcdsa ka = new GXEcdsa(key);
                    z = ka.GenerateSecret(pub);
                    if (c.TransactionId == null)
                    {
                        tmp2.SetUInt8(0x0);
                    }
                    else
                    {
                        tmp2.SetUInt8((byte)c.TransactionId.Length);
                        tmp2.Set(c.TransactionId);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Invalid key-id.");
                }
            }
            else
            {
                //If external Hardware Security Module is used.
                byte[] ret = p.settings.Crypt(CertificateType.DigitalSignature, data, true);
                if (ret != null)
                {
                    return ret;
                }
                key = c.SigningKeyPair.Value;
                pub = c.SigningKeyPair.Key;
                if (key == null)
                {
                    key = (GXPrivateKey)p.settings.GetKey(CertificateType.DigitalSignature, p.settings.Cipher.SystemTitle, true);
                    if (key == null)
                    {
                        throw new ArgumentOutOfRangeException("Can't find digical signature for Client system title: " +
                            GXCommon.ToHex(p.settings.Cipher.SystemTitle, true));
                    }
                    c.KeyAgreementKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                }
                if (pub == null)
                {
                    pub = (GXPublicKey)p.settings.GetKey(CertificateType.DigitalSignature, p.settings.SourceSystemTitle, false);
                    if (pub == null)
                    {
                        throw new ArgumentOutOfRangeException("Can't find digical signature for Server system title: " +
                            GXCommon.ToHex(p.settings.SourceSystemTitle, true));
                    }
                    c.KeyAgreementKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                }
                System.Diagnostics.Debug.WriteLine("Private signing key: " + key.ToHex());
                System.Diagnostics.Debug.WriteLine("Public signing key: " + pub.ToHex());
            }
            tmp2.Set(p.settings.SourceSystemTitle);
            if (z != null)
            {
                System.Diagnostics.Debug.WriteLine("Shared secret: " + GXCommon.ToHex(z, true));
            }
            AesGcmParameter s;
            if (sign)
            {
                s = GetCipheringParameters(p);
            }
            else
            {
                GXByteBuffer kdf = new GXByteBuffer();
                kdf.Set(GXSecure.GenerateKDF(c.SecuritySuite, z, algorithmID, c.SystemTitle, tmp2.Array(), null, null));
                System.Diagnostics.Debug.WriteLine("kdf: " + kdf.ToString());
                s = new AesGcmParameter(sc,
                p.settings,
                c.Security,
                c.SecuritySuite,
                p.settings.Cipher.InvocationCounter,
                // KDF
                kdf.SubArray(0, 16),
                // Authentication key.
                c.AuthenticationKey,
                // Originator system title.
                c.SystemTitle,
                // recipient system title.
                p.settings.SourceSystemTitle,
                // Date time
                null,
                // Other information.
                null);
            }
            GXByteBuffer reply = new GXByteBuffer();
            if (sign)
            {
                reply.SetUInt8(Command.GeneralSigning);
            }
            else
            {
                reply.SetUInt8(Command.GeneralCiphering);
            }
            if (c.TransactionId == null)
            {
                GXCommon.SetObjectCount(0, reply);
            }
            else
            {
                GXCommon.SetObjectCount(c.TransactionId.Length, reply);
                reply.Set(c.TransactionId);
            }
            GXCommon.SetObjectCount(s.SystemTitle.Length, reply);
            reply.Set(s.SystemTitle);
            GXCommon.SetObjectCount(s.RecipientSystemTitle.Length, reply);
            reply.Set(s.RecipientSystemTitle);
            // date-time not present.
            reply.SetUInt8(0);
            // other-information not present
            reply.SetUInt8(0);
            if (!sign)
            {
                // optional flag
                reply.SetUInt8(1);
                // agreed-key CHOICE
                reply.SetUInt8(2);
                // key-parameters
                reply.SetUInt8(1);
                reply.SetUInt8(keyid);
            }
            if (keyid == 1)
            {
                key = c.SigningKeyPair.Value;
                pub = c.SigningKeyPair.Key;
                if (key == null)
                {
                    key = (GXPrivateKey)p.settings.GetKey(CertificateType.DigitalSignature, p.settings.Cipher.SystemTitle, true);
                    c.SigningKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                }
                if (pub == null)
                {
                    pub = (GXPublicKey)p.settings.GetKey(CertificateType.DigitalSignature, p.settings.SourceSystemTitle, false);
                    c.SigningKeyPair = new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, key);
                }
                // key-ciphered-data
                GXCommon.SetObjectCount(0x80, reply);
                // Ephemeral public key client.
                reply.Set(c.EphemeralKeyPair.Key.RawValue, 1, c.EphemeralKeyPair.Key.RawValue.Length - 1);

                // Ephemeral Public Key Signature.
                reply.Set(GXSecure.GetEphemeralPublicKeySignature(keyid,
                        c.EphemeralKeyPair.Key, c.SigningKeyPair.Value));
            }
            else if (!sign)
            {
                reply.SetUInt8(0);
            }
            // ciphered-content
            s.Type = (CountType.Data | CountType.Tag);
            System.Diagnostics.Debug.WriteLine("Data: " + GXDLMSTranslator.ToHex(data));
            byte[] tmp = GXCiphering.Encrypt(s, data);
            //Content length is not add for the signed data.
            GXByteBuffer signedData = new GXByteBuffer();
            signedData.Set(reply.Data, 1, reply.Size - 1);
            if (c.Security != Security.None)
            {
                if (sign)
                {
                    //Content length is not add for the signed data.
                    GXCommon.SetObjectCount(6 + GXCommon.GetObjectCountSizeInBytes(5 + tmp.Length) + tmp.Length, reply);
                    //Add ciphered command.
                    if (p.settings.Cipher.DedicatedKey == null)
                    {
                        reply.SetUInt8(GetGloMessage(p.command));
                        signedData.SetUInt8(GetGloMessage(p.command));
                    }
                    else
                    {
                        reply.SetUInt8(GetDedMessage(p.command));
                        signedData.SetUInt8(GetDedMessage(p.command));
                    }
                }
                // Len
                GXCommon.SetObjectCount(5 + tmp.Length, reply);
                GXCommon.SetObjectCount(5 + tmp.Length, signedData);
                // Add SC
                reply.SetUInt8(sc);
                signedData.SetUInt8(sc);
                // Add IC.
                reply.SetUInt32(p.settings.Cipher.InvocationCounter);
                signedData.SetUInt32(p.settings.Cipher.InvocationCounter);
            }
            else if (!sign)
            {
                // Len
                GXCommon.SetObjectCount(tmp.Length, reply);
            }
            ++p.settings.Cipher.InvocationCounter;
            reply.Set(tmp);
            signedData.Set(tmp);
            if (sign)
            {
                // Signature
                GXEcdsa ecdsa = new GXEcdsa(key);
                System.Diagnostics.Debug.WriteLine("Counting signature: " + signedData);
                byte[] signature = ecdsa.Sign(signedData.Array());
                GXCommon.SetObjectCount(signature.Length, reply);
                reply.Set(signature);
            }

            System.Diagnostics.Debug.WriteLine("Encrypted:" + reply.ToString());
            return reply.Array();
        }

        /// <summary>
        /// Return amount of the bytes that signing requires.
        /// </summary>
        /// <returns></returns>
        static private byte GetSigningSize(GXDLMSLNParameters p)
        {
            byte size = 0;
            //If signing is used.
            if (p.settings.Cipher != null && p.settings.Cipher.Signing == Signing.GeneralSigning)
            {
                if (p.settings.Cipher.SecuritySuite == SecuritySuite.Suite1)
                {
                    size = 65;
                }
                else if (p.settings.Cipher.SecuritySuite == SecuritySuite.Suite2)
                {
                    size = 99;
                }
            }
            return size;
        }

        /// <summary>
        /// Get next logical name PDU.
        /// </summary>
        /// <param name="p">LN parameters.</param>
        /// <param name="reply">Generated message.</param>
        internal static void GetLNPdu(GXDLMSLNParameters p, GXByteBuffer reply)
        {
            bool ciphering = p.command != Command.Aarq && p.command != Command.Aare &&
                (p.settings.IsCiphered(true) || p.cipheredCommand != Command.None ||
                (p.settings.Cipher != null && p.settings.Cipher.Signing == Signing.GeneralSigning));            
            int len = 0;
            if (p.command == Command.Aarq)
            {
                if (p.settings.Gateway != null && p.settings.Gateway.PhysicalDeviceAddress != null)
                {
                    reply.SetUInt8(Command.GatewayRequest);
                    reply.SetUInt8(p.settings.Gateway.NetworkId);
                    reply.SetUInt8((byte)p.settings.Gateway.PhysicalDeviceAddress.Length);
                    reply.Set(p.settings.Gateway.PhysicalDeviceAddress);
                }
                reply.Set(p.attributeDescriptor);
            }
            else
            {
                // Add command.
                if (p.command != Command.GeneralBlockTransfer)
                {
                    reply.SetUInt8((byte)p.command);
                }
                if (p.command == Command.EventNotification ||
                    p.command == Command.DataNotification ||
                    p.command == Command.AccessRequest ||
                    p.command == Command.AccessResponse)
                {
                    // Add Long-Invoke-Id-And-Priority
                    if (p.command != Command.EventNotification)
                    {
                        if (p.InvokeId != 0)
                        {
                            reply.SetUInt32(p.InvokeId);
                        }
                        else
                        {
                            reply.SetUInt32(GetLongInvokeIDPriority(p.settings));
                        }
                    }
                    // Add date time.
                    if (p.time == null || p.time.Value.DateTime == DateTime.MinValue || p.time.Value.DateTime == DateTime.MaxValue ||
                            p.time.Value.LocalDateTime == DateTime.MinValue || p.time.Value.LocalDateTime == DateTime.MaxValue)
                    {
                        reply.SetUInt8(DataType.None);
                    }
                    else
                    {
                        // Data is send in octet string. Remove data type except from Event Notification.
                        int pos = reply.Size;
                        GXCommon.SetData(p.settings, reply, DataType.OctetString, p.time);
                        if (p.command != Command.EventNotification)
                        {
                            reply.Move(pos + 1, pos, reply.Size - pos - 1);
                        }
                    }
                    MultipleBlocks(p, reply, ciphering);
                }
                else if (p.command != Command.ReleaseRequest && p.command != Command.ExceptionResponse)
                {
                    //Get request size can be bigger than PDU size.
                    if (p.command != Command.GetRequest &&
                            p.data != null && p.data.Size != 0)
                    {
                        MultipleBlocks(p, reply, ciphering);
                    }
                    //Change Request type if Set request and multiple blocks is needed.
                    if (p.command == Command.SetRequest)
                    {
                        if (p.multipleBlocks && (p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) == 0)
                        {
                            if (p.requestType == (byte)SetRequestType.Normal)
                            {
                                p.requestType = (byte)SetRequestType.FirstDataBlock;
                            }
                            else if (p.requestType == (byte)SetRequestType.FirstDataBlock)
                            {
                                p.requestType = (byte)SetRequestType.WithDataBlock;
                            }
                        }
                    }
                    //Change Request type if action request and multiple blocks is needed.
                    else if (p.command == Command.MethodRequest)
                    {
                        if (p.multipleBlocks && (p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) == 0)
                        {
                            if (p.requestType == (byte)ActionRequestType.Normal)
                            {
                                //Remove Method Invocation Parameters tag.
                                --p.attributeDescriptor.Size;
                                p.requestType = (byte)ActionRequestType.WithFirstBlock;
                            }
                            else if (p.requestType == (byte)ActionRequestType.WithFirstBlock)
                            {
                                p.requestType = (byte)ActionRequestType.WithBlock;
                            }
                        }
                    }
                    //Change Request type if action request and multiple blocks is needed.
                    else if (p.command == Command.MethodResponse)
                    {
                        if (p.multipleBlocks && (p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) == 0)
                        {
                            //There is no status fiel in action resonse.
                            p.status = 0xFF;
                            if (p.requestType == (byte)ActionResponseType.Normal)
                            {
                                //Remove Method Invocation Parameters tag.
                                ++p.data.Position;
                                ++p.data.Position;
                                p.requestType = (byte)ActionResponseType.WithBlock;
                            }
                            else if (p.requestType == (byte)ActionResponseType.WithBlock && p.data.Available == 0)
                            {
                                //If server asks next part of PDU.
                                p.requestType = (byte)ActionResponseType.NextBlock;
                            }
                        }
                    }
                    //Change request type If get response and multiple blocks is needed.
                    else if (p.command == Command.GetResponse)
                    {
                        if (p.multipleBlocks && (p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) == 0)
                        {
                            if (p.requestType == 1)
                            {
                                p.requestType = 2;
                            }
                        }
                    }
                    if (p.command != Command.GeneralBlockTransfer)
                    {
                        reply.SetUInt8(p.requestType);
                        // Add Invoke Id And Priority.
                        if (p.InvokeId != 0)
                        {
                            reply.SetUInt8((byte)p.InvokeId);
                        }
                        else
                        {
                            reply.SetUInt8(GetInvokeIDPriority(p.settings, p.settings.AutoIncreaseInvokeID));
                        }
                    }
                }

                //Add attribute descriptor.
                reply.Set(p.attributeDescriptor);
                //If multiple blocks.
                if (p.multipleBlocks && (p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) == 0)
                {
                    if (p.command != Command.SetResponse && (p.command != Command.MethodResponse || p.data.Size != 0))
                    {
                        // Is last block.
                        if (p.lastBlock)
                        {
                            reply.SetUInt8(1);
                            p.settings.Count = p.settings.Index = 0;
                        }
                        else
                        {
                            reply.SetUInt8(0);
                        }
                    }
                    // Block index.
                    reply.SetUInt32(p.blockIndex);
                    ++p.blockIndex;
                    //Add status if reply.
                    if (p.status != 0xFF)
                    {
                        if (p.status != 0 && p.command == Command.GetResponse)
                        {
                            reply.SetUInt8(1);
                        }
                        reply.SetUInt8(p.status);
                    }
                    //Block size.
                    if (p.data != null)
                    {
                        len = p.data.Available;
                    }
                    else
                    {
                        len = 0;
                    }
                    int totalLength = len + reply.Available;
                    if (ciphering)
                    {
                        totalLength += CipheringHeaderSize;
                    }
                    totalLength += GetSigningSize(p);

                    if (totalLength > p.settings.MaxPduSize)
                    {
                        len = p.settings.MaxPduSize - reply.Available;
                        if (len < 0)
                        {
                            len = p.settings.MaxPduSize;
                        }
                        if (ciphering)
                        {
                            len -= CipheringHeaderSize;
                        }
                        len -= GetSigningSize(p);
                        len -= GXCommon.GetObjectCountSizeInBytes(len);
                    }
                    //If server is not asking the next block.
                    if (!(len == 0 && p.command == Command.MethodResponse && p.requestType == (byte)ActionResponseType.NextBlock))
                    {
                        GXCommon.SetObjectCount(len, reply);
                        reply.Set(p.data, len);
                    }
                }

                //Add data that fits to one block.
                if (len == 0)
                {
                    //Add status if reply.
                    if (p.status != 0xFF && p.command != Command.GeneralBlockTransfer)
                    {
                        if (p.status != 0 && p.command == Command.GetResponse)
                        {
                            reply.SetUInt8(1);
                        }
                        reply.SetUInt8(p.status);
                    }
                    if (p.data != null && p.data.Size != 0)
                    {
                        len = p.data.Size - p.data.Position;
                        if (p.settings.Gateway != null && p.settings.Gateway.PhysicalDeviceAddress != null)
                        {
                            if (3 + len + p.settings.Gateway.PhysicalDeviceAddress.Length > p.settings.MaxPduSize)
                            {
                                len -= (3 + p.settings.Gateway.PhysicalDeviceAddress.Length);
                            }
                            GXByteBuffer tmp = new GXByteBuffer(reply);
                            reply.Size = 0;

                            if (p.settings.IsServer && p.command != Command.DataNotification &&
                                p.command != Command.EventNotification && p.command != Command.InformationReport)
                            {
                                reply.SetUInt8(Command.GatewayResponse);
                            }
                            else
                            {
                                reply.SetUInt8(Command.GatewayRequest);
                            }
                            reply.SetUInt8(p.settings.Gateway.NetworkId);
                            reply.SetUInt8((byte)p.settings.Gateway.PhysicalDeviceAddress.Length);
                            reply.Set(p.settings.Gateway.PhysicalDeviceAddress);
                            reply.Set(tmp);
                        }
                        //Get request size can be bigger than PDU size.
                        if ((p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) != 0)
                        {
                            if (7 + len + reply.Size > p.settings.MaxPduSize)
                            {
                                len = p.settings.MaxPduSize - reply.Size - 7;
                            }
                            //Cipher data only once.
                            if (ciphering && p.command != Command.GeneralBlockTransfer)
                            {
                                byte[] tmp;
                                reply.Set(p.data);
                                bool sign = ShoudSign(p);
                                if ((p.settings.Connected & ConnectionState.Dlms) == 0 ||
                                     !sign)
                                {
                                    tmp = Cipher0(p, reply.Array());
                                }
                                else
                                {
                                    tmp = Cipher1(p, reply.Array(), sign);
                                }
                                p.data.Size = 0;
                                p.data.Set(tmp);
                                reply.Size = 0;
                                len = p.data.Size;
                                if (7 + len > p.settings.MaxPduSize)
                                {
                                    len = p.settings.MaxPduSize - 7;
                                }
                                len -= GetSigningSize(p);
                                ciphering = false;
                            }
                        }
                        else if (p.command != Command.GetRequest && len + reply.Size > p.settings.MaxPduSize)
                        {
                            len = p.settings.MaxPduSize - reply.Size;
                            len -= GetSigningSize(p);
                        }
                        reply.Set(p.data, len);
                    }
                    else if ((p.settings.Gateway != null && p.settings.Gateway.PhysicalDeviceAddress != null) &&
                        !(p.command == Command.GeneralBlockTransfer || (p.multipleBlocks && (p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) != 0)))
                    {
                        GXByteBuffer tmp = new GXByteBuffer(reply);
                        reply.Size = 0;
                        reply.SetUInt8(Command.GatewayRequest);
                        reply.SetUInt8(p.settings.Gateway.NetworkId);
                        reply.SetUInt8((byte)p.settings.Gateway.PhysicalDeviceAddress.Length);
                        reply.Set(p.settings.Gateway.PhysicalDeviceAddress);
                        reply.Set(tmp);
                    }
                }
                if (reply.Size != 0 && p.command != Command.GeneralBlockTransfer && p.settings.CryptoNotifier != null && p.settings.CryptoNotifier.pdu != null)
                {
                    p.settings.CryptoNotifier.pdu(p.settings.CryptoNotifier, reply.Array());
                }
                if (ciphering && reply.Size != 0 && p.command != Command.ReleaseRequest && (!p.multipleBlocks || (p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) == 0))
                {
                    //GBT ciphering is done for all the data, not just block.
                    byte[] tmp;
                    bool sign = ShoudSign(p);
                    if ((p.settings.Connected & ConnectionState.Dlms) == 0 ||
                        !sign)
                    {
                        tmp = Cipher0(p, reply.Array());
                    }
                    else
                    {
                        tmp = Cipher1(p, reply.Array(), sign);
                    }
                    reply.Size = 0;
                    reply.Set(tmp);
                }
                if (p.command == Command.GeneralBlockTransfer || (p.multipleBlocks && (p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) != 0))
                {
                    GXByteBuffer bb = new GXByteBuffer();
                    bb.Set(reply);
                    reply.Clear();
                    reply.SetUInt8((byte)Command.GeneralBlockTransfer);
                    byte value = 0;
                    // Is last block
                    if (p.lastBlock)
                    {
                        value = 0x80;
                    }
                    if (p.Streaming)
                    {
                        value |= 0x40;
                    }
                    value |= p.GbtWindowSize;
                    reply.SetUInt8(value);
                    // Set block number sent.
                    reply.SetUInt16((UInt16)p.blockIndex);
                    ++p.blockIndex;
                    // Set block number acknowledged
                    if (p.command != Command.DataNotification && p.blockNumberAck != 0)
                    {
                        // Set block number acknowledged
                        reply.SetUInt16(p.blockNumberAck);
                        ++p.blockNumberAck;
                    }
                    else
                    {
                        p.blockNumberAck = UInt16.MaxValue;
                        reply.SetUInt16(0);
                    }
                    //Add data length.
                    GXCommon.SetObjectCount(bb.Size, reply);
                    reply.Set(bb);
                    ++p.blockNumberAck;
                    if (p.command != Command.GeneralBlockTransfer)
                    {
                        p.command = Command.GeneralBlockTransfer;
                        p.blockNumberAck = (UInt16)(p.settings.BlockNumberAck + 1);
                    }
                    if (p.settings.Gateway != null && p.settings.Gateway.PhysicalDeviceAddress != null)
                    {
                        GXByteBuffer tmp = new GXByteBuffer(reply);
                        reply.Size = 0;
                        reply.SetUInt8(Command.GatewayRequest);
                        reply.SetUInt8(p.settings.Gateway.NetworkId);
                        reply.SetUInt8((byte)p.settings.Gateway.PhysicalDeviceAddress.Length);
                        reply.Set(p.settings.Gateway.PhysicalDeviceAddress);
                        reply.Set(tmp);
                    }
                }
            }
            if (UseHdlc(p.settings.InterfaceType))
            {
                AddLLCBytes(p.settings, reply);
            }
        }

        /// <summary>
        ///  Get all Logical name messages. Client uses this to generate messages.
        /// </summary>
        /// <param name="p">LN settings.</param>
        /// <returns>Generated messages.</returns>
        internal static byte[][] GetLnMessages(GXDLMSLNParameters p)
        {
            GXByteBuffer reply = new GXByteBuffer();
            List<byte[]> messages = new List<byte[]>();
            byte frame = 0;
            if (p.command == Command.DataNotification || p.command == Command.EventNotification)
            {
                frame = 0x13;
            }
            do
            {
                GetLNPdu(p, reply);
                p.lastBlock = true;
                if (p.attributeDescriptor == null)
                {
                    ++p.settings.BlockIndex;
                }
                if (p.command == Command.Aarq && p.command == Command.GetRequest)
                {
                    System.Diagnostics.Debug.Assert(!(p.settings.MaxPduSize < reply.Size));
                }
                while (reply.Position != reply.Size)
                {
                    switch (p.settings.InterfaceType)
                    {
                        case InterfaceType.WRAPPER:
                            messages.Add(GXDLMS.GetWrapperFrame(p.settings, p.command, reply));
                            break;
                        case InterfaceType.HDLC:
                        case InterfaceType.HdlcWithModeE:
                            messages.Add(GXDLMS.GetHdlcFrame(p.settings, frame, reply));
                            if (reply.Position != reply.Size)
                            {
                                frame = p.settings.NextSend(false);
                            }
                            break;
                        case InterfaceType.PDU:
                            messages.Add(reply.Array());
                            reply.Position = reply.Size;
                            break;
                        case InterfaceType.Plc:
                            messages.Add(GXDLMS.GetPlcFrame(p.settings, 0x90, reply));
                            break;
                        case InterfaceType.PlcHdlc:
                            messages.Add(GXDLMS.GetMacHdlcFrame(p.settings, frame, 0, reply));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("InterfaceType");
                    }
                }
                reply.Clear();
                frame = 0;
            }
            while (p.data != null && p.data.Position != p.data.Size);
            return messages.ToArray();
        }

        /// <summary>
        /// Get all Short Name messages. Client uses this to generate messages.
        /// </summary>
        /// <param name="p">DLMS SN Parameters.</param>
        /// <returns>Generated messages.</returns>
        internal static byte[][] GetSnMessages(GXDLMSSNParameters p)
        {
            GXByteBuffer reply = new GXByteBuffer();
            List<byte[]> messages = new List<byte[]>();
            byte frame = 0x0;
            if (p.command == Command.InformationReport ||
                p.command == Command.DataNotification)
            {
                if ((p.settings.Connected & ConnectionState.Dlms) != 0)
                {
                    //If connection is established.
                    frame = 0x13;
                }
                else
                {
                    frame = 0x3;
                }
            }
            do
            {
                GetSNPdu(p, reply);
                if (p.command != Command.Aarq && p.command != Command.Aare)
                {
                    System.Diagnostics.Debug.Assert(!(p.settings.MaxPduSize < reply.Size));
                }
                //Command is not add to next PDUs.
                while (reply.Position != reply.Size)
                {
                    if (p.settings.InterfaceType == Enums.InterfaceType.WRAPPER)
                    {
                        messages.Add(GXDLMS.GetWrapperFrame(p.settings, p.command, reply));
                    }
                    else if (p.settings.InterfaceType == Enums.InterfaceType.HDLC ||
                        p.settings.InterfaceType == Enums.InterfaceType.HdlcWithModeE)
                    {
                        messages.Add(GXDLMS.GetHdlcFrame(p.settings, frame, reply));
                        if (reply.Position != reply.Size)
                        {
                            frame = p.settings.NextSend(false);
                        }
                    }
                    else if (p.settings.InterfaceType == Enums.InterfaceType.PDU)
                    {
                        messages.Add(reply.Array());
                        break;
                    }
                    else if (p.settings.InterfaceType == Enums.InterfaceType.Plc)
                    {
                        int val;
                        if (p.command == Command.Aarq)
                        {
                            val = 0x90;
                        }
                        else
                        {
                            val = p.settings.Plc.InitialCredit << 5;
                            val |= p.settings.Plc.CurrentCredit << 2;
                            val |= p.settings.Plc.DeltaCredit & 0x3;
                        }
                        messages.Add(GXDLMS.GetPlcFrame(p.settings, (byte)val, reply));
                        break;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("InterfaceType");
                    }
                }
                reply.Clear();
                frame = 0;
            } while (p.data != null && p.data.Position != p.data.Size);
            return messages.ToArray();
        }

        static int AppendMultipleSNBlocks(GXDLMSSNParameters p, GXByteBuffer reply)
        {
            bool ciphering = p.settings.IsCiphered(false);
            int hSize = reply.Size + 3;
            //Add LLC bytes.
            if (p.command == Command.WriteRequest ||
                    p.command == Command.ReadRequest)
            {
                hSize += 1 + GXCommon.GetObjectCountSizeInBytes(p.count);
            }
            int maxSize = p.settings.MaxPduSize - hSize;
            if (ciphering)
            {
                maxSize -= CipheringHeaderSize;
                if (UseHdlc(p.settings.InterfaceType))
                {
                    maxSize -= 3;
                }
            }
            maxSize -= GXCommon.GetObjectCountSizeInBytes(maxSize);
            if (p.data.Size - p.data.Position > maxSize)
            {
                //More blocks.
                reply.SetUInt8(0);
            }
            else
            {
                //Last block.
                reply.SetUInt8(1);
                maxSize = p.data.Size - p.data.Position;
            }
            //Add block index.
            reply.SetUInt16(p.blockIndex);
            if (p.command == Command.WriteRequest)
            {
                ++p.blockIndex;
                GXCommon.SetObjectCount(p.count, reply);
                reply.SetUInt8(DataType.OctetString);
            }
            if (p.command == Command.ReadRequest)
            {
                ++p.blockIndex;
            }
            GXCommon.SetObjectCount(maxSize, reply);
            return maxSize;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="p"></param>
        /// <param name="reply"></param>
        internal static void GetSNPdu(GXDLMSSNParameters p, GXByteBuffer reply)
        {
            bool ciphering = p.command != Command.Aarq && p.command != Command.Aare && p.settings.IsCiphered(false);
            if (!ciphering && UseHdlc(p.settings.InterfaceType))
            {
                if (p.settings.IsServer)
                {
                    reply.Set(GXCommon.LLCReplyBytes);
                }
                else if (reply.Size == 0)
                {
                    reply.Set(GXCommon.LLCSendBytes);
                }
            }
            int cnt = 0, cipherSize = 0;
            if (ciphering)
            {
                cipherSize = CipheringHeaderSize;
                /*
                if (p.settings.Cipher.Security == Security.Encryption)
                {
                    cipherSize = 7;
                }
                else if (p.settings.Cipher.Security == Security.Authentication)
                {
                    cipherSize = 19;
                }
                else if (p.settings.Cipher.Security == Security.AuthenticationEncryption)
                {
                    cipherSize = 7;
                }
                 * */
            }
            if (p.data != null)
            {
                cnt = p.data.Size - p.data.Position;
            }
            // Add command.
            if (p.command == Command.InformationReport)
            {
                reply.SetUInt8((byte)p.command);
                // Add date time.
                if (p.time == null || p.time.Value.DateTime == DateTime.MinValue || p.time.Value.DateTime == DateTime.MaxValue ||
                    p.time.Value.LocalDateTime == DateTime.MinValue || p.time.Value.LocalDateTime == DateTime.MaxValue)
                {
                    reply.SetUInt8(DataType.None);
                }
                else
                {
                    // Data is send in octet string. Remove data type.
                    int pos = reply.Size;
                    GXCommon.SetData(p.settings, reply, DataType.OctetString, p.time);
                    reply.Move(pos + 1, pos, reply.Size - pos - 1);
                }
                GXCommon.SetObjectCount(p.count, reply);
                reply.Set(p.attributeDescriptor);
            }
            else if (p.command != Command.Aarq && p.command != Command.Aare)
            {
                reply.SetUInt8((byte)p.command);
                if (p.count != 0xFF)
                {
                    GXCommon.SetObjectCount(p.count, reply);
                }
                if (p.requestType != 0xFF)
                {
                    reply.SetUInt8(p.requestType);
                }
                reply.Set(p.attributeDescriptor);

                if (!p.multipleBlocks)
                {
                    p.multipleBlocks = reply.Size + cipherSize + cnt > p.settings.MaxPduSize;
                    //If reply data is not fit to one PDU.
                    if (p.multipleBlocks)
                    {
                        //Remove command.
                        reply.Size = 0;
                        if (!ciphering && UseHdlc(p.settings.InterfaceType))
                        {
                            if (p.settings.IsServer)
                            {
                                reply.Set(GXCommon.LLCReplyBytes);
                            }
                            else if (reply.Size == 0)
                            {
                                reply.Set(GXCommon.LLCSendBytes);
                            }
                        }
                        if (p.command == Command.WriteRequest)
                        {
                            p.requestType = (byte)VariableAccessSpecification.WriteDataBlockAccess;
                        }
                        else if (p.command == Command.ReadRequest)
                        {
                            p.requestType = (byte)VariableAccessSpecification.ReadDataBlockAccess;
                        }
                        else if (p.command == Command.ReadResponse)
                        {
                            p.requestType = (byte)SingleReadResponse.DataBlockResult;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid command.");
                        }
                        reply.SetUInt8((byte)p.command);
                        //Set object count.
                        reply.SetUInt8(1);
                        if (p.requestType != 0xFF)
                        {
                            reply.SetUInt8(p.requestType);
                        }
                        cnt = GXDLMS.AppendMultipleSNBlocks(p, reply);
                    }
                }
                else
                {
                    cnt = GXDLMS.AppendMultipleSNBlocks(p, reply);
                }
            }
            // Add data.
            reply.Set(p.data, cnt);
            //Af all data is transfered.
            if (p.data != null && p.data.Position == p.data.Size)
            {
                p.settings.Index = p.settings.Count = 0;
            }
            // If Ciphering is used.
            if (ciphering && p.command != Command.Aarq && p.command != Command.Aare)
            {
                GXICipher cipher = p.settings.Cipher;
                AesGcmParameter s = new AesGcmParameter(
                    GetGloMessage(p.command),
                    cipher.Security,
                    cipher.SecuritySuite,
                    cipher.InvocationCounter, cipher.SystemTitle,
                    GetBlockCipherKey(p.settings),
                    GetAuthenticationKey(p.settings));
                ++cipher.InvocationCounter;
                byte[] tmp = GXCiphering.Encrypt(s, reply.Array());
                System.Diagnostics.Debug.Assert(!(p.settings.MaxPduSize < tmp.Length));
                reply.Size = 0;
                if (UseHdlc(p.settings.InterfaceType))
                {
                    if (p.settings.IsServer)
                    {
                        reply.Set(GXCommon.LLCReplyBytes);
                    }
                    else if (reply.Size == 0)
                    {
                        reply.Set(GXCommon.LLCSendBytes);
                    }
                }
                reply.Set(tmp);
            }
        }

        /// <summary>
        /// Get HDLC address.
        /// </summary>
        /// <param name="value">HDLC address.</param>
        /// <param name="size">HDLC address size. This is optional.</param>
        /// <returns>HDLC address.</returns>
        internal static Object GetHdlcAddress(int value, byte size)
        {
            if (size < 2 && value < 0x80)
            {
                return (byte)(value << 1 | 1);
            }
            if (size < 4 && value < 0x4000)
            {
                return (UInt16)((value & 0x3F80) << 2 | (value & 0x7F) << 1 | 1);
            }
            if (value < 0x10000000)
            {
                return (UInt32)((value & 0xFE00000) << 4 | (value & 0x1FC000) << 3
                                | (value & 0x3F80) << 2 | (value & 0x7F) << 1 | 1);
            }
            throw new ArgumentException("Invalid address.");
        }

        /// <summary>
        /// Convert HDLC address to bytes.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size">Address size in bytes.</param>
        /// <returns></returns>
        internal static byte[] GetHdlcAddressBytes(int value, byte size)
        {
            Object tmp = GetHdlcAddress(value, size);
            GXByteBuffer bb = new GXByteBuffer();
            if (tmp is byte && size < 2)
            {
                bb.SetUInt8((byte)tmp);
            }
            else if (tmp is UInt16 && size < 4)
            {
                bb.SetUInt16((UInt16)tmp);
            }
            else if (tmp is UInt32)
            {
                bb.SetUInt32((UInt32)tmp);
            }
            else
            {
                throw new ArgumentException("Invalid address type.");
            }
            return bb.Array();
        }

        /// <summary>
        /// Split DLMS PDU to wrapper frames.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="command">DLMS command.</param>
        /// <param name="data"> Wrapped data.</param>
        /// <returns>Wrapper frames</returns>
        internal static byte[] GetWrapperFrame(GXDLMSSettings settings, Command command, GXByteBuffer data)
        {
            GXByteBuffer bb = new GXByteBuffer();
            // Add version.
            bb.SetUInt16(1);
            if (settings.IsServer)
            {
                bb.SetUInt16((UInt16)settings.ServerAddress);
                if (settings.PushClientAddress != 0 && (command == Command.DataNotification || command == Command.EventNotification))
                {
                    bb.SetUInt16((UInt16)settings.PushClientAddress);
                }
                else
                {
                    bb.SetUInt16((UInt16)settings.ClientAddress);
                }
            }
            else
            {
                bb.SetUInt16((UInt16)settings.ClientAddress);
                bb.SetUInt16((UInt16)settings.ServerAddress);
            }
            if (data == null)
            {
                // Data length.
                bb.SetUInt16(0);
            }
            else
            {
                // Data length.
                bb.SetUInt16((UInt16)data.Size);
                // Data
                bb.Set(data);
            }
            //Remove sent data in server side.
            if (settings.IsServer)
            {
                if (data.Size == data.Position)
                {
                    data.Clear();
                }
                else
                {
                    data.Move(data.Position, 0, data.Size - data.Position);
                    data.Position = 0;
                }
            }
            return bb.Array();
        }

        /// <summary>
        /// Get HDLC frame for data.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="frame">Frame ID. If zero new is generated.</param>
        /// <param name="data">Data to add.</param>
        /// <returns>HDLC frames.</returns>
        internal static byte[] GetHdlcFrame(GXDLMSSettings settings, byte frame, GXByteBuffer data)
        {
            return GetHdlcFrame(settings, frame, data, true);
        }

        /// <summary>
        /// Get HDLC frame for data.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="frame">Frame ID. If zero new is generated.</param>
        /// <param name="data">Data to add.</param>
        /// <returns>HDLC frames.</returns>
        internal static byte[] GetHdlcFrame(GXDLMSSettings settings, byte frame, GXByteBuffer data, bool final)
        {
            GXByteBuffer bb = new GXByteBuffer();
            int frameSize, len;
            byte[] primaryAddress, secondaryAddress;
            if (settings.IsServer)
            {
                if ((frame == 0x13 || frame == 0x3) && settings.PushClientAddress != 0)
                {
                    primaryAddress = GetHdlcAddressBytes(settings.PushClientAddress, 0);
                }
                else
                {
                    primaryAddress = GetHdlcAddressBytes(settings.ClientAddress, 0);
                }
                secondaryAddress = GetHdlcAddressBytes(settings.ServerAddress, settings.ServerAddressSize);
                len = secondaryAddress.Length;
            }
            else
            {
                primaryAddress = GetHdlcAddressBytes(settings.ServerAddress, settings.ServerAddressSize);
                secondaryAddress = GetHdlcAddressBytes(settings.ClientAddress, 0);
                len = primaryAddress.Length;
            }
            // Add BOP
            bb.SetUInt8(GXCommon.HDLCFrameStartEnd);
            frameSize = Convert.ToInt32(settings.Hdlc.MaxInfoTX);
            //Remove BOP, type, len, primaryAddress, secondaryAddress, frame, header CRC, data CRC and EOP from data length.
            if (settings.Hdlc.UseFrameSize)
            {
                frameSize -= (10 + len);
            }
            else
            {
                if (data != null && data.Size == 0)
                {
                    frameSize -= 3;
                }
            }
            // If no data
            if (data == null || data.Size == 0)
            {
                len = 0;
                bb.SetUInt8(0xA0);
            }
            else if (data.Size - data.Position <= frameSize)
            {
                // Is last packet.
                len = data.Available;
                bb.SetUInt8((byte)(0xA0 | ((7 + primaryAddress.Length + secondaryAddress.Length + len) >> 8) & 0x7));
            }
            else
            {
                // More data to left.
                len = frameSize;
                bb.SetUInt8((byte)(0xA8 | ((7 + primaryAddress.Length + secondaryAddress.Length + len) >> 8) & 0x7));
            }
            //Frame len.
            if (len == 0)
            {
                bb.SetUInt8((byte)(5 + primaryAddress.Length + secondaryAddress.Length + len));
            }
            else
            {
                bb.SetUInt8((byte)(7 + primaryAddress.Length + secondaryAddress.Length + len));
            }
            // Add primary address.
            bb.Set(primaryAddress);
            // Add secondary address.
            bb.Set(secondaryAddress);
            //Add frame ID.
            if (frame == 0)
            {
                frame = settings.NextSend(true);
            }
            if (!final)
            {
                frame = (byte)(frame & ~0x10);
            }
            bb.SetUInt8(frame);
            // Add header CRC.
            UInt16 crc = GXFCS16.CountFCS16(bb.Data, 1, bb.Size - 1);
            bb.SetUInt16(crc);
            if (len != 0)
            {
                //Add data.
                bb.Set(data, len);
                // Add data CRC.
                crc = GXFCS16.CountFCS16(bb.Data, 1, bb.Size - 1);
                bb.SetUInt16(crc);
            }
            // Add EOP
            bb.SetUInt8(GXCommon.HDLCFrameStartEnd);
            if (data != null)
            {
                //Remove sent data in server side.
                if (settings.IsServer)
                {
                    if (data.Size == data.Position)
                    {
                        data.Clear();
                    }
                    else
                    {
                        data.Move(data.Position, 0, data.Size - data.Position);
                        data.Position = 0;
                    }
                }
            }
            return bb.Array();
        }


        /// <summary>
        /// Get MAC LLC frame for data.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="frame">HDLC frame sequence number.</param>
        /// <param name="creditFields">Credit fields.</param>
        /// <param name="data">Data to add.</param>
        /// <returns>MAC frame.</returns>
        internal static byte[] GetMacFrame(GXDLMSSettings settings, byte frame, byte creditFields, GXByteBuffer data)
        {
            if (settings.InterfaceType == InterfaceType.Plc)
            {
                return GetPlcFrame(settings, creditFields, data);
            }
            return GetMacHdlcFrame(settings, frame, creditFields, data);
        }

        /// <summary>
        /// Get MAC LLC frame for data.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data">Data to add.</param>
        /// <returns>MAC frame.</returns>
        private static byte[] GetPlcFrame(GXDLMSSettings settings, byte creditFields, GXByteBuffer data)
        {
            int frameSize = data.Available;
            //Max frame size is 124 bytes.
            if (frameSize > 134)
            {
                frameSize = 134;
            }
            //PAD Length.
            int padLen = (36 - ((11 + frameSize) % 36)) % 36;
            GXByteBuffer bb = new GXByteBuffer();
            bb.Capacity = 15 + frameSize + padLen;
            //Add STX
            bb.SetUInt8(2);
            //Length.
            bb.SetUInt8((byte)(11 + frameSize));
            //Length.
            bb.SetUInt8(0x50);
            //Add  Credit fields.
            bb.SetUInt8(creditFields);
            //Add source and target MAC addresses.
            bb.SetUInt8((byte)(settings.Plc.MacSourceAddress >> 4));
            int val = settings.Plc.MacSourceAddress << 12;
            val |= settings.Plc.MacDestinationAddress & 0xFFF;
            bb.SetUInt16((UInt16)val);
            bb.SetUInt8((byte)padLen);
            //Control byte.
            bb.SetUInt8(PlcDataLinkData.Request);
            bb.SetUInt8((byte)settings.ServerAddress);
            bb.SetUInt8((byte)settings.ClientAddress);
            bb.Set(data, frameSize);
            //Add padding.
            while (padLen != 0)
            {
                bb.SetUInt8(0);
                --padLen;
            }
            //Checksum.
            UInt16 crc = GXFCS16.CountFCS16(bb.Data, 0, bb.Size);
            bb.SetUInt16(crc);
            //Remove sent data in server side.
            if (settings.IsServer)
            {
                if (data.Size == data.Position)
                {
                    data.Clear();
                }
                else
                {
                    data.Move(data.Position, 0, data.Size - data.Position);
                    data.Position = 0;
                }
            }
            return bb.Array();
        }

        /// <summary>
        /// Get MAC HDLC frame for data.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="frame">HDLC frame.</param>
        /// <param name="creditFields">Credit fields.</param>
        /// <param name="data">Data to add.</param>
        /// <returns>MAC frame.</returns>
        internal static byte[] GetMacHdlcFrame(GXDLMSSettings settings, byte frame, byte creditFields, GXByteBuffer data)
        {
            if (settings.Hdlc.MaxInfoTX > 126)
            {
                settings.Hdlc.MaxInfoTX = 86;
            }
            GXByteBuffer bb = new GXByteBuffer();
            //Lenght is updated last.
            bb.SetUInt16(0);
            //Add  Credit fields.
            bb.SetUInt8(creditFields);
            //Add source and target MAC addresses.
            bb.SetUInt8((byte)(settings.Plc.MacSourceAddress >> 4));
            int val = settings.Plc.MacSourceAddress << 12;
            val |= settings.Plc.MacDestinationAddress & 0xFFF;
            bb.SetUInt16((UInt16)val);
            byte[] tmp = GXDLMS.GetHdlcFrame(settings, frame, data, true);
            int padLen = (36 - ((10 + tmp.Length) % 36)) % 36;
            bb.SetUInt8((byte)padLen);
            bb.Set(tmp);
            //Add padding.
            while (padLen != 0)
            {
                bb.SetUInt8(0);
                --padLen;
            }
            //Checksum.
            UInt32 crc = GXFCS16.CountFCS24(bb.Data, 2, bb.Size - 2 - padLen);
            bb.SetUInt8((byte)(crc >> 16));
            bb.SetUInt16((UInt16)crc);
            //Add NC
            val = bb.Size / 36;
            if (bb.Size % 36 != 0)
            {
                ++val;
            }
            if (val == 1)
            {
                val = (int)PlcMacSubframes.One;
            }
            else if (val == 2)
            {
                val = (int)PlcMacSubframes.Two;
            }
            else if (val == 3)
            {
                val = (int)PlcMacSubframes.Three;
            }
            else if (val == 4)
            {
                val = (int)PlcMacSubframes.Four;
            }
            else if (val == 5)
            {
                val = (int)PlcMacSubframes.Five;
            }
            else if (val == 6)
            {
                val = (int)PlcMacSubframes.Six;
            }
            else if (val == 7)
            {
                val = (int)PlcMacSubframes.Seven;
            }
            else
            {
                throw new Exception("Data length is too high.");
            }
            bb.SetUInt16(0, (UInt16)val);
            return bb.Array();
        }

        /// <summary>
        ///  Check LLC bytes.
        /// </summary>
        /// <param name="server">Is server.</param>
        /// <param name="data">Received data.</param>
        private static bool GetLLCBytes(bool server,
                                        GXByteBuffer data)
        {
            if (server)
            {
                return data.Compare(GXCommon.LLCSendBytes);
            }
            return data.Compare(GXCommon.LLCReplyBytes);
        }

        /// <summary>
        /// Get HDLC sender and receiver address information.
        /// </summary>
        /// <param name="reply">Received data.</param>
        /// <param name="target">target (primary) address</param>
        /// <param name="source">Source (secondary) address.</param>
        /// <param name="type">DLMS frame type.</param>
        internal static void GetHdlcAddressInfo(GXByteBuffer reply, out int target, out int source, out byte type)
        {
            int position = reply.Position;
            target = source = 0;
            type = 0;
            try
            {
                short ch;
                int pos, packetStartID = reply.Position, frameLen = 0;
                // If whole frame is not received yet.
                if (reply.Size - reply.Position < 9)
                {
                    return;
                }
                // Find start of HDLC frame.
                for (pos = reply.Position; pos < reply.Size; ++pos)
                {
                    ch = reply.GetUInt8();
                    if (ch == GXCommon.HDLCFrameStartEnd)
                    {
                        packetStartID = pos;
                        break;
                    }
                }
                // Not a HDLC frame.
                // Sometimes meters can send some strange data between DLMS frames.
                if (reply.Position == reply.Size)
                {
                    // Not enough data to parse;
                    return;
                }
                byte frame = reply.GetUInt8();
                // Check frame length.
                if ((frame & 0x7) != 0)
                {
                    frameLen = ((frame & 0x7) << 8);
                }
                ch = reply.GetUInt8();
                // If not enough data.
                frameLen += ch;
                if (reply.Size - reply.Position + 1 < frameLen)
                {
                    reply.Position = packetStartID;
                    // Not enough data to parse;
                    return;
                }
                int eopPos = frameLen + packetStartID + 1;
                ch = reply.GetUInt8(eopPos);
                if (ch != GXCommon.HDLCFrameStartEnd)
                {
                    throw new GXDLMSException("Invalid data format.");
                }
                //Get address.
                target = GXCommon.GetHDLCAddress(reply);
                source = GXCommon.GetHDLCAddress(reply);
                type = reply.GetUInt8();
            }
            finally
            {
                reply.Position = position;
            }
        }

        static byte GetHdlcData(bool server, GXDLMSSettings settings, GXByteBuffer reply, GXReplyData data, GXReplyData notify)
        {
            short ch;
            int pos, packetStartID = reply.Position, frameLen = 0;
            int crc, crcRead;
            bool first = (data.MoreData & RequestTypes.Frame) == 0 ||
                (notify != null && (notify.MoreData & RequestTypes.Frame) != 0);
            // If whole frame is not received yet.
            if (reply.Size - reply.Position < 9)
            {
                data.IsComplete = false;
                if (notify != null)
                {
                    notify.IsComplete = false;
                }
                return 0;
            }
            data.IsComplete = true;
            if (notify != null)
            {
                notify.IsComplete = true;
            }
            bool isNotify = false;
            // Find start of HDLC frame.
            for (pos = reply.Position; pos < reply.Size; ++pos)
            {
                ch = reply.GetUInt8();
                if (ch == GXCommon.HDLCFrameStartEnd)
                {
                    packetStartID = pos;
                    break;
                }
            }
            // Not a HDLC frame.
            // Sometimes meters can send some strange data between DLMS frames.
            if (reply.Position == reply.Size)
            {
                data.IsComplete = false;
                if (notify != null)
                {
                    notify.IsComplete = false;
                }
                // Not enough data to parse;
                return 0;
            }
            byte frame = reply.GetUInt8();
            if ((frame & 0xF0) != 0xA0)
            {
                --reply.Position;
                return GetHdlcData(server, settings, reply, data, notify);
            }
            // Check frame length.
            if ((frame & 0x7) != 0)
            {
                frameLen = ((frame & 0x7) << 8);
            }
            ch = reply.GetUInt8();
            // If not enough data.
            frameLen += ch;
            if (reply.Size - reply.Position + 1 < frameLen)
            {
                data.IsComplete = false;
                reply.Position = packetStartID;
                // Not enough data to parse;
                return 0;
            }
            int eopPos = frameLen + packetStartID + 1;
            ch = reply.GetUInt8(eopPos);
            if (ch != GXCommon.HDLCFrameStartEnd)
            {
                reply.Position -= 2;
                return GetHdlcData(server, settings, reply, data, notify);
            }

            // Check addresses.
            int source, target;
            bool ret;
            try
            {
                ret = CheckHdlcAddress(server, settings, reply, eopPos, out source, out target);
            }
            catch
            {
                ret = false;
                source = target = 0;
            }
            if (!ret)
            {
                //If not notify.
                if (!(reply.Position < reply.Size && (reply.GetUInt8(reply.Position) == 0x13 || reply.GetUInt8(reply.Position) == 0x3)))
                {
                    //If echo.
                    reply.Position = 1 + eopPos;
                    return GetHdlcData(server, settings, reply, data, notify);
                }
                if (notify != null)
                {
                    isNotify = true;
                    notify.TargetAddress = target;
                    notify.SourceAddress = source;
                }
            }
            // Is there more data available.
            bool moreData = (frame & 0x8) != 0;
            // Get frame type.
            frame = reply.GetUInt8();
            if (data.Xml == null && !settings.CheckFrame(frame, data.Xml))
            {
                reply.Position = (eopPos + 1);
                return GetHdlcData(server, settings, reply, data, notify);
            }
            //If server is using same client and server address for notifications.
            if ((frame == 0x13 || frame == 0x3) && !isNotify && notify != null)
            {
                isNotify = true;
                notify.TargetAddress = target;
                notify.SourceAddress = source;
            }
            if (moreData)
            {
                if (isNotify)
                {
                    notify.MoreData = (RequestTypes)(notify.MoreData | RequestTypes.Frame);
                    notify.HdlcStreaming = (frame & 0x10) == 0;
                }
                else
                {
                    data.MoreData = (RequestTypes)(data.MoreData | RequestTypes.Frame);
                    data.HdlcStreaming = (frame & 0x10) == 0;
                }
            }
            //If the final bit is set. This is used when Window size > 1.
            else if ((frame & 0x10) != 0 || settings.Hdlc.WindowSizeRX == 1)
            {
                if (isNotify)
                {
                    notify.MoreData = (RequestTypes)(notify.MoreData & ~RequestTypes.Frame);
                    notify.HdlcStreaming = false;
                }
                else
                {
                    data.MoreData = (RequestTypes)(data.MoreData & ~RequestTypes.Frame);
                    data.HdlcStreaming = false;
                }
            }
            // Check that header CRC is correct.
            crc = GXFCS16.CountFCS16(reply.Data, packetStartID + 1, reply.Position - packetStartID - 1);
            crcRead = reply.GetUInt16();
            if (crc != crcRead)
            {
                if (reply.Size - reply.Position > 8)
                {
                    return GetHdlcData(server, settings, reply, data, notify);
                }
                if (data.Xml == null)
                {
                    throw new Exception("Invalid header checksum.");
                }
                data.Xml.AppendComment("Invalid header checksum.");
            }
            // Check that packet CRC match only if there is a data part.
            if (reply.Position != packetStartID + frameLen + 1)
            {
                crc = GXFCS16.CountFCS16(reply.Data, packetStartID + 1,
                                         frameLen - 2);
                crcRead = reply.GetUInt16(packetStartID + frameLen - 1);
                if (crc != crcRead)
                {
                    if (data.Xml == null)
                    {
                        throw new Exception("Invalid data checksum.");
                    }
                    data.Xml.AppendComment("Invalid data checksum.");
                }
                // Remove CRC and EOP from packet length.
                if (isNotify)
                {
                    notify.PacketLength = eopPos - 2;
                }
                else
                {
                    data.PacketLength = eopPos - 2;
                }
            }
            else
            {
                if (isNotify)
                {
                    notify.PacketLength = reply.Position + 1;
                }
                else
                {
                    data.PacketLength = reply.Position + 1;
                }
            }
            //If client want to know used server and client address.
            if (data.TargetAddress == 0 && data.SourceAddress == 0)
            {
                data.TargetAddress = target;
                data.SourceAddress = source;
            }
            if (frame != 0x13 && frame != 0x3 && (frame & (byte)HdlcFrameType.Uframe) == (byte)HdlcFrameType.Uframe)
            {
                //Get Eop if there is no data.
                if (reply.Position == packetStartID + frameLen + 1)
                {
                    // Get EOP.
                    reply.GetUInt8();
                }
                if (frame == 0x97)
                {
                    data.Error = (int)ErrorCode.UnacceptableFrame;
                }
                else if (frame == 0x1F)
                {
                    data.Error = (int)ErrorCode.DisconnectMode;
                }
                else if (frame == 0x17)
                {
                    data.Error = (int)ErrorCode.DisconnectMode;
                }
                data.Command = (Command)frame;
                if (data.Command == Command.Snrm)
                {
                    settings.Connected &= ~ConnectionState.Iec;
                }
            }
            //If S-frame
            else if (frame != 0x13 && frame != 0x3 && (frame & (byte)HdlcFrameType.Sframe) == (byte)HdlcFrameType.Sframe)
            {
                //If frame is rejected.
                int tmp = (frame >> 2) & 0x3;
                if (tmp == (byte)HdlcControlFrame.Reject)
                {
                    data.Error = (int)ErrorCode.Rejected;
                }
                else if (tmp == (byte)HdlcControlFrame.ReceiveNotReady)
                {
                    data.Error = (int)ErrorCode.ReceiveNotReady;
                }
                else if (tmp == (byte)HdlcControlFrame.ReceiveReady)
                {
                    System.Diagnostics.Debug.WriteLine("ReceiveReady.");
                }
                //Get Eop if there is no data.
                if (reply.Position == packetStartID + frameLen + 1)
                {
                    // Get EOP.
                    reply.GetUInt8();
                }
            }
            else //Iframe
            {
                //Get Eop if there is no data.
                if (reply.Position == packetStartID + frameLen + 1)
                {
                    // Get EOP.
                    reply.GetUInt8();
                    if ((frame & 0x1) == 0x1)
                    {
                        data.MoreData = RequestTypes.Frame;
                    }
                }
                else
                {
                    if (first)
                    {
                        bool llc = GetLLCBytes(server, reply);
                        if (data.Xml == null)
                        {
                            if (!llc)
                            {
                                throw new Exception("LLC bytes are missing from the message.");
                            }
                        }
                        else if (!llc)
                        {
                            //We don't know is this the first message when XML is handed.
                            GetLLCBytes(!server, reply);
                        }
                    }
                }
            }
            return frame;
        }

        private static void GetServerAddress(int address, out int logical, out int physical)
        {
            if (address < 0x4000)
            {
                logical = address >> 7;
                physical = address & 0x7F;
            }
            else
            {
                logical = address >> 14;
                physical = address & 0x3FFF;
            }
        }

        /// <summary>
        /// Check that client and server address match.
        /// </summary>
        /// <param name="server">Is server.</param>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="reply">Received data.</param>
        /// <param name="index">Position.</param>
        /// <returns>True, if client and server address match.</returns>
        private static bool CheckHdlcAddress(
            bool server,
            GXDLMSSettings settings,
            GXByteBuffer reply,
            int index, out int source, out int target)
        {
            // Get destination and source addresses.
            target = GXCommon.GetHDLCAddress(reply);
            source = GXCommon.GetHDLCAddress(reply);
            if (server)
            {
                // Check that server addresses match.
                if (settings.ServerAddress != 0
                        && settings.ServerAddress != target)
                {
                    if (reply.GetUInt8(reply.Position) == (int)Command.Snrm)
                    {
                        settings.ServerAddress = target;
                    }
                    else
                    {
                        throw new GXDLMSException(
                            "Destination addresses do not match. It is "
                            + target.ToString() + ". It should be "
                            + settings.ServerAddress.ToString() + ".");
                    }
                }
                else
                {
                    settings.ServerAddress = target;
                }
                // Check that client addresses match.
                if (settings.ClientAddress != 0 && settings.ClientAddress != source)
                {
                    if (reply.GetUInt8(reply.Position) == (int)Command.Snrm)
                    {
                        settings.ClientAddress = source;
                    }
                    else
                    {
                        throw new GXDLMSException(
                            "Source addresses do not match. It is "
                            + source.ToString() + ". It should be "
                            + settings.ClientAddress.ToString()
                            + ".");
                    }
                }
                else
                {
                    settings.ClientAddress = source;
                }
            }
            else
            {
                // Check that client addresses match.
                if (settings.ClientAddress != target)
                {
                    //If echo.
                    if (settings.ClientAddress == source &&
                            settings.ServerAddress == target)
                    {
                        reply.Position = (index + 1);
                    }
                    //If client wants to know used client and server address.
                    else if (settings.ClientAddress == 0 && settings.ServerAddress == 0x7F)
                    {
                        return true;
                    }
                    return false;
                }
                // Check that server addresses match.
                if (settings.ServerAddress != source &&
                    // If All-station (Broadcast).
                    (settings.ServerAddress & 0x7F) != 0x7F && (settings.ServerAddress & 0x3FFF) != 0x3FFF)
                {
                    //Check logical and physical address separately.
                    //This is done because some meters might send four bytes
                    //when only two bytes is needed.
                    int readLogical, readPhysical, logical, physical;
                    GetServerAddress(source, out readLogical, out readPhysical);
                    GetServerAddress(settings.ServerAddress, out logical, out physical);
                    if (readLogical != logical || readPhysical != physical)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Get data from TCP/IP frame.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="buff">Received data.</param>
        /// <param name="data">Reply information.</param>
        /// <param name="notify">Notify information.</param>
        static bool GetTcpData(GXDLMSSettings settings,
                               GXByteBuffer buff, GXReplyData data, GXReplyData notify)
        {
            // If whole frame is not received yet.
            if (buff.Size - buff.Position < 8)
            {
                data.IsComplete = false;
                return true;
            }
            bool isData = true;
            int pos = buff.Position;
            int value;
            data.IsComplete = false;
            if (notify != null)
            {
                notify.IsComplete = false;
            }
            while (buff.Available > 2)
            {
                // Get version
                value = buff.GetUInt16();
                if (value == 1)
                {
                    if (buff.Available < 6)
                    {
                        isData = false;
                        break;
                    }
                    // Check TCP/IP addresses.
                    if (!CheckWrapperAddress(settings, buff, data, notify))
                    {
                        data = notify;
                        isData = false;
                    }
                    // Get length.
                    value = buff.GetUInt16();
                    data.IsComplete = !((buff.Size - buff.Position) < value);
                    if (!data.IsComplete)
                    {
                        buff.Position = pos;
                    }
                    else
                    {
                        data.PacketLength = buff.Position + value;
                    }
                    break;
                }
                else
                {
                    --buff.Position;
                }
            }
            return isData;
        }

        /// <summary>
        /// Validate M-Bus checksum
        /// </summary>
        /// <param name="bb"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static bool ValidateCheckSum(GXByteBuffer bb, int count)
        {
            byte value = 0;
            for (int pos = 0; pos != count; ++pos)
            {
                value += bb.GetUInt8(bb.Position + pos);
            }
            return value == bb.GetUInt8(bb.Position + count);
        }

        /// <summary>
        /// Get data from wired M-Bus frame.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="buff">Received data.</param>
        /// <param name="data">Reply information.</param>
        static void GetWiredMBusData(GXDLMSSettings settings,
                               GXByteBuffer buff, GXReplyData data)
        {
            int packetStartID = buff.Position;
            if (buff.GetUInt8() != 0x68 || buff.Available < 5)
            {
                data.IsComplete = false;
                --buff.Position;
            }
            else
            {
                //L-field.
                int len = buff.GetUInt8();
                //L-field.
                if (buff.GetUInt8() != len ||
                    buff.Available < 3 + len ||
                    buff.GetUInt8() != 0x68)
                {
                    data.IsComplete = false;
                    buff.Position = packetStartID;
                }
                else
                {
                    bool crc = ValidateCheckSum(buff, len);
                    if (!crc && data.Xml == null)
                    {
                        data.IsComplete = false;
                        buff.Position = packetStartID;
                    }
                    else
                    {
                        if (!crc)
                        {
                            data.Xml.AppendComment("Invalid checksum.");
                        }
                        //Check EOP.
                        if (buff.GetUInt8(buff.Position + len + 1) != 0x16)
                        {
                            data.IsComplete = false;
                            buff.Position = packetStartID;
                            return;
                        }
                        data.PacketLength = buff.Position + len;
                        data.IsComplete = true;
                        int index = data.Data.Position;
                        //Control field (C-Field)
                        byte tmp = buff.GetUInt8();
                        MBusCommand cmd = (MBusCommand)(tmp & 0xF);
                        //Address (A-field)
                        byte id = buff.GetUInt8();
                        // The Control Information Field (CI-field)
                        byte ci = buff.GetUInt8();
                        if (ci == 0x0)
                        {
                            data.MoreData |= RequestTypes.Frame;
                        }
                        else if ((ci >> 4) == (ci & 0xf))
                        {
                            //If this is the last telegram.
                            data.MoreData &= ~RequestTypes.Frame;
                        }
                        //If M-Bus data header is present
                        if (ci != 0)
                        {

                        }
                        if ((tmp & 0x40) != 0)
                        {
                            //Message from primary(initiating) station
                            //Destination Transport Service Access Point
                            settings.ClientAddress = buff.GetUInt8();
                            //Source Transport Service Access Point
                            settings.ServerAddress = buff.GetUInt8();
                        }
                        else
                        {
                            //Message from secondary (responding) station.
                            //Source Transport Service Access Point
                            settings.ServerAddress = buff.GetUInt8();
                            //Destination Transport Service Access Point
                            settings.ClientAddress = buff.GetUInt8();
                        }
                        if (data.Xml != null && data.Xml.Comments)
                        {
                            data.Xml.AppendComment("Command: " + cmd);
                            data.Xml.AppendComment("A-Field: " + id);
                            data.Xml.AppendComment("CI-Field: " + ci);
                            if ((tmp & 0x40) != 0)
                            {
                                data.Xml.AppendComment("Primary station: " + settings.ServerAddress);
                                data.Xml.AppendComment("Secondary station: " + settings.ClientAddress);
                            }
                            else
                            {
                                data.Xml.AppendComment("Primary station: " + settings.ClientAddress);
                                data.Xml.AppendComment("Secondary station: " + settings.ServerAddress);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get data from Wireless M-Bus frame.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="buff">Received data.</param>
        /// <param name="data">Reply information.</param>
        static void GetWirelessMBusData(GXDLMSSettings settings,
                               GXByteBuffer buff, GXReplyData data)
        {
            //L-field.
            int len = buff.GetUInt8();
            //Some meters are counting length to frame size.
            if (buff.Size < len - 1)
            {
                data.IsComplete = false;
                --buff.Position;
            }
            else
            {
                //Some meters are counting length to frame size.
                if (buff.Size < len)
                {
                    --len;
                }
                data.PacketLength = len;
                data.IsComplete = true;
                //C-field.
                MBusCommand cmd = (MBusCommand)(buff.GetUInt8() & 0x4);
                //M-Field.
                UInt16 manufacturerID = buff.GetUInt16();
                string man = GXCommon.DecryptManufacturer(manufacturerID);
                //A-Field.
                UInt32 id = buff.GetUInt32();
                byte meterVersion = buff.GetUInt8();
                MBusMeterType type = (MBusMeterType)buff.GetUInt8();
                // CI-Field
                MBusControlInfo ci = (MBusControlInfo)buff.GetUInt8();
                //Access number.
                byte frameId = buff.GetUInt8();
                //State of the meter
                byte state = buff.GetUInt8();
                //Configuration word.
                UInt16 configurationWord = buff.GetUInt16();
                byte encryptedBlocks = (byte)(configurationWord >> 12);
                MBusEncryptionMode encryption = (MBusEncryptionMode)(configurationWord & 7);
                settings.ClientAddress = buff.GetUInt8();
                settings.ServerAddress = buff.GetUInt8();
                if (data.Xml != null && data.Xml.Comments)
                {
                    data.Xml.AppendComment("Command: " + cmd);
                    data.Xml.AppendComment("Manufacturer: " + man);
                    data.Xml.AppendComment("Meter Version: " + meterVersion);
                    data.Xml.AppendComment("Meter Type: " + type);
                    data.Xml.AppendComment("Control Info: " + ci);
                    data.Xml.AppendComment("Encryption: " + encryption);
                }
                else if (settings.MBus != null)
                {
                    settings.MBus.ManufacturerId = GXCommon.DecryptManufacturer(manufacturerID);
                    settings.MBus.Version = meterVersion;
                    settings.MBus.MeterType = type;
                }
            }
        }

        /// <summary>
        /// Get data from S-FSK PLC frame.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="buff">Received data.</param>
        /// <param name="data">Reply information.</param>
        static void GetPlcData(GXDLMSSettings settings,
                                   GXByteBuffer buff, GXReplyData data)
        {
            if (buff.Available < 9)
            {
                data.IsComplete = false;
                return;
            }
            int pos;
            int packetStartID = buff.Position;
            // Find STX.
            byte stx;
            for (pos = buff.Position; pos < buff.Size; ++pos)
            {
                stx = buff.GetUInt8();
                if (stx == 2)
                {
                    packetStartID = pos;
                    break;
                }
            }
            // Not a PLC frame.
            if (buff.Position == buff.Size)
            {
                // Not enough data to parse;
                data.IsComplete = false;
                buff.Position = packetStartID;
                return;
            }
            int len = buff.GetUInt8();
            int index = buff.Position;
            if (buff.Available < len)
            {
                data.IsComplete = false;
                buff.Position -= 2;
            }
            else
            {
                buff.GetUInt8();
                //Credit fields.  IC, CC, DC
                byte credit = buff.GetUInt8();
                //MAC Addresses.
                int mac = buff.GetUInt24();
                //SA.
                UInt16 macSa = (UInt16)(mac >> 12);
                //DA.
                UInt16 macDa = (UInt16)(mac & 0xFFF);
                //PAD length.
                byte padLen = buff.GetUInt8();
                if (buff.Size < len + padLen + 2)
                {
                    data.IsComplete = false;
                    buff.Position -= index + 6;
                }
                else
                {
                    //DL.Data.request
                    byte ch = buff.GetUInt8();
                    if (ch != (int)PlcDataLinkData.Request)
                    {
                        throw new Exception("Parsing MAC LLC data failed. Invalid DataLink data request.");
                    }
                    byte da = buff.GetUInt8();
                    byte sa = buff.GetUInt8();
                    if (settings.IsServer)
                    {
                        data.IsComplete = data.Xml != null ||
                            ((macDa == (UInt16)PlcDestinationAddress.AllPhysical || macDa == settings.Plc.MacSourceAddress) &&
                        (macSa == (UInt16)PlcSourceAddress.Initiator || macSa == settings.Plc.MacDestinationAddress));
                        data.SourceAddress = macDa;
                        data.TargetAddress = macSa;
                    }
                    else
                    {
                        data.IsComplete = data.Xml != null ||
                            (macDa == (UInt16)PlcDestinationAddress.AllPhysical ||
                            macDa == (UInt16)PlcSourceAddress.Initiator ||
                            macDa == settings.Plc.MacDestinationAddress);
                        data.TargetAddress = macDa;
                        data.SourceAddress = macSa;
                    }
                    //Skip padding.
                    if (data.IsComplete)
                    {
                        UInt16 crcCount = GXFCS16.CountFCS16(buff.Data, 0, len + padLen);
                        UInt16 crc = buff.GetUInt16(len + padLen);
                        //Check CRC.
                        if (crc != crcCount)
                        {
                            if (data.Xml == null)
                            {
                                throw new Exception("Invalid data checksum.");
                            }
                            data.Xml.AppendComment("Invalid data checksum.");
                        }
                        data.PacketLength = len;
                    }
                    else
                    {
                        buff.Position = packetStartID;
                    }
                }
            }
        }


        /// <summary>
        /// Get data from S-FSK PLC Hdlc frame.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="buff">Received data.</param>
        /// <param name="data">Reply information.</param>
        static byte GetPlcHdlcData(GXDLMSSettings settings,
                                   GXByteBuffer buff, GXReplyData data)
        {
            if (buff.Available < 2)
            {
                data.IsComplete = false;
                return 0;
            }
            byte frame = 0;
            //SN field.
            byte frameLen = GetPlcSfskFrameSize(buff);
            if (frameLen == 0)
            {
                throw new Exception("Invalid PLC frame size.");
            }
            if (buff.Available < frameLen)
            {
                data.IsComplete = false;
            }
            else
            {
                buff.Position += 2;
                int index = buff.Position;
                //Credit fields.  IC, CC, DC
                byte credit = buff.GetUInt8();
                //MAC Addresses.
                int mac = buff.GetUInt24();
                //SA.
                short sa = (short)(mac >> 12);
                //DA.
                short da = (short)(mac & 0xFFF);
                if (settings.IsServer)
                {
                    data.IsComplete = data.Xml != null ||
                        ((da == (UInt16)PlcDestinationAddress.AllPhysical || da == settings.Plc.MacSourceAddress) &&
                    (sa == (UInt16)PlcHdlcSourceAddress.Initiator || sa == settings.Plc.MacDestinationAddress));
                    data.SourceAddress = da;
                    data.TargetAddress = sa;
                }
                else
                {
                    data.IsComplete = data.Xml != null ||
                        (da == (UInt16)PlcHdlcSourceAddress.Initiator || da == settings.Plc.MacDestinationAddress);
                    data.TargetAddress = sa;
                    data.SourceAddress = da;
                }
                if (data.IsComplete)
                {
                    //PAD length.
                    byte padLen = buff.GetUInt8();
                    frame = GetHdlcData(settings.IsServer, settings, buff, data, null);
                    GetDataFromFrame(buff, data, true);
                    buff.Position += padLen;
                    UInt32 crcCount = GXFCS16.CountFCS24(buff.Data, index, buff.Position - index);
                    int crc = buff.GetUInt24(buff.Position);
                    //Check CRC.
                    if (crc != crcCount)
                    {
                        if (data.Xml == null)
                        {
                            throw new Exception("Invalid data checksum.");
                        }
                        data.Xml.AppendComment("Invalid data checksum.");
                    }
                    data.PacketLength = 2 + buff.Position - index;
                }
                else
                {
                    buff.Position += frameLen - index - 4;
                }
            }
            return frame;
        }

        /// <summary>
        /// Check is this wireless M-Bus message.
        /// </summary>
        /// <param name="buff">Received data.</param>
        /// <returns>True, if this is wireless M-Bus message.</returns>
        internal static bool IsWirelessMBusData(GXByteBuffer buff)
        {
            if (buff.Size - buff.Position < 2)
            {
                return false;
            }
            byte cmd = buff.GetUInt8(buff.Position + 1);
            return (cmd & (byte)MBusCommand.SndNr) != 0 ||
                (cmd & (byte)MBusCommand.SndUd) != 0 ||
                (cmd & (byte)MBusCommand.RspUd) != 0;
        }

        /// <summary>
        /// Check is this wired M-Bus message.
        /// </summary>
        /// <param name="buff">Received data.</param>
        /// <returns>True, if this is wired M-Bus message.</returns>
        internal static bool IsWiredMBusData(GXByteBuffer buff)
        {
            if (buff.Size - buff.Position < 1)
            {
                return false;
            }
            return buff.GetUInt8(buff.Position) == 0x68;
        }

        /// <summary>
        /// Check is this PLC S-FSK message.
        /// </summary>
        /// <param name="buff">Received data.</param>
        /// <returns>S-FSK frame size in bytes.</returns>
        internal static byte GetPlcSfskFrameSize(GXByteBuffer buff)
        {
            byte ret;
            if (buff.Size - buff.Position < 2)
            {
                ret = 0;
            }
            else
            {
                UInt16 len = buff.GetUInt16(buff.Position);
                switch (len)
                {
                    case (int)PlcMacSubframes.One:
                        ret = 36;
                        break;
                    case (int)PlcMacSubframes.Two:
                        ret = 2 * 36;
                        break;
                    case (int)PlcMacSubframes.Three:
                        ret = 3 * 36;
                        break;
                    case (int)PlcMacSubframes.Four:
                        ret = 4 * 36;
                        break;
                    case (int)PlcMacSubframes.Five:
                        ret = 5 * 36;
                        break;
                    case (int)PlcMacSubframes.Six:
                        ret = 6 * 36;
                        break;
                    case (int)PlcMacSubframes.Seven:
                        ret = 7 * 36;
                        break;
                    default:
                        ret = 0;
                        break;
                }
            }
            return ret;
        }

        internal static bool CheckWrapperAddress(GXDLMSSettings settings, GXByteBuffer buff, GXReplyData data, GXReplyData notify)
        {
            bool ret = true;
            int value;
            if (settings.IsServer)
            {
                value = buff.GetUInt16();
                data.SourceAddress = value;
                // Check that client addresses match.
                if (data.Xml == null && settings.ClientAddress != 0
                        && settings.ClientAddress != value)
                {
                    throw new Exception("Source addresses do not match. It is "
                        + value.ToString() + ". It should be "
                        + settings.ClientAddress.ToString());
                }
                settings.ClientAddress = value;
                value = buff.GetUInt16();
                data.TargetAddress = value;
                // Check that server addresses match.
                if (data.Xml == null && settings.ServerAddress != 0
                        && settings.ServerAddress != value)
                {
                    throw new Exception("Destination addresses do not match. It is "
                    + value.ToString() + ". It should be "
                    + settings.ServerAddress.ToString()
                    + ".");
                }
                settings.ServerAddress = value;
            }
            else
            {
                value = buff.GetUInt16();
                data.TargetAddress = value;
                // Check that server addresses match.
                if (data.Xml == null && settings.ServerAddress != 0
                        && settings.ServerAddress != value)
                {
                    if (notify == null)
                    {
                        throw new Exception("Source addresses do not match. It is "
                        + value.ToString() + ". It should be "
                        + settings.ServerAddress.ToString()
                        + ".");
                    }
                    notify.SourceAddress = value;
                    ret = false;
                }
                else
                {
                    settings.ServerAddress = value;
                }
                value = buff.GetUInt16();
                data.SourceAddress = value;
                // Check that client addresses match.
                if (data.Xml == null && settings.ClientAddress != 0
                        && settings.ClientAddress != value)
                {
                    if (notify == null)
                    {
                        throw new Exception("Destination addresses do not match. It is "
                        + value.ToString() + ". It should be "
                        + settings.ClientAddress.ToString() + ".");
                    }
                    ret = false;
                    notify.TargetAddress = value;
                }
                else
                {
                    settings.ClientAddress = value;
                }
            }
            return ret;
        }

        /// <summary>
        /// Handle read response data block result.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="reply">Received reply.</param>
        /// <param name="index">Starting index.</param>
        static bool ReadResponseDataBlockResult(GXDLMSSettings settings, GXReplyData reply, int index)
        {
            reply.Error = 0;
            byte lastBlock = reply.Data.GetUInt8();
            // Get Block number.
            UInt32 number = reply.Data.GetUInt16();
            int blockLength = GXCommon.GetObjectCount(reply.Data);
            // Is not Last block.
            if (lastBlock == 0)
            {
                reply.MoreData = (RequestTypes)(reply.MoreData | RequestTypes.DataBlock);
            }
            else
            {
                reply.MoreData = (RequestTypes)(reply.MoreData & ~RequestTypes.DataBlock);
            }

            //If meter's block index is zero based.
            if (number != 1 && settings.BlockIndex == 1)
            {
                settings.BlockIndex = (uint)number;
            }
            UInt32 expectedIndex = settings.BlockIndex;
            if (number != expectedIndex)
            {
                throw new ArgumentException(
                    "Invalid Block number. It is " + number
                    + " and it should be " + expectedIndex + ".");
            }
            // If whole block is not read.
            if ((reply.MoreData & RequestTypes.Frame) != 0)
            {
                GetDataFromBlock(reply.Data, index);
                return false;
            }
            // Check block length when all data is received.
            if (blockLength != reply.Data.Size - reply.Data.Position)
            {
                throw new GXDLMSException("Invalid block length.");
            }
            reply.Command = Command.None;
            if (reply.Xml != null)
            {
                reply.Data.Trim();
                reply.Xml.AppendStartTag(Command.ReadResponse, SingleReadResponse.DataBlockResult);
                reply.Xml.AppendLine(TranslatorTags.LastBlock, "Value", reply.Xml.IntegerToHex(lastBlock, 2));
                reply.Xml.AppendLine(TranslatorTags.BlockNumber, "Value", reply.Xml.IntegerToHex(number, 4));
                reply.Xml.AppendLine(TranslatorTags.RawData, "Value", GXCommon.ToHex(reply.Data.Data, false, 0, reply.Data.Size));
                reply.Xml.AppendEndTag(Command.ReadResponse, SingleReadResponse.DataBlockResult);
                return false;
            }
            GetDataFromBlock(reply.Data, index);
            reply.TotalCount = 0;
            // If last packet and data is not try to peek.
            if (reply.MoreData == RequestTypes.None)
            {
                settings.ResetBlockIndex();
            }
            return true;
        }

        /// <summary>
        /// Handle read response and get data from block and/or update error status.
        /// </summary>
        /// <param name="reply">Received data from the client.</param>
        static bool HandleReadResponse(GXDLMSSettings settings, GXReplyData reply, int index)
        {
            int pos, cnt = reply.TotalCount;
            // If we are reading value first time or block is handed.
            bool first = cnt == 0 || reply.CommandType == (byte)SingleReadResponse.DataBlockResult;
            if (first)
            {
                cnt = GXCommon.GetObjectCount(reply.Data);
                reply.TotalCount = cnt;
            }
            SingleReadResponse type;
            List<Object> values = null;
            if (cnt != 1)
            {
                //Parse data after all data is received when readlist is used.
                if (reply.IsMoreData)
                {
                    GetDataFromBlock(reply.Data, 0);
                    return false;
                }
                if (!first)
                {
                    reply.Data.Position = 0;
                    first = true;
                }
                values = new List<object>();
                if (reply.Value is List<object>)
                {
                    values.AddRange((List<object>)reply.Value);
                }
                reply.Value = null;
            }
            if (reply.Xml != null)
            {
                reply.Xml.AppendStartTag(Command.ReadResponse, "Qty", reply.Xml.IntegerToHex(cnt, 2));
            }
            bool standardXml = reply.Xml != null && reply.Xml.OutputType == TranslatorOutputType.StandardXml;
            for (pos = 0; pos != cnt; ++pos)
            {
                // Get response type code.
                if (first)
                {
                    type = (SingleReadResponse)reply.Data.GetUInt8();
                    reply.CommandType = (byte)type;
                }
                else
                {
                    type = (SingleReadResponse)reply.CommandType;
                }
                switch (type)
                {
                    case SingleReadResponse.Data:
                        reply.Error = 0;
                        if (reply.Xml != null)
                        {
                            if (standardXml)
                            {
                                reply.Xml.AppendStartTag(TranslatorTags.Choice);
                            }
                            reply.Xml.AppendStartTag(Command.ReadResponse, SingleReadResponse.Data);
                            GXDataInfo di = new GXDataInfo();
                            di.xml = reply.Xml;
                            GXCommon.GetData(settings, reply.Data, di);
                            reply.Xml.AppendEndTag(Command.ReadResponse, SingleReadResponse.Data);
                            if (standardXml)
                            {
                                reply.Xml.AppendEndTag(TranslatorTags.Choice);
                            }
                        }
                        else if (cnt == 1)
                        {
                            GetDataFromBlock(reply.Data, 0);
                        }
                        else
                        {
                            reply.ReadPosition = reply.Data.Position;
                            GetValueFromData(settings, reply);
                            reply.Data.Position = reply.ReadPosition;
                            values.Add(reply.Value);
                            reply.Value = null;
                        }
                        break;
                    case SingleReadResponse.DataAccessError:
                        // Get error code.
                        reply.Error = reply.Data.GetUInt8();
                        if (reply.Xml != null)
                        {
                            if (standardXml)
                            {
                                reply.Xml.AppendStartTag(TranslatorTags.Choice);
                            }
                            reply.Xml.AppendLine(
                                (int)Command.ReadResponse << 8
                                | (int)SingleReadResponse.DataAccessError,
                                null,
                                GXDLMSTranslator.ErrorCodeToString(
                                    reply.Xml.OutputType,
                                    (ErrorCode)reply.Error));
                            if (standardXml)
                            {
                                reply.Xml.AppendEndTag(TranslatorTags.Choice);
                            }
                        }
                        break;
                    case SingleReadResponse.DataBlockResult:
                        if (!ReadResponseDataBlockResult(settings, reply, index))
                        {
                            //If xml only received bytes are shown. Data is not try to parse.
                            if (reply.Xml != null)
                            {
                                reply.Xml.AppendEndTag(Command.ReadResponse);
                            }
                            return false;
                        }
                        break;
                    case SingleReadResponse.BlockNumber:
                        // Get Block number.
                        UInt32 number = reply.Data.GetUInt16();
                        if (number != settings.BlockIndex)
                        {
                            throw new ArgumentException(
                                "Invalid Block number. It is " + number
                                + " and it should be " + settings.BlockIndex + ".");
                        }
                        settings.IncreaseBlockIndex();
                        reply.MoreData = (RequestTypes)(reply.MoreData | RequestTypes.DataBlock);
                        break;
                    default:
                        throw new GXDLMSException("HandleReadResponse failed. Invalid tag.");
                }
            }
            if (reply.Xml != null)
            {
                reply.Xml.AppendEndTag(Command.ReadResponse);
                return true;
            }
            if (values != null)
            {
                reply.Value = values.ToArray();
            }
            return cnt == 1;
        }

        private static void HandleActionResponseNormal(GXDLMSSettings settings, GXReplyData data)
        {
            //Get Action-Result
            byte ret = data.Data.GetUInt8();
            if (ret != 0)
            {
                data.Error = ret;
            }
            if (data.Xml != null)
            {
                if (data.Xml.OutputType == TranslatorOutputType.StandardXml)
                {
                    data.Xml.AppendStartTag(TranslatorTags.SingleResponse);
                }
                data.Xml.AppendLine(TranslatorTags.Result, null,
                                    GXDLMSTranslator.ErrorCodeToString(
                                        data.Xml.OutputType,
                                        (ErrorCode)data.Error));
            }
            settings.ResetBlockIndex();
            // Response normal. Get data if exists. Some meters do not return here anything.
            if (data.Error == 0 && data.Data.Position < data.Data.Size)
            {
                //Get-Data-Result
                ret = data.Data.GetUInt8();
                //If data.
                if (ret == 0)
                {
                    GetDataFromBlock(data.Data, 0);
                }
                else if (ret == 1)
                {
                    //Get Data-Access-Result
                    ret = data.Data.GetUInt8();
                    if (ret != 0)
                    {
                        data.Error = data.Data.GetUInt8();
                        //Handle Texas Instrument missing byte here.
                        if (ret == 9 && data.Error == 16)
                        {
                            data.Data.Position -= 2;
                            GetDataFromBlock(data.Data, 0);
                            data.Error = 0;
                            ret = 0;
                        }
                    }
                    else
                    {
                        GetDataFromBlock(data.Data, 0);
                    }
                }
                else
                {
                    throw new GXDLMSException("HandleActionResponseNormal failed. Invalid tag.");
                }
                if (data.Xml != null && (ret != 0 || data.Data.Position < data.Data.Size))
                {
                    data.Xml.AppendStartTag(TranslatorTags.ReturnParameters);
                    if (ret != 0)
                    {
                        data.Xml.AppendLine(
                            TranslatorTags.DataAccessError, null,
                            GXDLMSTranslator.ErrorCodeToString(
                                data.Xml.OutputType, (ErrorCode)data.Error));
                    }
                    else
                    {
                        data.Xml.AppendStartTag(TranslatorTags.Data);
                        GXDataInfo di = new GXDataInfo();
                        di.xml = data.Xml;
                        GXCommon.GetData(settings, data.Data, di);
                        data.Xml.AppendEndTag(TranslatorTags.Data);
                    }
                    data.Xml.AppendEndTag(TranslatorTags.ReturnParameters);

                    if (data.Xml.OutputType == TranslatorOutputType.StandardXml)
                    {
                        data.Xml.AppendEndTag(TranslatorTags.SingleResponse);
                    }
                }
            }
        }

        internal static void AddInvokeId(GXDLMSTranslatorStructure xml, Command command, Enum type, UInt32 invokeId)
        {
            if (xml != null)
            {
                xml.AppendStartTag(command);
                xml.AppendStartTag(command, type);
                //InvokeIdAndPriority
                if (xml.Comments)
                {
                    StringBuilder sb = new StringBuilder();
                    if ((invokeId & 0x80) != 0)
                    {
                        sb.Append("Priority: High, ");
                    }
                    else
                    {
                        sb.Append("Priority: Normal, ");
                    }
                    if ((invokeId & 0x40) != 0)
                    {
                        sb.Append("ServiceClass: Confirmed, ");
                    }
                    else
                    {
                        sb.Append("ServiceClass: UnConfirmed, ");
                    }
                    sb.Append("Invoke ID: " + (invokeId & 0xF).ToString());
                    xml.AppendComment(sb.ToString());
                }
                xml.AppendLine(TranslatorTags.InvokeId, null, xml.IntegerToHex(invokeId, 2));
            }
        }

        private static bool HandleActionResponseWithBlock(
           GXDLMSSettings settings,
           GXReplyData reply,
           int index)
        {
            bool ret = true;
            short ch;
            long number;
            ch = reply.Data.GetUInt8();
            if (reply.Xml != null)
            {
                //Result start tag.
                reply.Xml.AppendStartTag(TranslatorTags.Pblock);
                //LastBlock
                reply.Xml.AppendLine(TranslatorTags.LastBlock, "Value", reply.Xml.IntegerToHex(ch, 2));
            }
            if (ch == 0)
            {
                reply.MoreData = (RequestTypes)(reply.MoreData | RequestTypes.DataBlock);
            }
            else
            {
                reply.MoreData = (RequestTypes)(reply.MoreData & ~RequestTypes.DataBlock);
            }
            // Get Block number.
            number = reply.Data.GetUInt32();
            if (reply.Xml != null)
            {
                //BlockNumber
                reply.Xml.AppendLine(TranslatorTags.BlockNumber, "Value", reply.Xml.IntegerToHex(number, 8));
            }
            else
            {
                //Update  initial block index. This is critical if message is send and received in multiple blocks.
                if (number == 1)
                {
                    settings.ResetBlockIndex();
                }
                if (number != settings.BlockIndex)
                {
                    throw new ArgumentException(
                        "Invalid Block number. It is " + number
                        + " and it should be " + settings.BlockIndex + ".");
                }
            }
            //Note! There is no status!!
            if (reply.Xml != null)
            {
                if (reply.Data.Available != 0)
                {
                    // Get data size.
                    int blockLength = GXCommon.GetObjectCount(reply.Data);
                    // if whole block is read.
                    if ((reply.MoreData & RequestTypes.Frame) == 0)
                    {
                        // Check Block length.
                        if (blockLength > reply.Data.Size - reply.Data.Position)
                        {
                            reply.Xml.AppendComment("Block is not complete." + (reply.Data.Size - reply.Data.Position).ToString() + "/" + blockLength + ".");
                        }
                    }
                    reply.Xml.AppendLine(TranslatorTags.RawData, "Value",
                                         GXCommon.ToHex(reply.Data.Data, false, reply.Data.Position, reply.Data.Available));
                }
                reply.Xml.AppendEndTag(TranslatorTags.Pblock);
            }
            else if (reply.Data.Available != 0)
            {
                // Get data size.
                int blockLength = GXCommon.GetObjectCount(reply.Data);
                // if whole block is read.
                if ((reply.MoreData & RequestTypes.Frame) == 0)
                {
                    // Check Block length.
                    if (blockLength > reply.Data.Available)
                    {
                        throw new OutOfMemoryException();
                    }
                    //Keep command if this is last block for XML Client.
                    if ((reply.MoreData & RequestTypes.DataBlock) != 0)
                    {
                        reply.Command = Command.None;
                    }
                }
                if (blockLength == 0)
                {
                    //If meter sends empty data block.
                    reply.Data.Size = index;
                }
                else
                {
                    GetDataFromBlock(reply.Data, index);
                }
                // If last packet and data is not try to peek.
                if (reply.MoreData == RequestTypes.None)
                {
                    if (!reply.Peek)
                    {
                        reply.Data.Position = 0;
                    }
                    settings.ResetBlockIndex();
                }
            }
            else if (reply.Data.Position == reply.Data.Size)
            {
                //Empty block. Conformance tests uses this.
                reply.EmptyResponses |= RequestTypes.DataBlock;
            }
            if (reply.MoreData == RequestTypes.None && settings != null && settings.Command == Command.MethodRequest &&
                settings.CommandType == (byte)ActionResponseType.WithList)
            {
                throw new NotImplementedException("Action with list is not implemented.");
            }
            return ret;
        }

        /// <summary>
        /// Handle method response and get data from block and/or update error status.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data">Received data from the client.</param>
        static void HandleMethodResponse(GXDLMSSettings settings,
                                         GXReplyData data,
                                         int index)
        {
            // Get type.
            ActionResponseType type = (ActionResponseType)data.Data.GetUInt8();
            // Get invoke ID and priority.
            data.InvokeId = data.Data.GetUInt8();
            VerifyInvokeId(settings, data);
            AddInvokeId(data.Xml, Command.MethodResponse, type, data.InvokeId);
            switch (type)
            {
                case ActionResponseType.Normal:
                    HandleActionResponseNormal(settings, data);
                    break;
                case ActionResponseType.WithBlock:
                    HandleActionResponseWithBlock(settings, data, index);
                    break;
                case ActionResponseType.WithList:
                    throw new ArgumentException("Invalid Command.");
                case ActionResponseType.NextBlock:
                    UInt32 number = data.Data.GetUInt32();
                    if (data.Xml != null)
                    {
                        data.Xml.AppendLine(TranslatorTags.BlockNumber, "Value", data.Xml.IntegerToHex(number, 8));
                    }
                    else if (number != settings.BlockIndex)
                    {
                        throw new ArgumentException(
                            "Invalid Block number. It is " + number
                            + " and it should be " + settings.BlockIndex + ".");
                    }
                    settings.IncreaseBlockIndex();
                    break;
                default:
                    throw new ArgumentException("Invalid Command.");
            }
            if (data.Xml != null)
            {
                data.Xml.AppendEndTag(Command.MethodResponse, type);
                data.Xml.AppendEndTag(Command.MethodResponse);
            }
        }

        static void HandleSetResponse(GXDLMSSettings settings, GXReplyData data)
        {
            SetResponseType type = (SetResponseType)data.Data.GetUInt8();
            //Invoke ID and priority.
            data.InvokeId = data.Data.GetUInt8();
            VerifyInvokeId(settings, data);
            AddInvokeId(data.Xml, Command.SetResponse, type, data.InvokeId);
            //SetResponseNormal
            if (type == SetResponseType.Normal)
            {
                data.Error = data.Data.GetUInt8();
                if (data.Xml != null)
                {
                    // Result start tag.
                    data.Xml.AppendLine(TranslatorTags.Result, "Value",
                                        GXDLMSTranslator.ErrorCodeToString(
                                            data.Xml.OutputType,
                                            (ErrorCode)data.Error));
                }
            }
            else if (type == SetResponseType.DataBlock)
            {
                UInt32 number = data.Data.GetUInt32();
                if (data.Xml != null)
                {
                    data.Xml.AppendLine(TranslatorTags.BlockNumber, "Value", data.Xml.IntegerToHex(number, 8));
                }
            }
            else if (type == SetResponseType.LastDataBlock)
            {
                data.Error = data.Data.GetUInt8();
                UInt32 number = data.Data.GetUInt32();
                if (data.Xml != null)
                {
                    // Result start tag.
                    data.Xml.AppendLine(TranslatorTags.Result, "Value",
                                        GXDLMSTranslator.ErrorCodeToString(
                                            data.Xml.OutputType,
                                            (ErrorCode)data.Error));
                    data.Xml.AppendLine(TranslatorTags.BlockNumber, "Value", data.Xml.IntegerToHex(number, 8));
                }
            }
            else if (type == SetResponseType.WithList)
            {
                int cnt = GXCommon.GetObjectCount(data.Data);
                if (data.Xml != null)
                {
                    data.Xml.AppendStartTag(TranslatorTags.Result, "Qty", cnt.ToString());
                    for (int pos = 0; pos != cnt; ++pos)
                    {
                        int err = data.Data.GetUInt8();
                        data.Xml.AppendLine(TranslatorTags.DataAccessResult, "Value",
                            GXDLMSTranslator.ErrorCodeToString(data.Xml.OutputType, (ErrorCode)err));
                    }
                    data.Xml.AppendEndTag(TranslatorTags.Result);
                }
                else
                {
                    for (int pos = 0; pos != cnt; ++pos)
                    {
                        byte err = data.Data.GetUInt8();
                        if (data.Error == 0 && err != 0)
                        {
                            data.Error = err;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid data type.");
            }
            if (data.Xml != null)
            {
                data.Xml.AppendEndTag(Command.SetResponse, type);
                data.Xml.AppendEndTag(Command.SetResponse);
            }
        }

        /// <summary>
        /// Handle write response and get data from block.
        /// </summary>
        /// <param name="reply">Received data from the client.</param>
        static void HandleWriteResponse(GXReplyData data)
        {
            int cnt = GXCommon.GetObjectCount(data.Data);
            byte ret;
            if (data.Xml != null)
            {
                data.Xml.AppendStartTag(Command.WriteResponse, "Qty", data.Xml.IntegerToHex(cnt, 2));
            }
            for (int pos = 0; pos != cnt; ++pos)
            {
                ret = data.Data.GetUInt8();
                if (ret != 0)
                {
                    data.Error = data.Data.GetUInt8();
                }
                if (data.Xml != null)
                {
                    if (ret == 0)
                    {
                        data.Xml.AppendLine("<" + ((ErrorCode)ret).ToString() + " />");
                    }
                    else
                    {
                        data.Xml.AppendLine(TranslatorTags.DataAccessError, "Value",
                                            ((ErrorCode)data.Error).ToString());
                    }
                }
            }
            if (data.Xml != null)
            {
                data.Xml.AppendEndTag(Command.WriteResponse);
            }
        }


        /// <summary>
        /// Handle get response with list.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="reply">Received data from the client.</param>
        static void HandleGetResponseWithList(GXDLMSSettings settings, GXReplyData reply)
        {
            byte ch = 0;
            //Get object count.
            int cnt = GXCommon.GetObjectCount(reply.Data);
            List<object> values = new List<object>(cnt);
            if (reply.Xml != null)
            {
                //Result start tag.
                reply.Xml.AppendStartTag(TranslatorTags.Result, "Qty", reply.Xml.IntegerToHex(cnt, 2));
            }
            for (int pos = 0; pos != cnt; ++pos)
            {
                // Result
                ch = reply.Data.GetUInt8();
                if (ch != 0)
                {
                    reply.Error = reply.Data.GetUInt8();
                }
                else
                {
                    if (reply.Xml != null)
                    {
                        GXDataInfo di = new GXDataInfo();
                        di.xml = reply.Xml;
                        //Data.
                        reply.Xml.AppendStartTag(Command.ReadResponse, SingleReadResponse.Data);
                        GXCommon.GetData(settings, reply.Data, di);
                        reply.Xml.AppendEndTag(Command.ReadResponse, SingleReadResponse.Data);
                    }
                    else
                    {
                        reply.ReadPosition = reply.Data.Position;
                        GetValueFromData(settings, reply);
                        reply.Data.Position = reply.ReadPosition;
                        values.Add(reply.Value);
                        reply.Value = null;
                    }
                }
            }
            reply.Value = values;
        }

        private static void VerifyInvokeId(GXDLMSSettings settings, GXReplyData reply)
        {
            if (reply.Xml == null && settings.AutoIncreaseInvokeID && reply.InvokeId != GetInvokeIDPriority(settings, false))
            {
                throw new Exception(string.Format("Invalid invoke ID. Expected: {0} Actual: {1}", GetInvokeIDPriority(settings, false).ToString("X"), reply.InvokeId.ToString("X")));
            }
        }

        /// <summary>
        /// Handle get response and get data from block and/or update error status.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="reply">Received data from the client.</param>
        /// <param name="index">Block index number.</param>
        static bool HandleGetResponse(GXDLMSSettings settings,
                                      GXReplyData reply, int index)
        {
            bool ret = true;
            GXByteBuffer data = reply.Data;
            bool empty = false;
            // Get type.
            GetCommandType type = (GetCommandType)data.GetUInt8();
            reply.CommandType = (byte)type;
            // Get invoke ID and priority.
            reply.InvokeId = data.GetUInt8();
            VerifyInvokeId(settings, reply);
            AddInvokeId(reply.Xml, Command.GetResponse, type, reply.InvokeId);
            switch (type)
            {
                case GetCommandType.Normal:
                    empty = HandleGetResponseNormal(settings, reply, data);
                    break;
                case GetCommandType.NextDataBlock:
                    // Is Last block.
                    ret = HandleGetResponseNextDataBlock(settings, reply, index, data);
                    break;
                case GetCommandType.WithList:
                    HandleGetResponseWithList(settings, reply);
                    ret = false;
                    break;
                default:
                    throw new ArgumentException("Invalid Get response.");
            }
            if (reply.Xml != null)
            {
                if (!empty)
                {
                    reply.Xml.AppendEndTag(TranslatorTags.Result);
                }
                reply.Xml.AppendEndTag(Command.GetResponse, type);
                reply.Xml.AppendEndTag(Command.GetResponse);
            }
            return ret;
        }

        private static bool HandleGetResponseNextDataBlock(
            GXDLMSSettings settings,
            GXReplyData reply,
            int index,
            GXByteBuffer data)
        {
            bool ret = true;
            short ch;
            long number;
            ch = data.GetUInt8();
            if (reply.Xml != null)
            {
                //Result start tag.
                reply.Xml.AppendStartTag(TranslatorTags.Result);
                //LastBlock
                reply.Xml.AppendLine(TranslatorTags.LastBlock, "Value", reply.Xml.IntegerToHex(ch, 2));
            }
            if (ch == 0)
            {
                reply.MoreData = (RequestTypes)(reply.MoreData | RequestTypes.DataBlock);
            }
            else
            {
                reply.MoreData = (RequestTypes)(reply.MoreData & ~RequestTypes.DataBlock);
            }
            // Get Block number.
            number = data.GetUInt32();
            if (reply.Xml != null)
            {
                //BlockNumber
                reply.Xml.AppendLine(TranslatorTags.BlockNumber, "Value", reply.Xml.IntegerToHex(number, 8));
            }
            else
            {
                //If meter's block index is zero based.
                if (number != 1 && settings.BlockIndex == 1)
                {
                    settings.BlockIndex = (uint)number;
                }
                if (number != settings.BlockIndex)
                {
                    throw new ArgumentException(
                        "Invalid Block number. It is " + number
                        + " and it should be " + settings.BlockIndex + ".");
                }
            }
            // Get status.
            ch = data.GetUInt8();
            if (ch != 0)
            {
                reply.Error = data.GetUInt8();
            }
            if (reply.Xml != null)
            {
                //Result
                reply.Xml.AppendStartTag(TranslatorTags.Result);
                if (reply.Error != 0)
                {
                    reply.Xml.AppendLine(TranslatorTags.DataAccessResult, "Value",
                        GXDLMSTranslator.ErrorCodeToString(reply.Xml.OutputType, (ErrorCode)reply.Error));
                }
                else if (reply.Data.Available != 0)
                {
                    // Get data size.
                    int blockLength = GXCommon.GetObjectCount(data);
                    // if whole block is read.
                    if ((reply.MoreData & RequestTypes.Frame) == 0)
                    {
                        // Check Block length.
                        if (blockLength > data.Available)
                        {
                            reply.Xml.AppendComment("Block is not complete." + (data.Size - data.Position).ToString() + "/" + blockLength + ".");
                        }
                    }
                    reply.Xml.AppendLine(TranslatorTags.RawData, "Value",
                                         GXCommon.ToHex(reply.Data.Data, false, data.Position, reply.Data.Available));
                }
                reply.Xml.AppendEndTag(TranslatorTags.Result);
            }
            else if (reply.Data.Available != 0)
            {
                // Get data size.
                int blockLength = GXCommon.GetObjectCount(data);
                // if whole block is read.
                if ((reply.MoreData & RequestTypes.Frame) == 0)
                {
                    // Check Block length.
                    if (blockLength > data.Size - data.Position)
                    {
                        throw new OutOfMemoryException();
                    }
                    //Keep command if this is last block for XML Client.
                    if ((reply.MoreData & RequestTypes.DataBlock) != 0)
                    {
                        reply.Command = Command.None;
                    }
                }
                if (blockLength == 0)
                {
                    //If meter sends empty data block.
                    data.Size = index;
                }
                else
                {
                    GetDataFromBlock(data, index);
                }
                // If last packet and data is not try to peek.
                if (reply.MoreData == RequestTypes.None)
                {
                    if (!reply.Peek)
                    {
                        data.Position = 0;
                    }
                    settings.ResetBlockIndex();
                }
            }
            else if (data.Position == data.Size)
            {
                //Empty block. Conformance tests uses this.
                reply.EmptyResponses |= RequestTypes.DataBlock;
            }
            if (reply.MoreData == RequestTypes.None && settings != null && settings.Command == Command.GetRequest &&
                settings.CommandType == (byte)GetCommandType.WithList)
            {
                HandleGetResponseWithList(settings, reply);
                ret = false;
            }

            return ret;
        }

        private static bool HandleGetResponseNormal(GXDLMSSettings settings, GXReplyData reply, GXByteBuffer data)
        {
            bool empty = false;
            if (data.Available == 0)
            {
                empty = true;
                GetDataFromBlock(data, 0);
            }
            else
            {
                // Result
                short ch = data.GetUInt8();
                if (ch != 0)
                {
                    reply.Error = data.GetUInt8();
                }
                if (reply.Xml != null)
                {
                    // Result start tag.
                    reply.Xml.AppendStartTag(TranslatorTags.Result);
                    if (reply.Error != 0)
                    {
                        reply.Xml.AppendLine(TranslatorTags.DataAccessError,
                                             "Value",
                                             GXDLMSTranslator.ErrorCodeToString(
                                                 reply.Xml.OutputType, (ErrorCode)reply.Error));
                    }
                    else
                    {
                        reply.Xml.AppendStartTag(TranslatorTags.Data);
                        GXDataInfo di = new GXDataInfo();
                        di.xml = reply.Xml;
                        GXCommon.GetData(settings, reply.Data, di);
                        reply.Xml.AppendEndTag(TranslatorTags.Data);
                    }
                }
                else
                {
                    GetDataFromBlock(data, 0);
                }
            }
            return empty;
        }

        /// <summary>
        /// Handle General block transfer message.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data"></param>
        internal static void HandleGbt(
            GXDLMSSettings settings,
            GXReplyData data)
        {
            int index = data.Data.Position - 1;
            data.GbtWindowSize = settings.GbtWindowSize;
            //BlockControl
            byte bc = data.Data.GetUInt8();
            //Is streaming active.
            data.Streaming = (bc & 0x40) != 0;
            //GBT Window size.
            byte windowSize = (byte)(bc & 0x3F);
            //Block number.
            UInt16 bn = data.Data.GetUInt16();
            //Block number acknowledged.
            UInt16 bna = data.Data.GetUInt16();
            if (data.Xml == null)
            {
                // Remove existing data when first block is received.
                if (bn == 1)
                {
                    index = 0;
                }
                else if (bna != settings.BlockIndex - 1)
                {
                    // If this block is already received.
                    data.Data.Size = index;
                    data.Command = Command.None;
                    return;
                }
            }
            data.BlockNumber = bn;
            data.BlockNumberAck = bna;
            settings.BlockNumberAck = data.BlockNumber;
            data.Command = Command.None;
            int len = GXCommon.GetObjectCount(data.Data);
            if (len > data.Data.Size - data.Data.Position)
            {
                data.IsComplete = false;
                return;
            }
            if (data.Xml != null)
            {
                if ((data.Data.Size - data.Data.Position) != len)
                {
                    data.Xml.AppendComment("Data length is " + len
                                    + " and there are " + (data.Data.Size - data.Data.Position)
                                    + " bytes.");
                }
                data.Xml.AppendStartTag(Command.GeneralBlockTransfer);
                if (data.Xml.Comments)
                {
                    data.Xml.AppendComment("Last block: " + ((bc & 0x80) != 0));
                    data.Xml.AppendComment("Streaming: " + data.Streaming);
                    data.Xml.AppendComment("Window size: " + windowSize);
                }
                data.Xml.AppendLine(TranslatorTags.BlockControl, null, data.Xml.IntegerToHex(bc, 2));
                data.Xml.AppendLine(TranslatorTags.BlockNumber, null, data.Xml.IntegerToHex(data.BlockNumber, 4));
                data.Xml.AppendLine(TranslatorTags.BlockNumberAck, null, data.Xml.IntegerToHex(data.BlockNumberAck, 4));
                //If last block and not streaming and comments.
                if ((bc & 0x80) != 0 && !data.Streaming && data.Xml.Comments && data.Data.Available != 0)
                {
                    int pos = data.Data.Position;
                    int len2 = data.Xml.GetXmlLength();
                    try
                    {
                        GXReplyData reply = new GXReplyData();
                        reply.Data = data.Data;
                        reply.Xml = data.Xml;
                        reply.Xml.StartComment("");
                        GetPdu(settings, reply);
                        reply.Xml.EndComment();
                    }
                    catch (Exception)
                    {
                        data.Xml.SetXmlLength(len2);
                        //It's ok if this fails.
                    }
                    data.Data.Position = pos;
                }
                data.Xml.AppendLine(TranslatorTags.BlockData, null, data.Data.RemainingHexString(true));
                data.Xml.AppendEndTag(Command.GeneralBlockTransfer);
                return;
            }
            GetDataFromBlock(data.Data, index);
            //Is Last block,
            if ((bc & 0x80) == 0)
            {
                data.MoreData |= RequestTypes.GBT;
            }
            else
            {
                data.MoreData &= ~RequestTypes.GBT;
                if (data.Data.Size != 0)
                {
                    data.Data.Position = 0;
                    GetPdu(settings, data);
                }
                // Get data if all data is read or we want to peek data.
                if (data.Data.Position != data.Data.Size
                        && (data.Command == Command.ReadResponse || data.Command == Command.GetResponse)
                        && (data.MoreData == RequestTypes.None
                            || data.Peek))
                {
                    data.Data.Position = 0;
                    GetValueFromData(settings, data);
                }
            }
        }

        internal static void HandleConfirmedServiceError(GXReplyData data)
        {
            if (data.Xml != null)
            {
                data.Xml.AppendStartTag(Command.ConfirmedServiceError);
                if (data.Xml.OutputType == TranslatorOutputType.StandardXml)
                {
                    data.Data.GetUInt8();
                    data.Xml.AppendStartTag(TranslatorTags.InitiateError);
                    ServiceError type = (ServiceError)data.Data.GetUInt8();

                    String tag = TranslatorStandardTags.ServiceErrorToString(type);
                    String value = TranslatorStandardTags.GetServiceErrorValue(type,
                                   (byte)data.Data.GetUInt8());
                    data.Xml.AppendLine("x:" + tag, null, value);
                    data.Xml.AppendEndTag(TranslatorTags.InitiateError);
                }
                else
                {
                    data.Xml.AppendLine(TranslatorTags.Service, "Value", data
                                        .Xml.IntegerToHex(data.Data.GetUInt8(), 2));
                    ServiceError type = (ServiceError)data.Data.GetUInt8();
                    data.Xml.AppendStartTag(TranslatorTags.ServiceError);
                    data.Xml.AppendLine(
                        TranslatorSimpleTags.ServiceErrorToString(type),
                        "Value", TranslatorSimpleTags.GetServiceErrorValue(type,
                                data.Data.GetUInt8()));
                    data.Xml.AppendEndTag(TranslatorTags.ServiceError);
                }
                data.Xml.AppendEndTag(Command.ConfirmedServiceError);
            }
            else
            {
                ConfirmedServiceError service = (ConfirmedServiceError)data.Data.GetUInt8();
                ServiceError type = (ServiceError)data.Data.GetUInt8();
                throw new GXDLMSConfirmedServiceError(service, type, data.Data.GetUInt8());
            }
        }

        internal static void HandleExceptionResponse(GXReplyData data)
        {
            ExceptionStateError state = (ExceptionStateError)data.Data.GetUInt8();
            ExceptionServiceError error = (ExceptionServiceError)data.Data.GetUInt8();
            object value = null;
            if (error == ExceptionServiceError.InvocationCounterError && data.Data.Available > 3)
            {
                value = data.Data.GetUInt32();
            }
            if (data.Xml != null)
            {
                data.Xml.AppendStartTag(Command.ExceptionResponse);
                if (data.Xml.OutputType == TranslatorOutputType.StandardXml)
                {
                    data.Xml.AppendLine(TranslatorTags.StateError, null,
                          TranslatorStandardTags.StateErrorToString(state));
                    data.Xml.AppendLine(TranslatorTags.ServiceError, null,
                            TranslatorStandardTags.ExceptionServiceErrorToString(error));
                }
                else
                {
                    data.Xml.AppendLine(TranslatorTags.StateError, null,
                         TranslatorSimpleTags.StateErrorToString(state));
                    data.Xml.AppendLine(TranslatorTags.ServiceError, null, TranslatorSimpleTags.ExceptionServiceErrorToString(error));
                }
                data.Xml.AppendEndTag(Command.ExceptionResponse);
            }
            else
            {
                throw new GXDLMSExceptionResponse(state, error, value);
            }
        }

        private static void HandleGloDedRequest(GXDLMSSettings settings,
                                              GXReplyData data)
        {
            if (data.Xml != null && !data.Xml.Comments)
            {
                --data.Data.Position;
            }
            else
            {
                if (settings.Cipher == null)
                {
                    throw new Exception(
                        "Secure connection is not supported.");
                }
                //If all frames are read.
                if ((data.MoreData & RequestTypes.Frame) == 0)
                {
                    --data.Data.Position;
                    int pos = data.Data.Position;
                    //Return copy from data because ciphering is changing it.
                    byte[] encrypted = data.Data.Array();
                    //If external Hardware Security Module is used.
                    byte[] ret = settings.Crypt(CertificateType.DigitalSignature, encrypted, false);
                    if (ret != null)
                    {
                        encrypted = data.Data.Array();
                        data.Data.Size = 0;
                        data.Data.Set(ret);
                    }
                    else
                    {
                        AesGcmParameter p;
                        GXICipher cipher = settings.Cipher;
                        if (data.Command == Command.GeneralDedCiphering)
                        {
                            p = new AesGcmParameter(settings, settings.SourceSystemTitle,
                                    cipher.DedicatedKey,
                                    GetAuthenticationKey(settings));
                        }
                        else if (data.Command == Command.GeneralGloCiphering)
                        {
                            p = new AesGcmParameter(settings, settings.SourceSystemTitle,
                                    GetBlockCipherKey(settings),
                                    GetAuthenticationKey(settings));
                        }
                        else if (cipher.DedicatedKey == null || IsGloMessage(data.Command))
                        {
                            p = new AesGcmParameter(settings, settings.SourceSystemTitle,
                                    GetBlockCipherKey(settings),
                                    GetAuthenticationKey(settings));
                        }
                        else
                        {
                            p = new AesGcmParameter(settings, settings.SourceSystemTitle,
                                    cipher.DedicatedKey,
                                    GetAuthenticationKey(settings));
                        }
                        byte[] tmp = GXCiphering.Decrypt(p, data.Data);
                        cipher.SecuritySuite = p.SecuritySuite;
                        if (settings.CryptoNotifier != null && settings.CryptoNotifier.pdu != null && data.IsComplete && (data.MoreData & RequestTypes.Frame) == 0)
                        {
                            settings.CryptoNotifier.pdu(settings.CryptoNotifier, tmp);
                        }
                        data.Data.Clear();
                        data.Data.Set(tmp);
                    }
                    // Get command.
                    data.CipheredCommand = data.Command;
                    data.Command = (Command)data.Data.GetUInt8();
                    //Validate command. Command is not valid is block cipher key is wrong.
                    if (!Enum.IsDefined(typeof(Command), data.Command))
                    {
                        data.Data.Clear();
                        data.Data.Set(encrypted);
                        data.Data.Position = pos;
                        throw new Exception("Failed to decrypt the data.");
                    }
                    if (data.Command == Command.DataNotification
                        || data.Command == Command.InformationReport)
                    {
                        data.Command = Command.None;
                        --data.Data.Position;
                        GetPdu(settings, data);
                    }
                }
                else
                {
                    --data.Data.Position;
                }
            }
        }

        private static void HandleGloDedResponse(
            GXDLMSSettings settings,
            GXReplyData data,
            int index)
        {
            if (data.Xml != null && !data.Xml.Comments)
            {
                --data.Data.Position;
            }
            else
            {
                if (settings.Cipher == null)
                {
                    throw new Exception(
                        "Secure connection is not supported.");
                }
                //If all frames are read.
                if ((data.MoreData & RequestTypes.Frame) == 0)
                {
                    --data.Data.Position;
                    GXByteBuffer bb = new GXByteBuffer(data.Data);
                    data.Data.Position = data.Data.Size = index;
                    //If external Hardware Security Module is used.
                    byte[] ret = settings.Crypt(CertificateType.DigitalSignature, bb.Array(), false);
                    if (ret != null)
                    {
                        data.Data.Set(ret);
                    }
                    else
                    {
                        AesGcmParameter p;
                        GXICipher cipher = settings.Cipher;
                        if (cipher.DedicatedKey != null
                                && (settings.Connected & ConnectionState.Dlms) != 0)
                        {
                            p = new AesGcmParameter(settings,
                                settings.SourceSystemTitle,
                                cipher.DedicatedKey,
                                GetAuthenticationKey(settings));
                        }
                        else
                        {
                            if (settings.PreEstablishedSystemTitle != null && (settings.Connected & ConnectionState.Dlms) == 0)
                            {
                                p = new AesGcmParameter(settings,
                                settings.PreEstablishedSystemTitle,
                                GetBlockCipherKey(settings),
                                GetAuthenticationKey(settings));
                            }
                            else
                            {
                                if (settings.SourceSystemTitle == null && (settings.Connected & ConnectionState.Dlms) != 0)
                                {
                                    if (settings.IsServer)
                                    {
                                        throw new Exception("Ciphered failed. Client system title is unknown.");
                                    }
                                    else
                                    {
                                        throw new Exception("Ciphered failed. Server system title is unknown.");
                                    }
                                }
                                p = new AesGcmParameter(settings,
                                    settings.SourceSystemTitle,
                                    GetBlockCipherKey(settings),
                                    GetAuthenticationKey(settings));
                            }
                        }
                        byte[] tmp = GXCiphering.Decrypt(p, bb);
                        data.Data.Set(tmp);
                        //If target is sending data ciphered using different security policy.
                        if (!settings.Cipher.SecurityChangeCheck && (settings.Connected & ConnectionState.Dlms) != 0 &&
                            settings.Cipher.Security != Security.None && settings.Cipher.Signing != Signing.GeneralSigning &&
                            settings.Cipher.Security != p.Security)
                        {
                            throw new GXDLMSCipherException(string.Format("Data is ciphered using different security level. Actual: {0}. Expected: {1}", p.Security, settings.Cipher.Security));
                        }
                        if (settings.ExpectedInvocationCounter != 0)
                        {
                            if (p.InvocationCounter < settings.ExpectedInvocationCounter)
                            {
                                throw new GXDLMSCipherException(string.Format("Data is ciphered using invalid invocation counter value. Actual: {0}. Expected: {1}", p.InvocationCounter, settings.ExpectedInvocationCounter));
                            }
                            settings.ExpectedInvocationCounter = p.InvocationCounter;
                        }
                        if (settings.CryptoNotifier != null && settings.CryptoNotifier.pdu != null)
                        {
                            settings.CryptoNotifier.pdu(settings.CryptoNotifier, tmp);
                        }
                    }
                    data.CipheredCommand = data.Command;
                    data.Command = Command.None;
                    GetPdu(settings, data);
                    data.CipherIndex = data.Data.Size;
                }
            }
        }

        /// <summary>
        /// Get PDU from the packet.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data">received data.</param>
        public static void GetPdu(
            GXDLMSSettings settings,
            GXReplyData data)
        {
            short ch;
            Command cmd = data.Command;
            // If header is not read yet or GBT message.
            if (data.Command == Command.None)
            {
                //If PDU is missing.
                if (data.Data.Size - data.Data.Position == 0)
                {
                    throw new InvalidOperationException("Invalid PDU.");
                }
                int index = data.Data.Position;
                // Get command.
                ch = data.Data.GetUInt8();
                cmd = (Command)ch;
                data.Command = cmd;
                switch (cmd)
                {
                    case Command.ReadResponse:
                        if (!HandleReadResponse(settings, data, index))
                        {
                            return;
                        }
                        break;
                    case Command.GetResponse:
                        if (!HandleGetResponse(settings, data, index))
                        {
                            return;
                        }
                        break;
                    case Command.SetResponse:
                        HandleSetResponse(settings, data);
                        break;
                    case Command.WriteResponse:
                        HandleWriteResponse(data);
                        break;
                    case Command.MethodResponse:
                        HandleMethodResponse(settings, data, index);
                        break;
                    case Command.AccessRequest:
                        if (data.Xml != null || (!settings.IsServer && (data.MoreData & RequestTypes.Frame) == 0))
                        {
                            GXDLMSLNCommandHandler.HandleAccessRequest(settings, null, data.Data, null, data.Xml, Command.None);
                        }

                        break;
                    case Command.AccessResponse:
                        if (data.Xml != null || (!settings.IsServer && (data.MoreData & RequestTypes.Frame) == 0))
                        {
                            HandleAccessResponse(settings, data);
                        }
                        break;
                    case Command.GeneralBlockTransfer:
                        if (data.Xml != null || (!settings.IsServer && (data.MoreData & RequestTypes.Frame) == 0))
                        {
                            HandleGbt(settings, data);
                        }
                        break;
                    case Command.Aarq:
                    case Command.Aare:
                        // This is parsed later.
                        --data.Data.Position;
                        break;
                    case Command.ReleaseResponse:
                        break;
                    case Command.ConfirmedServiceError:
                        HandleConfirmedServiceError(data);
                        break;
                    case Command.ExceptionResponse:
                        HandleExceptionResponse(data);
                        break;
                    case Command.GetRequest:
                        if (data.Xml != null || (!settings.IsServer && (data.MoreData & RequestTypes.Frame) == 0))
                        {
                            GXDLMSLNCommandHandler.HandleGetRequest(settings, null, data.Data, null, data.Xml, Command.None);
                        }
                        break;
                    case Command.ReadRequest:
                        if (data.Xml != null || (!settings.IsServer && (data.MoreData & RequestTypes.Frame) == 0))
                        {
                            GXDLMSSNCommandHandler.HandleReadRequest(settings, null, data.Data, null, data.Xml, Command.None);
                        }
                        break;
                    case Command.WriteRequest:
                        if (data.Xml != null || (!settings.IsServer && (data.MoreData & RequestTypes.Frame) == 0))
                        {
                            GXDLMSSNCommandHandler.HandleWriteRequest(settings, null, data.Data, null, data.Xml, Command.None);
                        }
                        break;
                    case Command.SetRequest:
                        if (data.Xml != null || (!settings.IsServer && (data.MoreData & RequestTypes.Frame) == 0))
                        {
                            GXDLMSLNCommandHandler.HandleSetRequest(settings, null, data.Data, null, data.Xml, Command.None);
                        }
                        break;
                    case Command.MethodRequest:
                        if (data.Xml != null || (!settings.IsServer && (data.MoreData & RequestTypes.Frame) == 0))
                        {
                            GXDLMSLNCommandHandler.HandleMethodRequest(settings, null, data.Data, null, null, data.Xml, Command.None);
                        }
                        break;
                    case Command.ReleaseRequest:
                        // Server handles this.
                        if ((data.MoreData & RequestTypes.Frame) != 0)
                        {
                            break;
                        }
                        break;
                    case Command.GloReadRequest:
                    case Command.GloWriteRequest:
                    case Command.GloGetRequest:
                    case Command.GloSetRequest:
                    case Command.GloMethodRequest:
                    case Command.DedGetRequest:
                    case Command.DedSetRequest:
                    case Command.DedMethodRequest:
                        HandleGloDedRequest(settings, data);
                        // Server handles this.
                        break;
                    case Command.GloReadResponse:
                    case Command.GloWriteResponse:
                    case Command.GloGetResponse:
                    case Command.GloSetResponse:
                    case Command.GloMethodResponse:
                    case Command.GloEventNotification:
                    case Command.DedGetResponse:
                    case Command.DedSetResponse:
                    case Command.DedMethodResponse:
                    case Command.DedEventNotification:
                    case Command.GloConfirmedServiceError:
                    case Command.DedConfirmedServiceError:
                        HandleGloDedResponse(settings, data, index);
                        break;
                    case Command.GeneralGloCiphering:
                    case Command.GeneralDedCiphering:
                        if (settings.IsServer)
                        {
                            HandleGloDedRequest(settings, data);
                        }
                        else
                        {
                            HandleGloDedResponse(settings, data, index);
                        }
                        break;
                    case Command.GeneralSigning:
                        if (settings.IsServer)
                        {
                            HandleGloDedRequest(settings, data);
                        }
                        else
                        {
                            HandleGloDedResponse(settings, data, index);
                        }
                        break;
                    case Command.DataNotification:
                        HandleDataNotification(settings, data);
                        //Client handles this.
                        break;
                    case Command.EventNotification:
                        //Client handles this.
                        break;
                    case Command.InformationReport:
                        //Client handles this.
                        break;
                    case Command.GeneralCiphering:
                        HandleGeneralCiphering(settings, data);
                        break;
                    case Command.GatewayRequest:
                    case Command.GatewayResponse:
                        data.Gateway = new GXDLMSGateway();
                        data.Gateway.NetworkId = data.Data.GetUInt8();
                        int len = GXCommon.GetObjectCount(data.Data);
                        data.Gateway.PhysicalDeviceAddress = new byte[len];
                        data.Data.Get(data.Gateway.PhysicalDeviceAddress);
                        GetDataFromBlock(data.Data, index);
                        data.Command = Command.None;
                        GetPdu(settings, data);
                        break;
                    case Command.PingResponse:
                    case Command.DiscoverReport:
                    case Command.DiscoverRequest:
                    case Command.RegisterRequest:
                        break;
                    default:
                        data.Command = Command.None;
                        throw new ArgumentException("Invalid Command.");
                }
            }
            else if ((data.MoreData & RequestTypes.Frame) == 0)
            {
                // Is whole block is read and if last packet and data is not try to
                // peek.
                if (!data.Peek && data.MoreData == RequestTypes.None)
                {
                    if (data.Command == Command.Aare || data.Command == Command.Aarq)
                    {
                        data.Data.Position = 0;
                    }
                    else
                    {
                        data.Data.Position = 1;
                    }
                }
                if (cmd == Command.GeneralBlockTransfer)
                {
                    if (data.Xml != null || !settings.IsServer)
                    {
                        data.Data.Position = data.CipherIndex + 1;
                        HandleGbt(settings, data);
                        data.CipherIndex = data.Data.Size;
                        data.Command = Command.None;
                    }
                }
                else if (settings.IsServer)
                {
                    // Get command if operating as a server.
                    //Ciphered messages are handled after whole PDU is received.
                    switch (cmd)
                    {
                        case Command.GloReadRequest:
                        case Command.GloWriteRequest:
                        case Command.GloGetRequest:
                        case Command.GloSetRequest:
                        case Command.GloMethodRequest:
                        case Command.GeneralSigning:
                            data.Command = Command.None;
                            data.Data.Position = data.CipherIndex;
                            GetPdu(settings, data);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    // Client do not need a command any more.
                    if (data.IsMoreData)
                    {
                        data.Command = Command.None;
                    }
                    //Ciphered messages are handled after whole PDU is received.
                    switch (cmd)
                    {
                        case Command.GloReadResponse:
                        case Command.GloWriteResponse:
                        case Command.GloGetResponse:
                        case Command.GloSetResponse:
                        case Command.GloMethodResponse:
                        case Command.DedReadResponse:
                        case Command.DedWriteResponse:
                        case Command.DedGetResponse:
                        case Command.DedSetResponse:
                        case Command.DedMethodResponse:
                        case Command.GeneralGloCiphering:
                        case Command.GeneralDedCiphering:
                        case Command.GeneralCiphering:
                        case Command.AccessResponse:
                        case Command.GeneralSigning:
                            data.Command = Command.None;
                            data.Data.Position = data.CipherIndex;
                            GetPdu(settings, data);
                            break;
                        default:
                            break;
                    }
                    if (cmd == Command.ReadResponse && data.TotalCount > 1)
                    {
                        if (!HandleReadResponse(settings, data, 0))
                        {
                            return;
                        }
                    }
                }
            }
            // Get data only blocks if SN is used. This is faster.
            if (cmd == Command.ReadResponse
                    && data.CommandType == (byte)SingleReadResponse.DataBlockResult
                    && (data.MoreData & RequestTypes.Frame) != 0)
            {
                return;
            }
            // Get data if all data is read or we want to peek data.
            if (data.Error == 0 && data.Xml == null && data.Data.Position != data.Data.Size &&
                    (cmd == Command.ReadResponse || cmd == Command.GetResponse || cmd == Command.MethodResponse)
                    && (data.MoreData == RequestTypes.None || data.Peek))
            {
                GetValueFromData(settings, data);
            }
        }

        private static void HandleAccessResponse(GXDLMSSettings settings, GXReplyData reply)
        {
            int start = reply.Data.Position - 1;
            //Get invoke id.
            UInt32 invokeId = reply.Data.GetUInt32();

            reply.Time = DateTime.MinValue;
            int len = reply.Data.GetUInt8();
            byte[] tmp = null;
            // If date time is given.
            if (len != 0)
            {
                tmp = new byte[len];
                reply.Data.Get(tmp);
                reply.Time = (GXDateTime)GXDLMSClient.ChangeType(tmp, DataType.DateTime, settings.UseUtc2NormalTime);
            }
            if (reply.Xml != null)
            {
                reply.Xml.AppendStartTag(Command.AccessResponse);
                reply.Xml.AppendLine(TranslatorTags.LongInvokeId, "Value", reply.Xml.IntegerToHex(invokeId, 8));
                if (reply.Time != null)
                {
                    reply.Xml.AppendComment(Convert.ToString(reply.Time));
                }
                reply.Xml.AppendLine(TranslatorTags.DateTime, "Value", GXCommon.ToHex(tmp, false));
                //access-request-specification OPTIONAL
                reply.Data.GetUInt8();
                len = GXCommon.GetObjectCount(reply.Data);
                reply.Xml.AppendStartTag(TranslatorTags.AccessResponseBody);
                reply.Xml.AppendStartTag(
                    TranslatorTags.AccessResponseListOfData, "Qty",
                    reply.Xml.IntegerToHex(len, 2));
                for (int pos = 0; pos != len; ++pos)
                {
                    if (reply.Xml.OutputType == TranslatorOutputType.StandardXml)
                    {
                        reply.Xml.AppendStartTag(Command.WriteRequest,
                                                 SingleReadResponse.Data);
                    }
                    GXDataInfo di = new GXDataInfo();
                    di.xml = reply.Xml;
                    GXCommon.GetData(settings, reply.Data, di);
                    if (reply.Xml.OutputType == TranslatorOutputType.StandardXml)
                    {
                        reply.Xml.AppendEndTag(Command.WriteRequest,
                                               SingleReadResponse.Data);
                    }
                }
                reply.Xml.AppendEndTag(TranslatorTags.AccessResponseListOfData);
                //access-response-specification
                int err;
                len = GXCommon.GetObjectCount(reply.Data);
                reply.Xml.AppendStartTag(TranslatorTags.ListOfAccessResponseSpecification, "Qty", reply.Xml.IntegerToHex(len, 2));
                for (int pos = 0; pos != len; ++pos)
                {
                    AccessServiceCommandType type = (AccessServiceCommandType)reply.Data.GetUInt8();
                    err = reply.Data.GetUInt8();
                    if (err != 0)
                    {
                        err = reply.Data.GetUInt8();
                    }
                    reply.Xml.AppendStartTag(TranslatorTags.AccessResponseSpecification);
                    reply.Xml.AppendStartTag(Command.AccessResponse, type);
                    reply.Xml.AppendLine(TranslatorTags.Result, null,
                                         GXDLMSTranslator.ErrorCodeToString(reply.Xml.OutputType,
                                                 (ErrorCode)err));
                    reply.Xml.AppendEndTag(Command.AccessResponse, type);
                    reply.Xml.AppendEndTag(
                        TranslatorTags.AccessResponseSpecification);
                }
                reply.Xml.AppendEndTag(
                    TranslatorTags.ListOfAccessResponseSpecification);
                reply.Xml.AppendEndTag(TranslatorTags.AccessResponseBody);
                reply.Xml.AppendEndTag(Command.AccessResponse);
            }
            else
            {
                //Skip access-request-specification
                reply.Data.GetUInt8();
            }
        }

        static void HandleDataNotification(GXDLMSSettings settings, GXReplyData reply)
        {
            int start = reply.Data.Position - 1;
            //Get invoke id.
            UInt32 invokeId = reply.Data.GetUInt32();
            reply.Time = DateTime.MinValue;
            int len = reply.Data.GetUInt8();
            byte[] tmp = null;
            // If date time is given.
            if (len != 0)
            {
                tmp = new byte[len];
                reply.Data.Get(tmp);
                reply.Time = (GXDateTime)GXDLMSClient.ChangeType(tmp, DataType.DateTime, settings.UseUtc2NormalTime);
            }
            if (reply.Xml != null)
            {
                reply.Xml.AppendStartTag(Command.DataNotification);
                if ((invokeId & 0x80000000) != 0)
                {
                    reply.Xml.AppendComment("High priority.");
                }
                if ((invokeId & 0x40000000) != 0)
                {
                    reply.Xml.AppendComment("Confirmed service.");
                }
                reply.Xml.AppendComment("Invoke ID: " + (invokeId & 0x3FFFFFFF));

                reply.Xml.AppendLine(TranslatorTags.LongInvokeId, null,
                                     reply.Xml.IntegerToHex(invokeId, 8));
                if (reply.Time != null)
                {
                    reply.Xml.AppendComment(Convert.ToString(reply.Time));
                }
                reply.Xml.AppendLine(TranslatorTags.DateTime, null,
                                     GXCommon.ToHex(tmp, false));
                reply.Xml.AppendStartTag(TranslatorTags.NotificationBody);
                reply.Xml.AppendStartTag(TranslatorTags.DataValue);
                GXDataInfo di = new GXDataInfo();
                di.xml = reply.Xml;
                GXCommon.GetData(settings, reply.Data, di);
                reply.Xml.AppendEndTag(TranslatorTags.DataValue);
                reply.Xml.AppendEndTag(TranslatorTags.NotificationBody);
                reply.Xml.AppendEndTag(Command.DataNotification);
            }
            else
            {
                GetDataFromBlock(reply.Data, start);
                GetValueFromData(settings, reply);
            }
        }

        private static void HandleGeneralCiphering(
            GXDLMSSettings settings,
            GXReplyData data)
        {
            if (settings.Cipher == null)
            {
                throw new Exception(
                   "Secure connection is not supported.");
            }
            // If all frames are read.
            if ((data.MoreData & RequestTypes.Frame) == 0)
            {
                int origPos = 0;
                if (data.Xml != null)
                {
                    origPos = data.Xml.GetXmlLength();
                }
                --data.Data.Position;
                AesGcmParameter p = new AesGcmParameter(settings,
                    settings.SourceSystemTitle,
                    GetBlockCipherKey(settings),
                    GetAuthenticationKey(settings));
                p.Xml = data.Xml;
                try
                {
                    byte[] tmp = GXCiphering.Decrypt(p, data.Data);
                    data.Data.Clear();
                    data.Data.Set(tmp);
                    data.CipheredCommand = Command.GeneralCiphering;
                    data.Command = Command.None;
                    if (p.Security != (byte)Security.None)
                    {
                        if (data.Xml != null && p != null && p.Xml.Comments)
                        {
                            data.Xml.AppendStartTag(Command.GeneralCiphering);
                            data.Xml.AppendLine(TranslatorTags.TransactionId, null,
                                    data.Xml.IntegerToHex(p.TransactionId, 16, true));
                            data.Xml.AppendLine(TranslatorTags.OriginatorSystemTitle,
                                    null, GXCommon.ToHex(p.SystemTitle, false));
                            data.Xml.AppendLine(TranslatorTags.RecipientSystemTitle,
                                    null,
                                    GXCommon.ToHex(p.RecipientSystemTitle, false));
                            data.Xml.AppendLine(TranslatorTags.DateTime, null,
                                    GXCommon.ToHex(p.DateTime, false));
                            data.Xml.AppendLine(TranslatorTags.OtherInformation, null,
                                    GXCommon.ToHex(p.OtherInformation, false));

                            data.Xml.AppendStartTag(TranslatorTags.KeyInfo);
                            data.Xml.AppendStartTag(TranslatorTags.AgreedKey);
                            data.Xml.AppendLine(TranslatorTags.KeyParameters, null,
                                    data.Xml.IntegerToHex(p.KeyParameters, 2,
                                            true));
                            data.Xml.AppendLine(TranslatorTags.KeyCipheredData, null,
                                    GXCommon.ToHex(p.KeyCipheredData, false));
                            data.Xml.AppendEndTag(TranslatorTags.AgreedKey);
                            data.Xml.AppendEndTag(TranslatorTags.KeyInfo);
                            p.Xml.StartComment("");
                        }
                        GetPdu(settings, data);
                        if (data.Xml != null && p != null && p.Xml.Comments)
                        {
                            p.Xml.EndComment();
                            data.Xml.AppendLine(TranslatorTags.CipheredContent, null,
                            GXCommon.ToHex(p.CipheredContent, false));
                            data.Xml.AppendEndTag(Command.GeneralCiphering);
                        }
                    }
                }
                catch (Exception)
                {
                    if (data.Xml == null)
                    {
                        throw;
                    }
                    data.Xml.SetXmlLength(origPos);
                    if (data.Xml != null && p != null && p.Xml.Comments)
                    {
                        data.Xml.AppendStartTag(Command.GeneralCiphering);
                        data.Xml.AppendLine(TranslatorTags.TransactionId, null,
                                data.Xml.IntegerToHex(p.TransactionId, 16, true));
                        data.Xml.AppendLine(TranslatorTags.OriginatorSystemTitle,
                                null, GXCommon.ToHex(p.SystemTitle, false));
                        data.Xml.AppendLine(TranslatorTags.RecipientSystemTitle,
                                null,
                                GXCommon.ToHex(p.RecipientSystemTitle, false));
                        data.Xml.AppendLine(TranslatorTags.DateTime, null,
                                GXCommon.ToHex(p.DateTime, false));
                        data.Xml.AppendLine(TranslatorTags.OtherInformation, null,
                                GXCommon.ToHex(p.OtherInformation, false));

                        data.Xml.AppendStartTag(TranslatorTags.KeyInfo);
                        data.Xml.AppendStartTag(TranslatorTags.AgreedKey);
                        data.Xml.AppendLine(TranslatorTags.KeyParameters, null,
                                data.Xml.IntegerToHex(p.KeyParameters, 2,
                                        true));
                        data.Xml.AppendLine(TranslatorTags.KeyCipheredData, null,
                                GXCommon.ToHex(p.KeyCipheredData, false));
                        data.Xml.AppendEndTag(TranslatorTags.AgreedKey);
                        data.Xml.AppendEndTag(TranslatorTags.KeyInfo);

                        data.Xml.AppendLine(TranslatorTags.CipheredContent, null,
                                GXCommon.ToHex(p.CipheredContent, false));
                        data.Xml.AppendEndTag(Command.GeneralCiphering);
                    }
                }
            }
        }

        /// <summary>
        /// Get value from the data.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="reply"> Received data.</param>
        internal static void GetValueFromData(GXDLMSSettings settings, GXReplyData reply)
        {
            GXByteBuffer data = reply.Data;
            GXDataInfo info = new GXDataInfo();
            if (reply.Value is List<object>)
            {
                info.Type = DataType.Array;
                info.Count = reply.TotalCount;
                info.Index = reply.Count;
            }
            int index = data.Position;
            data.Position = reply.ReadPosition;
            try
            {
                Object value = GXCommon.GetData(settings, data, info);
                if (value != null)
                {
                    lock (reply)
                    {
                        // If new data.
                        if (!(value is List<object>))
                        {
                            reply.DataType = info.Type;
                            reply.Value = value;
                            reply.TotalCount = 0;
                            reply.ReadPosition = data.Position;
                        }
                        else
                        {
                            if (((List<object>)value).Count != 0)
                            {
                                if (reply.Value == null)
                                {
                                    reply.Value = value;
                                }
                                else
                                {
                                    // Add items to collection.
                                    ((List<object>)reply.Value).AddRange((List<object>)value);
                                }
                            }
                            reply.ReadPosition = data.Position;
                            // Element count.
                            reply.TotalCount = info.Count;
                        }
                    }
                }
                else if (info.Complete
                         && reply.Command == Command.DataNotification)
                {
                    // If last item is null. This is a special case.
                    reply.ReadPosition = data.Position;
                }
            }
            finally
            {
                data.Position = index;
            }

            // If last data frame of the data block is read.
            if (reply.Command != Command.DataNotification
                    && info.Complete && reply.MoreData == RequestTypes.None)
            {
                // If all blocks are read.
                settings.ResetBlockIndex();
                data.Position = 0;
            }
        }

        public static bool GetData(
            GXDLMSSettings settings,
            GXByteBuffer reply,
            GXReplyData data,
            GXReplyData notify)
        {
            byte frame = 0;
            bool isLast = true;
            bool isNotify = false;
            switch (settings.InterfaceType)
            {
                // If DLMS frame is generated.
                case InterfaceType.HDLC:
                case InterfaceType.HdlcWithModeE:
                    {
                        frame = GetHdlcData(settings.IsServer, settings, reply, data, notify);
                        isLast = (frame & 0x10) != 0;
                        if (notify != null && (frame == 0x13 || frame == 0x3))
                        {
                            data = notify;
                            isNotify = true;
                        }
                        data.FrameId = frame;
                    }
                    break;
                case InterfaceType.WRAPPER:
                    if (!GetTcpData(settings, reply, data, notify))
                    {
                        if (notify != null)
                        {
                            data = notify;
                        }
                        isNotify = true;
                    }
                    break;
                case InterfaceType.WirelessMBus:
                    GetWirelessMBusData(settings, reply, data);
                    break;
                case InterfaceType.PDU:
                    data.PacketLength = reply.Size;
                    data.IsComplete = reply.Size != 0;
                    break;
                case InterfaceType.Plc:
                    GetPlcData(settings, reply, data);
                    break;
                case InterfaceType.PlcHdlc:
                    frame = GetPlcHdlcData(settings, reply, data);
                    break;
                case InterfaceType.WiredMBus:
                    GetWiredMBusData(settings, reply, data);
                    break;
                default:
                    throw new ArgumentException("Invalid Interface type.");
            }
            // If all data is not read yet.
            if (!data.IsComplete)
            {
                return false;
            }
            if (settings.InterfaceType != InterfaceType.PlcHdlc)
            {
                GetDataFromFrame(reply, data, UseHdlc(settings.InterfaceType));
            }
            // If keepalive or get next frame request.
            if (data.Xml != null || (((frame != 0x13 && frame != 0x3) || data.IsMoreData) && (frame & 0x1) != 0))
            {
                if ((settings.InterfaceType == InterfaceType.HDLC || settings.InterfaceType == InterfaceType.HdlcWithModeE) &&
                    (data.Error == (int)ErrorCode.Rejected || data.Data.Size != 0))
                {
                    System.Diagnostics.Debug.Assert(reply.GetUInt8(reply.Position - 1) == 0x7e);
                }
                if (frame == 0x3 && data.IsMoreData)
                {
                    bool tmp = GetData(settings, reply, data, notify);
                    data.Data.Position = 0;
                    return tmp;
                }
                return true;
            }
            if (data.RawPdu)
            {
                data.Data.Position = 0;
                return !isNotify;
            }
            if (frame == 0x13 && !data.IsMoreData)
            {
                data.Data.Position = 0;
            }
            try
            {
                GetPdu(settings, data);
            }
            catch (Exception)
            {
                //Ignore received data if meter is sending invalid push message while data is read from the meter.
                if (!isNotify)
                {
                    throw;
                }
                isLast = false;
                data.Command = Command.None;
            }
            if (notify != null && !isNotify)
            {
                //Check command to make sure it's not notify message.
                switch (data.Command)
                {
                    case Command.DataNotification:
                    case Command.GloEventNotification:
                    case Command.InformationReport:
                    case Command.EventNotification:
                    case Command.DedInformationReport:
                    case Command.DedEventNotification:
                        isNotify = true;
                        notify.IsComplete = data.IsComplete;
                        notify.MoreData = data.MoreData;
                        notify.Command = data.Command;
                        data.Command = Command.None;
                        notify.Time = data.Time;
                        data.Time = DateTime.MinValue;
                        notify.Data.Set(data.Data);
                        data.Data.Trim();
                        notify.Value = data.Value;
                        data.Value = null;
                        break;
                    default:
                        break;
                }
            }
            if (!isLast || (data.MoreData == RequestTypes.GBT && reply.Available != 0))
            {
                //Clear received notify message.
                if (data.Command == Command.DataNotification && data.MoreData == RequestTypes.None)
                {
                    return !isNotify;
                }
                return GetData(settings, reply, data, notify);
            }
            return !isNotify;
        }

        /// <summary>
        /// Get data from HDLC or wrapper frame.
        /// </summary>
        /// <param name="reply">Received data that includes HDLC frame.</param>
        /// <param name="info">Reply data.</param>
        private static void GetDataFromFrame(GXByteBuffer reply, GXReplyData info, bool hdlc)
        {
            int offset = info.Data.Size;
            int cnt = info.PacketLength - reply.Position;
            if (cnt != 0)
            {
                info.Data.Capacity = (offset + cnt);
                info.Data.Set(reply.Data, reply.Position, cnt);
                reply.Position = (reply.Position + cnt);
                if (hdlc)
                {
                    reply.Position += 3;
                }
            }
            // Set position to begin of new data.
            info.Data.Position = offset;
        }

        /// <summary>
        /// Get data from Block.
        /// </summary>
        /// <param name="data">Stored data block.</param>
        /// <param name="index">Position where data starts.</param>
        /// <returns>Amount of removed bytes.</returns>
        private static int GetDataFromBlock(GXByteBuffer data,
                                            int index)
        {
            if (data.Size == data.Position)
            {
                data.Clear();
                return 0;
            }
            int len = data.Position - index;
            System.Buffer.BlockCopy(data.Data, data.Position, data.Data,
                                    data.Position - len, data.Size - data.Position);
            data.Position = (data.Position - len);
            data.Size = (data.Size - len);
            return len;
        }

        /// <summary>
        /// Returns action method information.
        /// </summary>
        /// <param name="objectType">object type.</param>
        /// <param name="value">Starting address of action methods.</param>
        /// <param name="count">Count of action methods</param>
        static internal void GetActionInfo(ObjectType objectType, out int value, out int count)
        {
            count = value = 0;
            switch (objectType)
            {
                case ObjectType.ImageTransfer:
                    value = 0x40;
                    count = 4;
                    break;
                case ObjectType.ActivityCalendar:
                    value = 0x50;
                    count = 1;
                    break;
                case ObjectType.AssociationLogicalName:
                    value = 0x60;
                    count = 4;
                    break;
                case ObjectType.AssociationShortName:
                    value = 0x20;
                    count = 8;
                    break;
                case ObjectType.Clock:
                    value = 0x60;
                    count = 6;
                    break;
                case ObjectType.DemandRegister:
                    value = 0x48;
                    count = 2;
                    break;
                case ObjectType.ExtendedRegister:
                    value = 0x38;
                    count = 1;
                    break;
                case ObjectType.Ip4Setup:
                    value = 0x60;
                    count = 3;
                    break;
                case ObjectType.MBusSlavePortSetup:
                    value = 0x60;
                    count = 8;
                    break;
                case ObjectType.ProfileGeneric:
                    value = 0x58;
                    count = 4;
                    break;
                case ObjectType.Register:
                    value = 0x28;
                    count = 1;
                    break;
                case ObjectType.RegisterActivation:
                    value = 0x30;
                    count = 3;
                    break;
                case ObjectType.RegisterTable:
                    value = 0x28;
                    count = 2;
                    break;
                case ObjectType.ScriptTable:
                    value = 0x20;
                    count = 1;
                    break;
                case ObjectType.SpecialDaysTable:
                    value = 0x10;
                    count = 2;
                    break;
                case ObjectType.DisconnectControl:
                    value = 0x20;
                    count = 2;
                    break;
                case ObjectType.SecuritySetup:
                    value = 0x30;
                    count = 8;
                    break;
                case ObjectType.PushSetup:
                    value = 0x38;
                    count = 1;
                    break;
                default:
                    value = 0;
                    count = 0;
                    break;
            }
        }

        internal static int GetAttributeSize(GXDLMSObject obj, int attributeIndex)
        {
            DataType dt, udt;
            int rowsize = 0;
            if (attributeIndex == 0)
            {
                for (int pos = 1; pos < (obj as IGXDLMSBase).GetAttributeCount(); ++pos)
                {
                    rowsize += GetAttributeSize(obj, pos);
                }
            }
            else
            {
                dt = obj.GetDataType(attributeIndex);
                if (dt == DataType.OctetString)
                {
                    udt = obj.GetUIDataType(attributeIndex);
                    if (udt == DataType.DateTime ||
                        udt == DataType.Date ||
                        udt == DataType.Time)
                    {
                        rowsize = GXCommon.GetDataTypeSize(udt);
                    }
                }
                else if (dt == DataType.None)
                {
                    rowsize = 2;
                }
                else
                {
                    rowsize = GXCommon.GetDataTypeSize(dt);
                }
            }
            return rowsize;
        }

        internal static UInt16 RowsToPdu(GXDLMSSettings settings, GXDLMSProfileGeneric pg)
        {
            //Count how many rows we can fit to one PDU.
            int rowsize = 0;
            foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in pg.CaptureObjects)
            {
                rowsize += GetAttributeSize(it.Key, it.Value.AttributeIndex);
            }
            if (rowsize != 0)
            {
                return (UInt16)(settings.MaxPduSize / rowsize);
            }
            return 0;
        }

        /// <summary>
        /// Parses SNRM or UA Response from byte array and update settings.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="data">Received data</param>
        internal static void ParseSnrmUaResponse(GXByteBuffer data, GXDLMSSettings settings)
        {
            //If default settings are used.
            if (data.Size == 0)
            {
                settings.Hdlc.MaxInfoRX = GXDLMSLimitsDefault.DefaultMaxInfoRX;
                settings.Hdlc.MaxInfoTX = GXDLMSLimitsDefault.DefaultMaxInfoTX;
                settings.Hdlc.WindowSizeRX = GXDLMSLimitsDefault.DefaultWindowSizeRX;
                settings.Hdlc.WindowSizeTX = GXDLMSLimitsDefault.DefaultWindowSizeTX;
                return;
            }
            data.GetUInt8(); // Skip FromatID
            data.GetUInt8(); // Skip Group ID.
            data.GetUInt8(); // Skip Group len
            Object val;
            while (data.Position < data.Size)
            {
                HDLCInfo id = (HDLCInfo)data.GetUInt8();
                short len = data.GetUInt8();
                switch (len)
                {
                    case 1:
                        val = data.GetUInt8();
                        break;
                    case 2:
                        val = data.GetUInt16();
                        break;
                    case 4:
                        val = data.GetUInt32();
                        break;
                    default:
                        throw new GXDLMSException("Invalid Exception.");
                }
                // RX / TX are delivered from the partner's point of view =>
                // reversed to ours
                switch (id)
                {
                    case HDLCInfo.MaxInfoTX:
                        settings.Hdlc.MaxInfoRX = Convert.ToUInt16(val);
                        break;
                    case HDLCInfo.MaxInfoRX:
                        settings.Hdlc.MaxInfoTX = Convert.ToUInt16(val);
                        if (settings.Hdlc.UseFrameSize)
                        {
                            byte[] secondaryAddress;
                            secondaryAddress = GXDLMS.GetHdlcAddressBytes(settings.ClientAddress, 0);
                            settings.Hdlc.MaxInfoTX += (UInt16)(10 + secondaryAddress.Length);
                        }
                        break;
                    case HDLCInfo.WindowSizeTX:
                        settings.Hdlc.WindowSizeRX = Convert.ToByte(val);
                        break;
                    case HDLCInfo.WindowSizeRX:
                        settings.Hdlc.WindowSizeTX = Convert.ToByte(val);
                        break;
                    default:
                        throw new GXDLMSException("Invalid UA response.");
                }
            }
        }

        /// <summary>
        /// Add HDLC parameter.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        internal static void AppendHdlcParameter(GXByteBuffer data, UInt16 value)
        {
            if (value < 0x100)
            {
                data.SetUInt8(1);
                data.SetUInt8((byte)value);
            }
            else
            {
                data.SetUInt8(2);
                data.SetUInt16(value);
            }
        }

        internal static bool IsCiphered(byte cmd)
        {
            switch ((Command)cmd)
            {
                case Command.GloReadRequest:
                case Command.GloWriteRequest:
                case Command.GloGetRequest:
                case Command.GloSetRequest:
                case Command.GloReadResponse:
                case Command.GloWriteResponse:
                case Command.GloGetResponse:
                case Command.GloSetResponse:
                case Command.GloMethodRequest:
                case Command.GloMethodResponse:
                case Command.DedGetRequest:
                case Command.DedSetRequest:
                case Command.DedReadResponse:
                case Command.DedGetResponse:
                case Command.DedSetResponse:
                case Command.DedMethodRequest:
                case Command.DedMethodResponse:
                case Command.GeneralGloCiphering:
                case Command.GeneralDedCiphering:
                case Command.Aare:
                case Command.Aarq:
                case Command.GloConfirmedServiceError:
                case Command.DedConfirmedServiceError:
                case Command.GeneralCiphering:
                case Command.ReleaseRequest:
                case Command.GeneralSigning:
                    return true;
                default:
                    return false;
            }
        }

    }
}