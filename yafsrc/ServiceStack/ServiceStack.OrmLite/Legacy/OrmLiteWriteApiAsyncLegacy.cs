// ***********************************************************************
// <copyright file="OrmLiteWriteApiAsyncLegacy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if ASYNC
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

namespace ServiceStack.OrmLite.Legacy
{
    /// <summary>
    /// Class OrmLiteWriteApiAsyncLegacy.
    /// </summary>
    public static class OrmLiteWriteApiAsyncLegacy
    {
        /// <summary>
        /// Delete rows using a SqlFormat filter. E.g:
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>number of rows deleted</returns>
        [Obsolete(Messages.LegacyApi)]
        public static Task<int> DeleteFmtAsync<T>(this IDbConnection dbConn, CancellationToken token, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync<T>(token, sqlFilter, filterParams));
        }
        /// <summary>
        /// Deletes the FMT asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [Obsolete(Messages.LegacyApi)]
        public static Task<int> DeleteFmtAsync<T>(this IDbConnection dbConn, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync<T>(default(CancellationToken), sqlFilter, filterParams));
        }

        /// <summary>
        /// Delete rows from the runtime table type using a SqlFormat filter. E.g:
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>number of rows deleted</returns>
        /// <para>db.DeleteFmt(typeof(Person), "Age = {0}", 27)</para>
        [Obsolete(Messages.LegacyApi)]
        public static Task<int> DeleteFmtAsync(this IDbConnection dbConn, CancellationToken token, Type tableType, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync(token, tableType, sqlFilter, filterParams));
        }
        /// <summary>
        /// Deletes the FMT asynchronous.
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [Obsolete(Messages.LegacyApi)]
        public static Task<int> DeleteFmtAsync(this IDbConnection dbConn, Type tableType, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync(default(CancellationToken), tableType, sqlFilter, filterParams));
        }
    }
}

#endif