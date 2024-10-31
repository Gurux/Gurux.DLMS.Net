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

using Gurux.DLMS.Internal;
using System;
using System.Text;

namespace Gurux.DLMS.ASN
{

    /// <summary>ASN1 bit string </summary>
    public class GXAsn1BitString
    {
        /// <summary>Number of extra bits at the end of the string. </summary>
        private int padBits;

        /// <summary>Number of extra bits at the end of the string. </summary>
        public int PadBits
        {
            get
            {
                return padBits;
            }
            private set
            {
                if (value < 0)
                {
                    throw new System.ArgumentException(nameof(PadBits));
                }
                padBits = value;
            }
        }

        /// <summary>Bit string. </summary>
        public byte[] Value
        {
            get;
            private set;
        }

        /// <summary>Constructor. </summary>
        public GXAsn1BitString()
        {

        }

        /// <summary>
        /// Append zeroes to the buffer.
        /// </summary>
        /// <param name="sb">Buffer where zeros are added.</param>
        /// <param name="count">Amount of zeroes.</param>
        private static void AppendZeros(StringBuilder sb, int count)
        {
            for (int pos = 0; pos != count; ++pos)
            {
                sb.Append('0');
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bitString">Bit string.</param>
        public GXAsn1BitString(string bitString)
        {
            PadBits = 8 - (bitString.Length % 8);
            if (PadBits == 8)
            {
                PadBits = 0;
            }
            StringBuilder sb = new StringBuilder(bitString);
            AppendZeros(sb, PadBits);
            Value = new byte[sb.Length / 8];
            for (int pos = 0; pos != Value.Length; ++pos)
            {
                Value[pos] = (byte)Convert.ToInt32(sb.ToString().Substring(8 * pos, 8 * (pos + 1) - (8 * pos)), 2);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Bit string.</param>
        /// <param name="padCount">Number of extra bits at the end of the string.</param>
        public GXAsn1BitString(byte[] value, int padCount)
        {
            if (value == null)
            {
                throw new ArgumentException(nameof(value));
            }
            if (PadBits < 0 || PadBits > 7)
            {
                throw new System.ArgumentException("PadCount must be in the range 0 to 7");
            }
            Value = value;
            PadBits = padCount;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Bit string.</param>
        public GXAsn1BitString(byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentException(nameof(value));
            }
            PadBits = value[0];
            if (PadBits < 0 || PadBits > 7)
            {
                throw new ArgumentException("PadCount must be in the range 0 to 7");
            }
            Value = new byte[value.Length - 1];
            Array.Copy(value, 1, Value, 0, value.Length - 1);
        }

        /// <summary>
        /// Number of extra bits at the end of the string.
        /// </summary>
        public int Length
        {
            get
            {
                if (Value == null)
                {
                    return 0;
                }
                return (8 * Value.Length) - PadBits;
            }
        }

        /// <inheritdoc/>
        public override sealed string ToString()
        {
            if (Value == null)
            {
                return "";
            }
            return Convert.ToString((8 * Value.Length) - PadBits) +
                " bit " + AsString();
        }

        public string AsString()
        {
            if (Value == null)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder(8 * Value.Length);
            foreach (byte it in Value)
            {
                GXCommon.ToBitString(sb, it, 8);
            }
            sb.Length = sb.Length - PadBits;
            return sb.ToString();
        }

        /// <summary>
        /// Converts ASN1 bit-string to integer value.
        /// </summary>
        /// <returns>The bit-string as an integer value.</returns>
        public int ToInteger()
        {
            UInt32 ret = 0;
            if (Value != null)
            {
                UInt16 bytePos = 0;
                foreach (byte it in Value)
                {
                    ret |= (UInt32)(GXCommon.SwapBits(it) << bytePos);
                    bytePos += 8;
                }
            }
            return (int)ret;
        }
    }
}