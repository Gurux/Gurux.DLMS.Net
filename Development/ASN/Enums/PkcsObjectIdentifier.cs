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
}