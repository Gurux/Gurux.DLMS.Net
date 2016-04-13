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
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMSSNSettings contains commands for retrieving and setting the Short Name 
    /// settings of the server, shortly said SN referencing support.
    /// </summary>
    public class GXDLMSSNSettings
    {
        /// <summary>
        /// Settings.
        /// </summary>
        internal byte[] ConformanceBlock;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSNSettings()
        {
            ConformanceBlock = new byte[3];
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSNSettings(byte[] conformanceBlock)
        {
            SetConformanceBlock(conformanceBlock);
        }

        /// <summary>
        /// User can initialize own conformance block.
        /// </summary>
        /// <param name="conformanceBlock"></param> 
        public void SetConformanceBlock(byte[] conformanceBlock)
        {
            if (conformanceBlock == null || conformanceBlock.Length != 3)
            {
                throw new ArgumentException("Invalid conformance block.");
            }
            ConformanceBlock = conformanceBlock;
        }

        /// <summary>
        /// Get conformance block bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] GetConformanceBlock()
        {
            return ConformanceBlock;
        }

        public void CopyTo(GXDLMSSNSettings target)
        {
            target.ConformanceBlock = this.ConformanceBlock;
        }

        /// <summary>
        /// Clear all bits.
        /// </summary>
        public void Clear()
        {
            ConformanceBlock[0] = 0;
            ConformanceBlock[1] = 0;
            ConformanceBlock[2] = 0;
        }

        /// <summary>
        /// Checks, whether data can be read from the server.
        /// </summary>
        public bool Read
        {
            get
            {
                return GXCommon.GetBits(ConformanceBlock[0], 0x10);
            }
            set
            {
                GXCommon.SetBits(ref ConformanceBlock[0], 0x10, value);
            }
        }

        /// <summary>
        /// Checks, whether data can be written to the server.
        /// </summary>
        public bool Write
        {
            get
            {
                return GXCommon.GetBits(ConformanceBlock[0], 0x8);
            }
            set
            {
                GXCommon.SetBits(ref ConformanceBlock[0], 0x8, value);
            }
        }
        
        public bool UnconfirmedWrite
        {
            get
            {
                return GXCommon.GetBits(ConformanceBlock[0], 0x4);
            }
            set
            {
                GXCommon.SetBits(ref ConformanceBlock[0], 0x4, value);
            }
        }

        public bool InformationReport
        {
            get
            {
                return GXCommon.GetBits(ConformanceBlock[1], 0x1);
            }
            set
            {
                GXCommon.SetBits(ref ConformanceBlock[1], 0x1, value);
            }
        }

        /// <summary>
        /// Is Logical Name referencing also supported.
        /// </summary>
        public bool MultipleReferences
        {
            get
            {
                return GXCommon.GetBits(ConformanceBlock[1], 0x2);
            }
            set
            {
                GXCommon.SetBits(ref ConformanceBlock[1], 0x2, value);
            }
        }

        /// <summary>
        /// Is parameterize access used.
        /// </summary>
        /// <remarks>
        /// Example Profile generic uses parameterize access if data is read using range or entry.
        /// With Logical Name referencing this is called SelectiveAccess.
        /// </remarks>
        /// <seealso cref="GXDLMSLNSettings.SelectiveAccess"/>
        public bool ParameterizedAccess
        {
            get
            {
                return GXCommon.GetBits(ConformanceBlock[2], 0x20);
            }
            set
            {
                GXCommon.SetBits(ref ConformanceBlock[2], 0x20, value);
            }
        }
    }
}
