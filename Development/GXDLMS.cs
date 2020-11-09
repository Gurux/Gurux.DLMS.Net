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

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMS implements methods to communicate with DLMS/COSEM metering devices.
    /// </summary>
    sealed class GXDLMS
    {
        internal static bool IsHdlc(InterfaceType type)
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
                availableObjectTypes.Add(ObjectType.Arbitrator, typeof(GXDLMSArbitrator));
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
        public static ObjectType[] GetObjectTypes2(Dictionary<ObjectType, Type> availableObjectTypes)
        {
            lock (availableObjectTypes)
            {
                GetCosemObjects(availableObjectTypes);
                return availableObjectTypes.Keys.ToArray();
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

        internal static GXDLMSObject CreateObject(ObjectType type, Dictionary<ObjectType, Type> availableObjectTypes)
        {
            lock (availableObjectTypes)
            {
                //Update objects.
                if (availableObjectTypes.Count == 0)
                {
                    GetObjectTypes(availableObjectTypes);
                }
                if (availableObjectTypes.ContainsKey(type))
                {
                    return Activator.CreateInstance(availableObjectTypes[type]) as GXDLMSObject;
                }
            }
            GXDLMSObject obj = new GXDLMSObject();
            obj.ObjectType = type;
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
        internal static byte[] ReceiverReady(GXDLMSSettings settings, RequestTypes type)
        {
            GXReplyData reply = new GXReplyData() { MoreData = type };
            reply.WindowSize = settings.WindowSize;
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
            Command cmd;
            if (settings.UseLogicalNameReferencing)
            {
                if (settings.IsServer)
                {
                    cmd = Command.GetResponse;
                }
                else
                {
                    cmd = Command.GetRequest;
                }
            }
            else
            {
                if (settings.IsServer)
                {
                    cmd = Command.ReadResponse;
                }
                else
                {
                    cmd = Command.ReadRequest;
                }
            }
            // Get next block.
            byte[][] data;
            if (reply.MoreData == RequestTypes.GBT)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(null, settings, 0, Command.GeneralBlockTransfer, 0, null, null, 0xff, Command.None);
                p.WindowSize = reply.WindowSize;
                p.blockNumberAck = reply.BlockNumberAck;
                p.blockIndex = reply.BlockNumber;
                p.Streaming = false;
                data = GXDLMS.GetLnMessages(p);
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
                    GXDLMSLNParameters p = new GXDLMSLNParameters(null, settings, 0, cmd, (byte)GetCommandType.NextDataBlock, bb, null, 0xff, Command.None);
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
        private static void AddLLCBytes(GXDLMSSettings settings, GXByteBuffer data)
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

        internal static byte[] Cipher0(GXDLMSLNParameters p, byte[] data)
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
                        key = cipher.BlockCipherKey;
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
                        key = cipher.BlockCipherKey;
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
                    key = cipher.BlockCipherKey;
                }
                else if (IsGloMessage(p.cipheredCommand))
                {
                    cmd = (byte)GetGloMessage(p.command);
                    key = cipher.BlockCipherKey;
                }
                else
                {
                    cmd = (byte)GetDedMessage(p.command);
                    key = cipher.DedicatedKey;
                }
            }
            AesGcmParameter s = new AesGcmParameter(cmd, cipher.Security,
                cipher.InvocationCounter, cipher.SystemTitle, key,
                cipher.AuthenticationKey);
            s.IgnoreSystemTitle = p.settings.Standard == Standard.Italy;
            byte[] tmp = GXCiphering.Encrypt(s, data);
            ++cipher.InvocationCounter;
            return tmp;
        }

        /// <summary>
        /// Get next logical name PDU.
        /// </summary>
        /// <param name="p">LN parameters.</param>
        /// <param name="reply">Generated message.</param>
        internal static void GetLNPdu(GXDLMSLNParameters p, GXByteBuffer reply)
        {
            bool ciphering = p.command != Command.Aarq && p.command != Command.Aare && p.settings.Cipher != null &&
                (p.settings.Cipher.Security != (byte)Security.None || p.cipheredCommand != Command.None);
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
                else if (p.command != Command.ReleaseRequest)
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
                            if (p.requestType == 1)
                            {
                                p.requestType = 2;
                            }
                            else if (p.requestType == 2)
                            {
                                p.requestType = 3;
                            }
                        }
                    }
                    //Change request type If get response and multiple blocks is needed.
                    if (p.command == Command.GetResponse)
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
                        len -= GXCommon.GetObjectCountSizeInBytes(len);
                    }
                    GXCommon.SetObjectCount(len, reply);
                    reply.Set(p.data, len);
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
                            reply.SetUInt8(Command.GatewayRequest);
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
                                reply.Set(p.data);
                                byte[] tmp = Cipher0(p, reply.Array());
                                p.data.Size = 0;
                                p.data.Set(tmp);
                                reply.Size = 0;
                                len = p.data.Size;
                                if (7 + len > p.settings.MaxPduSize)
                                {
                                    len = p.settings.MaxPduSize - 7;
                                }
                                ciphering = false;
                            }
                        }
                        else if (p.command != Command.GetRequest && len + reply.Size > p.settings.MaxPduSize)
                        {
                            len = p.settings.MaxPduSize - reply.Size;
                        }
                        reply.Set(p.data, len);
                    }
                    else if ((p.settings.Gateway != null && p.settings.Gateway.PhysicalDeviceAddress != null) &&
                        !(p.command == Command.GeneralBlockTransfer || (p.multipleBlocks && (p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) != 0)))
                    {
                        if (3 + len + p.settings.Gateway.PhysicalDeviceAddress.Length > p.settings.MaxPduSize)
                        {
                            len -= (3 + p.settings.Gateway.PhysicalDeviceAddress.Length);
                        }
                        GXByteBuffer tmp = new GXByteBuffer(reply);
                        reply.Size = 0;
                        reply.SetUInt8(Command.GatewayRequest);
                        reply.SetUInt8(p.settings.Gateway.NetworkId);
                        reply.SetUInt8((byte)p.settings.Gateway.PhysicalDeviceAddress.Length);
                        reply.Set(p.settings.Gateway.PhysicalDeviceAddress);
                        reply.Set(tmp);
                    }
                }
                if (reply.Size != 0 && p.command != Command.GeneralBlockTransfer && p.Owner != null && p.Owner.pdu != null)
                {
                    p.Owner.pdu(p.Owner, reply.Array());
                }
                if (ciphering && reply.Size != 0 && p.command != Command.ReleaseRequest && (!p.multipleBlocks || (p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) == 0))
                {
                    //GBT ciphering is done for all the data, not just block.
                    byte[] tmp = Cipher0(p, reply.Array());
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
                    else if (p.Streaming)
                    {
                        value |= 0x40;
                    }
                    value |= p.WindowSize;
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
                        if (3 + len + p.settings.Gateway.PhysicalDeviceAddress.Length > p.settings.MaxPduSize)
                        {
                            len -= (3 + p.settings.Gateway.PhysicalDeviceAddress.Length);
                        }
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
            if (IsHdlc(p.settings.InterfaceType))
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
                        messages.Add(GXDLMS.GetPlcFrame(p.settings, 0x90, reply));
                        break;
                    }
                    else if (p.settings.InterfaceType == Enums.InterfaceType.PlcHdlc)
                    {
                        messages.Add(GXDLMS.GetMacHdlcFrame(p.settings, frame, 0, reply));
                        break;
                    }
                    else
                    {
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
                frame = 0x13;
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
            bool ciphering = p.settings.Cipher != null && p.settings.Cipher.Security != (byte)Security.None;
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
                if (IsHdlc(p.settings.InterfaceType))
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
            bool ciphering = p.command != Command.Aarq && p.command != Command.Aare && p.settings.Cipher != null && p.settings.Cipher.Security != (byte)Security.None;
            if (!ciphering && IsHdlc(p.settings.InterfaceType))
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
                        if (!ciphering && IsHdlc(p.settings.InterfaceType))
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
                    GetGloMessage(p.command), cipher.Security,
                    cipher.InvocationCounter, cipher.SystemTitle,
                    cipher.BlockCipherKey, cipher.AuthenticationKey);
                ++cipher.InvocationCounter;
                byte[] tmp = GXCiphering.Encrypt(s, reply.Array());
                System.Diagnostics.Debug.Assert(!(p.settings.MaxPduSize < tmp.Length));
                reply.Size = 0;
                if (IsHdlc(p.settings.InterfaceType))
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
            GXByteBuffer bb = new GXByteBuffer();
            int frameSize, len;
            byte[] primaryAddress, secondaryAddress;
            if (settings.IsServer)
            {
                if (frame == 0x13 && settings.PushClientAddress != 0)
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
            frameSize = Convert.ToInt32(settings.Limits.MaxInfoTX);
            //Remove BOP, type, len, primaryAddress, secondaryAddress, frame, header CRC, data CRC and EOP from data length.
            if (settings.Limits.UseFrameSize)
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
                len = data.Size - data.Position;
                // Is last packet.
                bb.SetUInt8((byte)(0xA0 | ((7 + primaryAddress.Length + secondaryAddress.Length + len) >> 8) & 0x7));
            }
            else
            {
                len = frameSize;
                // More data to left.
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
            bb.SetUInt8(DataLinkData.Request);
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
            if (settings.Limits.MaxInfoTX > 126)
            {
                settings.Limits.MaxInfoTX = 86;
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
            byte[] tmp = GXDLMS.GetHdlcFrame(settings, frame, data);
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
                if (!(reply.Position < reply.Size && reply.GetUInt8(reply.Position) == 0x13))
                {
                    //If echo.
                    reply.Position = 1 + eopPos;
                    return GetHdlcData(server, settings, reply, data, notify);
                }
                if (notify != null)
                {
                    isNotify = true;
                    notify.ClientAddress = target;
                    notify.ServerAddress = source;
                }
            }
            // Is there more data available.
            bool moreData = (frame & 0x8) != 0;
            // Get frame type.
            frame = reply.GetUInt8();
            //If server is using same client and server address for notifications.
            if (frame == 0x13 && !isNotify && notify != null)
            {
                isNotify = true;
                notify.ClientAddress = target;
                notify.ServerAddress = source;
            }
            if (moreData)
            {
                if (isNotify)
                {
                    notify.MoreData = (RequestTypes)(notify.MoreData | RequestTypes.Frame);
                }
                else
                {
                    data.MoreData = (RequestTypes)(data.MoreData | RequestTypes.Frame);
                }
            }
            else
            {
                if (isNotify)
                {
                    notify.MoreData = (RequestTypes)(notify.MoreData & ~RequestTypes.Frame);
                }
                else
                {
                    data.MoreData = (RequestTypes)(data.MoreData & ~RequestTypes.Frame);
                }
            }
            if (data.Xml == null && !settings.CheckFrame(frame))
            {
                reply.Position = (eopPos + 1);
                return GetHdlcData(server, settings, reply, data, notify);
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
            if (frame != 0x13 && (frame & (byte)HdlcFrameType.Uframe) == (byte)HdlcFrameType.Uframe)
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
                //If client want to know used server and client address.
                if (data.ClientAddress == 0 && data.ServerAddress == 0)
                {
                    data.ClientAddress = target;
                    data.ServerAddress = source;
                }
            }
            //If S-frame
            else if (frame != 0x13 && (frame & (byte)HdlcFrameType.Sframe) == (byte)HdlcFrameType.Sframe)
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
                    if (!GetLLCBytes(server, reply) && data.Xml != null)
                    {
                        GetLLCBytes(!server, reply);
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
        /// Get data from Wireless M-Bus frame.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="buff">Received data.</param>
        /// <param name="data">Reply information.</param>
        static void GetMBusData(GXDLMSSettings settings,
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
                MBusCommand cmd = (MBusCommand)buff.GetUInt8();
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
            }
        }

        public enum SubLayer
        {
            /// <summary>
            /// S-FSK PLC profile is using the IEC61334-4-32:1996 LLC sublayer.
            /// </summary>
            Llc,
            /// <summary>
            ///  S-FSK PLC profile is using the HDLC based LLC sublayer.
            /// </summary>
            Hdlc
        };

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
                int mac = buff.GetUInt16() << 8 | buff.GetUInt8();
                //SA.
                short macSa = (short)(mac >> 12);
                //DA.
                short macDa = (short)(mac & 0xFFF);
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
                    if (ch != (int)DataLinkData.Request)
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
                        data.ServerAddress = macDa;
                        data.ClientAddress = macSa;
                    }
                    else
                    {
                        // FFF (All)
                        data.IsComplete = data.Xml != null ||
                            (macDa == (UInt16)PlcDestinationAddress.AllPhysical || macDa == settings.Plc.MacDestinationAddress);
                        data.ClientAddress = macSa;
                        data.ServerAddress = macDa;
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
            int frameLen;
            //SN field.
            UInt16 ns = buff.GetUInt16();
            if (ns == (int)PlcMacSubframes.One && buff.Available < 34)
            {
                data.IsComplete = false;
            }
            else if (ns == (int)PlcMacSubframes.Two && buff.Available < (2 * 36) - 2)
            {
                data.IsComplete = false;
            }
            else if (ns == (int)PlcMacSubframes.Three && buff.Available < (3 * 36) - 2)
            {
                data.IsComplete = false;
            }
            else if (ns == (int)PlcMacSubframes.Four && buff.Available < (4 * 36) - 2)
            {
                data.IsComplete = false;
            }
            else if (ns == (int)PlcMacSubframes.Five && buff.Available < (5 * 36) - 2)
            {
                data.IsComplete = false;
            }
            else if (ns == (int)PlcMacSubframes.Six && buff.Available < (6 * 36) - 2)
            {
                data.IsComplete = false;
            }
            else if (ns == (int)PlcMacSubframes.Seven && buff.Available < (7 * 36) - 2)
            {
                data.IsComplete = false;
            }
            else
            {
                if (ns == (int)PlcMacSubframes.One)
                {
                    frameLen = 36;
                }
                else if (ns == (int)PlcMacSubframes.Two)
                {
                    frameLen = 2 * 36;
                }
                else if (ns == (int)PlcMacSubframes.Three)
                {
                    frameLen = 3 * 36;
                }
                else if (ns == (int)PlcMacSubframes.Four)
                {
                    frameLen = 4 * 36;
                }
                else if (ns == (int)PlcMacSubframes.Five)
                {
                    frameLen = 5 * 36;
                }
                else if (ns == (int)PlcMacSubframes.Six)
                {
                    frameLen = 6 * 36;
                }
                else if (ns == (int)PlcMacSubframes.Seven)
                {
                    frameLen = 7 * 36;
                }
                else
                {
                    throw new Exception("Invalid PLC frame size.");
                }
                int index = buff.Position;
                //Credit fields.  IC, CC, DC
                byte credit = buff.GetUInt8();
                //MAC Addresses.
                int mac = buff.GetUInt16() << 8 | buff.GetUInt8();
                //SA.
                short sa = (short)(mac >> 12);
                //DA.
                short da = (short)(mac & 0xFFF);
                if (settings.IsServer)
                {
                    data.IsComplete = data.Xml != null ||
                        ((da == (UInt16)PlcDestinationAddress.AllPhysical || da == settings.Plc.MacSourceAddress) &&
                    (sa == (UInt16) PlcHdlcSourceAddress.Initiator || sa == settings.Plc.MacDestinationAddress));
                    data.ServerAddress = da;
                    data.ClientAddress = sa;
                }
                else
                {
                    data.IsComplete = data.Xml != null ||
                        (da == (UInt16)PlcHdlcSourceAddress.Initiator || da == settings.Plc.MacDestinationAddress);
                    data.ClientAddress = sa;
                    data.ServerAddress = da;
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
        /// Check is this M-Bus message.
        /// </summary>
        /// <param name="buff">Received data.</param>
        /// <returns>True, if this is M-Bus message.</returns>
        internal static bool IsMBusData(GXByteBuffer buff)
        {
            if (buff.Size - buff.Position < 2)
            {
                return false;
            }
            MBusCommand cmd = (MBusCommand)buff.GetUInt8(buff.Position + 1);
            if (!(cmd == MBusCommand.SndNr ||
                cmd == MBusCommand.SndUd2 ||
                cmd == MBusCommand.RspUd))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check is this PLC S-FSK message.
        /// </summary>
        /// <param name="buff">Received data.</param>
        /// <returns>True, if this is M-Bus message.</returns>
        internal static bool IsPlcSfskData(GXByteBuffer buff)
        {
            if (buff.Size - buff.Position < 2)
            {
                return false;
            }
            UInt16 len = buff.GetUInt16(buff.Position);
            return len == (int)PlcMacSubframes.One ||
                len == (int)PlcMacSubframes.Two ||
                len == (int)PlcMacSubframes.Three;
        }

        private static bool CheckWrapperAddress(GXDLMSSettings settings, GXByteBuffer buff, GXReplyData data, GXReplyData notify)
        {
            bool ret = true;
            int value;
            if (settings.IsServer)
            {
                value = buff.GetUInt16();
                // Check that client addresses match.
                if (data.Xml == null && settings.ClientAddress != 0
                        && settings.ClientAddress != value)
                {
                    throw new Exception("Source addresses do not match. It is "
                        + value.ToString() + ". It should be "
                        + settings.ClientAddress.ToString());
                }
                else
                {
                    settings.ClientAddress = value;
                }

                value = buff.GetUInt16();
                // Check that server addresses match.
                if (data.Xml == null && settings.ServerAddress != 0
                        && settings.ServerAddress != value)
                {
                    throw new Exception("Destination addresses do not match. It is "
                    + value.ToString() + ". It should be "
                    + settings.ServerAddress.ToString()
                    + ".");
                }
                else
                {
                    settings.ServerAddress = value;
                }
            }
            else
            {
                value = buff.GetUInt16();
                // Check that server addresses match.
                if (settings.ServerAddress != 0
                        && settings.ServerAddress != value)
                {
                    if (notify == null)
                    {
                        throw new Exception("Source addresses do not match. It is "
                        + value.ToString() + ". It should be "
                        + settings.ServerAddress.ToString()
                        + ".");
                    }
                    notify.ServerAddress = value;
                    ret = false;
                }
                else
                {
                    settings.ServerAddress = value;
                }

                value = buff.GetUInt16();
                // Check that client addresses match.
                if (settings.ClientAddress != 0
                        && settings.ClientAddress != value)
                {
                    if (notify == null)
                    {
                        throw new Exception("Destination addresses do not match. It is "
                        + value.ToString() + ". It should be "
                        + settings.ClientAddress.ToString() + ".");
                    }
                    ret = false;
                    notify.ClientAddress = value;
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
                        data.Xml.AppendStartTag(Command.ReadResponse,
                                                SingleReadResponse.Data);
                        GXDataInfo di = new GXDataInfo();
                        di.xml = data.Xml;
                        GXCommon.GetData(settings, data.Data, di);
                        data.Xml.AppendEndTag(Command.ReadResponse,
                                              SingleReadResponse.Data);
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
                    sb.Append("ID: " + (invokeId & 0xF).ToString());
                    xml.AppendComment(sb.ToString());
                }
                xml.AppendLine(TranslatorTags.InvokeId, null, xml.IntegerToHex(invokeId, 2));
            }
        }

        /// <summary>
        /// Handle method response and get data from block and/or update error status.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data">Received data from the client.</param>
        static void HandleMethodResponse(GXDLMSSettings settings,
                                         GXReplyData data)
        {
            // Get type.
            ActionResponseType type = (ActionResponseType)data.Data.GetUInt8();
            // Get invoke ID and priority.
            data.InvokeId = data.Data.GetUInt8();
            VerifyInvokeId(settings, data);
            AddInvokeId(data.Xml, Command.MethodResponse, type, data.InvokeId);
            //Action-Response-Normal
            if (type == ActionResponseType.Normal)
            {
                HandleActionResponseNormal(settings, data);
            }
            //Action-Response-With-Pblock
            else if (type == ActionResponseType.WithFirstBlock)
            {
                throw new ArgumentException("Invalid Command.");
            }
            // Action-Response-With-List.
            else if (type == ActionResponseType.WithList)
            {
                throw new ArgumentException("Invalid Command.");
            }
            //Action-Response-Next-Pblock
            else if (type == ActionResponseType.WithBlock)
            {
                throw new ArgumentException("Invalid Command.");
            }
            else
            {
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
            long number;
            short ch = 0;
            GXByteBuffer data = reply.Data;
            bool empty = false;
            // Get type.
            GetCommandType type = (GetCommandType)data.GetUInt8();
            reply.CommandType = (byte)type;
            // Get invoke ID and priority.
            reply.InvokeId = data.GetUInt8();
            VerifyInvokeId(settings, reply);
            AddInvokeId(reply.Xml, Command.GetResponse, type, reply.InvokeId);
            // Response normal
            if (type == GetCommandType.Normal)
            {
                if (data.Available == 0)
                {
                    empty = true;
                    GetDataFromBlock(data, 0);
                }
                else
                {
                    // Result
                    ch = data.GetUInt8();
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
            }
            else if (type == GetCommandType.NextDataBlock)
            {
                // Is Last block.
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
                            if (blockLength > data.Size - data.Position)
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
            }
            else if (type == GetCommandType.WithList)
            {
                HandleGetResponseWithList(settings, reply);
                ret = false;
            }
            else
            {
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

        /// <summary>
        /// Handle General block transfer message.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data"></param>
        internal static void HandleGbt(GXDLMSSettings settings, GXReplyData data)
        {
            int index = data.Data.Position - 1;
            data.WindowSize = settings.WindowSize;
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
                                    + "and there are " + (data.Data.Size - data.Data.Position)
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
                //If last block and comments.
                if ((bc & 0x80) != 0 && data.Xml.Comments && data.Data.Available != 0)
                {
                    int pos = data.Data.Position;
                    int len2 = data.Xml.GetXmlLength();
                    try
                    {
                        GXReplyData reply = new GXReplyData();
                        reply.Data = data.Data;
                        reply.Xml = data.Xml;
                        reply.Xml.StartComment("");
                        GetPdu(settings, reply, null);
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
                    GetPdu(settings, data, null);
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
                                              GXReplyData data, GXDLMSClient client)
        {
            if (data.Xml != null)
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
                    AesGcmParameter p;
                    GXICipher cipher = settings.Cipher;
                    if (data.Command == Command.GeneralDedCiphering)
                    {
                        p = new AesGcmParameter(settings.SourceSystemTitle,
                                cipher.DedicatedKey,
                                cipher.AuthenticationKey);
                    }
                    else if (data.Command == Command.GeneralGloCiphering)
                    {
                        p = new AesGcmParameter(settings.SourceSystemTitle,
                                cipher.BlockCipherKey,
                                cipher.AuthenticationKey);
                    }
                    else if (IsGloMessage(data.Command))
                    {
                        p = new AesGcmParameter(settings.SourceSystemTitle,
                                cipher.BlockCipherKey,
                                cipher.AuthenticationKey);
                    }
                    else
                    {
                        p = new AesGcmParameter(settings.SourceSystemTitle,
                                cipher.DedicatedKey,
                                cipher.AuthenticationKey);
                    }
                    byte[] tmp = GXCiphering.Decrypt(p, data.Data);
                    cipher.SecuritySuite = p.SecuritySuite;
                    cipher.Security = p.Security;
                    if (client != null && client.pdu != null && data.IsComplete && (data.MoreData & RequestTypes.Frame) == 0)
                    {
                        client.pdu(client, tmp);
                    }
                    data.Data.Clear();
                    data.Data.Set(tmp);
                    // Get command.
                    data.CipheredCommand = data.Command;
                    data.Command = (Command)data.Data.GetUInt8();
                    if (data.Command == Command.DataNotification
                        || data.Command == Command.InformationReport)
                    {
                        data.Command = Command.None;
                        --data.Data.Position;
                        GetPdu(settings, data, client);
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
            int index,
            GXDLMSClient client)
        {
            if (data.Xml != null)
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
                    AesGcmParameter p;
                    GXICipher cipher = settings.Cipher;
                    if (cipher.DedicatedKey != null
                            && (settings.Connected & ConnectionState.Dlms) != 0)
                    {
                        p = new AesGcmParameter(settings.SourceSystemTitle,
                                cipher.DedicatedKey,
                                cipher.AuthenticationKey);
                    }
                    else
                    {
                        if (settings.PreEstablishedSystemTitle != null && (settings.Connected & ConnectionState.Dlms) == 0)
                        {
                            p = new AesGcmParameter(settings.PreEstablishedSystemTitle,
                            cipher.BlockCipherKey,
                            cipher.AuthenticationKey);
                        }
                        else
                        {
                            if (settings.SourceSystemTitle == null)
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
                            p = new AesGcmParameter(settings.SourceSystemTitle,
                                    cipher.BlockCipherKey,
                                    cipher.AuthenticationKey);
                        }
                    }
                    byte[] tmp = GXCiphering.Decrypt(p, bb);
                    //If target is sending data ciphered using different security policy.
                    if (settings.Cipher.Security != 0 && settings.Cipher.Security != p.Security)
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
                    if (client != null && client.pdu != null)
                    {
                        client.pdu(client, tmp);
                    }
                    data.Data.Set(tmp);
                    data.CipheredCommand = data.Command;
                    data.Command = Command.None;
                    GetPdu(settings, data, client);
                    data.CipherIndex = data.Data.Size;
                }
            }
        }

        /// <summary>
        /// Get PDU from the packet.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data">received data.</param>
        public static void GetPdu(GXDLMSSettings settings,
                                  GXReplyData data, GXDLMSClient client)
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
                        HandleMethodResponse(settings, data);
                        break;
                    case Command.AccessResponse:
                        HandleAccessResponse(settings, data);
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
                    case Command.ReadRequest:
                    case Command.WriteRequest:
                    case Command.SetRequest:
                    case Command.MethodRequest:
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
                        HandleGloDedRequest(settings, data, client);
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
                        HandleGloDedResponse(settings, data, index, client);
                        break;
                    case Command.GeneralGloCiphering:
                    case Command.GeneralDedCiphering:
                        if (settings.IsServer)
                        {
                            HandleGloDedRequest(settings, data, client);
                        }
                        else
                        {
                            HandleGloDedResponse(settings, data, index, client);
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
                        GetPdu(settings, data, null);
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
                    data.Data.Position = data.CipherIndex + 1;
                    HandleGbt(settings, data);
                    data.CipherIndex = data.Data.Size;
                    data.Command = Command.None;
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
                            data.Command = Command.None;
                            data.Data.Position = data.CipherIndex;
                            GetPdu(settings, data, client);
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
                            data.Command = Command.None;
                            data.Data.Position = data.CipherIndex;
                            GetPdu(settings, data, client);
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
                    reply.Xml.AppendStartTag(
                        TranslatorTags.AccessResponseSpecification);

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

        private static void HandleGeneralCiphering(GXDLMSSettings settings, GXReplyData data)
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
                AesGcmParameter p = new AesGcmParameter(settings.SourceSystemTitle,
                        settings.Cipher.BlockCipherKey,
                        settings.Cipher.AuthenticationKey);
                try
                {
                    byte[] tmp = GXCiphering.Decrypt(p, data.Data);
                    data.Data.Clear();
                    data.Data.Set(tmp);
                    data.CipheredCommand = Command.GeneralCiphering;
                    data.Command = Command.None;
                    if (p.Security != (byte)Security.None)
                    {
                        GetPdu(settings, data, null);
                    }
                }
                catch (Exception ex)
                {
                    if (data.Xml == null)
                    {
                        throw ex;
                    }
                    data.Xml.SetXmlLength(origPos);
                }
                if (data.Xml != null && p != null)
                {
                    data.Xml.AppendStartTag(Command.GeneralCiphering);
                    data.Xml.AppendLine(TranslatorTags.TransactionId, null,
                            data.Xml.IntegerToHex(p.InvocationCounter, 16, true));
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

        public static bool GetData(GXDLMSSettings settings,
                                   GXByteBuffer reply, GXReplyData data, GXReplyData notify, GXDLMSClient client)
        {
            byte frame = 0;
            bool isNotify = false;
            // If DLMS frame is generated.
            if (settings.InterfaceType == InterfaceType.HDLC ||
                settings.InterfaceType == InterfaceType.HdlcWithModeE)
            {
                frame = GetHdlcData(settings.IsServer, settings, reply, data, notify);
                if (notify != null && frame == 0x13)
                {
                    data = notify;
                    isNotify = true;
                }
                data.FrameId = frame;
            }
            else if (settings.InterfaceType == InterfaceType.WRAPPER)
            {
                if (!GetTcpData(settings, reply, data, notify))
                {
                    if (notify != null)
                    {
                        data = notify;
                    }
                    isNotify = true;
                }
            }
            else if (settings.InterfaceType == InterfaceType.WirelessMBus)
            {
                GetMBusData(settings, reply, data);
            }
            else if (settings.InterfaceType == InterfaceType.PDU)
            {
                data.PacketLength = reply.Size;
                data.IsComplete = reply.Size != 0;
            }
            else if (settings.InterfaceType == InterfaceType.Plc)
            {
                GetPlcData(settings, reply, data);
            }
            else if (settings.InterfaceType == InterfaceType.PlcHdlc)
            {
                frame = GetPlcHdlcData(settings, reply, data);
            }
            else
            {
                throw new ArgumentException("Invalid Interface type.");
            }
            // If all data is not read yet.
            if (!data.IsComplete)
            {
                return false;
            }
            if (settings.InterfaceType != InterfaceType.PlcHdlc)
            {
                GetDataFromFrame(reply, data, IsHdlc(settings.InterfaceType));
            }
            // If keepalive or get next frame request.
            if (data.Xml != null || ((frame != 0x13 || data.IsMoreData) && (frame & 0x1) != 0))
            {
                if ((settings.InterfaceType == InterfaceType.HDLC || settings.InterfaceType == InterfaceType.HdlcWithModeE) &&
                    (data.Error == (int)ErrorCode.Rejected || data.Data.Size != 0))
                {
                    System.Diagnostics.Debug.Assert(reply.GetUInt8(reply.Position - 1) == 0x7e);
                }
                return true;
            }
            GetPdu(settings, data, client);
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
                settings.Limits.MaxInfoRX = GXDLMSLimitsDefault.DefaultMaxInfoRX;
                settings.Limits.MaxInfoTX = GXDLMSLimitsDefault.DefaultMaxInfoTX;
                settings.Limits.WindowSizeRX = GXDLMSLimitsDefault.DefaultWindowSizeRX;
                settings.Limits.WindowSizeTX = GXDLMSLimitsDefault.DefaultWindowSizeTX;
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
                        settings.Limits.MaxInfoRX = Convert.ToUInt16(val);
                        break;
                    case HDLCInfo.MaxInfoRX:
                        settings.Limits.MaxInfoTX = Convert.ToUInt16(val);
                        if (settings.Limits.UseFrameSize)
                        {
                            byte[] secondaryAddress;
                            secondaryAddress = GXDLMS.GetHdlcAddressBytes(settings.ClientAddress, 0);
                            settings.Limits.MaxInfoTX += (UInt16)(10 + secondaryAddress.Length);
                        }
                        break;
                    case HDLCInfo.WindowSizeTX:
                        settings.Limits.WindowSizeRX = Convert.ToByte(val);
                        break;
                    case HDLCInfo.WindowSizeRX:
                        settings.Limits.WindowSizeTX = Convert.ToByte(val);
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
    }
}