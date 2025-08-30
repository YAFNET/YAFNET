// ***********************************************************************
// <copyright file="IOrmLiteDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Interface IOrmLiteDialectProvider
/// </summary>
public interface IOrmLiteDialectProvider
{
    /// <summary>
    /// Configure Provider with connection string options
    /// </summary>
    void Init(string connectionString);

    /// <summary>
    /// Register custom value type converter
    /// </summary>
    void RegisterConverter<T>(IOrmLiteConverter converter);

    /// <summary>
    /// Used to create an OrmLiteConnection
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="namedConnection"></param>
    /// <returns></returns>
    OrmLiteConnection CreateOrmLiteConnection(OrmLiteConnectionFactory factory, string namedConnection = null);

    /// <summary>
    /// Initializes the connection.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    void InitConnection(IDbConnection dbConn);

    /// <summary>
    /// Invoked when a DB Connection is opened
    /// </summary>
    /// <value>The on open connection.</value>
    Action<IDbConnection> OnOpenConnection { get; set; }

    /// <summary>
    /// Gets or sets the on dispose connection.
    /// </summary>
    /// <value>The on dispose connection.</value>
    Action<IDbConnection> OnDisposeConnection { get; set; }

    /// <summary>
    /// Gets or sets the on before execute non query.
    /// </summary>
    /// <value>The on before execute non query.</value>
    Action<IDbCommand> OnBeforeExecuteNonQuery { get; set; }

    /// <summary>
    /// Gets or sets the on after execute non query.
    /// </summary>
    /// <value>The on after execute non query.</value>
    Action<IDbCommand> OnAfterExecuteNonQuery { get; set; }

    /// <summary>
    /// Gets or sets the execute filter.
    /// </summary>
    /// <value>The execute filter.</value>
    IOrmLiteExecFilter ExecFilter { get; set; }

    /// <summary>
    /// Gets the explicit Converter registered for a specific type
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>IOrmLiteConverter.</returns>
    IOrmLiteConverter GetConverter(Type type);

    /// <summary>
    /// Return best matching converter, falling back to Enum, Value or Ref Type Converters
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>IOrmLiteConverter.</returns>
    IOrmLiteConverter GetConverterBestMatch(Type type);

    /// <summary>
    /// Gets the converter best match.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>IOrmLiteConverter.</returns>
    IOrmLiteConverter GetConverterBestMatch(FieldDefinition fieldDef);

    /// <summary>
    /// Gets or sets the parameter string.
    /// </summary>
    /// <value>The parameter string.</value>
    string ParamString { get; set; }

    /// <summary>
    /// Escapes the wildcards.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    string EscapeWildcards(string value);

    /// <summary>
    /// Gets or sets the naming strategy.
    /// </summary>
    /// <value>The naming strategy.</value>
    INamingStrategy NamingStrategy { get; set; }

    /// <summary>
    /// Gets or sets the string serializer.
    /// </summary>
    /// <value>The string serializer.</value>
    IStringSerializer StringSerializer { get; set; }

    /// <summary>
    /// Gets or sets the parameter name filter.
    /// </summary>
    /// <value>The parameter name filter.</value>
    Func<string, string> ParamNameFilter { get; set; }

    /// <summary>
    /// Gets the variables.
    /// </summary>
    /// <value>The variables.</value>
    Dictionary<string, string> Variables { get; }

    /// <summary>
    /// Gets a value indicating whether [supports schema].
    /// </summary>
    /// <value><c>true</c> if [supports schema]; otherwise, <c>false</c>.</value>
    bool SupportsSchema { get; }

    /// <summary>
    /// Quote the string so that it can be used inside an SQL-expression
    /// Escape quotes inside the string
    /// </summary>
    /// <param name="paramValue">The parameter value.</param>
    /// <returns>System.String.</returns>
    string GetQuotedValue(string paramValue);

    /// <summary>
    /// Gets the quoted value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="fieldType">Type of the field.</param>
    /// <returns>System.String.</returns>
    string GetQuotedValue(object value, Type fieldType);

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.String.</returns>
    string GetDefaultValue(Type tableType, string fieldName);

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    string GetDefaultValue(FieldDefinition fieldDef);

    /// <summary>
    /// Determines whether [has insert return values] [the specified model definition].
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns><c>true</c> if [has insert return values] [the specified model definition]; otherwise, <c>false</c>.</returns>
    bool HasInsertReturnValues(ModelDefinition modelDef);

    /// <summary>
    /// Gets the parameter value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="fieldType">Type of the field.</param>
    /// <returns>System.Object.</returns>
    object GetParamValue(object value, Type fieldType);

    // Customize DB Parameters in SELECT or WHERE queries
    /// <summary>
    /// Initializes the query parameter.
    /// </summary>
    /// <param name="param">The parameter.</param>
    void InitQueryParam(IDbDataParameter param);

    // Customize UPDATE or INSERT DB Parameters
    /// <summary>
    /// Initializes the update parameter.
    /// </summary>
    /// <param name="param">The parameter.</param>
    void InitUpdateParam(IDbDataParameter param);

    /// <summary>
    /// Converts to dbvalue.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    object ToDbValue(object value, Type type);

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    object FromDbValue(object value, Type type);

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    object GetValue(IDataReader reader, int columnIndex, Type type);

    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="values">The values.</param>
    /// <returns>System.Int32.</returns>
    int GetValues(IDataReader reader, object[] values);

    /// <summary>
    /// Creates the connection.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="options">The options.</param>
    /// <returns>IDbConnection.</returns>
    IDbConnection CreateConnection(string filePath, Dictionary<string, string> options);

    string GetTableNameOnly(TableRef tableRef);
    string UnquotedTable(TableRef tableRef);

    /// <summary>
    /// Gets the table name with brackets.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>System.String.</returns>
    string GetTableNameWithBrackets<T>();

    /// <summary>
    /// Gets the table name with brackets.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    string GetTableNameWithBrackets(ModelDefinition modelDef);

    /// <summary>
    /// Gets the table name with brackets.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    string GetTableNameWithBrackets(string tableName, string schema = null);

    string GetQuotedTableName(Type modelType);

    string QuoteSchema(string schema, string table);
    string QuoteTable(TableRef tableRef);

    /// <summary>
    /// Gets the name of the quoted table.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    string GetQuotedTableName(ModelDefinition modelDef);

    /// <summary>
    /// Gets the name of the quoted column.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>System.String.</returns>
    string GetQuotedColumnName(string columnName);

    /// <summary>
    /// Gets the name of the quoted column.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    string GetQuotedColumnName(FieldDefinition fieldDef);

    /// <summary>
    /// Gets the name of the quoted.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string GetQuotedName(string name);

    /// <summary>
    /// Gets the name of the quoted.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    string GetQuotedName(string name, string schema);

    /// <summary>
    /// Sanitizes the name of the field name for parameter.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.String.</returns>
    string SanitizeFieldNameForParamName(string fieldName);

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    string GetColumnDefinition(FieldDefinition fieldDef);

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    string GetColumnDefinition(FieldDefinition fieldDef, ModelDefinition modelDef);

    /// <summary>
    /// Gets the last insert identifier.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>System.Int64.</returns>
    long GetLastInsertId(IDbCommand command);

    /// <summary>
    /// Gets the last insert identifier SQL suffix.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>System.String.</returns>
    string GetLastInsertIdSqlSuffix<T>();

    /// <summary>
    /// Converts to selectstatement.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>System.String.</returns>
    string ToSelectStatement(Type tableType, string sqlFilter, params object[] filterParams);

    /// <summary>
    /// Converts to selectstatement.
    /// </summary>
    /// <param name="queryType">Type of the query.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="selectExpression">The select expression.</param>
    /// <param name="bodyExpression">The body expression.</param>
    /// <param name="orderByExpression">The order by expression.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="rows">The rows.</param>
    /// <param name="tags">The tags.</param>
    /// <returns>System.String.</returns>
    string ToSelectStatement(
        QueryType queryType,
        ModelDefinition modelDef,
        string selectExpression,
        string bodyExpression,
        string orderByExpression = null,
        int? offset = null,
        int? rows = null,
        ISet<string> tags = null);

    /// <summary>
    /// Converts to insertrowsql.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>System.String.</returns>
    string ToInsertRowSql<T>(T obj, ICollection<string> insertFields = null);

    /// <summary>
    /// Converts to insertrowssql.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objs">The objs.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>System.String.</returns>
    string ToInsertRowsSql<T>(IEnumerable<T> objs, ICollection<string> insertFields = null);

    /// <summary>
    /// Bulks the insert.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db">The database.</param>
    /// <param name="objs">The objs.</param>
    /// <param name="config">The configuration.</param>
    void BulkInsert<T>(IDbConnection db, IEnumerable<T> objs, BulkInsertConfig config = null);

    /// <summary>
    /// Converts to insertrowstatement.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="objWithProperties">The object with properties.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>System.String.</returns>
    string ToInsertRowStatement(IDbCommand cmd, object objWithProperties, ICollection<string> insertFields = null);

    /// <summary>
    /// Prepares the parameterized insert statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <param name="shouldInclude">The should include.</param>
    void PrepareParameterizedInsertStatement<T>(IDbCommand cmd, ICollection<string> insertFields = null,
        Func<FieldDefinition, bool> shouldInclude = null);

    /// <summary>
    /// Prepares the parameterized update statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <returns>If had RowVersion</returns>
    bool PrepareParameterizedUpdateStatement<T>(IDbCommand cmd, ICollection<string> updateFields = null);

    /// <summary>
    /// Prepares the parameterized delete statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="deleteFieldValues">The delete field values.</param>
    /// <returns>If had RowVersion</returns>
    bool PrepareParameterizedDeleteStatement<T>(IDbCommand cmd, IDictionary<string, object> deleteFieldValues);

    /// <summary>
    /// Prepares the stored procedure statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="obj">The object.</param>
    void PrepareStoredProcedureStatement<T>(IDbCommand cmd, T obj);

    /// <summary>
    /// Sets the parameter values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    void SetParameterValues<T>(IDbCommand dbCmd, object obj);

    /// <summary>
    /// Sets the parameter.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="p">The p.</param>
    void SetParameter(FieldDefinition fieldDef, IDbDataParameter p);

    /// <summary>
    /// Enables the identity insert.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    void EnableIdentityInsert<T>(IDbCommand cmd);

    /// <summary>
    /// Enables the identity insert asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    Task EnableIdentityInsertAsync<T>(IDbCommand cmd, CancellationToken token = default);

    /// <summary>
    /// Disables the identity insert.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    void DisableIdentityInsert<T>(IDbCommand cmd);

    /// <summary>
    /// Disables the identity insert asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    Task DisableIdentityInsertAsync<T>(IDbCommand cmd, CancellationToken token = default);

    /// <summary>
    /// Enables the foreign keys check.
    /// </summary>
    /// <param name="cmd">The command.</param>
    void EnableForeignKeysCheck(IDbCommand cmd);

    /// <summary>
    /// Enables the foreign keys check asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    Task EnableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default);

    /// <summary>
    /// Disables the foreign keys check.
    /// </summary>
    /// <param name="cmd">The command.</param>
    void DisableForeignKeysCheck(IDbCommand cmd);

    /// <summary>
    /// Disables the foreign keys check asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    Task DisableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default);

    /// <summary>
    /// Gets the field definition map.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>Dictionary&lt;System.String, FieldDefinition&gt;.</returns>
    Dictionary<string, FieldDefinition> GetFieldDefinitionMap(ModelDefinition modelDef);

    /// <summary>
    /// Gets the field value.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    object GetFieldValue(FieldDefinition fieldDef, object value);

    /// <summary>
    /// Gets the field value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    object GetFieldValue(Type fieldType, object value);

    /// <summary>
    /// Prepares the update row statement.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="objWithProperties">The object with properties.</param>
    /// <param name="updateFields">The update fields.</param>
    void PrepareUpdateRowStatement(IDbCommand dbCmd, object objWithProperties, ICollection<string> updateFields = null);

    /// <summary>
    /// Prepares the update row statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    void PrepareUpdateRowStatement<T>(IDbCommand dbCmd, Dictionary<string, object> args, string sqlFilter);

    /// <summary>
    /// Prepares the upsert row statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="model">The model.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    void PrepareUpsertRowStatement<T>(IDbCommand dbCmd, T model, string sqlFilter);

    /// <summary>
    /// Prepares the update row add statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    void PrepareUpdateRowAddStatement<T>(IDbCommand dbCmd, Dictionary<string, object> args, string sqlFilter);

    /// <summary>
    /// Prepares the insert row statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="args">The arguments.</param>
    void PrepareInsertRowStatement<T>(IDbCommand dbCmd, Dictionary<string, object> args);

    /// <summary>
    /// Gets the insert columns statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>System.String.</returns>
    string GetInsertColumnsStatement<T>();

    /// <summary>
    /// Converts to deletestatement.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>System.String.</returns>
    string ToDeleteStatement(Type tableType, string sqlFilter, params object[] filterParams);

    /// <summary>
    /// Creates the parameterized delete statement.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="objWithProperties">The object with properties.</param>
    /// <returns>IDbCommand.</returns>
    IDbCommand CreateParameterizedDeleteStatement(IDbConnection connection, object objWithProperties);

    /// <summary>
    /// Converts to existstatement.
    /// </summary>
    /// <param name="fromTableType">Type of from table.</param>
    /// <param name="objWithProperties">The object with properties.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>System.String.</returns>
    string ToExistStatement(Type fromTableType,
        object objWithProperties,
        string sqlFilter,
        params object[] filterParams);

    /// <summary>
    /// Converts to selectfromprocedurestatement.
    /// </summary>
    /// <param name="fromObjWithProperties">From object with properties.</param>
    /// <param name="outputModelType">Type of the output model.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>System.String.</returns>
    string ToSelectFromProcedureStatement(object fromObjWithProperties,
        Type outputModelType,
        string sqlFilter,
        params object[] filterParams);

    /// <summary>
    /// Converts to executeprocedurestatement.
    /// </summary>
    /// <param name="objWithProperties">The object with properties.</param>
    /// <returns>System.String.</returns>
    string ToExecuteProcedureStatement(object objWithProperties);

    /// <summary>
    /// Converts to createschemastatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    string ToCreateSchemaStatement(string schema);

    /// <summary>
    /// Converts to createtablestatement.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>System.String.</returns>
    string ToCreateTableStatement(Type tableType);

    /// <summary>
    /// Converts to postcreatetablestatement.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    string ToPostCreateTableStatement(ModelDefinition modelDef);

    /// <summary>
    /// Converts to postdroptablestatement.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    string ToPostDropTableStatement(ModelDefinition modelDef);

    /// <summary>
    /// Converts to createindexstatements.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    List<string> ToCreateIndexStatements(Type tableType);

    /// <summary>
    /// Converts to createsequencestatements.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    List<string> ToCreateSequenceStatements(Type tableType);

    /// <summary>
    /// Converts to createsequencestatement.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="sequenceName">Name of the sequence.</param>
    /// <returns>System.String.</returns>
    string ToCreateSequenceStatement(Type tableType, string sequenceName);

    string ToResetSequenceStatement(Type tableType, string columnName, int value);

    /// <summary>
    /// Converts to createsavepoint.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string ToCreateSavePoint(string name);

    /// <summary>
    /// Converts to releasesavepoint.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string ToReleaseSavePoint(string name);

    /// <summary>
    /// Converts to rollbacksavepoint.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string ToRollbackSavePoint(string name);

    /// <summary>
    /// Sequences the list.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    List<string> SequenceList(Type tableType);

    /// <summary>
    /// Sequences the list asynchronous.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
    Task<List<string>> SequenceListAsync(Type tableType, CancellationToken token = default);

    /// <summary>
    /// Gets the schemas.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    List<string> GetSchemas(IDbCommand dbCmd);

    /// <summary>
    /// Gets the schema tables.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>Dictionary&lt;System.String, List&lt;System.String&gt;&gt;.</returns>
    Dictionary<string, List<string>> GetSchemaTables(IDbCommand dbCmd);

    /// <summary>
    /// Doeses the schema exist.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="schema">The schema.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool DoesSchemaExist(IDbCommand dbCmd, string schema);

    /// <summary>
    /// Doeses the schema exist asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> DoesSchemaExistAsync(IDbCommand dbCmd, string schema, CancellationToken token = default);

    /// <summary>
    /// Doeses the table exist.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool DoesTableExist(IDbConnection db, TableRef tableRef);

    /// <summary>
    /// Doeses the table exist asynchronous.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> DoesTableExistAsync(IDbConnection db, TableRef tableRef, CancellationToken token = default);

    /// <summary>
    /// Doeses the table exist.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool DoesTableExist(IDbCommand dbCmd, TableRef tableRef);

    /// <summary>
    /// Doeses the table exist asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> DoesTableExistAsync(IDbCommand dbCmd, TableRef tableRef, CancellationToken token = default);

    /// <summary>
    /// Doeses the column exist.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool DoesColumnExist(IDbConnection db, string columnName, TableRef tableRef);

    /// <summary>
    /// Doeses the column exist asynchronous.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> DoesColumnExistAsync(IDbConnection db, string columnName, TableRef tableRef,
        CancellationToken token = default);

    /// <summary>
    /// Gets the type of the column data.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns>System.String.</returns>
    string GetColumnDataType(IDbConnection db, string columnName, TableRef tableRef);

    /// <summary>
    /// Columns the is nullable.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool ColumnIsNullable(IDbConnection db, string columnName, TableRef tableRef);

    /// <summary>
    /// Gets the maximum length of the column.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns>System.Int64.</returns>
    long GetColumnMaxLength(IDbConnection db, string columnName, TableRef tableRef);

    /// <summary>
    /// Doeses the sequence exist.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sequence">Name of the sequence.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool DoesSequenceExist(IDbCommand dbCmd, string sequence);

    /// <summary>
    /// Doeses the sequence exist asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sequenceName">Name of the sequence.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> DoesSequenceExistAsync(IDbCommand dbCmd, string sequenceName, CancellationToken token = default);

    /// <summary>
    /// Froms the database row version.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    object FromDbRowVersion(Type fieldType, object value);

    /// <summary>
    /// Gets the row version select column.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="tablePrefix">The table prefix.</param>
    /// <returns>SelectItem.</returns>
    SelectItem GetRowVersionSelectColumn(FieldDefinition field, string tablePrefix = null);

    /// <summary>
    /// Gets the row version column.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="tablePrefix">The table prefix.</param>
    /// <returns>System.String.</returns>
    string GetRowVersionColumn(FieldDefinition field, string tablePrefix = null);

    /// <summary>
    /// Gets the column names.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    string GetColumnNames(ModelDefinition modelDef);

    /// <summary>
    /// Gets the column names.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="tablePrefix">The table prefix.</param>
    /// <returns>SelectItem[].</returns>
    SelectItem[] GetColumnNames(ModelDefinition modelDef, string tablePrefix);

    /// <summary>
    /// SQLs the expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    SqlExpression<T> SqlExpression<T>();

    /// <summary>
    /// Creates the parameter.
    /// </summary>
    /// <returns>IDbDataParameter.</returns>
    IDbDataParameter CreateParam();

    /// <summary>
    /// Gets the drop function.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="functionName">Name of the function.</param>
    /// <returns>System.String.</returns>
    string GetDropFunction(string database, string functionName);

    /// <summary>
    /// Gets the create view.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="selectSql">The select SQL.</param>
    /// <returns>System.String.</returns>
    string GetCreateView(string database, ModelDefinition modelDef, StringBuilder selectSql);

    /// <summary>
    /// Gets the drop view.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    string GetDropView(string database, ModelDefinition modelDef);

    /// <summary>
    /// Gets the create index view.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <param name="selectSql">The select SQL.</param>
    /// <returns>System.String.</returns>
    string GetCreateIndexView(ModelDefinition modelDef, string name, string selectSql);

    /// <summary>
    /// Gets the drop index view.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string GetDropIndexView(ModelDefinition modelDef, string name);

    /// <summary>
    /// Gets the drop index constraint.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string GetDropIndexConstraint(ModelDefinition modelDef, string name = null);

    /// <summary>
    /// Gets the add composite primary key sql command.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="fieldNameA">The field name a.</param>
    /// <param name="fieldNameB">The field name b.</param>
    /// <returns>Returns the SQL Command</returns>
    string GetAddCompositePrimaryKey(string database, ModelDefinition modelDef, string fieldNameA, string fieldNameB);

    /// <summary>
    /// Gets the name of the primary key.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>Returns the Primary Key Name</returns>
    string GetPrimaryKeyName(ModelDefinition modelDef);

    /// <summary>
    /// Gets the drop primary key constraint.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name);

    /// <summary>
    /// Gets the drop primary key constraint.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <param name="fieldNameA">The field name a.</param>
    /// <param name="fieldNameB">The field name b.</param>
    /// <returns>System.String.</returns>
    string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name, string fieldNameA,
        string fieldNameB);

    /// <summary>
    /// Gets the drop foreign key constraint.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string GetDropForeignKeyConstraint(ModelDefinition modelDef, string name);

    /// <summary>
    /// Gets the name of the constraint.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.String.</returns>
    string GetConstraintName(string database, ModelDefinition modelDef, string fieldName);

    /// <summary>
    /// Gets the drop constraint.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    string GetDropConstraint(ModelDefinition modelDef, string name);

    //DDL
    /// <summary>
    /// Gets the drop foreign key constraints.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    string GetDropForeignKeyConstraints(ModelDefinition modelDef);

    /// <summary>
    /// Converts to addcolumnstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    string ToAddColumnStatement(TableRef tableRef, FieldDefinition fieldDef);

    /// <summary>
    /// Converts to altercolumnstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    string ToAlterColumnStatement(TableRef tableRef, FieldDefinition fieldDef);

    /// <summary>
    /// Converts to changecolumnnamestatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="oldColumn">The old column.</param>
    /// <returns>System.String.</returns>
    string ToChangeColumnNameStatement(TableRef tableRef, FieldDefinition fieldDef, string oldColumn);

    /// <summary>
    /// Converts to renamecolumnstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="oldColumn">The old column.</param>
    /// <param name="newColumn">The new column.</param>
    /// <returns>System.String.</returns>
    string ToRenameColumnStatement(TableRef tableRef, string oldColumn, string newColumn);

    /// <summary>
    /// Converts to dropcolumnstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="column">The column.</param>
    /// <returns>System.String.</returns>
    string ToDropColumnStatement(TableRef tableRef, string column);

    /// <summary>
    /// Converts to dropconstraintstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="constraint">The constraint.</param>
    /// <returns>System.String.</returns>
    string ToDropConstraintStatement(TableRef tableRef, string constraint);

    /// <summary>
    /// Converts to addforeignkeystatement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TForeign">The type of the t foreign.</typeparam>
    /// <param name="field">The field.</param>
    /// <param name="foreignField">The foreign field.</param>
    /// <param name="onUpdate">The on update.</param>
    /// <param name="onDelete">The on delete.</param>
    /// <param name="foreignKeyName">Name of the foreign key.</param>
    /// <returns>System.String.</returns>
    string ToAddForeignKeyStatement<T, TForeign>(Expression<Func<T, object>> field,
        Expression<Func<TForeign, object>> foreignField,
        OnFkOption onUpdate,
        OnFkOption onDelete,
        string foreignKeyName = null);

    /// <summary>
    /// Converts to dropforeignkeystatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="foreignKeyName">Name of the foreign key.</param>
    /// <returns>System.String.</returns>
    string ToDropForeignKeyStatement(TableRef tableRef, string foreignKeyName);

    /// <summary>
    /// Converts to createindexstatement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field">The field.</param>
    /// <param name="indexName">Name of the index.</param>
    /// <param name="unique">if set to <c>true</c> [unique].</param>
    /// <returns>System.String.</returns>
    string ToCreateIndexStatement<T>(Expression<Func<T, object>> field, string indexName = null, bool unique = false);

    //Async
    /// <summary>
    /// Gets a value indicating whether [supports asynchronous].
    /// </summary>
    /// <value><c>true</c> if [supports asynchronous]; otherwise, <c>false</c>.</value>
    bool SupportsAsync { get; }

    /// <summary>
    /// Opens the asynchronous.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    Task OpenAsync(IDbConnection db, CancellationToken token = default);

    /// <summary>
    /// Executes the reader asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;IDataReader&gt;.</returns>
    Task<IDataReader> ExecuteReaderAsync(IDbCommand cmd, CancellationToken token = default);

    /// <summary>
    /// Executes the non query asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    Task<int> ExecuteNonQueryAsync(IDbCommand cmd, CancellationToken token = default);

    /// <summary>
    /// Executes the scalar asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Object&gt;.</returns>
    Task<object> ExecuteScalarAsync(IDbCommand cmd, CancellationToken token = default);

    /// <summary>
    /// Reads the asynchronous.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> ReadAsync(IDataReader reader, CancellationToken token = default);

    /// <summary>
    /// Readers the each.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="fn">The function.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
    Task<List<T>> ReaderEach<T>(IDataReader reader, Func<T> fn, CancellationToken token = default);

    /// <summary>
    /// Readers the each.
    /// </summary>
    /// <typeparam name="Return">The type of the return.</typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="fn">The function.</param>
    /// <param name="source">The source.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;Return&gt;.</returns>
    Task<Return> ReaderEach<Return>(IDataReader reader, Action fn, Return source, CancellationToken token = default);

    /// <summary>
    /// Readers the read.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="fn">The function.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;T&gt;.</returns>
    Task<T> ReaderRead<T>(IDataReader reader, Func<T> fn, CancellationToken token = default);

    /// <summary>
    /// Inserts the and get last insert identifier asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int64&gt;.</returns>
    Task<long> InsertAndGetLastInsertIdAsync<T>(IDbCommand dbCmd, CancellationToken token);

    /// <summary>
    /// Gets the load children sub select.
    /// </summary>
    /// <typeparam name="From">The type of from.</typeparam>
    /// <param name="expr">The expr.</param>
    /// <returns>System.String.</returns>
    string GetLoadChildrenSubSelect<From>(SqlExpression<From> expr);

    /// <summary>
    /// Converts to rowcountstatement.
    /// </summary>
    /// <param name="innerSql">The inner SQL.</param>
    /// <returns>System.String.</returns>
    string ToRowCountStatement(string innerSql);

    /// <summary>
    /// Converts to updatestatement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="item">The item.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <returns>System.String.</returns>
    string ToUpdateStatement<T>(IDbCommand dbCmd, T item, ICollection<string> updateFields = null);

    /// <summary>
    /// Converts to insertstatement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="item">The item.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>System.String.</returns>
    string ToInsertStatement<T>(IDbCommand dbCmd, T item, ICollection<string> insertFields = null);

    /// <summary>
    /// Merges the parameters into SQL.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="dbParams">The database parameters.</param>
    /// <returns>System.String.</returns>
    string MergeParamsIntoSql(string sql, IEnumerable<IDbDataParameter> dbParams);

    /// <summary>
    /// Gets the reference self SQL.
    /// </summary>
    /// <typeparam name="From">The type of from.</typeparam>
    /// <param name="refQ">The reference q.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="refSelf">The reference self.</param>
    /// <param name="refModelDef">The reference model definition.</param>
    /// <returns>System.String.</returns>
    string GetRefSelfSql<From>(SqlExpression<From> refQ, ModelDefinition modelDef, FieldDefinition refSelf,
        ModelDefinition refModelDef, FieldDefinition refId);

    /// <summary>
    /// Gets the reference field SQL.
    /// </summary>
    /// <param name="subSql">The sub SQL.</param>
    /// <param name="refModelDef">The reference model definition.</param>
    /// <param name="refField">The reference field.</param>
    /// <returns>System.String.</returns>
    string GetRefFieldSql(string subSql, ModelDefinition refModelDef, FieldDefinition refField);

    /// <summary>
    /// Gets the field reference SQL.
    /// </summary>
    /// <param name="subSql">The sub SQL.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="fieldRef">The field reference.</param>
    /// <returns>System.String.</returns>
    string GetFieldReferenceSql(string subSql, FieldDefinition fieldDef, FieldReference fieldRef);

    /// <summary>
    /// Converts to tablenamesstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    string ToTableNamesStatement(string schema);

    /// <summary>
    /// Return table, row count SQL for listing all tables with their row counts
    /// </summary>
    /// <param name="live">If true returns live current row counts of each table (slower), otherwise returns cached row counts from RDBMS table stats</param>
    /// <param name="schema">The table schema if any</param>
    /// <returns>System.String.</returns>
    string ToTableNamesWithRowCountsStatement(bool live, string schema);

    /// <summary>
    /// SQLs the conflict.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="conflictResolution">The conflict resolution.</param>
    /// <returns>System.String.</returns>
    string SqlConflict(string sql, string conflictResolution);

    /// <summary>
    /// SQLs the concat.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>System.String.</returns>
    string SqlConcat(IEnumerable<object> args);

    /// <summary>
    /// SQLs the currency.
    /// </summary>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <returns>System.String.</returns>
    string SqlCurrency(string fieldOrValue);

    /// <summary>
    /// SQLs the currency.
    /// </summary>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <param name="currencySymbol">The currency symbol.</param>
    /// <returns>System.String.</returns>
    string SqlCurrency(string fieldOrValue, string currencySymbol);

    /// <summary>
    /// SQLs the bool.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    /// <returns>System.String.</returns>
    string SqlBool(bool value);

    /// <summary>
    /// SQLs the limit.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="rows">The rows.</param>
    /// <returns>System.String.</returns>
    string SqlLimit(int? offset = null, int? rows = null);

    /// <summary>
    /// SQLs the cast.
    /// </summary>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <param name="castAs">The cast as.</param>
    /// <returns>System.String.</returns>
    string SqlCast(object fieldOrValue, string castAs);

    /// <summary>
    /// Gets the SQL random.
    /// </summary>
    /// <value>The SQL random.</value>
    string SqlRandom { get; }

    /// <summary>
    /// Generates a SQL comment.
    /// </summary>
    /// <param name="text">The comment text.</param>
    /// <returns>The generated SQL.</returns>
    string GenerateComment(in string text);

    /// <summary>
    /// Gets the UTC date function.
    /// </summary>
    /// <returns>System.String.</returns>
    string GetUtcDateFunction();

    /// <summary>
    /// Dates the difference function.
    /// </summary>
    /// <param name="interval">The interval.</param>
    /// <param name="date1">The date1.</param>
    /// <param name="date2">The date2.</param>
    /// <returns>System.String.</returns>
    string DateDiffFunction(string interval, string date1, string date2);

    /// <summary>
    /// Determines whether [is null function] [the specified expression].
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="alternateValue">The alternate value.</param>
    /// <returns>System.String.</returns>
    string IsNullFunction(string expression, object alternateValue);

    /// <summary>
    /// Converts the flag.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>System.String.</returns>
    string ConvertFlag(string expression);

    /// <summary>
    /// Databases the fragmentation information.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    string DatabaseFragmentationInfo(string database);

    /// <summary>
    /// Databases the size.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    string DatabaseSize(string database);

    /// <summary>
    /// SQLs the version.
    /// </summary>
    /// <returns>System.String.</returns>
    string SQLVersion();

    /// <summary>
    /// SQLs the name of the server.
    /// </summary>
    /// <returns>System.String.</returns>
    string SQLServerName();

    /// <summary>
    /// Shrinks the database.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    string ShrinkDatabase(string database);

    /// <summary>
    /// Res the index database.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="objectQualifier">The object qualifier.</param>
    /// <returns>System.String.</returns>
    string ReIndexDatabase(string database, string objectQualifier);

    /// <summary>
    /// Changes the recovery mode.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="mode">The mode.</param>
    /// <returns>System.String.</returns>
    string ChangeRecoveryMode(string database, string mode);

    /// <summary>
    /// Inners the run SQL execute reader.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>System.String.</returns>
    string InnerRunSqlExecuteReader(IDbCommand command);
}