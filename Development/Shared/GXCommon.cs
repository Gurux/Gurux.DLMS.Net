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
using System.Runtime.InteropServices;

namespace Gurux.Shared
{
	class GXCommon
	{	
        /// <summary>
        /// Searches for the specified pattern and returns the index of the first occurrence
        /// within the range of elements in the byte buffer that starts at the specified
        /// index and contains the specified number of elements.
        /// </summary>
        /// <param name="input">Input byte buffer</param>
        /// <param name="pattern"></param>
        /// <param name="index">Index where search is started.</param>
        /// <param name="count">Maximum search buffer size.</param>
        /// <returns></returns>
		public static int IndexOf(byte[] input, byte[] pattern, int index, int count)
		{
			//If not enough data available.
            if (count < pattern.Length)
			{
				return -1;
			}
			byte firstByte = pattern[0];
			int pos = -1;
            if ((pos = Array.IndexOf(input, firstByte, index, count - index)) >= 0)
			{
				for (int i = 0; i < pattern.Length; i++)
				{
					if (pos + i >= input.Length || pattern[i] != input[pos + i])
					{
						return -1;
					}
				}
			}
			return pos;
		}
	}
}
