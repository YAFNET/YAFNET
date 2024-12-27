// ***********************************************************************
// <copyright file="DictionaryExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

using System.Collections.Generic;

/// <summary>
/// Class DictionaryExtensions.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Tries the remove.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <param name="map">The map.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key, out TValue value)
    {
        lock (map)
        {
            return map.Remove(key, out value);
        }
    }
}