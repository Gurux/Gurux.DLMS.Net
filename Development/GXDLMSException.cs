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

namespace Gurux.DLMS
{
	/// <summary>
	/// DLMS specific exception class that has error description available from GetDescription method.
	/// </summary>
    public class GXDLMSException : Exception
    {
        static private string GetDescription(int errCode)
        {
            string str = null;
            switch (errCode)
            {                    
                case 1: //Access Error : Device reports a hardware fault
                    str = Gurux.DLMS.Properties.Resources.HardwareFaultTxt;
                    break;
                case 2: //Access Error : Device reports a temporary failure
                    str = Gurux.DLMS.Properties.Resources.TemporaryFailureTxt;
                    break;
                case 3: // Access Error : Device reports Read-Write denied
                    str = Gurux.DLMS.Properties.Resources.ReadWriteDeniedTxt;
                    break;
                case 4: // Access Error : Device reports a undefined object
                    str = Gurux.DLMS.Properties.Resources.UndefinedObjectTxt;
                    break;
                case 5: // Access Error : Device reports a inconsistent Class or object
                    str = Gurux.DLMS.Properties.Resources.InconsistentClassTxt;
                    break;
                case 6: // Access Error : Device reports a unavailable object
                    str = Gurux.DLMS.Properties.Resources.UnavailableObjectTxt;
                    break;
                case 7: // Access Error : Device reports a unmatched type
                    str = Gurux.DLMS.Properties.Resources.UnmatchedTypeTxt;
                    break;
                case 8: // Access Error : Device reports scope of access violated
                    str = Gurux.DLMS.Properties.Resources.AccessViolatedTxt;
                    break;
                default:
                    str = Gurux.DLMS.Properties.Resources.UnknownErrorTxt;
                    break;
            }
            return str;
        }
        public GXDLMSException(int errCode)
            : base(GetDescription(errCode))
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
        internal GXDLMSException(AssociationResult result, SourceDiagnostic diagnostic)
            : base("Connection is " + GetResult(result) + Environment.NewLine + GetDiagnostic(diagnostic))
        {

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
    }
}
