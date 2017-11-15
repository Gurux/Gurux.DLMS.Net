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
	/// An argument class for media state changed event.
	/// </summary>
    public class MediaStateEventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MediaStateEventArgs()
        {
            Accept = true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MediaStateEventArgs(MediaState state)
            : base()
        {
            State = state;
        }

        /// <summary>
        /// Status information from media state.
        /// </summary>
        public MediaState State
        {
            get;
            set;
        }

        /// <summary>
        /// True is returned if media state change succeeded.
        /// </summary>
        public bool Accept
        {
            get;
            set;
        }
    }    
}
