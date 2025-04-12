// ***********************************************************************
// <copyright file="OrmLiteUtilExtensionsAsync.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteUtilExtensionsAsync.
/// </summary>
static internal class OrmLiteUtilExtensionsAsync
{
    /// <summary>
    /// Creates the instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    public static T CreateInstance<T>()
    {
        return (T)ReflectionExtensions.CreateInstance<T>();
    }

    /// <summary>
    /// Convert to as an asynchronous operation.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="type">The type.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Object&gt; representing the asynchronous operation.</returns>
    public async static Task<object> ConvertToAsync(this IDataReader reader, IOrmLiteDialectProvider dialectProvider, Type type, CancellationToken token)
    {
        var modelDef = type.GetModelDefinition();
        var indexCache = reader.GetIndexFieldsCache(modelDef, dialectProvider);
        var values = new object[reader.FieldCount];

        using (reader)
        {
            return await dialectProvider.ReaderRead(reader, () =>
            {
                var row = type.CreateInstance();
                row.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
                return row;
            }, token).ConfigAwait();
        }
    }

    /// <summary>
    /// Convert to as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;T&gt; representing the asynchronous operation.</returns>
    public async static Task<T> ConvertToAsync<T>(this IDataReader reader, IOrmLiteDialectProvider dialectProvider, CancellationToken token)
    {
        using (reader)
        {
            return await dialectProvider.ReaderRead(reader, () =>
            {
                if (typeof(T) == typeof(List<object>))
                {
                    return (T)(object)reader.ConvertToListObjects();
                }

                if (typeof(T) == typeof(Dictionary<string, object>))
                {
                    return (T)(object)reader.ConvertToDictionaryObjects();
                }

                var values = new object[reader.FieldCount];

                if (typeof(T).IsValueTuple())
                {
                    return reader.ConvertToValueTuple<T>(values, dialectProvider);
                }

                var row = CreateInstance<T>();
                var indexCache = reader.GetIndexFieldsCache(ModelDefinition<T>.Definition, dialectProvider);
                row.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
                return row;
            }, token).ConfigAwait();
        }
    }

    /// <summary>
    /// Convert to list as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="onlyFields">The only fields.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
    public async static Task<List<T>> ConvertToListAsync<T>(this IDataReader reader, IOrmLiteDialectProvider dialectProvider, HashSet<string> onlyFields, CancellationToken token)
    {
        if (typeof(T) == typeof(List<object>))
        {
            using (reader)
            {
                return await dialectProvider.ReaderEach(reader, () =>
                {
                    var row = reader.ConvertToListObjects();
                    return (T)(object)row;
                }, token).ConfigAwait();
            }
        }
        if (typeof(T) == typeof(Dictionary<string, object>))
        {
            using (reader)
            {
                return await dialectProvider.ReaderEach(reader, () =>
                {
                    var row = reader.ConvertToDictionaryObjects();
                    return (T)(object)row;
                }, token).ConfigAwait();
            }
        }
        if (typeof(T) == typeof(object))
        {
            using (reader)
            {
                return await dialectProvider.ReaderEach(reader, () =>
                {
                    var row = reader.ConvertToExpandoObject();
                    return (T)(object)row;
                }, token).ConfigAwait();
            }
        }
        if (typeof(T).IsValueTuple())
        {
            using (reader)
            {
                var values = new object[reader.FieldCount];
                return await dialectProvider.ReaderEach(reader, () =>
                {
                    var row = reader.ConvertToValueTuple<T>(values, dialectProvider);
                    return row;
                }, token).ConfigAwait();
            }
        }
        if (typeof(T).IsTuple())
        {
            var genericArgs = typeof(T).GetGenericArguments();
            var modelIndexCaches = reader.GetMultiIndexCaches(dialectProvider, onlyFields, genericArgs);

            var values = new object[reader.FieldCount];
            var genericTupleMi = typeof(T).GetGenericTypeDefinition().GetCachedGenericType(genericArgs);
            var activator = genericTupleMi.GetConstructor(genericArgs).GetActivator();

            using (reader)
            {
                return await dialectProvider.ReaderEach(reader, () =>
                {
                    var tupleArgs = reader.ToMultiTuple(dialectProvider, modelIndexCaches, genericArgs, values);
                    var tuple = activator([.. tupleArgs]);
                    return (T)tuple;
                }, token).ConfigAwait();
            }
        }
        else
        {
            var indexCache = reader.GetIndexFieldsCache(ModelDefinition<T>.Definition, dialectProvider, onlyFields: onlyFields);
            var values = new object[reader.FieldCount];

            using (reader)
            {
                return await dialectProvider.ReaderEach(reader, () =>
                {
                    var row = CreateInstance<T>();
                    row.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
                    return row;
                }, token).ConfigAwait();
            }
        }
    }

    /// <summary>
    /// Convert to list as an asynchronous operation.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="type">The type.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IList&gt; representing the asynchronous operation.</returns>
    public async static Task<IList> ConvertToListAsync(this IDataReader reader, IOrmLiteDialectProvider dialectProvider, Type type, CancellationToken token)
    {
        var modelDef = type.GetModelDefinition();
        var indexCache = reader.GetIndexFieldsCache(modelDef, dialectProvider);
        var values = new object[reader.FieldCount];

        using (reader)
        {
            var ret = await dialectProvider.ReaderEach(reader, () =>
            {
                var row = type.CreateInstance();
                row.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
                return row;
            }, token).ConfigAwait();

            var to = (IList)typeof(List<>).GetCachedGenericType(type).CreateInstance();
            ret.Each(o => to.Add(o));
            return to;
        }
    }
}