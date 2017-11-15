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
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Linq.Expressions;
using Gurux.Common.Internal;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Gurux.Common.JSon
{
    /// <summary>
    /// This class is used to handle JSON serialization.
    /// </summary>
    public class GXJsonParser
    {
        /// <summary>
        /// Cached objects
        /// </summary>
        static Hashtable CachedObjects = new Hashtable();

        /// <summary>
        /// Serialized objects are saved to cache to make serialization faster.
        /// </summary>
        private readonly Dictionary<Type, SortedDictionary<string, GXSerializedItem>> SerializedObjects = new Dictionary<Type, SortedDictionary<string, GXSerializedItem>>();

        private CreateObjectEventhandler m_CreateObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXJsonParser()
        {
            ExtraTypes = new List<Type>();
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
        /// Indent elements.
        /// </summary>
        public bool Indent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an object that contains data to associate with the parser.
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Notifies that abstract class is created.
        /// </summary>
        public event CreateObjectEventhandler OnCreateObject
        {
            add
            {
                m_CreateObject += value;
            }
            remove
            {
                m_CreateObject -= value;
            }
        }

        /// <summary>
        /// Parse JSON Objects.
        /// </summary>
        /// <param name="data">data string.</param>
        /// <returns>Name/value pair of found objects.</returns>
        public static Dictionary<string, object> ParseObjects(string data)
        {
            using (TextReader reader = new StringReader(data))
            {
                StringBuilder sb = new StringBuilder();
                return ParseObjects(reader, sb, false);
            }
        }

        /// <summary>
        /// Parse JSON Objects from File.
        /// </summary>
        /// <param name="path">Text file where objects are loaded.</param>
        /// <returns>Name/value pair of found objects.</returns>
        public static Dictionary<string, object> ParseObjectsFromFile(string path)
        {
            using (TextReader reader = File.OpenText(path))
            {
                StringBuilder sb = new StringBuilder();
                return ParseObjects(reader, sb, false);
            }
        }

        /// <summary>
        /// Convert JSON epoch time to DateTime.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static DateTime GetDateTime(string str)
        {
            if (str != null && str.StartsWith("\\/Date("))
            {
                DateTime dt;
                int index = str.Length - 8;
                char ch = str[index];
                if (ch == '-' || ch == '+')
                {
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    int hours = int.Parse(str.Substring(index + 1, 2));
                    int minutes = int.Parse(str.Substring(index + 3, 2));
                    long seconds = long.Parse(str.Substring(7, index - 7));
                    if (ch == '-')
                    {
                        epoch.AddHours(hours);
                        epoch.AddMinutes(minutes);
                    }
                    else
                    {
                        epoch.AddHours(-hours);
                        epoch.AddMinutes(-minutes);
                    }
                    dt = epoch.AddSeconds(seconds / 1000);
                    //If Min value do not convert to local time.
                    if (dt == DateTime.MinValue)
                    {
                        return dt;
                    }
                    return dt.ToLocalTime();

                }
                else//If offset is not given we are in local time zone.
                {
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
                    long seconds = long.Parse(str.Substring(7, str.Length - 10));
                    dt = epoch.AddSeconds(seconds / 1000);
                    return dt;
                }
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Set object value.
        /// </summary>
        /// <param name="target">Instance where parsed data is saved.</param>
        /// <param name="item">GXSerializedItem</param>
        /// <param name="value">String value to parse.</param>
        /// <param name="type">String value type.</param>
        private void SetValue(object target, GXSerializedItem item, string value, Type type)
        {
            object val;
            if (type == typeof(byte[]))
            {
                val = Convert.FromBase64String(value);
            }
            else if (type == typeof(DateTime))
            {
                val = GetDateTime(value);
            }
            else if (type == typeof(Guid))
            {
                val = new Guid(value);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                SetValue(target, item, value, Nullable.GetUnderlyingType(type));
                return;
            }
            else if (type == typeof(double))
            {
                val = Convert.ToDouble(value, CultureInfo.InvariantCulture.NumberFormat);
            }
            else if (type == typeof(float))
            {
                val = (float)Convert.ToDouble(value, CultureInfo.InvariantCulture.NumberFormat);
            }
            else if (type == typeof(TimeSpan))
            {
                val = System.Xml.XmlConvert.ToTimeSpan(value);
            }
            else if (type.IsEnum)
            {
                val = Enum.Parse(type, value);
            }
            else if (type == typeof(Type))
            {
                val = Type.GetType(value);
            }
            else if (type == typeof(object))
            {
                if (!string.IsNullOrEmpty(value))
                {
                    int pos = value.IndexOf(";");
                    if (pos != -1)
                    {
                        string tmp = value.Substring(0, pos);
                        Type tp = Type.GetType(tmp);
                        if (tp.IsClass && tp != typeof(string) && tp != typeof(Guid) && tp != typeof(DateTime))
                        {
                            tmp = value.Substring(pos + 1);
                            val = Deserialize(tmp, tp);
                        }
                        else
                        {
                            tmp = value.Substring(pos + 1);
                            val = Convert.ChangeType(tmp, tp);
                        }
                    }
                    else
                    {
                        val = value;
                    }
                }
                else
                {
                    val = null;
                }
            }
            else
            {
                val = Convert.ChangeType(value, type);
            }
            if (item.Set != null)
            {
                item.Set(target, val);
            }
            else
            {
                PropertyInfo pi = item.Target as PropertyInfo;
                if (pi != null)
                {
                    pi.SetValue(target, val, null);
                }
                else
                {
                    FieldInfo fi = item.Target as FieldInfo;
                    fi.SetValue(target, val);
                }
            }
        }

        /// <summary>
        /// Deserialize JSON data to objects.
        /// </summary>
        /// <typeparam name="T">Deserialized type.</typeparam>
        /// <param name="data">JSON data.</param>
        /// <returns>Deserialized object.</returns>
        public T Deserialize<T>(string data)
        {
            return (T)Deserialize(data, typeof(T));
        }

        /// <summary>
        /// Deserialize JSON data to objects.
        /// </summary>
        /// <typeparam name="T">Deserialized type.</typeparam>
        /// <param name="stream">Strem where object is deserialized.</param>
        /// <returns>Deserialized object.</returns>
        public T Deserialize<T>(System.IO.Stream stream)
        {
            TextReader reader = new StreamReader(stream);
            StringBuilder sb = new StringBuilder();
            Dictionary<string, object> list = ParseObjects(reader, sb, false);
            return (T)Deserialize(list, typeof(T));
        }

        /// <summary>
        /// Deserialize JSON data to objects.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Deserialize(System.IO.Stream stream, Type type)
        {
            TextReader reader = new StreamReader(stream);
            StringBuilder sb = new StringBuilder();
            Dictionary<string, object> list = ParseObjects(reader, sb, false);
            return Deserialize(list, type);
        }

        /// <summary>
        /// Deserialize JSON data to objects.
        /// </summary>
        /// <param name="type">Data type.</param>
        /// <param name="data">JSON data as a string.</param>
        /// <returns>JSON data.</returns>
        public object Deserialize(string data, Type type)
        {
            using (TextReader reader = new StringReader(data))
            {
                StringBuilder sb = new StringBuilder();
                Dictionary<string, object> list = ParseObjects(reader, sb, false);
                return Deserialize(list, type);
            }
        }

        /// <summary>
        /// Save object as JSON object.
        /// </summary>
        /// <param name="target">Object to save.</param>
        /// <param name="path">File path.</param>
        public static void Save(object target, string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
#if !__MOBILE__
                GXFileSystemSecurity.UpdateDirectorySecurity(dir);
#endif
            }
            GXJsonParser parser = new GXJsonParser();
            using (TextWriter writer = File.CreateText(path))
            {
                parser.Serialize(target, writer, false, false, true, false);
            }
#if !__MOBILE__
            GXFileSystemSecurity.UpdateFileSecurity(path);
#endif
        }

        /// <summary>
        /// Save object as JSON object.
        /// </summary>
        /// <param name="target">Object to save.</param>
        /// <param name="stream">Stream shere object is saved.</param>
        public void Serialize(object target, System.IO.Stream stream)
        {
            TextWriter writer = new StreamWriter(stream);
            Serialize(target, writer, false, false, Indent, false);
        }

        /// <summary>
        /// Save object as JSON object.
        /// </summary>
        /// <param name="target">Object to save.</param>
        /// <param name="stream">Stream shere object is saved.</param>
        /// <param name="http">Is stream http stream.</param>
        /// <param name="get">Is this get message.</param>
        private void Serialize(object target, System.IO.Stream stream, bool http, bool get)
        {
            TextWriter writer = new StreamWriter(stream);
            Serialize(target, writer, http, get, Indent, false);
        }

        /// <summary>
        /// Load JSON object.
        /// </summary>
        /// <typeparam name="T">Object type to load.</typeparam>
        /// <param name="path">File path.</param>
        /// <returns>Loaded object.</returns>
        public static T Load<T>(string path)
        {
            if (File.Exists(path))
            {
                GXJsonParser parser = new GXJsonParser();
                using (TextReader reader = File.OpenText(path))
                {
                    StringBuilder sb = new StringBuilder();
                    Dictionary<string, object> list = ParseObjects(reader, sb, false);
                    return (T)parser.Deserialize(list, typeof(T));
                }
            }
            return default(T);
        }

        /// <summary>
        /// Is file JSON file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsJSONFile(string path)
        {
            using (TextReader reader = File.OpenText(path))
            {
                if (reader.Peek() == '{')
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Try load JSON object.
        /// </summary>
        /// <typeparam name="T">Object type to load.</typeparam>
        /// <param name="path">File path.</param>
        /// <param name="result">Loaded object.</param>
        /// <returns>Returns is object loaded.</returns>
        public static bool TryLoad<T>(string path, out T result)
        {
            if (File.Exists(path))
            {
                GXJsonParser parser = new GXJsonParser();
                using (TextReader reader = File.OpenText(path))
                {
                    if (reader.Peek() == '{')
                    {
                        try
                        {
                            StringBuilder sb = new StringBuilder();
                            Dictionary<string, object> list = ParseObjects(reader, sb, false);
                            result = (T)parser.Deserialize(list, typeof(T));
                            return true;
                        }
                        catch (Exception)
                        {
                            result = default(T);
                            return false;
                        }
                    }
                }
            }
            result = default(T);
            return false;
        }

        /// <summary>
        /// Load JSON object.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="type">Object type</param>
        public static object Load(string path, Type type)
        {
            GXJsonParser parser = new GXJsonParser();
            using (TextReader reader = File.OpenText(path))
            {
                StringBuilder sb = new StringBuilder();
                Dictionary<string, object> list = ParseObjects(reader, sb, false);
                return parser.Deserialize(list, type);
            }
        }

        /// <summary>
        /// Load JSON object.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="type">Object type</param>
        public object LoadFile(string path, Type type)
        {
            using (TextReader reader = File.OpenText(path))
            {
                StringBuilder sb = new StringBuilder();
                Dictionary<string, object> list = ParseObjects(reader, sb, false);
                return Deserialize(list, type);
            }
        }

        /// <summary>
        /// Serialize object to JSON string.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public string Serialize(object target)
        {
            TextWriter writer = new StringWriter();
            Serialize(target, writer, false, false, Indent, false);
            return writer.ToString();
        }

        /// <summary>
        /// Serialize object to HTTP stream.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public string SerializeToHttp(object target)
        {
            TextWriter writer = new StringWriter();
            Serialize(target, writer, true, false, Indent, false);
            return writer.ToString();
        }

        private static void UpdateAttributes(Type type, object[] attributes, GXSerializedItem s)
        {
            foreach (object it in attributes)
            {
                if (it is DefaultValueAttribute)
                {
                    DefaultValueAttribute def = it as DefaultValueAttribute;
                    s.DefaultValue = def.Value;
                }
                if (it is DataMemberAttribute)
                {
                    DataMemberAttribute attr = it as DataMemberAttribute;
                    if (attr.IsRequired)
                    {
                        s.Attributes = Attributes.Required;
                    }
                }
            }
        }

        /// <summary>
        /// Serialize object to JSON string.
        /// </summary>
        /// <param name="target">Serialized object.</param>
        /// <param name="writer">Text writer that us used for serializing.</param>
        /// <param name="http">Is this http request.</param>
        /// <param name="get">Is this http get request.</param>
        /// <param name="indent">Is indent used.</param>
        /// <param name="isObject">Is target serialized as generic object.</param>
        internal void Serialize(object target, TextWriter writer, bool http, bool get, bool indent, bool isObject)
        {
            SortedDictionary<string, GXSerializedItem> list;
            Type type = target.GetType();
            lock (SerializedObjects)
            {
                if (SerializedObjects.ContainsKey(type))
                {
                    list = SerializedObjects[type];
                }
                else
                {
                    list = (SortedDictionary<string, GXSerializedItem>)GXInternal.GetValues(type, true, UpdateAttributes);
                    SerializedObjects.Add(type, list);
                }
            }
            if (list == null)
            {
                writer.Write(target);
                return;
            }
            if (target is System.Collections.IEnumerable)
            {
                System.Collections.IEnumerator coll = (target as System.Collections.IEnumerable).GetEnumerator();
                writer.Write("[");
                bool first = true;
                while (coll.MoveNext())
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(",");
                        if (indent)
                        {
                            writer.Write(Environment.NewLine);
                        }
                    }
                    Serialize(coll.Current, writer, http, get, false, isObject);
                }
                writer.Write("]");
            }
            else
            {
                if (!get)
                {
                    writer.Write("{");
                }
                bool first = true;
                foreach (var it in list)
                {
                    object value;
                    Type dataType = it.Value.Type;
                    if (it.Value.Get != null)
                    {
                        value = GXInternal.ShouldSerializeValue(it.Value.Get(target));
                    }
                    else if (it.Value.Target is PropertyInfo)
                    {
                        value = GXInternal.ShouldSerializeValue(((it.Value.Target) as PropertyInfo).GetValue(target, null));
                    }
                    else
                    {
                        value = GXInternal.ShouldSerializeValue((it.Value.Target as FieldInfo).GetValue(target));
                    }

                    if (value != null && !value.Equals(it.Value.DefaultValue))
                    {
                        if (!first)
                        {
                            if (get)
                            {
                                writer.Write("&");
                            }
                            else
                            {
                                writer.Write(",");
                            }
                            if (indent)
                            {
                                writer.Write(Environment.NewLine);
                            }
                        }
                        else
                        {
                            first = false;
                        }
                        if (!get)
                        {
                            if (isObject)
                            {
                                writer.Write('\\');
                            }
                            writer.Write("\"");
                        }
                        writer.Write(it.Key);
                        if (!http)
                        {
                            if (isObject)
                            {
                                writer.Write('\\');
                            }
                            writer.Write("\":");
                        }
                        else
                        {
                            if (!get)
                            {
                                if (isObject)
                                {
                                    writer.Write('\\');
                                }
                                writer.Write("\":");
                            }
                            else
                            {
                                writer.Write("=");
                            }
                        }

                        if (value is string)
                        {
                            string str = (value as string);
                            if (!get)
                            {
                                if (isObject)
                                {
                                    writer.Write('\\');
                                }
                                writer.Write("\"");
                                if (str.Contains("\\"))
                                {
                                    str = str.Replace("\\", "\\\\");
                                }
                                if (str.Contains("\""))
                                {
                                    str = str.Replace("\"", "\\\"");
                                }
                            }
                            writer.Write(str);
                            if (!get)
                            {
                                if (isObject)
                                {
                                    writer.Write('\\');
                                }
                                writer.Write("\"");
                            }
                        }
                        else if (value is byte[])
                        {
                            if (isObject)
                            {
                                writer.Write('\\');
                            }
                            writer.Write("\"");
                            writer.Write(Convert.ToBase64String((byte[])value));
                            if (isObject)
                            {
                                writer.Write('\\');
                            }
                            writer.Write("\"");
                        }
                        else if (value is DateTime)
                        {
                            writer.Write(GXInternal.ToString((DateTime)value, isObject ? true : get));
                        }
                        else if (value is Guid)
                        {
                            if (!get)
                            {
                                if (isObject)
                                {
                                    writer.Write('\\');
                                }
                                writer.Write("\"");
                            }
                            writer.Write(value.ToString().Replace("-", ""));
                            if (!get)
                            {
                                if (isObject)
                                {
                                    writer.Write('\\');
                                }
                                writer.Write("\"");
                            }
                        }
                        else if (value is System.Collections.IEnumerable)
                        {
                            System.Collections.IEnumerator coll = (value as System.Collections.IEnumerable).GetEnumerator();
                            writer.Write("[");
                            bool first2 = true;
                            while (coll.MoveNext())
                            {
                                if (first2)
                                {
                                    first2 = false;
                                }
                                else
                                {
                                    writer.Write(",");
                                    if (indent)
                                    {
                                        writer.Write(Environment.NewLine);
                                    }
                                }
                                Serialize(coll.Current, writer, http, get, false, isObject);
                            }
                            writer.Write("]");
                        }
                        else if (value is float)
                        {
                            writer.Write(((float)value).ToString(CultureInfo.InvariantCulture.NumberFormat));
                        }
                        else if (value is double)
                        {
                            writer.Write(((double)value).ToString(CultureInfo.InvariantCulture.NumberFormat));
                        }
                        else if (value is TimeSpan)
                        {
                            if (!get)
                            {
                                if (isObject)
                                {
                                    writer.Write('\\');
                                }
                                writer.Write("\"");
                            }
                            writer.Write(System.Xml.XmlConvert.ToString((TimeSpan)value));
                            if (!get)
                            {
                                if (isObject)
                                {
                                    writer.Write('\\');
                                }
                                writer.Write("\"");
                            }
                        }
                        else if (value.GetType().IsEnum)
                        {
                            if (!get)
                            {
                                if (isObject)
                                {
                                    writer.Write('\\');
                                }
                                writer.Write("\"");
                            }
                            writer.Write(value.ToString());
                            if (!get)
                            {
                                if (isObject)
                                {
                                    writer.Write('\\');
                                }
                                writer.Write("\"");
                            }
                        }
                        else if (value is Type)
                        {
                            if (!get)
                            {
                                if (isObject)
                                {
                                    writer.Write('\\');
                                }
                                writer.Write("\"");
                            }
                            writer.Write(value.ToString());
                            if (!get)
                            {
                                if (isObject)
                                {
                                    writer.Write('\\');
                                }
                                writer.Write("\"");
                            }
                        }
                        else if (dataType == typeof(object))
                        {
                            if (!get)
                            {
                                writer.Write("\"");
                            }
                            if (!(value is string || value is Guid) && value.GetType().IsClass)
                            {
                                writer.Write(value.GetType().AssemblyQualifiedName);
                                writer.Write(";");
                                Serialize(value, writer, http, get, false, true);
                            }
                            else
                            {
                                writer.Write(value.GetType().FullName);
                                writer.Write(";");
                                writer.Write(value.ToString());
                            }
                            if (!get)
                            {
                                writer.Write("\"");
                            }
                        }
                        else if (value.GetType().IsClass)
                        {
                            Serialize(value, writer, http, get, false, isObject);
                        }
                        else
                        {
                            writer.Write(value.ToString());
                        }
                    }
                }
                if (!get)
                {
                    writer.Write("}");
                }
            }
        }

        /// <summary>
        /// Deserialize JSON data to objects.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Deserialize(Dictionary<string, object> data, Type type)
        {
            return Deserialize(data, type, null);
        }

        /// <summary>
        /// Creates instance of given type.
        /// </summary>
        /// <param name="type">Created data type.</param>
        /// <returns>Created instance.</returns>
        public static object CreateInstance(Type type)
        {
            lock (CachedObjects)
            {
                Func<object> tmp = (Func<object>)CachedObjects[type];
                if (tmp != null)
                {
                    return tmp();
                }

                tmp = Expression.Lambda<Func<object>>
                      (
                          Expression.New(type)
                      ).Compile();
                CachedObjects.Add(type, tmp);
                return tmp();
            }
        }

        /// <summary>
        /// Deserialize JSON data to objects.
        /// </summary>
        /// <param name="parameters">Deserialized target if created yet.</param>
        /// <param name="type">Object type.</param>
        /// <param name="target"></param>
        /// <returns>Deserialized object.</returns>
        private object Deserialize(Dictionary<string, object> parameters, Type type, object target)
        {
            if (target == null && !type.IsArray)
            {
                if (type.IsAbstract)
                {
                    if (m_CreateObject == null)
                    {
                        throw new Exception("Can't create abstract class: " + type.FullName);
                    }
                    GXCreateObjectEventArgs e = new GXCreateObjectEventArgs(type, parameters, ExtraTypes);
                    m_CreateObject(this, e);
                    target = e.Object;
                    type = target.GetType();
                }
                else
                {
                    if (m_CreateObject != null)
                    {
                        GXCreateObjectEventArgs e = new GXCreateObjectEventArgs(type, parameters, ExtraTypes);
                        m_CreateObject(this, e);
                        target = e.Object;
                    }
                    if (target != null)
                    {
                        type = target.GetType();
                    }
                    else
                    {
                        if (type.IsEnum)
                        {
                            target = Enum.ToObject(type, 0);
                        }
                        else
                        {
                            target = CreateInstance(type);
                        }
                    }
                }
            }
            SortedDictionary<string, GXSerializedItem> list;
            lock (SerializedObjects)
            {
                if (SerializedObjects.ContainsKey(type))
                {
                    list = SerializedObjects[type];
                }
                else
                {
                    list = (SortedDictionary<string, GXSerializedItem>)GXInternal.GetValues(type, true, UpdateAttributes);
                    SerializedObjects.Add(type, list);
                }
            }
            Dictionary<string, object>.Enumerator serializedItem = parameters.GetEnumerator();
            SortedDictionary<string, GXSerializedItem>.Enumerator item = list.GetEnumerator();
            while (serializedItem.MoveNext())
            {
                if (target is System.Collections.IList && string.Compare(serializedItem.Current.Key, "Items") == 0)
                {
                    Type tp = GXInternal.GetPropertyType(type);
                    if (serializedItem.Current.Value != null)
                    {
                        System.Collections.IList list2 = target as System.Collections.IList;
                        foreach (object it in (System.Collections.IEnumerable)serializedItem.Current.Value)
                        {
                            list2.Add(Deserialize((Dictionary<string, object>)it, tp));
                        }
                    }
                }
                else if (type.IsArray)
                {
                    Type itemType = GXInternal.GetPropertyType(type);
                    if (serializedItem.Current.Value != null)
                    {
                        System.Collections.IList sItems = (System.Collections.IList)serializedItem.Current.Value;
                        Array items = Array.CreateInstance(itemType, sItems.Count);
                        target = items;
                        if (sItems.Count != 0)
                        {
                            int pos = -1;
                            foreach (object it in sItems)
                            {
                                items.SetValue(Deserialize((Dictionary<string, object>)it, itemType), ++pos);
                            }
                        }
                    }
                    else//If array is empty.
                    {
                        target = Array.CreateInstance(itemType, 0);
                    }
                }
                else
                {
                    if (!item.MoveNext())
                    {
                        break;
                    }
                    if (!UpdateValue(target, item.Current, serializedItem.Current))
                    {
                        bool found = false;
                        while (item.MoveNext())
                        {
                            if (UpdateValue(target, item.Current, serializedItem.Current))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            System.Diagnostics.Debug.WriteLine("Unknown tag: " + serializedItem.Current.Key);
                        }
                    }
                }
            }
            //If array is empty.
            if (type.IsArray && target == null)
            {
                Type itemType = type.GetElementType();
                target = Array.CreateInstance(itemType, 0);
            }
            return target;
        }

        /// <summary>
        /// Update serialized value.
        /// </summary>
        /// <param name="target">Component where value is updated.</param>
        /// <param name="item">Property where data is updated.</param>
        /// <param name="serializedItem">Serialized data</param>
        /// <returns>True, if value is updated.</returns>
        private bool UpdateValue(object target, KeyValuePair<string, GXSerializedItem> item, KeyValuePair<string, object> serializedItem)
        {
            bool ret = string.Compare(item.Key, serializedItem.Key, true) == 0;
            if (ret)
            {
                Type tp = item.Value.Type;
                if (tp != typeof(object) && tp != typeof(string) && tp != typeof(Type) && tp.IsClass && !tp.IsArray)
                {
                    if (serializedItem.Value is List<object>)
                    {
                        System.Collections.IList list2 = (System.Collections.IList)CreateInstance(tp);
                        Type itemType = GXInternal.GetPropertyType(tp);
                        foreach (object it in (List<object>)serializedItem.Value)
                        {
                            object value2 = Deserialize((Dictionary<string, object>)it, itemType);
                            list2.Add(value2);
                        }
                        if (item.Value.Set != null)
                        {
                            item.Value.Set(target, list2);
                        }
                        else
                        {
                            PropertyInfo pi = item.Value.Target as PropertyInfo;
                            if (pi != null)
                            {
                                pi.SetValue(target, list2, null);
                            }
                            else
                            {
                                FieldInfo fi = item.Value.Target as FieldInfo;
                                fi.SetValue(target, list2);
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, object> value = (Dictionary<string, object>)serializedItem.Value;
                        object val = null;
                        if (item.Value.Get != null)
                        {
                            val = item.Value.Get(target);
                        }
                        else
                        {
                            PropertyInfo pi = item.Value.Target as PropertyInfo;
                            if (pi != null)
                            {
                                val = pi.GetValue(target, null);
                            }
                            else
                            {
                                FieldInfo fi = item.Value.Target as FieldInfo;
                                val = fi.GetValue(target);
                            }
                        }
                        //If class is already made.
                        if (val != null)
                        {
                            Deserialize(value, val.GetType(), val);
                            return true;
                        }
                        if (item.Value.Set != null)
                        {
                            item.Value.Set(target, Deserialize(value, tp, val));
                        }
                        else
                        {
                            PropertyInfo pi = item.Value.Target as PropertyInfo;
                            if (pi != null)
                            {
                                pi.SetValue(target, Deserialize(value, tp, val), null);
                            }
                            else
                            {
                                FieldInfo fi = item.Value.Target as FieldInfo;
                                fi.SetValue(target, Deserialize(value, tp, val));
                            }
                        }
                    }
                }
                else if (tp.IsArray && tp != typeof(byte[]))
                {
                    Type itemType = tp.GetElementType();
                    List<object> items = (List<object>)serializedItem.Value;
                    Array list2 = Array.CreateInstance(itemType, items.Count);
                    if (items.Count != 0)
                    {
                        if (items[0] is System.Collections.IDictionary)
                        {
                            int pos = -1;
                            foreach (object it in items)
                            {
                                object value2 = Deserialize((Dictionary<string, object>)it, itemType);
                                list2.SetValue(value2, ++pos);
                            }
                        }
                        else
                        {
                            int pos = 0;
                            foreach (object it in items)
                            {
                                if (itemType == typeof(Guid) && it.GetType() == typeof(string))
                                {
                                    list2.SetValue(new Guid((string)it), pos);
                                }
                                else
                                {
                                    list2.SetValue(Convert.ChangeType(it, itemType), pos);
                                }
                                ++pos;
                            }
                        }
                    }
                    if (item.Value.Set != null)
                    {
                        item.Value.Set(target, list2);
                    }
                    else
                    {
                        PropertyInfo pi = item.Value.Target as PropertyInfo;
                        if (pi != null)
                        {
                            pi.SetValue(target, list2, null);
                        }
                        else
                        {
                            FieldInfo fi = item.Value.Target as FieldInfo;
                            fi.SetValue(target, list2);
                        }
                    }
                }
                else
                {
                    SetValue(target, item.Value, (string)serializedItem.Value, tp);
                }
            }
            return ret;
        }

        /// <summary>
        /// Is char special char.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        static bool IsSpecialChar(char ch)
        {
            return ch == ',' || ch == '{' || ch == '}' || ch == '[' || ch == ']' || ch == ':' || ch == '=' || ch == '\"';
        }

        /// <summary>
        /// Parse JSON Objects.
        /// </summary>
        /// <param name="stream">Data string where JSON objects ar parsed.</param>
        /// <param name="sb">Target string builder where found name/value pair is saved. It's faster give it as a parameter than create new one for every time this function is called.</param>
        /// <param name="collection">Is this a collection.</param>
        /// <returns>Name/value pair of found objects.</returns>
        static Dictionary<string, object> ParseObjects(TextReader stream, StringBuilder sb, bool collection)
        {
            SortedDictionary<string, object> list = new SortedDictionary<string, object>();
            List<object> array = null;
            if (collection)
            {
                array = new List<object>();
            }
            string key = null;
            object value = null;
            bool insideString = false;
            char lastch, ch = '\0';
            int tmp;
            int replaced = 0;
            while ((tmp = stream.Read()) != -1)
            {
                lastch = ch;
                ch = (char)tmp;
                //If control char and not inside of a string.
                if (!insideString && IsSpecialChar(ch))
                {
                    if (ch == '\"')
                    {
                        insideString = true;
                    }
                    else if (ch == ':' || ch == '=')
                    {
                        key = sb.ToString();
                        sb.Length = 0;
                    }
                    else if (ch == ',')
                    {
                        if (sb.Length != 0)
                        {
                            value = sb.ToString();
                            if ((replaced & 2) == 2)
                            {
                                value = ((string)value).Replace("\\\"", "\"");
                            }
                            if ((replaced & 1) == 1)
                            {
                                value = ((string)value).Replace("\\\\", "\\");
                            }
                            replaced = 0;
                            sb.Length = 0;
                        }
                        if (value != null)
                        {
                            if (key != null)
                            {
                                list.Add(key, value);
                                key = null;
                                value = null;
                            }
                            else if (array != null)
                            {
                                array.Add(value);
                            }
                        }
                    }
                    //Object starts.
                    else if (ch == '{')
                    {
                        value = ParseObjects(stream, sb, false);
                        if (array != null)
                        {
                            array.Add(value);
                            //If we are adding array of values. Like string[].
                            //Reset value or it's added twice.
                            if (key == null)
                            {
                                value = null;
                            }
                        }
                        else
                        {
                            if (key != null)
                            {
                                list.Add(key, value);
                                key = null;
                            }
                            else
                            {
                                foreach (var it in (Dictionary<string, object>)value)
                                {
                                    list.Add(it.Key, it.Value);
                                }
                            }
                        }
                    }
                    //Object ends.
                    else if (ch == '}')
                    {
                        if (sb.Length != 0)
                        {
                            value = sb.ToString();
                            if ((replaced & 2) == 2)
                            {
                                value = ((string)value).Replace("\\\"", "\"");
                            }
                            if ((replaced & 1) == 1)
                            {
                                value = ((string)value).Replace("\\\\", "\\");
                            }
                            replaced = 0;
                            sb.Length = 0;
                        }
                        if (key != null)
                        {
                            list.Add(key, value);
                            key = null;
                            value = null;
                        }
                        return new Dictionary<string, object>(list);
                    }
                    //Collection starts
                    else if (ch == '[')
                    {
                        array = new List<object>();
                        value = ParseObjects(stream, sb, true);
                        foreach (var it in (Dictionary<string, object>)value)
                        {
                            if (key == null)
                            {
                                list.Add(it.Key, it.Value);
                            }
                            else
                            {
                                list.Add(key, it.Value);
                            }
                        }
                        //If property of objects.
                        if (key == null)
                        {
                            return (Dictionary<string, object>)value;
                        }
                        //If array is property array.
                        sb.Length = 0;
                        value = null;
                        key = null;
                    }
                    //Collection ends
                    else if (ch == ']')
                    {
                        if (sb.Length != 0)
                        {
                            value = sb.ToString();
                            if ((replaced & 2) == 2)
                            {
                                value = ((string)value).Replace("\\\"", "\"");
                            }
                            if ((replaced & 1) == 1)
                            {
                                value = ((string)value).Replace("\\\\", "\\");
                            }
                            array.Add(value);
                            sb.Length = 0;
                        }
                        list.Add("Items", array);
                        break;
                    }
                    else if (key != null && array == null)
                    {
                        value = sb.ToString();
                        if ((replaced & 2) == 2)
                        {
                            value = ((string)value).Replace("\\\"", "\"");
                        }
                        if ((replaced & 1) == 1)
                        {
                            value = ((string)value).Replace("\\\\", "\\");
                        }
                        replaced = 0;
                        list.Add(key, value);
                        sb.Length = 0;
                        key = null;
                        value = null;
                    }
                }
                else if (insideString || (ch != '\r' && ch != '\n'))
                {
                    if (ch == '\"' && lastch != '\\')
                    {
                        insideString = false;
                    }
                    else if (ch != '\"' || insideString)
                    {
                        sb.Append(ch);
                        if (replaced != 1 && ch == '\\' && stream.Peek() == '\\')
                        {
                            replaced |= 1;
                        }
                        else if (replaced != 2 && ch == '\\' && stream.Peek() == '\"')
                        {
                            replaced |= 2;
                        }
                    }
                }
            }
            return new Dictionary<string, object>(list);
        }
    }
}
