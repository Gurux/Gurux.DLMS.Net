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

namespace Gurux.DLMS
{
    /// <summary>
    /// this class is used to handle SN commands.
    /// </summary>
    internal sealed class GXDLMSSNCommandHandler
    {
        /// <summary>
        /// Handle read request.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="server">DLMS server.</param>
        /// <param name="data">Received data.</param>
        public static void HandleReadRequest(GXDLMSSettings settings, GXDLMSServer server, GXByteBuffer data, GXByteBuffer replyData, GXDLMSTranslatorStructure xml, Command cipheredCommand)
        {
            //Return error if connection is not established.
            if (xml == null && (settings.Connected & ConnectionState.Dlms) == 0 && cipheredCommand == Command.None)
            {
                replyData.Add(GXDLMSServer.GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                              ServiceError.Service, (byte)Service.Unsupported));
                return;
            }

            GXByteBuffer bb = new GXByteBuffer();
            int cnt = 0xFF;
            byte type = 0;
            List<ValueEventArgs> list = new List<ValueEventArgs>();
            List<ValueEventArgs> reads = new List<ValueEventArgs>();
            //If get next frame.
            if (xml == null && data.Size == 0)
            {
                if (server.transaction != null)
                {
                    return;
                }
                bb.Set(replyData);
                replyData.Clear();
                foreach (ValueEventArgs it in server.transaction.targets)
                {
                    list.Add(it);
                }
            }
            else
            {
                cnt = GXCommon.GetObjectCount(data);
                if (xml != null)
                {
                    xml.AppendStartTag(Command.ReadRequest, "Qty", xml.IntegerToHex(cnt, 2));
                }
                for (int pos = 0; pos != cnt; ++pos)
                {
                    type = data.GetUInt8();
                    if (type == (byte)VariableAccessSpecification.VariableName ||
                            type == (byte)VariableAccessSpecification.ParameterisedAccess)
                    {
                        HandleRead(settings, server, type, data, list, reads, replyData, xml, cipheredCommand);
                    }
                    else if (type == (byte)VariableAccessSpecification.BlockNumberAccess)
                    {
                        HandleReadBlockNumberAccess(settings, server, data, replyData, xml);
                        if (xml != null)
                        {
                            xml.AppendEndTag(Command.ReadRequest);
                        }
                        return;
                    }
                    else if (type == (byte)VariableAccessSpecification.ReadDataBlockAccess)
                    {
                        HandleReadDataBlockAccess(settings, server, Command.ReadResponse, data, cnt, replyData, xml, cipheredCommand);
                        if (xml != null)
                        {
                            xml.AppendEndTag(Command.ReadRequest);
                        }
                        return;
                    }
                    else
                    {
                        ReturnSNError(settings, server, Command.ReadResponse, ErrorCode.ReadWriteDenied, replyData);
                        if (xml != null)
                        {
                            xml.AppendEndTag(Command.ReadRequest);
                        }
                        return;
                    }
                }
                if (reads.Count != 0)
                {
                    server.NotifyRead(reads.ToArray());
                }
            }
            if (xml != null)
            {
                xml.AppendEndTag(Command.ReadRequest);
                return;
            }

            byte requestType = (byte)GetReadData(settings, list.ToArray(), bb);
            if (reads.Count != 0)
            {
                server.NotifyPostRead(reads.ToArray());
            }
            GXDLMSSNParameters p = new GXDLMSSNParameters(settings, Command.ReadResponse, list.Count, requestType, null, bb);
            GXDLMS.GetSNPdu(p, replyData);
            if (server.transaction == null && (bb.Size != bb.Position || settings.Count != settings.Index))
            {
                reads = new List<ValueEventArgs>();
                foreach (var it in list)
                {
                    reads.Add(it);
                }
                server.transaction = new GXDLMSLongTransaction(reads.ToArray(), Command.ReadRequest, bb);
            }
            else if (server.transaction != null)
            {
                replyData.Set(bb);
                return;
            }
        }

        ///<summary>
        /// Handle write request.
        ///</summary>
        public static void HandleWriteRequest(GXDLMSSettings settings, GXDLMSServer server, GXByteBuffer data,
                                              GXByteBuffer replyData, GXDLMSTranslatorStructure xml, Command cipheredCommand)
        {
            short type;
            object value;
            // Get object count.
            List<GXSNInfo> targets = new List<GXSNInfo>();
            int cnt = GXCommon.GetObjectCount(data);
            if (xml != null)
            {
                xml.AppendStartTag(Command.WriteRequest);
                xml.AppendStartTag(
                    TranslatorTags.ListOfVariableAccessSpecification, "Qty",
                    xml.IntegerToHex(cnt, 2));
                if (xml.OutputType == TranslatorOutputType.StandardXml)
                {
                    xml.AppendStartTag(
                        TranslatorTags.VariableAccessSpecification);
                }
            }
            GXDataInfo di;
            GXSNInfo info;
            GXByteBuffer results = new GXByteBuffer((ushort)cnt);
            for (int pos = 0; pos != cnt; ++pos)
            {
                type = data.GetUInt8();
                if (type == (byte)VariableAccessSpecification.VariableName)
                {
                    int sn = data.GetUInt16();
                    if (xml != null)
                    {
                        xml.AppendLine(
                            (int)Command.WriteRequest << 8
                            | type,
                            "Value", xml.IntegerToHex(sn, 4));
                    }
                    else
                    {
                        info = FindSNObject(server, sn);
                        targets.Add(info);
                        // If target is unknown.
                        if (info == null)
                        {
                            // Device reports a undefined object.
                            results.SetUInt8(ErrorCode.UndefinedObject);
                        }
                        else
                        {
                            results.SetUInt8(ErrorCode.Ok);
                        }
                    }
                }
                else if (type == (byte)VariableAccessSpecification.WriteDataBlockAccess)
                {
                    //Return error if connection is not established.
                    if (xml == null && (settings.Connected & ConnectionState.Dlms) == 0 && cipheredCommand == Command.None)
                    {
                        replyData.Add(GXDLMSServer.GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                                      ServiceError.Service, (byte)Service.Unsupported));
                        return;
                    }
                    HandleReadDataBlockAccess(settings, server, Command.WriteResponse, data, cnt, replyData, xml, cipheredCommand);
                    if (xml == null)
                    {
                        return;
                    }
                }
                else
                {
                    // Device reports a HW error.
                    results.SetUInt8(ErrorCode.HardwareFault);
                }
            }

            if (xml != null)
            {
                if (xml.OutputType == TranslatorOutputType.StandardXml)
                {
                    xml.AppendEndTag(TranslatorTags.VariableAccessSpecification);
                }
                xml.AppendEndTag(
                    TranslatorTags.ListOfVariableAccessSpecification);
            }
            // Get data count.
            cnt = GXCommon.GetObjectCount(data);
            di = new GXDataInfo();
            di.xml = xml;
            if (xml != null)
            {
                xml.AppendStartTag(TranslatorTags.ListOfData, "Qty", xml.IntegerToHex(cnt, 2));
            }
            for (int pos = 0; pos != cnt; ++pos)
            {
                di.Clear();
                if (xml != null)
                {
                    if (xml.OutputType == TranslatorOutputType.StandardXml)
                    {
                        xml.AppendStartTag(Command.WriteRequest,
                                           SingleReadResponse.Data);
                    }
                    value = GXCommon.GetData(settings, data, di);
                    if (!di.Complete)
                    {
                        value = GXCommon.ToHex(data.Data, false,
                                               data.Position, data.Size - data.Position);
                        xml.AppendLine(
                            GXDLMS.DATA_TYPE_OFFSET + (int)di.Type,
                            "Value", value.ToString());
                    }
                    if (xml.OutputType == TranslatorOutputType.StandardXml)
                    {
                        xml.AppendEndTag(Command.WriteRequest, SingleReadResponse.Data);
                    }
                }
                else if (results.GetUInt8(pos) == 0)
                {
                    bool access = true;
                    // If object has found.
                    GXSNInfo target = targets[pos];
                    value = GXCommon.GetData(settings, data, di);
                    ValueEventArgs e = new ValueEventArgs(server, target.Item, target.Index, 0, null);
                    if (target.IsAction)
                    {
                        MethodAccessMode am = server.NotifyGetMethodAccess(e);
                        // If action is denied.
                        if (am != MethodAccessMode.Access)
                        {
                            access = false;
                        }
                    }
                    else
                    {
                        if (value is byte[])
                        {
                            DataType dt = target.Item.GetDataType(target.Index);
                            if (dt != DataType.None && dt != DataType.OctetString)
                            {
                                value = GXDLMSClient.ChangeType((byte[])value, dt, settings.UseUtc2NormalTime);
                            }
                        }
                        AccessMode am = server.NotifyGetAttributeAccess(e);
                        // If write is denied.
                        if (am != AccessMode.Write && am != AccessMode.ReadWrite)
                        {
                            access = false;
                        }
                    }
                    if (access)
                    {
                        if (target.IsAction)
                        {
                            e.Parameters = value;
                            ValueEventArgs[] actions = new ValueEventArgs[] {e };
                            server.NotifyPreAction(actions);
                            if (!e.Handled)
                            {
                                byte[] reply = (target.Item as IGXDLMSBase).Invoke(settings, e);
                                server.NotifyPostAction(actions);
                                if (target.Item is GXDLMSAssociationShortName && target.Index == 8 && reply != null)
                                {
                                    GXByteBuffer bb = new GXByteBuffer();
                                    bb.SetUInt8((byte)DataType.OctetString);
                                    bb.SetUInt8((byte) reply.Length);
                                    bb.Set(reply);
                                    GXDLMSSNParameters p = new GXDLMSSNParameters(settings, Command.ReadResponse, 1, 0, null, bb);
                                    GXDLMS.GetSNPdu(p, replyData);
                                }
                            }
                        }
                        else
                        {
                            e.Value = value;
                            server.NotifyWrite(new ValueEventArgs[] { e });
                            if (e.Error != 0)
                            {
                                results.SetUInt8((byte)pos, (byte)e.Error);
                            }
                            else if (!e.Handled)
                            {
                                (target.Item as IGXDLMSBase).SetValue(settings, e);
                                server.NotifyPostWrite(new ValueEventArgs[] { e });
                            }
                        }
                    }
                    else
                    {
                        results.SetUInt8((byte)pos, (byte)ErrorCode.ReadWriteDenied);
                    }
                }
            }
            if (xml != null)
            {
                xml.AppendEndTag(TranslatorTags.ListOfData);
                xml.AppendEndTag(Command.WriteRequest);
                return;
            }
            GenerateWriteResponse(settings, results, replyData);
        }

        /// <summary>
        /// Generate write reply.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="results"></param>
        /// <param name="replyData"></param>
        internal static void GenerateWriteResponse(GXDLMSSettings settings, GXByteBuffer results, GXByteBuffer replyData)
        {
            GXByteBuffer bb = new GXByteBuffer((UInt16)(2 * results.Size));
            byte ret;
            for (int pos = 0; pos != results.Size; ++pos)
            {
                ret = results.GetUInt8(pos);
                // If meter returns error.
                if (ret != 0)
                {
                    bb.SetUInt8(1);
                }
                bb.SetUInt8(ret);
            }
            GXDLMSSNParameters p = new GXDLMSSNParameters(settings, Command.WriteResponse, results.Size, 0xFF, null, bb);
            GXDLMS.GetSNPdu(p, replyData);
        }

        private static void HandleRead(GXDLMSSettings settings, GXDLMSServer server, byte type, GXByteBuffer data,
                                       List<ValueEventArgs> list, List<ValueEventArgs> reads,
                                       GXByteBuffer replyData, GXDLMSTranslatorStructure xml, Command cipheredCommand)
        {
            int sn = data.GetInt16();
            if (xml != null)
            {
                if (xml.OutputType == TranslatorOutputType.StandardXml)
                {
                    xml.AppendStartTag(
                        TranslatorTags.VariableAccessSpecification);
                }
                else
                {
                    sn &= 0xFFFF;
                }
                if (type == (byte)VariableAccessSpecification.ParameterisedAccess)
                {
                    xml.AppendStartTag(Command.ReadRequest,
                                       VariableAccessSpecification.ParameterisedAccess);
                    xml.AppendLine(
                        (int)Command.ReadRequest << 8
                        | (int)VariableAccessSpecification.VariableName,
                        "Value", xml.IntegerToHex(sn, 4));
                    xml.AppendLine(TranslatorTags.Selector, "Value",
                                   xml.IntegerToHex(data.GetUInt8(), 2));
                    GXDataInfo di = new GXDataInfo();
                    di.xml = xml;
                    xml.AppendStartTag(TranslatorTags.Parameter);
                    GXCommon.GetData(settings, data, di);
                    xml.AppendEndTag(TranslatorTags.Parameter);
                    xml.AppendEndTag(Command.ReadRequest,
                                     VariableAccessSpecification.ParameterisedAccess);
                }
                else
                {
                    xml.AppendLine(
                        (int)Command.ReadRequest << 8
                        | (int)VariableAccessSpecification.VariableName,
                        "Value", xml.IntegerToHex(sn, 4));
                }
                if (xml.OutputType == TranslatorOutputType.StandardXml)
                {
                    xml.AppendEndTag(TranslatorTags.VariableAccessSpecification);
                }
                return;
            }

            GXSNInfo info = FindSNObject(server, sn & 0xFFFF);
            ValueEventArgs e = new ValueEventArgs(server, info.Item, info.Index, 0, null);
            e.action = info.IsAction;
            if (type == (byte)VariableAccessSpecification.ParameterisedAccess)
            {
                e.Selector = data.GetUInt8();
                GXDataInfo di = new GXDataInfo();
                e.Parameters = GXCommon.GetData(settings, data, di);
            }
            //Return error if connection is not established.
            if ((settings.Connected & ConnectionState.Dlms) == 0 && cipheredCommand == Command.None && (!e.action || e.Target.ShortName != 0xFA00 || e.Index != 8))
            {
                replyData.Add(GXDLMSServer.GenerateConfirmedServiceError(ConfirmedServiceError.InitiateError,
                              ServiceError.Service, (byte)Service.Unsupported));
                return;
            }
            if (e.Target is GXDLMSProfileGeneric && info.Index == 2)
            {
                e.RowToPdu = GXDLMS.RowsToPdu(settings, (GXDLMSProfileGeneric)e.Target);
            }
            list.Add(e);
            if (!e.action && server.NotifyGetAttributeAccess(e) == AccessMode.NoAccess)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            else if (e.action && server.NotifyGetMethodAccess(e) == MethodAccessMode.NoAccess)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            else
            {
                reads.Add(e);
            }
        }

        /// <summary>
        /// Handle read Block in blocks.
        /// </summary>
        /// <param name="data">Received data.</param>
        private static void HandleReadBlockNumberAccess(GXDLMSSettings settings, GXDLMSServer server, GXByteBuffer data, GXByteBuffer replyData, GXDLMSTranslatorStructure xml)
        {
            UInt16 blockNumber = data.GetUInt16();
            if (xml != null)
            {
                xml.AppendStartTag(Command.ReadRequest, VariableAccessSpecification.BlockNumberAccess);
                xml.AppendLine("<BlockNumber Value=\"" + xml.IntegerToHex(blockNumber, 4) + "\" />");
                xml.AppendEndTag(Command.ReadRequest, VariableAccessSpecification.BlockNumberAccess);
                return;
            }

            if (blockNumber != settings.BlockIndex)
            {
                GXByteBuffer bb = new GXByteBuffer();
                Debug.WriteLine("handleReadRequest failed. Invalid block number. " + settings.BlockIndex + "/" + blockNumber);
                bb.SetUInt8(ErrorCode.DataBlockNumberInvalid);
                GXDLMS.GetSNPdu(new GXDLMSSNParameters(settings, Command.ReadResponse, 1, (byte)SingleReadResponse.DataAccessError, bb, null), replyData);
                settings.ResetBlockIndex();
                return;
            }
            if (settings.Index != settings.Count && server.transaction.data.Size < settings.MaxPduSize)
            {
                List<ValueEventArgs> reads = new List<ValueEventArgs>();
                List<ValueEventArgs> actions = new List<ValueEventArgs>();
                foreach (ValueEventArgs it in server.transaction.targets)
                {
                    if (it.action)
                    {
                        actions.Add(it);
                    }
                    else
                    {
                        reads.Add(it);
                    }
                }
                if (reads.Count != 0)
                {
                    server.NotifyRead(reads.ToArray());
                }

                if (actions.Count != 0)
                {
                    server.NotifyPreAction(actions.ToArray());
                }
                GetReadData(settings, server.transaction.targets, server.transaction.data);
                if (reads.Count != 0)
                {
                    server.NotifyPostRead(reads.ToArray());
                }

                if (actions.Count != 0)
                {
                    server.NotifyPostAction(actions.ToArray());
                }
            }
            settings.IncreaseBlockIndex();
            GXDLMSSNParameters p = new GXDLMSSNParameters(settings, Command.ReadResponse, 1,
                    (byte)SingleReadResponse.DataBlockResult, null, server.transaction.data);
            p.multipleBlocks = true;
            GXDLMS.GetSNPdu(p, replyData);
            //If all data is sent.
            if (server.transaction.data.Size == server.transaction.data.Position)
            {
                server.transaction = null;
                settings.ResetBlockIndex();
            }
            else
            {
                server.transaction.data.Trim();
            }
        }

        private static void HandleReadDataBlockAccess(
            GXDLMSSettings settings, GXDLMSServer server,
            Command command, GXByteBuffer data, int cnt, GXByteBuffer replyData, GXDLMSTranslatorStructure xml, Command cipheredCommand)
        {
            GXByteBuffer bb = new GXByteBuffer();
            byte lastBlock = data.GetUInt8();
            UInt16 blockNumber = data.GetUInt16();
            if (xml != null)
            {
                if (command == Command.WriteResponse)
                {
                    xml.AppendStartTag(TranslatorTags.WriteDataBlockAccess);
                }
                else
                {
                    xml.AppendStartTag(TranslatorTags.ReadDataBlockAccess);
                }
                xml.AppendLine("<LastBlock Value=\"" + xml.IntegerToHex(lastBlock, 2) + "\" />");
                xml.AppendLine("<BlockNumber Value=\"" + xml.IntegerToHex(blockNumber, 4) + "\" />");
                if (command == Command.WriteResponse)
                {
                    xml.AppendEndTag(TranslatorTags.WriteDataBlockAccess);
                }
                else
                {
                    xml.AppendEndTag(TranslatorTags.ReadDataBlockAccess);
                }
                return;
            }
            if (blockNumber != settings.BlockIndex)
            {
                Debug.WriteLine("handleReadRequest failed. Invalid block number. " + settings.BlockIndex + "/" + blockNumber);
                bb.SetUInt8(ErrorCode.DataBlockNumberInvalid);
                GXDLMS.GetSNPdu(new GXDLMSSNParameters(settings, command, 1, (byte)SingleReadResponse.DataAccessError, bb, null), replyData);
                settings.ResetBlockIndex();
                return;
            }
            int count = 1;
            byte type = (byte)DataType.OctetString;
            if (command == Command.WriteResponse)
            {
                count = GXCommon.GetObjectCount(data);
                type = data.GetUInt8();
            }
            int size = GXCommon.GetObjectCount(data);
            int realSize = data.Size - data.Position;
            if (count != 1 || type != (byte)DataType.OctetString || size != realSize)
            {
                Debug.WriteLine("handleGetRequest failed. Invalid block size.");
                bb.SetUInt8(ErrorCode.DataBlockUnavailable);
                GXDLMS.GetSNPdu(new GXDLMSSNParameters(settings, command, cnt, (byte)SingleReadResponse.DataAccessError, bb, null), replyData);
                settings.ResetBlockIndex();
                return;
            }
            if (server.transaction == null)
            {
                server.transaction = new GXDLMSLongTransaction(null, command, data);
            }
            else
            {
                server.transaction.data.Set(data);
            }
            if (lastBlock == 0)
            {
                bb.SetUInt16(blockNumber);
                settings.IncreaseBlockIndex();
                if (command == Command.ReadResponse)
                {
                    type = (byte)SingleReadResponse.BlockNumber;
                }
                else
                {
                    type = (byte)SingleWriteResponse.BlockNumber;
                }
                GXDLMS.GetSNPdu(new GXDLMSSNParameters(settings, command, cnt, type, null, bb), replyData);
                return;
            }
            else
            {
                if (server.transaction != null)
                {
                    data.Size = 0;
                    data.Set(server.transaction.data);
                    server.transaction = null;
                }
                if (command == Command.ReadResponse)
                {
                    HandleReadRequest(settings, server, data, replyData, xml, cipheredCommand);
                }
                else
                {
                    HandleWriteRequest(settings, server, data, replyData, xml, cipheredCommand);
                }
                settings.ResetBlockIndex();
            }
        }

        internal static GXSNInfo FindSNObject(GXDLMSObjectCollection items, int sn)
        {
            GXSNInfo i = new GXSNInfo();
            int offset, count;
            foreach (GXDLMSObject it in items)
            {
                if (sn >= it.ShortName)
                {
                    //If attribute is accessed.
                    if (sn < it.ShortName + (it as IGXDLMSBase).GetAttributeCount() * 8)
                    {
                        i.IsAction = false;
                        i.Item = it;
                        i.Index = ((sn - i.Item.ShortName) / 8) + 1;
                        break;
                    }
                    else
                    {
                        //If method is accessed.
                        GXDLMS.GetActionInfo(it.ObjectType, out offset, out count);
                        if (sn < it.ShortName + offset + (8 * count))
                        {
                            i.Item = it;
                            i.IsAction = true;
                            i.Index = (sn - it.ShortName - offset) / 8 + 1;
                            break;
                        }
                    }
                }
            }
            return i;
        }

        ///<summary>
        /// Find Short Name object.
        ///</summary>
        ///<param name="sn">
        ///Short name to find.
        ///</param>
        private static GXSNInfo FindSNObject(GXDLMSServer server, int sn)
        {
            GXSNInfo i = FindSNObject(server.Items, sn);
            if (i.Item == null)
            {
                i.Item = server.NotifyFindObject(ObjectType.None, sn, null);
            }
            return i;
        }

        private static void ReturnSNError(GXDLMSSettings settings, GXDLMSServer server, Command cmd, ErrorCode error, GXByteBuffer replyData)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(error);
            GXDLMS.GetSNPdu(new GXDLMSSNParameters(settings, cmd, 1, (byte)SingleReadResponse.DataAccessError, bb, null), replyData);
            settings.ResetBlockIndex();
        }

        /// <summary>
        /// Get data for Read command.
        /// </summary>
        /// <param name="list">received objects.</param>
        /// <param name="data">Data as byte array.</param>
        /// <returns>Response type.</returns>
        private static SingleReadResponse GetReadData(GXDLMSSettings settings, ValueEventArgs[] list, GXByteBuffer data)
        {
            object value;
            bool first = true;
            SingleReadResponse type = SingleReadResponse.Data;
            foreach (ValueEventArgs e in list)
            {
                if (e.Handled)
                {
                    value = e.Value;
                }
                else
                {
                    //If action.
                    if (e.action)
                    {
                        value = ((IGXDLMSBase)e.Target).Invoke(settings, e);
                    }
                    else
                    {
                        value = (e.Target as IGXDLMSBase).GetValue(settings, e);
                    }
                }
                if (e.Error == 0)
                {
                    if (!first && list.Length != 1)
                    {
                        data.SetUInt8(SingleReadResponse.Data);
                    }
                    //If action.
                    if (e.action)
                    {
                        GXCommon.SetData(settings, data, GXDLMSConverter.GetDLMSDataType(value), value);
                    }
                    else
                    {
                        GXDLMS.AppendData(settings, e.Target, e.Index, data, value);
                    }
                }
                else
                {
                    if (!first && list.Length != 1)
                    {
                        data.SetUInt8(SingleReadResponse.DataAccessError);
                    }
                    data.SetUInt8(e.Error);
                    type = SingleReadResponse.DataAccessError;
                }
                first = false;
            }
            return type;
        }

        /// <summary>
        /// Handle Information Report.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="reply"></param>
        /// <returns></returns>
        public static void HandleInformationReport(GXDLMSSettings settings, GXReplyData reply, List<KeyValuePair<GXDLMSObject, int>> list)
        {
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
            byte type;
            int count = GXCommon.GetObjectCount(reply.Data);
            if (reply.Xml != null)
            {
                reply.Xml.AppendStartTag(Command.InformationReport);
                if (reply.Time != DateTime.MinValue)
                {
                    reply.Xml.AppendComment(Convert.ToString(reply.Time));
                    if (reply.Xml.OutputType == TranslatorOutputType.SimpleXml)
                    {
                        reply.Xml.AppendLine(TranslatorTags.CurrentTime, null, GXCommon.ToHex(tmp, false));
                    }
                    else
                    {
                        reply.Xml.AppendLine(TranslatorTags.CurrentTime, null,
                                GXCommon.GeneralizedTime(reply.Time));
                    }
                }
                reply.Xml.AppendStartTag(TranslatorTags.ListOfVariableAccessSpecification, "Qty", reply.Xml.IntegerToHex(count, 2));
            }
            for (int pos = 0; pos != count; ++pos)
            {
                type = reply.Data.GetUInt8();
                if (type == (byte)VariableAccessSpecification.VariableName)
                {
                    int sn = reply.Data.GetUInt16();
                    if (reply.Xml != null)
                    {
                        reply.Xml.AppendLine(
                            (int)Command.WriteRequest << 8
                            | (int)VariableAccessSpecification.VariableName,
                            "Value", reply.Xml.IntegerToHex(sn, 4));
                    }
                    else
                    {
                        GXSNInfo info = FindSNObject(settings.Objects, sn);
                        if (info.Item != null)
                        {
                            list.Add(new KeyValuePair<GXDLMSObject, int>(info.Item, info.Index));
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0}.", sn));
                        }
                    }
                }
            }
            if (reply.Xml != null)
            {
                reply.Xml.AppendEndTag(TranslatorTags.ListOfVariableAccessSpecification);
                reply.Xml.AppendStartTag(TranslatorTags.ListOfData, "Qty", reply.Xml.IntegerToHex(count, 2));
            }
            //Get values.
            count = GXCommon.GetObjectCount(reply.Data);
            GXDataInfo di = new GXDataInfo();
            di.xml = reply.Xml;
            for (int pos = 0; pos != count; ++pos)
            {
                di.Clear();
                if (reply.Xml != null)
                {
                    GXCommon.GetData(settings, reply.Data, di);
                }
                else
                {
                    ValueEventArgs v = new ValueEventArgs(list[pos].Key, list[pos].Value, 0, null);
                    v.Value = GXCommon.GetData(settings, reply.Data, di);
                    (list[pos].Key as IGXDLMSBase).SetValue(settings, v);
                }
            }
            if (reply.Xml != null)
            {
                reply.Xml.AppendEndTag(TranslatorTags.ListOfData);
                reply.Xml.AppendEndTag(Command.InformationReport);
            }
        }
    }
}
