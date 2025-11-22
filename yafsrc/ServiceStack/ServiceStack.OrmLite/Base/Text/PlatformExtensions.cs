// ***********************************************************************
// <copyright file="PlatformExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

using ServiceStack.OrmLite.Base.Common;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class PlatformExtensions.
/// </summary>
public static class PlatformExtensions
{
    /// <param name="subType">Type of the sub.</param>
    extension(Type subType)
    {
        /// <summary>
        /// Gets the types public properties.
        /// </summary>
        /// <returns>PropertyInfo[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PropertyInfo[] GetTypesPublicProperties()
        {
            return subType.GetProperties(
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.Instance);
        }

        /// <summary>
        /// Gets the types properties.
        /// </summary>
        /// <returns>PropertyInfo[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PropertyInfo[] GetTypesProperties()
        {
            return subType.GetProperties(
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);
        }

        /// <summary>
        /// Fieldses the specified type.
        /// </summary>
        /// <returns>FieldInfo[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldInfo[] Fields()
        {
            return subType.GetFields(
                BindingFlags.FlattenHierarchy |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic);
        }

        /// <summary>
        /// Gets all fields.
        /// </summary>
        /// <returns>FieldInfo[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldInfo[] GetAllFields()
        {
            return subType.IsInterface ? TypeConstants.EmptyFieldInfoArray : subType.Fields();
        }

        /// <summary>
        /// Gets the public fields.
        /// </summary>
        /// <returns>FieldInfo[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldInfo[] GetPublicFields()
        {
            return subType.IsInterface
                ? TypeConstants.EmptyFieldInfoArray
                : CollectionExtensions.ToArray(subType.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public |
                                                                 BindingFlags.Instance));
        }

        /// <summary>
        /// Gets the public members.
        /// </summary>
        /// <returns>MemberInfo[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemberInfo[] GetPublicMembers()
        {
            return subType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// Gets all public members.
        /// </summary>
        /// <returns>MemberInfo[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemberInfo[] GetAllPublicMembers()
        {
            return subType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }

        /// <summary>
        /// Gets the static method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>MethodInfo.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MethodInfo GetStaticMethod(string methodName)
        {
            return subType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Gets the instance method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>MethodInfo.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MethodInfo GetInstanceMethod(string methodName)
        {
            return subType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Determines whether the specified type has attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><c>true</c> if the specified type has attribute; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasAttribute<T>()
        {
            return subType.AllAttributes().Exists(x => x.GetType() == typeof(T));
        }
    }

    /// <summary>
    /// Determines whether the specified pi has attribute.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pi">The pi.</param>
    /// <returns><c>true</c> if the specified pi has attribute; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAttribute<T>(this PropertyInfo pi)
    {
        return pi.AllAttributes().Exists(x => x.GetType() == typeof(T));
    }

    /// <summary>
    /// Determines whether the specified fi has attribute.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fi">The fi.</param>
    /// <returns><c>true</c> if the specified fi has attribute; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAttribute<T>(this FieldInfo fi)
    {
        return fi.AllAttributes().Exists(x => x.GetType() == typeof(T));
    }

    /// <summary>
    /// The has attribute cache
    /// </summary>
    private readonly static ConcurrentDictionary<Tuple<MemberInfo, Type>, bool> hasAttributeCache = new();
    /// <summary>
    /// Determines whether [has attribute cached] [the specified member information].
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="memberInfo">The member information.</param>
    /// <returns><c>true</c> if [has attribute cached] [the specified member information]; otherwise, <c>false</c>.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public static bool HasAttributeCached<T>(this MemberInfo memberInfo)
    {
        var key = new Tuple<MemberInfo, Type>(memberInfo, typeof(T));
        if (hasAttributeCache.TryGetValue(key, out var hasAttr))
        {
            return hasAttr;
        }

        hasAttr = memberInfo is Type t
                      ? t.AllAttributes().Exists(x => x.GetType() == typeof(T))
                      : memberInfo is PropertyInfo pi
                          ? pi.AllAttributes().Exists(x => x.GetType() == typeof(T))
                          : memberInfo is FieldInfo fi
                              ? fi.AllAttributes().Exists(x => x.GetType() == typeof(T))
                              : memberInfo is MethodInfo mi
                                  ? mi.AllAttributes().Exists(x => x.GetType() == typeof(T))
                                  : throw new NotSupportedException(memberInfo.GetType().Name);

        hasAttributeCache[key] = hasAttr;

        return hasAttr;
    }

    /// <summary>
    /// Determines whether [has attribute named] [the specified name].
    /// </summary>
    /// <param name="pi">The pi.</param>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if [has attribute named] [the specified name]; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAttributeNamed(this PropertyInfo pi, string name)
    {
        var normalizedAttr = name.Replace("Attribute", "").ToLower();
        return pi.AllAttributesLazy().Any(x => x.GetType().Name.Replace("Attribute", "").ToLower() == normalizedAttr);
    }

    /// <summary>
    /// The data contract
    /// </summary>
    private const string DataContract = "DataContractAttribute";

    /// <summary>
    /// Determines whether the specified type is dto.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if the specified type is dto; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDto(this Type type)
    {
        if (type == null)
        {
            return false;
        }

        return !Env.IsMono
                   ? type.HasAttribute<DataContractAttribute>()
                   : type.GetCustomAttributes(true).Exists(x => x.GetType().Name == DataContract);
    }

    //Should only register Runtime Attributes on StartUp, So using non-ThreadSafe Dictionary is OK
    /// <summary>
    /// The property attributes map
    /// </summary>
    private static Dictionary<string, List<Attribute>> propertyAttributesMap = [];

    /// <summary>
    /// The type attributes map
    /// </summary>
    private static Dictionary<Type, List<Attribute>> typeAttributesMap = [];

    /// <summary>
    /// Clears the runtime attributes.
    /// </summary>
    public static void ClearRuntimeAttributes()
    {
        propertyAttributesMap = [];
        typeAttributesMap = [];
    }

    /// <summary>
    /// Uniques the key.
    /// </summary>
    /// <param name="pi">The pi.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.ArgumentException">Property '{0}' has no DeclaringType".Fmt(pi.Name)</exception>
    static internal string UniqueKey(this PropertyInfo pi)
    {
        if (pi.DeclaringType == null)
        {
            throw new ArgumentException("Property '{0}' has no DeclaringType".Fmt(pi.Name));
        }

        return pi.DeclaringType.Namespace + "." + pi.DeclaringType.Name + "." + pi.Name;
    }

    /// <summary>
    /// Adds the attributes.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="attrs">The attrs.</param>
    /// <returns>Type.</returns>
    public static Type AddAttributes(this Type type, params Attribute[] attrs)
    {
        if (!typeAttributesMap.TryGetValue(type, out var typeAttrs))
        {
            typeAttributesMap[type] = typeAttrs = [];
        }

        typeAttrs.AddRange(attrs);
        return type;
    }

    /// <param name="propertyInfo">The property information.</param>
    extension(PropertyInfo propertyInfo)
    {
        /// <summary>
        /// Add a Property attribute at runtime.
        /// <para>Not threadsafe, should only add attributes on Startup.</para>
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>PropertyInfo.</returns>
        public PropertyInfo AddAttributes(params Attribute[] attrs)
        {
            var key = propertyInfo.UniqueKey();
            if (!propertyAttributesMap.TryGetValue(key, out var propertyAttrs))
            {
                propertyAttributesMap[key] = propertyAttrs = [];
            }

            propertyAttrs.AddRange(attrs);

            return propertyInfo;
        }

        /// <summary>
        /// Add a Property attribute at runtime.
        /// <para>Not threadsafe, should only add attributes on Startup.</para>
        /// </summary>
        /// <param name="attr">The attribute.</param>
        /// <returns>PropertyInfo.</returns>
        public PropertyInfo ReplaceAttribute(Attribute attr)
        {
            var key = propertyInfo.UniqueKey();

            if (!propertyAttributesMap.TryGetValue(key, out var propertyAttrs))
            {
                propertyAttributesMap[key] = propertyAttrs = [];
            }

            propertyAttrs.RemoveAll(x => x.GetType() == attr.GetType());

            propertyAttrs.Add(attr);

            return propertyInfo;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="TAttr">The type of the t attribute.</typeparam>
        /// <returns>List&lt;TAttr&gt;.</returns>
        public List<TAttr> GetAttributes<TAttr>()
        {
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out var propertyAttrs)
                ? []
                : [.. propertyAttrs.OfType<TAttr>()];
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <returns>List&lt;Attribute&gt;.</returns>
        public List<Attribute> GetAttributes()
        {
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out var propertyAttrs)
                ? []
                : [.. propertyAttrs];
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns>List&lt;Attribute&gt;.</returns>
        public List<Attribute> GetAttributes(Type attrType)
        {
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out var propertyAttrs)
                ? []
                : [.. propertyAttrs.Where(x => attrType.IsInstanceOf(x.GetType()))];
        }

        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <returns>System.Object[].</returns>
        public object[] AllAttributes()
        {
            var attrs = propertyInfo.GetCustomAttributes(true);
            var runtimeAttrs = propertyInfo.GetAttributes();
            if (runtimeAttrs.Count == 0)
            {
                return attrs;
            }

            runtimeAttrs.AddRange(attrs.Cast<Attribute>());
            return [.. runtimeAttrs.Cast<object>()];
        }

        /// <summary>
        /// Alls the attributes lazy.
        /// </summary>
        /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
        public IEnumerable<object> AllAttributesLazy()
        {
            var attrs = propertyInfo.GetCustomAttributes(true);
            var runtimeAttrs = propertyInfo.GetAttributes();
            foreach (var attr in runtimeAttrs)
            {
                yield return attr;
            }
            foreach (var attr in attrs)
            {
                yield return attr;
            }
        }

        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns>System.Object[].</returns>
        public object[] AllAttributes(Type attrType)
        {
            var attrs = propertyInfo.GetCustomAttributes(attrType, true);
            var runtimeAttrs = propertyInfo.GetAttributes(attrType);
            if (runtimeAttrs.Count == 0)
            {
                return attrs;
            }

            runtimeAttrs.AddRange(attrs.Cast<Attribute>());
            return [.. runtimeAttrs.Cast<object>()];
        }

        /// <summary>
        /// Alls the attributes lazy.
        /// </summary>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
        public IEnumerable<object> AllAttributesLazy(Type attrType)
        {
            foreach (var attr in propertyInfo.GetAttributes(attrType))
            {
                yield return attr;
            }
            foreach (var attr in propertyInfo.GetCustomAttributes(attrType, true))
            {
                yield return attr;
            }
        }
    }

    /// <summary>
    /// Alls the attributes.
    /// </summary>
    /// <param name="paramInfo">The parameter information.</param>
    /// <returns>System.Object[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object[] AllAttributes(this ParameterInfo paramInfo)
    {
        return paramInfo.GetCustomAttributes(true);
    }

    /// <summary>
    /// Alls the attributes.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <returns>System.Object[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object[] AllAttributes(this FieldInfo fieldInfo)
    {
        return fieldInfo.GetCustomAttributes(true);
    }

    /// <summary>
    /// Alls the attributes.
    /// </summary>
    /// <param name="memberInfo">The member information.</param>
    /// <returns>System.Object[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object[] AllAttributes(this MemberInfo memberInfo)
    {
        return memberInfo.GetCustomAttributes(true);
    }

    /// <summary>
    /// Alls the attributes.
    /// </summary>
    /// <param name="paramInfo">The parameter information.</param>
    /// <param name="attrType">Type of the attribute.</param>
    /// <returns>System.Object[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object[] AllAttributes(this ParameterInfo paramInfo, Type attrType)
    {
        return paramInfo.GetCustomAttributes(attrType, true);
    }

    /// <summary>
    /// Alls the attributes.
    /// </summary>
    /// <param name="memberInfo">The member information.</param>
    /// <param name="attrType">Type of the attribute.</param>
    /// <returns>System.Object[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object[] AllAttributes(this MemberInfo memberInfo, Type attrType)
    {
        var prop = memberInfo as PropertyInfo;
        return prop != null
                   ? prop.AllAttributes(attrType)
                   : memberInfo.GetCustomAttributes(attrType, true);
    }

    /// <summary>
    /// Alls the attributes.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <param name="attrType">Type of the attribute.</param>
    /// <returns>System.Object[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object[] AllAttributes(this FieldInfo fieldInfo, Type attrType)
    {
        return fieldInfo.GetCustomAttributes(attrType, true);
    }

    /// <summary>
    /// Alls the attributes.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>System.Object[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object[] AllAttributes(this Type type)
    {
        return [.. type.GetCustomAttributes(true).Union(type.GetRuntimeAttributes())];
    }

    /// <summary>
    /// Alls the attributes.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns>System.Object[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object[] AllAttributes(this Assembly assembly)
    {
        return CollectionExtensions.ToArray(assembly.GetCustomAttributes(true));
    }

    /// <summary>
    /// Alls the attributes.
    /// </summary>
    /// <typeparam name="TAttr">The type of the t attribute.</typeparam>
    /// <param name="pi">The pi.</param>
    /// <returns>TAttr[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAttr[] AllAttributes<TAttr>(this ParameterInfo pi)
    {
        return [.. pi.AllAttributes(typeof(TAttr)).Cast<TAttr>()];
    }

    /// <summary>
    /// Alls the attributes.
    /// </summary>
    /// <typeparam name="TAttr">The type of the t attribute.</typeparam>
    /// <param name="mi">The mi.</param>
    /// <returns>TAttr[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAttr[] AllAttributes<TAttr>(this MemberInfo mi)
    {
        return [.. mi.AllAttributes(typeof(TAttr)).Cast<TAttr>()];
    }

    /// <summary>
    /// Alls the attributes.
    /// </summary>
    /// <typeparam name="TAttr">The type of the t attribute.</typeparam>
    /// <param name="fi">The fi.</param>
    /// <returns>TAttr[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAttr[] AllAttributes<TAttr>(this FieldInfo fi)
    {
        return [.. fi.AllAttributes(typeof(TAttr)).Cast<TAttr>()];
    }

    /// <param name="pi">The pi.</param>
    extension(PropertyInfo pi)
    {
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <typeparam name="TAttr">The type of the t attribute.</typeparam>
        /// <returns>TAttr[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TAttr[] AllAttributes<TAttr>()
        {
            return [.. pi.AllAttributes(typeof(TAttr)).Cast<TAttr>()];
        }

        /// <summary>
        /// Alls the attributes lazy.
        /// </summary>
        /// <typeparam name="TAttr">The type of the t attribute.</typeparam>
        /// <returns>IEnumerable&lt;TAttr&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<TAttr> AllAttributesLazy<TAttr>()
        {
            return pi.AllAttributesLazy(typeof(TAttr)).Cast<TAttr>();
        }
    }

    /// <param name="type">The type.</param>
    extension(Type type)
    {
        /// <summary>
        /// Gets the runtime attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<T> GetRuntimeAttributes<T>()
        {
            return typeAttributesMap.TryGetValue(type, out var attrs)
                ? attrs.OfType<T>()
                : [];
        }

        /// <summary>
        /// Gets the runtime attributes.
        /// </summary>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns>IEnumerable&lt;Attribute&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<Attribute> GetRuntimeAttributes(Type attrType = null)
        {
            return typeAttributesMap.TryGetValue(type, out var attrs)
                ? attrs.Where(x => attrType == null || attrType.IsInstanceOf(x.GetType()))
                : [];
        }

        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <typeparam name="TAttr">The type of the t attribute.</typeparam>
        /// <returns>TAttr[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TAttr[] AllAttributes<TAttr>()
        {
            return [.. type.GetCustomAttributes(typeof(TAttr), true)
                .OfType<TAttr>()
                .Union(type.GetRuntimeAttributes<TAttr>())];
        }

        /// <summary>
        /// Firsts the attribute.
        /// </summary>
        /// <typeparam name="TAttr">The type of the t attribute.</typeparam>
        /// <returns>TAttr.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TAttr FirstAttribute<TAttr>() where TAttr : class
        {
            return (TAttr)type.GetCustomAttributes(typeof(TAttr), true)
                       .FirstOrDefault()
                   ?? type.GetRuntimeAttributes<TAttr>().FirstOrDefault();
        }
    }

    /// <summary>
    /// Firsts the attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the t attribute.</typeparam>
    /// <param name="memberInfo">The member information.</param>
    /// <returns>TAttribute.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAttribute FirstAttribute<TAttribute>(this MemberInfo memberInfo)
    {
        return memberInfo.AllAttributes<TAttribute>().FirstOrDefault();
    }

    /// <summary>
    /// Firsts the attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the t attribute.</typeparam>
    /// <param name="paramInfo">The parameter information.</param>
    /// <returns>TAttribute.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAttribute FirstAttribute<TAttribute>(this ParameterInfo paramInfo)
    {
        return paramInfo.AllAttributes<TAttribute>().FirstOrDefault();
    }

    /// <summary>
    /// Firsts the attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the t attribute.</typeparam>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>TAttribute.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAttribute FirstAttribute<TAttribute>(this PropertyInfo propertyInfo)
    {
        return propertyInfo.AllAttributesLazy<TAttribute>().FirstOrDefault();
    }

    /// <param name="type">The type.</param>
    extension(Type type)
    {
        /// <summary>
        /// Gets the static method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="types">The types.</param>
        /// <returns>MethodInfo.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MethodInfo GetStaticMethod(string methodName, Type[] types)
        {
            return types == null
                ? type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
                : type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, types, null);
        }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="types">The types.</param>
        /// <returns>MethodInfo.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MethodInfo GetMethodInfo(string methodName, Type[] types = null)
        {
            return types == null
                ? type.GetMethod(methodName)
                : type.GetMethod(methodName, types);
        }

        /// <summary>
        /// Gets the public static field.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>FieldInfo.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldInfo GetPublicStaticField(string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
        }
    }

    /// <summary>
    /// Makes the delegate.
    /// </summary>
    /// <param name="mi">The mi.</param>
    /// <param name="delegateType">Type of the delegate.</param>
    /// <param name="throwOnBindFailure">if set to <c>true</c> [throw on bind failure].</param>
    /// <returns>Delegate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Delegate MakeDelegate(this MethodInfo mi, Type delegateType, bool throwOnBindFailure = true)
    {
        return Delegate.CreateDelegate(delegateType, mi, throwOnBindFailure);
    }

    /// <param name="type">The type.</param>
    extension(Type type)
    {
        /// <summary>
        /// Determines whether [is standard class] [the specified type].
        /// </summary>
        /// <returns><c>true</c> if [is standard class] [the specified type]; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsStandardClass()
        {
            return type.IsClass && !type.IsAbstract && !type.IsInterface;
        }

        /// <summary>
        /// Gets the writable fields.
        /// </summary>
        /// <returns>FieldInfo[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldInfo[] GetWritableFields()
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField);
        }

        /// <summary>
        /// Determines whether [is enum flags] [the specified type].
        /// </summary>
        /// <returns><c>true</c> if [is enum flags] [the specified type]; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnumFlags()
        {
            return type.IsEnum && type.FirstAttribute<FlagsAttribute>() != null;
        }

        /// <summary>
        /// Gets the instance methods.
        /// </summary>
        /// <returns>MethodInfo[].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MethodInfo[] GetInstanceMethods()
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Gets the name of the declaring type.
        /// </summary>
        /// <returns>System.String.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetDeclaringTypeName()
        {
            if (type.DeclaringType != null)
            {
                return type.DeclaringType.Name;
            }

            if (type.ReflectedType != null)
            {
                return type.ReflectedType.Name;
            }

            return null;
        }
    }

    /// <summary>
    /// Gets the name of the declaring type.
    /// </summary>
    /// <param name="mi">The mi.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDeclaringTypeName(this MemberInfo mi)
    {
        if (mi.DeclaringType != null)
        {
            return mi.DeclaringType.Name;
        }

        return mi.ReflectedType.Name;
    }

    /// <param name="methodInfo">The method information.</param>
    extension(MethodInfo methodInfo)
    {
        /// <summary>
        /// Creates the delegate.
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <returns>Delegate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Delegate CreateDelegate(Type delegateType)
        {
            return Delegate.CreateDelegate(delegateType, methodInfo);
        }

        /// <summary>
        /// Creates the delegate.
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="target">The target.</param>
        /// <returns>Delegate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Delegate CreateDelegate(Type delegateType, object target)
        {
            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }
    }

    /// <summary>
    /// Gets the type of the collection.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Type.</returns>
    public static Type GetCollectionType(this Type type)
    {
        return type.GetElementType()
               ?? type.GetGenericArguments().LastOrDefault() //new[] { str }.Select(x => new Type()) => WhereSelectArrayIterator<string,Type>
               ?? (type.BaseType != null && type.BaseType != typeof(object) ? type.BaseType.GetCollectionType() : null); //e.g. ArrayOfString : List<string>
    }

    /// <summary>
    /// The generic type cache
    /// </summary>
    private static Dictionary<string, Type> GenericTypeCache = [];

    /// <summary>
    /// Gets the type of the cached generic.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="argTypes">The argument types.</param>
    /// <returns>Type.</returns>
    /// <exception cref="System.ArgumentException"></exception>
    public static Type GetCachedGenericType(this Type type, params Type[] argTypes)
    {
        if (!type.IsGenericTypeDefinition)
        {
            throw new ArgumentException(type.FullName + " is not a Generic Type Definition");
        }

        argTypes ??= TypeConstants.EmptyTypeArray;

        var sb = StringBuilderThreadStatic.Allocate()
            .Append(type.FullName);

        foreach (var argType in argTypes)
        {
            sb.Append('|')
                .Append(argType.FullName);
        }

        var key = StringBuilderThreadStatic.ReturnAndFree(sb);

        if (GenericTypeCache.TryGetValue(key, out var genericType))
        {
            return genericType;
        }

        genericType = type.MakeGenericType(argTypes);

        Dictionary<string, Type> snapshot, newCache;
        do
        {
            snapshot = GenericTypeCache;
            newCache = new Dictionary<string, Type>(GenericTypeCache)
                           {
                               [key] = genericType
                           };

        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref GenericTypeCache, newCache, snapshot), snapshot));

        return genericType;
    }

    /// <summary>
    /// To object map cache
    /// </summary>
    private readonly static ConcurrentDictionary<Type, ObjectDictionaryDefinition> toObjectMapCache =
        new();

    /// <summary>
    /// Class ObjectDictionaryDefinition.
    /// </summary>
    internal class ObjectDictionaryDefinition
    {
        /// <summary>
        /// The type
        /// </summary>
        public Type Type;
        /// <summary>
        /// The fields
        /// </summary>
        public readonly List<ObjectDictionaryFieldDefinition> Fields = [];
        /// <summary>
        /// The fields map
        /// </summary>
        public readonly Dictionary<string, ObjectDictionaryFieldDefinition> FieldsMap = [];

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fieldDef">The field definition.</param>
        public void Add(string name, ObjectDictionaryFieldDefinition fieldDef)
        {
            this.Fields.Add(fieldDef);
            this.FieldsMap[name] = fieldDef;
        }
    }

    /// <summary>
    /// Class ObjectDictionaryFieldDefinition.
    /// </summary>
    internal class ObjectDictionaryFieldDefinition
    {
        /// <summary>
        /// The name
        /// </summary>
        public string Name;
        /// <summary>
        /// The type
        /// </summary>
        public Type Type;

        /// <summary>
        /// The get value function
        /// </summary>
        public GetMemberDelegate GetValueFn;
        /// <summary>
        /// The set value function
        /// </summary>
        public SetMemberDelegate SetValueFn;

        /// <summary>
        /// The convert type
        /// </summary>
        public Type ConvertType;
        /// <summary>
        /// The convert value function
        /// </summary>
        public GetMemberDelegate ConvertValueFn;

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object instance, object value)
        {
            var lockObj = new object();

            if (this.SetValueFn == null)
            {
                return;
            }

            if (this.Type != typeof(object))
            {
                if (value is IEnumerable<KeyValuePair<string, object>> dictionary)
                {
                    value = dictionary.FromObjectDictionary(this.Type);
                }

                if (!this.Type.IsInstanceOfType(value))
                {
                    lock (lockObj)
                    {
                        //Only caches object converter used on first use
                        if (this.ConvertType == null)
                        {
                            this.ConvertType = value.GetType();
                            this.ConvertValueFn = TypeConverter.CreateTypeConverter(this.ConvertType, this.Type);
                        }
                    }

                    if (this.ConvertType.IsInstanceOfType(value))
                    {
                        value = this.ConvertValueFn(value);
                    }
                    else
                    {
                        var tempConvertFn = TypeConverter.CreateTypeConverter(value.GetType(), this.Type);
                        value = tempConvertFn(value);
                    }
                }
            }

            this.SetValueFn(instance, value);
        }
    }

    /// <summary>
    /// Converts to dictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="mapper">The mapper.</param>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    private static Dictionary<string, object> ConvertToDictionary<T>(
        IEnumerable<KeyValuePair<string, T>> collection,
        Func<string, object, object> mapper = null)
    {
        if (mapper != null)
        {
            return mapper.MapToDictionary(collection);
        }

        var to = new Dictionary<string, object>();
        foreach (var entry in collection)
        {
            var key = entry.Key;
            object value = entry.Value;
            to[key] = value;
        }
        return to;
    }

    /// <summary>
    /// Maps to dictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mapper">The mapper.</param>
    /// <param name="collection">The collection.</param>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    private static Dictionary<string, object> MapToDictionary<T>(
        this Func<string, object, object> mapper,
        IEnumerable<KeyValuePair<string, T>> collection)
    {
        return collection.ToDictionary(
            pair => pair.Key,
            pair => mapper(pair.Key, pair.Value));
    }

    /// <param name="obj">The object.</param>
    extension(object obj)
    {
        /// <summary>
        /// Converts to objectdictionary.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public Dictionary<string, object> ToObjectDictionary()
        {
            return ToObjectDictionary(obj, null);
        }

        /// <summary>
        /// Converts to objectdictionary.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public Dictionary<string, object> ToObjectDictionary(Func<string, object, object> mapper)
        {
            switch (obj)
            {
                case null:
                    return null;
                case Dictionary<string, object> alreadyDict when mapper != null:
                    return mapper.MapToDictionary(alreadyDict);
                case Dictionary<string, object> alreadyDict:
                    return alreadyDict;
                case IDictionary<string, object> interfaceDict when mapper != null:
                    return mapper.MapToDictionary(interfaceDict);
                case IDictionary<string, object> interfaceDict:
                    return new Dictionary<string, object>(interfaceDict);
            }

            var to = new Dictionary<string, object>();
            if (obj is Dictionary<string, string> stringDict)
            {
                return ConvertToDictionary(stringDict, mapper);
            }

            if (obj is IDictionary d)
            {
                foreach (var key in d.Keys)
                {
                    var k = key.ToString();
                    var v = d[key];
                    v = mapper?.Invoke(k, v) ?? v;
                    to[k] = v;
                }
                return to;
            }

            if (obj is NameValueCollection nvc)
            {
                for (var i = 0; i < nvc.Count; i++)
                {
                    var k = nvc.GetKey(i);
                    object v = nvc.Get(i);
                    v = mapper?.Invoke(k, v) ?? v;
                    to[k] = v;
                }
                return to;
            }

            if (obj is IEnumerable<KeyValuePair<string, object>> objKvps)
            {
                return ConvertToDictionary(objKvps, mapper);
            }
            if (obj is IEnumerable<KeyValuePair<string, string>> strKvps)
            {
                return ConvertToDictionary(strKvps, mapper);
            }

            var type = obj.GetType();
            if (type.GetKeyValuePairsTypes(out var keyType, out var valueType, out var kvpType) && obj is IEnumerable e)
            {
                var keyGetter = TypeProperties.Get(kvpType).GetPublicGetter("Key");
                var valueGetter = TypeProperties.Get(kvpType).GetPublicGetter("Value");

                foreach (var entry in e)
                {
                    var key = keyGetter(entry);
                    var value = valueGetter(entry);
                    var k = key.ConvertTo<string>();
                    value = mapper?.Invoke(k, value) ?? value;
                    to[k] = value;
                }
                return to;
            }


            if (obj is KeyValuePair<string, object> objKvp)
            {
                var kk = nameof(objKvp.Key);
                object kv = objKvp.Key;
                kv = mapper?.Invoke(kk, kv) ?? kv;

                var vk = nameof(objKvp.Value);
                var vv = objKvp.Value;
                vv = mapper?.Invoke(vk, vv) ?? vv;

                return new Dictionary<string, object>
                {
                    [kk] = kv,
                    [vk] = vv
                };
            }
            if (obj is KeyValuePair<string, string> strKvp)
            {
                var kk = nameof(objKvp.Key);
                object kv = strKvp.Key;
                kv = mapper?.Invoke(kk, kv) ?? kv;

                var vk = nameof(strKvp.Value);
                object vv = strKvp.Value;
                vv = mapper?.Invoke(vk, vv) ?? vv;

                return new Dictionary<string, object>
                {
                    [kk] = kv,
                    [vk] = vv
                };
            }
            if (type.GetKeyValuePairTypes(out _, out var _))
            {
                var kk = "Key";
                object kv = TypeProperties.Get(type).GetPublicGetter("Key")(obj).ConvertTo<string>();
                kv = mapper?.Invoke(kk, kv) ?? kv;

                var vk = "Value";
                var vv = TypeProperties.Get(type).GetPublicGetter("Value")(obj);
                vv = mapper?.Invoke(vk, vv) ?? vv;

                return new Dictionary<string, object>
                {
                    [kk] = kv,
                    [vk] = vv
                };
            }

            if (!toObjectMapCache.TryGetValue(type, out var def))
            {
                toObjectMapCache[type] = def = CreateObjectDictionaryDefinition(type);
            }

            foreach (var fieldDef in def.Fields)
            {
                var k = fieldDef.Name;
                var v = fieldDef.GetValueFn(obj);
                v = mapper?.Invoke(k, v) ?? v;
                to[k] = v;
            }

            return to;
        }
    }

    /// <param name="dictType">Type of the dictionary.</param>
    extension(Type dictType)
    {
        /// <summary>
        /// Gets the key value pairs type definition.
        /// </summary>
        /// <returns>Type.</returns>
        public Type GetKeyValuePairsTypeDef()
        {
            //matches IDictionary<,>, IReadOnlyDictionary<,>, List<KeyValuePair<string, object>>
            var genericDef = dictType.GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));
            if (genericDef == null)
            {
                return null;
            }

            var genericEnumType = genericDef.GetGenericArguments()[0];
            return GetKeyValuePairTypeDef(genericEnumType);
        }

        /// <summary>
        /// Gets the key value pair type definition.
        /// </summary>
        /// <returns>Type.</returns>
        public Type GetKeyValuePairTypeDef()
        {
            return dictType.GetTypeWithGenericTypeDefinitionOf(typeof(KeyValuePair<,>));
        }

        /// <summary>
        /// Gets the key value pairs types.
        /// </summary>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GetKeyValuePairsTypes(out Type keyType, out Type valueType)
        {
            return dictType.GetKeyValuePairsTypes(out keyType, out valueType, out _);
        }

        /// <summary>
        /// Gets the key value pairs types.
        /// </summary>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="kvpType">Type of the KVP.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GetKeyValuePairsTypes(out Type keyType, out Type valueType, out Type kvpType)
        {
            //matches IDictionary<,>, IReadOnlyDictionary<,>, List<KeyValuePair<string, object>>
            var genericDef = dictType.GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));
            if (genericDef != null)
            {
                kvpType = genericDef.GetGenericArguments()[0];
                if (GetKeyValuePairTypes(kvpType, out keyType, out valueType))
                {
                    return true;
                }
            }
            kvpType = keyType = valueType = null;
            return false;
        }

        /// <summary>
        /// Gets the key value pair types.
        /// </summary>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GetKeyValuePairTypes(out Type keyType, out Type valueType)
        {
            var genericKvps = dictType.GetTypeWithGenericTypeDefinitionOf(typeof(KeyValuePair<,>));
            if (genericKvps != null)
            {
                var genericArgs = dictType.GetGenericArguments();
                keyType = genericArgs[0];
                valueType = genericArgs[1];
                return true;
            }

            keyType = valueType = null;
            return false;
        }
    }

    /// <param name="values">The values.</param>
    extension(IEnumerable<KeyValuePair<string, object>> values)
    {
        /// <summary>
        /// Froms the object dictionary.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object FromObjectDictionary(Type type)
        {
            if (values == null)
            {
                return null;
            }

            var alreadyDict = typeof(IEnumerable<KeyValuePair<string, object>>).IsAssignableFrom(type);
            if (alreadyDict)
            {
                return values;
            }

            var to = type.CreateInstance();
            if (to is IDictionary d)
            {
                if (type.GetKeyValuePairsTypes(out var toKeyType, out var toValueType))
                {
                    foreach (var entry in values)
                    {
                        var toKey = entry.Key.ConvertTo(toKeyType);
                        var toValue = entry.Value.ConvertTo(toValueType);
                        d[toKey] = toValue;
                    }
                }
                else
                {
                    foreach (var entry in values)
                    {
                        d[entry.Key] = entry.Value;
                    }
                }
            }
            else
            {
                PopulateInstanceInternal(values, to, type);
            }

            return to;
        }

        /// <summary>
        /// Populates the instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void PopulateInstance(object instance)
        {
            if (values == null || instance == null)
            {
                return;
            }

            PopulateInstanceInternal(values, instance, instance.GetType());
        }
    }

    /// <summary>
    /// Populates the instance internal.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="to">To.</param>
    /// <param name="type">The type.</param>
    private static void PopulateInstanceInternal(IEnumerable<KeyValuePair<string, object>> values, object to, Type type)
    {
        if (!toObjectMapCache.TryGetValue(type, out var def))
        {
            toObjectMapCache[type] = def = CreateObjectDictionaryDefinition(type);
        }

        foreach (var entry in values)
        {
            if (!def.FieldsMap.TryGetValue(entry.Key, out var fieldDef) &&
                !def.FieldsMap.TryGetValue(entry.Key.ToPascalCase(), out fieldDef)
                || entry.Value == null
                || entry.Value == DBNull.Value)
            {
                continue;
            }

            fieldDef.SetValue(to, entry.Value);
        }
    }

    /// <summary>
    /// Populates the instance.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="instance">The instance.</param>
    public static void PopulateInstance(this IEnumerable<KeyValuePair<string, string>> values, object instance)
    {
        if (values == null || instance == null)
        {
            return;
        }

        PopulateInstanceInternal(values, instance, instance.GetType());
    }

    /// <summary>
    /// Populates the instance internal.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="to">To.</param>
    /// <param name="type">The type.</param>
    private static void PopulateInstanceInternal(IEnumerable<KeyValuePair<string, string>> values, object to, Type type)
    {
        if (!toObjectMapCache.TryGetValue(type, out var def))
        {
            toObjectMapCache[type] = def = CreateObjectDictionaryDefinition(type);
        }

        foreach (var entry in values)
        {
            if (!def.FieldsMap.TryGetValue(entry.Key, out var fieldDef) &&
                !def.FieldsMap.TryGetValue(entry.Key.ToPascalCase(), out fieldDef)
                || entry.Value == null)
            {
                continue;
            }

            fieldDef.SetValue(to, entry.Value);
        }
    }

    /// <summary>
    /// Froms the object dictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values.</param>
    /// <returns>T.</returns>
    public static T FromObjectDictionary<T>(this IEnumerable<KeyValuePair<string, object>> values)
    {
        return (T)values.FromObjectDictionary(typeof(T));
    }

    /// <summary>
    /// Creates the object dictionary definition.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ObjectDictionaryDefinition.</returns>
    private static ObjectDictionaryDefinition CreateObjectDictionaryDefinition(Type type)
    {
        var def = new ObjectDictionaryDefinition
                      {
                          Type = type
                      };

        foreach (var pi in type.GetSerializableProperties())
        {
            def.Add(pi.Name, new ObjectDictionaryFieldDefinition
                                 {
                                     Name = pi.Name,
                                     Type = pi.PropertyType,
                                     GetValueFn = pi.CreateGetter(),
                                     SetValueFn = pi.CreateSetter()
                                 });
        }

        if (JsConfig.IncludePublicFields)
        {
            foreach (var fi in type.GetSerializableFields())
            {
                def.Add(fi.Name, new ObjectDictionaryFieldDefinition
                                     {
                                         Name = fi.Name,
                                         Type = fi.FieldType,
                                         GetValueFn = fi.CreateGetter(),
                                         SetValueFn = fi.CreateSetter()
                                     });
            }
        }
        return def;
    }

    /// <summary>
    /// Converts to safepartialobjectdictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance">The instance.</param>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public static Dictionary<string, object> ToSafePartialObjectDictionary<T>(this T instance)
    {
        var to = new Dictionary<string, object>();
        var propValues = instance.ToObjectDictionary();
        if (propValues != null)
        {
            foreach (var entry in propValues)
            {
                var valueType = entry.Value?.GetType();

                try
                {
                    if (valueType is not {IsClass: true} || valueType == typeof(string))
                    {
                        to[entry.Key] = entry.Value;
                    }
                    else if (!TypeSerializer.HasCircularReferences(entry.Value))
                    {
                        if (entry.Value is IEnumerable enumerable)
                        {
                            to[entry.Key] = entry.Value;
                        }
                        else
                        {
                            to[entry.Key] = entry.Value.ToSafePartialObjectDictionary();
                        }
                    }
                    else
                    {
                        to[entry.Key] = entry.Value.ToString();
                    }

                }
                catch (Exception ignore)
                {
                    Tracer.Instance.WriteDebug($"Could not retrieve value from '{valueType?.GetType().Name}': ${ignore.Message}");
                }
            }
        }
        return to;
    }
}