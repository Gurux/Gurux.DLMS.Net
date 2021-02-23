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
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

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

        /// <summary>
        /// Sign given data using public and private key.
        /// </summary>
        /// <param name="data">Data to sign.</param>
        /// <returns>Signature</returns>
        public byte[] Sign(byte[] data)
        {
            if (PrivateKey == null)
            {
                throw new ArgumentException("Invalid private key.");
            }
            GXBigInteger msg;
            if (PrivateKey.Scheme == Ecc.P256)
            {
                using (SHA256 sha = new SHA256CryptoServiceProvider())
                {
                    msg = new GXBigInteger(sha.ComputeHash(data));
                }
            }
            else
            {
                using (SHA384 sha = new SHA384CryptoServiceProvider())
                {
                    msg = new GXBigInteger(sha.ComputeHash(data));
                }
            }
            GXBigInteger pk = new GXBigInteger(PrivateKey.RawValue);
            GXEccPoint p;
            GXBigInteger n;
            GXBigInteger r;
            GXBigInteger s;
            do
            {
                n = GetRandomNumber(PrivateKey.Scheme);
                p = new GXEccPoint(curve.G.x, curve.G.y, new GXBigInteger(1));
                Multiply(p, n, curve.N, curve.A, curve.P);
                r = p.x;
                r.Mod(curve.N);
                n.Inv(curve.N);
                //s
                s = new GXBigInteger(r);
                s.Multiply(pk);
                s.Add(msg);
                s.Multiply(n);
                s.Mod(curve.N);
            } while (r.IsZero || s.IsZero);
            GXByteBuffer signature = new GXByteBuffer();
            signature.Set(r.ToArray());
            signature.Set(s.ToArray());
            return signature.Array();
        }

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
            GXByteBuffer bb = new GXByteBuffer();
            bb.Set(publicKey.RawValue);
            int size = SchemeSize(PrivateKey.Scheme);
            GXBigInteger x = new GXBigInteger(bb.SubArray(1, size));
            GXBigInteger y = new GXBigInteger(bb.SubArray(1 + size, size));
            GXBigInteger pk = new GXBigInteger(PrivateKey.RawValue);
            GXCurve curve = new GXCurve(PrivateKey.Scheme);
            GXEccPoint p = new GXEccPoint(x, y, new GXBigInteger(1));
            p = JacobianMultiply(p, pk, curve.N, curve.A, curve.P);
            FromJacobian(p, curve.P);
            return p.x.ToArray();
        }

        /// <summary>
        /// Generate public and private key pair.
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<GXPrivateKey, GXPublicKey> GenerateKeyPair(Ecc scheme)
        {
            byte[] raw = GetRandomNumber(scheme).ToArray();
            GXPrivateKey pk = GXPrivateKey.FromRawBytes(raw);
            GXPublicKey pub = pk.GetPublicKey();
            return new KeyValuePair<GXPrivateKey, GXPublicKey>(pk, pub);
        }

        /// <summary>
        /// Verify that signature matches the data.
        /// </summary>
        /// <param name="signature">Generated signature.</param>
        /// <param name="data">Data to valuate.</param>
        /// <returns></returns>
        public bool Verify(byte[] signature, byte[] data)
        {
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
                using (SHA256 sha = new SHA256CryptoServiceProvider())
                {
                    msg = new GXBigInteger(sha.ComputeHash(data));
                }
            }
            else
            {
                using (SHA384 sha = new SHA384CryptoServiceProvider())
                {
                    msg = new GXBigInteger(sha.ComputeHash(data));
                }
            }
            GXByteBuffer pk = new GXByteBuffer(PublicKey.RawValue);
            GXByteBuffer bb = new GXByteBuffer(signature);
            int size = SchemeSize(PublicKey.Scheme);
            GXBigInteger sigR = new GXBigInteger(bb.SubArray(0, size));
            GXBigInteger sigS = new GXBigInteger(bb.SubArray(size, size));
            GXBigInteger inv = sigS;
            inv.Inv(curve.N);
            // Calculate u1 and u2.
            GXEccPoint u1 = new GXEccPoint(curve.G.x, curve.G.y, new GXBigInteger(1));
            GXEccPoint u2 = new GXEccPoint(new GXBigInteger(pk.SubArray(1, size)),
                new GXBigInteger(pk.SubArray(1 + size, size)), new GXBigInteger(1));
            GXBigInteger n = msg;
            n.Multiply(inv);
            n.Mod(curve.N);
            Multiply(u1, n, curve.N, curve.A, curve.P);
            n = new GXBigInteger(sigR);
            n.Multiply(inv);
            n.Mod(curve.N);
            Multiply(u2, n, curve.N, curve.A, curve.P);
            u1.z = new GXBigInteger(1);
            u2.z = new GXBigInteger(1);
            JacobianAdd(u1, u2, curve.A, curve.P);
            FromJacobian(u1, curve.P);
            return sigR.Compare(u1.x) == 0;
        }

        private static void Multiply(GXEccPoint p, GXBigInteger n, GXBigInteger N, GXBigInteger A, GXBigInteger P)
        {
            GXEccPoint p2 = JacobianMultiply(p, n, N, A, P);
            p.x = p2.x;
            p.y = p2.y;
            p.z = p2.z;
            FromJacobian(p, P);
        }

        /// <summary>
        /// Y^2 = X^3 + A*X + B (mod p)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="A"></param>
        /// <param name="P">Prime number</param>
        private static void JacobianAdd(GXEccPoint p, GXEccPoint q, GXBigInteger A, GXBigInteger P)
        {
            if (!(p.y.IsZero || q.y.IsZero))
            {
                GXBigInteger U1 = new GXBigInteger(p.x);
                U1.Multiply(q.z);
                U1.Multiply(q.z);
                U1.Mod(P);
                GXBigInteger U2 = new GXBigInteger(p.z);
                U2.Multiply(p.z);
                U2.Multiply(q.x);
                U2.Mod(P);
                GXBigInteger S1 = new GXBigInteger(p.y);
                S1.Multiply(q.z);
                S1.Multiply(q.z);
                S1.Multiply(q.z);
                S1.Mod(P);
                GXBigInteger S2 = new GXBigInteger(q.y);
                S2.Multiply(p.z);
                S2.Multiply(p.z);
                S2.Multiply(p.z);
                S2.Mod(P);
                if (U1.Compare(U2) == 0)
                {
                    if (S1.Compare(S2) != 0)
                    {
                        p.x = p.y = new GXBigInteger(0);
                        p.z = new GXBigInteger(1);
                    }
                    else
                    {
                        p.x = A;
                        p.y = P;
                    }
                }
                //H
                GXBigInteger H = U2;
                H.Sub(U1);
                //R
                GXBigInteger R = S2;
                R.Sub(S1);
                GXBigInteger H2 = new GXBigInteger(H);
                H2.Multiply(H);
                H2.Mod(P);
                GXBigInteger H3 = new GXBigInteger(H);
                H3.Multiply(H2);
                H3.Mod(P);
                GXBigInteger U1H2 = new GXBigInteger(U1);
                U1H2.Multiply(H2);
                U1H2.Mod(P);
                GXBigInteger tmp = new GXBigInteger(2);
                tmp.Multiply(U1H2);
                //nx
                GXBigInteger nx = new GXBigInteger(R);
                nx.Multiply(R);
                nx.Sub(H3);
                nx.Sub(tmp);
                nx.Mod(P);
                //ny
                GXBigInteger ny = R;
                tmp = new GXBigInteger(U1H2);
                tmp.Sub(nx);
                ny.Multiply(tmp);
                tmp = new GXBigInteger(S1);
                tmp.Multiply(H3);
                ny.Sub(tmp);
                ny.Mod(P);
                //nz
                GXBigInteger nz = H;
                nz.Multiply(p.z);
                nz.Multiply(q.z);
                nz.Mod(P);
                p.x = nx;
                p.y = ny;
                p.z = nz;
            }
        }

        /// <summary>
        /// Get ECC point from Jacobian coordinates.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="P"></param>
        internal static void FromJacobian(GXEccPoint p, GXBigInteger P)
        {
            p.z.Inv(P);
            p.x.Multiply(p.z);
            p.x.Multiply(p.z);
            p.x.Mod(P);

            p.y.Multiply(p.z);
            p.y.Multiply(p.z);
            p.y.Multiply(p.z);
            p.y.Mod(P);
            p.z.Clear();
        }

        /// <summary>
        /// Multily elliptic curve point and scalar.
        /// </summary>
        /// <remarks>
        /// Y^2 = X^3 + A*X + B (mod p)
        /// </remarks>
        /// <param name="eccSize"></param>
        /// <param name="p">Point to multiply</param>
        /// <param name="n">Scalar to multiply</param>
        /// <param name="N">Elliptic curve order.</param>
        /// <param name="A"></param>
        /// <param name="P">Prime number</param>
        internal static GXEccPoint JacobianMultiply(GXEccPoint p, GXBigInteger n, GXBigInteger N, GXBigInteger A, GXBigInteger P)
        {
            GXBigInteger tmp;
            if (p.y.IsZero || n.IsZero)
            {
                return new GXEccPoint(0, 0, 1);
            }
            if (n.IsOne)
            {
                return p;
            }
            if (n.Compare(0) == -1 || n.Compare(N) != -1)
            {
                tmp = new GXBigInteger(n);
                tmp.Mod(N);
                return JacobianMultiply(p, tmp, N, A, P);
            }
            if (n.IsEven)
            {
                tmp = new GXBigInteger(n);
                tmp.Rshift(1);
                return JacobianDouble(JacobianMultiply(p, tmp, N, A, P), A, P);
            }
            tmp = new GXBigInteger(n);
            tmp.Rshift(1);
            GXEccPoint p2 = JacobianDouble(JacobianMultiply(p, tmp, N, A, P), A, P);
            JacobianAdd(p2, p, A, P);
            return p2;
        }

        /// <summary>
        /// Convert ECC point to Jacobian.
        /// </summary>
        /// <param name="p">ECC point.</param>
        /// <param name="A"></param>
        /// <param name="P">Prime number.</param>
        /// <returns></returns>
        private static GXEccPoint JacobianDouble(GXEccPoint p, GXBigInteger A, GXBigInteger P)
        {
            GXBigInteger ysq = new GXBigInteger(p.y);
            ysq.Multiply(p.y);
            ysq.Mod(P);
            GXBigInteger S = new GXBigInteger(p.x);
            S.Multiply(new GXBigInteger(4));
            S.Multiply(ysq);
            S.Mod(P);
            GXBigInteger M = new GXBigInteger(p.x);
            M.Multiply(p.x);
            M.Multiply(new GXBigInteger(3));
            GXBigInteger tmp = new GXBigInteger(p.z);
            tmp.Multiply(p.z);
            tmp.Multiply(p.z);
            tmp.Multiply(p.z);
            tmp.Multiply(A);
            M.Add(tmp);
            M.Mod(P);
            //nx
            GXBigInteger nx = new GXBigInteger(M);
            nx.Multiply(M);
            tmp = new GXBigInteger(S);
            tmp.Multiply(new GXBigInteger(2));
            nx.Sub(tmp);
            nx.Mod(P);
            //ny
            GXBigInteger ny = new GXBigInteger(S);
            ny.Sub(nx);
            ny.Multiply(M);
            tmp = new GXBigInteger(ysq);
            tmp.Multiply(ysq);
            tmp.Multiply(new GXBigInteger(8));
            ny.Sub(tmp);
            ny.Mod(P);
            //nz
            GXBigInteger nz = new GXBigInteger(p.y);
            nz.Multiply(p.z);
            nz.Multiply(new GXBigInteger(2));
            nz.Mod(P);
            return new GXEccPoint(nx, ny, nz);
        }

        /// <summary>
        /// Generate random number.
        /// </summary>
        /// <param name="schema"></param>
        /// <returns>Random number.</returns>
        /// <summary>
        private static GXBigInteger GetRandomNumber(Ecc schema)
        {
            byte[] bytes = new byte[SchemeSize(schema)];
            Random random = new Random();
            random.NextBytes(bytes);
            return new GXBigInteger(bytes);
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
