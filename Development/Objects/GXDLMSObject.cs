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
using System.ComponentModel;
using Gurux.DLMS.ManufacturerSettings;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Notifiest that user has change value.
    /// </summary>
    public delegate void ObjectChangeEventHandler(GXDLMSObject sender, bool Dirty, int attributeIndex, object value);

    /// <summary>
    /// GXDLMSObject provides an interface to DLMS registers.
    /// </summary>
    public class GXDLMSObject
    {
        /// <summary>
        /// Is attribute read.
        /// </summary>
        /// <param name="index">Attribute index to read.</param>
        /// <returns>Returns true if attribute is read.</returns>
        virtual public bool IsRead(int index)
        {
            if (!CanRead(index))
            {
                return true;
            }
            lock (Status)
            {
                //If value is changed or value is not read yet.
                return !(Status.ContainsKey(index) ||
                     GetLastReadTime(index) == DateTime.MinValue);
            }
        }

        /// <summary>
        /// Is attribute of the object readable.
        /// </summary>
        /// <param name="index">Attribute index of the object.</param>
        /// <returns>True, if attribute of the object is readable.</returns>
        public bool CanRead(int index)
        {
            //Association version number is not known
            //and for this reason all access levels must be checked.
            AccessMode access = GetAccess(index);
            return (access & AccessMode.Read) != 0 ||
                (access == AccessMode.AuthenticatedRead) ||
                (access == AccessMode.AuthenticatedReadWrite) ||
                (GetAccess3(index) & AccessMode3.Read) != 0;
        }

        class GXStatusInfo
        {
            /// <summary>
            /// Last read time.
            /// </summary>
            public DateTime Read
            {
                get;
                set;
            }

            /// <summary>
            /// Changed value.
            /// </summary>
            public object Value
            {
                get;
                set;
            }

            /// <summary>
            /// Last exception.
            /// </summary>
            public Exception Error
            {
                get;
                set;
            }
        }
        SortedDictionary<int, GXStatusInfo> Status = new SortedDictionary<int, GXStatusInfo>();

        public event ObjectChangeEventHandler OnChange;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSObject()
        : this(ObjectType.None, null, 0)
        {
        }

        /// <summary>
        /// Constructor,
        /// </summary>
        protected GXDLMSObject(ObjectType objectType)
        : this(objectType, null, 0)
        {
        }

        /// <summary>
        /// Validate logical name.
        /// </summary>
        /// <param name="ln"></param>
        public static void ValidateLogicalName(String ln)
        {
            string[] items = ln.Split('.');
            if (items.Length != 6)
            {
                throw new GXDLMSException("Invalid Logical Name.");
            }
        }

        /// <summary>
        /// Constructor,
        /// </summary>
        protected GXDLMSObject(ObjectType objectType, string ln, ushort sn)
        {
            Attributes = new Gurux.DLMS.ManufacturerSettings.GXAttributeCollection();
            MethodAttributes = new Gurux.DLMS.ManufacturerSettings.GXAttributeCollection();
            ObjectType = objectType;
            this.ShortName = sn;
            if (ln != null)
            {
                ValidateLogicalName(ln);
            }
            this.LogicalName = ln;
        }

#if !WINDOWS_UWP
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        [System.Xml.Serialization.XmlIgnore()]
        public Gurux.DLMS.Objects.GXDLMSObjectCollection Parent
        {
            get;
            internal set;
        }

        /// <summary>
        /// Creates and returns a copy of this object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        public GXDLMSObject Clone()
        {
            GXDLMSObjectCollection objs = new GXDLMSObjectCollection();
            objs.Add(this);
            using (Stream stream = new MemoryStream())
            {
                GXXmlWriterSettings settings = new GXXmlWriterSettings();
                objs.Save(stream, settings);
                stream.Seek(0, SeekOrigin.Begin);
                objs = GXDLMSObjectCollection.Load(stream);
                return objs[0];
            }
        }

        /// <summary>
        ///  Check are content of the objects equal.
        /// </summary>
        /// <param name="obj1">Object 1</param>
        /// <param name="obj2">Object 2</param>
        /// <returns> True, if content of the objects is equal.</returns>
        public static bool Equals(GXDLMSObject obj1, GXDLMSObject obj2)
        {
            List<Type> types = new List<Type>(GXDLMSClient.GetObjectTypes());
            types.Add(typeof(GXDLMSAttributeSettings));
            types.Add(typeof(GXDLMSAttribute));
            using (Stream stream = new MemoryStream())
            {
                string expected;
                XmlSerializer x = new XmlSerializer(obj1.GetType(), types.ToArray());
                x.Serialize(stream, obj1);
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(stream))
                {
                    expected = sr.ReadToEnd();
                }
                stream.Seek(0, SeekOrigin.Begin);
                x.Serialize(stream, obj2);
                using (StreamReader sr = new StreamReader(stream))
                {
                    string actual = sr.ReadToEnd();
                    return expected == actual;
                }
            }
        }

        /// <summary>
        /// Logical or Short Name of DLMS object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (ShortName != 0)
            {
                return ShortName.ToString() + " " + Description;
            }
            return LogicalName + " " + Description;
        }

        /// <summary>
        /// Interface type of the DLMS object.
        /// </summary>
#if !WINDOWS_UWP
        [ReadOnly(true)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        [System.Xml.Serialization.XmlIgnore()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ObjectType ObjectType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the object that contains data about the control.
        /// </summary>
#if !WINDOWS_UWP
        [ReadOnly(true)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        [System.Xml.Serialization.XmlIgnore()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// DLMS version number.
        /// </summary>
#if !WINDOWS_UWP
        [ReadOnly(true)]
#endif
        public virtual int Version
        {
            get;
            set;
        }

        /// <summary>
        /// The base name of the object, if using SN.
        /// </summary>
        /// <remarks>
        /// When using SN referencing, retrieves the base name of the DLMS object.
        /// When using LN referencing, the value is 0.
        /// </remarks>
#if !WINDOWS_UWP
        [ReadOnly(true)]
#endif
        [DefaultValue(0)]
        public ushort ShortName
        {
            get;
            set;
        }

        /// <summary>
        /// Logical or Short Name of DLMS object.
        /// </summary>
        /// <returns></returns>
        public object Name
        {
            get
            {
                if (ShortName != 0)
                {
                    return ShortName;
                }
                return LogicalName;
            }
        }

        /// <summary>
        /// Logical Name of DLMS object.
        /// </summary>
        public virtual string LogicalName
        {
            get;
            set;
        }

        /// <summary>
        /// Description of DLMS object.
        /// </summary>
        [DefaultValue(null)]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// object attribute collection.
        /// </summary>
#if !WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlArray("Attributes")]
        [XmlArrayItem("Item")]
        public GXAttributeCollection Attributes
        {
            get;
            set;
        }

        /// <summary>
        /// object attribute collection.
        /// </summary>
#if !WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlArray("Methods")]
        [XmlArrayItem("Item")]
        public GXAttributeCollection MethodAttributes
        {
            get;
            set;
        }

        /// <summary>
        /// Clear dirty flag from selected attribute.
        /// </summary>
        /// <param name="attributeIndex">Attribute index of the object.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ClearDirty(int attributeIndex)
        {
            if (attributeIndex == 0)
            {
                lock (Status)
                {
                    Status.Clear();
                }
            }
            else
            {
                lock (Status)
                {
                    if (Status.ContainsKey(attributeIndex))
                    {
                        Status[attributeIndex].Value = null;
                    }
                }
            }
            if (OnChange != null)
            {
                OnChange(this, false, attributeIndex, null);
            }
        }

        /// <summary>
        /// Clears status information.
        /// </summary>
        /// <param name="attributeIndex">Attribute index of the COSEM object.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ClearStatus(int attributeIndex)
        {
            if (attributeIndex == 0)
            {
                lock (Status)
                {
                    Status.Clear();
                }
            }
            else
            {
                lock (Status)
                {
                    Status.Remove(attributeIndex);
                }
            }
            if (OnChange != null)
            {
                OnChange(this, false, attributeIndex, null);
            }
        }

        /// <summary>
        /// Is value of selected attribute changed.
        /// </summary>
        /// <param name="attributeIndex">Attribute index of the object.</param>
        /// <param name="value"></param>
        /// <returns>Returns True if attribute is changed.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetDirty(int attributeIndex, out object value)
        {
            lock (Status)
            {
                if (Status.ContainsKey(attributeIndex))
                {
                    value = Status[attributeIndex].Value;
                    return value != null;
                }
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Returns collection of dirty attribute indexes.
        /// </summary>
        /// <returns>Collection of dirty attribute index.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int[] GetDirtyAttributeIndexes()
        {
            List<int> list = new List<int>();
            lock (Status)
            {
                foreach (var it in Status)
                {
                    if (it.Value != null && it.Value.Value != null)
                    {
                        list.Add(it.Key);
                    }
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Update dirty state of attribute index.
        /// </summary>
        /// <param name="attributeIndex"></param>
        /// <param name="value"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateDirty(int attributeIndex, object value)
        {
            lock (Status)
            {
                if (!Status.ContainsKey(attributeIndex))
                {
                    GXStatusInfo s = new GXStatusInfo();
                    s.Value = value;
                    Status.Add(attributeIndex, s);
                }
                else
                {
                    Status[attributeIndex].Value = value;
                }
            }
            if (OnChange != null)
            {
                OnChange(this, true, attributeIndex, value);
            }
        }

        /// <summary>
        /// Returns time when attribute was last time read.
        /// </summary>-
        /// <param name="attributeIndex">Attribute index.</param>
        /// <returns>Is attribute read only.</returns>
        public DateTime GetLastReadTime(int attributeIndex)
        {
            lock (Status)
            {
                if (!Status.ContainsKey(attributeIndex))
                {
                    return DateTime.MinValue;
                }
                return Status[attributeIndex].Read;
            }
        }

        /// <summary>
        /// Set time when attribute was last time read.
        /// </summary>
        /// <param name="attributeIndex"></param>
        /// <param name="tm"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetLastReadTime(int attributeIndex, DateTime tm)
        {
            lock (Status)
            {
                if (!Status.ContainsKey(attributeIndex))
                {
                    GXStatusInfo s = new GXStatusInfo();
                    s.Read = tm;
                    Status.Add(attributeIndex, s);
                }
                else
                {
                    Status[attributeIndex].Read = tm;
                }
            }
        }

        /// <summary>
        /// Returns last error.
        /// </summary>-
        /// <param name="attributeIndex">Attribute index.</param>
        /// <returns>Last exception.</returns>
        public Exception GetLastError(int attributeIndex)
        {
            lock (Status)
            {
                if (!Status.ContainsKey(attributeIndex))
                {
                    return null;
                }
                return Status[attributeIndex].Error;
            }
        }

        /// <summary>
        /// Returns last errors.
        /// </summary>-
        /// <returns>Last exception.</returns>
        public SortedDictionary<int, Exception> GetLastErrors()
        {
            SortedDictionary<int, Exception> errors = new SortedDictionary<int, Exception>();
            lock (Status)
            {
                foreach (var it in Status)
                {
                    if (it.Value.Error != null)
                    {
                        errors.Add(it.Key, it.Value.Error);
                    }
                }
            }
            return errors;
        }

        /// <summary>
        /// Set time when attribute was last time read.
        /// </summary>
        /// <param name="attributeIndex">>Attribute index.</param>
        /// <param name="error">Last Error.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetLastError(int attributeIndex, Exception error)
        {
            lock (Status)
            {
                if (!Status.ContainsKey(attributeIndex))
                {
                    GXStatusInfo s = new GXStatusInfo();
                    s.Error = error;
                    Status.Add(attributeIndex, s);
                }
                else
                {
                    Status[attributeIndex].Error = error;
                }
            }
        }

        /// <summary>
        /// Returns attribute access mode.
        /// </summary>-
        /// <param name="index">Attribute index.</param>
        /// <returns>Is attribute read only.</returns>
        public AccessMode GetAccess(int index)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, null);
            return att.Access;
        }

        /// <summary>
        /// Set attribute access.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="access"></param>
        public void SetAccess(int index, AccessMode access)
        {
            if (access > AccessMode.AuthenticatedReadWrite)
            {
                throw new ArgumentOutOfRangeException(nameof(access));
            }

            GXDLMSAttributeSettings att = GetAttribute(index, Attributes);
            att.Access = access;
            att.Access3 = AccessMode3.NoAccess;
        }

        /// <summary>
        /// Returns is attribute read only.
        /// </summary>-
        /// <param name="index">Attribute index.</param>
        /// <returns>Is attribute read only.</returns>
        public AccessMode3 GetAccess3(int index)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, null);
            return att.Access3;
        }

        /// <summary>
        /// Set attribute access.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="access"></param>
        public void SetAccess3(int index, AccessMode3 access)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, Attributes);
            att.Access3 = access;
            att.Access = AccessMode.NoAccess;
        }

        /// <summary>
        /// Returns is Method attribute read only.
        /// </summary>-
        /// <param name="index">Method Attribute index.</param>
        /// <returns>Is attribute read only.</returns>
        public MethodAccessMode GetMethodAccess(int index)
        {
            GXDLMSAttributeSettings att = MethodAttributes.Find(index);
            if (att != null)
            {
                return att.MethodAccess;
            }
            return MethodAccessMode.Access;
        }

        /// <summary>
        /// Set Method attribute access.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="access"></param>
        public void SetMethodAccess(int index, MethodAccessMode access)
        {
            if (access > MethodAccessMode.AuthenticatedAccess)
            {
                throw new ArgumentOutOfRangeException(nameof(access));
            }
            GXDLMSAttributeSettings att = MethodAttributes.Find(index);
            if (att == null)
            {
                att = new GXDLMSAttributeSettings(index);
                MethodAttributes.Add(att);
            }
            att.MethodAccess = access;
            att.MethodAccess3 = MethodAccessMode3.NoAccess;
        }

        /// <summary>
        /// Returns is Method attribute read only.
        /// </summary>-
        /// <param name="index">Method Attribute index.</param>
        /// <returns>Is attribute read only.</returns>
        public MethodAccessMode3 GetMethodAccess3(int index)
        {
            GXDLMSAttributeSettings att = MethodAttributes.Find(index);
            if (att != null)
            {
                return att.MethodAccess3;
            }
            return MethodAccessMode3.Access;
        }

        /// <summary>
        /// Set Method attribute access.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="access"></param>
        public void SetMethodAccess3(int index, MethodAccessMode3 access)
        {
            GXDLMSAttributeSettings att = MethodAttributes.Find(index);
            if (att == null)
            {
                att = new GXDLMSAttributeSettings(index);
                MethodAttributes.Add(att);
            }
            att.MethodAccess3 = access;
            att.MethodAccess = MethodAccessMode.NoAccess;
        }

        /// <summary>
        /// Returns device data type of selected attribute index.
        /// </summary>
        /// <param name="index">Attribute index of the object.</param>
        /// <returns>Device data type of the object.</returns>
        public virtual DataType GetDataType(int index)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, null);
            return att.Type;
        }

        /// <summary>
        /// Returns UI data type of selected index.
        /// </summary>
        /// <param name="index">Attribute index of the object.</param>
        /// <returns>UI data type of the object.</returns>
        public virtual DataType GetUIDataType(int index)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, null);
            return att.UIType;
        }

        /// <summary>
        /// Returns attributes as an array.
        /// </summary>
        /// <returns>Collection of COSEM object values.</returns>
        public virtual object[] GetValues()
        {
            throw new NotImplementedException("GetValues");
        }

        protected GXDLMSAttributeSettings GetAttribute(int index, GXAttributeCollection attributes)
        {
            GXDLMSAttributeSettings att = this.Attributes.Find(index);
            if (att == null)
            {
                att = new GXDLMSAttributeSettings(index);
                //LN is read only.
                if (index == 1)
                {
                    att.Access = AccessMode.Read;
                }
                if (attributes != null)
                {
                    attributes.Add(att);
                }
            }
            return att;
        }

        public void SetDataType(int index, DataType type)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, Attributes);
            att.Type = type;
        }

        public void SetUIDataType(int index, DataType type)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, Attributes);
            att.UIType = type;
        }

        public void SetStatic(int index, bool isStatic)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, Attributes);
            att.Static = isStatic;
        }

        /// <summary>
        /// Update enumeration values.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="values"></param>
        public void SetValues(int index, GXObisValueItemCollection values)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, Attributes);
            att.Values.Clear();
            att.Values.AddRange(values);
        }

        /// <summary>
        /// Update XML template.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="values"></param>
        public void SetXml(int index, string xml)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, Attributes);
            att.Xml = xml;
        }

        /// <summary>
        /// How this value is visualized on the UI.
        /// </summary>
        public void SetUIValueType(int index, ValueFieldType value)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, Attributes);
            att.UIValueType = value;
        }

        /// <summary>
        /// How this value is visualized on the UI.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ValueFieldType GetUIValueType(int index)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, null);
            return att.UIValueType;
        }

        public bool GetStatic(int index)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, null);
            return att.Static;
        }

        /// <summary>
        /// Server calls this when it's started.
        /// </summary>
        internal virtual void Start(GXDLMSServer server)
        {

        }

        /// <summary>
        /// Server calls this when it's closed.
        /// </summary>
        internal virtual void Stop(GXDLMSServer server)
        {

        }

        /// <summary>
        /// Copy content.
        /// </summary>
        /// <param name="target">Target object.</param>
        public void CopyTo(GXDLMSObject target)
        {
            foreach (PropertyInfo it in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (it.CanRead && it.CanWrite)
                {
                    object value = it.GetValue(this, null);
                    it.SetValue(target, value, null);
                }
            }
        }

        /// <summary>
        /// Returns is Method attribute read only.
        /// </summary>-
        /// <param name="index">Method Attribute index.</param>
        /// <returns>Is attribute read only.</returns>
        public byte GetAccessSelector(int index)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, Attributes);
            return att.AccessSelector;
        }

        /// <summary>
        /// Set Method attribute access.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="access"></param>
        public void SetAccessSelector(int index, byte value)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, Attributes);
            att.AccessSelector = value;
        }

    }
}
