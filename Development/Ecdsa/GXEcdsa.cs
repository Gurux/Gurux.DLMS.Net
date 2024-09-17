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
using Gurux.DLMS.Ecdsa.Enums;
using Gurux.DLMS.Internal;
using System;
using System.Collections.Generic;
#if !WINDOWS_UWP
using System.Security.Cryptography;
#endif //!WINDOWS_UWP

namespace Gurux.DLMS.Ecdsa
{
    /// <summary>
    /// ECDSA asynchronous ciphering.
    /// </summary>
    public class GXEcdsa
    {
        /// <summary>
        /// Public key.
        /// </summary>
        private GXPublicKey PublicKey;
        /// <summary>
        /// Private key.
        /// </summary>
        private readonly GXPrivateKey PrivateKey;

        GXCurve curve;

        /// <summary>
        /// Get scheme size in bytes.
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        private static int SchemeSize(Ecc scheme)
        {
            return scheme == Ecc.P256 ? 32 : 48;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">Public key.</param>
        public GXEcdsa(GXPublicKey key) : this(key.Scheme)
        {
            PublicKey = key;
        }

        private GXEcdsa(Ecc scheme)
        {
            curve = new GXCurve(scheme);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">Private key.</param>
        public GXEcdsa(GXPrivateKey key) : this(key.Scheme)
        {
            PrivateKey = key;
        }

#if !WINDOWS_UWP
        /// <summary>
        /// Sign given data using public and private key.
        /// </summary>
        /// <param name="data">Data to sign.</param>
        /// <returns>Signature</returns>
        public byte[] Sign(byte[] data)
        {
#if NET462 || NET46
            if (PrivateKey == null)
            {
                throw new ArgumentException("Invalid private key.");
            }
            GXBigInteger msg;
            if (PrivateKey.Scheme == Ecc.P256)
            {
                using (SHA256 sha = SHA256.Create())
                {
                    msg = new GXBigInteger(sha.ComputeHash(data));
                }
            }
            else
            {
                using (SHA384 sha = SHA384.Create())
                {
                    msg = new GXBigInteger(sha.ComputeHash(data));
                }
            }
            // R = k * G, r = R[x]
            GXBigInteger k = GetRandomNumber(curve.N);

            GXBigInteger pk = new GXBigInteger(PrivateKey.RawValue);
            GXEccPoint R = new GXEccPoint(new GXBigInteger(), new GXBigInteger(), new GXBigInteger());
            GXShamirs.PointMulti(curve, R, curve.G, k);
            GXBigInteger r = new GXBigInteger(R.x);
            r.Mod(curve.N);
            //s = (k ^ -1 * (e + d * r)) mod n
            GXBigInteger s = new GXBigInteger(pk);
            s.Multiply(r);
            s.Add(msg);
            GXBigInteger kinv = new GXBigInteger(k);
            kinv.Inv(curve.N);
            s.Multiply(kinv);
            s.Mod(curve.N);
            GXByteBuffer signature = new GXByteBuffer();
            signature.Set(r.ToArray(false));
            signature.Set(s.ToArray(false));
            return signature.Array();
#else
            ECParameters p = new ECParameters();
            p.D = PrivateKey.RawValue;
            p.Q = new ECPoint() { X = PrivateKey.GetPublicKey().X, Y = PrivateKey.GetPublicKey().Y };
            p.Curve = PrivateKey.Scheme == Ecc.P256 ? ECCurve.NamedCurves.nistP256 : ECCurve.NamedCurves.nistP384;
            ECDsa ecdsa = ECDsa.Create(p);
            return ecdsa.SignData(data,
                PrivateKey.Scheme == Ecc.P256 ? HashAlgorithmName.SHA256 : HashAlgorithmName.SHA384);
#endif
        }

#endif //!WINDOWS_UWP

        /// <summary>
        /// Generate shared secret from public and private key.
        /// </summary>
        /// <param name="publicKey">Public key.</param>
        /// <returns>Generated secret.</returns>
        public byte[] GenerateSecret(GXPublicKey publicKey)
        {
            if (PrivateKey == null)
            {
                throw new ArgumentNullException("Invalid private key.");
            }
            if (PrivateKey.Scheme != publicKey.Scheme)
            {
                throw new ArgumentNullException("Private key scheme is different than public key.");
            }
            GXBigInteger pk = new GXBigInteger(PrivateKey.RawValue);

            GXByteBuffer bb = new GXByteBuffer();
            bb.Set(publicKey.RawValue);
            int size = SchemeSize(PrivateKey.Scheme);
            GXBigInteger x = new GXBigInteger(bb.SubArray(1, size));
            GXBigInteger y = new GXBigInteger(bb.SubArray(1 + size, size));
            GXEccPoint p = new GXEccPoint(x, y, null);
            GXCurve curve = new GXCurve(PrivateKey.Scheme);
            GXEccPoint ret = new GXEccPoint(null, null, null);
            GXShamirs.PointMulti(curve, ret, p, pk);
            return ret.x.ToArray();
        }

        /// <summary>
        /// Generate public and private key pair.
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<GXPublicKey, GXPrivateKey> GenerateKeyPair(Ecc scheme)
        {
#if NET462 || NET46 || WINDOWS_UWP
            byte[] raw = GetRandomNumber(new GXCurve(scheme).N).ToArray(false);
            GXPrivateKey pk = GXPrivateKey.FromRawBytes(raw);
            GXPublicKey pub = pk.GetPublicKey();
            return new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, pk);
#else
            ECCurve curve = scheme == Ecc.P256 ? ECCurve.NamedCurves.nistP256 : ECCurve.NamedCurves.nistP384;
            ECDsa ecdsa = ECDsa.Create(curve);
            ecdsa.GenerateKey(curve);
            var p = ecdsa.ExportParameters(true);
            GXPrivateKey pk = GXPrivateKey.FromRawBytes(p.D);
            GXByteBuffer tmp = new GXByteBuffer();
            tmp.SetUInt8(4);
            tmp.Set(p.Q.X);
            tmp.Set(p.Q.Y);
            GXPublicKey pub = GXPublicKey.FromRawBytes(tmp.Array());
            pk.publicKey = pub;
            return new KeyValuePair<GXPublicKey, GXPrivateKey>(pub, pk);
#endif
        }

#if !WINDOWS_UWP

        /// <summary>
        /// Verify that signature matches the data.
        /// </summary>
        /// <param name="signature">Generated signature.</param>
        /// <param name="data">Data to valuate.</param>
        /// <returns></returns>
        public bool Verify(byte[] signature, byte[] data)
        {
#if NET462 || NET46
            GXBigInteger msg;
            if (PublicKey == null)
            {
                if (PrivateKey == null)
                {
                    throw new ArgumentNullException("Invalid private key.");
                }
                PublicKey = PrivateKey.GetPublicKey();
            }
            if (PublicKey.Scheme == Ecc.P256)
            {
                using (SHA256 sha = SHA256.Create())
                {
                    msg = new GXBigInteger(sha.ComputeHash(data));
                }
            }
            else
            {
                using (SHA384 sha = SHA384.Create())
                {
                    msg = new GXBigInteger(sha.ComputeHash(data));
                }
            }
            GXByteBuffer bb = new GXByteBuffer(signature);
            int size = SchemeSize(PublicKey.Scheme);
            GXBigInteger sigR = new GXBigInteger(bb.SubArray(0, size));
            GXBigInteger sigS = new GXBigInteger(bb.SubArray(size, size));
            GXBigInteger w = sigS;
            w.Inv(curve.N);
            GXBigInteger u1 = msg;
            u1.Multiply(w);
            u1.Mod(curve.N);
            GXBigInteger u2 = new GXBigInteger(sigR);
            u2.Multiply(w);
            u2.Mod(curve.N);
            GXEccPoint tmp = new GXEccPoint(null, null, null);
            GXShamirs.Trick(curve, PublicKey, tmp, u1, u2);
            tmp.x.Mod(curve.N);
            return tmp.x.Compare(sigR) == 0;
#else
            if (PublicKey == null)
            {
                if (PrivateKey == null)
                {
                    throw new ArgumentNullException("Invalid private key.");
                }
                PublicKey = PrivateKey.GetPublicKey();
            }
            ECParameters p = new ECParameters();
            if (PrivateKey != null)
            {
                p.D = PrivateKey.RawValue;
            }
            p.Q = new ECPoint() { X = PublicKey.X, Y = PublicKey.Y };
            p.Curve = PublicKey.Scheme == Ecc.P256 ? ECCurve.NamedCurves.nistP256 : ECCurve.NamedCurves.nistP384;
            ECDsa ecdsa = ECDsa.Create(p);
            return ecdsa.VerifyData(data, signature,
                PublicKey.Scheme == Ecc.P256 ? HashAlgorithmName.SHA256 : HashAlgorithmName.SHA384);
#endif
        }
#endif //!WINDOWS_UWP

      
        /// <summary>
        /// Generate random number.
        /// </summary>
        /// <param name="N">N</param>
        /// <returns>Random number.</returns>
        /// <summary>
        private static GXBigInteger GetRandomNumber(GXBigInteger N)
        {
            byte[] bytes = new byte[4 * N.Count];
            Random random = new Random();
            random.NextBytes(bytes);
            byte[] tmp = new byte[4 * N.Count];
            Array.Copy(bytes, tmp, bytes.Length);
            return new GXBigInteger(tmp);
        }

        /// <summary>
        /// Check that this is correct public key.
        /// </summary>
        /// <remarks>
        /// This method can be used to verify that public and private key are on the curve.
        /// </remarks>
        public static void Validate(GXPublicKey publicKey)
        {
            if (publicKey == null)
            {
                throw new ArgumentNullException("Invalid public key.");
            }
            GXByteBuffer bb = new GXByteBuffer();
            bb.Set(publicKey.RawValue);
            int size = SchemeSize(publicKey.Scheme);
            GXBigInteger x = new GXBigInteger(bb.SubArray(1, size));
            GXBigInteger y = new GXBigInteger(bb.SubArray(1 + size, size));
            GXCurve curve = new GXCurve(publicKey.Scheme);
            y.Multiply(y);
            y.Mod(curve.P);

            GXBigInteger tmpX = new GXBigInteger(x);
            tmpX.Multiply(x);
            tmpX.Mod(curve.P);
            tmpX.Add(curve.A);
            tmpX.Multiply(x);
            tmpX.Add(curve.B);
            tmpX.Mod(curve.P);
            if (y.Compare(tmpX) != 0)
            {
                throw new ArgumentException("Public key validate failed. Public key is not valid ECDSA public key.");
            }
        }
    }
}
