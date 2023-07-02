// ***********************************************************************
// <copyright file="X.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

using System;
using System.Runtime.CompilerServices;

namespace ServiceStack;

/// <summary>
/// Avoid polluting extension methods on every type with a 'X.*' short-hand
/// </summary>
public static class X
{
    /// <summary>
    /// Maps the specified from.
    /// </summary>
    /// <typeparam name="From">The type of from.</typeparam>
    /// <typeparam name="To">The type of to.</typeparam>
    /// <param name="from">From.</param>
    /// <param name="fn">The function.</param>
    /// <returns>System.Nullable&lt;To&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static To? Map<From, To>(From? from, Func<From, To> fn) => from == null ? default : fn(from);

    /// <summary>
    /// Applies the specified object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="fn">The function.</param>
    /// <returns>T.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Apply<T>(T obj, Action<T>? fn = null)
    {
        fn?.Invoke(obj);

        return obj;
    }
}
