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

namespace Gurux.DLMS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Gurux.DLMS.Objects;
    using Gurux.DLMS.Enums;
    using Gurux.DLMS.Internal;

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
            GXDLMSTranslator.AddTag(list, Command.Ua, "Ua");
            GXDLMSTranslator.AddTag(list, Command.Aarq, "x:aarq");
            GXDLMSTranslator.AddTag(list, Command.Aare, "x:aare");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ApplicationContextName,
                                    "x:application-context-name");
            GXDLMSTranslator.AddTag(list, Command.InitiateResponse,
                                    "InitiateResponse");
            GXDLMSTranslator.AddTag(list, Command.InitiateRequest,
                                    "x:user-information");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.NegotiatedQualityOfService,
                                    "x:negotiated-quality-of-service");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ProposedQualityOfService,
                                    "x:proposed-quality-of-service");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ProposedDlmsVersionNumber,
                                    "x:proposed-dlms-version-number");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ProposedMaxPduSize,
                                    "x:client-max-receive-pdu-size");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ProposedConformance,
                                    "x:proposed-conformance");
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
                                    "x:sender-acse-requirements");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ResponderACSERequirement,
                                    "x:responder-acse-requirements");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.RespondingMechanismName,
                                    "x:mechanism-name");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.CallingMechanismName,
                                    "x:mechanism-name");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.CallingAuthentication,
                                    "x:calling-authentication-value");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.RespondingAuthentication,
                                    "x:responding-authentication-value");
            GXDLMSTranslator.AddTag(list, Command.ReleaseRequest,
                                    "ReleaseRequest");
            GXDLMSTranslator.AddTag(list, Command.ReleaseResponse,
                                    "ReleaseResponse");
            GXDLMSTranslator.AddTag(list, Command.DisconnectRequest, "Disc");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.AssociationResult,
                                    "x:result");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorGeneralTags.ResultSourceDiagnostic,
                                    "x:result-source-diagnostic");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.ACSEServiceUser,
                                    "x:acse-service-user");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.CallingAPTitle,
                                    "CallingAPTitle");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.RespondingAPTitle,
                                    "RespondingAPTitle");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.CharString,
                                    "x:charstring");
        }

        /// <summary>
        /// Get SN tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetSnTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            list.Add((int)Command.ReadRequest, "x:readRequest");
            list.Add((int)Command.WriteRequest, "x:writeRequest");
            list.Add((int)Command.WriteResponse, "x:writeResponse");
            list.Add((int)Command.WriteRequest << 8 | (int)SingleReadResponse.Data,
                     "x:Data");
            list.Add(
                (int)Command.ReadRequest << 8
                | (int)VariableAccessSpecification.VariableName,
                "x:variable-name");
            list.Add(
                (int)Command.ReadRequest << 8
                | (int)VariableAccessSpecification.ParameterisedAccess,
                "x:parameterized-access");
            list.Add(
                (int)Command.ReadRequest << 8
                | (int)VariableAccessSpecification.BlockNumberAccess,
                "BlockNumberAccess");
            list.Add(
                (int)Command.WriteRequest << 8
                | (int)VariableAccessSpecification.VariableName,
                "x:variable-name");

            list.Add((int)Command.ReadResponse, "x:readResponse");
            list.Add(
                (int)Command.ReadResponse << 8
                | (int)SingleReadResponse.DataBlockResult,
                "DataBlockResult");
            list.Add((int)Command.ReadResponse << 8 | (int)SingleReadResponse.Data,
                     "x:data");
            list.Add((int)Command.WriteResponse << 8 | (int)SingleReadResponse.Data,
                     "x:data");
            list.Add(
                (int)Command.ReadResponse << 8
                | (int)SingleReadResponse.DataAccessError,
                "x:data-access-error");
        }

        /// <summary>
        /// Get LN tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetLnTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            GXDLMSTranslator.AddTag(list, Command.GetRequest, "x:get-request");
            list.Add((int)Command.GetRequest << 8 | (int)GetCommandType.Normal,
                     "x:get-request-normal");
            list.Add((int)Command.GetRequest << 8 | (int)GetCommandType.NextDataBlock,
                     "x:get-request-next");
            list.Add((int)Command.GetRequest << 8 | (int)GetCommandType.WithList,
                     "x:get-request-with-list");
            GXDLMSTranslator.AddTag(list, Command.SetRequest, "x:set-request");
            list.Add((int)Command.SetRequest << 8 | (int)SetRequestType.Normal,
                     "x:set-request-normal");
            list.Add((int)Command.SetRequest << 8 | (int)SetRequestType.FirstDataBlock,
                     "x:set-request-first-data-block");
            list.Add((int)Command.SetRequest << 8 | (int)SetRequestType.WithDataBlock,
                     "x:set-request-with-data-block");
            list.Add((int)Command.SetRequest << 8 | (int)SetRequestType.WithList,
                     "x:set-request-with-list");
            GXDLMSTranslator.AddTag(list, Command.MethodRequest,
                                    "x:action-request");
            list.Add((int)Command.MethodRequest << 8 | (int)ActionRequestType.Normal,
                     "x:action-request-normal");
            list.Add((int)Command.MethodRequest << 8 | (int)ActionRequestType.NextBlock,
                     "ActionRequestForNextDataBlock");
            list.Add((int)Command.MethodRequest << 8 | (int)ActionRequestType.WithList,
                     "x:action-request-with-list");
            GXDLMSTranslator.AddTag(list, Command.MethodResponse,
                                    "x:action-response");
            list.Add((int)Command.MethodResponse << 8 | (int)ActionResponseType.Normal,
                     "x:action-response-normal");
            list.Add((int)Command.MethodResponse << 8 | (int)ActionResponseType.WithFirstBlock,
                     "x:action-response-with-first-block");
            list.Add((int)Command.MethodResponse << 8 | (int)ActionResponseType.WithList,
                     "x:action-response-with-list");
            list.Add((int)TranslatorTags.SingleResponse, "x:single-response");
            list.Add((int)Command.DataNotification, "x:data-notification");
            GXDLMSTranslator.AddTag(list, Command.GetResponse, "x:get-response");
            list.Add((int)Command.GetResponse << 8 | (int)GetCommandType.Normal,
                     "x:get-response-normal");
            list.Add((int)Command.GetResponse << 8 | (int)GetCommandType.NextDataBlock,
                     "x:get-response-with-data-block");
            list.Add((int)Command.GetResponse << 8 | (int)GetCommandType.WithList,
                     "x:get-response-with-list");
            GXDLMSTranslator.AddTag(list, Command.SetResponse, "x:set-response");
            list.Add((int)Command.SetResponse << 8 | (int)SetResponseType.Normal,
                     "x:set-response-normal");
            list.Add((int)Command.SetResponse << 8 | (int)SetResponseType.DataBlock,
                     "x:set-response-data-block");
            list.Add((int)Command.SetResponse << 8 | (int)SetResponseType.LastDataBlock,
                     "x:set-response-with-last-data-block");
            list.Add((int)Command.SetResponse << 8 | (int)SetResponseType.WithList,
                     "x:set-response-with-list");

            GXDLMSTranslator.AddTag(list, Command.AccessRequest,
                                    "x:access-request");
            list.Add((int)Command.AccessRequest << 8 | (int)AccessServiceCommandType.Get,
                     "x:access-request-get");
            list.Add((int)Command.AccessRequest << 8 | (int)AccessServiceCommandType.Set,
                     "x:access-request-set");
            list.Add((int)
                     Command.AccessRequest << 8 | (int)AccessServiceCommandType.Action,
                     "x:access-request-action");
            GXDLMSTranslator.AddTag(list, Command.AccessResponse,
                                    "x:access-response");
            list.Add((int)Command.AccessResponse << 8 | (int)AccessServiceCommandType.Get,
                     "x:access-response-get");
            list.Add((int)Command.AccessResponse << 8 | (int)AccessServiceCommandType.Set,
                     "x:access-response-set");
            list.Add((int)Command.AccessResponse << 8 | (int)AccessServiceCommandType.Action,
                     "x:access-response-action");

            list.Add((int)TranslatorTags.AccessRequestBody, "x:access-request-body");
            list.Add((int)TranslatorTags.ListOfAccessRequestSpecification,
                     "x:access-request-specification");
            list.Add((int)TranslatorTags.AccessRequestSpecification,
                     "x:Access-Request-Specification");
            list.Add((int)TranslatorTags.AccessRequestListOfData,
                     "x:access-request-list-of-data");

            list.Add((int)TranslatorTags.AccessResponseBody, "x:access-response-body");
            list.Add((int)TranslatorTags.ListOfAccessResponseSpecification,
                     "x:access-response-specification");
            list.Add((int)TranslatorTags.AccessResponseSpecification,
                     "x:Access-Response-Specification");
            list.Add((int)TranslatorTags.AccessResponseListOfData,
                     "x:access-response-list-of-data");
        }

        /// <summary>
        /// Get glo tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetGloTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            GXDLMSTranslator.AddTag(list, Command.GloInitiateRequest, "gloInitiateRequest");
            GXDLMSTranslator.AddTag(list, Command.GloInitiateResponse, "gloInitiateResponse");
            GXDLMSTranslator.AddTag(list, Command.GloGetRequest, "gloGetRequest");
            GXDLMSTranslator.AddTag(list, Command.GloGetResponse, "gloGetResponse");
            GXDLMSTranslator.AddTag(list, Command.GloSetRequest, "gloSetRequest");
            GXDLMSTranslator.AddTag(list, Command.GloSetResponse, "gloSetResponse");
            GXDLMSTranslator.AddTag(list, Command.GloMethodRequest, "gloActionRequest");
            GXDLMSTranslator.AddTag(list, Command.GloMethodResponse, "gloActionResponse");
            GXDLMSTranslator.AddTag(list, Command.GloReadRequest, "gloReadRequest");
            GXDLMSTranslator.AddTag(list, Command.GloReadResponse, "gloReadResponse");
            GXDLMSTranslator.AddTag(list, Command.GloWriteRequest, "gloWriteRequest");
            GXDLMSTranslator.AddTag(list, Command.GloWriteResponse, "gloWriteResponse");
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
            GXDLMSTranslator.AddTag(list, TranslatorTags.PduDlms, "x:xDLMS-APDU");
            GXDLMSTranslator.AddTag(list, TranslatorTags.PduCse, "x:aCSE-APDU");
            GXDLMSTranslator.AddTag(list, TranslatorTags.TargetAddress,
                                    "TargetAddress");
            GXDLMSTranslator.AddTag(list, TranslatorTags.SourceAddress,
                                    "SourceAddress");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorTags.ListOfVariableAccessSpecification,
                                    "x:variable-access-specification");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ListOfData,
                                    "x:list-of-data");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Success, "Success");
            GXDLMSTranslator.AddTag(list, TranslatorTags.DataAccessError,
                                    "x:data-access-result");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AttributeDescriptor,
                                    "x:cosem-attribute-descriptor");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ClassId, "x:class-id");
            GXDLMSTranslator.AddTag(list, TranslatorTags.InstanceId,
                                    "x:instance-id");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AttributeId,
                                    "x:attribute-id");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorTags.MethodInvocationParameters,
                                    "x:method-invocation-parameters");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Selector, "x:selector");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Parameter, "x:parameter");
            GXDLMSTranslator.AddTag(list, TranslatorTags.LastBlock, "LastBlock");
            GXDLMSTranslator.AddTag(list, TranslatorTags.BlockNumber,
                                    "x:block-number");
            GXDLMSTranslator.AddTag(list, TranslatorTags.RawData, "RawData");
            GXDLMSTranslator.AddTag(list, TranslatorTags.MethodDescriptor,
                                    "x:cosem-method-descriptor");
            GXDLMSTranslator.AddTag(list, TranslatorTags.MethodId, "x:method-id");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Result, "x:result");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ReturnParameters,
                                    "x:return-parameters");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AccessSelection,
                                    "x:access-selection");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Value, "x:value");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AccessSelector,
                                    "x:access-selector");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AccessParameters,
                                    "x:access-parameters");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AttributeDescriptorList,
                                    "AttributeDescriptorList");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorTags.AttributeDescriptorWithSelection,
                                    "AttributeDescriptorWithSelection");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ReadDataBlockAccess,
                                    "ReadDataBlockAccess");
            GXDLMSTranslator.AddTag(list, TranslatorTags.WriteDataBlockAccess,
                                    "WriteDataBlockAccess");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Data, "x:data");
            GXDLMSTranslator.AddTag(list, TranslatorTags.InvokeId,
                                    "x:invoke-id-and-priority");
            GXDLMSTranslator.AddTag(list, TranslatorTags.LongInvokeId,
                                    "x:long-invoke-id-and-priority");
            GXDLMSTranslator.AddTag(list, TranslatorTags.DateTime, "x:date-time");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Reason, "Reason");
            GXDLMSTranslator.AddTag(list,
                                    TranslatorTags.VariableAccessSpecification,
                                    "x:Variable-Access-Specification");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Choice, "x:CHOICE");
            GXDLMSTranslator.AddTag(list, TranslatorTags.NotificationBody,
                                    "x:notification-body");
            GXDLMSTranslator.AddTag(list, TranslatorTags.DataValue,
                                    "x:data-value");
        }

        public static void GetDataTypeTags(SortedList<int, string> list)
        {
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.None,
                     "x:null-data");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Array,
                     "x:array");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Bcd, "x:bcd");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.BitString,
                     "x:bit-string");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Boolean,
                     "x:boolean");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.CompactArray,
                     "x:compact-array");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Date, "x:date");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.DateTime,
                     "x:date-time");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Enum, "x:enum");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Float32,
                     "x:float32");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Float64,
                     "x:float64,");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int16, "x:long");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int32,
                     "x:double-long");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int64,
                     "x:long64");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int8,
                     "x:integer");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.OctetString,
                     "x:octet-string");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.String,
                     "x:visible-string");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.StringUTF8,
                     "x:utf8-string");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Structure,
                     "x:structure");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Time, "x:time");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt16,
                     "x:long-unsigned");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt32,
                     "x:double-long-unsigned");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt64,
                     "x:long64-unsigned");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt8,
                     "x:unsigned");
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
                    str = "long-get-aborted";
                    break;
                case ErrorCode.LongSetOrWriteAborted:
                    str = "long-set-aborted";
                    break;
                case ErrorCode.NoLongGetOrReadInProgress:
                    str = "no-long-get-in-progress";
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
            else if ("long-get-aborted".CompareTo(value) == 0)
            {
                v = ErrorCode.LongGetOrReadAborted;
            }
            else if ("long-set-aborted".CompareTo(value) == 0)
            {
                v = ErrorCode.LongSetOrWriteAborted;
            }
            else if ("no-long-get-in-progress".CompareTo(value) == 0)
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
    }
}