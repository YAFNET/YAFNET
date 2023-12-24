// ***********************************************************************
// <copyright file="ListExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System.Collections.Generic;

/// <summary>
/// Class ListExtensions.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Joins the specified seperator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values.</param>
    /// <param name="seperator">The seperator.</param>
    /// <returns>System.String.</returns>
    public static string Join<T>(this IEnumerable<T> values, string seperator)
    {
        var sb = StringBuilderThreadStatic.Allocate();
        foreach (var value in values)
        {
            if (sb.Length > 0)
                sb.Append(seperator);
            sb.Append(value);
        }
        return StringBuilderThreadStatic.ReturnAndFree(sb);
    }

    /// <summary>
    /// Adds if not exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list.</param>
    /// <param name="item">The item.</param>
    public static void AddIfNotExists<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
            list.Add(item);
    }
}