// ***********************************************************************
// <copyright file="ReadExpressionCommandExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Class ReadExpressionCommandExtensions.
/// </summary>
static internal class ReadExpressionCommandExtensions
{
    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Selects the specified q.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q">The q.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Select<T>(SqlExpression<T> q)
        {
            var sql = q.SelectInto<T>(QueryType.Select);
            return dbCmd.ExprConvertToList<T>(sql, q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Select<T>(Expression<Func<T, bool>> predicate)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            var sql = q.Where(predicate).SelectInto<T>(QueryType.Select);

            return dbCmd.ExprConvertToList<T>(sql, q.Params);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="q">The q.</param>
        /// <returns>List&lt;Tuple&lt;T, T2&gt;&gt;.</returns>
        internal List<Tuple<T, T2>> SelectMulti<T, T2>(SqlExpression<T> q)
        {
            q.SelectIfDistinct(q.CreateMultiSelect<T, T2, EOT, EOT, EOT, EOT, EOT, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToList<Tuple<T, T2>>(q.ToSelectStatement(QueryType.Select), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <param name="q">The q.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3>> SelectMulti<T, T2, T3>(SqlExpression<T> q)
        {
            q.SelectIfDistinct(q.CreateMultiSelect<T, T2, T3, EOT, EOT, EOT, EOT, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3>>(q.ToSelectStatement(QueryType.Select), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <param name="q">The q.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3, T4>> SelectMulti<T, T2, T3, T4>(SqlExpression<T> q)
        {
            q.SelectIfDistinct(q.CreateMultiSelect<T, T2, T3, T4, EOT, EOT, EOT, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3, T4>>(q.ToSelectStatement(QueryType.Select), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <param name="q">The q.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3, T4, T5>> SelectMulti<T, T2, T3, T4, T5>(SqlExpression<T> q)
        {
            q.SelectIfDistinct(q.CreateMultiSelect<T, T2, T3, T4, T5, EOT, EOT, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3, T4, T5>>(q.ToSelectStatement(QueryType.Select), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <param name="q">The q.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3, T4, T5, T6>> SelectMulti<T, T2, T3, T4, T5, T6>(SqlExpression<T> q)
        {
            q.SelectIfDistinct(q.CreateMultiSelect<T, T2, T3, T4, T5, T6, EOT, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3, T4, T5, T6>>(q.ToSelectStatement(QueryType.Select), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <param name="q">The q.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6, T7&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3, T4, T5, T6, T7>> SelectMulti<T, T2, T3, T4, T5, T6, T7>(SqlExpression<T> q)
        {
            q.SelectIfDistinct(q.CreateMultiSelect<T, T2, T3, T4, T5, T6, T7, EOT>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3, T4, T5, T6, T7>>(q.ToSelectStatement(QueryType.Select), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <typeparam name="T8">The type of the t8.</typeparam>
        /// <param name="q">The q.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6, T7, T8&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3, T4, T5, T6, T7, T8>> SelectMulti<T, T2, T3, T4, T5, T6, T7, T8>(SqlExpression<T> q)
        {
            q.SelectIfDistinct(q.CreateMultiSelect<T, T2, T3, T4, T5, T6, T7, T8>(dbCmd.GetDialectProvider()));
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3, T4, T5, T6, T7, T8>>(q.ToSelectStatement(QueryType.Select), q.Params, onlyFields: q.OnlyFields);
        }
    }

    /// <summary>
    /// Creates the multi select.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="T3">The type of the t3.</typeparam>
    /// <typeparam name="T4">The type of the t4.</typeparam>
    /// <typeparam name="T5">The type of the t5.</typeparam>
    /// <typeparam name="T6">The type of the t6.</typeparam>
    /// <typeparam name="T7">The type of the t7.</typeparam>
    /// <typeparam name="T8">The type of the t8.</typeparam>
    /// <param name="q">The q.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>System.String.</returns>
    static internal string CreateMultiSelect<T, T2, T3, T4, T5, T6, T7, T8>(this SqlExpression<T> q, IOrmLiteDialectProvider dialectProvider)
    {
        var sb = StringBuilderCache.Allocate()
            .Append($"{dialectProvider.GetQuotedTableName(typeof(T).GetModelDefinition())}.*, {Sql.EOT}");

        if (typeof(T2) != typeof(EOT))
        {
            sb.Append($", {dialectProvider.GetQuotedTableName(typeof(T2).GetModelDefinition())}.*, {Sql.EOT}");
        }

        if (typeof(T3) != typeof(EOT))
        {
            sb.Append($", {dialectProvider.GetQuotedTableName(typeof(T3).GetModelDefinition())}.*, {Sql.EOT}");
        }

        if (typeof(T4) != typeof(EOT))
        {
            sb.Append($", {dialectProvider.GetQuotedTableName(typeof(T4).GetModelDefinition())}.*, {Sql.EOT}");
        }

        if (typeof(T5) != typeof(EOT))
        {
            sb.Append($", {dialectProvider.GetQuotedTableName(typeof(T5).GetModelDefinition())}.*, {Sql.EOT}");
        }

        if (typeof(T6) != typeof(EOT))
        {
            sb.Append($", {dialectProvider.GetQuotedTableName(typeof(T6).GetModelDefinition())}.*, {Sql.EOT}");
        }

        if (typeof(T7) != typeof(EOT))
        {
            sb.Append($", {dialectProvider.GetQuotedTableName(typeof(T7).GetModelDefinition())}.*, {Sql.EOT}");
        }

        if (typeof(T8) != typeof(EOT))
        {
            sb.Append($", {dialectProvider.GetQuotedTableName(typeof(T8).GetModelDefinition())}.*, {Sql.EOT}");
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Creates the multi select.
    /// </summary>
    /// <param name="q">The q.</param>
    /// <param name="tableSelects">The table selects.</param>
    /// <returns>System.String.</returns>
    static internal string CreateMultiSelect(this ISqlExpression q, string[] tableSelects)
    {
        var sb = StringBuilderCache.Allocate();

        foreach (var tableSelect in tableSelects)
        {
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }

            sb.Append($"{tableSelect}, {Sql.EOT}");
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2&gt;&gt;.</returns>
        internal List<Tuple<T, T2>> SelectMulti<T, T2>(SqlExpression<T> q, string[] tableSelects)
        {
            return dbCmd.ExprConvertToList<Tuple<T, T2>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3>> SelectMulti<T, T2, T3>(SqlExpression<T> q, string[] tableSelects)
        {
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3, T4>> SelectMulti<T, T2, T3, T4>(SqlExpression<T> q, string[] tableSelects)
        {
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3, T4>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3, T4, T5>> SelectMulti<T, T2, T3, T4, T5>(SqlExpression<T> q, string[] tableSelects)
        {
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3, T4, T5>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3, T4, T5, T6>> SelectMulti<T, T2, T3, T4, T5, T6>(SqlExpression<T> q, string[] tableSelects)
        {
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3, T4, T5, T6>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6, T7&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3, T4, T5, T6, T7>> SelectMulti<T, T2, T3, T4, T5, T6, T7>(SqlExpression<T> q, string[] tableSelects)
        {
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3, T4, T5, T6, T7>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <typeparam name="T8">The type of the t8.</typeparam>
        /// <param name="q">The q.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6, T7, T8&gt;&gt;.</returns>
        internal List<Tuple<T, T2, T3, T4, T5, T6, T7, T8>> SelectMulti<T, T2, T3, T4, T5, T6, T7, T8>(SqlExpression<T> q, string[] tableSelects)
        {
            return dbCmd.ExprConvertToList<Tuple<T, T2, T3, T4, T5, T6, T7, T8>>(q.Select(q.CreateMultiSelect(tableSelects)).ToSelectStatement(QueryType.Select), q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Singles the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns>T.</returns>
        internal T Single<T>(Expression<Func<T, bool>> predicate)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();

            return Single(dbCmd, q.Where(predicate));
        }

        /// <summary>
        /// Singles the specified q.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q">The q.</param>
        /// <returns>T.</returns>
        internal T Single<T>(SqlExpression<T> q)
        {
            var sql = q.SelectInto<T>(QueryType.Single);

            return dbCmd.ExprConvertTo<T>(sql, q.Params, onlyFields: q.OnlyFields);
        }

        /// <summary>
        /// Scalars the specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>TKey.</returns>
        public TKey Scalar<T, TKey>(SqlExpression<T> expression)
        {
            var sql = expression.SelectInto<T>(QueryType.Select);
            return dbCmd.Scalar<TKey>(sql, expression.Params);
        }

        /// <summary>
        /// Scalars the specified field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="field">The field.</param>
        /// <returns>TKey.</returns>
        public TKey Scalar<T, TKey>(Expression<Func<T, object>> field)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Select(field);
            var sql = q.SelectInto<T>(QueryType.Select);
            return dbCmd.Scalar<TKey>(sql, q.Params);
        }

        /// <summary>
        /// Scalars the specified field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>TKey.</returns>
        internal TKey Scalar<T, TKey>(Expression<Func<T, object>> field, Expression<Func<T, bool>> predicate)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Select(field).Where(predicate);
            var sql = q.SelectInto<T>(QueryType.Select);
            return dbCmd.Scalar<TKey>(sql, q.Params);
        }

        /// <summary>
        /// Counts the specified database command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.Int64.</returns>
        internal long Count<T>()
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            var sql = q.ToCountStatement();
            return GetCount(dbCmd, sql, q.Params);
        }

        /// <summary>
        /// Counts the specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Int64.</returns>
        internal long Count<T>(SqlExpression<T> expression)
        {
            var sql = expression.ToCountStatement();
            return GetCount(dbCmd, sql, expression.Params);
        }

        /// <summary>
        /// Counts the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns>System.Int64.</returns>
        internal long Count<T>(Expression<Func<T, bool>> predicate)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Where(predicate);
            var sql = q.ToCountStatement();
            return GetCount(dbCmd, sql, q.Params);
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>System.Int64.</returns>
        internal long GetCount(string sql)
        {
            return dbCmd.Column<long>(sql).Sum();
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>System.Int64.</returns>
        internal long GetCount(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbCmd.Column<long>(sql, sqlParams).Sum();
        }

        /// <summary>
        /// Rows the count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Int64.</returns>
        internal long RowCount<T>(SqlExpression<T> expression)
        {
            //ORDER BY throws when used in sub selects in SQL Server. Removing OrderBy() clause since it doesn't impact results
            var countExpr = expression.Clone().OrderBy();
            return dbCmd.Scalar<long>(dbCmd.GetDialectProvider().ToRowCountStatement(countExpr.ToSelectStatement(QueryType.Scalar)), countExpr.Params);
        }

        /// <summary>
        /// Rows the count.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>System.Int64.</returns>
        internal long RowCount(string sql, object anonType)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters(anonType.ToObjectDictionary(), excludeDefaults: false, sql: ref sql);
            }

            return dbCmd.Scalar<long>(dbCmd.GetDialectProvider().ToRowCountStatement(sql));
        }

        /// <summary>
        /// Rows the count.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>System.Int64.</returns>
        internal long RowCount(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbCmd.SetParameters(sqlParams).Scalar<long>(dbCmd.GetDialectProvider().ToRowCountStatement(sql));
        }

        /// <summary>
        /// Loads the select.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> LoadSelect<T>(SqlExpression<T> expression = null, IEnumerable<string> include = null)
        {
            return dbCmd.LoadListWithReferences<T, T>(expression, include);
        }

        /// <summary>
        /// Loads the select.
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;Into&gt;.</returns>
        internal List<Into> LoadSelect<Into, From>(SqlExpression<From> expression, IEnumerable<string> include = null)
        {
            return dbCmd.LoadListWithReferences<Into, From>(expression, include);
        }

        /// <summary>
        /// Loads the select.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> LoadSelect<T>(Expression<Func<T, bool>> predicate, IEnumerable<string> include = null)
        {
            var expr = dbCmd.GetDialectProvider().SqlExpression<T>().Where(predicate);
            return dbCmd.LoadListWithReferences<T, T>(expr, include);
        }

        /// <summary>
        /// Gets the schema table.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>DataTable.</returns>
        internal DataTable GetSchemaTable(string sql)
        {
            using var reader = dbCmd.ExecReader(sql, CommandBehavior.KeyInfo);
            return reader.GetSchemaTable();
        }

        /// <summary>
        /// Gets the table columns.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>ColumnSchema[].</returns>
        public ColumnSchema[] GetTableColumns(Type table)
        {
            return dbCmd.GetTableColumns(
                $"SELECT * FROM {dbCmd.GetDialectProvider().GetQuotedTableName(table.GetModelDefinition())}");
        }

        /// <summary>
        /// Gets the table columns.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>ColumnSchema[].</returns>
        public ColumnSchema[] GetTableColumns(string sql)
        {
            return dbCmd.GetSchemaTable(sql).ToColumnSchemas(dbCmd);
        }
    }

    /// <summary>
    /// Converts to columnschemas.
    /// </summary>
    /// <param name="dt">The dt.</param>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>ColumnSchema[].</returns>
    static internal ColumnSchema[] ToColumnSchemas(this DataTable dt, IDbCommand dbCmd)
    {
        var ret = new List<ColumnSchema>();
        foreach (DataRow row in dt.Rows)
        {
            var obj = new Dictionary<string, object>();
            foreach (DataColumn column in dt.Columns)
            {
                obj[column.ColumnName] = row[column.Ordinal];
            }

            var to = obj.FromObjectDictionary<ColumnSchema>();
            //MySQL doesn't populate DataTypeName, so reverse populate it from Type Converter ColumnDefinition
            if (to.DataTypeName == null && to.DataType != null)
            {
                to.DataTypeName = dbCmd.GetDialectProvider().GetConverter(to.DataType)?.ColumnDefinition.LeftPart('(');
            }

            if (to.DataTypeName == null)
            {
                continue;
            }

            ret.Add(to);
        }

        return [.. ret];
    }
}