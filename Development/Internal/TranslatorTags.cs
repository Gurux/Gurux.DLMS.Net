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

using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{
    internal enum TranslatorTags
    {
        Wrapper = 0xFF01,
        Hdlc,
        PduDlms,
        PduCse,
        TargetAddress,
        SourceAddress,
        ListOfVariableAccessSpecification,
        ListOfData,
        Success,
        DataAccessError,
        AttributeDescriptor,
        ClassId,
        InstanceId,
        AttributeId,
        MethodInvocationParameters,
        Selector,
        Parameter,
        LastBlock,
        BlockNumber,
        RawData,
        MethodDescriptor,
        MethodId,
        Result,
        ReturnParameters,
        AccessSelection,
        Value,
        AccessSelector,
        AccessParameters,
        AttributeDescriptorList,
        AttributeDescriptorWithSelection,
        ReadDataBlockAccess,
        WriteDataBlockAccess,
        Data,
        InvokeId,
        LongInvokeId,
        DateTime,
        Reason,
        VariableAccessSpecification,
        ListOfResult,
        Choice,
        NotificationBody,
        DataValue,
        AccessRequestBody,
        ListOfAccessRequestSpecification,
        AccessRequestSpecification,
        AccessRequestListOfData,
        AccessResponseBody,
        ListOfAccessResponseSpecification,
        AccessResponseSpecification,
        AccessResponseListOfData,
        SingleResponse
    }
}
