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

    class TranslatorSimpleTags
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
            GXDLMSTranslator.AddTag(list, Command.Aarq, "AssociationRequest");
            GXDLMSTranslator.AddTag(list, Command.Aare, "AssociationResponse");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.ApplicationContextName, "ApplicationContextName");
            GXDLMSTranslator.AddTag(list, Command.InitiateResponse, "InitiateResponse");
            GXDLMSTranslator.AddTag(list, Command.InitiateRequest, "InitiateRequest");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.NegotiatedQualityOfService, "NegotiatedQualityOfService");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.ProposedDlmsVersionNumber, "ProposedDlmsVersionNumber");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.ProposedMaxPduSize, "ProposedMaxPduSize");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.ProposedConformance, "ProposedConformance");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.VaaName, "VaaName");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.NegotiatedConformance, "NegotiatedConformance");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.NegotiatedDlmsVersionNumber, "NegotiatedDlmsVersionNumber");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.NegotiatedMaxPduSize, "NegotiatedMaxPduSize");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.ConformanceBit, "ConformanceBit");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.SenderACSERequirements, "SenderACSERequirements");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.ResponderACSERequirement, "ResponderACSERequirement");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.RespondingMechanismName, "MechanismName");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.CallingMechanismName, "MechanismName");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.CallingAuthentication, "CallingAuthentication");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.RespondingAuthentication, "RespondingAuthentication");
            GXDLMSTranslator.AddTag(list, Command.ReleaseRequest, "ReleaseRequest");
            GXDLMSTranslator.AddTag(list, Command.ReleaseResponse, "ReleaseResponse");
            GXDLMSTranslator.AddTag(list, Command.DisconnectRequest, "Disc");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.AssociationResult, "AssociationResult");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.ResultSourceDiagnostic, "ResultSourceDiagnostic");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.ACSEServiceUser, "ACSEServiceUser");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.CallingAPTitle, "CallingAPTitle");
            GXDLMSTranslator.AddTag(list, TranslatorGeneralTags.RespondingAPTitle, "RespondingAPTitle");
            GXDLMSTranslator.AddTag(list, Command.Rejected, "Rejected");
        }

        /// <summary>
        /// Get SN tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetSnTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            list.Add((int)Command.ReadRequest, "ReadRequest");
            list.Add((int)Command.WriteRequest, "WriteRequest");
            list.Add((int)Command.WriteResponse, "WriteResponse");
            list.Add((int)(Command.ReadRequest) << 8 | (byte)VariableAccessSpecification.VariableName, "VariableName");
            list.Add((int)(Command.ReadRequest) << 8 | (byte)VariableAccessSpecification.ParameterisedAccess, "ParameterisedAccess");
            list.Add((int)(Command.ReadRequest) << 8 | (byte)VariableAccessSpecification.BlockNumberAccess, "BlockNumberAccess");
            list.Add(
                (int)Command.WriteRequest << 8
                | (int)VariableAccessSpecification.VariableName,
                "VariableName");
            list.Add((int)Command.ReadResponse, "ReadResponse");
            list.Add((int)(Command.ReadResponse) << 8 | (byte)SingleReadResponse.DataBlockResult, "DataBlockResult");
            list.Add((int)(Command.ReadResponse) << 8 | (byte)SingleReadResponse.Data, "Data");
            GXDLMSTranslator.AddTag(list, Command.GetResponse, "GetResponse");
            list.Add((int)(Command.GetResponse) << 8 | (byte)GetCommandType.Normal, "Normal");
            list.Add((int)(Command.GetResponse) << 8 | (byte)GetCommandType.NextDataBlock, "GetResponsewithDataBlock");
            list.Add((int)(Command.GetResponse) << 8 | (byte)GetCommandType.WithList, "GetResponseWithList");
            GXDLMSTranslator.AddTag(list, Command.SetResponse, "SetResponse");
            list.Add((int)(Command.SetResponse) << 8 | (byte)SetResponseType.Normal, "SetResponseNormal");
            list.Add((int)(Command.SetResponse) << 8 | (byte)SetResponseType.DataBlock, "SetResponseDataBlock");
            list.Add((int)(Command.SetResponse) << 8 | (byte)SetResponseType.LastDataBlock, "SetResponseWithLastDataBlock");
            list.Add((int)(Command.SetResponse) << 8 | (byte)SetResponseType.WithList, "SetResponseWithList");
            list.Add((int)Command.ReadResponse << 8
                     | (int)SingleReadResponse.DataAccessError,
                     "DataAccessError");
        }

        /// <summary>
        /// Get LN tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetLnTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            GXDLMSTranslator.AddTag(list, Command.GetRequest, "GetRequest");
            list.Add((int)(Command.GetRequest) << 8 | (byte)GetCommandType.Normal, "GetRequestNormal");
            list.Add((int)(Command.GetRequest) << 8 | (byte)GetCommandType.NextDataBlock, "GetRequestForNextDataBlock");
            list.Add((int)(Command.GetRequest) << 8 | (byte)GetCommandType.WithList, "GetRequestWithList");
            GXDLMSTranslator.AddTag(list, Command.SetRequest, "SetRequest");
            list.Add((int)(Command.SetRequest) << 8 | (byte)SetRequestType.Normal, "SetRequestNormal");
            list.Add((int)(Command.SetRequest) << 8 | (byte)SetRequestType.FirstDataBlock, "SetRequestFirstDataBlock");
            list.Add((int)(Command.SetRequest) << 8 | (byte)SetRequestType.WithDataBlock, "SetRequestWithDataBlock");
            list.Add((int)(Command.SetRequest) << 8 | (byte)SetRequestType.WithList, "SetRequestWithList");
            GXDLMSTranslator.AddTag(list, Command.MethodRequest, "ActionRequest");
            list.Add((int)(Command.MethodRequest) << 8 | (byte)ActionRequestType.Normal, "ActionRequestNormal");
            list.Add((int)(Command.MethodRequest) << 8 | (byte)ActionRequestType.NextBlock, "ActionRequestForNextDataBlock");
            list.Add((int)(Command.MethodRequest) << 8 | (byte)ActionRequestType.WithList, "ActionRequestWithList");
            GXDLMSTranslator.AddTag(list, Command.MethodResponse, "ActionResponse");
            list.Add((int)(Command.MethodResponse) << 8 | (byte)ActionRequestType.Normal, "ActionResponseNormal");
            list.Add((int)(Command.MethodResponse) << 8 | (byte)ActionRequestType.WithFirstBlock, "ActionResponseWithFirstBlock");
            list.Add((int)(Command.MethodResponse) << 8 | (byte)ActionRequestType.WithList, "ActionResponseWithList");
            list.Add((int)Command.DataNotification, "DataNotification");
            GXDLMSTranslator.AddTag(list, Command.AccessRequest, "AccessRequest");
            list.Add((int)(Command.AccessRequest) << 8 | (byte)AccessServiceCommandType.Get, "AccessRequestGet");
            list.Add((int)(Command.AccessRequest) << 8 | (byte)AccessServiceCommandType.Set, "AccessRequestSet");
            list.Add((int)(Command.AccessRequest) << 8 | (byte)AccessServiceCommandType.Action, "AccessRequestAction");
            GXDLMSTranslator.AddTag(list, Command.AccessResponse, "AccessResponse");
            list.Add((int)(Command.AccessResponse) << 8 | (byte)AccessServiceCommandType.Get, "AccessResponseGet");
            list.Add((int)(Command.AccessResponse) << 8 | (byte)AccessServiceCommandType.Set, "AccessResponseSet");
            list.Add((int)(Command.AccessResponse) << 8 | (byte)AccessServiceCommandType.Action, "AccessResponseAction");
        }

        /// <summary>
        /// Get glo tags.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        internal static void GetGloTags(TranslatorOutputType type, SortedList<int, string> list)
        {
            GXDLMSTranslator.AddTag(list, Command.GloInitiateRequest, "glo_InitiateRequest");
            GXDLMSTranslator.AddTag(list, Command.GloInitiateResponse, "glo_InitiateResponse");
            GXDLMSTranslator.AddTag(list, Command.GloGetRequest, "glo_GetRequest");
            GXDLMSTranslator.AddTag(list, Command.GloGetResponse, "glo_GetResponse");
            GXDLMSTranslator.AddTag(list, Command.GloSetRequest, "glo_SetRequest");
            GXDLMSTranslator.AddTag(list, Command.GloSetResponse, "glo_SetResponse");
            GXDLMSTranslator.AddTag(list, Command.GloMethodRequest, "glo_ActionRequest");
            GXDLMSTranslator.AddTag(list, Command.GloMethodResponse, "glo_ActionResponse");
            GXDLMSTranslator.AddTag(list, Command.GloReadRequest, "glo_ReadRequest");
            GXDLMSTranslator.AddTag(list, Command.GloReadResponse, "glo_ReadResponse");
            GXDLMSTranslator.AddTag(list, Command.GloWriteRequest, "glo_WriteRequest");
            GXDLMSTranslator.AddTag(list, Command.GloWriteResponse, "glo_WriteResponse");
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
            GXDLMSTranslator.AddTag(list, TranslatorTags.PduDlms, "Pdu");
            GXDLMSTranslator.AddTag(list, TranslatorTags.TargetAddress, "TargetAddress");
            GXDLMSTranslator.AddTag(list, TranslatorTags.SourceAddress, "SourceAddress");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ListOfVariableAccessSpecification, "ListOfVariableAccessSpecification");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ListOfData, "ListOfData");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Success, "Ok");
            GXDLMSTranslator.AddTag(list, TranslatorTags.DataAccessError, "DataAccessError");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AttributeDescriptor, "AttributeDescriptor");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ClassId, "ClassId");
            GXDLMSTranslator.AddTag(list, TranslatorTags.InstanceId, "InstanceId");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AttributeId, "AttributeId");
            GXDLMSTranslator.AddTag(list, TranslatorTags.MethodInvocationParameters, "MethodInvocationParameters");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Selector, "Selector");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Parameter, "Parameter");
            GXDLMSTranslator.AddTag(list, TranslatorTags.LastBlock, "LastBlock");
            GXDLMSTranslator.AddTag(list, TranslatorTags.BlockNumber, "BlockNumber");
            GXDLMSTranslator.AddTag(list, TranslatorTags.RawData, "RawData");
            GXDLMSTranslator.AddTag(list, TranslatorTags.MethodDescriptor, "MethodDescriptor");
            GXDLMSTranslator.AddTag(list, TranslatorTags.MethodId, "MethodId");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Result, "Result");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ReturnParameters, "ReturnParameters");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AccessSelection, "AccessSelection");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Value, "Value");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AccessSelector, "AccessSelector");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AccessParameters, "AccessParameters");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AttributeDescriptorList, "AttributeDescriptorList");
            GXDLMSTranslator.AddTag(list, TranslatorTags.AttributeDescriptorWithSelection, "AttributeDescriptorWithSelection");
            GXDLMSTranslator.AddTag(list, TranslatorTags.ReadDataBlockAccess, "ReadDataBlockAccess");
            GXDLMSTranslator.AddTag(list, TranslatorTags.WriteDataBlockAccess, "WriteDataBlockAccess");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Data, "Data");
            GXDLMSTranslator.AddTag(list, TranslatorTags.InvokeId, "InvokeIdAndPriority");
            GXDLMSTranslator.AddTag(list, TranslatorTags.LongInvokeId, "LongInvokeIdAndPriority");
            GXDLMSTranslator.AddTag(list, TranslatorTags.DateTime, "DateTime");
            GXDLMSTranslator.AddTag(list, TranslatorTags.Reason, "Reason");
            GXDLMSTranslator.AddTag(list, TranslatorTags.NotificationBody, "NotificationBody");
            GXDLMSTranslator.AddTag(list, TranslatorTags.DataValue, "DataValue");
            GXDLMSTranslator.AddTag(list, TranslatorTags.CipheredService, "CipheredService");
            GXDLMSTranslator.AddTag(list, TranslatorTags.SystemTitle, "SystemTitle");
        }

        public static void GetDataTypeTags(SortedList<int, string> list)
        {
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.None, "None");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Array, "Array");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Bcd, "BCD");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.BitString,
                     "BitString");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Boolean,
                     "Boolean");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.CompactArray,
                     "CompactArray");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Date, "Date");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.DateTime,
                     "DateTime");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Enum, "Enum");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Float32,
                     "Float32");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Float64,
                     "Float64");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int16, "Int16");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int32, "Int32");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int64, "Int64");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Int8, "Int8");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.OctetString,
                     "OctetString");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.String,
                     "String");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.StringUTF8,
                     "StringUTF8");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Structure,
                     "Structure");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.Time, "Time");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt16,
                     "UInt16");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt32,
                     "UInt32");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt64,
                     "UInt64");
            list.Add(GXDLMS.DATA_TYPE_OFFSET + (int)DataType.UInt8, "UInt8");
        }

        public static String ErrorCodeToString(ErrorCode value)
        {
            String str;
            switch (value)
            {
                case ErrorCode.AccessViolated:
                    str = "AccessViolated";
                    break;
                case ErrorCode.DataBlockNumberInvalid:
                    str = "DataBlockNumberInvalid";
                    break;
                case ErrorCode.DataBlockUnavailable:
                    str = "DataBlockUnavailable";
                    break;
                case ErrorCode.HardwareFault:
                    str = "HardwareFault";
                    break;
                case ErrorCode.InconsistentClass:
                    str = "InconsistentClass";
                    break;
                case ErrorCode.LongGetOrReadAborted:
                    str = "LongGetOrReadAborted";
                    break;
                case ErrorCode.LongSetOrWriteAborted:
                    str = "LongSetOrWriteAborted";
                    break;
                case ErrorCode.NoLongGetOrReadInProgress:
                    str = "NoLongGetOrReadInProgress";
                    break;
                case ErrorCode.NoLongSetOrWriteInProgress:
                    str = "NoLongSetOrWriteInProgress";
                    break;
                case ErrorCode.Ok:
                    str = "Ok";
                    break;
                case ErrorCode.OtherReason:
                    str = "OtherReason";
                    break;
                case ErrorCode.ReadWriteDenied:
                    str = "ReadWriteDenied";
                    break;
                case ErrorCode.TemporaryFailure:
                    str = "TemporaryFailure";
                    break;
                case ErrorCode.UnavailableObject:
                    str = "UnavailableObject";
                    break;
                case ErrorCode.UndefinedObject:
                    str = "UndefinedObject";
                    break;
                case ErrorCode.UnmatchedType:
                    str = "UnmatchedType";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Error code: " + value);
            }
            return str;
        }

        public static ErrorCode ValueOfErrorCode(String value)
        {
            ErrorCode v;
            if (string.Compare("AccessViolated", value, true) == 0)
            {
                v = ErrorCode.AccessViolated;
            }
            else if (string.Compare("DataBlockNumberInvalid", value, true) == 0)
            {
                v = ErrorCode.DataBlockNumberInvalid;
            }
            else if (string.Compare("DataBlockUnavailable", value, true) == 0)
            {
                v = ErrorCode.DataBlockUnavailable;
            }
            else if (string.Compare("HardwareFault", value, true) == 0)
            {
                v = ErrorCode.HardwareFault;
            }
            else if (string.Compare("InconsistentClass", value, true) == 0)
            {
                v = ErrorCode.InconsistentClass;
            }
            else if (string.Compare("LongGetOrReadAborted", value, true) == 0)
            {
                v = ErrorCode.LongGetOrReadAborted;
            }
            else if (string.Compare("LongSetOrWriteAborted", value, true) == 0)
            {
                v = ErrorCode.LongSetOrWriteAborted;
            }
            else if (string.Compare("NoLongGetOrReadInProgress", value, true) == 0)
            {
                v = ErrorCode.NoLongGetOrReadInProgress;
            }
            else if (string.Compare("NoLongSetOrWriteInProgress", value, true) == 0)
            {
                v = ErrorCode.NoLongSetOrWriteInProgress;
            }
            else if (string.Compare("Ok", value, true) == 0)
            {
                v = ErrorCode.Ok;
            }
            else if (string.Compare("OtherReason", value, true) == 0)
            {
                v = ErrorCode.OtherReason;
            }
            else if (string.Compare("ReadWriteDenied", value, true) == 0)
            {
                v = ErrorCode.ReadWriteDenied;
            }
            else if (string.Compare("TemporaryFailure", value, true) == 0)
            {
                v = ErrorCode.TemporaryFailure;
            }
            else if (string.Compare("UnavailableObject", value, true) == 0)
            {
                v = ErrorCode.UnavailableObject;
            }
            else if (string.Compare("UndefinedObject", value, true) == 0)
            {
                v = ErrorCode.UndefinedObject;
            }
            else if (string.Compare("UnmatchedType", value, true) == 0)
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
            list.Add(ServiceError.ApplicationReference, "ApplicationReference");
            list.Add(ServiceError.HardwareResource, "HardwareResource");
            list.Add(ServiceError.VdeStateError, "VdeStateError");
            list.Add(ServiceError.Service, "Service");
            list.Add(ServiceError.Definition, "Definition");
            list.Add(ServiceError.Access, "Access");
            list.Add(ServiceError.Initiate, "Initiate");
            list.Add(ServiceError.LoadDataSet, "LoadDataSet");
            list.Add(ServiceError.Task, "Task");
            return list;
        }

        static Dictionary<ApplicationReference, String> GetApplicationReference()
        {
            Dictionary<ApplicationReference, String> list =
                    new Dictionary<ApplicationReference, String>();
            list.Add(ApplicationReference.ApplicationContextUnsupported,
                    "ApplicationContextUnsupported");
            list.Add(ApplicationReference.ApplicationReferenceInvalid,
                    "ApplicationReferenceInvalid");
            list.Add(ApplicationReference.ApplicationUnreachable,
                    "ApplicationUnreachable");
            list.Add(ApplicationReference.DecipheringError, "DecipheringError");
            list.Add(ApplicationReference.Other, "Other");
            list.Add(ApplicationReference.ProviderCommunicationError,
                    "ProviderCommunicationError");
            list.Add(ApplicationReference.TimeElapsed, "TimeElapsed");
            return list;
        }

        static Dictionary<HardwareResource, String> GetHardwareResource()
        {
            Dictionary<HardwareResource, String> list =
                    new Dictionary<HardwareResource, String>();
            list.Add(HardwareResource.MassStorageUnavailable,
                    "MassStorageUnavailable");
            list.Add(HardwareResource.MemoryUnavailable, "MemoryUnavailable");
            list.Add(HardwareResource.Other, "Other");
            list.Add(HardwareResource.OtherResourceUnavailable,
                    "OtherResourceUnavailable");
            list.Add(HardwareResource.ProcessorResourceUnavailable,
                    "ProcessorResourceUnavailable");
            return list;
        }

        static Dictionary<VdeStateError, String> GetVdeStateError()
        {
            Dictionary<VdeStateError, String> list = new Dictionary<VdeStateError, String>();
            list.Add(VdeStateError.LoadingDataSet, "LoadingDataSet");
            list.Add(VdeStateError.NoDlmsContext, "NoDlmsContext");
            list.Add(VdeStateError.Other, "Other");
            list.Add(VdeStateError.StatusInoperable, "StatusInoperable");
            list.Add(VdeStateError.StatusNochange, "StatusNochange");
            return list;
        }

        static Dictionary<Service, String> GetService()
        {
            Dictionary<Service, String> list = new Dictionary<Service, String>();
            list.Add(Service.Other, "Other");
            list.Add(Service.PduSize, "PduSize");
            list.Add(Service.Unsupported, "ServiceUnsupported");
            return list;
        }

        static Dictionary<Definition, String> GetDefinition()
        {
            Dictionary<Definition, String> list = new Dictionary<Definition, String>();
            list.Add(Definition.ObjectAttributeInconsistent,
                    "ObjectAttributeInconsistent");
            list.Add(Definition.ObjectClassInconsistent,
                    "ObjectClassInconsistent");
            list.Add(Definition.ObjectUndefined, "ObjectUndefined");
            list.Add(Definition.Other, "Other");
            return list;
        }

        static Dictionary<Access, String> GetAccess()
        {
            Dictionary<Access, String> list = new Dictionary<Access, String>();
            list.Add(Access.HardwareFault, "HardwareFault");
            list.Add(Access.ObjectAccessInvalid, "ObjectAccessInvalid");
            list.Add(Access.ObjectUnavailable, "ObjectUnavailable");
            list.Add(Access.Other, "Other");
            list.Add(Access.ScopeOfAccessViolated, "ScopeOfAccessViolated");
            return list;
        }

        static Dictionary<Initiate, String> GetInitiate()
        {
            Dictionary<Initiate, String> list = new Dictionary<Initiate, String>();
            list.Add(Initiate.DlmsVersionTooLow, "DlmsVersionTooLow");
            list.Add(Initiate.IncompatibleConformance, "IncompatibleConformance");
            list.Add(Initiate.Other, "Other");
            list.Add(Initiate.PduSizeTooShort, "PduSizeTooShort");
            list.Add(Initiate.RefusedByTheVDEHandler, "RefusedByTheVDEHandler");
            return list;
        }

        static Dictionary<LoadDataSet, String> GetLoadDataSet()
        {
            Dictionary<LoadDataSet, String> list = new Dictionary<LoadDataSet, String>();
            list.Add(LoadDataSet.DatasetNotReady, "DataSetNotReady");
            list.Add(LoadDataSet.DatasetSizeTooLarge, "DatasetSizeTooLarge");
            list.Add(LoadDataSet.InterpretationFailure, "InterpretationFailure");
            list.Add(LoadDataSet.NotAwaitedSegment, "NotAwaitedSegment");
            list.Add(LoadDataSet.NotLoadable, "NotLoadable");
            list.Add(LoadDataSet.Other, "Other");
            list.Add(LoadDataSet.PrimitiveOutOfSequence,
                    "PrimitiveOutOfSequence");
            list.Add(LoadDataSet.StorageFailure, "StorageFailure");
            return list;
        }

        static Dictionary<Task, String> GetTask()
        {
            Dictionary<Task, String> list = new Dictionary<Task, String>();
            list.Add(Task.NoRemoteControl, "NoRemoteControl");
            list.Add(Task.Other, "Other");
            list.Add(Task.TiRunning, "tiRunning");
            list.Add(Task.TiStopped, "tiStopped");
            list.Add(Task.TiUnusable, "tiUnusable");
            return list;
        }

        internal static String GetServiceErrorValue(ServiceError error,
                byte value)
        {
            switch (error)
            {
                case ServiceError.ApplicationReference:
                    return GetApplicationReference()[(ApplicationReference)value];
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
        /// <returns>Service error simple XML tag.</returns>
        internal static String ServiceErrorToString(ServiceError error)
        {
            return GetServiceErrors()[error];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Service error simple XML tag.</param>
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
    }
}