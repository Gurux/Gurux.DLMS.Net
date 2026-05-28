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
using System;
using System.ComponentModel;

namespace Gurux.DLMS.Compression
{
    public class GXCompressionOptions
    {
        /// <summary>
        /// Default codeword size.
        /// </summary>
        private int _defaultCodewordSize = 6;
        /// <summary>
        /// Default codeword size.
        /// </summary>
        private int _defaultOrdinalSize = 7;

        /// <summary>
        /// Is V.44 compression enabled.
        /// </summary>
        public bool EnableCompression { get; set; }

        /// <summary>
        /// Default codeword size.
        /// </summary>
        /// <remarks>
        /// In standard : C2
        /// </remarks>
        [DefaultValue(6)]
        public int DefaultCodewordSize
        {
            get
            {
                return _defaultCodewordSize;
            }
            set
            {
                if (value < 6 || value > 120)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The minimum allowed codeword size is 6 bytes.");
                }
                _defaultCodewordSize = value;
            }
        }

        /// <summary>
        /// Default ordinal size.
        /// </summary>
        /// <remarks>
        /// In standard : C5
        /// </remarks>
        [DefaultValue(8)]
        public int DefaultOrdinalSize
        {
            get
            {
                return _defaultOrdinalSize;
            }
            set
            {
                if (value < 7 || value > 8)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The orginal size ranges from 7 to 8 bytes.");
                }
                _defaultOrdinalSize = value;
            }
        }

        /// <summary>
        /// Gets the maximum number of codewords that can be processed.
        /// </summary>
        public UInt16 MaxCodewords { get; set; } = 1024;   // N2
        /// <summary>
        ///  The maximum string length.
        /// </summary>
        /// <remarks>
        ///  N7T is count from this.
        ///  </remarks>
        public MaximumStringLength MaximumStringLength { get; set; } = MaximumStringLength.Value255;

        /// <summary>
        /// Max Dictionary size.
        /// </summary>
        /// <remarks>
        /// The dictionary is reset when the maximum dictionary size is reached.
        /// The dictionary size is equal to N2 (the total number of codewords).
        /// </remarks>
        [DefaultValue(3072)]
        public UInt32 MaxDictionarySize
        {
            get;
            set;
        } = 3072;

    }
}