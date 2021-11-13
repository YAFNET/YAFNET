// ***********************************************************************
// <copyright file="DbScriptsAsync.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Data;
using ServiceStack.Script;
using ServiceStack.Text;

namespace ServiceStack.OrmLite
{
    /// <summary>
    /// Class DbScriptsAsync.
    /// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptMethods" />
    public partial class DbScriptsAsync : ScriptMethods
    {
        /// <summary>
        /// The database information
        /// </summary>
        private const string DbInfo = "__dbinfo"; // Keywords.DbInfo
        /// <summary>
        /// The database connection
        /// </summary>
        private const string DbConnection = "__dbconnection"; // useDb global

        /// <summary>
        /// The database factory
        /// </summary>
        private IDbConnectionFactory dbFactory;
        /// <summary>
        /// Gets or sets the database factory.
        /// </summary>
        /// <value>The database factory.</value>
        public IDbConnectionFactory DbFactory
        {
            get => dbFactory ??= Context.Container.Resolve<IDbConnectionFactory>();
            set => dbFactory = value;
        }

        /// <summary>
        /// Open database connection as an asynchronous operation.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="options">The options.</param>
        /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
        public async Task<IDbConnection> OpenDbConnectionAsync(ScriptScopeContext scope, Dictionary<string, object> options)
        {
            var dbConn = await OpenDbConnectionFromOptionsAsync(options).ConfigAwait();
            if (dbConn != null)
                return dbConn;

            if (scope.PageResult != null)
            {
                if (scope.PageResult.Args.TryGetValue(DbInfo, out var oDbInfo) && oDbInfo is ConnectionInfo dbInfo)
                    return await DbFactory.OpenDbConnectionAsync(dbInfo);

                if (scope.PageResult.Args.TryGetValue(DbConnection, out var oDbConn) && oDbConn is Dictionary<string, object> globalDbConn)
                    return await OpenDbConnectionFromOptionsAsync(globalDbConn);
            }

            return await DbFactory.OpenAsync();
        }

        /// <summary>
        /// Dialects the specified scope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope">The scope.</param>
        /// <param name="fn">The function.</param>
        /// <returns>T.</returns>
        T dialect<T>(ScriptScopeContext scope, Func<IOrmLiteDialectProvider, T> fn)
        {
            if (scope.PageResult != null)
            {
                if (scope.PageResult.Args.TryGetValue(DbInfo, out var oDbInfo) && oDbInfo is ConnectionInfo dbInfo)
                    return fn(DbFactory.GetDialectProvider(dbInfo));

                if (scope.PageResult.Args.TryGetValue(DbConnection, out var oDbConn) && oDbConn is Dictionary<string, object> useDb)
                    return fn(DbFactory.GetDialectProvider(
                        providerName: useDb.GetValueOrDefault("providerName")?.ToString(),
                        namedConnection: useDb.GetValueOrDefault("namedConnection")?.ToString()));
            }
            return fn(OrmLiteConfig.DialectProvider);
        }

        /// <summary>
        /// Uses the database.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="dbConnOptions">The database connection options.</param>
        /// <returns>IgnoreResult.</returns>
        /// <exception cref="System.NotSupportedException">useDb</exception>
        public IgnoreResult useDb(ScriptScopeContext scope, Dictionary<string, object> dbConnOptions)
        {
            if (dbConnOptions == null)
            {
                scope.PageResult.Args.Remove(DbConnection);
            }
            else
            {
                if (!dbConnOptions.ContainsKey("connectionString") && !dbConnOptions.ContainsKey("namedConnection"))
                    throw new NotSupportedException(nameof(useDb) + " requires either 'connectionString' or 'namedConnection' property");

                scope.PageResult.Args[DbConnection] = dbConnOptions;
            }
            return IgnoreResult.Value;
        }

        /// <summary>
        /// Open database connection from options as an asynchronous operation.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
        private async Task<IDbConnection> OpenDbConnectionFromOptionsAsync(Dictionary<string, object> options)
        {
            if (options != null)
            {
                if (options.TryGetValue("connectionString", out var connectionString))
                {
                    return options.TryGetValue("providerName", out var providerName)
                        ? await DbFactory.OpenDbConnectionStringAsync((string)connectionString, (string)providerName).ConfigAwait()
                        : await DbFactory.OpenDbConnectionStringAsync((string)connectionString).ConfigAwait();
                }

                if (options.TryGetValue("namedConnection", out var namedConnection))
                {
                    return await DbFactory.OpenDbConnectionAsync((string)namedConnection).ConfigAwait();
                }
            }

            return null;
        }

        /// <summary>
        /// Executes the specified function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fn">The function.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="ServiceStack.Script.StopFilterExecutionException"></exception>
        async Task<object> exec<T>(Func<IDbConnection, Task<T>> fn, ScriptScopeContext scope, object options)
        {
            try
            {
                using var db = await OpenDbConnectionAsync(scope, options as Dictionary<string, object>).ConfigAwait();
                var result = await fn(db).ConfigAwait();
                return result;
            }
            catch (Exception ex)
            {
                throw new StopFilterExecutionException(scope, options, ex);
            }
        }

        /// <summary>
        /// Databases the select.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbSelect(ScriptScopeContext scope, string sql) =>
            exec(db => db.SqlListAsync<Dictionary<string, object>>(sql), scope, null);

        /// <summary>
        /// Databases the select.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbSelect(ScriptScopeContext scope, string sql, Dictionary<string, object> args) =>
            exec(db => db.SqlListAsync<Dictionary<string, object>>(sql, args), scope, null);

        /// <summary>
        /// Databases the select.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbSelect(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) =>
            exec(db => db.SqlListAsync<Dictionary<string, object>>(sql, args), scope, options);


        /// <summary>
        /// Databases the single.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbSingle(ScriptScopeContext scope, string sql) =>
            exec(db => db.SingleAsync<Dictionary<string, object>>(sql), scope, null);

        /// <summary>
        /// Databases the single.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbSingle(ScriptScopeContext scope, string sql, Dictionary<string, object> args) =>
            exec(db => db.SingleAsync<Dictionary<string, object>>(sql, args), scope, null);

        /// <summary>
        /// Databases the single.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbSingle(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) =>
            exec(db => db.SingleAsync<Dictionary<string, object>>(sql, args), scope, options);


        /// <summary>
        /// Databases the scalar.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbScalar(ScriptScopeContext scope, string sql) =>
            exec(db => db.ScalarAsync<object>(sql), scope, null);

        /// <summary>
        /// Databases the scalar.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbScalar(ScriptScopeContext scope, string sql, Dictionary<string, object> args) =>
            exec(db => db.ScalarAsync<object>(sql, args), scope, null);

        /// <summary>
        /// Databases the scalar.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbScalar(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) =>
            exec(db => db.ScalarAsync<object>(sql, args), scope, options);


        /// <summary>
        /// Databases the count.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbCount(ScriptScopeContext scope, string sql) =>
            exec(db => db.RowCountAsync(sql), scope, null);

        /// <summary>
        /// Databases the count.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbCount(ScriptScopeContext scope, string sql, Dictionary<string, object> args) =>
            exec(db => db.RowCountAsync(sql, args), scope, null);

        /// <summary>
        /// Databases the count.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbCount(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) =>
            exec(db => db.RowCountAsync(sql, args), scope, options);


        /// <summary>
        /// Databases the exists.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>System.Object.</returns>
        public async Task<object> dbExists(ScriptScopeContext scope, string sql) =>
            await dbScalar(scope, sql).ConfigAwait() != null;

        /// <summary>
        /// Databases the exists.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Object.</returns>
        public async Task<object> dbExists(ScriptScopeContext scope, string sql, Dictionary<string, object> args) =>
            await dbScalar(scope, sql, args).ConfigAwait() != null;

        /// <summary>
        /// Databases the exists.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public async Task<object> dbExists(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) =>
            await dbScalar(scope, sql, args, options).ConfigAwait() != null;


        /// <summary>
        /// Databases the execute.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbExec(ScriptScopeContext scope, string sql) =>
            exec(db => db.ExecuteSqlAsync(sql), scope, null);

        /// <summary>
        /// Databases the execute.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbExec(ScriptScopeContext scope, string sql, Dictionary<string, object> args) =>
            exec(db => db.ExecuteSqlAsync(sql, args), scope, null);

        /// <summary>
        /// Databases the execute.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbExec(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) =>
            exec(db => db.ExecuteSqlAsync(sql, args), scope, options);

        /// <summary>
        /// Databases the named connections.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> dbNamedConnections() => OrmLiteConnectionFactory.NamedConnections.Keys.ToList();
        /// <summary>
        /// Databases the table names.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbTableNames(ScriptScopeContext scope) => dbTableNames(scope, null, null);
        /// <summary>
        /// Databases the table names.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbTableNames(ScriptScopeContext scope, Dictionary<string, object> args) => dbTableNames(scope, args, null);
        /// <summary>
        /// Databases the table names.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbTableNames(ScriptScopeContext scope, Dictionary<string, object> args, object options) =>
            exec(db => db.GetTableNamesAsync(args != null && args.TryGetValue("schema", out var oSchema) ? oSchema as string : null), scope, options);

        /// <summary>
        /// Databases the table names with row counts.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbTableNamesWithRowCounts(ScriptScopeContext scope) =>
            dbTableNamesWithRowCounts(scope, null, null);
        /// <summary>
        /// Databases the table names with row counts.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbTableNamesWithRowCounts(ScriptScopeContext scope, Dictionary<string, object> args) =>
            dbTableNamesWithRowCounts(scope, args, null);
        /// <summary>
        /// Databases the table names with row counts.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbTableNamesWithRowCounts(ScriptScopeContext scope, Dictionary<string, object> args, object options) =>
            exec(db => args == null
                    ? db.GetTableNamesWithRowCountsAsync()
                    : db.GetTableNamesWithRowCountsAsync(
                        live: args.TryGetValue("live", out var oLive) && oLive is bool b && b,
                        schema: args.TryGetValue("schema", out var oSchema) ? oSchema as string : null),
                scope, options);

        /// <summary>
        /// Databases the column names.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbColumnNames(ScriptScopeContext scope, string tableName) => dbColumnNames(scope, tableName, null);
        /// <summary>
        /// Databases the column names.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbColumnNames(ScriptScopeContext scope, string tableName, object options) =>
            exec(async db => (await db.GetTableColumnsAsync($"SELECT * FROM {sqlQuote(scope, tableName)}").ConfigAwait())
                .Select(x => x.ColumnName).ToArray(), scope, options);

        /// <summary>
        /// Databases the columns.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbColumns(ScriptScopeContext scope, string tableName) => dbColumns(scope, tableName, null);
        /// <summary>
        /// Databases the columns.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbColumns(ScriptScopeContext scope, string tableName, object options) =>
            exec(db => db.GetTableColumnsAsync($"SELECT * FROM {sqlQuote(scope, tableName)}"), scope, options);

        /// <summary>
        /// Databases the desc.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbDesc(ScriptScopeContext scope, string sql) => dbDesc(scope, sql, null);
        /// <summary>
        /// Databases the desc.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> dbDesc(ScriptScopeContext scope, string sql, object options) => exec(db => db.GetTableColumnsAsync(sql), scope, options);

        /// <summary>
        /// SQLs the quote.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string sqlQuote(ScriptScopeContext scope, string name) => dialect(scope, d => d.GetQuotedName(name));
        /// <summary>
        /// SQLs the concat.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="values">The values.</param>
        /// <returns>System.String.</returns>
        public string sqlConcat(ScriptScopeContext scope, IEnumerable<object> values) => dialect(scope, d => d.SqlConcat(values));
        /// <summary>
        /// SQLs the currency.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="fieldOrValue">The field or value.</param>
        /// <returns>System.String.</returns>
        public string sqlCurrency(ScriptScopeContext scope, string fieldOrValue) => dialect(scope, d => d.SqlCurrency(fieldOrValue));
        /// <summary>
        /// SQLs the currency.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="fieldOrValue">The field or value.</param>
        /// <param name="symbol">The symbol.</param>
        /// <returns>System.String.</returns>
        public string sqlCurrency(ScriptScopeContext scope, string fieldOrValue, string symbol) =>
            dialect(scope, d => d.SqlCurrency(fieldOrValue, symbol));

        /// <summary>
        /// SQLs the cast.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="fieldOrValue">The field or value.</param>
        /// <param name="castAs">The cast as.</param>
        /// <returns>System.String.</returns>
        public string sqlCast(ScriptScopeContext scope, object fieldOrValue, string castAs) =>
            dialect(scope, d => d.SqlCast(fieldOrValue, castAs));
        /// <summary>
        /// SQLs the bool.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>System.String.</returns>
        public string sqlBool(ScriptScopeContext scope, bool value) => dialect(scope, d => d.SqlBool(value));
        /// <summary>
        /// SQLs the true.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.String.</returns>
        public string sqlTrue(ScriptScopeContext scope) => dialect(scope, d => d.SqlBool(true));
        /// <summary>
        /// SQLs the false.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.String.</returns>
        public string sqlFalse(ScriptScopeContext scope) => dialect(scope, d => d.SqlBool(false));
        /// <summary>
        /// SQLs the limit.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>System.String.</returns>
        public string sqlLimit(ScriptScopeContext scope, int? offset, int? limit) =>
            dialect(scope, d => padCondition(d.SqlLimit(offset, limit)));
        /// <summary>
        /// SQLs the limit.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>System.String.</returns>
        public string sqlLimit(ScriptScopeContext scope, int? limit) =>
            dialect(scope, d => padCondition(d.SqlLimit(null, limit)));
        /// <summary>
        /// SQLs the skip.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>System.String.</returns>
        public string sqlSkip(ScriptScopeContext scope, int? offset) =>
            dialect(scope, d => padCondition(d.SqlLimit(offset, null)));
        /// <summary>
        /// SQLs the take.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>System.String.</returns>
        public string sqlTake(ScriptScopeContext scope, int? limit) =>
            dialect(scope, d => padCondition(d.SqlLimit(null, limit)));
        /// <summary>
        /// SQLs the order by fields.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>System.String.</returns>
        public string sqlOrderByFields(ScriptScopeContext scope, string orderBy) =>
            dialect(scope, d => OrmLiteUtils.OrderByFields(d, orderBy));
        /// <summary>
        /// Ormlites the variable.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string ormliteVar(ScriptScopeContext scope, string name) =>
            dialect(scope, d => d.Variables.TryGetValue(name, out var value) ? value : null);

        /// <summary>
        /// SQLs the verify fragment.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>System.String.</returns>
        public string sqlVerifyFragment(string sql) => sql.SqlVerifyFragment();
        /// <summary>
        /// Determines whether [is unsafe SQL] [the specified SQL].
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns><c>true</c> if [is unsafe SQL] [the specified SQL]; otherwise, <c>false</c>.</returns>
        public bool isUnsafeSql(string sql) => OrmLiteUtils.isUnsafeSql(sql, OrmLiteUtils.VerifySqlRegEx);
        /// <summary>
        /// Determines whether [is unsafe SQL fragment] [the specified SQL].
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns><c>true</c> if [is unsafe SQL fragment] [the specified SQL]; otherwise, <c>false</c>.</returns>
        public bool isUnsafeSqlFragment(string sql) => OrmLiteUtils.isUnsafeSql(sql, OrmLiteUtils.VerifyFragmentRegEx);

        /// <summary>
        /// Pads the condition.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        private string padCondition(string text) => string.IsNullOrEmpty(text) ? "" : " " + text;
    }
}