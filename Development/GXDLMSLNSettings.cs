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
    /// GXDLMSLNSettings contains commands for retrieving, and setting, the
    /// Logical Name settings of the server, shortly said LN referencing support.
    /// </summary>
    public class GXDLMSLNSettings
    {
        /// <summary>
        /// Settings.
        /// </summary>
        internal byte[] m_ConformanceBlock = new byte[3];

        /// <summary>
        /// Constructor.
        /// </summary>
        internal GXDLMSLNSettings(byte[] conformanceBlock)
        {
            //Set default values.
            m_ConformanceBlock = conformanceBlock;
        }

        public void CopyTo(GXDLMSLNSettings target)
        {
            target.m_ConformanceBlock = this.m_ConformanceBlock;
        }

        public bool Attribute0SetReferencing
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[1], 0x1);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[1], 0x1, value);
            }
        }

        public bool PriorityManagement
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[1], 0x2);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[1], 0x2, value);
            }
        }

        public bool Attribute0GetReferencing
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[1], 0x4);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[1], 0x4, value);
            }
        }

        /// <summary>
        /// Checks, if data from the server can be read in blocks.
        /// </summary>
        public bool GetBlockTransfer
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[1], 0x8);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[1], 0x8, value);
            }
        }

        /// <summary>
        /// Checks, if data to the server can be written in blocks.
        /// </summary>
        public bool SetBlockTransfer
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[1], 0x10);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[1], 0x10, value);
            }

        }

        public bool ActionBlockTransfer
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[1], 0x20);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[1], 0x20, value);
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


        /// <summary>
        /// Checks, if data can be read from the server.
        /// </summary>
        public bool Get
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[2], 0x8);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[2], 0x8, value);
            }

        }

        /// <summary>
        /// Checks, if data can be written to the server.
        /// </summary>
        public bool Set
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[2], 0x10);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[2], 0x10, value);
            }
        }

        public bool Action
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[2], 0x20);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[2], 0x20, value);
            }

        }

        public bool EventNotification
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[2], 0x40);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[2], 0x40, value);
            }

        }


        /// <summary>
        /// Is selective access used.
        /// </summary>
        public bool SelectiveAccess
        {
            get
            {
                return GXCommon.GetBits(m_ConformanceBlock[2], 0x80);
            }
            set
            {
                GXCommon.SetBits(m_ConformanceBlock[2], 0x80, value);
            }
        }
    }
}
