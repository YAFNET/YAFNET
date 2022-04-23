// ***********************************************************************
// <copyright file="DeserializeCustomGenericType.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Linq;
using ServiceStack.Text.Json;

namespace ServiceStack.Text.Common;

/// <summary>
/// Class DeserializeCustomGenericType.
/// </summary>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
internal static class DeserializeCustomGenericType<TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// Gets the parse method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringDelegate.</returns>
    public static ParseStringDelegate GetParseMethod(Type type) => v => GetParseStringSpanMethod(type)(v.AsSpan());

    /// <summary>
    /// Gets the parse string span method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    public static ParseStringSpanDelegate GetParseStringSpanMethod(Type type)
    {
        if (type.Name.IndexOf("Tuple`", StringComparison.Ordinal) >= 0)
            return x => ParseTuple(type, x);

        return null;
    }

    /// <summary>
    /// Parses the tuple.
    /// </summary>
    /// <param name="tupleType">Type of the tuple.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ParseTuple(Type tupleType, string value) => ParseTuple(tupleType, value.AsSpan());

    /// <summary>
    /// Parses the tuple.
    /// </summary>
    /// <param name="tupleType">Type of the tuple.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public static object ParseTuple(Type tupleType, ReadOnlySpan<char> value)
    {
        var index = 0;
        Serializer.EatMapStartChar(value, ref index);
        if (JsonTypeSerializer.IsEmptyMap(value, index))
            return tupleType.CreateInstance();

        var genericArgs = tupleType.GetGenericArguments();
        var argValues = new object[genericArgs.Length];
        var valueLength = value.Length;
        while (index < valueLength)
        {
            var keyValue = Serializer.EatMapKey(value, ref index);
            Serializer.EatMapKeySeperator(value, ref index);
            var elementValue = Serializer.EatValue(value, ref index);
            if (keyValue.IsEmpty) continue;

            var keyIndex = keyValue.Slice("Item".Length).ParseInt32() - 1;
            var parseFn = Serializer.GetParseStringSpanFn(genericArgs[keyIndex]);
            argValues[keyIndex] = parseFn(elementValue);

            Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
        }

        var ctor = tupleType.GetConstructors()
            .First(x => x.GetParameters().Length == genericArgs.Length);
        return ctor.Invoke(argValues);
    }
}