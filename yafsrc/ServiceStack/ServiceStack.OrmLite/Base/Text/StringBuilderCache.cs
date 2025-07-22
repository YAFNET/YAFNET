// ***********************************************************************
// <copyright file="StringBuilderCache.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Text;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Reusable StringBuilder ThreadStatic Cache
/// </summary>
public static class StringBuilderCache
{
    /// <summary>
    /// The cache
    /// </summary>
    [ThreadStatic]
    static StringBuilder cache;

    /// <summary>
    /// Allocates this instance.
    /// </summary>
    /// <returns>StringBuilder.</returns>
    public static StringBuilder Allocate()
    {
        var ret = cache;
        if (ret == null)
        {
            return new StringBuilder();
        }

        ret.Length = 0;
        cache = null;  //don't re-issue cached instance until it's freed
        return ret;
    }

    /// <summary>
    /// Frees the specified sb.
    /// </summary>
    /// <param name="sb">The sb.</param>
    public static void Free(StringBuilder sb)
    {
        cache = sb;
    }

    /// <summary>
    /// Returns the and free.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <returns>System.String.</returns>
    public static string ReturnAndFree(StringBuilder sb)
    {
        var ret = sb.ToString();
        cache = sb;
        return ret;
    }
}

/// <summary>
/// Alternative Reusable StringBuilder ThreadStatic Cache
/// </summary>
public static class StringBuilderCacheAlt
{
    /// <summary>
    /// The cache
    /// </summary>
    [ThreadStatic]
    static StringBuilder cache;

    /// <summary>
    /// Allocates this instance.
    /// </summary>
    /// <returns>StringBuilder.</returns>
    public static StringBuilder Allocate()
    {
        return Allocate(16); // 16 is the default capacity for StringBuilder
    }

    /// <summary>
    /// Allocates the specified capacity.
    /// </summary>
    /// <param name="capacity">The capacity.</param>
    /// <returns>StringBuilder.</returns>
    public static StringBuilder Allocate(int capacity)
    {
        var ret = cache;
        if (ret == null)
        {
            return new StringBuilder(capacity);
        }

        ret.Length = 0;
        cache = null;  //don't re-issue cached instance until it's freed
        return ret;
    }

    /// <summary>
    /// Frees the specified sb.
    /// </summary>
    /// <param name="sb">The sb.</param>
    public static void Free(StringBuilder sb)
    {
        cache = sb;
    }

    /// <summary>
    /// Returns the and free.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <returns>System.String.</returns>
    public static string ReturnAndFree(StringBuilder sb)
    {
        var ret = sb.ToString();
        cache = sb;
        return ret;
    }
}

//Use separate cache internally to avoid re-allocations and cache misses
/// <summary>
/// Class StringBuilderThreadStatic.
/// </summary>
static internal class StringBuilderThreadStatic
{
    /// <summary>
    /// The cache
    /// </summary>
    [ThreadStatic]
    static StringBuilder cache;

    /// <summary>
    /// Allocates this instance.
    /// </summary>
    /// <returns>StringBuilder.</returns>
    public static StringBuilder Allocate()
    {
        var ret = cache;
        if (ret == null)
        {
            return new StringBuilder();
        }

        ret.Length = 0;
        cache = null;  //don't re-issue cached instance until it's freed
        return ret;
    }

    /// <summary>
    /// Frees the specified sb.
    /// </summary>
    /// <param name="sb">The sb.</param>
    public static void Free(StringBuilder sb)
    {
        cache = sb;
    }

    /// <summary>
    /// Returns the and free.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <returns>System.String.</returns>
    public static string ReturnAndFree(StringBuilder sb)
    {
        var ret = sb.ToString();
        cache = sb;
        return ret;
    }
}