// ***********************************************************************
// <copyright file="AutoMappingUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class AutoMappingUtils.
/// </summary>
public static class AutoMappingUtils
{
    /// <summary>
    /// The converters
    /// </summary>
    readonly static internal ConcurrentDictionary<Tuple<Type, Type>, GetMemberDelegate> converters
        = new();

    /// <summary>
    /// The populators
    /// </summary>
    readonly static internal ConcurrentDictionary<Tuple<Type, Type>, PopulateMemberDelegate> populators
        = new();

    /// <summary>
    /// The ignore mappings
    /// </summary>
    readonly static internal ConcurrentDictionary<Tuple<Type, Type>, bool> ignoreMappings
        = new();

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public static void Reset()
    {
        converters.Clear();
        populators.Clear();
        ignoreMappings.Clear();
        AssignmentDefinitionCache.Clear();
    }

    /// <summary>
    /// Shoulds the ignore mapping.
    /// </summary>
    /// <param name="fromType">From type.</param>
    /// <param name="toType">To type.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool ShouldIgnoreMapping(Type fromType, Type toType)
    {
        return ignoreMappings.ContainsKey(Tuple.Create(fromType, toType));
    }

    /// <summary>
    /// Gets the converter.
    /// </summary>
    /// <param name="fromType">From type.</param>
    /// <param name="toType">To type.</param>
    /// <returns>GetMemberDelegate.</returns>
    public static GetMemberDelegate GetConverter(Type fromType, Type toType)
    {
        if (converters.IsEmpty)
        {
            return null;
        }

        var key = Tuple.Create(fromType, toType);
        return converters.GetValueOrDefault(key);
    }

    /// <summary>
    /// Gets the populator.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="sourceType">Type of the source.</param>
    /// <returns>PopulateMemberDelegate.</returns>
    public static PopulateMemberDelegate GetPopulator(Type targetType, Type sourceType)
    {
        if (populators.IsEmpty)
        {
            return null;
        }

        var key = Tuple.Create(targetType, sourceType);
        return populators.GetValueOrDefault(key);
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="from">From.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>T.</returns>
    public static T ConvertTo<T>(this object from, T defaultValue)
    {
        return from is null or ""
            ? defaultValue
            : from.ConvertTo<T>();
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="from">From.</param>
    /// <returns>T.</returns>
    public static T ConvertTo<T>(this object from)
    {
        return from.ConvertTo<T>(false);
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="from">From.</param>
    /// <param name="skipConverters">if set to <c>true</c> [skip converters].</param>
    /// <returns>T.</returns>
    public static T ConvertTo<T>(this object from, bool skipConverters)
    {
        if (from == null)
        {
            return default;
        }

        if (from is T t)
        {
            return t;
        }

        return (T)ConvertTo(from, typeof(T), skipConverters);
    }

    /// <summary>
    /// Creates the copy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="from">From.</param>
    /// <returns>T.</returns>
    public static T CreateCopy<T>(this T from)
    {
        if (from == null)
        {
            return from;
        }

        var type = from.GetType();

        if (from is ICloneable clone)
        {
            return (T)clone.Clone();
        }

        if (typeof(T).IsValueType)
        {
            return (T)ChangeValueType(from, type);
        }

        if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            var listResult = TranslateListWithElements.TryTranslateCollections(from.GetType(), type, from);
            return (T)listResult;
        }

        var to = type.CreateInstance<T>();
        return to.PopulateWith(from);
    }

    /// <summary>
    /// Thens the do.
    /// </summary>
    /// <typeparam name="To">The type of to.</typeparam>
    /// <param name="to">To.</param>
    /// <param name="fn">The function.</param>
    /// <returns>To.</returns>
    public static To ThenDo<To>(this To to, Action<To> fn)
    {
        fn(to);
        return to;
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="toType">To type.</param>
    /// <returns>System.Object.</returns>
    public static object ConvertTo(this object from, Type toType)
    {
        return from.ConvertTo(toType, skipConverters: false);
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="toType">To type.</param>
    /// <param name="skipConverters">if set to <c>true</c> [skip converters].</param>
    /// <returns>System.Object.</returns>
    public static object ConvertTo(this object from, Type toType, bool skipConverters)
    {
        if (from == null)
        {
            return null;
        }

        var fromType = from.GetType();
        if (ShouldIgnoreMapping(fromType, toType))
        {
            return null;
        }

        if (!skipConverters)
        {
            var converter = GetConverter(fromType, toType);
            if (converter != null)
            {
                return converter(from);
            }
        }

        if (fromType == toType || toType == typeof(object))
        {
            return from;
        }

        if (fromType.IsValueType || toType.IsValueType)
        {
            return ChangeValueType(from, toType);
        }

        var mi = GetImplicitCastMethod(fromType, toType);
        if (mi != null)
        {
            return mi.Invoke(null, [from]);
        }

        switch (from)
        {
            case string str:
                return TypeSerializer.DeserializeFromString(str, toType);
            case ReadOnlyMemory<char> rom:
                return TypeSerializer.DeserializeFromSpan(toType, rom.Span);
        }

        if (toType == typeof(string))
        {
            return from.ToJsv();
        }

        if (typeof(IEnumerable).IsAssignableFrom(toType))
        {
            var listResult = TryConvertCollections(fromType, toType, from);
            return listResult;
        }

        switch (from)
        {
            case IEnumerable<KeyValuePair<string, object>> objDict:
                return objDict.FromObjectDictionary(toType);
            case IEnumerable<KeyValuePair<string, string>> strDict:
                return strDict.ToObjectDictionary().FromObjectDictionary(toType);
            default:
                {
                    var to = toType.CreateInstance();
                    return to.PopulateWith(from);
                }
        }
    }

    /// <summary>
    /// Gets the implicit cast method.
    /// </summary>
    /// <param name="fromType">From type.</param>
    /// <param name="toType">To type.</param>
    /// <returns>MethodInfo.</returns>
    public static MethodInfo GetImplicitCastMethod(Type fromType, Type toType)
    {
        foreach (var mi in fromType.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (mi.Name == "op_Implicit" && mi.ReturnType == toType &&
                mi.GetParameters().FirstOrDefault()?.ParameterType == fromType)
            {
                return mi;
            }
        }

        return Array.Find(toType.GetMethods(BindingFlags.Public | BindingFlags.Static),
            mi => mi.Name == "op_Implicit" && mi.ReturnType == toType &&
                  mi.GetParameters().FirstOrDefault()?.ParameterType == fromType);
    }

    /// <summary>
    /// Gets the explicit cast method.
    /// </summary>
    /// <param name="fromType">From type.</param>
    /// <param name="toType">To type.</param>
    /// <returns>MethodInfo.</returns>
    public static MethodInfo GetExplicitCastMethod(Type fromType, Type toType)
    {
        foreach (var mi in toType.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (mi.Name == "op_Explicit" && mi.ReturnType == toType &&
                mi.GetParameters().FirstOrDefault()?.ParameterType == fromType)
            {
                return mi;
            }
        }

        return Array.Find(fromType.GetMethods(BindingFlags.Public | BindingFlags.Static),
            mi => mi.Name == "op_Explicit" && mi.ReturnType == toType &&
                  mi.GetParameters().FirstOrDefault()?.ParameterType == fromType);
    }

    /// <summary>
    /// Changes the type of the value.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="toType">To type.</param>
    /// <returns>System.Object.</returns>
    public static object ChangeValueType(object from, Type toType)
    {
        var s = from as string;

        var fromType = from.GetType();
        if (!fromType.IsEnum && !toType.IsEnum)
        {
            var toString = toType == typeof(string);
            if (toType == typeof(char) && s != null)
            {
                return s.Length > 0 ? s[0] : null;
            }

            if (toString && from is char c)
            {
                return c.ToString();
            }

            if (toType == typeof(TimeSpan) && from is long ticks)
            {
                return new TimeSpan(ticks);
            }

            if (toType == typeof(long) && from is TimeSpan time)
            {
                return time.Ticks;
            }

            var destNumberType = DynamicNumber.GetNumber(toType);
            if (destNumberType != null)
            {
                if (s is "")
                {
                    return destNumberType.DefaultValue;
                }

                var value = destNumberType.ConvertFrom(from);
                if (value != null)
                {
                    return toType == typeof(char)
                               ? value.ToString()[0]
                               : value;
                }
            }

            if (toString)
            {
                var srcNumberType = DynamicNumber.GetNumber(from.GetType());
                if (srcNumberType != null)
                {
                    return srcNumberType.ToString(from);
                }
            }
        }

        var mi = GetImplicitCastMethod(fromType, toType);
        if (mi != null)
        {
            return mi.Invoke(null, [from]);
        }

        mi = GetExplicitCastMethod(fromType, toType);
        if (mi != null)
        {
            return mi.Invoke(null, [from]);
        }

        if (s != null)
        {
            return TypeSerializer.DeserializeFromString(s, toType);
        }

        if (toType == typeof(string))
        {
            return from.ToJsv();
        }

        if (toType.HasInterface(typeof(IConvertible)))
        {
            return Convert.ChangeType(from, toType, null);
        }

        var fromKvpType = fromType.GetTypeWithGenericTypeDefinitionOf(typeof(KeyValuePair<,>));
        if (fromKvpType != null)
        {
            var fromProps = TypeProperties.Get(fromKvpType);
            var fromKey = fromProps.GetPublicGetter("Key")(from);
            var fromValue = fromProps.GetPublicGetter("Value")(from);

            var toKvpType = toType.GetTypeWithGenericTypeDefinitionOf(typeof(KeyValuePair<,>));
            if (toKvpType != null)
            {

                var toKvpArgs = toKvpType.GetGenericArguments();
                var toCtor = toKvpType.GetConstructor(toKvpArgs);
                var to = toCtor.Invoke([fromKey.ConvertTo(toKvpArgs[0]), fromValue.ConvertTo(toKvpArgs[1])]);
                return to;
            }

            if (typeof(IDictionary).IsAssignableFrom(toType))
            {
                var genericDef = toType.GetTypeWithGenericTypeDefinitionOf(typeof(IDictionary<,>));
                var toArgs = genericDef.GetGenericArguments();
                var toKeyType = toArgs[0];
                var toValueType = toArgs[1];

                var to = (IDictionary)toType.CreateInstance();
                to["Key"] = fromKey.ConvertTo(toKeyType);
                to["Value"] = fromValue.ConvertTo(toValueType);
                return to;
            }
        }

        return TypeSerializer.DeserializeFromString(from.ToJsv(), toType);
    }

    /// <summary>
    /// Changes to.
    /// </summary>
    /// <param name="strValue">The string value.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public static object ChangeTo(this string strValue, Type type)
    {
        if (type.IsValueType && !type.IsEnum && type.HasInterface(typeof(IConvertible)))
        {
            try
            {
                return Convert.ChangeType(strValue, type, null);
            }
            catch (Exception ex)
            {
                Tracer.Instance.WriteError(ex);
            }
        }
        return TypeSerializer.DeserializeFromString(strValue, type);
    }

    /// <summary>
    /// Populates the object with example data.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="recursionInfo">Tracks how deeply nested we are</param>
    /// <returns>System.Object.</returns>
    private static object PopulateObjectInternal(object obj, Dictionary<Type, int> recursionInfo)
    {
        switch (obj)
        {
            case null:
                return null;
            case string:
                return obj; // prevents it from dropping into the char[] Chars property.
        }

        var type = obj.GetType();

        var members = type.GetPublicMembers();
        foreach (var info in members)
        {
            var fieldInfo = info as FieldInfo;
            var propertyInfo = info as PropertyInfo;
            if (fieldInfo != null || propertyInfo != null)
            {
                var memberType = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                var value = CreateDefaultValue(memberType, recursionInfo);
                SetValue(fieldInfo, propertyInfo, obj, value);
            }
        }
        return obj;
    }

    /// <summary>
    /// The default value types
    /// </summary>
    private static Dictionary<Type, object> DefaultValueTypes = [];

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public static object GetDefaultValue(this Type type)
    {
        if (!type.IsValueType)
        {
            return null;
        }

        if (DefaultValueTypes.TryGetValue(type, out var defaultValue))
        {
            return defaultValue;
        }

        defaultValue = Activator.CreateInstance(type);

        Dictionary<Type, object> snapshot, newCache;
        do
        {
            snapshot = DefaultValueTypes;
            newCache = new Dictionary<Type, object>(DefaultValueTypes) { [type] = defaultValue };

        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref DefaultValueTypes, newCache, snapshot), snapshot));

        return defaultValue;
    }

    /// <summary>
    /// Determines whether [is default value] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if [is default value] [the specified value]; otherwise, <c>false</c>.</returns>
    public static bool IsDefaultValue(object value)
    {
        return IsDefaultValue(value, value?.GetType());
    }

    /// <summary>
    /// Determines whether [is default value] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="valueType">Type of the value.</param>
    /// <returns><c>true</c> if [is default value] [the specified value]; otherwise, <c>false</c>.</returns>
    public static bool IsDefaultValue(object value, Type valueType)
    {
        return value == null
               || valueType.IsValueType && value.Equals(valueType.GetDefaultValue());
    }

    /// <summary>
    /// The assignment definition cache
    /// </summary>
    private readonly static ConcurrentDictionary<string, AssignmentDefinition> AssignmentDefinitionCache
        = new();

    /// <summary>
    /// Gets the assignment definition.
    /// </summary>
    /// <param name="toType">To type.</param>
    /// <param name="fromType">From type.</param>
    /// <returns>AssignmentDefinition.</returns>
    static internal AssignmentDefinition GetAssignmentDefinition(Type toType, Type fromType)
    {
        var cacheKey = CreateCacheKey(fromType, toType);

        return AssignmentDefinitionCache.GetOrAdd(cacheKey, delegate
            {
                var definition = new AssignmentDefinition
                                     {
                                         ToType = toType,
                                         FromType = fromType
                                     };

                var readMap = GetMembers(fromType, true);
                var writeMap = GetMembers(toType, false);

                foreach (var assignmentMember in readMap)
                {
                    if (writeMap.TryGetValue(assignmentMember.Key, out var writeMember))
                    {
                        definition.AddMatch(assignmentMember.Key, assignmentMember.Value, writeMember);
                    }
                }

                return definition;
            });
    }

    /// <summary>
    /// Creates the cache key.
    /// </summary>
    /// <param name="fromType">From type.</param>
    /// <param name="toType">To type.</param>
    /// <returns>System.String.</returns>
    static internal string CreateCacheKey(Type fromType, Type toType)
    {
        var cacheKey = fromType.FullName + ">" + toType.FullName;
        return cacheKey;
    }

    /// <summary>
    /// Gets the members.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="isReadable">if set to <c>true</c> [is readable].</param>
    /// <returns>Dictionary&lt;System.String, AssignmentMember&gt;.</returns>
    private static Dictionary<string, AssignmentMember> GetMembers(Type type, bool isReadable)
    {
        var map = new Dictionary<string, AssignmentMember>();

        var members = type.GetAllPublicMembers();
        foreach (var info in members)
        {
            if (info.DeclaringType == typeof(object))
            {
                continue;
            }

            var propertyInfo = info as PropertyInfo;
            if (propertyInfo != null)
            {
                if (isReadable)
                {
                    if (propertyInfo.CanRead)
                    {
                        map[info.Name] = new AssignmentMember(propertyInfo.PropertyType, propertyInfo);
                        continue;
                    }
                }
                else
                {
                    if (propertyInfo.CanWrite && propertyInfo.GetSetMethod(true) != null)
                    {
                        map[info.Name] = new AssignmentMember(propertyInfo.PropertyType, propertyInfo);
                        continue;
                    }
                }
            }

            var fieldInfo = info as FieldInfo;
            if (fieldInfo != null)
            {
                map[info.Name] = new AssignmentMember(fieldInfo.FieldType, fieldInfo);
                continue;
            }
        }
        return map;
    }

    /// <summary>
    /// Populates the with.
    /// </summary>
    /// <typeparam name="To">The type of to.</typeparam>
    /// <typeparam name="From">The type of from.</typeparam>
    /// <param name="to">To.</param>
    /// <param name="from">From.</param>
    /// <returns>To.</returns>
    public static To PopulateWith<To, From>(this To to, From from)
    {
        if (Equals(to, default(To)) || Equals(from, default(From)))
        {
            return default;
        }

        var assignmentDefinition = GetAssignmentDefinition(to.GetType(), from.GetType());

        assignmentDefinition.Populate(to, from);

        return to;
    }

    /// <summary>
    /// Sets the property.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <param name="obj">The object.</param>
    /// <param name="value">The value.</param>
    public static void SetProperty(this PropertyInfo propertyInfo, object obj, object value)
    {
        if (!propertyInfo.CanWrite)
        {
            Tracer.Instance.WriteWarning("Attempted to set read only property '{0}'", propertyInfo.Name);
            return;
        }

        var propertySetMethodInfo = propertyInfo.GetSetMethod(true);

        propertySetMethodInfo?.Invoke(obj, [value]);
    }

    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <param name="obj">The object.</param>
    /// <returns>System.Object.</returns>
    public static object GetProperty(this PropertyInfo propertyInfo, object obj)
    {
        if (propertyInfo == null || !propertyInfo.CanRead)
        {
            return null;
        }

        var getMethod = propertyInfo.GetGetMethod(true);
        return getMethod?.Invoke(obj, TypeConstants.EmptyObjectArray);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="fieldInfo">The field information.</param>
    /// <param name="propertyInfo">The property information.</param>
    /// <param name="obj">The object.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(FieldInfo fieldInfo, PropertyInfo propertyInfo, object obj, object value)
    {
        try
        {
            if (IsUnsettableValue(propertyInfo))
            {
                return;
            }

            if (fieldInfo != null && !fieldInfo.IsLiteral)
            {
                fieldInfo.SetValue(obj, value);
            }
            else
            {
                SetProperty(propertyInfo, obj, value);
            }
        }
        catch (Exception ex)
        {
            var name = fieldInfo != null ? fieldInfo.Name : propertyInfo.Name;
            Tracer.Instance.WriteDebug("Could not set member: {0}. Error: {1}", name, ex.Message);
        }
    }

    /// <summary>
    /// Determines whether [is unsettable value] [the specified field information].
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns><c>true</c> if [is unsettable value] [the specified field information]; otherwise, <c>false</c>.</returns>
    public static bool IsUnsettableValue(PropertyInfo propertyInfo)
    {
        // Properties on non-user defined classes should not be set
        // Currently we define those properties as properties declared on
        // types defined in mscorlib
        if (propertyInfo != null && propertyInfo.ReflectedType != null)
        {
            return PclExport.Instance.InSameAssembly(propertyInfo.DeclaringType, typeof(object));
        }

        return false;
    }

    /// <summary>
    /// Creates the default values.
    /// </summary>
    /// <param name="types">The types.</param>
    /// <param name="recursionInfo">The recursion information.</param>
    /// <returns>System.Object[].</returns>
    public static object[] CreateDefaultValues(IEnumerable<Type> types, Dictionary<Type, int> recursionInfo)
    {
        return [.. types.Select(type => CreateDefaultValue(type, recursionInfo))];
    }

    /// <summary>
    /// The maximum recursion level for default values
    /// </summary>
    private const int MaxRecursionLevelForDefaultValues = 2; // do not nest a single type more than this deep.

    /// <summary>
    /// Creates the default value.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="recursionInfo">The recursion information.</param>
    /// <returns>System.Object.</returns>
    public static object CreateDefaultValue(Type type, Dictionary<Type, int> recursionInfo)
    {
        if (type == typeof(string))
        {
            return type.Name;
        }

        if (type.IsEnum)
        {
            return Enum.GetValues(type).GetValue(0);
        }

        if (type.IsAbstract)
        {
            return null;
        }

        // If we have hit our recursion limit for this type, then return null
        recursionInfo.TryGetValue(type, out var recurseLevel);
        if (recurseLevel > MaxRecursionLevelForDefaultValues)
        {
            return null;
        }

        recursionInfo[type] = recurseLevel + 1; // increase recursion level for this type
        try // use a try/finally block to make sure we decrease the recursion level for this type no matter which code path we take,
        {

            //when using KeyValuePair<TKey, TValue>, TKey must be non-default to stuff in a Dictionary
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var genericTypes = type.GetGenericArguments();
                var valueType = Activator.CreateInstance(type, CreateDefaultValue(genericTypes[0], recursionInfo), CreateDefaultValue(genericTypes[1], recursionInfo));
                return PopulateObjectInternal(valueType, recursionInfo);
            }

            if (type.IsValueType)
            {
                return type.CreateInstance();
            }

            if (type.IsArray)
            {
                return PopulateArray(type, recursionInfo);
            }

            var constructorInfo = type.GetConstructor(Type.EmptyTypes);
            var hasEmptyConstructor = constructorInfo != null;

            if (hasEmptyConstructor)
            {
                var value = constructorInfo.Invoke(TypeConstants.EmptyObjectArray);

                var genericCollectionType = PclExport.Instance.GetGenericCollectionType(type);
                if (genericCollectionType != null)
                {
                    SetGenericCollection(genericCollectionType, value, recursionInfo);
                }

                //when the object might have nested properties such as enums with non-0 values, etc
                return PopulateObjectInternal(value, recursionInfo);
            }
            return null;
        }
        finally
        {
            recursionInfo[type] = recurseLevel;
        }
    }

    /// <summary>
    /// Sets the generic collection.
    /// </summary>
    /// <param name="realizedListType">Type of the realized list.</param>
    /// <param name="genericObj">The generic object.</param>
    /// <param name="recursionInfo">The recursion information.</param>
    public static void SetGenericCollection(Type realizedListType, object genericObj, Dictionary<Type, int> recursionInfo)
    {
        var args = realizedListType.GetGenericArguments();
        if (args.Length != 1)
        {
            Tracer.Instance.WriteError("Found a generic list that does not take one generic argument: {0}", realizedListType);

            return;
        }

        var methodInfo = realizedListType.GetMethodInfo("Add");
        if (methodInfo != null)
        {
            var argValues = CreateDefaultValues(args, recursionInfo);

            methodInfo.Invoke(genericObj, argValues);
        }
    }

    /// <summary>
    /// Populates the array.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="recursionInfo">The recursion information.</param>
    /// <returns>Array.</returns>
    public static Array PopulateArray(Type type, Dictionary<Type, int> recursionInfo)
    {
        var elementType = type.GetElementType();
        var objArray = Array.CreateInstance(elementType, 1);
        var objElementType = CreateDefaultValue(elementType, recursionInfo);
        objArray.SetValue(objElementType, 0);

        return objArray;
    }

    /// <summary>
    /// Determines whether this instance can cast the specified to type.
    /// </summary>
    /// <param name="toType">To type.</param>
    /// <param name="fromType">From type.</param>
    /// <returns><c>true</c> if this instance can cast the specified to type; otherwise, <c>false</c>.</returns>
    public static bool CanCast(Type toType, Type fromType)
    {
        if (toType.IsInterface)
        {
            var interfaceList = fromType.GetInterfaces().ToList();
            if (interfaceList.Contains(toType))
            {
                return true;
            }
        }
        else
        {
            var baseType = fromType;
            bool areSameTypes;
            do
            {
                areSameTypes = baseType == toType;
            }
            while (!areSameTypes && (baseType = fromType.BaseType) != null);

            if (areSameTypes)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Tries the convert collections.
    /// </summary>
    /// <param name="fromType">From type.</param>
    /// <param name="toType">To type.</param>
    /// <param name="fromValue">From value.</param>
    /// <returns>System.Object.</returns>
    public static object TryConvertCollections(Type fromType, Type toType, object fromValue)
    {
        if (fromValue is IEnumerable values)
        {
            var toEnumObjs = toType == typeof(IEnumerable<object>);
            if (typeof(IList).IsAssignableFrom(toType) || toEnumObjs)
            {
                var to = (IList)(toType.IsArray || toEnumObjs ? new List<object>() : toType.CreateInstance());
                var elType = toType.GetCollectionType();
                foreach (var item in values)
                {
                    to.Add(elType != null ? item.ConvertTo(elType) : item);
                }
                if (elType != null && toType.IsArray)
                {
                    var arr = Array.CreateInstance(elType, to.Count);
                    to.CopyTo(arr, 0);
                    return arr;
                }

                return to;
            }

            if (fromValue is IDictionary d)
            {
                var obj = toType.CreateInstance();
                switch (obj)
                {
                    case List<KeyValuePair<string, string>> toList:
                        {
                            foreach (var key in d.Keys)
                            {
                                toList.Add(new KeyValuePair<string, string>(key.ConvertTo<string>(), d[key].ConvertTo<string>()));
                            }
                            return toList;
                        }
                    case List<KeyValuePair<string, object>> toObjList:
                        {
                            foreach (var key in d.Keys)
                            {
                                toObjList.Add(new KeyValuePair<string, object>(key.ConvertTo<string>(), d[key]));
                            }
                            return toObjList;
                        }
                    case IDictionary toDict:
                        {
                            if (toType.GetKeyValuePairsTypes(out var toKeyType, out var toValueType))
                            {
                                foreach (var key in d.Keys)
                                {
                                    var toKey = toKeyType != null
                                                    ? key.ConvertTo(toKeyType)
                                                    : key;
                                    var toValue = d[key].ConvertTo(toValueType);
                                    toDict[toKey] = toValue;
                                }
                                return toDict;
                            }
                            else
                            {
                                var from = fromValue.ToObjectDictionary();
                                var to = from.FromObjectDictionary(toType);
                                return to;
                            }
                        }
                }
            }

            var genericDef = fromType.GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));
            if (genericDef != null)
            {
                var genericEnumType = genericDef.GetGenericArguments()[0];
                var genericKvps = genericEnumType.GetTypeWithGenericTypeDefinitionOf(typeof(KeyValuePair<,>));
                if (genericKvps != null)
                {
                    // Improve perf with Specialized handling of common KVP combinations
                    var obj = toType.CreateInstance();
                    switch (fromValue)
                    {
                        case IEnumerable<KeyValuePair<string, string>> sKvps:
                            switch (obj)
                            {
                                case IDictionary toDict:
                                    {
                                        toType.GetKeyValuePairsTypes(out var toKeyType, out var toValueType);
                                        foreach (var entry in sKvps)
                                        {
                                            var toKey = toKeyType != null
                                                            ? entry.Key.ConvertTo(toKeyType)
                                                            : entry.Key;
                                            toDict[toKey] = toValueType != null
                                                                ? entry.Value.ConvertTo(toValueType)
                                                                : entry.Value;
                                        }
                                        return toDict;
                                    }
                                case List<KeyValuePair<string, string>> toList:
                                    {
                                        foreach (var entry in sKvps)
                                        {
                                            toList.Add(new KeyValuePair<string, string>(entry.Key, entry.Value));
                                        }
                                        return toList;
                                    }
                                case List<KeyValuePair<string, object>> toObjList:
                                    {
                                        foreach (var entry in sKvps)
                                        {
                                            toObjList.Add(new KeyValuePair<string, object>(entry.Key, entry.Value));
                                        }
                                        return toObjList;
                                    }
                            }

                            break;
                        case IEnumerable<KeyValuePair<string, object>> oKvps:
                            switch (obj)
                            {
                                case IDictionary toDict:
                                    {
                                        toType.GetKeyValuePairsTypes(out var toKeyType, out var toValueType);
                                        foreach (var entry in oKvps)
                                        {
                                            var toKey = entry.Key.ConvertTo<string>();
                                            toDict[toKey] = toValueType != null
                                                                ? entry.Value.ConvertTo(toValueType)
                                                                : entry.Value;
                                        }
                                        return toDict;
                                    }
                                case List<KeyValuePair<string, string>> toList:
                                    {
                                        foreach (var entry in oKvps)
                                        {
                                            toList.Add(new KeyValuePair<string, string>(entry.Key, entry.Value.ConvertTo<string>()));
                                        }
                                        return toList;
                                    }
                                case List<KeyValuePair<string, object>> toObjList:
                                    {
                                        foreach (var entry in oKvps)
                                        {
                                            toObjList.Add(new KeyValuePair<string, object>(entry.Key, entry.Value));
                                        }
                                        return toObjList;
                                    }
                            }

                            break;
                    }


                    // Fallback for handling any KVP combo
                    var toKvpDefType = toType.GetKeyValuePairsTypeDef();
                    switch (obj)
                    {
                        case IDictionary toDict:
                            {
                                var keyProp = TypeProperties.Get(toKvpDefType).GetPublicGetter("Key");
                                var valueProp = TypeProperties.Get(toKvpDefType).GetPublicGetter("Value");

                                foreach (var entry in values)
                                {
                                    var toKvp = entry.ConvertTo(toKvpDefType);
                                    var toKey = keyProp(toKvp);
                                    var toValue = valueProp(toKvp);
                                    toDict[toKey] = toValue;
                                }
                                return toDict;
                            }
                        case List<KeyValuePair<string, string>> toStringList:
                            {
                                foreach (var entry in values)
                                {
                                    var toEntry = entry.ConvertTo(toKvpDefType);
                                    toStringList.Add((KeyValuePair<string, string>)toEntry);
                                }
                                return toStringList;
                            }
                        case List<KeyValuePair<string, object>> toObjList:
                            {
                                foreach (var entry in values)
                                {
                                    var toEntry = entry.ConvertTo(toKvpDefType);
                                    toObjList.Add((KeyValuePair<string, object>)toEntry);
                                }
                                return toObjList;
                            }
                        case IEnumerable toList:
                            {
                                var addMethod = toType.GetMethod(nameof(IList.Add), [toKvpDefType]);
                                if (addMethod != null)
                                {
                                    foreach (var entry in values)
                                    {
                                        var toEntry = entry.ConvertTo(toKvpDefType);
                                        addMethod.Invoke(toList, [toEntry]);
                                    }
                                    return toList;
                                }
                                break;
                            }
                    }
                }
            }

            var fromElementType = fromType.GetCollectionType();
            var toElementType = toType.GetCollectionType();

            if (fromElementType != null && toElementType != null && fromElementType != toElementType &&
                !(typeof(IDictionary).IsAssignableFrom(fromElementType) || typeof(IDictionary).IsAssignableFrom(toElementType)))
            {
                var to = (from object item in values select item.ConvertTo(toElementType)).ToList();
                var ret = TranslateListWithElements.TryTranslateCollections(to.GetType(), toType, to);
                return ret ?? fromValue;
            }
        }
        else if (fromType.IsClass &&
                 (typeof(IDictionary).IsAssignableFrom(toType) ||
                  typeof(IEnumerable<KeyValuePair<string, object>>).IsAssignableFrom(toType) ||
                  typeof(IEnumerable<KeyValuePair<string, string>>).IsAssignableFrom(toType)))
        {
            var fromDict = fromValue.ToObjectDictionary();
            return TryConvertCollections(fromType.GetType(), toType, fromDict);
        }

        var listResult = TranslateListWithElements.TryTranslateCollections(fromType, toType, fromValue);
        return listResult ?? fromValue;
    }
}

/// <summary>
/// Class AssignmentEntry.
/// </summary>
public class AssignmentEntry
{
    /// <summary>
    /// The name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// From
    /// </summary>
    public AssignmentMember From { get; }

    /// <summary>
    /// To
    /// </summary>
    public AssignmentMember To { get; }

    /// <summary>
    /// The get value function
    /// </summary>
    public GetMemberDelegate GetValueFn { get; }

    /// <summary>
    /// The set value function
    /// </summary>
    public SetMemberDelegate SetValueFn { get; }

    /// <summary>
    /// The convert value function
    /// </summary>
    public GetMemberDelegate ConvertValueFn { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignmentEntry" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    public AssignmentEntry(string name, AssignmentMember from, AssignmentMember to)
    {
        this.Name = name;
        this.From = from;
        this.To = to;

        this.GetValueFn = this.From.CreateGetter();
        this.SetValueFn = this.To.CreateSetter();
        this.ConvertValueFn = TypeConverter.CreateTypeConverter(this.From.Type, this.To.Type);
    }
}

/// <summary>
/// Class AssignmentMember.
/// </summary>
public class AssignmentMember
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssignmentMember" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="propertyInfo">The property information.</param>
    public AssignmentMember(Type type, PropertyInfo propertyInfo)
    {
        this.Type = type;
        this.PropertyInfo = propertyInfo;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignmentMember" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="fieldInfo">The field information.</param>
    public AssignmentMember(Type type, FieldInfo fieldInfo)
    {
        this.Type = type;
        this.FieldInfo = fieldInfo;
    }

    /// <summary>
    /// The type
    /// </summary>
    public Type Type { get; }
    /// <summary>
    /// The property information
    /// </summary>
    public PropertyInfo PropertyInfo { get; }
    /// <summary>
    /// The field information
    /// </summary>
    public FieldInfo FieldInfo { get; }
    /// <summary>
    /// The method information
    /// </summary>
    public MethodInfo MethodInfo { get; }

    /// <summary>
    /// Creates the getter.
    /// </summary>
    /// <returns>GetMemberDelegate.</returns>
    public GetMemberDelegate CreateGetter()
    {
        if (this.PropertyInfo != null)
        {
            return this.PropertyInfo.CreateGetter();
        }

        if (this.FieldInfo != null)
        {
            return this.FieldInfo.CreateGetter();
        }

        return (GetMemberDelegate)this.MethodInfo?.CreateDelegate(typeof(GetMemberDelegate));
    }

    /// <summary>
    /// Creates the setter.
    /// </summary>
    /// <returns>SetMemberDelegate.</returns>
    public SetMemberDelegate CreateSetter()
    {
        if (this.PropertyInfo != null)
        {
            return this.PropertyInfo.CreateSetter();
        }

        if (this.FieldInfo != null)
        {
            return this.FieldInfo.CreateSetter();
        }

        return (SetMemberDelegate)this.MethodInfo?.MakeDelegate(typeof(SetMemberDelegate));
    }
}

/// <summary>
/// Class AssignmentDefinition.
/// </summary>
internal class AssignmentDefinition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssignmentDefinition" /> class.
    /// </summary>
    public AssignmentDefinition()
    {
        this.AssignmentMemberMap = [];
    }

    /// <summary>
    /// Gets or sets from type.
    /// </summary>
    /// <value>From type.</value>
    public Type FromType { get; set; }
    /// <summary>
    /// Converts to type.
    /// </summary>
    /// <value>To type.</value>
    public Type ToType { get; set; }

    /// <summary>
    /// Gets or sets the assignment member map.
    /// </summary>
    /// <value>The assignment member map.</value>
    public Dictionary<string, AssignmentEntry> AssignmentMemberMap { get; set; }

    /// <summary>
    /// Adds the match.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="readMember">The read member.</param>
    /// <param name="writeMember">The write member.</param>
    public void AddMatch(string name, AssignmentMember readMember, AssignmentMember writeMember)
    {
        if (AutoMappingUtils.ShouldIgnoreMapping(readMember.Type, writeMember.Type))
        {
            return;
        }

        // Ignore mapping collections if Element Types are ignored
        if (typeof(IEnumerable).IsAssignableFrom(readMember.Type) && typeof(IEnumerable).IsAssignableFrom(writeMember.Type))
        {
            var fromGenericDef = readMember.Type.GetTypeWithGenericTypeDefinitionOf(typeof(IDictionary<,>));
            var toGenericDef = writeMember.Type.GetTypeWithGenericTypeDefinitionOf(typeof(IDictionary<,>));
            if (fromGenericDef != null && toGenericDef != null)
            {
                // Check if to/from Key or Value Types are ignored
                var fromArgs = fromGenericDef.GetGenericArguments();
                var toArgs = toGenericDef.GetGenericArguments();
                if (AutoMappingUtils.ShouldIgnoreMapping(fromArgs[0], toArgs[0]))
                {
                    return;
                }

                if (AutoMappingUtils.ShouldIgnoreMapping(fromArgs[1], toArgs[1]))
                {
                    return;
                }
            }
            else if (readMember.Type != typeof(string) && writeMember.Type != typeof(string))
            {
                var elFromType = readMember.Type.GetCollectionType();
                var elToType = writeMember.Type.GetCollectionType();

                if (AutoMappingUtils.ShouldIgnoreMapping(elFromType, elToType))
                {
                    return;
                }
            }
        }

        this.AssignmentMemberMap[name] = new AssignmentEntry(name, readMember, writeMember);
    }

    /// <summary>
    /// Populates the specified to.
    /// </summary>
    /// <param name="to">To.</param>
    /// <param name="from">From.</param>
    public void Populate(object to, object from)
    {
        this.Populate(to, from, null, null);
    }

    /// <summary>
    /// Populates the specified to.
    /// </summary>
    /// <param name="to">To.</param>
    /// <param name="from">From.</param>
    /// <param name="propertyInfoPredicate">The property information predicate.</param>
    /// <param name="valuePredicate">The value predicate.</param>
    public void Populate(object to, object from,
                         Func<PropertyInfo, bool> propertyInfoPredicate,
                         Func<object, Type, bool> valuePredicate)
    {
        foreach (var assignmentEntry in this.AssignmentMemberMap.Select(assignmentEntryMap => assignmentEntryMap.Value))
        {
            var fromMember = assignmentEntry.From;
            var toMember = assignmentEntry.To;

            if (fromMember.PropertyInfo != null && propertyInfoPredicate != null && !propertyInfoPredicate(fromMember.PropertyInfo))
            {
                continue;
            }

            var fromType = fromMember.Type;
            var toType = toMember.Type;
            try
            {
                var fromValue = assignmentEntry.GetValueFn(from);

                if (valuePredicate != null
                    && (fromType == toType
                        || Nullable.GetUnderlyingType(fromType) == toType) && !valuePredicate(fromValue, fromMember.PropertyInfo.PropertyType)) // don't short-circuit nullable <-> non-null values
                {
                    continue;
                }

                if (assignmentEntry.ConvertValueFn != null)
                {
                    fromValue = assignmentEntry.ConvertValueFn(fromValue);
                }

                var setterFn = assignmentEntry.SetValueFn;
                setterFn(to, fromValue);
            }
            catch (Exception ex)
            {
                Tracer.Instance.WriteWarning("Error trying to set properties {0}.{1} > {2}.{3}:\n{4}",
                    this.FromType.FullName, fromType.Name,
                    this.ToType.FullName, toType.Name, ex);
            }
        }

        var populator = AutoMappingUtils.GetPopulator(to.GetType(), from.GetType());
        populator?.Invoke(to, from);
    }
}

/// <summary>
/// Delegate GetMemberDelegate
/// </summary>
/// <param name="instance">The instance.</param>
/// <returns>System.Object.</returns>
public delegate object GetMemberDelegate(object instance);
/// <summary>
/// Delegate GetMemberDelegate
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="instance">The instance.</param>
/// <returns>System.Object.</returns>
public delegate object GetMemberDelegate<in T>(T instance);

/// <summary>
/// Delegate PopulateMemberDelegate
/// </summary>
/// <param name="target">The target.</param>
/// <param name="source">The source.</param>
public delegate void PopulateMemberDelegate(object target, object source);

/// <summary>
/// Delegate SetMemberDelegate
/// </summary>
/// <param name="instance">The instance.</param>
/// <param name="value">The value.</param>
public delegate void SetMemberDelegate(object instance, object value);

/// <summary>
/// Delegate SetMemberDelegate
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="instance">The instance.</param>
/// <param name="value">The value.</param>
public delegate void SetMemberDelegate<in T>(T instance, object value);

/// <summary>
/// Delegate SetMemberRefDelegate
/// </summary>
/// <param name="instance">The instance.</param>
/// <param name="propertyValue">The property value.</param>
public delegate void SetMemberRefDelegate(ref object instance, object propertyValue);

/// <summary>
/// Delegate SetMemberRefDelegate
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="instance">The instance.</param>
/// <param name="value">The value.</param>
public delegate void SetMemberRefDelegate<T>(ref T instance, object value);

/// <summary>
/// Class TypeConverter.
/// </summary>
static internal class TypeConverter
{
    /// <summary>
    /// Creates the type converter.
    /// </summary>
    /// <param name="fromType">From type.</param>
    /// <param name="toType">To type.</param>
    /// <returns>GetMemberDelegate.</returns>
    public static GetMemberDelegate CreateTypeConverter(Type fromType, Type toType)
    {
        if (fromType == toType)
        {
            return null;
        }

        var converter = AutoMappingUtils.GetConverter(fromType, toType);
        if (converter != null)
        {
            return converter;
        }

        if (fromType == typeof(string))
        {
            return fromValue => TypeSerializer.DeserializeFromString((string)fromValue, toType);
        }

        if (toType == typeof(string))
        {
            return o => TypeSerializer.SerializeToString(o).StripQuotes();
        }

        var underlyingToType = Nullable.GetUnderlyingType(toType) ?? toType;
        var underlyingFromType = Nullable.GetUnderlyingType(fromType) ?? fromType;

        if (underlyingToType.IsEnum)
        {
            if (underlyingFromType.IsEnum || fromType == typeof(string))
            {
                return fromValue => Enum.Parse(underlyingToType, fromValue.ToString(), true);
            }

            if (underlyingFromType.IsIntegerType())
            {
                return fromValue => Enum.ToObject(underlyingToType, fromValue);
            }
        }
        else if (underlyingFromType.IsEnum)
        {
            if (underlyingToType.IsIntegerType())
            {
                return fromValue => Convert.ChangeType(fromValue, underlyingToType, null);
            }
        }
        else if (typeof(IEnumerable).IsAssignableFrom(fromType) && underlyingToType != typeof(string))
        {
            return fromValue => AutoMappingUtils.TryConvertCollections(fromType, underlyingToType, fromValue);
        }
        else if (underlyingToType.IsValueType)
        {
            return fromValue =>
                {
                    if (fromValue == null && toType.IsNullableType())
                    {
                        return null;
                    }

                    return AutoMappingUtils.ChangeValueType(fromValue, underlyingToType);
                };
        }
        else
        {
            return fromValue =>
                {
                    if (fromValue == null)
                    {
                        return null;
                    }

                    if (toType == typeof(string))
                    {
                        return fromValue.ToJsv();
                    }

                    var toValue = toType.CreateInstance();
                    toValue.PopulateWith(fromValue);
                    return toValue;
                };
        }

        return null;
    }
}