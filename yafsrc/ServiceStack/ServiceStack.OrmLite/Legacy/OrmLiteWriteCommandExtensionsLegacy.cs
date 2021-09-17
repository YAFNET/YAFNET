// ***********************************************************************
// <copyright file="OrmLiteWriteCommandExtensionsLegacy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;

namespace ServiceStack.OrmLite.Legacy
{
    /// <summary>
    /// Class OrmLiteWriteCommandExtensionsLegacy.
    /// </summary>
    [Obsolete(Messages.LegacyApi)]
    public static class OrmLiteWriteCommandExtensionsLegacy
    {
        /// <summary>
        /// Delete rows using a SqlFormat filter. E.g:
        /// <para>db.Delete&lt;Person&gt;("Age &gt; {0}", 42)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>number of rows deleted</returns>
        [Obsolete(Messages.LegacyApi)]
        public static int DeleteFmt<T>(this IDbConnection dbConn, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmt<T>(sqlFilter, filterParams));
        }

        /// <summary>
        /// Delete rows from the runtime table type using a SqlFormat filter. E.g:
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>number of rows deleted</returns>
        /// <para>db.DeleteFmt(typeof(Person), "Age = {0}", 27)</para>
        [Obsolete(Messages.LegacyApi)]
        public static int DeleteFmt(this IDbConnection dbConn, Type tableType, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmt(tableType, sqlFilter, filterParams));
        }
    }
}