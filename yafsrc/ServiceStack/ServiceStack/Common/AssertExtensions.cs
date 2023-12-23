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