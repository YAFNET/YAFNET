// ***********************************************************************
// <copyright file="WriteLists.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ServiceStack.Text.Common;

/// <summary>
/// Class WriteListsOfElements.
/// </summary>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
public static class WriteListsOfElements<TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The serializer
    /// </summary>
    private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// The list cache FNS
    /// </summary>
    static Dictionary<Type, WriteObjectDelegate> ListCacheFns = new();

    /// <summary>
    /// Gets the list write function.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <returns>WriteObjectDelegate.</returns>
    public static WriteObjectDelegate GetListWriteFn(Type elementType)
    {
        WriteObjectDelegate writeFn;
        if (ListCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

        var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
        var mi = genericType.GetStaticMethod("WriteList");
        writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

        Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
        do
        {
            snapshot = ListCacheFns;
            newCache = new Dictionary<Type, WriteObjectDelegate>(ListCacheFns);
            newCache[elementType] = writeFn;

        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref ListCacheFns, newCache, snapshot), snapshot));

        return writeFn;
    }


    /// <summary>
    /// The i list cache FNS
    /// </summary>
    static Dictionary<Type, WriteObjectDelegate> IListCacheFns = new();

    /// <summary>
    /// Gets the i list write function.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <returns>WriteObjectDelegate.</returns>
    public static WriteObjectDelegate GetIListWriteFn(Type elementType)
    {
        WriteObjectDelegate writeFn;
        if (IListCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

        var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
        var mi = genericType.GetStaticMethod("WriteIList");
        writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

        Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
        do
        {
            snapshot = IListCacheFns;
            newCache = new Dictionary<Type, WriteObjectDelegate>(IListCacheFns);
            newCache[elementType] = writeFn;

        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref IListCacheFns, newCache, snapshot), snapshot));

        return writeFn;
    }

    /// <summary>
    /// The cache FNS
    /// </summary>
    static Dictionary<Type, WriteObjectDelegate> CacheFns = new();

    /// <summary>
    /// Gets the generic write array.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <returns>WriteObjectDelegate.</returns>
    public static WriteObjectDelegate GetGenericWriteArray(Type elementType)
    {
        WriteObjectDelegate writeFn;
        if (CacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

        var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
        var mi = genericType.GetStaticMethod("WriteArray");
        writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

        Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
        do
        {
            snapshot = CacheFns;
            newCache = new Dictionary<Type, WriteObjectDelegate>(CacheFns);
            newCache[elementType] = writeFn;

        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref CacheFns, newCache, snapshot), snapshot));

        return writeFn;
    }

    /// <summary>
    /// The enumerable cache FNS
    /// </summary>
    static Dictionary<Type, WriteObjectDelegate> EnumerableCacheFns = new();

    /// <summary>
    /// Gets the generic write enumerable.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <returns>WriteObjectDelegate.</returns>
    public static WriteObjectDelegate GetGenericWriteEnumerable(Type elementType)
    {
        WriteObjectDelegate writeFn;
        if (EnumerableCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

        var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
        var mi = genericType.GetStaticMethod("WriteEnumerable");
        writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

        Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
        do
        {
            snapshot = EnumerableCacheFns;
            newCache = new Dictionary<Type, WriteObjectDelegate>(EnumerableCacheFns);
            newCache[elementType] = writeFn;

        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref EnumerableCacheFns, newCache, snapshot), snapshot));

        return writeFn;
    }

    /// <summary>
    /// The list value type cache FNS
    /// </summary>
    static Dictionary<Type, WriteObjectDelegate> ListValueTypeCacheFns = new();

    /// <summary>
    /// Gets the type of the write list value.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <returns>WriteObjectDelegate.</returns>
    public static WriteObjectDelegate GetWriteListValueType(Type elementType)
    {
        WriteObjectDelegate writeFn;
        if (ListValueTypeCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

        var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
        var mi = genericType.GetStaticMethod("WriteListValueType");
        writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

        Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
        do
        {
            snapshot = ListValueTypeCacheFns;
            newCache = new Dictionary<Type, WriteObjectDelegate>(ListValueTypeCacheFns);
            newCache[elementType] = writeFn;

        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref ListValueTypeCacheFns, newCache, snapshot), snapshot));

        return writeFn;
    }

    /// <summary>
    /// The i list value type cache FNS
    /// </summary>
    static Dictionary<Type, WriteObjectDelegate> IListValueTypeCacheFns = new();

    /// <summary>
    /// Gets the type of the write i list value.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <returns>WriteObjectDelegate.</returns>
    public static WriteObjectDelegate GetWriteIListValueType(Type elementType)
    {
        WriteObjectDelegate writeFn;

        if (IListValueTypeCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

        var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
        var mi = genericType.GetStaticMethod("WriteIListValueType");
        writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

        Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
        do
        {
            snapshot = IListValueTypeCacheFns;
            newCache = new Dictionary<Type, WriteObjectDelegate>(IListValueTypeCacheFns);
            newCache[elementType] = writeFn;

        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref IListValueTypeCacheFns, newCache, snapshot), snapshot));

        return writeFn;
    }

    /// <summary>
    /// Writes the i enumerable.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oValueCollection">The o value collection.</param>
    public static void WriteIEnumerable(TextWriter writer, object oValueCollection)
    {
        WriteObjectDelegate toStringFn = null;

        writer.Write(JsWriter.ListStartChar);

        var valueCollection = (IEnumerable)oValueCollection;
        var ranOnce = false;
        Type lastType = null;
        foreach (var valueItem in valueCollection)
        {
            if (toStringFn == null || valueItem != null && valueItem.GetType() != lastType)
            {
                if (valueItem != null)
                {
                    if (valueItem.GetType() != lastType)
                    {
                        lastType = valueItem.GetType();
                        toStringFn = Serializer.GetWriteFn(lastType);
                    }
                }
                else
                {
                    // this can happen if the first item in the collection was null
                    lastType = typeof(object);
                    toStringFn = Serializer.GetWriteFn(lastType);
                }
            }

            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

            toStringFn(writer, valueItem);
        }

        writer.Write(JsWriter.ListEndChar);
    }
}

/// <summary>
/// Class WriteListsOfElements.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
public static class WriteListsOfElements<T, TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The element write function
    /// </summary>
    private static readonly WriteObjectDelegate ElementWriteFn;

    /// <summary>
    /// Initializes static members of the <see cref="WriteListsOfElements{T, TSerializer}" /> class.
    /// </summary>
    static WriteListsOfElements()
    {
        var fn = JsWriter.GetTypeSerializer<TSerializer>().GetWriteFn<T>();
        ElementWriteFn = (writer, obj) =>
            {
                try
                {
                    if (!JsState.Traverse(obj))
                        return;

                    fn(writer, obj);
                }
                finally
                {
                    JsState.UnTraverse();
                }
            };
    }

    /// <summary>
    /// Writes the list.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oList">The o list.</param>
    public static void WriteList(TextWriter writer, object oList)
    {
        WriteGenericIList(writer, (IList<T>)oList);
    }

    /// <summary>
    /// Writes the generic list.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="list">The list.</param>
    public static void WriteGenericList(TextWriter writer, List<T> list)
    {
        writer.Write(JsWriter.ListStartChar);

        var ranOnce = false;
        var listLength = list.Count;
        for (var i = 0; i < listLength; i++)
        {
            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
            ElementWriteFn(writer, list[i]);
        }

        writer.Write(JsWriter.ListEndChar);
    }

    /// <summary>
    /// Writes the type of the list value.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="list">The list.</param>
    public static void WriteListValueType(TextWriter writer, object list)
    {
        WriteGenericListValueType(writer, (List<T>)list);
    }

    /// <summary>
    /// Writes the type of the generic list value.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="list">The list.</param>
    public static void WriteGenericListValueType(TextWriter writer, List<T> list)
    {
        if (list == null) return; //AOT

        writer.Write(JsWriter.ListStartChar);

        var ranOnce = false;
        var listLength = list.Count;
        for (var i = 0; i < listLength; i++)
        {
            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
            ElementWriteFn(writer, list[i]);
        }

        writer.Write(JsWriter.ListEndChar);
    }

    /// <summary>
    /// Writes the i list.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oList">The o list.</param>
    public static void WriteIList(TextWriter writer, object oList)
    {
        WriteGenericIList(writer, (IList<T>)oList);
    }

    /// <summary>
    /// Writes the generic i list.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="list">The list.</param>
    public static void WriteGenericIList(TextWriter writer, IList<T> list)
    {
        if (list == null) return;
        writer.Write(JsWriter.ListStartChar);

        var ranOnce = false;
        var listLength = list.Count;
        try
        {
            for (var i = 0; i < listLength; i++)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                ElementWriteFn(writer, list[i]);
            }

        }
        catch (Exception ex)
        {
            Tracer.Instance.WriteError(ex);
            throw;
        }
        writer.Write(JsWriter.ListEndChar);
    }

    /// <summary>
    /// Writes the type of the i list value.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="list">The list.</param>
    public static void WriteIListValueType(TextWriter writer, object list)
    {
        WriteGenericIListValueType(writer, (IList<T>)list);
    }

    /// <summary>
    /// Writes the type of the generic i list value.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="list">The list.</param>
    public static void WriteGenericIListValueType(TextWriter writer, IList<T> list)
    {
        if (list == null) return; //AOT

        writer.Write(JsWriter.ListStartChar);

        var ranOnce = false;
        var listLength = list.Count;
        for (var i = 0; i < listLength; i++)
        {
            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
            ElementWriteFn(writer, list[i]);
        }

        writer.Write(JsWriter.ListEndChar);
    }

    /// <summary>
    /// Writes the array.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oArrayValue">The o array value.</param>
    public static void WriteArray(TextWriter writer, object oArrayValue)
    {
        if (oArrayValue == null) return;
        WriteGenericArray(writer, (Array)oArrayValue);
    }

    /// <summary>
    /// Writes the type of the generic array value.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oArray">The o array.</param>
    public static void WriteGenericArrayValueType(TextWriter writer, object oArray)
    {
        WriteGenericArrayValueType(writer, (T[])oArray);
    }

    /// <summary>
    /// Writes the type of the generic array value.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="array">The array.</param>
    public static void WriteGenericArrayValueType(TextWriter writer, T[] array)
    {
        if (array == null) return;
        writer.Write(JsWriter.ListStartChar);

        var ranOnce = false;
        var arrayLength = array.Length;
        for (var i = 0; i < arrayLength; i++)
        {
            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
            ElementWriteFn(writer, array[i]);
        }

        writer.Write(JsWriter.ListEndChar);
    }

    /// <summary>
    /// Writes the generic array multi dimension.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="array">The array.</param>
    /// <param name="rank">The rank.</param>
    /// <param name="indices">The indices.</param>
    private static void WriteGenericArrayMultiDimension(TextWriter writer, Array array, int rank, int[] indices)
    {
        var ranOnce = false;
        writer.Write(JsWriter.ListStartChar);
        for (int i = 0; i < array.GetLength(rank); i++)
        {
            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
            indices[rank] = i;

            if (rank < array.Rank - 1)
                WriteGenericArrayMultiDimension(writer, array, rank + 1, indices);
            else
                ElementWriteFn(writer, array.GetValue(indices));
        }
        writer.Write(JsWriter.ListEndChar);
    }

    /// <summary>
    /// Writes the generic array.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="array">The array.</param>
    public static void WriteGenericArray(TextWriter writer, Array array)
    {
        WriteGenericArrayMultiDimension(writer, array, 0, new int[array.Rank]);
    }
    /// <summary>
    /// Writes the enumerable.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="oEnumerable">The o enumerable.</param>
    public static void WriteEnumerable(TextWriter writer, object oEnumerable)
    {
        WriteGenericEnumerable(writer, (IEnumerable<T>)oEnumerable);
    }

    /// <summary>
    /// Writes the generic enumerable.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="enumerable">The enumerable.</param>
    public static void WriteGenericEnumerable(TextWriter writer, IEnumerable<T> enumerable)
    {
        if (enumerable == null) return;
        writer.Write(JsWriter.ListStartChar);

        var ranOnce = false;
        foreach (var value in enumerable)
        {
            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
            ElementWriteFn(writer, value);
        }

        writer.Write(JsWriter.ListEndChar);
    }
}

/// <summary>
/// Class WriteLists.
/// </summary>
internal static class WriteLists
{
    /// <summary>
    /// Writes the list string.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="list">The list.</param>
    public static void WriteListString(ITypeSerializer serializer, TextWriter writer, object list)
    {
        WriteListString(serializer, writer, (List<string>)list);
    }

    /// <summary>
    /// Writes the list string.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="list">The list.</param>
    public static void WriteListString(ITypeSerializer serializer, TextWriter writer, List<string> list)
    {
        writer.Write(JsWriter.ListStartChar);

        var ranOnce = false;
        foreach (var x in list)
        {
            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
            serializer.WriteString(writer, x);
        }

        writer.Write(JsWriter.ListEndChar);
    }

    /// <summary>
    /// Writes the i list string.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="list">The list.</param>
    public static void WriteIListString(ITypeSerializer serializer, TextWriter writer, object list)
    {
        WriteIListString(serializer, writer, (IList<string>)list);
    }

    /// <summary>
    /// Writes the i list string.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="list">The list.</param>
    public static void WriteIListString(ITypeSerializer serializer, TextWriter writer, IList<string> list)
    {
        writer.Write(JsWriter.ListStartChar);

        var ranOnce = false;
        var listLength = list.Count;
        for (var i = 0; i < listLength; i++)
        {
            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
            serializer.WriteString(writer, list[i]);
        }

        writer.Write(JsWriter.ListEndChar);
    }

    /// <summary>
    /// Writes the bytes.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="byteValue">The byte value.</param>
    public static void WriteBytes(ITypeSerializer serializer, TextWriter writer, object byteValue)
    {
        if (byteValue == null) return;
        serializer.WriteBytes(writer, byteValue);
    }

    /// <summary>
    /// Writes the string array.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="oList">The o list.</param>
    public static void WriteStringArray(ITypeSerializer serializer, TextWriter writer, object oList)
    {
        writer.Write(JsWriter.ListStartChar);

        var list = (string[])oList;
        var ranOnce = false;
        var listLength = list.Length;
        for (var i = 0; i < listLength; i++)
        {
            JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
            serializer.WriteString(writer, list[i]);
        }

        writer.Write(JsWriter.ListEndChar);
    }
}

/// <summary>
/// Class WriteLists.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
internal static class WriteLists<T, TSerializer>
    where TSerializer : ITypeSerializer
{
    /// <summary>
    /// The cache function
    /// </summary>
    private static readonly WriteObjectDelegate CacheFn;
    /// <summary>
    /// The serializer
    /// </summary>
    private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

    /// <summary>
    /// Initializes static members of the <see cref="WriteLists{T, TSerializer}" /> class.
    /// </summary>
    static WriteLists()
    {
        CacheFn = GetWriteFn();
    }

    /// <summary>
    /// Gets the write.
    /// </summary>
    /// <value>The write.</value>
    public static WriteObjectDelegate Write => CacheFn;

    /// <summary>
    /// Gets the write function.
    /// </summary>
    /// <returns>WriteObjectDelegate.</returns>
    /// <exception cref="System.ArgumentException"></exception>
    public static WriteObjectDelegate GetWriteFn()
    {
        var type = typeof(T);

        var listInterface = type.GetTypeWithGenericTypeDefinitionOf(typeof(IList<>));
        if (listInterface == null)
        {
            throw new ArgumentException($"Type {type.FullName} is not of type IList<>");
        }

        //optimized access for regularly used types
        if (type == typeof(List<string>))
            return (w, x) => WriteLists.WriteListString(Serializer, w, x);
        if (type == typeof(IList<string>))
            return (w, x) => WriteLists.WriteIListString(Serializer, w, x);

        if (type == typeof(List<int>))
            return WriteListsOfElements<int, TSerializer>.WriteListValueType;
        if (type == typeof(IList<int>))
            return WriteListsOfElements<int, TSerializer>.WriteIListValueType;

        if (type == typeof(List<long>))
            return WriteListsOfElements<long, TSerializer>.WriteListValueType;
        if (type == typeof(IList<long>))
            return WriteListsOfElements<long, TSerializer>.WriteIListValueType;

        var elementType = listInterface.GetGenericArguments()[0];

        var isGenericList = typeof(T).IsGenericType
                            && typeof(T).GetGenericTypeDefinition() == typeof(List<>);

        if (elementType.IsValueType
            && JsWriter.ShouldUseDefaultToStringMethod(elementType))
        {
            return isGenericList ? WriteListsOfElements<TSerializer>.GetWriteListValueType(elementType) : WriteListsOfElements<TSerializer>.GetWriteIListValueType(elementType);
        }

        return isGenericList
                   ? WriteListsOfElements<TSerializer>.GetListWriteFn(elementType)
                   : WriteListsOfElements<TSerializer>.GetIListWriteFn(elementType);
    }

}