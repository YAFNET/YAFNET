// ***********************************************************************
// <copyright file="AssertUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceStack;

/// <summary>
/// Class AssertUtils.
/// </summary>
public static class AssertUtils
{
    /// <summary>
    /// Ares the not null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fields">The fields.</param>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static void AreNotNull<T>(params T[] fields)
    {
        if (fields.Contains(default))
        {
            throw new ArgumentNullException(typeof(T).Name);
        }
    }

    /// <summary>
    /// Ares the not null.
    /// </summary>
    /// <param name="fieldMap">The field map.</param>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static void AreNotNull(IDictionary<string, object> fieldMap)
    {
        foreach (var pair in fieldMap)
        {
            if (pair.Value == null)
            {
                throw new ArgumentNullException(pair.Key);
            }
        }
    }
}