// ***********************************************************************
// <copyright file="CachedTypeInfo.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

namespace ServiceStack.Text;

/// <summary>
/// Class CachedTypeInfo.
/// </summary>
public class CachedTypeInfo
{
    /// <summary>
    /// The cache map
    /// </summary>
    static Dictionary<Type, CachedTypeInfo> CacheMap = new();

    /// <summary>
    /// Gets the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>CachedTypeInfo.</returns>
    public static CachedTypeInfo Get(Type type)
    {
        if (CacheMap.TryGetValue(type, out var value))
            return value;

        var instance = new CachedTypeInfo(type);

        Dictionary<Type, CachedTypeInfo> snapshot, newCache;
        do
        {
            snapshot = CacheMap;
            newCache = new Dictionary<Type, CachedTypeInfo>(CacheMap)
                           {
                               [type] = instance
                           };
        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref CacheMap, newCache, snapshot), snapshot));

        return instance;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedTypeInfo"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public CachedTypeInfo(Type type)
    {
        EnumInfo = EnumInfo.GetEnumInfo(type);
    }

    /// <summary>
    /// Gets the enum information.
    /// </summary>
    /// <value>The enum information.</value>
    public EnumInfo EnumInfo { get; }
}

/// <summary>
/// Class EnumInfo.
/// </summary>
public class EnumInfo
{
    /// <summary>
    /// Gets the enum information.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>EnumInfo.</returns>
    public static EnumInfo GetEnumInfo(Type type)
    {
        if (type.IsEnum)
        {
            return new EnumInfo(type);
        }

        var nullableType = Nullable.GetUnderlyingType(type);
        return nullableType?.IsEnum == true ? new EnumInfo(nullableType) : null;
    }

    /// <summary>
    /// The enum type
    /// </summary>
    private readonly Type enumType;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumInfo"/> class.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    private EnumInfo(Type enumType)
    {
        this.enumType = enumType;
        enumMemberReverseLookup = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        var enumMembers = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var fi in enumMembers)
        {
            var enumValue = fi.GetValue(null);
            var strEnum = fi.Name;
            var enumMemberAttr = fi.FirstAttribute<EnumMemberAttribute>();
            if (enumMemberAttr?.Value != null)
            {
                if (enumMemberValues == null)
                {
                    enumMemberValues = new Dictionary<object, string>();
                }
                enumMemberValues[enumValue] = enumMemberAttr.Value;
                enumMemberReverseLookup[enumMemberAttr.Value] = enumValue;
            }
            else
            {
                enumMemberReverseLookup[strEnum] = enumValue;
            }
        }
        isEnumFlag = enumType.IsEnumFlags();
    }

    /// <summary>
    /// The is enum flag
    /// </summary>
    private readonly bool isEnumFlag;
    /// <summary>
    /// The enum member values
    /// </summary>
    private readonly Dictionary<object, string> enumMemberValues;
    /// <summary>
    /// The enum member reverse lookup
    /// </summary>
    private readonly Dictionary<string, object> enumMemberReverseLookup;

    /// <summary>
    /// Gets the serialized value.
    /// </summary>
    /// <param name="enumValue">The enum value.</param>
    /// <returns>System.Object.</returns>
    public object GetSerializedValue(object enumValue)
    {
        if (enumMemberValues != null && enumMemberValues.TryGetValue(enumValue, out var memberValue))
            return memberValue;
        if (isEnumFlag || JsConfig.TreatEnumAsInteger)
            return enumValue;
        return enumValue.ToString();
    }

    /// <summary>
    /// Parses the specified serialized value.
    /// </summary>
    /// <param name="serializedValue">The serialized value.</param>
    /// <returns>System.Object.</returns>
    public object Parse(string serializedValue)
    {
        return enumMemberReverseLookup.TryGetValue(serializedValue, out var enumValue) ? enumValue : Enum.Parse(enumType, serializedValue, true);
    }
}