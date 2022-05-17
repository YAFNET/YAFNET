// ***********************************************************************
// <copyright file="WriteDictionary.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text.Common;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using ServiceStack.Text.Json;

/// <summary>
/// Delegate WriteMapDelegate
/// </summary>
/// <param name="writer">The writer.</param>
/// <param name="oMap">The o map.</param>
/// <param name="writeKeyFn">The write key function.</param>
/// <param name="writeValueFn">The write value function.</param>
internal delegate void WriteMapDelegate(
    TextWriter writer,
    object oMap,
    WriteObjectDelegate writeKeyFn,
    WriteObjectDelegate writeValueFn);

/// <summary>
/// Class WriteDictionary.
/// </summary>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
internal static class WriteDictionary<TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// Class MapKey.
    /// </summary>
    internal class MapKey
    {
        /// <summary>
        /// The key type
        /// </summary>
        internal Type KeyType;
        /// <summary>
        /// The value type
        /// </summary>
        internal Type ValueType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapKey" /> class.
        /// </summary>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="valueType">Type of the value.</param>
        public MapKey(Type keyType, Type valueType)
        {
            KeyType = keyType;
            ValueType = valueType;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Equals(MapKey other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.KeyType == KeyType && other.ValueType == ValueType;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == typeof(MapKey) && Equals((MapKey)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((KeyType != null ? KeyType.GetHashCode() : 0) * 397) ^ (ValueType != null ? ValueType.GetHashCode() : 0);
            }
        }
    }

    /// <summary>
    /// The cache FNS
    /// </summary>
    static Dictionary<MapKey, WriteMapDelegate> CacheFns = new();

    /// <summary>
    /// Gets the write generic dictionary.
    /// </summary>
    /// <param name="keyType">Type of the key.</param>
    /// <param name="valueType">Type of the value.</param>
    /// <returns>Action&lt;TextWriter, System.Object, WriteObjectDelegate, WriteObjectDelegate&gt;.</returns>
    public static Action<TextWriter, object, WriteObjectDelegate, WriteObjectDelegate>
        GetWriteGenericDictionary(Type keyType, Type valueType)
    {
        var mapKey = new MapKey(keyType, valueType);
        if (CacheFns.TryGetValue(mapKey, out var writeFn)) return writeFn.Invoke;

        var genericType = typeof(ToStringDictionaryMethods<,,>).MakeGenericType(keyType, valueType, typeof(TSerializer));
        var mi = genericType.GetStaticMethod("WriteIDictionary");
        writeFn = (WriteMapDelegate)mi.MakeDelegate(typeof(WriteMapDelegate));

        Dictionary<MapKey, WriteMapDelegate> snapshot, newCache;
        do
        {
            snapshot = CacheFns;
            newCache = new Dictionary<MapKey, WriteMapDelegate>(CacheFns) {
                                                                              [mapKey] = writeFn
                                                                          };
        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref CacheFns, newCache, snapshot), snapshot));

        return writeFn.Invoke;
    }

    /// <summary>
    /// Writes the i dictionary.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oMap">The o map.</param>
    public static void WriteIDictionary(TextWriter writer, object oMap)
    {
        WriteObjectDelegate writeKeyFn = null;
        WriteObjectDelegate writeValueFn = null;

        writer.Write(JsWriter.MapStartChar);
        var encodeMapKey = false;
        Type lastKeyType = null;
        Type lastValueType = null;

        var map = (IDictionary)oMap;
        var ranOnce = false;
        foreach (var key in map.Keys)
        {
            var dictionaryValue = map[key];

            var isNull = dictionaryValue == null;
            if (isNull && !Serializer.IncludeNullValuesInDictionaries) continue;

            var keyType = key.GetType();
            if (writeKeyFn == null || lastKeyType != keyType)
            {
                lastKeyType = keyType;
                writeKeyFn = Serializer.GetWriteFn(keyType);
                encodeMapKey = Serializer.GetTypeInfo(keyType).EncodeMapKey;
            }

            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

            JsState.WritingKeyCount++;
            try
            {
                if (encodeMapKey)
                {
                    JsState.IsWritingValue = true; //prevent ""null""
                    try
                    {
                        writer.Write(JsWriter.QuoteChar);
                        writeKeyFn(writer, key);
                        writer.Write(JsWriter.QuoteChar);
                    }
                    finally
                    {
                        JsState.IsWritingValue = false;
                    }
                }
                else
                {
                    writeKeyFn(writer, key);
                }
            }
            finally
            {
                JsState.WritingKeyCount--;
            }

            writer.Write(JsWriter.MapKeySeperator);

            if (isNull)
            {
                writer.Write(JsonUtils.Null);
            }
            else
            {
                var valueType = dictionaryValue.GetType();
                if (writeValueFn == null || lastValueType != valueType)
                {
                    lastValueType = valueType;
                    writeValueFn = Serializer.GetWriteFn(valueType);
                }

                JsState.IsWritingValue = true;
                try
                {
                    writeValueFn(writer, dictionaryValue);
                }
                finally
                {
                    JsState.IsWritingValue = false;
                }
            }
        }

        writer.Write(JsWriter.MapEndChar);
    }
}

/// <summary>
/// Class ToStringDictionaryMethods.
/// </summary>
/// <typeparam name="TKey">The type of the t key.</typeparam>
/// <typeparam name="TValue">The type of the t value.</typeparam>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
public static class ToStringDictionaryMethods<TKey, TValue, TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// Writes the i dictionary.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oMap">The o map.</param>
    /// <param name="writeKeyFn">The write key function.</param>
    /// <param name="writeValueFn">The write value function.</param>
    public static void WriteIDictionary(
        TextWriter writer,
        object oMap,
        WriteObjectDelegate writeKeyFn,
        WriteObjectDelegate writeValueFn)
    {
        if (writer == null) return; //AOT
        WriteGenericIDictionary(writer, (IDictionary<TKey, TValue>)oMap, writeKeyFn, writeValueFn);
    }

    /// <summary>
    /// Writes the generic i dictionary.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="map">The map.</param>
    /// <param name="writeKeyFn">The write key function.</param>
    /// <param name="writeValueFn">The write value function.</param>
    public static void WriteGenericIDictionary(
        TextWriter writer,
        IDictionary<TKey, TValue> map,
        WriteObjectDelegate writeKeyFn,
        WriteObjectDelegate writeValueFn)
    {
        if (map == null)
        {
            writer.Write(JsonUtils.Null);
            return;
        }


        if (map is JsonObject jsonObject)
            map = (IDictionary<TKey, TValue>)jsonObject.ToUnescapedDictionary();

        writer.Write(JsWriter.MapStartChar);

        var encodeMapKey = Serializer.GetTypeInfo(typeof(TKey)).EncodeMapKey;

        var ranOnce = false;
        foreach (var kvp in map)
        {
            var isNull = kvp.Value == null;
            if (isNull && !Serializer.IncludeNullValuesInDictionaries) continue;

            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

            JsState.WritingKeyCount++;
            try
            {
                if (encodeMapKey)
                {
                    JsState.IsWritingValue = true; //prevent ""null""
                    try
                    {
                        writer.Write(JsWriter.QuoteChar);
                        writeKeyFn(writer, kvp.Key);
                        writer.Write(JsWriter.QuoteChar);
                    }
                    finally
                    {
                        JsState.IsWritingValue = false;
                    }
                }
                else
                {
                    writeKeyFn(writer, kvp.Key);
                }
            }
            finally
            {
                JsState.WritingKeyCount--;
            }

            writer.Write(JsWriter.MapKeySeperator);

            if (isNull)
            {
                writer.Write(JsonUtils.Null);
            }
            else
            {
                JsState.IsWritingValue = true;
                try
                {
                    writeValueFn(writer, kvp.Value);
                }
                finally
                {
                    JsState.IsWritingValue = false;
                }
            }
        }

        writer.Write(JsWriter.MapEndChar);
    }
}