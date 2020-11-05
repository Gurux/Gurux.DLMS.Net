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

    /// <summary>
    /// X509 names.
    /// </summary>
    public enum X509Name
    {
        None,
        /// <summary>country code - StringType(SIZE(2)) </summary>
        C,
        /// <summary>organization - StringType(SIZE(1..64)) </summary>
        O,
        /// <summary>organizational unit name - StringType(SIZE(1..64)) </summary>
        OU,
        /// <summary>Title </summary>
        T,
        /// <summary>common name - StringType(SIZE(1..64)) </summary>
        CN,
        /// <summary>street - StringType(SIZE(1..64)) </summary>
        STREET,
        /// <summary>device serial number name - StringType(SIZE(1..64)) </summary>
        SerialNumber,
        /// <summary>locality name - StringType(SIZE(1..64)) </summary>
        L,
        /// <summary>state, or province name - StringType(SIZE(1..64)) </summary>
        ST,
        /// <summary>Naming attributes of type X520name </summary>
        SurName,
        /// <summary>Given name. </summary>
        GivenName,
        /// <summary>Initials. </summary>
        Initials,
        /// <summary>Generation. </summary>
        Generation,
        /// <summary>Unique identifier. </summary>
        UniqueIdentifier,
        /// <summary>businessCategory - DirectoryString(SIZE(1..128)) </summary>
        BusinessCategory,
        /// <summary>postalCode - DirectoryString(SIZE(1..40)) </summary>
        PostalCode,
        /// <summary>dnQualifier - DirectoryString(SIZE(1..64)) </summary>
        DnQualifier,
        /// <summary>RFC 3039 Pseudonym - DirectoryString(SIZE(1..64)) </summary>
        Pseudonym,
        /// <summary>RFC 3039 DateOfBirth - GeneralizedTime - YYYYMMDD000000Z </summary>
        DateOfBirth,
        /// <summary>RFC 3039 PlaceOfBirth - DirectoryString(SIZE(1..128)) </summary>
        PlaceOfBirth,
        /// <summary>RFC 3039 DateOfBirth - PrintableString (SIZE(1 -- "M", "F", "m" or "f") </summary>
        Gender,
        /// <summary>RFC 3039 CountryOfCitizenship - PrintableString (SIZE (2 -- ISO 3166)) codes only </summary>
        CountryOfCitizenship,
        /// <summary>RFC 3039 CountryOfCitizenship - PrintableString (SIZE (2 -- ISO 3166)) codes only </summary>
        CountryOfResidence,
        /// <summary>ISIS-MTT NameAtBirth - DirectoryString(SIZE(1..64)) </summary>
        NameAtBirth,
        /// <summary>RFC 3039 PostalAddress - SEQUENCE SIZE (1..6 OF DirectoryString(SIZE(1..30))) </summary>
        PostalAddress,
        /// <summary>RFC 2256 dmdName </summary>
        DmdName,
        /// <summary>id-at-telephoneNumber </summary>
        TelephoneNumber,
        /// <summary>id-at-name </summary>
        Name,
        /// <summary>email address in Verisign certificates </summary>
        E,
        DC,
        /// <summary>LDAP User id. </summary>
        UID
    }

    public static class X509NameConverter
    {
        public static string GetString(X509Name value)
        {
            switch (value)
            {
                case X509Name.C:
                    return "2.5.4.6";
                case X509Name.O:
                    return "2.5.4.10";
                case X509Name.OU:
                    return "2.5.4.11";
                case X509Name.T:
                    return "2.5.4.12";
                case X509Name.CN:
                    return "2.5.4.3";
                case X509Name.STREET:
                    return "2.5.4.9";
                case X509Name.SerialNumber:
                    return "2.5.4.5";
                case X509Name.L:
                    return "2.5.4.7";
                case X509Name.ST:
                    return "2.5.4.8";
                case X509Name.SurName:
                    return "2.5.4.4";
                case X509Name.GivenName:
                    return "2.5.4.42";
                case X509Name.Initials:
                    return "2.5.4.43";
                case X509Name.Generation:
                    return "2.5.4.44";
                case X509Name.UniqueIdentifier:
                    return "2.5.4.45";
                case X509Name.BusinessCategory:
                    return "2.5.4.15";
                case X509Name.PostalCode:
                    return "2.5.4.17";
                case X509Name.DnQualifier:
                    return "2.5.4.46";
                case X509Name.Pseudonym:
                    return "2.5.4.65";
                case X509Name.DateOfBirth:
                    return "1.3.6.1.5.5.7.9.1";
                case X509Name.PlaceOfBirth:
                    return "1.3.6.1.5.5.7.9.2";
                case X509Name.Gender:
                    return "1.3.6.1.5.5.7.9.3";
                case X509Name.CountryOfCitizenship:
                    return "1.3.6.1.5.5.7.9.4";
                case X509Name.CountryOfResidence:
                    return "1.3.6.1.5.5.7.9.5";
                case X509Name.NameAtBirth:
                    return "1.3.36.8.3.14";
                case X509Name.PostalAddress:
                    return "2.5.4.16";
                case X509Name.DmdName:
                    return "2.5.4.54";
                case X509Name.TelephoneNumber:
                    return "2.5.4.20";
                case X509Name.Name:
                    return "2.5.4.41";
                case X509Name.E:
                    return "1.2.840.113549.1.9.1";
                case X509Name.DC:
                    return "0.9.2342.19200300.100.1.25";
                case X509Name.UID:
                    return "0.9.2342.19200300.100.1.1";
                default:
                    throw new ArgumentOutOfRangeException("Invalid X509Name. " + value);
            }
        }

        public static X509Name FromString(string value)
        {
            if (value == "2.5.4.6")
            {
                return X509Name.C;
            }
            if (value == "2.5.4.10")
            {
                return X509Name.O;
            }
            if (value == "2.5.4.11")
            {
                return X509Name.OU;
            }
            if (value == "2.5.4.12")
            {
                return X509Name.T;
            }
            if (value == "2.5.4.3")
            {
                return X509Name.CN;
            }
            if (value == "2.5.4.9")
            {
                return X509Name.STREET;
            }
            if (value == "2.5.4.5")
            {
                return X509Name.SerialNumber;
            }
            if (value == "2.5.4.7")
            {
                return X509Name.L;
            }
            if (value == "2.5.4.8")
            {
                return X509Name.ST;
            }
            if (value == "2.5.4.4")
            {
                return X509Name.SurName;
            }
            if (value == "2.5.4.42")
            {
                return X509Name.GivenName;
            }
            if (value == "2.5.4.43")
            {
                return X509Name.Initials;
            }
            if (value == "2.5.4.44")
            {
                return X509Name.Generation;
            }
            if (value == "2.5.4.45")
            {
                return X509Name.UniqueIdentifier;
            }
            if (value == "2.5.4.15")
            {
                return X509Name.BusinessCategory;
            }
            if (value == "2.5.4.17")
            {
                return X509Name.PostalCode;
            }
            if (value == "2.5.4.46")
            {
                return X509Name.DnQualifier;
            }
            if (value == "2.5.4.65")
            {
                return X509Name.Pseudonym;
            }
            if (value == "1.3.6.1.5.5.7.9.1")
            {
                return X509Name.DateOfBirth;
            }
            if (value == "1.3.6.1.5.5.7.9.2")
            {
                return X509Name.PlaceOfBirth;
            }
            if (value == "1.3.6.1.5.5.7.9.3")
            {
                return X509Name.Gender;
            }
            if (value == "1.3.6.1.5.5.7.9.4")
            {
                return X509Name.CountryOfCitizenship;
            }

            if (value == "1.3.6.1.5.5.7.9.5")
            {
                return X509Name.CountryOfResidence;
            }

            if (value == "1.3.36.8.3.14")
            {
                return X509Name.NameAtBirth;
            }
            if (value == "2.5.4.16")
            {
                return X509Name.PostalAddress;
            }
            if (value == "2.5.4.54")
            {
                return X509Name.DmdName;
            }
            if (value == "2.5.4.20")
            {
                return X509Name.TelephoneNumber;
            }
            if (value == "2.5.4.41")
            {
                return X509Name.Name;
            }
            if (value == "1.2.840.113549.1.9.1")
            {
                return X509Name.E;
            }
            if (value == "0.9.2342.19200300.100.1.25")
            {
                return X509Name.DC;
            }
            if (value == "0.9.2342.19200300.100.1.1")
            {
                return X509Name.UID;
            }
            return X509Name.None;
        }
    }
}