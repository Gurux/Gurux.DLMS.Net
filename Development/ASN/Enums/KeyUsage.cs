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
    /// Key Usage.
    /// </summary>
    [Flags]
    public enum KeyUsage
    {
        ///<summary>
        ///Key is not used.
        ///</summary>
        None = 0,
        ///<summary>
        ///Digital signature.
        ///</summary>
        DigitalSignature = 0x1,
        ///<summary>
        ///Non Repudiation.
        ///</summary>
        NonRepudiation = 0x2,
        ///<summary>
        ///Key encipherment.
        ///</summary>
        KeyEncipherment = 0x4,
        ///<summary>
        ///Data encipherment.
        ///</summary>
        DataEncipherment = 0x8,
        ///<summary>
        ///Key agreement.
        ///</summary>
        KeyAgreement = 0x10,
        ///<summary>
        ///Used with CA certificates when the subject public key is used to verify a signature on certificates.
        ///</summary>
        KeyCertSign = 0x20,
        ///<summary>
        ///Used when the subject public key is to verify a signature.
        ///</summary>
        CrlSign = 0x40,
        ///<summary>
        ///Encipher only.
        ///</summary>
        EncipherOnly = 0x80,
        ///<summary>
        ///Decipher only.
        ///</summary>
        DecipherOnly = 0x100
    }
}