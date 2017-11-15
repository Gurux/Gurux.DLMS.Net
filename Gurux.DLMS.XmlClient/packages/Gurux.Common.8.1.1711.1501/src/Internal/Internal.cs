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
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using Gurux.Common.JSon;

namespace Gurux.Common.Internal
{
    /// <summary>
    /// Property getter delegate.
    /// </summary>
    /// <param name="instance">Target.</param>
    /// <returns>Property value</returns>
    delegate object GetHandler(object instance);

    /// <summary>
    /// Property setter delegate.
    /// </summary>
    /// <param name="instance">Target.</param>
    /// <param name="value">New value.</param>
    delegate void SetHandler(object instance, object value);

    /// <summary>
    /// Gurux.Service.Rest method invoke handler.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    delegate object InvokeHandler(object instance, object value);

    [Flags]
    enum Attributes : int
    {
        None = 0,
        /// <summary>
        /// Field is Id.
        /// </summary>
        Id = 1,
        /// <summary>
        /// Is index attribute used.
        /// </summary>
        Index = 2,
        /// <summary>
        /// Is auto increment attribute used.
        /// </summary>
        AutoIncrement = 4,
        /// <summary>
        /// Field is primary key.
        /// </summary>
        PrimaryKey = 8,
        /// <summary>
        /// Is foreign key used.
        /// </summary>
        ForeignKey = 0x10,
        //Value is required.
        Required = 0x20,
        /// <summary>
        /// Property is ignored. DB uses this.
        /// </summary>
        Ignored = 0x40,
        /// <summary>
        /// Value is relation to parent table. This is used with 1:n relation.
        /// </summary>
        Relation = 0x80
    }

    enum RelationType
    {
        /// <summary>
        /// Relation between two tables.
        /// </summary>
        Relation,
        /// <summary>
        /// Primary key 1:1.
        /// </summary>
        OneToOne,
        /// <summary>
        /// Primary key 1:n
        /// </summary>
        OneToMany,
        /// <summary>
        /// Primary key n:n.
        /// </summary>
        ManyToMany
    }

    [DataContract]
    class GXRelationTable
    {
        public GXRelationTable()
        {
            return;
        }

        /// <summary>
        /// Column 1 table.
        /// </summary>
        public Type PrimaryTable;

        /// <summary>
        /// Column 1 id.
        /// </summary>
        public GXSerializedItem PrimaryId;

        /// <summary>
        /// Column 1 info.
        /// </summary>
        public GXSerializedItem Column;

        /// <summary>
        /// Column 2 table.
        /// </summary>
        public Type ForeignTable;

        /// <summary>
        /// Column 2 id.
        /// </summary>
        public GXSerializedItem ForeignId;

        /// <summary>
        /// Relation map table if used.
        /// </summary>
        public GXSerializedItem RelationMapTable;

        public RelationType RelationType;
    }

    class GXSerializedItem
    {
        /// <summary>
        /// Property type.
        /// </summary>
        public Type Type;

        public object Target;
        /// <summary>
        /// Default value if given.
        /// </summary>
        public object DefaultValue;
        /// <summary>
        /// Set method.
        /// </summary>
        public SetHandler Set;
        /// <summary>
        /// Get Method.
        /// </summary>
        public GetHandler Get;

        public Attributes Attributes;

        public GXRelationTable Relation;

        public GXSerializedItem Clone()
        {
            GXSerializedItem item = new GXSerializedItem(); ;
            item.Type = Type;
            item.Target = Target;
            item.DefaultValue = DefaultValue;
            item.Set = Set;
            item.Get = Get;
            item.Attributes = Attributes;
            item.Relation = Relation;
            return item;
        }
    }

    /// <summary>
    /// GXJSON and Gurux.Service.Rest internal methods.
    /// </summary>
    class GXInternal
    {
#if !__IOS__
        /// <summary>
        /// Create method handler for Gurux.Service.Rest methods.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodinfo"></param>
        /// <returns></returns>
        internal static InvokeHandler CreateMethodHandler(Type type, MethodInfo methodinfo)
        {
            DynamicMethod dynamic = new DynamicMethod(methodinfo.Name, typeof(object), new Type[] { typeof(object), typeof(object) }, methodinfo.DeclaringType, true);
            // Get an ILGenerator and emit a body for the dynamic method.
            ILGenerator il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, methodinfo);
            il.Emit(OpCodes.Ret);
            return (InvokeHandler)dynamic.CreateDelegate(typeof(InvokeHandler));
        }

        private static DynamicMethod CreateGetDynamicMethod(Type type)
        {
            return new DynamicMethod("Get", typeof(object),
                                     new Type[] { typeof(object) }, type, true);
        }

        private static DynamicMethod CreateSetDynamicMethod(Type type)
        {
            return new DynamicMethod("Set", typeof(void),
                                     new Type[] { typeof(object), typeof(object) }, type, true);
        }

        private static void BoxIfNeeded(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Box, type);
            }
        }

        private static void UnboxIfNeeded(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, type);
            }
        }

        internal static GetHandler CreateGetHandler(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
            DynamicMethod dynamicGet = CreateGetDynamicMethod(type);
            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Call, getMethodInfo);
            BoxIfNeeded(getMethodInfo.ReturnType, getGenerator);
            getGenerator.Emit(OpCodes.Ret);

            return (GetHandler)dynamicGet.CreateDelegate(typeof(GetHandler));
        }

        internal static GetHandler CreateGetHandler(Type type, FieldInfo fieldInfo)
        {
            DynamicMethod dynamicGet = CreateGetDynamicMethod(type);
            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Ldfld, fieldInfo);
            BoxIfNeeded(fieldInfo.FieldType, getGenerator);
            getGenerator.Emit(OpCodes.Ret);

            return (GetHandler)dynamicGet.CreateDelegate(typeof(GetHandler));
        }

        internal static SetHandler CreateSetHandler(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);
            DynamicMethod dynamicSet = CreateSetDynamicMethod(type);
            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            UnboxIfNeeded(setMethodInfo.GetParameters()[0].ParameterType, setGenerator);
            setGenerator.Emit(OpCodes.Call, setMethodInfo);
            setGenerator.Emit(OpCodes.Ret);
            return (SetHandler)dynamicSet.CreateDelegate(typeof(SetHandler));
        }

        internal static SetHandler CreateSetHandler(Type type, FieldInfo fieldInfo)
        {
            DynamicMethod dynamicSet = CreateSetDynamicMethod(type);
            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            UnboxIfNeeded(fieldInfo.FieldType, setGenerator);
            setGenerator.Emit(OpCodes.Stfld, fieldInfo);
            setGenerator.Emit(OpCodes.Ret);
            return (SetHandler)dynamicSet.CreateDelegate(typeof(SetHandler));
        }
#endif
        /// <summary>
        /// Split sent data to packets.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<string> SplitInParts(String str, Int32 length)
        {
            if (str == null)
            {
                throw new ArgumentNullException("String");
            }
            if (length <= 0)
            {
                throw new ArgumentException("Length has to be positive.");
            }
            List<string> items = new List<string>();
            for (int pos = 0; pos < str.Length; pos += length)
            {
                items.Add(str.Substring(pos, Math.Min(length, str.Length - pos)));
            }
            return items;
        }

        /// <summary>
        /// Get custom attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        static public T GetAttribute<T>(object target)
        {
            T[] atts;
            PropertyInfo pi = target as PropertyInfo;
            if (pi != null)
            {
                atts = pi.GetCustomAttributes(typeof(T), true) as T[];
            }
            else
            {
                FieldInfo fi = target as FieldInfo;
                atts = fi.GetCustomAttributes(typeof(T), true) as T[];
            }
            if (atts.Length == 0)
            {
                return default(T);
            }
            return atts[0];
        }

        /// <summary>
        /// Get custom attribute.
        /// </summary>
        /// <typeparam name="T">Target class type.</typeparam>
        /// <param name="type">Custom attribute type to search for.</param>
        /// <returns>Find custom attribute.</returns>
        static public T GetAttribute<T>(Type type)
        {
            T[] atts = type.GetCustomAttributes(typeof(T), true) as T[];
            if (atts.Length == 0)
            {
                return default(T);
            }
            return atts[0];
        }

        /// <summary>
        /// Get property value.
        /// </summary>
        /// <param name="instance">Class instance where value is get.</param>
        /// <param name="target">Property what is get from the instance.</param>
        public static object GetValue(object instance, object target)
        {
            PropertyInfo pi = target as PropertyInfo;
            if (pi != null)
            {
                return pi.GetValue(instance, null);
            }
            else
            {
                FieldInfo fi = target as FieldInfo;
                return fi.GetValue(instance);
            }
        }

        /// <summary>
        /// Set property value.
        /// </summary>
        /// <param name="instance">Instance where value is set.</param>
        /// <param name="target">Property type.</param>
        /// <param name="value">New value.</param>
        public static void SetValue(object instance, object target, object value)
        {
            PropertyInfo pi = target as PropertyInfo;
            if (pi != null)
            {
                if (value is IEnumerable)
                {
                    if (!pi.PropertyType.IsAssignableFrom(value.GetType()))
                    {
                        if (pi.PropertyType.IsArray)
                        {
                            int pos = 0;
                            Array items = Array.CreateInstance(GXInternal.GetPropertyType(pi.PropertyType), ((IList)value).Count);
                            foreach (object it in (IList)value)
                            {
                                items.SetValue(it, pos);
                                ++pos;
                            }
                            value = items;
                        }
#if !__MOBILE__
                        else if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(System.Data.Linq.EntitySet<>))
                        {
                            Type listT = typeof(System.Data.Linq.EntitySet<>).MakeGenericType(new[] { GXInternal.GetPropertyType(pi.PropertyType) });
                            IList list = (IList)GXJsonParser.CreateInstance(listT);
                            foreach (object it in (IList)value)
                            {
                                list.Add(it);
                            }
                            value = list;
                        }
#endif //__MOBILE__
                        else
                        {
                            Type listT = typeof(List<>).MakeGenericType(new[] { GXInternal.GetPropertyType(pi.PropertyType) });
                            IList list = (IList)GXJsonParser.CreateInstance(listT);
                            foreach (object it in (IList)value)
                            {
                                list.Add(it);
                            }
                            value = list;
                        }
                    }
                }
                pi.SetValue(instance, value, null);
            }
            else
            {
                FieldInfo fi = target as FieldInfo;
                fi.SetValue(instance, value);
            }
        }

        internal delegate void UpdateAttributes(Type type, object[] attributes, GXSerializedItem s);

        /// <summary>
        /// Get serialized property and field values.
        /// </summary>
        /// <param name="type">Instance type.</param>
        /// <param name="sorted">Are values returned as sorted dictionary.</param>
        /// <param name="attributeUpdater">Updater that is called to get wanted value. Can be null.</param>
        /// <returns>Dictionary of values.</returns>
        internal static IDictionary<string, GXSerializedItem> GetValues(Type type, bool sorted, UpdateAttributes attributeUpdater)
        {
            GXSerializedItem s;
            if (type.IsPrimitive || type == typeof(string) || type == typeof(Guid))
            {
                return null;
            }
            IDictionary<string, GXSerializedItem> list;
            if (sorted)
            {
                list = new SortedDictionary<string, GXSerializedItem>(StringComparer.InvariantCultureIgnoreCase);
            }
            else
            {
                list = new Dictionary<string, GXSerializedItem>(StringComparer.InvariantCultureIgnoreCase);
            }
            bool all = type.GetCustomAttributes(typeof(DataContractAttribute), true).Length == 0;
            BindingFlags flags;
            //If DataContractAttribute is not set get only public property values.
            if (all)
            {
                flags = BindingFlags.Instance | BindingFlags.Public;
            }
            else
            {
                flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            }
            //Save properties.
            foreach (PropertyInfo it in type.GetProperties(flags))
            {
                DataMemberAttribute[] attr = (DataMemberAttribute[])it.GetCustomAttributes(typeof(DataMemberAttribute), true);
                //If value is not marked as ignored.
                if (((all && it.CanWrite) || attr.Length != 0) &&
                        it.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), true).Length == 0)
                {
                    if (!list.ContainsKey(it.Name) && it.Name != "Capacity")
                    {
                        s = new GXSerializedItem();
                        s.Target = it;
                        s.Type = it.PropertyType;
                        string name;
                        if (attr.Length == 0 || string.IsNullOrEmpty(attr[0].Name))
                        {
                            name = it.Name;
                        }
                        else
                        {
                            name = attr[0].Name;
                        }
                        if (attributeUpdater != null)
                        {
                            attributeUpdater(type, it.GetCustomAttributes(true), s);
                        }
                        if ((s.Attributes & Attributes.Ignored) == 0)
                        {
                            if (!it.PropertyType.IsArray)
                            {
#if !__IOS__
                                s.Get = GXInternal.CreateGetHandler(it.PropertyType, it);
                                s.Set = GXInternal.CreateSetHandler(it.PropertyType, it);
#endif
                            }
                            list.Add(name, s);
                        }
                    }
                }
            }
            if (!all)
            {
                //Save data members.
                foreach (FieldInfo it in type.GetFields(flags))
                {
                    DataMemberAttribute[] attr = (DataMemberAttribute[])it.GetCustomAttributes(typeof(DataMemberAttribute), true);
                    if (attr.Length != 0)
                    {
                        if (!list.ContainsKey(it.Name))
                        {
                            s = new GXSerializedItem();
                            s.Target = it;
                            s.Type = it.FieldType;
                            string name;
                            if (attr.Length == 0 || string.IsNullOrEmpty(attr[0].Name))
                            {
                                name = it.Name;
                            }
                            else
                            {
                                name = attr[0].Name;
                            }
                            if (attributeUpdater != null)
                            {
                                attributeUpdater(type, it.GetCustomAttributes(true), s);
                            }
                            if ((s.Attributes & Attributes.Ignored) == 0)
                            {
                                if (!it.FieldType.IsArray)
                                {
#if !__IOS__
                                    s.Get = GXInternal.CreateGetHandler(it.FieldType, it);
                                    s.Set = GXInternal.CreateSetHandler(it.FieldType, it);
#endif
                                }
                                list.Add(name, s);
                            }
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Change DB value type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="utc"></param>
        /// <returns></returns>
        static public object ChangeType(object value, Type type, bool utc)
        {
            if (value == null || value is DBNull)
            {
                return null;
            }
            if (type == typeof(byte[]))
            {
                if (value is string)
                {
                    return GXCommon.HexToBytes((string)value);
                }
                return GXCommon.HexToBytes(ASCIIEncoding.ASCII.GetString((byte[])value));
            }
            //Date times are saved in UTC format.
            if (type == typeof(DateTime))
            {
                DateTime dt = (DateTime)Convert.ChangeType(value, type);
                if (dt == DateTime.MinValue)
                {
                    return dt;
                }
                //Milliseconst are not saved.
                if (dt.Ticks == DateTime.MaxValue.AddTicks(-9999999).Ticks)
                {
                    return DateTime.MaxValue;
                }
                if (utc)
                {
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                }
                else
                {
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
                }
                return dt.ToLocalTime();
            }
            if (value.GetType() == type)
            {
                return value;
            }
            if (type == typeof(Guid))
            {
                if (value is string)
                {
                    Guid g = new Guid((string)value);
                    return g;
                }
            }
            else if (type.IsEnum)
            {
                if (value is string)
                {
                    return Enum.Parse(type, (string)value);
                }
                return Enum.Parse(type, value.ToString());
            }
            else if (type == typeof(System.Decimal))
            {
                if (Convert.ToDouble(value) == -7.9228162514264338E+28)
                {
                    return System.Decimal.MinValue;
                }
                if (Convert.ToDouble(value) == 7.9228162514264338E+28)
                {
                    return System.Decimal.MaxValue;
                }
                Convert.ToDecimal(value);
            }
            else if (type == typeof(Int64))
            {
                if (value is double)
                {
                    if ((double)value == 9.2233720368547758E+18)
                    {
                        return Int64.MaxValue;
                    }
                }
            }
            else if (type == typeof(UInt64))
            {
                if (value is double)
                {
                    if ((double)value == 1.8446744073709552E+19)
                    {
                        return UInt64.MaxValue;
                    }
                }
            }
            else if (type == typeof(TimeSpan))
            {
                return new TimeSpan(Convert.ToInt64(value) * 10000);
            }
            else if (type == typeof(DateTimeOffset))
            {
                DateTime dt = (DateTime)Convert.ChangeType(value, typeof(DateTime));
                if (utc)
                {
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                }
                else
                {
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
                }
                return new DateTimeOffset(dt.ToLocalTime());
            }
            //If nullable.
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                {
                    return null;
                }
                Type tp = Nullable.GetUnderlyingType(type);
                //Date times are saved in UTC format.
                if (tp == typeof(DateTime))
                {
                    DateTime dt = (DateTime)Convert.ChangeType(value, tp);
                    if (utc)
                    {
                        dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    }
                    else
                    {
                        dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
                    }
                    return dt.ToLocalTime();
                }
                if (value.GetType() != tp)
                {
                    return ChangeType(value, tp, utc);
                }
                return value;
            }
            if (type.IsArray)
            {
                Type pt = GXInternal.GetPropertyType(type);
                string[] tmp = ((string)value).Split(new char[] { ';' });
                Array items = Array.CreateInstance(pt, tmp.Length);
                int pos2 = -1;
                foreach (string it in tmp)
                {
                    items.SetValue(GXInternal.ChangeType(it, pt, utc), ++pos2);
                }
                return items;
            }
            if (type == typeof(Type))
            {
                return Type.GetType(value.ToString());
            }
            return Convert.ChangeType(value, type);
        }

        internal static object ShouldSerializeValue(object value)
        {
            //If value is nullable.
            if (value == null)
            {
                return null;
            }
            if (value is sbyte)
            {
                if ((sbyte)value == 0)
                {
                    return null;
                }
            }
            else if (value is Int16)
            {
                if ((Int16)value == 0)
                {
                    return null;
                }
            }
            else if (value is Int32)
            {
                if ((Int32)value == 0)
                {
                    return null;
                }
            }
            else if (value is Int64)
            {
                if ((Int64)value == 0)
                {
                    return null;
                }
            }
            if (value is byte)
            {
                if ((byte)value == 0)
                {
                    return null;
                }
            }
            else if (value is UInt16)
            {
                if ((UInt16)value == 0)
                {
                    return null;
                }
            }
            else if (value is UInt32)
            {
                if ((UInt32)value == 0)
                {
                    return null;
                }
            }
            else if (value is UInt64)
            {
                if ((UInt64)value == 0)
                {
                    return null;
                }
            }
            else if (value is double)
            {
                if ((double)value == 0)
                {
                    return null;
                }
            }
            else if (value is float)
            {
                if ((float)value == 0)
                {
                    return null;
                }
            }
            else if (value is bool)
            {
                if (!(bool)value)
                {
                    return null;
                }
            }
            else if (value is string)
            {
                if (string.IsNullOrEmpty((string)value))
                {
                    return null;
                }
            }
            else if (value is DateTime)
            {
                if ((DateTime)value == DateTime.MinValue)
                {
                    return null;
                }
            }
            else if (value is TimeSpan)
            {
                if ((TimeSpan)value == TimeSpan.Zero)
                {
                    return null;
                }
            }
            else if (value is Guid)
            {
                if ((Guid)value == Guid.Empty)
                {
                    return null;
                }
            }
            else if (value is Type)
            {
                return value;
            }
            else if (value is System.Collections.IEnumerable)
            {
                //If collection is empty.
                if (!(value as System.Collections.IEnumerable).GetEnumerator().MoveNext())
                {
                    return null;
                }
            }
            else if (value is object)
            {
                if (value.GetType() == typeof(object))
                {
                    return null;
                }
            }
            return value;
        }

        internal static bool IsGenericDataType(Type type)
        {
            //If nullable.
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }
            return type.IsPrimitive || type.IsEnum || type == typeof(Guid) || type == typeof(DateTime) ||
                   type == typeof(string) || type == typeof(Type) || type == typeof(object) ||
                   type == typeof(decimal) || type == typeof(TimeSpan) || type == typeof(DateTimeOffset);
        }

        internal static Type GetPropertyType(Type target)
        {
            if (target == typeof(object))
            {
                return target;
            }
            if (target.IsArray)
            {
                return target.GetElementType();
            }
            Type[] types = target.GetGenericArguments();
            if (types.Length == 0)
            {
                if (target.BaseType == typeof(object))
                {
                    return target;
                }
                return GetPropertyType(target.BaseType);
            }
            return types[0];
        }

        /// <summary>
        /// Convert date time to epoch string.
        /// </summary>
        /// <param name="dt">Date time to convert.</param>
        /// <param name="get">Is this http get request.</param>
        /// <returns>Date time as epoch string.</returns>
        public static string ToString(DateTime dt, bool get)
        {
            double offset = 0;
            if (get && dt.Kind == DateTimeKind.Local)
            {
                dt = dt.ToUniversalTime();
            }
            else
            {
                if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                {
                    offset = TimeZone.CurrentTimeZone.GetUtcOffset(dt).TotalMinutes;
                }
            }
            long value = (long)(dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, dt.Kind)).TotalMilliseconds;
            if (offset != 0)
            {
                string str;
                if (get)
                {
                    str = "/Date(" + value.ToString();
                }
                else
                {
                    str = "\"\\/Date(" + value.ToString();
                }
                if (offset > 0)
                {
                    str += "+";
                }
                else
                {
                    str += "-";
                }
                str += TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours.ToString("00") +
                       TimeZone.CurrentTimeZone.GetUtcOffset(dt).Minutes.ToString("00");
                if (get)
                {
                    str += ")/";
                }
                else
                {
                    str += ")\\/\"";
                }
                return str;
            }
            if (get)
            {
                return "/Date(" + value.ToString() + ")/";
            }
            return "\"\\/Date(" + value.ToString() + ")\\/\"";
        }
    }
}