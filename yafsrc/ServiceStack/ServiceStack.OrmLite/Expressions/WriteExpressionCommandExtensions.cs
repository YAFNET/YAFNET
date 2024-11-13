// ***********************************************************************
// <copyright file="WriteExpressionCommandExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Class WriteExpressionCommandExtensions.
/// </summary>
static internal class WriteExpressionCommandExtensions
{
    /// <summary>
    /// Update only Fields.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="model">The model.</param>
    /// <param name="onlyFields">The only fields.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    public static int UpdateOnlyFields<T>(this IDbCommand dbCmd,
                                          T model,
                                          SqlExpression<T> onlyFields,
                                          Action<IDbCommand> commandFilter = null)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        UpdateOnlySql(dbCmd, model, onlyFields);
        commandFilter?.Invoke(dbCmd);
        return dbCmd.ExecNonQuery();
    }

    /// <summary>
    /// Updates the only SQL.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="model">The model.</param>
    /// <param name="onlyFields">The only fields.</param>
    static internal void UpdateOnlySql<T>(this IDbCommand dbCmd, T model, SqlExpression<T> onlyFields)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, model);

        var fieldsToUpdate = onlyFields.UpdateFields.Count == 0
                                 ? onlyFields.GetAllFields()
                                 : onlyFields.UpdateFields;

        onlyFields.CopyParamsTo(dbCmd);

        dbCmd.GetDialectProvider().PrepareUpdateRowStatement(dbCmd, model, fieldsToUpdate);

        if (!onlyFields.WhereExpression.IsNullOrEmpty())
        {
            dbCmd.CommandText += " " + onlyFields.WhereExpression;
        }
    }

    /// <summary>
    /// Updates the only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <param name="onlyFields">The only fields.</param>
    /// <param name="where">The where.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="System.ArgumentNullException">onlyFields</exception>
    static internal int UpdateOnlyFields<T>(this IDbCommand dbCmd, T obj,
                                            Expression<Func<T, object>> onlyFields = null,
                                            Expression<Func<T, bool>> where = null,
                                            Action<IDbCommand> commandFilter = null)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        ArgumentNullException.ThrowIfNull(onlyFields);

        var q = dbCmd.GetDialectProvider().SqlExpression<T>();
        q.Update(onlyFields);
        q.Where(where);
        return dbCmd.UpdateOnlyFields(obj, q, commandFilter);
    }

    /// <summary>
    /// Updates the only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <param name="onlyFields">The only fields.</param>
    /// <param name="where">The where.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="System.ArgumentNullException">onlyFields</exception>
    static internal int UpdateOnlyFields<T>(this IDbCommand dbCmd, T obj,
                                            string[] onlyFields = null,
                                            Expression<Func<T, bool>> where = null,
                                            Action<IDbCommand> commandFilter = null)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        ArgumentNullException.ThrowIfNull(onlyFields);

        var q = dbCmd.GetDialectProvider().SqlExpression<T>();
        q.Update(onlyFields);
        q.Where(where);
        return dbCmd.UpdateOnlyFields(obj, q, commandFilter);
    }

    /// <summary>
    /// Updates the only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="q">The q.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    static internal int UpdateOnlyFields<T>(this IDbCommand dbCmd,
                                            Expression<Func<T>> updateFields,
                                            SqlExpression<T> q,
                                            Action<IDbCommand> commandFilter = null)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        var cmd = dbCmd.InitUpdateOnly(updateFields, q);
        commandFilter?.Invoke(cmd);
        return cmd.ExecNonQuery();
    }

    /// <summary>
    /// Updates the only.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="q">The q.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    static internal int UpdateOnly<T>(this IDbCommand dbCmd,
                                      Expression<Func<T>> updateFields,
                                      SqlExpression<T> q,
                                      Action<IDbCommand> commandFilter = null)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        var cmd = dbCmd.InitUpdateOnly(updateFields, q);
        commandFilter?.Invoke(cmd);
        return cmd.ExecNonQuery();
    }

    /// <summary>
    /// Initializes the update only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="q">The q.</param>
    /// <returns>IDbCommand.</returns>
    /// <exception cref="System.ArgumentNullException">updateFields</exception>
    static internal IDbCommand InitUpdateOnly<T>(this IDbCommand dbCmd, Expression<Func<T>> updateFields, SqlExpression<T> q)
    {
        ArgumentNullException.ThrowIfNull(updateFields);

        OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, updateFields.EvalFactoryFn());

        q.CopyParamsTo(dbCmd);

        var updateFieldValues = updateFields.AssignedValues();
        dbCmd.GetDialectProvider().PrepareUpdateRowStatement<T>(dbCmd, updateFieldValues, q.WhereExpression);

        return dbCmd;
    }

    /// <summary>
    /// Updates the only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="whereExpression">The where expression.</param>
    /// <param name="dbParams">The database parameters.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    static internal int UpdateOnlyFields<T>(this IDbCommand dbCmd,
                                            Expression<Func<T>> updateFields,
                                            string whereExpression,
                                            IEnumerable<IDbDataParameter> dbParams,
                                            Action<IDbCommand> commandFilter = null)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        var cmd = dbCmd.InitUpdateOnly(updateFields, whereExpression, dbParams);
        commandFilter?.Invoke(cmd);
        return cmd.ExecNonQuery();
    }

    /// <summary>
    /// Initializes the update only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="whereExpression">The where expression.</param>
    /// <param name="sqlParams">The SQL parameters.</param>
    /// <returns>IDbCommand.</returns>
    /// <exception cref="System.ArgumentNullException">updateFields</exception>
    static internal IDbCommand InitUpdateOnly<T>(this IDbCommand dbCmd, Expression<Func<T>> updateFields, string whereExpression, IEnumerable<IDbDataParameter> sqlParams)
    {
        ArgumentNullException.ThrowIfNull(updateFields);

        OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, updateFields.EvalFactoryFn());

        dbCmd.SetParameters(sqlParams);

        var updateFieldValues = updateFields.AssignedValues();
        dbCmd.GetDialectProvider().PrepareUpdateRowStatement<T>(dbCmd, updateFieldValues, whereExpression);

        return dbCmd;
    }

    /// <summary>
    /// Updates the add.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="q">The q.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    public static int UpdateAdd<T>(this IDbCommand dbCmd,
                                   Expression<Func<T>> updateFields,
                                   SqlExpression<T> q,
                                   Action<IDbCommand> commandFilter)
    {
        var cmd = dbCmd.InitUpdateAdd(updateFields, q);
        commandFilter?.Invoke(cmd);
        return cmd.ExecNonQuery();
    }

    /// <summary>
    /// Initializes the update add.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="q">The q.</param>
    /// <returns>IDbCommand.</returns>
    /// <exception cref="System.ArgumentNullException">updateFields</exception>
    static internal IDbCommand InitUpdateAdd<T>(this IDbCommand dbCmd, Expression<Func<T>> updateFields, SqlExpression<T> q)
    {
        ArgumentNullException.ThrowIfNull(updateFields);

        OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, updateFields.EvalFactoryFn());

        q.CopyParamsTo(dbCmd);

        var updateFieldValues = updateFields.AssignedValues();
        dbCmd.GetDialectProvider().PrepareUpdateRowAddStatement<T>(dbCmd, updateFieldValues, q.WhereExpression);

        return dbCmd;
    }

    /// <summary>
    /// Updates the only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="where">The where.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="System.ArgumentNullException">updateFields</exception>
    public static int UpdateOnlyFields<T>(this IDbCommand dbCmd,
                                          Dictionary<string, object> updateFields,
                                          Expression<Func<T, bool>> where,
                                          Action<IDbCommand> commandFilter = null)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        ArgumentNullException.ThrowIfNull(updateFields);

        OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, updateFields.ToFilterType<T>());

        var q = dbCmd.GetDialectProvider().SqlExpression<T>();
        q.Where(where);
        q.PrepareUpdateStatement(dbCmd, updateFields);
        return dbCmd.UpdateAndVerify<T>(commandFilter, updateFields.ContainsKey(ModelDefinition.RowVersionName));
    }

    /// <summary>
    /// Gets the update only where expression.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.NotSupportedException">'{typeof(T).Name}' does not have a primary key</exception>
    static internal string GetUpdateOnlyWhereExpression<T>(this IOrmLiteDialectProvider dialectProvider,
                                                           Dictionary<string, object> updateFields, out object[] args)
    {
        var modelDef = typeof(T).GetModelDefinition();
        var pkField = modelDef.PrimaryKey;
        if (pkField == null)
        {
            throw new NotSupportedException($"'{typeof(T).Name}' does not have a primary key");
        }

        var idValue = updateFields.TryRemove(pkField.Name, out var nameValue)
                          ? nameValue
                          : pkField.Alias != null && updateFields.TryRemove(pkField.Alias, out var aliasValue)
                              ? aliasValue
                              : null;

        if (idValue == null)
        {
            var caseInsensitiveMap =
                new Dictionary<string, object>(updateFields, StringComparer.InvariantCultureIgnoreCase);
            idValue = caseInsensitiveMap.TryRemove(pkField.Name, out nameValue)
                          ? nameValue
                          : pkField.Alias != null && caseInsensitiveMap.TryRemove(pkField.Alias, out aliasValue)
                              ? aliasValue
                              : new NotSupportedException(
                                  $"UpdateOnly<{typeof(T).Name}> requires a '{pkField.Name}' Primary Key Value");
        }

        if (modelDef.RowVersion == null || !updateFields.TryGetValue(ModelDefinition.RowVersionName, out var rowVersion))
        {
            args = [idValue];
            return "(" + dialectProvider.GetQuotedColumnName(pkField.FieldName) + " = {0})";
        }

        args = [idValue, rowVersion];
        return "(" + dialectProvider.GetQuotedColumnName(pkField.FieldName) + " = {0} AND " + dialectProvider.GetRowVersionColumn(modelDef.RowVersion) + " = {1})";
    }

    /// <summary>
    /// Updates the only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    public static int UpdateOnlyFields<T>(this IDbCommand dbCmd,
                                          Dictionary<string, object> updateFields,
                                          Action<IDbCommand> commandFilter = null)
    {
        return dbCmd.UpdateOnlyReferences<T>(updateFields, dbFields => {
                var whereExpr = dbCmd.GetDialectProvider().GetUpdateOnlyWhereExpression<T>(dbFields, out var exprArgs);
                dbCmd.PrepareUpdateOnly<T>(dbFields, whereExpr, exprArgs);
                return dbCmd.UpdateAndVerify<T>(commandFilter, dbFields.ContainsKey(ModelDefinition.RowVersionName));
            });
    }

    /// <summary>
    /// Updates the only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="whereExpression">The where expression.</param>
    /// <param name="whereParams">The where parameters.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    public static int UpdateOnlyFields<T>(this IDbCommand dbCmd,
                                          Dictionary<string, object> updateFields,
                                          string whereExpression,
                                          object[] whereParams,
                                          Action<IDbCommand> commandFilter = null)
    {
        return dbCmd.UpdateOnlyReferences<T>(updateFields, dbFields => {
                dbCmd.PrepareUpdateOnly<T>(dbFields, whereExpression, whereParams);
                return dbCmd.UpdateAndVerify<T>(commandFilter, dbFields.ContainsKey(ModelDefinition.RowVersionName));
            });
    }

    /// <summary>
    /// The update only references.
    /// </summary>
    /// <typeparam name="T">#
    /// The Model</typeparam>
    /// <param name="dbCmd">The db cmd.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="fn">The fn.</param>
    /// <returns>The <see cref="int" />.</returns>
    /// <exception cref="System.ArgumentNullException">updateFields</exception>
    public static int UpdateOnlyReferences<T>(this IDbCommand dbCmd,
                                              Dictionary<string, object> updateFields, Func<Dictionary<string, object>, int> fn)
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

        var ret = fn(dbFields);

        if (hasReferences)
        {
            var instance = updateFields.FromObjectDictionary<T>();
            dbCmd.SaveAllReferences(instance);
        }
        return ret;
    }

    /// <summary>
    /// Prepares the update only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="whereExpression">The where expression.</param>
    /// <param name="whereParams">The where parameters.</param>
    /// <exception cref="System.ArgumentNullException">updateFields</exception>
    static internal void PrepareUpdateOnly<T>(this IDbCommand dbCmd, Dictionary<string, object> updateFields, string whereExpression, object[] whereParams)
    {
        ArgumentNullException.ThrowIfNull(updateFields);

        OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, updateFields.ToFilterType<T>());

        var q = dbCmd.GetDialectProvider().SqlExpression<T>();
        q.Where(whereExpression, whereParams);
        q.PrepareUpdateStatement(dbCmd, updateFields);
    }

    /// <summary>
    /// Updates the non defaults.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="item">The item.</param>
    /// <param name="where">The where.</param>
    /// <returns>System.Int32.</returns>
    public static int UpdateNonDefaults<T>(this IDbCommand dbCmd, T item, Expression<Func<T, bool>> where)
    {
        OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, item);

        var q = dbCmd.GetDialectProvider().SqlExpression<T>();
        q.Where(where);
        q.PrepareUpdateStatement(dbCmd, item, excludeDefaults: true);
        return dbCmd.ExecNonQuery();
    }

    /// <summary>
    /// Updates the specified item.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="item">The item.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    public static int Update<T>(this IDbCommand dbCmd, T item, Expression<Func<T, bool>> expression, Action<IDbCommand> commandFilter = null)
    {
        OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, item);

        var q = dbCmd.GetDialectProvider().SqlExpression<T>();
        q.Where(expression);
        q.PrepareUpdateStatement(dbCmd, item);
        commandFilter?.Invoke(dbCmd);
        return dbCmd.ExecNonQuery();
    }

    /// <summary>
    /// Updates the specified update only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="updateOnly">The update only.</param>
    /// <param name="where">The where.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    public static int Update<T>(this IDbCommand dbCmd, object updateOnly, Expression<Func<T, bool>> where = null, Action<IDbCommand> commandFilter = null)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, updateOnly.ToFilterType<T>());

        var q = dbCmd.GetDialectProvider().SqlExpression<T>();
        var whereSql = q.Where(where).WhereExpression;
        q.CopyParamsTo(dbCmd);
        var hadRowVersion = dbCmd.PrepareUpdateAnonSql<T>(dbCmd.GetDialectProvider(), updateOnly, whereSql);

        return dbCmd.UpdateAndVerify<T>(commandFilter, hadRowVersion);
    }

    /// <summary>
    /// Prepares the update anon SQL.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="updateOnly">The update only.</param>
    /// <param name="whereSql">The where SQL.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    static internal bool PrepareUpdateAnonSql<T>(this IDbCommand dbCmd, IOrmLiteDialectProvider dialectProvider, object updateOnly, string whereSql)
    {
        var sql = StringBuilderCache.Allocate();
        var modelDef = typeof(T).GetModelDefinition();
        var fields = modelDef.FieldDefinitionsArray;

        var fieldDefs = new List<FieldDefinition>();
        if (updateOnly is IDictionary d)
        {
            foreach (DictionaryEntry entry in d)
            {
                var fieldDef = modelDef.GetFieldDefinition((string)entry.Key);
                if (fieldDef == null || fieldDef.ShouldSkipUpdate())
                {
                    continue;
                }
                fieldDefs.Add(fieldDef);
            }
        }
        else
        {
            foreach (var setField in updateOnly.GetType().GetPublicProperties())
            {
                var fieldDef = fields.FirstOrDefault(x =>
                    string.Equals(x.Name, setField.Name, StringComparison.OrdinalIgnoreCase));
                if (fieldDef == null || fieldDef.ShouldSkipUpdate())
                {
                    continue;
                }
                fieldDefs.Add(fieldDef);
            }
        }

        var hadRowVersion = false;
        foreach (var fieldDef in fieldDefs)
        {
            var value = fieldDef.GetValue(updateOnly);
            if (fieldDef.IsPrimaryKey || fieldDef.AutoIncrement || fieldDef.IsRowVersion)
            {
                if (fieldDef.IsRowVersion)
                {
                    hadRowVersion = true;
                }

                whereSql += string.IsNullOrEmpty(whereSql) ? "WHERE " : " AND ";
                whereSql += $"{dialectProvider.GetQuotedColumnName(fieldDef.FieldName)} = {dialectProvider.AddQueryParam(dbCmd, value, fieldDef).ParameterName}";
                continue;
            }

            if (sql.Length > 0)
            {
                sql.Append(", ");
            }

            sql
                .Append(dialectProvider.GetQuotedColumnName(fieldDef.FieldName))
                .Append('=')
                .Append(dialectProvider.GetUpdateParam(dbCmd, value, fieldDef));
        }

        dbCmd.CommandText = $"UPDATE {dialectProvider.GetQuotedTableName(modelDef)} " +
                            $"SET {StringBuilderCache.ReturnAndFree(sql)} {whereSql}";

        return hadRowVersion;
    }

    /// <summary>
    /// Inserts the only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <param name="onlyFields">The only fields.</param>
    /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
    /// <returns>System.Int64.</returns>
    public static long InsertOnly<T>(this IDbCommand dbCmd, T obj, string[] onlyFields, bool selectIdentity)
    {
        OrmLiteConfig.InsertFilter?.Invoke(dbCmd, obj);

        var dialectProvider = dbCmd.GetDialectProvider();
        var sql = dialectProvider.ToInsertRowStatement(dbCmd, obj, onlyFields);

        dialectProvider.SetParameterValues<T>(dbCmd, obj);

        return selectIdentity ? dbCmd.ExecLongScalar(sql + dialectProvider.GetLastInsertIdSqlSuffix<T>()) : dbCmd.ExecuteSql(sql);
    }

    /// <summary>
    /// Upserts the specified model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="model">The model.</param>
    /// <param name="q">The q.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    static internal int Upsert<T>(this IDbCommand dbCmd,
        T model,
        SqlExpression<T> q,
        Action<IDbCommand> commandFilter = null)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        OrmLiteConfig.UpdateFilter?.Invoke(dbCmd, model);

        var cmd = dbCmd.InitUpsert(model, q);
        commandFilter?.Invoke(cmd);
        return cmd.ExecNonQuery();
    }

    /// <summary>
    /// Inserts the only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
    /// <returns>System.Int64.</returns>
    public static long InsertOnly<T>(this IDbCommand dbCmd, Expression<Func<T>> insertFields, bool selectIdentity)
    {
        dbCmd.InitInsertOnly(insertFields);

        return selectIdentity ? dbCmd.ExecLongScalar(dbCmd.CommandText + dbCmd.GetDialectProvider().GetLastInsertIdSqlSuffix<T>()) : dbCmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Initializes the upsert.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="model">The model.</param>
    /// <param name="q">The q.</param>
    /// <returns>IDbCommand.</returns>
    static internal IDbCommand InitUpsert<T>(this IDbCommand dbCmd, T model, SqlExpression<T> q)
    {
        OrmLiteUtils.AssertNotAnonType<T>();

        q.CopyParamsTo(dbCmd);

        dbCmd.GetDialectProvider().PrepareUpsertRowStatement(dbCmd, model, q.WhereExpression);

        return dbCmd;
    }

    /// <summary>
    /// Initializes the insert only.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>IDbCommand.</returns>
    /// <exception cref="System.ArgumentNullException">insertFields</exception>
    static internal IDbCommand InitInsertOnly<T>(this IDbCommand dbCmd, Expression<Func<T>> insertFields)
    {
        ArgumentNullException.ThrowIfNull(insertFields);

        OrmLiteConfig.InsertFilter?.Invoke(dbCmd, insertFields.EvalFactoryFn());

        var fieldValuesMap = insertFields.AssignedValues();
        dbCmd.GetDialectProvider().PrepareInsertRowStatement<T>(dbCmd, fieldValuesMap);
        return dbCmd;
    }

    /// <summary>
    /// Deletes the specified where.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="where">The where.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    public static int Delete<T>(this IDbCommand dbCmd, Expression<Func<T, bool>> where, Action<IDbCommand> commandFilter = null)
    {
        var ev = dbCmd.GetDialectProvider().SqlExpression<T>();
        ev.Where(where);
        return dbCmd.Delete(ev, commandFilter);
    }

    /// <summary>
    /// Deletes the specified where.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="where">The where.</param>
    /// <param name="commandFilter">The command filter.</param>
    /// <returns>System.Int32.</returns>
    public static int Delete<T>(this IDbCommand dbCmd, SqlExpression<T> where, Action<IDbCommand> commandFilter = null)
    {
        var sql = where.ToDeleteRowStatement();
        return dbCmd.ExecuteSql(sql, where.Params, commandFilter);
    }

    /// <summary>
    /// Deletes the where.
    /// </summary>
    /// <typeparam name="T">The Model</typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="whereFilter">The where filter.</param>
    /// <param name="whereParams">The where parameters.</param>
    /// <returns>System.Int32.</returns>
    public static int DeleteWhere<T>(this IDbCommand dbCmd, string whereFilter, object[] whereParams)
    {
        var q = dbCmd.GetDialectProvider().SqlExpression<T>();
        q.Where(whereFilter, whereParams);
        var sql = q.ToDeleteRowStatement();
        return dbCmd.ExecuteSql(sql, q.Params);
    }
}