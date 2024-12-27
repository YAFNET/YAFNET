// ***********************************************************************
// <copyright file="DeserializeTypeRef.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

namespace ServiceStack.OrmLite.Base.Text.Common;

/// <summary>
/// Class DeserializeTypeRef.
/// </summary>
static internal class DeserializeTypeRef
{
    /// <summary>
    /// Creates the serialization error.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="strType">Type of the string.</param>
    /// <returns>System.Runtime.Serialization.SerializationException.</returns>
    static internal SerializationException CreateSerializationError(Type type, string strType)
    {
        return new SerializationException(
            $"Type definitions should start with a '{JsWriter.MapStartChar}', expecting serialized type '{type.Name}', got string starting with: {strType.Substring(0, strType.Length < 50 ? strType.Length : 50)}");
    }

    /// <summary>
    /// Gets the serialization exception.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="propertyValueString">The property value string.</param>
    /// <param name="propertyType">Type of the property.</param>
    /// <param name="e">The e.</param>
    /// <returns>System.Runtime.Serialization.SerializationException.</returns>
    static internal SerializationException GetSerializationException(string propertyName, string propertyValueString, Type propertyType, Exception e)
    {
        var serializationException = new SerializationException($"Failed to set property '{propertyName}' with '{propertyValueString}'", e);
        if (propertyName != null)
        {
            serializationException.Data.Add("propertyName", propertyName);
        }
        if (propertyValueString != null)
        {
            serializationException.Data.Add("propertyValueString", propertyValueString);
        }
        if (propertyType != null)
        {
            serializationException.Data.Add("propertyType", propertyType);
        }
        return serializationException;
    }

    /// <summary>
    /// The type accessors cache
    /// </summary>
    private static Dictionary<Type, KeyValuePair<string, TypeAccessor>[]> TypeAccessorsCache = [];

    /// <summary>
    /// Gets the cached type accessors.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="serializer">The serializer.</param>
    /// <returns>System.Collections.Generic.KeyValuePair&lt;string, ServiceStack.Text.Common.TypeAccessor&gt;[].</returns>
    static internal KeyValuePair<string, TypeAccessor>[] GetCachedTypeAccessors(Type type, ITypeSerializer serializer)
    {
        if (TypeAccessorsCache.TryGetValue(type, out var typeAccessors))
        {
            return typeAccessors;
        }

        var typeConfig = new TypeConfig(type);
        typeAccessors = GetTypeAccessors(typeConfig, serializer);

        Dictionary<Type, KeyValuePair<string, TypeAccessor>[]> snapshot, newCache;
        do
        {
            snapshot = TypeAccessorsCache;
            newCache = new Dictionary<Type, KeyValuePair<string, TypeAccessor>[]>(TypeAccessorsCache)
                           {
                               [type] = typeAccessors
                           };
        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref TypeAccessorsCache, newCache, snapshot), snapshot));

        return typeAccessors;
    }

    /// <summary>
    /// Gets the type accessors.
    /// </summary>
    /// <param name="typeConfig">The type configuration.</param>
    /// <param name="serializer">The serializer.</param>
    /// <returns>System.Collections.Generic.KeyValuePair&lt;string, ServiceStack.Text.Common.TypeAccessor&gt;[].</returns>
    static internal KeyValuePair<string, TypeAccessor>[] GetTypeAccessors(TypeConfig typeConfig, ITypeSerializer serializer)
    {
        var type = typeConfig.Type;

        var propertyInfos = type.GetSerializableProperties();
        var fieldInfos = type.GetSerializableFields();
        if (propertyInfos.Length == 0 && fieldInfos.Length == 0)
        {
            return null;
        }

        var accessors = new KeyValuePair<string, TypeAccessor>[propertyInfos.Length + fieldInfos.Length];
        var i = 0;

        if (propertyInfos.Length != 0)
        {
            for (; i < propertyInfos.Length; i++)
            {
                var propertyInfo = propertyInfos[i];
                var propertyName = propertyInfo.Name;
                var dcsDataMember = propertyInfo.GetDataMember();
                if (dcsDataMember?.Name != null)
                {
                    propertyName = dcsDataMember.Name;
                }

                accessors[i] = new KeyValuePair<string, TypeAccessor>(propertyName, TypeAccessor.Create(serializer, typeConfig, propertyInfo));
            }
        }

        if (fieldInfos.Length != 0)
        {
            for (var j = 0; j < fieldInfos.Length; j++)
            {
                var fieldInfo = fieldInfos[j];
                var fieldName = fieldInfo.Name;
                var dcsDataMember = fieldInfo.GetDataMember();
                if (dcsDataMember?.Name != null)
                {
                    fieldName = dcsDataMember.Name;
                }

                accessors[i + j] = new KeyValuePair<string, TypeAccessor>(fieldName, TypeAccessor.Create(serializer, typeConfig, fieldInfo));
            }
        }

        Array.Sort(accessors, (x, y) => string.Compare(x.Key, y.Key, StringComparison.OrdinalIgnoreCase));
        return accessors;
    }
}

//The same class above but JSON-specific to enable inlining in this hot class.