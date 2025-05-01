// ***********************************************************************
// <copyright file="JsvWriter.Generic.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using ServiceStack.OrmLite.Base.Text.Common;

namespace ServiceStack.OrmLite.Base.Text.Jsv;

/// <summary>
/// Class JsvWriter.
/// </summary>
public static class JsvWriter
{
    /// <summary>
    /// The instance
    /// </summary>
    public readonly static JsWriter<JsvTypeSerializer> Instance = new();

    /// <summary>
    /// The write function cache
    /// </summary>
    private static Dictionary<Type, WriteObjectDelegate> WriteFnCache = [];

    /// <summary>
    /// Removes the cache function.
    /// </summary>
    /// <param name="forType">For type.</param>
    static internal void RemoveCacheFn(Type forType)
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
    public static WriteObjectDelegate GetWriteFn(Type type)
    {
        try
        {
            if (WriteFnCache.TryGetValue(type, out var writeFn))
            {
                return writeFn;
            }

            var genericType = typeof(JsvWriter<>).MakeGenericType(type);
            var mi = genericType.GetStaticMethod("WriteFn");
            var writeFactoryFn = (Func<WriteObjectDelegate>)mi.MakeDelegate(typeof(Func<WriteObjectDelegate>));

            writeFn = writeFactoryFn();

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = WriteFnCache;
                newCache = new Dictionary<Type, WriteObjectDelegate>(WriteFnCache) {
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
    /// Writes the late bound object.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public static void WriteLateBoundObject(TextWriter writer, object value)
    {
        if (value == null)
        {
            return;
        }

        try
        {
            if (!JsState.Traverse(value))
            {
                return;
            }

            var type = value.GetType();
            var writeFn = type == typeof(object)
                              ? WriteType<object, JsvTypeSerializer>.WriteObjectType
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
}

/// <summary>
/// Implement the serializer using a more static approach
/// </summary>
/// <typeparam name="T"></typeparam>
public static class JsvWriter<T>
{
    /// <summary>
    /// The cache function
    /// </summary>
    private static WriteObjectDelegate CacheFn;

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public static void Reset()
    {
        JsvWriter.RemoveCacheFn(typeof(T));
        Refresh();
    }

    /// <summary>
    /// Refreshes this instance.
    /// </summary>
    public static void Refresh()
    {
        if (JsvWriter.Instance == null)
        {
            return;
        }

        CacheFn = typeof(T) == typeof(object)
                      ? JsvWriter.WriteLateBoundObject
                      : JsvWriter.Instance.GetWriteFn<T>();
        JsConfig.AddUniqueType(typeof(T));
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
    /// Initializes static members of the <see cref="JsvWriter{T}" /> class.
    /// </summary>
    static JsvWriter()
    {
        CacheFn = typeof(T) == typeof(object)
                      ? JsvWriter.WriteLateBoundObject
                      : JsvWriter.Instance.GetWriteFn<T>();
    }

    /// <summary>
    /// Writes the object.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    public static void WriteObject(TextWriter writer, object value)
    {
        if (writer == null)
        {
            return; //AOT
        }

        TypeConfig<T>.Init();

        try
        {
            if (!JsState.Traverse(value))
            {
                return;
            }

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
        if (writer == null)
        {
            return; //AOT
        }

        TypeConfig<T>.Init();
        TypeSerializer.OnSerialize?.Invoke(value);

        JsState.Depth = 0;
        CacheFn(writer, value);
    }

}