﻿// ***********************************************************************
// <copyright file="ReadExpressionCommandExtensionsAsync.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite
{
    /// <summary>
    /// Class ReadExpressionCommandExtensionsAsync.
    /// </summary>
    static internal class ReadExpressionCommandExtensionsAsync
    {
        /// <summary>
        /// Selects the asynchronous.
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Into&gt;&gt;.</returns>
        static internal Task<List<Into>> SelectAsync<Into, From>(this IDbCommand dbCmd, SqlExpression<From> q, CancellationToken token)
        {
            var sql = q.SelectInto<Into>(QueryType.Select);
            return dbCmd.ExprConvertToListAsync<Into>(sql, q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        static internal Task<List<T>> SelectAsync<T>(this IDbCommand dbCmd, SqlExpression<T> q, CancellationToken token)
        {
            var sql = q.SelectInto<T>(QueryType.Select);
            return dbCmd.ExprConvertToListAsync<T>(sql, q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        static internal Task<List<T>> SelectAsync<T>(this IDbCommand dbCmd, Expression<Func<T, bool>> predicate, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            var sql = q.Where(predicate).SelectInto<T>(QueryType.Select);

            return dbCmd.ExprConvertToListAsync<T>(sql, q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2>>> SelectMultiAsync<T, T2>(this IDbCommand dbCmd, SqlExpression<T> q, CancellationToken token)
        {
            q.Select(q.CreateMultiSelect<T, T2, EOT, EOT, EOT, EOT, EOT, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2>>(q.ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2, T3&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2, T3>>> SelectMultiAsync<T, T2, T3>(this IDbCommand dbCmd, SqlExpression<T> q, CancellationToken token)
        {
            q.Select(q.CreateMultiSelect<T, T2, T3, EOT, EOT, EOT, EOT, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3>>(q.ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2, T3, T4&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2, T3, T4>>> SelectMultiAsync<T, T2, T3, T4>(this IDbCommand dbCmd, SqlExpression<T> q, CancellationToken token)
        {
            q.Select(q.CreateMultiSelect<T, T2, T3, T4, EOT, EOT, EOT, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3, T4>>(q.ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2, T3, T4, T5&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2, T3, T4, T5>>> SelectMultiAsync<T, T2, T3, T4, T5>(this IDbCommand dbCmd, SqlExpression<T> q, CancellationToken token)
        {
            q.Select(q.CreateMultiSelect<T, T2, T3, T4, T5, EOT, EOT, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3, T4, T5>>(q.ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2, T3, T4, T5, T6&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2, T3, T4, T5, T6>>> SelectMultiAsync<T, T2, T3, T4, T5, T6>(this IDbCommand dbCmd, SqlExpression<T> q, CancellationToken token)
        {
            q.Select(q.CreateMultiSelect<T, T2, T3, T4, T5, T6, EOT, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3, T4, T5, T6>>(q.ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2, T3, T4, T5, T6, T7&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2, T3, T4, T5, T6, T7>>> SelectMultiAsync<T, T2, T3, T4, T5, T6, T7>(this IDbCommand dbCmd, SqlExpression<T> q, CancellationToken token)
        {
            q.Select(q.CreateMultiSelect<T, T2, T3, T4, T5, T6, T7, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3, T4, T5, T6, T7>>(q.ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        static internal Task<List<Tuple<T, T2, T3, T4, T5, T6, T7, T8>>> SelectMultiAsync<T, T2, T3, T4, T5, T6, T7, T8>(this IDbCommand dbCmd, SqlExpression<T> q, CancellationToken token)
        {
            q.Select(q.CreateMultiSelect<T, T2, T3, T4, T5, T6, T7, T8>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3, T4, T5, T6, T7, T8>>(q.ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2>>> SelectMultiAsync<T, T2>(this IDbCommand dbCmd, SqlExpression<T> q, string[] tableSelects, CancellationToken token)
        {
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2, T3&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2, T3>>> SelectMultiAsync<T, T2, T3>(this IDbCommand dbCmd, SqlExpression<T> q, string[] tableSelects, CancellationToken token)
        {
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2, T3, T4&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2, T3, T4>>> SelectMultiAsync<T, T2, T3, T4>(this IDbCommand dbCmd, SqlExpression<T> q, string[] tableSelects, CancellationToken token)
        {
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3, T4>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2, T3, T4, T5&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2, T3, T4, T5>>> SelectMultiAsync<T, T2, T3, T4, T5>(this IDbCommand dbCmd, SqlExpression<T> q, string[] tableSelects, CancellationToken token)
        {
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3, T4, T5>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2, T3, T4, T5, T6&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2, T3, T4, T5, T6>>> SelectMultiAsync<T, T2, T3, T4, T5, T6>(this IDbCommand dbCmd, SqlExpression<T> q, string[] tableSelects, CancellationToken token)
        {
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3, T4, T5, T6>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the multi asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Tuple&lt;T, T2, T3, T4, T5, T6, T7&gt;&gt;&gt;.</returns>
        static internal Task<List<Tuple<T, T2, T3, T4, T5, T6, T7>>> SelectMultiAsync<T, T2, T3, T4, T5, T6, T7>(this IDbCommand dbCmd, SqlExpression<T> q, string[] tableSelects, CancellationToken token)
        {
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3, T4, T5, T6, T7>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        static internal Task<List<Tuple<T, T2, T3, T4, T5, T6, T7, T8>>> SelectMultiAsync<T, T2, T3, T4, T5, T6, T7, T8>(this IDbCommand dbCmd, SqlExpression<T> q, string[] tableSelects, CancellationToken token)
        {
            return dbCmd.ExprConvertToListAsync<Tuple<T, T2, T3, T4, T5, T6, T7, T8>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(QueryType.Select), q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Singles the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        static internal Task<T> SingleAsync<T>(this IDbCommand dbCmd, Expression<Func<T, bool>> predicate, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();

            return SingleAsync(dbCmd, q.Where(predicate), token);
        }

        /// <summary>
        /// Singles the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        static internal Task<T> SingleAsync<T>(this IDbCommand dbCmd, SqlExpression<T> expression, CancellationToken token)
        {
            var sql = expression.Limit(1).SelectInto<T>(QueryType.Single);

            return dbCmd.ExprConvertToAsync<T>(sql, expression.Params, token);
        }

        /// <summary>
        /// Scalars the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="field">The field.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;TKey&gt;.</returns>
        public static Task<TKey> ScalarAsync<T, TKey>(this IDbCommand dbCmd, Expression<Func<T, object>> field, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Select(field);
            var sql = q.SelectInto<T>(QueryType.Select);
            return dbCmd.ScalarAsync<TKey>(sql, q.Params, token);
        }

        /// <summary>
        /// Scalars the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="field">The field.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;TKey&gt;.</returns>
        static internal Task<TKey> ScalarAsync<T, TKey>(this IDbCommand dbCmd,
            Expression<Func<T, object>> field, Expression<Func<T, bool>> predicate, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Select(field).Where(predicate);
            var sql = q.SelectInto<T>(QueryType.Select);
            return dbCmd.ScalarAsync<TKey>(sql, q.Params, token);
        }

        /// <summary>
        /// Counts the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        static internal Task<long> CountAsync<T>(this IDbCommand dbCmd, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            var sql = q.ToCountStatement();
            return GetCountAsync(dbCmd, sql, q.Params, token);
        }

        /// <summary>
        /// Counts the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        static internal Task<long> CountAsync<T>(this IDbCommand dbCmd, SqlExpression<T> q, CancellationToken token)
        {
            var sql = q.ToCountStatement();
            return GetCountAsync(dbCmd, sql, q.Params, token);
        }

        /// <summary>
        /// Counts the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        static internal Task<long> CountAsync<T>(this IDbCommand dbCmd, Expression<Func<T, bool>> predicate, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Where(predicate);
            var sql = q.ToCountStatement();
            return GetCountAsync(dbCmd, sql, q.Params, token);
        }

        /// <summary>
        /// Get count as an asynchronous operation.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;System.Int64&gt; representing the asynchronous operation.</returns>
        async static internal Task<long> GetCountAsync(this IDbCommand dbCmd, string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token)
        {
            var ret = await dbCmd.ColumnAsync<long>(sql, sqlParams, token).ConfigAwait();
            return ret.Sum();
        }

        /// <summary>
        /// Rows the count asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        static internal Task<long> RowCountAsync<T>(this IDbCommand dbCmd, SqlExpression<T> expression, CancellationToken token)
        {
            var countExpr = expression.Clone().OrderBy();
            return dbCmd.ScalarAsync<long>(dbCmd.GetDialectProvider().ToRowCountStatement(countExpr.ToSelectStatement(QueryType.Select)), countExpr.Params, token);
        }

        /// <summary>
        /// Rows the count asynchronous.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        static internal Task<long> RowCountAsync(this IDbCommand dbCmd, string sql, object anonType, CancellationToken token)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters(anonType.ToObjectDictionary(), excludeDefaults: false, sql: ref sql);
            }

            return dbCmd.ScalarAsync<long>(dbCmd.GetDialectProvider().ToRowCountStatement(sql), token);
        }

        /// <summary>
        /// Loads the select asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        static internal Task<List<T>> LoadSelectAsync<T>(this IDbCommand dbCmd, SqlExpression<T> expression = null, IEnumerable<string> include = null, CancellationToken token = default)
        {
            return dbCmd.LoadListWithReferences<T, T>(expression, include, token);
        }

        /// <summary>
        /// Loads the select asynchronous.
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Into&gt;&gt;.</returns>
        static internal Task<List<Into>> LoadSelectAsync<Into, From>(this IDbCommand dbCmd, SqlExpression<From> expression, IEnumerable<string> include = null, CancellationToken token = default)
        {
            return dbCmd.LoadListWithReferences<Into, From>(expression, include, token);
        }

        /// <summary>
        /// Loads the select asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="include">The include.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        static internal Task<List<T>> LoadSelectAsync<T>(this IDbCommand dbCmd, Expression<Func<T, bool>> predicate, IEnumerable<string> include = null, CancellationToken token = default)
        {
            var expr = dbCmd.GetDialectProvider().SqlExpression<T>().Where(predicate);
            return dbCmd.LoadListWithReferences<T, T>(expr, include, token);
        }

        /// <summary>
        /// Exprs the convert to asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="dialectProvider">The dialect provider.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        static internal Task<T> ExprConvertToAsync<T>(this IDataReader dataReader, IOrmLiteDialectProvider dialectProvider, CancellationToken token)
        {
            return dialectProvider.ReaderRead(dataReader,
                () => dataReader.ConvertTo<T>(dialectProvider), token);
        }

        /// <summary>
        /// Selects the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        static internal Task<List<T>> Select<T>(this IDbCommand dbCmd, Expression<Func<T, bool>> predicate, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            var sql = q.Where(predicate).SelectInto<T>(QueryType.Select);

            return dbCmd.ExprConvertToListAsync<T>(sql, q.Params, q.OnlyFields, token);
        }


        /// <summary>
        /// Get schema table as an asynchronous operation.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;DataTable&gt; representing the asynchronous operation.</returns>
        async static internal Task<DataTable> GetSchemaTableAsync(this IDbCommand dbCmd, string sql, CancellationToken token)
        {
            using var reader = await dbCmd.ExecReaderAsync(sql, token).ConfigAwait();
            return reader.GetSchemaTable();
        }

        /// <summary>
        /// Gets the table columns asynchronous.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="table">The table.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;ColumnSchema[]&gt;.</returns>
        public static Task<ColumnSchema[]> GetTableColumnsAsync(this IDbCommand dbCmd, Type table, CancellationToken token) =>
            dbCmd.GetTableColumnsAsync($"SELECT * FROM {dbCmd.GetDialectProvider().GetQuotedTableName(table.GetModelDefinition())}", token);

        /// <summary>
        /// Get table columns as an asynchronous operation.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;ColumnSchema[]&gt; representing the asynchronous operation.</returns>
        public async static Task<ColumnSchema[]> GetTableColumnsAsync(this IDbCommand dbCmd, string sql, CancellationToken token) =>
            (await dbCmd.GetSchemaTableAsync(sql, token).ConfigAwait()).ToColumnSchemas(dbCmd);
    }
}