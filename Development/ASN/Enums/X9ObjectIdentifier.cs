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

namespace Gurux.DLMS.ASN.Enums
{

    /// <summaryX9 object identifiers. </summary>
    public enum X9ObjectIdentifier
    {
        None,
        IdFieldType,
        PrimeField,
        CharacteristicTwoField,
        GNBasis,
        TPBasis,
        PPBasis,
        ECDsaWithSha1,
        IdECPublicKey,
        ECDsaWithSha2,
        ECDsaWithSha224,
        ECDsaWithSha256,
        ECDsaWithSha384,
        ECDsaWithSha512,
        EllipticCurve,
        CTwoCurve,
        C2Pnb163v1,
        C2Pnb163v2,
        C2Pnb163v3,
        C2Pnb176w1,
        C2Tnb191v1,
        C2Tnb191v2,
        C2Tnb191v3,
        C2Onb191v4,
        C2Onb191v5,
        C2Pnb208w1,
        C2Tnb239v1,
        C2Tnb239v2,
        C2Tnb239v3,
        C2Onb239v4,
        C2Onb239v5,
        C2Pnb272w1,
        C2Pnb304w1,
        C2Tnb359v1,
        C2Pnb368w1,
        C2Tnb431r1,
        PrimeCurve,
        Prime192v1,
        Prime192v2,
        Prime192v3,
        Prime239v1,
        Prime239v2,
        Prime239v3,
        Prime256v1,
        IdDsa,
        IdDsaWithSha1,
        X9x63Scheme,
        DHSinglePassStdDHSha1KdfScheme,
        DHSinglePassCofactorDHSha1KdfScheme,
        MqvSinglePassSha1KdfScheme,
        ansi_x9_42,
        DHPublicNumber,
        X9x42Schemes,
        DHStatic,
        DHEphem,
        DHOneFlow,
        DHHybrid1,
        DHHybrid2,
        DHHybridOneFlow,
        Mqv2,
        Mqv1,
        Secp384r1
    }
}