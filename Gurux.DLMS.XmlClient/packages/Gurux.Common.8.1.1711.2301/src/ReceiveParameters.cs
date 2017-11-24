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
    /// ReceiveArgs class is used when data ir read synchronously.
    /// </summary>
    public class ReceiveParameters<T>
    {        
        /// <summary>
        /// Constructor.
        /// </summary>
        public ReceiveParameters()
        {
            Peek = false;
            WaitTime = -1;
        }
        /// <summary>
        /// If true, returns the bytes from the buffer without removing.
        /// </summary>
        public bool Peek
        {
            get;
            set;
        }

        /// <summary>
        /// The end of packet (EOP) waited for.
        /// </summary>
        /// <remarks>
        /// The EOP can, for example be a single byte ('0xA1'), 
        /// a string ("OK") or an array of bytes.        
        /// </remarks>
        public object Eop
        {
            get;
            set;
        }

        /// <summary>
        /// The number of reply data bytes to be read.
        /// </summary>
        /// <remarks>
        /// Count can be between 0 and n bytes.
        /// </remarks>
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum time, in milliseconds, to wait for reply data. 
        /// WaitTime -1 (Default value) indicates infinite wait time.
        /// </summary>
        public int WaitTime
        {
            get;
            set;
        }

        /// <summary>
        /// If True, all the reply data is moved to ReplyData.
        /// </summary>
        public bool AllData
        {
            get;
            set;
        }        

        /// <summary>
        /// Received reply data.
        /// </summary>
        public T Reply
        {
            get;
            set;
        }        
    }
}
