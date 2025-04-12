// ***********************************************************************
// <copyright file="EnumerableExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite.Base.Common;

/// <summary>
/// Class EnumerableExtensions.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Determines whether the specified collection is empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">The collection.</param>
    /// <returns><c>true</c> if the specified collection is empty; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(this ICollection<T> collection)
    {
        return collection == null || collection.Count == 0;
    }

    /// <summary>
    /// Determines whether the specified collection is empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">The collection.</param>
    /// <returns><c>true</c> if the specified collection is empty; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(this T[] collection)
    {
        return collection == null || collection.Length == 0;
    }

    /// <summary>
    /// Converts to set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items">The items.</param>
    /// <returns>HashSet&lt;T&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashSet<T> ToSet<T>(this IEnumerable<T> items)
    {
        return [..items];
    }

    /// <summary>
    /// Eaches the specified action.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values.</param>
    /// <param name="action">The action.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Each<T>(this IEnumerable<T> values, Action<T> action)
    {
        if (values == null)
        {
            return;
        }

        foreach (var value in values)
        {
            action(value);
        }
    }

    /// <summary>
    /// Eaches the specified action.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <param name="map">The map.</param>
    /// <param name="action">The action.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Each<TKey, TValue>(this IDictionary<TKey, TValue> map, Action<TKey, TValue> action)
    {
        if (map == null)
        {
            return;
        }

        var keys = map.Keys.ToList();
        foreach (var key in keys)
        {
            action(key, map[key]);
        }
    }

    /// <summary>
    /// Maps the specified converter.
    /// </summary>
    /// <typeparam name="To">The type of to.</typeparam>
    /// <typeparam name="From">The type of from.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>List&lt;To&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<To> Map<To, From>(this IEnumerable<From> items, Func<From, To> converter)
    {
        return items == null ? [] : [.. items.Select(converter)];
    }

    /// <summary>
    /// Maps the specified converter.
    /// </summary>
    /// <typeparam name="To">The type of to.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>List&lt;To&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<To> Map<To>(this IEnumerable items, Func<object, To> converter)
    {
        return items == null ? [] : [.. (from object item in items select converter(item))];
    }

    /// <summary>
    /// Batcheses the of.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence">The sequence.</param>
    /// <param name="batchSize">Size of the batch.</param>
    /// <returns>IEnumerable&lt;T[]&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T[]> BatchesOf<T>(this IEnumerable<T> sequence, int batchSize)
    {
        var batch = new List<T>(batchSize);
        foreach (var item in sequence)
        {
            batch.Add(item);
            if (batch.Count >= batchSize)
            {
                yield return batch.ToArray();
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            yield return batch.ToArray();
            batch.Clear();
        }
    }

    /// <summary>
    /// Converts to safedictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="expr">The expr.</param>
    /// <returns>Dictionary&lt;TKey, T&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, T> ToSafeDictionary<T, TKey>(this IEnumerable<T> list, Func<T, TKey> expr) where TKey : notnull
    {
        var map = new Dictionary<TKey, T>();

        if (list == null)
        {
            return map;
        }

        foreach (var item in list)
        {
            map[expr(item)] = item;
        }

        return map;
    }

    /// <summary>
    /// Safes the specified enumerable.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>IEnumerable.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable Safe(this IEnumerable enumerable)
    {
        return enumerable ?? TypeConstants.EmptyObjectArray;
    }

    /// <summary>
    /// Check if match Exists in the Array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">The array.</param>
    /// <param name="match">The match.</param>
    /// <returns>bool.</returns>
    public static bool Exists<T>(this T[] array, Predicate<T> match)
    {
        return Array.Exists(array, match);
    }

    /// <summary>
    /// Finds the specified match in the  array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">The array.</param>
    /// <param name="match">The match.</param>
    /// <returns>T?.</returns>
    public static T? Find<T>(T[] array, Predicate<T> match)
    {
        return Array.Find(array, match);
    }
}