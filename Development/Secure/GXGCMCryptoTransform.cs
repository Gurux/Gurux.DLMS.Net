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
using System.Security.Cryptography;

namespace Gurux.DLMS.Secure
{
    /// <summary>
    /// This class implements Galois/Counter Mode (GCM) crypto transform.
    /// </summary>
    internal class GXGCMCryptoTransform : ICryptoTransform
    {
        internal readonly byte[] _nonceAndCounter;
        private readonly ICryptoTransform _counterEncryptor;
        private readonly SymmetricAlgorithm _symmetricAlgorithm;

        /// <summary>
        /// Block counter is used when data is not fit to one symmetric algorithm block.
        /// </summary>
        private UInt32 _algorithmInitialblockCounter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symmetricAlgorithm">symmetric algorithm</param>
        /// <param name="cipherKey">Block cipher key.</param>
        /// <param name="nonce">Nonse</param>
        /// <param name="counter">Block counter value.</param>
        /// <param name="algorithmInitialblockCounter">Initial symmetric algorithm block counter value.</param>
        public GXGCMCryptoTransform(
            SymmetricAlgorithm symmetricAlgorithm,
            byte[] cipherKey, byte[] nonce,
            UInt32 counter,
            UInt32 algorithmInitialblockCounter)
        {
            if (cipherKey == null)
            {
                throw new ArgumentNullException(nameof(cipherKey));
            }
            if (symmetricAlgorithm == null)
            {
                throw new ArgumentNullException(nameof(symmetricAlgorithm));
            }
            _algorithmInitialblockCounter = algorithmInitialblockCounter;
            _symmetricAlgorithm = symmetricAlgorithm;
            _nonceAndCounter = new byte[16];
            GXByteBuffer bb = new GXByteBuffer();
            bb.Set(nonce);
            bb.SetUInt32(counter);
            bb.SetUInt32(_algorithmInitialblockCounter);
            bb.Get(_nonceAndCounter);
            var zeroIv = new byte[_symmetricAlgorithm.BlockSize / 8];
            _counterEncryptor = symmetricAlgorithm.CreateEncryptor(cipherKey, zeroIv);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var output = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
            return output;
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer,
            int outputOffset)
        {
            byte[] counterModeBlock = new byte[_symmetricAlgorithm.BlockSize / 8];
            for (int pos = 0; pos < inputCount; pos += 16)
            {
                _counterEncryptor.TransformBlock(_nonceAndCounter, 0, _nonceAndCounter.Length, counterModeBlock, 0);
                IncreaseBlockCounter();
                for (int pos2 = 0; pos2 != Math.Min(16, inputCount - pos); ++pos2)
                {
                    outputBuffer[inputOffset] = (byte)(inputBuffer[inputOffset] ^ counterModeBlock[pos2]);
                    ++inputOffset;
                }
            }
            return inputCount;
        }

        /// <summary>
        /// Increase the block counter.
        /// </summary>
        private void IncreaseBlockCounter()
        {
            ++_algorithmInitialblockCounter;
            GXByteBuffer bb = new GXByteBuffer();
            bb.Capacity = 16;
            bb.Set(_nonceAndCounter);
            bb.SetUInt32(12, _algorithmInitialblockCounter);
            bb.Get(_nonceAndCounter);
        }

        /// <inheritdoc/>
        public int InputBlockSize
        {
            get
            {
                return _symmetricAlgorithm.BlockSize / 8;
            }
        }

        /// <inheritdoc/>
        public int OutputBlockSize
        {
            get
            {
                return _symmetricAlgorithm.BlockSize / 8;
            }
        }
        /// <inheritdoc/>
        public bool CanTransformMultipleBlocks => true;
        /// <inheritdoc/>
        public bool CanReuseTransform => false;

        public void Dispose()
        {
            if (_counterEncryptor != null)
            {
                _counterEncryptor.Dispose();
            }
        }
    }
}
