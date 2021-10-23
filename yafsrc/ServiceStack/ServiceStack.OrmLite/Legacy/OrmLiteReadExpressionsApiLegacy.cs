// ***********************************************************************
// <copyright file="OrmLiteReadExpressionsApiLegacy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;

namespace ServiceStack.OrmLite.Legacy
{
    using ServiceStack.Text;

    /// <summary>
    /// Class OrmLiteReadExpressionsApiLegacy.
    /// </summary>
    [Obsolete(Messages.LegacyApi)]
    public static class OrmLiteReadExpressionsApiLegacy
    {
        /// <summary>
        /// Create a new SqlExpression builder allowing typed LINQ-like queries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <returns>SqlExpression&lt;T&gt;.</returns>
        [Obsolete("Use From<T>")]
        public static SqlExpression<T> SqlExpression<T>(this IDbConnection dbConn)
        {
            return dbConn.GetExecFilter().SqlExpression<T>(dbConn);
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Select&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;T&gt;.</returns>
        [Obsolete("Use db.Select(db.From<T>())")]
        public static List<T> Select<T>(this IDbConnection dbConn, Func<SqlExpression<T>, SqlExpression<T>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select(expression));
        }

        /// <summary>
        /// Project results from a number of joined tables into a different model
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;Into&gt;.</returns>
        [Obsolete("Use db.Select<Into>(db.From<From>())")]
        public static List<Into> Select<Into, From>(this IDbConnection dbConn, SqlExpression<From> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<Into, From>(expression));
        }

        /// <summary>
        /// Project results from a number of joined tables into a different model
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;Into&gt;.</returns>
        [Obsolete("Use db.Select<Into>(db.From<T>())")]
        public static List<Into> Select<Into, From>(this IDbConnection dbConn, Func<SqlExpression<From>, SqlExpression<From>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<Into, From>(expression));
        }

        /// <summary>
        /// Returns a single result from using an SqlExpression lambda. E.g:
        /// <para>db.Single&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age == 42))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>T.</returns>
        [Obsolete("Use db.Single(db.From<T>())")]
        public static T Single<T>(this IDbConnection dbConn, Func<SqlExpression<T>, SqlExpression<T>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single(expression));
        }

        /// <summary>
        /// Returns the count of rows that match the SqlExpression lambda, E.g:
        /// <para>db.Count&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Int64.</returns>
        [Obsolete("Use db.Count(db.From<T>())")]
        public static long Count<T>(this IDbConnection dbConn, Func<SqlExpression<T>, SqlExpression<T>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Count(expression));
        }

        /// <summary>
        /// Returns results with references from using an SqlExpression lambda. E.g:
        /// <para>db.LoadSelect&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;T&gt;.</returns>
        [Obsolete("Use db.LoadSelect(db.From<T>())")]
        public static List<T> LoadSelect<T>(this IDbConnection dbConn, Func<SqlExpression<T>, SqlExpression<T>> expression, IEnumerable<string> include = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(expression, include));
        }

        /// <summary>
        /// Returns results with references from using an SqlExpression lambda. E.g:
        /// <para>db.LoadSelect&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &gt; 40), include: x =&gt; new { x.PrimaryAddress })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;T&gt;.</returns>
        [Obsolete("Use db.LoadSelect(db.From<T>())")]
        public static List<T> LoadSelect<T>(this IDbConnection dbConn, Func<SqlExpression<T>, SqlExpression<T>> expression, Func<T, object> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(expression, include(typeof(T).CreateInstance<T>()).GetType().AllAnonFields()));
        }
    }
}