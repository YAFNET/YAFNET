// ***********************************************************************
// <copyright file="DeserializeKeyValuePair.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Threading;

namespace ServiceStack.OrmLite.Base.Text.Common;

/// <summary>
/// Class DeserializeKeyValuePair.
/// </summary>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
static internal class DeserializeKeyValuePair<TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private readonly static ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// The key index
    /// </summary>
    const int KeyIndex = 0;
    /// <summary>
    /// The value index
    /// </summary>
    const int ValueIndex = 1;

    /// <summary>
    /// Gets the parse method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringDelegate.</returns>
    public static ParseStringDelegate GetParseMethod(Type type)
    {
        return v => GetParseStringSpanMethod(type)(v.AsSpan());
    }

    /// <summary>
    /// Gets the parse string span method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    public static ParseStringSpanDelegate GetParseStringSpanMethod(Type type)
    {
        var mapInterface = type.GetTypeWithGenericInterfaceOf(typeof(KeyValuePair<,>));

        var keyValuePairArgs = mapInterface.GetGenericArguments();
        var keyTypeParseMethod = Serializer.GetParseStringSpanFn(keyValuePairArgs[KeyIndex]);
        if (keyTypeParseMethod == null)
        {
            return null;
        }

        var valueTypeParseMethod = Serializer.GetParseStringSpanFn(keyValuePairArgs[ValueIndex]);
        if (valueTypeParseMethod == null)
        {
            return null;
        }

        var createMapType = type.HasAnyTypeDefinitionsOf(typeof(KeyValuePair<,>))
                                ? null : type;

        return value => ParseKeyValuePairType(value, createMapType, keyValuePairArgs, keyTypeParseMethod, valueTypeParseMethod);
    }

    /// <summary>
    /// The parse delegate cache
    /// </summary>
    private static Dictionary<string, ParseKeyValuePairDelegate> ParseDelegateCache
        = [];

    /// <summary>
    /// Delegate ParseKeyValuePairDelegate
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="createMapType">Type of the create map.</param>
    /// <param name="keyParseFn">The key parse function.</param>
    /// <param name="valueParseFn">The value parse function.</param>
    /// <returns>System.Object.</returns>
    private delegate object ParseKeyValuePairDelegate(ReadOnlySpan<char> value, Type createMapType,
                                                      ParseStringSpanDelegate keyParseFn, ParseStringSpanDelegate valueParseFn);

    /// <summary>
    /// The signature
    /// </summary>
    readonly static Type[] signature = [typeof(ReadOnlySpan<char>), typeof(Type), typeof(ParseStringSpanDelegate), typeof(ParseStringSpanDelegate)
    ];

    /// <summary>
    /// Parses the type of the key value pair.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="createMapType">Type of the create map.</param>
    /// <param name="argTypes">The argument types.</param>
    /// <param name="keyParseFn">The key parse function.</param>
    /// <param name="valueParseFn">The value parse function.</param>
    /// <returns>System.Object.</returns>
    public static object ParseKeyValuePairType(ReadOnlySpan<char> value, Type createMapType, Type[] argTypes,
                                               ParseStringSpanDelegate keyParseFn, ParseStringSpanDelegate valueParseFn)
    {
        var key = GetTypesKey(argTypes);
        if (ParseDelegateCache.TryGetValue(key, out var parseDelegate))
        {
            return parseDelegate(value, createMapType, keyParseFn, valueParseFn);
        }

        var mi = typeof(DeserializeKeyValuePair<TSerializer>).GetStaticMethod("ParseKeyValuePair", signature);
        var genericMi = mi.MakeGenericMethod(argTypes);
        parseDelegate = (ParseKeyValuePairDelegate)genericMi.MakeDelegate(typeof(ParseKeyValuePairDelegate));

        Dictionary<string, ParseKeyValuePairDelegate> snapshot, newCache;
        do
        {
            snapshot = ParseDelegateCache;
            newCache = new Dictionary<string, ParseKeyValuePairDelegate>(ParseDelegateCache) {
                           [key] = parseDelegate
                       };
        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

        return parseDelegate(value, createMapType, keyParseFn, valueParseFn);
    }

    /// <summary>
    /// Gets the types key.
    /// </summary>
    /// <param name="types">The types.</param>
    /// <returns>System.String.</returns>
    private static string GetTypesKey(params Type[] types)
    {
        var sb = StringBuilderThreadStatic.Allocate();
        foreach (var type in types)
        {
            if (sb.Length > 0)
            {
                sb.Append('>');
            }

            sb.Append(type.FullName);
        }
        return StringBuilderThreadStatic.ReturnAndFree(sb);
    }
}