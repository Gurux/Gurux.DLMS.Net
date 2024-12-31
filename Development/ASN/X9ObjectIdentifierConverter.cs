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
    public static class X9ObjectIdentifierConverter
    {
        public static string GetString(X9ObjectIdentifier value)
        {
            switch (value)
            {
                case X9ObjectIdentifier.IdFieldType:
                    return "1.2.840.10045.1";
                case X9ObjectIdentifier.PrimeField:
                    return "1.2.840.10045.1";
                case X9ObjectIdentifier.CharacteristicTwoField:
                    return "1.2.840.10045.1.2";
                case X9ObjectIdentifier.GNBasis:
                    return "1.2.840.10045.1.2.3.1";
                case X9ObjectIdentifier.TPBasis:
                    return "1.2.840.10045.1.2.3.2";
                case X9ObjectIdentifier.PPBasis:
                    return "1.2.840.10045.1.2.3.3";
                case X9ObjectIdentifier.ECDsaWithSha1:
                    return "1.2.840.10045.4.1";
                case X9ObjectIdentifier.IdECPublicKey:
                    return "1.2.840.10045.2.1";
                case X9ObjectIdentifier.ECDsaWithSha2:
                    return "1.2.840.10045.4.3";
                case X9ObjectIdentifier.ECDsaWithSha224:
                    return "1.2.840.10045.4.31";
                case X9ObjectIdentifier.ECDsaWithSha256:
                    return "1.2.840.10045.4.32";
                case X9ObjectIdentifier.ECDsaWithSha384:
                    return "1.2.840.10045.4.33";
                case X9ObjectIdentifier.ECDsaWithSha512:
                    return "1.2.840.10045.4.34";
                case X9ObjectIdentifier.EllipticCurve:
                    return "1.2.840.10045.3";
                case X9ObjectIdentifier.CTwoCurve:
                    return "1.2.840.10045.3.0";
                case X9ObjectIdentifier.C2Pnb163v1:
                    return "1.2.840.10045.3.0.1";
                case X9ObjectIdentifier.C2Pnb163v2:
                    return "1.2.840.10045.3.0.2";
                case X9ObjectIdentifier.C2Pnb163v3:
                    return "1.2.840.10045.3.0.3";
                case X9ObjectIdentifier.C2Pnb176w1:
                    return "1.2.840.10045.3.0.4";
                case X9ObjectIdentifier.C2Tnb191v1:
                    return "1.2.840.10045.3.0.5";
                case X9ObjectIdentifier.C2Tnb191v2:
                    return "1.2.840.10045.3.0.6";
                case X9ObjectIdentifier.C2Tnb191v3:
                    return "1.2.840.10045.3.0.7";
                case X9ObjectIdentifier.C2Onb191v4:
                    return "1.2.840.10045.3.0.8";
                case X9ObjectIdentifier.C2Onb191v5:
                    return "1.2.840.10045.3.0.9";
                case X9ObjectIdentifier.C2Pnb208w1:
                    return "1.2.840.10045.3.0.10";
                case X9ObjectIdentifier.C2Tnb239v1:
                    return "1.2.840.10045.3.0.11";
                case X9ObjectIdentifier.C2Tnb239v2:
                    return "1.2.840.10045.3.0.12";
                case X9ObjectIdentifier.C2Tnb239v3:
                    return "1.2.840.10045.3.0.13";
                case X9ObjectIdentifier.C2Onb239v4:
                    return "1.2.840.10045.3.0.14";
                case X9ObjectIdentifier.C2Onb239v5:
                    return "1.2.840.10045.3.0.15";
                case X9ObjectIdentifier.C2Pnb272w1:
                    return "1.2.840.10045.3.0.16";
                case X9ObjectIdentifier.C2Pnb304w1:
                    return "1.2.840.10045.3.0.17";
                case X9ObjectIdentifier.C2Tnb359v1:
                    return "1.2.840.10045.3.0.18";
                case X9ObjectIdentifier.C2Pnb368w1:
                    return "1.2.840.10045.3.0.19";
                case X9ObjectIdentifier.C2Tnb431r1:
                    return "1.2.840.10045.3.0.20";
                case X9ObjectIdentifier.PrimeCurve:
                    return "1.2.840.10045.3.1";
                case X9ObjectIdentifier.Prime192v1:
                    return "1.2.840.10045.3.1.1";
                case X9ObjectIdentifier.Prime192v2:
                    return "1.2.840.10045.3.1.2";
                case X9ObjectIdentifier.Prime192v3:
                    return "1.2.840.10045.3.1.3";
                case X9ObjectIdentifier.Prime239v1:
                    return "1.2.840.10045.3.1.4";
                case X9ObjectIdentifier.Prime239v2:
                    return "1.2.840.10045.3.1.5";
                case X9ObjectIdentifier.Prime239v3:
                    return "1.2.840.10045.3.1.6";
                case X9ObjectIdentifier.Prime256v1:
                    return "1.2.840.10045.3.1.7";
                case X9ObjectIdentifier.IdDsa:
                    return "1.2.840.10040.4.1";
                case X9ObjectIdentifier.IdDsaWithSha1:
                    return "1.2.840.10040.4.3";
                case X9ObjectIdentifier.X9x63Scheme:
                    return "1.3.133.16.840.63.0";
                case X9ObjectIdentifier.DHSinglePassStdDHSha1KdfScheme:
                    return "1.3.133.16.840.63.0.2";
                case X9ObjectIdentifier.DHSinglePassCofactorDHSha1KdfScheme:
                    return "1.3.133.16.840.63.0.3";
                case X9ObjectIdentifier.MqvSinglePassSha1KdfScheme:
                    return "1.3.133.16.840.63.0.16";
                case X9ObjectIdentifier.ansi_x9_42:
                    return "1.2.840.10046";
                case X9ObjectIdentifier.DHPublicNumber:
                    return "1.2.840.10046.2.1";
                case X9ObjectIdentifier.X9x42Schemes:
                    return "1.2.840.10046.2.3";
                case X9ObjectIdentifier.DHStatic:
                    return "1.2.840.10046.2.3.1";
                case X9ObjectIdentifier.DHEphem:
                    return "1.2.840.10046.2.3.2";
                case X9ObjectIdentifier.DHOneFlow:
                    return "1.2.840.10046.2.3.3";
                case X9ObjectIdentifier.DHHybrid1:
                    return "1.2.840.10046.2.3.4";
                case X9ObjectIdentifier.DHHybrid2:
                    return "1.2.840.10046.2.3.5";
                case X9ObjectIdentifier.DHHybridOneFlow:
                    return "1.2.840.10046.2.3.6";
                case X9ObjectIdentifier.Mqv2:
                    return "1.2.840.10046.2.3.7";
                case X9ObjectIdentifier.Mqv1:
                    return "1.2.840.10046.2.3.8";
                case X9ObjectIdentifier.Secp384r1:
                    return "1.3.132.0.34";
                default:
                    throw new ArgumentOutOfRangeException("Invalid X509Name. " + value);
            }
        }

        public static X9ObjectIdentifier FromString(string value)
        {
            if (value == "1.2.840.10045.1")
                return X9ObjectIdentifier.IdFieldType;
            if (value == "1.2.840.10045.1")
                return X9ObjectIdentifier.PrimeField;
            if (value == "1.2.840.10045.1.2")
                return X9ObjectIdentifier.CharacteristicTwoField;
            if (value == "1.2.840.10045.1.2.3.1")
                return X9ObjectIdentifier.GNBasis;
            if (value == "1.2.840.10045.1.2.3.2")
                return X9ObjectIdentifier.TPBasis;
            if (value == "1.2.840.10045.1.2.3.3")
                return X9ObjectIdentifier.PPBasis;
            if (value == "1.2.840.10045.4.1")
                return X9ObjectIdentifier.ECDsaWithSha1;
            if (value == "1.2.840.10045.2.1")
                return X9ObjectIdentifier.IdECPublicKey;
            if (value == "1.2.840.10045.4.3")
                return X9ObjectIdentifier.ECDsaWithSha2;
            if (value == "1.2.840.10045.4.31")
                return X9ObjectIdentifier.ECDsaWithSha224;
            if (value == "1.2.840.10045.4.32")
                return X9ObjectIdentifier.ECDsaWithSha256;
            if (value == "1.2.840.10045.4.33")
                return X9ObjectIdentifier.ECDsaWithSha384;
            if (value == "1.2.840.10045.4.34")
                return X9ObjectIdentifier.ECDsaWithSha512;
            if (value == "1.2.840.10045.3")
                return X9ObjectIdentifier.EllipticCurve;
            if (value == "1.2.840.10045.3.0")
                return X9ObjectIdentifier.CTwoCurve;
            if (value == "1.2.840.10045.3.0.1")
                return X9ObjectIdentifier.C2Pnb163v1;
            if (value == "1.2.840.10045.3.0.2")
                return X9ObjectIdentifier.C2Pnb163v2;
            if (value == "1.2.840.10045.3.0.3")
                return X9ObjectIdentifier.C2Pnb163v3;
            if (value == "1.2.840.10045.3.0.4")
                return X9ObjectIdentifier.C2Pnb176w1;
            if (value == "1.2.840.10045.3.0.5")
                return X9ObjectIdentifier.C2Tnb191v1;
            if (value == "1.2.840.10045.3.0.6")
                return X9ObjectIdentifier.C2Tnb191v2;
            if (value == "1.2.840.10045.3.0.7")
                return X9ObjectIdentifier.C2Tnb191v3;
            if (value == "1.2.840.10045.3.0.8")
                return X9ObjectIdentifier.C2Onb191v4;
            if (value == "1.2.840.10045.3.0.9")
                return X9ObjectIdentifier.C2Onb191v5;
            if (value == "1.2.840.10045.3.0.10")
                return X9ObjectIdentifier.C2Pnb208w1;
            if (value == "1.2.840.10045.3.0.11")
                return X9ObjectIdentifier.C2Tnb239v1;
            if (value == "1.2.840.10045.3.0.12")
                return X9ObjectIdentifier.C2Tnb239v2;
            if (value == "1.2.840.10045.3.0.13")
                return X9ObjectIdentifier.C2Tnb239v3;
            if (value == "1.2.840.10045.3.0.14")
                return X9ObjectIdentifier.C2Onb239v4;
            if (value == "1.2.840.10045.3.0.15")
                return X9ObjectIdentifier.C2Onb239v5;
            if (value == "1.2.840.10045.3.0.16")
                return X9ObjectIdentifier.C2Pnb272w1;
            if (value == "1.2.840.10045.3.0.17")
                return X9ObjectIdentifier.C2Pnb304w1;
            if (value == "1.2.840.10045.3.0.18")
                return X9ObjectIdentifier.C2Tnb359v1;
            if (value == "1.2.840.10045.3.0.19")
                return X9ObjectIdentifier.C2Pnb368w1;
            if (value == "1.2.840.10045.3.0.20")
                return X9ObjectIdentifier.C2Tnb431r1;
            if (value == "1.2.840.10045.3.1")
                return X9ObjectIdentifier.PrimeCurve;
            if (value == "1.2.840.10045.3.1.1")
                return X9ObjectIdentifier.Prime192v1;
            if (value == "1.2.840.10045.3.1.2")
                return X9ObjectIdentifier.Prime192v2;
            if (value == "1.2.840.10045.3.1.3")
                return X9ObjectIdentifier.Prime192v3;
            if (value == "1.2.840.10045.3.1.4")
                return X9ObjectIdentifier.Prime239v1;
            if (value == "1.2.840.10045.3.1.5")
                return X9ObjectIdentifier.Prime239v2;
            if (value == "1.2.840.10045.3.1.6")
                return X9ObjectIdentifier.Prime239v3;
            if (value == "1.2.840.10045.3.1.7")
                return X9ObjectIdentifier.Prime256v1;
            if (value == "1.2.840.10040.4.1")
                return X9ObjectIdentifier.IdDsa;
            if (value == "1.2.840.10040.4.3")
                return X9ObjectIdentifier.IdDsaWithSha1;
            if (value == "1.3.133.16.840.63.0")
                return X9ObjectIdentifier.X9x63Scheme;
            if (value == "1.3.133.16.840.63.0.2")
                return X9ObjectIdentifier.DHSinglePassStdDHSha1KdfScheme;
            if (value == "1.3.133.16.840.63.0.3")
                return X9ObjectIdentifier.DHSinglePassCofactorDHSha1KdfScheme;
            if (value == "1.3.133.16.840.63.0.16")
                return X9ObjectIdentifier.MqvSinglePassSha1KdfScheme;
            if (value == "1.2.840.10046")
                return X9ObjectIdentifier.ansi_x9_42;
            if (value == "1.2.840.10046.2.1")
                return X9ObjectIdentifier.DHPublicNumber;
            if (value == "1.2.840.10046.2.3")
                return X9ObjectIdentifier.X9x42Schemes;
            if (value == "1.2.840.10046.2.3.1")
                return X9ObjectIdentifier.DHStatic;
            if (value == "1.2.840.10046.2.3.2")
                return X9ObjectIdentifier.DHEphem;
            if (value == "1.2.840.10046.2.3.3")
                return X9ObjectIdentifier.DHOneFlow;
            if (value == "1.2.840.10046.2.3.4")
                return X9ObjectIdentifier.DHHybrid1;
            if (value == "1.2.840.10046.2.3.5")
                return X9ObjectIdentifier.DHHybrid2;
            if (value == "1.2.840.10046.2.3.6")
                return X9ObjectIdentifier.DHHybridOneFlow;
            if (value == "1.2.840.10046.2.3.7")
                return X9ObjectIdentifier.Mqv2;
            if (value == "1.2.840.10046.2.3.8")
                return X9ObjectIdentifier.Mqv1;
            if (value == "1.3.132.0.34")
                return X9ObjectIdentifier.Secp384r1;
            return X9ObjectIdentifier.None;
        }
    }
}