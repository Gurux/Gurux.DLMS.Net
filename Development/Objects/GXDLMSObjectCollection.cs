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
using System.ComponentModel;

using System.Collections;
using Gurux.DLMS.Enums;
using System.Xml;
using System.IO;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Collection of DLMS objects.
    /// </summary>
    public class GXDLMSObjectCollection : IList<GXDLMSObject>,
                            ICollection<GXDLMSObject>, IEnumerable<GXDLMSObject>
    {
        private List<GXDLMSObject> Objects;

        #region Protected Properties
        /// <summary>
        /// Returns the list of items in the class.
        /// </summary>
        protected List<GXDLMSObject> Items
        {
            get { return this.Objects; }
            private set { this.Objects = value; }
        }
        #endregion


        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSObjectCollection()
            : this(0)
        {
            //Nothing to do
        }

        /// <summary>
        /// Constructor.
        /// </summary>        
        public GXDLMSObjectCollection(int capacity)
        {
            this.Items = new List<GXDLMSObject>(capacity);
        }

        #region IList Methods
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public GXDLMSObject this[int index]
        {
            get { return this.Items[index]; }
            set { this.Items[index] = value; }
        }

        /// <summary>
        /// Inserts an item to the System.Collections.Generic.IList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the System.Collections.Generic.IList.</param>
        public void Insert(int index, GXDLMSObject item)
        {
            this.Items.Insert(index, item);
        }

        /// <summary>
        /// Removes the System.Collections.Generic.IList item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            this.Items.RemoveAt(index);
        }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent"></param>
        public GXDLMSObjectCollection(Object parent)
            : this(0)
        {
            this.Parent = parent;
        }

        public object Parent
        {
            get;
            internal set;
        }

        public object Tag
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

        public GXDLMSObject FindByLN(ObjectType type, string ln)
        {
            foreach (GXDLMSObject it in this)
            {
                if ((type == ObjectType.None || it.ObjectType == type) && it.LogicalName.Trim() == ln)
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

        public void AddRange(IEnumerable<GXDLMSObject> collection)
        {
            foreach (GXDLMSObject it in collection)
            {
                Add(it);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            foreach (GXDLMSObject it in this)
            {
                if (sb.Length != 1)
                {
                    sb.Append(", ");
                }
                sb.Append(it.Name.ToString());
            }
            sb.Append(']');
            return sb.ToString();
        }

        #region IList<GXDLMSObject> Members

        /// <summary>
        /// Determines the index of a specific item in the System.Collections.Generic.IList.
        /// </summary>
        /// <param name="item">The object to locate in the System.Collections.Generic.IList.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(GXDLMSObject item)
        {
            return Objects.IndexOf(item);
        }

        #endregion

        #region ICollection<GXDLMSObject> Members

        public void Add(GXDLMSObject item)
        {
            Objects.Add(item);
            if (item.Parent == null)
            {
                item.Parent = this;
            }
        }

        public void Clear()
        {
            Objects.Clear();
        }

        public bool Contains(GXDLMSObject item)
        {
            return Objects.Contains(item);
        }

        public void CopyTo(GXDLMSObject[] array, int arrayIndex)
        {
            Objects.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return Objects.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(GXDLMSObject item)
        {
            return Objects.Remove(item);
        }

        #endregion

        #region IEnumerable<GXDLMSObject> Members

        public IEnumerator<GXDLMSObject> GetEnumerator()
        {
            return Objects.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Objects.GetEnumerator();
        }

        #endregion

        #region ICollection<GXDLMSObject> Members


        void ICollection<GXDLMSObject>.Clear()
        {
            Objects.Clear();
        }

        bool ICollection<GXDLMSObject>.Contains(GXDLMSObject item)
        {
            return Objects.Contains(item);
        }

        #endregion

        /// <summary>
        /// Save COSEM objects to the file.
        /// </summary>
        /// <param name="filename">File path.</param>
        /// <param name="values">Are attribute values also serialized.</param>
        public void Save(string filename, bool values)
        {
            using (GXXmlWriter writer = new GXXmlWriter(filename))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Objects");
                foreach (GXDLMSObject it in this)
                {
                    writer.WriteStartElement("Object");
                    writer.WriteAttributeString("Type", it.ObjectType.ToString());
                    // Add SN
                    if (it.ShortName != 0)
                    {
                        writer.WriteElementString("SN", it.ShortName.ToString());
                    }
                    // Add LN
                    writer.WriteElementString("LN", it.LogicalName);
                    // Add description if given.
                    if (!string.IsNullOrEmpty(it.Description))
                    {
                        writer.WriteElementString("Description", it.Description);
                    }
                    if (values)
                    {
                        (it as IGXDLMSBase).Save(writer);
                    }
                    // Close object.
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
