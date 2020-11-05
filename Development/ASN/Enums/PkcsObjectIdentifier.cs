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

    public enum PkcsObjectIdentifier
    {
        None,
        RsaEncryption,
        MD2WithRsaEncryption,
        MD4WithRsaEncryption,
        MD5WithRsaEncryption,
        Sha1WithRsaEncryption,
        SrsaOaepEncryptionSet,
        IdRsaesOaep,
        IdMgf1,
        IdPSpecified,
        IdRsassaPss,
        Sha256WithRsaEncryption,
        Sha384WithRsaEncryption,
        Sha512WithRsaEncryption,
        Sha224WithRsaEncryption,
        DhKeyAgree1ment,
        PbeWithMD2AndDesCbc,
        PbeWithMD2AndRC2Cbc,
        PbeWithMD5AndDesCbc,
        PbeWithMD5AndRC2Cbc,
        PbeWithSha1AndDesCbc,
        PbeWithSha1AndRC2Cbc,
        IdPbeS2,
        IdPbkdf2,
        DesEde3Cbc,
        RC2Cbc,
        MD2,
        MD4,
        MD5,
        IdHmacWithSha1,
        IdHmacWithSha224,
        IdHmacWithSha256,
        IdHmacWithSha384,
        IdHmacWithSha512,
        Data,
        SignedData,
        EnvelopedData,
        SignedAndEnvelopedData,
        DigestedData,
        EncryptedData,
        Pkcs9AtEmailAddress,
        Pkcs9AtUnstructuredName,
        Pkcs9AtContentType,
        Pkcs9AtMessageDigest,
        Pkcs9AtSigningTime,
        Pkcs9AtCounterSignature,
        Pkcs9AtChallengePassword,
        Pkcs9AtUnstructuredAddress,
        Pkcs9AtExtendedCertificateAttributes,
        Pkcs9AtSigningDescription,
        Pkcs9AtExtensionRequest,
        Pkcs9AtSmimeCapabilities,
        IdSmime,
        Pkcs9AtFriendlyName,
        Pkcs9AtLocalKeyID,
        X509Certificate,
        SdsiCertificate,
        X509Crl,
        IdAlg,
        IdAlgEsdh,
        IdAlgCms3DesWrap,
        IdAlgCmsRC2Wrap,
        IdAlgPwriKek,
        IdAlgSsdh,
        IdRsaKem,
        PreferSignedData,
        CannotDecryptAny,
        SmimeCapabilitiesVersions,
        IdAAReceiptRequest,
        IdCTAuthData,
        IdCTTstInfo,
        IdCTCompressedData,
        IdCTAuthEnvelopedData,
        IdCTTimestampedData,
        IdCtiEtsProofOfOrigin,
        IdCtiEtsProofOfReceipt,
        IdCtiEtsProofOfDelivery,
        IdCtiEtsProofOfSender,
        IdCtiEtsProofOfApproval,
        IdCtiEtsProofOfCreation,
        IdAAContentHint,
        IdAAMsgSigDigest,
        IdAAContentReference,
        IdAAEncrypKeyPref,
        IdAASigningCertificate,
        IdAASigningCertificateV2,
        IdAAContentIdentifier,
        IdAASignatureTimeStampToken,
        IdAAEtsSigPolicyID,
        IdAAEtsCommitmentType,
        IdAAEtsSignerLocation,
        IdAAEtsSignerAttr,
        IdAAEtsOtherSigCert,
        IdAAEtsContentTimestamp,
        IdAAEtsCertificateRefs,
        IdAAEtsRevocationRefs,
        IdAAEtsCertValues,
        IdAAEtsRevocationValues,
        IdAAEtsEscTimeStamp,
        IdAAEtsCertCrlTimestamp,
        IdAAEtsArchiveTimestamp,
        IdSpqEtsUri,
        IdSpqEtsUNotice,
        KeyBag,
        Pkcs8ShroudedKeyBag,
        CertBag,
        CrlBag,
        SecretBag,
        SafeContentsBag,
        PbeWithShaAnd128BitRC4,
        PbeWithShaAnd40BitRC4,
        PbeWithShaAnd3KeyTripleDesCbc,
        PbeWithShaAnd2KeyTripleDesCbc,
        PbeWithShaAnd128BitRC2Cbc,
        PbewithShaAnd40BitRC2Cbc
    }

    public static class PkcsObjectIdentifierConverter
    {
        public static string GetString(PkcsObjectIdentifier value)
        {
            switch (value)
            {
                case PkcsObjectIdentifier.RsaEncryption:
                    return "1.2.840.113549.1.1.1";
                case PkcsObjectIdentifier.MD2WithRsaEncryption:
                    return "1.2.840.113549.1.1.2";
                case PkcsObjectIdentifier.MD4WithRsaEncryption:
                    return "1.2.840.113549.1.1.3";
                case PkcsObjectIdentifier.MD5WithRsaEncryption:
                    return "1.2.840.113549.1.1.4";
                case PkcsObjectIdentifier.Sha1WithRsaEncryption:
                    return "1.2.840.113549.1.1.5";
                case PkcsObjectIdentifier.SrsaOaepEncryptionSet:
                    return "1.2.840.113549.1.1.6";
                case PkcsObjectIdentifier.IdRsaesOaep:
                    return "1.2.840.113549.1.1.7";
                case PkcsObjectIdentifier.IdMgf1:
                    return "1.2.840.113549.1.1.8";
                case PkcsObjectIdentifier.IdPSpecified:
                    return "1.2.840.113549.1.1.9";
                case PkcsObjectIdentifier.IdRsassaPss:
                    return "1.2.840.113549.1.1.10";
                case PkcsObjectIdentifier.Sha256WithRsaEncryption:
                    return "1.2.840.113549.1.1.11";
                case PkcsObjectIdentifier.Sha384WithRsaEncryption:
                    return "1.2.840.113549.1.1.12";
                case PkcsObjectIdentifier.Sha512WithRsaEncryption:
                    return "1.2.840.113549.1.1.13";
                case PkcsObjectIdentifier.Sha224WithRsaEncryption:
                    return "1.2.840.113549.1.1.14";
                case PkcsObjectIdentifier.DhKeyAgree1ment:
                    return "1.2.840.113549.1.3.1";
                case PkcsObjectIdentifier.PbeWithMD2AndDesCbc:
                    return "1.2.840.113549.1.5.1";
                case PkcsObjectIdentifier.PbeWithMD2AndRC2Cbc:
                    return "1.2.840.113549.1.5.4";
                case PkcsObjectIdentifier.PbeWithMD5AndDesCbc:
                    return "1.2.840.113549.1.5.3";
                case PkcsObjectIdentifier.PbeWithMD5AndRC2Cbc:
                    return "1.2.840.113549.1.5.6";
                case PkcsObjectIdentifier.PbeWithSha1AndDesCbc:
                    return "1.2.840.113549.1.5.10";
                case PkcsObjectIdentifier.PbeWithSha1AndRC2Cbc:
                    return "1.2.840.113549.1.5.11";
                case PkcsObjectIdentifier.IdPbeS2:
                    return "1.2.840.113549.1.5.13";
                case PkcsObjectIdentifier.IdPbkdf2:
                    return "1.2.840.113549.1.5.12";
                case PkcsObjectIdentifier.DesEde3Cbc:
                    return "1.2.840.113549.3.7";
                case PkcsObjectIdentifier.RC2Cbc:
                    return "1.2.840.113549.3.2";
                case PkcsObjectIdentifier.MD2:
                    return "1.2.840.113549.2.2";
                case PkcsObjectIdentifier.MD4:
                    return "1.2.840.113549.2.4";
                case PkcsObjectIdentifier.MD5:
                    return "1.2.840.113549.2.5";
                case PkcsObjectIdentifier.IdHmacWithSha1:
                    return "1.2.840.113549.2.7";
                case PkcsObjectIdentifier.IdHmacWithSha224:
                    return "1.2.840.113549.2.8";
                case PkcsObjectIdentifier.IdHmacWithSha256:
                    return "1.2.840.113549.2.9";
                case PkcsObjectIdentifier.IdHmacWithSha384:
                    return "1.2.840.113549.2.10";
                case PkcsObjectIdentifier.IdHmacWithSha512:
                    return "1.2.840.113549.2.11";
                case PkcsObjectIdentifier.Data:
                    return "1.2.840.113549.1.7.1";
                case PkcsObjectIdentifier.SignedData:
                    return "1.2.840.113549.1.7.2";
                case PkcsObjectIdentifier.EnvelopedData:
                    return "1.2.840.113549.1.7.3";
                case PkcsObjectIdentifier.SignedAndEnvelopedData:
                    return "1.2.840.113549.1.7.4";
                case PkcsObjectIdentifier.DigestedData:
                    return "1.2.840.113549.1.7.5";
                case PkcsObjectIdentifier.EncryptedData:
                    return "1.2.840.113549.1.7.6";
                case PkcsObjectIdentifier.Pkcs9AtEmailAddress:
                    return "1.2.840.113549.1.9.1";
                case PkcsObjectIdentifier.Pkcs9AtUnstructuredName:
                    return "1.2.840.113549.1.9.2";
                case PkcsObjectIdentifier.Pkcs9AtContentType:
                    return "1.2.840.113549.1.9.3";
                case PkcsObjectIdentifier.Pkcs9AtMessageDigest:
                    return "1.2.840.113549.1.9.4";
                case PkcsObjectIdentifier.Pkcs9AtSigningTime:
                    return "1.2.840.113549.1.9.5";
                case PkcsObjectIdentifier.Pkcs9AtCounterSignature:
                    return "1.2.840.113549.1.9.6";
                case PkcsObjectIdentifier.Pkcs9AtChallengePassword:
                    return "1.2.840.113549.1.9.7";
                case PkcsObjectIdentifier.Pkcs9AtUnstructuredAddress:
                    return "1.2.840.113549.1.9.8";
                case PkcsObjectIdentifier.Pkcs9AtExtendedCertificateAttributes:
                    return "1.2.840.113549.1.9.9";
                case PkcsObjectIdentifier.Pkcs9AtSigningDescription:
                    return "1.2.840.113549.1.9.13";
                case PkcsObjectIdentifier.Pkcs9AtExtensionRequest:
                    return "1.2.840.113549.1.9.14";
                case PkcsObjectIdentifier.Pkcs9AtSmimeCapabilities:
                    return "1.2.840.113549.1.9.15";
                case PkcsObjectIdentifier.IdSmime:
                    return "1.2.840.113549.1.9.16";
                case PkcsObjectIdentifier.Pkcs9AtFriendlyName:
                    return "1.2.840.113549.1.9.20";
                case PkcsObjectIdentifier.Pkcs9AtLocalKeyID:
                    return "1.2.840.113549.1.9.21";
                case PkcsObjectIdentifier.X509Certificate:
                    return "1.2.840.113549.1.9.22.1";
                case PkcsObjectIdentifier.SdsiCertificate:
                    return "1.2.840.113549.1.9.22.2";
                case PkcsObjectIdentifier.X509Crl:
                    return "1.2.840.113549.1.9.23.1";
                case PkcsObjectIdentifier.IdAlg:
                    return "1.2.840.113549.1.9.16.3";
                case PkcsObjectIdentifier.IdAlgEsdh:
                    return "1.2.840.113549.1.9.16.3.5";
                case PkcsObjectIdentifier.IdAlgCms3DesWrap:
                    return "1.2.840.113549.1.9.16.3.6";
                case PkcsObjectIdentifier.IdAlgCmsRC2Wrap:
                    return "1.2.840.113549.1.9.16.3.7";
                case PkcsObjectIdentifier.IdAlgPwriKek:
                    return "1.2.840.113549.1.9.16.3.9";
                case PkcsObjectIdentifier.IdAlgSsdh:
                    return "1.2.840.113549.1.9.16.3.10";
                case PkcsObjectIdentifier.IdRsaKem:
                    return "1.2.840.113549.1.9.16.3.14";
                case PkcsObjectIdentifier.PreferSignedData:
                    return "1.2.840.113549.1.9.15.1";
                case PkcsObjectIdentifier.CannotDecryptAny:
                    return "1.2.840.113549.1.9.15.2";
                case PkcsObjectIdentifier.SmimeCapabilitiesVersions:
                    return "1.2.840.113549.1.9.15.3";
                case PkcsObjectIdentifier.IdAAReceiptRequest:
                    return "1.2.840.113549.1.9.16.2.1";
                case PkcsObjectIdentifier.IdCTAuthData:
                    return "1.2.840.113549.1.9.16.1.2";
                case PkcsObjectIdentifier.IdCTTstInfo:
                    return "1.2.840.113549.1.9.16.1.4";
                case PkcsObjectIdentifier.IdCTCompressedData:
                    return "1.2.840.113549.1.9.16.1.9";
                case PkcsObjectIdentifier.IdCTAuthEnvelopedData:
                    return "1.2.840.113549.1.9.16.1.23";
                case PkcsObjectIdentifier.IdCTTimestampedData:
                    return "1.2.840.113549.1.9.16.1.31";
                case PkcsObjectIdentifier.IdCtiEtsProofOfOrigin:
                    return "1.2.840.113549.1.9.16.6.1";
                case PkcsObjectIdentifier.IdCtiEtsProofOfReceipt:
                    return "1.2.840.113549.1.9.16.6.2";
                case PkcsObjectIdentifier.IdCtiEtsProofOfDelivery:
                    return "1.2.840.113549.1.9.16.6.3";
                case PkcsObjectIdentifier.IdCtiEtsProofOfSender:
                    return "1.2.840.113549.1.9.16.6.4";
                case PkcsObjectIdentifier.IdCtiEtsProofOfApproval:
                    return "1.2.840.113549.1.9.16.6.5";
                case PkcsObjectIdentifier.IdCtiEtsProofOfCreation:
                    return "1.2.840.113549.1.9.16.6.6";
                case PkcsObjectIdentifier.IdAAContentHint:
                    return "1.2.840.113549.1.9.16.2.4";
                case PkcsObjectIdentifier.IdAAMsgSigDigest:
                    return "1.2.840.113549.1.9.16.2.5";
                case PkcsObjectIdentifier.IdAAContentReference:
                    return "1.2.840.113549.1.9.16.2.10";
                case PkcsObjectIdentifier.IdAAEncrypKeyPref:
                    return "1.2.840.113549.1.9.16.2.11";
                case PkcsObjectIdentifier.IdAASigningCertificate:
                    return "1.2.840.113549.1.9.16.2.12";
                case PkcsObjectIdentifier.IdAASigningCertificateV2:
                    return "1.2.840.113549.1.9.16.2.47";
                case PkcsObjectIdentifier.IdAAContentIdentifier:
                    return "1.2.840.113549.1.9.16.2.7";
                case PkcsObjectIdentifier.IdAASignatureTimeStampToken:
                    return "1.2.840.113549.1.9.16.2.14";
                case PkcsObjectIdentifier.IdAAEtsSigPolicyID:
                    return "1.2.840.113549.1.9.16.2.15";
                case PkcsObjectIdentifier.IdAAEtsCommitmentType:
                    return "1.2.840.113549.1.9.16.2.16";
                case PkcsObjectIdentifier.IdAAEtsSignerLocation:
                    return "1.2.840.113549.1.9.16.2.17";
                case PkcsObjectIdentifier.IdAAEtsSignerAttr:
                    return "1.2.840.113549.1.9.16.2.18";
                case PkcsObjectIdentifier.IdAAEtsOtherSigCert:
                    return "1.2.840.113549.1.9.16.2.19";
                case PkcsObjectIdentifier.IdAAEtsContentTimestamp:
                    return "1.2.840.113549.1.9.16.2.20";
                case PkcsObjectIdentifier.IdAAEtsCertificateRefs:
                    return "1.2.840.113549.1.9.16.2.21";
                case PkcsObjectIdentifier.IdAAEtsRevocationRefs:
                    return "1.2.840.113549.1.9.16.2.22";
                case PkcsObjectIdentifier.IdAAEtsCertValues:
                    return "1.2.840.113549.1.9.16.2.23";
                case PkcsObjectIdentifier.IdAAEtsRevocationValues:
                    return "1.2.840.113549.1.9.16.2.24";
                case PkcsObjectIdentifier.IdAAEtsEscTimeStamp:
                    return "1.2.840.113549.1.9.16.2.25";
                case PkcsObjectIdentifier.IdAAEtsCertCrlTimestamp:
                    return "1.2.840.113549.1.9.16.2.26";
                case PkcsObjectIdentifier.IdAAEtsArchiveTimestamp:
                    return "1.2.840.113549.1.9.16.2.27";
                case PkcsObjectIdentifier.IdSpqEtsUri:
                    return "1.2.840.113549.1.9.16.5.1";
                case PkcsObjectIdentifier.IdSpqEtsUNotice:
                    return "1.2.840.113549.1.9.16.5.2";
                case PkcsObjectIdentifier.KeyBag:
                    return "1.2.840.113549.1.12.10.1.1";
                case PkcsObjectIdentifier.Pkcs8ShroudedKeyBag:
                    return "1.2.840.113549.1.12.10.1.2";
                case PkcsObjectIdentifier.CertBag:
                    return "1.2.840.113549.1.12.10.1.3";
                case PkcsObjectIdentifier.CrlBag:
                    return "1.2.840.113549.1.12.10.1.4";
                case PkcsObjectIdentifier.SecretBag:
                    return "1.2.840.113549.1.12.10.1.5";
                case PkcsObjectIdentifier.SafeContentsBag:
                    return "1.2.840.113549.1.12.10.1.6";
                case PkcsObjectIdentifier.PbeWithShaAnd128BitRC4:
                    return "1.2.840.113549.1.12.1.1";
                case PkcsObjectIdentifier.PbeWithShaAnd40BitRC4:
                    return "1.2.840.113549.1.12.1.2";
                case PkcsObjectIdentifier.PbeWithShaAnd3KeyTripleDesCbc:
                    return "1.2.840.113549.1.12.1.3";
                case PkcsObjectIdentifier.PbeWithShaAnd2KeyTripleDesCbc:
                    return "1.2.840.113549.1.12.1.4";
                case PkcsObjectIdentifier.PbeWithShaAnd128BitRC2Cbc:
                    return "1.2.840.113549.1.12.1.5";
                case PkcsObjectIdentifier.PbewithShaAnd40BitRC2Cbc:
                    return "1.2.840.113549.1.12.1.6";
                default:
                    return null;
            }
        }

        public static PkcsObjectIdentifier FromString(string value)
        {
            switch (value)
            {
                case "1.2.840.113549.1.1.1":
                    return PkcsObjectIdentifier.RsaEncryption;
                case "1.2.840.113549.1.1.2":
                    return PkcsObjectIdentifier.MD2WithRsaEncryption;
                case "1.2.840.113549.1.1.3":
                    return PkcsObjectIdentifier.MD4WithRsaEncryption;
                case "1.2.840.113549.1.1.4":
                    return PkcsObjectIdentifier.MD5WithRsaEncryption;
                case "1.2.840.113549.1.1.5":
                    return PkcsObjectIdentifier.Sha1WithRsaEncryption;
                case "1.2.840.113549.1.1.6":
                    return PkcsObjectIdentifier.SrsaOaepEncryptionSet;
                case "1.2.840.113549.1.1.7":
                    return PkcsObjectIdentifier.IdRsaesOaep;
                case "1.2.840.113549.1.1.8":
                    return PkcsObjectIdentifier.IdMgf1;
                case "1.2.840.113549.1.1.9":
                    return PkcsObjectIdentifier.IdPSpecified;
                case "1.2.840.113549.1.1.10":
                    return PkcsObjectIdentifier.IdRsassaPss;
                case "1.2.840.113549.1.1.11":
                    return PkcsObjectIdentifier.Sha256WithRsaEncryption;
                case "1.2.840.113549.1.1.12":
                    return PkcsObjectIdentifier.Sha384WithRsaEncryption;
                case "1.2.840.113549.1.1.13":
                    return PkcsObjectIdentifier.Sha512WithRsaEncryption;
                case "1.2.840.113549.1.1.14":
                    return PkcsObjectIdentifier.Sha224WithRsaEncryption;
                case "1.2.840.113549.1.3.1":
                    return PkcsObjectIdentifier.DhKeyAgree1ment;
                case "1.2.840.113549.1.5.1":
                    return PkcsObjectIdentifier.PbeWithMD2AndDesCbc;
                case "1.2.840.113549.1.5.4":
                    return PkcsObjectIdentifier.PbeWithMD2AndRC2Cbc;
                case "1.2.840.113549.1.5.3":
                    return PkcsObjectIdentifier.PbeWithMD5AndDesCbc;
                case "1.2.840.113549.1.5.6":
                    return PkcsObjectIdentifier.PbeWithMD5AndRC2Cbc;
                case "1.2.840.113549.1.5.10":
                    return PkcsObjectIdentifier.PbeWithSha1AndDesCbc;
                case "1.2.840.113549.1.5.11":
                    return PkcsObjectIdentifier.PbeWithSha1AndRC2Cbc;
                case "1.2.840.113549.1.5.13":
                    return PkcsObjectIdentifier.IdPbeS2;
                case "1.2.840.113549.1.5.12":
                    return PkcsObjectIdentifier.IdPbkdf2;
                case "1.2.840.113549.3.7":
                    return PkcsObjectIdentifier.DesEde3Cbc;
                case "1.2.840.113549.3.2":
                    return PkcsObjectIdentifier.RC2Cbc;
                case "1.2.840.113549.2.2":
                    return PkcsObjectIdentifier.MD2;
                case "1.2.840.113549.2.4":
                    return PkcsObjectIdentifier.MD4;
                case "1.2.840.113549.2.5":
                    return PkcsObjectIdentifier.MD5;
                case "1.2.840.113549.2.7":
                    return PkcsObjectIdentifier.IdHmacWithSha1;
                case "1.2.840.113549.2.8":
                    return PkcsObjectIdentifier.IdHmacWithSha224;
                case "1.2.840.113549.2.9":
                    return PkcsObjectIdentifier.IdHmacWithSha256;
                case "1.2.840.113549.2.10":
                    return PkcsObjectIdentifier.IdHmacWithSha384;
                case "1.2.840.113549.2.11":
                    return PkcsObjectIdentifier.IdHmacWithSha512;
                case "1.2.840.113549.1.7.1":
                    return PkcsObjectIdentifier.Data;
                case "1.2.840.113549.1.7.2":
                    return PkcsObjectIdentifier.SignedData;
                case "1.2.840.113549.1.7.3":
                    return PkcsObjectIdentifier.EnvelopedData;
                case "1.2.840.113549.1.7.4":
                    return PkcsObjectIdentifier.SignedAndEnvelopedData;
                case "1.2.840.113549.1.7.5":
                    return PkcsObjectIdentifier.DigestedData;
                case "1.2.840.113549.1.7.6":
                    return PkcsObjectIdentifier.EncryptedData;
                case "1.2.840.113549.1.9.1":
                    return PkcsObjectIdentifier.Pkcs9AtEmailAddress;
                case "1.2.840.113549.1.9.2":
                    return PkcsObjectIdentifier.Pkcs9AtUnstructuredName;
                case "1.2.840.113549.1.9.3":
                    return PkcsObjectIdentifier.Pkcs9AtContentType;
                case "1.2.840.113549.1.9.4":
                    return PkcsObjectIdentifier.Pkcs9AtMessageDigest;
                case "1.2.840.113549.1.9.5":
                    return PkcsObjectIdentifier.Pkcs9AtSigningTime;
                case "1.2.840.113549.1.9.6":
                    return PkcsObjectIdentifier.Pkcs9AtCounterSignature;
                case "1.2.840.113549.1.9.7":
                    return PkcsObjectIdentifier.Pkcs9AtChallengePassword;
                case "1.2.840.113549.1.9.8":
                    return PkcsObjectIdentifier.Pkcs9AtUnstructuredAddress;
                case "1.2.840.113549.1.9.9":
                    return PkcsObjectIdentifier.Pkcs9AtExtendedCertificateAttributes;
                case "1.2.840.113549.1.9.13":
                    return PkcsObjectIdentifier.Pkcs9AtSigningDescription;
                case "1.2.840.113549.1.9.14":
                    return PkcsObjectIdentifier.Pkcs9AtExtensionRequest;
                case "1.2.840.113549.1.9.15":
                    return PkcsObjectIdentifier.Pkcs9AtSmimeCapabilities;
                case "1.2.840.113549.1.9.16":
                    return PkcsObjectIdentifier.IdSmime;
                case "1.2.840.113549.1.9.20":
                    return PkcsObjectIdentifier.Pkcs9AtFriendlyName;
                case "1.2.840.113549.1.9.21":
                    return PkcsObjectIdentifier.Pkcs9AtLocalKeyID;
                case "1.2.840.113549.1.9.22.1":
                    return PkcsObjectIdentifier.X509Certificate;
                case "1.2.840.113549.1.9.22.2":
                    return PkcsObjectIdentifier.SdsiCertificate;
                case "1.2.840.113549.1.9.23.1":
                    return PkcsObjectIdentifier.X509Crl;
                case "1.2.840.113549.1.9.16.3":
                    return PkcsObjectIdentifier.IdAlg;
                case "1.2.840.113549.1.9.16.3.5":
                    return PkcsObjectIdentifier.IdAlgEsdh;
                case "1.2.840.113549.1.9.16.3.6":
                    return PkcsObjectIdentifier.IdAlgCms3DesWrap;
                case "1.2.840.113549.1.9.16.3.7":
                    return PkcsObjectIdentifier.IdAlgCmsRC2Wrap;
                case "1.2.840.113549.1.9.16.3.9":
                    return PkcsObjectIdentifier.IdAlgPwriKek;
                case "1.2.840.113549.1.9.16.3.10":
                    return PkcsObjectIdentifier.IdAlgSsdh;
                case "1.2.840.113549.1.9.16.3.14":
                    return PkcsObjectIdentifier.IdRsaKem;
                case "1.2.840.113549.1.9.15.1":
                    return PkcsObjectIdentifier.PreferSignedData;
                case "1.2.840.113549.1.9.15.2":
                    return PkcsObjectIdentifier.CannotDecryptAny;
                case "1.2.840.113549.1.9.15.3":
                    return PkcsObjectIdentifier.SmimeCapabilitiesVersions;
                case "1.2.840.113549.1.9.16.2.1":
                    return PkcsObjectIdentifier.IdAAReceiptRequest;
                case "1.2.840.113549.1.9.16.1.2":
                    return PkcsObjectIdentifier.IdCTAuthData;
                case "1.2.840.113549.1.9.16.1.4":
                    return PkcsObjectIdentifier.IdCTTstInfo;
                case "1.2.840.113549.1.9.16.1.9":
                    return PkcsObjectIdentifier.IdCTCompressedData;
                case "1.2.840.113549.1.9.16.1.23":
                    return PkcsObjectIdentifier.IdCTAuthEnvelopedData;
                case "1.2.840.113549.1.9.16.1.31":
                    return PkcsObjectIdentifier.IdCTTimestampedData;
                case "1.2.840.113549.1.9.16.6.1":
                    return PkcsObjectIdentifier.IdCtiEtsProofOfOrigin;
                case "1.2.840.113549.1.9.16.6.2":
                    return PkcsObjectIdentifier.IdCtiEtsProofOfReceipt;
                case "1.2.840.113549.1.9.16.6.3":
                    return PkcsObjectIdentifier.IdCtiEtsProofOfDelivery;
                case "1.2.840.113549.1.9.16.6.4":
                    return PkcsObjectIdentifier.IdCtiEtsProofOfSender;
                case "1.2.840.113549.1.9.16.6.5":
                    return PkcsObjectIdentifier.IdCtiEtsProofOfApproval;
                case "1.2.840.113549.1.9.16.6.6":
                    return PkcsObjectIdentifier.IdCtiEtsProofOfCreation;
                case "1.2.840.113549.1.9.16.2.4":
                    return PkcsObjectIdentifier.IdAAContentHint;
                case "1.2.840.113549.1.9.16.2.5":
                    return PkcsObjectIdentifier.IdAAMsgSigDigest;
                case "1.2.840.113549.1.9.16.2.10":
                    return PkcsObjectIdentifier.IdAAContentReference;
                case "1.2.840.113549.1.9.16.2.11":
                    return PkcsObjectIdentifier.IdAAEncrypKeyPref;
                case "1.2.840.113549.1.9.16.2.12":
                    return PkcsObjectIdentifier.IdAASigningCertificate;
                case "1.2.840.113549.1.9.16.2.47":
                    return PkcsObjectIdentifier.IdAASigningCertificateV2;
                case "1.2.840.113549.1.9.16.2.7":
                    return PkcsObjectIdentifier.IdAAContentIdentifier;
                case "1.2.840.113549.1.9.16.2.14":
                    return PkcsObjectIdentifier.IdAASignatureTimeStampToken;
                case "1.2.840.113549.1.9.16.2.15":
                    return PkcsObjectIdentifier.IdAAEtsSigPolicyID;
                case "1.2.840.113549.1.9.16.2.16":
                    return PkcsObjectIdentifier.IdAAEtsCommitmentType;
                case "1.2.840.113549.1.9.16.2.17":
                    return PkcsObjectIdentifier.IdAAEtsSignerLocation;
                case "1.2.840.113549.1.9.16.2.18":
                    return PkcsObjectIdentifier.IdAAEtsSignerAttr;
                case "1.2.840.113549.1.9.16.2.19":
                    return PkcsObjectIdentifier.IdAAEtsOtherSigCert;
                case "1.2.840.113549.1.9.16.2.20":
                    return PkcsObjectIdentifier.IdAAEtsContentTimestamp;
                case "1.2.840.113549.1.9.16.2.21":
                    return PkcsObjectIdentifier.IdAAEtsCertificateRefs;
                case "1.2.840.113549.1.9.16.2.22":
                    return PkcsObjectIdentifier.IdAAEtsRevocationRefs;
                case "1.2.840.113549.1.9.16.2.23":
                    return PkcsObjectIdentifier.IdAAEtsCertValues;
                case "1.2.840.113549.1.9.16.2.24":
                    return PkcsObjectIdentifier.IdAAEtsRevocationValues;
                case "1.2.840.113549.1.9.16.2.25":
                    return PkcsObjectIdentifier.IdAAEtsEscTimeStamp;
                case "1.2.840.113549.1.9.16.2.26":
                    return PkcsObjectIdentifier.IdAAEtsCertCrlTimestamp;
                case "1.2.840.113549.1.9.16.2.27":
                    return PkcsObjectIdentifier.IdAAEtsArchiveTimestamp;
                case "1.2.840.113549.1.9.16.5.1":
                    return PkcsObjectIdentifier.IdSpqEtsUri;
                case "1.2.840.113549.1.9.16.5.2":
                    return PkcsObjectIdentifier.IdSpqEtsUNotice;
                case "1.2.840.113549.1.12.10.1.1":
                    return PkcsObjectIdentifier.KeyBag;
                case "1.2.840.113549.1.12.10.1.2":
                    return PkcsObjectIdentifier.Pkcs8ShroudedKeyBag;
                case "1.2.840.113549.1.12.10.1.3":
                    return PkcsObjectIdentifier.CertBag;
                case "1.2.840.113549.1.12.10.1.4":
                    return PkcsObjectIdentifier.CrlBag;
                case "1.2.840.113549.1.12.10.1.5":
                    return PkcsObjectIdentifier.SecretBag;
                case "1.2.840.113549.1.12.10.1.6":
                    return PkcsObjectIdentifier.SafeContentsBag;
                case "1.2.840.113549.1.12.1.1":
                    return PkcsObjectIdentifier.PbeWithShaAnd128BitRC4;
                case "1.2.840.113549.1.12.1.2":
                    return PkcsObjectIdentifier.PbeWithShaAnd40BitRC4;
                case "1.2.840.113549.1.12.1.3":
                    return PkcsObjectIdentifier.PbeWithShaAnd3KeyTripleDesCbc;
                case "1.2.840.113549.1.12.1.4":
                    return PkcsObjectIdentifier.PbeWithShaAnd2KeyTripleDesCbc;
                case "1.2.840.113549.1.12.1.5":
                    return PkcsObjectIdentifier.PbeWithShaAnd128BitRC2Cbc;
                case "1.2.840.113549.1.12.1.6":
                    return PkcsObjectIdentifier.PbewithShaAnd40BitRC2Cbc;
                default:
                    return PkcsObjectIdentifier.None;
            }
        }
    }
}