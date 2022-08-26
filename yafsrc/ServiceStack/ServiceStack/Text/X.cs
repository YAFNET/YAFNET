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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static To? Map<From, To>(From from, Func<From, To> fn) => from == null ? default : fn(from);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Apply<T>(T obj, Action<T> fn)
    {
        fn(obj);
        return obj;
    }
}
