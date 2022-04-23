// ***********************************************************************
// <copyright file="DbScripts.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServiceStack.Data;
using ServiceStack.Script;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class DbScripts.
/// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
/// </summary>
/// <seealso cref="ServiceStack.Script.ScriptMethods" />
public class DbScripts : ScriptMethods
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
        get => this.dbFactory ??= this.Context.Container.Resolve<IDbConnectionFactory>();
        set => this.dbFactory = value;
    }

    /// <summary>
    /// Opens the database connection.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="options">The options.</param>
    /// <returns>IDbConnection.</returns>
    public IDbConnection OpenDbConnection(ScriptScopeContext scope, Dictionary<string, object> options)
    {
        var dbConn = this.OpenDbConnectionFromOptions(options);
        if (dbConn != null)
            return dbConn;

        if (scope.PageResult != null)
        {
            if (scope.PageResult.Args.TryGetValue(DbInfo, out var oDbInfo) && oDbInfo is ConnectionInfo dbInfo)
                return this.DbFactory.OpenDbConnection(dbInfo);

            if (scope.PageResult.Args.TryGetValue(DbConnection, out var oDbConn) && oDbConn is Dictionary<string, object> globalDbConn)
                return this.OpenDbConnectionFromOptions(globalDbConn);
        }

        return this.DbFactory.OpenDbConnection();
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
                return fn(this.DbFactory.GetDialectProvider(dbInfo));

            if (scope.PageResult.Args.TryGetValue(DbConnection, out var oDbConn) && oDbConn is Dictionary<string, object> useDb)
                return fn(
                    this.DbFactory.GetDialectProvider(
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
                throw new NotSupportedException(nameof(this.useDb) + " requires either 'connectionString' or 'namedConnection' property");

            scope.PageResult.Args[DbConnection] = dbConnOptions;
        }

        return IgnoreResult.Value;
    }

    /// <summary>
    /// Opens the database connection from options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>IDbConnection.</returns>
    private IDbConnection OpenDbConnectionFromOptions(Dictionary<string, object> options)
    {
        if (options != null)
        {
            if (options.TryGetValue("connectionString", out var connectionString))
            {
                return options.TryGetValue("providerName", out var providerName)
                           ? this.DbFactory.OpenDbConnectionString((string)connectionString, (string)providerName)
                           : this.DbFactory.OpenDbConnectionString((string)connectionString);
            }

            if (options.TryGetValue("namedConnection", out var namedConnection))
            {
                return this.DbFactory.OpenDbConnection((string)namedConnection);
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
    /// <returns>T.</returns>
    /// <exception cref="ServiceStack.Script.StopFilterExecutionException"></exception>
    T exec<T>(Func<IDbConnection, T> fn, ScriptScopeContext scope, object options)
    {
        try
        {
            using var db = this.OpenDbConnection(scope, options as Dictionary<string, object>);
            return fn(db);
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
    /// <returns>System.Object.</returns>
    public object dbSelect(ScriptScopeContext scope, string sql) => this.exec(db => db.SqlList<Dictionary<string, object>>(sql), scope, null);

    /// <summary>
    /// Databases the select.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Object.</returns>
    public object dbSelect(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.exec(db => db.SqlList<Dictionary<string, object>>(sql, args), scope, null);

    /// <summary>
    /// Databases the select.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Object.</returns>
    public object dbSelect(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.exec(db => db.SqlList<Dictionary<string, object>>(sql, args), scope, options);


    /// <summary>
    /// Databases the single.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>System.Object.</returns>
    public object dbSingle(ScriptScopeContext scope, string sql) => this.exec(db => db.Single<Dictionary<string, object>>(sql), scope, null);

    /// <summary>
    /// Databases the single.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Object.</returns>
    public object dbSingle(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.exec(db => db.Single<Dictionary<string, object>>(sql, args), scope, null);

    /// <summary>
    /// Databases the single.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Object.</returns>
    public object dbSingle(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.exec(db => db.Single<Dictionary<string, object>>(sql, args), scope, options);


    /// <summary>
    /// Databases the scalar.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>System.Object.</returns>
    public object dbScalar(ScriptScopeContext scope, string sql) => this.exec(db => db.Scalar<object>(sql), scope, null);

    /// <summary>
    /// Databases the scalar.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Object.</returns>
    public object dbScalar(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.exec(db => db.Scalar<object>(sql, args), scope, null);

    /// <summary>
    /// Databases the scalar.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Object.</returns>
    public object dbScalar(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.exec(db => db.Scalar<object>(sql, args), scope, options);


    /// <summary>
    /// Databases the count.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>System.Int64.</returns>
    public long dbCount(ScriptScopeContext scope, string sql) => this.exec(db => db.RowCount(sql), scope, null);

    /// <summary>
    /// Databases the count.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Int64.</returns>
    public long dbCount(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.exec(db => db.RowCount(sql, args), scope, null);

    /// <summary>
    /// Databases the count.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Int64.</returns>
    public long dbCount(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.exec(db => db.RowCount(sql, args), scope, options);


    /// <summary>
    /// Databases the exists.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool dbExists(ScriptScopeContext scope, string sql) => this.dbScalar(scope, sql) != null;

    /// <summary>
    /// Databases the exists.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool dbExists(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.dbScalar(scope, sql, args) != null;

    /// <summary>
    /// Databases the exists.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool dbExists(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.dbScalar(scope, sql, args, options) != null;


    /// <summary>
    /// Databases the execute.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>System.Int32.</returns>
    public int dbExec(ScriptScopeContext scope, string sql) => this.exec(db => db.ExecuteSql(sql), scope, null);

    /// <summary>
    /// Databases the execute.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Int32.</returns>
    public int dbExec(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.exec(db => db.ExecuteSql(sql, args), scope, null);

    /// <summary>
    /// Databases the execute.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Int32.</returns>
    public int dbExec(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.exec(db => db.ExecuteSql(sql, args), scope, options);

    /// <summary>
    /// Databases the named connections.
    /// </summary>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> dbNamedConnections() => OrmLiteConnectionFactory.NamedConnections.Keys.ToList();

    /// <summary>
    /// Databases the table names.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> dbTableNames(ScriptScopeContext scope) => this.dbTableNames(scope, null, null);

    /// <summary>
    /// Databases the table names.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> dbTableNames(ScriptScopeContext scope, Dictionary<string, object> args) => this.dbTableNames(scope, args, null);

    /// <summary>
    /// Databases the table names.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> dbTableNames(ScriptScopeContext scope, Dictionary<string, object> args, object options) => this.exec(db => db.GetTableNames(args != null && args.TryGetValue("schema", out var oSchema) ? oSchema as string : null), scope, options);

    /// <summary>
    /// Databases the table names with row counts.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>List&lt;KeyValuePair&lt;System.String, System.Int64&gt;&gt;.</returns>
    public List<KeyValuePair<string, long>> dbTableNamesWithRowCounts(ScriptScopeContext scope) => this.dbTableNamesWithRowCounts(scope, null, null);

    /// <summary>
    /// Databases the table names with row counts.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>List&lt;KeyValuePair&lt;System.String, System.Int64&gt;&gt;.</returns>
    public List<KeyValuePair<string, long>> dbTableNamesWithRowCounts(ScriptScopeContext scope, Dictionary<string, object> args) => this.dbTableNamesWithRowCounts(scope, args, null);

    /// <summary>
    /// Databases the table names with row counts.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>List&lt;KeyValuePair&lt;System.String, System.Int64&gt;&gt;.</returns>
    public List<KeyValuePair<string, long>> dbTableNamesWithRowCounts(ScriptScopeContext scope, Dictionary<string, object> args, object options) =>
        this.exec(db => args == null
                            ? db.GetTableNamesWithRowCounts()
                            : db.GetTableNamesWithRowCounts(
                                live: args.TryGetValue("live", out var oLive) && oLive is bool b && b,
                                schema: args.TryGetValue("schema", out var oSchema) ? oSchema as string : null),
            scope, options);

    /// <summary>
    /// Databases the column names.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <returns>System.String[].</returns>
    public string[] dbColumnNames(ScriptScopeContext scope, string tableName) => this.dbColumnNames(scope, tableName, null);

    /// <summary>
    /// Databases the column names.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String[].</returns>
    public string[] dbColumnNames(ScriptScopeContext scope, string tableName, object options) => this.dbColumns(scope, tableName, options).Select(x => x.ColumnName).ToArray();

    /// <summary>
    /// Databases the columns.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <returns>ColumnSchema[].</returns>
    public ColumnSchema[] dbColumns(ScriptScopeContext scope, string tableName) => this.dbColumns(scope, tableName, null);

    /// <summary>
    /// Databases the columns.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="options">The options.</param>
    /// <returns>ColumnSchema[].</returns>
    public ColumnSchema[] dbColumns(ScriptScopeContext scope, string tableName, object options) => this.exec(db => db.GetTableColumns($"SELECT * FROM {this.sqlQuote(scope, tableName)}"), scope, options);

    /// <summary>
    /// Databases the desc.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>ColumnSchema[].</returns>
    public ColumnSchema[] dbDesc(ScriptScopeContext scope, string sql) => this.dbDesc(scope, sql, null);

    /// <summary>
    /// Databases the desc.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="options">The options.</param>
    /// <returns>ColumnSchema[].</returns>
    public ColumnSchema[] dbDesc(ScriptScopeContext scope, string sql, object options) => this.exec(db => db.GetTableColumns(sql), scope, options);


    /// <summary>
    /// SQLs the quote.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public string sqlQuote(ScriptScopeContext scope, string name) => this.dialect(scope, d => d.GetQuotedName(name));

    /// <summary>
    /// SQLs the concat.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="values">The values.</param>
    /// <returns>System.String.</returns>
    public string sqlConcat(ScriptScopeContext scope, IEnumerable<object> values) => this.dialect(scope, d => d.SqlConcat(values));

    /// <summary>
    /// SQLs the currency.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <returns>System.String.</returns>
    public string sqlCurrency(ScriptScopeContext scope, string fieldOrValue) => this.dialect(scope, d => d.SqlCurrency(fieldOrValue));

    /// <summary>
    /// SQLs the currency.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <param name="symbol">The symbol.</param>
    /// <returns>System.String.</returns>
    public string sqlCurrency(ScriptScopeContext scope, string fieldOrValue, string symbol) => this.dialect(scope, d => d.SqlCurrency(fieldOrValue, symbol));

    /// <summary>
    /// SQLs the cast.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <param name="castAs">The cast as.</param>
    /// <returns>System.String.</returns>
    public string sqlCast(ScriptScopeContext scope, object fieldOrValue, string castAs) => this.dialect(scope, d => d.SqlCast(fieldOrValue, castAs));

    /// <summary>
    /// SQLs the bool.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    /// <returns>System.String.</returns>
    public string sqlBool(ScriptScopeContext scope, bool value) => this.dialect(scope, d => d.SqlBool(value));

    /// <summary>
    /// SQLs the true.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.String.</returns>
    public string sqlTrue(ScriptScopeContext scope) => this.dialect(scope, d => d.SqlBool(true));

    /// <summary>
    /// SQLs the false.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.String.</returns>
    public string sqlFalse(ScriptScopeContext scope) => this.dialect(scope, d => d.SqlBool(false));

    /// <summary>
    /// SQLs the limit.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="limit">The limit.</param>
    /// <returns>System.String.</returns>
    public string sqlLimit(ScriptScopeContext scope, int? offset, int? limit) => this.dialect(scope, d => this.padCondition(d.SqlLimit(offset, limit)));

    /// <summary>
    /// SQLs the limit.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="limit">The limit.</param>
    /// <returns>System.String.</returns>
    public string sqlLimit(ScriptScopeContext scope, int? limit) => this.dialect(scope, d => this.padCondition(d.SqlLimit(null, limit)));

    /// <summary>
    /// SQLs the skip.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>System.String.</returns>
    public string sqlSkip(ScriptScopeContext scope, int? offset) => this.dialect(scope, d => this.padCondition(d.SqlLimit(offset, null)));

    /// <summary>
    /// SQLs the take.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="limit">The limit.</param>
    /// <returns>System.String.</returns>
    public string sqlTake(ScriptScopeContext scope, int? limit) => this.dialect(scope, d => this.padCondition(d.SqlLimit(null, limit)));

    /// <summary>
    /// SQLs the order by fields.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="orderBy">The order by.</param>
    /// <returns>System.String.</returns>
    public string sqlOrderByFields(ScriptScopeContext scope, string orderBy) => this.dialect(scope, d => OrmLiteUtils.OrderByFields(d, orderBy));

    /// <summary>
    /// Ormlites the variable.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public string ormliteVar(ScriptScopeContext scope, string name) => this.dialect(scope, d => d.Variables.TryGetValue(name, out var value) ? value : null);

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
    private string padCondition(string text) => string.IsNullOrEmpty(text) ? string.Empty : " " + text;
}

/// <summary>
/// Class DbScriptsAsync.
/// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
/// </summary>
/// <seealso cref="ServiceStack.Script.ScriptMethods" />
public partial class DbScriptsAsync
{
    /// <summary>
    /// The synchronize
    /// </summary>
    private DbScripts sync;

    /// <summary>
    /// Gets the synchronize.
    /// </summary>
    /// <value>The synchronize.</value>
    private DbScripts Sync => this.sync ??= new DbScripts { Context = this.Context, Pages = this.Pages };

    /// <summary>
    /// Databases the select synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>System.Object.</returns>
    public object dbSelectSync(ScriptScopeContext scope, string sql) => this.Sync.dbSelect(scope, sql);

    /// <summary>
    /// Databases the select synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Object.</returns>
    public object dbSelectSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.Sync.dbSelect(scope, sql, args);

    /// <summary>
    /// Databases the select synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Object.</returns>
    public object dbSelectSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.Sync.dbSelect(scope, sql, args, options);


    /// <summary>
    /// Databases the single synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>System.Object.</returns>
    public object dbSingleSync(ScriptScopeContext scope, string sql) => this.Sync.dbSingle(scope, sql);

    /// <summary>
    /// Databases the single synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Object.</returns>
    public object dbSingleSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.Sync.dbSingle(scope, sql, args);

    /// <summary>
    /// Databases the single synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Object.</returns>
    public object dbSingleSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.Sync.dbSingle(scope, sql, args, options);


    /// <summary>
    /// Databases the scalar synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>System.Object.</returns>
    public object dbScalarSync(ScriptScopeContext scope, string sql) => this.Sync.dbScalar(scope, sql);

    /// <summary>
    /// Databases the scalar synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Object.</returns>
    public object dbScalarSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.Sync.dbScalar(scope, sql, args);

    /// <summary>
    /// Databases the scalar synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Object.</returns>
    public object dbScalarSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.Sync.dbScalar(scope, sql, args, options);


    /// <summary>
    /// Databases the count synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>System.Int64.</returns>
    public long dbCountSync(ScriptScopeContext scope, string sql) => this.Sync.dbCount(scope, sql);

    /// <summary>
    /// Databases the count synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Int64.</returns>
    public long dbCountSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.Sync.dbCount(scope, sql, args);

    /// <summary>
    /// Databases the count synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Int64.</returns>
    public long dbCountSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.Sync.dbCount(scope, sql, args, options);


    /// <summary>
    /// Databases the exists synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool dbExistsSync(ScriptScopeContext scope, string sql) => this.Sync.dbExists(scope, sql);

    /// <summary>
    /// Databases the exists synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool dbExistsSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.Sync.dbExists(scope, sql, args);

    /// <summary>
    /// Databases the exists synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool dbExistsSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.Sync.dbExists(scope, sql, args, options);


    /// <summary>
    /// Databases the execute synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>System.Int32.</returns>
    public int dbExecSync(ScriptScopeContext scope, string sql) => this.Sync.dbExec(scope, sql);

    /// <summary>
    /// Databases the execute synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Int32.</returns>
    public int dbExecSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args) => this.Sync.dbExec(scope, sql, args);

    /// <summary>
    /// Databases the execute synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Int32.</returns>
    public int dbExecSync(ScriptScopeContext scope, string sql, Dictionary<string, object> args, object options) => this.Sync.dbExec(scope, sql, args, options);

    /// <summary>
    /// Databases the table names synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> dbTableNamesSync(ScriptScopeContext scope) => this.dbTableNamesSync(scope, null, null);

    /// <summary>
    /// Databases the table names synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> dbTableNamesSync(ScriptScopeContext scope, Dictionary<string, object> args) => this.dbTableNamesSync(scope, args, null);

    /// <summary>
    /// Databases the table names synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> dbTableNamesSync(ScriptScopeContext scope, Dictionary<string, object> args, object options) => this.Sync.dbTableNames(scope, args, options);

    /// <summary>
    /// Databases the table names with row counts synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>List&lt;KeyValuePair&lt;System.String, System.Int64&gt;&gt;.</returns>
    public List<KeyValuePair<string, long>> dbTableNamesWithRowCountsSync(ScriptScopeContext scope) => this.dbTableNamesWithRowCountsSync(scope, null, null);

    /// <summary>
    /// Databases the table names with row counts synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>List&lt;KeyValuePair&lt;System.String, System.Int64&gt;&gt;.</returns>
    public List<KeyValuePair<string, long>> dbTableNamesWithRowCountsSync(ScriptScopeContext scope, Dictionary<string, object> args) => this.dbTableNamesWithRowCountsSync(scope, args, null);

    /// <summary>
    /// Databases the table names with row counts synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>List&lt;KeyValuePair&lt;System.String, System.Int64&gt;&gt;.</returns>
    public List<KeyValuePair<string, long>> dbTableNamesWithRowCountsSync(ScriptScopeContext scope, Dictionary<string, object> args, object options) => this.Sync.dbTableNamesWithRowCounts(scope, args, options);

    /// <summary>
    /// Databases the column names synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <returns>System.String[].</returns>
    public string[] dbColumnNamesSync(ScriptScopeContext scope, string tableName) => this.dbColumnNamesSync(scope, tableName, null);

    /// <summary>
    /// Databases the column names synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String[].</returns>
    public string[] dbColumnNamesSync(ScriptScopeContext scope, string tableName, object options) => this.dbColumnsSync(scope, tableName, options).Select(x => x.ColumnName).ToArray();

    /// <summary>
    /// Databases the columns synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <returns>ColumnSchema[].</returns>
    public ColumnSchema[] dbColumnsSync(ScriptScopeContext scope, string tableName) => this.dbColumnsSync(scope, tableName, null);

    /// <summary>
    /// Databases the columns synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="options">The options.</param>
    /// <returns>ColumnSchema[].</returns>
    public ColumnSchema[] dbColumnsSync(ScriptScopeContext scope, string tableName, object options) => this.Sync.dbColumns(scope, tableName, options);

    /// <summary>
    /// Databases the desc synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <returns>ColumnSchema[].</returns>
    public ColumnSchema[] dbDescSync(ScriptScopeContext scope, string sql) => this.dbDescSync(scope, sql, null);

    /// <summary>
    /// Databases the desc synchronize.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="options">The options.</param>
    /// <returns>ColumnSchema[].</returns>
    public ColumnSchema[] dbDescSync(ScriptScopeContext scope, string sql, object options) => this.Sync.dbDesc(scope, sql, options);
}