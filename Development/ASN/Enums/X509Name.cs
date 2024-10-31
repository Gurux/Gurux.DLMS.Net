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
        /// <summary>
        /// Domain component
        /// </summary>
        DC,
        /// <summary>LDAP User id. </summary>
        UID
    }
}