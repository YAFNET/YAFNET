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
    /// Maps the absolute path.
    /// </summary>
    /// <param name="relativePath">The relative path.</param>
    /// <param name="appendPartialPathModifier">The append partial path modifier.</param>
    /// <returns>System.String.</returns>
    public static string MapAbsolutePath(this string relativePath, string appendPartialPathModifier)
    {
        return PclExport.Instance.MapAbsolutePath(relativePath, appendPartialPathModifier);
    }

    /// <summary>
    /// Maps the path of a file in the bin\ folder of a self-hosted scenario
    /// </summary>
    /// <param name="relativePath">the relative path</param>
    /// <returns>the absolute path</returns>
    /// <remarks>Assumes static content is copied to /bin/ folder with the assemblies</remarks>
    public static string MapAbsolutePath(this string relativePath)
    {
        return PclExport.Instance.MapAbsolutePath(relativePath, null);
    }

    /// <summary>
    /// Combines the paths.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <param name="paths">The paths.</param>
    /// <returns>System.String.</returns>
    internal static string CombinePaths(StringBuilder sb, params string[] paths)
    {
        AppendPaths(sb, paths);
        return sb.ToString();
    }

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
                continue;

            if (sb.Length > 0 && sb[sb.Length - 1] != '/')
                sb.Append("/");

            sb.Append(path.Replace('\\', '/').TrimStart('/'));
        }
    }

    /// <summary>
    /// Combines the paths.
    /// </summary>
    /// <param name="paths">The paths.</param>
    /// <returns>System.String.</returns>
    public static string CombinePaths(params string[] paths)
    {
        var sb = StringBuilderThreadStatic.Allocate();
        AppendPaths(sb, paths);
        return StringBuilderThreadStatic.ReturnAndFree(sb);
    }

    /// <summary>
    /// Asserts the dir.
    /// </summary>
    /// <param name="dirPath">The dir path.</param>
    /// <returns>System.String.</returns>
    public static string AssertDir(this string dirPath)
    {
        if (!dirPath.DirectoryExists())
            dirPath.CreateDirectory();
        return dirPath;
    }

    /// <summary>
    /// The slashes
    /// </summary>
    private static readonly char[] Slashes = { '/', '\\' };

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
            return path;

        var lastChar = path[path.Length - 1];
        return chars.Any(c => c == lastChar) ? path.TrimEnd(chars) : path;
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
            return path;
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

        if (thesePaths.Length == 1 && thesePaths[0] == null) return path;
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
        if (thesePaths.Length == 1 && thesePaths[0] == null) return path;

        var sb = StringBuilderThreadStatic.Allocate();
        sb.Append(path.TrimEndIf(Slashes));
        AppendPaths(sb, ToStrings(thesePaths));
        return StringBuilderThreadStatic.ReturnAndFree(sb);
    }

    /// <summary>
    /// Resolves the paths.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    public static string ResolvePaths(this string path)
    {
        if (path == null || path.IndexOfAny("./", "/.") == -1)
            return path;

        var schemePos = path.IndexOf("://", StringComparison.Ordinal);
        var prefix = schemePos >= 0
                         ? path.Substring(0, schemePos + 3)
                         : "";

        var parts = path.Substring(prefix.Length).Split('/').ToList();
        var combinedPaths = new List<string>();
        foreach (var part in parts)
        {
            if (string.IsNullOrEmpty(part) || part == ".")
                continue;

            if (part == ".." && combinedPaths.Count > 0)
                combinedPaths.RemoveAt(combinedPaths.Count - 1);
            else
                combinedPaths.Add(part);
        }

        var resolvedPath = string.Join("/", combinedPaths);
        if (path[0] == '/' && prefix.Length == 0)
            resolvedPath = "/" + resolvedPath;

        return path[path.Length - 1] == '/' && resolvedPath.Length > 0
                   ? prefix + resolvedPath + "/"
                   : prefix + resolvedPath;
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
    internal static List<To> Map<To>(System.Collections.IEnumerable items, Func<object, To> converter)
    {
        return items == null ? new List<To>() : (from object item in items select converter(item)).ToList();
    }
}