// ***********************************************************************
// <copyright file="OrmLiteReadCommandExtensionsAsyncLegacy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if ASYNC
// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Legacy
{
    /// <summary>
    /// Class OrmLiteReadCommandExtensionsAsyncLegacy.
    /// </summary>
    [Obsolete(Messages.LegacyApi)]
    internal static class OrmLiteReadCommandExtensionsAsyncLegacy
    {
        /// <summary>
        /// Singles the FMT asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        internal static Task<T> SingleFmtAsync<T>(this IDbCommand dbCmd, CancellationToken token, string filter, params object[] filterParams)
        {
            return dbCmd.ConvertToAsync<T>(dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), filter, filterParams), token);
        }

        /// <summary>
        /// Selects the FMT asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        internal static Task<List<T>> SelectFmtAsync<T>(this IDbCommand dbCmd, CancellationToken token, string sqlFilter, params object[] filterParams)
        {
            return dbCmd.ConvertToListAsync<T>(
                dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), sqlFilter, filterParams), token);
        }

        /// <summary>
        /// Selects the FMT asynchronous.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>Task&lt;List&lt;TModel&gt;&gt;.</returns>
        internal static Task<List<TModel>> SelectFmtAsync<TModel>(this IDbCommand dbCmd, CancellationToken token, Type fromTableType, string sqlFilter, params object[] filterParams)
        {
            var sql = OrmLiteReadCommandExtensionsLegacy.ToSelectFmt<TModel>(dbCmd.GetDialectProvider(), fromTableType, sqlFilter, filterParams);
            return dbCmd.ConvertToListAsync<TModel>(sql, token);
        }

        /// <summary>
        /// Scalars the FMT asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        internal static Task<T> ScalarFmtAsync<T>(this IDbCommand dbCmd, CancellationToken token, string sql, params object[] sqlParams)
        {
            return dbCmd.ScalarAsync<T>(sql.SqlFmt(sqlParams), token);
        }

        /// <summary>
        /// Columns the FMT asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        internal static Task<List<T>> ColumnFmtAsync<T>(this IDbCommand dbCmd, CancellationToken token, string sql, params object[] sqlParams)
        {
            return dbCmd.ColumnAsync<T>(sql.SqlFmt(sqlParams), token);
        }

        /// <summary>
        /// Columns the distinct FMT asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>Task&lt;HashSet&lt;T&gt;&gt;.</returns>
        internal static Task<HashSet<T>> ColumnDistinctFmtAsync<T>(this IDbCommand dbCmd, CancellationToken token, string sql, params object[] sqlParams)
        {
            return dbCmd.ColumnDistinctAsync<T>(sql.SqlFmt(sqlParams), token);
        }

        /// <summary>
        /// Lookups the FMT asynchronous.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>Task&lt;Dictionary&lt;K, List&lt;V&gt;&gt;&gt;.</returns>
        internal static Task<Dictionary<K, List<V>>> LookupFmtAsync<K, V>(this IDbCommand dbCmd, CancellationToken token, string sql, params object[] sqlParams)
        {
            return dbCmd.LookupAsync<K, V>(sql.SqlFmt(sqlParams), token);
        }

        /// <summary>
        /// Dictionaries the FMT asynchronous.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="sqlFormat">The SQL format.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>Task&lt;Dictionary&lt;K, V&gt;&gt;.</returns>
        internal static Task<Dictionary<K, V>> DictionaryFmtAsync<K, V>(this IDbCommand dbCmd, CancellationToken token, string sqlFormat, params object[] sqlParams)
        {
            return dbCmd.DictionaryAsync<K, V>(sqlFormat.SqlFmt(sqlParams), token);
        }

        /// <summary>
        /// Existses the FMT asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        internal static Task<bool> ExistsFmtAsync<T>(this IDbCommand dbCmd, CancellationToken token, string sqlFilter, params object[] filterParams)
        {
            var fromTableType = typeof(T);
            return dbCmd.ScalarAsync(dbCmd.GetDialectProvider().ToSelectStatement(fromTableType, sqlFilter, filterParams), token)
                .Then(x => x != null);
        }

        /// <summary>
        /// Deletes the FMT asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal static Task<int> DeleteFmtAsync<T>(this IDbCommand dbCmd, CancellationToken token, string sqlFilter, params object[] filterParams)
        {
            return DeleteFmtAsync(dbCmd, token, typeof(T), sqlFilter, filterParams);
        }

        /// <summary>
        /// Deletes the FMT asynchronous.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal static Task<int> DeleteFmtAsync(this IDbCommand dbCmd, CancellationToken token, Type tableType, string sqlFilter, params object[] filterParams)
        {
            var dialectProvider = dbCmd.GetDialectProvider();
            return dbCmd.ExecuteSqlAsync(dialectProvider.ToDeleteStatement(tableType, sqlFilter, filterParams), token);
        }
    }
}

#endif