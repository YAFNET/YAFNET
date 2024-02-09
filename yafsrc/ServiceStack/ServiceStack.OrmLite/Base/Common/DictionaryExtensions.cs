// ***********************************************************************
// <copyright file="DictionaryExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

/// <summary>
/// Class DictionaryExtensions.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>TValue.</returns>
    public static TValue GetValue<TValue, TKey>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValue)
    {
        return dictionary.ContainsKey(key) ? dictionary[key] : defaultValue();
    }

    /// <summary>
    /// Determines whether [is null or empty] [the specified dictionary].
    /// </summary>
    /// <param name="dictionary">The dictionary.</param>
    /// <returns><c>true</c> if [is null or empty] [the specified dictionary]; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty(this IDictionary dictionary)
    {
        return dictionary == null || dictionary.Count == 0;
    }

    /// <summary>
    /// Fors the each.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="onEachFn">The on each function.</param>
    public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Action<TKey, TValue> onEachFn)
    {
        foreach (var entry in dictionary)
        {
            onEachFn(entry.Key, entry.Value);
        }
    }

    /// <summary>
    /// Converts all.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="map">The map.</param>
    /// <param name="createFn">The create function.</param>
    /// <returns>List&lt;T&gt;.</returns>
    public static List<T> ConvertAll<T, K, V>(IDictionary<K, V> map, Func<K, V, T> createFn)
    {
        var list = new List<T>();
        map.Each((kvp) => list.Add(createFn(kvp.Key, kvp.Value)));
        return list;
    }

    /// <summary>
    /// Gets the or add.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="map">The map.</param>
    /// <param name="key">The key.</param>
    /// <param name="createFn">The create function.</param>
    /// <returns>V.</returns>
    public static V GetOrAdd<K, V>(this Dictionary<K, V> map, K key, Func<K, V> createFn)
    {
        //simulate ConcurrentDictionary.GetOrAdd
        lock (map)
        {
            if (!map.TryGetValue(key, out V val))
                map[key] = val = createFn(key);

            return val;
        }
    }

    /// <summary>
    /// Converts to dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <param name="map">The map.</param>
    /// <returns>Dictionary&lt;TKey, TValue&gt;.</returns>
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> map) => new(map);

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
            if (!map.TryGetValue(key, out value)) return false;
            map.Remove(key);
            return true;
        }
    }

    /// <summary>
    /// Merges the specified with sources.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <param name="initial">The initial.</param>
    /// <param name="withSources">The with sources.</param>
    /// <returns>Dictionary&lt;TKey, TValue&gt;.</returns>
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> initial,
                                                               params IEnumerable<KeyValuePair<TKey, TValue>>[] withSources)
    {
        var to = new Dictionary<TKey, TValue>(initial);
        foreach (var kvps in withSources)
        {
            foreach (var kvp in kvps)
            {
                to[kvp.Key] = kvp.Value;
            }
        }
        return to;
    }

}