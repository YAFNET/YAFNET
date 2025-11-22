// ***********************************************************************
// <copyright file="OrmLiteResultsFilterExtensionsAsync.cs" company="ServiceStack, Inc.">
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
using ServiceStack.Logging;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteResultsFilterExtensionsAsync.
/// </summary>
public static class OrmLiteResultsFilterExtensionsAsync
{
    /// <summary>
    /// The log
    /// </summary>
    static internal ILog Log => OrmLiteLog.Log;

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Executes the non query asynchronous.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> ExecNonQueryAsync(string sql, object anonType, CancellationToken token = default)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters(anonType.ToObjectDictionary(), (bool)false, sql: ref sql);
            }

            dbCmd.CommandText = sql;

            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.ExecuteSql(dbCmd).InTask();
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            return dbCmd.WithLog(dbCmd.GetDialectProvider().ExecuteNonQueryAsync(dbCmd, token));
        }

        /// <summary>
        /// Executes the non query asynchronous.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> ExecNonQueryAsync(string sql, Dictionary<string, object> dict, CancellationToken token = default)
        {
            if (dict != null)
            {
                dbCmd.SetParameters(dict, (bool)false, sql: ref sql);
            }

            dbCmd.CommandText = sql;

            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.ExecuteSql(dbCmd).InTask();
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            return dbCmd.WithLog(dbCmd.GetDialectProvider().ExecuteNonQueryAsync(dbCmd, token));
        }

        /// <summary>
        /// Executes the non query asynchronous.
        /// </summary>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> ExecNonQueryAsync(CancellationToken token = default)
        {
            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.ExecuteSql(dbCmd).InTask();
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            return dbCmd.WithLog(dbCmd.GetDialectProvider().ExecuteNonQueryAsync(dbCmd, token));
        }

        /// <summary>
        /// Converts to list asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> ConvertToListAsync<T>()
        {
            return dbCmd.ConvertToListAsync<T>(null, CancellationToken.None);
        }

        /// <summary>
        /// Convert to list as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
        public async Task<List<T>> ConvertToListAsync<T>(string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetList<T>(dbCmd);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            if (OrmLiteUtils.IsScalar<T>())
            {
                return await reader.ColumnAsync<T>(dialectProvider, token).ConfigAwait();
            }

            return await reader.ConvertToListAsync<T>(dialectProvider, null, token).ConfigAwait();
        }

        /// <summary>
        /// Converts to list asynchronous.
        /// </summary>
        /// <param name="refType">Type of the reference.</param>
        /// <returns>Task&lt;IList&gt;.</returns>
        public Task<IList> ConvertToListAsync(Type refType)
        {
            return dbCmd.ConvertToListAsync(refType, null, CancellationToken.None);
        }

        /// <summary>
        /// Convert to list as an asynchronous operation.
        /// </summary>
        /// <param name="refType">Type of the reference.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;IList&gt; representing the asynchronous operation.</returns>
        public async Task<IList> ConvertToListAsync(Type refType, string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetRefList(dbCmd, refType);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return await reader.ConvertToListAsync(dialectProvider, refType, token).ConfigAwait();
        }

        /// <summary>
        /// Expr convert to list as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
        async internal Task<List<T>> ExprConvertToListAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, HashSet<string> onlyFields, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            dbCmd.SetParameters(sqlParams);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetList<T>(dbCmd);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return await reader.ConvertToListAsync<T>(dialectProvider, onlyFields, token).ConfigAwait();
        }

        /// <summary>
        /// Converts to asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> ConvertToAsync<T>()
        {
            return dbCmd.ConvertToAsync<T>(null, CancellationToken.None);
        }

        /// <summary>
        /// Convert to as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;T&gt; representing the asynchronous operation.</returns>
        public async Task<T> ConvertToAsync<T>(string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetSingle<T>(dbCmd);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return await reader.ConvertToAsync<T>(dialectProvider, token).ConfigAwait();
        }

        /// <summary>
        /// Convert to as an asynchronous operation.
        /// </summary>
        /// <param name="refType">Type of the reference.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;System.Object&gt; representing the asynchronous operation.</returns>
        async internal Task<object> ConvertToAsync(Type refType, string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetRefSingle(dbCmd, refType);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return await reader.ConvertToAsync(dialectProvider, refType, token).ConfigAwait();
        }

        /// <summary>
        /// Scalars the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> ScalarAsync<T>()
        {
            return dbCmd.ScalarAsync<T>(null, CancellationToken.None);
        }

        /// <summary>
        /// Scalars the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> ScalarAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token)
        {
            return dbCmd.SetParameters(sqlParams).ScalarAsync<T>(sql, token);
        }

        /// <summary>
        /// Scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;T&gt; representing the asynchronous operation.</returns>
        public async Task<T> ScalarAsync<T>(string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetScalar<T>(dbCmd);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return await reader.ScalarAsync<T>(dialectProvider, token).ConfigAwait();
        }

        /// <summary>
        /// Scalars the asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> ScalarAsync()
        {
            return dbCmd.ScalarAsync((string)null, CancellationToken.None);
        }

        /// <summary>
        /// Scalars the asynchronous.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> ScalarAsync(ISqlExpression expression, CancellationToken token)
        {
            dbCmd.PopulateWith(expression, QueryType.Scalar);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetScalar(dbCmd).InTask();
            }

            return dbCmd.GetDialectProvider().ExecuteScalarAsync(dbCmd, token);
        }

        /// <summary>
        /// Scalars the asynchronous.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> ScalarAsync(string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetScalar(dbCmd).InTask();
            }

            return dbCmd.GetDialectProvider().ExecuteScalarAsync(dbCmd, token);
        }

        /// <summary>
        /// Executes the long scalar asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        public Task<long> ExecLongScalarAsync()
        {
            return dbCmd.ExecLongScalarAsync(null, CancellationToken.None);
        }

        /// <summary>
        /// Executes the long scalar asynchronous.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        public Task<long> ExecLongScalarAsync(string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetLongScalar(dbCmd).InTask();
            }

            return dbCmd.WithLog(dbCmd.LongScalarAsync(token));
        }

        /// <summary>
        /// Expr convert to as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;T&gt; representing the asynchronous operation.</returns>
        async internal Task<T> ExprConvertToAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            dbCmd.SetParameters(sqlParams);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetSingle<T>(dbCmd);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return await reader.ConvertToAsync<T>(dialectProvider, token).ConfigAwait();
        }

        /// <summary>
        /// Columns the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        internal Task<List<T>> ColumnAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token)
        {
            return dbCmd.SetParameters(sqlParams).ColumnAsync<T>(sql, token);
        }

        /// <summary>
        /// Column as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
        async internal Task<List<T>> ColumnAsync<T>(string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetColumn<T>(dbCmd);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return await reader.ColumnAsync<T>(dialectProvider, token).ConfigAwait();
        }

        /// <summary>
        /// Columns the distinct asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;HashSet&lt;T&gt;&gt;.</returns>
        internal Task<HashSet<T>> ColumnDistinctAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token)
        {
            return dbCmd.SetParameters(sqlParams).ColumnDistinctAsync<T>(sql, token);
        }

        /// <summary>
        /// Column distinct as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;HashSet`1&gt; representing the asynchronous operation.</returns>
        async internal Task<HashSet<T>> ColumnDistinctAsync<T>(string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetColumnDistinct<T>(dbCmd);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return await reader.ColumnDistinctAsync<T>(dialectProvider, token).ConfigAwait();
        }

        /// <summary>
        /// Dictionaries the asynchronous.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Dictionary&lt;K, V&gt;&gt;.</returns>
        internal Task<Dictionary<K, V>> DictionaryAsync<K, V>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token)
        {
            return dbCmd.SetParameters(sqlParams).DictionaryAsync<K, V>(sql, token);
        }

        /// <summary>
        /// Dictionary as an asynchronous operation.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;Dictionary`2&gt; representing the asynchronous operation.</returns>
        async internal Task<Dictionary<K, V>> DictionaryAsync<K, V>(string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetDictionary<K, V>(dbCmd);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return await reader.DictionaryAsync<K, V>(dialectProvider, token).ConfigAwait();
        }

        /// <summary>
        /// Keys the value pairs asynchronous.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;KeyValuePair&lt;K, V&gt;&gt;&gt;.</returns>
        internal Task<List<KeyValuePair<K, V>>> KeyValuePairsAsync<K, V>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token)
        {
            return dbCmd.SetParameters(sqlParams).KeyValuePairsAsync<K, V>(sql, token);
        }

        /// <summary>
        /// Key value pairs as an asynchronous operation.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
        async internal Task<List<KeyValuePair<K, V>>> KeyValuePairsAsync<K, V>(string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetKeyValuePairs<K, V>(dbCmd);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return await reader.KeyValuePairsAsync<K, V>(dialectProvider, token).ConfigAwait();
        }

        /// <summary>
        /// Lookups the asynchronous.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Dictionary&lt;K, List&lt;V&gt;&gt;&gt;.</returns>
        internal Task<Dictionary<K, List<V>>> LookupAsync<K, V>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token)
        {
            return dbCmd.SetParameters(sqlParams).LookupAsync<K, V>(sql, token);
        }

        /// <summary>
        /// Lookup as an asynchronous operation.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;Dictionary`2&gt; representing the asynchronous operation.</returns>
        async internal Task<Dictionary<K, List<V>>> LookupAsync<K, V>(string sql, CancellationToken token)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetLookup<K, V>(dbCmd);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return await reader.LookupAsync<K, V>(dialectProvider, token).ConfigAwait();
        }
    }
}