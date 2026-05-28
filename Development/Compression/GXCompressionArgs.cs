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

using Gurux.DLMS.Compression.Enums;

namespace Gurux.DLMS.Compression
{
    public class GXCompressionArgs
    {
        /// <summary>
        /// Gets or sets the compression operation to perform.
        /// </summary>
        /// <remarks>
        /// Compression algorithms typically produce compressed data that is smaller in size than the original input data.
        /// Setting this property to Compress indicates that the OutputData contains compressed data, 
        /// while setting it to Decompress indicates that the OutputData contains uncompressed data.
        /// </remarks>
        public CompressionOperation Operation { get; internal set; }

        /// <summary>
        /// Gets or sets the compression options for the V.44 compression.
        /// </summary>
        public GXCompressionOptions Options { get; internal set; }

        /// <summary>
        /// Gets the input data that is being compressed or decompressed.
        /// </summary>
        public byte[] InputData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the output data produced by the compression or decompression operation.
        /// </summary>
        public byte[] OutputData
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new instance of the GXCompressionArgs class with the specified operation, options, and input data.
        /// </summary>
        /// <param name="operation">The compression operation to perform.</param>
        /// <param name="options">The compression options for the V.44 compression.</param>
        /// <param name="inputData">The input data that is being compressed or decompressed.</param>
        public GXCompressionArgs(CompressionOperation operation, GXCompressionOptions options, byte[] inputData)
        {
            Operation = operation;
            Options = options;
            InputData = inputData;
        }
    }
}