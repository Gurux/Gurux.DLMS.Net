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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using System.Diagnostics;
using Gurux.DLMS.Objects;
using System.Text;

namespace Gurux.DLMS
{
    /// <summary>
    /// This class is used to handle PRIME DC notifications.
    /// </summary>
    internal sealed class GXPrimeDcHandlers
    {        
        /// <summary>
        /// Handle new device notification message.
        /// </summary>
        /// <param name="data">Received data</param>
        /// <param name="replyData">Reply data.</param>
        /// <param name="xml">XML settings.</param>
        private static void HandleNewDeviceNotification(GXByteBuffer data,
            GXReplyData replyData,
            GXDLMSTranslatorStructure xml)
        {            
            UInt16 deviceId = data.GetUInt16();
            UInt16 capabilities = data.GetUInt16();
            byte len = data.GetUInt8();
            byte[] id = new byte[len];
            data.Get(id);
            byte[] eui48 = new byte[6];
            data.Get(eui48);
            if (replyData != null)
            {
                replyData.PrimeDc = new GXDLMSPrimeDataConcentrator()
                {
                    Type = PrimeDcMsgType.NewDeviceNotification,
                    DeviceID = deviceId,
                    Capabilities = capabilities,
                    DlmsId = id,
                    Eui48 = eui48
                };
            }
            if (xml != null)
            {
                xml.AppendLine("<DeviceId Value=\"" + deviceId + "\" />");
                xml.AppendLine("<Capabilities Value=\"" + capabilities + "\" />");
                if (xml.Comments)
                {
                    xml.AppendComment("DLMS ID " + ASCIIEncoding.ASCII.GetString(id));
                }
                xml.AppendLine("<DlmsId Value=\"" + GXCommon.ToHex(id, false) + "\" />");
                xml.AppendLine("<Eui48 Value=\"" + GXCommon.ToHex(eui48, false) + "\" />");
            }
        }

        /// <summary>
        /// Handle remove device notification message.
        /// </summary>
        /// <param name="data">Received data</param>
        /// <param name="replyData">Reply data.</param>
        /// <param name="xml">XML settings.</param>
        private static void HandleRemoveDeviceNotification(GXByteBuffer data,
            GXReplyData replyData,
            GXDLMSTranslatorStructure xml)
        {
            UInt16 deviceId = data.GetUInt16();
            if (replyData != null)
            {
                replyData.PrimeDc = new GXDLMSPrimeDataConcentrator()
                {
                    Type = PrimeDcMsgType.RemoveDeviceNotification,
                    DeviceID = deviceId,
                };
            }
            if (xml != null)
            {
                xml.AppendLine("<DeviceId Value=\"" + deviceId + "\" />");
            }
        }

        /// <summary>
        /// Handle start reporting meters notification message.
        /// </summary>
        /// <param name="data">Received data</param>
        /// <param name="replyData">Reply data.</param>
        /// <param name="xml">XML settings.</param>
        private static void HandleStartReportingMeters(GXByteBuffer data,
            GXReplyData replyData,
            GXDLMSTranslatorStructure xml)
        {
            if (replyData != null)
            {
                replyData.PrimeDc = new GXDLMSPrimeDataConcentrator()
                {
                    Type = PrimeDcMsgType.StartReportingMeters
                };
            }
        }
        /// <summary>
        /// Handle delete meters notification message.
        /// </summary>
        /// <param name="data">Received data</param>
        /// <param name="replyData">Reply data.</param>
        /// <param name="xml">XML settings.</param>
        private static void HandleDeleteMeters(GXByteBuffer data,
            GXReplyData replyData,
            GXDLMSTranslatorStructure xml)
        {
            UInt16 deviceId = data.GetUInt16();
            if (replyData != null)
            {
                replyData.PrimeDc = new GXDLMSPrimeDataConcentrator()
                {
                    Type = PrimeDcMsgType.DeleteMeters,
                    DeviceID = deviceId,
                };
            }
            if (xml != null)
            {
                xml.AppendLine("<DeviceId Value=\"" + deviceId + "\" />");
            }
        }

        /// <summary>
        /// Handle enable auto close notification message.
        /// </summary>
        /// <param name="data">Received data</param>
        /// <param name="replyData">Reply data.</param>
        /// <param name="xml">XML settings.</param>
        private static void HandleEnableAutoClose(GXByteBuffer data,
            GXReplyData replyData,
            GXDLMSTranslatorStructure xml)
        {
            UInt16 deviceId = data.GetUInt16();
            if (replyData != null)
            {
                replyData.PrimeDc = new GXDLMSPrimeDataConcentrator()
                {
                    Type = PrimeDcMsgType.EnableAutoClose,
                    DeviceID = deviceId,
                };
            }
            if (xml != null)
            {
                xml.AppendLine("<DeviceId Value=\"" + deviceId + "\" />");
            }
        }
        /// <summary>
        /// Handle disable auto close notification message.
        /// </summary>
        /// <param name="data">Received data</param>
        /// <param name="replyData">Reply data.</param>
        /// <param name="xml">XML settings.</param>
        private static void HandleDisableAutoClose(GXByteBuffer data,
            GXReplyData replyData,
            GXDLMSTranslatorStructure xml)
        {
            UInt16 deviceId = data.GetUInt16();
            if (replyData != null)
            {
                replyData.PrimeDc = new GXDLMSPrimeDataConcentrator()
                {
                    Type = PrimeDcMsgType.DisableAutoClose,
                    DeviceID = deviceId,
                };
            }
            if (xml != null)
            {
                xml.AppendLine("<DeviceId Value=\"" + deviceId + "\" />");
            }
        }

        public static bool HandleNotification(GXByteBuffer data,
            GXReplyData replyData,
            GXDLMSTranslatorStructure xml)
        {
            PrimeDcMsgType type = (PrimeDcMsgType)data.GetUInt8();
            switch (type)
            {
                case PrimeDcMsgType.NewDeviceNotification:
                    xml?.AppendStartTag(TranslatorGeneralTags.PrimeNewDeviceNotification);
                    HandleNewDeviceNotification(data, replyData, xml);
                    xml?.AppendEndTag(TranslatorGeneralTags.PrimeNewDeviceNotification);
                    break;
                case PrimeDcMsgType.RemoveDeviceNotification:
                    xml?.AppendStartTag(TranslatorGeneralTags.PrimeRemoveDeviceNotification);
                    HandleRemoveDeviceNotification(data, replyData, xml);
                    xml?.AppendEndTag(TranslatorGeneralTags.PrimeRemoveDeviceNotification);
                    break;
                case PrimeDcMsgType.StartReportingMeters:
                    xml?.AppendStartTag(TranslatorGeneralTags.PrimeStartReportingMeters);
                    HandleStartReportingMeters(data, replyData, xml);
                    xml?.AppendEndTag(TranslatorGeneralTags.PrimeStartReportingMeters);
                    break;
                case PrimeDcMsgType.DeleteMeters:
                    xml?.AppendStartTag(TranslatorGeneralTags.PrimeDeleteMeters);
                    HandleDeleteMeters(data, replyData, xml);
                    xml?.AppendEndTag(TranslatorGeneralTags.PrimeDeleteMeters);
                    break;
                case PrimeDcMsgType.EnableAutoClose:
                    xml?.AppendStartTag(TranslatorGeneralTags.PrimeEnableAutoClose);
                    HandleEnableAutoClose(data, replyData, xml);
                    xml?.AppendEndTag(TranslatorGeneralTags.PrimeEnableAutoClose);
                    break;
                case PrimeDcMsgType.DisableAutoClose:
                    xml?.AppendStartTag(TranslatorGeneralTags.PrimeDisableAutoClose);
                    HandleDisableAutoClose(data, replyData, xml);
                    xml?.AppendEndTag(TranslatorGeneralTags.PrimeDisableAutoClose);
                    break;
                default:
                    --data.Position;
                    return false ;
            }
            return true;
        }

        ///<summary>
        ///Handle set request.
        ///</summary>
        ///<returns>
        ///Reply to the client.
        ///</returns>
        public static void HandleSetRequest(GXDLMSSettings settings, GXDLMSServer server, GXByteBuffer data, GXByteBuffer replyData, GXDLMSTranslatorStructure xml, Command cipheredCommand)
        {
            //Return error if connection is not established.
            if (xml == null && (settings.Connected & ConnectionState.Dlms) == 0 && cipheredCommand == Command.None)
            {
                replyData.Set(GXDLMSServer.GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                              ServiceError.Service, (byte)Service.Unsupported));
                return;
            }
            // Get type.
            byte type = data.GetUInt8();
            GXDLMSLNParameters p = null;
            try
            {
                // Get invoke ID and priority.
                byte invoke = data.GetUInt8();
                settings.UpdateInvokeId(invoke);
                p = new GXDLMSLNParameters(settings, invoke, Command.SetResponse, (byte)type, null, null, 0xFF, cipheredCommand);
                // SetRequest normal or Set Request With First Data Block
                if (xml != null)
                {
                    if (type < 6)
                    {
                        GXDLMS.AddInvokeId(xml, Command.SetRequest, (SetRequestType)type, invoke);
                    }
                    else
                    {
                        xml.AppendStartTag(Command.SetRequest);
                        xml.AppendComment("Unknown tag: " + type);
                        //InvokeIdAndPriority
                        xml.AppendLine(TranslatorTags.InvokeId, "Value", xml.IntegerToHex(invoke, 2));
                    }
                }
                switch (type)
                {
                    case (byte)SetRequestType.Normal:
                    case (byte)SetRequestType.FirstDataBlock:
                        if (type == (byte)SetRequestType.Normal)
                        {
                            p.status = 0;
                        }
                        HandleSetRequestNormal(settings, server, data, (byte)type, p, replyData, xml);
                        break;
                    case (byte)SetRequestType.WithDataBlock:
                        HanleSetRequestWithDataBlock(settings, server, data, p, replyData, xml);
                        break;
                    case (byte)SetRequestType.WithList:
                        HanleSetRequestWithList(settings, invoke, server, data, p, replyData, xml);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("HandleSetRequest failed. Unknown command.");
                        data.Clear();
                        settings.ResetBlockIndex();
                        p.status = (byte)ErrorCode.ReadWriteDenied;
                        break;
                }
                if (xml != null)
                {
                    if (type < 6)
                    {
                        xml.AppendEndTag(Command.SetRequest, (SetRequestType)type);
                    }
                    xml.AppendEndTag(Command.SetRequest);
                    return;
                }
            }
            catch (Exception ex)
            {
                if (xml != null)
                {
                    throw ex;
                }
                Debug.WriteLine("HandleGetRequest failed. " + ex.Message);
                data.Clear();
                settings.ResetBlockIndex();
                p.status = (byte)ErrorCode.ReadWriteDenied;
            }
            GXDLMS.GetLNPdu(p, replyData);
        }

        /// <summary>
        /// Handle method request next data block command.
        /// </summary>
        /// <param name="data">Received data.</param>
        internal static void MethodRequestNextDataBlock(
            GXDLMSSettings settings,
            byte invokeID,
            GXDLMSServer server,
            GXByteBuffer data,
            GXByteBuffer replyData,
            GXDLMSTranslatorStructure xml,
            bool streaming,
            Command cipheredCommand)
        {
            GXByteBuffer bb = new GXByteBuffer();
            byte lastBlock = 0;
            if (!streaming)
            {
                lastBlock = data.GetUInt8();
                // Get block index.
                UInt32 index = data.GetUInt32();
                if (xml != null)
                {
                    xml.AppendLine(TranslatorTags.BlockNumber, null, xml.IntegerToHex(index, 8));
                    return;
                }
                if (index != settings.BlockIndex)
                {
                    Debug.WriteLine("MethodRequestNextDataBlock failed. Invalid block number. " + settings.BlockIndex + "/" + index);
                    GXDLMS.GetLNPdu(new GXDLMSLNParameters(settings, 0, Command.MethodResponse, (byte)ActionResponseType.Normal, null,
                        bb, (byte)ErrorCode.DataBlockNumberInvalid, cipheredCommand), replyData);
                    return;
                }
            }
            settings.IncreaseBlockIndex();
            GXDLMSLNParameters p = new GXDLMSLNParameters(settings, invokeID,
                streaming ? Command.GeneralBlockTransfer : Command.MethodResponse, (byte)ActionResponseType.Normal,
                null, bb, (byte)ErrorCode.Ok, cipheredCommand);
            p.Streaming = settings.GbtWindowSize != 1;
            p.GbtWindowSize = settings.GbtWindowSize;
            //If transaction is not in progress.
            if (server.transaction == null)
            {
                p.status = (byte)ErrorCode.NoLongGetOrReadInProgress;
                p.requestType = 1;
                GXDLMS.GetLNPdu(p, replyData);
            }
            else
            {
                try
                {
                    if (data.Available != 0)
                    {
                        server.transaction.data.Set(data);
                    }
                    if (lastBlock == 1)
                    {
                        GXDataInfo info = new GXDataInfo();
                        object parameters = GXCommon.GetData(settings, server.transaction.data, info);
                        p.data.Clear();
                        object value;
                        foreach (ValueEventArgs arg in server.transaction.targets)
                        {
                            arg.InvokeId = p.InvokeId;
                            arg.Parameters = parameters;
                            server.NotifyPreAction(new ValueEventArgs[] { arg });
                            if (arg.Handled)
                            {
                                value = arg.Value;
                            }
                            else
                            {
                                value = (arg.Target as IGXDLMSBase).Invoke(settings, arg);
                            }
                            server.NotifyPostAction(new ValueEventArgs[] { arg });
                            p.InvokeId = arg.InvokeId;
                            // Set default action reply if not given.
                            if (value != null && arg.Error == ErrorCode.Ok)
                            {
                                // Add return parameters
                                bb.SetUInt8(1);
                                // Add parameters error code.
                                bb.SetUInt8(0);
                                if (arg.ByteArray)
                                {
                                    bb.Set((byte[])value);
                                }
                                else
                                {
                                    GXCommon.SetData(settings, bb, GXDLMSConverter.GetDLMSDataType(value), value);
                                }
                            }
                            else
                            {
                                p.requestType = 1;
                                // Add parameters error code.
                                p.status = (byte)arg.Error;
                                // Add return parameters
                                bb.SetUInt8(0);
                            }
                        }
                        server.transaction = null;
                        settings.ResetBlockIndex();
                    }
                    else
                    {
                        bb.Set(server.transaction.data);
                        // Ask next block.
                        p.requestType = (byte)ActionResponseType.NextBlock;
                        p.status = 0xFF;
                    }
                }
                catch (Exception)
                {
                    p.status = (byte)ErrorCode.InconsistentClass;
                    // Add return parameters
                    bb.SetUInt8(0);
                    server.transaction = null;
                    settings.ResetBlockIndex();
                }
                GXDLMS.GetLNPdu(p, replyData);
                if (p.status == 0xFF && p.multipleBlocks)
                {
                    settings.IncreaseBlockIndex();
                }
                if (bb.Size - bb.Position != 0)
                {
                    server.transaction.data = bb;
                }
                else
                {
                    server.transaction = null;
                    settings.ResetBlockIndex();
                }

            }
        }

        public static void MethodRequest(
            GXDLMSSettings settings,
            ActionRequestType type,
            byte invokeId,
            GXDLMSServer server,
            GXByteBuffer data,
            GXDLMSConnectionEventArgs connectionInfo,
            GXByteBuffer replyData,
            GXDLMSTranslatorStructure xml,
            Command cipheredCommand)
        {
            ErrorCode error = ErrorCode.Ok;
            GXByteBuffer bb = new GXByteBuffer();
            // CI
            ObjectType ci = (ObjectType)data.GetUInt16();
            byte[] ln = new byte[6];
            data.Get(ln);
            // Attribute Id
            byte id = data.GetUInt8();
            object parameters = null;
            GXDLMSLNParameters p = new GXDLMSLNParameters(settings, invokeId, Command.MethodResponse,
                (byte)ActionResponseType.Normal,
                null, bb, 0, cipheredCommand);
            if (type == ActionRequestType.Normal)
            {
                // Get parameters.
                byte selection = data.GetUInt8();
                if (xml != null)
                {
                    AppendMethodDescriptor(xml, (int)ci, ln, id);
                    if (selection != 0)
                    {
                        //MethodInvocationParameters
                        xml.AppendStartTag(TranslatorTags.MethodInvocationParameters);
                        GXDataInfo di = new GXDataInfo();
                        di.xml = xml;
                        GXCommon.GetData(settings, data, di);
                        xml.AppendEndTag(TranslatorTags.MethodInvocationParameters);
                    }
                    return;
                }
                if (selection != 0)
                {
                    GXDataInfo info = new GXDataInfo();
                    parameters = GXCommon.GetData(settings, data, info);
                }
            }
            else if (type == ActionRequestType.WithFirstBlock)
            {
                p.requestType = (byte)ActionResponseType.NextBlock;
                p.status = 0xFF;
                byte lastBlock = data.GetUInt8();
                p.multipleBlocks = lastBlock == 0;
                UInt32 blockNumber = data.GetUInt32();
                if (xml == null && blockNumber != settings.BlockIndex)
                {
                    Debug.WriteLine("MethodRequest failed. Invalid block number. " + settings.BlockIndex + "/" + blockNumber);
                    p.status = (byte)ErrorCode.DataBlockNumberInvalid;
                    return;
                }
                settings.IncreaseBlockIndex();
                int size = GXCommon.GetObjectCount(data);
                int realSize = data.Size - data.Position;
                if (size != realSize)
                {
                    if (xml == null)
                    {
                        Debug.WriteLine("MethodRequest failed. Invalid block size.");
                        p.status = (byte)ErrorCode.DataBlockUnavailable;
                        return;
                    }
                    xml.AppendComment("Invalid block size.");
                }
                if (xml != null)
                {
                    AppendMethodDescriptor(xml, (int)ci, ln, id);
                    xml.AppendStartTag(TranslatorTags.DataBlock);
                    xml.AppendLine(TranslatorTags.LastBlock, "Value", xml.IntegerToHex(lastBlock, 2));
                    xml.AppendLine(TranslatorTags.BlockNumber, "Value", xml.IntegerToHex(blockNumber, 8));
                    xml.AppendLine(TranslatorTags.RawData, "Value", data.RemainingHexString(false));
                    xml.AppendEndTag(TranslatorTags.DataBlock);
                    return;
                }
            }

            GXDLMSObject obj = null;
            if (ci == ObjectType.AssociationLogicalName && GXCommon.ToLogicalName(ln) == "0.0.40.0.0.255")
            {
                obj = settings.AssignedAssociation;
            }
            if (obj == null)
            {
                obj = settings.Objects.FindByLN(ci, GXCommon.ToLogicalName(ln));
            }
            if ((settings.Connected & ConnectionState.Dlms) == 0 && cipheredCommand == Command.None && (ci != ObjectType.AssociationLogicalName || id != 1))
            {
                replyData.Set(GXDLMSServer.GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                              ServiceError.Service, (byte)Service.Unsupported));
                return;
            }
            ValueEventArgs e = null;
            if (obj == null)
            {
                obj = server.NotifyFindObject(ci, 0, GXCommon.ToLogicalName(ln));
            }
            if (obj == null)
            {
                // Device reports a undefined object.
                error = ErrorCode.UndefinedObject;
            }
            else
            {
                if (settings.AssignedAssociation != null)
                {
                    p.AccessMode = (int)settings.AssignedAssociation.GetMethodAccess3(obj, id);
                }
                e = new ValueEventArgs(server, obj, id, 0, parameters);
                e.InvokeId = invokeId;
                if ((server.NotifyGetMethodAccess(e) & (int)MethodAccessMode.Access) == 0)
                {
                    error = ErrorCode.ReadWriteDenied;
                }
                else
                {
                    try
                    {
                        if (p.multipleBlocks)
                        {
                            server.transaction = new GXDLMSLongTransaction(new ValueEventArgs[] { e }, Command.MethodRequest, data);
                        }
                        //Check transaction so invoke is not called multiple times.
                        //This might happen when all data can't fit to one PDU.
                        else if (server.transaction == null)
                        {
                            p.requestType = (byte)ActionResponseType.Normal;
                            server.NotifyPreAction(new ValueEventArgs[] { e });
                            byte[] actionReply;
                            if (e.Handled)
                            {
                                actionReply = (byte[])e.Value;
                            }
                            else
                            {
                                actionReply = (obj as IGXDLMSBase).Invoke(settings, e);
                                server.NotifyPostAction(new ValueEventArgs[] { e });
                            }
                            //Set default action reply if not given.
                            if (actionReply != null && e.Error == 0)
                            {
                                //Add return parameters
                                bb.SetUInt8(1);
                                //Add parameters error code.
                                bb.SetUInt8(0);
                                GXCommon.SetData(settings, bb, GXDLMSConverter.GetDLMSDataType(actionReply), actionReply);
                            }
                            else
                            {
                                error = e.Error;
                                //Add return parameters
                                bb.SetUInt8(0);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        error = ErrorCode.ReadWriteDenied;
                        //Add return parameters
                        bb.SetUInt8(0);
                    }
                }
                p.InvokeId = (byte)e.InvokeId;
            }
            if (error != 0)
            {
                p.status = (byte)error;
            }
            GXDLMS.GetLNPdu(p, replyData);
            //If all reply data doesn't fit to one PDU.
            if (server.transaction == null && p.data.Available != 0)
            {
                server.transaction = new GXDLMSLongTransaction(new ValueEventArgs[] { e }, Command.MethodResponse, p.data);
            }
            //If High level authentication fails.
            if (error == 0 && obj is GXDLMSAssociationLogicalName && id == 1)
            {
                if ((obj as GXDLMSAssociationLogicalName).AssociationStatus == Objects.Enums.AssociationStatus.Associated)
                {
                    server.NotifyConnected(connectionInfo);
                    settings.Connected |= ConnectionState.Dlms;
                }
                else
                {
                    server.NotifyInvalidConnection(connectionInfo);
                    settings.Connected &= ~ConnectionState.Dlms;
                }
            }
            //Start to use new keys.
            if (e != null && error == 0 && obj is GXDLMSSecuritySetup && id == 2)
            {
                ((GXDLMSSecuritySetup)obj).ApplyKeys(settings, e);
            }
        }

        /// <summary>
        /// Handle method request next data block command.
        /// </summary>
        /// <param name="data">Received data.</param>
        internal static void MethodRequestNextDataBlock(GXDLMSSettings settings, GXDLMSServer server, GXByteBuffer data, byte invokeID, GXByteBuffer replyData, GXDLMSTranslatorStructure xml, bool streaming, Command cipheredCommand)
        {
            GXByteBuffer bb = new GXByteBuffer();
            if (!streaming)
            {
                UInt32 index;
                // Get block index.
                index = data.GetUInt32();
                if (xml != null)
                {
                    xml.AppendLine(TranslatorTags.BlockNumber, null, xml.IntegerToHex(index, 8));
                    return;
                }
                if (index != settings.BlockIndex)
                {
                    Debug.WriteLine("handleGetRequest failed. Invalid block number. " + settings.BlockIndex + "/" + index);
                    GXDLMS.GetLNPdu(new GXDLMSLNParameters(settings, 0, Command.MethodResponse, 1, null, bb, (byte)ErrorCode.DataBlockNumberInvalid, cipheredCommand), replyData);
                    return;
                }
            }
            settings.IncreaseBlockIndex();
            GXDLMSLNParameters p = new GXDLMSLNParameters(settings, invokeID, streaming ? Command.GeneralBlockTransfer : Command.MethodResponse, (byte)ActionResponseType.WithBlock, null, bb, (byte)ErrorCode.Ok, cipheredCommand);
            p.Streaming = streaming;
            p.GbtWindowSize = settings.GbtWindowSize;
            //If transaction is not in progress.
            if (server.transaction == null)
            {
                p.status = (byte)ErrorCode.NoLongGetOrReadInProgress;
                p.requestType = 1;
                GXDLMS.GetLNPdu(p, replyData);
            }
            else
            {
                bb.Set(server.transaction.data);
                bool moreData = settings.Index != settings.Count;
                if (moreData)
                {
                    //If there is multiple blocks on the buffer.
                    //This might happen when Max PDU size is very small.
                    if (bb.Size < settings.MaxPduSize)
                    {
                        foreach (ValueEventArgs arg in server.transaction.targets)
                        {
                            object value;
                            server.NotifyPreAction(new ValueEventArgs[] { arg });
                            if (arg.Handled)
                            {
                                value = arg.Value;
                            }
                            else
                            {
                                value = (arg.Target as IGXDLMSBase).Invoke(settings, arg);
                            }
                            //Set default action reply if not given.
                            if (value != null && arg.Error == 0)
                            {
                                //Add return parameters
                                bb.SetUInt8(1);
                                //Add parameters error code.
                                bb.SetUInt8(0);
                                GXCommon.SetData(settings, bb, GXDLMSConverter.GetDLMSDataType(value), value);
                            }
                            else
                            {
                                p.requestType = 1;
                                p.status = (byte)arg.Error;
                                //Add return parameters
                                bb.SetUInt8(0);
                            }
                        }
                        moreData = settings.Index != settings.Count;
                    }
                }
                p.multipleBlocks = true;
                GXDLMS.GetLNPdu(p, replyData);
                if (moreData || bb.Size != bb.Position)
                {
                    server.transaction.data = bb;
                }
                else
                {
                    server.transaction = null;
                    settings.ResetBlockIndex();
                }
            }
        }

        public static void MethodRequestNextBlock(
            GXDLMSSettings settings,
            GXDLMSServer server,
            GXByteBuffer data,
            GXDLMSConnectionEventArgs connectionInfo,
            GXByteBuffer replyData,
            GXDLMSTranslatorStructure xml,
            bool streaming,
            Command cipheredCommand)
        {
            ValueEventArgs e = null;
            GXByteBuffer bb = new GXByteBuffer();
            byte lastBlock = data.GetUInt8();
            if (!streaming)
            {
                UInt32 blockNumber;
                // Get block index.
                blockNumber = data.GetUInt32();
                //Get data size.
                int size = GXCommon.GetObjectCount(data);
                if (xml != null)
                {
                    xml.AppendStartTag(TranslatorTags.DataBlock);
                    xml.AppendLine(TranslatorTags.LastBlock, null, xml.IntegerToHex(lastBlock, 2));
                    xml.AppendLine(TranslatorTags.BlockNumber, null, xml.IntegerToHex(blockNumber, 8));
                    xml.AppendLine(TranslatorTags.RawData, null, data.RemainingHexString(false));
                    xml.AppendEndTag(TranslatorTags.DataBlock);
                    return;
                }
                if (blockNumber != settings.BlockIndex)
                {
                    server.transaction = null;
                    Debug.WriteLine("MethodRequestNextBlock failed. Invalid block number. " + settings.BlockIndex + "/" + blockNumber);
                    settings.ResetBlockIndex();
                    GXDLMS.GetLNPdu(new GXDLMSLNParameters(settings, 0, Command.MethodResponse,
                        (byte)ActionResponseType.Normal, null, bb, (byte)ErrorCode.DataBlockNumberInvalid, cipheredCommand), replyData);
                    return;
                }
                if (size < data.Available)
                {
                    server.transaction = null;
                    settings.ResetBlockIndex();
                    Debug.WriteLine("MethodRequestNextBlock failed. Not enought data. Actual: " + data.Available + ". Expected" + size);
                    GXDLMS.GetLNPdu(new GXDLMSLNParameters(settings, 0, Command.MethodResponse,
                        (byte)ActionResponseType.Normal, null, bb, (byte)ErrorCode.DataBlockNumberInvalid, cipheredCommand), replyData);
                    return;
                }
            }
            GXDLMSLNParameters p = new GXDLMSLNParameters(settings, 0, streaming ? Command.GeneralBlockTransfer : Command.MethodResponse,
                (byte)ActionResponseType.Normal, null, bb, (byte)ErrorCode.Ok, cipheredCommand);
            p.multipleBlocks = lastBlock == 0;
            p.Streaming = streaming;
            p.GbtWindowSize = settings.GbtWindowSize;
            //If transaction is not in progress.
            if (server.transaction == null)
            {
                p.status = (byte)ErrorCode.NoLongGetOrReadInProgress;
            }
            else
            {
                try
                {
                    server.transaction.data.Set(data);
                    if (lastBlock == 1)
                    {
                        GXDataInfo info = new GXDataInfo();
                        object parameters = GXCommon.GetData(settings, server.transaction.data, info);
                        foreach (ValueEventArgs arg in server.transaction.targets)
                        {
                            object value;
                            arg.Parameters = parameters;
                            ValueEventArgs[] args = new ValueEventArgs[] { arg };
                            server.NotifyPreAction(args);
                            if (arg.Handled)
                            {
                                value = arg.Value;
                            }
                            else
                            {
                                value = (arg.Target as IGXDLMSBase).Invoke(settings, arg);
                            }
                            server.NotifyPostAction(args);
                            //Set default action reply if not given.
                            if (value != null && arg.Error == 0)
                            {
                                //If High level authentication fails.
                                if (p.status == 0 && arg.Target is GXDLMSAssociationLogicalName && arg.Index == 1)
                                {
                                    if ((arg.Target as GXDLMSAssociationLogicalName).AssociationStatus == Objects.Enums.AssociationStatus.Associated)
                                    {
                                        server.NotifyConnected(connectionInfo);
                                        settings.Connected |= ConnectionState.Dlms;
                                    }
                                    else
                                    {
                                        server.NotifyInvalidConnection(connectionInfo);
                                        settings.Connected &= ~ConnectionState.Dlms;
                                    }
                                }
                                //Start to use new keys.
                                if (e != null && arg.Target is GXDLMSSecuritySetup && arg.Index == 2)
                                {
                                    ((GXDLMSSecuritySetup)arg.Target).ApplyKeys(settings, e);
                                }
                                //Add return parameters
                                bb.SetUInt8(1);
                                //Add parameters error code.
                                bb.SetUInt8(0);
                                GXCommon.SetData(settings, bb, GXDLMSConverter.GetDLMSDataType(value), value);
                            }
                            else
                            {
                                p.status = (byte)arg.Error;
                                //Add return parameters
                                bb.SetUInt8(0);
                            }
                        }
                        server.transaction = null;
                        settings.ResetBlockIndex();
                        p.blockIndex = 1;
                    }
                    else
                    {
                        //Ask next block.
                        p.requestType = (byte)ActionResponseType.NextBlock;
                        p.status = 0xFF;
                    }
                }
                catch (Exception)
                {
                    p.status = (byte)ErrorCode.InconsistentClass;
                    //Add return parameters
                    bb.SetUInt8(0);
                    server.transaction = null;
                    settings.ResetBlockIndex();
                }
                GXDLMS.GetLNPdu(p, replyData);
                if (settings.Count != settings.Index || bb.Size != bb.Position)
                {
                    server.transaction = new GXDLMSLongTransaction(new ValueEventArgs[] { e }, Command.MethodRequest, bb);
                }
                if (lastBlock == 0)
                {
                    settings.IncreaseBlockIndex();
                }
            }
        }

        ///<summary>
        /// Handle action request.
        ///</summary>
        public static void HandleMethodRequest(GXDLMSSettings settings,
            GXDLMSServer server,
            GXByteBuffer data,
            GXDLMSConnectionEventArgs connectionInfo,
            GXByteBuffer replyData,
            GXDLMSTranslatorStructure xml,
            Command cipheredCommand)
        {
            // Get type.
            byte invokeID;
            ActionRequestType type = (ActionRequestType)data.GetUInt8();
            // Get invoke ID and priority.
            invokeID = data.GetUInt8();
            settings.UpdateInvokeId(invokeID);
            if (xml != null)
            {
                if (type > 0 && type <= ActionRequestType.WithBlock)
                {
                    GXDLMS.AddInvokeId(xml, Command.MethodRequest, type, invokeID);
                }
                else
                {
                    xml.AppendStartTag(Command.MethodRequest);
                    xml.AppendComment("Unknown tag: " + type);
                    xml.AppendLine(TranslatorTags.InvokeId, "Value", xml.IntegerToHex(invokeID, 2));
                }
            }
            switch (type)
            {
                case ActionRequestType.Normal:
                case ActionRequestType.WithFirstBlock:
                    MethodRequest(settings, type, invokeID, server, data, connectionInfo, replyData, xml, cipheredCommand);
                    break;
                case ActionRequestType.NextBlock:
                    MethodRequestNextDataBlock(settings, server, data, invokeID, replyData, xml, false, cipheredCommand);
                    break;
                case ActionRequestType.WithBlock:
                    MethodRequestNextBlock(settings, server, data, connectionInfo, replyData, xml, false, cipheredCommand);
                    break;
                default:
                    if (xml == null)
                    {
                        Debug.WriteLine("HandleMethodRequest failed. Invalid command type.");
                        settings.ResetBlockIndex();
                        type = ActionRequestType.Normal;
                        data.Clear();
                        GXDLMS.GetLNPdu(new GXDLMSLNParameters(settings, invokeID, Command.MethodResponse, (byte)type, null, null, (byte)ErrorCode.ReadWriteDenied, cipheredCommand), replyData);
                    }
                    break;
            }
            if (xml != null)
            {
                if (type > 0 && type <= ActionRequestType.WithBlock)
                {
                    xml.AppendEndTag(Command.MethodRequest, type);
                }
                else
                {
                    xml.AppendEndTag(Command.MethodRequest);
                }
                xml.AppendEndTag(Command.MethodRequest);
            }
        }

        private static void AppendAttributeDescriptor(GXDLMSTranslatorStructure xml, int ci, byte[] ln, byte attributeIndex)
        {
            xml.AppendStartTag(TranslatorTags.AttributeDescriptor);
            if (xml.Comments)
            {
                xml.AppendComment(((ObjectType)ci).ToString());
            }
            xml.AppendLine(TranslatorTags.ClassId, "Value", xml.IntegerToHex((int)ci, 4));
            xml.AppendComment(GXCommon.ToLogicalName(ln));
            xml.AppendLine(TranslatorTags.InstanceId, "Value", GXCommon.ToHex(ln, false));
            try
            {
                IGXDLMSBase obj = GXDLMSClient.CreateObject((ObjectType)ci) as IGXDLMSBase;
                if (obj != null)
                {
                    xml.AppendComment(obj.GetNames()[attributeIndex - 1]);
                }
            }
            catch (Exception)
            {
                //It's OK if this fails.
            }
            xml.AppendLine(TranslatorTags.AttributeId, "Value", xml.IntegerToHex(attributeIndex, 2));
            xml.AppendEndTag(TranslatorTags.AttributeDescriptor);
        }

        private static void AppendMethodDescriptor(GXDLMSTranslatorStructure xml, int ci, byte[] ln, byte attributeIndex)
        {
            xml.AppendStartTag(TranslatorTags.MethodDescriptor);
            if (xml.Comments)
            {
                xml.AppendComment(((ObjectType)ci).ToString());
            }
            xml.AppendLine(TranslatorTags.ClassId, "Value", xml.IntegerToHex((int)ci, 4));
            xml.AppendComment(GXCommon.ToLogicalName(ln));
            xml.AppendLine(TranslatorTags.InstanceId, "Value", GXCommon.ToHex(ln, false));
            try
            {
                IGXDLMSBase obj = GXDLMSClient.CreateObject((ObjectType)ci) as IGXDLMSBase;
                if (obj != null)
                {
                    ((GXDLMSObject)obj).Version = obj.GetMaxSupportedVersion();
                    xml.AppendComment(obj.GetMethodNames()[attributeIndex - 1]);
                }
            }
            catch (Exception)
            {
                //It's OK if this fails.
            }
            xml.AppendLine(TranslatorTags.MethodId, "Value", xml.IntegerToHex(attributeIndex, 2));
            xml.AppendEndTag(TranslatorTags.MethodDescriptor);
        }

        /// <summary>
        /// Handle get request normal command.
        /// </summary>
        /// <param name="data">Received data.</param>
        private static void GetRequestNormal(GXDLMSSettings settings, byte invokeID, GXDLMSServer server, GXByteBuffer data, GXByteBuffer replyData, GXDLMSTranslatorStructure xml, Command cipheredCommand)
        {
            ValueEventArgs e = null;
            GXByteBuffer bb = new GXByteBuffer();
            // Get type.
            ErrorCode status = ErrorCode.Ok;

            settings.Count = settings.Index = 0;
            settings.ResetBlockIndex();
            // CI
            ObjectType ci = (ObjectType)data.GetUInt16();
            if (data.Available < 6)
            {
                if (xml != null)
                {
                    xml.AppendComment("Logical name is missing.");
                    xml.AppendComment("Attribute Id is missing.");
                    xml.AppendComment("Access Selection is missing.");
                    return;
                }
                throw new ArgumentException("Get request is not complete.");
            }
            byte[] ln = new byte[6];
            data.Get(ln);
            // Attribute Id
            byte attributeIndex = data.GetUInt8();

            // AccessSelection
            byte selection = data.GetUInt8();
            byte selector = 0;
            object parameters = null;
            GXDataInfo info = new GXDataInfo();
            if (selection != 0)
            {
                selector = data.GetUInt8();
            }
            if (xml != null)
            {
                AppendAttributeDescriptor(xml, (int)ci, ln, attributeIndex);
                if (selection != 0)
                {
                    info.xml = xml;
                    xml.AppendStartTag(TranslatorTags.AccessSelection);
                    xml.AppendLine(TranslatorTags.AccessSelector, "Value", xml.IntegerToHex(selector, 2));
                    xml.AppendStartTag(TranslatorTags.AccessParameters);
                    GXCommon.GetData(settings, data, info);
                    xml.AppendEndTag(TranslatorTags.AccessParameters);
                    xml.AppendEndTag(TranslatorTags.AccessSelection);
                }
                return;
            }
            if (selection != 0)
            {
                parameters = GXCommon.GetData(settings, data, info);
            }

            GXDLMSObject obj = null;
            if (ci == ObjectType.AssociationLogicalName && GXCommon.ToLogicalName(ln) == "0.0.40.0.0.255")
            {
                obj = settings.AssignedAssociation;
            }
            if (obj == null)
            {
                obj = settings.Objects.FindByLN(ci, GXCommon.ToLogicalName(ln));
            }
            if (obj == null)
            {
                obj = server.NotifyFindObject(ci, 0, GXCommon.ToLogicalName(ln));
            }
            e = new ValueEventArgs(server, obj, attributeIndex, selector, parameters);
            e.InvokeId = invokeID;
            int mode = 0;
            if (obj == null)
            {
                // "Access Error : Device reports a undefined object."
                status = ErrorCode.UndefinedObject;
            }
            else
            {
                int access = server.NotifyGetAttributeAccess(e);
                if ((access & (int)AccessMode.Read) == 0 && (access & (int)AccessMode.AuthenticatedRead) == 0)
                {
                    //Read Write denied.
                    status = ErrorCode.ReadWriteDenied;
                }
                else
                {
                    if (settings.AssignedAssociation != null)
                    {
                        mode = (int)settings.AssignedAssociation.GetAccess3(obj, attributeIndex);
                    }
                    // Handle default Association LN read as a special case.
                    if ((obj is GXDLMSAssociationLogicalName
                            || obj is GXDLMSAssociationShortName) && attributeIndex == 1)
                    {
                        GXDLMS.AppendData(settings, obj, attributeIndex, bb, new byte[] { 0, 0, 40, 0, 0, 255 });
                    }
                    else
                    {
                        if (e.Target is GXDLMSProfileGeneric && attributeIndex == 2)
                        {
                            e.RowToPdu = GXDLMS.RowsToPdu(settings, (GXDLMSProfileGeneric)e.Target);
                        }
                        object value;
                        server.NotifyRead(new ValueEventArgs[] { e });
                        if (e.Handled)
                        {
                            value = e.Value;
                        }
                        else
                        {
                            settings.Count = e.RowEndIndex - e.RowBeginIndex;
                            value = (obj as IGXDLMSBase).GetValue(settings, e);
                        }
                        if (e.ByteArray)
                        {
                            bb.Set((byte[])value);
                        }
                        else
                        {
                            GXDLMS.AppendData(settings, obj, attributeIndex, bb, value);
                        }
                        server.NotifyPostRead(new ValueEventArgs[] { e });
                        status = e.Error;
                    }
                }
            }
            GXDLMSLNParameters p = new GXDLMSLNParameters(settings, e.InvokeId, Command.GetResponse, 1, null, bb, (byte)status, cipheredCommand);
            p.AccessMode = mode;
            GXDLMS.GetLNPdu(p, replyData);
            if (settings.Count != settings.Index || bb.Size != bb.Position)
            {
                server.transaction = new GXDLMSLongTransaction(new ValueEventArgs[] { e }, Command.GetRequest, bb);
            }
        }

        /// <summary>
        /// Handle get request next data block command.
        /// </summary>
        /// <param name="data">Received data.</param>
        internal static void GetRequestNextDataBlock(GXDLMSSettings settings, byte invokeID, GXDLMSServer server, GXByteBuffer data, GXByteBuffer replyData, GXDLMSTranslatorStructure xml, bool streaming, Command cipheredCommand)
        {
            GXByteBuffer bb = new GXByteBuffer();
            if (!streaming)
            {
                UInt32 index;
                // Get block index.
                index = data.GetUInt32();
                if (xml != null)
                {
                    xml.AppendLine(TranslatorTags.BlockNumber, null, xml.IntegerToHex(index, 8));
                    return;
                }
                if (index != settings.BlockIndex)
                {
                    Debug.WriteLine("handleGetRequest failed. Invalid block number. " + settings.BlockIndex + "/" + index);
                    GXDLMS.GetLNPdu(new GXDLMSLNParameters(settings, 0, Command.GetResponse, 2, null, bb, (byte)ErrorCode.DataBlockNumberInvalid, cipheredCommand), replyData);
                    return;
                }
            }
            settings.IncreaseBlockIndex();
            GXDLMSLNParameters p = new GXDLMSLNParameters(settings, invokeID, streaming ? Command.GeneralBlockTransfer : Command.GetResponse, 2, null, bb, (byte)ErrorCode.Ok, cipheredCommand);
            p.Streaming = settings.GbtWindowSize != 1;
            p.GbtWindowSize = settings.GbtWindowSize;
            //If transaction is not in progress.
            if (server.transaction == null)
            {
                p.status = (byte)ErrorCode.NoLongGetOrReadInProgress;
            }
            else
            {
                bb.Set(server.transaction.data);
                bool moreData = settings.Index != settings.Count;
                if (moreData)
                {
                    //If there is multiple blocks on the buffer.
                    //This might happen when Max PDU size is very small.
                    if (bb.Size < settings.MaxPduSize)
                    {
                        foreach (ValueEventArgs arg in server.transaction.targets)
                        {
                            object value;
                            server.NotifyRead(new ValueEventArgs[] { arg });
                            if (arg.Handled)
                            {
                                value = arg.Value;
                            }
                            else
                            {
                                value = (arg.Target as IGXDLMSBase).GetValue(settings, arg);
                            }
                            //Add data.
                            if (arg.ByteArray)
                            {
                                bb.Set((byte[])value);
                            }
                            else
                            {
                                GXDLMS.AppendData(settings, arg.Target, arg.Index, bb, value);
                            }
                        }
                        moreData = settings.Index != settings.Count;
                    }
                }
                p.multipleBlocks = true;
                GXDLMS.GetLNPdu(p, replyData);
                if (moreData || bb.Size - bb.Position != 0)
                {
                    server.transaction.data = bb;
                }
                else
                {
                    server.transaction = null;
                    settings.ResetBlockIndex();
                }
            }
        }

        /// <summary>
        /// Handle get request with list command.
        /// </summary>
        /// <param name="data">Received data.</param>
        private static void GetRequestWithList(GXDLMSSettings settings, byte invokeID, GXDLMSServer server, GXByteBuffer data, GXByteBuffer replyData, GXDLMSTranslatorStructure xml, Command cipheredCommand)
        {
            ValueEventArgs e;
            GXByteBuffer bb = new GXByteBuffer();
            int pos;
            int cnt = GXCommon.GetObjectCount(data);
            GXCommon.SetObjectCount(cnt, bb);
            List<ValueEventArgs> list = new List<ValueEventArgs>();
            if (xml != null)
            {
                xml.AppendStartTag(TranslatorTags.AttributeDescriptorList, "Qty", xml.IntegerToHex(cnt, 2));
            }
            try
            {
                for (pos = 0; pos != cnt; ++pos)
                {
                    ObjectType ci = (ObjectType)data.GetUInt16();
                    byte[] ln = new byte[6];
                    data.Get(ln);
                    short attributeIndex = data.GetUInt8();
                    // AccessSelection
                    int selection = data.GetUInt8();
                    int selector = 0;
                    object parameters = null;
                    if (selection != 0)
                    {
                        selector = data.GetUInt8();
                        GXDataInfo info = new GXDataInfo();
                        parameters = GXCommon.GetData(settings, data, info);
                    }
                    if (xml != null)
                    {
                        xml.AppendStartTag(TranslatorTags.AttributeDescriptorWithSelection);
                        xml.AppendStartTag(TranslatorTags.AttributeDescriptor);
                        xml.AppendComment(ci.ToString());
                        xml.AppendLine(TranslatorTags.ClassId, "Value", xml.IntegerToHex((int)ci, 4));
                        xml.AppendComment(GXCommon.ToLogicalName(ln));
                        xml.AppendLine(TranslatorTags.InstanceId, "Value", GXCommon.ToHex(ln, false));
                        xml.AppendLine(TranslatorTags.AttributeId, "Value", xml.IntegerToHex(attributeIndex, 2));
                        xml.AppendEndTag(TranslatorTags.AttributeDescriptor);
                        xml.AppendEndTag(TranslatorTags.AttributeDescriptorWithSelection);
                    }
                    else
                    {
                        GXDLMSObject obj = null;
                        if (ci == ObjectType.AssociationLogicalName && GXCommon.ToLogicalName(ln) == "0.0.40.0.0.255")
                        {
                            obj = settings.AssignedAssociation;
                        }
                        if (obj == null)
                        {
                            obj = settings.Objects.FindByLN(ci, GXCommon.ToLogicalName(ln));
                        }
                        if (obj == null)
                        {
                            obj = server.NotifyFindObject(ci, 0, GXCommon.ToLogicalName(ln));
                        }
                        if (obj == null)
                        {
                            // "Access Error : Device reports a undefined object."
                            e = new ValueEventArgs(server, obj, attributeIndex, 0, 0);
                            e.Error = ErrorCode.UndefinedObject;
                            list.Add(e);
                        }
                        else
                        {
                            ValueEventArgs arg = new ValueEventArgs(server, obj, attributeIndex, selector, parameters);
                            arg.InvokeId = invokeID;
                            int access = server.NotifyGetAttributeAccess(arg);
                            if ((access & (int)AccessMode.Read) == 0 && (access & (int)AccessMode.AuthenticatedRead) == 0)
                            {
                                //Read Write denied.
                                arg.Error = ErrorCode.ReadWriteDenied;
                                list.Add(arg);
                            }
                            else
                            {
                                list.Add(arg);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (xml == null)
                {
                    throw ex;
                }
            }
            if (xml != null)
            {
                xml.AppendEndTag(TranslatorTags.AttributeDescriptorList);
                return;
            }

            server.NotifyRead(list.ToArray());
            object value;
            pos = 0;
            foreach (ValueEventArgs it in list)
            {
                try
                {
                    if (it.Handled)
                    {
                        value = it.Value;
                    }
                    else
                    {
                        value = (it.Target as IGXDLMSBase).GetValue(settings, it);
                    }
                    bb.SetUInt8(it.Error);
                    if (it.ByteArray)
                    {
                        bb.Set((byte[])value);
                    }
                    else
                    {
                        GXDLMS.AppendData(settings, it.Target, it.Index, bb, value);
                    }
                    invokeID = (byte)it.InvokeId;
                }
                catch (Exception)
                {
                    bb.SetUInt8((byte)ErrorCode.HardwareFault);
                }
                ++pos;
            }
            server.NotifyPostRead(list.ToArray());
            GXDLMSLNParameters p = new GXDLMSLNParameters(settings, invokeID, Command.GetResponse, 3, null, bb, 0xFF, cipheredCommand);
            GXDLMS.GetLNPdu(p, replyData);
            if (settings.Index != settings.Count || bb.Available != 0)
            {
                server.transaction = new GXDLMSLongTransaction(list.ToArray(), Command.GetRequest, bb);
            }
        }


        private static void HandleSetRequestNormal(GXDLMSSettings settings, GXDLMSServer server, GXByteBuffer data, byte type, GXDLMSLNParameters p, GXByteBuffer replyData, GXDLMSTranslatorStructure xml)
        {
            object value = null;
            GXDataInfo reply = new GXDataInfo();
            // CI
            ObjectType ci = (ObjectType)data.GetUInt16();
            if (data.Available < 8)
            {
                if (xml != null)
                {
                    xml.AppendComment("Logical name is missing.");
                    xml.AppendComment("Attribute Id is missing.");
                    xml.AppendComment("Access Selection is missing.");
                    return;
                }
                throw new ArgumentException("Set request is not complete.");
            }
            byte[] ln = new byte[6];
            data.Get(ln);
            // Attribute index.
            byte index = data.GetUInt8();
            // Get Access Selection.
            data.GetUInt8();
            if (type == 2)
            {
                byte lastBlock = data.GetUInt8();
                p.multipleBlocks = lastBlock == 0;
                UInt32 blockNumber = data.GetUInt32();
                if (blockNumber != settings.BlockIndex)
                {
                    Debug.WriteLine("HandleSetRequest failed. Invalid block number. " + settings.BlockIndex + "/" + blockNumber);
                    p.status = (byte)ErrorCode.DataBlockNumberInvalid;
                    return;
                }
                settings.IncreaseBlockIndex();
                int size = GXCommon.GetObjectCount(data);
                int realSize = data.Size - data.Position;
                if (size != realSize)
                {
                    Debug.WriteLine("HandleSetRequest failed. Invalid block size.");
                    p.status = (byte)ErrorCode.DataBlockUnavailable;
                    return;
                }
                if (xml != null)
                {
                    AppendAttributeDescriptor(xml, (int)ci, ln, index);
                    xml.AppendStartTag(TranslatorTags.DataBlock);
                    xml.AppendLine(TranslatorTags.LastBlock, "Value", xml.IntegerToHex(lastBlock, 2));
                    xml.AppendLine(TranslatorTags.BlockNumber, "Value", xml.IntegerToHex(blockNumber, 8));
                    xml.AppendLine(TranslatorTags.RawData, "Value", data.RemainingHexString(false));
                    xml.AppendEndTag(TranslatorTags.DataBlock);
                }
            }
            if (xml != null)
            {
                AppendAttributeDescriptor(xml, (int)ci, ln, index);
                xml.AppendStartTag(TranslatorTags.Value);
                GXDataInfo di = new GXDataInfo();
                di.xml = xml;
                value = GXCommon.GetData(settings, data, di);
                if (!di.Complete)
                {
                    GXCommon.ToHex(data.Data, false, data.Position, data.Size - data.Position);
                }
                else if (value is byte[])
                {
                    GXCommon.ToHex((byte[])value, false);
                }
                xml.AppendEndTag(TranslatorTags.Value);
                return;
            }

            if (!p.multipleBlocks)
            {
                settings.ResetBlockIndex();
                value = GXCommon.GetData(settings, data, reply);
            }

            GXDLMSObject obj = null;
            if (ci == ObjectType.AssociationLogicalName && GXCommon.ToLogicalName(ln) == "0.0.40.0.0.255")
            {
                obj = settings.AssignedAssociation;
            }
            if (obj == null)
            {
                obj = settings.Objects.FindByLN(ci, GXCommon.ToLogicalName(ln));
            }
            if (obj == null)
            {
                obj = server.NotifyFindObject(ci, 0, GXCommon.ToLogicalName(ln));
            }
            // If target is unknown.
            if (obj == null)
            {
                // Device reports a undefined object.
                p.status = (byte)ErrorCode.UndefinedObject;
            }
            else
            {
                ValueEventArgs e = new ValueEventArgs(server, obj, index, 0, null);
                e.InvokeId = p.InvokeId;
                int am = server.NotifyGetAttributeAccess(e);
                // If write is denied.
                if ((am & (int)AccessMode.Write) == 0)
                {
                    //Read Write denied.
                    p.status = (byte)ErrorCode.ReadWriteDenied;
                }
                else
                {
                    try
                    {
                        if (value is byte[])
                        {
                            DataType dt = (obj as IGXDLMSBase).GetDataType(index);
                            if (dt != DataType.None && dt != DataType.OctetString && dt != DataType.Structure)
                            {
                                value = GXDLMSClient.ChangeType((byte[])value, dt, settings.UseUtc2NormalTime);
                            }
                        }
                        e.Value = value;
                        ValueEventArgs[] list = new ValueEventArgs[] { e };
                        if (p.multipleBlocks)
                        {
                            server.transaction = new GXDLMSLongTransaction(list, Command.GetRequest, data);
                        }
                        server.NotifyWrite(list);
                        if (e.Error != 0)
                        {
                            p.status = (byte)e.Error;
                        }
                        else if (!e.Handled && !p.multipleBlocks)
                        {
                            (obj as IGXDLMSBase).SetValue(settings, e);
                            server.NotifyPostWrite(list);
                            if (e.Error != 0)
                            {
                                p.status = (byte)e.Error;
                            }
                        }
                        p.InvokeId = e.InvokeId;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        p.status = (byte)ErrorCode.InconsistentClass;
                    }
                    catch (Exception)
                    {
                        p.status = (byte)ErrorCode.UnmatchedType;
                    }
                }
            }
        }

        private static void HanleSetRequestWithDataBlock(GXDLMSSettings settings, GXDLMSServer server, GXByteBuffer data, GXDLMSLNParameters p, GXByteBuffer replyData, GXDLMSTranslatorStructure xml)
        {
            byte ret = 0;
            GXDataInfo reply = new GXDataInfo();
            reply.xml = xml;
            byte lastBlock = data.GetUInt8();
            p.multipleBlocks = lastBlock == 0;
            UInt32 blockNumber = data.GetUInt32();
            if (xml == null && blockNumber != settings.BlockIndex)
            {
                Debug.WriteLine("HanleSetRequestWithDataBlock failed. Invalid block number. " + settings.BlockIndex + "/" + blockNumber);
                ret = (byte)ErrorCode.DataBlockNumberInvalid;
            }
            else
            {
                settings.IncreaseBlockIndex();
                int size = GXCommon.GetObjectCount(data);
                int realSize = data.Size - data.Position;
                if (size != realSize)
                {
                    Debug.WriteLine("HanleSetRequestWithDataBlock failed. Invalid block size.");
                    ret = (byte)ErrorCode.DataBlockUnavailable;
                }
                if (xml != null)
                {
                    xml.AppendStartTag(TranslatorTags.DataBlock);
                    xml.AppendLine(TranslatorTags.LastBlock, "Value", xml.IntegerToHex(lastBlock, 2));
                    xml.AppendLine(TranslatorTags.BlockNumber, "Value", xml.IntegerToHex(blockNumber, 8));
                    xml.AppendLine(TranslatorTags.RawData, "Value", data.RemainingHexString(false));
                    xml.AppendEndTag(TranslatorTags.DataBlock);
                    return;
                }
                server.transaction.data.Set(data);
                //If all data is received.
                if (!p.multipleBlocks)
                {
                    try
                    {
                        object value = GXCommon.GetData(settings, server.transaction.data, reply);
                        if (value is byte[])
                        {
                            DataType dt = (server.transaction.targets[0].Target as IGXDLMSBase).GetDataType(server.transaction.targets[0].Index);
                            if (dt != DataType.None && dt != DataType.OctetString)
                            {
                                value = GXDLMSClient.ChangeType((byte[])value, dt, settings.UseUtc2NormalTime);
                            }
                        }
                        server.transaction.targets[0].Value = value;
                        server.NotifyWrite(server.transaction.targets);
                        if (!server.transaction.targets[0].Handled && !p.multipleBlocks)
                        {
                            (server.transaction.targets[0].Target as IGXDLMSBase).SetValue(settings, server.transaction.targets[0]);
                            server.NotifyPostWrite(server.transaction.targets);
                            ret = (byte)server.transaction.targets[0].Error;
                        }
                    }
                    catch (Exception)
                    {
                        ret = (byte)ErrorCode.InconsistentClass;
                    }
                    finally
                    {
                        server.transaction = null;
                    }
                    settings.ResetBlockIndex();
                }
            }
            if (ret != 0)
            {
                p.attributeDescriptor = new GXByteBuffer();
                p.attributeDescriptor.SetUInt8(ret);
            }
            p.multipleBlocks = true;
        }

        private static void HanleSetRequestWithList(GXDLMSSettings settings, byte invokeID, GXDLMSServer server, GXByteBuffer data,
            GXDLMSLNParameters p, GXByteBuffer replyData, GXDLMSTranslatorStructure xml)
        {
            int cnt = GXCommon.GetObjectCount(data);
            Dictionary<int, byte> status = new Dictionary<int, byte>();
            if (xml != null)
            {
                xml.AppendStartTag(TranslatorTags.AttributeDescriptorList, "Qty", xml.IntegerToHex(cnt, 2));
            }
            try
            {
                for (int pos = 0; pos != cnt; ++pos)
                {
                    status.Add(pos, 0);
                    ObjectType ci = (ObjectType)data.GetUInt16();
                    byte[] ln = new byte[6];
                    data.Get(ln);
                    short attributeIndex = data.GetUInt8();
                    // AccessSelection
                    int selection = data.GetUInt8();
                    int selector = 0;
                    object parameters = null;
                    if (selection != 0)
                    {
                        selector = data.GetUInt8();
                        GXDataInfo info = new GXDataInfo();
                        parameters = GXCommon.GetData(settings, data, info);
                    }
                    if (xml != null)
                    {
                        xml.AppendStartTag(TranslatorTags.AttributeDescriptorWithSelection);
                        xml.AppendStartTag(TranslatorTags.AttributeDescriptor);
                        xml.AppendComment(ci.ToString());
                        xml.AppendLine(TranslatorTags.ClassId, "Value", xml.IntegerToHex((int)ci, 4));
                        xml.AppendComment(GXCommon.ToLogicalName(ln));
                        xml.AppendLine(TranslatorTags.InstanceId, "Value", GXCommon.ToHex(ln, false));
                        xml.AppendLine(TranslatorTags.AttributeId, "Value", xml.IntegerToHex(attributeIndex, 2));
                        xml.AppendEndTag(TranslatorTags.AttributeDescriptor);
                        xml.AppendEndTag(TranslatorTags.AttributeDescriptorWithSelection);
                    }
                    else
                    {
                        GXDLMSObject obj = null;
                        if (ci == ObjectType.AssociationLogicalName && GXCommon.ToLogicalName(ln) == "0.0.40.0.0.255")
                        {
                            obj = settings.AssignedAssociation;
                        }
                        if (obj == null)
                        {
                            obj = settings.Objects.FindByLN(ci, GXCommon.ToLogicalName(ln));
                        }
                        if (obj == null)
                        {
                            obj = server.NotifyFindObject(ci, 0, GXCommon.ToLogicalName(ln));
                        }
                        if (obj == null)
                        {
                            status[pos] = (byte)ErrorCode.UndefinedObject;
                        }
                        else
                        {
                            ValueEventArgs arg = new ValueEventArgs(server, obj, attributeIndex, selector, parameters);
                            arg.InvokeId = invokeID;
                            if ((server.NotifyGetAttributeAccess(arg) & (int)AccessMode.Write) == 0)
                            {
                                status[pos] = (byte)ErrorCode.ReadWriteDenied;
                            }
                        }
                    }
                }
                cnt = GXCommon.GetObjectCount(data);
                if (xml != null)
                {
                    xml.AppendEndTag(TranslatorTags.AttributeDescriptorList);
                    xml.AppendStartTag(TranslatorTags.ValueList, "Qty", xml.IntegerToHex(cnt, 2));
                }
                for (int pos = 0; pos != cnt; ++pos)
                {
                    if (xml != null || status[pos] == 0)
                    {
                        GXDataInfo di = new GXDataInfo();
                        di.xml = xml;
                        if (xml != null && xml.OutputType == TranslatorOutputType.StandardXml)
                        {
                            xml.AppendStartTag(Command.WriteRequest, SingleReadResponse.Data);
                        }
                        try
                        {
                            object value = GXCommon.GetData(settings, data, di);
                            if (!di.Complete)
                            {
                                value = GXCommon.ToHex(data.Data, false, data.Position, data.Size - data.Position);
                            }
                            else if (value is byte[])
                            {
                                value = GXCommon.ToHex((byte[])value, false);
                            }
                            if (xml != null && xml
                                    .OutputType == TranslatorOutputType.StandardXml)
                            {
                                xml.AppendEndTag(Command.WriteRequest, SingleReadResponse.Data);
                            }
                        }
                        catch (Exception)
                        {
                            status[pos] = (byte)ErrorCode.ReadWriteDenied;
                        }
                    }
                }
                if (xml != null)
                {
                    xml.AppendEndTag(TranslatorTags.ValueList);
                }
            }
            catch (Exception ex)
            {
                if (xml == null)
                {
                    throw ex;
                }
            }
            p.status = 0xFF;
            p.attributeDescriptor = new GXByteBuffer();
            GXCommon.SetObjectCount(status.Count, p.attributeDescriptor);
            foreach (var it in status)
            {
                p.attributeDescriptor.SetUInt8(it.Value);
            }
            p.requestType = (byte)SetResponseType.WithList;
        }

        ///<summary>
        /// Handle Access request.
        ///</summary>
        public static void HandleAccessRequest(GXDLMSSettings settings, GXDLMSServer server, GXByteBuffer data,
                                               GXByteBuffer reply, GXDLMSTranslatorStructure xml, Command cipheredCommand)
        {
            //Return error if connection is not established.
            if (xml == null && (settings.Connected & ConnectionState.Dlms) == 0 && cipheredCommand == Command.None)
            {
                reply.Set(GXDLMSServer.GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                          ServiceError.Service, (byte)Service.Unsupported));
                return;
            }
            //Get long invoke id and priority.
            UInt32 invokeId = data.GetUInt32();
            settings.longInvokeID = invokeId;
            int len = GXCommon.GetObjectCount(data);
            byte[] tmp = null;
            // If date time is given.
            if (len != 0)
            {
                tmp = new byte[len];
                data.Get(tmp);
                if (xml == null)
                {
                    DataType dt = DataType.DateTime;
                    if (len == 4)
                    {
                        dt = DataType.Time;
                    }
                    else if (len == 5)
                    {
                        dt = DataType.Date;
                    }
                    GXDataInfo info = new GXDataInfo();
                    info.Type = dt;
                    GXCommon.GetData(settings, new GXByteBuffer(tmp), info);

                }
            }
            // Get object count.
            int cnt = GXCommon.GetObjectCount(data);
            if (xml != null)
            {
                xml.AppendStartTag(Command.AccessRequest);
                xml.AppendLine(TranslatorTags.LongInvokeId, "Value", xml.IntegerToHex(invokeId, 2));
                xml.AppendLine(TranslatorTags.DateTime, "Value", GXCommon.ToHex(tmp, false));
                xml.AppendStartTag(TranslatorTags.AccessRequestBody);
                xml.AppendStartTag(TranslatorTags.ListOfAccessRequestSpecification, "Qty",
                                   xml.IntegerToHex(cnt, 2));
            }
            List<GXDLMSAccessItem> list = new List<GXDLMSAccessItem>();
            AccessServiceCommandType type;
            for (int pos = 0; pos != cnt; ++pos)
            {
                type = (AccessServiceCommandType)data.GetUInt8();
                if (!(type == AccessServiceCommandType.Get ||
                        type == AccessServiceCommandType.Set ||
                        type == AccessServiceCommandType.Action))
                {
                    throw new ArgumentException("Invalid access service command type.");
                }
                // CI
                ObjectType ci = (ObjectType)data.GetUInt16();
                byte[] ln = new byte[6];
                data.Get(ln);
                // Attribute Id
                byte attributeIndex = data.GetUInt8();
                if (xml != null)
                {
                    xml.AppendStartTag(TranslatorTags.AccessRequestSpecification);
                    xml.AppendStartTag(Command.AccessRequest, type);
                    AppendAttributeDescriptor(xml, (int)ci, ln, attributeIndex);
                    xml.AppendEndTag(Command.AccessRequest, type);
                    xml.AppendEndTag(TranslatorTags.AccessRequestSpecification);
                }
                else
                {
                    GXDLMSObject obj = null;
                    if (ci == ObjectType.AssociationLogicalName && GXCommon.ToLogicalName(ln) == "0.0.40.0.0.255")
                    {
                        obj = settings.AssignedAssociation;
                    }
                    if (obj == null)
                    {
                        obj = settings.Objects.FindByLN(ci, GXCommon.ToLogicalName(ln));
                    }
                    list.Add(new GXDLMSAccessItem(type, obj, attributeIndex));
                }
            }
            if (xml != null)
            {
                xml.AppendEndTag(TranslatorTags.ListOfAccessRequestSpecification);
                xml.AppendStartTag(TranslatorTags.AccessRequestListOfData, "Qty", xml.IntegerToHex(cnt, 2));
            }
            // Get data count.
            cnt = GXCommon.GetObjectCount(data);
            GXByteBuffer bb = new GXByteBuffer();
            // access-request-specification.
            bb.SetUInt8(0);
            GXCommon.SetObjectCount(cnt, bb);
            GXByteBuffer results = new GXByteBuffer();
            GXCommon.SetObjectCount(cnt, results);
            for (int pos = 0; pos != cnt; ++pos)
            {
                GXDataInfo di = new GXDataInfo();
                di.xml = xml;
                if (xml != null && xml.OutputType == TranslatorOutputType.StandardXml)
                {
                    xml.AppendStartTag(Command.WriteRequest, SingleReadResponse.Data);
                }
                object value = GXCommon.GetData(settings, data, di);
                if (!di.Complete)
                {
                    value = GXCommon.ToHex(data.Data, false, data.Position, data.Size - data.Position);
                }
                else if (value is byte[])
                {
                    value = GXCommon.ToHex((byte[])value, false);
                }
                if (xml == null)
                {
                    GXDLMSAccessItem it = list[pos];
                    results.SetUInt8(it.Command);
                    if (it.Target == null)
                    {
                        //If target is unknown.
                        bb.SetUInt8(0);
                        results.SetUInt8(ErrorCode.UnavailableObject);
                    }
                    else
                    {
                        ValueEventArgs e = new ValueEventArgs(settings, it.Target, it.Index, 0, value);
                        if (it.Command == AccessServiceCommandType.Get)
                        {
                            int access = server.NotifyGetAttributeAccess(e);
                            if ((access & (int)AccessMode.Read) == 0 && (access & (int)AccessMode.AuthenticatedRead) == 0)
                            {
                                //Read Write denied.
                                bb.SetUInt8(0);
                                results.SetUInt8(ErrorCode.ReadWriteDenied);
                            }
                            else
                            {
                                server.NotifyRead(new ValueEventArgs[] { e });
                                if (e.Handled)
                                {
                                    value = e.Value;
                                }
                                else
                                {
                                    value = (it.Target as IGXDLMSBase).GetValue(settings, e);
                                }
                                //If all data is not fit to PDU and GBT is not used.
                                if (settings.Index != settings.Count)
                                {
                                    settings.Count = settings.Index = 0;
                                    bb.SetUInt8(0);
                                    results.SetUInt8(ErrorCode.ReadWriteDenied);
                                }
                                else
                                {
                                    if (e.ByteArray)
                                    {
                                        bb.Set((byte[])value);
                                    }
                                    else
                                    {
                                        GXDLMS.AppendData(settings, it.Target, it.Index, bb, value);
                                    }
                                    server.NotifyPostRead(new ValueEventArgs[] { e });
                                    results.SetUInt8(ErrorCode.Ok);
                                }
                            }
                        }
                        else if (it.Command == AccessServiceCommandType.Set)
                        {
                            results.SetUInt8(ErrorCode.Ok);
                        }
                        else
                        {
                            results.SetUInt8(ErrorCode.Ok);
                        }
                    }
                }
                if (xml != null && xml
                        .OutputType == TranslatorOutputType.StandardXml)
                {
                    xml.AppendEndTag(Command.WriteRequest, SingleReadResponse.Data);
                }
            }
            if (xml != null)
            {
                xml.AppendEndTag(TranslatorTags.AccessRequestListOfData);
                xml.AppendEndTag(TranslatorTags.AccessRequestBody);
                xml.AppendEndTag(Command.AccessRequest);
            }
            else
            {
                // Append status codes.
                bb.Set(results);
                GXDLMS.GetLNPdu(new GXDLMSLNParameters(settings, invokeId,
                        Command.AccessResponse, 0xff, null, bb, 0xFF, cipheredCommand), reply);
            }
        }

        ///<summary>
        /// Handle Event Notification.
        ///</summary>
        internal static void HandleEventNotification(GXDLMSSettings settings, GXReplyData reply, List<KeyValuePair<GXDLMSObject, int>> list)
        {
            reply.Time = DateTime.MinValue;
            //Check is there date-time.
            int len = reply.Data.GetUInt8();
            byte[] tmp = null;
            // If date time is given.
            if (len != 0)
            {
                len = reply.Data.GetUInt8();
                tmp = new byte[len];
                reply.Data.Get(tmp);
                reply.Time = (GXDateTime)GXDLMSClient.ChangeType(tmp, DataType.DateTime, settings.UseUtc2NormalTime);
            }
            if (reply.Xml != null)
            {
                reply.Xml.AppendStartTag(Command.EventNotification);
                if (reply.Time != DateTime.MinValue)
                {
                    reply.Xml.AppendComment(Convert.ToString(reply.Time));
                    reply.Xml.AppendLine(TranslatorTags.Time, null,
                                         GXCommon.ToHex(tmp, false));
                }
            }
            int ci = reply.Data.GetUInt16();
            byte[] ln = new byte[6];
            reply.Data.Get(ln);
            byte index = reply.Data.GetUInt8();
            if (reply.Xml != null)
            {
                AppendAttributeDescriptor(reply.Xml, ci, ln, index);
                reply.Xml.AppendStartTag(TranslatorTags.AttributeValue);
            }
            GXDataInfo di = new GXDataInfo();
            di.xml = reply.Xml;
            object value = GXCommon.GetData(settings, reply.Data, di);

            if (reply.Xml != null)
            {
                reply.Xml.AppendEndTag(TranslatorTags.AttributeValue);
                reply.Xml.AppendEndTag(Command.EventNotification);
            }
            else
            {
                GXDLMSObject obj = null;
                if ((ObjectType)ci == ObjectType.AssociationLogicalName && GXCommon.ToLogicalName(ln) == "0.0.40.0.0.255")
                {
                    obj = settings.AssignedAssociation;
                }
                if (obj == null)
                {
                    obj = settings.Objects.FindByLN((ObjectType)ci, GXCommon.ToLogicalName(ln));
                }
                if (obj != null)
                {
                    ValueEventArgs v = new ValueEventArgs(obj, index, 0, null);
                    v.Value = value;
                    (obj as IGXDLMSBase).SetValue(settings, v);
                    list.Add(new KeyValuePair<GXDLMSObject, int>(obj, index));
                }
            }
        }

    }
}
