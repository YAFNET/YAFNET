// ***********************************************************************
// <copyright file="CollectionExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System;
using System.Collections.Generic;

/// <summary>
/// Class CollectionExtensions.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Creates the and populate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ofCollectionType">Type of the of collection.</param>
    /// <param name="withItems">The with items.</param>
    /// <returns>ICollection&lt;T&gt;.</returns>
    public static ICollection<T> CreateAndPopulate<T>(Type ofCollectionType, T[] withItems)
    {
        if (withItems == null)
            return null;

        if (ofCollectionType == null)
            return new List<T>(withItems);

        var genericType = ofCollectionType.FirstGenericType();
        var genericTypeDefinition = genericType != null
                                        ? genericType.GetGenericTypeDefinition()
                                        : null;

        if (genericTypeDefinition == typeof(HashSet<>))
            return new HashSet<T>(withItems);

        if (genericTypeDefinition == typeof(LinkedList<>))
            return new LinkedList<T>(withItems);

        var collection = (ICollection<T>)ofCollectionType.CreateInstance();
        foreach (var item in withItems)
        {
            collection.Add(item);
        }
        return collection;
    }

    /// <summary>
    /// Converts to array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">The collection.</param>
    /// <returns>T[].</returns>
    public static T[] ToArray<T>(this ICollection<T> collection)
    {
        var to = new T[collection.Count];
        collection.CopyTo(to, 0);
        return to;
    }

    /// <summary>
    /// Converts the specified object collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objCollection">The object collection.</param>
    /// <param name="toCollectionType">Type of to collection.</param>
    /// <returns>System.Object.</returns>
    public static object Convert<T>(object objCollection, Type toCollectionType)
    {
        var collection = (ICollection<T>)objCollection;
        var to = new T[collection.Count];
        collection.CopyTo(to, 0);
        return CreateAndPopulate(toCollectionType, to);
    }
}