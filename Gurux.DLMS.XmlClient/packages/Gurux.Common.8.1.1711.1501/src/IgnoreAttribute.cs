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
//--------------------------------------------------------------------------

using System;

namespace Gurux.Common
{
    /// <summary>
    /// Ignore type enumeration.
    /// </summary>
    [Flags]
    public enum IgnoreType : int
    {
        /// <summary>
        /// Nothing is ingoner.
        /// </summary>
        None = 0,
        /// <summary>
        /// DB is ignored.
        /// </summary>
        Db = 1,
        /// <summary>
        /// JSON formatter is ignored.
        /// </summary>
        Json = 2,
        /// <summary>
        /// This is ignored always.
        /// </summary>
        All = -1
    }

    /// <summary>
    /// Properties with this IgnoreAttribute are ignored when building sql sentences 
    /// and they are not saved to DB or JSON data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreAttribute : Attribute
    {
        /// <summary>
        /// Ignore types.
        /// </summary>
        public IgnoreType IgnoreType
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public IgnoreAttribute()
        {
            IgnoreType = IgnoreType.All;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public IgnoreAttribute(IgnoreType ignore)
        {
            IgnoreType = ignore;
        }
    }
}
