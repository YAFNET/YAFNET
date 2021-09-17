// ***********************************************************************
// <copyright file="PgSql.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL
{
    using Npgsql;

    /// <summary>
    /// Class PgSql.
    /// </summary>
    public static class PgSql
    {
        /// <summary>
        /// Parameters the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>NpgsqlParameter.</returns>
        public static NpgsqlParameter Param<T>(string name, T value) =>
            new(name, PostgreSqlDialect.Instance.GetDbType<T>()) {
                Value = value
            };

        /// <summary>
        /// Arrays the specified items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns>System.String.</returns>
        public static string Array<T>(params T[] items) =>
            "ARRAY[" + PostgreSqlDialect.Provider.SqlSpread(items) + "]";

        /// <summary>
        /// Arrays the specified items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="nullIfEmpty">if set to <c>true</c> [null if empty].</param>
        /// <returns>System.String.</returns>
        public static string Array<T>(T[] items, bool nullIfEmpty) => 
            nullIfEmpty && (items == null || items.Length == 0)
            ? "null"
            : "ARRAY[" + PostgreSqlDialect.Provider.SqlSpread(items) + "]";
    }
}