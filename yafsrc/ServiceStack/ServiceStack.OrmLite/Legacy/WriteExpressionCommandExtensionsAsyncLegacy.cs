// ***********************************************************************
// <copyright file="WriteExpressionCommandExtensionsAsyncLegacy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if ASYNC
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Legacy
{
    /// <summary>
    /// Class WriteExpressionCommandExtensionsAsyncLegacy.
    /// </summary>
    [Obsolete(Messages.LegacyApi)]
    internal static class WriteExpressionCommandExtensionsAsyncLegacy
    {
        /// <summary>
        /// Inserts the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        [Obsolete("Use db.InsertOnlyAsync(obj, db.From<T>())")]
        internal static Task InsertOnlyAsync<T>(this IDbCommand dbCmd, T obj, Func<SqlExpression<T>, SqlExpression<T>> onlyFields, CancellationToken token)
        {
            return dbCmd.InsertOnlyAsync(obj, onlyFields(dbCmd.GetDialectProvider().SqlExpression<T>()), token);
        }

        /// <summary>
        /// Updates the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="model">The model.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [Obsolete("Use db.UpdateOnlyAsync(model, db.From<T>())")]
        internal static Task<int> UpdateOnlyAsync<T>(this IDbCommand dbCmd, T model, Func<SqlExpression<T>, SqlExpression<T>> onlyFields, CancellationToken token)
        {
            return dbCmd.UpdateOnlyAsync(model, onlyFields(dbCmd.GetDialectProvider().SqlExpression<T>()), null, token);
        }

        /// <summary>
        /// Updates the FMT asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="set">The set.</param>
        /// <param name="where">The where.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal static Task<int> UpdateFmtAsync<T>(this IDbCommand dbCmd, string set, string where, CancellationToken token)
        {
            return dbCmd.UpdateFmtAsync(typeof(T).GetModelDefinition().ModelName, set, where, token);
        }

        /// <summary>
        /// Updates the FMT asynchronous.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="table">The table.</param>
        /// <param name="set">The set.</param>
        /// <param name="where">The where.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal static Task<int> UpdateFmtAsync(this IDbCommand dbCmd, string table, string set, string where, CancellationToken token)
        {
            var sql = WriteExpressionCommandExtensionsLegacy.UpdateFmtSql(dbCmd.GetDialectProvider(), table, set, where);
            return dbCmd.ExecuteSqlAsync(sql, token);
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="where">The where.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [Obsolete("Use db.DeleteAsync(db.From<T>())")]
        internal static Task<int> DeleteAsync<T>(this IDbCommand dbCmd, Func<SqlExpression<T>, SqlExpression<T>> where, CancellationToken token)
        {
            return dbCmd.DeleteAsync(where(dbCmd.GetDialectProvider().SqlExpression<T>()), null, token);
        }

        /// <summary>
        /// Deletes the FMT asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="where">The where.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal static Task<int> DeleteFmtAsync<T>(this IDbCommand dbCmd, string where, CancellationToken token)
        {
            return dbCmd.DeleteFmtAsync(typeof(T).GetModelDefinition().ModelName, where, token);
        }

        /// <summary>
        /// Deletes the FMT asynchronous.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="table">The table.</param>
        /// <param name="where">The where.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal static Task<int> DeleteFmtAsync(this IDbCommand dbCmd, string table, string where, CancellationToken token)
        {
            var sql = WriteExpressionCommandExtensionsLegacy.DeleteFmtSql(dbCmd.GetDialectProvider(), table, where);
            return dbCmd.ExecuteSqlAsync(sql, token);
        }

        /// <summary>
        /// Inserts the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        internal static Task InsertOnlyAsync<T>(this IDbCommand dbCmd, T obj, SqlExpression<T> onlyFields, CancellationToken token)
        {
            if (OrmLiteConfig.InsertFilter != null)
                OrmLiteConfig.InsertFilter(dbCmd, obj);

            var dialectProvider = dbCmd.GetDialectProvider();
            var sql = dialectProvider.ToInsertRowStatement(dbCmd, obj, onlyFields.InsertFields);

            dialectProvider.SetParameterValues<T>(dbCmd, obj);

            return dbCmd.ExecuteSqlAsync(sql, token);
        }
    }
}

#endif