// ***********************************************************************
// <copyright file="StringWriterCache.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Globalization;
using System.IO;

namespace ServiceStack.Text;

/// <summary>
/// Reusable StringWriter ThreadStatic Cache
/// </summary>
public static class StringWriterCache
{
    /// <summary>
    /// The cache
    /// </summary>
    [ThreadStatic]
    static StringWriter cache;

    /// <summary>
    /// Allocates this instance.
    /// </summary>
    /// <returns>StringWriter.</returns>
    public static StringWriter Allocate()
    {
        var ret = cache;
        if (ret == null)
            return new StringWriter(CultureInfo.InvariantCulture);

        var sb = ret.GetStringBuilder();
        sb.Length = 0;
        cache = null;  //don't re-issue cached instance until it's freed
        return ret;
    }

    /// <summary>
    /// Frees the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public static void Free(StringWriter writer)
    {
        cache = writer;
    }

    /// <summary>
    /// Returns the and free.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <returns>System.String.</returns>
    public static string ReturnAndFree(StringWriter writer)
    {
        var ret = writer.ToString();
        cache = writer;
        return ret;
    }
}

/// <summary>
/// Alternative Reusable StringWriter ThreadStatic Cache
/// </summary>
public static class StringWriterCacheAlt
{
    /// <summary>
    /// The cache
    /// </summary>
    [ThreadStatic]
    static StringWriter cache;

    /// <summary>
    /// Allocates this instance.
    /// </summary>
    /// <returns>StringWriter.</returns>
    public static StringWriter Allocate()
    {
        var ret = cache;
        if (ret == null)
            return new StringWriter(CultureInfo.InvariantCulture);

        var sb = ret.GetStringBuilder();
        sb.Length = 0;
        cache = null;  //don't re-issue cached instance until it's freed
        return ret;
    }

    /// <summary>
    /// Frees the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public static void Free(StringWriter writer)
    {
        cache = writer;
    }

    /// <summary>
    /// Returns the and free.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <returns>System.String.</returns>
    public static string ReturnAndFree(StringWriter writer)
    {
        var ret = writer.ToString();
        cache = writer;
        return ret;
    }
}

//Use separate cache internally to avoid reallocations and cache misses
/// <summary>
/// Class StringWriterThreadStatic.
/// </summary>
internal static class StringWriterThreadStatic
{
    /// <summary>
    /// The cache
    /// </summary>
    [ThreadStatic]
    static StringWriter cache;

    /// <summary>
    /// Allocates this instance.
    /// </summary>
    /// <returns>StringWriter.</returns>
    public static StringWriter Allocate()
    {
        var ret = cache;
        if (ret == null)
            return new StringWriter(CultureInfo.InvariantCulture);

        var sb = ret.GetStringBuilder();
        sb.Length = 0;
        cache = null;  //don't re-issue cached instance until it's freed
        return ret;
    }

    /// <summary>
    /// Frees the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public static void Free(StringWriter writer)
    {
        cache = writer;
    }

    /// <summary>
    /// Returns the and free.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <returns>System.String.</returns>
    public static string ReturnAndFree(StringWriter writer)
    {
        var ret = writer.ToString();
        cache = writer;
        return ret;
    }
}