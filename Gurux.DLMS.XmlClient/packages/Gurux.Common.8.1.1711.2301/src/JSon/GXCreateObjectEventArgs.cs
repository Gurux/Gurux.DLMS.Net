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

namespace Gurux.Common.JSon
{
    /// <summary>
    /// JSON parser want's to create new class.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void CreateObjectEventhandler(object sender, GXCreateObjectEventArgs e);

    /// <summary>
    /// Event argument class that when JSON serializer need to create new object.
    /// </summary>
    public class GXCreateObjectEventArgs : EventArgs
    {
        /// <summary>
        /// Object type to create.
        /// </summary>
        public Type Type
        {
            get;
            private set;
        }

        /// <summary>
        /// JSON parameters.
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Extra types.
        /// </summary>
        public List<Type> ExtraTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// Created object.
        /// </summary>
        public object Object
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GXCreateObjectEventArgs(Type type, Dictionary<string, object> data, List<Type> extra)
        {
            ExtraTypes = extra;
            Type = type;
            Parameters = data;
        }
    }
}
