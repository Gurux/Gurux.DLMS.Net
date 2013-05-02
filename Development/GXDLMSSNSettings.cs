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
        internal byte[] m_ConformanceBlock;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal GXDLMSSNSettings(byte[] conformanceBlock)
        {
            m_ConformanceBlock = conformanceBlock;
        }

        public void CopyTo(GXDLMSSNSettings target)
        {
            target.m_ConformanceBlock = this.m_ConformanceBlock;
        }

        /// <summary>
        /// Checks, whether data can be read from the server.
        /// </summary>
        public bool Read
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[0], 0x8);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[0], 0x8, value);
            }
        }

        /// <summary>
        /// Checks, whether data can be written to the server.
        /// </summary>
        public bool Write
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[0], 0x10);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[0], 0x10, value);
            }
        }
        
        public bool UnconfirmedWrite
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[0], 0x20);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[0], 0x20, value);
            }
        }

        public bool InformationReport
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[1], 0x80);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[1], 0x80, value);
            }
        }

        public bool MultipleReferences
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[1], 0x40);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[1], 0x40, value);
            }
        }

        public bool ParameterizedAccess
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[2], 0x4);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[2], 0x4, value);
            }
        }
    }
}
