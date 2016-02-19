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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Internal
{
    class GXAes128
    {
        /// <summary>
        /// S box
        /// </summary>
        static readonly byte[] SBox =
		{
            0x63, 0x7C, 0x77, 0x7B, 0xF2, 0x6B, 0x6F, 0xC5, 0x30, 0x01, 0x67, 0x2B, 
            0xFE, 0xD7, 0xAB, 0x76, 0xCA, 0x82, 0xC9, 0x7D, 0xFA, 0x59, 0x47, 0xF0, 
            0xAD, 0xD4, 0xA2, 0xAF, 0x9C, 0xA4, 0x72, 0xC0, 0xB7, 0xFD, 0x93, 0x26, 
            0x36, 0x3F, 0xF7, 0xCC, 0x34, 0xA5, 0xE5, 0xF1, 0x71, 0xD8, 0x31, 0x15, 
            0x04, 0xC7, 0x23, 0xC3, 0x18, 0x96, 0x05, 0x9A, 0x07, 0x12, 0x80, 0xE2, 
            0xEB, 0x27, 0xB2, 0x75, 0x09, 0x83, 0x2C, 0x1A, 0x1B, 0x6E, 0x5A, 0xA0, 
            0x52, 0x3B, 0xD6, 0xB3, 0x29, 0xE3, 0x2F, 0x84, 0x53, 0xD1, 0x00, 0xED, 
            0x20, 0xFC, 0xB1, 0x5B, 0x6A, 0xCB, 0xBE, 0x39, 0x4A, 0x4C, 0x58, 0xCF, 
            0xD0, 0xEF, 0xAA, 0xFB, 0x43, 0x4D, 0x33, 0x85, 0x45, 0xF9, 0x02, 0x7F, 
            0x50, 0x3C, 0x9F, 0xA8, 0x51, 0xA3, 0x40, 0x8F, 0x92, 0x9D, 0x38, 0xF5, 
            0xBC, 0xB6, 0xDA, 0x21, 0x10, 0xFF, 0xF3, 0xD2, 0xCD, 0x0C, 0x13, 0xEC, 
            0x5F, 0x97, 0x44, 0x17, 0xC4, 0xA7, 0x7E, 0x3D, 0x64, 0x5D, 0x19, 0x73, 
            0x60, 0x81, 0x4F, 0xDC, 0x22, 0x2A, 0x90, 0x88, 0x46, 0xEE, 0xB8, 0x14, 
            0xDE, 0x5E, 0x0B, 0xDB, 0xE0, 0x32, 0x3A, 0x0A, 0x49, 0x06, 0x24, 0x5C, 
            0xC2, 0xD3, 0xAC, 0x62, 0x91, 0x95, 0xE4, 0x79, 0xE7, 0xC8, 0x37, 0x6D, 
            0x8D, 0xD5, 0x4E, 0xA9, 0x6C, 0x56, 0xF4, 0xEA, 0x65, 0x7A, 0xAE, 0x08, 
            0xBA, 0x78, 0x25, 0x2E, 0x1C, 0xA6, 0xB4, 0xC6, 0xE8, 0xDD, 0x74, 0x1F, 
            0x4B, 0xBD, 0x8B, 0x8A, 0x70, 0x3E, 0xB5, 0x66, 0x48, 0x03, 0xF6, 0x0E, 
            0x61, 0x35, 0x57, 0xB9, 0x86, 0xC1, 0x1D, 0x9E, 0xE1, 0xF8, 0x98, 0x11, 
            0x69, 0xD9, 0x8E, 0x94, 0x9B, 0x1E, 0x87, 0xE9, 0xCE, 0x55, 0x28, 0xDF, 
            0x8C, 0xA1, 0x89, 0x0D, 0xBF, 0xE6, 0x42, 0x68, 0x41, 0x99, 0x2D, 0x0F, 
            0xB0, 0x54, 0xBB, 0x16
        };

        /// <summary>
        /// The inverse S-box.
        /// </summary>        
        private static readonly byte[] SBoxInverse =
		{
            0x52, 0x09, 0x6A, 0xD5, 0x30, 0x36, 0xA5, 0x38, 0xBF, 0x40, 0xA3, 0x9E, 
            0x81, 0xF3, 0xD7, 0xFB, 0x7C, 0xE3, 0x39, 0x82, 0x9B, 0x2F, 0xFF, 0x87, 
            0x34, 0x8E, 0x43, 0x44, 0xC4, 0xDE, 0xE9, 0xCB, 0x54, 0x7B, 0x94, 0x32, 
            0xA6, 0xC2, 0x23, 0x3D, 0xEE, 0x4C, 0x95, 0x0B, 0x42, 0xFA, 0xC3, 0x4E, 
            0x08, 0x2E, 0xA1, 0x66, 0x28, 0xD9, 0x24, 0xB2, 0x76, 0x5B, 0xA2, 0x49, 
            0x6D, 0x8B, 0xD1, 0x25, 0x72, 0xF8, 0xF6, 0x64, 0x86, 0x68, 0x98, 0x16, 
            0xD4, 0xA4, 0x5C, 0xCC, 0x5D, 0x65, 0xB6, 0x92, 0x6C, 0x70, 0x48, 0x50, 
            0xFD, 0xED, 0xB9, 0xDA, 0x5E, 0x15, 0x46, 0x57, 0xA7, 0x8D, 0x9D, 0x84, 
            0x90, 0xD8, 0xAB, 0x00, 0x8C, 0xBC, 0xD3, 0x0A, 0xF7, 0xE4, 0x58, 0x05, 
            0xB8, 0xB3, 0x45, 0x06, 0xD0, 0x2C, 0x1E, 0x8F, 0xCA, 0x3F, 0x0F, 0x02, 
            0xC1, 0xAF, 0xBD, 0x03, 0x01, 0x13, 0x8A, 0x6B, 0x3A, 0x91, 0x11, 0x41, 
            0x4F, 0x67, 0xDC, 0xEA, 0x97, 0xF2, 0xCF, 0xCE, 0xF0, 0xB4, 0xE6, 0x73, 
            0x96, 0xAC, 0x74, 0x22, 0xE7, 0xAD, 0x35, 0x85, 0xE2, 0xF9, 0x37, 0xE8, 
            0x1C, 0x75, 0xDF, 0x6E, 0x47, 0xF1, 0x1A, 0x71, 0x1D, 0x29, 0xC5, 0x89, 
            0x6F, 0xB7, 0x62, 0x0E, 0xAA, 0x18, 0xBE, 0x1B, 0xFC, 0x56, 0x3E, 0x4B, 
            0xC6, 0xD2, 0x79, 0x20, 0x9A, 0xDB, 0xC0, 0xFE, 0x78, 0xCD, 0x5A, 0xF4, 
            0x1F, 0xDD, 0xA8, 0x33, 0x88, 0x07, 0xC7, 0x31, 0xB1, 0x12, 0x10, 0x59, 
            0x27, 0x80, 0xEC, 0x5F, 0x60, 0x51, 0x7F, 0xA9, 0x19, 0xB5, 0x4A, 0x0D, 
            0x2D, 0xE5, 0x7A, 0x9F, 0x93, 0xC9, 0x9C, 0xEF, 0xA0, 0xE0, 0x3B, 0x4D, 
            0xAE, 0x2A, 0xF5, 0xB0, 0xC8, 0xEB, 0xBB, 0x3C, 0x83, 0x53, 0x99, 0x61, 
            0x17, 0x2B, 0x04, 0x7E, 0xBA, 0x77, 0xD6, 0x26, 0xE1, 0x69, 0x14, 0x63, 
            0x55, 0x21, 0x0C, 0x7D			
		};

        // round constant
        private static readonly byte[] Rcon = { 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36 };

        static void expandKey(byte[] expandedKey, byte[] key)
        {
            int ii, buf1;
            for (ii = 0; ii < 16; ii++)
            {
                expandedKey[ii] = key[ii];
            }
            for (ii = 1; ii < 11; ii++)
            {
                buf1 = expandedKey[ii * 16 - 4];
                expandedKey[ii * 16 + 0] = (byte)(SBox[expandedKey[ii * 16 - 3]] ^ expandedKey[(ii - 1) * 16 + 0] ^ Rcon[ii]);
                expandedKey[ii * 16 + 1] = (byte)(SBox[expandedKey[ii * 16 - 2]] ^ expandedKey[(ii - 1) * 16 + 1]);
                expandedKey[ii * 16 + 2] = (byte)(SBox[expandedKey[ii * 16 - 1]] ^ expandedKey[(ii - 1) * 16 + 2]);
                expandedKey[ii * 16 + 3] = (byte)(SBox[buf1] ^ expandedKey[(ii - 1) * 16 + 3]);
                expandedKey[ii * 16 + 4] = (byte)(expandedKey[(ii - 1) * 16 + 4] ^ expandedKey[ii * 16 + 0]);
                expandedKey[ii * 16 + 5] = (byte)(expandedKey[(ii - 1) * 16 + 5] ^ expandedKey[ii * 16 + 1]);
                expandedKey[ii * 16 + 6] = (byte)(expandedKey[(ii - 1) * 16 + 6] ^ expandedKey[ii * 16 + 2]);
                expandedKey[ii * 16 + 7] = (byte)(expandedKey[(ii - 1) * 16 + 7] ^ expandedKey[ii * 16 + 3]);
                expandedKey[ii * 16 + 8] = (byte)(expandedKey[(ii - 1) * 16 + 8] ^ expandedKey[ii * 16 + 4]);
                expandedKey[ii * 16 + 9] = (byte)(expandedKey[(ii - 1) * 16 + 9] ^ expandedKey[ii * 16 + 5]);
                expandedKey[ii * 16 + 10] = (byte)(expandedKey[(ii - 1) * 16 + 10] ^ expandedKey[ii * 16 + 6]);
                expandedKey[ii * 16 + 11] = (byte)(expandedKey[(ii - 1) * 16 + 11] ^ expandedKey[ii * 16 + 7]);
                expandedKey[ii * 16 + 12] = (byte)(expandedKey[(ii - 1) * 16 + 12] ^ expandedKey[ii * 16 + 8]);
                expandedKey[ii * 16 + 13] = (byte)(expandedKey[(ii - 1) * 16 + 13] ^ expandedKey[ii * 16 + 9]);
                expandedKey[ii * 16 + 14] = (byte)(expandedKey[(ii - 1) * 16 + 14] ^ expandedKey[ii * 16 + 10]);
                expandedKey[ii * 16 + 15] = (byte)(expandedKey[(ii - 1) * 16 + 15] ^ expandedKey[ii * 16 + 11]);
            }
        }

        // multiply by 2 in the galois field
        static byte galois_mul2(int value)
        {
            if (value >> 7 != 0)
            {
                value = (value << 1);
                return (byte)(value ^ 0x1b);
            }
            else
            {
                return (byte)(value << 1);
            }
        }

        static void aes_encr(byte[] state, byte[] expandedKey)
        {
            byte buf1, buf2, buf3, round;

            for (round = 0; round < 9; round++)
            {
                // row 0
                state[0] = SBox[(state[0] ^ expandedKey[(round * 16)])];
                state[4] = SBox[(state[4] ^ expandedKey[(round * 16) + 4])];
                state[8] = SBox[(state[8] ^ expandedKey[(round * 16) + 8])];
                state[12] = SBox[(state[12] ^ expandedKey[(round * 16) + 12])];
                // row 1
                buf1 = (byte)(state[1] ^ expandedKey[(round * 16) + 1]);
                state[1] = SBox[(state[5] ^ expandedKey[(round * 16) + 5])];
                state[5] = SBox[(state[9] ^ expandedKey[(round * 16) + 9])];
                state[9] = SBox[(state[13] ^ expandedKey[(round * 16) + 13])];
                state[13] = SBox[buf1];
                // row 2
                buf1 = (byte)(state[2] ^ expandedKey[(round * 16) + 2]);
                buf2 = (byte)(state[6] ^ expandedKey[(round * 16) + 6]);
                state[2] = SBox[(state[10] ^ expandedKey[(round * 16) + 10])];
                state[6] = SBox[(state[14] ^ expandedKey[(round * 16) + 14])];
                state[10] = SBox[buf1];
                state[14] = SBox[buf2];
                // row 3
                buf1 = (byte)(state[15] ^ expandedKey[(round * 16) + 15]);
                state[15] = SBox[(state[11] ^ expandedKey[(round * 16) + 11])];
                state[11] = SBox[(state[7] ^ expandedKey[(round * 16) + 7])];
                state[7] = SBox[(state[3] ^ expandedKey[(round * 16) + 3])];
                state[3] = SBox[buf1];

                // col1
                buf1 = (byte)(state[0] ^ state[1] ^ state[2] ^ state[3]);
                buf2 = state[0];
                buf3 = (byte)(state[0] ^ state[1]);
                buf3 = galois_mul2(buf3);
                state[0] = (byte)(state[0] ^ buf3 ^ buf1);
                buf3 = (byte)(state[1] ^ state[2]);
                buf3 = galois_mul2(buf3);
                state[1] = (byte)(state[1] ^ buf3 ^ buf1);
                buf3 = (byte)(state[2] ^ state[3]);
                buf3 = galois_mul2(buf3);
                state[2] = (byte)(state[2] ^ buf3 ^ buf1);
                buf3 = (byte)(state[3] ^ buf2);
                buf3 = galois_mul2(buf3);
                state[3] = (byte)(state[3] ^ buf3 ^ buf1);
                // col2
                buf1 = (byte)(state[4] ^ state[5] ^ state[6] ^ state[7]);
                buf2 = state[4];
                buf3 = (byte)(state[4] ^ state[5]);
                buf3 = galois_mul2(buf3);
                state[4] = (byte)(state[4] ^ buf3 ^ buf1);
                buf3 = (byte)(state[5] ^ state[6]);
                buf3 = galois_mul2(buf3);
                state[5] = (byte)(state[5] ^ buf3 ^ buf1);
                buf3 = (byte)(state[6] ^ state[7]);
                buf3 = galois_mul2(buf3);
                state[6] = (byte)(state[6] ^ buf3 ^ buf1);
                buf3 = (byte)(state[7] ^ buf2);
                buf3 = galois_mul2(buf3);
                state[7] = (byte)(state[7] ^ buf3 ^ buf1);
                // col3
                buf1 = (byte)(state[8] ^ state[9] ^ state[10] ^ state[11]);
                buf2 = state[8];
                buf3 = (byte)(state[8] ^ state[9]);
                buf3 = galois_mul2(buf3);
                state[8] = (byte)(state[8] ^ buf3 ^ buf1);
                buf3 = (byte)(state[9] ^ state[10]);
                buf3 = galois_mul2(buf3);
                state[9] = (byte)(state[9] ^ buf3 ^ buf1);
                buf3 = (byte)(state[10] ^ state[11]);
                buf3 = galois_mul2(buf3);
                state[10] = (byte)(state[10] ^ buf3 ^ buf1);
                buf3 = (byte)(state[11] ^ buf2);
                buf3 = galois_mul2(buf3);
                state[11] = (byte)(state[11] ^ buf3 ^ buf1);
                // col4
                buf1 = (byte)(state[12] ^ state[13] ^ state[14] ^ state[15]);
                buf2 = state[12];
                buf3 = (byte)(state[12] ^ state[13]);
                buf3 = galois_mul2(buf3);
                state[12] = (byte)(state[12] ^ buf3 ^ buf1);
                buf3 = (byte)(state[13] ^ state[14]);
                buf3 = galois_mul2(buf3);
                state[13] = (byte)(state[13] ^ buf3 ^ buf1);
                buf3 = (byte)(state[14] ^ state[15]);
                buf3 = galois_mul2(buf3);
                state[14] = (byte)(state[14] ^ buf3 ^ buf1);
                buf3 = (byte)(state[15] ^ buf2);
                buf3 = galois_mul2(buf3);
                state[15] = (byte)(state[15] ^ buf3 ^ buf1);

            }
            // 10th round without mixcols
            state[0] = SBox[(state[0] ^ expandedKey[(round * 16)])];
            state[4] = SBox[(state[4] ^ expandedKey[(round * 16) + 4])];
            state[8] = SBox[(state[8] ^ expandedKey[(round * 16) + 8])];
            state[12] = SBox[(state[12] ^ expandedKey[(round * 16) + 12])];
            // row 1
            buf1 = (byte)(state[1] ^ expandedKey[(round * 16) + 1]);
            state[1] = SBox[(state[5] ^ expandedKey[(round * 16) + 5])];
            state[5] = SBox[(state[9] ^ expandedKey[(round * 16) + 9])];
            state[9] = SBox[(state[13] ^ expandedKey[(round * 16) + 13])];
            state[13] = SBox[buf1];
            // row 2
            buf1 = (byte)(state[2] ^ expandedKey[(round * 16) + 2]);
            buf2 = (byte)(state[6] ^ expandedKey[(round * 16) + 6]);
            state[2] = SBox[(state[10] ^ expandedKey[(round * 16) + 10])];
            state[6] = SBox[(state[14] ^ expandedKey[(round * 16) + 14])];
            state[10] = SBox[buf1];
            state[14] = SBox[buf2];
            // row 3
            buf1 = (byte)(state[15] ^ expandedKey[(round * 16) + 15]);
            state[15] = SBox[(state[11] ^ expandedKey[(round * 16) + 11])];
            state[11] = SBox[(state[7] ^ expandedKey[(round * 16) + 7])];
            state[7] = SBox[(state[3] ^ expandedKey[(round * 16) + 3])];
            state[3] = SBox[buf1];
            // last addroundkey
            state[0] ^= expandedKey[160];
            state[1] ^= expandedKey[161];
            state[2] ^= expandedKey[162];
            state[3] ^= expandedKey[163];
            state[4] ^= expandedKey[164];
            state[5] ^= expandedKey[165];
            state[6] ^= expandedKey[166];
            state[7] ^= expandedKey[167];
            state[8] ^= expandedKey[168];
            state[9] ^= expandedKey[169];
            state[10] ^= expandedKey[170];
            state[11] ^= expandedKey[171];
            state[12] ^= expandedKey[172];
            state[13] ^= expandedKey[173];
            state[14] ^= expandedKey[174];
            state[15] ^= expandedKey[175];
        }

        static void aes_decr(byte[] state, byte[] expandedKey)
        {
            byte buf1, buf2, buf3;
            byte round;
            round = 9;

            // initial addroundkey
            state[0] ^= expandedKey[160];
            state[1] ^= expandedKey[161];
            state[2] ^= expandedKey[162];
            state[3] ^= expandedKey[163];
            state[4] ^= expandedKey[164];
            state[5] ^= expandedKey[165];
            state[6] ^= expandedKey[166];
            state[7] ^= expandedKey[167];
            state[8] ^= expandedKey[168];
            state[9] ^= expandedKey[169];
            state[10] ^= expandedKey[170];
            state[11] ^= expandedKey[171];
            state[12] ^= expandedKey[172];
            state[13] ^= expandedKey[173];
            state[14] ^= expandedKey[174];
            state[15] ^= expandedKey[175];

            // 10th round without mixcols
            state[0] = (byte)(SBoxInverse[state[0]] ^ expandedKey[(round * 16)]);
            state[4] = (byte)(SBoxInverse[state[4]] ^ expandedKey[(round * 16) + 4]);
            state[8] = (byte)(SBoxInverse[state[8]] ^ expandedKey[(round * 16) + 8]);
            state[12] = (byte)(SBoxInverse[state[12]] ^ expandedKey[(round * 16) + 12]);
            // row 1
            buf1 = (byte)(SBoxInverse[state[13]] ^ expandedKey[(round * 16) + 1]);
            state[13] = (byte)(SBoxInverse[state[9]] ^ expandedKey[(round * 16) + 13]);
            state[9] = (byte)(SBoxInverse[state[5]] ^ expandedKey[(round * 16) + 9]);
            state[5] = (byte)(SBoxInverse[state[1]] ^ expandedKey[(round * 16) + 5]);
            state[1] = buf1;
            // row 2
            buf1 = (byte)(SBoxInverse[state[2]] ^ expandedKey[(round * 16) + 10]);
            buf2 = (byte)(SBoxInverse[state[6]] ^ expandedKey[(round * 16) + 14]);
            state[2] = (byte)(SBoxInverse[state[10]] ^ expandedKey[(round * 16) + 2]);
            state[6] = (byte)(SBoxInverse[state[14]] ^ expandedKey[(round * 16) + 6]);
            state[10] = buf1;
            state[14] = buf2;
            // row 3
            buf1 = (byte)(SBoxInverse[state[3]] ^ expandedKey[(round * 16) + 15]);
            state[3] = (byte)(SBoxInverse[state[7]] ^ expandedKey[(round * 16) + 3]);
            state[7] = (byte)(SBoxInverse[state[11]] ^ expandedKey[(round * 16) + 7]);
            state[11] = (byte)(SBoxInverse[state[15]] ^ expandedKey[(round * 16) + 11]);
            state[15] = buf1;

            for (round = 8; round >= 0; round--)
            {
                // barreto
                //col1
                buf1 = galois_mul2((byte)(galois_mul2(state[0] ^ state[2])));
                buf2 = galois_mul2((byte)(galois_mul2(state[1] ^ state[3])));
                state[0] ^= buf1; state[1] ^= buf2; state[2] ^= buf1; state[3] ^= buf2;
                //col2
                buf1 = galois_mul2(galois_mul2(state[4] ^ state[6]));
                buf2 = galois_mul2(galois_mul2(state[5] ^ state[7]));
                state[4] ^= buf1; state[5] ^= buf2; state[6] ^= buf1; state[7] ^= buf2;
                //col3
                buf1 = galois_mul2(galois_mul2(state[8] ^ state[10]));
                buf2 = galois_mul2(galois_mul2(state[9] ^ state[11]));
                state[8] ^= buf1; state[9] ^= buf2; state[10] ^= buf1; state[11] ^= buf2;
                //col4
                buf1 = galois_mul2(galois_mul2(state[12] ^ state[14]));
                buf2 = galois_mul2(galois_mul2(state[13] ^ state[15]));
                state[12] ^= buf1; state[13] ^= buf2; state[14] ^= buf1; state[15] ^= buf2;
                // mixcolums //////////
                // col1
                buf1 = (byte)(state[0] ^ state[1] ^ state[2] ^ state[3]);
                buf2 = state[0];
                buf3 = (byte)(state[0] ^ state[1]);
                buf3 = galois_mul2(buf3);
                state[0] = (byte)(state[0] ^ buf3 ^ buf1);
                buf3 = (byte)(state[1] ^ state[2]);
                buf3 = galois_mul2(buf3);
                state[1] = (byte)(state[1] ^ buf3 ^ buf1);
                buf3 = (byte)(state[2] ^ state[3]);
                buf3 = galois_mul2(buf3);
                state[2] = (byte)(state[2] ^ buf3 ^ buf1);
                buf3 = (byte)(state[3] ^ buf2);
                buf3 = galois_mul2(buf3);
                state[3] = (byte)(state[3] ^ buf3 ^ buf1);
                // col2
                buf1 = (byte)(state[4] ^ state[5] ^ state[6] ^ state[7]);
                buf2 = state[4];
                buf3 = (byte)(state[4] ^ state[5]);
                buf3 = galois_mul2(buf3);
                state[4] = (byte)(state[4] ^ buf3 ^ buf1);
                buf3 = (byte)(state[5] ^ state[6]);
                buf3 = galois_mul2(buf3);
                state[5] = (byte)(state[5] ^ buf3 ^ buf1);
                buf3 = (byte)(state[6] ^ state[7]);
                buf3 = galois_mul2(buf3);
                state[6] = (byte)(state[6] ^ buf3 ^ buf1);
                buf3 = (byte)(state[7] ^ buf2);
                buf3 = galois_mul2(buf3);
                state[7] = (byte)(state[7] ^ buf3 ^ buf1);
                // col3
                buf1 = (byte)(state[8] ^ state[9] ^ state[10] ^ state[11]);
                buf2 = state[8];
                buf3 = (byte)(state[8] ^ state[9]);
                buf3 = galois_mul2(buf3);
                state[8] = (byte)(state[8] ^ buf3 ^ buf1);
                buf3 = (byte)(state[9] ^ state[10]);
                buf3 = galois_mul2(buf3);
                state[9] = (byte)(state[9] ^ buf3 ^ buf1);
                buf3 = (byte)(state[10] ^ state[11]);
                buf3 = galois_mul2(buf3);
                state[10] = (byte)(state[10] ^ buf3 ^ buf1);
                buf3 = (byte)(state[11] ^ buf2);
                buf3 = galois_mul2(buf3);
                state[11] = (byte)(state[11] ^ buf3 ^ buf1);
                // col4
                buf1 = (byte)(state[12] ^ state[13] ^ state[14] ^ state[15]);
                buf2 = state[12];
                buf3 = (byte)(state[12] ^ state[13]);
                buf3 = galois_mul2(buf3);
                state[12] = (byte)(state[12] ^ buf3 ^ buf1);
                buf3 = (byte)(state[13] ^ state[14]);
                buf3 = galois_mul2(buf3);
                state[13] = (byte)(state[13] ^ buf3 ^ buf1);
                buf3 = (byte)(state[14] ^ state[15]);
                buf3 = galois_mul2(buf3);
                state[14] = (byte)(state[14] ^ buf3 ^ buf1);
                buf3 = (byte)(state[15] ^ buf2);
                buf3 = galois_mul2(buf3);
                state[15] = (byte)(state[15] ^ buf3 ^ buf1);

                // addroundkey, rsbox and shiftrows
                // row 0
                state[0] = (byte)(SBoxInverse[state[0]] ^ expandedKey[(round * 16)]);
                state[4] = (byte)(SBoxInverse[state[4]] ^ expandedKey[(round * 16) + 4]);
                state[8] = (byte)(SBoxInverse[state[8]] ^ expandedKey[(round * 16) + 8]);
                state[12] = (byte)(SBoxInverse[state[12]] ^ expandedKey[(round * 16) + 12]);
                // row 1
                buf1 = (byte)(SBoxInverse[state[13]] ^ expandedKey[(round * 16) + 1]);
                state[13] = (byte)(SBoxInverse[state[9]] ^ expandedKey[(round * 16) + 13]);
                state[9] = (byte)(SBoxInverse[state[5]] ^ expandedKey[(round * 16) + 9]);
                state[5] = (byte)(SBoxInverse[state[1]] ^ expandedKey[(round * 16) + 5]);
                state[1] = buf1;
                // row 2
                buf1 = (byte)(SBoxInverse[state[2]] ^ expandedKey[(round * 16) + 10]);
                buf2 = (byte)(SBoxInverse[state[6]] ^ expandedKey[(round * 16) + 14]);
                state[2] = (byte)(SBoxInverse[state[10]] ^ expandedKey[(round * 16) + 2]);
                state[6] = (byte)(SBoxInverse[state[14]] ^ expandedKey[(round * 16) + 6]);
                state[10] = buf1;
                state[14] = buf2;
                // row 3
                buf1 = (byte)(SBoxInverse[state[3]] ^ expandedKey[(round * 16) + 15]);
                state[3] = (byte)(SBoxInverse[state[7]] ^ expandedKey[(round * 16) + 3]);
                state[7] = (byte)(SBoxInverse[state[11]] ^ expandedKey[(round * 16) + 7]);
                state[11] = (byte)(SBoxInverse[state[15]] ^ expandedKey[(round * 16) + 11]);
                state[15] = buf1;
            }


        }

        // encrypt
        public static void Encrypt(byte[] challenge, byte[] secret)
        {
            byte[] expandedKey = new byte[176];
            expandKey(expandedKey, secret);       // expand the key into 176 bytes
            aes_encr(challenge, expandedKey);
        }
 
        // decrypt
        public static void Decrypt(byte[] state, byte[] key)
        {
            byte[] expandedKey = new byte[176];
            expandKey(expandedKey, key);       // expand the key into 176 bytes
            aes_decr(state, expandedKey);
        }
    }
}
