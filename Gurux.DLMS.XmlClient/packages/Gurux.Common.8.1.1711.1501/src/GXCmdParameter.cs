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
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Gurux.Common;

namespace Gurux.Common
{
    /// <summary>
    /// This class is used to save command line parameters.
    /// </summary>
    public class GXCmdParameter
    {
        /// <summary>
        /// Command line parameter tag
        /// </summary>
        public char Tag
        {
            get;
            internal set;
        }

        /// <summary>
        /// Command line parameter value.
        /// </summary>
        public string Value
        {
            get;
            internal set;
        }
        /// <summary>
        /// Parameter is missing.
        /// </summary>
        public bool Missing
        {
            get;
            internal set;
        }

        /// <summary>
        /// Command line parameter as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Missing)
            {
                return Tag + " is missing.";
            }
            if (Value == null)
            {
                return Tag.ToString();
            }
            return Tag + "=" + Value;
        }
    }
}
