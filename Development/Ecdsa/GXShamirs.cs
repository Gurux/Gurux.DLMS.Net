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

namespace Gurux.DLMS.Ecdsa
{
    /// <summary>
    /// This class implements GXShamir's trick.
    /// </summary>
    internal static class GXShamirs
    {
        /// <summary>
        /// Count Shamir's trick.
        /// </summary>
        /// <param name="curve">Used curve.</param>
        /// <param name="pub">Public key.</param>
        /// <param name="ret">Result.</param>
        /// <param name="u1"></param>
        /// <param name="u2"></param>
        public static void Trick(GXCurve curve, 
            GXPublicKey pub, 
            GXEccPoint ret, 
            GXBigInteger u1, 
            GXBigInteger u2)
        {
            GXEccPoint sum = new GXEccPoint(null, null, null);
            GXEccPoint op2 = new GXEccPoint(new GXBigInteger(pub.X), new GXBigInteger(pub.Y), null);
            PointAdd(curve, sum, curve.G, op2);
            UInt16 bits1 = u1.UsedBits;
            UInt16 bits2 = u2.UsedBits;
            UInt16 pos = bits1 > bits2 ? bits1 : bits2;
            --pos;
            if (u1.IsBitSet(pos) && u2.IsBitSet(pos))
            {
                ret.x = sum.x;
                ret.y = sum.y;
            }
            else if (u1.IsBitSet(pos))
            {
                ret.x = curve.G.x;
                ret.y = curve.G.y;
            }
            else if (u2.IsBitSet(pos))
            {
                ret.x = new GXBigInteger(pub.X);
                ret.y = new GXBigInteger(pub.Y);
            }
            GXEccPoint tmp = new GXEccPoint(null, null, null);
            --pos;
            while (true)
            {
                tmp.x = ret.x;
                tmp.y = ret.y;
                PointDouble(curve, ret, tmp);
                tmp.x = ret.x;
                tmp.y = ret.y;
                if (u1.IsBitSet(pos) && u2.IsBitSet(pos))
                {
                    PointAdd(curve, ret, tmp, sum);
                }
                else if (u1.IsBitSet(pos))
                {
                    PointAdd(curve, ret, tmp, curve.G);
                }
                else if (u2.IsBitSet(pos))
                {
                    PointAdd(curve, ret, tmp, op2);
                }
                if (pos == 0)
                {
                    break;
                }
                --pos;
            }
        }

        /// <summary>
        /// Add points.
        /// </summary>
        /// <param name="curve">Used curve.</param>
        /// <param name="ret">Result.</param>
        /// <param name="p1">Point 1.</param>
        /// <param name="p2">Point 2.</param>
        static void PointAdd(GXCurve curve, GXEccPoint ret, GXEccPoint p1, GXEccPoint p2)
        {
            GXBigInteger negy = new GXBigInteger(curve.P);
            negy.Sub(new GXBigInteger(p2.y));
            // Calculate lambda.
            GXBigInteger ydiff = new GXBigInteger(p2.y);
            ydiff.Sub(p1.y);
            GXBigInteger xdiff = new GXBigInteger(p2.x);
            xdiff.Sub(p1.x);
            xdiff.Inv(curve.P);
            GXBigInteger lambda = new GXBigInteger(ydiff);
            lambda.Multiply(xdiff);
            lambda.Mod(curve.P);
            // calculate resulting x coord.
            ret.x = new GXBigInteger(lambda);
            ret.x.Multiply(lambda);
            ret.x.Sub(p1.x);
            ret.x.Sub(p2.x);
            ret.x.Mod(curve.P);
            //calculate resulting y coord
            ret.y = new GXBigInteger(p1.x);
            ret.y.Sub(ret.x);
            ret.y.Multiply(lambda);
            ret.y.Sub(p1.y);
            ret.y.Mod(curve.P);
        }

        /// <summary>
        /// Double point.
        /// </summary>
        /// <param name="curve">Used curve.</param>
        /// <param name="ret">Result value.</param>
        /// <param name="p1">Point to double</param>
        static internal void PointDouble(GXCurve curve, 
            GXEccPoint ret, 
            GXEccPoint p1)
        {
            GXBigInteger numer = new GXBigInteger(p1.x);
            numer.Multiply(p1.x);
            numer.Multiply(3);
            numer.Add(curve.A);
            GXBigInteger denom = new GXBigInteger(p1.y);
            denom.Multiply(2);
            denom.Inv(curve.P);
            GXBigInteger lambda = new GXBigInteger(numer);
            lambda.Multiply(denom);
            lambda.Mod(curve.P);
            // calculate resulting x coord
            ret.x = new GXBigInteger(lambda);
            ret.x.Multiply(lambda);
            ret.x.Sub(p1.x);
            ret.x.Sub(p1.x);
            ret.x.Mod(curve.P);
            //calculate resulting y coord
            ret.y = new GXBigInteger(p1.x);
            ret.y.Sub(ret.x);
            ret.y.Multiply(lambda);
            ret.y.Sub(p1.y);
            ret.y.Mod(curve.P);
        }

        /// <summary>
        /// Multiply point with big integer value.
        /// </summary>
        /// <param name="curve">Used curve.</param>
        /// <param name="ret">Return value.</param>
        /// <param name="point">Point.</param>
        /// <param name="scalar">Scaler.</param>
        static internal void PointMulti(GXCurve curve, 
            GXEccPoint ret, 
            GXEccPoint point, 
            GXBigInteger scalar)
        {
            GXEccPoint R0 = new GXEccPoint(new GXBigInteger(point.x), 
                new GXBigInteger(point.y), null);
            GXEccPoint R1 = new GXEccPoint(null, null, null);
            GXEccPoint tmp = new GXEccPoint(null, null, null);
            PointDouble(curve, R1, point);
            UInt16 dbits = scalar.UsedBits;
            dbits -= 2;
            while (true)
            {
                if (scalar.IsBitSet(dbits))
                {
                    tmp.x = R0.x;
                    tmp.y = R0.y;
                    PointAdd(curve, R0, R1, tmp);
                    tmp.x = R1.x;
                    tmp.y = R1.y;
                    PointDouble(curve, R1, tmp);
                }
                else
                {
                    tmp.x = R1.x;
                    tmp.y = R1.y;
                    PointAdd(curve, R1, R0, tmp);
                    tmp.x = R0.x;
                    tmp.y = R0.y;
                    PointDouble(curve, R0, tmp);
                }
                if (dbits == 0)
                {
                    break;
                }
                --dbits;
            }
            ret.x = R0.x;
            ret.y = R0.y;
        }
    }
}
