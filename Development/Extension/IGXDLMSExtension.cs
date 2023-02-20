//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Version:         $Revision: 13534 $,
//                  $Date: 2023-02-06 11:47:17 +0200 (ma, 06 helmi 2023) $
//                  $Author: gurux01 $
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
// More information of Gurux DLMS/COSEM Director: http://www.gurux.org/GXDLMSDirector
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------
namespace Gurux.DLMS.Extension
{
    public interface IGXDLMSExtension
    {
        /// <summary>
        /// Get keys from external storage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void GetKeys(object sender, GXCryptoKeyParameter args);

        /// <summary>
        /// Called to encrypt or decrypt the data using external Hardware Security Module.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Crypto arguments.</param>
        void Crypt(object sender, GXCryptoKeyParameter args);
    }
}
