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
    /// DLMS specific exception class that has error description available from GetDescription method.
    /// </summary>
    /// <remarks>
    /// https://www.gurux.fi/Gurux.DLMS.ErrorCodes
    /// </remarks>
    public class GXDLMSException : Exception
    {
        public GXDLMSException(int errCode)
            : base(GXDLMS.GetDescription((ErrorCode)errCode))
        {
            ErrorCode = errCode;
            HelpLink = "https://www.gurux.fi/Gurux.DLMS.ErrorCodes#" + ErrorCode;
        }

        public GXDLMSException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Constructor for AARE error.
        /// </summary>
        internal GXDLMSException(AssociationResult result, SourceDiagnostic diagnostic) : this(result, diagnostic, null)
        {

        }

        /// <summary>
        /// Constructor for AARE error.
        /// </summary>
        internal GXDLMSException(AssociationResult result, SourceDiagnostic diagnostic, string extra)
            : base("Connection is " + GetResult(result) + ". " + GetDiagnostic(diagnostic) + extra)
        {
            Result = result;
            Diagnostic = (byte)diagnostic;
            HelpLink = "https://www.gurux.fi/Gurux.DLMS.ErrorCodes";
        }


        /// <summary>
        /// Constructor for AARE error.
        /// </summary>
        internal GXDLMSException(AssociationResult result, AcseServiceProvider diagnostic)
            : base("Connection is " + GetResult(result) + ". " + GetDiagnostic(diagnostic))
        {
            Result = result;
            Diagnostic = (byte)diagnostic;
            HelpLink = "https://www.gurux.fi/Gurux.DLMS.ErrorCodes";
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
        static string GetDiagnostic(AcseServiceProvider diagnostic)
        {
            string str;
            switch (diagnostic)
            {
                case AcseServiceProvider.None:
                    str = "None.";
                    break;
                case AcseServiceProvider.NoReasonGiven:
                    str = "No reason given.";
                    break;
                case AcseServiceProvider.NoCommonAcseVersion:
                    str = "No Common AcseVersion.";
                    break;
                default:
                    str = "Unknown diagnostic error.";
                    break;
            }
            return str;
        }

        /// <summary>
        /// Get diagnostic as a string.
        /// </summary>
        /// <param name="diagnostic"></param>
        /// <returns></returns>
        static string GetDiagnostic(SourceDiagnostic diagnostic)
        {
            string str;
            switch (diagnostic)
            {
                case SourceDiagnostic.None:
                    str = "None";
                    break;
                case SourceDiagnostic.NoReasonGiven:
                    str = "No reason is given.";
                    break;
                case SourceDiagnostic.ApplicationContextNameNotSupported:
                    str = "The application context name is not supported.";
                    break;
                case SourceDiagnostic.CallingApTitleNotRecognized:
                    str = "Calling AP title not recognized.";
                    break;
                case SourceDiagnostic.CallingApInvocationIdentifierNotRecognized:
                    str = "Calling AP invocation Identifier not recognized.";
                    break;
                case SourceDiagnostic.CallingAeQualifierNotRecognized:
                    str = "Calling AE qualifier not recognized.";
                    break;
                case SourceDiagnostic.CallingAeInvocationIdentifierNotRecognized:
                    str = "Calling AE invocation identifier not recognized";
                    break;
                case SourceDiagnostic.CalledApTitleNotRecognized:
                    str = "Called AP title not recognized.";
                    break;
                case SourceDiagnostic.CalledApInvocationIdentifierNotRecognized:
                    str = "Called AP invocation identifier not recognized.";
                    break;
                case SourceDiagnostic.CalledAeQualifierNotRecognized:
                    str = "Called AE qualifier not recognized.";
                    break;
                case SourceDiagnostic.CalledAeInvocationIdentifierNotRecognized:
                    str = "Called AE invocation identifier not recognized.";
                    break;
                case SourceDiagnostic.AuthenticationMechanismNameNotRecognized:
                    str = "Authentication mechanism name not recognized.";
                    break;
                case SourceDiagnostic.AuthenticationMechanismNameReguired:
                    str = "Authentication mechanism name is required.";
                    break;
                case SourceDiagnostic.AuthenticationFailure:
                    str = "Authentication failure.";
                    break;
                case SourceDiagnostic.AuthenticationRequired:
                    str = "Authentication is required.";
                    break;
                default:
                    str = "Unknown diagnostic error.";
                    break;
            }
            return str;
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
        public byte Diagnostic
        {
            get;
            internal set;
        }
    }
}
