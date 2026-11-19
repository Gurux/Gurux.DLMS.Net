// --------------------------------------------------------------------------
// Gurux Ltd
//
// Copyright (c) Gurux Ltd.
//
// This file is part of Gurux Device Framework.
//
// Commit: $Format:%H$
// Date:   $Format:%cI$
//
// This software is available under either of the following licenses:
//
// 1. GNU General Public License, version 2 only (GPL-2.0-only).
//    See the LICENSE file included with this distribution.
//
// 2. A commercial license granted by Gurux Ltd under a separate written
//    agreement. Use under this option requires a valid commercial license
//    and is governed by the terms of that agreement.
//
// If you have not obtained a commercial license from Gurux Ltd,
// use of this software is governed by GPL-2.0-only.
//
// Commercial licensing: https://www.gurux.fi/
//
// SPDX-License-Identifier: GPL-2.0-only OR LicenseRef-Gurux-Commercial
// --------------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gurux.DLMS.ASN;
using Gurux.DLMS.DeviceAccess;

namespace Gurux.DLMS.Client.Example.Net
{
    /// <summary>Provides application-owned PEM selection and loading for client key requests.</summary>
    partial class Program
    {
        /// <summary>Loads the requested key from a PEM path selected by the application's callback.</summary>
        /// <param name="reader">Reader exposing the application's per-request path selector.</param>
        /// <param name="args">Request to populate with a private key or certificate public key.</param>
        /// <exception cref="ArgumentNullException">The reader or request is null.</exception>
        /// <exception cref="InvalidOperationException">No path selector is configured.</exception>
        /// <exception cref="OperationCanceledException">The selector returns a blank path.</exception>
        /// <exception cref="IOException">The selected PEM file cannot be read.</exception>
        internal static void Client_OnKeys(GXDLMSReader reader, GXCryptoKeyParameter args)
        {
            ArgumentNullException.ThrowIfNull(reader);
            ArgumentNullException.ThrowIfNull(args);
            var requestPath = reader.OnKeyPathRequested ?? throw new InvalidOperationException(Properties.Resources.KeyPathCallbackRequired);
            string path = requestPath(args);
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new OperationCanceledException(Properties.Resources.KeyPathSelectionCanceled);
            }

            if (args.Encrypt)
            {
                args.PrivateKey = GXPkcs8.Load(path).PrivateKey;
            }
            else
            {
                args.PublicKey = GXx509Certificate.Load(path).PublicKey;
            }
        }

        /// <summary>Prompts for a PEM path for each request without caching a previous selection.</summary>
        /// <param name="request">Context identifying the requested key and system title.</param>
        /// <returns>The entered path, or null when standard input is closed.</returns>
        /// <param name="cancellationToken">Token that cancels pending work and communication waits.</param>
        /// <exception cref="OperationCanceledException">Cancellation interrupts console input.</exception>
        private static string SelectKeyPath(GXCryptoKeyParameter request, CancellationToken cancellationToken = default)
        {
            string systemTitle = request.SystemTitle == null ? Properties.Resources.MissingSystemTitle : Convert.ToHexString(request.SystemTitle);
            string message = request.Encrypt ? Properties.Resources.SelectPrivateKey : Properties.Resources.SelectCertificate;
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, message, request.SecuritySuite, request.CertificateType, request.KeyType, systemTitle));
            Console.Write(Properties.Resources.PemFilePathPrompt);
            cancellationToken.ThrowIfCancellationRequested();
            return Task.Run(Console.ReadLine, cancellationToken).WaitAsync(cancellationToken).GetAwaiter().GetResult();
        }
    }
}
