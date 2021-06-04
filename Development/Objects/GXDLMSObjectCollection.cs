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
using System.Linq;
using System.Text;
using System.Collections;
using Gurux.DLMS.Enums;
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
            if (Items[index].Parent == this)
            {
                Items[index].Parent = null;
            }
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

        /// <summary>
        /// Gets or sets the object that contains data about the control.
        /// </summary>
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

        public GXDLMSObject FindByLN(ObjectType type, byte[] ln)
        {
            string name = GXCommon.ToLogicalName(ln);
            foreach (GXDLMSObject it in this)
            {
                if ((type == ObjectType.None || it.ObjectType == type) && it.LogicalName.Trim() == name)
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
            if (item.Parent == this)
            {
                item.Parent = null;
            }
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
        ///  Load COSEM objects from the file.
        /// </summary>
        /// <param name="filename"> File path.</param>
        /// <returns>Collection of serialized COSEM objects.</returns>
        public static GXDLMSObjectCollection Load(string filename)
        {
            using (Stream stream = File.OpenRead(filename))
            {
                return Load(stream);
            }
        }

        /// <summary>
        ///  Load COSEM objects from the file.
        /// </summary>
        /// <param name="filename"> File path.</param>
        /// <returns>Collection of serialized COSEM objects.</returns>
        public static void Load(string filename, GXDLMSObjectCollection objects)
        {
            using (Stream stream = File.OpenRead(filename))
            {
                Load(stream, objects);
            }
        }

        /// <summary>
        ///  Load COSEM objects from the file.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>Collection of serialized COSEM objects.</returns>
        public static GXDLMSObjectCollection Load(Stream stream)
        {
            GXDLMSObjectCollection objects = new GXDLMSObjectCollection();
            Load(stream, objects);
            return objects;
        }

        /// <summary>
        ///  Load COSEM objects from the file.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="objects">Collection of COSEM objects.</param>
        public static void Load(Stream stream, GXDLMSObjectCollection objects)
        {
            GXDLMSObject obj = null;
            String target;
            ObjectType type;
            using (GXXmlReader reader = new GXXmlReader(stream))
            {
                reader.Objects = objects;
                while (!reader.EOF)
                {
                    if (reader.IsStartElement())
                    {
                        target = reader.Name;
                        if (string.Compare("Objects", target, true) == 0)
                        {
                            //Skip.
                            reader.Read();
                        }
                        else if (target.StartsWith("GXDLMS"))
                        {
                            string str = target.Substring(6);
                            reader.Read();
                            type = (ObjectType)Enum.Parse(typeof(ObjectType), str);
                            obj = GXDLMSClient.CreateObject(type);
                            obj.Version = 0;
                        }
                        else if (string.Compare("Object", target, true) == 0)
                        {
                            int r = 0;
                            string str = reader.GetAttribute(0);
                            if (int.TryParse(str, out r))
                            {
                                type = (ObjectType)r;
                            }
                            else
                            {
                                type = (ObjectType)Enum.Parse(typeof(ObjectType), str);
                            }
                            reader.Read();
                            obj = GXDLMSClient.CreateObject(type);
                            obj.Version = 0;
                            reader.Objects.Add(obj);
                        }
                        else if (string.Compare("SN", target, true) == 0)
                        {
                            obj.ShortName = (UInt16)reader.ReadElementContentAsInt("SN");
                        }
                        else if (string.Compare("LN", target, true) == 0)
                        {
                            obj.LogicalName = reader.ReadElementContentAsString("LN");
                            GXDLMSObject tmp = reader.Objects.FindByLN(obj.ObjectType, obj.LogicalName);
                            if (tmp == null)
                            {
                                reader.Objects.Add(obj);
                            }
                            else
                            {
                                obj = tmp;
                            }
                        }
                        else if (string.Compare("Description", target, true) == 0)
                        {
                            obj.Description = reader.ReadElementContentAsString("Description");
                        }
                        else if (string.Compare("Version", target, true) == 0)
                        {
                            obj.Version = (UInt16)reader.ReadElementContentAsInt("Version");
                        }
                        else if (string.Compare("Access", target, true) == 0)
                        {
                            int pos = 0;
                            foreach (byte it in reader.ReadElementContentAsString("Access"))
                            {
                                ++pos;
                                obj.SetAccess(pos, (AccessMode)(it - 0x30));
                            }
                        }
                        else if (string.Compare("MethodAccess", target, true) == 0)
                        {
                            int pos = 0;
                            foreach (byte it in reader.ReadElementContentAsString("MethodAccess"))
                            {
                                ++pos;
                                obj.SetMethodAccess(pos, (MethodAccessMode)(it - 0x30));
                            }
                        }
                        else
                        {
                            (obj as IGXDLMSBase).Load(reader);
                            obj = null;
                        }
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                foreach (GXDLMSObject it in reader.Objects)
                {
                    (it as IGXDLMSBase).PostLoad(reader);
                }
            }
        }

        /// <summary>
        /// Save COSEM objects to the file.
        /// </summary>
        /// <param name="filename">File name.</param>
        /// <param name="settings">XML write settings.</param>
        public void Save(string filename, GXXmlWriterSettings settings)
        {
            bool ignoreDescription = false;
            if (settings != null)
            {
                ignoreDescription = settings.IgnoreDescription;
            }
            using (Stream stream = File.Create(filename))
            {
                Save(stream, settings);
            }
        }

        /// <summary>
        /// Save COSEM objects to the file.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="settings">XML write settings.</param>
        public void Save(Stream stream, GXXmlWriterSettings settings)
        {
            bool omitXmlDeclaration = false;
            bool ignoreDescription = false;
            if (settings != null)
            {
                ignoreDescription = settings.IgnoreDescription;
                omitXmlDeclaration = settings.OmitXmlDeclaration;
            }
            int index = 0;
            if (settings != null)
            {
                index = settings.Index;
            }
            using (GXXmlWriter writer = new GXXmlWriter(stream, settings))
            {
                if (!ignoreDescription)
                {
                    writer.WriteStartDocument();
                }
                writer.WriteStartElement("Objects", 0);
                foreach (GXDLMSObject it in this)
                {
                    if (it is IGXDLMSBase)
                    {
                        if (index < 2)
                        {
                            if (settings == null || !settings.Old)
                            {
                                writer.WriteStartElement("GXDLMS" + it.ObjectType.ToString(), 0);
                            }
                            else
                            {
                                writer.WriteStartElement("Object", 0);
                                writer.WriteAttributeString("Type", ((int)it.ObjectType).ToString(), 0);
                            }
                        }
                        if (index == 0)
                        {
                            // Add SN
                            if (it.ShortName != 0)
                            {
                                writer.WriteElementString("SN", it.ShortName, 0);
                            }
                            // Add LN
                            writer.WriteElementString("LN", it.LogicalName, 0);
                            // Add Version
                            if (it.Version != 0)
                            {
                                writer.WriteElementString("Version", it.Version, 0);
                            }
                            // Add description if given.
                            if (!ignoreDescription && !string.IsNullOrEmpty(it.Description))
                            {
                                writer.WriteElementString("Description", it.Description, 0);
                            }
                            //Add access rights.
                            StringBuilder sb = new StringBuilder();
                            for (int pos = 1; pos != (it as IGXDLMSBase).GetAttributeCount() + 1; ++pos)
                            {
                                sb.Append(((int)it.GetAccess(pos)).ToString());
                            }
                            writer.WriteElementString("Access", sb.ToString(), 0);
                            sb.Length = 0;
                            for (int pos = 1; pos != (it as IGXDLMSBase).GetMethodCount() + 1; ++pos)
                            {
                                sb.Append(((int)it.GetMethodAccess(pos)).ToString());
                            }
                            writer.WriteElementString("MethodAccess", sb.ToString(), 0);
                        }
                        if (settings == null || settings.Values)
                        {
                            (it as IGXDLMSBase).Save(writer);
                        }
                        if (index < 2)
                        {
                            // Close object.
                            writer.WriteEndElement();
                        }
                    }
                }
                writer.WriteEndElement();
                if (!ignoreDescription)
                {
                    writer.WriteEndDocument();
                }
            }
        }
    }
}
