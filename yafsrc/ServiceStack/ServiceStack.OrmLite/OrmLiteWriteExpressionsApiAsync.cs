// ***********************************************************************
// <copyright file="OrmLiteWriteExpressionsApiAsync.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteWriteExpressionsApiAsync.
/// </summary>
public static class OrmLiteWriteExpressionsApiAsync
{
    /// <param name="dbConn">The database connection.</param>
    extension(IDbConnection dbConn)
    {
        /// <summary>
        /// Use an SqlExpression to select which fields to update and construct the where expression, E.g:
        /// var q = db.From&gt;Person&lt;());
        /// db.UpdateOnlyFieldsAsync(new Person { FirstName = "JJ" }, q.Update(p =&gt; p.FirstName).Where(x =&gt; x.FirstName == "Jimi"));
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("FirstName" = 'Jimi')
        /// What's not in the update expression doesn't get updated. No where expression updates all rows. E.g:
        /// db.UpdateOnlyFieldsAsync(new Person { FirstName = "JJ", LastName = "Hendo" }, ev.Update(p =&gt; p.FirstName));
        /// UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateOnlyFieldsAsync<T>(T model,
            SqlExpression<T> onlyFields,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyFieldsAsync(model, onlyFields, commandFilter, token));
        }

        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// db.UpdateOnlyFieldsAsync(() =&gt; new Person { FirstName = "JJ" }, where: p =&gt; p.LastName == "Hendrix");
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        /// db.UpdateOnlyFieldsAsync(() =&gt; new Person { FirstName = "JJ" });
        /// UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateOnlyFieldsAsync<T>(Expression<Func<T>> updateFields,
            Expression<Func<T, bool>> where = null,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyFieldsAsync(updateFields, dbCmd.GetDialectProvider().SqlExpression<T>().Where(where), commandFilter, token));
        }

        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// db.UpdateOnlyFieldsAsync(() =&gt; new Person { FirstName = "JJ" }, db.From&lt;Person&gt;().Where(p =&gt; p.LastName == "Hendrix"));
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="q">The q.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateOnlyFieldsAsync<T>(Expression<Func<T>> updateFields,
            SqlExpression<T> q,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyFieldsAsync(updateFields, q, commandFilter, token));
        }

        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// var q = db.From&gt;Person&lt;().Where(p =&gt; p.LastName == "Hendrix");
        /// db.UpdateOnlyFieldsAsync(() =&gt; new Person { FirstName = "JJ" }, q.WhereExpression, q.Params);
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="whereExpression">The where expression.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateOnlyFieldsAsync<T>(Expression<Func<T>> updateFields,
            string whereExpression,
            IEnumerable<IDbDataParameter> sqlParams,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyFieldsAsync(updateFields, whereExpression, sqlParams, commandFilter, token));
        }

        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// db.UpdateOnlyFieldsAsync(new Person { FirstName = "JJ" }, p =&gt; p.FirstName, p =&gt; p.LastName == "Hendrix");
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        /// db.UpdateOnlyFieldsAsync(new Person { FirstName = "JJ" }, p =&gt; p.FirstName);
        /// UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateOnlyFieldsAsync<T>(T obj,
            Expression<Func<T, object>> onlyFields = null,
            Expression<Func<T, bool>> where = null,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyFieldsAsync(obj, onlyFields, where, commandFilter, token));
        }

        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// Numeric fields generates an increment sql which is useful to increment counters, etc...
        /// avoiding concurrency conflicts
        /// db.UpdateAddAsync(() =&gt; new Person { Age = 5 }, where: p =&gt; p.LastName == "Hendrix");
        /// UPDATE "Person" SET "Age" = "Age" + 5 WHERE ("LastName" = 'Hendrix')
        /// db.UpdateAddAsync(() =&gt; new Person { Age = 5 });
        /// UPDATE "Person" SET "Age" = "Age" + 5
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAddAsync<T>(Expression<Func<T>> updateFields,
            Expression<Func<T, bool>> where = null,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAddAsync(updateFields, dbCmd.GetDialectProvider().SqlExpression<T>().Where(where), commandFilter, token));
        }

        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// db.UpdateOnlyFieldsAsync(new Person { FirstName = "JJ" }, new[]{ "FirstName" }, p =&gt; p.LastName == "Hendrix");
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateOnlyFieldsAsync<T>(T obj,
            string[] onlyFields,
            Expression<Func<T, bool>> where = null,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyFieldsAsync(obj, onlyFields, where, commandFilter, token));
        }

        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// Numeric fields generates an increment sql which is useful to increment counters, etc...
        /// avoiding concurrency conflicts
        /// db.UpdateAddAsync(() =&gt; new Person { Age = 5 }, db.From&lt;Person&gt;().Where(p =&gt; p.LastName == "Hendrix"));
        /// UPDATE "Person" SET "Age" = "Age" + 5 WHERE ("LastName" = 'Hendrix')
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="q">The q.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAddAsync<T>(Expression<Func<T>> updateFields,
            SqlExpression<T> q,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAddAsync(updateFields, q, commandFilter, token));
        }

        /// <summary>
        /// Updates all values from Object Dictionary matching the where condition. E.g
        /// db.UpdateOnlyFieldsAsync&lt;Person&gt;(new Dictionary&lt;string,object&lt; { {"FirstName", "JJ"} }, where:p =&gt; p.FirstName == "Jimi");
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("FirstName" = 'Jimi')
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateOnlyFieldsAsync<T>(Dictionary<string, object> updateFields,
            Expression<Func<T, bool>> where,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyFieldsAsync(updateFields, where, commandFilter, token));
        }

        /// <summary>
        /// Updates all values from Object Dictionary, Requires Id which is used as a Primary Key Filter. E.g
        /// db.UpdateOnlyFieldsAsync&lt;Person&gt;(new Dictionary&lt;string,object&lt; { {"Id", 1}, {"FirstName", "JJ"} });
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("Id" = 1)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateOnlyFieldsAsync<T>(Dictionary<string, object> updateFields,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyFieldsAsync<T>(updateFields, commandFilter, token));
        }

        /// <summary>
        /// Updates all values from Object Dictionary matching the where condition. E.g
        /// db.UpdateOnlyFieldsAsync&lt;Person&gt;(new Dictionary&lt;string,object&lt; { {"FirstName", "JJ"} }, "FirstName == {0}", new[]{ "Jimi" });
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("FirstName" = 'Jimi')
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="whereExpression">The where expression.</param>
        /// <param name="whereParams">The where parameters.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateOnlyFieldsAsync<T>(Dictionary<string, object> updateFields,
            string whereExpression,
            object[] whereParams,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyFieldsAsync<T>(updateFields, whereExpression, whereParams, commandFilter, token));
        }

        /// <summary>
        /// Updates all non-default values set on item matching the where condition (if any). E.g
        /// db.UpdateNonDefaultsAsync(new Person { FirstName = "JJ" }, p =&gt; p.FirstName == "Jimi");
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("FirstName" = 'Jimi')
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="obj">The object.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateNonDefaultsAsync<T>(T item, Expression<Func<T, bool>> obj, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateNonDefaultsAsync(item, obj, token));
        }

        /// <summary>
        /// Updates all values set on item matching the where condition (if any). E.g
        /// db.UpdateAsync(new Person { Id = 1, FirstName = "JJ" }, p =&gt; p.LastName == "Hendrix");
        /// UPDATE "Person" SET "Id" = 1,"FirstName" = 'JJ',"LastName" = NULL,"Age" = 0 WHERE ("LastName" = 'Hendrix')
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAsync<T>(T item,
            Expression<Func<T, bool>> where,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(item, where, commandFilter, token));
        }

        /// <summary>
        /// Updates all matching fields populated on anonymousType that matches where condition (if any). E.g:
        /// db.UpdateAsync&lt;Person&gt;(new { FirstName = "JJ" }, p =&gt; p.LastName == "Hendrix");
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateOnly">The update only.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAsync<T>(object updateOnly,
            Expression<Func<T, bool>> where = null,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(updateOnly, where, commandFilter, token));
        }

        /// <summary>
        /// Using an SqlExpression to only Insert the fields specified, e.g:
        /// db.InsertOnlyAsync(new Person { FirstName = "Amy" }, p =&gt; p.FirstName));
        /// INSERT INTO "Person" ("FirstName") VALUES ('Amy');
        /// db.InsertOnlyAsync(new Person { Id =1 , FirstName="Amy" }, p =&gt; new { p.Id, p.FirstName }));
        /// INSERT INTO "Person" ("Id", "FirstName") VALUES (1, 'Amy');
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task InsertOnlyAsync<T>(T obj, Expression<Func<T, object>> onlyFields, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertOnlyAsync(obj, onlyFields.GetFieldNames(), token));
        }

        /// <summary>
        /// Using an SqlExpression to only Insert the fields specified, e.g:
        /// db.InsertOnlyAsync(new Person { FirstName = "Amy" }, new[]{ "FirstName" }));
        /// INSERT INTO "Person" ("FirstName") VALUES ('Amy');
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task InsertOnlyAsync<T>(T obj, string[] onlyFields, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertOnlyAsync(obj, onlyFields, token));
        }

        /// <summary>
        /// Using an SqlExpression to only Insert the fields specified, e.g:
        /// db.InsertOnlyAsync(() =&gt; new Person { FirstName = "Amy" }));
        /// INSERT INTO "Person" ("FirstName") VALUES (@FirstName);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="insertFields">The insert fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> InsertOnlyAsync<T>(Expression<Func<T>> insertFields, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertOnlyAsync(insertFields, token));
        }

        /// <summary>
        /// Delete the rows that matches the where expression, e.g:
        /// db.DeleteAsync&lt;Person&gt;(p =&gt; p.Age == 27);
        /// DELETE FROM "Person" WHERE ("Age" = 27)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> DeleteAsync<T>(Expression<Func<T, bool>> where,
            Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(where, commandFilter, token));
        }

        /// <summary>
        /// Delete the rows that matches the where expression, e.g:
        /// var q = db.From&gt;Person&lt;());
        /// db.DeleteAsync&lt;Person&gt;(q.Where(p =&gt; p.Age == 27));
        /// DELETE FROM "Person" WHERE ("Age" = 27)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> DeleteAsync<T>(SqlExpression<T> where
            , Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(where, commandFilter, token));
        }

        /// <summary>
        /// Delete the rows that matches the where filter, e.g:
        /// db.DeleteWhereAsync&lt;Person&gt;("Age = {0}", new object[] { 27 });
        /// DELETE FROM "Person" WHERE ("Age" = 27)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereFilter">The where filter.</param>
        /// <param name="whereParams">The where parameters.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> DeleteWhereAsync<T>(string whereFilter, object[] whereParams
            , Action<IDbCommand> commandFilter = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteWhereAsync<T>(whereFilter, whereParams, commandFilter, token));
        }
    }
}