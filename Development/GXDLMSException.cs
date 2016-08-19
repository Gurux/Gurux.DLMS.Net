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
using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{
	/// <summary>
	/// DLMS specific exception class that has error description available from GetDescription method.
	/// </summary>
    public class GXDLMSException : Exception
    {
        public GXDLMSException(int errCode)
            : base(GXDLMS.GetDescription((ErrorCode)errCode))
        {
            ErrorCode = errCode;
        }

        public GXDLMSException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Constructor for AARE error.
        /// </summary>
        internal GXDLMSException(ExceptionStateError stateError, ExceptionServiceError serviceError)
            : base("Meter returns " + GetStateError(stateError) + " exception. " + GetServiceError(serviceError))
        {
            StateError = stateError;
            ExceptionServiceError = serviceError;
        }

        /// <summary>
        /// Constructor for Confirmed ServiceError.
        /// </summary>
        internal GXDLMSException(ConfirmedServiceError service, ServiceError type, byte value)
            : base("ServiceError " + GetConfirmedServiceError(service) + " exception. " + GetServiceError(type) + " " + GetServiceErrorValue(type, value))
        {
            ConfirmedServiceError = service;
            ServiceError = type;
            ServiceErrorValue = value;
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

        private static string GetConfirmedServiceError(ConfirmedServiceError stateError)
        {
            switch (stateError)
            {
                case ConfirmedServiceError.InitiateError:
                    return "Initiate Error";
                case ConfirmedServiceError.Read:
                    return "Read";
                case ConfirmedServiceError.Write:
                    return "Write";
                default:
                    break;
            }
             return string.Empty;
        }

        private static string GetServiceError(ServiceError error)
        {
            switch (error)
            {
                case ServiceError.ApplicationReference:
                    return "ApplicationReference";
                case ServiceError.HardwareResource:
                    return "HardwareResource";
                case ServiceError.VdeStateError:
                    return "VdeStateError";
                case ServiceError.Service:
                    return "Service";
                case ServiceError.Definition:
                    return "Definition";
                case ServiceError.Access:
                    return "Access";
                case ServiceError.Initiate:
                    return "Initiate";
                case ServiceError.LoadDataSet:
                    return "Load data set";
                case ServiceError.Task:
                    return "Task";
                case ServiceError.OtherError:
                    return "Other Error";
                default:
                    break;
            }
             return string.Empty;
        }

        private static string GetServiceErrorValue(ServiceError error, byte value)
        {
            switch (error)
            {
                case ServiceError.ApplicationReference:
                    return ((ApplicationReference) value).ToString();
                case ServiceError.HardwareResource:
                    return ((HardwareResource)value).ToString();
                case ServiceError.VdeStateError:
                    return ((VdeStateError)value).ToString();
                case ServiceError.Service:
                    return ((Service)value).ToString();
                case ServiceError.Definition:
                    return ((Definition)value).ToString();
                case ServiceError.Access:
                    return ((Access)value).ToString();
                case ServiceError.Initiate:
                    return ((Initiate)value).ToString();
                case ServiceError.LoadDataSet:
                    return ((LoadDataSet)value).ToString();
                case ServiceError.Task:
                    return ((Task)value).ToString();
                case ServiceError.OtherError:
                    return value.ToString();
                default:
                    break;
            }
             return string.Empty;
        }

        private static string GetServiceError(ExceptionServiceError serviceError)
        {
            switch (serviceError)
            {
                case ExceptionServiceError.OperationNotPossible:
                    return "Operation not possible";
                case ExceptionServiceError.OtherReason:
                    return "Other reason";
                case ExceptionServiceError.ServiceNotSupported:
                    return "Service not supported";

            }
            return string.Empty;
        }
        

        /// <summary>
        /// Constructor for AARE error.
        /// </summary>
        internal GXDLMSException(AssociationResult result, SourceDiagnostic diagnostic)
            : base("Connection is " + GetResult(result) + ". " + GetDiagnostic(diagnostic))
        {
            Result = result;
            Diagnostic = diagnostic;
        }

        /// <summary>
        /// Get resulat as a string.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        static string GetResult(AssociationResult result)
        {
            if (result == AssociationResult.PermanentRejected)
            {
                return "permanently rejected";
            }
            if (result == AssociationResult.TransientRejected)
            {
                return "transient rejected";
            }
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Get diagnostic as a string.
        /// </summary>
        /// <param name="diagnostic"></param>
        /// <returns></returns>
        static string GetDiagnostic(SourceDiagnostic diagnostic)
        {
            if (diagnostic == SourceDiagnostic.NoReasonGiven)
            {
                return "No reason is given.";
            }
            if (diagnostic == SourceDiagnostic.ApplicationContextNameNotSupported)
            {
                return "The application context name is not supported.";
            }
            if (diagnostic == SourceDiagnostic.AuthenticationMechanismNameNotRecognised)
            {
                return "The authentication mechanism name is not recognized.";
            }
            if (diagnostic == SourceDiagnostic.AuthenticationMechanismNameReguired)
            {
                return "Authentication mechanism name is required.";
            }
            if (diagnostic == SourceDiagnostic.AuthenticationFailure)
            {
                return "Authentication failure.";
            }
            if (diagnostic == SourceDiagnostic.AuthenticationRequired)
            {
                return "Authentication is required.";
            }
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns occurred error code.
        /// </summary>
        public int ErrorCode
        {
            get;
            internal set;
        }


        /// <summary>
        /// Returns occurred Association Result in AARE message.
        /// </summary>
        public AssociationResult Result
        {
            get;
            internal set;
        }

        /// <summary>
        /// Returns Diagnostic code in AARE message.
        /// </summary>
        public SourceDiagnostic Diagnostic
        {
            get;
            internal set;
        }

        /// <summary>
        /// State error.
        /// </summary>
        public ExceptionStateError StateError
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Service error.
        /// </summary>
        public ExceptionServiceError ExceptionServiceError
        {
            get;
            private set;
        }

        /// <summary>
        /// Confirmed service error.
        /// </summary>
        public ConfirmedServiceError ConfirmedServiceError
        {
            get;
            private set;
        }

        /// <summary>
        /// Service error.
        /// </summary>
        public ServiceError ServiceError
        {
            get;
            private set;
        }

        /// <summary>
        /// Service error value.
        /// </summary>
        public byte ServiceErrorValue
        {
            get;
            private set;
        }

    }
}
