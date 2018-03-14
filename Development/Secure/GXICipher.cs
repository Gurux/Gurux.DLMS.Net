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
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
namespace Gurux.DLMS.Secure
{
    internal interface GXICipher
    {
        /// <summary>
        /// Encrypt PDU.
        /// </summary>
        /// <param name="tag">Command.</param>
        /// <param name="title">System title.</param>
        /// <param name="data">Data to encrypt.</param>
        /// <returns>Encrypted data.</returns>
        byte[] Encrypt(byte tag, byte[] title, byte[] data);

        /// <summary>
        /// Decrypt data.
        /// </summary>
        /// <param name="title">System title.</param>
        /// <param name="data">Decrypted data.</param>
        AesGcmParameter Decrypt(byte[] title, GXByteBuffer data);

        /// <summary>
        /// Reset encrypt settings.
        /// </summary>
        void Reset();

        /// <summary>
        /// Is ciphering used.
        /// </summary>
        /// <returns></returns>
        bool IsCiphered();

        /// <summary>
        /// Used security.
        /// </summary>
        Gurux.DLMS.Enums.Security Security
        {
            get;
            set;
        }

        /// <summary>
        /// System title.
        /// </summary>
        byte[] SystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Block cipher key.
        /// </summary>
        byte[] BlockCipherKey
        {
            get;
            set;
        }

        /// <summary>
        /// Authentication key.
        /// </summary>
        byte[] AuthenticationKey
        {
            get;
            set;
        }

        /// <summary>
        /// Frame counter. Invocation counter.
        /// </summary>
        UInt32 InvocationCounter
        {
            get;
        }
    }
}
