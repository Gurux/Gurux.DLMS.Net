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
using Gurux.DLMS.Objects;

namespace Gurux.DLMS
{
    public class ConnectionEventArgs
    {
        /// <summary>
        /// Server ID that client try to use to make connection.
        /// </summary>
        public object ServerAddress     
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serverId"></param>
        internal ConnectionEventArgs(object serverId)
        {
            ServerAddress = serverId;
        }
    }

    public class ValueEventArgs
    {
        /// <summary>
        /// Target DLMS object.
        /// </summary>
        public GXDLMSObject Target
        {
            get;
            private set;
        }

        /// <summary>
        /// Attribute index of queried object.
        /// </summary>
        public int Index
        {
            get;
            private set;
        }

        /// <summary>
        /// object value
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Is request handled.
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }

        /// <summary>
        /// Parameterised access selector.
        /// </summary>
        public int Selector
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ValueEventArgs(GXDLMSObject target, int index, int selector)
        {
            Target = target;
            Index = index;
            Selector = selector;
        }
    }
}
