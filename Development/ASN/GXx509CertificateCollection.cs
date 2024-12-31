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
using Gurux.DLMS.ASN.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Gurux.DLMS.ASN
{
    /// <summary>
    /// List of x509 certificates.
    /// </summary>
    public class GXx509CertificateCollection : List<GXx509Certificate>
    {
        /// <summary>
        /// Find public key certificate by public key.
        /// </summary>
        /// <param name="key">X509 certificate to search for.</param>
        /// <returns>Certificate found or null, if the certificate is not found.</returns>
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
        /// Find public key certificate by serial number.
        /// </summary>
        /// <param name="serialNumber">X509 certificate serial number to search for.</param>
        /// <param name="issuer">X509 certificate issuer.</param>
        /// <returns>Found certificate or null if certificate is not found.</returns>
        public GXx509Certificate FindBySerial(BigInteger serialNumber, string issuer)
        {
            foreach (GXx509Certificate it in this)
            {
                if (it.SerialNumber == serialNumber && it.Issuer == issuer)
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
        /// <param name="usage">Key usage.</param>
        /// <returns>Found certificate or null if certificate is not found.</returns>
        public GXx509Certificate FindBySystemTitle(byte[] systemTitle, KeyUsage usage)
        {
            string commonName;
            if (systemTitle == null || systemTitle.Length == 0)
            {
                commonName = null;
            }
            else
            {
                commonName = GXAsn1Converter.SystemTitleToSubject(systemTitle);
            }
            return FindByCommonName(commonName, usage);
        }

        /// <summary>
        /// Find certificates by key usage.
        /// </summary>
        /// <param name="usage">Key usage.</param>
        /// <returns>>Found certificates.</returns>
        public List<GXx509Certificate> GetCertificates(KeyUsage usage)
        {
            List<GXx509Certificate> certificates = new List<GXx509Certificate>();
            foreach (GXx509Certificate it in this)
            {
                if (it.KeyUsage == usage)
                {
                    certificates.Add(it);
                }
            }
            return certificates;
        }

        /// <summary>
        /// Find public key certificate by common name (CN).
        /// </summary>
        /// <param name="commonName">Common name.</param>
        /// <param name="usage">Key usage.</param>
        /// <returns>Found certificate or null if certificate is not found.</returns>
        public GXx509Certificate FindByCommonName(string commonName, KeyUsage usage)
        {
            foreach (GXx509Certificate it in this)
            {
                if ((usage == KeyUsage.None || (it.KeyUsage & usage) != 0) && it.Subject.Contains(commonName))
                {
                    return it;
                }
            }
            return null;
        }

        /// <summary>
        /// Import certificates from the given folder.
        /// </summary>
        /// <param name="folder">The folder from which the certificates are imported.</param>
        public void Import(string folder)
        {
            foreach (string it in Directory.GetFiles(folder))
            {
                string ext = Path.GetExtension(it);
                if (string.Compare(ext, ".pem", true) == 0 || string.Compare(ext, ".cer", true) == 0)
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
}