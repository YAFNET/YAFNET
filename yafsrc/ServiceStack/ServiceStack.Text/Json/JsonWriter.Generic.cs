// ***********************************************************************
// <copyright file="JsonWriter.Generic.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using ServiceStack.Text.Common;

namespace ServiceStack.Text.Json
{
    /// <summary>
    /// Class JsonWriter.
    /// </summary>
    public static class JsonWriter
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly JsWriter<JsonTypeSerializer> Instance = new();

        /// <summary>
        /// The write function cache
        /// </summary>
        private static Dictionary<Type, WriteObjectDelegate> WriteFnCache = new();

        /// <summary>
        /// Removes the cache function.
        /// </summary>
        /// <param name="forType">For type.</param>
        internal static void RemoveCacheFn(Type forType)
        {
            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = WriteFnCache;
                newCache = new Dictionary<Type, WriteObjectDelegate>(WriteFnCache);
                newCache.Remove(forType);

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref WriteFnCache, newCache, snapshot), snapshot));
        }

        /// <summary>
        /// Gets the write function.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>WriteObjectDelegate.</returns>
        internal static WriteObjectDelegate GetWriteFn(Type type)
        {
            try
            {
                if (WriteFnCache.TryGetValue(type, out var writeFn)) return writeFn;

                var genericType = typeof(JsonWriter<>).MakeGenericType(type);
                var mi = genericType.GetStaticMethod("WriteFn");
                var writeFactoryFn = (Func<WriteObjectDelegate>)mi.MakeDelegate(typeof(Func<WriteObjectDelegate>));
                writeFn = writeFactoryFn();

                Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
                do
                {
                    snapshot = WriteFnCache;
                    newCache = new Dictionary<Type, WriteObjectDelegate>(WriteFnCache)
                    {
                        [type] = writeFn
                    };

                } while (!ReferenceEquals(
                    Interlocked.CompareExchange(ref WriteFnCache, newCache, snapshot), snapshot));

                return writeFn;
            }
            catch (Exception ex)
            {
                Tracer.Instance.WriteError(ex);
                throw;
            }
        }

        /// <summary>
        /// The json type information cache
        /// </summary>
        private static Dictionary<Type, TypeInfo> JsonTypeInfoCache = new();

        /// <summary>
        /// Gets the type information.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>TypeInfo.</returns>
        internal static TypeInfo GetTypeInfo(Type type)
        {
            try
            {
                if (JsonTypeInfoCache.TryGetValue(type, out var writeFn)) return writeFn;

                var genericType = typeof(JsonWriter<>).MakeGenericType(type);
                var mi = genericType.GetStaticMethod("GetTypeInfo");
                var writeFactoryFn = (Func<TypeInfo>)mi.MakeDelegate(typeof(Func<TypeInfo>));
                writeFn = writeFactoryFn();

                Dictionary<Type, TypeInfo> snapshot, newCache;
                do
                {
                    snapshot = JsonTypeInfoCache;
                    newCache = new Dictionary<Type, TypeInfo>(JsonTypeInfoCache)
                    {
                        [type] = writeFn
                    };

                } while (!ReferenceEquals(
                    Interlocked.CompareExchange(ref JsonTypeInfoCache, newCache, snapshot), snapshot));

                return writeFn;
            }
            catch (Exception ex)
            {
                Tracer.Instance.WriteError(ex);
                throw;
            }
        }

        /// <summary>
        /// Writes the late bound object.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        internal static void WriteLateBoundObject(TextWriter writer, object value)
        {
            if (value == null)
            {
                writer.Write(JsonUtils.Null);
                return;
            }

            try
            {
                if (!JsState.Traverse(value))
                    return;

                var type = value.GetType();
                var writeFn = type == typeof(object)
                    ? WriteType<object, JsonTypeSerializer>.WriteObjectType
                    : GetWriteFn(type);

                var prevState = JsState.IsWritingDynamic;
                JsState.IsWritingDynamic = true;
                writeFn(writer, value);
                JsState.IsWritingDynamic = prevState;
            }
            finally
            {
                JsState.UnTraverse();
            }
        }

        /// <summary>
        /// Gets the value type to string method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>WriteObjectDelegate.</returns>
        internal static WriteObjectDelegate GetValueTypeToStringMethod(Type type)
        {
            return Instance.GetValueTypeToStringMethod(type);
        }

        /// <summary>
        /// Initializes the aot.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void InitAot<T>()
        {
            Text.Json.JsonWriter<T>.WriteFn();
            Text.Json.JsonWriter.Instance.GetWriteFn<T>();
            Text.Json.JsonWriter.Instance.GetValueTypeToStringMethod(typeof(T));
            JsWriter.GetTypeSerializer<Text.Json.JsonTypeSerializer>().GetWriteFn<T>();
        }
    }

    /// <summary>
    /// Class TypeInfo.
    /// </summary>
    public class TypeInfo
    {
        /// <summary>
        /// The encode map key
        /// </summary>
        internal bool EncodeMapKey;
    }

    /// <summary>
    /// Implement the serializer using a more static approach
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class JsonWriter<T>
    {
        /// <summary>
        /// The type information
        /// </summary>
        internal static TypeInfo TypeInfo;
        /// <summary>
        /// The cache function
        /// </summary>
        private static WriteObjectDelegate CacheFn;

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public static void Reset()
        {
            JsonWriter.RemoveCacheFn(typeof(T));
            Refresh();
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public static void Refresh()
        {
            if (JsonWriter.Instance == null)
                return;

            CacheFn = typeof(T) == typeof(object)
                ? JsonWriter.WriteLateBoundObject
                : JsonWriter.Instance.GetWriteFn<T>();
        }

        /// <summary>
        /// Writes the function.
        /// </summary>
        /// <returns>WriteObjectDelegate.</returns>
        public static WriteObjectDelegate WriteFn()
        {
            return CacheFn ?? WriteObject;
        }

        /// <summary>
        /// Gets the type information.
        /// </summary>
        /// <returns>TypeInfo.</returns>
        public static TypeInfo GetTypeInfo()
        {
            return TypeInfo;
        }

        /// <summary>
        /// Initializes static members of the <see cref="JsonWriter{T}" /> class.
        /// </summary>
        static JsonWriter()
        {
            if (JsonWriter.Instance == null)
                return;

            var isNumeric = typeof(T).IsNumericType();
            TypeInfo = new TypeInfo
            {
                EncodeMapKey = typeof(T) == typeof(bool) || isNumeric,
            };

            CacheFn = typeof(T) == typeof(object)
                ? JsonWriter.WriteLateBoundObject
                : JsonWriter.Instance.GetWriteFn<T>();
        }

        /// <summary>
        /// Writes the object.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        public static void WriteObject(TextWriter writer, object value)
        {
            TypeConfig<T>.Init();

            try
            {
                if (!JsState.Traverse(value))
                    return;

                CacheFn(writer, value);
            }
            finally
            {
                JsState.UnTraverse();
            }
        }

        /// <summary>
        /// Writes the root object.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        public static void WriteRootObject(TextWriter writer, object value)
        {
            TypeConfig<T>.Init();
            JsonSerializer.OnSerialize?.Invoke(value);

            JsState.Depth = 0;
            CacheFn(writer, value);
        }
    }

}