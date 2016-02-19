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

using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{
    /// <summary>
    /// This class is used in DLMS data parsing. 
    /// </summary>
    class GXDataInfo
    {
        ///<summary>
        /// Last array index. 
        ///</summary>        
        public int Index
        {
            get;
            internal set;
        }

        ///<summary>
        /// Items count in array.
        ///</summary>
        public int Count
        {
            get;
            internal set;
        }

        ///<summary>
        /// Object data type. 
        ///</summary>
        public DataType Type
        {
            get;
            internal set;
        }

        ///<summary>
        ///Is data parsed to the end.
        ///</summary>
        public bool Compleate
        {
            get;
            internal set;
        }

        /// <summary>
        /// Clear settings.
        /// </summary>
        public virtual void Clear()
        {
            Index = 0;
            Count = 0;
            Type = DataType.None;
            Compleate = true;
        }
    }
}
