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
    /// Hash algorithms.
    /// </summary>
    public enum HashAlgorithm
    {
        None,
        Sha1Rsa,
        Md5Rsa,
        Sha1Dsa,
        Sha1Rsa1,
        ShaRsa,
        Md5Rsa1,
        Md2Rsa1,
        Md4Rsa,
        Md4Rsa1,
        Md4Rsa2,
        Md2Rsa,
        Sha1Dsa1,
        DsaSha1,
        MosaicUpdatedSig,
        Sha1NoSign,
        Md5NoSign,
        Sha256NoSign,
        Sha384NoSign,
        Sha512NoSign,
        Sha256Rsa,
        Sha384Rsa,
        Sha512Rsa,
        RsaSsaPss,
        Sha1withecdsa,
        Sha256WithEcdsa,
        Sha384WithEcdsa,
        Sha512WithEcdsa,
        SpecifiedEcdsa
    }
    public static class HashAlgorithmConverter
    {
        public static string GetString(HashAlgorithm value)
        {
            switch (value)
            {
                case HashAlgorithm.Sha1Rsa:
                    return "1.2.840.113549.1.1.5";
                case HashAlgorithm.Md5Rsa:
                    return "1.2.840.113549.1.1.4";
                case HashAlgorithm.Sha1Dsa:
                    return "1.2.840.10040.4.3";
                case HashAlgorithm.Sha1Rsa1:
                    return "1.3.14.3.2.29";
                case HashAlgorithm.ShaRsa:
                    return "1.3.14.3.2.15";
                case HashAlgorithm.Md5Rsa1:
                    return "1.3.14.3.2.3";
                case HashAlgorithm.Md2Rsa1:
                    return "1.2.840.113549.1.1.2";
                case HashAlgorithm.Md4Rsa:
                    return "1.2.840.113549.1.1.3";
                case HashAlgorithm.Md4Rsa1:
                    return "1.3.14.3.2.2";
                case HashAlgorithm.Md4Rsa2:
                    return "1.3.14.3.2.4";
                case HashAlgorithm.Md2Rsa:
                    return "1.3.14.7.2.3.1";
                case HashAlgorithm.Sha1Dsa1:
                    return "1.3.14.3.2.13";
                case HashAlgorithm.DsaSha1:
                    return "1.3.14.3.2.27";
                case HashAlgorithm.MosaicUpdatedSig:
                    return "2.16.840.1.101.2.1.1.19";
                case HashAlgorithm.Sha1NoSign:
                    return "1.3.14.3.2.26";
                case HashAlgorithm.Md5NoSign:
                    return "1.2.840.113549.2.5";
                case HashAlgorithm.Sha256NoSign:
                    return "2.16.840.1.101.3.4.2.1";
                case HashAlgorithm.Sha384NoSign:
                    return "2.16.840.1.101.3.4.2.2";
                case HashAlgorithm.Sha512NoSign:
                    return "2.16.840.1.101.3.4.2.3";
                case HashAlgorithm.Sha256Rsa:
                    return "1.2.840.113549.1.1.11";
                case HashAlgorithm.Sha384Rsa:
                    return "1.2.840.113549.1.1.12";
                case HashAlgorithm.Sha512Rsa:
                    return "1.2.840.113549.1.1.13";
                case HashAlgorithm.RsaSsaPss:
                    return "1.2.840.113549.1.1.10";
                case HashAlgorithm.Sha1withecdsa:
                    return "1.2.840.10045.4.1";
                case HashAlgorithm.Sha256WithEcdsa:
                    return "1.2.840.10045.4.3.2";
                case HashAlgorithm.Sha384WithEcdsa:
                    return "1.2.840.10045.4.3.3";
                case HashAlgorithm.Sha512WithEcdsa:
                    return "1.2.840.10045.4.3.4";
                case HashAlgorithm.SpecifiedEcdsa:
                    return "1.2.840.10045.4.3";
                default:
                    throw new ArgumentOutOfRangeException("Invalid HashAlgorithm. " + value);
            }
        }

        public static HashAlgorithm FromString(string value)
        {
            switch (value)
            {
                case "1.2.840.113549.1.1.5":
                    return HashAlgorithm.Sha1Rsa;
                case "1.2.840.113549.1.1.4":
                    return HashAlgorithm.Md5Rsa;
                case "1.2.840.10040.4.3":
                    return HashAlgorithm.Sha1Dsa;
                case "1.3.14.3.2.29":
                    return HashAlgorithm.Sha1Rsa1;
                case "1.3.14.3.2.15":
                    return HashAlgorithm.ShaRsa;
                case "1.3.14.3.2.3":
                    return HashAlgorithm.Md5Rsa1;
                case "1.2.840.113549.1.1.2":
                    return HashAlgorithm.Md2Rsa1;
                case "1.2.840.113549.1.1.3":
                    return HashAlgorithm.Md4Rsa;
                case "1.3.14.3.2.2":
                    return HashAlgorithm.Md4Rsa1;
                case "1.3.14.3.2.4":
                    return HashAlgorithm.Md4Rsa2;
                case "1.3.14.7.2.3.1":
                    return HashAlgorithm.Md2Rsa;
                case "1.3.14.3.2.13":
                    return HashAlgorithm.Sha1Dsa1;
                case "1.3.14.3.2.27":
                    return HashAlgorithm.DsaSha1;
                case "2.16.840.1.101.2.1.1.19":
                    return HashAlgorithm.MosaicUpdatedSig;
                case "1.3.14.3.2.26":
                    return HashAlgorithm.Sha1NoSign;
                case "1.2.840.113549.2.5":
                    return HashAlgorithm.Md5NoSign;
                case "2.16.840.1.101.3.4.2.1":
                    return HashAlgorithm.Sha256NoSign;
                case "2.16.840.1.101.3.4.2.2":
                    return HashAlgorithm.Sha384NoSign;
                case "2.16.840.1.101.3.4.2.3":
                    return HashAlgorithm.Sha512NoSign;
                case "1.2.840.113549.1.1.11":
                    return HashAlgorithm.Sha256Rsa;
                case "1.2.840.113549.1.1.12":
                    return HashAlgorithm.Sha384Rsa;
                case "1.2.840.113549.1.1.13":
                    return HashAlgorithm.Sha512Rsa;
                case "1.2.840.113549.1.1.10":
                    return HashAlgorithm.RsaSsaPss;
                case "1.2.840.10045.4.1":
                    return HashAlgorithm.Sha1withecdsa;
                case "1.2.840.10045.4.3.2":
                    return HashAlgorithm.Sha256WithEcdsa;
                case "1.2.840.10045.4.3.3":
                    return HashAlgorithm.Sha384WithEcdsa;
                case "1.2.840.10045.4.3.4":
                    return HashAlgorithm.Sha512WithEcdsa;
                case "1.2.840.10045.4.3":
                    return HashAlgorithm.SpecifiedEcdsa;
                default:
                    return HashAlgorithm.None;
            }
        }
    }
}