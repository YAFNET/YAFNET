// ***********************************************************************
// <copyright file="DeserializeCollection.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Threading;

namespace ServiceStack.Text.Common
{
    /// <summary>
    /// Class DeserializeCollection.
    /// </summary>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    internal static class DeserializeCollection<TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>
        /// The serializer
        /// </summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>
        /// Gets the parse method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringDelegate.</returns>
        public static ParseStringDelegate GetParseMethod(Type type) => v => GetParseStringSpanMethod(type)(v.AsSpan());

        /// <summary>
        /// Gets the parse string span method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringSpanDelegate.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static ParseStringSpanDelegate GetParseStringSpanMethod(Type type)
        {
            var collectionInterface = type.GetTypeWithGenericInterfaceOf(typeof(ICollection<>));
            if (collectionInterface == null)
            {
                throw new ArgumentException($"Type {type.FullName} is not of type ICollection<>");
            }

            //optimized access for regularly used types
            if (type.HasInterface(typeof(ICollection<string>)))
            {
                return value => ParseStringCollection(value, type);
            }

            if (type.HasInterface(typeof(ICollection<int>)))
            {
                return value => ParseIntCollection(value, type);
            }

            var elementType = collectionInterface.GetGenericArguments()[0];
            var supportedTypeParseMethod = Serializer.GetParseStringSpanFn(elementType);
            if (supportedTypeParseMethod != null)
            {
                var createCollectionType = type.HasAnyTypeDefinitionsOf(typeof(ICollection<>))
                    ? null : type;

                return value => ParseCollectionType(value, createCollectionType, elementType, supportedTypeParseMethod);
            }

            return null;
        }

        /// <summary>
        /// Parses the string collection.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <returns>ICollection&lt;System.String&gt;.</returns>
        public static ICollection<string> ParseStringCollection(string value, Type createType) => ParseStringCollection(value.AsSpan(), createType);

        /// <summary>
        /// Parses the string collection.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <returns>ICollection&lt;System.String&gt;.</returns>
        public static ICollection<string> ParseStringCollection(ReadOnlySpan<char> value, Type createType)
        {
            var items = DeserializeArrayWithElements<string, TSerializer>.ParseGenericArray(value, Serializer.ParseString);
            return CollectionExtensions.CreateAndPopulate(createType, items);
        }

        /// <summary>
        /// Parses the int collection.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <returns>ICollection&lt;System.Int32&gt;.</returns>
        public static ICollection<int> ParseIntCollection(string value, Type createType) => ParseIntCollection(value.AsSpan(), createType);

        /// <summary>
        /// Parses the int collection.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <returns>ICollection&lt;System.Int32&gt;.</returns>
        public static ICollection<int> ParseIntCollection(ReadOnlySpan<char> value, Type createType)
        {
            var items = DeserializeArrayWithElements<int, TSerializer>.ParseGenericArray(value, x => x.ParseInt32());
            return CollectionExtensions.CreateAndPopulate(createType, items);
        }

        /// <summary>
        /// Parses the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <param name="parseFn">The parse function.</param>
        /// <returns>ICollection&lt;T&gt;.</returns>
        public static ICollection<T> ParseCollection<T>(string value, Type createType, ParseStringDelegate parseFn) =>
            ParseCollection<T>(value.AsSpan(), createType, v => parseFn(v.ToString()));

        /// <summary>
        /// Parses the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <param name="parseFn">The parse function.</param>
        /// <returns>ICollection&lt;T&gt;.</returns>
        public static ICollection<T> ParseCollection<T>(ReadOnlySpan<char> value, Type createType, ParseStringSpanDelegate parseFn)
        {
            if (value.IsEmpty) return null;

            var items = DeserializeArrayWithElements<T, TSerializer>.ParseGenericArray(value, parseFn);
            return CollectionExtensions.CreateAndPopulate(createType, items);
        }

        /// <summary>
        /// The parse delegate cache
        /// </summary>
        private static Dictionary<Type, ParseCollectionDelegate> ParseDelegateCache
            = new();

        /// <summary>
        /// Delegate ParseCollectionDelegate
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <param name="parseFn">The parse function.</param>
        /// <returns>System.Object.</returns>
        private delegate object ParseCollectionDelegate(ReadOnlySpan<char> value, Type createType, ParseStringSpanDelegate parseFn);

        /// <summary>
        /// Parses the type of the collection.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="parseFn">The parse function.</param>
        /// <returns>System.Object.</returns>
        public static object ParseCollectionType(string value, Type createType, Type elementType, ParseStringDelegate parseFn) =>
            ParseCollectionType(value.AsSpan(), createType, elementType, v => parseFn(v.ToString()));


        /// <summary>
        /// The arguments
        /// </summary>
        static Type[] arguments = { typeof(ReadOnlySpan<char>), typeof(Type), typeof(ParseStringSpanDelegate) };

        /// <summary>
        /// Parses the type of the collection.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="parseFn">The parse function.</param>
        /// <returns>System.Object.</returns>
        public static object ParseCollectionType(ReadOnlySpan<char> value, Type createType, Type elementType, ParseStringSpanDelegate parseFn)
        {
            if (ParseDelegateCache.TryGetValue(elementType, out var parseDelegate))
                return parseDelegate(value, createType, parseFn);

            var mi = typeof(DeserializeCollection<TSerializer>).GetStaticMethod("ParseCollection", arguments);
            var genericMi = mi.MakeGenericMethod(new[] { elementType });
            parseDelegate = (ParseCollectionDelegate)genericMi.MakeDelegate(typeof(ParseCollectionDelegate));

            Dictionary<Type, ParseCollectionDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<Type, ParseCollectionDelegate>(ParseDelegateCache);
                newCache[elementType] = parseDelegate;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

            return parseDelegate(value, createType, parseFn);
        }
    }
}