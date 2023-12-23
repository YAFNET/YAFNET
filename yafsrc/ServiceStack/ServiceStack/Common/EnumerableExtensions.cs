// ***********************************************************************
// <copyright file="EnumerableExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ServiceStack.Text;

namespace ServiceStack;

/// <summary>
/// Class EnumerableUtils.
/// </summary>
public static class EnumerableUtils
{
    /// <summary>
    /// Converts to list.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>List&lt;System.Object&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<object> ToList(IEnumerable items)
    {
        if (items == null)
            return TypeConstants.EmptyObjectList;
        var to = new List<object>();
        foreach (var item in items)
        {
            to.Add(item);
        }
        return to;
    }

    /// <summary>
    /// Nulls if empty.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>IEnumerable.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable NullIfEmpty(IEnumerable items)
    {
        if (items != null)
        {
            foreach (var item in items)
                return items;
        }
        return null;
    }

    /// <summary>
    /// Determines whether the specified items is empty.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns><c>true</c> if the specified items is empty; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(IEnumerable items) => EnumerableUtils.NullIfEmpty(items) == null;
}

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
    public static bool IsEmpty<T>(this ICollection<T> collection) => collection == null || collection.Count == 0;

    /// <summary>
    /// Determines whether the specified collection is empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">The collection.</param>
    /// <returns><c>true</c> if the specified collection is empty; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(this T[] collection) => collection == null || collection.Length == 0;

    /// <summary>
    /// Converts to set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items">The items.</param>
    /// <returns>HashSet&lt;T&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashSet<T> ToSet<T>(this IEnumerable<T> items) => [..items];

    /// <summary>
    /// Eaches the specified action.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values.</param>
    /// <param name="action">The action.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Each<T>(this IEnumerable<T> values, Action<T> action)
    {
        if (values == null) return;

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
        if (map == null) return;

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
        if (items == null)
            return [];

        var list = new List<To>();
        foreach (var item in items)
        {
            list.Add(converter(item));
        }
        return list;
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
        return items == null ? [] : (from object item in items select converter(item)).ToList();
    }

    /// <summary>
    /// Equivalents to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">The array.</param>
    /// <param name="otherArray">The other array.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EquivalentTo<T>(this T[] array, T[] otherArray, Func<T, T, bool> comparer = null)
    {
        if (array == null || otherArray == null)
            return array == otherArray;

        if (array.Length != otherArray.Length)
            return false;

        comparer ??= (v1, v2) => v1.Equals(v2);

        return !array.Where((t, i) => !comparer(t, otherArray[i])).Any();
    }

    /// <summary>
    /// Equivalents to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="thisList">The this list.</param>
    /// <param name="otherList">The other list.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool EquivalentTo<T>(this IEnumerable<T> thisList, IEnumerable<T> otherList, Func<T, T, bool> comparer = null)
    {
        comparer ??= (v1, v2) => v1.Equals(v2);

        if (thisList == null || otherList == null)
            return thisList == otherList;

        var otherEnum = otherList.GetEnumerator();
        foreach (var item in thisList)
        {
            if (!otherEnum.MoveNext()) return false;

            var thisIsDefault = Equals(item, default(T));
            var otherIsDefault = Equals(otherEnum.Current, default(T));
            if (thisIsDefault || otherIsDefault)
            {
                return thisIsDefault && otherIsDefault;
            }

            if (!comparer(item, otherEnum.Current)) return false;
        }
        var hasNoMoreLeftAsWell = !otherEnum.MoveNext();
        return hasNoMoreLeftAsWell;
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
    public static Dictionary<TKey, T> ToSafeDictionary<T, TKey>(this IEnumerable<T> list, Func<T, TKey> expr)
    {
        var map = new Dictionary<TKey, T>();
        if (list != null)
        {
            foreach (var item in list)
            {
                map[expr(item)] = item;
            }
        }
        return map;
    }

    /// <summary>
    /// Return T[0] when enumerable is null, safe to use in enumerations like foreach
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>IEnumerable&lt;T&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> Safe<T>(this IEnumerable<T> enumerable)
    {
        return enumerable ?? TypeConstants<T>.EmptyArray;
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

}