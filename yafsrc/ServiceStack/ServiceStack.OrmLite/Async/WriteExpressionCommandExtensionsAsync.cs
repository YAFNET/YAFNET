// ***********************************************************************
// <copyright file="WriteExpressionCommandExtensionsAsync.cs" company="ServiceStack, Inc.">
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

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class WriteExpressionCommandExtensionsAsync.
/// </summary>
static internal class WriteExpressionCommandExtensionsAsync
{
    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Updates the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal Task<int> UpdateOnlyFieldsAsync<T>(T model, SqlExpression<T> onlyFields, Action<IDbCommand> commandFilter, CancellationToken token)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            dbCmd.UpdateOnlySql(model, onlyFields);
            commandFilter?.Invoke(dbCmd);
            return dbCmd.ExecNonQueryAsync(token);
        }

        /// <summary>
        /// Updates the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">onlyFields</exception>
        internal Task<int> UpdateOnlyFieldsAsync<T>(T obj,
            Expression<Func<T, object>> onlyFields,
            Expression<Func<T, bool>> where,
            Action<IDbCommand> commandFilter,
            CancellationToken token)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            ArgumentNullException.ThrowIfNull(onlyFields);

            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Update(onlyFields);
            q.Where(where);
            return dbCmd.UpdateOnlyFieldsAsync(obj, q, commandFilter, token);
        }

        /// <summary>
        /// Updates the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">onlyFields</exception>
        internal Task<int> UpdateOnlyFieldsAsync<T>(T obj,
            string[] onlyFields,
            Expression<Func<T, bool>> where,
            Action<IDbCommand> commandFilter,
            CancellationToken token)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            ArgumentNullException.ThrowIfNull(onlyFields);

            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Update(onlyFields);
            q.Where(where);
            return dbCmd.UpdateOnlyFieldsAsync(obj, q, commandFilter, token);
        }

        /// <summary>
        /// Updates the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="q">The q.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal Task<int> UpdateOnlyFieldsAsync<T>(Expression<Func<T>> updateFields,
            SqlExpression<T> q,
            Action<IDbCommand> commandFilter,
            CancellationToken token)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            var cmd = dbCmd.InitUpdateOnly(updateFields, q);
            commandFilter?.Invoke(cmd);
            return cmd.ExecNonQueryAsync(token);
        }

        /// <summary>
        /// Updates the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="whereExpression">The where expression.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal Task<int> UpdateOnlyFieldsAsync<T>(Expression<Func<T>> updateFields,
            string whereExpression,
            IEnumerable<IDbDataParameter> sqlParams,
            Action<IDbCommand> commandFilter,
            CancellationToken token)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            var cmd = dbCmd.InitUpdateOnly(updateFields, whereExpression, sqlParams);
            commandFilter?.Invoke(cmd);
            return cmd.ExecNonQueryAsync(token);
        }

        /// <summary>
        /// Updates the add asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="q">The q.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> UpdateAddAsync<T>(Expression<Func<T>> updateFields,
            SqlExpression<T> q,
            Action<IDbCommand> commandFilter,
            CancellationToken token)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            var cmd = dbCmd.InitUpdateAdd(updateFields, q);
            commandFilter?.Invoke(cmd);
            return cmd.ExecNonQueryAsync(token);
        }

        /// <summary>
        /// Updates the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">updateFields</exception>
        public Task<int> UpdateOnlyFieldsAsync<T>(Dictionary<string, object> updateFields,
            Expression<Func<T, bool>> where,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            ArgumentNullException.ThrowIfNull(updateFields);

            OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, updateFields.ToFilterType<T>());

            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Where(where);
            q.PrepareUpdateStatement(dbCmd, updateFields);
            return dbCmd.UpdateAndVerifyAsync<T>(commandFilter, updateFields.ContainsKey(ModelDefinition.RowVersionName), token);
        }

        /// <summary>
        /// Updates the only asynchronous.
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
            return dbCmd.UpdateOnlyReferencesAsync<T>(updateFields, dbFields =>
            {
                var whereExpr = dbCmd.GetDialectProvider().GetUpdateOnlyWhereExpression<T>(dbFields, out var exprArgs);
                dbCmd.PrepareUpdateOnly<T>(dbFields, whereExpr, exprArgs);
                return dbCmd.UpdateAndVerifyAsync<T>(commandFilter, dbFields.ContainsKey(ModelDefinition.RowVersionName), token);
            }, token);
        }

        /// <summary>
        /// Updates the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="whereExpression">The where expression.</param>
        /// <param name="whereParams">The where parameters.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">updateFields</exception>
        public Task<int> UpdateOnlyFieldsAsync<T>(Dictionary<string, object> updateFields,
            string whereExpression,
            object[] whereParams,
            Action<IDbCommand> commandFilter = null,
            CancellationToken token = default)
        {
            return dbCmd.UpdateOnlyReferencesAsync<T>(updateFields, dbFields =>
            {
                var q = dbCmd.GetDialectProvider().SqlExpression<T>();
                q.Where(whereExpression, whereParams);
                q.PrepareUpdateStatement(dbCmd, dbFields);
                return dbCmd.UpdateAndVerifyAsync<T>(commandFilter, dbFields.ContainsKey(ModelDefinition.RowVersionName), token);
            }, token);
        }

        /// <summary>
        /// Update only references as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="fn">The function.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;System.Int32&gt; representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">updateFields</exception>
        public async Task<int> UpdateOnlyReferencesAsync<T>(Dictionary<string, object> updateFields, Func<Dictionary<string, object>, Task<int>> fn, CancellationToken token = default)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            ArgumentNullException.ThrowIfNull(updateFields);

            OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, updateFields.ToFilterType<T>());

            var dbFields = updateFields;
            var modelDef = ModelDefinition<T>.Definition;
            var hasReferences = modelDef.HasAnyReferences(updateFields.Keys);
            if (hasReferences)
            {
                dbFields = [];
                foreach (var entry in updateFields)
                {
                    if (!modelDef.IsReference(entry.Key))
                    {
                        dbFields[entry.Key] = entry.Value;
                    }
                }
            }

            var ret = await fn(dbFields).ConfigAwait();

            if (hasReferences)
            {
                var instance = updateFields.FromObjectDictionary<T>();
                await dbCmd.SaveAllReferencesAsync(instance, token).ConfigAwait();
            }
            return ret;
        }

        /// <summary>
        /// Updates the non defaults asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="obj">The object.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal Task<int> UpdateNonDefaultsAsync<T>(T item, Expression<Func<T, bool>> obj, CancellationToken token)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, item);

            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Where(obj);
            q.PrepareUpdateStatement(dbCmd, item, excludeDefaults: true);
            return dbCmd.ExecNonQueryAsync(token);
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal Task<int> UpdateAsync<T>(T item, Expression<Func<T, bool>> expression, Action<IDbCommand> commandFilter, CancellationToken token)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, item);

            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Where(expression);
            q.PrepareUpdateStatement(dbCmd, item);
            commandFilter?.Invoke(dbCmd);
            return dbCmd.ExecNonQueryAsync(token);
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateOnly">The update only.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal Task<int> UpdateAsync<T>(object updateOnly, Expression<Func<T, bool>> where, Action<IDbCommand> commandFilter, CancellationToken token)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, updateOnly.ToFilterType<T>());

            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            var whereSql = q.Where(where).WhereExpression;
            q.CopyParamsTo(dbCmd);
            dbCmd.PrepareUpdateAnonSql<T>(dbCmd.GetDialectProvider(), updateOnly, whereSql);
            commandFilter?.Invoke(dbCmd);

            return dbCmd.ExecNonQueryAsync(token);
        }

        /// <summary>
        /// Inserts the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        internal Task InsertOnlyAsync<T>(T obj, string[] onlyFields, CancellationToken token)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            OrmLiteConfig.InsertFilter?.Invoke(dbCmd, obj);

            var dialectProvider = dbCmd.GetDialectProvider();
            var sql = dialectProvider.ToInsertRowStatement(dbCmd, obj, onlyFields);

            dialectProvider.SetParameterValues<T>(dbCmd, obj);

            return dbCmd.ExecuteSqlAsync(sql, token);
        }

        /// <summary>
        /// Inserts the only asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="insertFields">The insert fields.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> InsertOnlyAsync<T>(Expression<Func<T>> insertFields, CancellationToken token)
        {
            OrmLiteUtils.AssertNotAnonType<T>();

            return dbCmd.InitInsertOnly(insertFields).ExecNonQueryAsync(token);
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal Task<int> DeleteAsync<T>(Expression<Func<T, bool>> where,
            Action<IDbCommand> commandFilter, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Where(where);
            return dbCmd.DeleteAsync(q, commandFilter, token);
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q">The q.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal Task<int> DeleteAsync<T>(SqlExpression<T> q,
            Action<IDbCommand> commandFilter, CancellationToken token)
        {
            var sql = q.ToDeleteRowStatement();
            return dbCmd.ExecuteSqlAsync(sql, q.Params, commandFilter, token);
        }

        /// <summary>
        /// Deletes the where asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereFilter">The where filter.</param>
        /// <param name="whereParams">The where parameters.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        internal Task<int> DeleteWhereAsync<T>(string whereFilter, object[] whereParams,
            Action<IDbCommand> commandFilter, CancellationToken token)
        {
            var q = dbCmd.GetDialectProvider().SqlExpression<T>();
            q.Where(whereFilter, whereParams);
            var sql = q.ToDeleteRowStatement();
            return dbCmd.ExecuteSqlAsync(sql, q.Params, commandFilter, token);
        }
    }
}