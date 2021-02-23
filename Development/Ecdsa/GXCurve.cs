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
using System.Globalization;
using System.Numerics;

namespace Gurux.DLMS.Ecdsa
{
    /// <summary>
    /// ECC x and y points in the curve.
    /// </summary>
    class GXCurve
    {
        /// <summary>
        /// ECC curve a value.
        /// </summary>
        public GXBigInteger A
        {
            get;
            private set;
        }

        /// <summary>
        /// ECC curve p value.
        /// </summary>
        public GXBigInteger P
        {
            get;
            private set;
        }
        /// <summary>
        /// ECC curve b parameter.
        /// </summary>
        public GXBigInteger B
        {
            get;
            private set;
        }
        /// <summary>
        /// x and y-coordinate of base point G
        /// </summary>
        public GXEccPoint G
        {
            get;
            private set;
        }
        /// <summary>
        /// Order of point G in ECC curve.
        /// </summary>
        public GXBigInteger N
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="a">ECC curve a value.</param>
        /// <param name="b">ECC curve b parameter.</param>
        /// <param name="p">ECC curve p value.</param>
        /// <param name="g">x and y-coordinate of base point G</param>
        /// <param name="n">Order of point G in ECC curve.</param>
        public GXCurve(Ecc scheme)
        {
            if (scheme == Ecc.P256)
            {
                //Table A. 1 – ECC_P256_Domain_Parameters
                A = new GXBigInteger(new UInt32[] { 0xFFFFFFFF, 0x00000001, 0x00000000, 0x00000000,
                    0x00000000, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFC });
                G = new GXEccPoint(new GXBigInteger(new UInt32[] { 0x6B17D1F2, 0xE12C4247, 0xF8BCE6E5, 0x63A440F2,
                    0x77037D81, 0x2DEB33A0, 0xF4A13945, 0xD898C296 }),
                    new GXBigInteger(new UInt32[] { 0x4FE342E2, 0xFE1A7F9B, 0x8EE7EB4A, 0x7C0F9E16,
                        0x2BCE3357, 0x6B315ECE, 0xCBB64068, 0x37BF51F5 }), new GXBigInteger(1));
                N = new GXBigInteger(new UInt32[] { 0xFFFFFFFF, 0x00000000, 0xFFFFFFFF,
                    0xFFFFFFFF, 0xBCE6FAAD, 0xA7179E84, 0xF3B9CAC2, 0xFC632551 });
                P = new GXBigInteger(new UInt32[] { 0xFFFFFFFF, 0x00000001, 0x00000000,
                    0x00000000, 0x00000000, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF });
                B = new GXBigInteger(new UInt32[] {0x5AC635D8, 0xAA3A93E7, 0xB3EBBD55, 0x769886BC,
                    0x651D06B0 , 0xCC53B0F6 , 0x3BCE3C3E , 0x27D2604B});
            }
            else if (scheme == Ecc.P384)
            {
                //Table A. 2 – ECC_P384_Domain_Parameters
                A = new GXBigInteger(new UInt32[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE,
                    0xFFFFFFFF, 0x00000000, 0x00000000, 0xFFFFFFFC });
                G = new GXEccPoint(new GXBigInteger(new UInt32[] { 0xAA87CA22, 0xBE8B0537, 0x8EB1C71E, 0xF320AD74,
                    0x6E1D3B62, 0x8BA79B98, 0x59F741E0, 0x82542A38,
                    0x5502F25D, 0xBF55296C, 0x3A545E38, 0x72760AB7 }),
                    new GXBigInteger(new UInt32[] { 0x3617DE4A, 0x96262C6F, 0x5D9E98BF, 0x9292DC29,
                        0xF8F41DBD, 0x289A147C, 0xE9DA3113, 0xB5F0B8C0,
                        0x0A60B1CE, 0x1D7E819D, 0x7A431D7C, 0x90EA0E5F }), new GXBigInteger(1));
                N = new GXBigInteger(new UInt32[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    0xFFFFFFFF, 0xFFFFFFFF, 0xC7634D81, 0xF4372DDF,
                    0x581A0DB2, 0x48B0A77A, 0xECEC196A, 0xCCC52973 });
                P = new GXBigInteger(new UInt32[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE,
                    0xFFFFFFFF, 0x00000000, 0x00000000, 0xFFFFFFFF });
                B = new GXBigInteger(new UInt32[] {0xB3312FA7, 0xE23EE7E4, 0x988E056B, 0xE3F82D19, 0x181D9C6E,
                    0xFE814112, 0x0314088F, 0x5013875A, 0xC656398D, 0x8A2ED19D, 0x2A85C8ED, 0xD3EC2AEF});
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid scheme.");
            }
        }
    };
}
