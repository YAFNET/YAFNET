// ***********************************************************************
// <copyright file="OrmLiteReadCommandExtensionsLegacy.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using ServiceStack.Text;

namespace ServiceStack.OrmLite.Legacy
{
    /// <summary>
    /// Class OrmLiteReadCommandExtensionsLegacy.
    /// </summary>
    [Obsolete(Messages.LegacyApi)]
    internal static class OrmLiteReadCommandExtensionsLegacy
    {
        /// <summary>
        /// Selects the FMT.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal static List<T> SelectFmt<T>(this IDbCommand dbCmd, string sqlFilter, params object[] filterParams)
        {
            return dbCmd.ConvertToList<T>(
                dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), sqlFilter, filterParams));
        }

        /// <summary>
        /// Selects the FMT.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>List&lt;TModel&gt;.</returns>
        internal static List<TModel> SelectFmt<TModel>(this IDbCommand dbCmd, Type fromTableType, string sqlFilter, params object[] filterParams)
        {
            var sql = ToSelectFmt<TModel>(dbCmd.GetDialectProvider(), fromTableType, sqlFilter, filterParams);

            return dbCmd.ConvertToList<TModel>(sql);
        }

        /// <summary>
        /// Converts to selectfmt.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="dialectProvider">The dialect provider.</param>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>System.String.</returns>
        internal static string ToSelectFmt<TModel>(IOrmLiteDialectProvider dialectProvider, Type fromTableType, string sqlFilter, object[] filterParams)
        {
            var sql = StringBuilderCache.Allocate();
            var modelDef = ModelDefinition<TModel>.Definition;
            sql.AppendFormat("SELECT {0} FROM {1}", dialectProvider.GetColumnNames(modelDef),
                             dialectProvider.GetQuotedTableName(fromTableType.GetModelDefinition()));
            if (!string.IsNullOrEmpty(sqlFilter))
            {
                sqlFilter = sqlFilter.SqlFmt(filterParams);
                sql.Append(" WHERE ");
                sql.Append(sqlFilter);
            }
            return StringBuilderCache.ReturnAndFree(sql);
        }

        /// <summary>
        /// Selects the lazy FMT.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        internal static IEnumerable<T> SelectLazyFmt<T>(this IDbCommand dbCmd, string filter, params object[] filterParams)
        {
            var dialectProvider = dbCmd.GetDialectProvider();
            dbCmd.CommandText = dialectProvider.ToSelectStatement(typeof(T), filter, filterParams);

            if (OrmLiteConfig.ResultsFilter != null)
            {
                foreach (var item in OrmLiteConfig.ResultsFilter.GetList<T>(dbCmd))
                {
                    yield return item;
                }
                yield break;
            }

            using (var reader = dbCmd.ExecReader(dbCmd.CommandText))
            {
                var indexCache = reader.GetIndexFieldsCache(ModelDefinition<T>.Definition, dialectProvider);
                var values = new object[reader.FieldCount];
                while (reader.Read())
                {
                    var row = OrmLiteUtils.CreateInstance<T>();
                    row.PopulateWithSqlReader(dialectProvider, reader, indexCache, values);
                    yield return row;
                }
            }
        }

        /// <summary>
        /// Singles the FMT.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>T.</returns>
        internal static T SingleFmt<T>(this IDbCommand dbCmd, string filter, params object[] filterParams)
        {
            return dbCmd.ConvertTo<T>(dbCmd.GetDialectProvider().ToSelectStatement(typeof(T), filter, filterParams));
        }

        /// <summary>
        /// Scalars the FMT.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>T.</returns>
        internal static T ScalarFmt<T>(this IDbCommand dbCmd, string sql, params object[] sqlParams)
        {
            return dbCmd.Scalar<T>(sql.SqlFmt(sqlParams));
        }

        /// <summary>
        /// Columns the FMT.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>List&lt;T&gt;.</returns>
        internal static List<T> ColumnFmt<T>(this IDbCommand dbCmd, string sql, params object[] sqlParams)
        {
            return dbCmd.Column<T>(sql.SqlFmt(sqlParams));
        }

        /// <summary>
        /// Columns the distinct FMT.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        internal static HashSet<T> ColumnDistinctFmt<T>(this IDbCommand dbCmd, string sql, params object[] sqlParams)
        {
            return dbCmd.ColumnDistinct<T>(sql.SqlFmt(sqlParams));
        }

        /// <summary>
        /// Lookups the FMT.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
        internal static Dictionary<K, List<V>> LookupFmt<K, V>(this IDbCommand dbCmd, string sql, params object[] sqlParams)
        {
            return dbCmd.Lookup<K, V>(sql.SqlFmt(sqlParams));
        }

        /// <summary>
        /// Dictionaries the FMT.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sqlFormat">The SQL format.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>Dictionary&lt;K, V&gt;.</returns>
        internal static Dictionary<K, V> DictionaryFmt<K, V>(this IDbCommand dbCmd, string sqlFormat, params object[] sqlParams)
        {
            return dbCmd.Dictionary<K, V>(sqlFormat.SqlFmt(sqlParams));
        }

        /// <summary>
        /// Existses the FMT.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool ExistsFmt<T>(this IDbCommand dbCmd, string sqlFilter, params object[] filterParams)
        {
            var fromTableType = typeof(T);
            var result = dbCmd.Scalar(dbCmd.GetDialectProvider().ToSelectStatement(fromTableType, sqlFilter, filterParams));
            return result != null;
        }
    }
}