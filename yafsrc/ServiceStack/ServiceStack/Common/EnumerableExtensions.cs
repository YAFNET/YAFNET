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
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack
{
    /// <summary>
    /// Class EnumerableUtils.
    /// </summary>
    public static class EnumerableUtils
    {
        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>System.Object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object FirstOrDefault(IEnumerable items)
        {
            if (items == null)
                return null;

            foreach (var item in items)
            {
                return item;
            }

            return null;
        }

        /// <summary>
        /// Elements at.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ElementAt(IEnumerable items, int index)
        {
            if (items == null)
                return null;
            var i = 0;
            foreach (var item in items)
                if (i++ == index)
                    return item;
            return null;
        }

        /// <summary>
        /// Skips the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="count">The count.</param>
        /// <returns>List&lt;System.Object&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<object> Skip(IEnumerable items, int count)
        {
            if (items == null)
                return TypeConstants.EmptyObjectList;
            var to = new List<object>();
            int i = 0;
            foreach (var item in items)
            {
                if (count > i++)
                    continue;

                to.Add(item);
            }
            return to;
        }

        /// <summary>
        /// Splits the on first.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="first">The first.</param>
        /// <returns>List&lt;System.Object&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<object> SplitOnFirst(IEnumerable items, out object first)
        {
            first = null;
            if (items == null)
                return TypeConstants.EmptyObjectList;
            var to = new List<object>();
            int i = 0;
            foreach (var item in items)
            {
                if (i++ < 1)
                {
                    first = item;
                    continue;
                }
                to.Add(item);
            }
            return to;
        }

        /// <summary>
        /// Takes the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="count">The count.</param>
        /// <returns>List&lt;System.Object&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<object> Take(IEnumerable items, int count)
        {
            if (items == null)
                return TypeConstants.EmptyObjectList;
            var to = new List<object>();
            int i = 0;
            foreach (var item in items)
            {
                if (count > i++)
                {
                    to.Add(item);
                    continue;
                }
                return to;
            }
            return to;
        }

        /// <summary>
        /// Counts the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(IEnumerable items)
        {
            if (items == null)
                return 0;
            return items is ICollection c
                ? c.Count
                : items.Cast<object>().Count();
        }

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
        /// Converts to hashset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        [Obsolete("Use ToSet() or 'using System.Linq;'")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items) => new(items);

        /// <summary>
        /// Converts to set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashSet<T> ToSet<T>(this IEnumerable<T> items) => new(items);

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
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <param name="action">The action.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Each<T>(this IEnumerable<T> values, Action<int, T> action)
        {
            if (values == null) return;

            var i = 0;
            foreach (var value in values)
            {
                action(i++, value);
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
                return new List<To>();

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
        public static List<To> Map<To>(this System.Collections.IEnumerable items, Func<object, To> converter)
        {
            if (items == null)
                return new List<To>();

            var list = new List<To>();
            foreach (var item in items)
            {
                list.Add(converter(item));
            }
            return list;
        }

        /// <summary>
        /// Converts to objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns>List&lt;System.Object&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<object> ToObjects<T>(this IEnumerable<T> items)
        {
            var to = new List<object>();
            foreach (var item in items)
            {
                to.Add(item);
            }
            return to;
        }

        /// <summary>
        /// Firsts the non default or empty.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>System.String.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FirstNonDefaultOrEmpty(this IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(value)) return value;
            }
            return null;
        }

        /// <summary>
        /// Firsts the non default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns>T.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FirstNonDefault<T>(this IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                if (!Equals(value, default(T))) return value;
            }
            return default(T);
        }

        /// <summary>
        /// Equivalents to.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EquivalentTo(this byte[] bytes, byte[] other)
        {
            if (bytes == null || other == null)
                return bytes == other;

            if (bytes.Length != other.Length)
                return false;

            var compare = 0;
            for (var i = 0; i < other.Length; i++)
                compare |= other[i] ^ bytes[i];

            return compare == 0;
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

            if (comparer == null)
                comparer = (v1, v2) => v1.Equals(v2);

            for (var i = 0; i < array.Length; i++)
            {
                if (!comparer(array[i], otherArray[i]))
                    return false;
            }

            return true;
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
            if (comparer == null)
                comparer = (v1, v2) => v1.Equals(v2);

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
        /// Equivalents to.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool EquivalentTo<K, V>(this IDictionary<K, V> a, IDictionary<K, V> b, Func<V, V, bool> comparer = null)
        {
            if (comparer == null)
                comparer = (v1, v2) => v1.Equals(v2);

            if (a == null || b == null)
                return a == b;

            if (a.Count != b.Count)
                return false;

            foreach (var entry in a)
            {
                V value;
                if (!b.TryGetValue(entry.Key, out value))
                    return false;
                if (entry.Value == null || value == null)
                {
                    if (entry.Value == null && value == null)
                        continue;

                    return false;
                }
                if (!comparer(entry.Value, value))
                    return false;
            }
            return true;
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
        /// Converts to dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="map">The map.</param>
        /// <returns>Dictionary&lt;TKey, TValue&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this IEnumerable<T> list, Func<T, KeyValuePair<TKey, TValue>> map)
        {
            var to = new Dictionary<TKey, TValue>();
            foreach (var item in list)
            {
                var entry = map(item);
                to[entry.Key] = entry.Value;
            }
            return to;
        }

        /// <summary>
        /// Return T[0] when enumerable is null, safe to use in enumerations like foreach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Safe<T>(this IEnumerable<T> enumerable) => enumerable ?? TypeConstants<T>.EmptyArray;

        /// <summary>
        /// Safes the specified enumerable.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>IEnumerable.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable Safe(this IEnumerable enumerable) => enumerable ?? TypeConstants.EmptyObjectArray;

        /// <summary>
        /// All as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">source</exception>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public static async Task<bool> AllAsync<T>(this IEnumerable<T> source, Func<T, Task<bool>> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            foreach (var item in source)
            {
                var result = await predicate(item).ConfigAwait();
                if (!result)
                    return false;
            }
            return true;
        }

        // This is for synchronous predicates with an async source.
        /// <summary>
        /// All as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">source</exception>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public static async Task<bool> AllAsync<T>(this IEnumerable<Task<T>> source, Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            foreach (var item in source)
            {
                var awaitedItem = await item.ConfigAwait();
                if (!predicate(awaitedItem))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Any as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">source</exception>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public static async Task<bool> AnyAsync<T>(this IEnumerable<T> source, Func<T, Task<bool>> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            foreach (var item in source)
            {
                var result = await predicate(item).ConfigAwait();
                if (result)
                    return true;
            }
            return false;
        }

        // This is for synchronous predicates with an async source.
        /// <summary>
        /// Any as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">source</exception>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public static async Task<bool> AnyAsync<T>(this IEnumerable<Task<T>> source, Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            foreach (var item in source)
            {
                var awaitedItem = await item.ConfigAwait();
                if (predicate(awaitedItem))
                    return true;
            }
            return false;
        }

    }
}