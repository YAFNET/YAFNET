// ***********************************************************************
// <copyright file="PathUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Class PathUtils.
/// </summary>
public static class PathUtils
{
    /// <summary>
    /// Appends the paths.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <param name="paths">The paths.</param>
    public static void AppendPaths(StringBuilder sb, string[] paths)
    {
        foreach (var path in paths)
        {
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            if (sb.Length > 0 && sb[^1] != '/')
            {
                sb.Append('/');
            }

            sb.Append(path.Replace('\\', '/').TrimStart('/'));
        }
    }

    /// <summary>
    /// The slashes
    /// </summary>
    private readonly static char[] Slashes = ['/', '\\'];

    /// <summary>
    /// Trims the end if.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="chars">The chars.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //only trim/allocate if need to
    private static string TrimEndIf(this string path, char[] chars)
    {
        if (string.IsNullOrEmpty(path) || chars == null || chars.Length == 0)
        {
            return path;
        }

        var lastChar = path[^1];
        return chars.Exists(c => c == lastChar) ? path.TrimEnd(chars) : path;
    }

    /// <summary>
    /// Combines the with.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="withPath">The with path.</param>
    /// <returns>System.String.</returns>
    public static string CombineWith(this string path, string withPath)
    {
        path ??= "";
        if (string.IsNullOrEmpty(withPath))
        {
            return path;
        }

        var startPath = path.TrimEndIf(Slashes);
        return startPath + (withPath[0] == '/' ? withPath : "/" + withPath);
    }

    /// <summary>
    /// Combines the with.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="thesePaths">The these paths.</param>
    /// <returns>System.String.</returns>
    public static string CombineWith(this string path, params string[] thesePaths)
    {
        path ??= "";

        if (thesePaths.Length == 1 && thesePaths[0] == null)
        {
            return path;
        }

        var startPath = path.TrimEndIf(Slashes);

        var sb = StringBuilderThreadStatic.Allocate();
        sb.Append(startPath);
        AppendPaths(sb, thesePaths);
        return StringBuilderThreadStatic.ReturnAndFree(sb);
    }

    /// <summary>
    /// Combines the with.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="thesePaths">The these paths.</param>
    /// <returns>System.String.</returns>
    public static string CombineWith(this string path, params object[] thesePaths)
    {
        if (thesePaths.Length == 1 && thesePaths[0] == null)
        {
            return path;
        }

        var sb = StringBuilderThreadStatic.Allocate();
        sb.Append(path.TrimEndIf(Slashes));
        AppendPaths(sb, ToStrings(thesePaths));
        return StringBuilderThreadStatic.ReturnAndFree(sb);
    }

    /// <summary>
    /// Converts to strings.
    /// </summary>
    /// <param name="thesePaths">The these paths.</param>
    /// <returns>System.String[].</returns>
    public static string[] ToStrings(object[] thesePaths)
    {
        var to = new string[thesePaths.Length];
        for (var i = 0; i < thesePaths.Length; i++)
        {
            to[i] = thesePaths[i].ToString();
        }
        return to;
    }

    /// <summary>
    /// Maps the specified items.
    /// </summary>
    /// <typeparam name="To">The type of to.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>List&lt;To&gt;.</returns>
    static internal List<To> Map<To>(System.Collections.IEnumerable items, Func<object, To> converter)
    {
        return items == null ? [] : (from object item in items select converter(item)).ToList();
    }
}