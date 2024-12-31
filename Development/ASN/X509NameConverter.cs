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

using Gurux.DLMS.ASN.Enums;
using System;

namespace Gurux.DLMS.ASN
{
    public static class X509NameConverter
    {
        public static string GetString(X509Name value)
        {
            string ret;
            switch (value)
            {
                case X509Name.C:
                    ret = "2.5.4.6";
                    break;
                case X509Name.O:
                    ret = "2.5.4.10";
                    break;
                case X509Name.OU:
                    ret = "2.5.4.11";
                    break;
                case X509Name.T:
                    ret = "2.5.4.12";
                    break;
                case X509Name.CN:
                    ret = "2.5.4.3";
                    break;
                case X509Name.STREET:
                    ret = "2.5.4.9";
                    break;
                case X509Name.SerialNumber:
                    ret = "2.5.4.5";
                    break;
                case X509Name.L:
                    ret = "2.5.4.7";
                    break;
                case X509Name.ST:
                    ret = "2.5.4.8";
                    break;
                case X509Name.SurName:
                    ret = "2.5.4.4";
                    break;
                case X509Name.GivenName:
                    ret = "2.5.4.42";
                    break;
                case X509Name.Initials:
                    ret = "2.5.4.43";
                    break;
                case X509Name.Generation:
                    ret = "2.5.4.44";
                    break;
                case X509Name.UniqueIdentifier:
                    ret = "2.5.4.45";
                    break;
                case X509Name.BusinessCategory:
                    ret = "2.5.4.15";
                    break;
                case X509Name.PostalCode:
                    ret = "2.5.4.17";
                    break;
                case X509Name.DnQualifier:
                    ret = "2.5.4.46";
                    break;
                case X509Name.Pseudonym:
                    ret = "2.5.4.65";
                    break;
                case X509Name.DateOfBirth:
                    ret = "1.3.6.1.5.5.7.9.1";
                    break;
                case X509Name.PlaceOfBirth:
                    ret = "1.3.6.1.5.5.7.9.2";
                    break;
                case X509Name.Gender:
                    ret = "1.3.6.1.5.5.7.9.3";
                    break;
                case X509Name.CountryOfCitizenship:
                    ret = "1.3.6.1.5.5.7.9.4";
                    break;
                case X509Name.CountryOfResidence:
                    ret = "1.3.6.1.5.5.7.9.5";
                    break;
                case X509Name.NameAtBirth:
                    ret = "1.3.36.8.3.14";
                    break;
                case X509Name.PostalAddress:
                    ret = "2.5.4.16";
                    break;
                case X509Name.DmdName:
                    ret = "2.5.4.54";
                    break;
                case X509Name.TelephoneNumber:
                    ret = "2.5.4.20";
                    break;
                case X509Name.Name:
                    ret = "2.5.4.41";
                    break;
                case X509Name.E:
                    ret = "1.2.840.113549.1.9.1";
                    break;
                case X509Name.DC:
                    ret = "0.9.2342.19200300.100.1.25";
                    break;
                case X509Name.UID:
                    ret = "0.9.2342.19200300.100.1.1";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid X509Name. " + value);
            }
            return ret;
        }

        public static X509Name FromString(string value)
        {
            X509Name ret;
            switch (value)
            {
                case "2.5.4.6":
                    ret = X509Name.C;
                    break;
                case "2.5.4.10":
                    ret = X509Name.O;
                    break;
                case "2.5.4.11":
                    ret = X509Name.OU;
                    break;
                case "2.5.4.12":
                    ret = X509Name.T;
                    break;
                case "2.5.4.3":
                    ret = X509Name.CN;
                    break;
                case "2.5.4.9":
                    ret = X509Name.STREET;
                    break;
                case "2.5.4.5":
                    ret = X509Name.SerialNumber;
                    break;
                case "2.5.4.7":
                    ret = X509Name.L;
                    break;
                case "2.5.4.8":
                    ret = X509Name.ST;
                    break;
                case "2.5.4.4":
                    ret = X509Name.SurName;
                    break;
                case "2.5.4.42":
                    ret = X509Name.GivenName;
                    break;
                case "2.5.4.43":
                    ret = X509Name.Initials;
                    break;
                case "2.5.4.44":
                    ret = X509Name.Generation;
                    break;
                case "2.5.4.45":
                    ret = X509Name.UniqueIdentifier;
                    break;
                case "2.5.4.15":
                    ret = X509Name.BusinessCategory;
                    break;
                case "2.5.4.17":
                    ret = X509Name.PostalCode;
                    break;
                case "2.5.4.46":
                    ret = X509Name.DnQualifier;
                    break;
                case "2.5.4.65":
                    ret = X509Name.Pseudonym;
                    break;
                case "1.3.6.1.5.5.7.9.1":
                    ret = X509Name.DateOfBirth;
                    break;
                case "1.3.6.1.5.5.7.9.2":
                    ret = X509Name.PlaceOfBirth;
                    break;
                case "1.3.6.1.5.5.7.9.3":
                    ret = X509Name.Gender;
                    break;
                case "1.3.6.1.5.5.7.9.4":
                    ret = X509Name.CountryOfCitizenship;
                    break;
                case "1.3.6.1.5.5.7.9.5":
                    ret = X509Name.CountryOfResidence;
                    break;
                case "1.3.36.8.3.14":
                    ret = X509Name.NameAtBirth;
                    break;
                case "2.5.4.16":
                    ret = X509Name.PostalAddress;
                    break;
                case "2.5.4.54":
                    ret = X509Name.DmdName;
                    break;
                case "2.5.4.20":
                    ret = X509Name.TelephoneNumber;
                    break;
                case "2.5.4.41":
                    ret = X509Name.Name;
                    break;
                case "1.2.840.113549.1.9.1":
                    ret = X509Name.E;
                    break;
                case "0.9.2342.19200300.100.1.25":
                    ret = X509Name.DC;
                    break;
                case "0.9.2342.19200300.100.1.1":
                    ret = X509Name.UID;
                    break;
                default:
                    ret = X509Name.None;
                    break;
            }
            return ret;
        }
    }
}