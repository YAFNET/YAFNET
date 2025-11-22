// ***********************************************************************
// <copyright file="OrmLiteReadCommandExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using ServiceStack.Logging;

using System.Linq;

using ServiceStack.OrmLite.Base.Text;
using ServiceStack.OrmLite.Support;

namespace ServiceStack.OrmLite;

/// <summary>
/// Delegate GetValueDelegate
/// </summary>
/// <param name="i">The i.</param>
/// <returns>System.Object.</returns>
public delegate object GetValueDelegate(int i);

/// <summary>
/// Class OrmLiteReadCommandExtensions.
/// </summary>
public static class OrmLiteReadCommandExtensions
{
    /// <summary>
    /// The log
    /// </summary>
    static internal ILog Log => OrmLiteLog.Log;

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>IDataReader.</returns>
        internal IDataReader ExecReader(string sql)
        {
            dbCmd.CommandText = sql;

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            return dbCmd.WithLog(dbCmd.ExecuteReader());
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="commandBehavior">The command behavior.</param>
        /// <returns>IDataReader.</returns>
        internal IDataReader ExecReader(string sql, CommandBehavior commandBehavior)
        {
            dbCmd.CommandText = sql;

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            return dbCmd.WithLog(dbCmd.ExecuteReader(commandBehavior));
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IDataReader.</returns>
        internal IDataReader ExecReader(string sql,
            IEnumerable<IDataParameter> parameters)
        {
            dbCmd.CommandText = sql;
            dbCmd.Parameters.Clear();

            foreach (var param in parameters)
            {
                dbCmd.Parameters.Add(param);
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            return dbCmd.WithLog(dbCmd.ExecuteReader());
        }

        /// <summary>
        /// Selects the specified database command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Select<T>()
        {
            return Select<T>(dbCmd, (string)null);
        }

        /// <summary>
        /// Sets the filter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        internal void SetFilter<T>(string name, object value)
        {
            var dialectProvider = dbCmd.GetDialectProvider();

            dbCmd.Parameters.Clear();
            var p = dbCmd.CreateParameter();
            p.ParameterName = name;
            p.Direction = ParameterDirection.Input;
            dialectProvider.InitDbParam(p, value.GetType(), value);

            dbCmd.Parameters.Add(p);
            dbCmd.CommandText = GetFilterSql<T>(dbCmd);
        }

        /// <summary>
        /// Sets the filters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="excludeDefaults">if set to <c>true</c> [exclude defaults].</param>
        /// <returns>IDbCommand.</returns>
        internal IDbCommand SetFilters<T>(object anonType, bool excludeDefaults)
        {
            string ignore = null;
            dbCmd.SetParameters<T>(anonType, excludeDefaults, ref ignore); //needs to be called first
            dbCmd.CommandText = dbCmd.GetFilterSql<T>();
            return dbCmd;
        }

        /// <summary>
        /// Populates the with.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="queryType">Type of the query.</param>
        internal void PopulateWith(ISqlExpression expression, QueryType queryType)
        {
            dbCmd.CommandText = expression.ToSelectStatement(queryType); //needs to evaluate SQL before setting params
            dbCmd.SetParameters(expression.Params);
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="excludeDefaults">if set to <c>true</c> [exclude defaults].</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>IDbCommand.</returns>
        internal IDbCommand SetParameters<T>(object anonType,
            bool excludeDefaults,
            ref string sql)
        {
            return dbCmd.SetParameters(typeof(T), anonType, excludeDefaults, ref sql);
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>IDbCommand.</returns>
        internal IDbCommand SetParameters(IEnumerable<IDbDataParameter> sqlParams)
        {
            if (sqlParams == null)
            {
                return dbCmd;
            }

            try
            {
                dbCmd.Parameters.Clear();
                foreach (var sqlParam in sqlParams)
                {
                    dbCmd.Parameters.Add(sqlParam);
                }
            }
            catch (Exception ex)
            {
                //SQL Server + PostgreSql doesn't allow re-using db params in multiple queries
                if (Log.IsDebugEnabled)
                {
                    Log.Debug("Exception trying to reuse db params, executing with cloned params instead", ex);
                }

                dbCmd.Parameters.Clear();
                foreach (var sqlParam in sqlParams)
                {
                    var p = dbCmd.CreateParameter();
                    p.PopulateWith(sqlParam);
                    dbCmd.Parameters.Add(p);
                }
            }

            return dbCmd;
        }
    }

    /// <summary>
    /// Gets the multi values.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>IEnumerable.</returns>
    private static IEnumerable GetMultiValues(object value)
    {
        if (value is SqlInValues inValues)
        {
            return inValues.GetValues();
        }

        return value is IEnumerable enumerable && !(enumerable is string ||
                                                    enumerable is IEnumerable<KeyValuePair<string, object>> ||
                                                    enumerable is byte[])
                   ? enumerable
                   : null;
    }

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        /// <param name="excludeDefaults">if set to <c>true</c> [exclude defaults].</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>IDbCommand.</returns>
        internal IDbCommand SetParameters(Dictionary<string, object> dict,
            bool excludeDefaults,
            ref string sql)
        {
            if (dict == null)
            {
                return dbCmd;
            }

            dbCmd.Parameters.Clear();
            var dialectProvider = dbCmd.GetDialectProvider();

            var paramIndex = 0;
            var sqlCopy = sql; //C# doesn't allow changing ref params in lambda's

            foreach (var kvp in dict)
            {
                var value = kvp.Value;
                var propName = kvp.Key;
                if (excludeDefaults && value == null)
                {
                    continue;
                }

                var inValues = sql != null ? GetMultiValues(value) : null;
                if (inValues != null)
                {
                    var propType = value?.GetType() ?? typeof(object);
                    var sb = StringBuilderCache.Allocate();
                    foreach (var item in inValues)
                    {
                        var p = dbCmd.CreateParameter();
                        p.ParameterName = "v" + paramIndex++;

                        if (sb.Length > 0)
                        {
                            sb.Append(',');
                        }

                        sb.Append(dialectProvider.ParamString + p.ParameterName);

                        p.Direction = ParameterDirection.Input;
                        dialectProvider.InitDbParam(p, item.GetType());

                        dialectProvider.SetParamValue(p, item, item.GetType());

                        dbCmd.Parameters.Add(p);
                    }

                    var sqlIn = StringBuilderCache.ReturnAndFree(sb);
                    if (string.IsNullOrEmpty(sqlIn))
                    {
                        sqlIn = "NULL";
                    }

                    sqlCopy = sqlCopy?.Replace(dialectProvider.ParamString + propName, sqlIn);
                    if (dialectProvider.ParamString != "@")
                    {
                        sqlCopy = sqlCopy?.Replace("@" + propName, sqlIn);
                    }
                }
                else
                {
                    var p = dbCmd.CreateParameter();
                    p.ParameterName = propName;

                    p.Direction = ParameterDirection.Input;
                    p.Value = value ?? DBNull.Value;
                    if (value != null)
                    {
                        dialectProvider.InitDbParam(p, value.GetType());
                    }

                    dbCmd.Parameters.Add(p);
                }
            }

            sql = sqlCopy;

            return dbCmd;
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <param name="excludeDefaults">if set to <c>true</c> [exclude defaults].</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>IDbCommand.</returns>
        internal IDbCommand SetParameters(Type type,
            object anonType,
            bool excludeDefaults,
            ref string sql)
        {
            if (anonType.AssertAnonObject() == null)
            {
                return dbCmd;
            }

            dbCmd.Parameters.Clear();

            var modelDef = type.GetModelDefinition();
            var dialectProvider = dbCmd.GetDialectProvider();
            var fieldMap = type.IsUserType() //Ensure T != Scalar<int>()
                ? dialectProvider.GetFieldDefinitionMap(modelDef)
                : null;

            var sqlCopy = sql; //C# doesn't allow changing ref params in lambda's
            Dictionary<string, PropertyAccessor> anonTypeProps = null;

            var paramIndex = 0;
            anonType.ToObjectDictionary().ForEachParam(
                modelDef,
                excludeDefaults,
                (propName, columnName, value) =>
                {
                    var propType = value?.GetType() ??
                                   ((anonTypeProps ??= TypeProperties.Get(anonType.GetType()).PropertyMap).TryGetValue(
                                       propName,
                                       out var pType)
                                       ? pType.PropertyInfo.PropertyType
                                       : typeof(object));
                    var inValues = GetMultiValues(value);
                    if (inValues != null)
                    {
                        var sb = StringBuilderCache.Allocate();
                        foreach (var item in inValues)
                        {
                            var p = dbCmd.CreateParameter();
                            p.ParameterName = "v" + paramIndex++;

                            if (sb.Length > 0)
                            {
                                sb.Append(',');
                            }

                            sb.Append(dialectProvider.ParamString + p.ParameterName);

                            p.Direction = ParameterDirection.Input;
                            dialectProvider.InitDbParam(p, item.GetType());

                            dialectProvider.SetParamValue(p, item, item.GetType());

                            dbCmd.Parameters.Add(p);
                        }

                        var sqlIn = StringBuilderCache.ReturnAndFree(sb);
                        if (string.IsNullOrEmpty(sqlIn))
                        {
                            sqlIn = "NULL";
                        }

                        sqlCopy = sqlCopy?.Replace(dialectProvider.ParamString + propName, sqlIn);
                        if (dialectProvider.ParamString != "@")
                        {
                            sqlCopy = sqlCopy?.Replace("@" + propName, sqlIn);
                        }
                    }
                    else
                    {
                        var p = dbCmd.CreateParameter();
                        p.ParameterName = propName;
                        p.Direction = ParameterDirection.Input;
                        dialectProvider.InitDbParam(p, propType);

                        FieldDefinition fieldDef = null;
                        fieldMap?.TryGetValue(columnName, out fieldDef);

                        dialectProvider.SetParamValue(p, value, propType, fieldDef);

                        dbCmd.Parameters.Add(p);
                    }
                });

            sql = sqlCopy;
            return dbCmd;
        }
    }

    /// <summary>
    /// Sets the parameter value.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="p">The p.</param>
    /// <param name="value">The value.</param>
    /// <param name="propType">Type of the property.</param>
    /// <param name="fieldDef">The field definition.</param>
    static internal void SetParamValue(
        this IOrmLiteDialectProvider dialectProvider,
        IDbDataParameter p,
        object value,
        Type propType,
        FieldDefinition fieldDef = null)
    {
        if (fieldDef != null)
        {
            value = dialectProvider.GetFieldValue(fieldDef, value);
            var valueType = value?.GetType();
            if (valueType != null && valueType != propType)
            {
                dialectProvider.InitDbParam(p, valueType);
            }
        }
        else
        {
            value = dialectProvider.GetFieldValue(propType, value);
            var valueType = value?.GetType();
            if (valueType != null && valueType != propType)
            {
                dialectProvider.InitDbParam(p, valueType);
            }
        }

        p.Value = value == null ? DBNull.Value : p.DbType == DbType.String ? value.ToString() : value;
    }

    /// <summary>
    /// Delegate ParamIterDelegate
    /// </summary>
    /// <param name="propName">Name of the property.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="value">The value.</param>
    internal delegate void ParamIterDelegate(string propName, string columnName, object value);

    /// <summary>
    /// Fors the each parameter.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="excludeDefaults">if set to <c>true</c> [exclude defaults].</param>
    /// <param name="fn">The function.</param>
    static internal void ForEachParam(
        this Dictionary<string, object> values,
        ModelDefinition modelDef,
        bool excludeDefaults,
        ParamIterDelegate fn)
    {
        if (values == null)
        {
            return;
        }

        foreach (var kvp in values)
        {
            var value = kvp.Value;

            if (excludeDefaults && (value == null || value.Equals(value.GetType().GetDefaultValue())))
            {
                continue;
            }

            var targetField = modelDef?.FieldDefinitions.FirstOrDefault(f => string.Equals(f.Name, kvp.Key));
            var columnName = !string.IsNullOrEmpty(targetField?.Alias) ? targetField.Alias : kvp.Key;

            fn(kvp.Key, columnName, value);
        }
    }

    /// <param name="anonType">Type of the anon.</param>
    extension(object anonType)
    {
        /// <summary>
        /// Alls the fields.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>List&lt;System.String&gt;.</returns>
        internal List<string> AllFields<T>()
        {
            var ret = new List<string>();
            anonType.ToObjectDictionary().ForEachParam(
                typeof(T).GetModelDefinition(),
                excludeDefaults: false,
                fn: (propName, columnName, value) => ret.Add(propName));
            return ret;
        }

        /// <summary>
        /// Alls the fields map.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        internal Dictionary<string, object> AllFieldsMap<T>()
        {
            var ret = new Dictionary<string, object>();
            anonType.ToObjectDictionary().ForEachParam(
                typeof(T).GetModelDefinition(),
                excludeDefaults: false,
                fn: (propName, columnName, value) => ret[propName] = value);
            return ret;
        }
    }

    /// <summary>
    /// Nons the defaults only.
    /// </summary>
    /// <param name="fieldValues">The field values.</param>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    static internal Dictionary<string, object> NonDefaultsOnly(this Dictionary<string, object> fieldValues)
    {
        var map = new Dictionary<string, object>();
        foreach (var entry in fieldValues)
        {
            if (entry.Value != null)
            {
                var type = entry.Value.GetType();
                if (!type.IsValueType || !entry.Value.Equals(type.GetDefaultValue()))
                {
                    map[entry.Key] = entry.Value;
                }
            }
        }

        return map;
    }

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Sets the filters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>IDbCommand.</returns>
        public IDbCommand SetFilters<T>(object anonType)
        {
            return dbCmd.SetFilters<T>(anonType, excludeDefaults: false);
        }

        /// <summary>
        /// Clears the filters.
        /// </summary>
        public void ClearFilters()
        {
            dbCmd.Parameters.Clear();
        }

        /// <summary>
        /// Gets the filter SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.String.</returns>
        internal string GetFilterSql<T>()
        {
            var dialectProvider = dbCmd.GetDialectProvider();
            var modelDef = typeof(T).GetModelDefinition();

            var sb = StringBuilderCache.Allocate();
            foreach (IDbDataParameter p in dbCmd.Parameters)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" AND ");
                }

                var fieldName = p.ParameterName;
                var fieldDef = modelDef.GetFieldDefinition(fieldName);
                if (fieldDef != null)
                {
                    fieldName = fieldDef.FieldName;
                }

                sb.Append(fieldDef != null
                    ? dialectProvider.GetQuotedColumnName(fieldDef)
                    : dialectProvider.GetQuotedColumnName(fieldName));

                p.ParameterName = dialectProvider.SanitizeFieldNameForParamName(fieldName);

                sb.Append(" = ");
                sb.Append(dialectProvider.GetParam(p.ParameterName));
            }

            return dialectProvider.ToSelectStatement(typeof(T), StringBuilderCache.ReturnAndFree(sb));
        }

        /// <summary>
        /// Selects the by ids.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValues">The identifier values.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> SelectByIds<T>(IEnumerable idValues)
        {
            var sqlIn = dbCmd.SetIdsInSqlParams(idValues);
            return string.IsNullOrEmpty(sqlIn)
                ? []
                : Select<T>(
                    dbCmd,
                    dbCmd.GetDialectProvider().GetQuotedColumnName(ModelDefinition<T>.Definition.PrimaryKey) + " IN (" +
                    sqlIn + ")");
        }

        /// <summary>
        /// Singles the by identifier.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.ArgumentNullException">value</exception>
        internal T SingleById<T>(object value)
        {
            ArgumentNullException.ThrowIfNull(value);

            SetFilter<T>(dbCmd, ModelDefinition<T>.PrimaryKeyName, value);
            return dbCmd.ConvertTo<T>();
        }

        /// <summary>
        /// Singles the where.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        internal T SingleWhere<T>(string name, object value)
        {
            SetFilter<T>(dbCmd, name, value);
            return dbCmd.ConvertTo<T>();
        }

        /// <summary>
        /// Singles the specified anon type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>T.</returns>
        internal T Single<T>(object anonType)
        {
            dbCmd.SetFilters<T>(anonType, excludeDefaults: false);

            return dbCmd.ConvertTo<T>();
        }

        /// <summary>
        /// Singles the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>T.</returns>
        internal T Single<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            dbCmd.SetParameters(sqlParams);

            return OrmLiteUtils.IsScalar<T>()
                ? dbCmd.Scalar<T>(sql)
                : dbCmd.ConvertTo<T>(dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), sql));
        }

        /// <summary>
        /// Singles the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>T.</returns>
        internal T Single<T>(string sql, object anonType)
        {
            dbCmd.SetParameters<T>(anonType, excludeDefaults: false, sql: ref sql);

            return OrmLiteUtils.IsScalar<T>()
                ? dbCmd.Scalar<T>(sql)
                : dbCmd.ConvertTo<T>(dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), sql));
        }

        /// <summary>
        /// Wheres the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Where<T>(string name, object value)
        {
            SetFilter<T>(dbCmd, name, value);
            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// Wheres the specified anon type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Where<T>(object anonType)
        {
            dbCmd.SetFilters<T>(anonType);

            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// Selects the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Select<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            dbCmd.CommandText = dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), sql);
            if (sqlParams != null)
            {
                dbCmd.SetParameters(sqlParams);
            }

            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// Selects the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Select<T>(string sql, object anonType = null)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters<T>(anonType, excludeDefaults: false, sql: ref sql);
            }

            dbCmd.CommandText = dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), sql);

            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// Selects the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Select<T>(string sql, Dictionary<string, object> dict)
        {
            if (dict != null)
            {
                SetParameters(dbCmd, dict, (bool)false, sql: ref sql);
            }

            dbCmd.CommandText = dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), sql);

            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// Selects the specified from table type.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="fromTableType">Type of from table.</param>
        /// <returns>List&lt;TModel&gt;.</returns>
        internal List<TModel> Select<TModel>(Type fromTableType)
        {
            return Select<TModel>(dbCmd, fromTableType, null);
        }

        /// <summary>
        /// Selects the specified from table type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Select<T>(Type fromTableType, string sql, object anonType = null)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters(fromTableType, anonType, excludeDefaults: false, sql: ref sql);
            }

            dbCmd.CommandText = ToSelect<T>(dbCmd.GetDialectProvider(), fromTableType, sql);

            return dbCmd.ConvertToList<T>();
        }
    }

    //        internal static bool CanReuseParam<T>(this IDbCommand dbCmd, string paramName)
    //        {
    //            return (dbCmd.Parameters.Count == 1
    //                    && ((IDbDataParameter)dbCmd.Parameters[0]).ParameterName == paramName
    //                    && lastQueryType != typeof(T));
    //        }

    /// <summary>
    /// Converts to select.
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="fromTableType">Type of from table.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <returns>System.String.</returns>
    static internal string ToSelect<TModel>(
        IOrmLiteDialectProvider dialectProvider,
        Type fromTableType,
        string sqlFilter)
    {
        var sql = StringBuilderCache.Allocate();
        var modelDef = ModelDefinition<TModel>.Definition;
        sql.Append(
            $"SELECT {dialectProvider.GetColumnNames(modelDef)} " +
            $"FROM {dialectProvider.GetQuotedTableName(fromTableType.GetModelDefinition())}");

        if (string.IsNullOrEmpty(sqlFilter))
        {
            return StringBuilderCache.ReturnAndFree(sql);
        }

        sql.Append(" WHERE ");
        sql.Append(sqlFilter);
        return StringBuilderCache.ReturnAndFree(sql);
    }

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// SQLs the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> SqlList<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            dbCmd.CommandText = sql;

            return dbCmd.SetParameters(sqlParams).ConvertToList<T>();
        }

        /// <summary>
        /// SQLs the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> SqlList<T>(string sql, object anonType = null)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters<T>(anonType, excludeDefaults: false, sql: ref sql);
            }

            dbCmd.CommandText = sql;

            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// SQLs the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> SqlList<T>(string sql, Dictionary<string, object> dict)
        {
            if (dict != null)
            {
                SetParameters(dbCmd, dict, false, sql: ref sql);
            }

            dbCmd.CommandText = sql;

            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// SQLs the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dbCmdFilter">The database command filter.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> SqlList<T>(string sql, Action<IDbCommand> dbCmdFilter)
        {
            dbCmdFilter?.Invoke(dbCmd);
            dbCmd.CommandText = sql;

            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// SQLs the column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> SqlColumn<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            dbCmd.SetParameters(sqlParams).CommandText = sql;
            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// SQLs the column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> SqlColumn<T>(string sql, object anonType = null)
        {
            dbCmd.SetParameters<T>(anonType, excludeDefaults: false, sql: ref sql).CommandText = sql;
            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// SQLs the column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> SqlColumn<T>(string sql, Dictionary<string, object> dict)
        {
            if (dict != null)
            {
                SetParameters(dbCmd, dict, false, sql: ref sql);
            }

            dbCmd.CommandText = sql;

            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// SQLs the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>T.</returns>
        internal T SqlScalar<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbCmd.SetParameters(sqlParams).Scalar<T>(sql);
        }

        /// <summary>
        /// SQLs the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>T.</returns>
        internal T SqlScalar<T>(string sql, object anonType = null)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters<T>(anonType, excludeDefaults: false, sql: ref sql);
            }

            return dbCmd.Scalar<T>(sql);
        }

        /// <summary>
        /// SQLs the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <returns>T.</returns>
        internal T SqlScalar<T>(string sql, Dictionary<string, object> dict)
        {
            if (dict != null)
            {
                SetParameters(dbCmd, dict, false, sql: ref sql);
            }

            return dbCmd.Scalar<T>(sql);
        }

        /// <summary>
        /// Selects the non defaults.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> SelectNonDefaults<T>(object filter)
        {
            dbCmd.SetFilters<T>(filter, excludeDefaults: true);

            return dbCmd.ConvertToList<T>();
        }

        /// <summary>
        /// Selects the non defaults.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> SelectNonDefaults<T>(string sql, object anonType = null)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters<T>(anonType, excludeDefaults: true, sql: ref sql);
            }

            return dbCmd.ConvertToList<T>(dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), sql));
        }

        /// <summary>
        /// Selects the lazy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        internal IEnumerable<T> SelectLazy<T>(string sql,
            IEnumerable<IDbDataParameter> sqlParams)
        {
            foreach (var p in dbCmd.SetParameters(sqlParams).SelectLazy<T>(sql))
            {
                yield return p;
            }
        }

        /// <summary>
        /// Selects the lazy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        internal IEnumerable<T> SelectLazy<T>(string sql, object anonType = null)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters<T>(anonType, excludeDefaults: false, sql: ref sql);
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            dbCmd.CommandText = dialectProvider.ToSelectStatement(typeof(T), sql);

            var resultsFilter = OrmLiteConfig.ResultsFilter;
            if (resultsFilter != null)
            {
                foreach (var item in resultsFilter.GetList<T>(dbCmd))
                {
                    yield return item;
                }

                yield break;
            }

            using var reader = dbCmd.ExecuteReader();
            var indexCache = reader.GetIndexFieldsCache(ModelDefinition<T>.Definition, dialectProvider);
            var values = new object[reader.FieldCount];
            while (reader.Read())
            {
                var row = OrmLiteUtils.CreateInstance<T>();
                row.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
                yield return row;
            }
        }

        /// <summary>
        /// Columns the lazy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        internal IEnumerable<T> ColumnLazy<T>(string sql,
            IEnumerable<IDbDataParameter> sqlParams)
        {
            foreach (var p in dbCmd.SetParameters(sqlParams).ColumnLazy<T>(sql))
            {
                yield return p;
            }
        }

        /// <summary>
        /// Columns the lazy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        internal IEnumerable<T> ColumnLazy<T>(string sql, object anonType)
        {
            foreach (var p in dbCmd.SetParameters<T>(anonType, excludeDefaults: false, sql: ref sql).ColumnLazy<T>(sql))
            {
                yield return p;
            }
        }

        /// <summary>
        /// Columns the lazy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        private IEnumerable<T> ColumnLazy<T>(string sql)
        {
            var dialectProvider = dbCmd.GetDialectProvider();
            dbCmd.CommandText = dialectProvider.ToSelectStatement(typeof(T), sql);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                foreach (var item in OrmLiteConfig.ResultsFilter.GetColumn<T>(dbCmd))
                {
                    yield return item;
                }

                yield break;
            }

            using var reader = dbCmd.ExecuteReader();
            while (reader.Read())
            {
                var value = dialectProvider.FromDbValue(reader, 0, typeof(T));
                if (value == DBNull.Value)
                {
                    yield return default;
                }
                else
                {
                    yield return (T)value;
                }
            }
        }

        /// <summary>
        /// Wheres the lazy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        internal IEnumerable<T> WhereLazy<T>(object anonType)
        {
            dbCmd.SetFilters<T>(anonType);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                foreach (var item in OrmLiteConfig.ResultsFilter.GetList<T>(dbCmd))
                {
                    yield return item;
                }

                yield break;
            }

            var dialectProvider = dbCmd.GetDialectProvider();
            using var reader = dbCmd.ExecuteReader();
            var indexCache = reader.GetIndexFieldsCache(ModelDefinition<T>.Definition, dialectProvider);
            var values = new object[reader.FieldCount];
            while (reader.Read())
            {
                var row = OrmLiteUtils.CreateInstance<T>();
                row.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
                yield return row;
            }
        }

        /// <summary>
        /// Selects the lazy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        internal IEnumerable<T> SelectLazy<T>()
        {
            return SelectLazy<T>(dbCmd, null);
        }

        /// <summary>
        /// Scalars the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>T.</returns>
        internal T Scalar<T>(string sql, object anonType = null)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters<T>(anonType, excludeDefaults: false, sql: ref sql);
            }

            return dbCmd.Scalar<T>(sql);
        }
    }

    /// <summary>
    /// Scalars the specified dialect provider.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>T.</returns>
    static internal T Scalar<T>(this IDataReader reader, IOrmLiteDialectProvider dialectProvider)
    {
        while (reader.Read())
        {
            return ToScalar<T>(dialectProvider, reader);
        }

        return default;
    }

    /// <summary>
    /// Converts to scalar.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="reader">The reader.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <returns>T.</returns>
    static internal T ToScalar<T>(IOrmLiteDialectProvider dialectProvider, IDataReader reader, int columnIndex = 0)
    {
        var nullableType = Nullable.GetUnderlyingType(typeof(T));
        if (nullableType != null)
        {
            var oValue = reader.GetValue(columnIndex);
            if (oValue == DBNull.Value)
            {
                return default;
            }
        }

        var underlyingType = nullableType ?? typeof(T);
        if (underlyingType == typeof(object))
        {
            return (T)reader.GetValue(0);
        }

        var converter = dialectProvider.GetConverterBestMatch(underlyingType);
        if (converter != null)
        {
            var oValue = converter.GetValue(reader, columnIndex, null);
            if (oValue == null)
            {
                return default;
            }

            var convertedValue = converter.FromDbValue(underlyingType, oValue);
            return convertedValue == null ? default : (T)convertedValue;
        }

        return (T)reader.GetValue(0);
    }

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Lasts the insert identifier.
        /// </summary>
        /// <returns>System.Int64.</returns>
        internal long LastInsertId()
        {
            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetLastInsertId(dbCmd);
            }

            return dbCmd.GetDialectProvider().GetLastInsertId(dbCmd);
        }

        /// <summary>
        /// Columns the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Column<T>(string sql, object anonType = null)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters<T>(anonType, excludeDefaults: false, sql: ref sql);
            }

            return dbCmd.Column<T>(dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), sql));
        }
    }

    /// <summary>
    /// Columns the specified dialect provider.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>List&lt;T&gt;.</returns>
    static internal List<T> Column<T>(this IDataReader reader, IOrmLiteDialectProvider dialectProvider)
    {
        var columValues = new List<T>();

        while (reader.Read())
        {
            var value = dialectProvider.FromDbValue(reader, 0, typeof(T));

            if (value == DBNull.Value || value == null)
            {
                value = default(T);
            }

            columValues.Add((T)value);
        }

        return columValues;
    }

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Columns the distinct.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        internal HashSet<T> ColumnDistinct<T>(string sql,
            IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbCmd.SetParameters(sqlParams).ColumnDistinct<T>(sql);
        }

        /// <summary>
        /// Columns the distinct.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        internal HashSet<T> ColumnDistinct<T>(string sql, object anonType = null)
        {
            return dbCmd.SetParameters<T>(anonType, excludeDefaults: false, sql: ref sql).ColumnDistinct<T>(sql);
        }
    }

    /// <summary>
    /// Columns the distinct.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>HashSet&lt;T&gt;.</returns>
    static internal HashSet<T> ColumnDistinct<T>(this IDataReader reader, IOrmLiteDialectProvider dialectProvider)
    {
        var columValues = new HashSet<T>();
        while (reader.Read())
        {
            var value = dialectProvider.FromDbValue(reader, 0, typeof(T));
            if (value == DBNull.Value)
            {
                value = default(T);
            }

            columValues.Add((T)value);
        }

        return columValues;
    }

    /// <summary>
    /// Lookups the specified SQL.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="anonType">Type of the anon.</param>
    /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
    public static Dictionary<K, List<V>> Lookup<K, V>(this IDbCommand dbCmd, string sql, object anonType = null)
    {
        return dbCmd.SetParameters(anonType.ToObjectDictionary(), false, sql: ref sql).Lookup<K, V>(sql);
    }

    /// <summary>
    /// Lookups the specified dialect provider.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
    static internal Dictionary<K, List<V>> Lookup<K, V>(
        this IDataReader reader,
        IOrmLiteDialectProvider dialectProvider)
    {
        var lookup = new Dictionary<K, List<V>>();

        while (reader.Read())
        {
            var key = (K)dialectProvider.FromDbValue(reader, 0, typeof(K));
            var value = (V)dialectProvider.FromDbValue(reader, 1, typeof(V));

            if (!lookup.TryGetValue(key, out var values))
            {
                values = [];
                lookup[key] = values;
            }

            values.Add(value);
        }

        return lookup;
    }

    /// <summary>
    /// Dictionaries the specified SQL.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="anonType">Type of the anon.</param>
    /// <returns>Dictionary&lt;K, V&gt;.</returns>
    static internal Dictionary<K, V> Dictionary<K, V>(this IDbCommand dbCmd, string sql, object anonType = null)
    {
        if (anonType != null)
        {
            SetParameters(dbCmd, anonType.ToObjectDictionary(), excludeDefaults: false, sql: ref sql);
        }

        return dbCmd.Dictionary<K, V>(sql);
    }

    /// <summary>
    /// Dictionaries the specified dialect provider.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>Dictionary&lt;K, V&gt;.</returns>
    static internal Dictionary<K, V> Dictionary<K, V>(
        this IDataReader reader,
        IOrmLiteDialectProvider dialectProvider)
    {
        var map = new Dictionary<K, V>();

        while (reader.Read())
        {
            var key = (K)dialectProvider.FromDbValue(reader, 0, typeof(K));
            var value = (V)dialectProvider.FromDbValue(reader, 1, typeof(V));

            map.Add(key, value);
        }

        return map;
    }

    /// <summary>
    /// Keys the value pairs.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="anonType">Type of the anon.</param>
    /// <returns>List&lt;KeyValuePair&lt;K, V&gt;&gt;.</returns>
    static internal List<KeyValuePair<K, V>> KeyValuePairs<K, V>(
        this IDbCommand dbCmd,
        string sql,
        object anonType = null)
    {
        if (anonType != null)
        {
            SetParameters(dbCmd, anonType.ToObjectDictionary(), excludeDefaults: false, sql: ref sql);
        }

        return dbCmd.KeyValuePairs<K, V>(sql);
    }

    /// <summary>
    /// Keys the value pairs.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>List&lt;KeyValuePair&lt;K, V&gt;&gt;.</returns>
    static internal List<KeyValuePair<K, V>> KeyValuePairs<K, V>(
        this IDataReader reader,
        IOrmLiteDialectProvider dialectProvider)
    {
        var to = new List<KeyValuePair<K, V>>();

        while (reader.Read())
        {
            var key = (K)dialectProvider.FromDbValue(reader, 0, typeof(K));
            var value = (V)dialectProvider.FromDbValue(reader, 1, typeof(V));

            to.Add(new KeyValuePair<K, V>(key, value));
        }

        return to;
    }

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Existses the specified anon type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool Exists<T>(object anonType)
        {
            string sql = null;
            if (anonType != null)
            {
                SetParameters(dbCmd, anonType.ToObjectDictionary(), excludeDefaults: true, sql: ref sql);
            }

            sql = GetFilterSql<T>(dbCmd);

            var result = dbCmd.Scalar(sql);
            return result != null;
        }

        /// <summary>
        /// Existses the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool Exists<T>(string sql, object anonType = null)
        {
            if (anonType != null)
            {
                SetParameters(dbCmd, anonType.ToObjectDictionary(), (bool)false, sql: ref sql);
            }

            var result = dbCmd.Scalar(dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), sql));
            return result != null;
        }

        internal bool ExistsById<T>(object value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var modelDef = ModelDefinition<T>.Definition;
            var pkName = ModelDefinition<T>.PrimaryKeyName;
            var dialect = dbCmd.GetDialectProvider();
            var result = dbCmd.SqlScalar<int>(
                "SELECT 1 FROM " + dialect.GetQuotedTableName(modelDef) +
                " WHERE " + dialect.GetQuotedColumnName(modelDef.PrimaryKey) + " = " + dialect.GetParam(pkName),
                new Dictionary<string, object>
                {
                    [pkName] = value
                });
            return result == 1;
        }

        /// <summary>
        /// SQLs the procedure.
        /// </summary>
        /// <typeparam name="TOutputModel">The type of the t output model.</typeparam>
        /// <param name="fromObjWithProperties">From object with properties.</param>
        /// <returns>List&lt;TOutputModel&gt;.</returns>
        internal List<TOutputModel> SqlProcedure<TOutputModel>(object fromObjWithProperties)
        {
            return SqlProcedureFmt<TOutputModel>(dbCmd, fromObjWithProperties, string.Empty);
        }

        /// <summary>
        /// SQLs the procedure FMT.
        /// </summary>
        /// <typeparam name="TOutputModel">The type of the t output model.</typeparam>
        /// <param name="fromObjWithProperties">From object with properties.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>List&lt;TOutputModel&gt;.</returns>
        internal List<TOutputModel> SqlProcedureFmt<TOutputModel>(object fromObjWithProperties,
            string sqlFilter,
            params object[] filterParams)
        {
            var modelType = typeof(TOutputModel);

            var sql = dbCmd.GetDialectProvider().ToSelectFromProcedureStatement(
                fromObjWithProperties,
                modelType,
                sqlFilter,
                filterParams);

            return dbCmd.ConvertToList<TOutputModel>(sql);
        }

        /// <summary>
        /// Longs the scalar.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public long LongScalar()
        {
            var result = dbCmd.ExecuteScalar();
            return ToLong(result);
        }
    }

    // procedures ...

    /// <summary>
    /// Converts to long.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>System.Int64.</returns>
    static internal long ToLong(int result)
    {
        return result;
    }

    /// <summary>
    /// Converts to long.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>System.Int64.</returns>
    static internal long ToLong(object result)
    {
        return result switch {
            DBNull => 0,
            int i => i,
            decimal result1 => Convert.ToInt64(result1),
            ulong => (long)Convert.ToUInt64(result),
            _ => (long)result
        };
    }

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Loads the single by identifier.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="include">The include.</param>
        /// <returns>T.</returns>
        internal T LoadSingleById<T>(object value, string[] include = null)
        {
            var row = dbCmd.SingleById<T>(value);
            if (row == null)
            {
                return default;
            }

            dbCmd.LoadReferences(row, include);

            return row;
        }

        /// <summary>
        /// Loads the references.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="include">The include.</param>
        public void LoadReferences<T>(T instance, IEnumerable<string> include = null)
        {
            var loadRef = new LoadReferencesSync<T>(dbCmd, instance);
            var fieldDefs = loadRef.FieldDefs;

            var includeSet = include != null ? new HashSet<string>(include, StringComparer.OrdinalIgnoreCase) : null;

            foreach (var fieldDef in fieldDefs)
            {
                if (includeSet != null && !includeSet.Contains(fieldDef.Name))
                {
                    continue;
                }

                dbCmd.Parameters.Clear();
                var listInterface = fieldDef.FieldType.GetTypeWithGenericInterfaceOf(typeof(IList<>));
                if (listInterface != null)
                {
                    loadRef.SetRefFieldList(fieldDef, listInterface.GetGenericArguments()[0]);
                }
                else if (fieldDef.FieldReference != null)
                {
                    loadRef.SetFieldReference(fieldDef, fieldDef.FieldReference);
                }
                else
                {
                    loadRef.SetRefField(fieldDef, fieldDef.FieldType);
                }
            }
        }

        /// <summary>
        /// Loads the list with references.
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="expr">The expr.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;Into&gt;.</returns>
        internal List<Into> LoadListWithReferences<Into, From>(SqlExpression<From> expr = null,
            IEnumerable<string> include = null)
        {
            var loadList = new LoadListSync<Into, From>(dbCmd, expr);
            var fieldDefs = loadList.FieldDefs;

            var includeSet = include != null ? new HashSet<string>(include, StringComparer.OrdinalIgnoreCase) : null;

            foreach (var fieldDef in fieldDefs)
            {
                if (includeSet != null && !includeSet.Contains(fieldDef.Name))
                {
                    continue;
                }

                var listInterface = fieldDef.FieldType.GetTypeWithGenericInterfaceOf(typeof(IList<>));
                if (listInterface != null)
                {
                    loadList.SetRefFieldList(fieldDef, listInterface.GetGenericArguments()[0]);
                }
                else if (fieldDef.FieldReference != null)
                {
                    loadList.SetFieldReference(fieldDef, fieldDef.FieldReference);
                }
                else
                {
                    loadList.SetRefField(fieldDef, fieldDef.FieldType);
                }
            }

            return loadList.ParentResults;
        }
    }

    /// <param name="modelDef">The model definition.</param>
    extension(ModelDefinition modelDef)
    {
        /// <summary>
        /// Gets the reference field definition.
        /// </summary>
        /// <param name="refModelDef">The reference model definition.</param>
        /// <param name="refType">Type of the reference.</param>
        /// <returns>FieldDefinition.</returns>
        /// <exception cref="System.ArgumentException">Cant find '{modelDef.ModelName + "Id"}' Property on Type '{refType.Name}'</exception>
        public FieldDefinition GetRefFieldDef(ModelDefinition refModelDef,
            Type refType)
        {
            var refField = GetRefFieldDefIfExists(modelDef, refModelDef);

            return refField ?? throw new ArgumentException(
                $"Cant find '{modelDef.ModelName + "Id"}' Property on Type '{refType.Name}'");
        }

        /// <summary>
        /// Gets the explicit reference field definition if exists.
        /// </summary>
        /// <param name="refModelDef">The reference model definition.</param>
        /// <returns>ServiceStack.OrmLite.FieldDefinition.</returns>
        public FieldDefinition GetExplicitRefFieldDefIfExists(ModelDefinition refModelDef)
        {
            var refField = refModelDef.FieldDefinitions.FirstOrDefault(x =>
                               x.ForeignKey != null && x.ForeignKey.ReferenceType == modelDef.ModelType &&
                               modelDef.IsRefField(x))
                           ?? refModelDef.FieldDefinitions.FirstOrDefault(x =>
                               x.ForeignKey != null && x.ForeignKey.ReferenceType == modelDef.ModelType);

            return refField;
        }

        /// <summary>
        /// Gets the reference field definition if exists.
        /// </summary>
        /// <param name="refModelDef">The reference model definition.</param>
        /// <returns>FieldDefinition.</returns>
        public FieldDefinition GetRefFieldDefIfExists(ModelDefinition refModelDef)
        {
            var refField = GetExplicitRefFieldDefIfExists(modelDef, refModelDef)
                           ?? refModelDef.FieldDefinitions.FirstOrDefault(modelDef.IsRefField);

            return refField;
        }

        /// <summary>
        /// Gets the self reference field definition if exists.
        /// </summary>
        /// <param name="refModelDef">The reference model definition.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>FieldDefinition.</returns>
        public FieldDefinition GetSelfRefFieldDefIfExists(ModelDefinition refModelDef,
            FieldDefinition fieldDef)
        {
            if (fieldDef?.ReferenceSelfId != null)
            {
                return modelDef.FieldDefinitions.FirstOrDefault(x => x.Name == fieldDef.ReferenceSelfId);
            }

            var refField = (fieldDef != null
                               ? modelDef.FieldDefinitions.FirstOrDefault(x =>
                                   x.ForeignKey != null && x.ForeignKey.ReferenceType == refModelDef.ModelType &&
                                   fieldDef.IsSelfRefField(x))
                               : null)
                           ?? modelDef.FieldDefinitions.FirstOrDefault(x => x.ForeignKey != null && x.ForeignKey.ReferenceType == refModelDef.ModelType)
                           ?? modelDef.FieldDefinitions.FirstOrDefault(refModelDef.IsRefField);

            return refField;
        }
    }

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="size">The size.</param>
        /// <param name="paramFilter">The parameter filter.</param>
        /// <returns>IDbDataParameter.</returns>
        public IDbDataParameter AddParam(string name,
            object value = null,
            ParameterDirection direction = ParameterDirection.Input,
            DbType? dbType = null,
            byte? precision = null,
            byte? scale = null,
            int? size = null,
            Action<IDbDataParameter> paramFilter = null)
        {
            var p = dbCmd.CreateParam(name, value, direction, dbType, precision, scale, size);
            paramFilter?.Invoke(p);
            dbCmd.Parameters.Add(p);
            return p;
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="size">The size.</param>
        /// <returns>IDbDataParameter.</returns>
        public IDbDataParameter CreateParam(string name,
            object value = null,
            ParameterDirection direction = ParameterDirection.Input,
            DbType? dbType = null,
            byte? precision = null,
            byte? scale = null,
            int? size = null)
        {
            var p = dbCmd.CreateParameter();
            var dialectProvider = dbCmd.GetDialectProvider();
            p.ParameterName = dialectProvider.GetParam(name);
            p.Direction = direction;

            if (p.DbType == DbType.String)
            {
                p.Size = dialectProvider.GetStringConverter().StringLength;
                if (value is string strValue && strValue.Length > p.Size)
                {
                    p.Size = strValue.Length;
                }
            }

            if (value != null)
            {
                p.Value = value;
                dialectProvider.InitDbParam(p, value.GetType());
            }
            else
            {
                p.Value = DBNull.Value;
            }

            if (dbType != null)
            {
                p.DbType = dbType.Value;
            }

            if (precision != null)
            {
                p.Precision = precision.Value;
            }

            if (scale != null)
            {
                p.Scale = scale.Value;
            }

            if (size != null)
            {
                p.Size = size.Value;
            }

            return p;
        }

        /// <summary>
        /// SQLs the proc.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="inParams">The in parameters.</param>
        /// <param name="excludeDefaults">if set to <c>true</c> [exclude defaults].</param>
        /// <returns>IDbCommand.</returns>
        internal IDbCommand SqlProc(string name,
            object inParams = null,
            bool excludeDefaults = false)
        {
            dbCmd.CommandType = CommandType.StoredProcedure;
            dbCmd.CommandText = name;

            string sql = null;
            dbCmd.SetParameters(inParams.ToObjectDictionary(), excludeDefaults, sql: ref sql);

            return dbCmd;
        }
    }
}