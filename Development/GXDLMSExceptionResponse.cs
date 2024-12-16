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
using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{
    /// <summary>
    /// DLMS specific exception response.
    /// </summary>
    /// <remarks>
    /// https://www.gurux.fi/Gurux.DLMS.ErrorCodes
    /// </remarks>
    public class GXDLMSExceptionResponse : Exception
    {
        public ExceptionStateError ExceptionStateError
        {
            get;
            private set;
        }

        public ExceptionServiceError ExceptionServiceError
        {
            get;
            private set;
        }
        public object Value
        {
            get;
            private set;
        }


        /// <summary>
        /// Constructor for Confirmed ServiceError.
        /// </summary>
        internal GXDLMSExceptionResponse(ExceptionStateError error, ExceptionServiceError type, object value)
            : base("Exception response. \"" + GetStateError(error) + "\"-exception. " + GetServiceError(type, value))
        {
            ExceptionStateError = error;
            ExceptionServiceError = type;
            Value = value;
            HelpLink = "https://www.gurux.fi/Gurux.DLMS.ErrorCodes";
        }

        private static string GetStateError(ExceptionStateError stateError)
        {
            switch (stateError)
            {
                case ExceptionStateError.ServiceNotAllowed:
                    return "Service not allowed";
                case ExceptionStateError.ServiceUnknown:
                    return "Service unknown";
            }
            return string.Empty;
        }

        private static string GetServiceError(ExceptionServiceError serviceError, object value)
        {
            switch (serviceError)
            {
                case ExceptionServiceError.OperationNotPossible:
                    return "Operation not possible";
                case ExceptionServiceError.OtherReason:
                    return "Other reason";
                case ExceptionServiceError.ServiceNotSupported:
                    return "Service not supported";
                case ExceptionServiceError.PduTooLong:
                    return "PDU is too long";
                case ExceptionServiceError.DecipheringError:
                    return "Deciphering failed";
                case ExceptionServiceError.InvocationCounterError:
                    return "Invocation counter is invalid. Expected value is " + Convert.ToString(value);
            }
            return string.Empty;
        }
    }
}
