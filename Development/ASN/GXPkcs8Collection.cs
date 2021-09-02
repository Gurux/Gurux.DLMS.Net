//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Filename:    $HeadURL$
//
// Version:     $Revision$,
//      $Date$
//      $Author$
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

using Gurux.DLMS.Ecdsa;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gurux.DLMS.ASN
{
    /// <summary>
    /// List of PKCS #8 certificates.
    /// </summary>
    public class GXPkcs8Collection : List<GXPkcs8>
    {
        /// <summary>
        /// Find private key certificate by public name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public GXPkcs8 Find(GXPublicKey key)
        {
            foreach(GXPkcs8 it in this)
            {
                if (it.PublicKey.Equals(key))
                {
                    return it;
                }
            }
            return null;
        }

        /// <summary>
        /// Import private key certificates from the given folder.
        /// </summary>
        /// <param name="path"></param>
        public void Import(string path)
        {
            foreach (string it in Directory.GetFiles(path))
            {
                string ext = Path.GetExtension(it);
                if (string.Compare(ext, ".pem", true) == 0 || string.Compare(ext, ".cer", true) == 0)
                {
                    try
                    {
                        GXPkcs8 cert = GXPkcs8.Load(it);
                        Add(cert);
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to load PKCS #8 certificate." + it);
                    }
                }
            }
        }
    }
}