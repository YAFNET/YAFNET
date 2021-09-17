// ***********************************************************************
// <copyright file="WriteExpressionCommandExtensionsLegacy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite.Legacy
{
    using System;
    using System.Data;

    using ServiceStack.Text;

    /// <summary>
    /// Class WriteExpressionCommandExtensionsLegacy.
    /// </summary>
    [Obsolete(Messages.LegacyApi)]
    internal static class WriteExpressionCommandExtensionsLegacy
    {
        /// <summary>
        /// Inserts the only.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        [Obsolete("Use db.InsertOnly(obj, db.From<T>())")]
        public static void InsertOnly<T>(this IDbCommand dbCmd, T obj, Func<SqlExpression<T>, SqlExpression<T>> onlyFields)
        {
            dbCmd.InsertOnly(obj, onlyFields(dbCmd.GetDialectProvider().SqlExpression<T>()));
        }

        /// <summary>
        /// Updates the only.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="model">The model.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <returns>System.Int32.</returns>
        [Obsolete("Use db.UpdateOnly(model, db.From<T>())")]
        public static int UpdateOnly<T>(this IDbCommand dbCmd, T model, Func<SqlExpression<T>, SqlExpression<T>> onlyFields)
        {
            return dbCmd.UpdateOnly(model, onlyFields(dbCmd.GetDialectProvider().SqlExpression<T>()));
        }

        /// <summary>
        /// Updates the FMT.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="set">The set.</param>
        /// <param name="where">The where.</param>
        /// <returns>System.Int32.</returns>
        public static int UpdateFmt<T>(this IDbCommand dbCmd, string set = null, string where = null)
        {
            return dbCmd.UpdateFmt(typeof(T).GetModelDefinition().ModelName, set, where);
        }

        /// <summary>
        /// Updates the FMT.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="table">The table.</param>
        /// <param name="set">The set.</param>
        /// <param name="where">The where.</param>
        /// <returns>System.Int32.</returns>
        public static int UpdateFmt(this IDbCommand dbCmd, string table = null, string set = null, string where = null)
        {
            var sql = UpdateFmtSql(dbCmd.GetDialectProvider(), table, set, @where);
            return dbCmd.ExecuteSql(sql);
        }

        /// <summary>
        /// Updates the FMT SQL.
        /// </summary>
        /// <param name="dialectProvider">The dialect provider.</param>
        /// <param name="table">The table.</param>
        /// <param name="set">The set.</param>
        /// <param name="where">The where.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">table</exception>
        /// <exception cref="System.ArgumentNullException">set</exception>
        internal static string UpdateFmtSql(IOrmLiteDialectProvider dialectProvider, string table, string set, string @where)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (set == null)
                throw new ArgumentNullException("set");

            var sql = StringBuilderCache.Allocate();
            sql.Append("UPDATE ");
            sql.Append(dialectProvider.GetQuotedTableName(table));
            sql.Append(" SET ");
            sql.Append(set.SqlVerifyFragment());
            if (!string.IsNullOrEmpty(@where))
            {
                sql.Append(" WHERE ");
                sql.Append(@where.SqlVerifyFragment());
            }
            return StringBuilderCache.ReturnAndFree(sql);
        }

        /// <summary>
        /// Deletes the FMT.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>System.Int32.</returns>
        internal static int DeleteFmt<T>(this IDbCommand dbCmd, string sqlFilter, params object[] filterParams)
        {
            return DeleteFmt(dbCmd, typeof(T), sqlFilter, filterParams);
        }

        /// <summary>
        /// Deletes the FMT.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>System.Int32.</returns>
        internal static int DeleteFmt(this IDbCommand dbCmd, Type tableType, string sqlFilter, params object[] filterParams)
        {
            return dbCmd.ExecuteSql(dbCmd.GetDialectProvider().ToDeleteStatement(tableType, sqlFilter, filterParams));
        }

        /// <summary>
        /// Deletes the FMT.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="where">The where.</param>
        /// <returns>System.Int32.</returns>
        public static int DeleteFmt<T>(this IDbCommand dbCmd, string where = null)
        {
            return dbCmd.DeleteFmt(typeof(T).GetModelDefinition().ModelName, where);
        }

        /// <summary>
        /// Deletes the FMT.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="table">The table.</param>
        /// <param name="where">The where.</param>
        /// <returns>System.Int32.</returns>
        public static int DeleteFmt(this IDbCommand dbCmd, string table = null, string where = null)
        {
            var sql = DeleteFmtSql(dbCmd.GetDialectProvider(), table, @where);
            return dbCmd.ExecuteSql(sql);
        }

        /// <summary>
        /// Deletes the FMT SQL.
        /// </summary>
        /// <param name="dialectProvider">The dialect provider.</param>
        /// <param name="table">The table.</param>
        /// <param name="where">The where.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">table</exception>
        /// <exception cref="System.ArgumentNullException">where</exception>
        internal static string DeleteFmtSql(IOrmLiteDialectProvider dialectProvider, string table, string @where)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (@where == null)
                throw new ArgumentNullException("where");

            var sql = StringBuilderCache.Allocate();
            sql.AppendFormat("DELETE FROM {0} WHERE {1}",
                             dialectProvider.GetQuotedTableName(table),
                             @where.SqlVerifyFragment());
            return StringBuilderCache.ReturnAndFree(sql);
        }

        /// <summary>
        /// Deletes the specified where.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="where">The where.</param>
        /// <returns>System.Int32.</returns>
        [Obsolete("Use db.Delete(db.From<T>())")]
        internal static int Delete<T>(this IDbCommand dbCmd, Func<SqlExpression<T>, SqlExpression<T>> where)
        {
            return dbCmd.Delete(where(dbCmd.GetDialectProvider().SqlExpression<T>()));
        }

        /// <summary>
        /// Inserts the only.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        [Obsolete(Messages.LegacyApi)]
        public static void InsertOnly<T>(this IDbCommand dbCmd, T obj, SqlExpression<T> onlyFields)
        {
            if (OrmLiteConfig.InsertFilter != null)
                OrmLiteConfig.InsertFilter(dbCmd, obj);

            var dialectProvider = dbCmd.GetDialectProvider();
            var sql = dialectProvider.ToInsertRowStatement(dbCmd, obj, onlyFields.InsertFields);

            dialectProvider.SetParameterValues<T>(dbCmd, obj);

            dbCmd.ExecuteSql(sql);
        }
    }
}