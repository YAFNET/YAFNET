// ***********************************************************************
// <copyright file="OrmLiteWriteApiAsync.cs" company="ServiceStack, Inc.">
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Data;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteWriteApiAsync.
/// </summary>
public static class OrmLiteWriteApiAsync
{
    /// <param name="dbConn">The database connection.</param>
    extension(IDbConnection dbConn)
    {
        /// <summary>
        /// Execute any arbitrary raw SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows affected</returns>
        public Task<int> ExecuteSqlAsync(string sql, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteSqlAsync(sql, token));
        }

        /// <summary>
        /// Execute any arbitrary raw SQL with db params.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="dbParams">The database parameters.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows affected</returns>
        public Task<int> ExecuteSqlAsync(string sql, object dbParams, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteSqlAsync(sql, dbParams, token));
        }

        /// <summary>
        /// Insert 1 POCO, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
        /// <para>var id = db.Insert(new Person { Id = 1, FirstName = "Jimi }, selectIdentity:true)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
        /// <param name="enableIdentityInsert">if set to <c>true</c> [enable identity insert].</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        public Task<long> InsertAsync<T>(T obj, bool selectIdentity = false,
            bool enableIdentityInsert = false, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAsync(obj, commandFilter: null, selectIdentity: selectIdentity, enableIdentityInsert, token: token));
        }

        /// <summary>
        /// Insert 1 POCO, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
        /// <para>var id = db.Insert(new Person { Id = 1, FirstName = "Jimi }, selectIdentity:true)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
        /// <param name="enableIdentityInsert">if set to <c>true</c> [enable identity insert].</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        public Task<long> InsertAsync<T>(T obj, Action<IDbCommand> commandFilter, bool selectIdentity = false, bool enableIdentityInsert = false, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAsync(obj, commandFilter: commandFilter, selectIdentity: selectIdentity, enableIdentityInsert, token: token));
        }

        /// <summary>
        /// Insert 1 POCO, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
        /// <para>var id = db.Insert(new Dictionary&lt;string,object&gt; { ["Id"] = 1, ["FirstName"] = "Jimi }, selectIdentity:true)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        public Task<long> InsertAsync<T>(Dictionary<string, object> obj, bool selectIdentity = false, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAsync<T>(obj, commandFilter: null, selectIdentity: selectIdentity, token));
        }

        /// <summary>
        /// Insert 1 POCO, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
        /// <para>var id = db.Insert(new Dictionary&lt;string,object&gt; { ["Id"] = 1, ["FirstName"] = "Jimi }, dbCmd =&gt; applyFilter(dbCmd))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="obj">The object.</param>
        /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        public Task<long> InsertAsync<T>(Action<IDbCommand> commandFilter,
            Dictionary<string, object> obj, bool selectIdentity = false, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAsync<T>(obj, commandFilter: commandFilter, selectIdentity: selectIdentity, token));
        }

        /// <summary>
        /// Insert 1 or more POCOs in a transaction. E.g:
        /// <para>db.InsertAsync(new Person { Id = 1, FirstName = "Tupac", LastName = "Shakur", Age = 25 },</para><para>               new Person { Id = 2, FirstName = "Biggie", LastName = "Smalls", Age = 24 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="objs">The objs.</param>
        /// <returns>Task.</returns>
        public Task InsertAsync<T>(CancellationToken token, params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAsync(commandFilter: null, token: token, objs: objs));
        }

        /// <summary>
        /// Inserts the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs">The objs.</param>
        /// <returns>Task.</returns>
        public Task InsertAsync<T>(params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAsync(commandFilter: null, token: CancellationToken.None, objs: objs));
        }

        /// <summary>
        /// Insert 1 or more POCOs in a transaction and modify populated IDbCommand with a commandFilter. E.g:
        /// <para>db.InsertAsync(dbCmd =&gt; applyFilter(dbCmd), token, </para><para>               new Person { Id = 1, FirstName = "Tupac", LastName = "Shakur", Age = 25 },</para><para>               new Person { Id = 2, FirstName = "Biggie", LastName = "Smalls", Age = 24 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="objs">The objs.</param>
        /// <returns>Task.</returns>
        public Task InsertAsync<T>(Action<IDbCommand> commandFilter, CancellationToken token, params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAsync(commandFilter: commandFilter, token: token, objs: objs));
        }

        /// <summary>
        /// Insert 1 or more POCOs in a transaction using Table default values when defined. E.g:
        /// <para>db.InsertUsingDefaultsAsync(new Person { FirstName = "Tupac", LastName = "Shakur" },</para><para>                            new Person { FirstName = "Biggie", LastName = "Smalls" })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs">The objs.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task InsertUsingDefaultsAsync<T>(T[] objs, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertUsingDefaultsAsync(objs, token));
        }

        /// <summary>
        /// Insert results from SELECT SqlExpression, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
        /// <para>db.InsertIntoSelectAsync&lt;Contact&gt;(db.From&lt;Person&gt;().Select(x =&gt; new { x.Id, Surname == x.LastName }))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        public Task<long> InsertIntoSelectAsync<T>(ISqlExpression query, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertIntoSelectAsync<T>(query, commandFilter: null, token: token));
        }

        /// <summary>
        /// Insert results from SELECT SqlExpression, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
        /// <para>db.InsertIntoSelectAsync&lt;Contact&gt;(db.From&lt;Person&gt;().Select(x =&gt; new { x.Id, Surname == x.LastName }))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        public Task<long> InsertIntoSelectAsync<T>(ISqlExpression query, Action<IDbCommand> commandFilter, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertIntoSelectAsync<T>(query, commandFilter: commandFilter, token: token));
        }

        /// <summary>
        /// Insert a collection of POCOs in a transaction. E.g:
        /// <para>db.InsertAllAsync(new[] { new Person { Id = 9, FirstName = "Biggie", LastName = "Smalls", Age = 24 } })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs">The objs.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task InsertAllAsync<T>(IEnumerable<T> objs, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAllAsync(objs, commandFilter: null, token: token));
        }

        /// <summary>
        /// Insert a collection of POCOs in a transaction and modify populated IDbCommand with a commandFilter. E.g:
        /// <para>db.InsertAllAsync(new[] { new Person { Id = 9, FirstName = "Biggie", LastName = "Smalls", Age = 24 } },</para><para>                  dbCmd =&gt; applyFilter(dbCmd))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs">The objs.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task InsertAllAsync<T>(IEnumerable<T> objs, Action<IDbCommand> commandFilter, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAllAsync(objs, commandFilter: commandFilter, token: token));
        }

        /// <summary>
        /// Updates 1 POCO. All fields are updated except for the PrimaryKey which is used as the identity selector. E.g:
        /// <para>db.Update(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAsync<T>(T obj, Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(obj, token, commandFilter));
        }

        /// <summary>
        /// Updates 1 POCO. All fields are updated except for the PrimaryKey which is used as the identity selector. E.g:
        /// <para>db.Update(new Dictionary&lt;string,object&gt; { ["Id"] = 1, ["FirstName"] = "Jimi", ["Age"] = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAsync<T>(Dictionary<string, object> obj, Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync<T>(obj, token, commandFilter));
        }

        /// <summary>
        /// Updates 1 or more POCOs in a transaction. E.g:
        /// <para>db.Update(new Person { Id = 1, FirstName = "Tupac", LastName = "Shakur", Age = 25 },</para><para>new Person { Id = 2, FirstName = "Biggie", LastName = "Smalls", Age = 24 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="objs">The objs.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAsync<T>(CancellationToken token, params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(commandFilter: null, token: token, objs: objs));
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs">The objs.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAsync<T>(params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(commandFilter: null, token: CancellationToken.None, objs: objs));
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="objs">The objs.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAsync<T>(Action<IDbCommand> commandFilter, CancellationToken token, params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(commandFilter: commandFilter, token: token, objs: objs));
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="objs">The objs.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAsync<T>(Action<IDbCommand> commandFilter, params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(commandFilter: commandFilter, token: CancellationToken.None, objs: objs));
        }

        /// <summary>
        /// Updates 1 or more POCOs in a transaction. E.g:
        /// <para>db.UpdateAllAsync(new[] { new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 } })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs">The objs.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAllAsync<T>(IEnumerable<T> objs, Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAllAsync(objs, commandFilter, token));
        }

        /// <summary>
        /// Delete rows using an anonymous type filter. E.g:
        /// <para>db.DeleteAsync&lt;Person&gt;(new { FirstName = "Jimi", Age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonFilter">The anon filter.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows deleted</returns>
        public Task<int> DeleteAsync<T>(object anonFilter,
            Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync<T>(anonFilter, token));
        }

        /// <summary>
        /// Delete rows using an Object Dictionary filters. E.g:
        /// <para>db.DeleteAsync&lt;Person&gt;(new Dictionary&lt;string,object&gt; { ["FirstName"] = "Jimi", ["Age"] = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filters">The filters.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows deleted</returns>
        public Task<int> DeleteAsync<T>(Dictionary<string, object> filters,
            Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync<T>(filters, token));
        }

        /// <summary>
        /// Delete 1 row using all fields in the commandFilter. E.g:
        /// <para>db.DeleteAsync(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allFieldsFilter">All fields filter.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows deleted</returns>
        public Task<int> DeleteAsync<T>(T allFieldsFilter,
            Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(allFieldsFilter, token));
        }

        /// <summary>
        /// Delete 1 or more rows in a transaction using all fields in the commandFilter. E.g:
        /// <para>db.DeleteAsync(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="allFieldsFilters">All fields filters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> DeleteAsync<T>(Action<IDbCommand> commandFilter = null, CancellationToken token = default, params T[] allFieldsFilters)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(token, allFieldsFilters));
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="allFieldsFilters">All fields filters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> DeleteAsync<T>(Action<IDbCommand> commandFilter = null, params T[] allFieldsFilters)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(CancellationToken.None, allFieldsFilters));
        }

        /// <summary>
        /// Delete 1 or more rows using only field with non-default values in the commandFilter. E.g:
        /// <para>db.DeleteNonDefaultsAsync(new Person { FirstName = "Jimi", Age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nonDefaultsFilter">The non defaults filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows deleted</returns>
        public Task<int> DeleteNonDefaultsAsync<T>(T nonDefaultsFilter, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteNonDefaultsAsync(nonDefaultsFilter, token));
        }

        /// <summary>
        /// Delete 1 or more rows in a transaction using only field with non-default values in the commandFilter. E.g:
        /// <para>db.DeleteNonDefaultsAsync(new Person { FirstName = "Jimi", Age = 27 },
        /// new Person { FirstName = "Janis", Age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="nonDefaultsFilters">The non defaults filters.</param>
        /// <returns>number of rows deleted</returns>
        public Task<int> DeleteNonDefaultsAsync<T>(CancellationToken token, params T[] nonDefaultsFilters)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteNonDefaultsAsync(token, nonDefaultsFilters));
        }

        /// <summary>
        /// Deletes the non defaults asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nonDefaultsFilters">The non defaults filters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> DeleteNonDefaultsAsync<T>(params T[] nonDefaultsFilters)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteNonDefaultsAsync(CancellationToken.None, nonDefaultsFilters));
        }

        /// <summary>
        /// Delete 1 row by the PrimaryKey. E.g:
        /// <para>db.DeleteByIdAsync&lt;Person&gt;(1)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows deleted</returns>
        public Task<int> DeleteByIdAsync<T>(object id,
            Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteByIdAsync<T>(id, commandFilter, token));
        }

        /// <summary>
        /// Delete 1 row by the PrimaryKey where the rowVersion matches the optimistic concurrency field.
        /// Will throw <exception cref="OptimisticConcurrencyException">RowModifiedException</exception> if the
        /// row does not exist or has a different row version.
        /// E.g: <para>db.DeleteByIdAsync&lt;Person&gt;(1)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="rowVersion">The row version.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task DeleteByIdAsync<T>(object id, ulong rowVersion,
            Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteByIdAsync<T>(id, rowVersion, commandFilter, token));
        }

        /// <summary>
        /// Delete all rows identified by the PrimaryKeys. E.g:
        /// <para>db.DeleteByIdsAsync&lt;Person&gt;(new[] { 1, 2, 3 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValues">The identifier values.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows deleted</returns>
        public Task<int> DeleteByIdsAsync<T>(IEnumerable idValues,
            Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteByIdsAsync<T>(idValues, commandFilter, token));
        }

        /// <summary>
        /// Delete all rows in the generic table type. E.g:
        /// <para>db.DeleteAllAsync&lt;Person&gt;()</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows deleted</returns>
        public Task<int> DeleteAllAsync<T>(CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAllAsync<T>(token));
        }

        /// <summary>
        /// Delete all rows provided. E.g:
        /// <para>db.DeleteAllAsync&lt;Person&gt;(people)</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public Task<int> DeleteAllAsync<T>(IEnumerable<T> rows, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAllAsync(rows, token));
        }

        /// <summary>
        /// Delete all rows in the runtime table type. E.g:
        /// <para>db.DeleteAllAsync(typeof(Person))</para>
        /// </summary>
        /// <param name="tableType">Type of the table.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows deleted</returns>
        public Task<int> DeleteAllAsync(Type tableType, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAllAsync(tableType, token));
        }

        /// <summary>
        /// Delete rows using sqlfilter, e.g:
        /// <para>db.DeleteAsync&lt;Person&gt;("FirstName = @FirstName AND Age = @Age", new { FirstName = "Jimi", Age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> DeleteAsync<T>(string sqlFilter, object anonType, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync<T>(sqlFilter, anonType, token));
        }

        /// <summary>
        /// Delete rows using sqlfilter and Runtime Type, e.g:
        /// <para>db.DeleteAsync(typeof(Person), "FirstName = @FirstName AND Age = @Age", new { FirstName = "Jimi", Age = 27 })</para>
        /// </summary>
        /// <param name="tableType">Type of the table.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> DeleteAsync(Type tableType, string sqlFilter, object anonType, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(tableType, sqlFilter, anonType, token));
        }

        /// <summary>
        /// Insert a new row or update existing row. Returns true if a new row was inserted.
        /// Optional references param decides whether to save all related references as well. E.g:
        /// <para>db.SaveAsync(customer, references:true)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="references">if set to <c>true</c> [references].</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>true if a row was inserted; false if it was updated</returns>
        public async Task<bool> SaveAsync<T>(T obj, bool references = false, CancellationToken token = default)
        {
            if (!references)
            {
                return await dbConn.Exec(dbCmd => dbCmd.SaveAsync(obj, token)).ConfigAwait();
            }

            var trans = dbConn.OpenTransactionIfNotExists();
            return await dbConn.Exec(async dbCmd =>
            {
                using (trans)
                {
                    var ret = await dbCmd.SaveAsync(obj, token).ConfigAwait();
                    await dbCmd.SaveAllReferencesAsync(obj, token).ConfigAwait();
                    trans?.Commit();
                    return ret;
                }
            }).ConfigAwait();
        }

        /// <summary>
        /// Insert new rows or update existing rows. Return number of rows added E.g:
        /// <para>db.SaveAsync(new Person { Id = 10, FirstName = "Amy", LastName = "Winehouse", Age = 27 })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="objs">The objs.</param>
        /// <returns>number of rows added</returns>
        public Task<int> SaveAsync<T>(CancellationToken token, params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveAsync(token, objs));
        }

        /// <summary>
        /// Saves the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs">The objs.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> SaveAsync<T>(params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveAsync(CancellationToken.None, objs));
        }

        /// <summary>
        /// Insert new rows or update existing rows. Return number of rows added E.g:
        /// <para>db.SaveAllAsync(new [] { new Person { Id = 10, FirstName = "Amy", LastName = "Winehouse", Age = 27 } })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs">The objs.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>number of rows added</returns>
        public Task<int> SaveAllAsync<T>(IEnumerable<T> objs, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveAllAsync(objs, token));
        }

        /// <summary>
        /// Populates all related references on the instance with its primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveAllReferences(customer)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task SaveAllReferencesAsync<T>(T instance, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveAllReferencesAsync(instance, token));
        }

        /// <summary>
        /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveReference(customer, customer.Orders)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRef">The type of the t reference.</typeparam>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="refs">The refs.</param>
        /// <returns>Task.</returns>
        public Task SaveReferencesAsync<T, TRef>(CancellationToken token, T instance, params TRef[] refs)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveReferencesAsync(token, instance, refs));
        }

        /// <summary>
        /// Saves the references asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRef">The type of the t reference.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="refs">The refs.</param>
        /// <returns>Task.</returns>
        public Task SaveReferencesAsync<T, TRef>(T instance, params TRef[] refs)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveReferencesAsync(CancellationToken.None, instance, refs));
        }

        /// <summary>
        /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveReference(customer, customer.Orders)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRef">The type of the t reference.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="refs">The refs.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task SaveReferencesAsync<T, TRef>(T instance, List<TRef> refs, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveReferencesAsync(token, instance, refs.ToArray()));
        }

        /// <summary>
        /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveReferences(customer, customer.Orders)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRef">The type of the t reference.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="refs">The refs.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task SaveReferencesAsync<T, TRef>(T instance, IEnumerable<TRef> refs, CancellationToken token)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveReferencesAsync(token, instance, refs.ToArray()));
        }

        /// <summary>
        /// Gets the row version asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> GetRowVersionAsync<T>(object id, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetRowVersionAsync(typeof(T).GetModelDefinition(), id, token));
        }

        /// <summary>
        /// Gets the row version asynchronous.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> GetRowVersionAsync(Type modelType, object id, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetRowVersionAsync(modelType.GetModelDefinition(), id, token));
        }

        /// <summary>
        /// Executes the procedure asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task ExecuteProcedureAsync<T>(T obj, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteProcedureAsync(obj, token));
        }
    }


    // Procedures
}