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

namespace Gurux.DLMS.ASN.Enums
{
    public static class X509CertificateTypeConverter
    {
        public static string GetString(X509CertificateType value)
        {
            string ret;
            switch (value)
            {
                case X509CertificateType.OldAuthorityKeyIdentifier:
                    ret = "2.5.29.1";
                    break;
                case X509CertificateType.OldPrimaryKeyAttributes:
                    ret = "2.5.29.2";
                    break;
                case X509CertificateType.CertificatePolicies:
                    ret = "2.5.29.3";
                    break;
                case X509CertificateType.OrimaryKeyUsageRestriction:
                    ret = "2.5.29.4";
                    break;
                case X509CertificateType.SubjectDirectoryAttributes:
                    ret = "2.5.29.9";
                    break;
                case X509CertificateType.SubjectKeyIdentifier:
                    ret = "2.5.29.14";
                    break;
                case X509CertificateType.KeyUsage:
                    ret = "2.5.29.15";
                    break;
                case X509CertificateType.PrivateKeyUsagePeriod:
                    ret = "2.5.29.16";
                    break;
                case X509CertificateType.SubjectAlternativeName:
                    ret = "2.5.29.17";
                    break;
                case X509CertificateType.IssuerAlternativeName:
                    ret = "2.5.29.18";
                    break;
                case X509CertificateType.BasicConstraints:
                    ret = "2.5.29.19";
                    break;
                case X509CertificateType.CrlNumber:
                    ret = "2.5.29.20";
                    break;
                case X509CertificateType.ReasonCode:
                    ret = "2.5.29.21";
                    break;
                case X509CertificateType.HoldInstructionCode:
                    ret = "2.5.29.23";
                    break;
                case X509CertificateType.InvalidityDate:
                    ret = "2.5.29.24";
                    break;
                case X509CertificateType.DeltaCrlIndicator:
                    ret = "2.5.29.27";
                    break;
                case X509CertificateType.IssuingDistributionPoint:
                    ret = "2.5.29.28";
                    break;
                case X509CertificateType.CertificateIssuer:
                    ret = "2.5.29.29";
                    break;
                case X509CertificateType.NameConstraints:
                    ret = "2.5.29.30";
                    break;
                case X509CertificateType.CrlDistributionPoints:
                    ret = "2.5.29.31";
                    break;
                case X509CertificateType.CertificatePolicies2:
                    ret = "2.5.29.32";
                    break;
                case X509CertificateType.PolicyMappings:
                    ret = "2.5.29.33";
                    break;
                case X509CertificateType.AuthorityKeyIdentifier:
                    ret = "2.5.29.35";
                    break;
                case X509CertificateType.PolicyConstraints:
                    ret = "2.5.29.36";
                    break;
                case X509CertificateType.ExtendedKeyUsage:
                    ret = "2.5.29.37";
                    break;
                case X509CertificateType.FreshestCrl:
                    ret = "2.5.29.46";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid X509Certificate. " + value);
            }
            return ret;
        }

        public static X509CertificateType FromString(string value)
        {
            X509CertificateType ret;
            switch (value)
            {
                case "2.5.29.1":
                    ret = X509CertificateType.OldAuthorityKeyIdentifier;
                    break;
                case "2.5.29.2":
                    ret = X509CertificateType.OldPrimaryKeyAttributes;
                    break;
                case "2.5.29.3":
                    ret = X509CertificateType.CertificatePolicies;
                    break;
                case "2.5.29.4":
                    ret = X509CertificateType.OrimaryKeyUsageRestriction;
                    break;
                case "2.5.29.9":
                    ret = X509CertificateType.SubjectDirectoryAttributes;
                    break;
                case "2.5.29.14":
                    ret = X509CertificateType.SubjectKeyIdentifier;
                    break;
                case "2.5.29.15":
                    ret = X509CertificateType.KeyUsage;
                    break;
                case "2.5.29.16":
                    ret = X509CertificateType.PrivateKeyUsagePeriod;
                    break;
                case "2.5.29.17":
                    ret = X509CertificateType.SubjectAlternativeName;
                    break;
                case "2.5.29.18":
                    ret = X509CertificateType.IssuerAlternativeName;
                    break;
                case "2.5.29.19":
                    ret = X509CertificateType.BasicConstraints;
                    break;
                case "2.5.29.20":
                    ret = X509CertificateType.CrlNumber;
                    break;
                case "2.5.29.21":
                    ret = X509CertificateType.ReasonCode;
                    break;
                case "2.5.29.23":
                    ret = X509CertificateType.HoldInstructionCode;
                    break;
                case "2.5.29.24":
                    ret = X509CertificateType.InvalidityDate;
                    break;
                case "2.5.29.27":
                    ret = X509CertificateType.DeltaCrlIndicator;
                    break;
                case "2.5.29.28":
                    ret = X509CertificateType.IssuingDistributionPoint;
                    break;
                case "2.5.29.29":
                    ret = X509CertificateType.CertificateIssuer;
                    break;
                case "2.5.29.30":
                    ret = X509CertificateType.NameConstraints;
                    break;
                case "2.5.29.31":
                    ret = X509CertificateType.CrlDistributionPoints;
                    break;
                case "2.5.29.32":
                    ret = X509CertificateType.CertificatePolicies2;
                    break;
                case "2.5.29.33":
                    ret = X509CertificateType.PolicyMappings;
                    break;
                case "2.5.29.35":
                    ret = X509CertificateType.AuthorityKeyIdentifier;
                    break;
                case "2.5.29.36":
                    ret = X509CertificateType.PolicyConstraints;
                    break;
                case "2.5.29.37":
                    ret = X509CertificateType.ExtendedKeyUsage;
                    break;
                case "2.5.29.46":
                    ret = X509CertificateType.FreshestCrl;
                    break;
                default:
                    ret = X509CertificateType.None;
                    break;
            }
            return ret;
        }
    }
}