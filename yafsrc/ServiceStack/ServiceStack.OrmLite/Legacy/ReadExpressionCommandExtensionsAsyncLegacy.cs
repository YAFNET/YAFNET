// ***********************************************************************
// <copyright file="ReadExpressionCommandExtensionsAsyncLegacy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if ASYNC
// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Legacy
{
    /// <summary>
    /// Class ReadExpressionCommandExtensionsAsyncLegacy.
    /// </summary>
    [Obsolete(Messages.LegacyApi)]
    internal static class ReadExpressionCommandExtensionsAsyncLegacy
    {
        /// <summary>
        /// Selects the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        [Obsolete("Use db.SelectAsync(db.From<T>())")]
        internal static Task<List<T>> SelectAsync<T>(this IDbCommand dbCmd, Func<SqlExpression<T>, SqlExpression<T>> expression, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            var sql = expression(q).SelectInto<T>(QueryType.Select);
            return dbCmd.ExprConvertToListAsync<T>(sql, q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Selects the asynchronous.
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;Into&gt;&gt;.</returns>
        [Obsolete("Use db.SelectAsync(db.From<T>())")]
        internal static Task<List<Into>> SelectAsync<Into, From>(this IDbCommand dbCmd, Func<SqlExpression<From>, SqlExpression<From>> expression, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<From>();
            string sql = expression(q).SelectInto<Into>(QueryType.Select);
            return dbCmd.ExprConvertToListAsync<Into>(sql, q.Params, q.OnlyFields, token);
        }

        /// <summary>
        /// Singles the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        [Obsolete("Use db.SingleAsync(db.From<T>())")]
        internal static Task<T> SingleAsync<T>(this IDbCommand dbCmd, Func<SqlExpression<T>, SqlExpression<T>> expression, CancellationToken token)
        {
            var expr = dbCmd.GetDialectProvider().SqlExpression<T>();
            return dbCmd.SingleAsync(expression(expr), token);
        }

        /// <summary>
        /// Counts the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        [Obsolete("Use db.CountAsync(db.From<T>())")]
        internal static Task<long> CountAsync<T>(this IDbCommand dbCmd, Func<SqlExpression<T>, SqlExpression<T>> expression, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            var sql = expression(q).ToCountStatement();
            return dbCmd.GetCountAsync(sql, q.Params, token);
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
        [Obsolete("Use db.LoadSelectAsync(db.From<T>())")]
        internal static Task<List<T>> LoadSelectAsync<T>(this IDbCommand dbCmd, Func<SqlExpression<T>, SqlExpression<T>> expression, string[] include = null, CancellationToken token = default)
        {
            var expr = dbCmd.GetDialectProvider().SqlExpression<T>();
            expr = expression(expr);
            return dbCmd.LoadListWithReferences<T, T>(expr, include, token);
        }
    }
}

#endif