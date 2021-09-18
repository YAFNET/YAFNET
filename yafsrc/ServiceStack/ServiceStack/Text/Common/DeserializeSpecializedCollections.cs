// ***********************************************************************
// <copyright file="DeserializeSpecializedCollections.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;

namespace ServiceStack.Text.Common
{
    using System.Linq;

    /// <summary>
    /// Class DeserializeSpecializedCollections.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    internal static class DeserializeSpecializedCollections<T, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>
        /// The cache function
        /// </summary>
        private static readonly ParseStringSpanDelegate CacheFn;

        /// <summary>
        /// Initializes static members of the <see cref="DeserializeSpecializedCollections{T, TSerializer}" /> class.
        /// </summary>
        static DeserializeSpecializedCollections()
        {
            CacheFn = GetParseStringSpanFn();
        }

        /// <summary>
        /// Gets the parse.
        /// </summary>
        /// <value>The parse.</value>
        public static ParseStringDelegate Parse => v => CacheFn(v.AsSpan());

        /// <summary>
        /// Gets the parse string span.
        /// </summary>
        /// <value>The parse string span.</value>
        public static ParseStringSpanDelegate ParseStringSpan => CacheFn;

        /// <summary>
        /// Gets the parse string span function.
        /// </summary>
        /// <returns>ParseStringSpanDelegate.</returns>
        public static ParseStringSpanDelegate GetParseStringSpanFn()
        {
            if (typeof(T).HasAnyTypeDefinitionsOf(typeof(Queue<>)))
            {
                if (typeof(T) == typeof(Queue<string>))
                    return ParseStringQueue;

                return typeof(T) == typeof(Queue<int>) ? ParseIntQueue : GetGenericQueueParseFn();
            }

            if (typeof(T).HasAnyTypeDefinitionsOf(typeof(Stack<>)))
            {
                if (typeof(T) == typeof(Stack<string>))
                    return ParseStringStack;

                return typeof(T) == typeof(Stack<int>) ? ParseIntStack : GetGenericStackParseFn();
            }

            var fn = PclExport.Instance.GetSpecializedCollectionParseStringSpanMethod<TSerializer>(typeof(T));
            if (fn != null)
                return fn;

            if (typeof(T) == typeof(IEnumerable) || typeof(T) == typeof(ICollection))
            {
                return GetEnumerableParseStringSpanFn();
            }

            return GetGenericEnumerableParseStringSpanFn();
        }

        /// <summary>
        /// Parses the string queue.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Queue&lt;System.String&gt;.</returns>
        public static Queue<string> ParseStringQueue(ReadOnlySpan<char> value)
        {
            var parse = (IEnumerable<string>)DeserializeList<List<string>, TSerializer>.ParseStringSpan(value);
            return new Queue<string>(parse);
        }

        /// <summary>
        /// Parses the int queue.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Queue&lt;System.Int32&gt;.</returns>
        public static Queue<int> ParseIntQueue(ReadOnlySpan<char> value)
        {
            var parse = (IEnumerable<int>)DeserializeList<List<int>, TSerializer>.ParseStringSpan(value);
            return new Queue<int>(parse);
        }

        /// <summary>
        /// Gets the generic queue parse function.
        /// </summary>
        /// <returns>ParseStringSpanDelegate.</returns>
        internal static ParseStringSpanDelegate GetGenericQueueParseFn()
        {
            var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));
            var elementType = enumerableInterface.GetGenericArguments()[0];
            var genericType = typeof(SpecializedQueueElements<>).MakeGenericType(elementType);
            var mi = genericType.GetStaticMethod("ConvertToQueue");
            var convertToQueue = (ConvertObjectDelegate)mi.MakeDelegate(typeof(ConvertObjectDelegate));

            var parseFn = DeserializeEnumerable<T, TSerializer>.GetParseStringSpanFn();

            return x => convertToQueue(parseFn(x));
        }

        /// <summary>
        /// Parses the string stack.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Stack&lt;System.String&gt;.</returns>
        public static Stack<string> ParseStringStack(string value) => ParseStringStack(value.AsSpan());

        /// <summary>
        /// Parses the string stack.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Stack&lt;System.String&gt;.</returns>
        public static Stack<string> ParseStringStack(ReadOnlySpan<char> value)
        {
            var parse = (IEnumerable<string>)DeserializeList<List<string>, TSerializer>.ParseStringSpan(value);
            return new Stack<string>(parse);
        }

        /// <summary>
        /// Parses the int stack.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Stack&lt;System.Int32&gt;.</returns>
        public static Stack<int> ParseIntStack(string value) => ParseIntStack(value.AsSpan());

        /// <summary>
        /// Parses the int stack.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Stack&lt;System.Int32&gt;.</returns>
        public static Stack<int> ParseIntStack(ReadOnlySpan<char> value)
        {
            var parse = (IEnumerable<int>)DeserializeList<List<int>, TSerializer>.ParseStringSpan(value);
            return new Stack<int>(parse);
        }

        /// <summary>
        /// Gets the generic stack parse function.
        /// </summary>
        /// <returns>ParseStringSpanDelegate.</returns>
        internal static ParseStringSpanDelegate GetGenericStackParseFn()
        {
            var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));

            var elementType = enumerableInterface.GetGenericArguments()[0];
            var genericType = typeof(SpecializedQueueElements<>).MakeGenericType(elementType);
            var mi = genericType.GetStaticMethod("ConvertToStack");
            var convertToQueue = (ConvertObjectDelegate)mi.MakeDelegate(typeof(ConvertObjectDelegate));

            var parseFn = DeserializeEnumerable<T, TSerializer>.GetParseStringSpanFn();

            return x => convertToQueue(parseFn(x));
        }

        /// <summary>
        /// Gets the enumerable parse string span function.
        /// </summary>
        /// <returns>ParseStringSpanDelegate.</returns>
        public static ParseStringSpanDelegate GetEnumerableParseStringSpanFn() => DeserializeListWithElements<TSerializer>.ParseStringList;

        /// <summary>
        /// Gets the generic enumerable parse string span function.
        /// </summary>
        /// <returns>ParseStringSpanDelegate.</returns>
        public static ParseStringSpanDelegate GetGenericEnumerableParseStringSpanFn()
        {
            var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));
            if (enumerableInterface == null) return null;
            var elementType = enumerableInterface.GetGenericArguments()[0];
            var genericType = typeof(SpecializedEnumerableElements<,>).MakeGenericType(typeof(T), elementType);
            var fi = genericType.GetPublicStaticField("ConvertFn");

            var convertFn = fi.GetValue(null) as ConvertObjectDelegate;
            if (convertFn == null) return null;

            var parseFn = DeserializeEnumerable<T, TSerializer>.GetParseStringSpanFn();

            return x => convertFn(parseFn(x));
        }
    }

    /// <summary>
    /// Class SpecializedQueueElements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SpecializedQueueElements<T>
    {
        /// <summary>
        /// Converts to stack.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>Stack&lt;T&gt;.</returns>
        public static Stack<T> ConvertToStack(object enumerable)
        {
            return enumerable == null ? null : new Stack<T>((IEnumerable<T>)enumerable);
        }
    }

    /// <summary>
    /// Class SpecializedEnumerableElements.
    /// </summary>
    /// <typeparam name="TCollection">The type of the t collection.</typeparam>
    /// <typeparam name="T"></typeparam>
    internal class SpecializedEnumerableElements<TCollection, T>
    {
        /// <summary>
        /// The convert function
        /// </summary>
        public static ConvertObjectDelegate ConvertFn;

        /// <summary>
        /// Initializes static members of the <see cref="SpecializedEnumerableElements{TCollection, T}" /> class.
        /// </summary>
        static SpecializedEnumerableElements()
        {
            if ((from ctorInfo in typeof(TCollection).GetConstructors()
                select ctorInfo.GetParameters()
                into ctorParams
                where ctorParams.Length == 1
                select ctorParams[0]).Any(
                ctorParam => typeof(IEnumerable).IsAssignableFrom(ctorParam.ParameterType) ||
                             ctorParam.ParameterType.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>))))
            {
                ConvertFn = fromObject =>
                {
                    var to = Activator.CreateInstance(typeof(TCollection), fromObject);
                    return to;
                };
                return;
            }

            if (typeof(TCollection).IsOrHasGenericInterfaceTypeOf(typeof(ICollection<>)))
            {
                ConvertFn = ConvertFromCollection;
            }
        }

        /// <summary>
        /// Converts the specified enumerable.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>System.Object.</returns>
        public static object Convert(object enumerable)
        {
            return ConvertFn(enumerable);
        }

        /// <summary>
        /// Converts from collection.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>System.Object.</returns>
        public static object ConvertFromCollection(object enumerable)
        {
            var to = (ICollection<T>)typeof(TCollection).CreateInstance();
            var from = (IEnumerable<T>)enumerable;
            foreach (var item in from)
            {
                to.Add(item);
            }
            return to;
        }
    }
}
