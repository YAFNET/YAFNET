// ***********************************************************************
// <copyright file="OrmLiteReadExpressionsApi.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Class OrmLiteReadExpressionsApi.
/// </summary>
public static class OrmLiteReadExpressionsApi
{
    /// <param name="dbConn">The database connection.</param>
    extension(IDbConnection dbConn)
    {
        /// <summary>
        /// Executes the specified filter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns>T.</returns>
        public T Exec<T>(Func<IDbCommand, T> filter)
        {
            return dbConn.GetExecFilter().Exec(dbConn, filter);
        }

        /// <summary>
        /// Executes the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void Exec(Action<IDbCommand> filter)
        {
            dbConn.GetExecFilter().Exec(dbConn, filter);
        }

        /// <summary>
        /// Executes the specified filter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> Exec<T>(Func<IDbCommand, Task<T>> filter)
        {
            return dbConn.GetExecFilter().Exec(dbConn, filter);
        }

        /// <summary>
        /// Executes the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Task.</returns>
        public Task Exec(Func<IDbCommand, Task> filter)
        {
            return dbConn.GetExecFilter().Exec(dbConn, filter);
        }

        /// <summary>
        /// Executes the lazy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> ExecLazy<T>(Func<IDbCommand, IEnumerable<T>> filter)
        {
            return dbConn.GetExecFilter().ExecLazy(dbConn, filter);
        }

        /// <summary>
        /// Executes the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>IDbCommand.</returns>
        public IDbCommand Exec(Func<IDbCommand, IDbCommand> filter)
        {
            return dbConn.GetExecFilter().Exec(dbConn, filter);
        }

        /// <summary>
        /// Executes the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;IDbCommand&gt;.</returns>
        public Task<IDbCommand> Exec(Func<IDbCommand, Task<IDbCommand>> filter)
        {
            return dbConn.GetExecFilter().Exec(dbConn, filter);
        }

        /// <summary>
        /// Creates a new SqlExpression builder allowing typed LINQ-like queries.
        /// Alias for SqlExpression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>SqlExpression&lt;T&gt;.</returns>
        public SqlExpression<T> From<T>()
        {
            return dbConn.GetExecFilter().SqlExpression<T>(dbConn);
        }

        /// <summary>
        /// Froms the specified options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options">The options.</param>
        /// <returns>SqlExpression&lt;T&gt;.</returns>
        public SqlExpression<T> From<T>(Action<SqlExpression<T>> options)
        {
            var q = dbConn.GetExecFilter().SqlExpression<T>(dbConn);
            options(q);
            return q;
        }

        /// <summary>
        /// Froms the specified join expr.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="JoinWith">The type of the join with.</typeparam>
        /// <param name="joinExpr">The join expr.</param>
        /// <returns>SqlExpression&lt;T&gt;.</returns>
        public SqlExpression<T> From<T, JoinWith>(Expression<Func<T, JoinWith, bool>> joinExpr = null)
        {
            var sql = dbConn.GetExecFilter().SqlExpression<T>(dbConn);
            sql.Join<T, JoinWith>(joinExpr);
            return sql;
        }

        /// <summary>
        /// Creates a new SqlExpression builder for the specified type using a user-defined FROM sql expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromExpression">From expression.</param>
        /// <returns>SqlExpression&lt;T&gt;.</returns>
        public SqlExpression<T> From<T>(string fromExpression)
        {
            var expr = dbConn.GetExecFilter().SqlExpression<T>(dbConn);
            expr.From(fromExpression);
            return expr;
        }

        /// <summary>
        /// Froms the specified table options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableOptions">The table options.</param>
        /// <returns>SqlExpression&lt;T&gt;.</returns>
        public SqlExpression<T> From<T>(TableOptions tableOptions)
        {
            var expr = dbConn.GetExecFilter().SqlExpression<T>(dbConn);
            if (!string.IsNullOrEmpty(tableOptions.Expression))
            {
                expr.From(tableOptions.Expression);
            }

            if (!string.IsNullOrEmpty(tableOptions.Alias))
            {
                expr.SetTableAlias(tableOptions.Alias);
            }

            return expr;
        }

        /// <summary>
        /// Froms the specified table options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableOptions">The table options.</param>
        /// <param name="options">The options.</param>
        /// <returns>SqlExpression&lt;T&gt;.</returns>
        public SqlExpression<T> From<T>(TableOptions tableOptions,
            Action<SqlExpression<T>> options)
        {
            var q = dbConn.From<T>(tableOptions);
            options(q);
            return q;
        }
    }

    /// <param name="expression">The expression.</param>
    /// <typeparam name="T"></typeparam>
    extension<T>(SqlExpression<T> expression)
    {
        /// <summary>
        /// Tags the with.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>SqlExpression&lt;T&gt;.</returns>
        public SqlExpression<T> TagWith(string tag)
        {
            expression.AddTag(tag);
            return expression;
        }

        /// <summary>
        /// Tags the with call site.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>SqlExpression&lt;T&gt;.</returns>
        public SqlExpression<T> TagWithCallSite([CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            expression.AddTag($"File: {filePath}:{lineNumber}");
            return expression;
        }
    }

    /// <param name="db">The database.</param>
    extension(IDbConnection db)
    {
        /// <summary>
        /// Tables the alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>TableOptions.</returns>
        public TableOptions TableAlias(string alias)
        {
            return new TableOptions { Alias = alias };
        }

        public string GetTableName<T>()
        {
            return db.GetDialectProvider().UnquotedTable(new TableRef(ModelDefinition<T>.Definition));
        }

        public string GetTableName(Type type)
        {
            return db.GetDialectProvider().UnquotedTable(new TableRef(type));
        }

        public string GetTableName(ModelDefinition modelDef)
        {
            return db.GetDialectProvider().UnquotedTable(new TableRef(modelDef));
        }

        /// <summary>
        /// Gets the table names.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> GetTableNames()
        {
            return GetTableNames(db, null);
        }

        /// <summary>
        /// Gets the table names.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> GetTableNames(string schema)
        {
            return db.Column<string>(db.GetDialectProvider().ToTableNamesStatement(schema));
        }

        /// <summary>
        /// Gets the table names asynchronous.
        /// </summary>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
        public Task<List<string>> GetTableNamesAsync()
        {
            return GetTableNamesAsync(db, null);
        }

        /// <summary>
        /// Gets the table names asynchronous.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
        public Task<List<string>> GetTableNamesAsync(string schema)
        {
            return db.ColumnAsync<string>(db.GetDialectProvider().ToTableNamesStatement(schema));
        }

        /// <summary>
        /// Gets the table names with row counts.
        /// </summary>
        /// <param name="live">if set to <c>true</c> [live].</param>
        /// <param name="schema">The schema.</param>
        /// <returns>List&lt;KeyValuePair&lt;System.String, System.Int64&gt;&gt;.</returns>
        public List<KeyValuePair<string, long>> GetTableNamesWithRowCounts(bool live = false,
            string schema = null)
        {
            List<KeyValuePair<string, long>> GetResults()
            {
                var sql = db.GetDialectProvider().ToTableNamesWithRowCountsStatement(live, schema);
                if (sql != null)
                {
                    return db.KeyValuePairs<string, long>(sql);
                }

                sql = CreateTableRowCountUnionSql(db, schema);
                return db.KeyValuePairs<string, long>(sql);
            }

            var results = GetResults();
            results.Sort((x, y) => y.Value.CompareTo(x.Value)); //sort desc
            return results;
        }

        /// <summary>
        /// Get table names with row counts as an asynchronous operation.
        /// </summary>
        /// <param name="live">if set to <c>true</c> [live].</param>
        /// <param name="schema">The schema.</param>
        /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
        public async Task<List<KeyValuePair<string, long>>> GetTableNamesWithRowCountsAsync(bool live = false,
            string schema = null)
        {
            Task<List<KeyValuePair<string, long>>> GetResultsAsync()
            {
                var sql = db.GetDialectProvider().ToTableNamesWithRowCountsStatement(live, schema);
                if (sql != null)
                {
                    return db.KeyValuePairsAsync<string, long>(sql);
                }

                sql = CreateTableRowCountUnionSql(db, schema);
                return db.KeyValuePairsAsync<string, long>(sql);
            }

            var results = await GetResultsAsync().ConfigAwait();
            results.Sort((x, y) => y.Value.CompareTo(x.Value)); //sort desc
            return results;
        }
    }

    /// <summary>
    /// Creates the table row count union SQL.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    private static string CreateTableRowCountUnionSql(IDbConnection db, string schema)
    {
        var sb = StringBuilderCache.Allocate();

        var dialect = db.GetDialectProvider();

        var tableNames = GetTableNames(db, schema);
        var schemaName = dialect.NamingStrategy.GetSchemaName(schema);
        foreach (var tableName in tableNames)
        {
            if (sb.Length > 0)
            {
                sb.Append(" UNION ");
            }

            // retain *real* table names and skip using naming strategy
            sb.AppendLine($"SELECT {dialect.GetQuotedValue(tableName)}, COUNT(*) FROM {dialect.QuoteSchema(schemaName, tableName)}");
        }

        var sql = StringBuilderCache.ReturnAndFree(sb);
        return sql;
    }

    /// <param name="db">The database.</param>
    extension(IDbConnection db)
    {
        /// <summary>
        /// Gets the name of the quoted table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.String.</returns>
        public string GetQuotedTableName<T>()
        {
            return db.GetDialectProvider().GetQuotedTableName(ModelDefinition<T>.Definition);
        }

        /// <summary>
        /// Open a Transaction in OrmLite
        /// </summary>
        /// <returns>IDbTransaction.</returns>
        public IDbTransaction OpenTransaction()
        {
            return OrmLiteTransaction.Create(db);
        }

        /// <summary>
        /// Returns a new transaction if not yet exists, otherwise null
        /// </summary>
        /// <returns>IDbTransaction.</returns>
        public IDbTransaction OpenTransactionIfNotExists()
        {
            return !db.InTransaction() ? OrmLiteTransaction.Create(db) : null;
        }

        /// <summary>
        /// Open a Transaction in OrmLite
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>IDbTransaction.</returns>
        public IDbTransaction OpenTransaction(IsolationLevel isolationLevel)
        {
            return OrmLiteTransaction.Create(db, isolationLevel);
        }

        /// <summary>
        /// Returns a new transaction if not yet exists, otherwise null
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>IDbTransaction.</returns>
        public IDbTransaction OpenTransactionIfNotExists(IsolationLevel isolationLevel)
        {
            return !db.InTransaction()
                ? OrmLiteTransaction.Create(db, isolationLevel)
                : null;
        }
    }

    /// <param name="trans">The transaction.</param>
    extension(IDbTransaction trans)
    {
        /// <summary>
        /// Create a SavePoint
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>SavePoint.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public SavePoint SavePoint(string name)
        {
            if (trans is not OrmLiteTransaction dbTrans)
            {
                throw new ArgumentException($"{trans.GetType().Name} is not an OrmLiteTransaction. Use db.OpenTransaction() to Create OrmLite Transactions");
            }

            var savePoint = new SavePoint(dbTrans, name);
            savePoint.Save();
            return savePoint;
        }

        /// <summary>
        /// Create a SavePoint
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>SavePoint.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public async Task<SavePoint> SavePointAsync(string name)
        {
            if (trans is not OrmLiteTransaction dbTrans)
            {
                throw new ArgumentException($"{trans.GetType().Name} is not an OrmLiteTransaction. Use db.OpenTransaction() to Create OrmLite Transactions");
            }

            var savePoint = new SavePoint(dbTrans, name);
            await savePoint.SaveAsync().ConfigAwait();
            return savePoint;
        }
    }

    /// <param name="dbConn">The database connection.</param>
    extension(IDbConnection dbConn)
    {
        /// <summary>
        /// Create a managed OrmLite IDbCommand
        /// </summary>
        /// <returns>IDbCommand.</returns>
        public IDbCommand OpenCommand()
        {
            return dbConn.GetExecFilter().CreateCommand(dbConn);
        }

        /// <summary>
        /// Returns results from using a LINQ Expression. E.g:
        /// <para>db.Select&lt;Person&gt;(x =&gt; x.Age &gt; 40)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Select<T>(Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select(predicate));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Select(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Select<T>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select(expression));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Select(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> Select<T>(ISqlExpression expression, object anonType = null)
        {
            if (anonType != null)
            {
                return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(expression.SelectInto<T>(QueryType.Select), anonType));
            }

            if (expression.Params != null && expression.Params.Count != 0)
            {
                return dbConn.Exec(
                    dbCmd => dbCmd.SqlList<T>(
                        expression.SelectInto<T>(QueryType.Select),
                        expression.Params.ToDictionary(param => param.ParameterName, param => param.Value)));
            }

            return dbConn.Exec(
                dbCmd => dbCmd.SqlList<T>(expression.SelectInto<T>(QueryType.Select), expression.Params));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;Tuple&lt;T, T2&gt;&gt;.</returns>
        public List<Tuple<T, T2>> SelectMulti<T, T2>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2>(expression));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3>> SelectMulti<T, T2, T3>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3>(expression));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3, T4>> SelectMulti<T, T2, T3, T4>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3, T4>(expression));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3, T4, T5>> SelectMulti<T, T2, T3, T4, T5>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3, T4, T5>(expression));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3, T4, T5, T6>> SelectMulti<T, T2, T3, T4, T5, T6>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3, T4, T5, T6>(expression));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6, T7&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3, T4, T5, T6, T7>> SelectMulti<T, T2, T3, T4, T5, T6, T7>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3, T4, T5, T6, T7>(expression));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <typeparam name="T8">The type of the t8.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6, T7, T8&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3, T4, T5, T6, T7, T8>> SelectMulti<T, T2, T3, T4, T5, T6, T7, T8>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3, T4, T5, T6, T7, T8>(expression));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2&gt;&gt;.</returns>
        public List<Tuple<T, T2>> SelectMulti<T, T2>(SqlExpression<T> expression,
            string[] tableSelects)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2>(expression, tableSelects));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3>> SelectMulti<T, T2, T3>(SqlExpression<T> expression,
            string[] tableSelects)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3>(expression, tableSelects));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3, T4>> SelectMulti<T, T2, T3, T4>(SqlExpression<T> expression,
            string[] tableSelects)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3, T4>(expression, tableSelects));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3, T4, T5>> SelectMulti<T, T2, T3, T4, T5>(SqlExpression<T> expression,
            string[] tableSelects)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3, T4, T5>(expression, tableSelects));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3, T4, T5, T6>> SelectMulti<T, T2, T3, T4, T5, T6>(SqlExpression<T> expression,
            string[] tableSelects)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3, T4, T5, T6>(expression, tableSelects));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6, T7&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3, T4, T5, T6, T7>> SelectMulti<T, T2, T3, T4, T5, T6, T7>(SqlExpression<T> expression,
            string[] tableSelects)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3, T4, T5, T6, T7>(expression, tableSelects));
        }

        /// <summary>
        /// Selects the multi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <typeparam name="T8">The type of the t8.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="tableSelects">The table selects.</param>
        /// <returns>List&lt;Tuple&lt;T, T2, T3, T4, T5, T6, T7, T8&gt;&gt;.</returns>
        public List<Tuple<T, T2, T3, T4, T5, T6, T7, T8>> SelectMulti<T, T2, T3, T4, T5, T6, T7, T8>(SqlExpression<T> expression,
            string[] tableSelects)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectMulti<T, T2, T3, T4, T5, T6, T7, T8>(expression, tableSelects));
        }

        /// <summary>
        /// Returns a single result from using a LINQ Expression. E.g:
        /// <para>db.Single&lt;Person&gt;(x =&gt; x.Age == 42)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns>T.</returns>
        public T Single<T>(Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single(predicate));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Select&lt;Person&gt;(x =&gt; x.Age &gt; 40)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>T.</returns>
        public T Single<T>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single(expression));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Single(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>T.</returns>
        public T Single<T>(ISqlExpression expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single<T>(expression.SelectInto<T>(QueryType.Single), expression.Params));
        }

        /// <summary>
        /// Returns a scalar result from using an SqlExpression lambda. E.g:
        /// <para>db.Scalar&lt;Person, int&gt;(x =&gt; Sql.Max(x.Age))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="field">The field.</param>
        /// <returns>TKey.</returns>
        public TKey Scalar<T, TKey>(Expression<Func<T, object>> field)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar<T, TKey>(field));
        }

        /// <summary>
        /// Returns a scalar result from using an SqlExpression lambda. E.g:
        /// <para>db.Scalar&lt;Person, int&gt;(x =&gt; Sql.Max(x.Age), , x =&gt; x.Age &lt; 50)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>TKey.</returns>
        public TKey Scalar<T, TKey>(Expression<Func<T, object>> field,
            Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar<T, TKey>(field, predicate));
        }

        /// <summary>
        /// Returns the count of rows that match the LINQ expression, E.g:
        /// <para>db.Count&lt;Person&gt;(x =&gt; x.Age &lt; 50)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Int64.</returns>
        public long Count<T>(Expression<Func<T, bool>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Count(expression));
        }

        /// <summary>
        /// Returns the count of rows that match the supplied SqlExpression, E.g:
        /// <para>db.Count(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Int64.</returns>
        public long Count<T>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Count(expression));
        }

        /// <summary>
        /// Counts the specified database connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.Int64.</returns>
        public long Count<T>()
        {
            var expression = dbConn.GetDialectProvider().SqlExpression<T>();
            return dbConn.Exec(dbCmd => dbCmd.Count(expression));
        }

        /// <summary>
        /// Return the number of rows returned by the supplied expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Int64.</returns>
        public long RowCount<T>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.RowCount(expression));
        }

        /// <summary>
        /// Return the number of rows returned by the supplied sql
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>System.Int64.</returns>
        public long RowCount(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.RowCount(sql, anonType));
        }

        /// <summary>
        /// Return the number of rows returned by the supplied sql and db params
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns>System.Int64.</returns>
        public long RowCount(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.RowCount(sql, sqlParams));
        }

        /// <summary>
        /// Return the number of rows in the specified table
        /// </summary>
        public long RowCount<T>()
        {
            return dbConn.SqlScalar<long>(dbConn.From<T>().Select(Sql.Count("*")));
        }

        /// <summary>
        /// Returns results with references from using a LINQ Expression. E.g:
        /// <para>db.LoadSelect&lt;Person&gt;(x =&gt; x.Age &gt; 40)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> LoadSelect<T>(Expression<Func<T, bool>> predicate,
            string[] include = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(predicate, include));
        }

        /// <summary>
        /// Returns results with references from using a LINQ Expression. E.g:
        /// <para>db.LoadSelect&lt;Person&gt;(x =&gt; x.Age &gt; 40, include: x =&gt; new { x.PrimaryAddress })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> LoadSelect<T>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(predicate, include.GetFieldNames()));
        }

        /// <summary>
        /// Returns results with references from using an SqlExpression lambda. E.g:
        /// <para>db.LoadSelect(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> LoadSelect<T>(SqlExpression<T> expression = null,
            string[] include = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(expression, include));
        }

        /// <summary>
        /// Returns results with references from using an SqlExpression lambda. E.g:
        /// <para>db.LoadSelect(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40), include:q.OnlyFields)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> LoadSelect<T>(SqlExpression<T> expression,
            IEnumerable<string> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(expression, include));
        }

        /// <summary>
        /// Returns results with references from using an SqlExpression lambda. E.g:
        /// <para>db.LoadSelect(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40), include: x =&gt; new { x.PrimaryAddress })</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> LoadSelect<T>(SqlExpression<T> expression,
            Expression<Func<T, object>> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(expression, include.GetFieldNames()));
        }

        /// <summary>
        /// Project results with references from a number of joined tables into a different model
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;Into&gt;.</returns>
        public List<Into> LoadSelect<Into, From>(SqlExpression<From> expression,
            string[] include = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect<Into, From>(expression, include));
        }

        /// <summary>
        /// Project results with references from a number of joined tables into a different model
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;Into&gt;.</returns>
        public List<Into> LoadSelect<Into, From>(SqlExpression<From> expression,
            IEnumerable<string> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect<Into, From>(expression, include));
        }

        /// <summary>
        /// Project results with references from a number of joined tables into a different model
        /// </summary>
        /// <typeparam name="Into">The type of the into.</typeparam>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="include">The include.</param>
        /// <returns>List&lt;Into&gt;.</returns>
        public List<Into> LoadSelect<Into, From>(SqlExpression<From> expression,
            Expression<Func<Into, object>> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect<Into, From>(expression, include.GetFieldNames()));
        }

        /// <summary>
        /// Return ADO.NET reader.GetSchemaTable() in a DataTable
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>DataTable.</returns>
        public DataTable GetSchemaTable(string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetSchemaTable(sql));
        }

        /// <summary>
        /// Get Table Column Schemas for specified table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ColumnSchema[].</returns>
        public ColumnSchema[] GetTableColumns<T>()
        {
            return dbConn.Exec(dbCmd => dbCmd.GetTableColumns(typeof(T)));
        }

        /// <summary>
        /// Get Table Column Schemas for specified table
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ColumnSchema[].</returns>
        public ColumnSchema[] GetTableColumns(Type type)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetTableColumns(type));
        }

        /// <summary>
        /// Get Table Column Schemas for result-set return from specified sql
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>ColumnSchema[].</returns>
        public ColumnSchema[] GetTableColumns(string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetTableColumns(sql));
        }

        /// <summary>
        /// Enables the foreign keys check.
        /// </summary>
        public void EnableForeignKeysCheck()
        {
            dbConn.Exec(dbConn.GetDialectProvider().EnableForeignKeysCheck);
        }

        /// <summary>
        /// Disables the foreign keys check.
        /// </summary>
        public void DisableForeignKeysCheck()
        {
            dbConn.Exec(dbConn.GetDialectProvider().DisableForeignKeysCheck);
        }
    }
}