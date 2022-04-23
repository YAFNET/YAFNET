// ***********************************************************************
// <copyright file="SharedPools.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Text.Pools;

// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

/// <summary>
/// Shared object pool for roslyn
/// Use this shared pool if only concern is reducing object allocations.
/// if perf of an object pool itself is also a concern, use ObjectPool directly.
/// For example, if you want to create a million of small objects within a second,
/// use the ObjectPool directly. it should have much less overhead than using this.
/// </summary>
public static class SharedPools
{
    /// <summary>
    /// pool that uses default constructor with 20 elements pooled
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>ServiceStack.Text.Pools.ObjectPool&lt;T&gt;.</returns>
    public static ObjectPool<T> Default<T>() where T : class, new()
    {
        return DefaultNormalPool<T>.Instance;
    }

    /// <summary>
    /// pool that uses string as key with StringComparer.OrdinalIgnoreCase as key comparer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>ServiceStack.Text.Pools.ObjectPool&lt;System.Collections.Generic.Dictionary&lt;string, T&gt;&gt;.</returns>
    public static ObjectPool<Dictionary<string, T>> StringIgnoreCaseDictionary<T>()
    {
        return StringIgnoreCaseDictionaryNormalPool<T>.Instance;
    }

    /// <summary>
    /// Used to reduce the # of temporary byte[]s created to satisfy serialization and
    /// other I/O requests
    /// </summary>
    public static readonly ObjectPool<byte[]> ByteArray = new(() => new byte[ByteBufferSize], ByteBufferCount);

    /// <summary>
    /// The byte buffer size
    /// </summary>
    /// pooled memory : 4K * 512 = 4MB
    public const int ByteBufferSize = 4 * 1024;
    /// <summary>
    /// The byte buffer count
    /// </summary>
    private const int ByteBufferCount = 512;

    /// <summary>
    /// Class DefaultBigPool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private static class DefaultBigPool<T> where T : class, new()
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly ObjectPool<T> Instance = new(() => new T(), 100);
    }

    /// <summary>
    /// Class DefaultNormalPool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private static class DefaultNormalPool<T> where T : class, new()
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly ObjectPool<T> Instance = new(() => new T(), 20);
    }

    /// <summary>
    /// Class StringIgnoreCaseDictionaryNormalPool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private static class StringIgnoreCaseDictionaryNormalPool<T>
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly ObjectPool<Dictionary<string, T>> Instance =
            new(() => new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase), 20);
    }
}