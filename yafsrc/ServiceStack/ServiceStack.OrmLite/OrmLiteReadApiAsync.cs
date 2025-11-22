// ***********************************************************************
// <copyright file="OrmLiteReadApiAsync.cs" company="ServiceStack, Inc.">
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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteReadApiAsync.
/// </summary>
public static class OrmLiteReadApiAsync
{
    /// <param name="dbConn">The database connection.</param>
    extension(IDbConnection dbConn)
    {
        /// <summary>
        /// Returns results from the active connection, E.g:
        /// <para>db.SelectAsync&lt;Person&gt;()</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SelectAsync<T>(CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<T>(token));
        }

        /// <summary>
        /// Returns results from using sql. E.g:
        /// <para>db.SelectAsync&lt;Person&gt;("Age &gt; 40")</para><para>db.SelectAsync&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; 40")</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SelectAsync<T>(string sql, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<T>(sql, (object)null, token));
        }

        /// <summary>
        /// Returns results from using a parameterized query. E.g:
        /// <para>db.SelectAsync&lt;Person&gt;("Age &gt; @age", new { age = 40})</para><para>db.SelectAsync&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new[] { db.CreateParam("age",40) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SelectAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns results from using a parameterized query. E.g:
        /// <para>db.SelectAsync&lt;Person&gt;("Age &gt; @age", new { age = 40})</para><para>db.SelectAsync&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new { age = 40})</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SelectAsync<T>(string sql, object anonType, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns results from using a parameterized query. E.g:
        /// <para>db.SelectAsync&lt;Person&gt;("Age &gt; @age", new Dictionary&lt;string, object&gt; { { "age", 40 } })</para><para>db.SelectAsync&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new Dictionary&lt;string, object&gt; { { "age", 40 } })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SelectAsync<T>(string sql, Dictionary<string, object> dict, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<T>(sql, dict, token));
        }

        /// <summary>
        /// Returns a partial subset of results from the specified tableType. E.g:
        /// <para>db.SelectAsync&lt;EntityWithId&gt;(typeof(Person))</para><para></para>
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;TModel&gt;&gt;.</returns>
        public Task<List<TModel>> SelectAsync<TModel>(Type fromTableType, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<TModel>(fromTableType, token));
        }

        /// <summary>
        /// Returns a partial subset of results from the specified tableType. E.g:
        /// <para>db.SelectAsync&lt;EntityWithId&gt;(typeof(Person), "Age = @age", new { age = 27 })</para><para></para>
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;TModel&gt;&gt;.</returns>
        public Task<List<TModel>> SelectAsync<TModel>(Type fromTableType, string sqlFilter, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<TModel>(fromTableType, sqlFilter, anonType, token));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.WhereAsync&lt;Person&gt;("Age", 27)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> WhereAsync<T>(string name, object value, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.WhereAsync<T>(name, value, token));
        }

        /// <summary>
        /// Returns results from using an anonymous type filter. E.g:
        /// <para>db.WhereAsync&lt;Person&gt;(new { Age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> WhereAsync<T>(object anonType, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.WhereAsync<T>(anonType, token));
        }

        /// <summary>
        /// Returns results using the supplied primary key ids. E.g:
        /// <para>db.SelectByIdsAsync&lt;Person&gt;(new[] { 1, 2, 3 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValues">The identifier values.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SelectByIdsAsync<T>(IEnumerable idValues, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectByIdsAsync<T>(idValues, token));
        }

        /// <summary>
        /// Query results using the non-default values in the supplied partially populated POCO example. E.g:
        /// <para>db.SelectNonDefaultsAsync(new Person { Id = 1 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SelectNonDefaultsAsync<T>(T filter, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectNonDefaultsAsync<T>(filter, token));
        }

        /// <summary>
        /// Query results using the non-default values in the supplied partially populated POCO example. E.g:
        /// <para>db.SelectNonDefaultsAsync("Age &gt; @Age", new Person { Age = 42 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SelectNonDefaultsAsync<T>(string sql, T filter, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectNonDefaultsAsync<T>(sql, filter, token));
        }

        /// <summary>
        /// Returns the first result using a parameterized query. E.g:
        /// <para>db.SingleAsync&lt;Person&gt;(new { Age = 42 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> SingleAsync<T>(object anonType, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleAsync<T>(anonType, token));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.SingleAsync&lt;Person&gt;("Age = @age", new[] { db.CreateParam("age",42) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> SingleAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.SingleAsync&lt;Person&gt;("Age = @age", new { age = 42 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> SingleAsync<T>(string sql, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns the first result using a primary key id. E.g:
        /// <para>db.SingleByIdAsync&lt;Person&gt;(1)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValue">The identifier value.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> SingleByIdAsync<T>(object idValue, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleByIdAsync<T>(idValue, token));
        }

        /// <summary>
        /// Returns the first result using a name, value filter. E.g:
        /// <para>db.SingleWhereAsync&lt;Person&gt;("Age", 42)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> SingleWhereAsync<T>(string name, object value, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleWhereAsync<T>(name, value, token));
        }
    }

    /// <summary>
    /// Scalars the asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="sqlParams">The SQL parameters.</param>
    /// <returns>T.</returns>
    public static T ScalarAsync<T>(this IDbCommand dbCmd, string sql, IEnumerable<IDbDataParameter> sqlParams)
    {
        if (sqlParams != null)
        {
            dbCmd.SetParameters(sqlParams);
        }

        return dbCmd.Scalar<T>(sql);
    }

    /// <param name="dbConn">The database connection.</param>
    extension(IDbConnection dbConn)
    {
        /// <summary>
        /// Returns a single scalar value using an SqlExpression. E.g:
        /// <para>db.ScalarAsync&lt;int&gt;(db.From&lt;Person&gt;().Select(x =&gt; Sql.Count("*")).Where(q =&gt; q.Age &gt; 40))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> ScalarAsync<T>(ISqlExpression sqlExpression, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarAsync<T>(sqlExpression.ToSelectStatement(QueryType.Scalar), sqlExpression.Params, token));
        }

        /// <summary>
        /// Returns a single scalar value using a parameterized query. E.g:
        /// <para>db.ScalarAsync&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &gt; @age", new[] { db.CreateParam("age",40) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> ScalarAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns a single scalar value using a parameterized query. E.g:
        /// <para>db.ScalarAsync&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &gt; @age", new { age = 40 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> ScalarAsync<T>(string sql, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlExpression. E.g:
        /// <para>db.ColumnAsync&lt;int&gt;(db.From&lt;Person&gt;().Select(x =&gt; x.LastName).Where(q =&gt; q.Age == 27))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> ColumnAsync<T>(ISqlExpression query, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnAsync<T>(query.ToSelectStatement(QueryType.Scalar), query.Params, token));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.ColumnAsync&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new[] { db.CreateParam("age",27) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> ColumnAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.ColumnAsync&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new { age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> ColumnAsync<T>(string sql, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Columns the distinct asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;HashSet&lt;T&gt;&gt;.</returns>
        public Task<HashSet<T>> ColumnDistinctAsync<T>(ISqlExpression query, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinctAsync<T>(query.ToSelectStatement(QueryType.Scalar), query.Params, token));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlFormat query. E.g:
        /// <para>db.ColumnDistinctAsync&lt;int&gt;("SELECT Age FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;HashSet&lt;T&gt;&gt;.</returns>
        public Task<HashSet<T>> ColumnDistinctAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinctAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlFormat query. E.g:
        /// <para>db.ColumnDistinctAsync&lt;int&gt;("SELECT Age FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;HashSet&lt;T&gt;&gt;.</returns>
        public Task<HashSet<T>> ColumnDistinctAsync<T>(string sql, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinctAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Lookups the asynchronous.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Dictionary&lt;K, List&lt;V&gt;&gt;&gt;.</returns>
        public Task<Dictionary<K, List<V>>> LookupAsync<K, V>(ISqlExpression sqlExpression, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.LookupAsync<K, V>(sqlExpression.ToSelectStatement(QueryType.Scalar), sqlExpression.Params, token));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an parameterized query. E.g:
        /// <para>db.LookupAsync&lt;int, string&gt;("SELECT Age, LastName FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Dictionary&lt;K, List&lt;V&gt;&gt;&gt;.</returns>
        public Task<Dictionary<K, List<V>>> LookupAsync<K, V>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.LookupAsync<K, V>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an parameterized query. E.g:
        /// <para>db.LookupAsync&lt;int, string&gt;("SELECT Age, LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Dictionary&lt;K, List&lt;V&gt;&gt;&gt;.</returns>
        public Task<Dictionary<K, List<V>>> LookupAsync<K, V>(string sql, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.LookupAsync<K, V>(sql, anonType, token));
        }

        /// <summary>
        /// Dictionaries the asynchronous.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Dictionary&lt;K, V&gt;&gt;.</returns>
        public Task<Dictionary<K, V>> DictionaryAsync<K, V>(ISqlExpression query, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DictionaryAsync<K, V>(query.ToSelectStatement(QueryType.Scalar), query.Params, token));
        }

        /// <summary>
        /// Returns a Dictionary from the first 2 columns: Column 1 (Keys), Column 2 (Values) using sql. E.g:
        /// <para>db.DictionaryAsync&lt;int, string&gt;("SELECT Id, LastName FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Dictionary&lt;K, V&gt;&gt;.</returns>
        public Task<Dictionary<K, V>> DictionaryAsync<K, V>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DictionaryAsync<K, V>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns a Dictionary from the first 2 columns: Column 1 (Keys), Column 2 (Values) using sql. E.g:
        /// <para>db.DictionaryAsync&lt;int, string&gt;("SELECT Id, LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Dictionary&lt;K, V&gt;&gt;.</returns>
        public Task<Dictionary<K, V>> DictionaryAsync<K, V>(string sql, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DictionaryAsync<K, V>(sql, anonType, token));
        }

        /// <summary>
        /// Keys the value pairs asynchronous.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;KeyValuePair&lt;K, V&gt;&gt;&gt;.</returns>
        public Task<List<KeyValuePair<K, V>>> KeyValuePairsAsync<K, V>(ISqlExpression query, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.KeyValuePairsAsync<K, V>(query.ToSelectStatement(QueryType.Scalar), query.Params, token));
        }

        /// <summary>
        /// Returns a list of KeyValuePairs from the first 2 columns: Column 1 (Keys), Column 2 (Values) using sql. E.g:
        /// <para>db.KeyValuePairsAsync&lt;int, string&gt;("SELECT Id, LastName FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;KeyValuePair&lt;K, V&gt;&gt;&gt;.</returns>
        public Task<List<KeyValuePair<K, V>>> KeyValuePairsAsync<K, V>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.KeyValuePairsAsync<K, V>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns a list of KeyValuePairs from the first 2 columns: Column 1 (Keys), Column 2 (Values) using sql. E.g:
        /// <para>db.KeyValuePairsAsync&lt;int, string&gt;("SELECT Id, LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;KeyValuePair&lt;K, V&gt;&gt;&gt;.</returns>
        public Task<List<KeyValuePair<K, V>>> KeyValuePairsAsync<K, V>(string sql, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.KeyValuePairsAsync<K, V>(sql, anonType, token));
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the LINQ expression, E.g:
        /// <para>db.ExistsAsync&lt;Person&gt;(x =&gt; x.Age &lt; 50)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> expression, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarAsync(dbConn.From<T>().Where(expression).Limit(1).Select("'exists'"), token).Then(x => x != null));
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
        /// <para>db.ExistsAsync(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExistsAsync<T>(SqlExpression<T> expression, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarAsync(expression.Limit(1).Select("'exists'"), token).Then(x => x != null));
        }

        /// <summary>
        /// Returns true if the Query returns any records, using an SqlFormat query. E.g:
        /// <para>db.ExistsAsync&lt;Person&gt;(new { Age = 42 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExistsAsync<T>(object anonType, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExistsAsync<T>(anonType, token));
        }

        /// <summary>
        /// Returns true if the Query returns any records, using a parameterized query. E.g:
        /// <para>db.ExistsAsync&lt;Person&gt;("Age = @age", new { age = 42 })</para><para>db.ExistsAsync&lt;Person&gt;("SELECT * FROM Person WHERE Age = @age", new { age = 42 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExistsAsync<T>(string sql, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExistsAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns true if the Query returns any records, using a parameterized query. E.g:
        /// <para>db.ExistsByIdAsync&lt;Person&gt;(1)</para>
        /// </summary>
        public Task<bool> ExistsByIdAsync<T>(object id, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExistsByIdAsync<T>(id, token));
        }

        /// <summary>
        /// Returns results from an arbitrary SqlExpression. E.g:
        /// <para>db.SqlListAsync&lt;Person&gt;(db.From&lt;Person&gt;().Select("*").Where(q =&gt; q.Age &lt; 50))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SqlListAsync<T>(ISqlExpression sqlExpression, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlListAsync<T>(sqlExpression.ToSelectStatement(QueryType.Scalar), sqlExpression.Params, token));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlListAsync&lt;Person&gt;("EXEC GetRockstarsAged @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SqlListAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlListAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlListAsync&lt;Person&gt;("EXEC GetRockstarsAged @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SqlListAsync<T>(string sql, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlListAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlListAsync&lt;Person&gt;("EXEC GetRockstarsAged @age", new Dictionary&lt;string, object&gt; { { "age", 42 } })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SqlListAsync<T>(string sql, Dictionary<string, object> dict, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlListAsync<T>(sql, dict, token));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query with a dbCmd filter. E.g:
        /// <para>db.SqlListAsync&lt;Person&gt;("EXEC GetRockstarsAged @age", dbCmd =&gt; ...)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dbCmdFilter">The database command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SqlListAsync<T>(string sql, Action<IDbCommand> dbCmdFilter, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlListAsync<T>(sql, dbCmdFilter, token));
        }

        /// <summary>
        /// SQLs the column asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SqlColumnAsync<T>(ISqlExpression sqlExpression, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumnAsync<T>(sqlExpression.ToSelectStatement(QueryType.Scalar), sqlExpression.Params, token));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumnAsync&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SqlColumnAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumnAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumnAsync&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SqlColumnAsync<T>(string sql, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumnAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumnAsync&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new Dictionary&lt;string, object&gt; { { "age", 50 } })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public Task<List<T>> SqlColumnAsync<T>(string sql, Dictionary<string, object> dict, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumnAsync<T>(sql, dict, token));
        }

        /// <summary>
        /// Returns a single Scalar value using an SqlExpression. E.g:
        /// <para>db.SqlScalarAsync&lt;int&gt;(db.From&lt;Person&gt;().Select(Sql.Count("*")).Where(q =&gt; q.Age &lt; 50))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> SqlScalarAsync<T>(ISqlExpression sqlExpression, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalarAsync<T>(sqlExpression.ToSelectStatement(QueryType.Scalar), sqlExpression.Params, token));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalarAsync&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> SqlScalarAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalarAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalarAsync&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> SqlScalarAsync<T>(string sql, object anonType = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalarAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalarAsync&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new Dictionary&lt;string, object&gt; { { "age", 50 } })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> SqlScalarAsync<T>(string sql, Dictionary<string, object> dict, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalarAsync<T>(sql, dict, token));
        }

        /// <summary>
        /// Executes a raw sql non-query using sql. E.g:
        /// <para>var rowsAffected = db.ExecuteNonQueryAsync("UPDATE Person SET LastName={0} WHERE Id={1}".SqlFormat("WaterHouse", 7))</para>
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows affected</returns>
        public Task<int> ExecuteNonQueryAsync(string sql, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQueryAsync(sql, null, token));
        }

        /// <summary>
        /// Executes a raw sql non-query using a parameterized query. E.g:
        /// <para>var rowsAffected = db.ExecuteNonQueryAsync("UPDATE Person SET LastName=@name WHERE Id=@id", new { name = "WaterHouse", id = 7 })</para>
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows affected</returns>
        public Task<int> ExecuteNonQueryAsync(string sql, object anonType, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQueryAsync(sql, anonType, token));
        }

        /// <summary>
        /// Executes a raw sql non-query using a parameterized query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows affected</returns>
        public Task<int> ExecuteNonQueryAsync(string sql, Dictionary<string, object> dict, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQueryAsync(sql, dict, token));
        }

        /// <summary>
        /// Returns results from a Stored Procedure, using a parameterized query.
        /// </summary>
        /// <typeparam name="TOutputModel">The type of the t output model.</typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;TOutputModel&gt;&gt;.</returns>
        public Task<List<TOutputModel>> SqlProcedureAsync<TOutputModel>(object anonType, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlProcedureAsync<TOutputModel>(anonType, token));
        }

        /// <summary>
        /// Returns the scalar result as a long.
        /// </summary>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        public Task<long> LongScalarAsync(CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecLongScalarAsync(null, token));
        }

        /// <summary>
        /// Returns the first result with all its references loaded, using a primary key id. E.g:
        /// <para>db.LoadSingleByIdAsync&lt;Person&gt;(1)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValue">The identifier value.</param>
        /// <param name="include">The include.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> LoadSingleByIdAsync<T>(object idValue, string[] include = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSingleByIdAsync<T>(idValue, include, token));
        }

        /// <summary>
        /// Returns the first result with all its references loaded, using a primary key id. E.g:
        /// <para>db.LoadSingleByIdAsync&lt;Person&gt;(1, include = x =&gt; new { x.Address })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValue">The identifier value.</param>
        /// <param name="include">The include.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> LoadSingleByIdAsync<T>(object idValue, Expression<Func<T, object>> include, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSingleByIdAsync<T>(idValue, include.GetFieldNames(), token));
        }

        /// <summary>
        /// Loads all the related references onto the instance. E.g:
        /// <para>db.LoadReferencesAsync(customer)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="include">The include.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task LoadReferencesAsync<T>(T instance, string[] include = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadReferencesAsync(instance, include, token));
        }
    }
}