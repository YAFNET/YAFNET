// ***********************************************************************
// <copyright file="AssertExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Common;

using System;

/// <summary>
/// Class AssertExtensions.
/// </summary>
public static class AssertExtensions
{
    /// <summary>
    /// Throws if null.
    /// </summary>
    /// <param name="obj">The object.</param>
    public static void ThrowIfNull(this object obj)
    {
        ThrowIfNull(obj, null);
    }

    /// <summary>
    /// Throws if null.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="varName">Name of the variable.</param>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static void ThrowIfNull(this object obj, string varName)
    {
        if (obj == null)
            throw new ArgumentNullException(varName ?? "object");
    }

    /// <summary>
    /// Throws if null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="varName">Name of the variable.</param>
    /// <returns>T.</returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static T ThrowIfNull<T>(this T obj, string varName)
    {
        if (obj == null)
            throw new ArgumentNullException(varName ?? "object");

        return obj;
    }

    /// <summary>
    /// Throws if null or empty.
    /// </summary>
    /// <param name="strValue">The string value.</param>
    /// <param name="varName">Name of the variable.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static string ThrowIfNullOrEmpty(this string strValue, string varName)
    {
        if (string.IsNullOrEmpty(strValue))
            throw new ArgumentNullException(varName ?? "string");

        return strValue;
    }
}