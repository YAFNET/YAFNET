﻿// ***********************************************************************
// <copyright file="SqliteOrmLiteDialectProviderBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite.Sqlite;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.OrmLite.Sqlite.Converters;

/// <summary>
/// Class SqliteOrmLiteDialectProviderBase.
/// Implements the <see cref="SqliteOrmLiteDialectProviderBase" />
/// </summary>
/// <seealso cref="SqliteOrmLiteDialectProviderBase" />
public abstract class SqliteOrmLiteDialectProviderBase : OrmLiteDialectProviderBase<SqliteOrmLiteDialectProviderBase>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteOrmLiteDialectProviderBase"/> class.
    /// </summary>
    protected SqliteOrmLiteDialectProviderBase()
    {
        base.SelectIdentitySql = "SELECT last_insert_rowid()";

        base.InitColumnTypeMap();

        OrmLiteConfig.DeoptimizeReader = true;
        base.RegisterConverter<DateTime>(new SqliteCoreDateTimeConverter());
        //Old behavior using native sqlite3.dll
        //base.RegisterConverter<DateTime>(new SqliteNativeDateTimeConverter());

        base.RegisterConverter<string>(new SqliteStringConverter());
        base.RegisterConverter<DateTimeOffset>(new SqliteDateTimeOffsetConverter());
        base.RegisterConverter<Guid>(new SqliteGuidConverter());
        base.RegisterConverter<bool>(new SqliteBoolConverter());
        base.RegisterConverter<byte[]>(new SqliteByteArrayConverter());

        this.Variables = new Dictionary<string, string>
                             {
                                 { OrmLiteVariables.SystemUtc, "CURRENT_TIMESTAMP" },
                                 { OrmLiteVariables.MaxText, "VARCHAR(1000000)" },
                                 { OrmLiteVariables.MaxTextUnicode, "NVARCHAR(1000000)" },
                                 { OrmLiteVariables.True, SqlBool(true) },
                                 { OrmLiteVariables.False, SqlBool(false) }
                             };
    }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    public static string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [ut f8 encoded].
    /// </summary>
    /// <value><c>true</c> if [ut f8 encoded]; otherwise, <c>false</c>.</value>
    public static bool UTF8Encoded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [parse via framework].
    /// </summary>
    /// <value><c>true</c> if [parse via framework]; otherwise, <c>false</c>.</value>
    public static bool ParseViaFramework { get; set; }

    public override bool SupportsSchema => false;

    /// <summary>
    /// Converts to insertrowssql.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objs">The objs.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>System.String.</returns>
    public override string ToInsertRowsSql<T>(IEnumerable<T> objs, ICollection<string> insertFields = null)
    {
        var modelDef = ModelDefinition<T>.Definition;
        var sb = StringBuilderCache.Allocate()
            .Append($"INSERT INTO {GetQuotedTableName(modelDef)} (");

        var fieldDefs = GetInsertFieldDefinitions(modelDef);
        var i = 0;
        foreach (var fieldDef in fieldDefs)
        {
            if (ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
                continue;

            if (i++ > 0)
                sb.Append(",");

            sb.Append(GetQuotedColumnName(fieldDef.FieldName));
        }

        sb.Append(") VALUES");

        var count = 0;
        foreach (var obj in objs)
        {
            count++;
            sb.AppendLine();
            sb.Append('(');
            i = 0;
            foreach (var fieldDef in fieldDefs)
            {
                if (ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
                    continue;

                if (i++ > 0)
                    sb.Append(',');

                AppendInsertRowValueSql(sb, fieldDef, obj);
            }

            sb.Append("),");
        }

        if (count == 0)
        {
            return "";
        }

        sb.Length--;
        sb.AppendLine(";");
        var sql = StringBuilderCache.ReturnAndFree(sb);
        return sql;
    }

    /// <summary>
    /// The row version trigger format
    /// </summary>
    public static string RowVersionTriggerFormat = "{0}RowVersionUpdateTrigger";

    public bool EnableForeignKeys
    {
        get => ConnectionCommands.Contains(SqlitePragmas.EnableForeignKeys);
        set
        {
            if (value)
                ConnectionCommands.AddIfNotExists(SqlitePragmas.EnableForeignKeys);
            else
                ConnectionCommands.Remove(SqlitePragmas.DisableForeignKeys);
        }
    }

    /// <summary>
    /// Converts to postdroptablestatement.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public override string ToPostDropTableStatement(ModelDefinition modelDef)
    {
        if (modelDef.RowVersion == null)
        {
            return null;
        }

        var triggerName = GetTriggerName(modelDef);
        return $"DROP TRIGGER IF EXISTS {GetQuotedName(triggerName)}";
    }

    /// <summary>
    /// Gets the name of the trigger.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    private string GetTriggerName(ModelDefinition modelDef)
    {
        return RowVersionTriggerFormat.Fmt(GetTableName(modelDef));
    }

    /// <summary>
    /// Gets the type of the column data.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDataType(
        IDbConnection db,
        string columnName,
        string tableName,
        string schema = null)
    {
        var sql =
            "select type from pragma_table_info(@tableName) where name= @columnName;"
                .SqlFmt(tableName, columnName);

        if (schema != null)
        {
            sql += " AND TABLE_SCHEMA = @schema";
        }

        return db.SqlScalar<string>(sql, new { tableName, columnName, schema });
    }

    /// <summary>
    /// Converts to postcreatetablestatement.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public override string ToPostCreateTableStatement(ModelDefinition modelDef)
    {
        if (modelDef.RowVersion == null)
        {
            return null;
        }

        var triggerName = GetTriggerName(modelDef);
        var tableName = GetTableName(modelDef);
        var triggerBody = string.Format("UPDATE {0} SET {1} = OLD.{1} + 1 WHERE {2} = NEW.{2};",
            tableName,
            modelDef.RowVersion.FieldName.SqlColumn(this),
            modelDef.PrimaryKey.FieldName.SqlColumn(this));

        var sql = $"CREATE TRIGGER {triggerName} BEFORE UPDATE ON {tableName} FOR EACH ROW BEGIN {triggerBody} END;";

        return sql;

    }

    /// <summary>
    /// Creates the full text create table statement.
    /// </summary>
    /// <param name="objectWithProperties">The object with properties.</param>
    /// <returns>System.String.</returns>
    public static string CreateFullTextCreateTableStatement(object objectWithProperties)
    {
        var sbColumns = StringBuilderCache.Allocate();

        foreach (var propertyInfo in objectWithProperties.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var columnDefinition = sbColumns.Length == 0
                                       ? $"{propertyInfo.Name} TEXT PRIMARY KEY"
                                       : $", {propertyInfo.Name} TEXT";

            sbColumns.AppendLine(columnDefinition);
        }

        var tableName = objectWithProperties.GetType().Name;
        var sql = $"CREATE VIRTUAL TABLE \"{tableName}\" USING FTS3 ({StringBuilderCache.ReturnAndFree(sbColumns)});";

        return sql;
    }

    /// <summary>
    /// Creates the connection.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="options">The options.</param>
    /// <returns>IDbConnection.</returns>
    public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
    {
        if (connectionString == "DataSource=:memory:")
        {
            connectionString = ":memory:";
        }

        var isFullConnectionString = connectionString.Contains(";");
        var connString = StringBuilderCache.Allocate();
        if (!isFullConnectionString)
        {
            if (connectionString != ":memory:")
            {
                var existingDir = Path.GetDirectoryName(connectionString);
                if (!string.IsNullOrEmpty(existingDir) && !Directory.Exists(existingDir))
                {
                    Directory.CreateDirectory(existingDir);
                }
            }
            connString.AppendFormat("Data Source={0};", connectionString.Trim());
        }
        else
        {
            connString.Append(connectionString);
        }
        if (!string.IsNullOrEmpty(Password))
        {
            connString.AppendFormat("Password={0};", Password);
        }
        if (UTF8Encoded)
        {
            connString.Append("UseUTF16Encoding=True;");
        }

        if (options != null)
        {
            foreach (var option in options)
            {
                connString.AppendFormat("{0}={1};", option.Key, option.Value);
            }
        }

        ConnectionStringFilter?.Invoke(connString);

        return CreateConnection(StringBuilderCache.ReturnAndFree(connString));
    }

    /// <summary>
    /// Gets or sets the connection string filter.
    /// </summary>
    /// <value>The connection string filter.</value>
    public Action<StringBuilder> ConnectionStringFilter { get; set; }

    /// <summary>
    /// Creates the connection.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>IDbConnection.</returns>
    protected abstract IDbConnection CreateConnection(string connectionString);

    /// <summary>
    /// Gets the name of the quoted.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    public override string GetQuotedName(string name, string schema) => GetQuotedName(name); //schema name is embedded in table name in MySql

    /// <summary>
    /// Converts to tablenamesstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    public override string ToTableNamesStatement(string schema)
    {
        return schema == null
                   ? "SELECT name FROM sqlite_master WHERE type ='table' AND name NOT LIKE 'sqlite_%'"
                   : "SELECT name FROM sqlite_master WHERE type ='table' AND name LIKE {0}".SqlFmt(this, GetTableName("",schema) + "%");
    }

    /// <summary>
    /// Gets the name of the schema.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    public override string GetSchemaName(string schema)
    {
        return schema != null
                   ? NamingStrategy.GetSchemaName(schema).Replace(".", "_")
                   : NamingStrategy.GetSchemaName(schema);
    }

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    public override string GetTableName(string table, string schema = null) =>
        GetTableName(table, schema, useStrategy: true);

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="useStrategy">if set to <c>true</c> [use strategy].</param>
    /// <returns>System.String.</returns>
    public override string GetTableName(string table, string schema, bool useStrategy)
    {
        if (useStrategy)
        {
            return schema != null && !table.StartsWithIgnoreCase(schema + "_")
                       ? $"{NamingStrategy.GetSchemaName(schema)}_{NamingStrategy.GetTableName(table)}"
                       : NamingStrategy.GetTableName(table);
        }

        return schema != null && !table.StartsWithIgnoreCase(schema + "_")
                   ? $"{schema}_{table}"
                   : table;
    }

    /// <summary>
    /// Gets the name of the quoted table.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    public override string GetQuotedTableName(string tableName, string schema = null) =>
        GetQuotedName(GetTableName(tableName, schema));

    /// <summary>
    /// SQLs the expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public override SqlExpression<T> SqlExpression<T>() => new SqliteExpression<T>(this);

    public override Dictionary<string, List<string>> GetSchemaTables(IDbCommand dbCmd)
    {
        return new Dictionary<string, List<string>>
        {
            ["default"] = dbCmd.SqlColumn<string>("SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%'")
        };
    }

    /// <summary>
    /// Doeses the schema exist.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="schemaName">Name of the schema.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="System.NotImplementedException">Schemas are not supported by sqlite</exception>
    public override bool DoesSchemaExist(IDbCommand dbCmd, string schemaName) => false;

    /// <summary>
    /// Converts to createschemastatement.
    /// </summary>
    /// <param name="schemaName">Name of the schema.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.NotImplementedException">Schemas are not supported by sqlite</exception>
    public override string ToCreateSchemaStatement(string schemaName)
    {
        throw new NotImplementedException("Schemas are not supported by sqlite");
    }

    /// <summary>
    /// Doeses the table exist.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="schema">The schema.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool DoesTableExist(IDbCommand dbCmd, string tableName, string schema = null)
    {
        var sql = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name = {0}"
            .SqlFmt(tableName);

        dbCmd.CommandText = sql;
        var result = dbCmd.LongScalar();

        return result > 0;
    }

    /// <summary>
    /// Doeses the column exist.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="schema">The schema.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool DoesColumnExist(IDbConnection db, string columnName, string tableName, string schema = null)
    {
        var sql = "PRAGMA table_info({0})"
            .SqlFmt(tableName);

        var columns = db.SqlList<Dictionary<string, object>>(sql);
        foreach (var column in columns)
        {
            if (column.TryGetValue("name", out var name) && name.ToString().EqualsIgnoreCase(columnName))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDefinition(FieldDefinition fieldDef)
    {
        // http://www.sqlite.org/lang_createtable.html#rowid
        var ret = base.GetColumnDefinition(fieldDef);
        if (fieldDef.IsPrimaryKey)
            return ret.Replace(" BIGINT ", " INTEGER ");
        if (fieldDef.IsRowVersion)
            return ret + " DEFAULT 1";

        return ret;
    }

    /// <summary>
    /// SQLs the conflict.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="conflictResolution">The conflict resolution.</param>
    /// <returns>System.String.</returns>
    public override string SqlConflict(string sql, string conflictResolution)
    {
        // http://www.sqlite.org/lang_conflict.html
        var parts = sql.SplitOnFirst(' ');
        return parts[0] + " OR " + conflictResolution + " " + parts[1];
    }

    /// <summary>
    /// SQLs the concat.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>System.String.</returns>
    public override string SqlConcat(IEnumerable<object> args) => string.Join(" || ", args);

    /// <summary>
    /// SQLs the currency.
    /// </summary>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <param name="currencySymbol">The currency symbol.</param>
    /// <returns>System.String.</returns>
    public override string SqlCurrency(string fieldOrValue, string currencySymbol) => SqlConcat(["'" + currencySymbol + "'", "printf(\"%.2f\", " + fieldOrValue + ")"
    ]);

    /// <summary>
    /// SQLs the bool.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    /// <returns>System.String.</returns>
    public override string SqlBool(bool value) => value ? "1" : "0";

    /// <summary>
    /// Gets the SQL random.
    /// </summary>
    /// <value>The SQL random.</value>
    public override string SqlRandom => "random()";

    /// <summary>
    /// Gets the drop function.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="functionName">Name of the function.</param>
    /// <returns>System.String.</returns>
    public override string GetDropFunction(string database, string functionName)
    {
        return string.Empty; // Not Supported in Sqlite
    }

    /// <summary>Gets the name of the primary key.</summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>Returns the Primary Key Name</returns>
    public override string GetPrimaryKeyName(ModelDefinition modelDef)
    {
        return $"{this.NamingStrategy.GetTableName(modelDef).ToUpper()}_PK";
    }

    /// <summary>Gets the drop primary key constraint.</summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <param name="fieldNameA">The field name a.</param>
    /// <param name="fieldNameB">The field name b.</param>
    /// <returns>System.String.</returns>
    public override string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name, string fieldNameA, string fieldNameB)
    {
        return string.Empty; // Not Supported in Sqlite
    }

    /// <summary>
    /// Gets the create view.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="selectSql">The select SQL.</param>
    /// <returns>System.String.</returns>
    public override string GetCreateView(string database, ModelDefinition modelDef, StringBuilder selectSql)
    {
        var sb = StringBuilderCache.Allocate();

        var tableName = $"{base.NamingStrategy.GetTableName(modelDef.ModelName)}";

        sb.AppendFormat("CREATE VIEW {0} as ", tableName);

        sb.Append(selectSql);

        sb.Append(";");

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Gets the drop view.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public override string GetDropView(string database, ModelDefinition modelDef)
    {
        var sb = StringBuilderCache.Allocate();

        var tableName = $"{database}.{base.NamingStrategy.GetTableName(modelDef.ModelName)}";

        sb.Append("DROP VIEW IF EXISTS ");
        sb.AppendFormat("{0}", tableName);

        sb.Append(";");

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Gets the UTC date function.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string GetUtcDateFunction()
    {
        return "datetime('now')";
    }

    /// <summary>
    /// Dates the difference function.
    /// </summary>
    /// <param name="interval">The interval.</param>
    /// <param name="date1">The date1.</param>
    /// <param name="date2">The date2.</param>
    /// <returns>System.String.</returns>
    public override string DateDiffFunction(string interval, string date1, string date2)
    {
        return interval == "minute"
                   ? $@"((JulianDay({date2}) - JulianDay({date1})) * 24 * 60)"
                   : $@"(JulianDay({date2}) - JulianDay({date1}))";
    }

    /// <summary>
    /// Gets the SQL ISNULL Function
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="alternateValue">The alternate Value.</param>
    /// <returns>The <see cref="string" />.</returns>
    public override string IsNullFunction(string expression, object alternateValue)
    {
        return $"IFNULL(({expression}), {alternateValue})";
    }

    /// <summary>
    /// Converts the flag.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>System.String.</returns>
    public override string ConvertFlag(string expression)
    {
        var value = expression.Substring(expression.Length - 1, 1);

        return $"CAST(({expression} = {value}) as integer)";
    }

    /// <summary>
    /// Databases the fragmentation information.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    public override string DatabaseFragmentationInfo(string database)
    {
        return string.Empty;// NOT SUPPORTED
    }

    /// <summary>
    /// Databases the size.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    public override string DatabaseSize(string database)
    {
        return "SELECT (page_count * page_size) / 1048576.0 as size FROM pragma_page_count(), pragma_page_size();";
    }

    /// <summary>
    /// SQLs the version.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string SQLVersion()
    {
        return "select sqlite_version()";
    }

    /// <summary>
    /// SQLs the name of the server.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string SQLServerName()
    {
        return "SQLite";
    }

    /// <summary>
    /// Shrinks the database.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    public override string ShrinkDatabase(string database)
    {
        return "PRAGMA auto_vacuum = FULL;";
    }

    /// <summary>
    /// Res the index database.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="objectQualifier">The object qualifier.</param>
    /// <returns>System.String.</returns>
    public override string ReIndexDatabase(string database, string objectQualifier)
    {
        return "PRAGMA auto_vacuum = INCREMENTAL;";
    }

    /// <summary>
    /// Changes the recovery mode.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="mode">The mode.</param>
    /// <returns>System.String.</returns>
    public override string ChangeRecoveryMode(string database, string mode)
    {
        return string.Empty;// NOT SUPPORTED
    }

    /// <summary>
    /// Just runs the SQL command according to specifications.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>Returns the Results</returns>
    public override string InnerRunSqlExecuteReader(IDbCommand command)
    {
        var sqlCommand = command as SQLiteCommand;

        SQLiteDataReader reader = null;
        var results = new StringBuilder();

        try
        {
            try
            {
                reader = sqlCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    var rowIndex = 1;
                    var columnNames = reader.GetSchemaTable().Rows.Cast<DataRow>()
                        .Select(r => r["ColumnName"].ToString()).ToList();

                    results.Append("RowNumber");

                    columnNames.ForEach(
                        n =>
                            {
                                results.Append(",");
                                results.Append(n);
                            });

                    results.AppendLine();

                    while (reader.Read())
                    {
                        results.AppendFormat(@"""{0}""", rowIndex++);

                        // dump all columns...
                        columnNames.ForEach(
                            col => results.AppendFormat(
                                @",""{0}""",
                                reader[col].ToString().Replace("\"", "\"\"")));

                        results.AppendLine();
                    }
                }
                else if (reader.RecordsAffected > 0)
                {
                    results.AppendFormat("{0} Record(s) Affected", reader.RecordsAffected);
                    results.AppendLine();
                }
                else
                {
                    results.AppendLine("No Results Returned.");
                }

                reader.Close();

                command.Transaction?.Commit();
            }
            finally
            {
                command.Transaction?.Rollback();
            }
        }
        catch (Exception x)
        {
            reader?.Close();

            results.AppendLine();
            results.AppendFormat("SQL ERROR: {0}", x);
        }

        return results.ToString();
    }
    public override void EnableForeignKeysCheck(IDbCommand cmd) => cmd.ExecNonQuery(SqlitePragmas.EnableForeignKeys);
    public override Task EnableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default) =>
        cmd.ExecNonQueryAsync(SqlitePragmas.EnableForeignKeys, null, token);

    public override void DisableForeignKeysCheck(IDbCommand cmd) => cmd.ExecNonQuery(SqlitePragmas.DisableForeignKeys);
    public override Task DisableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default) =>
        cmd.ExecNonQueryAsync(SqlitePragmas.DisableForeignKeys, null, token);
}

public static class SqlitePragmas
{
    public const string EnableForeignKeys = "PRAGMA foreign_keys=ON;";
    public const string DisableForeignKeys = "PRAGMA foreign_keys=OFF;";
}

/// <summary>
/// Class SqliteExtensions.
/// </summary>
public static class SqliteExtensions
{
    /// <summary>
    /// Configures the specified password.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="password">The password.</param>
    /// <param name="parseViaFramework">if set to <c>true</c> [parse via framework].</param>
    /// <param name="utf8Encoding">if set to <c>true</c> [UTF8 encoding].</param>
    /// <returns>IOrmLiteDialectProvider.</returns>
    public static IOrmLiteDialectProvider Configure(this IOrmLiteDialectProvider provider,
                                                    string password = null, bool parseViaFramework = false, bool utf8Encoding = false)
    {
        if (password != null)
            SqliteOrmLiteDialectProviderBase.Password = password;
        if (parseViaFramework)
            SqliteOrmLiteDialectProviderBase.ParseViaFramework = true;
        if (utf8Encoding)
            SqliteOrmLiteDialectProviderBase.UTF8Encoded = true;

        return provider;
    }
}