// ***********************************************************************
// <copyright file="OrmLiteWriteExpressionsApiAsyncLegacy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if ASYNC
using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Legacy
{
    /// <summary>
    /// Class OrmLiteWriteExpressionsApiAsyncLegacy.
    /// </summary>
    [Obsolete(Messages.LegacyApi)]
    public static class OrmLiteWriteExpressionsApiAsyncLegacy
    {
        /// <summary>
        /// Insert only fields in POCO specified by the SqlExpression lambda. E.g:
        /// <para>db.InsertOnly(new Person { FirstName = "Amy", Age = 27 }, q =&gt; q.Insert(p =&gt; new { p.FirstName, p.Age }))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        [Obsolete("Use db.InsertOnlyAsync(obj, db.From<T>())")]
        public static Task InsertOnlyAsync<T>(this IDbConnection dbConn, T obj, Func<SqlExpression<T>, SqlExpression<T>> onlyFields, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertOnlyAsync(obj, onlyFields, token));
        }

        /// <summary>
        /// Using an SqlExpression to only Insert the fields specified, e.g:
        /// db.InsertOnly(new Person { FirstName = "Amy" }, q =&gt; q.Insert(p =&gt; new { p.FirstName }));
        /// INSERT INTO "Person" ("FirstName") VALUES ('Amy');
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public static Task InsertOnlyAsync<T>(this IDbConnection dbConn, T obj, SqlExpression<T> onlyFields, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertOnlyAsync(obj, onlyFields, token));
        }

        /// <summary>
        /// Use an SqlExpression to select which fields to update and construct the where expression, E.g:
        /// db.UpdateOnly(new Person { FirstName = "JJ" }, ev =&gt; ev.Update(p =&gt; p.FirstName).Where(x =&gt; x.FirstName == "Jimi"));
        /// UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("FirstName" = 'Jimi')
        /// What's not in the update expression doesn't get updated. No where expression updates all rows. E.g:
        /// db.UpdateOnly(new Person { FirstName = "JJ", LastName = "Hendo" }, ev =&gt; ev.Update(p =&gt; p.FirstName));
        /// UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="model">The model.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [Obsolete("Use db.UpdateOnlyAsync(model, db.From<T>())")]
        public static Task<int> UpdateOnlyAsync<T>(this IDbConnection dbConn, T model, Func<SqlExpression<T>, SqlExpression<T>> onlyFields, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyAsync(model, onlyFields, token));
        }

        /// <summary>
        /// Flexible Update method to succinctly execute a free-text update statement using optional params. E.g:
        /// db.Update&lt;Person&gt;(set:"FirstName = {0}".Params("JJ"), where:"LastName = {0}".Params("Hendrix"));
        /// UPDATE "Person" SET FirstName = 'JJ' WHERE LastName = 'Hendrix'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="set">The set.</param>
        /// <param name="where">The where.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [Obsolete(Messages.LegacyApi)]
        public static Task<int> UpdateFmtAsync<T>(this IDbConnection dbConn, string set = null, string where = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateFmtAsync<T>(set, where, token));
        }

        /// <summary>
        /// Flexible Update method to succinctly execute a free-text update statement using optional params. E.g.
        /// db.Update(table:"Person", set: "FirstName = {0}".Params("JJ"), where: "LastName = {0}".Params("Hendrix"));
        /// UPDATE "Person" SET FirstName = 'JJ' WHERE LastName = 'Hendrix'
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="table">The table.</param>
        /// <param name="set">The set.</param>
        /// <param name="where">The where.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [Obsolete(Messages.LegacyApi)]
        public static Task<int> UpdateFmtAsync(this IDbConnection dbConn, string table = null, string set = null, string where = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateFmtAsync(table, set, where, token));
        }

        /// <summary>
        /// Flexible Delete method to succinctly execute a delete statement using free-text where expression. E.g.
        /// db.Delete&lt;Person&gt;(where:"Age = {0}".Params(27));
        /// DELETE FROM "Person" WHERE Age = 27
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="where">The where.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [Obsolete(Messages.LegacyApi)]
        public static Task<int> DeleteFmtAsync<T>(this IDbConnection dbConn, string where, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync<T>(where, token));
        }
        /// <summary>
        /// Deletes the FMT asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="where">The where.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [Obsolete(Messages.LegacyApi)]
        public static Task<int> DeleteFmtAsync<T>(this IDbConnection dbConn, string where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync<T>(where, default(CancellationToken)));
        }

        /// <summary>
        /// Flexible Delete method to succinctly execute a delete statement using free-text where expression. E.g.
        /// db.Delete(table:"Person", where: "Age = {0}".Params(27));
        /// DELETE FROM "Person" WHERE Age = 27
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="table">The table.</param>
        /// <param name="where">The where.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [Obsolete(Messages.LegacyApi)]
        public static Task<int> DeleteFmtAsync(this IDbConnection dbConn, string table = null, string where = null, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync(table, where, token));
        }

        /// <summary>
        /// Delete the rows that matches the where expression, e.g:
        /// db.Delete&lt;Person&gt;(ev =&gt; ev.Where(p =&gt; p.Age == 27));
        /// DELETE FROM "Person" WHERE ("Age" = 27)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="where">The where.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [Obsolete("Use db.DeleteAsync(db.From<T>())")]
        public static Task<int> DeleteAsync<T>(this IDbConnection dbConn, Func<SqlExpression<T>, SqlExpression<T>> where, CancellationToken token = default)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(where, token));
        }
    }
}

#endif