﻿// ***********************************************************************
// <copyright file="StaticParseMethod.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

using ServiceStack.OrmLite.Base.Text.Jsv;

namespace ServiceStack.OrmLite.Base.Text.Common;

/// <summary>
/// Delegate ParseDelegate
/// </summary>
/// <param name="value">The value.</param>
/// <returns>System.Object.</returns>
internal delegate object ParseDelegate(string value);

/// <summary>
/// Class ParseMethodUtilities.
/// </summary>
static internal class ParseMethodUtilities
{
    /// <summary>
    /// Gets the parse function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parseMethod">The parse method.</param>
    /// <returns>ParseStringDelegate.</returns>
    public static ParseStringDelegate GetParseFn<T>(string parseMethod)
    {
        // Get the static Parse(string) method on the type supplied
        var parseMethodInfo = typeof(T).GetStaticMethod(parseMethod, [typeof(string)]);
        if (parseMethodInfo == null)
            return null;

        ParseDelegate parseDelegate = null;
        try
        {
            if (parseMethodInfo.ReturnType != typeof(T))
            {
                parseDelegate = (ParseDelegate)parseMethodInfo.MakeDelegate(typeof(ParseDelegate), false);
            }
            if (parseDelegate == null)
            {
                //Try wrapping strongly-typed return with wrapper fn.
                var typedParseDelegate = (Func<string, T>)parseMethodInfo.MakeDelegate(typeof(Func<string, T>));
                parseDelegate = x => typedParseDelegate(x);
            }
        }
        catch (ArgumentException)
        {
            Tracer.Instance.WriteDebug("Nonstandard Parse method on type {0}", typeof(T));
        }

        if (parseDelegate != null)
            return value => parseDelegate(value.FromCsvField());

        return null;
    }

    /// <summary>
    /// Delegate ParseStringSpanGenericDelegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>T.</returns>
    delegate T ParseStringSpanGenericDelegate<out T>(ReadOnlySpan<char> value);

    /// <summary>
    /// Gets the parse string span function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parseMethod">The parse method.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    public static ParseStringSpanDelegate GetParseStringSpanFn<T>(string parseMethod)
    {
        // Get the static Parse(string) method on the type supplied
        var parseMethodInfo = typeof(T).GetStaticMethod(parseMethod, [typeof(string)]);
        if (parseMethodInfo == null)
            return null;

        ParseStringSpanDelegate parseDelegate = null;
        try
        {
            if (parseMethodInfo.ReturnType != typeof(T))
            {
                parseDelegate = (ParseStringSpanDelegate)parseMethodInfo.MakeDelegate(typeof(ParseStringSpanDelegate), false);
            }
            if (parseDelegate == null)
            {
                //Try wrapping strongly-typed return with wrapper fn.
                var typedParseDelegate = (ParseStringSpanGenericDelegate<T>)parseMethodInfo.MakeDelegate(typeof(ParseStringSpanGenericDelegate<T>));
                parseDelegate = x => typedParseDelegate(x);
            }
        }
        catch (ArgumentException)
        {
            Tracer.Instance.WriteDebug("Nonstandard Parse method on type {0}", typeof(T));
        }

        if (parseDelegate != null)
            return value => parseDelegate(value.ToString().FromCsvField().AsSpan());

        return null;
    }
}

/// <summary>
/// Class StaticParseMethod.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class StaticParseMethod<T>
{
    /// <summary>
    /// The parse method
    /// </summary>
    const string ParseMethod = "Parse";

    /// <summary>
    /// Gets the parse.
    /// </summary>
    /// <value>The parse.</value>
    public static ParseStringDelegate Parse { get; }

    /// <summary>
    /// Gets the parse string span.
    /// </summary>
    /// <value>The parse string span.</value>
    public static ParseStringSpanDelegate ParseStringSpan { get; }

    /// <summary>
    /// Initializes static members of the <see cref="StaticParseMethod{T}" /> class.
    /// </summary>
    static StaticParseMethod()
    {
        Parse = ParseMethodUtilities.GetParseFn<T>(ParseMethod);
        ParseStringSpan = ParseMethodUtilities.GetParseStringSpanFn<T>(ParseMethod);
    }

}

/// <summary>
/// Class StaticParseRefTypeMethod.
/// </summary>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
/// <typeparam name="T"></typeparam>
static internal class StaticParseRefTypeMethod<TSerializer, T>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The parse method
    /// </summary>
    readonly static string ParseMethod = typeof(TSerializer) == typeof(JsvTypeSerializer)
                                             ? "ParseJsv"
                                             : "ParseJson";

    /// <summary>
    /// The parse string span method
    /// </summary>
    readonly static string ParseStringSpanMethod = typeof(TSerializer) == typeof(JsvTypeSerializer)
                                                       ? "ParseStringSpanJsv"
                                                       : "ParseStringSpanJson";

    /// <summary>
    /// Gets the parse.
    /// </summary>
    /// <value>The parse.</value>
    public static ParseStringDelegate Parse { get; }

    /// <summary>
    /// Gets the parse string span.
    /// </summary>
    /// <value>The parse string span.</value>
    public static ParseStringSpanDelegate ParseStringSpan { get; }

    /// <summary>
    /// Initializes static members of the <see cref="StaticParseRefTypeMethod{TSerializer, T}" /> class.
    /// </summary>
    static StaticParseRefTypeMethod()
    {
        Parse = ParseMethodUtilities.GetParseFn<T>(ParseMethod);
        ParseStringSpan = ParseMethodUtilities.GetParseStringSpanFn<T>(ParseStringSpanMethod);
    }
}