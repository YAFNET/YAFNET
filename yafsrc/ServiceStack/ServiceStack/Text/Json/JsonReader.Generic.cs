// ***********************************************************************
// <copyright file="JsonReader.Generic.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using ServiceStack.Text.Common;

namespace ServiceStack.Text.Json;

/// <summary>
/// Class JsonReader.
/// </summary>
public static class JsonReader
{
    /// <summary>
    /// The instance
    /// </summary>
    public static readonly JsReader<JsonTypeSerializer> Instance = new();

    /// <summary>
    /// The parse function cache
    /// </summary>
    private static Dictionary<Type, ParseFactoryDelegate> ParseFnCache = new();

    /// <summary>
    /// Gets the parse function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringDelegate.</returns>
    internal static ParseStringDelegate GetParseFn(Type type) => v => GetParseStringSpanFn(type)(v.AsSpan());

    /// <summary>
    /// Gets the parse span function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    internal static ParseStringSpanDelegate GetParseSpanFn(Type type) => v => GetParseStringSpanFn(type)(v);

    /// <summary>
    /// Gets the parse string span function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    internal static ParseStringSpanDelegate GetParseStringSpanFn(Type type)
    {
        ParseFnCache.TryGetValue(type, out var parseFactoryFn);

        if (parseFactoryFn != null)
            return parseFactoryFn();

        var genericType = typeof(JsonReader<>).MakeGenericType(type);
        var mi = genericType.GetStaticMethod(nameof(GetParseStringSpanFn));
        parseFactoryFn = (ParseFactoryDelegate)mi.MakeDelegate(typeof(ParseFactoryDelegate));

        Dictionary<Type, ParseFactoryDelegate> snapshot, newCache;
        do
        {
            snapshot = ParseFnCache;
            newCache = new Dictionary<Type, ParseFactoryDelegate>(ParseFnCache)
                           {
                               [type] = parseFactoryFn
                           };

        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref ParseFnCache, newCache, snapshot), snapshot));

        return parseFactoryFn();
    }

    /// <summary>
    /// Initializes the aot.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void InitAot<T>()
    {
        Text.Json.JsonReader.Instance.GetParseFn<T>();
        Text.Json.JsonReader<T>.Parse(TypeConstants.NullStringSpan);
        Text.Json.JsonReader<T>.GetParseFn();
        Text.Json.JsonReader<T>.GetParseStringSpanFn();
    }
}

/// <summary>
/// Class JsonReader.
/// </summary>
/// <typeparam name="T"></typeparam>
internal static class JsonReader<T>
{
    /// <summary>
    /// The read function
    /// </summary>
    private static ParseStringSpanDelegate ReadFn;

    /// <summary>
    /// Initializes static members of the <see cref="JsonReader{T}" /> class.
    /// </summary>
    static JsonReader()
    {
        Refresh();
    }

    /// <summary>
    /// Refreshes this instance.
    /// </summary>
    public static void Refresh()
    {
        JsConfig.InitStatics();

        if (JsonReader.Instance == null)
            return;

        ReadFn = JsonReader.Instance.GetParseStringSpanFn<T>();
        JsConfig.AddUniqueType(typeof(T));
    }

    /// <summary>
    /// Gets the parse function.
    /// </summary>
    /// <returns>ParseStringDelegate.</returns>
    public static ParseStringDelegate GetParseFn() => ReadFn != null
                                                          ? (ParseStringDelegate)(v => ReadFn(v.AsSpan()))
                                                          : Parse;

    /// <summary>
    /// Gets the parse string span function.
    /// </summary>
    /// <returns>ParseStringSpanDelegate.</returns>
    public static ParseStringSpanDelegate GetParseStringSpanFn() => ReadFn ?? Parse;

    /// <summary>
    /// Parses the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object Parse(string value) => value != null
                                                    ? Parse(value.AsSpan())
                                                    : null;

    /// <summary>
    /// Parses the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Can not deserialize interface type: "
    ///                         + typeof(T).Name</exception>
    public static object Parse(ReadOnlySpan<char> value)
    {
        TypeConfig<T>.Init();

        value = value.WithoutBom();

        if (ReadFn == null)
        {
            if (typeof(T).IsAbstract || typeof(T).IsInterface)
            {
                if (value.IsNullOrEmpty()) return null;
                var concreteType = DeserializeType<JsonTypeSerializer>.ExtractType(value);
                if (concreteType != null)
                {
                    return JsonReader.GetParseStringSpanFn(concreteType)(value);
                }
                throw new NotSupportedException("Can not deserialize interface type: "
                                                + typeof(T).Name);
            }

            Refresh();
        }

        return !value.IsEmpty
                   ? ReadFn(value)
                   : null;
    }
}