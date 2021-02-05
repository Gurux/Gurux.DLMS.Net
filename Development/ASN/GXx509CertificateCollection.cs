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

using System;
using System.Collections.Generic;
using System.IO;

namespace Gurux.DLMS.ASN
{
    /// <summary>
    /// List of x509 certificates.
    /// </summary>
    public class GXx509CertificateCollection : List<GXx509Certificate>
    {
        /// <summary>
        /// Find public key certificate by public name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public GXx509Certificate Find(GXx509Certificate key)
        {
            foreach (GXx509Certificate it in this)
            {
                if (it.PublicKey.Equals(key))
                {
                    return it;
                }
            }
            return null;
        }

        /// <summary>
        /// Find public key certificate by system title.
        /// </summary>
        /// <param name="systemTitle">System title.</param>
        /// <returns>Found certificate or null if certificate is not found.</returns>
        public GXx509Certificate FindBySystemTitle(byte[] systemTitle)
        {
            return FindByCommonName(GXAsn1Converter.SystemTitleToSubject(systemTitle));
        }

        /// <summary>
        /// Find public key certificate by common name (CN).
        /// </summary>
        /// <param name="commonName">Common name.</param>
        /// <returns>Found certificate or null if certificate is not found.</returns>
        public GXx509Certificate FindByCommonName(string commonName)
        {
            foreach (GXx509Certificate it in this)
            {
                if (it.Subject.Contains(commonName))
                {
                    return it;
                }
            }
            return null;
        }

        /// <summary>
        /// Import certificates from the given folder.
        /// </summary>
        /// <param name="path"></param>
        public void Import(string path)
        {
            foreach (string it in Directory.GetFiles(path, "*.pem"))
            {
                try
                {
                    GXx509Certificate cert = GXx509Certificate.Load(it);
                    Add(cert);
                }
                catch (Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to load x509 certificate." + it);
                }
            }
        }
    }
}