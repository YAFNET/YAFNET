// ***********************************************************************
// <copyright file="ListExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text
{
    using System;
    using System.Collections.Generic;

    using ServiceStack.Text.Common;

    /// <summary>
    /// Class ListExtensions.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Joins the specified values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns>System.String.</returns>
        public static string Join<T>(this IEnumerable<T> values)
        {
            return Join(values, JsWriter.ItemSeperatorString);
        }

        /// <summary>
        /// Joins the specified seperator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns>System.String.</returns>
        public static string Join<T>(this IEnumerable<T> values, string seperator)
        {
            var sb = StringBuilderThreadStatic.Allocate();
            foreach (var value in values)
            {
                if (sb.Length > 0)
                    sb.Append(seperator);
                sb.Append(value);
            }
            return StringBuilderThreadStatic.ReturnAndFree(sb);
        }

        /// <summary>
        /// Determines whether [is null or empty] [the specified list].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns><c>true</c> if [is null or empty] [the specified list]; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list == null || list.Count == 0;
        }

        //TODO: make it work

        /// <summary>
        /// Nullables the count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns>System.Int32.</returns>
        public static int NullableCount<T>(this List<T> list)
        {
            return list == null ? 0 : list.Count;
        }

        /// <summary>
        /// Adds if not exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="item">The item.</param>
        public static void AddIfNotExists<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        /// <summary>
        /// Creates new array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="with">The with.</param>
        /// <param name="without">The without.</param>
        /// <returns>T[].</returns>
        public static T[] NewArray<T>(this T[] array, T with = null, T without = null) where T : class
        {
            var to = new List<T>(array);

            if (with != null)
                to.Add(with);

            if (without != null)
                to.Remove(without);

            return to.ToArray();
        }

        /// <summary>
        /// Ins the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public static List<T> InList<T>(this T value)
        {
            return new List<T> { value };
        }

        /// <summary>
        /// Ins the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T[].</returns>
        public static T[] InArray<T>(this T value)
        {
            return new[] { value };
        }

        /// <summary>
        /// Adds the specified types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="types">The types.</param>
        /// <returns>List&lt;Type&gt;.</returns>
        public static List<Type> Add<T>(this List<Type> types)
        {
            types.Add(typeof(T));
            return types;
        }
    }
}