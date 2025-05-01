// ***********************************************************************
// <copyright file="DeserializeListWithElements.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace ServiceStack.OrmLite.Base.Text.Common;

/// <summary>
/// Class DeserializeListWithElements.
/// </summary>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
public static class DeserializeListWithElements<TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    readonly static internal ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// The parse delegate cache
    /// </summary>
    private static Dictionary<Type, ParseListDelegate> ParseDelegateCache
        = [];

    /// <summary>
    /// Delegate ParseListDelegate
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="createListType">Type of the create list.</param>
    /// <param name="parseFn">The parse function.</param>
    /// <returns>System.Object.</returns>
    public delegate object ParseListDelegate(ReadOnlySpan<char> value, Type createListType, ParseStringSpanDelegate parseFn);

    /// <summary>
    /// The signature
    /// </summary>
    private readonly static Type[] signature = [typeof(ReadOnlySpan<char>), typeof(Type), typeof(ParseStringSpanDelegate)
    ];

    /// <summary>
    /// Gets the list type parse string span function.
    /// </summary>
    /// <param name="createListType">Type of the create list.</param>
    /// <param name="elementType">Type of the element.</param>
    /// <param name="parseFn">The parse function.</param>
    /// <returns>ParseListDelegate.</returns>
    public static ParseListDelegate GetListTypeParseStringSpanFn(
        Type createListType, Type elementType, ParseStringSpanDelegate parseFn)
    {
        if (ParseDelegateCache.TryGetValue(elementType, out var parseDelegate))
        {
            return parseDelegate.Invoke;
        }

        var genericType = typeof(DeserializeListWithElements<,>).MakeGenericType(elementType, typeof(TSerializer));
        var mi = genericType.GetStaticMethod("ParseGenericList", signature);
        parseDelegate = (ParseListDelegate)mi.MakeDelegate(typeof(ParseListDelegate));

        Dictionary<Type, ParseListDelegate> snapshot, newCache;
        do
        {
            snapshot = ParseDelegateCache;
            newCache = new Dictionary<Type, ParseListDelegate>(ParseDelegateCache) { [elementType] = parseDelegate };

        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

        return parseDelegate.Invoke;
    }

    /// <summary>
    /// Strips the list.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
    public static ReadOnlySpan<char> StripList(ReadOnlySpan<char> value)
    {
        if (value.IsNullOrEmpty())
        {
            return default;
        }

        value = value.Trim();

        const int startQuotePos = 1;
        const int endQuotePos = 2;
        var val = value[0] == JsWriter.ListStartChar
                      ? value.Slice(startQuotePos, value.Length - endQuotePos)
                      : value;

        if (val.Length == 0 || val.AdvancePastWhitespace().Length == 0)
        {
            return TypeConstants.EmptyStringSpan;
        }

        return val;
    }

    /// <summary>
    /// Parses the string list.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public static List<string> ParseStringList(ReadOnlySpan<char> value)
    {
        if ((value = StripList(value)).IsNullOrEmpty())
        {
            return value.IsEmpty ? null : [];
        }

        var to = new List<string>();
        var valueLength = value.Length;

        var i = 0;
        while (i < valueLength)
        {
            var elementValue = Serializer.EatValue(value, ref i);
            var listValue = Serializer.UnescapeString(elementValue);
            to.Add(listValue.Value());
            if (Serializer.EatItemSeperatorOrMapEndChar(value, ref i) && i == valueLength)
            {
                // If we ate a separator and we are at the end of the value,
                // it means the last element is empty => add default
                to.Add(null);
            }
        }

        return to;
    }

    /// <summary>
    /// Parses the int list.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>List&lt;System.Int32&gt;.</returns>
    public static List<int> ParseIntList(ReadOnlySpan<char> value)
    {
        if ((value = StripList(value)).IsNullOrEmpty())
        {
            return value.IsEmpty ? null : [];
        }

        var to = new List<int>();
        var valueLength = value.Length;

        var i = 0;
        while (i < valueLength)
        {
            var elementValue = Serializer.EatValue(value, ref i);
            to.Add(MemoryProvider.Instance.ParseInt32(elementValue));
            Serializer.EatItemSeperatorOrMapEndChar(value, ref i);
        }

        return to;
    }

    /// <summary>
    /// Parses the byte list.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>List&lt;System.Byte&gt;.</returns>
    public static List<byte> ParseByteList(ReadOnlySpan<char> value)
    {
        if ((value = StripList(value)).IsNullOrEmpty())
        {
            return value.IsEmpty ? null : [];
        }

        var to = new List<byte>();
        var valueLength = value.Length;

        var i = 0;
        while (i < valueLength)
        {
            var elementValue = Serializer.EatValue(value, ref i);
            to.Add(MemoryProvider.Instance.ParseByte(elementValue));
            Serializer.EatItemSeperatorOrMapEndChar(value, ref i);
        }

        return to;
    }
}

/// <summary>
/// Class DeserializeListWithElements.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
public static class DeserializeListWithElements<T, TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private readonly static ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// Parses the generic list.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="createListType">Type of the create list.</param>
    /// <param name="parseFn">The parse function.</param>
    /// <returns>ICollection&lt;T&gt;.</returns>
    public static ICollection<T> ParseGenericList(ReadOnlySpan<char> value, Type createListType, ParseStringSpanDelegate parseFn)
    {
        var isReadOnly = createListType is { IsGenericType: true } && createListType.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>);
        var to = createListType == null || isReadOnly
                     ? []
                     : (ICollection<T>)createListType.CreateInstance();

        var objSerializer = Json.JsonTypeSerializer.Instance.ObjectDeserializer;
        if (to is List<object> && objSerializer != null && typeof(TSerializer) == typeof(Json.JsonTypeSerializer))
        {
            return (ICollection<T>)objSerializer(value);
        }

        if ((value = DeserializeListWithElements<TSerializer>.StripList(value)).IsEmpty)
        {
            return null;
        }

        if (value.IsNullOrEmpty())
        {
            return isReadOnly ? (ICollection<T>)Activator.CreateInstance(createListType, to) : to;
        }

        var tryToParseItemsAsPrimitiveTypes =
            typeof(T) == typeof(object) && JsConfig.TryToParsePrimitiveTypeValues;

        if (!value.IsNullOrEmpty())
        {
            var valueLength = value.Length;
            var i = 0;
            Serializer.EatWhitespace(value, ref i);
            if (i < valueLength && value[i] == JsWriter.MapStartChar)
            {
                do
                {
                    var itemValue = Serializer.EatTypeValue(value, ref i);
                    if (!itemValue.IsEmpty)
                    {
                        to.Add((T)parseFn(itemValue));
                    }
                    else
                    {
                        to.Add(default);
                    }
                    Serializer.EatWhitespace(value, ref i);
                } while (++i < value.Length);
            }
            else
            {

                while (i < valueLength)
                {
                    var startIndex = i;
                    var elementValue = Serializer.EatValue(value, ref i);
                    var listValue = elementValue;
                    var isEmpty = listValue.IsNullOrEmpty();
                    if (!isEmpty)
                    {
                        if (tryToParseItemsAsPrimitiveTypes)
                        {
                            Serializer.EatWhitespace(value, ref startIndex);
                            to.Add((T)DeserializeType<TSerializer>.ParsePrimitive(elementValue.Value(), value[startIndex]));
                        }
                        else
                        {
                            to.Add((T)parseFn(elementValue));
                        }
                    }

                    if (Serializer.EatItemSeperatorOrMapEndChar(value, ref i) && i == valueLength)
                    {
                        // If we ate a separator and we are at the end of the value,
                        // it means the last element is empty => add default
                        to.Add(default);
                        continue;
                    }

                    if (isEmpty)
                    {
                        to.Add(default);
                    }
                }

            }
        }

        //TODO: 8-10-2011 -- this CreateInstance call should probably be moved over to ReflectionExtensions,
        //but not sure how you'd like to go about caching constructors with parameters -- I would probably build a NewExpression, .Compile to a LambdaExpression and cache
        return isReadOnly ? (ICollection<T>)Activator.CreateInstance(createListType, to) : to;
    }
}

/// <summary>
/// Class DeserializeList.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
public static class DeserializeList<T, TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The cache function
    /// </summary>
    private readonly static ParseStringSpanDelegate CacheFn;

    /// <summary>
    /// Initializes static members of the <see cref="DeserializeList{T, TSerializer}" /> class.
    /// </summary>
    static DeserializeList()
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
    /// <exception cref="System.ArgumentException">Type {typeof(T).FullName} is not of type IList</exception>
    public static ParseStringSpanDelegate GetParseStringSpanFn()
    {
        var listInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IList<>));
        if (listInterface == null)
        {
            throw new ArgumentException($"Type {typeof(T).FullName} is not of type IList<>");
        }

        //optimized access for regularly used types
        if (typeof(T) == typeof(List<string>))
        {
            return DeserializeListWithElements<TSerializer>.ParseStringList;
        }

        if (typeof(T) == typeof(List<int>))
        {
            return DeserializeListWithElements<TSerializer>.ParseIntList;
        }

        var elementType = listInterface.GetGenericArguments()[0];

        var supportedTypeParseMethod = DeserializeListWithElements<TSerializer>.Serializer.GetParseStringSpanFn(elementType);
        if (supportedTypeParseMethod != null)
        {
            var createListType = typeof(T).HasAnyTypeDefinitionsOf(typeof(List<>), typeof(IList<>))
                                     ? null : typeof(T);

            var parseFn = DeserializeListWithElements<TSerializer>.GetListTypeParseStringSpanFn(createListType, elementType, supportedTypeParseMethod);
            return value => parseFn(value, createListType, supportedTypeParseMethod);
        }

        return null;
    }

}

/// <summary>
/// Class DeserializeEnumerable.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
static internal class DeserializeEnumerable<T, TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The cache function
    /// </summary>
    private readonly static ParseStringSpanDelegate CacheFn;

    /// <summary>
    /// Initializes static members of the <see cref="DeserializeEnumerable{T, TSerializer}" /> class.
    /// </summary>
    static DeserializeEnumerable()
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
    /// <exception cref="System.ArgumentException">Type {typeof(T).FullName} is not of type IEnumerable</exception>
    public static ParseStringSpanDelegate GetParseStringSpanFn()
    {
        var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));
        if (enumerableInterface == null)
        {
            throw new ArgumentException($"Type {typeof(T).FullName} is not of type IEnumerable<>");
        }

        //optimized access for regularly used types
        if (typeof(T) == typeof(IEnumerable<string>))
        {
            return DeserializeListWithElements<TSerializer>.ParseStringList;
        }

        if (typeof(T) == typeof(IEnumerable<int>))
        {
            return DeserializeListWithElements<TSerializer>.ParseIntList;
        }

        var elementType = enumerableInterface.GetGenericArguments()[0];

        var supportedTypeParseMethod = DeserializeListWithElements<TSerializer>.Serializer.GetParseStringSpanFn(elementType);
        if (supportedTypeParseMethod != null)
        {
            const Type createListTypeWithNull = null; //Use conversions outside this class. see: Queue

            var parseFn = DeserializeListWithElements<TSerializer>.GetListTypeParseStringSpanFn(
                createListTypeWithNull, elementType, supportedTypeParseMethod);

            return value => parseFn(value, createListTypeWithNull, supportedTypeParseMethod);
        }

        return null;
    }

}