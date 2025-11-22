// ***********************************************************************
// <copyright file="OrmLiteReadApi.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteReadApi.
/// </summary>
public static class OrmLiteReadApi
{
    /// <param name="dbConn">The database connection.</param>
    extension(IDbConnection dbConn)
    {
        /// <summary>
        /// Returns results from the active connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Select<T>()
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>());
        }

        /// <summary>
        /// Returns results from using sql. E.g:
        /// <para>db.Select&lt;Person&gt;("Age &gt; 40")</para><para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; 40")</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Select<T>(string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>(sql));
        }

        /// <summary>
        /// Returns results from using sql. E.g:
        /// <para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new[] { db.CreateParam("age", 40) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Select<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns results from using a parameterized query. E.g:
        /// <para>db.Select&lt;Person&gt;("Age &gt; @age", new { age = 40})</para><para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new { age = 40})</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Select<T>(string sql, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>(sql, anonType));
        }

        /// <summary>
        /// Returns results from using a parameterized query. E.g:
        /// <para>db.Select&lt;Person&gt;("Age &gt; @age", new Dictionary&lt;string, object&gt; { { "age", 40 } })</para><para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new Dictionary&lt;string, object&gt; { { "age", 40 } })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Select<T>(string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>(sql, dict));
        }

        /// <summary>
        /// Returns a partial subset of results from the specified tableType. E.g:
        /// <para>db.Select&lt;EntityWithId&gt;(typeof(Person))</para><para></para>
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;TModel&gt;.</returns>
        public List<TModel> Select<TModel>(Type fromTableType, string sql, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<TModel>(fromTableType, sql, anonType));
        }

        /// <summary>
        /// Returns a partial subset of results from the specified tableType. E.g:
        /// <para>db.Select&lt;EntityWithId&gt;(typeof(Person))</para><para></para>
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="fromTableType">Type of from table.</param>
        /// <returns>List&lt;TModel&gt;.</returns>
        public List<TModel> Select<TModel>(Type fromTableType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<TModel>(fromTableType));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.Where&lt;Person&gt;("Age", 27)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Where<T>(string name, object value)
        {
            return dbConn.Exec(dbCmd => dbCmd.Where<T>(name, value));
        }

        /// <summary>
        /// Returns results from using an anonymous type filter. E.g:
        /// <para>db.Where&lt;Person&gt;(new { Age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Where<T>(object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Where<T>(anonType));
        }

        /// <summary>
        /// Returns results using the supplied primary key ids. E.g:
        /// <para>db.SelectByIds&lt;Person&gt;(new[] { 1, 2, 3 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValues">The identifier values.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SelectByIds<T>(IEnumerable idValues)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectByIds<T>(idValues));
        }

        /// <summary>
        /// Query results using the non-default values in the supplied partially populated POCO example. E.g:
        /// <para>db.SelectNonDefaults(new Person { Id = 1 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SelectNonDefaults<T>(T filter)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectNonDefaults<T>(filter));
        }

        /// <summary>
        /// Query results using the non-default values in the supplied partially populated POCO example. E.g:
        /// <para>db.SelectNonDefaults("Age &gt; @Age", new Person { Age = 42 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SelectNonDefaults<T>(string sql, T filter)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectNonDefaults<T>(sql, filter));
        }

        /// <summary>
        /// Returns a lazyily loaded stream of results. E.g:
        /// <para>db.SelectLazy&lt;Person&gt;()</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> SelectLazy<T>()
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.SelectLazy<T>());
        }

        /// <summary>
        /// Returns a lazyily loaded stream of results using a parameterized query. E.g:
        /// <para>db.SelectLazy&lt;Person&gt;("Age &gt; @age", new { age = 40 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> SelectLazy<T>(string sql, object anonType = null)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.SelectLazy<T>(sql, anonType));
        }

        /// <summary>
        /// Returns a lazyily loaded stream of results using a parameterized query. E.g:
        /// <para>db.SelectLazy(db.From&lt;Person&gt;().Where(x =&gt; x == 40))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> SelectLazy<T>(SqlExpression<T> expression)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.SelectLazy<T>(expression.ToSelectStatement(QueryType.Select), expression.Params));
        }

        /// <summary>
        /// Returns a stream of results that are lazily loaded using a parameterized query. E.g:
        /// <para>db.WhereLazy&lt;Person&gt;(new { Age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> WhereLazy<T>(object anonType)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.WhereLazy<T>(anonType));
        }

        /// <summary>
        /// Returns the first result using a parameterized query. E.g:
        /// <para>db.Single&lt;Person&gt;(new { Age = 42 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>T.</returns>
        public T Single<T>(object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single<T>(anonType));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.Single&lt;Person&gt;("Age = @age", new[] { db.CreateParam("age",40) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>T.</returns>
        public T Single<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.Single&lt;Person&gt;("Age = @age", new { age = 42 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>T.</returns>
        public T Single<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single<T>(sql, anonType));
        }

        /// <summary>
        /// Returns the first result using a primary key id. E.g:
        /// <para>db.SingleById&lt;Person&gt;(1)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValue">The identifier value.</param>
        /// <returns>T.</returns>
        public T SingleById<T>(object idValue)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleById<T>(idValue));
        }

        /// <summary>
        /// Returns the first result using a name, value filter. E.g:
        /// <para>db.SingleWhere&lt;Person&gt;("Age", 42)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public T SingleWhere<T>(string name, object value)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleWhere<T>(name, value));
        }

        /// <summary>
        /// Returns a single scalar value using an SqlExpression. E.g:
        /// <para>db.Column&lt;int&gt;(db.From&lt;Person&gt;().Select(x =&gt; Sql.Count("*")).Where(q =&gt; q.Age &gt; 40))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <returns>T.</returns>
        public T Scalar<T>(ISqlExpression sqlExpression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar<T>(sqlExpression.ToSelectStatement(QueryType.Scalar), sqlExpression.Params));
        }

        /// <summary>
        /// Returns a single scalar value using a parameterized query. E.g:
        /// <para>db.Scalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &gt; @age", new[] { db.CreateParam("age",40) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>T.</returns>
        public T Scalar<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns a single scalar value using a parameterized query. E.g:
        /// <para>db.Scalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &gt; @age", new { age = 40 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>T.</returns>
        public T Scalar<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar<T>(sql, anonType));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlExpression. E.g:
        /// <para>db.Column&lt;int&gt;(db.From&lt;Person&gt;().Select(x =&gt; x.LastName).Where(q =&gt; q.Age == 27))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Column<T>(ISqlExpression query)
        {
            return dbConn.Exec(dbCmd => dbCmd.Column<T>(query.ToSelectStatement(QueryType.Select), query.Params));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.Column&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new[] { db.CreateParam("age",27) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Column<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Column<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlExpression. E.g:
        /// <para>db.ColumnLazy&lt;int&gt;(db.From&lt;Person&gt;().Select(x =&gt; x.LastName).Where(q =&gt; q.Age == 27))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> ColumnLazy<T>(ISqlExpression query)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.ColumnLazy<T>(query.ToSelectStatement(QueryType.Select), query.Params));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.ColumnLazy&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new[] { db.CreateParam("age",27) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> ColumnLazy<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.ColumnLazy<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.ColumnLazy&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new { age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> ColumnLazy<T>(string sql, object anonType = null)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.ColumnLazy<T>(sql, anonType));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.Column&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new { age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Column<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Column<T>(sql, anonType));
        }

        /// <summary>
        /// Columns the distinct.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        public HashSet<T> ColumnDistinct<T>(ISqlExpression query)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinct<T>(query));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlFormat query. E.g:
        /// <para>db.ColumnDistinct&lt;int&gt;("SELECT Age FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        public HashSet<T> ColumnDistinct<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinct<T>(sql, anonType));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlFormat query. E.g:
        /// <para>db.ColumnDistinct&lt;int&gt;("SELECT Age FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        public HashSet<T> ColumnDistinct<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinct<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an Sql Expression. E.g:
        /// <para>db.Lookup&lt;int, string&gt;(db.From&lt;Person&gt;().Select(x =&gt; new { x.Age, x.LastName }).Where(q =&gt; q.Age &lt; 50))</para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
        public Dictionary<K, List<V>> Lookup<K, V>(ISqlExpression sqlExpression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Lookup<K, V>(sqlExpression.ToSelectStatement(QueryType.Select), sqlExpression.Params));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an parameterized query. E.g:
        /// <para>db.Lookup&lt;int, string&gt;("SELECT Age, LastName FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
        public Dictionary<K, List<V>> Lookup<K, V>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Lookup<K, V>(sql, sqlParams));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an parameterized query. E.g:
        /// <para>db.Lookup&lt;int, string&gt;("SELECT Age, LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
        public Dictionary<K, List<V>> Lookup<K, V>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Lookup<K, V>(sql, anonType));
        }

        /// <summary>
        /// Dictionaries the specified query.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns>Dictionary&lt;K, V&gt;.</returns>
        public Dictionary<K, V> Dictionary<K, V>(ISqlExpression query)
        {
            return dbConn.Exec(dbCmd => dbCmd.Dictionary<K, V>(query));
        }

        /// <summary>
        /// Returns a Dictionary from the first 2 columns: Column 1 (Keys), Column 2 (Values) using sql. E.g:
        /// <para>db.Dictionary&lt;int, string&gt;("SELECT Id, LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>Dictionary&lt;K, V&gt;.</returns>
        public Dictionary<K, V> Dictionary<K, V>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Dictionary<K, V>(sql, anonType));
        }

        /// <summary>
        /// Keys the value pairs.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns>List&lt;KeyValuePair&lt;K, V&gt;&gt;.</returns>
        public List<KeyValuePair<K, V>> KeyValuePairs<K, V>(ISqlExpression query)
        {
            return dbConn.Exec(dbCmd => dbCmd.KeyValuePairs<K, V>(query));
        }

        /// <summary>
        /// Returns a list of KeyValuePairs from the first 2 columns: Column 1 (Keys), Column 2 (Values) using sql. E.g:
        /// <para>db.KeyValuePairs&lt;int, string&gt;("SELECT Id, LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;KeyValuePair&lt;K, V&gt;&gt;.</returns>
        public List<KeyValuePair<K, V>> KeyValuePairs<K, V>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.KeyValuePairs<K, V>(sql, anonType));
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the LINQ expression, E.g:
        /// <para>db.Exists&lt;Person&gt;(x =&gt; x.Age &lt; 50)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Exists<T>(Expression<Func<T, bool>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar(dbConn.From<T>().Where(expression).Limit(1).Select("'exists'"))) != null;
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
        /// <para>db.Exists(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Exists<T>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar(expression.Limit(1).Select("'exists'"))) != null;
        }

        /// <summary>
        /// Returns true if the Query returns any records, using an SqlFormat query. E.g:
        /// <para>db.Exists&lt;Person&gt;(new { Age = 42 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Exists<T>(object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Exists<T>(anonType));
        }

        /// <summary>
        /// Returns true if the Query returns any records, using a parameterized query. E.g:
        /// <para>db.Exists&lt;Person&gt;("Age = @age", new { age = 42 })</para><para>db.Exists&lt;Person&gt;("SELECT * FROM Person WHERE Age = @age", new { age = 42 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Exists<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Exists<T>(sql, anonType));
        }

        /// <summary>
        /// Returns true if the Query returns any records, using a parameterized query. E.g:
        /// <para>db.ExistsById&lt;Person&gt;(1)</para>
        /// </summary>
        public bool ExistsById<T>(object id)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExistsById<T>(id));
        }

        /// <summary>
        /// Returns results from an arbitrary SqlExpression. E.g:
        /// <para>db.SqlList&lt;Person&gt;(db.From&lt;Person&gt;().Select("*").Where(q =&gt; q.Age &lt; 50))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SqlList<T>(ISqlExpression sqlExpression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sqlExpression.ToSelectStatement(QueryType.Select), sqlExpression.Params));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SqlList<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SqlList<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sql, anonType));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", new Dictionary&lt;string, object&gt; { { "age", 42 } })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SqlList<T>(string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sql, dict));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query with a dbCmd filter. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", dbCmd =&gt; ...)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dbCmdFilter">The database command filter.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SqlList<T>(string sql, Action<IDbCommand> dbCmdFilter)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sql, dbCmdFilter));
        }

        /// <summary>
        /// Prepare Stored Procedure with Input parameters, optionally populated with Input Params. E.g:
        /// <para>var cmd = db.SqlProc("GetRockstarsAged", new { age = 42 })</para>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="inParams">The in parameters.</param>
        /// <param name="excludeDefaults">if set to <c>true</c> [exclude defaults].</param>
        /// <returns>IDbCommand.</returns>
        public IDbCommand SqlProc(string name, object inParams = null, bool excludeDefaults = false)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlProc(name, inParams, excludeDefaults));
        }

        /// <summary>
        /// SQLs the column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SqlColumn<T>(ISqlExpression sqlExpression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumn<T>(sqlExpression.ToSelectStatement(QueryType.Select), sqlExpression.Params));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumn&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SqlColumn<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumn<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumn&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SqlColumn<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumn<T>(sql, anonType));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumn&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new Dictionary&lt;string, object&gt; { { "age", 50 } })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SqlColumn<T>(string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumn<T>(sql, dict));
        }

        /// <summary>
        /// Returns a single Scalar value using an SqlExpression. E.g:
        /// <para>db.SqlScalar&lt;int&gt;(db.From&lt;Person&gt;().Select(Sql.Count("*")).Where(q =&gt; q.Age &lt; 50))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <returns>T.</returns>
        public T SqlScalar<T>(ISqlExpression sqlExpression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalar<T>(sqlExpression.ToSelectStatement(QueryType.Select), sqlExpression.Params));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new[]{ db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>T.</returns>
        public T SqlScalar<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalar<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>T.</returns>
        public T SqlScalar<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalar<T>(sql, anonType));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new Dictionary&lt;string, object&gt; { { "age", 50 } })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <returns>T.</returns>
        public T SqlScalar<T>(string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalar<T>(sql, dict));
        }

        /// <summary>
        /// Returns the last insert Id made from this connection.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public long LastInsertId()
        {
            return dbConn.Exec(dbCmd => dbCmd.LastInsertId());
        }

        /// <summary>
        /// Executes a raw sql non-query using sql. E.g:
        /// <para>var rowsAffected = db.ExecuteNonQuery("UPDATE Person SET LastName={0} WHERE Id={1}".SqlFormat("WaterHouse", 7))</para>
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>number of rows affected</returns>
        public int ExecuteNonQuery(string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQuery(sql));
        }

        /// <summary>
        /// Executes a raw sql non-query using a parameterized query. E.g:
        /// <para>var rowsAffected = db.ExecuteNonQuery("UPDATE Person SET LastName=@name WHERE Id=@id", new { name = "WaterHouse", id = 7 })</para>
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>number of rows affected</returns>
        public int ExecuteNonQuery(string sql, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQuery(sql, anonType));
        }

        /// <summary>
        /// Executes a raw sql non-query using a parameterized query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <returns>number of rows affected</returns>
        public int ExecuteNonQuery(string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQuery(sql, dict));
        }

        /// <summary>
        /// Executes a raw sql non-query using a parameterized query with a dbCmd filter. E.g:
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="dbCmdFilter">The database command filter.</param>
        /// <returns>number of rows affected</returns>
        public int ExecuteNonQuery(string sql, Action<IDbCommand> dbCmdFilter)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQuery(sql, dbCmdFilter));
        }

        /// <summary>
        /// Returns results from a Stored Procedure, using a parameterized query.
        /// </summary>
        /// <typeparam name="TOutputModel">The type of the t output model.</typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;TOutputModel&gt;.</returns>
        public List<TOutputModel> SqlProcedure<TOutputModel>(object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlProcedure<TOutputModel>(anonType));
        }

        /// <summary>
        /// Returns results from a Stored Procedure using an SqlFormat query. E.g:
        /// <para></para>
        /// </summary>
        /// <typeparam name="TOutputModel">The type of the t output model.</typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>List&lt;TOutputModel&gt;.</returns>
        public List<TOutputModel> SqlProcedure<TOutputModel>(object anonType,
            string sqlFilter,
            params object[] filterParams)
            where TOutputModel : new()
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlProcedureFmt<TOutputModel>(
                anonType, sqlFilter, filterParams));
        }

        /// <summary>
        /// Returns the scalar result as a long.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public long LongScalar()
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecLongScalar());
        }

        /// <summary>
        /// Returns the first result with all its references loaded, using a primary key id. E.g:
        /// <para>db.LoadSingleById&lt;Person&gt;(1, include = new[]{ "Address" })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValue">The identifier value.</param>
        /// <param name="include">The include.</param>
        /// <returns>T.</returns>
        public T LoadSingleById<T>(object idValue, string[] include = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSingleById<T>(idValue, include));
        }

        /// <summary>
        /// Returns the first result with all its references loaded, using a primary key id. E.g:
        /// <para>db.LoadSingleById&lt;Person&gt;(1, include = x =&gt; new{ x.Address })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValue">The identifier value.</param>
        /// <param name="include">The include.</param>
        /// <returns>T.</returns>
        public T LoadSingleById<T>(object idValue, Expression<Func<T, object>> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSingleById<T>(idValue, include.GetFieldNames()));
        }

        /// <summary>
        /// Loads all the related references onto the instance. E.g:
        /// <para>db.LoadReferences(customer)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        public void LoadReferences<T>(T instance)
        {
            dbConn.Exec(dbCmd => dbCmd.LoadReferences(instance));
        }
    }
}