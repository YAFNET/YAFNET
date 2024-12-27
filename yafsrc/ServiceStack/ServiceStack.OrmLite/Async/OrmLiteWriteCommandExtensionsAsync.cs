// ***********************************************************************
// <copyright file="OrmLiteWriteCommandExtensionsAsync.cs" company="ServiceStack, Inc.">
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
using ServiceStack.Logging;
using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteWriteCommandExtensionsAsync.
/// </summary>
static internal class OrmLiteWriteCommandExtensionsAsync
{
    /// <summary>
    /// The log
    /// </summary>
    static internal ILog Log = LogManager.GetLogger(typeof(OrmLiteWriteCommandExtensionsAsync));

    /// <summary>
    /// Executes the SQL asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="sqlParams">The SQL parameters.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> ExecuteSqlAsync(this IDbCommand dbCmd, string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token)
    {
        return dbCmd.SetParameters(sqlParams).ExecuteSqlAsync(sql, (Action<IDbCommand>)null, token);
    }

    /// <summary>
    /// Executes the SQL asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="sqlParams">The SQL parameters.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> ExecuteSqlAsync(this IDbCommand dbCmd, string sql, IEnumerable<IDbDataParameter> sqlParams,
        Action<IDbCommand> commandFilter, CancellationToken token)
    {
        return dbCmd.SetParameters(sqlParams).ExecuteSqlAsync(sql, commandFilter, token);
    }

    /// <summary>
    /// Executes the SQL asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> ExecuteSqlAsync(this IDbCommand dbCmd, string sql, CancellationToken token)
    {
        return dbCmd.ExecuteSqlAsync(sql, (Action<IDbCommand>)null, token);
    }

    /// <summary>
    /// Executes the SQL asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> ExecuteSqlAsync(this IDbCommand dbCmd, string sql,
        Action<IDbCommand> commandFilter, CancellationToken token)
    {
        dbCmd.CommandText = sql;

        commandFilter?.Invoke(dbCmd);

        if (Log.IsDebugEnabled)
        {
            Log.DebugCommand(dbCmd);
        }

        OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

        if (OrmLiteConfig.ResultsFilter != null)
        {
            return OrmLiteConfig.ResultsFilter.ExecuteSql(dbCmd).InTask();
        }

        return dbCmd.GetDialectProvider().ExecuteNonQueryAsync(dbCmd, token);
    }

    /// <summary>
    /// Executes the SQL asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="anonType">Type of the anon.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> ExecuteSqlAsync(this IDbCommand dbCmd, string sql, object anonType, CancellationToken token)
    {
        return dbCmd.ExecuteSqlAsync(sql, anonType, null, token);
    }

    /// <summary>
    /// Executes the SQL asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="anonType">Type of the anon.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> ExecuteSqlAsync(this IDbCommand dbCmd, string sql, object anonType,
        Action<IDbCommand> commandFilter, CancellationToken token)
    {
        if (anonType != null)
        {
            dbCmd.SetParameters(anonType.ToObjectDictionary(), excludeDefaults: false, sql: ref sql);
        }

        dbCmd.CommandText = sql;

        commandFilter?.Invoke(dbCmd);

        if (Log.IsDebugEnabled)
        {
            Log.DebugCommand(dbCmd);
        }

        OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

        if (OrmLiteConfig.ResultsFilter != null)
        {
            return OrmLiteConfig.ResultsFilter.ExecuteSql(dbCmd).InTask();
        }

        return dbCmd.GetDialectProvider().ExecuteNonQueryAsync(dbCmd, token);
    }

    /// <summary>
    /// Updates the asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> UpdateAsync<T>(this IDbCommand dbCmd, T obj, CancellationToken token, Action<IDbCommand> commandFilter = null)
    {
        return dbCmd.UpdateInternalAsync<T>(obj, token, commandFilter);
    }

    /// <summary>
    /// Updates the asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> UpdateAsync<T>(this IDbCommand dbCmd, Dictionary<string, object> obj, CancellationToken token, Action<IDbCommand> commandFilter = null)
    {
        return dbCmd.UpdateInternalAsync<T>(obj, token, commandFilter);
    }

    /// <summary>
    /// Update internal as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>A Task&lt;System.Int32&gt; representing the asynchronous operation.</returns>
    async static internal Task<int> UpdateInternalAsync<T>(this IDbCommand dbCmd, object obj, CancellationToken token, Action<IDbCommand> commandFilter = null)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, obj.ToFilterType<T>());

        var dialectProvider = dbCmd.GetDialectProvider();
        var hadRowVersion = dialectProvider.PrepareParameterizedUpdateStatement<T>(dbCmd);
        if (string.IsNullOrEmpty(dbCmd.CommandText))
        {
            return 0;
        }

        dialectProvider.SetParameterValues<T>(dbCmd, obj);

        return await dbCmd.UpdateAndVerifyAsync<T>(commandFilter, hadRowVersion, token).ConfigAwait();
    }

    /// <summary>
    /// Update and verify as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="hadRowVersion">if set to <c>true</c> [had row version].</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int32&gt; representing the asynchronous operation.</returns>
    /// <exception cref="ServiceStack.Data.OptimisticConcurrencyException"></exception>
    async static internal Task<int> UpdateAndVerifyAsync<T>(this IDbCommand dbCmd, Action<IDbCommand> commandFilter, bool hadRowVersion, CancellationToken token)
    {
        commandFilter?.Invoke(dbCmd);
        var rowsUpdated = await dbCmd.ExecNonQueryAsync(token).ConfigAwait();

        if (hadRowVersion && rowsUpdated == 0)
        {
            throw new OptimisticConcurrencyException();
        }

        return rowsUpdated;
    }

    /// <summary>
    /// Updates the asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="objs">The objs.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> UpdateAsync<T>(this IDbCommand dbCmd, Action<IDbCommand> commandFilter, CancellationToken token, T[] objs)
    {
        return dbCmd.UpdateAllAsync(objs, commandFilter, token);
    }

    /// <summary>
    /// Update all as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="objs">The objs.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int32&gt; representing the asynchronous operation.</returns>
    /// <exception cref="ServiceStack.Data.OptimisticConcurrencyException"></exception>
    async static internal Task<int> UpdateAllAsync<T>(this IDbCommand dbCmd, IEnumerable<T> objs, Action<IDbCommand> commandFilter, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        IDbTransaction dbTrans = null;

        var count = 0;
        dbCmd.Transaction ??= dbTrans = dbCmd.Connection.BeginTransaction();

        var dialectProvider = dbCmd.GetDialectProvider();

        var hadRowVersion = dialectProvider.PrepareParameterizedUpdateStatement<T>(dbCmd);
        if (string.IsNullOrEmpty(dbCmd.CommandText))
        {
            return 0;
        }

        using (dbTrans)
        {
            foreach (var obj in objs)
            {
                OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, obj);

                dialectProvider.SetParameterValues<T>(dbCmd, obj);

                commandFilter?.Invoke(dbCmd); //filters can augment SQL & only should be invoked once
                commandFilter = null;

                var rowsUpdated = await dbCmd.ExecNonQueryAsync(token).ConfigAwait();

                if (hadRowVersion && rowsUpdated == 0)
                {
                    throw new OptimisticConcurrencyException();
                }

                count += rowsUpdated;
            }

            dbTrans?.Commit();
        }
        return count;
    }

    /// <summary>
    /// Assert rows updated as an asynchronous operation.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="hadRowVersion">if set to <c>true</c> [had row version].</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int32&gt; representing the asynchronous operation.</returns>
    /// <exception cref="ServiceStack.Data.OptimisticConcurrencyException"></exception>
    private async static Task<int> AssertRowsUpdatedAsync(IDbCommand dbCmd, bool hadRowVersion, CancellationToken token)
    {
        var rowsUpdated = await dbCmd.ExecNonQueryAsync(token).ConfigAwait();
        if (hadRowVersion && rowsUpdated == 0)
        {
            throw new OptimisticConcurrencyException();
        }

        return rowsUpdated;
    }

    /// <summary>
    /// Deletes the asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> DeleteAsync<T>(this IDbCommand dbCmd, T filter, CancellationToken token)
    {
        return dbCmd.DeleteAsync<T>((object)filter, token);
    }

    /// <summary>
    /// Deletes the asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="anonType">Type of the anon.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> DeleteAsync<T>(this IDbCommand dbCmd, object anonType, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        var dialectProvider = dbCmd.GetDialectProvider();

        var hadRowVersion = dialectProvider.PrepareParameterizedDeleteStatement<T>(
            dbCmd, anonType.AllFieldsMap<T>());

        dialectProvider.SetParameterValues<T>(dbCmd, anonType);

        return AssertRowsUpdatedAsync(dbCmd, hadRowVersion, token);
    }

    /// <summary>
    /// Deletes the non defaults asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> DeleteNonDefaultsAsync<T>(this IDbCommand dbCmd, T filter, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        var dialectProvider = dbCmd.GetDialectProvider();
        var hadRowVersion = dialectProvider.PrepareParameterizedDeleteStatement<T>(
            dbCmd, filter.AllFieldsMap<T>().NonDefaultsOnly());

        dialectProvider.SetParameterValues<T>(dbCmd, filter);

        return AssertRowsUpdatedAsync(dbCmd, hadRowVersion, token);
    }

    /// <summary>
    /// Deletes the asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="objs">The objs.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> DeleteAsync<T>(this IDbCommand dbCmd, CancellationToken token, params T[] objs)
    {
        if (objs.Length == 0)
        {
            return TaskResult.Zero;
        }

        return DeleteAllAsync(dbCmd, objs, fieldValuesFn: null, token: token);
    }

    /// <summary>
    /// Deletes the non defaults asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="filters">The filters.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> DeleteNonDefaultsAsync<T>(this IDbCommand dbCmd, CancellationToken token, params T[] filters)
    {
        if (filters.Length == 0)
        {
            return TaskResult.Zero;
        }

        return DeleteAllAsync(dbCmd, filters, o => o.AllFieldsMap<T>().NonDefaultsOnly(), token: token);
    }

    /// <summary>
    /// Delete all as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="objs">The objs.</param>
    /// <param name="fieldValuesFn">The field values function.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int32&gt; representing the asynchronous operation.</returns>
    private async static Task<int> DeleteAllAsync<T>(IDbCommand dbCmd, IEnumerable<T> objs, Func<object, Dictionary<string, object>> fieldValuesFn = null,
        Action<IDbCommand> commandFilter = null, CancellationToken token = default)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        IDbTransaction dbTrans = null;

        var count = 0;
        dbCmd.Transaction ??= dbTrans = dbCmd.Connection.BeginTransaction();

        var dialectProvider = dbCmd.GetDialectProvider();

        using (dbTrans)
        {
            foreach (var obj in objs)
            {
                var fieldValues = fieldValuesFn != null
                    ? fieldValuesFn(obj)
                    : obj.AllFieldsMap<T>();

                dialectProvider.PrepareParameterizedDeleteStatement<T>(dbCmd, fieldValues);

                dialectProvider.SetParameterValues<T>(dbCmd, obj);

                commandFilter?.Invoke(dbCmd); //filters can augment SQL & only should be invoked once
                commandFilter = null;

                var rowsAffected = await dbCmd.ExecNonQueryAsync(token).ConfigAwait();
                count += rowsAffected;
            }
            dbTrans?.Commit();
        }

        return count;
    }

    /// <summary>
    /// Deletes the by identifier asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> DeleteByIdAsync<T>(this IDbCommand dbCmd, object id,
        Action<IDbCommand> commandFilter, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        var sql = dbCmd.DeleteByIdSql<T>(id);
        return dbCmd.ExecuteSqlAsync(sql, commandFilter, token);
    }

    /// <summary>
    /// Delete by identifier as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="rowVersion">The row version.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <exception cref="ServiceStack.Data.OptimisticConcurrencyException">The row was modified or deleted since the last read</exception>
    async static internal Task DeleteByIdAsync<T>(this IDbCommand dbCmd, object id, ulong rowVersion,
        Action<IDbCommand> commandFilter, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        var sql = dbCmd.DeleteByIdSql<T>(id, rowVersion);

        var rowsAffected = await dbCmd.ExecuteSqlAsync(sql, commandFilter, token).ConfigAwait();
        if (rowsAffected == 0)
        {
            throw new OptimisticConcurrencyException("The row was modified or deleted since the last read");
        }
    }

    /// <summary>
    /// Deletes the by ids asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="idValues">The identifier values.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> DeleteByIdsAsync<T>(this IDbCommand dbCmd, IEnumerable idValues,
        Action<IDbCommand> commandFilter, CancellationToken token)
    {
        var sqlIn = dbCmd.SetIdsInSqlParams(idValues);
        if (string.IsNullOrEmpty(sqlIn))
        {
            return TaskResult.Zero;
        }

        var sql = OrmLiteWriteCommandExtensions.GetDeleteByIdsSql<T>(sqlIn, dbCmd.GetDialectProvider());

        return dbCmd.ExecuteSqlAsync(sql, commandFilter, token);
    }

    /// <summary>
    /// Deletes all asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> DeleteAllAsync<T>(this IDbCommand dbCmd, CancellationToken token)
    {
        return DeleteAllAsync(dbCmd, typeof(T), token);
    }

    /// <summary>
    /// Deletes all asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> DeleteAllAsync(this IDbCommand dbCmd, Type tableType, CancellationToken token)
    {
        var dialectProvider = dbCmd.GetDialectProvider();
        return dbCmd.ExecuteSqlAsync(dialectProvider.ToDeleteStatement(tableType, null), token);
    }

    /// <summary>
    /// Deletes the asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="anonType">Type of the anon.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> DeleteAsync<T>(this IDbCommand dbCmd, string sql, object anonType, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        if (anonType != null)
        {
            dbCmd.SetParameters<T>(anonType, excludeDefaults: false, sql: ref sql);
        }

        return dbCmd.ExecuteSqlAsync(dbCmd.GetDialectProvider().ToDeleteStatement(typeof(T), sql), token);
    }

    /// <summary>
    /// Deletes the asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="anonType">Type of the anon.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> DeleteAsync(this IDbCommand dbCmd, Type tableType, string sql, object anonType, CancellationToken token)
    {
        if (anonType != null)
        {
            dbCmd.SetParameters(tableType, anonType, excludeDefaults: false, sql: ref sql);
        }

        return dbCmd.ExecuteSqlAsync(dbCmd.GetDialectProvider().ToDeleteStatement(tableType, sql), token);
    }

    /// <summary>
    /// Insert as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
    /// <param name="enableIdentityInsert">if set to <c>true</c> [enable identity insert].</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int64&gt; representing the asynchronous operation.</returns>
    async static internal Task<long> InsertAsync<T>(this IDbCommand dbCmd, T obj, Action<IDbCommand> commandFilter, bool selectIdentity, bool enableIdentityInsert, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        OrmLiteConfig.InsertFilter?.Invoke(dbCmd, obj);

        var dialectProvider = dbCmd.GetDialectProvider();
        var pkField = ModelDefinition<T>.Definition.FieldDefinitions.Find(f => f.IsPrimaryKey);
        if (!enableIdentityInsert || pkField == null || !pkField.AutoIncrement)
        {
            dialectProvider.PrepareParameterizedInsertStatement<T>(dbCmd,
                insertFields: dialectProvider.GetNonDefaultValueInsertFields<T>(obj));
            return await InsertInternalAsync<T>(dialectProvider, dbCmd, obj, commandFilter, selectIdentity, token).ConfigAwait();
        }
        else
        {
            try
            {
                await dialectProvider.EnableIdentityInsertAsync<T>(dbCmd, token);
                dialectProvider.PrepareParameterizedInsertStatement<T>(dbCmd,
                    insertFields: dialectProvider.GetNonDefaultValueInsertFields<T>(obj),
                    shouldInclude: f => f == pkField);
                await InsertInternalAsync<T>(dialectProvider, dbCmd, obj, commandFilter, selectIdentity, token).ConfigAwait();
                if (selectIdentity)
                {
                    var id = pkField.GetValue(obj);
                    return Convert.ToInt64(id);
                }

                return 0;
            }
            finally
            {
                await dialectProvider.DisableIdentityInsertAsync<T>(dbCmd, token);
            }
        }
    }

    /// <summary>
    /// Insert as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int64&gt; representing the asynchronous operation.</returns>
    async static internal Task<long> InsertAsync<T>(this IDbCommand dbCmd, Dictionary<string, object> obj,
        Action<IDbCommand> commandFilter, bool selectIdentity, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();
        OrmLiteConfig.InsertFilter?.Invoke(dbCmd, obj.ToFilterType<T>());

        var dialectProvider = dbCmd.GetDialectProvider();
        var modelDef = ModelDefinition<T>.Definition;
        var pkField = modelDef.PrimaryKey;
        object id = null;
        var enableIdentityInsert = pkField?.AutoIncrement == true && obj.TryGetValue(pkField.Name, out id);

        try
        {
            if (enableIdentityInsert)
            {
                await dialectProvider.EnableIdentityInsertAsync<T>(dbCmd, token).ConfigAwait();
            }

            dialectProvider.PrepareParameterizedInsertStatement<T>(dbCmd,
                insertFields: dialectProvider.GetNonDefaultValueInsertFields<T>(obj),
                shouldInclude: f => obj.ContainsKey(f.Name));

            var ret = await InsertInternalAsync<T>(dialectProvider, dbCmd, obj, commandFilter, selectIdentity, token).ConfigAwait();
            if (enableIdentityInsert)
            {
                ret = Convert.ToInt64(id);
            }

            if (!modelDef.HasAnyReferences(obj.Keys))
            {
                return ret;
            }

            if (pkField != null)
            {
                obj.TryAdd(pkField.Name, ret);
            }

            var instance = obj.FromObjectDictionary<T>();
            await dbCmd.SaveAllReferencesAsync(instance, token).ConfigAwait();

            return ret;
        }
        finally
        {
            if (enableIdentityInsert)
            {
                await dialectProvider.DisableIdentityInsertAsync<T>(dbCmd, token).ConfigAwait();
            }
        }
    }

    /// <summary>
    /// Insert internal as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int64&gt; representing the asynchronous operation.</returns>
    private async static Task<long> InsertInternalAsync<T>(IOrmLiteDialectProvider dialectProvider,
        IDbCommand dbCmd, object obj, Action<IDbCommand> commandFilter, bool selectIdentity, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        dialectProvider.SetParameterValues<T>(dbCmd, obj);

        commandFilter?.Invoke(dbCmd);

        if (dialectProvider.HasInsertReturnValues(ModelDefinition<T>.Definition))
        {
            using var reader = await dbCmd.ExecReaderAsync(dbCmd.CommandText, token).ConfigAwait();
            return reader.PopulateReturnValues<T>(dialectProvider, obj);
        }

        if (selectIdentity)
        {
            dbCmd.CommandText += dialectProvider.GetLastInsertIdSqlSuffix<T>();

            return await dbCmd.ExecLongScalarAsync().ConfigAwait();
        }

        return await dbCmd.ExecNonQueryAsync(token).ConfigAwait();
    }

    /// <summary>
    /// Inserts the asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="objs">The objs.</param>
    /// <returns>Task.</returns>
    static internal Task InsertAsync<T>(this IDbCommand dbCmd, Action<IDbCommand> commandFilter, CancellationToken token, T[] objs)
    {
        return InsertAllAsync(dbCmd, objs, commandFilter, token);
    }

    /// <summary>
    /// Insert using defaults as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="objs">The objs.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    async static internal Task InsertUsingDefaultsAsync<T>(this IDbCommand dbCmd, T[] objs, CancellationToken token)
    {
        IDbTransaction dbTrans = null;

        dbCmd.Transaction ??= dbTrans = dbCmd.Connection.BeginTransaction();

        var dialectProvider = dbCmd.GetDialectProvider();

        var modelDef = typeof(T).GetModelDefinition();
        var fieldsWithoutDefaults = modelDef.FieldDefinitionsArray
            .Where(x => x.DefaultValue == null)
            .Select(x => x.Name)
            .ToSet();

        dialectProvider.PrepareParameterizedInsertStatement<T>(dbCmd, insertFields: fieldsWithoutDefaults);

        using (dbTrans)
        {
            foreach (var obj in objs)
            {
                OrmLiteConfig.InsertFilter?.Invoke(dbCmd, obj);

                dialectProvider.SetParameterValues<T>(dbCmd, obj);

                await dbCmd.ExecNonQueryAsync(token).ConfigAwait();
            }
            dbTrans?.Commit();
        }
    }

    /// <summary>
    /// Insert into select as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="query">The query.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int64&gt; representing the asynchronous operation.</returns>
    async static internal Task<long> InsertIntoSelectAsync<T>(this IDbCommand dbCmd, ISqlExpression query, Action<IDbCommand> commandFilter, CancellationToken token)
    {
        return OrmLiteReadCommandExtensions.ToLong(await dbCmd.InsertIntoSelectInternal<T>(query, commandFilter)
            .ExecNonQueryAsync(token: token).ConfigAwait());
    }

    /// <summary>
    /// Insert all as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="objs">The objs.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    async static internal Task InsertAllAsync<T>(this IDbCommand dbCmd, IEnumerable<T> objs, Action<IDbCommand> commandFilter, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        IDbTransaction dbTrans = null;

        dbCmd.Transaction ??= dbTrans = dbCmd.Connection.BeginTransaction();

        var dialectProvider = dbCmd.GetDialectProvider();

        dialectProvider.PrepareParameterizedInsertStatement<T>(dbCmd);

        using (dbTrans)
        {
            foreach (var obj in objs)
            {
                OrmLiteConfig.InsertFilter?.Invoke(dbCmd, obj);

                dialectProvider.SetParameterValues<T>(dbCmd, obj);

                commandFilter?.Invoke(dbCmd); //filters can augment SQL & only should be invoked once
                commandFilter = null;

                await dbCmd.ExecNonQueryAsync(token).ConfigAwait();
            }
            dbTrans?.Commit();
        }
    }

    /// <summary>
    /// Saves the asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="objs">The objs.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    static internal Task<int> SaveAsync<T>(this IDbCommand dbCmd, CancellationToken token, params T[] objs)
    {
        return SaveAllAsync(dbCmd, objs, token);
    }

    /// <summary>
    /// Save as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
    async static internal Task<bool> SaveAsync<T>(this IDbCommand dbCmd, T obj, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        var modelDef = typeof(T).GetModelDefinition();
        var id = modelDef.GetPrimaryKey(obj);
        var existingRow = id != null ? await dbCmd.SingleByIdAsync<T>(id, token).ConfigAwait() : default;

        if (Equals(existingRow, default(T)))
        {
            if (modelDef.HasAutoIncrementId)
            {

                var newId = await dbCmd.InsertAsync(obj, commandFilter: null, selectIdentity: true, enableIdentityInsert: false, token: token).ConfigAwait();
                var safeId = dbCmd.GetDialectProvider().FromDbValue(newId, modelDef.PrimaryKey.FieldType);
                modelDef.PrimaryKey.SetValue(obj, safeId);
                id = newId;
            }
            else
            {
                await dbCmd.InsertAsync(obj, commandFilter: null, selectIdentity: false, enableIdentityInsert: false, token: token).ConfigAwait();
            }

            modelDef.RowVersion?.SetValue(obj, await dbCmd.GetRowVersionAsync(modelDef, id, token).ConfigAwait());

            return true;
        }

        await dbCmd.UpdateAsync(obj, token, null);

        modelDef.RowVersion?.SetValue(obj, await dbCmd.GetRowVersionAsync(modelDef, id, token).ConfigAwait());

        return false;
    }

    /// <summary>
    /// Save all as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="objs">The objs.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int32&gt; representing the asynchronous operation.</returns>
    async static internal Task<int> SaveAllAsync<T>(this IDbCommand dbCmd, IEnumerable<T> objs, CancellationToken token)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        var saveRows = objs.ToList();

        var firstRow = saveRows.FirstOrDefault();
        if (Equals(firstRow, default(T)))
        {
            return 0;
        }

        var modelDef = typeof(T).GetModelDefinition();

        var firstRowId = modelDef.GetPrimaryKey(firstRow);
        var defaultIdValue = firstRowId?.GetType().GetDefaultValue();

        var idMap = defaultIdValue != null
            ? saveRows.Where(x => !defaultIdValue.Equals(modelDef.GetPrimaryKey(x))).ToSafeDictionary(x => modelDef.GetPrimaryKey(x))
            : saveRows.Where(x => modelDef.GetPrimaryKey(x) != null).ToSafeDictionary(x => modelDef.GetPrimaryKey(x));

        var existingRowsMap = (await dbCmd.SelectByIdsAsync<T>(idMap.Keys, token).ConfigAwait()).ToDictionary(x => modelDef.GetPrimaryKey(x));

        var rowsAdded = 0;

        IDbTransaction dbTrans = null;

        dbCmd.Transaction ??= dbTrans = dbCmd.Connection.BeginTransaction();

        var dialectProvider = dbCmd.GetDialectProvider();

        using (dbTrans)
        {
            foreach (var row in saveRows)
            {
                var id = modelDef.GetPrimaryKey(row);
                if (id != defaultIdValue && existingRowsMap.ContainsKey(id))
                {
                    await dbCmd.UpdateAsync(row, token, null).ConfigAwait();
                }
                else
                {
                    if (modelDef.HasAutoIncrementId)
                    {
                        var newId = await dbCmd.InsertAsync(row, commandFilter: null, selectIdentity: true, enableIdentityInsert: false, token: token).ConfigAwait();
                        var safeId = dialectProvider.FromDbValue(newId, modelDef.PrimaryKey.FieldType);
                        modelDef.PrimaryKey.SetValue(row, safeId);
                        id = newId;
                    }
                    else
                    {
                        await dbCmd.InsertAsync(row, commandFilter: null, selectIdentity: false, enableIdentityInsert: false, token: token).ConfigAwait();
                    }

                    rowsAdded++;
                }

                modelDef.RowVersion?.SetValue(row, await dbCmd.GetRowVersionAsync(modelDef, id, token).ConfigAwait());
            }

            dbTrans?.Commit();
        }

        return rowsAdded;
    }

    /// <summary>
    /// Save all references as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    async static internal Task SaveAllReferencesAsync<T>(this IDbCommand dbCmd, T instance, CancellationToken token)
    {
        await SaveAllReferences(dbCmd, ModelDefinition<T>.Definition, instance, token).ConfigAwait();
    }

    /// <summary>
    /// Saves all references.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    async static internal Task SaveAllReferences(IDbCommand dbCmd, ModelDefinition modelDef, object instance, CancellationToken token)
    {
        var pkValue = modelDef.PrimaryKey.GetValue(instance);
        var fieldDefs = modelDef.ReferenceFieldDefinitionsArray;

        var updateInstance = false;
        foreach (var fieldDef in fieldDefs)
        {
            var listInterface = fieldDef.FieldType.GetTypeWithGenericInterfaceOf(typeof(IList<>));
            if (listInterface != null)
            {
                var refType = listInterface.GetGenericArguments()[0];
                var refModelDef = refType.GetModelDefinition();

                var refField = modelDef.GetRefFieldDef(refModelDef, refType);

                var results = (IEnumerable)fieldDef.GetValue(instance);
                if (results != null)
                {
                    foreach (var oRef in results)
                    {
                        refField.SetValue(oRef, pkValue);
                    }

                    await dbCmd.CreateTypedApi(refType).SaveAllAsync(results, token).ConfigAwait();
                }
            }
            else
            {
                var refType = fieldDef.FieldType;
                var refModelDef = refType.GetModelDefinition();

                var refSelf = modelDef.GetSelfRefFieldDefIfExists(refModelDef, fieldDef);

                var result = fieldDef.GetValue(instance);
                var refField = refSelf == null
                    ? modelDef.GetRefFieldDef(refModelDef, refType)
                    : modelDef.GetRefFieldDefIfExists(refModelDef);

                if (result != null)
                {
                    refField?.SetValue(result, pkValue);

                    await dbCmd.CreateTypedApi(refType).SaveAsync(result, token).ConfigAwait();

                    //Save Self Table.RefTableId PK
                    if (refSelf != null)
                    {
                        var refPkValue = refModelDef.PrimaryKey.GetValue(result);
                        refSelf.SetValue(instance, refPkValue);
                        updateInstance = true;
                    }
                }
            }
        }

        if (updateInstance)
        {
            await dbCmd.CreateTypedApi(instance.GetType()).UpdateAsync(instance, token).ConfigAwait();
        }
    }

    /// <summary>
    /// Save references as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRef">The type of the t reference.</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="refs">The refs.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async static Task SaveReferencesAsync<T, TRef>(this IDbCommand dbCmd, CancellationToken token, T instance, params TRef[] refs)
    {
        var modelDef = ModelDefinition<T>.Definition;
        var pkValue = modelDef.PrimaryKey.GetValue(instance);

        var refType = typeof(TRef);
        var refModelDef = ModelDefinition<TRef>.Definition;

        var refSelf = modelDef.GetSelfRefFieldDefIfExists(refModelDef, null);

        foreach (var oRef in refs)
        {
            var refField = refSelf == null
                ? modelDef.GetRefFieldDef(refModelDef, refType)
                : modelDef.GetRefFieldDefIfExists(refModelDef);

            refField?.SetValue(oRef, pkValue);
        }

        await dbCmd.SaveAllAsync(refs, token).ConfigAwait();

        foreach (var oRef in refs)
        {
            //Save Self Table.RefTableId PK
            if (refSelf != null)
            {
                var refPkValue = refModelDef.PrimaryKey.GetValue(oRef);
                refSelf.SetValue(instance, refPkValue);
                await dbCmd.UpdateAsync(instance, token, null).ConfigAwait();
            }
        }
    }

    // Procedures
    /// <summary>
    /// Executes the procedure asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCommand">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    static internal Task ExecuteProcedureAsync<T>(this IDbCommand dbCommand, T obj, CancellationToken token)
    {
        var dialectProvider = dbCommand.GetDialectProvider();
        var sql = dialectProvider.ToExecuteProcedureStatement(obj);
        dbCommand.CommandType = CommandType.StoredProcedure;
        return dbCommand.ExecuteSqlAsync(sql, token);
    }

    /// <summary>
    /// Get row version as an asynchronous operation.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Object&gt; representing the asynchronous operation.</returns>
    async static internal Task<object> GetRowVersionAsync(this IDbCommand dbCmd, ModelDefinition modelDef, object id, CancellationToken token)
    {
        var sql = dbCmd.RowVersionSql(modelDef, id);
        var rowVersion = await dbCmd.ScalarAsync<object>(sql, token).ConfigAwait();
        var to = dbCmd.GetDialectProvider().FromDbRowVersion(modelDef.RowVersion.FieldType, rowVersion);

        if (to is ulong u && modelDef.RowVersion.ColumnType == typeof(byte[]))
        {
            return BitConverter.GetBytes(u);
        }

        return to ?? modelDef.RowVersion.ColumnType.GetDefaultValue();
    }
}