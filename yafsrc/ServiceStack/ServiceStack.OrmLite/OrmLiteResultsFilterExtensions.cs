// ***********************************************************************
// <copyright file="OrmLiteResultsFilterExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using ServiceStack.Logging;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteResultsFilterExtensions.
/// </summary>
public static class OrmLiteResultsFilterExtensions
{
    /// <summary>
    /// The log
    /// </summary>
    static internal ILog Log => OrmLiteLog.Log;

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>System.Int32.</returns>
        public int ExecNonQuery(string sql, object anonType = null)
        {
            if (anonType != null)
            {
                dbCmd.SetParameters(anonType.AssertAnonObject().ToObjectDictionary(), (bool)false, sql: ref sql);
            }

            dbCmd.CommandText = sql;

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.ExecuteSql(dbCmd);
            }

            return dbCmd.WithLog(dbCmd.ExecuteNonQuery());
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="dict">The dictionary.</param>
        /// <returns>System.Int32.</returns>
        public int ExecNonQuery(string sql, Dictionary<string, object> dict)
        {

            if (dict != null)
            {
                dbCmd.SetParameters(dict, (bool)false, sql: ref sql);
            }

            dbCmd.CommandText = sql;

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.ExecuteSql(dbCmd);
            }

            return dbCmd.WithLog(dbCmd.ExecuteNonQuery());
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int ExecNonQuery()
        {
            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.ExecuteSql(dbCmd);
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            return dbCmd.WithLog(dbCmd.ExecuteNonQuery());
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="dbCmdFilter">The database command filter.</param>
        /// <returns>System.Int32.</returns>
        public int ExecNonQuery(string sql, Action<IDbCommand> dbCmdFilter)
        {
            dbCmdFilter?.Invoke(dbCmd);

            dbCmd.CommandText = sql;

            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.ExecuteSql(dbCmd);
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            return dbCmd.WithLog(dbCmd.ExecuteNonQuery());
        }

        /// <summary>
        /// Converts to list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> ConvertToList<T>(string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            var isScalar = OrmLiteUtils.IsScalar<T>();

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return isScalar
                    ? OrmLiteConfig.ResultsFilter.GetColumn<T>(dbCmd)
                    : OrmLiteConfig.ResultsFilter.GetList<T>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return isScalar
                ? reader.Column<T>(dbCmd.GetDialectProvider())
                : reader.ConvertToList<T>(dbCmd.GetDialectProvider());
        }

        /// <summary>
        /// Converts to list.
        /// </summary>
        /// <param name="refType">Type of the reference.</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>IList.</returns>
        public IList ConvertToList(Type refType, string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetRefList(dbCmd, refType);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.ConvertToList(dbCmd.GetDialectProvider(), refType);
        }
    }

    /// <summary>
    /// Populates the with.
    /// </summary>
    /// <param name="to">To.</param>
    /// <param name="from">From.</param>
    /// <returns>IDbDataParameter.</returns>
    public static IDbDataParameter PopulateWith(this IDbDataParameter to, IDbDataParameter from)
    {
        to.ParameterName = from.ParameterName;
        to.DbType = from.DbType;
        to.Value = from.Value;

        if (from.Precision != 0)
        {
            to.Precision = from.Precision;
        }

        if (from.Scale != 0)
        {
            to.Scale = from.Scale;
        }

        if (from.Size != 0)
        {
            to.Size = from.Size;
        }

        return to;
    }

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Exprs the convert to list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> ExprConvertToList<T>(string sql = null, IEnumerable<IDbDataParameter> sqlParams = null, HashSet<string> onlyFields = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            dbCmd.SetParameters(sqlParams);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetList<T>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.ConvertToList<T>(dbCmd.GetDialectProvider(), onlyFields: onlyFields);
        }

        /// <summary>
        /// Converts to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns>T.</returns>
        public T ConvertTo<T>(string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetSingle<T>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.ConvertTo<T>(dbCmd.GetDialectProvider());
        }

        /// <summary>
        /// Converts to.
        /// </summary>
        /// <param name="refType">Type of the reference.</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>System.Object.</returns>
        internal object ConvertTo(Type refType, string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetRefSingle(dbCmd, refType);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.ConvertTo(dbCmd.GetDialectProvider(), refType);
        }

        /// <summary>
        /// Scalars the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>T.</returns>
        internal T Scalar<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbCmd.SetParameters(sqlParams).Scalar<T>(sql);
        }

        /// <summary>
        /// Scalars the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns>T.</returns>
        internal T Scalar<T>(string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetScalar<T>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.Scalar<T>(dbCmd.GetDialectProvider());
        }

        /// <summary>
        /// Scalars the specified SQL expression.
        /// </summary>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <returns>System.Object.</returns>
        public object Scalar(ISqlExpression sqlExpression)
        {
            dbCmd.PopulateWith(sqlExpression, QueryType.Scalar);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetScalar(dbCmd);
            }

            return dbCmd.ExecuteScalar();
        }

        /// <summary>
        /// Scalars the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>System.Object.</returns>
        public object Scalar(string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetScalar(dbCmd);
            }

            return dbCmd.ExecuteScalar();
        }

        /// <summary>
        /// Executes the long scalar.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>System.Int64.</returns>
        public long ExecLongScalar(string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugCommand(dbCmd);
            }

            OrmLiteConfig.BeforeExecFilter?.Invoke(dbCmd);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetLongScalar(dbCmd);
            }

            return dbCmd.WithLog(dbCmd.LongScalar());
        }

        /// <summary>
        /// Exprs the convert to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <returns>T.</returns>
        internal T ExprConvertTo<T>(string sql = null, IEnumerable<IDbDataParameter> sqlParams = null, HashSet<string> onlyFields = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            dbCmd.SetParameters(sqlParams);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetSingle<T>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.ConvertTo<T>(dbCmd.GetDialectProvider(), onlyFields: onlyFields);
        }

        /// <summary>
        /// Columns the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Column<T>(string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetColumn<T>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.Column<T>(dbCmd.GetDialectProvider());
        }

        /// <summary>
        /// Columns the specified SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal List<T> Column<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbCmd.SetParameters(sqlParams).Column<T>(sql);
        }

        /// <summary>
        /// Columns the distinct.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        internal HashSet<T> ColumnDistinct<T>(string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetColumnDistinct<T>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.ColumnDistinct<T>(dbCmd.GetDialectProvider());
        }

        /// <summary>
        /// Columns the distinct.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        internal HashSet<T> ColumnDistinct<T>(ISqlExpression expression)
        {
            dbCmd.PopulateWith(expression, QueryType.Select);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetColumnDistinct<T>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.ColumnDistinct<T>(dbCmd.GetDialectProvider());
        }

        /// <summary>
        /// Dictionaries the specified SQL.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns>Dictionary&lt;K, V&gt;.</returns>
        internal Dictionary<K, V> Dictionary<K, V>(string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetDictionary<K, V>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.Dictionary<K, V>(dbCmd.GetDialectProvider());
        }

        /// <summary>
        /// Dictionaries the specified expression.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>Dictionary&lt;K, V&gt;.</returns>
        internal Dictionary<K, V> Dictionary<K, V>(ISqlExpression expression)
        {
            dbCmd.PopulateWith(expression, QueryType.Select);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetDictionary<K, V>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.Dictionary<K, V>(dbCmd.GetDialectProvider());
        }

        /// <summary>
        /// Keys the value pairs.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns>List&lt;KeyValuePair&lt;K, V&gt;&gt;.</returns>
        internal List<KeyValuePair<K, V>> KeyValuePairs<K, V>(string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetKeyValuePairs<K, V>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.KeyValuePairs<K, V>(dbCmd.GetDialectProvider());
        }

        /// <summary>
        /// Keys the value pairs.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;KeyValuePair&lt;K, V&gt;&gt;.</returns>
        internal List<KeyValuePair<K, V>> KeyValuePairs<K, V>(ISqlExpression expression)
        {
            dbCmd.PopulateWith(expression, QueryType.Select);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetKeyValuePairs<K, V>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.KeyValuePairs<K, V>(dbCmd.GetDialectProvider());
        }

        /// <summary>
        /// Lookups the specified SQL.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
        internal Dictionary<K, List<V>> Lookup<K, V>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbCmd.SetParameters(sqlParams).Lookup<K, V>(sql);
        }

        /// <summary>
        /// Lookups the specified SQL.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
        internal Dictionary<K, List<V>> Lookup<K, V>(string sql = null)
        {
            if (sql != null)
            {
                dbCmd.CommandText = sql;
            }

            if (OrmLiteConfig.ResultsFilter != null)
            {
                return OrmLiteConfig.ResultsFilter.GetLookup<K, V>(dbCmd);
            }

            using var reader = dbCmd.ExecReader(dbCmd.CommandText);
            return reader.Lookup<K, V>(dbCmd.GetDialectProvider());
        }
    }
}