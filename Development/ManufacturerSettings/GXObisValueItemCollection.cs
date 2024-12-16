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
// More information of Gurux products: https://www.gurux.org
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Gurux.DLMS.ManufacturerSettings
{
#if !WINDOWS_UWP
    [Serializable]
#endif
    public class GXObisValueItemCollection : List<GXObisValueItem>, IList<GXObisValueItem>
    {
        #region IList<GXObisValueItem> Members

        /// <summary>
        /// Inserts new GXObisValueItem item to the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        new public void Insert(int index, GXObisValueItem item)
        {
            if (!this.Contains(item))
            {
                base.Insert(index, item);
            }
        }

        #endregion

        #region ICollection<GXObisValueItem> Members

        /// <summary>
        /// Add new GXObisValueItem item to the collection.
        /// </summary>
        /// <param name="item"></param>
        public new void Add(GXObisValueItem item)
        {
            if (!this.Contains(item))
            {
                base.Add(item);
            }
        }

        /// <summary>
        /// Determines whether an GXObisValueItem is in the collection.
        /// </summary>
        /// <param name="item">The GXObisValueItem to locate in the collection.</param>
        /// <returns></returns>
        new public bool Contains(GXObisValueItem item)
        {
            foreach (GXObisValueItem it in this)
            {
                if (it.Value == item.Value)
                {
                    return true;
                }
            }
            return false;
        }


        #endregion
    }
}
