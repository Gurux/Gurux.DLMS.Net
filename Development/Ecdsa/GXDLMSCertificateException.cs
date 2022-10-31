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

namespace Gurux.DLMS.Ecdsa
{
    /// <summary>
    /// DLMS specific exception class for certificate exceptions.
    /// </summary>
    /// <remarks>
    /// https://www.gurux.fi/Gurux.DLMS.Secure
    /// </remarks>
#if !WINDOWS_UWP
    [Serializable]
#endif //!WINDOWS_UWP
    public class GXDLMSCertificateException : Exception
    {
        public GXDLMSCertificateException(string message)
            : base(message)
        {
            HelpLink = "https://gurux.fi/Gurux.DLMS.Secure";
        }

        public GXDLMSCertificateException()
        {
            HelpLink = "https://gurux.fi/Gurux.DLMS.Secure";
        }

        public GXDLMSCertificateException(string message, Exception innerException) : base(message, innerException)
        {
            HelpLink = "https://gurux.fi/Gurux.DLMS.Secure";
        }
    }
}
