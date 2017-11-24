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

namespace Gurux.Common
{
	/// <summary>
	/// Argument class for IGXMedia data received events.
	/// </summary>
    public class ReceiveEventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ReceiveEventArgs()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ReceiveEventArgs(object data, string senderInfo)
        {
            Data = data;
            SenderInfo = senderInfo;
        }

        /// <summary>
        /// Pointer to the byte buffer containing the data, 
        /// which is received from the device.
        /// </summary>
        public object Data
        {
            get;
            set;
        }
        /// <summary>
        /// Media depend sender information.
        /// </summary>
        public string SenderInfo
        {
            get;
            set;
        }
    }
}
