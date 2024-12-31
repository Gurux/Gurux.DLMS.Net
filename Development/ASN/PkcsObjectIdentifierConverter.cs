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

namespace Gurux.DLMS.ASN
{
    public static class PkcsObjectIdentifierConverter
    {
        public static string GetString(PkcsObjectIdentifier value)
        {
            string ret;
            switch (value)
            {
                case PkcsObjectIdentifier.RsaEncryption:
                    ret = "1.2.840.113549.1.1.1";
                    break;
                case PkcsObjectIdentifier.MD2WithRsaEncryption:
                    ret = "1.2.840.113549.1.1.2";
                    break;
                case PkcsObjectIdentifier.MD4WithRsaEncryption:
                    ret = "1.2.840.113549.1.1.3";
                    break;
                case PkcsObjectIdentifier.MD5WithRsaEncryption:
                    ret = "1.2.840.113549.1.1.4";
                    break;
                case PkcsObjectIdentifier.Sha1WithRsaEncryption:
                    ret = "1.2.840.113549.1.1.5";
                    break;
                case PkcsObjectIdentifier.SrsaOaepEncryptionSet:
                    ret = "1.2.840.113549.1.1.6";
                    break;
                case PkcsObjectIdentifier.IdRsaesOaep:
                    ret = "1.2.840.113549.1.1.7";
                    break;
                case PkcsObjectIdentifier.IdMgf1:
                    ret = "1.2.840.113549.1.1.8";
                    break;
                case PkcsObjectIdentifier.IdPSpecified:
                    ret = "1.2.840.113549.1.1.9";
                    break;
                case PkcsObjectIdentifier.IdRsassaPss:
                    ret = "1.2.840.113549.1.1.10";
                    break;
                case PkcsObjectIdentifier.Sha256WithRsaEncryption:
                    ret = "1.2.840.113549.1.1.11";
                    break;
                case PkcsObjectIdentifier.Sha384WithRsaEncryption:
                    ret = "1.2.840.113549.1.1.12";
                    break;
                case PkcsObjectIdentifier.Sha512WithRsaEncryption:
                    ret = "1.2.840.113549.1.1.13";
                    break;
                case PkcsObjectIdentifier.Sha224WithRsaEncryption:
                    ret = "1.2.840.113549.1.1.14";
                    break;
                case PkcsObjectIdentifier.DhKeyAgree1ment:
                    ret = "1.2.840.113549.1.3.1";
                    break;
                case PkcsObjectIdentifier.PbeWithMD2AndDesCbc:
                    ret = "1.2.840.113549.1.5.1";
                    break;
                case PkcsObjectIdentifier.PbeWithMD2AndRC2Cbc:
                    ret = "1.2.840.113549.1.5.4";
                    break;
                case PkcsObjectIdentifier.PbeWithMD5AndDesCbc:
                    ret = "1.2.840.113549.1.5.3";
                    break;
                case PkcsObjectIdentifier.PbeWithMD5AndRC2Cbc:
                    ret = "1.2.840.113549.1.5.6";
                    break;
                case PkcsObjectIdentifier.PbeWithSha1AndDesCbc:
                    ret = "1.2.840.113549.1.5.10";
                    break;
                case PkcsObjectIdentifier.PbeWithSha1AndRC2Cbc:
                    ret = "1.2.840.113549.1.5.11";
                    break;
                case PkcsObjectIdentifier.IdPbeS2:
                    ret = "1.2.840.113549.1.5.13";
                    break;
                case PkcsObjectIdentifier.IdPbkdf2:
                    ret = "1.2.840.113549.1.5.12";
                    break;
                case PkcsObjectIdentifier.DesEde3Cbc:
                    ret = "1.2.840.113549.3.7";
                    break;
                case PkcsObjectIdentifier.RC2Cbc:
                    ret = "1.2.840.113549.3.2";
                    break;
                case PkcsObjectIdentifier.MD2:
                    ret = "1.2.840.113549.2.2";
                    break;
                case PkcsObjectIdentifier.MD4:
                    ret = "1.2.840.113549.2.4";
                    break;
                case PkcsObjectIdentifier.MD5:
                    ret = "1.2.840.113549.2.5";
                    break;
                case PkcsObjectIdentifier.IdHmacWithSha1:
                    ret = "1.2.840.113549.2.7";
                    break;
                case PkcsObjectIdentifier.IdHmacWithSha224:
                    ret = "1.2.840.113549.2.8";
                    break;
                case PkcsObjectIdentifier.IdHmacWithSha256:
                    ret = "1.2.840.113549.2.9";
                    break;
                case PkcsObjectIdentifier.IdHmacWithSha384:
                    ret = "1.2.840.113549.2.10";
                    break;
                case PkcsObjectIdentifier.IdHmacWithSha512:
                    ret = "1.2.840.113549.2.11";
                    break;
                case PkcsObjectIdentifier.Data:
                    ret = "1.2.840.113549.1.7.1";
                    break;
                case PkcsObjectIdentifier.SignedData:
                    ret = "1.2.840.113549.1.7.2";
                    break;
                case PkcsObjectIdentifier.EnvelopedData:
                    ret = "1.2.840.113549.1.7.3";
                    break;
                case PkcsObjectIdentifier.SignedAndEnvelopedData:
                    ret = "1.2.840.113549.1.7.4";
                    break;
                case PkcsObjectIdentifier.DigestedData:
                    ret = "1.2.840.113549.1.7.5";
                    break;
                case PkcsObjectIdentifier.EncryptedData:
                    ret = "1.2.840.113549.1.7.6";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtEmailAddress:
                    ret = "1.2.840.113549.1.9.1";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtUnstructuredName:
                    ret = "1.2.840.113549.1.9.2";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtContentType:
                    ret = "1.2.840.113549.1.9.3";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtMessageDigest:
                    ret = "1.2.840.113549.1.9.4";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtSigningTime:
                    ret = "1.2.840.113549.1.9.5";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtCounterSignature:
                    ret = "1.2.840.113549.1.9.6";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtChallengePassword:
                    ret = "1.2.840.113549.1.9.7";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtUnstructuredAddress:
                    ret = "1.2.840.113549.1.9.8";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtExtendedCertificateAttributes:
                    ret = "1.2.840.113549.1.9.9";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtSigningDescription:
                    ret = "1.2.840.113549.1.9.13";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtExtensionRequest:
                    ret = "1.2.840.113549.1.9.14";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtSmimeCapabilities:
                    ret = "1.2.840.113549.1.9.15";
                    break;
                case PkcsObjectIdentifier.IdSmime:
                    ret = "1.2.840.113549.1.9.16";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtFriendlyName:
                    ret = "1.2.840.113549.1.9.20";
                    break;
                case PkcsObjectIdentifier.Pkcs9AtLocalKeyID:
                    ret = "1.2.840.113549.1.9.21";
                    break;
                case PkcsObjectIdentifier.X509Certificate:
                    ret = "1.2.840.113549.1.9.22.1";
                    break;
                case PkcsObjectIdentifier.SdsiCertificate:
                    ret = "1.2.840.113549.1.9.22.2";
                    break;
                case PkcsObjectIdentifier.X509Crl:
                    ret = "1.2.840.113549.1.9.23.1";
                    break;
                case PkcsObjectIdentifier.IdAlg:
                    ret = "1.2.840.113549.1.9.16.3";
                    break;
                case PkcsObjectIdentifier.IdAlgEsdh:
                    ret = "1.2.840.113549.1.9.16.3.5";
                    break;
                case PkcsObjectIdentifier.IdAlgCms3DesWrap:
                    ret = "1.2.840.113549.1.9.16.3.6";
                    break;
                case PkcsObjectIdentifier.IdAlgCmsRC2Wrap:
                    ret = "1.2.840.113549.1.9.16.3.7";
                    break;
                case PkcsObjectIdentifier.IdAlgPwriKek:
                    ret = "1.2.840.113549.1.9.16.3.9";
                    break;
                case PkcsObjectIdentifier.IdAlgSsdh:
                    ret = "1.2.840.113549.1.9.16.3.10";
                    break;
                case PkcsObjectIdentifier.IdRsaKem:
                    ret = "1.2.840.113549.1.9.16.3.14";
                    break;
                case PkcsObjectIdentifier.PreferSignedData:
                    ret = "1.2.840.113549.1.9.15.1";
                    break;
                case PkcsObjectIdentifier.CannotDecryptAny:
                    ret = "1.2.840.113549.1.9.15.2";
                    break;
                case PkcsObjectIdentifier.SmimeCapabilitiesVersions:
                    ret = "1.2.840.113549.1.9.15.3";
                    break;
                case PkcsObjectIdentifier.IdAAReceiptRequest:
                    ret = "1.2.840.113549.1.9.16.2.1";
                    break;
                case PkcsObjectIdentifier.IdCTAuthData:
                    ret = "1.2.840.113549.1.9.16.1.2";
                    break;
                case PkcsObjectIdentifier.IdCTTstInfo:
                    ret = "1.2.840.113549.1.9.16.1.4";
                    break;
                case PkcsObjectIdentifier.IdCTCompressedData:
                    ret = "1.2.840.113549.1.9.16.1.9";
                    break;
                case PkcsObjectIdentifier.IdCTAuthEnvelopedData:
                    ret = "1.2.840.113549.1.9.16.1.23";
                    break;
                case PkcsObjectIdentifier.IdCTTimestampedData:
                    ret = "1.2.840.113549.1.9.16.1.31";
                    break;
                case PkcsObjectIdentifier.IdCtiEtsProofOfOrigin:
                    ret = "1.2.840.113549.1.9.16.6.1";
                    break;
                case PkcsObjectIdentifier.IdCtiEtsProofOfReceipt:
                    ret = "1.2.840.113549.1.9.16.6.2";
                    break;
                case PkcsObjectIdentifier.IdCtiEtsProofOfDelivery:
                    ret = "1.2.840.113549.1.9.16.6.3";
                    break;
                case PkcsObjectIdentifier.IdCtiEtsProofOfSender:
                    ret = "1.2.840.113549.1.9.16.6.4";
                    break;
                case PkcsObjectIdentifier.IdCtiEtsProofOfApproval:
                    ret = "1.2.840.113549.1.9.16.6.5";
                    break;
                case PkcsObjectIdentifier.IdCtiEtsProofOfCreation:
                    ret = "1.2.840.113549.1.9.16.6.6";
                    break;
                case PkcsObjectIdentifier.IdAAContentHint:
                    ret = "1.2.840.113549.1.9.16.2.4";
                    break;
                case PkcsObjectIdentifier.IdAAMsgSigDigest:
                    ret = "1.2.840.113549.1.9.16.2.5";
                    break;
                case PkcsObjectIdentifier.IdAAContentReference:
                    ret = "1.2.840.113549.1.9.16.2.10";
                    break;
                case PkcsObjectIdentifier.IdAAEncrypKeyPref:
                    ret = "1.2.840.113549.1.9.16.2.11";
                    break;
                case PkcsObjectIdentifier.IdAASigningCertificate:
                    ret = "1.2.840.113549.1.9.16.2.12";
                    break;
                case PkcsObjectIdentifier.IdAASigningCertificateV2:
                    ret = "1.2.840.113549.1.9.16.2.47";
                    break;
                case PkcsObjectIdentifier.IdAAContentIdentifier:
                    ret = "1.2.840.113549.1.9.16.2.7";
                    break;
                case PkcsObjectIdentifier.IdAASignatureTimeStampToken:
                    ret = "1.2.840.113549.1.9.16.2.14";
                    break;
                case PkcsObjectIdentifier.IdAAEtsSigPolicyID:
                    ret = "1.2.840.113549.1.9.16.2.15";
                    break;
                case PkcsObjectIdentifier.IdAAEtsCommitmentType:
                    ret = "1.2.840.113549.1.9.16.2.16";
                    break;
                case PkcsObjectIdentifier.IdAAEtsSignerLocation:
                    ret = "1.2.840.113549.1.9.16.2.17";
                    break;
                case PkcsObjectIdentifier.IdAAEtsSignerAttr:
                    ret = "1.2.840.113549.1.9.16.2.18";
                    break;
                case PkcsObjectIdentifier.IdAAEtsOtherSigCert:
                    ret = "1.2.840.113549.1.9.16.2.19";
                    break;
                case PkcsObjectIdentifier.IdAAEtsContentTimestamp:
                    ret = "1.2.840.113549.1.9.16.2.20";
                    break;
                case PkcsObjectIdentifier.IdAAEtsCertificateRefs:
                    ret = "1.2.840.113549.1.9.16.2.21";
                    break;
                case PkcsObjectIdentifier.IdAAEtsRevocationRefs:
                    ret = "1.2.840.113549.1.9.16.2.22";
                    break;
                case PkcsObjectIdentifier.IdAAEtsCertValues:
                    ret = "1.2.840.113549.1.9.16.2.23";
                    break;
                case PkcsObjectIdentifier.IdAAEtsRevocationValues:
                    ret = "1.2.840.113549.1.9.16.2.24";
                    break;
                case PkcsObjectIdentifier.IdAAEtsEscTimeStamp:
                    ret = "1.2.840.113549.1.9.16.2.25";
                    break;
                case PkcsObjectIdentifier.IdAAEtsCertCrlTimestamp:
                    ret = "1.2.840.113549.1.9.16.2.26";
                    break;
                case PkcsObjectIdentifier.IdAAEtsArchiveTimestamp:
                    ret = "1.2.840.113549.1.9.16.2.27";
                    break;
                case PkcsObjectIdentifier.IdSpqEtsUri:
                    ret = "1.2.840.113549.1.9.16.5.1";
                    break;
                case PkcsObjectIdentifier.IdSpqEtsUNotice:
                    ret = "1.2.840.113549.1.9.16.5.2";
                    break;
                case PkcsObjectIdentifier.KeyBag:
                    ret = "1.2.840.113549.1.12.10.1.1";
                    break;
                case PkcsObjectIdentifier.Pkcs8ShroudedKeyBag:
                    ret = "1.2.840.113549.1.12.10.1.2";
                    break;
                case PkcsObjectIdentifier.CertBag:
                    ret = "1.2.840.113549.1.12.10.1.3";
                    break;
                case PkcsObjectIdentifier.CrlBag:
                    ret = "1.2.840.113549.1.12.10.1.4";
                    break;
                case PkcsObjectIdentifier.SecretBag:
                    ret = "1.2.840.113549.1.12.10.1.5";
                    break;
                case PkcsObjectIdentifier.SafeContentsBag:
                    ret = "1.2.840.113549.1.12.10.1.6";
                    break;
                case PkcsObjectIdentifier.PbeWithShaAnd128BitRC4:
                    ret = "1.2.840.113549.1.12.1.1";
                    break;
                case PkcsObjectIdentifier.PbeWithShaAnd40BitRC4:
                    ret = "1.2.840.113549.1.12.1.2";
                    break;
                case PkcsObjectIdentifier.PbeWithShaAnd3KeyTripleDesCbc:
                    ret = "1.2.840.113549.1.12.1.3";
                    break;
                case PkcsObjectIdentifier.PbeWithShaAnd2KeyTripleDesCbc:
                    ret = "1.2.840.113549.1.12.1.4";
                    break;
                case PkcsObjectIdentifier.PbeWithShaAnd128BitRC2Cbc:
                    ret = "1.2.840.113549.1.12.1.5";
                    break;
                case PkcsObjectIdentifier.PbewithShaAnd40BitRC2Cbc:
                    ret = "1.2.840.113549.1.12.1.6";
                    break;
                default:
                    ret = null;
                    break;
            }
            return ret;
        }

        public static PkcsObjectIdentifier FromString(string value)
        {
            PkcsObjectIdentifier ret;
            switch (value)
            {
                case "1.2.840.113549.1.1.1":
                    ret = PkcsObjectIdentifier.RsaEncryption;
                    break;
                case "1.2.840.113549.1.1.2":
                    ret = PkcsObjectIdentifier.MD2WithRsaEncryption;
                    break;
                case "1.2.840.113549.1.1.3":
                    ret = PkcsObjectIdentifier.MD4WithRsaEncryption;
                    break;
                case "1.2.840.113549.1.1.4":
                    ret = PkcsObjectIdentifier.MD5WithRsaEncryption;
                    break;
                case "1.2.840.113549.1.1.5":
                    ret = PkcsObjectIdentifier.Sha1WithRsaEncryption;
                    break;
                case "1.2.840.113549.1.1.6":
                    ret = PkcsObjectIdentifier.SrsaOaepEncryptionSet;
                    break;
                case "1.2.840.113549.1.1.7":
                    ret = PkcsObjectIdentifier.IdRsaesOaep;
                    break;
                case "1.2.840.113549.1.1.8":
                    ret = PkcsObjectIdentifier.IdMgf1;
                    break;
                case "1.2.840.113549.1.1.9":
                    ret = PkcsObjectIdentifier.IdPSpecified;
                    break;
                case "1.2.840.113549.1.1.10":
                    ret = PkcsObjectIdentifier.IdRsassaPss;
                    break;
                case "1.2.840.113549.1.1.11":
                    ret = PkcsObjectIdentifier.Sha256WithRsaEncryption;
                    break;
                case "1.2.840.113549.1.1.12":
                    ret = PkcsObjectIdentifier.Sha384WithRsaEncryption;
                    break;
                case "1.2.840.113549.1.1.13":
                    ret = PkcsObjectIdentifier.Sha512WithRsaEncryption;
                    break;
                case "1.2.840.113549.1.1.14":
                    ret = PkcsObjectIdentifier.Sha224WithRsaEncryption;
                    break;
                case "1.2.840.113549.1.3.1":
                    ret = PkcsObjectIdentifier.DhKeyAgree1ment;
                    break;
                case "1.2.840.113549.1.5.1":
                    ret = PkcsObjectIdentifier.PbeWithMD2AndDesCbc;
                    break;
                case "1.2.840.113549.1.5.4":
                    ret = PkcsObjectIdentifier.PbeWithMD2AndRC2Cbc;
                    break;
                case "1.2.840.113549.1.5.3":
                    ret = PkcsObjectIdentifier.PbeWithMD5AndDesCbc;
                    break;
                case "1.2.840.113549.1.5.6":
                    ret = PkcsObjectIdentifier.PbeWithMD5AndRC2Cbc;
                    break;
                case "1.2.840.113549.1.5.10":
                    ret = PkcsObjectIdentifier.PbeWithSha1AndDesCbc;
                    break;
                case "1.2.840.113549.1.5.11":
                    ret = PkcsObjectIdentifier.PbeWithSha1AndRC2Cbc;
                    break;
                case "1.2.840.113549.1.5.13":
                    ret = PkcsObjectIdentifier.IdPbeS2;
                    break;
                case "1.2.840.113549.1.5.12":
                    ret = PkcsObjectIdentifier.IdPbkdf2;
                    break;
                case "1.2.840.113549.3.7":
                    ret = PkcsObjectIdentifier.DesEde3Cbc;
                    break;
                case "1.2.840.113549.3.2":
                    ret = PkcsObjectIdentifier.RC2Cbc;
                    break;
                case "1.2.840.113549.2.2":
                    ret = PkcsObjectIdentifier.MD2;
                    break;
                case "1.2.840.113549.2.4":
                    ret = PkcsObjectIdentifier.MD4;
                    break;
                case "1.2.840.113549.2.5":
                    ret = PkcsObjectIdentifier.MD5;
                    break;
                case "1.2.840.113549.2.7":
                    ret = PkcsObjectIdentifier.IdHmacWithSha1;
                    break;
                case "1.2.840.113549.2.8":
                    ret = PkcsObjectIdentifier.IdHmacWithSha224;
                    break;
                case "1.2.840.113549.2.9":
                    ret = PkcsObjectIdentifier.IdHmacWithSha256;
                    break;
                case "1.2.840.113549.2.10":
                    ret = PkcsObjectIdentifier.IdHmacWithSha384;
                    break;
                case "1.2.840.113549.2.11":
                    ret = PkcsObjectIdentifier.IdHmacWithSha512;
                    break;
                case "1.2.840.113549.1.7.1":
                    ret = PkcsObjectIdentifier.Data;
                    break;
                case "1.2.840.113549.1.7.2":
                    ret = PkcsObjectIdentifier.SignedData;
                    break;
                case "1.2.840.113549.1.7.3":
                    ret = PkcsObjectIdentifier.EnvelopedData;
                    break;
                case "1.2.840.113549.1.7.4":
                    ret = PkcsObjectIdentifier.SignedAndEnvelopedData;
                    break;
                case "1.2.840.113549.1.7.5":
                    ret = PkcsObjectIdentifier.DigestedData;
                    break;
                case "1.2.840.113549.1.7.6":
                    ret = PkcsObjectIdentifier.EncryptedData;
                    break;
                case "1.2.840.113549.1.9.1":
                    ret = PkcsObjectIdentifier.Pkcs9AtEmailAddress;
                    break;
                case "1.2.840.113549.1.9.2":
                    ret = PkcsObjectIdentifier.Pkcs9AtUnstructuredName;
                    break;
                case "1.2.840.113549.1.9.3":
                    ret = PkcsObjectIdentifier.Pkcs9AtContentType;
                    break;
                case "1.2.840.113549.1.9.4":
                    ret = PkcsObjectIdentifier.Pkcs9AtMessageDigest;
                    break;
                case "1.2.840.113549.1.9.5":
                    ret = PkcsObjectIdentifier.Pkcs9AtSigningTime;
                    break;
                case "1.2.840.113549.1.9.6":
                    ret = PkcsObjectIdentifier.Pkcs9AtCounterSignature;
                    break;
                case "1.2.840.113549.1.9.7":
                    ret = PkcsObjectIdentifier.Pkcs9AtChallengePassword;
                    break;
                case "1.2.840.113549.1.9.8":
                    ret = PkcsObjectIdentifier.Pkcs9AtUnstructuredAddress;
                    break;
                case "1.2.840.113549.1.9.9":
                    ret = PkcsObjectIdentifier.Pkcs9AtExtendedCertificateAttributes;
                    break;
                case "1.2.840.113549.1.9.13":
                    ret = PkcsObjectIdentifier.Pkcs9AtSigningDescription;
                    break;
                case "1.2.840.113549.1.9.14":
                    ret = PkcsObjectIdentifier.Pkcs9AtExtensionRequest;
                    break;
                case "1.2.840.113549.1.9.15":
                    ret = PkcsObjectIdentifier.Pkcs9AtSmimeCapabilities;
                    break;
                case "1.2.840.113549.1.9.16":
                    ret = PkcsObjectIdentifier.IdSmime;
                    break;
                case "1.2.840.113549.1.9.20":
                    ret = PkcsObjectIdentifier.Pkcs9AtFriendlyName;
                    break;
                case "1.2.840.113549.1.9.21":
                    ret = PkcsObjectIdentifier.Pkcs9AtLocalKeyID;
                    break;
                case "1.2.840.113549.1.9.22.1":
                    ret = PkcsObjectIdentifier.X509Certificate;
                    break;
                case "1.2.840.113549.1.9.22.2":
                    ret = PkcsObjectIdentifier.SdsiCertificate;
                    break;
                case "1.2.840.113549.1.9.23.1":
                    ret = PkcsObjectIdentifier.X509Crl;
                    break;
                case "1.2.840.113549.1.9.16.3":
                    ret = PkcsObjectIdentifier.IdAlg;
                    break;
                case "1.2.840.113549.1.9.16.3.5":
                    ret = PkcsObjectIdentifier.IdAlgEsdh;
                    break;
                case "1.2.840.113549.1.9.16.3.6":
                    ret = PkcsObjectIdentifier.IdAlgCms3DesWrap;
                    break;
                case "1.2.840.113549.1.9.16.3.7":
                    ret = PkcsObjectIdentifier.IdAlgCmsRC2Wrap;
                    break;
                case "1.2.840.113549.1.9.16.3.9":
                    ret = PkcsObjectIdentifier.IdAlgPwriKek;
                    break;
                case "1.2.840.113549.1.9.16.3.10":
                    ret = PkcsObjectIdentifier.IdAlgSsdh;
                    break;
                case "1.2.840.113549.1.9.16.3.14":
                    ret = PkcsObjectIdentifier.IdRsaKem;
                    break;
                case "1.2.840.113549.1.9.15.1":
                    ret = PkcsObjectIdentifier.PreferSignedData;
                    break;
                case "1.2.840.113549.1.9.15.2":
                    ret = PkcsObjectIdentifier.CannotDecryptAny;
                    break;
                case "1.2.840.113549.1.9.15.3":
                    ret = PkcsObjectIdentifier.SmimeCapabilitiesVersions;
                    break;
                case "1.2.840.113549.1.9.16.2.1":
                    ret = PkcsObjectIdentifier.IdAAReceiptRequest;
                    break;
                case "1.2.840.113549.1.9.16.1.2":
                    ret = PkcsObjectIdentifier.IdCTAuthData;
                    break;
                case "1.2.840.113549.1.9.16.1.4":
                    ret = PkcsObjectIdentifier.IdCTTstInfo;
                    break;
                case "1.2.840.113549.1.9.16.1.9":
                    ret = PkcsObjectIdentifier.IdCTCompressedData;
                    break;
                case "1.2.840.113549.1.9.16.1.23":
                    ret = PkcsObjectIdentifier.IdCTAuthEnvelopedData;
                    break;
                case "1.2.840.113549.1.9.16.1.31":
                    ret = PkcsObjectIdentifier.IdCTTimestampedData;
                    break;
                case "1.2.840.113549.1.9.16.6.1":
                    ret = PkcsObjectIdentifier.IdCtiEtsProofOfOrigin;
                    break;
                case "1.2.840.113549.1.9.16.6.2":
                    ret = PkcsObjectIdentifier.IdCtiEtsProofOfReceipt;
                    break;
                case "1.2.840.113549.1.9.16.6.3":
                    ret = PkcsObjectIdentifier.IdCtiEtsProofOfDelivery;
                    break;
                case "1.2.840.113549.1.9.16.6.4":
                    ret = PkcsObjectIdentifier.IdCtiEtsProofOfSender;
                    break;
                case "1.2.840.113549.1.9.16.6.5":
                    ret = PkcsObjectIdentifier.IdCtiEtsProofOfApproval;
                    break;
                case "1.2.840.113549.1.9.16.6.6":
                    ret = PkcsObjectIdentifier.IdCtiEtsProofOfCreation;
                    break;
                case "1.2.840.113549.1.9.16.2.4":
                    ret = PkcsObjectIdentifier.IdAAContentHint;
                    break;
                case "1.2.840.113549.1.9.16.2.5":
                    ret = PkcsObjectIdentifier.IdAAMsgSigDigest;
                    break;
                case "1.2.840.113549.1.9.16.2.10":
                    ret = PkcsObjectIdentifier.IdAAContentReference;
                    break;
                case "1.2.840.113549.1.9.16.2.11":
                    ret = PkcsObjectIdentifier.IdAAEncrypKeyPref;
                    break;
                case "1.2.840.113549.1.9.16.2.12":
                    ret = PkcsObjectIdentifier.IdAASigningCertificate;
                    break;
                case "1.2.840.113549.1.9.16.2.47":
                    ret = PkcsObjectIdentifier.IdAASigningCertificateV2;
                    break;
                case "1.2.840.113549.1.9.16.2.7":
                    ret = PkcsObjectIdentifier.IdAAContentIdentifier;
                    break;
                case "1.2.840.113549.1.9.16.2.14":
                    ret = PkcsObjectIdentifier.IdAASignatureTimeStampToken;
                    break;
                case "1.2.840.113549.1.9.16.2.15":
                    ret = PkcsObjectIdentifier.IdAAEtsSigPolicyID;
                    break;
                case "1.2.840.113549.1.9.16.2.16":
                    ret = PkcsObjectIdentifier.IdAAEtsCommitmentType;
                    break;
                case "1.2.840.113549.1.9.16.2.17":
                    ret = PkcsObjectIdentifier.IdAAEtsSignerLocation;
                    break;
                case "1.2.840.113549.1.9.16.2.18":
                    ret = PkcsObjectIdentifier.IdAAEtsSignerAttr;
                    break;
                case "1.2.840.113549.1.9.16.2.19":
                    ret = PkcsObjectIdentifier.IdAAEtsOtherSigCert;
                    break;
                case "1.2.840.113549.1.9.16.2.20":
                    ret = PkcsObjectIdentifier.IdAAEtsContentTimestamp;
                    break;
                case "1.2.840.113549.1.9.16.2.21":
                    ret = PkcsObjectIdentifier.IdAAEtsCertificateRefs;
                    break;
                case "1.2.840.113549.1.9.16.2.22":
                    ret = PkcsObjectIdentifier.IdAAEtsRevocationRefs;
                    break;
                case "1.2.840.113549.1.9.16.2.23":
                    ret = PkcsObjectIdentifier.IdAAEtsCertValues;
                    break;
                case "1.2.840.113549.1.9.16.2.24":
                    ret = PkcsObjectIdentifier.IdAAEtsRevocationValues;
                    break;
                case "1.2.840.113549.1.9.16.2.25":
                    ret = PkcsObjectIdentifier.IdAAEtsEscTimeStamp;
                    break;
                case "1.2.840.113549.1.9.16.2.26":
                    ret = PkcsObjectIdentifier.IdAAEtsCertCrlTimestamp;
                    break;
                case "1.2.840.113549.1.9.16.2.27":
                    ret = PkcsObjectIdentifier.IdAAEtsArchiveTimestamp;
                    break;
                case "1.2.840.113549.1.9.16.5.1":
                    ret = PkcsObjectIdentifier.IdSpqEtsUri;
                    break;
                case "1.2.840.113549.1.9.16.5.2":
                    ret = PkcsObjectIdentifier.IdSpqEtsUNotice;
                    break;
                case "1.2.840.113549.1.12.10.1.1":
                    ret = PkcsObjectIdentifier.KeyBag;
                    break;
                case "1.2.840.113549.1.12.10.1.2":
                    ret = PkcsObjectIdentifier.Pkcs8ShroudedKeyBag;
                    break;
                case "1.2.840.113549.1.12.10.1.3":
                    ret = PkcsObjectIdentifier.CertBag;
                    break;
                case "1.2.840.113549.1.12.10.1.4":
                    ret = PkcsObjectIdentifier.CrlBag;
                    break;
                case "1.2.840.113549.1.12.10.1.5":
                    ret = PkcsObjectIdentifier.SecretBag;
                    break;
                case "1.2.840.113549.1.12.10.1.6":
                    ret = PkcsObjectIdentifier.SafeContentsBag;
                    break;
                case "1.2.840.113549.1.12.1.1":
                    ret = PkcsObjectIdentifier.PbeWithShaAnd128BitRC4;
                    break;
                case "1.2.840.113549.1.12.1.2":
                    ret = PkcsObjectIdentifier.PbeWithShaAnd40BitRC4;
                    break;
                case "1.2.840.113549.1.12.1.3":
                    ret = PkcsObjectIdentifier.PbeWithShaAnd3KeyTripleDesCbc;
                    break;
                case "1.2.840.113549.1.12.1.4":
                    ret = PkcsObjectIdentifier.PbeWithShaAnd2KeyTripleDesCbc;
                    break;
                case "1.2.840.113549.1.12.1.5":
                    ret = PkcsObjectIdentifier.PbeWithShaAnd128BitRC2Cbc;
                    break;
                case "1.2.840.113549.1.12.1.6":
                    ret = PkcsObjectIdentifier.PbewithShaAnd40BitRC2Cbc;
                    break;
                default:
                    ret = PkcsObjectIdentifier.None;
                    break;
            }
            return ret;
        }
    }
}