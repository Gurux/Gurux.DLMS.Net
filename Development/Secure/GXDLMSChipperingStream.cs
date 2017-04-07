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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Secure
{
    /// <summary>
    /// Implements GMAC.
    /// This class is based to this doc:
    /// http://csrc.nist.gov/publications/nistpubs/800-38D/SP-800-38D.pdf
    /// </summary>
    class GXDLMSChipperingStream
    {
        List<byte> Output = new List<byte>();
        //Consts.
        private const int BlockSize = 16;
        Gurux.DLMS.Enums.Security Security;
        //Properties.
        private readonly uint[][][] M = new uint[32][][];
        private ulong totalLength;
        private static readonly byte[] Zeroes = new byte[BlockSize];
        private byte[] S;
        private byte[] counter;
        private byte[] Aad;
        private byte[] J0;
        //How many butes are not crypted/encrypted.
        private int BytesRemaining;
        private byte[] H;
        private byte[] bufBlock;
        private byte[] Tag;
        private int Rounds;
        private uint[,] WorkingKey;
        private uint C0, C1, C2, C3;
        private bool Encrypt;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="encrypt"></param>
        /// <param name="kek">Key Encrypting Key, also known as Master key.</param>
        public GXDLMSChipperingStream(bool encrypt, byte[] kek)
        {
            Encrypt = encrypt;
            WorkingKey = GenerateKey(encrypt, kek);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="security">Used security level.</param>
        /// <param name="encrypt"></param>
        /// <param name="blockCipherKey"></param>
        /// <param name="aad"></param>
        /// <param name="iv"></param>
        /// <param name="tag"></param>
        public GXDLMSChipperingStream(Gurux.DLMS.Enums.Security security, bool encrypt, byte[] blockCipherKey, byte[] aad, byte[] iv, byte[] tag)
        {
            this.Security = security;
            const int TagSize = 0x10;
            this.Tag = tag;
            if (this.Tag == null)//Tag size is 12 bytes.
            {
                this.Tag = new byte[12];
            }
            else if (this.Tag.Length != 12)
            {
                throw new ArgumentOutOfRangeException("Invalid tag.");
            }
            Encrypt = encrypt;
            WorkingKey = GenerateKey(encrypt, blockCipherKey);
            int bufLength = Encrypt ? BlockSize : (BlockSize + TagSize);
            this.bufBlock = new byte[bufLength];
            Aad = aad;
            this.H = new byte[BlockSize];
            ProcessBlock(H, 0, H, 0);
            Init(H);
            this.J0 = new byte[16];
            Array.Copy(iv, 0, J0, 0, iv.Length);
            this.J0[15] = 0x01;
            this.S = GetGHash(Aad);
            this.counter = (byte[])J0.Clone();
            this.BytesRemaining = 0;
            this.totalLength = 0;
        }

        /// <summary>
        /// Get tag from crypted data.
        /// </summary>
        /// <returns>Tag from crypted data.</returns>
        public virtual byte[] GetTag()
        {
            return Tag;
        }

        /// <summary>
        /// Convert byte array to Little Endian.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        static UInt32 ToUInt32(byte[] value, int offset)
        {
            return (uint)(value[offset] | value[++offset] << 8 | value[++offset] << 16 | value[++offset] << 24);
        }

        static UInt32 SubWord(uint value)
        {
            return (uint)SBox[value & 0xFF] | (((uint)SBox[(value >> 8) & 0xFF]) << 8) |
                   (((uint)SBox[(value >> 16) & 0xFF]) << 16) | (((uint)SBox[(value >> 24) & 0xFF]) << 24);
        }

        /// <summary>
        /// Shift value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        UInt32 Shift(UInt32 value, int shift)
        {
            return (value >> shift) | (value << (32 - shift));
        }

        /// <summary>
        /// Key schedule Vector (powers of x).
        /// </summary>
        static readonly byte[] rcon =
        {
        0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a,
        0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91
    };


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

        #region Rijndael (AES) Encryption
        /// <summary>
        /// Rijndael (AES) Encryption fast table.
        /// </summary>
        private static readonly uint[] AES =
        {
        0xa56363c6, 0x847c7cf8, 0x997777ee, 0x8d7b7bf6, 0x0df2f2ff,
        0xbd6b6bd6, 0xb16f6fde, 0x54c5c591, 0x50303060, 0x03010102,
        0xa96767ce, 0x7d2b2b56, 0x19fefee7, 0x62d7d7b5, 0xe6abab4d,
        0x9a7676ec, 0x45caca8f, 0x9d82821f, 0x40c9c989, 0x877d7dfa,
        0x15fafaef, 0xeb5959b2, 0xc947478e, 0x0bf0f0fb, 0xecadad41,
        0x67d4d4b3, 0xfda2a25f, 0xeaafaf45, 0xbf9c9c23, 0xf7a4a453,
        0x967272e4, 0x5bc0c09b, 0xc2b7b775, 0x1cfdfde1, 0xae93933d,
        0x6a26264c, 0x5a36366c, 0x413f3f7e, 0x02f7f7f5, 0x4fcccc83,
        0x5c343468, 0xf4a5a551, 0x34e5e5d1, 0x08f1f1f9, 0x937171e2,
        0x73d8d8ab, 0x53313162, 0x3f15152a, 0x0c040408, 0x52c7c795,
        0x65232346, 0x5ec3c39d, 0x28181830, 0xa1969637, 0x0f05050a,
        0xb59a9a2f, 0x0907070e, 0x36121224, 0x9b80801b, 0x3de2e2df,
        0x26ebebcd, 0x6927274e, 0xcdb2b27f, 0x9f7575ea, 0x1b090912,
        0x9e83831d, 0x742c2c58, 0x2e1a1a34, 0x2d1b1b36, 0xb26e6edc,
        0xee5a5ab4, 0xfba0a05b, 0xf65252a4, 0x4d3b3b76, 0x61d6d6b7,
        0xceb3b37d, 0x7b292952, 0x3ee3e3dd, 0x712f2f5e, 0x97848413,
        0xf55353a6, 0x68d1d1b9, 0x00000000, 0x2cededc1, 0x60202040,
        0x1ffcfce3, 0xc8b1b179, 0xed5b5bb6, 0xbe6a6ad4, 0x46cbcb8d,
        0xd9bebe67, 0x4b393972, 0xde4a4a94, 0xd44c4c98, 0xe85858b0,
        0x4acfcf85, 0x6bd0d0bb, 0x2aefefc5, 0xe5aaaa4f, 0x16fbfbed,
        0xc5434386, 0xd74d4d9a, 0x55333366, 0x94858511, 0xcf45458a,
        0x10f9f9e9, 0x06020204, 0x817f7ffe, 0xf05050a0, 0x443c3c78,
        0xba9f9f25, 0xe3a8a84b, 0xf35151a2, 0xfea3a35d, 0xc0404080,
        0x8a8f8f05, 0xad92923f, 0xbc9d9d21, 0x48383870, 0x04f5f5f1,
        0xdfbcbc63, 0xc1b6b677, 0x75dadaaf, 0x63212142, 0x30101020,
        0x1affffe5, 0x0ef3f3fd, 0x6dd2d2bf, 0x4ccdcd81, 0x140c0c18,
        0x35131326, 0x2fececc3, 0xe15f5fbe, 0xa2979735, 0xcc444488,
        0x3917172e, 0x57c4c493, 0xf2a7a755, 0x827e7efc, 0x473d3d7a,
        0xac6464c8, 0xe75d5dba, 0x2b191932, 0x957373e6, 0xa06060c0,
        0x98818119, 0xd14f4f9e, 0x7fdcdca3, 0x66222244, 0x7e2a2a54,
        0xab90903b, 0x8388880b, 0xca46468c, 0x29eeeec7, 0xd3b8b86b,
        0x3c141428, 0x79dedea7, 0xe25e5ebc, 0x1d0b0b16, 0x76dbdbad,
        0x3be0e0db, 0x56323264, 0x4e3a3a74, 0x1e0a0a14, 0xdb494992,
        0x0a06060c, 0x6c242448, 0xe45c5cb8, 0x5dc2c29f, 0x6ed3d3bd,
        0xefacac43, 0xa66262c4, 0xa8919139, 0xa4959531, 0x37e4e4d3,
        0x8b7979f2, 0x32e7e7d5, 0x43c8c88b, 0x5937376e, 0xb76d6dda,
        0x8c8d8d01, 0x64d5d5b1, 0xd24e4e9c, 0xe0a9a949, 0xb46c6cd8,
        0xfa5656ac, 0x07f4f4f3, 0x25eaeacf, 0xaf6565ca, 0x8e7a7af4,
        0xe9aeae47, 0x18080810, 0xd5baba6f, 0x887878f0, 0x6f25254a,
        0x722e2e5c, 0x241c1c38, 0xf1a6a657, 0xc7b4b473, 0x51c6c697,
        0x23e8e8cb, 0x7cdddda1, 0x9c7474e8, 0x211f1f3e, 0xdd4b4b96,
        0xdcbdbd61, 0x868b8b0d, 0x858a8a0f, 0x907070e0, 0x423e3e7c,
        0xc4b5b571, 0xaa6666cc, 0xd8484890, 0x05030306, 0x01f6f6f7,
        0x120e0e1c, 0xa36161c2, 0x5f35356a, 0xf95757ae, 0xd0b9b969,
        0x91868617, 0x58c1c199, 0x271d1d3a, 0xb99e9e27, 0x38e1e1d9,
        0x13f8f8eb, 0xb398982b, 0x33111122, 0xbb6969d2, 0x70d9d9a9,
        0x898e8e07, 0xa7949433, 0xb69b9b2d, 0x221e1e3c, 0x92878715,
        0x20e9e9c9, 0x49cece87, 0xff5555aa, 0x78282850, 0x7adfdfa5,
        0x8f8c8c03, 0xf8a1a159, 0x80898909, 0x170d0d1a, 0xdabfbf65,
        0x31e6e6d7, 0xc6424284, 0xb86868d0, 0xc3414182, 0xb0999929,
        0x772d2d5a, 0x110f0f1e, 0xcbb0b07b, 0xfc5454a8, 0xd6bbbb6d,
        0x3a16162c
    };

        private static readonly uint[] Reversed_AES1 =
        {
        0x50a7f451, 0x5365417e, 0xc3a4171a, 0x965e273a, 0xcb6bab3b,
        0xf1459d1f, 0xab58faac, 0x9303e34b, 0x55fa3020, 0xf66d76ad,
        0x9176cc88, 0x254c02f5, 0xfcd7e54f, 0xd7cb2ac5, 0x80443526,
        0x8fa362b5, 0x495ab1de, 0x671bba25, 0x980eea45, 0xe1c0fe5d,
        0x02752fc3, 0x12f04c81, 0xa397468d, 0xc6f9d36b, 0xe75f8f03,
        0x959c9215, 0xeb7a6dbf, 0xda595295, 0x2d83bed4, 0xd3217458,
        0x2969e049, 0x44c8c98e, 0x6a89c275, 0x78798ef4, 0x6b3e5899,
        0xdd71b927, 0xb64fe1be, 0x17ad88f0, 0x66ac20c9, 0xb43ace7d,
        0x184adf63, 0x82311ae5, 0x60335197, 0x457f5362, 0xe07764b1,
        0x84ae6bbb, 0x1ca081fe, 0x942b08f9, 0x58684870, 0x19fd458f,
        0x876cde94, 0xb7f87b52, 0x23d373ab, 0xe2024b72, 0x578f1fe3,
        0x2aab5566, 0x0728ebb2, 0x03c2b52f, 0x9a7bc586, 0xa50837d3,
        0xf2872830, 0xb2a5bf23, 0xba6a0302, 0x5c8216ed, 0x2b1ccf8a,
        0x92b479a7, 0xf0f207f3, 0xa1e2694e, 0xcdf4da65, 0xd5be0506,
        0x1f6234d1, 0x8afea6c4, 0x9d532e34, 0xa055f3a2, 0x32e18a05,
        0x75ebf6a4, 0x39ec830b, 0xaaef6040, 0x069f715e, 0x51106ebd,
        0xf98a213e, 0x3d06dd96, 0xae053edd, 0x46bde64d, 0xb58d5491,
        0x055dc471, 0x6fd40604, 0xff155060, 0x24fb9819, 0x97e9bdd6,
        0xcc434089, 0x779ed967, 0xbd42e8b0, 0x888b8907, 0x385b19e7,
        0xdbeec879, 0x470a7ca1, 0xe90f427c, 0xc91e84f8, 0x00000000,
        0x83868009, 0x48ed2b32, 0xac70111e, 0x4e725a6c, 0xfbff0efd,
        0x5638850f, 0x1ed5ae3d, 0x27392d36, 0x64d90f0a, 0x21a65c68,
        0xd1545b9b, 0x3a2e3624, 0xb1670a0c, 0x0fe75793, 0xd296eeb4,
        0x9e919b1b, 0x4fc5c080, 0xa220dc61, 0x694b775a, 0x161a121c,
        0x0aba93e2, 0xe52aa0c0, 0x43e0223c, 0x1d171b12, 0x0b0d090e,
        0xadc78bf2, 0xb9a8b62d, 0xc8a91e14, 0x8519f157, 0x4c0775af,
        0xbbdd99ee, 0xfd607fa3, 0x9f2601f7, 0xbcf5725c, 0xc53b6644,
        0x347efb5b, 0x7629438b, 0xdcc623cb, 0x68fcedb6, 0x63f1e4b8,
        0xcadc31d7, 0x10856342, 0x40229713, 0x2011c684, 0x7d244a85,
        0xf83dbbd2, 0x1132f9ae, 0x6da129c7, 0x4b2f9e1d, 0xf330b2dc,
        0xec52860d, 0xd0e3c177, 0x6c16b32b, 0x99b970a9, 0xfa489411,
        0x2264e947, 0xc48cfca8, 0x1a3ff0a0, 0xd82c7d56, 0xef903322,
        0xc74e4987, 0xc1d138d9, 0xfea2ca8c, 0x360bd498, 0xcf81f5a6,
        0x28de7aa5, 0x268eb7da, 0xa4bfad3f, 0xe49d3a2c, 0x0d927850,
        0x9bcc5f6a, 0x62467e54, 0xc2138df6, 0xe8b8d890, 0x5ef7392e,
        0xf5afc382, 0xbe805d9f, 0x7c93d069, 0xa92dd56f, 0xb31225cf,
        0x3b99acc8, 0xa77d1810, 0x6e639ce8, 0x7bbb3bdb, 0x097826cd,
        0xf418596e, 0x01b79aec, 0xa89a4f83, 0x656e95e6, 0x7ee6ffaa,
        0x08cfbc21, 0xe6e815ef, 0xd99be7ba, 0xce366f4a, 0xd4099fea,
        0xd67cb029, 0xafb2a431, 0x31233f2a, 0x3094a5c6, 0xc066a235,
        0x37bc4e74, 0xa6ca82fc, 0xb0d090e0, 0x15d8a733, 0x4a9804f1,
        0xf7daec41, 0x0e50cd7f, 0x2ff69117, 0x8dd64d76, 0x4db0ef43,
        0x544daacc, 0xdf0496e4, 0xe3b5d19e, 0x1b886a4c, 0xb81f2cc1,
        0x7f516546, 0x04ea5e9d, 0x5d358c01, 0x737487fa, 0x2e410bfb,
        0x5a1d67b3, 0x52d2db92, 0x335610e9, 0x1347d66d, 0x8c61d79a,
        0x7a0ca137, 0x8e14f859, 0x893c13eb, 0xee27a9ce, 0x35c961b7,
        0xede51ce1, 0x3cb1477a, 0x59dfd29c, 0x3f73f255, 0x79ce1418,
        0xbf37c773, 0xeacdf753, 0x5baafd5f, 0x146f3ddf, 0x86db4478,
        0x81f3afca, 0x3ec468b9, 0x2c342438, 0x5f40a3c2, 0x72c31d16,
        0x0c25e2bc, 0x8b493c28, 0x41950dff, 0x7101a839, 0xdeb30c08,
        0x9ce4b4d8, 0x90c15664, 0x6184cb7b, 0x70b632d5, 0x745c6c48,
        0x4257b8d0
    };

        #endregion //AES

        /// <summary>
        /// Initialise the key schedule from the user supplied key.
        /// </summary>
        /// <returns></returns>
        private uint StarX(uint value)
        {
            const uint m1 = 0x80808080;
            const uint m2 = 0x7f7f7f7f;
            const uint m3 = 0x0000001b;
            return ((value & m2) << 1) ^ (((value & m1) >> 7) * m3);
        }

        private uint ImixCol(uint x)
        {
            uint f2 = StarX(x);
            uint f4 = StarX(f2);
            uint f8 = StarX(f4);
            uint f9 = x ^ f8;
            return f2 ^ f4 ^ f8 ^ Shift(f2 ^ f9, 8) ^ Shift(f4 ^ f9, 16) ^ Shift(f9, 24);
        }

        /// <summary>
        /// Get bytes from UIn32.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        internal static void GetUInt32(uint value, byte[] data, int offset)
        {
            data[offset] = (byte)(value);
            data[++offset] = (byte)(value >> 8);
            data[++offset] = (byte)(value >> 16);
            data[++offset] = (byte)(value >> 24);
        }

        private void UnPackBlock(byte[] bytes, int offset)
        {
            C0 = ToUInt32(bytes, offset);
            C1 = ToUInt32(bytes, offset + 4);
            C2 = ToUInt32(bytes, offset + 8);
            C3 = ToUInt32(bytes, offset + 12);
        }

        private void PackBlock(byte[] bytes, int offset)
        {
            GetUInt32(C0, bytes, offset);
            GetUInt32(C1, bytes, offset + 4);
            GetUInt32(C2, bytes, offset + 8);
            GetUInt32(C3, bytes, offset + 12);
        }

        /// <summary>
        /// Encrypt data block.
        /// </summary>
        /// <param name="key"></param>
        void EncryptBlock(uint[,] key)
        {
            int r;
            uint r0, r1, r2, r3;
            C0 ^= key[0, 0];
            C1 ^= key[0, 1];
            C2 ^= key[0, 2];
            C3 ^= key[0, 3];
            for (r = 1; r < Rounds - 1;)
            {
                r0 = AES[C0 & 0xFF] ^ Shift(AES[(C1 >> 8) & 0xFF], 24) ^ Shift(AES[(C2 >> 16) & 0xFF], 16) ^ Shift(AES[C3 >> 24], 8) ^ key[r, 0];
                r1 = AES[C1 & 0xFF] ^ Shift(AES[(C2 >> 8) & 0xFF], 24) ^ Shift(AES[(C3 >> 16) & 0xFF], 16) ^ Shift(AES[C0 >> 24], 8) ^ key[r, 1];
                r2 = AES[C2 & 0xFF] ^ Shift(AES[(C3 >> 8) & 0xFF], 24) ^ Shift(AES[(C0 >> 16) & 0xFF], 16) ^ Shift(AES[C1 >> 24], 8) ^ key[r, 2];
                r3 = AES[C3 & 0xFF] ^ Shift(AES[(C0 >> 8) & 0xFF], 24) ^ Shift(AES[(C1 >> 16) & 0xFF], 16) ^ Shift(AES[C2 >> 24], 8) ^ key[r++, 3];
                C0 = AES[r0 & 0xFF] ^ Shift(AES[(r1 >> 8) & 0xFF], 24) ^ Shift(AES[(r2 >> 16) & 0xFF], 16) ^ Shift(AES[r3 >> 24], 8) ^ key[r, 0];
                C1 = AES[r1 & 0xFF] ^ Shift(AES[(r2 >> 8) & 0xFF], 24) ^ Shift(AES[(r3 >> 16) & 0xFF], 16) ^ Shift(AES[r0 >> 24], 8) ^ key[r, 1];
                C2 = AES[r2 & 0xFF] ^ Shift(AES[(r3 >> 8) & 0xFF], 24) ^ Shift(AES[(r0 >> 16) & 0xFF], 16) ^ Shift(AES[r1 >> 24], 8) ^ key[r, 2];
                C3 = AES[r3 & 0xFF] ^ Shift(AES[(r0 >> 8) & 0xFF], 24) ^ Shift(AES[(r1 >> 16) & 0xFF], 16) ^ Shift(AES[r2 >> 24], 8) ^ key[r++, 3];
            }
            r0 = AES[C0 & 0xFF] ^ Shift(AES[(C1 >> 8) & 0xFF], 24) ^ Shift(AES[(C2 >> 16) & 0xFF], 16) ^ Shift(AES[C3 >> 24], 8) ^ key[r, 0];
            r1 = AES[C1 & 0xFF] ^ Shift(AES[(C2 >> 8) & 0xFF], 24) ^ Shift(AES[(C3 >> 16) & 0xFF], 16) ^ Shift(AES[C0 >> 24], 8) ^ key[r, 1];
            r2 = AES[C2 & 0xFF] ^ Shift(AES[(C3 >> 8) & 0xFF], 24) ^ Shift(AES[(C0 >> 16) & 0xFF], 16) ^ Shift(AES[C1 >> 24], 8) ^ key[r, 2];
            r3 = AES[C3 & 0xFF] ^ Shift(AES[(C0 >> 8) & 0xFF], 24) ^ Shift(AES[(C1 >> 16) & 0xFF], 16) ^ Shift(AES[C2 >> 24], 8) ^ key[r++, 3];

            C0 = (uint)SBox[r0 & 0xFF] ^ (((uint)SBox[(r1 >> 8) & 0xFF]) << 8) ^ (((uint)SBox[(r2 >> 16) & 0xFF]) << 16) ^ (((uint)SBox[r3 >> 24]) << 24) ^ key[r, 0];
            C1 = (uint)SBox[r1 & 0xFF] ^ (((uint)SBox[(r2 >> 8) & 0xFF]) << 8) ^ (((uint)SBox[(r3 >> 16) & 0xFF]) << 16) ^ (((uint)SBox[r0 >> 24]) << 24) ^ key[r, 1];
            C2 = (uint)SBox[r2 & 0xFF] ^ (((uint)SBox[(r3 >> 8) & 0xFF]) << 8) ^ (((uint)SBox[(r0 >> 16) & 0xFF]) << 16) ^ (((uint)SBox[r1 >> 24]) << 24) ^ key[r, 2];
            C3 = (uint)SBox[r3 & 0xFF] ^ (((uint)SBox[(r0 >> 8) & 0xFF]) << 8) ^ (((uint)SBox[(r1 >> 16) & 0xFF]) << 16) ^ (((uint)SBox[r2 >> 24]) << 24) ^ key[r, 3];
        }

        private void DecryptBlock(uint[,] key)
        {
            uint t0 = this.C0 ^ key[Rounds, 0];
            uint t1 = this.C1 ^ key[Rounds, 1];
            uint t2 = this.C2 ^ key[Rounds, 2];

            uint r0, r1, r2, r3 = this.C3 ^ key[Rounds, 3];
            int r = Rounds - 1;
            while (r > 1)
            {
                r0 = Reversed_AES1[t0 & 255] ^ Shift(Reversed_AES1[(r3 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(t2 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(t1 >> 24) & 255], 8) ^ key[r, 0];
                r1 = Reversed_AES1[t1 & 255] ^ Shift(Reversed_AES1[(t0 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(r3 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(t2 >> 24) & 255], 8) ^ key[r, 1];
                r2 = Reversed_AES1[t2 & 255] ^ Shift(Reversed_AES1[(t1 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(t0 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(r3 >> 24) & 255], 8) ^ key[r, 2];
                r3 = Reversed_AES1[r3 & 255] ^ Shift(Reversed_AES1[(t2 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(t1 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(t0 >> 24) & 255], 8) ^ key[r, 3];
                --r;
                t0 = Reversed_AES1[r0 & 255] ^ Shift(Reversed_AES1[(r3 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(r2 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(r1 >> 24) & 255], 8) ^ key[r, 0];
                t1 = Reversed_AES1[r1 & 255] ^ Shift(Reversed_AES1[(r0 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(r3 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(r2 >> 24) & 255], 8) ^ key[r, 1];
                t2 = Reversed_AES1[r2 & 255] ^ Shift(Reversed_AES1[(r1 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(r0 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(r3 >> 24) & 255], 8) ^ key[r, 2];
                r3 = Reversed_AES1[r3 & 255] ^ Shift(Reversed_AES1[(r2 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(r1 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(r0 >> 24) & 255], 8) ^ key[r, 3];
                --r;
            }

            r = 1;
            r0 = Reversed_AES1[t0 & 255] ^ Shift(Reversed_AES1[(r3 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(t2 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(t1 >> 24) & 255], 8) ^ key[r, 0];
            r1 = Reversed_AES1[t1 & 255] ^ Shift(Reversed_AES1[(t0 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(r3 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(t2 >> 24) & 255], 8) ^ key[r, 1];
            r2 = Reversed_AES1[t2 & 255] ^ Shift(Reversed_AES1[(t1 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(t0 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(r3 >> 24) & 255], 8) ^ key[r, 2];
            r3 = Reversed_AES1[r3 & 255] ^ Shift(Reversed_AES1[(t2 >> 8) & 255], 24) ^ Shift(Reversed_AES1[(t1 >> 16) & 255], 16) ^ Shift(Reversed_AES1[(t0 >> 24) & 255], 8) ^ key[r, 3];

            r = 0;
            this.C0 = (uint)SBoxInverse[r0 & 255] ^ (((uint)SBoxInverse[(r3 >> 8) & 255]) << 8) ^ (((uint)SBoxInverse[(r2 >> 16) & 255]) << 16) ^ (((uint)SBoxInverse[(r1 >> 24) & 255]) << 24) ^ key[r, 0];
            this.C1 = (uint)SBoxInverse[r1 & 255] ^ (((uint)SBoxInverse[(r0 >> 8) & 255]) << 8) ^ (((uint)SBoxInverse[(r3 >> 16) & 255]) << 16) ^ (((uint)SBoxInverse[(r2 >> 24) & 255]) << 24) ^ key[r, 1];
            this.C2 = (uint)SBoxInverse[r2 & 255] ^ (((uint)SBoxInverse[(r1 >> 8) & 255]) << 8) ^ (((uint)SBoxInverse[(r0 >> 16) & 255]) << 16) ^ (((uint)SBoxInverse[(r3 >> 24) & 255]) << 24) ^ key[r, 2];
            this.C3 = (uint)SBoxInverse[r3 & 255] ^ (((uint)SBoxInverse[(r2 >> 8) & 255]) << 8) ^ (((uint)SBoxInverse[(r1 >> 16) & 255]) << 16) ^ (((uint)SBoxInverse[(r0 >> 24) & 255]) << 24) ^ key[r, 3];
        }

        int ProcessBlock(byte[] input, int inOffset, byte[] output, int outOffset)
        {
            if ((inOffset + (32 / 2)) > input.Length)
            {
                throw new ArgumentOutOfRangeException("Invalid input buffer.");
            }

            if ((outOffset + (32 / 2)) > output.Length)
            {
                throw new ArgumentOutOfRangeException("Invalid output buffer.");
            }

            UnPackBlock(input, inOffset);
            if (Encrypt)
            {
                EncryptBlock(WorkingKey);
            }
            else
            {
                DecryptBlock(WorkingKey);
            }
            PackBlock(output, outOffset);
            return BlockSize;
        }

        /// <summary>
        /// Convert Big Endian byte array to Little Endian UInt 32.
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static uint BEToUInt32(byte[] buff, int offset)
        {
            return (uint)((buff[offset] << 24) | buff[++offset] << 16 | (buff[++offset] << 8) | buff[++offset]);
        }

        /// <summary>
        /// Shift block to right.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="count"></param>
        internal static void ShiftRight(uint[] block, int count)
        {
            uint bit = 0;
            for (int i = 0; i < 4; ++i)
            {
                uint b = block[i];
                block[i] = (b >> count) | bit;
                bit = b << (32 - count);
            }
        }

        internal static void MultiplyP(uint[] x)
        {
            bool lsb = (x[3] & 1) != 0;
            ShiftRight(x, 1);
            if (lsb)
            {
                x[0] ^= 0xe1000000;
            }
        }

        /// <summary>
        /// Get Uint 128 as array of UInt32.
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        internal static uint[] GetUint128(byte[] buff)
        {
            uint[] us = new uint[4];
            us[0] = BEToUInt32(buff, 0);
            us[1] = BEToUInt32(buff, 4);
            us[2] = BEToUInt32(buff, 8);
            us[3] = BEToUInt32(buff, 12);
            return us;
        }

        /// <summary>
        /// Make Xor for 128 bits.
        /// </summary>
        /// <param name="block">block.</param>
        /// <param name="value"></param>
        static void Xor(byte[] block, byte[] value)
        {
            for (int pos = 0; pos != 16; ++pos)
            {
                block[pos] ^= value[pos];
            }
        }

        /// <summary>
        /// Make Xor for 128 bits.
        /// </summary>
        /// <param name="block">block.</param>
        /// <param name="value"></param>
        static void Xor(uint[] block, uint[] value)
        {
            for (int pos = 0; pos != 4; ++pos)
            {
                block[pos] ^= value[pos];
            }
        }

        static void MultiplyP8(uint[] x)
        {
            uint lsw = x[3];
            ShiftRight(x, 8);
            for (int pos = 0; pos != 8; ++pos)
            {
                if ((lsw & (1 << pos)) != 0)
                {
                    x[0] ^= (0xe1000000 >> (7 - pos));
                }
            }
        }

        /// <summary>
        /// The GF(2128) field used is defined by the polynomial x^{128}+x^7+x^2+x+1.
        /// The authentication tag is constructed by feeding blocks of data into the GHASH function,
        /// and encrypting the result.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        byte[] GetGHash(byte[] b)
        {
            byte[] Y = new byte[16];
            for (int pos = 0; pos < b.Length; pos += 16)
            {
                byte[] X = new byte[16];
                int cnt = System.Math.Min(b.Length - pos, 16);
                Array.Copy(b, pos, X, 0, cnt);
                Xor(Y, X);
                MultiplyH(Y);
            }
            return Y;
        }

        /// <summary>
        /// Convert uint32 to Big Endian byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buff"></param>
        /// <param name="offset"></param>
        static void UInt32_To_BE(uint value, byte[] buff, int offset)
        {
            buff[offset] = (byte)(value >> 24);
            buff[++offset] = (byte)(value >> 16);
            buff[++offset] = (byte)(value >> 8);
            buff[++offset] = (byte)(value);
        }

        void MultiplyH(byte[] value)
        {
            uint[] tmp = new uint[4];
            for (int pos = 0; pos != 16; ++pos)
            {
                uint[] m = M[pos + pos][value[pos] & 0x0f];
                tmp[0] ^= m[0];
                tmp[1] ^= m[1];
                tmp[2] ^= m[2];
                tmp[3] ^= m[3];
                m = M[pos + pos + 1][(value[pos] & 0xf0) >> 4];
                tmp[0] ^= m[0];
                tmp[1] ^= m[1];
                tmp[2] ^= m[2];
                tmp[3] ^= m[3];
            }

            UInt32_To_BE(tmp[0], value, 0);
            UInt32_To_BE(tmp[1], value, 4);
            UInt32_To_BE(tmp[2], value, 8);
            UInt32_To_BE(tmp[3], value, 12);
        }

        void Init(byte[] H)
        {
            M[0] = new uint[16][];
            M[1] = new uint[16][];
            M[0][0] = new uint[4];
            M[1][0] = new uint[4];
            M[1][8] = GetUint128(H);
            uint[] tmp;
            for (int pos = 4; pos >= 1; pos >>= 1)
            {
                tmp = (uint[])M[1][pos + pos].Clone();
                MultiplyP(tmp);
                M[1][pos] = tmp;
            }
            tmp = (uint[])M[1][1].Clone();
            MultiplyP(tmp);
            M[0][8] = tmp;

            for (int pos = 4; pos >= 1; pos >>= 1)
            {
                tmp = (uint[])M[0][pos + pos].Clone();
                MultiplyP(tmp);
                M[0][pos] = tmp;
            }

            for (int pos1 = 0; ;)
            {
                for (int pos2 = 2; pos2 < 16; pos2 += pos2)
                {
                    for (int k = 1; k < pos2; ++k)
                    {
                        tmp = (uint[])M[pos1][pos2].Clone();
                        Xor(tmp, M[pos1][k]);
                        M[pos1][pos2 + k] = tmp;
                    }
                }

                if (++pos1 == 32)
                {
                    return;
                }

                if (pos1 > 1)
                {
                    M[pos1] = new uint[16][];
                    M[pos1][0] = new uint[4];
                    for (int pos = 8; pos > 0; pos >>= 1)
                    {
                        tmp = (uint[])M[pos1 - 2][pos].Clone();
                        MultiplyP8(tmp);
                        M[pos1][pos] = tmp;
                    }
                }
            }
        }

        void gCTRBlock(byte[] buf, int bufCount)
        {
            for (int i = 15; i >= 12; --i)
            {
                if (++counter[i] != 0)
                {
                    break;
                }
            }
            byte[] tmp = new byte[BlockSize];
            ProcessBlock(counter, 0, tmp, 0);
            byte[] hashBytes;
            if (Encrypt)
            {
                Array.Copy(Zeroes, bufCount, tmp, bufCount, BlockSize - bufCount);
                hashBytes = tmp;
            }
            else
            {
                hashBytes = buf;
            }
            for (int pos = 0; pos != bufCount; ++pos)
            {
                tmp[pos] ^= buf[pos];
                Output.Add(tmp[pos]);
            }
            Xor(S, hashBytes);
            MultiplyH(S);
            totalLength += (ulong)bufCount;
        }

        /// <summary>
        /// Set packet length to byte array.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="buff"></param>
        /// <param name="offset"></param>
        static void SetPackLength(ulong length, byte[] buff, int offset)
        {
            UInt32_To_BE((uint)(length >> 32), buff, offset);
            UInt32_To_BE((uint)length, buff, offset + 4);
        }

        /// <summary>
        /// Reset
        /// </summary>
        void Reset()
        {
            S = GetGHash(Aad);
            counter = (byte[])J0.Clone();
            BytesRemaining = 0;
            totalLength = 0;
        }

        /// <summary>
        /// Are tags equals.
        /// </summary>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        /// <returns></returns>
        internal static bool TagsEquals(byte[] tag1, byte[] tag2)
        {
            for (int pos = 0; pos != 12; ++pos)
            {
                if (tag1[pos] != tag2[pos])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Write bytes to decrypt/encrypt.
        /// </summary>
        /// <param name="input"></param>
        public virtual void Write(byte[] input)
        {
            foreach (byte it in input)
            {
                bufBlock[BytesRemaining++] = it;
                if (BytesRemaining == BlockSize)
                {
                    gCTRBlock(bufBlock, BlockSize);
                    if (!Encrypt)
                    {
                        Array.Copy(bufBlock, BlockSize, bufBlock, 0, Tag.Length);
                    }
                    BytesRemaining = 0;
                }
            }
        }

        /// <summary>
        /// Encrypt data using AES RFC3394.
        /// </summary>
        /// <param name="data">Encrypted data.</param>
        internal virtual byte[] EncryptAes(byte[] data)
        {
            int n = data.Length / 8;

            if ((n * 8) != data.Length)
            {
                throw new ArgumentOutOfRangeException("Invalid data.");
            }
            byte[] iv = { 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6 };
            byte[] block = new byte[data.Length + iv.Length];
            byte[] buf = new byte[8 + iv.Length];

            Array.Copy(iv, 0, block, 0, iv.Length);
            Array.Copy(data, 0, block, iv.Length, data.Length);

            for (int j = 0; j != 6; j++)
            {
                for (int i = 1; i <= n; i++)
                {
                    Array.Copy(block, 0, buf, 0, iv.Length);
                    Array.Copy(block, 8 * i, buf, iv.Length, 8);
                    ProcessBlock(buf, 0, buf, 0);

                    int t = n * j + i;
                    for (int k = 1; t != 0; k++)
                    {
                        byte v = (byte)t;

                        buf[iv.Length - k] ^= v;
                        t = (int)((uint)t >> 8);
                    }

                    Array.Copy(buf, 0, block, 0, 8);
                    Array.Copy(buf, 8, block, 8 * i, 8);
                }
            }
            return block;
        }

        /// <summary>
        /// Decrypt data using AES RFC3394.
        /// </summary>
        /// <param name="input">Decrypted data.</param>
        internal byte[] DecryptAes(byte[] input)
        {
            int n = input.Length / 8;

            if ((n * 8) != input.Length)
            {
                throw new ArgumentOutOfRangeException("Invalid data.");
            }

            byte[] iv = { 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6 };
            byte[] block;
            if (input.Length > iv.Length)
            {
                block = new byte[input.Length - iv.Length];
            }
            else
            {
                block = new byte[iv.Length];
            }
            byte[] a = new byte[iv.Length];
            byte[] buf = new byte[8 + iv.Length];

            Array.Copy(input, 0, a, 0, iv.Length);
            Array.Copy(input, 0 + iv.Length, block, 0, input.Length - iv.Length);

            n = n - 1;
            if (n == 0)
            {
                n = 1;
            }

            for (int j = 5; j >= 0; j--)
            {
                for (int i = n; i >= 1; i--)
                {
                    Array.Copy(a, 0, buf, 0, iv.Length);
                    Array.Copy(block, 8 * (i - 1), buf, iv.Length, 8);

                    int t = n * j + i;
                    for (int k = 1; t != 0; k++)
                    {
                        byte v = (byte)t;

                        buf[iv.Length - k] ^= v;
                        t = (int)((uint)t >> 8);
                    }

                    ProcessBlock(buf, 0, buf, 0);
                    Array.Copy(buf, 0, a, 0, 8);
                    Array.Copy(buf, 8, block, 8 * (i - 1), 8);
                }
            }
            if (!GXCommon.Compare(a, iv))
            {
                throw new ArithmeticException("Invalid CRC");
            }
            return block;
        }

        /// <summary>
        /// Process encrypting/decrypting.
        /// </summary>
        /// <returns></returns>
        public byte[] FlushFinalBlock()
        {
            //Crypt/Uncrypt remaining bytes.
            if (BytesRemaining > 0)
            {
                byte[] tmp = new byte[BlockSize];
                Array.Copy(bufBlock, 0, tmp, 0, BytesRemaining);
                gCTRBlock(tmp, BytesRemaining);
            }
            //If tag is not needed.
            if (Security == Gurux.DLMS.Enums.Security.Encryption)
            {
                Reset();
                return this.Output.ToArray();
            }
            //Count HASH.
            byte[] X = new byte[16];
            SetPackLength((ulong)Aad.Length * 8UL, X, 0);
            SetPackLength(totalLength * 8UL, X, 8);

            Xor(S, X);
            MultiplyH(S);
            byte[] tag = new byte[BlockSize];
            ProcessBlock(J0, 0, tag, 0);
            Xor(tag, S);
            if (!Encrypt)
            {
                if (!TagsEquals(this.Tag, tag))
                {
                    throw new GXDLMSException("Decrypt failed. Invalid tag.");
                }
            }
            else
            {
                //Tag size is 12 bytes.
                Array.Copy(tag, 0, Tag, 0, 12);
            }
            Reset();
            return this.Output.ToArray();
        }

        /// <summary>
        /// Generate AES keys.
        /// </summary>
        /// <param name="encrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        uint[,] GenerateKey(bool encrypt, byte[] key)
        {
            // Key length in words.
            int keyLen = key.Length / 4;

            Rounds = keyLen + 6;
            // 4 words make one block.
            uint[,] W = new uint[Rounds + 1, 4];
            // Copy the key into the round key array.
            int t = 0;
            for (int i = 0; i < key.Length; t++)
            {
                W[t >> 2, t & 3] = ToUInt32(key, i);
                i += 4;
            }
            // while not enough round key material calculated calculate new values.
            int k = (Rounds + 1) << 2;
            for (int i = keyLen; (i < k); i++)
            {
                uint temp = W[(i - 1) >> 2, (i - 1) & 3];
                if ((i % keyLen) == 0)
                {
                    temp = SubWord(Shift(temp, 8)) ^ rcon[(i / keyLen) - 1];
                }
                else if ((keyLen > 6) && ((i % keyLen) == 4))
                {
                    temp = SubWord(temp);
                }

                W[i >> 2, i & 3] = W[(i - keyLen) >> 2, (i - keyLen) & 3] ^ temp;
            }

            if (!encrypt)
            {
                for (int j = 1; j < Rounds; j++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        W[j, i] = ImixCol(W[j, i]);
                    }
                }
            }

            return W;
        }
    }
}