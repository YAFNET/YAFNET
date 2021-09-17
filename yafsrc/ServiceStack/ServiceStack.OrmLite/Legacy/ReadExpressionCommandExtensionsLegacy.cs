// ***********************************************************************
// <copyright file="ReadExpressionCommandExtensionsLegacy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;

namespace ServiceStack.OrmLite.Legacy
{
    /// <summary>
    /// Class ReadExpressionCommandExtensionsLegacy.
    /// </summary>
    [Obsolete(Messages.LegacyApi)]
    internal static class ReadExpressionCommandExtensionsLegacy
    {
        /// <summary>
        /// Selects the specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;T&gt;.</returns>
        [Obsolete("Use db.Select(db.From<T>())")]
        internal static List<T> Select<T>(this IDbCommand dbCmd, Func<SqlExpression<T>, SqlExpression<T>> expression)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            string sql = expression(q).SelectInto<T>(QueryType.Select);

            return dbCmd.ExprConvertToList<T>(sql, q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the specified expression.
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;Into&gt;.</returns>
        [Obsolete("Use db.Select<Into,From>(db.From<From>())")]
        internal static List<Into> Select<Into, From>(this IDbCommand dbCmd, Func<SqlExpression<From>, SqlExpression<From>> expression)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<From>();
            string sql = expression(q).SelectInto<Into>(QueryType.Select);

            return dbCmd.ExprConvertToList<Into>(sql, q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the specified q.
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="q">The q.</param>
        /// <returns>List&lt;Into&gt;.</returns>
        internal static List<Into> Select<Into, From>(this IDbCommand dbCmd, SqlExpression<From> q)
        {
            string sql = q.SelectInto<Into>(QueryType.Select);
            return dbCmd.ExprConvertToList<Into>(sql, q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Singles the specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>T.</returns>
        [Obsolete("Use db.Single(db.From<T>())")]
        internal static T Single<T>(this IDbCommand dbCmd, Func<SqlExpression<T>, SqlExpression<T>> expression)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            return dbCmd.Single(expression(q));
        }

        /// <summary>
        /// Counts the specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Int64.</returns>
        [Obsolete("Use db.Count(db.From<T>())")]
        internal static long Count<T>(this IDbCommand dbCmd, Func<SqlExpression<T>, SqlExpression<T>> expression)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            var sql = expression(q).ToCountStatement();
            return dbCmd.GetCount(sql, q.Params);
        }

        /// <summary>
        /// Loads the select.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;T&gt;.</returns>
        [Obsolete("Use db.LoadSelect(db.From<T>())")]
        internal static List<T> LoadSelect<T>(this IDbCommand dbCmd, Func<SqlExpression<T>, SqlExpression<T>> expression, IEnumerable<string> include = null)
        {
            var expr = dbCmd.GetDialectProvider().SqlExpression<T>();
            expr = expression(expr);
            return dbCmd.LoadListWithReferences<T, T>(expr, include);
        }
    }
}