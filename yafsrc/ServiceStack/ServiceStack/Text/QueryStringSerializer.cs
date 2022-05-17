// ***********************************************************************
// <copyright file="QueryStringSerializer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using ServiceStack.Text.Common;
using ServiceStack.Text.Json;
using ServiceStack.Text.Jsv;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>
/// Class QueryStringSerializer.
/// </summary>
public static class QueryStringSerializer
{
    /// <summary>
    /// Initializes static members of the <see cref="QueryStringSerializer"/> class.
    /// </summary>
    static QueryStringSerializer()
    {
        JsConfig.InitStatics();
        Instance = new JsWriter<JsvTypeSerializer>();
    }

    /// <summary>
    /// The instance
    /// </summary>
    internal static readonly JsWriter<JsvTypeSerializer> Instance;

    /// <summary>
    /// The write function cache
    /// </summary>
    private static Dictionary<Type, WriteObjectDelegate> WriteFnCache = new();

    /// <summary>
    /// Gets or sets the complex type strategy.
    /// </summary>
    /// <value>The complex type strategy.</value>
    public static WriteComplexTypeDelegate ComplexTypeStrategy { get; set; }

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>WriteObjectDelegate.</returns>
    internal static WriteObjectDelegate GetWriteFn(Type type)
    {
        try
        {
            if (WriteFnCache.TryGetValue(type, out var writeFn)) return writeFn;

            var genericType = typeof(QueryStringWriter<>).MakeGenericType(type);
            var mi = genericType.GetStaticMethod("WriteFn");
            var writeFactoryFn = (Func<WriteObjectDelegate>)mi.MakeDelegate(
                typeof(Func<WriteObjectDelegate>));

            writeFn = writeFactoryFn();

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = WriteFnCache;
                newCache = new Dictionary<Type, WriteObjectDelegate>(WriteFnCache) {
                               [type] = writeFn
                           };
            } while (!ReferenceEquals(
                         Interlocked.CompareExchange(ref WriteFnCache, newCache, snapshot), snapshot));

            return writeFn;
        }
        catch (Exception ex)
        {
            Tracer.Instance.WriteError(ex);
            throw;
        }
    }

    /// <summary>
    /// Writes the late bound object.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public static void WriteLateBoundObject(TextWriter writer, object value)
    {
        if (value == null) return;
        var writeFn = GetWriteFn(value.GetType());
        writeFn(writer, value);
    }

    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public static string SerializeToString<T>(T value)
    {
        var writer = StringWriterThreadStatic.Allocate();
        GetWriteFn(value.GetType())(writer, value);
        return StringWriterThreadStatic.ReturnAndFree(writer);
    }

    /// <summary>
    /// Initializes the aot.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void InitAot<T>()
    {
        QueryStringWriter<T>.WriteFn();
    }
}

/// <summary>
/// Implement the serializer using a more static approach
/// </summary>
/// <typeparam name="T"></typeparam>
public static class QueryStringWriter<T>
{
    /// <summary>
    /// The cache function
    /// </summary>
    private static readonly WriteObjectDelegate CacheFn;

    /// <summary>
    /// Writes the function.
    /// </summary>
    /// <returns>WriteObjectDelegate.</returns>
    public static WriteObjectDelegate WriteFn()
    {
        return CacheFn;
    }

    /// <summary>
    /// Initializes static members of the <see cref="QueryStringWriter{T}"/> class.
    /// </summary>
    static QueryStringWriter()
    {
        if (typeof(T) == typeof(object))
        {
            CacheFn = QueryStringSerializer.WriteLateBoundObject;
        }
        else if (typeof(T).IsAssignableFrom(typeof(IDictionary))
                 || typeof(T).HasInterface(typeof(IDictionary)))
        {
            CacheFn = WriteIDictionary;
        }
        else
        {
            var isEnumerable = typeof(T).IsAssignableFrom(typeof(IEnumerable))
                               || typeof(T).HasInterface(typeof(IEnumerable));

            if ((typeof(T).IsClass || typeof(T).IsInterface)
                && !isEnumerable)
            {
                var canWriteType = WriteType<T, JsvTypeSerializer>.Write;
                if (canWriteType != null)
                {
                    CacheFn = WriteType<T, JsvTypeSerializer>.WriteQueryString;
                    return;
                }
            }

            CacheFn = QueryStringSerializer.Instance.GetWriteFn<T>();
        }
    }

    /// <summary>
    /// Writes the object.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public static void WriteObject(TextWriter writer, object value)
    {
        if (writer == null) return;
        CacheFn(writer, value);
    }

    /// <summary>
    /// The serializer
    /// </summary>
    private static readonly ITypeSerializer Serializer = JsvTypeSerializer.Instance;
    /// <summary>
    /// Writes the i dictionary.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oMap">The o map.</param>
    public static void WriteIDictionary(TextWriter writer, object oMap)
    {
        WriteObjectDelegate writeKeyFn = null;
        WriteObjectDelegate writeValueFn = null;

        try
        {
            JsState.QueryStringMode = true;

            var isObjectDictionary = typeof(T) == typeof(Dictionary<string, object>);
            var map = (IDictionary)oMap;
            var ranOnce = false;
            foreach (var key in map.Keys)
            {
                var dictionaryValue = map[key];
                if (dictionaryValue == null)
                    continue;

                if (writeKeyFn == null)
                {
                    var keyType = key.GetType();
                    writeKeyFn = Serializer.GetWriteFn(keyType);
                }

                if (writeValueFn == null || isObjectDictionary)
                {
                    writeValueFn = dictionaryValue is string
                                       ? (w, x) => w.Write(((string)x).UrlEncode())
                                       : Serializer.GetWriteFn(dictionaryValue.GetType());
                }

                if (ranOnce)
                    writer.Write("&");
                else
                    ranOnce = true;

                JsState.WritingKeyCount++;
                try
                {
                    JsState.IsWritingValue = false;

                    writeKeyFn(writer, key);
                }
                finally
                {
                    JsState.WritingKeyCount--;
                }

                writer.Write("=");

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
        finally
        {
            JsState.QueryStringMode = false;
        }
    }
}

/// <summary>
/// Delegate WriteComplexTypeDelegate
/// </summary>
/// <param name="writer">The writer.</param>
/// <param name="propertyName">Name of the property.</param>
/// <param name="obj">The object.</param>
/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
public delegate bool WriteComplexTypeDelegate(TextWriter writer, string propertyName, object obj);

/// <summary>
/// Class PropertyTypeConfig.
/// </summary>
internal class PropertyTypeConfig
{
    /// <summary>
    /// The type configuration
    /// </summary>
    public TypeConfig TypeConfig;
    /// <summary>
    /// The write function
    /// </summary>
    public Action<string, TextWriter, object> WriteFn;
}

/// <summary>
/// Class PropertyTypeConfig.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class PropertyTypeConfig<T>
{
    /// <summary>
    /// The configuration
    /// </summary>
    public static PropertyTypeConfig Config;

    /// <summary>
    /// Initializes static members of the <see cref="PropertyTypeConfig{T}"/> class.
    /// </summary>
    static PropertyTypeConfig()
    {
        Config = new PropertyTypeConfig
                     {
                         TypeConfig = TypeConfig<T>.GetState(),
                         WriteFn = WriteType<T, JsvTypeSerializer>.WriteComplexQueryStringProperties,
                     };
    }
}

/// <summary>
/// Class QueryStringStrategy.
/// </summary>
public static class QueryStringStrategy
{
    /// <summary>
    /// The type configuration cache
    /// </summary>
    static readonly ConcurrentDictionary<Type, PropertyTypeConfig> typeConfigCache =
        new();

    /// <summary>
    /// Forms the URL encoded.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="obj">The object.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool FormUrlEncoded(TextWriter writer, string propertyName, object obj)
    {
        if (obj is IDictionary map)
        {
            var i = 0;
            foreach (var key in map.Keys)
            {
                if (i++ > 0)
                    writer.Write('&');

                var value = map[key];
                writer.Write(propertyName);
                writer.Write('[');
                writer.Write(key.ToString());
                writer.Write("]=");

                switch (value)
                {
                    case null:
                        writer.Write(JsonUtils.Null);
                        break;
                    case string strValue when strValue == string.Empty: /*ignore*/
                        break;
                    default:
                        {
                            var writeFn = JsvWriter.GetWriteFn(value.GetType());
                            writeFn(writer, value);
                            break;
                        }
                }
            }

            return true;
        }

        var typeConfig = typeConfigCache.GetOrAdd(obj.GetType(), t =>
            {
                var genericType = typeof(PropertyTypeConfig<>).MakeGenericType(t);
                var fi = genericType.Fields().First(x => x.Name == "Config");

                var config = (PropertyTypeConfig)fi.GetValue(null);
                return config;
            });

        typeConfig.WriteFn(propertyName, writer, obj);

        return true;
    }
}