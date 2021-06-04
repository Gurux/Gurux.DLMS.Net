//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Filename:    $HeadURL$
//
// Version:     $Revision$,
//      $Date$
//      $Author$
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

using Gurux.DLMS.ASN.Enums;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.ASN
{
    /// <summary>
    /// Certificate request
    /// </summary>
    public class GXCertificateRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXCertificateRequest()
        {

        }

        public GXCertificateRequest(CertificateType certificateType, GXPkcs10 certificate)
        {
            Certificate = certificate;
            CertificateType = certificateType;
        }

        /// <summary>
        /// Certificate type.
        /// </summary>
        public CertificateType CertificateType
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the purpose for which the certified public key is used.
        /// </summary>
        public ExtendedKeyUsage ExtendedKeyUsage
        {
            get;
            set;
        }

        /// <summary>
        /// Certificate Signing Request.
        /// </summary>
        public GXPkcs10 Certificate
        {
            get;
            set;
        }
    }
}