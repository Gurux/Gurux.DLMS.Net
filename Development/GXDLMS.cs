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
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Internal;
#if !WINDOWS_UWP
using System.Security.Cryptography;
#endif
using System.IO;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMS implements methods to communicate with DLMS/COSEM metering devices.
    /// </summary>
    sealed class GXDLMS
    {
        const byte CipheringHeaderSize = 7 + 12 + 3;
        internal const int DATA_TYPE_OFFSET = 0xFF0000;

        /// <summary>
        /// Generates Invoke ID and priority.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <returns>Invoke ID and priority.</returns>
        static byte GetInvokeIDPriority(GXDLMSSettings settings)
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
                availableObjectTypes.Add(ObjectType.IecLocalPortSetup, typeof(GXDLMSIECOpticalPortSetup));
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
                return availableObjectTypes.Values.ToArray();
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
            if (type == RequestTypes.None)
            {
                throw new ArgumentException("Invalid receiverReady RequestTypes parameter.");
            }
            // Get next frame.
            if ((type & RequestTypes.Frame) != 0)
            {
                byte id = settings.ReceiverReady();
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
            GXByteBuffer bb = new GXByteBuffer(6);
            if (settings.UseLogicalNameReferencing)
            {
                bb.SetUInt32(settings.BlockIndex);
            }
            else
            {
                bb.SetUInt16((UInt16)settings.BlockIndex);
            }
            settings.IncreaseBlockIndex();
            byte[][] reply;
            if (settings.UseLogicalNameReferencing)
            {
                GXDLMSLNParameters p = new GXDLMSLNParameters(settings, 0, cmd, (byte)GetCommandType.NextDataBlock, bb, null, 0xff);
                reply = GXDLMS.GetLnMessages(p);
            }
            else
            {
                GXDLMSSNParameters p = new GXDLMSSNParameters(settings, cmd, 1, (byte)VariableAccessSpecification.BlockNumberAccess, bb, null);
                reply = GXDLMS.GetSnMessages(p);
            }
            return reply[0];
        }

        /// <summary>
        /// Get error description.
        /// </summary>
        /// <param name="error">Error number.</param>
        /// <returns>Error as plain text.</returns>
        internal static string GetDescription(ErrorCode error)
        {
#if !WINDOWS_UWP
            string str = null;
            switch (error)
            {
                case ErrorCode.Ok:
                    str = "";
                    break;
                case ErrorCode.Rejected:
                    str = "Connection rejected.";
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
            return error.ToString();
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
                    tp = GXCommon.GetValueType(value);
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
                default:
                    throw new GXDLMSException("Invalid GLO command.");
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
            int len = p.data.Size - p.data.Position;
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
                p.lastBlock = !(8 + reply.Size + len > p.settings.MaxPduSize);
            }
            if (p.lastBlock)
            {
                //Add command type and invoke and priority.
                p.lastBlock = !(8 + reply.Size + len > p.settings.MaxPduSize);
            }
        }

        /// <summary>
        /// Get next logical name PDU.
        /// </summary>
        /// <param name="p">LN parameters.</param>
        /// <param name="reply">Generated message.</param>
        internal static void GetLNPdu(GXDLMSLNParameters p, GXByteBuffer reply)
        {
            bool ciphering = p.settings.Cipher != null && p.settings.Cipher.Security != Gurux.DLMS.Enums.Security.None;
            int len = 0;
            if (!ciphering && p.settings.InterfaceType == InterfaceType.HDLC)
            {
                AddLLCBytes(p.settings, reply);
            }
            if (p.command == Command.Aarq)
            {
                reply.Set(p.attributeDescriptor);
            }
            else
            {
                if ((p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) != 0)
                {
                    reply.SetUInt8((byte)Command.GeneralBlockTransfer);
                    MultipleBlocks(p, reply, ciphering);
                    // Is last block
                    if (!p.lastBlock)
                    {
                        reply.SetUInt8(0);
                    }
                    else
                    {
                        reply.SetUInt8(0x80);
                    }
                    // Set block number sent.
                    reply.SetUInt8(0);
                    // Set block number acknowledged
                    reply.SetUInt8((byte)p.blockIndex);
                    ++p.blockIndex;
                    // Add APU tag.
                    reply.SetUInt8(0);
                    // Add Addl fields
                    reply.SetUInt8(0);
                }
                // Add command.
                reply.SetUInt8((byte)p.command);

                if (p.command == Command.DataNotification ||
                        p.command == Command.AccessRequest ||
                        p.command == Command.AccessResponse)
                {
                    // Add Long-Invoke-Id-And-Priority
                    if (p.InvokeId != 0)
                    {
                        reply.SetUInt32(p.InvokeId);
                    }
                    else
                    {
                        reply.SetUInt32(GetLongInvokeIDPriority(p.settings));
                    }
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
                }
                else
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
                        if (p.multipleBlocks)
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
                        if (p.multipleBlocks)
                        {
                            if (p.requestType == 1)
                            {
                                p.requestType = 2;
                            }
                        }
                    }
                    reply.SetUInt8(p.requestType);
                    // Add Invoke Id And Priority.
                    if (p.InvokeId != 0)
                    {
                        reply.SetUInt8((byte)p.InvokeId);
                    }
                    else
                    {
                        reply.SetUInt8(GetInvokeIDPriority(p.settings));
                    }
                }

                //Add attribute descriptor.
                reply.Set(p.attributeDescriptor);
                if (p.command != Command.DataNotification && (p.settings.NegotiatedConformance & Conformance.GeneralBlockTransfer) == 0)
                {
                    //If multiple blocks.
                    if (p.multipleBlocks)
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
                            len = p.data.Size - p.data.Position;
                        }
                        else
                        {
                            len = 0;
                        }
                        int totalLength = len + reply.Size;
                        if (ciphering)
                        {
                            totalLength += CipheringHeaderSize;
                        }

                        if (totalLength > p.settings.MaxPduSize)
                        {
                            len = p.settings.MaxPduSize - reply.Size;
                            if (ciphering)
                            {
                                len -= CipheringHeaderSize;
                            }
                            len -= GXCommon.GetObjectCountSizeInBytes(len);
                        }
                        GXCommon.SetObjectCount(len, reply);
                        reply.Set(p.data, len);
                    }
                }
                //Add data that fits to one block.
                if (len == 0)
                {
                    //Add status if reply.
                    if (p.status != 0xFF)
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
                        //Get request size can be bigger than PDU size.
                        if (p.command != Command.GetRequest && len + reply.Size > p.settings.MaxPduSize)
                        {
                            len = p.settings.MaxPduSize - reply.Size;
                        }
                        reply.Set(p.data, len);
                    }
                }
                if (ciphering)
                {
                    byte[] tmp = p.settings.Cipher.Encrypt((byte)GetGloMessage(p.command),
                                                           p.settings.Cipher.SystemTitle, reply.Array());
                    reply.Size = 0;
                    if (p.settings.InterfaceType == InterfaceType.HDLC)
                    {
                        AddLLCBytes(p.settings, reply);
                    }
                    if (p.command == Command.DataNotification)
                    {
                        // Add command.
                        reply.SetUInt8(tmp[0]);
                        // Add system title.
                        GXCommon.SetObjectCount(
                            p.settings.Cipher.SystemTitle.Length,
                            reply);
                        reply.Set(p.settings.Cipher.SystemTitle);
                        // Add data.
                        reply.Set(tmp, 1, tmp.Length - 1);
                    }
                    else
                    {
                        reply.Set(tmp);
                    }
                }
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
            if (p.command == Command.Aarq)
            {
                frame = 0x10;
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
                        messages.Add(GXDLMS.GetWrapperFrame(p.settings, reply));
                    }
                    else if (p.settings.InterfaceType == Enums.InterfaceType.HDLC)
                    {
                        messages.Add(GXDLMS.GetHdlcFrame(p.settings, frame, reply));
                        if (reply.Position != reply.Size)
                        {
                            if (p.settings.IsServer || p.command == Command.SetRequest)
                            {
                                frame = 0;
                            }
                            else
                            {
                                frame = p.settings.NextSend(false);
                            }
                        }
                    }
                    else if (p.settings.InterfaceType == Enums.InterfaceType.PDU)
                    {
                        messages.Add(reply.Array());
                        frame = 0;
                        break;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("InterfaceType");
                    }
                }
                reply.Clear();
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
            if (p.command == Command.Aarq)
            {
                frame = 0x10;
            }
            else if (p.command == Command.None)
            {
                frame = p.settings.NextSend(true);
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
                        messages.Add(GXDLMS.GetWrapperFrame(p.settings, reply));
                    }
                    else if (p.settings.InterfaceType == Enums.InterfaceType.HDLC)
                    {
                        messages.Add(GXDLMS.GetHdlcFrame(p.settings, frame, reply));
                        if (reply.Position != reply.Size)
                        {
                            if (p.settings.IsServer)
                            {
                                frame = 0;
                            }
                            else
                            {
                                frame = p.settings.NextSend(false);
                            }
                        }
                    }
                    else if (p.settings.InterfaceType == Enums.InterfaceType.PDU)
                    {
                        messages.Add(reply.Array());
                        break;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("InterfaceType");
                    }
                }
                reply.Clear();
            } while (p.data != null && p.data.Position != p.data.Size);
            return messages.ToArray();
        }

        static int AppendMultipleSNBlocks(GXDLMSSNParameters p, GXByteBuffer header, GXByteBuffer reply)
        {
            bool ciphering = p.settings.Cipher != null && p.settings.Cipher.Security != Gurux.DLMS.Enums.Security.None;
            int hSize = reply.Size + 3;
            if (header != null)
            {
                hSize += header.Size;
            }
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
                if (p.settings.InterfaceType == InterfaceType.HDLC)
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
            if (header != null)
            {
                GXCommon.SetObjectCount(maxSize + header.Size, reply);
                reply.Set(header);
            }
            else
            {
                GXCommon.SetObjectCount(maxSize, reply);
            }
            return maxSize;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="p"></param>
        /// <param name="reply"></param>
        internal static void GetSNPdu(GXDLMSSNParameters p, GXByteBuffer reply)
        {
            bool ciphering = p.settings.Cipher != null && p.settings.Cipher.Security != Gurux.DLMS.Enums.Security.None;
            if (!ciphering && p.settings.InterfaceType == InterfaceType.HDLC)
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
            if (p.command != Command.Aarq && p.command != Command.Aare)
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
                        GXByteBuffer tmp = new GXByteBuffer();
                        int offset = 1;
                        if (!ciphering && p.settings.InterfaceType == InterfaceType.HDLC)
                        {
                            offset = 4;
                        }
                        tmp.Set(reply.Data, offset, reply.Size - offset);
                        reply.Size = 0;
                        if (!ciphering && p.settings.InterfaceType == InterfaceType.HDLC)
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
                        cnt = GXDLMS.AppendMultipleSNBlocks(p, tmp, reply);
                    }
                }
                else
                {
                    cnt = GXDLMS.AppendMultipleSNBlocks(p, null, reply);
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
                byte[] tmp = p.settings.Cipher.Encrypt((byte)GetGloMessage(p.command), p.settings.Cipher.SystemTitle, reply.Array());
                System.Diagnostics.Debug.Assert(!(p.settings.MaxPduSize < tmp.Length));
                reply.Size = 0;
                if (p.settings.InterfaceType == InterfaceType.HDLC)
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
        internal static Object GetHhlcAddress(int value, byte size)
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
        private static byte[] GetHdlcAddressBytes(int value, byte size)
        {
            Object tmp = GetHhlcAddress(value, size);
            GXByteBuffer bb = new GXByteBuffer();
            if (tmp is byte)
            {
                bb.SetUInt8((byte)tmp);
            }
            else if (tmp is UInt16)
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
        /// <param name="data"> Wrapped data.</param>
        /// <returns>Wrapper frames</returns>
        internal static byte[] GetWrapperFrame(GXDLMSSettings settings, GXByteBuffer data)
        {
            GXByteBuffer bb = new GXByteBuffer();
            // Add version.
            bb.SetUInt16(1);
            if (settings.IsServer)
            {
                bb.SetUInt16((UInt16)settings.ServerAddress);
                bb.SetUInt16((UInt16)settings.ClientAddress);
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
            int frameSize, len = 0;
            byte[] primaryAddress, secondaryAddress;
            if (settings.IsServer)
            {
                primaryAddress = GetHdlcAddressBytes(settings.ClientAddress, 0);
                secondaryAddress = GetHdlcAddressBytes(settings.ServerAddress, settings.ServerAddressSize);
            }
            else
            {
                primaryAddress = GetHdlcAddressBytes(settings.ServerAddress, settings.ServerAddressSize);
                secondaryAddress = GetHdlcAddressBytes(settings.ClientAddress, 0);
            }
            // Add BOP
            bb.SetUInt8(GXCommon.HDLCFrameStartEnd);
            frameSize = Convert.ToInt32(settings.Limits.MaxInfoTX);
            // If no data
            if (data == null || data.Size == 0)
            {
                bb.SetUInt8(0xA0);
            }
            else if (data.Size - data.Position <= frameSize)
            {
                len = data.Size - data.Position;
                // Is last packet.
                bb.SetUInt8((byte)(0xA0 | (len >> 8) & 0x7));
            }
            else
            {
                len = frameSize;
                // More data to left.
                bb.SetUInt8((byte)(0xA8 | (len >> 8) & 0x7));
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

        static byte GetHdlcData(bool server, GXDLMSSettings settings, GXByteBuffer reply, GXReplyData data)
        {
            short ch;
            int pos, packetStartID = reply.Position, frameLen = 0;
            int crc, crcRead;
            // If whole frame is not received yet.
            if (reply.Size - reply.Position < 9)
            {
                data.IsComplete = false;
                return 0;
            }
            data.IsComplete = true;
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
                // Not enough data to parse;
                return 0;
            }
            byte frame = reply.GetUInt8();
            if ((frame & 0xF0) != 0xA0)
            {
                //If same strage data.
                return GetHdlcData(server, settings, reply, data);
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
                throw new GXDLMSException("Invalid data format.");
            }

            // Check addresses.
            if (!CheckHdlcAddress(server, settings, reply, eopPos))
            {
                //If echo,
                reply.Position = 1 + eopPos;
                return GetHdlcData(server, settings, reply, data);
            }

            // Is there more data available.
            if ((frame & 0x8) != 0)
            {
                data.MoreData = (RequestTypes)(data.MoreData | RequestTypes.Frame);
            }
            else
            {
                data.MoreData = (RequestTypes)(data.MoreData & ~RequestTypes.Frame);
            }
            // Get frame type.
            frame = reply.GetUInt8();
            if (data.Xml == null && !settings.CheckFrame(frame))
            {
                reply.Position = (eopPos + 1);
                return GetHdlcData(server, settings, reply, data);
            }
            // Check that header CRC is correct.
            crc = GXFCS16.CountFCS16(reply.Data, packetStartID + 1, reply.Position - packetStartID - 1);
            crcRead = reply.GetUInt16();
            if (crc != crcRead)
            {
                throw new Exception("Wrong CRC.");
            }
            // Check that packet CRC match only if there is a data part.
            if (reply.Position != packetStartID + frameLen + 1)
            {
                crc = GXFCS16.CountFCS16(reply.Data, packetStartID + 1,
                                         frameLen - 2);
                crcRead = reply.GetUInt16(packetStartID + frameLen - 1);
                if (crc != crcRead)
                {
                    throw new Exception("Wrong CRC.");
                }
                // Remove CRC and EOP from packet length.
                data.PacketLength = eopPos - 2;
            }
            else
            {
                data.PacketLength = reply.Position + 1;
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
                data.Command = (Command)frame;
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
                    System.Diagnostics.Debug.WriteLine("Get next frame.");
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
            int index)
        {
            int source, target;
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
                    return false;
                }
                // Check that server addresses match.
                if (settings.ServerAddress != source)
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
        /// <param name="data"> Reply information.</param>
        static void GetTcpData(GXDLMSSettings settings,
                               GXByteBuffer buff, GXReplyData data)
        {
            // If whole frame is not received yet.
            if (buff.Size - buff.Position < 8)
            {
                data.IsComplete = false;
                return;
            }
            int pos = buff.Position;
            int value;
            // Get version
            value = buff.GetUInt16();
            if (value != 1)
            {
                throw new GXDLMSException("Unknown version.");
            }

            // Check TCP/IP addresses.
            CheckWrapperAddress(settings, buff);
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
        }

        private static void CheckWrapperAddress(GXDLMSSettings settings,
                                                GXByteBuffer buff)
        {
            int value;
            if (settings.IsServer)
            {
                value = buff.GetUInt16();
                // Check that client addresses match.
                if (settings.ClientAddress != 0
                        && settings.ClientAddress != value)
                {
                    throw new GXDLMSException(
                        "Source addresses do not match. It is "
                        + value.ToString() + ". It should be "
                        + settings.ClientAddress.ToString()
                        + ".");
                }
                else
                {
                    settings.ClientAddress = value;
                }

                value = buff.GetUInt16();
                // Check that server addresses match.
                if (settings.ServerAddress != 0
                        && settings.ServerAddress != value)
                {
                    throw new GXDLMSException(
                        "Destination addresses do not match. It is "
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
                if (settings.ClientAddress != 0
                        && settings.ServerAddress != value)
                {
                    throw new GXDLMSException(
                        "Source addresses do not match. It is "
                        + value.ToString() + ". It should be "
                        + settings.ServerAddress.ToString()
                        + ".");

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
                    throw new GXDLMSException(
                        "Destination addresses do not match. It is "
                        + value.ToString() + ". It should be "
                        + settings.ClientAddress.ToString()
                        + ".");
                }
                else
                {
                    settings.ClientAddress = value;
                }
            }
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
            // Check block length when all data is received.
            if ((reply.MoreData & RequestTypes.Frame) == 0)
            {
                if (blockLength != reply.Data.Size - reply.Data.Position)
                {
                    throw new OutOfMemoryException();
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
            }
            GetDataFromBlock(reply.Data, index);
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
            // If last packet and data is not try to peek.
            if (reply.MoreData == RequestTypes.None)
            {
                reply.Data.Position = 0;
                HandleReadResponse(settings, reply, index);
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
            int pos = 0, cnt;
            if (reply.Count == 0)
            {
                cnt = GXCommon.GetObjectCount(reply.Data);
            }
            else
            {
                cnt = reply.TotalCount;
            }
            //Set total count if not set yet.
            if (reply.TotalCount == 0)
            {
                reply.TotalCount = cnt;
            }
            SingleReadResponse type = SingleReadResponse.Data;
            List<object> values = null;
            if (cnt != 1)
            {
                values = new List<object>();
                reply.Value = null;
            }
            if (reply.Xml != null)
            {
                reply.Xml.AppendStartTag(Command.ReadResponse, "Qty", reply.Xml.IntegerToHex(cnt, 2));
            }
            for (; pos != cnt; ++pos)
            {
                if (reply.Data.Available == 0)
                {
                    if (cnt != 1)
                    {
                        reply.Value = values.ToArray();
                    }
                    return false;
                }
                // Get status code.
                reply.CommandType = reply.Data.GetUInt8();
                type = (SingleReadResponse)reply.CommandType;
                bool standardXml = reply.Xml != null && reply.Xml.OutputType == TranslatorOutputType.StandardXml;
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
                            if (reply.Data.Position == reply.ReadPosition)
                            {
                                //If multiple values remove command.
                                if (cnt != 1 && reply.TotalCount == 0)
                                {
                                    ++index;
                                }
                                reply.TotalCount = 0;
                                reply.Data.Position = index;
                                GetDataFromBlock(reply.Data, 0);
                                reply.Value = null;
                                //Ask that data is parsed after last block is received.
                                reply.CommandType = (byte)SingleReadResponse.DataBlockResult;
                                return false;
                            }
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
                if (data.Xml
                        .OutputType == TranslatorOutputType.StandardXml)
                {
                    data.Xml
                    .AppendStartTag(TranslatorTags.SingleResponse);
                }
                data.Xml.AppendLine(TranslatorTags.Result, null,
                                    GXDLMSTranslator.ErrorCodeToString(
                                        data.Xml.OutputType,
                                        (ErrorCode)data.Error));
            }
            // Response normal. Get data if exists. Some meters do not return here anything.
            if (data.Data.Position < data.Data.Size)
            {
                //Get-Data-Result
                ret = data.Data.GetUInt8();
                //If data.
                if (ret == 0)
                {
                    GetDataFromBlock(data.Data, 0);
                }
                else
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
                if (data.Xml != null)
                {

                    data.Xml.AppendStartTag(TranslatorTags.ReturnParameters);
                    if (ret != 0)
                    {
                        data.Xml.AppendLine(
                            TranslatorTags.DataAccessError, null,
                            GXDLMSTranslator.ErrorCodeToString(
                                data.Xml.OutputType, (ErrorCode)
                                ret));

                    }
                    else
                    {
                        data.Xml.AppendStartTag(Command.ReadResponse,
                                                SingleReadResponse.Data);
                        if (data.Data.Position == data.Data.Size)
                        {
                            int tag = GXDLMS.DATA_TYPE_OFFSET | (int)DataType.None;
                            data.Xml.AppendStartTag(tag, null, null);
                            data.Xml.AppendEndTag(tag);
                        }
                        else
                        {
                            GXDataInfo di = new GXDataInfo();
                            di.xml = data.Xml;
                            GXCommon.GetData(settings, data.Data, di);
                        }
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
            byte invoke = data.Data.GetUInt8();
            if (data.Xml != null)
            {
                data.Xml.AppendStartTag(Command.MethodResponse);
                data.Xml.AppendStartTag(Command.MethodResponse, type);
                //InvokeIdAndPriority
                data.Xml.AppendLine(TranslatorTags.InvokeId, "Value",
                                        data.Xml.IntegerToHex(invoke, 2));
            }
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
            byte invokeId = data.Data.GetUInt8();
            if (data.Xml != null)
            {
                data.Xml.AppendStartTag(Command.SetResponse);
                data.Xml.AppendStartTag(Command.SetResponse, type);
                //InvokeIdAndPriority
                data.Xml.AppendLine(TranslatorTags.InvokeId, "Value", data.Xml.IntegerToHex(invokeId, 2));
            }

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
            else if (type == SetResponseType.DataBlock ||
                type == SetResponseType.LastDataBlock)
            {
                data.Data.GetUInt32();
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

            // Get type.
            GetCommandType type = (GetCommandType)data.GetUInt8();
            // Get invoke ID and priority.
            ch = data.GetUInt8();

            if (reply.Xml != null)
            {
                reply.Xml.AppendStartTag(Command.GetResponse);
                reply.Xml.AppendStartTag(Command.GetResponse, type);
                //InvokeIdAndPriority
                reply.Xml.AppendLine(TranslatorTags.InvokeId, "Value", reply.Xml.IntegerToHex(ch, 2));
            }
            // Response normal
            if (type == GetCommandType.Normal)
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
                    }
                    //Result
                    reply.Xml.AppendStartTag(TranslatorTags.Result);
                    reply.Xml.AppendLine(TranslatorTags.RawData, "Value",
                                         GXCommon.ToHex(reply.Data.Data, false, data.Position, reply.Data.Size - data.Position));
                    reply.Xml.AppendEndTag(TranslatorTags.Result);
                }
                else if (data.Position != data.Size)
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
                        reply.Command = Command.None;
                    }
                    GetDataFromBlock(data, index);
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
            }
            else if (type == GetCommandType.WithList)
            {
                //Get object count.
                int cnt = GXCommon.GetObjectCount(data);
                object[] values = new object[cnt];
                if (reply.Xml != null)
                {
                    //Result start tag.
                    reply.Xml.AppendStartTag(TranslatorTags.Result, "Qty", reply.Xml.IntegerToHex(cnt, 2));
                }
                for (int pos = 0; pos != cnt; ++pos)
                {
                    // Result
                    ch = data.GetUInt8();
                    if (ch != 0)
                    {
                        reply.Error = data.GetUInt8();
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
                            if (values != null)
                            {
                                values[pos] = reply.Value;
                            }
                            reply.Value = null;
                        }
                    }
                }
                reply.Value = values;
                ret = false;
            }
            else
            {
                throw new ArgumentException("Invalid Get response.");
            }
            if (reply.Xml != null)
            {
                reply.Xml.AppendEndTag(TranslatorTags.Result);
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
        private static void HandleGbt(GXDLMSSettings settings, GXReplyData data)
        {
            data.Gbt = true;
            int index = data.Data.Position - 1;
            byte ch = data.Data.GetUInt8();

            //Is streaming active.
            bool streaming = (ch & 0x40) == 1;
            byte window = (byte)(ch & 0x3F);
            //Block number.
            byte bn = data.Data.GetUInt8();
            //Block number acknowledged.
            byte bna = data.Data.GetUInt8();
            // Get APU tag.
            if (data.Data.GetUInt8() != 0)
            {
                throw new Exception("Invalid APU.");
            }
            // Get Addl tag.
            if (data.Data.GetUInt8() != 0)
            {
                throw new Exception("Invalid APU.");
            }
            data.Command = Command.None;
            if (window != 0)
            {
                int len = GXCommon.GetObjectCount(data.Data);
                if (len != data.Data.Size - data.Data.Position)
                {
                    data.IsComplete = false;
                    return;
                }
            }
            GetDataFromBlock(data.Data, index);
            GetPdu(settings, data);
            //Is Last block,
            if ((ch & 0x80) == 0)
            {
                data.MoreData |= RequestTypes.DataBlock;
            }
            else
            {
                data.MoreData &= ~RequestTypes.DataBlock;
            }
            // Get data if all data is read or we want to peek data.
            if (data.Data.Position != data.Data.Size
                    && (data.Command == Command.ReadResponse
                        || data.Command == Command.GetResponse)
                    && (data.MoreData == RequestTypes.None
                        || data.Peek))
            {
                data.Data.Position = 0;
                GetValueFromData(settings, data);
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

        private static void HandledGloRequest(GXDLMSSettings settings,
                                              GXReplyData data)
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
                    settings.Cipher.Decrypt(settings.SourceSystemTitle, data.Data);
                    // Get command.
                    data.Command = (Command)data.Data.GetUInt8();
                }
                else
                {
                    --data.Data.Position;
                }
            }
        }

        private static void HandledGloResponse(GXDLMSSettings settings,
                                               GXReplyData data, int index)
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
                    settings.Cipher.Decrypt(settings.SourceSystemTitle, bb);
                    data.Data.Set(bb);
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
        public static void GetPdu(GXDLMSSettings settings,
                                  GXReplyData data)
        {
            short ch;
            Command cmd = data.Command;
            // If header is not read yet or GBT message.
            if (data.Command == Command.None || data.Gbt)
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
                        HandleGbt(settings, data);
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
                        throw new GXDLMSException((ExceptionStateError)data.Data.GetUInt8(),
                                                  (ExceptionServiceError)data.Data.GetUInt8());
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
                        HandledGloRequest(settings, data);
                        // Server handles this.
                        break;
                    case Command.GloReadResponse:
                    case Command.GloWriteResponse:
                    case Command.GloGetResponse:
                    case Command.GloSetResponse:
                    case Command.GloMethodResponse:
                    case Command.GloEventNotificationRequest:
                        HandledGloResponse(settings, data, index);
                        break;
                    case Command.GeneralGloCiphering:
                        if (settings.IsServer)
                        {
                            HandledGloRequest(settings, data);
                        }
                        else
                        {
                            HandledGloResponse(settings, data, index);
                        }
                        break;

                    case Command.DataNotification:
                        HandleDataNotification(settings, data);
                        //Client handles this.
                        break;
                    case Command.GeneralCiphering:
                        HandleGeneralCiphering(settings, data);
                        break;
                    default:
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
                // Get command if operating as a server.
                if (settings.IsServer)
                {
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
                            GetPdu(settings, data);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    //If we are receiving last read block in frames.
                    if (data.Command == Command.ReadResponse && !data.IsMoreData && data.CommandType == (byte)SingleReadResponse.DataBlockResult)
                    {
                        data.Data.Position = 0;
                        if (!HandleReadResponse(settings, data, -1))
                        {
                            return;
                        }
                    }
                    else if (data.Command == Command.ReadResponse && !data.IsMoreData && data.CommandType == (byte)SingleReadResponse.Data &&
                        data.Value != null)
                    {
                        if (!HandleReadResponse(settings, data, -1))
                        {
                            return;
                        }
                    }
                    // Client do not need a command any more.
                    data.Command = Command.None;
                    //Ciphered messages are handled after whole PDU is received.
                    switch (cmd)
                    {
                        case Command.GloReadResponse:
                        case Command.GloWriteResponse:
                        case Command.GloGetResponse:
                        case Command.GloSetResponse:
                        case Command.GloMethodResponse:
                            data.Data.Position = data.CipherIndex;
                            GetPdu(settings, data);
                            break;
                        default:
                            break;
                    }
                }
            }
            // Get data if all data is read or we want to peek data.
            if (data.Xml == null && data.Data.Position != data.Data.Size &&
                    (cmd == Command.ReadResponse || cmd == Command.GetResponse)
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

        private static void HandleDataNotification(GXDLMSSettings settings, GXReplyData reply)
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
                data.Data.Position = data.Data.Position - 1;
                settings.Cipher.Decrypt(null, data.Data);
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
            if (reply.Value is Object[])
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
                        if (!(value is Object[]))
                        {
                            reply.DataType = info.Type;
                            reply.Value = value;
                            reply.TotalCount = 0;
                            reply.ReadPosition = data.Position;
                        }
                        else
                        {
                            if (((Object[])value).Length != 0)
                            {
                                if (reply.Value == null)
                                {
                                    reply.Value = value;
                                }
                                else
                                {
                                    // Add items to collection.
                                    List<Object> list = new List<Object>();
                                    list.AddRange((Object[])reply.Value);
                                    list.AddRange((Object[])value);
                                    reply.Value = list.ToArray();
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
                                   GXByteBuffer reply, GXReplyData data)
        {
            byte frame = 0;
            // If DLMS frame is generated.
            if (settings.InterfaceType == InterfaceType.HDLC)
            {
                frame = GetHdlcData(settings.IsServer, settings, reply, data);
                data.FrameId = frame;
            }
            else if (settings.InterfaceType == InterfaceType.WRAPPER)
            {
                GetTcpData(settings, reply, data);
            }
            else if (settings.InterfaceType == InterfaceType.PDU)
            {
                data.PacketLength = reply.Size;
                data.IsComplete = true;
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

            GetDataFromFrame(reply, data);
            // If keepalive or get next frame request.
            if (data.Xml != null || (frame & 0x1) != 0)
            {
                if (settings.InterfaceType == InterfaceType.HDLC && (data.Error == (int)ErrorCode.Rejected || data.Data.Size != 0))
                {
                    if (reply.Position != reply.Size)
                    {
                        reply.Position += 3;
                    }
                    System.Diagnostics.Debug.Assert(reply.GetUInt8(reply.Position - 1) == 0x7e);
                }
                return true;
            }
            GetPdu(settings, data);

            if (data.Command == Command.DataNotification)
            {
                // Check is there more messages left.
                // This is Push message special case.
                if (reply.Position == reply.Size)
                {
                    reply.Clear();
                }
                else
                {
                    int cnt = reply.Size - reply.Position;
                    reply.Move(reply.Position, 0, cnt);
                    reply.Position = 0;
                }
            }
            return true;
        }

        /// <summary>
        /// Get data from HDLC or wrapper frame.
        /// </summary>
        /// <param name="reply">Received data that includes HDLC frame.</param>
        /// <param name="info">Reply data.</param>
        private static void GetDataFromFrame(GXByteBuffer reply, GXReplyData info)
        {
            GXByteBuffer data = info.Data;
            int offset = data.Size;
            int cnt = info.PacketLength - reply.Position;
            if (cnt != 0)
            {
                data.Capacity = (offset + cnt);
                data.Set(reply.Data, reply.Position, cnt);
                reply.Position = (reply.Position + cnt);
            }
            // Set position to begin of new data.
            data.Position = offset;
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
                case ObjectType.Data:
                case ObjectType.ActionSchedule:
                case ObjectType.None:
                case ObjectType.AutoAnswer:
                case ObjectType.AutoConnect:
                case ObjectType.MacAddressSetup:
                case ObjectType.GprsSetup:
                case ObjectType.IecHdlcSetup:
                case ObjectType.IecLocalPortSetup:
                case ObjectType.IecTwistedPairSetup:
                case ObjectType.ModemConfiguration:
                case ObjectType.PppSetup:
                case ObjectType.RegisterMonitor:
                case ObjectType.SapAssignment:
                case ObjectType.ZigBeeSasStartup:
                case ObjectType.ZigBeeSasJoin:
                case ObjectType.ZigBeeSasApsFragmentation:
                case ObjectType.Schedule:
                case ObjectType.StatusMapping:
                case ObjectType.TcpUdpSetup:
                case ObjectType.UtilityTables:
                    value = 0;
                    count = 0;
                    break;
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
            }
        }

        internal static UInt16 RowsToPdu(GXDLMSSettings settings, GXDLMSProfileGeneric pg)
        {
            //Count how many rows we can fit to one PDU.
            DataType dt;
            int rowsize = 0;
            foreach (var it in pg.CaptureObjects)
            {
                dt = it.Key.GetDataType(it.Value.AttributeIndex);
                if (dt == DataType.OctetString)
                {
                    if (it.Key.GetUIDataType(it.Value.AttributeIndex) == DataType.DateTime)
                    {
                        rowsize += GXCommon.GetDataTypeSize(DataType.DateTime);
                    }
                    else if (it.Key.GetUIDataType(it.Value.AttributeIndex) == DataType.Date)
                    {
                        rowsize += GXCommon.GetDataTypeSize(DataType.Date);
                    }
                    else if (it.Key.GetUIDataType(it.Value.AttributeIndex) == DataType.Time)
                    {
                        rowsize += GXCommon.GetDataTypeSize(DataType.Time);
                    }
                }
                else if (dt == DataType.None)
                {
                    rowsize += 2;
                }
                else
                {
                    rowsize += GXCommon.GetDataTypeSize(dt);
                }
            }
            if (rowsize != 0)
            {
                return (UInt16)(settings.MaxPduSize / rowsize);
            }
            return 0;
        }
    }
}