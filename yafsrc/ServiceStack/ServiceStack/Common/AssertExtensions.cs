// ***********************************************************************
// <copyright file="AssertExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;

namespace ServiceStack
{
    /// <summary>
    /// Class AssertExtensions.
    /// </summary>
    public static class AssertExtensions
    {
        /// <summary>
        /// Throws the on first null.
        /// </summary>
        /// <param name="objs">The objs.</param>
        public static void ThrowOnFirstNull(params object[] objs)
        {
            foreach (var obj in objs)
            {
                ThrowIfNull(obj);
            }
        }

        /// <summary>
        /// Throws if null.
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void ThrowIfNull(this object obj)
        {
            ThrowIfNull(obj, null);
        }

        /// <summary>
        /// Throws if null.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="varName">Name of the variable.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void ThrowIfNull(this object obj, string varName)
        {
            if (obj == null)
                throw new ArgumentNullException(varName ?? "object");
        }

        /// <summary>
        /// Throws if null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="varName">Name of the variable.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static T ThrowIfNull<T>(this T obj, string varName)
        {
            if (obj == null)
                throw new ArgumentNullException(varName ?? "object");

            return obj;
        }

        /// <summary>
        /// Throws if null or empty.
        /// </summary>
        /// <param name="strValue">The string value.</param>
        /// <returns>System.String.</returns>
        public static string ThrowIfNullOrEmpty(this string strValue)
        {
            return ThrowIfNullOrEmpty(strValue, null);
        }

        /// <summary>
        /// Throws if null or empty.
        /// </summary>
        /// <param name="strValue">The string value.</param>
        /// <param name="varName">Name of the variable.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string ThrowIfNullOrEmpty(this string strValue, string varName)
        {
            if (string.IsNullOrEmpty(strValue))
                throw new ArgumentNullException(varName ?? "string");

            return strValue;
        }

        /// <summary>
        /// Throws if null or empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>ICollection.</returns>
        public static ICollection ThrowIfNullOrEmpty(this ICollection collection)
        {
            ThrowIfNullOrEmpty(collection, null);

            return collection;
        }

        /// <summary>
        /// Throws if null or empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="varName">Name of the variable.</param>
        /// <returns>ICollection.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static ICollection ThrowIfNullOrEmpty(this ICollection collection, string varName)
        {
            var fieldName = varName ?? "collection";

            if (collection == null)
                throw new ArgumentNullException(fieldName);

            if (collection.Count == 0)
                throw new ArgumentException(fieldName + " is empty");

            return collection;
        }

        /// <summary>
        /// Throws if null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>ICollection&lt;T&gt;.</returns>
        public static ICollection<T> ThrowIfNullOrEmpty<T>(this ICollection<T> collection)
        {
            ThrowIfNullOrEmpty(collection, null);

            return collection;
        }

        /// <summary>
        /// Throws if null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="varName">Name of the variable.</param>
        /// <returns>ICollection&lt;T&gt;.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static ICollection<T> ThrowIfNullOrEmpty<T>(this ICollection<T> collection, string varName)
        {
            var fieldName = varName ?? "collection";

            if (collection == null)
                throw new ArgumentNullException(fieldName);

            if (collection.Count == 0)
                throw new ArgumentException(fieldName + " is empty");

            return collection;
        }
    }
}