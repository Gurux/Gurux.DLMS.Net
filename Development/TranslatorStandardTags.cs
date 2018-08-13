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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{
    class TranslatorStandardTags
    {
        /// <summary>
        /// Get general tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetGeneralTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            GXDLMSTranslator.AddTag(list, Command.Snrm, "Snrm");
            GXDLMSTranslator.AddTag(list, Command.UnacceptableFrame, "UnacceptableFrame");
            GXDLMSTranslator.AddTag(list, Command.DisconnectMode, "DisconnectMode");
            GXDLMSTranslator.AddTag(list, Command.Ua, "Ua");
            GXDLMSTranslator.AddTag(list, Command.Aarq, "aarq");
            GXDLMSTranslator.AddTag(list, Command.Aare, "aare");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ApplicationContextName,
                                    "application-context-name");
            GXDLMSTranslator.AddTag(list, Command.InitiateResponse,
                                    "InitiateResponse");
            GXDLMSTranslator.AddTag(list, Command.InitiateRequest,
                                    "user-information");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.NegotiatedQualityOfService,
                                    "negotiated-quality-of-service");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ProposedQualityOfService,
                                    "proposed-quality-of-service");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ProposedDlmsVersionNumber,
                                    "proposed-dlms-version-number");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ProposedMaxPduSize,
                                    "client-max-receive-pdu-size");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ProposedConformance,
                                    "proposed-conformance");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.VaaName,
                                    "VaaName");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.NegotiatedConformance,
                                    "NegotiatedConformance");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.NegotiatedDlmsVersionNumber,
                                    "NegotiatedDlmsVersionNumber");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.NegotiatedMaxPduSize,
                                    "NegotiatedMaxPduSize");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.ConformanceBit,
                                    "ConformanceBit");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.SenderACSERequirements,
                                    "sender-acse-requirements");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ResponderACSERequirement,
                                    "responder-acse-requirements");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.RespondingMechanismName,
                                    "mechanism-name");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.CallingMechanismName,
                                    "mechanism-name");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.CallingAuthentication,
                                    "calling-authentication-value");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.RespondingAuthentication,
                                    "responding-authentication-value");
            GXDLMSTranslator.AddTag(list, Command.ReleaseRequest,
                                    "ReleaseRequest");
            GXDLMSTranslator.AddTag(list, Command.ReleaseResponse,
                                    "ReleaseResponse");
            GXDLMSTranslator.AddTag(list, Command.DisconnectRequest, "Disc");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.AssociationResult,
                                    "result");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ResultSourceDiagnostic,
                                    "result-source-diagnostic");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.ACSEServiceUser,
                                    "acse-service-user");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.CallingAPTitle,
                                    "CallingAPTitle");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.RespondingAPTitle,
                                    "RespondingAPTitle");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.CharString, "charstring");

            GXDLMSTranslator.AddTag(list, Command.ConfirmedServiceError, "confirmedServiceError");
            GXDLMSTranslator.AddTag(list, Command.InformationReport, "InformationReport");
            GXDLMSTranslator.AddTag(list, Command.EventNotification, "event-notification-request");

            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.CallingAeInvocationId,
                                    "calling-AE-invocation-id");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.CalledAeInvocationId,
                                    "called-AE-invocation-id");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.RespondingAeInvocationId, "responding-AE-invocation-id");
        }

        /// <summary>
        /// Get SN tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetSnTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            list.Add((int)Command.ReadRequest, "readRequest");
            list.Add((int)Command.WriteRequest, "writeRequest");
            list.Add((int)Command.WriteResponse, "writeResponse");
            list.Add((int)Command.WriteRequest << 8 | (int)SingleReadResponse.Data,
                     "Data");
            list.Add(
                (int)Command.ReadRequest << 8
                | (int)VariableAccessSpecification.VariableName,
                "variable-name");
            list.Add(
                (int)Command.ReadRequest << 8
                | (int)VariableAccessSpecification.ParameterisedAccess,
                "parameterized-access");
            list.Add(
                (int)Command.ReadRequest << 8
                | (int)VariableAccessSpecification.BlockNumberAccess,
                "BlockNumberAccess");
            list.Add(
                (int)Command.WriteRequest << 8
                | (int)VariableAccessSpecification.VariableName,
                "variable-name");

            list.Add((int)Command.ReadResponse, "readResponse");
            list.Add(
                (int)Command.ReadResponse << 8
                | (int)SingleReadResponse.DataBlockResult,
                "DataBlockResult");
            list.Add((int)Command.ReadResponse << 8 | (int)SingleReadResponse.Data,
                     "data");
            list.Add((int)Command.WriteResponse << 8 | (int)SingleReadResponse.Data,
                     "data");
            list.Add(
                (int)Command.ReadResponse << 8
                | (int)SingleReadResponse.DataAccessError,
                "data-access-error");
        }

        /// <summary>
        /// Get LN tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetLnTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            GXDLMSTranslator.AddTag(list, Command.GetRequest, "get-request");
            list.Add((int)Command.GetRequest << 8 | (int)GetCommandType.Normal,
                     "get-request-normal");
            list.Add((int)Command.GetRequest << 8 | (int)GetCommandType.NextDataBlock,
                     "get-request-next");
            list.Add((int)Command.GetRequest << 8 | (int)GetCommandType.WithList,
                     "get-request-with-list");
            GXDLMSTranslator.AddTag(list, Command.SetRequest, "set-request");
            list.Add((int)Command.SetRequest << 8 | (int)SetRequestType.Normal,
                     "set-request-normal");
            list.Add((int)Command.SetRequest << 8 | (int)SetRequestType.FirstDataBlock,
                     "set-request-first-data-block");
            list.Add((int)Command.SetRequest << 8 | (int)SetRequestType.WithDataBlock,
                     "set-request-with-data-block");
            list.Add((int)Command.SetRequest << 8 | (int)SetRequestType.WithList,
                     "set-request-with-list");
            GXDLMSTranslator.AddTag(list, Command.MethodRequest,
                                    "action-request");
            list.Add((int)Command.MethodRequest << 8 | (int)ActionRequestType.Normal,
                     "action-request-normal");
            list.Add((int)Command.MethodRequest << 8 | (int)ActionRequestType.NextBlock,
                     "ActionRequestForNextDataBlock");
            list.Add((int)Command.MethodRequest << 8 | (int)ActionRequestType.WithList,
                     "action-request-with-list");
            GXDLMSTranslator.AddTag(list, Command.MethodResponse,
                                    "action-response");
            list.Add((int)Command.MethodResponse << 8 | (int)ActionResponseType.Normal,
                     "action-response-normal");
            list.Add((int)Command.MethodResponse << 8 | (int)ActionResponseType.WithFirstBlock,
                     "action-response-with-first-block");
            list.Add((int)Command.MethodResponse << 8 | (int)ActionResponseType.WithList,
                     "action-response-with-list");
            list.Add((int)TranslatorTags.SingleResponse, "single-response");
            list.Add((int)Command.DataNotification, "data-notification");
            list.Add((int)Command.GetResponse, "get-response");
            list.Add((int)Command.GetResponse << 8 | (int)GetCommandType.Normal, "get-response-normal");
            list.Add((int)Command.GetResponse << 8 | (int)GetCommandType.NextDataBlock, "get-response-with-data-block");
            list.Add((int)Command.GetResponse << 8 | (int)GetCommandType.WithList, "get-response-with-list");
            GXDLMSTranslator.AddTag(list, Command.SetResponse, "set-response");
            list.Add((int)Command.SetResponse << 8 | (int)SetResponseType.Normal, "set-response-normal");
            list.Add((int)Command.SetResponse << 8 | (int)SetResponseType.DataBlock,
                     "set-response-data-block");
            list.Add((int)Command.SetResponse << 8 | (int)SetResponseType.LastDataBlock,
                     "set-response-with-last-data-block");
            list.Add((int)Command.SetResponse << 8 | (int)SetResponseType.WithList,
                     "set-response-with-list");

            GXDLMSTranslator.AddTag(list, Command.AccessRequest,
                                    "access-request");
            list.Add((int)Command.AccessRequest << 8 | (int)AccessServiceCommandType.Get,
                     "access-request-get");
            list.Add((int)Command.AccessRequest << 8 | (int)AccessServiceCommandType.Set,
                     "access-request-set");
            list.Add((int)
                     Command.AccessRequest << 8 | (int)AccessServiceCommandType.Action,
                     "access-request-action");
            GXDLMSTranslator.AddTag(list, Command.AccessResponse,
                                    "access-response");
            list.Add((int)Command.AccessResponse << 8 | (int)AccessServiceCommandType.Get,
                     "access-response-get");
            list.Add((int)Command.AccessResponse << 8 | (int)AccessServiceCommandType.Set,
                     "access-response-set");
            list.Add((int)Command.AccessResponse << 8 | (int)AccessServiceCommandType.Action,
                     "access-response-action");

            list.Add((int)TranslatorTags.AccessRequestBody, "access-request-body");
            list.Add((int)TranslatorTags.ListOfAccessRequestSpecification,
                     "access-request-specification");
            list.Add((int)TranslatorTags.AccessRequestSpecification,
                     "Access-Request-Specification");
            list.Add((int)TranslatorTags.AccessRequestListOfData,
                     "access-request-list-of-data");

            list.Add((int)TranslatorTags.AccessResponseBody, "access-response-body");
            list.Add((int)TranslatorTags.ListOfAccessResponseSpecification,
                     "access-response-specification");
            list.Add((int)TranslatorTags.AccessResponseSpecification,
                     "Access-Response-Specification");
            list.Add((int)TranslatorTags.AccessResponseListOfData,
                     "access-response-list-of-data");

            list.Add((int)TranslatorTags.Service, "service");
            list.Add((int)TranslatorTags.ServiceError, "service-error");
            GXDLMSTranslator.AddTag(list, Command.GeneralBlockTransfer, "general-block-transfer");
            GXDLMSTranslator.AddTag(list, Command.GatewayRequest, "gateway-request");
            GXDLMSTranslator.AddTag(list, Command.GatewayResponse, "gateway-response");
        }

        /// <summary>
        /// Get glo tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetGloTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            GXDLMSTranslator.AddTag(list, Command.GloInitiateRequest, "glo-initiate-request");
            GXDLMSTranslator.AddTag(list, Command.GloInitiateResponse, "glo-initiate-response");
            GXDLMSTranslator.AddTag(list, Command.GloGetRequest, "glo-get-request");
            GXDLMSTranslator.AddTag(list, Command.GloGetResponse, "glo-get-response");
            GXDLMSTranslator.AddTag(list, Command.GloSetRequest, "glo-set-request");
            GXDLMSTranslator.AddTag(list, Command.GloSetResponse, "glo-set-response");
            GXDLMSTranslator.AddTag(list, Command.GloMethodRequest, "glo-action-request");
            GXDLMSTranslator.AddTag(list, Command.GloMethodResponse, "glo-action-response");
            GXDLMSTranslator.AddTag(list, Command.GloReadRequest, "glo-read-request");
            GXDLMSTranslator.AddTag(list, Command.GloReadResponse, "glo-read-response");
            GXDLMSTranslator.AddTag(list, Command.GloWriteRequest, "glo-write-request");
            GXDLMSTranslator.AddTag(list, Command.GloWriteResponse, "glo-write-response");
            GXDLMSTranslator.AddTag(list, Command.GeneralGloCiphering, "general-glo-ciphering");
        }

        /// <summary>
        /// Get ded tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetDedTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            GXDLMSTranslator.AddTag(list, Command.DedGetRequest, "ded-get-request");
            GXDLMSTranslator.AddTag(list, Command.DedGetResponse, "ded-get-response");
            GXDLMSTranslator.AddTag(list, Command.DedSetRequest, "ded-set-request");
            GXDLMSTranslator.AddTag(list, Command.DedSetResponse, "ded-set-response");
            GXDLMSTranslator.AddTag(list, Command.DedMethodRequest, "ded-action-request");
            GXDLMSTranslator.AddTag(list, Command.DedMethodResponse, "ded-action-response");
            GXDLMSTranslator.AddTag(list, Command.GeneralDedCiphering, "general-ded-ciphering");
        }

        /// <summary>
        /// Get translator tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetTranslatorTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            GXDLMSTranslator.AddTag(list, TranslatorTags.Wrapper, "Wrapper");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Hdlc, "Hdlc");
            GXDLMSTranslator.AddTag(list, TranslatorTags.PduDlms, "xDLMS-APDU");
            GXDLMSTranslator.AddTag(list, TranslatorTags.PduCse, "aCSE-APDU");
            GXDLMSTranslator.AddTag(list, TranslatorTags.TargetAddress,
                                    "TargetAddress");
            GXDLMSTranslator.AddTag(list, TranslatorTags.SourceAddress,
                                    "SourceAddress");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorTags.ListOfVariableAccessSpecification,
                                    "variable-access-specification");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ListOfData,
                                    "list-of-data");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Success, "Success");
            GXDLMSTranslator.AddTag(list, TranslatorTags.DataAccessError,
                                    "data-access-result");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AttributeDescriptor,
                                    "cosem-attribute-descriptor");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ClassId, "class-id");
            GXDLMSTranslator.AddTag(list, TranslatorTags.InstanceId,
                                    "instance-id");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AttributeId,
                                    "attribute-id");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorTags.MethodInvocationParameters,
                                    "method-invocation-parameters");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Selector, "selector");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Parameter, "parameter");
            GXDLMSTranslator.AddTag(list, TranslatorTags.LastBlock, "LastBlock");
            GXDLMSTranslator.AddTag(list, TranslatorTags.BlockNumber,
                                    "block-number");
            GXDLMSTranslator.AddTag(list, TranslatorTags.RawData, "RawData");
            GXDLMSTranslator.AddTag(list, TranslatorTags.MethodDescriptor,
                                    "cosem-method-descriptor");
            GXDLMSTranslator.AddTag(list, TranslatorTags.MethodId, "method-id");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Result, "result");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ReturnParameters,
                                    "return-parameters");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AccessSelection,
                                    "access-selection");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Value, "value");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AccessSelector,
                                    "access-selector");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AccessParameters,
                                    "access-parameters");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AttributeDescriptorList,
                                    "AttributeDescriptorList");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorTags.AttributeDescriptorWithSelection,
                                    "AttributeDescriptorWithSelection");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ReadDataBlockAccess,
                                    "ReadDataBlockAccess");
            GXDLMSTranslator.AddTag(list, TranslatorTags.WriteDataBlockAccess,
                                    "WriteDataBlockAccess");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Data, "data");
            GXDLMSTranslator.AddTag(list, TranslatorTags.InvokeId,
                                    "invoke-id-and-priority");
            GXDLMSTranslator.AddTag(list, TranslatorTags.LongInvokeId,
                                    "long-invoke-id-and-priority");
            GXDLMSTranslator.AddTag(list, TranslatorTags.DateTime, "date-time");
            GXDLMSTranslator.AddTag(list, TranslatorTags.CurrentTime, "current-time");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Time, "time");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Reason, "Reason");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorTags.VariableAccessSpecification,
                                    "Variable-Access-Specification");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Choice, "CHOICE");
            GXDLMSTranslator.AddTag(list, TranslatorTags.NotificationBody, "notification-body");
            GXDLMSTranslator.AddTag(list, TranslatorTags.DataValue, "data-value");
            GXDLMSTranslator.AddTag(list, TranslatorTags.InitiateError, "initiateError");
            GXDLMSTranslator.AddTag(list, TranslatorTags.CipheredService, "ciphered-content");
            GXDLMSTranslator.AddTag(list, TranslatorTags.SystemTitle, "system-title");
            GXDLMSTranslator.AddTag(list, TranslatorTags.DataBlock, "DataBlock");
            GXDLMSTranslator.AddTag(list, TranslatorTags.TransactionId, "TransactionId");
            GXDLMSTranslator.AddTag(list, TranslatorTags.OriginatorSystemTitle, "OriginatorSystemTitle");
            GXDLMSTranslator.AddTag(list, TranslatorTags.RecipientSystemTitle, "RecipientSystemTitle");
            GXDLMSTranslator.AddTag(list, TranslatorTags.OtherInformation, "OtherInformation");
            GXDLMSTranslator.AddTag(list, TranslatorTags.KeyInfo, "KeyInfo");
            GXDLMSTranslator.AddTag(list, TranslatorTags.CipheredContent, "CipheredContent");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AgreedKey, "AgreedKey");
            GXDLMSTranslator.AddTag(list, TranslatorTags.KeyParameters, "KeyParameters");
            GXDLMSTranslator.AddTag(list, TranslatorTags.KeyCipheredData, "KeyCipheredData");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AttributeValue, "attribute-value");
            GXDLMSTranslator.AddTag(list, TranslatorTags.MaxInfoRX, "MaxInfoRX");
            GXDLMSTranslator.AddTag(list, TranslatorTags.MaxInfoTX, "MaxInfoTX");
            GXDLMSTranslator.AddTag(list, TranslatorTags.WindowSizeRX, "WindowSizeRX");
            GXDLMSTranslator.AddTag(list, TranslatorTags.WindowSizeTX, "WindowSizeTX");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ValueList, "value-list");
            GXDLMSTranslator.AddTag(list, TranslatorTags.DataAccessResult, "data-access-result");
            GXDLMSTranslator.AddTag(list, TranslatorTags.BlockControl, "block-control");
            GXDLMSTranslator.AddTag(list, TranslatorTags.BlockNumberAck, "block-number-ack");
            GXDLMSTranslator.AddTag(list, TranslatorTags.BlockData, "block-data");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ContentsDescription, "contents-description");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ArrayContents, "array-contents");
            GXDLMSTranslator.AddTag(list, TranslatorTags.NetworkId, "network-id");
            GXDLMSTranslator.AddTag(list, TranslatorTags.PhysicalDeviceAddress, "physical-device-address");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ProtocolVersion, "protocol-version");
            GXDLMSTranslator.AddTag(list, TranslatorTags.CalledAPTitle, "called-ap-title");
            GXDLMSTranslator.AddTag(list, TranslatorTags.CalledAPInvocationId, "called-ap-invocation-id");            
            GXDLMSTranslator.AddTag(list, TranslatorTags.CalledAEInvocationId, "called-ae-invocation-id");
            GXDLMSTranslator.AddTag(list, TranslatorTags.CallingApInvocationId, "calling-ap-invocation-id");
            GXDLMSTranslator.AddTag(list, TranslatorTags.CalledAEQualifier, "called-ae-qualifier");
        }

        public static void GetDataTypeTags(SortedList<int, string> list)
        {
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.None,
                     "null-data");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Array,
                     "array");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Bcd, "bcd");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.BitString,
                     "bit-string");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Boolean,
                     "boolean");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.CompactArray,
                     "compact-array");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Date, "date");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.DateTime,
                     "date-time");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Enum, "enum");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Float32,
                     "float32");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Float64,
                     "float64,");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int16, "long");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int32,
                     "double-long");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int64,
                     "long64");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int8,
                     "integer");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.OctetString,
                     "octet-string");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.String,
                     "visible-string");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.StringUTF8,
                     "utf8-string");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Structure,
                     "structure");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Time, "time");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt16,
                     "long-unsigned");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt32,
                     "double-long-unsigned");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt64,
                     "long64-unsigned");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt8,
                     "unsigned");
        }

        public static String ErrorCodeToString(ErrorCode value)
        {
            String str;
            switch (value)
            {
                case ErrorCode.AccessViolated:
                    str = "scope-of-access-violated";
                    break;
                case ErrorCode.DataBlockNumberInvalid:
                    str = "data-block-number-invalid";
                    break;
                case ErrorCode.DataBlockUnavailable:
                    str = "data-block-unavailable";
                    break;
                case ErrorCode.HardwareFault:
                    str = "hardware-fault";
                    break;
                case ErrorCode.InconsistentClass:
                    str = "object-class-inconsistent";
                    break;
                case ErrorCode.LongGetOrReadAborted:
                    str = "long-Get-aborted";
                    break;
                case ErrorCode.LongSetOrWriteAborted:
                    str = "long-set-aborted";
                    break;
                case ErrorCode.NoLongGetOrReadInProgress:
                    str = "no-long-Get-in-progress";
                    break;
                case ErrorCode.NoLongSetOrWriteInProgress:
                    str = "no-long-set-in-progress";
                    break;
                case ErrorCode.Ok:
                    str = "success";
                    break;
                case ErrorCode.OtherReason:
                    str = "other-reason";
                    break;
                case ErrorCode.ReadWriteDenied:
                    str = "read-write-denied";
                    break;
                case ErrorCode.TemporaryFailure:
                    str = "temporary-failure";
                    break;
                case ErrorCode.UnavailableObject:
                    str = "object-unavailable";
                    break;
                case ErrorCode.UndefinedObject:
                    str = "object-undefined";
                    break;
                case ErrorCode.UnmatchedType:
                    str = "type-unmatched";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Error code: " + value);
            }
            return str;
        }

        public static ErrorCode ValueOfErrorCode(String value)
        {
            ErrorCode v;
            if ("scope-of-access-violated".CompareTo(value) == 0)
            {
                v = ErrorCode.AccessViolated;
            }
            else if ("data-block-number-invalid".CompareTo(value) == 0)
            {
                v = ErrorCode.DataBlockNumberInvalid;
            }
            else if ("data-block-unavailable".CompareTo(value) == 0)
            {
                v = ErrorCode.DataBlockUnavailable;
            }
            else if ("hardware-fault".CompareTo(value) == 0)
            {
                v = ErrorCode.HardwareFault;
            }
            else if ("object-class-inconsistent".CompareTo(value) == 0)
            {
                v = ErrorCode.InconsistentClass;
            }
            else if ("long-Get-aborted".CompareTo(value) == 0)
            {
                v = ErrorCode.LongGetOrReadAborted;
            }
            else if ("long-set-aborted".CompareTo(value) == 0)
            {
                v = ErrorCode.LongSetOrWriteAborted;
            }
            else if ("no-long-Get-in-progress".CompareTo(value) == 0)
            {
                v = ErrorCode.NoLongGetOrReadInProgress;
            }
            else if ("no-long-set-in-progress".CompareTo(value) == 0)
            {
                v = ErrorCode.NoLongSetOrWriteInProgress;
            }
            else if ("success".CompareTo(value) == 0)
            {
                v = ErrorCode.Ok;
            }
            else if ("other-reason".CompareTo(value) == 0)
            {
                v = ErrorCode.OtherReason;
            }
            else if ("read-write-denied".CompareTo(value) == 0)
            {
                v = ErrorCode.ReadWriteDenied;
            }
            else if ("temporary-failure".CompareTo(value) == 0)
            {
                v = ErrorCode.TemporaryFailure;
            }
            else if ("object-unavailable".CompareTo(value) == 0)
            {
                v = ErrorCode.UnavailableObject;
            }
            else if ("object-undefined".CompareTo(value) == 0)
            {
                v = ErrorCode.UndefinedObject;
            }
            else if ("type-unmatched".CompareTo(value) == 0)
            {
                v = ErrorCode.UnmatchedType;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Error code: " + value);
            }
            return v;
        }

        private static Dictionary<ServiceError, String> GetServiceErrors()
        {
            Dictionary<ServiceError, String> list = new Dictionary<ServiceError, String>();
            list.Add(ServiceError.ApplicationReference, "application-reference");
            list.Add(ServiceError.HardwareResource, "hardware-resource");
            list.Add(ServiceError.VdeStateError, "vde-state-error");
            list.Add(ServiceError.Service, "service");
            list.Add(ServiceError.Definition, "definition");
            list.Add(ServiceError.Access, "access");
            list.Add(ServiceError.Initiate, "initiate");
            list.Add(ServiceError.LoadDataSet, "load-data-set");
            list.Add(ServiceError.Task, "task");
            return list;
        }

        static Dictionary<ApplicationReference, String> GetApplicationReference()
        {
            Dictionary<ApplicationReference, String> list =
                new Dictionary<ApplicationReference, String>();
            list.Add(ApplicationReference.ApplicationContextUnsupported,
                     "application-context-unsupported");
            list.Add(ApplicationReference.ApplicationReferenceInvalid,
                     "application-reference-invalid");
            list.Add(ApplicationReference.ApplicationUnreachable,
                     "application-unreachable");
            list.Add(ApplicationReference.DecipheringError, "deciphering-error");
            list.Add(ApplicationReference.Other, "other");
            list.Add(ApplicationReference.ProviderCommunicationError,
                     "provider-communication-error");
            list.Add(ApplicationReference.TimeElapsed, "time-elapsed");
            return list;
        }

        static Dictionary<HardwareResource, String> GetHardwareResource()
        {
            Dictionary<HardwareResource, String> list =
                new Dictionary<HardwareResource, String>();
            list.Add(HardwareResource.MassStorageUnavailable,
                     "mass-storage-unavailable");
            list.Add(HardwareResource.MemoryUnavailable, "memory-unavailable");
            list.Add(HardwareResource.Other, "other");
            list.Add(HardwareResource.OtherResourceUnavailable,
                     "other-resource-unavailable");
            list.Add(HardwareResource.ProcessorResourceUnavailable,
                     "processor-resource-unavailable");
            return list;
        }

        static Dictionary<VdeStateError, String> GetVdeStateError()
        {
            Dictionary<VdeStateError, String> list = new Dictionary<VdeStateError, String>();
            list.Add(VdeStateError.LoadingDataSet, "loading-data-set");
            list.Add(VdeStateError.NoDlmsContext, "no-dlms-context");
            list.Add(VdeStateError.Other, "other");
            list.Add(VdeStateError.StatusInoperable, "status-inoperable");
            list.Add(VdeStateError.StatusNochange, "status-nochange");
            return list;
        }

        static Dictionary<Service, String> GetService()
        {
            Dictionary<Service, String> list = new Dictionary<Service, String>();
            list.Add(Service.Other, "other");
            list.Add(Service.PduSize, "pdu-size");
            list.Add(Service.Unsupported, "service-unsupported");
            return list;
        }

        static Dictionary<Definition, String> GetDefinition()
        {
            Dictionary<Definition, String> list = new Dictionary<Definition, String>();
            list.Add(Definition.ObjectAttributeInconsistent,
                     "object-attribute-inconsistent");
            list.Add(Definition.ObjectClassInconsistent,
                     "object-class-inconsistent");
            list.Add(Definition.ObjectUndefined, "object-undefined");
            list.Add(Definition.Other, "other");
            return list;
        }

        static Dictionary<Access, String> GetAccess()
        {
            Dictionary<Access, String> list = new Dictionary<Access, String>();
            list.Add(Access.HardwareFault, "hardware-fault");
            list.Add(Access.ObjectAccessInvalid, "object-access-violated");
            list.Add(Access.ObjectUnavailable, "object-unavailable");
            list.Add(Access.Other, "other");
            list.Add(Access.ScopeOfAccessViolated, "scope-of-access-violated");
            return list;
        }

        static Dictionary<Initiate, String> GetInitiate()
        {
            Dictionary<Initiate, String> list = new Dictionary<Initiate, String>();
            list.Add(Initiate.DlmsVersionTooLow, "dlms-version-too-low");
            list.Add(Initiate.IncompatibleConformance, "incompatible-conformance");
            list.Add(Initiate.Other, "other");
            list.Add(Initiate.PduSizeTooShort, "pdu-size-too-short");
            list.Add(Initiate.RefusedByTheVDEHandler,
                     "refused-by-the-VDE-Handler");
            return list;
        }

        static Dictionary<LoadDataSet, String> GetLoadDataSet()
        {
            Dictionary<LoadDataSet, String> list = new Dictionary<LoadDataSet, String>();
            list.Add(LoadDataSet.DatasetNotReady, "data-set-not-ready");
            list.Add(LoadDataSet.DatasetSizeTooLarge, "dataset-size-too-large");
            list.Add(LoadDataSet.InterpretationFailure, "interpretation-failure");
            list.Add(LoadDataSet.NotAwaitedSegment, "not-awaited-segment");
            list.Add(LoadDataSet.NotLoadable, "not-loadable");
            list.Add(LoadDataSet.Other, "other");
            list.Add(LoadDataSet.PrimitiveOutOfSequence,
                     "primitive-out-of-sequence");
            list.Add(LoadDataSet.StorageFailure, "storage-failure");
            return list;
        }

        static Dictionary<Task, String> GetTask()
        {
            Dictionary<Task, String> list = new Dictionary<Task, String>();
            list.Add(Task.NoRemoteControl, "no-remote-control");
            list.Add(Task.Other, "other");
            list.Add(Task.TiRunning, "ti-running");
            list.Add(Task.TiStopped, "ti-stopped");
            list.Add(Task.TiUnusable, "ti-unusable");
            return list;
        }

        internal static String GetServiceErrorValue(ServiceError error,
                byte value)
        {
            switch (error)
            {
                case ServiceError.ApplicationReference:
                    return GetApplicationReference()
                           [(ApplicationReference)value];
                case ServiceError.HardwareResource:
                    return GetHardwareResource()[(HardwareResource)value];
                case ServiceError.VdeStateError:
                    return GetVdeStateError()[(VdeStateError)value];
                case ServiceError.Service:
                    return GetService()[(Service)value];
                case ServiceError.Definition:
                    return GetDefinition()[(Definition)value];
                case ServiceError.Access:
                    return GetAccess()[(Access)value];
                case ServiceError.Initiate:
                    return GetInitiate()[(Initiate)value];
                case ServiceError.LoadDataSet:
                    return GetLoadDataSet()[(LoadDataSet)value];
                case ServiceError.Task:
                    return GetTask()[(Task)value];
                case ServiceError.OtherError:
                    return value.ToString();
                default:
                    break;
            }
            return "";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="error">Service error enumeration value.</param>
        /// <returns>Service error standard XML tag.</returns>
        internal static String ServiceErrorToString(ServiceError error)
        {
            return GetServiceErrors()[error];
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value">Service error standard XML tag.</param>
        /// <returns>Service error enumeration value.</returns>
        internal static ServiceError GetServiceError(String value)
        {
            foreach (var it in GetServiceErrors())
            {
                if (string.Compare(value, it.Value, true) == 0)
                {
                    return it.Key;
                }
            }
            throw new ArgumentException();
        }

        private static int GetApplicationReference(String value)
        {
            int ret = -1;
            foreach (var it in GetApplicationReference())
            {
                if (string.Compare(value, it.Value, true) == 0)
                {
                    ret = (int)it.Key;
                    break;
                }
            }
            if (ret == -1)
            {
                throw new ArgumentException();
            }
            return ret;
        }

        private static int GetHardwareResource(String value)
        {
            int ret = -1;
            foreach (var it in GetHardwareResource())
            {
                if (string.Compare(value, it.Value, true) == 0)
                {
                    ret = (int)it.Key;
                    break;
                }
            }
            if (ret == -1)
            {
                throw new ArgumentException();
            }
            return ret;
        }

        private static int GetVdeStateError(String value)
        {
            int ret = -1;
            foreach (var it in GetVdeStateError())
            {
                if (string.Compare(value, it.Value, true) == 0)
                {
                    ret = (int)it.Key;
                    break;
                }
            }
            if (ret == -1)
            {
                throw new ArgumentException();
            }
            return ret;
        }

        private static int GetService(String value)
        {
            int ret = -1;
            foreach (var it in GetService())
            {
                if (string.Compare(value, it.Value, true) == 0)
                {
                    ret = (int)it.Key;
                    break;
                }
            }
            if (ret == -1)
            {
                throw new ArgumentException();
            }
            return ret;
        }

        private static int GetDefinition(String value)
        {
            int ret = -1;
            foreach (var it in GetDefinition())
            {
                if (string.Compare(value, it.Value, true) == 0)
                {
                    ret = (int)it.Key;
                    break;
                }
            }
            if (ret == -1)
            {
                throw new ArgumentException();
            }
            return ret;
        }

        private static int GetAccess(String value)
        {
            int ret = -1;
            foreach (var it in GetAccess())
            {
                if (string.Compare(value, it.Value, true) == 0)
                {
                    ret = (int)it.Key;
                    break;
                }
            }
            if (ret == -1)
            {
                throw new ArgumentException();
            }
            return ret;
        }

        private static int GetInitiate(String value)
        {
            int ret = -1;
            foreach (var it in GetInitiate())
            {
                if (string.Compare(value, it.Value, true) == 0)
                {
                    ret = (int)it.Key;
                    break;
                }
            }
            if (ret == -1)
            {
                throw new ArgumentException();
            }
            return ret;
        }

        private static int GetLoadDataSet(String value)
        {
            int ret = -1;
            foreach (var it in GetLoadDataSet())
            {
                if (string.Compare(value, it.Value, true) == 0)
                {
                    ret = (int)it.Key;
                    break;
                }
            }
            if (ret == -1)
            {
                throw new ArgumentException();
            }
            return ret;
        }

        private static int GetTask(String value)
        {
            int ret = -1;
            foreach (var it in GetTask())
            {
                if (string.Compare(value, it.Value, true) == 0)
                {
                    ret = (int)it.Key;
                    break;
                }
            }
            if (ret == -1)
            {
                throw new ArgumentException();
            }
            return ret;
        }

        internal static byte GetError(ServiceError serviceError, String value)
        {
            int ret = 0;
            switch (serviceError)
            {
                case ServiceError.ApplicationReference:
                    ret = GetApplicationReference(value);
                    break;
                case ServiceError.HardwareResource:
                    ret = GetHardwareResource(value);
                    break;
                case ServiceError.VdeStateError:
                    ret = GetVdeStateError(value);
                    break;
                case ServiceError.Service:
                    ret = GetService(value);
                    break;
                case ServiceError.Definition:
                    ret = GetDefinition(value);
                    break;
                case ServiceError.Access:
                    ret = GetAccess(value);
                    break;
                case ServiceError.Initiate:
                    ret = GetInitiate(value);
                    break;
                case ServiceError.LoadDataSet:
                    ret = GetLoadDataSet(value);
                    break;
                case ServiceError.Task:
                    ret = GetTask(value);
                    break;
                case ServiceError.OtherError:
                    ret = int.Parse(value);
                    break;
                default:
                    break;
            }
            return (byte)ret;
        }

        internal static Conformance ValueOfConformance(string value)
        {
            Conformance ret;
            if (string.Compare("access", value, true) == 0)
            {
                ret = Conformance.Access;
            }
            else if (string.Compare("action", value, true) == 0)
            {
                ret = Conformance.Action;
            }
            else if (string.Compare("attribute0-supported-with-get", value, true) == 0)
            {
                ret = Conformance.Attribute0SupportedWithGet;
            }
            else if (string.Compare("attribute0-supported-with-set", value, true) == 0)
            {
                ret = Conformance.Attribute0SupportedWithSet;
            }
            else if (string.Compare("block-transfer-with-action", value, true) == 0)
            {
                ret = Conformance.BlockTransferWithAction;
            }
            else if (string.Compare("block-transfer-with-get-or-read", value, true) == 0)
            {
                ret = Conformance.BlockTransferWithGetOrRead;
            }
            else if (string.Compare("block-transfer-with-set-or-write", value, true) == 0)
            {
                ret = Conformance.BlockTransferWithSetOrWrite;
            }
            else if (string.Compare("data-notification", value, true) == 0)
            {
                ret = Conformance.DataNotification;
            }
            else if (string.Compare("event-notification", value, true) == 0)
            {
                ret = Conformance.EventNotification;
            }
            else if (string.Compare("general-block-transfer", value, true) == 0)
            {
                ret = Conformance.GeneralBlockTransfer;
            }
            else if (string.Compare("general-protection", value, true) == 0)
            {
                ret = Conformance.GeneralProtection;
            }
            else if (string.Compare("get", value, true) == 0)
            {
                ret = Conformance.Get;
            }
            else if (string.Compare("information-report", value, true) == 0)
            {
                ret = Conformance.InformationReport;
            }
            else if (string.Compare("multiple-references", value, true) == 0)
            {
                ret = Conformance.MultipleReferences;
            }
            else if (string.Compare("parameterized-access", value, true) == 0)
            {
                ret = Conformance.ParameterizedAccess;
            }
            else if (string.Compare("priority-mgmt-supported", value, true) == 0)
            {
                ret = Conformance.PriorityMgmtSupported;
            }
            else if (string.Compare("read", value, true) == 0)
            {
                ret = Conformance.Read;
            }
            else if (string.Compare("reserved-seven", value, true) == 0)
            {
                ret = Conformance.ReservedSeven;
            }
            else if (string.Compare("reserved-six", value, true) == 0)
            {
                ret = Conformance.ReservedSix;
            }
            else if (string.Compare("reserved-zero", value, true) == 0)
            {
                ret = Conformance.ReservedZero;
            }
            else if (string.Compare("selective-access", value, true) == 0)
            {
                ret = Conformance.SelectiveAccess;
            }
            else if (string.Compare("set", value, true) == 0)
            {
                ret = Conformance.Set;
            }
            else if (string.Compare("unconfirmed-write", value, true) == 0)
            {
                ret = Conformance.UnconfirmedWrite;
            }
            else if (string.Compare("write", value, true) == 0)
            {
                ret = Conformance.Write;
            }
            else
            {
                throw new ArgumentOutOfRangeException(value);
            }
            return ret;
        }

        public static string ReleaseResponseReasonToString(ReleaseResponseReason value)
        {
            String str;
            switch (value)
            {
                case ReleaseResponseReason.Normal:
                    str = "Normal";
                    break;
                case ReleaseResponseReason.NotFinished:
                    str = "not-finished";
                    break;
                case ReleaseResponseReason.UserDefined:
                    str = "user-defined";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(value.ToString());
            }
            return str;
        }

        public static ReleaseResponseReason ValueOfReleaseResponseReason(string value)
        {
            ReleaseResponseReason ret;
            if (string.Compare(value, "normal", true) == 0)
            {
                ret = ReleaseResponseReason.Normal;
            }
            else if (string.Compare(value, "not-finished", true) == 0)
            {
                ret = ReleaseResponseReason.NotFinished;
            }
            else if (string.Compare(value, "user-defined", true) == 0)
            {
                ret = ReleaseResponseReason.UserDefined;
            }
            else
            {
                throw new ArgumentOutOfRangeException(value);
            }
            return ret;
        }

        public static String ReleaseRequestReasonToString(ReleaseRequestReason value)
        {
            String str;
            switch (value)
            {
                case ReleaseRequestReason.Normal:
                    str = "normal";
                    break;
                case ReleaseRequestReason.Urgent:
                    str = "urgent";
                    break;
                case ReleaseRequestReason.UserDefined:
                    str = "user-defined";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(value.ToString());
            }
            return str;
        }

        public static ReleaseRequestReason ValueOfReleaseRequestReason(string value)
        {
            ReleaseRequestReason ret;
            if (string.Compare(value, "normal", true) == 0)
            {
                ret = ReleaseRequestReason.Normal;
            }
            else if (string.Compare(value, "urgent", true) == 0)
            {
                ret = ReleaseRequestReason.Urgent;
            }
            else if (string.Compare(value, "user-defined", true) == 0)
            {
                ret = ReleaseRequestReason.UserDefined;
            }
            else
            {
                throw new ArgumentOutOfRangeException(value);
            }
            return ret;
        }
    }
}