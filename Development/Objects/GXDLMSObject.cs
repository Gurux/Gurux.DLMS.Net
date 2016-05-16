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
using Gurux.DLMS.ManufacturerSettings;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{  
    /// <summary>
    /// Notifiest that user has change value.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="Dirty"></param>
    /// <param name="attributeIndex"></param>
    /// <param name="value"></param>
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
            return GetLastReadTime(index) != DateTime.MinValue;
        }
        
        /// <summary>
        /// Is attribute of the object readable.
        /// </summary>
        /// <param name="index">Attribute index of the object.</param>
        /// <returns>True, if attribute of the object is readable.</returns>
        public bool CanRead(int index)
        {
            return GetAccess(index) != AccessMode.NoAccess;
        }

        /// <summary>
        /// List of changed items.
        /// </summary>
        System.Collections.Generic.SortedDictionary<int, object> DirtyAttributes = new SortedDictionary<int, object>();

        System.Collections.Generic.SortedDictionary<int, DateTime> ReadTimes = new SortedDictionary<int, DateTime>();

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
                string[] items = ln.Split('.');
                if (items.Length != 6)
                {
                    throw new GXDLMSException("Invalid Logical Name.");
                }                
            }
            this.LogicalName = ln;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [System.Xml.Serialization.XmlIgnore()]
        public Gurux.DLMS.Objects.GXDLMSObjectCollection Parent
        {
            get;
            internal set;
        }

        public GXDLMSObject Clone()
        {
            List<Type> types = new List<Type>(GXDLMSClient.GetObjectTypes());
            types.Add(typeof(GXDLMSAttributeSettings));
            types.Add(typeof(GXDLMSAttribute));
            using (Stream stream = new MemoryStream())
            {
                XmlSerializer x = new XmlSerializer(this.GetType(), types.ToArray());
                x.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                return x.Deserialize(stream) as GXDLMSObject;
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
        /// Reserved for internal use.
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        internal static string ToLogicalName(byte[] buff)
        {
            if (buff.Length == 6)
            {            
                return (buff[0] & 0xFF) + "." + (buff[1] & 0xFF) + "." + (buff[2] & 0xFF) + "." + 
                    (buff[3] & 0xFF) + "." + (buff[4] & 0xFF) + "." + (buff[5] & 0xFF);
            }
            return "";
        }

        /// <summary>
        /// Interface type of the DLMS object.
        /// </summary>
        [ReadOnly(true)]
        [System.Xml.Serialization.XmlIgnore()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ObjectType ObjectType
        {
            get;
            internal set;
        }

        /// <summary>
        /// DLMS version number.
        /// </summary>
        [ReadOnly(true)]
        [DefaultValue(0)]
        public int Version
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
        [ReadOnly(true)]
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
            DirtyAttributes.Remove(attributeIndex);
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
            if (DirtyAttributes.ContainsKey(attributeIndex))
            {
                value = DirtyAttributes[attributeIndex];
                return true;
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
            return DirtyAttributes.Keys.ToArray();                        
        }

        /// <summary>
        /// Update dirty state of attribute index.
        /// </summary>
        /// <param name="attributeIndex"></param>
        /// <param name="value"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateDirty(int attributeIndex, object value)
        {
            DirtyAttributes[attributeIndex] = value;
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
            if (!ReadTimes.ContainsKey(attributeIndex))
            {
                return DateTime.MinValue;
            }
            return ReadTimes[attributeIndex];
        }

        /// <summary>
        /// Set time when attribute was last time read.
        /// </summary>
        /// <param name="attributeIndex"></param>
        /// <param name="tm"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetLastReadTime(int attributeIndex, DateTime tm)
        {
            ReadTimes[attributeIndex] = tm;
        }

        /// <summary>
        /// Returns is attribute read only.
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
            GXDLMSAttributeSettings att = GetAttribute(index, Attributes);
            att.Access = access;
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
            GXDLMSAttributeSettings att = MethodAttributes.Find(index);
            if (att == null)
            {
                att = new GXDLMSAttributeSettings(index);
                MethodAttributes.Add(att);
            }
            att.MethodAccess = access;
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

        public bool GetStatic(int index)
        {
            GXDLMSAttributeSettings att = GetAttribute(index, null);
            return att.Static;
        }        
    }
}
