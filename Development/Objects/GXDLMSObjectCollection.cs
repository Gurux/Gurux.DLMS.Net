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

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Collection of DLMS objects.
    /// </summary>
    public class GXDLMSObjectCollection : List<GXDLMSObject>, IList<GXDLMSObject>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSObjectCollection()
        {
            return;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent"></param>
        public GXDLMSObjectCollection(Object parent)
        {
            this.Parent = parent;
        }

        public object Parent
        {
            get;
            set;
        }

        public GXDLMSObjectCollection GetObjects(ObjectType type)
        {
            GXDLMSObjectCollection items = new GXDLMSObjectCollection();
            foreach (GXDLMSObject it in this)
            {
                if (it.ObjectType == type)
                {
                    items.Add(it);
                }
            }
            return items;
        }

        public GXDLMSObjectCollection GetObjects(ObjectType[] types)
        {
            GXDLMSObjectCollection items = new GXDLMSObjectCollection();
            foreach (GXDLMSObject it in this)
            {
                if (types.Contains(it.ObjectType))
                {
                    items.Add(it);
                }
            }
            return items;
        }

        public GXDLMSObject FindByLN(Gurux.DLMS.ObjectType type, string ln)
        {
            foreach (GXDLMSObject it in this)
            {
                if ((type == Gurux.DLMS.ObjectType.All || it.ObjectType == type) && it.LogicalName.Trim() == ln)
                {
                    return it;
                }
            }
            return null;
        }

        public GXDLMSObject FindBySN(ushort sn)
        {
            foreach (GXDLMSObject it in this)
            {
                if (it.ShortName == sn)
                {
                    return it;
                }
            }
            return null;
        }
       
        #region ICollection<GXDLMSObject> Members

        public void Add(GXDLMSObject item)
        {
            base.Add(item);
            if (this.Parent != null && item.Parent == null)
            {
                item.Parent = this;
            }
        }
        #endregion
       
    }
}
