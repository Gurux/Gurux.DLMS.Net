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

namespace Gurux.DLMS
{    
    /// <summary>
    /// OBIS Code class is used to find out default descrition to OBIS Code.
    /// </summary>
    class GXStandardObisCode
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXStandardObisCode()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXStandardObisCode(string[] obis, string desc, string interfaces, string dataType)
        {
            this.OBIS = new string[6];
            if (obis != null)
            {
                Array.Copy(obis, this.OBIS, 6);
            }
            this.Description = desc;
            this.Interfaces = interfaces;
            DataType = dataType;
        }

        /// <summary>
        /// OBIS code.
        /// </summary>
        public string[] OBIS
        {
            get;
            set;
        }

        /// <summary>
        /// OBIS code description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Interfaces that are using this OBIS code.
        /// </summary>
        public string Interfaces
        {
            get;
            set;
        }

        /// <summary>
        /// Standard data types.
        /// </summary>
        public string DataType
        {
            get;
            set;
        }

        /// <summary>
        /// Standard data types.
        /// </summary>
        public string UIDataType
        {
            get;
            set;
        }

        /// <summary>
        /// Convert to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(".", OBIS) + " " + Description;
        }
    }
}
