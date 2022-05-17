// ***********************************************************************
// <copyright file="DeserializeArray.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Threading;

namespace ServiceStack.Text.Common;

/// <summary>
/// Class DeserializeArrayWithElements.
/// </summary>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
public static class DeserializeArrayWithElements<TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The parse delegate cache
    /// </summary>
    private static Dictionary<Type, ParseArrayOfElementsDelegate> ParseDelegateCache
        = new();

    /// <summary>
    /// Delegate ParseArrayOfElementsDelegate
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="parseFn">The parse function.</param>
    /// <returns>System.Object.</returns>
    public delegate object ParseArrayOfElementsDelegate(ReadOnlySpan<char> value, ParseStringSpanDelegate parseFn);

    /// <summary>
    /// Gets the parse function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Func&lt;System.String, ParseStringDelegate, System.Object&gt;.</returns>
    public static Func<string, ParseStringDelegate, object> GetParseFn(Type type)
    {
        var func = GetParseStringSpanFn(type);
        return (s, d) => func(s.AsSpan(), v => d(v.ToString()));
    }

    /// <summary>
    /// The signature
    /// </summary>
    private static readonly Type[] signature = { typeof(ReadOnlySpan<char>), typeof(ParseStringSpanDelegate) };

    /// <summary>
    /// Gets the parse string span function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseArrayOfElementsDelegate.</returns>
    public static ParseArrayOfElementsDelegate GetParseStringSpanFn(Type type)
    {
        if (ParseDelegateCache.TryGetValue(type, out var parseFn)) return parseFn.Invoke;

        var genericType = typeof(DeserializeArrayWithElements<,>).MakeGenericType(type, typeof(TSerializer));
        var mi = genericType.GetStaticMethod("ParseGenericArray", signature);
        parseFn = (ParseArrayOfElementsDelegate)mi.CreateDelegate(typeof(ParseArrayOfElementsDelegate));

        Dictionary<Type, ParseArrayOfElementsDelegate> snapshot, newCache;
        do
        {
            snapshot = ParseDelegateCache;
            newCache = new Dictionary<Type, ParseArrayOfElementsDelegate>(ParseDelegateCache) {
                           [type] = parseFn
                       };
        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

        return parseFn.Invoke;
    }
}

/// <summary>
/// Class DeserializeArrayWithElements.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
public static class DeserializeArrayWithElements<T, TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// Parses the generic array.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="elementParseFn">The element parse function.</param>
    /// <returns>T[].</returns>
    public static T[] ParseGenericArray(string value, ParseStringDelegate elementParseFn) =>
        ParseGenericArray(value.AsSpan(), v => elementParseFn(v.ToString()));

    /// <summary>
    /// Parses the generic array.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="elementParseFn">The element parse function.</param>
    /// <returns>T[].</returns>
    public static T[] ParseGenericArray(ReadOnlySpan<char> value, ParseStringSpanDelegate elementParseFn)
    {
        if ((value = DeserializeListWithElements<TSerializer>.StripList(value)).IsNullOrEmpty())
            return value.IsEmpty ? null : Array.Empty<T>();

        if (value[0] == JsWriter.MapStartChar)
        {
            var itemValues = new List<T>();
            var i = 0;
            do
            {
                var spanValue = Serializer.EatTypeValue(value, ref i);
                itemValues.Add((T)elementParseFn(spanValue));
                Serializer.EatItemSeperatorOrMapEndChar(value, ref i);
            } while (i < value.Length);

            return itemValues.ToArray();
        }
        else
        {
            var to = new List<T>();
            var valueLength = value.Length;

            var i = 0;
            while (i < valueLength)
            {
                var elementValue = Serializer.EatValue(value, ref i);
                var listValue = elementValue;
                to.Add((T)elementParseFn(listValue));
                if (Serializer.EatItemSeperatorOrMapEndChar(value, ref i)
                    && i == valueLength)
                {
                    // If we ate a separator and we are at the end of the value,
                    // it means the last element is empty => add default
                    to.Add(default);
                }
            }

            return to.ToArray();
        }
    }
}

/// <summary>
/// Class DeserializeArray.
/// </summary>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
internal static class DeserializeArray<TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The parse delegate cache
    /// </summary>
    private static Dictionary<Type, ParseStringSpanDelegate> ParseDelegateCache = new();

    /// <summary>
    /// Gets the parse function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringDelegate.</returns>
    public static ParseStringDelegate GetParseFn(Type type) => v => GetParseStringSpanFn(type)(v.AsSpan());

    /// <summary>
    /// Gets the parse string span function.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    public static ParseStringSpanDelegate GetParseStringSpanFn(Type type)
    {
        if (ParseDelegateCache.TryGetValue(type, out var parseFn)) return parseFn;

        var genericType = typeof(DeserializeArray<,>).MakeGenericType(type, typeof(TSerializer));

        var mi = genericType.GetStaticMethod("GetParseStringSpanFn");
        var parseFactoryFn = (Func<ParseStringSpanDelegate>)mi.MakeDelegate(
            typeof(Func<ParseStringSpanDelegate>));
        parseFn = parseFactoryFn();

        Dictionary<Type, ParseStringSpanDelegate> snapshot, newCache;
        do
        {
            snapshot = ParseDelegateCache;
            newCache = new Dictionary<Type, ParseStringSpanDelegate>(ParseDelegateCache) { [type] = parseFn };

        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

        return parseFn;
    }
}

/// <summary>
/// Class DeserializeArray.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
internal static class DeserializeArray<T, TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// The cache function
    /// </summary>
    private static readonly ParseStringSpanDelegate CacheFn;

    /// <summary>
    /// Initializes static members of the <see cref="DeserializeArray{T, TSerializer}" /> class.
    /// </summary>
    static DeserializeArray()
    {
        CacheFn = GetParseStringSpanFn();
    }

    /// <summary>
    /// Gets the parse.
    /// </summary>
    /// <value>The parse.</value>
    public static ParseStringDelegate Parse => v => CacheFn(v.AsSpan());

    /// <summary>
    /// Gets the parse string span.
    /// </summary>
    /// <value>The parse string span.</value>
    public static ParseStringSpanDelegate ParseStringSpan => CacheFn;

    /// <summary>
    /// Gets the parse string span function.
    /// </summary>
    /// <returns>ParseStringSpanDelegate.</returns>
    /// <exception cref="System.ArgumentException">Type {type.FullName} is not an Array type</exception>
    public static ParseStringSpanDelegate GetParseStringSpanFn()
    {
        var type = typeof(T);
        if (!type.IsArray)
            throw new ArgumentException($"Type {type.FullName} is not an Array type");

        if (type == typeof(string[]))
            return ParseStringArray;
        if (type == typeof(byte[]))
            return v => ParseByteArray(v.ToString());

        var elementType = type.GetElementType();
        var elementParseFn = Serializer.GetParseStringSpanFn(elementType);
        if (elementParseFn != null)
        {
            var parseFn = DeserializeArrayWithElements<TSerializer>.GetParseStringSpanFn(elementType);
            return value => parseFn(value, elementParseFn);
        }
        return null;
    }

    /// <summary>
    /// Parses the string array.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String[].</returns>
    public static string[] ParseStringArray(ReadOnlySpan<char> value)
    {
        if ((value = DeserializeListWithElements<TSerializer>.StripList(value)).IsNullOrEmpty())
            return value.IsEmpty ? null : TypeConstants.EmptyStringArray;
        return DeserializeListWithElements<TSerializer>.ParseStringList(value).ToArray();
    }

    /// <summary>
    /// Parses the string array.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String[].</returns>
    public static string[] ParseStringArray(string value) => ParseStringArray(value.AsSpan());

    /// <summary>
    /// Parses the byte array.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte[].</returns>
    public static byte[] ParseByteArray(string value) => ParseByteArray(value.AsSpan());

    /// <summary>
    /// Parses the byte array.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte[].</returns>
    public static byte[] ParseByteArray(ReadOnlySpan<char> value)
    {
        var isArray = value.Length > 1 && value[0] == '[';

        if ((value = DeserializeListWithElements<TSerializer>.StripList(value)).IsNullOrEmpty())
            return value.IsEmpty ? null : TypeConstants.EmptyByteArray;

        if ((value = Serializer.UnescapeString(value)).IsNullOrEmpty())
            return TypeConstants.EmptyByteArray;

        return !isArray
                   ? value.ParseBase64()
                   : DeserializeListWithElements<TSerializer>.ParseByteList(value).ToArray();
    }
}